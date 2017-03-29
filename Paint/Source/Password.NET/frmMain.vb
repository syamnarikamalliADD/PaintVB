'*** Notes **************************************************************************
'PasswordMsg
'   0 - ProcName
'   1 - Action
'   2 - Privilege
'   3 - Password
'   4 - UserName
'   5 - LastLogin
'   6 - Group
'   7 - Parameter

'Add this enum to mDeclares
'Friend Enum ePrivilege As Integer
'    None = 0
'    Edit = 1
'    Copy = 2
'    Delete = 3
'    Administrate = 4
'    Restore = 5
'    Execute = 6
'    Remote = 7
'    Operate = 8
'    Special = 9
'End Enum
'*** End Notes **********************************************************************


' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' America or FANUC LTD Japan immediately upon request. This material
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
' Form/Module: Password
'
' Description: Password application available to all Paintworks screens
'              using clsInterProcessComm
' 
'
' Dependancies:  
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
'    Date       By      Reason                                                              Version 
' 03/09/2012    RJO     Initial code                                                        4.1.2.0
' 03/29/2012    MSW     Set the caption and icon for frmMain                                4.1.3.0
' 04/11/2012    RJO     Set custom passphrase for crypto.dll 4.1.2.0                        4.1.3.1
' 04/13/2012    RJO     Bug fixes in GetLaunchPrivilege.                                    4.1.3.2
' 10/24/2012    RJO     Added StartupModule to project to prevent multiple instances        4.1.3.3
' 11/15/2012    HGB     Updates to subDoPasswordAction and ChangePassword that allow        4.1.3.4 
'                       another users password to be changed properly. The username of the 
'                       user to change was not being passed, this resulted in the current
'                       user’s password being changed.
' 04/16/2013    MSW     Add Canadian language files                                      4.01.05.00
'    07/09/13   MSW     Update and standardize logos                                     4.01.05.01
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up   4.01.05.02
'    09/30/13   MSW     Use the control's min and max instead of literal constants       4.01.06.00
'    11/21/13   MSW     More attempts to get focus on the login prompt                   4.01.06.01
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call                 4.01.07.00
'***************************************************************************************************

Option Compare Text
Option Explicit On
Option Strict On

Friend Class frmMain

#Region " Declares "

    '******** Form Constants ************************************************************************
    Friend Const msSCREEN_NAME As String = "Password"
    Private Const msMODULE As String = "frmMain"
    Private Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Private Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Private Const msROBOT_ASSEMBLY_LOCAL As String = ""
    '******** End Form Constants ********************************************************************

    '******** Form Variables ************************************************************************
    Private mbWarningShown As Boolean
    Private mdtLogoutWarnTime As DateTime
    Private mdtLogoutTime As DateTime
    Private WithEvents moIPC As Paintworks_IPC.clsInterProcessComm

    Private msLoginProcName As String
    '******** End Form Variables ********************************************************************

    '******** Property Variables ********************************************************************
    Private mbAutoLogoutEnable As Boolean
    Private mbIsUserFANUC As Boolean

    Private mLoggedInUser As udsUserInfo
    Private mnAutoLogoutTimeout As Integer
    Private mnAutoLogoutWarning As Integer
    Private mnProgress As Integer
    Private msCulture As String = "en-US" 'Default to english form text
    Private msIpcPath As String
    Private msPassword As String
    Private msXmlPath As String
    '******** End Property Variables ****************************************************************

    '******** Structures ****************************************************************************
    Private Structure udsPrivilege
        Public ID As Integer
        Public Name As String
    End Structure

    Private Structure udsUserInfo
        Public UserID As Integer
        Public GroupID As Integer
        Public Name As String
        Public Password As String
        Public LastLogin As String
    End Structure
    '******** End Structures     ********************************************************************

    '******** Delegates    **************************************************************************
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet)
    '******** End Delegates    **********************************************************************

#End Region

