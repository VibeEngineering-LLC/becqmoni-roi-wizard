# Модуль «Конструктор ROI и наборов нуклидов» для BecqMoni

Порт веб-инструмента внутрь приложения: не генератор XML на диск, а часть BecqMoni,
которая кладёт результат прямо в `ROIConfigManager` и `NuclideDefinitionManager`.

Целевая платформа — та же, что у приложения: **.NET Framework 4.8**, `XmlSerializer`,
WinForms. Новых зависимостей нет — используется только то, что в проекте уже есть:
`XmlSerializer` (каталог ядерных данных читается тем же сериализатором, что и остальные
конфигурации BecqMoni), `XPTable` для таблиц и `DockPanelSuite` для окна.

**Как выглядит окно — [docs/SCREENSHOTS.md](../docs/SCREENSHOTS.md):** все три шага
в разных состояниях и окно справки, снято с живой формы, собранной вместе
с `BecquerelMonitor.exe`.

## Что лежит в комплекте

```
BecquerelMonitor/RoiWizard/
  NuclideCatalog.cs     снимок IAEA/ENSDF: модель, загрузка из встроенного ресурса, индексы
  SpectralLine.cs       линия набора, модель разрешения, критерии слияния, равновесие ряда
  LineSetBuilder.cs     сбор линий по выбранным источникам, фильтры, топ-N
  LineMerger.cs         слияние близких линий под разрешение детектора
  SecondaryPeaks.cs     вторичные особенности: обратное рассеяние, край, вылеты, суммирование
  AnchorPicker.cs       выбор якорной линии (сильная и одинокая γ, без ХРИ и вторичных)
  ZoneCalculator.cs     границы ROI-зоны
  SetChecker.cs         проверки перед сохранением: совещательные для ROI, блокирующие для набора
  SetExporter.cs        сборка ROIConfigData и NuclideSet — единственный файл, зависящий от типов хоста
  WizardTheme.cs        палитра и шрифт из styles/becqmoni.css, обход дерева контролов
  CatalogCellRenderers.cs  свои ячейки XPTable: бейджи семейств и типов, счётчики γ/X,
                        микро-бар интенсивности (штатная ячейка знает один цвет)
  RoiWizardForm.cs           окно инструмента: три вкладки, обработчики, раскладка
  RoiWizardForm.Designer.cs  разметка формы (XPTable, как в NuclideSetForm)
  HelpForm.cs           окно справки: разбор того же подмножества разметки, что на странице
  RoiWizardStrings.Designer.cs  доступ к таблице строк (генерируется tools/gen_strings.py)
  nuclides.xml          сам снимок (121 нуклид, 1222 γ, 327 X, 3 ряда, 10 элементов ХРИ; 101 КБ)
  help.xml              текст справки на двух языках, выгружен из index.html
  RoiWizardStrings.resx        подписи интерфейса, нейтральная (английская) таблица
  RoiWizardStrings.ru.resx     русский перевод; MSBuild собирает из него сателлит
host-patch/
  apply_patch.py        встраивание в дерево BecqMoni: файлы, .csproj, пункт меню, ресурсы
  README.md             то же самое построчно, если применять руками
tools/
  export_catalog.py     пересборка nuclides.xml из data/nuclides.js и data/xrf.js
  export_help.py        пересборка help.xml из index.html (оба языка)
  gen_strings.py        таблица подписей: .resx, .ru.resx и доступ к ним
tests/
  RoiWizardTests.cs     тесты инвариантов, run_tests.cmd — сборка и прогон
  HostStubs.cs          заглушки типов BecqMoni: только для тестовой сборки,
                        в проект приложения НЕ добавлять (конфликт имён)
```

Ядро не зависит от формы: его можно вызвать из любого окна или использовать без UI вовсе.

## Как подключить

Одной командой — всё, что описано ниже, плюс пункт меню и подписи к нему:

```
python integration\host-patch\apply_patch.py <путь к дереву BecqMoni>
```

Скрипт идемпотентен и ничего не меняет в существующих строках, только добавляет.
Руками — по шагам:

1. Скопировать папку `BecquerelMonitor/RoiWizard/` в дерево проекта.
2. В `BecquerelMonitor.csproj` добавить исходники и ресурс:

