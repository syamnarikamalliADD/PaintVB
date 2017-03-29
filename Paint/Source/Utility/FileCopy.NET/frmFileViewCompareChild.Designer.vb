<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmFileViewCompareChild
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
        Dim ListViewItem1 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem(New String() {"ItemBlue"}, -1, System.Drawing.Color.RoyalBlue, System.Drawing.Color.Empty, Nothing)
        Dim ListViewItem2 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem(New String() {"ItemRed"}, -1, System.Drawing.Color.Red, System.Drawing.Color.Empty, Nothing)
        Me.mRtbText = New System.Windows.Forms.RichTextBox
        Me.stsStatus = New System.Windows.Forms.StatusStrip
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel
        Me.mlstvwFolder = New System.Windows.Forms.ListView
        Me.stsStatus.SuspendLayout()
        Me.SuspendLayout()
        '
        'mRtbText
        '
        Me.mRtbText.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.mRtbText.Location = New System.Drawing.Point(0, 0)
        Me.mRtbText.Name = "mRtbText"
        Me.mRtbText.ReadOnly = True
        Me.mRtbText.Size = New System.Drawing.Size(218, 247)
        Me.mRtbText.TabIndex = 0
        Me.mRtbText.Text = "Abort"
        '
        'stsStatus
        '
        Me.stsStatus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatus})
        Me.stsStatus.Location = New System.Drawing.Point(0, 336)
        Me.stsStatus.Name = "stsStatus"
        Me.stsStatus.Size = New System.Drawing.Size(417, 22)
        Me.stsStatus.TabIndex = 8
        Me.stsStatus.Text = "StatusStrip"
        '
        'lblStatus
        '
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(0, 17)
        '
        'mlstvwFolder
        '
        Me.mlstvwFolder.Items.AddRange(New System.Windows.Forms.ListViewItem() {ListViewItem1, ListViewItem2})
        Me.mlstvwFolder.Location = New System.Drawing.Point(230, 72)
        Me.mlstvwFolder.MultiSelect = False
        Me.mlstvwFolder.Name = "mlstvwFolder"
        Me.mlstvwFolder.Size = New System.Drawing.Size(187, 209)
        Me.mlstvwFolder.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.mlstvwFolder.TabIndex = 10
        Me.mlstvwFolder.UseCompatibleStateImageBehavior = False
        Me.mlstvwFolder.View = System.Windows.Forms.View.List
        '
        'frmFileViewCompareChild
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(417, 358)
        Me.Controls.Add(Me.mlstvwFolder)
        Me.Controls.Add(Me.stsStatus)
        Me.Controls.Add(Me.mRtbText)
        Me.Name = "frmFileViewCompareChild"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "frmFileViewCompareChild"
        Me.stsStatus.ResumeLayout(False)
        Me.stsStatus.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents mRtbText As System.Windows.Forms.RichTextBox
    Friend WithEvents stsStatus As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents mlstvwFolder As System.Windows.Forms.ListView
End Class
