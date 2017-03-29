<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmBrowse
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
        Me.btnRefresh = New System.Windows.Forms.Button
        Me.btnOK = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.cboPath = New System.Windows.Forms.ComboBox
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.lstFolders = New System.Windows.Forms.ListView
        Me.colHdr1 = New System.Windows.Forms.ColumnHeader
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnRefresh
        '
        Me.btnRefresh.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnRefresh.Location = New System.Drawing.Point(198, 3)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(84, 21)
        Me.btnRefresh.TabIndex = 4
        Me.btnRefresh.Text = "btnRefresh"
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'btnOK
        '
        Me.btnOK.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnOK.Location = New System.Drawing.Point(108, 237)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(84, 26)
        Me.btnOK.TabIndex = 0
        Me.btnOK.Text = "btnOK"
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnCancel.Location = New System.Drawing.Point(198, 237)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(84, 26)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "btnCancel"
        '
        'cboPath
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.cboPath, 2)
        Me.cboPath.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cboPath.FormattingEnabled = True
        Me.cboPath.Location = New System.Drawing.Point(3, 3)
        Me.cboPath.MaximumSize = New System.Drawing.Size(200, 0)
        Me.cboPath.Name = "cboPath"
        Me.cboPath.Size = New System.Drawing.Size(189, 21)
        Me.cboPath.TabIndex = 2
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.cboPath, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnCancel, 2, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.btnOK, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.lstFolders, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.btnRefresh, 2, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(285, 266)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'lstFolders
        '
        Me.lstFolders.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colHdr1})
        Me.TableLayoutPanel1.SetColumnSpan(Me.lstFolders, 3)
        Me.lstFolders.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstFolders.Location = New System.Drawing.Point(3, 30)
        Me.lstFolders.MultiSelect = False
        Me.lstFolders.Name = "lstFolders"
        Me.lstFolders.ShowGroups = False
        Me.lstFolders.Size = New System.Drawing.Size(279, 201)
        Me.lstFolders.TabIndex = 3
        Me.lstFolders.UseCompatibleStateImageBehavior = False
        '
        'colHdr1
        '
        Me.colHdr1.Text = "Folder"
        '
        'frmBrowse
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(285, 266)
        Me.ControlBox = False
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmBrowse"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "frmBrowse"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnRefresh As System.Windows.Forms.Button
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents cboPath As System.Windows.Forms.ComboBox
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lstFolders As System.Windows.Forms.ListView
    Friend WithEvents colHdr1 As System.Windows.Forms.ColumnHeader

End Class
