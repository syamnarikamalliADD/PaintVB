'
' This material is the joint property of FANUC Robotics North America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics North America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2002
' FANUC Robotics North America
' FANUC LTD Japan
'
' Form/Module: mPWDesktop.bas
'
' Dependencies: None
'
' Language: Microsoft Visual Basic
'
' Description: PAINTworks "desktop" routines - replaces frwm32.dll
'
' Author: Speedy
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.  48309-3253
'
' Modification history:
'
' Date       By        Reason
' 07/08/02             Initial Release
' 11/5/04   gks        Minor tweeks
' 04/07/09  rjo        .NET Conversion
' 03/14/12  rjo        Added RunningApps Property to support Password.NET
' 12/13/12  MSW        mPWDesktop.StartPWApp - Tweak multistart feature to take        4.01.04.00
'                      advantage of single instance logic in browser
' 12/03/13  MSW        Deal with screen start failures                                 4.01.06.01
' 05/05/14  MSW        mPWDesktop - StartPWApp - find hidden windows                   4.01.07.02
'********************************************************************************************
' Declarations
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports System.Reflection

Friend Module mPWDesktop

#Region " Declares "

    '***** tweekable stuff here *****************************************************************
    Private Const mnMAX_APPS As Int32 = 3             ' how many pwapps can we run?
    Private Const mlWAITFORWINDOWMS As Int32 = 10000  ' miliseconds for timeout
    '********************************************************************************************

    Private mbFrmFirstVisible As Boolean
    Private mbDeskTopOk As Boolean

    'Used on the change Log Stuff
    '10/18/07
    'Public gsScreenNameChngLog As String
    'Public gsSCREEN_CAPTION As String
    'Public gsPWRoot As String
    'Public Const gsSCREEN_NAME As String = "Main"

    Private mhSearchdisk As Int32                    ' searchdisk window handle
    Private mhIE As Int32                            ' internet explorer handle
    Private mhAlarmMan As Int32                      ' AlarmMan window handle

    'type definitions
    Public Structure RECT                               ' square stuff
        Public Left As Int32
        Public Top As Int32
        Public Right As Int32
        Public Bottom As Int32
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure PROCESS_INFORMATION
        Public hProcess As Integer
        Public hThread As Integer
        Public dwProcessID As Integer
        Public dwThreadID As Integer
    End Structure 'PROCESS_INFORMATION                  'for CreateProcess

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure SECURITY_ATTRIBUTES
        Public nLength As Integer
        Public lpSecurityDescriptor As IntPtr
        Public bInheritHandle As Boolean
    End Structure 'SECURITY_ATTRIBUTES                  'for CreateProcess?

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure STARTUPINFO
        Public cb As Integer
        Public lpReserved As String
        Public lpDesktop As String
        Public lpTitle As String
        Public dwX As Integer
        Public dwY As Integer
        Public dwXSize As Integer
        Public dwYSize As Integer
        Public dwXCountChars As UInteger
        Public dwYCountChars As UInteger
        Public dwFillAttribute As UInteger
        Public dwFlags As UInteger
        Public wShowWindow As Short
        Public cbReserved2 As Short
        Public lpReserved2 As IntPtr
        Public hStdInput As IntPtr
        Public hStdOutput As IntPtr
        Public hStdError As IntPtr
    End Structure 'STARTINFO                            'for CreateProcess

    Private Structure PWApp                             'structure to keep track of our apps
        Public hwnd As Int32
        Public flags As Int32
        Public LaunchString As String
        Public lpProcInfo As PROCESS_INFORMATION
    End Structure

    Private mPWApps() As PWApp                      ' to keep track of our child apps
    Private mOtherApps() As PWApp                   ' to keep track of apps not part of desktop eg. alarmman

    Private mnTile As eTile                         ' property holder

    Private mhPWProcessID As Int32                  ' id of pwmain
    Private mbDesktopExists As Boolean              ' set when we hide the apps
    Private mhHiddenWindows() As Int32              ' non - pw apps we have hidden
    Private mnAppsRunning As Int32                  ' number of pw apps running

    Private mtPWDesktop As RECT                     ' used to size pwapps - what's left of
    ' the screen after frmmain
    Private msWindowTitle As String                 ' String holder, can't pass a string to callback
    Private mhHandleHolder As Int32                 ' used to hold handle found in callback

    ' constants for setwindowpos
    Friend Const HWND_TOPMOST As Int32 = -1
    Friend Const HWND_NOTOPMOST As Int32 = -2

    ' constants for showwindow
    Friend Const SW_HIDE As Int32 = 0
    Friend Const SW_RESTORE As Int32 = 9
    Friend Const SW_SHOWNA As Int32 = 8
    Friend Const SW_NORMAL As Int32 = 1
    Friend Const SW_SHOWMAXIMIZED As Int32 = 3
    Friend Const SW_SHOWMINIMIZED As Int32 = 2
    Friend Const SW_SHOWMINNOACTIVE As Int32 = 7


    ' constants for startupinfo
    Private Const STARTF_FORCEOFFFEEDBACK As Int32 = &H80&
    Private Const STARTF_FORCEONFEEDBACK As Int32 = &H40&
    Private Const STARTF_USEPOSITION As Int32 = &H4&
    Private Const STARTF_USESHOWWINDOW As Int32 = &H1&
    Private Const STARTF_USESIZE As Int32 = &H2&
    Private Const CREATE_NEW_CONSOLE As Int32 = &H10&
    Private Const DETACHED_PROCESS As Int32 = &H8&
    Private Const NORMAL_PRIORITY_CLASS As Int32 = &H20&
    ' constants for getsystemmetrics
    Private Const SM_CYCAPTION As Int32 = 4
    Private Const SM_CYFRAME As Int32 = 33

    Private Const PROCESS_ALL_ACCESS As Int32 = &H1F0FFF

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
        ExclusiveMode = &H40& '64
        AllowMultiStart = &H100& '256
        MultiStartSingleInstance = &H300& '768
    End Enum
    'callback delegate 07/09/08
    Friend Delegate Function EnumWindowsCallbackDelegate(ByVal hwnd As Int32, ByVal lParam As Int32) As Boolean

    'winapi calls
    Private Declare Function CloseHandle Lib "kernel32" (ByVal hObject As Int32) As Int32
    <DllImport("kernel32.dll")> _
    Function CreateProcess(ByVal lpApplicationName As String, ByVal lpCommandLine As String, _
                           ByVal lpProcessAttributes As IntPtr, ByVal lpThreadAttributes As IntPtr, _
                           ByVal bInheritHandles As Boolean, ByVal dwCreationFlags As UInteger, _
                           ByVal lpEnvironment As IntPtr, ByVal lpCurrentDirectory As String, _
                           ByRef lpStartupInfo As STARTUPINFO, ByRef lpProcessInformation As PROCESS_INFORMATION) As Boolean
    End Function
    Friend Declare Function DestroyWindow Lib "user32" (ByVal hwnd As Int32) As Int32
    Private Declare Function EnumWindows Lib "user32" (ByVal lpEnumFunc As EnumWindowsCallbackDelegate, _
                                                       ByVal lParam As Int32) As Int32
    Friend Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As Int32, _
                                                    ByVal lpWindowName As String) As Int32
    Private Declare Function GetCurrentProcessId Lib "kernel32" () As Int32
    Private Declare Function GetDesktopWindow Lib "user32" () As Int32
    Private Declare Function GetSystemMetrics Lib "user32" (ByVal nIndex As Int32) As Int32
    Private Declare Function GetTickCount Lib "kernel32" () As Int32
    Private Declare Function GetWindowRect Lib "user32" (ByVal hwnd As Int32, _
                                                         ByRef lpRect As RECT) As Int32
    Private Declare Function GetWindowText Lib "user32" Alias "GetWindowTextA" _
                            (ByVal hwnd As Int32, ByVal lpString As String, ByVal cch As Int32) As Int32
    Private Declare Function GetWindowThreadProcessId Lib "user32" (ByVal hwnd As Int32, _
                                                                    ByRef lpdwProcessId As Int32) As Int32
    Private Declare Function IsIconic Lib "user32" (ByVal hwnd As Int32) As Int32
    Private Declare Function IsWindow Lib "user32" (ByVal hwnd As Int32) As Int32
    Private Declare Function IsWindowVisible Lib "user32" (ByVal hwnd As Int32) As Int32
    Private Declare Function MoveWindow Lib "user32" (ByVal hwnd As Int32, _
                                                      ByVal X As Int32, ByVal y As Int32, _
                                                      ByVal nWidth As Int32, ByVal nHeight As Int32, _
                                                      ByVal bRepaint As Boolean) As Int32
    Private Declare Function OpenProcess Lib "kernel32" (ByVal dwDesiredAccess As Int32, _
                                                         ByVal bInheritHandle As Int32, _
                                                         ByVal dwProcessID As Int32) As Int32
    Friend Declare Function PostMessage Lib "user32" Alias "PostMessageA" (ByVal hwnd As Int32, _
                                                                            ByVal wMsg As Int32, _
                                                                            ByVal wParam As Int32, _
                                                                            ByVal lParam As Int32) As Int32
    Friend Declare Sub SetWindowPos Lib "user32" (ByVal hwnd As Int32, ByVal hWndInsertAfter As Int32, _
                                                  ByVal X As Int32, ByVal y As Int32, ByVal cx As Int32, _
                                                  ByVal cy As Int32, ByVal wFlags As Int32)

    Friend Declare Function ShowWindow Lib "user32" (ByVal hwnd As Int32, _
                                                     ByVal nCmdShow As Int32) As Int32
    Private Declare Function TerminateProcess Lib "kernel32" (ByVal hProcess As Int32, _
                                                              ByVal uExitCode As Int32) As Int32
    Private Declare Function WaitForInputIdle Lib "user32" (ByVal hProcess As Int32, _
                                                            ByVal dwMilliseconds As Int32) As Int32
    Private Declare Function WaitForSingleObject Lib "kernel32" (ByVal hHandle As Int32, _
                                                                 ByVal dwMilliseconds As Int32) As Int32


    'shutdown windows stuff
    Private Const ANYSIZE_ARRAY As Int32 = 1
    Private Const EWX_FORCE As Int32 = 4
    Private Const EWX_LOGOFF As Int32 = 0
    Private Const EWX_REBOOT As Int32 = 2
    Private Const EWX_SHUTDOWN As Int32 = 1
    Private Const WM_CLOSE As Int32 = &H10

    Friend Const WM_QUIT As Int32 = &H12
    Friend Const BLANK As Int32 = 0&


    Private Structure LUID
        Public lowpart As Int32
        Public highpart As Int32
    End Structure

    Private Structure LUID_AND_ATTRIBUTES
        Public pLuid As LUID
        Public Attributes As Int32
    End Structure

    Private Structure TOKEN_PRIVILEGES
        Public PrivilegeCount As Int32
        Public Privileges() As LUID_AND_ATTRIBUTES
    End Structure

    Private Declare Function AdjustTokenPrivileges Lib "advapi32.dll" (ByVal TokenHandle As Int32, _
                                    ByVal DisableAllPrivileges As Int32, ByVal NewState As TOKEN_PRIVILEGES, _
                                    ByVal BufferLength As Int32, ByVal PreviousState As TOKEN_PRIVILEGES, _
                                    ByVal ReturnLength As Int32) As Int32
    Private Declare Function ExitWindowsEx Lib "user32" (ByVal uFlags As Int32, _
                                                         ByVal dwReserved As Int32) As Int32
    Private Declare Function GetCurrentProcess Lib "kernel32" () As Int32
    Private Declare Function LookupPrivilegeValue Lib "advapi32.dll" Alias "LookupPrivilegeValueA" _
                    (ByVal lpSystemName As String, ByVal lpName As String, ByVal lpLuid As LUID) As Int32
    Private Declare Function OpenProcessToken Lib "advapi32.dll" (ByVal ProcessHandle As Int32, _
                                            ByVal DesiredAccess As Int32, ByVal TokenHandle As Int32) As Int32
    'check for service stuff
    Private Const GENERIC_READ As Int32 = &H80000000
    Private Const SC_MANAGER_CONNECT As Int32 = &H1
    Private Const SC_MANAGER_CREATE_SERVICE As Int32 = &H2
    Private Const SC_MANAGER_ENUMERATE_SERVICE As Int32 = &H4
    Private Const SC_MANAGER_LOCK As Int32 = &H8
    Private Const SC_MANAGER_MODIFY_BOOT_CONFIG As Int32 = &H20
    Private Const SC_MANAGER_QUERY_LOCK_STATUS As Int32 = &H10
    Private Const SERVICE_RUNNING As Int32 = &H4
    Private Const STANDARD_RIGHTS_REQUIRED As Int32 = &HF0000
    Private Const SC_MANAGER_ALL_ACCESS As Int32 = (STANDARD_RIGHTS_REQUIRED Or _
                                                    SC_MANAGER_CONNECT Or _
                                                    SC_MANAGER_CREATE_SERVICE Or _
                                                    SC_MANAGER_ENUMERATE_SERVICE Or _
                                                    SC_MANAGER_LOCK Or _
                                                    SC_MANAGER_QUERY_LOCK_STATUS Or _
                                                    SC_MANAGER_MODIFY_BOOT_CONFIG)

    Private Structure SERVICE_STATUS
        Public dwServiceType As Int32
        Public dwCurrentState As Int32
        Public dwControlsAccepted As Int32
        Public dwWin32ExitCode As Int32
        Public dwServiceSpecificExitCode As Int32
        Public dwCheckPoint As Int32
        Public dwWaitHint As Int32
    End Structure

    Private Declare Function CloseServiceHandle Lib "advapi32.dll" _
                                                    (ByVal hSCObject As Int32) As Int32
    Private Declare Function OpenService Lib "advapi32.dll" Alias "OpenServiceA" _
                                (ByVal hSCManager As Int32, ByVal lpServiceName As String, _
                                 ByVal dwDesiredAccess As Int32) As Int32
    Private Declare Function OpenSCManager Lib "advapi32.dll" Alias "OpenSCManagerA" _
                                (ByVal lpMachineName As String, ByVal lpDatabaseName As String, _
                                 ByVal dwDesiredAccess As Int32) As Int32
    Private Declare Function QueryServiceStatus Lib "advapi32.dll" (ByVal hService As Int32, _
                                                                    ByVal lpServiceStatus As SERVICE_STATUS) As Int32
    ' code timer stuff
    Public Declare Function SetTimer Lib "user32" (ByVal hwnd As Int32, _
                                                   ByVal nIDEvent As Int32, ByVal uElapse As Int32, _
                                                   ByVal lpTimerFunc As Int32) As Int32
    Public Declare Function KillTimer Lib "user32" (ByVal hwnd As Int32, _
                                                    ByVal nIDEvent As Int32) As Int32

    Public gbTimerDone As Boolean

    ' mouse hook stuff
    Private Const WM_RBUTTONDBLCLK As Int32 = &H206
    Private Const WM_RBUTTONDOWN As Int32 = &H204
    Private Const WM_RBUTTONUP As Int32 = &H205
    Private Const WH_MOUSE As Int32 = 7

    Friend Structure POINTAPI
        Public X As Int32
        Public y As Int32
    End Structure

    Friend Structure MOUSEHOOKSTRUCT
        Public pt As POINTAPI
        Public hwnd As Int32
        Public wHitTestCode As Int32
        Public dwExtraInfo As Int32
    End Structure

    Private ghHook As Int32

    Private Delegate Function MouseProcCallbackDelegate(ByVal nCode As Int32, ByVal wParam As Int32, ByRef lParam As MOUSEHOOKSTRUCT) As Int32

    Private Declare Function CallNextHookEx Lib "user32" _
                (ByVal hHook As Int32, ByVal nCode As Int32, ByVal wParam As Int32, ByVal lParam As MOUSEHOOKSTRUCT) As Int32
    Private Declare Function SetWindowsHookEx Lib "user32" Alias "SetWindowsHookExA" _
                (ByVal idHook As Int32, ByVal lpfn As MouseProcCallbackDelegate, ByVal hmod As Int32, ByVal dwThreadID As Int32) As Int32
    Private Declare Function UnhookWindowsHookEx Lib "user32" (ByVal hHook As Int32) As Int32

