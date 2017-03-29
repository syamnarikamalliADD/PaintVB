<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmImport
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
        Me.tlpButtons = New System.Windows.Forms.TableLayoutPanel
        Me.btnCancel = New System.Windows.Forms.Button
        Me.btnOK = New System.Windows.Forms.Button
        Me.cboItems = New System.Windows.Forms.ComboBox
        Me.lblLabel3 = New System.Windows.Forms.Label
        Me.numudItem = New System.Windows.Forms.NumericUpDown
        Me.lblLabel1 = New System.Windows.Forms.Label
        Me.lblSource = New System.Windows.Forms.Label
        Me.lblZoneLbl = New System.Windows.Forms.Label
        Me.lblDeviceLbl = New System.Windows.Forms.Label
        Me.lblZone = New System.Windows.Forms.Label
        Me.lblDevice = New System.Windows.Forms.Label
        Me.lblLabel2 = New System.Windows.Forms.Label
        Me.dgTmp = New System.Windows.Forms.DataGridView
        Me.tlpButtons.SuspendLayout()
        CType(Me.numudItem, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgTmp, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tlpButtons
        '
        Me.tlpButtons.ColumnCount = 2
        Me.tlpButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.00062!))
        Me.tlpButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.00062!))
        Me.tlpButtons.Controls.Add(Me.btnCancel, 0, 0)
        Me.tlpButtons.Controls.Add(Me.btnOK, 0, 0)
        Me.tlpButtons.Location = New System.Drawing.Point(327, 383)
        Me.tlpButtons.Name = "tlpButtons"
        Me.tlpButtons.RowCount = 1
        Me.tlpButtons.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpButtons.Size = New System.Drawing.Size(301, 29)
        Me.tlpButtons.TabIndex = 0
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnCancel.Location = New System.Drawing.Point(153, 3)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(145, 23)
        Me.btnCancel.TabIndex = 5
        Me.btnCancel.Text = "btnCancel"
        '
        'btnOK
        '
        Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnOK.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnOK.Location = New System.Drawing.Point(3, 3)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(144, 23)
        Me.btnOK.TabIndex = 3
        Me.btnOK.Text = "btnOK"
        '
        'cboItems
        '
        Me.cboItems.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboItems.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cboItems.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboItems.ItemHeight = 18
        Me.cboItems.Location = New System.Drawing.Point(17, 145)
        Me.cboItems.Name = "cboItems"
        Me.cboItems.Size = New System.Drawing.Size(188, 26)
        Me.cboItems.TabIndex = 133
        '
        'lblLabel3
        '
        Me.lblLabel3.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLabel3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblLabel3.Location = New System.Drawing.Point(226, 146)
        Me.lblLabel3.Name = "lblLabel3"
        Me.lblLabel3.Size = New System.Drawing.Size(185, 25)
        Me.lblLabel3.TabIndex = 134
        Me.lblLabel3.Tag = "11"
        Me.lblLabel3.Text = "lblLabel3"
        Me.lblLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'numudItem
        '
        Me.numudItem.Location = New System.Drawing.Point(419, 148)
        Me.numudItem.Name = "numudItem"
        Me.numudItem.Size = New System.Drawing.Size(55, 20)
        Me.numudItem.TabIndex = 135
        '
        'lblLabel1
        '
        Me.lblLabel1.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLabel1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblLabel1.Location = New System.Drawing.Point(14, 9)
        Me.lblLabel1.Name = "lblLabel1"
        Me.lblLabel1.Size = New System.Drawing.Size(546, 26)
        Me.lblLabel1.TabIndex = 38
        Me.lblLabel1.Tag = "11"
        Me.lblLabel1.Text = "lblLabel1"
        Me.lblLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblSource
        '
        Me.lblSource.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSource.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblSource.Location = New System.Drawing.Point(14, 37)
        Me.lblSource.Name = "lblSource"
        Me.lblSource.Size = New System.Drawing.Size(546, 26)
        Me.lblSource.TabIndex = 42
        Me.lblSource.Tag = "11"
        Me.lblSource.Text = "lblSource"
        Me.lblSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblZoneLbl
        '
        Me.lblZoneLbl.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblZoneLbl.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblZoneLbl.Location = New System.Drawing.Point(14, 65)
        Me.lblZoneLbl.Name = "lblZoneLbl"
        Me.lblZoneLbl.Size = New System.Drawing.Size(144, 19)
        Me.lblZoneLbl.TabIndex = 36
        Me.lblZoneLbl.Tag = "11"
        Me.lblZoneLbl.Text = "lblZoneLbl"
        Me.lblZoneLbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblDeviceLbl
        '
        Me.lblDeviceLbl.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDeviceLbl.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblDeviceLbl.Location = New System.Drawing.Point(14, 86)
        Me.lblDeviceLbl.Name = "lblDeviceLbl"
        Me.lblDeviceLbl.Size = New System.Drawing.Size(144, 19)
        Me.lblDeviceLbl.TabIndex = 37
        Me.lblDeviceLbl.Tag = "11"
        Me.lblDeviceLbl.Text = "lblDeviceLbl"
        Me.lblDeviceLbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblZone
        '
        Me.lblZone.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblZone.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblZone.Location = New System.Drawing.Point(164, 65)
        Me.lblZone.Name = "lblZone"
        Me.lblZone.Size = New System.Drawing.Size(320, 19)
        Me.lblZone.TabIndex = 39
        Me.lblZone.Tag = "11"
        Me.lblZone.Text = "lblZone"
        Me.lblZone.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblDevice
        '
        Me.lblDevice.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDevice.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblDevice.Location = New System.Drawing.Point(164, 86)
        Me.lblDevice.Name = "lblDevice"
        Me.lblDevice.Size = New System.Drawing.Size(320, 19)
        Me.lblDevice.TabIndex = 40
        Me.lblDevice.Tag = "11"
        Me.lblDevice.Text = "lblDevice"
        Me.lblDevice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblLabel2
        '
        Me.lblLabel2.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLabel2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblLabel2.Location = New System.Drawing.Point(4, 116)
        Me.lblLabel2.Name = "lblLabel2"
        Me.lblLabel2.Size = New System.Drawing.Size(572, 25)
        Me.lblLabel2.TabIndex = 41
        Me.lblLabel2.Tag = "11"
        Me.lblLabel2.Text = "lblLabel2"
        Me.lblLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'dgTmp
        '
        Me.dgTmp.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgTmp.Location = New System.Drawing.Point(7, 177)
        Me.dgTmp.Name = "dgTmp"
        Me.dgTmp.Size = New System.Drawing.Size(621, 200)
        Me.dgTmp.TabIndex = 136
        '
        'frmImport
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(640, 458)
        Me.ControlBox = False
        Me.Controls.Add(Me.dgTmp)
        Me.Controls.Add(Me.numudItem)
        Me.Controls.Add(Me.lblLabel3)
        Me.Controls.Add(Me.cboItems)
        Me.Controls.Add(Me.lblSource)
        Me.Controls.Add(Me.lblLabel2)
        Me.Controls.Add(Me.lblDevice)
        Me.Controls.Add(Me.lblZone)
        Me.Controls.Add(Me.lblLabel1)
        Me.Controls.Add(Me.lblDeviceLbl)
        Me.Controls.Add(Me.lblZoneLbl)
        Me.Controls.Add(Me.tlpButtons)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmImport"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "frmImport"
        Me.TopMost = True
        Me.tlpButtons.ResumeLayout(False)
        CType(Me.numudItem, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgTmp, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tlpButtons As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents cboItems As System.Windows.Forms.ComboBox
    Friend WithEvents lblLabel3 As System.Windows.Forms.Label
    Friend WithEvents numudItem As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblLabel1 As System.Windows.Forms.Label
    Friend WithEvents lblSource As System.Windows.Forms.Label
    Friend WithEvents lblZoneLbl As System.Windows.Forms.Label
    Friend WithEvents lblDeviceLbl As System.Windows.Forms.Label
    Friend WithEvents lblZone As System.Windows.Forms.Label
    Friend WithEvents lblDevice As System.Windows.Forms.Label
    Friend WithEvents lblLabel2 As System.Windows.Forms.Label
    Friend WithEvents dgTmp As System.Windows.Forms.DataGridView

End Class
