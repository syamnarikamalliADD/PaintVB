<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.btnOpen = New System.Windows.Forms.Button
        Me.dgRacks = New System.Windows.Forms.DataGridView
        Me.lblRacks = New System.Windows.Forms.Label
        Me.tlpMain = New System.Windows.Forms.TableLayoutPanel
        Me.btnSave = New System.Windows.Forms.Button
        Me.dgPoints = New System.Windows.Forms.DataGridView
        Me.lblPoints = New System.Windows.Forms.Label
        Me.dgSlots = New System.Windows.Forms.DataGridView
        Me.lblSlots = New System.Windows.Forms.Label
        Me.lblRobotIO = New System.Windows.Forms.Label
        Me.lblResX1 = New System.Windows.Forms.Label
        Me.lblResx2 = New System.Windows.Forms.Label
        CType(Me.dgRacks, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tlpMain.SuspendLayout()
        CType(Me.dgPoints, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgSlots, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnOpen
        '
        Me.btnOpen.Location = New System.Drawing.Point(3, 3)
        Me.btnOpen.Name = "btnOpen"
        Me.btnOpen.Size = New System.Drawing.Size(101, 19)
        Me.btnOpen.TabIndex = 0
        Me.btnOpen.Text = "Open"
        Me.btnOpen.UseVisualStyleBackColor = True
        '
        'dgRacks
        '
        Me.dgRacks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.tlpMain.SetColumnSpan(Me.dgRacks, 2)
        Me.dgRacks.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgRacks.Location = New System.Drawing.Point(3, 48)
        Me.dgRacks.Name = "dgRacks"
        Me.dgRacks.Size = New System.Drawing.Size(281, 73)
        Me.dgRacks.TabIndex = 1
        '
        'lblRacks
        '
        Me.lblRacks.AutoSize = True
        Me.tlpMain.SetColumnSpan(Me.lblRacks, 2)
        Me.lblRacks.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblRacks.Location = New System.Drawing.Point(3, 25)
        Me.lblRacks.Name = "lblRacks"
        Me.lblRacks.Size = New System.Drawing.Size(281, 20)
        Me.lblRacks.TabIndex = 2
        Me.lblRacks.Text = "Racks"
        '
        'tlpMain
        '
        Me.tlpMain.ColumnCount = 2
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpMain.Controls.Add(Me.btnSave, 1, 0)
        Me.tlpMain.Controls.Add(Me.dgPoints, 0, 6)
        Me.tlpMain.Controls.Add(Me.lblPoints, 0, 5)
        Me.tlpMain.Controls.Add(Me.dgSlots, 0, 4)
        Me.tlpMain.Controls.Add(Me.lblSlots, 0, 3)
        Me.tlpMain.Controls.Add(Me.btnOpen, 0, 0)
        Me.tlpMain.Controls.Add(Me.dgRacks, 0, 2)
        Me.tlpMain.Controls.Add(Me.lblRacks, 0, 1)
        Me.tlpMain.Location = New System.Drawing.Point(0, 0)
        Me.tlpMain.Name = "tlpMain"
        Me.tlpMain.RowCount = 7
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40.0!))
        Me.tlpMain.Size = New System.Drawing.Size(287, 481)
        Me.tlpMain.TabIndex = 3
        '
        'btnSave
        '
        Me.btnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSave.Location = New System.Drawing.Point(183, 3)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(101, 19)
        Me.btnSave.TabIndex = 8
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'dgPoints
        '
        Me.dgPoints.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.tlpMain.SetColumnSpan(Me.dgPoints, 2)
        Me.dgPoints.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgPoints.Location = New System.Drawing.Point(3, 325)
        Me.dgPoints.Name = "dgPoints"
        Me.dgPoints.Size = New System.Drawing.Size(281, 153)
        Me.dgPoints.TabIndex = 7
        '
        'lblPoints
        '
        Me.lblPoints.AutoSize = True
        Me.tlpMain.SetColumnSpan(Me.lblPoints, 2)
        Me.lblPoints.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblPoints.Location = New System.Drawing.Point(3, 302)
        Me.lblPoints.Name = "lblPoints"
        Me.lblPoints.Size = New System.Drawing.Size(281, 20)
        Me.lblPoints.TabIndex = 6
        Me.lblPoints.Text = "Points"
        '
        'dgSlots
        '
        Me.dgSlots.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.tlpMain.SetColumnSpan(Me.dgSlots, 2)
        Me.dgSlots.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgSlots.Location = New System.Drawing.Point(3, 147)
        Me.dgSlots.Name = "dgSlots"
        Me.dgSlots.Size = New System.Drawing.Size(281, 152)
        Me.dgSlots.TabIndex = 4
        '
        'lblSlots
        '
        Me.lblSlots.AutoSize = True
        Me.tlpMain.SetColumnSpan(Me.lblSlots, 2)
        Me.lblSlots.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblSlots.Location = New System.Drawing.Point(3, 124)
        Me.lblSlots.Name = "lblSlots"
        Me.lblSlots.Size = New System.Drawing.Size(281, 20)
        Me.lblSlots.TabIndex = 3
        Me.lblSlots.Text = "Slots"
        '
        'lblRobotIO
        '
        Me.lblRobotIO.AutoSize = True
        Me.lblRobotIO.Font = New System.Drawing.Font("Microsoft Sans Serif", 3.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblRobotIO.Location = New System.Drawing.Point(290, 3)
        Me.lblRobotIO.Name = "lblRobotIO"
        Me.lblRobotIO.Size = New System.Drawing.Size(145, 4480)
        Me.lblRobotIO.TabIndex = 4
        Me.lblRobotIO.Text = resources.GetString("lblRobotIO.Text")
        '
        'lblResX1
        '
        Me.lblResX1.AutoSize = True
        Me.lblResX1.Font = New System.Drawing.Font("Microsoft Sans Serif", 3.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblResX1.Location = New System.Drawing.Point(349, 0)
        Me.lblResX1.Name = "lblResX1"
        Me.lblResX1.Size = New System.Drawing.Size(372, 822)
        Me.lblResX1.TabIndex = 5
        Me.lblResX1.Text = resources.GetString("lblResX1.Text")
        '
        'lblResx2
        '
        Me.lblResx2.AutoSize = True
        Me.lblResx2.Font = New System.Drawing.Font("Microsoft Sans Serif", 3.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblResx2.Location = New System.Drawing.Point(690, 9)
        Me.lblResx2.Name = "lblResx2"
        Me.lblResx2.Size = New System.Drawing.Size(196, 16440)
        Me.lblResx2.TabIndex = 6
        Me.lblResx2.Text = resources.GetString("lblResx2.Text")
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(874, 482)
        Me.Controls.Add(Me.tlpMain)
        Me.Controls.Add(Me.lblResx2)
        Me.Controls.Add(Me.lblResX1)
        Me.Controls.Add(Me.lblRobotIO)
        Me.Name = "frmMain"
        Me.Text = "Logix Tag Converter"
        CType(Me.dgRacks, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tlpMain.ResumeLayout(False)
        Me.tlpMain.PerformLayout()
        CType(Me.dgPoints, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgSlots, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnOpen As System.Windows.Forms.Button
    Friend WithEvents dgRacks As System.Windows.Forms.DataGridView
    Friend WithEvents lblRacks As System.Windows.Forms.Label
    Friend WithEvents tlpMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents dgSlots As System.Windows.Forms.DataGridView
    Friend WithEvents lblSlots As System.Windows.Forms.Label
    Friend WithEvents dgPoints As System.Windows.Forms.DataGridView
    Friend WithEvents lblPoints As System.Windows.Forms.Label
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents lblRobotIO As System.Windows.Forms.Label
    Friend WithEvents lblResX1 As System.Windows.Forms.Label
    Friend WithEvents lblResx2 As System.Windows.Forms.Label

End Class
