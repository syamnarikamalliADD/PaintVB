
' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2009
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: frmMain
'
' Description: User Password Maintenance Screen
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
'    Date       By      Reason                                                        Version
'    03/07/12   RJO     Initial code                                                  4.1.2.0
'    03/29/12   RJO     Added Process/Privilege maintenance on Tab 4 for user FANUC   4.1.2.1
'    04/20/12   MSW     frmMain_KeyDown-Add excape key handler                        4.01.03.00
'    09/05/12   RJO     Bug fix to SaveGroupPrivileges.                               4.01.03.01
'    11/15/12   HGB     Updates to btnEditPwdAccept_Click that allow another user's   4.01.03.02
'                       password to be changed properly. The username of the user to 
'                       change was not being passed, this resulted in the current 
'                       user’s password being changed.
'    12/13/12   MSW     trvPrivileges_DoubleClick-Add another  not nothing check      4.01.04.00
'    04/16/13   MSW     Add Canadian language files                                   4.01.05.00
'                       Standalone Changelog
'    07/09/13   MSW     Update and standardize logos                                  4.01.05.01
'    08/20/13   MSW     Clean up screen names for display                             4.01.05.02
'                       Progress, Status - Add error handler so it doesn't get hung up
'    09/30/13   MSW     Save screenshots as jpegs                                     4.01.06.00
'    01/07/14   MSW     Remove ControlBox from main form                              4.01.06.01
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call              4.01.07.00
'    08/01/14   RJO/BD  Fixed problems with No node selected or Parent node selected. 4.01.07.01
'                       Also, fixed problems with Displayed Name <> Process Name.
'**************************************************************************************************

Option Compare Binary
Option Explicit On
Option Strict On

Imports Response = System.Windows.Forms.DialogResult

Friend Class frmMain

#Region " Declares "
    '******** Form Constants   *********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Public Const msSCREEN_NAME As String = "PWUserMaint"   ' <-- For password area change log etc.
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    Friend Const mbUSEROBOTS As Boolean = False
    Private Const mnNEW_MEMBER_ID As Integer = -1
    Private Const mnTIMEOUT_MAX As Integer = 1440 '1 day in minutes
    Private Const mnTIMEOUT_MIN As Integer = 2
    Private Const mnWARNING_MAX As Integer = 1439
    Private Const mnWARNING_MIN As Integer = 1
    '******** End Form Constants ********************************************************************

    '******** Structures ****************************************************************************
    Private Structure udsGroup
        Public Name As String
        Public ID As Integer
    End Structure

    Private Structure udsPrivilege
        Public ProcessName As String
        Public PrivilegeName As String
        Public PrivilegeID As Integer
        Public Allowed As Boolean
        Public Allowed_Org As Boolean
    End Structure

    Private Structure udsSetup
        Public AutoLogoutEnable As Boolean
        Public AutoLogoutEnable_Org As Boolean
        Public AutoLogoutTime As Integer
        Public AutoLogoutTime_Org As Integer
        Public AutoLogoutWarn As Integer
        Public AutoLogoutWarn_Org As Integer
    End Structure

    Private Structure udsUser
        Public Name As String
        Public ID As Integer
        Public GroupID As Integer
    End Structure
    '******** End Structures ************************************************************************

    '******** Form Variables ************************************************************************
    Friend gsChangeLogArea As String = msSCREEN_NAME
    Private msCulture As String = "en-US" 'Default to English
    Private msOldZone As String = String.Empty
    Private mcolZones As clsZones = Nothing
    Private mbLocal As Boolean
    Private mbScreenLoaded As Boolean
    Private mbEventBlocker As Boolean
    Private mbPrintEnable As Boolean
    Private mbRemoteZone As Boolean
    Private mColorOff As Color = Color.Red
    Private mColorOn As Color = Color.ForestGreen
    Private mEditGroup As udsGroup
    Private mEditSetup As udsSetup
    Private mEditUser As udsUser
    Private mGroups() As udsGroup
    Private mPrivileges() As udsPrivilege
    Private msCurrentTabName As String = "TabPage1" ' always start on first page
    Private msSCREEN_DUMP_NAME As String = String.Empty 'changes with selected tab
    Private mPrintHtml As clsPrintHtml
    Private mUsers() As udsUser
    '******** End Form Variables ********************************************************************

    '******** Property Variables ********************************************************************
    Private mbEditsMade As Boolean
    Private msUserName As String = String.Empty
    Private msXMLPath As String
    Private mbDataLoaded As Boolean
    Private mnProgress As Integer
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    '******** End Property Variables ****************************************************************

    '******** New Password.NET interface class ******************************************************
    Friend WithEvents moPassword As clsPWUser
    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    '******** End Password.NET interface ************************************************************

    '******* New program-to-program communication object ********************************************
    Friend WithEvents oIPC As New Paintworks_IPC.clsInterProcessComm
    ' The delegates (function pointers) enable asynchronous calls from the IPC object.
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet)
    '******* End program-to-program communication object ********************************************

#End Region

#Region " Properties "

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
                                                                String.Empty)

        End Set

    End Property

    Friend Property DataLoaded() As Boolean
        '********************************************************************************************
        'Description:  Data loaded flag for form
        '
        'Parameters: Set to true when done loading data
        'Returns:    True if data is loaded
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mbDataLoaded
        End Get

        Set(ByVal Value As Boolean)
            mbDataLoaded = Value
            If mbDataLoaded Then
                'just finished loading reset & refresh
                EditsMade = False
            End If
            'if not loaded disable all controls
            subEnableControls(Value)
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

    Friend Property EditsMade() As Boolean
        '********************************************************************************************
        'Description:  Edit flag for form
        '
        'Parameters: set when somebody changed something
        'Returns:    True if active edits
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mbEditsMade
        End Get

        Set(ByVal Value As Boolean)
            Dim bOld As Boolean = mbEditsMade
            mbEditsMade = Value
            If mbEditsMade <> bOld Then subEnableControls(True)
        End Set

    End Property

    Public Property LoggedOnUser() As String
        '********************************************************************************************
        'Description:  Who's Logged on
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msUserName
        End Get

        Set(ByVal Value As String)
            msUserName = Value
        End Set

    End Property

    Friend Property Privilege() As ePrivilege
        '********************************************************************************************
        'Description:  What can a user do - for now take privilege from password object and massage 
        '               it to work - need this property for future 
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mPrivilegeGranted
        End Get

        Set(ByVal Value As ePrivilege)

            'this is needed with old password object raising events
            'and can hopefully be removed when password is redone
           'Control.CheckForIllegalCrossThreadCalls = False

            mPrivilegeRequested = Value

            'handle logout
            If mPrivilegeRequested = ePrivilege.None Then
                mPrivilegeGranted = ePrivilege.None
                subEnableControls(True)  ' This is confusing True but really false sub will look at privilege
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
                'this is needed with old password object raising events
                'and can hopefully be removed when password is redone
               'Control.CheckForIllegalCrossThreadCalls = True
                Exit Property
            End If

            'prevent recursion
            If mPrivilegeGranted = mPrivilegeRequested Then
                'this is needed with old password object raising events
                'and can hopefully be removed when password is redone
               'Control.CheckForIllegalCrossThreadCalls = True
                Exit Property
            End If


            If moPassword.CheckPassword(Value) Then
                'passed
                mPrivilegeGranted = mPrivilegeRequested
                'if privilege changed may have to enable controls
                subEnableControls(True)
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
            Else
                'denied
                If moPassword.UserName <> String.Empty Then _
                        mPrivilegeRequested = mPrivilegeGranted
                subEnableControls(True)
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
            End If

            'this is needed with old password object raising events
            'and can hopefully be removed when password is redone
           'Control.CheckForIllegalCrossThreadCalls = True

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
                    lblSpacer.Width = gtSSSize.SpaceLabelInvisSize
                    tspProgress.Visible = True
                Else
                    lblSpacer.Width = gtSSSize.SpaceLabelVisbleSize
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
        'Description:  write status messages to listbox and statusbar
        '
        'Parameters: If StatusbarOnly is true, doesn't write to listbox
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
		' 08/20/13  MSW     Add error handler so it doesn't get hung up during exit
        '********************************************************************************************
        Get
            Return stsStatus.Text
        End Get
        Set(ByVal Value As String)
            Try
                If StatusStripOnly = False Then
                    mPWCommon.AddToStatusBox(lstStatus, Value)
                End If
                stsStatus.Items("lblStatus").Text = Strings.Replace(Value, vbTab, "  ")
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Property: Status", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
    End Property


    Private ReadOnly Property XMLPath() As String
        '********************************************************************************************
        'Description: The full path to the folder that will contain the IPC files
        '
        'Parameters: None
        'Returns:    The path
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            If msXMLPath = String.Empty Then
                'Set to default
                mPWCommon.GetDefaultFilePath(msXMLPath, eDir.XML, String.Empty, String.Empty)
            End If
            Return msXMLPath
        End Get

    End Property

#End Region

