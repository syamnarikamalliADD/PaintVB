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
        Me.tlpDoc = New System.Windows.Forms.TableLayoutPanel
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.splMain = New System.Windows.Forms.SplitContainer
        Me.tlpFolders = New System.Windows.Forms.TableLayoutPanel
        Me.lblFolders = New System.Windows.Forms.Label
        Me.chkFolders = New System.Windows.Forms.CheckedListBox
        Me.mnuListMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuSelectAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuUnselectAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuRemoveItem = New System.Windows.Forms.ToolStripMenuItem
        Me.tlpFiles = New System.Windows.Forms.TableLayoutPanel
        Me.lblFiles = New System.Windows.Forms.Label
        Me.dgFiles = New System.Windows.Forms.DataGridView
        Me.pnlFilter = New System.Windows.Forms.Panel
        Me.chkAfter = New System.Windows.Forms.CheckBox
        Me.chkBefore = New System.Windows.Forms.CheckBox
        Me.btnClearFilters = New System.Windows.Forms.Button
        Me.lblBefore = New System.Windows.Forms.Label
        Me.dtBeforeTime = New System.Windows.Forms.DateTimePicker
        Me.dtBeforeDate = New System.Windows.Forms.DateTimePicker
        Me.lblAfter = New System.Windows.Forms.Label
        Me.dtAfterTime = New System.Windows.Forms.DateTimePicker
        Me.dtAfterDate = New System.Windows.Forms.DateTimePicker
        Me.btnApplyFilter = New System.Windows.Forms.Button
        Me.txtvin = New System.Windows.Forms.TextBox
        Me.lblVin = New System.Windows.Forms.Label
        Me.cboOption = New System.Windows.Forms.ComboBox
        Me.lblOption = New System.Windows.Forms.Label
        Me.cboColor = New System.Windows.Forms.ComboBox
        Me.lblColor = New System.Windows.Forms.Label
        Me.cboStyle = New System.Windows.Forms.ComboBox
        Me.lblStyle = New System.Windows.Forms.Label
        Me.cboDevice = New System.Windows.Forms.ComboBox
        Me.lblDevice = New System.Windows.Forms.Label
        Me.cboZone = New System.Windows.Forms.ComboBox
        Me.lblZone = New System.Windows.Forms.Label
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.btnOpen = New System.Windows.Forms.ToolStripDropDownButton
        Me.mnuAddFolder = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAddZip = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuOpenDmonFile = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuImportCfg = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuExportCfg = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator
        Me.btnRefresh = New System.Windows.Forms.ToolStripButton
        Me.btnShow = New System.Windows.Forms.ToolStripButton
        Me.btnHide = New System.Windows.Forms.ToolStripButton
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
        Me.tscMain.ContentPanel.SuspendLayout()
        Me.tscMain.TopToolStripPanel.SuspendLayout()
        Me.tscMain.SuspendLayout()
        Me.tlpDoc.SuspendLayout()
        Me.splMain.Panel1.SuspendLayout()
        Me.splMain.Panel2.SuspendLayout()
        Me.splMain.SuspendLayout()
        Me.tlpFolders.SuspendLayout()
        Me.mnuListMenu.SuspendLayout()
        Me.tlpFiles.SuspendLayout()
        CType(Me.dgFiles, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlFilter.SuspendLayout()
        Me.tlsMain.SuspendLayout()
        Me.stsStatus.SuspendLayout()
        Me.SuspendLayout()
        '
        'tscMain
        '
        '
        'tscMain.ContentPanel
        '
        Me.tscMain.ContentPanel.Controls.Add(Me.tlpDoc)
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
        'tlpDoc
        '
        resources.ApplyResources(Me.tlpDoc, "tlpDoc")
        Me.tlpDoc.Controls.Add(Me.lstStatus, 0, 0)
        Me.tlpDoc.Controls.Add(Me.splMain, 0, 2)
        Me.tlpDoc.Controls.Add(Me.pnlFilter, 0, 1)
        Me.tlpDoc.Name = "tlpDoc"
        '
        'lstStatus
        '
        Me.lstStatus.BackColor = System.Drawing.SystemColors.Info
        resources.ApplyResources(Me.lstStatus, "lstStatus")
        Me.lstStatus.Name = "lstStatus"
        '
        'splMain
        '
        resources.ApplyResources(Me.splMain, "splMain")
        Me.splMain.Name = "splMain"
        '
        'splMain.Panel1
        '
        Me.splMain.Panel1.Controls.Add(Me.tlpFolders)
        '
        'splMain.Panel2
        '
        Me.splMain.Panel2.Controls.Add(Me.tlpFiles)
        '
        'tlpFolders
        '
        resources.ApplyResources(Me.tlpFolders, "tlpFolders")
        Me.tlpFolders.Controls.Add(Me.lblFolders, 0, 0)
        Me.tlpFolders.Controls.Add(Me.chkFolders, 0, 1)
        Me.tlpFolders.Name = "tlpFolders"
        '
        'lblFolders
        '
        resources.ApplyResources(Me.lblFolders, "lblFolders")
        Me.lblFolders.Name = "lblFolders"
        '
        'chkFolders
        '
        Me.chkFolders.ContextMenuStrip = Me.mnuListMenu
        resources.ApplyResources(Me.chkFolders, "chkFolders")
        Me.chkFolders.FormattingEnabled = True
        Me.chkFolders.Name = "chkFolders"
        '
        'mnuListMenu
        '
        Me.mnuListMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuSelectAll, Me.mnuUnselectAll, Me.mnuRemoveItem})
        Me.mnuListMenu.Name = "mnuCheckList"
        resources.ApplyResources(Me.mnuListMenu, "mnuListMenu")
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
        'mnuRemoveItem
        '
        Me.mnuRemoveItem.Name = "mnuRemoveItem"
        resources.ApplyResources(Me.mnuRemoveItem, "mnuRemoveItem")
        '
        'tlpFiles
        '
        resources.ApplyResources(Me.tlpFiles, "tlpFiles")
        Me.tlpFiles.Controls.Add(Me.lblFiles, 0, 0)
        Me.tlpFiles.Controls.Add(Me.dgFiles, 0, 1)
        Me.tlpFiles.Name = "tlpFiles"
        '
        'lblFiles
        '
        resources.ApplyResources(Me.lblFiles, "lblFiles")
        Me.lblFiles.Name = "lblFiles"
        '
        'dgFiles
        '
        Me.dgFiles.AllowUserToAddRows = False
        Me.dgFiles.AllowUserToDeleteRows = False
        Me.dgFiles.AllowUserToOrderColumns = True
        Me.dgFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.dgFiles, "dgFiles")
        Me.dgFiles.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.dgFiles.MultiSelect = False
        Me.dgFiles.Name = "dgFiles"
        Me.dgFiles.ReadOnly = True
        Me.dgFiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        '
        'pnlFilter
        '
        resources.ApplyResources(Me.pnlFilter, "pnlFilter")
        Me.pnlFilter.Controls.Add(Me.chkAfter)
        Me.pnlFilter.Controls.Add(Me.chkBefore)
        Me.pnlFilter.Controls.Add(Me.btnClearFilters)
        Me.pnlFilter.Controls.Add(Me.lblBefore)
        Me.pnlFilter.Controls.Add(Me.dtBeforeTime)
        Me.pnlFilter.Controls.Add(Me.dtBeforeDate)
        Me.pnlFilter.Controls.Add(Me.lblAfter)
        Me.pnlFilter.Controls.Add(Me.dtAfterTime)
        Me.pnlFilter.Controls.Add(Me.dtAfterDate)
        Me.pnlFilter.Controls.Add(Me.btnApplyFilter)
        Me.pnlFilter.Controls.Add(Me.txtvin)
        Me.pnlFilter.Controls.Add(Me.lblVin)
        Me.pnlFilter.Controls.Add(Me.cboOption)
        Me.pnlFilter.Controls.Add(Me.lblOption)
        Me.pnlFilter.Controls.Add(Me.cboColor)
        Me.pnlFilter.Controls.Add(Me.lblColor)
        Me.pnlFilter.Controls.Add(Me.cboStyle)
        Me.pnlFilter.Controls.Add(Me.lblStyle)
        Me.pnlFilter.Controls.Add(Me.cboDevice)
        Me.pnlFilter.Controls.Add(Me.lblDevice)
        Me.pnlFilter.Controls.Add(Me.cboZone)
        Me.pnlFilter.Controls.Add(Me.lblZone)
        Me.pnlFilter.Name = "pnlFilter"
        '
        'chkAfter
        '
        resources.ApplyResources(Me.chkAfter, "chkAfter")
        Me.chkAfter.Name = "chkAfter"
        Me.chkAfter.UseVisualStyleBackColor = True
        '
        'chkBefore
        '
        resources.ApplyResources(Me.chkBefore, "chkBefore")
        Me.chkBefore.Name = "chkBefore"
        Me.chkBefore.UseVisualStyleBackColor = True
        '
        'btnClearFilters
        '
        resources.ApplyResources(Me.btnClearFilters, "btnClearFilters")
        Me.btnClearFilters.Name = "btnClearFilters"
        Me.btnClearFilters.UseVisualStyleBackColor = True
        '
        'lblBefore
        '
        resources.ApplyResources(Me.lblBefore, "lblBefore")
        Me.lblBefore.Name = "lblBefore"
        '
        'dtBeforeTime
        '
        resources.ApplyResources(Me.dtBeforeTime, "dtBeforeTime")
        Me.dtBeforeTime.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.dtBeforeTime.Name = "dtBeforeTime"
        Me.dtBeforeTime.ShowUpDown = True
        '
        'dtBeforeDate
        '
        resources.ApplyResources(Me.dtBeforeDate, "dtBeforeDate")
        Me.dtBeforeDate.Checked = False
        Me.dtBeforeDate.Name = "dtBeforeDate"
        '
        'lblAfter
        '
        resources.ApplyResources(Me.lblAfter, "lblAfter")
        Me.lblAfter.Name = "lblAfter"
        '
        'dtAfterTime
        '
        resources.ApplyResources(Me.dtAfterTime, "dtAfterTime")
        Me.dtAfterTime.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.dtAfterTime.Name = "dtAfterTime"
        Me.dtAfterTime.ShowUpDown = True
        '
        'dtAfterDate
        '
        resources.ApplyResources(Me.dtAfterDate, "dtAfterDate")
        Me.dtAfterDate.Checked = False
        Me.dtAfterDate.Name = "dtAfterDate"
        '
        'btnApplyFilter
        '
        resources.ApplyResources(Me.btnApplyFilter, "btnApplyFilter")
        Me.btnApplyFilter.Name = "btnApplyFilter"
        Me.btnApplyFilter.UseVisualStyleBackColor = True
        '
        'txtvin
        '
        resources.ApplyResources(Me.txtvin, "txtvin")
        Me.txtvin.Name = "txtvin"
        '
        'lblVin
        '
        resources.ApplyResources(Me.lblVin, "lblVin")
        Me.lblVin.Name = "lblVin"
        '
        'cboOption
        '
        Me.cboOption.FormattingEnabled = True
        resources.ApplyResources(Me.cboOption, "cboOption")
        Me.cboOption.Name = "cboOption"
        '
        'lblOption
        '
        resources.ApplyResources(Me.lblOption, "lblOption")
        Me.lblOption.Name = "lblOption"
        '
        'cboColor
        '
        Me.cboColor.FormattingEnabled = True
        resources.ApplyResources(Me.cboColor, "cboColor")
        Me.cboColor.Name = "cboColor"
        '
        'lblColor
        '
        resources.ApplyResources(Me.lblColor, "lblColor")
        Me.lblColor.Name = "lblColor"
        '
        'cboStyle
        '
        Me.cboStyle.FormattingEnabled = True
        resources.ApplyResources(Me.cboStyle, "cboStyle")
        Me.cboStyle.Name = "cboStyle"
        '
        'lblStyle
        '
        resources.ApplyResources(Me.lblStyle, "lblStyle")
        Me.lblStyle.Name = "lblStyle"
        '
        'cboDevice
        '
        Me.cboDevice.FormattingEnabled = True
        resources.ApplyResources(Me.cboDevice, "cboDevice")
        Me.cboDevice.Name = "cboDevice"
        '
        'lblDevice
        '
        resources.ApplyResources(Me.lblDevice, "lblDevice")
        Me.lblDevice.Name = "lblDevice"
        '
        'cboZone
        '
        Me.cboZone.FormattingEnabled = True
        resources.ApplyResources(Me.cboZone, "cboZone")
        Me.cboZone.Name = "cboZone"
        '
        'lblZone
        '
        resources.ApplyResources(Me.lblZone, "lblZone")
        Me.lblZone.Name = "lblZone"
        '
        'tlsMain
        '
        resources.ApplyResources(Me.tlsMain, "tlsMain")
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.ToolStripSeparator1, Me.btnOpen, Me.ToolStripSeparator4, Me.btnRefresh, Me.btnShow, Me.btnHide})
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
        'btnOpen
        '
        Me.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnOpen.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuAddFolder, Me.mnuAddZip, Me.mnuOpenDmonFile, Me.mnuImportCfg, Me.mnuExportCfg})
        resources.ApplyResources(Me.btnOpen, "btnOpen")
        Me.btnOpen.Name = "btnOpen"
        '
        'mnuAddFolder
        '
        Me.mnuAddFolder.Name = "mnuAddFolder"
        resources.ApplyResources(Me.mnuAddFolder, "mnuAddFolder")
        '
        'mnuAddZip
        '
        Me.mnuAddZip.Name = "mnuAddZip"
        resources.ApplyResources(Me.mnuAddZip, "mnuAddZip")
        '
        'mnuOpenDmonFile
        '
        Me.mnuOpenDmonFile.Name = "mnuOpenDmonFile"
        resources.ApplyResources(Me.mnuOpenDmonFile, "mnuOpenDmonFile")
        '
        'mnuImportCfg
        '
        Me.mnuImportCfg.Name = "mnuImportCfg"
        resources.ApplyResources(Me.mnuImportCfg, "mnuImportCfg")
        '
        'mnuExportCfg
        '
        Me.mnuExportCfg.Name = "mnuExportCfg"
        resources.ApplyResources(Me.mnuExportCfg, "mnuExportCfg")
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        resources.ApplyResources(Me.ToolStripSeparator4, "ToolStripSeparator4")
        '
        'btnRefresh
        '
        resources.ApplyResources(Me.btnRefresh, "btnRefresh")
        Me.btnRefresh.Name = "btnRefresh"
        '
        'btnShow
        '
        Me.btnShow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.btnShow, "btnShow")
        Me.btnShow.Name = "btnShow"
        '
        'btnHide
        '
        Me.btnHide.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.btnHide, "btnHide")
        Me.btnHide.Name = "btnHide"
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
        Me.tlpDoc.ResumeLayout(False)
        Me.splMain.Panel1.ResumeLayout(False)
        Me.splMain.Panel2.ResumeLayout(False)
        Me.splMain.ResumeLayout(False)
        Me.tlpFolders.ResumeLayout(False)
        Me.mnuListMenu.ResumeLayout(False)
        Me.tlpFiles.ResumeLayout(False)
        CType(Me.dgFiles, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlFilter.ResumeLayout(False)
        Me.pnlFilter.PerformLayout()
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
    Friend WithEvents tlpDoc As System.Windows.Forms.TableLayoutPanel
    Public WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BottomToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents TopToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents RightToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents LeftToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents ContentPanel As System.Windows.Forms.ToolStripContentPanel
    Friend WithEvents splMain As System.Windows.Forms.SplitContainer
    Friend WithEvents tlpFolders As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblFolders As System.Windows.Forms.Label
    Friend WithEvents tlpFiles As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblFiles As System.Windows.Forms.Label
    Friend WithEvents dgFiles As System.Windows.Forms.DataGridView
    Friend WithEvents btnRefresh As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnOpen As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents mnuAddFolder As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAddZip As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chkFolders As System.Windows.Forms.CheckedListBox
    Friend WithEvents mnuOpenDmonFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnShow As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnHide As System.Windows.Forms.ToolStripButton
    Friend WithEvents mnuListMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuSelectAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuUnselectAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lstStatus As System.Windows.Forms.ListBox
    Friend WithEvents pnlFilter As System.Windows.Forms.Panel
    Friend WithEvents cboOption As System.Windows.Forms.ComboBox
    Friend WithEvents lblOption As System.Windows.Forms.Label
    Friend WithEvents cboColor As System.Windows.Forms.ComboBox
    Friend WithEvents lblColor As System.Windows.Forms.Label
    Friend WithEvents cboStyle As System.Windows.Forms.ComboBox
    Friend WithEvents lblStyle As System.Windows.Forms.Label
    Friend WithEvents cboDevice As System.Windows.Forms.ComboBox
    Friend WithEvents lblDevice As System.Windows.Forms.Label
    Friend WithEvents cboZone As System.Windows.Forms.ComboBox
    Friend WithEvents lblZone As System.Windows.Forms.Label
    Friend WithEvents txtvin As System.Windows.Forms.TextBox
    Friend WithEvents lblVin As System.Windows.Forms.Label
    Friend WithEvents lblAfter As System.Windows.Forms.Label
    Friend WithEvents dtAfterTime As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtAfterDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents btnApplyFilter As System.Windows.Forms.Button
    Friend WithEvents lblBefore As System.Windows.Forms.Label
    Friend WithEvents dtBeforeTime As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtBeforeDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents btnClearFilters As System.Windows.Forms.Button
    Friend WithEvents chkAfter As System.Windows.Forms.CheckBox
    Friend WithEvents chkBefore As System.Windows.Forms.CheckBox
    Friend WithEvents mnuRemoveItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuImportCfg As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuExportCfg As System.Windows.Forms.ToolStripMenuItem
End Class
