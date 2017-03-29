<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class uctlRC
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
        Me.gpbRC = New System.Windows.Forms.GroupBox
        Me.lblPurge = New System.Windows.Forms.Label
        Me.picPurge = New System.Windows.Forms.PictureBox
        Me.lblTP = New System.Windows.Forms.Label
        Me.lblDisconnect = New System.Windows.Forms.Label
        Me.lblEstop = New System.Windows.Forms.Label
        Me.picServoDisc = New System.Windows.Forms.PictureBox
        Me.picTP = New System.Windows.Forms.PictureBox
        Me.picSOPEstop = New System.Windows.Forms.PictureBox
        Me.ttRC = New System.Windows.Forms.ToolTip(Me.components)
        Me.gpbRC.SuspendLayout()
        CType(Me.picPurge, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picServoDisc, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picTP, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picSOPEstop, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'gpbRC
        '
        Me.gpbRC.BackColor = System.Drawing.Color.Transparent
        Me.gpbRC.Controls.Add(Me.lblPurge)
        Me.gpbRC.Controls.Add(Me.picPurge)
        Me.gpbRC.Controls.Add(Me.lblTP)
        Me.gpbRC.Controls.Add(Me.lblDisconnect)
        Me.gpbRC.Controls.Add(Me.lblEstop)
        Me.gpbRC.Controls.Add(Me.picServoDisc)
        Me.gpbRC.Controls.Add(Me.picTP)
        Me.gpbRC.Controls.Add(Me.picSOPEstop)
        Me.gpbRC.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gpbRC.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gpbRC.Location = New System.Drawing.Point(0, 0)
        Me.gpbRC.Name = "gpbRC"
        Me.gpbRC.Size = New System.Drawing.Size(175, 62)
        Me.gpbRC.TabIndex = 0
        Me.gpbRC.TabStop = False
        Me.gpbRC.Text = "RC???"
        '
        'lblPurge
        '
        Me.lblPurge.AutoSize = True
        Me.lblPurge.BackColor = System.Drawing.Color.Transparent
        Me.lblPurge.Font = New System.Drawing.Font("Arial", 9.0!)
        Me.lblPurge.Location = New System.Drawing.Point(135, 42)
        Me.lblPurge.Name = "lblPurge"
        Me.lblPurge.Size = New System.Drawing.Size(40, 15)
        Me.lblPurge.TabIndex = 17
        Me.lblPurge.Text = "Purge"
        Me.lblPurge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'picPurge
        '
        Me.picPurge.Image = Global.BSD.My.Resources.Resources.gray
        Me.picPurge.Location = New System.Drawing.Point(138, 15)
        Me.picPurge.Name = "picPurge"
        Me.picPurge.Size = New System.Drawing.Size(24, 24)
        Me.picPurge.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picPurge.TabIndex = 16
        Me.picPurge.TabStop = False
        Me.picPurge.Tag = "imlLed"
        '
        'lblTP
        '
        Me.lblTP.AutoSize = True
        Me.lblTP.BackColor = System.Drawing.Color.Transparent
        Me.lblTP.Font = New System.Drawing.Font("Arial", 9.0!)
        Me.lblTP.Location = New System.Drawing.Point(99, 42)
        Me.lblTP.Name = "lblTP"
        Me.lblTP.Size = New System.Drawing.Size(22, 15)
        Me.lblTP.TabIndex = 8
        Me.lblTP.Text = "TP"
        Me.lblTP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblDisconnect
        '
        Me.lblDisconnect.AutoSize = True
        Me.lblDisconnect.BackColor = System.Drawing.Color.Transparent
        Me.lblDisconnect.Font = New System.Drawing.Font("Arial", 9.0!)
        Me.lblDisconnect.Location = New System.Drawing.Point(6, 42)
        Me.lblDisconnect.Name = "lblDisconnect"
        Me.lblDisconnect.Size = New System.Drawing.Size(38, 15)
        Me.lblDisconnect.TabIndex = 7
        Me.lblDisconnect.Text = "Servo"
        Me.lblDisconnect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblEstop
        '
        Me.lblEstop.AutoSize = True
        Me.lblEstop.BackColor = System.Drawing.Color.Transparent
        Me.lblEstop.Font = New System.Drawing.Font("Arial", 9.0!)
        Me.lblEstop.Location = New System.Drawing.Point(46, 42)
        Me.lblEstop.Name = "lblEstop"
        Me.lblEstop.Size = New System.Drawing.Size(43, 15)
        Me.lblEstop.TabIndex = 6
        Me.lblEstop.Text = "E-stop"
        Me.lblEstop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'picServoDisc
        '
        Me.picServoDisc.Image = Global.BSD.My.Resources.Resources.Disc
        Me.picServoDisc.Location = New System.Drawing.Point(9, 15)
        Me.picServoDisc.Name = "picServoDisc"
        Me.picServoDisc.Size = New System.Drawing.Size(24, 24)
        Me.picServoDisc.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picServoDisc.TabIndex = 5
        Me.picServoDisc.TabStop = False
        Me.picServoDisc.Tag = "imlDisc"
        '
        'picTP
        '
        Me.picTP.Image = Global.BSD.My.Resources.Resources.Pendant
        Me.picTP.Location = New System.Drawing.Point(95, 15)
        Me.picTP.Name = "picTP"
        Me.picTP.Size = New System.Drawing.Size(24, 24)
        Me.picTP.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picTP.TabIndex = 4
        Me.picTP.TabStop = False
        Me.picTP.Tag = "imlPendant"
        '
        'picSOPEstop
        '
        Me.picSOPEstop.Image = Global.BSD.My.Resources.Resources.EstopOFF
        Me.picSOPEstop.Location = New System.Drawing.Point(52, 15)
        Me.picSOPEstop.Name = "picSOPEstop"
        Me.picSOPEstop.Size = New System.Drawing.Size(24, 24)
        Me.picSOPEstop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picSOPEstop.TabIndex = 3
        Me.picSOPEstop.TabStop = False
        Me.picSOPEstop.Tag = "imlEstop_Cstop"
        '
        'ttRC
        '
        Me.ttRC.IsBalloon = True
        '
        'uctlRC
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Transparent
        Me.Controls.Add(Me.gpbRC)
        Me.Name = "uctlRC"
        Me.Size = New System.Drawing.Size(175, 62)
        Me.gpbRC.ResumeLayout(False)
        Me.gpbRC.PerformLayout()
        CType(Me.picPurge, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picServoDisc, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picTP, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picSOPEstop, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents gpbRC As System.Windows.Forms.GroupBox
    Friend WithEvents picServoDisc As System.Windows.Forms.PictureBox
    Friend WithEvents picTP As System.Windows.Forms.PictureBox
    Friend WithEvents picSOPEstop As System.Windows.Forms.PictureBox
    Friend WithEvents lblEstop As System.Windows.Forms.Label
    Friend WithEvents lblTP As System.Windows.Forms.Label
    Friend WithEvents lblDisconnect As System.Windows.Forms.Label
    Friend WithEvents picPurge As System.Windows.Forms.PictureBox
    Friend WithEvents lblPurge As System.Windows.Forms.Label
    Friend WithEvents ttRC As System.Windows.Forms.ToolTip

End Class
