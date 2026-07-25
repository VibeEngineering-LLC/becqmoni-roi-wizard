namespace BecquerelMonitor.RoiWizard
{
    partial class RoiWizardForm
    {
        System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // Разметка собрана руками, без дизайнера: три вкладки повторяют шаги веб-версии.
        // Подписи заданы по-английски — базовый язык интерфейса BecqMoni; русский
        // накладывается в RoiWizardForm.ApplyRussian() по текущей культуре UI.
        void InitializeComponent()
        {
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabSources = new System.Windows.Forms.TabPage();
            this.tabLines = new System.Windows.Forms.TabPage();
            this.tabExport = new System.Windows.Forms.TabPage();

            // — шаг 1: поиск и группы
            this.groupSearch = new System.Windows.Forms.GroupBox();
            this.textSearch = new System.Windows.Forms.TextBox();
            this.buttonAddSingle = new System.Windows.Forms.Button();
            this.buttonAddFamily = new System.Windows.Forms.Button();
            this.buttonAddChain = new System.Windows.Forms.Button();
            this.tableCatalog = new XPTable.Models.Table();
            this.columnModelCatalog = new XPTable.Models.ColumnModel();
            this.columnCatalogName = new XPTable.Models.TextColumn();
            this.columnCatalogFamilies = new XPTable.Models.TextColumn();
            this.columnCatalogHalfLife = new XPTable.Models.TextColumn();
            this.columnCatalogLines = new XPTable.Models.TextColumn();
            this.tableModelCatalog = new XPTable.Models.TableModel();

            this.groupGroup = new System.Windows.Forms.GroupBox();
            this.comboGroup = new System.Windows.Forms.ComboBox();
            this.buttonGroupAll = new System.Windows.Forms.Button();
            this.buttonGroupFamily = new System.Windows.Forms.Button();
            this.buttonGroupChain = new System.Windows.Forms.Button();
            this.checkedGroup = new System.Windows.Forms.CheckedListBox();
            this.buttonFamilyInfo = new System.Windows.Forms.Button();
            this.labelFamilyInfo = new System.Windows.Forms.Label();
            this.labelSearchHint = new System.Windows.Forms.Label();
            this.panelPresets = new System.Windows.Forms.FlowLayoutPanel();
            this.labelXrfHint = new System.Windows.Forms.Label();
            this.labelGroupHint = new System.Windows.Forms.Label();
            this.groupXrf = new System.Windows.Forms.GroupBox();
            this.checkedXrf = new System.Windows.Forms.CheckedListBox();
            this.labelXrf = new System.Windows.Forms.Label();

            this.groupSelected = new System.Windows.Forms.GroupBox();
            this.panelSelected = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonClear = new System.Windows.Forms.Button();

            // — шаг 2: разрешение, слияние, фильтры, таблица линий
            this.groupResolution = new System.Windows.Forms.GroupBox();
            this.labelResolution = new System.Windows.Forms.Label();
            this.numResolution = new System.Windows.Forms.NumericUpDown();
            this.buttonFromSpectrum = new System.Windows.Forms.Button();
            this.labelCriterion = new System.Windows.Forms.Label();
            this.comboCriterion = new System.Windows.Forms.ComboBox();
            this.numFactor = new System.Windows.Forms.NumericUpDown();
            this.labelFactor = new System.Windows.Forms.Label();
            this.buttonMerge = new System.Windows.Forms.Button();
            this.buttonUnmerge = new System.Windows.Forms.Button();
            this.labelMergeInfo = new System.Windows.Forms.Label();

            this.groupFilters = new System.Windows.Forms.GroupBox();
            this.checkIntensity = new System.Windows.Forms.CheckBox();
            this.numMinIntensity = new System.Windows.Forms.NumericUpDown();
            this.comboIntensityMode = new System.Windows.Forms.ComboBox();
            this.checkEnergy = new System.Windows.Forms.CheckBox();
            this.numMinEnergy = new System.Windows.Forms.NumericUpDown();
            this.numMaxEnergy = new System.Windows.Forms.NumericUpDown();
            this.checkHalfLife = new System.Windows.Forms.CheckBox();
            this.numMinHalfLife = new System.Windows.Forms.NumericUpDown();
            this.comboMinHalfLifeUnit = new System.Windows.Forms.ComboBox();
            this.numMaxHalfLife = new System.Windows.Forms.NumericUpDown();
            this.comboMaxHalfLifeUnit = new System.Windows.Forms.ComboBox();
            this.checkHideUnselected = new System.Windows.Forms.CheckBox();
            this.labelTypes = new System.Windows.Forms.Label();
            this.checkTypeGamma = new System.Windows.Forms.CheckBox();
            this.checkTypeXray = new System.Windows.Forms.CheckBox();
            this.checkTypeXrf = new System.Windows.Forms.CheckBox();
            this.checkTypeSecondary = new System.Windows.Forms.CheckBox();
            this.checkEquilibrium = new System.Windows.Forms.CheckBox();
            this.groupSecondary = new System.Windows.Forms.GroupBox();
            this.labelSecondaryMin = new System.Windows.Forms.Label();
            this.numSecondaryMin = new System.Windows.Forms.NumericUpDown();
            this.checkSecBackscatter = new System.Windows.Forms.CheckBox();
            this.checkSecComptonEdge = new System.Windows.Forms.CheckBox();
            this.checkSecSingleEscape = new System.Windows.Forms.CheckBox();
            this.checkSecDoubleEscape = new System.Windows.Forms.CheckBox();
            this.checkSecIodine = new System.Windows.Forms.CheckBox();
            this.checkSecAnnihilation = new System.Windows.Forms.CheckBox();
            this.checkSecSum = new System.Windows.Forms.CheckBox();
            this.checkSecPileUp = new System.Windows.Forms.CheckBox();
            this.buttonGenerateSecondary = new System.Windows.Forms.Button();
            this.groupNear = new System.Windows.Forms.GroupBox();
            this.labelNearEnergy = new System.Windows.Forms.Label();
            this.numNearEnergy = new System.Windows.Forms.NumericUpDown();
            this.labelNearWindow = new System.Windows.Forms.Label();
            this.numNearWindow = new System.Windows.Forms.NumericUpDown();
            this.labelNearIntensity = new System.Windows.Forms.Label();
            this.numNearIntensity = new System.Windows.Forms.NumericUpDown();
            this.labelNearHalfLife = new System.Windows.Forms.Label();
            this.numNearHalfLife = new System.Windows.Forms.NumericUpDown();
            this.comboNearHalfLifeUnit = new System.Windows.Forms.ComboBox();
            this.buttonNearSearch = new System.Windows.Forms.Button();
            this.listNear = new System.Windows.Forms.ListBox();
            this.buttonNearAdd = new System.Windows.Forms.Button();
            this.buttonSelectAll = new System.Windows.Forms.Button();
            this.buttonSelectNone = new System.Windows.Forms.Button();
            this.numTopN = new System.Windows.Forms.NumericUpDown();
            this.buttonSelectTop = new System.Windows.Forms.Button();

            this.tableLines = new XPTable.Models.Table();
            this.columnModelLines = new XPTable.Models.ColumnModel();
            this.columnLineSelected = new XPTable.Models.CheckBoxColumn();
            this.columnLineName = new XPTable.Models.TextColumn();
            this.columnLineEnergy = new XPTable.Models.TextColumn();
            this.columnLineIntensity = new XPTable.Models.TextColumn();
            this.columnLineRelative = new XPTable.Models.TextColumn();
            this.columnLineHalfLife = new XPTable.Models.TextColumn();
            this.columnLineType = new XPTable.Models.TextColumn();
            this.tableModelLines = new XPTable.Models.TableModel();

            // — шаг 3: оформление и экспорт
            this.groupStyle = new System.Windows.Forms.GroupBox();
            this.labelStyle = new System.Windows.Forms.Label();
            this.comboStyle = new System.Windows.Forms.ComboBox();
            this.labelWidth = new System.Windows.Forms.Label();
            this.comboWidthMode = new System.Windows.Forms.ComboBox();
            this.numZonePercent = new System.Windows.Forms.NumericUpDown();
            this.numZoneFactor = new System.Windows.Forms.NumericUpDown();
            this.labelColors = new System.Windows.Forms.Label();
            this.buttonColorByChain = new System.Windows.Forms.Button();
            this.buttonColorByNuclide = new System.Windows.Forms.Button();
            this.panelColors = new System.Windows.Forms.FlowLayoutPanel();

            this.groupExport = new System.Windows.Forms.GroupBox();
            this.labelConfigName = new System.Windows.Forms.Label();
            this.textConfigName = new System.Windows.Forms.TextBox();
            this.buttonCreateRoi = new System.Windows.Forms.Button();
            this.labelSetName = new System.Windows.Forms.Label();
            this.textSetName = new System.Windows.Forms.TextBox();
            this.labelAnchor = new System.Windows.Forms.Label();
            this.comboAnchor = new System.Windows.Forms.ComboBox();
            this.buttonCreateSet = new System.Windows.Forms.Button();
            this.checkFullSet = new System.Windows.Forms.CheckBox();
            this.labelAnchorCount = new System.Windows.Forms.Label();
            this.numAnchors = new System.Windows.Forms.NumericUpDown();
            this.listIssues = new System.Windows.Forms.ListBox();
            this.labelIssues = new System.Windows.Forms.Label();

            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();

            ((System.ComponentModel.ISupportInitialize)(this.numResolution)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFactor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinIntensity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinEnergy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxEnergy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTopN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinHalfLife)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxHalfLife)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSecondaryMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNearEnergy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNearWindow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNearIntensity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNearHalfLife)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numZonePercent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numZoneFactor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAnchors)).BeginInit();
            this.SuspendLayout();

            // ─── вкладки ───────────────────────────────────────────────────
            // размер задаётся до наполнения страниц: иначе дети запомнят расстояния
            // до краёв страницы размером 200x100 по умолчанию и на реальном разъедутся
            this.tabs.Size = new System.Drawing.Size(1180, 586);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Controls.Add(this.tabSources);
            this.tabs.Controls.Add(this.tabLines);
            this.tabs.Controls.Add(this.tabExport);

            // размер каждой странице явно: TabControl размечает только выбранную,
            // остальные остаются 200x100 и портят привязки своих детей
            this.tabSources.Size = new System.Drawing.Size(1172, 560);
            this.tabLines.Size = new System.Drawing.Size(1172, 560);
            this.tabExport.Size = new System.Drawing.Size(1172, 560);
            this.tabSources.Text = "1 · Nuclides";
            this.tabSources.Padding = new System.Windows.Forms.Padding(6);
            this.tabSources.UseVisualStyleBackColor = true;
            this.tabLines.Text = "2 · Lines";
            this.tabLines.Padding = new System.Windows.Forms.Padding(6);
            this.tabLines.UseVisualStyleBackColor = true;
            this.tabExport.Text = "3 · Styling and export";
            this.tabExport.Padding = new System.Windows.Forms.Padding(6);
            this.tabExport.UseVisualStyleBackColor = true;

            // ─── шаг 1 ─────────────────────────────────────────────────────
            this.groupSearch.Text = "Nuclide search";
            this.groupSearch.Location = new System.Drawing.Point(8, 6);
            this.groupSearch.Size = new System.Drawing.Size(376, 340);
            this.groupSearch.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;

            this.textSearch.Location = new System.Drawing.Point(8, 20);
            this.textSearch.Size = new System.Drawing.Size(360, 21);
            this.textSearch.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.buttonAddSingle.Text = "Add";
            this.buttonAddSingle.Location = new System.Drawing.Point(8, 48);
            this.buttonAddSingle.Size = new System.Drawing.Size(104, 25);
            this.buttonAddFamily.Text = "+ family";
            this.buttonAddFamily.Location = new System.Drawing.Point(118, 48);
            this.buttonAddFamily.Size = new System.Drawing.Size(122, 25);
            this.buttonAddChain.Text = "+ chain";
            this.buttonAddChain.Location = new System.Drawing.Point(246, 48);
            this.buttonAddChain.Size = new System.Drawing.Size(122, 25);

            this.tableCatalog.Location = new System.Drawing.Point(8, 80);
            this.tableCatalog.Size = new System.Drawing.Size(360, 190);
            this.tableCatalog.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;
            this.tableCatalog.BorderColor = System.Drawing.Color.Black;
            this.tableCatalog.ColumnModel = this.columnModelCatalog;
            this.tableCatalog.FullRowSelect = true;
            this.tableCatalog.GridLines = XPTable.Models.GridLines.Rows;
            this.tableCatalog.TableModel = this.tableModelCatalog;
            // строка списка нуклидов повторяет .nuc на странице: имя, бейджи семейств,
            // приглушённый хвост «T½ γN XN». Высота 18 px — line-height 16 плюс padding.
            this.tableModelCatalog.RowHeight = 18;
            this.columnCatalogName.Editable = false;   // таблицы только для чтения: правки идут через контролы
            this.columnCatalogName.Text = "Nuclide";
            this.columnCatalogName.Width = 72;
            this.columnCatalogFamilies.Editable = false;
            this.columnCatalogFamilies.Text = "Families";
            this.columnCatalogFamilies.Width = 132;
            this.columnCatalogFamilies.Renderer = new FamilyBadgeCellRenderer();
            this.columnCatalogHalfLife.Editable = false;
            this.columnCatalogHalfLife.Text = "T½";
            this.columnCatalogHalfLife.Width = 78;
            this.columnCatalogHalfLife.Renderer = new HintCellRenderer();
            this.columnCatalogLines.Editable = false;
            this.columnCatalogLines.Text = "Lines";
            this.columnCatalogLines.Width = 56;
            this.columnCatalogLines.Renderer = new LineCountCellRenderer();
            this.columnModelCatalog.Columns.AddRange(new XPTable.Models.Column[] {
                this.columnCatalogName, this.columnCatalogFamilies,
                this.columnCatalogHalfLife, this.columnCatalogLines });

            this.groupSearch.Controls.Add(this.textSearch);
            this.groupSearch.Controls.Add(this.buttonAddSingle);
            this.groupSearch.Controls.Add(this.buttonAddFamily);
            this.groupSearch.Controls.Add(this.buttonAddChain);
            this.labelSearchHint.Text = "Typing narrows the list: by name or by family code.";
            this.labelSearchHint.Location = new System.Drawing.Point(8, 274);
            this.labelSearchHint.Size = new System.Drawing.Size(360, 16);
            this.labelSearchHint.Anchor = System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            // строка пресетов: готовые наборы одним щелчком, как .presets на странице
            this.panelPresets.Location = new System.Drawing.Point(6, 292);
            this.panelPresets.Size = new System.Drawing.Size(364, 44);
            this.panelPresets.WrapContents = true;   // .presets переносится: flex-wrap:wrap
            this.panelPresets.AutoScroll = false;
            this.panelPresets.Anchor = System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.groupSearch.Controls.Add(this.tableCatalog);
            this.groupSearch.Controls.Add(this.labelSearchHint);
            this.groupSearch.Controls.Add(this.panelPresets);

            this.groupGroup.Text = "Group";
            this.groupGroup.Location = new System.Drawing.Point(392, 6);
            this.groupGroup.Size = new System.Drawing.Size(376, 340);
            this.groupGroup.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.comboGroup.Location = new System.Drawing.Point(8, 22);
            this.comboGroup.Size = new System.Drawing.Size(330, 23);
            this.comboGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboGroup.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.buttonFamilyInfo.Text = "i";
            this.buttonFamilyInfo.Location = new System.Drawing.Point(342, 22);
            this.buttonFamilyInfo.Size = new System.Drawing.Size(26, 23);
            this.buttonFamilyInfo.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Right;
            // словарик кодов — поверх списка, чтобы не двигать вёрстку (.infoPop)
            this.labelFamilyInfo.Location = new System.Drawing.Point(8, 47);
            this.labelFamilyInfo.Size = new System.Drawing.Size(360, 158);
            this.labelFamilyInfo.BackColor = System.Drawing.Color.FromArgb(255, 255, 225);
            this.labelFamilyInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelFamilyInfo.Padding = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.labelFamilyInfo.Visible = false;
            this.labelFamilyInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelFamilyInfo.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.buttonGroupAll.Text = "add all";
            this.buttonGroupAll.Location = new System.Drawing.Point(8, 50);
            this.buttonGroupAll.Size = new System.Drawing.Size(104, 25);
            this.buttonGroupFamily.Text = "+ family lines";
            this.buttonGroupFamily.Location = new System.Drawing.Point(118, 50);
            this.buttonGroupFamily.Size = new System.Drawing.Size(140, 25);
            this.buttonGroupChain.Text = "+ chain";
            this.buttonGroupChain.Location = new System.Drawing.Point(264, 50);
            this.buttonGroupChain.Size = new System.Drawing.Size(104, 25);
            this.checkedGroup.Location = new System.Drawing.Point(8, 82);
            this.checkedGroup.Size = new System.Drawing.Size(360, 230);
            this.checkedGroup.CheckOnClick = true;
            this.checkedGroup.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;
            this.labelGroupHint.Text = "Tick a nuclide - the buttons apply to it.";
            this.labelGroupHint.Location = new System.Drawing.Point(8, 316);
            this.labelGroupHint.Size = new System.Drawing.Size(360, 18);
            this.labelGroupHint.Anchor = System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;

            this.groupXrf.Text = "XRF elements";
            this.groupXrf.Location = new System.Drawing.Point(776, 6);
            this.groupXrf.Size = new System.Drawing.Size(396, 340);
            this.groupXrf.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;
            this.labelXrf.Text = "Shielding and detector materials:";
            this.labelXrf.Location = new System.Drawing.Point(8, 20);
            this.labelXrf.AutoSize = true;
            this.checkedXrf.Location = new System.Drawing.Point(8, 44);
            this.checkedXrf.Size = new System.Drawing.Size(380, 268);
            this.labelXrfHint.Text = "Ka/Kb (+L for heavy). Nominal intensities (Ka1 = 100) — markers only.";
            this.labelXrfHint.Location = new System.Drawing.Point(8, 316);
            this.labelXrfHint.Size = new System.Drawing.Size(380, 18);
            this.labelXrfHint.Anchor = System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.checkedXrf.CheckOnClick = true;
            this.checkedXrf.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;
            this.groupGroup.Controls.Add(this.labelFamilyInfo);
            this.groupGroup.Controls.Add(this.comboGroup);
            this.groupGroup.Controls.Add(this.buttonFamilyInfo);
            this.groupGroup.Controls.Add(this.buttonGroupAll);
            this.groupGroup.Controls.Add(this.buttonGroupFamily);
            this.groupGroup.Controls.Add(this.buttonGroupChain);
            this.groupGroup.Controls.Add(this.checkedGroup);
            this.groupGroup.Controls.Add(this.labelGroupHint);
            this.groupXrf.Controls.Add(this.labelXrf);
            this.groupXrf.Controls.Add(this.checkedXrf);
            this.groupXrf.Controls.Add(this.labelXrfHint);

            this.groupSelected.Text = "Selected";
            this.groupSelected.Location = new System.Drawing.Point(8, 352);
            this.groupSelected.Size = new System.Drawing.Size(1156, 72);
            this.groupSelected.Anchor = System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.panelSelected.Location = new System.Drawing.Point(8, 18);
            this.panelSelected.Size = new System.Drawing.Size(1038, 48);
            this.panelSelected.AutoScroll = true;
            this.panelSelected.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;
            this.buttonClear.Text = "clear all";
            this.buttonClear.Location = new System.Drawing.Point(1054, 18);
            this.buttonClear.Size = new System.Drawing.Size(94, 25);
            this.buttonClear.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Right;
            this.groupSelected.Controls.Add(this.panelSelected);
            this.groupSelected.Controls.Add(this.buttonClear);

            this.tabSources.Controls.Add(this.groupSearch);
            this.tabSources.Controls.Add(this.groupGroup);
            this.tabSources.Controls.Add(this.groupXrf);
            this.tabSources.Controls.Add(this.groupSelected);

            // ─── шаг 2 ─────────────────────────────────────────────────────
            this.groupResolution.Text = "Detector-resolution adaptation";
            this.groupResolution.Location = new System.Drawing.Point(8, 6);
            this.groupResolution.Size = new System.Drawing.Size(1156, 80);
            this.groupResolution.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.labelResolution.Text = "R, % at 662 keV";
            this.labelResolution.Location = new System.Drawing.Point(8, 23);
            this.labelResolution.AutoSize = true;
            this.numResolution.Location = new System.Drawing.Point(102, 20);
            this.numResolution.Size = new System.Drawing.Size(56, 21);
            this.numResolution.DecimalPlaces = 1;
            this.numResolution.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            this.numResolution.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            this.numResolution.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            this.numResolution.Value = new decimal(new int[] { 75, 0, 0, 65536 });
            this.buttonFromSpectrum.Text = "from spectrum";
            this.buttonFromSpectrum.Location = new System.Drawing.Point(164, 19);
            this.buttonFromSpectrum.Size = new System.Drawing.Size(104, 23);
            this.labelCriterion.Text = "criterion";
            this.labelCriterion.Location = new System.Drawing.Point(276, 23);
            this.labelCriterion.AutoSize = true;
            this.comboCriterion.Location = new System.Drawing.Point(330, 20);
            this.comboCriterion.Size = new System.Drawing.Size(300, 23);
            this.comboCriterion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.numFactor.Location = new System.Drawing.Point(576, 20);
            this.numFactor.Size = new System.Drawing.Size(56, 21);
            this.numFactor.DecimalPlaces = 2;
            this.numFactor.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            this.numFactor.Minimum = new decimal(new int[] { 5, 0, 0, 131072 });
            this.numFactor.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            this.numFactor.Value = new decimal(new int[] { 85, 0, 0, 131072 });
            this.labelFactor.Text = "× FWHM";
            this.labelFactor.Location = new System.Drawing.Point(638, 23);
            this.labelFactor.AutoSize = true;
            this.buttonMerge.Text = "Merge close lines";
            this.buttonMerge.Location = new System.Drawing.Point(838, 19);
            this.buttonMerge.Size = new System.Drawing.Size(150, 25);
            this.buttonUnmerge.Text = "Restore originals";
            this.buttonUnmerge.Location = new System.Drawing.Point(996, 19);
            this.buttonUnmerge.Size = new System.Drawing.Size(158, 25);
            this.groupResolution.Controls.Add(this.labelResolution);
            this.groupResolution.Controls.Add(this.numResolution);
            this.groupResolution.Controls.Add(this.buttonFromSpectrum);
            this.groupResolution.Controls.Add(this.labelCriterion);
            this.groupResolution.Controls.Add(this.comboCriterion);
            this.groupResolution.Controls.Add(this.numFactor);
            this.groupResolution.Controls.Add(this.labelFactor);
            this.groupResolution.Controls.Add(this.buttonMerge);
            this.groupResolution.Controls.Add(this.buttonUnmerge);

            this.labelMergeInfo.Location = new System.Drawing.Point(12, 62);
            this.labelMergeInfo.Size = new System.Drawing.Size(958, 16);
            this.labelMergeInfo.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;

            this.groupFilters.Text = "Filters and selection";
            this.groupFilters.Location = new System.Drawing.Point(8, 82);
            this.groupFilters.Size = new System.Drawing.Size(1156, 106);
            this.groupFilters.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.checkIntensity.Text = "intensity ≥, %";
            this.checkIntensity.Location = new System.Drawing.Point(8, 21);
            this.checkIntensity.Size = new System.Drawing.Size(124, 20);
            this.numMinIntensity.Location = new System.Drawing.Point(136, 20);
            this.numMinIntensity.Size = new System.Drawing.Size(52, 21);
            this.numMinIntensity.DecimalPlaces = 1;
            this.numMinIntensity.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numMinIntensity.Value = new decimal(new int[] { 3, 0, 0, 0 });
            this.comboIntensityMode.Location = new System.Drawing.Point(196, 20);
            this.comboIntensityMode.Size = new System.Drawing.Size(292, 23);
            this.comboIntensityMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.checkEnergy.Text = "energy, keV";
            this.checkEnergy.Location = new System.Drawing.Point(500, 21);
            this.checkEnergy.Size = new System.Drawing.Size(92, 20);
            this.numMinEnergy.Location = new System.Drawing.Point(596, 20);
            this.numMinEnergy.Size = new System.Drawing.Size(60, 21);
            this.numMinEnergy.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            this.numMinEnergy.Value = new decimal(new int[] { 10, 0, 0, 0 });
            this.numMaxEnergy.Location = new System.Drawing.Point(662, 20);
            this.numMaxEnergy.Size = new System.Drawing.Size(60, 21);
            this.numMaxEnergy.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            this.numMaxEnergy.Value = new decimal(new int[] { 3000, 0, 0, 0 });
            // фильтр по периоду полураспада — как в вебе: два поля со своими единицами
            this.checkHalfLife.Text = "T½";
            this.checkHalfLife.Location = new System.Drawing.Point(738, 21);
            this.checkHalfLife.Size = new System.Drawing.Size(40, 20);
            this.numMinHalfLife.Location = new System.Drawing.Point(782, 20);
            this.numMinHalfLife.Size = new System.Drawing.Size(52, 21);
            this.numMinHalfLife.DecimalPlaces = 2;
            this.numMinHalfLife.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            this.numMinHalfLife.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.comboMinHalfLifeUnit.Location = new System.Drawing.Point(840, 20);
            this.comboMinHalfLifeUnit.Size = new System.Drawing.Size(56, 21);
            this.comboMinHalfLifeUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.numMaxHalfLife.Location = new System.Drawing.Point(906, 20);
            this.numMaxHalfLife.Size = new System.Drawing.Size(52, 21);
            this.numMaxHalfLife.DecimalPlaces = 2;
            this.numMaxHalfLife.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            this.comboMaxHalfLifeUnit.Location = new System.Drawing.Point(964, 20);
            this.comboMaxHalfLifeUnit.Size = new System.Drawing.Size(56, 21);
            this.comboMaxHalfLifeUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.checkHideUnselected.Text = "hide unselected";
            this.checkHideUnselected.Location = new System.Drawing.Point(430, 47);
            this.checkHideUnselected.Size = new System.Drawing.Size(180, 20);

            this.checkEquilibrium.Text = "series equilibrium (intensities per one decay of the parent)";
            this.labelTypes.Text = "line types";
            this.labelTypes.Location = new System.Drawing.Point(8, 75);
            this.labelTypes.Size = new System.Drawing.Size(66, 16);
            this.checkTypeGamma.Text = "γ";
            this.checkTypeGamma.Location = new System.Drawing.Point(78, 73);
            this.checkTypeGamma.Size = new System.Drawing.Size(40, 20);
            this.checkTypeGamma.Checked = true;
            this.checkTypeXray.Text = "X (decay)";
            this.checkTypeXray.Location = new System.Drawing.Point(120, 73);
            this.checkTypeXray.Size = new System.Drawing.Size(90, 20);
            this.checkTypeXray.Checked = true;
            this.checkTypeXrf.Text = "XRF";
            this.checkTypeXrf.Location = new System.Drawing.Point(212, 73);
            this.checkTypeXrf.Size = new System.Drawing.Size(60, 20);
            this.checkTypeXrf.Checked = true;
            this.checkTypeSecondary.Text = "secondary";
            this.checkTypeSecondary.Location = new System.Drawing.Point(274, 73);
            this.checkTypeSecondary.Size = new System.Drawing.Size(96, 20);
            this.checkTypeSecondary.Checked = true;

            this.checkEquilibrium.Location = new System.Drawing.Point(402, 73);
            this.checkEquilibrium.Size = new System.Drawing.Size(560, 20);
            this.checkEquilibrium.Checked = true;
            this.buttonSelectAll.Text = "✓ select all visible";
            this.buttonSelectAll.Location = new System.Drawing.Point(8, 46);
            this.buttonSelectAll.Size = new System.Drawing.Size(140, 25);
            this.buttonSelectNone.Text = "✗ deselect all visible";
            this.buttonSelectNone.Location = new System.Drawing.Point(152, 46);
            this.buttonSelectNone.Size = new System.Drawing.Size(130, 25);
            this.numTopN.Location = new System.Drawing.Point(288, 47);
            this.numTopN.Size = new System.Drawing.Size(48, 21);
            this.numTopN.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numTopN.Value = new decimal(new int[] { 5, 0, 0, 0 });
            this.buttonSelectTop.Text = "top-N per nuclide";
            this.buttonSelectTop.Location = new System.Drawing.Point(280, 46);
            this.buttonSelectTop.Size = new System.Drawing.Size(140, 25);
            // Панель вторичных пиков повторяет блок веб-версии: порог по родительской
            // линии, восемь видов особенностей и кнопка расчёта. Расчёт по кнопке, а не
            // автоматически: маркеры добавляются к текущему набору линий.
            this.groupSecondary.Text = "Secondary peaks (computed from selected γ lines)";
            this.groupSecondary.Location = new System.Drawing.Point(8, 192);
            this.groupSecondary.Size = new System.Drawing.Size(1156, 78);
            this.groupSecondary.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.labelSecondaryMin.Text = "for γ lines with I ≥, %";
            this.labelSecondaryMin.Location = new System.Drawing.Point(8, 26);
            this.labelSecondaryMin.Size = new System.Drawing.Size(140, 18);
            this.numSecondaryMin.Location = new System.Drawing.Point(152, 23);
            this.numSecondaryMin.Size = new System.Drawing.Size(56, 23);
            this.numSecondaryMin.DecimalPlaces = 1;
            this.numSecondaryMin.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numSecondaryMin.Value = new decimal(new int[] { 10, 0, 0, 0 });
            this.checkSecBackscatter.Text = "backscatter (BS)";
            this.checkSecBackscatter.Location = new System.Drawing.Point(224, 24);
            this.checkSecBackscatter.Size = new System.Drawing.Size(150, 20);
            this.checkSecBackscatter.Checked = true;
            this.checkSecComptonEdge.Text = "Compton edge (CE)";
            this.checkSecComptonEdge.Location = new System.Drawing.Point(380, 24);
            this.checkSecComptonEdge.Size = new System.Drawing.Size(166, 20);
            this.checkSecSingleEscape.Text = "escape 511 (SE)";
            this.checkSecSingleEscape.Location = new System.Drawing.Point(552, 24);
            this.checkSecSingleEscape.Size = new System.Drawing.Size(146, 20);
            this.checkSecSingleEscape.Checked = true;
            this.checkSecDoubleEscape.Text = "escape 1022 (DE)";
            this.checkSecDoubleEscape.Location = new System.Drawing.Point(704, 24);
            this.checkSecDoubleEscape.Size = new System.Drawing.Size(156, 20);
            this.checkSecDoubleEscape.Checked = true;
            this.checkSecIodine.Text = "I-K escape (NaI, −28.6)";
            this.checkSecIodine.Location = new System.Drawing.Point(224, 48);
            this.checkSecIodine.Size = new System.Drawing.Size(190, 20);
            this.checkSecAnnihilation.Text = "annihilation 511";
            this.checkSecAnnihilation.Location = new System.Drawing.Point(420, 48);
            this.checkSecAnnihilation.Size = new System.Drawing.Size(146, 20);
            this.checkSecSum.Text = "cascade sum (E1+E2)";
            this.checkSecSum.Location = new System.Drawing.Point(572, 48);
            this.checkSecSum.Size = new System.Drawing.Size(180, 20);
            this.checkSecPileUp.Text = "pile-up 2×E";
            this.checkSecPileUp.Location = new System.Drawing.Point(758, 48);
            this.checkSecPileUp.Size = new System.Drawing.Size(120, 20);
            this.buttonGenerateSecondary.Text = "Generate";
            this.buttonGenerateSecondary.Location = new System.Drawing.Point(940, 22);
            this.buttonGenerateSecondary.Size = new System.Drawing.Size(150, 25);
            this.groupSecondary.Controls.Add(this.labelSecondaryMin);
            this.groupSecondary.Controls.Add(this.numSecondaryMin);
            this.groupSecondary.Controls.Add(this.checkSecBackscatter);
            this.groupSecondary.Controls.Add(this.checkSecComptonEdge);
            this.groupSecondary.Controls.Add(this.checkSecSingleEscape);
            this.groupSecondary.Controls.Add(this.checkSecDoubleEscape);
            this.groupSecondary.Controls.Add(this.checkSecIodine);
            this.groupSecondary.Controls.Add(this.checkSecAnnihilation);
            this.groupSecondary.Controls.Add(this.checkSecSum);
            this.groupSecondary.Controls.Add(this.checkSecPileUp);
            this.groupSecondary.Controls.Add(this.buttonGenerateSecondary);
            this.groupFilters.Controls.Add(this.checkIntensity);
            this.groupFilters.Controls.Add(this.numMinIntensity);
            this.groupFilters.Controls.Add(this.comboIntensityMode);
            this.groupFilters.Controls.Add(this.checkEnergy);
            this.groupFilters.Controls.Add(this.numMinEnergy);
            this.groupFilters.Controls.Add(this.numMaxEnergy);
            this.groupFilters.Controls.Add(this.checkHalfLife);
            this.groupFilters.Controls.Add(this.numMinHalfLife);
            this.groupFilters.Controls.Add(this.comboMinHalfLifeUnit);
            this.groupFilters.Controls.Add(this.numMaxHalfLife);
            this.groupFilters.Controls.Add(this.comboMaxHalfLifeUnit);
            this.groupFilters.Controls.Add(this.checkHideUnselected);
            this.groupFilters.Controls.Add(this.labelTypes);
            this.groupFilters.Controls.Add(this.checkTypeGamma);
            this.groupFilters.Controls.Add(this.checkTypeXray);
            this.groupFilters.Controls.Add(this.checkTypeXrf);
            this.groupFilters.Controls.Add(this.checkTypeSecondary);
            this.groupFilters.Controls.Add(this.checkEquilibrium);
            this.groupFilters.Controls.Add(this.buttonSelectAll);
            this.groupFilters.Controls.Add(this.buttonSelectNone);
            this.groupFilters.Controls.Add(this.numTopN);
            this.groupFilters.Controls.Add(this.buttonSelectTop);
            this.tabLines.Controls.Add(this.groupSecondary);

            this.groupNear.Text = "Nearby-line search (whole database — who else emits here)";
            this.groupNear.Location = new System.Drawing.Point(8, 276);
            this.groupNear.Size = new System.Drawing.Size(1156, 122);
            this.groupNear.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.labelNearEnergy.Text = "energy, keV";
            this.labelNearEnergy.Location = new System.Drawing.Point(8, 26);
            this.labelNearEnergy.Size = new System.Drawing.Size(90, 18);
            this.numNearEnergy.Location = new System.Drawing.Point(102, 23);
            this.numNearEnergy.Size = new System.Drawing.Size(72, 23);
            this.numNearEnergy.DecimalPlaces = 2;
            this.numNearEnergy.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            this.numNearEnergy.Value = new decimal(new int[] { 362, 0, 0, 0 });
            this.labelNearWindow.Text = "± window";
            this.labelNearWindow.Location = new System.Drawing.Point(186, 26);
            this.labelNearWindow.Size = new System.Drawing.Size(72, 18);
            this.numNearWindow.Location = new System.Drawing.Point(262, 23);
            this.numNearWindow.Size = new System.Drawing.Size(60, 23);
            this.numNearWindow.DecimalPlaces = 1;
            this.numNearWindow.Minimum = new decimal(new int[] { 5, 0, 0, 65536 });
            this.numNearWindow.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numNearWindow.Value = new decimal(new int[] { 10, 0, 0, 0 });
            this.labelNearIntensity.Text = "I ≥, %";
            this.labelNearIntensity.Location = new System.Drawing.Point(334, 26);
            this.labelNearIntensity.Size = new System.Drawing.Size(50, 18);
            this.numNearIntensity.Location = new System.Drawing.Point(388, 23);
            this.numNearIntensity.Size = new System.Drawing.Size(60, 23);
            this.numNearIntensity.DecimalPlaces = 2;
            this.numNearIntensity.Value = new decimal(new int[] { 5, 0, 0, 65536 });
            this.labelNearHalfLife.Text = "T½ ≥";
            this.labelNearHalfLife.Location = new System.Drawing.Point(460, 26);
            this.labelNearHalfLife.Size = new System.Drawing.Size(44, 18);
            this.numNearHalfLife.Location = new System.Drawing.Point(508, 23);
            this.numNearHalfLife.Size = new System.Drawing.Size(60, 23);
            this.numNearHalfLife.DecimalPlaces = 2;
            this.numNearHalfLife.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            this.comboNearHalfLifeUnit.Location = new System.Drawing.Point(574, 23);
            this.comboNearHalfLifeUnit.Size = new System.Drawing.Size(64, 23);
            this.comboNearHalfLifeUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.buttonNearSearch.Text = "Search";
            this.buttonNearSearch.Location = new System.Drawing.Point(654, 22);
            this.buttonNearSearch.Size = new System.Drawing.Size(110, 25);
            this.buttonNearAdd.Text = "+ add";
            this.buttonNearAdd.Location = new System.Drawing.Point(772, 22);
            this.buttonNearAdd.Size = new System.Drawing.Size(120, 25);
            this.listNear.Location = new System.Drawing.Point(8, 52);
            this.listNear.Size = new System.Drawing.Size(1140, 62);
            this.listNear.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.groupNear.Controls.Add(this.labelNearEnergy);
            this.groupNear.Controls.Add(this.numNearEnergy);
            this.groupNear.Controls.Add(this.labelNearWindow);
            this.groupNear.Controls.Add(this.numNearWindow);
            this.groupNear.Controls.Add(this.labelNearIntensity);
            this.groupNear.Controls.Add(this.numNearIntensity);
            this.groupNear.Controls.Add(this.labelNearHalfLife);
            this.groupNear.Controls.Add(this.numNearHalfLife);
            this.groupNear.Controls.Add(this.comboNearHalfLifeUnit);
            this.groupNear.Controls.Add(this.buttonNearSearch);
            this.groupNear.Controls.Add(this.buttonNearAdd);
            this.groupNear.Controls.Add(this.listNear);
            this.tabLines.Controls.Add(this.groupNear);

            this.tableLines.Location = new System.Drawing.Point(8, 404);
            this.tableLines.Size = new System.Drawing.Size(1156, 150);
            this.tableLines.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;
            this.tableLines.BorderColor = System.Drawing.Color.Black;
            this.tableLines.ColumnModel = this.columnModelLines;
            this.tableLines.FullRowSelect = true;
            this.tableLines.GridLines = XPTable.Models.GridLines.Rows;
            this.tableLines.TableModel = this.tableModelLines;
            this.columnLineSelected.Resizable = false;
            this.columnLineSelected.Sortable = false;
            this.columnLineSelected.Text = "✓";
            this.columnLineSelected.Width = 30;
            this.columnLineName.Editable = false;
            this.columnLineName.Text = "Nuclide";
            this.columnLineName.Width = 320;
            this.columnLineEnergy.Editable = false;
            this.columnLineEnergy.Text = "E, keV";
            this.columnLineEnergy.Width = 90;
            this.columnLineEnergy.Alignment = XPTable.Models.ColumnAlignment.Right;
            this.columnLineIntensity.Editable = false;
            this.columnLineIntensity.Text = "I, %";
            this.columnLineIntensity.Width = 90;
            this.columnLineIntensity.Alignment = XPTable.Models.ColumnAlignment.Right;
            this.columnLineRelative.Editable = false;
            this.columnLineRelative.Text = "I rel., %";
            this.columnLineRelative.Width = 80;
            this.columnLineRelative.Alignment = XPTable.Models.ColumnAlignment.Right;
            this.columnLineHalfLife.Editable = false;
            this.columnLineHalfLife.Text = "T½";
            this.columnLineHalfLife.Width = 90;
            this.columnLineHalfLife.Alignment = XPTable.Models.ColumnAlignment.Right;
            this.columnLineType.Editable = false;
            this.columnLineType.Text = "Type";
            this.columnLineType.Width = 80;
            this.columnModelLines.Columns.AddRange(new XPTable.Models.Column[] {
                this.columnLineSelected, this.columnLineName, this.columnLineEnergy,
                this.columnLineIntensity, this.columnLineRelative,
                this.columnLineHalfLife, this.columnLineType });

            this.tabLines.Controls.Add(this.groupResolution);
            this.tabLines.Controls.Add(this.labelMergeInfo);
            this.tabLines.Controls.Add(this.groupFilters);
            this.tabLines.Controls.Add(this.tableLines);

            // ─── шаг 3 ─────────────────────────────────────────────────────
            this.groupStyle.Text = "ROI styling";
            this.groupStyle.Location = new System.Drawing.Point(8, 6);
            this.groupStyle.Size = new System.Drawing.Size(1156, 104);
            this.groupStyle.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.labelStyle.Text = "mode";
            this.labelStyle.Location = new System.Drawing.Point(8, 23);
            this.labelStyle.AutoSize = true;
            this.comboStyle.Location = new System.Drawing.Point(56, 20);
            this.comboStyle.Size = new System.Drawing.Size(260, 21);
            this.comboStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.labelWidth.Text = "zone width";
            this.labelWidth.Location = new System.Drawing.Point(330, 23);
            this.labelWidth.AutoSize = true;
            this.comboWidthMode.Location = new System.Drawing.Point(402, 20);
            this.comboWidthMode.Size = new System.Drawing.Size(220, 21);
            this.comboWidthMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.numZonePercent.Location = new System.Drawing.Point(628, 20);
            this.numZonePercent.Size = new System.Drawing.Size(56, 21);
            this.numZonePercent.DecimalPlaces = 1;
            this.numZonePercent.Minimum = new decimal(new int[] { 5, 0, 0, 65536 });
            this.numZonePercent.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            this.numZonePercent.Value = new decimal(new int[] { 5, 0, 0, 0 });
            this.numZoneFactor.Location = new System.Drawing.Point(690, 20);
            this.numZoneFactor.Size = new System.Drawing.Size(56, 21);
            this.numZoneFactor.DecimalPlaces = 1;
            this.numZoneFactor.Minimum = new decimal(new int[] { 5, 0, 0, 65536 });
            this.numZoneFactor.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            this.numZoneFactor.Value = new decimal(new int[] { 3, 0, 0, 0 });
            this.groupStyle.Controls.Add(this.labelStyle);
            this.groupStyle.Controls.Add(this.comboStyle);
            this.groupStyle.Controls.Add(this.labelWidth);
            this.groupStyle.Controls.Add(this.comboWidthMode);
            this.groupStyle.Controls.Add(this.numZonePercent);
            this.labelColors.Text = "Colours";
            this.labelColors.Location = new System.Drawing.Point(8, 60);
            this.labelColors.Size = new System.Drawing.Size(70, 18);
            this.buttonColorByChain.Text = "by chain";
            this.buttonColorByChain.Location = new System.Drawing.Point(80, 56);
            this.buttonColorByChain.Size = new System.Drawing.Size(110, 25);
            this.buttonColorByNuclide.Text = "by nuclide";
            this.buttonColorByNuclide.Location = new System.Drawing.Point(196, 56);
            this.buttonColorByNuclide.Size = new System.Drawing.Size(110, 25);
            // чипы владельцев: цветной квадрат + подпись, клик по квадрату открывает выбор
            this.panelColors.Location = new System.Drawing.Point(316, 56);
            this.panelColors.Size = new System.Drawing.Size(836, 28);
            this.panelColors.AutoScroll = true;
            this.panelColors.WrapContents = false;
            this.groupStyle.Controls.Add(this.numZoneFactor);
            this.groupStyle.Controls.Add(this.labelColors);
            this.groupStyle.Controls.Add(this.buttonColorByChain);
            this.groupStyle.Controls.Add(this.buttonColorByNuclide);
            this.groupStyle.Controls.Add(this.panelColors);

            this.groupExport.Text = "Export";
            this.groupExport.Location = new System.Drawing.Point(8, 114);
            this.groupExport.Size = new System.Drawing.Size(1156, 120);
            this.groupExport.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.labelConfigName.Text = "ROI configuration name";
            this.labelConfigName.Location = new System.Drawing.Point(8, 23);
            this.labelConfigName.AutoSize = true;
            this.textConfigName.Location = new System.Drawing.Point(148, 20);
            this.textConfigName.Size = new System.Drawing.Size(220, 21);
            this.textConfigName.Text = "IAEA lines";
            this.buttonCreateRoi.Text = "Create ROI configuration";
            this.buttonCreateRoi.Location = new System.Drawing.Point(376, 19);
            this.buttonCreateRoi.Size = new System.Drawing.Size(180, 23);
            this.labelSetName.Text = "set name (NuclideSet)";
            this.labelSetName.Location = new System.Drawing.Point(8, 53);
            this.labelSetName.AutoSize = true;
            this.textSetName.Location = new System.Drawing.Point(148, 50);
            this.textSetName.Size = new System.Drawing.Size(220, 21);
            this.textSetName.Text = "IAEA set";
            this.labelAnchor.Text = "anchor line";
            this.labelAnchor.Location = new System.Drawing.Point(376, 53);
            this.labelAnchor.AutoSize = true;
            this.comboAnchor.Location = new System.Drawing.Point(468, 50);
            this.comboAnchor.Size = new System.Drawing.Size(278, 21);
            this.comboAnchor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.buttonCreateSet.Text = "Add set to the library";
            this.buttonCreateSet.Location = new System.Drawing.Point(754, 49);
            this.buttonCreateSet.Size = new System.Drawing.Size(192, 23);
            this.checkFullSet.Text = "full set (all lines, for fitting)";
            this.checkFullSet.Location = new System.Drawing.Point(148, 80);
            this.checkFullSet.Size = new System.Drawing.Size(220, 19);
            this.labelAnchorCount.Text = "anchor lines";
            this.labelAnchorCount.Location = new System.Drawing.Point(376, 82);
            this.labelAnchorCount.AutoSize = true;
            this.numAnchors.Location = new System.Drawing.Point(446, 79);
            this.numAnchors.Size = new System.Drawing.Size(60, 21);
            this.numAnchors.Minimum = 1;
            this.numAnchors.Maximum = 9;
            this.numAnchors.Value = 3;
            this.groupExport.Controls.Add(this.labelConfigName);
            this.groupExport.Controls.Add(this.textConfigName);
            this.groupExport.Controls.Add(this.buttonCreateRoi);
            this.groupExport.Controls.Add(this.labelSetName);
            this.groupExport.Controls.Add(this.textSetName);
            this.groupExport.Controls.Add(this.labelAnchor);
            this.groupExport.Controls.Add(this.comboAnchor);
            this.groupExport.Controls.Add(this.buttonCreateSet);
            this.groupExport.Controls.Add(this.checkFullSet);
            this.groupExport.Controls.Add(this.labelAnchorCount);
            this.groupExport.Controls.Add(this.numAnchors);

            this.labelIssues.Text = "Data check:";
            this.labelIssues.Location = new System.Drawing.Point(12, 240);
            this.labelIssues.AutoSize = true;
            this.listIssues.Location = new System.Drawing.Point(8, 258);
            this.listIssues.Size = new System.Drawing.Size(1156, 158);
            this.listIssues.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;
            this.listIssues.HorizontalScrollbar = true;

            this.tabExport.Controls.Add(this.groupStyle);
            this.tabExport.Controls.Add(this.groupExport);
            this.tabExport.Controls.Add(this.labelIssues);
            this.tabExport.Controls.Add(this.listIssues);

            // ─── строка состояния ──────────────────────────────────────────
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.statusLabel });
            this.statusLabel.Text = "";

            // ─── форма ─────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 608);
            this.MinimumSize = new System.Drawing.Size(1000, 500);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.statusStrip);
            this.Name = "RoiWizardForm";
            this.Text = "ROI and nuclide set builder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

            ((System.ComponentModel.ISupportInitialize)(this.numResolution)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFactor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinIntensity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinEnergy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxEnergy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTopN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinHalfLife)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxHalfLife)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSecondaryMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNearEnergy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNearWindow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNearIntensity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNearHalfLife)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numZonePercent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numZoneFactor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAnchors)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        System.Windows.Forms.TabControl tabs;
        System.Windows.Forms.TabPage tabSources;
        System.Windows.Forms.TabPage tabLines;
        System.Windows.Forms.TabPage tabExport;

        System.Windows.Forms.GroupBox groupSearch;
        System.Windows.Forms.TextBox textSearch;
        System.Windows.Forms.Button buttonAddSingle;
        System.Windows.Forms.Button buttonAddFamily;
        System.Windows.Forms.Button buttonAddChain;
        XPTable.Models.Table tableCatalog;
        XPTable.Models.ColumnModel columnModelCatalog;
        XPTable.Models.TextColumn columnCatalogName;
        XPTable.Models.TextColumn columnCatalogFamilies;
        XPTable.Models.TextColumn columnCatalogHalfLife;
        XPTable.Models.TextColumn columnCatalogLines;
        XPTable.Models.TableModel tableModelCatalog;

        System.Windows.Forms.GroupBox groupGroup;
        System.Windows.Forms.GroupBox groupXrf;
        System.Windows.Forms.CheckedListBox checkedGroup;
        System.Windows.Forms.Button buttonFamilyInfo;
        System.Windows.Forms.Label labelFamilyInfo;
        System.Windows.Forms.Label labelSearchHint;
        System.Windows.Forms.FlowLayoutPanel panelPresets;
        System.Windows.Forms.Label labelXrfHint;
        System.Windows.Forms.Label labelGroupHint;
        System.Windows.Forms.ComboBox comboGroup;
        System.Windows.Forms.Button buttonGroupAll;
        System.Windows.Forms.Button buttonGroupFamily;
        System.Windows.Forms.Button buttonGroupChain;
        System.Windows.Forms.Label labelXrf;
        System.Windows.Forms.CheckedListBox checkedXrf;

        System.Windows.Forms.GroupBox groupSelected;
        System.Windows.Forms.FlowLayoutPanel panelSelected;
        System.Windows.Forms.Button buttonClear;

        System.Windows.Forms.GroupBox groupResolution;
        System.Windows.Forms.Label labelResolution;
        System.Windows.Forms.NumericUpDown numResolution;
        System.Windows.Forms.Button buttonFromSpectrum;
        System.Windows.Forms.Label labelCriterion;
        System.Windows.Forms.ComboBox comboCriterion;
        System.Windows.Forms.NumericUpDown numFactor;
        System.Windows.Forms.Label labelFactor;
        System.Windows.Forms.Button buttonMerge;
        System.Windows.Forms.Button buttonUnmerge;
        System.Windows.Forms.Label labelMergeInfo;

        System.Windows.Forms.GroupBox groupFilters;
        System.Windows.Forms.CheckBox checkIntensity;
        System.Windows.Forms.NumericUpDown numMinIntensity;
        System.Windows.Forms.ComboBox comboIntensityMode;
        System.Windows.Forms.CheckBox checkEnergy;
        System.Windows.Forms.NumericUpDown numMinEnergy;
        System.Windows.Forms.NumericUpDown numMaxEnergy;
        System.Windows.Forms.CheckBox checkEquilibrium;
        System.Windows.Forms.GroupBox groupSecondary;
        System.Windows.Forms.Label labelSecondaryMin;
        System.Windows.Forms.NumericUpDown numSecondaryMin;
        System.Windows.Forms.CheckBox checkSecBackscatter;
        System.Windows.Forms.CheckBox checkSecComptonEdge;
        System.Windows.Forms.CheckBox checkSecSingleEscape;
        System.Windows.Forms.CheckBox checkSecDoubleEscape;
        System.Windows.Forms.CheckBox checkSecIodine;
        System.Windows.Forms.CheckBox checkSecAnnihilation;
        System.Windows.Forms.CheckBox checkSecSum;
        System.Windows.Forms.CheckBox checkSecPileUp;
        System.Windows.Forms.Button buttonGenerateSecondary;
        System.Windows.Forms.GroupBox groupNear;
        System.Windows.Forms.Label labelNearEnergy;
        System.Windows.Forms.NumericUpDown numNearEnergy;
        System.Windows.Forms.Label labelNearWindow;
        System.Windows.Forms.NumericUpDown numNearWindow;
        System.Windows.Forms.Label labelNearIntensity;
        System.Windows.Forms.NumericUpDown numNearIntensity;
        System.Windows.Forms.Label labelNearHalfLife;
        System.Windows.Forms.NumericUpDown numNearHalfLife;
        System.Windows.Forms.ComboBox comboNearHalfLifeUnit;
        System.Windows.Forms.Button buttonNearSearch;
        System.Windows.Forms.Button buttonNearAdd;
        System.Windows.Forms.ListBox listNear;
        System.Windows.Forms.Button buttonSelectAll;
        System.Windows.Forms.Button buttonSelectNone;
        System.Windows.Forms.NumericUpDown numTopN;
        System.Windows.Forms.Button buttonSelectTop;

        XPTable.Models.Table tableLines;
        XPTable.Models.ColumnModel columnModelLines;
        XPTable.Models.CheckBoxColumn columnLineSelected;
        XPTable.Models.TextColumn columnLineName;
        XPTable.Models.TextColumn columnLineEnergy;
        XPTable.Models.TextColumn columnLineIntensity;
        XPTable.Models.TextColumn columnLineRelative;
        XPTable.Models.TextColumn columnLineHalfLife;
        XPTable.Models.TextColumn columnLineType;
        System.Windows.Forms.CheckBox checkHalfLife;
        System.Windows.Forms.NumericUpDown numMinHalfLife;
        System.Windows.Forms.ComboBox comboMinHalfLifeUnit;
        System.Windows.Forms.NumericUpDown numMaxHalfLife;
        System.Windows.Forms.ComboBox comboMaxHalfLifeUnit;
        System.Windows.Forms.CheckBox checkHideUnselected;
        System.Windows.Forms.Label labelTypes;
        System.Windows.Forms.CheckBox checkTypeGamma;
        System.Windows.Forms.CheckBox checkTypeXray;
        System.Windows.Forms.CheckBox checkTypeXrf;
        System.Windows.Forms.CheckBox checkTypeSecondary;
        XPTable.Models.TableModel tableModelLines;

        System.Windows.Forms.GroupBox groupStyle;
        System.Windows.Forms.Label labelStyle;
        System.Windows.Forms.ComboBox comboStyle;
        System.Windows.Forms.Label labelWidth;
        System.Windows.Forms.ComboBox comboWidthMode;
        System.Windows.Forms.NumericUpDown numZonePercent;
        System.Windows.Forms.NumericUpDown numZoneFactor;
        System.Windows.Forms.Label labelColors;
        System.Windows.Forms.Button buttonColorByChain;
        System.Windows.Forms.Button buttonColorByNuclide;
        System.Windows.Forms.FlowLayoutPanel panelColors;

        System.Windows.Forms.GroupBox groupExport;
        System.Windows.Forms.Label labelConfigName;
        System.Windows.Forms.TextBox textConfigName;
        System.Windows.Forms.Button buttonCreateRoi;
        System.Windows.Forms.Label labelSetName;
        System.Windows.Forms.TextBox textSetName;
        System.Windows.Forms.Label labelAnchor;
        System.Windows.Forms.ComboBox comboAnchor;
        System.Windows.Forms.Button buttonCreateSet;
        System.Windows.Forms.CheckBox checkFullSet;
        System.Windows.Forms.Label labelAnchorCount;
        System.Windows.Forms.NumericUpDown numAnchors;
        System.Windows.Forms.Label labelIssues;
        System.Windows.Forms.ListBox listIssues;

        System.Windows.Forms.StatusStrip statusStrip;
        System.Windows.Forms.ToolStripStatusLabel statusLabel;
    }
}