#Region " Routines "

    Private Function bAskForSave() As Boolean
        '********************************************************************************************
        'Description:  Check to see if we need to save when screen is closing 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim oCulture As System.Globalization.CultureInfo = DisplayCulture

        Dim lRet As Response = MessageBox.Show(gcsRM.GetString("csSAVEMSG1", oCulture) & vbCrLf & _
                                               gcsRM.GetString("csSAVEMSG", oCulture), _
                                               gcsRM.GetString("csSAVE_CHANGES", oCulture), _
                                               MessageBoxButtons.YesNoCancel, _
                                               MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

        Select Case lRet
            Case Response.Yes 'Response.Yes
                Call subSaveData()
                Return True
            Case Response.Cancel
                'false aborts closing form , changing zone etc
                Status = gcsRM.GetString("csSAVE_CANCEL", oCulture)
                Return False
            Case Else
                EditsMade = False
                Return True
        End Select

    End Function

    Private Function bCheckEditsMade() As Boolean
        '********************************************************************************************
        'Description:  Verify that at least one item was changed on the current tab. Return False if
        '              not.
        '
        'Parameters:  none
        'Returns:     True if EditsMade is valid.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case msCurrentTabName
            Case "TabPage2", "TabPage3"
                If Not mPrivileges Is Nothing Then
                    For Each oPrivilege As udsPrivilege In mPrivileges
                        If oPrivilege.Allowed <> oPrivilege.Allowed_Org Then Return True
                    Next 'oPrivilege
                End If
            Case "TabPage4"
                With mEditSetup
                    If .AutoLogoutEnable <> .AutoLogoutEnable_Org Then Return True
                    If .AutoLogoutTime <> .AutoLogoutTime_Org Then Return True
                    If .AutoLogoutWarn <> .AutoLogoutWarn_Org Then Return True
                End With
        End Select

        'If it makes it to here...
        Return False

    End Function

    Private Function bPrintdoc(ByVal bPrint As Boolean) As Boolean
        '********************************************************************************************
        'Description:  Data Print Routine
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Me.Cursor = System.Windows.Forms.Cursors.AppStarting

        Try
            btnPrint.Enabled = False
            Status = gcsRM.GetString("csPRINTING", DisplayCulture)

            Progress = 5

            mPrintHtml.subSetPageFormat()
            mPrintHtml.subClearTableFormat()
            mPrintHtml.subSetColumnCfg("align=right", 0)
            mPrintHtml.subSetRowcfg("Bold=on", 0, 0)

            mPrintHtml.HeaderRowsPerTable = 1

            Dim oTab As TabPage = tabMain.SelectedTab
            Dim sPageTitle As String = gpsRM.GetString("psSCREENCAPTION") & " - " & oTab.Text
            Dim sTitle(1) As String
            Dim sStatus As String = String.Empty

            sTitle(0) = oTab.Text
            sTitle(1) = String.Empty

            Dim sSubTitle(0) As String
            sSubTitle(0) = mcolZones.SiteName & " - " & mcolZones.ActiveZone.Name

            Progress = 10
            mPrintHtml.subStartDoc(Status, sPageTitle)

            Select Case oTab.Name
                Case "TabPage2"
                    sTitle(1) = gpsRM.GetString("psUSER_TXT", DisplayCulture) & mEditUser.Name
                    Call subPrintPrivileges(sTitle, sSubTitle)
                Case "TabPage3"
                    sTitle(1) = gpsRM.GetString("psGROUP_TXT", DisplayCulture) & mEditGroup.Name
                    Call subPrintPrivileges(sTitle, sSubTitle)
                Case "TabPage4"
                    Call subPrintSetup(sTitle, sSubTitle)
            End Select

            Progress = 80

            Status = gcsRM.GetString("csPRINT_SENDING", DisplayCulture)
            mPrintHtml.subCloseFile(Status)

            If bPrint Then
                mPrintHtml.subPrintDoc()
            End If

            Progress = 0

            Status = gcsRM.GetString("csREADY", DisplayCulture)
            Me.Cursor = System.Windows.Forms.Cursors.Default
            btnPrint.Enabled = True
            Return (True)

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                               Status, MessageBoxButtons.OK)

            Progress = 0
            Status = gcsRM.GetString("csREADY", DisplayCulture)
            Me.Cursor = System.Windows.Forms.Cursors.Default
            btnPrint.Enabled = True
            Return False
        End Try

    End Function

    Private Function bSaveToGUI() As Boolean
        '********************************************************************************************
        'Description:  Save data stored on GUI in database, xml file etc.
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bSuccess As Boolean

        Try
            Select Case msCurrentTabName
                Case "TabPage2"
                    'Save user privilege changes to database.
                    bSuccess = SaveUserPrivileges(True)
                Case "TabPage3"
                    'Save group privilege changes to database.
                    bSuccess = SaveGroupPrivileges(mEditGroup.ID)
                Case "TabPage4"
                    'Save group privilege changes to database.
                    If ValidateSetup() Then
                        bSuccess = SaveSetup()
                    Else
                        bSuccess = False
                    End If
                Case Else
                    'Shouldn't be here
                    bSuccess = False
            End Select

        Catch ex As Exception
            bSuccess = False
            mDebug.ShowErrorMessagebox(gpsRM.GetString("psDATA_SAVE_FAIL_MSG", DisplayCulture), _
                                       ex, msSCREEN_NAME, Status, MessageBoxButtons.OK)
        End Try

        Return bSuccess

    End Function

    Private Function CancelNewGroup() As Boolean
        '********************************************************************************************
        'Description:  Check to see if a new Group has been added. If so delete all evidence and 
        '              return True.
        '
        'Parameters: none
        'Returns:    True if New Group was cancelled
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nLastIndex As Integer = mGroups.GetUpperBound(0)

        If mGroups(nLastIndex).ID = mnNEW_MEMBER_ID Then
            ReDim Preserve mGroups(nLastIndex - 1)

            Return True
        Else
            Return False
        End If

    End Function

    Private Function CancelNewUser() As Boolean
        '********************************************************************************************
        'Description:  Check to see if a new User has been added. If so delete all evidence and 
        '              return True.
        '
        'Parameters: none
        'Returns:    True if New User was cancelled
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nLastIndex As Integer = mUsers.GetUpperBound(0)

        If mUsers(nLastIndex).ID = mnNEW_MEMBER_ID Then
            ReDim Preserve mUsers(nLastIndex - 1)

            Return True
        Else
            Return False
        End If

    End Function

    Private Function EditUserIndex() As Integer
        '****************************************************************************************
        'Description: Returns the index of the element in mUsers that corresponds to mEditUser.
        '
        'Parameters: none
        'Returns:    EditUser Index
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        For nIndex As Integer = 0 To mUsers.GetUpperBound(0)
            If mUsers(nIndex).ID = mEditUser.ID Then Return nIndex
        Next

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

    Private Function GetGroupName(ByVal GroupID As Integer) As String
        '****************************************************************************************
        'Description: Returns the Group Name corresponding to the supplied GroupID.
        '
        'Parameters: GroupID - number
        'Returns:    Group Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim sName As String = gpsRM.GetString("psCUSTOM_TXT")

        For Each oGroup As udsGroup In mGroups
            If oGroup.ID = GroupID Then
                sName = oGroup.Name
                Exit For
            End If
        Next 'oGroup

        Return sName

    End Function

    Private Function GetUniqueGroupID() As Integer
        '****************************************************************************************
        'Description: Returns the next available unassigned GroupID
        '
        'Parameters: None
        'Returns:    Next available ID
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim nID As Integer = 1

        If Not (mGroups Is Nothing) Then
            Dim bFound As Boolean = True

            Do 'search the array for the first available unassigned ID
                bFound = False
                For Each oGroup As udsGroup In mGroups
                    If nID = oGroup.ID Then
                        nID += 1
                        bFound = True
                        Exit For
                    End If
                Next 'nItem
            Loop Until bFound = False
        End If

        Return nID

    End Function

    Private Function SaveGroupPrivileges(ByVal GroupID As Integer) As Boolean
        '********************************************************************************************
        'Description:  Write Privilege "Allowed" values to the database for GroupID.
        '
        'Parameters: Group ID - NUmber
        'Returns:    True if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/05/12  RJO     Bug fix. Using wrong GroupID for GDRs Select and ChangeLog.
        '********************************************************************************************
        Dim sAllow As String = gpsRM.GetString("psBTN_ALLOW_TXT")
        Dim sGroup As String = gpsRM.GetString("psGROUP_TXT")
        Dim sNotAllow As String = gpsRM.GetString("psBTN_NOT_ALLOW_TXT")
        Dim sPrivilege As String = gpsRM.GetString("psPRIVILEGE_TXT")
        Dim sScreen As String = gpsRM.GetString("psSCREEN_TXT")

        Try
            Dim DS As DataSet = GetDataSet()
            Dim DR As DataRow
            Dim GDRs() As DataRow = DS.Tables("Group_Access_List").Select("GroupID = " & GroupID.ToString, "PrivilegeID ASC")

            'update database
            For Each DR In GDRs
                Dim nPrivilegeID As Integer = CType(DR.Item("PrivilegeID"), Integer)

                For nIndex As Integer = 0 To mPrivileges.GetUpperBound(0)
                    If mPrivileges(nIndex).PrivilegeID = nPrivilegeID Then
                        DR.Item("Allowed") = mPrivileges(nIndex).Allowed
                        Exit For
                    End If
                Next 'nIndex
            Next 'DR
            DS.AcceptChanges()
            DS.WriteXml(msXMLPath & "PWUser.xml")

            'update change log
            For Each oPrivilege As udsPrivilege In mPrivileges
                'Add changes to Changelog list.
                If oPrivilege.Allowed <> oPrivilege.Allowed_Org Then
                    Dim sFrom As String
                    Dim sTo As String
                    Dim sWhat As String

                    If oPrivilege.Allowed Then
                        sTo = sAllow
                        sFrom = sNotAllow
                    Else
                        sTo = sNotAllow
                        sFrom = sAllow
                    End If

                    sWhat = sGroup & GetGroupName(GroupID) & _
                            sScreen & oPrivilege.ProcessName & _
                            sPrivilege & oPrivilege.PrivilegeName

                    Call subUpdateChangeLog(sTo, sFrom, String.Empty, mcolZones.ActiveZone, sWhat, String.Empty)
                End If
            Next 'oPrivilege

        Catch ex As Exception
            'Message to User, Log internal error
            mDebug.ShowErrorMessagebox(gpsRM.GetString("psDATA_SAVE_FAIL_MSG", DisplayCulture), _
                                       ex, msSCREEN_NAME, Status, MessageBoxButtons.OK)
            Return False
        End Try

        Return True

    End Function

    Private Function SaveSetup() As Boolean
        '********************************************************************************************
        'Description:  Write Setup values to the database. Log the changes. Let Password.NET know 
        '              that Setup changed.
        '
        'Parameters: None
        'Returns:    True if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bSuccess As Boolean = True
        Try
            Dim DS As DataSet = GetDataSet()
            Dim DR As DataRow = DS.Tables("Setup").Rows(0)
            Dim sMsg() As String = {moPassword.ProcessName, "setupchange"}

            With DR
                .Item("AutoLogout") = mEditSetup.AutoLogoutEnable
                .Item("AutoLogoutTime") = mEditSetup.AutoLogoutTime
                .Item("AutoLogoutWarningTime") = mEditSetup.AutoLogoutWarn
            End With

            DS.AcceptChanges()
            DS.WriteXml(msXMLPath & "PWUser.xml")

            With mEditSetup
                Dim sFrom As String
                Dim sTo As String
                Dim sWhat As String

                If .AutoLogoutEnable_Org <> .AutoLogoutEnable Then
                    If .AutoLogoutEnable Then
                        sFrom = gpsRM.GetString("psDISABLED_TXT")
                        sTo = gpsRM.GetString("psENABLED_TXT")
                    Else
                        sFrom = gpsRM.GetString("psENABLED_TXT")
                        sTo = gpsRM.GetString("psDISABLED_TXT")
                    End If
                    sWhat = gpsRM.GetString("psLBL_EN_AL_TXT")
                    .AutoLogoutEnable_Org = .AutoLogoutEnable
                    Call subUpdateChangeLog(sTo, sFrom, String.Empty, mcolZones.ActiveZone, sWhat, String.Empty)
                End If

                If .AutoLogoutTime_Org <> .AutoLogoutTime Then
                    sFrom = .AutoLogoutTime_Org.ToString
                    sTo = .AutoLogoutTime.ToString
                    sWhat = gpsRM.GetString("psLBL_AL_TIME_TXT")
                    .AutoLogoutTime_Org = .AutoLogoutTime
                    Call subUpdateChangeLog(sTo, sFrom, String.Empty, mcolZones.ActiveZone, sWhat, String.Empty)
                End If

                If .AutoLogoutWarn_Org <> .AutoLogoutWarn Then
                    sFrom = .AutoLogoutWarn_Org.ToString
                    sTo = .AutoLogoutWarn.ToString
                    sWhat = gpsRM.GetString("psLBL_AL_WARN_TXT")
                    .AutoLogoutWarn_Org = .AutoLogoutWarn
                    Call subUpdateChangeLog(sTo, sFrom, String.Empty, mcolZones.ActiveZone, sWhat, String.Empty)
                End If
            End With

            ftbAutoLogoutTime.ForeColor = Color.Black
            ftbAutoLogoutTime.BackColor = Color.White
            ftbAutoLogoutWarn.ForeColor = Color.Black
            ftbAutoLogoutWarn.BackColor = Color.White

            EditsMade = False

            oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sMsg)

        Catch ex As Exception
            'Message to user, Log error in Windows event log.
            mDebug.ShowErrorMessagebox(gpsRM.GetString("psDATA_SAVE_FAIL_MSG", DisplayCulture), _
                                       ex, msSCREEN_NAME, Status, MessageBoxButtons.OK)
            bSuccess = False
        End Try

        Return bSuccess

    End Function

    Private Function SaveUserPrivileges(ByVal CheckGroup As Boolean) As Boolean
        '********************************************************************************************
        'Description:  Write Privilege "Allowed" values to the database for mEditUser and associated
        '              Group if it was affected by the changes.
        '
        'Parameters: CheckGroup - True = Ask user if changes apply to Group also
        'Returns:    True if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bModifyGroup As Boolean = False
        Dim bSuccess As Boolean = True
        Dim sAllow As String = gpsRM.GetString("psBTN_ALLOW_TXT")
        Dim sGroup As String = gpsRM.GetString("psUSER_GROUP_TXT")
        Dim sNotAllow As String = gpsRM.GetString("psBTN_NOT_ALLOW_TXT")
        Dim sPrivilege As String = gpsRM.GetString("psPRIVILEGE_TXT")
        Dim sScreen As String = gpsRM.GetString("psSCREEN_TXT")
        Dim sUser As String = gpsRM.GetString("psUSER_TXT")

        If CheckGroup Then
            If mEditUser.GroupID > 0 Then
                'ask user if Privilege changes should affect Group.
                Dim lRet As Response = MessageBox.Show(gpsRM.GetString("psUPDATE_GROUP_MSG") & vbCrLf & _
                                                       gpsRM.GetString("psANSWERING_NO_MSG"), _
                                                       gpsRM.GetString("psGPB_USER_PRIV_TXT"), _
                                                       MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                                                       MessageBoxDefaultButton.Button2)
                If lRet = Response.Yes Then bModifyGroup = True
            End If
        End If 'CheckGroup

        Try
            'update database
            Dim DS As DataSet = GetDataSet()
            Dim DR As DataRow
            Dim UDRs() As DataRow = DS.Tables("Access_List").Select("UserID = " & mEditUser.ID.ToString, "PrivilegeID ASC")

            For Each DR In UDRs
                Dim nPrivilegeID As Integer = CType(DR.Item("PrivilegeID"), Integer)

                For nIndex As Integer = 0 To mPrivileges.GetUpperBound(0)
                    If mPrivileges(nIndex).PrivilegeID = nPrivilegeID Then
                        DR.Item("Allowed") = mPrivileges(nIndex).Allowed
                        Exit For
                    End If
                Next 'nIndex
            Next 'DR

            DS.AcceptChanges()

            'update change log
            If CheckGroup Then
                For Each oPrivilege As udsPrivilege In mPrivileges
                    'Add changes to Changelog list.
                    If oPrivilege.Allowed <> oPrivilege.Allowed_Org Then
                        Dim sFrom As String
                        Dim sTo As String
                        Dim sWhat As String

                        If oPrivilege.Allowed Then
                            sTo = sAllow
                            sFrom = sNotAllow
                        Else
                            sTo = sNotAllow
                            sFrom = sAllow
                        End If

                        sWhat = sUser & mEditUser.Name & _
                                sScreen & oPrivilege.ProcessName & _
                                sPrivilege & oPrivilege.PrivilegeName

                        Call subUpdateChangeLog(sTo, sFrom, String.Empty, mcolZones.ActiveZone, sWhat, String.Empty)
                    End If
                Next 'oPrivilege
            End If 'CheckGroup

            'modify user's GroupID if necessary
            If CheckGroup And Not bModifyGroup Then
                'change Users GroupID to 0 in the database and log change.
                For Each DR In DS.Tables("Users").Rows
                    If CType(DR.Item("UserID"), Integer) = mEditUser.ID Then
                        DR.Item("GroupID") = 0 'Custom
                        Exit For
                    End If
                Next 'DR

                DS.AcceptChanges()

                'update Change log
                Dim sOldGroup As String = GetGroupName(mEditUser.GroupID)

                mEditUser.GroupID = 0
                mUsers(EditUserIndex).GroupID = 0
                Call subUpdateChangeLog(gpsRM.GetString("psCUSTOM_TXT"), sOldGroup, String.Empty, _
                                        mcolZones.ActiveZone, sUser & mEditUser.Name & sGroup, String.Empty)
            End If

            DS.WriteXml(msXMLPath & "PWUser.xml")

            'modify Privileges assigned to user's GroupID if necessary
            If bModifyGroup Then
                bSuccess = SaveGroupPrivileges(mEditUser.GroupID)
            End If

        Catch ex As Exception
            'Message to User, Log internal error
            mDebug.ShowErrorMessagebox(gpsRM.GetString("psDATA_SAVE_FAIL_MSG", DisplayCulture), _
                                       ex, msSCREEN_NAME, Status, MessageBoxButtons.OK)
            Return False
        End Try

        Return bSuccess

    End Function

    Private Sub subChangeGroup(ByVal GroupID As Integer)
        '********************************************************************************************
        'Description: New Group selected for mEditUser - update database
        '
        'Parameters: none 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim DS As DataSet = GetDataSet()
            Dim DRs() As DataRow = DS.Tables("Users").Select("UserID = " & mEditUser.ID.ToString)

            'Set user's GroupID to new group
            DRs(0).Item("GroupID") = GroupID
            DS.AcceptChanges()
            DS.WriteXml(XMLPath & "PWUser.xml")

            If GroupID > 0 Then
                'change user's privilege set to new group privilege set
                Dim GARs() As DataRow = DS.Tables("Group_Access_List").Select("GroupID = " & GroupID.ToString, "PrivilegeID ASC")

                For Each DR As DataRow In GARs
                    Dim nPrivID As Integer = CType(DR.Item("PrivilegeID"), Integer)

                    For nItem As Integer = 0 To mPrivileges.GetUpperBound(0)
                        If mPrivileges(nItem).PrivilegeID = nPrivID Then
                            mPrivileges(nItem).Allowed = CType(DR.Item("Allowed"), Boolean)
                            Exit For
                        End If
                    Next 'nItem
                Next 'DR

                If SaveUserPrivileges(False) Then
                    'Update Treeview
                    Call subLoadUserPrivilegeTree(mEditUser.ID)
                    'Update changelog
                    Dim sWhat As String = gpsRM.GetString("psUSER_TXT") & mEditUser.Name & gpsRM.GetString("psUSER_GROUP_TXT")

                    Call subUpdateChangeLog(GetGroupName(GroupID), GetGroupName(mEditUser.GroupID), String.Empty, mcolZones.ActiveZone, sWhat, String.Empty)
                    Call subLogChanges()
                End If
            End If 'GroupID > 0

            mEditUser.GroupID = GroupID
            mUsers(EditUserIndex).GroupID = GroupID

        Catch ex As Exception
            'Message to User, Log internal error
            mDebug.ShowErrorMessagebox(gpsRM.GetString("psDATA_SAVE_FAIL_MSG", DisplayCulture), _
                                       ex, msSCREEN_NAME, Status, MessageBoxButtons.OK)
        End Try

    End Sub

    Private Sub subChangeZone()
        '********************************************************************************************
        'Description: New Zone Selected - check for save then load new info
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Progress = 10

        If EditsMade Then
            ' false means user pressed cancel
            If bAskForSave() = False Then
                cboZone.Text = msOldZone
                Progress = 100
                Exit Sub
            End If
        End If  'EditsMade

        'just in case
        If cboZone.Text = String.Empty Then
            Exit Sub
        Else

            tabMain.Enabled = True

            If cboZone.Text = msOldZone Then Exit Sub
        End If
        msOldZone = cboZone.Text
        mcolZones.CurrentZone = cboZone.Text

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subEnableControls(False)
            EditsMade = False
            DataLoaded = False

            tabMain.TabPages.Item(0).Select()

            If mbScreenLoaded = True Then
                Call subLoadData()
            End If

        Catch ex As Exception

            mDebug.ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), ex, msSCREEN_NAME, _
                                       Status, MessageBoxButtons.OK)
        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try

    End Sub

    Private Sub subDeletePrivilege(ByVal ProcessName As String, ByVal Privilege As String)
        '********************************************************************************************
        'Description: Maintenance routine to delete the Privilege associated with ProcessName from
        '             the database.
        '
        'Parameters: ProcessName - the process that has the privilege to delete 
        '            Privilege - the privilege to delete
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nIndex As Integer
        Dim nPrivID As Integer
        Dim sChange As String = gpsRM.GetString("psDELETE_PROC_MSG") & ProcessName & _
                                gpsRM.GetString("psDELETE_PRIV_MSG") & Privilege

        If mPrivileges Is Nothing Then Call subGetPrivilegeSet()

        For Each oPriv As udsPrivilege In mPrivileges
            If (oPriv.ProcessName = ProcessName) And (oPriv.PrivilegeName = Privilege) Then
                nPrivID = oPriv.PrivilegeID
                Exit For
            End If
        Next 'oPriv

        If nPrivID > 0 Then
            Try
                Dim DS As DataSet = GetDataSet()

                'Delete this privilege for all users
                Dim UDRs() As DataRow = DS.Tables("Access_List").Select("PrivilegeID = " & nPrivID.ToString)

                For nIndex = UDRs.GetUpperBound(0) To 0 Step -1
                    UDRs(nIndex).Delete()
                Next 'nIndex

                'Delete this privilege for all groups
                Dim GDRs() As DataRow = DS.Tables("Group_Access_List").Select("PrivilegeID = " & nPrivID.ToString)

                For nIndex = GDRs.GetUpperBound(0) To 0 Step -1
                    GDRs(nIndex).Delete()
                Next 'nIndex

                'Delete this privilege
                Dim PDRs() As DataRow = DS.Tables("Privileges").Select("PrivilegeID = " & nPrivID.ToString)

                PDRs(0).Delete()

                'Save the changes
                DS.AcceptChanges()
                DS.WriteXml(msXMLPath & "PWUser.xml")

                'Log Change
                Call mPWCommon.AddChangeRecordToCollection(gsChangeLogArea, LoggedOnUser, _
                                                           mcolZones.ActiveZone.ZoneNumber, String.Empty, _
                                                           String.Empty, sChange, mcolZones.ActiveZone.Name)
                Call subLogChanges()

                'Refresh mPrivileges
                Call subGetPrivilegeSet()

            Catch ex As Exception
                'Message to User, Log internal error
                mDebug.ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), _
                                           ex, msSCREEN_NAME, Status, MessageBoxButtons.OK)
            End Try

        End If

    End Sub

    Private Sub subDeleteProcess(ByVal ProcessName As String)
        '********************************************************************************************
        'Description: Maintenance routine to delete the ProcessName and all associated Privileges
        '             from the database.
        '
        'Parameters: ProcessName - the process to delete 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If mPrivileges Is Nothing Then Call subGetPrivilegeSet()

        'Delete the Privileges associated with this Process
        For Each oPriv As udsPrivilege In mPrivileges
            If oPriv.ProcessName = ProcessName Then
                Call subDeletePrivilege(ProcessName, oPriv.PrivilegeName)
            End If
        Next 'oPriv

        'Delete the Process
        Try
            Dim DS As DataSet = GetDataSet()
            Dim DRs() As DataRow = DS.Tables("Processes").Select("ProcessName = '" & ProcessName & "'")
            Dim sChange As String = gpsRM.GetString("psDELETE_PROC_MSG") & ProcessName

            DRs(0).Delete()
            DS.AcceptChanges()
            DS.WriteXml(msXMLPath & "PWUser.xml")

            'Log Change
            Call mPWCommon.AddChangeRecordToCollection(gsChangeLogArea, LoggedOnUser, _
                                                       mcolZones.ActiveZone.ZoneNumber, String.Empty, _
                                                       String.Empty, sChange, mcolZones.ActiveZone.Name)
            Call subLogChanges()

        Catch ex As Exception
            'Message to User, Log internal error
            mDebug.ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), _
                                       ex, msSCREEN_NAME, Status, MessageBoxButtons.OK)
        End Try

    End Sub

    Private Sub subEnableControls(ByVal bEnable As Boolean)
        '********************************************************************************************
        'Description: Disable or enable controls. Check privileges and edits etc. 
        '
        'Parameters: bEnable - false can be used to disable during load etc 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bRestOfControls As Boolean = False

        btnClose.Enabled = True

        If bEnable = False Then
            btnSave.Enabled = False
            btnUndo.Enabled = False
            btnPrint.Enabled = False
            btnChangeLog.Enabled = False
            bRestOfControls = False
            PnlMain.Enabled = False
            gpbChangePwd.Enabled = False
            gpbEditUser.Enabled = False
            gpbEditGroup.Enabled = False
            gpbSetup.Enabled = False
            gpbMaint.Enabled = False
            gpbMaint.Visible = False
        Else
            Select Case Privilege
                Case ePrivilege.None
                    btnSave.Enabled = False
                    btnUndo.Enabled = False
                    btnPrint.Enabled = mbPrintEnable 'DataLoaded
                    btnChangeLog.Enabled = DataLoaded
                    bRestOfControls = (LoggedOnUser <> String.Empty)
                    PnlMain.Enabled = True
                    gpbChangePwd.Enabled = True
                    gpbEditUser.Enabled = False
                    gpbEditGroup.Enabled = False
                    gpbSetup.Enabled = False
                    gpbMaint.Enabled = False
                    gpbMaint.Visible = False

                Case ePrivilege.Administrate
                    btnSave.Enabled = EditsMade
                    btnUndo.Enabled = EditsMade
                    btnPrint.Enabled = mbPrintEnable 'DataLoaded
                    btnChangeLog.Enabled = DataLoaded
                    bRestOfControls = (LoggedOnUser <> String.Empty)
                    PnlMain.Enabled = DataLoaded
                    gpbChangePwd.Enabled = True
                    gpbEditUser.Enabled = True
                    gpbEditGroup.Enabled = True
                    gpbSetup.Enabled = True
                    gpbMaint.Enabled = (LoggedOnUser.ToUpper = "FANUC")
                    gpbMaint.Visible = (LoggedOnUser.ToUpper = "FANUC")
            End Select
        End If

        '*************************************************************************
        'rest of controls here
        If bRestOfControls Then
            Call subUnlockControls()
        Else
            Call subLockControls()
        End If

    End Sub

    Private Sub subGetPrivilegeSet()
        '********************************************************************************************
        'Description: load an array of udsPrivilege that contains info for all screen privileges
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        ReDim mPrivileges(0)
        Dim DS As DataSet = GetDataSet()
        Dim DRs() As DataRow = DS.Tables("Privileges").Select("PrivilegeID > 0", "ProcessName ASC, Privilege ASC")
        Dim nIndex As Integer

        For Each DR As DataRow In DRs
            ReDim Preserve mPrivileges(nIndex)

            With mPrivileges(nIndex)
                .ProcessName = DR.Item("ProcessName").ToString
                .PrivilegeName = DR.Item("Privilege").ToString
                .PrivilegeID = CType(DR.Item("PrivilegeID"), Integer)
            End With

            nIndex += 1
        Next 'DR

    End Sub

    Private Sub subInitFormText()
        '********************************************************************************************
        'Description: load text for form labels etc
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oCulture As CultureInfo = DisplayCulture

        Try

            With gpsRM
                ' set labels for tab #1
                TabPage1.Text = .GetString("psTAB1_TXT", oCulture)
                gpbChangePwd.Text = .GetString("psGPB_CHG_PWD_TXT", oCulture)
                lblChangePwdCurrent.Text = .GetString("psLBL_CHG_PWD_CUR_TXT", oCulture)
                lblChangePwdNew.Text = .GetString("psLBL_CHG_PWD_NEW_TXT", oCulture)
                lblChangePwdVerifyNew.Text = .GetString("psLBL_CHG_PWD_VRF_TXT", oCulture)
                btnChangePwdAccept.Text = .GetString("psBTN_ACCEPT_TXT", oCulture)
                btnChangePwdCancel.Text = .GetString("psBTN_CANCEL_TXT", oCulture)

                ' set labels for tab #2
                TabPage2.Text = .GetString("psTAB2_TXT", oCulture)
                gpbEditUser.Text = .GetString("psGPB_EDIT_USER_TXT", oCulture)
                gpbEditGroupPwd.Text = .GetString("psGPB_EDIT_GRP_PWD_TXT", oCulture)
                gpbUserPrivileges.Text = .GetString("psGPB_USER_PRIV_TXT", oCulture)
                lblSelectUserGroup.Text = .GetString("psCBO_USER_GRP_TXT", oCulture)
                lblEditPwdNew.Text = .GetString("psLBL_EDIT_PWD_NEW_TXT", oCulture)
                lblEditPwdNewVerify.Text = .GetString("psLBL_EDIT_PWD_VRF_TXT", oCulture)
                btnEditPwdAccept.Text = .GetString("psBTN_ACCEPT_TXT", oCulture)
                btnEditPwdCancel.Text = .GetString("psBTN_CANCEL_TXT", oCulture)
                btnUserAllowed.Text = .GetString("psBTN_ALLOW_TXT", oCulture)
                btnUserNotAllowed.Text = .GetString("psBTN_NOT_ALLOW_TXT", oCulture)
                btnAddUser.Text = .GetString("psBTN_ADD_USER_TXT", oCulture)
                btnRemoveUser.Text = .GetString("psBTN_REM_USER_TXT", oCulture)
                btnChange.Text = .GetString("psBTN_CHG_USER_TXT", oCulture)

                ' set labels for tab #3
                TabPage3.Text = .GetString("psTAB3_TXT", oCulture)
                gpbEditGroup.Text = .GetString("psGPB_EDIT_GRP_TXT", oCulture)
                gpbGroupPrivileges.Text = .GetString("psGPB_GRP_PRIV_TXT", oCulture)
                btnAddGroup.Text = .GetString("psBTN_ADD_GRP_TXT", oCulture)
                btnRemoveGroup.Text = .GetString("psBTN_REM_GRP_TXT", oCulture)
                btnGroupAllowed.Text = .GetString("psBTN_ALLOW_TXT", oCulture)
                btnGroupNotAllowed.Text = .GetString("psBTN_NOT_ALLOW_TXT", oCulture)

                ' set labels for tab #4
                TabPage4.Text = .GetString("psTAB4_TXT", oCulture)
                gpbSetup.Text = .GetString("psGBP_SETUP_TXT", oCulture)
                lblEbableAutoLogout.Text = .GetString("psLBL_EN_AL_TXT", oCulture)
                lblAutoLogoutWarn.Text = .GetString("psLBL_AL_WARN_TXT", oCulture)
                lblAutoLogoutTime.Text = .GetString("psLBL_AL_TIME_TXT", oCulture)
                lblWarnUnits.Text = .GetString("psLBL_AL_UNITS_TXT", oCulture)
                lblTimeoutUnits.Text = .GetString("psLBL_AL_UNITS_TXT", oCulture)
                'add init for process/privilege maintence controls 'RJO 03/29/12
                btnDelPriv.Text = .GetString("psBTN_DEL_PRIV_TXT", oCulture)
                btnDelProc.Text = .GetString("psBTN_DEL_PROC_TXT", oCulture)
                gpbMaint.Text = .GetString("psGPB_MAINT_TXT", oCulture)

                ' modify standard Print Dropdown menu item
                mnuPrint.Text = .GetString("psPRINT_TAB_MNU", oCulture)

            End With 'gpsRM

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitFormText", _
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
        Dim lReply As Response = Response.OK
        'Dim cachePrivilege As mPassword.ePrivilege

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

        Status = gcsRM.GetString("csINITALIZING", DisplayCulture)
        Progress = 1

        Try
            DataLoaded = False
            EditsMade = False
            mbScreenLoaded = False

            subProcessCommandLine()

            mcolZones = New clsZones(String.Empty)
            'Show the Zone combobox only if this computer connects to remote zones
            mbLocal = True
            For Each oZone As clsZone In mcolZones
                If oZone.IsRemoteZone Then
                    mbLocal = False
                    Exit For
                End If
            Next
            lblZone.Visible = Not mbLocal
            cboZone.Visible = Not mbLocal

            Progress = 5

            Me.Text = gpsRM.GetString("psSCREENCAPTION", DisplayCulture)

            mScreenSetup.LoadZoneBox(cboZone, mcolZones, False)

            'init the new password object
            moPassword = New clsPWUser

            Progress = 30

            mScreenSetup.InitializeForm(Me)
            Call subInitFormText()
            Me.Show()

            Progress = 70

            mPrintHtml = New clsPrintHtml(msSCREEN_NAME)

            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound

            'statusbar text
            If (Not mbLocal) And (cboZone.Text = String.Empty) Then
                Status(True) = gcsRM.GetString("csSELECT_ZONE", DisplayCulture)
            End If

            Progress = 98

            mbScreenLoaded = True
            If mbLocal Then
                cboZone.Text = cboZone.Items(0).ToString
                Call subChangeZone()
            End If

            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, Privilege, mcolZones.ActiveZone.IsRemoteZone)

        Catch ex As Exception

            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.AbortRetryIgnore)

            Select Case lReply
                Case Response.Abort
                    Me.Cursor = System.Windows.Forms.Cursors.Default
                    End
                Case Response.Ignore
                Case Response.Retry
                    subInitializeForm()
            End Select
        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try

    End Sub

    Private Sub subLoadData()
        '********************************************************************************************
        'Description:  Data Load Routine
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        DataLoaded = False

        Try
            Dim sTmp As String = gcsRM.GetString("csLOADINGDATA", DisplayCulture)

            Status = sTmp
            Progress = 50

            DataLoaded = True

            Progress = 98

            If DataLoaded Then
                Status = gcsRM.GetString("csLOADDONE", DisplayCulture)

                subShowNewPage(True)

                'this resets edit flag
                DataLoaded = True

                Status = gcsRM.GetString("csLOADDONE", DisplayCulture)

            Else
                Status = gcsRM.GetString("csLOADFAILED", DisplayCulture)
                'load from DB failed
                MessageBox.Show(gcsRM.GetString("csLOADFAILED"), msSCREEN_NAME, MessageBoxButtons.OK, _
                                MessageBoxIcon.Error)

            End If

        Catch ex As Exception

            mDebug.ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), ex, msSCREEN_NAME, _
                                       Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default

        End Try

    End Sub

    Private Sub subLoadGroupCombo()
        '********************************************************************************************
        'Description:  Load cboUserGroup from the database.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim DS As DataSet = GetDataSet()
            Dim DRs() As DataRow
            Dim nIndex As Integer = 1
            ReDim mGroups(0)

            'Add Custom Group
            With mGroups(0)
                .Name = gpsRM.GetString("psCUSTOM_TXT")
                .ID = 0
                cboUserGroup.Items.Add(.Name)
            End With

            DRs = DS.Tables("Groups").Select("GroupID > 0", "GroupID ASC")
            For Each DR As DataRow In DRs
                ReDim Preserve mGroups(nIndex)

                With mGroups(nIndex)
                    .Name = DR.Item("Group").ToString
                    .ID = CType(DR.Item("GroupID"), Integer)
                    cboUserGroup.Items.Add(.Name)
                End With

                nIndex += 1
            Next 'DR

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subLoadGroupCombo", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subLoadGroupPrivilegeTree(ByVal GroupID As Integer)
        '********************************************************************************************
        'Description:  Load TreeView with privilege data for this Group. Enable buttons.
        '
        'Parameters: GroupID - Used to determine which Privilege set to display
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/20/13  MSW     Clean up screen names for display
        ' 08/01/14  RJO/BD  Fixed problems with Displayed Name <> Process Name
        '********************************************************************************************
        Dim oPrivilege As udsPrivilege
        Dim sOFF As String = gpsRM.GetString("psNOT_ALLOW_TXT")
        Dim sON As String = gpsRM.GetString("psALLOW_TXT")

        If GroupID = mnNEW_MEMBER_ID Then
            'Initial "New Group" privilege set - all are "Not Allowed"
            For Each oPrivilege In mPrivileges
                oPrivilege.Allowed = False
                oPrivilege.Allowed_Org = False
            Next
        Else
            'Set Group's Allowed property for each Privilege
            Dim DS As DataSet = GetDataSet()
            Dim DRs() As DataRow = DS.Tables("Group_Access_List").Select("GroupID = " & GroupID.ToString, "PrivilegeID ASC")

            For nIndex As Integer = 0 To mPrivileges.GetUpperBound(0)
                For Each DR As DataRow In DRs
                    If CType(DR.Item("PrivilegeID"), Integer) = mPrivileges(nIndex).PrivilegeID Then
                        mPrivileges(nIndex).Allowed = CType(DR.Item("Allowed"), Boolean)
                        mPrivileges(nIndex).Allowed_Org = mPrivileges(nIndex).Allowed
                        Exit For
                    End If
                Next 'DR
            Next 'nIndex
        End If

        'Build the TreeView
        trvGroupPrivileges.Nodes.Clear()

        For Each oPrivilege In mPrivileges
            Dim sAllowed As String = sOFF
            Dim oColor As Color = mColorOff

            If oPrivilege.Allowed Then
                sAllowed = sON
                oColor = mColorOn
            End If

            ' 08/20/13  MSW     Clean up screen names for display
            Dim sDisplayName As String = mPWCommon.sGetScreenDisplayName(oPrivilege.ProcessName)
            If Not trvGroupPrivileges.Nodes.ContainsKey(oPrivilege.ProcessName) Then
                trvGroupPrivileges.Nodes.Add(oPrivilege.ProcessName, sDisplayName)
                trvGroupPrivileges.Nodes(oPrivilege.ProcessName).Tag = oPrivilege.ProcessName 'RJO 08/01/14
            End If

            trvGroupPrivileges.Nodes(oPrivilege.ProcessName).Nodes.Add(oPrivilege.PrivilegeName, oPrivilege.PrivilegeName & sAllowed)
            trvGroupPrivileges.Nodes(oPrivilege.ProcessName).Nodes(oPrivilege.PrivilegeName).ForeColor = oColor
            trvGroupPrivileges.Nodes(oPrivilege.ProcessName).Nodes(oPrivilege.PrivilegeName).Tag = oPrivilege.PrivilegeName

        Next 'oPrivilege

        trvGroupPrivileges.ExpandAll()
        trvGroupPrivileges.Nodes(0).EnsureVisible()

        'Enable edit buttons
        btnGroupAllowed.Enabled = False 'RJO/BD 08/01/14
        btnGroupNotAllowed.Enabled = False  'RJO/BD 08/01/14

    End Sub

    Private Sub subLoadGroupsList()
        '********************************************************************************************
        'Description:  Load lstGroups. Enable buttons.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim DS As DataSet = GetDataSet()
            Dim DRs() As DataRow
            Dim nIndex As Integer = 1
            ReDim mGroups(0)

            'Add Custom Group
            With mGroups(0)
                .Name = gpsRM.GetString("psCUSTOM_TXT")
                .ID = 0
            End With

            DRs = DS.Tables("Groups").Select("GroupID > 0", "GroupID ASC")
            For Each DR As DataRow In DRs
                ReDim Preserve mGroups(nIndex)

                With mGroups(nIndex)
                    .Name = DR.Item("Group").ToString
                    .ID = CType(DR.Item("GroupID"), Integer)
                    lstGroups.Items.Add(.Name)
                End With

                nIndex += 1
            Next 'DR

            btnAddGroup.Enabled = True

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subLoadGroupsList", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subLoadMaintItems()
        '********************************************************************************************
        'Description:  Read Process and Privilege items from database and display on TabPage4.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/20/13  MSW     Clean up screen names for display
        '********************************************************************************************
        Dim oPrivilege As udsPrivilege

        'Build the TreeView
        Call subGetPrivilegeSet()
        With trvMaint
            .Nodes.Clear()

            For Each oPrivilege In mPrivileges
                ' 08/20/13  MSW     Clean up screen names for display
                Dim sDisplayName As String = mPWCommon.sGetScreenDisplayName(oPrivilege.ProcessName)

                If Not .Nodes.ContainsKey(oPrivilege.ProcessName) Then
                    .Nodes.Add(oPrivilege.ProcessName, sDisplayName)
                    .Nodes(oPrivilege.ProcessName).Tag = oPrivilege.ProcessName
                End If

                .Nodes(oPrivilege.ProcessName).Nodes.Add(oPrivilege.PrivilegeName, oPrivilege.PrivilegeName)
                .Nodes(oPrivilege.ProcessName).Nodes(oPrivilege.PrivilegeName).ForeColor = Color.Black
                .Nodes(oPrivilege.ProcessName).Nodes(oPrivilege.PrivilegeName).Tag = oPrivilege.PrivilegeName

            Next 'oPrivilege

            .ExpandAll()
            .Nodes(0).EnsureVisible()
        End With

        'Disable edit buttons
        btnDelPriv.Enabled = False
        btnDelProc.Enabled = False

    End Sub

    Private Sub subLoadSetupItems()
        '********************************************************************************************
        'Description:  Read Setup items from database and display on TabPage4.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim DS As DataSet = GetDataSet()
            Dim DR As DataRow = DS.Tables("Setup").Rows(0)

            With DR
                mEditSetup.AutoLogoutTime = CType(.Item("AutoLogoutTime"), Integer)
                mEditSetup.AutoLogoutTime_Org = mEditSetup.AutoLogoutTime
                ftbAutoLogoutTime.Text = .Item("AutoLogoutTime").ToString
                ftbAutoLogoutTime.ForeColor = Color.Black

                mEditSetup.AutoLogoutWarn = CType(.Item("AutoLogoutWarningTime"), Integer)
                mEditSetup.AutoLogoutWarn_Org = mEditSetup.AutoLogoutWarn
                ftbAutoLogoutWarn.Text = .Item("AutoLogoutWarningTime").ToString
                ftbAutoLogoutWarn.ForeColor = Color.Black

                If CType(.Item("AutoLogout"), Boolean) Then
                    mEditSetup.AutoLogoutEnable = True
                    mEditSetup.AutoLogoutEnable_Org = True
                    cboEnableAutoLogout.SelectedIndex = 1
                Else
                    mEditSetup.AutoLogoutEnable = False
                    mEditSetup.AutoLogoutEnable_Org = False
                    cboEnableAutoLogout.SelectedIndex = 0
                End If
            End With

            EditsMade = False

        Catch ex As Exception
            'Message to User, Log internal error
            mDebug.ShowErrorMessagebox(gpsRM.GetString("psDATA_LOAD_FAIL_MSG", DisplayCulture), _
                                       ex, msSCREEN_NAME, Status, MessageBoxButtons.OK)
        End Try

    End Sub

    Private Sub subLoadUserPrivilegeTree(ByVal UserID As Integer)
        '********************************************************************************************
        'Description:  Load TreeView with privilege data for this User. Enable buttons.
        '
        'Parameters: UserID - Used to determine which Privilege set to display
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/20/13  MSW     Clean up screen names for display
        ' 08/01/14  RJO/BD  Fixed problems with Displayed Name <> Process Name
        '********************************************************************************************
        Dim oPrivilege As udsPrivilege
        Dim sOFF As String = gpsRM.GetString("psNOT_ALLOW_TXT")
        Dim sON As String = gpsRM.GetString("psALLOW_TXT")

        'Set User's Allowed property for each Privilege
        If UserID = mnNEW_MEMBER_ID Then
            'Initial "Custom" privilege set - all are "Not Allowed"
            For Each oPrivilege In mPrivileges
                oPrivilege.Allowed = False
                oPrivilege.Allowed_Org = False
            Next
        Else
            Dim DS As DataSet = GetDataSet()
            Dim DRs() As DataRow = DS.Tables("Access_List").Select("UserID = " & UserID.ToString, "PrivilegeID ASC")

            For nIndex As Integer = 0 To mPrivileges.GetUpperBound(0)
                For Each DR As DataRow In DRs
                    If CType(DR.Item("PrivilegeID"), Integer) = mPrivileges(nIndex).PrivilegeID Then
                        mPrivileges(nIndex).Allowed = CType(DR.Item("Allowed"), Boolean)
                        mPrivileges(nIndex).Allowed_Org = mPrivileges(nIndex).Allowed
                        Exit For
                    End If
                Next 'DR
            Next 'nIndex
        End If

        'Build the TreeView
        trvUserPrivileges.Nodes.Clear()

        For Each oPrivilege In mPrivileges
            Dim sAllowed As String = sOFF
            Dim oColor As Color = mColorOff

            If oPrivilege.Allowed Then
                sAllowed = sON
                oColor = mColorOn
            End If
            ' 08/20/13  MSW     Clean up screen names for display
            Dim sDisplayName As String = mPWCommon.sGetScreenDisplayName(oPrivilege.ProcessName)
            If Not trvUserPrivileges.Nodes.ContainsKey(oPrivilege.ProcessName) Then
                trvUserPrivileges.Nodes.Add(oPrivilege.ProcessName, sDisplayName)
                trvUserPrivileges.Nodes(oPrivilege.ProcessName).Tag = oPrivilege.ProcessName 'RJO 08/01/14
            End If

            trvUserPrivileges.Nodes(oPrivilege.ProcessName).Nodes.Add(oPrivilege.PrivilegeName, oPrivilege.PrivilegeName & sAllowed)
            trvUserPrivileges.Nodes(oPrivilege.ProcessName).Nodes(oPrivilege.PrivilegeName).ForeColor = oColor
            trvUserPrivileges.Nodes(oPrivilege.ProcessName).Nodes(oPrivilege.PrivilegeName).Tag = oPrivilege.PrivilegeName

        Next 'oPrivilege

        trvUserPrivileges.ExpandAll()
        trvUserPrivileges.Nodes(0).EnsureVisible()

        'Enable edit buttons
        btnUserAllowed.Enabled = False  'RJO/BD 08/01/14
        btnUserNotAllowed.Enabled = False   'RJO/BD 08/01/14

    End Sub

    Private Sub subLoadUsersList(ByVal sUsers As String)
        '********************************************************************************************
        'Description:  Load lstUsers with data returned by Password.NET. Enable buttons.
        '
        'Parameters: DeCrypted Users string
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If sUsers <> String.Empty Then
            Dim sData() As String = Strings.Split(sUsers, ",")
            Dim nIndex As Integer
            ReDim mUsers(0)

            For sItem As Integer = 0 To sData.GetUpperBound(0) Step 3
                ReDim Preserve mUsers(nIndex)

                With mUsers(nIndex)
                    .Name = sData(sItem)
                    .ID = CType(sData(sItem + 1), Integer)
                    .GroupID = CType(sData(sItem + 2), Integer)
                    lstUsers.Items.Add(.Name)
                End With

                nIndex += 1
            Next 'sItem
        End If

        btnAddUser.Enabled = True

    End Sub

    Private Sub subLockControls()
        '********************************************************************************************
        'Description:  Disable Change Password controls if no user is logged in.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        txtChangePwdCurrent.Enabled = False
        txtChangePwdNew.Enabled = False
        txtChangePwdVerifyNew.Enabled = False
        btnChangePwdAccept.Enabled = False
        btnChangePwdCancel.Enabled = False

    End Sub

    Private Sub subLogChanges()
        '********************************************************************************************
        'Description:  Log changes to the database after a save
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        mPWCommon.SaveToChangeLog(mcolZones.ActiveZone) 'SQL database

    End Sub

    Private Sub subPrintPrivileges(ByVal sTitle() As String, ByVal sSubTitle() As String)
        '********************************************************************************************
        'Description:  User/Group Privilege Data Print Routine
        '
        'Parameters: Report
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sText(0) As String

        Try
            Progress = 10

            'Column Headers
            sText(0) = gpsRM.GetString("psPROC_NAME_TXT", DisplayCulture) & vbTab & _
                       gpsRM.GetString("psPRIV_NAME_TXT", DisplayCulture) & vbTab & _
                       gpsRM.GetString("psPERMISSION_TXT", DisplayCulture)
            sText(0) = Strings.Trim(sText(0))

            'Column Data
            Dim nIndex As Integer = 1
            Dim sAllow As String = gpsRM.GetString("psBTN_ALLOW_TXT", DisplayCulture)
            Dim sNotAllow As String = gpsRM.GetString("psBTN_NOT_ALLOW_TXT", DisplayCulture)
            Dim sProcess As String = String.Empty

            For Each oPriv As udsPrivilege In mPrivileges
                ReDim Preserve sText(nIndex)
                Dim sStatus As String = sNotAllow
                Dim sPrintProcess As String = " "

                With oPriv
                    If .Allowed_Org Then sStatus = sAllow
                    If .ProcessName <> sProcess Then
                        sProcess = .ProcessName
                        sPrintProcess = .ProcessName
                    End If
                    sText(nIndex) = sPrintProcess & vbTab & .PrivilegeName & vbTab & sStatus
                    sText(nIndex) = Strings.Trim(sText(nIndex))
                End With 'oPriv

                nIndex += 1
            Next 'oPriv

            Progress = 30
            mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle)
            Progress = 55
            Status = gcsRM.GetString("csPRINT_FORMATTING", DisplayCulture)

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                               Status, MessageBoxButtons.OK)
        End Try

    End Sub

    Private Sub subPrintSetup(ByVal sTitle() As String, ByVal sSubTitle() As String)
        '********************************************************************************************
        'Description:  Setup Data Print Routine
        '
        'Parameters: Report
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sText(3) As String

        Try
            Progress = 10

            'Column Headers
            sText(0) = gpsRM.GetString("psDESC_TXT", DisplayCulture) & vbTab & _
                       gpsRM.GetString("psVALUE_TXT", DisplayCulture)
            sText(0) = Strings.Trim(sText(0))

            'Column Data
            Dim sEnable As String = gpsRM.GetString("psDISABLED_TXT", DisplayCulture)

            If mEditSetup.AutoLogoutEnable_Org Then
                sEnable = gpsRM.GetString("psENABLED_TXT", DisplayCulture)
            End If

            sText(1) = gpsRM.GetString("psLBL_EN_AL_TXT", DisplayCulture) & vbTab & sEnable
            sText(1) = Trim(sText(1))

            sText(2) = gpsRM.GetString("psLBL_AL_WARN_TXT", DisplayCulture) & vbTab & _
                       mEditSetup.AutoLogoutWarn_Org.ToString & " " & _
                       gpsRM.GetString("psLBL_AL_UNITS_TXT", DisplayCulture)
            sText(2) = Trim(sText(2))

            sText(3) = gpsRM.GetString("psLBL_AL_TIME_TXT", DisplayCulture) & vbTab & _
                       mEditSetup.AutoLogoutTime_Org.ToString & " " & _
                       gpsRM.GetString("psLBL_AL_UNITS_TXT", DisplayCulture)
            sText(3) = Trim(sText(3))

            Progress = 30
            mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle)
            Progress = 55
            Status = gcsRM.GetString("csPRINT_FORMATTING", DisplayCulture)

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                               Status, MessageBoxButtons.OK)
        End Try

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
        Dim sCultureArg As String = "/culture="

        'If a culture string has been passed in, set the current culture (display language)
        For Each s As String In My.Application.CommandLineArgs
            If s.ToLower.StartsWith(sCultureArg) Then
                Culture = s.Remove(0, sCultureArg.Length)
            End If
        Next

    End Sub

    Private Sub subSaveData()
        '********************************************************************************************
        'Description:  Data Save Routine
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case Privilege
            Case ePrivilege.None
                ' shouldnt be here
                subEnableControls(False)
                Exit Sub

            Case Else
                'ok
        End Select

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

        Try
            Status = gcsRM.GetString("csSAVINGDATA", DisplayCulture)

            ' do save
            If bSaveToGUI() Then

                Call subLogChanges()
                Call subLoadData()
                ' save done
                EditsMade = False

                'subShowNewPage(True) 'RJO 02/28/12
                Status = gcsRM.GetString("csSAVE_DONE", DisplayCulture)

            Else
                Status = gcsRM.GetString("csSAVEFAILED", DisplayCulture)
                'save to DB failed
                MessageBox.Show(gcsRM.GetString("csSAVEFAILED"), msSCREEN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If 'bSaveToGUI()

        Catch ex As Exception

            mDebug.ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                       Status, MessageBoxButtons.OK)

        Finally

            Me.Cursor = System.Windows.Forms.Cursors.Default

        End Try

    End Sub

    Private Sub subSetPrivilege(ByRef TrvControl As TreeView, ByRef Node As TreeNode, ByVal Allowed As Boolean)
        '********************************************************************************************
        'Description:  Sets privilege Node in TrvControl to the value of Allowed, and then updates
        '              display and mPrivileges array.
        '
        'Parameters: TrvControl - Treeview control that contains the node
        '            Node       - The node to Toggle
        '            Allowed    - The new Privilege state
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/01/14  RJO/BD  Fixed problems with Displayed Name <> Process Name
        '********************************************************************************************
        Dim sNode As String = Node.Tag.ToString

        Dim sProcess As String = Node.Parent.Tag.ToString 'Node.Parent.Text 'RJO 08/01/14

        For nIndex As Integer = 0 To mPrivileges.GetUpperBound(0)
            With mPrivileges(nIndex)
                If .ProcessName.ToLower = sProcess.ToLower And .PrivilegeName = sNode Then
                    .Allowed = Allowed

                    If .Allowed Then
                        Node.Text = sNode & gpsRM.GetString("psALLOW_TXT")
                        Node.ForeColor = mColorOn
                    Else
                        Node.Text = sNode & gpsRM.GetString("psNOT_ALLOW_TXT")
                        Node.ForeColor = mColorOff
                    End If

                    If .Allowed <> .Allowed_Org Then EditsMade = True
                    Exit For
                End If
            End With ' mPrivileges(nIndex)
        Next 'nIndex

    End Sub

    Private Sub subShowButtons(ByVal TabName As String)
        '********************************************************************************************
        'Description:  Show/hide menu buttons as required for current Tab
        '
        'Parameters: Current Tab name
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case TabName
            Case "TabPage1"
                btnSave.Visible = False
                btnPrint.Visible = False
                btnUndo.Visible = False
            Case Else
                btnSave.Visible = True
                btnPrint.Visible = True
                btnUndo.Visible = True
        End Select

    End Sub

    Private Sub subShowChangeLog(ByVal nIndex As Integer)
        '********************************************************************************************
        'Description:  show the change log form
        '
        'Parameters: how many changes to show
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/16/13  MSW     Standalone ChangeLog                                          4.01.05.00
        '********************************************************************************************
        
        Try
        
            mPWCommon.subShowChangeLog(mcolZones.CurrentZone, gsChangeLogArea, msSCREEN_NAME, gcsRM.GetString("csALL"), _
                                        gcsRM.GetString("csALL"), mDeclares.eParamType.None, nIndex, False, False, oIPC)
        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
                                
        End Try

    End Sub

    Private Sub subShowEditGroupPwdPanel()
        '********************************************************************************************
        'Description:  Initialize and show the Edit Group/Password panel
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        lstUsers.Enabled = False
        txtEditPwdNew.Text = String.Empty
        txtEditPwdNewVerify.Text = String.Empty
        btnEditPwdAccept.Enabled = False
        gpbEditGroupPwd.Visible = True
        cboUserGroup.Enabled = True
        'Select the user's current group in the group combo.
        If mEditUser.GroupID = 0 Then
            cboUserGroup.SelectedIndex = 0
        Else
            Dim sGroupName As String = String.Empty

            For Each oGroup As udsGroup In mGroups
                If oGroup.ID = mEditUser.GroupID Then
                    sGroupName = oGroup.Name
                    Exit For
                End If
            Next 'oGroup

            cboUserGroup.SelectedItem = sGroupName
        End If

    End Sub

    Private Sub subShowNewPage(ByVal BlockEvents As Boolean)
        '********************************************************************************************
        'Description:  Data is all loaded or changed and screen needs to be refreshed
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbEventBlocker = BlockEvents

        Call subShowButtons(msCurrentTabName)
        Select Case msCurrentTabName
            Case "TabPage1"   '[User Password Tab]
                Call subShowTabPage1()
            Case "TabPage2"   '[User Privileges Tab]
                Call subShowTabPage2()
            Case "TabPage3"   '[Group Privileges Tab]
                Call subShowTabPage3()
            Case "TabPage4"   '[Auto Logout Tab]
                Call subShowTabPage4()
        End Select

        subEnableControls(True)
        mbEventBlocker = False

    End Sub

    Private Sub subShowTabPage1()
        '********************************************************************************************
        'Description:  Initialize TabPage1 controls, then show it.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            msSCREEN_DUMP_NAME = "Utilities_PWUserMaint_ChangePwd.jpg"

            txtChangePwdCurrent.Text = String.Empty
            txtChangePwdNew.Text = String.Empty
            txtChangePwdVerifyNew.Text = String.Empty

            TabPage1.Show()

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subShowTabPage1", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subShowTabPage2()
        '********************************************************************************************
        'Description:  Initialize TabPage2 controls, then show it.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            msSCREEN_DUMP_NAME = "Utilities_PWUserMaint_UserPriv.jpg"

            lstUsers.Items.Clear()
            trvUserPrivileges.Nodes.Clear()
            cboUserGroup.Items.Clear()
            cboUserGroup.Enabled = False
            txtEditPwdNew.Text = String.Empty
            txtEditPwdNewVerify.Text = String.Empty
            btnUserAllowed.Enabled = False
            btnUserNotAllowed.Enabled = False
            gpbEditGroupPwd.Visible = False
            gpbUserPrivileges.Visible = True
            btnAddUser.Enabled = False
            btnRemoveUser.Enabled = False
            btnChange.Enabled = False

            If gpbEditUser.Enabled Then
                'request user names from Password.NET
                Dim sMsg() As String = {moPassword.ProcessName, "getusernames"}

                oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sMsg)

                'populate the full set of screen privileges
                Call subGetPrivilegeSet()
                'populate User Groups combo box
                Call subLoadGroupCombo()
            End If

            TabPage2.Show()

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subShowTabPage2", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subShowTabPage3()
        '********************************************************************************************
        'Description:  Initialize TabPage3 controls, then show it.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            msSCREEN_DUMP_NAME = "Utilities_PWUserMaint_GroupPriv.jpg"

            lstGroups.Items.Clear()
            trvGroupPrivileges.Nodes.Clear()
            btnGroupAllowed.Enabled = False
            btnGroupNotAllowed.Enabled = False
            btnAddGroup.Enabled = False
            btnRemoveGroup.Enabled = False

            If gpbEditGroup.Enabled Then
                Call subLoadGroupsList()
                'populate the full set of screen privileges
                Call subGetPrivilegeSet()
            End If

            TabPage3.Show()

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subShowTabPage3", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subShowTabPage4()
        '********************************************************************************************
        'Description:  Initialize TabPage4 controls, then show it.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            mbEventBlocker = True
            msSCREEN_DUMP_NAME = "Utilities_PWUserMaint_Setup.jpg"

            With cboEnableAutoLogout
                .Items.Clear()
                .Items.Add(gpsRM.GetString("psDISABLED_TXT"))
                .Items.Add(gpsRM.GetString("psENABLED_TXT"))
            End With

            ftbAutoLogoutTime.Text = String.Empty
            ftbAutoLogoutWarn.Text = String.Empty

            If gpbSetup.Enabled Then
                Call subLoadSetupItems()
                Call subLoadMaintItems() 'RJO 03/29/12
                mbPrintEnable = True
                Call subEnableControls(True)
            End If

            mbEventBlocker = False

            TabPage4.Show()

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subShowTabPage4", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subTogglePrivilege(ByRef TrvControl As TreeView, ByRef Node As TreeNode)
        '********************************************************************************************
        'Description:  Toggles privilege Node in TrvControl, and then updates display and mPrivileges
        '              array.
        '
        'Parameters: TrvControl - Treeview control that contains the node
        '            Node       - The node to Toggle
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/20/13  MSW     Clean up screen names for display 
        '********************************************************************************************
        Dim sNode As String = Node.Tag.ToString
        Dim sProcess As String = Node.Parent.Name

        For nIndex As Integer = 0 To mPrivileges.GetUpperBound(0)
            With mPrivileges(nIndex)
                If .ProcessName = sProcess And .PrivilegeName = sNode Then
                    .Allowed = Not .Allowed

                    If .Allowed Then
                        Node.Text = sNode & gpsRM.GetString("psALLOW_TXT")
                        Node.ForeColor = mColorOn
                    Else
                        Node.Text = sNode & gpsRM.GetString("psNOT_ALLOW_TXT")
                        Node.ForeColor = mColorOff
                    End If

                    EditsMade = True
                    Exit For
                End If
            End With ' mPrivileges(nIndex)
        Next 'nIndex

    End Sub

    Private Sub subUndoData()
        '********************************************************************************************
        'Description:  Undo Button Pressed
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If MessageBox.Show(gcsRM.GetString("csUNDOMSG", DisplayCulture), msSCREEN_NAME, _
                           MessageBoxButtons.OKCancel, MessageBoxIcon.Question, _
                           MessageBoxDefaultButton.Button2) = Response.OK Then

            Select Case msCurrentTabName
                Case "TabPage2"
                    'undo changes to mPrivileges
                    For nIndex As Integer = 0 To mPrivileges.GetUpperBound(0)
                        mPrivileges(nIndex).Allowed = mPrivileges(nIndex).Allowed_Org
                    Next

                    Call subLoadUserPrivilegeTree(mEditUser.ID)
                    EditsMade = False

                Case "TabPage3"
                    'undo changes to mPrivileges
                    For nIndex As Integer = 0 To mPrivileges.GetUpperBound(0)
                        mPrivileges(nIndex).Allowed = mPrivileges(nIndex).Allowed_Org
                    Next

                    Call subLoadGroupPrivilegeTree(mEditGroup.ID)
                    EditsMade = False

                Case "TabPage4"
                    'undo Setup changes
                    With mEditSetup
                        If .AutoLogoutEnable <> .AutoLogoutEnable_Org Then
                            If .AutoLogoutEnable_Org Then
                                cboEnableAutoLogout.SelectedIndex = 1
                            Else
                                cboEnableAutoLogout.SelectedIndex = 0
                            End If
                            .AutoLogoutEnable = .AutoLogoutEnable_Org
                        End If

                        ftbAutoLogoutTime.Text = .AutoLogoutTime_Org.ToString
                        ftbAutoLogoutTime.ForeColor = Color.Black
                        ftbAutoLogoutTime.BackColor = Color.White
                        .AutoLogoutTime = .AutoLogoutTime_Org

                        ftbAutoLogoutWarn.Text = .AutoLogoutWarn_Org.ToString
                        ftbAutoLogoutWarn.ForeColor = Color.Black
                        ftbAutoLogoutWarn.BackColor = Color.White
                        .AutoLogoutWarn = .AutoLogoutWarn_Org
                    End With 'mEditSetup
                    EditsMade = False

                Case Else
                    'Shouldn't be here
            End Select

        End If 'MessageBox.Show...

    End Sub

    Private Sub subUnlockControls()
        '********************************************************************************************
        'Description:  The current user has edit privilege, unlock the controls to allow changes to 
        '              settings.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        txtChangePwdCurrent.Enabled = True
        txtChangePwdNew.Enabled = True
        txtChangePwdVerifyNew.Enabled = True
        btnChangePwdAccept.Enabled = False
        btnChangePwdCancel.Enabled = True

    End Sub

    Private Sub subUpdateChangeLog(ByVal NewValue As String, _
                                        ByVal OldValue As String, ByVal ParamName As String, _
                                        ByVal oZone As clsZone, ByVal WhatChanged As String, _
                                        ByVal DeviceName As String)
        '********************************************************************************************
        'Description:  Build up the change text for a value change
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp As String = WhatChanged & " "

        sTmp = sTmp & gcsRM.GetString("csCHANGED_FROM", DisplayCulture) & " " & OldValue & " " & _
                        gcsRM.GetString("csTO", DisplayCulture) & " " & NewValue

        Call mPWCommon.AddChangeRecordToCollection(gsChangeLogArea, LoggedOnUser, _
                                        oZone.ZoneNumber, DeviceName, ParamName, sTmp, oZone.Name)

    End Sub

    Private Function ValidateSetup() As Boolean
        '********************************************************************************************
        'Description:  Return True if Setup values are valid.
        '
        'Parameters: None
        'Returns:    True if Setup values are valid
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bValid As Boolean = True
        Dim sAboveMax As String = gcsRM.GetString("csVAL_ABOVE_MAX")
        Dim sBelowMin As String = gcsRM.GetString("csVAL_BELOW_MIN")
        Dim sMsg As String
        Dim sTitle As String = gpsRM.GetString("psGBP_SETUP_TXT")

        With mEditSetup
            If .AutoLogoutEnable Then
                'Check Warning and Timeout values
                sMsg = gpsRM.GetString("psLBL_AL_TIME_TXT") & " "
                If .AutoLogoutTime > mnTIMEOUT_MAX Then
                    sMsg = sMsg & sAboveMax
                    bValid = False
                ElseIf .AutoLogoutTime < mnTIMEOUT_MIN Then
                    sMsg = sMsg & sBelowMin
                    bValid = False
                End If

                If Not bValid Then
                    .AutoLogoutTime = .AutoLogoutTime_Org
                    ftbAutoLogoutTime.Text = .AutoLogoutTime.ToString
                    MessageBox.Show(sMsg, sTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End If

                sMsg = gpsRM.GetString("psLBL_AL_WARN_TXT") & " "
                If .AutoLogoutWarn > mnWARNING_MAX Then
                    sMsg = sMsg & sAboveMax
                    bValid = False
                ElseIf .AutoLogoutWarn < mnWARNING_MIN Then
                    sMsg = sMsg & sBelowMin
                    bValid = False
                End If

                If bValid Then
                    If .AutoLogoutWarn >= .AutoLogoutTime Then
                        sMsg = sMsg & gpsRM.GetString("psINV_AL_WARN_MSG")
                        bValid = False
                    End If
                Else
                    .AutoLogoutWarn = .AutoLogoutWarn_Org
                End If

                If Not bValid Then
                    ftbAutoLogoutWarn.Text = .AutoLogoutWarn.ToString
                    MessageBox.Show(sMsg, sTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End If

            End If '.AutoLogoutEnable

        End With 'mEditSetup

        Return bValid

    End Function

#End Region

#Region " Form Events "

    Private Sub frmMain_Closing(ByVal sender As Object, _
                            ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        '********************************************************************************************
        'Description:  setting e.cancel to true aborts close
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If EditsMade Then
            e.Cancel = (bAskForSave() = False)
        End If

    End Sub

    Private Sub frmMain_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        '********************************************************************************************
        'Description: Trap alt - key  combinations to simulate menu button clicks
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '    04/20/12   MSW     frmMain_KeyDown-Add excape key handler
		'    09/30/13   MSW     Save screenshots as jpegs
        '********************************************************************************************
        Dim sKeyValue As String = String.Empty

        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode
                Case Keys.F1
                    'Help Screen request
                    sKeyValue = "btnHelp"
                    subLaunchHelp(gs_HELP_UTILITIES_PWUSERMAINT, oIPC)

                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty

                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath & msSCREEN_DUMP_NAME, Imaging.ImageFormat.Jpeg)

                Case Keys.Escape
                    Me.Close()

                Case Else

            End Select
        End If

        'TODO - Add HotKeys
        'If sKeyValue = String.Empty Then
        '    'Trap Main Menu HotKey presses
        '    If e.Alt And (Not (e.Control Or e.Shift)) Then
        '        For Each oMenuButton As Windows.Forms.ToolStripButton In tlsMain.Items
        '            If oMenuButton.Tag.ToString = e.KeyCode.ToString Then
        '                sKeyValue = oMenuButton.Name
        '                Exit For
        '            End If
        '        Next 'oMenuButton
        '    End If
        'End If 'sKeyValue = String.Empty

        'If sKeyValue <> String.Empty Then
        '    'Click the Menu Button
        '    If tlsMain.Items(sKeyValue).Enabled Then
        '        tlsMain.Items(sKeyValue).PerformClick()
        '    End If
        'End If

    End Sub

    Private Sub frmMain_Layout(ByVal sender As Object, _
                        ByVal e As System.Windows.Forms.LayoutEventArgs) Handles MyBase.Layout
        '********************************************************************************************
        'Description:  Form needs a redraw due to resize
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'TODO - find out what this -70 is from.
        Dim nHeight As Integer = tscMain.ContentPanel.Height - 70
        Dim nWidth As Integer = tscMain.ContentPanel.Width - (PnlMain.Left * 2)

        If nHeight < 100 Then nHeight = 100
        If nWidth < 100 Then nWidth = 100

        PnlMain.Height = nHeight
        PnlMain.Width = nWidth

    End Sub

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                                        Handles MyBase.Load
        '********************************************************************************************
        'Description: Runs after class constructor (new)
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim sCultureArg As String = "/culture="

            'If a culture string has been passed in, set the current culture (display language)
            For Each s As String In My.Application.CommandLineArgs
                If s.ToLower.StartsWith(sCultureArg) Then
                    Culture = s.Remove(0, sCultureArg.Length)
                    Exit For
                End If
            Next

            Call subInitializeForm()

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: frmMain_Load", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Public Sub New()
        '********************************************************************************************
        'Description:  I feel like a new form!
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'for language support
        mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, msBASE_ASSEMBLY_LOCAL, _
                                      String.Empty)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub oIPC_NewMessage(ByVal Schema As String, ByVal DS As System.Data.DataSet) Handles oIPC.NewMessage
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
            Dim dNewMessage As New NewMessage_CallBack(AddressOf oIPC_NewMessage)
            Me.BeginInvoke(dNewMessage, New Object() {Schema, DS})
        Else
            Select Case Schema.ToLower
                Case oIPC.CONTROL_MSG_SCHEMA.ToLower
                    'Call subDoControlAction(DS.Tables("Command").Rows(0))

                Case oIPC.PASSWORD_MSG_SCHEMA.ToLower
                    Call moPassword.ProcessPasswordMessage(DS.Tables("Command").Rows(0))

                Case Else
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: oIPC_NewMessage", _
                                           "Unrecognized schema [" & Schema & "].")
            End Select
        End If 'Me.InvokeRequired
    End Sub

