using System;
using System.Collections.Generic;
using System.Globalization;

namespace BecquerelMonitor.RoiWizard
{
    // Сборка результата в объекты BecqMoni. Файлы не пишутся: конфигурация уходит в
    // ROIConfigManager, а записи набора — в NuclideDefinitionManager, то есть инструмент
    // работает как часть приложения, а не как генератор XML на диск.
    public class SetExporter
    {
        readonly ResolutionModel resolution;

        public SetExporter(ResolutionModel resolution) : this(resolution, null)
        {
        }

        public SetExporter(ResolutionModel resolution, ZoneCalculator zones)
        {
            this.resolution = resolution;
            this.Zones = zones ?? new ZoneCalculator(resolution);
        }

        // границы зоны считает отдельный калькулятор — он же уходит в проверки
        public ZoneCalculator Zones { get; private set; }


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
                this.Zones.LimitsFor(line, out lower, out upper);

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

}
