<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmWatch
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmWatch))
        Me.BBWatch = New BingoBoard.BingoBoard
        Me.tmrToolTip = New System.Windows.Forms.Timer(Me.components)
        Me.lblToolTip = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'BBWatch
        '
        Me.BBWatch.AutosizeColumns = True
        Me.BBWatch.Columns = 1
        Me.BBWatch.ColumnWidth = 150
        Me.BBWatch.EnableToolTips = False
        Me.BBWatch.ItemBitIndex = New String() {"-1"}
        Me.BBWatch.ItemCount = 1
        Me.BBWatch.ItemData = New String() {"0"}
        Me.BBWatch.ItemErrorColor = System.Drawing.Color.Silver
        Me.BBWatch.ItemFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BBWatch.ItemHorizSpace = 5
        Me.BBWatch.ItemOffColor = System.Drawing.Color.Red
        Me.BBWatch.ItemOffText = New String() {"Item 0 OFF"}
        Me.BBWatch.ItemOnColor = System.Drawing.Color.Lime
        Me.BBWatch.ItemOnText = New String() {"Item 0 ON"}
        Me.BBWatch.ItemToolTipText = Nothing
        Me.BBWatch.ItemVertSpace = 0
        resources.ApplyResources(Me.BBWatch, "BBWatch")
        Me.BBWatch.Name = "BBWatch"
        Me.BBWatch.TitleBackColor = System.Drawing.Color.Black
        Me.BBWatch.TitleColor = System.Drawing.Color.White
        Me.BBWatch.TitleFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BBWatch.TitleText = "Title"
        Me.BBWatch.TitleVisible = False
        '
        'tmrToolTip
        '
        Me.tmrToolTip.Interval = 3000
        '
        'lblToolTip
        '
        resources.ApplyResources(Me.lblToolTip, "lblToolTip")
        Me.lblToolTip.BackColor = System.Drawing.SystemColors.Info
        Me.lblToolTip.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblToolTip.Name = "lblToolTip"
        '
        'frmWatch
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.Controls.Add(Me.lblToolTip)
        Me.Controls.Add(Me.BBWatch)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmWatch"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BBWatch As BingoBoard.BingoBoard
    Friend WithEvents tmrToolTip As System.Windows.Forms.Timer
    Friend WithEvents lblToolTip As System.Windows.Forms.Label
End Class
