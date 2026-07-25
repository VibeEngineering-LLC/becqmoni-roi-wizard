using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using BecquerelMonitor.RoiWizard;

namespace BecquerelMonitor.RoiWizard.Tests
{
    // Тесты инвариантов, поломка которых тиха и фатальна: сборка проходит, набор
    // выгружается, а связка амплитуд в BecqMoni молча не собирается.
    //
    // Зависимостей нет намеренно: тестовый фреймворк в решение BecqMoni не тянется,
    // это обычное консольное приложение. Собирается вместе с ядром модуля
    // (см. run_tests.cmd), файлы формы и SetExporter не нужны — они завязаны на типы
    // хоста, а проверяется расчётная часть.
    public static class Program
    {
        static int failed;
        static int passed;

        public static int Main(string[] args)
        {
            string catalogPath = args.Length > 0
                ? args[0]
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nuclides.xml");

            LibraryNameTests();
            MergerTests();
            EquilibriumTests();
            EquilibriumScopeTests(catalogPath);
            AnchorTests(catalogPath);
            SetTests(catalogPath);
            SecondaryTests();

            Console.WriteLine();
            Console.WriteLine("пройдено: {0}, провалено: {1}", passed, failed);
            return failed == 0 ? 0 : 1;
        }

        // ── 1. LibraryName: последние скобки — всегда родитель, либо скобок нет ──
        static void LibraryNameTests()
        {
            Section("LibraryName");

            Equal("слитая с родителем",
                Line("Ac-228 (Th-232)", true, "964.8–969.0").LibraryName,
                "Ac-228 964.8–969.0 (Th-232)");

            Equal("слитая без родителя",
                Line("Cs-137", true, "658–664").LibraryName,
                "Cs-137 658–664");

            // X-линия: суффикс вставляется до скобок ещё на сборке подписи,
            // поэтому родитель остаётся последним и после слияния
            string xrayLabel = LineSetBuilder.WithSuffix("Ac-228 (Th-232)", "X KA1");
            Equal("подпись X-линии", xrayLabel, "Ac-228 X KA1 (Th-232)");
            Equal("слитая X-линия",
                Line(xrayLabel, true, "93.3–93.9").LibraryName,
                "Ac-228 X KA1 93.3–93.9 (Th-232)");

            Equal("не слитая — как есть",
                Line("Ac-228 (Th-232)", false, null).LibraryName,
                "Ac-228 (Th-232)");

            // сам инвариант, а не отдельные строки
            string[] labels = { "Cs-137", "Ac-228 (Th-232)", "Ac-228 X KA1 (Th-232)", "XRF Pb Ka1" };
            foreach (string label in labels)
            {
                foreach (bool merged in new[] { false, true })
                {
                    string name = Line(label, merged, merged ? "100–110" : null).LibraryName;
                    Check("интервал не в скобках: " + name, !name.Contains("(100–110)"));
                    int close = name.LastIndexOf(')');
                    if (close == name.Length - 1)
                    {
                        int open = name.LastIndexOf('(', close - 1);
                        string inside = name.Substring(open + 1, close - open - 1);
                        Check("в последних скобках родитель, а не интервал: " + name,
                              inside.IndexOf('–') < 0);
                    }
                }
            }
        }

