<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class uctlLifeCycles
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.lblViewRobotCap = New System.Windows.Forms.Label
        Me.cboRobot = New System.Windows.Forms.ComboBox
        Me.mnuReset = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuResetSingle = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.mnuResetAll = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuSeparator2 = New System.Windows.Forms.ToolStripSeparator
        Me.mnuSaveAll = New System.Windows.Forms.ToolStripMenuItem
        Me.gpbEstatCable = New System.Windows.Forms.GroupBox
        Me.ftbEstatTol1 = New FocusedTextBox.FocusedTextBox
        Me.ftbEstatFlt1 = New FocusedTextBox.FocusedTextBox
        Me.lblEstatTolCap = New System.Windows.Forms.Label
        Me.lblEstatFltCap = New System.Windows.Forms.Label
        Me.lblEstatActCap = New System.Windows.Forms.Label
        Me.lblEstatAct1 = New System.Windows.Forms.Label
        Me.picEstat1 = New System.Windows.Forms.PictureBox
        Me.gpbRegulator = New System.Windows.Forms.GroupBox
        Me.ftbRegTol1 = New FocusedTextBox.FocusedTextBox
        Me.ftbRegFlt1 = New FocusedTextBox.FocusedTextBox
        Me.lblRegTolCap = New System.Windows.Forms.Label
        Me.lblRegFltCap = New System.Windows.Forms.Label
        Me.lblRegActCap = New System.Windows.Forms.Label
        Me.lblRegAct1 = New System.Windows.Forms.Label
        Me.picReg1 = New System.Windows.Forms.PictureBox
        Me.gpbPump = New System.Windows.Forms.GroupBox
        Me.ftbPumpTol2 = New FocusedTextBox.FocusedTextBox
        Me.ftbPumpTol1 = New FocusedTextBox.FocusedTextBox
        Me.ftbPumpFlt2 = New FocusedTextBox.FocusedTextBox
        Me.ftbPumpFlt1 = New FocusedTextBox.FocusedTextBox
        Me.lblPumpTolCap = New System.Windows.Forms.Label
        Me.lblPumpFltCap = New System.Windows.Forms.Label
        Me.lblPumpActCap = New System.Windows.Forms.Label
        Me.lblPump2 = New System.Windows.Forms.Label
        Me.lblPump1 = New System.Windows.Forms.Label
        Me.lblPumpAct2 = New System.Windows.Forms.Label
        Me.picPump2 = New System.Windows.Forms.PictureBox
        Me.lblPumpAct1 = New System.Windows.Forms.Label
        Me.picPump1 = New System.Windows.Forms.PictureBox
        Me.lblTitle = New System.Windows.Forms.Label
        Me.ttHint = New System.Windows.Forms.ToolTip(Me.components)
        Me.pnlColorValves = New System.Windows.Forms.Panel
        Me.ftbColorValveTol1 = New FocusedTextBox.FocusedTextBox
        Me.ftbColorValveFlt1 = New FocusedTextBox.FocusedTextBox
        Me.lblColorValveAct1 = New System.Windows.Forms.Label
        Me.lblColorValve1 = New System.Windows.Forms.Label
        Me.picColorValve1 = New System.Windows.Forms.PictureBox
        Me.lblColorValveActCap = New System.Windows.Forms.Label
        Me.lblColorValveFltCap = New System.Windows.Forms.Label
        Me.lblColorValveTolCap = New System.Windows.Forms.Label
        Me.gpbColorValves = New System.Windows.Forms.GroupBox
        Me.pnlCCValves = New System.Windows.Forms.Panel
        Me.ftbCCValveTol1 = New FocusedTextBox.FocusedTextBox
        Me.ftbCCValveFlt1 = New FocusedTextBox.FocusedTextBox
        Me.lblCCValveAct1 = New System.Windows.Forms.Label
        Me.lblCCValve1 = New System.Windows.Forms.Label
        Me.picCCValve1 = New System.Windows.Forms.PictureBox
        Me.lblCCValveActCap = New System.Windows.Forms.Label
        Me.lblCCValveFltCap = New System.Windows.Forms.Label
        Me.lblCCValveTolCap = New System.Windows.Forms.Label
        Me.gpbCCValves = New System.Windows.Forms.GroupBox
        Me.mnuReset.SuspendLayout()
        Me.gpbEstatCable.SuspendLayout()
        CType(Me.picEstat1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gpbRegulator.SuspendLayout()
        CType(Me.picReg1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gpbPump.SuspendLayout()
        CType(Me.picPump2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picPump1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlColorValves.SuspendLayout()
        CType(Me.picColorValve1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gpbColorValves.SuspendLayout()
        Me.pnlCCValves.SuspendLayout()
        CType(Me.picCCValve1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gpbCCValves.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblViewRobotCap
        '
        Me.lblViewRobotCap.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblViewRobotCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblViewRobotCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblViewRobotCap.Location = New System.Drawing.Point(731, 29)
        Me.lblViewRobotCap.Name = "lblViewRobotCap"
        Me.lblViewRobotCap.Size = New System.Drawing.Size(93, 19)
        Me.lblViewRobotCap.TabIndex = 176
        Me.lblViewRobotCap.Text = "Robot"
        Me.lblViewRobotCap.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblViewRobotCap.UseMnemonic = False
        '
        'cboRobot
        '
        Me.cboRobot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboRobot.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboRobot.ItemHeight = 18
        Me.cboRobot.Location = New System.Drawing.Point(830, 25)
        Me.cboRobot.Name = "cboRobot"
        Me.cboRobot.Size = New System.Drawing.Size(160, 26)
        Me.cboRobot.TabIndex = 175
        '
        'mnuReset
        '
        Me.mnuReset.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuResetSingle, Me.mnuSeparator1, Me.mnuResetAll, Me.mnuSeparator2, Me.mnuSaveAll})
        Me.mnuReset.Name = "mnuReset"
        Me.mnuReset.Size = New System.Drawing.Size(151, 82)
        '
        'mnuResetSingle
        '
        Me.mnuResetSingle.Name = "mnuResetSingle"
        Me.mnuResetSingle.Size = New System.Drawing.Size(150, 22)
        Me.mnuResetSingle.Text = "mnuResetSingle"
        '
        'mnuSeparator1
        '
        Me.mnuSeparator1.Name = "mnuSeparator1"
        Me.mnuSeparator1.Size = New System.Drawing.Size(147, 6)
        '
        'mnuResetAll
        '
        Me.mnuResetAll.Name = "mnuResetAll"
        Me.mnuResetAll.Size = New System.Drawing.Size(150, 22)
        Me.mnuResetAll.Text = "mnuResetAll"
        '
        'mnuSeparator2
        '
        Me.mnuSeparator2.Name = "mnuSeparator2"
        Me.mnuSeparator2.Size = New System.Drawing.Size(147, 6)
        '
        'mnuSaveAll
        '
        Me.mnuSaveAll.Name = "mnuSaveAll"
        Me.mnuSaveAll.Size = New System.Drawing.Size(150, 22)
        Me.mnuSaveAll.Text = "mnuSaveAll"
        '
        'gpbEstatCable
        '
        Me.gpbEstatCable.Controls.Add(Me.ftbEstatTol1)
        Me.gpbEstatCable.Controls.Add(Me.ftbEstatFlt1)
        Me.gpbEstatCable.Controls.Add(Me.lblEstatTolCap)
        Me.gpbEstatCable.Controls.Add(Me.lblEstatFltCap)
        Me.gpbEstatCable.Controls.Add(Me.lblEstatActCap)
        Me.gpbEstatCable.Controls.Add(Me.lblEstatAct1)
        Me.gpbEstatCable.Controls.Add(Me.picEstat1)
        Me.gpbEstatCable.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gpbEstatCable.Location = New System.Drawing.Point(797, 79)
        Me.gpbEstatCable.Name = "gpbEstatCable"
        Me.gpbEstatCable.Size = New System.Drawing.Size(298, 116)
        Me.gpbEstatCable.TabIndex = 179
        Me.gpbEstatCable.TabStop = False
        Me.gpbEstatCable.Text = "gpbEstatCable"
        '
        'ftbEstatTol1
        '
        Me.ftbEstatTol1.ContextMenuStrip = Me.mnuReset
        Me.ftbEstatTol1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ftbEstatTol1.ForeColor = System.Drawing.Color.Red
        Me.ftbEstatTol1.Location = New System.Drawing.Point(201, 51)
        Me.ftbEstatTol1.MaxLength = 6
        Me.ftbEstatTol1.Name = "ftbEstatTol1"
        Me.ftbEstatTol1.NumericOnly = True
        Me.ftbEstatTol1.Size = New System.Drawing.Size(65, 20)
        Me.ftbEstatTol1.TabIndex = 30
        Me.ftbEstatTol1.TabStop = False
        Me.ftbEstatTol1.Text = "100000"
        Me.ftbEstatTol1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'ftbEstatFlt1
        '
        Me.ftbEstatFlt1.ContextMenuStrip = Me.mnuReset
        Me.ftbEstatFlt1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ftbEstatFlt1.ForeColor = System.Drawing.Color.Red
        Me.ftbEstatFlt1.Location = New System.Drawing.Point(130, 51)
        Me.ftbEstatFlt1.MaxLength = 10
        Me.ftbEstatFlt1.Name = "ftbEstatFlt1"
        Me.ftbEstatFlt1.NumericOnly = True
        Me.ftbEstatFlt1.Size = New System.Drawing.Size(69, 20)
        Me.ftbEstatFlt1.TabIndex = 29
        Me.ftbEstatFlt1.TabStop = False
        Me.ftbEstatFlt1.Text = "0000000000"
        Me.ftbEstatFlt1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'lblEstatTolCap
        '
        Me.lblEstatTolCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblEstatTolCap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEstatTolCap.Location = New System.Drawing.Point(199, 30)
        Me.lblEstatTolCap.Name = "lblEstatTolCap"
        Me.lblEstatTolCap.Size = New System.Drawing.Size(67, 16)
        Me.lblEstatTolCap.TabIndex = 28
        Me.lblEstatTolCap.Text = "Tolerance"
        Me.lblEstatTolCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblEstatFltCap
        '
        Me.lblEstatFltCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblEstatFltCap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEstatFltCap.Location = New System.Drawing.Point(130, 30)
        Me.lblEstatFltCap.Name = "lblEstatFltCap"
        Me.lblEstatFltCap.Size = New System.Drawing.Size(69, 16)
        Me.lblEstatFltCap.TabIndex = 22
        Me.lblEstatFltCap.Text = "Fault"
        Me.lblEstatFltCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblEstatActCap
        '
        Me.lblEstatActCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblEstatActCap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEstatActCap.Location = New System.Drawing.Point(59, 30)
        Me.lblEstatActCap.Name = "lblEstatActCap"
        Me.lblEstatActCap.Size = New System.Drawing.Size(69, 16)
        Me.lblEstatActCap.TabIndex = 21
        Me.lblEstatActCap.Text = "Actual (hours)"
        Me.lblEstatActCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblEstatAct1
        '
        Me.lblEstatAct1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblEstatAct1.ContextMenuStrip = Me.mnuReset
        Me.lblEstatAct1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEstatAct1.Location = New System.Drawing.Point(59, 51)
        Me.lblEstatAct1.Name = "lblEstatAct1"
        Me.lblEstatAct1.Size = New System.Drawing.Size(69, 20)
        Me.lblEstatAct1.TabIndex = 18
        Me.lblEstatAct1.Text = "0000000000"
        Me.lblEstatAct1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'picEstat1
        '
        Me.picEstat1.Image = Global.BSD.My.Resources.Resources.gray
        Me.picEstat1.Location = New System.Drawing.Point(272, 49)
        Me.picEstat1.Name = "picEstat1"
        Me.picEstat1.Size = New System.Drawing.Size(20, 20)
        Me.picEstat1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.picEstat1.TabIndex = 17
        Me.picEstat1.TabStop = False
        Me.picEstat1.Tag = "imlLed"
        '
        'gpbRegulator
        '
        Me.gpbRegulator.Controls.Add(Me.ftbRegTol1)
        Me.gpbRegulator.Controls.Add(Me.ftbRegFlt1)
        Me.gpbRegulator.Controls.Add(Me.lblRegTolCap)
        Me.gpbRegulator.Controls.Add(Me.lblRegFltCap)
        Me.gpbRegulator.Controls.Add(Me.lblRegActCap)
        Me.gpbRegulator.Controls.Add(Me.lblRegAct1)
        Me.gpbRegulator.Controls.Add(Me.picReg1)
        Me.gpbRegulator.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gpbRegulator.Location = New System.Drawing.Point(797, 354)
        Me.gpbRegulator.Name = "gpbRegulator"
        Me.gpbRegulator.Size = New System.Drawing.Size(298, 116)
        Me.gpbRegulator.TabIndex = 180
        Me.gpbRegulator.TabStop = False
        Me.gpbRegulator.Text = "gpbRegulator"
        Me.gpbRegulator.Visible = False
        '
        'ftbRegTol1
        '
        Me.ftbRegTol1.ContextMenuStrip = Me.mnuReset
        Me.ftbRegTol1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ftbRegTol1.ForeColor = System.Drawing.Color.Red
        Me.ftbRegTol1.Location = New System.Drawing.Point(201, 51)
        Me.ftbRegTol1.MaxLength = 6
        Me.ftbRegTol1.Name = "ftbRegTol1"
        Me.ftbRegTol1.NumericOnly = True
        Me.ftbRegTol1.Size = New System.Drawing.Size(65, 20)
        Me.ftbRegTol1.TabIndex = 29
        Me.ftbRegTol1.TabStop = False
        Me.ftbRegTol1.Text = "100000"
        Me.ftbRegTol1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'ftbRegFlt1
        '
        Me.ftbRegFlt1.ContextMenuStrip = Me.mnuReset
        Me.ftbRegFlt1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ftbRegFlt1.ForeColor = System.Drawing.Color.Red
        Me.ftbRegFlt1.Location = New System.Drawing.Point(130, 51)
        Me.ftbRegFlt1.MaxLength = 10
        Me.ftbRegFlt1.Name = "ftbRegFlt1"
        Me.ftbRegFlt1.NumericOnly = True
        Me.ftbRegFlt1.Size = New System.Drawing.Size(69, 20)
        Me.ftbRegFlt1.TabIndex = 28
        Me.ftbRegFlt1.TabStop = False
        Me.ftbRegFlt1.Text = "0000000000"
        Me.ftbRegFlt1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'lblRegTolCap
        '
        Me.lblRegTolCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblRegTolCap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblRegTolCap.Location = New System.Drawing.Point(199, 32)
        Me.lblRegTolCap.Name = "lblRegTolCap"
        Me.lblRegTolCap.Size = New System.Drawing.Size(67, 16)
        Me.lblRegTolCap.TabIndex = 27
        Me.lblRegTolCap.Text = "Tolerance"
        Me.lblRegTolCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblRegFltCap
        '
        Me.lblRegFltCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblRegFltCap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblRegFltCap.Location = New System.Drawing.Point(130, 32)
        Me.lblRegFltCap.Name = "lblRegFltCap"
        Me.lblRegFltCap.Size = New System.Drawing.Size(69, 16)
        Me.lblRegFltCap.TabIndex = 26
        Me.lblRegFltCap.Text = "Fault"
        Me.lblRegFltCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblRegActCap
        '
        Me.lblRegActCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblRegActCap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblRegActCap.Location = New System.Drawing.Point(59, 32)
        Me.lblRegActCap.Name = "lblRegActCap"
        Me.lblRegActCap.Size = New System.Drawing.Size(69, 16)
        Me.lblRegActCap.TabIndex = 25
        Me.lblRegActCap.Text = "Actual (cycles)"
        Me.lblRegActCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblRegAct1
        '
        Me.lblRegAct1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblRegAct1.ContextMenuStrip = Me.mnuReset
        Me.lblRegAct1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblRegAct1.Location = New System.Drawing.Point(59, 51)
        Me.lblRegAct1.Name = "lblRegAct1"
        Me.lblRegAct1.Size = New System.Drawing.Size(69, 20)
        Me.lblRegAct1.TabIndex = 22
        Me.lblRegAct1.Text = "0000000000"
        Me.lblRegAct1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'picReg1
        '
        Me.picReg1.Image = Global.BSD.My.Resources.Resources.gray
        Me.picReg1.Location = New System.Drawing.Point(272, 51)
        Me.picReg1.Name = "picReg1"
        Me.picReg1.Size = New System.Drawing.Size(20, 20)
        Me.picReg1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.picReg1.TabIndex = 21
        Me.picReg1.TabStop = False
        Me.picReg1.Tag = "imlLed"
        '
        'gpbPump
        '
        Me.gpbPump.Controls.Add(Me.ftbPumpTol2)
        Me.gpbPump.Controls.Add(Me.ftbPumpTol1)
        Me.gpbPump.Controls.Add(Me.ftbPumpFlt2)
        Me.gpbPump.Controls.Add(Me.ftbPumpFlt1)
        Me.gpbPump.Controls.Add(Me.lblPumpTolCap)
        Me.gpbPump.Controls.Add(Me.lblPumpFltCap)
        Me.gpbPump.Controls.Add(Me.lblPumpActCap)
        Me.gpbPump.Controls.Add(Me.lblPump2)
        Me.gpbPump.Controls.Add(Me.lblPump1)
        Me.gpbPump.Controls.Add(Me.lblPumpAct2)
        Me.gpbPump.Controls.Add(Me.picPump2)
        Me.gpbPump.Controls.Add(Me.lblPumpAct1)
        Me.gpbPump.Controls.Add(Me.picPump1)
        Me.gpbPump.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gpbPump.Location = New System.Drawing.Point(797, 201)
        Me.gpbPump.Name = "gpbPump"
        Me.gpbPump.Size = New System.Drawing.Size(298, 147)
        Me.gpbPump.TabIndex = 181
        Me.gpbPump.TabStop = False
        Me.gpbPump.Text = "gpbPump"
        '
        'ftbPumpTol2
        '
        Me.ftbPumpTol2.ContextMenuStrip = Me.mnuReset
        Me.ftbPumpTol2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ftbPumpTol2.ForeColor = System.Drawing.Color.Red
        Me.ftbPumpTol2.Location = New System.Drawing.Point(201, 83)
        Me.ftbPumpTol2.MaxLength = 6
        Me.ftbPumpTol2.Name = "ftbPumpTol2"
        Me.ftbPumpTol2.NumericOnly = True
        Me.ftbPumpTol2.Size = New System.Drawing.Size(65, 20)
        Me.ftbPumpTol2.TabIndex = 37
        Me.ftbPumpTol2.TabStop = False
        Me.ftbPumpTol2.Text = "100000"
        Me.ftbPumpTol2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'ftbPumpTol1
        '
        Me.ftbPumpTol1.ContextMenuStrip = Me.mnuReset
        Me.ftbPumpTol1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ftbPumpTol1.ForeColor = System.Drawing.Color.Red
        Me.ftbPumpTol1.Location = New System.Drawing.Point(201, 51)
        Me.ftbPumpTol1.MaxLength = 6
        Me.ftbPumpTol1.Name = "ftbPumpTol1"
        Me.ftbPumpTol1.NumericOnly = True
        Me.ftbPumpTol1.Size = New System.Drawing.Size(65, 20)
        Me.ftbPumpTol1.TabIndex = 36
        Me.ftbPumpTol1.TabStop = False
        Me.ftbPumpTol1.Text = "100000"
        Me.ftbPumpTol1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'ftbPumpFlt2
        '
        Me.ftbPumpFlt2.ContextMenuStrip = Me.mnuReset
        Me.ftbPumpFlt2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ftbPumpFlt2.ForeColor = System.Drawing.Color.Red
        Me.ftbPumpFlt2.Location = New System.Drawing.Point(130, 83)
        Me.ftbPumpFlt2.MaxLength = 10
        Me.ftbPumpFlt2.Name = "ftbPumpFlt2"
        Me.ftbPumpFlt2.NumericOnly = True
        Me.ftbPumpFlt2.Size = New System.Drawing.Size(69, 20)
        Me.ftbPumpFlt2.TabIndex = 35
        Me.ftbPumpFlt2.TabStop = False
        Me.ftbPumpFlt2.Text = "0000000000"
        Me.ftbPumpFlt2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'ftbPumpFlt1
        '
        Me.ftbPumpFlt1.ContextMenuStrip = Me.mnuReset
        Me.ftbPumpFlt1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ftbPumpFlt1.ForeColor = System.Drawing.Color.Red
        Me.ftbPumpFlt1.Location = New System.Drawing.Point(130, 51)
        Me.ftbPumpFlt1.MaxLength = 10
        Me.ftbPumpFlt1.Name = "ftbPumpFlt1"
        Me.ftbPumpFlt1.NumericOnly = True
        Me.ftbPumpFlt1.Size = New System.Drawing.Size(69, 20)
        Me.ftbPumpFlt1.TabIndex = 34
        Me.ftbPumpFlt1.TabStop = False
        Me.ftbPumpFlt1.Text = "0000000000"
        Me.ftbPumpFlt1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'lblPumpTolCap
        '
        Me.lblPumpTolCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblPumpTolCap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPumpTolCap.Location = New System.Drawing.Point(199, 32)
        Me.lblPumpTolCap.Name = "lblPumpTolCap"
        Me.lblPumpTolCap.Size = New System.Drawing.Size(67, 16)
        Me.lblPumpTolCap.TabIndex = 33
        Me.lblPumpTolCap.Text = "Tolerance"
        Me.lblPumpTolCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblPumpFltCap
        '
        Me.lblPumpFltCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblPumpFltCap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPumpFltCap.Location = New System.Drawing.Point(130, 32)
        Me.lblPumpFltCap.Name = "lblPumpFltCap"
        Me.lblPumpFltCap.Size = New System.Drawing.Size(69, 16)
        Me.lblPumpFltCap.TabIndex = 32
        Me.lblPumpFltCap.Text = "Fault"
        Me.lblPumpFltCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblPumpActCap
        '
        Me.lblPumpActCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblPumpActCap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPumpActCap.Location = New System.Drawing.Point(59, 32)
        Me.lblPumpActCap.Name = "lblPumpActCap"
        Me.lblPumpActCap.Size = New System.Drawing.Size(69, 16)
        Me.lblPumpActCap.TabIndex = 31
        Me.lblPumpActCap.Text = "Actual (liters)"
        Me.lblPumpActCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblPump2
        '
        Me.lblPump2.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblPump2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPump2.Location = New System.Drawing.Point(3, 87)
        Me.lblPump2.Name = "lblPump2"
        Me.lblPump2.Size = New System.Drawing.Size(53, 13)
        Me.lblPump2.TabIndex = 30
        Me.lblPump2.Text = "lblPump2"
        Me.lblPump2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblPump1
        '
        Me.lblPump1.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblPump1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPump1.Location = New System.Drawing.Point(3, 55)
        Me.lblPump1.Name = "lblPump1"
        Me.lblPump1.Size = New System.Drawing.Size(53, 13)
        Me.lblPump1.TabIndex = 29
        Me.lblPump1.Text = "lblPump1"
        Me.lblPump1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblPumpAct2
        '
        Me.lblPumpAct2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblPumpAct2.ContextMenuStrip = Me.mnuReset
        Me.lblPumpAct2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPumpAct2.Location = New System.Drawing.Point(59, 83)
        Me.lblPumpAct2.Name = "lblPumpAct2"
        Me.lblPumpAct2.Size = New System.Drawing.Size(69, 20)
        Me.lblPumpAct2.TabIndex = 26
        Me.lblPumpAct2.Text = "0000000000"
        Me.lblPumpAct2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'picPump2
        '
        Me.picPump2.Image = Global.BSD.My.Resources.Resources.gray
        Me.picPump2.Location = New System.Drawing.Point(272, 83)
        Me.picPump2.Name = "picPump2"
        Me.picPump2.Size = New System.Drawing.Size(20, 20)
        Me.picPump2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.picPump2.TabIndex = 25
        Me.picPump2.TabStop = False
        Me.picPump2.Tag = "imlLed"
        '
        'lblPumpAct1
        '
        Me.lblPumpAct1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblPumpAct1.ContextMenuStrip = Me.mnuReset
        Me.lblPumpAct1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPumpAct1.Location = New System.Drawing.Point(59, 51)
        Me.lblPumpAct1.Name = "lblPumpAct1"
        Me.lblPumpAct1.Size = New System.Drawing.Size(69, 20)
        Me.lblPumpAct1.TabIndex = 22
        Me.lblPumpAct1.Text = "0000000000"
        Me.lblPumpAct1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'picPump1
        '
        Me.picPump1.Image = Global.BSD.My.Resources.Resources.gray
        Me.picPump1.Location = New System.Drawing.Point(272, 51)
        Me.picPump1.Name = "picPump1"
        Me.picPump1.Size = New System.Drawing.Size(20, 20)
        Me.picPump1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.picPump1.TabIndex = 21
        Me.picPump1.TabStop = False
        Me.picPump1.Tag = "imlLed"
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Arial", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(447, 19)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(160, 32)
        Me.lblTitle.TabIndex = 182
        Me.lblTitle.Text = "Life Cycles"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnlColorValves
        '
        Me.pnlColorValves.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.pnlColorValves.AutoScroll = True
        Me.pnlColorValves.Controls.Add(Me.ftbColorValveTol1)
        Me.pnlColorValves.Controls.Add(Me.ftbColorValveFlt1)
        Me.pnlColorValves.Controls.Add(Me.lblColorValveAct1)
        Me.pnlColorValves.Controls.Add(Me.lblColorValve1)
        Me.pnlColorValves.Controls.Add(Me.picColorValve1)
        Me.pnlColorValves.Location = New System.Drawing.Point(6, 49)
        Me.pnlColorValves.Name = "pnlColorValves"
        Me.pnlColorValves.Size = New System.Drawing.Size(374, 475)
        Me.pnlColorValves.TabIndex = 1
        '
        'ftbColorValveTol1
        '
        Me.ftbColorValveTol1.ContextMenuStrip = Me.mnuReset
        Me.ftbColorValveTol1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ftbColorValveTol1.ForeColor = System.Drawing.Color.Red
        Me.ftbColorValveTol1.Location = New System.Drawing.Point(246, 0)
        Me.ftbColorValveTol1.MaxLength = 6
        Me.ftbColorValveTol1.Name = "ftbColorValveTol1"
        Me.ftbColorValveTol1.NumericOnly = True
        Me.ftbColorValveTol1.Size = New System.Drawing.Size(56, 20)
        Me.ftbColorValveTol1.TabIndex = 18
        Me.ftbColorValveTol1.TabStop = False
        Me.ftbColorValveTol1.Text = "100000"
        Me.ftbColorValveTol1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'ftbColorValveFlt1
        '
        Me.ftbColorValveFlt1.ContextMenuStrip = Me.mnuReset
        Me.ftbColorValveFlt1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ftbColorValveFlt1.ForeColor = System.Drawing.Color.Red
        Me.ftbColorValveFlt1.Location = New System.Drawing.Point(171, 0)
        Me.ftbColorValveFlt1.MaxLength = 10
        Me.ftbColorValveFlt1.Name = "ftbColorValveFlt1"
        Me.ftbColorValveFlt1.NumericOnly = True
        Me.ftbColorValveFlt1.Size = New System.Drawing.Size(69, 20)
        Me.ftbColorValveFlt1.TabIndex = 17
        Me.ftbColorValveFlt1.TabStop = False
        Me.ftbColorValveFlt1.Text = "0000000000"
        Me.ftbColorValveFlt1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'lblColorValveAct1
        '
        Me.lblColorValveAct1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblColorValveAct1.ContextMenuStrip = Me.mnuReset
        Me.lblColorValveAct1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblColorValveAct1.Location = New System.Drawing.Point(96, 0)
        Me.lblColorValveAct1.Name = "lblColorValveAct1"
        Me.lblColorValveAct1.Size = New System.Drawing.Size(69, 20)
        Me.lblColorValveAct1.TabIndex = 14
        Me.lblColorValveAct1.Tag = "0"
        Me.lblColorValveAct1.Text = "0000000000"
        Me.lblColorValveAct1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblColorValve1
        '
        Me.lblColorValve1.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblColorValve1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblColorValve1.Location = New System.Drawing.Point(3, 4)
        Me.lblColorValve1.Name = "lblColorValve1"
        Me.lblColorValve1.Size = New System.Drawing.Size(87, 13)
        Me.lblColorValve1.TabIndex = 13
        Me.lblColorValve1.Tag = "0"
        Me.lblColorValve1.Text = "lblColorValve1"
        Me.lblColorValve1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'picColorValve1
        '
        Me.picColorValve1.Image = Global.BSD.My.Resources.Resources.gray
        Me.picColorValve1.Location = New System.Drawing.Point(308, 0)
        Me.picColorValve1.Name = "picColorValve1"
        Me.picColorValve1.Size = New System.Drawing.Size(20, 20)
        Me.picColorValve1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.picColorValve1.TabIndex = 12
        Me.picColorValve1.TabStop = False
        Me.picColorValve1.Tag = "0"
        '
        'lblColorValveActCap
        '
        Me.lblColorValveActCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblColorValveActCap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblColorValveActCap.Location = New System.Drawing.Point(102, 30)
        Me.lblColorValveActCap.Name = "lblColorValveActCap"
        Me.lblColorValveActCap.Size = New System.Drawing.Size(69, 16)
        Me.lblColorValveActCap.TabIndex = 10
        Me.lblColorValveActCap.Text = "Actual (cycles)"
        Me.lblColorValveActCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblColorValveFltCap
        '
        Me.lblColorValveFltCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblColorValveFltCap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblColorValveFltCap.Location = New System.Drawing.Point(177, 30)
        Me.lblColorValveFltCap.Name = "lblColorValveFltCap"
        Me.lblColorValveFltCap.Size = New System.Drawing.Size(65, 16)
        Me.lblColorValveFltCap.TabIndex = 11
        Me.lblColorValveFltCap.Text = "Fault"
        Me.lblColorValveFltCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblColorValveTolCap
        '
        Me.lblColorValveTolCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblColorValveTolCap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblColorValveTolCap.Location = New System.Drawing.Point(252, 30)
        Me.lblColorValveTolCap.Name = "lblColorValveTolCap"
        Me.lblColorValveTolCap.Size = New System.Drawing.Size(56, 16)
        Me.lblColorValveTolCap.TabIndex = 28
        Me.lblColorValveTolCap.Text = "Tolerance"
        Me.lblColorValveTolCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'gpbColorValves
        '
        Me.gpbColorValves.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.gpbColorValves.Controls.Add(Me.lblColorValveTolCap)
        Me.gpbColorValves.Controls.Add(Me.lblColorValveFltCap)
        Me.gpbColorValves.Controls.Add(Me.lblColorValveActCap)
        Me.gpbColorValves.Controls.Add(Me.pnlColorValves)
        Me.gpbColorValves.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gpbColorValves.Location = New System.Drawing.Point(405, 79)
        Me.gpbColorValves.Name = "gpbColorValves"
        Me.gpbColorValves.Size = New System.Drawing.Size(386, 530)
        Me.gpbColorValves.TabIndex = 178
        Me.gpbColorValves.TabStop = False
        Me.gpbColorValves.Text = "gpbColorValves"
        '
        'pnlCCValves
        '
        Me.pnlCCValves.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.pnlCCValves.AutoScroll = True
        Me.pnlCCValves.Controls.Add(Me.ftbCCValveTol1)
        Me.pnlCCValves.Controls.Add(Me.ftbCCValveFlt1)
        Me.pnlCCValves.Controls.Add(Me.lblCCValveAct1)
        Me.pnlCCValves.Controls.Add(Me.lblCCValve1)
        Me.pnlCCValves.Controls.Add(Me.picCCValve1)
        Me.pnlCCValves.Location = New System.Drawing.Point(6, 49)
        Me.pnlCCValves.Name = "pnlCCValves"
        Me.pnlCCValves.Size = New System.Drawing.Size(374, 475)
        Me.pnlCCValves.TabIndex = 0
        '
        'ftbCCValveTol1
        '
        Me.ftbCCValveTol1.ContextMenuStrip = Me.mnuReset
        Me.ftbCCValveTol1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ftbCCValveTol1.ForeColor = System.Drawing.Color.Red
        Me.ftbCCValveTol1.Location = New System.Drawing.Point(246, 0)
        Me.ftbCCValveTol1.MaxLength = 6
        Me.ftbCCValveTol1.Name = "ftbCCValveTol1"
        Me.ftbCCValveTol1.NumericOnly = True
        Me.ftbCCValveTol1.Size = New System.Drawing.Size(56, 20)
        Me.ftbCCValveTol1.TabIndex = 13
        Me.ftbCCValveTol1.TabStop = False
        Me.ftbCCValveTol1.Text = "100000"
        Me.ftbCCValveTol1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'ftbCCValveFlt1
        '
        Me.ftbCCValveFlt1.ContextMenuStrip = Me.mnuReset
        Me.ftbCCValveFlt1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ftbCCValveFlt1.ForeColor = System.Drawing.Color.Red
        Me.ftbCCValveFlt1.Location = New System.Drawing.Point(171, 0)
        Me.ftbCCValveFlt1.MaxLength = 10
        Me.ftbCCValveFlt1.Name = "ftbCCValveFlt1"
        Me.ftbCCValveFlt1.NumericOnly = True
        Me.ftbCCValveFlt1.Size = New System.Drawing.Size(69, 20)
        Me.ftbCCValveFlt1.TabIndex = 12
        Me.ftbCCValveFlt1.TabStop = False
        Me.ftbCCValveFlt1.Text = "0000000000"
        Me.ftbCCValveFlt1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'lblCCValveAct1
        '
        Me.lblCCValveAct1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblCCValveAct1.ContextMenuStrip = Me.mnuReset
        Me.lblCCValveAct1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCCValveAct1.Location = New System.Drawing.Point(96, 0)
        Me.lblCCValveAct1.Name = "lblCCValveAct1"
        Me.lblCCValveAct1.Size = New System.Drawing.Size(69, 20)
        Me.lblCCValveAct1.TabIndex = 9
        Me.lblCCValveAct1.Tag = "0"
        Me.lblCCValveAct1.Text = "0000000000"
        Me.lblCCValveAct1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblCCValve1
        '
        Me.lblCCValve1.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblCCValve1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCCValve1.Location = New System.Drawing.Point(3, 4)
        Me.lblCCValve1.Name = "lblCCValve1"
        Me.lblCCValve1.Size = New System.Drawing.Size(87, 13)
        Me.lblCCValve1.TabIndex = 8
        Me.lblCCValve1.Tag = "0"
        Me.lblCCValve1.Text = "lblCCValve1"
        Me.lblCCValve1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'picCCValve1
        '
        Me.picCCValve1.Image = Global.BSD.My.Resources.Resources.gray
        Me.picCCValve1.Location = New System.Drawing.Point(308, 0)
        Me.picCCValve1.Name = "picCCValve1"
        Me.picCCValve1.Size = New System.Drawing.Size(20, 20)
        Me.picCCValve1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.picCCValve1.TabIndex = 7
        Me.picCCValve1.TabStop = False
        Me.picCCValve1.Tag = "0"
        '
        'lblCCValveActCap
        '
        Me.lblCCValveActCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblCCValveActCap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCCValveActCap.Location = New System.Drawing.Point(102, 30)
        Me.lblCCValveActCap.Name = "lblCCValveActCap"
        Me.lblCCValveActCap.Size = New System.Drawing.Size(69, 16)
        Me.lblCCValveActCap.TabIndex = 9
        Me.lblCCValveActCap.Text = "Actual (cycles)"
        Me.lblCCValveActCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblCCValveFltCap
        '
        Me.lblCCValveFltCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblCCValveFltCap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCCValveFltCap.Location = New System.Drawing.Point(177, 30)
        Me.lblCCValveFltCap.Name = "lblCCValveFltCap"
        Me.lblCCValveFltCap.Size = New System.Drawing.Size(69, 16)
        Me.lblCCValveFltCap.TabIndex = 10
        Me.lblCCValveFltCap.Text = "Fault"
        Me.lblCCValveFltCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblCCValveTolCap
        '
        Me.lblCCValveTolCap.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblCCValveTolCap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCCValveTolCap.Location = New System.Drawing.Point(252, 30)
        Me.lblCCValveTolCap.Name = "lblCCValveTolCap"
        Me.lblCCValveTolCap.Size = New System.Drawing.Size(56, 16)
        Me.lblCCValveTolCap.TabIndex = 28
        Me.lblCCValveTolCap.Text = "Tolerance"
        Me.lblCCValveTolCap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'gpbCCValves
        '
        Me.gpbCCValves.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.gpbCCValves.Controls.Add(Me.lblCCValveTolCap)
        Me.gpbCCValves.Controls.Add(Me.lblCCValveFltCap)
        Me.gpbCCValves.Controls.Add(Me.lblCCValveActCap)
        Me.gpbCCValves.Controls.Add(Me.pnlCCValves)
        Me.gpbCCValves.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gpbCCValves.Location = New System.Drawing.Point(13, 79)
        Me.gpbCCValves.Name = "gpbCCValves"
        Me.gpbCCValves.Size = New System.Drawing.Size(386, 530)
        Me.gpbCCValves.TabIndex = 177
        Me.gpbCCValves.TabStop = False
        Me.gpbCCValves.Text = "gpbCCValves"
        '
        'uctlLifeCycles
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.gpbCCValves)
        Me.Controls.Add(Me.gpbColorValves)
        Me.Controls.Add(Me.gpbPump)
        Me.Controls.Add(Me.gpbRegulator)
        Me.Controls.Add(Me.gpbEstatCable)
        Me.Controls.Add(Me.lblViewRobotCap)
        Me.Controls.Add(Me.cboRobot)
        Me.Name = "uctlLifeCycles"
        Me.Size = New System.Drawing.Size(1236, 622)
        Me.mnuReset.ResumeLayout(False)
        Me.gpbEstatCable.ResumeLayout(False)
        Me.gpbEstatCable.PerformLayout()
        CType(Me.picEstat1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gpbRegulator.ResumeLayout(False)
        Me.gpbRegulator.PerformLayout()
        CType(Me.picReg1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gpbPump.ResumeLayout(False)
        Me.gpbPump.PerformLayout()
        CType(Me.picPump2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picPump1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlColorValves.ResumeLayout(False)
        Me.pnlColorValves.PerformLayout()
        CType(Me.picColorValve1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gpbColorValves.ResumeLayout(False)
        Me.pnlCCValves.ResumeLayout(False)
        Me.pnlCCValves.PerformLayout()
        CType(Me.picCCValve1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gpbCCValves.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblViewRobotCap As System.Windows.Forms.Label
    Friend WithEvents cboRobot As System.Windows.Forms.ComboBox
    Friend WithEvents gpbEstatCable As System.Windows.Forms.GroupBox
    Friend WithEvents gpbRegulator As System.Windows.Forms.GroupBox
    Friend WithEvents gpbPump As System.Windows.Forms.GroupBox
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblEstatAct1 As System.Windows.Forms.Label
    Friend WithEvents picEstat1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblRegAct1 As System.Windows.Forms.Label
    Friend WithEvents picReg1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblPumpAct2 As System.Windows.Forms.Label
    Friend WithEvents picPump2 As System.Windows.Forms.PictureBox
    Friend WithEvents lblPumpAct1 As System.Windows.Forms.Label
    Friend WithEvents picPump1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblPump1 As System.Windows.Forms.Label
    Friend WithEvents lblPump2 As System.Windows.Forms.Label
    Friend WithEvents lblRegActCap As System.Windows.Forms.Label
    Friend WithEvents lblPumpActCap As System.Windows.Forms.Label
    Friend WithEvents lblEstatActCap As System.Windows.Forms.Label
    Friend WithEvents lblEstatFltCap As System.Windows.Forms.Label
    Friend WithEvents lblRegFltCap As System.Windows.Forms.Label
    Friend WithEvents lblPumpFltCap As System.Windows.Forms.Label
    Friend WithEvents lblEstatTolCap As System.Windows.Forms.Label
    Friend WithEvents lblRegTolCap As System.Windows.Forms.Label
    Friend WithEvents lblPumpTolCap As System.Windows.Forms.Label
    Friend WithEvents ttHint As System.Windows.Forms.ToolTip
    Friend WithEvents ftbEstatTol1 As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbEstatFlt1 As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbRegTol1 As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbRegFlt1 As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbPumpTol2 As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbPumpTol1 As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbPumpFlt2 As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbPumpFlt1 As FocusedTextBox.FocusedTextBox
    Friend WithEvents mnuReset As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuResetSingle As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuResetAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuSaveAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents pnlColorValves As System.Windows.Forms.Panel
    Friend WithEvents ftbColorValveTol1 As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbColorValveFlt1 As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblColorValveAct1 As System.Windows.Forms.Label
    Friend WithEvents lblColorValve1 As System.Windows.Forms.Label
    Friend WithEvents picColorValve1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblColorValveActCap As System.Windows.Forms.Label
    Friend WithEvents lblColorValveFltCap As System.Windows.Forms.Label
    Friend WithEvents lblColorValveTolCap As System.Windows.Forms.Label
    Friend WithEvents gpbColorValves As System.Windows.Forms.GroupBox
    Friend WithEvents pnlCCValves As System.Windows.Forms.Panel
    Friend WithEvents ftbCCValveTol1 As FocusedTextBox.FocusedTextBox
    Friend WithEvents ftbCCValveFlt1 As FocusedTextBox.FocusedTextBox
    Friend WithEvents lblCCValveAct1 As System.Windows.Forms.Label
    Friend WithEvents lblCCValve1 As System.Windows.Forms.Label
    Friend WithEvents picCCValve1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblCCValveActCap As System.Windows.Forms.Label
    Friend WithEvents lblCCValveFltCap As System.Windows.Forms.Label
    Friend WithEvents lblCCValveTolCap As System.Windows.Forms.Label
    Friend WithEvents gpbCCValves As System.Windows.Forms.GroupBox

End Class
