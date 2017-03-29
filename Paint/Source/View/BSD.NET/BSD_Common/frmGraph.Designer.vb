<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmGraph
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmGraph))
        Me.tlpMain = New System.Windows.Forms.TableLayoutPanel
        Me.tlpControls = New System.Windows.Forms.TableLayoutPanel
        Me.lblSecPerDiv = New System.Windows.Forms.Label
        Me.trkSecPerDiv = New System.Windows.Forms.TrackBar
        Me.btnPlay = New System.Windows.Forms.Button
        Me.tlpLegend = New System.Windows.Forms.TableLayoutPanel
        Me.lblLegend3 = New System.Windows.Forms.Label
        Me.lblLegend4 = New System.Windows.Forms.Label
        Me.lblLegend2 = New System.Windows.Forms.Label
        Me.lblLegend1 = New System.Windows.Forms.Label
        Me.lblLegend0 = New System.Windows.Forms.Label
        Me.lblYAxis1Bottom = New System.Windows.Forms.Label
        Me.lblYAxis1Top = New System.Windows.Forms.Label
        Me.lblYAxis0Bottom = New System.Windows.Forms.Label
        Me.lblYaxis0Top = New System.Windows.Forms.Label
        Me.pnlGraph = New System.Windows.Forms.Panel
        Me.tlpMain.SuspendLayout()
        Me.tlpControls.SuspendLayout()
        CType(Me.trkSecPerDiv, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tlpLegend.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpMain
        '
        Me.tlpMain.ColumnCount = 3
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75.0!))
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75.0!))
        Me.tlpMain.Controls.Add(Me.tlpControls, 1, 0)
        Me.tlpMain.Controls.Add(Me.tlpLegend, 0, 3)
        Me.tlpMain.Controls.Add(Me.lblYAxis1Bottom, 2, 2)
        Me.tlpMain.Controls.Add(Me.lblYAxis1Top, 2, 1)
        Me.tlpMain.Controls.Add(Me.lblYAxis0Bottom, 0, 2)
        Me.tlpMain.Controls.Add(Me.lblYaxis0Top, 0, 1)
        Me.tlpMain.Controls.Add(Me.pnlGraph, 1, 1)
        Me.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpMain.Location = New System.Drawing.Point(0, 0)
        Me.tlpMain.Name = "tlpMain"
        Me.tlpMain.RowCount = 4
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35.0!))
        Me.tlpMain.Size = New System.Drawing.Size(621, 354)
        Me.tlpMain.TabIndex = 7
        '
        'tlpControls
        '
        Me.tlpControls.ColumnCount = 3
        Me.tlpMain.SetColumnSpan(Me.tlpControls, 2)
        Me.tlpControls.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 63.0!))
        Me.tlpControls.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 127.0!))
        Me.tlpControls.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpControls.Controls.Add(Me.lblSecPerDiv, 0, 0)
        Me.tlpControls.Controls.Add(Me.trkSecPerDiv, 0, 0)
        Me.tlpControls.Controls.Add(Me.btnPlay, 0, 0)
        Me.tlpControls.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpControls.Location = New System.Drawing.Point(78, 3)
        Me.tlpControls.Name = "tlpControls"
        Me.tlpControls.RowCount = 1
        Me.tlpControls.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpControls.Size = New System.Drawing.Size(540, 35)
        Me.tlpControls.TabIndex = 12
        '
        'lblSecPerDiv
        '
        Me.lblSecPerDiv.AutoSize = True
        Me.lblSecPerDiv.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblSecPerDiv.Location = New System.Drawing.Point(193, 0)
        Me.lblSecPerDiv.Name = "lblSecPerDiv"
        Me.lblSecPerDiv.Size = New System.Drawing.Size(344, 35)
        Me.lblSecPerDiv.TabIndex = 5
        Me.lblSecPerDiv.Text = "lblSecPerDiv"
        Me.lblSecPerDiv.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'trkSecPerDiv
        '
        Me.trkSecPerDiv.Dock = System.Windows.Forms.DockStyle.Fill
        Me.trkSecPerDiv.Location = New System.Drawing.Point(66, 3)
        Me.trkSecPerDiv.Minimum = 1
        Me.trkSecPerDiv.Name = "trkSecPerDiv"
        Me.trkSecPerDiv.Size = New System.Drawing.Size(121, 29)
        Me.trkSecPerDiv.TabIndex = 4
        Me.trkSecPerDiv.Value = 1
        '
        'btnPlay
        '
        Me.btnPlay.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnPlay.Location = New System.Drawing.Point(3, 3)
        Me.btnPlay.Name = "btnPlay"
        Me.btnPlay.Size = New System.Drawing.Size(57, 29)
        Me.btnPlay.TabIndex = 3
        Me.btnPlay.Text = "btnPlay"
        Me.btnPlay.UseVisualStyleBackColor = True
        '
        'tlpLegend
        '
        Me.tlpLegend.ColumnCount = 5
        Me.tlpMain.SetColumnSpan(Me.tlpLegend, 3)
        Me.tlpLegend.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.tlpLegend.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.tlpLegend.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.tlpLegend.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.tlpLegend.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.tlpLegend.Controls.Add(Me.lblLegend3, 0, 0)
        Me.tlpLegend.Controls.Add(Me.lblLegend4, 0, 0)
        Me.tlpLegend.Controls.Add(Me.lblLegend2, 0, 0)
        Me.tlpLegend.Controls.Add(Me.lblLegend1, 0, 0)
        Me.tlpLegend.Controls.Add(Me.lblLegend0, 0, 0)
        Me.tlpLegend.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpLegend.Location = New System.Drawing.Point(3, 322)
        Me.tlpLegend.Name = "tlpLegend"
        Me.tlpLegend.RowCount = 1
        Me.tlpLegend.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpLegend.Size = New System.Drawing.Size(615, 29)
        Me.tlpLegend.TabIndex = 11
        '
        'lblLegend3
        '
        Me.lblLegend3.AutoSize = True
        Me.lblLegend3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblLegend3.Location = New System.Drawing.Point(372, 0)
        Me.lblLegend3.Name = "lblLegend3"
        Me.lblLegend3.Size = New System.Drawing.Size(117, 29)
        Me.lblLegend3.TabIndex = 9
        Me.lblLegend3.Text = "lblLegend4"
        Me.lblLegend3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblLegend3.Visible = False
        '
        'lblLegend4
        '
        Me.lblLegend4.AutoSize = True
        Me.lblLegend4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblLegend4.Location = New System.Drawing.Point(495, 0)
        Me.lblLegend4.Name = "lblLegend4"
        Me.lblLegend4.Size = New System.Drawing.Size(117, 29)
        Me.lblLegend4.TabIndex = 8
        Me.lblLegend4.Text = "lblLegend4"
        Me.lblLegend4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblLegend4.Visible = False
        '
        'lblLegend2
        '
        Me.lblLegend2.AutoSize = True
        Me.lblLegend2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblLegend2.Location = New System.Drawing.Point(249, 0)
        Me.lblLegend2.Name = "lblLegend2"
        Me.lblLegend2.Size = New System.Drawing.Size(117, 29)
        Me.lblLegend2.TabIndex = 7
        Me.lblLegend2.Text = "lblLegend2"
        Me.lblLegend2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblLegend2.Visible = False
        '
        'lblLegend1
        '
        Me.lblLegend1.AutoSize = True
        Me.lblLegend1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblLegend1.Location = New System.Drawing.Point(3, 0)
        Me.lblLegend1.Name = "lblLegend1"
        Me.lblLegend1.Size = New System.Drawing.Size(117, 29)
        Me.lblLegend1.TabIndex = 6
        Me.lblLegend1.Text = "lblLegend1"
        Me.lblLegend1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblLegend1.Visible = False
        '
        'lblLegend0
        '
        Me.lblLegend0.AutoSize = True
        Me.lblLegend0.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblLegend0.Location = New System.Drawing.Point(126, 0)
        Me.lblLegend0.Name = "lblLegend0"
        Me.lblLegend0.Size = New System.Drawing.Size(117, 29)
        Me.lblLegend0.TabIndex = 5
        Me.lblLegend0.Text = "lblLegend0"
        Me.lblLegend0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblLegend0.Visible = False
        '
        'lblYAxis1Bottom
        '
        Me.lblYAxis1Bottom.AutoSize = True
        Me.lblYAxis1Bottom.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblYAxis1Bottom.Location = New System.Drawing.Point(549, 180)
        Me.lblYAxis1Bottom.Name = "lblYAxis1Bottom"
        Me.lblYAxis1Bottom.Size = New System.Drawing.Size(69, 139)
        Me.lblYAxis1Bottom.TabIndex = 10
        Me.lblYAxis1Bottom.Text = "lblYAxis1 Bottom"
        Me.lblYAxis1Bottom.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'lblYAxis1Top
        '
        Me.lblYAxis1Top.AutoSize = True
        Me.lblYAxis1Top.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblYAxis1Top.Location = New System.Drawing.Point(549, 41)
        Me.lblYAxis1Top.Name = "lblYAxis1Top"
        Me.lblYAxis1Top.Size = New System.Drawing.Size(69, 139)
        Me.lblYAxis1Top.TabIndex = 9
        Me.lblYAxis1Top.Text = "lblYAxis1Top"
        '
        'lblYAxis0Bottom
        '
        Me.lblYAxis0Bottom.AutoSize = True
        Me.lblYAxis0Bottom.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblYAxis0Bottom.Location = New System.Drawing.Point(3, 180)
        Me.lblYAxis0Bottom.Name = "lblYAxis0Bottom"
        Me.lblYAxis0Bottom.Size = New System.Drawing.Size(69, 139)
        Me.lblYAxis0Bottom.TabIndex = 8
        Me.lblYAxis0Bottom.Text = "lblYAxis0 Bottom"
        Me.lblYAxis0Bottom.TextAlign = System.Drawing.ContentAlignment.BottomRight
        '
        'lblYaxis0Top
        '
        Me.lblYaxis0Top.AutoSize = True
        Me.lblYaxis0Top.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblYaxis0Top.Location = New System.Drawing.Point(3, 41)
        Me.lblYaxis0Top.Name = "lblYaxis0Top"
        Me.lblYaxis0Top.Size = New System.Drawing.Size(69, 139)
        Me.lblYaxis0Top.TabIndex = 7
        Me.lblYaxis0Top.Text = "lblYaxis0Top"
        Me.lblYaxis0Top.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'pnlGraph
        '
        Me.pnlGraph.BackColor = System.Drawing.Color.Black
        Me.pnlGraph.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlGraph.Location = New System.Drawing.Point(78, 44)
        Me.pnlGraph.Name = "pnlGraph"
        Me.tlpMain.SetRowSpan(Me.pnlGraph, 2)
        Me.pnlGraph.Size = New System.Drawing.Size(465, 272)
        Me.pnlGraph.TabIndex = 1
        '
        'frmGraph
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(621, 354)
        Me.Controls.Add(Me.tlpMain)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmGraph"
        Me.Text = "frmGraph"
        Me.tlpMain.ResumeLayout(False)
        Me.tlpMain.PerformLayout()
        Me.tlpControls.ResumeLayout(False)
        Me.tlpControls.PerformLayout()
        CType(Me.trkSecPerDiv, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tlpLegend.ResumeLayout(False)
        Me.tlpLegend.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tlpMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents pnlGraph As System.Windows.Forms.Panel
    Friend WithEvents lblYAxis0Bottom As System.Windows.Forms.Label
    Friend WithEvents lblYaxis0Top As System.Windows.Forms.Label
    Friend WithEvents tlpLegend As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblLegend3 As System.Windows.Forms.Label
    Friend WithEvents lblLegend4 As System.Windows.Forms.Label
    Friend WithEvents lblLegend2 As System.Windows.Forms.Label
    Friend WithEvents lblLegend1 As System.Windows.Forms.Label
    Friend WithEvents lblLegend0 As System.Windows.Forms.Label
    Friend WithEvents lblYAxis1Bottom As System.Windows.Forms.Label
    Friend WithEvents lblYAxis1Top As System.Windows.Forms.Label
    Friend WithEvents tlpControls As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblSecPerDiv As System.Windows.Forms.Label
    Friend WithEvents trkSecPerDiv As System.Windows.Forms.TrackBar
    Friend WithEvents btnPlay As System.Windows.Forms.Button
End Class
