# Модуль «Конструктор ROI и наборов нуклидов» для BecqMoni

Порт веб-инструмента внутрь приложения: не генератор XML на диск, а часть BecqMoni,
которая кладёт результат прямо в `ROIConfigManager` и `NuclideDefinitionManager`.

Целевая платформа — та же, что у приложения: **.NET Framework 4.8**, `XmlSerializer`,
WinForms. Новых зависимостей нет: каталог ядерных данных читается тем же сериализатором,
что и остальные конфигурации BecqMoni.

## Что лежит в комплекте

```
BecquerelMonitor/RoiWizard/
  NuclideCatalog.cs     снимок IAEA/ENSDF: модель, загрузка из встроенного ресурса, индексы
  SpectralLine.cs       линия набора, модель разрешения, критерии слияния, равновесие ряда
  LineSetBuilder.cs     сбор линий по выбранным источникам, фильтры, топ-N
  LineMerger.cs         слияние близких линий под разрешение детектора
  SecondaryPeaks.cs     вторичные особенности: обратное рассеяние, край, вылеты, суммирование
  SetExporter.cs        выбор якоря, сборка ROIConfigData и NuclideSet, проверки перед сохранением
  RoiWizardForm.cs           окно инструмента: три вкладки, обработчики, русские подписи
  RoiWizardForm.Designer.cs  разметка формы (XPTable, как в NuclideSetForm)
  nuclides.xml          сам снимок (121 нуклид, 1222 γ, 327 X, 3 ряда, 10 элементов ХРИ; 96 КБ)
tools/
  export_catalog.py     пересборка nuclides.xml из data/nuclides.js и data/xrf.js
```

Ядро не зависит от формы: его можно вызвать из любого окна или использовать без UI вовсе.

## Как подключить

1. Скопировать папку `BecquerelMonitor/RoiWizard/` в дерево проекта.
2. В `BecquerelMonitor.csproj` добавить исходники и ресурс:

```xml
<Compile Include="RoiWizard\NuclideCatalog.cs" />
<Compile Include="RoiWizard\SpectralLine.cs" />
<Compile Include="RoiWizard\LineSetBuilder.cs" />
<Compile Include="RoiWizard\LineMerger.cs" />
<Compile Include="RoiWizard\SecondaryPeaks.cs" />
<Compile Include="RoiWizard\SetExporter.cs" />
<Compile Include="RoiWizard\RoiWizardForm.cs">
  <SubType>Form</SubType>
</Compile>
<Compile Include="RoiWizard\RoiWizardForm.Designer.cs">
  <DependentUpon>RoiWizardForm.cs</DependentUpon>
</Compile>
<EmbeddedResource Include="RoiWizard\nuclides.xml" />
```

Имя ресурса должно получиться `BecquerelMonitor.RoiWizard.nuclides.xml` — это значение
по умолчанию для корневого пространства имён `BecquerelMonitor`; оно захардкожено в
`NuclideCatalog.ResourceName`.

3. Собрать. Всё, что нужно из приложения, — уже существующие типы `ROIConfigData`,
`ROIDefinitionData`, `NuclideDefinition`, `NuclideSet`, `SerializableColor`.

## Минимальный сценарий

