using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using XPTable.Models;

namespace BecquerelMonitor.RoiWizard
{
    // Окно конструктора: три шага повторяют веб-версию инструмента, но результат
    // никуда не выгружается файлом — ROI-конфигурация уходит в ROIConfigManager,
    // а набор нуклидов в NuclideDefinitionManager.
    public partial class RoiWizardForm : Form
    {
        readonly NuclideCatalog catalog;
        readonly SourceSelection selection = new SourceSelection();
        readonly LineSetBuilder builder;
        // пересоздаются при смене R: модель разрешения захватывается экземпляром,
        // иначе ширина зон считалась бы по устаревшему значению
        SetExporter exporter;
        ZoneCalculator zones;

        List<SpectralLine> lines = new List<SpectralLine>();
        List<SpectralLine> beforeMerge;
        // источник разрешения из хоста: FWHM-калибровка открытого спектра.
        // Если не задан, кнопка «из спектра» просто выключена — форма остаётся
        // самостоятельной и тестируемой без приложения.
        readonly Func<double> resolutionProvider;

        readonly List<string> groupKeys = new List<string>();
        readonly List<string> xrfSymbols = new List<string>();
        bool suspendEvents;

        public RoiWizardForm() : this(null)
        {
        }

        public RoiWizardForm(Func<double> resolutionProvider)
        {
            this.InitializeComponent();
            // цвета и шрифт — из темы веб-версии, чтобы окно выглядело так же
            WizardTheme.Apply(this);
            this.resolutionProvider = resolutionProvider;

            this.catalog = NuclideCatalog.GetInstance();
            this.builder = new LineSetBuilder(this.catalog).Reset();
            this.zones = new ZoneCalculator(this.Resolution);
            this.exporter = new SetExporter(this.Resolution, this.zones);

            this.FillCombos();
            this.FillGroups();
            this.FillXrf();
            this.RefreshCatalog();
            this.WireEvents();

            this.buttonFromSpectrum.Enabled = resolutionProvider != null;
            this.SyncSetControls();
            if (this.russian)
            {
                this.ApplyRussian();
            }
            // после ApplyRussian: списки и подсказки собираются из русских строк
            this.FillXrf();
            this.LayoutSources();
            this.LayoutLineColumns();
            this.RefreshGroupList();
            this.UpdateMergeInfo();
            this.UpdateStatus();
        }

        ResolutionModel Resolution
        {
            get { return new ResolutionModel((double)this.numResolution.Value); }
        }

        // ─── наполнение ─────────────────────────────────────────────────────

        void FillCombos()
        {
            this.comboCriterion.Items.AddRange(new object[] {
                "Sparrow limit — ROI markers (0.85·FWHM)",
                "anchored set — library fit (0.25·FWHM)",
                "manual"
            });
            this.comboCriterion.SelectedIndex = 0;

            this.comboIntensityMode.Items.AddRange(new object[] {
                "relative (within nuclide, max = 100)",
                "absolute (per decay)"
            });
            this.comboIntensityMode.SelectedIndex = 0;

            this.comboStyle.Items.AddRange(new object[] {
                "marker lines (height ∝ I, no zones)",
                "zones (limits around the peak)",
                "zones + intensity markers"
            });
            this.comboStyle.SelectedIndex = 0;

            this.comboWidthMode.Items.AddRange(new object[] {
                "% of energy (BecqMoni style)",
                "k × FWHM (scintillator)"
            });
            this.comboWidthMode.SelectedIndex = 0;

            object[] units = { "s", "h", "d", "y" };
            this.comboMinHalfLifeUnit.Items.AddRange(units);
            this.comboMinHalfLifeUnit.SelectedIndex = 2;      // сутки, как в вебе
            this.comboMaxHalfLifeUnit.Items.AddRange((object[])units.Clone());
            this.comboMaxHalfLifeUnit.SelectedIndex = 3;      // годы
            this.comboNearHalfLifeUnit.Items.AddRange((object[])units.Clone());
            this.comboNearHalfLifeUnit.SelectedIndex = 2;
            this.SyncZoneControls();
        }

        void FillGroups()
        {
            this.comboGroup.Items.Clear();
            this.groupKeys.Clear();
            string[] families = { "POPULAR", "NORM", "MED", "IND", "SNM", "FISS", "NAA", "WASTE" };
            foreach (string family in families)
            {
                int count = 0;
                foreach (CatalogNuclide nuclide in this.catalog.ByFamily(family))
                {
                    count++;
                }
                if (count == 0)
                {
                    continue;
                }
                this.groupKeys.Add("f:" + family);
                this.comboGroup.Items.Add(family + " (" + count + ")");
            }
            foreach (CatalogChain chain in this.catalog.Chains)
            {
                this.groupKeys.Add("c:" + chain.Id);
                this.comboGroup.Items.Add(chain.Title + " (" + chain.Members.Count + ")");
            }
            if (this.comboGroup.Items.Count > 0)
            {
                this.comboGroup.SelectedIndex = 0;
            }
        }

        void FillXrf()
        {
            this.checkedXrf.Items.Clear();
            this.xrfSymbols.Clear();
            foreach (XrfElement element in this.catalog.XrfElements)
            {
                this.xrfSymbols.Add(element.Symbol);
                string context = this.russian && !string.IsNullOrEmpty(element.ContextRu)
                    ? element.ContextRu
                    : element.Context;
                this.checkedXrf.Items.Add(element.Symbol + " — " + context);
            }
        }

        void RefreshCatalog()
        {
            string filter = this.textSearch.Text.Trim();
            this.tableCatalog.SuspendLayout();
            this.tableModelCatalog.Rows.Clear();
            foreach (CatalogNuclide nuclide in this.catalog.Nuclides)
            {
                if (filter.Length > 0 &&
                    nuclide.Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0 &&
                    (nuclide.Families ?? "").IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }
                Row row = new Row();
                row.Cells.Add(new Cell(nuclide.Name));
                row.Cells.Add(new Cell(nuclide.Families ?? ""));
                row.Cells.Add(new Cell(nuclide.HalfLifeText ?? "", nuclide.HalfLifeYears));
                // счётчики уходят рендереру парой чисел: «γ» и «X» красятся по-разному
                row.Cells.Add(new Cell(
                    nuclide.Gamma.Count.ToString(CultureInfo.InvariantCulture) + " " +
                    nuclide.Xray.Count.ToString(CultureInfo.InvariantCulture), nuclide.LineCount));
                if (nuclide.LineCount == 0)
                {
                    row.ForeColor = WizardTheme.NoLines;   // .nuc.nolines — нечего искать в спектре
                }
                row.Tag = nuclide;
                this.tableModelCatalog.Rows.Add(row);
            }
            this.tableCatalog.ResumeLayout();
            this.LayoutCatalogColumns();
        }

        // Три колонки шага 1 делят ширину поровну — .cols3 на странице задана как
        // grid-template-columns: repeat(3, 1fr). Привязки WinForms умеют только
        // «держать край», поэтому доли считаются здесь; полоса «Выбрано» прижата к низу.
        void LayoutSources()
        {
            int width = this.tabSources.ClientSize.Width;
            int height = this.tabSources.ClientSize.Height;
            if (width < 120 || height < 120)
            {
                return;
            }
            const int Pad = 8;
            const int Gap = 8;
            const int Top = 6;
            this.groupSelected.SetBounds(Pad, height - Pad - this.groupSelected.Height,
                                         width - Pad * 2, this.groupSelected.Height);
            int column = (width - Pad * 2 - Gap * 2) / 3;
            int boxHeight = this.groupSelected.Top - Gap - Top;
            this.groupSearch.SetBounds(Pad, Top, column, boxHeight);
            this.groupGroup.SetBounds(Pad + column + Gap, Top, column, boxHeight);
            this.groupXrf.SetBounds(Pad + (column + Gap) * 2, Top,
                                    width - Pad - (Pad + (column + Gap) * 2), boxHeight);
        }

