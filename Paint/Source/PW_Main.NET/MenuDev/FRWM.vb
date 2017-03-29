Module FRWM
    '***** tweekable stuff here *****************************************************************
    Private Const mnMAX_APPS As Integer = 3         ' how many pwapps can we run?
    Private Const mlWAITFORWINDOWMS As Long = 10000  ' miliseconds for timeout
    '********************************************************************************************

    Private mbFrmFirstVisible As Boolean
    Private mbDeskTopOk As Boolean
    Private mhSearchdisk As Long                    ' searchdisk window handle
    Private mhIE As Long                            ' internet explorer handle 

    Private Structure RECT                               ' square stuff
        Friend Left As Long
        Friend Top As Long
        Friend Right As Long
        Friend Bottom As Long
    End Structure

    Private Structure SECURITY_ATTRIBUTES                ' for CreateProcess
        Friend nLength As Long
        Friend lpSecurityDescriptor As Long
        Friend bInheritHandle As Long
    End Structure

    Private Structure PROCESS_INFORMATION                ' for CreateProcess
        Friend hProcess As Long
        Friend hThread As Long
        Friend dwProcessId As Long
        Friend dwThreadId As Long
    End Structure

    Private Structure STARTUPINFO                        ' for CreateProcess
        Friend cb As Long
        Friend lpReserved As String
        Friend lpDesktop As String
        Friend lpTitle As String
        Friend dwX As Long
        Friend dwY As Long
        Friend dwXSize As Long
        Friend dwYSize As Long
        Friend dwXCountChars As Long
        Friend dwYCountChars As Long
        Friend dwFillAttribute As Long
        Friend dwFlags As Long
        Friend wShowWindow As Integer
        Friend cbReserved2 As Integer
        Friend lpReserved2 As Long
        Friend hStdInput As Long
        Friend hStdOutput As Long
        Friend hStdError As Long
    End Structure

    Private Structure PWApp                              'type to keep track of our apps
        Friend hWnd As Long
        Friend flags As Long
        Friend LaunchString As String
        Friend lpProcInfo As PROCESS_INFORMATION
    End Structure

    Private mPWApps() As PWApp                      ' to keep track of our child apps
    Private mOtherApps() As PWApp                   ' to keep track of apps not part of desktop eg. alarmman

    Private mnTile As Integer                       ' property holder

    Private mhPWProcessID As Long                   ' id of pwmain
    Private mbDesktopExists As Boolean              ' set when we hide the apps
    Private mhHiddenWindows() As Long               ' non - pw apps we have hidden
    Private mnAppsRunning As Integer                ' number of pw apps running

    Private mtPWDesktop As RECT                     ' used to size pwapps - what's left of
    ' the screen after frmmain
    Private msWindowTitle As String                 ' String holder, can't pass a string to callback
    Private mhHandleHolder As Long                  ' used to hold handle found in callback

    ' constants for showwindow
    Private Const SW_HIDE = 0
    Private Const SW_RESTORE = 9
    Private Const SW_SHOWNA = 8
    Private Const SW_NORMAL = 1
    Private Const SW_SHOWMAXIMIZED = 3
    Private Const SW_SHOWMINIMIZED = 2
    Private Const SW_SHOWMINNOACTIVE = 7
    ' constants for startupinfo
    Private Const STARTF_FORCEOFFFEEDBACK = &H80&
    Private Const STARTF_FORCEONFEEDBACK = &H40&
    Private Const STARTF_USEPOSITION = &H4&
    Private Const STARTF_USESHOWWINDOW = &H1&
    Private Const STARTF_USESIZE = &H2&
    Private Const CREATE_NEW_CONSOLE = &H10&
    Private Const DETACHED_PROCESS = &H8&
    Private Const NORMAL_PRIORITY_CLASS = &H20&
    ' constants for getsystemmetrics
    Private Const SM_CYCAPTION = 4
    Private Const SM_CYFRAME = 33

    Private Const PROCESS_ALL_ACCESS = &H1F0FFF

    ' hide or toast the windows
    Public Enum eHelpWin
        Hide = 1&
        Destroy = 2&
    End Enum

    ' method of arranging windows
    Public Enum eTile
        Cascade = 0
        Horizontal = 1
        Vertical = 2
    End Enum

    ' flags for launching a pw app
    Public Enum eStartupFlags
        ExclusiveMode = &H40&
        AllowMultiStart = &H100&
    End Enum

    'winapi calls
    Private Declare Function CloseHandle Lib "kernel32" (ByVal hObject As Long) As Long
    Private Declare Function CreateProcess Lib "kernel32" Alias "CreateProcessA" _
                    (ByVal lpApplicationName As String, ByVal lpCommandLine As String, _
                     ByVal lpProcessAttributes As SECURITY_ATTRIBUTES, _
                     ByVal lpThreadAttributes As SECURITY_ATTRIBUTES, _
                     ByVal bInheritHandles As Long, ByVal dwCreationFlags As Long, _
                     ByVal lpEnvironment As Long, ByVal lpCurrentDriectory As String, _
                     ByVal lpStartupInfo As STARTUPINFO, _
                     ByVal lpProcessInformation As PROCESS_INFORMATION) As Long
    Private Declare Function DestroyWindow Lib "user32" (ByVal hWnd As Long) As Long
    Private Declare Function EnumWindows Lib "user32" (ByVal lpEnumFunc As Long, _
                     ByVal lParam As Long) As Long
    Private Declare Function GetCurrentProcessId Lib "kernel32" () As Long
    Private Declare Function GetDesktopWindow Lib "user32" () As Long
    Private Declare Function GetSystemMetrics Lib "user32" (ByVal nIndex As Long) As Long
    Private Declare Function GetTickCount Lib "kernel32" () As Long
    Private Declare Function GetWindowRect Lib "user32" (ByVal hWnd As Long, _
                     ByVal lpRect As RECT) As Long
    Private Declare Function GetWindowText Lib "user32" Alias "GetWindowTextA" _
                    (ByVal hWnd As Long, ByVal lpString As String, ByVal cch As Long) As Long
    Private Declare Function GetWindowThreadProcessId Lib "user32" (ByVal hWnd As Long, _
                     ByVal lpdwProcessId As Long) As Long
    Private Declare Function IsIconic Lib "user32" (ByVal hWnd As Long) As Long
    Private Declare Function IsWindow Lib "user32" (ByVal hWnd As Long) As Long
    Private Declare Function IsWindowVisible Lib "user32" (ByVal hWnd As Long) As Long
    Private Declare Function MessageBox Lib "user32" Alias "MessageBoxA" _
                        (ByVal hwnd As Long, ByVal lpText As String, ByVal lpCaption As String, _
                        ByVal wType As Long) As Long
    Private Declare Function MoveWindow Lib "user32" (ByVal hWnd As Long, _
                     ByVal x As Long, ByVal y As Long, _
                     ByVal nWidth As Long, ByVal nHeight As Long, _
                     ByVal bRepaint As Long) As Long
    Private Declare Function OpenProcess Lib "kernel32" (ByVal dwDesiredAccess As Long, _
                     ByVal bInheritHandle As Long, ByVal dwProcessId As Long) As Long
    Public Declare Function ShowWindow Lib "user32" (ByVal hWnd As Long, _
                    ByVal nCmdShow As Long) As Long
    Private Declare Function TerminateProcess Lib "kernel32" (ByVal hProcess As Long, _
                     ByVal uExitCode As Long) As Long
    Private Declare Function WaitForInputIdle Lib "user32" (ByVal hProcess As Long, _
                     ByVal dwMilliseconds As Long) As Long

    'shutdown windows stuff
    Private Const ANYSIZE_ARRAY = 1
    Private Const EWX_FORCE = 4
    Private Const EWX_LOGOFF = 0
    Private Const EWX_REBOOT = 2
    Private Const EWX_SHUTDOWN = 1

    Private Structure LUID
        Friend lowpart As Long
        Friend highpart As Long
    End Structure

    Private Structure LUID_AND_ATTRIBUTES
        Friend pLuid As LUID
        Friend Attributes As Long
    End Structure

    Private Structure TOKEN_PRIVILEGES
        Friend PrivilegeCount As Long
        Friend Privileges() As LUID_AND_ATTRIBUTES
    End Structure

    Private Declare Function AdjustTokenPrivileges Lib "advapi32.dll" (ByVal TokenHandle As Long, _
                    ByVal DisableAllPrivileges As Long, ByVal NewState As TOKEN_PRIVILEGES, _
                                    ByVal BufferLength As Long, ByVal PreviousState As TOKEN_PRIVILEGES, _
    ByVal ReturnLength As Long) As Long
    Private Declare Function ExitWindowsEx Lib "user32" (ByVal uFlags As Long, _
                                                ByVal dwReserved As Long) As Long
    Private Declare Function GetCurrentProcess Lib "kernel32" () As Long
    Private Declare Function LookupPrivilegeValue Lib "advapi32.dll" Alias "LookupPrivilegeValueA" _
                    (ByVal lpSystemName As String, ByVal lpName As String, ByVal lpLuid As LUID) As Long
    Private Declare Function OpenProcessToken Lib "advapi32.dll" (ByVal ProcessHandle As Long, _
                                            ByVal DesiredAccess As Long, ByVal TokenHandle As Long) As Long
    'check for service stuff
    Private Const GENERIC_READ = &H80000000
    Private Const SC_MANAGER_CONNECT = &H1
    Private Const SC_MANAGER_CREATE_SERVICE = &H2
    Private Const SC_MANAGER_ENUMERATE_SERVICE = &H4
    Private Const SC_MANAGER_LOCK = &H8
    Private Const SC_MANAGER_MODIFY_BOOT_CONFIG = &H20
    Private Const SC_MANAGER_QUERY_LOCK_STATUS = &H10
    Private Const SERVICE_RUNNING = &H4
    Private Const STANDARD_RIGHTS_REQUIRED = &HF0000
    Private Const SC_MANAGER_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED Or _
                                            SC_MANAGER_CONNECT Or _
                                            SC_MANAGER_CREATE_SERVICE Or _
                                            SC_MANAGER_ENUMERATE_SERVICE Or _
                                            SC_MANAGER_LOCK Or _
                                            SC_MANAGER_QUERY_LOCK_STATUS Or _
                                            SC_MANAGER_MODIFY_BOOT_CONFIG)

    Private Structure SERVICE_STATUS
        Friend dwServiceType As Long
        Friend dwCurrentState As Long
        Friend dwControlsAccepted As Long
        Friend dwWin32ExitCode As Long
        Friend dwServiceSpecificExitCode As Long
        Friend dwCheckPoint As Long
        Friend dwWaitHint As Long
    End Structure

    Private Declare Function CloseServiceHandle Lib "advapi32.dll" _
                                                    (ByVal hSCObject As Long) As Long
    Private Declare Function OpenService Lib "advapi32.dll" Alias "OpenServiceA" _
                                (ByVal hSCManager As Long, ByVal lpServiceName As String, _
                                ByVal dwDesiredAccess As Long) As Long
    Private Declare Function OpenSCManager Lib "advapi32.dll" Alias "OpenSCManagerA" _
                                (ByVal lpMachineName As String, ByVal lpDatabaseName As String, _
                                ByVal dwDesiredAccess As Long) As Long
    Private Declare Function QueryServiceStatus Lib "advapi32.dll" (ByVal hService As Long, _
    ByVal lpServiceStatus As SERVICE_STATUS) As Long
    ' code timer stuff
    Public Declare Function SetTimer Lib "user32" (ByVal hWnd As Long, _
            ByVal nIDEvent As Long, ByVal uElapse As Long, ByVal lpTimerFunc As Long) As Long
    Public Declare Function KillTimer Lib "user32" (ByVal hWnd As Long, _
            ByVal nIDEvent As Long) As Long

    Public gbTimerDone As Boolean

    ' mouse hook stuff
    Private Const WM_RBUTTONDBLCLK = &H206
    Private Const WM_RBUTTONDOWN = &H204
    Private Const WM_RBUTTONUP = &H205
    Private Const WH_MOUSE = 7

    Private Structure POINTAPI
        Friend x As Long
        Friend y As Long
    End Structure

    Private Structure MOUSEHOOKSTRUCT
        Friend pt As POINTAPI
        Friend hWnd As Long
        Friend wHitTestCode As Long
        Friend dwExtraInfo As Long
    End Structure

    Private ghHook As Long

    Private Declare Function CallNextHookEx Lib "user32" _
                (ByVal hHook As Long, ByVal nCode As Long, ByVal wParam As Long, ByVal lParam As Long) As Long
    Private Declare Function SetWindowsHookEx Lib "user32" Alias "SetWindowsHookExA" _
                (ByVal idHook As Long, ByVal lpfn As Long, ByVal hmod As Long, ByVal dwThreadId As Long) As Long
    Private Declare Function UnhookWindowsHookEx Lib "user32" (ByVal hHook As Long) As Long

    Public Sub Initialize()
        '*******************************************************************************
        ' Call me first!
        '
        ' Parameters: none
        ' Returns: none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************

        ReDim mhHiddenWindows(0)
        ReDim mPWApps(2)
        ReDim mOtherApps(0)
        mhSearchdisk = 0
        mhIE = 0
        OkToCreateDesktop = True

    End Sub

    Friend Property OkToCreateDesktop() As Boolean
        '*******************************************************************************
        ' This is here so we can skip the desktop in the ide. This is set true in initialize
        '
        ' Parameters: none
        ' Returns: none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        Get
            OkToCreateDesktop = mbDeskTopOk
        End Get
        Set(ByVal Value As Boolean)
            mbDeskTopOk = Value
        End Set

    End Property

    Friend Property FormFirstVisible() As Boolean
        '*******************************************************************************
        ' to keep track if we can post messages
        '
        ' Parameters: none
        ' Returns: t or f
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        Get
            FormFirstVisible = mbFrmFirstVisible
        End Get
        Set(ByVal Value As Boolean)
            mbFrmFirstVisible = Value
        End Set

    End Property

    Friend Property TileMode() As eTile
        '*******************************************************************************
        ' This is set when user clicks the tile option buttons on the main form
        '           calls the arrange windows routine
        '
        ' Parameters: eTile mode
        ' Returns: eTile mode
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        Get
            ' default to cascade
            If (mnTile < eTile.Cascade) Or (mnTile > eTile.Vertical) Then
                mnTile = eTile.Cascade
            End If
            TileMode = mnTile
        End Get
        Set(ByVal Value As eTile)
            mnTile = Value
            ArrangeWindows(mnTile)
            'TODO - What's this all about?
            'If frmMain.tmrCheckTile.Enabled = False Then _
            '        frmMain.tmrCheckTile.Enabled = True
        End Set

    End Property

    Public Function CreatePWDesktop(ByRef MainForm As System.Windows.Forms.Form) As Boolean
        '*******************************************************************************
        ' This doesn't really create a desktop, it hides all the stuff we are not using
        ' we keep an array with all the hidden window handles  to show them when we are
        ' done
        '
        ' Parameters: None
        ' Returns: true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        Dim tMainFrmRect As RECT
        Dim pntScreenPoint As Point
        Dim recWorkingArea As Rectangle
        Dim hDesktop As Long

        CreatePWDesktop = False

        mhPWProcessID = GetCurrentProcessId()

        ''position frmMain
        'With frmMenuTest
        '    ' top of screen - maybe someday bottom of screen could be an option here
        '    .Move(0, 0, (Screen.Width), (.picToolBar.Height))
        '    Call GetWindowRect(.hWnd, tMainFrmRect)
        'End With
        With tMainFrmRect
            .Left = MainForm.Left
            .Top = MainForm.Top
            .Right = MainForm.Width + MainForm.Left
            .Bottom = MainForm.Height + MainForm.Top
        End With

        'get the size of the desktop
        'hDesktop = GetDesktopWindow()
        'Call GetWindowRect(hDesktop, mtPWDesktop)
        pntScreenPoint.X = 1
        pntScreenPoint.Y = 1
        recWorkingArea = Screen.GetWorkingArea(pntScreenPoint)

        With mtPWDesktop
            .Left = recWorkingArea.Left
            .Top = recWorkingArea.Top
            .Right = recWorkingArea.Right
            .Bottom = recWorkingArea.Bottom
        End With

        ' subtract the height if the main form
        mtPWDesktop.Top = _
                (mtPWDesktop.Top + (tMainFrmRect.Bottom - tMainFrmRect.Top))

        If OkToCreateDesktop Then
            mbDesktopExists = bHideNonPWApps()
        End If


        CreatePWDesktop = True

    End Function

    Public Function DestroyPWDesktop() As Boolean
        '*******************************************************************************
        ' show the windows that aren't a part of paintworks that we hid
        '
        ' Parameters: None
        ' Returns: true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        DestroyPWDesktop = False

        If mbDesktopExists Then
            mbDesktopExists = bRestoreHiddenWindows()
            DestroyPWDesktop = mbDesktopExists
        Else
            DestroyPWDesktop = True
        End If

    End Function

    Public Sub CheckWindowTile()
        '*******************************************************************************
        ' Called by timer. We dont get the messages of our child windows being closed
        ' so we use this to resize the windows if necessary
        '
        ' Parameters: None
        ' Returns: none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        Dim nTmpCount As Integer

        If mnAppsRunning < 2 Then Exit Sub

        nTmpCount = mnAppsRunning

        bRefreshWindowList()

        If (nTmpCount <> mnAppsRunning) Then
            ArrangeWindows(TileMode)
        End If

    End Sub

    Public Function ArrangeWindows(ByVal Method As eTile) As Boolean
        '*******************************************************************************
        ' tile or cascade our windows
        '
        ' Parameters: None
        ' Returns: true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        Dim nNumApps As Integer
        Dim nApp As Integer
        Dim lLeft As Long
        Dim lTop As Long
        Dim lWidth As Long
        Dim lHeight As Long
        Dim lResult As Long
        Dim lAdjust As Long

        On Error GoTo TileErr

        ArrangeWindows = True

        ' make sure we don't have invalid handles
        bRefreshWindowList()

        nNumApps = UBound(mPWApps)

        ' nothing to do
        If nNumApps < 1 Then Exit Function

        ' if 1 app we dont care about the method
        If nNumApps = 1 Then
            lLeft = mtPWDesktop.Left
            lTop = mtPWDesktop.Top
            lWidth = mtPWDesktop.Right - mtPWDesktop.Left
            lHeight = mtPWDesktop.Bottom - mtPWDesktop.Top

            lResult = MoveWindow(mPWApps(1).hWnd, lLeft, lTop, lWidth, lHeight, True)
            'repaint time
            Application.DoEvents()
            Exit Function

        End If  'nNumApps = 1

        Select Case Method
            Case eTile.Cascade
                lLeft = mtPWDesktop.Left
                lTop = mtPWDesktop.Top
                lWidth = mtPWDesktop.Right - mtPWDesktop.Left
                lHeight = mtPWDesktop.Bottom - mtPWDesktop.Top
                lAdjust = (GetSystemMetrics(SM_CYCAPTION) + GetSystemMetrics(SM_CYFRAME))

                lWidth = lWidth - (lAdjust * (nNumApps - 1))
                lHeight = lHeight - (lAdjust * (nNumApps - 1))

                For nApp = 1 To nNumApps
                    lResult = MoveWindow(mPWApps(nApp).hWnd, lLeft, lTop, lWidth, lHeight, True)
                    lTop = lTop + lAdjust
                    lLeft = lLeft + lAdjust
                Next 'nApp

            Case eTile.Horizontal
                lLeft = mtPWDesktop.Left
                lTop = mtPWDesktop.Top
                lWidth = mtPWDesktop.Right - mtPWDesktop.Left
                lAdjust = ((mtPWDesktop.Bottom - mtPWDesktop.Top) \ nNumApps)
                lHeight = lAdjust
                For nApp = 1 To nNumApps
                    lResult = MoveWindow(mPWApps(nApp).hWnd, lLeft, lTop, lWidth, lHeight, True)
                    lTop = lTop + lAdjust
                Next 'nApp

            Case eTile.Vertical
                lLeft = mtPWDesktop.Left
                lTop = mtPWDesktop.Top
                lAdjust = ((mtPWDesktop.Right - mtPWDesktop.Left) \ nNumApps)
                lHeight = mtPWDesktop.Bottom - mtPWDesktop.Top
                lWidth = lAdjust
                For nApp = 1 To nNumApps
                    lResult = MoveWindow(mPWApps(nApp).hWnd, lLeft, lTop, lWidth, lHeight, True)
                    lLeft = lLeft + lAdjust
                Next 'nApp

        End Select
        'repaint time
        Application.DoEvents()
        Exit Function

