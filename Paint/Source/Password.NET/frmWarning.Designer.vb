<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmWarning
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmWarning))
        Me.picWarning = New System.Windows.Forms.PictureBox
        Me.btnReset = New System.Windows.Forms.Button
        Me.lblTimeRemaining = New System.Windows.Forms.Label
        Me.lblMessage = New System.Windows.Forms.Label
        Me.lblClock = New System.Windows.Forms.Label
        CType(Me.picWarning, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'picWarning
        '
        Me.picWarning.Image = CType(resources.GetObject("picWarning.Image"), System.Drawing.Image)
        Me.picWarning.Location = New System.Drawing.Point(12, 12)
        Me.picWarning.Name = "picWarning"
        Me.picWarning.Size = New System.Drawing.Size(48, 48)
        Me.picWarning.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picWarning.TabIndex = 0
        Me.picWarning.TabStop = False
        '
        'btnReset
        '
        Me.btnReset.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnReset.Location = New System.Drawing.Point(169, 128)
        Me.btnReset.Name = "btnReset"
        Me.btnReset.Size = New System.Drawing.Size(136, 23)
        Me.btnReset.TabIndex = 1
        Me.btnReset.Text = "&Reset"
        Me.btnReset.UseVisualStyleBackColor = True
        '
        'lblTimeRemaining
        '
        Me.lblTimeRemaining.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTimeRemaining.Location = New System.Drawing.Point(82, 12)
        Me.lblTimeRemaining.Name = "lblTimeRemaining"
        Me.lblTimeRemaining.Size = New System.Drawing.Size(283, 36)
        Me.lblTimeRemaining.TabIndex = 2
        Me.lblTimeRemaining.Text = "Time remaining before you wil be logged out:"
        '
        'lblMessage
        '
        Me.lblMessage.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMessage.Location = New System.Drawing.Point(82, 71)
        Me.lblMessage.Name = "lblMessage"
        Me.lblMessage.Size = New System.Drawing.Size(283, 54)
        Me.lblMessage.TabIndex = 3
        Me.lblMessage.Text = "Click the Reset button to reset the Automatic Logout Timer and remain logged in."
        '
        'lblClock
        '
        Me.lblClock.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblClock.Location = New System.Drawing.Point(371, 12)
        Me.lblClock.Name = "lblClock"
        Me.lblClock.Size = New System.Drawing.Size(79, 18)
        Me.lblClock.TabIndex = 4
        Me.lblClock.Text = "HH:MM:SS"
        '
        'frmWarning
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(473, 163)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblClock)
        Me.Controls.Add(Me.lblMessage)
        Me.Controls.Add(Me.lblTimeRemaining)
        Me.Controls.Add(Me.btnReset)
        Me.Controls.Add(Me.picWarning)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmWarning"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Automatic Logout Warning"
        Me.TopMost = True
        CType(Me.picWarning, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents picWarning As System.Windows.Forms.PictureBox
    Friend WithEvents btnReset As System.Windows.Forms.Button
    Friend WithEvents lblTimeRemaining As System.Windows.Forms.Label
    Friend WithEvents lblMessage As System.Windows.Forms.Label
    Friend WithEvents lblClock As System.Windows.Forms.Label
End Class