        // Свободное место таблицы линий уходит в имя нуклида: с пометкой цепочки
        // «Ra-228 X L (Th-232)» подписи длинные, а числовые колонки фиксированы.
        // доли из разметки: имя, E, I, I отн., T½, тип
        static readonly int[] LineColumnShares = { 320, 90, 90, 80, 90, 80 };

        void LayoutLineColumns()
        {
            int free = this.tableLines.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4
                       - this.columnLineSelected.Width;
            if (free < 400)
            {
                return;
            }
            Column[] columns = {
                this.columnLineName, this.columnLineEnergy, this.columnLineIntensity,
                this.columnLineRelative, this.columnLineHalfLife, this.columnLineType };
            int total = 0;
            foreach (int share in LineColumnShares)
            {
                total += share;
            }
            int used = 0;
            for (int i = 0; i < columns.Length - 1; i++)
            {
                int width = free * LineColumnShares[i] / total;
                columns[i].Width = width;
                used += width;
            }
            columns[columns.Length - 1].Width = free - used;   // остаток — последней
        }

        // Свободное место забирает колонка семейств: «T½ γN X N» остаётся прижатым
        // к правому краю строки — это margin-left:auto у .nuc .hl на странице.
        void LayoutCatalogColumns()
        {
            int free = this.tableCatalog.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4
                       - this.columnCatalogName.Width
                       - this.columnCatalogHalfLife.Width
                       - this.columnCatalogLines.Width;
            if (free > 60)
            {
                this.columnCatalogFamilies.Width = free;
            }
        }

        // ─── события ────────────────────────────────────────────────────────

        void WireEvents()
        {
            this.tabSources.Resize += delegate { this.LayoutSources(); };
            this.tableCatalog.Resize += delegate { this.LayoutCatalogColumns(); };
            this.tableLines.Resize += delegate { this.LayoutLineColumns(); };
            this.textSearch.TextChanged += delegate { this.RefreshCatalog(); };
            this.buttonAddSingle.Click += delegate { this.AddFromCatalog(AddMode.Single); };
            this.buttonAddFamily.Click += delegate { this.AddFromCatalog(AddMode.FamilyLines); };
            this.buttonAddChain.Click += delegate { this.AddFromCatalog(AddMode.Chain); };
            this.tableCatalog.DoubleClick += delegate { this.AddFromCatalog(AddMode.Single); };

            this.comboGroup.SelectedIndexChanged += delegate { this.RefreshGroupList(); };
            this.checkedGroup.ItemCheck += this.OnGroupItemCheck;
            this.buttonGroupAll.Click += delegate { this.AddFromGroup(AddMode.Single); };
            this.buttonGroupFamily.Click += delegate { this.AddFromGroup(AddMode.FamilyLines); };
            this.buttonGroupChain.Click += delegate { this.AddFromGroup(AddMode.Chain); };
            this.checkedXrf.ItemCheck += this.OnXrfCheck;

            this.buttonClear.Click += delegate
            {
                this.selection.Clear();
                for (int i = 0; i < this.checkedXrf.Items.Count; i++)
                {
                    this.checkedXrf.SetItemChecked(i, false);
                }
                this.RefreshGroupList();
                this.Rebuild();
            };

            this.numResolution.ValueChanged += delegate { this.UpdateMergeInfo(); };
            this.comboCriterion.SelectedIndexChanged += this.OnCriterionChanged;
            this.numFactor.ValueChanged += delegate { this.UpdateMergeInfo(); };
            this.buttonMerge.Click += delegate { this.MergeLines(); };
            this.buttonUnmerge.Click += delegate { this.UnmergeLines(); };

            EventHandler rebuild = delegate { this.Rebuild(); };
            this.checkIntensity.CheckedChanged += rebuild;
            this.numMinIntensity.ValueChanged += rebuild;
            this.comboIntensityMode.SelectedIndexChanged += rebuild;
            this.checkEnergy.CheckedChanged += rebuild;
            this.numMinEnergy.ValueChanged += rebuild;
            this.numMaxEnergy.ValueChanged += rebuild;
            this.checkHalfLife.CheckedChanged += rebuild;
            this.numMinHalfLife.ValueChanged += rebuild;
            this.comboMinHalfLifeUnit.SelectedIndexChanged += rebuild;
            this.numMaxHalfLife.ValueChanged += rebuild;
            this.comboMaxHalfLifeUnit.SelectedIndexChanged += rebuild;
            EventHandler refreshLines = delegate { this.RefreshLines(); };
            this.checkHideUnselected.CheckedChanged += refreshLines;
            this.checkTypeGamma.CheckedChanged += refreshLines;
            this.checkTypeXray.CheckedChanged += refreshLines;
            this.checkTypeXrf.CheckedChanged += refreshLines;
            this.checkTypeSecondary.CheckedChanged += refreshLines;
            this.checkEquilibrium.CheckedChanged += rebuild;

            this.buttonSelectAll.Click += delegate { this.SetVisibleSelected(true); };
            this.buttonSelectNone.Click += delegate { this.SetVisibleSelected(false); };
            this.buttonGenerateSecondary.Click += delegate { this.GenerateSecondary(); };
            this.buttonNearSearch.Click += delegate { this.SearchNearby(); };
            this.buttonNearAdd.Click += delegate { this.AddFromNearby(); };
            this.listNear.DoubleClick += delegate { this.AddFromNearby(); };
            this.buttonSelectTop.Click += delegate
            {
                LineSetBuilder.SelectTopPerNuclide(this.lines, (int)this.numTopN.Value);
                this.RefreshLines();
            };
            this.tableLines.CellCheckChanged += this.OnLineCheckChanged;

            this.comboStyle.SelectedIndexChanged += delegate { this.SyncZoneControls(); this.RunChecks(); };
            this.comboWidthMode.SelectedIndexChanged += delegate { this.SyncZoneControls(); this.RunChecks(); };
            this.numZonePercent.ValueChanged += delegate { this.RunChecks(); };
            this.numZoneFactor.ValueChanged += delegate { this.RunChecks(); };
            this.buttonColorByChain.Click += delegate { this.SetColorMode(true); };
            this.buttonColorByNuclide.Click += delegate { this.SetColorMode(false); };
            this.buttonCreateRoi.Click += delegate { this.CreateRoiConfig(); };
            this.buttonCreateSet.Click += delegate { this.CreateNuclideSet(); };
            // при «полном наборе» таблица и ручной якорь не участвуют — набор собирается
            // заново из источников, поэтому выбор якоря отдаётся автоматике
            this.checkFullSet.CheckedChanged += delegate { this.SyncSetControls(); this.RunChecks(); };
            this.numAnchors.ValueChanged += delegate { this.RunChecks(); };
            this.buttonFromSpectrum.Click += delegate { this.TakeResolutionFromSpectrum(); };
            this.tabs.SelectedIndexChanged += delegate
            {
                if (this.tabs.SelectedTab == this.tabExport)
                {
                    this.RefreshAnchorCombo();
                    this.RunChecks();
                }
            };
        }

        void OnCriterionChanged(object sender, EventArgs e)
        {
            MergeCriterion criterion = (MergeCriterion)this.comboCriterion.SelectedIndex;
            this.suspendEvents = true;
            this.numFactor.Value = (decimal)MergeCriterionInfo.DefaultFactor(criterion);
            // предел Sparrow — величина физическая, менять её руками смысла нет
            this.numFactor.Enabled = criterion != MergeCriterion.Sparrow;
            this.suspendEvents = false;
            this.UpdateMergeInfo();
        }

