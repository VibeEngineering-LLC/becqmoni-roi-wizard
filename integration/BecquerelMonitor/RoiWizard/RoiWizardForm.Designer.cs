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
            this.columnCatalogHalfLife = new XPTable.Models.TextColumn();
            this.columnCatalogLines = new XPTable.Models.TextColumn();
            this.tableModelCatalog = new XPTable.Models.TableModel();

            this.groupGroup = new System.Windows.Forms.GroupBox();
            this.comboGroup = new System.Windows.Forms.ComboBox();
            this.buttonGroupAll = new System.Windows.Forms.Button();
            this.buttonGroupFamily = new System.Windows.Forms.Button();
            this.buttonGroupChain = new System.Windows.Forms.Button();
            this.checkedXrf = new System.Windows.Forms.CheckedListBox();
            this.labelXrf = new System.Windows.Forms.Label();

            this.groupSelected = new System.Windows.Forms.GroupBox();
            this.listSelected = new System.Windows.Forms.ListBox();
            this.buttonRemove = new System.Windows.Forms.Button();
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
            this.checkEquilibrium = new System.Windows.Forms.CheckBox();
            this.checkSecondary = new System.Windows.Forms.CheckBox();
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

            this.groupExport = new System.Windows.Forms.GroupBox();
            this.labelConfigName = new System.Windows.Forms.Label();
            this.textConfigName = new System.Windows.Forms.TextBox();
            this.buttonCreateRoi = new System.Windows.Forms.Button();
            this.labelSetName = new System.Windows.Forms.Label();
            this.textSetName = new System.Windows.Forms.TextBox();
            this.labelAnchor = new System.Windows.Forms.Label();
            this.comboAnchor = new System.Windows.Forms.ComboBox();
            this.buttonCreateSet = new System.Windows.Forms.Button();
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
            ((System.ComponentModel.ISupportInitialize)(this.numZonePercent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numZoneFactor)).BeginInit();
            this.SuspendLayout();

            // ─── вкладки ───────────────────────────────────────────────────
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Controls.Add(this.tabSources);
            this.tabs.Controls.Add(this.tabLines);
            this.tabs.Controls.Add(this.tabExport);

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
            this.groupSearch.Size = new System.Drawing.Size(330, 400);
            this.groupSearch.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;

            this.textSearch.Location = new System.Drawing.Point(8, 20);
            this.textSearch.Size = new System.Drawing.Size(140, 21);
            this.buttonAddSingle.Text = "Add";
            this.buttonAddSingle.Location = new System.Drawing.Point(154, 19);
            this.buttonAddSingle.Size = new System.Drawing.Size(54, 23);
            this.buttonAddFamily.Text = "+ family";
            this.buttonAddFamily.Location = new System.Drawing.Point(212, 19);
            this.buttonAddFamily.Size = new System.Drawing.Size(56, 23);
            this.buttonAddChain.Text = "+ chain";
            this.buttonAddChain.Location = new System.Drawing.Point(272, 19);
            this.buttonAddChain.Size = new System.Drawing.Size(50, 23);

            this.tableCatalog.Location = new System.Drawing.Point(8, 48);
            this.tableCatalog.Size = new System.Drawing.Size(314, 344);
            this.tableCatalog.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;
            this.tableCatalog.BorderColor = System.Drawing.Color.Black;
            this.tableCatalog.ColumnModel = this.columnModelCatalog;
            this.tableCatalog.FullRowSelect = true;
            this.tableCatalog.GridLines = XPTable.Models.GridLines.Rows;
            this.tableCatalog.TableModel = this.tableModelCatalog;
            this.columnCatalogName.Editable = false;   // таблицы только для чтения: правки идут через контролы
            this.columnCatalogName.Text = "Nuclide";
            this.columnCatalogName.Width = 120;
            this.columnCatalogHalfLife.Editable = false;
            this.columnCatalogHalfLife.Text = "T½";
            this.columnCatalogHalfLife.Width = 90;
            this.columnCatalogLines.Editable = false;
            this.columnCatalogLines.Text = "Lines";
            this.columnCatalogLines.Width = 70;
            this.columnModelCatalog.Columns.AddRange(new XPTable.Models.Column[] {
                this.columnCatalogName, this.columnCatalogHalfLife, this.columnCatalogLines });

            this.groupSearch.Controls.Add(this.textSearch);
            this.groupSearch.Controls.Add(this.buttonAddSingle);
            this.groupSearch.Controls.Add(this.buttonAddFamily);
            this.groupSearch.Controls.Add(this.buttonAddChain);
            this.groupSearch.Controls.Add(this.tableCatalog);

            this.groupGroup.Text = "Group";
            this.groupGroup.Location = new System.Drawing.Point(346, 6);
            this.groupGroup.Size = new System.Drawing.Size(330, 400);
            this.groupGroup.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.comboGroup.Location = new System.Drawing.Point(8, 20);
            this.comboGroup.Size = new System.Drawing.Size(314, 21);
            this.comboGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.buttonGroupAll.Text = "add all";
            this.buttonGroupAll.Location = new System.Drawing.Point(8, 46);
            this.buttonGroupAll.Size = new System.Drawing.Size(92, 23);
            this.buttonGroupFamily.Text = "+ family lines";
            this.buttonGroupFamily.Location = new System.Drawing.Point(104, 46);
            this.buttonGroupFamily.Size = new System.Drawing.Size(104, 23);
            this.buttonGroupChain.Text = "+ chain";
            this.buttonGroupChain.Location = new System.Drawing.Point(212, 46);
            this.buttonGroupChain.Size = new System.Drawing.Size(110, 23);
            this.labelXrf.Text = "XRF of shielding and detector materials:";
            this.labelXrf.Location = new System.Drawing.Point(8, 76);
            this.labelXrf.AutoSize = true;
            this.checkedXrf.Location = new System.Drawing.Point(8, 94);
            this.checkedXrf.Size = new System.Drawing.Size(314, 298);
            this.checkedXrf.CheckOnClick = true;
            this.checkedXrf.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;
            this.groupGroup.Controls.Add(this.comboGroup);
            this.groupGroup.Controls.Add(this.buttonGroupAll);
            this.groupGroup.Controls.Add(this.buttonGroupFamily);
            this.groupGroup.Controls.Add(this.buttonGroupChain);
            this.groupGroup.Controls.Add(this.labelXrf);
            this.groupGroup.Controls.Add(this.checkedXrf);

            this.groupSelected.Text = "Selected";
            this.groupSelected.Location = new System.Drawing.Point(684, 6);
            this.groupSelected.Size = new System.Drawing.Size(290, 400);
            this.groupSelected.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;
            this.listSelected.Location = new System.Drawing.Point(8, 20);
            this.listSelected.Size = new System.Drawing.Size(274, 342);
            this.listSelected.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;
            this.buttonRemove.Text = "remove";
            this.buttonRemove.Location = new System.Drawing.Point(8, 368);
            this.buttonRemove.Size = new System.Drawing.Size(90, 23);
            this.buttonRemove.Anchor = System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Left;
            this.buttonClear.Text = "clear all";
            this.buttonClear.Location = new System.Drawing.Point(104, 368);
            this.buttonClear.Size = new System.Drawing.Size(90, 23);
            this.buttonClear.Anchor = System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Left;
            this.groupSelected.Controls.Add(this.listSelected);
            this.groupSelected.Controls.Add(this.buttonRemove);
            this.groupSelected.Controls.Add(this.buttonClear);

            this.tabSources.Controls.Add(this.groupSearch);
            this.tabSources.Controls.Add(this.groupGroup);
            this.tabSources.Controls.Add(this.groupSelected);

            // ─── шаг 2 ─────────────────────────────────────────────────────
            this.groupResolution.Text = "Detector-resolution adaptation";
            this.groupResolution.Location = new System.Drawing.Point(8, 6);
            this.groupResolution.Size = new System.Drawing.Size(966, 52);
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
            this.comboCriterion.Size = new System.Drawing.Size(240, 21);
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
            this.buttonMerge.Location = new System.Drawing.Point(700, 19);
            this.buttonMerge.Size = new System.Drawing.Size(120, 23);
            this.buttonUnmerge.Text = "Restore originals";
            this.buttonUnmerge.Location = new System.Drawing.Point(826, 19);
            this.buttonUnmerge.Size = new System.Drawing.Size(120, 23);
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
            this.groupFilters.Size = new System.Drawing.Size(966, 76);
            this.groupFilters.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.checkIntensity.Text = "intensity ≥, %";
            this.checkIntensity.Location = new System.Drawing.Point(8, 21);
            this.checkIntensity.Size = new System.Drawing.Size(100, 20);
            this.numMinIntensity.Location = new System.Drawing.Point(112, 20);
            this.numMinIntensity.Size = new System.Drawing.Size(52, 21);
            this.numMinIntensity.DecimalPlaces = 1;
            this.numMinIntensity.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numMinIntensity.Value = new decimal(new int[] { 3, 0, 0, 0 });
            this.comboIntensityMode.Location = new System.Drawing.Point(170, 20);
            this.comboIntensityMode.Size = new System.Drawing.Size(240, 21);
            this.comboIntensityMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.checkEnergy.Text = "energy, keV";
            this.checkEnergy.Location = new System.Drawing.Point(422, 21);
            this.checkEnergy.Size = new System.Drawing.Size(92, 20);
            this.numMinEnergy.Location = new System.Drawing.Point(516, 20);
            this.numMinEnergy.Size = new System.Drawing.Size(60, 21);
            this.numMinEnergy.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            this.numMinEnergy.Value = new decimal(new int[] { 10, 0, 0, 0 });
            this.numMaxEnergy.Location = new System.Drawing.Point(582, 20);
            this.numMaxEnergy.Size = new System.Drawing.Size(60, 21);
            this.numMaxEnergy.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            this.numMaxEnergy.Value = new decimal(new int[] { 3000, 0, 0, 0 });
            this.checkEquilibrium.Text = "series equilibrium (intensities per one decay of the parent)";
            this.checkEquilibrium.Location = new System.Drawing.Point(650, 21);
            this.checkEquilibrium.Size = new System.Drawing.Size(310, 20);
            this.checkEquilibrium.Checked = true;
            this.buttonSelectAll.Text = "✓ select all";
            this.buttonSelectAll.Location = new System.Drawing.Point(8, 46);
            this.buttonSelectAll.Size = new System.Drawing.Size(96, 23);
            this.buttonSelectNone.Text = "✗ deselect all";
            this.buttonSelectNone.Location = new System.Drawing.Point(108, 46);
            this.buttonSelectNone.Size = new System.Drawing.Size(102, 23);
            this.numTopN.Location = new System.Drawing.Point(216, 47);
            this.numTopN.Size = new System.Drawing.Size(48, 21);
            this.numTopN.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numTopN.Value = new decimal(new int[] { 5, 0, 0, 0 });
            this.buttonSelectTop.Text = "top-N per nuclide";
            this.buttonSelectTop.Location = new System.Drawing.Point(270, 46);
            this.buttonSelectTop.Size = new System.Drawing.Size(126, 23);
            this.checkSecondary.Text = "add secondary peaks (backscatter, Compton edge, escapes)";
            this.checkSecondary.Location = new System.Drawing.Point(422, 47);
            this.checkSecondary.Size = new System.Drawing.Size(360, 20);
            this.groupFilters.Controls.Add(this.checkIntensity);
            this.groupFilters.Controls.Add(this.numMinIntensity);
            this.groupFilters.Controls.Add(this.comboIntensityMode);
            this.groupFilters.Controls.Add(this.checkEnergy);
            this.groupFilters.Controls.Add(this.numMinEnergy);
            this.groupFilters.Controls.Add(this.numMaxEnergy);
            this.groupFilters.Controls.Add(this.checkEquilibrium);
            this.groupFilters.Controls.Add(this.buttonSelectAll);
            this.groupFilters.Controls.Add(this.buttonSelectNone);
            this.groupFilters.Controls.Add(this.numTopN);
            this.groupFilters.Controls.Add(this.buttonSelectTop);
            this.groupFilters.Controls.Add(this.checkSecondary);

            this.tableLines.Location = new System.Drawing.Point(8, 164);
            this.tableLines.Size = new System.Drawing.Size(966, 242);
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
            this.columnLineType.Editable = false;
            this.columnLineType.Text = "Type";
            this.columnLineType.Width = 80;
            this.columnModelLines.Columns.AddRange(new XPTable.Models.Column[] {
                this.columnLineSelected, this.columnLineName, this.columnLineEnergy,
                this.columnLineIntensity, this.columnLineType });

            this.tabLines.Controls.Add(this.groupResolution);
            this.tabLines.Controls.Add(this.labelMergeInfo);
            this.tabLines.Controls.Add(this.groupFilters);
            this.tabLines.Controls.Add(this.tableLines);

            // ─── шаг 3 ─────────────────────────────────────────────────────
            this.groupStyle.Text = "ROI styling";
            this.groupStyle.Location = new System.Drawing.Point(8, 6);
            this.groupStyle.Size = new System.Drawing.Size(966, 54);
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
            this.groupStyle.Controls.Add(this.numZoneFactor);

            this.groupExport.Text = "Export";
            this.groupExport.Location = new System.Drawing.Point(8, 66);
            this.groupExport.Size = new System.Drawing.Size(966, 86);
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
            this.comboAnchor.Location = new System.Drawing.Point(446, 50);
            this.comboAnchor.Size = new System.Drawing.Size(300, 21);
            this.comboAnchor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.buttonCreateSet.Text = "Add set to the library";
            this.buttonCreateSet.Location = new System.Drawing.Point(754, 49);
            this.buttonCreateSet.Size = new System.Drawing.Size(192, 23);
            this.groupExport.Controls.Add(this.labelConfigName);
            this.groupExport.Controls.Add(this.textConfigName);
            this.groupExport.Controls.Add(this.buttonCreateRoi);
            this.groupExport.Controls.Add(this.labelSetName);
            this.groupExport.Controls.Add(this.textSetName);
            this.groupExport.Controls.Add(this.labelAnchor);
            this.groupExport.Controls.Add(this.comboAnchor);
            this.groupExport.Controls.Add(this.buttonCreateSet);

            this.labelIssues.Text = "Data check:";
            this.labelIssues.Location = new System.Drawing.Point(12, 158);
            this.labelIssues.AutoSize = true;
            this.listIssues.Location = new System.Drawing.Point(8, 176);
            this.listIssues.Size = new System.Drawing.Size(966, 230);
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
            this.ClientSize = new System.Drawing.Size(996, 470);
            this.MinimumSize = new System.Drawing.Size(880, 440);
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
            ((System.ComponentModel.ISupportInitialize)(this.numZonePercent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numZoneFactor)).EndInit();
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
        XPTable.Models.TextColumn columnCatalogHalfLife;
        XPTable.Models.TextColumn columnCatalogLines;
        XPTable.Models.TableModel tableModelCatalog;

        System.Windows.Forms.GroupBox groupGroup;
        System.Windows.Forms.ComboBox comboGroup;
        System.Windows.Forms.Button buttonGroupAll;
        System.Windows.Forms.Button buttonGroupFamily;
        System.Windows.Forms.Button buttonGroupChain;
        System.Windows.Forms.Label labelXrf;
        System.Windows.Forms.CheckedListBox checkedXrf;

        System.Windows.Forms.GroupBox groupSelected;
        System.Windows.Forms.ListBox listSelected;
        System.Windows.Forms.Button buttonRemove;
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
        System.Windows.Forms.CheckBox checkSecondary;
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
        XPTable.Models.TextColumn columnLineType;
        XPTable.Models.TableModel tableModelLines;

        System.Windows.Forms.GroupBox groupStyle;
        System.Windows.Forms.Label labelStyle;
        System.Windows.Forms.ComboBox comboStyle;
        System.Windows.Forms.Label labelWidth;
        System.Windows.Forms.ComboBox comboWidthMode;
        System.Windows.Forms.NumericUpDown numZonePercent;
        System.Windows.Forms.NumericUpDown numZoneFactor;

        System.Windows.Forms.GroupBox groupExport;
        System.Windows.Forms.Label labelConfigName;
        System.Windows.Forms.TextBox textConfigName;
        System.Windows.Forms.Button buttonCreateRoi;
        System.Windows.Forms.Label labelSetName;
        System.Windows.Forms.TextBox textSetName;
        System.Windows.Forms.Label labelAnchor;
        System.Windows.Forms.ComboBox comboAnchor;
        System.Windows.Forms.Button buttonCreateSet;
        System.Windows.Forms.Label labelIssues;
        System.Windows.Forms.ListBox listIssues;

        System.Windows.Forms.StatusStrip statusStrip;
        System.Windows.Forms.ToolStripStatusLabel statusLabel;
    }
}
