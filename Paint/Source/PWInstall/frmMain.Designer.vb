<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class frmMain
#Region "Windows Form Designer generated code "
    <System.Diagnostics.DebuggerNonUserCode()> Public Sub New()
        MyBase.New()
        'This call is required by the Windows Form Designer.
        InitializeComponent()
    End Sub
    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> Protected Overloads Overrides Sub Dispose(ByVal Disposing As Boolean)
        If Disposing Then
            If Not components Is Nothing Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(Disposing)
    End Sub
    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer
    Public ToolTip1 As System.Windows.Forms.ToolTip
    Public WithEvents lstStatus As System.Windows.Forms.ListBox
    Public WithEvents btnNext As System.Windows.Forms.Button
    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.btnNext = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.lblLabel = New System.Windows.Forms.Label
        Me.btnSkip = New System.Windows.Forms.Button
        Me.clbActions = New System.Windows.Forms.CheckedListBox
        Me.mnuSelect = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuSelectAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuUnSelectAll = New System.Windows.Forms.ToolStripMenuItem
        Me.picFANUC = New System.Windows.Forms.PictureBox
        Me.lblPAINTworks = New System.Windows.Forms.Label
        Me.picRobot = New System.Windows.Forms.PictureBox
        Me.cboZone = New System.Windows.Forms.ComboBox
        Me.mnuSelect.SuspendLayout()
        CType(Me.picFANUC, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picRobot, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lstStatus
        '
        Me.lstStatus.BackColor = System.Drawing.SystemColors.Window
        Me.lstStatus.Cursor = System.Windows.Forms.Cursors.Default
        Me.lstStatus.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstStatus.ForeColor = System.Drawing.SystemColors.WindowText
        Me.lstStatus.ItemHeight = 14
        Me.lstStatus.Location = New System.Drawing.Point(8, 196)
        Me.lstStatus.Name = "lstStatus"
        Me.lstStatus.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lstStatus.Size = New System.Drawing.Size(508, 102)
        Me.lstStatus.TabIndex = 1
        '
        'btnNext
        '
        Me.btnNext.BackColor = System.Drawing.SystemColors.Control
        Me.btnNext.Cursor = System.Windows.Forms.Cursors.Default
        Me.btnNext.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnNext.ForeColor = System.Drawing.SystemColors.ControlText
        Me.btnNext.Location = New System.Drawing.Point(233, 462)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(79, 33)
        Me.btnNext.TabIndex = 0
        Me.btnNext.Text = "btnNext"
        Me.btnNext.UseVisualStyleBackColor = False
        '
        'btnCancel
        '
        Me.btnCancel.BackColor = System.Drawing.SystemColors.Control
        Me.btnCancel.Cursor = System.Windows.Forms.Cursors.Default
        Me.btnCancel.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText
        Me.btnCancel.Location = New System.Drawing.Point(437, 462)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.btnCancel.Size = New System.Drawing.Size(79, 33)
        Me.btnCancel.TabIndex = 3
        Me.btnCancel.Text = "btnCancel"
        Me.btnCancel.UseVisualStyleBackColor = False
        '
        'lblLabel
        '
        Me.lblLabel.BackColor = System.Drawing.Color.White
        Me.lblLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLabel.Location = New System.Drawing.Point(230, 314)
        Me.lblLabel.Name = "lblLabel"
        Me.lblLabel.Size = New System.Drawing.Size(286, 127)
        Me.lblLabel.TabIndex = 4
        Me.lblLabel.Text = "lblLabel"
        '
        'btnSkip
        '
        Me.btnSkip.BackColor = System.Drawing.SystemColors.Control
        Me.btnSkip.Cursor = System.Windows.Forms.Cursors.Default
        Me.btnSkip.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSkip.ForeColor = System.Drawing.SystemColors.ControlText
        Me.btnSkip.Location = New System.Drawing.Point(335, 462)
        Me.btnSkip.Name = "btnSkip"
        Me.btnSkip.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.btnSkip.Size = New System.Drawing.Size(79, 33)
        Me.btnSkip.TabIndex = 5
        Me.btnSkip.Text = "btnSkip"
        Me.btnSkip.UseVisualStyleBackColor = False
        Me.btnSkip.Visible = False
        '
        'clbActions
        '
        Me.clbActions.ContextMenuStrip = Me.mnuSelect
        Me.clbActions.FormattingEnabled = True
        Me.clbActions.Location = New System.Drawing.Point(8, 105)
        Me.clbActions.Name = "clbActions"
        Me.clbActions.ScrollAlwaysVisible = True
        Me.clbActions.Size = New System.Drawing.Size(659, 349)
        Me.clbActions.TabIndex = 6
        Me.clbActions.Visible = False
        '
        'mnuSelect
        '
        Me.mnuSelect.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuSelectAll, Me.mnuUnSelectAll})
        Me.mnuSelect.Name = "mnuSelect"
        Me.mnuSelect.Size = New System.Drawing.Size(148, 48)
        '
        'mnuSelectAll
        '
        Me.mnuSelectAll.Name = "mnuSelectAll"
        Me.mnuSelectAll.Size = New System.Drawing.Size(147, 22)
        Me.mnuSelectAll.Text = "mnuSelectAll"
        '
        'mnuUnSelectAll
        '
        Me.mnuUnSelectAll.Name = "mnuUnSelectAll"
        Me.mnuUnSelectAll.Size = New System.Drawing.Size(147, 22)
        Me.mnuUnSelectAll.Text = "mnuUnSelectAll"
        '
        'picFANUC
        '
        Me.picFANUC.BackColor = System.Drawing.Color.Transparent
        Me.picFANUC.Image = CType(resources.GetObject("picFANUC.Image"), System.Drawing.Image)
        Me.picFANUC.Location = New System.Drawing.Point(224, 3)
        Me.picFANUC.Name = "picFANUC"
        Me.picFANUC.Size = New System.Drawing.Size(223, 35)
        Me.picFANUC.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picFANUC.TabIndex = 7
        Me.picFANUC.TabStop = False
        '
        'lblPAINTworks
        '
        Me.lblPAINTworks.AutoSize = True
        Me.lblPAINTworks.BackColor = System.Drawing.Color.Transparent
        Me.lblPAINTworks.Font = New System.Drawing.Font("Tahoma", 30.0!, System.Drawing.FontStyle.Bold)
        Me.lblPAINTworks.ForeColor = System.Drawing.Color.FromArgb(CType(CType(186, Byte), Integer), CType(CType(12, Byte), Integer), CType(CType(47, Byte), Integer))
        Me.lblPAINTworks.Location = New System.Drawing.Point(200, 47)
        Me.lblPAINTworks.Name = "lblPAINTworks"
        Me.lblPAINTworks.Size = New System.Drawing.Size(271, 48)
        Me.lblPAINTworks.TabIndex = 8
        Me.lblPAINTworks.Text = "PAINTworks"
        '
        'picRobot
        '
        Me.picRobot.BackColor = System.Drawing.Color.Transparent
        Me.picRobot.Image = CType(resources.GetObject("picRobot.Image"), System.Drawing.Image)
        Me.picRobot.Location = New System.Drawing.Point(388, 44)
        Me.picRobot.Name = "picRobot"
        Me.picRobot.Size = New System.Drawing.Size(279, 398)
        Me.picRobot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picRobot.TabIndex = 9
        Me.picRobot.TabStop = False
        '
        'cboZone
        '
        Me.cboZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboZone.FormattingEnabled = True
        Me.cboZone.Location = New System.Drawing.Point(285, 168)
        Me.cboZone.Name = "cboZone"
        Me.cboZone.Size = New System.Drawing.Size(101, 22)
        Me.cboZone.TabIndex = 10
        Me.cboZone.Visible = False
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(679, 527)
        Me.Controls.Add(Me.clbActions)
        Me.Controls.Add(Me.cboZone)
        Me.Controls.Add(Me.lblPAINTworks)
        Me.Controls.Add(Me.picFANUC)
        Me.Controls.Add(Me.btnSkip)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnNext)
        Me.Controls.Add(Me.lstStatus)
        Me.Controls.Add(Me.lblLabel)
        Me.Controls.Add(Me.picRobot)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Location = New System.Drawing.Point(4, 23)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(687, 554)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(687, 554)
        Me.Name = "frmMain"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "PAINTworks Installation"
        Me.mnuSelect.ResumeLayout(False)
        CType(Me.picFANUC, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picRobot, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Public WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents lblLabel As System.Windows.Forms.Label
    Public WithEvents btnSkip As System.Windows.Forms.Button
    Friend WithEvents clbActions As System.Windows.Forms.CheckedListBox
    Friend WithEvents mnuSelect As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuSelectAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuUnSelectAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents picFANUC As System.Windows.Forms.PictureBox
    Friend WithEvents lblPAINTworks As System.Windows.Forms.Label
    Friend WithEvents picRobot As System.Windows.Forms.PictureBox
    Friend WithEvents cboZone As System.Windows.Forms.ComboBox
#End Region
End Class