```xml
<Compile Include="RoiWizard\NuclideCatalog.cs" />
<Compile Include="RoiWizard\SpectralLine.cs" />
<Compile Include="RoiWizard\LineSetBuilder.cs" />
<Compile Include="RoiWizard\LineMerger.cs" />
<Compile Include="RoiWizard\SecondaryPeaks.cs" />
<Compile Include="RoiWizard\AnchorPicker.cs" />
<Compile Include="RoiWizard\ZoneCalculator.cs" />
<Compile Include="RoiWizard\SetChecker.cs" />
<Compile Include="RoiWizard\SetExporter.cs" />
<Compile Include="RoiWizard\WizardTheme.cs" />
<Compile Include="RoiWizard\CatalogCellRenderers.cs" />
<Compile Include="RoiWizard\RoiWizardStrings.Designer.cs">
  <AutoGen>True</AutoGen>
  <DependentUpon>RoiWizardStrings.resx</DependentUpon>
</Compile>
<Compile Include="RoiWizard\HelpForm.cs">
  <SubType>Form</SubType>
</Compile>
<Compile Include="RoiWizard\RoiWizardForm.cs">
  <SubType>Form</SubType>
</Compile>
<Compile Include="RoiWizard\RoiWizardForm.Designer.cs">
  <DependentUpon>RoiWizardForm.cs</DependentUpon>
</Compile>
<EmbeddedResource Include="RoiWizard\nuclides.xml" />
<EmbeddedResource Include="RoiWizard\help.xml" />
<EmbeddedResource Include="RoiWizard\RoiWizardStrings.resx" />
<EmbeddedResource Include="RoiWizard\RoiWizardStrings.ru.resx" />
```

Имя ресурса должно получиться `BecquerelMonitor.RoiWizard.nuclides.xml` — это значение
по умолчанию для корневого пространства имён `BecquerelMonitor`; оно захардкожено в
`NuclideCatalog.ResourceName`.

Папку `tests/` в проект добавлять не нужно: `HostStubs.cs` объявляет те же типы, что
приходят из самого приложения, и сборка встанет на `CS0101`.

3. Собрать. Всё, что нужно из приложения, — уже существующие типы `ROIConfigData`,
`ROIDefinitionData`, `NuclideDefinition`, `NuclideSet`, `SerializableColor`.

Проверено на чистом клоне `master` (`d1eab74`, 26.07.2026): `apply_patch.py` → `restore` →
`Rebuild Release` даёт **ноль ошибок**; единственное предупреждение (`MSB3327`, нет
сертификата подписи ClickOnce) воспроизводится и на дереве без модуля.

### Чем модуль сцеплён с приложением

Всего три точки — ничего, кроме них, в хосте не задействовано:

| Точка | Где | Если делать иначе |
|---|---|---|
| `MainForm.dockPanel1` | показ окна: `Show(dockPanel1, bounds)` | **`RoiWizardForm` наследует `DockContent`, а тот — обычный `Form`, поэтому `form.ShowDialog(this)` работает без единой правки в модуле.** Тогда теряется только стыковка/группировка, всё остальное — как есть |
| `ROIConfigManager`, `NuclideDefinitionManager` | запись результата | других способов принять результат у приложения нет |
| `EnergyResolutionCalculator` / `FwhmCalibration` | `Func<double>` в конструкторе | конструктор без аргументов: кнопка «из спектра» выключается, форма работает автономно |

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
// ядро отдаёт код замечания и подстановки, фразу собирает вызывающий: тексты
// проверок переводятся, и в ядре им делать нечего
foreach (var issue in SetChecker.Check(lines, false, exporter))       // false = проверка для ROI
    Console.WriteLine(issue.Level + ": " + issue.Kind);

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
оформление и экспорт. Шаг 1 повторяет и раскладку: «Поиск изотопа» | «Группа» |
«ХРИ — элементы» в ряд, «Выбрано» — полосой снизу.

**Окно — `DockContent` из DockPanelSuite**, то есть родная док-панель BecqMoni:
открывается плавающей поверх главного окна, пристыковывается к любому краю,
группируется вкладкой с другими панелями и убирается в автоскрытие булавкой —
полоску заголовка и кнопки рисует та же `VS2015BlueTheme`, что у «Обнаружения
пиков». Окно немодальное: спектр остаётся доступным, пока мастер открыт.
Закрытие панели прячет её (`HideOnClose`), так что выбранные источники и
настройки переживают переоткрытие из меню. Окно справки в док-систему не входит —
ему полоску в стиле панелей (высота, палитра, кнопки, окантовка `ToolWindowBorder`)
рисует `WizardTheme.ApplyCaption`.