#End Region

#Region " Control Events "

    Private Sub btnAddGroup_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddGroup.Click
        '********************************************************************************************
        'Description:  Show dialog to allow the addition of a new group. 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sMsg As String = gpsRM.GetString("psENTER_NEW_GROUP_MSG")
        Dim sTitle As String = gpsRM.GetString("psBTN_ADD_GRP_TXT")
        Dim sGroupName As String = InputBox(sMsg, sTitle)

        If sGroupName = String.Empty Then
            MessageBox.Show(gpsRM.GetString("psEMPTY_GROUP_ERR_MSG"), sTitle, _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            Dim nNewIndex As Integer

            If Not (mGroups Is Nothing) Then
                'Make sure sGroupName doesn't exist in Groups list
                For Each oGroup As udsGroup In mGroups
                    If oGroup.Name.ToLower = sGroupName.ToLower Then
                        MessageBox.Show(gpsRM.GetString("psGROUP_NOT_UNIQUE_MSG"), sTitle, _
                                        MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit Sub
                    End If
                Next 'oGroup

                nNewIndex = mGroups.GetUpperBound(0) + 1
            End If

            ReDim Preserve mGroups(nNewIndex)

            With mGroups(nNewIndex)
                .Name = sGroupName
                .ID = mnNEW_MEMBER_ID
            End With

            Try
                Dim DS As DataSet = GetDataSet()
                Dim DR As DataRow
                Dim nNewID As Integer = GetUniqueGroupID()
                Dim sChange As String = gpsRM.GetString("psADD_GROUP_MSG") & sGroupName

                'Add new Group and associated Privilege set to database.
                DR = DS.Tables("Groups").NewRow
                DR.Item("GroupID") = nNewID
                DR.Item("Group") = sGroupName
                DS.Tables("Groups").Rows.Add(DR)
                DS.AcceptChanges()

                For Each oPriv As udsPrivilege In mPrivileges
                    DR = DS.Tables("Group_Access_List").NewRow
                    DR.Item("GroupID") = nNewID
                    DR.Item("PrivilegeID") = oPriv.PrivilegeID
                    DR.Item("Allowed") = False
                    DS.Tables("Group_Access_List").Rows.Add(DR)
                Next 'oPriv

                DS.AcceptChanges()
                DS.WriteXml(msXMLPath & "PWUser.xml")

                'Log Change    
                Call mPWCommon.AddChangeRecordToCollection(gsChangeLogArea, LoggedOnUser, _
                                           mcolZones.ActiveZone.ZoneNumber, String.Empty, _
                                           String.Empty, sChange, mcolZones.ActiveZone.Name)
                Call subLogChanges()

                mGroups(nNewIndex).ID = nNewID
                With mEditGroup
                    .Name = sGroupName
                    .ID = nNewID
                End With

            Catch ex As Exception
                'Message to User, Log internal error
                mDebug.ShowErrorMessagebox(gpsRM.GetString("psDATA_SAVE_FAIL_MSG", DisplayCulture), _
                                           ex, msSCREEN_NAME, Status, MessageBoxButtons.OK)
                'Delete New Group from mGroups 
                Call CancelNewGroup()
            End Try

            Call subShowTabPage3()

        End If 'sGroupName = String.Empty

    End Sub

    Private Sub btnAddUser_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddUser.Click
        '********************************************************************************************
        'Description:  Show dialog to allow the addition of a new user. 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sMsg As String = gpsRM.GetString("psENTER_NEW_USER_MSG")
        Dim sTitle As String = gpsRM.GetString("psBTN_ADD_USER_TXT")
        Dim sUserName As String = InputBox(sMsg, sTitle)

        If sUserName = String.Empty Then
            MessageBox.Show(gpsRM.GetString("psEMPTY_STRING_MSG"), sTitle, _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            Try
                Dim nNewIndex As Integer

                If Not (mUsers Is Nothing) Then
                    'Make sure sUserName doesn't exist in Users list
                    For Each oUser As udsUser In mUsers
                        If oUser.Name.ToLower = sUserName.ToLower Then
                            MessageBox.Show(gpsRM.GetString("psUSER_NOT_UNIQUE_MSG"), sTitle, _
                                            MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Exit Sub
                        End If
                    Next 'oUser

                    nNewIndex = mUsers.GetUpperBound(0) + 1
                End If

                ReDim Preserve mUsers(nNewIndex)

                With mUsers(nNewIndex)
                    .Name = sUserName
                    .ID = mnNEW_MEMBER_ID
                    .GroupID = 0
                End With

                With mEditUser
                    .Name = sUserName
                    .ID = mnNEW_MEMBER_ID
                    .GroupID = 0
                End With

                'Show gpbEditGroupPwd visible with "Custom" group selected
                Call subShowEditGroupPwdPanel()

            Catch ex As Exception
                'Message to User, Log internal error
                mDebug.ShowErrorMessagebox(gpsRM.GetString("psDATA_LOAD_FAIL_MSG", DisplayCulture), _
                                           ex, msSCREEN_NAME, Status, MessageBoxButtons.OK)
                'Delete New User from mUsers 
                Call CancelNewUser()
                Call subShowTabPage2()
            End Try
        End If

    End Sub

    Private Sub btnChange_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnChange.Click
        '********************************************************************************************
        'Description:  Show the Edit Group/Password Panel for the selected user. 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        mEditUser = mUsers(lstUsers.SelectedIndex)
        mbEventBlocker = True
        btnAddUser.Enabled = False
        btnChange.Enabled = False
        btnRemoveUser.Enabled = False
        Call subShowEditGroupPwdPanel()
        mbEventBlocker = False

    End Sub

    Private Sub btnChangePwdAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnChangePwdAccept.Click
        '********************************************************************************************
        'Description:  Password Change was entered by user. 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Make sure logged in user is not user "FANUC"
        If LoggedOnUser.ToUpper = "FANUC" Then
            MessageBox.Show(gpsRM.GetString("psCHG_USER_FANUC_MSG"), gpsRM.GetString("psGPB_CHG_PWD_TXT"), _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If
        'Make sure the New and Verify passwords match
        If txtChangePwdNew.Text <> txtChangePwdVerifyNew.Text Then
            MessageBox.Show(gpsRM.GetString("psPWD_MISMATCH_MSG"), gpsRM.GetString("psGPB_CHG_PWD_TXT"), _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If
        'Submit the change request
        Dim sMsg() As String = {gs_COM_ID_PWUSERMAINT, "changepwd", "", txtChangePwdNew.Text, "", "", "", txtChangePwdCurrent.Text}

        oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sMsg)

    End Sub

    Private Sub btnChangePwdCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnChangePwdCancel.Click
        '********************************************************************************************
        'Description:  Password Change was canceled by user. Clear fields.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        txtChangePwdCurrent.Text = String.Empty
        txtChangePwdNew.Text = String.Empty
        txtChangePwdVerifyNew.Text = String.Empty

    End Sub

    Private Sub btnDelPriv_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelPriv.Click
        '********************************************************************************************
        'Description:  Delete the selected privilege, then reload the treeview
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sPrivilege As String = trvMaint.SelectedNode.Tag.ToString
        Dim sProcess As String = trvMaint.SelectedNode.Parent.Tag.ToString

        Call subDeletePrivilege(sProcess, sPrivilege)
        Call subLoadMaintItems()

    End Sub

    Private Sub btnDelProc_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelProc.Click
        '********************************************************************************************
        'Description:  Delete the selected process and all associated privileges, then reload the 
        '              treeview.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Call subDeleteProcess(trvMaint.SelectedNode.Tag.ToString)
        Call subLoadMaintItems()

    End Sub

    Private Sub btnEditPwdAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEditPwdAccept.Click
        '********************************************************************************************
        'Description:  Password Edit was entered by user.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/15/12  HGB     Pass username when requesting password change
        '********************************************************************************************

        If txtEditPwdNew.Text = txtEditPwdNewVerify.Text Then
            Dim nGroup As Integer
            Dim nLastIndex As Integer = mUsers.GetUpperBound(0)

            'Get selected group
            For Each oGroup As udsGroup In mGroups
                If cboUserGroup.SelectedItem.ToString = oGroup.Name Then
                    nGroup = oGroup.ID
                    Exit For
                End If
            Next 'oGroup

            If mEditUser.ID = mnNEW_MEMBER_ID Then
                'Add User
                Dim sMsg() As String = {moPassword.ProcessName, "adduser", "", txtEditPwdNew.Text, _
                                        mEditUser.Name, "", nGroup.ToString}

                oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sMsg)
            Else
                'Change User Password and/or Group if necessary.
                If mEditUser.GroupID <> nGroup Then Call subChangeGroup(nGroup)

                If txtEditPwdNew.TextLength > 0 Then
                    'HGB added username
                    Dim sMsg() As String = {moPassword.ProcessName, "changepwd", "", txtEditPwdNew.Text, _
                                            mEditUser.Name}

                    oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sMsg)
                End If
            End If 'mEditUser.ID = mnNEW_MEMBER_ID

            'update Change Log
            Call subLogChanges()

            'hide edit Group/Password groupbox
            gpbEditGroupPwd.Visible = False
            lstUsers.Enabled = True
            btnAddUser.Enabled = True
            btnChange.Enabled = True
            btnRemoveUser.Enabled = True

        Else
            MessageBox.Show(gpsRM.GetString("psPWD_MISMATCH_MSG"), gpsRM.GetString("psGPB_EDIT_GRP_PWD_TXT"), _
                                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

    End Sub

    Private Sub btnEditPwdCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEditPwdCancel.Click
        '********************************************************************************************
        'Description:  Password Edit was canceled by user. Clean up, then hide the Edit Group/
        '              Password group box.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bNewUser As Boolean = CancelNewUser()

        gpbEditGroupPwd.Visible = False
        lstUsers.Enabled = True
        btnAddUser.Enabled = True
        btnChange.Enabled = True
        btnRemoveUser.Enabled = True

    End Sub

    Private Sub btnGroup_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                          Handles btnGroupAllowed.Click, btnGroupNotAllowed.Click
        '********************************************************************************************
        'Description:  Change the state of the selected Privilege node depending on which button was
        '              clicked. If a Process is selected, change the state of all Privileges for that
        '              Process to the selected state.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/01/14  RJO/BD  Fixed problems with No node selected or Parent node selected
        '********************************************************************************************
        Dim oButton As Button = DirectCast(sender, Button)
        Dim oNode As TreeNode = trvGroupPrivileges.SelectedNode
        Dim bAllowed As Boolean = (oButton.Name = "btnGroupAllowed")

        If oNode.Parent Is Nothing Then
            For Each oSubNode As TreeNode In oNode.Nodes
                Call subSetPrivilege(trvGroupPrivileges, oSubNode, bAllowed)
            Next
            'rehighlight the parent node.
            With trvGroupPrivileges
                .BeginUpdate()
                .SelectedNode = oNode
                .Select()
                .EndUpdate()
            End With
        Else
            Call subSetPrivilege(trvUserPrivileges, oNode, bAllowed)
        End If

    End Sub

    Private Sub btnRemoveGroup_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRemoveGroup.Click
        '********************************************************************************************
        'Description:  Reassign all Users that belong to the selected group to the "Custom" group,
        '              then Delete the selected group.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim DS As DataSet = GetDataSet()
            Dim DR As DataRow
            Dim sChange As String = gpsRM.GetString("psDELETE_GROUP_MSG") & mEditGroup.Name

            'Reassign users that belong to deleted group to "Custom" group (0)
            For Each DR In DS.Tables("Users").Rows
                If CType(DR.Item("GroupID"), Integer) = mEditGroup.ID Then DR.Item("GroupID") = 0
            Next

            'DS.AcceptChanges() 'TODO - Does this have to be done here or is just once at the end OK

            'Delete the Privilege set associated with the deleted group
            Dim PDRs() As DataRow = DS.Tables("Group_Access_List").Select("GroupID = " & mEditGroup.ID.ToString)

            For nIndex As Integer = PDRs.GetUpperBound(0) To 0 Step -1
                PDRs(nIndex).Delete()
            Next

            'DS.AcceptChanges()

            'Delete the Group
            Dim GDRs() As DataRow = DS.Tables("Groups").Select("GroupID = " & mEditGroup.ID.ToString)

            'Should only be one
            GDRs(0).Delete()

            DS.AcceptChanges()
            DS.WriteXml(msXMLPath & "PWUser.xml")

            'Log Changes
            Call mPWCommon.AddChangeRecordToCollection(gsChangeLogArea, LoggedOnUser, _
                           mcolZones.ActiveZone.ZoneNumber, String.Empty, _
                           String.Empty, sChange, mcolZones.ActiveZone.Name)
            Call subLogChanges()

        Catch ex As Exception
            'Message to User, Log internal error
            mDebug.ShowErrorMessagebox(gpsRM.GetString("psDATA_LOAD_FAIL_MSG", DisplayCulture), _
                                       ex, msSCREEN_NAME, Status, MessageBoxButtons.OK)
        End Try

        Call subShowTabPage3()

    End Sub

    Private Sub btnRemoveUser_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRemoveUser.Click
        '********************************************************************************************
        'Description:  Instruct Password.NET to delete the selected user.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sMsg() As String = {moPassword.ProcessName, "deleteuser", "", "", mEditUser.Name}

        oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sMsg)

    End Sub

    Private Sub btnUser_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                              Handles btnUserAllowed.Click, btnUserNotAllowed.Click
        '********************************************************************************************
        'Description:  Change the state of the selected Privilege node depending on which button was
        '              clicked. If a Process is selected, change the state of all Privileges for that
        '              Process to the selected state.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/01/14  RJO/BD  Fixed problems with No node selected or Parent node selected
        '********************************************************************************************
        Dim oButton As Button = DirectCast(sender, Button)
        Dim oNode As TreeNode = trvUserPrivileges.SelectedNode
        Dim bAllowed As Boolean = (oButton.Name = "btnUserAllowed")

        If oNode.Parent Is Nothing Then 'RJO 08/01/14

            For Each oSubNode As TreeNode In oNode.Nodes

                Call subSetPrivilege(trvUserPrivileges, oSubNode, bAllowed)

            Next
            'rehighlight the parent node.
            With trvUserPrivileges
                .BeginUpdate()
                .SelectedNode = oNode
                .Select()
                .EndUpdate()
            End With
        Else
            Call subSetPrivilege(trvUserPrivileges, oNode, bAllowed)
        End If

    End Sub

    Private Sub cboEnableAutoLogout_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboEnableAutoLogout.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Auto Logout Enable Combo Changed 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case cboEnableAutoLogout.SelectedIndex
            Case 0 'Disabled
                mEditSetup.AutoLogoutEnable = False
                If mEditSetup.AutoLogoutEnable_Org Then EditsMade = True
            Case 1 'Enabled
                mEditSetup.AutoLogoutEnable = True
                If Not mEditSetup.AutoLogoutEnable_Org Then EditsMade = True
        End Select

    End Sub

    Private Sub cboZone_SelectionChangeCommitted1(ByVal sender As Object, ByVal e As System.EventArgs) _
                                        Handles cboZone.SelectionChangeCommitted
        '********************************************************************************************
        'Description:  Zone Combo Changed 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If cboZone.Text <> msOldZone Then
            ' so we don't go off half painted
            Me.Refresh()
            Application.DoEvents()
            subChangeZone()
        End If

    End Sub

    Private Sub ftbAutoLogout_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
                                          ftbAutoLogoutWarn.TextChanged, ftbAutoLogoutTime.TextChanged
        '********************************************************************************************
        'Description:  Auto Logout Setup Changed 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bChange As Boolean = True
        Dim oCtl As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)

        If Not mbEventBlocker Then
            Select Case oCtl.Name   'JZ 12072016 - Every field can be cleared.
                Case "ftbAutoLogoutWarn"
                    If Not ftbAutoLogoutWarn.Text = String.Empty Then
                        If CType(oCtl.Text, Integer) = mEditSetup.AutoLogoutWarn_Org Then
                            bChange = False
                        Else
                            mEditSetup.AutoLogoutWarn = CType(oCtl.Text, Integer)
                        End If
                    Else
                        bChange = False
                    End If
                Case "ftbAutoLogoutTime"
                    If Not ftbAutoLogoutTime.Text = String.Empty Then
                        If CType(oCtl.Text, Integer) = mEditSetup.AutoLogoutTime_Org Then
                            bChange = False
                        Else
                            mEditSetup.AutoLogoutTime = CType(oCtl.Text, Integer)
                        End If
                    Else
                        bChange = False
                    End If
            End Select

            If bChange Then
                oCtl.ForeColor = Color.Red
                oCtl.BackColor = Color.Yellow
                EditsMade = True
            Else
                oCtl.ForeColor = Color.Black
                oCtl.BackColor = Color.White
            End If
        End If 'Not mbEventBlocker

    End Sub

    Private Sub gpbChangePwd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles gpbChangePwd.Click
        '********************************************************************************************
        'Description: If nobody is logged in, let the user know
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If LoggedOnUser = String.Empty Then
            Dim Reply As Response = MessageBox.Show(gpsRM.GetString("psLOGIN_REQD_MSG"), _
                                                    gpsRM.GetString("psSCREENCAPTION"), _
                                                    MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation)

            If Reply = Windows.Forms.DialogResult.OK Then
                'Show Password Login and check for Administrate privilege
                Privilege = ePrivilege.Administrate
            End If

        End If

    End Sub

    Private Sub lstGroups_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstGroups.SelectedIndexChanged
        '********************************************************************************************
        'Description: The operator selected a User Group from the list. Load the selected Group's 
        '             Privileges in the tree view. Enable the Remove Group button.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bEnable As Boolean = (lstGroups.Items.Count > 0) And (lstGroups.SelectedIndex > -1)

        'check for unsaved edits.
        If EditsMade Then
            If Not bAskForSave() Then
                'reselect the previously selected user
                For nIndex As Integer = 0 To lstGroups.Items.Count - 1
                    If lstGroups.Items(nIndex).ToString = mEditGroup.Name Then
                        lstGroups.SelectedIndex = nIndex
                        Exit Sub
                    End If
                Next
            End If 'Not bAskForSave
        End If 'EditsMade


        If bEnable Then
            If lstGroups.SelectedItem IsNot Nothing Then
                'Can get in an odd situation here after a save
            Dim sGroupName As String = lstGroups.SelectedItem.ToString

            For Each oGroup As udsGroup In mGroups
                If oGroup.Name = sGroupName Then
                    mEditGroup = oGroup
                    Exit For
                End If
            Next 'oGroup

            If mEditGroup.ID > 0 Then
                Call subLoadGroupPrivilegeTree(mEditGroup.ID)
                mbPrintEnable = True
                Call subEnableControls(True)
            End If

            End If
        Else
            trvGroupPrivileges.Nodes.Clear()
            mbPrintEnable = False
            Call subEnableControls(True)
        End If

        btnRemoveGroup.Enabled = bEnable

    End Sub

    Private Sub lstUsers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstUsers.SelectedIndexChanged
        '********************************************************************************************
        'Description: The operator selected a user from the list. Load the selected User's Privileges
        '             in the tree view. Enable the Remove User and the Change Group/Password buttons.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bEnable As Boolean = (lstUsers.Items.Count > 0) And (lstUsers.SelectedIndex > -1)

        'check for unsaved edits.
        If EditsMade Then
            If Not bAskForSave() Then
                'reselect the previously selected user
                For nIndex As Integer = 0 To lstUsers.Items.Count - 1
                    If lstUsers.Items(nIndex).ToString = mEditUser.Name Then
                        lstUsers.SelectedIndex = nIndex
                        Exit Sub
                    End If
                Next
            End If 'Not bAskForSave
        End If 'EditsMade

        If bEnable Then
            Dim sUserName As String = lstUsers.SelectedItem.ToString

            For Each oUser As udsUser In mUsers
                If oUser.Name = sUserName Then
                    mEditUser = oUser
                    Exit For
                End If
            Next 'oUser

            If mEditUser.ID > 0 Then
                Call subLoadUserPrivilegeTree(mEditUser.ID)
                mbPrintEnable = True
                Call subEnableControls(True)
            End If
        Else
            trvUserPrivileges.Nodes.Clear()
            mbPrintEnable = False
            Call subEnableControls(True)
        End If

        btnRemoveUser.Enabled = bEnable
        btnChange.Enabled = bEnable

    End Sub

    Private Sub subChangePwd_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
                                                                 txtChangePwdCurrent.TextChanged, _
                                                                 txtChangePwdNew.TextChanged, _
                                                                 txtChangePwdVerifyNew.TextChanged
        '********************************************************************************************
        'Description:  Enable the Accept button when all fields have been filled in.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bEnable As Boolean = True

        If txtChangePwdCurrent.Text.Length = 0 Then bEnable = False
        If txtChangePwdNew.Text.Length = 0 Then bEnable = False
        If txtChangePwdNew.Text.Length <> txtChangePwdVerifyNew.Text.Length Then bEnable = False

        btnChangePwdAccept.Enabled = bEnable

    End Sub

    Private Sub subEditPwdN_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtEditPwdNewVerify.TextChanged, txtEditPwdNew.TextChanged, cboUserGroup.SelectedIndexChanged

        '********************************************************************************************
        'Description:  Enable the Accept button when all fields have been filled in or new user group
        '              is selected.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If Not mbEventBlocker Then
            Dim bEnable As Boolean = True

            If txtEditPwdNew.Text.Length <> txtEditPwdNewVerify.Text.Length Then bEnable = False

            btnEditPwdAccept.Enabled = bEnable
        End If

    End Sub

    Private Sub tabMain_Selected(ByVal sender As Object, _
                ByVal e As System.Windows.Forms.TabControlEventArgs) Handles tabMain.Selected
        '********************************************************************************************
        'Description:  a tab page was just selected
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        mbPrintEnable = False

        Select Case msCurrentTabName
            Case "TabPage1"
                'Add any special cleanup code here
            Case "TabPage2"
                'Add any special cleanup code here
                lstUsers.SelectedIndex = -1
                mEditUser.Name = String.Empty
                mEditUser.ID = 0
                mEditUser.GroupID = 0
            Case "TabPage3"
                'Add any special cleanup code here
            Case "TabPage4"
                'Add any special cleanup code here
        End Select

        msCurrentTabName = e.TabPage.Name
        Call subShowNewPage(True)

    End Sub

    Private Sub tabMain_Selecting(ByVal sender As Object, _
                ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles tabMain.Selecting
        '********************************************************************************************
        'Description:  A tab page is being selected. Make sure there are no unsaved edits on the
        '              current tab before switching to the selected tab.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If EditsMade And bCheckEditsMade() Then
            e.Cancel = (bAskForSave() = False)
        Else
            EditsMade = False
        End If

    End Sub

    Private Sub tabPage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabMain.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Event handler to show the Login form if the user clicks on a TabPage
        '              but doesn't have the Administrate privilege.
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If Not mbEventBlocker Then
            If (msCurrentTabName = "TabPage1") Or (Privilege = ePrivilege.Administrate) Then
                Exit Sub
            Else
                Dim Reply As Response = MessageBox.Show(gpsRM.GetString("psINSUF_PRIV_MSG"), _
                                        gpsRM.GetString("psSCREENCAPTION"), _
                                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
        End If 'Not mbEventBlocker

    End Sub

    Private Sub trvMaint_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles trvMaint.AfterSelect
        '********************************************************************************************
        'Description:  Enable Process/Privilege Maintenance buttons based on selected node.
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If trvMaint.SelectedNode.Parent Is Nothing Then
            btnDelPriv.Enabled = False
            btnDelProc.Enabled = True
        Else
            btnDelPriv.Enabled = True
            btnDelProc.Enabled = False
        End If

    End Sub

    Private Sub trvUserPrivileges_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles trvUserPrivileges.AfterSelect
        '********************************************************************************************
        'Description:  Enable Process/Privilege  buttons based on selected node.
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '08/01/14   TJO/BD  Enable allow not allow buttons after privilage selected only
        '********************************************************************************************

        btnUserAllowed.Enabled = True
        btnUserNotAllowed.Enabled = True

    End Sub

    Private Sub trvGroupPrivileges_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles trvGroupPrivileges.AfterSelect
        '********************************************************************************************
        'Description:  Enable Process/Privilege  buttons based on selected node.
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '08/01/14   TJO/BD  Enable allow not allow buttons after privilage selected only
        '********************************************************************************************

        btnGroupAllowed.Enabled = True
        btnGroupNotAllowed.Enabled = True
    End Sub

    Private Sub trvPrivileges_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
                                          trvUserPrivileges.DoubleClick, trvGroupPrivileges.DoubleClick
        '********************************************************************************************
        'Description:  If the user double-clicked a Privilege node, toggle its value
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/13/12  MSW     trvPrivileges_DoubleClick-Add another  not nothing check
        '********************************************************************************************
        Dim oTrv As TreeView = DirectCast(sender, TreeView)
        Dim oNode As TreeNode = oTrv.SelectedNode

        ' 12/13/12  MSW     trvPrivileges_DoubleClick-Add another  not nothing check
        If oNode Is Nothing Then Exit Sub

        If oNode.Parent Is Nothing Then Exit Sub

        Call subTogglePrivilege(oTrv, oNode)

    End Sub

#End Region

#Region " Password Events "

    Private Sub moPassword_LogIn() Handles moPassword.LogIn
        '********************************************************************************************
        'Description: User logged in  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/12/09  MSW     add delegate declarations to handle cross-thread calls
        '********************************************************************************************
        'This can get called by the password thread
        If Me.stsStatus.InvokeRequired Then
            Dim dLogin As New LoginCallBack(AddressOf moPassword_LogIn)
            Me.BeginInvoke(dLogin)
        Else
            Me.LoggedOnUser = moPassword.UserName
            Status = Me.LoggedOnUser & " " & gcsRM.GetString("csLOGGED_IN")
            Privilege = ePrivilege.Administrate
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
        End If

    End Sub

    Private Sub moPassword_LogOut() Handles moPassword.Logout
        '********************************************************************************************
        'Description: User logged out 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/12/09  MSW     add delegate declarations to handle cross-thread calls
        '********************************************************************************************
        'This can get called by the password thread
        If Me.stsStatus.InvokeRequired Then
            Dim dLogin As New LogOutCallBack(AddressOf moPassword_LogOut)
            Me.BeginInvoke(dLogin)
        Else
            Status = Me.LoggedOnUser & " " & gcsRM.GetString("csLOGGED_OUT")
            Me.LoggedOnUser = String.Empty
            Me.Privilege = ePrivilege.None
            'Show the Change Password Tab
            tabMain.SelectedTab = TabPage1
            EditsMade = False
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
        End If

    End Sub

    Private Sub moPassword_Result(ByVal Action As String, ByVal ReturnValue As String) Handles moPassword.Result
        '********************************************************************************************
        'Description: Result returned by User Maintenance operation
        '
        'Parameters: Action - operation name, ReturnValue - returned status/data
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bShowMsg As Boolean = True
        Dim bShowTabPage2 As Boolean = True
        Dim oIcon As MessageBoxIcon = MessageBoxIcon.Error
        Dim sTitle As String = String.Empty
        Dim sMsg As String = String.Empty

        With gpsRM
            Select Case Action
                Case "adduser"
                    sTitle = .GetString("psGPB_EDIT_USER_TXT")
                    Select Case ReturnValue
                        Case "emptystring"
                            sMsg = .GetString("psEMPTY_STRING_MSG")
                        Case "failed"
                            sMsg = .GetString("psOP_FAIL_MSG")
                        Case "namena"
                            sMsg = .GetString("psUSERNAME_NA_MSG")
                        Case "pwdna"
                            sMsg = .GetString("psPWD_NA_MSG")
                        Case "success"
                            Dim sChange As String = gpsRM.GetString("psADD_USER_MSG") & mEditUser.Name

                            sMsg = .GetString("psOP_SUCCESS_MSG")
                            oIcon = MessageBoxIcon.Information
                            'update Change Log
                            Call mPWCommon.AddChangeRecordToCollection(gsChangeLogArea, LoggedOnUser, _
                                                                       mcolZones.ActiveZone.ZoneNumber, String.Empty, _
                                                                       String.Empty, sChange, mcolZones.ActiveZone.Name)
                            Call subLogChanges()
                        Case Else
                            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: moPassword_Result", _
                                                   "Action [adduser] - Unrecognized result string [" & ReturnValue & "].")
                    End Select
                Case "changepwd"
                    sTitle = .GetString("psGPB_CHG_PWD_TXT")
                    Select Case ReturnValue
                        Case "failed"
                            sMsg = .GetString("psOP_FAIL_MSG")
                        Case "invaliduser"
                            sMsg = .GetString("psPWD_INVALID_MSG")
                        Case "pwdna"
                            sMsg = .GetString("psPWD_NA_MSG")
                        Case "success"
                            Dim sChange As String = gpsRM.GetString("psCHG_USER_PWD_MSG")

                            sMsg = .GetString("psOP_SUCCESS_MSG")
                            oIcon = MessageBoxIcon.Information

                            If msCurrentTabName = "TabPage1" Then
                                sChange = sChange & LoggedOnUser
                                bShowTabPage2 = False
                                'Clear user entries
                                Call subShowTabPage1()
                            Else 'msCurrentTabName = "TabPage2"
                                sChange = sChange & mEditUser.Name
                            End If
                            'update Change Log
                            Call mPWCommon.AddChangeRecordToCollection(gsChangeLogArea, LoggedOnUser, _
                                                                       mcolZones.ActiveZone.ZoneNumber, String.Empty, _
                                                                       String.Empty, sChange, mcolZones.ActiveZone.Name)
                            Call subLogChanges()
                        Case Else
                            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: moPassword_Result", _
                                                   "Action [changepwd] - Unrecognized result string [" & ReturnValue & "].")
                    End Select
                Case "deleteuser"
                    sTitle = .GetString("psGPB_EDIT_USER_TXT")
                    Select Case ReturnValue
                        Case "failed"
                            sMsg = .GetString("psOP_FAIL_MSG")
                        Case "success"
                            Dim sChange As String = gpsRM.GetString("psDELETE_USER_MSG") & mEditUser.Name

                            sMsg = .GetString("psOP_SUCCESS_MSG")
                            oIcon = MessageBoxIcon.Information
                            'update Change Log
                            Call mPWCommon.AddChangeRecordToCollection(gsChangeLogArea, LoggedOnUser, _
                                                                       mcolZones.ActiveZone.ZoneNumber, String.Empty, _
                                                                       String.Empty, sChange, mcolZones.ActiveZone.Name)
                            Call subLogChanges()
                        Case "usercurrent"
                            sMsg = .GetString("psUSER_CURR_DEL_FAIL_MSG")
                        Case "userfanuc"
                            sMsg = .GetString("psUSER_FANUC_DEL_FAIL_MSG")
                        Case Else
                            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: moPassword_Result", _
                                                   "Action [deleteuser] - Unrecognized result string [" & ReturnValue & "].")
                    End Select
                Case "usernames"
                    bShowMsg = False
                    bShowTabPage2 = False
                    Call subLoadUsersList(ReturnValue)
                    If mEditUser.Name <> String.Empty Then
                        lstUsers.SelectedIndex = lstUsers.Items.IndexOf(mEditUser.Name)
                        mEditUser.Name = String.Empty
                    End If

                Case Else
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: moPassword_Result", _
                                           "Unrecognized action string [" & Action & "].")
            End Select 'Case Action
        End With 'gpsRM

        If bShowMsg Then
            MessageBox.Show(sMsg, sTitle, MessageBoxButtons.OK, oIcon)
        End If

        If bShowTabPage2 Then Call subShowTabPage2()

    End Sub