#End Region

    'TODO - Need to replace On Error with Try/Catch blocks and clean up this code in general

#Region " Properties "

    Friend WriteOnly Property AlarmManHwnd() As Int32
        '********************************************************************************************
        'Description: From frmMain so AlarmMan is not hidden when the desktop is created.
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As Int32)
            mhAlarmMan = value
        End Set

    End Property

    Friend Property FormFirstVisible() As Boolean
        '********************************************************************************************
        'Description: To keep track if we can post messages
        '
        'Parameters: True or False
        'Returns: none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mbFrmFirstVisible
        End Get

        Set(ByVal value As Boolean)
            mbFrmFirstVisible = value
        End Set

    End Property

    Friend WriteOnly Property OkToCreateDesktop() As Boolean
        '********************************************************************************************
        'Description: This is here so we can skip the desktop in the ide. This is set true in initialize
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As Boolean)
            mbDeskTopOk = value
        End Set

    End Property

    Friend ReadOnly Property PWDesktopExists() As Boolean
        '********************************************************************************************
        'Description: Did we create the desktop?
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mbDesktopExists
        End Get

    End Property

    Friend ReadOnly Property PWDesktopRect() As RECT
        '********************************************************************************************
        'Description: Make the desktop dimensions available to PW_Main
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/04/07  RJO     Added to support .NET AlarmMan
        '********************************************************************************************
        'TODO - Is this required?
        Get
            Return mtPWDesktop
        End Get

    End Property

    Friend ReadOnly Property RunningApps() As String
        '********************************************************************************************
        'Description: Make the Process Names of Running Apps available to PW_Main to support new
        '             Password.NET
        '
        'Parameters: none
        'Returns: comma separated Running Apps string
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Dim sProcNames As String = String.Empty

            If bRefreshWindowList() Then
                If mnAppsRunning > 0 Then
                    For nApp As Integer = 1 To mPWApps.GetUpperBound(0)
                        Dim oProc As Process = Process.GetProcessById(mPWApps(nApp).lpProcInfo.dwProcessID)

                        If sProcNames.Length > 0 Then sProcNames = sProcNames & ","
                        sProcNames = sProcNames & oProc.ProcessName & "(" & oProc.Id.ToString & ")"
                    Next 'nApp
                End If
            End If 'bRefreshWindowList

            Return sProcNames
        End Get

    End Property

    Friend Property TileMode() As eTile
        '********************************************************************************************
        'Description: This is set when user clicks the tile option buttons on the main 
        '             form calls the arrange windows routine.
        '
        'Parameters: index of the button
        'Returns: none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            ' default cascade
            If (mnTile < 0) Or (mnTile > 2) Then mnTile = eTile.Cascade
            Return mnTile
        End Get

        Set(ByVal value As eTile)
            mnTile = value
            ArrangeWindows(mnTile)
            If frmMain.tmrCheckTile.Enabled = False Then
                frmMain.tmrCheckTile.Enabled = True
            End If
        End Set

    End Property

