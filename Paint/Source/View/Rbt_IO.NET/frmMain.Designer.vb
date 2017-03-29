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
        Me.tlpMain = New System.Windows.Forms.TableLayoutPanel
        Me.lblIOType = New System.Windows.Forms.Label
        Me.pnlZoneRobotCbos = New System.Windows.Forms.Panel
        Me.cboZone = New System.Windows.Forms.ComboBox
        Me.lblRobot = New System.Windows.Forms.Label
        Me.lblZone = New System.Windows.Forms.Label
        Me.cboRobot = New System.Windows.Forms.ComboBox
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.dgIOPoints = New System.Windows.Forms.DataGridView
        Me.cboIOType = New System.Windows.Forms.ComboBox
        Me.pnlPointDetail = New System.Windows.Forms.Panel
        Me.btnMonitor = New System.Windows.Forms.Button
        Me.btnUnsimAll = New System.Windows.Forms.Button
        Me.btnSetComment = New System.Windows.Forms.Button
        Me.pnlSim = New System.Windows.Forms.Panel
        Me.btnSim = New System.Windows.Forms.Button
        Me.lblSim = New System.Windows.Forms.Label
        Me.pnlSetVal = New System.Windows.Forms.Panel
        Me.btnSetVal = New System.Windows.Forms.Button
        Me.txtSetVal = New System.Windows.Forms.TextBox
        Me.lblIO = New System.Windows.Forms.Label
        Me.dgMonitor = New System.Windows.Forms.DataGridView
        Me.mnuMonitor = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuEndMonitor = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuClearAllMon = New System.Windows.Forms.ToolStripMenuItem
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.btnSave = New System.Windows.Forms.ToolStripButton
        Me.btnAutoRefresh = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuRefresh = New System.Windows.Forms.ToolStripMenuItem
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
        Me.tlpMain.SuspendLayout()
        Me.pnlZoneRobotCbos.SuspendLayout()
        CType(Me.dgIOPoints, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlPointDetail.SuspendLayout()
        Me.pnlSim.SuspendLayout()
        Me.pnlSetVal.SuspendLayout()
        CType(Me.dgMonitor, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.mnuMonitor.SuspendLayout()
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
        Me.tscMain.ContentPanel.Controls.Add(Me.tlpMain)
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
        Me.mnuLogin.Size = New System.Drawing.Size(146, 22)
        Me.mnuLogin.Text = "mnuLogin"
        '
        'mnuLogOut
        '
        Me.mnuLogOut.Name = "mnuLogOut"
        Me.mnuLogOut.Size = New System.Drawing.Size(146, 22)
        Me.mnuLogOut.Text = "mnuLogOut"
        '
        'mnuRemote
        '
        Me.mnuRemote.Name = "mnuRemote"
        Me.mnuRemote.Size = New System.Drawing.Size(146, 22)
        Me.mnuRemote.Text = "mnuRemote"
        '
        'mnuLocal
        '
        Me.mnuLocal.Name = "mnuLocal"
        Me.mnuLocal.Size = New System.Drawing.Size(146, 22)
        Me.mnuLocal.Text = "mnuLocal"
        '
        'tlpMain
        '
        Me.tlpMain.ColumnCount = 3
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpMain.Controls.Add(Me.lblIOType, 0, 1)
        Me.tlpMain.Controls.Add(Me.pnlZoneRobotCbos, 0, 0)
        Me.tlpMain.Controls.Add(Me.lstStatus, 2, 0)
        Me.tlpMain.Controls.Add(Me.dgIOPoints, 0, 2)
        Me.tlpMain.Controls.Add(Me.cboIOType, 1, 1)
        Me.tlpMain.Controls.Add(Me.pnlPointDetail, 2, 1)
        Me.tlpMain.Controls.Add(Me.dgMonitor, 2, 3)
        Me.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpMain.Location = New System.Drawing.Point(0, 0)
        Me.tlpMain.Name = "tlpMain"
        Me.tlpMain.RowCount = 4
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 79.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 95.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpMain.Size = New System.Drawing.Size(1016, 628)
        Me.tlpMain.TabIndex = 22
        '
        'lblIOType
        '
        Me.lblIOType.AutoSize = True
        Me.lblIOType.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblIOType.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblIOType.Location = New System.Drawing.Point(3, 79)
        Me.lblIOType.Name = "lblIOType"
        Me.lblIOType.Size = New System.Drawing.Size(248, 35)
        Me.lblIOType.TabIndex = 2
        Me.lblIOType.Text = "lblIOType"
        Me.lblIOType.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'pnlZoneRobotCbos
        '
        Me.tlpMain.SetColumnSpan(Me.pnlZoneRobotCbos, 2)
        Me.pnlZoneRobotCbos.Controls.Add(Me.cboZone)
        Me.pnlZoneRobotCbos.Controls.Add(Me.lblRobot)
        Me.pnlZoneRobotCbos.Controls.Add(Me.lblZone)
        Me.pnlZoneRobotCbos.Controls.Add(Me.cboRobot)
        Me.pnlZoneRobotCbos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlZoneRobotCbos.Location = New System.Drawing.Point(3, 3)
        Me.pnlZoneRobotCbos.Name = "pnlZoneRobotCbos"
        Me.pnlZoneRobotCbos.Size = New System.Drawing.Size(502, 73)
        Me.pnlZoneRobotCbos.TabIndex = 0
        '
        'cboZone
        '
        Me.cboZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboZone.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboZone.ItemHeight = 18
        Me.cboZone.Location = New System.Drawing.Point(8, 34)
        Me.cboZone.Name = "cboZone"
        Me.cboZone.Size = New System.Drawing.Size(160, 26)
        Me.cboZone.TabIndex = 10
        '
        'lblRobot
        '
        Me.lblRobot.AutoSize = True
        Me.lblRobot.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblRobot.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblRobot.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblRobot.Location = New System.Drawing.Point(224, 9)
        Me.lblRobot.Name = "lblRobot"
        Me.lblRobot.Size = New System.Drawing.Size(74, 19)
        Me.lblRobot.TabIndex = 21
        Me.lblRobot.Text = "lblRobot"
        Me.lblRobot.UseMnemonic = False
        '
        'lblZone
        '
        Me.lblZone.AutoSize = True
        Me.lblZone.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblZone.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblZone.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblZone.Location = New System.Drawing.Point(65, 9)
        Me.lblZone.Name = "lblZone"
        Me.lblZone.Size = New System.Drawing.Size(47, 19)
        Me.lblZone.TabIndex = 12
        Me.lblZone.Text = "Zone"
        Me.lblZone.UseMnemonic = False
        '
        'cboRobot
        '
        Me.cboRobot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboRobot.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboRobot.ItemHeight = 18
        Me.cboRobot.Location = New System.Drawing.Point(174, 34)
        Me.cboRobot.Name = "cboRobot"
        Me.cboRobot.Size = New System.Drawing.Size(175, 26)
        Me.cboRobot.TabIndex = 20
        '
        'lstStatus
        '
        Me.lstStatus.BackColor = System.Drawing.SystemColors.Info
        Me.lstStatus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstStatus.ItemHeight = 14
        Me.lstStatus.Location = New System.Drawing.Point(511, 3)
        Me.lstStatus.Name = "lstStatus"
        Me.lstStatus.Size = New System.Drawing.Size(502, 60)
        Me.lstStatus.TabIndex = 11
        Me.lstStatus.Visible = False
        '
        'dgIOPoints
        '
        Me.dgIOPoints.AllowUserToAddRows = False
        Me.dgIOPoints.AllowUserToDeleteRows = False
        Me.dgIOPoints.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.dgIOPoints.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.tlpMain.SetColumnSpan(Me.dgIOPoints, 2)
        Me.dgIOPoints.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgIOPoints.Location = New System.Drawing.Point(3, 117)
        Me.dgIOPoints.Name = "dgIOPoints"
        Me.dgIOPoints.ReadOnly = True
        Me.tlpMain.SetRowSpan(Me.dgIOPoints, 2)
        Me.dgIOPoints.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgIOPoints.Size = New System.Drawing.Size(502, 508)
        Me.dgIOPoints.TabIndex = 13
        '
        'cboIOType
        '
        Me.cboIOType.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cboIOType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboIOType.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboIOType.ItemHeight = 18
        Me.cboIOType.Location = New System.Drawing.Point(257, 82)
        Me.cboIOType.Name = "cboIOType"
        Me.cboIOType.Size = New System.Drawing.Size(248, 26)
        Me.cboIOType.TabIndex = 12
        '
        'pnlPointDetail
        '
        Me.pnlPointDetail.Controls.Add(Me.btnMonitor)
        Me.pnlPointDetail.Controls.Add(Me.btnUnsimAll)
        Me.pnlPointDetail.Controls.Add(Me.btnSetComment)
        Me.pnlPointDetail.Controls.Add(Me.pnlSim)
        Me.pnlPointDetail.Controls.Add(Me.pnlSetVal)
        Me.pnlPointDetail.Controls.Add(Me.lblIO)
        Me.pnlPointDetail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlPointDetail.Location = New System.Drawing.Point(511, 82)
        Me.pnlPointDetail.Name = "pnlPointDetail"
        Me.tlpMain.SetRowSpan(Me.pnlPointDetail, 2)
        Me.pnlPointDetail.Size = New System.Drawing.Size(502, 124)
        Me.pnlPointDetail.TabIndex = 14
        Me.pnlPointDetail.Visible = False
        '
        'btnMonitor
        '
        Me.btnMonitor.Location = New System.Drawing.Point(230, 88)
        Me.btnMonitor.Name = "btnMonitor"
        Me.btnMonitor.Size = New System.Drawing.Size(83, 25)
        Me.btnMonitor.TabIndex = 9
        Me.btnMonitor.Text = "btnMonitor"
        Me.btnMonitor.UseVisualStyleBackColor = True
        '
        'btnUnsimAll
        '
        Me.btnUnsimAll.Location = New System.Drawing.Point(3, 47)
        Me.btnUnsimAll.Name = "btnUnsimAll"
        Me.btnUnsimAll.Size = New System.Drawing.Size(125, 25)
        Me.btnUnsimAll.TabIndex = 8
        Me.btnUnsimAll.Text = "btnUnsimAll"
        Me.btnUnsimAll.UseVisualStyleBackColor = True
        '
        'btnSetComment
        '
        Me.btnSetComment.Location = New System.Drawing.Point(134, 47)
        Me.btnSetComment.Name = "btnSetComment"
        Me.btnSetComment.Size = New System.Drawing.Size(83, 25)
        Me.btnSetComment.TabIndex = 7
        Me.btnSetComment.Text = "btnSetComment"
        Me.btnSetComment.UseVisualStyleBackColor = True
        '
        'pnlSim
        '
        Me.pnlSim.Controls.Add(Me.btnSim)
        Me.pnlSim.Controls.Add(Me.lblSim)
        Me.pnlSim.Location = New System.Drawing.Point(10, 84)
        Me.pnlSim.Name = "pnlSim"
        Me.pnlSim.Size = New System.Drawing.Size(214, 33)
        Me.pnlSim.TabIndex = 6
        '
        'btnSim
        '
        Me.btnSim.Location = New System.Drawing.Point(123, 4)
        Me.btnSim.Name = "btnSim"
        Me.btnSim.Size = New System.Drawing.Size(83, 25)
        Me.btnSim.TabIndex = 5
        Me.btnSim.Text = "btnSim"
        Me.btnSim.UseVisualStyleBackColor = True
        '
        'lblSim
        '
        Me.lblSim.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblSim.Location = New System.Drawing.Point(25, 7)
        Me.lblSim.Name = "lblSim"
        Me.lblSim.Size = New System.Drawing.Size(94, 19)
        Me.lblSim.TabIndex = 4
        Me.lblSim.Text = "lblSim"
        Me.lblSim.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'pnlSetVal
        '
        Me.pnlSetVal.Controls.Add(Me.btnSetVal)
        Me.pnlSetVal.Controls.Add(Me.txtSetVal)
        Me.pnlSetVal.Location = New System.Drawing.Point(223, 44)
        Me.pnlSetVal.Name = "pnlSetVal"
        Me.pnlSetVal.Size = New System.Drawing.Size(134, 31)
        Me.pnlSetVal.TabIndex = 5
        '
        'btnSetVal
        '
        Me.btnSetVal.Location = New System.Drawing.Point(80, 3)
        Me.btnSetVal.Name = "btnSetVal"
        Me.btnSetVal.Size = New System.Drawing.Size(44, 25)
        Me.btnSetVal.TabIndex = 2
        Me.btnSetVal.Text = "btnSetVal"
        Me.btnSetVal.UseVisualStyleBackColor = True
        '
        'txtSetVal
        '
        Me.txtSetVal.Location = New System.Drawing.Point(3, 5)
        Me.txtSetVal.Name = "txtSetVal"
        Me.txtSetVal.Size = New System.Drawing.Size(70, 20)
        Me.txtSetVal.TabIndex = 1
        '
        'lblIO
        '
        Me.lblIO.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblIO.Location = New System.Drawing.Point(3, 9)
        Me.lblIO.Name = "lblIO"
        Me.lblIO.Size = New System.Drawing.Size(354, 19)
        Me.lblIO.TabIndex = 0
        Me.lblIO.Text = "lblIO"
        Me.lblIO.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'dgMonitor
        '
        Me.dgMonitor.AllowUserToAddRows = False
        Me.dgMonitor.AllowUserToDeleteRows = False
        Me.dgMonitor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgMonitor.ContextMenuStrip = Me.mnuMonitor
        Me.dgMonitor.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgMonitor.Location = New System.Drawing.Point(511, 212)
        Me.dgMonitor.Name = "dgMonitor"
        Me.dgMonitor.Size = New System.Drawing.Size(502, 413)
        Me.dgMonitor.TabIndex = 15
        '
        'mnuMonitor
        '
        Me.mnuMonitor.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuEndMonitor, Me.mnuClearAllMon})
        Me.mnuMonitor.Name = "mnuMonitor"
        Me.mnuMonitor.Size = New System.Drawing.Size(151, 48)
        '
        'mnuEndMonitor
        '
        Me.mnuEndMonitor.Name = "mnuEndMonitor"
        Me.mnuEndMonitor.Size = New System.Drawing.Size(150, 22)
        Me.mnuEndMonitor.Text = "mnuEndMonitor"
        '
        'mnuClearAllMon
        '
        Me.mnuClearAllMon.Name = "mnuClearAllMon"
        Me.mnuClearAllMon.Size = New System.Drawing.Size(150, 22)
        Me.mnuClearAllMon.Text = "mnuClearAllMon"
        '
        'tlsMain
        '
        Me.tlsMain.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.tlsMain.Dock = System.Windows.Forms.DockStyle.None
        Me.tlsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.btnSave, Me.btnAutoRefresh, Me.btnUndo, Me.btnChangeLog})
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
        Me.btnSave.Enabled = False
        Me.btnSave.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnSave.Image = CType(resources.GetObject("btnSave.Image"), System.Drawing.Image)
        Me.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(70, 57)
        Me.btnSave.Text = "Save"
        Me.btnSave.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.btnSave.Visible = False
        '
        'btnAutoRefresh
        '
        Me.btnAutoRefresh.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.btnAutoRefresh.DropDownButtonWidth = 13
        Me.btnAutoRefresh.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuRefresh})
        Me.btnAutoRefresh.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnAutoRefresh.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnAutoRefresh.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnAutoRefresh.Name = "btnAutoRefresh"
        Me.btnAutoRefresh.Size = New System.Drawing.Size(115, 57)
        Me.btnAutoRefresh.Text = "btnAutoRefresh"
        Me.btnAutoRefresh.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnAutoRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'mnuRefresh
        '
        Me.mnuRefresh.Name = "mnuRefresh"
        Me.mnuRefresh.Size = New System.Drawing.Size(145, 22)
        Me.mnuRefresh.Text = "mnuRefresh"
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
        Me.btnUndo.Visible = False
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
        Me.tscMain.TopToolStripPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.PerformLayout()
        Me.tscMain.ResumeLayout(False)
        Me.tscMain.PerformLayout()
        Me.stsStatus.ResumeLayout(False)
        Me.stsStatus.PerformLayout()
        Me.tlpMain.ResumeLayout(False)
        Me.tlpMain.PerformLayout()
        Me.pnlZoneRobotCbos.ResumeLayout(False)
        Me.pnlZoneRobotCbos.PerformLayout()
        CType(Me.dgIOPoints, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlPointDetail.ResumeLayout(False)
        Me.pnlSim.ResumeLayout(False)
        Me.pnlSetVal.ResumeLayout(False)
        Me.pnlSetVal.PerformLayout()
        CType(Me.dgMonitor, System.ComponentModel.ISupportInitialize).EndInit()
        Me.mnuMonitor.ResumeLayout(False)
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
    Friend WithEvents tmrRefresh As System.Windows.Forms.Timer
    Friend WithEvents lblRobot As System.Windows.Forms.Label
    Friend WithEvents cboRobot As System.Windows.Forms.ComboBox
    Friend WithEvents btnSave As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlpMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents pnlZoneRobotCbos As System.Windows.Forms.Panel
    Friend WithEvents cboIOType As System.Windows.Forms.ComboBox
    Friend WithEvents dgIOPoints As System.Windows.Forms.DataGridView
    Friend WithEvents lblIOType As System.Windows.Forms.Label
    Friend WithEvents pnlPointDetail As System.Windows.Forms.Panel
    Friend WithEvents lblSim As System.Windows.Forms.Label
    Friend WithEvents txtSetVal As System.Windows.Forms.TextBox
    Friend WithEvents lblIO As System.Windows.Forms.Label
    Friend WithEvents btnAutoRefresh As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuRefresh As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents pnlSetVal As System.Windows.Forms.Panel
    Friend WithEvents btnSetVal As System.Windows.Forms.Button
    Friend WithEvents pnlSim As System.Windows.Forms.Panel
    Friend WithEvents btnSim As System.Windows.Forms.Button
    Friend WithEvents btnSetComment As System.Windows.Forms.Button
    Friend WithEvents btnUnsimAll As System.Windows.Forms.Button
    Friend WithEvents btnMonitor As System.Windows.Forms.Button
    Friend WithEvents dgMonitor As System.Windows.Forms.DataGridView
    Friend WithEvents mnuMonitor As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuEndMonitor As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuClearAllMon As System.Windows.Forms.ToolStripMenuItem
End Class
