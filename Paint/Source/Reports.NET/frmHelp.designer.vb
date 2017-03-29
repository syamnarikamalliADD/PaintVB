<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmHelp
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmHelp))
        Me.wbHelp = New System.Windows.Forms.WebBrowser
        Me.btnClose = New System.Windows.Forms.Button
        Me.tlsHosts = New System.Windows.Forms.ToolStrip
        Me.tlspSaveHosts = New System.Windows.Forms.ToolStripButton
        Me.tlspPrintHelp = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuPrintHelp = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPreviewHelp = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPageSetupHelp = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintFileHelp = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator
        Me.btnSingleAlarm = New System.Windows.Forms.ToolStripButton
        Me.btnFullFile = New System.Windows.Forms.ToolStripButton
        Me.tlsHosts.SuspendLayout()
        Me.SuspendLayout()
        '
        'wbHelp
        '
        Me.wbHelp.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.wbHelp.Location = New System.Drawing.Point(12, 28)
        Me.wbHelp.MinimumSize = New System.Drawing.Size(20, 20)
        Me.wbHelp.Name = "wbHelp"
        Me.wbHelp.ScriptErrorsSuppressed = True
        Me.wbHelp.Size = New System.Drawing.Size(712, 155)
        Me.wbHelp.TabIndex = 15
        '
        'btnClose
        '
        Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnClose.Location = New System.Drawing.Point(422, 149)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(75, 12)
        Me.btnClose.TabIndex = 16
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'tlsHosts
        '
        Me.tlsHosts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tlspSaveHosts, Me.tlspPrintHelp, Me.ToolStripSeparator7, Me.btnSingleAlarm, Me.btnFullFile})
        Me.tlsHosts.Location = New System.Drawing.Point(0, 0)
        Me.tlsHosts.Name = "tlsHosts"
        Me.tlsHosts.Size = New System.Drawing.Size(736, 25)
        Me.tlsHosts.TabIndex = 17
        '
        'tlspSaveHosts
        '
        Me.tlspSaveHosts.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tlspSaveHosts.Image = CType(resources.GetObject("tlspSaveHosts.Image"), System.Drawing.Image)
        Me.tlspSaveHosts.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tlspSaveHosts.Name = "tlspSaveHosts"
        Me.tlspSaveHosts.Size = New System.Drawing.Size(23, 22)
        Me.tlspSaveHosts.Text = "&Save"
        '
        'tlspPrintHelp
        '
        Me.tlspPrintHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tlspPrintHelp.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuPrintHelp, Me.mnuPreviewHelp, Me.mnuPageSetupHelp, Me.mnuPrintFileHelp})
        Me.tlspPrintHelp.Image = CType(resources.GetObject("tlspPrintHelp.Image"), System.Drawing.Image)
        Me.tlspPrintHelp.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tlspPrintHelp.Name = "tlspPrintHelp"
        Me.tlspPrintHelp.Size = New System.Drawing.Size(32, 22)
        Me.tlspPrintHelp.Text = "&Print"
        '
        'mnuPrintHelp
        '
        Me.mnuPrintHelp.Name = "mnuPrintHelp"
        Me.mnuPrintHelp.Size = New System.Drawing.Size(167, 22)
        Me.mnuPrintHelp.Text = "mnuPrintHelp"
        '
        'mnuPreviewHelp
        '
        Me.mnuPreviewHelp.Name = "mnuPreviewHelp"
        Me.mnuPreviewHelp.Size = New System.Drawing.Size(167, 22)
        Me.mnuPreviewHelp.Text = "mnuPreviewHelp"
        '
        'mnuPageSetupHelp
        '
        Me.mnuPageSetupHelp.Name = "mnuPageSetupHelp"
        Me.mnuPageSetupHelp.Size = New System.Drawing.Size(167, 22)
        Me.mnuPageSetupHelp.Text = "mnuPageSetupHelp"
        '
        'mnuPrintFileHelp
        '
        Me.mnuPrintFileHelp.Name = "mnuPrintFileHelp"
        Me.mnuPrintFileHelp.Size = New System.Drawing.Size(167, 22)
        Me.mnuPrintFileHelp.Text = "mnuPrintFileHelp"
        '
        'ToolStripSeparator7
        '
        Me.ToolStripSeparator7.Name = "ToolStripSeparator7"
        Me.ToolStripSeparator7.Size = New System.Drawing.Size(6, 25)
        '
        'btnSingleAlarm
        '
        Me.btnSingleAlarm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnSingleAlarm.Image = CType(resources.GetObject("btnSingleAlarm.Image"), System.Drawing.Image)
        Me.btnSingleAlarm.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnSingleAlarm.Name = "btnSingleAlarm"
        Me.btnSingleAlarm.Size = New System.Drawing.Size(82, 22)
        Me.btnSingleAlarm.Text = "btnSingleAlarm"
        '
        'btnFullFile
        '
        Me.btnFullFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnFullFile.Image = CType(resources.GetObject("btnFullFile.Image"), System.Drawing.Image)
        Me.btnFullFile.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnFullFile.Name = "btnFullFile"
        Me.btnFullFile.Size = New System.Drawing.Size(59, 22)
        Me.btnFullFile.Text = "btnFullFile"
        '
        'frmHelp
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnClose
        Me.ClientSize = New System.Drawing.Size(736, 195)
        Me.Controls.Add(Me.tlsHosts)
        Me.Controls.Add(Me.wbHelp)
        Me.Controls.Add(Me.btnClose)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(744, 222)
        Me.Name = "frmHelp"
        Me.Text = "frmHelp"
        Me.TopMost = True
        Me.tlsHosts.ResumeLayout(False)
        Me.tlsHosts.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents wbHelp As System.Windows.Forms.WebBrowser
    Friend WithEvents btnClose As System.Windows.Forms.Button

    Private Sub frmHelp_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.MouseHover

    End Sub
    Friend WithEvents tlsHosts As System.Windows.Forms.ToolStrip
    Friend WithEvents tlspSaveHosts As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlspPrintHelp As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuPrintHelp As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPreviewHelp As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPageSetupHelp As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintFileHelp As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents btnSingleAlarm As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnFullFile As System.Windows.Forms.ToolStripButton
End Class
