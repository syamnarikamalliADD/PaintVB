<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEstatTest
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEstatTest))
        Me.gpbRefData = New System.Windows.Forms.GroupBox
        Me.dgvRefData = New System.Windows.Forms.DataGridView
        Me.gpbTestData = New System.Windows.Forms.GroupBox
        Me.dgvTestData = New System.Windows.Forms.DataGridView
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.btnRefresh = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
        Me.btnStart = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator
        Me.lblStatus = New System.Windows.Forms.Label
        Me.tmrPoll = New System.Windows.Forms.Timer(Me.components)
        Me.gpbRefData.SuspendLayout()
        CType(Me.dgvRefData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gpbTestData.SuspendLayout()
        CType(Me.dgvTestData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tlsMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'gpbRefData
        '
        Me.gpbRefData.Controls.Add(Me.dgvRefData)
        Me.gpbRefData.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gpbRefData.Location = New System.Drawing.Point(12, 93)
        Me.gpbRefData.Name = "gpbRefData"
        Me.gpbRefData.Size = New System.Drawing.Size(420, 420)
        Me.gpbRefData.TabIndex = 0
        Me.gpbRefData.TabStop = False
        Me.gpbRefData.Text = "Reference Data"
        '
        'dgvRefData
        '
        Me.dgvRefData.AllowUserToAddRows = False
        Me.dgvRefData.AllowUserToDeleteRows = False
        Me.dgvRefData.AllowUserToResizeColumns = False
        Me.dgvRefData.AllowUserToResizeRows = False
        Me.dgvRefData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvRefData.Location = New System.Drawing.Point(6, 19)
        Me.dgvRefData.MultiSelect = False
        Me.dgvRefData.Name = "dgvRefData"
        Me.dgvRefData.ReadOnly = True
        Me.dgvRefData.RowHeadersVisible = False
        Me.dgvRefData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvRefData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.dgvRefData.Size = New System.Drawing.Size(408, 395)
        Me.dgvRefData.TabIndex = 0
        '
        'gpbTestData
        '
        Me.gpbTestData.Controls.Add(Me.dgvTestData)
        Me.gpbTestData.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gpbTestData.Location = New System.Drawing.Point(449, 93)
        Me.gpbTestData.Name = "gpbTestData"
        Me.gpbTestData.Size = New System.Drawing.Size(536, 420)
        Me.gpbTestData.TabIndex = 1
        Me.gpbTestData.TabStop = False
        Me.gpbTestData.Text = "Test Data"
        '
        'dgvTestData
        '
        Me.dgvTestData.AllowUserToAddRows = False
        Me.dgvTestData.AllowUserToDeleteRows = False
        Me.dgvTestData.AllowUserToResizeColumns = False
        Me.dgvTestData.AllowUserToResizeRows = False
        Me.dgvTestData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvTestData.Location = New System.Drawing.Point(6, 19)
        Me.dgvTestData.MultiSelect = False
        Me.dgvTestData.Name = "dgvTestData"
        Me.dgvTestData.ReadOnly = True
        Me.dgvTestData.RowHeadersVisible = False
        Me.dgvTestData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvTestData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.dgvTestData.Size = New System.Drawing.Size(524, 395)
        Me.dgvTestData.TabIndex = 1
        '
        'tlsMain
        '
        Me.tlsMain.AutoSize = False
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.ToolStripSeparator1, Me.btnRefresh, Me.ToolStripSeparator2, Me.btnStart, Me.ToolStripSeparator3})
        Me.tlsMain.Location = New System.Drawing.Point(0, 0)
        Me.tlsMain.Name = "tlsMain"
        Me.tlsMain.Size = New System.Drawing.Size(994, 62)
        Me.tlsMain.Stretch = True
        Me.tlsMain.TabIndex = 2
        Me.tlsMain.Text = "ToolStrip1"
        '
        'btnClose
        '
        Me.btnClose.AutoSize = False
        Me.btnClose.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnClose.Image = CType(resources.GetObject("btnClose.Image"), System.Drawing.Image)
        Me.btnClose.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(44, 59)
        Me.btnClose.Text = "Close"
        Me.btnClose.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 62)
        '
        'btnRefresh
        '
        Me.btnRefresh.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRefresh.Image = CType(resources.GetObject("btnRefresh.Image"), System.Drawing.Image)
        Me.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(55, 59)
        Me.btnRefresh.Text = "Refresh"
        Me.btnRefresh.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.btnRefresh.ToolTipText = "Refresh Data"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 62)
        '
        'btnStart
        '
        Me.btnStart.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnStart.Image = CType(resources.GetObject("btnStart.Image"), System.Drawing.Image)
        Me.btnStart.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(62, 59)
        Me.btnStart.Text = "Start Test"
        Me.btnStart.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnStart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.btnStart.ToolTipText = "Start Test"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 62)
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblStatus.Location = New System.Drawing.Point(15, 524)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(59, 16)
        Me.lblStatus.TabIndex = 3
        Me.lblStatus.Text = "lblStatus"
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tmrPoll
        '
        Me.tmrPoll.Interval = 1000
        '
        'frmEstatTest
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(994, 546)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.tlsMain)
        Me.Controls.Add(Me.gpbTestData)
        Me.Controls.Add(Me.gpbRefData)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmEstatTest"
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Electrostatic Cable Test"
        Me.TopMost = True
        Me.gpbRefData.ResumeLayout(False)
        CType(Me.dgvRefData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gpbTestData.ResumeLayout(False)
        CType(Me.dgvTestData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tlsMain.ResumeLayout(False)
        Me.tlsMain.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents gpbRefData As System.Windows.Forms.GroupBox
    Friend WithEvents gpbTestData As System.Windows.Forms.GroupBox
    Friend WithEvents dgvRefData As System.Windows.Forms.DataGridView
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnClose As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnRefresh As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnStart As System.Windows.Forms.ToolStripButton
    Friend WithEvents dgvTestData As System.Windows.Forms.DataGridView
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents tmrPoll As System.Windows.Forms.Timer
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
End Class
