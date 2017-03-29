<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> <System.Runtime.InteropServices.ComVisible(False)> _
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
        Me.lblParm = New System.Windows.Forms.Label
        Me.cboParm = New System.Windows.Forms.ComboBox
        Me.lblRobot = New System.Windows.Forms.Label
        Me.cboRobot = New System.Windows.Forms.ComboBox
        Me.pnlMain = New System.Windows.Forms.Panel
        Me.tabMain = New System.Windows.Forms.TabControl
        Me.tabSystem = New System.Windows.Forms.TabPage
        Me.chkRC_12 = New System.Windows.Forms.CheckBox
        Me.chkRC_11 = New System.Windows.Forms.CheckBox
        Me.chkRC_10 = New System.Windows.Forms.CheckBox
        Me.chkRC_9 = New System.Windows.Forms.CheckBox
        Me.chkRC_8 = New System.Windows.Forms.CheckBox
        Me.chkRC_7 = New System.Windows.Forms.CheckBox
        Me.chkRC_6 = New System.Windows.Forms.CheckBox
        Me.chkRC_5 = New System.Windows.Forms.CheckBox
        Me.chkRC_4 = New System.Windows.Forms.CheckBox
        Me.chkRC_3 = New System.Windows.Forms.CheckBox
        Me.chkRC_2 = New System.Windows.Forms.CheckBox
        Me.chkRC_1 = New System.Windows.Forms.CheckBox
        Me.cboSchedule12 = New System.Windows.Forms.ComboBox
        Me.cboEnable12 = New System.Windows.Forms.ComboBox
        Me.cboSchedule11 = New System.Windows.Forms.ComboBox
        Me.cboEnable11 = New System.Windows.Forms.ComboBox
        Me.cboSchedule10 = New System.Windows.Forms.ComboBox
        Me.cboEnable10 = New System.Windows.Forms.ComboBox
        Me.cboSchedule9 = New System.Windows.Forms.ComboBox
        Me.cboEnable9 = New System.Windows.Forms.ComboBox
        Me.cboSchedule8 = New System.Windows.Forms.ComboBox
        Me.cboEnable8 = New System.Windows.Forms.ComboBox
        Me.cboSchedule7 = New System.Windows.Forms.ComboBox
        Me.cboEnable7 = New System.Windows.Forms.ComboBox
        Me.cboSchedule6 = New System.Windows.Forms.ComboBox
        Me.cboEnable6 = New System.Windows.Forms.ComboBox
        Me.cboSchedule5 = New System.Windows.Forms.ComboBox
        Me.cboEnable5 = New System.Windows.Forms.ComboBox
        Me.cboSchedule4 = New System.Windows.Forms.ComboBox
        Me.cboEnable4 = New System.Windows.Forms.ComboBox
        Me.cboSchedule3 = New System.Windows.Forms.ComboBox
        Me.cboEnable3 = New System.Windows.Forms.ComboBox
        Me.cboSchedule2 = New System.Windows.Forms.ComboBox
        Me.cboSchedule1 = New System.Windows.Forms.ComboBox
        Me.cboEnable2 = New System.Windows.Forms.ComboBox
        Me.cboEnable1 = New System.Windows.Forms.ComboBox
        Me.ftbArchive = New FocusedTextBox.FocusedTextBox
        Me.ftbDMONFiles = New FocusedTextBox.FocusedTextBox
        Me.lblArchive = New System.Windows.Forms.Label
        Me.lblDMONFiles = New System.Windows.Forms.Label
        Me.lblDaysToKeep = New System.Windows.Forms.Label
        Me.lblScheduleCap = New System.Windows.Forms.Label
        Me.lblEnableCap = New System.Windows.Forms.Label
        Me.lblJobOrCC = New System.Windows.Forms.Label
        Me.tabSchedules = New System.Windows.Forms.TabPage
        Me.lblSaveWarning = New System.Windows.Forms.Label
        Me.cboRecordMode = New System.Windows.Forms.ComboBox
        Me.lblRecordMode = New System.Windows.Forms.Label
        Me.ftbStopValue = New FocusedTextBox.FocusedTextBox
        Me.cboStopCond = New System.Windows.Forms.ComboBox
        Me.chkStopItem = New System.Windows.Forms.CheckBox
        Me.cboStopItem = New System.Windows.Forms.ComboBox
        Me.ftbStartValue = New FocusedTextBox.FocusedTextBox
        Me.cboStartCond = New System.Windows.Forms.ComboBox
        Me.chkStartItem = New System.Windows.Forms.CheckBox
        Me.cboStartItem = New System.Windows.Forms.ComboBox
        Me.ftbRecordAct = New FocusedTextBox.FocusedTextBox
        Me.ftbRecordReq = New FocusedTextBox.FocusedTextBox
        Me.lblRecord = New System.Windows.Forms.Label
        Me.lblMonitor = New System.Windows.Forms.Label
        Me.lblSample = New System.Windows.Forms.Label
        Me.ftbMonitorAct = New FocusedTextBox.FocusedTextBox
        Me.ftbMonitorReq = New FocusedTextBox.FocusedTextBox
        Me.ftbSampleAct = New FocusedTextBox.FocusedTextBox
        Me.ftbSampleReq = New FocusedTextBox.FocusedTextBox
        Me.lblAct = New System.Windows.Forms.Label
        Me.lblReq = New System.Windows.Forms.Label
        Me.lblFrequency = New System.Windows.Forms.Label
        Me.lblKB = New System.Windows.Forms.Label
        Me.ftbFileSize = New FocusedTextBox.FocusedTextBox
        Me.lblFileSize = New System.Windows.Forms.Label
        Me.ftbFileIndex = New FocusedTextBox.FocusedTextBox
        Me.lblFileIndex = New System.Windows.Forms.Label
        Me.cboFileDevice = New System.Windows.Forms.ComboBox
        Me.lblFileDevice = New System.Windows.Forms.Label
        Me.ftbNumItems = New FocusedTextBox.FocusedTextBox
        Me.lblNumItems = New System.Windows.Forms.Label
        Me.cboItem10 = New System.Windows.Forms.ComboBox
        Me.lblItem10 = New System.Windows.Forms.Label
        Me.cboItem9 = New System.Windows.Forms.ComboBox
        Me.lblItem9 = New System.Windows.Forms.Label
        Me.cboItem8 = New System.Windows.Forms.ComboBox
        Me.lblItem8 = New System.Windows.Forms.Label
        Me.cboItem7 = New System.Windows.Forms.ComboBox
        Me.lblItem7 = New System.Windows.Forms.Label
        Me.cboItem6 = New System.Windows.Forms.ComboBox
        Me.lblItem6 = New System.Windows.Forms.Label
        Me.cboItem5 = New System.Windows.Forms.ComboBox
        Me.lblItem5 = New System.Windows.Forms.Label
        Me.cboItem4 = New System.Windows.Forms.ComboBox
        Me.lblItem4 = New System.Windows.Forms.Label
        Me.cboItem3 = New System.Windows.Forms.ComboBox
        Me.lblItem3 = New System.Windows.Forms.Label
        Me.cboItem2 = New System.Windows.Forms.ComboBox
        Me.lblItem2 = New System.Windows.Forms.Label
        Me.cboItem1 = New System.Windows.Forms.ComboBox
        Me.lblItem1 = New System.Windows.Forms.Label
        Me.ftbFileName = New FocusedTextBox.FocusedTextBox
        Me.lblFileName = New System.Windows.Forms.Label
        Me.ftbComment = New FocusedTextBox.FocusedTextBox
        Me.lblComment = New System.Windows.Forms.Label
        Me.tabItems = New System.Windows.Forms.TabPage
        Me.btnBrowse = New System.Windows.Forms.Button
        Me.lblPortNumComment = New System.Windows.Forms.Label
        Me.btnValidate = New System.Windows.Forms.Button
        Me.lblSaveWarning1 = New System.Windows.Forms.Label
        Me.lblA = New System.Windows.Forms.Label
        Me.lblG = New System.Windows.Forms.Label
        Me.ftbAxis = New FocusedTextBox.FocusedTextBox
        Me.ftbGroup = New FocusedTextBox.FocusedTextBox
        Me.lblAxis = New System.Windows.Forms.Label
        Me.ftbIntercept = New FocusedTextBox.FocusedTextBox
        Me.lblIntercept = New System.Windows.Forms.Label
        Me.ftbSlope = New FocusedTextBox.FocusedTextBox
        Me.lblSlope = New System.Windows.Forms.Label
        Me.ftbUnits = New FocusedTextBox.FocusedTextBox
        Me.lblUnits = New System.Windows.Forms.Label
        Me.ftbDesc = New FocusedTextBox.FocusedTextBox
        Me.lblDesc = New System.Windows.Forms.Label
        Me.ftbVarName = New FocusedTextBox.FocusedTextBox
        Me.lblVarName = New System.Windows.Forms.Label
        Me.lblIoType = New System.Windows.Forms.Label
        Me.cboIoType = New System.Windows.Forms.ComboBox
        Me.lblType = New System.Windows.Forms.Label
        Me.cboType = New System.Windows.Forms.ComboBox
        Me.ftbPrgName = New FocusedTextBox.FocusedTextBox
        Me.lblPrgName = New System.Windows.Forms.Label
        Me.ftbPortNum = New FocusedTextBox.FocusedTextBox
        Me.lblPortNum = New System.Windows.Forms.Label
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.lblZone = New System.Windows.Forms.Label
        Me.cboZone = New System.Windows.Forms.ComboBox
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.btnSave = New System.Windows.Forms.ToolStripButton
        Me.btnPrint = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuPrintAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrint = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintPreview = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPageSetup = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintFile = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintOptions = New System.Windows.Forms.ToolStripMenuItem
        Me.btnUndo = New System.Windows.Forms.ToolStripButton
        Me.btnChangeLog = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuLast24 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLast7 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAllChanges = New System.Windows.Forms.ToolStripMenuItem
        Me.btnUtilities = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuExport = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuImport = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuSaveDefault = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuRestoreDefault = New System.Windows.Forms.ToolStripMenuItem
        Me.pnlTxt = New System.Windows.Forms.StatusBarPanel
        Me.pnlProg = New System.Windows.Forms.StatusBarPanel
        Me.tscMain.BottomToolStripPanel.SuspendLayout()
        Me.tscMain.ContentPanel.SuspendLayout()
        Me.tscMain.TopToolStripPanel.SuspendLayout()
        Me.tscMain.SuspendLayout()
        Me.stsStatus.SuspendLayout()
        Me.pnlMain.SuspendLayout()
        Me.tabMain.SuspendLayout()
        Me.tabSystem.SuspendLayout()
        Me.tabSchedules.SuspendLayout()
        Me.tabItems.SuspendLayout()
        Me.tlsMain.SuspendLayout()
        CType(Me.pnlTxt, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pnlProg, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.tscMain.ContentPanel.Controls.Add(Me.lblParm)
        Me.tscMain.ContentPanel.Controls.Add(Me.cboParm)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblRobot)
        Me.tscMain.ContentPanel.Controls.Add(Me.cboRobot)
        Me.tscMain.ContentPanel.Controls.Add(Me.pnlMain)
        Me.tscMain.ContentPanel.Controls.Add(Me.lstStatus)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblZone)
        Me.tscMain.ContentPanel.Controls.Add(Me.cboZone)
        Me.tscMain.ContentPanel.Size = New System.Drawing.Size(918, 615)
        Me.tscMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tscMain.LeftToolStripPanelVisible = False
        Me.tscMain.Location = New System.Drawing.Point(0, 0)
        Me.tscMain.Name = "tscMain"
        Me.tscMain.RightToolStripPanelVisible = False
        Me.tscMain.Size = New System.Drawing.Size(918, 704)
        Me.tscMain.TabIndex = 1
        Me.tscMain.Text = "ToolStripContainer1"
        '
        'tscMain.TopToolStripPanel
        '
        Me.tscMain.TopToolStripPanel.Controls.Add(Me.tlsMain)
        '
        'stsStatus
        '
        Me.stsStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.stsStatus.Dock = System.Windows.Forms.DockStyle.None
        Me.stsStatus.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.stsStatus.GripMargin = New System.Windows.Forms.Padding(0)
        Me.stsStatus.ImageScalingSize = New System.Drawing.Size(22, 22)
        Me.stsStatus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatus, Me.tspProgress, Me.lblSpacer, Me.btnFunction})
        Me.stsStatus.Location = New System.Drawing.Point(0, 0)
        Me.stsStatus.Name = "stsStatus"
        Me.stsStatus.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode
        Me.stsStatus.ShowItemToolTips = True
        Me.stsStatus.Size = New System.Drawing.Size(918, 29)
        Me.stsStatus.TabIndex = 0
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = False
        Me.lblStatus.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedOuter
        Me.lblStatus.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.lblStatus.ImageTransparentColor = System.Drawing.Color.Silver
        Me.lblStatus.Margin = New System.Windows.Forms.Padding(0)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(482, 29)
        Me.lblStatus.Text = "Status...."
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tspProgress
        '
        Me.tspProgress.AutoSize = False
        Me.tspProgress.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.tspProgress.Margin = New System.Windows.Forms.Padding(2, 3, 1, 3)
        Me.tspProgress.Name = "tspProgress"
        Me.tspProgress.Padding = New System.Windows.Forms.Padding(2)
        Me.tspProgress.Size = New System.Drawing.Size(0, 23)
        Me.tspProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        '
        'lblSpacer
        '
        Me.lblSpacer.AutoSize = False
        Me.lblSpacer.Name = "lblSpacer"
        Me.lblSpacer.Size = New System.Drawing.Size(10, 24)
        '
        'btnFunction
        '
        Me.btnFunction.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.btnFunction.AutoSize = False
        Me.btnFunction.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnFunction.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuLogin, Me.mnuLogOut, Me.mnuRemote, Me.mnuLocal})
        Me.btnFunction.ImageTransparentColor = System.Drawing.Color.White
        Me.btnFunction.Name = "btnFunction"
        Me.btnFunction.Padding = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.btnFunction.Size = New System.Drawing.Size(39, 27)
        '
        'mnuLogin
        '
        Me.mnuLogin.Name = "mnuLogin"
        Me.mnuLogin.Size = New System.Drawing.Size(190, 22)
        Me.mnuLogin.Text = "ToolStripMenuItem1"
        '
        'mnuLogOut
        '
        Me.mnuLogOut.Name = "mnuLogOut"
        Me.mnuLogOut.Size = New System.Drawing.Size(190, 22)
        Me.mnuLogOut.Text = "ToolStripMenuItem1"
        '
        'mnuRemote
        '
        Me.mnuRemote.Name = "mnuRemote"
        Me.mnuRemote.Size = New System.Drawing.Size(190, 22)
        Me.mnuRemote.Text = "ToolStripMenuItem1"
        '
        'mnuLocal
        '
        Me.mnuLocal.Name = "mnuLocal"
        Me.mnuLocal.Size = New System.Drawing.Size(190, 22)
        Me.mnuLocal.Text = "ToolStripMenuItem1"
        '
        'lblParm
        '
        Me.lblParm.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblParm.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblParm.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblParm.Location = New System.Drawing.Point(322, 8)
        Me.lblParm.Name = "lblParm"
        Me.lblParm.Size = New System.Drawing.Size(241, 19)
        Me.lblParm.TabIndex = 101
        Me.lblParm.Text = "parameter"
        Me.lblParm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblParm.UseMnemonic = False
        Me.lblParm.Visible = False
        '
        'cboParm
        '
        Me.cboParm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboParm.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboParm.ItemHeight = 18
        Me.cboParm.Location = New System.Drawing.Point(322, 33)
        Me.cboParm.Name = "cboParm"
        Me.cboParm.Size = New System.Drawing.Size(241, 26)
        Me.cboParm.TabIndex = 100
        Me.cboParm.Visible = False
        '
        'lblRobot
        '
        Me.lblRobot.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblRobot.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblRobot.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblRobot.Location = New System.Drawing.Point(195, 8)
        Me.lblRobot.Name = "lblRobot"
        Me.lblRobot.Size = New System.Drawing.Size(111, 19)
        Me.lblRobot.TabIndex = 99
        Me.lblRobot.Text = "Robot"
        Me.lblRobot.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblRobot.UseMnemonic = False
        Me.lblRobot.Visible = False
        '
        'cboRobot
        '
        Me.cboRobot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboRobot.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboRobot.ItemHeight = 18
        Me.cboRobot.Location = New System.Drawing.Point(195, 33)
        Me.cboRobot.Name = "cboRobot"
        Me.cboRobot.Size = New System.Drawing.Size(111, 26)
        Me.cboRobot.TabIndex = 98
        Me.cboRobot.Visible = False
        '
        'pnlMain
        '
        Me.pnlMain.Controls.Add(Me.tabMain)
        Me.pnlMain.Font = New System.Drawing.Font("Arial", 8.25!)
        Me.pnlMain.Location = New System.Drawing.Point(12, 68)
        Me.pnlMain.Name = "pnlMain"
        Me.pnlMain.Size = New System.Drawing.Size(882, 542)
        Me.pnlMain.TabIndex = 19
        '
        'tabMain
        '
        Me.tabMain.Controls.Add(Me.tabSystem)
        Me.tabMain.Controls.Add(Me.tabSchedules)
        Me.tabMain.Controls.Add(Me.tabItems)
        Me.tabMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabMain.Location = New System.Drawing.Point(0, 0)
        Me.tabMain.Name = "tabMain"
        Me.tabMain.SelectedIndex = 0
        Me.tabMain.Size = New System.Drawing.Size(882, 542)
        Me.tabMain.TabIndex = 0
        '
        'tabSystem
        '
        Me.tabSystem.AutoScroll = True
        Me.tabSystem.BackColor = System.Drawing.SystemColors.Control
        Me.tabSystem.Controls.Add(Me.chkRC_12)
        Me.tabSystem.Controls.Add(Me.chkRC_11)
        Me.tabSystem.Controls.Add(Me.chkRC_10)
        Me.tabSystem.Controls.Add(Me.chkRC_9)
        Me.tabSystem.Controls.Add(Me.chkRC_8)
        Me.tabSystem.Controls.Add(Me.chkRC_7)
        Me.tabSystem.Controls.Add(Me.chkRC_6)
        Me.tabSystem.Controls.Add(Me.chkRC_5)
        Me.tabSystem.Controls.Add(Me.chkRC_4)
        Me.tabSystem.Controls.Add(Me.chkRC_3)
        Me.tabSystem.Controls.Add(Me.chkRC_2)
        Me.tabSystem.Controls.Add(Me.chkRC_1)
        Me.tabSystem.Controls.Add(Me.cboSchedule12)
        Me.tabSystem.Controls.Add(Me.cboEnable12)
        Me.tabSystem.Controls.Add(Me.cboSchedule11)
        Me.tabSystem.Controls.Add(Me.cboEnable11)
        Me.tabSystem.Controls.Add(Me.cboSchedule10)
        Me.tabSystem.Controls.Add(Me.cboEnable10)
        Me.tabSystem.Controls.Add(Me.cboSchedule9)
        Me.tabSystem.Controls.Add(Me.cboEnable9)
        Me.tabSystem.Controls.Add(Me.cboSchedule8)
        Me.tabSystem.Controls.Add(Me.cboEnable8)
        Me.tabSystem.Controls.Add(Me.cboSchedule7)
        Me.tabSystem.Controls.Add(Me.cboEnable7)
        Me.tabSystem.Controls.Add(Me.cboSchedule6)
        Me.tabSystem.Controls.Add(Me.cboEnable6)
        Me.tabSystem.Controls.Add(Me.cboSchedule5)
        Me.tabSystem.Controls.Add(Me.cboEnable5)
        Me.tabSystem.Controls.Add(Me.cboSchedule4)
        Me.tabSystem.Controls.Add(Me.cboEnable4)
        Me.tabSystem.Controls.Add(Me.cboSchedule3)
        Me.tabSystem.Controls.Add(Me.cboEnable3)
        Me.tabSystem.Controls.Add(Me.cboSchedule2)
        Me.tabSystem.Controls.Add(Me.cboSchedule1)
        Me.tabSystem.Controls.Add(Me.cboEnable2)
        Me.tabSystem.Controls.Add(Me.cboEnable1)
        Me.tabSystem.Controls.Add(Me.ftbArchive)
        Me.tabSystem.Controls.Add(Me.ftbDMONFiles)
        Me.tabSystem.Controls.Add(Me.lblArchive)
        Me.tabSystem.Controls.Add(Me.lblDMONFiles)
        Me.tabSystem.Controls.Add(Me.lblDaysToKeep)
        Me.tabSystem.Controls.Add(Me.lblScheduleCap)
        Me.tabSystem.Controls.Add(Me.lblEnableCap)
        Me.tabSystem.Controls.Add(Me.lblJobOrCC)
        Me.tabSystem.Location = New System.Drawing.Point(4, 23)
        Me.tabSystem.Name = "tabSystem"
        Me.tabSystem.Padding = New System.Windows.Forms.Padding(3)
        Me.tabSystem.Size = New System.Drawing.Size(874, 515)
        Me.tabSystem.TabIndex = 0
        Me.tabSystem.Text = "tabSystem"
        '
        'chkRC_12
        '
        Me.chkRC_12.Enabled = False
        Me.chkRC_12.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.chkRC_12.Location = New System.Drawing.Point(228, 424)
        Me.chkRC_12.Name = "chkRC_12"
        Me.chkRC_12.Size = New System.Drawing.Size(108, 26)
        Me.chkRC_12.TabIndex = 186
        Me.chkRC_12.Text = "chkRC_12"
        Me.chkRC_12.UseVisualStyleBackColor = True
        Me.chkRC_12.Visible = False
        '
        'chkRC_11
        '
        Me.chkRC_11.Enabled = False
        Me.chkRC_11.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.chkRC_11.Location = New System.Drawing.Point(228, 392)
        Me.chkRC_11.Name = "chkRC_11"
        Me.chkRC_11.Size = New System.Drawing.Size(108, 26)
        Me.chkRC_11.TabIndex = 185
        Me.chkRC_11.Text = "chkRC_11"
        Me.chkRC_11.UseVisualStyleBackColor = True
        Me.chkRC_11.Visible = False
        '
        'chkRC_10
        '
        Me.chkRC_10.Enabled = False
        Me.chkRC_10.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.chkRC_10.Location = New System.Drawing.Point(228, 360)
        Me.chkRC_10.Name = "chkRC_10"
        Me.chkRC_10.Size = New System.Drawing.Size(108, 26)
        Me.chkRC_10.TabIndex = 184
        Me.chkRC_10.Text = "chkRC_10"
        Me.chkRC_10.UseVisualStyleBackColor = True
        Me.chkRC_10.Visible = False
        '
        'chkRC_9
        '
        Me.chkRC_9.Enabled = False
        Me.chkRC_9.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.chkRC_9.Location = New System.Drawing.Point(228, 328)
        Me.chkRC_9.Name = "chkRC_9"
        Me.chkRC_9.Size = New System.Drawing.Size(108, 26)
        Me.chkRC_9.TabIndex = 183
        Me.chkRC_9.Text = "chkRC_9"
        Me.chkRC_9.UseVisualStyleBackColor = True
        Me.chkRC_9.Visible = False
        '
        'chkRC_8
        '
        Me.chkRC_8.Enabled = False
        Me.chkRC_8.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.chkRC_8.Location = New System.Drawing.Point(228, 296)
        Me.chkRC_8.Name = "chkRC_8"
        Me.chkRC_8.Size = New System.Drawing.Size(108, 26)
        Me.chkRC_8.TabIndex = 182
        Me.chkRC_8.Text = "chkRC_8"
        Me.chkRC_8.UseVisualStyleBackColor = True
        Me.chkRC_8.Visible = False
        '
        'chkRC_7
        '
        Me.chkRC_7.Enabled = False
        Me.chkRC_7.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.chkRC_7.Location = New System.Drawing.Point(228, 264)
        Me.chkRC_7.Name = "chkRC_7"
        Me.chkRC_7.Size = New System.Drawing.Size(108, 26)
        Me.chkRC_7.TabIndex = 181
        Me.chkRC_7.Text = "chkRC_7"
        Me.chkRC_7.UseVisualStyleBackColor = True
        Me.chkRC_7.Visible = False
        '
        'chkRC_6
        '
        Me.chkRC_6.Enabled = False
        Me.chkRC_6.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.chkRC_6.Location = New System.Drawing.Point(228, 232)
        Me.chkRC_6.Name = "chkRC_6"
        Me.chkRC_6.Size = New System.Drawing.Size(108, 26)
        Me.chkRC_6.TabIndex = 180
        Me.chkRC_6.Text = "chkRC_6"
        Me.chkRC_6.UseVisualStyleBackColor = True
        Me.chkRC_6.Visible = False
        '
        'chkRC_5
        '
        Me.chkRC_5.Enabled = False
        Me.chkRC_5.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.chkRC_5.Location = New System.Drawing.Point(228, 200)
        Me.chkRC_5.Name = "chkRC_5"
        Me.chkRC_5.Size = New System.Drawing.Size(108, 26)
        Me.chkRC_5.TabIndex = 179
        Me.chkRC_5.Text = "chkRC_5"
        Me.chkRC_5.UseVisualStyleBackColor = True
        Me.chkRC_5.Visible = False
        '
        'chkRC_4
        '
        Me.chkRC_4.Enabled = False
        Me.chkRC_4.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.chkRC_4.Location = New System.Drawing.Point(228, 168)
        Me.chkRC_4.Name = "chkRC_4"
        Me.chkRC_4.Size = New System.Drawing.Size(108, 26)
        Me.chkRC_4.TabIndex = 178
        Me.chkRC_4.Text = "chkRC_4"
        Me.chkRC_4.UseVisualStyleBackColor = True
        Me.chkRC_4.Visible = False
        '
        'chkRC_3
        '
        Me.chkRC_3.Enabled = False
        Me.chkRC_3.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.chkRC_3.Location = New System.Drawing.Point(228, 136)
        Me.chkRC_3.Name = "chkRC_3"
        Me.chkRC_3.Size = New System.Drawing.Size(108, 26)
        Me.chkRC_3.TabIndex = 177
        Me.chkRC_3.Text = "chkRC_3"
        Me.chkRC_3.UseVisualStyleBackColor = True
        Me.chkRC_3.Visible = False
        '
        'chkRC_2
        '
        Me.chkRC_2.Enabled = False
        Me.chkRC_2.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.chkRC_2.Location = New System.Drawing.Point(228, 104)
        Me.chkRC_2.Name = "chkRC_2"
        Me.chkRC_2.Size = New System.Drawing.Size(108, 26)
        Me.chkRC_2.TabIndex = 176
        Me.chkRC_2.Text = "chkRC_2"
        Me.chkRC_2.UseVisualStyleBackColor = True
        Me.chkRC_2.Visible = False
        '
        'chkRC_1
        '
        Me.chkRC_1.Enabled = False
        Me.chkRC_1.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.chkRC_1.Location = New System.Drawing.Point(228, 72)
        Me.chkRC_1.Name = "chkRC_1"
        Me.chkRC_1.Size = New System.Drawing.Size(108, 26)
        Me.chkRC_1.TabIndex = 175
        Me.chkRC_1.Text = "chkRC_1"
        Me.chkRC_1.UseVisualStyleBackColor = True
        Me.chkRC_1.Visible = False
        '
        'cboSchedule12
        '
        Me.cboSchedule12.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSchedule12.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboSchedule12.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboSchedule12.ItemHeight = 18
        Me.cboSchedule12.Location = New System.Drawing.Point(477, 424)
        Me.cboSchedule12.MaxDropDownItems = 10
        Me.cboSchedule12.Name = "cboSchedule12"
        Me.cboSchedule12.Size = New System.Drawing.Size(169, 26)
        Me.cboSchedule12.TabIndex = 161
        Me.cboSchedule12.Visible = False
        '
        'cboEnable12
        '
        Me.cboEnable12.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboEnable12.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboEnable12.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboEnable12.ItemHeight = 18
        Me.cboEnable12.Location = New System.Drawing.Point(342, 424)
        Me.cboEnable12.MaxDropDownItems = 10
        Me.cboEnable12.Name = "cboEnable12"
        Me.cboEnable12.Size = New System.Drawing.Size(111, 26)
        Me.cboEnable12.TabIndex = 160
        Me.cboEnable12.Visible = False
        '
        'cboSchedule11
        '
        Me.cboSchedule11.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSchedule11.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboSchedule11.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboSchedule11.ItemHeight = 18
        Me.cboSchedule11.Location = New System.Drawing.Point(477, 392)
        Me.cboSchedule11.MaxDropDownItems = 10
        Me.cboSchedule11.Name = "cboSchedule11"
        Me.cboSchedule11.Size = New System.Drawing.Size(169, 26)
        Me.cboSchedule11.TabIndex = 158
        Me.cboSchedule11.Visible = False
        '
        'cboEnable11
        '
        Me.cboEnable11.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboEnable11.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboEnable11.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboEnable11.ItemHeight = 18
        Me.cboEnable11.Location = New System.Drawing.Point(342, 392)
        Me.cboEnable11.MaxDropDownItems = 10
        Me.cboEnable11.Name = "cboEnable11"
        Me.cboEnable11.Size = New System.Drawing.Size(111, 26)
        Me.cboEnable11.TabIndex = 157
        Me.cboEnable11.Visible = False
        '
        'cboSchedule10
        '
        Me.cboSchedule10.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSchedule10.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboSchedule10.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboSchedule10.ItemHeight = 18
        Me.cboSchedule10.Location = New System.Drawing.Point(477, 360)
        Me.cboSchedule10.MaxDropDownItems = 10
        Me.cboSchedule10.Name = "cboSchedule10"
        Me.cboSchedule10.Size = New System.Drawing.Size(169, 26)
        Me.cboSchedule10.TabIndex = 155
        Me.cboSchedule10.Visible = False
        '
        'cboEnable10
        '
        Me.cboEnable10.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboEnable10.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboEnable10.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboEnable10.ItemHeight = 18
        Me.cboEnable10.Location = New System.Drawing.Point(342, 360)
        Me.cboEnable10.MaxDropDownItems = 10
        Me.cboEnable10.Name = "cboEnable10"
        Me.cboEnable10.Size = New System.Drawing.Size(111, 26)
        Me.cboEnable10.TabIndex = 154
        Me.cboEnable10.Visible = False
        '
        'cboSchedule9
        '
        Me.cboSchedule9.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSchedule9.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboSchedule9.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboSchedule9.ItemHeight = 18
        Me.cboSchedule9.Location = New System.Drawing.Point(477, 328)
        Me.cboSchedule9.MaxDropDownItems = 10
        Me.cboSchedule9.Name = "cboSchedule9"
        Me.cboSchedule9.Size = New System.Drawing.Size(169, 26)
        Me.cboSchedule9.TabIndex = 152
        Me.cboSchedule9.Visible = False
        '
        'cboEnable9
        '
        Me.cboEnable9.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboEnable9.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboEnable9.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboEnable9.ItemHeight = 18
        Me.cboEnable9.Location = New System.Drawing.Point(342, 328)
        Me.cboEnable9.MaxDropDownItems = 10
        Me.cboEnable9.Name = "cboEnable9"
        Me.cboEnable9.Size = New System.Drawing.Size(111, 26)
        Me.cboEnable9.TabIndex = 151
        Me.cboEnable9.Visible = False
        '
        'cboSchedule8
        '
        Me.cboSchedule8.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSchedule8.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboSchedule8.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboSchedule8.ItemHeight = 18
        Me.cboSchedule8.Location = New System.Drawing.Point(477, 296)
        Me.cboSchedule8.MaxDropDownItems = 10
        Me.cboSchedule8.Name = "cboSchedule8"
        Me.cboSchedule8.Size = New System.Drawing.Size(169, 26)
        Me.cboSchedule8.TabIndex = 149
        Me.cboSchedule8.Visible = False
        '
        'cboEnable8
        '
        Me.cboEnable8.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboEnable8.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboEnable8.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboEnable8.ItemHeight = 18
        Me.cboEnable8.Location = New System.Drawing.Point(342, 296)
        Me.cboEnable8.MaxDropDownItems = 10
        Me.cboEnable8.Name = "cboEnable8"
        Me.cboEnable8.Size = New System.Drawing.Size(111, 26)
        Me.cboEnable8.TabIndex = 148
        Me.cboEnable8.Visible = False
        '
        'cboSchedule7
        '
        Me.cboSchedule7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSchedule7.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboSchedule7.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboSchedule7.ItemHeight = 18
        Me.cboSchedule7.Location = New System.Drawing.Point(477, 264)
        Me.cboSchedule7.MaxDropDownItems = 10
        Me.cboSchedule7.Name = "cboSchedule7"
        Me.cboSchedule7.Size = New System.Drawing.Size(169, 26)
        Me.cboSchedule7.TabIndex = 146
        Me.cboSchedule7.Visible = False
        '
        'cboEnable7
        '
        Me.cboEnable7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboEnable7.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboEnable7.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboEnable7.ItemHeight = 18
        Me.cboEnable7.Location = New System.Drawing.Point(342, 264)
        Me.cboEnable7.MaxDropDownItems = 10
        Me.cboEnable7.Name = "cboEnable7"
        Me.cboEnable7.Size = New System.Drawing.Size(111, 26)
        Me.cboEnable7.TabIndex = 145
        Me.cboEnable7.Visible = False
        '
        'cboSchedule6
        '
        Me.cboSchedule6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSchedule6.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboSchedule6.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboSchedule6.ItemHeight = 18
        Me.cboSchedule6.Location = New System.Drawing.Point(477, 232)
        Me.cboSchedule6.MaxDropDownItems = 10
        Me.cboSchedule6.Name = "cboSchedule6"
        Me.cboSchedule6.Size = New System.Drawing.Size(169, 26)
        Me.cboSchedule6.TabIndex = 143
        Me.cboSchedule6.Visible = False
        '
        'cboEnable6
        '
        Me.cboEnable6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboEnable6.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboEnable6.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboEnable6.ItemHeight = 18
        Me.cboEnable6.Location = New System.Drawing.Point(342, 232)
        Me.cboEnable6.MaxDropDownItems = 10
        Me.cboEnable6.Name = "cboEnable6"
        Me.cboEnable6.Size = New System.Drawing.Size(111, 26)
        Me.cboEnable6.TabIndex = 142
        Me.cboEnable6.Visible = False
        '
        'cboSchedule5
        '
        Me.cboSchedule5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSchedule5.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboSchedule5.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboSchedule5.ItemHeight = 18
        Me.cboSchedule5.Location = New System.Drawing.Point(477, 200)
        Me.cboSchedule5.MaxDropDownItems = 10
        Me.cboSchedule5.Name = "cboSchedule5"
        Me.cboSchedule5.Size = New System.Drawing.Size(169, 26)
        Me.cboSchedule5.TabIndex = 140
        Me.cboSchedule5.Visible = False
        '
        'cboEnable5
        '
        Me.cboEnable5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboEnable5.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboEnable5.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboEnable5.ItemHeight = 18
        Me.cboEnable5.Location = New System.Drawing.Point(342, 200)
        Me.cboEnable5.MaxDropDownItems = 10
        Me.cboEnable5.Name = "cboEnable5"
        Me.cboEnable5.Size = New System.Drawing.Size(111, 26)
        Me.cboEnable5.TabIndex = 139
        Me.cboEnable5.Visible = False
        '
        'cboSchedule4
        '
        Me.cboSchedule4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSchedule4.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboSchedule4.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboSchedule4.ItemHeight = 18
        Me.cboSchedule4.Location = New System.Drawing.Point(477, 168)
        Me.cboSchedule4.MaxDropDownItems = 10
        Me.cboSchedule4.Name = "cboSchedule4"
        Me.cboSchedule4.Size = New System.Drawing.Size(169, 26)
        Me.cboSchedule4.TabIndex = 137
        Me.cboSchedule4.Visible = False
        '
        'cboEnable4
        '
        Me.cboEnable4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboEnable4.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboEnable4.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboEnable4.ItemHeight = 18
        Me.cboEnable4.Location = New System.Drawing.Point(342, 168)
        Me.cboEnable4.MaxDropDownItems = 10
        Me.cboEnable4.Name = "cboEnable4"
        Me.cboEnable4.Size = New System.Drawing.Size(111, 26)
        Me.cboEnable4.TabIndex = 136
        Me.cboEnable4.Visible = False
        '
        'cboSchedule3
        '
        Me.cboSchedule3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSchedule3.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboSchedule3.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboSchedule3.ItemHeight = 18
        Me.cboSchedule3.Location = New System.Drawing.Point(477, 136)
        Me.cboSchedule3.MaxDropDownItems = 10
        Me.cboSchedule3.Name = "cboSchedule3"
        Me.cboSchedule3.Size = New System.Drawing.Size(169, 26)
        Me.cboSchedule3.TabIndex = 134
        Me.cboSchedule3.Visible = False
        '
        'cboEnable3
        '
        Me.cboEnable3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboEnable3.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboEnable3.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboEnable3.ItemHeight = 18
        Me.cboEnable3.Location = New System.Drawing.Point(342, 136)
        Me.cboEnable3.MaxDropDownItems = 10
        Me.cboEnable3.Name = "cboEnable3"
        Me.cboEnable3.Size = New System.Drawing.Size(111, 26)
        Me.cboEnable3.TabIndex = 133
        Me.cboEnable3.Visible = False
        '
        'cboSchedule2
        '
        Me.cboSchedule2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSchedule2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboSchedule2.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboSchedule2.ItemHeight = 18
        Me.cboSchedule2.Location = New System.Drawing.Point(477, 104)
        Me.cboSchedule2.MaxDropDownItems = 10
        Me.cboSchedule2.Name = "cboSchedule2"
        Me.cboSchedule2.Size = New System.Drawing.Size(169, 26)
        Me.cboSchedule2.TabIndex = 131
        Me.cboSchedule2.Visible = False
        '
        'cboSchedule1
        '
        Me.cboSchedule1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSchedule1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboSchedule1.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboSchedule1.ItemHeight = 18
        Me.cboSchedule1.Location = New System.Drawing.Point(477, 72)
        Me.cboSchedule1.MaxDropDownItems = 10
        Me.cboSchedule1.Name = "cboSchedule1"
        Me.cboSchedule1.Size = New System.Drawing.Size(169, 26)
        Me.cboSchedule1.TabIndex = 130
        Me.cboSchedule1.Visible = False
        '
        'cboEnable2
        '
        Me.cboEnable2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboEnable2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboEnable2.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboEnable2.ItemHeight = 18
        Me.cboEnable2.Location = New System.Drawing.Point(342, 104)
        Me.cboEnable2.MaxDropDownItems = 10
        Me.cboEnable2.Name = "cboEnable2"
        Me.cboEnable2.Size = New System.Drawing.Size(111, 26)
        Me.cboEnable2.TabIndex = 129
        Me.cboEnable2.Visible = False
        '
        'cboEnable1
        '
        Me.cboEnable1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboEnable1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboEnable1.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboEnable1.ItemHeight = 18
        Me.cboEnable1.Location = New System.Drawing.Point(342, 72)
        Me.cboEnable1.MaxDropDownItems = 10
        Me.cboEnable1.Name = "cboEnable1"
        Me.cboEnable1.Size = New System.Drawing.Size(111, 26)
        Me.cboEnable1.TabIndex = 128
        Me.cboEnable1.Visible = False
        '
        'ftbArchive
        '
        Me.ftbArchive.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbArchive.ForeColor = System.Drawing.Color.Red
        Me.ftbArchive.Location = New System.Drawing.Point(156, 104)
        Me.ftbArchive.MaxLength = 2
        Me.ftbArchive.Name = "ftbArchive"
        Me.ftbArchive.NumericOnly = True
        Me.ftbArchive.Size = New System.Drawing.Size(48, 22)
        Me.ftbArchive.TabIndex = 127
        Me.ftbArchive.TabStop = False
        Me.ftbArchive.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.ftbArchive.WordWrap = False
        '
        'ftbDMONFiles
        '
        Me.ftbDMONFiles.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbDMONFiles.ForeColor = System.Drawing.Color.Red
        Me.ftbDMONFiles.Location = New System.Drawing.Point(156, 78)
        Me.ftbDMONFiles.MaxLength = 2
        Me.ftbDMONFiles.Name = "ftbDMONFiles"
        Me.ftbDMONFiles.NumericOnly = True
        Me.ftbDMONFiles.Size = New System.Drawing.Size(48, 22)
        Me.ftbDMONFiles.TabIndex = 126
        Me.ftbDMONFiles.TabStop = False
        Me.ftbDMONFiles.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.ftbDMONFiles.WordWrap = False
        '
        'lblArchive
        '
        Me.lblArchive.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblArchive.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblArchive.Location = New System.Drawing.Point(6, 107)
        Me.lblArchive.Name = "lblArchive"
        Me.lblArchive.Size = New System.Drawing.Size(144, 19)
        Me.lblArchive.TabIndex = 36
        Me.lblArchive.Tag = "11"
        Me.lblArchive.Text = "lblArchive"
        Me.lblArchive.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblDMONFiles
        '
        Me.lblDMONFiles.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblDMONFiles.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblDMONFiles.Location = New System.Drawing.Point(6, 78)
        Me.lblDMONFiles.Name = "lblDMONFiles"
        Me.lblDMONFiles.Size = New System.Drawing.Size(144, 19)
        Me.lblDMONFiles.TabIndex = 35
        Me.lblDMONFiles.Tag = "11"
        Me.lblDMONFiles.Text = "lblDMONFiles"
        Me.lblDMONFiles.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblDaysToKeep
        '
        Me.lblDaysToKeep.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblDaysToKeep.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblDaysToKeep.Location = New System.Drawing.Point(6, 50)
        Me.lblDaysToKeep.Name = "lblDaysToKeep"
        Me.lblDaysToKeep.Size = New System.Drawing.Size(144, 19)
        Me.lblDaysToKeep.TabIndex = 34
        Me.lblDaysToKeep.Tag = "11"
        Me.lblDaysToKeep.Text = "lblDaysToKeep"
        Me.lblDaysToKeep.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblScheduleCap
        '
        Me.lblScheduleCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblScheduleCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblScheduleCap.Location = New System.Drawing.Point(477, 50)
        Me.lblScheduleCap.Name = "lblScheduleCap"
        Me.lblScheduleCap.Size = New System.Drawing.Size(169, 19)
        Me.lblScheduleCap.TabIndex = 31
        Me.lblScheduleCap.Tag = "11"
        Me.lblScheduleCap.Text = "lblScheduleCap"
        Me.lblScheduleCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblEnableCap
        '
        Me.lblEnableCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblEnableCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblEnableCap.Location = New System.Drawing.Point(224, 50)
        Me.lblEnableCap.Name = "lblEnableCap"
        Me.lblEnableCap.Size = New System.Drawing.Size(111, 19)
        Me.lblEnableCap.TabIndex = 30
        Me.lblEnableCap.Tag = "11"
        Me.lblEnableCap.Text = "lblEnableCap"
        Me.lblEnableCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblJobOrCC
        '
        Me.lblJobOrCC.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblJobOrCC.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblJobOrCC.Location = New System.Drawing.Point(341, 50)
        Me.lblJobOrCC.Name = "lblJobOrCC"
        Me.lblJobOrCC.Size = New System.Drawing.Size(112, 19)
        Me.lblJobOrCC.TabIndex = 29
        Me.lblJobOrCC.Tag = "11"
        Me.lblJobOrCC.Text = "lblJobOrCC"
        Me.lblJobOrCC.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'tabSchedules
        '
        Me.tabSchedules.AutoScroll = True
        Me.tabSchedules.BackColor = System.Drawing.SystemColors.Control
        Me.tabSchedules.Controls.Add(Me.lblSaveWarning)
        Me.tabSchedules.Controls.Add(Me.cboRecordMode)
        Me.tabSchedules.Controls.Add(Me.lblRecordMode)
        Me.tabSchedules.Controls.Add(Me.ftbStopValue)
        Me.tabSchedules.Controls.Add(Me.cboStopCond)
        Me.tabSchedules.Controls.Add(Me.chkStopItem)
        Me.tabSchedules.Controls.Add(Me.cboStopItem)
        Me.tabSchedules.Controls.Add(Me.ftbStartValue)
        Me.tabSchedules.Controls.Add(Me.cboStartCond)
        Me.tabSchedules.Controls.Add(Me.chkStartItem)
        Me.tabSchedules.Controls.Add(Me.cboStartItem)
        Me.tabSchedules.Controls.Add(Me.ftbRecordAct)
        Me.tabSchedules.Controls.Add(Me.ftbRecordReq)
        Me.tabSchedules.Controls.Add(Me.lblRecord)
        Me.tabSchedules.Controls.Add(Me.lblMonitor)
        Me.tabSchedules.Controls.Add(Me.lblSample)
        Me.tabSchedules.Controls.Add(Me.ftbMonitorAct)
        Me.tabSchedules.Controls.Add(Me.ftbMonitorReq)
        Me.tabSchedules.Controls.Add(Me.ftbSampleAct)
        Me.tabSchedules.Controls.Add(Me.ftbSampleReq)
        Me.tabSchedules.Controls.Add(Me.lblAct)
        Me.tabSchedules.Controls.Add(Me.lblReq)
        Me.tabSchedules.Controls.Add(Me.lblFrequency)
        Me.tabSchedules.Controls.Add(Me.lblKB)
        Me.tabSchedules.Controls.Add(Me.ftbFileSize)
        Me.tabSchedules.Controls.Add(Me.lblFileSize)
        Me.tabSchedules.Controls.Add(Me.ftbFileIndex)
        Me.tabSchedules.Controls.Add(Me.lblFileIndex)
        Me.tabSchedules.Controls.Add(Me.cboFileDevice)
        Me.tabSchedules.Controls.Add(Me.lblFileDevice)
        Me.tabSchedules.Controls.Add(Me.ftbNumItems)
        Me.tabSchedules.Controls.Add(Me.lblNumItems)
        Me.tabSchedules.Controls.Add(Me.cboItem10)
        Me.tabSchedules.Controls.Add(Me.lblItem10)
        Me.tabSchedules.Controls.Add(Me.cboItem9)
        Me.tabSchedules.Controls.Add(Me.lblItem9)
        Me.tabSchedules.Controls.Add(Me.cboItem8)
        Me.tabSchedules.Controls.Add(Me.lblItem8)
        Me.tabSchedules.Controls.Add(Me.cboItem7)
        Me.tabSchedules.Controls.Add(Me.lblItem7)
        Me.tabSchedules.Controls.Add(Me.cboItem6)
        Me.tabSchedules.Controls.Add(Me.lblItem6)
        Me.tabSchedules.Controls.Add(Me.cboItem5)
        Me.tabSchedules.Controls.Add(Me.lblItem5)
        Me.tabSchedules.Controls.Add(Me.cboItem4)
        Me.tabSchedules.Controls.Add(Me.lblItem4)
        Me.tabSchedules.Controls.Add(Me.cboItem3)
        Me.tabSchedules.Controls.Add(Me.lblItem3)
        Me.tabSchedules.Controls.Add(Me.cboItem2)
        Me.tabSchedules.Controls.Add(Me.lblItem2)
        Me.tabSchedules.Controls.Add(Me.cboItem1)
        Me.tabSchedules.Controls.Add(Me.lblItem1)
        Me.tabSchedules.Controls.Add(Me.ftbFileName)
        Me.tabSchedules.Controls.Add(Me.lblFileName)
        Me.tabSchedules.Controls.Add(Me.ftbComment)
        Me.tabSchedules.Controls.Add(Me.lblComment)
        Me.tabSchedules.Location = New System.Drawing.Point(4, 23)
        Me.tabSchedules.Name = "tabSchedules"
        Me.tabSchedules.Padding = New System.Windows.Forms.Padding(3)
        Me.tabSchedules.Size = New System.Drawing.Size(874, 515)
        Me.tabSchedules.TabIndex = 1
        Me.tabSchedules.Text = "tabSchedules"
        '
        'lblSaveWarning
        '
        Me.lblSaveWarning.AutoSize = True
        Me.lblSaveWarning.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSaveWarning.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblSaveWarning.Location = New System.Drawing.Point(29, 483)
        Me.lblSaveWarning.Name = "lblSaveWarning"
        Me.lblSaveWarning.Size = New System.Drawing.Size(116, 18)
        Me.lblSaveWarning.TabIndex = 184
        Me.lblSaveWarning.Tag = "11"
        Me.lblSaveWarning.Text = "lblSaveWarning"
        Me.lblSaveWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cboRecordMode
        '
        Me.cboRecordMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboRecordMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboRecordMode.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboRecordMode.ItemHeight = 18
        Me.cboRecordMode.Location = New System.Drawing.Point(170, 348)
        Me.cboRecordMode.Name = "cboRecordMode"
        Me.cboRecordMode.Size = New System.Drawing.Size(234, 26)
        Me.cboRecordMode.TabIndex = 182
        '
        'lblRecordMode
        '
        Me.lblRecordMode.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblRecordMode.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblRecordMode.Location = New System.Drawing.Point(17, 352)
        Me.lblRecordMode.Name = "lblRecordMode"
        Me.lblRecordMode.Size = New System.Drawing.Size(140, 19)
        Me.lblRecordMode.TabIndex = 181
        Me.lblRecordMode.Tag = "11"
        Me.lblRecordMode.Text = "lblRecordMode"
        Me.lblRecordMode.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ftbStopValue
        '
        Me.ftbStopValue.BackColor = System.Drawing.Color.White
        Me.ftbStopValue.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbStopValue.ForeColor = System.Drawing.Color.Red
        Me.ftbStopValue.Location = New System.Drawing.Point(524, 426)
        Me.ftbStopValue.Name = "ftbStopValue"
        Me.ftbStopValue.NumericOnly = True
        Me.ftbStopValue.Size = New System.Drawing.Size(53, 22)
        Me.ftbStopValue.TabIndex = 180
        Me.ftbStopValue.TabStop = False
        Me.ftbStopValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.ftbStopValue.WordWrap = False
        '
        'cboStopCond
        '
        Me.cboStopCond.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboStopCond.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboStopCond.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboStopCond.ItemHeight = 18
        Me.cboStopCond.Location = New System.Drawing.Point(422, 424)
        Me.cboStopCond.Name = "cboStopCond"
        Me.cboStopCond.Size = New System.Drawing.Size(81, 26)
        Me.cboStopCond.TabIndex = 179
        '
        'chkStopItem
        '
        Me.chkStopItem.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.chkStopItem.Location = New System.Drawing.Point(32, 424)
        Me.chkStopItem.Name = "chkStopItem"
        Me.chkStopItem.Size = New System.Drawing.Size(138, 26)
        Me.chkStopItem.TabIndex = 178
        Me.chkStopItem.Text = "chkStopItem"
        Me.chkStopItem.UseVisualStyleBackColor = True
        '
        'cboStopItem
        '
        Me.cboStopItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboStopItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboStopItem.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboStopItem.ItemHeight = 18
        Me.cboStopItem.Location = New System.Drawing.Point(170, 424)
        Me.cboStopItem.Name = "cboStopItem"
        Me.cboStopItem.Size = New System.Drawing.Size(234, 26)
        Me.cboStopItem.TabIndex = 177
        '
        'ftbStartValue
        '
        Me.ftbStartValue.BackColor = System.Drawing.Color.White
        Me.ftbStartValue.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbStartValue.ForeColor = System.Drawing.Color.Red
        Me.ftbStartValue.Location = New System.Drawing.Point(524, 394)
        Me.ftbStartValue.Name = "ftbStartValue"
        Me.ftbStartValue.NumericOnly = True
        Me.ftbStartValue.Size = New System.Drawing.Size(53, 22)
        Me.ftbStartValue.TabIndex = 176
        Me.ftbStartValue.TabStop = False
        Me.ftbStartValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.ftbStartValue.WordWrap = False
        '
        'cboStartCond
        '
        Me.cboStartCond.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboStartCond.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboStartCond.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboStartCond.ItemHeight = 18
        Me.cboStartCond.Location = New System.Drawing.Point(422, 392)
        Me.cboStartCond.Name = "cboStartCond"
        Me.cboStartCond.Size = New System.Drawing.Size(81, 26)
        Me.cboStartCond.TabIndex = 175
        '
        'chkStartItem
        '
        Me.chkStartItem.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.chkStartItem.Location = New System.Drawing.Point(32, 392)
        Me.chkStartItem.Name = "chkStartItem"
        Me.chkStartItem.Size = New System.Drawing.Size(138, 26)
        Me.chkStartItem.TabIndex = 174
        Me.chkStartItem.Text = "chkStartItem"
        Me.chkStartItem.UseVisualStyleBackColor = True
        '
        'cboStartItem
        '
        Me.cboStartItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboStartItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboStartItem.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboStartItem.ItemHeight = 18
        Me.cboStartItem.Location = New System.Drawing.Point(170, 392)
        Me.cboStartItem.Name = "cboStartItem"
        Me.cboStartItem.Size = New System.Drawing.Size(234, 26)
        Me.cboStartItem.TabIndex = 173
        '
        'ftbRecordAct
        '
        Me.ftbRecordAct.BackColor = System.Drawing.Color.White
        Me.ftbRecordAct.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbRecordAct.ForeColor = System.Drawing.Color.Red
        Me.ftbRecordAct.Location = New System.Drawing.Point(259, 305)
        Me.ftbRecordAct.Name = "ftbRecordAct"
        Me.ftbRecordAct.NumericOnly = True
        Me.ftbRecordAct.ReadOnly = True
        Me.ftbRecordAct.Size = New System.Drawing.Size(53, 22)
        Me.ftbRecordAct.TabIndex = 171
        Me.ftbRecordAct.TabStop = False
        Me.ftbRecordAct.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.ftbRecordAct.WordWrap = False
        '
        'ftbRecordReq
        '
        Me.ftbRecordReq.BackColor = System.Drawing.Color.White
        Me.ftbRecordReq.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbRecordReq.ForeColor = System.Drawing.Color.Red
        Me.ftbRecordReq.Location = New System.Drawing.Point(170, 305)
        Me.ftbRecordReq.Name = "ftbRecordReq"
        Me.ftbRecordReq.NumericOnly = True
        Me.ftbRecordReq.Size = New System.Drawing.Size(53, 22)
        Me.ftbRecordReq.TabIndex = 170
        Me.ftbRecordReq.TabStop = False
        Me.ftbRecordReq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.ftbRecordReq.WordWrap = False
        '
        'lblRecord
        '
        Me.lblRecord.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblRecord.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblRecord.Location = New System.Drawing.Point(-1, 306)
        Me.lblRecord.Name = "lblRecord"
        Me.lblRecord.Size = New System.Drawing.Size(144, 19)
        Me.lblRecord.TabIndex = 169
        Me.lblRecord.Tag = "11"
        Me.lblRecord.Text = "lblRecord"
        Me.lblRecord.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblMonitor
        '
        Me.lblMonitor.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblMonitor.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblMonitor.Location = New System.Drawing.Point(-1, 277)
        Me.lblMonitor.Name = "lblMonitor"
        Me.lblMonitor.Size = New System.Drawing.Size(144, 19)
        Me.lblMonitor.TabIndex = 168
        Me.lblMonitor.Tag = "11"
        Me.lblMonitor.Text = "lblMonitor"
        Me.lblMonitor.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblSample
        '
        Me.lblSample.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblSample.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblSample.Location = New System.Drawing.Point(-1, 249)
        Me.lblSample.Name = "lblSample"
        Me.lblSample.Size = New System.Drawing.Size(144, 19)
        Me.lblSample.TabIndex = 167
        Me.lblSample.Tag = "11"
        Me.lblSample.Text = "lblSample"
        Me.lblSample.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ftbMonitorAct
        '
        Me.ftbMonitorAct.BackColor = System.Drawing.Color.White
        Me.ftbMonitorAct.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbMonitorAct.ForeColor = System.Drawing.Color.Red
        Me.ftbMonitorAct.Location = New System.Drawing.Point(259, 274)
        Me.ftbMonitorAct.Name = "ftbMonitorAct"
        Me.ftbMonitorAct.NumericOnly = True
        Me.ftbMonitorAct.ReadOnly = True
        Me.ftbMonitorAct.Size = New System.Drawing.Size(53, 22)
        Me.ftbMonitorAct.TabIndex = 166
        Me.ftbMonitorAct.TabStop = False
        Me.ftbMonitorAct.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.ftbMonitorAct.WordWrap = False
        '
        'ftbMonitorReq
        '
        Me.ftbMonitorReq.BackColor = System.Drawing.Color.White
        Me.ftbMonitorReq.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbMonitorReq.ForeColor = System.Drawing.Color.Red
        Me.ftbMonitorReq.Location = New System.Drawing.Point(170, 274)
        Me.ftbMonitorReq.Name = "ftbMonitorReq"
        Me.ftbMonitorReq.NumericOnly = True
        Me.ftbMonitorReq.Size = New System.Drawing.Size(53, 22)
        Me.ftbMonitorReq.TabIndex = 165
        Me.ftbMonitorReq.TabStop = False
        Me.ftbMonitorReq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.ftbMonitorReq.WordWrap = False
        '
        'ftbSampleAct
        '
        Me.ftbSampleAct.BackColor = System.Drawing.Color.White
        Me.ftbSampleAct.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbSampleAct.ForeColor = System.Drawing.Color.Red
        Me.ftbSampleAct.Location = New System.Drawing.Point(259, 246)
        Me.ftbSampleAct.Name = "ftbSampleAct"
        Me.ftbSampleAct.NumericOnly = True
        Me.ftbSampleAct.ReadOnly = True
        Me.ftbSampleAct.Size = New System.Drawing.Size(53, 22)
        Me.ftbSampleAct.TabIndex = 164
        Me.ftbSampleAct.TabStop = False
        Me.ftbSampleAct.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.ftbSampleAct.WordWrap = False
        '
        'ftbSampleReq
        '
        Me.ftbSampleReq.BackColor = System.Drawing.Color.White
        Me.ftbSampleReq.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbSampleReq.ForeColor = System.Drawing.Color.Red
        Me.ftbSampleReq.Location = New System.Drawing.Point(170, 246)
        Me.ftbSampleReq.Name = "ftbSampleReq"
        Me.ftbSampleReq.NumericOnly = True
        Me.ftbSampleReq.Size = New System.Drawing.Size(53, 22)
        Me.ftbSampleReq.TabIndex = 163
        Me.ftbSampleReq.TabStop = False
        Me.ftbSampleReq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.ftbSampleReq.WordWrap = False
        '
        'lblAct
        '
        Me.lblAct.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblAct.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblAct.Location = New System.Drawing.Point(245, 224)
        Me.lblAct.Name = "lblAct"
        Me.lblAct.Size = New System.Drawing.Size(81, 19)
        Me.lblAct.TabIndex = 162
        Me.lblAct.Tag = "11"
        Me.lblAct.Text = "lblAct"
        Me.lblAct.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblReq
        '
        Me.lblReq.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblReq.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblReq.Location = New System.Drawing.Point(156, 224)
        Me.lblReq.Name = "lblReq"
        Me.lblReq.Size = New System.Drawing.Size(81, 19)
        Me.lblReq.TabIndex = 161
        Me.lblReq.Tag = "11"
        Me.lblReq.Text = "lblReq"
        Me.lblReq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblFrequency
        '
        Me.lblFrequency.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblFrequency.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblFrequency.Location = New System.Drawing.Point(-1, 224)
        Me.lblFrequency.Name = "lblFrequency"
        Me.lblFrequency.Size = New System.Drawing.Size(144, 19)
        Me.lblFrequency.TabIndex = 160
        Me.lblFrequency.Tag = "11"
        Me.lblFrequency.Text = "lblFrequency"
        Me.lblFrequency.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblKB
        '
        Me.lblKB.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblKB.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblKB.Location = New System.Drawing.Point(215, 141)
        Me.lblKB.Name = "lblKB"
        Me.lblKB.Size = New System.Drawing.Size(57, 19)
        Me.lblKB.TabIndex = 159
        Me.lblKB.Tag = "11"
        Me.lblKB.Text = "lblKB"
        Me.lblKB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ftbFileSize
        '
        Me.ftbFileSize.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbFileSize.ForeColor = System.Drawing.Color.Red
        Me.ftbFileSize.Location = New System.Drawing.Point(156, 141)
        Me.ftbFileSize.Name = "ftbFileSize"
        Me.ftbFileSize.NumericOnly = True
        Me.ftbFileSize.Size = New System.Drawing.Size(53, 22)
        Me.ftbFileSize.TabIndex = 158
        Me.ftbFileSize.TabStop = False
        Me.ftbFileSize.WordWrap = False
        '
        'lblFileSize
        '
        Me.lblFileSize.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblFileSize.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblFileSize.Location = New System.Drawing.Point(-1, 141)
        Me.lblFileSize.Name = "lblFileSize"
        Me.lblFileSize.Size = New System.Drawing.Size(144, 19)
        Me.lblFileSize.TabIndex = 157
        Me.lblFileSize.Tag = "11"
        Me.lblFileSize.Text = "lblFileSize"
        Me.lblFileSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ftbFileIndex
        '
        Me.ftbFileIndex.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbFileIndex.ForeColor = System.Drawing.Color.Red
        Me.ftbFileIndex.Location = New System.Drawing.Point(156, 109)
        Me.ftbFileIndex.MaxLength = 3
        Me.ftbFileIndex.Name = "ftbFileIndex"
        Me.ftbFileIndex.NumericOnly = True
        Me.ftbFileIndex.Size = New System.Drawing.Size(53, 22)
        Me.ftbFileIndex.TabIndex = 156
        Me.ftbFileIndex.TabStop = False
        Me.ftbFileIndex.WordWrap = False
        '
        'lblFileIndex
        '
        Me.lblFileIndex.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblFileIndex.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblFileIndex.Location = New System.Drawing.Point(-1, 109)
        Me.lblFileIndex.Name = "lblFileIndex"
        Me.lblFileIndex.Size = New System.Drawing.Size(144, 19)
        Me.lblFileIndex.TabIndex = 155
        Me.lblFileIndex.Tag = "11"
        Me.lblFileIndex.Text = "lblFileIndex"
        Me.lblFileIndex.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboFileDevice
        '
        Me.cboFileDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboFileDevice.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboFileDevice.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboFileDevice.ItemHeight = 18
        Me.cboFileDevice.Location = New System.Drawing.Point(156, 41)
        Me.cboFileDevice.Name = "cboFileDevice"
        Me.cboFileDevice.Size = New System.Drawing.Size(81, 26)
        Me.cboFileDevice.TabIndex = 154
        '
        'lblFileDevice
        '
        Me.lblFileDevice.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblFileDevice.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblFileDevice.Location = New System.Drawing.Point(3, 48)
        Me.lblFileDevice.Name = "lblFileDevice"
        Me.lblFileDevice.Size = New System.Drawing.Size(140, 19)
        Me.lblFileDevice.TabIndex = 153
        Me.lblFileDevice.Tag = "11"
        Me.lblFileDevice.Text = "lblFileDevice"
        Me.lblFileDevice.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ftbNumItems
        '
        Me.ftbNumItems.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbNumItems.ForeColor = System.Drawing.Color.Red
        Me.ftbNumItems.Location = New System.Drawing.Point(156, 173)
        Me.ftbNumItems.Name = "ftbNumItems"
        Me.ftbNumItems.NumericOnly = True
        Me.ftbNumItems.Size = New System.Drawing.Size(53, 22)
        Me.ftbNumItems.TabIndex = 152
        Me.ftbNumItems.TabStop = False
        Me.ftbNumItems.WordWrap = False
        '
        'lblNumItems
        '
        Me.lblNumItems.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblNumItems.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblNumItems.Location = New System.Drawing.Point(-1, 173)
        Me.lblNumItems.Name = "lblNumItems"
        Me.lblNumItems.Size = New System.Drawing.Size(144, 19)
        Me.lblNumItems.TabIndex = 151
        Me.lblNumItems.Tag = "11"
        Me.lblNumItems.Text = "lblNumItems"
        Me.lblNumItems.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboItem10
        '
        Me.cboItem10.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboItem10.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboItem10.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboItem10.ItemHeight = 18
        Me.cboItem10.Location = New System.Drawing.Point(422, 301)
        Me.cboItem10.Name = "cboItem10"
        Me.cboItem10.Size = New System.Drawing.Size(234, 26)
        Me.cboItem10.TabIndex = 150
        '
        'lblItem10
        '
        Me.lblItem10.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblItem10.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblItem10.Location = New System.Drawing.Point(332, 304)
        Me.lblItem10.Name = "lblItem10"
        Me.lblItem10.Size = New System.Drawing.Size(84, 19)
        Me.lblItem10.TabIndex = 149
        Me.lblItem10.Tag = "11"
        Me.lblItem10.Text = "lblItem10"
        Me.lblItem10.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboItem9
        '
        Me.cboItem9.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboItem9.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboItem9.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboItem9.ItemHeight = 18
        Me.cboItem9.Location = New System.Drawing.Point(422, 269)
        Me.cboItem9.Name = "cboItem9"
        Me.cboItem9.Size = New System.Drawing.Size(234, 26)
        Me.cboItem9.TabIndex = 148
        '
        'lblItem9
        '
        Me.lblItem9.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblItem9.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblItem9.Location = New System.Drawing.Point(332, 272)
        Me.lblItem9.Name = "lblItem9"
        Me.lblItem9.Size = New System.Drawing.Size(84, 19)
        Me.lblItem9.TabIndex = 147
        Me.lblItem9.Tag = "11"
        Me.lblItem9.Text = "lblItem9"
        Me.lblItem9.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboItem8
        '
        Me.cboItem8.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboItem8.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboItem8.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboItem8.ItemHeight = 18
        Me.cboItem8.Location = New System.Drawing.Point(422, 237)
        Me.cboItem8.Name = "cboItem8"
        Me.cboItem8.Size = New System.Drawing.Size(234, 26)
        Me.cboItem8.TabIndex = 146
        '
        'lblItem8
        '
        Me.lblItem8.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblItem8.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblItem8.Location = New System.Drawing.Point(332, 240)
        Me.lblItem8.Name = "lblItem8"
        Me.lblItem8.Size = New System.Drawing.Size(84, 19)
        Me.lblItem8.TabIndex = 145
        Me.lblItem8.Tag = "11"
        Me.lblItem8.Text = "lblItem8"
        Me.lblItem8.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboItem7
        '
        Me.cboItem7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboItem7.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboItem7.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboItem7.ItemHeight = 18
        Me.cboItem7.Location = New System.Drawing.Point(422, 205)
        Me.cboItem7.Name = "cboItem7"
        Me.cboItem7.Size = New System.Drawing.Size(234, 26)
        Me.cboItem7.TabIndex = 144
        '
        'lblItem7
        '
        Me.lblItem7.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblItem7.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblItem7.Location = New System.Drawing.Point(332, 208)
        Me.lblItem7.Name = "lblItem7"
        Me.lblItem7.Size = New System.Drawing.Size(84, 19)
        Me.lblItem7.TabIndex = 143
        Me.lblItem7.Tag = "11"
        Me.lblItem7.Text = "lblItem7"
        Me.lblItem7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboItem6
        '
        Me.cboItem6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboItem6.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboItem6.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboItem6.ItemHeight = 18
        Me.cboItem6.Location = New System.Drawing.Point(422, 173)
        Me.cboItem6.Name = "cboItem6"
        Me.cboItem6.Size = New System.Drawing.Size(234, 26)
        Me.cboItem6.TabIndex = 142
        '
        'lblItem6
        '
        Me.lblItem6.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblItem6.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblItem6.Location = New System.Drawing.Point(332, 176)
        Me.lblItem6.Name = "lblItem6"
        Me.lblItem6.Size = New System.Drawing.Size(84, 19)
        Me.lblItem6.TabIndex = 141
        Me.lblItem6.Tag = "11"
        Me.lblItem6.Text = "lblItem6"
        Me.lblItem6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboItem5
        '
        Me.cboItem5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboItem5.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboItem5.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboItem5.ItemHeight = 18
        Me.cboItem5.Location = New System.Drawing.Point(422, 141)
        Me.cboItem5.Name = "cboItem5"
        Me.cboItem5.Size = New System.Drawing.Size(234, 26)
        Me.cboItem5.TabIndex = 140
        '
        'lblItem5
        '
        Me.lblItem5.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblItem5.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblItem5.Location = New System.Drawing.Point(332, 144)
        Me.lblItem5.Name = "lblItem5"
        Me.lblItem5.Size = New System.Drawing.Size(84, 19)
        Me.lblItem5.TabIndex = 139
        Me.lblItem5.Tag = "11"
        Me.lblItem5.Text = "lblItem5"
        Me.lblItem5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboItem4
        '
        Me.cboItem4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboItem4.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboItem4.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboItem4.ItemHeight = 18
        Me.cboItem4.Location = New System.Drawing.Point(422, 109)
        Me.cboItem4.Name = "cboItem4"
        Me.cboItem4.Size = New System.Drawing.Size(234, 26)
        Me.cboItem4.TabIndex = 138
        '
        'lblItem4
        '
        Me.lblItem4.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblItem4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblItem4.Location = New System.Drawing.Point(332, 112)
        Me.lblItem4.Name = "lblItem4"
        Me.lblItem4.Size = New System.Drawing.Size(84, 19)
        Me.lblItem4.TabIndex = 137
        Me.lblItem4.Tag = "11"
        Me.lblItem4.Text = "lblItem4"
        Me.lblItem4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboItem3
        '
        Me.cboItem3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboItem3.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboItem3.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboItem3.ItemHeight = 18
        Me.cboItem3.Location = New System.Drawing.Point(422, 77)
        Me.cboItem3.Name = "cboItem3"
        Me.cboItem3.Size = New System.Drawing.Size(234, 26)
        Me.cboItem3.TabIndex = 136
        '
        'lblItem3
        '
        Me.lblItem3.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblItem3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblItem3.Location = New System.Drawing.Point(332, 80)
        Me.lblItem3.Name = "lblItem3"
        Me.lblItem3.Size = New System.Drawing.Size(84, 19)
        Me.lblItem3.TabIndex = 135
        Me.lblItem3.Tag = "11"
        Me.lblItem3.Text = "lblItem3"
        Me.lblItem3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboItem2
        '
        Me.cboItem2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboItem2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboItem2.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboItem2.ItemHeight = 18
        Me.cboItem2.Location = New System.Drawing.Point(422, 45)
        Me.cboItem2.Name = "cboItem2"
        Me.cboItem2.Size = New System.Drawing.Size(234, 26)
        Me.cboItem2.TabIndex = 134
        '
        'lblItem2
        '
        Me.lblItem2.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblItem2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblItem2.Location = New System.Drawing.Point(332, 48)
        Me.lblItem2.Name = "lblItem2"
        Me.lblItem2.Size = New System.Drawing.Size(84, 19)
        Me.lblItem2.TabIndex = 133
        Me.lblItem2.Tag = "11"
        Me.lblItem2.Text = "lblItem2"
        Me.lblItem2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboItem1
        '
        Me.cboItem1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboItem1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboItem1.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboItem1.ItemHeight = 18
        Me.cboItem1.Location = New System.Drawing.Point(422, 12)
        Me.cboItem1.Name = "cboItem1"
        Me.cboItem1.Size = New System.Drawing.Size(234, 26)
        Me.cboItem1.TabIndex = 132
        '
        'lblItem1
        '
        Me.lblItem1.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblItem1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblItem1.Location = New System.Drawing.Point(332, 15)
        Me.lblItem1.Name = "lblItem1"
        Me.lblItem1.Size = New System.Drawing.Size(84, 19)
        Me.lblItem1.TabIndex = 131
        Me.lblItem1.Tag = "11"
        Me.lblItem1.Text = "lblItem1"
        Me.lblItem1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ftbFileName
        '
        Me.ftbFileName.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbFileName.ForeColor = System.Drawing.Color.Red
        Me.ftbFileName.Location = New System.Drawing.Point(156, 77)
        Me.ftbFileName.Name = "ftbFileName"
        Me.ftbFileName.NumericOnly = False
        Me.ftbFileName.Size = New System.Drawing.Size(162, 22)
        Me.ftbFileName.TabIndex = 130
        Me.ftbFileName.TabStop = False
        Me.ftbFileName.WordWrap = False
        '
        'lblFileName
        '
        Me.lblFileName.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblFileName.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblFileName.Location = New System.Drawing.Point(-1, 77)
        Me.lblFileName.Name = "lblFileName"
        Me.lblFileName.Size = New System.Drawing.Size(144, 19)
        Me.lblFileName.TabIndex = 129
        Me.lblFileName.Tag = "11"
        Me.lblFileName.Text = "lblFileName"
        Me.lblFileName.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ftbComment
        '
        Me.ftbComment.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbComment.ForeColor = System.Drawing.Color.Red
        Me.ftbComment.Location = New System.Drawing.Point(156, 14)
        Me.ftbComment.Name = "ftbComment"
        Me.ftbComment.NumericOnly = False
        Me.ftbComment.Size = New System.Drawing.Size(162, 22)
        Me.ftbComment.TabIndex = 128
        Me.ftbComment.TabStop = False
        Me.ftbComment.WordWrap = False
        '
        'lblComment
        '
        Me.lblComment.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblComment.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblComment.Location = New System.Drawing.Point(-1, 14)
        Me.lblComment.Name = "lblComment"
        Me.lblComment.Size = New System.Drawing.Size(144, 19)
        Me.lblComment.TabIndex = 127
        Me.lblComment.Tag = "11"
        Me.lblComment.Text = "lblComment"
        Me.lblComment.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'tabItems
        '
        Me.tabItems.AutoScroll = True
        Me.tabItems.BackColor = System.Drawing.SystemColors.Control
        Me.tabItems.Controls.Add(Me.btnBrowse)
        Me.tabItems.Controls.Add(Me.lblPortNumComment)
        Me.tabItems.Controls.Add(Me.btnValidate)
        Me.tabItems.Controls.Add(Me.lblSaveWarning1)
        Me.tabItems.Controls.Add(Me.lblA)
        Me.tabItems.Controls.Add(Me.lblG)
        Me.tabItems.Controls.Add(Me.ftbAxis)
        Me.tabItems.Controls.Add(Me.ftbGroup)
        Me.tabItems.Controls.Add(Me.lblAxis)
        Me.tabItems.Controls.Add(Me.ftbIntercept)
        Me.tabItems.Controls.Add(Me.lblIntercept)
        Me.tabItems.Controls.Add(Me.ftbSlope)
        Me.tabItems.Controls.Add(Me.lblSlope)
        Me.tabItems.Controls.Add(Me.ftbUnits)
        Me.tabItems.Controls.Add(Me.lblUnits)
        Me.tabItems.Controls.Add(Me.ftbDesc)
        Me.tabItems.Controls.Add(Me.lblDesc)
        Me.tabItems.Controls.Add(Me.ftbVarName)
        Me.tabItems.Controls.Add(Me.lblVarName)
        Me.tabItems.Controls.Add(Me.lblIoType)
        Me.tabItems.Controls.Add(Me.cboIoType)
        Me.tabItems.Controls.Add(Me.lblType)
        Me.tabItems.Controls.Add(Me.cboType)
        Me.tabItems.Controls.Add(Me.ftbPrgName)
        Me.tabItems.Controls.Add(Me.lblPrgName)
        Me.tabItems.Controls.Add(Me.ftbPortNum)
        Me.tabItems.Controls.Add(Me.lblPortNum)
        Me.tabItems.Location = New System.Drawing.Point(4, 23)
        Me.tabItems.Name = "tabItems"
        Me.tabItems.Size = New System.Drawing.Size(874, 515)
        Me.tabItems.TabIndex = 2
        Me.tabItems.Text = "tabItems"
        '
        'btnBrowse
        '
        Me.btnBrowse.Location = New System.Drawing.Point(310, 29)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(111, 20)
        Me.btnBrowse.TabIndex = 187
        Me.btnBrowse.Text = "btnBrowse"
        Me.btnBrowse.UseVisualStyleBackColor = True
        '
        'lblPortNumComment
        '
        Me.lblPortNumComment.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPortNumComment.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblPortNumComment.Location = New System.Drawing.Point(225, 107)
        Me.lblPortNumComment.Name = "lblPortNumComment"
        Me.lblPortNumComment.Size = New System.Drawing.Size(299, 19)
        Me.lblPortNumComment.TabIndex = 186
        Me.lblPortNumComment.Tag = "11"
        Me.lblPortNumComment.Text = "lblPortNumComment"
        Me.lblPortNumComment.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnValidate
        '
        Me.btnValidate.Location = New System.Drawing.Point(310, 3)
        Me.btnValidate.Name = "btnValidate"
        Me.btnValidate.Size = New System.Drawing.Size(111, 20)
        Me.btnValidate.TabIndex = 185
        Me.btnValidate.Text = "btnValidate"
        Me.btnValidate.UseVisualStyleBackColor = True
        '
        'lblSaveWarning1
        '
        Me.lblSaveWarning1.AutoSize = True
        Me.lblSaveWarning1.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSaveWarning1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblSaveWarning1.Location = New System.Drawing.Point(56, 389)
        Me.lblSaveWarning1.Name = "lblSaveWarning1"
        Me.lblSaveWarning1.Size = New System.Drawing.Size(116, 18)
        Me.lblSaveWarning1.TabIndex = 184
        Me.lblSaveWarning1.Tag = "11"
        Me.lblSaveWarning1.Text = "lblSaveWarning"
        Me.lblSaveWarning1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblA
        '
        Me.lblA.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblA.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblA.Location = New System.Drawing.Point(222, 143)
        Me.lblA.Name = "lblA"
        Me.lblA.Size = New System.Drawing.Size(22, 19)
        Me.lblA.TabIndex = 152
        Me.lblA.Tag = "11"
        Me.lblA.Text = "a"
        Me.lblA.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblG
        '
        Me.lblG.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblG.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblG.Location = New System.Drawing.Point(163, 143)
        Me.lblG.Name = "lblG"
        Me.lblG.Size = New System.Drawing.Size(28, 19)
        Me.lblG.TabIndex = 151
        Me.lblG.Tag = "11"
        Me.lblG.Text = "g"
        Me.lblG.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ftbAxis
        '
        Me.ftbAxis.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbAxis.ForeColor = System.Drawing.Color.Red
        Me.ftbAxis.Location = New System.Drawing.Point(247, 141)
        Me.ftbAxis.Name = "ftbAxis"
        Me.ftbAxis.NumericOnly = True
        Me.ftbAxis.Size = New System.Drawing.Size(22, 22)
        Me.ftbAxis.TabIndex = 150
        Me.ftbAxis.TabStop = False
        Me.ftbAxis.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.ftbAxis.WordWrap = False
        '
        'ftbGroup
        '
        Me.ftbGroup.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbGroup.ForeColor = System.Drawing.Color.Red
        Me.ftbGroup.Location = New System.Drawing.Point(194, 141)
        Me.ftbGroup.Name = "ftbGroup"
        Me.ftbGroup.NumericOnly = True
        Me.ftbGroup.Size = New System.Drawing.Size(22, 22)
        Me.ftbGroup.TabIndex = 149
        Me.ftbGroup.TabStop = False
        Me.ftbGroup.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.ftbGroup.WordWrap = False
        '
        'lblAxis
        '
        Me.lblAxis.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblAxis.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblAxis.Location = New System.Drawing.Point(6, 143)
        Me.lblAxis.Name = "lblAxis"
        Me.lblAxis.Size = New System.Drawing.Size(144, 19)
        Me.lblAxis.TabIndex = 148
        Me.lblAxis.Tag = "11"
        Me.lblAxis.Text = "lblAxis"
        Me.lblAxis.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ftbIntercept
        '
        Me.ftbIntercept.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbIntercept.ForeColor = System.Drawing.Color.Red
        Me.ftbIntercept.Location = New System.Drawing.Point(163, 337)
        Me.ftbIntercept.Name = "ftbIntercept"
        Me.ftbIntercept.NumericOnly = True
        Me.ftbIntercept.Size = New System.Drawing.Size(64, 22)
        Me.ftbIntercept.TabIndex = 147
        Me.ftbIntercept.TabStop = False
        Me.ftbIntercept.WordWrap = False
        '
        'lblIntercept
        '
        Me.lblIntercept.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblIntercept.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblIntercept.Location = New System.Drawing.Point(6, 339)
        Me.lblIntercept.Name = "lblIntercept"
        Me.lblIntercept.Size = New System.Drawing.Size(144, 19)
        Me.lblIntercept.TabIndex = 146
        Me.lblIntercept.Tag = "11"
        Me.lblIntercept.Text = "lblIntercept"
        Me.lblIntercept.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ftbSlope
        '
        Me.ftbSlope.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbSlope.ForeColor = System.Drawing.Color.Red
        Me.ftbSlope.Location = New System.Drawing.Point(163, 304)
        Me.ftbSlope.Name = "ftbSlope"
        Me.ftbSlope.NumericOnly = True
        Me.ftbSlope.Size = New System.Drawing.Size(64, 22)
        Me.ftbSlope.TabIndex = 145
        Me.ftbSlope.TabStop = False
        Me.ftbSlope.WordWrap = False
        '
        'lblSlope
        '
        Me.lblSlope.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblSlope.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblSlope.Location = New System.Drawing.Point(6, 306)
        Me.lblSlope.Name = "lblSlope"
        Me.lblSlope.Size = New System.Drawing.Size(144, 19)
        Me.lblSlope.TabIndex = 144
        Me.lblSlope.Tag = "11"
        Me.lblSlope.Text = "lblSlope"
        Me.lblSlope.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ftbUnits
        '
        Me.ftbUnits.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbUnits.ForeColor = System.Drawing.Color.Red
        Me.ftbUnits.Location = New System.Drawing.Point(163, 271)
        Me.ftbUnits.Name = "ftbUnits"
        Me.ftbUnits.NumericOnly = False
        Me.ftbUnits.Size = New System.Drawing.Size(148, 22)
        Me.ftbUnits.TabIndex = 143
        Me.ftbUnits.TabStop = False
        Me.ftbUnits.WordWrap = False
        '
        'lblUnits
        '
        Me.lblUnits.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblUnits.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblUnits.Location = New System.Drawing.Point(6, 273)
        Me.lblUnits.Name = "lblUnits"
        Me.lblUnits.Size = New System.Drawing.Size(144, 19)
        Me.lblUnits.TabIndex = 142
        Me.lblUnits.Tag = "11"
        Me.lblUnits.Text = "lblUnits"
        Me.lblUnits.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ftbDesc
        '
        Me.ftbDesc.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbDesc.ForeColor = System.Drawing.Color.Red
        Me.ftbDesc.Location = New System.Drawing.Point(163, 238)
        Me.ftbDesc.Name = "ftbDesc"
        Me.ftbDesc.NumericOnly = False
        Me.ftbDesc.Size = New System.Drawing.Size(292, 22)
        Me.ftbDesc.TabIndex = 141
        Me.ftbDesc.TabStop = False
        Me.ftbDesc.WordWrap = False
        '
        'lblDesc
        '
        Me.lblDesc.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblDesc.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblDesc.Location = New System.Drawing.Point(6, 240)
        Me.lblDesc.Name = "lblDesc"
        Me.lblDesc.Size = New System.Drawing.Size(144, 19)
        Me.lblDesc.TabIndex = 140
        Me.lblDesc.Tag = "11"
        Me.lblDesc.Text = "lblDesc"
        Me.lblDesc.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ftbVarName
        '
        Me.ftbVarName.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbVarName.ForeColor = System.Drawing.Color.Red
        Me.ftbVarName.Location = New System.Drawing.Point(163, 205)
        Me.ftbVarName.Name = "ftbVarName"
        Me.ftbVarName.NumericOnly = False
        Me.ftbVarName.Size = New System.Drawing.Size(450, 22)
        Me.ftbVarName.TabIndex = 139
        Me.ftbVarName.TabStop = False
        Me.ftbVarName.WordWrap = False
        '
        'lblVarName
        '
        Me.lblVarName.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblVarName.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblVarName.Location = New System.Drawing.Point(6, 207)
        Me.lblVarName.Name = "lblVarName"
        Me.lblVarName.Size = New System.Drawing.Size(144, 19)
        Me.lblVarName.TabIndex = 138
        Me.lblVarName.Tag = "11"
        Me.lblVarName.Text = "lblVarName"
        Me.lblVarName.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblIoType
        '
        Me.lblIoType.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblIoType.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblIoType.Location = New System.Drawing.Point(3, 59)
        Me.lblIoType.Name = "lblIoType"
        Me.lblIoType.Size = New System.Drawing.Size(144, 19)
        Me.lblIoType.TabIndex = 137
        Me.lblIoType.Tag = "11"
        Me.lblIoType.Text = "lblIoType"
        Me.lblIoType.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboIoType
        '
        Me.cboIoType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboIoType.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboIoType.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboIoType.ItemHeight = 18
        Me.cboIoType.Location = New System.Drawing.Point(163, 56)
        Me.cboIoType.Name = "cboIoType"
        Me.cboIoType.Size = New System.Drawing.Size(148, 26)
        Me.cboIoType.TabIndex = 136
        '
        'lblType
        '
        Me.lblType.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblType.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblType.Location = New System.Drawing.Point(3, 15)
        Me.lblType.Name = "lblType"
        Me.lblType.Size = New System.Drawing.Size(144, 19)
        Me.lblType.TabIndex = 135
        Me.lblType.Tag = "11"
        Me.lblType.Text = "lblType"
        Me.lblType.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboType
        '
        Me.cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboType.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboType.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboType.ItemHeight = 18
        Me.cboType.Location = New System.Drawing.Point(163, 12)
        Me.cboType.Name = "cboType"
        Me.cboType.Size = New System.Drawing.Size(127, 26)
        Me.cboType.TabIndex = 134
        '
        'ftbPrgName
        '
        Me.ftbPrgName.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbPrgName.ForeColor = System.Drawing.Color.Red
        Me.ftbPrgName.Location = New System.Drawing.Point(163, 172)
        Me.ftbPrgName.Name = "ftbPrgName"
        Me.ftbPrgName.NumericOnly = False
        Me.ftbPrgName.Size = New System.Drawing.Size(292, 22)
        Me.ftbPrgName.TabIndex = 132
        Me.ftbPrgName.TabStop = False
        Me.ftbPrgName.WordWrap = False
        '
        'lblPrgName
        '
        Me.lblPrgName.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblPrgName.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblPrgName.Location = New System.Drawing.Point(6, 174)
        Me.lblPrgName.Name = "lblPrgName"
        Me.lblPrgName.Size = New System.Drawing.Size(144, 19)
        Me.lblPrgName.TabIndex = 131
        Me.lblPrgName.Tag = "11"
        Me.lblPrgName.Text = "lblPrgName"
        Me.lblPrgName.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ftbPortNum
        '
        Me.ftbPortNum.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.ftbPortNum.ForeColor = System.Drawing.Color.Red
        Me.ftbPortNum.Location = New System.Drawing.Point(163, 105)
        Me.ftbPortNum.Name = "ftbPortNum"
        Me.ftbPortNum.NumericOnly = True
        Me.ftbPortNum.Size = New System.Drawing.Size(56, 22)
        Me.ftbPortNum.TabIndex = 130
        Me.ftbPortNum.TabStop = False
        Me.ftbPortNum.WordWrap = False
        '
        'lblPortNum
        '
        Me.lblPortNum.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblPortNum.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblPortNum.Location = New System.Drawing.Point(6, 107)
        Me.lblPortNum.Name = "lblPortNum"
        Me.lblPortNum.Size = New System.Drawing.Size(144, 19)
        Me.lblPortNum.TabIndex = 129
        Me.lblPortNum.Tag = "11"
        Me.lblPortNum.Text = "lblPortNum"
        Me.lblPortNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lstStatus
        '
        Me.lstStatus.BackColor = System.Drawing.SystemColors.Info
        Me.lstStatus.ItemHeight = 14
        Me.lstStatus.Location = New System.Drawing.Point(569, 4)
        Me.lstStatus.Name = "lstStatus"
        Me.lstStatus.Size = New System.Drawing.Size(425, 60)
        Me.lstStatus.TabIndex = 11
        Me.lstStatus.Visible = False
        '
        'lblZone
        '
        Me.lblZone.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblZone.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblZone.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblZone.Location = New System.Drawing.Point(17, 8)
        Me.lblZone.Name = "lblZone"
        Me.lblZone.Size = New System.Drawing.Size(160, 19)
        Me.lblZone.TabIndex = 12
        Me.lblZone.Text = "Zone"
        Me.lblZone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblZone.UseMnemonic = False
        Me.lblZone.Visible = False
        '
        'cboZone
        '
        Me.cboZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboZone.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboZone.ItemHeight = 18
        Me.cboZone.Location = New System.Drawing.Point(17, 33)
        Me.cboZone.Name = "cboZone"
        Me.cboZone.Size = New System.Drawing.Size(160, 26)
        Me.cboZone.TabIndex = 10
        Me.cboZone.Visible = False
        '
        'tlsMain
        '
        Me.tlsMain.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.tlsMain.Dock = System.Windows.Forms.DockStyle.None
        Me.tlsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.btnSave, Me.btnPrint, Me.btnUndo, Me.btnChangeLog, Me.btnUtilities})
        Me.tlsMain.Location = New System.Drawing.Point(0, 0)
        Me.tlsMain.MinimumSize = New System.Drawing.Size(0, 50)
        Me.tlsMain.Name = "tlsMain"
        Me.tlsMain.Size = New System.Drawing.Size(918, 60)
        Me.tlsMain.Stretch = True
        Me.tlsMain.TabIndex = 2
        Me.tlsMain.TabStop = True
        '
        'btnClose
        '
        Me.btnClose.AutoSize = False
        Me.btnClose.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnClose.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(70, 57)
        Me.btnClose.Text = "Close"
        Me.btnClose.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'btnSave
        '
        Me.btnSave.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnSave.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(54, 57)
        Me.btnSave.Text = "?Save?"
        Me.btnSave.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'btnPrint
        '
        Me.btnPrint.DropDownButtonWidth = 13
        Me.btnPrint.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuPrintAll, Me.mnuPrint, Me.mnuPrintPreview, Me.mnuPageSetup, Me.mnuPrintFile, Me.mnuPrintOptions})
        Me.btnPrint.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnPrint.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnPrint.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(53, 57)
        Me.btnPrint.Text = "Print"
        Me.btnPrint.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'mnuPrintAll
        '
        Me.mnuPrintAll.Checked = True
        Me.mnuPrintAll.CheckOnClick = True
        Me.mnuPrintAll.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuPrintAll.Name = "mnuPrintAll"
        Me.mnuPrintAll.Size = New System.Drawing.Size(173, 22)
        Me.mnuPrintAll.Text = "mnuPrintAll"
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
        Me.mnuPrintPreview.Text = "Print Preview"
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
        'btnUndo
        '
        Me.btnUndo.AutoSize = False
        Me.btnUndo.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnUndo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnUndo.Name = "btnUndo"
        Me.btnUndo.Size = New System.Drawing.Size(70, 57)
        Me.btnUndo.Text = "Undo"
        Me.btnUndo.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnUndo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'btnChangeLog
        '
        Me.btnChangeLog.DropDownButtonWidth = 13
        Me.btnChangeLog.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuLast24, Me.mnuLast7, Me.mnuAllChanges})
        Me.btnChangeLog.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnChangeLog.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnChangeLog.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnChangeLog.Name = "btnChangeLog"
        Me.btnChangeLog.Size = New System.Drawing.Size(95, 57)
        Me.btnChangeLog.Text = "Change Log"
        Me.btnChangeLog.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnChangeLog.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'mnuLast24
        '
        Me.mnuLast24.Name = "mnuLast24"
        Me.mnuLast24.Size = New System.Drawing.Size(157, 22)
        Me.mnuLast24.Text = "Last 24 Hours"
        '
        'mnuLast7
        '
        Me.mnuLast7.Name = "mnuLast7"
        Me.mnuLast7.Size = New System.Drawing.Size(157, 22)
        Me.mnuLast7.Text = "Last 7 Days"
        '
        'mnuAllChanges
        '
        Me.mnuAllChanges.Name = "mnuAllChanges"
        Me.mnuAllChanges.Size = New System.Drawing.Size(157, 22)
        Me.mnuAllChanges.Text = "All Changes"
        '
        'btnUtilities
        '
        Me.btnUtilities.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuExport, Me.mnuImport, Me.mnuSaveDefault, Me.mnuRestoreDefault})
        Me.btnUtilities.Image = CType(resources.GetObject("btnUtilities.Image"), System.Drawing.Image)
        Me.btnUtilities.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnUtilities.Name = "btnUtilities"
        Me.btnUtilities.Size = New System.Drawing.Size(73, 57)
        Me.btnUtilities.Text = "btnUtilities"
        Me.btnUtilities.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'mnuExport
        '
        Me.mnuExport.Name = "mnuExport"
        Me.mnuExport.Size = New System.Drawing.Size(167, 22)
        Me.mnuExport.Text = "mnuExport"
        '
        'mnuImport
        '
        Me.mnuImport.Name = "mnuImport"
        Me.mnuImport.Size = New System.Drawing.Size(167, 22)
        Me.mnuImport.Text = "mnuImport"
        '
        'mnuSaveDefault
        '
        Me.mnuSaveDefault.Name = "mnuSaveDefault"
        Me.mnuSaveDefault.Size = New System.Drawing.Size(167, 22)
        Me.mnuSaveDefault.Text = "mnuSaveDefault"
        '
        'mnuRestoreDefault
        '
        Me.mnuRestoreDefault.Name = "mnuRestoreDefault"
        Me.mnuRestoreDefault.Size = New System.Drawing.Size(167, 22)
        Me.mnuRestoreDefault.Text = "mnuRestoreDefault"
        '
        'pnlTxt
        '
        Me.pnlTxt.MinWidth = 100
        Me.pnlTxt.Name = "pnlTxt"
        '
        'pnlProg
        '
        Me.pnlProg.MinWidth = 100
        Me.pnlProg.Name = "pnlProg"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(918, 704)
        Me.ControlBox = False
        Me.Controls.Add(Me.tscMain)
        Me.Font = New System.Drawing.Font("Arial", 8.25!)
        Me.KeyPreview = True
        Me.Name = "frmMain"
        Me.tscMain.BottomToolStripPanel.ResumeLayout(False)
        Me.tscMain.BottomToolStripPanel.PerformLayout()
        Me.tscMain.ContentPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.PerformLayout()
        Me.tscMain.ResumeLayout(False)
        Me.tscMain.PerformLayout()
        Me.stsStatus.ResumeLayout(False)
        Me.stsStatus.PerformLayout()
        Me.pnlMain.ResumeLayout(False)
        Me.tabMain.ResumeLayout(False)
        Me.tabSystem.ResumeLayout(False)
        Me.tabSystem.PerformLayout()
        Me.tabSchedules.ResumeLayout(False)
        Me.tabSchedules.PerformLayout()
        Me.tabItems.ResumeLayout(False)
        Me.tabItems.PerformLayout()
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
    Friend WithEvents lstStatus As System.Windows.Forms.ListBox
    Friend WithEvents lblZone As System.Windows.Forms.Label
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
    Friend WithEvents btnChangeLog As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuLast7 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAllChanges As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLast24 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents pnlMain As System.Windows.Forms.Panel
    Friend WithEvents tabMain As System.Windows.Forms.TabControl
    Friend WithEvents tabSystem As System.Windows.Forms.TabPage
    Friend WithEvents tabSchedules As System.Windows.Forms.TabPage
    Friend WithEvents tabItems As System.Windows.Forms.TabPage
    Friend WithEvents lblArchive As System.Windows.Forms.Label
    Friend WithEvents lblDMONFiles As System.Windows.Forms.Label
    Friend WithEvents lblDaysToKeep As System.Windows.Forms.Label
    Friend WithEvents lblScheduleCap As System.Windows.Forms.Label
    Friend WithEvents lblEnableCap As System.Windows.Forms.Label
    Friend WithEvents ftbArchive As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbDMONFiles As FocusedTextBox.FocusedTextBox
    Friend WithEvents cboEnable1 As System.Windows.Forms.ComboBox
    Friend WithEvents cboEnable2 As System.Windows.Forms.ComboBox
    Friend WithEvents cboSchedule2 As System.Windows.Forms.ComboBox
    Friend WithEvents cboSchedule1 As System.Windows.Forms.ComboBox
    Friend WithEvents lblParm As System.Windows.Forms.Label
    Friend WithEvents cboParm As System.Windows.Forms.ComboBox
    Friend WithEvents lblRobot As System.Windows.Forms.Label
    Friend WithEvents cboRobot As System.Windows.Forms.ComboBox
    Friend WithEvents btnSave As System.Windows.Forms.ToolStripButton
    Friend WithEvents ftbFileName As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblFileName As System.Windows.Forms.Label
    Friend WithEvents ftbComment As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblComment As System.Windows.Forms.Label
    Friend WithEvents cboFileDevice As System.Windows.Forms.ComboBox
    Friend WithEvents lblFileDevice As System.Windows.Forms.Label
    Friend WithEvents ftbNumItems As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblNumItems As System.Windows.Forms.Label
    Friend WithEvents cboItem10 As System.Windows.Forms.ComboBox
    Friend WithEvents lblItem10 As System.Windows.Forms.Label
    Friend WithEvents cboItem9 As System.Windows.Forms.ComboBox
    Friend WithEvents lblItem9 As System.Windows.Forms.Label
    Friend WithEvents cboItem8 As System.Windows.Forms.ComboBox
    Friend WithEvents lblItem8 As System.Windows.Forms.Label
    Friend WithEvents cboItem7 As System.Windows.Forms.ComboBox
    Friend WithEvents lblItem7 As System.Windows.Forms.Label
    Friend WithEvents cboItem6 As System.Windows.Forms.ComboBox
    Friend WithEvents lblItem6 As System.Windows.Forms.Label
    Friend WithEvents cboItem5 As System.Windows.Forms.ComboBox
    Friend WithEvents lblItem5 As System.Windows.Forms.Label
    Friend WithEvents cboItem4 As System.Windows.Forms.ComboBox
    Friend WithEvents lblItem4 As System.Windows.Forms.Label
    Friend WithEvents cboItem3 As System.Windows.Forms.ComboBox
    Friend WithEvents lblItem3 As System.Windows.Forms.Label
    Friend WithEvents cboItem2 As System.Windows.Forms.ComboBox
    Friend WithEvents lblItem2 As System.Windows.Forms.Label
    Friend WithEvents cboItem1 As System.Windows.Forms.ComboBox
    Friend WithEvents lblItem1 As System.Windows.Forms.Label
    Friend WithEvents ftbFileSize As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblFileSize As System.Windows.Forms.Label
    Friend WithEvents ftbFileIndex As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblFileIndex As System.Windows.Forms.Label
    Friend WithEvents ftbRecordAct As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbRecordReq As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblRecord As System.Windows.Forms.Label
    Friend WithEvents lblMonitor As System.Windows.Forms.Label
    Friend WithEvents lblSample As System.Windows.Forms.Label
    Friend WithEvents ftbMonitorAct As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbMonitorReq As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbSampleAct As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbSampleReq As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblAct As System.Windows.Forms.Label
    Friend WithEvents lblReq As System.Windows.Forms.Label
    Friend WithEvents lblFrequency As System.Windows.Forms.Label
    Friend WithEvents lblKB As System.Windows.Forms.Label
    Friend WithEvents cboStartItem As System.Windows.Forms.ComboBox
    Friend WithEvents chkStartItem As System.Windows.Forms.CheckBox
    Friend WithEvents ftbStopValue As FocusedTextBox.FocusedTextBox
    Friend WithEvents cboStopCond As System.Windows.Forms.ComboBox
    Friend WithEvents chkStopItem As System.Windows.Forms.CheckBox
    Friend WithEvents cboStopItem As System.Windows.Forms.ComboBox
    Friend WithEvents ftbStartValue As FocusedTextBox.FocusedTextBox
    Friend WithEvents cboStartCond As System.Windows.Forms.ComboBox
    Friend WithEvents cboSchedule12 As System.Windows.Forms.ComboBox
    Friend WithEvents cboEnable12 As System.Windows.Forms.ComboBox
    Friend WithEvents cboSchedule11 As System.Windows.Forms.ComboBox
    Friend WithEvents cboEnable11 As System.Windows.Forms.ComboBox
    Friend WithEvents cboSchedule10 As System.Windows.Forms.ComboBox
    Friend WithEvents cboEnable10 As System.Windows.Forms.ComboBox
    Friend WithEvents cboSchedule9 As System.Windows.Forms.ComboBox
    Friend WithEvents cboEnable9 As System.Windows.Forms.ComboBox
    Friend WithEvents cboSchedule8 As System.Windows.Forms.ComboBox
    Friend WithEvents cboEnable8 As System.Windows.Forms.ComboBox
    Friend WithEvents cboSchedule7 As System.Windows.Forms.ComboBox
    Friend WithEvents cboEnable7 As System.Windows.Forms.ComboBox
    Friend WithEvents cboSchedule6 As System.Windows.Forms.ComboBox
    Friend WithEvents cboEnable6 As System.Windows.Forms.ComboBox
    Friend WithEvents cboSchedule5 As System.Windows.Forms.ComboBox
    Friend WithEvents cboEnable5 As System.Windows.Forms.ComboBox
    Friend WithEvents cboSchedule4 As System.Windows.Forms.ComboBox
    Friend WithEvents cboEnable4 As System.Windows.Forms.ComboBox
    Friend WithEvents cboSchedule3 As System.Windows.Forms.ComboBox
    Friend WithEvents cboEnable3 As System.Windows.Forms.ComboBox
    Friend WithEvents cboRecordMode As System.Windows.Forms.ComboBox
    Friend WithEvents lblRecordMode As System.Windows.Forms.Label
    Friend WithEvents ftbPrgName As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblPrgName As System.Windows.Forms.Label
    Friend WithEvents ftbPortNum As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblPortNum As System.Windows.Forms.Label
    Friend WithEvents lblIoType As System.Windows.Forms.Label
    Friend WithEvents cboIoType As System.Windows.Forms.ComboBox
    Friend WithEvents lblType As System.Windows.Forms.Label
    Friend WithEvents cboType As System.Windows.Forms.ComboBox
    Friend WithEvents ftbIntercept As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblIntercept As System.Windows.Forms.Label
    Friend WithEvents ftbSlope As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblSlope As System.Windows.Forms.Label
    Friend WithEvents ftbUnits As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblUnits As System.Windows.Forms.Label
    Friend WithEvents ftbDesc As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblDesc As System.Windows.Forms.Label
    Friend WithEvents ftbVarName As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblVarName As System.Windows.Forms.Label
    Friend WithEvents lblA As System.Windows.Forms.Label
    Friend WithEvents lblG As System.Windows.Forms.Label
    Friend WithEvents ftbAxis As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbGroup As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblAxis As System.Windows.Forms.Label
    Friend WithEvents btnPrint As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuPrint As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintPreview As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnUtilities As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuExport As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuImport As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblSaveWarning As System.Windows.Forms.Label
    Friend WithEvents lblSaveWarning1 As System.Windows.Forms.Label
    Friend WithEvents chkRC_2 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRC_1 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRC_7 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRC_6 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRC_5 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRC_4 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRC_3 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRC_12 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRC_11 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRC_10 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRC_9 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRC_8 As System.Windows.Forms.CheckBox
    Friend WithEvents lblJobOrCC As System.Windows.Forms.Label
    Friend WithEvents btnValidate As System.Windows.Forms.Button
    Friend WithEvents lblPortNumComment As System.Windows.Forms.Label
    Friend WithEvents btnBrowse As System.Windows.Forms.Button
    Friend WithEvents mnuSaveDefault As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuRestoreDefault As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPageSetup As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintOptions As System.Windows.Forms.ToolStripMenuItem
End Class
