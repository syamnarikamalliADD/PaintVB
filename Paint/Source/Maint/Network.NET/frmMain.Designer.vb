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
        Me.tlsList = New System.Windows.Forms.ToolStrip
        Me.tlsbControllers = New System.Windows.Forms.ToolStripButton
        Me.tlsbHostFileItems = New System.Windows.Forms.ToolStripButton
        Me.txtAddress = New System.Windows.Forms.TextBox
        Me.pnlHosts = New System.Windows.Forms.Panel
        Me.rtbHosts = New System.Windows.Forms.RichTextBox
        Me.tlsHosts = New System.Windows.Forms.ToolStrip
        Me.tlspSaveHosts = New System.Windows.Forms.ToolStripButton
        Me.tlspPrintHosts = New System.Windows.Forms.ToolStripSplitButton
        Me.MnuPrintHosts = New System.Windows.Forms.ToolStripMenuItem
        Me.MnuPreviewHosts = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPageSetupHosts = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintFileHosts = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator
        Me.tlspCutHosts = New System.Windows.Forms.ToolStripButton
        Me.tlspCopyHosts = New System.Windows.Forms.ToolStripButton
        Me.tlspPasteHosts = New System.Windows.Forms.ToolStripButton
        Me.toolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator
        Me.btnHosts = New System.Windows.Forms.ToolStripButton
        Me.btnBootP = New System.Windows.Forms.ToolStripButton
        Me.btnTFTP = New System.Windows.Forms.ToolStripButton
        Me.btnAutoConfig = New System.Windows.Forms.ToolStripButton
        Me.pnlCmds = New System.Windows.Forms.Panel
        Me.tscCmds = New System.Windows.Forms.ToolStripContainer
        Me.lstStatus = New System.Windows.Forms.ListBox
        Me.rtbCmds = New System.Windows.Forms.RichTextBox
        Me.tlsCmdsLeft = New System.Windows.Forms.ToolStrip
        Me.tlsbFanucPing = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
        Me.tlsbPing = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator
        Me.tlsbArp = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator
        Me.tlsbTraceRoute = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator
        Me.tlsbIPConfig = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator
        Me.tlsbECBRUpdate = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator14 = New System.Windows.Forms.ToolStripSeparator
        Me.tlsbTFTPStatus = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator15 = New System.Windows.Forms.ToolStripSeparator
        Me.tlsbBOOTPStatus = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator12 = New System.Windows.Forms.ToolStripSeparator
        Me.tlsbTFTPStart = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator13 = New System.Windows.Forms.ToolStripSeparator
        Me.tlsbBOOTPStart = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator10 = New System.Windows.Forms.ToolStripSeparator
        Me.tlsbTFTPStop = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator11 = New System.Windows.Forms.ToolStripSeparator
        Me.tlsbBOOTPStop = New System.Windows.Forms.ToolStripButton
        Me.tlsCmdsTop = New System.Windows.Forms.ToolStrip
        Me.tlspSaveCmds = New System.Windows.Forms.ToolStripButton
        Me.tlspPrintCmds = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuPrintCmds = New System.Windows.Forms.ToolStripMenuItem
        Me.MnuPreviewCmds = New System.Windows.Forms.ToolStripMenuItem
        Me.MnuPageSetupCmds = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintFileCmds = New System.Windows.Forms.ToolStripMenuItem
        Me.toolStripSeparator = New System.Windows.Forms.ToolStripSeparator
        Me.tlspCutCmds = New System.Windows.Forms.ToolStripButton
        Me.tlspCopyCmds = New System.Windows.Forms.ToolStripButton
        Me.tlspPasteCmds = New System.Windows.Forms.ToolStripButton
        Me.toolStripSeparator9 = New System.Windows.Forms.ToolStripSeparator
        Me.tlspClearCmds = New System.Windows.Forms.ToolStripButton
        Me.tlsLblCmds = New System.Windows.Forms.ToolStripLabel
        Me.lblManualAddress = New System.Windows.Forms.Label
        Me.chkSelectAddresses = New System.Windows.Forms.CheckedListBox
        Me.mnuCheckList = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuSelectAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuUnselectAll = New System.Windows.Forms.ToolStripMenuItem
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.btnCancel = New System.Windows.Forms.ToolStripButton
        Me.btnChangeLog = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuLast24 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLast7 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAllChanges = New System.Windows.Forms.ToolStripMenuItem
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
        Me.tlpMain.SuspendLayout()
        Me.tlsList.SuspendLayout()
        Me.pnlHosts.SuspendLayout()
        Me.tlsHosts.SuspendLayout()
        Me.pnlCmds.SuspendLayout()
        Me.tscCmds.ContentPanel.SuspendLayout()
        Me.tscCmds.LeftToolStripPanel.SuspendLayout()
        Me.tscCmds.TopToolStripPanel.SuspendLayout()
        Me.tscCmds.SuspendLayout()
        Me.tlsCmdsLeft.SuspendLayout()
        Me.tlsCmdsTop.SuspendLayout()
        Me.mnuCheckList.SuspendLayout()
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
        Me.tlpMain.Controls.Add(Me.tlsList, 0, 0)
        Me.tlpMain.Controls.Add(Me.txtAddress, 0, 3)
        Me.tlpMain.Controls.Add(Me.pnlHosts, 0, 4)
        Me.tlpMain.Controls.Add(Me.pnlCmds, 1, 0)
        Me.tlpMain.Controls.Add(Me.lblManualAddress, 0, 2)
        Me.tlpMain.Controls.Add(Me.chkSelectAddresses, 0, 1)
        Me.tlpMain.Name = "tlpMain"
        '
        'tlsList
        '
        Me.tlsList.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tlsbControllers, Me.tlsbHostFileItems})
        resources.ApplyResources(Me.tlsList, "tlsList")
        Me.tlsList.Name = "tlsList"
        '
        'tlsbControllers
        '
        Me.tlsbControllers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.tlsbControllers, "tlsbControllers")
        Me.tlsbControllers.Name = "tlsbControllers"
        '
        'tlsbHostFileItems
        '
        Me.tlsbHostFileItems.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.tlsbHostFileItems, "tlsbHostFileItems")
        Me.tlsbHostFileItems.Name = "tlsbHostFileItems"
        '
        'txtAddress
        '
        resources.ApplyResources(Me.txtAddress, "txtAddress")
        Me.txtAddress.Name = "txtAddress"
        '
        'pnlHosts
        '
        Me.tlpMain.SetColumnSpan(Me.pnlHosts, 2)
        Me.pnlHosts.Controls.Add(Me.rtbHosts)
        Me.pnlHosts.Controls.Add(Me.tlsHosts)
        resources.ApplyResources(Me.pnlHosts, "pnlHosts")
        Me.pnlHosts.Name = "pnlHosts"
        '
        'rtbHosts
        '
        Me.rtbHosts.AcceptsTab = True
        resources.ApplyResources(Me.rtbHosts, "rtbHosts")
        Me.rtbHosts.Name = "rtbHosts"
        '
        'tlsHosts
        '
        Me.tlsHosts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tlspSaveHosts, Me.tlspPrintHosts, Me.ToolStripSeparator7, Me.tlspCutHosts, Me.tlspCopyHosts, Me.tlspPasteHosts, Me.toolStripSeparator5, Me.btnHosts, Me.btnBootP, Me.btnTFTP, Me.btnAutoConfig})
        resources.ApplyResources(Me.tlsHosts, "tlsHosts")
        Me.tlsHosts.Name = "tlsHosts"
        '
        'tlspSaveHosts
        '
        Me.tlspSaveHosts.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.tlspSaveHosts, "tlspSaveHosts")
        Me.tlspSaveHosts.Name = "tlspSaveHosts"
        '
        'tlspPrintHosts
        '
        Me.tlspPrintHosts.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tlspPrintHosts.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuPrintHosts, Me.MnuPreviewHosts, Me.mnuPageSetupHosts, Me.mnuPrintFileHosts})
        resources.ApplyResources(Me.tlspPrintHosts, "tlspPrintHosts")
        Me.tlspPrintHosts.Name = "tlspPrintHosts"
        '
        'MnuPrintHosts
        '
        Me.MnuPrintHosts.Name = "MnuPrintHosts"
        resources.ApplyResources(Me.MnuPrintHosts, "MnuPrintHosts")
        '
        'MnuPreviewHosts
        '
        Me.MnuPreviewHosts.Name = "MnuPreviewHosts"
        resources.ApplyResources(Me.MnuPreviewHosts, "MnuPreviewHosts")
        '
        'mnuPageSetupHosts
        '
        Me.mnuPageSetupHosts.Name = "mnuPageSetupHosts"
        resources.ApplyResources(Me.mnuPageSetupHosts, "mnuPageSetupHosts")
        '
        'mnuPrintFileHosts
        '
        Me.mnuPrintFileHosts.Name = "mnuPrintFileHosts"
        resources.ApplyResources(Me.mnuPrintFileHosts, "mnuPrintFileHosts")
        '
        'ToolStripSeparator7
        '
        Me.ToolStripSeparator7.Name = "ToolStripSeparator7"
        resources.ApplyResources(Me.ToolStripSeparator7, "ToolStripSeparator7")
        '
        'tlspCutHosts
        '
        Me.tlspCutHosts.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.tlspCutHosts, "tlspCutHosts")
        Me.tlspCutHosts.Name = "tlspCutHosts"
        '
        'tlspCopyHosts
        '
        Me.tlspCopyHosts.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.tlspCopyHosts, "tlspCopyHosts")
        Me.tlspCopyHosts.Name = "tlspCopyHosts"
        '
        'tlspPasteHosts
        '
        Me.tlspPasteHosts.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.tlspPasteHosts, "tlspPasteHosts")
        Me.tlspPasteHosts.Name = "tlspPasteHosts"
        '
        'toolStripSeparator5
        '
        Me.toolStripSeparator5.Name = "toolStripSeparator5"
        resources.ApplyResources(Me.toolStripSeparator5, "toolStripSeparator5")
        '
        'btnHosts
        '
        Me.btnHosts.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.btnHosts, "btnHosts")
        Me.btnHosts.Name = "btnHosts"
        '
        'btnBootP
        '
        Me.btnBootP.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.btnBootP, "btnBootP")
        Me.btnBootP.Name = "btnBootP"
        '
        'btnTFTP
        '
        Me.btnTFTP.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.btnTFTP, "btnTFTP")
        Me.btnTFTP.Name = "btnTFTP"
        '
        'btnAutoConfig
        '
        Me.btnAutoConfig.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.btnAutoConfig, "btnAutoConfig")
        Me.btnAutoConfig.Name = "btnAutoConfig"
        '
        'pnlCmds
        '
        Me.pnlCmds.Controls.Add(Me.tscCmds)
        resources.ApplyResources(Me.pnlCmds, "pnlCmds")
        Me.pnlCmds.Name = "pnlCmds"
        Me.tlpMain.SetRowSpan(Me.pnlCmds, 4)
        '
        'tscCmds
        '
        '
        'tscCmds.ContentPanel
        '
        Me.tscCmds.ContentPanel.Controls.Add(Me.lstStatus)
        Me.tscCmds.ContentPanel.Controls.Add(Me.rtbCmds)
        resources.ApplyResources(Me.tscCmds.ContentPanel, "tscCmds.ContentPanel")
        resources.ApplyResources(Me.tscCmds, "tscCmds")
        '
        'tscCmds.LeftToolStripPanel
        '
        Me.tscCmds.LeftToolStripPanel.Controls.Add(Me.tlsCmdsLeft)
        Me.tscCmds.Name = "tscCmds"
        '
        'tscCmds.TopToolStripPanel
        '
        Me.tscCmds.TopToolStripPanel.Controls.Add(Me.tlsCmdsTop)
        '
        'lstStatus
        '
        resources.ApplyResources(Me.lstStatus, "lstStatus")
        Me.lstStatus.BackColor = System.Drawing.SystemColors.Info
        Me.lstStatus.Name = "lstStatus"
        '
        'rtbCmds
        '
        Me.rtbCmds.AcceptsTab = True
        resources.ApplyResources(Me.rtbCmds, "rtbCmds")
        Me.rtbCmds.EnableAutoDragDrop = True
        Me.rtbCmds.Name = "rtbCmds"
        '
        'tlsCmdsLeft
        '
        resources.ApplyResources(Me.tlsCmdsLeft, "tlsCmdsLeft")
        Me.tlsCmdsLeft.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tlsbFanucPing, Me.ToolStripSeparator2, Me.tlsbPing, Me.ToolStripSeparator3, Me.tlsbArp, Me.ToolStripSeparator4, Me.tlsbTraceRoute, Me.ToolStripSeparator6, Me.tlsbIPConfig, Me.ToolStripSeparator8, Me.tlsbECBRUpdate, Me.ToolStripSeparator14, Me.tlsbTFTPStatus, Me.ToolStripSeparator15, Me.tlsbBOOTPStatus, Me.ToolStripSeparator12, Me.tlsbTFTPStart, Me.ToolStripSeparator13, Me.tlsbBOOTPStart, Me.ToolStripSeparator10, Me.tlsbTFTPStop, Me.ToolStripSeparator11, Me.tlsbBOOTPStop})
        Me.tlsCmdsLeft.Name = "tlsCmdsLeft"
        Me.tlsCmdsLeft.Stretch = True
        '
        'tlsbFanucPing
        '
        Me.tlsbFanucPing.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.tlsbFanucPing, "tlsbFanucPing")
        Me.tlsbFanucPing.Name = "tlsbFanucPing"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        resources.ApplyResources(Me.ToolStripSeparator2, "ToolStripSeparator2")
        '
        'tlsbPing
        '
        Me.tlsbPing.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.tlsbPing, "tlsbPing")
        Me.tlsbPing.Name = "tlsbPing"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        resources.ApplyResources(Me.ToolStripSeparator3, "ToolStripSeparator3")
        '
        'tlsbArp
        '
        Me.tlsbArp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.tlsbArp, "tlsbArp")
        Me.tlsbArp.Name = "tlsbArp"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        resources.ApplyResources(Me.ToolStripSeparator4, "ToolStripSeparator4")
        '
        'tlsbTraceRoute
        '
        Me.tlsbTraceRoute.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.tlsbTraceRoute, "tlsbTraceRoute")
        Me.tlsbTraceRoute.Name = "tlsbTraceRoute"
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        resources.ApplyResources(Me.ToolStripSeparator6, "ToolStripSeparator6")
        '
        'tlsbIPConfig
        '
        Me.tlsbIPConfig.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.tlsbIPConfig, "tlsbIPConfig")
        Me.tlsbIPConfig.Name = "tlsbIPConfig"
        '
        'ToolStripSeparator8
        '
        Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
        resources.ApplyResources(Me.ToolStripSeparator8, "ToolStripSeparator8")
        '
        'tlsbECBRUpdate
        '
        Me.tlsbECBRUpdate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.tlsbECBRUpdate, "tlsbECBRUpdate")
        Me.tlsbECBRUpdate.Name = "tlsbECBRUpdate"
        '
        'ToolStripSeparator14
        '
        resources.ApplyResources(Me.ToolStripSeparator14, "ToolStripSeparator14")
        Me.ToolStripSeparator14.Name = "ToolStripSeparator14"
        '
        'tlsbTFTPStatus
        '
        resources.ApplyResources(Me.tlsbTFTPStatus, "tlsbTFTPStatus")
        Me.tlsbTFTPStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tlsbTFTPStatus.Name = "tlsbTFTPStatus"
        '
        'ToolStripSeparator15
        '
        resources.ApplyResources(Me.ToolStripSeparator15, "ToolStripSeparator15")
        Me.ToolStripSeparator15.Name = "ToolStripSeparator15"
        '
        'tlsbBOOTPStatus
        '
        resources.ApplyResources(Me.tlsbBOOTPStatus, "tlsbBOOTPStatus")
        Me.tlsbBOOTPStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tlsbBOOTPStatus.Name = "tlsbBOOTPStatus"
        '
        'ToolStripSeparator12
        '
        resources.ApplyResources(Me.ToolStripSeparator12, "ToolStripSeparator12")
        Me.ToolStripSeparator12.Name = "ToolStripSeparator12"
        '
        'tlsbTFTPStart
        '
        resources.ApplyResources(Me.tlsbTFTPStart, "tlsbTFTPStart")
        Me.tlsbTFTPStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tlsbTFTPStart.Name = "tlsbTFTPStart"
        '
        'ToolStripSeparator13
        '
        resources.ApplyResources(Me.ToolStripSeparator13, "ToolStripSeparator13")
        Me.ToolStripSeparator13.Name = "ToolStripSeparator13"
        '
        'tlsbBOOTPStart
        '
        resources.ApplyResources(Me.tlsbBOOTPStart, "tlsbBOOTPStart")
        Me.tlsbBOOTPStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tlsbBOOTPStart.Name = "tlsbBOOTPStart"
        '
        'ToolStripSeparator10
        '
        resources.ApplyResources(Me.ToolStripSeparator10, "ToolStripSeparator10")
        Me.ToolStripSeparator10.Name = "ToolStripSeparator10"
        '
        'tlsbTFTPStop
        '
        resources.ApplyResources(Me.tlsbTFTPStop, "tlsbTFTPStop")
        Me.tlsbTFTPStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tlsbTFTPStop.Name = "tlsbTFTPStop"
        '
        'ToolStripSeparator11
        '
        resources.ApplyResources(Me.ToolStripSeparator11, "ToolStripSeparator11")
        Me.ToolStripSeparator11.Name = "ToolStripSeparator11"
        '
        'tlsbBOOTPStop
        '
        resources.ApplyResources(Me.tlsbBOOTPStop, "tlsbBOOTPStop")
        Me.tlsbBOOTPStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tlsbBOOTPStop.Name = "tlsbBOOTPStop"
        '
        'tlsCmdsTop
        '
        resources.ApplyResources(Me.tlsCmdsTop, "tlsCmdsTop")
        Me.tlsCmdsTop.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tlspSaveCmds, Me.tlspPrintCmds, Me.toolStripSeparator, Me.tlspCutCmds, Me.tlspCopyCmds, Me.tlspPasteCmds, Me.toolStripSeparator9, Me.tlspClearCmds, Me.tlsLblCmds})
        Me.tlsCmdsTop.Name = "tlsCmdsTop"
        Me.tlsCmdsTop.Stretch = True
        '
        'tlspSaveCmds
        '
        Me.tlspSaveCmds.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.tlspSaveCmds, "tlspSaveCmds")
        Me.tlspSaveCmds.Name = "tlspSaveCmds"
        '
        'tlspPrintCmds
        '
        Me.tlspPrintCmds.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tlspPrintCmds.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuPrintCmds, Me.MnuPreviewCmds, Me.MnuPageSetupCmds, Me.mnuPrintFileCmds})
        resources.ApplyResources(Me.tlspPrintCmds, "tlspPrintCmds")
        Me.tlspPrintCmds.Name = "tlspPrintCmds"
        '
        'mnuPrintCmds
        '
        Me.mnuPrintCmds.Name = "mnuPrintCmds"
        resources.ApplyResources(Me.mnuPrintCmds, "mnuPrintCmds")
        '
        'MnuPreviewCmds
        '
        Me.MnuPreviewCmds.Name = "MnuPreviewCmds"
        resources.ApplyResources(Me.MnuPreviewCmds, "MnuPreviewCmds")
        '
        'MnuPageSetupCmds
        '
        Me.MnuPageSetupCmds.Name = "MnuPageSetupCmds"
        resources.ApplyResources(Me.MnuPageSetupCmds, "MnuPageSetupCmds")
        '
        'mnuPrintFileCmds
        '
        Me.mnuPrintFileCmds.Name = "mnuPrintFileCmds"
        resources.ApplyResources(Me.mnuPrintFileCmds, "mnuPrintFileCmds")
        '
        'toolStripSeparator
        '
        Me.toolStripSeparator.Name = "toolStripSeparator"
        resources.ApplyResources(Me.toolStripSeparator, "toolStripSeparator")
        '
        'tlspCutCmds
        '
        Me.tlspCutCmds.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.tlspCutCmds, "tlspCutCmds")
        Me.tlspCutCmds.Name = "tlspCutCmds"
        '
        'tlspCopyCmds
        '
        Me.tlspCopyCmds.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.tlspCopyCmds, "tlspCopyCmds")
        Me.tlspCopyCmds.Name = "tlspCopyCmds"
        '
        'tlspPasteCmds
        '
        Me.tlspPasteCmds.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.tlspPasteCmds, "tlspPasteCmds")
        Me.tlspPasteCmds.Name = "tlspPasteCmds"
        '
        'toolStripSeparator9
        '
        Me.toolStripSeparator9.Name = "toolStripSeparator9"
        resources.ApplyResources(Me.toolStripSeparator9, "toolStripSeparator9")
        '
        'tlspClearCmds
        '
        Me.tlspClearCmds.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.tlspClearCmds, "tlspClearCmds")
        Me.tlspClearCmds.Name = "tlspClearCmds"
        '
        'tlsLblCmds
        '
        Me.tlsLblCmds.Name = "tlsLblCmds"
        resources.ApplyResources(Me.tlsLblCmds, "tlsLblCmds")
        '
        'lblManualAddress
        '
        resources.ApplyResources(Me.lblManualAddress, "lblManualAddress")
        Me.lblManualAddress.Name = "lblManualAddress"
        '
        'chkSelectAddresses
        '
        Me.chkSelectAddresses.CheckOnClick = True
        Me.chkSelectAddresses.ContextMenuStrip = Me.mnuCheckList
        resources.ApplyResources(Me.chkSelectAddresses, "chkSelectAddresses")
        Me.chkSelectAddresses.FormattingEnabled = True
        Me.chkSelectAddresses.Name = "chkSelectAddresses"
        '
        'mnuCheckList
        '
        Me.mnuCheckList.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuSelectAll, Me.mnuUnselectAll})
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
        'tlsMain
        '
        resources.ApplyResources(Me.tlsMain, "tlsMain")
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.ToolStripSeparator1, Me.btnCancel, Me.btnChangeLog})
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
        'btnCancel
        '
        resources.ApplyResources(Me.btnCancel, "btnCancel")
        Me.btnCancel.Name = "btnCancel"
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
        Me.tlpMain.ResumeLayout(False)
        Me.tlpMain.PerformLayout()
        Me.tlsList.ResumeLayout(False)
        Me.tlsList.PerformLayout()
        Me.pnlHosts.ResumeLayout(False)
        Me.pnlHosts.PerformLayout()
        Me.tlsHosts.ResumeLayout(False)
        Me.tlsHosts.PerformLayout()
        Me.pnlCmds.ResumeLayout(False)
        Me.tscCmds.ContentPanel.ResumeLayout(False)
        Me.tscCmds.LeftToolStripPanel.ResumeLayout(False)
        Me.tscCmds.LeftToolStripPanel.PerformLayout()
        Me.tscCmds.TopToolStripPanel.ResumeLayout(False)
        Me.tscCmds.TopToolStripPanel.PerformLayout()
        Me.tscCmds.ResumeLayout(False)
        Me.tscCmds.PerformLayout()
        Me.tlsCmdsLeft.ResumeLayout(False)
        Me.tlsCmdsLeft.PerformLayout()
        Me.tlsCmdsTop.ResumeLayout(False)
        Me.tlsCmdsTop.PerformLayout()
        Me.mnuCheckList.ResumeLayout(False)
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
    Friend WithEvents tlpMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents pnlHosts As System.Windows.Forms.Panel
    Friend WithEvents pnlCmds As System.Windows.Forms.Panel
    Friend WithEvents rtbCmds As System.Windows.Forms.RichTextBox
    Friend WithEvents tlsCmdsLeft As System.Windows.Forms.ToolStrip
    Friend WithEvents tlsbPing As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlsCmdsTop As System.Windows.Forms.ToolStrip
    Friend WithEvents tlsbFanucPing As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlspSaveCmds As System.Windows.Forms.ToolStripButton
    Friend WithEvents toolStripSeparator As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tlspCutCmds As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlspCopyCmds As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlspPasteCmds As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlsbArp As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlsbTraceRoute As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlsbIPConfig As System.Windows.Forms.ToolStripButton
    Friend WithEvents txtAddress As System.Windows.Forms.TextBox
    Friend WithEvents chkSelectAddresses As System.Windows.Forms.CheckedListBox
    Friend WithEvents lblManualAddress As System.Windows.Forms.Label
    Friend WithEvents rtbHosts As System.Windows.Forms.RichTextBox
    Friend WithEvents tlsHosts As System.Windows.Forms.ToolStrip
    Friend WithEvents tlspSaveHosts As System.Windows.Forms.ToolStripButton
    Friend WithEvents toolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tlspCutHosts As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlspCopyHosts As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlspPasteHosts As System.Windows.Forms.ToolStripButton
    Friend WithEvents BottomToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents TopToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents RightToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents LeftToolStripPanel As System.Windows.Forms.ToolStripPanel
    Friend WithEvents ContentPanel As System.Windows.Forms.ToolStripContentPanel
    Friend WithEvents lstStatus As System.Windows.Forms.ListBox
    Friend WithEvents tlsList As System.Windows.Forms.ToolStrip
    Friend WithEvents tlsbControllers As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlsbHostFileItems As System.Windows.Forms.ToolStripButton
    Friend WithEvents tscCmds As System.Windows.Forms.ToolStripContainer
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuCheckList As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuSelectAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuUnselectAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripSeparator9 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tlspClearCmds As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnCancel As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlsLblCmds As System.Windows.Forms.ToolStripLabel
    Friend WithEvents MnuPreviewCmds As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPageSetupCmds As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tlspPrintHosts As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents MnuPreviewHosts As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPageSetupHosts As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPrintHosts As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintCmds As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnChangeLog As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuLast24 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLast7 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuAllChanges As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuPrintFileHosts As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintFileCmds As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents tlspPrintCmds As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents btnHosts As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnBootP As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnTFTP As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator8 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tlsbBOOTPStop As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnAutoConfig As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlsbECBRUpdate As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlsbTFTPStop As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator10 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator11 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator14 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tlsbTFTPStatus As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator15 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tlsbBOOTPStatus As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator12 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tlsbTFTPStart As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator13 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tlsbBOOTPStart As System.Windows.Forms.ToolStripButton
End Class
