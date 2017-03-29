<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmOverrides
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmOverrides))
        Me.tscMain = New System.Windows.Forms.ToolStripContainer
        Me.btnDefault = New System.Windows.Forms.Button
        Me.chkEnable = New System.Windows.Forms.CheckBox
        Me.tkbParm04 = New System.Windows.Forms.TrackBar
        Me.lblParm04 = New System.Windows.Forms.Label
        Me.tkbParm03 = New System.Windows.Forms.TrackBar
        Me.lblParm03 = New System.Windows.Forms.Label
        Me.tkbParm02 = New System.Windows.Forms.TrackBar
        Me.lblParm02 = New System.Windows.Forms.Label
        Me.tkbParm01 = New System.Windows.Forms.TrackBar
        Me.lblParm01 = New System.Windows.Forms.Label
        Me.tlsMain = New System.Windows.Forms.ToolStrip
        Me.btnClose = New System.Windows.Forms.ToolStripButton
        Me.btnSave = New System.Windows.Forms.ToolStripButton
        Me.tscMain.ContentPanel.SuspendLayout()
        Me.tscMain.TopToolStripPanel.SuspendLayout()
        Me.tscMain.SuspendLayout()
        CType(Me.tkbParm04, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tkbParm03, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tkbParm02, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tkbParm01, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tlsMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'tscMain
        '
        Me.tscMain.BottomToolStripPanelVisible = False
        '
        'tscMain.ContentPanel
        '
        Me.tscMain.ContentPanel.Controls.Add(Me.btnDefault)
        Me.tscMain.ContentPanel.Controls.Add(Me.chkEnable)
        Me.tscMain.ContentPanel.Controls.Add(Me.tkbParm04)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblParm04)
        Me.tscMain.ContentPanel.Controls.Add(Me.tkbParm03)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblParm03)
        Me.tscMain.ContentPanel.Controls.Add(Me.tkbParm02)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblParm02)
        Me.tscMain.ContentPanel.Controls.Add(Me.tkbParm01)
        Me.tscMain.ContentPanel.Controls.Add(Me.lblParm01)
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
        'btnDefault
        '
        resources.ApplyResources(Me.btnDefault, "btnDefault")
        Me.btnDefault.Name = "btnDefault"
        Me.btnDefault.UseVisualStyleBackColor = True
        '
        'chkEnable
        '
        resources.ApplyResources(Me.chkEnable, "chkEnable")
        Me.chkEnable.Name = "chkEnable"
        Me.chkEnable.UseVisualStyleBackColor = True
        '
        'tkbParm04
        '
        resources.ApplyResources(Me.tkbParm04, "tkbParm04")
        Me.tkbParm04.Maximum = 150
        Me.tkbParm04.Minimum = 50
        Me.tkbParm04.Name = "tkbParm04"
        Me.tkbParm04.TickFrequency = 5
        Me.tkbParm04.Value = 100
        '
        'lblParm04
        '
        resources.ApplyResources(Me.lblParm04, "lblParm04")
        Me.lblParm04.Name = "lblParm04"
        '
        'tkbParm03
        '
        resources.ApplyResources(Me.tkbParm03, "tkbParm03")
        Me.tkbParm03.Maximum = 150
        Me.tkbParm03.Minimum = 50
        Me.tkbParm03.Name = "tkbParm03"
        Me.tkbParm03.TickFrequency = 5
        Me.tkbParm03.Value = 100
        '
        'lblParm03
        '
        resources.ApplyResources(Me.lblParm03, "lblParm03")
        Me.lblParm03.Name = "lblParm03"
        '
        'tkbParm02
        '
        resources.ApplyResources(Me.tkbParm02, "tkbParm02")
        Me.tkbParm02.Maximum = 150
        Me.tkbParm02.Minimum = 50
        Me.tkbParm02.Name = "tkbParm02"
        Me.tkbParm02.TickFrequency = 5
        Me.tkbParm02.Value = 100
        '
        'lblParm02
        '
        resources.ApplyResources(Me.lblParm02, "lblParm02")
        Me.lblParm02.Name = "lblParm02"
        '
        'tkbParm01
        '
        resources.ApplyResources(Me.tkbParm01, "tkbParm01")
        Me.tkbParm01.Maximum = 150
        Me.tkbParm01.Minimum = 50
        Me.tkbParm01.Name = "tkbParm01"
        Me.tkbParm01.TickFrequency = 5
        Me.tkbParm01.Value = 100
        '
        'lblParm01
        '
        resources.ApplyResources(Me.lblParm01, "lblParm01")
        Me.lblParm01.Name = "lblParm01"
        '
        'tlsMain
        '
        resources.ApplyResources(Me.tlsMain, "tlsMain")
        Me.tlsMain.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.tlsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnClose, Me.btnSave})
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
        'btnSave
        '
        resources.ApplyResources(Me.btnSave, "btnSave")
        Me.btnSave.Name = "btnSave"
        '
        'frmOverrides
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.tscMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmOverrides"
        Me.TopMost = True
        Me.tscMain.ContentPanel.ResumeLayout(False)
        Me.tscMain.ContentPanel.PerformLayout()
        Me.tscMain.TopToolStripPanel.ResumeLayout(False)
        Me.tscMain.TopToolStripPanel.PerformLayout()
        Me.tscMain.ResumeLayout(False)
        Me.tscMain.PerformLayout()
        CType(Me.tkbParm04, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tkbParm03, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tkbParm02, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tkbParm01, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tlsMain.ResumeLayout(False)
        Me.tlsMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tlsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnClose As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnSave As System.Windows.Forms.ToolStripButton
    Friend WithEvents tscMain As System.Windows.Forms.ToolStripContainer
    Friend WithEvents btnDefault As System.Windows.Forms.Button
    Friend WithEvents chkEnable As System.Windows.Forms.CheckBox
    Friend WithEvents tkbParm04 As System.Windows.Forms.TrackBar
    Friend WithEvents lblParm04 As System.Windows.Forms.Label
    Friend WithEvents tkbParm03 As System.Windows.Forms.TrackBar
    Friend WithEvents lblParm03 As System.Windows.Forms.Label
    Friend WithEvents tkbParm02 As System.Windows.Forms.TrackBar
    Friend WithEvents lblParm02 As System.Windows.Forms.Label
    Friend WithEvents tkbParm01 As System.Windows.Forms.TrackBar
    Friend WithEvents lblParm01 As System.Windows.Forms.Label
End Class
