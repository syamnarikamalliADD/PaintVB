<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class clsPLCComm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(clsPLCComm))
        Me.AxActEasyIF0 = New AxACTMULTILib.AxActEasyIF
        Me.tmrHotLink = New System.Windows.Forms.Timer(Me.components)
        Me.tmrManRW = New System.Windows.Forms.Timer(Me.components)
        Me.tmrPoll = New System.Windows.Forms.Timer(Me.components)
        CType(Me.AxActEasyIF0, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'AxActEasyIF0
        '
        Me.AxActEasyIF0.Enabled = True
        Me.AxActEasyIF0.Location = New System.Drawing.Point(12, 12)
        Me.AxActEasyIF0.Name = "AxActEasyIF0"
        Me.AxActEasyIF0.OcxState = CType(resources.GetObject("AxActEasyIF0.OcxState"), System.Windows.Forms.AxHost.State)
        Me.AxActEasyIF0.Size = New System.Drawing.Size(32, 32)
        Me.AxActEasyIF0.TabIndex = 0
        '
        'tmrHotLink
        '
        Me.tmrHotLink.Interval = 50
        '
        'tmrManRW
        '
        Me.tmrManRW.Interval = 1000
        '
        'tmrPoll
        '
        '
        'clsPLCComm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(292, 72)
        Me.Controls.Add(Me.AxActEasyIF0)
        Me.Name = "clsPLCComm"
        Me.Text = "clsActEasyIFComm"
        CType(Me.AxActEasyIF0, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents AxActEasyIF0 As AxACTMULTILib.AxActEasyIF
    Friend WithEvents tmrHotLink As System.Windows.Forms.Timer
    Friend WithEvents tmrManRW As System.Windows.Forms.Timer
    Friend WithEvents tmrPoll As System.Windows.Forms.Timer
End Class
