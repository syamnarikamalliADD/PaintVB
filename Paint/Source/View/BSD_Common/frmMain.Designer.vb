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
        Me.pnlTxt = New System.Windows.Forms.StatusBarPanel
        Me.pnlProg = New System.Windows.Forms.StatusBarPanel
        Me.stsStatus = New System.Windows.Forms.StatusStrip
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel
        Me.tspProgress = New System.Windows.Forms.ToolStripProgressBar
        Me.lblSpacer = New System.Windows.Forms.ToolStripStatusLabel
        Me.btnFunction = New System.Windows.Forms.ToolStripDropDownButton
        Me.mnuLogin = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLogOut = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuRemote = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLocal = New System.Windows.Forms.ToolStripMenuItem
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.lblZone = New System.Windows.Forms.ToolStripLabel
        Me.cboZone = New System.Windows.Forms.ToolStripComboBox
        Me.btnBooth = New System.Windows.Forms.ToolStripButton
        Me.btnQueue = New System.Windows.Forms.ToolStripButton
        Me.btnGhost = New System.Windows.Forms.ToolStripButton
        Me.btnZDT = New System.Windows.Forms.ToolStripButton
        Me.btnProcess = New System.Windows.Forms.ToolStripSplitButton
        Me.btnConv = New System.Windows.Forms.ToolStripButton
        Me.btnUtil = New System.Windows.Forms.ToolStripDropDownButton
        Me.mnuResetJobCount = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLampTest = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEstatTest = New System.Windows.Forms.ToolStripMenuItem
        Me.btnChangeLog = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuLast24 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLast7 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAllChanges = New System.Windows.Forms.ToolStripMenuItem
        Me.btnStatus = New System.Windows.Forms.ToolStripButton
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.tscMain = New System.Windows.Forms.ToolStripContainer
        Me.imlLed = New System.Windows.Forms.ImageList(Me.components)
        Me.imlEstop_Cstop = New System.Windows.Forms.ImageList(Me.components)
        Me.imlSelSwitch = New System.Windows.Forms.ImageList(Me.components)
        Me.imlPendant = New System.Windows.Forms.ImageList(Me.components)
        Me.imlDisc = New System.Windows.Forms.ImageList(Me.components)
        Me.imlEstat = New System.Windows.Forms.ImageList(Me.components)
        Me.tmrSA = New System.Windows.Forms.Timer(Me.components)
        Me.imlLimitSwitches = New System.Windows.Forms.ImageList(Me.components)
        Me.imlPEC = New System.Windows.Forms.ImageList(Me.components)
        Me.imlProxSwitch = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP700BL_Normal = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP700BR_Normal = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP700TL_Normal = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP700TR_Normal = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP700TR_Mouseover = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP700BR_Mouseover = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP700TL_Mouseover = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP700BL_Mouseover = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP700BL_Selected = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP700BR_Selected = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP700TL_Selected = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP700TR_Selected = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP20BL_Normal = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP20BR_Normal = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP20TL_Normal = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP20TR_Normal = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP20BL_Mouseover = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP20BR_Mouseover = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP20TL_Mouseover = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP20TR_Mouseover = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP20BL_Selected = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP20BR_Selected = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP20TL_Selected = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP20TR_Selected = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP25BL_Normal = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP25BR_Normal = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP25TL_Normal = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP25TR_Normal = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP25BL_Mouseover = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP25BR_Mouseover = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP25TL_Mouseover = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP25TR_Mouseover = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP25BL_Selected = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP25BR_Selected = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP25TL_Selected = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP25TR_Selected = New System.Windows.Forms.ImageList(Me.components)
        Me.imlMaintDoor = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP500T_Mouseover = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP500B_Normal = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP500B_Selected = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP500B_Mouseover = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP500T_Normal = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP500T_Selected = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP200BL_Normal = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP200BR_Normal = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP200TL_Normal = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP200TR_Normal = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP200BL_Mouseover = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP200BR_Mouseover = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP200TL_Mouseover = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP200TR_Mouseover = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP200BL_Selected = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP200BR_Selected = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP200TL_Selected = New System.Windows.Forms.ImageList(Me.components)
        Me.imlP200TR_Selected = New System.Windows.Forms.ImageList(Me.components)
        Me.tmrStartupDelay = New System.Windows.Forms.Timer(Me.components)
        Me.imlEnabDev = New System.Windows.Forms.ImageList(Me.components)
        CType(Me.pnlTxt, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pnlProg, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.stsStatus.SuspendLayout()
        Me.tlsMain.SuspendLayout()
        Me.tscMain.TopToolStripPanel.SuspendLayout()
        Me.tscMain.SuspendLayout()
        Me.SuspendLayout()
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
        'stsStatus
        '
        Me.stsStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.stsStatus.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.stsStatus.GripMargin = New System.Windows.Forms.Padding(0)
        Me.stsStatus.ImageScalingSize = New System.Drawing.Size(22, 22)
        Me.stsStatus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatus, Me.tspProgress, Me.lblSpacer, Me.btnFunction})
        Me.stsStatus.Location = New System.Drawing.Point(0, 688)
        Me.stsStatus.Name = "stsStatus"
        Me.stsStatus.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode
        Me.stsStatus.ShowItemToolTips = True
        Me.stsStatus.Size = New System.Drawing.Size(1016, 29)
        Me.stsStatus.TabIndex = 6
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
        Me.lblStatus.Text = "test"
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tspProgress
        '
        Me.tspProgress.AutoSize = False
        Me.tspProgress.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.tspProgress.Margin = New System.Windows.Forms.Padding(2, 3, 1, 3)
        Me.tspProgress.Name = "tspProgress"
        Me.tspProgress.Padding = New System.Windows.Forms.Padding(2)
        Me.tspProgress.Size = New System.Drawing.Size(113, 23)
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
        'tlsMain
        '
        Me.tlsMain.Dock = System.Windows.Forms.DockStyle.None
        Me.tlsMain.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.lblZone, Me.cboZone, Me.btnBooth, Me.btnQueue, Me.btnGhost, Me.btnZDT, Me.btnProcess, Me.btnConv, Me.btnUtil, Me.btnChangeLog, Me.btnStatus})
        Me.tlsMain.Location = New System.Drawing.Point(0, 0)
        Me.tlsMain.Margin = New System.Windows.Forms.Padding(0, 5, 0, 5)
        Me.tlsMain.MinimumSize = New System.Drawing.Size(0, 57)
        Me.tlsMain.Name = "tlsMain"
        Me.tlsMain.Size = New System.Drawing.Size(1016, 62)
        Me.tlsMain.Stretch = True
        Me.tlsMain.TabIndex = 5
        Me.tlsMain.TabStop = True
        '
        'btnClose
        '
        Me.btnClose.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnClose.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(44, 59)
        Me.btnClose.Text = "Close"
        Me.btnClose.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'lblZone
        '
        Me.lblZone.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblZone.Name = "lblZone"
        Me.lblZone.Size = New System.Drawing.Size(47, 59)
        Me.lblZone.Text = "Zone"
        '
        'cboZone
        '
        Me.cboZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboZone.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboZone.Margin = New System.Windows.Forms.Padding(1, 0, 4, 0)
        Me.cboZone.Name = "cboZone"
        Me.cboZone.Size = New System.Drawing.Size(121, 62)
        '
        'btnBooth
        '
        Me.btnBooth.Enabled = False
        Me.btnBooth.Image = Global.BSD.My.Resources.ProjectStrings.imgBooth
        Me.btnBooth.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnBooth.Name = "btnBooth"
        Me.btnBooth.Size = New System.Drawing.Size(44, 59)
        Me.btnBooth.Text = "Booth"
        Me.btnBooth.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnBooth.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'btnQueue
        '
        Me.btnQueue.Enabled = False
        Me.btnQueue.Image = Global.BSD.My.Resources.ProjectStrings.imgQueue
        Me.btnQueue.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnQueue.Name = "btnQueue"
        Me.btnQueue.Size = New System.Drawing.Size(48, 59)
        Me.btnQueue.Text = "Queue"
        Me.btnQueue.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnQueue.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'btnGhost
        '
        Me.btnGhost.Enabled = False
        Me.btnGhost.Image = Global.BSD.My.Resources.ProjectStrings.imgGhost
        Me.btnGhost.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnGhost.Name = "btnGhost"
        Me.btnGhost.Size = New System.Drawing.Size(44, 59)
        Me.btnGhost.Text = "Ghost"
        Me.btnGhost.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnGhost.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'btnZDT
        '
        Me.btnZDT.AutoSize = False
        Me.btnZDT.Enabled = False
        Me.btnZDT.Image = CType(resources.GetObject("btnZDT.Image"), System.Drawing.Image)
        Me.btnZDT.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnZDT.Name = "btnZDT"
        Me.btnZDT.Size = New System.Drawing.Size(35, 59)
        Me.btnZDT.Text = "ZDT"
        Me.btnZDT.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.btnZDT.Visible = False
        '
        'btnProcess
        '
        Me.btnProcess.Enabled = False
        Me.btnProcess.Image = Global.BSD.My.Resources.ProjectStrings.imgProcess
        Me.btnProcess.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnProcess.Name = "btnProcess"
        Me.btnProcess.Size = New System.Drawing.Size(69, 59)
        Me.btnProcess.Text = "Process"
        Me.btnProcess.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnProcess.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'btnConv
        '
        Me.btnConv.Enabled = False
        Me.btnConv.Image = Global.BSD.My.Resources.ProjectStrings.imgConv
        Me.btnConv.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnConv.Name = "btnConv"
        Me.btnConv.Size = New System.Drawing.Size(100, 59)
        Me.btnConv.Text = "Conveyor Status"
        Me.btnConv.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnConv.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'btnUtil
        '
        Me.btnUtil.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuResetJobCount, Me.mnuLampTest, Me.mnuEstatTest})
        Me.btnUtil.Enabled = False
        Me.btnUtil.Image = Global.BSD.My.Resources.ProjectStrings.imgUtil
        Me.btnUtil.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnUtil.Name = "btnUtil"
        Me.btnUtil.Size = New System.Drawing.Size(61, 59)
        Me.btnUtil.Text = "Utilities"
        Me.btnUtil.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnUtil.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'mnuResetJobCount
        '
        Me.mnuResetJobCount.Name = "mnuResetJobCount"
        Me.mnuResetJobCount.Size = New System.Drawing.Size(163, 22)
        Me.mnuResetJobCount.Text = "Reset Job count"
        '
        'mnuLampTest
        '
        Me.mnuLampTest.Name = "mnuLampTest"
        Me.mnuLampTest.Size = New System.Drawing.Size(163, 22)
        Me.mnuLampTest.Text = "Lamp Test"
        '
        'mnuEstatTest
        '
        Me.mnuEstatTest.Name = "mnuEstatTest"
        Me.mnuEstatTest.Size = New System.Drawing.Size(163, 22)
        Me.mnuEstatTest.Text = "Estat Test"
        Me.mnuEstatTest.Visible = False
        '
        'btnChangeLog
        '
        Me.btnChangeLog.DropDownButtonWidth = 13
        Me.btnChangeLog.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuLast24, Me.mnuLast7, Me.mnuAllChanges})
        Me.btnChangeLog.Enabled = False
        Me.btnChangeLog.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnChangeLog.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnChangeLog.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnChangeLog.Name = "btnChangeLog"
        Me.btnChangeLog.Size = New System.Drawing.Size(95, 59)
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
        'btnStatus
        '
        Me.btnStatus.AutoSize = False
        Me.btnStatus.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnStatus.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnStatus.Name = "btnStatus"
        Me.btnStatus.Size = New System.Drawing.Size(70, 57)
        Me.btnStatus.Text = "Status"
        Me.btnStatus.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnStatus.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.btnStatus.Visible = False
        '
        'lstStatus
        '
        Me.lstStatus.BackColor = System.Drawing.SystemColors.Info
        Me.lstStatus.ItemHeight = 14
        Me.lstStatus.Location = New System.Drawing.Point(518, 53)
        Me.lstStatus.Name = "lstStatus"
        Me.lstStatus.Size = New System.Drawing.Size(498, 46)
        Me.lstStatus.TabIndex = 13
        Me.lstStatus.Visible = False
        '
        'tscMain
        '
        '
        'tscMain.ContentPanel
        '
        Me.tscMain.ContentPanel.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tscMain.ContentPanel.Size = New System.Drawing.Size(1016, 626)
        Me.tscMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tscMain.LeftToolStripPanelVisible = False
        Me.tscMain.Location = New System.Drawing.Point(0, 0)
        Me.tscMain.Name = "tscMain"
        Me.tscMain.RightToolStripPanelVisible = False
        Me.tscMain.Size = New System.Drawing.Size(1016, 688)
        Me.tscMain.TabIndex = 14
        Me.tscMain.Text = "ToolStripContainer1"
        '
        'tscMain.TopToolStripPanel
        '
        Me.tscMain.TopToolStripPanel.Controls.Add(Me.tlsMain)
        '
        'imlLed
        '
        Me.imlLed.ImageStream = CType(resources.GetObject("imlLed.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlLed.TransparentColor = System.Drawing.Color.Transparent
        Me.imlLed.Images.SetKeyName(0, "gray")
        Me.imlLed.Images.SetKeyName(1, "green")
        Me.imlLed.Images.SetKeyName(2, "red")
        Me.imlLed.Images.SetKeyName(3, "yellow")
        Me.imlLed.Images.SetKeyName(4, "blue")
        '
        'imlEstop_Cstop
        '
        Me.imlEstop_Cstop.ImageStream = CType(resources.GetObject("imlEstop_Cstop.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlEstop_Cstop.TransparentColor = System.Drawing.Color.Transparent
        Me.imlEstop_Cstop.Images.SetKeyName(0, "c_in")
        Me.imlEstop_Cstop.Images.SetKeyName(1, "e_in")
        Me.imlEstop_Cstop.Images.SetKeyName(2, "e_off")
        Me.imlEstop_Cstop.Images.SetKeyName(3, "e_out")
        Me.imlEstop_Cstop.Images.SetKeyName(4, "c_off")
        Me.imlEstop_Cstop.Images.SetKeyName(5, "c_out")
        '
        'imlSelSwitch
        '
        Me.imlSelSwitch.ImageStream = CType(resources.GetObject("imlSelSwitch.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlSelSwitch.TransparentColor = System.Drawing.Color.Transparent
        Me.imlSelSwitch.Images.SetKeyName(0, "unknown")
        Me.imlSelSwitch.Images.SetKeyName(1, "red_right")
        Me.imlSelSwitch.Images.SetKeyName(2, "red_left")
        Me.imlSelSwitch.Images.SetKeyName(3, "yellow_right")
        Me.imlSelSwitch.Images.SetKeyName(4, "yellow_left")
        Me.imlSelSwitch.Images.SetKeyName(5, "green_left")
        Me.imlSelSwitch.Images.SetKeyName(6, "green_right")
        '
        'imlPendant
        '
        Me.imlPendant.ImageStream = CType(resources.GetObject("imlPendant.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlPendant.TransparentColor = System.Drawing.Color.Transparent
        Me.imlPendant.Images.SetKeyName(0, "unknown")
        Me.imlPendant.Images.SetKeyName(1, "enabled")
        Me.imlPendant.Images.SetKeyName(2, "enabled_link")
        Me.imlPendant.Images.SetKeyName(3, "estop")
        Me.imlPendant.Images.SetKeyName(4, "estop_link")
        Me.imlPendant.Images.SetKeyName(5, "ok")
        Me.imlPendant.Images.SetKeyName(6, "ok_link")
        '
        'imlDisc
        '
        Me.imlDisc.ImageStream = CType(resources.GetObject("imlDisc.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlDisc.TransparentColor = System.Drawing.Color.Transparent
        Me.imlDisc.Images.SetKeyName(0, "on")
        Me.imlDisc.Images.SetKeyName(1, "unknown")
        Me.imlDisc.Images.SetKeyName(2, "e_off")
        Me.imlDisc.Images.SetKeyName(3, "h_off")
        Me.imlDisc.Images.SetKeyName(4, "s_off")
        '
        'imlEstat
        '
        Me.imlEstat.ImageStream = CType(resources.GetObject("imlEstat.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlEstat.TransparentColor = System.Drawing.Color.Transparent
        Me.imlEstat.Images.SetKeyName(0, "disabled")
        Me.imlEstat.Images.SetKeyName(1, "faulted")
        Me.imlEstat.Images.SetKeyName(2, "on")
        Me.imlEstat.Images.SetKeyName(3, "unknown")
        Me.imlEstat.Images.SetKeyName(4, "locked")
        '
        'tmrSA
        '
        Me.tmrSA.Interval = 250
        '
        'imlLimitSwitches
        '
        Me.imlLimitSwitches.ImageStream = CType(resources.GetObject("imlLimitSwitches.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlLimitSwitches.TransparentColor = System.Drawing.Color.Transparent
        Me.imlLimitSwitches.Images.SetKeyName(0, "down_off")
        Me.imlLimitSwitches.Images.SetKeyName(1, "down_on_left")
        Me.imlLimitSwitches.Images.SetKeyName(2, "down_on_right")
        Me.imlLimitSwitches.Images.SetKeyName(3, "down_on_stuck_left")
        Me.imlLimitSwitches.Images.SetKeyName(4, "down_on_stuck_right")
        Me.imlLimitSwitches.Images.SetKeyName(5, "up_off")
        Me.imlLimitSwitches.Images.SetKeyName(6, "up_on_left")
        Me.imlLimitSwitches.Images.SetKeyName(7, "up_on_right")
        Me.imlLimitSwitches.Images.SetKeyName(8, "up_on_stuck_left")
        Me.imlLimitSwitches.Images.SetKeyName(9, "up_on_stuck_right")
        '
        'imlPEC
        '
        Me.imlPEC.ImageStream = CType(resources.GetObject("imlPEC.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlPEC.TransparentColor = System.Drawing.Color.Transparent
        Me.imlPEC.Images.SetKeyName(0, "Mute_Down")
        Me.imlPEC.Images.SetKeyName(1, "Mute_Up")
        Me.imlPEC.Images.SetKeyName(2, "Off_Down")
        Me.imlPEC.Images.SetKeyName(3, "Off_Up")
        Me.imlPEC.Images.SetKeyName(4, "On_Down")
        Me.imlPEC.Images.SetKeyName(5, "On_Up")
        Me.imlPEC.Images.SetKeyName(6, "Fault_Down")
        Me.imlPEC.Images.SetKeyName(7, "Fault_Up")
        '
        'imlProxSwitch
        '
        Me.imlProxSwitch.ImageStream = CType(resources.GetObject("imlProxSwitch.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlProxSwitch.TransparentColor = System.Drawing.Color.Transparent
        Me.imlProxSwitch.Images.SetKeyName(0, "off")
        Me.imlProxSwitch.Images.SetKeyName(1, "on")
        Me.imlProxSwitch.Images.SetKeyName(2, "stuck")
        Me.imlProxSwitch.Images.SetKeyName(3, "warn")
        '
        'imlP700BL_Normal
        '
        Me.imlP700BL_Normal.ImageStream = CType(resources.GetObject("imlP700BL_Normal.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP700BL_Normal.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP700BL_Normal.Images.SetKeyName(0, "bypass")
        Me.imlP700BL_Normal.Images.SetKeyName(1, "canc_cont")
        Me.imlP700BL_Normal.Images.SetKeyName(2, "canc_only")
        Me.imlP700BL_Normal.Images.SetKeyName(3, "colchg")
        Me.imlP700BL_Normal.Images.SetKeyName(4, "cycle")
        Me.imlP700BL_Normal.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP700BL_Normal.Images.SetKeyName(6, "cycle_spray")
        Me.imlP700BL_Normal.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP700BL_Normal.Images.SetKeyName(8, "fault")
        Me.imlP700BL_Normal.Images.SetKeyName(9, "home")
        Me.imlP700BL_Normal.Images.SetKeyName(10, "home_appoff")
        Me.imlP700BL_Normal.Images.SetKeyName(11, "home_trigoff")
        Me.imlP700BL_Normal.Images.SetKeyName(12, "maint")
        Me.imlP700BL_Normal.Images.SetKeyName(13, "master1")
        Me.imlP700BL_Normal.Images.SetKeyName(14, "purge")
        Me.imlP700BL_Normal.Images.SetKeyName(15, "spec1")
        Me.imlP700BL_Normal.Images.SetKeyName(16, "spec2")
        Me.imlP700BL_Normal.Images.SetKeyName(17, "unknown")
        '
        'imlP700BR_Normal
        '
        Me.imlP700BR_Normal.ImageStream = CType(resources.GetObject("imlP700BR_Normal.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP700BR_Normal.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP700BR_Normal.Images.SetKeyName(0, "bypass")
        Me.imlP700BR_Normal.Images.SetKeyName(1, "canc_cont")
        Me.imlP700BR_Normal.Images.SetKeyName(2, "canc_only")
        Me.imlP700BR_Normal.Images.SetKeyName(3, "colchg")
        Me.imlP700BR_Normal.Images.SetKeyName(4, "cycle")
        Me.imlP700BR_Normal.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP700BR_Normal.Images.SetKeyName(6, "cycle_spray")
        Me.imlP700BR_Normal.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP700BR_Normal.Images.SetKeyName(8, "fault")
        Me.imlP700BR_Normal.Images.SetKeyName(9, "home")
        Me.imlP700BR_Normal.Images.SetKeyName(10, "home_appoff")
        Me.imlP700BR_Normal.Images.SetKeyName(11, "home_trigoff")
        Me.imlP700BR_Normal.Images.SetKeyName(12, "maint")
        Me.imlP700BR_Normal.Images.SetKeyName(13, "master1")
        Me.imlP700BR_Normal.Images.SetKeyName(14, "purge")
        Me.imlP700BR_Normal.Images.SetKeyName(15, "spec1")
        Me.imlP700BR_Normal.Images.SetKeyName(16, "spec2")
        Me.imlP700BR_Normal.Images.SetKeyName(17, "unknown")
        '
        'imlP700TL_Normal
        '
        Me.imlP700TL_Normal.ImageStream = CType(resources.GetObject("imlP700TL_Normal.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP700TL_Normal.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP700TL_Normal.Images.SetKeyName(0, "bypass")
        Me.imlP700TL_Normal.Images.SetKeyName(1, "canc_cont")
        Me.imlP700TL_Normal.Images.SetKeyName(2, "canc_only")
        Me.imlP700TL_Normal.Images.SetKeyName(3, "colchg")
        Me.imlP700TL_Normal.Images.SetKeyName(4, "cycle")
        Me.imlP700TL_Normal.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP700TL_Normal.Images.SetKeyName(6, "cycle_spray")
        Me.imlP700TL_Normal.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP700TL_Normal.Images.SetKeyName(8, "fault")
        Me.imlP700TL_Normal.Images.SetKeyName(9, "home")
        Me.imlP700TL_Normal.Images.SetKeyName(10, "home_appoff")
        Me.imlP700TL_Normal.Images.SetKeyName(11, "home_trigoff")
        Me.imlP700TL_Normal.Images.SetKeyName(12, "maint")
        Me.imlP700TL_Normal.Images.SetKeyName(13, "master1")
        Me.imlP700TL_Normal.Images.SetKeyName(14, "purge")
        Me.imlP700TL_Normal.Images.SetKeyName(15, "spec1")
        Me.imlP700TL_Normal.Images.SetKeyName(16, "spec2")
        Me.imlP700TL_Normal.Images.SetKeyName(17, "unknown")
        '
        'imlP700TR_Normal
        '
        Me.imlP700TR_Normal.ImageStream = CType(resources.GetObject("imlP700TR_Normal.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP700TR_Normal.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP700TR_Normal.Images.SetKeyName(0, "bypass")
        Me.imlP700TR_Normal.Images.SetKeyName(1, "canc_cont")
        Me.imlP700TR_Normal.Images.SetKeyName(2, "canc_only")
        Me.imlP700TR_Normal.Images.SetKeyName(3, "colchg")
        Me.imlP700TR_Normal.Images.SetKeyName(4, "cycle")
        Me.imlP700TR_Normal.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP700TR_Normal.Images.SetKeyName(6, "cycle_spray")
        Me.imlP700TR_Normal.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP700TR_Normal.Images.SetKeyName(8, "fault")
        Me.imlP700TR_Normal.Images.SetKeyName(9, "home")
        Me.imlP700TR_Normal.Images.SetKeyName(10, "home_appoff")
        Me.imlP700TR_Normal.Images.SetKeyName(11, "home_trigoff")
        Me.imlP700TR_Normal.Images.SetKeyName(12, "maint")
        Me.imlP700TR_Normal.Images.SetKeyName(13, "master1")
        Me.imlP700TR_Normal.Images.SetKeyName(14, "purge")
        Me.imlP700TR_Normal.Images.SetKeyName(15, "spec1")
        Me.imlP700TR_Normal.Images.SetKeyName(16, "spec2")
        Me.imlP700TR_Normal.Images.SetKeyName(17, "unknown")
        '
        'imlP700TR_Mouseover
        '
        Me.imlP700TR_Mouseover.ImageStream = CType(resources.GetObject("imlP700TR_Mouseover.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP700TR_Mouseover.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP700TR_Mouseover.Images.SetKeyName(0, "bypass")
        Me.imlP700TR_Mouseover.Images.SetKeyName(1, "canc_cont")
        Me.imlP700TR_Mouseover.Images.SetKeyName(2, "canc_only")
        Me.imlP700TR_Mouseover.Images.SetKeyName(3, "colchg")
        Me.imlP700TR_Mouseover.Images.SetKeyName(4, "cycle")
        Me.imlP700TR_Mouseover.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP700TR_Mouseover.Images.SetKeyName(6, "cycle_spray")
        Me.imlP700TR_Mouseover.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP700TR_Mouseover.Images.SetKeyName(8, "fault")
        Me.imlP700TR_Mouseover.Images.SetKeyName(9, "home")
        Me.imlP700TR_Mouseover.Images.SetKeyName(10, "home_appoff")
        Me.imlP700TR_Mouseover.Images.SetKeyName(11, "home_trigoff")
        Me.imlP700TR_Mouseover.Images.SetKeyName(12, "maint")
        Me.imlP700TR_Mouseover.Images.SetKeyName(13, "master1")
        Me.imlP700TR_Mouseover.Images.SetKeyName(14, "purge")
        Me.imlP700TR_Mouseover.Images.SetKeyName(15, "spec1")
        Me.imlP700TR_Mouseover.Images.SetKeyName(16, "spec2")
        Me.imlP700TR_Mouseover.Images.SetKeyName(17, "unknown")
        '
        'imlP700BR_Mouseover
        '
        Me.imlP700BR_Mouseover.ImageStream = CType(resources.GetObject("imlP700BR_Mouseover.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP700BR_Mouseover.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP700BR_Mouseover.Images.SetKeyName(0, "bypass")
        Me.imlP700BR_Mouseover.Images.SetKeyName(1, "canc_cont")
        Me.imlP700BR_Mouseover.Images.SetKeyName(2, "canc_only")
        Me.imlP700BR_Mouseover.Images.SetKeyName(3, "colchg")
        Me.imlP700BR_Mouseover.Images.SetKeyName(4, "cycle")
        Me.imlP700BR_Mouseover.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP700BR_Mouseover.Images.SetKeyName(6, "cycle_spray")
        Me.imlP700BR_Mouseover.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP700BR_Mouseover.Images.SetKeyName(8, "fault")
        Me.imlP700BR_Mouseover.Images.SetKeyName(9, "home")
        Me.imlP700BR_Mouseover.Images.SetKeyName(10, "home_appoff")
        Me.imlP700BR_Mouseover.Images.SetKeyName(11, "home_trigoff")
        Me.imlP700BR_Mouseover.Images.SetKeyName(12, "maint")
        Me.imlP700BR_Mouseover.Images.SetKeyName(13, "master1")
        Me.imlP700BR_Mouseover.Images.SetKeyName(14, "purge")
        Me.imlP700BR_Mouseover.Images.SetKeyName(15, "spec1")
        Me.imlP700BR_Mouseover.Images.SetKeyName(16, "spec2")
        Me.imlP700BR_Mouseover.Images.SetKeyName(17, "unknown")
        '
        'imlP700TL_Mouseover
        '
        Me.imlP700TL_Mouseover.ImageStream = CType(resources.GetObject("imlP700TL_Mouseover.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP700TL_Mouseover.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP700TL_Mouseover.Images.SetKeyName(0, "bypass")
        Me.imlP700TL_Mouseover.Images.SetKeyName(1, "canc_cont")
        Me.imlP700TL_Mouseover.Images.SetKeyName(2, "canc_only")
        Me.imlP700TL_Mouseover.Images.SetKeyName(3, "colchg")
        Me.imlP700TL_Mouseover.Images.SetKeyName(4, "cycle")
        Me.imlP700TL_Mouseover.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP700TL_Mouseover.Images.SetKeyName(6, "cycle_spray")
        Me.imlP700TL_Mouseover.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP700TL_Mouseover.Images.SetKeyName(8, "fault")
        Me.imlP700TL_Mouseover.Images.SetKeyName(9, "home")
        Me.imlP700TL_Mouseover.Images.SetKeyName(10, "home_appoff")
        Me.imlP700TL_Mouseover.Images.SetKeyName(11, "home_trigoff")
        Me.imlP700TL_Mouseover.Images.SetKeyName(12, "maint")
        Me.imlP700TL_Mouseover.Images.SetKeyName(13, "master1")
        Me.imlP700TL_Mouseover.Images.SetKeyName(14, "purge")
        Me.imlP700TL_Mouseover.Images.SetKeyName(15, "spec1")
        Me.imlP700TL_Mouseover.Images.SetKeyName(16, "spec2")
        Me.imlP700TL_Mouseover.Images.SetKeyName(17, "unknown")
        '
        'imlP700BL_Mouseover
        '
        Me.imlP700BL_Mouseover.ImageStream = CType(resources.GetObject("imlP700BL_Mouseover.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP700BL_Mouseover.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP700BL_Mouseover.Images.SetKeyName(0, "bypass")
        Me.imlP700BL_Mouseover.Images.SetKeyName(1, "canc_cont")
        Me.imlP700BL_Mouseover.Images.SetKeyName(2, "canc_only")
        Me.imlP700BL_Mouseover.Images.SetKeyName(3, "colchg")
        Me.imlP700BL_Mouseover.Images.SetKeyName(4, "cycle")
        Me.imlP700BL_Mouseover.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP700BL_Mouseover.Images.SetKeyName(6, "cycle_spray")
        Me.imlP700BL_Mouseover.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP700BL_Mouseover.Images.SetKeyName(8, "fault")
        Me.imlP700BL_Mouseover.Images.SetKeyName(9, "home")
        Me.imlP700BL_Mouseover.Images.SetKeyName(10, "home_appoff")
        Me.imlP700BL_Mouseover.Images.SetKeyName(11, "home_trigoff")
        Me.imlP700BL_Mouseover.Images.SetKeyName(12, "maint")
        Me.imlP700BL_Mouseover.Images.SetKeyName(13, "master1")
        Me.imlP700BL_Mouseover.Images.SetKeyName(14, "purge")
        Me.imlP700BL_Mouseover.Images.SetKeyName(15, "spec1")
        Me.imlP700BL_Mouseover.Images.SetKeyName(16, "spec2")
        Me.imlP700BL_Mouseover.Images.SetKeyName(17, "unknown")
        '
        'imlP700BL_Selected
        '
        Me.imlP700BL_Selected.ImageStream = CType(resources.GetObject("imlP700BL_Selected.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP700BL_Selected.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP700BL_Selected.Images.SetKeyName(0, "bypass")
        Me.imlP700BL_Selected.Images.SetKeyName(1, "canc_cont")
        Me.imlP700BL_Selected.Images.SetKeyName(2, "canc_only")
        Me.imlP700BL_Selected.Images.SetKeyName(3, "colchg")
        Me.imlP700BL_Selected.Images.SetKeyName(4, "cycle")
        Me.imlP700BL_Selected.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP700BL_Selected.Images.SetKeyName(6, "cycle_spray")
        Me.imlP700BL_Selected.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP700BL_Selected.Images.SetKeyName(8, "fault")
        Me.imlP700BL_Selected.Images.SetKeyName(9, "home")
        Me.imlP700BL_Selected.Images.SetKeyName(10, "home_appoff")
        Me.imlP700BL_Selected.Images.SetKeyName(11, "home_trigoff")
        Me.imlP700BL_Selected.Images.SetKeyName(12, "maint")
        Me.imlP700BL_Selected.Images.SetKeyName(13, "master1")
        Me.imlP700BL_Selected.Images.SetKeyName(14, "purge")
        Me.imlP700BL_Selected.Images.SetKeyName(15, "spec1")
        Me.imlP700BL_Selected.Images.SetKeyName(16, "spec2")
        Me.imlP700BL_Selected.Images.SetKeyName(17, "unknown")
        '
        'imlP700BR_Selected
        '
        Me.imlP700BR_Selected.ImageStream = CType(resources.GetObject("imlP700BR_Selected.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP700BR_Selected.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP700BR_Selected.Images.SetKeyName(0, "bypass")
        Me.imlP700BR_Selected.Images.SetKeyName(1, "canc_cont")
        Me.imlP700BR_Selected.Images.SetKeyName(2, "canc_only")
        Me.imlP700BR_Selected.Images.SetKeyName(3, "colchg")
        Me.imlP700BR_Selected.Images.SetKeyName(4, "cycle")
        Me.imlP700BR_Selected.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP700BR_Selected.Images.SetKeyName(6, "cycle_spray")
        Me.imlP700BR_Selected.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP700BR_Selected.Images.SetKeyName(8, "fault")
        Me.imlP700BR_Selected.Images.SetKeyName(9, "home")
        Me.imlP700BR_Selected.Images.SetKeyName(10, "home_appoff")
        Me.imlP700BR_Selected.Images.SetKeyName(11, "home_trigoff")
        Me.imlP700BR_Selected.Images.SetKeyName(12, "maint")
        Me.imlP700BR_Selected.Images.SetKeyName(13, "master1")
        Me.imlP700BR_Selected.Images.SetKeyName(14, "purge")
        Me.imlP700BR_Selected.Images.SetKeyName(15, "spec1")
        Me.imlP700BR_Selected.Images.SetKeyName(16, "spec2")
        Me.imlP700BR_Selected.Images.SetKeyName(17, "unknown")
        '
        'imlP700TL_Selected
        '
        Me.imlP700TL_Selected.ImageStream = CType(resources.GetObject("imlP700TL_Selected.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP700TL_Selected.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP700TL_Selected.Images.SetKeyName(0, "bypass")
        Me.imlP700TL_Selected.Images.SetKeyName(1, "canc_cont")
        Me.imlP700TL_Selected.Images.SetKeyName(2, "canc_only")
        Me.imlP700TL_Selected.Images.SetKeyName(3, "colchg")
        Me.imlP700TL_Selected.Images.SetKeyName(4, "cycle")
        Me.imlP700TL_Selected.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP700TL_Selected.Images.SetKeyName(6, "cycle_spray")
        Me.imlP700TL_Selected.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP700TL_Selected.Images.SetKeyName(8, "fault")
        Me.imlP700TL_Selected.Images.SetKeyName(9, "home")
        Me.imlP700TL_Selected.Images.SetKeyName(10, "home_appoff")
        Me.imlP700TL_Selected.Images.SetKeyName(11, "home_trigoff")
        Me.imlP700TL_Selected.Images.SetKeyName(12, "maint")
        Me.imlP700TL_Selected.Images.SetKeyName(13, "master1")
        Me.imlP700TL_Selected.Images.SetKeyName(14, "purge")
        Me.imlP700TL_Selected.Images.SetKeyName(15, "spec1")
        Me.imlP700TL_Selected.Images.SetKeyName(16, "spec2")
        Me.imlP700TL_Selected.Images.SetKeyName(17, "unknown")
        '
        'imlP700TR_Selected
        '
        Me.imlP700TR_Selected.ImageStream = CType(resources.GetObject("imlP700TR_Selected.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP700TR_Selected.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP700TR_Selected.Images.SetKeyName(0, "bypass")
        Me.imlP700TR_Selected.Images.SetKeyName(1, "canc_cont")
        Me.imlP700TR_Selected.Images.SetKeyName(2, "canc_only")
        Me.imlP700TR_Selected.Images.SetKeyName(3, "colchg")
        Me.imlP700TR_Selected.Images.SetKeyName(4, "cycle")
        Me.imlP700TR_Selected.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP700TR_Selected.Images.SetKeyName(6, "cycle_spray")
        Me.imlP700TR_Selected.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP700TR_Selected.Images.SetKeyName(8, "fault")
        Me.imlP700TR_Selected.Images.SetKeyName(9, "home")
        Me.imlP700TR_Selected.Images.SetKeyName(10, "home_appoff")
        Me.imlP700TR_Selected.Images.SetKeyName(11, "home_trigoff")
        Me.imlP700TR_Selected.Images.SetKeyName(12, "maint")
        Me.imlP700TR_Selected.Images.SetKeyName(13, "master1")
        Me.imlP700TR_Selected.Images.SetKeyName(14, "purge")
        Me.imlP700TR_Selected.Images.SetKeyName(15, "spec1")
        Me.imlP700TR_Selected.Images.SetKeyName(16, "spec2")
        Me.imlP700TR_Selected.Images.SetKeyName(17, "unknown")
        '
        'imlP20BL_Normal
        '
        Me.imlP20BL_Normal.ImageStream = CType(resources.GetObject("imlP20BL_Normal.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP20BL_Normal.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP20BL_Normal.Images.SetKeyName(0, "bypass")
        Me.imlP20BL_Normal.Images.SetKeyName(1, "canc_cont")
        Me.imlP20BL_Normal.Images.SetKeyName(2, "canc_only")
        Me.imlP20BL_Normal.Images.SetKeyName(3, "cycle")
        Me.imlP20BL_Normal.Images.SetKeyName(4, "fault")
        Me.imlP20BL_Normal.Images.SetKeyName(5, "home")
        Me.imlP20BL_Normal.Images.SetKeyName(6, "maint")
        Me.imlP20BL_Normal.Images.SetKeyName(7, "master1")
        Me.imlP20BL_Normal.Images.SetKeyName(8, "spec1")
        Me.imlP20BL_Normal.Images.SetKeyName(9, "spec2")
        Me.imlP20BL_Normal.Images.SetKeyName(10, "unknown")
        '
        'imlP20BR_Normal
        '
        Me.imlP20BR_Normal.ImageStream = CType(resources.GetObject("imlP20BR_Normal.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP20BR_Normal.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP20BR_Normal.Images.SetKeyName(0, "bypass")
        Me.imlP20BR_Normal.Images.SetKeyName(1, "canc_cont")
        Me.imlP20BR_Normal.Images.SetKeyName(2, "canc_only")
        Me.imlP20BR_Normal.Images.SetKeyName(3, "cycle")
        Me.imlP20BR_Normal.Images.SetKeyName(4, "fault")
        Me.imlP20BR_Normal.Images.SetKeyName(5, "home")
        Me.imlP20BR_Normal.Images.SetKeyName(6, "maint")
        Me.imlP20BR_Normal.Images.SetKeyName(7, "master1")
        Me.imlP20BR_Normal.Images.SetKeyName(8, "spec1")
        Me.imlP20BR_Normal.Images.SetKeyName(9, "spec2")
        Me.imlP20BR_Normal.Images.SetKeyName(10, "unknown")
        '
        'imlP20TL_Normal
        '
        Me.imlP20TL_Normal.ImageStream = CType(resources.GetObject("imlP20TL_Normal.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP20TL_Normal.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP20TL_Normal.Images.SetKeyName(0, "bypass")
        Me.imlP20TL_Normal.Images.SetKeyName(1, "canc_cont")
        Me.imlP20TL_Normal.Images.SetKeyName(2, "canc_only")
        Me.imlP20TL_Normal.Images.SetKeyName(3, "cycle")
        Me.imlP20TL_Normal.Images.SetKeyName(4, "fault")
        Me.imlP20TL_Normal.Images.SetKeyName(5, "home")
        Me.imlP20TL_Normal.Images.SetKeyName(6, "maint")
        Me.imlP20TL_Normal.Images.SetKeyName(7, "master1")
        Me.imlP20TL_Normal.Images.SetKeyName(8, "spec1")
        Me.imlP20TL_Normal.Images.SetKeyName(9, "spec2")
        Me.imlP20TL_Normal.Images.SetKeyName(10, "unknown")
        '
        'imlP20TR_Normal
        '
        Me.imlP20TR_Normal.ImageStream = CType(resources.GetObject("imlP20TR_Normal.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP20TR_Normal.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP20TR_Normal.Images.SetKeyName(0, "bypass")
        Me.imlP20TR_Normal.Images.SetKeyName(1, "canc_cont")
        Me.imlP20TR_Normal.Images.SetKeyName(2, "canc_only")
        Me.imlP20TR_Normal.Images.SetKeyName(3, "cycle")
        Me.imlP20TR_Normal.Images.SetKeyName(4, "fault")
        Me.imlP20TR_Normal.Images.SetKeyName(5, "home")
        Me.imlP20TR_Normal.Images.SetKeyName(6, "maint")
        Me.imlP20TR_Normal.Images.SetKeyName(7, "master1")
        Me.imlP20TR_Normal.Images.SetKeyName(8, "spec1")
        Me.imlP20TR_Normal.Images.SetKeyName(9, "spec2")
        Me.imlP20TR_Normal.Images.SetKeyName(10, "unknown")
        '
        'imlP20BL_Mouseover
        '
        Me.imlP20BL_Mouseover.ImageStream = CType(resources.GetObject("imlP20BL_Mouseover.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP20BL_Mouseover.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP20BL_Mouseover.Images.SetKeyName(0, "bypass")
        Me.imlP20BL_Mouseover.Images.SetKeyName(1, "canc_cont")
        Me.imlP20BL_Mouseover.Images.SetKeyName(2, "canc_only")
        Me.imlP20BL_Mouseover.Images.SetKeyName(3, "cycle")
        Me.imlP20BL_Mouseover.Images.SetKeyName(4, "fault")
        Me.imlP20BL_Mouseover.Images.SetKeyName(5, "home")
        Me.imlP20BL_Mouseover.Images.SetKeyName(6, "maint")
        Me.imlP20BL_Mouseover.Images.SetKeyName(7, "master1")
        Me.imlP20BL_Mouseover.Images.SetKeyName(8, "spec1")
        Me.imlP20BL_Mouseover.Images.SetKeyName(9, "spec2")
        Me.imlP20BL_Mouseover.Images.SetKeyName(10, "unknown")
        '
        'imlP20BR_Mouseover
        '
        Me.imlP20BR_Mouseover.ImageStream = CType(resources.GetObject("imlP20BR_Mouseover.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP20BR_Mouseover.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP20BR_Mouseover.Images.SetKeyName(0, "bypass")
        Me.imlP20BR_Mouseover.Images.SetKeyName(1, "canc_cont")
        Me.imlP20BR_Mouseover.Images.SetKeyName(2, "canc_only")
        Me.imlP20BR_Mouseover.Images.SetKeyName(3, "cycle")
        Me.imlP20BR_Mouseover.Images.SetKeyName(4, "fault")
        Me.imlP20BR_Mouseover.Images.SetKeyName(5, "home")
        Me.imlP20BR_Mouseover.Images.SetKeyName(6, "maint")
        Me.imlP20BR_Mouseover.Images.SetKeyName(7, "master1")
        Me.imlP20BR_Mouseover.Images.SetKeyName(8, "spec1")
        Me.imlP20BR_Mouseover.Images.SetKeyName(9, "spec2")
        Me.imlP20BR_Mouseover.Images.SetKeyName(10, "unknown")
        '
        'imlP20TL_Mouseover
        '
        Me.imlP20TL_Mouseover.ImageStream = CType(resources.GetObject("imlP20TL_Mouseover.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP20TL_Mouseover.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP20TL_Mouseover.Images.SetKeyName(0, "bypass")
        Me.imlP20TL_Mouseover.Images.SetKeyName(1, "canc_cont")
        Me.imlP20TL_Mouseover.Images.SetKeyName(2, "canc_only")
        Me.imlP20TL_Mouseover.Images.SetKeyName(3, "cycle")
        Me.imlP20TL_Mouseover.Images.SetKeyName(4, "fault")
        Me.imlP20TL_Mouseover.Images.SetKeyName(5, "home")
        Me.imlP20TL_Mouseover.Images.SetKeyName(6, "maint")
        Me.imlP20TL_Mouseover.Images.SetKeyName(7, "master1")
        Me.imlP20TL_Mouseover.Images.SetKeyName(8, "spec1")
        Me.imlP20TL_Mouseover.Images.SetKeyName(9, "spec2")
        Me.imlP20TL_Mouseover.Images.SetKeyName(10, "unknown")
        '
        'imlP20TR_Mouseover
        '
        Me.imlP20TR_Mouseover.ImageStream = CType(resources.GetObject("imlP20TR_Mouseover.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP20TR_Mouseover.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP20TR_Mouseover.Images.SetKeyName(0, "bypass")
        Me.imlP20TR_Mouseover.Images.SetKeyName(1, "canc_cont")
        Me.imlP20TR_Mouseover.Images.SetKeyName(2, "canc_only")
        Me.imlP20TR_Mouseover.Images.SetKeyName(3, "cycle")
        Me.imlP20TR_Mouseover.Images.SetKeyName(4, "fault")
        Me.imlP20TR_Mouseover.Images.SetKeyName(5, "home")
        Me.imlP20TR_Mouseover.Images.SetKeyName(6, "maint")
        Me.imlP20TR_Mouseover.Images.SetKeyName(7, "master1")
        Me.imlP20TR_Mouseover.Images.SetKeyName(8, "spec1")
        Me.imlP20TR_Mouseover.Images.SetKeyName(9, "spec2")
        Me.imlP20TR_Mouseover.Images.SetKeyName(10, "unknown")
        '
        'imlP20BL_Selected
        '
        Me.imlP20BL_Selected.ImageStream = CType(resources.GetObject("imlP20BL_Selected.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP20BL_Selected.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP20BL_Selected.Images.SetKeyName(0, "bypass")
        Me.imlP20BL_Selected.Images.SetKeyName(1, "canc_cont")
        Me.imlP20BL_Selected.Images.SetKeyName(2, "canc_only")
        Me.imlP20BL_Selected.Images.SetKeyName(3, "cycle")
        Me.imlP20BL_Selected.Images.SetKeyName(4, "fault")
        Me.imlP20BL_Selected.Images.SetKeyName(5, "home")
        Me.imlP20BL_Selected.Images.SetKeyName(6, "maint")
        Me.imlP20BL_Selected.Images.SetKeyName(7, "master1")
        Me.imlP20BL_Selected.Images.SetKeyName(8, "spec1")
        Me.imlP20BL_Selected.Images.SetKeyName(9, "spec2")
        Me.imlP20BL_Selected.Images.SetKeyName(10, "unknown")
        '
        'imlP20BR_Selected
        '
        Me.imlP20BR_Selected.ImageStream = CType(resources.GetObject("imlP20BR_Selected.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP20BR_Selected.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP20BR_Selected.Images.SetKeyName(0, "bypass")
        Me.imlP20BR_Selected.Images.SetKeyName(1, "canc_cont")
        Me.imlP20BR_Selected.Images.SetKeyName(2, "canc_only")
        Me.imlP20BR_Selected.Images.SetKeyName(3, "cycle")
        Me.imlP20BR_Selected.Images.SetKeyName(4, "fault")
        Me.imlP20BR_Selected.Images.SetKeyName(5, "home")
        Me.imlP20BR_Selected.Images.SetKeyName(6, "maint")
        Me.imlP20BR_Selected.Images.SetKeyName(7, "master1")
        Me.imlP20BR_Selected.Images.SetKeyName(8, "spec1")
        Me.imlP20BR_Selected.Images.SetKeyName(9, "spec2")
        Me.imlP20BR_Selected.Images.SetKeyName(10, "unknown")
        '
        'imlP20TL_Selected
        '
        Me.imlP20TL_Selected.ImageStream = CType(resources.GetObject("imlP20TL_Selected.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP20TL_Selected.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP20TL_Selected.Images.SetKeyName(0, "bypass")
        Me.imlP20TL_Selected.Images.SetKeyName(1, "canc_cont")
        Me.imlP20TL_Selected.Images.SetKeyName(2, "canc_only")
        Me.imlP20TL_Selected.Images.SetKeyName(3, "cycle")
        Me.imlP20TL_Selected.Images.SetKeyName(4, "fault")
        Me.imlP20TL_Selected.Images.SetKeyName(5, "home")
        Me.imlP20TL_Selected.Images.SetKeyName(6, "maint")
        Me.imlP20TL_Selected.Images.SetKeyName(7, "master1")
        Me.imlP20TL_Selected.Images.SetKeyName(8, "spec1")
        Me.imlP20TL_Selected.Images.SetKeyName(9, "spec2")
        Me.imlP20TL_Selected.Images.SetKeyName(10, "unknown")
        '
        'imlP20TR_Selected
        '
        Me.imlP20TR_Selected.ImageStream = CType(resources.GetObject("imlP20TR_Selected.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP20TR_Selected.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP20TR_Selected.Images.SetKeyName(0, "bypass")
        Me.imlP20TR_Selected.Images.SetKeyName(1, "canc_cont")
        Me.imlP20TR_Selected.Images.SetKeyName(2, "canc_only")
        Me.imlP20TR_Selected.Images.SetKeyName(3, "cycle")
        Me.imlP20TR_Selected.Images.SetKeyName(4, "fault")
        Me.imlP20TR_Selected.Images.SetKeyName(5, "home")
        Me.imlP20TR_Selected.Images.SetKeyName(6, "maint")
        Me.imlP20TR_Selected.Images.SetKeyName(7, "master1")
        Me.imlP20TR_Selected.Images.SetKeyName(8, "spec1")
        Me.imlP20TR_Selected.Images.SetKeyName(9, "spec2")
        Me.imlP20TR_Selected.Images.SetKeyName(10, "unknown")
        '
        'imlP25BL_Normal
        '
        Me.imlP25BL_Normal.ImageStream = CType(resources.GetObject("imlP25BL_Normal.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP25BL_Normal.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP25BL_Normal.Images.SetKeyName(0, "bypass")
        Me.imlP25BL_Normal.Images.SetKeyName(1, "canc_cont")
        Me.imlP25BL_Normal.Images.SetKeyName(2, "canc_only")
        Me.imlP25BL_Normal.Images.SetKeyName(3, "cycle")
        Me.imlP25BL_Normal.Images.SetKeyName(4, "fault")
        Me.imlP25BL_Normal.Images.SetKeyName(5, "home")
        Me.imlP25BL_Normal.Images.SetKeyName(6, "maint")
        Me.imlP25BL_Normal.Images.SetKeyName(7, "master1")
        Me.imlP25BL_Normal.Images.SetKeyName(8, "spec1")
        Me.imlP25BL_Normal.Images.SetKeyName(9, "spec2")
        Me.imlP25BL_Normal.Images.SetKeyName(10, "unknown")
        '
        'imlP25BR_Normal
        '
        Me.imlP25BR_Normal.ImageStream = CType(resources.GetObject("imlP25BR_Normal.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP25BR_Normal.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP25BR_Normal.Images.SetKeyName(0, "bypass")
        Me.imlP25BR_Normal.Images.SetKeyName(1, "canc_cont")
        Me.imlP25BR_Normal.Images.SetKeyName(2, "canc_only")
        Me.imlP25BR_Normal.Images.SetKeyName(3, "cycle")
        Me.imlP25BR_Normal.Images.SetKeyName(4, "fault")
        Me.imlP25BR_Normal.Images.SetKeyName(5, "home")
        Me.imlP25BR_Normal.Images.SetKeyName(6, "maint")
        Me.imlP25BR_Normal.Images.SetKeyName(7, "master1")
        Me.imlP25BR_Normal.Images.SetKeyName(8, "spec1")
        Me.imlP25BR_Normal.Images.SetKeyName(9, "spec2")
        Me.imlP25BR_Normal.Images.SetKeyName(10, "unknown")
        '
        'imlP25TL_Normal
        '
        Me.imlP25TL_Normal.ImageStream = CType(resources.GetObject("imlP25TL_Normal.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP25TL_Normal.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP25TL_Normal.Images.SetKeyName(0, "bypass")
        Me.imlP25TL_Normal.Images.SetKeyName(1, "canc_cont")
        Me.imlP25TL_Normal.Images.SetKeyName(2, "canc_only")
        Me.imlP25TL_Normal.Images.SetKeyName(3, "cycle")
        Me.imlP25TL_Normal.Images.SetKeyName(4, "fault")
        Me.imlP25TL_Normal.Images.SetKeyName(5, "home")
        Me.imlP25TL_Normal.Images.SetKeyName(6, "maint")
        Me.imlP25TL_Normal.Images.SetKeyName(7, "master1")
        Me.imlP25TL_Normal.Images.SetKeyName(8, "spec1")
        Me.imlP25TL_Normal.Images.SetKeyName(9, "spec2")
        Me.imlP25TL_Normal.Images.SetKeyName(10, "unknown")
        '
        'imlP25TR_Normal
        '
        Me.imlP25TR_Normal.ImageStream = CType(resources.GetObject("imlP25TR_Normal.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP25TR_Normal.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP25TR_Normal.Images.SetKeyName(0, "bypass")
        Me.imlP25TR_Normal.Images.SetKeyName(1, "canc_cont")
        Me.imlP25TR_Normal.Images.SetKeyName(2, "canc_only")
        Me.imlP25TR_Normal.Images.SetKeyName(3, "cycle")
        Me.imlP25TR_Normal.Images.SetKeyName(4, "fault")
        Me.imlP25TR_Normal.Images.SetKeyName(5, "home")
        Me.imlP25TR_Normal.Images.SetKeyName(6, "maint")
        Me.imlP25TR_Normal.Images.SetKeyName(7, "master1")
        Me.imlP25TR_Normal.Images.SetKeyName(8, "spec1")
        Me.imlP25TR_Normal.Images.SetKeyName(9, "spec2")
        Me.imlP25TR_Normal.Images.SetKeyName(10, "unknown")
        '
        'imlP25BL_Mouseover
        '
        Me.imlP25BL_Mouseover.ImageStream = CType(resources.GetObject("imlP25BL_Mouseover.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP25BL_Mouseover.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP25BL_Mouseover.Images.SetKeyName(0, "bypass")
        Me.imlP25BL_Mouseover.Images.SetKeyName(1, "canc_cont")
        Me.imlP25BL_Mouseover.Images.SetKeyName(2, "canc_only")
        Me.imlP25BL_Mouseover.Images.SetKeyName(3, "cycle")
        Me.imlP25BL_Mouseover.Images.SetKeyName(4, "fault")
        Me.imlP25BL_Mouseover.Images.SetKeyName(5, "home")
        Me.imlP25BL_Mouseover.Images.SetKeyName(6, "maint")
        Me.imlP25BL_Mouseover.Images.SetKeyName(7, "master1")
        Me.imlP25BL_Mouseover.Images.SetKeyName(8, "spec1")
        Me.imlP25BL_Mouseover.Images.SetKeyName(9, "spec2")
        Me.imlP25BL_Mouseover.Images.SetKeyName(10, "unknown")
        '
        'imlP25BR_Mouseover
        '
        Me.imlP25BR_Mouseover.ImageStream = CType(resources.GetObject("imlP25BR_Mouseover.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP25BR_Mouseover.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP25BR_Mouseover.Images.SetKeyName(0, "bypass")
        Me.imlP25BR_Mouseover.Images.SetKeyName(1, "canc_cont")
        Me.imlP25BR_Mouseover.Images.SetKeyName(2, "canc_only")
        Me.imlP25BR_Mouseover.Images.SetKeyName(3, "cycle")
        Me.imlP25BR_Mouseover.Images.SetKeyName(4, "fault")
        Me.imlP25BR_Mouseover.Images.SetKeyName(5, "home")
        Me.imlP25BR_Mouseover.Images.SetKeyName(6, "maint")
        Me.imlP25BR_Mouseover.Images.SetKeyName(7, "master1")
        Me.imlP25BR_Mouseover.Images.SetKeyName(8, "spec1")
        Me.imlP25BR_Mouseover.Images.SetKeyName(9, "spec2")
        Me.imlP25BR_Mouseover.Images.SetKeyName(10, "unknown")
        '
        'imlP25TL_Mouseover
        '
        Me.imlP25TL_Mouseover.ImageStream = CType(resources.GetObject("imlP25TL_Mouseover.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP25TL_Mouseover.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP25TL_Mouseover.Images.SetKeyName(0, "bypass")
        Me.imlP25TL_Mouseover.Images.SetKeyName(1, "canc_cont")
        Me.imlP25TL_Mouseover.Images.SetKeyName(2, "canc_only")
        Me.imlP25TL_Mouseover.Images.SetKeyName(3, "cycle")
        Me.imlP25TL_Mouseover.Images.SetKeyName(4, "fault")
        Me.imlP25TL_Mouseover.Images.SetKeyName(5, "home")
        Me.imlP25TL_Mouseover.Images.SetKeyName(6, "maint")
        Me.imlP25TL_Mouseover.Images.SetKeyName(7, "master1")
        Me.imlP25TL_Mouseover.Images.SetKeyName(8, "spec1")
        Me.imlP25TL_Mouseover.Images.SetKeyName(9, "spec2")
        Me.imlP25TL_Mouseover.Images.SetKeyName(10, "unknown")
        '
        'imlP25TR_Mouseover
        '
        Me.imlP25TR_Mouseover.ImageStream = CType(resources.GetObject("imlP25TR_Mouseover.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP25TR_Mouseover.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP25TR_Mouseover.Images.SetKeyName(0, "bypass")
        Me.imlP25TR_Mouseover.Images.SetKeyName(1, "canc_cont")
        Me.imlP25TR_Mouseover.Images.SetKeyName(2, "canc_only")
        Me.imlP25TR_Mouseover.Images.SetKeyName(3, "cycle")
        Me.imlP25TR_Mouseover.Images.SetKeyName(4, "fault")
        Me.imlP25TR_Mouseover.Images.SetKeyName(5, "home")
        Me.imlP25TR_Mouseover.Images.SetKeyName(6, "maint")
        Me.imlP25TR_Mouseover.Images.SetKeyName(7, "master1")
        Me.imlP25TR_Mouseover.Images.SetKeyName(8, "spec1")
        Me.imlP25TR_Mouseover.Images.SetKeyName(9, "spec2")
        Me.imlP25TR_Mouseover.Images.SetKeyName(10, "unknown")
        '
        'imlP25BL_Selected
        '
        Me.imlP25BL_Selected.ImageStream = CType(resources.GetObject("imlP25BL_Selected.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP25BL_Selected.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP25BL_Selected.Images.SetKeyName(0, "bypass")
        Me.imlP25BL_Selected.Images.SetKeyName(1, "canc_cont")
        Me.imlP25BL_Selected.Images.SetKeyName(2, "canc_only")
        Me.imlP25BL_Selected.Images.SetKeyName(3, "cycle")
        Me.imlP25BL_Selected.Images.SetKeyName(4, "fault")
        Me.imlP25BL_Selected.Images.SetKeyName(5, "home")
        Me.imlP25BL_Selected.Images.SetKeyName(6, "maint")
        Me.imlP25BL_Selected.Images.SetKeyName(7, "master1")
        Me.imlP25BL_Selected.Images.SetKeyName(8, "spec1")
        Me.imlP25BL_Selected.Images.SetKeyName(9, "spec2")
        Me.imlP25BL_Selected.Images.SetKeyName(10, "unknown")
        '
        'imlP25BR_Selected
        '
        Me.imlP25BR_Selected.ImageStream = CType(resources.GetObject("imlP25BR_Selected.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP25BR_Selected.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP25BR_Selected.Images.SetKeyName(0, "bypass")
        Me.imlP25BR_Selected.Images.SetKeyName(1, "canc_cont")
        Me.imlP25BR_Selected.Images.SetKeyName(2, "canc_only")
        Me.imlP25BR_Selected.Images.SetKeyName(3, "cycle")
        Me.imlP25BR_Selected.Images.SetKeyName(4, "fault")
        Me.imlP25BR_Selected.Images.SetKeyName(5, "home")
        Me.imlP25BR_Selected.Images.SetKeyName(6, "maint")
        Me.imlP25BR_Selected.Images.SetKeyName(7, "master1")
        Me.imlP25BR_Selected.Images.SetKeyName(8, "spec1")
        Me.imlP25BR_Selected.Images.SetKeyName(9, "spec2")
        Me.imlP25BR_Selected.Images.SetKeyName(10, "unknown")
        '
        'imlP25TL_Selected
        '
        Me.imlP25TL_Selected.ImageStream = CType(resources.GetObject("imlP25TL_Selected.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP25TL_Selected.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP25TL_Selected.Images.SetKeyName(0, "bypass")
        Me.imlP25TL_Selected.Images.SetKeyName(1, "canc_cont")
        Me.imlP25TL_Selected.Images.SetKeyName(2, "canc_only")
        Me.imlP25TL_Selected.Images.SetKeyName(3, "cycle")
        Me.imlP25TL_Selected.Images.SetKeyName(4, "fault")
        Me.imlP25TL_Selected.Images.SetKeyName(5, "home")
        Me.imlP25TL_Selected.Images.SetKeyName(6, "maint")
        Me.imlP25TL_Selected.Images.SetKeyName(7, "master1")
        Me.imlP25TL_Selected.Images.SetKeyName(8, "spec1")
        Me.imlP25TL_Selected.Images.SetKeyName(9, "spec2")
        Me.imlP25TL_Selected.Images.SetKeyName(10, "unknown")
        '
        'imlP25TR_Selected
        '
        Me.imlP25TR_Selected.ImageStream = CType(resources.GetObject("imlP25TR_Selected.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP25TR_Selected.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP25TR_Selected.Images.SetKeyName(0, "bypass")
        Me.imlP25TR_Selected.Images.SetKeyName(1, "canc_cont")
        Me.imlP25TR_Selected.Images.SetKeyName(2, "canc_only")
        Me.imlP25TR_Selected.Images.SetKeyName(3, "cycle")
        Me.imlP25TR_Selected.Images.SetKeyName(4, "fault")
        Me.imlP25TR_Selected.Images.SetKeyName(5, "home")
        Me.imlP25TR_Selected.Images.SetKeyName(6, "master1")
        Me.imlP25TR_Selected.Images.SetKeyName(7, "spec1")
        Me.imlP25TR_Selected.Images.SetKeyName(8, "spec2")
        Me.imlP25TR_Selected.Images.SetKeyName(9, "unknown")
        Me.imlP25TR_Selected.Images.SetKeyName(10, "maint")
        '
        'imlMaintDoor
        '
        Me.imlMaintDoor.ImageStream = CType(resources.GetObject("imlMaintDoor.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlMaintDoor.TransparentColor = System.Drawing.Color.Transparent
        Me.imlMaintDoor.Images.SetKeyName(0, "maint_lock_gray")
        Me.imlMaintDoor.Images.SetKeyName(1, "maint_lock_green")
        Me.imlMaintDoor.Images.SetKeyName(2, "maint_lock_red")
        Me.imlMaintDoor.Images.SetKeyName(3, "maint_lock_yellow")
        '
        'imlP500T_Mouseover
        '
        Me.imlP500T_Mouseover.ImageStream = CType(resources.GetObject("imlP500T_Mouseover.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP500T_Mouseover.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.imlP500T_Mouseover.Images.SetKeyName(0, "bypass")
        Me.imlP500T_Mouseover.Images.SetKeyName(1, "colchg")
        Me.imlP500T_Mouseover.Images.SetKeyName(2, "canc_cont")
        Me.imlP500T_Mouseover.Images.SetKeyName(3, "canc_only")
        Me.imlP500T_Mouseover.Images.SetKeyName(4, "guntest")
        Me.imlP500T_Mouseover.Images.SetKeyName(5, "home_trigoff")
        Me.imlP500T_Mouseover.Images.SetKeyName(6, "home_appoff")
        Me.imlP500T_Mouseover.Images.SetKeyName(7, "home")
        Me.imlP500T_Mouseover.Images.SetKeyName(8, "cycle_trigoff")
        Me.imlP500T_Mouseover.Images.SetKeyName(9, "cycle_appoff")
        Me.imlP500T_Mouseover.Images.SetKeyName(10, "cycle")
        Me.imlP500T_Mouseover.Images.SetKeyName(11, "purge")
        Me.imlP500T_Mouseover.Images.SetKeyName(12, "spec1")
        Me.imlP500T_Mouseover.Images.SetKeyName(13, "spec2")
        Me.imlP500T_Mouseover.Images.SetKeyName(14, "cycle_spray")
        Me.imlP500T_Mouseover.Images.SetKeyName(15, "unknown")
        Me.imlP500T_Mouseover.Images.SetKeyName(16, "master1")
        Me.imlP500T_Mouseover.Images.SetKeyName(17, "fault")
        '
        'imlP500B_Normal
        '
        Me.imlP500B_Normal.ImageStream = CType(resources.GetObject("imlP500B_Normal.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP500B_Normal.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.imlP500B_Normal.Images.SetKeyName(0, "bypass")
        Me.imlP500B_Normal.Images.SetKeyName(1, "colchg")
        Me.imlP500B_Normal.Images.SetKeyName(2, "canc_cont")
        Me.imlP500B_Normal.Images.SetKeyName(3, "canc_only")
        Me.imlP500B_Normal.Images.SetKeyName(4, "guntest")
        Me.imlP500B_Normal.Images.SetKeyName(5, "home_trigoff")
        Me.imlP500B_Normal.Images.SetKeyName(6, "home_appoff")
        Me.imlP500B_Normal.Images.SetKeyName(7, "home")
        Me.imlP500B_Normal.Images.SetKeyName(8, "cycle_trigoff")
        Me.imlP500B_Normal.Images.SetKeyName(9, "cycle_appoff")
        Me.imlP500B_Normal.Images.SetKeyName(10, "cycle")
        Me.imlP500B_Normal.Images.SetKeyName(11, "purge")
        Me.imlP500B_Normal.Images.SetKeyName(12, "spec1")
        Me.imlP500B_Normal.Images.SetKeyName(13, "spec2")
        Me.imlP500B_Normal.Images.SetKeyName(14, "cycle_spray")
        Me.imlP500B_Normal.Images.SetKeyName(15, "unknown")
        Me.imlP500B_Normal.Images.SetKeyName(16, "master1")
        Me.imlP500B_Normal.Images.SetKeyName(17, "fault")
        '
        'imlP500B_Selected
        '
        Me.imlP500B_Selected.ImageStream = CType(resources.GetObject("imlP500B_Selected.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP500B_Selected.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.imlP500B_Selected.Images.SetKeyName(0, "bypass")
        Me.imlP500B_Selected.Images.SetKeyName(1, "colchg")
        Me.imlP500B_Selected.Images.SetKeyName(2, "canc_cont")
        Me.imlP500B_Selected.Images.SetKeyName(3, "canc_only")
        Me.imlP500B_Selected.Images.SetKeyName(4, "guntest")
        Me.imlP500B_Selected.Images.SetKeyName(5, "home_trigoff")
        Me.imlP500B_Selected.Images.SetKeyName(6, "home_appoff")
        Me.imlP500B_Selected.Images.SetKeyName(7, "home")
        Me.imlP500B_Selected.Images.SetKeyName(8, "cycle_trigoff")
        Me.imlP500B_Selected.Images.SetKeyName(9, "cycle_appoff")
        Me.imlP500B_Selected.Images.SetKeyName(10, "cycle")
        Me.imlP500B_Selected.Images.SetKeyName(11, "purge")
        Me.imlP500B_Selected.Images.SetKeyName(12, "spec1")
        Me.imlP500B_Selected.Images.SetKeyName(13, "spec2")
        Me.imlP500B_Selected.Images.SetKeyName(14, "cycle_spray")
        Me.imlP500B_Selected.Images.SetKeyName(15, "unknown")
        Me.imlP500B_Selected.Images.SetKeyName(16, "master1")
        Me.imlP500B_Selected.Images.SetKeyName(17, "fault")
        '
        'imlP500B_Mouseover
        '
        Me.imlP500B_Mouseover.ImageStream = CType(resources.GetObject("imlP500B_Mouseover.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP500B_Mouseover.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.imlP500B_Mouseover.Images.SetKeyName(0, "bypass")
        Me.imlP500B_Mouseover.Images.SetKeyName(1, "colchg")
        Me.imlP500B_Mouseover.Images.SetKeyName(2, "canc_cont")
        Me.imlP500B_Mouseover.Images.SetKeyName(3, "canc_only")
        Me.imlP500B_Mouseover.Images.SetKeyName(4, "guntest")
        Me.imlP500B_Mouseover.Images.SetKeyName(5, "home_trigoff")
        Me.imlP500B_Mouseover.Images.SetKeyName(6, "home_appoff")
        Me.imlP500B_Mouseover.Images.SetKeyName(7, "home")
        Me.imlP500B_Mouseover.Images.SetKeyName(8, "cycle_trigoff")
        Me.imlP500B_Mouseover.Images.SetKeyName(9, "cycle_appoff")
        Me.imlP500B_Mouseover.Images.SetKeyName(10, "cycle")
        Me.imlP500B_Mouseover.Images.SetKeyName(11, "purge")
        Me.imlP500B_Mouseover.Images.SetKeyName(12, "spec1")
        Me.imlP500B_Mouseover.Images.SetKeyName(13, "spec2")
        Me.imlP500B_Mouseover.Images.SetKeyName(14, "cycle_spray")
        Me.imlP500B_Mouseover.Images.SetKeyName(15, "unknown")
        Me.imlP500B_Mouseover.Images.SetKeyName(16, "master1")
        Me.imlP500B_Mouseover.Images.SetKeyName(17, "fault")
        '
        'imlP500T_Normal
        '
        Me.imlP500T_Normal.ImageStream = CType(resources.GetObject("imlP500T_Normal.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP500T_Normal.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.imlP500T_Normal.Images.SetKeyName(0, "bypass")
        Me.imlP500T_Normal.Images.SetKeyName(1, "colchg")
        Me.imlP500T_Normal.Images.SetKeyName(2, "canc_cont")
        Me.imlP500T_Normal.Images.SetKeyName(3, "canc_only")
        Me.imlP500T_Normal.Images.SetKeyName(4, "guntest")
        Me.imlP500T_Normal.Images.SetKeyName(5, "home_trigoff")
        Me.imlP500T_Normal.Images.SetKeyName(6, "home_appoff")
        Me.imlP500T_Normal.Images.SetKeyName(7, "home")
        Me.imlP500T_Normal.Images.SetKeyName(8, "cycle_trigoff")
        Me.imlP500T_Normal.Images.SetKeyName(9, "cycle_appoff")
        Me.imlP500T_Normal.Images.SetKeyName(10, "cycle")
        Me.imlP500T_Normal.Images.SetKeyName(11, "purge")
        Me.imlP500T_Normal.Images.SetKeyName(12, "spec1")
        Me.imlP500T_Normal.Images.SetKeyName(13, "spec2")
        Me.imlP500T_Normal.Images.SetKeyName(14, "cycle_spray")
        Me.imlP500T_Normal.Images.SetKeyName(15, "unknown")
        Me.imlP500T_Normal.Images.SetKeyName(16, "master1")
        Me.imlP500T_Normal.Images.SetKeyName(17, "fault")
        '
        'imlP500T_Selected
        '
        Me.imlP500T_Selected.ImageStream = CType(resources.GetObject("imlP500T_Selected.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP500T_Selected.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.imlP500T_Selected.Images.SetKeyName(0, "bypass")
        Me.imlP500T_Selected.Images.SetKeyName(1, "colchg")
        Me.imlP500T_Selected.Images.SetKeyName(2, "canc_cont")
        Me.imlP500T_Selected.Images.SetKeyName(3, "canc_only")
        Me.imlP500T_Selected.Images.SetKeyName(4, "guntest")
        Me.imlP500T_Selected.Images.SetKeyName(5, "home_trigoff")
        Me.imlP500T_Selected.Images.SetKeyName(6, "home_appoff")
        Me.imlP500T_Selected.Images.SetKeyName(7, "home")
        Me.imlP500T_Selected.Images.SetKeyName(8, "cycle_trigoff")
        Me.imlP500T_Selected.Images.SetKeyName(9, "cycle_appoff")
        Me.imlP500T_Selected.Images.SetKeyName(10, "cycle")
        Me.imlP500T_Selected.Images.SetKeyName(11, "purge")
        Me.imlP500T_Selected.Images.SetKeyName(12, "spec1")
        Me.imlP500T_Selected.Images.SetKeyName(13, "spec2")
        Me.imlP500T_Selected.Images.SetKeyName(14, "cycle_spray")
        Me.imlP500T_Selected.Images.SetKeyName(15, "unknown")
        Me.imlP500T_Selected.Images.SetKeyName(16, "master1")
        Me.imlP500T_Selected.Images.SetKeyName(17, "fault")
        '
        'imlP200BL_Normal
        '
        Me.imlP200BL_Normal.ImageStream = CType(resources.GetObject("imlP200BL_Normal.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP200BL_Normal.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP200BL_Normal.Images.SetKeyName(0, "bypass")
        Me.imlP200BL_Normal.Images.SetKeyName(1, "canc_cont")
        Me.imlP200BL_Normal.Images.SetKeyName(2, "canc_only")
        Me.imlP200BL_Normal.Images.SetKeyName(3, "colchg")
        Me.imlP200BL_Normal.Images.SetKeyName(4, "cycle")
        Me.imlP200BL_Normal.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP200BL_Normal.Images.SetKeyName(6, "cycle_spray")
        Me.imlP200BL_Normal.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP200BL_Normal.Images.SetKeyName(8, "fault")
        Me.imlP200BL_Normal.Images.SetKeyName(9, "home_appoff")
        Me.imlP200BL_Normal.Images.SetKeyName(10, "home_trigoff")
        Me.imlP200BL_Normal.Images.SetKeyName(11, "home")
        Me.imlP200BL_Normal.Images.SetKeyName(12, "maint")
        Me.imlP200BL_Normal.Images.SetKeyName(13, "master1")
        Me.imlP200BL_Normal.Images.SetKeyName(14, "purge")
        Me.imlP200BL_Normal.Images.SetKeyName(15, "spec1")
        Me.imlP200BL_Normal.Images.SetKeyName(16, "spec2")
        Me.imlP200BL_Normal.Images.SetKeyName(17, "unknown")
        '
        'imlP200BR_Normal
        '
        Me.imlP200BR_Normal.ImageStream = CType(resources.GetObject("imlP200BR_Normal.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP200BR_Normal.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP200BR_Normal.Images.SetKeyName(0, "bypass")
        Me.imlP200BR_Normal.Images.SetKeyName(1, "canc_cont")
        Me.imlP200BR_Normal.Images.SetKeyName(2, "canc_only")
        Me.imlP200BR_Normal.Images.SetKeyName(3, "colchg")
        Me.imlP200BR_Normal.Images.SetKeyName(4, "cycle")
        Me.imlP200BR_Normal.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP200BR_Normal.Images.SetKeyName(6, "cycle_spray")
        Me.imlP200BR_Normal.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP200BR_Normal.Images.SetKeyName(8, "fault")
        Me.imlP200BR_Normal.Images.SetKeyName(9, "home")
        Me.imlP200BR_Normal.Images.SetKeyName(10, "home_appoff")
        Me.imlP200BR_Normal.Images.SetKeyName(11, "home_trigoff")
        Me.imlP200BR_Normal.Images.SetKeyName(12, "maint")
        Me.imlP200BR_Normal.Images.SetKeyName(13, "purge")
        Me.imlP200BR_Normal.Images.SetKeyName(14, "spec1")
        Me.imlP200BR_Normal.Images.SetKeyName(15, "spec2")
        Me.imlP200BR_Normal.Images.SetKeyName(16, "unknown")
        Me.imlP200BR_Normal.Images.SetKeyName(17, "master1")
        '
        'imlP200TL_Normal
        '
        Me.imlP200TL_Normal.ImageStream = CType(resources.GetObject("imlP200TL_Normal.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP200TL_Normal.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP200TL_Normal.Images.SetKeyName(0, "bypass")
        Me.imlP200TL_Normal.Images.SetKeyName(1, "canc_cont")
        Me.imlP200TL_Normal.Images.SetKeyName(2, "canc_only")
        Me.imlP200TL_Normal.Images.SetKeyName(3, "colchg")
        Me.imlP200TL_Normal.Images.SetKeyName(4, "cycle")
        Me.imlP200TL_Normal.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP200TL_Normal.Images.SetKeyName(6, "cycle_spray")
        Me.imlP200TL_Normal.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP200TL_Normal.Images.SetKeyName(8, "fault")
        Me.imlP200TL_Normal.Images.SetKeyName(9, "home")
        Me.imlP200TL_Normal.Images.SetKeyName(10, "home_appoff")
        Me.imlP200TL_Normal.Images.SetKeyName(11, "home_trigoff")
        Me.imlP200TL_Normal.Images.SetKeyName(12, "maint")
        Me.imlP200TL_Normal.Images.SetKeyName(13, "master1")
        Me.imlP200TL_Normal.Images.SetKeyName(14, "purge")
        Me.imlP200TL_Normal.Images.SetKeyName(15, "spec1")
        Me.imlP200TL_Normal.Images.SetKeyName(16, "spec2")
        Me.imlP200TL_Normal.Images.SetKeyName(17, "unknown")
        '
        'imlP200TR_Normal
        '
        Me.imlP200TR_Normal.ImageStream = CType(resources.GetObject("imlP200TR_Normal.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP200TR_Normal.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP200TR_Normal.Images.SetKeyName(0, "bypass")
        Me.imlP200TR_Normal.Images.SetKeyName(1, "canc_cont")
        Me.imlP200TR_Normal.Images.SetKeyName(2, "canc_only")
        Me.imlP200TR_Normal.Images.SetKeyName(3, "colchg")
        Me.imlP200TR_Normal.Images.SetKeyName(4, "cycle")
        Me.imlP200TR_Normal.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP200TR_Normal.Images.SetKeyName(6, "cycle_spray")
        Me.imlP200TR_Normal.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP200TR_Normal.Images.SetKeyName(8, "fault")
        Me.imlP200TR_Normal.Images.SetKeyName(9, "home")
        Me.imlP200TR_Normal.Images.SetKeyName(10, "home_appoff")
        Me.imlP200TR_Normal.Images.SetKeyName(11, "home_trigoff")
        Me.imlP200TR_Normal.Images.SetKeyName(12, "maint")
        Me.imlP200TR_Normal.Images.SetKeyName(13, "master1")
        Me.imlP200TR_Normal.Images.SetKeyName(14, "purge")
        Me.imlP200TR_Normal.Images.SetKeyName(15, "spec1")
        Me.imlP200TR_Normal.Images.SetKeyName(16, "spec2")
        Me.imlP200TR_Normal.Images.SetKeyName(17, "unknown")
        '
        'imlP200BL_Mouseover
        '
        Me.imlP200BL_Mouseover.ImageStream = CType(resources.GetObject("imlP200BL_Mouseover.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP200BL_Mouseover.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP200BL_Mouseover.Images.SetKeyName(0, "bypass")
        Me.imlP200BL_Mouseover.Images.SetKeyName(1, "canc_cont")
        Me.imlP200BL_Mouseover.Images.SetKeyName(2, "canc_only")
        Me.imlP200BL_Mouseover.Images.SetKeyName(3, "colchg")
        Me.imlP200BL_Mouseover.Images.SetKeyName(4, "cycle")
        Me.imlP200BL_Mouseover.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP200BL_Mouseover.Images.SetKeyName(6, "cycle_spray")
        Me.imlP200BL_Mouseover.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP200BL_Mouseover.Images.SetKeyName(8, "fault")
        Me.imlP200BL_Mouseover.Images.SetKeyName(9, "home")
        Me.imlP200BL_Mouseover.Images.SetKeyName(10, "home_appoff")
        Me.imlP200BL_Mouseover.Images.SetKeyName(11, "home_trigoff")
        Me.imlP200BL_Mouseover.Images.SetKeyName(12, "maint")
        Me.imlP200BL_Mouseover.Images.SetKeyName(13, "master1")
        Me.imlP200BL_Mouseover.Images.SetKeyName(14, "purge")
        Me.imlP200BL_Mouseover.Images.SetKeyName(15, "spec1")
        Me.imlP200BL_Mouseover.Images.SetKeyName(16, "spec2")
        Me.imlP200BL_Mouseover.Images.SetKeyName(17, "unknown")
        '
        'imlP200BR_Mouseover
        '
        Me.imlP200BR_Mouseover.ImageStream = CType(resources.GetObject("imlP200BR_Mouseover.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP200BR_Mouseover.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP200BR_Mouseover.Images.SetKeyName(0, "bypass")
        Me.imlP200BR_Mouseover.Images.SetKeyName(1, "canc_cont")
        Me.imlP200BR_Mouseover.Images.SetKeyName(2, "canc_only")
        Me.imlP200BR_Mouseover.Images.SetKeyName(3, "colchg")
        Me.imlP200BR_Mouseover.Images.SetKeyName(4, "cycle")
        Me.imlP200BR_Mouseover.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP200BR_Mouseover.Images.SetKeyName(6, "cycle_spray")
        Me.imlP200BR_Mouseover.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP200BR_Mouseover.Images.SetKeyName(8, "fault")
        Me.imlP200BR_Mouseover.Images.SetKeyName(9, "home")
        Me.imlP200BR_Mouseover.Images.SetKeyName(10, "home_appoff")
        Me.imlP200BR_Mouseover.Images.SetKeyName(11, "home_trigoff")
        Me.imlP200BR_Mouseover.Images.SetKeyName(12, "maint")
        Me.imlP200BR_Mouseover.Images.SetKeyName(13, "master1")
        Me.imlP200BR_Mouseover.Images.SetKeyName(14, "purge")
        Me.imlP200BR_Mouseover.Images.SetKeyName(15, "spec1")
        Me.imlP200BR_Mouseover.Images.SetKeyName(16, "spec2")
        Me.imlP200BR_Mouseover.Images.SetKeyName(17, "unknown")
        '
        'imlP200TL_Mouseover
        '
        Me.imlP200TL_Mouseover.ImageStream = CType(resources.GetObject("imlP200TL_Mouseover.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP200TL_Mouseover.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP200TL_Mouseover.Images.SetKeyName(0, "bypass")
        Me.imlP200TL_Mouseover.Images.SetKeyName(1, "canc_cont")
        Me.imlP200TL_Mouseover.Images.SetKeyName(2, "canc_only")
        Me.imlP200TL_Mouseover.Images.SetKeyName(3, "colchg")
        Me.imlP200TL_Mouseover.Images.SetKeyName(4, "cycle")
        Me.imlP200TL_Mouseover.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP200TL_Mouseover.Images.SetKeyName(6, "cycle_spray")
        Me.imlP200TL_Mouseover.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP200TL_Mouseover.Images.SetKeyName(8, "fault")
        Me.imlP200TL_Mouseover.Images.SetKeyName(9, "home")
        Me.imlP200TL_Mouseover.Images.SetKeyName(10, "home_appoff")
        Me.imlP200TL_Mouseover.Images.SetKeyName(11, "home_trigoff")
        Me.imlP200TL_Mouseover.Images.SetKeyName(12, "maint")
        Me.imlP200TL_Mouseover.Images.SetKeyName(13, "master1")
        Me.imlP200TL_Mouseover.Images.SetKeyName(14, "purge")
        Me.imlP200TL_Mouseover.Images.SetKeyName(15, "spec1")
        Me.imlP200TL_Mouseover.Images.SetKeyName(16, "spec2")
        Me.imlP200TL_Mouseover.Images.SetKeyName(17, "unknown")
        '
        'imlP200TR_Mouseover
        '
        Me.imlP200TR_Mouseover.ImageStream = CType(resources.GetObject("imlP200TR_Mouseover.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP200TR_Mouseover.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP200TR_Mouseover.Images.SetKeyName(0, "bypass")
        Me.imlP200TR_Mouseover.Images.SetKeyName(1, "canc_cont")
        Me.imlP200TR_Mouseover.Images.SetKeyName(2, "canc_only")
        Me.imlP200TR_Mouseover.Images.SetKeyName(3, "colchg")
        Me.imlP200TR_Mouseover.Images.SetKeyName(4, "cycle")
        Me.imlP200TR_Mouseover.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP200TR_Mouseover.Images.SetKeyName(6, "cycle_spray")
        Me.imlP200TR_Mouseover.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP200TR_Mouseover.Images.SetKeyName(8, "fault")
        Me.imlP200TR_Mouseover.Images.SetKeyName(9, "home")
        Me.imlP200TR_Mouseover.Images.SetKeyName(10, "home_appoff")
        Me.imlP200TR_Mouseover.Images.SetKeyName(11, "home_trigoff")
        Me.imlP200TR_Mouseover.Images.SetKeyName(12, "maint")
        Me.imlP200TR_Mouseover.Images.SetKeyName(13, "master1")
        Me.imlP200TR_Mouseover.Images.SetKeyName(14, "purge")
        Me.imlP200TR_Mouseover.Images.SetKeyName(15, "spec1")
        Me.imlP200TR_Mouseover.Images.SetKeyName(16, "spec2")
        Me.imlP200TR_Mouseover.Images.SetKeyName(17, "unknown")
        '
        'imlP200BL_Selected
        '
        Me.imlP200BL_Selected.ImageStream = CType(resources.GetObject("imlP200BL_Selected.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP200BL_Selected.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP200BL_Selected.Images.SetKeyName(0, "bypass")
        Me.imlP200BL_Selected.Images.SetKeyName(1, "canc_cont")
        Me.imlP200BL_Selected.Images.SetKeyName(2, "canc_only")
        Me.imlP200BL_Selected.Images.SetKeyName(3, "colchg")
        Me.imlP200BL_Selected.Images.SetKeyName(4, "cycle")
        Me.imlP200BL_Selected.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP200BL_Selected.Images.SetKeyName(6, "cycle_spray")
        Me.imlP200BL_Selected.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP200BL_Selected.Images.SetKeyName(8, "fault")
        Me.imlP200BL_Selected.Images.SetKeyName(9, "home")
        Me.imlP200BL_Selected.Images.SetKeyName(10, "home_appoff")
        Me.imlP200BL_Selected.Images.SetKeyName(11, "home_trigoff")
        Me.imlP200BL_Selected.Images.SetKeyName(12, "maint")
        Me.imlP200BL_Selected.Images.SetKeyName(13, "purge")
        Me.imlP200BL_Selected.Images.SetKeyName(14, "spec1")
        Me.imlP200BL_Selected.Images.SetKeyName(15, "spec2")
        Me.imlP200BL_Selected.Images.SetKeyName(16, "unknown")
        Me.imlP200BL_Selected.Images.SetKeyName(17, "master1")
        '
        'imlP200BR_Selected
        '
        Me.imlP200BR_Selected.ImageStream = CType(resources.GetObject("imlP200BR_Selected.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP200BR_Selected.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP200BR_Selected.Images.SetKeyName(0, "bypass")
        Me.imlP200BR_Selected.Images.SetKeyName(1, "canc_cont")
        Me.imlP200BR_Selected.Images.SetKeyName(2, "canc_only")
        Me.imlP200BR_Selected.Images.SetKeyName(3, "colchg")
        Me.imlP200BR_Selected.Images.SetKeyName(4, "cycle")
        Me.imlP200BR_Selected.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP200BR_Selected.Images.SetKeyName(6, "cycle_spray")
        Me.imlP200BR_Selected.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP200BR_Selected.Images.SetKeyName(8, "fault")
        Me.imlP200BR_Selected.Images.SetKeyName(9, "home")
        Me.imlP200BR_Selected.Images.SetKeyName(10, "home_appoff")
        Me.imlP200BR_Selected.Images.SetKeyName(11, "home_trigoff")
        Me.imlP200BR_Selected.Images.SetKeyName(12, "maint")
        Me.imlP200BR_Selected.Images.SetKeyName(13, "master1")
        Me.imlP200BR_Selected.Images.SetKeyName(14, "purge")
        Me.imlP200BR_Selected.Images.SetKeyName(15, "spec1")
        Me.imlP200BR_Selected.Images.SetKeyName(16, "spec2")
        Me.imlP200BR_Selected.Images.SetKeyName(17, "unknown")
        '
        'imlP200TL_Selected
        '
        Me.imlP200TL_Selected.ImageStream = CType(resources.GetObject("imlP200TL_Selected.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP200TL_Selected.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP200TL_Selected.Images.SetKeyName(0, "bypass")
        Me.imlP200TL_Selected.Images.SetKeyName(1, "canc_cont")
        Me.imlP200TL_Selected.Images.SetKeyName(2, "canc_only")
        Me.imlP200TL_Selected.Images.SetKeyName(3, "colchg")
        Me.imlP200TL_Selected.Images.SetKeyName(4, "cycle")
        Me.imlP200TL_Selected.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP200TL_Selected.Images.SetKeyName(6, "cycle_spray")
        Me.imlP200TL_Selected.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP200TL_Selected.Images.SetKeyName(8, "fault")
        Me.imlP200TL_Selected.Images.SetKeyName(9, "home")
        Me.imlP200TL_Selected.Images.SetKeyName(10, "home_appoff")
        Me.imlP200TL_Selected.Images.SetKeyName(11, "home_trigoff")
        Me.imlP200TL_Selected.Images.SetKeyName(12, "maint")
        Me.imlP200TL_Selected.Images.SetKeyName(13, "master1")
        Me.imlP200TL_Selected.Images.SetKeyName(14, "purge")
        Me.imlP200TL_Selected.Images.SetKeyName(15, "spec1")
        Me.imlP200TL_Selected.Images.SetKeyName(16, "spec2")
        Me.imlP200TL_Selected.Images.SetKeyName(17, "unknown")
        '
        'imlP200TR_Selected
        '
        Me.imlP200TR_Selected.ImageStream = CType(resources.GetObject("imlP200TR_Selected.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlP200TR_Selected.TransparentColor = System.Drawing.Color.Transparent
        Me.imlP200TR_Selected.Images.SetKeyName(0, "bypass")
        Me.imlP200TR_Selected.Images.SetKeyName(1, "canc_cont")
        Me.imlP200TR_Selected.Images.SetKeyName(2, "canc_only")
        Me.imlP200TR_Selected.Images.SetKeyName(3, "colchg")
        Me.imlP200TR_Selected.Images.SetKeyName(4, "cycle")
        Me.imlP200TR_Selected.Images.SetKeyName(5, "cycle_appoff")
        Me.imlP200TR_Selected.Images.SetKeyName(6, "cycle_spary")
        Me.imlP200TR_Selected.Images.SetKeyName(7, "cycle_trigoff")
        Me.imlP200TR_Selected.Images.SetKeyName(8, "fault")
        Me.imlP200TR_Selected.Images.SetKeyName(9, "home")
        Me.imlP200TR_Selected.Images.SetKeyName(10, "home_appoff")
        Me.imlP200TR_Selected.Images.SetKeyName(11, "home_trigoff")
        Me.imlP200TR_Selected.Images.SetKeyName(12, "maint")
        Me.imlP200TR_Selected.Images.SetKeyName(13, "master1")
        Me.imlP200TR_Selected.Images.SetKeyName(14, "purge")
        Me.imlP200TR_Selected.Images.SetKeyName(15, "spec1")
        Me.imlP200TR_Selected.Images.SetKeyName(16, "spec2")
        Me.imlP200TR_Selected.Images.SetKeyName(17, "unknown")
        '
        'tmrStartupDelay
        '
        Me.tmrStartupDelay.Interval = 2000
        '
        'imlEnabDev
        '
        Me.imlEnabDev.ImageStream = CType(resources.GetObject("imlEnabDev.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlEnabDev.TransparentColor = System.Drawing.Color.Transparent
        Me.imlEnabDev.Images.SetKeyName(0, "off_left")
        Me.imlEnabDev.Images.SetKeyName(1, "off_right")
        Me.imlEnabDev.Images.SetKeyName(2, "fault_left")
        Me.imlEnabDev.Images.SetKeyName(3, "fault_right")
        Me.imlEnabDev.Images.SetKeyName(4, "ok_left")
        Me.imlEnabDev.Images.SetKeyName(5, "ok_right")
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1016, 717)
        Me.ControlBox = False
        Me.Controls.Add(Me.tscMain)
        Me.Controls.Add(Me.lstStatus)
        Me.Controls.Add(Me.stsStatus)
        Me.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Name = "frmMain"
        Me.Text = "BSD"
        CType(Me.pnlTxt, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pnlProg, System.ComponentModel.ISupportInitialize).EndInit()
        Me.stsStatus.ResumeLayout(False)
        Me.stsStatus.PerformLayout()
        Me.tlsMain.ResumeLayout(False)
        Me.tlsMain.PerformLayout()
        Me.tscMain.TopToolStripPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.PerformLayout()
        Me.tscMain.ResumeLayout(False)
        Me.tscMain.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents pnlTxt As System.Windows.Forms.StatusBarPanel
    Friend WithEvents pnlProg As System.Windows.Forms.StatusBarPanel
    Friend WithEvents stsStatus As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tspProgress As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents lblSpacer As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents btnFunction As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents mnuLogin As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLogOut As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuRemote As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLocal As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnClose As System.Windows.Forms.ToolStripButton
    Friend WithEvents lblZone As System.Windows.Forms.ToolStripLabel
    Friend WithEvents cboZone As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents btnChangeLog As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuLast24 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLast7 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAllChanges As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnStatus As System.Windows.Forms.ToolStripButton
    Friend WithEvents lstStatus As System.Windows.Forms.ListBox
    Friend WithEvents tscMain As System.Windows.Forms.ToolStripContainer
    Friend WithEvents btnBooth As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnQueue As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnGhost As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnConv As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnUtil As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents mnuResetJobCount As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents imlLed As System.Windows.Forms.ImageList
    Friend WithEvents imlEstop_Cstop As System.Windows.Forms.ImageList
    Friend WithEvents imlSelSwitch As System.Windows.Forms.ImageList
    Friend WithEvents imlPendant As System.Windows.Forms.ImageList
    Friend WithEvents imlDisc As System.Windows.Forms.ImageList
    Friend WithEvents imlEstat As System.Windows.Forms.ImageList
    Friend WithEvents tmrSA As System.Windows.Forms.Timer
    Friend WithEvents btnProcess As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents imlLimitSwitches As System.Windows.Forms.ImageList
    Friend WithEvents imlPEC As System.Windows.Forms.ImageList
    Friend WithEvents mnuLampTest As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents imlProxSwitch As System.Windows.Forms.ImageList
    Friend WithEvents imlP700BL_Normal As System.Windows.Forms.ImageList
    Friend WithEvents imlP700BR_Normal As System.Windows.Forms.ImageList
    Friend WithEvents imlP700TL_Normal As System.Windows.Forms.ImageList
    Friend WithEvents imlP700TR_Normal As System.Windows.Forms.ImageList
    Friend WithEvents imlP700TR_Mouseover As System.Windows.Forms.ImageList
    Friend WithEvents imlP700BR_Mouseover As System.Windows.Forms.ImageList
    Friend WithEvents imlP700TL_Mouseover As System.Windows.Forms.ImageList
    Friend WithEvents imlP700BL_Mouseover As System.Windows.Forms.ImageList
    Friend WithEvents imlP700BL_Selected As System.Windows.Forms.ImageList
    Friend WithEvents imlP700BR_Selected As System.Windows.Forms.ImageList
    Friend WithEvents imlP700TL_Selected As System.Windows.Forms.ImageList
    Friend WithEvents imlP700TR_Selected As System.Windows.Forms.ImageList
    Friend WithEvents imlP20BL_Normal As System.Windows.Forms.ImageList
    Friend WithEvents imlP20BR_Normal As System.Windows.Forms.ImageList
    Friend WithEvents imlP20TL_Normal As System.Windows.Forms.ImageList
    Friend WithEvents imlP20TR_Normal As System.Windows.Forms.ImageList
    Friend WithEvents imlP20BL_Mouseover As System.Windows.Forms.ImageList
    Friend WithEvents imlP20BR_Mouseover As System.Windows.Forms.ImageList
    Friend WithEvents imlP20TL_Mouseover As System.Windows.Forms.ImageList
    Friend WithEvents imlP20TR_Mouseover As System.Windows.Forms.ImageList
    Friend WithEvents imlP20BL_Selected As System.Windows.Forms.ImageList
    Friend WithEvents imlP20BR_Selected As System.Windows.Forms.ImageList
    Friend WithEvents imlP20TL_Selected As System.Windows.Forms.ImageList
    Friend WithEvents imlP20TR_Selected As System.Windows.Forms.ImageList
    Friend WithEvents imlP25BL_Normal As System.Windows.Forms.ImageList
    Friend WithEvents imlP25BR_Normal As System.Windows.Forms.ImageList
    Friend WithEvents imlP25TL_Normal As System.Windows.Forms.ImageList
    Friend WithEvents imlP25TR_Normal As System.Windows.Forms.ImageList
    Friend WithEvents imlP25BL_Mouseover As System.Windows.Forms.ImageList
    Friend WithEvents imlP25BR_Mouseover As System.Windows.Forms.ImageList
    Friend WithEvents imlP25TL_Mouseover As System.Windows.Forms.ImageList
    Friend WithEvents imlP25TR_Mouseover As System.Windows.Forms.ImageList
    Friend WithEvents imlP25BL_Selected As System.Windows.Forms.ImageList
    Friend WithEvents imlP25BR_Selected As System.Windows.Forms.ImageList
    Friend WithEvents imlP25TL_Selected As System.Windows.Forms.ImageList
    Friend WithEvents imlP25TR_Selected As System.Windows.Forms.ImageList
    Friend WithEvents imlMaintDoor As System.Windows.Forms.ImageList
    Public WithEvents imlP500T_Mouseover As System.Windows.Forms.ImageList
    Public WithEvents imlP500B_Normal As System.Windows.Forms.ImageList
    Public WithEvents imlP500B_Selected As System.Windows.Forms.ImageList
    Public WithEvents imlP500B_Mouseover As System.Windows.Forms.ImageList
    Public WithEvents imlP500T_Normal As System.Windows.Forms.ImageList
    Public WithEvents imlP500T_Selected As System.Windows.Forms.ImageList
    Friend WithEvents imlP200BL_Normal As System.Windows.Forms.ImageList
    Friend WithEvents imlP200BR_Normal As System.Windows.Forms.ImageList
    Friend WithEvents imlP200TL_Normal As System.Windows.Forms.ImageList
    Friend WithEvents imlP200TR_Normal As System.Windows.Forms.ImageList
    Friend WithEvents imlP200BL_Mouseover As System.Windows.Forms.ImageList
    Friend WithEvents imlP200BR_Mouseover As System.Windows.Forms.ImageList
    Friend WithEvents imlP200TL_Mouseover As System.Windows.Forms.ImageList
    Friend WithEvents imlP200TR_Mouseover As System.Windows.Forms.ImageList
    Friend WithEvents imlP200BL_Selected As System.Windows.Forms.ImageList
    Friend WithEvents imlP200BR_Selected As System.Windows.Forms.ImageList
    Friend WithEvents imlP200TL_Selected As System.Windows.Forms.ImageList
    Friend WithEvents imlP200TR_Selected As System.Windows.Forms.ImageList
    Friend WithEvents mnuEstatTest As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tmrStartupDelay As System.Windows.Forms.Timer
    Friend WithEvents btnZDT As System.Windows.Forms.ToolStripButton
    Friend WithEvents imlEnabDev As System.Windows.Forms.ImageList
End Class
