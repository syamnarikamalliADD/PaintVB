<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class uctlDevice
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(uctlDevice))
        Me.gpbDevice = New System.Windows.Forms.GroupBox
        Me.picAppicatorStatus = New System.Windows.Forms.PictureBox
        Me.mnuApplStatus = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuApplStatusItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuApplStatusItem2 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuApplStatusItem3 = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuApplStatusItem4 = New System.Windows.Forms.ToolStripMenuItem
        Me.picBypassProx = New System.Windows.Forms.PictureBox
        Me.picEstat = New System.Windows.Forms.PictureBox
        Me.lblPosition = New System.Windows.Forms.Label
        Me.ttDevice = New System.Windows.Forms.ToolTip(Me.components)
        Me.imlStatus = New System.Windows.Forms.ImageList(Me.components)
        Me.gpbDevice.SuspendLayout()
        CType(Me.picAppicatorStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.mnuApplStatus.SuspendLayout()
        CType(Me.picBypassProx, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picEstat, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'gpbDevice
        '
        Me.gpbDevice.Controls.Add(Me.picAppicatorStatus)
        Me.gpbDevice.Controls.Add(Me.picBypassProx)
        Me.gpbDevice.Controls.Add(Me.picEstat)
        Me.gpbDevice.Controls.Add(Me.lblPosition)
        Me.gpbDevice.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gpbDevice.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gpbDevice.Location = New System.Drawing.Point(0, 0)
        Me.gpbDevice.Margin = New System.Windows.Forms.Padding(0)
        Me.gpbDevice.Name = "gpbDevice"
        Me.gpbDevice.Size = New System.Drawing.Size(191, 43)
        Me.gpbDevice.TabIndex = 0
        Me.gpbDevice.TabStop = False
        Me.gpbDevice.Text = "DeviceName"
        '
        'picAppicatorStatus
        '
        Me.picAppicatorStatus.ContextMenuStrip = Me.mnuApplStatus
        Me.picAppicatorStatus.Image = Global.BSD.My.Resources.Resources.gray
        Me.picAppicatorStatus.Location = New System.Drawing.Point(169, 17)
        Me.picAppicatorStatus.Name = "picAppicatorStatus"
        Me.picAppicatorStatus.Size = New System.Drawing.Size(16, 16)
        Me.picAppicatorStatus.TabIndex = 8
        Me.picAppicatorStatus.TabStop = False
        '
        'mnuApplStatus
        '
        Me.mnuApplStatus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuApplStatusItem1, Me.mnuApplStatusItem2, Me.mnuApplStatusItem3, Me.mnuApplStatusItem4})
        Me.mnuApplStatus.Name = "mnuApplStatus"
        Me.mnuApplStatus.Size = New System.Drawing.Size(187, 92)
        '
        'mnuApplStatusItem1
        '
        Me.mnuApplStatusItem1.Image = Global.BSD.My.Resources.Resources.gray
        Me.mnuApplStatusItem1.Name = "mnuApplStatusItem1"
        Me.mnuApplStatusItem1.Size = New System.Drawing.Size(186, 22)
        Me.mnuApplStatusItem1.Tag = "0"
        Me.mnuApplStatusItem1.Text = "mnuApplStatusItem1"
        '
        'mnuApplStatusItem2
        '
        Me.mnuApplStatusItem2.Image = Global.BSD.My.Resources.Resources.gray
        Me.mnuApplStatusItem2.Name = "mnuApplStatusItem2"
        Me.mnuApplStatusItem2.Size = New System.Drawing.Size(186, 22)
        Me.mnuApplStatusItem2.Tag = "0"
        Me.mnuApplStatusItem2.Text = "mnuApplStatusItem2"
        '
        'mnuApplStatusItem3
        '
        Me.mnuApplStatusItem3.Image = Global.BSD.My.Resources.Resources.gray
        Me.mnuApplStatusItem3.Name = "mnuApplStatusItem3"
        Me.mnuApplStatusItem3.Size = New System.Drawing.Size(186, 22)
        Me.mnuApplStatusItem3.Tag = "0"
        Me.mnuApplStatusItem3.Text = "mnuApplStatusItem3"
        '
        'mnuApplStatusItem4
        '
        Me.mnuApplStatusItem4.Image = Global.BSD.My.Resources.Resources.gray
        Me.mnuApplStatusItem4.Name = "mnuApplStatusItem4"
        Me.mnuApplStatusItem4.Size = New System.Drawing.Size(186, 22)
        Me.mnuApplStatusItem4.Tag = "0"
        Me.mnuApplStatusItem4.Text = "mnuApplStatusItem4"
        '
        'picBypassProx
        '
        Me.picBypassProx.Image = Global.BSD.My.Resources.Resources.prox_gray
        Me.picBypassProx.Location = New System.Drawing.Point(149, 17)
        Me.picBypassProx.Name = "picBypassProx"
        Me.picBypassProx.Size = New System.Drawing.Size(16, 16)
        Me.picBypassProx.TabIndex = 7
        Me.picBypassProx.TabStop = False
        Me.picBypassProx.Tag = "imlEstat"
        '
        'picEstat
        '
        Me.picEstat.Image = Global.BSD.My.Resources.Resources.EstatUnknown
        Me.picEstat.Location = New System.Drawing.Point(129, 17)
        Me.picEstat.Name = "picEstat"
        Me.picEstat.Size = New System.Drawing.Size(16, 16)
        Me.picEstat.TabIndex = 6
        Me.picEstat.TabStop = False
        Me.picEstat.Tag = "imlEstat"
        '
        'lblPosition
        '
        Me.lblPosition.BackColor = System.Drawing.Color.Transparent
        Me.lblPosition.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPosition.Location = New System.Drawing.Point(2, 15)
        Me.lblPosition.Name = "lblPosition"
        Me.lblPosition.Size = New System.Drawing.Size(121, 20)
        Me.lblPosition.TabIndex = 5
        Me.lblPosition.Text = "Unknown"
        Me.lblPosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ttDevice
        '
        Me.ttDevice.IsBalloon = True
        '
        'imlStatus
        '
        Me.imlStatus.ImageStream = CType(resources.GetObject("imlStatus.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlStatus.TransparentColor = System.Drawing.Color.Transparent
        Me.imlStatus.Images.SetKeyName(0, "green")
        Me.imlStatus.Images.SetKeyName(1, "red")
        '
        'uctlDevice
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Transparent
        Me.Controls.Add(Me.gpbDevice)
        Me.Margin = New System.Windows.Forms.Padding(0)
        Me.MaximumSize = New System.Drawing.Size(191, 53)
        Me.MinimumSize = New System.Drawing.Size(191, 43)
        Me.Name = "uctlDevice"
        Me.Size = New System.Drawing.Size(191, 43)
        Me.gpbDevice.ResumeLayout(False)
        CType(Me.picAppicatorStatus, System.ComponentModel.ISupportInitialize).EndInit()
        Me.mnuApplStatus.ResumeLayout(False)
        CType(Me.picBypassProx, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picEstat, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents gpbDevice As System.Windows.Forms.GroupBox
    Friend WithEvents lblPosition As System.Windows.Forms.Label
    Friend WithEvents picEstat As System.Windows.Forms.PictureBox
    Friend WithEvents picBypassProx As System.Windows.Forms.PictureBox
    Friend WithEvents ttDevice As System.Windows.Forms.ToolTip
    Friend WithEvents picAppicatorStatus As System.Windows.Forms.PictureBox
    Friend WithEvents imlStatus As System.Windows.Forms.ImageList

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Friend WithEvents mnuApplStatus As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuApplStatusItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuApplStatusItem2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuApplStatusItem3 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuApplStatusItem4 As System.Windows.Forms.ToolStripMenuItem
End Class