#Region " Properties "

    Private ReadOnly Property AutoLogoutEnable() As Boolean
        '********************************************************************************************
        'Description: Returns True if Auto LogOut is enabled.
        '
        'Parameters: None
        'Returns:    True/False
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbAutoLogoutEnable
        End Get
    End Property

    Private ReadOnly Property AutoLogoutTimeout() As Double
        '********************************************************************************************
        'Description: Returns the Auto LogOut timeout value in seconds.
        '
        'Parameters: None
        'Returns:    Timeout value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return CType(mnAutoLogoutTimeout, Double)
        End Get
    End Property

    Private ReadOnly Property AutoLogoutWarning() As Double
        '********************************************************************************************
        'Description: Returns the Auto LogOut Timeout Warning value in seconds.
        '
        'Parameters: None
        'Returns:    Timeout Warning value
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return CType(mnAutoLogoutWarning, Double)
        End Get
    End Property

    Friend WriteOnly Property Culture() As String
        '********************************************************************************************
        'Description:  Write to this property to change the screen language.
        '
        'Parameters: Culture String (ex. "en-US")
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As String)

            msCulture = value
            mLanguage.DisplayCultureString = value

            'Use current language text for screen labels
            Dim Void As Boolean = mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, _
                                                                msBASE_ASSEMBLY_LOCAL, _
                                                                msROBOT_ASSEMBLY_LOCAL)

        End Set

    End Property

    Friend ReadOnly Property DisplayCulture() As System.Globalization.CultureInfo
        '********************************************************************************************
        'Description:  The Culture Club
        '
        'Parameters: None
        'Returns:    CultureInfo for current culture.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return New System.Globalization.CultureInfo(msCulture)
        End Get

    End Property

    Private ReadOnly Property IPCPath() As String
        '********************************************************************************************
        'Description:  Returns the path to the Inter Process Communication folder.
        '
        'Parameters: None
        'Returns:    Path
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msIpcPath
        End Get
    End Property

    Private ReadOnly Property IsUserFANUC() As Boolean
        '********************************************************************************************
        'Description: Returns True if user FANUC is logged in.
        '
        'Parameters:  None
        'Returns:     None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbIsUserFANUC
        End Get
    End Property

    Private Property LoggedInUser() As udsUserInfo
        '********************************************************************************************
        'Description: Sets/Gets logged in user information.
        '
        'Parameters: None
        'Returns:    User Info
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mLoggedInUser
        End Get
        Set(ByVal value As udsUserInfo)
            mLoggedInUser = value
        End Set
    End Property

    Friend WriteOnly Property Password() As String
        '********************************************************************************************
        'Description: A user has attempted to Log In with the password stored in value.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As String)
            msPassword = value
            Call subCheckUserFANUC()

            If IsUserFANUC Then
                LoggedInUser = GetFanucUserInfo()
            Else
                LoggedInUser = GetUserInfo()
            End If

            If LoggedInUser.Name = String.Empty Then
                Dim Response As DialogResult
                Dim sMsg As String = gpsRM.GetString("psINVALID_PWD_MSG")
                Dim sCaption As String = gpsRM.GetString("psPWD_NOT_FOUND_CAP")

                'Login failed. Put up message box and wait for response
                Response = MessageBox.Show(sMsg, sCaption, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation)
                If Response <> Windows.Forms.DialogResult.OK Then
                    frmLogIn.Close()
                End If
            Else
                Dim sMessage() As String = {gs_COM_ID_PASSWORD, "login", "", "", LoggedInUser.Name, LoggedInUser.LastLogin}

                If moIPC.GetProcess(gs_COM_ID_PW_MAIN) Is Nothing Then
                    'PW4_Main not running. Send "logout" message to Procces that user logger out from
                    moIPC.WritePasswordMsg(msLoginProcName, sMessage)
                Else
                    'Send "logout" message to PW4_Main who will broadcast it to any running screens.
                    moIPC.WritePasswordMsg(gs_COM_ID_PW_MAIN, sMessage)
                End If
                frmLogIn.Close()
                Call ResetAutoLogOut()
            End If
        End Set
    End Property

    Friend Property Progress() As Integer
        '********************************************************************************************
        'Description:  run the progress bar
        '
        'Parameters: 1 to 100 percent
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/20/13  MSW     Add error handler so it doesn't get hung up during exit
        ' 09/30/13  MSW     Use the control's min and max instead of literal constants
        '********************************************************************************************
        Set(ByVal Value As Integer)
            Try
                If Value < tspProgress.Minimum Then Value = tspProgress.Minimum
                If Value > tspProgress.Maximum Then Value = tspProgress.Minimum
                mnProgress = Value
                tspProgress.Value = mnProgress
                If mnProgress > 0 And mnProgress < 100 Then
                    tspProgress.Visible = True
                Else
                    tspProgress.Visible = False
                End If
                stsStatus.Invalidate()
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Property: Progress", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
        Get
            Return mnProgress
        End Get
    End Property

    Friend Property Status(Optional ByVal StatusStripOnly As Boolean = False) As String
        '********************************************************************************************
        'Description:  Write status messages to listbox and statusbar - Included for compatibility  
        '              with Paintworks common modules.
        '
        'Parameters: If StatusbarOnly is true, doesn't write to listbox
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return stsStatus.Text
        End Get
        Set(ByVal Value As String)
            stsStatus.Items("lblStatus").Text = Strings.Replace(Value, vbTab, "  ")
        End Set
    End Property

    Private ReadOnly Property XMLPath() As String
        '********************************************************************************************
        'Description:  Returns the path to the XML database files folder.
        '
        'Parameters: None
        'Returns:    Path
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msXmlPath
        End Get
    End Property

#End Region