```csharp
using BecquerelMonitor.RoiWizard;

var catalog = NuclideCatalog.GetInstance();

// 1. источники: ряд тория цепочкой + свинец защиты
var selection = new SourceSelection();
selection.Add(catalog, "Th-232", AddMode.Chain);
selection.XrfElements.Add("Pb");

// 2. линии с фильтром по интенсивности
var filter = new LineFilter { IntensityOn = true, MinIntensity = 3.0, RelativeIntensity = true };
var lines = new LineSetBuilder(catalog).Reset().Build(selection, filter);

// 3. слияние под разрешение детектора
var resolution = new ResolutionModel(7.5);                       // R, % на 662 кэВ
var merger = LineMerger.For(resolution, MergeCriterion.Sparrow); // 0.85·FWHM для маркеров
lines = merger.Merge(lines);

// 4. вторичные пики (положения с поправками по измерениям)
lines.AddRange(SecondaryPeaks.Generate(lines, resolution,
    SecondaryKind.Backscatter | SecondaryKind.ComptonEdge | SecondaryKind.SingleEscape, 10.0));

// 5. проверки и выгрузка
var exporter = new SetExporter(resolution).Reset();
foreach (var issue in SetChecker.Check(lines, false, exporter))       // false = проверка для ROI
    Console.WriteLine(issue.Level + ": " + issue.Text);

ROIConfigData built = exporter.BuildRoiConfig(lines, "Th-232 chain", line => Color.OrangeRed);

// регистрирует менеджер: CreateConfig заполняет и список, и карту по guid, и шлёт событие.
// Просто добавить в ROIConfigList нельзя — SaveConfig начинается с roiConfigMap[Guid].
var roiManager = ROIConfigManager.GetInstance();
ROIConfigData config = roiManager.CreateConfig("Th-232 chain.xml");
config.Name = built.Name;
config.ROIDefinitions.AddRange(built.ROIDefinitions);
roiManager.SaveConfig(config);

List<NuclideDefinition> definitions;
NuclideSet set = exporter.BuildNuclideSet(lines, "Th-232 (IAEA)", line => Color.OrangeRed,
                                          null, out definitions);      // null = якорь выбрать автоматически
var nuclides = NuclideDefinitionManager.GetInstance();
nuclides.NuclideSets.Add(set);
nuclides.NuclideDefinitions.AddRange(definitions);
nuclides.SaveDefinitionFile();
```

## Форма

`RoiWizardForm` — окно из трёх вкладок, повторяющих шаги веб-версии: изотопы, линии,
оформление и экспорт. Разметка собрана руками в `RoiWizardForm.Designer.cs`, таблицы — на
`XPTable` (как в `NuclideSetForm`), подписи по умолчанию английские, русские накладываются
в `ApplyRussian()` по текущей культуре UI. Штатный `Localizable = true` с `.resx` намеренно
не используется: держать координаты контролов в ресурсах ради двух языков дороже словаря
подписей — если понадобится, форму можно открыть в дизайнере и включить локализацию.

Подключение к меню приложения:

```csharp
// MainForm.cs — рядом с ShowNuclideSetForm()
public void ShowRoiWizardForm()
{
    using (var form = new RoiWizard.RoiWizardForm(this.RoiWizardResolution))
    {
        form.ShowDialog(this);
    }
}

// Разрешение из FWHM-калибровки активного спектра. Вернуть 0, если взять неоткуда —
// форма тогда просто оставит значение, введённое руками.
double RoiWizardResolution()
{
    ResultData active = this.ActiveResultData;
    if (active == null || active.EnergySpectrum == null) return 0;

    // выбрать диапазон каналов вокруг опорного пика (проще всего — вокруг активного ROI)
    EnergyResolutionResult result =
        EnergyResolutionCalculator.CalculateFWHM(active.EnergySpectrum, startChannel, endChannel);
    return result != null ? result.Resolution : 0;
}
```

Пункт меню добавляется там же, где `NuclideSetForm` и `NuclideDefinitionForm` — обработчик
вызывает `ShowRoiWizardForm()`. Конструктор без аргументов тоже есть: тогда кнопка
«из спектра» на втором шаге просто выключена, и форма работает автономно.

Что делает форма по кнопкам:

| Кнопка | Действие |
|---|---|
| **Создать ROI-конфигурацию** | собирает `ROIConfigData`, кладёт в `ROIConfigManager.ROIConfigList` и вызывает `SaveConfig`. Имя файла — из имени конфигурации, недопустимые символы заменяются |
| **Добавить набор в библиотеку** | собирает `NuclideSet` и записи `NuclideDefinition`, добавляет в `NuclideDefinitionManager` и вызывает `SaveDefinitionFile()`. Существующие нуклиды и наборы не трогаются |
| **Объединить близкие** | слияние под текущее R и критерий; «Вернуть исходные» откатывает к состоянию до слияния |

