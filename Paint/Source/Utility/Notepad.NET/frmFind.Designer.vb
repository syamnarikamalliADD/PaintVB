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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.btnPrev = New System.Windows.Forms.Button
        Me.btnNext = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.lblFind = New System.Windows.Forms.Label
        Me.txtSearchText = New System.Windows.Forms.TextBox
        Me.rdoThisDoc = New System.Windows.Forms.RadioButton
        Me.rdoTopics = New System.Windows.Forms.RadioButton
        Me.rdoAllDocs = New System.Windows.Forms.RadioButton
        Me.chkCaseSens = New System.Windows.Forms.CheckBox
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.Controls.Add(Me.btnPrev, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnNext, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnCancel, 2, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(176, 175)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(268, 29)
        Me.TableLayoutPanel1.TabIndex = 0
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
        Me.lblFind.Location = New System.Drawing.Point(12, 26)
        Me.lblFind.Name = "lblFind"
        Me.lblFind.Size = New System.Drawing.Size(124, 18)
        Me.lblFind.TabIndex = 1
        Me.lblFind.Text = "lblFind"
        Me.lblFind.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtSearchText
        '
        Me.txtSearchText.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSearchText.Location = New System.Drawing.Point(137, 23)
        Me.txtSearchText.Name = "txtSearchText"
        Me.txtSearchText.Size = New System.Drawing.Size(283, 26)
        Me.txtSearchText.TabIndex = 2
        '
        'rdoThisDoc
        '
        Me.rdoThisDoc.AutoSize = True
        Me.rdoThisDoc.Checked = True
        Me.rdoThisDoc.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdoThisDoc.Location = New System.Drawing.Point(12, 79)
        Me.rdoThisDoc.Name = "rdoThisDoc"
        Me.rdoThisDoc.Size = New System.Drawing.Size(107, 22)
        Me.rdoThisDoc.TabIndex = 3
        Me.rdoThisDoc.TabStop = True
        Me.rdoThisDoc.Text = "rdoThisDoc"
        Me.rdoThisDoc.UseVisualStyleBackColor = True
        '
        'rdoTopics
        '
        Me.rdoTopics.AutoSize = True
        Me.rdoTopics.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdoTopics.Location = New System.Drawing.Point(12, 107)
        Me.rdoTopics.Name = "rdoTopics"
        Me.rdoTopics.Size = New System.Drawing.Size(94, 22)
        Me.rdoTopics.TabIndex = 4
        Me.rdoTopics.Text = "rdoTopics"
        Me.rdoTopics.UseVisualStyleBackColor = True
        '
        'rdoAllDocs
        '
        Me.rdoAllDocs.AutoSize = True
        Me.rdoAllDocs.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdoAllDocs.Location = New System.Drawing.Point(12, 135)
        Me.rdoAllDocs.Name = "rdoAllDocs"
        Me.rdoAllDocs.Size = New System.Drawing.Size(103, 22)
        Me.rdoAllDocs.TabIndex = 5
        Me.rdoAllDocs.Text = "rdoAllDocs"
        Me.rdoAllDocs.UseVisualStyleBackColor = True
        '
        'chkCaseSens
        '
        Me.chkCaseSens.AutoSize = True
        Me.chkCaseSens.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.chkCaseSens.Location = New System.Drawing.Point(243, 79)
        Me.chkCaseSens.Name = "chkCaseSens"
        Me.chkCaseSens.Size = New System.Drawing.Size(125, 22)
        Me.chkCaseSens.TabIndex = 6
        Me.chkCaseSens.Text = "chkCaseSens"
        Me.chkCaseSens.UseVisualStyleBackColor = True
        '
        'frmFind
        '
        Me.AcceptButton = Me.btnNext
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(456, 216)
        Me.ControlBox = False
        Me.Controls.Add(Me.chkCaseSens)
        Me.Controls.Add(Me.rdoAllDocs)
        Me.Controls.Add(Me.rdoTopics)
        Me.Controls.Add(Me.rdoThisDoc)
        Me.Controls.Add(Me.txtSearchText)
        Me.Controls.Add(Me.lblFind)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmFind"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "frmFind"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnNext As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents lblFind As System.Windows.Forms.Label
    Friend WithEvents txtSearchText As System.Windows.Forms.TextBox
    Friend WithEvents btnPrev As System.Windows.Forms.Button
    Friend WithEvents rdoThisDoc As System.Windows.Forms.RadioButton
    Friend WithEvents rdoTopics As System.Windows.Forms.RadioButton
    Friend WithEvents rdoAllDocs As System.Windows.Forms.RadioButton
    Friend WithEvents chkCaseSens As System.Windows.Forms.CheckBox

End Class