#Region " Routines "

    Private Function AddUser(ByVal UserName As String, ByVal Password As String, ByVal GroupID As Integer) As String
        '****************************************************************************************
        'Description: Add a new user if UserName, Password and GroupID are valid.
        '
        'Parameters: New UserName, Password (not encrypted) and GroupID
        'Returns:    Result string
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim bValid As Boolean
        Dim sResult As String = "success"

        If (UserName = String.Empty) Or (Password = String.Empty) Then
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: AddUser", _
                                   "Username and/or Password cannot be an empty string.")
            Return "emptystring"
        End If
        If UserName.ToLower = "fanuc" Then
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: AddUser", _
                                   "Username FANUC is reserved.")
            Return "namena"
        End If
        If Strings.Left(Password, 4).ToLower = "frna" Then
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: AddUser", _
                                   "Supplied Password is reserved.")
            Return "pwdna"
        End If

        Try
            Dim DS As DataSet = GetDataSet()
            Dim DT As DataTable
            Dim DR As DataRow

            If GroupID = 0 Then
                'Custom privileges
                bValid = True
            Else
                'verify GroupID
                DT = DS.Tables("Groups")
                Dim nGIDs() As Integer = GetIDs(DT, "GroupID")

                For Each nID As Integer In nGIDs
                    If GroupID = nID Then
                        bValid = True
                    End If
                Next
            End If

            If Not bValid Then
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: AddUser", _
                                       "Supplied Group ID is invalid.")
                Return "failed"
            End If

            For Each DR In DS.Tables("Users").Rows
                Dim sName As String = Crypto.Crypto.Decrypt(DR.Item("Name").ToString)
                Dim sPwd As String = Crypto.Crypto.Decrypt(DR.Item("Password").ToString)

                'verify UserName and Password do not already exist
                If sName.ToLower = UserName.ToLower Then
                    bValid = False
                    sResult = "namena"
                ElseIf sPwd.ToLower = Password.ToLower Then
                    bValid = False
                    sResult = "pwdna"
                End If
                If Not bValid Then
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: AddUser", _
                                           "Username and/or Password in use.")
                    Return sResult
                End If
            Next 'DR

            DT = DS.Tables("Users")
            DR = DT.NewRow
            Dim DRs() As DataRow
            Dim nUserID As Integer = GetUniqueID(DT, "UserID")

            'Add new user to Users table. Encrypt UserName and Password, assign next 
            'available UserID.
            DR.Item("UserID") = nUserID
            DR.Item("GroupID") = GroupID
            DR.Item("Name") = Crypto.Crypto.Encrypt(UserName)
            DR.Item("Password") = Crypto.Crypto.Encrypt(Password)
            DR.Item("LastLogin") = gcsRM.GetString("csNONE")
            DT.Rows.Add(DR)
            DT.AcceptChanges()

            'Create privilege set for new user
            DT = DS.Tables("Access_List")

            If GroupID = 0 Then
                DRs = DS.Tables("Privileges").Select()
            Else
                DRs = DS.Tables("Group_Access_List").Select("GroupID = " & GroupID.ToString)
            End If

            For Each DRP As DataRow In DRs
                DR = DT.NewRow
                DR.Item("UserID") = nUserID
                DR.Item("PrivilegeID") = DRP.Item("PrivilegeID")
                If GroupID = 0 Then
                    DR.Item("Allowed") = False
                Else
                    DR.Item("Allowed") = DRP.Item("Allowed")
                End If
                DT.Rows.Add(DR)
                DT.AcceptChanges()
            Next

            DS.WriteXml(XMLPath & "PWUser.xml")

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: AddUser", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            sResult = "failed"
        End Try

        Return sResult

    End Function

    Private Function ChangePassword(ByRef NewPassword As String, ByRef OldPassword As String, _
                                    ByVal Username As String) As String
        '****************************************************************************************
        'Description: Attempt to change the user's password and then return a result string.
        '
        'Parameters: New Password, Old Password
        'Returns:    Result string
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/15/12  HGB     Added username param to allow an existing user to be edited
        '*****************************************************************************************
        Dim sSResult As String = "success"

        Try
            Dim DS As DataSet = GetDataSet()
            Dim DT As DataTable = DS.Tables("Users")
            Dim sUsername As String '= Crypto.Crypto.Encrypt(LoggedInUser.Name) 'HGB 11/15/12

            If Username = String.Empty Then
                sUsername = Crypto.Crypto.Encrypt(LoggedInUser.Name)
            Else
                sUsername = Crypto.Crypto.Encrypt(Username)
            End If

            Dim Rows() As DataRow = DT.Select("Name = '" & sUsername & "'")

            'Old Password will be empty if this is called from the PWUserMaint User Privileges Tab
            If OldPassword <> String.Empty Then
                'Make sure the Current Password matches the logged in user's password
                If Rows.Count > 0 Then
                    Dim sPwd As String = Crypto.Crypto.Encrypt(OldPassword)

                    If sPwd <> Rows(0).Item("Password").ToString Then
                        mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: ChangePassword", _
                                               "Incorrect Password for logged in user [" & LoggedInUser.Name & "].")
                        Return "invaliduser"
                    End If
                Else
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: ChangePassword", _
                                           "Cannot locate user [" & LoggedInUser.Name & "] in database.")
                    Return "failed"
                End If
            End If

            'Make sure the New Password is not reserved.
            If Strings.Left(NewPassword, 4).ToLower = "frna" Then
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: ChangePassword", _
                                       "Requested New Password is reserved.")
                Return "pwdna"
            End If

            'Make sure the New Password is unique. Even though passwords are case sensitive, we don't want
            'one user with a passerord like "User1" while another user's password is "user1" so convert to 
            'lower case before comparing.
            For Each DR As DataRow In DT.Rows
                Dim sPwd As String = Crypto.Crypto.Decrypt(DR.Item("Password").ToString).ToLower

                If sPwd = NewPassword.ToLower Then
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: ChangePassword", _
                                           "Requested New Password is not unique.")
                    Return "pwdna"
                End If

            Next 'DR

            'Change the password
            Rows(0).Item("Password") = Crypto.Crypto.Encrypt(NewPassword)
            DS.AcceptChanges()
            DS.WriteXml(XMLPath & "PWUser.xml")

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: ChangePassword", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            sSResult = "failed"
        End Try

        Return sSResult

    End Function

    Private Function CheckUserFANUC() As Boolean
        '****************************************************************************************
        'Description: Did user FANUC just Log In?
        '
        'Parameters: none
        'Returns:    True if user FANUC logged in
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        If msPassword = "FRNA" & Now.ToString("HHmm") Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Function GetDataSet() As DataSet
        '****************************************************************************************
        'Description: Returns the entire PWUser dataset.
        '
        'Parameters: none
        'Returns:    PWUser DataSet
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim DS As New DataSet

        DS.ReadXmlSchema(XMLPath & "PWUser.XSD")
        DS.ReadXml(XMLPath & "PWUser.XML")

        Return DS

    End Function

    Private Function GetFanucUserInfo() As udsUserInfo
        '****************************************************************************************
        'Description: User FANUC is omnipotent (in Paintworks). Returns FANUC user info and 
        '             updates LastLogin in PWUser database.
        '
        'Parameters: none
        'Returns:    LoggedInUser Info for user FANUC
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim sUserName As String = Crypto.Crypto.Encrypt("FANUC")
        Dim UserInfo As udsUserInfo = NewUser()

        Try
            Dim bExists As Boolean
            Dim DS As DataSet = GetDataSet()
            Dim DT As DataTable = DS.Tables("Users")

            For Each DR As DataRow In DT.Rows
                If DR.Item("Name").ToString = sUserName Then
                    With UserInfo
                        .UserID = CType(DR.Item("UserID"), Integer)
                        .GroupID = CType(DR.Item("GroupID"), Integer)
                        .Name = "FANUC"
                        .Password = String.Empty
                        .LastLogin = DR.Item("LastLogin").ToString
                    End With

                    'update LastLogin
                    DR.Item("LastLogin") = Now.ToShortDateString & " " & Now.ToLongTimeString
                    DT.AcceptChanges()
                    DS.WriteXml(XMLPath & "PWUser.xml")

                    bExists = True
                    Exit For
                End If
            Next

            If Not bExists Then
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: GetFanucUserInfo", _
                                       "Cannot locate user FANUC in database.")
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: GetFanucUserInfo", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

        Return UserInfo

    End Function

    Private Function GetIDs(ByRef DT As DataTable, ByVal Item As String) As Integer()
        '****************************************************************************************
        'Description: Returns an array of GroupIDs, PrivilegeIDs, or UserIDs from DT
        '
        'Parameters: ProcessName - Screen Name
        'Returns:    Privileges (text) separated by commas
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim nIDs(0) As Integer
        Dim nItem As Integer

        For Each DR As DataRow In DT.Rows
            ReDim Preserve nIDs(nItem)
            nIDs(nItem) = CType(DR.Item(Item), Integer)
            nItem += 1
        Next

        Return nIDs

    End Function

    Private Function GetLaunchPrivilege(ByVal AppName As String, ByVal Privilege As String) As Boolean
        '****************************************************************************************
        'Description: Returns True if the LoggedInUser has privilege to launch 3rd party 
        '             AppName.
        '
        'Parameters: AppName - 3rd Party Application Screen Name
        '            Privilege - Requested Privilege
        'Returns:    Privilege granted True/False
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/13/12  RJO     Bug fixes. User FANUC is always OK. Added UserID to Access_List query.
        '*****************************************************************************************
        Dim bAllowed As Boolean

        Try
            If IsUserFANUC Then
                Return True
            Else
                Dim DS As DataSet = GetDataSet()
                Dim PDRs() As DataRow = DS.Tables("Privileges").Select("ProcessName = '" & AppName & "' AND Privilege = '" & Privilege & "'")

                If Not PDRs Is Nothing Then
                    Dim nPrivID As Integer = CType(PDRs(0).Item("PrivilegeID"), Integer)
                    Dim nUserID As Integer = LoggedInUser.UserID
                    Dim ALRs() As DataRow = DS.Tables("Access_List").Select("PrivilegeID = " & nPrivID.ToString & " AND UserID = " & nUserID.ToString)

                    If Not ALRs Is Nothing Then
                        bAllowed = CType(ALRs(0).Item("Allowed"), Boolean)
                    End If
                End If 'Not PDRs Is Nothing
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: GetLaunchPrivilege", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

        Return bAllowed

    End Function

    Private Function GetPrivileges(ByVal ProcessName As String) As String
        '****************************************************************************************
        'Description: Returns the Privileges the LoggedInUser has for this screen.
        '
        'Parameters: ProcessName - Screen Name
        'Returns:    Privileges (text) separated by commas
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim DS As DataSet = GetDataSet()
        Dim sPrivileges As String = String.Empty
        Dim Privileges(0) As udsPrivilege
        Dim nIndex As Integer

        Try
            'get the list of privileges associated with this process (screen)
            For Each DR As DataRow In DS.Tables("Privileges").Rows
                Dim sProcName As String = DR.Item("ProcessName").ToString

                If sProcName.ToLower = ProcessName.ToLower Then
                    ReDim Preserve Privileges(nIndex)

                    Privileges(nIndex).ID = CType(DR.Item("PrivilegeID"), Integer)
                    Privileges(nIndex).Name = DR.Item("Privilege").ToString
                    nIndex += 1
                End If
            Next

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: GetPrivileges", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

        If nIndex > 0 Then
            If IsUserFANUC Then
                'for user FANUC, all privileges are Allowed
                For nIndex = 0 To Privileges.GetUpperBound(0)
                    If sPrivileges.Length > 0 Then sPrivileges = sPrivileges & ","
                    sPrivileges = sPrivileges & Privileges(nIndex).Name & ",true"
                Next
            Else
                Try
                    Dim DT As DataTable = DS.Tables("Access_List")
                    Dim sFilter As String = "UserID = " & LoggedInUser.UserID
                    Dim DRs() As DataRow = DT.Select(sFilter, "PrivilegeID ASC")

                    'other users only get allowed privileges
                    For Each DR As DataRow In DRs
                        Dim nPrivID As Integer = CType(DR.Item("PrivilegeID"), Integer)

                        For nIndex = 0 To Privileges.GetUpperBound(0)
                            If Privileges(nIndex).ID = nPrivID Then
                                If sPrivileges.Length > 0 Then sPrivileges = sPrivileges & ","
                                sPrivileges = sPrivileges & Privileges(nIndex).Name & "," & DR.Item("Allowed").ToString
                            End If
                        Next
                    Next

                Catch ex As Exception
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: GetPrivileges", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try

            End If 'IsUserFANUC
        End If 'Privileges(0).Name.Length > 0

        Return sPrivileges

    End Function

    Private Function GetScreenNames() As String
        '****************************************************************************************
        'Description: Returns a comma separated string of all Screen (Process) Names.
        '
        'Parameters: None
        'Returns:    Screen Names
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim sScreens As String = String.Empty

        Try
            Dim DS As DataSet = GetDataSet()

            For Each DR As DataRow In DS.Tables("Processes").Rows
                Dim sScreenName As String = DR.Item("ProcessName").ToString

                If sScreens.Length > 0 Then sScreens = sScreens & ","
                sScreens = sScreens & sScreenName
            Next 'DR

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: GetScreenNames", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

        Return sScreens

    End Function

    Private Function GetUniqueID(ByRef DT As DataTable, ByVal Item As String) As Integer
        '****************************************************************************************
        'Description: Returns the next available unassigned GroupID, PrivilegeID or UserID as 
        '             specified in Item.
        '
        'Parameters: Datatable and Item to check
        'Returns:    Next available ID
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim nID As Integer = 1

        If DT.Rows.Count > 0 Then
            Dim nIDs() As Integer = GetIDs(DT, Item)
            Dim bFound As Boolean = True

            Do 'search the table for the first available unassigned ID
                bFound = False
                For Each nItem As Integer In nIDs
                    If nID = nItem Then
                        nID += 1
                        bFound = True
                        Exit For
                    End If
                Next 'nItem
            Loop Until bFound = False
        End If

        Return nID

    End Function

    Private Function GetUserInfo() As udsUserInfo
        '****************************************************************************************
        'Description: Returns user info from the database for the logged in user and then updates 
        '             LastLogin for this user in PWUser database.
        '
        'Parameters: none
        'Returns:    LoggedInUser Info for user FANUC
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim sPassword As String = Crypto.Crypto.Encrypt(msPassword)
        Dim UserInfo As udsUserInfo = NewUser()

        Try
            Dim DS As DataSet = GetDataSet()
            Dim DT As DataTable = DS.Tables("Users")

            For Each DR As DataRow In DT.Rows
                If DR.Item("Password").ToString = sPassword Then
                    With UserInfo
                        .UserID = CType(DR.Item("UserID"), Integer)
                        .GroupID = CType(DR.Item("GroupID"), Integer)
                        .Name = Crypto.Crypto.Decrypt(DR.Item("Name").ToString)
                        .Password = msPassword 'TODO - Is this necessary??
                        .LastLogin = DR.Item("LastLogin").ToString
                    End With

                    'update LastLogin
                    DR.Item("LastLogin") = Now.ToShortDateString & " " & Now.ToLongTimeString
                    DT.AcceptChanges()
                    DS.WriteXml(XMLPath & "PWUser.xml")

                    Exit For
                End If
            Next 'DR

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: GetUserInfo", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

        Return UserInfo

    End Function

    Private Function GetUserNames() As String
        '****************************************************************************************
        'Description: Returns a comma separated string of all UserNames except "FANUC", (decrypted)
        '             and their associated UserdIDs and GroupIDs. (ex. "Name1,ID1,GID1,Name2,ID2,GID2")
        'Parameters: None
        'Returns:    UserNames
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim sUsers As String = String.Empty

        Try
            Dim DS As DataSet = GetDataSet()

            For Each DR As DataRow In DS.Tables("Users").Rows
                Dim sUserName As String = Crypto.Crypto.Decrypt(DR.Item("Name").ToString)
                Dim sIDs As String = DR.Item("UserID").ToString & "," & DR.Item("GroupID").ToString

                If sUserName <> "FANUC" Then
                    If sUsers.Length > 0 Then sUsers = sUsers & ","
                    sUsers = sUsers & sUserName & "," & sIDs
                End If
            Next 'DR

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: GetUserNames", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

        Return sUsers

    End Function

    Private Function NewUser() As udsUserInfo
        '****************************************************************************************
        'Description: Returns an initilized udsUserInfo so we don't get warnings.
        '
        'Parameters: None
        'Returns:    udsUserInfo
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        'TODO - Make sure all of these fields are used. If not, eliminate them.
        Dim UserInfo As udsUserInfo

        With UserInfo
            .UserID = 0
            .GroupID = 0
            .Name = String.Empty
            .Password = String.Empty
            .LastLogin = String.Empty
        End With

        Return UserInfo

    End Function

    Private Function RemoveUser(ByVal UserName As String) As String
        '********************************************************************************************
        'Description: Remove Username from the Users table and all associated Privileges from the
        '             Access_List table.
        '
        'Parameters: Username to delete
        'Returns:    Result String
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sResult As String = "success"
        Dim sUser As String = Crypto.Crypto.Encrypt(UserName)

        If UserName = "FANUC" Then
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: RemoveUser", _
                                   "Cannot delete user FANUC.")
            Return "userfanuc"
        End If

        If UserName = LoggedInUser.Name Then
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: RemoveUser", _
                                   "Cannot delete the current user [" & UserName & "].")
            Return "usercurrent"
        End If

        Try
            Dim DS As DataSet = GetDataSet()
            Dim DT As DataTable = DS.Tables("Users")
            Dim DR As DataRow
            Dim nUserID As Integer

            'Find the user in the Users table
            For Each DR In DT.Rows
                If DR.Item("Name").ToString = sUser Then
                    'Delete this user
                    nUserID = CType(DR.Item("UserID"), Integer)
                    DT.Rows.Remove(DR)
                    DT.AcceptChanges()
                    Exit For
                End If
            Next 'DR

            If nUserID = 0 Then
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: RemoveUser", _
                                       "Unable to delete user [" & UserName & "] - not found in database.")
                Return "failed"
            End If

            DT = DS.Tables("Access_List")

            'Find and delete all rows with matching UserID from the Access_List table
            For nRow As Integer = DT.Rows.Count - 1 To 0 Step -1
                DR = DT.Rows(nRow)
                If CType(DR.Item("UserID"), Integer) = nUserID Then
                    DT.Rows.Remove(DR)
                End If
            Next 'nRow

            DT.AcceptChanges()
            DS.WriteXml(XMLPath & "PWUser.xml")

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: RemoveUser", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            sResult = "failed"
        End Try

        Return sResult

    End Function

    Friend Sub ResetAutoLogOut()
        '********************************************************************************************
        'Description: A user just logged in or reset the Auto LogOut warning.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        mdtLogoutTime = DateAdd(DateInterval.Minute, AutoLogoutTimeout, Now)
        mdtLogoutWarnTime = DateAdd(DateInterval.Minute, AutoLogoutWarning, Now)
        mbWarningShown = False

        tmrWatchdog.Enabled = AutoLogoutEnable And Not IsUserFANUC

    End Sub

    Private Sub subAddPrivilege(ByVal ProcName As String, ByVal Privilege As String)
        '****************************************************************************************
        'Description: Add the specified Privilege for the Paintworks application or all users 
        '             with Allowed set to false.
        '
        'Parameters: ProcName, Privilege
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Try
            Dim DS As DataSet = GetDataSet()
            Dim DT As DataTable = DS.Tables("Privileges")
            Dim DR As DataRow
            Dim nPrivID As Integer = GetUniqueID(DT, "PrivilegeID")

            'add the new Privilege to the privileges table
            DR = DT.NewRow
            DR.Item("PrivilegeID") = nPrivID
            DR.Item("ProcessName") = ProcName
            DR.Item("Privilege") = Privilege
            DT.Rows.Add(DR)
            DT.AcceptChanges()

            'add it for each GroupID
            DT = DS.Tables("Groups")
            If DT.Rows.Count > 0 Then
                Dim nIDs() As Integer = GetIDs(DT, "GroupID")

                DT = DS.Tables("Group_Access_List")
                For Each nID As Integer In nIDs
                    DR = DT.NewRow
                    DR.Item("GroupID") = nID
                    DR.Item("PrivilegeID") = nPrivID
                    DR.Item("Allowed") = False
                    DT.Rows.Add(DR)
                    DT.AcceptChanges()
                Next
            End If

            'add it for each UserID
            DT = DS.Tables("Users")
            If DT.Rows.Count > 0 Then
                Dim nIDs() As Integer = GetIDs(DT, "UserID")

                DT = DS.Tables("Access_List")
                For Each nID As Integer In nIDs
                    DR = DT.NewRow
                    DR.Item("UserID") = nID
                    DR.Item("PrivilegeID") = nPrivID
                    DR.Item("Allowed") = False
                    DT.Rows.Add(DR)
                    DT.AcceptChanges()
                Next
            End If
            'Write the dataset
            DS.WriteXml(XMLPath & "PWUser.xml")

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subAddPrivilege", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subCheckPrivilege(ByVal ProcName As String, ByVal Privilege As String)
        '****************************************************************************************
        'Description: Verify that the specified Privilege exists for the Paintworks application.
        '             If not, add it for all users with Allowed set to false.
        '
        'Parameters: ProcName - Paintworks app to check
        '            Privilege - Privilege to verify
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Try
            Dim DS As DataSet = GetDataSet()
            Dim DRs() As DataRow = DS.Tables("Available_Privileges").Select("Privilege = '" & Privilege.ToLower & "'")

            If DRs.Count > 0 Then
                Dim sFilter As String = "ProcessName = '" & ProcName.ToLower & "' AND Privilege = '" & Privilege.ToLower & "'"
                Dim PRs() As DataRow = DS.Tables("Privileges").Select(sFilter)

                If PRs.Count > 0 Then
                    Exit Sub
                Else
                    'add Privilege to Privilege Set for ProcessName
                    Call subAddPrivilege(ProcName.ToLower, Privilege.ToLower)
                End If
            Else
                'add Privilege to Available_Privileges. The associated ePrivilege will be ePrivilege.Special
                Dim DR As DataRow = DS.Tables("Available_Privileges").NewRow

                DR.Item("Privilege") = Privilege.ToLower
                DS.AcceptChanges()
                DS.WriteXml(XMLPath & "PWUser.xml")

                'add Privilege to Privilege Set for ProcessName
                Call subAddPrivilege(ProcName.ToLower, Privilege.ToLower)

                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subCheckPrivilege", _
                                       "Privilege [" & Privilege & "] added to Available_Privileges.")
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subCheckPrivilege", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subCheckUserFANUC()
        '****************************************************************************************
        'Description: If the user logged in with the magic password, then the user is FANUC.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        mbIsUserFANUC = (msPassword = "FRNA" & Now.ToString("HHmm"))

    End Sub

    Private Sub subDoControlAction(ByVal DR As DataRow)
        '****************************************************************************************
        'Description: Executes application control requests from PW4_Main.
        '
        'Parameters: DR - contains the command and related parameters
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim sAction As String = DR.Item("Action").ToString

        Select Case sAction.ToLower
            Case "close"
                Application.Exit()
                Me.Close()
            Case "culturestring"
                Culture = DR.Item("Parameter").ToString
            Case Else
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoControlAction", _
                                       "Unrecognized command [" & sAction & "] received.")
        End Select

    End Sub

    Private Sub subDoPasswordAction(ByVal DR As DataRow)
        '****************************************************************************************
        'Description: Executes password related commands from Paintworks applications.
        '
        'Parameters: DR - contains the command and related parameters
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/15/12  HGB     Added username for Changepassword to allow editing an existing user
        '*****************************************************************************************
        Dim sAction As String = DR.Item("Action").ToString
        Dim sProcName As String = DR.Item("ProcName").ToString
        Dim sScreen() As String = Strings.Split(sProcName, "(") 'Strip off the ProcessID

        Select Case sAction.ToLower
            Case "initialize"
                Call subInitPWApp(sProcName)
            Case "displaylogin"
                msLoginProcName = sProcName
                frmLogIn.Show()
            Case "hidelogin"
                frmLogIn.Close()
            Case "logout"
                msLoginProcName = sProcName
                Call subLogout()
            Case "privilege"
                'check if this privilege needs to be registered for ProcName
                If DR.Item("Privilege").ToString <> String.Empty Then
                    Call subVerifyScreen(sScreen(0))
                    Call subCheckPrivilege(sScreen(0), DR.Item("Privilege").ToString)
                End If
                'send LoggedInUser's privilege set for ProcName to ProcName
                Dim sPrivileges As String = GetPrivileges(sScreen(0))
                Dim sMessage() As String = {sScreen(0), "privileges", sPrivileges}

                moIPC.WritePasswordMsg(sProcName, sMessage)
            Case "getlaunchpriv"
                Dim sPriv As String = DR.Item("Privilege").ToString
                Dim sProc As String = DR.Item("Parameter").ToString
                'check if this privilege needs to be registered for 3rd party app name
                Call subVerifyScreen(sProc)
                Call subCheckPrivilege(sProc, sPriv)
                'check if logged in user has Privilege for 3rd party app (Parameter)
                If Not GetLaunchPrivilege(sProc, sPriv) Then
                    sPriv = "None"
                End If
                'send LoggedInUser's privilege for sProc to sProcName
                Dim sMessage() As String = {sProcName, "launchpriv", sPriv, "", "", "", "", sProc}

                moIPC.WritePasswordMsg(sProcName, sMessage)
            Case "changepwd"
                If sScreen(0) = gs_COM_ID_PWUSERMAINT Then
                    Dim sStatus As String = ChangePassword(DR.Item("Password").ToString, DR.Item("Parameter").ToString, DR.Item("UserName").ToString)
                    Dim sMessage() As String = {sScreen(0), "changepwd", "", "", "", "", "", sStatus}

                    'Send password change result to PWUserMaint application
                    moIPC.WritePasswordMsg(sProcName, sMessage)
                Else
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoPasswordAction", _
                                           "changepwd command received from screen [" & sProcName & "] - ignored.")
                End If
            Case "adduser"
                If sScreen(0) = gs_COM_ID_PWUSERMAINT Then
                    Dim sStatus As String = AddUser(DR.Item("UserName").ToString, DR.Item("Password").ToString, CType(DR.Item("Group"), Integer))
                    Dim sMessage() As String = {sScreen(0), "adduser", "", "", "", "", "", sStatus}

                    'Send password change result to PWUserMaint application
                    moIPC.WritePasswordMsg(sProcName, sMessage)
                Else
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoPasswordAction", _
                                           "adduser [" & DR.Item("UserName").ToString & "] command received from screen [" & sProcName & "] - ignored.")
                End If
            Case "deleteuser"
                If sScreen(0) = gs_COM_ID_PWUSERMAINT Then
                    Dim sStatus As String = RemoveUser(DR.Item("UserName").ToString)
                    Dim sMessage() As String = {sScreen(0), "deleteuser", "", "", "", "", "", sStatus}

                    'Send password change result to PWUserMaint application
                    moIPC.WritePasswordMsg(sProcName, sMessage)
                Else
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoPasswordAction", _
                                           "deleteuser command received from screen [" & sProcName & "] - ignored.")
                End If
            Case "getscreennames"
                If sScreen(0) = gs_COM_ID_REPORTS Then
                    Dim sScreens As String = GetScreenNames()
                    Dim sMessage() As String = {sScreen(0), "screenames", "", "", "", "", "", sScreens}

                    moIPC.WritePasswordMsg(sProcName, sMessage)
                Else
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoPasswordAction", _
                                           "getscreennames command received from screen [" & sProcName & "] - ignored.")
                End If
            Case "getusernames"
                If (sScreen(0) = gs_COM_ID_PWUSERMAINT) Or (sScreen(0) = gs_COM_ID_REPORTS) Then
                    Dim sUsers As String = GetUserNames()
                    Dim sMessage() As String = {sScreen(0), "usernames", "", "", sUsers}

                    moIPC.WritePasswordMsg(sProcName, sMessage)
                Else
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoPasswordAction", _
                                           "getusernames command received from screen [" & sProcName & "] - ignored.")
                End If
            Case "setupchange"
                If sScreen(0) = gs_COM_ID_PWUSERMAINT Then
                    Call subInitAutoLogout()
                Else
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoPasswordAction", _
                                           "setupchange command received from screen [" & sProcName & "] - ignored.")
                End If
            Case Else
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoPasswordAction", _
                                       "Unrecognized command [" & sAction & "] received from screen [" & sProcName & "].")
        End Select

        If Not (LoggedInUser.Name = String.Empty) Then Call ResetAutoLogOut()

    End Sub

    Private Sub subInitAutoLogout()
        '****************************************************************************************
        'Description: Get AutoLogout settings from the "Setup" table.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Try

            Dim DS As DataSet = GetDataSet()
            Dim DT As DataTable = DS.Tables("Setup")

            If DT.Rows.Count > 0 Then
                Dim DR As DataRow = DT.Rows(0)
                Dim bDataOK As Boolean

                With DR
                    mnAutoLogoutTimeout = CType(.Item("AutoLogoutTime"), Integer)
                    mnAutoLogoutWarning = CType(.Item("AutoLogoutWarningTime"), Integer)
                    mbAutoLogoutEnable = CType(.Item("AutoLogout"), Boolean)
                End With

                'make sure the settings are in order
                bDataOK = mnAutoLogoutTimeout > 0
                If bDataOK Then bDataOK = mnAutoLogoutWarning > 0
                If bDataOK Then bDataOK = mnAutoLogoutWarning < mnAutoLogoutTimeout

                If Not bDataOK Then
                    mbAutoLogoutEnable = False
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitAutoLogout", _
                                           "Autologout setup data - settings error. Autologout disabled.")
                End If
            Else
                mbAutoLogoutEnable = False
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitAutoLogout", _
                                       "Autologout setup data not found in database. Autologout disabled.")
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitAutoLogout", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subInitializeForm()
        '********************************************************************************************
        'Description: Called on form load
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bSuccess As Boolean = True

        Try
            Call subProcessCommandLine()

            'Set custom crypto passphrase
            Crypto.Crypto.SetPassphrase("Ql3Pz7&xKm07ylcDSfW5")
            'get path to control and data files
            mPWCommon.GetDefaultFilePath(msIpcPath, eDir.IPC, String.Empty, String.Empty)
            mPWCommon.GetDefaultFilePath(msXmlPath, eDir.XML, String.Empty, String.Empty)
            Me.Icon = CType(gpsRM.GetObject("FormIcon"), Icon)
            'set autologout properties
            Call subInitAutoLogout()

            'initialize LoggedInUser
            LoggedInUser = NewUser()

            moIPC = New Paintworks_IPC.clsInterProcessComm("password*.xml", IPCPath, XMLPath)

        Catch ex As Exception
            Dim sErrMsg As String = ex.Message
            Dim sMessage(1) As String

            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitializeForm", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

            bSuccess = False
            sMessage(0) = "poststatusmsg"
            sMessage(1) = sErrMsg
            moIPC.WriteControlMsg(gs_COM_ID_PW_MAIN, sMessage)

        Finally
            Dim sMessage(1) As String

            sMessage(0) = "passwordload"
            sMessage(1) = bSuccess.ToString
            moIPC.WriteControlMsg(gs_COM_ID_PW_MAIN, sMessage)
        End Try

    End Sub

    Private Sub subInitPWApp(ByVal ProcessName As String)
        '****************************************************************************************
        'Description: Tell <ProcessName> who's logged in, when they logged in last, and what they
        '             are allowed to do on this screen.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim sScreen() As String = Strings.Split(ProcessName, "(")

        Call subVerifyScreen(sScreen(0))

        If LoggedInUser.Name = String.Empty Then
            Dim sMessage() As String = {sScreen(0), "initialized"}

            moIPC.WritePasswordMsg(ProcessName, sMessage)
        Else

            Try
                Dim sPrivileges As String = GetPrivileges(sScreen(0))
                Dim User As udsUserInfo = LoggedInUser
                Dim sMessage() As String = {sScreen(0), "initialized", sPrivileges, _
                                            String.Empty, User.Name, User.LastLogin}

                moIPC.WritePasswordMsg(ProcessName, sMessage)

            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitPWApp", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try

        End If

    End Sub

    Private Sub subLogout()
        '********************************************************************************************
        'Description: The current user is Logged Out.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sMessage() As String = {gs_COM_ID_PASSWORD, "logout"}

        mbIsUserFANUC = False
        tmrWatchdog.Enabled = False
        LoggedInUser = NewUser()

        If moIPC.GetProcess(gs_COM_ID_PW_MAIN) Is Nothing Then
            'PW4_Main not running. Send "logout" message to Procces that user logger out from
            moIPC.WritePasswordMsg(msLoginProcName, sMessage)
        Else
            'Send "logout" message to PW4_Main who will broadcast it to any running screens.
            moIPC.WritePasswordMsg(gs_COM_ID_PW_MAIN, sMessage)
        End If

    End Sub

    Private Sub subProcessCommandLine()
        '********************************************************************************************
        'Description: If there are command line parameters - process here
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sArg As String = "/culture="

        For Each s As String In My.Application.CommandLineArgs
            'TODO - Test this
            If s.ToLower.StartsWith(sArg) Then
                Culture = s.Remove(0, sArg.Length)
            End If
        Next

    End Sub

    Private Sub subVerifyScreen(ByVal ProcessName As String)
        '********************************************************************************************
        'Description: Check if ProcessName exists in "Screens" table. If not, add it.
        '
        'Parameters: DS - PWUser Dataset, ProcessName - Paintworks screen name
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim bExists As Boolean
            Dim DS As DataSet = GetDataSet()
            Dim DT As DataTable = DS.Tables("Processes")

            If DT.Rows.Count > 0 Then
                For Each DR As DataRow In DT.Rows
                    Dim sScreen As String = DR.Item("ProcessName").ToString

                    If sScreen.ToLower = ProcessName.ToLower Then
                        bExists = True
                        Exit For
                    End If
                Next 'DR
            End If

            If Not bExists Then
                Dim DR As DataRow = DT.NewRow

                'Add ProcessName to list of screens
                DR.Item("ProcessName") = ProcessName
                DT.Rows.Add(DR)
                DT.AcceptChanges()
                DS.WriteXml(XMLPath & "PWUser.xml")
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subVerifyScreen", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