        void OnXrfCheck(object sender, ItemCheckEventArgs e)
        {
            if (this.suspendEvents)
            {
                return;
            }
            string symbol = this.xrfSymbols[e.Index];
            if (e.NewValue == CheckState.Checked)
            {
                this.selection.XrfElements.Add(symbol);
            }
            else
            {
                this.selection.XrfElements.Remove(symbol);
            }
            this.BeginInvoke((MethodInvoker)delegate { this.Rebuild(); });
        }

        void OnLineCheckChanged(object sender, XPTable.Events.CellCheckBoxEventArgs e)
        {
            if (this.suspendEvents)
            {
                return;
            }
            Row row = this.tableModelLines.Rows[e.Row];
            SpectralLine line = row.Tag as SpectralLine;
            if (line != null)
            {
                line.Selected = row.Cells[0].Checked;
                this.UpdateStatus();
            }
        }

        // ─── выбор источников ───────────────────────────────────────────────

        void AddFromCatalog(AddMode mode)
        {
            CatalogNuclide nuclide = this.CurrentCatalogNuclide();
            if (nuclide == null)
            {
                return;
            }
            this.selection.Add(this.catalog, nuclide.Name, mode);
            this.Rebuild();
        }

        CatalogNuclide CurrentCatalogNuclide()
        {
            int index = this.tableCatalog.SelectedIndicies.Length > 0
                ? this.tableCatalog.SelectedIndicies[0]
                : -1;
            if (index < 0 || index >= this.tableModelCatalog.Rows.Count)
            {
                // если строка не выбрана — берём точное совпадение из поля поиска
                return this.catalog.Find(this.textSearch.Text.Trim());
            }
            return this.tableModelCatalog.Rows[index].Tag as CatalogNuclide;
        }

        // Члены выбранной группы с галочками — как в веб-версии: галочка означает
        // «нуклид взят», и она же выбирает цель для кнопок раскрытия.
        void RefreshGroupList()
        {
            int index = this.comboGroup.SelectedIndex;
            this.groupMembers.Clear();
            if (index >= 0 && index < this.groupKeys.Count)
            {
                string key = this.groupKeys[index];
                if (key.StartsWith("f:", StringComparison.Ordinal))
                {
                    foreach (CatalogNuclide nuclide in this.catalog.ByFamily(key.Substring(2)))
                    {
                        this.groupMembers.Add(nuclide.Name);
                    }
                }
                else
                {
                    CatalogChain chain = this.catalog.FindChain(key.Substring(2));
                    if (chain != null)
                    {
                        foreach (string member in chain.Members)
                        {
                            if (this.catalog.Find(member) != null)
                            {
                                this.groupMembers.Add(member);
                            }
                        }
                    }
                }
            }

            this.suppressGroupCheck = true;
            this.checkedGroup.BeginUpdate();
            this.checkedGroup.Items.Clear();
            foreach (string member in this.groupMembers)
            {
                CatalogNuclide nuclide = this.catalog.Find(member);
                string title = nuclide != null && !string.IsNullOrEmpty(nuclide.HalfLifeText)
                    ? member + "   " + nuclide.HalfLifeText
                    : member;
                this.checkedGroup.Items.Add(title, this.selection.Nuclides.ContainsKey(member));
            }
            this.checkedGroup.EndUpdate();
            this.suppressGroupCheck = false;
            this.SyncGroupButtons();
        }

        void OnGroupItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (this.suppressGroupCheck || e.Index < 0 || e.Index >= this.groupMembers.Count)
            {
                return;
            }
            string name = this.groupMembers[e.Index];
            if (e.NewValue == CheckState.Checked)
            {
                this.selection.AddGroupMember(this.catalog, name);
            }
            else
            {
                this.selection.Remove(name);
            }
            // область действия кнопок зависит от того, что отмечено
            this.BeginInvoke((MethodInvoker)delegate { this.SyncGroupButtons(); this.Rebuild(); });
        }

        // Отмеченные члены текущей группы — цели для кнопок раскрытия.
        List<string> GroupPicked()
        {
            List<string> picked = new List<string>();
            foreach (int index in this.checkedGroup.CheckedIndices)
            {
                if (index >= 0 && index < this.groupMembers.Count)
                {
                    picked.Add(this.groupMembers[index]);
                }
            }
            return picked;
        }

        // Раскрытие («+ линии семейства», «+ цепочка») применяется к отмеченным; если
        // не отмечено ничего — ко всей группе, и тогда оно осмысленно лишь там, где есть
        // кого раскрывать. У члена ЕРН-ряда родитель задан самим рядом: подменять его
        // предшественником нельзя, иначе цепочка развалится.
        void SyncGroupButtons()
        {
            int index = this.comboGroup.SelectedIndex;
            bool isChain = index >= 0 && index < this.groupKeys.Count &&
                           this.groupKeys[index].StartsWith("c:", StringComparison.Ordinal);
            List<string> picked = this.GroupPicked();
            bool expandable;
            if (picked.Count > 0)
            {
                expandable = false;
                foreach (string name in picked)
                {
                    if (this.HasDaughters(name))
                    {
                        expandable = true;
                        break;
                    }
                }
            }
            else if (isChain)
            {
                expandable = true;
            }
            else
            {
                expandable = false;
                foreach (string name in this.groupMembers)
                {
                    CatalogNuclide nuclide = this.catalog.Find(name);
                    if (nuclide != null && string.IsNullOrEmpty(nuclide.Chain) && this.HasDaughters(name))
                    {
                        expandable = true;
                        break;
                    }
                }
            }
            this.buttonGroupFamily.Enabled = expandable;
            this.buttonGroupChain.Enabled = expandable;
            this.labelGroupHint.Text = picked.Count > 0
                ? string.Format(CultureInfo.CurrentCulture, this.hintPicked, picked.Count)
                : this.hintNone;
        }

        bool HasDaughters(string name)
        {
            CatalogNuclide nuclide = this.catalog.Find(name);
            if (nuclide == null || string.IsNullOrEmpty(nuclide.Chain))
            {
                return false;
            }
            CatalogChain chain = this.catalog.FindChain(nuclide.Chain);
            if (chain == null)
            {
                return false;
            }
            int start = chain.Members.IndexOf(name);
            return start >= 0 && start < chain.Members.Count - 1;
        }

        void AddFromGroup(AddMode mode)
        {
            int index = this.comboGroup.SelectedIndex;
            if (index < 0 || index >= this.groupKeys.Count)
            {
                return;
            }
            // раскрытие — по отмеченным; «добавить все» всегда работает по группе
            List<string> picked = this.GroupPicked();
            if (mode != AddMode.Single && picked.Count > 0)
            {
                foreach (string name in picked)
                {
                    this.selection.Add(this.catalog, name, mode);
                }
                this.RefreshGroupList();
                this.Rebuild();
                return;
            }

            string key = this.groupKeys[index];
            if (key.StartsWith("f:", StringComparison.Ordinal))
            {
                foreach (CatalogNuclide nuclide in this.catalog.ByFamily(key.Substring(2)))
                {
                    this.selection.AddGroupMember(this.catalog, nuclide.Name);
                }
            }
            else
            {
                CatalogChain chain = this.catalog.FindChain(key.Substring(2));
                if (chain == null)
                {
                    return;
                }
                if (mode == AddMode.Single)
                {
                    foreach (string member in chain.Members)
                    {
                        this.selection.AddGroupMember(this.catalog, member);
                    }
                }
                else
                {
                    this.selection.Add(this.catalog, chain.Root, mode);
                }
            }
            this.RefreshGroupList();
            this.Rebuild();
        }

        // Чип убирает свой источник по клику — крестик на странице делает то же самое.
        void RemoveNuclide(string name)
        {
            this.selection.Remove(name);
            this.RefreshGroupList();
            this.Rebuild();
        }

