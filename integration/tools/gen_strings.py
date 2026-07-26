# -*- coding: utf-8 -*-
"""Сборка таблицы строк модуля: RoiWizardStrings.resx / .ru.resx / .Designer.cs.

Механизм тот же, что у самого BecqMoni (`Properties/Resources.resx` плюс сателлиты
`ru\\BecquerelMonitor.resources.dll`): нейтральная таблица — английская, рядом
русская, MSBuild собирает сателлит. Новый язык добавляется файлом
`RoiWizardStrings.<culture>.resx` — код при этом не трогают.

Источник ключей — этот файл: пары «английский / русский» лежат ниже списком.
`RoiWizardStrings.Designer.cs` генерируется отсюда же (так же поступает и Visual
Studio) и содержит английский текст запасным значением: тесты ядра собираются
консольным компилятором без ресурсов, и без запасного значения падали бы.

Запуск:  python integration/tools/gen_strings.py
"""
import io
import os
import sys
from xml.sax.saxutils import escape

HERE = os.path.dirname(os.path.abspath(__file__))
OUT = os.path.join(os.path.dirname(HERE), "BecquerelMonitor", "RoiWizard")

# ── таблица строк: ключ, английский, русский ────────────────────────────────
# Ключ вида «контрол_свойство» ставится в разметке; остальные — по смыслу.
STRINGS = [
    # ── окно и вкладки ──
    ("form_Title", "ROI and nuclide set builder", "Конструктор ROI и наборов нуклидов"),
    ("tabSources_Text", "1 · Nuclides", "1 · Изотопы"),
    ("tabLines_Text", "2 · Lines", "2 · Линии"),
    ("tabExport_Text", "3 · Styling and export", "3 · Оформление и экспорт"),
    ("statusFormat", "lines: {0} of {1} · nuclides: {2}", "линий: {0} из {1} · нуклидов: {2}"),
    ("buttonHelp_Text", "Help", "Справка"),
    ("stepBack", "◂ Back", "◂ Назад"),
    ("stepForward", "Next ▸", "Вперёд ▸"),
    ("stepNuclides", "Nuclides", "Изотопы"),
    ("stepLines", "Lines", "Линии"),
    ("stepExport", "Styling and export", "Оформление и экспорт"),

    # ── шаг 1: поиск ──
    ("groupSearch_Text", "Nuclide search", "Поиск изотопа"),
    ("buttonAddSingle_Text", "Add", "Добавить"),
    ("buttonAddFamily_Text", "+ family", "+ семейство"),
    ("buttonAddChain_Text", "+ chain", "+ цепочка"),
    ("columnCatalogName_Text", "Nuclide", "Нуклид"),
    ("columnCatalogFamilies_Text", "Families", "Семейства"),
    ("columnCatalogLines_Text", "Lines", "Линий"),
    ("labelSearchHint_Text", "Typing narrows the list: by name or by family code.",
     "Ввод сужает список: по имени или по коду семейства."),
    ("presetsCaption", "Presets:", "Пресеты:"),

    # ── шаг 1: пресеты ──
    ("preset1_Title", "NORM background", "ЕРН-фон"),
    ("preset1_Hint", "Th-232 + U-238 as chains + K-40", "Th-232 + U-238 цепочками + K-40"),
    ("preset2_Title", "Cs-137 / Co-60 check", "Поверка Cs-137 / Co-60"),
    ("preset2_Hint", "Reference check sources", "Контрольные источники"),
    ("preset3_Title", "Calibration set", "ОСГИ / калибровка"),
    ("preset3_Hint", "Am-241, Ba-133, Eu-152, Cs-137, Co-60", "Am-241, Ba-133, Eu-152, Cs-137, Co-60"),
    ("preset4_Title", "Medical", "Медицинские"),
    ("preset4_Hint", "MED family", "Семейство MED"),
    ("preset5_Title", "Detector and shield XRF", "ХРИ детектора и защиты"),
    ("preset5_Hint", "Pb, W, La, Ba, I", "Pb, W, La, Ba, I"),

    # ── шаг 1: группа ──
    ("groupGroup_Text", "Group", "Группа"),
    ("buttonGroupAll_Text", "add all", "добавить все"),
    ("buttonGroupFamily_Text", "+ family lines", "+ линии семейства"),
    ("buttonGroupChain_Text", "+ chain", "+ цепочкой"),
    ("hintNone", "Tick a nuclide — the buttons apply to it.",
     "Отметьте нуклид — кнопки применятся к нему."),
    ("hintPicked", "Applies to the ticked ones ({0}).", "Применяется к отмеченным ({0})."),

    # ── шаг 1: ХРИ и выбранное ──
    ("groupXrf_Text", "XRF elements", "ХРИ — элементы"),
    ("labelXrf_Text", "Shielding and detector materials:", "Материалы защиты и детектора:"),
    ("labelXrfHint_Text",
     "Kα/Kβ (+L for heavy elements). Intensities are nominal (Kα1 = 100) — markers only.",
     "Kα/Kβ (+L для тяжёлых). Интенсивности относительные (Kα1 = 100) — только маркеры."),
    ("groupSelected_Text", "Selected", "Выбрано"),
    ("buttonClear_Text", "clear all", "очистить всё"),
    ("xrfChipPrefix", "XRF ", "ХРИ "),
    ("emptySelectionHint", "empty — start with a group above", "пусто — начните с группы выше"),

    # ── шаг 2: разрешение ──
    ("groupResolution_Text", "Detector-resolution adaptation", "Адаптация под разрешение детектора"),
    ("labelResolution_Text", "R, % at 662 keV", "R, % на 662 кэВ"),
    ("buttonFromSpectrum_Text", "from spectrum", "из спектра"),
    ("labelCriterion_Text", "criterion", "критерий"),
    ("labelFactor_Text", "× FWHM", "× FWHM"),
    ("buttonMerge_Text", "Merge close lines", "Объединить близкие"),
    ("buttonUnmerge_Text", "Restore originals", "Вернуть исходные"),
    ("mergeInfoFormat",
     "threshold {0:0.##}·FWHM: lines merge closer than {1:0.#} keV at 100, {2:0.#} at 662, {3:0.#} at 1500",
     "порог {0:0.##}·FWHM: сливаются линии ближе {1:0.#} кэВ на 100, {2:0.#} на 662, {3:0.#} на 1500"),
    ("criterionSparrow", "Sparrow limit — ROI markers (0.85·FWHM)",
     "предел Sparrow — маркеры ROI (0,85·FWHM)"),
    ("criterionAnchored", "anchored set — library fit (0.25·FWHM)",
     "якорный набор — библиотечный фит (0,25·FWHM)"),
    ("criterionManual", "manual", "вручную"),

    # ── шаг 2: фильтры ──
    ("groupFilters_Text", "Filters and selection", "Фильтры и выбор"),
    ("checkIntensity_Text", "intensity ≥, %", "интенсивность ≥,"),
    ("intensityRelative", "relative (within nuclide, max = 100)",
     "относительная (внутри изотопа, макс = 100)"),
    ("intensityAbsolute", "absolute (per decay)", "абсолютная (на распад)"),
    ("checkEnergy_Text", "energy, keV", "энергия, кэВ"),
    ("checkHalfLife_Text", "T½", "T½"),
    ("buttonSelectAll_Text", "✓ select all visible", "✓ выбрать все видимые"),
    ("buttonSelectNone_Text", "✗ deselect all visible", "✗ снять все видимые"),
    ("labelTopN_Text", "top-N by I per nuclide", "топ-N по I на нуклид"),
    ("buttonSelectTop_Text", "Select top-N", "Выбрать топ-N"),
    ("checkHideUnselected_Text", "hide unselected", "скрыть невыбранные"),
    ("labelTypes_Text", "Line types", "Тип линий"),
    ("checkTypeXray_Text", "X (decay)", "X (распад)"),
    ("checkTypeXrf_Text", "XRF", "ХРИ"),
    ("checkTypeSecondary_Text", "secondary", "вторичные"),
    ("checkEquilibrium_Text", "series equilibrium (intensities per parent decay)",
     "равновесие ряда (интенсивности на распад родителя)"),
    ("unitSeconds", "s", "сек"),
    ("unitHours", "h", "ч"),
    ("unitDays", "d", "сут"),
    ("unitYears", "y", "лет"),

    # ── единицы T½ в подписях: каталог хранит их по-русски, показываются они
    #    на языке интерфейса (в фильтре единицы свои — там «сек», здесь «с»)
    ("hlSeconds", "s", "с"),
    ("hlMinutes", "min", "мин"),
    ("hlHours", "h", "ч"),
    ("hlDays", "d", "сут"),
    ("hlYears", "y", "лет"),

    # ── шаг 2: таблица линий ──
    ("columnLineName_Text", "Nuclide", "Нуклид"),
    ("columnLineEnergy_Text", "E, keV", "E, кэВ"),
    ("columnLineIntensity_Text", "I, %", "I, %"),
    ("columnLineRelative_Text", "I rel., %", "I отн., %"),
    ("columnLineHalfLife_Text", "T½", "T½"),
    ("columnLineType_Text", "Type", "Тип"),
    ("lineTypeXrf", "XRF", "ХРИ"),
    ("lineTypeSecondary", "sec", "втор"),

    # ── шаг 2: вторичные пики ──
    ("groupSecondary_Text", "Secondary peaks (computed from selected γ lines)",
     "Вторичные пики (расчёт по выбранным γ-линиям)"),
    ("labelSecondaryMin_Text", "for γ lines with I ≥, %", "для γ-линий с I ≥, %"),
    ("checkSecBackscatter_Text", "backscatter (BS)", "рассеяние назад (BS)"),
    ("checkSecComptonEdge_Text", "Compton edge (CE)", "комптон-край (CE)"),
    ("checkSecSingleEscape_Text", "escape 511 (SE)", "вылет 511 (SE)"),
    ("checkSecDoubleEscape_Text", "escape 1022 (DE)", "вылет 1022 (DE)"),
    ("checkSecIodine_Text", "I-K escape (NaI, −28.6)", "вылет I-K (NaI, −28.6)"),
    ("checkSecAnnihilation_Text", "annihilation 511", "аннигиляция 511"),
    ("checkSecSum_Text", "cascade sum (E1+E2)", "суммирование каскадное"),
    ("checkSecPileUp_Text", "pile-up 2×E", "наложение 2×E"),
    ("buttonGenerateSecondary_Text", "Generate", "Сгенерировать"),
    ("secondaryFormat", "secondary markers added: {0}", "добавлено вторичных маркеров: {0}"),
    ("annihilationLabel", "Annihilation 511", "Аннигиляция 511"),

    # ── шаг 2: поиск близких линий ──
    ("groupNear_Text", "Nearby-line search (whole database — who else emits here)",
     "Поиск близких линий (по всей базе — кто ещё светит рядом)"),
    ("labelNearEnergy_Text", "energy, keV", "энергия, кэВ"),
    ("labelNearWindow_Text", "± window", "± окно"),
    ("labelNearIntensity_Text", "I ≥, %", "I ≥, %"),
    ("labelNearHalfLife_Text", "T½ ≥", "T½ ≥"),
    ("buttonNearSearch_Text", "Search", "Искать"),
    ("buttonNearAdd_Text", "+ add", "+ добавить"),
    ("columnNearDelta_Text", "ΔE", "ΔE"),
    ("nearAdded", "added", "в наборе"),
    ("nearMoreFormat", "showing the first {0} of {1}", "показаны первые {0} из {1}"),
    ("nearEmptyFormat", "nothing found within {0} ± {1} keV",
     "в окне {0} ± {1} кэВ ничего не найдено"),

    # ── шаг 3: оформление ──
    ("groupStyle_Text", "ROI styling", "Оформление ROI"),
    ("labelStyle_Text", "mode", "режим"),
    ("labelWidth_Text", "zone width", "ширина зоны"),
    ("labelColors_Text", "Colours", "Цвета"),
    ("buttonColorByChain_Text", "by chain", "по цепочке"),
    ("buttonColorByNuclide_Text", "by nuclide", "по нуклиду"),
    ("roiStyleMarkers", "marker lines (height ∝ I, no zones)",
     "линии-маркеры (высота ∝ I, без зон)"),
    ("roiStyleZones", "zones (limits around the peak)", "зоны (границы вокруг пика)"),
    ("roiStyleBoth", "zones + intensity markers", "зоны + маркеры интенсивности"),
    ("widthModePercent", "% of energy (BecqMoni style)", "% от энергии (как в BecqMoni)"),
    ("widthModeFwhm", "k × FWHM (scintillator)", "k × FWHM (сцинтиллятор)"),

    # ── шаг 3: экспорт ──
    ("groupExport_Text", "Export", "Экспорт"),
    ("labelConfigName_Text", "ROI configuration name", "имя ROI-конфигурации"),
    ("buttonCreateRoi_Text", "Create ROI configuration", "Создать ROI-конфигурацию"),
    ("buttonPreview_Text", "Preview", "Предпросмотр"),
    ("labelSetName_Text", "set name (NuclideSet)", "имя набора (NuclideSet)"),
    ("textSetName_Text", "IAEA set", "Набор IAEA"),
    ("labelAnchor_Text", "anchor line", "якорная линия"),
    ("buttonCreateSet_Text", "Add set to the library", "Добавить набор в библиотеку"),
    ("checkFullSet_Text", "full set (all lines, for fitting)", "полный набор (все линии, для фита)"),
    ("labelAnchorCount_Text", "anchor lines", "якорей"),
    ("labelIssues_Text", "Data check:", "Проверка данных:"),
    ("previewEmpty", "no lines selected", "линии не выбраны"),
    ("anchorAuto", "auto — {0} {1}", "auto — {0} {1}"),

    # ── проверки данных ──
    ("issuePrefixRoi", "ROI", "ROI"),
    ("issuePrefixSet", "SET", "SET"),
    ("issueNone", "no issues", "замечаний нет"),
    ("issueEqualEnergies",
     "equal energies: “{0}” and “{1}” ({2} / {3} keV) — the amplitude fit degenerates here",
     "равные энергии: «{0}» и «{1}» ({2} / {3} кэВ) — подгонка амплитуд на этой позиции вырождается"),
    ("issueZeroYield", "zero yield: “{0}” ({1} keV)", "нулевой выход: «{0}» ({1} кэВ)"),
    ("issueAnchorIsXrf",
     "the anchor “{0}” ({1} keV) is a characteristic X-ray of a material, not a decay line: "
     "the fit would rest on a line whose position or intensity is nominal",
     "якорем выбрана линия «{0}» ({1} кэВ): это характеристический рентген материала, "
     "а не линия распада. Фит сел бы на опору, положение или интенсивность которой условны"),
    ("issueAnchorIsSecondary",
     "the anchor “{0}” ({1} keV) is a computed secondary marker, not a decay line: "
     "the fit would rest on a line whose position or intensity is nominal",
     "якорем выбрана линия «{0}» ({1} кэВ): это расчётный вторичный маркер, "
     "а не линия распада. Фит сел бы на опору, положение или интенсивность которой условны"),
    ("issueNoAnchor",
     "no anchor line: the set holds no decay line at all (XRF and secondary markers cannot be "
     "anchors) — the library fit does not start without one",
     "нет якорной линии: в наборе нет ни одной линии распада (ХРИ и вторичные маркеры якорем "
     "быть не могут) — библиотечный фит без якоря не запускается"),
    ("issueAnchorIsXray",
     "the anchor is the X-ray line “{0}” ({1} keV): a γ line is a firmer footing for the fit",
     "якорь — рентгеновская линия «{0}» ({1} кэВ): для опоры фита надёжнее γ-линия"),
    ("issueZonesOverlap", "zones overlap: “{0}” [{1}–{2}] and “{3}” [{4}–{5}]",
     "перекрытие зон: «{0}» [{1}–{2}] и «{3}» [{4}–{5}]"),

    # ── диалоги ──
    ("confirmTitle", "ROI and nuclide set builder", "Конструктор ROI и наборов нуклидов"),
    ("confirmRoiOverwrite",
     "A configuration named “{0}” already exists — its file will be overwritten. Continue?",
     "Конфигурация «{0}» уже есть — её файл будет перезаписан. Продолжить?"),
    ("confirmSetDuplicate",
     "The library already holds a set named “{0}”. Add another one with the same name?",
     "Набор «{0}» в библиотеке уже есть. Добавить ещё один с тем же именем?"),
    ("noLinesSelected", "No lines selected.", "Линии не выбраны."),
    ("noResolutionFromSpectrum", "The resolution could not be taken from the active spectrum.",
     "Не удалось взять разрешение из активного спектра."),
    ("confirmErrorsHead", "The set cannot be saved — the data check found errors:",
     "Набор сохранить нельзя — проверка данных нашла ошибки:"),
    ("confirmIssuesHead", "The data check found issues:", "Проверка данных нашла замечания:"),
    ("confirmErrorsTail",
     "Two lines at the same energy make the amplitude fit degenerate, and zero intensity "
     "drops a line out of the chain coupling.",
     "Две линии на одной энергии вырождают подгонку амплитуд, а нулевая интенсивность "
     "выбрасывает линию из связки по цепочке."),
    ("confirmSaveAnyway", "Save anyway?", "Всё равно сохранить?"),

    # ── справка ──
    ("helpTitle", "Help: ROI and nuclide set builder",
     "Справка: конструктор ROI и наборов нуклидов"),
    ("helpSourcesArrow", "   →   ", "   →   "),
]

