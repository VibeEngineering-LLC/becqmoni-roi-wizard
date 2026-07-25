# Правки в самом BecqMoni: пункт меню

Модуль лежит целиком в `BecquerelMonitor/RoiWizard/` и приложение не трогает — кроме
одного: окно надо чем-то открыть. Здесь ровно то, что для этого добавляется в хост.

Четыре файла, шесть вставок. Ничего существующего не изменяется, только добавляется.

Проверено сборкой на снимке `master` от 25.07.2026 (`apply_patch.py` применяет всё это
к дереву и компилирует).

## 1. `MainForm.Designer.cs`

**а)** рядом с созданием соседних пунктов (после `this.NuclideSetToolStripMenuItem = …`):

```csharp
this.RoiWizardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
```

**б)** в список `DropDownItems` того же меню, следующей строкой после
`this.NuclideSetToolStripMenuItem,`:

```csharp
this.RoiWizardToolStripMenuItem,
```

**в)** настройка — рядом с блоком `// NuclideSetToolStripMenuItem`:

```csharp
// 
// RoiWizardToolStripMenuItem
// 
this.RoiWizardToolStripMenuItem.Name = "RoiWizardToolStripMenuItem";
resources.ApplyResources(this.RoiWizardToolStripMenuItem, "RoiWizardToolStripMenuItem");
this.RoiWizardToolStripMenuItem.Click += new System.EventHandler(this.RoiWizardToolStripMenuItem_Click);
```

**г)** объявление поля — рядом с `NuclideSetToolStripMenuItem`:

```csharp
global::System.Windows.Forms.ToolStripMenuItem RoiWizardToolStripMenuItem;
```

## 2. `MainForm.cs`

Рядом с `NuclideSetToolStripMenuItem_Click` и `ShowNuclideSetForm` — тот же образец:

```csharp
void RoiWizardToolStripMenuItem_Click(object sender, EventArgs e)
{
    this.ShowRoiWizardForm();
}

public void ShowRoiWizardForm()
{
    using (RoiWizard.RoiWizardForm form = new RoiWizard.RoiWizardForm(this.RoiWizardResolution))
    {
        form.ShowDialog(this);
    }
}

// Разрешение детектора для мастера: FWHM активного спектра в окне вокруг выбранного ROI.
// Вернуть 0, если брать неоткуда — форма тогда оставит значение, введённое руками.
double RoiWizardResolution()
{
    ResultData active = this.ActiveResultData;
    if (active == null || active.EnergySpectrum == null)
    {
        return 0;
    }
    EnergySpectrum spectrum = active.EnergySpectrum;
    ROIDefinitionData roi = this.SelectedROIDefinition;   // либо любой другой источник окна
    if (roi == null || roi.LowerLimit <= 0 || roi.UpperLimit <= roi.LowerLimit)
    {
        return 0;
    }
    int start = spectrum.EnergyCalibration.EnergyToChannel(roi.LowerLimit);
    int end = spectrum.EnergyCalibration.EnergyToChannel(roi.UpperLimit);
    EnergyResolutionResult result = EnergyResolutionCalculator.CalculateFWHM(spectrum, start, end);
    return result != null ? result.Resolution : 0;
}
```

Последний метод — единственное место, которое стоит подогнать под то, как в приложении
удобнее выбирать опорный пик: подпись `RoiWizardForm(Func<double>)`, всё остальное
безразлично. Есть и конструктор без аргументов — тогда кнопка «из спектра» на втором шаге
просто выключена, а форма работает автономно.

## 3. `MainForm.resx` (базовый, английский)

```xml
<data name="RoiWizardToolStripMenuItem.Size" type="System.Drawing.Size, System.Drawing">
  <value>224, 22</value>
</data>
<data name="RoiWizardToolStripMenuItem.Text" xml:space="preserve">
  <value>ROI and nuclide set builder...</value>
</data>
```

## 4. `MainForm.ru.resx`

```xml
<data name="RoiWizardToolStripMenuItem.Text" xml:space="preserve">
  <value>Конструктор ROI и наборов нуклидов...</value>
</data>
```

## Проверка

```
python integration\host-patch\apply_patch.py <путь к дереву BecqMoni>
```

Скрипт вставляет всё перечисленное, копирует `RoiWizard/` в проект и добавляет записи в
`.csproj`. Идемпотентен: повторный запуск ничего не дублирует. Предназначен для проверки
и для подготовки ветки под pull request, а не для постоянного использования.
