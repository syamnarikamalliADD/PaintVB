<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMultipleChoiceDlg
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMultipleChoiceDlg))
        Me.tlpButtons = New System.Windows.Forms.TableLayoutPanel
        Me.btn2 = New System.Windows.Forms.Button
        Me.btn3 = New System.Windows.Forms.Button
        Me.btn0 = New System.Windows.Forms.Button
        Me.btn1 = New System.Windows.Forms.Button
        Me.lblText = New System.Windows.Forms.Label
        Me.tlpButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpButtons
        '
        Me.tlpButtons.ColumnCount = 4
        Me.tlpButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.00062!))
        Me.tlpButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.00062!))
        Me.tlpButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.00062!))
        Me.tlpButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.99813!))
        Me.tlpButtons.Controls.Add(Me.btn2, 0, 0)
        Me.tlpButtons.Controls.Add(Me.btn3, 0, 0)
        Me.tlpButtons.Controls.Add(Me.btn0, 0, 0)
        Me.tlpButtons.Controls.Add(Me.btn1, 0, 0)
        Me.tlpButtons.Location = New System.Drawing.Point(18, 127)
        Me.tlpButtons.Name = "tlpButtons"
        Me.tlpButtons.RowCount = 1
        Me.tlpButtons.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpButtons.Size = New System.Drawing.Size(405, 29)
        Me.tlpButtons.TabIndex = 0
        '
        'btn2
        '
        Me.btn2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btn2.Location = New System.Drawing.Point(205, 3)
        Me.btn2.Name = "btn2"
        Me.btn2.Size = New System.Drawing.Size(95, 23)
        Me.btn2.TabIndex = 5
        Me.btn2.Text = "btn2"
        '
        'btn3
        '
        Me.btn3.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btn3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btn3.Location = New System.Drawing.Point(306, 3)
        Me.btn3.Name = "btn3"
        Me.btn3.Size = New System.Drawing.Size(96, 23)
        Me.btn3.TabIndex = 4
        Me.btn3.Text = "btn3"
        '
        'btn0
        '
        Me.btn0.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btn0.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btn0.Location = New System.Drawing.Point(3, 3)
        Me.btn0.Name = "btn0"
        Me.btn0.Size = New System.Drawing.Size(95, 23)
        Me.btn0.TabIndex = 3
        Me.btn0.Text = "btn0"
        '
        'btn1
        '
        Me.btn1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btn1.Location = New System.Drawing.Point(104, 3)
        Me.btn1.Name = "btn1"
        Me.btn1.Size = New System.Drawing.Size(95, 23)
        Me.btn1.TabIndex = 0
        Me.btn1.Text = "btn1"
        '
        'lblText
        '
        Me.lblText.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblText.Location = New System.Drawing.Point(15, 16)
        Me.lblText.Name = "lblText"
        Me.lblText.Size = New System.Drawing.Size(405, 97)
        Me.lblText.TabIndex = 1
        Me.lblText.Text = "lblText"
        '
        'frmMultipleChoiceDlg
        '
        Me.AcceptButton = Me.btn1
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btn0
        Me.ClientSize = New System.Drawing.Size(433, 170)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblText)
        Me.Controls.Add(Me.tlpButtons)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMultipleChoiceDlg"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmMultipleChoiceDlg"
        Me.TopMost = True
        Me.tlpButtons.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tlpButtons As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btn1 As System.Windows.Forms.Button
    Friend WithEvents btn3 As System.Windows.Forms.Button
    Friend WithEvents btn0 As System.Windows.Forms.Button
    Friend WithEvents lblText As System.Windows.Forms.Label
    Friend WithEvents btn2 As System.Windows.Forms.Button

End Class