#End Region

#Region " Events "

    Private Sub frmMain_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.GotFocus
        Try
            frmLogIn.BringToFront()
			'11/21/13 MSW more attempts to get focus on the login prompt            
            Dim oProc As Process = Process.GetCurrentProcess
            ShowWindowAsync(frmLogIn.Handle, 5) 'SW_SHOW
            Threading.Thread.Sleep(10)
            'ShowWindowAsync(hWnd, SW_RESTORE);
            ShowWindowAsync(frmLogIn.Handle, 9) 'SW_RESTORE
            Threading.Thread.Sleep(10)
            SetForegroundWindow(frmLogIn.Handle)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description: Position this form off of the screen, then perform other inititalization. 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim dpOffScreen As System.Drawing.Point

        dpOffScreen.X = Me.Width * -1
        dpOffScreen.Y = Me.Top * -1
        Me.Location = dpOffScreen

        Call subInitializeForm()

    End Sub

    Private Sub moIPC_NewMessage(ByVal Schema As String, ByVal DS As System.Data.DataSet) Handles moIPC.NewMessage
        '********************************************************************************************
        'Description: A new message has been received from another Paintworks application. 
        '
        'Parameters: DS - PWUser Dataset, ProcessName - Paintworks screen name
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If Me.InvokeRequired Then
            Dim dNewMessage As New NewMessage_CallBack(AddressOf moIPC_NewMessage)
            Me.BeginInvoke(dNewMessage, New Object() {Schema, DS})
        Else
            Select Case Schema.ToLower
                Case moIPC.CONTROL_MSG_SCHEMA.ToLower
                    Call subDoControlAction(DS.Tables("Command").Rows(0))

                Case moIPC.PASSWORD_MSG_SCHEMA.ToLower
                    Call subDoPasswordAction(DS.Tables("Command").Rows(0))

                Case Else
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: moIPC_NewMessage", _
                                           "Unrecognized schema [" & Schema & "].")
            End Select
        End If 'Me.InvokeRequired

    End Sub

    Public Sub New()
        '********************************************************************************************
        'Description: Initialize language support.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'for language support
        mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, msBASE_ASSEMBLY_LOCAL, _
                                            msROBOT_ASSEMBLY_LOCAL)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub tmrWatchDog_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrWatchdog.Tick
        '********************************************************************************************
        'Description: Keep track of how long User has been logged in.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If Now >= mdtLogoutTime Then
            If mbWarningShown Then
                frmWarning.Close()
                mbWarningShown = False
                Call subLogout()
            End If
        ElseIf Now >= mdtLogoutWarnTime Then
            If Not mbWarningShown Then
                frmWarning.Show()
                mbWarningShown = True
            End If

            If Not frmWarning Is Nothing Then
                frmWarning.Clock = DateDiff(DateInterval.Second, Now, mdtLogoutTime)
            End If
        End If

    End Sub

#End Region

#Region " Debug "

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        '********************************************************************************************
        'Description: Debug stuff
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        '********************************************************************************************
        'Description: Debug stuff
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

    End Sub

#End Region

End Class