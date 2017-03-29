<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAxisJog
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAxisJog))
        Me.ToolStripContainer1 = New System.Windows.Forms.ToolStripContainer
        Me.picJog = New System.Windows.Forms.PictureBox
        Me.gpbAxis = New System.Windows.Forms.GroupBox
        Me.rdoDisable = New System.Windows.Forms.RadioButton
        Me.rdoAxis9 = New System.Windows.Forms.RadioButton
        Me.rdoAxis8 = New System.Windows.Forms.RadioButton
        Me.rdoAxis7 = New System.Windows.Forms.RadioButton
        Me.rdoAxis6 = New System.Windows.Forms.RadioButton
        Me.rdoAxis5 = New System.Windows.Forms.RadioButton
        Me.rdoAxis4 = New System.Windows.Forms.RadioButton
        Me.rdoAxis3 = New System.Windows.Forms.RadioButton
        Me.rdoAxis2 = New System.Windows.Forms.RadioButton
        Me.rdoAxis1 = New System.Windows.Forms.RadioButton
        Me.gpbCoord = New System.Windows.Forms.GroupBox
        Me.rdoWorld = New System.Windows.Forms.RadioButton
        Me.rdoJoint = New System.Windows.Forms.RadioButton
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.lblRobot = New System.Windows.Forms.ToolStripLabel
        Me.cboRobot = New System.Windows.Forms.ToolStripComboBox
        Me.ToolStripContainer1.ContentPanel.SuspendLayout()
        Me.ToolStripContainer1.TopToolStripPanel.SuspendLayout()
        Me.ToolStripContainer1.SuspendLayout()
        CType(Me.picJog, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gpbAxis.SuspendLayout()
        Me.gpbCoord.SuspendLayout()
        Me.tlsMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolStripContainer1
        '
        Me.ToolStripContainer1.BottomToolStripPanelVisible = False
        '
        'ToolStripContainer1.ContentPanel
        '
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.picJog)
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.gpbAxis)
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.gpbCoord)
        resources.ApplyResources(Me.ToolStripContainer1.ContentPanel, "ToolStripContainer1.ContentPanel")
        resources.ApplyResources(Me.ToolStripContainer1, "ToolStripContainer1")
        Me.ToolStripContainer1.LeftToolStripPanelVisible = False
        Me.ToolStripContainer1.Name = "ToolStripContainer1"
        Me.ToolStripContainer1.RightToolStripPanelVisible = False
        '
        'ToolStripContainer1.TopToolStripPanel
        '
        Me.ToolStripContainer1.TopToolStripPanel.Controls.Add(Me.tlsMain)
        '
        'picJog
        '
        Me.picJog.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.picJog, "picJog")
        Me.picJog.Name = "picJog"
        Me.picJog.TabStop = False
        '
        'gpbAxis
        '
        Me.gpbAxis.Controls.Add(Me.rdoDisable)
        Me.gpbAxis.Controls.Add(Me.rdoAxis9)
        Me.gpbAxis.Controls.Add(Me.rdoAxis8)
        Me.gpbAxis.Controls.Add(Me.rdoAxis7)
        Me.gpbAxis.Controls.Add(Me.rdoAxis6)
        Me.gpbAxis.Controls.Add(Me.rdoAxis5)
        Me.gpbAxis.Controls.Add(Me.rdoAxis4)
        Me.gpbAxis.Controls.Add(Me.rdoAxis3)
        Me.gpbAxis.Controls.Add(Me.rdoAxis2)
        Me.gpbAxis.Controls.Add(Me.rdoAxis1)
        resources.ApplyResources(Me.gpbAxis, "gpbAxis")
        Me.gpbAxis.Name = "gpbAxis"
        Me.gpbAxis.TabStop = False
        '
        'rdoDisable
        '
        resources.ApplyResources(Me.rdoDisable, "rdoDisable")
        Me.rdoDisable.Name = "rdoDisable"
        Me.rdoDisable.TabStop = True
        Me.rdoDisable.UseVisualStyleBackColor = True
        '
        'rdoAxis9
        '
        resources.ApplyResources(Me.rdoAxis9, "rdoAxis9")
        Me.rdoAxis9.Name = "rdoAxis9"
        Me.rdoAxis9.TabStop = True
        Me.rdoAxis9.UseVisualStyleBackColor = True
        '
        'rdoAxis8
        '
        resources.ApplyResources(Me.rdoAxis8, "rdoAxis8")
        Me.rdoAxis8.Name = "rdoAxis8"
        Me.rdoAxis8.TabStop = True
        Me.rdoAxis8.UseVisualStyleBackColor = True
        '
        'rdoAxis7
        '
        resources.ApplyResources(Me.rdoAxis7, "rdoAxis7")
        Me.rdoAxis7.Name = "rdoAxis7"
        Me.rdoAxis7.TabStop = True
        Me.rdoAxis7.UseVisualStyleBackColor = True
        '
        'rdoAxis6
        '
        resources.ApplyResources(Me.rdoAxis6, "rdoAxis6")
        Me.rdoAxis6.Name = "rdoAxis6"
        Me.rdoAxis6.TabStop = True
        Me.rdoAxis6.UseVisualStyleBackColor = True
        '
        'rdoAxis5
        '
        resources.ApplyResources(Me.rdoAxis5, "rdoAxis5")
        Me.rdoAxis5.Name = "rdoAxis5"
        Me.rdoAxis5.TabStop = True
        Me.rdoAxis5.UseVisualStyleBackColor = True
        '
        'rdoAxis4
        '
        resources.ApplyResources(Me.rdoAxis4, "rdoAxis4")
        Me.rdoAxis4.Name = "rdoAxis4"
        Me.rdoAxis4.TabStop = True
        Me.rdoAxis4.UseVisualStyleBackColor = True
        '
        'rdoAxis3
        '
        resources.ApplyResources(Me.rdoAxis3, "rdoAxis3")
        Me.rdoAxis3.Name = "rdoAxis3"
        Me.rdoAxis3.TabStop = True
        Me.rdoAxis3.UseVisualStyleBackColor = True
        '
        'rdoAxis2
        '
        resources.ApplyResources(Me.rdoAxis2, "rdoAxis2")
        Me.rdoAxis2.Name = "rdoAxis2"
        Me.rdoAxis2.TabStop = True
        Me.rdoAxis2.UseVisualStyleBackColor = True
        '
        'rdoAxis1
        '
        resources.ApplyResources(Me.rdoAxis1, "rdoAxis1")
        Me.rdoAxis1.Name = "rdoAxis1"
        Me.rdoAxis1.TabStop = True
        Me.rdoAxis1.UseVisualStyleBackColor = True
        '
        'gpbCoord
        '
        Me.gpbCoord.Controls.Add(Me.rdoWorld)
        Me.gpbCoord.Controls.Add(Me.rdoJoint)
        resources.ApplyResources(Me.gpbCoord, "gpbCoord")
        Me.gpbCoord.Name = "gpbCoord"
        Me.gpbCoord.TabStop = False
        '
        'rdoWorld
        '
        resources.ApplyResources(Me.rdoWorld, "rdoWorld")
        Me.rdoWorld.Name = "rdoWorld"
        Me.rdoWorld.TabStop = True
        Me.rdoWorld.UseVisualStyleBackColor = True
        '
        'rdoJoint
        '
        resources.ApplyResources(Me.rdoJoint, "rdoJoint")
        Me.rdoJoint.Name = "rdoJoint"
        Me.rdoJoint.TabStop = True
        Me.rdoJoint.UseVisualStyleBackColor = True
        '
        'tlsMain
        '
        resources.ApplyResources(Me.tlsMain, "tlsMain")
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.lblRobot, Me.cboRobot})
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
        'lblRobot
        '
        Me.lblRobot.Name = "lblRobot"
        resources.ApplyResources(Me.lblRobot, "lblRobot")
        '
        'cboRobot
        '
        Me.cboRobot.Name = "cboRobot"
        resources.ApplyResources(Me.cboRobot, "cboRobot")
        '
        'frmAxisJog
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.ToolStripContainer1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmAxisJog"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.TopMost = True
        Me.ToolStripContainer1.ContentPanel.ResumeLayout(False)
        Me.ToolStripContainer1.TopToolStripPanel.ResumeLayout(False)
        Me.ToolStripContainer1.TopToolStripPanel.PerformLayout()
        Me.ToolStripContainer1.ResumeLayout(False)
        Me.ToolStripContainer1.PerformLayout()
        CType(Me.picJog, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gpbAxis.ResumeLayout(False)
        Me.gpbAxis.PerformLayout()
        Me.gpbCoord.ResumeLayout(False)
        Me.gpbCoord.PerformLayout()
        Me.tlsMain.ResumeLayout(False)
        Me.tlsMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnClose As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripContainer1 As System.Windows.Forms.ToolStripContainer
    Friend WithEvents gpbAxis As System.Windows.Forms.GroupBox
    Friend WithEvents gpbCoord As System.Windows.Forms.GroupBox
    Friend WithEvents rdoWorld As System.Windows.Forms.RadioButton
    Friend WithEvents rdoJoint As System.Windows.Forms.RadioButton
    Friend WithEvents rdoAxis7 As System.Windows.Forms.RadioButton
    Friend WithEvents rdoAxis6 As System.Windows.Forms.RadioButton
    Friend WithEvents rdoAxis5 As System.Windows.Forms.RadioButton
    Friend WithEvents rdoAxis4 As System.Windows.Forms.RadioButton
    Friend WithEvents rdoAxis3 As System.Windows.Forms.RadioButton
    Friend WithEvents rdoAxis2 As System.Windows.Forms.RadioButton
    Friend WithEvents rdoAxis1 As System.Windows.Forms.RadioButton
    Friend WithEvents picJog As System.Windows.Forms.PictureBox
    Friend WithEvents rdoAxis9 As System.Windows.Forms.RadioButton
    Friend WithEvents rdoAxis8 As System.Windows.Forms.RadioButton
    Friend WithEvents lblRobot As System.Windows.Forms.ToolStripLabel
    Friend WithEvents cboRobot As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents rdoDisable As System.Windows.Forms.RadioButton
End Class
