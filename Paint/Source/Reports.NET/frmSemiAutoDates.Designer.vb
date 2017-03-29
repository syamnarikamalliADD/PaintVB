<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSemiAutoDates
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
        Me.lblEndCap = New System.Windows.Forms.Label
        Me.lblStartCap = New System.Windows.Forms.Label
        Me.dtpEndTime = New System.Windows.Forms.DateTimePicker
        Me.dtpStartTime = New System.Windows.Forms.DateTimePicker
        Me.dtpEndDate = New System.Windows.Forms.DateTimePicker
        Me.dtpStartDate = New System.Windows.Forms.DateTimePicker
        Me.lblAfterSpan = New System.Windows.Forms.Label
        Me.lblBeforeSpan = New System.Windows.Forms.Label
        Me.lblBeforeEndCap = New System.Windows.Forms.Label
        Me.lblBeforeStartCap = New System.Windows.Forms.Label
        Me.dtpBeforeEndTime = New System.Windows.Forms.DateTimePicker
        Me.dtpBeforeStartTime = New System.Windows.Forms.DateTimePicker
        Me.dtpBeforeEndDate = New System.Windows.Forms.DateTimePicker
        Me.dtpBeforeStartDate = New System.Windows.Forms.DateTimePicker
        Me.numDays = New System.Windows.Forms.NumericUpDown
        Me.numHours = New System.Windows.Forms.NumericUpDown
        Me.lblTimeSpan = New System.Windows.Forms.Label
        Me.lblDays = New System.Windows.Forms.Label
        Me.lblHours = New System.Windows.Forms.Label
        Me.chkSyncTimeSpans = New System.Windows.Forms.CheckBox
        Me.lblMinutes = New System.Windows.Forms.Label
        Me.numMinutes = New System.Windows.Forms.NumericUpDown
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.numDays, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numHours, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numMinutes, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(732, 257)
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
        'lblEndCap
        '
        Me.lblEndCap.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Bold)
        Me.lblEndCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblEndCap.Location = New System.Drawing.Point(470, 143)
        Me.lblEndCap.Name = "lblEndCap"
        Me.lblEndCap.Size = New System.Drawing.Size(406, 22)
        Me.lblEndCap.TabIndex = 13
        Me.lblEndCap.Text = "End"
        Me.lblEndCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblStartCap
        '
        Me.lblStartCap.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Bold)
        Me.lblStartCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblStartCap.Location = New System.Drawing.Point(12, 143)
        Me.lblStartCap.Name = "lblStartCap"
        Me.lblStartCap.Size = New System.Drawing.Size(394, 29)
        Me.lblStartCap.TabIndex = 12
        Me.lblStartCap.Text = "Start"
        Me.lblStartCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'dtpEndTime
        '
        Me.dtpEndTime.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.dtpEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.dtpEndTime.Location = New System.Drawing.Point(750, 177)
        Me.dtpEndTime.Name = "dtpEndTime"
        Me.dtpEndTime.ShowUpDown = True
        Me.dtpEndTime.Size = New System.Drawing.Size(126, 26)
        Me.dtpEndTime.TabIndex = 11
        '
        'dtpStartTime
        '
        Me.dtpStartTime.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.dtpStartTime.Location = New System.Drawing.Point(280, 177)
        Me.dtpStartTime.Name = "dtpStartTime"
        Me.dtpStartTime.ShowUpDown = True
        Me.dtpStartTime.Size = New System.Drawing.Size(126, 26)
        Me.dtpStartTime.TabIndex = 9
        '
        'dtpEndDate
        '
        Me.dtpEndDate.CalendarFont = New System.Drawing.Font("Arial", 12.0!)
        Me.dtpEndDate.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.dtpEndDate.Location = New System.Drawing.Point(472, 177)
        Me.dtpEndDate.Name = "dtpEndDate"
        Me.dtpEndDate.Size = New System.Drawing.Size(271, 26)
        Me.dtpEndDate.TabIndex = 10
        '
        'dtpStartDate
        '
        Me.dtpStartDate.CalendarFont = New System.Drawing.Font("Arial", 12.0!)
        Me.dtpStartDate.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.dtpStartDate.Location = New System.Drawing.Point(12, 177)
        Me.dtpStartDate.Name = "dtpStartDate"
        Me.dtpStartDate.Size = New System.Drawing.Size(262, 26)
        Me.dtpStartDate.TabIndex = 8
        '
        'lblAfterSpan
        '
        Me.lblAfterSpan.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Bold)
        Me.lblAfterSpan.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblAfterSpan.Location = New System.Drawing.Point(249, 115)
        Me.lblAfterSpan.Name = "lblAfterSpan"
        Me.lblAfterSpan.Size = New System.Drawing.Size(394, 29)
        Me.lblAfterSpan.TabIndex = 14
        Me.lblAfterSpan.Text = "lblAfterSpan"
        Me.lblAfterSpan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblBeforeSpan
        '
        Me.lblBeforeSpan.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Bold)
        Me.lblBeforeSpan.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblBeforeSpan.Location = New System.Drawing.Point(250, 8)
        Me.lblBeforeSpan.Name = "lblBeforeSpan"
        Me.lblBeforeSpan.Size = New System.Drawing.Size(394, 29)
        Me.lblBeforeSpan.TabIndex = 21
        Me.lblBeforeSpan.Text = "lblBeforeSpan"
        Me.lblBeforeSpan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblBeforeEndCap
        '
        Me.lblBeforeEndCap.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Bold)
        Me.lblBeforeEndCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblBeforeEndCap.Location = New System.Drawing.Point(471, 35)
        Me.lblBeforeEndCap.Name = "lblBeforeEndCap"
        Me.lblBeforeEndCap.Size = New System.Drawing.Size(406, 22)
        Me.lblBeforeEndCap.TabIndex = 20
        Me.lblBeforeEndCap.Text = "End"
        Me.lblBeforeEndCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblBeforeStartCap
        '
        Me.lblBeforeStartCap.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Bold)
        Me.lblBeforeStartCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblBeforeStartCap.Location = New System.Drawing.Point(13, 35)
        Me.lblBeforeStartCap.Name = "lblBeforeStartCap"
        Me.lblBeforeStartCap.Size = New System.Drawing.Size(394, 29)
        Me.lblBeforeStartCap.TabIndex = 19
        Me.lblBeforeStartCap.Text = "Start"
        Me.lblBeforeStartCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'dtpBeforeEndTime
        '
        Me.dtpBeforeEndTime.CustomFormat = ""
        Me.dtpBeforeEndTime.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.dtpBeforeEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.dtpBeforeEndTime.Location = New System.Drawing.Point(751, 69)
        Me.dtpBeforeEndTime.Name = "dtpBeforeEndTime"
        Me.dtpBeforeEndTime.ShowUpDown = True
        Me.dtpBeforeEndTime.Size = New System.Drawing.Size(126, 26)
        Me.dtpBeforeEndTime.TabIndex = 18
        '
        'dtpBeforeStartTime
        '
        Me.dtpBeforeStartTime.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.dtpBeforeStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.dtpBeforeStartTime.Location = New System.Drawing.Point(281, 69)
        Me.dtpBeforeStartTime.Name = "dtpBeforeStartTime"
        Me.dtpBeforeStartTime.ShowUpDown = True
        Me.dtpBeforeStartTime.Size = New System.Drawing.Size(126, 26)
        Me.dtpBeforeStartTime.TabIndex = 16
        '
        'dtpBeforeEndDate
        '
        Me.dtpBeforeEndDate.CalendarFont = New System.Drawing.Font("Arial", 12.0!)
        Me.dtpBeforeEndDate.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.dtpBeforeEndDate.Location = New System.Drawing.Point(473, 69)
        Me.dtpBeforeEndDate.Name = "dtpBeforeEndDate"
        Me.dtpBeforeEndDate.Size = New System.Drawing.Size(271, 26)
        Me.dtpBeforeEndDate.TabIndex = 17
        '
        'dtpBeforeStartDate
        '
        Me.dtpBeforeStartDate.CalendarFont = New System.Drawing.Font("Arial", 12.0!)
        Me.dtpBeforeStartDate.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.dtpBeforeStartDate.Location = New System.Drawing.Point(13, 69)
        Me.dtpBeforeStartDate.Name = "dtpBeforeStartDate"
        Me.dtpBeforeStartDate.Size = New System.Drawing.Size(262, 26)
        Me.dtpBeforeStartDate.TabIndex = 15
        '
        'numDays
        '
        Me.numDays.Font = New System.Drawing.Font("Arial Narrow", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.numDays.Location = New System.Drawing.Point(317, 250)
        Me.numDays.Name = "numDays"
        Me.numDays.Size = New System.Drawing.Size(43, 26)
        Me.numDays.TabIndex = 22
        '
        'numHours
        '
        Me.numHours.Font = New System.Drawing.Font("Arial Narrow", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.numHours.Location = New System.Drawing.Point(443, 250)
        Me.numHours.Name = "numHours"
        Me.numHours.Size = New System.Drawing.Size(43, 26)
        Me.numHours.TabIndex = 23
        '
        'lblTimeSpan
        '
        Me.lblTimeSpan.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Bold)
        Me.lblTimeSpan.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblTimeSpan.Location = New System.Drawing.Point(136, 249)
        Me.lblTimeSpan.Name = "lblTimeSpan"
        Me.lblTimeSpan.Size = New System.Drawing.Size(177, 29)
        Me.lblTimeSpan.TabIndex = 24
        Me.lblTimeSpan.Text = "lblTimeSpan"
        Me.lblTimeSpan.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblDays
        '
        Me.lblDays.Font = New System.Drawing.Font("Arial Narrow", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDays.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblDays.Location = New System.Drawing.Point(364, 249)
        Me.lblDays.Name = "lblDays"
        Me.lblDays.Size = New System.Drawing.Size(75, 29)
        Me.lblDays.TabIndex = 25
        Me.lblDays.Text = "lblDays"
        Me.lblDays.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblHours
        '
        Me.lblHours.Font = New System.Drawing.Font("Arial Narrow", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHours.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblHours.Location = New System.Drawing.Point(490, 249)
        Me.lblHours.Name = "lblHours"
        Me.lblHours.Size = New System.Drawing.Size(75, 29)
        Me.lblHours.TabIndex = 26
        Me.lblHours.Text = "lblHours"
        Me.lblHours.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'chkSyncTimeSpans
        '
        Me.chkSyncTimeSpans.AutoSize = True
        Me.chkSyncTimeSpans.Location = New System.Drawing.Point(11, 255)
        Me.chkSyncTimeSpans.Name = "chkSyncTimeSpans"
        Me.chkSyncTimeSpans.Size = New System.Drawing.Size(121, 17)
        Me.chkSyncTimeSpans.TabIndex = 27
        Me.chkSyncTimeSpans.Text = "chkSyncTimeSpans"
        Me.chkSyncTimeSpans.UseVisualStyleBackColor = True
        '
        'lblMinutes
        '
        Me.lblMinutes.Font = New System.Drawing.Font("Arial Narrow", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMinutes.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblMinutes.Location = New System.Drawing.Point(616, 249)
        Me.lblMinutes.Name = "lblMinutes"
        Me.lblMinutes.Size = New System.Drawing.Size(75, 29)
        Me.lblMinutes.TabIndex = 29
        Me.lblMinutes.Text = "lblMinutes"
        Me.lblMinutes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'numMinutes
        '
        Me.numMinutes.Font = New System.Drawing.Font("Arial Narrow", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.numMinutes.Location = New System.Drawing.Point(569, 250)
        Me.numMinutes.Name = "numMinutes"
        Me.numMinutes.Size = New System.Drawing.Size(43, 26)
        Me.numMinutes.TabIndex = 28
        '
        'frmSemiAutoDates
        '
        Me.AcceptButton = Me.btnOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(890, 298)
        Me.Controls.Add(Me.lblMinutes)
        Me.Controls.Add(Me.numMinutes)
        Me.Controls.Add(Me.chkSyncTimeSpans)
        Me.Controls.Add(Me.lblHours)
        Me.Controls.Add(Me.lblDays)
        Me.Controls.Add(Me.lblTimeSpan)
        Me.Controls.Add(Me.numHours)
        Me.Controls.Add(Me.numDays)
        Me.Controls.Add(Me.lblBeforeSpan)
        Me.Controls.Add(Me.lblBeforeEndCap)
        Me.Controls.Add(Me.lblBeforeStartCap)
        Me.Controls.Add(Me.dtpBeforeEndTime)
        Me.Controls.Add(Me.dtpBeforeStartTime)
        Me.Controls.Add(Me.dtpBeforeEndDate)
        Me.Controls.Add(Me.dtpBeforeStartDate)
        Me.Controls.Add(Me.lblAfterSpan)
        Me.Controls.Add(Me.lblEndCap)
        Me.Controls.Add(Me.lblStartCap)
        Me.Controls.Add(Me.dtpEndTime)
        Me.Controls.Add(Me.dtpStartTime)
        Me.Controls.Add(Me.dtpEndDate)
        Me.Controls.Add(Me.dtpStartDate)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmSemiAutoDates"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "frmSemiAutoDates"
        Me.TableLayoutPanel1.ResumeLayout(False)
        CType(Me.numDays, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numHours, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numMinutes, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents lblEndCap As System.Windows.Forms.Label
    Friend WithEvents lblStartCap As System.Windows.Forms.Label
    Friend WithEvents dtpEndTime As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpStartTime As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpEndDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpStartDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblAfterSpan As System.Windows.Forms.Label
    Friend WithEvents lblBeforeSpan As System.Windows.Forms.Label
    Friend WithEvents lblBeforeEndCap As System.Windows.Forms.Label
    Friend WithEvents lblBeforeStartCap As System.Windows.Forms.Label
    Friend WithEvents dtpBeforeEndTime As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpBeforeStartTime As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpBeforeEndDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpBeforeStartDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents numDays As System.Windows.Forms.NumericUpDown
    Friend WithEvents numHours As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblTimeSpan As System.Windows.Forms.Label
    Friend WithEvents lblDays As System.Windows.Forms.Label
    Friend WithEvents lblHours As System.Windows.Forms.Label
    Friend WithEvents chkSyncTimeSpans As System.Windows.Forms.CheckBox
    Friend WithEvents lblMinutes As System.Windows.Forms.Label
    Friend WithEvents numMinutes As System.Windows.Forms.NumericUpDown

End Class
