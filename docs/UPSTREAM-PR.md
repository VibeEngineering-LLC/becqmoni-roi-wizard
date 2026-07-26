# Внесение модуля в BecqMoni через pull request

Как довести `integration/` до состояния, в котором его не стыдно предложить в
[`Am6er/BecqMoni`](https://github.com/Am6er/BecqMoni), и в каком порядке это делать.

Все факты об upstream проверены по репозиторию **25.07.2026**; где это важно, указано,
чем именно проверено.

## Коротко

PR — правильная форма передачи: копия папки в письме живёт до первого обновления
BecqMoni, PR живёт вместе с проектом.

Автор проекта предложил PR сам, поэтому отдельного issue «можно ли» не требуется.
Порядок: **форк и ветка `feat/roi-wizard` → draft PR с описанием и скриншотом →
доводка по замечаниям.** Вопросы, на которые нужен его ответ (локализация, снимок
данных, скрипт обновления), задаются прямо в описании PR — они не блокируют ревью.

**Состояние на 26.07.2026: PR отправлен — [Am6er/BecqMoni#32](https://github.com/Am6er/BecqMoni/pull/32)**
(draft, 24 файла, +10 443/−0). Форк — `VibeEngineering-LLC/BecqMoni`, ветка
`feat/roi-wizard`; ветка собрана перед отправкой (Release, ноль ошибок), тесты модуля —
100 проверок, все зелёные, прогон на реальном спектре сделан, форма открыта в дизайнере
Visual Studio без ошибок (см. чек-лист).

## 1. Что вносится

| Часть | Файлы | Строк |
|---|---|---|
| Расчётное ядро | `NuclideCatalog`, `SpectralLine`, `LineSetBuilder`, `LineMerger`, `SecondaryPeaks`, `AnchorPicker`, `ZoneCalculator`, `SetChecker` | ≈1600 |
| Мост к приложению | `SetExporter` | 145 |
| Форма | `RoiWizardForm` + `.Designer`, `WizardTheme`, `CatalogCellRenderers`, `HelpForm` | ≈2400 |
| Правка хоста | пункт меню в `MainForm` + две строки ресурсов | ≈40 |
| Данные | `nuclides.xml` — снимок IAEA/ENSDF: 121 нуклид, 1222 γ, 327 X, 3 ряда, 10 элементов ХРИ, словарь семейств | 101 КБ |
| | `help.xml` — текст справки на двух языках | 21 КБ |
| Подписи | `RoiWizardStrings.resx` (английская) + `.ru.resx`, доступ через `RoiWizardStrings.Designer.cs` | 156 строк |
| Инструменты | `tools/export_catalog.py`, `tools/export_help.py`, `tools/gen_strings.py` | 500 |
| Тесты | `tests/RoiWizardTests.cs`, `HostStubs.cs`, `run_tests.cmd` | 490 |

Новых зависимостей нет: `XmlSerializer`, WinForms и `XPTable` — всё уже в проекте.

### Локализация — механизмом проекта

Подписи модуля лежат в `RoiWizard/RoiWizardStrings.resx` (нейтральная, английская)
и `RoiWizardStrings.ru.resx`. MSBuild собирает из второй сателлит
`ru\BecquerelMonitor.resources.dll` — ровно так же, как для остальных 31 формы
BecqMoni и для `Properties/Resources.resx`. Новый язык добавляется файлом
`RoiWizardStrings.<culture>.resx`, код при этом не трогают, и строки модуля можно
выгрузить в тот же `BecqMoni_Localization.xlsx`, по которому работают переводчики.

Ядро языка не знает вовсе: `SetChecker` возвращает код замечания и подстановки
(`IssueKind` + `Args`), фразу собирает форма. Раньше тексты проверок были только
русскими и в английском интерфейсе оставались русскими; тесты заодно сверяют код,
а не обрывок фразы.

### Как выглядит окно

Полный набор снимков — **[docs/SCREENSHOTS.md](SCREENSHOTS.md)**: каждый экран
в своём состоянии, с пояснениями. Снимки лежат в этом репозитории, а не в PR:
класть мегабайт картинок в дерево BecqMoni незачем, в описании PR они вставляются
ссылками на `raw.githubusercontent.com` и отображаются прямо в теле.

Для описания PR достаточно пяти — пункт меню, по одному на шаг и справка:

```markdown
![Пункт меню](https://raw.githubusercontent.com/VibeEngineering-LLC/becqmoni-roi-wizard/main/docs/screenshots/13-main-menu.png)
![Шаг 1 · Изотопы](https://raw.githubusercontent.com/VibeEngineering-LLC/becqmoni-roi-wizard/main/docs/screenshots/03-step1-preset.png)
![Шаг 2 · Линии](https://raw.githubusercontent.com/VibeEngineering-LLC/becqmoni-roi-wizard/main/docs/screenshots/05-step2-folded.png)
![Шаг 3 · Оформление и экспорт](https://raw.githubusercontent.com/VibeEngineering-LLC/becqmoni-roi-wizard/main/docs/screenshots/08-step3.png)
![Справка](https://raw.githubusercontent.com/VibeEngineering-LLC/becqmoni-roi-wizard/main/docs/screenshots/11-help.png)
```

## 2. Состояние upstream (проверено 25.07.2026)

| Что | Значение | Чем проверено |
|---|---|---|
| Лицензия | **GPL-2.0** | `gh api repos/Am6er/BecqMoni` |
| Ветка по умолчанию | `master` | там же |
| Активность | коммиты 24.07.2026, 52 звезды, 2 форка | там же |
| PR-практика | последние 10 PR — все слиты; есть внешний контрибьютор `Maksim-Bartoshyk` (33 коммита) | `gh pr list --state all` |
| Имена веток | `feat/…`, `fix/…`, `feature/…` | те же PR |
| `CONTRIBUTING.md` | нет | листинг корня |
| CI | два workflow — сборка релизов (`current-release.yml`, `version-release.yml`); проверки на PR нет | `.github/workflows` |
| Формат проекта | старый (non-SDK) `.csproj`, ToolsVersion 12.0, **507 записей `<Compile>`**, `RootNamespace = BecquerelMonitor`, `TargetFrameworkVersion v4.8` | `BecquerelMonitor.csproj` |
| Локализация | штатная, сателлитными ресурсами: 34 записи `*.ru.resx` / `*.ja.resx` | тот же файл |
| Язык комментариев | русский и английский вперемешку — наш стиль конфликта не создаёт | `LibraryPeakFitter.cs` и др. |
| Имя `RoiWizard` | не занято (0 вхождений) | поиск по `.csproj` |

## 3. Лицензии

- Наш код — MIT, upstream — GPL-2.0. **MIT-код включается в GPL-проект без препятствий**:
  условия MIT (сохранить копирайт и текст лицензии) совместимы с GPL-2.0. Внутри BecqMoni
  файлы будут распространяться на условиях GPL-2.0 — это нормальный и ожидаемый исход.
- **Данные.** `nuclides.xml` — снимок IAEA Live Chart of Nuclides (ENSDF). В PR это стоит
  назвать явно: источник, дата снимка, скрипт пересборки. Атрибуция уже лежит в
  атрибутах корневого элемента (`Generated`, пороги интенсивности).
- Заимствований из кода BecqMoni в модуле нет: пороги (0.85 / 0.25 / z ≥ 4) — это числа,
  прочитанные из `LibraryPeakFitter.cs`, а не скопированный код.

## 4. Что доделать до отправки

- [x] **Собрать в настоящей среде.** Сделано 25.07: полная пересборка
      `BecquerelMonitor.sln` (MSBuild из VS 2022 Build Tools, `.NET Framework 4.8
      Developer Pack`, `/t:Rebuild`) вместе с модулем и патчем меню — **ноль ошибок**.
      Результат: `bin\Release\BecquerelMonitor.exe`, 11,2 МБ, внутри `RoiWizardForm`,
      `AnchorPicker`, `BuildFullSet` и ресурс `BecquerelMonitor.RoiWizard.nuclides.xml`;
      сателлитная сборка `ru\BecquerelMonitor.resources.dll` содержит подпись пункта меню
      «Конструктор ROI и наборов нуклидов…». Единственное предупреждение сборки —
      `MSB3327` про сертификат подписи ClickOnce, к вкладу отношения не имеет.
      *(Если Developer Pack ставить некуда, решение собирается и без него: reference
      assemblies из NuGet-пакета `Microsoft.NETFramework.ReferenceAssemblies.net48`
      передаются через `/p:TargetFrameworkRootPath=`, недоступной остаётся только
      линковка сателлитов — она требует `al.exe` из .NET Framework SDK.)*
- [x] **Пункт меню и его подписи** — `integration/host-patch/` (§5): скрипт применяет
      правку к дереву и идемпотентен; разрешение детектора мастер получает из родной
      `FwhmCalibration` активного спектра, приведённой к R (%) на 662 кэВ.
- [x] **Режим «полный набор»** — `LineSetBuilder.BuildFullSet`, галка на вкладке экспорта.
      Набор собирается заново из источников, минуя галки, фильтры и слияние; вторичных
      маркеров в нём нет по построению; равновесие ряда действует.
- [x] **Несколько якорей** — `AnchorPicker.PickMany`, по умолчанию 3 (поле «якорей» на
      форме, 1–9). Правило прежнее: сильные и одинокие γ-линии, одинокие вперёд.
- [x] **Открыта в дизайнере** (26.07, VS 2022 Community): форма рисуется на поверхности
      конструктора целиком — три вкладки, панели шага 1, таблицы `XPTable`, лоток
      компонентов (`columnModelCatalog`, `tableModelNear`, `statusStrip`, …), ноль
      ошибок. Значит присваивания вида `RoiWizardStrings.columnLineName_Text` и
      `new NearAddCellRenderer()` в `InitializeComponent` дизайнер разбирает.
      Условие: **проект должен быть собран в активной конфигурации**. При пустом
      `bin\Debug` дизайнер выдаёт девять ошибок вида «Не удалось найти тип
      XPTable.Models.Table» и «Переменная 'tableCatalog' не объявлена» — типы он берёт
      из собранной сборки проекта, а не из исходников; собранного `bin\Release`
      для этого мало. Это не свойство модуля: так же ведёт себя любая форма BecqMoni.
- [x] **Локализация переведена на механизм проекта** (26.07): подписи в
      `RoiWizardStrings.resx` / `.ru.resx`, сателлит собирает MSBuild. Заодно закрыта
      течь: тексты проверок данных были только русскими и в английском интерфейсе
      оставались русскими — теперь ядро отдаёт код замечания, фразу собирает форма.
      Проверено прогоном формы при `ru-RU` и `en-US`.
- [x] **Прогон на реальном спектре.** Сделан 25.07 на спектре оператора (AtomNano 5 PRO
      LaBr₃, 8192 канала, 6,4·10⁶ импульсов, торированный электрод WT-20 + KCl, R = 2,5 %).
      Набор собран модулем (Th-232 цепочкой + K-40 + ХРИ W): 166 линий, 3 якоря.
      Прогон боевого пути `PeakDetector.DetectPeak` с библиотечным фитом, без UI:
      **детектор нашёл 11 пиков, библиотечный фит добавил 97**; по цепочкам — Th-232
      103 линии, ХРИ W 3 линии. Контрольные: 2614,5 ✓ (SNR 77), 911,2 ✓ (130),
      583,2 ✓ (library, SNR 287), 238,6 ✓ (library, SNR 533), W Kα1 59,5 ✓ (117).
      Оговорка для описания PR: пик 1458 кэВ фит отдал Ac-228 (1459,1), а не K-40 (1460,8) —
      при 2,5 % это одна линия (FWHM ≈ 25 кэВ на 1460), разделения быть не может.

## 5. Как встраивается (точные места)

**`.csproj`** — добавить в существующий `ItemGroup` с `<Compile>`:

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
<Compile Include="RoiWizard\RoiWizardForm.cs">
  <SubType>Form</SubType>
</Compile>
<Compile Include="RoiWizard\RoiWizardForm.Designer.cs">
  <DependentUpon>RoiWizardForm.cs</DependentUpon>
</Compile>
<EmbeddedResource Include="RoiWizard\nuclides.xml" />
```

Имя ресурса получится `BecquerelMonitor.RoiWizard.nuclides.xml` — оно и захардкожено в
`NuclideCatalog.ResourceName` (корневое пространство имён проекта — `BecquerelMonitor`).

**`tests/HostStubs.cs` в проект не добавлять** — там заглушки тех же типов, будет `CS0101`.

**Меню.** Образец — соседний пункт «Nuclide sets» (`MainForm.cs`):

```csharp
void NuclideSetToolStripMenuItem_Click(object sender, EventArgs e)
{
    this.ShowNuclideSetForm();
}

public void ShowNuclideSetForm()
{
    NuclideSetForm form = new NuclideSetForm(this);
    form.ShowDialog();
}
```

Для мастера — так же: объявить `RoiWizardToolStripMenuItem` в `MainForm.Designer.cs`
рядом с `NuclideSetToolStripMenuItem` (:878), обработчик в `MainForm.cs`, подписи —
в `MainForm.resx` и `MainForm.ru.resx` (ключ `RoiWizardToolStripMenuItem.Text`; в ru.resx
подписи соседних пунктов лежат так же, например :303).

Разрешение детектора передаётся конструктором `RoiWizardForm(Func<double>)`; если брать
неоткуда — конструктор без аргументов, тогда кнопка «из спектра» просто выключена.

## 6. Порядок

1. **Форк `Am6er/BecqMoni`** в организацию `VibeEngineering-LLC` + ветка `feat/roi-wizard`.
   Ветка готовится скриптом: `python integration/host-patch/apply_patch.py <форк>` —
   он копирует модуль, правит `.csproj` и добавляет пункт меню с подписями.
   Клонировать форк нужно с `-c core.longpaths=true`: в дереве лежат вендорные пакеты
   с путями длиннее 260 символов, и обычный `git clone` обрывается на них.
   После клона — `msbuild -t:restore`, иначе сборка падает на ссылках пакетов.
2. **Коммиты по смыслу**: (а) ядро и данные, (б) форма, (в) пункт меню и ресурсы,
   (г) тесты. Один PR — дробить нечего: без формы ядро мертво, без ядра форма не собирается.
3. **Draft PR** с описанием: что делает, скриншот окна, откуда числа
   (`LibraryPeakFitter.cs` — 0.85 / 0.25 / z ≥ 4, гейт якоря), откуда данные (IAEA/ENSDF,
   скрипт пересборки), как проверялось (100 проверок, `run_tests.cmd`, код возврата 0/1;
   решение собирается вместе с модулем). Вопросы из §7 — там же, отдельным разделом.
4. **Доводка** по замечаниям, снятие статуса draft.

## 7. Вопросы владельцу

*Вопрос о локализации снят 26.07: подписи переведены на штатный механизм проекта
(`RoiWizardStrings.resx` + `.ru.resx`, сателлит собирает MSBuild), спрашивать нечего.*

1. **Снимок данных в репозитории.** 101 КБ XML внутри сборки — приемлемо, или каталог
   должен подтягиваться из `NucBase`, который в BecqMoni уже есть?
2. **Обновление каталога.** Сейчас — python-скрипт по IAEA Live Chart. Нужен ли он в
   дереве проекта или достаточно готового снимка?
3. **Якоря — главный вопрос.** Модуль помечает **несколько** якорных линий (по умолчанию
   три, поле 1–9), веб-версия — одну. Расхождение оставлено намеренно, решать автору.
   Основание для нескольких: `LibraryPeakFitter` перебирает все записи с `IsAnchor`,
   берёт сдвиг калибровки с сильнейшей по SNR и требует совпадения с найденным пиком
   хотя бы одной (допуск 0,5·FWHM) — при единственном якоре не нашлась линия 2614,5,
   и молчит весь набор. Вопрос: так ли задуман гейт, и не мешает ли несколько якорей
   чему-то, что видно изнутри приложения.
4. **Форма окна.** Три вкладки (изотопы → линии → оформление и экспорт) против одного
   окна с секциями — что ближе к тому, как устроен остальной интерфейс.

## 8. Риски

| Риск | Чем снимается |
|---|---|
| Конфликт в `.csproj` (507 записей `<Compile>`, автор правит их постоянно) | ветку держать свежей, пересобирать перед пушем; конфликт в `.csproj` разрешается тривиально, но чинить его придётся вручную |
| PR пересекается с активной работой по library-fit (правки 23–24.07) | сначала issue, потом код; в PR явно сослаться на константы, из которых исходили |
| Размер вклада отпугнёт | issue с демонстрацией до кода; тесты и раздел «откуда числа» в описании |
| Данные IAEA устареют | скрипт пересборки в комплекте, снимок датирован |
| Автор попросит переписать локализацию | вопрос задан заранее (§7.1), переделка механическая |
