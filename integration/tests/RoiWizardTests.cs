using System;
using System.Collections.Generic;
using System.Drawing;
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
            ExporterTests();
            FullSetTests(catalogPath);
            GroupMemberTests(catalogPath);
            MultiAnchorTests(catalogPath);

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

            // у одиночного нуклида родителя нет, но цепочку слитая линия обязана нести:
            // иначе «Cs-137 658–664» — отдельная цепочка, не связанная с «Cs-137»
            Equal("слитая без родителя",
                Line("Cs-137", true, "658–664").LibraryName,
                "Cs-137 658–664 (Cs-137)");

            // У корня ряда и у одиночного нуклида родителя в подписи нет, поэтому линия
            // с суффиксом («U-238 X L») становилась бы собственной цепочкой и выпадала
            // из связки — цепочка дописывается явно
            SpectralLine rootXray = new SpectralLine
            {
                Key = "x|U-238|13.6", Nuclide = "U-238", Label = "U-238 X L",
                Energy = 13.6, Intensity = 5, Type = LineType.Xray
            };
            Equal("X-линия корня ряда несёт цепочку", rootXray.LibraryName, "U-238 X L (U-238)");
            SpectralLine plainGamma = new SpectralLine
            {
                Key = "g|Cs-137|661.7", Nuclide = "Cs-137", Label = "Cs-137",
                Energy = 661.7, Intensity = 85, Type = LineType.Gamma
            };
            Equal("обычная γ-линия скобок не получает", plainGamma.LibraryName, "Cs-137");
            SpectralLine xrf = new SpectralLine
            {
                Key = "xrf|Pb|74.97", Nuclide = "XRF Pb", Label = "XRF Pb Ka1",
                Energy = 74.97, Intensity = 100, Type = LineType.Xrf
            };
            Equal("ХРИ материала цепочки не получает", xrf.LibraryName, "XRF Pb Ka1");

            // X-линия члена ряда: суффикс вставляется до скобок ещё на сборке подписи,
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

        // ── 7. Экспорт: сборка объектов BecqMoni ──
        //
        // Собирается против заглушек типов хоста (HostStubs.cs) — иначе SetExporter.cs
        // остаётся единственным файлом модуля, который вне дерева BecqMoni не
        // компилируется вовсе, и туда незаметно проходят вещи вроде забытого
        // using System.Drawing.
        static void ExporterTests()
        {
            Section("SetExporter");

            ResolutionModel resolution = new ResolutionModel(7.5);
            SetExporter exporter = new SetExporter(resolution);

            SpectralLine cs = Gamma("Cs-137", 661.66, 85.1);
            SpectralLine ba = Gamma("Ba-133", 356.0, 62.1);
            SpectralLine dropped = Gamma("K-40", 1460.8, 10.55);
            dropped.Selected = false;
            SpectralLine xrf = Gamma("XRF Pb Ka1", 74.97, 100.0);
            xrf.Type = LineType.Xrf;
            List<SpectralLine> lines = new List<SpectralLine> { cs, ba, dropped, xrf };

            ROIConfigData config = exporter.BuildRoiConfig(lines, "", delegate { return Color.Red; });
            Equal("снятые галки в конфигурацию не идут",
                  config.ROIDefinitions.Count.ToString(CultureInfo.InvariantCulture), "3");
            Equal("пустое имя заменяется", config.Name, "IAEA lines");
            Check("guid проставлен", !string.IsNullOrEmpty(config.Guid));

            // режим маркеров: BecqMoni ждёт -10, это признак «зоны нет»
            foreach (ROIDefinitionData roi in config.ROIDefinitions)
            {
                if (roi.PeakEnergy == 661.66)
                {
                    Near("маркер: нижняя граница -10", roi.LowerLimit, -10, 1e-9);
                    Near("маркер: верхняя граница -10", roi.UpperLimit, -10, 1e-9);
                    Near("интенсивность перенесена", roi.Intencity, 85.1, 1e-9);
                    Near("стабильный период даёт 0", roi.HalfLife, 0, 1e-9);
                }
            }

            // режим зон: границы обязаны совпасть с ZoneCalculator, иначе проверка
            // перекрытий в форме считает одно, а в конфигурацию уходит другое
            SetExporter zoned = new SetExporter(resolution);
            zoned.Zones.Style = RoiStyle.Zones;
            zoned.Zones.WidthMode = ZoneWidthMode.PercentOfEnergy;
            zoned.Zones.ZonePercent = 5.0;
            ROIConfigData zonedConfig = zoned.BuildRoiConfig(new List<SpectralLine> { cs }, "z",
                                                            delegate { return Color.Red; });
            double lower, upper;
            zoned.Zones.LimitsFor(cs, out lower, out upper);
            Near("зона: нижняя граница как у калькулятора", zonedConfig.ROIDefinitions[0].LowerLimit, lower, 1e-9);
            Near("зона: верхняя граница как у калькулятора", zonedConfig.ROIDefinitions[0].UpperLimit, upper, 1e-9);

            List<NuclideDefinition> definitions;
            NuclideSet set = exporter.BuildNuclideSet(lines, "", delegate { return Color.Gray; },
                                                      null, out definitions);
            Equal("в наборе только выбранные",
                  definitions.Count.ToString(CultureInfo.InvariantCulture), "3");
            int anchors = 0;
            NuclideDefinition anchorDefinition = null;
            foreach (NuclideDefinition definition in definitions)
            {
                if (definition.IsAnchor)
                {
                    anchors++;
                    anchorDefinition = definition;
                }
                Check("запись отнесена к набору: " + definition.Name, definition.Sets.Contains(set.Id));
            }
            // якорей может быть несколько (LibraryPeakFitter перебирает все), но ХРИ среди
            // них быть не должно: здесь γ-линий ровно две — обе и помечаются
            Equal("якорями стали обе γ-линии", anchors.ToString(CultureInfo.InvariantCulture), "2");
            Check("сильнейшая γ-линия среди якорей",
                  anchorDefinition != null && definitions.Exists(
                      delegate(NuclideDefinition d) { return d.IsAnchor && d.Name == "Cs-137"; }));
            Check("ХРИ якорем не стал", !definitions.Exists(
                delegate(NuclideDefinition d) { return d.IsAnchor && d.Name == "XRF Pb Ka1"; }));
            foreach (NuclideDefinition definition in definitions)
            {
                if (definition.Name == "XRF Pb Ka1")
                {
                    Near("у ХРИ период полураспада не заполняется", definition.HalfLife, 0, 1e-9);
                }
            }

            // Ручной выбор якоря: экспорт слепо ставит IsAnchor на переданную линию,
            // поэтому неприемлемый якорь обязан отсекаться проверками ДО записи.
            NuclideSet forced = exporter.BuildNuclideSet(lines, "forced", delegate { return Color.Gray; },
                                                         xrf, out definitions);
            Check("ручной якорь уважается экспортом", forced != null && definitions.Exists(
                delegate(NuclideDefinition d) { return d.IsAnchor && d.Name == "XRF Pb Ka1"; }));
            Check("ХРИ ручным якорем — ошибка проверки",
                  HasError(SetChecker.Check(lines, true, null, resolution, xrf), "не линия распада"));
            Check("γ-линия ручным якорем ошибкой не считается",
                  !HasError(SetChecker.Check(lines, true, null, resolution, ba), "не линия распада"));
        }

        // ── 8. Профиль «полный набор» ──
        static void FullSetTests(string catalogPath)
        {
            Section("BuildFullSet");
            NuclideCatalog catalog = LoadCatalog(catalogPath);
            if (catalog == null)
            {
                return;
            }

            SourceSelection selection = new SourceSelection();
            selection.Add(catalog, "Th-232", AddMode.Chain);

            LineSetBuilder builder = new LineSetBuilder(catalog).Reset();
            LineFilter filter = new LineFilter { IntensityOn = true, MinIntensity = 5.0, RelativeIntensity = true };
            List<SpectralLine> filtered = builder.Build(selection, filter);
            List<SpectralLine> full = builder.BuildFullSet(selection);

            int filteredSelected = 0;
            foreach (SpectralLine line in filtered)
            {
                if (line.Selected)
                {
                    filteredSelected++;
                }
            }
            Check("фильтр отсекает часть линий (" + filteredSelected + " из " + filtered.Count + ")",
                  filteredSelected < filtered.Count);

            int fullSelected = 0;
            bool secondary = false;
            foreach (SpectralLine line in full)
            {
                if (line.Selected)
                {
                    fullSelected++;
                }
                if (line.Type == LineType.Secondary)
                {
                    secondary = true;
                }
            }
            Equal("полный набор берёт все линии до единой",
                  fullSelected.ToString(CultureInfo.InvariantCulture),
                  full.Count.ToString(CultureInfo.InvariantCulture));
            Check("полный набор не беднее отфильтрованного", fullSelected > filteredSelected);
            Check("вторичных маркеров в полном наборе нет", !secondary);

            // равновесие ряда обязано работать и здесь: веса связанных линий BecqMoni
            // берёт из Intencity, и без пересчёта высоты линий разных членов ряда несопоставимы
            foreach (SpectralLine line in full)
            {
                if (line.Nuclide == "Tl-208" && Math.Abs(line.Energy - 2614.511) < 0.2)
                {
                    Near("Tl-208 2614.5 пересчитан на распад Th-232", line.Intensity,
                         99.755 * 0.3594, 0.5);
                }
            }
        }

        // ── 8b. Групповое добавление не должно рвать цепочки ──
        //
        // Кнопка «добавить все» по ряду или по семейству добавляет членов ЕРН-рядов.
        // Если подпись такого нуклида не несёт корень ряда, ChainOf в BecqMoni считает
        // цепочкой его собственное имя — набор распадается на десятки одиночных
        // «цепочек», и связка амплитуд не собирается ни по одной.
        static void GroupMemberTests(string catalogPath)
        {
            Section("AddGroupMember");
            NuclideCatalog catalog = LoadCatalog(catalogPath);
            if (catalog == null)
            {
                return;
            }

            SourceSelection group = new SourceSelection();
            foreach (string name in new string[] { "U-238", "Ra-226", "Bi-214", "Th-232", "Tl-208", "K-40" })
            {
                group.AddGroupMember(catalog, name);
            }
            Equal("корень ряда — без скобок", group.Nuclides["U-238"], "U-238");
            Equal("член ряда несёт КОРЕНЬ, а не предшественника", group.Nuclides["Ra-226"], "Ra-226 (U-238)");
            Equal("Bi-214 тоже от корня", group.Nuclides["Bi-214"], "Bi-214 (U-238)");
            Equal("Tl-208 от Th-232", group.Nuclides["Tl-208"], "Tl-208 (Th-232)");
            Equal("вне рядов — своим именем", group.Nuclides["K-40"], "K-40");

            List<SpectralLine> lines = new LineSetBuilder(catalog).Reset().BuildFullSet(group);
            Dictionary<string, int> chains = new Dictionary<string, int>();
            foreach (SpectralLine line in lines)
            {
                string name = line.LibraryName;
                int close = name.LastIndexOf(')');
                int open = close > 0 ? name.LastIndexOf('(', close - 1) : -1;
                string chain = close == name.Length - 1 && open > 0
                    ? name.Substring(open + 1, close - open - 1)
                    : line.Nuclide;
                if (!chains.ContainsKey(chain))
                {
                    chains[chain] = 0;
                }
                chains[chain]++;
            }
            Check("цепочек ровно три: U-238, Th-232, K-40 (получено " + chains.Count + ")",
                  chains.Count == 3 && chains.ContainsKey("U-238") &&
                  chains.ContainsKey("Th-232") && chains.ContainsKey("K-40"));

            // одиночное добавление — по-прежнему без родителя: равновесный пересчёт
            // к нуклиду, взятому вне ряда, не применяется
            SourceSelection single = new SourceSelection();
            single.Add(catalog, "Tl-208", AddMode.Single);
            Equal("одиночный нуклид остаётся без скобок", single.Nuclides["Tl-208"], "Tl-208");
        }

        // ── 9. Несколько якорных линий ──
        //
        // LibraryPeakFitter перебирает все записи с IsAnchor, берёт сдвиг калибровки
        // с сильнейшей по SNR и требует совпадения с найденным пиком хотя бы одной.
        // Единственный якорь означает: не нашёлся он — молчит весь набор.
        static void MultiAnchorTests(string catalogPath)
        {
            Section("PickMany");
            NuclideCatalog catalog = LoadCatalog(catalogPath);
            if (catalog == null)
            {
                return;
            }

            ResolutionModel resolution = new ResolutionModel(7.5);
            List<SpectralLine> chain = BuildChain(catalog, "Th-232");
            List<SpectralLine> anchors = AnchorPicker.PickMany(chain, resolution, 3);

            Equal("выбрано три якоря", anchors.Count.ToString(CultureInfo.InvariantCulture), "3");
            Check("первый якорь совпадает с одиночным выбором",
                  ReferenceEquals(anchors[0], AnchorPicker.Pick(chain, resolution)));
            // Порядок в списке не глобально убывающий: одинокие линии идут раньше сильных,
            // но стоящих в дублете. Это и есть правило — сосед внутри FWHM смещает центроид
            // найденного пика, и совпадение с табличной энергией перестаёт быть надёжным.
            double maxGamma = 0.0;
            foreach (SpectralLine line in chain)
            {
                if (line.Type == LineType.Gamma && line.Intensity > maxGamma)
                {
                    maxGamma = line.Intensity;
                }
            }
            bool allGamma = true;
            bool aboveThreshold = true;
            foreach (SpectralLine anchor in anchors)
            {
                if (anchor.Type != LineType.Gamma)
                {
                    allGamma = false;
                }
                if (anchor.Intensity < 0.2 * maxGamma)
                {
                    aboveThreshold = false;
                }
            }
            Check("все якоря — γ-линии", allGamma);
            Check("все якоря сильнее 0.2·max по γ", aboveThreshold);
            Check("якоря различны", !ReferenceEquals(anchors[0], anchors[1]) &&
                                     !ReferenceEquals(anchors[1], anchors[2]));

            // выгрузка: помечены ровно те линии, что выбраны
            SetExporter exporter = new SetExporter(resolution);
            List<NuclideDefinition> definitions;
            exporter.BuildNuclideSet(chain, "Th-232 full", delegate { return Color.Gray; },
                                     null, 3, out definitions);
            int marked = 0;
            foreach (NuclideDefinition definition in definitions)
            {
                if (definition.IsAnchor)
                {
                    marked++;
                }
            }
            Equal("в выгрузке три якоря", marked.ToString(CultureInfo.InvariantCulture), "3");

            exporter.BuildNuclideSet(chain, "Th-232 single", delegate { return Color.Gray; },
                                     null, 1, out definitions);
            marked = 0;
            foreach (NuclideDefinition definition in definitions)
            {
                if (definition.IsAnchor)
                {
                    marked++;
                }
            }
            Equal("количество якорей управляемо", marked.ToString(CultureInfo.InvariantCulture), "1");

            // ХРИ и вторичные не попадают в якоря ни при каком количестве
            List<SpectralLine> mixed = new List<SpectralLine>();
            SpectralLine xrf1 = Gamma("XRF Pb Ka1", 74.97, 100.0);
            xrf1.Type = LineType.Xrf;
            SpectralLine xrf2 = Gamma("XRF Pb Kb1", 84.94, 60.0);
            xrf2.Type = LineType.Xrf;
            mixed.Add(xrf1);
            mixed.Add(xrf2);
            mixed.Add(Gamma("U-238", 49.55, 0.064));
            List<SpectralLine> mixedAnchors = AnchorPicker.PickMany(mixed, resolution, 3);
            Check("среди ХРИ якорем становится линия распада",
                  mixedAnchors.Count == 1 && mixedAnchors[0].Nuclide == "U-238");
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
                // Nuclide — имя до скобок: от него зависит цепочка, которую LibraryName
                // дописывает линии, оставшейся без родителя
                Key = label, Nuclide = label.Split(' ')[0],
                Label = merged ? label + " (" + interval + ")" : label,
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
