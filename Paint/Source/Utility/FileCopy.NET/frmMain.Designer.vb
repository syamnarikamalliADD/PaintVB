<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
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
        Me.tlpMain = New System.Windows.Forms.TableLayoutPanel
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.tabMain = New System.Windows.Forms.TabControl
        Me.tabSource = New System.Windows.Forms.TabPage
        Me.tlpSource = New System.Windows.Forms.TableLayoutPanel
        Me.btnSourceNext = New System.Windows.Forms.Button
        Me.cboSourceType = New System.Windows.Forms.ComboBox
        Me.pnlSourceRobotDev = New System.Windows.Forms.Panel
        Me.lblSourceRobotDev = New System.Windows.Forms.Label
        Me.cboSourceRobotDev = New System.Windows.Forms.ComboBox
        Me.pnlSourceFileType = New System.Windows.Forms.Panel
        Me.lblSourceFileType = New System.Windows.Forms.Label
        Me.cboSourceFileType = New System.Windows.Forms.ComboBox
        Me.chkSourceRobots = New System.Windows.Forms.CheckedListBox
        Me.mnuCheckList = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuSelectAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuUnselectAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuViewCompare = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrint = New System.Windows.Forms.ToolStripMenuItem
        Me.pnlSourceSelectAll = New System.Windows.Forms.Panel
        Me.rdoSourceSelectFiles = New System.Windows.Forms.RadioButton
        Me.rdoSourceAllFiles = New System.Windows.Forms.RadioButton
        Me.lblSourceType = New System.Windows.Forms.Label
        Me.tabFiles = New System.Windows.Forms.TabPage
        Me.tlpFiles = New System.Windows.Forms.TableLayoutPanel
        Me.lblFileSelect = New System.Windows.Forms.Label
        Me.chkFiles = New System.Windows.Forms.CheckedListBox
        Me.btnFilesNext = New System.Windows.Forms.Button
        Me.btnFilesPrev = New System.Windows.Forms.Button
        Me.tabDest = New System.Windows.Forms.TabPage
        Me.tlpDest = New System.Windows.Forms.TableLayoutPanel
        Me.lblDestRobotDev = New System.Windows.Forms.Label
        Me.cboDestRobotDev = New System.Windows.Forms.ComboBox
        Me.lblDestType = New System.Windows.Forms.Label
        Me.btnDestNext = New System.Windows.Forms.Button
        Me.cboDestType = New System.Windows.Forms.ComboBox
        Me.chkDestRobots = New System.Windows.Forms.CheckedListBox
        Me.btnDestPrev = New System.Windows.Forms.Button
        Me.tabDoCopy = New System.Windows.Forms.TabPage
        Me.tlpDoCopy = New System.Windows.Forms.TableLayoutPanel
        Me.chkOverWriteFiles = New System.Windows.Forms.CheckBox
        Me.lstSummary = New System.Windows.Forms.ListBox
        Me.btnDoCopy = New System.Windows.Forms.Button
        Me.btnDoCopyPrev = New System.Windows.Forms.Button
        Me.lblZone = New System.Windows.Forms.Label
        Me.cboZone = New System.Windows.Forms.ComboBox
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.btnChangeLog = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuLast24 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLast7 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAllChanges = New System.Windows.Forms.ToolStripMenuItem
        Me.btnStatus = New System.Windows.Forms.ToolStripButton
        Me.btnDeleteFiles = New System.Windows.Forms.ToolStripButton
        Me.btnUtilities = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuBackupAllTemp = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuBackupAllTempWithTextFiles = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuBackupAllMaster = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuBackupAllMasterWithTextFiles = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuGetDiag = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuBackupTextFiles = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuCompare = New System.Windows.Forms.ToolStripMenuItem
        Me.cboParam = New System.Windows.Forms.ComboBox
        Me.BottomToolStripPanel = New System.Windows.Forms.ToolStripPanel
        Me.TopToolStripPanel = New System.Windows.Forms.ToolStripPanel
        Me.RightToolStripPanel = New System.Windows.Forms.ToolStripPanel
        Me.LeftToolStripPanel = New System.Windows.Forms.ToolStripPanel
        Me.ContentPanel = New System.Windows.Forms.ToolStripContentPanel
        Me.stsStatus = New System.Windows.Forms.StatusStrip
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel
        Me.tspProgress = New System.Windows.Forms.ToolStripProgressBar
        Me.lblSpacer = New System.Windows.Forms.ToolStripStatusLabel
        Me.btnFunction = New System.Windows.Forms.ToolStripDropDownButton
        Me.mnuLogin = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLogOut = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuRemote = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLocal = New System.Windows.Forms.ToolStripMenuItem
        Me.Button1 = New System.Windows.Forms.Button
        Me.CheckedListBox1 = New System.Windows.Forms.CheckedListBox
        Me.ComboBox1 = New System.Windows.Forms.ComboBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.tscMain.ContentPanel.SuspendLayout()
        Me.tscMain.TopToolStripPanel.SuspendLayout()
        Me.tscMain.SuspendLayout()
        Me.tlpMain.SuspendLayout()
        Me.tabMain.SuspendLayout()
        Me.tabSource.SuspendLayout()
        Me.tlpSource.SuspendLayout()
        Me.pnlSourceRobotDev.SuspendLayout()
        Me.pnlSourceFileType.SuspendLayout()
        Me.mnuCheckList.SuspendLayout()
        Me.pnlSourceSelectAll.SuspendLayout()
        Me.tabFiles.SuspendLayout()
        Me.tlpFiles.SuspendLayout()
        Me.tabDest.SuspendLayout()
        Me.tlpDest.SuspendLayout()
        Me.tabDoCopy.SuspendLayout()
        Me.tlpDoCopy.SuspendLayout()
        Me.tlsMain.SuspendLayout()
        Me.stsStatus.SuspendLayout()
        Me.SuspendLayout()
        '
        'tscMain
        '
        '
        'tscMain.ContentPanel
        '
        Me.tscMain.ContentPanel.Controls.Add(Me.tlpMain)
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
        'tlpMain
        '
        resources.ApplyResources(Me.tlpMain, "tlpMain")
        Me.tlpMain.Controls.Add(Me.lstStatus, 1, 0)
        Me.tlpMain.Controls.Add(Me.tabMain, 0, 2)
        Me.tlpMain.Controls.Add(Me.lblZone, 0, 0)
        Me.tlpMain.Controls.Add(Me.cboZone, 0, 1)
        Me.tlpMain.Name = "tlpMain"
        '
        'lstStatus
        '
        Me.lstStatus.BackColor = System.Drawing.SystemColors.Info
        resources.ApplyResources(Me.lstStatus, "lstStatus")
        Me.lstStatus.Name = "lstStatus"
        Me.tlpMain.SetRowSpan(Me.lstStatus, 2)
        '
        'tabMain
        '
        Me.tlpMain.SetColumnSpan(Me.tabMain, 2)
        Me.tabMain.Controls.Add(Me.tabSource)
        Me.tabMain.Controls.Add(Me.tabFiles)
        Me.tabMain.Controls.Add(Me.tabDest)
        Me.tabMain.Controls.Add(Me.tabDoCopy)
        resources.ApplyResources(Me.tabMain, "tabMain")
        Me.tabMain.Name = "tabMain"
        Me.tabMain.SelectedIndex = 0
        '
        'tabSource
        '
        resources.ApplyResources(Me.tabSource, "tabSource")
        Me.tabSource.Controls.Add(Me.tlpSource)
        Me.tabSource.ForeColor = System.Drawing.SystemColors.ControlText
        Me.tabSource.Name = "tabSource"
        Me.tabSource.UseVisualStyleBackColor = True
        '
        'tlpSource
        '
        resources.ApplyResources(Me.tlpSource, "tlpSource")
        Me.tlpSource.Controls.Add(Me.btnSourceNext, 2, 4)
        Me.tlpSource.Controls.Add(Me.cboSourceType, 0, 1)
        Me.tlpSource.Controls.Add(Me.pnlSourceRobotDev, 3, 0)
        Me.tlpSource.Controls.Add(Me.pnlSourceFileType, 1, 2)
        Me.tlpSource.Controls.Add(Me.chkSourceRobots, 0, 2)
        Me.tlpSource.Controls.Add(Me.pnlSourceSelectAll, 1, 0)
        Me.tlpSource.Controls.Add(Me.lblSourceType, 0, 0)
        Me.tlpSource.Name = "tlpSource"
        '
        'btnSourceNext
        '
        resources.ApplyResources(Me.btnSourceNext, "btnSourceNext")
        Me.btnSourceNext.Name = "btnSourceNext"
        Me.btnSourceNext.UseVisualStyleBackColor = True
        '
        'cboSourceType
        '
        resources.ApplyResources(Me.cboSourceType, "cboSourceType")
        Me.cboSourceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSourceType.Name = "cboSourceType"
        '
        'pnlSourceRobotDev
        '
        Me.pnlSourceRobotDev.Controls.Add(Me.lblSourceRobotDev)
        Me.pnlSourceRobotDev.Controls.Add(Me.cboSourceRobotDev)
        resources.ApplyResources(Me.pnlSourceRobotDev, "pnlSourceRobotDev")
        Me.pnlSourceRobotDev.Name = "pnlSourceRobotDev"
        Me.tlpSource.SetRowSpan(Me.pnlSourceRobotDev, 4)
        '
        'lblSourceRobotDev
        '
        resources.ApplyResources(Me.lblSourceRobotDev, "lblSourceRobotDev")
        Me.lblSourceRobotDev.Name = "lblSourceRobotDev"
        '
        'cboSourceRobotDev
        '
        resources.ApplyResources(Me.cboSourceRobotDev, "cboSourceRobotDev")
        Me.cboSourceRobotDev.FormattingEnabled = True
        Me.cboSourceRobotDev.MaximumSize = New System.Drawing.Size(200, 0)
        Me.cboSourceRobotDev.Name = "cboSourceRobotDev"
        '
        'pnlSourceFileType
        '
        Me.tlpSource.SetColumnSpan(Me.pnlSourceFileType, 2)
        Me.pnlSourceFileType.Controls.Add(Me.lblSourceFileType)
        Me.pnlSourceFileType.Controls.Add(Me.cboSourceFileType)
        resources.ApplyResources(Me.pnlSourceFileType, "pnlSourceFileType")
        Me.pnlSourceFileType.Name = "pnlSourceFileType"
        Me.tlpSource.SetRowSpan(Me.pnlSourceFileType, 2)
        '
        'lblSourceFileType
        '
        resources.ApplyResources(Me.lblSourceFileType, "lblSourceFileType")
        Me.lblSourceFileType.Name = "lblSourceFileType"
        '
        'cboSourceFileType
        '
        resources.ApplyResources(Me.cboSourceFileType, "cboSourceFileType")
        Me.cboSourceFileType.FormattingEnabled = True
        Me.cboSourceFileType.Name = "cboSourceFileType"
        '
        'chkSourceRobots
        '
        Me.chkSourceRobots.CheckOnClick = True
        Me.chkSourceRobots.ContextMenuStrip = Me.mnuCheckList
        resources.ApplyResources(Me.chkSourceRobots, "chkSourceRobots")
        Me.chkSourceRobots.FormattingEnabled = True
        Me.chkSourceRobots.Name = "chkSourceRobots"
        Me.tlpSource.SetRowSpan(Me.chkSourceRobots, 2)
        '
        'mnuCheckList
        '
        Me.mnuCheckList.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuSelectAll, Me.mnuUnselectAll, Me.mnuViewCompare, Me.mnuPrint})
        Me.mnuCheckList.Name = "mnuCheckList"
        resources.ApplyResources(Me.mnuCheckList, "mnuCheckList")
        '
        'mnuSelectAll
        '
        Me.mnuSelectAll.Name = "mnuSelectAll"
        resources.ApplyResources(Me.mnuSelectAll, "mnuSelectAll")
        '
        'mnuUnselectAll
        '
        Me.mnuUnselectAll.Name = "mnuUnselectAll"
        resources.ApplyResources(Me.mnuUnselectAll, "mnuUnselectAll")
        '
        'mnuViewCompare
        '
        Me.mnuViewCompare.Name = "mnuViewCompare"
        resources.ApplyResources(Me.mnuViewCompare, "mnuViewCompare")
        '
        'mnuPrint
        '
        Me.mnuPrint.Name = "mnuPrint"
        resources.ApplyResources(Me.mnuPrint, "mnuPrint")
        '
        'pnlSourceSelectAll
        '
        Me.tlpSource.SetColumnSpan(Me.pnlSourceSelectAll, 2)
        Me.pnlSourceSelectAll.Controls.Add(Me.rdoSourceSelectFiles)
        Me.pnlSourceSelectAll.Controls.Add(Me.rdoSourceAllFiles)
        resources.ApplyResources(Me.pnlSourceSelectAll, "pnlSourceSelectAll")
        Me.pnlSourceSelectAll.Name = "pnlSourceSelectAll"
        Me.tlpSource.SetRowSpan(Me.pnlSourceSelectAll, 2)
        '
        'rdoSourceSelectFiles
        '
        resources.ApplyResources(Me.rdoSourceSelectFiles, "rdoSourceSelectFiles")
        Me.rdoSourceSelectFiles.Name = "rdoSourceSelectFiles"
        Me.rdoSourceSelectFiles.UseVisualStyleBackColor = True
        '
        'rdoSourceAllFiles
        '
        resources.ApplyResources(Me.rdoSourceAllFiles, "rdoSourceAllFiles")
        Me.rdoSourceAllFiles.Checked = True
        Me.rdoSourceAllFiles.Name = "rdoSourceAllFiles"
        Me.rdoSourceAllFiles.TabStop = True
        Me.rdoSourceAllFiles.UseVisualStyleBackColor = True
        '
        'lblSourceType
        '
        resources.ApplyResources(Me.lblSourceType, "lblSourceType")
        Me.lblSourceType.Name = "lblSourceType"
        '
        'tabFiles
        '
        Me.tabFiles.Controls.Add(Me.tlpFiles)
        resources.ApplyResources(Me.tabFiles, "tabFiles")
        Me.tabFiles.Name = "tabFiles"
        Me.tabFiles.UseVisualStyleBackColor = True
        '
        'tlpFiles
        '
        resources.ApplyResources(Me.tlpFiles, "tlpFiles")
        Me.tlpFiles.Controls.Add(Me.lblFileSelect, 0, 0)
        Me.tlpFiles.Controls.Add(Me.chkFiles, 0, 1)
        Me.tlpFiles.Controls.Add(Me.btnFilesNext, 2, 2)
        Me.tlpFiles.Controls.Add(Me.btnFilesPrev, 1, 2)
        Me.tlpFiles.Name = "tlpFiles"
        '
        'lblFileSelect
        '
        Me.tlpFiles.SetColumnSpan(Me.lblFileSelect, 4)
        resources.ApplyResources(Me.lblFileSelect, "lblFileSelect")
        Me.lblFileSelect.Name = "lblFileSelect"
        '
        'chkFiles
        '
        Me.chkFiles.CheckOnClick = True
        Me.tlpFiles.SetColumnSpan(Me.chkFiles, 4)
        Me.chkFiles.ContextMenuStrip = Me.mnuCheckList
        resources.ApplyResources(Me.chkFiles, "chkFiles")
        Me.chkFiles.FormattingEnabled = True
        Me.chkFiles.MultiColumn = True
        Me.chkFiles.Name = "chkFiles"
        '
        'btnFilesNext
        '
        resources.ApplyResources(Me.btnFilesNext, "btnFilesNext")
        Me.btnFilesNext.Name = "btnFilesNext"
        Me.btnFilesNext.UseVisualStyleBackColor = True
        '
        'btnFilesPrev
        '
        resources.ApplyResources(Me.btnFilesPrev, "btnFilesPrev")
        Me.btnFilesPrev.Name = "btnFilesPrev"
        Me.btnFilesPrev.UseVisualStyleBackColor = True
        '
        'tabDest
        '
        Me.tabDest.Controls.Add(Me.tlpDest)
        resources.ApplyResources(Me.tabDest, "tabDest")
        Me.tabDest.Name = "tabDest"
        Me.tabDest.UseVisualStyleBackColor = True
        '
        'tlpDest
        '
        resources.ApplyResources(Me.tlpDest, "tlpDest")
        Me.tlpDest.Controls.Add(Me.lblDestRobotDev, 3, 1)
        Me.tlpDest.Controls.Add(Me.cboDestRobotDev, 3, 0)
        Me.tlpDest.Controls.Add(Me.lblDestType, 0, 0)
        Me.tlpDest.Controls.Add(Me.btnDestNext, 2, 3)
        Me.tlpDest.Controls.Add(Me.cboDestType, 0, 1)
        Me.tlpDest.Controls.Add(Me.chkDestRobots, 0, 2)
        Me.tlpDest.Controls.Add(Me.btnDestPrev, 1, 3)
        Me.tlpDest.Name = "tlpDest"
        '
        'lblDestRobotDev
        '
        resources.ApplyResources(Me.lblDestRobotDev, "lblDestRobotDev")
        Me.lblDestRobotDev.Name = "lblDestRobotDev"
        Me.tlpDest.SetRowSpan(Me.lblDestRobotDev, 2)
        '
        'cboDestRobotDev
        '
        resources.ApplyResources(Me.cboDestRobotDev, "cboDestRobotDev")
        Me.cboDestRobotDev.FormattingEnabled = True
        Me.cboDestRobotDev.MaximumSize = New System.Drawing.Size(200, 0)
        Me.cboDestRobotDev.Name = "cboDestRobotDev"
        '
        'lblDestType
        '
        resources.ApplyResources(Me.lblDestType, "lblDestType")
        Me.lblDestType.Name = "lblDestType"
        '
        'btnDestNext
        '
        resources.ApplyResources(Me.btnDestNext, "btnDestNext")
        Me.btnDestNext.Name = "btnDestNext"
        Me.btnDestNext.UseVisualStyleBackColor = True
        '
        'cboDestType
        '
        resources.ApplyResources(Me.cboDestType, "cboDestType")
        Me.cboDestType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboDestType.FormattingEnabled = True
        Me.cboDestType.Name = "cboDestType"
        '
        'chkDestRobots
        '
        Me.chkDestRobots.CheckOnClick = True
        Me.chkDestRobots.ContextMenuStrip = Me.mnuCheckList
        resources.ApplyResources(Me.chkDestRobots, "chkDestRobots")
        Me.chkDestRobots.FormattingEnabled = True
        Me.chkDestRobots.Name = "chkDestRobots"
        '
        'btnDestPrev
        '
        resources.ApplyResources(Me.btnDestPrev, "btnDestPrev")
        Me.btnDestPrev.Name = "btnDestPrev"
        Me.btnDestPrev.UseVisualStyleBackColor = True
        '
        'tabDoCopy
        '
        Me.tabDoCopy.Controls.Add(Me.tlpDoCopy)
        resources.ApplyResources(Me.tabDoCopy, "tabDoCopy")
        Me.tabDoCopy.Name = "tabDoCopy"
        Me.tabDoCopy.UseVisualStyleBackColor = True
        '
        'tlpDoCopy
        '
        resources.ApplyResources(Me.tlpDoCopy, "tlpDoCopy")
        Me.tlpDoCopy.Controls.Add(Me.chkOverWriteFiles, 3, 1)
        Me.tlpDoCopy.Controls.Add(Me.lstSummary, 0, 0)
        Me.tlpDoCopy.Controls.Add(Me.btnDoCopy, 2, 1)
        Me.tlpDoCopy.Controls.Add(Me.btnDoCopyPrev, 1, 1)
        Me.tlpDoCopy.Name = "tlpDoCopy"
        '
        'chkOverWriteFiles
        '
        resources.ApplyResources(Me.chkOverWriteFiles, "chkOverWriteFiles")
        Me.chkOverWriteFiles.Name = "chkOverWriteFiles"
        Me.chkOverWriteFiles.UseVisualStyleBackColor = True
        '
        'lstSummary
        '
        Me.lstSummary.BackColor = System.Drawing.SystemColors.Info
        Me.tlpDoCopy.SetColumnSpan(Me.lstSummary, 4)
        resources.ApplyResources(Me.lstSummary, "lstSummary")
        Me.lstSummary.Name = "lstSummary"
        Me.lstSummary.SelectionMode = System.Windows.Forms.SelectionMode.None
        '
        'btnDoCopy
        '
        resources.ApplyResources(Me.btnDoCopy, "btnDoCopy")
        Me.btnDoCopy.Name = "btnDoCopy"
        Me.btnDoCopy.UseVisualStyleBackColor = True
        '
        'btnDoCopyPrev
        '
        resources.ApplyResources(Me.btnDoCopyPrev, "btnDoCopyPrev")
        Me.btnDoCopyPrev.Name = "btnDoCopyPrev"
        Me.btnDoCopyPrev.UseVisualStyleBackColor = True
        '
        'lblZone
        '
        resources.ApplyResources(Me.lblZone, "lblZone")
        Me.lblZone.Name = "lblZone"
        '
        'cboZone
        '
        resources.ApplyResources(Me.cboZone, "cboZone")
        Me.cboZone.FormattingEnabled = True
        Me.cboZone.Name = "cboZone"
        '
        'tlsMain
        '
        resources.ApplyResources(Me.tlsMain, "tlsMain")
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.ToolStripSeparator1, Me.btnChangeLog, Me.btnStatus, Me.btnDeleteFiles, Me.btnUtilities})
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
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
        '
        'btnChangeLog
        '
        Me.btnChangeLog.DropDownButtonWidth = 13
        Me.btnChangeLog.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuLast24, Me.mnuLast7, Me.mnuAllChanges})
        resources.ApplyResources(Me.btnChangeLog, "btnChangeLog")
        Me.btnChangeLog.Name = "btnChangeLog"
        '
        'mnuLast24
        '
        Me.mnuLast24.Name = "mnuLast24"
        resources.ApplyResources(Me.mnuLast24, "mnuLast24")
        '
        'mnuLast7
        '
        Me.mnuLast7.Name = "mnuLast7"
        resources.ApplyResources(Me.mnuLast7, "mnuLast7")
        '
        'mnuAllChanges
        '
        Me.mnuAllChanges.Name = "mnuAllChanges"
        resources.ApplyResources(Me.mnuAllChanges, "mnuAllChanges")
        '
        'btnStatus
        '
        resources.ApplyResources(Me.btnStatus, "btnStatus")
        Me.btnStatus.CheckOnClick = True
        Me.btnStatus.Name = "btnStatus"
        '
        'btnDeleteFiles
        '
        Me.btnDeleteFiles.CheckOnClick = True
        resources.ApplyResources(Me.btnDeleteFiles, "btnDeleteFiles")
        Me.btnDeleteFiles.Name = "btnDeleteFiles"
        '
        'btnUtilities
        '
        Me.btnUtilities.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuBackupAllTemp, Me.mnuBackupAllTempWithTextFiles, Me.mnuBackupAllMaster, Me.mnuBackupAllMasterWithTextFiles, Me.mnuGetDiag, Me.mnuBackupTextFiles, Me.mnuCompare})
        resources.ApplyResources(Me.btnUtilities, "btnUtilities")
        Me.btnUtilities.Name = "btnUtilities"
        '
        'mnuBackupAllTemp
        '
        Me.mnuBackupAllTemp.Name = "mnuBackupAllTemp"
        resources.ApplyResources(Me.mnuBackupAllTemp, "mnuBackupAllTemp")
        '
        'mnuBackupAllTempWithTextFiles
        '
        Me.mnuBackupAllTempWithTextFiles.Name = "mnuBackupAllTempWithTextFiles"
        resources.ApplyResources(Me.mnuBackupAllTempWithTextFiles, "mnuBackupAllTempWithTextFiles")
        '
        'mnuBackupAllMaster
        '
        Me.mnuBackupAllMaster.Name = "mnuBackupAllMaster"
        resources.ApplyResources(Me.mnuBackupAllMaster, "mnuBackupAllMaster")
        '
        'mnuBackupAllMasterWithTextFiles
        '
        Me.mnuBackupAllMasterWithTextFiles.Name = "mnuBackupAllMasterWithTextFiles"
        resources.ApplyResources(Me.mnuBackupAllMasterWithTextFiles, "mnuBackupAllMasterWithTextFiles")
        '
        'mnuGetDiag
        '
        Me.mnuGetDiag.Name = "mnuGetDiag"
        resources.ApplyResources(Me.mnuGetDiag, "mnuGetDiag")
        '
        'mnuBackupTextFiles
        '
        Me.mnuBackupTextFiles.Name = "mnuBackupTextFiles"
        resources.ApplyResources(Me.mnuBackupTextFiles, "mnuBackupTextFiles")
        '
        'mnuCompare
        '
        Me.mnuCompare.Name = "mnuCompare"
        resources.ApplyResources(Me.mnuCompare, "mnuCompare")
        '
        'cboParam
        '
        Me.cboParam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboParam, "cboParam")
        Me.cboParam.Name = "cboParam"
        '
        'BottomToolStripPanel
        '
        resources.ApplyResources(Me.BottomToolStripPanel, "BottomToolStripPanel")
        Me.BottomToolStripPanel.Name = "BottomToolStripPanel"
        Me.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.BottomToolStripPanel.RowMargin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        '
        'TopToolStripPanel
        '
        resources.ApplyResources(Me.TopToolStripPanel, "TopToolStripPanel")
        Me.TopToolStripPanel.Name = "TopToolStripPanel"
        Me.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.TopToolStripPanel.RowMargin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        '
        'RightToolStripPanel
        '
        resources.ApplyResources(Me.RightToolStripPanel, "RightToolStripPanel")
        Me.RightToolStripPanel.Name = "RightToolStripPanel"
        Me.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.RightToolStripPanel.RowMargin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        '
        'LeftToolStripPanel
        '
        resources.ApplyResources(Me.LeftToolStripPanel, "LeftToolStripPanel")
        Me.LeftToolStripPanel.Name = "LeftToolStripPanel"
        Me.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.LeftToolStripPanel.RowMargin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        '
        'ContentPanel
        '
        resources.ApplyResources(Me.ContentPanel, "ContentPanel")
        '
        'stsStatus
        '
        resources.ApplyResources(Me.stsStatus, "stsStatus")
        Me.stsStatus.GripMargin = New System.Windows.Forms.Padding(0)
        Me.stsStatus.ImageScalingSize = New System.Drawing.Size(22, 22)
        Me.stsStatus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatus, Me.tspProgress, Me.lblSpacer, Me.btnFunction})
        Me.stsStatus.Name = "stsStatus"
        Me.stsStatus.ShowItemToolTips = True
        '
        'lblStatus
        '
        Me.lblStatus.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedOuter
        resources.ApplyResources(Me.lblStatus, "lblStatus")
        Me.lblStatus.Margin = New System.Windows.Forms.Padding(0)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Spring = True
        '
        'tspProgress
        '
        Me.tspProgress.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.tspProgress.Margin = New System.Windows.Forms.Padding(2, 3, 1, 3)
        Me.tspProgress.Name = "tspProgress"
        Me.tspProgress.Padding = New System.Windows.Forms.Padding(2)
        resources.ApplyResources(Me.tspProgress, "tspProgress")
        Me.tspProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        '
        'lblSpacer
        '
        resources.ApplyResources(Me.lblSpacer, "lblSpacer")
        Me.lblSpacer.Name = "lblSpacer"
        '
        'btnFunction
        '
        Me.btnFunction.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.btnFunction.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnFunction.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuLogin, Me.mnuLogOut, Me.mnuRemote, Me.mnuLocal})
        resources.ApplyResources(Me.btnFunction, "btnFunction")
        Me.btnFunction.Name = "btnFunction"
        Me.btnFunction.Padding = New System.Windows.Forms.Padding(2, 0, 2, 0)
        '
        'mnuLogin
        '
        Me.mnuLogin.Name = "mnuLogin"
        resources.ApplyResources(Me.mnuLogin, "mnuLogin")
        '
        'mnuLogOut
        '
        Me.mnuLogOut.Name = "mnuLogOut"
        resources.ApplyResources(Me.mnuLogOut, "mnuLogOut")
        '
        'mnuRemote
        '
        Me.mnuRemote.Name = "mnuRemote"
        resources.ApplyResources(Me.mnuRemote, "mnuRemote")
        '
        'mnuLocal
        '
        Me.mnuLocal.Name = "mnuLocal"
        resources.ApplyResources(Me.mnuLocal, "mnuLocal")
        '
        'Button1
        '
        resources.ApplyResources(Me.Button1, "Button1")
        Me.Button1.Name = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'CheckedListBox1
        '
        Me.CheckedListBox1.ContextMenuStrip = Me.mnuCheckList
        Me.CheckedListBox1.FormattingEnabled = True
        resources.ApplyResources(Me.CheckedListBox1, "CheckedListBox1")
        Me.CheckedListBox1.MaximumSize = New System.Drawing.Size(215, 364)
        Me.CheckedListBox1.Name = "CheckedListBox1"
        '
        'ComboBox1
        '
        Me.ComboBox1.FormattingEnabled = True
        resources.ApplyResources(Me.ComboBox1, "ComboBox1")
        Me.ComboBox1.Name = "ComboBox1"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'frmMain
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.stsStatus)
        Me.Controls.Add(Me.tscMain)
        Me.Name = "frmMain"
        Me.tscMain.ContentPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.PerformLayout()
        Me.tscMain.ResumeLayout(False)
        Me.tscMain.PerformLayout()
        Me.tlpMain.ResumeLayout(False)
        Me.tabMain.ResumeLayout(False)
        Me.tabSource.ResumeLayout(False)
        Me.tlpSource.ResumeLayout(False)
        Me.pnlSourceRobotDev.ResumeLayout(False)
        Me.pnlSourceFileType.ResumeLayout(False)
        Me.mnuCheckList.ResumeLayout(False)
        Me.pnlSourceSelectAll.ResumeLayout(False)
        Me.pnlSourceSelectAll.PerformLayout()
        Me.tabFiles.ResumeLayout(False)
        Me.tlpFiles.ResumeLayout(False)
        Me.tabDest.ResumeLayout(False)
        Me.tlpDest.ResumeLayout(False)
        Me.tabDoCopy.ResumeLayout(False)
        Me.tlpDoCopy.ResumeLayout(False)
        Me.tlpDoCopy.PerformLayout()
        Me.tlsMain.ResumeLayout(False)
        Me.tlsMain.PerformLayout()
        Me.stsStatus.ResumeLayout(False)
        Me.stsStatus.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents cboParam As System.Windows.Forms.ComboBox
    Friend WithEvents tscMain As System.Windows.Forms.ToolStripContainer
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnClose As System.Windows.Forms.ToolStripButton
    Friend WithEvents stsStatus As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tspProgress As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents lblSpacer As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents btnFunction As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents mnuLogin As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLogOut As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuRemote As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLocal As System.Windows.Forms.ToolStripMenuItem
    Public WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuCheckList As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuSelectAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuUnselectAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnChangeLog As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuLast24 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLast7 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAllChanges As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BottomToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents TopToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents RightToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents LeftToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents ContentPanel As System.Windows.Forms.ToolStripContentPanel
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents CheckedListBox1 As System.Windows.Forms.CheckedListBox
    Friend WithEvents ComboBox1 As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents tlpMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents tabMain As System.Windows.Forms.TabControl
    Friend WithEvents tabSource As System.Windows.Forms.TabPage
    Friend WithEvents btnSourceNext As System.Windows.Forms.Button
    Friend WithEvents pnlSourceFileType As System.Windows.Forms.Panel
    Friend WithEvents lblSourceFileType As System.Windows.Forms.Label
    Friend WithEvents cboSourceFileType As System.Windows.Forms.ComboBox
    Friend WithEvents pnlSourceRobotDev As System.Windows.Forms.Panel
    Friend WithEvents lblSourceRobotDev As System.Windows.Forms.Label
    Friend WithEvents pnlSourceSelectAll As System.Windows.Forms.Panel
    Friend WithEvents rdoSourceSelectFiles As System.Windows.Forms.RadioButton
    Friend WithEvents rdoSourceAllFiles As System.Windows.Forms.RadioButton
    Friend WithEvents chkSourceRobots As System.Windows.Forms.CheckedListBox
    Friend WithEvents lblSourceType As System.Windows.Forms.Label
    Friend WithEvents cboSourceType As System.Windows.Forms.ComboBox
    Friend WithEvents tabFiles As System.Windows.Forms.TabPage
    Friend WithEvents btnFilesPrev As System.Windows.Forms.Button
    Friend WithEvents btnFilesNext As System.Windows.Forms.Button
    Friend WithEvents tabDest As System.Windows.Forms.TabPage
    Friend WithEvents btnDestNext As System.Windows.Forms.Button
    Friend WithEvents tabDoCopy As System.Windows.Forms.TabPage
    Friend WithEvents btnDoCopyPrev As System.Windows.Forms.Button
    Friend WithEvents btnDoCopy As System.Windows.Forms.Button
    Friend WithEvents lstStatus As System.Windows.Forms.ListBox
    Friend WithEvents tlpDoCopy As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents tlpDest As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblDestType As System.Windows.Forms.Label
    Friend WithEvents cboDestType As System.Windows.Forms.ComboBox
    Friend WithEvents chkDestRobots As System.Windows.Forms.CheckedListBox
    Friend WithEvents btnDestPrev As System.Windows.Forms.Button
    Friend WithEvents tlpFiles As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents tlpSource As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents chkFiles As System.Windows.Forms.CheckedListBox
    Friend WithEvents lblZone As System.Windows.Forms.Label
    Friend WithEvents cboZone As System.Windows.Forms.ComboBox
    Friend WithEvents btnStatus As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnUtilities As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuBackupAllTemp As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuGetDiag As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lstSummary As System.Windows.Forms.ListBox
    Friend WithEvents btnDeleteFiles As System.Windows.Forms.ToolStripButton
    Friend WithEvents lblFileSelect As System.Windows.Forms.Label
    Friend WithEvents chkOverWriteFiles As System.Windows.Forms.CheckBox
    Friend WithEvents mnuViewCompare As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrint As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblDestRobotDev As System.Windows.Forms.Label
    Friend WithEvents cboDestRobotDev As System.Windows.Forms.ComboBox
    Friend WithEvents mnuBackupAllMaster As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuBackupTextFiles As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuCompare As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents cboSourceRobotDev As System.Windows.Forms.ComboBox
    Friend WithEvents mnuBackupAllTempWithTextFiles As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuBackupAllMasterWithTextFiles As System.Windows.Forms.ToolStripMenuItem
End Class
