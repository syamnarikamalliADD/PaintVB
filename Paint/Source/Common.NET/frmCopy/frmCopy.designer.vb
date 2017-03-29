<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCopy
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCopy))
        Me.tscMain = New System.Windows.Forms.ToolStripContainer
        Me.stsStatus = New System.Windows.Forms.StatusStrip
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel
        Me.tspProgress = New System.Windows.Forms.ToolStripProgressBar
        Me.lblSpacer = New System.Windows.Forms.ToolStripStatusLabel
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.pnlMain = New System.Windows.Forms.Panel
        Me.gpbTo = New System.Windows.Forms.GroupBox
        Me.lblViewStyleTo = New System.Windows.Forms.Label
        Me.clbStyleTo = New System.Windows.Forms.CheckedListBox
        Me.lblstyleTo = New System.Windows.Forms.Label
        Me.clbZoneTo = New System.Windows.Forms.CheckedListBox
        Me.lblViewSubParamTo = New System.Windows.Forms.Label
        Me.lblViewParamTo = New System.Windows.Forms.Label
        Me.lblViewDevTo = New System.Windows.Forms.Label
        Me.lblViewTo = New System.Windows.Forms.Label
        Me.lblSubParamTo = New System.Windows.Forms.Label
        Me.clbSubParamTo = New System.Windows.Forms.CheckedListBox
        Me.clbParamTo = New System.Windows.Forms.CheckedListBox
        Me.lblParamTo = New System.Windows.Forms.Label
        Me.LblRobotTo = New System.Windows.Forms.Label
        Me.clbRobotTo = New System.Windows.Forms.CheckedListBox
        Me.lblZoneTo = New System.Windows.Forms.Label
        Me.gpbFrom = New System.Windows.Forms.GroupBox
        Me.lblViewStyleFrom = New System.Windows.Forms.Label
        Me.clbStyleFrom = New System.Windows.Forms.CheckedListBox
        Me.lblStyle = New System.Windows.Forms.Label
        Me.lblViewSubParamFrom = New System.Windows.Forms.Label
        Me.lblViewParamFrom = New System.Windows.Forms.Label
        Me.lblViewDevFrom = New System.Windows.Forms.Label
        Me.lblViewFrom = New System.Windows.Forms.Label
        Me.lblSubParam = New System.Windows.Forms.Label
        Me.clbSubParamFrom = New System.Windows.Forms.CheckedListBox
        Me.clbParamFrom = New System.Windows.Forms.CheckedListBox
        Me.lblParam = New System.Windows.Forms.Label
        Me.lblRobot = New System.Windows.Forms.Label
        Me.clbRobotFrom = New System.Windows.Forms.CheckedListBox
        Me.lblZone = New System.Windows.Forms.Label
        Me.cboZoneFrom = New System.Windows.Forms.ComboBox
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.btnCopy = New System.Windows.Forms.ToolStripButton
        Me.btnCopyV = New System.Windows.Forms.ToolStripSplitButton
        Me.btnCopyD = New System.Windows.Forms.ToolStripButton
        Me.btnStatus = New System.Windows.Forms.ToolStripButton
        Me.mnuSelect = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuNotAll = New System.Windows.Forms.ToolStripMenuItem
        Me.tscMain.BottomToolStripPanel.SuspendLayout()
        Me.tscMain.ContentPanel.SuspendLayout()
        Me.tscMain.TopToolStripPanel.SuspendLayout()
        Me.tscMain.SuspendLayout()
        Me.stsStatus.SuspendLayout()
        Me.pnlMain.SuspendLayout()
        Me.gpbTo.SuspendLayout()
        Me.gpbFrom.SuspendLayout()
        Me.tlsMain.SuspendLayout()
        Me.mnuSelect.SuspendLayout()
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
        Me.tscMain.ContentPanel.Controls.Add(Me.lstStatus)
        Me.tscMain.ContentPanel.Controls.Add(Me.pnlMain)
        resources.ApplyResources(Me.tscMain.ContentPanel, "tscMain.ContentPanel")
        resources.ApplyResources(Me.tscMain, "tscMain")
        Me.tscMain.Name = "tscMain"
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
        Me.stsStatus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatus, Me.tspProgress, Me.lblSpacer})
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
        'lstStatus
        '
        Me.lstStatus.BackColor = System.Drawing.SystemColors.Info
        resources.ApplyResources(Me.lstStatus, "lstStatus")
        Me.lstStatus.Name = "lstStatus"
        '
        'pnlMain
        '
        Me.pnlMain.Controls.Add(Me.gpbTo)
        Me.pnlMain.Controls.Add(Me.gpbFrom)
        resources.ApplyResources(Me.pnlMain, "pnlMain")
        Me.pnlMain.Name = "pnlMain"
        '
        'gpbTo
        '
        Me.gpbTo.Controls.Add(Me.lblViewStyleTo)
        Me.gpbTo.Controls.Add(Me.clbStyleTo)
        Me.gpbTo.Controls.Add(Me.lblstyleTo)
        Me.gpbTo.Controls.Add(Me.clbZoneTo)
        Me.gpbTo.Controls.Add(Me.lblViewSubParamTo)
        Me.gpbTo.Controls.Add(Me.lblViewParamTo)
        Me.gpbTo.Controls.Add(Me.lblViewDevTo)
        Me.gpbTo.Controls.Add(Me.lblViewTo)
        Me.gpbTo.Controls.Add(Me.lblSubParamTo)
        Me.gpbTo.Controls.Add(Me.clbSubParamTo)
        Me.gpbTo.Controls.Add(Me.clbParamTo)
        Me.gpbTo.Controls.Add(Me.lblParamTo)
        Me.gpbTo.Controls.Add(Me.LblRobotTo)
        Me.gpbTo.Controls.Add(Me.clbRobotTo)
        Me.gpbTo.Controls.Add(Me.lblZoneTo)
        Me.gpbTo.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.gpbTo, "gpbTo")
        Me.gpbTo.Name = "gpbTo"
        Me.gpbTo.TabStop = False
        '
        'lblViewStyleTo
        '
        resources.ApplyResources(Me.lblViewStyleTo, "lblViewStyleTo")
        Me.lblViewStyleTo.Name = "lblViewStyleTo"
        '
        'clbStyleTo
        '
        Me.clbStyleTo.CheckOnClick = True
        resources.ApplyResources(Me.clbStyleTo, "clbStyleTo")
        Me.clbStyleTo.FormattingEnabled = True
        Me.clbStyleTo.Name = "clbStyleTo"
        '
        'lblstyleTo
        '
        Me.lblstyleTo.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblstyleTo, "lblstyleTo")
        Me.lblstyleTo.Name = "lblstyleTo"
        Me.lblstyleTo.UseMnemonic = False
        '
        'clbZoneTo
        '
        Me.clbZoneTo.CheckOnClick = True
        resources.ApplyResources(Me.clbZoneTo, "clbZoneTo")
        Me.clbZoneTo.FormattingEnabled = True
        Me.clbZoneTo.Name = "clbZoneTo"
        '
        'lblViewSubParamTo
        '
        resources.ApplyResources(Me.lblViewSubParamTo, "lblViewSubParamTo")
        Me.lblViewSubParamTo.Name = "lblViewSubParamTo"
        '
        'lblViewParamTo
        '
        resources.ApplyResources(Me.lblViewParamTo, "lblViewParamTo")
        Me.lblViewParamTo.Name = "lblViewParamTo"
        '
        'lblViewDevTo
        '
        resources.ApplyResources(Me.lblViewDevTo, "lblViewDevTo")
        Me.lblViewDevTo.Name = "lblViewDevTo"
        '
        'lblViewTo
        '
        resources.ApplyResources(Me.lblViewTo, "lblViewTo")
        Me.lblViewTo.Name = "lblViewTo"
        '
        'lblSubParamTo
        '
        Me.lblSubParamTo.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblSubParamTo, "lblSubParamTo")
        Me.lblSubParamTo.Name = "lblSubParamTo"
        Me.lblSubParamTo.UseMnemonic = False
        '
        'clbSubParamTo
        '
        Me.clbSubParamTo.CheckOnClick = True
        resources.ApplyResources(Me.clbSubParamTo, "clbSubParamTo")
        Me.clbSubParamTo.FormattingEnabled = True
        Me.clbSubParamTo.Name = "clbSubParamTo"
        '
        'clbParamTo
        '
        Me.clbParamTo.CheckOnClick = True
        resources.ApplyResources(Me.clbParamTo, "clbParamTo")
        Me.clbParamTo.FormattingEnabled = True
        Me.clbParamTo.Name = "clbParamTo"
        '
        'lblParamTo
        '
        Me.lblParamTo.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblParamTo, "lblParamTo")
        Me.lblParamTo.Name = "lblParamTo"
        Me.lblParamTo.UseMnemonic = False
        '
        'LblRobotTo
        '
        Me.LblRobotTo.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.LblRobotTo, "LblRobotTo")
        Me.LblRobotTo.Name = "LblRobotTo"
        Me.LblRobotTo.UseMnemonic = False
        '
        'clbRobotTo
        '
        Me.clbRobotTo.CheckOnClick = True
        resources.ApplyResources(Me.clbRobotTo, "clbRobotTo")
        Me.clbRobotTo.FormattingEnabled = True
        Me.clbRobotTo.Name = "clbRobotTo"
        '
        'lblZoneTo
        '
        Me.lblZoneTo.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblZoneTo, "lblZoneTo")
        Me.lblZoneTo.Name = "lblZoneTo"
        Me.lblZoneTo.UseMnemonic = False
        '
        'gpbFrom
        '
        Me.gpbFrom.Controls.Add(Me.lblViewStyleFrom)
        Me.gpbFrom.Controls.Add(Me.clbStyleFrom)
        Me.gpbFrom.Controls.Add(Me.lblStyle)
        Me.gpbFrom.Controls.Add(Me.lblViewSubParamFrom)
        Me.gpbFrom.Controls.Add(Me.lblViewParamFrom)
        Me.gpbFrom.Controls.Add(Me.lblViewDevFrom)
        Me.gpbFrom.Controls.Add(Me.lblViewFrom)
        Me.gpbFrom.Controls.Add(Me.lblSubParam)
        Me.gpbFrom.Controls.Add(Me.clbSubParamFrom)
        Me.gpbFrom.Controls.Add(Me.clbParamFrom)
        Me.gpbFrom.Controls.Add(Me.lblParam)
        Me.gpbFrom.Controls.Add(Me.lblRobot)
        Me.gpbFrom.Controls.Add(Me.clbRobotFrom)
        Me.gpbFrom.Controls.Add(Me.lblZone)
        Me.gpbFrom.Controls.Add(Me.cboZoneFrom)
        Me.gpbFrom.FlatStyle = System.Windows.Forms.FlatStyle.System
        resources.ApplyResources(Me.gpbFrom, "gpbFrom")
        Me.gpbFrom.Name = "gpbFrom"
        Me.gpbFrom.TabStop = False
        '
        'lblViewStyleFrom
        '
        resources.ApplyResources(Me.lblViewStyleFrom, "lblViewStyleFrom")
        Me.lblViewStyleFrom.Name = "lblViewStyleFrom"
        '
        'clbStyleFrom
        '
        Me.clbStyleFrom.CheckOnClick = True
        resources.ApplyResources(Me.clbStyleFrom, "clbStyleFrom")
        Me.clbStyleFrom.FormattingEnabled = True
        Me.clbStyleFrom.Name = "clbStyleFrom"
        '
        'lblStyle
        '
        Me.lblStyle.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblStyle, "lblStyle")
        Me.lblStyle.Name = "lblStyle"
        Me.lblStyle.UseMnemonic = False
        '
        'lblViewSubParamFrom
        '
        resources.ApplyResources(Me.lblViewSubParamFrom, "lblViewSubParamFrom")
        Me.lblViewSubParamFrom.Name = "lblViewSubParamFrom"
        '
        'lblViewParamFrom
        '
        resources.ApplyResources(Me.lblViewParamFrom, "lblViewParamFrom")
        Me.lblViewParamFrom.Name = "lblViewParamFrom"
        '
        'lblViewDevFrom
        '
        resources.ApplyResources(Me.lblViewDevFrom, "lblViewDevFrom")
        Me.lblViewDevFrom.Name = "lblViewDevFrom"
        '
        'lblViewFrom
        '
        resources.ApplyResources(Me.lblViewFrom, "lblViewFrom")
        Me.lblViewFrom.Name = "lblViewFrom"
        '
        'lblSubParam
        '
        Me.lblSubParam.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblSubParam, "lblSubParam")
        Me.lblSubParam.Name = "lblSubParam"
        Me.lblSubParam.UseMnemonic = False
        '
        'clbSubParamFrom
        '
        Me.clbSubParamFrom.CheckOnClick = True
        resources.ApplyResources(Me.clbSubParamFrom, "clbSubParamFrom")
        Me.clbSubParamFrom.FormattingEnabled = True
        Me.clbSubParamFrom.Name = "clbSubParamFrom"
        '
        'clbParamFrom
        '
        Me.clbParamFrom.CheckOnClick = True
        resources.ApplyResources(Me.clbParamFrom, "clbParamFrom")
        Me.clbParamFrom.FormattingEnabled = True
        Me.clbParamFrom.Name = "clbParamFrom"
        '
        'lblParam
        '
        Me.lblParam.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblParam, "lblParam")
        Me.lblParam.Name = "lblParam"
        Me.lblParam.UseMnemonic = False
        '
        'lblRobot
        '
        Me.lblRobot.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblRobot, "lblRobot")
        Me.lblRobot.Name = "lblRobot"
        Me.lblRobot.UseMnemonic = False
        '
        'clbRobotFrom
        '
        Me.clbRobotFrom.CheckOnClick = True
        resources.ApplyResources(Me.clbRobotFrom, "clbRobotFrom")
        Me.clbRobotFrom.FormattingEnabled = True
        Me.clbRobotFrom.Name = "clbRobotFrom"
        '
        'lblZone
        '
        Me.lblZone.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblZone, "lblZone")
        Me.lblZone.Name = "lblZone"
        Me.lblZone.UseMnemonic = False
        '
        'cboZoneFrom
        '
        Me.cboZoneFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboZoneFrom, "cboZoneFrom")
        Me.cboZoneFrom.Name = "cboZoneFrom"
        '
        'tlsMain
        '
        resources.ApplyResources(Me.tlsMain, "tlsMain")
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.btnCopy, Me.btnCopyV, Me.btnCopyD, Me.btnStatus})
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
        'btnCopy
        '
        resources.ApplyResources(Me.btnCopy, "btnCopy")
        Me.btnCopy.Name = "btnCopy"
        '
        'btnCopyV
        '
        resources.ApplyResources(Me.btnCopyV, "btnCopyV")
        Me.btnCopyV.Name = "btnCopyV"
        '
        'btnCopyD
        '
        resources.ApplyResources(Me.btnCopyD, "btnCopyD")
        Me.btnCopyD.Name = "btnCopyD"
        '
        'btnStatus
        '
        resources.ApplyResources(Me.btnStatus, "btnStatus")
        Me.btnStatus.Name = "btnStatus"
        '
        'mnuSelect
        '
        resources.ApplyResources(Me.mnuSelect, "mnuSelect")
        Me.mnuSelect.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuAll, Me.mnuNotAll})
        Me.mnuSelect.Name = "mnuSelect"
        '
        'mnuAll
        '
        Me.mnuAll.Name = "mnuAll"
        resources.ApplyResources(Me.mnuAll, "mnuAll")
        '
        'mnuNotAll
        '
        Me.mnuNotAll.Name = "mnuNotAll"
        resources.ApplyResources(Me.mnuNotAll, "mnuNotAll")
        '
        'frmCopy
        '
        resources.ApplyResources(Me, "$this")
        Me.ControlBox = False
        Me.Controls.Add(Me.tscMain)
        Me.KeyPreview = True
        Me.Name = "frmCopy"
        Me.TopMost = True
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
        Me.gpbTo.ResumeLayout(False)
        Me.gpbTo.PerformLayout()
        Me.gpbFrom.ResumeLayout(False)
        Me.gpbFrom.PerformLayout()
        Me.tlsMain.ResumeLayout(False)
        Me.tlsMain.PerformLayout()
        Me.mnuSelect.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnClose As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnCopy As System.Windows.Forms.ToolStripButton
    Friend WithEvents tscMain As System.Windows.Forms.ToolStripContainer
    Friend WithEvents stsStatus As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tspProgress As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents lblSpacer As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents pnlMain As System.Windows.Forms.Panel
    Friend WithEvents gpbTo As System.Windows.Forms.GroupBox
    Friend WithEvents lblSubParamTo As System.Windows.Forms.Label
    Friend WithEvents clbSubParamTo As System.Windows.Forms.CheckedListBox
    Friend WithEvents clbParamTo As System.Windows.Forms.CheckedListBox
    Friend WithEvents lblParamTo As System.Windows.Forms.Label
    Friend WithEvents LblRobotTo As System.Windows.Forms.Label
    Friend WithEvents clbRobotTo As System.Windows.Forms.CheckedListBox
    Friend WithEvents lblZoneTo As System.Windows.Forms.Label
    Friend WithEvents gpbFrom As System.Windows.Forms.GroupBox
    Friend WithEvents lblSubParam As System.Windows.Forms.Label
    Friend WithEvents clbSubParamFrom As System.Windows.Forms.CheckedListBox
    Friend WithEvents clbParamFrom As System.Windows.Forms.CheckedListBox
    Friend WithEvents lblParam As System.Windows.Forms.Label
    Friend WithEvents lblRobot As System.Windows.Forms.Label
    Friend WithEvents clbRobotFrom As System.Windows.Forms.CheckedListBox
    Friend WithEvents lblZone As System.Windows.Forms.Label
    Friend WithEvents cboZoneFrom As System.Windows.Forms.ComboBox
    Friend WithEvents lstStatus As System.Windows.Forms.ListBox
    Friend WithEvents btnCopyD As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnStatus As System.Windows.Forms.ToolStripButton
    Friend WithEvents mnuSelect As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuNotAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnCopyV As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents lblViewTo As System.Windows.Forms.Label
    Friend WithEvents lblViewFrom As System.Windows.Forms.Label
    Friend WithEvents lblViewSubParamTo As System.Windows.Forms.Label
    Friend WithEvents lblViewParamTo As System.Windows.Forms.Label
    Friend WithEvents lblViewDevTo As System.Windows.Forms.Label
    Friend WithEvents lblViewSubParamFrom As System.Windows.Forms.Label
    Friend WithEvents lblViewParamFrom As System.Windows.Forms.Label
    Friend WithEvents lblViewDevFrom As System.Windows.Forms.Label
    Friend WithEvents clbZoneTo As System.Windows.Forms.CheckedListBox
    Friend WithEvents clbStyleTo As System.Windows.Forms.CheckedListBox
    Friend WithEvents lblstyleTo As System.Windows.Forms.Label
    Friend WithEvents clbStyleFrom As System.Windows.Forms.CheckedListBox
    Friend WithEvents lblStyle As System.Windows.Forms.Label
    Friend WithEvents lblViewStyleTo As System.Windows.Forms.Label
    Friend WithEvents lblViewStyleFrom As System.Windows.Forms.Label
End Class