**Галочка в списке членов группы — это выбор цели**, а не только признак «взят».
«+ линии семейства» и «+ цепочкой» применяются к отмеченным нуклидам; если не отмечено
ничего — ко всей группе. «Добавить все» всегда работает по группе. Подпись под списком
говорит, что именно произойдёт. Кнопки раскрытия выключаются, когда раскрывать нечего:
у члена ЕРН-ряда родитель задан самим рядом, и подменять его предшественником нельзя —
цепочка развалится (ровно этот дефект и был в обеих версиях). Разметка собрана руками в `RoiWizardForm.Designer.cs`, таблицы — на
`XPTable` (как в `NuclideSetForm`), подписи по умолчанию английские, русские накладываются
в `ApplyRussian()` по текущей культуре UI. Штатный `Localizable = true` с `.resx` намеренно
не используется: держать координаты контролов в ресурсах ради двух языков дороже словаря
подписей — если понадобится, форму можно открыть в дизайнере и включить локализацию.

Подключение к меню приложения:

```csharp
// MainForm.cs — рядом с ShowNuclideSetForm(). Окно — DockContent, поэтому
// показывается плавающей панелью на общем dockPanel1; экземпляр один,
// закрытие прячет панель (HideOnClose), настройки переживают переоткрытие.
RoiWizard.RoiWizardForm roiWizardForm;

public void ShowRoiWizardForm()
{
    if (this.roiWizardForm == null || this.roiWizardForm.IsDisposed)
    {
        this.roiWizardForm = new RoiWizard.RoiWizardForm(this.RoiWizardResolution);
        this.roiWizardForm.Show(this.dockPanel1, floatWindowBounds);
    }
    else
    {
        this.roiWizardForm.Show(this.dockPanel1);
        this.roiWizardForm.Activate();
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
| **полный набор (все линии, для фита)** | набор собирается заново из выбранных источников, минуя галки, фильтры и слияние: `LineSetBuilder.BuildFullSet`. Ручной выбор якоря при этом выключается — линии в таблице и линии набора уже не одно и то же |
| **якорей** | сколько линий пометить `IsAnchor` при автовыборе (по умолчанию 3) |

Проверки перед сохранением встроены: для ROI выводится вопрос «сохранить всё равно?», для
набора совпавшие энергии и нулевая интенсивность останавливают запись с объяснением.

## Тесты

```
integration\tests\run_tests.cmd
```

Собирает расчётное ядро компилятором из состава .NET Framework и прогоняет проверки —
**никакого тестового фреймворка в решение BecqMoni не добавляется**. Код возврата 0 при
успехе, 1 при провале, так что скрипт годится и для CI. `SetExporter.cs` собирается
вместе с ядром — против заглушек типов хоста (`HostStubs.cs`); вне этой сборки он не
компилировался бы нигде, кроме дерева BecqMoni, и туда однажды уже проскочила забытая
директива `using`. Не входят в сборку только файлы формы: им нужны `XPTable` и менеджеры,
то есть само приложение.

Покрыты инварианты из раздела «Что важно не сломать» — те, чья поломка не видна при сборке:

| Что проверяется | Почему это важно |
|---|---|
| `LibraryName`: последние скобки — всегда родитель либо скобок нет | иначе `ChainOf` читает мусор и связка амплитуд по ряду не собирается |
| слияние меряет порог от первой линии группы, интенсивности суммируются, центроид взвешен | single-linkage склеивал группу шире порога |
| вторичные маркеры не участвуют в слиянии | |
| коэффициенты равновесия, включая сумму по развилке Bi-212 = 1 | |
| якорь: Th-232 → 2614,5, Ra-226 → 609,3; на одних ХРИ якоря нет | якорь на маркере с условной интенсивностью — нефизическая опора для фита |
| ровно один якорь на набор, нулевых интенсивностей нет, цепочка сохранена в 155 именах | |
| совпадение энергий: ошибка для набора, предупреждение для ROI | |
| равновесие ряда применяется только к нуклиду, взятому в составе ряда | одиночный Tl-208 иначе показывал бы 35,85 % вместо табличных 99,75 |
| вторичные пики: формулы и поправки (BS +10 кэВ, CE −0,8·FWHM, доли 8 / 6 %) | |
| экспорт: снятые галки не выгружаются, границы совпадают с `ZoneCalculator`, у ХРИ период не заполняется | расхождение проверок и выгрузки — молчаливое |
| ручной якорь уважается экспортом, но ХРИ ручным якорем — ошибка проверки | иначе опора фита выбирается там, где её физически нет |

На момент последнего прогона: **100 проверок, все зелёные**. Сверх того, решение
BecqMoni собирается вместе с модулем целиком (MSBuild, `/t:Rebuild`, ноль ошибок),
а собранный набор проверен на реальном спектре — см. `docs/UPSTREAM-PR.md`.

## Точки сращивания с приложением

Ради них порт и делается — в вебе это невозможно.

| Что | Как получить из хоста |
|---|---|
| **R детектора** вместо ручного ввода | `EnergyResolutionCalculator.CalculateFWHM(spectrum, start, end)` → `EnergyResolutionResult.Resolution`. Тогда пороги слияния считаются по реальной калибровке текущего спектра, а не по паспорту |
| **Список нуклидов** вместо ручного выбора | взять из результатов поиска пиков (`PeakDetector`, `ResultData`) — те, что уже опознаны в открытом спектре |
| **Применение результата** | `ROIConfigManager.GetInstance().SaveConfig(config)` и `MainForm.ShowROIConfigForm(config)`; набор — `NuclideDefinitionManager.SaveDefinitionFile()` после добавления записей |
| **Живой предпросмотр** | границы `ROIDefinitionData.LowerLimit/UpperLimit` можно рисовать поверх открытого спектра до сохранения |

## Что важно не сломать

- **Хотя бы один `IsAnchor = true` на набор.** Без якоря `LibraryPeakFitter` не стартует
  вовсе. Он перебирает **все** линии с `IsAnchor`, берёт сдвиг калибровки с сильнейшей по
  SNR и требует, чтобы с найденным пиком совпала хотя бы одна (допуск 0,5·FWHM). Поэтому
  автовыбор помечает несколько линий (`AnchorPicker.PickMany`, по умолчанию 3): единственный
  якорь — единственная точка отказа, не нашёлся 2614,5 и набор молчит целиком. Правило
  выбора прежнее — сильные и одинокие γ-линии, одинокие вперёд (для Th-232 первая 2614,5,
  для Ra-226 — 609,3).
- **Скобки в имени — это цепочка.** `ChainOf` в `LibraryPeakFitter` читает текст в последних
  скобках имени как имя родителя. Поэтому у слитой линии интервал энергий выносится наружу:
  `SpectralLine.LibraryName` даёт «Ac-228 964.8–969.0 (Th-232)», а не
  «Ac-228 (Th-232) (964.8–969.0)» — иначе цепочкой считалось бы «964.8–969.0» и связка
  амплитуд не собиралась бы.
- **`Intencity > 0` у всех членов набора.** Линия с нулевой интенсивностью выпадает из
  связки по цепочке — `SetChecker` считает это ошибкой для набора и предупреждением для ROI.
  А вот **совпавшие энергии экспорт не блокируют**: пара «рентген распада + ХРИ того же
  элемента» (Tl-208 X KA1 и ХРИ Pb Kα1 на 74,16 кэВ) — физически одна линия, K-серия
  свинца, и снимать её или оставить решает оператор. Замечание выдаётся, запрета нет.
- **Цепочка должна быть у каждой линии набора.** `ChainOf` читает последние скобки, а без
  них берёт имя целиком: «U-238 X L» оказалось бы собственной цепочкой. Поэтому
  `LibraryName` дописывает цепочку явно — «U-238 X L (U-238)», причём берёт её из подписи,
  под которой линия числится (`OwnerLabel`), а не из имени нуклида: в режиме «линии
  семейства» линия Ra-228 идёт под именем Th-232, и цепочка у неё — Th-232.
- **Равновесие ряда — только для членов ряда.** Множитель применяется, лишь если нуклид
  добавлен в составе цепочки или семейства (подпись отличается от имени). У одиночно
  добавленного Tl-208 родителя в наборе нет, и пересчёт на его распад бессмыслен.
- **Якорем не может быть ХРИ или вторичный маркер.** У первых интенсивность условная
  (Kα1 = 100), у вторых положение — эмпирическая поправка; фит сел бы на нефизическую
  опору. Порог 0,2·max тоже считается по одним γ-линиям. Это правило действует и для
  ручного выбора: в комбобокс якоря попадают только линии, проходящие
  `AnchorPicker.IsAcceptable`, а `SetChecker` проверяет тот якорь, который реально уйдёт
  в набор, а не тот, что предложил бы автовыбор.
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
доли площадей 8 / 6 / 6 %) выведены из измерений комплекса Gamma-1C (NaI(Tl) 63×63,
защита Pb 50 мм с вкладышем Cd/Cu): 31 надёжная запись из 41. Физическое
основание — Knoll, «Radiation Detection and Measurement», гл. 10 (комптоновский край —
разд. II.B, обратное рассеяние и пики вылета — разд. III) и Gilmore, «Practical Gamma-ray
Spectrometry», разд. 9.2.3 (поиск пиков по второй производной). Разбор со страницами —
`docs/REFERENCES-secondary-peaks.md` в корне репозитория.
