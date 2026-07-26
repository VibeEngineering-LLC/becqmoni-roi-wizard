# -*- coding: utf-8 -*-
"""Встраивание модуля RoiWizard в дерево BecqMoni: файлы, .csproj, пункт меню, ресурсы.

Ровно то, что описано в README.md рядом, только машинно — чтобы правку можно было
проверить сборкой, а не глазами, и чтобы ветка под pull request собиралась одной
командой.

    python integration/host-patch/apply_patch.py C:\\path\\to\\BecqMoni

Идемпотентен: повторный запуск ничего не дублирует. Файлы приложения только дополняются —
ни одна существующая строка не меняется и не удаляется.
"""
from __future__ import print_function

import codecs
import io
import os
import shutil
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
MODULE = os.path.join(os.path.dirname(HERE), "BecquerelMonitor", "RoiWizard")

MENU_ITEM = "RoiWizardToolStripMenuItem"

CSPROJ_ENTRIES = """    <Compile Include="RoiWizard\\NuclideCatalog.cs" />
    <Compile Include="RoiWizard\\SpectralLine.cs" />
    <Compile Include="RoiWizard\\LineSetBuilder.cs" />
    <Compile Include="RoiWizard\\LineMerger.cs" />
    <Compile Include="RoiWizard\\SecondaryPeaks.cs" />
    <Compile Include="RoiWizard\\AnchorPicker.cs" />
    <Compile Include="RoiWizard\\ZoneCalculator.cs" />
    <Compile Include="RoiWizard\\SetChecker.cs" />
    <Compile Include="RoiWizard\\SetExporter.cs" />
    <Compile Include="RoiWizard\\WizardTheme.cs" />
    <Compile Include="RoiWizard\\CatalogCellRenderers.cs" />
    <Compile Include="RoiWizard\\RoiWizardStrings.Designer.cs">
      <AutoGen>True</AutoGen>
      <DependentUpon>RoiWizardStrings.resx</DependentUpon>
    </Compile>
    <Compile Include="RoiWizard\\HelpForm.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="RoiWizard\\RoiWizardForm.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="RoiWizard\\RoiWizardForm.Designer.cs">
      <DependentUpon>RoiWizardForm.cs</DependentUpon>
    </Compile>
"""

MAINFORM_CODE = """
        void RoiWizardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowRoiWizardForm();
        }

        // Окно мастера — плавающая док-панель на общем dockPanel1: её можно пристыковать,
        // сгруппировать с другими панелями и убрать в автоскрытие булавкой, как любую
        // панель приложения. Экземпляр один: закрытие прячет панель (HideOnClose),
        // повторный вызов из меню возвращает её со всеми настройками.
        RoiWizard.RoiWizardForm roiWizardForm;

        public void ShowRoiWizardForm()
        {
            if (this.roiWizardForm == null || this.roiWizardForm.IsDisposed)
            {
                this.roiWizardForm = new RoiWizard.RoiWizardForm(this.RoiWizardResolution);
                System.Drawing.Rectangle bounds = new System.Drawing.Rectangle(
                    this.Location.X + Math.Max(0, (this.Width - 1200) / 2),
                    this.Location.Y + Math.Max(0, (this.Height - 700) / 2),
                    1200, 700);
                this.roiWizardForm.Show(this.dockPanel1, bounds);
            }
            else
            {
                this.roiWizardForm.Show(this.dockPanel1);
                this.roiWizardForm.Activate();
            }
        }

        // Разрешение для мастера — из родной FWHM-калибровки активного спектра, приведённой
        // к тому виду, в котором его ждёт мастер: R в процентах на 662 кэВ. Калибровка
        // задана в каналах, поэтому ширина переводится в энергию так же, как в
        // DCPeakDetectionView: по краям окна ±FWHM/2.
        double RoiWizardResolution()
        {
            DocEnergySpectrum document = this.ActiveDocument;
            if (document == null || document.ActiveResultData == null)
            {
                return 0;
            }
            ResultData active = document.ActiveResultData;
            EnergySpectrum spectrum = active.EnergySpectrum;
            if (spectrum == null || spectrum.EnergyCalibration == null || active.FwhmCalibration == null)
            {
                return 0;
            }
            double channel = spectrum.EnergyCalibration.EnergyToChannel(662.0, maxChannels: spectrum.NumberOfChannels);
            double fwhmChannels = active.FwhmCalibration.ChannelToFwhm(channel);
            if (!(fwhmChannels > 0))
            {
                return 0;
            }
            double left = spectrum.EnergyCalibration.ChannelToEnergy(channel - fwhmChannels / 2.0);
            double right = spectrum.EnergyCalibration.ChannelToEnergy(channel + fwhmChannels / 2.0);
            return right > left ? (right - left) / 662.0 * 100.0 : 0;
        }
"""


