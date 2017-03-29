<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAll
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAll))
        Me.tscMain = New System.Windows.Forms.ToolStripContainer
        Me.stsStatus = New System.Windows.Forms.StatusStrip
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel
        Me.tspProgress = New System.Windows.Forms.ToolStripProgressBar
        Me.lblSpacer = New System.Windows.Forms.ToolStripStatusLabel
        Me.chkSyncScroll = New System.Windows.Forms.CheckBox
        Me.lblWidth = New System.Windows.Forms.Label
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.cboParam = New System.Windows.Forms.ComboBox
        Me.pnlMain = New System.Windows.Forms.Panel
        Me.tlpMain = New System.Windows.Forms.TableLayoutPanel
        Me.dg4 = New System.Windows.Forms.DataGridView
        Me.dg2 = New System.Windows.Forms.DataGridView
        Me.dg3 = New System.Windows.Forms.DataGridView
        Me.dg8 = New System.Windows.Forms.DataGridView
        Me.lblTable8 = New System.Windows.Forms.Label
        Me.lblTable7 = New System.Windows.Forms.Label
        Me.lblTable6 = New System.Windows.Forms.Label
        Me.lblTable5 = New System.Windows.Forms.Label
        Me.lblTable4 = New System.Windows.Forms.Label
        Me.lblTable2 = New System.Windows.Forms.Label
        Me.dg5 = New System.Windows.Forms.DataGridView
        Me.dg6 = New System.Windows.Forms.DataGridView
        Me.dg7 = New System.Windows.Forms.DataGridView
        Me.lblTable3 = New System.Windows.Forms.Label
        Me.lblTable1 = New System.Windows.Forms.Label
        Me.dg1 = New System.Windows.Forms.DataGridView
        Me.lblParam = New System.Windows.Forms.Label
        Me.cboRobot = New System.Windows.Forms.ComboBox
        Me.cboZone = New System.Windows.Forms.ComboBox
        Me.lblZone = New System.Windows.Forms.Label
        Me.lblRobot = New System.Windows.Forms.Label
        Me.cboSubParam = New System.Windows.Forms.ComboBox
        Me.lblSubParam = New System.Windows.Forms.Label
        Me.trkWidth = New System.Windows.Forms.TrackBar
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.btnPrint = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuPrint = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintPreview = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPageSetup = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintFile = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintOptions = New System.Windows.Forms.ToolStripMenuItem
        Me.btnAdd = New System.Windows.Forms.ToolStripButton
        Me.btnClear = New System.Windows.Forms.ToolStripButton
        Me.btnStatus = New System.Windows.Forms.ToolStripButton
        Me.tscMain.BottomToolStripPanel.SuspendLayout()
        Me.tscMain.ContentPanel.SuspendLayout()
        Me.tscMain.TopToolStripPanel.SuspendLayout()
        Me.tscMain.SuspendLayout()
        Me.stsStatus.SuspendLayout()
        Me.pnlMain.SuspendLayout()
        Me.tlpMain.SuspendLayout()
        CType(Me.dg4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dg2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dg3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dg8, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dg5, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dg6, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dg7, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dg1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkWidth, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.tscMain.ContentPanel.Controls.Add(Me.chkSyncScroll)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblWidth)
        Me.tscMain.ContentPanel.Controls.Add(Me.lstStatus)
        Me.tscMain.ContentPanel.Controls.Add(Me.cboParam)
        Me.tscMain.ContentPanel.Controls.Add(Me.pnlMain)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblParam)
        Me.tscMain.ContentPanel.Controls.Add(Me.cboRobot)
        Me.tscMain.ContentPanel.Controls.Add(Me.cboZone)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblZone)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblRobot)
        Me.tscMain.ContentPanel.Controls.Add(Me.cboSubParam)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblSubParam)
        Me.tscMain.ContentPanel.Controls.Add(Me.trkWidth)
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
        Me.stsStatus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatus, Me.tspProgress, Me.lblSpacer})
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
        'chkSyncScroll
        '
        resources.ApplyResources(Me.chkSyncScroll, "chkSyncScroll")
        Me.chkSyncScroll.Name = "chkSyncScroll"
        Me.chkSyncScroll.UseVisualStyleBackColor = True
        '
        'lblWidth
        '
        resources.ApplyResources(Me.lblWidth, "lblWidth")
        Me.lblWidth.Name = "lblWidth"
        '
        'lstStatus
        '
        resources.ApplyResources(Me.lstStatus, "lstStatus")
        Me.lstStatus.BackColor = System.Drawing.SystemColors.Info
        Me.lstStatus.Name = "lstStatus"
        '
        'cboParam
        '
        Me.cboParam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboParam, "cboParam")
        Me.cboParam.Name = "cboParam"
        '
        'pnlMain
        '
        resources.ApplyResources(Me.pnlMain, "pnlMain")
        Me.pnlMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.pnlMain.Controls.Add(Me.tlpMain)
        Me.pnlMain.Name = "pnlMain"
        '
        'tlpMain
        '
        resources.ApplyResources(Me.tlpMain, "tlpMain")
        Me.tlpMain.Controls.Add(Me.dg4, 0, 1)
        Me.tlpMain.Controls.Add(Me.dg2, 0, 1)
        Me.tlpMain.Controls.Add(Me.dg3, 0, 1)
        Me.tlpMain.Controls.Add(Me.dg8, 4, 1)
        Me.tlpMain.Controls.Add(Me.lblTable8, 7, 0)
        Me.tlpMain.Controls.Add(Me.lblTable7, 6, 0)
        Me.tlpMain.Controls.Add(Me.lblTable6, 5, 0)
        Me.tlpMain.Controls.Add(Me.lblTable5, 4, 0)
        Me.tlpMain.Controls.Add(Me.lblTable4, 3, 0)
        Me.tlpMain.Controls.Add(Me.lblTable2, 0, 0)
        Me.tlpMain.Controls.Add(Me.dg5, 1, 1)
        Me.tlpMain.Controls.Add(Me.dg6, 2, 1)
        Me.tlpMain.Controls.Add(Me.dg7, 3, 1)
        Me.tlpMain.Controls.Add(Me.lblTable3, 2, 0)
        Me.tlpMain.Controls.Add(Me.lblTable1, 0, 0)
        Me.tlpMain.Controls.Add(Me.dg1, 0, 1)
        Me.tlpMain.Name = "tlpMain"
        '
        'dg4
        '
        Me.dg4.AllowUserToAddRows = False
        Me.dg4.AllowUserToDeleteRows = False
        Me.dg4.AllowUserToResizeRows = False
        Me.dg4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.dg4, "dg4")
        Me.dg4.Name = "dg4"
        '
        'dg2
        '
        Me.dg2.AllowUserToAddRows = False
        Me.dg2.AllowUserToDeleteRows = False
        Me.dg2.AllowUserToResizeRows = False
        Me.dg2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.dg2, "dg2")
        Me.dg2.Name = "dg2"
        '
        'dg3
        '
        Me.dg3.AllowUserToAddRows = False
        Me.dg3.AllowUserToDeleteRows = False
        Me.dg3.AllowUserToResizeRows = False
        Me.dg3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.dg3, "dg3")
        Me.dg3.Name = "dg3"
        '
        'dg8
        '
        Me.dg8.AllowUserToAddRows = False
        Me.dg8.AllowUserToDeleteRows = False
        Me.dg8.AllowUserToResizeRows = False
        Me.dg8.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.dg8, "dg8")
        Me.dg8.Name = "dg8"
        '
        'lblTable8
        '
        resources.ApplyResources(Me.lblTable8, "lblTable8")
        Me.lblTable8.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblTable8.Name = "lblTable8"
        Me.lblTable8.UseMnemonic = False
        '
        'lblTable7
        '
        resources.ApplyResources(Me.lblTable7, "lblTable7")
        Me.lblTable7.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblTable7.Name = "lblTable7"
        Me.lblTable7.UseMnemonic = False
        '
        'lblTable6
        '
        resources.ApplyResources(Me.lblTable6, "lblTable6")
        Me.lblTable6.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblTable6.Name = "lblTable6"
        Me.lblTable6.UseMnemonic = False
        '
        'lblTable5
        '
        resources.ApplyResources(Me.lblTable5, "lblTable5")
        Me.lblTable5.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblTable5.Name = "lblTable5"
        Me.lblTable5.UseMnemonic = False
        '
        'lblTable4
        '
        resources.ApplyResources(Me.lblTable4, "lblTable4")
        Me.lblTable4.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblTable4.Name = "lblTable4"
        Me.lblTable4.UseMnemonic = False
        '
        'lblTable2
        '
        resources.ApplyResources(Me.lblTable2, "lblTable2")
        Me.lblTable2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblTable2.Name = "lblTable2"
        Me.lblTable2.UseMnemonic = False
        '
        'dg5
        '
        Me.dg5.AllowUserToAddRows = False
        Me.dg5.AllowUserToDeleteRows = False
        Me.dg5.AllowUserToResizeRows = False
        Me.dg5.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.dg5, "dg5")
        Me.dg5.Name = "dg5"
        '
        'dg6
        '
        Me.dg6.AllowUserToAddRows = False
        Me.dg6.AllowUserToDeleteRows = False
        Me.dg6.AllowUserToResizeRows = False
        Me.dg6.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.dg6, "dg6")
        Me.dg6.Name = "dg6"
        '
        'dg7
        '
        Me.dg7.AllowUserToAddRows = False
        Me.dg7.AllowUserToDeleteRows = False
        Me.dg7.AllowUserToResizeRows = False
        Me.dg7.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.dg7, "dg7")
        Me.dg7.Name = "dg7"
        '
        'lblTable3
        '
        resources.ApplyResources(Me.lblTable3, "lblTable3")
        Me.lblTable3.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblTable3.Name = "lblTable3"
        Me.lblTable3.UseMnemonic = False
        '
        'lblTable1
        '
        resources.ApplyResources(Me.lblTable1, "lblTable1")
        Me.lblTable1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblTable1.Name = "lblTable1"
        Me.lblTable1.UseMnemonic = False
        '
        'dg1
        '
        Me.dg1.AllowUserToAddRows = False
        Me.dg1.AllowUserToDeleteRows = False
        Me.dg1.AllowUserToResizeRows = False
        Me.dg1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.dg1, "dg1")
        Me.dg1.Name = "dg1"
        '
        'lblParam
        '
        Me.lblParam.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblParam, "lblParam")
        Me.lblParam.Name = "lblParam"
        Me.lblParam.UseMnemonic = False
        '
        'cboRobot
        '
        Me.cboRobot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboRobot, "cboRobot")
        Me.cboRobot.Name = "cboRobot"
        '
        'cboZone
        '
        Me.cboZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboZone, "cboZone")
        Me.cboZone.Name = "cboZone"
        '
        'lblZone
        '
        Me.lblZone.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblZone, "lblZone")
        Me.lblZone.Name = "lblZone"
        Me.lblZone.UseMnemonic = False
        '
        'lblRobot
        '
        Me.lblRobot.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblRobot, "lblRobot")
        Me.lblRobot.Name = "lblRobot"
        Me.lblRobot.UseMnemonic = False
        '
        'cboSubParam
        '
        Me.cboSubParam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboSubParam, "cboSubParam")
        Me.cboSubParam.Name = "cboSubParam"
        '
        'lblSubParam
        '
        Me.lblSubParam.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        resources.ApplyResources(Me.lblSubParam, "lblSubParam")
        Me.lblSubParam.Name = "lblSubParam"
        Me.lblSubParam.UseMnemonic = False
        '
        'trkWidth
        '
        resources.ApplyResources(Me.trkWidth, "trkWidth")
        Me.trkWidth.Maximum = 20
        Me.trkWidth.Minimum = 10
        Me.trkWidth.Name = "trkWidth"
        Me.trkWidth.TickFrequency = 5
        Me.trkWidth.Value = 10
        '
        'tlsMain
        '
        resources.ApplyResources(Me.tlsMain, "tlsMain")
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.btnPrint, Me.btnAdd, Me.btnClear, Me.btnStatus})
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
        'btnPrint
        '
        resources.ApplyResources(Me.btnPrint, "btnPrint")
        Me.btnPrint.DropDownButtonWidth = 13
        Me.btnPrint.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuPrint, Me.mnuPrintPreview, Me.mnuPageSetup, Me.mnuPrintFile, Me.mnuPrintOptions})
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
        'btnAdd
        '
        resources.ApplyResources(Me.btnAdd, "btnAdd")
        Me.btnAdd.Name = "btnAdd"
        '
        'btnClear
        '
        resources.ApplyResources(Me.btnClear, "btnClear")
        Me.btnClear.Name = "btnClear"
        '
        'btnStatus
        '
        resources.ApplyResources(Me.btnStatus, "btnStatus")
        Me.btnStatus.Name = "btnStatus"
        '
        'frmAll
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.tscMain)
        Me.KeyPreview = True
        Me.Name = "frmAll"
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
        Me.pnlMain.ResumeLayout(False)
        Me.tlpMain.ResumeLayout(False)
        CType(Me.dg4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dg2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dg3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dg8, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dg5, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dg6, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dg7, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dg1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkWidth, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tlsMain.ResumeLayout(False)
        Me.tlsMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnClose As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnPrint As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuPrint As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblZone As System.Windows.Forms.Label
    Friend WithEvents lblRobot As System.Windows.Forms.Label
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnAdd As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnClear As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnStatus As System.Windows.Forms.ToolStripButton
    Friend WithEvents stsStatus As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tspProgress As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents lblSpacer As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents lstStatus As System.Windows.Forms.ListBox
    Friend WithEvents pnlMain As System.Windows.Forms.Panel
    Friend WithEvents tscMain As System.Windows.Forms.ToolStripContainer
    Friend WithEvents cboParam As System.Windows.Forms.ComboBox
    Friend WithEvents lblParam As System.Windows.Forms.Label
    Friend WithEvents cboRobot As System.Windows.Forms.ComboBox
    Friend WithEvents cboZone As System.Windows.Forms.ComboBox

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Friend WithEvents cboSubParam As System.Windows.Forms.ComboBox
    Friend WithEvents lblSubParam As System.Windows.Forms.Label
    Friend WithEvents tlpMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents dg1 As System.Windows.Forms.DataGridView
    Friend WithEvents lblTable3 As System.Windows.Forms.Label
    Friend WithEvents lblTable1 As System.Windows.Forms.Label
    Friend WithEvents lblTable4 As System.Windows.Forms.Label
    Friend WithEvents lblTable2 As System.Windows.Forms.Label
    Friend WithEvents dg5 As System.Windows.Forms.DataGridView
    Friend WithEvents dg6 As System.Windows.Forms.DataGridView
    Friend WithEvents dg7 As System.Windows.Forms.DataGridView
    Friend WithEvents lblTable8 As System.Windows.Forms.Label
    Friend WithEvents lblTable7 As System.Windows.Forms.Label
    Friend WithEvents lblTable6 As System.Windows.Forms.Label
    Friend WithEvents lblTable5 As System.Windows.Forms.Label
    Friend WithEvents dg4 As System.Windows.Forms.DataGridView
    Friend WithEvents dg2 As System.Windows.Forms.DataGridView
    Friend WithEvents dg3 As System.Windows.Forms.DataGridView
    Friend WithEvents dg8 As System.Windows.Forms.DataGridView
    Friend WithEvents trkWidth As System.Windows.Forms.TrackBar
    Friend WithEvents lblWidth As System.Windows.Forms.Label
    Friend WithEvents chkSyncScroll As System.Windows.Forms.CheckBox
    Friend WithEvents mnuPageSetup As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintPreview As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintOptions As System.Windows.Forms.ToolStripMenuItem
End Class
