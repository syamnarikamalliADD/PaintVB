<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPrintOptions
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.btnOK = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.chkSplitLargeTables = New System.Windows.Forms.CheckBox
        Me.nudMaxRows = New System.Windows.Forms.NumericUpDown
        Me.lblMaxRows = New System.Windows.Forms.Label
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.nudMaxRows, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.btnOK, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnCancel, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(241, 214)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 29)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'btnOK
        '
        Me.btnOK.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btnOK.Location = New System.Drawing.Point(3, 3)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(67, 23)
        Me.btnOK.TabIndex = 0
        Me.btnOK.Text = "btnOK"
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(76, 3)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(67, 23)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "btnCancel"
        '
        'chkSplitLargeTables
        '
        Me.chkSplitLargeTables.AutoSize = True
        Me.chkSplitLargeTables.Location = New System.Drawing.Point(21, 44)
        Me.chkSplitLargeTables.Name = "chkSplitLargeTables"
        Me.chkSplitLargeTables.Size = New System.Drawing.Size(123, 17)
        Me.chkSplitLargeTables.TabIndex = 1
        Me.chkSplitLargeTables.Text = "chkSplitLargeTables"
        Me.chkSplitLargeTables.UseVisualStyleBackColor = True
        '
        'nudMaxRows
        '
        Me.nudMaxRows.Location = New System.Drawing.Point(47, 67)
        Me.nudMaxRows.Name = "nudMaxRows"
        Me.nudMaxRows.Size = New System.Drawing.Size(45, 20)
        Me.nudMaxRows.TabIndex = 2
        '
        'lblMaxRows
        '
        Me.lblMaxRows.AutoSize = True
        Me.lblMaxRows.Location = New System.Drawing.Point(98, 69)
        Me.lblMaxRows.Name = "lblMaxRows"
        Me.lblMaxRows.Size = New System.Drawing.Size(64, 13)
        Me.lblMaxRows.TabIndex = 3
        Me.lblMaxRows.Text = "lblMaxRows"
        '
        'frmPrintOptions
        '
        Me.AcceptButton = Me.btnOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(399, 255)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblMaxRows)
        Me.Controls.Add(Me.nudMaxRows)
        Me.Controls.Add(Me.chkSplitLargeTables)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmPrintOptions"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "frmPrintOptions"
        Me.TableLayoutPanel1.ResumeLayout(False)
        CType(Me.nudMaxRows, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents chkSplitLargeTables As System.Windows.Forms.CheckBox
    Friend WithEvents nudMaxRows As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblMaxRows As System.Windows.Forms.Label

End Class
