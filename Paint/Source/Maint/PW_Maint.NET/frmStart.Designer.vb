<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmStart
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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmStart))
        Me.lblTimeRemaining = New System.Windows.Forms.Label
        Me.lblCountDown = New System.Windows.Forms.Label
        Me.tmrCountDown = New System.Windows.Forms.Timer(Me.components)
        Me.lblMsg = New System.Windows.Forms.Label
        Me.btnOK = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.pnlCheckBoxes = New System.Windows.Forms.Panel
        Me.chkCleanup = New System.Windows.Forms.CheckBox
        Me.chkRobotBackup = New System.Windows.Forms.CheckBox
        Me.chkFiles = New System.Windows.Forms.CheckBox
        Me.pnlCheckBoxes.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTimeRemaining
        '
        Me.lblTimeRemaining.AutoSize = True
        Me.lblTimeRemaining.Location = New System.Drawing.Point(186, 9)
        Me.lblTimeRemaining.Name = "lblTimeRemaining"
        Me.lblTimeRemaining.Size = New System.Drawing.Size(90, 13)
        Me.lblTimeRemaining.TabIndex = 0
        Me.lblTimeRemaining.Text = "lblTimeRemaining"
        Me.lblTimeRemaining.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblCountDown
        '
        Me.lblCountDown.AutoSize = True
        Me.lblCountDown.Location = New System.Drawing.Point(282, 9)
        Me.lblCountDown.Name = "lblCountDown"
        Me.lblCountDown.Size = New System.Drawing.Size(34, 13)
        Me.lblCountDown.TabIndex = 1
        Me.lblCountDown.Text = "00:00"
        '
        'tmrCountDown
        '
        '
        'lblMsg
        '
        Me.lblMsg.Location = New System.Drawing.Point(12, 36)
        Me.lblMsg.Name = "lblMsg"
        Me.lblMsg.Size = New System.Drawing.Size(342, 74)
        Me.lblMsg.TabIndex = 2
        Me.lblMsg.Text = "lblMsg"
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(91, 113)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 3
        Me.btnOK.Text = "btnOK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(189, 113)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 4
        Me.btnCancel.Text = "btnCancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'pnlCheckBoxes
        '
        Me.pnlCheckBoxes.Controls.Add(Me.chkFiles)
        Me.pnlCheckBoxes.Controls.Add(Me.chkRobotBackup)
        Me.pnlCheckBoxes.Controls.Add(Me.chkCleanup)
        Me.pnlCheckBoxes.Location = New System.Drawing.Point(179, 31)
        Me.pnlCheckBoxes.Name = "pnlCheckBoxes"
        Me.pnlCheckBoxes.Size = New System.Drawing.Size(189, 74)
        Me.pnlCheckBoxes.TabIndex = 5
        '
        'chkCleanup
        '
        Me.chkCleanup.AutoSize = True
        Me.chkCleanup.Location = New System.Drawing.Point(3, 2)
        Me.chkCleanup.Name = "chkCleanup"
        Me.chkCleanup.Size = New System.Drawing.Size(138, 17)
        Me.chkCleanup.TabIndex = 0
        Me.chkCleanup.Text = "File/Database Cleanup "
        Me.chkCleanup.UseVisualStyleBackColor = True
        '
        'chkRobotBackup
        '
        Me.chkRobotBackup.AutoSize = True
        Me.chkRobotBackup.Location = New System.Drawing.Point(3, 24)
        Me.chkRobotBackup.Name = "chkRobotBackup"
        Me.chkRobotBackup.Size = New System.Drawing.Size(98, 17)
        Me.chkRobotBackup.TabIndex = 1
        Me.chkRobotBackup.Text = "Robot Backup "
        Me.chkRobotBackup.UseVisualStyleBackColor = True
        '
        'chkFiles
        '
        Me.chkFiles.AutoSize = True
        Me.chkFiles.Location = New System.Drawing.Point(3, 46)
        Me.chkFiles.Name = "chkFiles"
        Me.chkFiles.Size = New System.Drawing.Size(148, 17)
        Me.chkFiles.TabIndex = 2
        Me.chkFiles.Text = "PAINTworks File Backup "
        Me.chkFiles.UseVisualStyleBackColor = True
        '
        'frmStart
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(366, 148)
        Me.ControlBox = False
        Me.Controls.Add(Me.pnlCheckBoxes)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.lblMsg)
        Me.Controls.Add(Me.lblCountDown)
        Me.Controls.Add(Me.lblTimeRemaining)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.MinimizeBox = False
        Me.Name = "frmStart"
        Me.ShowInTaskbar = False
        Me.Text = "PWMaint Startup Notofication"
        Me.TopMost = True
        Me.pnlCheckBoxes.ResumeLayout(False)
        Me.pnlCheckBoxes.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblTimeRemaining As System.Windows.Forms.Label
    Friend WithEvents lblCountDown As System.Windows.Forms.Label
    Friend WithEvents tmrCountDown As System.Windows.Forms.Timer
    Friend WithEvents lblMsg As System.Windows.Forms.Label
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents pnlCheckBoxes As System.Windows.Forms.Panel
    Friend WithEvents chkFiles As System.Windows.Forms.CheckBox
    Friend WithEvents chkRobotBackup As System.Windows.Forms.CheckBox
    Friend WithEvents chkCleanup As System.Windows.Forms.CheckBox

End Class
