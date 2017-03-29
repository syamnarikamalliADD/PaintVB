<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Me.tscMain = New System.Windows.Forms.ToolStripContainer
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.tblpData = New System.Windows.Forms.TableLayoutPanel
        Me.pnlOtherData = New System.Windows.Forms.Panel
        Me.gpbPushVol = New System.Windows.Forms.GroupBox
        Me.nudPushVol = New System.Windows.Forms.NumericUpDown
        Me.gpbTotalTime = New System.Windows.Forms.GroupBox
        Me.lblTotalTime = New System.Windows.Forms.Label
        Me.gpbPreset = New System.Windows.Forms.GroupBox
        Me.lblParm06 = New System.Windows.Forms.Label
        Me.lblParm05 = New System.Windows.Forms.Label
        Me.lblParm04 = New System.Windows.Forms.Label
        Me.lblParm03 = New System.Windows.Forms.Label
        Me.lblParm02 = New System.Windows.Forms.Label
        Me.lblParm01 = New System.Windows.Forms.Label
        Me.nudPreset = New System.Windows.Forms.NumericUpDown
        Me.gpbStepTime = New System.Windows.Forms.GroupBox
        Me.nudStepTime = New System.Windows.Forms.NumericUpDown
        Me.gpbValves = New System.Windows.Forms.GroupBox
        Me.lstValves = New System.Windows.Forms.CheckedListBox
        Me.gpbStepActEvt = New System.Windows.Forms.GroupBox
        Me.lstStepActEvt = New System.Windows.Forms.ListBox
        Me.gpbGridView = New System.Windows.Forms.GroupBox
        Me.btnTall = New System.Windows.Forms.Button
        Me.Button2 = New System.Windows.Forms.Button
        Me.btnWide = New System.Windows.Forms.Button
        Me.dgGridView = New System.Windows.Forms.DataGridView
        Me.columnDefault = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.pnlScale = New System.Windows.Forms.Panel
        Me.picScale = New System.Windows.Forms.PictureBox
        Me.lblTooLong = New System.Windows.Forms.Label
        Me.rdoTime = New System.Windows.Forms.RadioButton
        Me.rdoSteps = New System.Windows.Forms.RadioButton
        Me.lblCycle = New System.Windows.Forms.Label
        Me.cboCycle = New System.Windows.Forms.ComboBox
        Me.cboColor = New System.Windows.Forms.ComboBox
        Me.lblColor = New System.Windows.Forms.Label
        Me.lblRobot = New System.Windows.Forms.Label
        Me.lblZone = New System.Windows.Forms.Label
        Me.cboRobot = New System.Windows.Forms.ComboBox
        Me.cboZone = New System.Windows.Forms.ComboBox
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.btnSave = New System.Windows.Forms.ToolStripButton
        Me.btnUndo = New System.Windows.Forms.ToolStripButton
        Me.btnCopy = New System.Windows.Forms.ToolStripButton
        Me.btnPrint = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuAllCycles = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrint = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintPreview = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPageSetup = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintFile = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintOptions = New System.Windows.Forms.ToolStripMenuItem
        Me.btnChangeLog = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuLast24 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLast7 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAllChanges = New System.Windows.Forms.ToolStripMenuItem
        Me.btnMultiView = New System.Windows.Forms.ToolStripButton
        Me.btnStatus = New System.Windows.Forms.ToolStripButton
        Me.btnRestore = New System.Windows.Forms.ToolStripButton
        Me.btnCCPresets = New System.Windows.Forms.ToolStripButton
        Me.btnUtilities = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuExport = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuImport = New System.Windows.Forms.ToolStripMenuItem
        Me.cboParam = New System.Windows.Forms.ComboBox
        Me.BottomToolStripPanel = New System.Windows.Forms.ToolStripPanel
        Me.TopToolStripPanel = New System.Windows.Forms.ToolStripPanel
        Me.RightToolStripPanel = New System.Windows.Forms.ToolStripPanel
        Me.LeftToolStripPanel = New System.Windows.Forms.ToolStripPanel
        Me.ContentPanel = New System.Windows.Forms.ToolStripContentPanel
        Me.stsStatus = New System.Windows.Forms.StatusStrip
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel
        Me.tspProgress = New System.Windows.Forms.ToolStripProgressBar
        Me.lblSpacer = New System.Windows.Forms.ToolStripStatusLabel
        Me.btnFunction = New System.Windows.Forms.ToolStripDropDownButton
        Me.mnuLogin = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLogOut = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuRemote = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLocal = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuStepActEvt = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuAction = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAction0 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAction1 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAction2 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAction3 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAction4 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAction5 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAction6 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAction7 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAction8 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEvent = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEvent0 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEvent1 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEvent2 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEvent3 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEvent4 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEvent5 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEvent6 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEvent7 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEvent8 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuInsert = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAppend = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuDelete = New System.Windows.Forms.ToolStripMenuItem
        Me.tscMain.ContentPanel.SuspendLayout()
        Me.tscMain.TopToolStripPanel.SuspendLayout()
        Me.tscMain.SuspendLayout()
        Me.tblpData.SuspendLayout()
        Me.pnlOtherData.SuspendLayout()
        Me.gpbPushVol.SuspendLayout()
        CType(Me.nudPushVol, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gpbTotalTime.SuspendLayout()
        Me.gpbPreset.SuspendLayout()
        CType(Me.nudPreset, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gpbStepTime.SuspendLayout()
        CType(Me.nudStepTime, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gpbValves.SuspendLayout()
        Me.gpbStepActEvt.SuspendLayout()
        Me.gpbGridView.SuspendLayout()
        CType(Me.dgGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlScale.SuspendLayout()
        CType(Me.picScale, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tlsMain.SuspendLayout()
        Me.stsStatus.SuspendLayout()
        Me.mnuStepActEvt.SuspendLayout()
        Me.SuspendLayout()
        '
        'tscMain
        '
        '
        'tscMain.ContentPanel
        '
        Me.tscMain.ContentPanel.Controls.Add(Me.lstStatus)
        Me.tscMain.ContentPanel.Controls.Add(Me.tblpData)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblCycle)
        Me.tscMain.ContentPanel.Controls.Add(Me.cboCycle)
        Me.tscMain.ContentPanel.Controls.Add(Me.cboColor)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblColor)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblRobot)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblZone)
        Me.tscMain.ContentPanel.Controls.Add(Me.cboRobot)
        Me.tscMain.ContentPanel.Controls.Add(Me.cboZone)
        resources.ApplyResources(Me.tscMain.ContentPanel, "tscMain.ContentPanel")
        resources.ApplyResources(Me.tscMain, "tscMain")
        Me.tscMain.LeftToolStripPanelVisible = False
        Me.tscMain.Name = "tscMain"
        Me.tscMain.RightToolStripPanelVisible = False
        '
        'tscMain.TopToolStripPanel
        '
        Me.tscMain.TopToolStripPanel.Controls.Add(Me.tlsMain)
        '
        'lstStatus
        '
        Me.lstStatus.BackColor = System.Drawing.SystemColors.Info
        resources.ApplyResources(Me.lstStatus, "lstStatus")
        Me.lstStatus.Name = "lstStatus"
        '
        'tblpData
        '
        resources.ApplyResources(Me.tblpData, "tblpData")
        Me.tblpData.Controls.Add(Me.pnlOtherData, 2, 0)
        Me.tblpData.Controls.Add(Me.gpbValves, 0, 0)
        Me.tblpData.Controls.Add(Me.gpbStepActEvt, 0, 0)
        Me.tblpData.Controls.Add(Me.gpbGridView, 0, 1)
        Me.tblpData.Name = "tblpData"
        '
        'pnlOtherData
        '
        resources.ApplyResources(Me.pnlOtherData, "pnlOtherData")
        Me.pnlOtherData.Controls.Add(Me.gpbPushVol)
        Me.pnlOtherData.Controls.Add(Me.gpbTotalTime)
        Me.pnlOtherData.Controls.Add(Me.gpbPreset)
        Me.pnlOtherData.Controls.Add(Me.gpbStepTime)
        Me.pnlOtherData.Name = "pnlOtherData"
        '
        'gpbPushVol
        '
        resources.ApplyResources(Me.gpbPushVol, "gpbPushVol")
        Me.gpbPushVol.Controls.Add(Me.nudPushVol)
        Me.gpbPushVol.Name = "gpbPushVol"
        Me.gpbPushVol.TabStop = False
        '
        'nudPushVol
        '
        Me.nudPushVol.DecimalPlaces = 1
        Me.nudPushVol.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        resources.ApplyResources(Me.nudPushVol, "nudPushVol")
        Me.nudPushVol.Maximum = New Decimal(New Integer() {999, 0, 0, 0})
        Me.nudPushVol.Name = "nudPushVol"
        '
        'gpbTotalTime
        '
        resources.ApplyResources(Me.gpbTotalTime, "gpbTotalTime")
        Me.gpbTotalTime.Controls.Add(Me.lblTotalTime)
        Me.gpbTotalTime.Name = "gpbTotalTime"
        Me.gpbTotalTime.TabStop = False
        '
        'lblTotalTime
        '
        resources.ApplyResources(Me.lblTotalTime, "lblTotalTime")
        Me.lblTotalTime.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblTotalTime.Name = "lblTotalTime"
        Me.lblTotalTime.UseMnemonic = False
        '
        'gpbPreset
        '
        resources.ApplyResources(Me.gpbPreset, "gpbPreset")
        Me.gpbPreset.Controls.Add(Me.lblParm06)
        Me.gpbPreset.Controls.Add(Me.lblParm05)
        Me.gpbPreset.Controls.Add(Me.lblParm04)
        Me.gpbPreset.Controls.Add(Me.lblParm03)
        Me.gpbPreset.Controls.Add(Me.lblParm02)
        Me.gpbPreset.Controls.Add(Me.lblParm01)
        Me.gpbPreset.Controls.Add(Me.nudPreset)
        Me.gpbPreset.Name = "gpbPreset"
        Me.gpbPreset.TabStop = False
        '
        'lblParm06
        '
        resources.ApplyResources(Me.lblParm06, "lblParm06")
        Me.lblParm06.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblParm06.Name = "lblParm06"
        Me.lblParm06.UseMnemonic = False
        '
        'lblParm05
        '
        resources.ApplyResources(Me.lblParm05, "lblParm05")
        Me.lblParm05.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblParm05.Name = "lblParm05"
        Me.lblParm05.UseMnemonic = False
        '
        'lblParm04
        '
        resources.ApplyResources(Me.lblParm04, "lblParm04")
        Me.lblParm04.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblParm04.Name = "lblParm04"
        Me.lblParm04.UseMnemonic = False
        '
        'lblParm03
        '
        resources.ApplyResources(Me.lblParm03, "lblParm03")
        Me.lblParm03.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblParm03.Name = "lblParm03"
        Me.lblParm03.UseMnemonic = False
        '
        'lblParm02
        '
        resources.ApplyResources(Me.lblParm02, "lblParm02")
        Me.lblParm02.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblParm02.Name = "lblParm02"
        Me.lblParm02.UseMnemonic = False
        '
        'lblParm01
        '
        resources.ApplyResources(Me.lblParm01, "lblParm01")
        Me.lblParm01.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblParm01.Name = "lblParm01"
        Me.lblParm01.UseMnemonic = False
        '
        'nudPreset
        '
        resources.ApplyResources(Me.nudPreset, "nudPreset")
        Me.nudPreset.Maximum = New Decimal(New Integer() {40, 0, 0, 0})
        Me.nudPreset.Name = "nudPreset"
        Me.nudPreset.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'gpbStepTime
        '
        resources.ApplyResources(Me.gpbStepTime, "gpbStepTime")
        Me.gpbStepTime.Controls.Add(Me.nudStepTime)
        Me.gpbStepTime.Name = "gpbStepTime"
        Me.gpbStepTime.TabStop = False
        '
        'nudStepTime
        '
        resources.ApplyResources(Me.nudStepTime, "nudStepTime")
        Me.nudStepTime.DecimalPlaces = 1
        Me.nudStepTime.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        Me.nudStepTime.Name = "nudStepTime"
        '
        'gpbValves
        '
        resources.ApplyResources(Me.gpbValves, "gpbValves")
        Me.gpbValves.Controls.Add(Me.lstValves)
        Me.gpbValves.Name = "gpbValves"
        Me.gpbValves.TabStop = False
        '
        'lstValves
        '
        resources.ApplyResources(Me.lstValves, "lstValves")
        Me.lstValves.FormattingEnabled = True
        Me.lstValves.Name = "lstValves"
        '
        'gpbStepActEvt
        '
        resources.ApplyResources(Me.gpbStepActEvt, "gpbStepActEvt")
        Me.gpbStepActEvt.Controls.Add(Me.lstStepActEvt)
        Me.gpbStepActEvt.Name = "gpbStepActEvt"
        Me.gpbStepActEvt.TabStop = False
        '
        'lstStepActEvt
        '
        resources.ApplyResources(Me.lstStepActEvt, "lstStepActEvt")
        Me.lstStepActEvt.ForeColor = System.Drawing.Color.Black
        Me.lstStepActEvt.FormattingEnabled = True
        Me.lstStepActEvt.Items.AddRange(New Object() {resources.GetString("lstStepActEvt.Items")})
        Me.lstStepActEvt.Name = "lstStepActEvt"
        '
        'gpbGridView
        '
        resources.ApplyResources(Me.gpbGridView, "gpbGridView")
        Me.tblpData.SetColumnSpan(Me.gpbGridView, 3)
        Me.gpbGridView.Controls.Add(Me.btnTall)
        Me.gpbGridView.Controls.Add(Me.Button2)
        Me.gpbGridView.Controls.Add(Me.btnWide)
        Me.gpbGridView.Controls.Add(Me.dgGridView)
        Me.gpbGridView.Controls.Add(Me.pnlScale)
        Me.gpbGridView.Controls.Add(Me.rdoTime)
        Me.gpbGridView.Controls.Add(Me.rdoSteps)
        Me.gpbGridView.Name = "gpbGridView"
        Me.gpbGridView.TabStop = False
        '
        'btnTall
        '
        resources.ApplyResources(Me.btnTall, "btnTall")
        Me.btnTall.Name = "btnTall"
        Me.btnTall.UseVisualStyleBackColor = True
        '
        'Button2
        '
        resources.ApplyResources(Me.Button2, "Button2")
        Me.Button2.Name = "Button2"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'btnWide
        '
        resources.ApplyResources(Me.btnWide, "btnWide")
        Me.btnWide.Name = "btnWide"
        Me.btnWide.UseVisualStyleBackColor = True
        '
        'dgGridView
        '
        Me.dgGridView.AllowUserToAddRows = False
        Me.dgGridView.AllowUserToDeleteRows = False
        Me.dgGridView.AllowUserToResizeRows = False
        resources.ApplyResources(Me.dgGridView, "dgGridView")
        Me.dgGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Arial", 10.0!, System.Drawing.FontStyle.Bold)
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.NullValue = Nothing
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.dgGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.columnDefault})
        Me.dgGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.dgGridView.MultiSelect = False
        Me.dgGridView.Name = "dgGridView"
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dgGridView.RowsDefaultCellStyle = DataGridViewCellStyle2
        Me.dgGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullColumnSelect
        '
        'columnDefault
        '
        resources.ApplyResources(Me.columnDefault, "columnDefault")
        Me.columnDefault.Name = "columnDefault"
        Me.columnDefault.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'pnlScale
        '
        resources.ApplyResources(Me.pnlScale, "pnlScale")
        Me.pnlScale.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.pnlScale.Controls.Add(Me.picScale)
        Me.pnlScale.Controls.Add(Me.lblTooLong)
        Me.pnlScale.Name = "pnlScale"
        '
        'picScale
        '
        resources.ApplyResources(Me.picScale, "picScale")
        Me.picScale.Image = Global.ColorChange.My.Resources.ProjectStrings.imgscale
        Me.picScale.Name = "picScale"
        Me.picScale.TabStop = False
        '
        'lblTooLong
        '
        resources.ApplyResources(Me.lblTooLong, "lblTooLong")
        Me.lblTooLong.Name = "lblTooLong"
        '
        'rdoTime
        '
        resources.ApplyResources(Me.rdoTime, "rdoTime")
        Me.rdoTime.Name = "rdoTime"
        Me.rdoTime.UseVisualStyleBackColor = True
        '
        'rdoSteps
        '
        resources.ApplyResources(Me.rdoSteps, "rdoSteps")
        Me.rdoSteps.Checked = True
        Me.rdoSteps.Name = "rdoSteps"
        Me.rdoSteps.TabStop = True
        Me.rdoSteps.UseVisualStyleBackColor = True
        '
        'lblCycle
        '
        Me.lblCycle.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblCycle, "lblCycle")
        Me.lblCycle.Name = "lblCycle"
        Me.lblCycle.UseMnemonic = False
        '
        'cboCycle
        '
        Me.cboCycle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboCycle, "cboCycle")
        Me.cboCycle.Name = "cboCycle"
        '
        'cboColor
        '
        Me.cboColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboColor, "cboColor")
        Me.cboColor.Name = "cboColor"
        '
        'lblColor
        '
        Me.lblColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblColor, "lblColor")
        Me.lblColor.Name = "lblColor"
        Me.lblColor.UseMnemonic = False
        '
        'lblRobot
        '
        Me.lblRobot.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblRobot, "lblRobot")
        Me.lblRobot.Name = "lblRobot"
        Me.lblRobot.UseMnemonic = False
        '
        'lblZone
        '
        Me.lblZone.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblZone, "lblZone")
        Me.lblZone.Name = "lblZone"
        Me.lblZone.UseMnemonic = False
        '
        'cboRobot
        '
        Me.cboRobot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboRobot, "cboRobot")
        Me.cboRobot.Name = "cboRobot"
        '
        'cboZone
        '
        Me.cboZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboZone, "cboZone")
        Me.cboZone.Name = "cboZone"
        '
        'tlsMain
        '
        resources.ApplyResources(Me.tlsMain, "tlsMain")
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.btnSave, Me.btnUndo, Me.btnCopy, Me.btnPrint, Me.btnChangeLog, Me.btnMultiView, Me.btnStatus, Me.btnRestore, Me.btnCCPresets, Me.btnUtilities})
        Me.tlsMain.MinimumSize = New System.Drawing.Size(0, 50)
        Me.tlsMain.Name = "tlsMain"
        Me.tlsMain.Stretch = True
        Me.tlsMain.TabStop = True
        '
        'btnClose
        '
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.Name = "btnClose"
        '
        'btnSave
        '
        resources.ApplyResources(Me.btnSave, "btnSave")
        Me.btnSave.Name = "btnSave"
        '
        'btnUndo
        '
        resources.ApplyResources(Me.btnUndo, "btnUndo")
        Me.btnUndo.Name = "btnUndo"
        '
        'btnCopy
        '
        resources.ApplyResources(Me.btnCopy, "btnCopy")
        Me.btnCopy.Name = "btnCopy"
        '
        'btnPrint
        '
        Me.btnPrint.DropDownButtonWidth = 13
        Me.btnPrint.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuAllCycles, Me.mnuPrint, Me.mnuPrintPreview, Me.mnuPageSetup, Me.mnuPrintFile, Me.mnuPrintOptions})
        resources.ApplyResources(Me.btnPrint, "btnPrint")
        Me.btnPrint.Name = "btnPrint"
        '
        'mnuAllCycles
        '
        Me.mnuAllCycles.Checked = True
        Me.mnuAllCycles.CheckOnClick = True
        Me.mnuAllCycles.CheckState = System.Windows.Forms.CheckState.Checked
        resources.ApplyResources(Me.mnuAllCycles, "mnuAllCycles")
        Me.mnuAllCycles.Name = "mnuAllCycles"
        '
        'mnuPrint
        '
        Me.mnuPrint.Name = "mnuPrint"
        resources.ApplyResources(Me.mnuPrint, "mnuPrint")
        '
        'mnuPrintPreview
        '
        Me.mnuPrintPreview.Name = "mnuPrintPreview"
        resources.ApplyResources(Me.mnuPrintPreview, "mnuPrintPreview")
        '
        'mnuPageSetup
        '
        Me.mnuPageSetup.Name = "mnuPageSetup"
        resources.ApplyResources(Me.mnuPageSetup, "mnuPageSetup")
        '
        'mnuPrintFile
        '
        Me.mnuPrintFile.Name = "mnuPrintFile"
        resources.ApplyResources(Me.mnuPrintFile, "mnuPrintFile")
        '
        'mnuPrintOptions
        '
        Me.mnuPrintOptions.Name = "mnuPrintOptions"
        resources.ApplyResources(Me.mnuPrintOptions, "mnuPrintOptions")
        '
        'btnChangeLog
        '
        Me.btnChangeLog.DropDownButtonWidth = 13
        Me.btnChangeLog.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuLast24, Me.mnuLast7, Me.mnuAllChanges})
        resources.ApplyResources(Me.btnChangeLog, "btnChangeLog")
        Me.btnChangeLog.Name = "btnChangeLog"
        '
        'mnuLast24
        '
        Me.mnuLast24.Name = "mnuLast24"
        resources.ApplyResources(Me.mnuLast24, "mnuLast24")
        '
        'mnuLast7
        '
        Me.mnuLast7.Name = "mnuLast7"
        resources.ApplyResources(Me.mnuLast7, "mnuLast7")
        '
        'mnuAllChanges
        '
        Me.mnuAllChanges.Name = "mnuAllChanges"
        resources.ApplyResources(Me.mnuAllChanges, "mnuAllChanges")
        '
        'btnMultiView
        '
        resources.ApplyResources(Me.btnMultiView, "btnMultiView")
        Me.btnMultiView.Name = "btnMultiView"
        '
        'btnStatus
        '
        resources.ApplyResources(Me.btnStatus, "btnStatus")
        Me.btnStatus.CheckOnClick = True
        Me.btnStatus.Name = "btnStatus"
        '
        'btnRestore
        '
        resources.ApplyResources(Me.btnRestore, "btnRestore")
        Me.btnRestore.Name = "btnRestore"
        '
        'btnCCPresets
        '
        Me.btnCCPresets.Image = Global.ColorChange.My.Resources.ProjectStrings.Fluid_Presets
        resources.ApplyResources(Me.btnCCPresets, "btnCCPresets")
        Me.btnCCPresets.Name = "btnCCPresets"
        '
        'btnUtilities
        '
        Me.btnUtilities.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuExport, Me.mnuImport})
        resources.ApplyResources(Me.btnUtilities, "btnUtilities")
        Me.btnUtilities.Name = "btnUtilities"
        '
        'mnuExport
        '
        Me.mnuExport.Name = "mnuExport"
        resources.ApplyResources(Me.mnuExport, "mnuExport")
        '
        'mnuImport
        '
        Me.mnuImport.Name = "mnuImport"
        resources.ApplyResources(Me.mnuImport, "mnuImport")
        '
        'cboParam
        '
        Me.cboParam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboParam, "cboParam")
        Me.cboParam.Name = "cboParam"
        '
        'BottomToolStripPanel
        '
        resources.ApplyResources(Me.BottomToolStripPanel, "BottomToolStripPanel")
        Me.BottomToolStripPanel.Name = "BottomToolStripPanel"
        Me.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.BottomToolStripPanel.RowMargin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        '
        'TopToolStripPanel
        '
        resources.ApplyResources(Me.TopToolStripPanel, "TopToolStripPanel")
        Me.TopToolStripPanel.Name = "TopToolStripPanel"
        Me.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.TopToolStripPanel.RowMargin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        '
        'RightToolStripPanel
        '
        resources.ApplyResources(Me.RightToolStripPanel, "RightToolStripPanel")
        Me.RightToolStripPanel.Name = "RightToolStripPanel"
        Me.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.RightToolStripPanel.RowMargin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        '
        'LeftToolStripPanel
        '
        resources.ApplyResources(Me.LeftToolStripPanel, "LeftToolStripPanel")
        Me.LeftToolStripPanel.Name = "LeftToolStripPanel"
        Me.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.LeftToolStripPanel.RowMargin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        '
        'ContentPanel
        '
        resources.ApplyResources(Me.ContentPanel, "ContentPanel")
        '
        'stsStatus
        '
        resources.ApplyResources(Me.stsStatus, "stsStatus")
        Me.stsStatus.GripMargin = New System.Windows.Forms.Padding(0)
        Me.stsStatus.ImageScalingSize = New System.Drawing.Size(22, 22)
        Me.stsStatus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatus, Me.tspProgress, Me.lblSpacer, Me.btnFunction})
        Me.stsStatus.Name = "stsStatus"
        Me.stsStatus.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode
        Me.stsStatus.ShowItemToolTips = True
        '
        'lblStatus
        '
        Me.lblStatus.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedOuter
        resources.ApplyResources(Me.lblStatus, "lblStatus")
        Me.lblStatus.Margin = New System.Windows.Forms.Padding(0)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Spring = True
        '
        'tspProgress
        '
        Me.tspProgress.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.tspProgress.Margin = New System.Windows.Forms.Padding(2, 3, 1, 3)
        Me.tspProgress.Name = "tspProgress"
        Me.tspProgress.Padding = New System.Windows.Forms.Padding(2)
        resources.ApplyResources(Me.tspProgress, "tspProgress")
        Me.tspProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        '
        'lblSpacer
        '
        resources.ApplyResources(Me.lblSpacer, "lblSpacer")
        Me.lblSpacer.Name = "lblSpacer"
        '
        'btnFunction
        '
        Me.btnFunction.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.btnFunction.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnFunction.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuLogin, Me.mnuLogOut, Me.mnuRemote, Me.mnuLocal})
        resources.ApplyResources(Me.btnFunction, "btnFunction")
        Me.btnFunction.Name = "btnFunction"
        Me.btnFunction.Padding = New System.Windows.Forms.Padding(2, 0, 2, 0)
        '
        'mnuLogin
        '
        Me.mnuLogin.Name = "mnuLogin"
        resources.ApplyResources(Me.mnuLogin, "mnuLogin")
        '
        'mnuLogOut
        '
        Me.mnuLogOut.Name = "mnuLogOut"
        resources.ApplyResources(Me.mnuLogOut, "mnuLogOut")
        '
        'mnuRemote
        '
        Me.mnuRemote.Name = "mnuRemote"
        resources.ApplyResources(Me.mnuRemote, "mnuRemote")
        '
        'mnuLocal
        '
        Me.mnuLocal.Name = "mnuLocal"
        resources.ApplyResources(Me.mnuLocal, "mnuLocal")
        '
        'mnuStepActEvt
        '
        Me.mnuStepActEvt.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuAction, Me.mnuEvent, Me.mnuInsert, Me.mnuAppend, Me.mnuDelete})
        Me.mnuStepActEvt.Name = "mnuStepActEvt"
        resources.ApplyResources(Me.mnuStepActEvt, "mnuStepActEvt")
        '
        'mnuAction
        '
        Me.mnuAction.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuAction0, Me.mnuAction1, Me.mnuAction2, Me.mnuAction3, Me.mnuAction4, Me.mnuAction5, Me.mnuAction6, Me.mnuAction7, Me.mnuAction8})
        Me.mnuAction.Name = "mnuAction"
        resources.ApplyResources(Me.mnuAction, "mnuAction")
        '
        'mnuAction0
        '
        Me.mnuAction0.Name = "mnuAction0"
        resources.ApplyResources(Me.mnuAction0, "mnuAction0")
        '
        'mnuAction1
        '
        Me.mnuAction1.Name = "mnuAction1"
        resources.ApplyResources(Me.mnuAction1, "mnuAction1")
        '
        'mnuAction2
        '
        Me.mnuAction2.Name = "mnuAction2"
        resources.ApplyResources(Me.mnuAction2, "mnuAction2")
        '
        'mnuAction3
        '
        Me.mnuAction3.Name = "mnuAction3"
        resources.ApplyResources(Me.mnuAction3, "mnuAction3")
        '
        'mnuAction4
        '
        Me.mnuAction4.Name = "mnuAction4"
        resources.ApplyResources(Me.mnuAction4, "mnuAction4")
        '
        'mnuAction5
        '
        Me.mnuAction5.Name = "mnuAction5"
        resources.ApplyResources(Me.mnuAction5, "mnuAction5")
        '
        'mnuAction6
        '
        Me.mnuAction6.Name = "mnuAction6"
        resources.ApplyResources(Me.mnuAction6, "mnuAction6")
        '
        'mnuAction7
        '
        Me.mnuAction7.Name = "mnuAction7"
        resources.ApplyResources(Me.mnuAction7, "mnuAction7")
        '
        'mnuAction8
        '
        Me.mnuAction8.Name = "mnuAction8"
        resources.ApplyResources(Me.mnuAction8, "mnuAction8")
        '
        'mnuEvent
        '
        Me.mnuEvent.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuEvent0, Me.mnuEvent1, Me.mnuEvent2, Me.mnuEvent3, Me.mnuEvent4, Me.mnuEvent5, Me.mnuEvent6, Me.mnuEvent7, Me.mnuEvent8})
        Me.mnuEvent.Name = "mnuEvent"
        resources.ApplyResources(Me.mnuEvent, "mnuEvent")
        '
        'mnuEvent0
        '
        Me.mnuEvent0.Name = "mnuEvent0"
        resources.ApplyResources(Me.mnuEvent0, "mnuEvent0")
        '
        'mnuEvent1
        '
        Me.mnuEvent1.Name = "mnuEvent1"
        resources.ApplyResources(Me.mnuEvent1, "mnuEvent1")
        '
        'mnuEvent2
        '
        Me.mnuEvent2.Name = "mnuEvent2"
        resources.ApplyResources(Me.mnuEvent2, "mnuEvent2")
        '
        'mnuEvent3
        '
        Me.mnuEvent3.Name = "mnuEvent3"
        resources.ApplyResources(Me.mnuEvent3, "mnuEvent3")
        '
        'mnuEvent4
        '
        Me.mnuEvent4.Name = "mnuEvent4"
        resources.ApplyResources(Me.mnuEvent4, "mnuEvent4")
        '
        'mnuEvent5
        '
        Me.mnuEvent5.Name = "mnuEvent5"
        resources.ApplyResources(Me.mnuEvent5, "mnuEvent5")
        '
        'mnuEvent6
        '
        Me.mnuEvent6.Name = "mnuEvent6"
        resources.ApplyResources(Me.mnuEvent6, "mnuEvent6")
        '
        'mnuEvent7
        '
        Me.mnuEvent7.Name = "mnuEvent7"
        resources.ApplyResources(Me.mnuEvent7, "mnuEvent7")
        '
        'mnuEvent8
        '
        Me.mnuEvent8.Name = "mnuEvent8"
        resources.ApplyResources(Me.mnuEvent8, "mnuEvent8")
        '
        'mnuInsert
        '
        Me.mnuInsert.Name = "mnuInsert"
        resources.ApplyResources(Me.mnuInsert, "mnuInsert")
        '
        'mnuAppend
        '
        Me.mnuAppend.Name = "mnuAppend"
        resources.ApplyResources(Me.mnuAppend, "mnuAppend")
        '
        'mnuDelete
        '
        Me.mnuDelete.Name = "mnuDelete"
        resources.ApplyResources(Me.mnuDelete, "mnuDelete")
        '
        'frmMain
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.stsStatus)
        Me.Controls.Add(Me.tscMain)
        Me.Name = "frmMain"
        Me.tscMain.ContentPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.PerformLayout()
        Me.tscMain.ResumeLayout(False)
        Me.tscMain.PerformLayout()
        Me.tblpData.ResumeLayout(False)
        Me.pnlOtherData.ResumeLayout(False)
        Me.gpbPushVol.ResumeLayout(False)
        CType(Me.nudPushVol, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gpbTotalTime.ResumeLayout(False)
        Me.gpbPreset.ResumeLayout(False)
        CType(Me.nudPreset, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gpbStepTime.ResumeLayout(False)
        CType(Me.nudStepTime, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gpbValves.ResumeLayout(False)
        Me.gpbStepActEvt.ResumeLayout(False)
        Me.gpbGridView.ResumeLayout(False)
        Me.gpbGridView.PerformLayout()
        CType(Me.dgGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlScale.ResumeLayout(False)
        CType(Me.picScale, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tlsMain.ResumeLayout(False)
        Me.tlsMain.PerformLayout()
        Me.stsStatus.ResumeLayout(False)
        Me.stsStatus.PerformLayout()
        Me.mnuStepActEvt.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents cboParam As System.Windows.Forms.ComboBox
    Friend WithEvents BottomToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents TopToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents RightToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents LeftToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents ContentPanel As System.Windows.Forms.ToolStripContentPanel
    Friend WithEvents cboZone As System.Windows.Forms.ComboBox
    Friend WithEvents cboRobot As System.Windows.Forms.ComboBox
    Friend WithEvents lblZone As System.Windows.Forms.Label
    Friend WithEvents lblRobot As System.Windows.Forms.Label
    Friend WithEvents lblColor As System.Windows.Forms.Label
    Friend WithEvents cboColor As System.Windows.Forms.ComboBox
    Friend WithEvents tscMain As System.Windows.Forms.ToolStripContainer
    Friend WithEvents lblCycle As System.Windows.Forms.Label
    Friend WithEvents cboCycle As System.Windows.Forms.ComboBox
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnClose As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnSave As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnUndo As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnCopy As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnChangeLog As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuLast24 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLast7 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAllChanges As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnMultiView As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnRestore As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnStatus As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnCCPresets As System.Windows.Forms.ToolStripButton
    Friend WithEvents stsStatus As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents lblSpacer As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents btnFunction As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents mnuLogin As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLogOut As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuRemote As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLocal As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lstStatus As System.Windows.Forms.ListBox
    Friend WithEvents tblpData As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents gpbStepActEvt As System.Windows.Forms.GroupBox
    Friend WithEvents lstStepActEvt As System.Windows.Forms.ListBox
    Friend WithEvents gpbGridView As System.Windows.Forms.GroupBox
    Friend WithEvents dgGridView As System.Windows.Forms.DataGridView
    Friend WithEvents pnlScale As System.Windows.Forms.Panel
    Friend WithEvents picScale As System.Windows.Forms.PictureBox
    Friend WithEvents lblTooLong As System.Windows.Forms.Label
    Friend WithEvents rdoTime As System.Windows.Forms.RadioButton
    Friend WithEvents rdoSteps As System.Windows.Forms.RadioButton
    Friend WithEvents gpbValves As System.Windows.Forms.GroupBox
    Friend WithEvents pnlOtherData As System.Windows.Forms.Panel
    Friend WithEvents gpbPushVol As System.Windows.Forms.GroupBox
    Friend WithEvents nudPushVol As System.Windows.Forms.NumericUpDown
    Friend WithEvents gpbTotalTime As System.Windows.Forms.GroupBox
    Friend WithEvents lblTotalTime As System.Windows.Forms.Label
    Friend WithEvents gpbPreset As System.Windows.Forms.GroupBox
    Friend WithEvents nudPreset As System.Windows.Forms.NumericUpDown
    Friend WithEvents gpbStepTime As System.Windows.Forms.GroupBox
    Friend WithEvents nudStepTime As System.Windows.Forms.NumericUpDown
    Friend WithEvents mnuStepActEvt As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuAction As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEvent As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuInsert As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuDelete As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblParm03 As System.Windows.Forms.Label
    Friend WithEvents lblParm02 As System.Windows.Forms.Label
    Friend WithEvents lblParm01 As System.Windows.Forms.Label
    Friend WithEvents mnuAppend As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lstValves As System.Windows.Forms.CheckedListBox
    Friend WithEvents mnuAction1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAction0 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAction2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAction3 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAction4 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAction5 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAction6 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAction7 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAction8 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEvent0 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEvent1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEvent2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEvent3 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEvent4 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEvent5 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEvent6 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEvent7 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuEvent8 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents columnDefault As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents btnWide As System.Windows.Forms.Button
    Friend WithEvents btnTall As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents tspProgress As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents lblParm04 As System.Windows.Forms.Label
    Friend WithEvents lblParm06 As System.Windows.Forms.Label
    Friend WithEvents lblParm05 As System.Windows.Forms.Label
    Friend WithEvents btnPrint As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuAllCycles As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrint As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintPreview As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPageSetup As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintOptions As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnUtilities As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuExport As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuImport As System.Windows.Forms.ToolStripMenuItem
End Class
