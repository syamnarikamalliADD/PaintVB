<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class uctrlValve
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(uctrlValve))
        Me.imlVlvHorzLt = New System.Windows.Forms.ImageList(Me.components)
        Me.imlVlvHorzRt = New System.Windows.Forms.ImageList(Me.components)
        Me.imlVlvVertRt = New System.Windows.Forms.ImageList(Me.components)
        Me.imlVlvVertLt = New System.Windows.Forms.ImageList(Me.components)
        Me.picValve = New System.Windows.Forms.PictureBox
        Me.imlGun = New System.Windows.Forms.ImageList(Me.components)
        Me.imlVlvVertRt2 = New System.Windows.Forms.ImageList(Me.components)
        Me.imlVlvVertLt2 = New System.Windows.Forms.ImageList(Me.components)
        Me.imlSpray = New System.Windows.Forms.ImageList(Me.components)
        Me.imlBell = New System.Windows.Forms.ImageList(Me.components)
        CType(Me.picValve, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'imlVlvHorzLt
        '
        Me.imlVlvHorzLt.ImageStream = CType(resources.GetObject("imlVlvHorzLt.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlVlvHorzLt.TransparentColor = System.Drawing.Color.Transparent
        Me.imlVlvHorzLt.Images.SetKeyName(0, "off")
        Me.imlVlvHorzLt.Images.SetKeyName(1, "on")
        Me.imlVlvHorzLt.Images.SetKeyName(2, "blue")
        Me.imlVlvHorzLt.Images.SetKeyName(3, "red")
        Me.imlVlvHorzLt.Images.SetKeyName(4, "purple")
        '
        'imlVlvHorzRt
        '
        Me.imlVlvHorzRt.ImageStream = CType(resources.GetObject("imlVlvHorzRt.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlVlvHorzRt.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.imlVlvHorzRt.Images.SetKeyName(0, "off")
        Me.imlVlvHorzRt.Images.SetKeyName(1, "on")
        Me.imlVlvHorzRt.Images.SetKeyName(2, "blue")
        Me.imlVlvHorzRt.Images.SetKeyName(3, "red")
        Me.imlVlvHorzRt.Images.SetKeyName(4, "purple")
        '
        'imlVlvVertRt
        '
        Me.imlVlvVertRt.ImageStream = CType(resources.GetObject("imlVlvVertRt.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlVlvVertRt.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.imlVlvVertRt.Images.SetKeyName(0, "off")
        Me.imlVlvVertRt.Images.SetKeyName(1, "on")
        Me.imlVlvVertRt.Images.SetKeyName(2, "blue")
        Me.imlVlvVertRt.Images.SetKeyName(3, "red")
        Me.imlVlvVertRt.Images.SetKeyName(4, "purple")
        '
        'imlVlvVertLt
        '
        Me.imlVlvVertLt.ImageStream = CType(resources.GetObject("imlVlvVertLt.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlVlvVertLt.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.imlVlvVertLt.Images.SetKeyName(0, "off")
        Me.imlVlvVertLt.Images.SetKeyName(1, "on")
        Me.imlVlvVertLt.Images.SetKeyName(2, "blue")
        Me.imlVlvVertLt.Images.SetKeyName(3, "red")
        Me.imlVlvVertLt.Images.SetKeyName(4, "purple")
        '
        'picValve
        '
        Me.picValve.Dock = System.Windows.Forms.DockStyle.Fill
        Me.picValve.Location = New System.Drawing.Point(0, 0)
        Me.picValve.Name = "picValve"
        Me.picValve.Size = New System.Drawing.Size(32, 32)
        Me.picValve.TabIndex = 0
        Me.picValve.TabStop = False
        '
        'imlGun
        '
        Me.imlGun.ImageStream = CType(resources.GetObject("imlGun.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlGun.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.imlGun.Images.SetKeyName(0, "off")
        Me.imlGun.Images.SetKeyName(1, "on")
        Me.imlGun.Images.SetKeyName(2, "blue")
        Me.imlGun.Images.SetKeyName(3, "red")
        Me.imlGun.Images.SetKeyName(4, "purple")
        Me.imlGun.Images.SetKeyName(5, "blue_off")
        Me.imlGun.Images.SetKeyName(6, "red_off")
        Me.imlGun.Images.SetKeyName(7, "purple_off")
        '
        'imlVlvVertRt2
        '
        Me.imlVlvVertRt2.ImageStream = CType(resources.GetObject("imlVlvVertRt2.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlVlvVertRt2.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.imlVlvVertRt2.Images.SetKeyName(0, "off")
        Me.imlVlvVertRt2.Images.SetKeyName(1, "on")
        Me.imlVlvVertRt2.Images.SetKeyName(2, "blue")
        Me.imlVlvVertRt2.Images.SetKeyName(3, "red")
        Me.imlVlvVertRt2.Images.SetKeyName(4, "purple")
        Me.imlVlvVertRt2.Images.SetKeyName(5, "blue_off")
        Me.imlVlvVertRt2.Images.SetKeyName(6, "red_off")
        Me.imlVlvVertRt2.Images.SetKeyName(7, "purple_off")
        '
        'imlVlvVertLt2
        '
        Me.imlVlvVertLt2.ImageStream = CType(resources.GetObject("imlVlvVertLt2.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlVlvVertLt2.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.imlVlvVertLt2.Images.SetKeyName(0, "off")
        Me.imlVlvVertLt2.Images.SetKeyName(1, "on")
        Me.imlVlvVertLt2.Images.SetKeyName(2, "blue")
        Me.imlVlvVertLt2.Images.SetKeyName(3, "red")
        Me.imlVlvVertLt2.Images.SetKeyName(4, "purple")
        Me.imlVlvVertLt2.Images.SetKeyName(5, "blue_off")
        Me.imlVlvVertLt2.Images.SetKeyName(6, "red_off")
        Me.imlVlvVertLt2.Images.SetKeyName(7, "purple_off")
        '
        'imlSpray
        '
        Me.imlSpray.ImageStream = CType(resources.GetObject("imlSpray.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlSpray.TransparentColor = System.Drawing.Color.Transparent
        Me.imlSpray.Images.SetKeyName(0, "off")
        Me.imlSpray.Images.SetKeyName(1, "on")
        Me.imlSpray.Images.SetKeyName(2, "blue")
        Me.imlSpray.Images.SetKeyName(3, "red")
        Me.imlSpray.Images.SetKeyName(4, "purple")
        '
        'imlBell
        '
        Me.imlBell.ImageStream = CType(resources.GetObject("imlBell.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imlBell.TransparentColor = System.Drawing.Color.Transparent
        Me.imlBell.Images.SetKeyName(0, "off")
        Me.imlBell.Images.SetKeyName(1, "on")
        Me.imlBell.Images.SetKeyName(2, "blue")
        Me.imlBell.Images.SetKeyName(3, "red")
        Me.imlBell.Images.SetKeyName(4, "purple")
        '
        'uctrlValve
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.picValve)
        Me.Name = "uctrlValve"
        Me.Size = New System.Drawing.Size(32, 32)
        CType(Me.picValve, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents imlVlvHorzLt As System.Windows.Forms.ImageList
    Friend WithEvents imlVlvHorzRt As System.Windows.Forms.ImageList
    Friend WithEvents imlVlvVertRt As System.Windows.Forms.ImageList
    Friend WithEvents imlVlvVertLt As System.Windows.Forms.ImageList
    Friend WithEvents picValve As System.Windows.Forms.PictureBox
    Friend WithEvents imlGun As System.Windows.Forms.ImageList
    Friend WithEvents imlVlvVertRt2 As System.Windows.Forms.ImageList
    Friend WithEvents imlVlvVertLt2 As System.Windows.Forms.ImageList
    Friend WithEvents imlSpray As System.Windows.Forms.ImageList
    Friend WithEvents imlBell As System.Windows.Forms.ImageList

End Class
