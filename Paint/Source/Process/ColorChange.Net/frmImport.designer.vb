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
        Me.components = New System.ComponentModel.Container
        Me.tlpButtons = New System.Windows.Forms.TableLayoutPanel
        Me.btnCancel = New System.Windows.Forms.Button
        Me.btnOK = New System.Windows.Forms.Button
        Me.lblLabel1 = New System.Windows.Forms.Label
        Me.lblFrom = New System.Windows.Forms.Label
        Me.chkFrom = New System.Windows.Forms.CheckedListBox
        Me.chkTo = New System.Windows.Forms.CheckedListBox
        Me.lblTo = New System.Windows.Forms.Label
        Me.dgPreview = New System.Windows.Forms.DataGridView
        Me.mnuSelect = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuSelectAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuUnselectAll = New System.Windows.Forms.ToolStripMenuItem
        Me.tlpButtons.SuspendLayout()
        CType(Me.dgPreview, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.mnuSelect.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpButtons
        '
        Me.tlpButtons.ColumnCount = 2
        Me.tlpButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.00062!))
        Me.tlpButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.00062!))
        Me.tlpButtons.Controls.Add(Me.btnCancel, 0, 0)
        Me.tlpButtons.Controls.Add(Me.btnOK, 0, 0)
        Me.tlpButtons.Location = New System.Drawing.Point(516, 366)
        Me.tlpButtons.Name = "tlpButtons"
        Me.tlpButtons.RowCount = 1
        Me.tlpButtons.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpButtons.Size = New System.Drawing.Size(229, 29)
        Me.tlpButtons.TabIndex = 0
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnCancel.Location = New System.Drawing.Point(117, 3)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(109, 23)
        Me.btnCancel.TabIndex = 5
        Me.btnCancel.Text = "btnCancel"
        '
        'btnOK
        '
        Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnOK.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnOK.Location = New System.Drawing.Point(3, 3)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(108, 23)
        Me.btnOK.TabIndex = 3
        Me.btnOK.Text = "btnOK"
        '
        'lblLabel1
        '
        Me.lblLabel1.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLabel1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblLabel1.Location = New System.Drawing.Point(14, 9)
        Me.lblLabel1.Name = "lblLabel1"
        Me.lblLabel1.Size = New System.Drawing.Size(408, 26)
        Me.lblLabel1.TabIndex = 38
        Me.lblLabel1.Tag = "11"
        Me.lblLabel1.Text = "lblLabel1"
        Me.lblLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblFrom
        '
        Me.lblFrom.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblFrom.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblFrom.Location = New System.Drawing.Point(14, 37)
        Me.lblFrom.Name = "lblFrom"
        Me.lblFrom.Size = New System.Drawing.Size(168, 26)
        Me.lblFrom.TabIndex = 42
        Me.lblFrom.Tag = "11"
        Me.lblFrom.Text = "lblFrom"
        Me.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'chkFrom
        '
        Me.chkFrom.ContextMenuStrip = Me.mnuSelect
        Me.chkFrom.FormattingEnabled = True
        Me.chkFrom.Location = New System.Drawing.Point(12, 66)
        Me.chkFrom.Name = "chkFrom"
        Me.chkFrom.Size = New System.Drawing.Size(170, 274)
        Me.chkFrom.TabIndex = 44
        '
        'chkTo
        '
        Me.chkTo.ContextMenuStrip = Me.mnuSelect
        Me.chkTo.FormattingEnabled = True
        Me.chkTo.Location = New System.Drawing.Point(188, 66)
        Me.chkTo.Name = "chkTo"
        Me.chkTo.Size = New System.Drawing.Size(186, 274)
        Me.chkTo.TabIndex = 45
        '
        'lblTo
        '
        Me.lblTo.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTo.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblTo.Location = New System.Drawing.Point(188, 35)
        Me.lblTo.Name = "lblTo"
        Me.lblTo.Size = New System.Drawing.Size(186, 26)
        Me.lblTo.TabIndex = 46
        Me.lblTo.Tag = "11"
        Me.lblTo.Text = "lblTo"
        Me.lblTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'dgPreview
        '
        Me.dgPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgPreview.Location = New System.Drawing.Point(380, 66)
        Me.dgPreview.Name = "dgPreview"
        Me.dgPreview.Size = New System.Drawing.Size(424, 273)
        Me.dgPreview.TabIndex = 47
        '
        'mnuSelect
        '
        Me.mnuSelect.Font = New System.Drawing.Font("Arial", 8.25!)
        Me.mnuSelect.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuSelectAll, Me.mnuUnselectAll})
        Me.mnuSelect.Name = "mnuSelect"
        Me.mnuSelect.Size = New System.Drawing.Size(149, 48)
        '
        'mnuSelectAll
        '
        Me.mnuSelectAll.Name = "mnuSelectAll"
        Me.mnuSelectAll.Size = New System.Drawing.Size(148, 22)
        Me.mnuSelectAll.Text = "mnuSelectAll"
        '
        'mnuUnselectAll
        '
        Me.mnuUnselectAll.Name = "mnuUnselectAll"
        Me.mnuUnselectAll.Size = New System.Drawing.Size(148, 22)
        Me.mnuUnselectAll.Text = "mnuUnselectAll"
        '
        'frmImport
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(808, 407)
        Me.ControlBox = False
        Me.Controls.Add(Me.dgPreview)
        Me.Controls.Add(Me.lblTo)
        Me.Controls.Add(Me.chkTo)
        Me.Controls.Add(Me.chkFrom)
        Me.Controls.Add(Me.lblFrom)
        Me.Controls.Add(Me.lblLabel1)
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
        CType(Me.dgPreview, System.ComponentModel.ISupportInitialize).EndInit()
        Me.mnuSelect.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tlpButtons As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents lblLabel1 As System.Windows.Forms.Label
    Friend WithEvents lblFrom As System.Windows.Forms.Label
    Friend WithEvents chkFrom As System.Windows.Forms.CheckedListBox
    Friend WithEvents chkTo As System.Windows.Forms.CheckedListBox
    Friend WithEvents lblTo As System.Windows.Forms.Label
    Friend WithEvents dgPreview As System.Windows.Forms.DataGridView
    Friend WithEvents mnuSelect As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuSelectAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuUnselectAll As System.Windows.Forms.ToolStripMenuItem

End Class