TileErr:
        Err.Clear()
        ArrangeWindows = False
    End Function

    Public Function StartPWApp(ByVal lpszCommandLine As String, ByVal dwFlags As Long) As Long
        '*******************************************************************************
        ' This routine is called to start a PW application
        '
        ' Parameters: None
        ' Returns: true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        Dim tSui As STARTUPINFO
        Dim tSa As SECURITY_ATTRIBUTES
        Dim nApp
        Dim objMousepointer As System.Windows.Forms.Cursor

        StartPWApp = 0

        On Error GoTo StartErr

        objMousepointer = frmMenuTest.MousePointer

        bRefreshWindowList()

        ' check for name
        If Len(lpszCommandLine) = 0 Then
            Call MessageBox(frmMain.hWnd, Localize.GetLocalString(PString.psMT_COMMANDLINE), _
                                 Localize.GetLocalString(PString.psSHORT_CAP), vbInformation)
            Exit Function
        End If

        ' got room for 1 more?
        If mnAppsRunning = mnMAX_APPS Then
            Call MessageBox(frmMain.hWnd, Localize.GetLocalString(PString.psMAX_PROG_RUNNING), _
                                 Localize.GetLocalString(PString.psSHORT_CAP), vbInformation)
            Exit Function
        End If

        ' check flags
        If (dwFlags And eStartupFlags.ExclusiveMode) = eStartupFlags.ExclusiveMode Then
            If mnAppsRunning <> 0 Then
                Call MessageBox(frmMain.hWnd, Localize.GetLocalString(PString.psEXCLUSIVE_MODE), _
                                     Localize.GetLocalString(PString.psSHORT_CAP), vbInformation)
                Exit Function
            End If
        Else
            'check if exclusive screen is already open
            If mnAppsRunning <> 0 Then
                For nApp = 1 To mnAppsRunning
                    If (mPWApps(nApp).flags And eStartupFlags.ExclusiveMode) = eStartupFlags.ExclusiveMode Then
                        Call MessageBox(frmMain.hWnd, Localize.GetLocalString(PString.psEXCLUSIVE_MODE), Localize.GetLocalString(PString.psSHORT_CAP), vbInformation)
                        Exit Function
                    End If
                Next
            End If
        End If

        If (dwFlags And eStartupFlags.AllowMultiStart) <> eStartupFlags.AllowMultiStart Then
            ' make sure its not already running
            For nApp = 1 To mnAppsRunning
                If lpszCommandLine = mPWApps(nApp).LaunchString Then
                    Call MessageBox(frmMain.hWnd, Localize.GetLocalString(PString.psPROG_RUNNING), _
                                 Localize.GetLocalString(PString.psSHORT_CAP), vbInformation)
                    Exit Function
                End If
            Next
        End If

        ' should be ok to start now
        frmMenuTest.MousePointer = System.Windows.Forms.Cursors.WaitCursor

        ' increment app counter - adjusted for closing in bRefreshWindowList
        mnAppsRunning = mnAppsRunning + 1
    ReDim Preserve mPWApps(1 To mnAppsRunning)

        mPWApps(mnAppsRunning).LaunchString = lpszCommandLine

        ' default security
        With tSa
            .nLength = Len(tSa)
            .bInheritHandle = 1&
            .lpSecurityDescriptor = 0&
        End With

        ' initialize startupinfo
        With tSui
            .cb = Len(tSui)
            .lpReserved = vbNullString
            .lpDesktop = vbNullString
            .lpTitle = vbNullString
            ' start in the corner of virtural desktop - this is voided by code in form itself...
            .dwX = mtPWDesktop.Left
            .dwY = mtPWDesktop.Top
            .dwXSize = mtPWDesktop.Right - mtPWDesktop.Left
            .dwYSize = mtPWDesktop.Bottom - mtPWDesktop.Top
            .dwFlags = STARTF_USESHOWWINDOW Or STARTF_USEPOSITION Or STARTF_USESIZE
            .wShowWindow = SW_NORMAL
        End With

        If bStartApp(lpszCommandLine, tSa, tSui, mPWApps(mnAppsRunning).lpProcInfo) = False Then

            'start failed
            Call MessageBox(frmMain.hWnd, Localize.GetLocalString(PString.psCOULD_NOT_START _
                                    & vbCrLf & lpszCommandLine), _
                                    Localize.GetLocalString(PString.psSHORT_CAP), vbCritical)

        Else
            ' this is the hwnd found in the callback function
            mPWApps(mnAppsRunning).hWnd = mhHandleHolder
            mPWApps(mnAppsRunning).flags = dwFlags
        End If



        ' re-arrange windows
        If mnAppsRunning = 1 Then
            ' we want to use the whole screen, tile horz
            Call ArrangeWindows(Horizontal)
        Else
            'use whatever is selected
            Call ArrangeWindows(TileMode)
        End If

