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
        Me.stsStatus = New System.Windows.Forms.StatusStrip
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel
        Me.tspProgress = New System.Windows.Forms.ToolStripProgressBar
        Me.lblSpacer = New System.Windows.Forms.ToolStripStatusLabel
        Me.btnFunction = New System.Windows.Forms.ToolStripDropDownButton
        Me.mnuLogin = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLogOut = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuRemote = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLocal = New System.Windows.Forms.ToolStripMenuItem
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.pnlMain = New System.Windows.Forms.Panel
        Me.tlpChart = New System.Windows.Forms.TableLayoutPanel
        Me.imgPie4 = New System.Windows.Forms.PictureBox
        Me.imgKey3 = New System.Windows.Forms.PictureBox
        Me.imgKey = New System.Windows.Forms.PictureBox
        Me.imgKey4 = New System.Windows.Forms.PictureBox
        Me.imgPie3 = New System.Windows.Forms.PictureBox
        Me.imgPie2 = New System.Windows.Forms.PictureBox
        Me.imgPie = New System.Windows.Forms.PictureBox
        Me.imgKey2 = New System.Windows.Forms.PictureBox
        Me.lblTitle11 = New System.Windows.Forms.Label
        Me.lblTitle10 = New System.Windows.Forms.Label
        Me.lblTitle09 = New System.Windows.Forms.Label
        Me.lblTitle08 = New System.Windows.Forms.Label
        Me.lblTitle05 = New System.Windows.Forms.Label
        Me.lblTitle02 = New System.Windows.Forms.Label
        Me.lblTitle07 = New System.Windows.Forms.Label
        Me.lblTitle04 = New System.Windows.Forms.Label
        Me.lblTitle01 = New System.Windows.Forms.Label
        Me.lblTitle06 = New System.Windows.Forms.Label
        Me.lblTitle03 = New System.Windows.Forms.Label
        Me.lblTitle00 = New System.Windows.Forms.Label
        Me.dgvMain = New System.Windows.Forms.DataGridView
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.btnCriteria = New System.Windows.Forms.ToolStripButton
        Me.btnSave = New System.Windows.Forms.ToolStripButton
        Me.btnPrint = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuPrint = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintPreview = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPageSetup = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintFile = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintOptions = New System.Windows.Forms.ToolStripMenuItem
        Me.btnStatus = New System.Windows.Forms.ToolStripButton
        Me.btnSelect = New System.Windows.Forms.ToolStripDropDownButton
        Me.btnRefresh = New System.Windows.Forms.ToolStripButton
        Me.btnChart = New System.Windows.Forms.ToolStripButton
        Me.pnlTxt = New System.Windows.Forms.StatusBarPanel
        Me.pnlProg = New System.Windows.Forms.StatusBarPanel
        Me.tscMain.BottomToolStripPanel.SuspendLayout()
        Me.tscMain.ContentPanel.SuspendLayout()
        Me.tscMain.TopToolStripPanel.SuspendLayout()
        Me.tscMain.SuspendLayout()
        Me.stsStatus.SuspendLayout()
        Me.pnlMain.SuspendLayout()
        Me.tlpChart.SuspendLayout()
        CType(Me.imgPie4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.imgKey3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.imgKey, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.imgKey4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.imgPie3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.imgPie2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.imgPie, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.imgKey2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.tscMain.ContentPanel.Controls.Add(Me.lstStatus)
        Me.tscMain.ContentPanel.Controls.Add(Me.pnlMain)
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
        resources.ApplyResources(Me.lblStatus, "lblStatus")
        Me.lblStatus.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedOuter
        Me.lblStatus.Margin = New System.Windows.Forms.Padding(0)
        Me.lblStatus.Name = "lblStatus"
        '
        'tspProgress
        '
        resources.ApplyResources(Me.tspProgress, "tspProgress")
        Me.tspProgress.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.tspProgress.Margin = New System.Windows.Forms.Padding(2, 3, 1, 3)
        Me.tspProgress.Name = "tspProgress"
        Me.tspProgress.Padding = New System.Windows.Forms.Padding(2)
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
        resources.ApplyResources(Me.btnFunction, "btnFunction")
        Me.btnFunction.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.btnFunction.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuLogin, Me.mnuLogOut, Me.mnuRemote, Me.mnuLocal})
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
        'lstStatus
        '
        Me.lstStatus.BackColor = System.Drawing.SystemColors.Info
        resources.ApplyResources(Me.lstStatus, "lstStatus")
        Me.lstStatus.Name = "lstStatus"
        '
        'pnlMain
        '
        Me.pnlMain.Controls.Add(Me.tlpChart)
        Me.pnlMain.Controls.Add(Me.lblTitle11)
        Me.pnlMain.Controls.Add(Me.lblTitle10)
        Me.pnlMain.Controls.Add(Me.lblTitle09)
        Me.pnlMain.Controls.Add(Me.lblTitle08)
        Me.pnlMain.Controls.Add(Me.lblTitle05)
        Me.pnlMain.Controls.Add(Me.lblTitle02)
        Me.pnlMain.Controls.Add(Me.lblTitle07)
        Me.pnlMain.Controls.Add(Me.lblTitle04)
        Me.pnlMain.Controls.Add(Me.lblTitle01)
        Me.pnlMain.Controls.Add(Me.lblTitle06)
        Me.pnlMain.Controls.Add(Me.lblTitle03)
        Me.pnlMain.Controls.Add(Me.lblTitle00)
        Me.pnlMain.Controls.Add(Me.dgvMain)
        resources.ApplyResources(Me.pnlMain, "pnlMain")
        Me.pnlMain.Name = "pnlMain"
        '
        'tlpChart
        '
        resources.ApplyResources(Me.tlpChart, "tlpChart")
        Me.tlpChart.Controls.Add(Me.imgPie4, 3, 0)
        Me.tlpChart.Controls.Add(Me.imgKey3, 2, 1)
        Me.tlpChart.Controls.Add(Me.imgKey, 0, 1)
        Me.tlpChart.Controls.Add(Me.imgKey4, 3, 1)
        Me.tlpChart.Controls.Add(Me.imgPie3, 2, 0)
        Me.tlpChart.Controls.Add(Me.imgPie2, 1, 0)
        Me.tlpChart.Controls.Add(Me.imgPie, 0, 0)
        Me.tlpChart.Controls.Add(Me.imgKey2, 1, 1)
        Me.tlpChart.Name = "tlpChart"
        '
        'imgPie4
        '
        resources.ApplyResources(Me.imgPie4, "imgPie4")
        Me.imgPie4.Name = "imgPie4"
        Me.imgPie4.TabStop = False
        '
        'imgKey3
        '
        resources.ApplyResources(Me.imgKey3, "imgKey3")
        Me.imgKey3.Name = "imgKey3"
        Me.imgKey3.TabStop = False
        '
        'imgKey
        '
        resources.ApplyResources(Me.imgKey, "imgKey")
        Me.imgKey.Name = "imgKey"
        Me.imgKey.TabStop = False
        '
        'imgKey4
        '
        resources.ApplyResources(Me.imgKey4, "imgKey4")
        Me.imgKey4.Name = "imgKey4"
        Me.imgKey4.TabStop = False
        '
        'imgPie3
        '
        resources.ApplyResources(Me.imgPie3, "imgPie3")
        Me.imgPie3.Name = "imgPie3"
        Me.imgPie3.TabStop = False
        '
        'imgPie2
        '
        resources.ApplyResources(Me.imgPie2, "imgPie2")
        Me.imgPie2.Name = "imgPie2"
        Me.imgPie2.TabStop = False
        '
        'imgPie
        '
        resources.ApplyResources(Me.imgPie, "imgPie")
        Me.imgPie.Name = "imgPie"
        Me.imgPie.TabStop = False
        '
        'imgKey2
        '
        resources.ApplyResources(Me.imgKey2, "imgKey2")
        Me.imgKey2.Name = "imgKey2"
        Me.imgKey2.TabStop = False
        '
        'lblTitle11
        '
        resources.ApplyResources(Me.lblTitle11, "lblTitle11")
        Me.lblTitle11.Name = "lblTitle11"
        '
        'lblTitle10
        '
        resources.ApplyResources(Me.lblTitle10, "lblTitle10")
        Me.lblTitle10.Name = "lblTitle10"
        '
        'lblTitle09
        '
        resources.ApplyResources(Me.lblTitle09, "lblTitle09")
        Me.lblTitle09.Name = "lblTitle09"
        '
        'lblTitle08
        '
        resources.ApplyResources(Me.lblTitle08, "lblTitle08")
        Me.lblTitle08.Name = "lblTitle08"
        '
        'lblTitle05
        '
        resources.ApplyResources(Me.lblTitle05, "lblTitle05")
        Me.lblTitle05.Name = "lblTitle05"
        '
        'lblTitle02
        '
        resources.ApplyResources(Me.lblTitle02, "lblTitle02")
        Me.lblTitle02.Name = "lblTitle02"
        '
        'lblTitle07
        '
        resources.ApplyResources(Me.lblTitle07, "lblTitle07")
        Me.lblTitle07.Name = "lblTitle07"
        '
        'lblTitle04
        '
        resources.ApplyResources(Me.lblTitle04, "lblTitle04")
        Me.lblTitle04.Name = "lblTitle04"
        '
        'lblTitle01
        '
        resources.ApplyResources(Me.lblTitle01, "lblTitle01")
        Me.lblTitle01.Name = "lblTitle01"
        '
        'lblTitle06
        '
        resources.ApplyResources(Me.lblTitle06, "lblTitle06")
        Me.lblTitle06.Name = "lblTitle06"
        '
        'lblTitle03
        '
        resources.ApplyResources(Me.lblTitle03, "lblTitle03")
        Me.lblTitle03.Name = "lblTitle03"
        '
        'lblTitle00
        '
        resources.ApplyResources(Me.lblTitle00, "lblTitle00")
        Me.lblTitle00.Name = "lblTitle00"
        '
        'dgvMain
        '
        Me.dgvMain.AllowUserToAddRows = False
        Me.dgvMain.AllowUserToDeleteRows = False
        Me.dgvMain.AllowUserToOrderColumns = True
        Me.dgvMain.BackgroundColor = System.Drawing.SystemColors.Control
        Me.dgvMain.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.dgvMain, "dgvMain")
        Me.dgvMain.Name = "dgvMain"
        Me.dgvMain.ReadOnly = True
        '
        'tlsMain
        '
        resources.ApplyResources(Me.tlsMain, "tlsMain")
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.btnCriteria, Me.btnSave, Me.btnPrint, Me.btnStatus, Me.btnSelect, Me.btnRefresh, Me.btnChart})
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
        'btnCriteria
        '
        resources.ApplyResources(Me.btnCriteria, "btnCriteria")
        Me.btnCriteria.Image = Global.Reports.My.Resources.ProjectStrings.criteria
        Me.btnCriteria.Name = "btnCriteria"
        '
        'btnSave
        '
        resources.ApplyResources(Me.btnSave, "btnSave")
        Me.btnSave.Image = Global.Reports.My.Resources.ProjectStrings.criteria
        Me.btnSave.Name = "btnSave"
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
        'btnStatus
        '
        resources.ApplyResources(Me.btnStatus, "btnStatus")
        Me.btnStatus.Name = "btnStatus"
        '
        'btnSelect
        '
        resources.ApplyResources(Me.btnSelect, "btnSelect")
        Me.btnSelect.Image = Global.Reports.My.Resources.ProjectStrings.reporttype
        Me.btnSelect.Name = "btnSelect"
        '
        'btnRefresh
        '
        resources.ApplyResources(Me.btnRefresh, "btnRefresh")
        Me.btnRefresh.Name = "btnRefresh"
        '
        'btnChart
        '
        resources.ApplyResources(Me.btnChart, "btnChart")
        Me.btnChart.Name = "btnChart"
        '
        'pnlTxt
        '
        resources.ApplyResources(Me.pnlTxt, "pnlTxt")
        '
        'pnlProg
        '
        resources.ApplyResources(Me.pnlProg, "pnlProg")
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
        Me.tscMain.BottomToolStripPanel.PerformLayout()
        Me.tscMain.ContentPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.PerformLayout()
        Me.tscMain.ResumeLayout(False)
        Me.tscMain.PerformLayout()
        Me.stsStatus.ResumeLayout(False)
        Me.stsStatus.PerformLayout()
        Me.pnlMain.ResumeLayout(False)
        Me.pnlMain.PerformLayout()
        Me.tlpChart.ResumeLayout(False)
        CType(Me.imgPie4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.imgKey3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.imgKey, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.imgKey4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.imgPie3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.imgPie2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.imgPie, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.imgKey2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
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
    Friend WithEvents pnlMain As System.Windows.Forms.Panel
    Friend WithEvents lstStatus As System.Windows.Forms.ListBox
    Friend WithEvents btnFunction As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents mnuLogin As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLogOut As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuRemote As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLocal As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblSpacer As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnClose As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnPrint As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuPrintPreview As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnStatus As System.Windows.Forms.ToolStripButton
    Friend WithEvents mnuPrint As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnCriteria As System.Windows.Forms.ToolStripButton
    Friend WithEvents dgvMain As System.Windows.Forms.DataGridView
    Friend WithEvents lblTitle08 As System.Windows.Forms.Label
    Friend WithEvents lblTitle05 As System.Windows.Forms.Label
    Friend WithEvents lblTitle02 As System.Windows.Forms.Label
    Friend WithEvents lblTitle07 As System.Windows.Forms.Label
    Friend WithEvents lblTitle04 As System.Windows.Forms.Label
    Friend WithEvents lblTitle01 As System.Windows.Forms.Label
    Friend WithEvents lblTitle06 As System.Windows.Forms.Label
    Friend WithEvents lblTitle03 As System.Windows.Forms.Label
    Friend WithEvents lblTitle00 As System.Windows.Forms.Label
    Friend WithEvents lblTitle09 As System.Windows.Forms.Label
    Friend WithEvents btnSelect As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents imgPie As System.Windows.Forms.PictureBox
    Friend WithEvents imgKey2 As System.Windows.Forms.PictureBox
    Friend WithEvents btnRefresh As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnChart As System.Windows.Forms.ToolStripButton
    Friend WithEvents lblTitle11 As System.Windows.Forms.Label
    Friend WithEvents lblTitle10 As System.Windows.Forms.Label
    Friend WithEvents mnuPageSetup As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintOptions As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnSave As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlpChart As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents imgPie3 As System.Windows.Forms.PictureBox
    Friend WithEvents imgPie2 As System.Windows.Forms.PictureBox
    Friend WithEvents imgPie4 As System.Windows.Forms.PictureBox
    Friend WithEvents imgKey3 As System.Windows.Forms.PictureBox
    Friend WithEvents imgKey As System.Windows.Forms.PictureBox
    Friend WithEvents imgKey4 As System.Windows.Forms.PictureBox
End Class
