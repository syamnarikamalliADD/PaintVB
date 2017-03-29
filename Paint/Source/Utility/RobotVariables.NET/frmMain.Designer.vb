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
        Me.lblRobot = New System.Windows.Forms.Label
        Me.cboRobot = New System.Windows.Forms.ComboBox
        Me.pnlMain = New System.Windows.Forms.Panel
        Me.btnRemove = New System.Windows.Forms.Button
        Me.lblAccess = New System.Windows.Forms.Label
        Me.lblAccessCap = New System.Windows.Forms.Label
        Me.lblProgName = New System.Windows.Forms.Label
        Me.lblProgNameCap = New System.Windows.Forms.Label
        Me.btnDescEdit = New System.Windows.Forms.Button
        Me.lblDescriptionCap = New System.Windows.Forms.Label
        Me.txtDescription = New System.Windows.Forms.TextBox
        Me.lblStorage = New System.Windows.Forms.Label
        Me.lblStorageCap = New System.Windows.Forms.Label
        Me.lblStrLen = New System.Windows.Forms.Label
        Me.lblStrLenCap = New System.Windows.Forms.Label
        Me.lblMax = New System.Windows.Forms.Label
        Me.lblMaxCap = New System.Windows.Forms.Label
        Me.lblMin = New System.Windows.Forms.Label
        Me.lblMinCap = New System.Windows.Forms.Label
        Me.lblVarType = New System.Windows.Forms.Label
        Me.lblVarTypeCap = New System.Windows.Forms.Label
        Me.lblVarName = New System.Windows.Forms.Label
        Me.lblVarNameCap = New System.Windows.Forms.Label
        Me.btnWriteRC = New System.Windows.Forms.Button
        Me.txtVarValue = New System.Windows.Forms.TextBox
        Me.trvBrowse = New System.Windows.Forms.TreeView
        Me.btnAdd = New System.Windows.Forms.Button
        Me.rdoBrowse = New System.Windows.Forms.RadioButton
        Me.rdoList = New System.Windows.Forms.RadioButton
        Me.lblSelectVariableCap = New System.Windows.Forms.Label
        Me.txtVarName = New System.Windows.Forms.TextBox
        Me.chkRC_2 = New System.Windows.Forms.CheckBox
        Me.mnuChkRC = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuSelectAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuUnselectAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuBrowse = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuCopyToAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuCopy = New System.Windows.Forms.ToolStripMenuItem
        Me.chkRC_1 = New System.Windows.Forms.CheckBox
        Me.lstVars = New System.Windows.Forms.ListBox
        Me.lblRCVar2 = New System.Windows.Forms.Label
        Me.lblRCVar1 = New System.Windows.Forms.Label
        Me.lblVariableValueCap = New System.Windows.Forms.Label
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.lblZone = New System.Windows.Forms.Label
        Me.cboZone = New System.Windows.Forms.ComboBox
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.btnSave = New System.Windows.Forms.ToolStripButton
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
        Me.mnuChkRC.SuspendLayout()
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
        Me.tscMain.ContentPanel.Controls.Add(Me.lblRobot)
        Me.tscMain.ContentPanel.Controls.Add(Me.cboRobot)
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
        'lblRobot
        '
        Me.lblRobot.AutoSize = True
        Me.lblRobot.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblRobot.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblRobot.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblRobot.Location = New System.Drawing.Point(236, 8)
        Me.lblRobot.Name = "lblRobot"
        Me.lblRobot.Size = New System.Drawing.Size(74, 19)
        Me.lblRobot.TabIndex = 21
        Me.lblRobot.Text = "lblRobot"
        Me.lblRobot.UseMnemonic = False
        '
        'cboRobot
        '
        Me.cboRobot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboRobot.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboRobot.ItemHeight = 18
        Me.cboRobot.Location = New System.Drawing.Point(186, 33)
        Me.cboRobot.Name = "cboRobot"
        Me.cboRobot.Size = New System.Drawing.Size(175, 26)
        Me.cboRobot.TabIndex = 20
        '
        'pnlMain
        '
        Me.pnlMain.Controls.Add(Me.btnRemove)
        Me.pnlMain.Controls.Add(Me.lblAccess)
        Me.pnlMain.Controls.Add(Me.lblAccessCap)
        Me.pnlMain.Controls.Add(Me.lblProgName)
        Me.pnlMain.Controls.Add(Me.lblProgNameCap)
        Me.pnlMain.Controls.Add(Me.btnDescEdit)
        Me.pnlMain.Controls.Add(Me.lblDescriptionCap)
        Me.pnlMain.Controls.Add(Me.txtDescription)
        Me.pnlMain.Controls.Add(Me.lblStorage)
        Me.pnlMain.Controls.Add(Me.lblStorageCap)
        Me.pnlMain.Controls.Add(Me.lblStrLen)
        Me.pnlMain.Controls.Add(Me.lblStrLenCap)
        Me.pnlMain.Controls.Add(Me.lblMax)
        Me.pnlMain.Controls.Add(Me.lblMaxCap)
        Me.pnlMain.Controls.Add(Me.lblMin)
        Me.pnlMain.Controls.Add(Me.lblMinCap)
        Me.pnlMain.Controls.Add(Me.lblVarType)
        Me.pnlMain.Controls.Add(Me.lblVarTypeCap)
        Me.pnlMain.Controls.Add(Me.lblVarName)
        Me.pnlMain.Controls.Add(Me.lblVarNameCap)
        Me.pnlMain.Controls.Add(Me.btnWriteRC)
        Me.pnlMain.Controls.Add(Me.txtVarValue)
        Me.pnlMain.Controls.Add(Me.trvBrowse)
        Me.pnlMain.Controls.Add(Me.btnAdd)
        Me.pnlMain.Controls.Add(Me.rdoBrowse)
        Me.pnlMain.Controls.Add(Me.rdoList)
        Me.pnlMain.Controls.Add(Me.lblSelectVariableCap)
        Me.pnlMain.Controls.Add(Me.txtVarName)
        Me.pnlMain.Controls.Add(Me.chkRC_2)
        Me.pnlMain.Controls.Add(Me.chkRC_1)
        Me.pnlMain.Controls.Add(Me.lstVars)
        Me.pnlMain.Controls.Add(Me.lblRCVar2)
        Me.pnlMain.Controls.Add(Me.lblRCVar1)
        Me.pnlMain.Controls.Add(Me.lblVariableValueCap)
        Me.pnlMain.Font = New System.Drawing.Font("Arial", 8.25!)
        Me.pnlMain.Location = New System.Drawing.Point(12, 66)
        Me.pnlMain.Name = "pnlMain"
        Me.pnlMain.Size = New System.Drawing.Size(1213, 677)
        Me.pnlMain.TabIndex = 19
        '
        'btnRemove
        '
        Me.btnRemove.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRemove.Location = New System.Drawing.Point(563, 13)
        Me.btnRemove.Name = "btnRemove"
        Me.btnRemove.Size = New System.Drawing.Size(63, 20)
        Me.btnRemove.TabIndex = 60
        Me.btnRemove.Text = "Remove"
        Me.btnRemove.UseVisualStyleBackColor = True
        '
        'lblAccess
        '
        Me.lblAccess.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblAccess.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblAccess.Location = New System.Drawing.Point(150, 168)
        Me.lblAccess.Name = "lblAccess"
        Me.lblAccess.Size = New System.Drawing.Size(134, 19)
        Me.lblAccess.TabIndex = 59
        Me.lblAccess.Tag = "11"
        Me.lblAccess.Text = "lblAccess"
        Me.lblAccess.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAccessCap
        '
        Me.lblAccessCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblAccessCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblAccessCap.Location = New System.Drawing.Point(8, 168)
        Me.lblAccessCap.Name = "lblAccessCap"
        Me.lblAccessCap.Size = New System.Drawing.Size(134, 19)
        Me.lblAccessCap.TabIndex = 58
        Me.lblAccessCap.Tag = "11"
        Me.lblAccessCap.Text = "lblAccessCap"
        Me.lblAccessCap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblProgName
        '
        Me.lblProgName.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblProgName.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblProgName.Location = New System.Drawing.Point(148, 93)
        Me.lblProgName.Name = "lblProgName"
        Me.lblProgName.Size = New System.Drawing.Size(134, 19)
        Me.lblProgName.TabIndex = 57
        Me.lblProgName.Tag = "11"
        Me.lblProgName.Text = "lblProgName"
        Me.lblProgName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblProgNameCap
        '
        Me.lblProgNameCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblProgNameCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblProgNameCap.Location = New System.Drawing.Point(8, 93)
        Me.lblProgNameCap.Name = "lblProgNameCap"
        Me.lblProgNameCap.Size = New System.Drawing.Size(134, 19)
        Me.lblProgNameCap.TabIndex = 56
        Me.lblProgNameCap.Tag = "11"
        Me.lblProgNameCap.Text = "lblProgNameCap"
        Me.lblProgNameCap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnDescEdit
        '
        Me.btnDescEdit.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.btnDescEdit.Location = New System.Drawing.Point(154, 274)
        Me.btnDescEdit.Name = "btnDescEdit"
        Me.btnDescEdit.Size = New System.Drawing.Size(73, 20)
        Me.btnDescEdit.TabIndex = 55
        Me.btnDescEdit.Text = "btnDescEdit"
        Me.btnDescEdit.UseVisualStyleBackColor = True
        '
        'lblDescriptionCap
        '
        Me.lblDescriptionCap.AutoSize = True
        Me.lblDescriptionCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblDescriptionCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblDescriptionCap.Location = New System.Drawing.Point(8, 274)
        Me.lblDescriptionCap.Name = "lblDescriptionCap"
        Me.lblDescriptionCap.Size = New System.Drawing.Size(146, 19)
        Me.lblDescriptionCap.TabIndex = 54
        Me.lblDescriptionCap.Tag = "11"
        Me.lblDescriptionCap.Text = "lblDescriptionCap"
        Me.lblDescriptionCap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtDescription
        '
        Me.txtDescription.Location = New System.Drawing.Point(3, 300)
        Me.txtDescription.Multiline = True
        Me.txtDescription.Name = "txtDescription"
        Me.txtDescription.ReadOnly = True
        Me.txtDescription.Size = New System.Drawing.Size(276, 149)
        Me.txtDescription.TabIndex = 53
        '
        'lblStorage
        '
        Me.lblStorage.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblStorage.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblStorage.Location = New System.Drawing.Point(150, 143)
        Me.lblStorage.Name = "lblStorage"
        Me.lblStorage.Size = New System.Drawing.Size(134, 19)
        Me.lblStorage.TabIndex = 52
        Me.lblStorage.Tag = "11"
        Me.lblStorage.Text = "lblStorage"
        Me.lblStorage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblStorageCap
        '
        Me.lblStorageCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblStorageCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblStorageCap.Location = New System.Drawing.Point(8, 143)
        Me.lblStorageCap.Name = "lblStorageCap"
        Me.lblStorageCap.Size = New System.Drawing.Size(134, 19)
        Me.lblStorageCap.TabIndex = 51
        Me.lblStorageCap.Tag = "11"
        Me.lblStorageCap.Text = "lblStorageCap"
        Me.lblStorageCap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblStrLen
        '
        Me.lblStrLen.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblStrLen.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblStrLen.Location = New System.Drawing.Point(150, 243)
        Me.lblStrLen.Name = "lblStrLen"
        Me.lblStrLen.Size = New System.Drawing.Size(134, 19)
        Me.lblStrLen.TabIndex = 50
        Me.lblStrLen.Tag = "11"
        Me.lblStrLen.Text = "lblStrLen"
        Me.lblStrLen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblStrLenCap
        '
        Me.lblStrLenCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblStrLenCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblStrLenCap.Location = New System.Drawing.Point(8, 243)
        Me.lblStrLenCap.Name = "lblStrLenCap"
        Me.lblStrLenCap.Size = New System.Drawing.Size(134, 19)
        Me.lblStrLenCap.TabIndex = 49
        Me.lblStrLenCap.Tag = "11"
        Me.lblStrLenCap.Text = "lblStrLenCap"
        Me.lblStrLenCap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblMax
        '
        Me.lblMax.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblMax.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblMax.Location = New System.Drawing.Point(150, 218)
        Me.lblMax.Name = "lblMax"
        Me.lblMax.Size = New System.Drawing.Size(134, 19)
        Me.lblMax.TabIndex = 48
        Me.lblMax.Tag = "11"
        Me.lblMax.Text = "lblMax"
        Me.lblMax.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblMaxCap
        '
        Me.lblMaxCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblMaxCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblMaxCap.Location = New System.Drawing.Point(8, 218)
        Me.lblMaxCap.Name = "lblMaxCap"
        Me.lblMaxCap.Size = New System.Drawing.Size(134, 19)
        Me.lblMaxCap.TabIndex = 47
        Me.lblMaxCap.Tag = "11"
        Me.lblMaxCap.Text = "lblMaxCap"
        Me.lblMaxCap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblMin
        '
        Me.lblMin.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblMin.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblMin.Location = New System.Drawing.Point(150, 193)
        Me.lblMin.Name = "lblMin"
        Me.lblMin.Size = New System.Drawing.Size(134, 19)
        Me.lblMin.TabIndex = 46
        Me.lblMin.Tag = "11"
        Me.lblMin.Text = "lblMin"
        Me.lblMin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblMinCap
        '
        Me.lblMinCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblMinCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblMinCap.Location = New System.Drawing.Point(8, 193)
        Me.lblMinCap.Name = "lblMinCap"
        Me.lblMinCap.Size = New System.Drawing.Size(134, 19)
        Me.lblMinCap.TabIndex = 45
        Me.lblMinCap.Tag = "11"
        Me.lblMinCap.Text = "lblMinCap"
        Me.lblMinCap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblVarType
        '
        Me.lblVarType.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblVarType.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblVarType.Location = New System.Drawing.Point(148, 118)
        Me.lblVarType.Name = "lblVarType"
        Me.lblVarType.Size = New System.Drawing.Size(134, 19)
        Me.lblVarType.TabIndex = 44
        Me.lblVarType.Tag = "11"
        Me.lblVarType.Text = "lblVarType"
        Me.lblVarType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblVarTypeCap
        '
        Me.lblVarTypeCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblVarTypeCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblVarTypeCap.Location = New System.Drawing.Point(8, 118)
        Me.lblVarTypeCap.Name = "lblVarTypeCap"
        Me.lblVarTypeCap.Size = New System.Drawing.Size(134, 19)
        Me.lblVarTypeCap.TabIndex = 43
        Me.lblVarTypeCap.Tag = "11"
        Me.lblVarTypeCap.Text = "lblVarTypeCap"
        Me.lblVarTypeCap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblVarName
        '
        Me.lblVarName.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblVarName.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblVarName.Location = New System.Drawing.Point(8, 43)
        Me.lblVarName.Name = "lblVarName"
        Me.lblVarName.Size = New System.Drawing.Size(276, 50)
        Me.lblVarName.TabIndex = 42
        Me.lblVarName.Tag = "11"
        Me.lblVarName.Text = "lblVarName"
        '
        'lblVarNameCap
        '
        Me.lblVarNameCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblVarNameCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblVarNameCap.Location = New System.Drawing.Point(8, 17)
        Me.lblVarNameCap.Name = "lblVarNameCap"
        Me.lblVarNameCap.Size = New System.Drawing.Size(271, 19)
        Me.lblVarNameCap.TabIndex = 41
        Me.lblVarNameCap.Tag = "11"
        Me.lblVarNameCap.Text = "lblVarNameCap"
        Me.lblVarNameCap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnWriteRC
        '
        Me.btnWriteRC.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.btnWriteRC.Location = New System.Drawing.Point(563, 107)
        Me.btnWriteRC.Name = "btnWriteRC"
        Me.btnWriteRC.Size = New System.Drawing.Size(258, 20)
        Me.btnWriteRC.TabIndex = 39
        Me.btnWriteRC.Text = "btnWriteRC"
        Me.btnWriteRC.UseVisualStyleBackColor = True
        '
        'txtVarValue
        '
        Me.txtVarValue.Location = New System.Drawing.Point(563, 133)
        Me.txtVarValue.Name = "txtVarValue"
        Me.txtVarValue.Size = New System.Drawing.Size(258, 20)
        Me.txtVarValue.TabIndex = 37
        '
        'trvBrowse
        '
        Me.trvBrowse.Location = New System.Drawing.Point(574, 164)
        Me.trvBrowse.Name = "trvBrowse"
        Me.trvBrowse.Size = New System.Drawing.Size(247, 210)
        Me.trvBrowse.TabIndex = 36
        Me.trvBrowse.Visible = False
        '
        'btnAdd
        '
        Me.btnAdd.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.btnAdd.Location = New System.Drawing.Point(510, 13)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(46, 20)
        Me.btnAdd.TabIndex = 35
        Me.btnAdd.Text = "btnAdd"
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'rdoBrowse
        '
        Me.rdoBrowse.AutoSize = True
        Me.rdoBrowse.Font = New System.Drawing.Font("Arial", 10.0!, System.Drawing.FontStyle.Bold)
        Me.rdoBrowse.Location = New System.Drawing.Point(419, 71)
        Me.rdoBrowse.Name = "rdoBrowse"
        Me.rdoBrowse.Size = New System.Drawing.Size(107, 20)
        Me.rdoBrowse.TabIndex = 34
        Me.rdoBrowse.Text = "rdoBrowse "
        Me.rdoBrowse.UseVisualStyleBackColor = True
        '
        'rdoList
        '
        Me.rdoList.AutoSize = True
        Me.rdoList.Checked = True
        Me.rdoList.Font = New System.Drawing.Font("Arial", 10.0!, System.Drawing.FontStyle.Bold)
        Me.rdoList.Location = New System.Drawing.Point(324, 70)
        Me.rdoList.Name = "rdoList"
        Me.rdoList.Size = New System.Drawing.Size(75, 20)
        Me.rdoList.TabIndex = 33
        Me.rdoList.TabStop = True
        Me.rdoList.Text = "rdoList"
        Me.rdoList.UseVisualStyleBackColor = True
        '
        'lblSelectVariableCap
        '
        Me.lblSelectVariableCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblSelectVariableCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblSelectVariableCap.Location = New System.Drawing.Point(290, 14)
        Me.lblSelectVariableCap.Name = "lblSelectVariableCap"
        Me.lblSelectVariableCap.Size = New System.Drawing.Size(214, 19)
        Me.lblSelectVariableCap.TabIndex = 32
        Me.lblSelectVariableCap.Tag = "11"
        Me.lblSelectVariableCap.Text = "lblSelectVariableCap"
        Me.lblSelectVariableCap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtVarName
        '
        Me.txtVarName.Location = New System.Drawing.Point(290, 41)
        Me.txtVarName.Name = "txtVarName"
        Me.txtVarName.Size = New System.Drawing.Size(266, 20)
        Me.txtVarName.TabIndex = 31
        '
        'chkRC_2
        '
        Me.chkRC_2.AutoSize = True
        Me.chkRC_2.ContextMenuStrip = Me.mnuChkRC
        Me.chkRC_2.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.chkRC_2.Location = New System.Drawing.Point(563, 73)
        Me.chkRC_2.Name = "chkRC_2"
        Me.chkRC_2.Size = New System.Drawing.Size(98, 23)
        Me.chkRC_2.TabIndex = 30
        Me.chkRC_2.Text = "chkRC_2"
        Me.chkRC_2.UseVisualStyleBackColor = True
        '
        'mnuChkRC
        '
        Me.mnuChkRC.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuSelectAll, Me.mnuUnselectAll, Me.mnuBrowse, Me.mnuCopyToAll, Me.mnuCopy})
        Me.mnuChkRC.Name = "mnuCheckList"
        Me.mnuChkRC.Size = New System.Drawing.Size(147, 114)
        '
        'mnuSelectAll
        '
        Me.mnuSelectAll.Name = "mnuSelectAll"
        Me.mnuSelectAll.Size = New System.Drawing.Size(146, 22)
        Me.mnuSelectAll.Text = "mnuSelectAll"
        '
        'mnuUnselectAll
        '
        Me.mnuUnselectAll.Name = "mnuUnselectAll"
        Me.mnuUnselectAll.Size = New System.Drawing.Size(146, 22)
        Me.mnuUnselectAll.Text = "mnuUnselectAll"
        '
        'mnuBrowse
        '
        Me.mnuBrowse.Name = "mnuBrowse"
        Me.mnuBrowse.Size = New System.Drawing.Size(146, 22)
        Me.mnuBrowse.Text = "mnuBrowse"
        '
        'mnuCopyToAll
        '
        Me.mnuCopyToAll.Name = "mnuCopyToAll"
        Me.mnuCopyToAll.Size = New System.Drawing.Size(146, 22)
        Me.mnuCopyToAll.Text = "mnuCopyToAll"
        '
        'mnuCopy
        '
        Me.mnuCopy.Name = "mnuCopy"
        Me.mnuCopy.Size = New System.Drawing.Size(146, 22)
        Me.mnuCopy.Text = "mnuCopy"
        '
        'chkRC_1
        '
        Me.chkRC_1.AutoSize = True
        Me.chkRC_1.ContextMenuStrip = Me.mnuChkRC
        Me.chkRC_1.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.chkRC_1.Location = New System.Drawing.Point(563, 39)
        Me.chkRC_1.Name = "chkRC_1"
        Me.chkRC_1.Size = New System.Drawing.Size(98, 23)
        Me.chkRC_1.TabIndex = 29
        Me.chkRC_1.Text = "chkRC_1"
        Me.chkRC_1.UseVisualStyleBackColor = True
        '
        'lstVars
        '
        Me.lstVars.FormattingEnabled = True
        Me.lstVars.ItemHeight = 14
        Me.lstVars.Location = New System.Drawing.Point(290, 97)
        Me.lstVars.Name = "lstVars"
        Me.lstVars.Size = New System.Drawing.Size(267, 312)
        Me.lstVars.Sorted = True
        Me.lstVars.TabIndex = 28
        '
        'lblRCVar2
        '
        Me.lblRCVar2.ContextMenuStrip = Me.mnuChkRC
        Me.lblRCVar2.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.lblRCVar2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblRCVar2.Location = New System.Drawing.Point(668, 74)
        Me.lblRCVar2.Name = "lblRCVar2"
        Me.lblRCVar2.Size = New System.Drawing.Size(153, 19)
        Me.lblRCVar2.TabIndex = 27
        Me.lblRCVar2.Tag = "11"
        Me.lblRCVar2.Text = "lblRCVar2"
        Me.lblRCVar2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblRCVar1
        '
        Me.lblRCVar1.ContextMenuStrip = Me.mnuChkRC
        Me.lblRCVar1.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.lblRCVar1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblRCVar1.Location = New System.Drawing.Point(667, 40)
        Me.lblRCVar1.Name = "lblRCVar1"
        Me.lblRCVar1.Size = New System.Drawing.Size(153, 19)
        Me.lblRCVar1.TabIndex = 25
        Me.lblRCVar1.Tag = "11"
        Me.lblRCVar1.Text = "lblRCVar1"
        Me.lblRCVar1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblVariableValueCap
        '
        Me.lblVariableValueCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblVariableValueCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblVariableValueCap.Location = New System.Drawing.Point(667, 14)
        Me.lblVariableValueCap.Name = "lblVariableValueCap"
        Me.lblVariableValueCap.Size = New System.Drawing.Size(204, 19)
        Me.lblVariableValueCap.TabIndex = 23
        Me.lblVariableValueCap.Tag = "11"
        Me.lblVariableValueCap.Text = "lblVariableValueCap"
        Me.lblVariableValueCap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
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
        Me.lblZone.Location = New System.Drawing.Point(77, 8)
        Me.lblZone.Name = "lblZone"
        Me.lblZone.Size = New System.Drawing.Size(47, 19)
        Me.lblZone.TabIndex = 12
        Me.lblZone.Text = "Zone"
        Me.lblZone.UseMnemonic = False
        '
        'cboZone
        '
        Me.cboZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboZone.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboZone.ItemHeight = 18
        Me.cboZone.Location = New System.Drawing.Point(20, 33)
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
        Me.btnSave.Enabled = False
        Me.btnSave.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnSave.Image = CType(resources.GetObject("btnSave.Image"), System.Drawing.Image)
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
        Me.tscMain.ContentPanel.PerformLayout()
        Me.tscMain.TopToolStripPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.PerformLayout()
        Me.tscMain.ResumeLayout(False)
        Me.tscMain.PerformLayout()
        Me.stsStatus.ResumeLayout(False)
        Me.stsStatus.PerformLayout()
        Me.pnlMain.ResumeLayout(False)
        Me.pnlMain.PerformLayout()
        Me.mnuChkRC.ResumeLayout(False)
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
    Friend WithEvents tmrRefresh As System.Windows.Forms.Timer
    Friend WithEvents chkRC_1 As System.Windows.Forms.CheckBox
    Friend WithEvents lstVars As System.Windows.Forms.ListBox
    Friend WithEvents lblRCVar2 As System.Windows.Forms.Label
    Friend WithEvents lblRCVar1 As System.Windows.Forms.Label
    Friend WithEvents lblVariableValueCap As System.Windows.Forms.Label
    Friend WithEvents lblRobot As System.Windows.Forms.Label
    Friend WithEvents cboRobot As System.Windows.Forms.ComboBox
    Friend WithEvents chkRC_2 As System.Windows.Forms.CheckBox
    Friend WithEvents lblSelectVariableCap As System.Windows.Forms.Label
    Friend WithEvents txtVarName As System.Windows.Forms.TextBox
    Friend WithEvents rdoBrowse As System.Windows.Forms.RadioButton
    Friend WithEvents rdoList As System.Windows.Forms.RadioButton
    Friend WithEvents btnAdd As System.Windows.Forms.Button
    Friend WithEvents trvBrowse As System.Windows.Forms.TreeView
    Friend WithEvents btnSave As System.Windows.Forms.ToolStripButton
    Friend WithEvents mnuChkRC As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuSelectAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuUnselectAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuBrowse As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuCopyToAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuCopy As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents txtVarValue As System.Windows.Forms.TextBox
    Friend WithEvents btnWriteRC As System.Windows.Forms.Button
    Friend WithEvents lblVarType As System.Windows.Forms.Label
    Friend WithEvents lblVarTypeCap As System.Windows.Forms.Label
    Friend WithEvents lblVarName As System.Windows.Forms.Label
    Friend WithEvents lblVarNameCap As System.Windows.Forms.Label
    Friend WithEvents lblStorage As System.Windows.Forms.Label
    Friend WithEvents lblStorageCap As System.Windows.Forms.Label
    Friend WithEvents lblStrLen As System.Windows.Forms.Label
    Friend WithEvents lblStrLenCap As System.Windows.Forms.Label
    Friend WithEvents lblMax As System.Windows.Forms.Label
    Friend WithEvents lblMaxCap As System.Windows.Forms.Label
    Friend WithEvents lblMin As System.Windows.Forms.Label
    Friend WithEvents lblMinCap As System.Windows.Forms.Label
    Friend WithEvents btnDescEdit As System.Windows.Forms.Button
    Friend WithEvents lblDescriptionCap As System.Windows.Forms.Label
    Friend WithEvents txtDescription As System.Windows.Forms.TextBox
    Friend WithEvents lblProgName As System.Windows.Forms.Label
    Friend WithEvents lblProgNameCap As System.Windows.Forms.Label
    Friend WithEvents lblAccess As System.Windows.Forms.Label
    Friend WithEvents lblAccessCap As System.Windows.Forms.Label
    Friend WithEvents btnRemove As System.Windows.Forms.Button
End Class