        // ── 2. Слияние: порог меряется от первой линии группы ──
        static void MergerTests()
        {
            Section("LineMerger");

            // группа Ac-228 из комментария в коде: single-linkage склеил бы 964.8…988.6
            List<SpectralLine> lines = new List<SpectralLine>
            {
                Gamma("Ac-228", 911.204, 25.8),
                Gamma("Ac-228", 964.766, 4.99),
                Gamma("Ac-228", 968.971, 15.8),
                Gamma("Ac-228", 988.63, 0.19)
            };

            ResolutionModel r75 = new ResolutionModel(7.5);
            LineMerger sparrow = new LineMerger(r75, MergeCriterionInfo.SparrowFwhm);
            List<SpectralLine> merged = sparrow.Merge(lines);

            Equal("групп после Sparrow", merged.Count.ToString(CultureInfo.InvariantCulture), "2");
            SpectralLine group = merged[1];
            Check("группа шире порога не склеена: " + group.Interval,
                  group.Energy - 964.766 <= MergeCriterionInfo.SparrowFwhm * r75.Fwhm(group.Energy) + 1e-9);
            Near("сумма интенсивностей", group.Intensity, 4.99 + 15.8 + 0.19, 1e-6);
            Near("центроид взвешен по I", group.Energy,
                 (964.766 * 4.99 + 968.971 * 15.8 + 988.63 * 0.19) / (4.99 + 15.8 + 0.19), 0.01);

            LineMerger anchored = new LineMerger(r75, MergeCriterionInfo.ClaimToleranceFwhm);
            Equal("при 0.25·FWHM группа распадается",
                  anchored.Merge(lines).Count.ToString(CultureInfo.InvariantCulture), "3");

            // вторичные маркеры не сливаются
            List<SpectralLine> withSecondary = new List<SpectralLine>(lines);
            withSecondary.Add(new SpectralLine
            {
                Key = "sec", Nuclide = "Ac-228", Label = "CE (Ac-228 969)",
                Energy = 968.0, Intensity = 1.0, Type = LineType.Secondary
            });
            List<SpectralLine> mergedWithSec = new LineMerger(r75, MergeCriterionInfo.SparrowFwhm).Merge(withSecondary);
            int secCount = 0;
            foreach (SpectralLine line in mergedWithSec)
            {
                if (line.Type == LineType.Secondary)
                {
                    secCount++;
                }
            }
            Equal("вторичный не поглощён слиянием", secCount.ToString(CultureInfo.InvariantCulture), "1");
        }

        // ── 3. Коэффициенты равновесия ряда ──
        static void EquilibriumTests()
        {
            Section("EquilibriumFactors");
            Near("Tl-208", EquilibriumFactors.For("Tl-208"), 0.3594, 1e-9);
            Near("Po-212", EquilibriumFactors.For("Po-212"), 0.6406, 1e-9);
            Near("Th-227", EquilibriumFactors.For("Th-227"), 0.9862, 1e-9);
            Near("Fr-223", EquilibriumFactors.For("Fr-223"), 0.0138, 1e-9);
            Near("Tl-207", EquilibriumFactors.For("Tl-207"), 0.99724, 1e-9);
            Near("неизвестный нуклид", EquilibriumFactors.For("Cs-137"), 1.0, 1e-9);
            // сумма по развилке Bi-212 должна давать единицу
            Near("Tl-208 + Po-212 = 1",
                 EquilibriumFactors.For("Tl-208") + EquilibriumFactors.For("Po-212"), 1.0, 1e-9);
        }

        // ── 3b. Равновесие применяется только к нуклиду, взятому в составе ряда ──
        static void EquilibriumScopeTests(string catalogPath)
        {
            Section("Равновесие: область применения");
            NuclideCatalog catalog = LoadCatalog(catalogPath);
            if (catalog == null)
            {
                return;
            }
            LineSetBuilder builder = new LineSetBuilder(catalog).Reset();

            // Tl-208 сам по себе: родителя в наборе нет, множитель применяться не должен
            SourceSelection single = new SourceSelection();
            single.Add(catalog, "Tl-208", AddMode.Single);
            Near("одиночный Tl-208: 2614 кэВ = табличные 99.75 %",
                 IntensityAt(builder.Build(single, null), 2614.51), 99.75, 0.3);

            // тот же Tl-208 в составе ряда: 99.75 × 0.3594
            SourceSelection chain = new SourceSelection();
            chain.Add(catalog, "Th-232", AddMode.Chain);
            Near("в ряду Th-232: 2614 кэВ = 99.75 × 0.3594",
                 IntensityAt(builder.Build(chain, null), 2614.51), 99.75 * 0.3594, 0.3);

            builder.ScaleToSeriesParent = false;
            Near("с выключенным равновесием — снова табличные",
                 IntensityAt(builder.Build(chain, null), 2614.51), 99.75, 0.3);
        }

        static double IntensityAt(List<SpectralLine> lines, double energy)
        {
            foreach (SpectralLine line in lines)
            {
                if (Math.Abs(line.Energy - energy) < 0.5)
                {
                    return line.Intensity;
                }
            }
            return -1;
        }

