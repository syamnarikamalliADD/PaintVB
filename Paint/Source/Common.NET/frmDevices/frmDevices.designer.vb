<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDevices
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmDevices))
        Me.tscMain = New System.Windows.Forms.ToolStripContainer
        Me.flpMain = New System.Windows.Forms.FlowLayoutPanel
        Me.bbR01 = New BingoBoard.BingoBoard
        Me.bbZone = New BingoBoard.BingoBoard
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.btnStatus = New System.Windows.Forms.ToolStripButton
        Me.tscMain.ContentPanel.SuspendLayout()
        Me.tscMain.TopToolStripPanel.SuspendLayout()
        Me.tscMain.SuspendLayout()
        Me.flpMain.SuspendLayout()
        Me.tlsMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'tscMain
        '
        Me.tscMain.BottomToolStripPanelVisible = False
        '
        'tscMain.ContentPanel
        '
        Me.tscMain.ContentPanel.Controls.Add(Me.flpMain)
        Me.tscMain.ContentPanel.Controls.Add(Me.bbZone)
        Me.tscMain.ContentPanel.Controls.Add(Me.lstStatus)
        resources.ApplyResources(Me.tscMain.ContentPanel, "tscMain.ContentPanel")
        resources.ApplyResources(Me.tscMain, "tscMain")
        Me.tscMain.LeftToolStripPanelVisible = False
        Me.tscMain.Name = "tscMain"
        Me.tscMain.RightToolStripPanelVisible = False
        '
        'tscMain.TopToolStripPanel
        '
        Me.tscMain.TopToolStripPanel.Controls.Add(Me.tlsMain)
        '
        'flpMain
        '
        resources.ApplyResources(Me.flpMain, "flpMain")
        Me.flpMain.BackColor = System.Drawing.SystemColors.ControlDarkDark
        Me.flpMain.Controls.Add(Me.bbR01)
        Me.flpMain.Name = "flpMain"
        '
        'bbR01
        '
        resources.ApplyResources(Me.bbR01, "bbR01")
        Me.bbR01.AutosizeColumns = True
        Me.bbR01.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bbR01.Columns = 1
        Me.bbR01.ColumnWidth = 150
        Me.bbR01.EnableToolTips = False
        Me.bbR01.ItemBitIndex = New String() {"-1"}
        Me.bbR01.ItemCount = 1
        Me.bbR01.ItemData = New String() {"0"}
        Me.bbR01.ItemErrorColor = System.Drawing.Color.Silver
        Me.bbR01.ItemFont = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bbR01.ItemHorizSpace = 5
        Me.bbR01.ItemOffColor = System.Drawing.Color.Red
        Me.bbR01.ItemOffText = New String() {"Item 0 OFF"}
        Me.bbR01.ItemOnColor = System.Drawing.Color.Lime
        Me.bbR01.ItemOnText = New String() {"Item 0 ON"}
        Me.bbR01.ItemToolTipText = Nothing
        Me.bbR01.ItemVertSpace = 0
        Me.bbR01.Name = "bbR01"
        Me.bbR01.TitleBackColor = System.Drawing.Color.Black
        Me.bbR01.TitleColor = System.Drawing.Color.White
        Me.bbR01.TitleFont = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bbR01.TitleText = "Title"
        Me.bbR01.TitleVisible = True
        '
        'bbZone
        '
        Me.bbZone.AutosizeColumns = True
        Me.bbZone.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bbZone.Columns = 1
        Me.bbZone.ColumnWidth = 150
        Me.bbZone.EnableToolTips = False
        resources.ApplyResources(Me.bbZone, "bbZone")
        Me.bbZone.ItemBitIndex = New String() {"-1"}
        Me.bbZone.ItemCount = 1
        Me.bbZone.ItemData = New String() {"0"}
        Me.bbZone.ItemErrorColor = System.Drawing.Color.Silver
        Me.bbZone.ItemFont = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bbZone.ItemHorizSpace = 5
        Me.bbZone.ItemOffColor = System.Drawing.Color.Red
        Me.bbZone.ItemOffText = New String() {"Item 0 OFF"}
        Me.bbZone.ItemOnColor = System.Drawing.Color.Lime
        Me.bbZone.ItemOnText = New String() {"Item 0 ON"}
        Me.bbZone.ItemToolTipText = Nothing
        Me.bbZone.ItemVertSpace = 0
        Me.bbZone.Name = "bbZone"
        Me.bbZone.TitleBackColor = System.Drawing.Color.Black
        Me.bbZone.TitleColor = System.Drawing.Color.White
        Me.bbZone.TitleFont = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bbZone.TitleText = "Title"
        Me.bbZone.TitleVisible = True
        '
        'lstStatus
        '
        Me.lstStatus.BackColor = System.Drawing.SystemColors.Info
        resources.ApplyResources(Me.lstStatus, "lstStatus")
        Me.lstStatus.Name = "lstStatus"
        '
        'tlsMain
        '
        resources.ApplyResources(Me.tlsMain, "tlsMain")
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.btnStatus})
        Me.tlsMain.MinimumSize = New System.Drawing.Size(0, 50)
        Me.tlsMain.Name = "tlsMain"
        Me.tlsMain.Stretch = True
        Me.tlsMain.TabStop = True
        '
        'btnClose
        '
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.Name = "btnClose"
        '
        'btnStatus
        '
        resources.ApplyResources(Me.btnStatus, "btnStatus")
        Me.btnStatus.Name = "btnStatus"
        '
        'frmDevices
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.tscMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Name = "frmDevices"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.tscMain.ContentPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.PerformLayout()
        Me.tscMain.ResumeLayout(False)
        Me.tscMain.PerformLayout()
        Me.flpMain.ResumeLayout(False)
        Me.tlsMain.ResumeLayout(False)
        Me.tlsMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tscMain As System.Windows.Forms.ToolStripContainer
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnClose As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnStatus As System.Windows.Forms.ToolStripButton
    Friend WithEvents lstStatus As System.Windows.Forms.ListBox
    Friend WithEvents bbZone As BingoBoard.BingoBoard
    Friend WithEvents flpMain As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents bbR01 As BingoBoard.BingoBoard
End Class
