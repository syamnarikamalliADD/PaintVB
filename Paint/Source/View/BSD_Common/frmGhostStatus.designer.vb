<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmGhostStatus
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmGhostStatus))
        Me.ToolStripContainer1 = New System.Windows.Forms.ToolStripContainer
        Me.tlpMain = New System.Windows.Forms.TableLayoutPanel
        Me.tlpButtons = New System.Windows.Forms.TableLayoutPanel
        Me.btnCancel = New System.Windows.Forms.Button
        Me.btnContinue = New System.Windows.Forms.Button
        Me.tlpBingoBoard = New System.Windows.Forms.TableLayoutPanel
        Me.bbGhostStatus = New BingoBoard.BingoBoard
        Me.ToolStripContainer1.ContentPanel.SuspendLayout()
        Me.ToolStripContainer1.SuspendLayout()
        Me.tlpMain.SuspendLayout()
        Me.tlpButtons.SuspendLayout()
        Me.tlpBingoBoard.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolStripContainer1
        '
        Me.ToolStripContainer1.BottomToolStripPanelVisible = False
        '
        'ToolStripContainer1.ContentPanel
        '
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.tlpMain)
        resources.ApplyResources(Me.ToolStripContainer1.ContentPanel, "ToolStripContainer1.ContentPanel")
        resources.ApplyResources(Me.ToolStripContainer1, "ToolStripContainer1")
        Me.ToolStripContainer1.LeftToolStripPanelVisible = False
        Me.ToolStripContainer1.Name = "ToolStripContainer1"
        Me.ToolStripContainer1.RightToolStripPanelVisible = False
        Me.ToolStripContainer1.TopToolStripPanelVisible = False
        '
        'tlpMain
        '
        resources.ApplyResources(Me.tlpMain, "tlpMain")
        Me.tlpMain.Controls.Add(Me.tlpButtons, 0, 1)
        Me.tlpMain.Controls.Add(Me.tlpBingoBoard, 0, 0)
        Me.tlpMain.Name = "tlpMain"
        '
        'tlpButtons
        '
        resources.ApplyResources(Me.tlpButtons, "tlpButtons")
        Me.tlpButtons.Controls.Add(Me.btnCancel, 1, 0)
        Me.tlpButtons.Controls.Add(Me.btnContinue, 0, 0)
        Me.tlpButtons.Name = "tlpButtons"
        '
        'btnCancel
        '
        resources.ApplyResources(Me.btnCancel, "btnCancel")
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnContinue
        '
        resources.ApplyResources(Me.btnContinue, "btnContinue")
        Me.btnContinue.Name = "btnContinue"
        Me.btnContinue.UseVisualStyleBackColor = True
        '
        'tlpBingoBoard
        '
        resources.ApplyResources(Me.tlpBingoBoard, "tlpBingoBoard")
        Me.tlpBingoBoard.Controls.Add(Me.bbGhostStatus, 1, 1)
        Me.tlpBingoBoard.Name = "tlpBingoBoard"
        '
        'bbGhostStatus
        '
        Me.bbGhostStatus.AutosizeColumns = True
        Me.bbGhostStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bbGhostStatus.Columns = 1
        Me.bbGhostStatus.ColumnWidth = 150
        resources.ApplyResources(Me.bbGhostStatus, "bbGhostStatus")
        Me.bbGhostStatus.EnableToolTips = False
        Me.bbGhostStatus.ItemBitIndex = New String() {"-1"}
        Me.bbGhostStatus.ItemCount = 1
        Me.bbGhostStatus.ItemData = New String() {"0"}
        Me.bbGhostStatus.ItemErrorColor = System.Drawing.Color.Silver
        Me.bbGhostStatus.ItemFont = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bbGhostStatus.ItemHorizSpace = 5
        Me.bbGhostStatus.ItemOffColor = System.Drawing.Color.Red
        Me.bbGhostStatus.ItemOffText = New String() {"Item 0 OFF"}
        Me.bbGhostStatus.ItemOnColor = System.Drawing.Color.Lime
        Me.bbGhostStatus.ItemOnText = New String() {"Item 0 ON"}
        Me.bbGhostStatus.ItemToolTipText = Nothing
        Me.bbGhostStatus.ItemVertSpace = 0
        Me.bbGhostStatus.Name = "bbGhostStatus"
        Me.bbGhostStatus.TitleBackColor = System.Drawing.Color.Black
        Me.bbGhostStatus.TitleColor = System.Drawing.Color.White
        Me.bbGhostStatus.TitleFont = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bbGhostStatus.TitleText = "Title"
        Me.bbGhostStatus.TitleVisible = True
        '
        'frmGhostStatus
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.ToolStripContainer1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmGhostStatus"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.TopMost = True
        Me.ToolStripContainer1.ContentPanel.ResumeLayout(False)
        Me.ToolStripContainer1.ResumeLayout(False)
        Me.ToolStripContainer1.PerformLayout()
        Me.tlpMain.ResumeLayout(False)
        Me.tlpButtons.ResumeLayout(False)
        Me.tlpBingoBoard.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ToolStripContainer1 As System.Windows.Forms.ToolStripContainer
    Friend WithEvents tlpMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents tlpButtons As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnContinue As System.Windows.Forms.Button
    Friend WithEvents tlpBingoBoard As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents bbGhostStatus As BingoBoard.BingoBoard
End Class