        // ── 4. Якорь: правило «сильная и одинокая», без ХРИ и вторичных ──
        static void AnchorTests(string catalogPath)
        {
            Section("AnchorPicker");
            NuclideCatalog catalog = LoadCatalog(catalogPath);
            if (catalog == null)
            {
                return;
            }
            ResolutionModel r = new ResolutionModel(7.5);

            SpectralLine thorium = AnchorPicker.Pick(BuildChain(catalog, "Th-232"), r);
            Near("ряд Th-232 → 2614.5", thorium == null ? 0 : thorium.Energy, 2614.51, 0.6);

            SpectralLine radium = AnchorPicker.Pick(BuildChain(catalog, "Ra-226"), r);
            Near("ряд Ra-226 → 609.3", radium == null ? 0 : radium.Energy, 609.31, 0.6);

            // сценарий из аудита: слабо-γ нуклид рядом с ХРИ (условная интенсивность 100)
            SourceSelection mixed = new SourceSelection();
            mixed.Add(catalog, "U-238", AddMode.Single);
            mixed.XrfElements.Add("Pb");
            List<SpectralLine> lines = new LineSetBuilder(catalog).Reset().Build(mixed, null);
            SpectralLine anchor = AnchorPicker.Pick(lines, r);
            Check("якорь не ХРИ и не вторичный", anchor == null || AnchorPicker.IsAcceptable(anchor));
            Check("якорь — линия распада: " + (anchor == null ? "нет" : anchor.Label),
                  anchor != null && anchor.Type != LineType.Xrf && anchor.Type != LineType.Secondary);

            // только ХРИ: якоря быть не должно, и проверка обязана это поймать
            SourceSelection onlyXrf = new SourceSelection();
            onlyXrf.XrfElements.Add("Pb");
            List<SpectralLine> xrfLines = new LineSetBuilder(catalog).Reset().Build(onlyXrf, null);
            Check("на одних ХРИ якоря нет", AnchorPicker.Pick(xrfLines, r) == null);
            Check("проверка набора ловит отсутствие якоря",
                  HasError(SetChecker.Check(xrfLines, true, null, r), "якорной линии"));
        }

        // ── 5. Набор: ровно один IsAnchor, ненулевые интенсивности, цепочка в скобках ──
        static void SetTests(string catalogPath)
        {
            Section("Набор для библиотеки");
            NuclideCatalog catalog = LoadCatalog(catalogPath);
            if (catalog == null)
            {
                return;
            }
            List<SpectralLine> lines = BuildChain(catalog, "Th-232");
            ResolutionModel r = new ResolutionModel(7.5);

            SpectralLine anchor = AnchorPicker.Pick(lines, r);
            int anchors = 0;
            int zeroIntensity = 0;
            int withChain = 0;
            foreach (SpectralLine line in lines)
            {
                if (ReferenceEquals(line, anchor))
                {
                    anchors++;
                }
                if (!(line.Intensity > 0))
                {
                    zeroIntensity++;
                }
                string name = line.LibraryName;
                if (name.EndsWith("(Th-232)", StringComparison.Ordinal))
                {
                    withChain++;
                }
            }
            Equal("ровно один якорь", anchors.ToString(CultureInfo.InvariantCulture), "1");
            Check("якорь — γ-линия", anchor != null && anchor.Type == LineType.Gamma);
            Equal("нулевых интенсивностей нет", zeroIntensity.ToString(CultureInfo.InvariantCulture), "0");
            Check("цепочка сохранена в именах (" + withChain + " записей)", withChain > 50);

            // совпадение энергий для набора — ошибка, для ROI — предупреждение
            List<SpectralLine> pair = new List<SpectralLine>
            {
                Gamma("A", 100.0, 10.0),
                Gamma("B", 100.4, 10.0)
            };
            Check("для набора совпадение энергий — ошибка",
                  HasError(SetChecker.Check(pair, true, null, null), "равные энергии"));
            Check("для ROI совпадение энергий — предупреждение",
                  !HasError(SetChecker.Check(pair, false, null, null), "равные энергии"));
        }

