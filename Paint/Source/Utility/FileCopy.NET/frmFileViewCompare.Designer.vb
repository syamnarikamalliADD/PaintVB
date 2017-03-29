<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmFileViewCompare
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFileViewCompare))
        Me.MenuStrip = New System.Windows.Forms.MenuStrip
        Me.mnuFile = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuOpen = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator
        Me.mnuSave = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator
        Me.mnuPrint = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintPreview = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintSetup = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator
        Me.mnuExit = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuEdit = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuCopy = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator
        Me.mnuSelectAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuCompSelBase = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuCompSelComp = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuCompRunComp = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuView = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuToolbar = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuStatusBar = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuWindows = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuCascade = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuTileVert = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuTileHorz = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuCloseAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuArrangeAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuHelp = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuHelpScreen = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStrip = New System.Windows.Forms.ToolStrip
        Me.tlsbOpen = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuOpenFolder = New System.Windows.Forms.ToolStripMenuItem
        Me.tlsbSave = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.tlsbPrint = New System.Windows.Forms.ToolStripButton
        Me.tlsbPrintPreview = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
        Me.tlsbPrev = New System.Windows.Forms.ToolStripButton
        Me.tlsbNext = New System.Windows.Forms.ToolStripButton
        Me.tlsbSyncScroll = New System.Windows.Forms.ToolStripButton
        Me.tlsbCompare = New System.Windows.Forms.ToolStripButton
        Me.StatusStrip = New System.Windows.Forms.StatusStrip
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.MenuStrip.SuspendLayout()
        Me.ToolStrip.SuspendLayout()
        Me.StatusStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip
        '
        Me.MenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuFile, Me.mnuEdit, Me.mnuView, Me.mnuWindows, Me.mnuHelp})
        Me.MenuStrip.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip.MdiWindowListItem = Me.mnuWindows
        Me.MenuStrip.Name = "MenuStrip"
        Me.MenuStrip.Size = New System.Drawing.Size(1016, 24)
        Me.MenuStrip.TabIndex = 5
        Me.MenuStrip.Text = "MenuStrip"
        '
        'mnuFile
        '
        Me.mnuFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuOpen, Me.ToolStripSeparator3, Me.mnuSave, Me.ToolStripSeparator4, Me.mnuPrint, Me.mnuPrintPreview, Me.mnuPrintSetup, Me.ToolStripSeparator5, Me.mnuExit})
        Me.mnuFile.ImageTransparentColor = System.Drawing.SystemColors.ActiveBorder
        Me.mnuFile.Name = "mnuFile"
        Me.mnuFile.Size = New System.Drawing.Size(35, 20)
        Me.mnuFile.Text = "&File"
        '
        'mnuOpen
        '
        Me.mnuOpen.Image = CType(resources.GetObject("mnuOpen.Image"), System.Drawing.Image)
        Me.mnuOpen.ImageTransparentColor = System.Drawing.Color.Black
        Me.mnuOpen.Name = "mnuOpen"
        Me.mnuOpen.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
        Me.mnuOpen.Size = New System.Drawing.Size(136, 22)
        Me.mnuOpen.Text = "&Open"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(133, 6)
        '
        'mnuSave
        '
        Me.mnuSave.Image = CType(resources.GetObject("mnuSave.Image"), System.Drawing.Image)
        Me.mnuSave.ImageTransparentColor = System.Drawing.Color.Black
        Me.mnuSave.Name = "mnuSave"
        Me.mnuSave.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.mnuSave.Size = New System.Drawing.Size(136, 22)
        Me.mnuSave.Text = "&Save"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(133, 6)
        '
        'mnuPrint
        '
        Me.mnuPrint.Image = CType(resources.GetObject("mnuPrint.Image"), System.Drawing.Image)
        Me.mnuPrint.ImageTransparentColor = System.Drawing.Color.Black
        Me.mnuPrint.Name = "mnuPrint"
        Me.mnuPrint.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.P), System.Windows.Forms.Keys)
        Me.mnuPrint.Size = New System.Drawing.Size(136, 22)
        Me.mnuPrint.Text = "&Print"
        '
        'mnuPrintPreview
        '
        Me.mnuPrintPreview.Image = CType(resources.GetObject("mnuPrintPreview.Image"), System.Drawing.Image)
        Me.mnuPrintPreview.ImageTransparentColor = System.Drawing.Color.Black
        Me.mnuPrintPreview.Name = "mnuPrintPreview"
        Me.mnuPrintPreview.Size = New System.Drawing.Size(136, 22)
        Me.mnuPrintPreview.Text = "Print Pre&view"
        '
        'mnuPrintSetup
        '
        Me.mnuPrintSetup.Name = "mnuPrintSetup"
        Me.mnuPrintSetup.Size = New System.Drawing.Size(136, 22)
        Me.mnuPrintSetup.Text = "Print Setup"
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(133, 6)
        '
        'mnuExit
        '
        Me.mnuExit.Name = "mnuExit"
        Me.mnuExit.Size = New System.Drawing.Size(136, 22)
        Me.mnuExit.Text = "E&xit"
        '
        'mnuEdit
        '
        Me.mnuEdit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuCopy, Me.ToolStripSeparator7, Me.mnuSelectAll, Me.mnuCompSelBase, Me.mnuCompSelComp, Me.mnuCompRunComp})
        Me.mnuEdit.Name = "mnuEdit"
        Me.mnuEdit.Size = New System.Drawing.Size(37, 20)
        Me.mnuEdit.Text = "&Edit"
        '
        'mnuCopy
        '
        Me.mnuCopy.Image = CType(resources.GetObject("mnuCopy.Image"), System.Drawing.Image)
        Me.mnuCopy.ImageTransparentColor = System.Drawing.Color.Black
        Me.mnuCopy.Name = "mnuCopy"
        Me.mnuCopy.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
        Me.mnuCopy.Size = New System.Drawing.Size(163, 22)
        Me.mnuCopy.Text = "&Copy"
        '
        'ToolStripSeparator7
        '
        Me.ToolStripSeparator7.Name = "ToolStripSeparator7"
        Me.ToolStripSeparator7.Size = New System.Drawing.Size(160, 6)
        '
        'mnuSelectAll
        '
        Me.mnuSelectAll.Name = "mnuSelectAll"
        Me.mnuSelectAll.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.A), System.Windows.Forms.Keys)
        Me.mnuSelectAll.Size = New System.Drawing.Size(163, 22)
        Me.mnuSelectAll.Text = "Select &All"
        '
        'mnuCompSelBase
        '
        Me.mnuCompSelBase.Name = "mnuCompSelBase"
        Me.mnuCompSelBase.Size = New System.Drawing.Size(163, 22)
        Me.mnuCompSelBase.Text = "mnuCompSelBase"
        '
        'mnuCompSelComp
        '
        Me.mnuCompSelComp.Name = "mnuCompSelComp"
        Me.mnuCompSelComp.Size = New System.Drawing.Size(163, 22)
        Me.mnuCompSelComp.Text = "mnuCompSelComp"
        '
        'mnuCompRunComp
        '
        Me.mnuCompRunComp.Name = "mnuCompRunComp"
        Me.mnuCompRunComp.Size = New System.Drawing.Size(163, 22)
        Me.mnuCompRunComp.Text = "mnuCompare"
        '
        'mnuView
        '
        Me.mnuView.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuToolbar, Me.mnuStatusBar})
        Me.mnuView.Name = "mnuView"
        Me.mnuView.Size = New System.Drawing.Size(42, 20)
        Me.mnuView.Text = "&View"
        '
        'mnuToolbar
        '
        Me.mnuToolbar.Checked = True
        Me.mnuToolbar.CheckOnClick = True
        Me.mnuToolbar.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuToolbar.Name = "mnuToolbar"
        Me.mnuToolbar.Size = New System.Drawing.Size(123, 22)
        Me.mnuToolbar.Text = "&Toolbar"
        '
        'mnuStatusBar
        '
        Me.mnuStatusBar.Checked = True
        Me.mnuStatusBar.CheckOnClick = True
        Me.mnuStatusBar.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuStatusBar.Name = "mnuStatusBar"
        Me.mnuStatusBar.Size = New System.Drawing.Size(123, 22)
        Me.mnuStatusBar.Text = "&Status Bar"
        '
        'mnuWindows
        '
        Me.mnuWindows.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuCascade, Me.mnuTileVert, Me.mnuTileHorz, Me.mnuCloseAll, Me.mnuArrangeAll})
        Me.mnuWindows.Name = "mnuWindows"
        Me.mnuWindows.Size = New System.Drawing.Size(63, 20)
        Me.mnuWindows.Text = "&Windows"
        '
        'mnuCascade
        '
        Me.mnuCascade.Name = "mnuCascade"
        Me.mnuCascade.Size = New System.Drawing.Size(141, 22)
        Me.mnuCascade.Text = "&Cascade"
        '
        'mnuTileVert
        '
        Me.mnuTileVert.Name = "mnuTileVert"
        Me.mnuTileVert.Size = New System.Drawing.Size(141, 22)
        Me.mnuTileVert.Text = "Tile &Vertical"
        '
        'mnuTileHorz
        '
        Me.mnuTileHorz.Name = "mnuTileHorz"
        Me.mnuTileHorz.Size = New System.Drawing.Size(141, 22)
        Me.mnuTileHorz.Text = "Tile &Horizontal"
        '
        'mnuCloseAll
        '
        Me.mnuCloseAll.Name = "mnuCloseAll"
        Me.mnuCloseAll.Size = New System.Drawing.Size(141, 22)
        Me.mnuCloseAll.Text = "C&lose All"
        '
        'mnuArrangeAll
        '
        Me.mnuArrangeAll.Name = "mnuArrangeAll"
        Me.mnuArrangeAll.Size = New System.Drawing.Size(141, 22)
        Me.mnuArrangeAll.Text = "&Arrange Icons"
        '
        'mnuHelp
        '
        Me.mnuHelp.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuHelpScreen})
        Me.mnuHelp.Name = "mnuHelp"
        Me.mnuHelp.Size = New System.Drawing.Size(41, 20)
        Me.mnuHelp.Text = "&Help"
        '
        'mnuHelpScreen
        '
        Me.mnuHelpScreen.Name = "mnuHelpScreen"
        Me.mnuHelpScreen.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.F1), System.Windows.Forms.Keys)
        Me.mnuHelpScreen.Size = New System.Drawing.Size(156, 22)
        Me.mnuHelpScreen.Text = "&Contents"
        '
        'ToolStrip
        '
        Me.ToolStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tlsbOpen, Me.tlsbSave, Me.ToolStripSeparator1, Me.tlsbPrint, Me.tlsbPrintPreview, Me.ToolStripSeparator2, Me.tlsbPrev, Me.tlsbNext, Me.tlsbSyncScroll, Me.tlsbCompare})
        Me.ToolStrip.Location = New System.Drawing.Point(0, 24)
        Me.ToolStrip.Name = "ToolStrip"
        Me.ToolStrip.Size = New System.Drawing.Size(1016, 25)
        Me.ToolStrip.TabIndex = 6
        Me.ToolStrip.Text = "ToolStrip"
        '
        'tlsbOpen
        '
        Me.tlsbOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tlsbOpen.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuOpenFolder})
        Me.tlsbOpen.Image = CType(resources.GetObject("tlsbOpen.Image"), System.Drawing.Image)
        Me.tlsbOpen.ImageTransparentColor = System.Drawing.Color.Black
        Me.tlsbOpen.Name = "tlsbOpen"
        Me.tlsbOpen.Size = New System.Drawing.Size(32, 22)
        Me.tlsbOpen.Text = "Open"
        '
        'mnuOpenFolder
        '
        Me.mnuOpenFolder.Name = "mnuOpenFolder"
        Me.mnuOpenFolder.Size = New System.Drawing.Size(149, 22)
        Me.mnuOpenFolder.Text = "mnuOpenFolder"
        '
        'tlsbSave
        '
        Me.tlsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tlsbSave.Image = CType(resources.GetObject("tlsbSave.Image"), System.Drawing.Image)
        Me.tlsbSave.ImageTransparentColor = System.Drawing.Color.Black
        Me.tlsbSave.Name = "tlsbSave"
        Me.tlsbSave.Size = New System.Drawing.Size(23, 22)
        Me.tlsbSave.Text = "Save"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'tlsbPrint
        '
        Me.tlsbPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tlsbPrint.Image = CType(resources.GetObject("tlsbPrint.Image"), System.Drawing.Image)
        Me.tlsbPrint.ImageTransparentColor = System.Drawing.Color.Black
        Me.tlsbPrint.Name = "tlsbPrint"
        Me.tlsbPrint.Size = New System.Drawing.Size(23, 22)
        Me.tlsbPrint.Text = "Print"
        '
        'tlsbPrintPreview
        '
        Me.tlsbPrintPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tlsbPrintPreview.Image = CType(resources.GetObject("tlsbPrintPreview.Image"), System.Drawing.Image)
        Me.tlsbPrintPreview.ImageTransparentColor = System.Drawing.Color.Black
        Me.tlsbPrintPreview.Name = "tlsbPrintPreview"
        Me.tlsbPrintPreview.Size = New System.Drawing.Size(23, 22)
        Me.tlsbPrintPreview.Text = "Print Preview"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'tlsbPrev
        '
        Me.tlsbPrev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tlsbPrev.Image = CType(resources.GetObject("tlsbPrev.Image"), System.Drawing.Image)
        Me.tlsbPrev.ImageTransparentColor = System.Drawing.Color.Silver
        Me.tlsbPrev.Name = "tlsbPrev"
        Me.tlsbPrev.Size = New System.Drawing.Size(23, 22)
        Me.tlsbPrev.Text = "tlsbPrev"
        Me.tlsbPrev.ToolTipText = "tlsbPrev"
        Me.tlsbPrev.Visible = False
        '
        'tlsbNext
        '
        Me.tlsbNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tlsbNext.Image = CType(resources.GetObject("tlsbNext.Image"), System.Drawing.Image)
        Me.tlsbNext.ImageTransparentColor = System.Drawing.Color.Silver
        Me.tlsbNext.Name = "tlsbNext"
        Me.tlsbNext.Size = New System.Drawing.Size(23, 22)
        Me.tlsbNext.Text = "tlsbNext"
        Me.tlsbNext.ToolTipText = "tlsbNext"
        Me.tlsbNext.Visible = False
        '
        'tlsbSyncScroll
        '
        Me.tlsbSyncScroll.CheckOnClick = True
        Me.tlsbSyncScroll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tlsbSyncScroll.Image = CType(resources.GetObject("tlsbSyncScroll.Image"), System.Drawing.Image)
        Me.tlsbSyncScroll.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tlsbSyncScroll.Name = "tlsbSyncScroll"
        Me.tlsbSyncScroll.Size = New System.Drawing.Size(77, 22)
        Me.tlsbSyncScroll.Text = "tlsbSyncScroll"
        Me.tlsbSyncScroll.Visible = False
        '
        'tlsbCompare
        '
        Me.tlsbCompare.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tlsbCompare.Image = CType(resources.GetObject("tlsbCompare.Image"), System.Drawing.Image)
        Me.tlsbCompare.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tlsbCompare.Name = "tlsbCompare"
        Me.tlsbCompare.Size = New System.Drawing.Size(69, 22)
        Me.tlsbCompare.Text = "tlsbCompare"
        '
        'StatusStrip
        '
        Me.StatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatus})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 712)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(1016, 22)
        Me.StatusStrip.TabIndex = 7
        Me.StatusStrip.Text = "StatusStrip"
        '
        'lblStatus
        '
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(37, 17)
        Me.lblStatus.Text = "Status"
        '
        'frmFileViewCompare
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1016, 734)
        Me.Controls.Add(Me.ToolStrip)
        Me.Controls.Add(Me.MenuStrip)
        Me.Controls.Add(Me.StatusStrip)
        Me.IsMdiContainer = True
        Me.KeyPreview = True
        Me.MainMenuStrip = Me.MenuStrip
        Me.Name = "frmFileViewCompare"
        Me.Text = "frmFileViewCompare"
        Me.MenuStrip.ResumeLayout(False)
        Me.MenuStrip.PerformLayout()
        Me.ToolStrip.ResumeLayout(False)
        Me.ToolStrip.PerformLayout()
        Me.StatusStrip.ResumeLayout(False)
        Me.StatusStrip.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents mnuHelpScreen As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuHelp As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuArrangeAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuCloseAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuWindows As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuCascade As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuTileVert As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuTileHorz As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tlsbPrintPreview As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolTip As System.Windows.Forms.ToolTip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents StatusStrip As System.Windows.Forms.StatusStrip
    Friend WithEvents tlsbPrint As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStrip As System.Windows.Forms.ToolStrip
    Friend WithEvents tlsbSave As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuPrintPreview As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrint As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuExit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuPrintSetup As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuOpen As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuSave As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MenuStrip As System.Windows.Forms.MenuStrip
    Friend WithEvents mnuEdit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuCopy As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuSelectAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuView As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuToolbar As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuStatusBar As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuCompRunComp As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuCompSelBase As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuCompSelComp As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tlsbPrev As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlsbNext As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlsbCompare As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlsbSyncScroll As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlsbOpen As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuOpenFolder As System.Windows.Forms.ToolStripMenuItem

End Class