#End Region

#Region " Main Menu Button Events "

    Private Sub btnFunction_DropDownOpening(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFunction.DropDownOpening
        '********************************************************************************************
        'Description:  this was needed to enable the menus for some reason
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim cachePrivilege As ePrivilege

        If LoggedOnUser <> String.Empty Then

            If moPassword.Privilege = ePrivilege.None Then
                cachePrivilege = ePrivilege.None
            Else
                If moPassword.ActionAllowed Then
                    cachePrivilege = moPassword.Privilege
                Else
                    cachePrivilege = ePrivilege.None
                End If
            End If

            Privilege = ePrivilege.Remote
            If Privilege = ePrivilege.Remote Then
                If mbRemoteZone = False Then
                    mnuRemote.Enabled = True
                End If
            Else
                mnuRemote.Enabled = False
            End If
            Privilege = cachePrivilege

        Else
            Privilege = ePrivilege.None
        End If

    End Sub

    Private Sub mnuAllChanges_Click(ByVal sender As Object, _
                                        ByVal e As System.EventArgs) Handles mnuAllChanges.Click
        '********************************************************************************************
        'Description:  show changes for last 7 days
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)

        subShowChangeLog(o.Owner.Items.IndexOf(o))

    End Sub

    Private Sub mnuLast7_Click(ByVal sender As Object, _
                                                ByVal e As System.EventArgs) Handles mnuLast7.Click
        '********************************************************************************************
        'Description:  show changes for last 7 days
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)

        subShowChangeLog(o.Owner.Items.IndexOf(o))

    End Sub

    Private Sub mnuLast24_Click(ByVal sender As Object, _
                                            ByVal e As System.EventArgs) Handles mnuLast24.Click
        '********************************************************************************************
        'Description:  show changes for last 24 hours
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)

        subShowChangeLog(o.Owner.Items.IndexOf(o))

    End Sub

    Private Sub mnuLocal_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuLocal.Click
        '********************************************************************************************
        'Description:  Select a local zone
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        mbRemoteZone = False
        moPassword = Nothing

        subInitializeForm()

    End Sub

    Private Sub mnuPageSetup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPageSetup.Click
        '********************************************************************************************
        'Description:  show page setup dialog 
        '
        'Parameters: none
        'Returns:    Print settings to use in printing
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If DataLoaded Then
            If bPrintdoc(False) Then
                mPrintHtml.subShowPageSetup()
            End If
        End If

    End Sub

    Private Sub mnuPrint_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                        Handles mnuPrint.Click
        '********************************************************************************************
        'Description:  call print routine for current tab only
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If DataLoaded Then
            Call bPrintdoc(True)
        End If

    End Sub

    Private Sub mnuPrintFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintFile.Click
        '********************************************************************************************
        'Description:  print the current tab data to a file
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If DataLoaded Then
            If bPrintdoc(False) Then
                mPrintHtml.subSaveAs()
            End If
        End If

    End Sub

    Private Sub mnuPrintOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintOptions.Click
        '********************************************************************************************
        'Description:  offer options for printout table setup.
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        mPrintHtml.subShowOptions()

    End Sub

    Private Sub mnuPrintPreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintPreview.Click
        '********************************************************************************************
        'Description:  Show the print preview for current tab only
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If DataLoaded Then
            If bPrintdoc(False) Then
                mPrintHtml.subShowPrintPreview()
            End If
        End If

    End Sub

    Private Sub mnuRemote_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                    Handles mnuRemote.Click
        '********************************************************************************************
        'Description:  Select a remote zone
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bTmp As Boolean = mbRemoteZone

        mbEventBlocker = True

        If SetRemoteServer() Then
            mbRemoteZone = True
            moPassword = Nothing

            subInitializeForm()
        Else
            mbRemoteZone = bTmp
        End If 'SetRemoteServer()

        mbEventBlocker = False

    End Sub

    Private Sub tlsMain_ItemClicked(ByVal sender As Object, _
            ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tlsMain.ItemClicked
        '********************************************************************************************
        'Description: Tool Bar button clicked - double check privilege here incase it expired
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case e.ClickedItem.Name ' case sensitive
            Case "btnClose"
                'Check to see if we need to save is performed in  bAskForSave
                Me.Close()

            Case "btnStatus"
                lstStatus.Visible = Not lstStatus.Visible

            Case "btnSave"
                'privilege check done in subroutine
                subSaveData()

            Case "btnPrint"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                bPrintdoc(True)

            Case "btnUndo"
                Select Case Privilege
                    Case ePrivilege.None
                        'logout occured while screen open
                        subEnableControls(False)
                    Case Else
                        subUndoData()
                End Select

            Case "btnChangeLog"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                subShowChangeLog(CType(eChangeMnu.Hours24, Integer))

        End Select

    End Sub

#End Region

#Region " Log In, Log Out Events "

    Private Sub mnuLogIn_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                  Handles mnuLogin.Click
        '********************************************************************************************
        'Description:  someone clicked on lock panel
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If LoggedOnUser = String.Empty Then
            If Privilege = ePrivilege.None Then
                Privilege = ePrivilege.Administrate
            End If
        End If

    End Sub

    Private Sub mnuLogOut_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                    Handles mnuLogOut.Click
        '********************************************************************************************
        'Description:  someone clicked on lock panel
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If LoggedOnUser <> String.Empty Then
            moPassword.LogUserOut()
        End If

    End Sub

#End Region

#Region " Debug Stuff "

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        'For debug
        Call subDeletePrivilege("scheduler", "myspecial")

    End Sub

    Private Sub Button2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button2.Click
        'For debug
        Call subDeleteProcess(TextBox1.Text)
    End Sub

#End Region

End Class