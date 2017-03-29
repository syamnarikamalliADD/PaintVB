<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PurgePanel
    Inherits System.Windows.Forms.UserControl

    'UserControl1 overrides dispose to clean up the component list.
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
        Me.lblEq1Pressure = New System.Windows.Forms.Label
        Me.lblDeviceName = New System.Windows.Forms.Label
        Me.lblEq1Flow = New System.Windows.Forms.Label
        Me.lblEq2Pressure = New System.Windows.Forms.Label
        Me.lblEq2Flow = New System.Windows.Forms.Label
        Me.lblPurgeEnabled = New System.Windows.Forms.Label
        Me.lblPurgeFault = New System.Windows.Forms.Label
        Me.lblPurgeStart = New System.Windows.Forms.Label
        Me.lblRobotPower = New System.Windows.Forms.Label
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar
        Me.lblPurgeProgress = New System.Windows.Forms.Label
        Me.picRobotPowerPL = New System.Windows.Forms.PictureBox
        Me.picPurgeStartPB = New System.Windows.Forms.PictureBox
        Me.picPurgeFaultPL = New System.Windows.Forms.PictureBox
        Me.picPurgeEnabledPL = New System.Windows.Forms.PictureBox
        Me.picEq2FlowPL = New System.Windows.Forms.PictureBox
        Me.picEq2PressurePL = New System.Windows.Forms.PictureBox
        Me.picEq1FlowPL = New System.Windows.Forms.PictureBox
        Me.picEq1PressurePL = New System.Windows.Forms.PictureBox
        Me.lblEq4Flow = New System.Windows.Forms.Label
        Me.lblEq4Pressure = New System.Windows.Forms.Label
        Me.lblEq3Flow = New System.Windows.Forms.Label
        Me.lblEq3Pressure = New System.Windows.Forms.Label
        Me.picEq4FlowPL = New System.Windows.Forms.PictureBox
        Me.picEq4PressurePL = New System.Windows.Forms.PictureBox
        Me.picEq3FlowPL = New System.Windows.Forms.PictureBox
        Me.picEq3PressurePL = New System.Windows.Forms.PictureBox
        CType(Me.picRobotPowerPL, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picPurgeStartPB, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picPurgeFaultPL, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picPurgeEnabledPL, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picEq2FlowPL, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picEq2PressurePL, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picEq1FlowPL, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picEq1PressurePL, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picEq4FlowPL, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picEq4PressurePL, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picEq3FlowPL, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picEq3PressurePL, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblEq1Pressure
        '
        Me.lblEq1Pressure.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblEq1Pressure.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblEq1Pressure.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEq1Pressure.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.lblEq1Pressure.Location = New System.Drawing.Point(20, 26)
        Me.lblEq1Pressure.Name = "lblEq1Pressure"
        Me.lblEq1Pressure.Size = New System.Drawing.Size(70, 32)
        Me.lblEq1Pressure.TabIndex = 4
        Me.lblEq1Pressure.Text = "EQ1 AIR PRESSURE"
        Me.lblEq1Pressure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblDeviceName
        '
        Me.lblDeviceName.AutoSize = True
        Me.lblDeviceName.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblDeviceName.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDeviceName.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.lblDeviceName.Location = New System.Drawing.Point(45, 6)
        Me.lblDeviceName.Name = "lblDeviceName"
        Me.lblDeviceName.Padding = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.lblDeviceName.Size = New System.Drawing.Size(98, 16)
        Me.lblDeviceName.TabIndex = 5
        Me.lblDeviceName.Text = "Device Name"
        '
        'lblEq1Flow
        '
        Me.lblEq1Flow.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblEq1Flow.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblEq1Flow.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEq1Flow.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.lblEq1Flow.Location = New System.Drawing.Point(20, 140)
        Me.lblEq1Flow.Name = "lblEq1Flow"
        Me.lblEq1Flow.Size = New System.Drawing.Size(70, 32)
        Me.lblEq1Flow.TabIndex = 6
        Me.lblEq1Flow.Text = "EQ1 AIR  FLOW"
        Me.lblEq1Flow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblEq2Pressure
        '
        Me.lblEq2Pressure.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblEq2Pressure.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblEq2Pressure.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEq2Pressure.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.lblEq2Pressure.Location = New System.Drawing.Point(104, 26)
        Me.lblEq2Pressure.Name = "lblEq2Pressure"
        Me.lblEq2Pressure.Size = New System.Drawing.Size(70, 32)
        Me.lblEq2Pressure.TabIndex = 7
        Me.lblEq2Pressure.Text = "EQ2 AIR PRESSURE"
        Me.lblEq2Pressure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblEq2Flow
        '
        Me.lblEq2Flow.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblEq2Flow.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblEq2Flow.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEq2Flow.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.lblEq2Flow.Location = New System.Drawing.Point(104, 140)
        Me.lblEq2Flow.Name = "lblEq2Flow"
        Me.lblEq2Flow.Size = New System.Drawing.Size(70, 32)
        Me.lblEq2Flow.TabIndex = 8
        Me.lblEq2Flow.Text = "EQ2 AIR  FLOW"
        Me.lblEq2Flow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblPurgeEnabled
        '
        Me.lblPurgeEnabled.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblPurgeEnabled.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblPurgeEnabled.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPurgeEnabled.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.lblPurgeEnabled.Location = New System.Drawing.Point(20, 254)
        Me.lblPurgeEnabled.Name = "lblPurgeEnabled"
        Me.lblPurgeEnabled.Size = New System.Drawing.Size(70, 32)
        Me.lblPurgeEnabled.TabIndex = 10
        Me.lblPurgeEnabled.Text = "PURGE ENABLED"
        Me.lblPurgeEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblPurgeFault
        '
        Me.lblPurgeFault.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblPurgeFault.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblPurgeFault.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPurgeFault.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.lblPurgeFault.Location = New System.Drawing.Point(104, 254)
        Me.lblPurgeFault.Name = "lblPurgeFault"
        Me.lblPurgeFault.Size = New System.Drawing.Size(70, 32)
        Me.lblPurgeFault.TabIndex = 12
        Me.lblPurgeFault.Text = "PURGE FAULT"
        Me.lblPurgeFault.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblPurgeStart
        '
        Me.lblPurgeStart.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblPurgeStart.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblPurgeStart.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPurgeStart.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.lblPurgeStart.Location = New System.Drawing.Point(20, 367)
        Me.lblPurgeStart.Name = "lblPurgeStart"
        Me.lblPurgeStart.Size = New System.Drawing.Size(70, 32)
        Me.lblPurgeStart.TabIndex = 14
        Me.lblPurgeStart.Text = "PURGE START"
        Me.lblPurgeStart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblRobotPower
        '
        Me.lblRobotPower.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblRobotPower.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblRobotPower.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblRobotPower.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.lblRobotPower.Location = New System.Drawing.Point(104, 367)
        Me.lblRobotPower.Name = "lblRobotPower"
        Me.lblRobotPower.Size = New System.Drawing.Size(70, 32)
        Me.lblRobotPower.TabIndex = 16
        Me.lblRobotPower.Text = "ROBOT POWER"
        Me.lblRobotPower.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.ProgressBar1.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.ProgressBar1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.ProgressBar1.Location = New System.Drawing.Point(96, 508)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(168, 23)
        Me.ProgressBar1.TabIndex = 17
        Me.ProgressBar1.Value = 100
        '
        'lblPurgeProgress
        '
        Me.lblPurgeProgress.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.lblPurgeProgress.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblPurgeProgress.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPurgeProgress.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.lblPurgeProgress.Location = New System.Drawing.Point(96, 483)
        Me.lblPurgeProgress.Name = "lblPurgeProgress"
        Me.lblPurgeProgress.Size = New System.Drawing.Size(168, 21)
        Me.lblPurgeProgress.TabIndex = 18
        Me.lblPurgeProgress.Text = "PURGE PROGRESS"
        Me.lblPurgeProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'picRobotPowerPL
        '
        Me.picRobotPowerPL.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.picRobotPowerPL.BackgroundImage = Global.PurgePanel.My.Resources.Resources.AB_Green_PL_OFF
        Me.picRobotPowerPL.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.picRobotPowerPL.Location = New System.Drawing.Point(104, 403)
        Me.picRobotPowerPL.Name = "picRobotPowerPL"
        Me.picRobotPowerPL.Size = New System.Drawing.Size(70, 70)
        Me.picRobotPowerPL.TabIndex = 15
        Me.picRobotPowerPL.TabStop = False
        '
        'picPurgeStartPB
        '
        Me.picPurgeStartPB.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.picPurgeStartPB.BackgroundImage = Global.PurgePanel.My.Resources.Resources.AB_Black_PB_UP
        Me.picPurgeStartPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.picPurgeStartPB.Cursor = System.Windows.Forms.Cursors.Hand
        Me.picPurgeStartPB.Location = New System.Drawing.Point(20, 403)
        Me.picPurgeStartPB.Name = "picPurgeStartPB"
        Me.picPurgeStartPB.Size = New System.Drawing.Size(70, 70)
        Me.picPurgeStartPB.TabIndex = 13
        Me.picPurgeStartPB.TabStop = False
        '
        'picPurgeFaultPL
        '
        Me.picPurgeFaultPL.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.picPurgeFaultPL.BackgroundImage = Global.PurgePanel.My.Resources.Resources.AB_Red_PL_OFF
        Me.picPurgeFaultPL.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.picPurgeFaultPL.Location = New System.Drawing.Point(104, 289)
        Me.picPurgeFaultPL.Name = "picPurgeFaultPL"
        Me.picPurgeFaultPL.Size = New System.Drawing.Size(70, 70)
        Me.picPurgeFaultPL.TabIndex = 11
        Me.picPurgeFaultPL.TabStop = False
        '
        'picPurgeEnabledPL
        '
        Me.picPurgeEnabledPL.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.picPurgeEnabledPL.BackgroundImage = Global.PurgePanel.My.Resources.Resources.AB_Amber_PL_OFF
        Me.picPurgeEnabledPL.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.picPurgeEnabledPL.Location = New System.Drawing.Point(20, 289)
        Me.picPurgeEnabledPL.Name = "picPurgeEnabledPL"
        Me.picPurgeEnabledPL.Size = New System.Drawing.Size(70, 70)
        Me.picPurgeEnabledPL.TabIndex = 9
        Me.picPurgeEnabledPL.TabStop = False
        '
        'picEq2FlowPL
        '
        Me.picEq2FlowPL.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.picEq2FlowPL.BackgroundImage = Global.PurgePanel.My.Resources.Resources.AB_Green_PL_OFF
        Me.picEq2FlowPL.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.picEq2FlowPL.Location = New System.Drawing.Point(104, 175)
        Me.picEq2FlowPL.Name = "picEq2FlowPL"
        Me.picEq2FlowPL.Size = New System.Drawing.Size(70, 70)
        Me.picEq2FlowPL.TabIndex = 3
        Me.picEq2FlowPL.TabStop = False
        '
        'picEq2PressurePL
        '
        Me.picEq2PressurePL.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.picEq2PressurePL.BackgroundImage = Global.PurgePanel.My.Resources.Resources.AB_Green_PL_OFF
        Me.picEq2PressurePL.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.picEq2PressurePL.Location = New System.Drawing.Point(104, 61)
        Me.picEq2PressurePL.Name = "picEq2PressurePL"
        Me.picEq2PressurePL.Size = New System.Drawing.Size(70, 70)
        Me.picEq2PressurePL.TabIndex = 2
        Me.picEq2PressurePL.TabStop = False
        '
        'picEq1FlowPL
        '
        Me.picEq1FlowPL.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.picEq1FlowPL.BackgroundImage = Global.PurgePanel.My.Resources.Resources.AB_Green_PL_OFF
        Me.picEq1FlowPL.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.picEq1FlowPL.Location = New System.Drawing.Point(20, 175)
        Me.picEq1FlowPL.Name = "picEq1FlowPL"
        Me.picEq1FlowPL.Size = New System.Drawing.Size(70, 70)
        Me.picEq1FlowPL.TabIndex = 1
        Me.picEq1FlowPL.TabStop = False
        '
        'picEq1PressurePL
        '
        Me.picEq1PressurePL.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.picEq1PressurePL.BackgroundImage = Global.PurgePanel.My.Resources.Resources.AB_Green_PL_OFF
        Me.picEq1PressurePL.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.picEq1PressurePL.Location = New System.Drawing.Point(20, 61)
        Me.picEq1PressurePL.Name = "picEq1PressurePL"
        Me.picEq1PressurePL.Size = New System.Drawing.Size(70, 70)
        Me.picEq1PressurePL.TabIndex = 0
        Me.picEq1PressurePL.TabStop = False
        '
        'lblEq4Flow
        '
        Me.lblEq4Flow.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblEq4Flow.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblEq4Flow.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEq4Flow.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.lblEq4Flow.Location = New System.Drawing.Point(271, 140)
        Me.lblEq4Flow.Name = "lblEq4Flow"
        Me.lblEq4Flow.Size = New System.Drawing.Size(70, 32)
        Me.lblEq4Flow.TabIndex = 26
        Me.lblEq4Flow.Text = "EQ4 AIR  FLOW"
        Me.lblEq4Flow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblEq4Pressure
        '
        Me.lblEq4Pressure.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblEq4Pressure.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblEq4Pressure.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEq4Pressure.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.lblEq4Pressure.Location = New System.Drawing.Point(271, 26)
        Me.lblEq4Pressure.Name = "lblEq4Pressure"
        Me.lblEq4Pressure.Size = New System.Drawing.Size(70, 32)
        Me.lblEq4Pressure.TabIndex = 25
        Me.lblEq4Pressure.Text = "EQ4 AIR PRESSURE"
        Me.lblEq4Pressure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblEq3Flow
        '
        Me.lblEq3Flow.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblEq3Flow.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblEq3Flow.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEq3Flow.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.lblEq3Flow.Location = New System.Drawing.Point(188, 140)
        Me.lblEq3Flow.Name = "lblEq3Flow"
        Me.lblEq3Flow.Size = New System.Drawing.Size(70, 32)
        Me.lblEq3Flow.TabIndex = 24
        Me.lblEq3Flow.Text = "EQ3 AIR  FLOW"
        Me.lblEq3Flow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblEq3Pressure
        '
        Me.lblEq3Pressure.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblEq3Pressure.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblEq3Pressure.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEq3Pressure.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.lblEq3Pressure.Location = New System.Drawing.Point(188, 26)
        Me.lblEq3Pressure.Name = "lblEq3Pressure"
        Me.lblEq3Pressure.Size = New System.Drawing.Size(70, 32)
        Me.lblEq3Pressure.TabIndex = 23
        Me.lblEq3Pressure.Text = "EQ3 AIR PRESSURE"
        Me.lblEq3Pressure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'picEq4FlowPL
        '
        Me.picEq4FlowPL.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.picEq4FlowPL.BackgroundImage = Global.PurgePanel.My.Resources.Resources.AB_Green_PL_OFF
        Me.picEq4FlowPL.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.picEq4FlowPL.Location = New System.Drawing.Point(271, 175)
        Me.picEq4FlowPL.Name = "picEq4FlowPL"
        Me.picEq4FlowPL.Size = New System.Drawing.Size(70, 70)
        Me.picEq4FlowPL.TabIndex = 22
        Me.picEq4FlowPL.TabStop = False
        '
        'picEq4PressurePL
        '
        Me.picEq4PressurePL.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.picEq4PressurePL.BackgroundImage = Global.PurgePanel.My.Resources.Resources.AB_Green_PL_OFF
        Me.picEq4PressurePL.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.picEq4PressurePL.Location = New System.Drawing.Point(271, 61)
        Me.picEq4PressurePL.Name = "picEq4PressurePL"
        Me.picEq4PressurePL.Size = New System.Drawing.Size(70, 70)
        Me.picEq4PressurePL.TabIndex = 21
        Me.picEq4PressurePL.TabStop = False
        '
        'picEq3FlowPL
        '
        Me.picEq3FlowPL.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.picEq3FlowPL.BackgroundImage = Global.PurgePanel.My.Resources.Resources.AB_Green_PL_OFF
        Me.picEq3FlowPL.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.picEq3FlowPL.Location = New System.Drawing.Point(188, 175)
        Me.picEq3FlowPL.Name = "picEq3FlowPL"
        Me.picEq3FlowPL.Size = New System.Drawing.Size(70, 70)
        Me.picEq3FlowPL.TabIndex = 20
        Me.picEq3FlowPL.TabStop = False
        '
        'picEq3PressurePL
        '
        Me.picEq3PressurePL.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.picEq3PressurePL.BackgroundImage = Global.PurgePanel.My.Resources.Resources.AB_Green_PL_OFF
        Me.picEq3PressurePL.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.picEq3PressurePL.Location = New System.Drawing.Point(188, 61)
        Me.picEq3PressurePL.Name = "picEq3PressurePL"
        Me.picEq3PressurePL.Size = New System.Drawing.Size(70, 70)
        Me.picEq3PressurePL.TabIndex = 19
        Me.picEq3PressurePL.TabStop = False
        '
        'PurgePanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Global.PurgePanel.My.Resources.Resources.Panel
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.Controls.Add(Me.lblEq4Flow)
        Me.Controls.Add(Me.lblEq4Pressure)
        Me.Controls.Add(Me.lblEq3Flow)
        Me.Controls.Add(Me.lblEq3Pressure)
        Me.Controls.Add(Me.picEq4FlowPL)
        Me.Controls.Add(Me.picEq4PressurePL)
        Me.Controls.Add(Me.picEq3FlowPL)
        Me.Controls.Add(Me.picEq3PressurePL)
        Me.Controls.Add(Me.lblPurgeProgress)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Controls.Add(Me.lblRobotPower)
        Me.Controls.Add(Me.picRobotPowerPL)
        Me.Controls.Add(Me.lblPurgeStart)
        Me.Controls.Add(Me.picPurgeStartPB)
        Me.Controls.Add(Me.lblPurgeFault)
        Me.Controls.Add(Me.picPurgeFaultPL)
        Me.Controls.Add(Me.lblPurgeEnabled)
        Me.Controls.Add(Me.picPurgeEnabledPL)
        Me.Controls.Add(Me.lblEq2Flow)
        Me.Controls.Add(Me.lblEq2Pressure)
        Me.Controls.Add(Me.lblEq1Flow)
        Me.Controls.Add(Me.lblDeviceName)
        Me.Controls.Add(Me.lblEq1Pressure)
        Me.Controls.Add(Me.picEq2FlowPL)
        Me.Controls.Add(Me.picEq2PressurePL)
        Me.Controls.Add(Me.picEq1FlowPL)
        Me.Controls.Add(Me.picEq1PressurePL)
        Me.MaximumSize = New System.Drawing.Size(360, 572)
        Me.MinimumSize = New System.Drawing.Size(194, 572)
        Me.Name = "PurgePanel"
        Me.Size = New System.Drawing.Size(360, 572)
        CType(Me.picRobotPowerPL, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picPurgeStartPB, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picPurgeFaultPL, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picPurgeEnabledPL, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picEq2FlowPL, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picEq2PressurePL, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picEq1FlowPL, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picEq1PressurePL, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picEq4FlowPL, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picEq4PressurePL, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picEq3FlowPL, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picEq3PressurePL, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents picEq1PressurePL As System.Windows.Forms.PictureBox
    Friend WithEvents picEq1FlowPL As System.Windows.Forms.PictureBox
    Friend WithEvents picEq2PressurePL As System.Windows.Forms.PictureBox
    Friend WithEvents picEq2FlowPL As System.Windows.Forms.PictureBox
    Friend WithEvents lblEq1Pressure As System.Windows.Forms.Label
    Friend WithEvents lblDeviceName As System.Windows.Forms.Label
    Friend WithEvents lblEq1Flow As System.Windows.Forms.Label
    Friend WithEvents lblEq2Pressure As System.Windows.Forms.Label
    Friend WithEvents lblEq2Flow As System.Windows.Forms.Label
    Friend WithEvents lblPurgeEnabled As System.Windows.Forms.Label
    Friend WithEvents picPurgeEnabledPL As System.Windows.Forms.PictureBox
    Friend WithEvents lblPurgeFault As System.Windows.Forms.Label
    Friend WithEvents picPurgeFaultPL As System.Windows.Forms.PictureBox
    Friend WithEvents lblPurgeStart As System.Windows.Forms.Label
    Friend WithEvents picPurgeStartPB As System.Windows.Forms.PictureBox
    Friend WithEvents lblRobotPower As System.Windows.Forms.Label
    Friend WithEvents picRobotPowerPL As System.Windows.Forms.PictureBox
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents lblPurgeProgress As System.Windows.Forms.Label
    Friend WithEvents lblEq4Flow As System.Windows.Forms.Label
    Friend WithEvents lblEq4Pressure As System.Windows.Forms.Label
    Friend WithEvents lblEq3Flow As System.Windows.Forms.Label
    Friend WithEvents lblEq3Pressure As System.Windows.Forms.Label
    Friend WithEvents picEq4FlowPL As System.Windows.Forms.PictureBox
    Friend WithEvents picEq4PressurePL As System.Windows.Forms.PictureBox
    Friend WithEvents picEq3FlowPL As System.Windows.Forms.PictureBox
    Friend WithEvents picEq3PressurePL As System.Windows.Forms.PictureBox

End Class