ExitHere:
        frmMenuTest.MousePointer = objMousepointer
        Exit Function

StartErr:

        GoTo ExitHere

    End Function

    Public Function StartOtherApp(ByVal lpszCommandLine As String, ByVal dwFlags As Long, _
                                                         ByVal bVerbose As Boolean) As Long
        '*******************************************************************************
        ' This routine is called to start an application not tied to the desktop
        '
        ' Parameters: command line and windowstyle flags
        ' Returns: true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        Dim tSui As STARTUPINFO
        Dim tSa As SECURITY_ATTRIBUTES
        Dim nPtr As Integer
        Dim nMouse As Integer

        StartOtherApp = 0

        nMouse = Screen.MousePointer

        On Error GoTo StartErr

        ' check for name
        If Len(lpszCommandLine) = 0 Then
            If bVerbose Then
                Call MessageBox(frmMain.hWnd, Localize.GetLocalString(PString.psMT_COMMANDLINE), _
                                     Localize.GetLocalString(PString.psSHORT_CAP), vbInformation)
            End If

            Exit Function

        End If

        ' should be ok to start now
        frmMenuTest.MousePointer = System.Windows.Forms.Cursors.WaitCursor 'hourglass
        nPtr = UBound(mOtherApps) + 1

        ReDim Preserve mOtherApps(nPtr)

        mOtherApps(nPtr).LaunchString = lpszCommandLine

        ' default security
        With tSa
            .nLength = Len(tSa)
            .bInheritHandle = 1&
            .lpSecurityDescriptor = 0&
        End With

        ' initialize startupinfo
        With tSui
            .cb = Len(tSui)
            .lpReserved = vbNullString
            .lpDesktop = vbNullString
            .lpTitle = vbNullString
            ' start in the corner of virtural desktop - this is voided by code in form itself...
            .dwFlags = STARTF_USESHOWWINDOW

            Select Case dwFlags
                Case vbHide
                    .wShowWindow = SW_HIDE
                Case vbNormalFocus
                    .wShowWindow = SW_NORMAL
                Case vbMinimizedFocus
                    .wShowWindow = SW_SHOWMINIMIZED
                Case vbMaximizedFocus
                    .wShowWindow = SW_SHOWMAXIMIZED
                Case vbNormalNoFocus
                    .wShowWindow = SW_SHOWNA
                Case vbMinimizedNoFocus
                    .wShowWindow = SW_SHOWMINNOACTIVE
                Case Else
                    .wShowWindow = SW_NORMAL
            End Select

        End With

        If bStartApp(lpszCommandLine, tSa, tSui, mOtherApps(nPtr).lpProcInfo) = False Then

            'start failed
            If bVerbose Then
                Call MessageBox(frmMain.hWnd, "*" & Localize.GetLocalString(PString.psCOULD_NOT_START), _
                                    Localize.GetLocalString(PString.psSHORT_CAP), vbCritical)
            End If
        Else
            ' this is the hwnd found in the callback function
            mOtherApps(nPtr).hWnd = mhHandleHolder
            StartOtherApp = mhHandleHolder
        End If

