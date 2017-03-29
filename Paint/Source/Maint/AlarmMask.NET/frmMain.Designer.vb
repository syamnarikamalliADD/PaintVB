<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.btnRead = New System.Windows.Forms.Button
        Me.lstMaskedAlarms = New System.Windows.Forms.ListBox
        Me.btnWrite = New System.Windows.Forms.Button
        Me.cboFacilityName = New System.Windows.Forms.ComboBox
        Me.lblHyphen = New System.Windows.Forms.Label
        Me.lblFacilityName = New System.Windows.Forms.Label
        Me.txtAlarmNumber = New System.Windows.Forms.TextBox
        Me.lblStatus = New System.Windows.Forms.Label
        Me.lblAlarmNumber = New System.Windows.Forms.Label
        Me.btnRemove = New System.Windows.Forms.Button
        Me.btnAdd = New System.Windows.Forms.Button
        Me.btnCheck = New System.Windows.Forms.Button
        Me.lblStatusCap = New System.Windows.Forms.Label
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog
        Me.SuspendLayout()
        '
        'btnRead
        '
        Me.btnRead.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRead.Location = New System.Drawing.Point(21, 27)
        Me.btnRead.Name = "btnRead"
        Me.btnRead.Size = New System.Drawing.Size(270, 23)
        Me.btnRead.TabIndex = 15
        Me.btnRead.Text = "Read Masked Alarms from XML File"
        Me.ToolTip1.SetToolTip(Me.btnRead, "Get the list of masked alarms from the data file.")
        Me.btnRead.UseVisualStyleBackColor = True
        '
        'lstMaskedAlarms
        '
        Me.lstMaskedAlarms.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstMaskedAlarms.FormattingEnabled = True
        Me.lstMaskedAlarms.ItemHeight = 16
        Me.lstMaskedAlarms.Location = New System.Drawing.Point(322, 27)
        Me.lstMaskedAlarms.Name = "lstMaskedAlarms"
        Me.lstMaskedAlarms.Size = New System.Drawing.Size(120, 292)
        Me.lstMaskedAlarms.Sorted = True
        Me.lstMaskedAlarms.TabIndex = 16
        Me.ToolTip1.SetToolTip(Me.lstMaskedAlarms, "Click on an alarm to select it.")
        '
        'btnWrite
        '
        Me.btnWrite.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnWrite.Location = New System.Drawing.Point(21, 69)
        Me.btnWrite.Name = "btnWrite"
        Me.btnWrite.Size = New System.Drawing.Size(270, 23)
        Me.btnWrite.TabIndex = 17
        Me.btnWrite.Text = "Write Masked Alarms to XML File"
        Me.ToolTip1.SetToolTip(Me.btnWrite, "Save the list of masked alarms to the data file.")
        Me.btnWrite.UseVisualStyleBackColor = True
        '
        'cboFacilityName
        '
        Me.cboFacilityName.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboFacilityName.FormattingEnabled = True
        Me.cboFacilityName.Location = New System.Drawing.Point(21, 144)
        Me.cboFacilityName.Name = "cboFacilityName"
        Me.cboFacilityName.Size = New System.Drawing.Size(84, 24)
        Me.cboFacilityName.Sorted = True
        Me.cboFacilityName.TabIndex = 18
        Me.ToolTip1.SetToolTip(Me.cboFacilityName, "Enter or Select a valid Facility Name.")
        '
        'lblHyphen
        '
        Me.lblHyphen.AutoSize = True
        Me.lblHyphen.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHyphen.Location = New System.Drawing.Point(111, 144)
        Me.lblHyphen.Name = "lblHyphen"
        Me.lblHyphen.Size = New System.Drawing.Size(14, 19)
        Me.lblHyphen.TabIndex = 19
        Me.lblHyphen.Text = "-"
        '
        'lblFacilityName
        '
        Me.lblFacilityName.AutoSize = True
        Me.lblFacilityName.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblFacilityName.Location = New System.Drawing.Point(21, 116)
        Me.lblFacilityName.Name = "lblFacilityName"
        Me.lblFacilityName.Size = New System.Drawing.Size(88, 16)
        Me.lblFacilityName.TabIndex = 20
        Me.lblFacilityName.Text = "Facility Name"
        '
        'txtAlarmNumber
        '
        Me.txtAlarmNumber.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtAlarmNumber.Location = New System.Drawing.Point(131, 144)
        Me.txtAlarmNumber.MaxLength = 3
        Me.txtAlarmNumber.Name = "txtAlarmNumber"
        Me.txtAlarmNumber.Size = New System.Drawing.Size(48, 22)
        Me.txtAlarmNumber.TabIndex = 21
        Me.txtAlarmNumber.Text = "000"
        Me.txtAlarmNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.ToolTip1.SetToolTip(Me.txtAlarmNumber, "Enter an Alarm Number from 0 to 999 or an asterisk (*) to mask ALL alarms for thi" & _
                "s Facility.")
        '
        'lblStatus
        '
        Me.lblStatus.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblStatus.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblStatus.Location = New System.Drawing.Point(21, 349)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(420, 25)
        Me.lblStatus.TabIndex = 22
        '
        'lblAlarmNumber
        '
        Me.lblAlarmNumber.AutoSize = True
        Me.lblAlarmNumber.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAlarmNumber.Location = New System.Drawing.Point(130, 116)
        Me.lblAlarmNumber.Name = "lblAlarmNumber"
        Me.lblAlarmNumber.Size = New System.Drawing.Size(144, 16)
        Me.lblAlarmNumber.TabIndex = 23
        Me.lblAlarmNumber.Text = "Alarm # ( * = Mask All )"
        '
        'btnRemove
        '
        Me.btnRemove.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRemove.Location = New System.Drawing.Point(21, 243)
        Me.btnRemove.Name = "btnRemove"
        Me.btnRemove.Size = New System.Drawing.Size(270, 22)
        Me.btnRemove.TabIndex = 24
        Me.btnRemove.Text = "Remove Masked Alarm from List"
        Me.ToolTip1.SetToolTip(Me.btnRemove, "Remove the selected alarm from the list of masked alarms.")
        Me.btnRemove.UseVisualStyleBackColor = True
        '
        'btnAdd
        '
        Me.btnAdd.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAdd.Location = New System.Drawing.Point(21, 201)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(270, 22)
        Me.btnAdd.TabIndex = 25
        Me.btnAdd.Text = "Add Masked Alarm to List"
        Me.ToolTip1.SetToolTip(Me.btnAdd, "Add the alarm above to the list of masked alarms")
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'btnCheck
        '
        Me.btnCheck.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCheck.Location = New System.Drawing.Point(21, 285)
        Me.btnCheck.Name = "btnCheck"
        Me.btnCheck.Size = New System.Drawing.Size(270, 22)
        Me.btnCheck.TabIndex = 26
        Me.btnCheck.Text = "Check Alarm Status"
        Me.ToolTip1.SetToolTip(Me.btnCheck, "Check whether the selected alarm is currently masked.")
        Me.btnCheck.UseVisualStyleBackColor = True
        '
        'lblStatusCap
        '
        Me.lblStatusCap.AutoSize = True
        Me.lblStatusCap.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblStatusCap.Location = New System.Drawing.Point(18, 327)
        Me.lblStatusCap.Name = "lblStatusCap"
        Me.lblStatusCap.Size = New System.Drawing.Size(46, 16)
        Me.lblStatusCap.TabIndex = 27
        Me.lblStatusCap.Text = "Status"
        '
        'ToolTip1
        '
        Me.ToolTip1.IsBalloon = True
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(454, 379)
        Me.Controls.Add(Me.lblStatusCap)
        Me.Controls.Add(Me.btnCheck)
        Me.Controls.Add(Me.btnAdd)
        Me.Controls.Add(Me.btnRemove)
        Me.Controls.Add(Me.lblAlarmNumber)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.txtAlarmNumber)
        Me.Controls.Add(Me.lblFacilityName)
        Me.Controls.Add(Me.lblHyphen)
        Me.Controls.Add(Me.cboFacilityName)
        Me.Controls.Add(Me.btnWrite)
        Me.Controls.Add(Me.lstMaskedAlarms)
        Me.Controls.Add(Me.btnRead)
        Me.DoubleBuffered = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximumSize = New System.Drawing.Size(470, 417)
        Me.MinimumSize = New System.Drawing.Size(470, 417)
        Me.Name = "frmMain"
        Me.Text = "Alarm Mask Editor Utility"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnRead As System.Windows.Forms.Button
    Friend WithEvents lstMaskedAlarms As System.Windows.Forms.ListBox
    Friend WithEvents btnWrite As System.Windows.Forms.Button
    Friend WithEvents cboFacilityName As System.Windows.Forms.ComboBox
    Friend WithEvents lblHyphen As System.Windows.Forms.Label
    Friend WithEvents lblFacilityName As System.Windows.Forms.Label
    Friend WithEvents txtAlarmNumber As System.Windows.Forms.TextBox
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents lblAlarmNumber As System.Windows.Forms.Label
    Friend WithEvents btnRemove As System.Windows.Forms.Button
    Friend WithEvents btnAdd As System.Windows.Forms.Button
    Friend WithEvents btnCheck As System.Windows.Forms.Button
    Friend WithEvents lblStatusCap As System.Windows.Forms.Label
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog

End Class
