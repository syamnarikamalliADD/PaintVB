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
        Me.tscMain = New System.Windows.Forms.ToolStripContainer
        Me.gpbPumpEnables = New System.Windows.Forms.GroupBox
        Me.chkPump2 = New System.Windows.Forms.CheckBox
        Me.chkPump1 = New System.Windows.Forms.CheckBox
        Me.lblHintAllRobots = New System.Windows.Forms.Label
        Me.gpbCurrentColor = New System.Windows.Forms.GroupBox
        Me.lblCurrentColor = New System.Windows.Forms.Label
        Me.gpbTPR = New System.Windows.Forms.GroupBox
        Me.txtTPR = New FocusedTextBox.FocusedTextBox
        Me.gpbDockControl = New System.Windows.Forms.GroupBox
        Me.btnMoveDeDock = New System.Windows.Forms.Button
        Me.btnMoveClean = New System.Windows.Forms.Button
        Me.btnMoveDock = New System.Windows.Forms.Button
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.gpbViewAll = New System.Windows.Forms.GroupBox
        Me.mnuViewSelect = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuViewSelectToon = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuViewSelectPaint = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuViewSelectAA_BS = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuViewSelectFA_SA = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuViewSelectEstat = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuViewSelectCalStat = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuViewSelectDockCalStat = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuViewSelectDQCalStat = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuViewSelectDQCalStat2 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuViewSelectPressureTest = New System.Windows.Forms.ToolStripMenuItem
        Me.lblStatC16 = New System.Windows.Forms.Label
        Me.lblStatC15 = New System.Windows.Forms.Label
        Me.lblStatB16 = New System.Windows.Forms.Label
        Me.lblStatB15 = New System.Windows.Forms.Label
        Me.lblStatA16 = New System.Windows.Forms.Label
        Me.lblStatA15 = New System.Windows.Forms.Label
        Me.lblCalStat16 = New System.Windows.Forms.Label
        Me.lblCalStat15 = New System.Windows.Forms.Label
        Me.lblRobot16 = New System.Windows.Forms.Label
        Me.lblRobot15 = New System.Windows.Forms.Label
        Me.lblStatC14 = New System.Windows.Forms.Label
        Me.lblStatC13 = New System.Windows.Forms.Label
        Me.lblStatB14 = New System.Windows.Forms.Label
        Me.lblStatB13 = New System.Windows.Forms.Label
        Me.lblStatA14 = New System.Windows.Forms.Label
        Me.lblStatA13 = New System.Windows.Forms.Label
        Me.lblCalStat14 = New System.Windows.Forms.Label
        Me.lblRobot14 = New System.Windows.Forms.Label
        Me.lblCalStat13 = New System.Windows.Forms.Label
        Me.lblRobot13 = New System.Windows.Forms.Label
        Me.lblStatCCap = New System.Windows.Forms.Label
        Me.lblStatC12 = New System.Windows.Forms.Label
        Me.lblStatC11 = New System.Windows.Forms.Label
        Me.lblStatC10 = New System.Windows.Forms.Label
        Me.lblStatC09 = New System.Windows.Forms.Label
        Me.lblStatC08 = New System.Windows.Forms.Label
        Me.lblStatC07 = New System.Windows.Forms.Label
        Me.lblStatC06 = New System.Windows.Forms.Label
        Me.lblStatC05 = New System.Windows.Forms.Label
        Me.lblStatC04 = New System.Windows.Forms.Label
        Me.lblStatC03 = New System.Windows.Forms.Label
        Me.lblStatC02 = New System.Windows.Forms.Label
        Me.lblStatC01 = New System.Windows.Forms.Label
        Me.lblStatB12 = New System.Windows.Forms.Label
        Me.lblStatA12 = New System.Windows.Forms.Label
        Me.lblCalStat12 = New System.Windows.Forms.Label
        Me.lblRobot12 = New System.Windows.Forms.Label
        Me.lblStatB11 = New System.Windows.Forms.Label
        Me.lblStatA11 = New System.Windows.Forms.Label
        Me.lblCalStat11 = New System.Windows.Forms.Label
        Me.lblRobot11 = New System.Windows.Forms.Label
        Me.lblStatB10 = New System.Windows.Forms.Label
        Me.lblStatB09 = New System.Windows.Forms.Label
        Me.lblStatB08 = New System.Windows.Forms.Label
        Me.lblStatB07 = New System.Windows.Forms.Label
        Me.lblStatB06 = New System.Windows.Forms.Label
        Me.lblStatB05 = New System.Windows.Forms.Label
        Me.lblStatB04 = New System.Windows.Forms.Label
        Me.lblStatB03 = New System.Windows.Forms.Label
        Me.lblStatB02 = New System.Windows.Forms.Label
        Me.lblStatB01 = New System.Windows.Forms.Label
        Me.lblStatA10 = New System.Windows.Forms.Label
        Me.lblStatA09 = New System.Windows.Forms.Label
        Me.lblStatA08 = New System.Windows.Forms.Label
        Me.lblStatA07 = New System.Windows.Forms.Label
        Me.lblStatA06 = New System.Windows.Forms.Label
        Me.lblStatA05 = New System.Windows.Forms.Label
        Me.lblStatA04 = New System.Windows.Forms.Label
        Me.lblStatA03 = New System.Windows.Forms.Label
        Me.lblStatA02 = New System.Windows.Forms.Label
        Me.lblStatA01 = New System.Windows.Forms.Label
        Me.lblStatBCap = New System.Windows.Forms.Label
        Me.lblStatACap = New System.Windows.Forms.Label
        Me.lblCalStatCap = New System.Windows.Forms.Label
        Me.lblCalStat10 = New System.Windows.Forms.Label
        Me.lblCalStat09 = New System.Windows.Forms.Label
        Me.lblCalStat08 = New System.Windows.Forms.Label
        Me.lblCalStat07 = New System.Windows.Forms.Label
        Me.lblCalStat06 = New System.Windows.Forms.Label
        Me.lblCalStat05 = New System.Windows.Forms.Label
        Me.lblCalStat04 = New System.Windows.Forms.Label
        Me.lblCalStat03 = New System.Windows.Forms.Label
        Me.lblCalStat02 = New System.Windows.Forms.Label
        Me.lblCalStat01 = New System.Windows.Forms.Label
        Me.lblRobot10 = New System.Windows.Forms.Label
        Me.lblRobot09 = New System.Windows.Forms.Label
        Me.lblRobot08 = New System.Windows.Forms.Label
        Me.lblRobot07 = New System.Windows.Forms.Label
        Me.lblRobot06 = New System.Windows.Forms.Label
        Me.lblRobot05 = New System.Windows.Forms.Label
        Me.lblRobot04 = New System.Windows.Forms.Label
        Me.lblRobot03 = New System.Windows.Forms.Label
        Me.lblRobot02 = New System.Windows.Forms.Label
        Me.lblRobot01 = New System.Windows.Forms.Label
        Me.lblCylPos = New System.Windows.Forms.Label
        Me.lblCylPosCap = New System.Windows.Forms.Label
        Me.cboApplicator = New System.Windows.Forms.ComboBox
        Me.lblApplicator = New System.Windows.Forms.Label
        Me.stsStatus = New System.Windows.Forms.StatusStrip
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel
        Me.tspProgress = New System.Windows.Forms.ToolStripProgressBar
        Me.lblSpacer = New System.Windows.Forms.ToolStripStatusLabel
        Me.btnFunction = New System.Windows.Forms.ToolStripDropDownButton
        Me.mnuLogin = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLogOut = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuRemote = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLocal = New System.Windows.Forms.ToolStripMenuItem
        Me.cboRobot = New System.Windows.Forms.ComboBox
        Me.gpbFlowParams = New System.Windows.Forms.GroupBox
        Me.ftbFan = New FocusedTextBox.FocusedTextBox
        Me.lblFan1Cap = New System.Windows.Forms.Label
        Me.mnuUnitPopup = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuSelectEngUnits = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuSelectCounts = New System.Windows.Forms.ToolStripMenuItem
        Me.ftbFan2 = New FocusedTextBox.FocusedTextBox
        Me.lblFan2Cap = New System.Windows.Forms.Label
        Me.lblCounts = New System.Windows.Forms.Label
        Me.ftbTime = New FocusedTextBox.FocusedTextBox
        Me.ftbEStat = New FocusedTextBox.FocusedTextBox
        Me.ftbAtom = New FocusedTextBox.FocusedTextBox
        Me.pnlFeedBack = New System.Windows.Forms.Panel
        Me.ftbActualFan2 = New FocusedTextBox.FocusedTextBox
        Me.lblFan2ActCap = New System.Windows.Forms.Label
        Me.ftbActualAtom = New FocusedTextBox.FocusedTextBox
        Me.lblFanFBCap = New System.Windows.Forms.Label
        Me.lblAtomFBCap = New System.Windows.Forms.Label
        Me.lblKVCap = New System.Windows.Forms.Label
        Me.lblUACap = New System.Windows.Forms.Label
        Me.ftbActualEstatUA = New FocusedTextBox.FocusedTextBox
        Me.ftbTotal = New FocusedTextBox.FocusedTextBox
        Me.ftbActualTime = New FocusedTextBox.FocusedTextBox
        Me.ftbActualEStatKv = New FocusedTextBox.FocusedTextBox
        Me.ftbActualFan = New FocusedTextBox.FocusedTextBox
        Me.ftbActualFlow = New FocusedTextBox.FocusedTextBox
        Me.proFlow = New System.Windows.Forms.ProgressBar
        Me.lblTotalCap = New System.Windows.Forms.Label
        Me.lblActualCap = New System.Windows.Forms.Label
        Me.lblValuesCap = New System.Windows.Forms.Label
        Me.ftbFlow = New FocusedTextBox.FocusedTextBox
        Me.lblTimeCap = New System.Windows.Forms.Label
        Me.lblEstatCap = New System.Windows.Forms.Label
        Me.lblFanCap = New System.Windows.Forms.Label
        Me.lblAtomCap = New System.Windows.Forms.Label
        Me.lblFlowCap = New System.Windows.Forms.Label
        Me.btnFlowTest = New System.Windows.Forms.Button
        Me.lblColor = New System.Windows.Forms.Label
        Me.cboColor = New System.Windows.Forms.ComboBox
        Me.lblParamOp = New System.Windows.Forms.Label
        Me.lblRobotView = New System.Windows.Forms.Label
        Me.lblZone = New System.Windows.Forms.Label
        Me.cboZone = New System.Windows.Forms.ComboBox
        Me.gpbCCCycle = New System.Windows.Forms.GroupBox
        Me.gpbCalibration = New System.Windows.Forms.GroupBox
        Me.btnPressTest = New System.Windows.Forms.Button
        Me.btnDQ2Cal = New System.Windows.Forms.Button
        Me.btnDQCal = New System.Windows.Forms.Button
        Me.btnAutoCal = New System.Windows.Forms.Button
        Me.gpbCCControl = New System.Windows.Forms.GroupBox
        Me.btnRun = New System.Windows.Forms.Button
        Me.cboCycle = New System.Windows.Forms.ComboBox
        Me.lblCCTime10 = New System.Windows.Forms.Label
        Me.lblCycleName10 = New System.Windows.Forms.Label
        Me.lblCCTime09 = New System.Windows.Forms.Label
        Me.lblCycleName09 = New System.Windows.Forms.Label
        Me.lblCCTime08 = New System.Windows.Forms.Label
        Me.lblCycleName08 = New System.Windows.Forms.Label
        Me.lblCCTime07 = New System.Windows.Forms.Label
        Me.lblCycleName07 = New System.Windows.Forms.Label
        Me.lblCCTime06 = New System.Windows.Forms.Label
        Me.lblCycleName06 = New System.Windows.Forms.Label
        Me.lblCCTime05 = New System.Windows.Forms.Label
        Me.lblCycleName05 = New System.Windows.Forms.Label
        Me.lblCCTime04 = New System.Windows.Forms.Label
        Me.lblCycleName04 = New System.Windows.Forms.Label
        Me.lblCCTime03 = New System.Windows.Forms.Label
        Me.lblCycleName03 = New System.Windows.Forms.Label
        Me.lblCCTime02 = New System.Windows.Forms.Label
        Me.lblCycleName02 = New System.Windows.Forms.Label
        Me.lblCCTime01 = New System.Windows.Forms.Label
        Me.lblCycleName01 = New System.Windows.Forms.Label
        Me.gpbRobots = New System.Windows.Forms.GroupBox
        Me.clbRobot = New System.Windows.Forms.CheckedListBox
        Me.mnuRobotList = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuSelectAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuUnSelectAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.btnSave = New System.Windows.Forms.ToolStripButton
        Me.btnUndo = New System.Windows.Forms.ToolStripButton
        Me.btnCopy = New System.Windows.Forms.ToolStripButton
        Me.btnPrint = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuPrint = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintPreview = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPageSetup = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintFile = New System.Windows.Forms.ToolStripMenuItem
        Me.MnuPrintOptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.btnChangeLog = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuLast24 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLast7 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAllChanges = New System.Windows.Forms.ToolStripMenuItem
        Me.btnMultiView = New System.Windows.Forms.ToolStripButton
        Me.btnRestore = New System.Windows.Forms.ToolStripButton
        Me.btnDevice = New System.Windows.Forms.ToolStripButton
        Me.btnStatus = New System.Windows.Forms.ToolStripButton
        Me.btnBeakerMode = New System.Windows.Forms.ToolStripButton
        Me.tmrPressureUpdate = New System.Windows.Forms.Timer(Me.components)
        Me.pnlTxt = New System.Windows.Forms.StatusBarPanel
        Me.pnlProg = New System.Windows.Forms.StatusBarPanel
        Me.imlGun = New System.Windows.Forms.ImageList(Me.components)
        Me.imlAppClnr = New System.Windows.Forms.ImageList(Me.components)
        Me.imlMisc = New System.Windows.Forms.ImageList(Me.components)
        Me.tmrUpdateSA = New System.Windows.Forms.Timer(Me.components)
        Me.ToolTipMain = New System.Windows.Forms.ToolTip(Me.components)
        Me.tscMain.ContentPanel.SuspendLayout()
        Me.tscMain.TopToolStripPanel.SuspendLayout()
        Me.tscMain.SuspendLayout()
        Me.gpbPumpEnables.SuspendLayout()
        Me.gpbCurrentColor.SuspendLayout()
        Me.gpbTPR.SuspendLayout()
        Me.gpbDockControl.SuspendLayout()
        Me.gpbViewAll.SuspendLayout()
        Me.mnuViewSelect.SuspendLayout()
        Me.stsStatus.SuspendLayout()
        Me.gpbFlowParams.SuspendLayout()
        Me.mnuUnitPopup.SuspendLayout()
        Me.pnlFeedBack.SuspendLayout()
        Me.gpbCCCycle.SuspendLayout()
        Me.gpbCalibration.SuspendLayout()
        Me.gpbCCControl.SuspendLayout()
        Me.gpbRobots.SuspendLayout()
        Me.mnuRobotList.SuspendLayout()
        Me.tlsMain.SuspendLayout()
        CType(Me.pnlTxt, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pnlProg, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tscMain
        '
        '
        'tscMain.ContentPanel
        '
        Me.tscMain.ContentPanel.Controls.Add(Me.gpbPumpEnables)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblHintAllRobots)
        Me.tscMain.ContentPanel.Controls.Add(Me.gpbCurrentColor)
        Me.tscMain.ContentPanel.Controls.Add(Me.gpbTPR)
        Me.tscMain.ContentPanel.Controls.Add(Me.gpbDockControl)
        Me.tscMain.ContentPanel.Controls.Add(Me.lstStatus)
        Me.tscMain.ContentPanel.Controls.Add(Me.gpbViewAll)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblCylPos)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblCylPosCap)
        Me.tscMain.ContentPanel.Controls.Add(Me.cboApplicator)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblApplicator)
        Me.tscMain.ContentPanel.Controls.Add(Me.stsStatus)
        Me.tscMain.ContentPanel.Controls.Add(Me.cboRobot)
        Me.tscMain.ContentPanel.Controls.Add(Me.gpbFlowParams)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblColor)
        Me.tscMain.ContentPanel.Controls.Add(Me.cboColor)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblParamOp)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblRobotView)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblZone)
        Me.tscMain.ContentPanel.Controls.Add(Me.cboZone)
        Me.tscMain.ContentPanel.Controls.Add(Me.gpbCCCycle)
        Me.tscMain.ContentPanel.Controls.Add(Me.gpbRobots)
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
        'gpbPumpEnables
        '
        Me.gpbPumpEnables.BackColor = System.Drawing.Color.Transparent
        Me.gpbPumpEnables.Controls.Add(Me.chkPump2)
        Me.gpbPumpEnables.Controls.Add(Me.chkPump1)
        resources.ApplyResources(Me.gpbPumpEnables, "gpbPumpEnables")
        Me.gpbPumpEnables.Name = "gpbPumpEnables"
        Me.gpbPumpEnables.TabStop = False
        '
        'chkPump2
        '
        resources.ApplyResources(Me.chkPump2, "chkPump2")
        Me.chkPump2.Checked = True
        Me.chkPump2.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkPump2.Name = "chkPump2"
        Me.chkPump2.UseVisualStyleBackColor = True
        '
        'chkPump1
        '
        resources.ApplyResources(Me.chkPump1, "chkPump1")
        Me.chkPump1.Checked = True
        Me.chkPump1.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkPump1.Name = "chkPump1"
        Me.chkPump1.UseVisualStyleBackColor = True
        '
        'lblHintAllRobots
        '
        Me.lblHintAllRobots.BackColor = System.Drawing.SystemColors.Info
        Me.lblHintAllRobots.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.lblHintAllRobots, "lblHintAllRobots")
        Me.lblHintAllRobots.Name = "lblHintAllRobots"
        '
        'gpbCurrentColor
        '
        Me.gpbCurrentColor.Controls.Add(Me.lblCurrentColor)
        resources.ApplyResources(Me.gpbCurrentColor, "gpbCurrentColor")
        Me.gpbCurrentColor.Name = "gpbCurrentColor"
        Me.gpbCurrentColor.TabStop = False
        '
        'lblCurrentColor
        '
        Me.lblCurrentColor.BackColor = System.Drawing.Color.White
        Me.lblCurrentColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblCurrentColor, "lblCurrentColor")
        Me.lblCurrentColor.Name = "lblCurrentColor"
        '
        'gpbTPR
        '
        Me.gpbTPR.Controls.Add(Me.txtTPR)
        resources.ApplyResources(Me.gpbTPR, "gpbTPR")
        Me.gpbTPR.Name = "gpbTPR"
        Me.gpbTPR.TabStop = False
        '
        'txtTPR
        '
        resources.ApplyResources(Me.txtTPR, "txtTPR")
        Me.txtTPR.ForeColor = System.Drawing.Color.Red
        Me.txtTPR.Name = "txtTPR"
        Me.txtTPR.NumericOnly = True
        Me.txtTPR.TabStop = False
        '
        'gpbDockControl
        '
        Me.gpbDockControl.BackColor = System.Drawing.Color.Transparent
        Me.gpbDockControl.Controls.Add(Me.btnMoveDeDock)
        Me.gpbDockControl.Controls.Add(Me.btnMoveClean)
        Me.gpbDockControl.Controls.Add(Me.btnMoveDock)
        resources.ApplyResources(Me.gpbDockControl, "gpbDockControl")
        Me.gpbDockControl.Name = "gpbDockControl"
        Me.gpbDockControl.TabStop = False
        '
        'btnMoveDeDock
        '
        resources.ApplyResources(Me.btnMoveDeDock, "btnMoveDeDock")
        Me.btnMoveDeDock.Name = "btnMoveDeDock"
        Me.btnMoveDeDock.Tag = Global.FluidMaint.Devices.dsZoneOff07
        Me.btnMoveDeDock.UseVisualStyleBackColor = True
        '
        'btnMoveClean
        '
        resources.ApplyResources(Me.btnMoveClean, "btnMoveClean")
        Me.btnMoveClean.Name = "btnMoveClean"
        Me.btnMoveClean.Tag = Global.FluidMaint.Devices.dsZoneOff07
        Me.btnMoveClean.UseVisualStyleBackColor = True
        '
        'btnMoveDock
        '
        resources.ApplyResources(Me.btnMoveDock, "btnMoveDock")
        Me.btnMoveDock.Name = "btnMoveDock"
        Me.btnMoveDock.Tag = Global.FluidMaint.Devices.dsZoneOff07
        Me.btnMoveDock.UseVisualStyleBackColor = True
        '
        'lstStatus
        '
        Me.lstStatus.BackColor = System.Drawing.SystemColors.Info
        resources.ApplyResources(Me.lstStatus, "lstStatus")
        Me.lstStatus.Name = "lstStatus"
        '
        'gpbViewAll
        '
        Me.gpbViewAll.ContextMenuStrip = Me.mnuViewSelect
        Me.gpbViewAll.Controls.Add(Me.lblStatC16)
        Me.gpbViewAll.Controls.Add(Me.lblStatC15)
        Me.gpbViewAll.Controls.Add(Me.lblStatB16)
        Me.gpbViewAll.Controls.Add(Me.lblStatB15)
        Me.gpbViewAll.Controls.Add(Me.lblStatA16)
        Me.gpbViewAll.Controls.Add(Me.lblStatA15)
        Me.gpbViewAll.Controls.Add(Me.lblCalStat16)
        Me.gpbViewAll.Controls.Add(Me.lblCalStat15)
        Me.gpbViewAll.Controls.Add(Me.lblRobot16)
        Me.gpbViewAll.Controls.Add(Me.lblRobot15)
        Me.gpbViewAll.Controls.Add(Me.lblStatC14)
        Me.gpbViewAll.Controls.Add(Me.lblStatC13)
        Me.gpbViewAll.Controls.Add(Me.lblStatB14)
        Me.gpbViewAll.Controls.Add(Me.lblStatB13)
        Me.gpbViewAll.Controls.Add(Me.lblStatA14)
        Me.gpbViewAll.Controls.Add(Me.lblStatA13)
        Me.gpbViewAll.Controls.Add(Me.lblCalStat14)
        Me.gpbViewAll.Controls.Add(Me.lblRobot14)
        Me.gpbViewAll.Controls.Add(Me.lblCalStat13)
        Me.gpbViewAll.Controls.Add(Me.lblRobot13)
        Me.gpbViewAll.Controls.Add(Me.lblStatCCap)
        Me.gpbViewAll.Controls.Add(Me.lblStatC12)
        Me.gpbViewAll.Controls.Add(Me.lblStatC11)
        Me.gpbViewAll.Controls.Add(Me.lblStatC10)
        Me.gpbViewAll.Controls.Add(Me.lblStatC09)
        Me.gpbViewAll.Controls.Add(Me.lblStatC08)
        Me.gpbViewAll.Controls.Add(Me.lblStatC07)
        Me.gpbViewAll.Controls.Add(Me.lblStatC06)
        Me.gpbViewAll.Controls.Add(Me.lblStatC05)
        Me.gpbViewAll.Controls.Add(Me.lblStatC04)
        Me.gpbViewAll.Controls.Add(Me.lblStatC03)
        Me.gpbViewAll.Controls.Add(Me.lblStatC02)
        Me.gpbViewAll.Controls.Add(Me.lblStatC01)
        Me.gpbViewAll.Controls.Add(Me.lblStatB12)
        Me.gpbViewAll.Controls.Add(Me.lblStatA12)
        Me.gpbViewAll.Controls.Add(Me.lblCalStat12)
        Me.gpbViewAll.Controls.Add(Me.lblRobot12)
        Me.gpbViewAll.Controls.Add(Me.lblStatB11)
        Me.gpbViewAll.Controls.Add(Me.lblStatA11)
        Me.gpbViewAll.Controls.Add(Me.lblCalStat11)
        Me.gpbViewAll.Controls.Add(Me.lblRobot11)
        Me.gpbViewAll.Controls.Add(Me.lblStatB10)
        Me.gpbViewAll.Controls.Add(Me.lblStatB09)
        Me.gpbViewAll.Controls.Add(Me.lblStatB08)
        Me.gpbViewAll.Controls.Add(Me.lblStatB07)
        Me.gpbViewAll.Controls.Add(Me.lblStatB06)
        Me.gpbViewAll.Controls.Add(Me.lblStatB05)
        Me.gpbViewAll.Controls.Add(Me.lblStatB04)
        Me.gpbViewAll.Controls.Add(Me.lblStatB03)
        Me.gpbViewAll.Controls.Add(Me.lblStatB02)
        Me.gpbViewAll.Controls.Add(Me.lblStatB01)
        Me.gpbViewAll.Controls.Add(Me.lblStatA10)
        Me.gpbViewAll.Controls.Add(Me.lblStatA09)
        Me.gpbViewAll.Controls.Add(Me.lblStatA08)
        Me.gpbViewAll.Controls.Add(Me.lblStatA07)
        Me.gpbViewAll.Controls.Add(Me.lblStatA06)
        Me.gpbViewAll.Controls.Add(Me.lblStatA05)
        Me.gpbViewAll.Controls.Add(Me.lblStatA04)
        Me.gpbViewAll.Controls.Add(Me.lblStatA03)
        Me.gpbViewAll.Controls.Add(Me.lblStatA02)
        Me.gpbViewAll.Controls.Add(Me.lblStatA01)
        Me.gpbViewAll.Controls.Add(Me.lblStatBCap)
        Me.gpbViewAll.Controls.Add(Me.lblStatACap)
        Me.gpbViewAll.Controls.Add(Me.lblCalStatCap)
        Me.gpbViewAll.Controls.Add(Me.lblCalStat10)
        Me.gpbViewAll.Controls.Add(Me.lblCalStat09)
        Me.gpbViewAll.Controls.Add(Me.lblCalStat08)
        Me.gpbViewAll.Controls.Add(Me.lblCalStat07)
        Me.gpbViewAll.Controls.Add(Me.lblCalStat06)
        Me.gpbViewAll.Controls.Add(Me.lblCalStat05)
        Me.gpbViewAll.Controls.Add(Me.lblCalStat04)
        Me.gpbViewAll.Controls.Add(Me.lblCalStat03)
        Me.gpbViewAll.Controls.Add(Me.lblCalStat02)
        Me.gpbViewAll.Controls.Add(Me.lblCalStat01)
        Me.gpbViewAll.Controls.Add(Me.lblRobot10)
        Me.gpbViewAll.Controls.Add(Me.lblRobot09)
        Me.gpbViewAll.Controls.Add(Me.lblRobot08)
        Me.gpbViewAll.Controls.Add(Me.lblRobot07)
        Me.gpbViewAll.Controls.Add(Me.lblRobot06)
        Me.gpbViewAll.Controls.Add(Me.lblRobot05)
        Me.gpbViewAll.Controls.Add(Me.lblRobot04)
        Me.gpbViewAll.Controls.Add(Me.lblRobot03)
        Me.gpbViewAll.Controls.Add(Me.lblRobot02)
        Me.gpbViewAll.Controls.Add(Me.lblRobot01)
        resources.ApplyResources(Me.gpbViewAll, "gpbViewAll")
        Me.gpbViewAll.Name = "gpbViewAll"
        Me.gpbViewAll.TabStop = False
        '
        'mnuViewSelect
        '
        Me.mnuViewSelect.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuViewSelectToon, Me.mnuViewSelectPaint, Me.mnuViewSelectAA_BS, Me.mnuViewSelectFA_SA, Me.mnuViewSelectEstat, Me.mnuViewSelectCalStat, Me.mnuViewSelectDockCalStat, Me.mnuViewSelectDQCalStat, Me.mnuViewSelectDQCalStat2, Me.mnuViewSelectPressureTest})
        Me.mnuViewSelect.Name = "mnuViewSelect"
        resources.ApplyResources(Me.mnuViewSelect, "mnuViewSelect")
        '
        'mnuViewSelectToon
        '
        Me.mnuViewSelectToon.Name = "mnuViewSelectToon"
        resources.ApplyResources(Me.mnuViewSelectToon, "mnuViewSelectToon")
        '
        'mnuViewSelectPaint
        '
        Me.mnuViewSelectPaint.Name = "mnuViewSelectPaint"
        resources.ApplyResources(Me.mnuViewSelectPaint, "mnuViewSelectPaint")
        '
        'mnuViewSelectAA_BS
        '
        Me.mnuViewSelectAA_BS.Name = "mnuViewSelectAA_BS"
        resources.ApplyResources(Me.mnuViewSelectAA_BS, "mnuViewSelectAA_BS")
        '
        'mnuViewSelectFA_SA
        '
        Me.mnuViewSelectFA_SA.Name = "mnuViewSelectFA_SA"
        resources.ApplyResources(Me.mnuViewSelectFA_SA, "mnuViewSelectFA_SA")
        '
        'mnuViewSelectEstat
        '
        Me.mnuViewSelectEstat.Name = "mnuViewSelectEstat"
        resources.ApplyResources(Me.mnuViewSelectEstat, "mnuViewSelectEstat")
        '
        'mnuViewSelectCalStat
        '
        Me.mnuViewSelectCalStat.Name = "mnuViewSelectCalStat"
        resources.ApplyResources(Me.mnuViewSelectCalStat, "mnuViewSelectCalStat")
        '
        'mnuViewSelectDockCalStat
        '
        Me.mnuViewSelectDockCalStat.Name = "mnuViewSelectDockCalStat"
        resources.ApplyResources(Me.mnuViewSelectDockCalStat, "mnuViewSelectDockCalStat")
        '
        'mnuViewSelectDQCalStat
        '
        Me.mnuViewSelectDQCalStat.Name = "mnuViewSelectDQCalStat"
        resources.ApplyResources(Me.mnuViewSelectDQCalStat, "mnuViewSelectDQCalStat")
        '
        'mnuViewSelectDQCalStat2
        '
        Me.mnuViewSelectDQCalStat2.Name = "mnuViewSelectDQCalStat2"
        resources.ApplyResources(Me.mnuViewSelectDQCalStat2, "mnuViewSelectDQCalStat2")
        '
        'mnuViewSelectPressureTest
        '
        Me.mnuViewSelectPressureTest.Name = "mnuViewSelectPressureTest"
        resources.ApplyResources(Me.mnuViewSelectPressureTest, "mnuViewSelectPressureTest")
        '
        'lblStatC16
        '
        Me.lblStatC16.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatC16.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatC16, "lblStatC16")
        Me.lblStatC16.Name = "lblStatC16"
        '
        'lblStatC15
        '
        Me.lblStatC15.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatC15.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatC15, "lblStatC15")
        Me.lblStatC15.Name = "lblStatC15"
        '
        'lblStatB16
        '
        Me.lblStatB16.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatB16.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatB16, "lblStatB16")
        Me.lblStatB16.Name = "lblStatB16"
        '
        'lblStatB15
        '
        Me.lblStatB15.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatB15.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatB15, "lblStatB15")
        Me.lblStatB15.Name = "lblStatB15"
        '
        'lblStatA16
        '
        Me.lblStatA16.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatA16.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatA16, "lblStatA16")
        Me.lblStatA16.Name = "lblStatA16"
        '
        'lblStatA15
        '
        Me.lblStatA15.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatA15.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatA15, "lblStatA15")
        Me.lblStatA15.Name = "lblStatA15"
        '
        'lblCalStat16
        '
        Me.lblCalStat16.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCalStat16.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblCalStat16, "lblCalStat16")
        Me.lblCalStat16.Name = "lblCalStat16"
        '
        'lblCalStat15
        '
        Me.lblCalStat15.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCalStat15.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblCalStat15, "lblCalStat15")
        Me.lblCalStat15.Name = "lblCalStat15"
        '
        'lblRobot16
        '
        Me.lblRobot16.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblRobot16, "lblRobot16")
        Me.lblRobot16.Name = "lblRobot16"
        '
        'lblRobot15
        '
        Me.lblRobot15.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblRobot15, "lblRobot15")
        Me.lblRobot15.Name = "lblRobot15"
        '
        'lblStatC14
        '
        Me.lblStatC14.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatC14.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatC14, "lblStatC14")
        Me.lblStatC14.Name = "lblStatC14"
        '
        'lblStatC13
        '
        Me.lblStatC13.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatC13.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatC13, "lblStatC13")
        Me.lblStatC13.Name = "lblStatC13"
        '
        'lblStatB14
        '
        Me.lblStatB14.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatB14.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatB14, "lblStatB14")
        Me.lblStatB14.Name = "lblStatB14"
        '
        'lblStatB13
        '
        Me.lblStatB13.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatB13.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatB13, "lblStatB13")
        Me.lblStatB13.Name = "lblStatB13"
        '
        'lblStatA14
        '
        Me.lblStatA14.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatA14.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatA14, "lblStatA14")
        Me.lblStatA14.Name = "lblStatA14"
        '
        'lblStatA13
        '
        Me.lblStatA13.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatA13.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatA13, "lblStatA13")
        Me.lblStatA13.Name = "lblStatA13"
        '
        'lblCalStat14
        '
        Me.lblCalStat14.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCalStat14.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblCalStat14, "lblCalStat14")
        Me.lblCalStat14.Name = "lblCalStat14"
        '
        'lblRobot14
        '
        Me.lblRobot14.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblRobot14, "lblRobot14")
        Me.lblRobot14.Name = "lblRobot14"
        '
        'lblCalStat13
        '
        Me.lblCalStat13.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCalStat13.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblCalStat13, "lblCalStat13")
        Me.lblCalStat13.Name = "lblCalStat13"
        '
        'lblRobot13
        '
        Me.lblRobot13.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblRobot13, "lblRobot13")
        Me.lblRobot13.Name = "lblRobot13"
        '
        'lblStatCCap
        '
        resources.ApplyResources(Me.lblStatCCap, "lblStatCCap")
        Me.lblStatCCap.Name = "lblStatCCap"
        '
        'lblStatC12
        '
        Me.lblStatC12.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatC12.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatC12, "lblStatC12")
        Me.lblStatC12.Name = "lblStatC12"
        '
        'lblStatC11
        '
        Me.lblStatC11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatC11.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatC11, "lblStatC11")
        Me.lblStatC11.Name = "lblStatC11"
        '
        'lblStatC10
        '
        Me.lblStatC10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatC10.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatC10, "lblStatC10")
        Me.lblStatC10.Name = "lblStatC10"
        '
        'lblStatC09
        '
        Me.lblStatC09.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatC09.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatC09, "lblStatC09")
        Me.lblStatC09.Name = "lblStatC09"
        '
        'lblStatC08
        '
        Me.lblStatC08.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatC08.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatC08, "lblStatC08")
        Me.lblStatC08.Name = "lblStatC08"
        '
        'lblStatC07
        '
        Me.lblStatC07.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatC07.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatC07, "lblStatC07")
        Me.lblStatC07.Name = "lblStatC07"
        '
        'lblStatC06
        '
        Me.lblStatC06.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatC06.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatC06, "lblStatC06")
        Me.lblStatC06.Name = "lblStatC06"
        '
        'lblStatC05
        '
        Me.lblStatC05.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatC05.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatC05, "lblStatC05")
        Me.lblStatC05.Name = "lblStatC05"
        '
        'lblStatC04
        '
        Me.lblStatC04.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatC04.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatC04, "lblStatC04")
        Me.lblStatC04.Name = "lblStatC04"
        '
        'lblStatC03
        '
        Me.lblStatC03.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatC03.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatC03, "lblStatC03")
        Me.lblStatC03.Name = "lblStatC03"
        '
        'lblStatC02
        '
        Me.lblStatC02.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatC02.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatC02, "lblStatC02")
        Me.lblStatC02.Name = "lblStatC02"
        '
        'lblStatC01
        '
        Me.lblStatC01.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatC01.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatC01, "lblStatC01")
        Me.lblStatC01.Name = "lblStatC01"
        '
        'lblStatB12
        '
        Me.lblStatB12.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatB12.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatB12, "lblStatB12")
        Me.lblStatB12.Name = "lblStatB12"
        '
        'lblStatA12
        '
        Me.lblStatA12.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatA12.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatA12, "lblStatA12")
        Me.lblStatA12.Name = "lblStatA12"
        '
        'lblCalStat12
        '
        Me.lblCalStat12.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCalStat12.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblCalStat12, "lblCalStat12")
        Me.lblCalStat12.Name = "lblCalStat12"
        '
        'lblRobot12
        '
        Me.lblRobot12.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblRobot12, "lblRobot12")
        Me.lblRobot12.Name = "lblRobot12"
        '
        'lblStatB11
        '
        Me.lblStatB11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatB11.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatB11, "lblStatB11")
        Me.lblStatB11.Name = "lblStatB11"
        '
        'lblStatA11
        '
        Me.lblStatA11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatA11.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatA11, "lblStatA11")
        Me.lblStatA11.Name = "lblStatA11"
        '
        'lblCalStat11
        '
        Me.lblCalStat11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCalStat11.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblCalStat11, "lblCalStat11")
        Me.lblCalStat11.Name = "lblCalStat11"
        '
        'lblRobot11
        '
        Me.lblRobot11.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblRobot11, "lblRobot11")
        Me.lblRobot11.Name = "lblRobot11"
        '
        'lblStatB10
        '
        Me.lblStatB10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatB10.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatB10, "lblStatB10")
        Me.lblStatB10.Name = "lblStatB10"
        '
        'lblStatB09
        '
        Me.lblStatB09.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatB09.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatB09, "lblStatB09")
        Me.lblStatB09.Name = "lblStatB09"
        '
        'lblStatB08
        '
        Me.lblStatB08.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatB08.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatB08, "lblStatB08")
        Me.lblStatB08.Name = "lblStatB08"
        '
        'lblStatB07
        '
        Me.lblStatB07.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatB07.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatB07, "lblStatB07")
        Me.lblStatB07.Name = "lblStatB07"
        '
        'lblStatB06
        '
        Me.lblStatB06.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatB06.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatB06, "lblStatB06")
        Me.lblStatB06.Name = "lblStatB06"
        '
        'lblStatB05
        '
        Me.lblStatB05.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatB05.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatB05, "lblStatB05")
        Me.lblStatB05.Name = "lblStatB05"
        '
        'lblStatB04
        '
        Me.lblStatB04.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatB04.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatB04, "lblStatB04")
        Me.lblStatB04.Name = "lblStatB04"
        '
        'lblStatB03
        '
        Me.lblStatB03.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatB03.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatB03, "lblStatB03")
        Me.lblStatB03.Name = "lblStatB03"
        '
        'lblStatB02
        '
        Me.lblStatB02.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatB02.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatB02, "lblStatB02")
        Me.lblStatB02.Name = "lblStatB02"
        '
        'lblStatB01
        '
        Me.lblStatB01.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatB01.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatB01, "lblStatB01")
        Me.lblStatB01.Name = "lblStatB01"
        '
        'lblStatA10
        '
        Me.lblStatA10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatA10.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatA10, "lblStatA10")
        Me.lblStatA10.Name = "lblStatA10"
        '
        'lblStatA09
        '
        Me.lblStatA09.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatA09.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatA09, "lblStatA09")
        Me.lblStatA09.Name = "lblStatA09"
        '
        'lblStatA08
        '
        Me.lblStatA08.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatA08.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatA08, "lblStatA08")
        Me.lblStatA08.Name = "lblStatA08"
        '
        'lblStatA07
        '
        Me.lblStatA07.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatA07.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatA07, "lblStatA07")
        Me.lblStatA07.Name = "lblStatA07"
        '
        'lblStatA06
        '
        Me.lblStatA06.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatA06.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatA06, "lblStatA06")
        Me.lblStatA06.Name = "lblStatA06"
        '
        'lblStatA05
        '
        Me.lblStatA05.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatA05.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatA05, "lblStatA05")
        Me.lblStatA05.Name = "lblStatA05"
        '
        'lblStatA04
        '
        Me.lblStatA04.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatA04.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatA04, "lblStatA04")
        Me.lblStatA04.Name = "lblStatA04"
        '
        'lblStatA03
        '
        Me.lblStatA03.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatA03.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatA03, "lblStatA03")
        Me.lblStatA03.Name = "lblStatA03"
        '
        'lblStatA02
        '
        Me.lblStatA02.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatA02.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatA02, "lblStatA02")
        Me.lblStatA02.Name = "lblStatA02"
        '
        'lblStatA01
        '
        Me.lblStatA01.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatA01.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblStatA01, "lblStatA01")
        Me.lblStatA01.Name = "lblStatA01"
        '
        'lblStatBCap
        '
        resources.ApplyResources(Me.lblStatBCap, "lblStatBCap")
        Me.lblStatBCap.Name = "lblStatBCap"
        '
        'lblStatACap
        '
        resources.ApplyResources(Me.lblStatACap, "lblStatACap")
        Me.lblStatACap.Name = "lblStatACap"
        '
        'lblCalStatCap
        '
        resources.ApplyResources(Me.lblCalStatCap, "lblCalStatCap")
        Me.lblCalStatCap.Name = "lblCalStatCap"
        '
        'lblCalStat10
        '
        Me.lblCalStat10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCalStat10.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblCalStat10, "lblCalStat10")
        Me.lblCalStat10.Name = "lblCalStat10"
        '
        'lblCalStat09
        '
        Me.lblCalStat09.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCalStat09.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblCalStat09, "lblCalStat09")
        Me.lblCalStat09.Name = "lblCalStat09"
        '
        'lblCalStat08
        '
        Me.lblCalStat08.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCalStat08.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblCalStat08, "lblCalStat08")
        Me.lblCalStat08.Name = "lblCalStat08"
        '
        'lblCalStat07
        '
        Me.lblCalStat07.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCalStat07.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblCalStat07, "lblCalStat07")
        Me.lblCalStat07.Name = "lblCalStat07"
        '
        'lblCalStat06
        '
        Me.lblCalStat06.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCalStat06.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblCalStat06, "lblCalStat06")
        Me.lblCalStat06.Name = "lblCalStat06"
        '
        'lblCalStat05
        '
        Me.lblCalStat05.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCalStat05.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblCalStat05, "lblCalStat05")
        Me.lblCalStat05.Name = "lblCalStat05"
        '
        'lblCalStat04
        '
        Me.lblCalStat04.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCalStat04.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblCalStat04, "lblCalStat04")
        Me.lblCalStat04.Name = "lblCalStat04"
        '
        'lblCalStat03
        '
        Me.lblCalStat03.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCalStat03.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblCalStat03, "lblCalStat03")
        Me.lblCalStat03.Name = "lblCalStat03"
        '
        'lblCalStat02
        '
        Me.lblCalStat02.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCalStat02.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblCalStat02, "lblCalStat02")
        Me.lblCalStat02.Name = "lblCalStat02"
        '
        'lblCalStat01
        '
        Me.lblCalStat01.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCalStat01.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblCalStat01, "lblCalStat01")
        Me.lblCalStat01.Name = "lblCalStat01"
        '
        'lblRobot10
        '
        Me.lblRobot10.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblRobot10, "lblRobot10")
        Me.lblRobot10.Name = "lblRobot10"
        '
        'lblRobot09
        '
        Me.lblRobot09.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblRobot09, "lblRobot09")
        Me.lblRobot09.Name = "lblRobot09"
        '
        'lblRobot08
        '
        Me.lblRobot08.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblRobot08, "lblRobot08")
        Me.lblRobot08.Name = "lblRobot08"
        '
        'lblRobot07
        '
        Me.lblRobot07.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblRobot07, "lblRobot07")
        Me.lblRobot07.Name = "lblRobot07"
        '
        'lblRobot06
        '
        Me.lblRobot06.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblRobot06, "lblRobot06")
        Me.lblRobot06.Name = "lblRobot06"
        '
        'lblRobot05
        '
        Me.lblRobot05.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblRobot05, "lblRobot05")
        Me.lblRobot05.Name = "lblRobot05"
        '
        'lblRobot04
        '
        Me.lblRobot04.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblRobot04, "lblRobot04")
        Me.lblRobot04.Name = "lblRobot04"
        '
        'lblRobot03
        '
        Me.lblRobot03.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblRobot03, "lblRobot03")
        Me.lblRobot03.Name = "lblRobot03"
        '
        'lblRobot02
        '
        Me.lblRobot02.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblRobot02, "lblRobot02")
        Me.lblRobot02.Name = "lblRobot02"
        '
        'lblRobot01
        '
        Me.lblRobot01.ContextMenuStrip = Me.mnuViewSelect
        resources.ApplyResources(Me.lblRobot01, "lblRobot01")
        Me.lblRobot01.Name = "lblRobot01"
        '
        'lblCylPos
        '
        resources.ApplyResources(Me.lblCylPos, "lblCylPos")
        Me.lblCylPos.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCylPos.Name = "lblCylPos"
        '
        'lblCylPosCap
        '
        resources.ApplyResources(Me.lblCylPosCap, "lblCylPosCap")
        Me.lblCylPosCap.Name = "lblCylPosCap"
        '
        'cboApplicator
        '
        Me.cboApplicator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboApplicator, "cboApplicator")
        Me.cboApplicator.Name = "cboApplicator"
        '
        'lblApplicator
        '
        Me.lblApplicator.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblApplicator, "lblApplicator")
        Me.lblApplicator.Name = "lblApplicator"
        Me.lblApplicator.UseMnemonic = False
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
        resources.ApplyResources(Me.lblStatus, "lblStatus")
        Me.lblStatus.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedOuter
        Me.lblStatus.Margin = New System.Windows.Forms.Padding(0)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Spring = True
        '
        'tspProgress
        '
        resources.ApplyResources(Me.tspProgress, "tspProgress")
        Me.tspProgress.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.tspProgress.Margin = New System.Windows.Forms.Padding(2, 3, 1, 3)
        Me.tspProgress.Name = "tspProgress"
        Me.tspProgress.Padding = New System.Windows.Forms.Padding(2)
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
        resources.ApplyResources(Me.btnFunction, "btnFunction")
        Me.btnFunction.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnFunction.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuLogin, Me.mnuLogOut, Me.mnuRemote, Me.mnuLocal})
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
        'cboRobot
        '
        Me.cboRobot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboRobot, "cboRobot")
        Me.cboRobot.Name = "cboRobot"
        '
        'gpbFlowParams
        '
        Me.gpbFlowParams.Controls.Add(Me.ftbFan)
        Me.gpbFlowParams.Controls.Add(Me.lblFan1Cap)
        Me.gpbFlowParams.Controls.Add(Me.ftbFan2)
        Me.gpbFlowParams.Controls.Add(Me.lblFan2Cap)
        Me.gpbFlowParams.Controls.Add(Me.lblCounts)
        Me.gpbFlowParams.Controls.Add(Me.ftbTime)
        Me.gpbFlowParams.Controls.Add(Me.ftbEStat)
        Me.gpbFlowParams.Controls.Add(Me.ftbAtom)
        Me.gpbFlowParams.Controls.Add(Me.pnlFeedBack)
        Me.gpbFlowParams.Controls.Add(Me.proFlow)
        Me.gpbFlowParams.Controls.Add(Me.lblTotalCap)
        Me.gpbFlowParams.Controls.Add(Me.lblActualCap)
        Me.gpbFlowParams.Controls.Add(Me.lblValuesCap)
        Me.gpbFlowParams.Controls.Add(Me.ftbFlow)
        Me.gpbFlowParams.Controls.Add(Me.lblTimeCap)
        Me.gpbFlowParams.Controls.Add(Me.lblEstatCap)
        Me.gpbFlowParams.Controls.Add(Me.lblFanCap)
        Me.gpbFlowParams.Controls.Add(Me.lblAtomCap)
        Me.gpbFlowParams.Controls.Add(Me.lblFlowCap)
        Me.gpbFlowParams.Controls.Add(Me.btnFlowTest)
        resources.ApplyResources(Me.gpbFlowParams, "gpbFlowParams")
        Me.gpbFlowParams.Name = "gpbFlowParams"
        Me.gpbFlowParams.TabStop = False
        '
        'ftbFan
        '
        Me.ftbFan.ForeColor = System.Drawing.Color.Red
        resources.ApplyResources(Me.ftbFan, "ftbFan")
        Me.ftbFan.Name = "ftbFan"
        Me.ftbFan.NumericOnly = True
        Me.ftbFan.TabStop = False
        '
        'lblFan1Cap
        '
        resources.ApplyResources(Me.lblFan1Cap, "lblFan1Cap")
        Me.lblFan1Cap.ContextMenuStrip = Me.mnuUnitPopup
        Me.lblFan1Cap.Name = "lblFan1Cap"
        '
        'mnuUnitPopup
        '
        Me.mnuUnitPopup.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuSelectEngUnits, Me.mnuSelectCounts})
        Me.mnuUnitPopup.Name = "cmnuRobotList"
        resources.ApplyResources(Me.mnuUnitPopup, "mnuUnitPopup")
        '
        'mnuSelectEngUnits
        '
        Me.mnuSelectEngUnits.Name = "mnuSelectEngUnits"
        resources.ApplyResources(Me.mnuSelectEngUnits, "mnuSelectEngUnits")
        '
        'mnuSelectCounts
        '
        Me.mnuSelectCounts.Name = "mnuSelectCounts"
        resources.ApplyResources(Me.mnuSelectCounts, "mnuSelectCounts")
        '
        'ftbFan2
        '
        Me.ftbFan2.ForeColor = System.Drawing.Color.Red
        resources.ApplyResources(Me.ftbFan2, "ftbFan2")
        Me.ftbFan2.Name = "ftbFan2"
        Me.ftbFan2.NumericOnly = True
        Me.ftbFan2.TabStop = False
        '
        'lblFan2Cap
        '
        resources.ApplyResources(Me.lblFan2Cap, "lblFan2Cap")
        Me.lblFan2Cap.ContextMenuStrip = Me.mnuUnitPopup
        Me.lblFan2Cap.Name = "lblFan2Cap"
        '
        'lblCounts
        '
        resources.ApplyResources(Me.lblCounts, "lblCounts")
        Me.lblCounts.Name = "lblCounts"
        '
        'ftbTime
        '
        Me.ftbTime.ForeColor = System.Drawing.Color.Red
        resources.ApplyResources(Me.ftbTime, "ftbTime")
        Me.ftbTime.Name = "ftbTime"
        Me.ftbTime.NumericOnly = True
        Me.ftbTime.TabStop = False
        '
        'ftbEStat
        '
        Me.ftbEStat.ForeColor = System.Drawing.Color.Red
        resources.ApplyResources(Me.ftbEStat, "ftbEStat")
        Me.ftbEStat.Name = "ftbEStat"
        Me.ftbEStat.NumericOnly = True
        Me.ftbEStat.TabStop = False
        '
        'ftbAtom
        '
        Me.ftbAtom.ForeColor = System.Drawing.Color.Red
        resources.ApplyResources(Me.ftbAtom, "ftbAtom")
        Me.ftbAtom.Name = "ftbAtom"
        Me.ftbAtom.NumericOnly = True
        Me.ftbAtom.TabStop = False
        '
        'pnlFeedBack
        '
        Me.pnlFeedBack.Controls.Add(Me.ftbActualFan2)
        Me.pnlFeedBack.Controls.Add(Me.lblFan2ActCap)
        Me.pnlFeedBack.Controls.Add(Me.ftbActualAtom)
        Me.pnlFeedBack.Controls.Add(Me.lblFanFBCap)
        Me.pnlFeedBack.Controls.Add(Me.lblAtomFBCap)
        Me.pnlFeedBack.Controls.Add(Me.lblKVCap)
        Me.pnlFeedBack.Controls.Add(Me.lblUACap)
        Me.pnlFeedBack.Controls.Add(Me.ftbActualEstatUA)
        Me.pnlFeedBack.Controls.Add(Me.ftbTotal)
        Me.pnlFeedBack.Controls.Add(Me.ftbActualTime)
        Me.pnlFeedBack.Controls.Add(Me.ftbActualEStatKv)
        Me.pnlFeedBack.Controls.Add(Me.ftbActualFan)
        Me.pnlFeedBack.Controls.Add(Me.ftbActualFlow)
        resources.ApplyResources(Me.pnlFeedBack, "pnlFeedBack")
        Me.pnlFeedBack.Name = "pnlFeedBack"
        '
        'ftbActualFan2
        '
        Me.ftbActualFan2.BackColor = System.Drawing.Color.White
        Me.ftbActualFan2.Cursor = System.Windows.Forms.Cursors.Default
        Me.ftbActualFan2.ForeColor = System.Drawing.Color.Red
        resources.ApplyResources(Me.ftbActualFan2, "ftbActualFan2")
        Me.ftbActualFan2.Name = "ftbActualFan2"
        Me.ftbActualFan2.NumericOnly = True
        Me.ftbActualFan2.ReadOnly = True
        Me.ftbActualFan2.TabStop = False
        '
        'lblFan2ActCap
        '
        resources.ApplyResources(Me.lblFan2ActCap, "lblFan2ActCap")
        Me.lblFan2ActCap.ContextMenuStrip = Me.mnuUnitPopup
        Me.lblFan2ActCap.Name = "lblFan2ActCap"
        '
        'ftbActualAtom
        '
        Me.ftbActualAtom.BackColor = System.Drawing.Color.White
        Me.ftbActualAtom.Cursor = System.Windows.Forms.Cursors.Default
        Me.ftbActualAtom.ForeColor = System.Drawing.Color.Red
        resources.ApplyResources(Me.ftbActualAtom, "ftbActualAtom")
        Me.ftbActualAtom.Name = "ftbActualAtom"
        Me.ftbActualAtom.NumericOnly = True
        Me.ftbActualAtom.ReadOnly = True
        Me.ftbActualAtom.TabStop = False
        '
        'lblFanFBCap
        '
        resources.ApplyResources(Me.lblFanFBCap, "lblFanFBCap")
        Me.lblFanFBCap.Name = "lblFanFBCap"
        '
        'lblAtomFBCap
        '
        resources.ApplyResources(Me.lblAtomFBCap, "lblAtomFBCap")
        Me.lblAtomFBCap.Name = "lblAtomFBCap"
        '
        'lblKVCap
        '
        resources.ApplyResources(Me.lblKVCap, "lblKVCap")
        Me.lblKVCap.Name = "lblKVCap"
        '
        'lblUACap
        '
        resources.ApplyResources(Me.lblUACap, "lblUACap")
        Me.lblUACap.Name = "lblUACap"
        '
        'ftbActualEstatUA
        '
        Me.ftbActualEstatUA.BackColor = System.Drawing.Color.White
        Me.ftbActualEstatUA.Cursor = System.Windows.Forms.Cursors.Default
        Me.ftbActualEstatUA.ForeColor = System.Drawing.Color.Red
        resources.ApplyResources(Me.ftbActualEstatUA, "ftbActualEstatUA")
        Me.ftbActualEstatUA.Name = "ftbActualEstatUA"
        Me.ftbActualEstatUA.NumericOnly = True
        Me.ftbActualEstatUA.ReadOnly = True
        Me.ftbActualEstatUA.TabStop = False
        '
        'ftbTotal
        '
        Me.ftbTotal.BackColor = System.Drawing.Color.White
        Me.ftbTotal.Cursor = System.Windows.Forms.Cursors.Default
        Me.ftbTotal.ForeColor = System.Drawing.Color.Red
        resources.ApplyResources(Me.ftbTotal, "ftbTotal")
        Me.ftbTotal.Name = "ftbTotal"
        Me.ftbTotal.NumericOnly = True
        Me.ftbTotal.ReadOnly = True
        Me.ftbTotal.TabStop = False
        '
        'ftbActualTime
        '
        Me.ftbActualTime.BackColor = System.Drawing.Color.White
        Me.ftbActualTime.Cursor = System.Windows.Forms.Cursors.Default
        Me.ftbActualTime.ForeColor = System.Drawing.Color.Red
        resources.ApplyResources(Me.ftbActualTime, "ftbActualTime")
        Me.ftbActualTime.Name = "ftbActualTime"
        Me.ftbActualTime.NumericOnly = True
        Me.ftbActualTime.ReadOnly = True
        Me.ftbActualTime.TabStop = False
        '
        'ftbActualEStatKv
        '
        Me.ftbActualEStatKv.BackColor = System.Drawing.Color.White
        Me.ftbActualEStatKv.Cursor = System.Windows.Forms.Cursors.Default
        Me.ftbActualEStatKv.ForeColor = System.Drawing.Color.Red
        resources.ApplyResources(Me.ftbActualEStatKv, "ftbActualEStatKv")
        Me.ftbActualEStatKv.Name = "ftbActualEStatKv"
        Me.ftbActualEStatKv.NumericOnly = True
        Me.ftbActualEStatKv.ReadOnly = True
        Me.ftbActualEStatKv.TabStop = False
        '
        'ftbActualFan
        '
        Me.ftbActualFan.BackColor = System.Drawing.Color.White
        Me.ftbActualFan.Cursor = System.Windows.Forms.Cursors.Default
        Me.ftbActualFan.ForeColor = System.Drawing.Color.Red
        resources.ApplyResources(Me.ftbActualFan, "ftbActualFan")
        Me.ftbActualFan.Name = "ftbActualFan"
        Me.ftbActualFan.NumericOnly = True
        Me.ftbActualFan.ReadOnly = True
        Me.ftbActualFan.TabStop = False
        '
        'ftbActualFlow
        '
        Me.ftbActualFlow.BackColor = System.Drawing.Color.White
        Me.ftbActualFlow.Cursor = System.Windows.Forms.Cursors.Default
        Me.ftbActualFlow.ForeColor = System.Drawing.Color.Red
        resources.ApplyResources(Me.ftbActualFlow, "ftbActualFlow")
        Me.ftbActualFlow.Name = "ftbActualFlow"
        Me.ftbActualFlow.NumericOnly = True
        Me.ftbActualFlow.ReadOnly = True
        Me.ftbActualFlow.TabStop = False
        '
        'proFlow
        '
        resources.ApplyResources(Me.proFlow, "proFlow")
        Me.proFlow.Name = "proFlow"
        Me.proFlow.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        '
        'lblTotalCap
        '
        resources.ApplyResources(Me.lblTotalCap, "lblTotalCap")
        Me.lblTotalCap.BackColor = System.Drawing.Color.Transparent
        Me.lblTotalCap.Name = "lblTotalCap"
        '
        'lblActualCap
        '
        resources.ApplyResources(Me.lblActualCap, "lblActualCap")
        Me.lblActualCap.BackColor = System.Drawing.Color.Transparent
        Me.lblActualCap.Name = "lblActualCap"
        '
        'lblValuesCap
        '
        resources.ApplyResources(Me.lblValuesCap, "lblValuesCap")
        Me.lblValuesCap.Name = "lblValuesCap"
        '
        'ftbFlow
        '
        Me.ftbFlow.ForeColor = System.Drawing.Color.Red
        resources.ApplyResources(Me.ftbFlow, "ftbFlow")
        Me.ftbFlow.Name = "ftbFlow"
        Me.ftbFlow.NumericOnly = True
        Me.ftbFlow.TabStop = False
        '
        'lblTimeCap
        '
        resources.ApplyResources(Me.lblTimeCap, "lblTimeCap")
        Me.lblTimeCap.Name = "lblTimeCap"
        '
        'lblEstatCap
        '
        resources.ApplyResources(Me.lblEstatCap, "lblEstatCap")
        Me.lblEstatCap.ContextMenuStrip = Me.mnuUnitPopup
        Me.lblEstatCap.Name = "lblEstatCap"
        '
        'lblFanCap
        '
        resources.ApplyResources(Me.lblFanCap, "lblFanCap")
        Me.lblFanCap.ContextMenuStrip = Me.mnuUnitPopup
        Me.lblFanCap.Name = "lblFanCap"
        '
        'lblAtomCap
        '
        resources.ApplyResources(Me.lblAtomCap, "lblAtomCap")
        Me.lblAtomCap.ContextMenuStrip = Me.mnuUnitPopup
        Me.lblAtomCap.Name = "lblAtomCap"
        '
        'lblFlowCap
        '
        resources.ApplyResources(Me.lblFlowCap, "lblFlowCap")
        Me.lblFlowCap.ContextMenuStrip = Me.mnuUnitPopup
        Me.lblFlowCap.Name = "lblFlowCap"
        '
        'btnFlowTest
        '
        resources.ApplyResources(Me.btnFlowTest, "btnFlowTest")
        Me.btnFlowTest.Name = "btnFlowTest"
        Me.btnFlowTest.UseVisualStyleBackColor = True
        '
        'lblColor
        '
        Me.lblColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblColor, "lblColor")
        Me.lblColor.Name = "lblColor"
        Me.lblColor.UseMnemonic = False
        '
        'cboColor
        '
        Me.cboColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboColor, "cboColor")
        Me.cboColor.Name = "cboColor"
        '
        'lblParamOp
        '
        Me.lblParamOp.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblParamOp, "lblParamOp")
        Me.lblParamOp.Name = "lblParamOp"
        Me.lblParamOp.UseMnemonic = False
        '
        'lblRobotView
        '
        Me.lblRobotView.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblRobotView, "lblRobotView")
        Me.lblRobotView.Name = "lblRobotView"
        Me.lblRobotView.UseMnemonic = False
        '
        'lblZone
        '
        Me.lblZone.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblZone, "lblZone")
        Me.lblZone.Name = "lblZone"
        Me.lblZone.UseMnemonic = False
        '
        'cboZone
        '
        Me.cboZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboZone, "cboZone")
        Me.cboZone.Name = "cboZone"
        '
        'gpbCCCycle
        '
        Me.gpbCCCycle.BackColor = System.Drawing.Color.Transparent
        Me.gpbCCCycle.Controls.Add(Me.gpbCalibration)
        Me.gpbCCCycle.Controls.Add(Me.gpbCCControl)
        Me.gpbCCCycle.Controls.Add(Me.lblCCTime10)
        Me.gpbCCCycle.Controls.Add(Me.lblCycleName10)
        Me.gpbCCCycle.Controls.Add(Me.lblCCTime09)
        Me.gpbCCCycle.Controls.Add(Me.lblCycleName09)
        Me.gpbCCCycle.Controls.Add(Me.lblCCTime08)
        Me.gpbCCCycle.Controls.Add(Me.lblCycleName08)
        Me.gpbCCCycle.Controls.Add(Me.lblCCTime07)
        Me.gpbCCCycle.Controls.Add(Me.lblCycleName07)
        Me.gpbCCCycle.Controls.Add(Me.lblCCTime06)
        Me.gpbCCCycle.Controls.Add(Me.lblCycleName06)
        Me.gpbCCCycle.Controls.Add(Me.lblCCTime05)
        Me.gpbCCCycle.Controls.Add(Me.lblCycleName05)
        Me.gpbCCCycle.Controls.Add(Me.lblCCTime04)
        Me.gpbCCCycle.Controls.Add(Me.lblCycleName04)
        Me.gpbCCCycle.Controls.Add(Me.lblCCTime03)
        Me.gpbCCCycle.Controls.Add(Me.lblCycleName03)
        Me.gpbCCCycle.Controls.Add(Me.lblCCTime02)
        Me.gpbCCCycle.Controls.Add(Me.lblCycleName02)
        Me.gpbCCCycle.Controls.Add(Me.lblCCTime01)
        Me.gpbCCCycle.Controls.Add(Me.lblCycleName01)
        resources.ApplyResources(Me.gpbCCCycle, "gpbCCCycle")
        Me.gpbCCCycle.Name = "gpbCCCycle"
        Me.gpbCCCycle.TabStop = False
        '
        'gpbCalibration
        '
        Me.gpbCalibration.BackColor = System.Drawing.Color.Transparent
        Me.gpbCalibration.Controls.Add(Me.btnPressTest)
        Me.gpbCalibration.Controls.Add(Me.btnDQ2Cal)
        Me.gpbCalibration.Controls.Add(Me.btnDQCal)
        Me.gpbCalibration.Controls.Add(Me.btnAutoCal)
        resources.ApplyResources(Me.gpbCalibration, "gpbCalibration")
        Me.gpbCalibration.Name = "gpbCalibration"
        Me.gpbCalibration.TabStop = False
        '
        'btnPressTest
        '
        resources.ApplyResources(Me.btnPressTest, "btnPressTest")
        Me.btnPressTest.Name = "btnPressTest"
        Me.btnPressTest.Tag = "PaintCalRobots"
        Me.btnPressTest.UseVisualStyleBackColor = True
        '
        'btnDQ2Cal
        '
        resources.ApplyResources(Me.btnDQ2Cal, "btnDQ2Cal")
        Me.btnDQ2Cal.Name = "btnDQ2Cal"
        Me.btnDQ2Cal.Tag = "DQ2CalRobots"
        Me.btnDQ2Cal.UseVisualStyleBackColor = True
        '
        'btnDQCal
        '
        resources.ApplyResources(Me.btnDQCal, "btnDQCal")
        Me.btnDQCal.Name = "btnDQCal"
        Me.btnDQCal.Tag = "DQCalRobots"
        Me.btnDQCal.UseVisualStyleBackColor = True
        '
        'btnAutoCal
        '
        resources.ApplyResources(Me.btnAutoCal, "btnAutoCal")
        Me.btnAutoCal.Name = "btnAutoCal"
        Me.btnAutoCal.Tag = "ScaleCalRobots"
        Me.btnAutoCal.UseVisualStyleBackColor = True
        '
        'gpbCCControl
        '
        Me.gpbCCControl.BackColor = System.Drawing.SystemColors.Control
        Me.gpbCCControl.Controls.Add(Me.btnRun)
        Me.gpbCCControl.Controls.Add(Me.cboCycle)
        resources.ApplyResources(Me.gpbCCControl, "gpbCCControl")
        Me.gpbCCControl.Name = "gpbCCControl"
        Me.gpbCCControl.TabStop = False
        '
        'btnRun
        '
        resources.ApplyResources(Me.btnRun, "btnRun")
        Me.btnRun.Name = "btnRun"
        Me.btnRun.UseVisualStyleBackColor = True
        '
        'cboCycle
        '
        Me.cboCycle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboCycle, "cboCycle")
        Me.cboCycle.Name = "cboCycle"
        '
        'lblCCTime10
        '
        resources.ApplyResources(Me.lblCCTime10, "lblCCTime10")
        Me.lblCCTime10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCCTime10.Name = "lblCCTime10"
        '
        'lblCycleName10
        '
        resources.ApplyResources(Me.lblCycleName10, "lblCycleName10")
        Me.lblCycleName10.Name = "lblCycleName10"
        '
        'lblCCTime09
        '
        resources.ApplyResources(Me.lblCCTime09, "lblCCTime09")
        Me.lblCCTime09.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCCTime09.Name = "lblCCTime09"
        '
        'lblCycleName09
        '
        resources.ApplyResources(Me.lblCycleName09, "lblCycleName09")
        Me.lblCycleName09.Name = "lblCycleName09"
        '
        'lblCCTime08
        '
        resources.ApplyResources(Me.lblCCTime08, "lblCCTime08")
        Me.lblCCTime08.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCCTime08.Name = "lblCCTime08"
        '
        'lblCycleName08
        '
        resources.ApplyResources(Me.lblCycleName08, "lblCycleName08")
        Me.lblCycleName08.Name = "lblCycleName08"
        '
        'lblCCTime07
        '
        resources.ApplyResources(Me.lblCCTime07, "lblCCTime07")
        Me.lblCCTime07.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCCTime07.Name = "lblCCTime07"
        '
        'lblCycleName07
        '
        resources.ApplyResources(Me.lblCycleName07, "lblCycleName07")
        Me.lblCycleName07.Name = "lblCycleName07"
        '
        'lblCCTime06
        '
        resources.ApplyResources(Me.lblCCTime06, "lblCCTime06")
        Me.lblCCTime06.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCCTime06.Name = "lblCCTime06"
        '
        'lblCycleName06
        '
        resources.ApplyResources(Me.lblCycleName06, "lblCycleName06")
        Me.lblCycleName06.Name = "lblCycleName06"
        '
        'lblCCTime05
        '
        resources.ApplyResources(Me.lblCCTime05, "lblCCTime05")
        Me.lblCCTime05.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCCTime05.Name = "lblCCTime05"
        '
        'lblCycleName05
        '
        resources.ApplyResources(Me.lblCycleName05, "lblCycleName05")
        Me.lblCycleName05.Name = "lblCycleName05"
        '
        'lblCCTime04
        '
        resources.ApplyResources(Me.lblCCTime04, "lblCCTime04")
        Me.lblCCTime04.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCCTime04.Name = "lblCCTime04"
        '
        'lblCycleName04
        '
        resources.ApplyResources(Me.lblCycleName04, "lblCycleName04")
        Me.lblCycleName04.Name = "lblCycleName04"
        '
        'lblCCTime03
        '
        resources.ApplyResources(Me.lblCCTime03, "lblCCTime03")
        Me.lblCCTime03.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCCTime03.Name = "lblCCTime03"
        '
        'lblCycleName03
        '
        resources.ApplyResources(Me.lblCycleName03, "lblCycleName03")
        Me.lblCycleName03.Name = "lblCycleName03"
        '
        'lblCCTime02
        '
        resources.ApplyResources(Me.lblCCTime02, "lblCCTime02")
        Me.lblCCTime02.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCCTime02.Name = "lblCCTime02"
        '
        'lblCycleName02
        '
        resources.ApplyResources(Me.lblCycleName02, "lblCycleName02")
        Me.lblCycleName02.Name = "lblCycleName02"
        '
        'lblCCTime01
        '
        resources.ApplyResources(Me.lblCCTime01, "lblCCTime01")
        Me.lblCCTime01.BackColor = System.Drawing.Color.Transparent
        Me.lblCCTime01.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCCTime01.Name = "lblCCTime01"
        '
        'lblCycleName01
        '
        resources.ApplyResources(Me.lblCycleName01, "lblCycleName01")
        Me.lblCycleName01.Name = "lblCycleName01"
        '
        'gpbRobots
        '
        Me.gpbRobots.Controls.Add(Me.clbRobot)
        resources.ApplyResources(Me.gpbRobots, "gpbRobots")
        Me.gpbRobots.Name = "gpbRobots"
        Me.gpbRobots.TabStop = False
        '
        'clbRobot
        '
        Me.clbRobot.ContextMenuStrip = Me.mnuRobotList
        Me.clbRobot.FormattingEnabled = True
        resources.ApplyResources(Me.clbRobot, "clbRobot")
        Me.clbRobot.MultiColumn = True
        Me.clbRobot.Name = "clbRobot"
        Me.clbRobot.ThreeDCheckBoxes = True
        Me.clbRobot.UseCompatibleTextRendering = True
        '
        'mnuRobotList
        '
        Me.mnuRobotList.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuSelectAllToolStripMenuItem, Me.mnuUnSelectAllToolStripMenuItem})
        Me.mnuRobotList.Name = "cmnuRobotList"
        resources.ApplyResources(Me.mnuRobotList, "mnuRobotList")
        '
        'mnuSelectAllToolStripMenuItem
        '
        Me.mnuSelectAllToolStripMenuItem.Name = "mnuSelectAllToolStripMenuItem"
        resources.ApplyResources(Me.mnuSelectAllToolStripMenuItem, "mnuSelectAllToolStripMenuItem")
        '
        'mnuUnSelectAllToolStripMenuItem
        '
        Me.mnuUnSelectAllToolStripMenuItem.Name = "mnuUnSelectAllToolStripMenuItem"
        resources.ApplyResources(Me.mnuUnSelectAllToolStripMenuItem, "mnuUnSelectAllToolStripMenuItem")
        '
        'tlsMain
        '
        resources.ApplyResources(Me.tlsMain, "tlsMain")
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.btnSave, Me.btnUndo, Me.btnCopy, Me.btnPrint, Me.btnChangeLog, Me.btnMultiView, Me.btnRestore, Me.btnDevice, Me.btnStatus, Me.btnBeakerMode})
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
        Me.btnPrint.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuPrint, Me.mnuPrintPreview, Me.mnuPageSetup, Me.mnuPrintFile, Me.MnuPrintOptionsToolStripMenuItem})
        resources.ApplyResources(Me.btnPrint, "btnPrint")
        Me.btnPrint.Name = "btnPrint"
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
        'MnuPrintOptionsToolStripMenuItem
        '
        Me.MnuPrintOptionsToolStripMenuItem.Name = "MnuPrintOptionsToolStripMenuItem"
        resources.ApplyResources(Me.MnuPrintOptionsToolStripMenuItem, "MnuPrintOptionsToolStripMenuItem")
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
        'btnRestore
        '
        resources.ApplyResources(Me.btnRestore, "btnRestore")
        Me.btnRestore.Name = "btnRestore"
        '
        'btnDevice
        '
        resources.ApplyResources(Me.btnDevice, "btnDevice")
        Me.btnDevice.Name = "btnDevice"
        '
        'btnStatus
        '
        resources.ApplyResources(Me.btnStatus, "btnStatus")
        Me.btnStatus.Name = "btnStatus"
        '
        'btnBeakerMode
        '
        resources.ApplyResources(Me.btnBeakerMode, "btnBeakerMode")
        Me.btnBeakerMode.Name = "btnBeakerMode"
        '
        'tmrPressureUpdate
        '
        '
        'pnlTxt
        '
        resources.ApplyResources(Me.pnlTxt, "pnlTxt")
        '
        'pnlProg
        '
        resources.ApplyResources(Me.pnlProg, "pnlProg")
        '
        'imlGun
        '
        Me.imlGun.ImageStream = CType(resources.GetObject("imlGun.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlGun.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.imlGun.Images.SetKeyName(0, "off")
        Me.imlGun.Images.SetKeyName(1, "blue_on")
        Me.imlGun.Images.SetKeyName(2, "green_on")
        Me.imlGun.Images.SetKeyName(3, "purple_on")
        Me.imlGun.Images.SetKeyName(4, "red_on")
        Me.imlGun.Images.SetKeyName(5, "blue_off")
        Me.imlGun.Images.SetKeyName(6, "purple_off")
        Me.imlGun.Images.SetKeyName(7, "red_off")
        '
        'imlAppClnr
        '
        Me.imlAppClnr.ImageStream = CType(resources.GetObject("imlAppClnr.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlAppClnr.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.imlAppClnr.Images.SetKeyName(0, "off")
        Me.imlAppClnr.Images.SetKeyName(1, "dump_blow")
        Me.imlAppClnr.Images.SetKeyName(2, "dump_off")
        Me.imlAppClnr.Images.SetKeyName(3, "dump_blue")
        Me.imlAppClnr.Images.SetKeyName(4, "dump_purple")
        Me.imlAppClnr.Images.SetKeyName(5, "dump_red")
        Me.imlAppClnr.Images.SetKeyName(6, "blue")
        Me.imlAppClnr.Images.SetKeyName(7, "red")
        Me.imlAppClnr.Images.SetKeyName(8, "purple")
        Me.imlAppClnr.Images.SetKeyName(9, "down_blue")
        Me.imlAppClnr.Images.SetKeyName(10, "up_blue")
        Me.imlAppClnr.Images.SetKeyName(11, "down_red")
        Me.imlAppClnr.Images.SetKeyName(12, "up_red")
        '
        'imlMisc
        '
        Me.imlMisc.ImageStream = CType(resources.GetObject("imlMisc.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlMisc.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.imlMisc.Images.SetKeyName(0, "can_off")
        Me.imlMisc.Images.SetKeyName(1, "finger")
        Me.imlMisc.Images.SetKeyName(2, "can_on")
        Me.imlMisc.Images.SetKeyName(3, "can_off_blue")
        Me.imlMisc.Images.SetKeyName(4, "can_off_purple")
        Me.imlMisc.Images.SetKeyName(5, "can_off_red")
        Me.imlMisc.Images.SetKeyName(6, "can_off_turb")
        Me.imlMisc.Images.SetKeyName(7, "can_on_blue")
        Me.imlMisc.Images.SetKeyName(8, "can_on_purple")
        Me.imlMisc.Images.SetKeyName(9, "can_on_red")
        Me.imlMisc.Images.SetKeyName(10, "beaker")
        '
        'tmrUpdateSA
        '
        Me.tmrUpdateSA.Interval = 700
        '
        'ToolTipMain
        '
        Me.ToolTipMain.ShowAlways = True
        Me.ToolTipMain.StripAmpersands = True
        '
        'frmMain
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.tscMain)
        Me.Name = "frmMain"
        Me.tscMain.ContentPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.PerformLayout()
        Me.tscMain.ResumeLayout(False)
        Me.tscMain.PerformLayout()
        Me.gpbPumpEnables.ResumeLayout(False)
        Me.gpbPumpEnables.PerformLayout()
        Me.gpbCurrentColor.ResumeLayout(False)
        Me.gpbTPR.ResumeLayout(False)
        Me.gpbTPR.PerformLayout()
        Me.gpbDockControl.ResumeLayout(False)
        Me.gpbViewAll.ResumeLayout(False)
        Me.mnuViewSelect.ResumeLayout(False)
        Me.stsStatus.ResumeLayout(False)
        Me.stsStatus.PerformLayout()
        Me.gpbFlowParams.ResumeLayout(False)
        Me.gpbFlowParams.PerformLayout()
        Me.mnuUnitPopup.ResumeLayout(False)
        Me.pnlFeedBack.ResumeLayout(False)
        Me.pnlFeedBack.PerformLayout()
        Me.gpbCCCycle.ResumeLayout(False)
        Me.gpbCalibration.ResumeLayout(False)
        Me.gpbCCControl.ResumeLayout(False)
        Me.gpbRobots.ResumeLayout(False)
        Me.mnuRobotList.ResumeLayout(False)
        Me.tlsMain.ResumeLayout(False)
        Me.tlsMain.PerformLayout()
        CType(Me.pnlTxt, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pnlProg, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tscMain As System.Windows.Forms.ToolStripContainer
    Friend WithEvents stsStatus As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tspProgress As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents pnlTxt As System.Windows.Forms.StatusBarPanel
    Friend WithEvents pnlProg As System.Windows.Forms.StatusBarPanel
    Friend WithEvents btnFunction As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents mnuLogin As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLogOut As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuRemote As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLocal As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblSpacer As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnClose As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnSave As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnUndo As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnCopy As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnChangeLog As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuLast7 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAllChanges As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnMultiView As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnRestore As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnStatus As System.Windows.Forms.ToolStripButton
    Friend WithEvents mnuLast24 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents cboColor As System.Windows.Forms.ComboBox
    Friend WithEvents lblParamOp As System.Windows.Forms.Label
    Friend WithEvents lblRobotView As System.Windows.Forms.Label
    Friend WithEvents lblZone As System.Windows.Forms.Label
    Friend WithEvents cboRobot As System.Windows.Forms.ComboBox
    Friend WithEvents cboZone As System.Windows.Forms.ComboBox
    Friend WithEvents gpbRobots As System.Windows.Forms.GroupBox
    Friend WithEvents clbRobot As System.Windows.Forms.CheckedListBox
    Friend WithEvents gpbFlowParams As System.Windows.Forms.GroupBox
    Friend WithEvents lblFlowCap As System.Windows.Forms.Label
    Friend WithEvents btnFlowTest As System.Windows.Forms.Button
    Friend WithEvents gpbCCCycle As System.Windows.Forms.GroupBox
    Friend WithEvents lblAtomCap As System.Windows.Forms.Label
    Friend WithEvents lblTimeCap As System.Windows.Forms.Label
    Friend WithEvents lblEstatCap As System.Windows.Forms.Label
    Friend WithEvents lblFanCap As System.Windows.Forms.Label
    Friend WithEvents ftbFlow As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblValuesCap As System.Windows.Forms.Label
    Friend WithEvents lblTotalCap As System.Windows.Forms.Label
    Friend WithEvents lblActualCap As System.Windows.Forms.Label
    Friend WithEvents lblCCTime01 As System.Windows.Forms.Label
    Friend WithEvents lblCycleName01 As System.Windows.Forms.Label
    Friend WithEvents lblCCTime10 As System.Windows.Forms.Label
    Friend WithEvents lblCycleName10 As System.Windows.Forms.Label
    Friend WithEvents lblCCTime09 As System.Windows.Forms.Label
    Friend WithEvents lblCycleName09 As System.Windows.Forms.Label
    Friend WithEvents lblCCTime08 As System.Windows.Forms.Label
    Friend WithEvents lblCycleName08 As System.Windows.Forms.Label
    Friend WithEvents lblCCTime07 As System.Windows.Forms.Label
    Friend WithEvents lblCycleName07 As System.Windows.Forms.Label
    Friend WithEvents lblCCTime06 As System.Windows.Forms.Label
    Friend WithEvents lblCycleName06 As System.Windows.Forms.Label
    Friend WithEvents lblCCTime05 As System.Windows.Forms.Label
    Friend WithEvents lblCycleName05 As System.Windows.Forms.Label
    Friend WithEvents lblCCTime04 As System.Windows.Forms.Label
    Friend WithEvents lblCycleName04 As System.Windows.Forms.Label
    Friend WithEvents lblCCTime03 As System.Windows.Forms.Label
    Friend WithEvents lblCycleName03 As System.Windows.Forms.Label
    Friend WithEvents lblCCTime02 As System.Windows.Forms.Label
    Friend WithEvents lblCycleName02 As System.Windows.Forms.Label
    Friend WithEvents imlGun As System.Windows.Forms.ImageList
    Friend WithEvents imlAppClnr As System.Windows.Forms.ImageList
    Friend WithEvents imlMisc As System.Windows.Forms.ImageList
    Friend WithEvents tmrUpdateSA As System.Windows.Forms.Timer
    Friend WithEvents gpbCCControl As System.Windows.Forms.GroupBox
    Friend WithEvents btnRun As System.Windows.Forms.Button
    Friend WithEvents cboCycle As System.Windows.Forms.ComboBox
    Friend WithEvents proFlow As System.Windows.Forms.ProgressBar
    Friend WithEvents ftbTime As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbEStat As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbFan As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbAtom As FocusedTextBox.FocusedTextBox
    Friend WithEvents pnlFeedBack As System.Windows.Forms.Panel
    Friend WithEvents lblUACap As System.Windows.Forms.Label
    Friend WithEvents ftbActualEstatUA As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbTotal As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbActualTime As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbActualEStatKv As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbActualFan As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbActualAtom As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbActualFlow As FocusedTextBox.FocusedTextBox
    Friend WithEvents btnDevice As System.Windows.Forms.ToolStripButton
    Friend WithEvents cboApplicator As System.Windows.Forms.ComboBox
    Friend WithEvents lblApplicator As System.Windows.Forms.Label
    Friend WithEvents lblColor As System.Windows.Forms.Label
    Friend WithEvents lblKVCap As System.Windows.Forms.Label
    Friend WithEvents mnuSelectAllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuUnSelectAllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuRobotList As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents lblFanFBCap As System.Windows.Forms.Label
    Friend WithEvents lblAtomFBCap As System.Windows.Forms.Label
    Friend WithEvents lblCylPos As System.Windows.Forms.Label
    Friend WithEvents lblCylPosCap As System.Windows.Forms.Label
    Friend WithEvents gpbCalibration As System.Windows.Forms.GroupBox
    Friend WithEvents btnDQCal As System.Windows.Forms.Button
    Friend WithEvents btnPressTest As System.Windows.Forms.Button
    Friend WithEvents btnAutoCal As System.Windows.Forms.Button
    Friend WithEvents mnuUnitPopup As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuSelectEngUnits As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSelectCounts As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents gpbViewAll As System.Windows.Forms.GroupBox
    Friend WithEvents lblCalStat10 As System.Windows.Forms.Label
    Friend WithEvents lblCalStat09 As System.Windows.Forms.Label
    Friend WithEvents lblCalStat08 As System.Windows.Forms.Label
    Friend WithEvents lblCalStat07 As System.Windows.Forms.Label
    Friend WithEvents lblCalStat06 As System.Windows.Forms.Label
    Friend WithEvents lblCalStat05 As System.Windows.Forms.Label
    Friend WithEvents lblCalStat04 As System.Windows.Forms.Label
    Friend WithEvents lblCalStat03 As System.Windows.Forms.Label
    Friend WithEvents lblCalStat02 As System.Windows.Forms.Label
    Friend WithEvents lblCalStat01 As System.Windows.Forms.Label
    Friend WithEvents lblRobot10 As System.Windows.Forms.Label
    Friend WithEvents lblRobot09 As System.Windows.Forms.Label
    Friend WithEvents lblRobot08 As System.Windows.Forms.Label
    Friend WithEvents lblRobot07 As System.Windows.Forms.Label
    Friend WithEvents lblRobot06 As System.Windows.Forms.Label
    Friend WithEvents lblRobot05 As System.Windows.Forms.Label
    Friend WithEvents lblRobot04 As System.Windows.Forms.Label
    Friend WithEvents lblRobot03 As System.Windows.Forms.Label
    Friend WithEvents lblRobot02 As System.Windows.Forms.Label
    Friend WithEvents lblRobot01 As System.Windows.Forms.Label
    Friend WithEvents lblStatACap As System.Windows.Forms.Label
    Friend WithEvents lblCalStatCap As System.Windows.Forms.Label
    Friend WithEvents lblStatB10 As System.Windows.Forms.Label
    Friend WithEvents lblStatB09 As System.Windows.Forms.Label
    Friend WithEvents lblStatB08 As System.Windows.Forms.Label
    Friend WithEvents lblStatB07 As System.Windows.Forms.Label
    Friend WithEvents lblStatB06 As System.Windows.Forms.Label
    Friend WithEvents lblStatB05 As System.Windows.Forms.Label
    Friend WithEvents lblStatB04 As System.Windows.Forms.Label
    Friend WithEvents lblStatB03 As System.Windows.Forms.Label
    Friend WithEvents lblStatB02 As System.Windows.Forms.Label
    Friend WithEvents lblStatB01 As System.Windows.Forms.Label
    Friend WithEvents lblStatA10 As System.Windows.Forms.Label
    Friend WithEvents lblStatA09 As System.Windows.Forms.Label
    Friend WithEvents lblStatA08 As System.Windows.Forms.Label
    Friend WithEvents lblStatA07 As System.Windows.Forms.Label
    Friend WithEvents lblStatA06 As System.Windows.Forms.Label
    Friend WithEvents lblStatA05 As System.Windows.Forms.Label
    Friend WithEvents lblStatA04 As System.Windows.Forms.Label
    Friend WithEvents lblStatA03 As System.Windows.Forms.Label
    Friend WithEvents lblStatA02 As System.Windows.Forms.Label
    Friend WithEvents lblStatA01 As System.Windows.Forms.Label
    Friend WithEvents lblStatBCap As System.Windows.Forms.Label
    Friend WithEvents lblStatB11 As System.Windows.Forms.Label
    Friend WithEvents lblStatA11 As System.Windows.Forms.Label
    Friend WithEvents lblCalStat11 As System.Windows.Forms.Label
    Friend WithEvents lblRobot11 As System.Windows.Forms.Label
    Friend WithEvents lblStatB12 As System.Windows.Forms.Label
    Friend WithEvents lblStatA12 As System.Windows.Forms.Label
    Friend WithEvents lblCalStat12 As System.Windows.Forms.Label
    Friend WithEvents lblRobot12 As System.Windows.Forms.Label
    Friend WithEvents mnuViewSelect As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuViewSelectToon As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewSelectPaint As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewSelectAA_BS As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewSelectFA_SA As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewSelectEstat As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewSelectCalStat As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuViewSelectDQCalStat As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnBeakerMode As System.Windows.Forms.ToolStripButton
    Friend WithEvents lstStatus As System.Windows.Forms.ListBox
    Friend WithEvents ToolTipMain As System.Windows.Forms.ToolTip
    Friend WithEvents lblCounts As System.Windows.Forms.Label
    Friend WithEvents lblStatC12 As System.Windows.Forms.Label
    Friend WithEvents lblStatC11 As System.Windows.Forms.Label
    Friend WithEvents lblStatC10 As System.Windows.Forms.Label
    Friend WithEvents lblStatC09 As System.Windows.Forms.Label
    Friend WithEvents lblStatC08 As System.Windows.Forms.Label
    Friend WithEvents lblStatC07 As System.Windows.Forms.Label
    Friend WithEvents lblStatC06 As System.Windows.Forms.Label
    Friend WithEvents lblStatC05 As System.Windows.Forms.Label
    Friend WithEvents lblStatC04 As System.Windows.Forms.Label
    Friend WithEvents lblStatC03 As System.Windows.Forms.Label
    Friend WithEvents lblStatC02 As System.Windows.Forms.Label
    Friend WithEvents lblStatC01 As System.Windows.Forms.Label
    Friend WithEvents lblStatCCap As System.Windows.Forms.Label
    Friend WithEvents ftbFan2 As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbActualFan2 As FocusedTextBox.FocusedTextBox
    Friend WithEvents btnDQ2Cal As System.Windows.Forms.Button
    Friend WithEvents lblFan2Cap As System.Windows.Forms.Label
    Friend WithEvents lblFan2ActCap As System.Windows.Forms.Label
    Friend WithEvents lblFan1Cap As System.Windows.Forms.Label
    Friend WithEvents mnuViewSelectPressureTest As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tmrPressureUpdate As System.Windows.Forms.Timer
    Friend WithEvents gpbDockControl As System.Windows.Forms.GroupBox
    Friend WithEvents btnMoveDeDock As System.Windows.Forms.Button
    Friend WithEvents btnMoveClean As System.Windows.Forms.Button
    Friend WithEvents btnMoveDock As System.Windows.Forms.Button
    Friend WithEvents gpbTPR As System.Windows.Forms.GroupBox
    Friend WithEvents txtTPR As FocusedTextBox.FocusedTextBox
    Friend WithEvents mnuViewSelectDockCalStat As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents gpbCurrentColor As System.Windows.Forms.GroupBox
    Friend WithEvents lblCurrentColor As System.Windows.Forms.Label
    Friend WithEvents mnuViewSelectDQCalStat2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnPrint As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuPrint As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintPreview As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPageSetup As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPrintOptionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblHintAllRobots As System.Windows.Forms.Label
    Friend WithEvents lblRobot14 As System.Windows.Forms.Label
    Friend WithEvents lblCalStat13 As System.Windows.Forms.Label
    Friend WithEvents lblRobot13 As System.Windows.Forms.Label
    Friend WithEvents lblStatC13 As System.Windows.Forms.Label
    Friend WithEvents lblStatB14 As System.Windows.Forms.Label
    Friend WithEvents lblStatB13 As System.Windows.Forms.Label
    Friend WithEvents lblStatA14 As System.Windows.Forms.Label
    Friend WithEvents lblStatA13 As System.Windows.Forms.Label
    Friend WithEvents lblCalStat14 As System.Windows.Forms.Label
    Friend WithEvents lblStatC14 As System.Windows.Forms.Label
    Friend WithEvents lblRobot15 As System.Windows.Forms.Label
    Friend WithEvents lblCalStat16 As System.Windows.Forms.Label
    Friend WithEvents lblCalStat15 As System.Windows.Forms.Label
    Friend WithEvents lblRobot16 As System.Windows.Forms.Label
    Friend WithEvents lblStatB16 As System.Windows.Forms.Label
    Friend WithEvents lblStatB15 As System.Windows.Forms.Label
    Friend WithEvents lblStatA16 As System.Windows.Forms.Label
    Friend WithEvents lblStatA15 As System.Windows.Forms.Label
    Friend WithEvents lblStatC16 As System.Windows.Forms.Label
    Friend WithEvents lblStatC15 As System.Windows.Forms.Label
    Friend WithEvents gpbPumpEnables As System.Windows.Forms.GroupBox
    Friend WithEvents chkPump2 As System.Windows.Forms.CheckBox
    Friend WithEvents chkPump1 As System.Windows.Forms.CheckBox
End Class
