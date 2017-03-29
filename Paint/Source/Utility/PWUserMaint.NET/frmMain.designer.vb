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
        Me.TextBox1 = New System.Windows.Forms.TextBox
        Me.Button2 = New System.Windows.Forms.Button
        Me.Button1 = New System.Windows.Forms.Button
        Me.PnlMain = New System.Windows.Forms.Panel
        Me.tabMain = New System.Windows.Forms.TabControl
        Me.TabPage1 = New System.Windows.Forms.TabPage
        Me.gpbChangePwd = New System.Windows.Forms.GroupBox
        Me.txtChangePwdVerifyNew = New System.Windows.Forms.TextBox
        Me.txtChangePwdNew = New System.Windows.Forms.TextBox
        Me.txtChangePwdCurrent = New System.Windows.Forms.TextBox
        Me.btnChangePwdCancel = New System.Windows.Forms.Button
        Me.btnChangePwdAccept = New System.Windows.Forms.Button
        Me.lblChangePwdVerifyNew = New System.Windows.Forms.Label
        Me.lblChangePwdNew = New System.Windows.Forms.Label
        Me.lblChangePwdCurrent = New System.Windows.Forms.Label
        Me.TabPage2 = New System.Windows.Forms.TabPage
        Me.gpbEditUser = New System.Windows.Forms.GroupBox
        Me.gpbEditGroupPwd = New System.Windows.Forms.GroupBox
        Me.txtEditPwdNewVerify = New System.Windows.Forms.TextBox
        Me.txtEditPwdNew = New System.Windows.Forms.TextBox
        Me.btnEditPwdCancel = New System.Windows.Forms.Button
        Me.btnEditPwdAccept = New System.Windows.Forms.Button
        Me.lblEditPwdNewVerify = New System.Windows.Forms.Label
        Me.lblEditPwdNew = New System.Windows.Forms.Label
        Me.lblSelectUserGroup = New System.Windows.Forms.Label
        Me.cboUserGroup = New System.Windows.Forms.ComboBox
        Me.gpbUserPrivileges = New System.Windows.Forms.GroupBox
        Me.btnUserNotAllowed = New System.Windows.Forms.Button
        Me.btnUserAllowed = New System.Windows.Forms.Button
        Me.trvUserPrivileges = New System.Windows.Forms.TreeView
        Me.btnChange = New System.Windows.Forms.Button
        Me.btnRemoveUser = New System.Windows.Forms.Button
        Me.btnAddUser = New System.Windows.Forms.Button
        Me.lstUsers = New System.Windows.Forms.ListBox
        Me.TabPage3 = New System.Windows.Forms.TabPage
        Me.gpbEditGroup = New System.Windows.Forms.GroupBox
        Me.gpbGroupPrivileges = New System.Windows.Forms.GroupBox
        Me.btnGroupNotAllowed = New System.Windows.Forms.Button
        Me.btnGroupAllowed = New System.Windows.Forms.Button
        Me.trvGroupPrivileges = New System.Windows.Forms.TreeView
        Me.btnRemoveGroup = New System.Windows.Forms.Button
        Me.btnAddGroup = New System.Windows.Forms.Button
        Me.lstGroups = New System.Windows.Forms.ListBox
        Me.TabPage4 = New System.Windows.Forms.TabPage
        Me.gpbMaint = New System.Windows.Forms.GroupBox
        Me.btnDelPriv = New System.Windows.Forms.Button
        Me.btnDelProc = New System.Windows.Forms.Button
        Me.trvMaint = New System.Windows.Forms.TreeView
        Me.gpbSetup = New System.Windows.Forms.GroupBox
        Me.lblTimeoutUnits = New System.Windows.Forms.Label
        Me.lblWarnUnits = New System.Windows.Forms.Label
        Me.ftbAutoLogoutTime = New FocusedTextBox.FocusedTextBox
        Me.ftbAutoLogoutWarn = New FocusedTextBox.FocusedTextBox
        Me.cboEnableAutoLogout = New System.Windows.Forms.ComboBox
        Me.lblAutoLogoutTime = New System.Windows.Forms.Label
        Me.lblAutoLogoutWarn = New System.Windows.Forms.Label
        Me.lblEbableAutoLogout = New System.Windows.Forms.Label
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.lblZone = New System.Windows.Forms.Label
        Me.cboZone = New System.Windows.Forms.ComboBox
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.btnSave = New System.Windows.Forms.ToolStripButton
        Me.btnUndo = New System.Windows.Forms.ToolStripButton
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
        Me.pnlTxt = New System.Windows.Forms.StatusBarPanel
        Me.pnlProg = New System.Windows.Forms.StatusBarPanel
        Me.tscMain.BottomToolStripPanel.SuspendLayout()
        Me.tscMain.ContentPanel.SuspendLayout()
        Me.tscMain.TopToolStripPanel.SuspendLayout()
        Me.tscMain.SuspendLayout()
        Me.stsStatus.SuspendLayout()
        Me.PnlMain.SuspendLayout()
        Me.tabMain.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.gpbChangePwd.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.gpbEditUser.SuspendLayout()
        Me.gpbEditGroupPwd.SuspendLayout()
        Me.gpbUserPrivileges.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.gpbEditGroup.SuspendLayout()
        Me.gpbGroupPrivileges.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.gpbMaint.SuspendLayout()
        Me.gpbSetup.SuspendLayout()
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
        Me.tscMain.ContentPanel.Controls.Add(Me.TextBox1)
        Me.tscMain.ContentPanel.Controls.Add(Me.Button2)
        Me.tscMain.ContentPanel.Controls.Add(Me.Button1)
        Me.tscMain.ContentPanel.Controls.Add(Me.PnlMain)
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
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(418, 30)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(129, 20)
        Me.TextBox1.TabIndex = 22
        Me.TextBox1.Visible = False
        '
        'Button2
        '
        Me.Button2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Button2.Location = New System.Drawing.Point(343, 29)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(65, 20)
        Me.Button2.TabIndex = 21
        Me.Button2.Text = "DelProc"
        Me.Button2.UseVisualStyleBackColor = True
        Me.Button2.Visible = False
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(272, 29)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(65, 20)
        Me.Button1.TabIndex = 20
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        Me.Button1.Visible = False
        '
        'PnlMain
        '
        Me.PnlMain.Controls.Add(Me.tabMain)
        Me.PnlMain.Font = New System.Drawing.Font("Arial", 8.25!)
        Me.PnlMain.Location = New System.Drawing.Point(12, 68)
        Me.PnlMain.Name = "PnlMain"
        Me.PnlMain.Size = New System.Drawing.Size(1213, 677)
        Me.PnlMain.TabIndex = 19
        '
        'tabMain
        '
        Me.tabMain.CausesValidation = False
        Me.tabMain.Controls.Add(Me.TabPage1)
        Me.tabMain.Controls.Add(Me.TabPage2)
        Me.tabMain.Controls.Add(Me.TabPage3)
        Me.tabMain.Controls.Add(Me.TabPage4)
        Me.tabMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabMain.Enabled = False
        Me.tabMain.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold)
        Me.tabMain.Location = New System.Drawing.Point(0, 0)
        Me.tabMain.Name = "tabMain"
        Me.tabMain.SelectedIndex = 0
        Me.tabMain.Size = New System.Drawing.Size(1213, 677)
        Me.tabMain.TabIndex = 1
        Me.tabMain.TabStop = False
        '
        'TabPage1
        '
        Me.TabPage1.AutoScroll = True
        Me.TabPage1.BackColor = System.Drawing.SystemColors.Control
        Me.TabPage1.Controls.Add(Me.gpbChangePwd)
        Me.TabPage1.ForeColor = System.Drawing.Color.Black
        Me.TabPage1.Location = New System.Drawing.Point(4, 25)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(1205, 648)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "TabPage1"
        '
        'gpbChangePwd
        '
        Me.gpbChangePwd.Controls.Add(Me.txtChangePwdVerifyNew)
        Me.gpbChangePwd.Controls.Add(Me.txtChangePwdNew)
        Me.gpbChangePwd.Controls.Add(Me.txtChangePwdCurrent)
        Me.gpbChangePwd.Controls.Add(Me.btnChangePwdCancel)
        Me.gpbChangePwd.Controls.Add(Me.btnChangePwdAccept)
        Me.gpbChangePwd.Controls.Add(Me.lblChangePwdVerifyNew)
        Me.gpbChangePwd.Controls.Add(Me.lblChangePwdNew)
        Me.gpbChangePwd.Controls.Add(Me.lblChangePwdCurrent)
        Me.gpbChangePwd.Location = New System.Drawing.Point(66, 50)
        Me.gpbChangePwd.Name = "gpbChangePwd"
        Me.gpbChangePwd.Size = New System.Drawing.Size(568, 291)
        Me.gpbChangePwd.TabIndex = 0
        Me.gpbChangePwd.TabStop = False
        Me.gpbChangePwd.Text = "gpbChangePwd"
        '
        'txtChangePwdVerifyNew
        '
        Me.txtChangePwdVerifyNew.Location = New System.Drawing.Point(242, 154)
        Me.txtChangePwdVerifyNew.Name = "txtChangePwdVerifyNew"
        Me.txtChangePwdVerifyNew.Size = New System.Drawing.Size(210, 22)
        Me.txtChangePwdVerifyNew.TabIndex = 3
        Me.txtChangePwdVerifyNew.UseSystemPasswordChar = True
        '
        'txtChangePwdNew
        '
        Me.txtChangePwdNew.Location = New System.Drawing.Point(242, 110)
        Me.txtChangePwdNew.Name = "txtChangePwdNew"
        Me.txtChangePwdNew.Size = New System.Drawing.Size(210, 22)
        Me.txtChangePwdNew.TabIndex = 2
        Me.txtChangePwdNew.UseSystemPasswordChar = True
        '
        'txtChangePwdCurrent
        '
        Me.txtChangePwdCurrent.Location = New System.Drawing.Point(242, 66)
        Me.txtChangePwdCurrent.Name = "txtChangePwdCurrent"
        Me.txtChangePwdCurrent.Size = New System.Drawing.Size(210, 22)
        Me.txtChangePwdCurrent.TabIndex = 1
        Me.txtChangePwdCurrent.UseSystemPasswordChar = True
        '
        'btnChangePwdCancel
        '
        Me.btnChangePwdCancel.Location = New System.Drawing.Point(347, 205)
        Me.btnChangePwdCancel.Name = "btnChangePwdCancel"
        Me.btnChangePwdCancel.Size = New System.Drawing.Size(131, 23)
        Me.btnChangePwdCancel.TabIndex = 5
        Me.btnChangePwdCancel.Text = "btnChangePwdCancel"
        Me.btnChangePwdCancel.UseVisualStyleBackColor = True
        '
        'btnChangePwdAccept
        '
        Me.btnChangePwdAccept.Location = New System.Drawing.Point(210, 205)
        Me.btnChangePwdAccept.Name = "btnChangePwdAccept"
        Me.btnChangePwdAccept.Size = New System.Drawing.Size(131, 23)
        Me.btnChangePwdAccept.TabIndex = 4
        Me.btnChangePwdAccept.Text = "btnChangePwdAccept"
        Me.btnChangePwdAccept.UseVisualStyleBackColor = True
        '
        'lblChangePwdVerifyNew
        '
        Me.lblChangePwdVerifyNew.Location = New System.Drawing.Point(29, 157)
        Me.lblChangePwdVerifyNew.Name = "lblChangePwdVerifyNew"
        Me.lblChangePwdVerifyNew.Size = New System.Drawing.Size(198, 16)
        Me.lblChangePwdVerifyNew.TabIndex = 2
        Me.lblChangePwdVerifyNew.Text = "lblChangePwdVerifyNew"
        Me.lblChangePwdVerifyNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblChangePwdNew
        '
        Me.lblChangePwdNew.Location = New System.Drawing.Point(29, 113)
        Me.lblChangePwdNew.Name = "lblChangePwdNew"
        Me.lblChangePwdNew.Size = New System.Drawing.Size(198, 16)
        Me.lblChangePwdNew.TabIndex = 1
        Me.lblChangePwdNew.Text = "lblChangePwdNew"
        Me.lblChangePwdNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblChangePwdCurrent
        '
        Me.lblChangePwdCurrent.Location = New System.Drawing.Point(29, 69)
        Me.lblChangePwdCurrent.Name = "lblChangePwdCurrent"
        Me.lblChangePwdCurrent.Size = New System.Drawing.Size(198, 16)
        Me.lblChangePwdCurrent.TabIndex = 0
        Me.lblChangePwdCurrent.Text = "lblChangePwdCurrent"
        Me.lblChangePwdCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TabPage2
        '
        Me.TabPage2.AutoScroll = True
        Me.TabPage2.BackColor = System.Drawing.SystemColors.Control
        Me.TabPage2.Controls.Add(Me.gpbEditUser)
        Me.TabPage2.Location = New System.Drawing.Point(4, 25)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(1205, 648)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "TabPage2"
        '
        'gpbEditUser
        '
        Me.gpbEditUser.Controls.Add(Me.gpbEditGroupPwd)
        Me.gpbEditUser.Controls.Add(Me.gpbUserPrivileges)
        Me.gpbEditUser.Controls.Add(Me.btnChange)
        Me.gpbEditUser.Controls.Add(Me.btnRemoveUser)
        Me.gpbEditUser.Controls.Add(Me.btnAddUser)
        Me.gpbEditUser.Controls.Add(Me.lstUsers)
        Me.gpbEditUser.Location = New System.Drawing.Point(66, 50)
        Me.gpbEditUser.Name = "gpbEditUser"
        Me.gpbEditUser.Size = New System.Drawing.Size(795, 394)
        Me.gpbEditUser.TabIndex = 0
        Me.gpbEditUser.TabStop = False
        Me.gpbEditUser.Text = "gpbEditUser"
        '
        'gpbEditGroupPwd
        '
        Me.gpbEditGroupPwd.Controls.Add(Me.txtEditPwdNewVerify)
        Me.gpbEditGroupPwd.Controls.Add(Me.txtEditPwdNew)
        Me.gpbEditGroupPwd.Controls.Add(Me.btnEditPwdCancel)
        Me.gpbEditGroupPwd.Controls.Add(Me.btnEditPwdAccept)
        Me.gpbEditGroupPwd.Controls.Add(Me.lblEditPwdNewVerify)
        Me.gpbEditGroupPwd.Controls.Add(Me.lblEditPwdNew)
        Me.gpbEditGroupPwd.Controls.Add(Me.lblSelectUserGroup)
        Me.gpbEditGroupPwd.Controls.Add(Me.cboUserGroup)
        Me.gpbEditGroupPwd.Location = New System.Drawing.Point(312, 21)
        Me.gpbEditGroupPwd.Name = "gpbEditGroupPwd"
        Me.gpbEditGroupPwd.Size = New System.Drawing.Size(448, 347)
        Me.gpbEditGroupPwd.TabIndex = 5
        Me.gpbEditGroupPwd.TabStop = False
        Me.gpbEditGroupPwd.Text = "gpbEditGroupPwd"
        '
        'txtEditPwdNewVerify
        '
        Me.txtEditPwdNewVerify.Location = New System.Drawing.Point(204, 182)
        Me.txtEditPwdNewVerify.Name = "txtEditPwdNewVerify"
        Me.txtEditPwdNewVerify.Size = New System.Drawing.Size(210, 22)
        Me.txtEditPwdNewVerify.TabIndex = 2
        Me.txtEditPwdNewVerify.UseSystemPasswordChar = True
        '
        'txtEditPwdNew
        '
        Me.txtEditPwdNew.Location = New System.Drawing.Point(204, 138)
        Me.txtEditPwdNew.Name = "txtEditPwdNew"
        Me.txtEditPwdNew.Size = New System.Drawing.Size(210, 22)
        Me.txtEditPwdNew.TabIndex = 1
        Me.txtEditPwdNew.UseSystemPasswordChar = True
        '
        'btnEditPwdCancel
        '
        Me.btnEditPwdCancel.Location = New System.Drawing.Point(225, 237)
        Me.btnEditPwdCancel.Name = "btnEditPwdCancel"
        Me.btnEditPwdCancel.Size = New System.Drawing.Size(131, 23)
        Me.btnEditPwdCancel.TabIndex = 4
        Me.btnEditPwdCancel.Text = "btnEditPwdCancel"
        Me.btnEditPwdCancel.UseVisualStyleBackColor = True
        '
        'btnEditPwdAccept
        '
        Me.btnEditPwdAccept.Location = New System.Drawing.Point(88, 237)
        Me.btnEditPwdAccept.Name = "btnEditPwdAccept"
        Me.btnEditPwdAccept.Size = New System.Drawing.Size(131, 23)
        Me.btnEditPwdAccept.TabIndex = 3
        Me.btnEditPwdAccept.Text = "btnEditPwdAccept"
        Me.btnEditPwdAccept.UseVisualStyleBackColor = True
        '
        'lblEditPwdNewVerify
        '
        Me.lblEditPwdNewVerify.Location = New System.Drawing.Point(26, 185)
        Me.lblEditPwdNewVerify.Name = "lblEditPwdNewVerify"
        Me.lblEditPwdNewVerify.Size = New System.Drawing.Size(165, 16)
        Me.lblEditPwdNewVerify.TabIndex = 5
        Me.lblEditPwdNewVerify.Text = "lblEditPwdNewVerify"
        Me.lblEditPwdNewVerify.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblEditPwdNew
        '
        Me.lblEditPwdNew.Location = New System.Drawing.Point(23, 141)
        Me.lblEditPwdNew.Name = "lblEditPwdNew"
        Me.lblEditPwdNew.Size = New System.Drawing.Size(168, 16)
        Me.lblEditPwdNew.TabIndex = 4
        Me.lblEditPwdNew.Text = "lblEditPwdNew"
        Me.lblEditPwdNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblSelectUserGroup
        '
        Me.lblSelectUserGroup.Location = New System.Drawing.Point(26, 97)
        Me.lblSelectUserGroup.Name = "lblSelectUserGroup"
        Me.lblSelectUserGroup.Size = New System.Drawing.Size(165, 16)
        Me.lblSelectUserGroup.TabIndex = 3
        Me.lblSelectUserGroup.Text = "lblSelectUserGroup"
        Me.lblSelectUserGroup.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboUserGroup
        '
        Me.cboUserGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboUserGroup.FormattingEnabled = True
        Me.cboUserGroup.Location = New System.Drawing.Point(204, 94)
        Me.cboUserGroup.Name = "cboUserGroup"
        Me.cboUserGroup.Size = New System.Drawing.Size(210, 24)
        Me.cboUserGroup.TabIndex = 0
        '
        'gpbUserPrivileges
        '
        Me.gpbUserPrivileges.Controls.Add(Me.btnUserNotAllowed)
        Me.gpbUserPrivileges.Controls.Add(Me.btnUserAllowed)
        Me.gpbUserPrivileges.Controls.Add(Me.trvUserPrivileges)
        Me.gpbUserPrivileges.Location = New System.Drawing.Point(312, 27)
        Me.gpbUserPrivileges.Name = "gpbUserPrivileges"
        Me.gpbUserPrivileges.Size = New System.Drawing.Size(448, 347)
        Me.gpbUserPrivileges.TabIndex = 4
        Me.gpbUserPrivileges.TabStop = False
        Me.gpbUserPrivileges.Text = "gpbUserPrivileges"
        '
        'btnUserNotAllowed
        '
        Me.btnUserNotAllowed.Location = New System.Drawing.Point(225, 311)
        Me.btnUserNotAllowed.Name = "btnUserNotAllowed"
        Me.btnUserNotAllowed.Size = New System.Drawing.Size(141, 23)
        Me.btnUserNotAllowed.TabIndex = 2
        Me.btnUserNotAllowed.Text = "btnUserNotAllowed"
        Me.btnUserNotAllowed.UseVisualStyleBackColor = True
        '
        'btnUserAllowed
        '
        Me.btnUserAllowed.Location = New System.Drawing.Point(78, 311)
        Me.btnUserAllowed.Name = "btnUserAllowed"
        Me.btnUserAllowed.Size = New System.Drawing.Size(141, 23)
        Me.btnUserAllowed.TabIndex = 1
        Me.btnUserAllowed.Text = "btnUserAllowed"
        Me.btnUserAllowed.UseVisualStyleBackColor = True
        '
        'trvUserPrivileges
        '
        Me.trvUserPrivileges.Location = New System.Drawing.Point(32, 35)
        Me.trvUserPrivileges.Name = "trvUserPrivileges"
        Me.trvUserPrivileges.Size = New System.Drawing.Size(383, 261)
        Me.trvUserPrivileges.TabIndex = 0
        '
        'btnChange
        '
        Me.btnChange.Location = New System.Drawing.Point(38, 338)
        Me.btnChange.Name = "btnChange"
        Me.btnChange.Size = New System.Drawing.Size(238, 23)
        Me.btnChange.TabIndex = 3
        Me.btnChange.Text = "btnChange"
        Me.btnChange.UseVisualStyleBackColor = True
        '
        'btnRemoveUser
        '
        Me.btnRemoveUser.Location = New System.Drawing.Point(160, 300)
        Me.btnRemoveUser.Name = "btnRemoveUser"
        Me.btnRemoveUser.Size = New System.Drawing.Size(116, 23)
        Me.btnRemoveUser.TabIndex = 2
        Me.btnRemoveUser.Text = "btnRemoveUser"
        Me.btnRemoveUser.UseVisualStyleBackColor = True
        '
        'btnAddUser
        '
        Me.btnAddUser.Location = New System.Drawing.Point(38, 300)
        Me.btnAddUser.Name = "btnAddUser"
        Me.btnAddUser.Size = New System.Drawing.Size(116, 23)
        Me.btnAddUser.TabIndex = 1
        Me.btnAddUser.Text = "btnAddUser"
        Me.btnAddUser.UseVisualStyleBackColor = True
        '
        'lstUsers
        '
        Me.lstUsers.FormattingEnabled = True
        Me.lstUsers.ItemHeight = 16
        Me.lstUsers.Location = New System.Drawing.Point(38, 27)
        Me.lstUsers.Name = "lstUsers"
        Me.lstUsers.Size = New System.Drawing.Size(238, 260)
        Me.lstUsers.TabIndex = 0
        '
        'TabPage3
        '
        Me.TabPage3.AutoScroll = True
        Me.TabPage3.BackColor = System.Drawing.SystemColors.Control
        Me.TabPage3.Controls.Add(Me.gpbEditGroup)
        Me.TabPage3.Location = New System.Drawing.Point(4, 25)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Size = New System.Drawing.Size(1205, 648)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "TabPage3"
        '
        'gpbEditGroup
        '
        Me.gpbEditGroup.Controls.Add(Me.gpbGroupPrivileges)
        Me.gpbEditGroup.Controls.Add(Me.btnRemoveGroup)
        Me.gpbEditGroup.Controls.Add(Me.btnAddGroup)
        Me.gpbEditGroup.Controls.Add(Me.lstGroups)
        Me.gpbEditGroup.Location = New System.Drawing.Point(44, 33)
        Me.gpbEditGroup.Name = "gpbEditGroup"
        Me.gpbEditGroup.Size = New System.Drawing.Size(889, 394)
        Me.gpbEditGroup.TabIndex = 1
        Me.gpbEditGroup.TabStop = False
        Me.gpbEditGroup.Text = "gpbEditGroup"
        '
        'gpbGroupPrivileges
        '
        Me.gpbGroupPrivileges.Controls.Add(Me.btnGroupNotAllowed)
        Me.gpbGroupPrivileges.Controls.Add(Me.btnGroupAllowed)
        Me.gpbGroupPrivileges.Controls.Add(Me.trvGroupPrivileges)
        Me.gpbGroupPrivileges.Location = New System.Drawing.Point(297, 27)
        Me.gpbGroupPrivileges.Name = "gpbGroupPrivileges"
        Me.gpbGroupPrivileges.Size = New System.Drawing.Size(448, 347)
        Me.gpbGroupPrivileges.TabIndex = 4
        Me.gpbGroupPrivileges.TabStop = False
        Me.gpbGroupPrivileges.Text = "gpbGroupPrivileges"
        '
        'btnGroupNotAllowed
        '
        Me.btnGroupNotAllowed.Location = New System.Drawing.Point(225, 311)
        Me.btnGroupNotAllowed.Name = "btnGroupNotAllowed"
        Me.btnGroupNotAllowed.Size = New System.Drawing.Size(141, 23)
        Me.btnGroupNotAllowed.TabIndex = 3
        Me.btnGroupNotAllowed.Text = "btnGroupNotAllowed"
        Me.btnGroupNotAllowed.UseVisualStyleBackColor = True
        '
        'btnGroupAllowed
        '
        Me.btnGroupAllowed.Location = New System.Drawing.Point(78, 311)
        Me.btnGroupAllowed.Name = "btnGroupAllowed"
        Me.btnGroupAllowed.Size = New System.Drawing.Size(141, 23)
        Me.btnGroupAllowed.TabIndex = 2
        Me.btnGroupAllowed.Text = "btnGroupAllowed"
        Me.btnGroupAllowed.UseVisualStyleBackColor = True
        '
        'trvGroupPrivileges
        '
        Me.trvGroupPrivileges.Location = New System.Drawing.Point(32, 29)
        Me.trvGroupPrivileges.Name = "trvGroupPrivileges"
        Me.trvGroupPrivileges.Size = New System.Drawing.Size(383, 267)
        Me.trvGroupPrivileges.TabIndex = 0
        '
        'btnRemoveGroup
        '
        Me.btnRemoveGroup.Location = New System.Drawing.Point(42, 338)
        Me.btnRemoveGroup.Name = "btnRemoveGroup"
        Me.btnRemoveGroup.Size = New System.Drawing.Size(182, 23)
        Me.btnRemoveGroup.TabIndex = 3
        Me.btnRemoveGroup.Text = "btnRemoveGroup"
        Me.btnRemoveGroup.UseVisualStyleBackColor = True
        '
        'btnAddGroup
        '
        Me.btnAddGroup.Location = New System.Drawing.Point(42, 300)
        Me.btnAddGroup.Name = "btnAddGroup"
        Me.btnAddGroup.Size = New System.Drawing.Size(182, 23)
        Me.btnAddGroup.TabIndex = 1
        Me.btnAddGroup.Text = "btnAddGroup"
        Me.btnAddGroup.UseVisualStyleBackColor = True
        '
        'lstGroups
        '
        Me.lstGroups.FormattingEnabled = True
        Me.lstGroups.ItemHeight = 16
        Me.lstGroups.Location = New System.Drawing.Point(14, 27)
        Me.lstGroups.Name = "lstGroups"
        Me.lstGroups.Size = New System.Drawing.Size(238, 260)
        Me.lstGroups.TabIndex = 0
        '
        'TabPage4
        '
        Me.TabPage4.AutoScroll = True
        Me.TabPage4.BackColor = System.Drawing.SystemColors.Control
        Me.TabPage4.Controls.Add(Me.gpbMaint)
        Me.TabPage4.Controls.Add(Me.gpbSetup)
        Me.TabPage4.Location = New System.Drawing.Point(4, 25)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage4.Size = New System.Drawing.Size(1205, 648)
        Me.TabPage4.TabIndex = 3
        Me.TabPage4.Text = "TabPage4"
        '
        'gpbMaint
        '
        Me.gpbMaint.Controls.Add(Me.btnDelPriv)
        Me.gpbMaint.Controls.Add(Me.btnDelProc)
        Me.gpbMaint.Controls.Add(Me.trvMaint)
        Me.gpbMaint.Location = New System.Drawing.Point(464, 44)
        Me.gpbMaint.Name = "gpbMaint"
        Me.gpbMaint.Size = New System.Drawing.Size(448, 347)
        Me.gpbMaint.TabIndex = 5
        Me.gpbMaint.TabStop = False
        Me.gpbMaint.Text = "gpbMaint"
        Me.gpbMaint.Visible = False
        '
        'btnDelPriv
        '
        Me.btnDelPriv.Location = New System.Drawing.Point(225, 311)
        Me.btnDelPriv.Name = "btnDelPriv"
        Me.btnDelPriv.Size = New System.Drawing.Size(141, 23)
        Me.btnDelPriv.TabIndex = 3
        Me.btnDelPriv.Text = "btnDelPriv"
        Me.btnDelPriv.UseVisualStyleBackColor = True
        '
        'btnDelProc
        '
        Me.btnDelProc.Location = New System.Drawing.Point(78, 311)
        Me.btnDelProc.Name = "btnDelProc"
        Me.btnDelProc.Size = New System.Drawing.Size(141, 23)
        Me.btnDelProc.TabIndex = 2
        Me.btnDelProc.Text = "btnDelProc"
        Me.btnDelProc.UseVisualStyleBackColor = True
        '
        'trvMaint
        '
        Me.trvMaint.Location = New System.Drawing.Point(32, 29)
        Me.trvMaint.Name = "trvMaint"
        Me.trvMaint.Size = New System.Drawing.Size(383, 267)
        Me.trvMaint.TabIndex = 0
        '
        'gpbSetup
        '
        Me.gpbSetup.Controls.Add(Me.lblTimeoutUnits)
        Me.gpbSetup.Controls.Add(Me.lblWarnUnits)
        Me.gpbSetup.Controls.Add(Me.ftbAutoLogoutTime)
        Me.gpbSetup.Controls.Add(Me.ftbAutoLogoutWarn)
        Me.gpbSetup.Controls.Add(Me.cboEnableAutoLogout)
        Me.gpbSetup.Controls.Add(Me.lblAutoLogoutTime)
        Me.gpbSetup.Controls.Add(Me.lblAutoLogoutWarn)
        Me.gpbSetup.Controls.Add(Me.lblEbableAutoLogout)
        Me.gpbSetup.Location = New System.Drawing.Point(44, 44)
        Me.gpbSetup.Name = "gpbSetup"
        Me.gpbSetup.Size = New System.Drawing.Size(405, 191)
        Me.gpbSetup.TabIndex = 0
        Me.gpbSetup.TabStop = False
        Me.gpbSetup.Text = "gpbSetup"
        '
        'lblTimeoutUnits
        '
        Me.lblTimeoutUnits.Location = New System.Drawing.Point(252, 138)
        Me.lblTimeoutUnits.Name = "lblTimeoutUnits"
        Me.lblTimeoutUnits.Size = New System.Drawing.Size(130, 16)
        Me.lblTimeoutUnits.TabIndex = 7
        Me.lblTimeoutUnits.Text = "lblTimeoutUnits"
        Me.lblTimeoutUnits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblWarnUnits
        '
        Me.lblWarnUnits.Location = New System.Drawing.Point(252, 94)
        Me.lblWarnUnits.Name = "lblWarnUnits"
        Me.lblWarnUnits.Size = New System.Drawing.Size(130, 16)
        Me.lblWarnUnits.TabIndex = 6
        Me.lblWarnUnits.Text = "lblWarnUnits"
        Me.lblWarnUnits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ftbAutoLogoutTime
        '
        Me.ftbAutoLogoutTime.ForeColor = System.Drawing.Color.Red
        Me.ftbAutoLogoutTime.Location = New System.Drawing.Point(197, 135)
        Me.ftbAutoLogoutTime.MaxLength = 4
        Me.ftbAutoLogoutTime.Name = "ftbAutoLogoutTime"
        Me.ftbAutoLogoutTime.NumericOnly = True
        Me.ftbAutoLogoutTime.Size = New System.Drawing.Size(44, 22)
        Me.ftbAutoLogoutTime.TabIndex = 5
        Me.ftbAutoLogoutTime.TabStop = False
        Me.ftbAutoLogoutTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'ftbAutoLogoutWarn
        '
        Me.ftbAutoLogoutWarn.ForeColor = System.Drawing.Color.Red
        Me.ftbAutoLogoutWarn.Location = New System.Drawing.Point(197, 91)
        Me.ftbAutoLogoutWarn.MaxLength = 4
        Me.ftbAutoLogoutWarn.Name = "ftbAutoLogoutWarn"
        Me.ftbAutoLogoutWarn.NumericOnly = True
        Me.ftbAutoLogoutWarn.Size = New System.Drawing.Size(44, 22)
        Me.ftbAutoLogoutWarn.TabIndex = 4
        Me.ftbAutoLogoutWarn.TabStop = False
        Me.ftbAutoLogoutWarn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'cboEnableAutoLogout
        '
        Me.cboEnableAutoLogout.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboEnableAutoLogout.FormattingEnabled = True
        Me.cboEnableAutoLogout.Location = New System.Drawing.Point(197, 47)
        Me.cboEnableAutoLogout.Name = "cboEnableAutoLogout"
        Me.cboEnableAutoLogout.Size = New System.Drawing.Size(95, 24)
        Me.cboEnableAutoLogout.TabIndex = 3
        '
        'lblAutoLogoutTime
        '
        Me.lblAutoLogoutTime.Location = New System.Drawing.Point(18, 138)
        Me.lblAutoLogoutTime.Name = "lblAutoLogoutTime"
        Me.lblAutoLogoutTime.Size = New System.Drawing.Size(165, 16)
        Me.lblAutoLogoutTime.TabIndex = 2
        Me.lblAutoLogoutTime.Text = "lblAutoLogoutTime"
        Me.lblAutoLogoutTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblAutoLogoutWarn
        '
        Me.lblAutoLogoutWarn.Location = New System.Drawing.Point(18, 94)
        Me.lblAutoLogoutWarn.Name = "lblAutoLogoutWarn"
        Me.lblAutoLogoutWarn.Size = New System.Drawing.Size(165, 16)
        Me.lblAutoLogoutWarn.TabIndex = 1
        Me.lblAutoLogoutWarn.Text = "lblAutoLogoutWarn"
        Me.lblAutoLogoutWarn.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblEbableAutoLogout
        '
        Me.lblEbableAutoLogout.Location = New System.Drawing.Point(18, 50)
        Me.lblEbableAutoLogout.Name = "lblEbableAutoLogout"
        Me.lblEbableAutoLogout.Size = New System.Drawing.Size(165, 16)
        Me.lblEbableAutoLogout.TabIndex = 0
        Me.lblEbableAutoLogout.Text = "lblEbableAutoLogout"
        Me.lblEbableAutoLogout.TextAlign = System.Drawing.ContentAlignment.MiddleRight
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
        Me.lblZone.AutoSize = True
        Me.lblZone.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblZone.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblZone.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblZone.Location = New System.Drawing.Point(56, 8)
        Me.lblZone.Name = "lblZone"
        Me.lblZone.Size = New System.Drawing.Size(65, 19)
        Me.lblZone.TabIndex = 12
        Me.lblZone.Text = "lblZone"
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
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.btnSave, Me.btnUndo, Me.btnPrint, Me.btnChangeLog})
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
        Me.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(70, 57)
        Me.btnSave.Text = "Save"
        Me.btnSave.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
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
        'btnPrint
        '
        Me.btnPrint.DropDownButtonWidth = 13
        Me.btnPrint.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuPrint, Me.mnuPrintPreview, Me.mnuPageSetup, Me.mnuPrintFile, Me.mnuPrintOptions})
        Me.btnPrint.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnPrint.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnPrint.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(53, 57)
        Me.btnPrint.Text = "Print"
        Me.btnPrint.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'mnuPrint
        '
        Me.mnuPrint.Name = "mnuPrint"
        Me.mnuPrint.Size = New System.Drawing.Size(173, 22)
        Me.mnuPrint.Text = "mnuPrint"
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
        Me.PnlMain.ResumeLayout(False)
        Me.tabMain.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.gpbChangePwd.ResumeLayout(False)
        Me.gpbChangePwd.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.gpbEditUser.ResumeLayout(False)
        Me.gpbEditGroupPwd.ResumeLayout(False)
        Me.gpbEditGroupPwd.PerformLayout()
        Me.gpbUserPrivileges.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        Me.gpbEditGroup.ResumeLayout(False)
        Me.gpbGroupPrivileges.ResumeLayout(False)
        Me.TabPage4.ResumeLayout(False)
        Me.gpbMaint.ResumeLayout(False)
        Me.gpbSetup.ResumeLayout(False)
        Me.gpbSetup.PerformLayout()
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
    Friend WithEvents btnSave As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnUndo As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnPrint As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents btnChangeLog As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuLast7 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAllChanges As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLast24 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrint As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PnlMain As System.Windows.Forms.Panel
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents mnuPrintPreview As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPageSetup As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintOptions As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tabMain As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents gpbChangePwd As System.Windows.Forms.GroupBox
    Friend WithEvents lblChangePwdCurrent As System.Windows.Forms.Label
    Friend WithEvents lblChangePwdNew As System.Windows.Forms.Label
    Friend WithEvents lblChangePwdVerifyNew As System.Windows.Forms.Label
    Friend WithEvents btnChangePwdAccept As System.Windows.Forms.Button
    Friend WithEvents btnChangePwdCancel As System.Windows.Forms.Button
    Friend WithEvents gpbEditUser As System.Windows.Forms.GroupBox
    Friend WithEvents btnRemoveUser As System.Windows.Forms.Button
    Friend WithEvents btnAddUser As System.Windows.Forms.Button
    Friend WithEvents lstUsers As System.Windows.Forms.ListBox
    Friend WithEvents btnChange As System.Windows.Forms.Button
    Friend WithEvents gpbUserPrivileges As System.Windows.Forms.GroupBox
    Friend WithEvents btnUserNotAllowed As System.Windows.Forms.Button
    Friend WithEvents btnUserAllowed As System.Windows.Forms.Button
    Friend WithEvents trvUserPrivileges As System.Windows.Forms.TreeView
    Friend WithEvents gpbEditGroup As System.Windows.Forms.GroupBox
    Friend WithEvents gpbGroupPrivileges As System.Windows.Forms.GroupBox
    Friend WithEvents btnGroupNotAllowed As System.Windows.Forms.Button
    Friend WithEvents btnGroupAllowed As System.Windows.Forms.Button
    Friend WithEvents trvGroupPrivileges As System.Windows.Forms.TreeView
    Friend WithEvents btnRemoveGroup As System.Windows.Forms.Button
    Friend WithEvents btnAddGroup As System.Windows.Forms.Button
    Friend WithEvents lstGroups As System.Windows.Forms.ListBox
    Friend WithEvents gpbSetup As System.Windows.Forms.GroupBox
    Friend WithEvents lblAutoLogoutTime As System.Windows.Forms.Label
    Friend WithEvents lblAutoLogoutWarn As System.Windows.Forms.Label
    Friend WithEvents lblEbableAutoLogout As System.Windows.Forms.Label
    Friend WithEvents cboEnableAutoLogout As System.Windows.Forms.ComboBox
    Friend WithEvents ftbAutoLogoutWarn As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbAutoLogoutTime As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblTimeoutUnits As System.Windows.Forms.Label
    Friend WithEvents lblWarnUnits As System.Windows.Forms.Label
    Friend WithEvents txtChangePwdCurrent As System.Windows.Forms.TextBox
    Friend WithEvents txtChangePwdVerifyNew As System.Windows.Forms.TextBox
    Friend WithEvents txtChangePwdNew As System.Windows.Forms.TextBox
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents gpbMaint As System.Windows.Forms.GroupBox
    Friend WithEvents btnDelPriv As System.Windows.Forms.Button
    Friend WithEvents btnDelProc As System.Windows.Forms.Button
    Friend WithEvents trvMaint As System.Windows.Forms.TreeView
    Friend WithEvents gpbEditGroupPwd As System.Windows.Forms.GroupBox
    Friend WithEvents txtEditPwdNewVerify As System.Windows.Forms.TextBox
    Friend WithEvents txtEditPwdNew As System.Windows.Forms.TextBox
    Friend WithEvents btnEditPwdCancel As System.Windows.Forms.Button
    Friend WithEvents btnEditPwdAccept As System.Windows.Forms.Button
    Friend WithEvents lblEditPwdNewVerify As System.Windows.Forms.Label
    Friend WithEvents lblEditPwdNew As System.Windows.Forms.Label
    Friend WithEvents lblSelectUserGroup As System.Windows.Forms.Label
    Friend WithEvents cboUserGroup As System.Windows.Forms.ComboBox
End Class
