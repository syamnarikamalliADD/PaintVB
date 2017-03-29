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
        Me.tmrManRW = New System.Windows.Forms.Timer(Me.components)
        Me.AxOPCData0 = New AxOPCDATALib.AxOPCData
        CType(Me.AxOPCData0, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tmrHotLink
        '
        '
        'tmrManRW
        '
        Me.tmrManRW.Interval = 1000
        '
        'AxOPCData0
        '
        Me.AxOPCData0.Enabled = True
        Me.AxOPCData0.Location = New System.Drawing.Point(81, 52)
        Me.AxOPCData0.Name = "AxOPCData0"
        Me.AxOPCData0.OcxState = CType(resources.GetObject("AxOPCData0.OcxState"), System.Windows.Forms.AxHost.State)
        Me.AxOPCData0.Size = New System.Drawing.Size(25, 27)
        Me.AxOPCData0.TabIndex = 0
        '
        'clsPLCComm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(227, 166)
        Me.Controls.Add(Me.AxOPCData0)
        Me.Name = "clsPLCComm"
        Me.Text = "frmOmronOPCComm"
        CType(Me.AxOPCData0, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tmrHotLink As System.Windows.Forms.Timer
    Friend WithEvents tmrManRW As System.Windows.Forms.Timer
    Friend WithEvents AxOPCData0 As AxOPCDATALib.AxOPCData
End Class
