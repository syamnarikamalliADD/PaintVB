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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.tscMain = New System.Windows.Forms.ToolStripContainer
        Me.stsStatus = New System.Windows.Forms.StatusStrip
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel
        Me.tspProgress = New System.Windows.Forms.ToolStripProgressBar
        Me.lblSpacer = New System.Windows.Forms.ToolStripStatusLabel
        Me.btnFunction = New System.Windows.Forms.ToolStripDropDownButton
        Me.mnuLogin = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLogOut = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuRemote = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLocal = New System.Windows.Forms.ToolStripMenuItem
        Me.pnlMain = New System.Windows.Forms.Panel
        Me.tabMain = New System.Windows.Forms.TabControl
        Me.TabPage1 = New System.Windows.Forms.TabPage
        Me.lblTwoCoats = New System.Windows.Forms.Label
        Me.pnlTab1 = New System.Windows.Forms.Panel
        Me.ftbTwoCoat001 = New System.Windows.Forms.CheckBox
        Me.lblFanucColor001 = New System.Windows.Forms.Label
        Me.ftbPlantColor001 = New FocusedTextBox.FocusedTextBox
        Me.ftbColorDesc001 = New FocusedTextBox.FocusedTextBox
        Me.ftbValveNumber001 = New FocusedTextBox.FocusedTextBox
        Me.ftbValveDesc001 = New FocusedTextBox.FocusedTextBox
        Me.lblColor001 = New System.Windows.Forms.Label
        Me.lblValveDescCap = New System.Windows.Forms.Label
        Me.lblColorDescCap = New System.Windows.Forms.Label
        Me.lblFanucColorCap = New System.Windows.Forms.Label
        Me.lblValveNumberCap = New System.Windows.Forms.Label
        Me.lblPlantColorCap = New System.Windows.Forms.Label
        Me.TabPage2 = New System.Windows.Forms.TabPage
        Me.pnlTab2 = New System.Windows.Forms.Panel
        Me.clbRobotsReq001 = New System.Windows.Forms.CheckedListBox
        Me.lblColorDesc001 = New System.Windows.Forms.Label
        Me.lblPlantColor001 = New System.Windows.Forms.Label
        Me.lblColorDesc2Cap = New System.Windows.Forms.Label
        Me.lblRobotReqCap = New System.Windows.Forms.Label
        Me.lblPlantColor2Cap = New System.Windows.Forms.Label
        Me.TabPage3 = New System.Windows.Forms.TabPage
        Me.ftbHardSolv001 = New FocusedTextBox.FocusedTextBox
        Me.ftbResinSolv001 = New FocusedTextBox.FocusedTextBox
        Me.lblPlantColorT3001 = New System.Windows.Forms.Label
        Me.lblColorDescT3001 = New System.Windows.Forms.Label
        Me.lblColon001 = New System.Windows.Forms.Label
        Me.lblHardSolvCap = New System.Windows.Forms.Label
        Me.ftbHardRatio001 = New FocusedTextBox.FocusedTextBox
        Me.lblResinSolvCap = New System.Windows.Forms.Label
        Me.ftbHardValve001 = New FocusedTextBox.FocusedTextBox
        Me.ftbResinRatio001 = New FocusedTextBox.FocusedTextBox
        Me.lblHardenerRatioCap = New System.Windows.Forms.Label
        Me.lblResinRatioCap = New System.Windows.Forms.Label
        Me.lblHardenerCap = New System.Windows.Forms.Label
        Me.pnlTab3 = New System.Windows.Forms.Panel
        Me.lblColorDesc3Cap = New System.Windows.Forms.Label
        Me.lblPlantColor3Cap = New System.Windows.Forms.Label
        Me.TabPage4 = New System.Windows.Forms.TabPage
        Me.pnlTab4 = New System.Windows.Forms.Panel
        Me.ftbTricoatColor001 = New FocusedTextBox.FocusedTextBox
        Me.lblTab4ColorDesc001 = New System.Windows.Forms.Label
        Me.lblTab4PlantColor001 = New System.Windows.Forms.Label
        Me.lblColorDesc4Cap = New System.Windows.Forms.Label
        Me.lblTricoat4Cap = New System.Windows.Forms.Label
        Me.lblPlantColor4Cap = New System.Windows.Forms.Label
        Me.TabPage5 = New System.Windows.Forms.TabPage
        Me.pnlTab5 = New System.Windows.Forms.Panel
        Me.lblSysNum001 = New System.Windows.Forms.Label
        Me.cboValveDesc001 = New System.Windows.Forms.ComboBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.cboParam = New System.Windows.Forms.ComboBox
        Me.lblParam = New System.Windows.Forms.Label
        Me.lblRobot = New System.Windows.Forms.Label
        Me.lblZone = New System.Windows.Forms.Label
        Me.cboRobot = New System.Windows.Forms.ComboBox
        Me.cboZone = New System.Windows.Forms.ComboBox
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.btnSave = New System.Windows.Forms.ToolStripSplitButton
        Me.btnUndo = New System.Windows.Forms.ToolStripButton
        Me.btnCopy = New System.Windows.Forms.ToolStripButton
        Me.btnPrint = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuPrint = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintPreview = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPageSetup = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintFile = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintOptions = New System.Windows.Forms.ToolStripMenuItem
        Me.btnChangeLog = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuLast24 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLast7 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAllChanges = New System.Windows.Forms.ToolStripMenuItem
        Me.btnUpload = New System.Windows.Forms.ToolStripDropDownButton
        Me.btnMultiView = New System.Windows.Forms.ToolStripButton
        Me.btnStatus = New System.Windows.Forms.ToolStripButton
        Me.btnUtilities = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuExport = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuImport = New System.Windows.Forms.ToolStripMenuItem
        Me.pnlTxt = New System.Windows.Forms.StatusBarPanel
        Me.pnlProg = New System.Windows.Forms.StatusBarPanel
        Me.pnlTab5crap = New System.Windows.Forms.Panel
        Me.lblSysNum001crap = New System.Windows.Forms.Label
        Me.cboValveDesc001crap = New System.Windows.Forms.ComboBox
        Me.lblValveDesc = New System.Windows.Forms.Label
        Me.lblSystemDesc = New System.Windows.Forms.Label
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.Label1 = New System.Windows.Forms.Label
        Me.ComboBox1 = New System.Windows.Forms.ComboBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.tscMain.BottomToolStripPanel.SuspendLayout()
        Me.tscMain.ContentPanel.SuspendLayout()
        Me.tscMain.TopToolStripPanel.SuspendLayout()
        Me.tscMain.SuspendLayout()
        Me.stsStatus.SuspendLayout()
        Me.pnlMain.SuspendLayout()
        Me.tabMain.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.pnlTab1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.pnlTab2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.pnlTab4.SuspendLayout()
        Me.TabPage5.SuspendLayout()
        Me.pnlTab5.SuspendLayout()
        Me.tlsMain.SuspendLayout()
        CType(Me.pnlTxt, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pnlProg, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlTab5crap.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tscMain
        '
        '
        'tscMain.BottomToolStripPanel
        '
        Me.tscMain.BottomToolStripPanel.Controls.Add(Me.stsStatus)
        '
        'tscMain.ContentPanel
        '
        Me.tscMain.ContentPanel.Controls.Add(Me.pnlMain)
        Me.tscMain.ContentPanel.Controls.Add(Me.lstStatus)
        Me.tscMain.ContentPanel.Controls.Add(Me.cboParam)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblParam)
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
        'pnlMain
        '
        resources.ApplyResources(Me.pnlMain, "pnlMain")
        Me.pnlMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.pnlMain.Controls.Add(Me.tabMain)
        Me.pnlMain.Name = "pnlMain"
        '
        'tabMain
        '
        Me.tabMain.Controls.Add(Me.TabPage1)
        Me.tabMain.Controls.Add(Me.TabPage2)
        Me.tabMain.Controls.Add(Me.TabPage3)
        Me.tabMain.Controls.Add(Me.TabPage4)
        Me.tabMain.Controls.Add(Me.TabPage5)
        resources.ApplyResources(Me.tabMain, "tabMain")
        Me.tabMain.Name = "tabMain"
        Me.tabMain.SelectedIndex = 0
        '
        'TabPage1
        '
        resources.ApplyResources(Me.TabPage1, "TabPage1")
        Me.TabPage1.BackColor = System.Drawing.SystemColors.Control
        Me.TabPage1.Controls.Add(Me.lblTwoCoats)
        Me.TabPage1.Controls.Add(Me.pnlTab1)
        Me.TabPage1.Controls.Add(Me.lblValveDescCap)
        Me.TabPage1.Controls.Add(Me.lblColorDescCap)
        Me.TabPage1.Controls.Add(Me.lblFanucColorCap)
        Me.TabPage1.Controls.Add(Me.lblValveNumberCap)
        Me.TabPage1.Controls.Add(Me.lblPlantColorCap)
        Me.TabPage1.Name = "TabPage1"
        '
        'lblTwoCoats
        '
        Me.lblTwoCoats.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblTwoCoats, "lblTwoCoats")
        Me.lblTwoCoats.Name = "lblTwoCoats"
        '
        'pnlTab1
        '
        resources.ApplyResources(Me.pnlTab1, "pnlTab1")
        Me.pnlTab1.Controls.Add(Me.ftbTwoCoat001)
        Me.pnlTab1.Controls.Add(Me.lblFanucColor001)
        Me.pnlTab1.Controls.Add(Me.ftbPlantColor001)
        Me.pnlTab1.Controls.Add(Me.ftbColorDesc001)
        Me.pnlTab1.Controls.Add(Me.ftbValveNumber001)
        Me.pnlTab1.Controls.Add(Me.ftbValveDesc001)
        Me.pnlTab1.Controls.Add(Me.lblColor001)
        Me.pnlTab1.Name = "pnlTab1"
        Me.pnlTab1.TabStop = True
        '
        'ftbTwoCoat001
        '
        resources.ApplyResources(Me.ftbTwoCoat001, "ftbTwoCoat001")
        Me.ftbTwoCoat001.ForeColor = System.Drawing.Color.Black
        Me.ftbTwoCoat001.Name = "ftbTwoCoat001"
        Me.ftbTwoCoat001.UseVisualStyleBackColor = True
        '
        'lblFanucColor001
        '
        Me.lblFanucColor001.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblFanucColor001.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblFanucColor001, "lblFanucColor001")
        Me.lblFanucColor001.Name = "lblFanucColor001"
        '
        'ftbPlantColor001
        '
        resources.ApplyResources(Me.ftbPlantColor001, "ftbPlantColor001")
        Me.ftbPlantColor001.ForeColor = System.Drawing.Color.Red
        Me.ftbPlantColor001.Name = "ftbPlantColor001"
        Me.ftbPlantColor001.NumericOnly = False
        Me.ftbPlantColor001.TabStop = False
        '
        'ftbColorDesc001
        '
        resources.ApplyResources(Me.ftbColorDesc001, "ftbColorDesc001")
        Me.ftbColorDesc001.ForeColor = System.Drawing.Color.Red
        Me.ftbColorDesc001.Name = "ftbColorDesc001"
        Me.ftbColorDesc001.NumericOnly = False
        Me.ftbColorDesc001.TabStop = False
        '
        'ftbValveNumber001
        '
        resources.ApplyResources(Me.ftbValveNumber001, "ftbValveNumber001")
        Me.ftbValveNumber001.ForeColor = System.Drawing.Color.Red
        Me.ftbValveNumber001.Name = "ftbValveNumber001"
        Me.ftbValveNumber001.NumericOnly = False
        Me.ftbValveNumber001.TabStop = False
        '
        'ftbValveDesc001
        '
        resources.ApplyResources(Me.ftbValveDesc001, "ftbValveDesc001")
        Me.ftbValveDesc001.ForeColor = System.Drawing.Color.Red
        Me.ftbValveDesc001.Name = "ftbValveDesc001"
        Me.ftbValveDesc001.NumericOnly = False
        Me.ftbValveDesc001.TabStop = False
        '
        'lblColor001
        '
        Me.lblColor001.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblColor001, "lblColor001")
        Me.lblColor001.Name = "lblColor001"
        '
        'lblValveDescCap
        '
        Me.lblValveDescCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblValveDescCap, "lblValveDescCap")
        Me.lblValveDescCap.Name = "lblValveDescCap"
        '
        'lblColorDescCap
        '
        Me.lblColorDescCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblColorDescCap, "lblColorDescCap")
        Me.lblColorDescCap.Name = "lblColorDescCap"
        '
        'lblFanucColorCap
        '
        Me.lblFanucColorCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblFanucColorCap, "lblFanucColorCap")
        Me.lblFanucColorCap.Name = "lblFanucColorCap"
        '
        'lblValveNumberCap
        '
        Me.lblValveNumberCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblValveNumberCap, "lblValveNumberCap")
        Me.lblValveNumberCap.Name = "lblValveNumberCap"
        '
        'lblPlantColorCap
        '
        Me.lblPlantColorCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblPlantColorCap, "lblPlantColorCap")
        Me.lblPlantColorCap.Name = "lblPlantColorCap"
        '
        'TabPage2
        '
        Me.TabPage2.BackColor = System.Drawing.SystemColors.Control
        Me.TabPage2.Controls.Add(Me.pnlTab2)
        Me.TabPage2.Controls.Add(Me.lblColorDesc2Cap)
        Me.TabPage2.Controls.Add(Me.lblRobotReqCap)
        Me.TabPage2.Controls.Add(Me.lblPlantColor2Cap)
        resources.ApplyResources(Me.TabPage2, "TabPage2")
        Me.TabPage2.Name = "TabPage2"
        '
        'pnlTab2
        '
        resources.ApplyResources(Me.pnlTab2, "pnlTab2")
        Me.pnlTab2.Controls.Add(Me.clbRobotsReq001)
        Me.pnlTab2.Controls.Add(Me.lblColorDesc001)
        Me.pnlTab2.Controls.Add(Me.lblPlantColor001)
        Me.pnlTab2.Name = "pnlTab2"
        Me.pnlTab2.TabStop = True
        '
        'clbRobotsReq001
        '
        resources.ApplyResources(Me.clbRobotsReq001, "clbRobotsReq001")
        Me.clbRobotsReq001.Items.AddRange(New Object() {resources.GetString("clbRobotsReq001.Items"), resources.GetString("clbRobotsReq001.Items1"), resources.GetString("clbRobotsReq001.Items2"), resources.GetString("clbRobotsReq001.Items3"), resources.GetString("clbRobotsReq001.Items4"), resources.GetString("clbRobotsReq001.Items5")})
        Me.clbRobotsReq001.MultiColumn = True
        Me.clbRobotsReq001.Name = "clbRobotsReq001"
        '
        'lblColorDesc001
        '
        Me.lblColorDesc001.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblColorDesc001.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblColorDesc001, "lblColorDesc001")
        Me.lblColorDesc001.Name = "lblColorDesc001"
        '
        'lblPlantColor001
        '
        Me.lblPlantColor001.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblPlantColor001.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblPlantColor001, "lblPlantColor001")
        Me.lblPlantColor001.Name = "lblPlantColor001"
        '
        'lblColorDesc2Cap
        '
        Me.lblColorDesc2Cap.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblColorDesc2Cap, "lblColorDesc2Cap")
        Me.lblColorDesc2Cap.Name = "lblColorDesc2Cap"
        '
        'lblRobotReqCap
        '
        Me.lblRobotReqCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblRobotReqCap, "lblRobotReqCap")
        Me.lblRobotReqCap.Name = "lblRobotReqCap"
        '
        'lblPlantColor2Cap
        '
        Me.lblPlantColor2Cap.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblPlantColor2Cap, "lblPlantColor2Cap")
        Me.lblPlantColor2Cap.Name = "lblPlantColor2Cap"
        '
        'TabPage3
        '
        Me.TabPage3.BackColor = System.Drawing.SystemColors.Control
        Me.TabPage3.Controls.Add(Me.ftbHardSolv001)
        Me.TabPage3.Controls.Add(Me.ftbResinSolv001)
        Me.TabPage3.Controls.Add(Me.lblPlantColorT3001)
        Me.TabPage3.Controls.Add(Me.lblColorDescT3001)
        Me.TabPage3.Controls.Add(Me.lblColon001)
        Me.TabPage3.Controls.Add(Me.lblHardSolvCap)
        Me.TabPage3.Controls.Add(Me.ftbHardRatio001)
        Me.TabPage3.Controls.Add(Me.lblResinSolvCap)
        Me.TabPage3.Controls.Add(Me.ftbHardValve001)
        Me.TabPage3.Controls.Add(Me.ftbResinRatio001)
        Me.TabPage3.Controls.Add(Me.lblHardenerRatioCap)
        Me.TabPage3.Controls.Add(Me.lblResinRatioCap)
        Me.TabPage3.Controls.Add(Me.lblHardenerCap)
        Me.TabPage3.Controls.Add(Me.pnlTab3)
        Me.TabPage3.Controls.Add(Me.lblColorDesc3Cap)
        Me.TabPage3.Controls.Add(Me.lblPlantColor3Cap)
        resources.ApplyResources(Me.TabPage3, "TabPage3")
        Me.TabPage3.Name = "TabPage3"
        '
        'ftbHardSolv001
        '
        resources.ApplyResources(Me.ftbHardSolv001, "ftbHardSolv001")
        Me.ftbHardSolv001.ForeColor = System.Drawing.Color.Red
        Me.ftbHardSolv001.Name = "ftbHardSolv001"
        Me.ftbHardSolv001.NumericOnly = False
        Me.ftbHardSolv001.TabStop = False
        '
        'ftbResinSolv001
        '
        resources.ApplyResources(Me.ftbResinSolv001, "ftbResinSolv001")
        Me.ftbResinSolv001.ForeColor = System.Drawing.Color.Red
        Me.ftbResinSolv001.Name = "ftbResinSolv001"
        Me.ftbResinSolv001.NumericOnly = False
        Me.ftbResinSolv001.TabStop = False
        '
        'lblPlantColorT3001
        '
        Me.lblPlantColorT3001.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblPlantColorT3001.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblPlantColorT3001, "lblPlantColorT3001")
        Me.lblPlantColorT3001.Name = "lblPlantColorT3001"
        '
        'lblColorDescT3001
        '
        Me.lblColorDescT3001.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblColorDescT3001.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblColorDescT3001, "lblColorDescT3001")
        Me.lblColorDescT3001.Name = "lblColorDescT3001"
        '
        'lblColon001
        '
        resources.ApplyResources(Me.lblColon001, "lblColon001")
        Me.lblColon001.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblColon001.Name = "lblColon001"
        '
        'lblHardSolvCap
        '
        Me.lblHardSolvCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblHardSolvCap, "lblHardSolvCap")
        Me.lblHardSolvCap.Name = "lblHardSolvCap"
        '
        'ftbHardRatio001
        '
        resources.ApplyResources(Me.ftbHardRatio001, "ftbHardRatio001")
        Me.ftbHardRatio001.ForeColor = System.Drawing.Color.Red
        Me.ftbHardRatio001.Name = "ftbHardRatio001"
        Me.ftbHardRatio001.NumericOnly = False
        Me.ftbHardRatio001.TabStop = False
        '
        'lblResinSolvCap
        '
        Me.lblResinSolvCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblResinSolvCap, "lblResinSolvCap")
        Me.lblResinSolvCap.Name = "lblResinSolvCap"
        '
        'ftbHardValve001
        '
        resources.ApplyResources(Me.ftbHardValve001, "ftbHardValve001")
        Me.ftbHardValve001.ForeColor = System.Drawing.Color.Red
        Me.ftbHardValve001.Name = "ftbHardValve001"
        Me.ftbHardValve001.NumericOnly = False
        Me.ftbHardValve001.TabStop = False
        '
        'ftbResinRatio001
        '
        resources.ApplyResources(Me.ftbResinRatio001, "ftbResinRatio001")
        Me.ftbResinRatio001.ForeColor = System.Drawing.Color.Red
        Me.ftbResinRatio001.Name = "ftbResinRatio001"
        Me.ftbResinRatio001.NumericOnly = False
        Me.ftbResinRatio001.TabStop = False
        '
        'lblHardenerRatioCap
        '
        Me.lblHardenerRatioCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblHardenerRatioCap, "lblHardenerRatioCap")
        Me.lblHardenerRatioCap.Name = "lblHardenerRatioCap"
        '
        'lblResinRatioCap
        '
        Me.lblResinRatioCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblResinRatioCap, "lblResinRatioCap")
        Me.lblResinRatioCap.Name = "lblResinRatioCap"
        '
        'lblHardenerCap
        '
        Me.lblHardenerCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblHardenerCap, "lblHardenerCap")
        Me.lblHardenerCap.Name = "lblHardenerCap"
        '
        'pnlTab3
        '
        resources.ApplyResources(Me.pnlTab3, "pnlTab3")
        Me.pnlTab3.Name = "pnlTab3"
        Me.pnlTab3.TabStop = True
        '
        'lblColorDesc3Cap
        '
        Me.lblColorDesc3Cap.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblColorDesc3Cap, "lblColorDesc3Cap")
        Me.lblColorDesc3Cap.Name = "lblColorDesc3Cap"
        '
        'lblPlantColor3Cap
        '
        Me.lblPlantColor3Cap.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblPlantColor3Cap, "lblPlantColor3Cap")
        Me.lblPlantColor3Cap.Name = "lblPlantColor3Cap"
        '
        'TabPage4
        '
        Me.TabPage4.BackColor = System.Drawing.SystemColors.Control
        Me.TabPage4.Controls.Add(Me.pnlTab4)
        Me.TabPage4.Controls.Add(Me.lblColorDesc4Cap)
        Me.TabPage4.Controls.Add(Me.lblTricoat4Cap)
        Me.TabPage4.Controls.Add(Me.lblPlantColor4Cap)
        resources.ApplyResources(Me.TabPage4, "TabPage4")
        Me.TabPage4.Name = "TabPage4"
        '
        'pnlTab4
        '
        resources.ApplyResources(Me.pnlTab4, "pnlTab4")
        Me.pnlTab4.Controls.Add(Me.ftbTricoatColor001)
        Me.pnlTab4.Controls.Add(Me.lblTab4ColorDesc001)
        Me.pnlTab4.Controls.Add(Me.lblTab4PlantColor001)
        Me.pnlTab4.Name = "pnlTab4"
        Me.pnlTab4.TabStop = True
        '
        'ftbTricoatColor001
        '
        resources.ApplyResources(Me.ftbTricoatColor001, "ftbTricoatColor001")
        Me.ftbTricoatColor001.ForeColor = System.Drawing.Color.Red
        Me.ftbTricoatColor001.Name = "ftbTricoatColor001"
        Me.ftbTricoatColor001.NumericOnly = False
        Me.ftbTricoatColor001.TabStop = False
        '
        'lblTab4ColorDesc001
        '
        Me.lblTab4ColorDesc001.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblTab4ColorDesc001.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblTab4ColorDesc001, "lblTab4ColorDesc001")
        Me.lblTab4ColorDesc001.Name = "lblTab4ColorDesc001"
        '
        'lblTab4PlantColor001
        '
        Me.lblTab4PlantColor001.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblTab4PlantColor001.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblTab4PlantColor001, "lblTab4PlantColor001")
        Me.lblTab4PlantColor001.Name = "lblTab4PlantColor001"
        '
        'lblColorDesc4Cap
        '
        Me.lblColorDesc4Cap.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblColorDesc4Cap, "lblColorDesc4Cap")
        Me.lblColorDesc4Cap.Name = "lblColorDesc4Cap"
        '
        'lblTricoat4Cap
        '
        Me.lblTricoat4Cap.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblTricoat4Cap, "lblTricoat4Cap")
        Me.lblTricoat4Cap.Name = "lblTricoat4Cap"
        '
        'lblPlantColor4Cap
        '
        Me.lblPlantColor4Cap.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.lblPlantColor4Cap, "lblPlantColor4Cap")
        Me.lblPlantColor4Cap.Name = "lblPlantColor4Cap"
        '
        'TabPage5
        '
        Me.TabPage5.BackColor = System.Drawing.SystemColors.Control
        Me.TabPage5.Controls.Add(Me.pnlTab5)
        Me.TabPage5.Controls.Add(Me.Label5)
        Me.TabPage5.Controls.Add(Me.Label6)
        resources.ApplyResources(Me.TabPage5, "TabPage5")
        Me.TabPage5.Name = "TabPage5"
        '
        'pnlTab5
        '
        resources.ApplyResources(Me.pnlTab5, "pnlTab5")
        Me.pnlTab5.Controls.Add(Me.lblSysNum001)
        Me.pnlTab5.Controls.Add(Me.cboValveDesc001)
        Me.pnlTab5.Name = "pnlTab5"
        Me.pnlTab5.TabStop = True
        '
        'lblSysNum001
        '
        resources.ApplyResources(Me.lblSysNum001, "lblSysNum001")
        Me.lblSysNum001.Name = "lblSysNum001"
        '
        'cboValveDesc001
        '
        Me.cboValveDesc001.FormattingEnabled = True
        resources.ApplyResources(Me.cboValveDesc001, "cboValveDesc001")
        Me.cboValveDesc001.Name = "cboValveDesc001"
        '
        'Label5
        '
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.Name = "Label5"
        '
        'Label6
        '
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Name = "Label6"
        '
        'lstStatus
        '
        Me.lstStatus.BackColor = System.Drawing.SystemColors.Info
        resources.ApplyResources(Me.lstStatus, "lstStatus")
        Me.lstStatus.Name = "lstStatus"
        '
        'cboParam
        '
        Me.cboParam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboParam, "cboParam")
        Me.cboParam.Name = "cboParam"
        '
        'lblParam
        '
        resources.ApplyResources(Me.lblParam, "lblParam")
        Me.lblParam.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblParam.Name = "lblParam"
        Me.lblParam.UseMnemonic = False
        '
        'lblRobot
        '
        resources.ApplyResources(Me.lblRobot, "lblRobot")
        Me.lblRobot.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblRobot.Name = "lblRobot"
        Me.lblRobot.UseMnemonic = False
        '
        'lblZone
        '
        resources.ApplyResources(Me.lblZone, "lblZone")
        Me.lblZone.FlatStyle = System.Windows.Forms.FlatStyle.Flat
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
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.btnSave, Me.btnUndo, Me.btnCopy, Me.btnPrint, Me.btnChangeLog, Me.btnUpload, Me.btnMultiView, Me.btnStatus, Me.btnUtilities})
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
        resources.ApplyResources(Me.btnPrint, "btnPrint")
        Me.btnPrint.DropDownButtonWidth = 13
        Me.btnPrint.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuPrint, Me.mnuPrintPreview, Me.mnuPageSetup, Me.mnuPrintFile, Me.mnuPrintOptions})
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
        'mnuPrintOptions
        '
        Me.mnuPrintOptions.Name = "mnuPrintOptions"
        resources.ApplyResources(Me.mnuPrintOptions, "mnuPrintOptions")
        '
        'btnChangeLog
        '
        resources.ApplyResources(Me.btnChangeLog, "btnChangeLog")
        Me.btnChangeLog.DropDownButtonWidth = 13
        Me.btnChangeLog.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuLast24, Me.mnuLast7, Me.mnuAllChanges})
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
        'btnUpload
        '
        resources.ApplyResources(Me.btnUpload, "btnUpload")
        Me.btnUpload.Name = "btnUpload"
        '
        'btnMultiView
        '
        resources.ApplyResources(Me.btnMultiView, "btnMultiView")
        Me.btnMultiView.Name = "btnMultiView"
        '
        'btnStatus
        '
        resources.ApplyResources(Me.btnStatus, "btnStatus")
        Me.btnStatus.Name = "btnStatus"
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
        'pnlTxt
        '
        resources.ApplyResources(Me.pnlTxt, "pnlTxt")
        '
        'pnlProg
        '
        resources.ApplyResources(Me.pnlProg, "pnlProg")
        '
        'pnlTab5crap
        '
        resources.ApplyResources(Me.pnlTab5crap, "pnlTab5crap")
        Me.pnlTab5crap.Controls.Add(Me.lblSysNum001crap)
        Me.pnlTab5crap.Controls.Add(Me.cboValveDesc001crap)
        Me.pnlTab5crap.Name = "pnlTab5crap"
        Me.pnlTab5crap.TabStop = True
        '
        'lblSysNum001crap
        '
        resources.ApplyResources(Me.lblSysNum001crap, "lblSysNum001crap")
        Me.lblSysNum001crap.Name = "lblSysNum001crap"
        '
        'cboValveDesc001crap
        '
        Me.cboValveDesc001crap.FormattingEnabled = True
        resources.ApplyResources(Me.cboValveDesc001crap, "cboValveDesc001crap")
        Me.cboValveDesc001crap.Name = "cboValveDesc001crap"
        '
        'lblValveDesc
        '
        resources.ApplyResources(Me.lblValveDesc, "lblValveDesc")
        Me.lblValveDesc.Name = "lblValveDesc"
        '
        'lblSystemDesc
        '
        resources.ApplyResources(Me.lblSystemDesc, "lblSystemDesc")
        Me.lblSystemDesc.Name = "lblSystemDesc"
        '
        'Panel1
        '
        resources.ApplyResources(Me.Panel1, "Panel1")
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.ComboBox1)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.TabStop = True
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'ComboBox1
        '
        Me.ComboBox1.FormattingEnabled = True
        resources.ApplyResources(Me.ComboBox1, "ComboBox1")
        Me.ComboBox1.Name = "ComboBox1"
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Name = "Label2"
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        '
        'frmMain
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.tscMain)
        Me.Name = "frmMain"
        Me.tscMain.BottomToolStripPanel.ResumeLayout(False)
        Me.tscMain.BottomToolStripPanel.PerformLayout()
        Me.tscMain.ContentPanel.ResumeLayout(False)
        Me.tscMain.ContentPanel.PerformLayout()
        Me.tscMain.TopToolStripPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.PerformLayout()
        Me.tscMain.ResumeLayout(False)
        Me.tscMain.PerformLayout()
        Me.stsStatus.ResumeLayout(False)
        Me.stsStatus.PerformLayout()
        Me.pnlMain.ResumeLayout(False)
        Me.tabMain.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.pnlTab1.ResumeLayout(False)
        Me.pnlTab1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.pnlTab2.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage3.PerformLayout()
        Me.TabPage4.ResumeLayout(False)
        Me.pnlTab4.ResumeLayout(False)
        Me.pnlTab4.PerformLayout()
        Me.TabPage5.ResumeLayout(False)
        Me.TabPage5.PerformLayout()
        Me.pnlTab5.ResumeLayout(False)
        Me.pnlTab5.PerformLayout()
        Me.tlsMain.ResumeLayout(False)
        Me.tlsMain.PerformLayout()
        CType(Me.pnlTxt, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pnlProg, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlTab5crap.ResumeLayout(False)
        Me.pnlTab5crap.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tscMain As System.Windows.Forms.ToolStripContainer
    Friend WithEvents stsStatus As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tspProgress As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents pnlTxt As System.Windows.Forms.StatusBarPanel
    Friend WithEvents pnlProg As System.Windows.Forms.StatusBarPanel
    Friend WithEvents pnlMain As System.Windows.Forms.Panel
    Friend WithEvents lstStatus As System.Windows.Forms.ListBox
    Friend WithEvents cboParam As System.Windows.Forms.ComboBox
    Friend WithEvents lblParam As System.Windows.Forms.Label
    Friend WithEvents lblRobot As System.Windows.Forms.Label
    Friend WithEvents lblZone As System.Windows.Forms.Label
    Friend WithEvents cboRobot As System.Windows.Forms.ComboBox
    Friend WithEvents cboZone As System.Windows.Forms.ComboBox
    Friend WithEvents btnFunction As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents mnuLogin As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLogOut As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuRemote As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLocal As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblSpacer As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnClose As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnUndo As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnCopy As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnPrint As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuPrintPreview As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnChangeLog As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuLast7 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAllChanges As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnMultiView As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnStatus As System.Windows.Forms.ToolStripButton
    Friend WithEvents mnuLast24 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrint As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tabMain As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents lblColorDescCap As System.Windows.Forms.Label
    Friend WithEvents lblValveNumberCap As System.Windows.Forms.Label
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents lblValveDescCap As System.Windows.Forms.Label
    Friend WithEvents lblPlantColorCap As System.Windows.Forms.Label
    Friend WithEvents lblFanucColorCap As System.Windows.Forms.Label
    Friend WithEvents lblColorDesc2Cap As System.Windows.Forms.Label
    Friend WithEvents lblRobotReqCap As System.Windows.Forms.Label
    Friend WithEvents lblPlantColor2Cap As System.Windows.Forms.Label
    Friend WithEvents btnSave As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents btnUpload As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents pnlTab2 As System.Windows.Forms.Panel
    Friend WithEvents clbRobotsReq001 As System.Windows.Forms.CheckedListBox
    Friend WithEvents lblColorDesc001 As System.Windows.Forms.Label
    Friend WithEvents lblPlantColor001 As System.Windows.Forms.Label
    Friend WithEvents pnlTab1 As System.Windows.Forms.Panel
    Friend WithEvents lblFanucColor001 As System.Windows.Forms.Label
    Friend WithEvents ftbPlantColor001 As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbColorDesc001 As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbValveNumber001 As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbValveDesc001 As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblColor001 As System.Windows.Forms.Label
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents pnlTab3 As System.Windows.Forms.Panel
    Friend WithEvents lblColorDescT3001 As System.Windows.Forms.Label
    Friend WithEvents lblPlantColorT3001 As System.Windows.Forms.Label
    Friend WithEvents lblColorDesc3Cap As System.Windows.Forms.Label
    Friend WithEvents lblPlantColor3Cap As System.Windows.Forms.Label
    Friend WithEvents lblHardenerRatioCap As System.Windows.Forms.Label
    Friend WithEvents lblResinRatioCap As System.Windows.Forms.Label
    Friend WithEvents lblHardenerCap As System.Windows.Forms.Label
    Friend WithEvents lblColon001 As System.Windows.Forms.Label
    Friend WithEvents ftbHardRatio001 As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbResinRatio001 As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbHardValve001 As FocusedTextBox.FocusedTextBox
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Friend WithEvents pnlTab4 As System.Windows.Forms.Panel
    Friend WithEvents lblTab4ColorDesc001 As System.Windows.Forms.Label
    Friend WithEvents lblTab4PlantColor001 As System.Windows.Forms.Label
    Friend WithEvents lblColorDesc4Cap As System.Windows.Forms.Label
    Friend WithEvents lblTricoat4Cap As System.Windows.Forms.Label
    Friend WithEvents lblPlantColor4Cap As System.Windows.Forms.Label
    Friend WithEvents ftbTricoatColor001 As FocusedTextBox.FocusedTextBox
    Friend WithEvents mnuPageSetup As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintOptions As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblHardSolvCap As System.Windows.Forms.Label
    Friend WithEvents lblResinSolvCap As System.Windows.Forms.Label
    Friend WithEvents ftbResinSolv001 As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbHardSolv001 As FocusedTextBox.FocusedTextBox
    Friend WithEvents btnUtilities As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuExport As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuImport As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblTwoCoats As System.Windows.Forms.Label
    Friend WithEvents ftbTwoCoat001 As System.Windows.Forms.CheckBox
    Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
    Friend WithEvents pnlTab5 As System.Windows.Forms.Panel
    Friend WithEvents lblSysNum001 As System.Windows.Forms.Label
    Friend WithEvents cboValveDesc001 As System.Windows.Forms.ComboBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents pnlTab5crap As System.Windows.Forms.Panel
    Friend WithEvents lblSysNum001crap As System.Windows.Forms.Label
    Friend WithEvents cboValveDesc001crap As System.Windows.Forms.ComboBox
    Friend WithEvents lblValveDesc As System.Windows.Forms.Label
    Friend WithEvents lblSystemDesc As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ComboBox1 As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
End Class
