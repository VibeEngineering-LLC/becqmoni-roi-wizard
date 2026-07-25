using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace BecquerelMonitor.RoiWizard
{
    public enum RoiStyle
    {
        // маркеры: границы −10, высота задаётся Intencity
        Markers,
        // зоны вокруг пика
        Zones,
        ZonesWithMarkers
    }

    public enum ZoneWidthMode
    {
        // процент от энергии — как задаётся ширина ROI в самом BecqMoni
        PercentOfEnergy,
        // k × FWHM по модели разрешения
        FwhmFactor
    }

    // Якорная линия: ровно одна запись набора получает IsAnchor = true. Найдя её в спектре,
    // BecqMoni сажает остальные линии набора на табличные позиции и подгоняет амплитуды
    // (библиотечный фит). Без якоря механизм не запускается вовсе.
    public static class AnchorPicker
    {
        // Хороший якорь — сильная И одинокая линия: сосед внутри FWHM смещает центроид
        // найденного пика, и совпадение с табличной энергией перестаёт быть надёжным.
        // Правило даёт 2614.5 для ряда Th-232 и 609.3 для Ra-226.
        public static SpectralLine Pick(IList<SpectralLine> lines, ResolutionModel resolution)
        {
            if (lines == null || lines.Count == 0)
            {
                return null;
            }
            double max = 0.0;
            foreach (SpectralLine line in lines)
            {
                if (line.Intensity > max)
                {
                    max = line.Intensity;
                }
            }

            SpectralLine best = null;
            SpectralLine bestLonely = null;
            foreach (SpectralLine line in lines)
            {
                if (line.Type != LineType.Gamma || line.Intensity < 0.2 * max)
                {
                    continue;
                }
                if (best == null || line.Intensity > best.Intensity)
                {
                    best = line;
                }
                if (IsLonely(line, lines, resolution, max) &&
                    (bestLonely == null || line.Intensity > bestLonely.Intensity))
                {
                    bestLonely = line;
                }
            }
            SpectralLine pick = bestLonely ?? best;
            if (pick != null)
            {
                return pick;
            }
            foreach (SpectralLine line in lines)
            {
                if (pick == null || line.Intensity > pick.Intensity)
                {
                    pick = line;
                }
            }
            return pick;
        }

        static bool IsLonely(SpectralLine line, IList<SpectralLine> lines,
                             ResolutionModel resolution, double max)
        {
            double window = resolution.Fwhm(line.Energy);
            foreach (SpectralLine other in lines)
            {
                if (!ReferenceEquals(other, line) && other.Intensity >= 0.05 * max &&
                    Math.Abs(other.Energy - line.Energy) < window)
                {
                    return false;
                }
            }
            return true;
        }
    }

    // Сборка результата в объекты BecqMoni. Файлы не пишутся: конфигурация уходит в
    // ROIConfigManager, а записи набора — в NuclideDefinitionManager, то есть инструмент
    // работает как часть приложения, а не как генератор XML на диск.
    public class SetExporter
    {
        readonly ResolutionModel resolution;

        public SetExporter(ResolutionModel resolution)
        {
            this.resolution = resolution;
        }

        public RoiStyle Style { get; set; }
        public ZoneWidthMode WidthMode { get; set; }
        public double ZonePercent { get; set; }
        public double ZoneFwhmFactor { get; set; }

        public SetExporter Reset()
        {
            this.Style = RoiStyle.Markers;
            this.WidthMode = ZoneWidthMode.PercentOfEnergy;
            this.ZonePercent = 5.0;
            this.ZoneFwhmFactor = 3.0;
            return this;
        }

        // Границы ROI. Для режима маркеров BecqMoni ожидает −10: это признак того, что
        // зоны нет, а запись рисуется штрихом высотой по Intencity.
        public void LimitsFor(SpectralLine line, out double lower, out double upper)
        {
            if (this.Style == RoiStyle.Markers)
            {
                lower = -10;
                upper = -10;
                return;
            }
            double halfWidth = this.WidthMode == ZoneWidthMode.PercentOfEnergy
                ? line.Energy * this.ZonePercent / 100.0 / 2.0
                : this.ZoneFwhmFactor * this.resolution.Fwhm(line.Energy) / 2.0;
            lower = Math.Floor(line.Energy - halfWidth);
            upper = Math.Ceiling(line.Energy + halfWidth);
        }

        public ROIConfigData BuildRoiConfig(IEnumerable<SpectralLine> lines, string name,
                                            Func<SpectralLine, Color> colorOf)
        {
            ROIConfigData config = new ROIConfigData();
            config.Guid = System.Guid.NewGuid().ToString();
            config.Name = string.IsNullOrEmpty(name) ? "IAEA lines" : name;
            config.LastUpdated = DateTime.Now;

            List<SpectralLine> ordered = Selected(lines);
            ordered.Sort(delegate(SpectralLine a, SpectralLine b)
            {
                int byLabel = string.CompareOrdinal(a.Label, b.Label);
                return byLabel != 0 ? byLabel : a.Energy.CompareTo(b.Energy);
            });

            foreach (SpectralLine line in ordered)
            {
                double lower, upper;
                this.LimitsFor(line, out lower, out upper);

                ROIDefinitionData roi = new ROIDefinitionData();
                roi.Name = line.Label;
                roi.Enabled = true;
                roi.PeakEnergy = line.Energy;
                roi.LowerLimit = lower;
                roi.UpperLimit = upper;
                roi.Color = colorOf != null ? colorOf(line) : Color.Red;
                // период полураспада в конфигурации ROI хранится в годах
                roi.HalfLife = line.HalfLifeYears >= 1e9 ? 0 : line.HalfLifeYears;
                roi.Intencity = line.Intensity;
                config.ROIDefinitions.Add(roi);
            }
            return config;
        }

        // Возвращает записи набора и сам NuclideSet. Вызывающая сторона добавляет их в
        // NuclideDefinitionManager — существующие нуклиды и наборы при этом не трогаются.
        public NuclideSet BuildNuclideSet(IEnumerable<SpectralLine> lines, string setName,
                                          Func<SpectralLine, Color> colorOf,
                                          SpectralLine anchorOverride,
                                          out List<NuclideDefinition> definitions)
        {
            List<SpectralLine> ordered = Selected(lines);
            ordered.Sort(delegate(SpectralLine a, SpectralLine b) { return a.Energy.CompareTo(b.Energy); });

            NuclideSet set = new NuclideSet();
            set.Id = System.Guid.NewGuid();
            set.Name = string.IsNullOrEmpty(setName) ? "IAEA set" : setName;
            set.HideUnknownPeaks = false;

            SpectralLine anchor = anchorOverride ?? AnchorPicker.Pick(ordered, this.resolution);
            definitions = new List<NuclideDefinition>();

            foreach (SpectralLine line in ordered)
            {
                NuclideDefinition definition = new NuclideDefinition();
                definition.Name = line.LibraryName;
                definition.Energy = line.Energy;
                // у нераспадных записей (ХРИ, вторичные) период полураспада не заполняется —
                // конвенция файла-образца BecqMoni
                definition.HalfLife = line.Type == LineType.Xrf || line.Type == LineType.Secondary
                    ? 0
                    : (line.HalfLifeYears >= 1e9 ? 0 : line.HalfLifeYears);
                definition.NuclideColor = colorOf != null ? colorOf(line) : Color.Gray;
                definition.Visible = true;
                definition.Intencity = line.Intensity;
                definition.Sets.Add(set.Id);
                definition.IsAnchor = ReferenceEquals(line, anchor);
                definitions.Add(definition);
            }
            return set;
        }

        static List<SpectralLine> Selected(IEnumerable<SpectralLine> lines)
        {
            List<SpectralLine> result = new List<SpectralLine>();
            foreach (SpectralLine line in lines)
            {
                if (line.Selected)
                {
                    result.Add(line);
                }
            }
            return result;
        }
    }

    public enum IssueLevel
    {
        Warning,
        Error
    }

    public class SetIssue
    {
        public IssueLevel Level { get; set; }
        public string Text { get; set; }
    }

    // Проверки перед сохранением. Для ROI всё совещательное, для набора совпавшие энергии
    // и нулевая интенсивность — ошибки: две линии на одной позиции вырождают подгонку
    // амплитуд (два параметра на один пик), а Intencity = 0 выбрасывает линию из связки
    // по цепочке.
    public static class SetChecker
    {
        public static List<SetIssue> Check(IEnumerable<SpectralLine> lines, bool forLibrary,
                                           SetExporter exporter)
        {
            List<SetIssue> issues = new List<SetIssue>();
            List<SpectralLine> sorted = new List<SpectralLine>();
            foreach (SpectralLine line in lines)
            {
                if (line.Selected)
                {
                    sorted.Add(line);
                }
            }
            sorted.Sort(delegate(SpectralLine a, SpectralLine b) { return a.Energy.CompareTo(b.Energy); });

            IssueLevel level = forLibrary ? IssueLevel.Error : IssueLevel.Warning;
            for (int i = 1; i < sorted.Count; i++)
            {
                if (Math.Abs(sorted[i].Energy - sorted[i - 1].Energy) < 1.0)
                {
                    issues.Add(new SetIssue
                    {
                        Level = level,
                        Text = string.Format(CultureInfo.CurrentCulture,
                            "равные энергии: «{0}» и «{1}» ({2} / {3} кэВ)",
                            Name(sorted[i - 1], forLibrary), Name(sorted[i], forLibrary),
                            sorted[i - 1].Energy, sorted[i].Energy)
                    });
                }
            }
            foreach (SpectralLine line in sorted)
            {
                if (!(line.Intensity > 0))
                {
                    issues.Add(new SetIssue
                    {
                        Level = level,
                        Text = string.Format(CultureInfo.CurrentCulture, "нулевой выход: «{0}» ({1} кэВ)",
                            Name(line, forLibrary), line.Energy)
                    });
                }
            }
            if (!forLibrary && exporter != null && exporter.Style != RoiStyle.Markers)
            {
                for (int i = 1; i < sorted.Count; i++)
                {
                    double lowerA, upperA, lowerB, upperB;
                    exporter.LimitsFor(sorted[i - 1], out lowerA, out upperA);
                    exporter.LimitsFor(sorted[i], out lowerB, out upperB);
                    if (lowerB < upperA)
                    {
                        issues.Add(new SetIssue
                        {
                            Level = IssueLevel.Warning,
                            Text = string.Format(CultureInfo.CurrentCulture,
                                "перекрытие зон: «{0}» [{1}–{2}] и «{3}» [{4}–{5}]",
                                sorted[i - 1].Label, lowerA, upperA, sorted[i].Label, lowerB, upperB)
                        });
                    }
                }
            }
            return issues;
        }

        static string Name(SpectralLine line, bool forLibrary)
        {
            return forLibrary ? line.LibraryName : line.Label;
        }
    }
}
