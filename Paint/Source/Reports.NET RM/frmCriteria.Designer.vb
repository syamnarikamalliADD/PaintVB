<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCriteria
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCriteria))
        Me.btnOK = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.dtpStartDate = New System.Windows.Forms.DateTimePicker
        Me.dtpEndDate = New System.Windows.Forms.DateTimePicker
        Me.dtpStartTime = New System.Windows.Forms.DateTimePicker
        Me.dtpEndTime = New System.Windows.Forms.DateTimePicker
        Me.lblStartCap = New System.Windows.Forms.Label
        Me.lblEndCap = New System.Windows.Forms.Label
        Me.gpbShift = New System.Windows.Forms.GroupBox
        Me.chkDaily = New System.Windows.Forms.CheckBox
        Me.cboZone = New System.Windows.Forms.ComboBox
        Me.btnShift3 = New System.Windows.Forms.Button
        Me.btnShift2 = New System.Windows.Forms.Button
        Me.btnShift1 = New System.Windows.Forms.Button
        Me.gpbOptions = New System.Windows.Forms.GroupBox
        Me.chkHBO = New System.Windows.Forms.CheckBox
        Me.udQuick = New System.Windows.Forms.NumericUpDown
        Me.chkQuick = New System.Windows.Forms.CheckBox
        Me.chkSummary = New System.Windows.Forms.CheckBox
        Me.tlpMain = New System.Windows.Forms.TableLayoutPanel
        Me.gpbSummaryOpt = New System.Windows.Forms.GroupBox
        Me.rbOpt08 = New System.Windows.Forms.RadioButton
        Me.rbOpt07 = New System.Windows.Forms.RadioButton
        Me.rbOpt06 = New System.Windows.Forms.RadioButton
        Me.rbOpt05 = New System.Windows.Forms.RadioButton
        Me.rbOpt04 = New System.Windows.Forms.RadioButton
        Me.rbOpt03 = New System.Windows.Forms.RadioButton
        Me.rbOpt02 = New System.Windows.Forms.RadioButton
        Me.rbOpt01 = New System.Windows.Forms.RadioButton
        Me.btnAutoGen = New System.Windows.Forms.Button
        Me.gpbShift.SuspendLayout()
        Me.gpbOptions.SuspendLayout()
        CType(Me.udQuick, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gpbSummaryOpt.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnOK
        '
        resources.ApplyResources(Me.btnOK, "btnOK")
        Me.btnOK.Name = "btnOK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        resources.ApplyResources(Me.btnCancel, "btnCancel")
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'dtpStartDate
        '
        resources.ApplyResources(Me.dtpStartDate, "dtpStartDate")
        Me.dtpStartDate.Name = "dtpStartDate"
        '
        'dtpEndDate
        '
        resources.ApplyResources(Me.dtpEndDate, "dtpEndDate")
        Me.dtpEndDate.Name = "dtpEndDate"
        '
        'dtpStartTime
        '
        resources.ApplyResources(Me.dtpStartTime, "dtpStartTime")
        Me.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.dtpStartTime.Name = "dtpStartTime"
        Me.dtpStartTime.ShowUpDown = True
        '
        'dtpEndTime
        '
        resources.ApplyResources(Me.dtpEndTime, "dtpEndTime")
        Me.dtpEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.dtpEndTime.Name = "dtpEndTime"
        Me.dtpEndTime.ShowUpDown = True
        '
        'lblStartCap
        '
        resources.ApplyResources(Me.lblStartCap, "lblStartCap")
        Me.lblStartCap.Name = "lblStartCap"
        '
        'lblEndCap
        '
        resources.ApplyResources(Me.lblEndCap, "lblEndCap")
        Me.lblEndCap.Name = "lblEndCap"
        '
        'gpbShift
        '
        Me.gpbShift.Controls.Add(Me.cboZone)
        Me.gpbShift.Controls.Add(Me.btnShift3)
        Me.gpbShift.Controls.Add(Me.btnShift2)
        Me.gpbShift.Controls.Add(Me.btnShift1)
        resources.ApplyResources(Me.gpbShift, "gpbShift")
        Me.gpbShift.Name = "gpbShift"
        Me.gpbShift.TabStop = False
        '
        'chkDaily
        '
        resources.ApplyResources(Me.chkDaily, "chkDaily")
        Me.chkDaily.Name = "chkDaily"
        Me.chkDaily.UseVisualStyleBackColor = True
        '
        'cboZone
        '
        Me.cboZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboZone, "cboZone")
        Me.cboZone.Name = "cboZone"
        '
        'btnShift3
        '
        resources.ApplyResources(Me.btnShift3, "btnShift3")
        Me.btnShift3.Name = "btnShift3"
        Me.btnShift3.UseVisualStyleBackColor = True
        '
        'btnShift2
        '
        resources.ApplyResources(Me.btnShift2, "btnShift2")
        Me.btnShift2.Name = "btnShift2"
        Me.btnShift2.UseVisualStyleBackColor = True
        '
        'btnShift1
        '
        resources.ApplyResources(Me.btnShift1, "btnShift1")
        Me.btnShift1.Name = "btnShift1"
        Me.btnShift1.UseVisualStyleBackColor = True
        '
        'gpbOptions
        '
        Me.gpbOptions.Controls.Add(Me.chkDaily)
        Me.gpbOptions.Controls.Add(Me.chkHBO)
        Me.gpbOptions.Controls.Add(Me.udQuick)
        Me.gpbOptions.Controls.Add(Me.chkQuick)
        Me.gpbOptions.Controls.Add(Me.chkSummary)
        resources.ApplyResources(Me.gpbOptions, "gpbOptions")
        Me.gpbOptions.Name = "gpbOptions"
        Me.gpbOptions.TabStop = False
        '
        'chkHBO
        '
        resources.ApplyResources(Me.chkHBO, "chkHBO")
        Me.chkHBO.Name = "chkHBO"
        Me.chkHBO.UseVisualStyleBackColor = True
        '
        'udQuick
        '
        resources.ApplyResources(Me.udQuick, "udQuick")
        Me.udQuick.Maximum = New Decimal(New Integer() {150, 0, 0, 0})
        Me.udQuick.Minimum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.udQuick.Name = "udQuick"
        Me.udQuick.Value = New Decimal(New Integer() {50, 0, 0, 0})
        '
        'chkQuick
        '
        resources.ApplyResources(Me.chkQuick, "chkQuick")
        Me.chkQuick.Name = "chkQuick"
        Me.chkQuick.UseVisualStyleBackColor = True
        '
        'chkSummary
        '
        resources.ApplyResources(Me.chkSummary, "chkSummary")
        Me.chkSummary.Name = "chkSummary"
        Me.chkSummary.UseVisualStyleBackColor = True
        '
        'tlpMain
        '
        resources.ApplyResources(Me.tlpMain, "tlpMain")
        Me.tlpMain.Name = "tlpMain"
        '
        'gpbSummaryOpt
        '
        Me.gpbSummaryOpt.Controls.Add(Me.rbOpt08)
        Me.gpbSummaryOpt.Controls.Add(Me.rbOpt07)
        Me.gpbSummaryOpt.Controls.Add(Me.rbOpt06)
        Me.gpbSummaryOpt.Controls.Add(Me.rbOpt05)
        Me.gpbSummaryOpt.Controls.Add(Me.rbOpt04)
        Me.gpbSummaryOpt.Controls.Add(Me.rbOpt03)
        Me.gpbSummaryOpt.Controls.Add(Me.rbOpt02)
        Me.gpbSummaryOpt.Controls.Add(Me.rbOpt01)
        resources.ApplyResources(Me.gpbSummaryOpt, "gpbSummaryOpt")
        Me.gpbSummaryOpt.Name = "gpbSummaryOpt"
        Me.gpbSummaryOpt.TabStop = False
        '
        'rbOpt08
        '
        resources.ApplyResources(Me.rbOpt08, "rbOpt08")
        Me.rbOpt08.Name = "rbOpt08"
        Me.rbOpt08.UseVisualStyleBackColor = True
        '
        'rbOpt07
        '
        resources.ApplyResources(Me.rbOpt07, "rbOpt07")
        Me.rbOpt07.Name = "rbOpt07"
        Me.rbOpt07.UseVisualStyleBackColor = True
        '
        'rbOpt06
        '
        resources.ApplyResources(Me.rbOpt06, "rbOpt06")
        Me.rbOpt06.Name = "rbOpt06"
        Me.rbOpt06.UseVisualStyleBackColor = True
        '
        'rbOpt05
        '
        resources.ApplyResources(Me.rbOpt05, "rbOpt05")
        Me.rbOpt05.Name = "rbOpt05"
        Me.rbOpt05.UseVisualStyleBackColor = True
        '
        'rbOpt04
        '
        resources.ApplyResources(Me.rbOpt04, "rbOpt04")
        Me.rbOpt04.Name = "rbOpt04"
        Me.rbOpt04.UseVisualStyleBackColor = True
        '
        'rbOpt03
        '
        resources.ApplyResources(Me.rbOpt03, "rbOpt03")
        Me.rbOpt03.Name = "rbOpt03"
        Me.rbOpt03.UseVisualStyleBackColor = True
        '
        'rbOpt02
        '
        resources.ApplyResources(Me.rbOpt02, "rbOpt02")
        Me.rbOpt02.Name = "rbOpt02"
        Me.rbOpt02.UseVisualStyleBackColor = True
        '
        'rbOpt01
        '
        resources.ApplyResources(Me.rbOpt01, "rbOpt01")
        Me.rbOpt01.Checked = True
        Me.rbOpt01.Name = "rbOpt01"
        Me.rbOpt01.TabStop = True
        Me.rbOpt01.UseVisualStyleBackColor = True
        '
        'btnAutoGen
        '
        resources.ApplyResources(Me.btnAutoGen, "btnAutoGen")
        Me.btnAutoGen.Name = "btnAutoGen"
        Me.btnAutoGen.UseVisualStyleBackColor = True
        '
        'frmCriteria
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.btnAutoGen)
        Me.Controls.Add(Me.gpbSummaryOpt)
        Me.Controls.Add(Me.tlpMain)
        Me.Controls.Add(Me.gpbOptions)
        Me.Controls.Add(Me.gpbShift)
        Me.Controls.Add(Me.lblEndCap)
        Me.Controls.Add(Me.lblStartCap)
        Me.Controls.Add(Me.dtpEndTime)
        Me.Controls.Add(Me.dtpStartTime)
        Me.Controls.Add(Me.dtpEndDate)
        Me.Controls.Add(Me.dtpStartDate)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmCriteria"
        Me.gpbShift.ResumeLayout(False)
        Me.gpbOptions.ResumeLayout(False)
        Me.gpbOptions.PerformLayout()
        CType(Me.udQuick, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gpbSummaryOpt.ResumeLayout(False)
        Me.gpbSummaryOpt.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents dtpStartDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpEndDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpStartTime As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpEndTime As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblStartCap As System.Windows.Forms.Label
    Friend WithEvents lblEndCap As System.Windows.Forms.Label
    Friend WithEvents gpbShift As System.Windows.Forms.GroupBox
    Friend WithEvents btnShift3 As System.Windows.Forms.Button
    Friend WithEvents btnShift2 As System.Windows.Forms.Button
    Friend WithEvents btnShift1 As System.Windows.Forms.Button
    Friend WithEvents gpbOptions As System.Windows.Forms.GroupBox
    Friend WithEvents chkQuick As System.Windows.Forms.CheckBox
    Friend WithEvents chkSummary As System.Windows.Forms.CheckBox
    Friend WithEvents udQuick As System.Windows.Forms.NumericUpDown
    Friend WithEvents tlpMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents cboZone As System.Windows.Forms.ComboBox
    Friend WithEvents gpbSummaryOpt As System.Windows.Forms.GroupBox
    Friend WithEvents rbOpt01 As System.Windows.Forms.RadioButton
    Friend WithEvents rbOpt08 As System.Windows.Forms.RadioButton
    Friend WithEvents rbOpt07 As System.Windows.Forms.RadioButton
    Friend WithEvents rbOpt06 As System.Windows.Forms.RadioButton
    Friend WithEvents rbOpt05 As System.Windows.Forms.RadioButton
    Friend WithEvents rbOpt04 As System.Windows.Forms.RadioButton
    Friend WithEvents rbOpt03 As System.Windows.Forms.RadioButton
    Friend WithEvents rbOpt02 As System.Windows.Forms.RadioButton
    Friend WithEvents chkHBO As System.Windows.Forms.CheckBox
    Friend WithEvents btnAutoGen As System.Windows.Forms.Button
    Friend WithEvents chkDaily As System.Windows.Forms.CheckBox
End Class