ExitHere:
        Screen.MousePointer = nMouse
        Exit Function

StartErr:

        GoTo ExitHere

    End Function


    Private Function bStartApp(ByVal lpszCommandLine As String, _
                                ByRef tSa As SECURITY_ATTRIBUTES, _
                                ByRef tSui As STARTUPINFO, _
                                ByRef lpProcessInfo As PROCESS_INFORMATION) As Boolean
        '*******************************************************************************
        ' Start an application
        '
        ' Parameters: None
        ' Returns: true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        Dim lStartTic As Long
        Dim bFound As Boolean

        bFound = False
        mhHandleHolder = 0

    If CreateProcess(vbNullString, lpszCommandLine, tSa, tSa, True, DETACHED_PROCESS, _
                        ByVal 0&, vbNullString, tSui, lpProcessInfo) = 0 Then
            bStartApp = False
            Call pw3api.LogInternalError("PW3_MAIN", "mPWdesktop", "bStartapp", Err.LastDllError, _
                            "Createprocess failed on: " & lpszCommandLine)
            Exit Function
        End If

        'wait for new apps message queue to be clear
        Call WaitForInputIdle(lpProcessInfo.hProcess, 7000)

        lStartTic = GetTickCount()

        Do While ((Abs(GetTickCount() - lStartTic) < mlWAITFORWINDOWMS))
            DoEvents()
            ' if window is found callback returns false which makes enumwindows return false
            If (EnumWindows(AddressOf bFindWindowCallback, _
                                        lpProcessInfo.dwProcessId) = 0) Then
                bFound = True
                Exit Do
            End If
        Loop

        If Not bFound Then
            Call pw3api.LogInternalError("PW3_MAIN", "mPWdesktop", "bStartapp", 0, _
                             lpszCommandLine & " started, main window not found. processID = " & _
                             lpProcessInfo.dwProcessId)
        End If

        bStartApp = True

    End Function

    Private Function bHideNonPWApps() As Boolean
        '*******************************************************************************
        ' hide the windows that aren't a part of paintworks
        '
        ' Parameters: None
        ' Returns: true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        bHideNonPWApps = False

        If EnumWindows(AddressOf bHideWindowsCallback, mhPWProcessID) <> 0 Then
            bHideNonPWApps = True
        End If

    End Function

    Private Function bHideWindowsCallback(ByVal hWnd As Long, ByVal lParam As Long) As Boolean
        '*******************************************************************************
        ' This is a callback function for EnumWindows
        '
        ' Parameters: hWnd - handle to window
        '             lParam - Application defined parameter
        '
        ' Returns: true to keep enumerating
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        Dim nApp
        Dim lProc As Long
        Dim nPtr As Integer
        '    Dim lStrLen As Long
        '    Dim sName As String
        '    Dim lpWindowTitle As String * 255
        '
        '    'for debug
        '    sName = vbNullString
        '    lStrLen = GetWindowText(hWnd, lpWindowTitle, Len(lpWindowTitle))
        '    If lStrLen > 0 Then
        '         sName = Left$(lpWindowTitle, lStrLen)
        '    End If

        ' is window already hidden?
        If IsWindowVisible(hWnd) = 0 Then
            bHideWindowsCallback = True
            Exit Function
        End If

        ' is it a paintworks app window?
        For nApp = 1 To UBound(mPWApps)
            If hWnd = mPWApps(nApp).hWnd Then
                ' its ours
                bHideWindowsCallback = True
                Exit Function
            End If
        Next 'nApp

        'Pw Main created window?
        Call GetWindowThreadProcessId(hWnd, lProc)
        If lProc = mhPWProcessID Then
            bHideWindowsCallback = True
            Exit Function
        End If

        ' hide it
        nPtr = UBound(mhHiddenWindows) + 1
        ReDim Preserve mhHiddenWindows(nPtr)
        mhHiddenWindows(nPtr) = hWnd

        Call ShowWindow(hWnd, SW_HIDE)

        bHideWindowsCallback = True

    End Function

    Private Function bFindWindowCallback(ByVal hWnd As Long, ByVal lParam As Long) As Boolean
        '*******************************************************************************
        ' This is a callback function for EnumWindows
        '
        ' Parameters: hWnd - handle to window
        '             lParam - Application defined parameter
        '
        ' Returns: true to keep enumerating
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        Dim lpdwProcessId As Long
        Dim lpRect As RECT

        If GetWindowThreadProcessId(hWnd, lpdwProcessId) Then
            If lpdwProcessId = lParam Then
                'were getting warm
                If ((IsWindowVisible(hWnd) <> 0) And (IsIconic(hWnd) = 0)) Then
                    'make sure rect is valid
                    GetWindowRect(hWnd, lpRect)
                    If (lpRect.Right > lpRect.Left) And (lpRect.Bottom > lpRect.Top) Then
                        'should be the one!
                        mhHandleHolder = hWnd
                        ' stop enumerating
                        bFindWindowCallback = False
                        Exit Function
                    End If
                End If
            End If  'lpdwProcessId = lParam
        End If

        bFindWindowCallback = True

    End Function

    Private Function bRestoreHiddenWindows() As Boolean
        '*******************************************************************************
        ' restore the hidden windows that aren't a part of paintworks
        '
        ' Parameters: None
        ' Returns: true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        Dim hTmp As Long

        For Each hTmp In mhHiddenWindows
            If (hTmp <> 0) And (IsWindow(hTmp)) Then
                Call ShowWindow(hTmp, SW_SHOWNA)
            End If
        Next

        ReDim mhHiddenWindows(0)

    End Function

    Private Function bRefreshWindowList() As Boolean
        '*******************************************************************************
        ' update our window list. This is recursive. is recursive.
        '
        ' Parameters: None
        ' Returns: true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        Dim nApp, nNextApp
        Dim nCount As Integer

        bRefreshWindowList = False

        nCount = UBound(mPWApps)

        For nApp = 1 To nCount

            If mPWApps(nApp).hWnd <> 0 Then

                If IsWindow(mPWApps(nApp).hWnd) Then
                    ' still open
                Else
                    ' window was closed - shift all windows up one
                    If (nApp + 1) <= nCount Then
                        For nNextApp = (nApp + 1) To nCount
                            mPWApps(nNextApp - 1) = mPWApps(nNextApp)
                        Next 'nNextApp
                    End If  '(nApp + 1) < nCount

                    If nCount > 1 Then
                        ' Stop leak for memory
                        Call CloseHandle(mPWApps(nCount).lpProcInfo.hProcess)
                        Call CloseHandle(mPWApps(nCount).lpProcInfo.hThread)
                        'shrink array by 1
                        ReDim Preserve mPWApps(nCount - 1)
                    Else
                        ' all gone
                        mPWApps(1).hWnd = 0
                    End If

                    ' check again
                    bRefreshWindowList()

                End If

            End If  'mPWApps(nApp).hWnd <> 0

            'in case we resized
            If nApp >= UBound(mPWApps) Then Exit For

        Next 'nApp

        ' adjust app counter
        If mPWApps(1).hWnd = 0 Then
            mnAppsRunning = 0
        Else
            mnAppsRunning = UBound(mPWApps)
        End If

        bRefreshWindowList = True

    End Function

    Public Function CloseAllPWApps() As Boolean
        '*******************************************************************************
        ' Close any pw apps we have started by the messiest method possible. The theory
        '   is that we are going to reboot after cleanup anyway.
        '
        ' Parameters: None
        ' Returns: true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        Dim i%

        On Error GoTo CloseErr

        CloseAllPWApps = True

        For i% = 1 To UBound(mPWApps)
            If mPWApps(i%).hWnd <> 0 Then

                If IsWindow(mPWApps(i%).hWnd) Then
                    Call TerminateProcess(mPWApps(i%).lpProcInfo.hProcess, 0&)
                End If

                Call CloseHandle(mPWApps(i%).lpProcInfo.hProcess)
                Call CloseHandle(mPWApps(i%).lpProcInfo.hThread)
            End If
        Next

        For i% = 1 To UBound(mOtherApps)
            If mOtherApps(i%).hWnd <> 0 Then

                If IsWindow(mOtherApps(i%).hWnd) Then
                    Call TerminateProcess(mOtherApps(i%).lpProcInfo.hProcess, 0&)
                End If

                Call CloseHandle(mOtherApps(i%).lpProcInfo.hProcess)
                Call CloseHandle(mOtherApps(i%).lpProcInfo.hThread)
            End If
        Next

        Exit Function