def read(path):
    return io.open(path, encoding="utf-8-sig").read()


def write(path, text):
    # BOM сохраняется таким, каким был у файла: без этого правка на одну строку
    # показывалась бы в дифференциале как изменение всего заголовка файла —
    # чужие файлы должны меняться ровно там, куда вставлено.
    with io.open(path, "rb") as probe:
        bom = probe.read(3) == codecs.BOM_UTF8
    encoding = "utf-8-sig" if bom else "utf-8"
    io.open(path, "w", encoding=encoding, newline="").write(text)


def insert_after(text, anchor, addition, tag):
    """Вставить addition после строки anchor. Возвращает (текст, что произошло)."""
    if addition.strip() in text:
        return text, "уже есть: " + tag
    index = text.find(anchor)
    if index < 0:
        raise SystemExit("не найдена точка вставки для %s: %s" % (tag, anchor.strip()))
    end = text.find("\n", index) + 1
    return text[:end] + addition + text[end:], "вставлено: " + tag


def patch_designer(root):
    path = os.path.join(root, "BecquerelMonitor", "MainForm.Designer.cs")
    text = read(path)
    steps = []

    text, note = insert_after(
        text,
        "            this.NuclideSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();",
        "            this.%s = new System.Windows.Forms.ToolStripMenuItem();\n" % MENU_ITEM,
        "создание пункта")
    steps.append(note)

    text, note = insert_after(
        text,
        "            this.NuclideSetToolStripMenuItem,",
        "            this.%s,\n" % MENU_ITEM,
        "пункт в меню")
    steps.append(note)

    text, note = insert_after(
        text,
        "            this.NuclideSetToolStripMenuItem.Click += new System.EventHandler(this.NuclideSetToolStripMenuItem_Click);",
        "            // \n"
        "            // %(name)s\n"
        "            // \n"
        "            this.%(name)s.Name = \"%(name)s\";\n"
        "            resources.ApplyResources(this.%(name)s, \"%(name)s\");\n"
        "            this.%(name)s.Click += new System.EventHandler(this.%(name)s_Click);\n"
        % {"name": MENU_ITEM},
        "настройка пункта")
    steps.append(note)

    text, note = insert_after(
        text,
        "\t\tglobal::System.Windows.Forms.ToolStripMenuItem NuclideSetToolStripMenuItem;",
        "\n\t\tglobal::System.Windows.Forms.ToolStripMenuItem %s;\n" % MENU_ITEM,
        "объявление поля")
    steps.append(note)

    write(path, text)
    return steps


def patch_mainform(root):
    path = os.path.join(root, "BecquerelMonitor", "MainForm.cs")
    text = read(path)
    if "ShowRoiWizardForm" in text:
        return ["уже есть: обработчик и ShowRoiWizardForm"]
    anchor = """        public void ShowNuclideSetForm()
        {
            NuclideSetForm form = new NuclideSetForm(this);
            form.ShowDialog();
        }
"""
    if anchor not in text:
        raise SystemExit("не найден ShowNuclideSetForm — образец, рядом с которым встраиваемся")
    write(path, text.replace(anchor, anchor + MAINFORM_CODE, 1))
    return ["вставлено: обработчик и ShowRoiWizardForm"]


