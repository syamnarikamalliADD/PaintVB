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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.tscMain = New System.Windows.Forms.ToolStripContainer
        Me.splMain = New System.Windows.Forms.SplitContainer
        Me.pnlList = New System.Windows.Forms.Panel
        Me.trvwPages = New System.Windows.Forms.TreeView
        Me.tlpRight = New System.Windows.Forms.TableLayoutPanel
        Me.webMain = New System.Windows.Forms.WebBrowser
        Me.pnlTitle = New System.Windows.Forms.Panel
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.lblTitle = New System.Windows.Forms.Label
        Me.cboAddress = New System.Windows.Forms.ComboBox
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.btnSave = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
        Me.btnPrint = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuPrint = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintPreview = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPageSetup = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator
        Me.btnShow = New System.Windows.Forms.ToolStripButton
        Me.btnHide = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator
        Me.btnBack = New System.Windows.Forms.ToolStripButton
        Me.btnForward = New System.Windows.Forms.ToolStripButton
        Me.btnPrevious = New System.Windows.Forms.ToolStripButton
        Me.btnNext = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator
        Me.btnMultiView = New System.Windows.Forms.ToolStripButton
        Me.btnStatus = New System.Windows.Forms.ToolStripButton
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
        Me.splMain.Panel1.SuspendLayout()
        Me.splMain.Panel2.SuspendLayout()
        Me.splMain.SuspendLayout()
        Me.pnlList.SuspendLayout()
        Me.tlpRight.SuspendLayout()
        Me.pnlTitle.SuspendLayout()
        Me.tlsMain.SuspendLayout()
        Me.stsStatus.SuspendLayout()
        Me.SuspendLayout()
        '
        'tscMain
        '
        '
        'tscMain.ContentPanel
        '
        Me.tscMain.ContentPanel.Controls.Add(Me.splMain)
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
        'splMain
        '
        resources.ApplyResources(Me.splMain, "splMain")
        Me.splMain.Name = "splMain"
        '
        'splMain.Panel1
        '
        Me.splMain.Panel1.Controls.Add(Me.pnlList)
        '
        'splMain.Panel2
        '
        Me.splMain.Panel2.Controls.Add(Me.tlpRight)
        '
        'pnlList
        '
        Me.pnlList.Controls.Add(Me.trvwPages)
        resources.ApplyResources(Me.pnlList, "pnlList")
        Me.pnlList.Name = "pnlList"
        '
        'trvwPages
        '
        resources.ApplyResources(Me.trvwPages, "trvwPages")
        Me.trvwPages.ItemHeight = 20
        Me.trvwPages.Name = "trvwPages"
        '
        'tlpRight
        '
        resources.ApplyResources(Me.tlpRight, "tlpRight")
        Me.tlpRight.Controls.Add(Me.webMain, 0, 2)
        Me.tlpRight.Controls.Add(Me.pnlTitle, 0, 0)
        Me.tlpRight.Controls.Add(Me.cboAddress, 0, 1)
        Me.tlpRight.Name = "tlpRight"
        '
        'webMain
        '
        resources.ApplyResources(Me.webMain, "webMain")
        Me.webMain.MinimumSize = New System.Drawing.Size(20, 20)
        Me.webMain.Name = "webMain"
        Me.webMain.ScriptErrorsSuppressed = True
        '
        'pnlTitle
        '
        Me.pnlTitle.Controls.Add(Me.lstStatus)
        Me.pnlTitle.Controls.Add(Me.lblTitle)
        resources.ApplyResources(Me.pnlTitle, "pnlTitle")
        Me.pnlTitle.Name = "pnlTitle"
        '
        'lstStatus
        '
        resources.ApplyResources(Me.lstStatus, "lstStatus")
        Me.lstStatus.BackColor = System.Drawing.SystemColors.Info
        Me.lstStatus.Name = "lstStatus"
        '
        'lblTitle
        '
        resources.ApplyResources(Me.lblTitle, "lblTitle")
        Me.lblTitle.Name = "lblTitle"
        '
        'cboAddress
        '
        resources.ApplyResources(Me.cboAddress, "cboAddress")
        Me.cboAddress.FormattingEnabled = True
        Me.cboAddress.Name = "cboAddress"
        '
        'tlsMain
        '
        resources.ApplyResources(Me.tlsMain, "tlsMain")
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.ToolStripSeparator1, Me.btnSave, Me.ToolStripSeparator2, Me.btnPrint, Me.ToolStripSeparator3, Me.btnShow, Me.btnHide, Me.ToolStripSeparator4, Me.btnBack, Me.btnForward, Me.btnPrevious, Me.btnNext, Me.ToolStripSeparator5, Me.btnMultiView, Me.btnStatus})
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
        'btnSave
        '
        resources.ApplyResources(Me.btnSave, "btnSave")
        Me.btnSave.Name = "btnSave"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        resources.ApplyResources(Me.ToolStripSeparator2, "ToolStripSeparator2")
        '
        'btnPrint
        '
        Me.btnPrint.DropDownButtonWidth = 13
        Me.btnPrint.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuPrint, Me.mnuPrintPreview, Me.mnuPageSetup})
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
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        resources.ApplyResources(Me.ToolStripSeparator3, "ToolStripSeparator3")
        '
        'btnShow
        '
        resources.ApplyResources(Me.btnShow, "btnShow")
        Me.btnShow.Name = "btnShow"
        '
        'btnHide
        '
        resources.ApplyResources(Me.btnHide, "btnHide")
        Me.btnHide.Name = "btnHide"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        resources.ApplyResources(Me.ToolStripSeparator4, "ToolStripSeparator4")
        '
        'btnBack
        '
        resources.ApplyResources(Me.btnBack, "btnBack")
        Me.btnBack.Name = "btnBack"
        '
        'btnForward
        '
        resources.ApplyResources(Me.btnForward, "btnForward")
        Me.btnForward.Name = "btnForward"
        '
        'btnPrevious
        '
        resources.ApplyResources(Me.btnPrevious, "btnPrevious")
        Me.btnPrevious.Name = "btnPrevious"
        '
        'btnNext
        '
        resources.ApplyResources(Me.btnNext, "btnNext")
        Me.btnNext.Name = "btnNext"
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        resources.ApplyResources(Me.ToolStripSeparator5, "ToolStripSeparator5")
        '
        'btnMultiView
        '
        resources.ApplyResources(Me.btnMultiView, "btnMultiView")
        Me.btnMultiView.Name = "btnMultiView"
        '
        'btnStatus
        '
        Me.btnStatus.CheckOnClick = True
        resources.ApplyResources(Me.btnStatus, "btnStatus")
        Me.btnStatus.Name = "btnStatus"
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
        'frmMain
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.stsStatus)
        Me.Controls.Add(Me.tscMain)
        Me.KeyPreview = True
        Me.Name = "frmMain"
        Me.tscMain.ContentPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.PerformLayout()
        Me.tscMain.ResumeLayout(False)
        Me.tscMain.PerformLayout()
        Me.splMain.Panel1.ResumeLayout(False)
        Me.splMain.Panel2.ResumeLayout(False)
        Me.splMain.ResumeLayout(False)
        Me.pnlList.ResumeLayout(False)
        Me.tlpRight.ResumeLayout(False)
        Me.pnlTitle.ResumeLayout(False)
        Me.tlsMain.ResumeLayout(False)
        Me.tlsMain.PerformLayout()
        Me.stsStatus.ResumeLayout(False)
        Me.stsStatus.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents cboParam As System.Windows.Forms.ComboBox
    Friend WithEvents BottomToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents TopToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents RightToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents LeftToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents ContentPanel As System.Windows.Forms.ToolStripContentPanel
    Friend WithEvents tscMain As System.Windows.Forms.ToolStripContainer
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnClose As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnSave As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnPrint As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuPrint As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnMultiView As System.Windows.Forms.ToolStripButton
    Friend WithEvents stsStatus As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tspProgress As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents lblSpacer As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents btnFunction As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents mnuLogin As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLogOut As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuRemote As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLocal As System.Windows.Forms.ToolStripMenuItem
    Public WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Public WithEvents btnNext As System.Windows.Forms.ToolStripButton
    Public WithEvents btnForward As System.Windows.Forms.ToolStripButton
    Public WithEvents btnBack As System.Windows.Forms.ToolStripButton
    Public WithEvents btnPrevious As System.Windows.Forms.ToolStripButton
    Public WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Public WithEvents btnShow As System.Windows.Forms.ToolStripButton
    Public WithEvents btnHide As System.Windows.Forms.ToolStripButton
    Public WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
    Public WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Public WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents btnStatus As System.Windows.Forms.ToolStripButton
    Friend WithEvents mnuPageSetup As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintPreview As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents splMain As System.Windows.Forms.SplitContainer
    Friend WithEvents trvwPages As System.Windows.Forms.TreeView
    Friend WithEvents tlpRight As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents webMain As System.Windows.Forms.WebBrowser
    Friend WithEvents pnlTitle As System.Windows.Forms.Panel
    Friend WithEvents lstStatus As System.Windows.Forms.ListBox
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents pnlList As System.Windows.Forms.Panel
    Friend WithEvents cboAddress As System.Windows.Forms.ComboBox
End Class