        void RemoveXrf(string symbol)
        {
            for (int i = 0; i < this.xrfSymbols.Count; i++)
            {
                if (string.Equals(this.xrfSymbols[i], symbol, StringComparison.Ordinal))
                {
                    this.checkedXrf.SetItemChecked(i, false);   // снятие галки само уберёт элемент
                    return;
                }
            }
            this.selection.XrfElements.Remove(symbol);
            this.Rebuild();
        }

        // ─── пересборка набора ──────────────────────────────────────────────

        LineFilter CurrentFilter()
        {
            return new LineFilter
            {
                IntensityOn = this.checkIntensity.Checked,
                MinIntensity = (double)this.numMinIntensity.Value,
                RelativeIntensity = this.comboIntensityMode.SelectedIndex == 0,
                EnergyOn = this.checkEnergy.Checked,
                MinEnergy = (double)this.numMinEnergy.Value,
                MaxEnergy = (double)this.numMaxEnergy.Value,
                HalfLifeOn = this.checkHalfLife.Checked,
                MinHalfLifeYears = HalfLifeYears(this.numMinHalfLife, this.comboMinHalfLifeUnit),
                // пустое верхнее поле = «∞», как placeholder в вебе
                MaxHalfLifeYears = this.numMaxHalfLife.Value > 0
                    ? HalfLifeYears(this.numMaxHalfLife, this.comboMaxHalfLifeUnit)
                    : double.PositiveInfinity
            };
        }

        // единицы периода — те же, что в вебе: секунды, часы, сутки, годы
        static readonly double[] HalfLifeUnits = { 1.0 / 31557600.0, 1.0 / 8766.0, 1.0 / 365.25, 1.0 };

        static double HalfLifeYears(NumericUpDown value, ComboBox unit)
        {
            int index = unit.SelectedIndex >= 0 ? unit.SelectedIndex : HalfLifeUnits.Length - 1;
            return (double)value.Value * HalfLifeUnits[index];
        }

        void Rebuild()
        {
            this.builder.ScaleToSeriesParent = this.checkEquilibrium.Checked;
            this.lines = this.builder.Build(this.selection, this.CurrentFilter());
            this.beforeMerge = null;

            this.RefreshSelectedList();
            this.RefreshLines();
            this.RefreshColorChips();
        }

        // Полоса «Выбрано» — чипы .chip.on со страницы: фон --sel, рамка #7aa7ce,
        // текст --accent-ink и крестик, снимающий источник.
        void RefreshSelectedList()
        {
            this.panelSelected.SuspendLayout();
            this.panelSelected.Controls.Clear();
            foreach (KeyValuePair<string, string> entry in this.selection.Nuclides)
            {
                string name = entry.Key;
                this.panelSelected.Controls.Add(
                    this.Chip(name + " ×", delegate { this.RemoveNuclide(name); }));
            }
            foreach (string symbol in this.selection.XrfElements)
            {
                string element = symbol;
                this.panelSelected.Controls.Add(
                    this.Chip(this.xrfChipPrefix + element + " ×", delegate { this.RemoveXrf(element); }));
            }
            if (this.panelSelected.Controls.Count == 0)
            {
                Label empty = new Label();
                empty.Text = this.emptySelectionHint;
                empty.AutoSize = true;
                empty.ForeColor = WizardTheme.Muted;
                empty.Margin = new Padding(2, 4, 4, 2);
                this.panelSelected.Controls.Add(empty);
            }
            this.panelSelected.ResumeLayout();
        }