CloseErr:
        CloseAllPWApps = False
        Resume Next
    End Function

    Public Function CloseAppByTitle(ByVal WindowTitle As String) As Boolean
        '*******************************************************************************
        ' Close an app given the window title. this is for things pwmain has started
        '
        ' Parameters: name of the window
        ' Returns: true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        Dim void As Long

        On Error GoTo CloseErr

        CloseAppByTitle = True

        msWindowTitle = WindowTitle

        Call EnumWindows(AddressOf bDestroyWindowCallback, void)

        DoEvents()

        Exit Function

CloseErr:
        CloseAppByTitle = False

    End Function

    Private Function bDestroyWindowCallback(ByVal hWnd As Long, ByVal lParam As Long) As Boolean
        '*******************************************************************************
        ' This is a callback function for EnumWindows
        '
        ' Parameters: hWnd - handle to window
        '             lParam - Application defined parameter
        '
        ' Returns: true to keep enumerating
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
    Dim sTmp As String * 255
        Dim nLen As Integer
        Dim sName As String

        bDestroyWindowCallback = True

        sTmp = Space(255)
        nLen = GetWindowText(hWnd, sTmp, Len(sTmp))

        If nLen > 0 Then
            sName = Left$(sTmp, nLen)
            If sName = msWindowTitle Then
                ' this is the one -  if user hasnt saved data by now its too late...
                DestroyWindow(hWnd)
                ' stop enumerating
                bDestroyWindowCallback = False
            End If  '
        End If  'nLen > 0

    End Function

    Private Function bWindowFromTitleCallback(ByVal hWnd As Long, ByVal lParam As Long) As Boolean
        '*******************************************************************************
        ' This is a callback function for EnumWindows
        '
        ' Parameters: hWnd - handle to window
        '             lParam - Application defined parameter
        '
        ' Returns: true to keep enumerating
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
    Dim sTmp As String * 255
        Dim nLen As Integer
        Dim sName As String

        bWindowFromTitleCallback = True

        sTmp = Space(255)
        nLen = GetWindowText(hWnd, sTmp, Len(sTmp))

        If nLen > 0 Then
            sName = Left$(sTmp, nLen)
            Debug.Print(sName)
            If sName = msWindowTitle Then
                ' this is the one -
                mhHandleHolder = hWnd
                ' stop enumerating
                bWindowFromTitleCallback = False
            End If  '
        End If  'nLen > 0

    End Function

    Public Function WaitForWindowByTitle(ByVal WindowTitle As String, _
                                            ByVal SecondstoWait As Integer) As Boolean
        '*******************************************************************************
        ' Wait for an application window to be ready If there is more than 1 window with
        ' the same title this stops at the first one it finds
        '
        ' Parameters: Window title
        '
        ' Returns: true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        Dim lMs As Long
        Dim hProcessId As Long
        Dim void As Long

        lMs = SecondstoWait * 1000
        msWindowTitle = WindowTitle

        WaitForWindowByTitle = False

        If mbFrmFirstVisible Then
            pw3api.PostStatusMsg(Localize.GetLocalString(psCHECK_FOR_PROGRAM) & _
                                                WindowTitle, frmFirst.lstDebugOutput)
        End If

        On Error GoTo WaitErr

        ' if window is found callback returns false which makes enumwindows return false
        If EnumWindows(AddressOf bWindowFromTitleCallback, void) = 0 Then

            If GetWindowThreadProcessId(mhHandleHolder, hProcessId) <> 0 Then

                Call WaitForInputIdle(hProcessId, lMs)
                WaitForWindowByTitle = True

            End If

        End If

        Exit Function
