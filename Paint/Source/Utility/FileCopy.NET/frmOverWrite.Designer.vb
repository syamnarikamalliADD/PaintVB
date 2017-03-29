<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmOverWrite
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmOverWrite))
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.btnYesAll = New System.Windows.Forms.Button
        Me.btnNo = New System.Windows.Forms.Button
        Me.btnYes = New System.Windows.Forms.Button
        Me.lblText = New System.Windows.Forms.Label
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.Controls.Add(Me.btnYesAll, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnNo, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnYes, 0, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(135, 127)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(288, 29)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'btnYesAll
        '
        Me.btnYesAll.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnYesAll.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnYesAll.Location = New System.Drawing.Point(195, 3)
        Me.btnYesAll.Name = "btnYesAll"
        Me.btnYesAll.Size = New System.Drawing.Size(90, 23)
        Me.btnYesAll.TabIndex = 4
        Me.btnYesAll.Text = "btnYesAll"
        '
        'btnNo
        '
        Me.btnNo.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnNo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnNo.Location = New System.Drawing.Point(99, 3)
        Me.btnNo.Name = "btnNo"
        Me.btnNo.Size = New System.Drawing.Size(90, 23)
        Me.btnNo.TabIndex = 3
        Me.btnNo.Text = "btnNo"
        '
        'btnYes
        '
        Me.btnYes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnYes.Location = New System.Drawing.Point(3, 3)
        Me.btnYes.Name = "btnYes"
        Me.btnYes.Size = New System.Drawing.Size(90, 23)
        Me.btnYes.TabIndex = 0
        Me.btnYes.Text = "btnYes"
        '
        'lblText
        '
        Me.lblText.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblText.Location = New System.Drawing.Point(15, 16)
        Me.lblText.Name = "lblText"
        Me.lblText.Size = New System.Drawing.Size(405, 97)
        Me.lblText.TabIndex = 1
        Me.lblText.Text = "FILENAME already exists at LOCATION.  Overwrite?"
        '
        'frmOverWrite
        '
        Me.AcceptButton = Me.btnYes
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnNo
        Me.ClientSize = New System.Drawing.Size(433, 170)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblText)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmOverWrite"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "frmOverWrite"
        Me.TopMost = True
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnYes As System.Windows.Forms.Button
    Friend WithEvents btnYesAll As System.Windows.Forms.Button
    Friend WithEvents btnNo As System.Windows.Forms.Button
    Friend WithEvents lblText As System.Windows.Forms.Label

End Class
