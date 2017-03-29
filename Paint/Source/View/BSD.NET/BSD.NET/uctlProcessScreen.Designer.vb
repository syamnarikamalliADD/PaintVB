<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class uctlProcessScreen
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
        Me.pnlProcess = New System.Windows.Forms.Panel
        Me.pnlRobotNames = New System.Windows.Forms.Panel
        Me.lblR1 = New System.Windows.Forms.Label
        Me.pnlLabels = New System.Windows.Forms.Panel
        Me.lblInvisbleLabel = New System.Windows.Forms.Label
        Me.mnuProcessPopUp = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuRobotName = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.mnuDetailView = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuSeparator2 = New System.Windows.Forms.ToolStripSeparator
        Me.mnuGraphParm1 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuGraphParm2 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuGraphParm3 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuGraphParm4 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuGraphParm5 = New System.Windows.Forms.ToolStripMenuItem
        Me.clbView = New System.Windows.Forms.CheckedListBox
        Me.lblViewArmCap = New System.Windows.Forms.Label
        Me.uctlLabels = New BSD.uctlList
        Me.uctlR1 = New BSD.uctlList
        Me.pnlProcess.SuspendLayout()
        Me.pnlRobotNames.SuspendLayout()
        Me.pnlLabels.SuspendLayout()
        Me.mnuProcessPopUp.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblViewRobotCap
        '
        Me.lblViewRobotCap.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblViewRobotCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblViewRobotCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblViewRobotCap.Location = New System.Drawing.Point(300, 9)
        Me.lblViewRobotCap.Name = "lblViewRobotCap"
        Me.lblViewRobotCap.Size = New System.Drawing.Size(200, 19)
        Me.lblViewRobotCap.TabIndex = 174
        Me.lblViewRobotCap.Text = "Robot"
        Me.lblViewRobotCap.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblViewRobotCap.UseMnemonic = False
        '
        'cboRobot
        '
        Me.cboRobot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboRobot.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.cboRobot.ItemHeight = 18
        Me.cboRobot.Location = New System.Drawing.Point(506, 6)
        Me.cboRobot.Name = "cboRobot"
        Me.cboRobot.Size = New System.Drawing.Size(160, 26)
        Me.cboRobot.TabIndex = 173
        '
        'pnlProcess
        '
        Me.pnlProcess.AutoScroll = True
        Me.pnlProcess.Controls.Add(Me.uctlR1)
        Me.pnlProcess.Location = New System.Drawing.Point(154, 63)
        Me.pnlProcess.Name = "pnlProcess"
        Me.pnlProcess.Size = New System.Drawing.Size(512, 556)
        Me.pnlProcess.TabIndex = 179
        '
        'pnlRobotNames
        '
        Me.pnlRobotNames.Controls.Add(Me.lblR1)
        Me.pnlRobotNames.Location = New System.Drawing.Point(154, 36)
        Me.pnlRobotNames.Name = "pnlRobotNames"
        Me.pnlRobotNames.Size = New System.Drawing.Size(512, 26)
        Me.pnlRobotNames.TabIndex = 183
        '
        'lblR1
        '
        Me.lblR1.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblR1.Location = New System.Drawing.Point(3, 0)
        Me.lblR1.Name = "lblR1"
        Me.lblR1.Size = New System.Drawing.Size(96, 24)
        Me.lblR1.TabIndex = 0
        Me.lblR1.Tag = "0"
        Me.lblR1.Text = "R1"
        Me.lblR1.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'pnlLabels
        '
        Me.pnlLabels.Controls.Add(Me.uctlLabels)
        Me.pnlLabels.Location = New System.Drawing.Point(3, 63)
        Me.pnlLabels.Name = "pnlLabels"
        Me.pnlLabels.Size = New System.Drawing.Size(148, 554)
        Me.pnlLabels.TabIndex = 184
        '
        'lblInvisbleLabel
        '
        Me.lblInvisbleLabel.AutoSize = True
        Me.lblInvisbleLabel.Location = New System.Drawing.Point(3, 6)
        Me.lblInvisbleLabel.Name = "lblInvisbleLabel"
        Me.lblInvisbleLabel.Size = New System.Drawing.Size(47, 14)
        Me.lblInvisbleLabel.TabIndex = 185
        Me.lblInvisbleLabel.Text = "Process"
        Me.lblInvisbleLabel.Visible = False
        '
        'mnuProcessPopUp
        '
        Me.mnuProcessPopUp.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuRobotName, Me.mnuSeparator1, Me.mnuDetailView, Me.mnuSeparator2, Me.mnuGraphParm1, Me.mnuGraphParm2, Me.mnuGraphParm3, Me.mnuGraphParm4, Me.mnuGraphParm5})
        Me.mnuProcessPopUp.Name = "mnuProcessPopUp"
        Me.mnuProcessPopUp.Size = New System.Drawing.Size(174, 170)
        '
        'mnuRobotName
        '
        Me.mnuRobotName.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.mnuRobotName.Name = "mnuRobotName"
        Me.mnuRobotName.Size = New System.Drawing.Size(173, 22)
        Me.mnuRobotName.Text = "mnuRobotName"
        '
        'mnuSeparator1
        '
        Me.mnuSeparator1.Name = "mnuSeparator1"
        Me.mnuSeparator1.Size = New System.Drawing.Size(170, 6)
        '
        'mnuDetailView
        '
        Me.mnuDetailView.Name = "mnuDetailView"
        Me.mnuDetailView.Size = New System.Drawing.Size(173, 22)
        Me.mnuDetailView.Text = "mnuDetailView"
        '
        'mnuSeparator2
        '
        Me.mnuSeparator2.Name = "mnuSeparator2"
        Me.mnuSeparator2.Size = New System.Drawing.Size(170, 6)
        '
        'mnuGraphParm1
        '
        Me.mnuGraphParm1.Name = "mnuGraphParm1"
        Me.mnuGraphParm1.Size = New System.Drawing.Size(173, 22)
        Me.mnuGraphParm1.Text = "mnuGraphParm1"
        '
        'mnuGraphParm2
        '
        Me.mnuGraphParm2.Name = "mnuGraphParm2"
        Me.mnuGraphParm2.Size = New System.Drawing.Size(173, 22)
        Me.mnuGraphParm2.Text = "mnuGraphParm2"
        '
        'mnuGraphParm3
        '
        Me.mnuGraphParm3.Name = "mnuGraphParm3"
        Me.mnuGraphParm3.Size = New System.Drawing.Size(173, 22)
        Me.mnuGraphParm3.Text = "mnuGraphParm3"
        '
        'mnuGraphParm4
        '
        Me.mnuGraphParm4.Name = "mnuGraphParm4"
        Me.mnuGraphParm4.Size = New System.Drawing.Size(173, 22)
        Me.mnuGraphParm4.Text = "mnuGraphParm4"
        '
        'mnuGraphParm5
        '
        Me.mnuGraphParm5.Name = "mnuGraphParm5"
        Me.mnuGraphParm5.Size = New System.Drawing.Size(173, 22)
        Me.mnuGraphParm5.Text = "mnuGraphParm5"
        '
        'clbView
        '
        Me.clbView.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.clbView.FormattingEnabled = True
        Me.clbView.Location = New System.Drawing.Point(290, 5)
        Me.clbView.Name = "clbView"
        Me.clbView.Size = New System.Drawing.Size(157, 25)
        Me.clbView.TabIndex = 189
        '
        'lblViewArmCap
        '
        Me.lblViewArmCap.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.lblViewArmCap.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblViewArmCap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblViewArmCap.Location = New System.Drawing.Point(84, 9)
        Me.lblViewArmCap.Name = "lblViewArmCap"
        Me.lblViewArmCap.Size = New System.Drawing.Size(200, 19)
        Me.lblViewArmCap.TabIndex = 190
        Me.lblViewArmCap.Text = "Arm"
        Me.lblViewArmCap.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblViewArmCap.UseMnemonic = False
        '
        'uctlLabels
        '
        Me.uctlLabels.LabelBorderStyle = System.Windows.Forms.BorderStyle.None
        Me.uctlLabels.LabelHeight = 34
        Me.uctlLabels.LabelSpacing = 8
        Me.uctlLabels.LabelTextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.uctlLabels.Location = New System.Drawing.Point(3, 5)
        Me.uctlLabels.Name = "uctlLabels"
        Me.uctlLabels.NumLabels = 2
        Me.uctlLabels.Size = New System.Drawing.Size(142, 61)
        Me.uctlLabels.TabIndex = 185
        '
        'uctlR1
        '
        Me.uctlR1.LabelBorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.uctlR1.LabelHeight = 34
        Me.uctlR1.LabelSpacing = 8
        Me.uctlR1.LabelTextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.uctlR1.Location = New System.Drawing.Point(3, 5)
        Me.uctlR1.Name = "uctlR1"
        Me.uctlR1.NumLabels = 2
        Me.uctlR1.Size = New System.Drawing.Size(96, 61)
        Me.uctlR1.TabIndex = 0
        '
        'uctlProcessScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lblViewArmCap)
        Me.Controls.Add(Me.clbView)
        Me.Controls.Add(Me.lblInvisbleLabel)
        Me.Controls.Add(Me.pnlLabels)
        Me.Controls.Add(Me.pnlRobotNames)
        Me.Controls.Add(Me.pnlProcess)
        Me.Controls.Add(Me.lblViewRobotCap)
        Me.Controls.Add(Me.cboRobot)
        Me.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "uctlProcessScreen"
        Me.Size = New System.Drawing.Size(1016, 622)
        Me.pnlProcess.ResumeLayout(False)
        Me.pnlRobotNames.ResumeLayout(False)
        Me.pnlLabels.ResumeLayout(False)
        Me.mnuProcessPopUp.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblViewRobotCap As System.Windows.Forms.Label
    Friend WithEvents cboRobot As System.Windows.Forms.ComboBox
    Friend WithEvents pnlProcess As System.Windows.Forms.Panel
    Friend WithEvents pnlRobotNames As System.Windows.Forms.Panel
    Friend WithEvents pnlLabels As System.Windows.Forms.Panel
    Friend WithEvents uctlLabels As BSD.uctlList
    Friend WithEvents lblR1 As System.Windows.Forms.Label
    Friend WithEvents lblInvisbleLabel As System.Windows.Forms.Label
    Friend WithEvents mnuProcessPopUp As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuRobotName As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuDetailView As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuGraphParm1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuGraphParm2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuGraphParm3 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuGraphParm4 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents mnuGraphParm5 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents clbView As System.Windows.Forms.CheckedListBox
    Friend WithEvents uctlR1 As BSD.uctlList
    Friend WithEvents lblViewArmCap As System.Windows.Forms.Label

End Class