WaitErr:

    End Function

    Public Function WaitForService(ByVal sName As String, _
                                            ByVal SecondstoWait As Integer) As Boolean
        '*******************************************************************************
        ' Wait for a service to be running
        ' the names can be found at HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services
        '
        ' Parameters: service name , how long to wait
        '
        ' Returns: true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        Dim lReturn As Long
        Dim sErrorMsg As String
        Dim lError As Long
        Dim hSCManager As Long
        Dim hService As Long
        Dim lpServiceStatus As SERVICE_STATUS
        Dim dStart As Date

        On Error GoTo ErrorHandler

        WaitForService = False

        If mbFrmFirstVisible Then
            pw3api.PostStatusMsg(Localize.GetLocalString(psCHECK_FOR_SERVICE) & sName, frmFirst.lstDebugOutput)
        End If

        hSCManager = OpenSCManager(vbNullString, vbNullString, SC_MANAGER_CONNECT)
        'call failed ?
        If hSCManager = 0 Then Exit Function

        hService = OpenService(hSCManager, sName, GENERIC_READ)
        'call failed ?
        If hService = 0 Then
            CloseServiceHandle(hSCManager)
            Exit Function
        End If

        dStart = Now

        Do While ((DateDiff(DateInterval.Second, dStart, Now) < SecondstoWait))

            lReturn = QueryServiceStatus(hService, lpServiceStatus)

            If lReturn <> 0 Then
                If lpServiceStatus.dwCurrentState = SERVICE_RUNNING Then
                    WaitForService = True
                    Exit Do
                End If
            End If

        Loop

        CloseServiceHandle(hSCManager)

        Exit Function