RESX_HEAD = u"""<?xml version="1.0" encoding="utf-8"?>
<root>
  <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
    <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
    <xsd:element name="root" msdata:IsDataSet="true">
      <xsd:complexType>
        <xsd:choice maxOccurs="unbounded">
          <xsd:element name="metadata">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" />
              </xsd:sequence>
              <xsd:attribute name="name" use="required" type="xsd:string" />
              <xsd:attribute name="type" type="xsd:string" />
              <xsd:attribute name="mimetype" type="xsd:string" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="assembly">
            <xsd:complexType>
              <xsd:attribute name="alias" type="xsd:string" />
              <xsd:attribute name="name" type="xsd:string" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="data">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" msdata:Ordinal="1" />
              <xsd:attribute name="type" type="xsd:string" msdata:Ordinal="3" />
              <xsd:attribute name="mimetype" type="xsd:string" msdata:Ordinal="4" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="resheader">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name="resmimetype">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name="version">
    <value>2.0</value>
  </resheader>
  <resheader name="reader">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name="writer">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
"""


def write_resx(path, index):
    parts = [RESX_HEAD]
    for entry in STRINGS:
        parts.append(u'  <data name="%s" xml:space="preserve">\n    <value>%s</value>\n  </data>\n'
                     % (entry[0], escape(entry[index])))
    parts.append(u"</root>\n")
    io.open(path, "w", encoding="utf-8", newline="\r\n").write(u"".join(parts))


