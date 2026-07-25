using System;
using System.Collections.Generic;
using System.Globalization;

namespace BecquerelMonitor.RoiWizard
{
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
                                           ZoneCalculator zones)
        {
            return Check(lines, forLibrary, zones, null);
        }

        // resolution нужен, чтобы проверить якорь ровно так же, как его выберет
        // BuildNuclideSet; без модели разрешения проверка якоря пропускается
        public static List<SetIssue> Check(IEnumerable<SpectralLine> lines, bool forLibrary,
                                           ZoneCalculator zones, ResolutionModel resolution)
        {
            return Check(lines, forLibrary, zones, resolution, null);
        }

        // anchorOverride — якорь, выбранный руками. Проверять надо именно его: в набор
        // уйдёт он, а не тот, что предложил бы AnchorPicker.
        public static List<SetIssue> Check(IEnumerable<SpectralLine> lines, bool forLibrary,
                                           ZoneCalculator zones, ResolutionModel resolution,
                                           SpectralLine anchorOverride)
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
            if (forLibrary && (resolution != null || anchorOverride != null))
            {
                SpectralLine anchor = anchorOverride ?? AnchorPicker.Pick(sorted, resolution);
                if (anchorOverride != null && !AnchorPicker.IsAcceptable(anchorOverride))
                {
                    issues.Add(new SetIssue
                    {
                        Level = IssueLevel.Error,
                        Text = string.Format(CultureInfo.CurrentCulture,
                            "якорем выбрана линия «{0}» ({1} кэВ): это {2}, а не линия распада. " +
                            "Фит сел бы на опору, положение или интенсивность которой условны",
                            anchorOverride.LibraryName, anchorOverride.Energy,
                            anchorOverride.Type == LineType.Xrf
                                ? "характеристический рентген материала"
                                : "расчётный вторичный маркер")
                    });
                }
                else if (anchor == null)
                {
                    issues.Add(new SetIssue
                    {
                        Level = IssueLevel.Error,
                        Text = "нет якорной линии: в наборе нет ни одной линии распада " +
                               "(ХРИ и вторичные маркеры якорем быть не могут) — " +
                               "библиотечный фит без якоря не запускается"
                    });
                }
                else if (anchor.Type != LineType.Gamma)
                {
                    issues.Add(new SetIssue
                    {
                        Level = IssueLevel.Warning,
                        Text = string.Format(CultureInfo.CurrentCulture,
                            "якорь — рентгеновская линия «{0}» ({1} кэВ): для опоры фита надёжнее γ-линия",
                            anchor.LibraryName, anchor.Energy)
                    });
                }
            }
            if (!forLibrary && zones != null && zones.Style != RoiStyle.Markers)
            {
                for (int i = 1; i < sorted.Count; i++)
                {
                    double lowerA, upperA, lowerB, upperB;
                    zones.LimitsFor(sorted[i - 1], out lowerA, out upperA);
                    zones.LimitsFor(sorted[i], out lowerB, out upperB);
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
