<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmFilter
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
        Me.lblTopic = New System.Windows.Forms.Label
        Me.txtTopic = New System.Windows.Forms.TextBox
        Me.lblAuthor = New System.Windows.Forms.Label
        Me.dtpStartDateMod = New System.Windows.Forms.DateTimePicker
        Me.lblStartDateMod = New System.Windows.Forms.Label
        Me.lblEndDateMod = New System.Windows.Forms.Label
        Me.dtpEndDateMod = New System.Windows.Forms.DateTimePicker
        Me.dtpEndTimeMod = New System.Windows.Forms.DateTimePicker
        Me.dtpStartTimeMod = New System.Windows.Forms.DateTimePicker
        Me.lblModifiedDate = New System.Windows.Forms.Label
        Me.lblCreatedDate = New System.Windows.Forms.Label
        Me.dtpEndTimeCre = New System.Windows.Forms.DateTimePicker
        Me.dtpStartTimeCre = New System.Windows.Forms.DateTimePicker
        Me.lblEndDateCre = New System.Windows.Forms.Label
        Me.dtpEndDateCre = New System.Windows.Forms.DateTimePicker
        Me.lblStartDateCre = New System.Windows.Forms.Label
        Me.dtpStartDateCre = New System.Windows.Forms.DateTimePicker
        Me.cboAuthor = New System.Windows.Forms.ComboBox
        Me.chkWildcards = New System.Windows.Forms.CheckBox
        Me.TableLayoutPanel1.SuspendLayout()
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
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(306, 317)
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
        Me.btnOK.Text = "OK"
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(76, 3)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(67, 23)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "Cancel"
        '
        'lblTopic
        '
        Me.lblTopic.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTopic.Location = New System.Drawing.Point(12, 26)
        Me.lblTopic.Name = "lblTopic"
        Me.lblTopic.Size = New System.Drawing.Size(192, 18)
        Me.lblTopic.TabIndex = 1
        Me.lblTopic.Text = "lblTopic"
        Me.lblTopic.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtTopic
        '
        Me.txtTopic.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtTopic.Location = New System.Drawing.Point(209, 23)
        Me.txtTopic.Name = "txtTopic"
        Me.txtTopic.Size = New System.Drawing.Size(214, 26)
        Me.txtTopic.TabIndex = 2
        '
        'lblAuthor
        '
        Me.lblAuthor.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAuthor.Location = New System.Drawing.Point(12, 56)
        Me.lblAuthor.Name = "lblAuthor"
        Me.lblAuthor.Size = New System.Drawing.Size(192, 18)
        Me.lblAuthor.TabIndex = 3
        Me.lblAuthor.Text = "lblAuthor"
        Me.lblAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'dtpStartDateMod
        '
        Me.dtpStartDateMod.Checked = False
        Me.dtpStartDateMod.Location = New System.Drawing.Point(158, 142)
        Me.dtpStartDateMod.Name = "dtpStartDateMod"
        Me.dtpStartDateMod.ShowCheckBox = True
        Me.dtpStartDateMod.Size = New System.Drawing.Size(171, 20)
        Me.dtpStartDateMod.TabIndex = 5
        '
        'lblStartDateMod
        '
        Me.lblStartDateMod.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblStartDateMod.Location = New System.Drawing.Point(29, 142)
        Me.lblStartDateMod.Name = "lblStartDateMod"
        Me.lblStartDateMod.Size = New System.Drawing.Size(121, 18)
        Me.lblStartDateMod.TabIndex = 6
        Me.lblStartDateMod.Text = "lblStartDateMod"
        Me.lblStartDateMod.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblEndDateMod
        '
        Me.lblEndDateMod.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEndDateMod.Location = New System.Drawing.Point(29, 172)
        Me.lblEndDateMod.Name = "lblEndDateMod"
        Me.lblEndDateMod.Size = New System.Drawing.Size(121, 18)
        Me.lblEndDateMod.TabIndex = 8
        Me.lblEndDateMod.Text = "lblEndDateMod"
        Me.lblEndDateMod.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'dtpEndDateMod
        '
        Me.dtpEndDateMod.Checked = False
        Me.dtpEndDateMod.Location = New System.Drawing.Point(158, 172)
        Me.dtpEndDateMod.Name = "dtpEndDateMod"
        Me.dtpEndDateMod.ShowCheckBox = True
        Me.dtpEndDateMod.Size = New System.Drawing.Size(171, 20)
        Me.dtpEndDateMod.TabIndex = 7
        '
        'dtpEndTimeMod
        '
        Me.dtpEndTimeMod.Checked = False
        Me.dtpEndTimeMod.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.dtpEndTimeMod.Location = New System.Drawing.Point(335, 172)
        Me.dtpEndTimeMod.Name = "dtpEndTimeMod"
        Me.dtpEndTimeMod.ShowUpDown = True
        Me.dtpEndTimeMod.Size = New System.Drawing.Size(88, 20)
        Me.dtpEndTimeMod.TabIndex = 11
        '
        'dtpStartTimeMod
        '
        Me.dtpStartTimeMod.Checked = False
        Me.dtpStartTimeMod.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.dtpStartTimeMod.Location = New System.Drawing.Point(335, 142)
        Me.dtpStartTimeMod.Name = "dtpStartTimeMod"
        Me.dtpStartTimeMod.ShowUpDown = True
        Me.dtpStartTimeMod.Size = New System.Drawing.Size(88, 20)
        Me.dtpStartTimeMod.TabIndex = 10
        '
        'lblModifiedDate
        '
        Me.lblModifiedDate.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblModifiedDate.Location = New System.Drawing.Point(32, 121)
        Me.lblModifiedDate.Name = "lblModifiedDate"
        Me.lblModifiedDate.Size = New System.Drawing.Size(391, 18)
        Me.lblModifiedDate.TabIndex = 12
        Me.lblModifiedDate.Text = "lblModifiedDate"
        Me.lblModifiedDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblCreatedDate
        '
        Me.lblCreatedDate.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCreatedDate.Location = New System.Drawing.Point(32, 205)
        Me.lblCreatedDate.Name = "lblCreatedDate"
        Me.lblCreatedDate.Size = New System.Drawing.Size(391, 18)
        Me.lblCreatedDate.TabIndex = 19
        Me.lblCreatedDate.Text = "lblCreatedDate"
        Me.lblCreatedDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'dtpEndTimeCre
        '
        Me.dtpEndTimeCre.Checked = False
        Me.dtpEndTimeCre.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.dtpEndTimeCre.Location = New System.Drawing.Point(335, 256)
        Me.dtpEndTimeCre.Name = "dtpEndTimeCre"
        Me.dtpEndTimeCre.ShowUpDown = True
        Me.dtpEndTimeCre.Size = New System.Drawing.Size(88, 20)
        Me.dtpEndTimeCre.TabIndex = 18
        '
        'dtpStartTimeCre
        '
        Me.dtpStartTimeCre.Checked = False
        Me.dtpStartTimeCre.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.dtpStartTimeCre.Location = New System.Drawing.Point(335, 226)
        Me.dtpStartTimeCre.Name = "dtpStartTimeCre"
        Me.dtpStartTimeCre.ShowUpDown = True
        Me.dtpStartTimeCre.Size = New System.Drawing.Size(88, 20)
        Me.dtpStartTimeCre.TabIndex = 17
        '
        'lblEndDateCre
        '
        Me.lblEndDateCre.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEndDateCre.Location = New System.Drawing.Point(29, 256)
        Me.lblEndDateCre.Name = "lblEndDateCre"
        Me.lblEndDateCre.Size = New System.Drawing.Size(121, 18)
        Me.lblEndDateCre.TabIndex = 16
        Me.lblEndDateCre.Text = "lblEndDateCre"
        Me.lblEndDateCre.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'dtpEndDateCre
        '
        Me.dtpEndDateCre.Checked = False
        Me.dtpEndDateCre.Location = New System.Drawing.Point(158, 256)
        Me.dtpEndDateCre.Name = "dtpEndDateCre"
        Me.dtpEndDateCre.ShowCheckBox = True
        Me.dtpEndDateCre.Size = New System.Drawing.Size(171, 20)
        Me.dtpEndDateCre.TabIndex = 15
        '
        'lblStartDateCre
        '
        Me.lblStartDateCre.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblStartDateCre.Location = New System.Drawing.Point(29, 226)
        Me.lblStartDateCre.Name = "lblStartDateCre"
        Me.lblStartDateCre.Size = New System.Drawing.Size(121, 18)
        Me.lblStartDateCre.TabIndex = 14
        Me.lblStartDateCre.Text = "lblStartDateCre"
        Me.lblStartDateCre.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'dtpStartDateCre
        '
        Me.dtpStartDateCre.Checked = False
        Me.dtpStartDateCre.Location = New System.Drawing.Point(158, 226)
        Me.dtpStartDateCre.Name = "dtpStartDateCre"
        Me.dtpStartDateCre.ShowCheckBox = True
        Me.dtpStartDateCre.Size = New System.Drawing.Size(171, 20)
        Me.dtpStartDateCre.TabIndex = 13
        '
        'cboAuthor
        '
        Me.cboAuthor.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboAuthor.FormattingEnabled = True
        Me.cboAuthor.Location = New System.Drawing.Point(209, 55)
        Me.cboAuthor.Name = "cboAuthor"
        Me.cboAuthor.Size = New System.Drawing.Size(214, 26)
        Me.cboAuthor.TabIndex = 9
        '
        'chkWildcards
        '
        Me.chkWildcards.AutoSize = True
        Me.chkWildcards.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.chkWildcards.Location = New System.Drawing.Point(209, 87)
        Me.chkWildcards.Name = "chkWildcards"
        Me.chkWildcards.Size = New System.Drawing.Size(121, 22)
        Me.chkWildcards.TabIndex = 20
        Me.chkWildcards.Text = "chkWildcards"
        Me.chkWildcards.UseVisualStyleBackColor = True
        '
        'frmFilter
        '
        Me.AcceptButton = Me.btnOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(464, 358)
        Me.ControlBox = False
        Me.Controls.Add(Me.chkWildcards)
        Me.Controls.Add(Me.lblCreatedDate)
        Me.Controls.Add(Me.dtpEndTimeCre)
        Me.Controls.Add(Me.dtpStartTimeCre)
        Me.Controls.Add(Me.lblEndDateCre)
        Me.Controls.Add(Me.dtpEndDateCre)
        Me.Controls.Add(Me.lblStartDateCre)
        Me.Controls.Add(Me.dtpStartDateCre)
        Me.Controls.Add(Me.lblModifiedDate)
        Me.Controls.Add(Me.dtpEndTimeMod)
        Me.Controls.Add(Me.dtpStartTimeMod)
        Me.Controls.Add(Me.cboAuthor)
        Me.Controls.Add(Me.lblEndDateMod)
        Me.Controls.Add(Me.dtpEndDateMod)
        Me.Controls.Add(Me.lblStartDateMod)
        Me.Controls.Add(Me.dtpStartDateMod)
        Me.Controls.Add(Me.lblAuthor)
        Me.Controls.Add(Me.txtTopic)
        Me.Controls.Add(Me.lblTopic)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmFilter"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "frmTopic"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents lblTopic As System.Windows.Forms.Label
    Friend WithEvents txtTopic As System.Windows.Forms.TextBox
    Friend WithEvents lblAuthor As System.Windows.Forms.Label
    Friend WithEvents dtpStartDateMod As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblStartDateMod As System.Windows.Forms.Label
    Friend WithEvents lblEndDateMod As System.Windows.Forms.Label
    Friend WithEvents dtpEndDateMod As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpEndTimeMod As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpStartTimeMod As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblModifiedDate As System.Windows.Forms.Label
    Friend WithEvents lblCreatedDate As System.Windows.Forms.Label
    Friend WithEvents dtpEndTimeCre As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpStartTimeCre As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblEndDateCre As System.Windows.Forms.Label
    Friend WithEvents dtpEndDateCre As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblStartDateCre As System.Windows.Forms.Label
    Friend WithEvents dtpStartDateCre As System.Windows.Forms.DateTimePicker
    Friend WithEvents cboAuthor As System.Windows.Forms.ComboBox
    Friend WithEvents chkWildcards As System.Windows.Forms.CheckBox

End Class