#End Region

#Region " Routines"

    Public Sub Initialize()
        '********************************************************************************************
        'Description: Call me first!
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/21/13  MSW     Deal with screen start failures
         '********************************************************************************************
        ReDim mhHiddenWindows(0)
        ReDim mPWApps(0)
        ReDim mOtherApps(0)

        mhSearchdisk = 0
        mhIE = 0
        mbDeskTopOk = True
        mbDesktopExists = False

    End Sub

    Public Function CreatePWDesktop() As Boolean
        '********************************************************************************************
        'Description: This doesn't really create a desktop, it hides all the stuff that 
        '             PW4 is not using. An array with all the hidden window handles is
        '             kept to allow them to be restored when PW4 is shut down.
        '
        'Parameters: None
        'Returns: true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        CreatePWDesktop = False

        'Hide everything on the screen except PW4_Main
        mhPWProcessID = GetCurrentProcessId()

        If mbDeskTopOk Then
            mbDesktopExists = bHideNonPWApps()
        End If

        'Get the working area of the screen
        Dim recWorkingArea As Rectangle = Screen.GetBounds(Screen.PrimaryScreen.Bounds)
        recWorkingArea.Y = frmMain.tlsMain.Height
        recWorkingArea.Height -= frmMain.tlsMain.Height

        With mtPWDesktop
            .Top = recWorkingArea.Top
            .Left = recWorkingArea.Left
            .Bottom = recWorkingArea.Bottom
            .Right = recWorkingArea.Right
        End With

        CreatePWDesktop = True

    End Function

    Public Function DestroyPWDesktop() As Boolean
        '********************************************************************************************
        'Description: Restores the windows that aren't a part of paintworks that were 
        '             hidden when PW4 was started.
        '
        'Parameters: none
        'Returns: True if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        DestroyPWDesktop = False

        If mbDesktopExists Then
            If bRestoreHiddenWindows() Then
                mbDesktopExists = False
                DestroyPWDesktop = True
            Else
                mbDesktopExists = True
                DestroyPWDesktop = False
            End If
        Else
            DestroyPWDesktop = True
        End If

    End Function

    Public Sub CheckWindowTile()
        '********************************************************************************************
        'Description: Called by timer. We don't receive the messages of our child 
        '             windows being closed so we use this to resize the remaining 
        '             windows if necessary.
        '
        'Parameters: none
        'Returns: none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nTmpCount As Integer

        If mnAppsRunning < 2 Then Exit Sub

        nTmpCount = mnAppsRunning

        bRefreshWindowList()

        If (nTmpCount <> mnAppsRunning) Then
            ArrangeWindows(TileMode)
        End If

    End Sub

    Public Function ArrangeWindows(ByVal Method As eTile) As Boolean
        '********************************************************************************************
        'Description: Tile or Cascade our windows.
        '
        ' Parameters: none
        ' Returns: True if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nNumApps As Int32
        Dim nLoop As Int32
        Dim lLeft As Int32
        Dim lTop As Int32
        Dim lWidth As Int32
        Dim lHeight As Int32
        Dim lResult As Int32
        Dim lAdjust As Int32

        Try

            ArrangeWindows = True

            ' make sure we don't have invalid handles
            bRefreshWindowList()

            nNumApps = mPWApps.GetUpperBound(0) 'UBound(mPWApps())

            ' nothing to do
            If nNumApps < 1 Then Exit Function

            ' if 1 app we dont care about the method
            If nNumApps = 1 Then
                lLeft = mtPWDesktop.Left
                lTop = mtPWDesktop.Top
                lWidth = mtPWDesktop.Right - mtPWDesktop.Left
                lHeight = mtPWDesktop.Bottom - mtPWDesktop.Top

                lResult = MoveWindow(mPWApps(1).hwnd, lLeft, lTop, lWidth, lHeight, True)
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

                    For nLoop = 1 To nNumApps
                        lResult = MoveWindow(mPWApps(nLoop).hwnd, lLeft, lTop, lWidth, lHeight, True)
                        lTop = lTop + lAdjust
                        lLeft = lLeft + lAdjust
                    Next 'nLoop

                Case eTile.Horizontal
                    lLeft = mtPWDesktop.Left
                    lTop = mtPWDesktop.Top
                    lWidth = mtPWDesktop.Right - mtPWDesktop.Left
                    lAdjust = ((mtPWDesktop.Bottom - mtPWDesktop.Top) \ nNumApps)
                    lHeight = lAdjust
                    For nLoop = 1 To nNumApps
                        lResult = MoveWindow(mPWApps(nLoop).hwnd, lLeft, lTop, lWidth, lHeight, True)
                        lTop = lTop + lAdjust
                    Next 'nLoop

                Case eTile.Vertical
                    lLeft = mtPWDesktop.Left
                    lTop = mtPWDesktop.Top
                    lAdjust = ((mtPWDesktop.Right - mtPWDesktop.Left) \ nNumApps)
                    lHeight = mtPWDesktop.Bottom - mtPWDesktop.Top
                    lWidth = lAdjust
                    For nLoop = 1 To nNumApps
                        lResult = MoveWindow(mPWApps(nLoop).hwnd, lLeft, lTop, lWidth, lHeight, True)
                        lLeft = lLeft + lAdjust
                    Next 'nLoop

            End Select
            'repaint time
            Application.DoEvents()
            Exit Function

        Catch ex As Exception
            ArrangeWindows = False
        End Try

    End Function

    Public Function StartPWApp(ByVal lpszCommandLine As String, ByVal dwFlags As Int32) As Int32
        '********************************************************************************************
        'Description: This routine is called to start a PW application.
        '
        'Parameters: none
        'Returns: True if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/13/12  MSW     mPWDesktop.StartPWApp - Tweak multistart feature to take  
        '                   advantage of single instance logic in browser
        ' 11/21/13  MSW     Deal with screen start failures
        ' 05/05/14  MSW     find hidden windows
        '********************************************************************************************
        Dim tSui As STARTUPINFO = New STARTUPINFO
        Dim tSa As SECURITY_ATTRIBUTES = New SECURITY_ATTRIBUTES
        Dim nLoop As Int32
        Dim MousePointer As Cursor
        Dim bTimerState As Boolean
        Dim Culture As System.Globalization.CultureInfo = frmMain.DisplayCulture

        StartPWApp = 0

        ' On Error GoTo StartErr

        MousePointer = frmMain.Cursor 'Screen.MousePointer
        Try
            bRefreshWindowList()

            ' check for name
            If Len(lpszCommandLine) = 0 Then
                MessageBox.Show(frmMain, gpsRM.GetString("psMT_COMMANDLINE", Culture), _
                                gpsRM.GetString("psSHORT_CAP", Culture), MessageBoxButtons.OK, MessageBoxIcon.Information)
                StartPWApp = 1
                Exit Function
            End If

            ' check flags
            If (dwFlags And eStartupFlags.ExclusiveMode) = eStartupFlags.ExclusiveMode Then
                If mnAppsRunning <> 0 Then
                    MessageBox.Show(frmMain, gpsRM.GetString("psEXCLUSIVE_MODE", Culture), _
                                    gpsRM.GetString("psSHORT_CAP", Culture), MessageBoxButtons.OK, MessageBoxIcon.Information)
                    StartPWApp = 3
                    Exit Function
                End If
            Else
                'check if exclusive screen is already open
                If mnAppsRunning <> 0 Then
                    For nLoop = 1 To mnAppsRunning
                        If (mPWApps(nLoop).flags And eStartupFlags.ExclusiveMode) = eStartupFlags.ExclusiveMode Then
                            MessageBox.Show(frmMain, gpsRM.GetString("psEXCLUSIVE_MODE", Culture), _
                                            gpsRM.GetString("psSHORT_CAP", Culture), MessageBoxButtons.OK, MessageBoxIcon.Information)
                            StartPWApp = 4
                            Exit Function
                        End If
                    Next
                End If
            End If

        Catch ex As Exception

        End Try
        Try
            'Check if anything needs to be kicked out of the apps running list
            For nPWApp As Integer = mPWApps.GetUpperBound(0) To 1 Step -1
                Dim sTmp1() As String = Split(mPWApps(nPWApp).LaunchString, "\")
                Dim sTmp2() As String = Split(sTmp1(sTmp1.GetUpperBound(0)), """")
                Dim sTmp3() As String = Split(sTmp2(0), ".")
                Dim sProcessName As String = sTmp2(0).Substring(0, sTmp2(0).Length - (sTmp3(sTmp3.GetUpperBound(0)).Length + 1))
                Dim oProcs() As Process = Process.GetProcessesByName(sProcessName)

                Dim bRemove As Boolean = True
                If (oProcs.Length > 0) AndAlso oProcs(0) IsNot Nothing AndAlso (mPWApps(nPWApp).lpProcInfo.dwProcessID <> 0) Then
                    If CInt(oProcs(0).MainWindowHandle) = 0 Then
                        Try
                            oProcs(0).Kill()
                        Catch ex As Exception
                        End Try

                    Else
                        bRemove = False
                    End If
                End If
                If bRemove Then
                    If (nPWApp < (mnAppsRunning - 1)) Then
                        For nShiftItem As Integer = nPWApp To mnAppsRunning - 1
                            mPWApps(nShiftItem).hwnd = mPWApps(nShiftItem + 1).hwnd
                            mPWApps(nShiftItem).flags = mPWApps(nShiftItem + 1).flags
                            mPWApps(nShiftItem).LaunchString = mPWApps(nShiftItem + 1).LaunchString
                            mPWApps(nShiftItem).lpProcInfo.dwProcessID = mPWApps(nShiftItem + 1).lpProcInfo.dwProcessID
                            mPWApps(nShiftItem).lpProcInfo.dwThreadID = mPWApps(nShiftItem + 1).lpProcInfo.dwThreadID
                            mPWApps(nShiftItem).lpProcInfo.hProcess = mPWApps(nShiftItem + 1).lpProcInfo.hProcess
                            mPWApps(nShiftItem).lpProcInfo.hThread = mPWApps(nShiftItem + 1).lpProcInfo.hThread
                        Next
                    End If
                    mnAppsRunning -= 1
                    ReDim Preserve mPWApps(mnAppsRunning)
                End If
            Next

        Catch ex As Exception

        End Try
        Try
            'See if this program is already running without a window.
            'if it is, kill it
            'MSW 12/18/13 adjust this to handle more complicated launch strings
            Dim sProcessName As String = String.Empty
            Dim sTmp1() As String = Split(lpszCommandLine, "\")
            'Debug.Print(sProcessName)
            For Each sTmp1a As String In sTmp1
                Dim nIndex As Integer = InStr(sTmp1a.ToLower, ".exe")
                If nIndex > 0 Then
                    sProcessName = sTmp1a.Substring(0, nIndex - 1)
                    'Debug.Print(sProcessName)
                End If
            Next
            If sProcessName = String.Empty Then
                Dim sTmp2() As String = Split(sTmp1(sTmp1.GetUpperBound(0)), """")
                Dim sTmp3() As String = Split(sTmp2(0), ".")
                sProcessName = sTmp2(0).Substring(0, sTmp2(0).Length - (sTmp3(sTmp3.GetUpperBound(0)).Length + 1))
            End If
            Dim oProcs() As Process = Process.GetProcessesByName(sProcessName)

            For Each oProc As Process In oProcs
                If (oProc.Id <> 0) Then
                    If (CInt(oProc.MainWindowHandle) = 0) Then
                    Try
                        oProc.Kill()
                    Catch ex As Exception
                    End Try
                    ElseIf (CInt(oProc.MainWindowHandle) <> 0) Then
                        'MSW 05/05/14 - find hidden windows
                        'Already running.  If not multistart, bring to front
                        If (dwFlags And eStartupFlags.AllowMultiStart) <> eStartupFlags.AllowMultiStart Then
                            Call ShowWindow(CInt(oProc.MainWindowHandle), SW_RESTORE)
                            StartPWApp = 0
                            Exit Function
                        End If
                    End If
                End If
            Next

        Catch ex As Exception

        End Try
        Try

            'Adjust multistart to accomodate the single instance features
            Dim bAddToList As Boolean = True
            'MSW 4/18/13 - catch the same screen with different parameters
            '              Also move the too many open check after the already open check
            Dim sTmpCompare As String() = Split(lpszCommandLine)
            ' make sure its not already running
            For nLoop = 1 To mnAppsRunning
                Dim sTmpCompare2 As String() = Split(mPWApps(nLoop).LaunchString)
                If sTmpCompare(0) = sTmpCompare2(0) Then 'lpszCommandLine = mPWApps(nLoop).LaunchString Then
                    If (dwFlags And eStartupFlags.AllowMultiStart) <> eStartupFlags.AllowMultiStart Then
                        MessageBox.Show(frmMain, gpsRM.GetString("psPROG_RUNNING", Culture), _
                                        gpsRM.GetString("psSHORT_CAP", Culture), MessageBoxButtons.OK, MessageBoxIcon.Information)
                        StartPWApp = 5
                        Exit Function
                    Else
                        If (dwFlags And eStartupFlags.MultiStartSingleInstance) = eStartupFlags.MultiStartSingleInstance Then
                            'Don't add to the list
                            bAddToList = False
                            Exit For
                        End If
                    End If
                End If
            Next


            ' got room for 1 more?
            If mnAppsRunning = mnMAX_APPS And (bAddToList = True) Then
                'Override max app limit for help or changelog
                If (InStr(sTmpCompare(0), "PWBrowser.exe", CompareMethod.Text) = 0) And _
                   (InStr(sTmpCompare(0), "ChangeLog.exe", CompareMethod.Text) = 0) Then
                    MessageBox.Show(frmMain, gpsRM.GetString("psMAX_PROG_RUN_MSG", Culture), _
                                    gpsRM.GetString("psSHORT_CAP", Culture), MessageBoxButtons.OK, MessageBoxIcon.Information)
                    StartPWApp = 2
                    Exit Function
                End If
            End If

            ' should be ok to start now
            frmMain.Cursor = Cursors.WaitCursor

            '07/31/05 gks
            bTimerState = frmMain.tmrPLCWatchDog.Enabled
            frmMain.tmrPLCWatchDog.Enabled = False

            If bAddToList Then
                ' increment app counter - adjusted for closing in bRefreshWindowList
                mnAppsRunning += 1
                ReDim Preserve mPWApps(mnAppsRunning)

                mPWApps(mnAppsRunning).LaunchString = lpszCommandLine

                ' default security
                'With tSa
                '    .nLength = Len(tSa)
                '    .bInheritHandle = 1&
                '    .lpSecurityDescriptor = 0&
                'End With
            End If
            ' initialize startupinfo
            With tSui
                '.cb = Len(tSui)
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
            'tSui.cb = Len(tSui)
            If bStartApp(lpszCommandLine, tSa, tSui, mPWApps(mnAppsRunning).lpProcInfo) = False Then

                'start failed
                MessageBox.Show(frmMain, gpsRM.GetString("psCOULD_NOT_START", Culture) & vbCrLf & lpszCommandLine, _
                                gpsRM.GetString("psSHORT_CAP", Culture), MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                StartPWApp = 6
                If bAddToList Then
                    mnAppsRunning -= 1
                    ReDim Preserve mPWApps(mnAppsRunning)
                End If
            Else
                If bAddToList Then
                    ' this is the hwnd found in the callback function
                    mPWApps(mnAppsRunning).hwnd = mhHandleHolder
                    mPWApps(mnAppsRunning).flags = dwFlags
                End If
            End If
            ' re-arrange windows
            If mnAppsRunning = 1 Then
                ' we want to use the whole screen, tile horz
                Call ArrangeWindows(eTile.Horizontal)
            Else
                'use whatever is selected
                Call ArrangeWindows(TileMode)
            End If
        Catch ex As Exception

        End Try

ExitHere:
        frmMain.Cursor = Cursors.Default
        frmMain.tmrPLCWatchDog.Enabled = bTimerState
        Exit Function

StartErr:

        GoTo ExitHere

    End Function

    Public Function StartOtherApp(ByVal lpszCommandLine As String, ByVal dwFlags As Long, _
                                                         ByVal bVerbose As Boolean) As Long
        '********************************************************************************************
        'Description: This routine is called to start an application not managed by
        '             Paintworks.
        '
        'Parameters: command line and windowstyle flags
        'Returns: true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim tSui As STARTUPINFO
        Dim tSa As SECURITY_ATTRIBUTES
        Dim nPtr As Integer
        Dim MousePointer As Cursor
        Dim Culture As System.Globalization.CultureInfo = frmMain.DisplayCulture

        StartOtherApp = 0

        MousePointer = frmMain.Cursor

        On Error GoTo StartErr

        ' check for name
        If Len(lpszCommandLine) = 0 Then
            If bVerbose Then
                MessageBox.Show(frmMain, gpsRM.GetString("psMT_COMMANDLINE", Culture), _
                                gpsRM.GetString("psSHORT_CAP", Culture), MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            Exit Function

        End If

        ' should be ok to start now
        frmMain.Cursor = Cursors.WaitCursor
        nPtr = mOtherApps.GetUpperBound(0) + 1

        ReDim Preserve mOtherApps(nPtr)

        mOtherApps(nPtr).LaunchString = lpszCommandLine

        ' default security
        'With tSa
        '    .nLength = Strings.Len(tSa)
        '    .bInheritHandle = 1&
        '    .lpSecurityDescriptor = 0&
        'End With

        ' initialize startupinfo
        With tSui
            '.cb = Len(tSui)
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
        tSui.cb = Len(tSui)

        If bStartApp(lpszCommandLine, tSa, tSui, mOtherApps(nPtr).lpProcInfo) = False Then

            'start failed
            If bVerbose Then
                MessageBox.Show(frmMain, "*" & gpsRM.GetString("psCOULD_NOT_START", Culture), _
                gpsRM.GetString("psSHORT_CAP", Culture), MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
        Else
            ' this is the hwnd found in the callback function
            mOtherApps(nPtr).hwnd = mhHandleHolder
            StartOtherApp = mhHandleHolder
        End If

ExitHere:
        frmMain.Cursor = MousePointer
        Exit Function

StartErr:

        GoTo ExitHere

    End Function

    Private Function bStartApp(ByVal lpszCommandLine As String, _
                                ByRef tSa As SECURITY_ATTRIBUTES, _
                                ByRef tSui As STARTUPINFO, _
                                ByRef lpProcessInfo As PROCESS_INFORMATION) As Boolean
        '********************************************************************************************
        'Description: Starts an application.
        '
        'Parameters: None
        'Returns: True if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/21/13  MSW     Deal with screen start failures
        '********************************************************************************************
        Dim oDelegate As EnumWindowsCallbackDelegate
        Dim lStartTic As Long
        Dim bFound As Boolean

        bFound = False
        mhHandleHolder = 0
        oDelegate = New EnumWindowsCallbackDelegate(AddressOf bFindWindowCallback)

        For nTry As Integer = 1 To 2
            Try
                If CreateProcess(vbNullString, lpszCommandLine, IntPtr.Zero, IntPtr.Zero, True, DETACHED_PROCESS, _
                                 IntPtr.Zero, vbNullString, tSui, lpProcessInfo) = False Then
                    If nTry = 2 Then
                        bStartApp = False
                        mDebug.WriteEventToLog("PW4_Main Module: mPWDesktop.vb Routine: bStartapp", _
                                               "Error: " & Err.LastDllError.ToString & " - Createprocess failed on: " & lpszCommandLine)
                        Exit Function
                    End If
                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Debug", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End Try


            'wait for new apps message queue to be clear
            Call WaitForInputIdle(lpProcessInfo.hProcess, 8000)

            lStartTic = GetTickCount

            Do While ((Math.Abs(GetTickCount - lStartTic) < mlWAITFORWINDOWMS)) And bFound = False
                Application.DoEvents()
                If EnumWindows(oDelegate, lpProcessInfo.dwProcessID) = 0 Then
                    bFound = True
                End If
            Loop

            If bFound Then
                bStartApp = True
                Exit Function
            Else
                If nTry = 2 Then
                    mDebug.WriteEventToLog("PW4_Main Module: mPWDesktop.vb Routine: bStartapp", _
                                           "Error: " & lpszCommandLine & " started, main window not found. processID = " & _
                                           lpProcessInfo.dwProcessID.ToString)
                    bStartApp = False
                Else
                    Try
                        'See if this program is already running without a window.
                        'if it is, kill it
                        Dim sTmp1() As String = Split(lpszCommandLine, "\")
                        Dim sTmp2() As String = Split(sTmp1(sTmp1.GetUpperBound(0)), """")
                        Dim sTmp3() As String = Split(sTmp2(0), ".")
                        Dim sProcessName As String = sTmp2(0).Substring(0, sTmp2(0).Length - (sTmp3(sTmp3.GetUpperBound(0)).Length + 1))
                        Dim oProcs() As Process = Process.GetProcessesByName(sProcessName)

                        Dim bRemove As Boolean = True
                        For Each oProc As Process In oProcs
                            If (oProc.Id <> 0) AndAlso (CInt(oProc.MainWindowHandle) = 0) Then
                                Try
                                    oProc.Kill()
                                Catch ex As Exception
                                End Try

                            Else
                                bRemove = False
                            End If

                        Next

                    Catch ex As Exception

                    End Try

                End If
            End If

        Next

    End Function

    Private Function bHideNonPWApps() As Boolean
        '********************************************************************************************
        'Description: Hides the windows that aren't a part of Paintworks.
        '
        'Parameters: none
        ' Returns: True if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/29/10  RJO     AlarmMan wouldn't respond to messages after desktop was hidden. 
        '********************************************************************************************
        Dim oDelegate As EnumWindowsCallbackDelegate

        bHideNonPWApps = False
        oDelegate = New EnumWindowsCallbackDelegate(AddressOf bHideWindowsCallback)

        If EnumWindows(oDelegate, mhPWProcessID) <> 0 Then
            bHideNonPWApps = True
        End If

        'AlarmMan is now hidden. Need to un-hide it.
        For Each BackgroundApp As frmMain.udsAppConfig In frmMain.gcolBackgroundApps
            If BackgroundApp.ProcessName.ToLower = "alarmman" Then
                'Must be a 2008 thing. Worked in VB.NET 2005
                'Call ShowWindow(BackgroundApp.hWnd, SW_SHOWMINIMIZED) 'RJO 01/29/10
                Try
                    Call ShowWindow(BackgroundApp.hWnd, SW_SHOWNA)
                Catch ex As Exception
                    'Dim oProc As Process = mWorksComm.GetProcess("AlarmMan")) 'RJO 03/20/12
                    Dim oProc As Process = frmMain.oIPC.GetProcess("AlarmMan")

                    BackgroundApp.ProcessId = oProc.Id
                    BackgroundApp.hWnd = oProc.MainWindowHandle.ToInt32
                    Call ShowWindow(BackgroundApp.hWnd, SW_SHOWNA)
                End Try
            End If
        Next

    End Function

    Private Function bHideWindowsCallback(ByVal hwnd As Int32, ByVal lParam As Int32) As Boolean
        '********************************************************************************************
        'Description: This is a callback function for EnumWindows.
        '
        'Parameters: hWnd - handle to window
        '            lParam - Application defined parameter
        '
        'Returns: True to keep enumerating
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nIndex As Integer
        Dim lProc As Int32
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
        If IsWindowVisible(hwnd) = 0 Then
            bHideWindowsCallback = True
            Exit Function
        End If

        ' is it a paintworks app window?
        For nIndex = 1 To mPWApps.GetUpperBound(0)
            If hwnd = mPWApps(nIndex).hwnd Then
                ' it's ours
                bHideWindowsCallback = True
                Exit Function
            End If
        Next 'nIndex

        If (mhAlarmMan > 0) And (hwnd = mhAlarmMan) Then
            '.NET AlarmMan doesn't get started as a PW App. It's also ours.
            bHideWindowsCallback = True
            Exit Function
        End If

        'Pw Main created window?
        Call GetWindowThreadProcessId(hwnd, lProc)
        If lProc = mhPWProcessID Then
            bHideWindowsCallback = True
            Exit Function
        End If

        ' hide it
        nPtr = mhHiddenWindows.GetUpperBound(0) + 1
        ReDim Preserve mhHiddenWindows(nPtr)
        mhHiddenWindows(nPtr) = hwnd

        Call ShowWindow(hwnd, SW_HIDE)

        bHideWindowsCallback = True

    End Function

    Private Function bFindWindowCallback(ByVal hwnd As Int32, ByVal lParam As Int32) As Boolean
        '********************************************************************************************
        'Description: This is a callback function for EnumWindows.
        '
        'Parameters: hWnd - handle to window
        '            lParam - Application defined parameter
        '
        'Returns: True to keep enumerating
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim lpdwProcessId As Int32
        Dim lpRect As RECT

        If GetWindowThreadProcessId(hwnd, lpdwProcessId) <> 0 Then
            ' new process Id passed in from enumwindows call
            If lpdwProcessId = lParam Then
                'were getting warm
                If ((IsWindowVisible(hwnd) <> 0) And (IsIconic(hwnd) = 0)) Then
                    'make sure rect is valid
                    GetWindowRect(hwnd, lpRect)
                    If (lpRect.Right >= lpRect.Left) And (lpRect.Bottom >= lpRect.Top) Then '10/10/04
                        'should be the one!
                        mhHandleHolder = hwnd
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
        '********************************************************************************************
        'Description: Restores the hidden windows that aren't a part of Paintworks.
        '
        'Parameters: none
        'Returns: True if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nLoop As Int32
        Dim hTmp As Int32

        Try

            For nLoop = mhHiddenWindows.GetLowerBound(0) To mhHiddenWindows.GetUpperBound(0)
                hTmp = mhHiddenWindows(nLoop)
                If (hTmp <> 0) And (IsWindow(hTmp) <> 0) Then
                    Call ShowWindow(hTmp, SW_SHOWNA)
                End If
            Next

            ReDim mhHiddenWindows(0)
            bRestoreHiddenWindows = True

        Catch ex As Exception
            bRestoreHiddenWindows = False
        End Try

    End Function

    Private Function bRefreshWindowList() As Boolean
        '********************************************************************************************
        'Description: Updates the Paintworks managed windows list. This is recursive. 
        '             This is recursive.
        '
        'Parameters: none
        'Returns: True if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/21/13  MSW     Deal with screen start failures
        '********************************************************************************************
        Dim nWindow As Integer
        Dim nShift As Integer
        Dim nCount As Integer

        bRefreshWindowList = False

        nCount = mPWApps.GetUpperBound(0)

        For nWindow = 1 To nCount

            If mPWApps(nWindow).hwnd <> 0 Then

                If IsWindow(mPWApps(nWindow).hwnd) <> 0 Then
                    ' still open
                Else
                    ' window was closed - shift all windows up one
                    If (nWindow + 1) <= nCount Then
                        For nShift = (nWindow + 1) To nCount
                            mPWApps(nShift - 1) = mPWApps(nShift)
                        Next 'nShift
                    End If  '(nWindow + 1) < nCount

                    'If nCount > 1 Then
                    ' Stop leak for memory
                    Call CloseHandle(mPWApps(nCount).lpProcInfo.hProcess)
                    Call CloseHandle(mPWApps(nCount).lpProcInfo.hThread)
                    'shrink array by 1
                    ReDim Preserve mPWApps(nCount - 1)
                    'Else
                    '    ' all gone
                    '    mPWApps(1).hwnd = 0
                    'End If

                    ' check again
                    bRefreshWindowList()

                End If

                End If  'mPWApps(nWindow).hWnd <> 0

            'in case we resized
            If nWindow >= mPWApps.GetUpperBound(0) Then Exit For

        Next 'nWindow

        ' adjust app counter
        'If mPWApps(1).hwnd = 0 Then
        '    mnAppsRunning = 0
        'Else
        mnAppsRunning = mPWApps.GetUpperBound(0)
        'End If

        bRefreshWindowList = True

    End Function

    Public Function CloseAllPWApps() As Boolean
        '********************************************************************************************
        'Description: Close any pw apps we have started by the messiest method possible. 
        '             The theory is that a reboot will occur after cleanup anyway.
        '
        'Parameters: none
        'Returns: True if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '04/19/05   gks     Give a chance to shut down before killing the app (allow alarmman to log)
        '********************************************************************************************
        Dim nIndex As Integer

        On Error GoTo CloseErr

        CloseAllPWApps = True

        For nIndex = 1 To mPWApps.GetUpperBound(0)
            If mPWApps(nIndex).hwnd <> 0 Then

                If IsWindow(mPWApps(nIndex).hwnd) <> 0 Then
                    'first try to be nice
                    Call PostMessage(mPWApps(nIndex).hwnd, WM_CLOSE, vbNull, vbNull)
                    Call WaitForSingleObject(mPWApps(nIndex).hwnd, 3000)
                    Application.DoEvents()
                    'do we need a club?
                    If IsWindow(mPWApps(nIndex).hwnd) <> 0 Then
                        Call TerminateProcess(mPWApps(nIndex).lpProcInfo.hProcess, 0&)
                    End If
                End If

                Call CloseHandle(mPWApps(nIndex).lpProcInfo.hProcess)
                Call CloseHandle(mPWApps(nIndex).lpProcInfo.hThread)
            End If
        Next

        For nIndex = 1 To mOtherApps.GetUpperBound(0)
            If mOtherApps(nIndex).hwnd <> 0 Then

                If IsWindow(mOtherApps(nIndex).hwnd) <> 0 Then
                    'first try to be nice
                    Call PostMessage(mOtherApps(nIndex).hwnd, WM_CLOSE, vbNull, vbNull)
                    Call WaitForSingleObject(mOtherApps(nIndex).hwnd, 5000)
                    Application.DoEvents()

                    'the above wait is not working for some reason  4/19/05
                    Dim lTmp As Long
                    For lTmp = 0 To 10
                        If IsWindow(mOtherApps(nIndex).hwnd) = 1 Then
                            System.Threading.Thread.Sleep(500)
                            Application.DoEvents()
                        Else
                            Exit For
                        End If
                    Next

                    'do we need a club?
                    If IsWindow(mOtherApps(nIndex).hwnd) <> 0 Then
                        Call TerminateProcess(mOtherApps(nIndex).lpProcInfo.hProcess, 0&)
                    End If
                End If

                Call CloseHandle(mOtherApps(nIndex).lpProcInfo.hProcess)
                Call CloseHandle(mOtherApps(nIndex).lpProcInfo.hThread)
            End If
        Next

        Exit Function

CloseErr:
        CloseAllPWApps = False
        Resume Next

    End Function

    Public Function CloseAppByTitle(ByVal WindowTitle As String) As Boolean
        '********************************************************************************************
        'Description: Close an app given the window title. This is for apps that pwmain 
        '             has started.
        '
        'Parameters: Name of the window
        'Returns: True if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oDelegate As EnumWindowsCallbackDelegate
        Dim void As Int32

        On Error GoTo CloseErr

        CloseAppByTitle = True
        oDelegate = New EnumWindowsCallbackDelegate(AddressOf bDestroyWindowCallback)

        msWindowTitle = WindowTitle

        Call EnumWindows(oDelegate, void)

        Application.DoEvents()

        Exit Function

CloseErr:
        CloseAppByTitle = False

    End Function

    Private Function bDestroyWindowCallback(ByVal hwnd As Int32, ByVal lParam As Int32) As Boolean
        '********************************************************************************************
        'Description: This is a callback function for EnumWindows.
        '
        'Parameters: hWnd - handle to window
        '            lParam - Application defined parameter
        '
        'Returns: True to keep enumerating
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp As String '* 255
        Dim nLen As Integer
        Dim sName As String

        bDestroyWindowCallback = True

        sTmp = Space(255)
        nLen = GetWindowText(hwnd, sTmp, Len(sTmp))

        If nLen > 0 Then
            sName = Strings.Left(sTmp, nLen)
            If sName = msWindowTitle Then
                ' this is the one -  if user hasn't saved data by now its too late...
                DestroyWindow(hwnd)
                ' stop enumerating
                bDestroyWindowCallback = False
            End If  '
        End If  'nLen > 0

    End Function

    Private Function bWindowFromTitleCallback(ByVal hwnd As Int32, ByVal lParam As Int32) As Boolean
        '********************************************************************************************
        'Description: This is a callback function for EnumWindows
        '
        'Parameters: hWnd - handle to window
        '            lParam - Application defined parameter
        '
        'Returns: True to keep enumerating
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp As String '* 255
        Dim nLen As Integer
        Dim sName As String

        bWindowFromTitleCallback = True

        sTmp = Space(255)
        nLen = GetWindowText(hwnd, sTmp, Len(sTmp))

        If nLen > 0 Then
            sName = Left$(sTmp, nLen)
            Debug.Print(sName)
            If sName = msWindowTitle Then
                ' this is the one -
                mhHandleHolder = hwnd
                ' stop enumerating
                bWindowFromTitleCallback = False
            End If  '
        End If  'nLen > 0

    End Function

    Public Function WaitForWindowByTitle(ByVal WindowTitle As String, _
                                            ByVal SecondstoWait As Integer) As Boolean
        '********************************************************************************************
        'Description: Waits for an application window to be ready. If there is more than 
        '             one window with the same title, this function stops at the first 
        '             one it finds.
        '
        'Parameters: Window title
        '
        'Returns: True if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oDelegate As EnumWindowsCallbackDelegate
        Dim lMs As Int32
        Dim hProcessId As Int32
        Dim void As Int32

        lMs = SecondstoWait * 1000
        msWindowTitle = WindowTitle
        oDelegate = New EnumWindowsCallbackDelegate(AddressOf bWindowFromTitleCallback)

        WaitForWindowByTitle = False

        If mbFrmFirstVisible Then
            PostStatusMsg(gpsRM.GetString("psCHECK_FOR_PROGRAM", frmMain.DisplayCulture) & _
                                          WindowTitle)
        End If

        On Error GoTo WaitErr

        ' if window is found callback returns false which makes enumwindows return false
        If EnumWindows(oDelegate, void) = 0 Then

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
        '********************************************************************************************
        'Description: Waits for a service to be running. The names can be found at 
        '             HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services.
        '
        'Parameters: service name , how long to wait
        '
        'Returns: True if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim lReturn As Long
        Dim sErrorMsg As String
        Dim lError As Long
        Dim hSCManager As Int32
        Dim hService As Int32
        Dim lpServiceStatus As SERVICE_STATUS
        Dim dStart As Date

        On Error GoTo ErrorHandler

        WaitForService = False

        If mbFrmFirstVisible Then
            PostStatusMsg(gpsRM.GetString("psCHECK_FOR_SERVICE", frmMain.DisplayCulture) & sName)
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

        dStart = DateTime.Now

        Do While ((DateDiff("s", dStart, DateTime.Now) < SecondstoWait))

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
        '********************************************************************************************
        'Description: Hides or removes the help (searchdisk and IE) windows.
        '
        'Parameters: none
        'Returns: True if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim oDelegate As EnumWindowsCallbackDelegate

            HelpWindows = False
            oDelegate = New EnumWindowsCallbackDelegate(AddressOf bHelpWindowCallback)

            Call EnumWindows(oDelegate, WhatToDo)

            HelpWindows = True

        Catch ex As Exception
            mDebug.WriteEventToLog("PW4_Main Module: mPWDesktop.vb Routine: HelpWindows", _
                           "Error: " & ex.Message & ", WhatToDo = " & WhatToDo)
        End Try

    End Function

    Private Function bHelpWindowCallback(ByVal hwnd As Int32, ByVal lParam As Int32) As Boolean
        '********************************************************************************************
        'Description: This is a callback function for EnumWindows.
        '
        'Parameters: hWnd - handle to window
        '            lParam - Application defined parameter
        '
        'Returns: True to keep enumerating
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp As String '* 255
        Dim nLen As Integer
        Dim lReturnValue As Int32
        Dim lProcess As Int32
        Dim lProcessID As Int32
        Dim sName As String

        Const sSEARCHDISK As String = "Exit SearchDisc"
        Const sIE As String = "FANUC Robotics CDROM"

        sTmp = Space(255)
        nLen = GetWindowText(hwnd, sTmp, Len(sTmp))
        sName = Left$(sTmp, nLen)

        If nLen > 0 Then

            Select Case lParam
                Case eHelpWin.Destroy
                    If (sName = sSEARCHDISK) Or _
                            (sIE = Left$(sName, Len(sIE))) Then
                        lReturnValue = GetWindowThreadProcessId(hwnd, lProcessID)
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
                        Call MoveWindow(hwnd, 20000, 0, 500, 500, True)
                        mhSearchdisk = hwnd
                        If mbFrmFirstVisible Then
                            PostStatusMsg(sSEARCHDISK & " " & gpsRM.GetString("psFOUND", frmMain.DisplayCulture))
                        End If
                    End If

                    If (sIE = Left$(sName, Len(sIE))) Then
                        Call MoveWindow(hwnd, 20000, 0, 500, 500, True)
                        mhIE = hwnd
                        If mbFrmFirstVisible Then
                            PostStatusMsg(sIE & " " & gpsRM.GetString("psFOUND", frmMain.DisplayCulture))
                        End If
                    End If

            End Select

        End If  'nLen > 0

        ' just keep enumerating - there may be more than 1 window with the title we want
        bHelpWindowCallback = True

    End Function

    Public Sub RestartWindows(Optional ByVal LogoffOnly As Boolean = False)
        '********************************************************************************************
        'Description: Reboot the computer
        '
        'Parameters: Maybe someday we will just logout and log back in...
        '
        'Returns: none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Const TOKEN_ADJUST_PRIVILEGES As Int32 = &H20
        Const TOKEN_QUERY As Int32 = &H8
        Const SE_PRIVILEGE_ENABLED As Int32 = &H2
        Const SE_SHUTDOWN_NAME As String = "SeShutdownPrivilege"

        Dim hProcessHandle As Int32
        Dim hTokenHandle As Int32
        Dim tmpLuid As LUID
        Dim tkp As TOKEN_PRIVILEGES
        Dim tkpPreviousState As TOKEN_PRIVILEGES
        Dim lSizeOfPriv As Int32
        Dim lRet As Int32

        hProcessHandle = GetCurrentProcess()

        lRet = OpenProcessToken(hProcessHandle, (TOKEN_ADJUST_PRIVILEGES Or _
                                            TOKEN_QUERY), hTokenHandle)

        ' Get the LUID for shutdown privilege.
        lRet = LookupPrivilegeValue(vbNullString, SE_SHUTDOWN_NAME, tmpLuid)

        '07/09/08
        ReDim tkp.Privileges(1)
        ReDim tkpPreviousState.Privileges(1)

        tkp.PrivilegeCount = 1    ' One privilege to set
        tkp.Privileges(0).pLuid = tmpLuid
        tkp.Privileges(0).Attributes = SE_PRIVILEGE_ENABLED

        ' Enable the shutdown privilege in the access token of this process.
        lRet = AdjustTokenPrivileges(hTokenHandle, CType(False, Int32), _
                    tkp, Len(tkpPreviousState), tkpPreviousState, lSizeOfPriv)

        'say goodnight dick... "goodnight dick"
        lRet = ExitWindowsEx((EWX_SHUTDOWN Or EWX_FORCE Or EWX_REBOOT), &HFFFF)

    End Sub

    Public Function WindowHandleFromTitle(ByVal WindowTitle As String) As Long
        '********************************************************************************************
        'Description: Get the handle to a (top level) window with the requested title. Partial titles
        '             will work. If there are more than 1 window with the same name, this returns the
        '             first one it finds.
        '
        'Parameters:    Title string to find
        'Returns:       Handle or 0 if not found
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oDelegate As EnumWindowsCallbackDelegate
        Dim void As Int32

        WindowHandleFromTitle = 0
        oDelegate = New EnumWindowsCallbackDelegate(AddressOf bFindWindowByTitleCallback)

        If Len(WindowTitle) = 0 Then Exit Function

        msWindowTitle = WindowTitle
        mhHandleHolder = 0

        If EnumWindows(oDelegate, void) = 0 Then
            ' 0 indicates failure
        End If

        WindowHandleFromTitle = mhHandleHolder

    End Function

    Private Function bFindWindowByTitleCallback(ByVal hwnd As Int32, _
                                                        ByVal lParam As Int32) As Boolean
        '********************************************************************************************
        'Description: This is the callback function for EnumWindows.
        '
        'Parameters: lParam could be set in calling routine and used in a case statment if we wanted 
        '            this to do different things
        '
        'Returns: True to keep enumerating
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim lStrLen As Int32
        Dim sName As String
        Dim lpWindowTitle As String ' * 255

        lpWindowTitle = Space(255)
        lStrLen = GetWindowText(hwnd, lpWindowTitle, Len(lpWindowTitle))
        If lStrLen > 0 Then
            sName = Left$(lpWindowTitle, lStrLen)
            'Debug.Print sName
            If Len(sName) >= Len(msWindowTitle) Then
                sName = Left$(sName, Len(msWindowTitle))
                If StrComp(sName, msWindowTitle, vbTextCompare) = 0 Then
                    mhHandleHolder = hwnd
                    bFindWindowByTitleCallback = False
                    Exit Function
                End If
            End If
        End If

        bFindWindowByTitleCallback = True

    End Function

    Public Sub TimerProc(ByVal hwnd As Long, ByVal uMsg As Long, _
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
        Dim oDelegate As MouseProcCallbackDelegate

        SetMouseHook = False
        oDelegate = New MouseProcCallbackDelegate(AddressOf MouseProc)

        ghHook = SetWindowsHookEx(WH_MOUSE, oDelegate, 0&, 0&) 'App.ThreadId)

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

    Private Function MouseProc(ByVal nCode As Int32, ByVal wParam As Int32, ByRef lParam As MOUSEHOOKSTRUCT) As Int32
        '********************************************************************************************
        'Description: This function takes the place of the windows MouseProd function. Add other 
        '             mouse massaging here if desired.
        '
        '             Note: According to the documentation, breakpoints should not be inserted in this 
        '                   routine. If the window receives another message, you will crash and burn.
        '
        'Parameters: wParam is the mouse message.
        'Returns:    1 to discard message, or passes wParam to next hook.
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

    Friend Sub PostStatusMsg(ByVal Msg As String)
        '********************************************************************************************
        'Description: Adds status information to the frmFirst lstDebugOutput listbox.
        '
        'Parameters: Msg is the message to display in the listbox.
        '            
        'Returns: None
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        If Msg Is Nothing Then
            Msg = "<Error: Msg = NULL>"
        End If

        Msg = Format(TimeString, "Long Time") & " " & Msg
        If FormFirstVisible Then
            With frmFirst.lstDebugOutput
                If .Items.Count = 32767 Then .Items.Remove(1)
                .Items.Add(Msg)
                .TopIndex = .Items.Count - 1
                .Refresh()
            End With
        End If

    End Sub

#End Region

End Module
