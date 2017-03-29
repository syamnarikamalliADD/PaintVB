<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BingoBoard
    Inherits System.Windows.Forms.UserControl

    'UserControl1 overrides dispose to clean up the component list.
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
        Me.pnlBingoBoard = New System.Windows.Forms.Panel
        Me.lblTitle = New System.Windows.Forms.Label
        Me.lblTemplate = New System.Windows.Forms.Label
        Me.pnlBingoBoard.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlBingoBoard
        '
        Me.pnlBingoBoard.BackColor = System.Drawing.Color.Black
        Me.pnlBingoBoard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlBingoBoard.Controls.Add(Me.lblTitle)
        Me.pnlBingoBoard.Controls.Add(Me.lblTemplate)
        Me.pnlBingoBoard.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlBingoBoard.Location = New System.Drawing.Point(0, 0)
        Me.pnlBingoBoard.Name = "pnlBingoBoard"
        Me.pnlBingoBoard.Size = New System.Drawing.Size(150, 162)
        Me.pnlBingoBoard.TabIndex = 2
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.BackColor = System.Drawing.Color.Black
        Me.lblTitle.ForeColor = System.Drawing.Color.White
        Me.lblTitle.Location = New System.Drawing.Point(50, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(26, 14)
        Me.lblTitle.TabIndex = 3
        Me.lblTitle.Text = "Title"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblTemplate
        '
        Me.lblTemplate.AutoSize = True
        Me.lblTemplate.BackColor = System.Drawing.Color.Black
        Me.lblTemplate.ForeColor = System.Drawing.Color.Red
        Me.lblTemplate.Location = New System.Drawing.Point(3, 25)
        Me.lblTemplate.Name = "lblTemplate"
        Me.lblTemplate.Size = New System.Drawing.Size(60, 14)
        Me.lblTemplate.TabIndex = 2
        Me.lblTemplate.Tag = "-1"
        Me.lblTemplate.Text = "lblTemplate"
        Me.lblTemplate.Visible = False
        '
        'BingoBoard
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.Controls.Add(Me.pnlBingoBoard)
        Me.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "BingoBoard"
        Me.Size = New System.Drawing.Size(150, 162)
        Me.pnlBingoBoard.ResumeLayout(False)
        Me.pnlBingoBoard.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents pnlBingoBoard As System.Windows.Forms.Panel
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblTemplate As System.Windows.Forms.Label

End Class
