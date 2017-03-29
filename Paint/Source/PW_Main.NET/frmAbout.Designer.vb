<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAbout
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAbout))
        Me.pnlAbout = New System.Windows.Forms.Panel
        Me.picFANUC = New System.Windows.Forms.PictureBox
        Me.lblSiteName = New System.Windows.Forms.Label
        Me.lblRegistered = New System.Windows.Forms.Label
        Me.lblAmerica = New System.Windows.Forms.Label
        Me.lblCopyrightInfo = New System.Windows.Forms.Label
        Me.lblPWVersion = New System.Windows.Forms.Label
        Me.picRobot = New System.Windows.Forms.PictureBox
        Me.lblWarning = New System.Windows.Forms.Label
        Me.btnSysInfo = New System.Windows.Forms.Button
        Me.btnOK = New System.Windows.Forms.Button
        Me.btnRegister = New System.Windows.Forms.Button
        Me.cboLanguage = New System.Windows.Forms.ComboBox
        Me.pnlFlag = New System.Windows.Forms.Panel
        Me.btnVersions = New System.Windows.Forms.Button
        Me.pnlAbout.SuspendLayout()
        CType(Me.picFANUC, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picRobot, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pnlAbout
        '
        Me.pnlAbout.BackgroundImage = CType(resources.GetObject("pnlAbout.BackgroundImage"), System.Drawing.Image)
        Me.pnlAbout.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.pnlAbout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlAbout.Controls.Add(Me.picFANUC)
        Me.pnlAbout.Controls.Add(Me.lblSiteName)
        Me.pnlAbout.Controls.Add(Me.lblRegistered)
        Me.pnlAbout.Controls.Add(Me.lblAmerica)
        Me.pnlAbout.Controls.Add(Me.lblCopyrightInfo)
        Me.pnlAbout.Controls.Add(Me.lblPWVersion)
        Me.pnlAbout.Controls.Add(Me.picRobot)
        Me.pnlAbout.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlAbout.Location = New System.Drawing.Point(0, 0)
        Me.pnlAbout.Name = "pnlAbout"
        Me.pnlAbout.Size = New System.Drawing.Size(478, 120)
        Me.pnlAbout.TabIndex = 0
        '
        'picFANUC
        '
        Me.picFANUC.BackColor = System.Drawing.Color.Transparent
        Me.picFANUC.Image = CType(resources.GetObject("picFANUC.Image"), System.Drawing.Image)
        Me.picFANUC.Location = New System.Drawing.Point(93, 20)
        Me.picFANUC.Name = "picFANUC"
        Me.picFANUC.Size = New System.Drawing.Size(63, 21)
        Me.picFANUC.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picFANUC.TabIndex = 7
        Me.picFANUC.TabStop = False
        '
        'lblSiteName
        '
        Me.lblSiteName.AutoSize = True
        Me.lblSiteName.BackColor = System.Drawing.Color.Transparent
        Me.lblSiteName.ForeColor = System.Drawing.Color.Black
        Me.lblSiteName.Location = New System.Drawing.Point(307, 40)
        Me.lblSiteName.Name = "lblSiteName"
        Me.lblSiteName.Size = New System.Drawing.Size(79, 13)
        Me.lblSiteName.TabIndex = 5
        Me.lblSiteName.Text = "<Unregistered>"
        '
        'lblRegistered
        '
        Me.lblRegistered.AutoSize = True
        Me.lblRegistered.BackColor = System.Drawing.Color.Transparent
        Me.lblRegistered.ForeColor = System.Drawing.Color.Black
        Me.lblRegistered.Location = New System.Drawing.Point(307, 24)
        Me.lblRegistered.Name = "lblRegistered"
        Me.lblRegistered.Size = New System.Drawing.Size(77, 13)
        Me.lblRegistered.TabIndex = 4
        Me.lblRegistered.Text = "Registered To:"
        '
        'lblAmerica
        '
        Me.lblAmerica.AutoSize = True
        Me.lblAmerica.BackColor = System.Drawing.Color.Transparent
        Me.lblAmerica.ForeColor = System.Drawing.Color.Black
        Me.lblAmerica.Location = New System.Drawing.Point(156, 24)
        Me.lblAmerica.Name = "lblAmerica"
        Me.lblAmerica.Size = New System.Drawing.Size(45, 13)
        Me.lblAmerica.TabIndex = 3
        Me.lblAmerica.Text = "America"
        '
        'lblCopyrightInfo
        '
        Me.lblCopyrightInfo.AutoSize = True
        Me.lblCopyrightInfo.BackColor = System.Drawing.Color.Transparent
        Me.lblCopyrightInfo.Location = New System.Drawing.Point(89, 66)
        Me.lblCopyrightInfo.Name = "lblCopyrightInfo"
        Me.lblCopyrightInfo.Size = New System.Drawing.Size(144, 13)
        Me.lblCopyrightInfo.TabIndex = 2
        Me.lblCopyrightInfo.Text = "© 2009 FANUC America, Inc"
        '
        'lblPWVersion
        '
        Me.lblPWVersion.AutoSize = True
        Me.lblPWVersion.BackColor = System.Drawing.Color.Transparent
        Me.lblPWVersion.Location = New System.Drawing.Point(89, 40)
        Me.lblPWVersion.Name = "lblPWVersion"
        Me.lblPWVersion.Size = New System.Drawing.Size(153, 13)
        Me.lblPWVersion.TabIndex = 1
        Me.lblPWVersion.Text = "PAINTworks Version 4.xx.xx.xx"
        '
        'picRobot
        '
        Me.picRobot.BackColor = System.Drawing.Color.Transparent
        Me.picRobot.Image = CType(resources.GetObject("picRobot.Image"), System.Drawing.Image)
        Me.picRobot.InitialImage = CType(resources.GetObject("picRobot.InitialImage"), System.Drawing.Image)
        Me.picRobot.Location = New System.Drawing.Point(-1, -1)
        Me.picRobot.Name = "picRobot"
        Me.picRobot.Size = New System.Drawing.Size(102, 120)
        Me.picRobot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picRobot.TabIndex = 6
        Me.picRobot.TabStop = False
        '
        'lblWarning
        '
        Me.lblWarning.BackColor = System.Drawing.Color.Transparent
        Me.lblWarning.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.lblWarning.Location = New System.Drawing.Point(0, 272)
        Me.lblWarning.Name = "lblWarning"
        Me.lblWarning.Size = New System.Drawing.Size(478, 53)
        Me.lblWarning.TabIndex = 3
        Me.lblWarning.Text = resources.GetString("lblWarning.Text")
        '
        'btnSysInfo
        '
        Me.btnSysInfo.Location = New System.Drawing.Point(12, 230)
        Me.btnSysInfo.Name = "btnSysInfo"
        Me.btnSysInfo.Size = New System.Drawing.Size(90, 25)
        Me.btnSysInfo.TabIndex = 4
        Me.btnSysInfo.Text = "System Info..."
        Me.btnSysInfo.UseVisualStyleBackColor = True
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(376, 230)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(90, 25)
        Me.btnOK.TabIndex = 5
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnRegister
        '
        Me.btnRegister.Location = New System.Drawing.Point(12, 147)
        Me.btnRegister.Name = "btnRegister"
        Me.btnRegister.Size = New System.Drawing.Size(90, 25)
        Me.btnRegister.TabIndex = 6
        Me.btnRegister.Text = "Register"
        Me.btnRegister.UseVisualStyleBackColor = True
        '
        'cboLanguage
        '
        Me.cboLanguage.FormattingEnabled = True
        Me.cboLanguage.Location = New System.Drawing.Point(131, 147)
        Me.cboLanguage.Name = "cboLanguage"
        Me.cboLanguage.Size = New System.Drawing.Size(215, 21)
        Me.cboLanguage.TabIndex = 7
        '
        'pnlFlag
        '
        Me.pnlFlag.BackgroundImage = Global.PW4_Main.My.Resources.ProjectStrings.Flgusa
        Me.pnlFlag.Location = New System.Drawing.Point(376, 140)
        Me.pnlFlag.Name = "pnlFlag"
        Me.pnlFlag.Size = New System.Drawing.Size(32, 32)
        Me.pnlFlag.TabIndex = 8
        '
        'btnVersions
        '
        Me.btnVersions.Location = New System.Drawing.Point(12, 189)
        Me.btnVersions.Name = "btnVersions"
        Me.btnVersions.Size = New System.Drawing.Size(90, 25)
        Me.btnVersions.TabIndex = 9
        Me.btnVersions.Text = "Versions"
        Me.btnVersions.UseVisualStyleBackColor = True
        '
        'frmAbout
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Info
        Me.ClientSize = New System.Drawing.Size(478, 325)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnVersions)
        Me.Controls.Add(Me.pnlFlag)
        Me.Controls.Add(Me.cboLanguage)
        Me.Controls.Add(Me.btnRegister)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.btnSysInfo)
        Me.Controls.Add(Me.lblWarning)
        Me.Controls.Add(Me.pnlAbout)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.KeyPreview = True
        Me.Name = "frmAbout"
        Me.Text = "About PAINTworks"
        Me.pnlAbout.ResumeLayout(False)
        Me.pnlAbout.PerformLayout()
        CType(Me.picFANUC, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picRobot, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents pnlAbout As System.Windows.Forms.Panel
    Friend WithEvents lblPWVersion As System.Windows.Forms.Label
    Friend WithEvents lblCopyrightInfo As System.Windows.Forms.Label
    Friend WithEvents lblWarning As System.Windows.Forms.Label
    Friend WithEvents btnSysInfo As System.Windows.Forms.Button
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnRegister As System.Windows.Forms.Button
    Friend WithEvents cboLanguage As System.Windows.Forms.ComboBox
    Friend WithEvents pnlFlag As System.Windows.Forms.Panel
    Friend WithEvents btnVersions As System.Windows.Forms.Button
    Friend WithEvents lblSiteName As System.Windows.Forms.Label
    Friend WithEvents lblRegistered As System.Windows.Forms.Label
    Friend WithEvents lblAmerica As System.Windows.Forms.Label
    Friend WithEvents picRobot As System.Windows.Forms.PictureBox
    Friend WithEvents picFANUC As System.Windows.Forms.PictureBox
End Class
