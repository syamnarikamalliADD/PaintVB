<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEdit
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEdit))
        Me.ToolStripContainer1 = New System.Windows.Forms.ToolStripContainer
        Me.tlpMain = New System.Windows.Forms.TableLayoutPanel
        Me.lblBanner = New System.Windows.Forms.Label
        Me.tlpCbos = New System.Windows.Forms.TableLayoutPanel
        Me.cboJobStatus = New System.Windows.Forms.ComboBox
        Me.cboColor = New System.Windows.Forms.ComboBox
        Me.cboTutone = New System.Windows.Forms.ComboBox
        Me.cboOption = New System.Windows.Forms.ComboBox
        Me.txtCarrier = New System.Windows.Forms.TextBox
        Me.lblJobStatus = New System.Windows.Forms.Label
        Me.lblColor = New System.Windows.Forms.Label
        Me.lblTutone = New System.Windows.Forms.Label
        Me.lblOption = New System.Windows.Forms.Label
        Me.lblStyle = New System.Windows.Forms.Label
        Me.lblCarrier = New System.Windows.Forms.Label
        Me.lblVin = New System.Windows.Forms.Label
        Me.txtVin = New System.Windows.Forms.TextBox
        Me.cboStyle = New System.Windows.Forms.ComboBox
        Me.tlpLower = New System.Windows.Forms.TableLayoutPanel
        Me.picGhost2 = New System.Windows.Forms.PictureBox
        Me.dgSimSpeeds = New System.Windows.Forms.DataGridView
        Me.gpbRepairPanels = New System.Windows.Forms.GroupBox
        Me.tlpRepair = New System.Windows.Forms.TableLayoutPanel
        Me.chkRepair15 = New System.Windows.Forms.CheckBox
        Me.chkRepair7 = New System.Windows.Forms.CheckBox
        Me.chkRepair14 = New System.Windows.Forms.CheckBox
        Me.chkRepair6 = New System.Windows.Forms.CheckBox
        Me.chkRepair13 = New System.Windows.Forms.CheckBox
        Me.chkRepair5 = New System.Windows.Forms.CheckBox
        Me.chkRepair12 = New System.Windows.Forms.CheckBox
        Me.chkRepair4 = New System.Windows.Forms.CheckBox
        Me.chkRepair11 = New System.Windows.Forms.CheckBox
        Me.chkRepair3 = New System.Windows.Forms.CheckBox
        Me.chkRepair10 = New System.Windows.Forms.CheckBox
        Me.chkRepair2 = New System.Windows.Forms.CheckBox
        Me.chkRepair9 = New System.Windows.Forms.CheckBox
        Me.chkRepair1 = New System.Windows.Forms.CheckBox
        Me.chkRepair8 = New System.Windows.Forms.CheckBox
        Me.chkRepair0 = New System.Windows.Forms.CheckBox
        Me.picGhost1 = New System.Windows.Forms.PictureBox
        Me.chkSimConveyor = New System.Windows.Forms.CheckBox
        Me.btnAccept = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.ToolStripContainer1.ContentPanel.SuspendLayout()
        Me.ToolStripContainer1.SuspendLayout()
        Me.tlpMain.SuspendLayout()
        Me.tlpCbos.SuspendLayout()
        Me.tlpLower.SuspendLayout()
        CType(Me.picGhost2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgSimSpeeds, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gpbRepairPanels.SuspendLayout()
        Me.tlpRepair.SuspendLayout()
        CType(Me.picGhost1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ToolStripContainer1
        '
        Me.ToolStripContainer1.BottomToolStripPanelVisible = False
        '
        'ToolStripContainer1.ContentPanel
        '
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.tlpMain)
        resources.ApplyResources(Me.ToolStripContainer1.ContentPanel, "ToolStripContainer1.ContentPanel")
        resources.ApplyResources(Me.ToolStripContainer1, "ToolStripContainer1")
        Me.ToolStripContainer1.LeftToolStripPanelVisible = False
        Me.ToolStripContainer1.Name = "ToolStripContainer1"
        Me.ToolStripContainer1.RightToolStripPanelVisible = False
        Me.ToolStripContainer1.TopToolStripPanelVisible = False
        '
        'tlpMain
        '
        resources.ApplyResources(Me.tlpMain, "tlpMain")
        Me.tlpMain.Controls.Add(Me.lblBanner, 0, 0)
        Me.tlpMain.Controls.Add(Me.tlpCbos, 0, 1)
        Me.tlpMain.Controls.Add(Me.tlpLower, 0, 2)
        Me.tlpMain.Name = "tlpMain"
        '
        'lblBanner
        '
        Me.lblBanner.BackColor = System.Drawing.Color.Red
        Me.lblBanner.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.lblBanner, "lblBanner")
        Me.lblBanner.ForeColor = System.Drawing.Color.White
        Me.lblBanner.Name = "lblBanner"
        '
        'tlpCbos
        '
        resources.ApplyResources(Me.tlpCbos, "tlpCbos")
        Me.tlpCbos.Controls.Add(Me.cboJobStatus, 6, 1)
        Me.tlpCbos.Controls.Add(Me.cboColor, 5, 1)
        Me.tlpCbos.Controls.Add(Me.cboTutone, 4, 1)
        Me.tlpCbos.Controls.Add(Me.cboOption, 3, 1)
        Me.tlpCbos.Controls.Add(Me.txtCarrier, 1, 1)
        Me.tlpCbos.Controls.Add(Me.lblJobStatus, 6, 0)
        Me.tlpCbos.Controls.Add(Me.lblColor, 5, 0)
        Me.tlpCbos.Controls.Add(Me.lblTutone, 4, 0)
        Me.tlpCbos.Controls.Add(Me.lblOption, 3, 0)
        Me.tlpCbos.Controls.Add(Me.lblStyle, 2, 0)
        Me.tlpCbos.Controls.Add(Me.lblCarrier, 1, 0)
        Me.tlpCbos.Controls.Add(Me.lblVin, 0, 0)
        Me.tlpCbos.Controls.Add(Me.txtVin, 0, 1)
        Me.tlpCbos.Controls.Add(Me.cboStyle, 2, 1)
        Me.tlpCbos.Name = "tlpCbos"
        '
        'cboJobStatus
        '
        resources.ApplyResources(Me.cboJobStatus, "cboJobStatus")
        Me.cboJobStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboJobStatus.FormattingEnabled = True
        Me.cboJobStatus.Name = "cboJobStatus"
        '
        'cboColor
        '
        resources.ApplyResources(Me.cboColor, "cboColor")
        Me.cboColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboColor.FormattingEnabled = True
        Me.cboColor.Name = "cboColor"
        '
        'cboTutone
        '
        resources.ApplyResources(Me.cboTutone, "cboTutone")
        Me.cboTutone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboTutone.FormattingEnabled = True
        Me.cboTutone.Name = "cboTutone"
        '
        'cboOption
        '
        resources.ApplyResources(Me.cboOption, "cboOption")
        Me.cboOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboOption.DropDownWidth = 200
        Me.cboOption.FormattingEnabled = True
        Me.cboOption.Name = "cboOption"
        '
        'txtCarrier
        '
        resources.ApplyResources(Me.txtCarrier, "txtCarrier")
        Me.txtCarrier.Name = "txtCarrier"
        '
        'lblJobStatus
        '
        resources.ApplyResources(Me.lblJobStatus, "lblJobStatus")
        Me.lblJobStatus.Name = "lblJobStatus"
        '
        'lblColor
        '
        resources.ApplyResources(Me.lblColor, "lblColor")
        Me.lblColor.Name = "lblColor"
        '
        'lblTutone
        '
        resources.ApplyResources(Me.lblTutone, "lblTutone")
        Me.lblTutone.Name = "lblTutone"
        '
        'lblOption
        '
        resources.ApplyResources(Me.lblOption, "lblOption")
        Me.lblOption.Name = "lblOption"
        '
        'lblStyle
        '
        resources.ApplyResources(Me.lblStyle, "lblStyle")
        Me.lblStyle.Name = "lblStyle"
        '
        'lblCarrier
        '
        resources.ApplyResources(Me.lblCarrier, "lblCarrier")
        Me.lblCarrier.Name = "lblCarrier"
        '
        'lblVin
        '
        resources.ApplyResources(Me.lblVin, "lblVin")
        Me.lblVin.Name = "lblVin"
        '
        'txtVin
        '
        resources.ApplyResources(Me.txtVin, "txtVin")
        Me.txtVin.Name = "txtVin"
        '
        'cboStyle
        '
        resources.ApplyResources(Me.cboStyle, "cboStyle")
        Me.cboStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboStyle.FormattingEnabled = True
        Me.cboStyle.Name = "cboStyle"
        '
        'tlpLower
        '
        resources.ApplyResources(Me.tlpLower, "tlpLower")
        Me.tlpLower.Controls.Add(Me.picGhost2, 4, 1)
        Me.tlpLower.Controls.Add(Me.dgSimSpeeds, 0, 0)
        Me.tlpLower.Controls.Add(Me.gpbRepairPanels, 2, 0)
        Me.tlpLower.Controls.Add(Me.picGhost1, 1, 1)
        Me.tlpLower.Controls.Add(Me.chkSimConveyor, 2, 1)
        Me.tlpLower.Controls.Add(Me.btnAccept, 2, 2)
        Me.tlpLower.Controls.Add(Me.btnCancel, 3, 2)
        Me.tlpLower.Name = "tlpLower"
        '
        'picGhost2
        '
        resources.ApplyResources(Me.picGhost2, "picGhost2")
        Me.picGhost2.Image = Global.BSD.My.Resources.Resources.Ghost
        Me.picGhost2.Name = "picGhost2"
        Me.picGhost2.TabStop = False
        '
        'dgSimSpeeds
        '
        Me.dgSimSpeeds.BackgroundColor = System.Drawing.SystemColors.Control
        Me.dgSimSpeeds.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.dgSimSpeeds.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.dgSimSpeeds, "dgSimSpeeds")
        Me.dgSimSpeeds.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.dgSimSpeeds.MultiSelect = False
        Me.dgSimSpeeds.Name = "dgSimSpeeds"
        Me.tlpLower.SetRowSpan(Me.dgSimSpeeds, 3)
        Me.dgSimSpeeds.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        '
        'gpbRepairPanels
        '
        Me.tlpLower.SetColumnSpan(Me.gpbRepairPanels, 2)
        Me.gpbRepairPanels.Controls.Add(Me.tlpRepair)
        resources.ApplyResources(Me.gpbRepairPanels, "gpbRepairPanels")
        Me.gpbRepairPanels.ForeColor = System.Drawing.SystemColors.ControlText
        Me.gpbRepairPanels.Name = "gpbRepairPanels"
        Me.gpbRepairPanels.TabStop = False
        '
        'tlpRepair
        '
        resources.ApplyResources(Me.tlpRepair, "tlpRepair")
        Me.tlpRepair.Controls.Add(Me.chkRepair15, 1, 7)
        Me.tlpRepair.Controls.Add(Me.chkRepair7, 0, 7)
        Me.tlpRepair.Controls.Add(Me.chkRepair14, 1, 6)
        Me.tlpRepair.Controls.Add(Me.chkRepair6, 0, 6)
        Me.tlpRepair.Controls.Add(Me.chkRepair13, 1, 5)
        Me.tlpRepair.Controls.Add(Me.chkRepair5, 0, 5)
        Me.tlpRepair.Controls.Add(Me.chkRepair12, 1, 4)
        Me.tlpRepair.Controls.Add(Me.chkRepair4, 0, 4)
        Me.tlpRepair.Controls.Add(Me.chkRepair11, 1, 3)
        Me.tlpRepair.Controls.Add(Me.chkRepair3, 0, 3)
        Me.tlpRepair.Controls.Add(Me.chkRepair10, 1, 2)
        Me.tlpRepair.Controls.Add(Me.chkRepair2, 0, 2)
        Me.tlpRepair.Controls.Add(Me.chkRepair9, 1, 1)
        Me.tlpRepair.Controls.Add(Me.chkRepair1, 0, 1)
        Me.tlpRepair.Controls.Add(Me.chkRepair8, 1, 0)
        Me.tlpRepair.Controls.Add(Me.chkRepair0, 0, 0)
        Me.tlpRepair.Name = "tlpRepair"
        '
        'chkRepair15
        '
        resources.ApplyResources(Me.chkRepair15, "chkRepair15")
        Me.chkRepair15.Name = "chkRepair15"
        Me.chkRepair15.UseVisualStyleBackColor = True
        '
        'chkRepair7
        '
        resources.ApplyResources(Me.chkRepair7, "chkRepair7")
        Me.chkRepair7.Name = "chkRepair7"
        Me.chkRepair7.UseVisualStyleBackColor = True
        '
        'chkRepair14
        '
        resources.ApplyResources(Me.chkRepair14, "chkRepair14")
        Me.chkRepair14.Name = "chkRepair14"
        Me.chkRepair14.UseVisualStyleBackColor = True
        '
        'chkRepair6
        '
        resources.ApplyResources(Me.chkRepair6, "chkRepair6")
        Me.chkRepair6.Name = "chkRepair6"
        Me.chkRepair6.UseVisualStyleBackColor = True
        '
        'chkRepair13
        '
        resources.ApplyResources(Me.chkRepair13, "chkRepair13")
        Me.chkRepair13.Name = "chkRepair13"
        Me.chkRepair13.UseVisualStyleBackColor = True
        '
        'chkRepair5
        '
        resources.ApplyResources(Me.chkRepair5, "chkRepair5")
        Me.chkRepair5.Name = "chkRepair5"
        Me.chkRepair5.UseVisualStyleBackColor = True
        '
        'chkRepair12
        '
        resources.ApplyResources(Me.chkRepair12, "chkRepair12")
        Me.chkRepair12.Name = "chkRepair12"
        Me.chkRepair12.UseVisualStyleBackColor = True
        '
        'chkRepair4
        '
        resources.ApplyResources(Me.chkRepair4, "chkRepair4")
        Me.chkRepair4.Name = "chkRepair4"
        Me.chkRepair4.UseVisualStyleBackColor = True
        '
        'chkRepair11
        '
        resources.ApplyResources(Me.chkRepair11, "chkRepair11")
        Me.chkRepair11.Name = "chkRepair11"
        Me.chkRepair11.UseVisualStyleBackColor = True
        '
        'chkRepair3
        '
        resources.ApplyResources(Me.chkRepair3, "chkRepair3")
        Me.chkRepair3.Name = "chkRepair3"
        Me.chkRepair3.UseVisualStyleBackColor = True
        '
        'chkRepair10
        '
        resources.ApplyResources(Me.chkRepair10, "chkRepair10")
        Me.chkRepair10.Name = "chkRepair10"
        Me.chkRepair10.UseVisualStyleBackColor = True
        '
        'chkRepair2
        '
        resources.ApplyResources(Me.chkRepair2, "chkRepair2")
        Me.chkRepair2.Name = "chkRepair2"
        Me.chkRepair2.UseVisualStyleBackColor = True
        '
        'chkRepair9
        '
        resources.ApplyResources(Me.chkRepair9, "chkRepair9")
        Me.chkRepair9.Name = "chkRepair9"
        Me.chkRepair9.UseVisualStyleBackColor = True
        '
        'chkRepair1
        '
        resources.ApplyResources(Me.chkRepair1, "chkRepair1")
        Me.chkRepair1.Name = "chkRepair1"
        Me.chkRepair1.UseVisualStyleBackColor = True
        '
        'chkRepair8
        '
        resources.ApplyResources(Me.chkRepair8, "chkRepair8")
        Me.chkRepair8.Name = "chkRepair8"
        Me.chkRepair8.UseVisualStyleBackColor = True
        '
        'chkRepair0
        '
        resources.ApplyResources(Me.chkRepair0, "chkRepair0")
        Me.chkRepair0.Name = "chkRepair0"
        Me.chkRepair0.UseVisualStyleBackColor = True
        '
        'picGhost1
        '
        resources.ApplyResources(Me.picGhost1, "picGhost1")
        Me.picGhost1.Image = Global.BSD.My.Resources.Resources.Ghost
        Me.picGhost1.Name = "picGhost1"
        Me.picGhost1.TabStop = False
        '
        'chkSimConveyor
        '
        resources.ApplyResources(Me.chkSimConveyor, "chkSimConveyor")
        Me.tlpLower.SetColumnSpan(Me.chkSimConveyor, 2)
        Me.chkSimConveyor.Name = "chkSimConveyor"
        Me.chkSimConveyor.UseVisualStyleBackColor = True
        '
        'btnAccept
        '
        resources.ApplyResources(Me.btnAccept, "btnAccept")
        Me.btnAccept.Name = "btnAccept"
        Me.btnAccept.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        resources.ApplyResources(Me.btnCancel, "btnCancel")
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'frmEdit
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.ToolStripContainer1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmEdit"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.TopMost = True
        Me.ToolStripContainer1.ContentPanel.ResumeLayout(False)
        Me.ToolStripContainer1.ResumeLayout(False)
        Me.ToolStripContainer1.PerformLayout()
        Me.tlpMain.ResumeLayout(False)
        Me.tlpCbos.ResumeLayout(False)
        Me.tlpCbos.PerformLayout()
        Me.tlpLower.ResumeLayout(False)
        Me.tlpLower.PerformLayout()
        CType(Me.picGhost2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgSimSpeeds, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gpbRepairPanels.ResumeLayout(False)
        Me.tlpRepair.ResumeLayout(False)
        Me.tlpRepair.PerformLayout()
        CType(Me.picGhost1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ToolStripContainer1 As System.Windows.Forms.ToolStripContainer
    Friend WithEvents tlpMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblBanner As System.Windows.Forms.Label
    Friend WithEvents tlpCbos As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblJobStatus As System.Windows.Forms.Label
    Friend WithEvents lblColor As System.Windows.Forms.Label
    Friend WithEvents lblOption As System.Windows.Forms.Label
    Friend WithEvents lblStyle As System.Windows.Forms.Label
    Friend WithEvents lblCarrier As System.Windows.Forms.Label
    Friend WithEvents txtCarrier As System.Windows.Forms.TextBox
    Friend WithEvents cboStyle As System.Windows.Forms.ComboBox
    Friend WithEvents cboJobStatus As System.Windows.Forms.ComboBox
    Friend WithEvents cboColor As System.Windows.Forms.ComboBox
    Friend WithEvents cboOption As System.Windows.Forms.ComboBox
    Friend WithEvents tlpLower As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents dgSimSpeeds As System.Windows.Forms.DataGridView
    Friend WithEvents gpbRepairPanels As System.Windows.Forms.GroupBox
    Friend WithEvents picGhost1 As System.Windows.Forms.PictureBox
    Friend WithEvents chkSimConveyor As System.Windows.Forms.CheckBox
    Friend WithEvents picGhost2 As System.Windows.Forms.PictureBox
    Friend WithEvents tlpRepair As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents chkRepair4 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRepair11 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRepair3 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRepair10 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRepair2 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRepair9 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRepair1 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRepair8 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRepair0 As System.Windows.Forms.CheckBox
    Friend WithEvents btnAccept As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents chkRepair15 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRepair7 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRepair14 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRepair6 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRepair13 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRepair5 As System.Windows.Forms.CheckBox
    Friend WithEvents chkRepair12 As System.Windows.Forms.CheckBox
    Friend WithEvents cboTutone As System.Windows.Forms.ComboBox
    Friend WithEvents lblTutone As System.Windows.Forms.Label
    Friend WithEvents lblVin As System.Windows.Forms.Label
    Friend WithEvents txtVin As System.Windows.Forms.TextBox
End Class