def patch_resx(root, filename, entries):
    path = os.path.join(root, "BecquerelMonitor", filename)
    text = read(path)
    if MENU_ITEM in text:
        return ["уже есть: " + filename]
    marker = "</root>"
    if marker not in text:
        raise SystemExit("не похоже на .resx: " + path)
    write(path, text.replace(marker, entries + marker, 1))
    return ["вставлено: " + filename]


def patch_csproj(root):
    path = os.path.join(root, "BecquerelMonitor", "BecquerelMonitor.csproj")
    text = read(path)
    steps = []
    # Записи проверяются по одной: в дереве, куда патч уже применяли, часть файлов
    # прописана, а новый файл модуля иначе молча не попал бы в сборку. Запись —
    # это блок: либо самозакрытая строка, либо <Compile …> … </Compile> с потрохами
    # (SubType, DependentUpon). Вставлять такой блок построчно нельзя — .csproj
    # перестаёт разбираться.
    blocks = []
    for line in CSPROJ_ENTRIES.strip("\n").split("\n"):
        if line.lstrip().startswith("<Compile Include="):
            blocks.append([line])
        elif blocks:
            blocks[-1].append(line)
    missing = ["\n".join(block) for block in blocks if block[0].split('"')[1] not in text]
    if not missing:
        steps.append("уже есть: записи Compile")
    else:
        anchor = "  <ItemGroup>\n"
        index = text.find(anchor)
        if index < 0:
            raise SystemExit("не найден ItemGroup в .csproj")
        end = index + len(anchor)
        text = text[:end] + "\n".join(missing) + "\n" + text[end:]
        steps.append("вставлено записей Compile: %d" % len(missing))

    for resource in ["RoiWizard\\nuclides.xml", "RoiWizard\\help.xml",
                     "RoiWizard\\RoiWizardStrings.resx", "RoiWizard\\RoiWizardStrings.ru.resx"]:
        if resource in text:
            steps.append("уже есть: EmbeddedResource %s" % resource)
            continue
        anchor = "    <EmbeddedResource Include="
        index = text.find(anchor)
        if index < 0:
            raise SystemExit("не найден ни один EmbeddedResource в .csproj")
        text = (text[:index] +
                '    <EmbeddedResource Include="%s" />\n' % resource +
                text[index:])
        steps.append("вставлено: EmbeddedResource %s" % resource)

    write(path, text)
    return steps


def copy_module(root):
    target = os.path.join(root, "BecquerelMonitor", "RoiWizard")
    if not os.path.isdir(target):
        os.makedirs(target)
    copied = 0
    for name in sorted(os.listdir(MODULE)):
        if not (name.endswith(".cs") or name.endswith(".xml") or name.endswith(".resx")):
            continue
        shutil.copy2(os.path.join(MODULE, name), os.path.join(target, name))
        copied += 1
    return ["скопировано файлов модуля: %d" % copied]


def main():
    if len(sys.argv) < 2:
        raise SystemExit(__doc__)
    root = os.path.abspath(sys.argv[1])
    if not os.path.isfile(os.path.join(root, "BecquerelMonitor.sln")):
        raise SystemExit("в %s нет BecquerelMonitor.sln" % root)

    steps = []
    steps += copy_module(root)
    steps += patch_csproj(root)
    steps += patch_designer(root)
    steps += patch_mainform(root)
    steps += patch_resx(root, "MainForm.resx", """  <data name="%(name)s.Size" type="System.Drawing.Size, System.Drawing">
    <value>224, 22</value>
  </data>
  <data name="%(name)s.Text" xml:space="preserve">
    <value>ROI and nuclide set builder...</value>
  </data>
""" % {"name": MENU_ITEM})
    steps += patch_resx(root, "MainForm.ru.resx", """  <data name="%(name)s.Text" xml:space="preserve">
    <value>Конструктор ROI и наборов нуклидов...</value>
  </data>
""" % {"name": MENU_ITEM})

    for step in steps:
        print("  " + step)
    print("готово: %s" % root)


if __name__ == "__main__":
    main()
