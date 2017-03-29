<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmChart
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            If moPengrid IsNot Nothing Then
                moPengrid.Dispose()
            End If
            If moPenBorder IsNot Nothing Then
                moPenBorder.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmChart))
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.btnSave = New System.Windows.Forms.ToolStripButton
        Me.btnExport = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator
        Me.btnPrint = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuPrint = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintPreview = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPageSetup = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintFile = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintOptions = New System.Windows.Forms.ToolStripMenuItem
        Me.tabMain = New System.Windows.Forms.TabControl
        Me.tabChart = New System.Windows.Forms.TabPage
        Me.spltChart = New System.Windows.Forms.SplitContainer
        Me.chkLine1 = New System.Windows.Forms.CheckBox
        Me.mnuCheckBoxes = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuSelectAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuUnSelectAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuSelectColor = New System.Windows.Forms.ToolStripMenuItem
        Me.chkLine0 = New System.Windows.Forms.CheckBox
        Me.tlpChartTab = New System.Windows.Forms.TableLayoutPanel
        Me.pnlChart = New System.Windows.Forms.Panel
        Me.trkScale = New System.Windows.Forms.TrackBar
        Me.lblScale = New System.Windows.Forms.Label
        Me.hscrChart = New System.Windows.Forms.HScrollBar
        Me.tabConfig = New System.Windows.Forms.TabPage
        Me.dgConfig = New System.Windows.Forms.DataGridView
        Me.tabTable = New System.Windows.Forms.TabPage
        Me.dgtable = New System.Windows.Forms.DataGridView
        Me.tabSchedule = New System.Windows.Forms.TabPage
        Me.dgSchedule = New System.Windows.Forms.DataGridView
        Me.tabItems = New System.Windows.Forms.TabPage
        Me.dgItems = New System.Windows.Forms.DataGridView
        Me.tlscMain = New System.Windows.Forms.ToolStripContainer
        Me.tlpChartMain = New System.Windows.Forms.TableLayoutPanel
        Me.pnlChartLabels = New System.Windows.Forms.Panel
        Me.lblFile = New System.Windows.Forms.Label
        Me.lblFileCap = New System.Windows.Forms.Label
        Me.lblVin = New System.Windows.Forms.Label
        Me.lblOption = New System.Windows.Forms.Label
        Me.lblColor = New System.Windows.Forms.Label
        Me.lblStyle = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.lblDevice = New System.Windows.Forms.Label
        Me.lblZone = New System.Windows.Forms.Label
        Me.lblDateCap = New System.Windows.Forms.Label
        Me.lblVinCap = New System.Windows.Forms.Label
        Me.lblOptionCap = New System.Windows.Forms.Label
        Me.lblColorCap = New System.Windows.Forms.Label
        Me.lblStyleCap = New System.Windows.Forms.Label
        Me.lblDeviceCap = New System.Windows.Forms.Label
        Me.lblZoneCap = New System.Windows.Forms.Label
        Me.mnuDataType = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuBit = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuInteger = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuFloat = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuMultiPlex = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuConfigColor = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuSelectConfigColor = New System.Windows.Forms.ToolStripMenuItem
        Me.tlsMain.SuspendLayout()
        Me.tabMain.SuspendLayout()
        Me.tabChart.SuspendLayout()
        Me.spltChart.Panel1.SuspendLayout()
        Me.spltChart.Panel2.SuspendLayout()
        Me.spltChart.SuspendLayout()
        Me.mnuCheckBoxes.SuspendLayout()
        Me.tlpChartTab.SuspendLayout()
        CType(Me.trkScale, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabConfig.SuspendLayout()
        CType(Me.dgConfig, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabTable.SuspendLayout()
        CType(Me.dgtable, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabSchedule.SuspendLayout()
        CType(Me.dgSchedule, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabItems.SuspendLayout()
        CType(Me.dgItems, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tlscMain.ContentPanel.SuspendLayout()
        Me.tlscMain.TopToolStripPanel.SuspendLayout()
        Me.tlscMain.SuspendLayout()
        Me.tlpChartMain.SuspendLayout()
        Me.pnlChartLabels.SuspendLayout()
        Me.mnuDataType.SuspendLayout()
        Me.mnuConfigColor.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlsMain
        '
        Me.tlsMain.Dock = System.Windows.Forms.DockStyle.None
        Me.tlsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.ToolStripSeparator1, Me.btnSave, Me.btnExport, Me.ToolStripSeparator4, Me.btnPrint})
        Me.tlsMain.Location = New System.Drawing.Point(0, 0)
        Me.tlsMain.MinimumSize = New System.Drawing.Size(0, 50)
        Me.tlsMain.Name = "tlsMain"
        Me.tlsMain.Size = New System.Drawing.Size(978, 63)
        Me.tlsMain.Stretch = True
        Me.tlsMain.TabIndex = 3
        Me.tlsMain.TabStop = True
        '
        'btnClose
        '
        Me.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.btnClose.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnClose.Image = CType(resources.GetObject("btnClose.Image"), System.Drawing.Image)
        Me.btnClose.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(45, 60)
        Me.btnClose.Text = "Close"
        Me.btnClose.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 63)
        '
        'btnSave
        '
        Me.btnSave.Image = CType(resources.GetObject("btnSave.Image"), System.Drawing.Image)
        Me.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(51, 60)
        Me.btnSave.Text = "btnSave"
        Me.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.btnSave.ToolTipText = "btnSave"
        '
        'btnExport
        '
        Me.btnExport.Image = CType(resources.GetObject("btnExport.Image"), System.Drawing.Image)
        Me.btnExport.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnExport.Name = "btnExport"
        Me.btnExport.Size = New System.Drawing.Size(59, 60)
        Me.btnExport.Text = "btnExport"
        Me.btnExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.btnExport.ToolTipText = "btnSave"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(6, 63)
        '
        'btnPrint
        '
        Me.btnPrint.DropDownButtonWidth = 13
        Me.btnPrint.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuPrint, Me.mnuPrintPreview, Me.mnuPageSetup, Me.mnuPrintFile, Me.mnuPrintOptions})
        Me.btnPrint.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnPrint.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnPrint.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(53, 60)
        Me.btnPrint.Text = "Print"
        Me.btnPrint.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'mnuPrint
        '
        Me.mnuPrint.Name = "mnuPrint"
        Me.mnuPrint.Size = New System.Drawing.Size(173, 22)
        Me.mnuPrint.Text = "Print"
        '
        'mnuPrintPreview
        '
        Me.mnuPrintPreview.Name = "mnuPrintPreview"
        Me.mnuPrintPreview.Size = New System.Drawing.Size(173, 22)
        Me.mnuPrintPreview.Text = "mnuPrintPreview"
        '
        'mnuPageSetup
        '
        Me.mnuPageSetup.Name = "mnuPageSetup"
        Me.mnuPageSetup.Size = New System.Drawing.Size(173, 22)
        Me.mnuPageSetup.Text = "mnuPageSetup"
        '
        'mnuPrintFile
        '
        Me.mnuPrintFile.Name = "mnuPrintFile"
        Me.mnuPrintFile.Size = New System.Drawing.Size(173, 22)
        Me.mnuPrintFile.Text = "mnuPrintFile"
        '
        'mnuPrintOptions
        '
        Me.mnuPrintOptions.Name = "mnuPrintOptions"
        Me.mnuPrintOptions.Size = New System.Drawing.Size(173, 22)
        Me.mnuPrintOptions.Text = "mnuPrintOptions"
        '
        'tabMain
        '
        Me.tabMain.Controls.Add(Me.tabChart)
        Me.tabMain.Controls.Add(Me.tabConfig)
        Me.tabMain.Controls.Add(Me.tabTable)
        Me.tabMain.Controls.Add(Me.tabSchedule)
        Me.tabMain.Controls.Add(Me.tabItems)
        Me.tabMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabMain.Location = New System.Drawing.Point(3, 128)
        Me.tabMain.Name = "tabMain"
        Me.tabMain.SelectedIndex = 0
        Me.tabMain.Size = New System.Drawing.Size(972, 382)
        Me.tabMain.TabIndex = 4
        '
        'tabChart
        '
        Me.tabChart.Controls.Add(Me.spltChart)
        Me.tabChart.Location = New System.Drawing.Point(4, 22)
        Me.tabChart.Name = "tabChart"
        Me.tabChart.Padding = New System.Windows.Forms.Padding(3)
        Me.tabChart.Size = New System.Drawing.Size(964, 356)
        Me.tabChart.TabIndex = 0
        Me.tabChart.Text = "tabChart"
        Me.tabChart.UseVisualStyleBackColor = True
        '
        'spltChart
        '
        Me.spltChart.Dock = System.Windows.Forms.DockStyle.Fill
        Me.spltChart.Location = New System.Drawing.Point(3, 3)
        Me.spltChart.Name = "spltChart"
        '
        'spltChart.Panel1
        '
        Me.spltChart.Panel1.AutoScroll = True
        Me.spltChart.Panel1.Controls.Add(Me.chkLine1)
        Me.spltChart.Panel1.Controls.Add(Me.chkLine0)
        '
        'spltChart.Panel2
        '
        Me.spltChart.Panel2.Controls.Add(Me.tlpChartTab)
        Me.spltChart.Size = New System.Drawing.Size(958, 350)
        Me.spltChart.SplitterDistance = 201
        Me.spltChart.TabIndex = 1
        '
        'chkLine1
        '
        Me.chkLine1.AutoSize = True
        Me.chkLine1.ContextMenuStrip = Me.mnuCheckBoxes
        Me.chkLine1.Location = New System.Drawing.Point(3, 23)
        Me.chkLine1.Name = "chkLine1"
        Me.chkLine1.Size = New System.Drawing.Size(70, 17)
        Me.chkLine1.TabIndex = 1
        Me.chkLine1.Text = "chkLine1"
        Me.chkLine1.UseVisualStyleBackColor = True
        '
        'mnuCheckBoxes
        '
        Me.mnuCheckBoxes.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuSelectAll, Me.mnuUnSelectAll, Me.mnuSelectColor})
        Me.mnuCheckBoxes.Name = "mnuCheckBoxes"
        Me.mnuCheckBoxes.Size = New System.Drawing.Size(149, 70)
        '
        'mnuSelectAll
        '
        Me.mnuSelectAll.Name = "mnuSelectAll"
        Me.mnuSelectAll.Size = New System.Drawing.Size(148, 22)
        Me.mnuSelectAll.Text = "mnuSelectAll"
        '
        'mnuUnSelectAll
        '
        Me.mnuUnSelectAll.Name = "mnuUnSelectAll"
        Me.mnuUnSelectAll.Size = New System.Drawing.Size(148, 22)
        Me.mnuUnSelectAll.Text = "mnuUnSelectAll"
        '
        'mnuSelectColor
        '
        Me.mnuSelectColor.Name = "mnuSelectColor"
        Me.mnuSelectColor.Size = New System.Drawing.Size(148, 22)
        Me.mnuSelectColor.Text = "mnuSelectColor"
        '
        'chkLine0
        '
        Me.chkLine0.AutoSize = True
        Me.chkLine0.ContextMenuStrip = Me.mnuCheckBoxes
        Me.chkLine0.Location = New System.Drawing.Point(3, 3)
        Me.chkLine0.Name = "chkLine0"
        Me.chkLine0.Size = New System.Drawing.Size(70, 17)
        Me.chkLine0.TabIndex = 0
        Me.chkLine0.Text = "chkLine0"
        Me.chkLine0.UseVisualStyleBackColor = True
        '
        'tlpChartTab
        '
        Me.tlpChartTab.ColumnCount = 2
        Me.tlpChartTab.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.07437!))
        Me.tlpChartTab.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.92563!))
        Me.tlpChartTab.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.tlpChartTab.Controls.Add(Me.pnlChart, 0, 0)
        Me.tlpChartTab.Controls.Add(Me.trkScale, 1, 2)
        Me.tlpChartTab.Controls.Add(Me.lblScale, 0, 2)
        Me.tlpChartTab.Controls.Add(Me.hscrChart, 0, 1)
        Me.tlpChartTab.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpChartTab.Location = New System.Drawing.Point(0, 0)
        Me.tlpChartTab.Name = "tlpChartTab"
        Me.tlpChartTab.RowCount = 3
        Me.tlpChartTab.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpChartTab.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16.0!))
        Me.tlpChartTab.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32.0!))
        Me.tlpChartTab.Size = New System.Drawing.Size(753, 350)
        Me.tlpChartTab.TabIndex = 0
        '
        'pnlChart
        '
        Me.tlpChartTab.SetColumnSpan(Me.pnlChart, 2)
        Me.pnlChart.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlChart.Location = New System.Drawing.Point(3, 3)
        Me.pnlChart.Name = "pnlChart"
        Me.pnlChart.Size = New System.Drawing.Size(747, 296)
        Me.pnlChart.TabIndex = 1
        '
        'trkScale
        '
        Me.trkScale.Dock = System.Windows.Forms.DockStyle.Fill
        Me.trkScale.Location = New System.Drawing.Point(365, 321)
        Me.trkScale.Name = "trkScale"
        Me.trkScale.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.trkScale.Size = New System.Drawing.Size(385, 26)
        Me.trkScale.TabIndex = 2
        '
        'lblScale
        '
        Me.lblScale.AutoSize = True
        Me.lblScale.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblScale.Location = New System.Drawing.Point(3, 318)
        Me.lblScale.Name = "lblScale"
        Me.lblScale.Size = New System.Drawing.Size(356, 32)
        Me.lblScale.TabIndex = 3
        Me.lblScale.Text = "lblScale"
        Me.lblScale.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'hscrChart
        '
        Me.tlpChartTab.SetColumnSpan(Me.hscrChart, 2)
        Me.hscrChart.Dock = System.Windows.Forms.DockStyle.Fill
        Me.hscrChart.LargeChange = 100
        Me.hscrChart.Location = New System.Drawing.Point(0, 302)
        Me.hscrChart.Name = "hscrChart"
        Me.hscrChart.Size = New System.Drawing.Size(753, 16)
        Me.hscrChart.TabIndex = 4
        '
        'tabConfig
        '
        Me.tabConfig.Controls.Add(Me.dgConfig)
        Me.tabConfig.Location = New System.Drawing.Point(4, 22)
        Me.tabConfig.Name = "tabConfig"
        Me.tabConfig.Size = New System.Drawing.Size(964, 356)
        Me.tabConfig.TabIndex = 4
        Me.tabConfig.Text = "tabConfig"
        Me.tabConfig.UseVisualStyleBackColor = True
        '
        'dgConfig
        '
        Me.dgConfig.AllowUserToAddRows = False
        Me.dgConfig.AllowUserToDeleteRows = False
        Me.dgConfig.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgConfig.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgConfig.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.dgConfig.Location = New System.Drawing.Point(0, 0)
        Me.dgConfig.Name = "dgConfig"
        Me.dgConfig.Size = New System.Drawing.Size(964, 356)
        Me.dgConfig.TabIndex = 2
        '
        'tabTable
        '
        Me.tabTable.Controls.Add(Me.dgtable)
        Me.tabTable.Location = New System.Drawing.Point(4, 22)
        Me.tabTable.Name = "tabTable"
        Me.tabTable.Padding = New System.Windows.Forms.Padding(3)
        Me.tabTable.Size = New System.Drawing.Size(964, 356)
        Me.tabTable.TabIndex = 1
        Me.tabTable.Text = "tabTable"
        Me.tabTable.UseVisualStyleBackColor = True
        '
        'dgtable
        '
        Me.dgtable.AllowUserToAddRows = False
        Me.dgtable.AllowUserToDeleteRows = False
        Me.dgtable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgtable.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgtable.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.dgtable.Location = New System.Drawing.Point(3, 3)
        Me.dgtable.Name = "dgtable"
        Me.dgtable.Size = New System.Drawing.Size(958, 350)
        Me.dgtable.TabIndex = 0
        '
        'tabSchedule
        '
        Me.tabSchedule.Controls.Add(Me.dgSchedule)
        Me.tabSchedule.Location = New System.Drawing.Point(4, 22)
        Me.tabSchedule.Name = "tabSchedule"
        Me.tabSchedule.Size = New System.Drawing.Size(964, 356)
        Me.tabSchedule.TabIndex = 2
        Me.tabSchedule.Text = "tabSchedule"
        Me.tabSchedule.UseVisualStyleBackColor = True
        '
        'dgSchedule
        '
        Me.dgSchedule.AllowUserToAddRows = False
        Me.dgSchedule.AllowUserToDeleteRows = False
        Me.dgSchedule.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgSchedule.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgSchedule.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.dgSchedule.Location = New System.Drawing.Point(0, 0)
        Me.dgSchedule.Name = "dgSchedule"
        Me.dgSchedule.Size = New System.Drawing.Size(964, 356)
        Me.dgSchedule.TabIndex = 1
        '
        'tabItems
        '
        Me.tabItems.Controls.Add(Me.dgItems)
        Me.tabItems.Location = New System.Drawing.Point(4, 22)
        Me.tabItems.Name = "tabItems"
        Me.tabItems.Size = New System.Drawing.Size(964, 356)
        Me.tabItems.TabIndex = 3
        Me.tabItems.Text = "tabItems"
        Me.tabItems.UseVisualStyleBackColor = True
        '
        'dgItems
        '
        Me.dgItems.AllowUserToAddRows = False
        Me.dgItems.AllowUserToDeleteRows = False
        Me.dgItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgItems.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgItems.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.dgItems.Location = New System.Drawing.Point(0, 0)
        Me.dgItems.Name = "dgItems"
        Me.dgItems.Size = New System.Drawing.Size(964, 356)
        Me.dgItems.TabIndex = 1
        '
        'tlscMain
        '
        '
        'tlscMain.ContentPanel
        '
        Me.tlscMain.ContentPanel.Controls.Add(Me.tlpChartMain)
        Me.tlscMain.ContentPanel.Size = New System.Drawing.Size(978, 513)
        Me.tlscMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlscMain.Location = New System.Drawing.Point(0, 0)
        Me.tlscMain.Name = "tlscMain"
        Me.tlscMain.Size = New System.Drawing.Size(978, 576)
        Me.tlscMain.TabIndex = 5
        Me.tlscMain.Text = "ToolStripContainer1"
        '
        'tlscMain.TopToolStripPanel
        '
        Me.tlscMain.TopToolStripPanel.Controls.Add(Me.tlsMain)
        '
        'tlpChartMain
        '
        Me.tlpChartMain.ColumnCount = 1
        Me.tlpChartMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpChartMain.Controls.Add(Me.tabMain, 0, 1)
        Me.tlpChartMain.Controls.Add(Me.pnlChartLabels, 0, 0)
        Me.tlpChartMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpChartMain.Location = New System.Drawing.Point(0, 0)
        Me.tlpChartMain.Name = "tlpChartMain"
        Me.tlpChartMain.RowCount = 2
        Me.tlpChartMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 125.0!))
        Me.tlpChartMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpChartMain.Size = New System.Drawing.Size(978, 513)
        Me.tlpChartMain.TabIndex = 5
        '
        'pnlChartLabels
        '
        Me.pnlChartLabels.Controls.Add(Me.lblFile)
        Me.pnlChartLabels.Controls.Add(Me.lblFileCap)
        Me.pnlChartLabels.Controls.Add(Me.lblVin)
        Me.pnlChartLabels.Controls.Add(Me.lblOption)
        Me.pnlChartLabels.Controls.Add(Me.lblColor)
        Me.pnlChartLabels.Controls.Add(Me.lblStyle)
        Me.pnlChartLabels.Controls.Add(Me.lblDate)
        Me.pnlChartLabels.Controls.Add(Me.lblDevice)
        Me.pnlChartLabels.Controls.Add(Me.lblZone)
        Me.pnlChartLabels.Controls.Add(Me.lblDateCap)
        Me.pnlChartLabels.Controls.Add(Me.lblVinCap)
        Me.pnlChartLabels.Controls.Add(Me.lblOptionCap)
        Me.pnlChartLabels.Controls.Add(Me.lblColorCap)
        Me.pnlChartLabels.Controls.Add(Me.lblStyleCap)
        Me.pnlChartLabels.Controls.Add(Me.lblDeviceCap)
        Me.pnlChartLabels.Controls.Add(Me.lblZoneCap)
        Me.pnlChartLabels.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlChartLabels.Location = New System.Drawing.Point(3, 3)
        Me.pnlChartLabels.Name = "pnlChartLabels"
        Me.pnlChartLabels.Size = New System.Drawing.Size(972, 119)
        Me.pnlChartLabels.TabIndex = 5
        '
        'lblFile
        '
        Me.lblFile.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold)
        Me.lblFile.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblFile.Location = New System.Drawing.Point(96, 79)
        Me.lblFile.Name = "lblFile"
        Me.lblFile.Size = New System.Drawing.Size(1111, 26)
        Me.lblFile.TabIndex = 31
        Me.lblFile.Text = "lblFile"
        Me.lblFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblFileCap
        '
        Me.lblFileCap.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold)
        Me.lblFileCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblFileCap.Location = New System.Drawing.Point(3, 79)
        Me.lblFileCap.Name = "lblFileCap"
        Me.lblFileCap.Size = New System.Drawing.Size(90, 26)
        Me.lblFileCap.TabIndex = 30
        Me.lblFileCap.Text = "lblFile"
        Me.lblFileCap.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblVin
        '
        Me.lblVin.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold)
        Me.lblVin.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblVin.Location = New System.Drawing.Point(505, 26)
        Me.lblVin.Name = "lblVin"
        Me.lblVin.Size = New System.Drawing.Size(119, 26)
        Me.lblVin.TabIndex = 29
        Me.lblVin.Text = "lblVin"
        Me.lblVin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblOption
        '
        Me.lblOption.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold)
        Me.lblOption.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblOption.Location = New System.Drawing.Point(505, 1)
        Me.lblOption.Name = "lblOption"
        Me.lblOption.Size = New System.Drawing.Size(119, 26)
        Me.lblOption.TabIndex = 28
        Me.lblOption.Text = "lblOption"
        Me.lblOption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblColor
        '
        Me.lblColor.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold)
        Me.lblColor.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblColor.Location = New System.Drawing.Point(297, 26)
        Me.lblColor.Name = "lblColor"
        Me.lblColor.Size = New System.Drawing.Size(123, 26)
        Me.lblColor.TabIndex = 27
        Me.lblColor.Text = "lblColor"
        Me.lblColor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblStyle
        '
        Me.lblStyle.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold)
        Me.lblStyle.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblStyle.Location = New System.Drawing.Point(297, 1)
        Me.lblStyle.Name = "lblStyle"
        Me.lblStyle.Size = New System.Drawing.Size(123, 26)
        Me.lblStyle.TabIndex = 26
        Me.lblStyle.Text = "lblStyle"
        Me.lblStyle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblDate
        '
        Me.lblDate.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold)
        Me.lblDate.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblDate.Location = New System.Drawing.Point(96, 52)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(465, 26)
        Me.lblDate.TabIndex = 25
        Me.lblDate.Text = "lblDate"
        Me.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblDevice
        '
        Me.lblDevice.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold)
        Me.lblDevice.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblDevice.Location = New System.Drawing.Point(96, 26)
        Me.lblDevice.Name = "lblDevice"
        Me.lblDevice.Size = New System.Drawing.Size(107, 26)
        Me.lblDevice.TabIndex = 24
        Me.lblDevice.Text = "lblDevice"
        Me.lblDevice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblZone
        '
        Me.lblZone.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold)
        Me.lblZone.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblZone.Location = New System.Drawing.Point(96, 1)
        Me.lblZone.Name = "lblZone"
        Me.lblZone.Size = New System.Drawing.Size(107, 26)
        Me.lblZone.TabIndex = 23
        Me.lblZone.Text = "lblZone"
        Me.lblZone.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblDateCap
        '
        Me.lblDateCap.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold)
        Me.lblDateCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblDateCap.Location = New System.Drawing.Point(3, 53)
        Me.lblDateCap.Name = "lblDateCap"
        Me.lblDateCap.Size = New System.Drawing.Size(90, 26)
        Me.lblDateCap.TabIndex = 22
        Me.lblDateCap.Text = "lblDate"
        Me.lblDateCap.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblVinCap
        '
        Me.lblVinCap.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold)
        Me.lblVinCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblVinCap.Location = New System.Drawing.Point(412, 26)
        Me.lblVinCap.Name = "lblVinCap"
        Me.lblVinCap.Size = New System.Drawing.Size(90, 26)
        Me.lblVinCap.TabIndex = 21
        Me.lblVinCap.Text = "lblVin"
        Me.lblVinCap.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblOptionCap
        '
        Me.lblOptionCap.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold)
        Me.lblOptionCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblOptionCap.Location = New System.Drawing.Point(412, 1)
        Me.lblOptionCap.Name = "lblOptionCap"
        Me.lblOptionCap.Size = New System.Drawing.Size(90, 26)
        Me.lblOptionCap.TabIndex = 20
        Me.lblOptionCap.Text = "lblOption"
        Me.lblOptionCap.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblColorCap
        '
        Me.lblColorCap.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold)
        Me.lblColorCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblColorCap.Location = New System.Drawing.Point(204, 26)
        Me.lblColorCap.Name = "lblColorCap"
        Me.lblColorCap.Size = New System.Drawing.Size(90, 26)
        Me.lblColorCap.TabIndex = 19
        Me.lblColorCap.Text = "lblColor"
        Me.lblColorCap.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblStyleCap
        '
        Me.lblStyleCap.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold)
        Me.lblStyleCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblStyleCap.Location = New System.Drawing.Point(204, 1)
        Me.lblStyleCap.Name = "lblStyleCap"
        Me.lblStyleCap.Size = New System.Drawing.Size(90, 26)
        Me.lblStyleCap.TabIndex = 18
        Me.lblStyleCap.Text = "lblStyle"
        Me.lblStyleCap.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblDeviceCap
        '
        Me.lblDeviceCap.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold)
        Me.lblDeviceCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblDeviceCap.Location = New System.Drawing.Point(3, 27)
        Me.lblDeviceCap.Name = "lblDeviceCap"
        Me.lblDeviceCap.Size = New System.Drawing.Size(90, 26)
        Me.lblDeviceCap.TabIndex = 17
        Me.lblDeviceCap.Text = "lblDevice"
        Me.lblDeviceCap.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblZoneCap
        '
        Me.lblZoneCap.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold)
        Me.lblZoneCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblZoneCap.Location = New System.Drawing.Point(3, 1)
        Me.lblZoneCap.Name = "lblZoneCap"
        Me.lblZoneCap.Size = New System.Drawing.Size(90, 26)
        Me.lblZoneCap.TabIndex = 16
        Me.lblZoneCap.Text = "lblZone"
        Me.lblZoneCap.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'mnuDataType
        '
        Me.mnuDataType.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuBit, Me.mnuInteger, Me.mnuFloat, Me.mnuMultiPlex})
        Me.mnuDataType.Name = "mnuDataType"
        Me.mnuDataType.Size = New System.Drawing.Size(137, 92)
        '
        'mnuBit
        '
        Me.mnuBit.Name = "mnuBit"
        Me.mnuBit.Size = New System.Drawing.Size(136, 22)
        Me.mnuBit.Text = "mnuBit"
        '
        'mnuInteger
        '
        Me.mnuInteger.Name = "mnuInteger"
        Me.mnuInteger.Size = New System.Drawing.Size(136, 22)
        Me.mnuInteger.Text = "mnuInteger"
        '
        'mnuFloat
        '
        Me.mnuFloat.Name = "mnuFloat"
        Me.mnuFloat.Size = New System.Drawing.Size(136, 22)
        Me.mnuFloat.Text = "mnuFloat"
        '
        'mnuMultiPlex
        '
        Me.mnuMultiPlex.Name = "mnuMultiPlex"
        Me.mnuMultiPlex.Size = New System.Drawing.Size(136, 22)
        Me.mnuMultiPlex.Text = "mnuMultiPlex"
        '
        'mnuConfigColor
        '
        Me.mnuConfigColor.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuSelectConfigColor})
        Me.mnuConfigColor.Name = "mnuCheckBoxes"
        Me.mnuConfigColor.Size = New System.Drawing.Size(149, 26)
        '
        'mnuSelectConfigColor
        '
        Me.mnuSelectConfigColor.Name = "mnuSelectConfigColor"
        Me.mnuSelectConfigColor.Size = New System.Drawing.Size(148, 22)
        Me.mnuSelectConfigColor.Text = "mnuSelectColor"
        '
        'frmChart
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(978, 576)
        Me.ControlBox = False
        Me.Controls.Add(Me.tlscMain)
        Me.Name = "frmChart"
        Me.Text = "frmChart"
        Me.tlsMain.ResumeLayout(False)
        Me.tlsMain.PerformLayout()
        Me.tabMain.ResumeLayout(False)
        Me.tabChart.ResumeLayout(False)
        Me.spltChart.Panel1.ResumeLayout(False)
        Me.spltChart.Panel1.PerformLayout()
        Me.spltChart.Panel2.ResumeLayout(False)
        Me.spltChart.ResumeLayout(False)
        Me.mnuCheckBoxes.ResumeLayout(False)
        Me.tlpChartTab.ResumeLayout(False)
        Me.tlpChartTab.PerformLayout()
        CType(Me.trkScale, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabConfig.ResumeLayout(False)
        CType(Me.dgConfig, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabTable.ResumeLayout(False)
        CType(Me.dgtable, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabSchedule.ResumeLayout(False)
        CType(Me.dgSchedule, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabItems.ResumeLayout(False)
        CType(Me.dgItems, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tlscMain.ContentPanel.ResumeLayout(False)
        Me.tlscMain.TopToolStripPanel.ResumeLayout(False)
        Me.tlscMain.TopToolStripPanel.PerformLayout()
        Me.tlscMain.ResumeLayout(False)
        Me.tlscMain.PerformLayout()
        Me.tlpChartMain.ResumeLayout(False)
        Me.pnlChartLabels.ResumeLayout(False)
        Me.mnuDataType.ResumeLayout(False)
        Me.mnuConfigColor.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnClose As System.Windows.Forms.ToolStripButton
    Public WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Public WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tabMain As System.Windows.Forms.TabControl
    Friend WithEvents tabChart As System.Windows.Forms.TabPage
    Friend WithEvents tabTable As System.Windows.Forms.TabPage
    Friend WithEvents dgtable As System.Windows.Forms.DataGridView
    Friend WithEvents tabSchedule As System.Windows.Forms.TabPage
    Friend WithEvents tabItems As System.Windows.Forms.TabPage
    Friend WithEvents dgSchedule As System.Windows.Forms.DataGridView
    Friend WithEvents dgItems As System.Windows.Forms.DataGridView
    Friend WithEvents btnPrint As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuPrint As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintPreview As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPageSetup As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintOptions As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnSave As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlscMain As System.Windows.Forms.ToolStripContainer
    Friend WithEvents tlpChartTab As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents tabConfig As System.Windows.Forms.TabPage
    Friend WithEvents btnExport As System.Windows.Forms.ToolStripButton
    Friend WithEvents dgConfig As System.Windows.Forms.DataGridView
    Friend WithEvents tlpChartMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents pnlChartLabels As System.Windows.Forms.Panel
    Friend WithEvents lblZone As System.Windows.Forms.Label
    Friend WithEvents lblDateCap As System.Windows.Forms.Label
    Friend WithEvents lblVinCap As System.Windows.Forms.Label
    Friend WithEvents lblOptionCap As System.Windows.Forms.Label
    Friend WithEvents lblColorCap As System.Windows.Forms.Label
    Friend WithEvents lblStyleCap As System.Windows.Forms.Label
    Friend WithEvents lblDeviceCap As System.Windows.Forms.Label
    Friend WithEvents lblZoneCap As System.Windows.Forms.Label
    Friend WithEvents lblVin As System.Windows.Forms.Label
    Friend WithEvents lblOption As System.Windows.Forms.Label
    Friend WithEvents lblColor As System.Windows.Forms.Label
    Friend WithEvents lblStyle As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblDevice As System.Windows.Forms.Label
    Friend WithEvents lblFile As System.Windows.Forms.Label
    Friend WithEvents lblFileCap As System.Windows.Forms.Label
    Friend WithEvents mnuDataType As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuBit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuInteger As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFloat As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents pnlChart As System.Windows.Forms.Panel
    Friend WithEvents trkScale As System.Windows.Forms.TrackBar
    Friend WithEvents lblScale As System.Windows.Forms.Label
    Friend WithEvents hscrChart As System.Windows.Forms.HScrollBar
    Friend WithEvents mnuMultiPlex As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents spltChart As System.Windows.Forms.SplitContainer
    Friend WithEvents chkLine1 As System.Windows.Forms.CheckBox
    Friend WithEvents chkLine0 As System.Windows.Forms.CheckBox
    Friend WithEvents mnuCheckBoxes As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuSelectAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuUnSelectAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSelectColor As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuConfigColor As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuSelectConfigColor As System.Windows.Forms.ToolStripMenuItem
End Class
