<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        If mSQLDb IsNot Nothing Then
            mSQLDb.Dispose()
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Me.tmrNewAlarm = New System.Windows.Forms.Timer(Me.components)
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.btnRefresh = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuAutoRefresh = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAutoAcknowledge = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuRemoveFilters = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuRemoveSort = New System.Windows.Forms.ToolStripMenuItem
        Me.btnView = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuOptions = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuCauseColView = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuZoneColView = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuProdIDColView = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuJobIDColView = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuStyleColView = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuColorColView = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuValveColView = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuJobNameColView = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuProcessColView = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuNodeColView = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuDefault = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuUser = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuSaveUser = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuAlarmMaskUtility = New System.Windows.Forms.ToolStripMenuItem
        Me.btnPrint = New System.Windows.Forms.ToolStripSplitButton
        Me.mnuPrint = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintPreview = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPageSetup = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintFile = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuPrintOptions = New System.Windows.Forms.ToolStripMenuItem
        Me.dgvAlarms = New System.Windows.Forms.DataGridView
        Me.AlarmNumber = New System.Windows.Forms.DataGridViewLinkColumn
        Me.Device = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Description = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Severity = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.CauseMnemonic = New System.Windows.Forms.DataGridViewLinkColumn
        Me.StartSerial = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.EndSerial = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Zone = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.ProdID = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.JobID = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Style = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Color = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Valve = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.JobName = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Process = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Node = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Category = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DowntimeFlag = New System.Windows.Forms.DataGridViewCheckBoxColumn
        Me.Facility = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Status_Col = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.cboFilter = New System.Windows.Forms.ComboBox
        Me.tmrUpdateGrid = New System.Windows.Forms.Timer(Me.components)
        Me.lstFilter = New System.Windows.Forms.ListBox
        Me.tmrBlink = New System.Windows.Forms.Timer(Me.components)
        Me.lblUpdate = New System.Windows.Forms.Label
        Me.Button1 = New System.Windows.Forms.Button
        Me.Button2 = New System.Windows.Forms.Button
        Me.Button3 = New System.Windows.Forms.Button
        Me.lblGridTT = New System.Windows.Forms.Label
        Me.tmrToolTip = New System.Windows.Forms.Timer(Me.components)
        Me.lblNoAlarms = New System.Windows.Forms.Label
        Me.stsStatus = New System.Windows.Forms.StatusStrip
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel
        Me.tspProgress = New System.Windows.Forms.ToolStripProgressBar
        Me.lblSpacer = New System.Windows.Forms.ToolStripStatusLabel
        Me.btnFunction = New System.Windows.Forms.ToolStripDropDownButton
        Me.mnuLogin = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLogOut = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuRemote = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuLocal = New System.Windows.Forms.ToolStripMenuItem
        Me.TextBox1 = New System.Windows.Forms.TextBox
        Me.lblRobotAlarms = New System.Windows.Forms.Label
        Me.lblPLCAlarms = New System.Windows.Forms.Label
        Me.tlsMain.SuspendLayout()
        CType(Me.dgvAlarms, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.stsStatus.SuspendLayout()
        Me.SuspendLayout()
        '
        'tmrNewAlarm
        '
        '
        'tlsMain
        '
        resources.ApplyResources(Me.tlsMain, "tlsMain")
        Me.tlsMain.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.tlsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.btnRefresh, Me.btnView, Me.btnPrint})
        Me.tlsMain.MinimumSize = New System.Drawing.Size(0, 54)
        Me.tlsMain.Name = "tlsMain"
        Me.tlsMain.Stretch = True
        Me.tlsMain.TabStop = True
        '
        'btnClose
        '
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.Name = "btnClose"
        '
        'btnRefresh
        '
        resources.ApplyResources(Me.btnRefresh, "btnRefresh")
        Me.btnRefresh.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuAutoRefresh, Me.mnuAutoAcknowledge, Me.mnuRemoveFilters, Me.mnuRemoveSort})
        Me.btnRefresh.Name = "btnRefresh"
        '
        'mnuAutoRefresh
        '
        resources.ApplyResources(Me.mnuAutoRefresh, "mnuAutoRefresh")
        Me.mnuAutoRefresh.Checked = True
        Me.mnuAutoRefresh.CheckOnClick = True
        Me.mnuAutoRefresh.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuAutoRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.mnuAutoRefresh.Image = Global.AlarmMan.My.Resources.ProjectStrings.CheckBox
        Me.mnuAutoRefresh.Name = "mnuAutoRefresh"
        '
        'mnuAutoAcknowledge
        '
        Me.mnuAutoAcknowledge.Checked = True
        Me.mnuAutoAcknowledge.CheckOnClick = True
        Me.mnuAutoAcknowledge.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuAutoAcknowledge.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.mnuAutoAcknowledge.Image = Global.AlarmMan.My.Resources.ProjectStrings.CheckBox
        resources.ApplyResources(Me.mnuAutoAcknowledge, "mnuAutoAcknowledge")
        Me.mnuAutoAcknowledge.Name = "mnuAutoAcknowledge"
        '
        'mnuRemoveFilters
        '
        Me.mnuRemoveFilters.Name = "mnuRemoveFilters"
        resources.ApplyResources(Me.mnuRemoveFilters, "mnuRemoveFilters")
        '
        'mnuRemoveSort
        '
        Me.mnuRemoveSort.Name = "mnuRemoveSort"
        resources.ApplyResources(Me.mnuRemoveSort, "mnuRemoveSort")
        '
        'btnView
        '
        resources.ApplyResources(Me.btnView, "btnView")
        Me.btnView.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuOptions, Me.mnuDefault, Me.mnuUser, Me.mnuSaveUser, Me.mnuAlarmMaskUtility})
        Me.btnView.Name = "btnView"
        '
        'mnuOptions
        '
        Me.mnuOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.mnuOptions.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuCauseColView, Me.mnuZoneColView, Me.mnuProdIDColView, Me.mnuJobIDColView, Me.mnuStyleColView, Me.mnuColorColView, Me.mnuValveColView, Me.mnuJobNameColView, Me.mnuProcessColView, Me.mnuNodeColView})
        resources.ApplyResources(Me.mnuOptions, "mnuOptions")
        Me.mnuOptions.Name = "mnuOptions"
        '
        'mnuCauseColView
        '
        Me.mnuCauseColView.Checked = True
        Me.mnuCauseColView.CheckOnClick = True
        Me.mnuCauseColView.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuCauseColView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.mnuCauseColView.Image = Global.AlarmMan.My.Resources.ProjectStrings.CheckBox
        resources.ApplyResources(Me.mnuCauseColView, "mnuCauseColView")
        Me.mnuCauseColView.Name = "mnuCauseColView"
        '
        'mnuZoneColView
        '
        Me.mnuZoneColView.CheckOnClick = True
        Me.mnuZoneColView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.mnuZoneColView.Image = Global.AlarmMan.My.Resources.ProjectStrings.CheckBox
        resources.ApplyResources(Me.mnuZoneColView, "mnuZoneColView")
        Me.mnuZoneColView.Name = "mnuZoneColView"
        '
        'mnuProdIDColView
        '
        Me.mnuProdIDColView.CheckOnClick = True
        Me.mnuProdIDColView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.mnuProdIDColView.Image = Global.AlarmMan.My.Resources.ProjectStrings.CheckBox
        resources.ApplyResources(Me.mnuProdIDColView, "mnuProdIDColView")
        Me.mnuProdIDColView.Name = "mnuProdIDColView"
        '
        'mnuJobIDColView
        '
        Me.mnuJobIDColView.CheckOnClick = True
        Me.mnuJobIDColView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.mnuJobIDColView.Image = Global.AlarmMan.My.Resources.ProjectStrings.CheckBox
        resources.ApplyResources(Me.mnuJobIDColView, "mnuJobIDColView")
        Me.mnuJobIDColView.Name = "mnuJobIDColView"
        '
        'mnuStyleColView
        '
        Me.mnuStyleColView.CheckOnClick = True
        Me.mnuStyleColView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.mnuStyleColView.Image = Global.AlarmMan.My.Resources.ProjectStrings.CheckBox
        resources.ApplyResources(Me.mnuStyleColView, "mnuStyleColView")
        Me.mnuStyleColView.Name = "mnuStyleColView"
        '
        'mnuColorColView
        '
        Me.mnuColorColView.Checked = True
        Me.mnuColorColView.CheckOnClick = True
        Me.mnuColorColView.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuColorColView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.mnuColorColView.Image = Global.AlarmMan.My.Resources.ProjectStrings.CheckBox
        resources.ApplyResources(Me.mnuColorColView, "mnuColorColView")
        Me.mnuColorColView.Name = "mnuColorColView"
        '
        'mnuValveColView
        '
        Me.mnuValveColView.Checked = True
        Me.mnuValveColView.CheckOnClick = True
        Me.mnuValveColView.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuValveColView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.mnuValveColView.Image = Global.AlarmMan.My.Resources.ProjectStrings.CheckBox
        resources.ApplyResources(Me.mnuValveColView, "mnuValveColView")
        Me.mnuValveColView.Name = "mnuValveColView"
        '
        'mnuJobNameColView
        '
        Me.mnuJobNameColView.Checked = True
        Me.mnuJobNameColView.CheckOnClick = True
        Me.mnuJobNameColView.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuJobNameColView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.mnuJobNameColView.Image = Global.AlarmMan.My.Resources.ProjectStrings.CheckBox
        resources.ApplyResources(Me.mnuJobNameColView, "mnuJobNameColView")
        Me.mnuJobNameColView.Name = "mnuJobNameColView"
        '
        'mnuProcessColView
        '
        Me.mnuProcessColView.Checked = True
        Me.mnuProcessColView.CheckOnClick = True
        Me.mnuProcessColView.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuProcessColView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.mnuProcessColView.Image = Global.AlarmMan.My.Resources.ProjectStrings.CheckBox
        resources.ApplyResources(Me.mnuProcessColView, "mnuProcessColView")
        Me.mnuProcessColView.Name = "mnuProcessColView"
        '
        'mnuNodeColView
        '
        Me.mnuNodeColView.CheckOnClick = True
        Me.mnuNodeColView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.mnuNodeColView.Image = Global.AlarmMan.My.Resources.ProjectStrings.CheckBox
        resources.ApplyResources(Me.mnuNodeColView, "mnuNodeColView")
        Me.mnuNodeColView.Name = "mnuNodeColView"
        '
        'mnuDefault
        '
        Me.mnuDefault.Checked = True
        Me.mnuDefault.CheckOnClick = True
        Me.mnuDefault.CheckState = System.Windows.Forms.CheckState.Checked
        Me.mnuDefault.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.mnuDefault, "mnuDefault")
        Me.mnuDefault.Image = Global.AlarmMan.My.Resources.ProjectStrings.CheckBox
        Me.mnuDefault.Name = "mnuDefault"
        '
        'mnuUser
        '
        Me.mnuUser.CheckOnClick = True
        Me.mnuUser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.mnuUser.Image = Global.AlarmMan.My.Resources.ProjectStrings.CheckBox
        resources.ApplyResources(Me.mnuUser, "mnuUser")
        Me.mnuUser.Name = "mnuUser"
        '
        'mnuSaveUser
        '
        Me.mnuSaveUser.Name = "mnuSaveUser"
        resources.ApplyResources(Me.mnuSaveUser, "mnuSaveUser")
        '
        'mnuAlarmMaskUtility
        '
        Me.mnuAlarmMaskUtility.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.mnuAlarmMaskUtility.Name = "mnuAlarmMaskUtility"
        resources.ApplyResources(Me.mnuAlarmMaskUtility, "mnuAlarmMaskUtility")
        '
        'btnPrint
        '
        resources.ApplyResources(Me.btnPrint, "btnPrint")
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
        'dgvAlarms
        '
        Me.dgvAlarms.AllowUserToAddRows = False
        Me.dgvAlarms.AllowUserToDeleteRows = False
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Arial", 8.25!)
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgvAlarms.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.dgvAlarms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvAlarms.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.AlarmNumber, Me.Device, Me.Description, Me.Severity, Me.CauseMnemonic, Me.StartSerial, Me.EndSerial, Me.Zone, Me.ProdID, Me.JobID, Me.Style, Me.Color, Me.Valve, Me.JobName, Me.Process, Me.Node, Me.Category, Me.DowntimeFlag, Me.Facility, Me.Status_Col})
        Me.dgvAlarms.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        resources.ApplyResources(Me.dgvAlarms, "dgvAlarms")
        Me.dgvAlarms.MaximumSize = New System.Drawing.Size(1500, 3200)
        Me.dgvAlarms.MultiSelect = False
        Me.dgvAlarms.Name = "dgvAlarms"
        Me.dgvAlarms.RowHeadersVisible = False
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dgvAlarms.RowsDefaultCellStyle = DataGridViewCellStyle2
        Me.dgvAlarms.RowTemplate.Height = 24
        Me.dgvAlarms.ShowEditingIcon = False
        '
        'AlarmNumber
        '
        Me.AlarmNumber.DataPropertyName = "AlarmNumber"
        resources.ApplyResources(Me.AlarmNumber, "AlarmNumber")
        Me.AlarmNumber.Name = "AlarmNumber"
        Me.AlarmNumber.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.AlarmNumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
        '
        'Device
        '
        Me.Device.DataPropertyName = "Device"
        resources.ApplyResources(Me.Device, "Device")
        Me.Device.Name = "Device"
        Me.Device.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
        '
        'Description
        '
        Me.Description.DataPropertyName = "Description"
        resources.ApplyResources(Me.Description, "Description")
        Me.Description.Name = "Description"
        Me.Description.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
        '
        'Severity
        '
        Me.Severity.DataPropertyName = "Severity"
        resources.ApplyResources(Me.Severity, "Severity")
        Me.Severity.Name = "Severity"
        Me.Severity.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
        '
        'CauseMnemonic
        '
        Me.CauseMnemonic.DataPropertyName = "CauseMnemonic"
        resources.ApplyResources(Me.CauseMnemonic, "CauseMnemonic")
        Me.CauseMnemonic.Name = "CauseMnemonic"
        Me.CauseMnemonic.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
        '
        'StartSerial
        '
        Me.StartSerial.DataPropertyName = "StartSerial"
        resources.ApplyResources(Me.StartSerial, "StartSerial")
        Me.StartSerial.Name = "StartSerial"
        Me.StartSerial.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
        '
        'EndSerial
        '
        Me.EndSerial.DataPropertyName = "EndSerial"
        resources.ApplyResources(Me.EndSerial, "EndSerial")
        Me.EndSerial.Name = "EndSerial"
        Me.EndSerial.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
        '
        'Zone
        '
        Me.Zone.DataPropertyName = "Zone"
        resources.ApplyResources(Me.Zone, "Zone")
        Me.Zone.Name = "Zone"
        Me.Zone.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
        '
        'ProdID
        '
        Me.ProdID.DataPropertyName = "ProdID"
        resources.ApplyResources(Me.ProdID, "ProdID")
        Me.ProdID.Name = "ProdID"
        Me.ProdID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
        '
        'JobID
        '
        Me.JobID.DataPropertyName = "JobID"
        resources.ApplyResources(Me.JobID, "JobID")
        Me.JobID.Name = "JobID"
        Me.JobID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
        '
        'Style
        '
        Me.Style.DataPropertyName = "Style"
        resources.ApplyResources(Me.Style, "Style")
        Me.Style.Name = "Style"
        Me.Style.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
        '
        'Color
        '
        Me.Color.DataPropertyName = "Color"
        resources.ApplyResources(Me.Color, "Color")
        Me.Color.Name = "Color"
        Me.Color.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
        '
        'Valve
        '
        Me.Valve.DataPropertyName = "Valve"
        resources.ApplyResources(Me.Valve, "Valve")
        Me.Valve.Name = "Valve"
        Me.Valve.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
        '
        'JobName
        '
        Me.JobName.DataPropertyName = "JobName"
        resources.ApplyResources(Me.JobName, "JobName")
        Me.JobName.Name = "JobName"
        Me.JobName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
        '
        'Process
        '
        Me.Process.DataPropertyName = "Process"
        resources.ApplyResources(Me.Process, "Process")
        Me.Process.Name = "Process"
        Me.Process.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
        '
        'Node
        '
        Me.Node.DataPropertyName = "Node"
        resources.ApplyResources(Me.Node, "Node")
        Me.Node.Name = "Node"
        Me.Node.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
        '
        'Category
        '
        Me.Category.DataPropertyName = "Category"
        resources.ApplyResources(Me.Category, "Category")
        Me.Category.Name = "Category"
        Me.Category.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
        '
        'DowntimeFlag
        '
        Me.DowntimeFlag.DataPropertyName = "DowntimeFlag"
        resources.ApplyResources(Me.DowntimeFlag, "DowntimeFlag")
        Me.DowntimeFlag.Name = "DowntimeFlag"
        '
        'Facility
        '
        Me.Facility.DataPropertyName = "Facility"
        resources.ApplyResources(Me.Facility, "Facility")
        Me.Facility.Name = "Facility"
        Me.Facility.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
        '
        'Status_Col
        '
        Me.Status_Col.DataPropertyName = "Status"
        resources.ApplyResources(Me.Status_Col, "Status_Col")
        Me.Status_Col.Name = "Status_Col"
        Me.Status_Col.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
        '
        'cboFilter
        '
        Me.cboFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboFilter.FormattingEnabled = True
        resources.ApplyResources(Me.cboFilter, "cboFilter")
        Me.cboFilter.Name = "cboFilter"
        '
        'tmrUpdateGrid
        '
        Me.tmrUpdateGrid.Interval = 1000
        '
        'lstFilter
        '
        Me.lstFilter.FormattingEnabled = True
        resources.ApplyResources(Me.lstFilter, "lstFilter")
        Me.lstFilter.Name = "lstFilter"
        '
        'tmrBlink
        '
        Me.tmrBlink.Interval = 1000
        '
        'lblUpdate
        '
        resources.ApplyResources(Me.lblUpdate, "lblUpdate")
        Me.lblUpdate.BackColor = System.Drawing.Color.LemonChiffon
        Me.lblUpdate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblUpdate.Name = "lblUpdate"
        '
        'Button1
        '
        resources.ApplyResources(Me.Button1, "Button1")
        Me.Button1.Name = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        resources.ApplyResources(Me.Button2, "Button2")
        Me.Button2.Name = "Button2"
        Me.Button2.UseVisualStyleBackColor = False
        '
        'Button3
        '
        resources.ApplyResources(Me.Button3, "Button3")
        Me.Button3.Name = "Button3"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'lblGridTT
        '
        resources.ApplyResources(Me.lblGridTT, "lblGridTT")
        Me.lblGridTT.BackColor = System.Drawing.Color.LemonChiffon
        Me.lblGridTT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblGridTT.MaximumSize = New System.Drawing.Size(170, 64)
        Me.lblGridTT.Name = "lblGridTT"
        '
        'tmrToolTip
        '
        Me.tmrToolTip.Interval = 5000
        '
        'lblNoAlarms
        '
        Me.lblNoAlarms.BackColor = System.Drawing.Color.PaleGreen
        Me.lblNoAlarms.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.lblNoAlarms, "lblNoAlarms")
        Me.lblNoAlarms.Name = "lblNoAlarms"
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
        'TextBox1
        '
        resources.ApplyResources(Me.TextBox1, "TextBox1")
        Me.TextBox1.Name = "TextBox1"
        '
        'lblRobotAlarms
        '
        resources.ApplyResources(Me.lblRobotAlarms, "lblRobotAlarms")
        Me.lblRobotAlarms.Name = "lblRobotAlarms"
        '
        'lblPLCAlarms
        '
        resources.ApplyResources(Me.lblPLCAlarms, "lblPLCAlarms")
        Me.lblPLCAlarms.Name = "lblPLCAlarms"
        '
        'frmMain
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.lblPLCAlarms)
        Me.Controls.Add(Me.lblRobotAlarms)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.stsStatus)
        Me.Controls.Add(Me.lblNoAlarms)
        Me.Controls.Add(Me.lblGridTT)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.lblUpdate)
        Me.Controls.Add(Me.lstFilter)
        Me.Controls.Add(Me.cboFilter)
        Me.Controls.Add(Me.dgvAlarms)
        Me.Controls.Add(Me.tlsMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.KeyPreview = True
        Me.Name = "frmMain"
        Me.tlsMain.ResumeLayout(False)
        Me.tlsMain.PerformLayout()
        CType(Me.dgvAlarms, System.ComponentModel.ISupportInitialize).EndInit()
        Me.stsStatus.ResumeLayout(False)
        Me.stsStatus.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents tmrNewAlarm As System.Windows.Forms.Timer
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnClose As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnRefresh As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuAutoRefresh As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuRemoveFilters As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuRemoveSort As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnView As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuOptions As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuCauseColView As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuZoneColView As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuProdIDColView As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuJobIDColView As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuStyleColView As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuColorColView As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuValveColView As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuJobNameColView As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuProcessColView As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuNodeColView As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuDefault As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnPrint As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents mnuPrint As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents dgvAlarms As System.Windows.Forms.DataGridView
    Friend WithEvents cboFilter As System.Windows.Forms.ComboBox
    Friend WithEvents tmrUpdateGrid As System.Windows.Forms.Timer
    Friend WithEvents lstFilter As System.Windows.Forms.ListBox
    Friend WithEvents tmrBlink As System.Windows.Forms.Timer
    Friend WithEvents lblUpdate As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents AlarmNumber As System.Windows.Forms.DataGridViewLinkColumn
    Friend WithEvents Device As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Description As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Severity As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents CauseMnemonic As System.Windows.Forms.DataGridViewLinkColumn
    Friend WithEvents StartSerial As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents EndSerial As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Zone As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ProdID As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents JobID As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Style As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Color As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Valve As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents JobName As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Process As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Node As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Category As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DowntimeFlag As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents Facility As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Status_Col As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents mnuAlarmMaskUtility As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblGridTT As System.Windows.Forms.Label
    Friend WithEvents tmrToolTip As System.Windows.Forms.Timer
    Friend WithEvents mnuUser As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSaveUser As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblNoAlarms As System.Windows.Forms.Label
    Friend WithEvents mnuAutoAcknowledge As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents stsStatus As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tspProgress As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents lblSpacer As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents btnFunction As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents mnuLogin As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLogOut As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuRemote As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLocal As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents mnuPrintPreview As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPageSetup As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuPrintOptions As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblRobotAlarms As System.Windows.Forms.Label
    Friend WithEvents lblPLCAlarms As System.Windows.Forms.Label
End Class
