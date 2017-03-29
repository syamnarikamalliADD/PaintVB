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
        Me.components = New System.ComponentModel.Container
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
        Me.lblRobotControllers = New System.Windows.Forms.Label
        Me.lblEditDateCap = New System.Windows.Forms.Label
        Me.lblEditTimeCap = New System.Windows.Forms.Label
        Me.lblGUITime = New System.Windows.Forms.Label
        Me.lblGUIDate = New System.Windows.Forms.Label
        Me.lblRCTime2 = New System.Windows.Forms.Label
        Me.lblRCDate2 = New System.Windows.Forms.Label
        Me.lblRC_2 = New System.Windows.Forms.Label
        Me.lblRCTime1 = New System.Windows.Forms.Label
        Me.lblRCDate1 = New System.Windows.Forms.Label
        Me.lblRC_1 = New System.Windows.Forms.Label
        Me.lblRobotTimeCap = New System.Windows.Forms.Label
        Me.lblRobotDateCap = New System.Windows.Forms.Label
        Me.lblRobotNameCap = New System.Windows.Forms.Label
        Me.lblPLCTime = New System.Windows.Forms.Label
        Me.lblPLCTimeCap = New System.Windows.Forms.Label
        Me.lblPLCDateCap = New System.Windows.Forms.Label
        Me.lblPLCDate = New System.Windows.Forms.Label
        Me.lblGUITimeCap = New System.Windows.Forms.Label
        Me.lblGUIDateCap = New System.Windows.Forms.Label
        Me.dtpTime = New System.Windows.Forms.DateTimePicker
        Me.dtpDate = New System.Windows.Forms.DateTimePicker
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.lblZone = New System.Windows.Forms.Label
        Me.cboZone = New System.Windows.Forms.ComboBox
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.btnSave = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuSaveGUIToAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuSavePLCToAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuSaveRobotToAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuSaveTimeR1 = New System.Windows.Forms.ToolStripMenuItem
        Me.btnUndo = New System.Windows.Forms.ToolStripButton
        Me.btnChangeLog = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuLast24 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLast7 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAllChanges = New System.Windows.Forms.ToolStripMenuItem
        Me.pnlTxt = New System.Windows.Forms.StatusBarPanel
        Me.pnlProg = New System.Windows.Forms.StatusBarPanel
        Me.tmrRefresh = New System.Windows.Forms.Timer(Me.components)
        Me.tscMain.BottomToolStripPanel.SuspendLayout()
        Me.tscMain.ContentPanel.SuspendLayout()
        Me.tscMain.TopToolStripPanel.SuspendLayout()
        Me.tscMain.SuspendLayout()
        Me.stsStatus.SuspendLayout()
        Me.pnlMain.SuspendLayout()
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
        Me.tscMain.ContentPanel.Controls.Add(Me.pnlMain)
        Me.tscMain.ContentPanel.Controls.Add(Me.lstStatus)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblZone)
        Me.tscMain.ContentPanel.Controls.Add(Me.cboZone)
        Me.tscMain.ContentPanel.Size = New System.Drawing.Size(1016, 628)
        Me.tscMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tscMain.LeftToolStripPanelVisible = False
        Me.tscMain.Location = New System.Drawing.Point(0, 0)
        Me.tscMain.Name = "tscMain"
        Me.tscMain.RightToolStripPanelVisible = False
        Me.tscMain.Size = New System.Drawing.Size(1016, 717)
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
        Me.stsStatus.Size = New System.Drawing.Size(1016, 29)
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
        'pnlMain
        '
        Me.pnlMain.Controls.Add(Me.lblRobotControllers)
        Me.pnlMain.Controls.Add(Me.lblEditDateCap)
        Me.pnlMain.Controls.Add(Me.lblEditTimeCap)
        Me.pnlMain.Controls.Add(Me.lblGUITime)
        Me.pnlMain.Controls.Add(Me.lblGUIDate)
        Me.pnlMain.Controls.Add(Me.lblRCTime2)
        Me.pnlMain.Controls.Add(Me.lblRCDate2)
        Me.pnlMain.Controls.Add(Me.lblRC_2)
        Me.pnlMain.Controls.Add(Me.lblRCTime1)
        Me.pnlMain.Controls.Add(Me.lblRCDate1)
        Me.pnlMain.Controls.Add(Me.lblRC_1)
        Me.pnlMain.Controls.Add(Me.lblRobotTimeCap)
        Me.pnlMain.Controls.Add(Me.lblRobotDateCap)
        Me.pnlMain.Controls.Add(Me.lblRobotNameCap)
        Me.pnlMain.Controls.Add(Me.lblPLCTime)
        Me.pnlMain.Controls.Add(Me.lblPLCTimeCap)
        Me.pnlMain.Controls.Add(Me.lblPLCDateCap)
        Me.pnlMain.Controls.Add(Me.lblPLCDate)
        Me.pnlMain.Controls.Add(Me.lblGUITimeCap)
        Me.pnlMain.Controls.Add(Me.lblGUIDateCap)
        Me.pnlMain.Controls.Add(Me.dtpTime)
        Me.pnlMain.Controls.Add(Me.dtpDate)
        Me.pnlMain.Font = New System.Drawing.Font("Arial", 8.25!)
        Me.pnlMain.Location = New System.Drawing.Point(12, 68)
        Me.pnlMain.Name = "pnlMain"
        Me.pnlMain.Size = New System.Drawing.Size(1213, 677)
        Me.pnlMain.TabIndex = 19
        '
        'lblRobotControllers
        '
        Me.lblRobotControllers.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblRobotControllers.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblRobotControllers.Location = New System.Drawing.Point(330, 37)
        Me.lblRobotControllers.Name = "lblRobotControllers"
        Me.lblRobotControllers.Size = New System.Drawing.Size(448, 19)
        Me.lblRobotControllers.TabIndex = 27
        Me.lblRobotControllers.Tag = "11"
        Me.lblRobotControllers.Text = "lblRobotControllers"
        Me.lblRobotControllers.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblEditDateCap
        '
        Me.lblEditDateCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblEditDateCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblEditDateCap.Location = New System.Drawing.Point(31, 37)
        Me.lblEditDateCap.Name = "lblEditDateCap"
        Me.lblEditDateCap.Size = New System.Drawing.Size(294, 19)
        Me.lblEditDateCap.TabIndex = 26
        Me.lblEditDateCap.Tag = "11"
        Me.lblEditDateCap.Text = "lblEditDateCap"
        Me.lblEditDateCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblEditTimeCap
        '
        Me.lblEditTimeCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblEditTimeCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblEditTimeCap.Location = New System.Drawing.Point(31, 94)
        Me.lblEditTimeCap.Name = "lblEditTimeCap"
        Me.lblEditTimeCap.Size = New System.Drawing.Size(294, 19)
        Me.lblEditTimeCap.TabIndex = 25
        Me.lblEditTimeCap.Tag = "11"
        Me.lblEditTimeCap.Text = "lblEditTimeCap"
        Me.lblEditTimeCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblGUITime
        '
        Me.lblGUITime.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.lblGUITime.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblGUITime.Location = New System.Drawing.Point(31, 232)
        Me.lblGUITime.Name = "lblGUITime"
        Me.lblGUITime.Size = New System.Drawing.Size(294, 19)
        Me.lblGUITime.TabIndex = 24
        Me.lblGUITime.Tag = "11"
        Me.lblGUITime.Text = "lblGUITime"
        Me.lblGUITime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblGUIDate
        '
        Me.lblGUIDate.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.lblGUIDate.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblGUIDate.Location = New System.Drawing.Point(31, 182)
        Me.lblGUIDate.Name = "lblGUIDate"
        Me.lblGUIDate.Size = New System.Drawing.Size(294, 19)
        Me.lblGUIDate.TabIndex = 23
        Me.lblGUIDate.Tag = "11"
        Me.lblGUIDate.Text = "lblGUIDate"
        Me.lblGUIDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblRCTime2
        '
        Me.lblRCTime2.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.lblRCTime2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblRCTime2.Location = New System.Drawing.Point(631, 123)
        Me.lblRCTime2.Name = "lblRCTime2"
        Me.lblRCTime2.Size = New System.Drawing.Size(148, 19)
        Me.lblRCTime2.TabIndex = 22
        Me.lblRCTime2.Tag = "11"
        Me.lblRCTime2.Text = "lblRCTime2"
        Me.lblRCTime2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblRCDate2
        '
        Me.lblRCDate2.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.lblRCDate2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblRCDate2.Location = New System.Drawing.Point(421, 123)
        Me.lblRCDate2.Name = "lblRCDate2"
        Me.lblRCDate2.Size = New System.Drawing.Size(204, 19)
        Me.lblRCDate2.TabIndex = 21
        Me.lblRCDate2.Tag = "11"
        Me.lblRCDate2.Text = "lblRCDate2"
        Me.lblRCDate2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblRC_2
        '
        Me.lblRC_2.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblRC_2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblRC_2.Location = New System.Drawing.Point(330, 123)
        Me.lblRC_2.Name = "lblRC_2"
        Me.lblRC_2.Size = New System.Drawing.Size(84, 19)
        Me.lblRC_2.TabIndex = 20
        Me.lblRC_2.Tag = "11"
        Me.lblRC_2.Text = "lblRC_2"
        Me.lblRC_2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblRCTime1
        '
        Me.lblRCTime1.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.lblRCTime1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblRCTime1.Location = New System.Drawing.Point(631, 94)
        Me.lblRCTime1.Name = "lblRCTime1"
        Me.lblRCTime1.Size = New System.Drawing.Size(148, 19)
        Me.lblRCTime1.TabIndex = 19
        Me.lblRCTime1.Tag = "11"
        Me.lblRCTime1.Text = "lblRCTime1"
        Me.lblRCTime1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblRCDate1
        '
        Me.lblRCDate1.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.lblRCDate1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblRCDate1.Location = New System.Drawing.Point(421, 94)
        Me.lblRCDate1.Name = "lblRCDate1"
        Me.lblRCDate1.Size = New System.Drawing.Size(204, 19)
        Me.lblRCDate1.TabIndex = 18
        Me.lblRCDate1.Tag = "11"
        Me.lblRCDate1.Text = "lblRCDate1"
        Me.lblRCDate1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblRC_1
        '
        Me.lblRC_1.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblRC_1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblRC_1.Location = New System.Drawing.Point(330, 94)
        Me.lblRC_1.Name = "lblRC_1"
        Me.lblRC_1.Size = New System.Drawing.Size(84, 19)
        Me.lblRC_1.TabIndex = 17
        Me.lblRC_1.Tag = "11"
        Me.lblRC_1.Text = "lblRC_1"
        Me.lblRC_1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblRobotTimeCap
        '
        Me.lblRobotTimeCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblRobotTimeCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblRobotTimeCap.Location = New System.Drawing.Point(630, 66)
        Me.lblRobotTimeCap.Name = "lblRobotTimeCap"
        Me.lblRobotTimeCap.Size = New System.Drawing.Size(148, 19)
        Me.lblRobotTimeCap.TabIndex = 16
        Me.lblRobotTimeCap.Tag = "11"
        Me.lblRobotTimeCap.Text = "lblRobotTimeCap"
        Me.lblRobotTimeCap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblRobotDateCap
        '
        Me.lblRobotDateCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblRobotDateCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblRobotDateCap.Location = New System.Drawing.Point(420, 66)
        Me.lblRobotDateCap.Name = "lblRobotDateCap"
        Me.lblRobotDateCap.Size = New System.Drawing.Size(204, 19)
        Me.lblRobotDateCap.TabIndex = 15
        Me.lblRobotDateCap.Tag = "11"
        Me.lblRobotDateCap.Text = "lblRobotDateCap"
        Me.lblRobotDateCap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblRobotNameCap
        '
        Me.lblRobotNameCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblRobotNameCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblRobotNameCap.Location = New System.Drawing.Point(330, 66)
        Me.lblRobotNameCap.Name = "lblRobotNameCap"
        Me.lblRobotNameCap.Size = New System.Drawing.Size(84, 19)
        Me.lblRobotNameCap.TabIndex = 14
        Me.lblRobotNameCap.Tag = "11"
        Me.lblRobotNameCap.Text = "lblRobotNameCap"
        Me.lblRobotNameCap.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblPLCTime
        '
        Me.lblPLCTime.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.lblPLCTime.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblPLCTime.Location = New System.Drawing.Point(31, 339)
        Me.lblPLCTime.Name = "lblPLCTime"
        Me.lblPLCTime.Size = New System.Drawing.Size(294, 19)
        Me.lblPLCTime.TabIndex = 13
        Me.lblPLCTime.Tag = "11"
        Me.lblPLCTime.Text = "lblPLCTime"
        Me.lblPLCTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblPLCTimeCap
        '
        Me.lblPLCTimeCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblPLCTimeCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblPLCTimeCap.Location = New System.Drawing.Point(31, 314)
        Me.lblPLCTimeCap.Name = "lblPLCTimeCap"
        Me.lblPLCTimeCap.Size = New System.Drawing.Size(294, 19)
        Me.lblPLCTimeCap.TabIndex = 12
        Me.lblPLCTimeCap.Tag = "11"
        Me.lblPLCTimeCap.Text = "lblPLCTimeCap"
        Me.lblPLCTimeCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblPLCDateCap
        '
        Me.lblPLCDateCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblPLCDateCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblPLCDateCap.Location = New System.Drawing.Point(31, 264)
        Me.lblPLCDateCap.Name = "lblPLCDateCap"
        Me.lblPLCDateCap.Size = New System.Drawing.Size(294, 19)
        Me.lblPLCDateCap.TabIndex = 11
        Me.lblPLCDateCap.Tag = "11"
        Me.lblPLCDateCap.Text = "lblPLCDateCap"
        Me.lblPLCDateCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblPLCDate
        '
        Me.lblPLCDate.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.lblPLCDate.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblPLCDate.Location = New System.Drawing.Point(31, 289)
        Me.lblPLCDate.Name = "lblPLCDate"
        Me.lblPLCDate.Size = New System.Drawing.Size(294, 19)
        Me.lblPLCDate.TabIndex = 10
        Me.lblPLCDate.Tag = "11"
        Me.lblPLCDate.Text = "lblPLCDate"
        Me.lblPLCDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblGUITimeCap
        '
        Me.lblGUITimeCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblGUITimeCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblGUITimeCap.Location = New System.Drawing.Point(31, 207)
        Me.lblGUITimeCap.Name = "lblGUITimeCap"
        Me.lblGUITimeCap.Size = New System.Drawing.Size(294, 19)
        Me.lblGUITimeCap.TabIndex = 9
        Me.lblGUITimeCap.Tag = "11"
        Me.lblGUITimeCap.Text = "lblGUITimeCap"
        Me.lblGUITimeCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblGUIDateCap
        '
        Me.lblGUIDateCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblGUIDateCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblGUIDateCap.Location = New System.Drawing.Point(31, 157)
        Me.lblGUIDateCap.Name = "lblGUIDateCap"
        Me.lblGUIDateCap.Size = New System.Drawing.Size(294, 19)
        Me.lblGUIDateCap.TabIndex = 8
        Me.lblGUIDateCap.Tag = "11"
        Me.lblGUIDateCap.Text = "lblGUIDateCap"
        Me.lblGUIDateCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'dtpTime
        '
        Me.dtpTime.Checked = False
        Me.dtpTime.CustomFormat = "hh:mm:ss tt"
        Me.dtpTime.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.dtpTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpTime.Location = New System.Drawing.Point(118, 119)
        Me.dtpTime.Name = "dtpTime"
        Me.dtpTime.ShowUpDown = True
        Me.dtpTime.Size = New System.Drawing.Size(121, 26)
        Me.dtpTime.TabIndex = 2
        Me.dtpTime.Tag = "1"
        '
        'dtpDate
        '
        Me.dtpDate.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.dtpDate.Location = New System.Drawing.Point(47, 62)
        Me.dtpDate.Name = "dtpDate"
        Me.dtpDate.Size = New System.Drawing.Size(262, 26)
        Me.dtpDate.TabIndex = 0
        Me.dtpDate.Value = New Date(2010, 1, 1, 0, 0, 0, 0)
        '
        'lstStatus
        '
        Me.lstStatus.BackColor = System.Drawing.SystemColors.Info
        Me.lstStatus.ItemHeight = 14
        Me.lstStatus.Location = New System.Drawing.Point(569, 4)
        Me.lstStatus.Name = "lstStatus"
        Me.lstStatus.Size = New System.Drawing.Size(425, 60)
        Me.lstStatus.TabIndex = 11
        '
        'lblZone
        '
        Me.lblZone.AutoSize = True
        Me.lblZone.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblZone.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblZone.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblZone.Location = New System.Drawing.Point(56, 8)
        Me.lblZone.Name = "lblZone"
        Me.lblZone.Size = New System.Drawing.Size(47, 19)
        Me.lblZone.TabIndex = 12
        Me.lblZone.Text = "Zone"
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
        '
        'tlsMain
        '
        Me.tlsMain.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.tlsMain.Dock = System.Windows.Forms.DockStyle.None
        Me.tlsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.btnSave, Me.btnUndo, Me.btnChangeLog})
        Me.tlsMain.Location = New System.Drawing.Point(0, 0)
        Me.tlsMain.MinimumSize = New System.Drawing.Size(0, 50)
        Me.tlsMain.Name = "tlsMain"
        Me.tlsMain.Size = New System.Drawing.Size(1016, 60)
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
        Me.btnSave.AutoSize = False
        Me.btnSave.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuSaveGUIToAll, Me.mnuSavePLCToAll, Me.mnuSaveRobotToAll})
        Me.btnSave.Enabled = False
        Me.btnSave.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(70, 57)
        Me.btnSave.Text = "Save"
        Me.btnSave.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'mnuSaveGUIToAll
        '
        Me.mnuSaveGUIToAll.Name = "mnuSaveGUIToAll"
        Me.mnuSaveGUIToAll.Size = New System.Drawing.Size(191, 22)
        Me.mnuSaveGUIToAll.Text = "mnuSaveGUIToAll"
        '
        'mnuSavePLCToAll
        '
        Me.mnuSavePLCToAll.Name = "mnuSavePLCToAll"
        Me.mnuSavePLCToAll.Size = New System.Drawing.Size(191, 22)
        Me.mnuSavePLCToAll.Text = "mnuSavePLCToAll"
        '
        'mnuSaveRobotToAll
        '
        Me.mnuSaveRobotToAll.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuSaveTimeR1})
        Me.mnuSaveRobotToAll.Name = "mnuSaveRobotToAll"
        Me.mnuSaveRobotToAll.Size = New System.Drawing.Size(191, 22)
        Me.mnuSaveRobotToAll.Text = "mnuSaveRobotToAll"
        '
        'mnuSaveTimeR1
        '
        Me.mnuSaveTimeR1.Name = "mnuSaveTimeR1"
        Me.mnuSaveTimeR1.Size = New System.Drawing.Size(173, 22)
        Me.mnuSaveTimeR1.Text = "mnuSaveTimeR1"
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
        'tmrRefresh
        '
        Me.tmrRefresh.Interval = 1000
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1016, 717)
        Me.ControlBox = False
        Me.Controls.Add(Me.tscMain)
        Me.Font = New System.Drawing.Font("Arial", 8.25!)
        Me.KeyPreview = True
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
    Friend WithEvents dtpDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblGUITimeCap As System.Windows.Forms.Label
    Friend WithEvents lblGUIDateCap As System.Windows.Forms.Label
    Friend WithEvents dtpTime As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblRobotNameCap As System.Windows.Forms.Label
    Friend WithEvents lblPLCTime As System.Windows.Forms.Label
    Friend WithEvents lblPLCTimeCap As System.Windows.Forms.Label
    Friend WithEvents lblPLCDateCap As System.Windows.Forms.Label
    Friend WithEvents lblPLCDate As System.Windows.Forms.Label
    Friend WithEvents lblRCTime2 As System.Windows.Forms.Label
    Friend WithEvents lblRCDate2 As System.Windows.Forms.Label
    Friend WithEvents lblRC_2 As System.Windows.Forms.Label
    Friend WithEvents lblRCTime1 As System.Windows.Forms.Label
    Friend WithEvents lblRCDate1 As System.Windows.Forms.Label
    Friend WithEvents lblRC_1 As System.Windows.Forms.Label
    Friend WithEvents lblRobotTimeCap As System.Windows.Forms.Label
    Friend WithEvents lblRobotDateCap As System.Windows.Forms.Label
    Friend WithEvents lblGUITime As System.Windows.Forms.Label
    Friend WithEvents lblGUIDate As System.Windows.Forms.Label
    Friend WithEvents lblEditDateCap As System.Windows.Forms.Label
    Friend WithEvents lblEditTimeCap As System.Windows.Forms.Label
    Friend WithEvents tmrRefresh As System.Windows.Forms.Timer
    Friend WithEvents lblRobotControllers As System.Windows.Forms.Label
    Friend WithEvents btnSave As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuSaveGUIToAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSavePLCToAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSaveRobotToAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSaveTimeR1 As System.Windows.Forms.ToolStripMenuItem
End Class