def write_designer(path):
    lines = [
        u"//------------------------------------------------------------------------------",
        u"// Сгенерировано integration/tools/gen_strings.py из RoiWizardStrings.resx.",
        u"// Править этот файл руками не нужно: правьте таблицу в gen_strings.py.",
        u"//------------------------------------------------------------------------------",
        u"",
        u"using System.Globalization;",
        u"using System.Resources;",
        u"",
        u"namespace BecquerelMonitor.RoiWizard",
        u"{",
        u"    // Подписи интерфейса модуля. Нейтральная таблица английская, рядом",
        u"    // RoiWizardStrings.ru.resx; MSBuild собирает сателлит, как и для остальных",
        u"    // форм BecqMoni. Новый язык — ещё один .resx, код не трогается.",
        u"    //",
        u"    // Если ресурс недоступен (тесты ядра собираются консольным компилятором",
        u"    // без ресурсов), возвращается английский текст, зашитый сюда генератором.",
        u"    internal static class RoiWizardStrings",
        u"    {",
        u"        static ResourceManager manager;",
        u"",
        u"        static string Get(string key, string fallback)",
        u"        {",
        u"            try",
        u"            {",
        u"                if (manager == null)",
        u"                {",
        u"                    manager = new ResourceManager(",
        u"                        \"BecquerelMonitor.RoiWizard.RoiWizardStrings\",",
        u"                        typeof(RoiWizardStrings).Assembly);",
        u"                }",
        u"                string value = manager.GetString(key, CultureInfo.CurrentUICulture);",
        u"                return value ?? fallback;",
        u"            }",
        u"            catch (MissingManifestResourceException)",
        u"            {",
        u"                return fallback;",
        u"            }",
        u"        }",
        u"",
    ]
    for key, english, _ in STRINGS:
        literal = english.replace("\\", "\\\\").replace('"', '\\"')
        lines.append(u"        public static string %s" % key)
        lines.append(u"        {")
        lines.append(u"            get { return Get(\"%s\", \"%s\"); }" % (key, literal))
        lines.append(u"        }")
        lines.append(u"")
    lines.append(u"    }")
    lines.append(u"}")
    io.open(path, "w", encoding="utf-8", newline="\r\n").write(u"\n".join(lines))


def main():
    keys = [entry[0] for entry in STRINGS]
    if len(set(keys)) != len(keys):
        duplicates = sorted(set(key for key in keys if keys.count(key) > 1))
        raise SystemExit("повторяющиеся ключи: %s" % ", ".join(duplicates))

    write_resx(os.path.join(OUT, "RoiWizardStrings.resx"), 1)
    write_resx(os.path.join(OUT, "RoiWizardStrings.ru.resx"), 2)
    write_designer(os.path.join(OUT, "RoiWizardStrings.Designer.cs"))
    print("строк: %d" % len(STRINGS))
    print("записаны RoiWizardStrings.resx, .ru.resx и .Designer.cs в %s" % OUT)


if __name__ == "__main__":
    sys.exit(main())
