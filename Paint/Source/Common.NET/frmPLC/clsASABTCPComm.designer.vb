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
        Me.tmrHotLink = New System.Windows.Forms.Timer(Me.components)
        Me.AxAsabtcp0 = New AxASABTCPLib.AxAsabtcp
        Me.tmrManRW = New System.Windows.Forms.Timer(Me.components)
        CType(Me.AxAsabtcp0, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tmrHotLink
        '
        '
        'AxAsabtcp0
        '
        Me.AxAsabtcp0.Enabled = True
        Me.AxAsabtcp0.Location = New System.Drawing.Point(12, 12)
        Me.AxAsabtcp0.Name = "AxAsabtcp0"
        Me.AxAsabtcp0.OcxState = CType(resources.GetObject("AxAsabtcp0.OcxState"), System.Windows.Forms.AxHost.State)
        Me.AxAsabtcp0.Size = New System.Drawing.Size(75, 77)
        Me.AxAsabtcp0.TabIndex = 0
        '
        'tmrManRW
        '
        Me.tmrManRW.Interval = 1000
        '
        'clsPLCComm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(292, 266)
        Me.Controls.Add(Me.AxAsabtcp0)
        Me.Name = "clsPLCComm"
        Me.Text = "frmASABTCPComm"
        CType(Me.AxAsabtcp0, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tmrHotLink As System.Windows.Forms.Timer
    Friend WithEvents AxAsabtcp0 As AxASABTCPLib.AxAsabtcp
    Friend WithEvents tmrManRW As System.Windows.Forms.Timer
End Class