Проверки перед сохранением встроены: для ROI выводится вопрос «сохранить всё равно?», для
набора совпавшие энергии и нулевая интенсивность останавливают запись с объяснением.

## Точки сращивания с приложением

Ради них порт и делается — в вебе это невозможно.

| Что | Как получить из хоста |
|---|---|
| **R детектора** вместо ручного ввода | `EnergyResolutionCalculator.CalculateFWHM(spectrum, start, end)` → `EnergyResolutionResult.Resolution`. Тогда пороги слияния считаются по реальной калибровке текущего спектра, а не по паспорту |
| **Список нуклидов** вместо ручного выбора | взять из результатов поиска пиков (`PeakDetector`, `ResultData`) — те, что уже опознаны в открытом спектре |
| **Применение результата** | `ROIConfigManager.GetInstance().SaveConfig(config)` и `MainForm.ShowROIConfigForm(config)`; набор — `NuclideDefinitionManager.SaveDefinitionFile()` после добавления записей |
| **Живой предпросмотр** | границы `ROIDefinitionData.LowerLimit/UpperLimit` можно рисовать поверх открытого спектра до сохранения |

## Что важно не сломать

- **Ровно один `IsAnchor = true` на набор.** Без якоря `LibraryPeakFitter` не стартует.
  Автовыбор — `AnchorPicker.Pick`: сильная и одинокая линия (для ряда Th-232 это 2614,5 кэВ,
  для Ra-226 — 609,3).
- **Скобки в имени — это цепочка.** `ChainOf` в `LibraryPeakFitter` читает текст в последних
  скобках имени как имя родителя. Поэтому у слитой линии интервал энергий выносится наружу:
  `SpectralLine.LibraryName` даёт «Ac-228 964.8–969.0 (Th-232)», а не
  «Ac-228 (Th-232) (964.8–969.0)» — иначе цепочкой считалось бы «964.8–969.0» и связка
  амплитуд не собиралась бы.
- **`Intencity > 0` у всех членов набора.** Линия с нулевой интенсивностью выпадает из
  связки по цепочке — `SetChecker` считает это ошибкой для набора и предупреждением для ROI.
- **Пороги слияния разные для разных задач.** 0,85·FWHM (предел Sparrow) — для ROI-маркеров;
  0,25·FWHM — для наборов в библиотеку, потому что пары от 0,25 до 0,85 FWHM разбирает сам
  библиотечный фит, и слияние их только обедняет набор.

## Обновление данных

`nuclides.xml` пересобирается из снимка веб-инструмента:

```
python integration/tools/export_catalog.py
```

Сам снимок обновляется `update_nuclides.py` (запрос к IAEA Live Chart; API не отдаёт CORS,
поэтому обновление скриптом, а не из страницы).

## Откуда числа

Пороги слияния, требование якоря и гейт значимости Fisher z ≥ 4 — из
`BecquerelMonitor/LibraryPeakFitter.cs` самого BecqMoni, не подобраны.

Поправки к положениям вторичных пиков (комптон-край −0,8·FWHM, обратное рассеяние +10 кэВ,
доли площадей 8 / 6 / 6 %) выведены из измерений комплекса Gamma-1C (NaI(Tl) 63×63 #0086-16,
поверка 2024, защита Pb 50 мм с вкладышем Cd/Cu): 31 надёжная запись из 41. Физическое
основание — Knoll, «Radiation Detection and Measurement», гл. 10 (комптоновский край —
разд. II.B, обратное рассеяние и пики вылета — разд. III) и Gilmore, «Practical Gamma-ray
Spectrometry», разд. 9.2.3 (поиск пиков по второй производной). Разбор со страницами —
`docs/REFERENCES-secondary-peaks.md` в корне репозитория.
