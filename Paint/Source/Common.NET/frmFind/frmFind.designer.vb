<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmFind
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
        Me.tlpButtons = New System.Windows.Forms.TableLayoutPanel
        Me.btnPrev = New System.Windows.Forms.Button
        Me.btnNext = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.lblFind = New System.Windows.Forms.Label
        Me.txtSearchText = New System.Windows.Forms.TextBox
        Me.tlpButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpButtons
        '
        Me.tlpButtons.ColumnCount = 3
        Me.tlpButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.tlpButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.tlpButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.tlpButtons.Controls.Add(Me.btnPrev, 0, 0)
        Me.tlpButtons.Controls.Add(Me.btnNext, 1, 0)
        Me.tlpButtons.Controls.Add(Me.btnCancel, 2, 0)
        Me.tlpButtons.Location = New System.Drawing.Point(154, 44)
        Me.tlpButtons.Name = "tlpButtons"
        Me.tlpButtons.RowCount = 1
        Me.tlpButtons.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpButtons.Size = New System.Drawing.Size(268, 29)
        Me.tlpButtons.TabIndex = 0
        '
        'btnPrev
        '
        Me.btnPrev.AutoSize = True
        Me.btnPrev.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnPrev.Location = New System.Drawing.Point(3, 3)
        Me.btnPrev.Name = "btnPrev"
        Me.btnPrev.Size = New System.Drawing.Size(83, 23)
        Me.btnPrev.TabIndex = 2
        Me.btnPrev.Text = "<< Previous"
        '
        'btnNext
        '
        Me.btnNext.AutoSize = True
        Me.btnNext.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnNext.Location = New System.Drawing.Point(92, 3)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(83, 23)
        Me.btnNext.TabIndex = 0
        Me.btnNext.Text = "btnNext"
        '
        'btnCancel
        '
        Me.btnCancel.AutoSize = True
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnCancel.Location = New System.Drawing.Point(181, 3)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(84, 23)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "Cancel"
        '
        'lblFind
        '
        Me.lblFind.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblFind.Location = New System.Drawing.Point(14, 15)
        Me.lblFind.Name = "lblFind"
        Me.lblFind.Size = New System.Drawing.Size(124, 18)
        Me.lblFind.TabIndex = 1
        Me.lblFind.Text = "lblFind"
        Me.lblFind.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtSearchText
        '
        Me.txtSearchText.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSearchText.Location = New System.Drawing.Point(139, 12)
        Me.txtSearchText.Name = "txtSearchText"
        Me.txtSearchText.Size = New System.Drawing.Size(283, 26)
        Me.txtSearchText.TabIndex = 2
        '
        'frmFind
        '
        Me.AcceptButton = Me.btnNext
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(430, 77)
        Me.ControlBox = False
        Me.Controls.Add(Me.txtSearchText)
        Me.Controls.Add(Me.lblFind)
        Me.Controls.Add(Me.tlpButtons)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmFind"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "frmFind"
        Me.tlpButtons.ResumeLayout(False)
        Me.tlpButtons.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents tlpButtons As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnNext As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents lblFind As System.Windows.Forms.Label
    Friend WithEvents txtSearchText As System.Windows.Forms.TextBox
    Friend WithEvents btnPrev As System.Windows.Forms.Button

End Class
