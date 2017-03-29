<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmFirst
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFirst))
        Me.lstDebugOutput = New System.Windows.Forms.ListBox
        Me.pnlLoadError = New System.Windows.Forms.Panel
        Me.btnAbort = New System.Windows.Forms.Button
        Me.btnOK = New System.Windows.Forms.Button
        Me.lblLoadError = New System.Windows.Forms.Label
        Me.picRobot = New System.Windows.Forms.PictureBox
        Me.picFANUC = New System.Windows.Forms.PictureBox
        Me.lblPAINTworks = New System.Windows.Forms.Label
        Me.pnlLoadError.SuspendLayout()
        CType(Me.picRobot, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picFANUC, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lstDebugOutput
        '
        Me.lstDebugOutput.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.lstDebugOutput.FormattingEnabled = True
        Me.lstDebugOutput.Location = New System.Drawing.Point(28, 168)
        Me.lstDebugOutput.Name = "lstDebugOutput"
        Me.lstDebugOutput.ScrollAlwaysVisible = True
        Me.lstDebugOutput.Size = New System.Drawing.Size(390, 186)
        Me.lstDebugOutput.TabIndex = 0
        '
        'pnlLoadError
        '
        Me.pnlLoadError.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.pnlLoadError.BackColor = System.Drawing.Color.Transparent
        Me.pnlLoadError.Controls.Add(Me.btnAbort)
        Me.pnlLoadError.Controls.Add(Me.btnOK)
        Me.pnlLoadError.Controls.Add(Me.lblLoadError)
        Me.pnlLoadError.Location = New System.Drawing.Point(28, 363)
        Me.pnlLoadError.Name = "pnlLoadError"
        Me.pnlLoadError.Size = New System.Drawing.Size(390, 76)
        Me.pnlLoadError.TabIndex = 1
        Me.pnlLoadError.Visible = False
        '
        'btnAbort
        '
        Me.btnAbort.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.btnAbort.Location = New System.Drawing.Point(214, 44)
        Me.btnAbort.Name = "btnAbort"
        Me.btnAbort.Size = New System.Drawing.Size(75, 23)
        Me.btnAbort.TabIndex = 2
        Me.btnAbort.Text = "Abort"
        Me.btnAbort.UseVisualStyleBackColor = True
        '
        'btnOK
        '
        Me.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.btnOK.Location = New System.Drawing.Point(103, 44)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 1
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'lblLoadError
        '
        Me.lblLoadError.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.lblLoadError.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLoadError.Location = New System.Drawing.Point(3, 9)
        Me.lblLoadError.Name = "lblLoadError"
        Me.lblLoadError.Size = New System.Drawing.Size(384, 16)
        Me.lblLoadError.TabIndex = 0
        Me.lblLoadError.Text = "Errors occurred during Startup"
        Me.lblLoadError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'picRobot
        '
        Me.picRobot.BackColor = System.Drawing.Color.Transparent
        Me.picRobot.Image = CType(resources.GetObject("picRobot.Image"), System.Drawing.Image)
        Me.picRobot.Location = New System.Drawing.Point(379, 12)
        Me.picRobot.Name = "picRobot"
        Me.picRobot.Size = New System.Drawing.Size(279, 398)
        Me.picRobot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picRobot.TabIndex = 2
        Me.picRobot.TabStop = False
        '
        'picFANUC
        '
        Me.picFANUC.BackColor = System.Drawing.Color.Transparent
        Me.picFANUC.Image = CType(resources.GetObject("picFANUC.Image"), System.Drawing.Image)
        Me.picFANUC.Location = New System.Drawing.Point(192, 12)
        Me.picFANUC.Name = "picFANUC"
        Me.picFANUC.Size = New System.Drawing.Size(223, 35)
        Me.picFANUC.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picFANUC.TabIndex = 3
        Me.picFANUC.TabStop = False
        '
        'lblPAINTworks
        '
        Me.lblPAINTworks.AutoSize = True
        Me.lblPAINTworks.BackColor = System.Drawing.Color.Transparent
        Me.lblPAINTworks.Font = New System.Drawing.Font("Tahoma", 30.0!, System.Drawing.FontStyle.Bold)
        Me.lblPAINTworks.ForeColor = System.Drawing.Color.FromArgb(CType(CType(186, Byte), Integer), CType(CType(12, Byte), Integer), CType(CType(47, Byte), Integer))
        Me.lblPAINTworks.Location = New System.Drawing.Point(114, 50)
        Me.lblPAINTworks.Name = "lblPAINTworks"
        Me.lblPAINTworks.Size = New System.Drawing.Size(340, 48)
        Me.lblPAINTworks.TabIndex = 4
        Me.lblPAINTworks.Text = "PAINTworks"
        '
        'frmFirst
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(640, 480)
        Me.Controls.Add(Me.picFANUC)
        Me.Controls.Add(Me.lstDebugOutput)
        Me.Controls.Add(Me.lblPAINTworks)
        Me.Controls.Add(Me.pnlLoadError)
        Me.Controls.Add(Me.picRobot)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(640, 480)
        Me.Name = "frmFirst"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.Text = "frmFirst"
        Me.TopMost = True
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.pnlLoadError.ResumeLayout(False)
        CType(Me.picRobot, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picFANUC, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lstDebugOutput As System.Windows.Forms.ListBox
    Friend WithEvents pnlLoadError As System.Windows.Forms.Panel
    Friend WithEvents lblLoadError As System.Windows.Forms.Label
    Friend WithEvents btnAbort As System.Windows.Forms.Button
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents picRobot As System.Windows.Forms.PictureBox
    Friend WithEvents picFANUC As System.Windows.Forms.PictureBox
    Friend WithEvents lblPAINTworks As System.Windows.Forms.Label
End Class
