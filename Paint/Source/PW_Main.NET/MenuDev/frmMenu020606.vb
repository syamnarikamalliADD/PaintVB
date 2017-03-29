Public Class frmMenu
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
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents lblCaption As System.Windows.Forms.Label
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents lblEmptyMenu As System.Windows.Forms.Label
    Friend WithEvents Button13 As System.Windows.Forms.Button
    Friend WithEvents Button12 As System.Windows.Forms.Button
    Friend WithEvents Button11 As System.Windows.Forms.Button
    Friend WithEvents Button10 As System.Windows.Forms.Button
    Friend WithEvents Button9 As System.Windows.Forms.Button
    Friend WithEvents Button8 As System.Windows.Forms.Button
    Friend WithEvents Button7 As System.Windows.Forms.Button
    Friend WithEvents Button6 As System.Windows.Forms.Button
    Friend WithEvents Button5 As System.Windows.Forms.Button
    Friend WithEvents Button4 As System.Windows.Forms.Button
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents lblMenuTitle As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.btnCancel = New System.Windows.Forms.Button
        Me.lblCaption = New System.Windows.Forms.Label
        Me.lblEmptyMenu = New System.Windows.Forms.Label
        Me.Button13 = New System.Windows.Forms.Button
        Me.Button12 = New System.Windows.Forms.Button
        Me.Button11 = New System.Windows.Forms.Button
        Me.Button10 = New System.Windows.Forms.Button
        Me.Button9 = New System.Windows.Forms.Button
        Me.Button8 = New System.Windows.Forms.Button
        Me.Button7 = New System.Windows.Forms.Button
        Me.Button6 = New System.Windows.Forms.Button
        Me.Button5 = New System.Windows.Forms.Button
        Me.Button4 = New System.Windows.Forms.Button
        Me.Button3 = New System.Windows.Forms.Button
        Me.Button2 = New System.Windows.Forms.Button
        Me.Button1 = New System.Windows.Forms.Button
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.lblMenuTitle = New System.Windows.Forms.Label
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.btnCancel)
        Me.GroupBox1.Controls.Add(Me.lblCaption)
        Me.GroupBox1.Location = New System.Drawing.Point(0, 223)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(384, 32)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(284, 8)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(96, 20)
        Me.btnCancel.TabIndex = 3
        Me.btnCancel.Text = "Cancel"
        '
        'lblCaption
        '
        Me.lblCaption.AutoSize = True
        Me.lblCaption.Location = New System.Drawing.Point(4, 12)
        Me.lblCaption.Name = "lblCaption"
        Me.lblCaption.Size = New System.Drawing.Size(55, 16)
        Me.lblCaption.TabIndex = 2
        Me.lblCaption.Text = "lblCaption"
        '
        'lblEmptyMenu
        '
        Me.lblEmptyMenu.AutoSize = True
        Me.lblEmptyMenu.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEmptyMenu.Location = New System.Drawing.Point(48, 204)
        Me.lblEmptyMenu.Name = "lblEmptyMenu"
        Me.lblEmptyMenu.Size = New System.Drawing.Size(250, 18)
        Me.lblEmptyMenu.TabIndex = 18
        Me.lblEmptyMenu.Text = "No Available Menu Options to Display."
        Me.lblEmptyMenu.Visible = False
        '
        'Button13
        '
        Me.Button13.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button13.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.Button13.Location = New System.Drawing.Point(284, 139)
        Me.Button13.Name = "Button13"
        Me.Button13.Size = New System.Drawing.Size(80, 56)
        Me.Button13.TabIndex = 31
        Me.Button13.Text = "Button13"
        Me.Button13.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.Button13.Visible = False
        '
        'Button12
        '
        Me.Button12.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button12.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.Button12.Location = New System.Drawing.Point(284, 71)
        Me.Button12.Name = "Button12"
        Me.Button12.Size = New System.Drawing.Size(80, 56)
        Me.Button12.TabIndex = 30
        Me.Button12.Text = "Button12"
        Me.Button12.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.Button12.Visible = False
        '
        'Button11
        '
        Me.Button11.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button11.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.Button11.Location = New System.Drawing.Point(276, 63)
        Me.Button11.Name = "Button11"
        Me.Button11.Size = New System.Drawing.Size(80, 56)
        Me.Button11.TabIndex = 29
        Me.Button11.Text = "Button11"
        Me.Button11.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.Button11.Visible = False
        '
        'Button10
        '
        Me.Button10.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button10.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.Button10.Location = New System.Drawing.Point(268, 55)
        Me.Button10.Name = "Button10"
        Me.Button10.Size = New System.Drawing.Size(80, 56)
        Me.Button10.TabIndex = 28
        Me.Button10.Text = "Button10"
        Me.Button10.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.Button10.Visible = False
        '
        'Button9
        '
        Me.Button9.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button9.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.Button9.Location = New System.Drawing.Point(260, 47)
        Me.Button9.Name = "Button9"
        Me.Button9.Size = New System.Drawing.Size(80, 56)
        Me.Button9.TabIndex = 27
        Me.Button9.Text = "Button9"
        Me.Button9.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.Button9.Visible = False
        '
        'Button8
        '
        Me.Button8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button8.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.Button8.Location = New System.Drawing.Point(160, 71)
        Me.Button8.Name = "Button8"
        Me.Button8.Size = New System.Drawing.Size(80, 56)
        Me.Button8.TabIndex = 26
        Me.Button8.Text = "Button8"
        Me.Button8.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.Button8.Visible = False
        '
        'Button7
        '
        Me.Button7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button7.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.Button7.Location = New System.Drawing.Point(152, 63)
        Me.Button7.Name = "Button7"
        Me.Button7.Size = New System.Drawing.Size(80, 56)
        Me.Button7.TabIndex = 25
        Me.Button7.Text = "Button7"
        Me.Button7.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.Button7.Visible = False
        '
        'Button6
        '
        Me.Button6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button6.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.Button6.Location = New System.Drawing.Point(144, 55)
        Me.Button6.Name = "Button6"
        Me.Button6.Size = New System.Drawing.Size(80, 56)
        Me.Button6.TabIndex = 24
        Me.Button6.Text = "Button6"
        Me.Button6.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.Button6.Visible = False
        '
        'Button5
        '
        Me.Button5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button5.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.Button5.Location = New System.Drawing.Point(136, 47)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(80, 56)
        Me.Button5.TabIndex = 23
        Me.Button5.Text = "Button5"
        Me.Button5.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.Button5.Visible = False
        '
        'Button4
        '
        Me.Button4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button4.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.Button4.Location = New System.Drawing.Point(36, 71)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(80, 56)
        Me.Button4.TabIndex = 22
        Me.Button4.Text = "Button4"
        Me.Button4.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.Button4.Visible = False
        '
        'Button3
        '
        Me.Button3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button3.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.Button3.Location = New System.Drawing.Point(24, 63)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(80, 56)
        Me.Button3.TabIndex = 21
        Me.Button3.Text = "Button3"
        Me.Button3.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.Button3.Visible = False
        '
        'Button2
        '
        Me.Button2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button2.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.Button2.Location = New System.Drawing.Point(12, 55)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(80, 56)
        Me.Button2.TabIndex = 20
        Me.Button2.Text = "Button2"
        Me.Button2.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.Button2.Visible = False
        '
        'Button1
        '
        Me.Button1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.Button1.Location = New System.Drawing.Point(4, 47)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(80, 56)
        Me.Button1.TabIndex = 19
        Me.Button1.Text = "Button1"
        Me.Button1.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.Button1.Visible = False
        '
        'lblMenuTitle
        '
        Me.lblMenuTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblMenuTitle.BackColor = System.Drawing.Color.FromArgb(CType(0, Byte), CType(0, Byte), CType(192, Byte))
        Me.lblMenuTitle.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMenuTitle.ForeColor = System.Drawing.SystemColors.ControlLightLight
        Me.lblMenuTitle.Location = New System.Drawing.Point(0, 0)
        Me.lblMenuTitle.Name = "lblMenuTitle"
        Me.lblMenuTitle.Size = New System.Drawing.Size(384, 20)
        Me.lblMenuTitle.TabIndex = 32
        Me.lblMenuTitle.Text = "Label1"
        Me.lblMenuTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'frmMenu
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(384, 254)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblMenuTitle)
        Me.Controls.Add(Me.Button13)
        Me.Controls.Add(Me.Button12)
        Me.Controls.Add(Me.Button11)
        Me.Controls.Add(Me.Button10)
        Me.Controls.Add(Me.Button9)
        Me.Controls.Add(Me.Button8)
        Me.Controls.Add(Me.Button7)
        Me.Controls.Add(Me.Button6)
        Me.Controls.Add(Me.Button5)
        Me.Controls.Add(Me.Button4)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.lblEmptyMenu)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMenu"
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "frmMenu"
        Me.TopMost = True
        Me.GroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Enum enumMenuProps
        Columns = 0
        ButtonHeight = 1
        ButtonWidth = 2
        MinWidth = 3
        TopMargin = 4
        BottomMargin = 5
        SideMargin = 6
        HorizontalSpacing = 7
        VerticalSpacing = 8
        TitleTag = 9
        CaptionTag = 10
        CancelCaptionTag = 11
        CancelToolTipTag = 12
        MainScreenID = 13
    End Enum

    Private Enum enumButtonProps
        CaptionTag = 0      'Text displayed in menu status field when button is moused over
        DescriptionTag = 1  'Text on button
        ToolTipTag = 2      'Button ToolTip
        Icon = 3
        Action = 4
        SubScreenID = 5
        LaunchFile = 6
        UseFRWM = 7
        LaunchFlags = 8
        WindowTitle = 9
        CommandLine = 10
        nOptionSpecific = 11
        nPrivilegeID = 12
        Name = 13
    End Enum

    Private mcolButtons As New Collection
    Private mnMenuTop As Integer
    Private mnMenuLeft As Integer
    Private msMenuProps() As String
    Private msButtonProps() As String
    Private msMenuScreenID As String
    Private rmMenuIcon As Resources.ResourceManager = Resources.ResourceManager.CreateFileBasedResourceManager("MenuIcon", "D:\Temp\test\MenuDev", Nothing)
    Private mdsMenuData As New DataSet

    Friend Property MenuData() As DataSet
        Get
            MenuData = mdsMenuData
        End Get
        Set(ByVal Value As DataSet)
            mdsMenuData = Value
        End Set
    End Property

    Friend Property MenuLeft() As Integer
        Get
            MenuLeft = mnMenuLeft
        End Get
        Set(ByVal Value As Integer)
            mnMenuLeft = Value
        End Set
    End Property

    Friend Property MenuScreenID() As String
        Get
            MenuScreenID = msMenuScreenID
        End Get
        Set(ByVal Value As String)
            msMenuScreenID = Value
        End Set
    End Property

    Friend Property MenuTop() As Integer
        Get
            MenuTop = mnMenuTop
        End Get
        Set(ByVal Value As Integer)
            mnMenuTop = Value
        End Set
    End Property

    Private Sub Button_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                            Handles Button1.Click, Button2.Click, Button3.Click, _
                            Button4.Click, Button5.Click, Button6.Click, _
                            Button7.Click, Button8.Click, Button9.Click, _
                            Button10.Click, Button11.Click, Button12.Click, _
                            Button13.Click
        Dim objThisButton As New System.Windows.Forms.Button
        Dim clsThisButton As New SubMenuButton

        objThisButton = sender
        clsThisButton = mcolButtons(objThisButton.Name)
        clsThisButton.Execute()

    End Sub

    Private Sub Button_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) _
                                  Handles Button1.MouseEnter, Button2.MouseEnter, _
                                  Button3.MouseEnter, Button4.MouseEnter, Button5.MouseEnter, _
                                  Button6.MouseEnter, Button7.MouseEnter, Button8.MouseEnter, _
                                  Button9.MouseEnter, Button10.MouseEnter, Button11.MouseEnter, _
                                  Button12.MouseEnter, Button13.MouseEnter

        Dim objThisButton As New System.Windows.Forms.Button
        Dim clsThisButton As New SubMenuButton

        objThisButton = sender
        clsThisButton = mcolButtons(objThisButton.Name)
        lblCaption.Text = clsThisButton.Caption

        clsThisButton = Nothing
        objThisButton = Nothing

    End Sub

    Private Sub Button_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) _
                              Handles Button1.MouseLeave, Button2.MouseLeave, _
                              Button3.MouseLeave, Button4.MouseLeave, Button5.MouseLeave, _
                              Button6.MouseLeave, Button7.MouseLeave, Button8.MouseLeave, _
                              Button9.MouseLeave, Button10.MouseLeave, Button11.MouseLeave, _
                              Button12.MouseLeave, Button13.MouseLeave, btnCancel.MouseLeave

        lblCaption.Text = msMenuProps(enumMenuProps.CaptionTag)

    End Sub

    Private Sub btnCancel_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.MouseEnter

        lblCaption.Text = msMenuProps(enumMenuProps.CancelToolTipTag)

    End Sub

    Private Sub frmMenu_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.Visible = False
        'Set up submenu button ToolTips
        With ToolTip1
            ' Set up the delays for the ToolTip.
            .AutoPopDelay = 5000
            .InitialDelay = 500
            .ReshowDelay = 500
            ' Force the ToolTip text to be displayed whether or not the form is active.
            .ShowAlways = True
        End With 'ToolTip1
        Call subShowMenu()
        Me.Left = MenuLeft
        Me.Top = MenuTop
        Me.Visible = True

    End Sub

    Private Sub subShowMenu()
        Dim dtMenuData As New DataTable
        Dim myRelation As DataRelation
        Dim drButtons() As DataRow
        Dim drThisButton As DataRow
        Dim drThisMenu As DataRow
        Dim nRow As Integer
        Dim nProp As Integer
        Dim nItems As Integer
        Dim sButtonProps() As String

        'Start with all buttons invisible
        Call subHideButtons()

        'Empty the button class Collection
        While mcolButtons.Count > 0
            mcolButtons.Remove(1)
        End While

        'Build the menu.
        'Start by getting the Menu Panel Properties
        With MenuData
            'Read the menu panel configuration for the selected menu
            dtMenuData = .Tables("MainMenuData")
            For Each drThisMenu In dtMenuData.Rows
                If drThisMenu.Item("MainScreenID") = MenuScreenID Then Exit For
            Next

            nItems = UBound(drThisMenu.ItemArray)
            ReDim msMenuProps(nItems)

            msMenuProps(enumMenuProps.Columns) = drThisMenu.Item("Columns")
            msMenuProps(enumMenuProps.ButtonHeight) = drThisMenu.Item("ButtonHeight")
            msMenuProps(enumMenuProps.ButtonWidth) = drThisMenu.Item("ButtonWidth")
            msMenuProps(enumMenuProps.MinWidth) = drThisMenu.Item("MinWidth")
            msMenuProps(enumMenuProps.TopMargin) = drThisMenu.Item("TopMargin")
            msMenuProps(enumMenuProps.BottomMargin) = drThisMenu.Item("BottomMargin")
            msMenuProps(enumMenuProps.SideMargin) = drThisMenu.Item("SideMargin")
            msMenuProps(enumMenuProps.HorizontalSpacing) = drThisMenu.Item("HorizontalSpacing")
            msMenuProps(enumMenuProps.VerticalSpacing) = drThisMenu.Item("VerticalSpacing")
            msMenuProps(enumMenuProps.TitleTag) = drThisMenu.Item("TitleTag")
            msMenuProps(enumMenuProps.CaptionTag) = drThisMenu.Item("CaptionTag")
            msMenuProps(enumMenuProps.CancelCaptionTag) = drThisMenu.Item("CancelCaptionTag")
            msMenuProps(enumMenuProps.CancelToolTipTag) = drThisMenu.Item("CancelToolTipTag")
            msMenuProps(enumMenuProps.MainScreenID) = drThisMenu.Item("MainScreenID")
        End With

        'Now configure the submenu buttons and position them on the menu panel.
        'First, get the data rows for the buttons that belong to this menu.
        myRelation = dtMenuData.ChildRelations(0)
        drButtons = drThisMenu.GetChildRows(myRelation)
        For Each drThisButton In drButtons
            'Get the properties for this button
            nItems = UBound(drThisButton.ItemArray)
            ReDim msButtonProps(nItems)
            msButtonProps(enumButtonProps.CaptionTag) = drThisButton.Item("CaptionTag")
            msButtonProps(enumButtonProps.DescriptionTag) = drThisButton.Item("DescriptionTag")
            msButtonProps(enumButtonProps.ToolTipTag) = drThisButton.Item("ToolTipTag")
            msButtonProps(enumButtonProps.Icon) = drThisButton.Item("Icon")
            msButtonProps(enumButtonProps.Action) = drThisButton.Item("Action")
            msButtonProps(enumButtonProps.SubScreenID) = drThisButton.Item("SubScreenID")
            msButtonProps(enumButtonProps.LaunchFile) = drThisButton.Item("LaunchFile")
            msButtonProps(enumButtonProps.UseFRWM) = drThisButton.Item("UseFRWM")
            msButtonProps(enumButtonProps.LaunchFlags) = drThisButton.Item("LaunchFlags")
            msButtonProps(enumButtonProps.WindowTitle) = drThisButton.Item("WindowTitle")
            msButtonProps(enumButtonProps.CommandLine) = drThisButton.Item("CommandLine")
            msButtonProps(enumButtonProps.nOptionSpecific) = drThisButton.Item("nOptionSpecific")
            msButtonProps(enumButtonProps.nPrivilegeID) = drThisButton.Item("nPrivilegeID")
            msButtonProps(enumButtonProps.Name) = drThisButton.Item("Name")
            'Add this button to the menu
            If bButtonAvailable() Then
                Call subAddSubMenuButton(CType(Mid(msButtonProps(enumButtonProps.SubScreenID), 2, 2), Integer))
            End If
        Next

        'Configure the panel
        If mcolButtons.Count > 0 Then
            Call subFormatMenuPanel()
        Else
            With lblEmptyMenu
                .Text = rmMenuIcon.GetString("NoMenuItems")
                .Left = (Me.Width - lblEmptyMenu.Width) / 2
                .Top = (Me.Height - lblEmptyMenu.Height) / 2
                .Visible = True
            End With 'lblEmptyMenu
        End If

        'Clean up
        dtMenuData = Nothing
        drButtons = Nothing
        myRelation = Nothing
        drThisMenu = Nothing
        drThisButton = Nothing

    End Sub

    Private Sub subAddSubMenuButton(ByVal Button As Integer)
        Dim objThisButton As New System.Windows.Forms.Button
        Dim objThisControl As New System.Windows.Forms.Control
        Dim clsThisButton As New SubMenuButton
        Dim nButtonX As Integer
        Dim nButtonY As Integer

        'Find the button
        For Each objThisControl In Me.Controls
            If objThisControl.Name = "Button" & CType(Button, String) Then
                objThisButton = objThisControl
                Exit For
            End If
        Next

        'Set the Button Control properties
        With objThisButton
            '.Text = rmMenuText.GetString(msButtonProps(enumButtonProps.DescriptionTag))
            .Text = msButtonProps(enumButtonProps.DescriptionTag)
            '.Image = Image.FromFile(msButtonProps(enumButtonProps.Icon))
            .Image = rmMenuIcon.GetObject(msButtonProps(enumButtonProps.Icon))
            .Tag = objThisButton.Name
            'TODO - Look at autosizing buttons based on description.
            '-could keep track of the widest button here by determining 
            'the width of the Text 
            .Height = CType(msMenuProps(enumMenuProps.ButtonHeight), Integer)
            .Width = CType(msMenuProps(enumMenuProps.ButtonWidth), Integer)
            .Visible = True
        End With
        'ToolTip1.SetToolTip(objThisButton, rmMenuText.GetString(msButtonProps(enumButtonProps.ToolTipTag)))
        ToolTip1.SetToolTip(objThisButton, msButtonProps(enumButtonProps.ToolTipTag))

        'Set the Button Class properties
        With clsThisButton
            .Action = msButtonProps(enumButtonProps.Action)
            '.Caption = rmMenuText.GetString(msButtonProps(enumButtonProps.CaptionTag))
            .Caption = msButtonProps(enumButtonProps.CaptionTag)
            .CommandLine = msButtonProps(enumButtonProps.CommandLine)
            .LaunchFile = msButtonProps(enumButtonProps.LaunchFile)
            .LaunchFlags = msButtonProps(enumButtonProps.LaunchFlags)
            .UseFRWM = msButtonProps(enumButtonProps.UseFRWM)
            .WindowTitle = msButtonProps(enumButtonProps.WindowTitle)
        End With 'clsThisButton

        'Add the Button Class to the Collection
        mcolButtons.Add(clsThisButton, objThisButton.Name)

        'Get (x,y) coords for this button and move it to the proper location on the menu panel
        Call subGetButtonLocation(nButtonX, nButtonY, mcolButtons.Count)
        objThisButton.Left = nButtonX
        objThisButton.Top = nButtonY

        clsThisButton = Nothing
        objThisButton = Nothing
        objThisControl = Nothing

    End Sub

    Private Sub subFormatMenuPanel()
        Dim nButtons As Integer
        Dim nColumns As Integer
        Dim nRows As Integer

        'Only display this if it turns out that there are no buttons available to this
        'user for this menu.
        lblEmptyMenu.Visible = False

        'Set the Menu Title and Caption
        'lblMenuTitle.Text = rmMenuText.GetString(msMenuProps(enumMenuProps.TitleTag))
        lblMenuTitle.Text = msMenuProps(enumMenuProps.TitleTag)
        'lblCaption.Text = rmMenuText.GetString(msMenuProps(enumMenuProps.CaptionTag))
        lblCaption.Text = msMenuProps(enumMenuProps.CaptionTag)

        'Set the Cancel Button Text and ToolTip
        'btnCancel.Text = rmMenuText.GetString(msMenuProps(enumMenuProps.CancelCaptionTag))
        btnCancel.Text = msMenuProps(enumMenuProps.CancelCaptionTag)
        'ToolTip1.SetToolTip(btnCancel, rmMenuText.GetString(msMenuProps(enumMenuProps.CancelToolTipTag)))
        ToolTip1.SetToolTip(btnCancel, msMenuProps(enumMenuProps.CancelToolTipTag))

        'Resize the panel
        nColumns = CType(msMenuProps(enumMenuProps.Columns), Integer)
        nButtons = mcolButtons.Count

        'Set panel width
        If nButtons < nColumns Then
            nColumns = nButtons
        End If
        Me.Width = (CType(msMenuProps(enumMenuProps.SideMargin), Integer) * 2) + _
                        (CType(msMenuProps(enumMenuProps.ButtonWidth), Integer) * nColumns) + _
                        (CType(msMenuProps(enumMenuProps.HorizontalSpacing), Integer) * (nColumns - 1))
        If Me.Width < CType(msMenuProps(enumMenuProps.MinWidth), Integer) Then
            Me.Width = CType(msMenuProps(enumMenuProps.MinWidth), Integer)
        End If

        'Set panel height
        nRows = nButtons \ nColumns
        If nButtons Mod nColumns > 0 Then
            nRows += 1
        End If
        Me.Height = CType(msMenuProps(enumMenuProps.TopMargin), Integer) + _
                         CType(msMenuProps(enumMenuProps.BottomMargin), Integer) + _
                         (CType(msMenuProps(enumMenuProps.ButtonHeight), Integer) * nRows) + _
                         (CType(msMenuProps(enumMenuProps.VerticalSpacing), Integer) * (nRows - 1))

    End Sub

    Private Sub subGetButtonLocation(ByRef X As Integer, ByRef Y As Integer, ByVal Index As Integer)
        Dim nRow As Integer
        Dim nCol As Integer

        'Determine which row this button is in
        nRow = (Index - 1) \ CType(msMenuProps(enumMenuProps.Columns), Integer)
        'Determine which column this button is in
        nCol = (Index - 1) Mod CType(msMenuProps(enumMenuProps.Columns), Integer)
        'Calculate X
        X = CType(msMenuProps(enumMenuProps.SideMargin), Integer) + _
            (nCol * (CType(msMenuProps(enumMenuProps.ButtonWidth), Integer) + _
            CType(msMenuProps(enumMenuProps.HorizontalSpacing), Integer)))
        'Calculate Y
        Y = CType(msMenuProps(enumMenuProps.TopMargin), Integer) + _
            (nRow * (CType(msMenuProps(enumMenuProps.ButtonHeight), Integer) + _
            CType(msMenuProps(enumMenuProps.VerticalSpacing), Integer)))

    End Sub

    Private Sub subHideButtons()
        Dim objThisControl As New System.Windows.Forms.Control

        For Each objThisControl In Me.Controls
            If Mid(objThisControl.Name, 1, 6) = "Button" Then
                objThisControl.Visible = False
            End If
        Next

    End Sub

    Friend Class SubMenuButton

        Private msCaption As String
        Private msAction As String
        Private msLaunchFile As String
        Private mbUseFRWM As Boolean
        Private mnLaunchFlags As Integer
        Private msWindowTitle As String
        Private msCommandLine As String
        Private bVisible As Boolean

        Friend Property Caption() As String
            Get
                Caption = msCaption
            End Get
            Set(ByVal Value As String)
                msCaption = Value
            End Set
        End Property

        Friend Property Action() As String
            Get
                Action = msAction
            End Get
            Set(ByVal Value As String)
                msAction = Value
            End Set
        End Property

        Friend Property LaunchFile() As String
            Get
                LaunchFile = msLaunchFile
            End Get
            Set(ByVal Value As String)
                msLaunchFile = Value
            End Set
        End Property

        Friend Property UseFRWM()
            Get
                UseFRWM = mbUseFRWM
            End Get
            Set(ByVal Value)
                Select Case LCase(Value)
                    Case "yes", "true"
                        mbUseFRWM = True
                    Case Else
                        mbUseFRWM = False
                End Select
            End Set
        End Property

        Friend Property LaunchFlags()
            Get
                LaunchFlags = mnLaunchFlags
            End Get
            Set(ByVal Value)
                mnLaunchFlags = CType(Value, Integer)
            End Set
        End Property

        Friend Property WindowTitle() As String
            Get
                WindowTitle = msWindowTitle
            End Get
            Set(ByVal Value As String)
                msWindowTitle = Value
            End Set
        End Property

        Friend Property CommandLine() As String
            Get
                CommandLine = msCommandLine
            End Get
            Set(ByVal Value As String)
                msCommandLine = Value
            End Set
        End Property

        Friend Sub Execute()
            'ShutdownRequest    Request to shutdown Paintworks
            'ButtonPress        Request to launch PW Main embedded <screenID> screen
            'LaunchFile         Request to launch an executable (.EXE) file
            Dim bUseFRWM As Boolean

            Select Case Action.ToLower
                Case "shutdownrequest"
                    MsgBox("PAINTworks ShutDown Requested", MsgBoxStyle.Information, "Button Action")
                Case "buttonpress"
                    MsgBox("PWMain Internal Button Action: " & CommandLine, MsgBoxStyle.Information, "Button Action")
                Case "launchfile"
                    bUseFRWM = UseFRWM
                    MsgBox("Launch Application: " & LaunchFile & _
                    " with CommandLine: " & CommandLine & _
                    " with LaunchFlags: " & LaunchFlags & _
                    ". Use FRWM = " & bUseFRWM.ToString, MsgBoxStyle.Information, "Button Action")
                Case Else
                    MsgBox("PAINTworks Unknown Request", MsgBoxStyle.Exclamation, "Button Action")
            End Select
        End Sub

    End Class

    Private Function bButtonAvailable() As Boolean

        bButtonAvailable = True
        If msButtonProps(enumButtonProps.Action) = "0" Then
            bButtonAvailable = System.IO.File.Exists(msButtonProps(enumButtonProps.LaunchFile))
        End If
        'TODO - might need to check other conditions
    End Function

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Hide()
    End Sub
End Class