ErrorHandler:

    End Function

    Public Function HelpWindows(ByVal WhatToDo As eHelpWin) As Boolean
        '*******************************************************************************
        ' Hide or remove the help (searchdisk and IE) windows
        '
        ' Parameters: None
        ' Returns: true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
        Dim lStartTic As Long

        HelpWindows = False
        On Error GoTo HelpErr

        Call EnumWindows(AddressOf bHelpWindowCallback, WhatToDo)

        HelpWindows = True
        Exit Function

HelpErr:
        pw3api.LogInternalError("main", "mPWDesktop", "helpwindows", Err.Number, _
                        Err.Description & " whattodo = " & WhatToDo)
    End Function

    Private Function bHelpWindowCallback(ByVal hWnd As Long, ByVal lParam As Long) As Boolean
        '*******************************************************************************
        ' This is a callback function for EnumWindows
        '
        ' Parameters: hWnd - handle to window
        '             lParam - Application defined parameter
        '
        ' Returns: true to keep enumerating
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************
    Dim sTmp As String * 255
        Dim nLen As Integer
        Dim lReturnValue As Long
        Dim lProcess As Long
        Dim lProcessID As Long
        Dim sName As String

        Const sSEARCHDISK As String = "Exit SearchDisc"
        Const sIE As String = "FANUC Robotics CDROM"


        sTmp = Space(255)
        nLen = GetWindowText(hWnd, sTmp, Len(sTmp))
        sName = Left$(sTmp, nLen)

        If nLen > 0 Then

            Select Case lParam
                Case eHelpWin.Destroy
                    If (sName = sSEARCHDISK) Or _
                            (sIE = Left$(sName, Len(sIE))) Then
                        lReturnValue = GetWindowThreadProcessId(hWnd, lProcessID)
                        lProcess = OpenProcess(PROCESS_ALL_ACCESS, 0&, lProcessID)
                        lReturnValue = TerminateProcess(lProcess, 0&)
                        lReturnValue = CloseHandle(lProcess)
                        lReturnValue = CloseHandle(lProcessID)
                    End If
                    mhSearchdisk = 0
                    mhIE = 0
                Case eHelpWin.Hide
                    If sName = sSEARCHDISK Then
                        'you wouldn't think you would need to set the repaint true, but if you
                        ' dont, it will leave pecker tracks on your vb window....
                        Call MoveWindow(hWnd, 20000, 0, 500, 500, True)
                        mhSearchdisk = hWnd
                        If mbFrmFirstVisible Then
                            pw3api.PostStatusMsg(sSEARCHDISK & " " & Localize.GetLocalString(psFOUND), _
                                            frmFirst.lstDebugOutput)
                        End If
                    End If

                    If (sIE = Left$(sName, Len(sIE))) Then
                        Call MoveWindow(hWnd, 20000, 0, 500, 500, True)
                        mhIE = hWnd
                        If mbFrmFirstVisible Then
                            pw3api.PostStatusMsg(sIE & " " & Localize.GetLocalString(psFOUND), _
                                            frmFirst.lstDebugOutput)
                        End If
                    End If

            End Select

        End If  'nLen > 0

        ' just keep enumerating - there may be more than 1 window with the title we want
        bHelpWindowCallback = True

    End Function


    Public Sub RestartWindows(Optional ByVal LogoffOnly As Boolean = False)
        '*******************************************************************************
        ' reboot the confuser
        '
        ' Parameters: maybe someday we will just logout and log back in...
        '
        ' Returns: (many happy...)
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************

        Const TOKEN_ADJUST_PRIVILEGES = &H20
        Const TOKEN_QUERY = &H8
        Const SE_PRIVILEGE_ENABLED = &H2
        Const SE_SHUTDOWN_NAME = "SeShutdownPrivilege"
        Dim hProcessHandle As Long
        Dim hTokenHandle As Long
        Dim tmpLuid As LUID
        Dim tkp As TOKEN_PRIVILEGES
        Dim tkpPreviousState As TOKEN_PRIVILEGES
        Dim lSizeOfPriv As Long
        Dim lRet As Long

        hProcessHandle = GetCurrentProcess()

        lRet = OpenProcessToken(hProcessHandle, (TOKEN_ADJUST_PRIVILEGES Or _
                                            TOKEN_QUERY), hTokenHandle)

        ' Get the LUID for shutdown privilege.
        lRet = LookupPrivilegeValue(vbNullString, SE_SHUTDOWN_NAME, tmpLuid)

        tkp.PrivilegeCount = 1    ' One privilege to set
        tkp.Privileges(0).pLuid = tmpLuid
        tkp.Privileges(0).Attributes = SE_PRIVILEGE_ENABLED

        ' Enable the shutdown privilege in the access token of this process.
        lRet = AdjustTokenPrivileges(hTokenHandle, False, _
                    tkp, Len(tkpPreviousState), tkpPreviousState, lSizeOfPriv)

        'say goodnight dick... "goodnight dick"
        lRet = ExitWindowsEx((EWX_SHUTDOWN Or EWX_FORCE Or EWX_REBOOT), &HFFFF)

    End Sub


    Public Function WindowHandleFromTitle(ByVal WindowTitle As String) As Long
        '********************************************************************************************
        'Description: Get the handle to a (top level) window with the requested title. Partial titles
        '               will work. If there are more than 1 window with the same name, this returns the
        '               first one it finds
        '
        'Parameters:    Title string to find
        'Returns:       Handle or 0 if not found
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim void As Long

        WindowHandleFromTitle = 0

        If Len(WindowTitle) = 0 Then Exit Function

        msWindowTitle = WindowTitle
        mhHandleHolder = 0

        If EnumWindows(AddressOf bFindWindowByTitleCallback, void) = 0 Then
            ' 0 indicates failure
        End If

        WindowHandleFromTitle = mhHandleHolder

    End Function

    Private Function bFindWindowByTitleCallback(ByVal hWnd As Long, _
                                                        ByVal lParam As Long) As Long
        '********************************************************************************************
        'Description: This is the callback function for EnumWindows.
        '
        'Parameters: lParam could be set in calling routine and used in a case statment
        '       if we wanted this to do different things
        '
        'Returns:
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim lStrLen As Long
        Dim sName As String
   Dim lpWindowTitle As String * 255

        lStrLen = GetWindowText(hWnd, lpWindowTitle, Len(lpWindowTitle))
        If lStrLen > 0 Then
            sName = Left$(lpWindowTitle, lStrLen)
            'Debug.Print sName
            If Len(sName) >= Len(msWindowTitle) Then
                sName = Left$(sName, Len(msWindowTitle))
                If StrComp(sName, msWindowTitle, vbTextCompare) = 0 Then
                    mhHandleHolder = hWnd
                    bFindWindowByTitleCallback = False
                    Exit Function
                End If
            End If
        End If

        bFindWindowByTitleCallback = True

    End Function

    Public Sub TimerProc(ByVal hWnd As Long, ByVal uMsg As Long, _
                         ByVal idEvent As Long, ByVal dwTime As Long)
        '********************************************************************************************
        'Description: Callback for SetTimer function
        '
        'Parameters:
        'Returns:
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        gbTimerDone = True

    End Sub


    Public Function SetMouseHook() As Boolean
        '********************************************************************************************
        'Description: Call this (once) to set hook for current thread
        '
        'Parameters:    None
        'Returns:   true if hook is set
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************


        SetMouseHook = False

        ghHook = SetWindowsHookEx(WH_MOUSE, AddressOf MouseProc, 0&, App.ThreadID)

        If ghHook <> 0 Then SetMouseHook = True

    End Function

    Public Function ReleaseMouseHook() As Boolean
        '********************************************************************************************
        'Description: Call to release hook. Don't forget!
        '
        'Parameters:    None
        'Returns:   None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        On Error Resume Next

        Call UnhookWindowsHookEx(ghHook)

    End Function

    Private Function MouseProc(ByVal nCode As Long, ByVal wParam As Long, ByRef lParam As MOUSEHOOKSTRUCT) As Long
        '********************************************************************************************
        'Description: This function takes the place of the windows MouseProd function
        '               Add other mouse massaging here if desired.
        '
        '           Note: according to the documentation, breakpoints should not be inserted in this routine,
        '               if the window receives another message, you will crash and burn
        '
        'Parameters:    wParam is the mouse message.
        'Returns:   1 to discard message, or passes wParam to next hook.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If nCode >= 0 Then
            'Discard all  Mouse  messages
            MouseProc = 1
            Exit Function
        End If

        MouseProc = CallNextHookEx(ghHook, nCode, wParam, lParam)

    End Function


End Module
