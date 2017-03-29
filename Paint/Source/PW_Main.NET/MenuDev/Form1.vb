Public Class frmMenuTest
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
    Friend WithEvents ToolBar1 As System.Windows.Forms.ToolBar
    Friend WithEvents ToolBarButton1 As System.Windows.Forms.ToolBarButton
    Friend WithEvents ToolBarButton2 As System.Windows.Forms.ToolBarButton
    Friend WithEvents ToolBarButton3 As System.Windows.Forms.ToolBarButton
    Friend WithEvents ToolBarButton4 As System.Windows.Forms.ToolBarButton
    Friend WithEvents ToolBarButton5 As System.Windows.Forms.ToolBarButton
    Friend WithEvents ToolBarButton6 As System.Windows.Forms.ToolBarButton
    Friend WithEvents ToolBarButton7 As System.Windows.Forms.ToolBarButton
    Friend WithEvents ToolBarButton8 As System.Windows.Forms.ToolBarButton
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents Button15 As System.Windows.Forms.Button
    Friend WithEvents btnTest As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents Button4 As System.Windows.Forms.Button
    Friend WithEvents Button5 As System.Windows.Forms.Button
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Button6 As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmMenuTest))
        Me.ToolBar1 = New System.Windows.Forms.ToolBar
        Me.ToolBarButton1 = New System.Windows.Forms.ToolBarButton
        Me.ToolBarButton2 = New System.Windows.Forms.ToolBarButton
        Me.ToolBarButton3 = New System.Windows.Forms.ToolBarButton
        Me.ToolBarButton4 = New System.Windows.Forms.ToolBarButton
        Me.ToolBarButton5 = New System.Windows.Forms.ToolBarButton
        Me.ToolBarButton6 = New System.Windows.Forms.ToolBarButton
        Me.ToolBarButton7 = New System.Windows.Forms.ToolBarButton
        Me.ToolBarButton8 = New System.Windows.Forms.ToolBarButton
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.Button15 = New System.Windows.Forms.Button
        Me.btnTest = New System.Windows.Forms.Button
        Me.Button1 = New System.Windows.Forms.Button
        Me.Button2 = New System.Windows.Forms.Button
        Me.Button3 = New System.Windows.Forms.Button
        Me.Button4 = New System.Windows.Forms.Button
        Me.Button5 = New System.Windows.Forms.Button
        Me.TextBox1 = New System.Windows.Forms.TextBox
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.Label1 = New System.Windows.Forms.Label
        Me.Button6 = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'ToolBar1
        '
        Me.ToolBar1.AutoSize = False
        Me.ToolBar1.Buttons.AddRange(New System.Windows.Forms.ToolBarButton() {Me.ToolBarButton1, Me.ToolBarButton2, Me.ToolBarButton3, Me.ToolBarButton4, Me.ToolBarButton5, Me.ToolBarButton6, Me.ToolBarButton7, Me.ToolBarButton8})
        Me.ToolBar1.ButtonSize = New System.Drawing.Size(84, 54)
        Me.ToolBar1.DropDownArrows = True
        Me.ToolBar1.ImageList = Me.ImageList1
        Me.ToolBar1.Location = New System.Drawing.Point(0, 0)
        Me.ToolBar1.Name = "ToolBar1"
        Me.ToolBar1.ShowToolTips = True
        Me.ToolBar1.Size = New System.Drawing.Size(855, 60)
        Me.ToolBar1.TabIndex = 0
        '
        'ToolBarButton1
        '
        Me.ToolBarButton1.ImageIndex = 0
        Me.ToolBarButton1.Tag = "100"
        Me.ToolBarButton1.Text = "Button1"
        '
        'ToolBarButton2
        '
        Me.ToolBarButton2.ImageIndex = 1
        Me.ToolBarButton2.Tag = "200"
        Me.ToolBarButton2.Text = "Button2"
        '
        'ToolBarButton3
        '
        Me.ToolBarButton3.ImageIndex = 2
        Me.ToolBarButton3.Tag = "300"
        Me.ToolBarButton3.Text = "Button3"
        '
        'ToolBarButton4
        '
        Me.ToolBarButton4.ImageIndex = 3
        Me.ToolBarButton4.Tag = "400"
        Me.ToolBarButton4.Text = "Button4"
        '
        'ToolBarButton5
        '
        Me.ToolBarButton5.ImageIndex = 4
        Me.ToolBarButton5.Tag = "500"
        Me.ToolBarButton5.Text = "Button5"
        '
        'ToolBarButton6
        '
        Me.ToolBarButton6.ImageIndex = 5
        Me.ToolBarButton6.Tag = "600"
        Me.ToolBarButton6.Text = "Button6"
        '
        'ToolBarButton7
        '
        Me.ToolBarButton7.ImageIndex = 6
        Me.ToolBarButton7.Tag = "700"
        Me.ToolBarButton7.Text = "Button7"
        '
        'ToolBarButton8
        '
        Me.ToolBarButton8.ImageIndex = 7
        Me.ToolBarButton8.Tag = "800"
        Me.ToolBarButton8.Text = "Button8"
        '
        'ImageList1
        '
        Me.ImageList1.ImageSize = New System.Drawing.Size(32, 32)
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        '
        'Button15
        '
        Me.Button15.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button15.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button15.Location = New System.Drawing.Point(832, 0)
        Me.Button15.Name = "Button15"
        Me.Button15.Size = New System.Drawing.Size(24, 24)
        Me.Button15.TabIndex = 18
        Me.Button15.Text = "X"
        '
        'btnTest
        '
        Me.btnTest.Location = New System.Drawing.Point(808, 40)
        Me.btnTest.Name = "btnTest"
        Me.btnTest.Size = New System.Drawing.Size(40, 24)
        Me.btnTest.TabIndex = 19
        Me.btnTest.Text = "Test"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(676, 0)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(64, 16)
        Me.Button1.TabIndex = 20
        Me.Button1.Text = "Maximize"
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(676, 16)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(64, 16)
        Me.Button2.TabIndex = 21
        Me.Button2.Text = "Minimize"
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(676, 32)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(64, 16)
        Me.Button3.TabIndex = 22
        Me.Button3.Text = "Normal"
        '
        'Button4
        '
        Me.Button4.Location = New System.Drawing.Point(744, 0)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(64, 16)
        Me.Button4.TabIndex = 23
        Me.Button4.Text = "Resize"
        '
        'Button5
        '
        Me.Button5.Location = New System.Drawing.Point(744, 16)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(64, 16)
        Me.Button5.TabIndex = 24
        Me.Button5.Text = "Close"
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(744, 40)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(56, 20)
        Me.TextBox1.TabIndex = 25
        Me.TextBox1.Text = "0"
        '
        'Timer1
        '
        Me.Timer1.Interval = 500
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(816, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(184, 16)
        Me.Label1.TabIndex = 26
        Me.Label1.Text = "Label1"
        '
        'Button6
        '
        Me.Button6.Location = New System.Drawing.Point(680, 48)
        Me.Button6.Name = "Button6"
        Me.Button6.Size = New System.Drawing.Size(56, 16)
        Me.Button6.TabIndex = 27
        Me.Button6.Text = "Button6"
        '
        'frmMenuTest
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(855, 558)
        Me.ControlBox = False
        Me.Controls.Add(Me.Button6)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.Button5)
        Me.Controls.Add(Me.Button4)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.btnTest)
        Me.Controls.Add(Me.Button15)
        Me.Controls.Add(Me.ToolBar1)
        Me.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmMenuTest"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Menu Test"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Enum FRWM_Flags
        FW_EXCLUSIVE_MODE = 64  'No other App can run while this one is running
        FW_MULTI_START = 256    'Multiple instances of this App can run simultaneously
    End Enum

    Private mdsMenuData As New DataSet
    Private mfrmSubMenu As frmMenu
    Private msButtonAction As String

    Friend Structure udsAction
        Friend Action As String
        Friend Commandline As String
        Friend LaunchFile As String
        Friend LaunchFlags As Integer
        Friend UseFRWM As Boolean
        Friend WindowTitle As String
    End Structure
    Private Shared mAction As udsAction

    Private Structure udsProcInfo
        Friend ID As String         'This field is set to -1 for local windows
        Friend Name As String
        Friend CmdLine As String
        Friend hWnd As Integer
    End Structure
    Private Shared mProcInfo(0) As udsProcInfo

    Public Shared mrecWorkingArea As Rectangle

    Private Shared frmForm2 As New Form2
    Private Shared frmForm3 As New Form3
    Private frmStatus As New frmStatusBar

    Private Shared mnNotepadProc As Integer
    Private Shared mnLoop As Integer

    Private Shared msTileMode As String
    'Private Shared mcolProcs As New Collection

    Const WM_NULL As Int32 = &H0
    Private Declare Function SendMessage Lib "user32.dll" Alias "SendMessageA" ( _
                                                                            ByVal hwnd As Int32, _
                                                                            ByVal wMsg As Int32, _
                                                                            ByVal wParam As Int32, _
                                                                            ByVal lParam As Int32) As Int32
    Private Declare Function BringWindowToTop Lib "user32" Alias "BringWindowToTop" (ByVal hwnd As Integer) As Integer
    Private Declare Function MoveWindow Lib "user32" Alias "MoveWindow" (ByVal hwnd As Integer, ByVal x As Integer, ByVal y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal bRepaint As Integer) As Long

    Friend Shared Property ButtonAction() As udsAction
        Get
            ButtonAction = mAction
        End Get
        Set(ByVal Value As udsAction)
            mAction = Value
            Call subExecuteSubMenuCmd()
        End Set
    End Property


    Private Sub ToolBar1_ButtonClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolBarButtonClickEventArgs) Handles ToolBar1.ButtonClick
        Dim frmSubMenu As New frmMenu

        'If a sub menu is already showing, then close it
        If Not mfrmSubMenu Is Nothing Then
            mfrmSubMenu.Close()
        End If

        'Show the sub-menu for the selected main menu button in the proper location
        mfrmSubMenu = frmSubMenu
        mfrmSubMenu.Hide()

        'Construct the sub-menu panel
        mfrmSubMenu.MenuData = mdsMenuData
        mfrmSubMenu.MenuScreenID() = e.Button.Tag()

        'Show the sub-menu for the selected main menu button in the proper location
        mfrmSubMenu.MenuLeft = e.Button.Rectangle.Left
        mfrmSubMenu.MenuTop = ToolBar1.Size.Height + ToolBar1.Location.Y
        mfrmSubMenu.Show()
        frmSubMenu = Nothing

    End Sub

    Private Sub subDrawMainMenu(ByVal nUserId As Integer)
        Dim dtMenuData As New DataTable
        Dim drMainMenuButton As DataRow
        Dim nButton As Integer
        Dim sMainScreenID As String
        Dim objUserData As New Authorization.AuthorizationService

        'Read the data for the submenus into a dataset from the Authorization web service
        mdsMenuData = Nothing
        mdsMenuData = objUserData.BuildDSScreenAccessList(nUserId)

        'Read the menu panel configurations for each main menu button
        dtMenuData = mdsMenuData.Tables("MainMenuData")

        'Temporarily, make all the main menu buttons invisible
        For nButton = 0 To ToolBar1.Buttons.Count - 1
            ToolBar1.Buttons(nButton).Visible = False
        Next

        'The only main menu buttons that should be visible are the ones this user has privilege for.
        For Each drMainMenuButton In dtMenuData.Rows
            sMainScreenID = drMainMenuButton.Item("MainScreenID")
            For nButton = 0 To ToolBar1.Buttons.Count - 1
                If ToolBar1.Buttons(nButton).Tag = sMainScreenID Then
                    ToolBar1.Buttons(nButton).Text = drMainMenuButton.Item("TitleTag")
                    ToolBar1.Buttons(nButton).Visible = True
                End If
            Next
        Next

        'Need to trick the tollbar into keeping the original button size. 
        'This is tacky, but so is VB sometimes ...ps)
        Me.ToolBar1.ButtonSize = New System.Drawing.Size(88, 54)
        Me.ToolBar1.Refresh()
        Me.ToolBar1.ButtonSize = New System.Drawing.Size(84, 54)
        Me.ToolBar1.Refresh()

        'Clean Up
        dtMenuData = Nothing
        drMainMenuButton = Nothing

    End Sub

    Private Shared Sub subExecuteSubMenuCmd()
        Dim myAction As udsAction = ButtonAction
        Dim nProcs As Integer
        Dim myID As Integer

        With myAction

            Select Case .Action.ToLower
                Case "buttonpress"
                    'Request to launch PW Main embedded screen
                    Select Case .LaunchFile.ToLower
                        Case "form2"
                            If Not frmForm2.Visible Then
                                nProcs = mProcInfo.GetUpperBound(0) + 1
                                ReDim Preserve mProcInfo(nProcs)
                                mProcInfo(nProcs).ID = "-1"
                                mProcInfo(nProcs).Name = "form2"
                                mProcInfo(nProcs).CmdLine = .Commandline
                                frmForm2.Show()
                                Call subTile()
                            Else
                                Call BringWindowToTop(frmForm2.Handle.ToInt32)
                                'frmForm2.WindowState = FormWindowState.Minimized
                                frmForm2.WindowState = FormWindowState.Normal
                            End If
                        Case "form3"
                            If Not frmForm3.Visible Then
                                nProcs = mProcInfo.GetUpperBound(0) + 1
                                ReDim Preserve mProcInfo(nProcs)
                                mProcInfo(nProcs).ID = "-1"
                                mProcInfo(nProcs).Name = "form3"
                                mProcInfo(nProcs).CmdLine = .Commandline
                                frmForm3.Show()
                                Call subTile()
                            Else
                                Call BringWindowToTop(frmForm3.Handle.ToInt32)
                                'frmForm3.WindowState = FormWindowState.Minimized
                                frmForm3.WindowState = FormWindowState.Normal
                            End If
                        Case Else

                    End Select


                Case "launchfile"
                    'Request to launch an executable (.EXE) file
                    If .UseFRWM Then
                        'Check FRWM flags to see if it's OK to launch
                        If OKToLaunch() Then
                            Dim Process1 As New Process


                            Process1.StartInfo.FileName = .LaunchFile
                            Process1.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
                            Process1.StartInfo.Arguments = .Commandline
                            Process1.StartInfo.UseShellExecute = False
                            Process1.Start()

                            nProcs = mProcInfo.GetUpperBound(0) + 1
                            ReDim Preserve mProcInfo(nProcs)
                            myID = Process1.Id
                            mProcInfo(nProcs).ID = myID.ToString
                            mProcInfo(nProcs).Name = Process1.ProcessName
                            mProcInfo(nProcs).CmdLine = .Commandline
                            mnLoop = 0
                            Do While Process1.MainWindowHandle.ToInt32 = 0
                                Application.DoEvents()
                                Process1.Refresh()
                                mnLoop += 1
                            Loop
                            mProcInfo(nProcs).hWnd = Process1.MainWindowHandle.ToInt32

                            'mcolProcs.Add(Process1, myID.ToString)

                            Call subSendFRWMMessage(mProcInfo(nProcs).ID & ",setmaximizedbounds,0," & _
                                                    mrecWorkingArea.X.ToString & "," & _
                                                    mrecWorkingArea.Y.ToString & "," & _
                                                    mrecWorkingArea.Width.ToString & "," & _
                                                    mrecWorkingArea.Height.ToString)
                            Application.DoEvents()
                            Call subTile()
                        End If
                    Else
                        'This application is not managed by Paintworks
                        'Valid .LaunchFlags values:
                        '0 - AppWinStyle.Hide 
                        '1 - AppWinStyle.NormalFocus
                        '2 - AppWinStyle.MinimizedFocus
                        '3 - AppWinStyle.MaximizedFocus
                        '4 - AppWinStyle.NormalNoFocus
                        '6 - AppWinStyle.MinimizedNoFocus
                        mnNotepadProc = Shell(.LaunchFile, .LaunchFlags, False, -1)
                        'TODO should we keep track of the procID so we can shut the proc down from Paintworks?
                    End If '.UseFRWM

                Case "shutdownrequest"
                    'Request to shutdown Paintworks
                    Call subCloseAllApps()
                    System.Threading.Thread.Sleep(1001)
                    End

                Case Else
                    'Future functionality
            End Select

        End With

    End Sub

    Private Sub frmMenuTest_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim objScreenBounds As Rectangle

        'Resize Main form Height to the Height of the ToolBar
        objScreenBounds = Screen.GetBounds(ToolBar1)
        Me.Height = ToolBar1.Height
        Me.Width = objScreenBounds.Width

        'Get the working area of the screen
        mrecWorkingArea = SystemInformation.WorkingArea

        'Show the statusbar, then calculate the Paintworks working area
        frmStatus.Width = mrecWorkingArea.Width
        frmStatus.ScreenBottom = mrecWorkingArea.Height
        frmStatus.Show()

        mrecWorkingArea.Y = ToolBar1.Height
        mrecWorkingArea.Height = mrecWorkingArea.Height - (ToolBar1.Height + frmStatus.Height)


        'Redraw the main menu for the current user
        Call subDrawMainMenu(50)

        'Default to cascade tile mode
        'Supported modes are: cascade, horizontal, icon, none, vertical
        msTileMode = "cascade"

    End Sub

    Private Sub btnTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTest.Click
        Dim nProcs As Integer
        Dim sParams() As String

        nProcs = mProcInfo.GetUpperBound(0) + 1
        ReDim Preserve mProcInfo(nProcs)

        sParams = Split(TextBox1.Text, ",")
        mProcInfo(nProcs).ID = sParams(0)
        mProcInfo(nProcs).hWnd = CType(sParams(1), Integer)
        Call subSendFRWMMessage(mProcInfo(nProcs).ID & ",setmaximizedbounds,0," & _
                                mrecWorkingArea.X.ToString & "," & _
                                mrecWorkingArea.Y.ToString & "," & _
                                mrecWorkingArea.Width.ToString & "," & _
                                mrecWorkingArea.Height.ToString)
        Call subTile()


    End Sub

    Private Sub Button15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button15.Click
        End
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim nProc As Integer
        Dim nProcs As Integer

        'Maximize
        nProcs = mProcInfo.GetUpperBound(0)
        If nProcs > 0 Then
            For nProc = 1 To nProcs
                If mProcInfo(nProc).ID > 0 Then
                    Call subSendFRWMMessage(mProcInfo(nProc).ID & ",Maximize,0,0,0,0,0")
                    Application.DoEvents()
                Else
                    Select Case mProcInfo(nProc).Name
                        Case "form2"
                            frmForm2.WindowState = FormWindowState.Maximized
                        Case "form3"
                            frmForm3.WindowState = FormWindowState.Maximized
                    End Select
                End If
            Next
        End If

    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim nProc As Integer
        Dim nProcs As Integer

        'Minimize 
        nProcs = mProcInfo.GetUpperBound(0)
        If nProcs > 0 Then
            For nProc = 1 To nProcs
                If mProcInfo(nProc).ID > 0 Then
                    Call subSendFRWMMessage(mProcInfo(nProc).ID & ",Minimize,0,0,0,0,0")
                    Application.DoEvents()
                Else
                    Select Case mProcInfo(nProc).Name
                        Case "form2"
                            frmForm2.WindowState = FormWindowState.Minimized
                        Case "form3"
                            frmForm3.WindowState = FormWindowState.Minimized
                    End Select
                End If
            Next
        End If

    End Sub

    Private Sub Button3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim nProc As Integer
        Dim nProcs As Integer

        'Normal
        nProcs = mProcInfo.GetUpperBound(0)
        If nProcs > 0 Then
            For nProc = 1 To nProcs
                If mProcInfo(nProc).ID > 0 Then
                    Call subSendFRWMMessage(mProcInfo(nProc).ID & ",Normal,0,0,0,0,0")
                    Application.DoEvents()
                Else
                    Select Case mProcInfo(nProc).Name
                        Case "form2"
                            frmForm2.WindowState = FormWindowState.Normal
                        Case "form3"
                            frmForm3.WindowState = FormWindowState.Normal
                    End Select
                End If
            Next
        End If

    End Sub

    Private Sub Button4_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button4.Click

        Call subTile()

    End Sub

    Private Sub Button5_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button5.Click

        'Close
        Call subCloseAllApps()

    End Sub

    Private Shared Sub subCloseAllApps()
        Dim nProc As Integer
        Dim nProcs As Integer

        nProcs = mProcInfo.GetUpperBound(0)
        If nProcs > 0 Then
            For nProc = 1 To nProcs
                If mProcInfo(nProc).ID > 0 Then
                    Call subSendFRWMMessage(mProcInfo(nProc).ID & ",Close,0,0,0,0,0")
                    Application.DoEvents()
                Else
                    Select Case mProcInfo(nProc).Name
                        Case "form2"
                            frmForm2.Hide()
                        Case "form3"
                            frmForm3.Hide()
                    End Select
                End If
            Next
            ReDim mProcInfo(0)
        End If


    End Sub

    Private Shared Sub subTile()
        Dim nProc As Integer
        Dim nProcs As Integer
        Dim sRect As String

        'Resize
        nProcs = mProcInfo.GetUpperBound(0)
        If nProcs > 0 Then
            For nProc = 1 To nProcs
                sRect = GetFormRect(nProcs, nProc)
                Dim sParams() As String
                sParams = Split(sRect, ",")
                If mProcInfo(nProc).ID > 0 Then
                    'Call subSendFRWMMessage(mProcInfo(nProc).ID & ",Resize,0," & sRect)
                    'http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/windows/windowreference/windowfunctions/movewindow.asp
                    Call subSendFRWMMessage(mProcInfo(nProc).ID & ",normal,0,0,0,0,0")
                    Call MoveWindow(mProcInfo(nProc).hWnd, CType(sParams(0), Integer), CType(sParams(1), Integer), CType(sParams(2), Integer), CType(sParams(3), Integer), True)
                    Application.DoEvents()
                Else
                    'Dim sParams() As String
                    'sParams = Split(sRect, ",")
                    Select Case mProcInfo(nProc).Name
                        Case "form2"
                            frmForm2.SetDesktopBounds(CType(sParams(0), Integer), CType(sParams(1), Integer), CType(sParams(2), Integer), CType(sParams(3), Integer))
                            frmForm2.WindowState = FormWindowState.Normal
                        Case "form3"
                            frmForm3.SetDesktopBounds(CType(sParams(0), Integer), CType(sParams(1), Integer), CType(sParams(2), Integer), CType(sParams(3), Integer))
                            frmForm3.WindowState = FormWindowState.Normal
                    End Select

                End If
            Next
        End If

    End Sub

    Private Shared Function GetFormRect(ByVal FormCount As Integer, ByVal FormIndex As Integer) As String
        Dim nXLoc As Integer
        Dim nYLoc As Integer
        Dim nHeight As Integer
        Dim nWidth As Integer

        Select Case msTileMode
            Case "cascade"
                nXLoc = mrecWorkingArea.Left + ((FormIndex - 1) * 30)
                nYLoc = mrecWorkingArea.Top + ((FormIndex - 1) * 30)
                nHeight = mrecWorkingArea.Height - ((FormCount - 1) * 30)
                nWidth = mrecWorkingArea.Width - ((FormCount - 1) * 30)

            Case "horizontal"
                Dim nCols As Integer = 1
                Dim nCol As Integer
                Dim nForms() As Integer

                'Check if the height of FormCount forms will be greater than minimum height.
                nHeight = mrecWorkingArea.Height \ FormCount
                If nHeight < 200 Then
                    'Too many forms to tile horizontally. We need more than 1 column.
                    nCols = (FormCount * 200) \ mrecWorkingArea.Height
                    If (FormCount * 200) > (nCols * mrecWorkingArea.Height) Then
                        nCols += 1
                    End If
                End If

                'Calculate the width of all forms
                nWidth = mrecWorkingArea.Width \ nCols

                'Calculate how many forms will be in each column.
                ReDim nForms(nCols)
                For nCol = 1 To nCols
                    nForms(nCol) = FormCount \ nCols
                Next
                If nCols > 1 Then nForms(nCols) = FormCount - ((nCols - 1) * nForms(1))

                'Determine which column this form (FormIndex) is in.
                For nCol = 1 To nCols
                    nForms(0) = nForms(0) + nForms(nCol)
                    If FormIndex <= nForms(0) Then Exit For
                Next

                'Calculate the height of this form
                nHeight = mrecWorkingArea.Height \ nForms(nCol)

                'Calculate the X (Left) coordinate of this form
                nXLoc = ((nCol - 1) * nWidth) + mrecWorkingArea.Left

                'Calculate the Y (Top) coordinate of this form
                nYLoc = ((nForms(0) - FormIndex) * nHeight) + mrecWorkingArea.Top

            Case "icon"
                Dim nRows As Integer = 1
                Dim nRow As Integer
                Dim nForms() As Integer

                'Check if the width of FormCount icons will be greater than minimum width.
                nWidth = mrecWorkingArea.Width \ FormCount
                If nWidth < 200 Then
                    'Too many icons. We need more than 1 row.
                    nRows = (FormCount * 200) \ mrecWorkingArea.Width
                    If (FormCount * 200) > (nRows * mrecWorkingArea.Width) Then
                        nRows += 1
                    End If
                End If

                'Height of all icons
                nHeight = 30

                'Calculate how many icons will be in each row.
                ReDim nForms(nRows)
                For nRow = 1 To nRows
                    nForms(nRow) = FormCount \ nRows
                Next
                If nRows > 1 Then nForms(nRows) = FormCount - ((nRows - 1) * nForms(1))

                'Determine which row this icon (FormIndex) is in.
                For nRow = 1 To nRows
                    nForms(0) = nForms(0) + nForms(nRow)
                    If FormIndex <= nForms(0) Then Exit For
                Next

                'Widthidth of all icons
                nWidth = 200

                'Calculate the X (Left) coordinate of this icon
                nXLoc = ((nForms(0) - FormIndex) * nWidth) + mrecWorkingArea.Left

                'Calculate the Y (Top) coordinate of this icon
                nYLoc = (mrecWorkingArea.Height + mrecWorkingArea.Top) - (nRow * nHeight)

            Case "none"
                'fill the working area with this form
                nXLoc = mrecWorkingArea.Left
                nYLoc = mrecWorkingArea.Top
                nHeight = mrecWorkingArea.Height
                nWidth = mrecWorkingArea.Width

            Case "vertical"
                Dim nRows As Integer = 1
                Dim nRow As Integer
                Dim nForms() As Integer

                'Check if the width of FormCount forms will be greater than minimum width.
                nWidth = mrecWorkingArea.Width \ FormCount
                If nWidth < 300 Then
                    'Too many forms to tile vertically. We need more than 1 row.
                    nRows = (FormCount * 300) \ mrecWorkingArea.Width
                    If (FormCount * 300) > (nRows * mrecWorkingArea.Width) Then
                        nRows += 1
                    End If
                End If

                'Calculate the Height of all forms
                nHeight = mrecWorkingArea.Height \ nRows

                'Calculate how many forms will be in each row.
                ReDim nForms(nRows)
                For nRow = 1 To nRows
                    nForms(nRow) = FormCount \ nRows
                Next
                If nRows > 1 Then nForms(nRows) = FormCount - ((nRows - 1) * nForms(1))

                'Determine which row this form (FormIndex) is in.
                For nRow = 1 To nRows
                    nForms(0) = nForms(0) + nForms(nRow)
                    If FormIndex <= nForms(0) Then Exit For
                Next

                'Calculate the width of this form
                nWidth = mrecWorkingArea.Width \ nForms(nRow)

                'Calculate the X (Left) coordinate of this form
                nXLoc = ((nForms(0) - FormIndex) * nWidth) + mrecWorkingArea.Left

                'Calculate the Y (Top) coordinate of this form
                nYLoc = ((nRow - 1) * nHeight) + mrecWorkingArea.Top

        End Select

        GetFormRect = nXLoc.ToString & "," & nYLoc.ToString & "," & nWidth.ToString & "," & nHeight.ToString

    End Function

    Private Shared Function OKToLaunch() As Boolean
        Dim ThisAction As udsAction = ButtonAction

        If ThisAction.LaunchFlags And FRWM_Flags.FW_EXCLUSIVE_MODE Then
            If mProcInfo.GetUpperBound(0) > 0 Then
                MsgBox("Please close all other applications before staring this application.", MsgBoxStyle.Exclamation, "FRWM Exclusive Mode")
                Return False
            End If
        End If

        If ThisAction.LaunchFlags And FRWM_Flags.FW_MULTI_START Then
            Return True
        Else
            'Only 1 instance can run at a time.
            Dim sFields() As String
            Dim nLast As Integer
            Dim nProc As Integer
            Dim sProcName As String

            'Parse the Process Name from .LaunchFile
            sFields = Split(ThisAction.LaunchFile, "\")
            nLast = sFields.GetUpperBound(0)
            sProcName = sFields(nLast)
            nLast = InStr(sProcName, ".")
            sProcName = Mid(sProcName, 1, nLast - 1)

            For nProc = 1 To mProcInfo.GetUpperBound(0)
                If (mProcInfo(nProc).Name = sProcName) And (mProcInfo(nProc).CmdLine.ToLower = ThisAction.Commandline.ToLower) Then
                    'This app is already running so force it to the foreground
                    'and don't start another.
                    Call BringWindowToTop(mProcInfo(nProc).hWnd)
                    Call subSendFRWMMessage(mProcInfo(nProc).ID & ",Normal,0,0,0,0,0")
                    Return False
                End If
            Next
        End If

        Return True

    End Function

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click

        msTileMode = TextBox1.Text

        '~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        'Dim MyProc As Process
        'Dim hWnd As System.IntPtr

        'MyProc = Process.GetProcessById(mnNotepadProc)
        'hWnd = MyProc.MainWindowHandle
        'MyProc.CloseMainWindow()
        '~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        'Dim nProc As Integer
        'Dim nProcs As Integer
        'Dim sXLoc As String
        'Dim sYLoc As String
        'Dim sHeight As String
        'Dim sWidth As String


    End Sub

    Private Shared Sub subSendFRWMMessage(ByVal sMsg As String)
        Dim nProcs As Integer
        Dim nProc As Integer
        Dim nItem As Integer

        nProcs = mProcInfo.GetUpperBound(0)
        If nProcs > 0 Then
            'Convert message string to byte array
            Dim bytMsg() As Byte = System.Text.Encoding.Default.GetBytes(sMsg)

            For nProc = 1 To nProcs
                'Only send FRWM commands to PW external apps
                If mProcInfo(nProc).ID > 0 Then
                    'This is the header message. LParam tells the target process how many 
                    'characters to expect.
                    SendMessage(mProcInfo(nProc).hWnd, WM_NULL, 0, sMsg.Length)
                    For nItem = 1 To sMsg.Length
                        'This is the mressage body. WParam contains 1 character so we'll send
                        'as many messages as it takes to deliver the whole string to the target 
                        'process.
                        SendMessage(mProcInfo(nProc).hWnd, WM_NULL, bytMsg(nItem - 1), 0)
                    Next
                End If
            Next
        End If
    End Sub

    Private Sub Button15_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button15.MouseEnter
        Label1.Text = mnLoop.ToString
    End Sub
End Class
