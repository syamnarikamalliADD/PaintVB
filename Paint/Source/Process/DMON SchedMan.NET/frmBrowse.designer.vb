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
        Me.tlpMain = New System.Windows.Forms.TableLayoutPanel
        Me.tlpButtons = New System.Windows.Forms.TableLayoutPanel
        Me.btnCancel = New System.Windows.Forms.Button
        Me.btnOK = New System.Windows.Forms.Button
        Me.tabMain = New System.Windows.Forms.TabControl
        Me.TabVar = New System.Windows.Forms.TabPage
        Me.tlpVar = New System.Windows.Forms.TableLayoutPanel
        Me.lblVarDetail2 = New System.Windows.Forms.Label
        Me.txtVar = New System.Windows.Forms.TextBox
        Me.btnVar = New System.Windows.Forms.Button
        Me.trvwVar = New System.Windows.Forms.TreeView
        Me.lblVarDetail1 = New System.Windows.Forms.Label
        Me.tabIO = New System.Windows.Forms.TabPage
        Me.tlpIO = New System.Windows.Forms.TableLayoutPanel
        Me.cboIOType = New System.Windows.Forms.ComboBox
        Me.lstIO = New System.Windows.Forms.ListBox
        Me.tabReg = New System.Windows.Forms.TabPage
        Me.lstReg = New System.Windows.Forms.ListBox
        Me.tabAxis = New System.Windows.Forms.TabPage
        Me.numudGroup = New System.Windows.Forms.NumericUpDown
        Me.numudAxis = New System.Windows.Forms.NumericUpDown
        Me.lblGroup = New System.Windows.Forms.Label
        Me.lblAxis = New System.Windows.Forms.Label
        Me.tlpMain.SuspendLayout()
        Me.tlpButtons.SuspendLayout()
        Me.tabMain.SuspendLayout()
        Me.TabVar.SuspendLayout()
        Me.tlpVar.SuspendLayout()
        Me.tabIO.SuspendLayout()
        Me.tlpIO.SuspendLayout()
        Me.tabReg.SuspendLayout()
        Me.tabAxis.SuspendLayout()
        CType(Me.numudGroup, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numudAxis, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tlpMain
        '
        Me.tlpMain.ColumnCount = 2
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 310.0!))
        Me.tlpMain.Controls.Add(Me.tlpButtons, 1, 1)
        Me.tlpMain.Controls.Add(Me.tabMain, 0, 0)
        Me.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpMain.Location = New System.Drawing.Point(0, 0)
        Me.tlpMain.Name = "tlpMain"
        Me.tlpMain.RowCount = 2
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36.0!))
        Me.tlpMain.Size = New System.Drawing.Size(612, 526)
        Me.tlpMain.TabIndex = 141
        '
        'tlpButtons
        '
        Me.tlpButtons.ColumnCount = 2
        Me.tlpButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.00062!))
        Me.tlpButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.00062!))
        Me.tlpButtons.Controls.Add(Me.btnCancel, 0, 0)
        Me.tlpButtons.Controls.Add(Me.btnOK, 0, 0)
        Me.tlpButtons.Location = New System.Drawing.Point(305, 493)
        Me.tlpButtons.Name = "tlpButtons"
        Me.tlpButtons.RowCount = 1
        Me.tlpButtons.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpButtons.Size = New System.Drawing.Size(301, 29)
        Me.tlpButtons.TabIndex = 142
        '
        'btnCancel
        '
        Me.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnCancel.Location = New System.Drawing.Point(153, 3)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(145, 23)
        Me.btnCancel.TabIndex = 5
        Me.btnCancel.Text = "btnCancel"
        '
        'btnOK
        '
        Me.btnOK.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnOK.Location = New System.Drawing.Point(3, 3)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(144, 23)
        Me.btnOK.TabIndex = 3
        Me.btnOK.Text = "btnOK"
        '
        'tabMain
        '
        Me.tlpMain.SetColumnSpan(Me.tabMain, 2)
        Me.tabMain.Controls.Add(Me.TabVar)
        Me.tabMain.Controls.Add(Me.tabIO)
        Me.tabMain.Controls.Add(Me.tabReg)
        Me.tabMain.Controls.Add(Me.tabAxis)
        Me.tabMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabMain.Location = New System.Drawing.Point(3, 3)
        Me.tabMain.Name = "tabMain"
        Me.tabMain.SelectedIndex = 0
        Me.tabMain.Size = New System.Drawing.Size(606, 484)
        Me.tabMain.TabIndex = 141
        '
        'TabVar
        '
        Me.TabVar.Controls.Add(Me.tlpVar)
        Me.TabVar.Location = New System.Drawing.Point(4, 22)
        Me.TabVar.Name = "TabVar"
        Me.TabVar.Padding = New System.Windows.Forms.Padding(3)
        Me.TabVar.Size = New System.Drawing.Size(598, 458)
        Me.TabVar.TabIndex = 0
        Me.TabVar.Text = "TabVar"
        Me.TabVar.UseVisualStyleBackColor = True
        '
        'tlpVar
        '
        Me.tlpVar.ColumnCount = 2
        Me.tlpVar.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpVar.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108.0!))
        Me.tlpVar.Controls.Add(Me.lblVarDetail2, 0, 3)
        Me.tlpVar.Controls.Add(Me.txtVar, 0, 0)
        Me.tlpVar.Controls.Add(Me.btnVar, 1, 0)
        Me.tlpVar.Controls.Add(Me.trvwVar, 0, 1)
        Me.tlpVar.Controls.Add(Me.lblVarDetail1, 0, 2)
        Me.tlpVar.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpVar.Location = New System.Drawing.Point(3, 3)
        Me.tlpVar.Name = "tlpVar"
        Me.tlpVar.RowCount = 4
        Me.tlpVar.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28.0!))
        Me.tlpVar.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpVar.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28.0!))
        Me.tlpVar.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28.0!))
        Me.tlpVar.Size = New System.Drawing.Size(592, 452)
        Me.tlpVar.TabIndex = 0
        '
        'lblVarDetail2
        '
        Me.lblVarDetail2.AutoSize = True
        Me.tlpVar.SetColumnSpan(Me.lblVarDetail2, 2)
        Me.lblVarDetail2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblVarDetail2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblVarDetail2.Location = New System.Drawing.Point(3, 424)
        Me.lblVarDetail2.Name = "lblVarDetail2"
        Me.lblVarDetail2.Size = New System.Drawing.Size(586, 28)
        Me.lblVarDetail2.TabIndex = 4
        Me.lblVarDetail2.Text = "lblVarDetail2"
        Me.lblVarDetail2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtVar
        '
        Me.txtVar.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtVar.Location = New System.Drawing.Point(3, 3)
        Me.txtVar.Name = "txtVar"
        Me.txtVar.Size = New System.Drawing.Size(478, 20)
        Me.txtVar.TabIndex = 0
        '
        'btnVar
        '
        Me.btnVar.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnVar.Location = New System.Drawing.Point(487, 3)
        Me.btnVar.Name = "btnVar"
        Me.btnVar.Size = New System.Drawing.Size(102, 22)
        Me.btnVar.TabIndex = 1
        Me.btnVar.Text = "Button1"
        Me.btnVar.UseVisualStyleBackColor = True
        '
        'trvwVar
        '
        Me.tlpVar.SetColumnSpan(Me.trvwVar, 2)
        Me.trvwVar.Dock = System.Windows.Forms.DockStyle.Fill
        Me.trvwVar.Location = New System.Drawing.Point(3, 31)
        Me.trvwVar.Name = "trvwVar"
        Me.trvwVar.Size = New System.Drawing.Size(586, 362)
        Me.trvwVar.TabIndex = 2
        '
        'lblVarDetail1
        '
        Me.lblVarDetail1.AutoSize = True
        Me.tlpVar.SetColumnSpan(Me.lblVarDetail1, 2)
        Me.lblVarDetail1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblVarDetail1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblVarDetail1.Location = New System.Drawing.Point(3, 396)
        Me.lblVarDetail1.Name = "lblVarDetail1"
        Me.lblVarDetail1.Size = New System.Drawing.Size(586, 28)
        Me.lblVarDetail1.TabIndex = 3
        Me.lblVarDetail1.Text = "lblVarDetail1"
        Me.lblVarDetail1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tabIO
        '
        Me.tabIO.Controls.Add(Me.tlpIO)
        Me.tabIO.Location = New System.Drawing.Point(4, 22)
        Me.tabIO.Name = "tabIO"
        Me.tabIO.Padding = New System.Windows.Forms.Padding(3)
        Me.tabIO.Size = New System.Drawing.Size(598, 458)
        Me.tabIO.TabIndex = 1
        Me.tabIO.Text = "tabIO"
        Me.tabIO.UseVisualStyleBackColor = True
        '
        'tlpIO
        '
        Me.tlpIO.ColumnCount = 2
        Me.tlpIO.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 204.0!))
        Me.tlpIO.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpIO.Controls.Add(Me.cboIOType, 0, 0)
        Me.tlpIO.Controls.Add(Me.lstIO, 1, 0)
        Me.tlpIO.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpIO.Location = New System.Drawing.Point(3, 3)
        Me.tlpIO.Name = "tlpIO"
        Me.tlpIO.RowCount = 2
        Me.tlpIO.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.tlpIO.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpIO.Size = New System.Drawing.Size(592, 452)
        Me.tlpIO.TabIndex = 1
        '
        'cboIOType
        '
        Me.cboIOType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboIOType.FormattingEnabled = True
        Me.cboIOType.Location = New System.Drawing.Point(3, 3)
        Me.cboIOType.Name = "cboIOType"
        Me.cboIOType.Size = New System.Drawing.Size(186, 21)
        Me.cboIOType.TabIndex = 1
        '
        'lstIO
        '
        Me.lstIO.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstIO.FormattingEnabled = True
        Me.lstIO.Location = New System.Drawing.Point(207, 3)
        Me.lstIO.Name = "lstIO"
        Me.tlpIO.SetRowSpan(Me.lstIO, 2)
        Me.lstIO.Size = New System.Drawing.Size(382, 446)
        Me.lstIO.TabIndex = 2
        '
        'tabReg
        '
        Me.tabReg.Controls.Add(Me.lstReg)
        Me.tabReg.Location = New System.Drawing.Point(4, 22)
        Me.tabReg.Name = "tabReg"
        Me.tabReg.Size = New System.Drawing.Size(598, 458)
        Me.tabReg.TabIndex = 2
        Me.tabReg.Text = "tabReg"
        Me.tabReg.UseVisualStyleBackColor = True
        '
        'lstReg
        '
        Me.lstReg.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstReg.FormattingEnabled = True
        Me.lstReg.Location = New System.Drawing.Point(0, 0)
        Me.lstReg.Name = "lstReg"
        Me.lstReg.Size = New System.Drawing.Size(598, 446)
        Me.lstReg.TabIndex = 0
        '
        'tabAxis
        '
        Me.tabAxis.Controls.Add(Me.numudGroup)
        Me.tabAxis.Controls.Add(Me.numudAxis)
        Me.tabAxis.Controls.Add(Me.lblGroup)
        Me.tabAxis.Controls.Add(Me.lblAxis)
        Me.tabAxis.Location = New System.Drawing.Point(4, 22)
        Me.tabAxis.Name = "tabAxis"
        Me.tabAxis.Size = New System.Drawing.Size(598, 458)
        Me.tabAxis.TabIndex = 3
        Me.tabAxis.Text = "tabAxis"
        Me.tabAxis.UseVisualStyleBackColor = True
        '
        'numudGroup
        '
        Me.numudGroup.Location = New System.Drawing.Point(30, 57)
        Me.numudGroup.Name = "numudGroup"
        Me.numudGroup.Size = New System.Drawing.Size(78, 20)
        Me.numudGroup.TabIndex = 139
        '
        'numudAxis
        '
        Me.numudAxis.Location = New System.Drawing.Point(132, 57)
        Me.numudAxis.Name = "numudAxis"
        Me.numudAxis.Size = New System.Drawing.Size(78, 20)
        Me.numudAxis.TabIndex = 142
        '
        'lblGroup
        '
        Me.lblGroup.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblGroup.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblGroup.Location = New System.Drawing.Point(30, 28)
        Me.lblGroup.Name = "lblGroup"
        Me.lblGroup.Size = New System.Drawing.Size(78, 26)
        Me.lblGroup.TabIndex = 140
        Me.lblGroup.Tag = "11"
        Me.lblGroup.Text = "lblGroup"
        Me.lblGroup.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblAxis
        '
        Me.lblAxis.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAxis.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblAxis.Location = New System.Drawing.Point(132, 28)
        Me.lblAxis.Name = "lblAxis"
        Me.lblAxis.Size = New System.Drawing.Size(78, 26)
        Me.lblAxis.TabIndex = 141
        Me.lblAxis.Tag = "11"
        Me.lblAxis.Text = "lblAxis"
        Me.lblAxis.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'frmBrowse
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(612, 526)
        Me.ControlBox = False
        Me.Controls.Add(Me.tlpMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmBrowse"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "frmBrowse"
        Me.TopMost = True
        Me.tlpMain.ResumeLayout(False)
        Me.tlpButtons.ResumeLayout(False)
        Me.tabMain.ResumeLayout(False)
        Me.TabVar.ResumeLayout(False)
        Me.tlpVar.ResumeLayout(False)
        Me.tlpVar.PerformLayout()
        Me.tabIO.ResumeLayout(False)
        Me.tlpIO.ResumeLayout(False)
        Me.tabReg.ResumeLayout(False)
        Me.tabAxis.ResumeLayout(False)
        CType(Me.numudGroup, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numudAxis, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tlpMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents tlpButtons As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents tabMain As System.Windows.Forms.TabControl
    Friend WithEvents TabVar As System.Windows.Forms.TabPage
    Friend WithEvents tabIO As System.Windows.Forms.TabPage
    Friend WithEvents tabReg As System.Windows.Forms.TabPage
    Friend WithEvents tabAxis As System.Windows.Forms.TabPage
    Friend WithEvents numudGroup As System.Windows.Forms.NumericUpDown
    Friend WithEvents numudAxis As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblGroup As System.Windows.Forms.Label
    Friend WithEvents lblAxis As System.Windows.Forms.Label
    Friend WithEvents lstReg As System.Windows.Forms.ListBox
    Friend WithEvents tlpIO As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents cboIOType As System.Windows.Forms.ComboBox
    Friend WithEvents lstIO As System.Windows.Forms.ListBox
    Friend WithEvents tlpVar As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents txtVar As System.Windows.Forms.TextBox
    Friend WithEvents btnVar As System.Windows.Forms.Button
    Friend WithEvents trvwVar As System.Windows.Forms.TreeView
    Friend WithEvents lblVarDetail2 As System.Windows.Forms.Label
    Friend WithEvents lblVarDetail1 As System.Windows.Forms.Label

End Class