        // ── 6. Вторичные пики: формулы и поправки ──
        static void SecondaryTests()
        {
            Section("SecondaryPeaks");
            const double me = 510.999;
            Near("обратное рассеяние 661.66", SecondaryPeaks.BackscatterEnergy(661.66),
                 661.66 / (1 + 2 * 661.66 / me), 1e-9);
            Near("комптон-край 661.66", SecondaryPeaks.ComptonEdgeEnergy(661.66),
                 661.66 - 661.66 / (1 + 2 * 661.66 / me), 1e-9);

            ResolutionModel r = new ResolutionModel(8.6);
            List<SpectralLine> parent = new List<SpectralLine> { Gamma("Cs-137", 661.66, 85.1) };
            List<SpectralLine> secondary = SecondaryPeaks.Generate(parent, r,
                SecondaryKind.Backscatter | SecondaryKind.ComptonEdge, 10.0);
            Equal("сгенерировано две особенности",
                  secondary.Count.ToString(CultureInfo.InvariantCulture), "2");

            double edge = SecondaryPeaks.ComptonEdgeEnergy(661.66);
            foreach (SpectralLine line in secondary)
            {
                if (line.Label.StartsWith("BS", StringComparison.Ordinal))
                {
                    // измерения Gamma-1C: центроид выше аналитического
                    Near("BS сдвинут вверх на 10 кэВ", line.Energy,
                         SecondaryPeaks.BackscatterEnergy(661.66) + 10.0, 0.01);
                    Near("доля BS 8 %", line.Intensity, 85.1 * 0.08, 0.01);
                }
                else
                {
                    Near("CE сдвинут вниз на 0.8·FWHM", line.Energy,
                         edge - 0.8 * r.Fwhm(edge), 0.01);
                    Near("доля CE 6 %", line.Intensity, 85.1 * 0.06, 0.01);
                }
            }
        }

        // ── вспомогательное ────────────────────────────────────────────────

        static NuclideCatalog LoadCatalog(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("  ПРОПУЩЕНО: не найден каталог {0}", path);
                failed++;
                return null;
            }
            using (FileStream stream = File.OpenRead(path))
            {
                return NuclideCatalog.Load(stream);
            }
        }

        static List<SpectralLine> BuildChain(NuclideCatalog catalog, string root)
        {
            SourceSelection selection = new SourceSelection();
            selection.Add(catalog, root, AddMode.Chain);
            return new LineSetBuilder(catalog).Reset().Build(selection, null);
        }

        static SpectralLine Line(string label, bool merged, string interval)
        {
            return new SpectralLine
            {
                Key = label, Nuclide = "X", Label = merged ? label + " (" + interval + ")" : label,
                Merged = merged, Interval = interval, Energy = 100, Intensity = 1
            };
        }

        static SpectralLine Gamma(string nuclide, double energy, double intensity)
        {
            return new SpectralLine
            {
                Key = nuclide + "|" + energy, Nuclide = nuclide, Label = nuclide,
                Energy = energy, Intensity = intensity, Type = LineType.Gamma
            };
        }

        static bool HasError(List<SetIssue> issues, string fragment)
        {
            foreach (SetIssue issue in issues)
            {
                if (issue.Level == IssueLevel.Error && issue.Text.IndexOf(fragment, StringComparison.Ordinal) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        static void Section(string name)
        {
            Console.WriteLine();
            Console.WriteLine("── {0} ──", name);
        }

        static void Check(string what, bool condition)
        {
            Report(what, condition, null);
        }

        static void Equal(string what, string actual, string expected)
        {
            Report(what, actual == expected, "получено «" + actual + "», ожидалось «" + expected + "»");
        }

        static void Near(string what, double actual, double expected, double tolerance)
        {
            Report(what, Math.Abs(actual - expected) <= tolerance,
                   string.Format(CultureInfo.InvariantCulture, "получено {0}, ожидалось {1} ± {2}",
                                 actual, expected, tolerance));
        }

        static void Report(string what, bool ok, string detail)
        {
            if (ok)
            {
                passed++;
                Console.WriteLine("  ok   {0}", what);
            }
            else
            {
                failed++;
                Console.WriteLine("  FAIL {0}{1}", what, detail == null ? "" : " — " + detail);
            }
        }
    }
}
