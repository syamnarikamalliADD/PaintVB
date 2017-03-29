<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.lblTitle = New System.Windows.Forms.Label
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.fraDate = New System.Windows.Forms.GroupBox
        Me.lblDate = New System.Windows.Forms.Label
        Me.fraTime = New System.Windows.Forms.GroupBox
        Me.lblTime = New System.Windows.Forms.Label
        Me.btnViewLog = New System.Windows.Forms.Button
        Me.btnPrintLog = New System.Windows.Forms.Button
        Me.btnRestartPW = New System.Windows.Forms.Button
        Me.stsStatus = New System.Windows.Forms.StatusStrip
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel
        Me.tspProgress = New System.Windows.Forms.ToolStripProgressBar
        Me.lblSpacer = New System.Windows.Forms.ToolStripStatusLabel
        Me.btnFunction = New System.Windows.Forms.ToolStripDropDownButton
        Me.mnuLogin = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLogOut = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuRemote = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLocal = New System.Windows.Forms.ToolStripMenuItem
        Me.tmrEvent = New System.Windows.Forms.Timer(Me.components)
        Me.tmrClose = New System.Windows.Forms.Timer(Me.components)
        Me.rtbPrintLog = New System.Windows.Forms.RichTextBox
        Me.fraDate.SuspendLayout()
        Me.fraTime.SuspendLayout()
        Me.stsStatus.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.lblTitle.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(292, 75)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(441, 22)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "Scheduled Maintenance Status"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lstStatus
        '
        Me.lstStatus.BackColor = System.Drawing.SystemColors.Info
        Me.lstStatus.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstStatus.FormattingEnabled = True
        Me.lstStatus.HorizontalScrollbar = True
        Me.lstStatus.ItemHeight = 16
        Me.lstStatus.Location = New System.Drawing.Point(133, 126)
        Me.lstStatus.Name = "lstStatus"
        Me.lstStatus.Size = New System.Drawing.Size(758, 404)
        Me.lstStatus.TabIndex = 1
        '
        'fraDate
        '
        Me.fraDate.Controls.Add(Me.lblDate)
        Me.fraDate.Location = New System.Drawing.Point(133, 545)
        Me.fraDate.Name = "fraDate"
        Me.fraDate.Size = New System.Drawing.Size(313, 54)
        Me.fraDate.TabIndex = 2
        Me.fraDate.TabStop = False
        '
        'lblDate
        '
        Me.lblDate.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDate.Location = New System.Drawing.Point(6, 16)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(301, 23)
        Me.lblDate.TabIndex = 0
        Me.lblDate.Text = "lblDate"
        Me.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'fraTime
        '
        Me.fraTime.Controls.Add(Me.lblTime)
        Me.fraTime.Location = New System.Drawing.Point(578, 545)
        Me.fraTime.Name = "fraTime"
        Me.fraTime.Size = New System.Drawing.Size(313, 54)
        Me.fraTime.TabIndex = 3
        Me.fraTime.TabStop = False
        '
        'lblTime
        '
        Me.lblTime.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTime.Location = New System.Drawing.Point(6, 16)
        Me.lblTime.Name = "lblTime"
        Me.lblTime.Size = New System.Drawing.Size(301, 23)
        Me.lblTime.TabIndex = 1
        Me.lblTime.Text = "lblTime"
        Me.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnViewLog
        '
        Me.btnViewLog.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnViewLog.Location = New System.Drawing.Point(133, 621)
        Me.btnViewLog.Name = "btnViewLog"
        Me.btnViewLog.Size = New System.Drawing.Size(165, 31)
        Me.btnViewLog.TabIndex = 4
        Me.btnViewLog.Text = "btnViewLog"
        Me.btnViewLog.UseVisualStyleBackColor = True
        Me.btnViewLog.Visible = False
        '
        'btnPrintLog
        '
        Me.btnPrintLog.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPrintLog.Location = New System.Drawing.Point(430, 621)
        Me.btnPrintLog.Name = "btnPrintLog"
        Me.btnPrintLog.Size = New System.Drawing.Size(165, 31)
        Me.btnPrintLog.TabIndex = 5
        Me.btnPrintLog.Text = "btnPrintLog"
        Me.btnPrintLog.UseVisualStyleBackColor = True
        Me.btnPrintLog.Visible = False
        '
        'btnRestartPW
        '
        Me.btnRestartPW.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRestartPW.Location = New System.Drawing.Point(726, 621)
        Me.btnRestartPW.Name = "btnRestartPW"
        Me.btnRestartPW.Size = New System.Drawing.Size(165, 31)
        Me.btnRestartPW.TabIndex = 6
        Me.btnRestartPW.Text = "btnRestartPW"
        Me.btnRestartPW.UseVisualStyleBackColor = True
        Me.btnRestartPW.Visible = False
        '
        'stsStatus
        '
        Me.stsStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.stsStatus.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.stsStatus.GripMargin = New System.Windows.Forms.Padding(0)
        Me.stsStatus.ImageScalingSize = New System.Drawing.Size(22, 22)
        Me.stsStatus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatus, Me.tspProgress, Me.lblSpacer, Me.btnFunction})
        Me.stsStatus.Location = New System.Drawing.Point(0, 737)
        Me.stsStatus.Name = "stsStatus"
        Me.stsStatus.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode
        Me.stsStatus.ShowItemToolTips = True
        Me.stsStatus.Size = New System.Drawing.Size(1018, 29)
        Me.stsStatus.TabIndex = 7
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
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tspProgress
        '
        Me.tspProgress.AutoSize = False
        Me.tspProgress.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.tspProgress.Margin = New System.Windows.Forms.Padding(2, 3, 1, 3)
        Me.tspProgress.Name = "tspProgress"
        Me.tspProgress.Padding = New System.Windows.Forms.Padding(2)
        Me.tspProgress.Size = New System.Drawing.Size(177, 23)
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
        Me.btnFunction.Visible = False
        '
        'mnuLogin
        '
        Me.mnuLogin.Name = "mnuLogin"
        Me.mnuLogin.Size = New System.Drawing.Size(201, 22)
        Me.mnuLogin.Text = "ToolStripMenuItem1"
        '
        'mnuLogOut
        '
        Me.mnuLogOut.Name = "mnuLogOut"
        Me.mnuLogOut.Size = New System.Drawing.Size(201, 22)
        Me.mnuLogOut.Text = "ToolStripMenuItem1"
        '
        'mnuRemote
        '
        Me.mnuRemote.Name = "mnuRemote"
        Me.mnuRemote.Size = New System.Drawing.Size(201, 22)
        Me.mnuRemote.Text = "ToolStripMenuItem1"
        '
        'mnuLocal
        '
        Me.mnuLocal.Name = "mnuLocal"
        Me.mnuLocal.Size = New System.Drawing.Size(201, 22)
        Me.mnuLocal.Text = "ToolStripMenuItem1"
        '
        'tmrEvent
        '
        Me.tmrEvent.Interval = 1000
        '
        'tmrClose
        '
        '
        'rtbPrintLog
        '
        Me.rtbPrintLog.Location = New System.Drawing.Point(722, 666)
        Me.rtbPrintLog.Name = "rtbPrintLog"
        Me.rtbPrintLog.Size = New System.Drawing.Size(168, 71)
        Me.rtbPrintLog.TabIndex = 8
        Me.rtbPrintLog.Text = "Abort"
        Me.rtbPrintLog.Visible = False
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1018, 766)
        Me.ControlBox = False
        Me.Controls.Add(Me.rtbPrintLog)
        Me.Controls.Add(Me.stsStatus)
        Me.Controls.Add(Me.btnRestartPW)
        Me.Controls.Add(Me.btnPrintLog)
        Me.Controls.Add(Me.btnViewLog)
        Me.Controls.Add(Me.fraTime)
        Me.Controls.Add(Me.fraDate)
        Me.Controls.Add(Me.lstStatus)
        Me.Controls.Add(Me.lblTitle)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.KeyPreview = True
        Me.Name = "frmMain"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.Text = "frmMain"
        Me.fraDate.ResumeLayout(False)
        Me.fraTime.ResumeLayout(False)
        Me.stsStatus.ResumeLayout(False)
        Me.stsStatus.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lstStatus As System.Windows.Forms.ListBox
    Friend WithEvents fraDate As System.Windows.Forms.GroupBox
    Friend WithEvents fraTime As System.Windows.Forms.GroupBox
    Friend WithEvents btnViewLog As System.Windows.Forms.Button
    Friend WithEvents btnPrintLog As System.Windows.Forms.Button
    Friend WithEvents btnRestartPW As System.Windows.Forms.Button
    Friend WithEvents stsStatus As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tspProgress As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents lblSpacer As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents btnFunction As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents mnuLogin As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLogOut As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuRemote As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLocal As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblTime As System.Windows.Forms.Label
    Friend WithEvents tmrEvent As System.Windows.Forms.Timer
    Friend WithEvents tmrClose As System.Windows.Forms.Timer
    Friend WithEvents rtbPrintLog As System.Windows.Forms.RichTextBox
End Class
