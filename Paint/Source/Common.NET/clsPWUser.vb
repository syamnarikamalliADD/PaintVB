' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2012
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: clsPWUser
'
' Description: Paintworks Application interface to Password.NET application
' 
'
' Dependencies: frmMain.vb, clsInterProcessComm.vb  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Rick Olejniczak
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
' 02/08/2012    RJO     Initial code                                                  4.1.2.0
' 04/13/2012    RJO     Fixed bug in CheckPassword for 3rd party app launch.          4.1.2.1
' 04/18/2013    MSW     Try harder to give focus to the login window                  4.1.5.0
' 07/08/2013    MSW     Allow passed in name instead of process name                  4.1.5.1
' 11/21/2013    MSW     More attempts to send Password login to front                 4.1.6.0
' 02/05/2014    kdp     Commented out old "bring to front" changes.  
'                       Added LoadLogin to display and hide Password login
'                       LoadLogin added to PW4_main - subStartPaintworks              4.1.7.0
'*****************************************************************************************

Option Compare Binary
Option Explicit On
Option Strict On

Friend Class clsPWUser

#Region " Declares "

    Declare Function ShowWindowAsync Lib "user32.dll" ( _
        ByVal Hwnd As IntPtr, _
        ByVal swCommand As Int32) As IntPtr

    Declare Function SetForegroundWindow Lib "user32.dll" ( _
        ByVal Hwnd As IntPtr) As Boolean

    Private Const msMODULE As String = "clsPWUser"

    Private mbActionAllowed As Boolean
    Private mbUserLoggedIn As Boolean
    Private mPrivilege As ePrivilege
    Private mPrivileges() As udsPermission
    Private msLastLogin As String
    Private msLoggedInUser As String
    Private msProcName As String

    Private moPasswordHwnd As System.IntPtr

    Private Structure udsPermission
        Public Name As String
        Public ID As ePrivilege
        Public Allowed As Boolean
    End Structure

    Friend Event LogIn()
    Friend Event Logout()
    Friend Event Result(ByVal Action As String, ByVal ReturnValue As String)

#End Region

#Region " Properties "

    Friend ReadOnly Property ActionAllowed() As Boolean
        '****************************************************************************************
        'Description: Returns the Allowed state of the last requested permission.
        '
        'Parameters: none
        'Returns:    True/False
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return mbActionAllowed
        End Get
    End Property

    Friend ReadOnly Property LastLogIn() As String
        '****************************************************************************************
        'Description: Returns the Time and Date of UserNames's most recent previous LogIn.
        '
        'Parameters: none
        'Returns:    Last LogIn Tame and Date string
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return msLastLogin
        End Get
    End Property

    Friend ReadOnly Property Privilege() As ePrivilege
        '****************************************************************************************
        'Description: Returns the last requested permission.
        '
        'Parameters: none
        'Returns:    ePrivilege
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return mPrivilege
        End Get
    End Property

    Friend ReadOnly Property ProcessName() As String
        '****************************************************************************************
        'Description: Returns the Process Name for this application.
        '
        'Parameters: none
        'Returns:    Process Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return msProcName
        End Get

    End Property

    Friend ReadOnly Property UserName() As String
        '****************************************************************************************
        'Description: Returns the currently logged in User's UserName.
        '
        'Parameters: none
        'Returns:    UserName
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return msLoggedInUser
        End Get
    End Property

#End Region

