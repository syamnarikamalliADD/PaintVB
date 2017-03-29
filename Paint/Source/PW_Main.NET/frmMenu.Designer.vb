<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMenu
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
        Me.lblTitle = New System.Windows.Forms.Label
        Me.grpbStatus = New System.Windows.Forms.GroupBox
        Me.btnCancel = New System.Windows.Forms.Button
        Me.lblCaption = New System.Windows.Forms.Label
        Me.lblEmptyMenu = New System.Windows.Forms.Label
        Me.ToolTipMenu = New System.Windows.Forms.ToolTip(Me.components)
        Me.grpbStatus.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.RoyalBlue
        Me.lblTitle.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblTitle.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.ForeColor = System.Drawing.SystemColors.ControlLightLight
        Me.lblTitle.Location = New System.Drawing.Point(0, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(384, 23)
        Me.lblTitle.TabIndex = 1
        Me.lblTitle.Text = "SubMenu"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'grpbStatus
        '
        Me.grpbStatus.Controls.Add(Me.btnCancel)
        Me.grpbStatus.Controls.Add(Me.lblCaption)
        Me.grpbStatus.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.grpbStatus.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.grpbStatus.Location = New System.Drawing.Point(0, 222)
        Me.grpbStatus.Name = "grpbStatus"
        Me.grpbStatus.Size = New System.Drawing.Size(384, 32)
        Me.grpbStatus.TabIndex = 2
        Me.grpbStatus.TabStop = False
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCancel.Location = New System.Drawing.Point(302, 8)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(78, 20)
        Me.btnCancel.TabIndex = 3
        Me.btnCancel.Text = "Cancel"
        '
        'lblCaption
        '
        Me.lblCaption.AutoSize = True
        Me.lblCaption.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCaption.Location = New System.Drawing.Point(4, 12)
        Me.lblCaption.Name = "lblCaption"
        Me.lblCaption.Size = New System.Drawing.Size(53, 14)
        Me.lblCaption.TabIndex = 2
        Me.lblCaption.Text = "lblCaption"
        '
        'lblEmptyMenu
        '
        Me.lblEmptyMenu.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.lblEmptyMenu.AutoSize = True
        Me.lblEmptyMenu.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEmptyMenu.Location = New System.Drawing.Point(68, 203)
        Me.lblEmptyMenu.Name = "lblEmptyMenu"
        Me.lblEmptyMenu.Size = New System.Drawing.Size(249, 16)
        Me.lblEmptyMenu.TabIndex = 19
        Me.lblEmptyMenu.Text = "No Available Menu Options to Display."
        Me.lblEmptyMenu.Visible = False
        '
        'frmMenu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(384, 254)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblEmptyMenu)
        Me.Controls.Add(Me.grpbStatus)
        Me.Controls.Add(Me.lblTitle)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMenu"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.Text = "frmMenu"
        Me.TopMost = True
        Me.grpbStatus.ResumeLayout(False)
        Me.grpbStatus.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents grpbStatus As System.Windows.Forms.GroupBox
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents lblCaption As System.Windows.Forms.Label
    Friend WithEvents lblEmptyMenu As System.Windows.Forms.Label
    Friend WithEvents ToolTipMenu As System.Windows.Forms.ToolTip
End Class
