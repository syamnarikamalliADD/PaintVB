<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> <System.Runtime.InteropServices.ComVisible(False)> _
Partial Class frmMain
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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.tscMain = New System.Windows.Forms.ToolStripContainer
        Me.stsStatus = New System.Windows.Forms.StatusStrip
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel
        Me.tspProgress = New System.Windows.Forms.ToolStripProgressBar
        Me.lblSpacer = New System.Windows.Forms.ToolStripStatusLabel
        Me.btnFunction = New System.Windows.Forms.ToolStripDropDownButton
        Me.mnuLogin = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLogOut = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuRemote = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLocal = New System.Windows.Forms.ToolStripMenuItem
        Me.PnlMain = New System.Windows.Forms.Panel
        Me.tabMain = New System.Windows.Forms.TabControl
        Me.tabDBSQL = New System.Windows.Forms.TabPage
        Me.spltDBSQL = New System.Windows.Forms.SplitContainer
        Me.tlpDBSQL = New System.Windows.Forms.TableLayoutPanel
        Me.lstDBSQL = New System.Windows.Forms.ListBox
        Me.mnuListMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuSelectAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuUnselectAll = New System.Windows.Forms.ToolStripMenuItem
        Me.lblDBSQL = New System.Windows.Forms.Label
        Me.pnlDBSQL = New System.Windows.Forms.Panel
        Me.btnExportToCSVZIPDBSql = New System.Windows.Forms.Button
        Me.btnExportToCSVDBSql = New System.Windows.Forms.Button
        Me.btnBackupToZipDBSql = New System.Windows.Forms.Button
        Me.btnBackupToFolderDBSql = New System.Windows.Forms.Button
        Me.tabDBAccess = New System.Windows.Forms.TabPage
        Me.spltDBAccess = New System.Windows.Forms.SplitContainer
        Me.tlpDBAccess = New System.Windows.Forms.TableLayoutPanel
        Me.lblDBAccess = New System.Windows.Forms.Label
        Me.lstDBAccess = New System.Windows.Forms.ListBox
        Me.pnlDBAccess = New System.Windows.Forms.Panel
        Me.btnBackupToZipDBAccess = New System.Windows.Forms.Button
        Me.btnBackupToFolderDBAccess = New System.Windows.Forms.Button
        Me.tabDBXML = New System.Windows.Forms.TabPage
        Me.spltDBXML = New System.Windows.Forms.SplitContainer
        Me.tlpDBXML = New System.Windows.Forms.TableLayoutPanel
        Me.lstDBXML = New System.Windows.Forms.ListBox
        Me.lblDBXML = New System.Windows.Forms.Label
        Me.pnlDBXML = New System.Windows.Forms.Panel
        Me.btnBackupToZipDBXML = New System.Windows.Forms.Button
        Me.btnBackupToFolderDBXML = New System.Windows.Forms.Button
        Me.tabImage = New System.Windows.Forms.TabPage
        Me.spltImage = New System.Windows.Forms.SplitContainer
        Me.tlpImage = New System.Windows.Forms.TableLayoutPanel
        Me.lstImage = New System.Windows.Forms.ListBox
        Me.lblImage = New System.Windows.Forms.Label
        Me.pnlImage = New System.Windows.Forms.Panel
        Me.btnBackupToZipImage = New System.Windows.Forms.Button
        Me.btnBackupToFolderImage = New System.Windows.Forms.Button
        Me.tabNotepad = New System.Windows.Forms.TabPage
        Me.spltNotepad = New System.Windows.Forms.SplitContainer
        Me.tlpNotepad = New System.Windows.Forms.TableLayoutPanel
        Me.lblNotepad = New System.Windows.Forms.Label
        Me.lstNotepad = New System.Windows.Forms.ListBox
        Me.pnlNotepad = New System.Windows.Forms.Panel
        Me.btnBackupToZipNotepad = New System.Windows.Forms.Button
        Me.btnBackupToFolderNotepad = New System.Windows.Forms.Button
        Me.tabRobots = New System.Windows.Forms.TabPage
        Me.spltRobots = New System.Windows.Forms.SplitContainer
        Me.tlpRobotFolders = New System.Windows.Forms.TableLayoutPanel
        Me.chkRobots = New System.Windows.Forms.CheckedListBox
        Me.lblBackupFolders = New System.Windows.Forms.Label
        Me.lblRobots = New System.Windows.Forms.Label
        Me.trvFolders = New System.Windows.Forms.TreeView
        Me.tlpRobotFiles = New System.Windows.Forms.TableLayoutPanel
        Me.lstRobotFiles = New System.Windows.Forms.ListBox
        Me.pnlRobots = New System.Windows.Forms.Panel
        Me.cboSourceFileType = New System.Windows.Forms.ComboBox
        Me.rdoSourceSelectFiles = New System.Windows.Forms.RadioButton
        Me.rdoSourceAllFiles = New System.Windows.Forms.RadioButton
        Me.btnBackupToZipRobots = New System.Windows.Forms.Button
        Me.btnBackupToFolderRobots = New System.Windows.Forms.Button
        Me.tabApplication = New System.Windows.Forms.TabPage
        Me.spltApplication = New System.Windows.Forms.SplitContainer
        Me.tlpApplFolders = New System.Windows.Forms.TableLayoutPanel
        Me.lstApplication = New System.Windows.Forms.ListBox
        Me.lblApplication = New System.Windows.Forms.Label
        Me.Panel2 = New System.Windows.Forms.Panel
        Me.btnBackupToZipsApplication = New System.Windows.Forms.Button
        Me.btnBackupToFolderApplication = New System.Windows.Forms.Button
        Me.tabDMON = New System.Windows.Forms.TabPage
        Me.spltDMON = New System.Windows.Forms.SplitContainer
        Me.tlpDMON = New System.Windows.Forms.TableLayoutPanel
        Me.lstDMON = New System.Windows.Forms.ListBox
        Me.lblDMON = New System.Windows.Forms.Label
        Me.pnlDMON = New System.Windows.Forms.Panel
        Me.btnBackupToZipDMON = New System.Windows.Forms.Button
        Me.btnBackupToFolderDMON = New System.Windows.Forms.Button
        Me.tabDMONArchive = New System.Windows.Forms.TabPage
        Me.spltDMONArchive = New System.Windows.Forms.SplitContainer
        Me.tlpDMONArchive = New System.Windows.Forms.TableLayoutPanel
        Me.lstDMONArchive = New System.Windows.Forms.ListBox
        Me.lblDMONArchive = New System.Windows.Forms.Label
        Me.pnlDMONArchive = New System.Windows.Forms.Panel
        Me.btnBackupToZipDMONArchive = New System.Windows.Forms.Button
        Me.btnBackupToFolderDMONArchive = New System.Windows.Forms.Button
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.lblZone = New System.Windows.Forms.Label
        Me.cboZone = New System.Windows.Forms.ComboBox
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.btnChangeLog = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuLast24 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLast7 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAllChanges = New System.Windows.Forms.ToolStripMenuItem
        Me.btnStatus = New System.Windows.Forms.ToolStripButton
        Me.btnUtilities = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuBackupAllDBToFolder = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuBackupAllDBToZip = New System.Windows.Forms.ToolStripMenuItem
        Me.pnlTxt = New System.Windows.Forms.StatusBarPanel
        Me.pnlProg = New System.Windows.Forms.StatusBarPanel
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog
        Me.tscMain.BottomToolStripPanel.SuspendLayout()
        Me.tscMain.ContentPanel.SuspendLayout()
        Me.tscMain.TopToolStripPanel.SuspendLayout()
        Me.tscMain.SuspendLayout()
        Me.stsStatus.SuspendLayout()
        Me.PnlMain.SuspendLayout()
        Me.tabMain.SuspendLayout()
        Me.tabDBSQL.SuspendLayout()
        Me.spltDBSQL.Panel1.SuspendLayout()
        Me.spltDBSQL.Panel2.SuspendLayout()
        Me.spltDBSQL.SuspendLayout()
        Me.tlpDBSQL.SuspendLayout()
        Me.mnuListMenu.SuspendLayout()
        Me.pnlDBSQL.SuspendLayout()
        Me.tabDBAccess.SuspendLayout()
        Me.spltDBAccess.Panel1.SuspendLayout()
        Me.spltDBAccess.Panel2.SuspendLayout()
        Me.spltDBAccess.SuspendLayout()
        Me.tlpDBAccess.SuspendLayout()
        Me.pnlDBAccess.SuspendLayout()
        Me.tabDBXML.SuspendLayout()
        Me.spltDBXML.Panel1.SuspendLayout()
        Me.spltDBXML.Panel2.SuspendLayout()
        Me.spltDBXML.SuspendLayout()
        Me.tlpDBXML.SuspendLayout()
        Me.pnlDBXML.SuspendLayout()
        Me.tabImage.SuspendLayout()
        Me.spltImage.Panel1.SuspendLayout()
        Me.spltImage.Panel2.SuspendLayout()
        Me.spltImage.SuspendLayout()
        Me.tlpImage.SuspendLayout()
        Me.pnlImage.SuspendLayout()
        Me.tabNotepad.SuspendLayout()
        Me.spltNotepad.Panel1.SuspendLayout()
        Me.spltNotepad.Panel2.SuspendLayout()
        Me.spltNotepad.SuspendLayout()
        Me.tlpNotepad.SuspendLayout()
        Me.pnlNotepad.SuspendLayout()
        Me.tabRobots.SuspendLayout()
        Me.spltRobots.Panel1.SuspendLayout()
        Me.spltRobots.Panel2.SuspendLayout()
        Me.spltRobots.SuspendLayout()
        Me.tlpRobotFolders.SuspendLayout()
        Me.tlpRobotFiles.SuspendLayout()
        Me.pnlRobots.SuspendLayout()
        Me.tabApplication.SuspendLayout()
        Me.spltApplication.Panel1.SuspendLayout()
        Me.spltApplication.Panel2.SuspendLayout()
        Me.spltApplication.SuspendLayout()
        Me.tlpApplFolders.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.tabDMON.SuspendLayout()
        Me.spltDMON.Panel1.SuspendLayout()
        Me.spltDMON.Panel2.SuspendLayout()
        Me.spltDMON.SuspendLayout()
        Me.tlpDMON.SuspendLayout()
        Me.pnlDMON.SuspendLayout()
        Me.tabDMONArchive.SuspendLayout()
        Me.spltDMONArchive.Panel1.SuspendLayout()
        Me.spltDMONArchive.Panel2.SuspendLayout()
        Me.spltDMONArchive.SuspendLayout()
        Me.tlpDMONArchive.SuspendLayout()
        Me.pnlDMONArchive.SuspendLayout()
        Me.tlsMain.SuspendLayout()
        CType(Me.pnlTxt, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pnlProg, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tscMain
        '
        '
        'tscMain.BottomToolStripPanel
        '
        Me.tscMain.BottomToolStripPanel.Controls.Add(Me.stsStatus)
        '
        'tscMain.ContentPanel
        '
        Me.tscMain.ContentPanel.Controls.Add(Me.PnlMain)
        Me.tscMain.ContentPanel.Controls.Add(Me.lstStatus)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblZone)
        Me.tscMain.ContentPanel.Controls.Add(Me.cboZone)
        Me.tscMain.ContentPanel.Size = New System.Drawing.Size(1016, 628)
        Me.tscMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tscMain.LeftToolStripPanelVisible = False
        Me.tscMain.Location = New System.Drawing.Point(0, 0)
        Me.tscMain.Name = "tscMain"
        Me.tscMain.RightToolStripPanelVisible = False
        Me.tscMain.Size = New System.Drawing.Size(1016, 717)
        Me.tscMain.TabIndex = 1
        Me.tscMain.Text = "ToolStripContainer1"
        '
        'tscMain.TopToolStripPanel
        '
        Me.tscMain.TopToolStripPanel.Controls.Add(Me.tlsMain)
        '
        'stsStatus
        '
        Me.stsStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.stsStatus.Dock = System.Windows.Forms.DockStyle.None
        Me.stsStatus.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.stsStatus.GripMargin = New System.Windows.Forms.Padding(0)
        Me.stsStatus.ImageScalingSize = New System.Drawing.Size(22, 22)
        Me.stsStatus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatus, Me.tspProgress, Me.lblSpacer, Me.btnFunction})
        Me.stsStatus.Location = New System.Drawing.Point(0, 0)
        Me.stsStatus.Name = "stsStatus"
        Me.stsStatus.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode
        Me.stsStatus.ShowItemToolTips = True
        Me.stsStatus.Size = New System.Drawing.Size(1016, 29)
        Me.stsStatus.TabIndex = 0
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = False
        Me.lblStatus.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedOuter
        Me.lblStatus.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.lblStatus.ImageTransparentColor = System.Drawing.Color.Silver
        Me.lblStatus.Margin = New System.Windows.Forms.Padding(0)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(482, 29)
        Me.lblStatus.Text = "Status...."
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tspProgress
        '
        Me.tspProgress.AutoSize = False
        Me.tspProgress.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.tspProgress.Margin = New System.Windows.Forms.Padding(2, 3, 1, 3)
        Me.tspProgress.Name = "tspProgress"
        Me.tspProgress.Padding = New System.Windows.Forms.Padding(2)
        Me.tspProgress.Size = New System.Drawing.Size(0, 23)
        Me.tspProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        '
        'lblSpacer
        '
        Me.lblSpacer.AutoSize = False
        Me.lblSpacer.Name = "lblSpacer"
        Me.lblSpacer.Size = New System.Drawing.Size(10, 24)
        '
        'btnFunction
        '
        Me.btnFunction.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.btnFunction.AutoSize = False
        Me.btnFunction.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnFunction.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuLogin, Me.mnuLogOut, Me.mnuRemote, Me.mnuLocal})
        Me.btnFunction.ImageTransparentColor = System.Drawing.Color.White
        Me.btnFunction.Name = "btnFunction"
        Me.btnFunction.Padding = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.btnFunction.Size = New System.Drawing.Size(39, 27)
        '
        'mnuLogin
        '
        Me.mnuLogin.Name = "mnuLogin"
        Me.mnuLogin.Size = New System.Drawing.Size(190, 22)
        Me.mnuLogin.Text = "ToolStripMenuItem1"
        '
        'mnuLogOut
        '
        Me.mnuLogOut.Name = "mnuLogOut"
        Me.mnuLogOut.Size = New System.Drawing.Size(190, 22)
        Me.mnuLogOut.Text = "ToolStripMenuItem1"
        '
        'mnuRemote
        '
        Me.mnuRemote.Name = "mnuRemote"
        Me.mnuRemote.Size = New System.Drawing.Size(190, 22)
        Me.mnuRemote.Text = "ToolStripMenuItem1"
        '
        'mnuLocal
        '
        Me.mnuLocal.Name = "mnuLocal"
        Me.mnuLocal.Size = New System.Drawing.Size(190, 22)
        Me.mnuLocal.Text = "ToolStripMenuItem1"
        '
        'PnlMain
        '
        Me.PnlMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.PnlMain.Controls.Add(Me.tabMain)
        Me.PnlMain.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PnlMain.Font = New System.Drawing.Font("Arial", 8.25!)
        Me.PnlMain.Location = New System.Drawing.Point(0, 83)
        Me.PnlMain.Name = "PnlMain"
        Me.PnlMain.Size = New System.Drawing.Size(1016, 545)
        Me.PnlMain.TabIndex = 19
        '
        'tabMain
        '
        Me.tabMain.CausesValidation = False
        Me.tabMain.Controls.Add(Me.tabDBSQL)
        Me.tabMain.Controls.Add(Me.tabDBAccess)
        Me.tabMain.Controls.Add(Me.tabDBXML)
        Me.tabMain.Controls.Add(Me.tabImage)
        Me.tabMain.Controls.Add(Me.tabNotepad)
        Me.tabMain.Controls.Add(Me.tabRobots)
        Me.tabMain.Controls.Add(Me.tabApplication)
        Me.tabMain.Controls.Add(Me.tabDMON)
        Me.tabMain.Controls.Add(Me.tabDMONArchive)
        Me.tabMain.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.tabMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabMain.Enabled = False
        Me.tabMain.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.tabMain.Location = New System.Drawing.Point(0, 0)
        Me.tabMain.Name = "tabMain"
        Me.tabMain.SelectedIndex = 0
        Me.tabMain.Size = New System.Drawing.Size(1016, 545)
        Me.tabMain.TabIndex = 1
        '
        'tabDBSQL
        '
        Me.tabDBSQL.BackColor = System.Drawing.Color.Transparent
        Me.tabDBSQL.Controls.Add(Me.spltDBSQL)
        Me.tabDBSQL.ForeColor = System.Drawing.Color.Black
        Me.tabDBSQL.Location = New System.Drawing.Point(4, 25)
        Me.tabDBSQL.Name = "tabDBSQL"
        Me.tabDBSQL.Padding = New System.Windows.Forms.Padding(3)
        Me.tabDBSQL.Size = New System.Drawing.Size(1008, 516)
        Me.tabDBSQL.TabIndex = 0
        Me.tabDBSQL.Text = "tabDBSQL"
        Me.tabDBSQL.UseVisualStyleBackColor = True
        '
        'spltDBSQL
        '
        Me.spltDBSQL.Dock = System.Windows.Forms.DockStyle.Fill
        Me.spltDBSQL.Location = New System.Drawing.Point(3, 3)
        Me.spltDBSQL.Name = "spltDBSQL"
        '
        'spltDBSQL.Panel1
        '
        Me.spltDBSQL.Panel1.Controls.Add(Me.tlpDBSQL)
        '
        'spltDBSQL.Panel2
        '
        Me.spltDBSQL.Panel2.Controls.Add(Me.pnlDBSQL)
        Me.spltDBSQL.Size = New System.Drawing.Size(1002, 510)
        Me.spltDBSQL.SplitterDistance = 405
        Me.spltDBSQL.TabIndex = 3
        '
        'tlpDBSQL
        '
        Me.tlpDBSQL.ColumnCount = 1
        Me.tlpDBSQL.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpDBSQL.Controls.Add(Me.lstDBSQL, 0, 1)
        Me.tlpDBSQL.Controls.Add(Me.lblDBSQL, 0, 0)
        Me.tlpDBSQL.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpDBSQL.Location = New System.Drawing.Point(0, 0)
        Me.tlpDBSQL.Name = "tlpDBSQL"
        Me.tlpDBSQL.RowCount = 2
        Me.tlpDBSQL.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.28807!))
        Me.tlpDBSQL.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.71194!))
        Me.tlpDBSQL.Size = New System.Drawing.Size(405, 510)
        Me.tlpDBSQL.TabIndex = 5
        '
        'lstDBSQL
        '
        Me.lstDBSQL.ContextMenuStrip = Me.mnuListMenu
        Me.lstDBSQL.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstDBSQL.FormattingEnabled = True
        Me.lstDBSQL.ItemHeight = 16
        Me.lstDBSQL.Location = New System.Drawing.Point(3, 55)
        Me.lstDBSQL.Name = "lstDBSQL"
        Me.lstDBSQL.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstDBSQL.Size = New System.Drawing.Size(399, 452)
        Me.lstDBSQL.TabIndex = 3
        '
        'mnuListMenu
        '
        Me.mnuListMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuSelectAll, Me.mnuUnselectAll})
        Me.mnuListMenu.Name = "mnuCheckList"
        Me.mnuListMenu.Size = New System.Drawing.Size(147, 48)
        '
        'mnuSelectAll
        '
        Me.mnuSelectAll.Name = "mnuSelectAll"
        Me.mnuSelectAll.Size = New System.Drawing.Size(146, 22)
        Me.mnuSelectAll.Text = "mnuSelectAll"
        '
        'mnuUnselectAll
        '
        Me.mnuUnselectAll.Name = "mnuUnselectAll"
        Me.mnuUnselectAll.Size = New System.Drawing.Size(146, 22)
        Me.mnuUnselectAll.Text = "mnuUnselectAll"
        '
        'lblDBSQL
        '
        Me.lblDBSQL.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblDBSQL.Location = New System.Drawing.Point(3, 0)
        Me.lblDBSQL.Name = "lblDBSQL"
        Me.lblDBSQL.Size = New System.Drawing.Size(399, 52)
        Me.lblDBSQL.TabIndex = 1
        Me.lblDBSQL.Text = "lblDBSQL"
        Me.lblDBSQL.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'pnlDBSQL
        '
        Me.pnlDBSQL.AutoScroll = True
        Me.pnlDBSQL.Controls.Add(Me.btnExportToCSVZIPDBSql)
        Me.pnlDBSQL.Controls.Add(Me.btnExportToCSVDBSql)
        Me.pnlDBSQL.Controls.Add(Me.btnBackupToZipDBSql)
        Me.pnlDBSQL.Controls.Add(Me.btnBackupToFolderDBSql)
        Me.pnlDBSQL.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlDBSQL.Location = New System.Drawing.Point(0, 0)
        Me.pnlDBSQL.Name = "pnlDBSQL"
        Me.pnlDBSQL.Size = New System.Drawing.Size(593, 510)
        Me.pnlDBSQL.TabIndex = 5
        '
        'btnExportToCSVZIPDBSql
        '
        Me.btnExportToCSVZIPDBSql.Location = New System.Drawing.Point(22, 175)
        Me.btnExportToCSVZIPDBSql.Name = "btnExportToCSVZIPDBSql"
        Me.btnExportToCSVZIPDBSql.Size = New System.Drawing.Size(235, 25)
        Me.btnExportToCSVZIPDBSql.TabIndex = 3
        Me.btnExportToCSVZIPDBSql.Text = "btnExportToCSVZIPDBSql"
        Me.btnExportToCSVZIPDBSql.UseVisualStyleBackColor = True
        '
        'btnExportToCSVDBSql
        '
        Me.btnExportToCSVDBSql.Location = New System.Drawing.Point(22, 126)
        Me.btnExportToCSVDBSql.Name = "btnExportToCSVDBSql"
        Me.btnExportToCSVDBSql.Size = New System.Drawing.Size(235, 25)
        Me.btnExportToCSVDBSql.TabIndex = 2
        Me.btnExportToCSVDBSql.Text = "btnExportToCSVDBSql"
        Me.btnExportToCSVDBSql.UseVisualStyleBackColor = True
        '
        'btnBackupToZipDBSql
        '
        Me.btnBackupToZipDBSql.Location = New System.Drawing.Point(22, 77)
        Me.btnBackupToZipDBSql.Name = "btnBackupToZipDBSql"
        Me.btnBackupToZipDBSql.Size = New System.Drawing.Size(235, 25)
        Me.btnBackupToZipDBSql.TabIndex = 1
        Me.btnBackupToZipDBSql.Text = "btnBackupToZipDBSql"
        Me.btnBackupToZipDBSql.UseVisualStyleBackColor = True
        '
        'btnBackupToFolderDBSql
        '
        Me.btnBackupToFolderDBSql.Location = New System.Drawing.Point(22, 28)
        Me.btnBackupToFolderDBSql.Name = "btnBackupToFolderDBSql"
        Me.btnBackupToFolderDBSql.Size = New System.Drawing.Size(235, 25)
        Me.btnBackupToFolderDBSql.TabIndex = 0
        Me.btnBackupToFolderDBSql.Text = "btnBackupToFolderDBSql"
        Me.btnBackupToFolderDBSql.UseVisualStyleBackColor = True
        '
        'tabDBAccess
        '
        Me.tabDBAccess.Controls.Add(Me.spltDBAccess)
        Me.tabDBAccess.Location = New System.Drawing.Point(4, 25)
        Me.tabDBAccess.Name = "tabDBAccess"
        Me.tabDBAccess.Size = New System.Drawing.Size(1008, 516)
        Me.tabDBAccess.TabIndex = 1
        Me.tabDBAccess.Text = "tabDBAccess"
        Me.tabDBAccess.UseVisualStyleBackColor = True
        '
        'spltDBAccess
        '
        Me.spltDBAccess.Dock = System.Windows.Forms.DockStyle.Fill
        Me.spltDBAccess.Location = New System.Drawing.Point(0, 0)
        Me.spltDBAccess.Name = "spltDBAccess"
        '
        'spltDBAccess.Panel1
        '
        Me.spltDBAccess.Panel1.Controls.Add(Me.tlpDBAccess)
        '
        'spltDBAccess.Panel2
        '
        Me.spltDBAccess.Panel2.Controls.Add(Me.pnlDBAccess)
        Me.spltDBAccess.Size = New System.Drawing.Size(1008, 516)
        Me.spltDBAccess.SplitterDistance = 405
        Me.spltDBAccess.TabIndex = 4
        '
        'tlpDBAccess
        '
        Me.tlpDBAccess.ColumnCount = 1
        Me.tlpDBAccess.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpDBAccess.Controls.Add(Me.lblDBAccess, 0, 0)
        Me.tlpDBAccess.Controls.Add(Me.lstDBAccess, 0, 1)
        Me.tlpDBAccess.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpDBAccess.Location = New System.Drawing.Point(0, 0)
        Me.tlpDBAccess.Name = "tlpDBAccess"
        Me.tlpDBAccess.RowCount = 2
        Me.tlpDBAccess.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.28807!))
        Me.tlpDBAccess.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.71194!))
        Me.tlpDBAccess.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.tlpDBAccess.Size = New System.Drawing.Size(405, 516)
        Me.tlpDBAccess.TabIndex = 5
        '
        'lblDBAccess
        '
        Me.lblDBAccess.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblDBAccess.Location = New System.Drawing.Point(3, 0)
        Me.lblDBAccess.Name = "lblDBAccess"
        Me.lblDBAccess.Size = New System.Drawing.Size(399, 53)
        Me.lblDBAccess.TabIndex = 4
        Me.lblDBAccess.Text = "lblDBAccess"
        Me.lblDBAccess.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'lstDBAccess
        '
        Me.lstDBAccess.ContextMenuStrip = Me.mnuListMenu
        Me.lstDBAccess.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstDBAccess.FormattingEnabled = True
        Me.lstDBAccess.ItemHeight = 16
        Me.lstDBAccess.Location = New System.Drawing.Point(3, 56)
        Me.lstDBAccess.Name = "lstDBAccess"
        Me.lstDBAccess.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstDBAccess.Size = New System.Drawing.Size(399, 452)
        Me.lstDBAccess.TabIndex = 3
        '
        'pnlDBAccess
        '
        Me.pnlDBAccess.AutoScroll = True
        Me.pnlDBAccess.Controls.Add(Me.btnBackupToZipDBAccess)
        Me.pnlDBAccess.Controls.Add(Me.btnBackupToFolderDBAccess)
        Me.pnlDBAccess.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlDBAccess.Location = New System.Drawing.Point(0, 0)
        Me.pnlDBAccess.Name = "pnlDBAccess"
        Me.pnlDBAccess.Size = New System.Drawing.Size(599, 516)
        Me.pnlDBAccess.TabIndex = 5
        '
        'btnBackupToZipDBAccess
        '
        Me.btnBackupToZipDBAccess.Location = New System.Drawing.Point(22, 77)
        Me.btnBackupToZipDBAccess.Name = "btnBackupToZipDBAccess"
        Me.btnBackupToZipDBAccess.Size = New System.Drawing.Size(235, 25)
        Me.btnBackupToZipDBAccess.TabIndex = 3
        Me.btnBackupToZipDBAccess.Text = "btnBackupToZipDBAccess"
        Me.btnBackupToZipDBAccess.UseVisualStyleBackColor = True
        '
        'btnBackupToFolderDBAccess
        '
        Me.btnBackupToFolderDBAccess.Location = New System.Drawing.Point(22, 28)
        Me.btnBackupToFolderDBAccess.Name = "btnBackupToFolderDBAccess"
        Me.btnBackupToFolderDBAccess.Size = New System.Drawing.Size(235, 25)
        Me.btnBackupToFolderDBAccess.TabIndex = 2
        Me.btnBackupToFolderDBAccess.Text = "btnBackupToFolderDBAccess"
        Me.btnBackupToFolderDBAccess.UseVisualStyleBackColor = True
        '
        'tabDBXML
        '
        Me.tabDBXML.Controls.Add(Me.spltDBXML)
        Me.tabDBXML.Location = New System.Drawing.Point(4, 25)
        Me.tabDBXML.Name = "tabDBXML"
        Me.tabDBXML.Size = New System.Drawing.Size(1008, 516)
        Me.tabDBXML.TabIndex = 2
        Me.tabDBXML.Text = "tabDBXML"
        Me.tabDBXML.UseVisualStyleBackColor = True
        '
        'spltDBXML
        '
        Me.spltDBXML.Dock = System.Windows.Forms.DockStyle.Fill
        Me.spltDBXML.Location = New System.Drawing.Point(0, 0)
        Me.spltDBXML.Name = "spltDBXML"
        '
        'spltDBXML.Panel1
        '
        Me.spltDBXML.Panel1.Controls.Add(Me.tlpDBXML)
        '
        'spltDBXML.Panel2
        '
        Me.spltDBXML.Panel2.Controls.Add(Me.pnlDBXML)
        Me.spltDBXML.Size = New System.Drawing.Size(1008, 516)
        Me.spltDBXML.SplitterDistance = 405
        Me.spltDBXML.TabIndex = 5
        '
        'tlpDBXML
        '
        Me.tlpDBXML.ColumnCount = 1
        Me.tlpDBXML.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpDBXML.Controls.Add(Me.lstDBXML, 0, 1)
        Me.tlpDBXML.Controls.Add(Me.lblDBXML, 0, 0)
        Me.tlpDBXML.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpDBXML.Location = New System.Drawing.Point(0, 0)
        Me.tlpDBXML.Name = "tlpDBXML"
        Me.tlpDBXML.RowCount = 2
        Me.tlpDBXML.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.28807!))
        Me.tlpDBXML.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.71194!))
        Me.tlpDBXML.Size = New System.Drawing.Size(405, 516)
        Me.tlpDBXML.TabIndex = 5
        '
        'lstDBXML
        '
        Me.lstDBXML.ContextMenuStrip = Me.mnuListMenu
        Me.lstDBXML.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstDBXML.FormattingEnabled = True
        Me.lstDBXML.ItemHeight = 16
        Me.lstDBXML.Location = New System.Drawing.Point(3, 56)
        Me.lstDBXML.Name = "lstDBXML"
        Me.lstDBXML.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstDBXML.Size = New System.Drawing.Size(399, 452)
        Me.lstDBXML.TabIndex = 3
        '
        'lblDBXML
        '
        Me.lblDBXML.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblDBXML.Location = New System.Drawing.Point(3, 0)
        Me.lblDBXML.Name = "lblDBXML"
        Me.lblDBXML.Size = New System.Drawing.Size(399, 53)
        Me.lblDBXML.TabIndex = 1
        Me.lblDBXML.Text = "lblDBXML"
        Me.lblDBXML.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'pnlDBXML
        '
        Me.pnlDBXML.AutoScroll = True
        Me.pnlDBXML.Controls.Add(Me.btnBackupToZipDBXML)
        Me.pnlDBXML.Controls.Add(Me.btnBackupToFolderDBXML)
        Me.pnlDBXML.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlDBXML.Location = New System.Drawing.Point(0, 0)
        Me.pnlDBXML.Name = "pnlDBXML"
        Me.pnlDBXML.Size = New System.Drawing.Size(599, 516)
        Me.pnlDBXML.TabIndex = 5
        '
        'btnBackupToZipDBXML
        '
        Me.btnBackupToZipDBXML.Location = New System.Drawing.Point(22, 77)
        Me.btnBackupToZipDBXML.Name = "btnBackupToZipDBXML"
        Me.btnBackupToZipDBXML.Size = New System.Drawing.Size(235, 25)
        Me.btnBackupToZipDBXML.TabIndex = 3
        Me.btnBackupToZipDBXML.Text = "btnBackupToZipDBXML"
        Me.btnBackupToZipDBXML.UseVisualStyleBackColor = True
        '
        'btnBackupToFolderDBXML
        '
        Me.btnBackupToFolderDBXML.Location = New System.Drawing.Point(22, 28)
        Me.btnBackupToFolderDBXML.Name = "btnBackupToFolderDBXML"
        Me.btnBackupToFolderDBXML.Size = New System.Drawing.Size(235, 25)
        Me.btnBackupToFolderDBXML.TabIndex = 2
        Me.btnBackupToFolderDBXML.Text = "btnBackupToFolderDBXML"
        Me.btnBackupToFolderDBXML.UseVisualStyleBackColor = True
        '
        'tabImage
        '
        Me.tabImage.Controls.Add(Me.spltImage)
        Me.tabImage.Location = New System.Drawing.Point(4, 25)
        Me.tabImage.Name = "tabImage"
        Me.tabImage.Size = New System.Drawing.Size(1008, 516)
        Me.tabImage.TabIndex = 3
        Me.tabImage.Text = "tabImage"
        Me.tabImage.UseVisualStyleBackColor = True
        '
        'spltImage
        '
        Me.spltImage.Dock = System.Windows.Forms.DockStyle.Fill
        Me.spltImage.Location = New System.Drawing.Point(0, 0)
        Me.spltImage.Name = "spltImage"
        '
        'spltImage.Panel1
        '
        Me.spltImage.Panel1.Controls.Add(Me.tlpImage)
        '
        'spltImage.Panel2
        '
        Me.spltImage.Panel2.Controls.Add(Me.pnlImage)
        Me.spltImage.Size = New System.Drawing.Size(1008, 516)
        Me.spltImage.SplitterDistance = 405
        Me.spltImage.TabIndex = 5
        '
        'tlpImage
        '
        Me.tlpImage.ColumnCount = 1
        Me.tlpImage.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpImage.Controls.Add(Me.lstImage, 0, 1)
        Me.tlpImage.Controls.Add(Me.lblImage, 0, 0)
        Me.tlpImage.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpImage.Location = New System.Drawing.Point(0, 0)
        Me.tlpImage.Name = "tlpImage"
        Me.tlpImage.RowCount = 2
        Me.tlpImage.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.28807!))
        Me.tlpImage.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.71194!))
        Me.tlpImage.Size = New System.Drawing.Size(405, 516)
        Me.tlpImage.TabIndex = 5
        '
        'lstImage
        '
        Me.lstImage.ContextMenuStrip = Me.mnuListMenu
        Me.lstImage.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstImage.FormattingEnabled = True
        Me.lstImage.ItemHeight = 16
        Me.lstImage.Location = New System.Drawing.Point(3, 56)
        Me.lstImage.Name = "lstImage"
        Me.lstImage.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstImage.Size = New System.Drawing.Size(399, 452)
        Me.lstImage.TabIndex = 3
        '
        'lblImage
        '
        Me.lblImage.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblImage.Location = New System.Drawing.Point(3, 0)
        Me.lblImage.Name = "lblImage"
        Me.lblImage.Size = New System.Drawing.Size(399, 53)
        Me.lblImage.TabIndex = 1
        Me.lblImage.Text = "lblImage"
        Me.lblImage.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'pnlImage
        '
        Me.pnlImage.AutoScroll = True
        Me.pnlImage.Controls.Add(Me.btnBackupToZipImage)
        Me.pnlImage.Controls.Add(Me.btnBackupToFolderImage)
        Me.pnlImage.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlImage.Location = New System.Drawing.Point(0, 0)
        Me.pnlImage.Name = "pnlImage"
        Me.pnlImage.Size = New System.Drawing.Size(599, 516)
        Me.pnlImage.TabIndex = 5
        '
        'btnBackupToZipImage
        '
        Me.btnBackupToZipImage.Location = New System.Drawing.Point(22, 77)
        Me.btnBackupToZipImage.Name = "btnBackupToZipImage"
        Me.btnBackupToZipImage.Size = New System.Drawing.Size(235, 25)
        Me.btnBackupToZipImage.TabIndex = 3
        Me.btnBackupToZipImage.Text = "btnBackupToZipImage"
        Me.btnBackupToZipImage.UseVisualStyleBackColor = True
        '
        'btnBackupToFolderImage
        '
        Me.btnBackupToFolderImage.Location = New System.Drawing.Point(22, 28)
        Me.btnBackupToFolderImage.Name = "btnBackupToFolderImage"
        Me.btnBackupToFolderImage.Size = New System.Drawing.Size(235, 25)
        Me.btnBackupToFolderImage.TabIndex = 2
        Me.btnBackupToFolderImage.Text = "btnBackupToFolderImage"
        Me.btnBackupToFolderImage.UseVisualStyleBackColor = True
        '
        'tabNotepad
        '
        Me.tabNotepad.Controls.Add(Me.spltNotepad)
        Me.tabNotepad.Location = New System.Drawing.Point(4, 25)
        Me.tabNotepad.Name = "tabNotepad"
        Me.tabNotepad.Size = New System.Drawing.Size(1008, 516)
        Me.tabNotepad.TabIndex = 8
        Me.tabNotepad.Text = "tabNotepad"
        Me.tabNotepad.UseVisualStyleBackColor = True
        '
        'spltNotepad
        '
        Me.spltNotepad.Dock = System.Windows.Forms.DockStyle.Fill
        Me.spltNotepad.Location = New System.Drawing.Point(0, 0)
        Me.spltNotepad.Name = "spltNotepad"
        '
        'spltNotepad.Panel1
        '
        Me.spltNotepad.Panel1.Controls.Add(Me.tlpNotepad)
        '
        'spltNotepad.Panel2
        '
        Me.spltNotepad.Panel2.Controls.Add(Me.pnlNotepad)
        Me.spltNotepad.Size = New System.Drawing.Size(1008, 516)
        Me.spltNotepad.SplitterDistance = 405
        Me.spltNotepad.TabIndex = 0
        '
        'tlpNotepad
        '
        Me.tlpNotepad.ColumnCount = 1
        Me.tlpNotepad.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpNotepad.Controls.Add(Me.lblNotepad, 0, 0)
        Me.tlpNotepad.Controls.Add(Me.lstNotepad, 0, 1)
        Me.tlpNotepad.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpNotepad.Location = New System.Drawing.Point(0, 0)
        Me.tlpNotepad.Name = "tlpNotepad"
        Me.tlpNotepad.RowCount = 2
        Me.tlpNotepad.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.83092!))
        Me.tlpNotepad.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88.16908!))
        Me.tlpNotepad.Size = New System.Drawing.Size(405, 516)
        Me.tlpNotepad.TabIndex = 0
        '
        'lblNotepad
        '
        Me.lblNotepad.AutoSize = True
        Me.lblNotepad.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblNotepad.Location = New System.Drawing.Point(3, 0)
        Me.lblNotepad.Name = "lblNotepad"
        Me.lblNotepad.Size = New System.Drawing.Size(399, 61)
        Me.lblNotepad.TabIndex = 0
        Me.lblNotepad.Text = "lblNotepad"
        Me.lblNotepad.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'lstNotepad
        '
        Me.lstNotepad.ContextMenuStrip = Me.mnuListMenu
        Me.lstNotepad.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstNotepad.FormattingEnabled = True
        Me.lstNotepad.ItemHeight = 16
        Me.lstNotepad.Location = New System.Drawing.Point(3, 64)
        Me.lstNotepad.Name = "lstNotepad"
        Me.lstNotepad.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstNotepad.Size = New System.Drawing.Size(399, 436)
        Me.lstNotepad.TabIndex = 3
        '
        'pnlNotepad
        '
        Me.pnlNotepad.Controls.Add(Me.btnBackupToZipNotepad)
        Me.pnlNotepad.Controls.Add(Me.btnBackupToFolderNotepad)
        Me.pnlNotepad.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlNotepad.Location = New System.Drawing.Point(0, 0)
        Me.pnlNotepad.Name = "pnlNotepad"
        Me.pnlNotepad.Size = New System.Drawing.Size(599, 516)
        Me.pnlNotepad.TabIndex = 0
        '
        'btnBackupToZipNotepad
        '
        Me.btnBackupToZipNotepad.Location = New System.Drawing.Point(37, 78)
        Me.btnBackupToZipNotepad.Name = "btnBackupToZipNotepad"
        Me.btnBackupToZipNotepad.Size = New System.Drawing.Size(235, 25)
        Me.btnBackupToZipNotepad.TabIndex = 5
        Me.btnBackupToZipNotepad.Text = "btnBackupToZipNotepad"
        Me.btnBackupToZipNotepad.UseVisualStyleBackColor = True
        '
        'btnBackupToFolderNotepad
        '
        Me.btnBackupToFolderNotepad.Location = New System.Drawing.Point(37, 29)
        Me.btnBackupToFolderNotepad.Name = "btnBackupToFolderNotepad"
        Me.btnBackupToFolderNotepad.Size = New System.Drawing.Size(235, 25)
        Me.btnBackupToFolderNotepad.TabIndex = 4
        Me.btnBackupToFolderNotepad.Text = "btnBackupToFolderNotepad"
        Me.btnBackupToFolderNotepad.UseVisualStyleBackColor = True
        '
        'tabRobots
        '
        Me.tabRobots.Controls.Add(Me.spltRobots)
        Me.tabRobots.Location = New System.Drawing.Point(4, 25)
        Me.tabRobots.Name = "tabRobots"
        Me.tabRobots.Size = New System.Drawing.Size(1008, 516)
        Me.tabRobots.TabIndex = 4
        Me.tabRobots.Text = "tabRobots"
        Me.tabRobots.UseVisualStyleBackColor = True
        '
        'spltRobots
        '
        Me.spltRobots.Dock = System.Windows.Forms.DockStyle.Fill
        Me.spltRobots.Location = New System.Drawing.Point(0, 0)
        Me.spltRobots.Name = "spltRobots"
        '
        'spltRobots.Panel1
        '
        Me.spltRobots.Panel1.Controls.Add(Me.tlpRobotFolders)
        '
        'spltRobots.Panel2
        '
        Me.spltRobots.Panel2.Controls.Add(Me.tlpRobotFiles)
        Me.spltRobots.Size = New System.Drawing.Size(1008, 516)
        Me.spltRobots.SplitterDistance = 483
        Me.spltRobots.TabIndex = 0
        '
        'tlpRobotFolders
        '
        Me.tlpRobotFolders.ColumnCount = 1
        Me.tlpRobotFolders.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpRobotFolders.Controls.Add(Me.chkRobots, 0, 1)
        Me.tlpRobotFolders.Controls.Add(Me.lblBackupFolders, 0, 2)
        Me.tlpRobotFolders.Controls.Add(Me.lblRobots, 0, 0)
        Me.tlpRobotFolders.Controls.Add(Me.trvFolders, 0, 3)
        Me.tlpRobotFolders.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpRobotFolders.Location = New System.Drawing.Point(0, 0)
        Me.tlpRobotFolders.Name = "tlpRobotFolders"
        Me.tlpRobotFolders.RowCount = 4
        Me.tlpRobotFolders.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
        Me.tlpRobotFolders.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.tlpRobotFolders.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
        Me.tlpRobotFolders.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpRobotFolders.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.tlpRobotFolders.Size = New System.Drawing.Size(483, 516)
        Me.tlpRobotFolders.TabIndex = 0
        '
        'chkRobots
        '
        Me.chkRobots.CheckOnClick = True
        Me.chkRobots.ContextMenuStrip = Me.mnuListMenu
        Me.chkRobots.Dock = System.Windows.Forms.DockStyle.Fill
        Me.chkRobots.FormattingEnabled = True
        Me.chkRobots.HorizontalScrollbar = True
        Me.chkRobots.Location = New System.Drawing.Point(3, 54)
        Me.chkRobots.Name = "chkRobots"
        Me.chkRobots.Size = New System.Drawing.Size(477, 140)
        Me.chkRobots.TabIndex = 5
        '
        'lblBackupFolders
        '
        Me.lblBackupFolders.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblBackupFolders.Location = New System.Drawing.Point(3, 205)
        Me.lblBackupFolders.Name = "lblBackupFolders"
        Me.lblBackupFolders.Size = New System.Drawing.Size(477, 51)
        Me.lblBackupFolders.TabIndex = 3
        Me.lblBackupFolders.Text = "lblBackupFolders"
        Me.lblBackupFolders.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'lblRobots
        '
        Me.lblRobots.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblRobots.Location = New System.Drawing.Point(3, 0)
        Me.lblRobots.Name = "lblRobots"
        Me.lblRobots.Size = New System.Drawing.Size(477, 51)
        Me.lblRobots.TabIndex = 2
        Me.lblRobots.Text = "lblRobots"
        Me.lblRobots.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'trvFolders
        '
        Me.trvFolders.CheckBoxes = True
        Me.trvFolders.ContextMenuStrip = Me.mnuListMenu
        Me.trvFolders.Dock = System.Windows.Forms.DockStyle.Fill
        Me.trvFolders.Location = New System.Drawing.Point(3, 259)
        Me.trvFolders.Name = "trvFolders"
        Me.trvFolders.Size = New System.Drawing.Size(477, 254)
        Me.trvFolders.TabIndex = 6
        '
        'tlpRobotFiles
        '
        Me.tlpRobotFiles.ColumnCount = 1
        Me.tlpRobotFiles.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpRobotFiles.Controls.Add(Me.lstRobotFiles, 0, 1)
        Me.tlpRobotFiles.Controls.Add(Me.pnlRobots, 0, 0)
        Me.tlpRobotFiles.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpRobotFiles.Location = New System.Drawing.Point(0, 0)
        Me.tlpRobotFiles.Name = "tlpRobotFiles"
        Me.tlpRobotFiles.RowCount = 2
        Me.tlpRobotFiles.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 188.0!))
        Me.tlpRobotFiles.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpRobotFiles.Size = New System.Drawing.Size(521, 516)
        Me.tlpRobotFiles.TabIndex = 7
        '
        'lstRobotFiles
        '
        Me.lstRobotFiles.ContextMenuStrip = Me.mnuListMenu
        Me.lstRobotFiles.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstRobotFiles.FormattingEnabled = True
        Me.lstRobotFiles.ItemHeight = 16
        Me.lstRobotFiles.Location = New System.Drawing.Point(3, 191)
        Me.lstRobotFiles.MultiColumn = True
        Me.lstRobotFiles.Name = "lstRobotFiles"
        Me.lstRobotFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstRobotFiles.Size = New System.Drawing.Size(515, 308)
        Me.lstRobotFiles.TabIndex = 8
        '
        'pnlRobots
        '
        Me.pnlRobots.AutoScroll = True
        Me.pnlRobots.Controls.Add(Me.cboSourceFileType)
        Me.pnlRobots.Controls.Add(Me.rdoSourceSelectFiles)
        Me.pnlRobots.Controls.Add(Me.rdoSourceAllFiles)
        Me.pnlRobots.Controls.Add(Me.btnBackupToZipRobots)
        Me.pnlRobots.Controls.Add(Me.btnBackupToFolderRobots)
        Me.pnlRobots.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlRobots.Location = New System.Drawing.Point(3, 3)
        Me.pnlRobots.Name = "pnlRobots"
        Me.pnlRobots.Size = New System.Drawing.Size(515, 182)
        Me.pnlRobots.TabIndex = 7
        '
        'cboSourceFileType
        '
        Me.cboSourceFileType.FormattingEnabled = True
        Me.cboSourceFileType.Location = New System.Drawing.Point(22, 153)
        Me.cboSourceFileType.Name = "cboSourceFileType"
        Me.cboSourceFileType.Size = New System.Drawing.Size(282, 24)
        Me.cboSourceFileType.TabIndex = 4
        '
        'rdoSourceSelectFiles
        '
        Me.rdoSourceSelectFiles.AutoSize = True
        Me.rdoSourceSelectFiles.Font = New System.Drawing.Font("Arial", 9.0!)
        Me.rdoSourceSelectFiles.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rdoSourceSelectFiles.Location = New System.Drawing.Point(173, 128)
        Me.rdoSourceSelectFiles.Name = "rdoSourceSelectFiles"
        Me.rdoSourceSelectFiles.Size = New System.Drawing.Size(143, 19)
        Me.rdoSourceSelectFiles.TabIndex = 1
        Me.rdoSourceSelectFiles.Text = "rdoSourceSelectFiles"
        Me.rdoSourceSelectFiles.UseVisualStyleBackColor = True
        '
        'rdoSourceAllFiles
        '
        Me.rdoSourceAllFiles.AutoSize = True
        Me.rdoSourceAllFiles.Checked = True
        Me.rdoSourceAllFiles.Font = New System.Drawing.Font("Arial", 9.0!)
        Me.rdoSourceAllFiles.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rdoSourceAllFiles.Location = New System.Drawing.Point(22, 128)
        Me.rdoSourceAllFiles.Name = "rdoSourceAllFiles"
        Me.rdoSourceAllFiles.Size = New System.Drawing.Size(122, 19)
        Me.rdoSourceAllFiles.TabIndex = 0
        Me.rdoSourceAllFiles.TabStop = True
        Me.rdoSourceAllFiles.Text = "rdoSourceAllFiles"
        Me.rdoSourceAllFiles.UseVisualStyleBackColor = True
        '
        'btnBackupToZipRobots
        '
        Me.btnBackupToZipRobots.Location = New System.Drawing.Point(22, 77)
        Me.btnBackupToZipRobots.Name = "btnBackupToZipRobots"
        Me.btnBackupToZipRobots.Size = New System.Drawing.Size(235, 25)
        Me.btnBackupToZipRobots.TabIndex = 3
        Me.btnBackupToZipRobots.Text = "btnBackupToZipRobots"
        Me.btnBackupToZipRobots.UseVisualStyleBackColor = True
        '
        'btnBackupToFolderRobots
        '
        Me.btnBackupToFolderRobots.Location = New System.Drawing.Point(22, 28)
        Me.btnBackupToFolderRobots.Name = "btnBackupToFolderRobots"
        Me.btnBackupToFolderRobots.Size = New System.Drawing.Size(235, 25)
        Me.btnBackupToFolderRobots.TabIndex = 2
        Me.btnBackupToFolderRobots.Text = "btnBackupToFolderRobots"
        Me.btnBackupToFolderRobots.UseVisualStyleBackColor = True
        '
        'tabApplication
        '
        Me.tabApplication.Controls.Add(Me.spltApplication)
        Me.tabApplication.Location = New System.Drawing.Point(4, 25)
        Me.tabApplication.Name = "tabApplication"
        Me.tabApplication.Padding = New System.Windows.Forms.Padding(3)
        Me.tabApplication.Size = New System.Drawing.Size(1008, 516)
        Me.tabApplication.TabIndex = 5
        Me.tabApplication.Text = "tabApplication"
        Me.tabApplication.UseVisualStyleBackColor = True
        '
        'spltApplication
        '
        Me.spltApplication.Dock = System.Windows.Forms.DockStyle.Fill
        Me.spltApplication.Location = New System.Drawing.Point(3, 3)
        Me.spltApplication.Name = "spltApplication"
        '
        'spltApplication.Panel1
        '
        Me.spltApplication.Panel1.Controls.Add(Me.tlpApplFolders)
        '
        'spltApplication.Panel2
        '
        Me.spltApplication.Panel2.Controls.Add(Me.Panel2)
        Me.spltApplication.Size = New System.Drawing.Size(1002, 510)
        Me.spltApplication.SplitterDistance = 405
        Me.spltApplication.TabIndex = 4
        '
        'tlpApplFolders
        '
        Me.tlpApplFolders.ColumnCount = 1
        Me.tlpApplFolders.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpApplFolders.Controls.Add(Me.lstApplication, 0, 1)
        Me.tlpApplFolders.Controls.Add(Me.lblApplication, 0, 0)
        Me.tlpApplFolders.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpApplFolders.Location = New System.Drawing.Point(0, 0)
        Me.tlpApplFolders.Name = "tlpApplFolders"
        Me.tlpApplFolders.RowCount = 2
        Me.tlpApplFolders.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.28807!))
        Me.tlpApplFolders.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.71194!))
        Me.tlpApplFolders.Size = New System.Drawing.Size(405, 510)
        Me.tlpApplFolders.TabIndex = 5
        '
        'lstApplication
        '
        Me.lstApplication.ContextMenuStrip = Me.mnuListMenu
        Me.lstApplication.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstApplication.FormattingEnabled = True
        Me.lstApplication.ItemHeight = 16
        Me.lstApplication.Location = New System.Drawing.Point(3, 55)
        Me.lstApplication.Name = "lstApplication"
        Me.lstApplication.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstApplication.Size = New System.Drawing.Size(399, 452)
        Me.lstApplication.TabIndex = 3
        '
        'lblApplication
        '
        Me.lblApplication.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblApplication.Location = New System.Drawing.Point(3, 0)
        Me.lblApplication.Name = "lblApplication"
        Me.lblApplication.Size = New System.Drawing.Size(399, 52)
        Me.lblApplication.TabIndex = 1
        Me.lblApplication.Text = "lblApplication"
        Me.lblApplication.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'Panel2
        '
        Me.Panel2.AutoScroll = True
        Me.Panel2.Controls.Add(Me.btnBackupToZipsApplication)
        Me.Panel2.Controls.Add(Me.btnBackupToFolderApplication)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(593, 510)
        Me.Panel2.TabIndex = 5
        '
        'btnBackupToZipsApplication
        '
        Me.btnBackupToZipsApplication.Location = New System.Drawing.Point(22, 77)
        Me.btnBackupToZipsApplication.Name = "btnBackupToZipsApplication"
        Me.btnBackupToZipsApplication.Size = New System.Drawing.Size(235, 25)
        Me.btnBackupToZipsApplication.TabIndex = 1
        Me.btnBackupToZipsApplication.Text = "btnBackupToZipsApplication"
        Me.btnBackupToZipsApplication.UseVisualStyleBackColor = True
        '
        'btnBackupToFolderApplication
        '
        Me.btnBackupToFolderApplication.Location = New System.Drawing.Point(22, 28)
        Me.btnBackupToFolderApplication.Name = "btnBackupToFolderApplication"
        Me.btnBackupToFolderApplication.Size = New System.Drawing.Size(235, 25)
        Me.btnBackupToFolderApplication.TabIndex = 0
        Me.btnBackupToFolderApplication.Text = "btnBackupToFolderApplication"
        Me.btnBackupToFolderApplication.UseVisualStyleBackColor = True
        '
        'tabDMON
        '
        Me.tabDMON.Controls.Add(Me.spltDMON)
        Me.tabDMON.Location = New System.Drawing.Point(4, 25)
        Me.tabDMON.Name = "tabDMON"
        Me.tabDMON.Padding = New System.Windows.Forms.Padding(3)
        Me.tabDMON.Size = New System.Drawing.Size(1008, 516)
        Me.tabDMON.TabIndex = 6
        Me.tabDMON.Text = "tabDMON"
        Me.tabDMON.UseVisualStyleBackColor = True
        '
        'spltDMON
        '
        Me.spltDMON.Dock = System.Windows.Forms.DockStyle.Fill
        Me.spltDMON.Location = New System.Drawing.Point(3, 3)
        Me.spltDMON.Name = "spltDMON"
        '
        'spltDMON.Panel1
        '
        Me.spltDMON.Panel1.Controls.Add(Me.tlpDMON)
        '
        'spltDMON.Panel2
        '
        Me.spltDMON.Panel2.Controls.Add(Me.pnlDMON)
        Me.spltDMON.Size = New System.Drawing.Size(1002, 510)
        Me.spltDMON.SplitterDistance = 405
        Me.spltDMON.TabIndex = 6
        '
        'tlpDMON
        '
        Me.tlpDMON.ColumnCount = 1
        Me.tlpDMON.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpDMON.Controls.Add(Me.lstDMON, 0, 1)
        Me.tlpDMON.Controls.Add(Me.lblDMON, 0, 0)
        Me.tlpDMON.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpDMON.Location = New System.Drawing.Point(0, 0)
        Me.tlpDMON.Name = "tlpDMON"
        Me.tlpDMON.RowCount = 2
        Me.tlpDMON.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.28807!))
        Me.tlpDMON.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.71194!))
        Me.tlpDMON.Size = New System.Drawing.Size(405, 510)
        Me.tlpDMON.TabIndex = 6
        '
        'lstDMON
        '
        Me.lstDMON.ContextMenuStrip = Me.mnuListMenu
        Me.lstDMON.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstDMON.FormattingEnabled = True
        Me.lstDMON.ItemHeight = 16
        Me.lstDMON.Location = New System.Drawing.Point(3, 55)
        Me.lstDMON.Name = "lstDMON"
        Me.lstDMON.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstDMON.Size = New System.Drawing.Size(399, 452)
        Me.lstDMON.TabIndex = 3
        '
        'lblDMON
        '
        Me.lblDMON.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblDMON.Location = New System.Drawing.Point(3, 0)
        Me.lblDMON.Name = "lblDMON"
        Me.lblDMON.Size = New System.Drawing.Size(399, 52)
        Me.lblDMON.TabIndex = 1
        Me.lblDMON.Text = "lblDMON"
        Me.lblDMON.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'pnlDMON
        '
        Me.pnlDMON.AutoScroll = True
        Me.pnlDMON.Controls.Add(Me.btnBackupToZipDMON)
        Me.pnlDMON.Controls.Add(Me.btnBackupToFolderDMON)
        Me.pnlDMON.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlDMON.Location = New System.Drawing.Point(0, 0)
        Me.pnlDMON.Name = "pnlDMON"
        Me.pnlDMON.Size = New System.Drawing.Size(593, 510)
        Me.pnlDMON.TabIndex = 6
        '
        'btnBackupToZipDMON
        '
        Me.btnBackupToZipDMON.Location = New System.Drawing.Point(22, 77)
        Me.btnBackupToZipDMON.Name = "btnBackupToZipDMON"
        Me.btnBackupToZipDMON.Size = New System.Drawing.Size(235, 25)
        Me.btnBackupToZipDMON.TabIndex = 3
        Me.btnBackupToZipDMON.Text = "btnBackupToZipDMON"
        Me.btnBackupToZipDMON.UseVisualStyleBackColor = True
        '
        'btnBackupToFolderDMON
        '
        Me.btnBackupToFolderDMON.Location = New System.Drawing.Point(22, 28)
        Me.btnBackupToFolderDMON.Name = "btnBackupToFolderDMON"
        Me.btnBackupToFolderDMON.Size = New System.Drawing.Size(235, 25)
        Me.btnBackupToFolderDMON.TabIndex = 2
        Me.btnBackupToFolderDMON.Text = "btnBackupToFolderDMON"
        Me.btnBackupToFolderDMON.UseVisualStyleBackColor = True
        '
        'tabDMONArchive
        '
        Me.tabDMONArchive.Controls.Add(Me.spltDMONArchive)
        Me.tabDMONArchive.Location = New System.Drawing.Point(4, 25)
        Me.tabDMONArchive.Name = "tabDMONArchive"
        Me.tabDMONArchive.Padding = New System.Windows.Forms.Padding(3)
        Me.tabDMONArchive.Size = New System.Drawing.Size(1008, 516)
        Me.tabDMONArchive.TabIndex = 7
        Me.tabDMONArchive.Text = "tabDMONArchive"
        Me.tabDMONArchive.UseVisualStyleBackColor = True
        '
        'spltDMONArchive
        '
        Me.spltDMONArchive.Dock = System.Windows.Forms.DockStyle.Fill
        Me.spltDMONArchive.Location = New System.Drawing.Point(3, 3)
        Me.spltDMONArchive.Name = "spltDMONArchive"
        '
        'spltDMONArchive.Panel1
        '
        Me.spltDMONArchive.Panel1.Controls.Add(Me.tlpDMONArchive)
        '
        'spltDMONArchive.Panel2
        '
        Me.spltDMONArchive.Panel2.Controls.Add(Me.pnlDMONArchive)
        Me.spltDMONArchive.Size = New System.Drawing.Size(1002, 510)
        Me.spltDMONArchive.SplitterDistance = 405
        Me.spltDMONArchive.TabIndex = 7
        '
        'tlpDMONArchive
        '
        Me.tlpDMONArchive.ColumnCount = 1
        Me.tlpDMONArchive.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpDMONArchive.Controls.Add(Me.lstDMONArchive, 0, 1)
        Me.tlpDMONArchive.Controls.Add(Me.lblDMONArchive, 0, 0)
        Me.tlpDMONArchive.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpDMONArchive.Location = New System.Drawing.Point(0, 0)
        Me.tlpDMONArchive.Name = "tlpDMONArchive"
        Me.tlpDMONArchive.RowCount = 2
        Me.tlpDMONArchive.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.28807!))
        Me.tlpDMONArchive.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.71194!))
        Me.tlpDMONArchive.Size = New System.Drawing.Size(405, 510)
        Me.tlpDMONArchive.TabIndex = 5
        '
        'lstDMONArchive
        '
        Me.lstDMONArchive.ContextMenuStrip = Me.mnuListMenu
        Me.lstDMONArchive.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstDMONArchive.FormattingEnabled = True
        Me.lstDMONArchive.ItemHeight = 16
        Me.lstDMONArchive.Location = New System.Drawing.Point(3, 55)
        Me.lstDMONArchive.Name = "lstDMONArchive"
        Me.lstDMONArchive.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstDMONArchive.Size = New System.Drawing.Size(399, 452)
        Me.lstDMONArchive.TabIndex = 3
        '
        'lblDMONArchive
        '
        Me.lblDMONArchive.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblDMONArchive.Location = New System.Drawing.Point(3, 0)
        Me.lblDMONArchive.Name = "lblDMONArchive"
        Me.lblDMONArchive.Size = New System.Drawing.Size(399, 52)
        Me.lblDMONArchive.TabIndex = 1
        Me.lblDMONArchive.Text = "lblDMONArchive"
        Me.lblDMONArchive.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'pnlDMONArchive
        '
        Me.pnlDMONArchive.AutoScroll = True
        Me.pnlDMONArchive.Controls.Add(Me.btnBackupToZipDMONArchive)
        Me.pnlDMONArchive.Controls.Add(Me.btnBackupToFolderDMONArchive)
        Me.pnlDMONArchive.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlDMONArchive.Location = New System.Drawing.Point(0, 0)
        Me.pnlDMONArchive.Name = "pnlDMONArchive"
        Me.pnlDMONArchive.Size = New System.Drawing.Size(593, 510)
        Me.pnlDMONArchive.TabIndex = 5
        '
        'btnBackupToZipDMONArchive
        '
        Me.btnBackupToZipDMONArchive.Location = New System.Drawing.Point(22, 77)
        Me.btnBackupToZipDMONArchive.Name = "btnBackupToZipDMONArchive"
        Me.btnBackupToZipDMONArchive.Size = New System.Drawing.Size(235, 25)
        Me.btnBackupToZipDMONArchive.TabIndex = 3
        Me.btnBackupToZipDMONArchive.Text = "btnBackupToZipDMONArchive"
        Me.btnBackupToZipDMONArchive.UseVisualStyleBackColor = True
        '
        'btnBackupToFolderDMONArchive
        '
        Me.btnBackupToFolderDMONArchive.Location = New System.Drawing.Point(22, 28)
        Me.btnBackupToFolderDMONArchive.Name = "btnBackupToFolderDMONArchive"
        Me.btnBackupToFolderDMONArchive.Size = New System.Drawing.Size(235, 25)
        Me.btnBackupToFolderDMONArchive.TabIndex = 2
        Me.btnBackupToFolderDMONArchive.Text = "btnBackupToFolderDMONArchive"
        Me.btnBackupToFolderDMONArchive.UseVisualStyleBackColor = True
        '
        'lstStatus
        '
        Me.lstStatus.BackColor = System.Drawing.SystemColors.Info
        Me.lstStatus.ItemHeight = 14
        Me.lstStatus.Location = New System.Drawing.Point(252, 4)
        Me.lstStatus.Name = "lstStatus"
        Me.lstStatus.Size = New System.Drawing.Size(742, 60)
        Me.lstStatus.TabIndex = 11
        '
        'lblZone
        '
        Me.lblZone.AutoSize = True
        Me.lblZone.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblZone.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblZone.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblZone.Location = New System.Drawing.Point(56, 8)
        Me.lblZone.Name = "lblZone"
        Me.lblZone.Size = New System.Drawing.Size(47, 19)
        Me.lblZone.TabIndex = 12
        Me.lblZone.Text = "Zone"
        Me.lblZone.UseMnemonic = False
        Me.lblZone.Visible = False
        '
        'cboZone
        '
        Me.cboZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboZone.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboZone.ItemHeight = 18
        Me.cboZone.Location = New System.Drawing.Point(17, 33)
        Me.cboZone.Name = "cboZone"
        Me.cboZone.Size = New System.Drawing.Size(160, 26)
        Me.cboZone.TabIndex = 10
        '
        'tlsMain
        '
        Me.tlsMain.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.tlsMain.Dock = System.Windows.Forms.DockStyle.None
        Me.tlsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.btnChangeLog, Me.btnStatus, Me.btnUtilities})
        Me.tlsMain.Location = New System.Drawing.Point(0, 0)
        Me.tlsMain.MinimumSize = New System.Drawing.Size(0, 50)
        Me.tlsMain.Name = "tlsMain"
        Me.tlsMain.Size = New System.Drawing.Size(1016, 60)
        Me.tlsMain.Stretch = True
        Me.tlsMain.TabIndex = 2
        Me.tlsMain.TabStop = True
        '
        'btnClose
        '
        Me.btnClose.AutoSize = False
        Me.btnClose.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnClose.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(70, 57)
        Me.btnClose.Text = "Close"
        Me.btnClose.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'btnChangeLog
        '
        Me.btnChangeLog.DropDownButtonWidth = 13
        Me.btnChangeLog.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuLast24, Me.mnuLast7, Me.mnuAllChanges})
        Me.btnChangeLog.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnChangeLog.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnChangeLog.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnChangeLog.Name = "btnChangeLog"
        Me.btnChangeLog.Size = New System.Drawing.Size(95, 57)
        Me.btnChangeLog.Text = "Change Log"
        Me.btnChangeLog.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnChangeLog.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'mnuLast24
        '
        Me.mnuLast24.Name = "mnuLast24"
        Me.mnuLast24.Size = New System.Drawing.Size(157, 22)
        Me.mnuLast24.Text = "Last 24 Hours"
        '
        'mnuLast7
        '
        Me.mnuLast7.Name = "mnuLast7"
        Me.mnuLast7.Size = New System.Drawing.Size(157, 22)
        Me.mnuLast7.Text = "Last 7 Days"
        '
        'mnuAllChanges
        '
        Me.mnuAllChanges.Name = "mnuAllChanges"
        Me.mnuAllChanges.Size = New System.Drawing.Size(157, 22)
        Me.mnuAllChanges.Text = "All Changes"
        '
        'btnStatus
        '
        Me.btnStatus.AutoSize = False
        Me.btnStatus.Checked = True
        Me.btnStatus.CheckOnClick = True
        Me.btnStatus.CheckState = System.Windows.Forms.CheckState.Checked
        Me.btnStatus.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.btnStatus.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnStatus.Name = "btnStatus"
        Me.btnStatus.Size = New System.Drawing.Size(70, 57)
        Me.btnStatus.Text = "Status"
        Me.btnStatus.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.btnStatus.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'btnUtilities
        '
        Me.btnUtilities.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuBackupAllDBToFolder, Me.mnuBackupAllDBToZip})
        Me.btnUtilities.Image = CType(resources.GetObject("btnUtilities.Image"), System.Drawing.Image)
        Me.btnUtilities.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnUtilities.Name = "btnUtilities"
        Me.btnUtilities.Size = New System.Drawing.Size(73, 57)
        Me.btnUtilities.Text = "btnUtilities"
        Me.btnUtilities.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.btnUtilities.ToolTipText = "Utilities"
        '
        'mnuBackupAllDBToFolder
        '
        Me.mnuBackupAllDBToFolder.Name = "mnuBackupAllDBToFolder"
        Me.mnuBackupAllDBToFolder.Size = New System.Drawing.Size(194, 22)
        Me.mnuBackupAllDBToFolder.Text = "mnuBackupAllDBToFolder"
        '
        'mnuBackupAllDBToZip
        '
        Me.mnuBackupAllDBToZip.Name = "mnuBackupAllDBToZip"
        Me.mnuBackupAllDBToZip.Size = New System.Drawing.Size(194, 22)
        Me.mnuBackupAllDBToZip.Text = "mnuBackupAllDBToZip"
        '
        'pnlTxt
        '
        Me.pnlTxt.MinWidth = 100
        Me.pnlTxt.Name = "pnlTxt"
        '
        'pnlProg
        '
        Me.pnlProg.MinWidth = 100
        Me.pnlProg.Name = "pnlProg"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1016, 717)
        Me.ControlBox = False
        Me.Controls.Add(Me.tscMain)
        Me.Font = New System.Drawing.Font("Arial", 8.25!)
        Me.KeyPreview = True
        Me.Name = "frmMain"
        Me.tscMain.BottomToolStripPanel.ResumeLayout(False)
        Me.tscMain.BottomToolStripPanel.PerformLayout()
        Me.tscMain.ContentPanel.ResumeLayout(False)
        Me.tscMain.ContentPanel.PerformLayout()
        Me.tscMain.TopToolStripPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.PerformLayout()
        Me.tscMain.ResumeLayout(False)
        Me.tscMain.PerformLayout()
        Me.stsStatus.ResumeLayout(False)
        Me.stsStatus.PerformLayout()
        Me.PnlMain.ResumeLayout(False)
        Me.tabMain.ResumeLayout(False)
        Me.tabDBSQL.ResumeLayout(False)
        Me.spltDBSQL.Panel1.ResumeLayout(False)
        Me.spltDBSQL.Panel2.ResumeLayout(False)
        Me.spltDBSQL.ResumeLayout(False)
        Me.tlpDBSQL.ResumeLayout(False)
        Me.mnuListMenu.ResumeLayout(False)
        Me.pnlDBSQL.ResumeLayout(False)
        Me.tabDBAccess.ResumeLayout(False)
        Me.spltDBAccess.Panel1.ResumeLayout(False)
        Me.spltDBAccess.Panel2.ResumeLayout(False)
        Me.spltDBAccess.ResumeLayout(False)
        Me.tlpDBAccess.ResumeLayout(False)
        Me.pnlDBAccess.ResumeLayout(False)
        Me.tabDBXML.ResumeLayout(False)
        Me.spltDBXML.Panel1.ResumeLayout(False)
        Me.spltDBXML.Panel2.ResumeLayout(False)
        Me.spltDBXML.ResumeLayout(False)
        Me.tlpDBXML.ResumeLayout(False)
        Me.pnlDBXML.ResumeLayout(False)
        Me.tabImage.ResumeLayout(False)
        Me.spltImage.Panel1.ResumeLayout(False)
        Me.spltImage.Panel2.ResumeLayout(False)
        Me.spltImage.ResumeLayout(False)
        Me.tlpImage.ResumeLayout(False)
        Me.pnlImage.ResumeLayout(False)
        Me.tabNotepad.ResumeLayout(False)
        Me.spltNotepad.Panel1.ResumeLayout(False)
        Me.spltNotepad.Panel2.ResumeLayout(False)
        Me.spltNotepad.ResumeLayout(False)
        Me.tlpNotepad.ResumeLayout(False)
        Me.tlpNotepad.PerformLayout()
        Me.pnlNotepad.ResumeLayout(False)
        Me.tabRobots.ResumeLayout(False)
        Me.spltRobots.Panel1.ResumeLayout(False)
        Me.spltRobots.Panel2.ResumeLayout(False)
        Me.spltRobots.ResumeLayout(False)
        Me.tlpRobotFolders.ResumeLayout(False)
        Me.tlpRobotFiles.ResumeLayout(False)
        Me.pnlRobots.ResumeLayout(False)
        Me.pnlRobots.PerformLayout()
        Me.tabApplication.ResumeLayout(False)
        Me.spltApplication.Panel1.ResumeLayout(False)
        Me.spltApplication.Panel2.ResumeLayout(False)
        Me.spltApplication.ResumeLayout(False)
        Me.tlpApplFolders.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.tabDMON.ResumeLayout(False)
        Me.spltDMON.Panel1.ResumeLayout(False)
        Me.spltDMON.Panel2.ResumeLayout(False)
        Me.spltDMON.ResumeLayout(False)
        Me.tlpDMON.ResumeLayout(False)
        Me.pnlDMON.ResumeLayout(False)
        Me.tabDMONArchive.ResumeLayout(False)
        Me.spltDMONArchive.Panel1.ResumeLayout(False)
        Me.spltDMONArchive.Panel2.ResumeLayout(False)
        Me.spltDMONArchive.ResumeLayout(False)
        Me.tlpDMONArchive.ResumeLayout(False)
        Me.pnlDMONArchive.ResumeLayout(False)
        Me.tlsMain.ResumeLayout(False)
        Me.tlsMain.PerformLayout()
        CType(Me.pnlTxt, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pnlProg, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tscMain As System.Windows.Forms.ToolStripContainer
    Friend WithEvents stsStatus As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tspProgress As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents pnlTxt As System.Windows.Forms.StatusBarPanel
    Friend WithEvents pnlProg As System.Windows.Forms.StatusBarPanel
    Friend WithEvents lstStatus As System.Windows.Forms.ListBox
    Friend WithEvents lblZone As System.Windows.Forms.Label
    Friend WithEvents cboZone As System.Windows.Forms.ComboBox
    Friend WithEvents btnFunction As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents mnuLogin As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLogOut As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuRemote As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLocal As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblSpacer As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnClose As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnChangeLog As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuLast7 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAllChanges As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLast24 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PnlMain As System.Windows.Forms.Panel
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents tabMain As System.Windows.Forms.TabControl
    Friend WithEvents tabDBSQL As System.Windows.Forms.TabPage
    Friend WithEvents btnStatus As System.Windows.Forms.ToolStripButton
    Friend WithEvents spltDBSQL As System.Windows.Forms.SplitContainer
    Friend WithEvents tlpDBSQL As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lstDBSQL As System.Windows.Forms.ListBox
    Friend WithEvents lblDBSQL As System.Windows.Forms.Label
    Friend WithEvents pnlDBSQL As System.Windows.Forms.Panel
    Friend WithEvents tabDBAccess As System.Windows.Forms.TabPage
    Friend WithEvents tabDBXML As System.Windows.Forms.TabPage
    Friend WithEvents tabImage As System.Windows.Forms.TabPage
    Friend WithEvents tabRobots As System.Windows.Forms.TabPage
    Friend WithEvents spltDBXML As System.Windows.Forms.SplitContainer
    Friend WithEvents tlpDBXML As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lstDBXML As System.Windows.Forms.ListBox
    Friend WithEvents lblDBXML As System.Windows.Forms.Label
    Friend WithEvents pnlDBXML As System.Windows.Forms.Panel
    Friend WithEvents spltDBAccess As System.Windows.Forms.SplitContainer
    Friend WithEvents tlpDBAccess As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lstDBAccess As System.Windows.Forms.ListBox
    Friend WithEvents pnlDBAccess As System.Windows.Forms.Panel
    Friend WithEvents spltImage As System.Windows.Forms.SplitContainer
    Friend WithEvents tlpImage As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lstImage As System.Windows.Forms.ListBox
    Friend WithEvents lblImage As System.Windows.Forms.Label
    Friend WithEvents pnlImage As System.Windows.Forms.Panel
    Friend WithEvents mnuListMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuSelectAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuUnselectAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnBackupToZipDBSql As System.Windows.Forms.Button
    Friend WithEvents btnBackupToFolderDBSql As System.Windows.Forms.Button
    Friend WithEvents btnBackupToZipDBAccess As System.Windows.Forms.Button
    Friend WithEvents btnBackupToFolderDBAccess As System.Windows.Forms.Button
    Friend WithEvents btnBackupToZipDBXML As System.Windows.Forms.Button
    Friend WithEvents btnBackupToFolderDBXML As System.Windows.Forms.Button
    Friend WithEvents btnBackupToZipImage As System.Windows.Forms.Button
    Friend WithEvents btnBackupToFolderImage As System.Windows.Forms.Button
    Friend WithEvents btnUtilities As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuBackupAllDBToFolder As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuBackupAllDBToZip As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents spltRobots As System.Windows.Forms.SplitContainer
    Friend WithEvents tlpRobotFolders As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblRobots As System.Windows.Forms.Label
    Friend WithEvents chkRobots As System.Windows.Forms.CheckedListBox
    Friend WithEvents lblBackupFolders As System.Windows.Forms.Label
    Friend WithEvents trvFolders As System.Windows.Forms.TreeView
    Friend WithEvents lblDBAccess As System.Windows.Forms.Label
    Friend WithEvents tlpRobotFiles As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lstRobotFiles As System.Windows.Forms.ListBox
    Friend WithEvents pnlRobots As System.Windows.Forms.Panel
    Friend WithEvents cboSourceFileType As System.Windows.Forms.ComboBox
    Friend WithEvents rdoSourceSelectFiles As System.Windows.Forms.RadioButton
    Friend WithEvents rdoSourceAllFiles As System.Windows.Forms.RadioButton
    Friend WithEvents btnBackupToZipRobots As System.Windows.Forms.Button
    Friend WithEvents btnBackupToFolderRobots As System.Windows.Forms.Button
    Friend WithEvents btnExportToCSVDBSql As System.Windows.Forms.Button
    Friend WithEvents btnExportToCSVZIPDBSql As System.Windows.Forms.Button
    Friend WithEvents tabApplication As System.Windows.Forms.TabPage
    Friend WithEvents spltApplication As System.Windows.Forms.SplitContainer
    Friend WithEvents tlpApplFolders As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lstApplication As System.Windows.Forms.ListBox
    Friend WithEvents lblApplication As System.Windows.Forms.Label
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents btnBackupToZipsApplication As System.Windows.Forms.Button
    Friend WithEvents btnBackupToFolderApplication As System.Windows.Forms.Button
    Friend WithEvents TableLayoutPanel3 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents ListBox2 As System.Windows.Forms.ListBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Button5 As System.Windows.Forms.Button
    Friend WithEvents Button6 As System.Windows.Forms.Button
    Friend WithEvents Button7 As System.Windows.Forms.Button
    Friend WithEvents Button8 As System.Windows.Forms.Button
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents SplitContainer3 As System.Windows.Forms.SplitContainer
    Friend WithEvents TableLayoutPanel4 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents ListBox3 As System.Windows.Forms.ListBox
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents Button9 As System.Windows.Forms.Button
    Friend WithEvents Button10 As System.Windows.Forms.Button
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents SplitContainer4 As System.Windows.Forms.SplitContainer
    Friend WithEvents TableLayoutPanel5 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents ListBox4 As System.Windows.Forms.ListBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents Button11 As System.Windows.Forms.Button
    Friend WithEvents Button12 As System.Windows.Forms.Button
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Friend WithEvents SplitContainer5 As System.Windows.Forms.SplitContainer
    Friend WithEvents TableLayoutPanel6 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents ListBox5 As System.Windows.Forms.ListBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Panel6 As System.Windows.Forms.Panel
    Friend WithEvents Button13 As System.Windows.Forms.Button
    Friend WithEvents Button14 As System.Windows.Forms.Button
    Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
    Friend WithEvents SplitContainer6 As System.Windows.Forms.SplitContainer
    Friend WithEvents TableLayoutPanel7 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents CheckedListBox1 As System.Windows.Forms.CheckedListBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents TreeView1 As System.Windows.Forms.TreeView
    Friend WithEvents TableLayoutPanel8 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents ListBox6 As System.Windows.Forms.ListBox
    Friend WithEvents Panel7 As System.Windows.Forms.Panel
    Friend WithEvents ComboBox1 As System.Windows.Forms.ComboBox
    Friend WithEvents RadioButton1 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton2 As System.Windows.Forms.RadioButton
    Friend WithEvents Button15 As System.Windows.Forms.Button
    Friend WithEvents Button16 As System.Windows.Forms.Button
    Friend WithEvents TabPage6 As System.Windows.Forms.TabPage
    Friend WithEvents SplitContainer7 As System.Windows.Forms.SplitContainer
    Friend WithEvents TableLayoutPanel9 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents ListBox7 As System.Windows.Forms.ListBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Panel8 As System.Windows.Forms.Panel
    Friend WithEvents Button17 As System.Windows.Forms.Button
    Friend WithEvents Button18 As System.Windows.Forms.Button
    Friend WithEvents tabDMON As System.Windows.Forms.TabPage
    Friend WithEvents spltDMON As System.Windows.Forms.SplitContainer
    Friend WithEvents tlpDMONArchive As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lstDMONArchive As System.Windows.Forms.ListBox
    Friend WithEvents lblDMONArchive As System.Windows.Forms.Label
    Friend WithEvents pnlDMONArchive As System.Windows.Forms.Panel
    Friend WithEvents btnBackupToZipDMONArchive As System.Windows.Forms.Button
    Friend WithEvents btnBackupToFolderDMONArchive As System.Windows.Forms.Button
    Friend WithEvents tabDMONArchive As System.Windows.Forms.TabPage
    Friend WithEvents spltDMONArchive As System.Windows.Forms.SplitContainer
    Friend WithEvents tlpDMON As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lstDMON As System.Windows.Forms.ListBox
    Friend WithEvents lblDMON As System.Windows.Forms.Label
    Friend WithEvents pnlDMON As System.Windows.Forms.Panel
    Friend WithEvents btnBackupToZipDMON As System.Windows.Forms.Button
    Friend WithEvents btnBackupToFolderDMON As System.Windows.Forms.Button
    Friend WithEvents tabNotepad As System.Windows.Forms.TabPage
    Friend WithEvents spltNotepad As System.Windows.Forms.SplitContainer
    Friend WithEvents pnlNotepad As System.Windows.Forms.Panel
    Friend WithEvents tlpNotepad As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnBackupToZipNotepad As System.Windows.Forms.Button
    Friend WithEvents btnBackupToFolderNotepad As System.Windows.Forms.Button
    Friend WithEvents lblNotepad As System.Windows.Forms.Label
    Friend WithEvents lstNotepad As System.Windows.Forms.ListBox
End Class