#Region " Routines "

    Friend Overloads Function CheckPassword(ByVal PrivilegeRequested As ePrivilege) As Boolean
        '****************************************************************************************
        'Description: Return True if Privilege is allowed for the Logged In User. 
        '
        'Parameters: ePrivilege
        'Returns:    true/false
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        If UserName = String.Empty Then
            Dim sMessage() As String = {ProcessName, "displaylogin"}

            frmMain.oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sMessage)

            'MSW 04/18/13 Send Password login to front
            Dim oProcs() As Process = Process.GetProcessesByName("Password")
            If oProcs IsNot Nothing AndAlso oProcs.GetUpperBound(0) >= 0 Then
                moPasswordHwnd = oProcs(0).MainWindowHandle
                ShowWindowAsync(moPasswordHwnd, 5) 'SW_SHOW
                Threading.Thread.Sleep(10)
                SetForegroundWindow(moPasswordHwnd)
            End If

            Return False
        Else
            If PrivilegeRequested = ePrivilege.Special Then
                mDebug.WriteEventToLog(ProcessName & " Module: " & msMODULE & " Routine: CheckPassword", _
                                       "Ambiguous Privilege [ePrivilege.Special] cannot be evaluated. " & _
                                       "Returning Allowed = False.")
                Return False
            Else
                Dim bFound As Boolean

                For Each Permission As udsPermission In mPrivileges
                    If Permission.ID = PrivilegeRequested Then
                        bFound = True
                        mPrivilege = PrivilegeRequested
                        If UserName = "FANUC" Then
                            mbActionAllowed = True
                            Return True
                        Else
                            mbActionAllowed = Permission.Allowed
                            Return Permission.Allowed
                        End If
                        Exit For
                    End If
                Next 'Permission

                If Not bFound Then
                    Dim sPrivilege As String = PrivilegeEnumToString(PrivilegeRequested)
                    Dim sMessage() As String = {ProcessName, "privilege", sPrivilege}

                    'This is the first time this privilege was requested for this Process. In
                    'this case it will need to be registered by Password.NET.
                    frmMain.oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sMessage)
                    mPrivilege = PrivilegeRequested
                    If UserName = "FANUC" Then
                        mbActionAllowed = True
                    Else
                        mbActionAllowed = False
                    End If
                End If 'Not bFound
            End If 'PrivilegeRequested = ePrivilege.Special
        End If 'UserName = String.Empty

        Return False

    End Function

    Friend Overloads Function CheckPassword(ByVal PrivilegeName As String) As Boolean
        '****************************************************************************************
        'Description: Return True if PrivilegeName is allowed for the Logged In User. Note: This
        '             function is intended for Special permissions that are not part of the 
        '             standard ePrivilege set.
        '
        'Parameters: PrivilegeName
        'Returns:    true/false
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        If UserName = String.Empty Then
            Dim sMessage() As String = {ProcessName, "displaylogin"}

            frmMain.oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sMessage)

            'MSW 04/18/13 Send Password login to front
            Dim oProcs() As Process = Process.GetProcessesByName("Password")
            If oProcs IsNot Nothing AndAlso oProcs.GetUpperBound(0) >= 0 Then
                moPasswordHwnd = oProcs(0).Handle
                ShowWindowAsync(moPasswordHwnd, 5) 'SW_SHOW
                Threading.Thread.Sleep(10)
                SetForegroundWindow(moPasswordHwnd)
            End If

            Return False
        Else
            Dim bFound As Boolean

            For Each Permission As udsPermission In mPrivileges
                If Permission.Name = PrivilegeName.ToLower Then
                    bFound = True
                    mPrivilege = Permission.ID
                    If UserName = "FANUC" Then
                        mbActionAllowed = True
                        Return True
                    Else
                        mbActionAllowed = Permission.Allowed
                        Return Permission.Allowed
                    End If
                    Exit For
                End If
            Next 'Permission

            If Not bFound Then
                Dim PrivilegeID As ePrivilege = PrivilegeStringToEnum(PrivilegeName)
                Dim sMessage() As String = {ProcessName, "privilege", PrivilegeName}

                'This is the first time this privilege was requested for this Process. In
                'this case it will need to be registered by Password.NET.
                frmMain.oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sMessage)
                mPrivilege = PrivilegeID
                If UserName = "FANUC" Then
                    mbActionAllowed = True
                Else
                    mbActionAllowed = False
                End If
            End If
        End If

        Return mbActionAllowed

    End Function

    Friend Overloads Function CheckPassword(ByVal Appname As String, ByVal PrivilegeRequested As ePrivilege) As Boolean

        '****************************************************************************************
        'Description: Returns True to indicate request was sent to Password application to 
        '             determine if the LoggedInUser has the Privilege to execute the AppName 3rd 
        '             party application. 
        '
        'Parameters: 3rd party App Name, ePrivilege
        'Returns:    true/false
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/13/12  RJO     Added sPrivilege to sMessage.
        '*****************************************************************************************

        If UserName = String.Empty Then
            Dim sMessage() As String = {ProcessName, "displaylogin"}

            frmMain.oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sMessage)
            'MSW 04/18/13 Send Password login to front
            Dim oProcs() As Process = Process.GetProcessesByName("Password")
            If oProcs IsNot Nothing AndAlso oProcs.GetUpperBound(0) >= 0 Then
                moPasswordHwnd = oProcs(0).Handle
                ShowWindowAsync(moPasswordHwnd, 5) 'SW_SHOW
                Threading.Thread.Sleep(10)
                SetForegroundWindow(moPasswordHwnd)
            End If

            Return False
        Else
            Dim sPrivilege As String = PrivilegeEnumToString(PrivilegeRequested)
            Dim sMessage() As String = {ProcessName, "getlaunchpriv", sPrivilege, "", "", "", "", Appname}

            frmMain.oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sMessage)
        End If

        Return True

    End Function
    Friend Sub LoadLogin()
        '****************************************************************************************
        'Description: Sends a message to the Password.NET application to show the LogIn screen.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date 02/05/2014      By kdp      Reason Added this to work around for Password focus issue
        '*****************************************************************************************
        Dim sMessage() As String = {ProcessName, "displaylogin"}
        Dim sMessage2() As String = {ProcessName, "hidelogin"}
        frmMain.oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sMessage)
        frmMain.oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sMessage2)

    End Sub
    Friend Sub DisplayLogin()
        '****************************************************************************************
        'Description: Sends a message to the Password.NET application to show the LogIn screen.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim sMessage() As String = {ProcessName, "displaylogin"}

        frmMain.oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sMessage)

        'MSW 04/18/13 Send Password login to front
        'MSW 11/21/13 More attempts to send Password login to front

        'kdp 02/05/14 - Commented out
        ' Dim oProcs() As Process = Process.GetProcessesByName("Password")
        ' If oProcs IsNot Nothing AndAlso oProcs.GetUpperBound(0) >= 0 Then
        ' moPasswordHwnd = oProcs(0).Handle
        ' ShowWindowAsync(moPasswordHwnd, 5) 'SW_SHOW
        ' Threading.Thread.Sleep(10)


            'SetForegroundWindow(moPasswordHwnd)

            'This is probably overkill, just hitting it with everything we can to get the desired program in front
            'ShowWindowAsync(oProcs(0).MainWindowHandle, 9) 'Restore
        'SW_SHOW = 5, SW_MINIMIZE = 6, SW_SHOWMINNOACTIVE = 7, SW_SHOWNA = 8, SW_RESTORE = 9, SW_SHOWDEFAULT = 10, 
        'SW_FORCEMINIMIZE = 11, SW_MAX = 11 } [DllImport("shell32.dll")] static extern IntPtr 
            'ShellExecute( IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirecto...
            'ShowWindowAsync(hWnd, SW_SHOW);
            'This is probably overkill, just hitting it with everything we can to get the desired program in front
        'ShowWindowAsync(oProcs(0).MainWindowHandle, 5) 'SW_SHOW
        'Threading.Thread.Sleep(10)
            'ShowWindowAsync(hWnd, SW_RESTORE);

        'kdp 02/05/14 - Commented out
        ' ShowWindowAsync(oProcs(0).MainWindowHandle, 9) 'SW_RESTORE
        ' ShowWindowAsync(oProcs(0).MainWindowHandle, 5) 'SW_SHOW
        ' Threading.Thread.Sleep(10)
        ' SetForegroundWindow(oProcs(0).MainWindowHandle)
        ' End If


    End Sub

    Friend Sub HideLogin()
        '****************************************************************************************
        'Description: Sends a message to the Password.NET application to hide the LogIn screen.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim sMessage() As String = {ProcessName, "hidelogin"}

        frmMain.oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sMessage)

    End Sub

    Friend Sub LogUserOut()
        '****************************************************************************************
        'Description: Command password.NET to  Log Out the current user. 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim sMessage() As String = {ProcessName, "logout"}

        frmMain.oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sMessage)

    End Sub

    Private Function PrivilegeEnumToString(ByVal ePrivilege As ePrivilege) As String
        '****************************************************************************************
        'Description: Convert a privilege enuneration to a string
        '
        'Parameters: enum
        'Returns:   string
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Select Case ePrivilege

            Case ePrivilege.Administrate
                Return "administrate"
            Case ePrivilege.Copy
                Return "copy"
            Case ePrivilege.Delete
                Return "delete"
            Case ePrivilege.Edit
                Return "edit"
            Case ePrivilege.Execute
                Return "execute"
            Case ePrivilege.Operate
                Return "operate"
            Case ePrivilege.Remote
                Return "remote"
            Case ePrivilege.Restore
                Return "restore"
            Case ePrivilege.Special
                Return "special"
            Case Else
                Return "none"
        End Select

    End Function

    Private Function PrivilegeStringToEnum(ByVal sPrivilege As String) As ePrivilege
        '****************************************************************************************
        'Description: Convert a Privilege string to a corresponding ePrivilege value 
        '
        'Parameters: string
        'Returns:   enum
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Select Case LCase(sPrivilege)
            Case "administrate"
                Return ePrivilege.Administrate
            Case "copy"
                Return ePrivilege.Copy
            Case "delete"
                Return ePrivilege.Delete
            Case "edit"
                Return ePrivilege.Edit
            Case "execute"
                Return ePrivilege.Execute
            Case "operate"
                Return ePrivilege.Operate
            Case "remote"
                Return ePrivilege.Remote
            Case "restore"
                Return ePrivilege.Restore
            Case String.Empty
                Return ePrivilege.None
            Case Else
                Return ePrivilege.Special
        End Select

    End Function

    Friend Sub ProcessPasswordMessage(ByVal DR As DataRow)
        '****************************************************************************************
        'Description: A message has been received by the host application from the Password.NET 
        '             application or the Paintworks Main application.
        '
        'Parameters: Password message DataRow
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim sAction As String = DR.Item("Action").ToString
        Dim sScreen() As String = Strings.Split(ProcessName.ToLower, "(")

        Select Case sAction.ToLower
            Case "adduser"
                If sScreen(0) = gs_COM_ID_PWUSERMAINT.ToLower Then
                    RaiseEvent Result("adduser", DR.Item("Parameter").ToString)
                End If
            Case "changepwd"
                If sScreen(0) = gs_COM_ID_PWUSERMAINT.ToLower Then
                    RaiseEvent Result("changepwd", DR.Item("Parameter").ToString)
                End If
            Case "deleteuser"
                If sScreen(0) = gs_COM_ID_PWUSERMAINT.ToLower Then
                    RaiseEvent Result("deleteuser", DR.Item("Parameter").ToString)
                End If
            Case "initialized"
                msLoggedInUser = DR.Item("UserName").ToString
                msLastLogin = DR.Item("LastLogin").ToString
                Call subSetPrivileges(DR.Item("Privilege").ToString)
                If msLoggedInUser <> String.Empty Then RaiseEvent LogIn()
            Case "launchpriv"
                If sScreen(0) = gs_COM_ID_PW_MAIN.ToLower Then
                    RaiseEvent Result("launchpriv", DR.Item("Privilege").ToString)
                End If
            Case "login"
                msLoggedInUser = DR.Item("UserName").ToString
                msLastLogin = DR.Item("LastLogin").ToString
                Call subRequestPrivileges(String.Empty)
                mbUserLoggedIn = True
            Case "logout"
                msLoggedInUser = String.Empty
                msLastLogin = String.Empty
                Call subResetPrivileges()
                RaiseEvent Logout()
            Case "privileges"
                Call subSetPrivileges(DR.Item("Privilege").ToString)
                If mbUserLoggedIn Then
                    RaiseEvent LogIn()
                    mbUserLoggedIn = False
                End If
            Case "screenames"
                If sScreen(0) = gs_COM_ID_REPORTS.ToLower Then
                    RaiseEvent Result("screenames", DR.Item("Parameter").ToString)
                End If
            Case "usernames"
                If (sScreen(0) = gs_COM_ID_PWUSERMAINT.ToLower) Or (sScreen(0) = gs_COM_ID_REPORTS.ToLower) Then
                    RaiseEvent Result("usernames", DR.Item("UserName").ToString)
                End If
            Case Else
                mDebug.WriteEventToLog(ProcessName & " Module: " & msMODULE & " Routine: subDoPasswordAction", _
                                       "Unrecognized command [" & sAction & "] received.")
        End Select

    End Sub

    Private Sub subRequestPrivileges(ByVal Privilege As String)
        '****************************************************************************************
        'Description: Request the Logged In User's privilege set for this Process. If privilege
        '             is not an empty string, Password.NET will add the privilege for this
        '             Process if it doesn't exist.
        '
        'Parameters: Privilege name to verify
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim sMessage() As String = {ProcessName, "privilege"}

        frmMain.oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sMessage)

    End Sub

    Private Sub subResetPrivileges()
        '****************************************************************************************
        'Description: Reset the mPrivleges structure and the last Privilege requested info.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        ReDim mPrivileges(0)

        With mPrivileges(0)
            .Name = String.Empty
            .ID = ePrivilege.None
            .Allowed = False
        End With

        mPrivilege = ePrivilege.None
        mbActionAllowed = False

    End Sub

    Private Sub subSetPrivileges(ByVal Privileges As String)
        '****************************************************************************************
        'Description: Build the mPrivleges structure from the Privileges string returned by
        '             Password.NET. The Privileges string is separated by commas and is 
        '             structured as folows: <name1>,<allowed1>,<name2>,<allowed2>,etc...
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Call subResetPrivileges()

        Try
            If Privileges <> String.Empty Then
                Dim nIndex As Integer
                Dim sData() As String = Strings.Split(Privileges, ",")

                For nItem As Integer = 0 To sData.GetUpperBound(0) Step 2
                    ReDim Preserve mPrivileges(nIndex)

                    With mPrivileges(nIndex)
                        .Name = sData(nItem).ToLower
                        .ID = PrivilegeStringToEnum(.Name)
                        .Allowed = CType(sData(nItem + 1), Boolean)
                    End With
                    nIndex += 1
                Next 'nItem
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(ProcessName & " Module: " & msMODULE & " Routine: subSetPrivileges", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

#End Region

#Region " Events "

    Public Sub New(Optional ByRef sProcName As String = "")
        '****************************************************************************************
        'Description: Initialize the new instance of this class.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 07/08/13  MSW     Allow passed in name instead of processname
        '*****************************************************************************************
        If sProcName = "" Then
            sProcName = Process.GetCurrentProcess.ProcessName
        End If

        Dim sProcID As String = "(" & Process.GetCurrentProcess.Id.ToString & ")"
        Dim nDot As Integer = Strings.InStr(sProcName, ".")

        'Get the process name for messages. Strip off ".vshost" if running in development mode.
        If nDot > 0 Then
            msProcName = Strings.Left(sProcName, nDot - 1)
        Else
            msProcName = sProcName
        End If

        msProcName = msProcName & sProcID

        'Initialize the Privilege Set
        Call subResetPrivileges()

        'Get initialization info from Password.NET
        Dim sMessage() As String = {ProcessName, "initialize"}

        frmMain.oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sMessage)

        'MSW 04/18/13 Send Password login to front
        Dim oProcs() As Process = Process.GetProcessesByName("Password")
        If oProcs IsNot Nothing AndAlso oProcs.GetUpperBound(0) >= 0 Then
            moPasswordHwnd = oProcs(0).Handle
        End If
    End Sub

#End Region

End Class