        Label Chip(string text, EventHandler onClick)
        {
            Label chip = new Label();
            chip.Text = text;
            chip.AutoSize = true;
            chip.Padding = new Padding(7, 1, 7, 1);     // .chip{padding:1px 7px}
            chip.Margin = new Padding(0, 2, 4, 2);      // .chipbar{gap:4px}
            chip.BackColor = WizardTheme.Selection;
            chip.ForeColor = WizardTheme.AccentInk;
            chip.Cursor = Cursors.Hand;
            chip.Click += onClick;
            // рамку рисуем сами: BorderStyle у Label даёт системный цвет, а нужен #7aa7ce
            chip.Paint += delegate(object sender, PaintEventArgs e)
            {
                Control control = (Control)sender;
                using (Pen pen = new Pen(WizardTheme.ChipLine))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, control.Width - 1, control.Height - 1);
                }
            };
            return chip;
        }

        string xrfChipPrefix = "XRF ";
        string emptySelectionHint = "empty — start with a group above";

        void RefreshLines()
        {
            this.suspendEvents = true;
            this.tableLines.SuspendLayout();
            this.tableModelLines.Rows.Clear();
            // «I отн.» — процент от сильнейшей линии того же нуклида, как в вебе
            Dictionary<string, double> strongest = new Dictionary<string, double>();
            foreach (SpectralLine line in this.lines)
            {
                double current;
                if (!strongest.TryGetValue(line.Nuclide, out current) || line.Intensity > current)
                {
                    strongest[line.Nuclide] = line.Intensity;
                }
            }
            bool hideUnselected = this.checkHideUnselected.Checked;
            foreach (SpectralLine line in this.lines)
            {
                // галки типов управляют видимостью, а не выбором: снятая «ХРИ» убирает
                // строки из таблицы, но линии остаются в наборе
                if (!this.IsTypeVisible(line.Type) || (hideUnselected && !line.Selected))
                {
                    continue;
                }
                double max;
                strongest.TryGetValue(line.Nuclide, out max);
                double relative = max > 0 ? 100.0 * line.Intensity / max : 0;

                Row row = new Row();
                row.Cells.Add(new Cell { Checked = line.Selected });
                row.Cells.Add(new Cell(line.Label));
                row.Cells.Add(new Cell(line.Energy.ToString("0.00", CultureInfo.CurrentCulture), line.Energy));
                row.Cells.Add(new Cell(line.Intensity.ToString("0.###", CultureInfo.CurrentCulture), line.Intensity));
                row.Cells.Add(new Cell(relative.ToString("0.#", CultureInfo.CurrentCulture), relative));
                row.Cells.Add(new Cell(line.HalfLifeText ?? "", line.HalfLifeYears));
                row.Cells.Add(new Cell(TypeName(line.Type)));
                row.Tag = line;
                this.tableModelLines.Rows.Add(row);
            }
            this.tableLines.ResumeLayout();
            this.suspendEvents = false;
            this.UpdateStatus();
        }

        bool IsTypeVisible(LineType type)
        {
            switch (type)
            {
                case LineType.Gamma: return this.checkTypeGamma.Checked;
                case LineType.Xray: return this.checkTypeXray.Checked;
                case LineType.Xrf: return this.checkTypeXrf.Checked;
                default: return this.checkTypeSecondary.Checked;
            }
        }

        static string TypeName(LineType type)
        {
            switch (type)
            {
                case LineType.Gamma: return "γ";
                case LineType.Xray: return "X";
                case LineType.Xrf: return "XRF";
                default: return "sec";
            }
        }

        // Кнопки работают по ВИДИМЫМ строкам — как в вебе: при включённом «скрыть
        // невыбранные» или фильтре типов «снять все» не должно трогать то, чего
        // пользователь сейчас не видит.
        // Виды особенностей — ровно те же восемь, что в вебе, и с теми же умолчаниями
        SecondaryKind SelectedSecondaryKinds()
        {
            SecondaryKind kinds = SecondaryKind.None;
            if (this.checkSecBackscatter.Checked) kinds |= SecondaryKind.Backscatter;
            if (this.checkSecComptonEdge.Checked) kinds |= SecondaryKind.ComptonEdge;
            if (this.checkSecSingleEscape.Checked) kinds |= SecondaryKind.SingleEscape;
            if (this.checkSecDoubleEscape.Checked) kinds |= SecondaryKind.DoubleEscape;
            if (this.checkSecIodine.Checked) kinds |= SecondaryKind.IodineEscape;
            if (this.checkSecAnnihilation.Checked) kinds |= SecondaryKind.Annihilation;
            if (this.checkSecSum.Checked) kinds |= SecondaryKind.CascadeSum;
            if (this.checkSecPileUp.Checked) kinds |= SecondaryKind.PileUp;
            return kinds;
        }

        void GenerateSecondary()
        {
            SecondaryKind kinds = this.SelectedSecondaryKinds();
            if (kinds == SecondaryKind.None)
            {
                return;
            }
            // прежние маркеры заменяются: иначе повторное нажатие плодит дубли
            this.lines.RemoveAll(delegate(SpectralLine line) { return line.Type == LineType.Secondary; });
            List<SpectralLine> generated = SecondaryPeaks.Generate(
                this.lines, this.Resolution, kinds, (double)this.numSecondaryMin.Value);
            this.lines.AddRange(generated);
            this.RefreshLines();
            this.statusLabel.Text = string.Format(CultureInfo.CurrentCulture,
                this.secondaryFormat, generated.Count);
        }

        // Кто ещё светит рядом: та же выборка, что на странице — γ и X всех нуклидов базы
        // плюс линии ХРИ, отсортированные по удалённости от заданной энергии.
        // Классический случай — 186 кэВ: Ra-226 3,6 % против U-235 57,2 %.
        void SearchNearby()
        {
            double energy = (double)this.numNearEnergy.Value;
            double window = (double)this.numNearWindow.Value;
            double minIntensity = (double)this.numNearIntensity.Value;
            double minHalfLife = HalfLifeYears(this.numNearHalfLife, this.comboNearHalfLifeUnit);

            this.nearHits.Clear();
            foreach (CatalogNuclide nuclide in this.catalog.Nuclides)
            {
                if (minHalfLife > 0 && nuclide.HalfLifeYears < minHalfLife)
                {
                    continue;
                }
                foreach (CatalogGammaLine gamma in nuclide.Gamma)
                {
                    if (Math.Abs(gamma.Energy - energy) <= window && gamma.Intensity >= minIntensity)
                    {
                        this.nearHits.Add(new NearHit(nuclide.Name, gamma.Energy, gamma.Intensity,
                                                      "γ", nuclide.HalfLifeText, null));
                    }
                }
                foreach (CatalogXrayLine xray in nuclide.Xray)
                {
                    if (Math.Abs(xray.Energy - energy) <= window && xray.Intensity >= minIntensity)
                    {
                        this.nearHits.Add(new NearHit(nuclide.Name, xray.Energy, xray.Intensity,
                                                      "X " + xray.Shell, nuclide.HalfLifeText, null));
                    }
                }
            }
            foreach (XrfElement element in this.catalog.XrfElements)
            {
                foreach (XrfLine line in element.Lines)
                {
                    if (Math.Abs(line.Energy - energy) <= window)
                    {
                        this.nearHits.Add(new NearHit("XRF " + element.Symbol, line.Energy, line.Intensity,
                                                      "XRF " + line.Label, "—", element.Symbol));
                    }
                }
            }
            double centre = energy;
            this.nearHits.Sort(delegate(NearHit a, NearHit b)
            {
                return Math.Abs(a.Energy - centre).CompareTo(Math.Abs(b.Energy - centre));
            });

            this.listNear.BeginUpdate();
            this.listNear.Items.Clear();
            foreach (NearHit hit in this.nearHits)
            {
                double delta = hit.Energy - energy;
                bool added = hit.XrfSymbol != null
                    ? this.selection.XrfElements.Contains(hit.XrfSymbol)
                    : this.selection.Nuclides.ContainsKey(hit.Nuclide);
                this.listNear.Items.Add(string.Format(CultureInfo.CurrentCulture,
                    "{0}{1:0.0}   {2,-14} {3,9:0.00} кэВ   I {4,7:0.###} %   {5,-8} {6}{7}",
                    delta >= 0 ? "+" : "", delta, hit.Nuclide, hit.Energy, hit.Intensity,
                    hit.Type, hit.HalfLife, added ? "   ✓" : ""));
            }
            this.listNear.EndUpdate();
            if (this.nearHits.Count == 0)
            {
                this.listNear.Items.Add(string.Format(CultureInfo.CurrentCulture,
                    this.nearEmptyFormat, energy, window));
            }
        }

        void AddFromNearby()
        {
            int index = this.listNear.SelectedIndex;
            if (index < 0 || index >= this.nearHits.Count)
            {
                return;
            }
            NearHit hit = this.nearHits[index];
            if (hit.XrfSymbol != null)
            {
                this.selection.XrfElements.Add(hit.XrfSymbol);
                for (int i = 0; i < this.xrfSymbols.Count; i++)
                {
                    if (string.Equals(this.xrfSymbols[i], hit.XrfSymbol, StringComparison.Ordinal))
                    {
                        this.checkedXrf.SetItemChecked(i, true);
                    }
                }
            }
            else
            {
                this.selection.Add(this.catalog, hit.Nuclide, AddMode.Single);
            }
            this.RefreshGroupList();
            this.Rebuild();
            this.SearchNearby();
        }

        sealed class NearHit
        {
            public readonly string Nuclide;
            public readonly double Energy;
            public readonly double Intensity;
            public readonly string Type;
            public readonly string HalfLife;
            public readonly string XrfSymbol;

            public NearHit(string nuclide, double energy, double intensity,
                           string type, string halfLife, string xrfSymbol)
            {
                this.Nuclide = nuclide;
                this.Energy = energy;
                this.Intensity = intensity;
                this.Type = type;
                this.HalfLife = string.IsNullOrEmpty(halfLife) ? "—" : halfLife;
                this.XrfSymbol = xrfSymbol;
            }
        }

        readonly List<NearHit> nearHits = new List<NearHit>();

        void SetColorMode(bool byChain)
        {
            this.colorByChain = byChain;
            this.buttonColorByChain.Enabled = !byChain;
            this.buttonColorByNuclide.Enabled = byChain;
            this.RefreshColorChips();
        }

        void SetVisibleSelected(bool value)
        {
            foreach (Row row in this.tableModelLines.Rows)
            {
                SpectralLine line = row.Tag as SpectralLine;
                if (line != null)
                {
                    line.Selected = value;
                }
            }
            this.RefreshLines();
        }

        // ─── слияние ────────────────────────────────────────────────────────

        void MergeLines()
        {
            if (this.lines.Count == 0)
            {
                return;
            }
            if (this.beforeMerge == null)
            {
                this.beforeMerge = new List<SpectralLine>(this.lines);
            }
            LineMerger merger = new LineMerger(this.Resolution, (double)this.numFactor.Value);
            this.lines = merger.Merge(this.beforeMerge);
            this.RefreshLines();
            this.statusLabel.Text = string.Format(CultureInfo.CurrentCulture,
                "merged groups: {0}, lines absorbed: {1}", merger.MergedGroups, merger.AbsorbedLines);
        }

        void UnmergeLines()
        {
            if (this.beforeMerge == null)
            {
                return;
            }
            this.lines = new List<SpectralLine>(this.beforeMerge);
            this.beforeMerge = null;
            this.RefreshLines();
        }

        void UpdateMergeInfo()
        {
            if (this.suspendEvents)
            {
                return;
            }
            LineMerger merger = new LineMerger(this.Resolution, (double)this.numFactor.Value);
            this.labelMergeInfo.Text = string.Format(CultureInfo.CurrentCulture, this.mergeInfoFormat,
                this.numFactor.Value, merger.ThresholdAt(100), merger.ThresholdAt(662), merger.ThresholdAt(1500));
        }

        void TakeResolutionFromSpectrum()
        {
            if (this.resolutionProvider == null)
            {
                return;
            }
            double value = this.resolutionProvider();
            if (value > 0)
            {
                this.numResolution.Value = Math.Min(this.numResolution.Maximum,
                    Math.Max(this.numResolution.Minimum, (decimal)value));
            }
            else
            {
                MessageBox.Show(this, "The resolution could not be taken from the active spectrum.",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ─── шаг 3 ──────────────────────────────────────────────────────────

        void SyncZoneControls()
        {
            bool zones = this.comboStyle.SelectedIndex != 0;
            this.comboWidthMode.Enabled = zones;
            this.numZonePercent.Enabled = zones && this.comboWidthMode.SelectedIndex == 0;
            this.numZoneFactor.Enabled = zones && this.comboWidthMode.SelectedIndex == 1;
            this.ApplyExporterSettings();
        }

        void ApplyExporterSettings()
        {
            this.zones = new ZoneCalculator(this.Resolution);
            this.zones.Style = (RoiStyle)this.comboStyle.SelectedIndex;
            this.zones.WidthMode = (ZoneWidthMode)Math.Max(0, this.comboWidthMode.SelectedIndex);
            this.zones.ZonePercent = (double)this.numZonePercent.Value;
            this.zones.ZoneFwhmFactor = (double)this.numZoneFactor.Value;
            this.exporter = new SetExporter(this.Resolution, this.zones);
        }

        void RefreshAnchorCombo()
        {
            this.comboAnchor.Items.Clear();
            // Кандидаты держатся списком, а не вычисляются по индексу заново: список
            // выбранных линий меняется галками, и индекс в комбобоксе иначе съезжает
            // на соседнюю линию. ХРИ и вторичные маркеры в кандидаты не попадают —
            // якорь на них означал бы опору с условным положением или интенсивностью.
            this.anchorCandidates.Clear();
            SpectralLine automatic = AnchorPicker.Pick(this.SelectedLines(), this.Resolution);
            this.comboAnchor.Items.Add(automatic != null
                ? "auto — " + automatic.Label + " " + automatic.Energy.ToString("0.0", CultureInfo.CurrentCulture)
                : "auto");
            foreach (SpectralLine line in this.SelectedLines())
            {
                if (!AnchorPicker.IsAcceptable(line))
                {
                    continue;
                }
                this.anchorCandidates.Add(line);
                this.comboAnchor.Items.Add(line.Label + " " + line.Energy.ToString("0.0", CultureInfo.CurrentCulture));
            }
            this.comboAnchor.SelectedIndex = 0;
        }

        readonly List<SpectralLine> anchorCandidates = new List<SpectralLine>();
        readonly List<string> groupMembers = new List<string>();
        readonly bool russian =
            Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ru";
        string mergeInfoFormat =
            "threshold {0:0.##}·FWHM: lines merge closer than {1:0.#} keV at 100, {2:0.#} at 662, {3:0.#} at 1500";
        string statusFormat = "lines: {0} of {1} · nuclides: {2}";
        string secondaryFormat = "secondary markers added: {0}";
        string nearEmptyFormat = "nothing found within {0} ± {1} keV";
        bool suppressGroupCheck;
        string hintPicked = "Applies to the ticked nuclides ({0}).";
        string hintNone = "Tick a nuclide - the buttons apply to it.";

        void SyncSetControls()
        {
            this.comboAnchor.Enabled = !this.checkFullSet.Checked;
            this.labelAnchor.Enabled = !this.checkFullSet.Checked;
        }

        List<SpectralLine> SelectedLines()
        {
            List<SpectralLine> result = new List<SpectralLine>();
            foreach (SpectralLine line in this.lines)
            {
                if (line.Selected)
                {
                    result.Add(line);
                }
            }
            return result;
        }

        SpectralLine CurrentAnchor()
        {
            int index = this.comboAnchor.SelectedIndex;
            if (index <= 0)
            {
                return null;                       // 0 — автоматический выбор
            }
            return index - 1 < this.anchorCandidates.Count ? this.anchorCandidates[index - 1] : null;
        }

        void RunChecks()
        {
            this.ApplyExporterSettings();
            this.listIssues.BeginUpdate();
            this.listIssues.Items.Clear();
            foreach (SetIssue issue in SetChecker.Check(this.lines, false, this.zones))
            {
                this.listIssues.Items.Add("ROI · " + issue.Text);
            }
            // проверяется то, что реально уйдёт в библиотеку: при «полном наборе» это не
            // содержимое таблицы, а все линии источников
            SpectralLine manual = this.checkFullSet.Checked ? null : this.CurrentAnchor();
            List<SpectralLine> manualAnchors = null;
            if (manual != null)
            {
                manualAnchors = new List<SpectralLine>();
                manualAnchors.Add(manual);
            }
            foreach (SetIssue issue in SetChecker.Check(this.LibraryLines(), true, this.zones,
                                                        this.Resolution, manualAnchors))
            {
                if (issue.Level == IssueLevel.Error)
                {
                    this.listIssues.Items.Add("SET · " + issue.Text);
                }
            }
            if (this.listIssues.Items.Count == 0)
            {
                this.listIssues.Items.Add("no issues");
            }
            this.listIssues.EndUpdate();
        }

        void CreateRoiConfig()
        {
            List<SpectralLine> selected = this.SelectedLines();
            if (selected.Count == 0)
            {
                MessageBox.Show(this, "No lines selected.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.ApplyExporterSettings();

            List<SetIssue> issues = SetChecker.Check(this.lines, false, this.zones);
            if (issues.Count > 0 && !this.Confirm(issues, false))
            {
                return;
            }

            ROIConfigData built = this.exporter.BuildRoiConfig(this.lines, this.textConfigName.Text,
                                                              this.ColorOfLine);
            // SaveConfig пишет файл по Filename, поэтому вторая конфигурация с тем же именем
            // молча затрёт файл первой
            if (!this.ConfirmOverwriteRoi(built.Name))
            {
                return;
            }

            // Регистрировать конфигурацию обязан сам менеджер: CreateConfig кладёт её и в
            // ROIConfigList, и в ROIConfigMap, и поднимает ROIConfigListChanged. Простое
            // добавление в список оставило бы карту пустой, а SaveConfig начинается с
            // roiConfigMap[Guid] — то есть упал бы KeyNotFoundException.
            ROIConfigManager manager = ROIConfigManager.GetInstance();
            ROIConfigData config = manager.CreateConfig(SafeFileName(built.Name) + ".xml");
            if (config == null)
            {
                return;                                  // менеджер уже показал сообщение об ошибке
            }
            config.Name = built.Name;
            config.ROIDefinitions.Clear();
            config.ROIDefinitions.AddRange(built.ROIDefinitions);
            manager.SaveConfig(config);

            this.statusLabel.Text = string.Format(CultureInfo.CurrentCulture,
                "ROI configuration «{0}» created: {1} regions", config.Name, config.ROIDefinitions.Count);
        }

        // Что уходит в библиотеку: либо выбранное в таблице, либо полный набор — все линии
        // источников минуя галки, фильтры и слияние (профиль для библиотечного фита).
        List<SpectralLine> LibraryLines()
        {
            return this.checkFullSet.Checked
                ? this.builder.BuildFullSet(this.selection)
                : this.lines;
        }

        void CreateNuclideSet()
        {
            List<SpectralLine> library = this.LibraryLines();
            if (Count(library) == 0)
            {
                MessageBox.Show(this, "No lines selected.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Ручной якорь уходит один — его выбрал пользователь. При автовыборе
            // помечается несколько линий: LibraryPeakFitter требует, чтобы с найденным
            // пиком совпала хотя бы одна, и единственный якорь делает набор хрупким.
            SpectralLine manual = this.checkFullSet.Checked ? null : this.CurrentAnchor();
            List<SpectralLine> anchors = null;
            if (manual != null)
            {
                anchors = new List<SpectralLine>();
                anchors.Add(manual);
            }
            int anchorCount = (int)this.numAnchors.Value;

            // для набора совпавшие энергии и нулевая интенсивность — ошибки: две линии на
            // одной позиции вырождают подгонку амплитуд, а Intencity = 0 выбрасывает линию
            // из связки по цепочке
            List<SetIssue> issues = SetChecker.Check(library, true, this.zones, this.Resolution, anchors);
            List<SetIssue> errors = issues.FindAll(delegate(SetIssue i) { return i.Level == IssueLevel.Error; });
            if (errors.Count > 0)
            {
                this.Confirm(errors, true);
                return;
            }

            // повторное нажатие добавило бы в библиотеку полный дубль записей
            if (!this.ConfirmDuplicateSet(this.textSetName.Text))
            {
                return;
            }

            List<NuclideDefinition> definitions;
            NuclideSet set = this.exporter.BuildNuclideSet(library, this.textSetName.Text, this.ColorOfLine,
                                                           anchors, anchorCount, out definitions);

            NuclideDefinitionManager manager = NuclideDefinitionManager.GetInstance();
            manager.NuclideSets.Add(set);
            manager.NuclideDefinitions.AddRange(definitions);
            manager.SaveDefinitionFile();

            int marked = definitions.FindAll(delegate(NuclideDefinition d) { return d.IsAnchor; }).Count;
            this.statusLabel.Text = string.Format(CultureInfo.CurrentCulture,
                "set «{0}» added to the library: {1} lines, {2} anchor(s)",
                set.Name, definitions.Count, marked);
        }

        static int Count(List<SpectralLine> lines)
        {
            int count = 0;
            foreach (SpectralLine line in lines)
            {
                if (line.Selected)
                {
                    count++;
                }
            }
            return count;
        }

        bool ConfirmOverwriteRoi(string name)
        {
            string filename = SafeFileName(name) + ".xml";
            foreach (ROIConfigData existing in ROIConfigManager.GetInstance().ROIConfigList)
            {
                if (string.Equals(existing.Filename, filename, StringComparison.OrdinalIgnoreCase))
                {
                    return MessageBox.Show(this,
                        string.Format(CultureInfo.CurrentCulture,
                            "Конфигурация «{0}» уже есть — её файл будет перезаписан. Продолжить?", name),
                        this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
                }
            }
            return true;
        }

        bool ConfirmDuplicateSet(string name)
        {
            foreach (NuclideSet existing in NuclideDefinitionManager.GetInstance().NuclideSets)
            {
                if (string.Equals(existing.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return MessageBox.Show(this,
                        string.Format(CultureInfo.CurrentCulture,
                            "Набор «{0}» в библиотеке уже есть. Добавить ещё один с тем же именем?", name),
                        this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
                }
            }
            return true;
        }

        bool Confirm(List<SetIssue> issues, bool blocking)
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine(blocking
                ? "The set cannot be saved — the data check found errors:"
                : "The data check found issues:");
            text.AppendLine();
            for (int i = 0; i < issues.Count && i < 8; i++)
            {
                text.AppendLine("• " + issues[i].Text);
            }
            if (issues.Count > 8)
            {
                text.AppendLine("…");
            }
            if (blocking)
            {
                text.AppendLine();
                text.AppendLine("Two lines at the same energy make the amplitude fit degenerate, " +
                                "and zero intensity drops a line out of the chain coupling.");
                MessageBox.Show(this, text.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            text.AppendLine();
            text.Append("Save anyway?");
            return MessageBox.Show(this, text.ToString(), this.Text,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        // цвет по нуклиду: одинаковые для линий одного источника, чтобы набор читался
        // цвет назначается «владельцу»: цепочке или нуклиду — как в вебе
        readonly Dictionary<string, Color> colors = new Dictionary<string, Color>();
        bool colorByChain = true;

        string OwnerOf(SpectralLine line)
        {
            if (line.Type == LineType.Xrf)
            {
                return line.Nuclide;                       // ХРИ всегда красятся по элементу
            }
            if (!this.colorByChain)
            {
                return line.Nuclide;
            }
            CatalogNuclide nuclide = this.catalog.Find(line.Nuclide);
            string root = nuclide != null ? this.catalog.ChainRoot(nuclide) : null;
            return string.IsNullOrEmpty(root) ? line.Nuclide : root;
        }

        Color ColorForOwner(string owner)
        {
            Color color;
            if (!this.colors.TryGetValue(owner, out color))
            {
                color = Palette[this.colors.Count % Palette.Length];
                this.colors[owner] = color;
            }
            return color;
        }

        // Чипы владельцев: квадрат цвета и подпись; клик по квадрату — выбор цвета,
        // как «input type=color» на странице.
        void RefreshColorChips()
        {
            List<string> owners = new List<string>();
            foreach (SpectralLine line in this.lines)
            {
                string owner = this.OwnerOf(line);
                if (!owners.Contains(owner))
                {
                    owners.Add(owner);
                }
            }
            this.panelColors.SuspendLayout();
            this.panelColors.Controls.Clear();
            foreach (string owner in owners)
            {
                Panel swatch = new Panel();
                swatch.Size = new Size(18, 18);
                swatch.Margin = new Padding(2, 4, 4, 2);
                swatch.BackColor = this.ColorForOwner(owner);
                swatch.BorderStyle = BorderStyle.FixedSingle;
                swatch.Cursor = Cursors.Hand;
                string captured = owner;
                swatch.Click += delegate { this.PickColor(captured, swatch); };

                Label caption = new Label();
                caption.Text = owner;
                caption.AutoSize = true;
                caption.Margin = new Padding(0, 5, 14, 2);

                this.panelColors.Controls.Add(swatch);
                this.panelColors.Controls.Add(caption);
            }
            this.panelColors.ResumeLayout();
        }

        void PickColor(string owner, Panel swatch)
        {
            using (ColorDialog dialog = new ColorDialog())
            {
                dialog.Color = this.ColorForOwner(owner);
                dialog.FullOpen = true;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    this.colors[owner] = dialog.Color;
                    swatch.BackColor = dialog.Color;
                }
            }
        }

        static readonly Color[] Palette = {
            Color.FromArgb(230, 130, 30), Color.FromArgb(192, 53, 53), Color.FromArgb(46, 125, 50),
            Color.FromArgb(21, 101, 192), Color.FromArgb(123, 31, 162), Color.FromArgb(0, 131, 143),
            Color.FromArgb(158, 122, 16), Color.FromArgb(216, 27, 96), Color.FromArgb(93, 64, 55)
        };

        Color ColorOfLine(SpectralLine line)
        {
            return this.ColorForOwner(this.OwnerOf(line));
        }

        static Color ColorOf(SpectralLine line)
        {
            int hash = 0;
            string key = line.Nuclide ?? "";
            for (int i = 0; i < key.Length; i++)
            {
                hash = (hash * 31 + key[i]) & 0x7fffffff;
            }
            return Palette[hash % Palette.Length];
        }

        static string SafeFileName(string name)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in name ?? "")
            {
                result.Append(Array.IndexOf(System.IO.Path.GetInvalidFileNameChars(), c) >= 0 ? '_' : c);
            }
            string text = result.ToString().Trim();
            return text.Length > 0 ? text : "ROI set";
        }

        void UpdateStatus()
        {
            int selected = 0;
            Dictionary<string, bool> nuclides = new Dictionary<string, bool>();
            foreach (SpectralLine line in this.lines)
            {
                if (line.Selected)
                {
                    selected++;
                    nuclides[line.Nuclide] = true;
                }
            }
            this.statusLabel.Text = string.Format(CultureInfo.CurrentCulture, this.statusFormat,
                selected, this.lines.Count, nuclides.Count);
        }

        // ─── русские подписи ────────────────────────────────────────────────
        // Штатный механизм WinForms (Localizable = true + RoiWizardForm.ru.resx) не
        // используется намеренно: разметка собрана руками, а держать координаты
        // контролов в ресурсах ради двух языков дороже, чем словарь подписей.
        void ApplyRussian()
        {
            this.Text = "Конструктор ROI и наборов нуклидов";
            this.tabSources.Text = "1 · Изотопы";
            this.tabLines.Text = "2 · Линии";
            this.tabExport.Text = "3 · Оформление и экспорт";

            this.groupSearch.Text = "Поиск изотопа";
            this.buttonAddSingle.Text = "Добавить";
            this.buttonAddFamily.Text = "+ семейство";
            this.buttonAddChain.Text = "+ цепочка";
            this.columnCatalogName.Text = "Нуклид";
            this.columnCatalogFamilies.Text = "Семейства";
            this.columnCatalogLines.Text = "Линий";

            this.groupGroup.Text = "Группа";
            this.buttonGroupAll.Text = "добавить все";
            this.buttonGroupFamily.Text = "+ линии семейства";
            this.buttonGroupChain.Text = "+ цепочкой";
            this.groupXrf.Text = "ХРИ — элементы";
            this.labelXrf.Text = "Материалы защиты и детектора:";
            this.hintPicked = "Применяется к отмеченным ({0}).";
            this.mergeInfoFormat = "порог {0:0.##}·FWHM: сливаются линии ближе {1:0.#} кэВ на 100, " +
                                   "{2:0.#} на 662, {3:0.#} на 1500";
            this.statusFormat = "линий: {0} из {1} · нуклидов: {2}";
            this.hintNone = "Отметьте нуклид — кнопки применятся к нему.";

            this.groupSelected.Text = "Выбрано";
            this.buttonClear.Text = "очистить всё";
            this.xrfChipPrefix = "ХРИ ";
            this.emptySelectionHint = "пусто — начните с группы выше";

            this.groupResolution.Text = "Адаптация под разрешение детектора";
            this.labelResolution.Text = "R, % на 662 кэВ";
            this.buttonFromSpectrum.Text = "из спектра";
            this.labelCriterion.Text = "критерий";
            this.buttonMerge.Text = "Объединить близкие";
            this.buttonUnmerge.Text = "Вернуть исходные";
            this.comboCriterion.Items.Clear();
            this.comboCriterion.Items.AddRange(new object[] {
                "предел Sparrow — маркеры ROI (0,85·FWHM)",
                "якорный набор — библиотечный фит (0,25·FWHM)",
                "вручную"
            });
            this.comboCriterion.SelectedIndex = 0;

            this.groupFilters.Text = "Фильтры и выбор";
            this.checkIntensity.Text = "интенсивность ≥, %";
            this.checkEnergy.Text = "энергия, кэВ";
            this.checkEquilibrium.Text = "равновесие ряда (интенсивности на распад родителя)";
            this.checkHalfLife.Text = "T½";
            this.checkHideUnselected.Text = "скрыть невыбранные";
            this.groupSecondary.Text = "Вторичные пики (расчёт по выбранным γ-линиям)";
            this.labelColors.Text = "Цвета";
            this.groupNear.Text = "Поиск близких линий (по всей базе — кто ещё светит рядом)";
            this.labelNearEnergy.Text = "энергия, кэВ";
            this.labelNearWindow.Text = "± окно";
            this.labelNearIntensity.Text = "I ≥, %";
            this.labelNearHalfLife.Text = "T½ ≥";
            this.buttonNearSearch.Text = "Искать";
            this.buttonNearAdd.Text = "+ добавить";
            this.comboNearHalfLifeUnit.Items.Clear();
            this.comboNearHalfLifeUnit.Items.AddRange(new object[] { "сек", "ч", "сут", "лет" });
            this.comboNearHalfLifeUnit.SelectedIndex = 2;
            this.nearEmptyFormat = "в окне {0} ± {1} кэВ ничего не найдено";
            this.buttonColorByChain.Text = "по цепочке";
            this.buttonColorByNuclide.Text = "по нуклиду";
            this.labelSecondaryMin.Text = "для γ-линий с I ≥, %";
            this.checkSecBackscatter.Text = "рассеяние назад (BS)";
            this.checkSecComptonEdge.Text = "комптон-край (CE)";
            this.checkSecSingleEscape.Text = "вылет 511 (SE)";
            this.checkSecDoubleEscape.Text = "вылет 1022 (DE)";
            this.checkSecIodine.Text = "вылет I-K (NaI, −28.6)";
            this.checkSecAnnihilation.Text = "аннигиляция 511";
            this.checkSecSum.Text = "суммирование каскадное (E1+E2)";
            this.checkSecPileUp.Text = "наложение 2×E";
            this.buttonGenerateSecondary.Text = "Сгенерировать";
            this.secondaryFormat = "добавлено вторичных маркеров: {0}";
            this.labelTypes.Text = "Тип линий";
            this.checkTypeXray.Text = "X (распад)";
            this.checkTypeXrf.Text = "ХРИ";
            this.checkTypeSecondary.Text = "вторичные";
            this.buttonSelectAll.Text = "✓ выбрать все видимые";
            this.buttonSelectNone.Text = "✗ снять все видимые";
            this.columnLineRelative.Text = "I отн., %";
            this.columnLineHalfLife.Text = "T½";
            this.comboMinHalfLifeUnit.Items.Clear();
            this.comboMinHalfLifeUnit.Items.AddRange(new object[] { "сек", "ч", "сут", "лет" });
            this.comboMinHalfLifeUnit.SelectedIndex = 2;
            this.comboMaxHalfLifeUnit.Items.Clear();
            this.comboMaxHalfLifeUnit.Items.AddRange(new object[] { "сек", "ч", "сут", "лет" });
            this.comboMaxHalfLifeUnit.SelectedIndex = 3;
            this.buttonSelectTop.Text = "топ-N на нуклид";
            this.comboIntensityMode.Items.Clear();
            this.comboIntensityMode.Items.AddRange(new object[] {
                "относительная (внутри изотопа, макс = 100)",
                "абсолютная (на распад)"
            });
            this.comboIntensityMode.SelectedIndex = 0;

            this.columnLineName.Text = "Нуклид";
            this.columnLineEnergy.Text = "E, кэВ";
            this.columnLineIntensity.Text = "I, %";
            this.columnLineType.Text = "Тип";

            this.groupStyle.Text = "Оформление ROI";
            this.labelStyle.Text = "режим";
            this.labelWidth.Text = "ширина зоны";
            this.comboStyle.Items.Clear();
            this.comboStyle.Items.AddRange(new object[] {
                "линии-маркеры (высота ∝ I, без зон)",
                "зоны (границы вокруг пика)",
                "зоны + маркеры интенсивности"
            });
            this.comboStyle.SelectedIndex = 0;
            this.comboWidthMode.Items.Clear();
            this.comboWidthMode.Items.AddRange(new object[] {
                "% от энергии (как в BecqMoni)",
                "k × FWHM (сцинтиллятор)"
            });
            this.comboWidthMode.SelectedIndex = 0;

            this.groupExport.Text = "Экспорт";
            this.labelConfigName.Text = "имя ROI-конфигурации";
            this.buttonCreateRoi.Text = "Создать ROI-конфигурацию";
            this.labelSetName.Text = "имя набора (NuclideSet)";
            this.labelAnchor.Text = "якорная линия";
            this.buttonCreateSet.Text = "Добавить набор в библиотеку";
            this.checkFullSet.Text = "полный набор (все линии, для фита)";
            this.labelAnchorCount.Text = "якорей";
            this.labelIssues.Text = "Проверка данных:";
            this.textSetName.Text = "Набор IAEA";
        }
    }
}
