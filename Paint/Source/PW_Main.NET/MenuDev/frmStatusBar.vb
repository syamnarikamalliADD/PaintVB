Public Class frmStatusBar
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents StatusBar1 As System.Windows.Forms.StatusBar
    Friend WithEvents RC1_Status As System.Windows.Forms.StatusBarPanel
    Friend WithEvents RC2_Status As System.Windows.Forms.StatusBarPanel
    Friend WithEvents RC3_Status As System.Windows.Forms.StatusBarPanel
    Friend WithEvents StatusBarPanel1 As System.Windows.Forms.StatusBarPanel
    Friend WithEvents StatusBarPanel2 As System.Windows.Forms.StatusBarPanel
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.StatusBar1 = New System.Windows.Forms.StatusBar
        Me.StatusBarPanel1 = New System.Windows.Forms.StatusBarPanel
        Me.StatusBarPanel2 = New System.Windows.Forms.StatusBarPanel
        Me.RC1_Status = New System.Windows.Forms.StatusBarPanel
        Me.RC2_Status = New System.Windows.Forms.StatusBarPanel
        Me.RC3_Status = New System.Windows.Forms.StatusBarPanel
        CType(Me.StatusBarPanel1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.StatusBarPanel2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RC1_Status, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RC2_Status, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RC3_Status, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'StatusBar1
        '
        Me.StatusBar1.Location = New System.Drawing.Point(0, 224)
        Me.StatusBar1.Name = "StatusBar1"
        Me.StatusBar1.Panels.AddRange(New System.Windows.Forms.StatusBarPanel() {Me.StatusBarPanel1, Me.StatusBarPanel2, Me.RC1_Status, Me.RC2_Status, Me.RC3_Status})
        Me.StatusBar1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.StatusBar1.ShowPanels = True
        Me.StatusBar1.Size = New System.Drawing.Size(684, 36)
        Me.StatusBar1.SizingGrip = False
        Me.StatusBar1.TabIndex = 0
        Me.StatusBar1.Text = "StatusBar1"
        '
        'StatusBarPanel1
        '
        Me.StatusBarPanel1.Text = "StatusBarPanel1"
        Me.StatusBarPanel1.Width = 300
        '
        'StatusBarPanel2
        '
        Me.StatusBarPanel2.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None
        '
        'RC1_Status
        '
        Me.RC1_Status.Alignment = System.Windows.Forms.HorizontalAlignment.Center
        Me.RC1_Status.Text = "RC1"
        Me.RC1_Status.Width = 30
        '
        'RC2_Status
        '
        Me.RC2_Status.Alignment = System.Windows.Forms.HorizontalAlignment.Center
        Me.RC2_Status.Text = "RC2"
        Me.RC2_Status.Width = 30
        '
        'RC3_Status
        '
        Me.RC3_Status.Alignment = System.Windows.Forms.HorizontalAlignment.Center
        Me.RC3_Status.Text = "RC3"
        Me.RC3_Status.Width = 30
        '
        'frmStatusBar
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(684, 260)
        Me.ControlBox = False
        Me.Controls.Add(Me.StatusBar1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmStatusBar"
        Me.Text = "frmStatusBar"
        Me.TopMost = True
        CType(Me.StatusBarPanel1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.StatusBarPanel2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RC1_Status, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RC2_Status, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RC3_Status, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private mnBottom As Integer

    Friend Property ScreenBottom() As Integer
        Get
            ScreenBottom = mnBottom
        End Get
        Set(ByVal Value As Integer)
            mnBottom = Value
        End Set
    End Property




    Private Sub frmStatusBar_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Height = StatusBar1.Height
        Me.Top = ScreenBottom - Me.Height
        StatusBar1.Panels(1).Width = StatusBar1.Width - (StatusBar1.Panels(0).Width + _
                                                         StatusBar1.Panels(2).Width + _
                                                         StatusBar1.Panels(3).Width + _
                                                         StatusBar1.Panels(4).Width)

    End Sub


End Class
