<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMiniGrid
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
        Me.dgvMini = New System.Windows.Forms.DataGridView
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        CType(Me.dgvMini, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tlsMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'dgvMini
        '
        Me.dgvMini.AllowUserToAddRows = False
        Me.dgvMini.AllowUserToDeleteRows = False
        Me.dgvMini.BackgroundColor = System.Drawing.SystemColors.Control
        Me.dgvMini.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.dgvMini.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvMini.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.dgvMini.Location = New System.Drawing.Point(12, 53)
        Me.dgvMini.Name = "dgvMini"
        Me.dgvMini.ReadOnly = True
        Me.dgvMini.Size = New System.Drawing.Size(617, 404)
        Me.dgvMini.TabIndex = 0
        '
        'tlsMain
        '
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose})
        Me.tlsMain.Location = New System.Drawing.Point(0, 0)
        Me.tlsMain.MinimumSize = New System.Drawing.Size(0, 50)
        Me.tlsMain.Name = "tlsMain"
        Me.tlsMain.Size = New System.Drawing.Size(737, 50)
        Me.tlsMain.Stretch = True
        Me.tlsMain.TabIndex = 3
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
        'frmMiniGrid
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(737, 519)
        Me.Controls.Add(Me.tlsMain)
        Me.Controls.Add(Me.dgvMini)
        Me.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMiniGrid"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.Text = "Data"
        CType(Me.dgvMini, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tlsMain.ResumeLayout(False)
        Me.tlsMain.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents dgvMini As System.Windows.Forms.DataGridView
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnClose As System.Windows.Forms.ToolStripButton
End Class
