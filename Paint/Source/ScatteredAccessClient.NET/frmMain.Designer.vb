<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If Disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(Disposing)
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
        Me.spltMain = New System.Windows.Forms.SplitContainer
        Me.tlpLeft = New System.Windows.Forms.TableLayoutPanel
        Me.clbArms = New System.Windows.Forms.CheckedListBox
        Me.dgvData = New System.Windows.Forms.DataGridView
        Me.pnlTop = New System.Windows.Forms.Panel
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.btnStatus = New System.Windows.Forms.ToolStripButton
        Me.btnSave = New System.Windows.Forms.ToolStripButton
        Me.btnUndo = New System.Windows.Forms.ToolStripButton
        Me.btnCopy = New System.Windows.Forms.ToolStripButton
        Me.btnPrint = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuPrint = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintPreview = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPageSetup = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintFile = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintOptions = New System.Windows.Forms.ToolStripMenuItem
        Me.btnChangeLog = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuLast24 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLast7 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAllChanges = New System.Windows.Forms.ToolStripMenuItem
        Me.btnMultiView = New System.Windows.Forms.ToolStripButton
        Me.btnAdd = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuAddAllRobots = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAddAllPainters = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAddAllOpeners = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAddAllCustom = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAddAllColors = New System.Windows.Forms.ToolStripMenuItem
        Me.btnClear = New System.Windows.Forms.ToolStripButton
        Me.btnRestore = New System.Windows.Forms.ToolStripButton
        Me.btnUtilities = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuExport = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuImport = New System.Windows.Forms.ToolStripMenuItem
        Me.BottomToolStripPanel = New System.Windows.Forms.ToolStripPanel
        Me.TopToolStripPanel = New System.Windows.Forms.ToolStripPanel
        Me.RightToolStripPanel = New System.Windows.Forms.ToolStripPanel
        Me.LeftToolStripPanel = New System.Windows.Forms.ToolStripPanel
        Me.ContentPanel = New System.Windows.Forms.ToolStripContentPanel
        Me.tmrDisplay = New System.Windows.Forms.Timer(Me.components)
        Me.tscMain.BottomToolStripPanel.SuspendLayout()
        Me.tscMain.ContentPanel.SuspendLayout()
        Me.tscMain.TopToolStripPanel.SuspendLayout()
        Me.tscMain.SuspendLayout()
        Me.stsStatus.SuspendLayout()
        Me.spltMain.Panel1.SuspendLayout()
        Me.spltMain.Panel2.SuspendLayout()
        Me.spltMain.SuspendLayout()
        Me.tlpLeft.SuspendLayout()
        CType(Me.dgvData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlTop.SuspendLayout()
        Me.tlsMain.SuspendLayout()
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
        Me.tscMain.ContentPanel.Controls.Add(Me.spltMain)
        Me.tscMain.ContentPanel.Controls.Add(Me.pnlTop)
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
        'stsStatus
        '
        resources.ApplyResources(Me.stsStatus, "stsStatus")
        Me.stsStatus.GripMargin = New System.Windows.Forms.Padding(0)
        Me.stsStatus.ImageScalingSize = New System.Drawing.Size(22, 22)
        Me.stsStatus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatus, Me.tspProgress, Me.lblSpacer, Me.btnFunction})
        Me.stsStatus.Name = "stsStatus"
        Me.stsStatus.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode
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
        'spltMain
        '
        resources.ApplyResources(Me.spltMain, "spltMain")
        Me.spltMain.Name = "spltMain"
        '
        'spltMain.Panel1
        '
        Me.spltMain.Panel1.Controls.Add(Me.tlpLeft)
        '
        'spltMain.Panel2
        '
        Me.spltMain.Panel2.Controls.Add(Me.dgvData)
        '
        'tlpLeft
        '
        resources.ApplyResources(Me.tlpLeft, "tlpLeft")
        Me.tlpLeft.Controls.Add(Me.clbArms, 0, 0)
        Me.tlpLeft.Name = "tlpLeft"
        '
        'clbArms
        '
        Me.clbArms.CheckOnClick = True
        resources.ApplyResources(Me.clbArms, "clbArms")
        Me.clbArms.FormattingEnabled = True
        Me.clbArms.Name = "clbArms"
        '
        'dgvData
        '
        Me.dgvData.AllowUserToAddRows = False
        Me.dgvData.AllowUserToDeleteRows = False
        Me.dgvData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.dgvData.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.dgvData, "dgvData")
        Me.dgvData.Name = "dgvData"
        '
        'pnlTop
        '
        Me.pnlTop.Controls.Add(Me.lstStatus)
        resources.ApplyResources(Me.pnlTop, "pnlTop")
        Me.pnlTop.Name = "pnlTop"
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
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.btnStatus, Me.btnSave, Me.btnUndo, Me.btnCopy, Me.btnPrint, Me.btnChangeLog, Me.btnMultiView, Me.btnAdd, Me.btnClear, Me.btnRestore, Me.btnUtilities})
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
        Me.btnStatus.CheckOnClick = True
        Me.btnStatus.Name = "btnStatus"
        '
        'btnSave
        '
        resources.ApplyResources(Me.btnSave, "btnSave")
        Me.btnSave.Name = "btnSave"
        '
        'btnUndo
        '
        resources.ApplyResources(Me.btnUndo, "btnUndo")
        Me.btnUndo.Name = "btnUndo"
        '
        'btnCopy
        '
        resources.ApplyResources(Me.btnCopy, "btnCopy")
        Me.btnCopy.Name = "btnCopy"
        '
        'btnPrint
        '
        Me.btnPrint.DropDownButtonWidth = 13
        Me.btnPrint.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuPrint, Me.mnuPrintPreview, Me.mnuPageSetup, Me.mnuPrintFile, Me.mnuPrintOptions})
        resources.ApplyResources(Me.btnPrint, "btnPrint")
        Me.btnPrint.Name = "btnPrint"
        '
        'mnuPrint
        '
        Me.mnuPrint.Name = "mnuPrint"
        resources.ApplyResources(Me.mnuPrint, "mnuPrint")
        '
        'mnuPrintPreview
        '
        Me.mnuPrintPreview.Name = "mnuPrintPreview"
        resources.ApplyResources(Me.mnuPrintPreview, "mnuPrintPreview")
        '
        'mnuPageSetup
        '
        Me.mnuPageSetup.Name = "mnuPageSetup"
        resources.ApplyResources(Me.mnuPageSetup, "mnuPageSetup")
        '
        'mnuPrintFile
        '
        Me.mnuPrintFile.Name = "mnuPrintFile"
        resources.ApplyResources(Me.mnuPrintFile, "mnuPrintFile")
        '
        'mnuPrintOptions
        '
        Me.mnuPrintOptions.Name = "mnuPrintOptions"
        resources.ApplyResources(Me.mnuPrintOptions, "mnuPrintOptions")
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
        'btnMultiView
        '
        resources.ApplyResources(Me.btnMultiView, "btnMultiView")
        Me.btnMultiView.Name = "btnMultiView"
        '
        'btnAdd
        '
        resources.ApplyResources(Me.btnAdd, "btnAdd")
        Me.btnAdd.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuAddAllRobots, Me.mnuAddAllPainters, Me.mnuAddAllOpeners, Me.mnuAddAllCustom, Me.mnuAddAllColors})
        Me.btnAdd.Name = "btnAdd"
        '
        'mnuAddAllRobots
        '
        resources.ApplyResources(Me.mnuAddAllRobots, "mnuAddAllRobots")
        Me.mnuAddAllRobots.Name = "mnuAddAllRobots"
        '
        'mnuAddAllPainters
        '
        resources.ApplyResources(Me.mnuAddAllPainters, "mnuAddAllPainters")
        Me.mnuAddAllPainters.Name = "mnuAddAllPainters"
        '
        'mnuAddAllOpeners
        '
        resources.ApplyResources(Me.mnuAddAllOpeners, "mnuAddAllOpeners")
        Me.mnuAddAllOpeners.Name = "mnuAddAllOpeners"
        '
        'mnuAddAllCustom
        '
        resources.ApplyResources(Me.mnuAddAllCustom, "mnuAddAllCustom")
        Me.mnuAddAllCustom.Name = "mnuAddAllCustom"
        '
        'mnuAddAllColors
        '
        resources.ApplyResources(Me.mnuAddAllColors, "mnuAddAllColors")
        Me.mnuAddAllColors.Name = "mnuAddAllColors"
        '
        'btnClear
        '
        resources.ApplyResources(Me.btnClear, "btnClear")
        Me.btnClear.Name = "btnClear"
        '
        'btnRestore
        '
        resources.ApplyResources(Me.btnRestore, "btnRestore")
        Me.btnRestore.Name = "btnRestore"
        '
        'btnUtilities
        '
        Me.btnUtilities.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuExport, Me.mnuImport})
        resources.ApplyResources(Me.btnUtilities, "btnUtilities")
        Me.btnUtilities.Name = "btnUtilities"
        '
        'mnuExport
        '
        Me.mnuExport.Name = "mnuExport"
        resources.ApplyResources(Me.mnuExport, "mnuExport")
        '
        'mnuImport
        '
        Me.mnuImport.Name = "mnuImport"
        resources.ApplyResources(Me.mnuImport, "mnuImport")
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
        'tmrDisplay
        '
        '
        'frmMain
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.tscMain)
        Me.KeyPreview = True
        Me.Name = "frmMain"
        Me.tscMain.BottomToolStripPanel.ResumeLayout(False)
        Me.tscMain.ContentPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.PerformLayout()
        Me.tscMain.ResumeLayout(False)
        Me.tscMain.PerformLayout()
        Me.stsStatus.ResumeLayout(False)
        Me.stsStatus.PerformLayout()
        Me.spltMain.Panel1.ResumeLayout(False)
        Me.spltMain.Panel2.ResumeLayout(False)
        Me.spltMain.ResumeLayout(False)
        Me.tlpLeft.ResumeLayout(False)
        CType(Me.dgvData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlTop.ResumeLayout(False)
        Me.tlsMain.ResumeLayout(False)
        Me.tlsMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BottomToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents TopToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents RightToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents LeftToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents ContentPanel As System.Windows.Forms.ToolStripContentPanel
    Friend WithEvents tscMain As System.Windows.Forms.ToolStripContainer
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnClose As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnSave As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnUndo As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnCopy As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnPrint As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuPrint As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnChangeLog As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuLast24 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLast7 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAllChanges As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnStatus As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnRestore As System.Windows.Forms.ToolStripButton
    Friend WithEvents stsStatus As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tspProgress As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents lblSpacer As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents btnFunction As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents mnuLogin As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLogOut As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuRemote As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLocal As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents pnlTop As System.Windows.Forms.Panel
    Friend WithEvents lstStatus As System.Windows.Forms.ListBox
    Friend WithEvents mnuPrintPreview As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPageSetup As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintOptions As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnUtilities As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuExport As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuImport As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnMultiView As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnAdd As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuAddAllRobots As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAddAllPainters As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAddAllOpeners As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAddAllCustom As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAddAllColors As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnClear As System.Windows.Forms.ToolStripButton
    Friend WithEvents spltMain As System.Windows.Forms.SplitContainer
    Friend WithEvents tlpLeft As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents clbArms As System.Windows.Forms.CheckedListBox
    Friend WithEvents dgvData As System.Windows.Forms.DataGridView
    Friend WithEvents tmrDisplay As System.Windows.Forms.Timer
End Class
