<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMiniGrid
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMiniGrid))
        Me.dgvMini = New System.Windows.Forms.DataGridView
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.btnSave = New System.Windows.Forms.ToolStripButton
        Me.btnUpdateLog = New System.Windows.Forms.ToolStripButton
        Me.btnHelp = New System.Windows.Forms.ToolStripButton
        Me.mnuResetStatus = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuResetItemStatus = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuResetData = New System.Windows.Forms.ToolStripMenuItem
        Me.lblUpdateMessage = New System.Windows.Forms.Label
        Me.tmrUpdateLog = New System.Windows.Forms.Timer(Me.components)
        CType(Me.dgvMini, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tlsMain.SuspendLayout()
        Me.mnuResetStatus.SuspendLayout()
        Me.SuspendLayout()
        '
        'dgvMini
        '
        Me.dgvMini.AllowUserToAddRows = False
        Me.dgvMini.AllowUserToDeleteRows = False
        Me.dgvMini.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.dgvMini.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.dgvMini.BackgroundColor = System.Drawing.SystemColors.Control
        Me.dgvMini.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.dgvMini.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvMini.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.dgvMini.Location = New System.Drawing.Point(12, 53)
        Me.dgvMini.Name = "dgvMini"
        Me.dgvMini.ReadOnly = True
        Me.dgvMini.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders
        Me.dgvMini.Size = New System.Drawing.Size(585, 404)
        Me.dgvMini.TabIndex = 0
        '
        'tlsMain
        '
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.btnSave, Me.btnUpdateLog, Me.btnHelp})
        Me.tlsMain.Location = New System.Drawing.Point(0, 0)
        Me.tlsMain.MinimumSize = New System.Drawing.Size(0, 50)
        Me.tlsMain.Name = "tlsMain"
        Me.tlsMain.Size = New System.Drawing.Size(955, 50)
        Me.tlsMain.Stretch = True
        Me.tlsMain.TabIndex = 3
        Me.tlsMain.TabStop = True
        '
        'btnClose
        '
        Me.btnClose.AutoSize = False
        Me.btnClose.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnClose.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(70, 50)
        Me.btnClose.Text = "Close"
        Me.btnClose.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'btnSave
        '
        Me.btnSave.AutoSize = False
        Me.btnSave.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(70, 50)
        Me.btnSave.Text = "Save"
        Me.btnSave.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'btnUpdateLog
        '
        Me.btnUpdateLog.AutoSize = False
        Me.btnUpdateLog.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnUpdateLog.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnUpdateLog.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnUpdateLog.Name = "btnUpdateLog"
        Me.btnUpdateLog.Size = New System.Drawing.Size(78, 50)
        Me.btnUpdateLog.Text = "Update Log"
        Me.btnUpdateLog.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnUpdateLog.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'btnHelp
        '
        Me.btnHelp.AutoSize = False
        Me.btnHelp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnHelp.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnHelp.Image = CType(resources.GetObject("btnHelp.Image"), System.Drawing.Image)
        Me.btnHelp.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnHelp.Name = "btnHelp"
        Me.btnHelp.Size = New System.Drawing.Size(78, 50)
        Me.btnHelp.Text = "Help"
        Me.btnHelp.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnHelp.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'mnuResetStatus
        '
        Me.mnuResetStatus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuResetItemStatus, Me.mnuResetData})
        Me.mnuResetStatus.Name = "mnuResetStatus"
        Me.mnuResetStatus.Size = New System.Drawing.Size(176, 48)
        Me.mnuResetStatus.Text = "mnuResetStatus"
        '
        'mnuResetItemStatus
        '
        Me.mnuResetItemStatus.Name = "mnuResetItemStatus"
        Me.mnuResetItemStatus.Size = New System.Drawing.Size(175, 22)
        Me.mnuResetItemStatus.Text = "mnuResetItemStatus"
        '
        'mnuResetData
        '
        Me.mnuResetData.Name = "mnuResetData"
        Me.mnuResetData.Size = New System.Drawing.Size(175, 22)
        Me.mnuResetData.Text = "mnuResetData"
        '
        'lblUpdateMessage
        '
        Me.lblUpdateMessage.BackColor = System.Drawing.Color.Transparent
        Me.lblUpdateMessage.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUpdateMessage.Location = New System.Drawing.Point(4, 53)
        Me.lblUpdateMessage.Name = "lblUpdateMessage"
        Me.lblUpdateMessage.Size = New System.Drawing.Size(928, 36)
        Me.lblUpdateMessage.TabIndex = 4
        Me.lblUpdateMessage.Text = "lblUpdateMessage"
        Me.lblUpdateMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblUpdateMessage.Visible = False
        '
        'tmrUpdateLog
        '
        Me.tmrUpdateLog.Interval = 500
        '
        'frmMiniGrid
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(955, 517)
        Me.Controls.Add(Me.lblUpdateMessage)
        Me.Controls.Add(Me.tlsMain)
        Me.Controls.Add(Me.dgvMini)
        Me.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMiniGrid"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.Text = "Data"
        CType(Me.dgvMini, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tlsMain.ResumeLayout(False)
        Me.tlsMain.PerformLayout()
        Me.mnuResetStatus.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents dgvMini As System.Windows.Forms.DataGridView
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnClose As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnSave As System.Windows.Forms.ToolStripButton
    Friend WithEvents mnuResetStatus As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuResetItemStatus As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnUpdateLog As System.Windows.Forms.ToolStripButton
    Friend WithEvents lblUpdateMessage As System.Windows.Forms.Label
    Friend WithEvents tmrUpdateLog As System.Windows.Forms.Timer
    Friend WithEvents btnHelp As System.Windows.Forms.ToolStripButton
    Friend WithEvents mnuResetData As System.Windows.Forms.ToolStripMenuItem
End Class
