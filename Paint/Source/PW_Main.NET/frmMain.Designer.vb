<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.tmrPLCWatchDog = New System.Windows.Forms.Timer(Me.components)
        Me.tmrCheckTile = New System.Windows.Forms.Timer(Me.components)
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnConfig = New System.Windows.Forms.ToolStripButton
        Me.btnProcess = New System.Windows.Forms.ToolStripButton
        Me.btnView = New System.Windows.Forms.ToolStripButton
        Me.btnOperate = New System.Windows.Forms.ToolStripButton
        Me.btnReports = New System.Windows.Forms.ToolStripButton
        Me.btnUtilities = New System.Windows.Forms.ToolStripButton
        Me.btnMaintenance = New System.Windows.Forms.ToolStripButton
        Me.btnHelp = New System.Windows.Forms.ToolStripButton
        Me.btnAlarms = New System.Windows.Forms.ToolStripButton
        Me.pnlToolBar = New System.Windows.Forms.Panel
        Me.pnlWindowTile = New System.Windows.Forms.Panel
        Me.optTileMode0 = New System.Windows.Forms.RadioButton
        Me.optTileMode2 = New System.Windows.Forms.RadioButton
        Me.optTileMode1 = New System.Windows.Forms.RadioButton
        Me.lblWindowTile = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.btnPrintScreen = New System.Windows.Forms.Button
        Me.lblUserName = New System.Windows.Forms.Label
        Me.btnUserLogOn = New System.Windows.Forms.Button
        Me.lblFanuc = New System.Windows.Forms.Label
        Me.tmrHideTilePanel = New System.Windows.Forms.Timer(Me.components)
        Me.tmrClock = New System.Windows.Forms.Timer(Me.components)
        Me.tmrRingBell = New System.Windows.Forms.Timer(Me.components)
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.tlsMain.SuspendLayout()
        Me.pnlToolBar.SuspendLayout()
        Me.pnlWindowTile.SuspendLayout()
        Me.SuspendLayout()
        '
        'tmrPLCWatchDog
        '
        Me.tmrPLCWatchDog.Interval = 10000
        '
        'tmrCheckTile
        '
        Me.tmrCheckTile.Interval = 1000
        '
        'tlsMain
        '
        Me.tlsMain.AutoSize = False
        Me.tlsMain.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.tlsMain.Dock = System.Windows.Forms.DockStyle.None
        Me.tlsMain.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tlsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnConfig, Me.btnProcess, Me.btnView, Me.btnOperate, Me.btnReports, Me.btnUtilities, Me.btnMaintenance, Me.btnHelp, Me.btnAlarms})
        Me.tlsMain.Location = New System.Drawing.Point(0, 0)
        Me.tlsMain.MinimumSize = New System.Drawing.Size(0, 70)
        Me.tlsMain.Name = "tlsMain"
        Me.tlsMain.Size = New System.Drawing.Size(649, 70)
        Me.tlsMain.Stretch = True
        Me.tlsMain.TabIndex = 1
        '
        'btnConfig
        '
        Me.btnConfig.AutoSize = False
        Me.btnConfig.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnConfig.Image = CType(resources.GetObject("btnConfig.Image"), System.Drawing.Image)
        Me.btnConfig.ImageTransparentColor = System.Drawing.Color.Transparent
        Me.btnConfig.Name = "btnConfig"
        Me.btnConfig.Size = New System.Drawing.Size(68, 57)
        Me.btnConfig.Tag = "C"
        Me.btnConfig.Text = "&Config"
        Me.btnConfig.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnConfig.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.btnConfig.ToolTipText = "Show Configuration Menu"
        '
        'btnProcess
        '
        Me.btnProcess.AutoSize = False
        Me.btnProcess.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnProcess.Image = CType(resources.GetObject("btnProcess.Image"), System.Drawing.Image)
        Me.btnProcess.ImageTransparentColor = System.Drawing.Color.Transparent
        Me.btnProcess.Name = "btnProcess"
        Me.btnProcess.Size = New System.Drawing.Size(68, 57)
        Me.btnProcess.Tag = "P"
        Me.btnProcess.Text = "&Process"
        Me.btnProcess.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnProcess.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.btnProcess.ToolTipText = "Show Process Menu"
        '
        'btnView
        '
        Me.btnView.AutoSize = False
        Me.btnView.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnView.Image = CType(resources.GetObject("btnView.Image"), System.Drawing.Image)
        Me.btnView.ImageTransparentColor = System.Drawing.Color.Transparent
        Me.btnView.Name = "btnView"
        Me.btnView.Size = New System.Drawing.Size(68, 57)
        Me.btnView.Tag = "V"
        Me.btnView.Text = "&View"
        Me.btnView.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.btnView.ToolTipText = "Show View Menu"
        '
        'btnOperate
        '
        Me.btnOperate.AutoSize = False
        Me.btnOperate.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnOperate.Image = CType(resources.GetObject("btnOperate.Image"), System.Drawing.Image)
        Me.btnOperate.ImageTransparentColor = System.Drawing.Color.Transparent
        Me.btnOperate.Name = "btnOperate"
        Me.btnOperate.Size = New System.Drawing.Size(68, 57)
        Me.btnOperate.Tag = "O"
        Me.btnOperate.Text = "&Operate"
        Me.btnOperate.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnOperate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.btnOperate.ToolTipText = "Show Operate Menu"
        '
        'btnReports
        '
        Me.btnReports.AutoSize = False
        Me.btnReports.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnReports.Image = CType(resources.GetObject("btnReports.Image"), System.Drawing.Image)
        Me.btnReports.ImageTransparentColor = System.Drawing.Color.Transparent
        Me.btnReports.Name = "btnReports"
        Me.btnReports.Size = New System.Drawing.Size(68, 57)
        Me.btnReports.Tag = "R"
        Me.btnReports.Text = "&Reports"
        Me.btnReports.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnReports.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.btnReports.ToolTipText = "Show Reports Menu"
        '
        'btnUtilities
        '
        Me.btnUtilities.AutoSize = False
        Me.btnUtilities.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnUtilities.Image = CType(resources.GetObject("btnUtilities.Image"), System.Drawing.Image)
        Me.btnUtilities.ImageTransparentColor = System.Drawing.Color.Transparent
        Me.btnUtilities.Name = "btnUtilities"
        Me.btnUtilities.Size = New System.Drawing.Size(68, 57)
        Me.btnUtilities.Tag = "U"
        Me.btnUtilities.Text = "&Utilities"
        Me.btnUtilities.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnUtilities.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.btnUtilities.ToolTipText = "Show Utilities Menu"
        '
        'btnMaintenance
        '
        Me.btnMaintenance.AutoSize = False
        Me.btnMaintenance.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnMaintenance.Image = CType(resources.GetObject("btnMaintenance.Image"), System.Drawing.Image)
        Me.btnMaintenance.ImageTransparentColor = System.Drawing.Color.Transparent
        Me.btnMaintenance.Name = "btnMaintenance"
        Me.btnMaintenance.Size = New System.Drawing.Size(68, 57)
        Me.btnMaintenance.Tag = "M"
        Me.btnMaintenance.Text = "&Maint"
        Me.btnMaintenance.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnMaintenance.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.btnMaintenance.ToolTipText = "Show Maintenance Menu"
        '
        'btnHelp
        '
        Me.btnHelp.AutoSize = False
        Me.btnHelp.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnHelp.Image = CType(resources.GetObject("btnHelp.Image"), System.Drawing.Image)
        Me.btnHelp.ImageTransparentColor = System.Drawing.Color.Transparent
        Me.btnHelp.Name = "btnHelp"
        Me.btnHelp.Size = New System.Drawing.Size(68, 57)
        Me.btnHelp.Tag = "H"
        Me.btnHelp.Text = "&Help"
        Me.btnHelp.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnHelp.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.btnHelp.ToolTipText = "Show Help Menu"
        '
        'btnAlarms
        '
        Me.btnAlarms.AutoSize = False
        Me.btnAlarms.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAlarms.Image = Global.PW4_Main.My.Resources.ProjectStrings.Bell
        Me.btnAlarms.ImageTransparentColor = System.Drawing.Color.Transparent
        Me.btnAlarms.Name = "btnAlarms"
        Me.btnAlarms.Size = New System.Drawing.Size(68, 57)
        Me.btnAlarms.Tag = "l"
        Me.btnAlarms.Text = "A&larms"
        Me.btnAlarms.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnAlarms.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.btnAlarms.ToolTipText = "Show Alarms Screen"
        '
        'pnlToolBar
        '
        Me.pnlToolBar.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlToolBar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlToolBar.Controls.Add(Me.pnlWindowTile)
        Me.pnlToolBar.Controls.Add(Me.lblDate)
        Me.pnlToolBar.Controls.Add(Me.btnPrintScreen)
        Me.pnlToolBar.Controls.Add(Me.lblUserName)
        Me.pnlToolBar.Controls.Add(Me.btnUserLogOn)
        Me.pnlToolBar.Controls.Add(Me.lblFanuc)
        Me.pnlToolBar.Location = New System.Drawing.Point(613, 0)
        Me.pnlToolBar.Name = "pnlToolBar"
        Me.pnlToolBar.Size = New System.Drawing.Size(410, 70)
        Me.pnlToolBar.TabIndex = 2
        '
        'pnlWindowTile
        '
        Me.pnlWindowTile.Controls.Add(Me.optTileMode0)
        Me.pnlWindowTile.Controls.Add(Me.optTileMode2)
        Me.pnlWindowTile.Controls.Add(Me.optTileMode1)
        Me.pnlWindowTile.Controls.Add(Me.lblWindowTile)
        Me.pnlWindowTile.Location = New System.Drawing.Point(149, 29)
        Me.pnlWindowTile.Name = "pnlWindowTile"
        Me.pnlWindowTile.Size = New System.Drawing.Size(216, 36)
        Me.pnlWindowTile.TabIndex = 4
        Me.pnlWindowTile.Visible = False
        '
        'optTileMode0
        '
        Me.optTileMode0.AutoSize = True
        Me.optTileMode0.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.optTileMode0.Location = New System.Drawing.Point(143, 15)
        Me.optTileMode0.Name = "optTileMode0"
        Me.optTileMode0.Size = New System.Drawing.Size(68, 18)
        Me.optTileMode0.TabIndex = 3
        Me.optTileMode0.TabStop = True
        Me.optTileMode0.Text = "Cascade"
        Me.optTileMode0.UseVisualStyleBackColor = True
        '
        'optTileMode2
        '
        Me.optTileMode2.AutoSize = True
        Me.optTileMode2.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.optTileMode2.Location = New System.Drawing.Point(75, 15)
        Me.optTileMode2.Name = "optTileMode2"
        Me.optTileMode2.Size = New System.Drawing.Size(61, 18)
        Me.optTileMode2.TabIndex = 2
        Me.optTileMode2.TabStop = True
        Me.optTileMode2.Text = "Vertical"
        Me.optTileMode2.UseVisualStyleBackColor = True
        '
        'optTileMode1
        '
        Me.optTileMode1.AutoSize = True
        Me.optTileMode1.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.optTileMode1.Location = New System.Drawing.Point(3, 15)
        Me.optTileMode1.Name = "optTileMode1"
        Me.optTileMode1.Size = New System.Drawing.Size(73, 18)
        Me.optTileMode1.TabIndex = 1
        Me.optTileMode1.TabStop = True
        Me.optTileMode1.Text = "Horizontal"
        Me.optTileMode1.UseVisualStyleBackColor = True
        '
        'lblWindowTile
        '
        Me.lblWindowTile.AutoSize = True
        Me.lblWindowTile.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblWindowTile.Location = New System.Drawing.Point(72, 0)
        Me.lblWindowTile.Name = "lblWindowTile"
        Me.lblWindowTile.Size = New System.Drawing.Size(66, 14)
        Me.lblWindowTile.TabIndex = 0
        Me.lblWindowTile.Text = "Window Tile"
        Me.lblWindowTile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblDate
        '
        Me.lblDate.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDate.Location = New System.Drawing.Point(151, 29)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(214, 33)
        Me.lblDate.TabIndex = 5
        Me.lblDate.Text = "18 March 2009, 16:25"
        Me.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnPrintScreen
        '
        Me.btnPrintScreen.BackgroundImage = Global.PW4_Main.My.Resources.ProjectStrings.PRINT
        Me.btnPrintScreen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnPrintScreen.Location = New System.Drawing.Point(371, 4)
        Me.btnPrintScreen.Name = "btnPrintScreen"
        Me.btnPrintScreen.Size = New System.Drawing.Size(33, 31)
        Me.btnPrintScreen.TabIndex = 3
        Me.btnPrintScreen.UseVisualStyleBackColor = True
        '
        'lblUserName
        '
        Me.lblUserName.BackColor = System.Drawing.SystemColors.ControlText
        Me.lblUserName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblUserName.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUserName.ForeColor = System.Drawing.SystemColors.ControlLightLight
        Me.lblUserName.Location = New System.Drawing.Point(3, 5)
        Me.lblUserName.Name = "lblUserName"
        Me.lblUserName.Size = New System.Drawing.Size(138, 20)
        Me.lblUserName.TabIndex = 2
        Me.lblUserName.Text = "User: None"
        Me.lblUserName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnUserLogOn
        '
        Me.btnUserLogOn.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnUserLogOn.Location = New System.Drawing.Point(3, 29)
        Me.btnUserLogOn.Name = "btnUserLogOn"
        Me.btnUserLogOn.Size = New System.Drawing.Size(138, 36)
        Me.btnUserLogOn.TabIndex = 1
        Me.btnUserLogOn.Text = "Log On"
        Me.btnUserLogOn.UseVisualStyleBackColor = True
        '
        'lblFanuc
        '
        Me.lblFanuc.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(221, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblFanuc.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblFanuc.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblFanuc.ForeColor = System.Drawing.Color.FromArgb(CType(CType(186, Byte), Integer), CType(CType(12, Byte), Integer), CType(CType(47, Byte), Integer))
        Me.lblFanuc.Location = New System.Drawing.Point(147, 5)
        Me.lblFanuc.Name = "lblFanuc"
        Me.lblFanuc.Size = New System.Drawing.Size(218, 20)
        Me.lblFanuc.TabIndex = 0
        Me.lblFanuc.Text = "FANUC America PAINTworks"
        Me.lblFanuc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'tmrHideTilePanel
        '
        Me.tmrHideTilePanel.Interval = 3000
        '
        'tmrClock
        '
        Me.tmrClock.Interval = 1000
        '
        'tmrRingBell
        '
        Me.tmrRingBell.Interval = 500
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1024, 266)
        Me.ControlBox = False
        Me.Controls.Add(Me.pnlToolBar)
        Me.Controls.Add(Me.tlsMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Name = "frmMain"
        Me.Text = "PW4_Main"
        Me.TransparencyKey = System.Drawing.Color.IndianRed
        Me.tlsMain.ResumeLayout(False)
        Me.tlsMain.PerformLayout()
        Me.pnlToolBar.ResumeLayout(False)
        Me.pnlWindowTile.ResumeLayout(False)
        Me.pnlWindowTile.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tmrPLCWatchDog As System.Windows.Forms.Timer
    Friend WithEvents tmrCheckTile As System.Windows.Forms.Timer
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnConfig As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnProcess As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnView As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnOperate As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnReports As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnUtilities As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnMaintenance As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnHelp As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnAlarms As System.Windows.Forms.ToolStripButton
    Friend WithEvents pnlToolBar As System.Windows.Forms.Panel
    Friend WithEvents lblFanuc As System.Windows.Forms.Label
    Friend WithEvents lblUserName As System.Windows.Forms.Label
    Friend WithEvents btnUserLogOn As System.Windows.Forms.Button
    Friend WithEvents btnPrintScreen As System.Windows.Forms.Button
    Friend WithEvents pnlWindowTile As System.Windows.Forms.Panel
    Friend WithEvents optTileMode1 As System.Windows.Forms.RadioButton
    Friend WithEvents lblWindowTile As System.Windows.Forms.Label
    Friend WithEvents optTileMode0 As System.Windows.Forms.RadioButton
    Friend WithEvents optTileMode2 As System.Windows.Forms.RadioButton
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents tmrHideTilePanel As System.Windows.Forms.Timer
    Friend WithEvents tmrClock As System.Windows.Forms.Timer
    Friend WithEvents tmrRingBell As System.Windows.Forms.Timer
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip

End Class
