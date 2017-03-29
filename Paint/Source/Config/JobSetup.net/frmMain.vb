' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2007 - 2008
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: frmMain
'
' Description: PLC Style Setup Screen
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Written by RickO, Ruined by Speedy
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
' Modification history:
'
'    Date       By      Reason                                                         Version
'    10/23/09   BTK     Added code so we can tell config screens where the
'                       first robot starts in the PLC, generally zero or one.
'    11/12/09   MSW     Make duplicates a warning so it doesn't lock up the program
'    11/18/09   MSW     subTextboxKeypressHandler, subFtbAddHandlers - Offer a login 
'                       if they try to enter data without a login        
'    02/22/10   BTK     bValidateIntrusionData, We can't use one job space for exit 
'                       start muting upper limit.
'    06/30/10   MSW     handle page up, page down, home, end
'    09/23/10   MSW     Convert to html print, remove microsoft word references
'    09/14/11   MSW     Assemble a standard version of everything                       4.1.0.0
'    09/27/11   MSW     Standalone changes round 1 - HGB SA paintshop computer changes	4.1.0.1
'    10/05/11   AM      Hide Options tab if MaxOptions = 0                              4.1.0.2
'    12/02/11   MSW     Option and style registers, DMON placeholder reference          4.1.1.0
'    12/13/11   RJO     Added support for Sealer combined Style/Option                  4.1.1.1
'    01/24/12   MSW     Placeholder include updates                                     4.1.1.2
'    02/16/12   MSW     Print/Import/Export updates, Force 32 bit build                 4.1.1.3
'    03/21/12   RJO     Modified to use .NET Password and IPC                           4.1.2.0
'    04/04/12   MSW     Handle plc link lengths larger than required, only fault if     4.1.3.0
'                       it's shorter. Move most of the tables from SQL to XML.
'    04/27/12   MSW     Fix Fkey handling                                               4.1.3.1
'    09/13/12   RJO     Bug fixes to bSaveIntrusionToPLC. nIdx loops were using (nItem) 4.1.3.2
'                       for pointer inside loops and change logging not working. Added 
'                       code to bSaveStylesToSQLDB that log changes to 
'                       EntranceMuteStartCount, ExitMuteStartCount, and MuteLength. Bug 
'                       fix to bLoadFromGUI for TabPage5 (PEC Muting).
'    10/04/12   RJO     Bug fix to change log reporting for Robots Required in          4.1.3.3
'                       bSaveStylesToSQLDB and bSaveOptionsToSQLDB.
'    10/23/12   RJO     Added StartupModule to project to prevent multiple instances    4.1.3.4
'    01/16/13   MSW     Added (Unused) Style  parameter for compatibility with frmCopy  4.1.3.5
'    04/16/13   MSW     Add Canadian language files                                     4.1.5.0
'                       Standalone Changelog
'    07/08/13   MSW     Move the style ID tables to XML                                 4.1.5.1
'    07/09/13   MSW     Update names and logos                                          4.1.5.2
'    07/29/13   HGB     In subUnloadPanel there was no code to handle the two coats     4.1.5.3
'                       check box. On a zone change an exception occurred since the 
'                       default type was a label.
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up  4.01.05.04
'    09/30/13   MSW     Save screenshots as jpegs, PLC DLL                              4.01.06.00
'    10/25/13   BTK     Added Tricoat by style changes.
'    10/29/13   BTK     bSaveStylesToSQLDB was using the wrong labels for               4.01.06.01
'                       muting change log.
'    01/06/14   MSW     Support stop-station style-ID                                   4.01.06.02
'                       Deal with non-consecutive robot numbers
'    02/03/14   RJO     Change gsChangeLogArea from "Job_Setup" to "Job Setup" for      4.01.06.03
'                       consistency with Change Log Reports screen. In subShowChangelog, 
'                       changed sScreenName argument to mPWCommon.subShowChangeLog from 
'                       msSCREEN_NAME to gsChangeLogArea.
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call                4.01.07.00
'    07/23/14   RJO     Big Changes to support more degrade modes than robots           4.01.07.01
'************************************************************************************************


'******** Unfinished Business   *****************************************************************
'TODO - Make this work (per the Speedy model) for multiple zones
'TODO - Clean up redundant code
'TODO - msRestoreString doesn't appear to be implemented.
'******** End Unfinished Business   *************************************************************
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Configuration.ConfigurationSettings
Imports Response = System.Windows.Forms.DialogResult
Imports Connstat = FRRobotNeighborhood.FRERNConnectionStatusConstants
Imports System.Xml
Imports System.Xml.XPath
Imports clsPLCComm = PLCComm.clsPLCComm

Friend Class frmMain

#Region " Declares "
    '******** Form Constants   **********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Public Const msSCREEN_NAME As String = "Job_Setup"   ' <-- For password area change log etc.
    Public Const msSCREEN_DUMP_NAME As String = "Config_JobSetup"
    Private Const mnMAININX As Integer = 100
    Private Const mnSCREENINX As Integer = 101
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    Friend Const mbUSEROBOTS As Boolean = False
    Private Const mnROWSPACE As Integer = 30 ' interval for rows of textboxes etc
    Private Const mnCOLUMNSPACE As Integer = 40 'interval for columns

    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    Friend gsChangeLogArea As String = "Job Setup" ' 02/03/14  RJO Changed from msSCREEN_NAME
    Private msCulture As String = "en-US" 'Default to English
    Private msOldZone As String = String.Empty
    Private msOldRobot As String = String.Empty
    Private msOldParam As String = String.Empty
    Private msOldController As String = String.Empty
    Friend mcolZones As clsZones = Nothing
    Friend mcolStyles As clsSysStyles = Nothing
    Friend mcolOptions As clsSysOptions = Nothing
    Friend mcolStyleID As clsSysStylesID = Nothing
    Private msRobotClbNames As String() = Nothing
    Private WithEvents mcolControllers As clsControllers = Nothing
    Public WithEvents mcolArms As clsArms = Nothing
    Private mHiddenTabs As TabControl = Nothing
    Private mnRepairLinkLength As Integer()
    Private mnStyleRbtRqdLinkLength As Integer()
    Private WithEvents moPLC As New clsPLCComm

    Private mcolStyleTextboxes As Collection(Of FocusedTextBox.FocusedTextBox) = _
                                        New Collection(Of FocusedTextBox.FocusedTextBox)
    Private mcolOptionTextboxes As Collection(Of FocusedTextBox.FocusedTextBox) = _
                                        New Collection(Of FocusedTextBox.FocusedTextBox)
    Private mcolRepairTextboxes As Collection(Of FocusedTextBox.FocusedTextBox) = _
                                        New Collection(Of FocusedTextBox.FocusedTextBox)
    Private mcolDegradeTextboxes As Collection(Of FocusedTextBox.FocusedTextBox) = _
                                        New Collection(Of FocusedTextBox.FocusedTextBox)
    Private mcolIntrusionTextboxes As Collection(Of FocusedTextBox.FocusedTextBox) = _
                                        New Collection(Of FocusedTextBox.FocusedTextBox)
    Private mcolStyleCLBoxes As Collection(Of CheckedListBox) = _
                                        New Collection(Of CheckedListBox)
    Private mcolOptionCLBoxes As Collection(Of CheckedListBox) = _
                                        New Collection(Of CheckedListBox)
    Private mcolRepairCLBoxes As Collection(Of CheckedListBox) = _
                                        New Collection(Of CheckedListBox)
    'jbw 03/03/11 Checkbox collection!!!
    Private mcolStyleCheckBoxes As Collection(Of CheckBox) = New Collection(Of CheckBox)

    'BTK
    Private mcolStyleIDPositionTextboxes As Collection(Of FocusedTextBox.FocusedTextBox) = _
                                        New Collection(Of FocusedTextBox.FocusedTextBox)

    Private mcolStyleIDPEButtons As Collection(Of Button) = _
                                New Collection(Of Button)

    '10/25/13 BTK Tricoat By Style
    Private mcolStyleTricoatEnbCLBoxes As Collection(Of CheckedListBox) = _
                                        New Collection(Of CheckedListBox)

    Private mbScreenLoaded As Boolean
    Private mbReLoad As Boolean
    Private mnStyleIDIndex As Integer = -1
    Private mbStylesTabInitialized As Boolean
    Private mbOptionsTabInitialized As Boolean
    Private mbRepairsTabInitialized As Boolean
    Private mbDegradeTabInitialized As Boolean
    Private mbIntrusionTabInitialized As Boolean
    'BTK Style ID
    Private mbStyleIDTabInitialized As Boolean
    '10/25/13 BTK Tricoat By Style
    Private mbStyleTricoatEnbTabInitialized As Boolean
    Private mnDataRows As Integer
    Private mnNumberofRobots As Integer
    Private mbEventBlocker As Boolean
    Private mbRemoteZone As Boolean
    Private msRestoreString As String = String.Empty
    Private msCurrentTabName As String = "TabPage1" ' always start on first page
    Private mbPLCFail As Boolean
    Private mnDogSpace As Integer
    Private mb32Bit As Boolean
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private mbEditsMade As Boolean
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean
    Private mnProgress As Integer
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    Private mPrintHtml As clsPrintHtml
    '******** End Property Variables    *************************************************************

    '******** This is the old pw3 password object interop  *****************************************
    'Friend WithEvents moPassword As PWPassword.PasswordObject 'RJO 03/21/12
    'Friend moPrivilege As PWPassword.cPrivilegeObject 'RJO 03/21/12
    '******** This is the old pw3 password object interop  *****************************************
    Friend WithEvents moPassword As clsPWUser 'RJO 03/21/12
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm  'RJO 03/21/12

    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    Delegate Sub NewMessage_Callback(ByVal Schema As String, ByVal DS As DataSet) 'RJO 03/21/12
    Delegate Sub ControllerStatusChange(ByVal Controller As clsController)

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


            'If mPassword.CheckPassword(msSCREEN_NAME, Value, moPassword, moPrivilege) Then 'RJO 03/21/12
            If moPassword.CheckPassword(Value) Then
                'passed
                mPrivilegeGranted = mPrivilegeRequested
                'if privilege changed may have to enable controls
                subEnableControls(True)
                If Privilege = ePrivilege.Edit Then
                    mScreenSetup.LockAllTextBoxes(mcolStyleTextboxes, False)
                    'jbw 03/03/11 checkbox stuff
                    mScreenSetup.LockAllCheckBoxes(mcolStyleCheckBoxes, True)
                End If
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
            Else
                'denied
                'hack me
                If moPassword.UserName <> String.Empty Then _
                        mPrivilegeRequested = mPrivilegeGranted
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
    Friend Property Status(Optional ByVal StatusStrip As Boolean = False) As String
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
                If StatusStrip = False Then
                    Dim sTmp As String = Value & vbTab & Format(Now, "Long time")
                    lstStatus.Items.Add(sTmp)
                End If
                lblStatus.Text = Strings.Replace(Value, vbTab, "  ")
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Property: Status", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
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
        Dim lRet As DialogResult
        Dim Culture As System.Globalization.CultureInfo = DisplayCulture

        lRet = MessageBox.Show(gcsRM.GetString("csSAVEMSG1", Culture) & vbCrLf & _
                            gcsRM.GetString("csSAVEMSG", Culture), _
                            gcsRM.GetString("csSAVE_CHANGES", Culture), _
                            MessageBoxButtons.YesNoCancel, _
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

        Select Case lRet
            Case Response.Yes 'Response.Yes
                subSaveData()
                Return True
            Case Response.Cancel
                'false aborts closing form , changing zone etc
                Status = gcsRM.GetString("csSAVE_CANCEL", Culture)
                Return False
            Case Else
                Return True
        End Select

    End Function


    Private Sub btnPhotoeyeAll_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

        If Privilege = ePrivilege.None Then
            Privilege = ePrivilege.Edit
            Exit Sub
        End If

        Dim sName As String = String.Empty
        Dim oStyleID As clsSysStyleID = Nothing
        Dim nItem As Integer = 0
        Dim nPhotoeyeIndex As Integer
        Dim nIndex As Integer

        If mnStyleIDIndex > -1 Then
            nPhotoeyeIndex = CInt(DirectCast(sender, Button).Tag)
            oStyleID = mcolStyleID.Item((mcolStyleID.NumberOfConveyorSnapshots * mnStyleIDIndex) + ((nPhotoeyeIndex - 1) \ mcolStyleID.NumberOfPhotoeyes))

            ' nPhotoeyeIndex = nItem + ((nIndex - 1) * mcolStyleID.NumberOfPhotoeyes)

            nIndex = nPhotoeyeIndex - (((nPhotoeyeIndex - 1) \ mcolStyleID.NumberOfPhotoeyes) * mcolStyleID.NumberOfPhotoeyes) - 1
            If DataLoaded And EditsMade = False Then EditsMade = True
            DirectCast(sender, Button).FlatAppearance.BorderColor = Color.Red
            Select Case DirectCast(sender, Button).BackColor
                Case Color.GreenYellow
                    'Turn Yellow
                    DirectCast(sender, Button).BackColor = Color.Yellow
                    oStyleID.PhotoeyePattern.Value = SetBit(oStyleID.PhotoeyePattern.Value, nIndex)
                    oStyleID.PhotoeyeIgnore.Value = ReSetBit(oStyleID.PhotoeyeIgnore.Value, nIndex)
                Case Color.Yellow
                    'For sealer, not using ignore
                    If mcolStyleID.NumberOfConveyorSnapshots = 0 Then
                        'Turn Green
                        DirectCast(sender, Button).BackColor = Color.GreenYellow
                        oStyleID.PhotoeyePattern.Value = ReSetBit(oStyleID.PhotoeyePattern.Value, nIndex)
                        oStyleID.PhotoeyeIgnore.Value = ReSetBit(oStyleID.PhotoeyeIgnore.Value, nIndex)
                    Else
                        'Turn Gray
                        DirectCast(sender, Button).BackColor = Color.Gray
                        oStyleID.PhotoeyeIgnore.Value = SetBit(oStyleID.PhotoeyeIgnore.Value, nIndex)
                        oStyleID.PhotoeyePattern.Value = ReSetBit(oStyleID.PhotoeyePattern.Value, nIndex)
                    End If
                Case Color.Gray
                    'Turn Green
                    DirectCast(sender, Button).BackColor = Color.GreenYellow
                    oStyleID.PhotoeyePattern.Value = ReSetBit(oStyleID.PhotoeyePattern.Value, nIndex)
                    oStyleID.PhotoeyeIgnore.Value = ReSetBit(oStyleID.PhotoeyeIgnore.Value, nIndex)
            End Select
        End If
    End Sub


    Private Function bLoadDegradeFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the degrade numbers from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************

        Dim sPLCData() As String
        Dim sTag As String = String.Empty
        Dim oZone As clsZone = mcolZones.ActiveZone
        Dim nPtr As Integer
        Dim nStyle As Integer
        Dim nUbound As Integer = mcolControllers.Count

        Try

            DataLoaded = False

            If oZone.UseSplitShell Then
                'numbers by arm not controller
                nUbound = mcolArms.Count
            End If

            For nCont As Integer = 1 To nUbound
                If oZone.UseSplitShell Then
                    'by arm
                    sTag = oZone.PLCTagPrefix & _
                                    mcolArms(nCont - 1).PLCTagPrefix & "DegradeNumbers"
                Else
                    ' use arm 1 on controller3
                    sTag = oZone.PLCTagPrefix & _
                                    mcolControllers(nCont - 1).Arms(0).PLCTagPrefix & "DegradeNumbers"
                End If

                moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
                sPLCData = moPLC.PLCData
                If sPLCData Is Nothing Then Return False

                'for now data is set up in 100 word links. 10 words per style for 20 styles
                '2 100 word links for each controller
                nPtr = 0
                For nStyle = 0 To 9
                    For nRDown As Integer = 1 To mcolStyles.Item(nStyle).PlantSpecific.MaxDegrades
                        mcolStyles.Item(nStyle).PlantSpecific.DegradeNumber(nCont, nRDown).Value = _
                                                                    CType(sPLCData(nPtr), Integer)
                        nPtr += 1
                    Next
                Next 'nstyle



                'next 100 word link
                If oZone.UseSplitShell Then
                    'by arm
                    sTag = oZone.PLCTagPrefix & _
                                    mcolArms(nCont - 1).PLCTagPrefix & "DegradeNumbers2"
                Else
                    ' use arm 1 on controller
                    sTag = oZone.PLCTagPrefix & _
                                    mcolControllers(nCont - 1).Arms(0).PLCTagPrefix & "DegradeNumbers2"
                End If

                moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
                sPLCData = moPLC.PLCData
                If sPLCData Is Nothing Then Return False

                'for now data is set up in 100 word links. 10 words per style for 20 styles
                '2 100 word links for each controller
                nPtr = 0
                For nStyle = 10 To 19
                    For nRDown As Integer = 1 To mcolStyles.Item(nStyle).PlantSpecific.MaxDegrades
                        mcolStyles.Item(nStyle).PlantSpecific.DegradeNumber(nCont, nRDown).Value = _
                                                                    CType(sPLCData(nPtr), Integer)
                        nPtr += 1
                    Next
                Next 'nstyle

                Application.DoEvents()

            Next

            DataLoaded = True
            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bLoadDegradeFromGUI() As Boolean
        '********************************************************************************************
        'Description:  Load style dropdown
        '
        'Parameters: none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            If Not mbDegradeTabInitialized Then
                mcolStyles.LoadStyleBoxFromCollection(cboPlantStyle, , , mcolZones.ActiveZone.DegradeStyleRbtsReq, mcolZones.ActiveZone.NumberOfDegrades)
                mPWRobotCommon.LoadRobotBoxFromCollection(cboController, mcolControllers, False)
            End If

            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try
    End Function
    Private Function bLoadStyleIDFromGUI() As Boolean
        '********************************************************************************************
        'Description:  Load style dropdown
        '
        'Parameters: none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            Return bLoadStyleIDCollectionFromDB()


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try
    End Function
    Private Function bLoadIntrusionFromGUI() As Boolean
        '********************************************************************************************
        'Description: 
        '
        'Parameters: none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try


            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try
    End Function
    Private Function bLoadIntrusionFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the Intrusion counts from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Dim nItem As Integer
        Dim sPLCData() As String
        Dim sTag As String = String.Empty
        Dim oZone As clsZone = mcolZones.ActiveZone
        Dim nUb As Integer = oZone.MaxStyles - 1


        Try
            DataLoaded = False

            'dog space
            sTag = oZone.PLCTagPrefix & "DogSpacing"
            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
            sPLCData = moPLC.PLCData
            If sPLCData Is Nothing Then Return False

            'mult by 100 in plc
            'BTK 03/01/10 need more then one job space.
            mnDogSpace = CInt((CInt(sPLCData(0)) / 100) * 4)

            'Read Mute Len data from PLC
            sTag = oZone.PLCTagPrefix & "PECMuteLength"
            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
            sPLCData = moPLC.PLCData
            If sPLCData Is Nothing Then Return False
            'Load Collection

            For nItem = 0 To nUb
                mcolStyles(nItem).MuteLength.Value = CInt(sPLCData(nItem))
            Next
            Application.DoEvents()

            'Read Entrance Start data from PLC
            sTag = oZone.PLCTagPrefix & "PECEntMuteStart"
            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
            sPLCData = moPLC.PLCData
            If sPLCData Is Nothing Then Return False
            'Load Collection

            For nItem = 0 To nUb
                mcolStyles(nItem).EntranceMuteStartCount.Value = CInt(sPLCData(nItem))
            Next
            Application.DoEvents()

            'Read exit Start data from PLC
            sTag = oZone.PLCTagPrefix & "PECExitMuteStart"
            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
            sPLCData = moPLC.PLCData
            If sPLCData Is Nothing Then Return False
            'Load Collection

            For nItem = 0 To nUb
                mcolStyles(nItem).ExitMuteStartCount.Value = CInt(sPLCData(nItem))
            Next
            Application.DoEvents()

            DataLoaded = True
            mcolStyles.Update()

            Return True


        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bLoadFromGUI() As Boolean
        '********************************************************************************************
        'Description:  Load data stored on GUI in database, xml file etc.
        '
        'Parameters: none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/13/12  RJO     Bug fix for TabPage5 (PEC Muting). bLoadIntrusionFrom GUI does nothing.
        '********************************************************************************************

        Select Case msCurrentTabName
            Case "TabPage1"
                Return bLoadStylesFromGUI()
            Case "TabPage2"
                Return bLoadOptionsFromGUI()
            Case "TabPage3"
                Return bLoadRepairsFromGUI()
            Case "TabPage4"
                Return bLoadDegradeFromGUI()
            Case "TabPage5"
                Return bLoadStylesFromGUI() 'bLoadIntrusionFromGUI() RJO 09/13/12
            Case "TabPage6"
                Return bLoadStyleIDFromGUI()
            Case "TabPage7"
                Return bLoadStyleTricoatEnbFromGUI()
        End Select

    End Function
    Private Function bLoadFromPLC() As Boolean
        '********************************************************************************************
        'Description:  Load data stored on PLC.
        '
        'Parameters: none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        mbPLCFail = False
        Select Case msCurrentTabName
            Case "TabPage1"
                Return bLoadStylesFromPLC()
            Case "TabPage2"
                Return bLoadOptionsFromPLC()
            Case "TabPage3"
                Return bLoadRepairsFromPLC()
            Case "TabPage4"
                Return bLoadDegradeFromPLC()
            Case "TabPage5"
                Return bLoadIntrusionFromPLC()
            Case "TabPage6"
                If mnStyleIDIndex = -1 Then
                    mbPLCFail = True
                    Return True
                Else
                    Return bLoadStyleIDFromPLC()
                End If
            Case "TabPage7"
                Return bLoadStyleTricoatEnbFromPLC()
        End Select

    End Function
    Private Function bLoadStyleTricoatEnbFromGUI() As Boolean
        '********************************************************************************************
        'Description: 
        '
        'Parameters: none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' '07/08/13 BTK     Tricoat By Style
        '********************************************************************************************

        Try


            Return bLoadStyleCollectionFromDB()

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try
    End Function
    Private Function bLoadStyleTricoatEnbFromPLC() As Boolean
        '********************************************************************************************
        'Description:  Load data stored on PLC.
        '
        'Parameters: none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 07/08/13  BTK     Tricoat By Style
        '********************************************************************************************
        Dim bLoadSuccess As Boolean = False

        Try
            'Build the styles collection
            bLoadSuccess = bLoadStyleCollectionFromDB()

            moPLC.ZoneName = mcolZones.ActiveZone.Name

            '07/08/13 BTK Tricoat By Style
            If bLoadSuccess Then bLoadSuccess = bReadTricoatRbtsEnbStylesFromPLC()


            If Not mbPLCFail Then
                Return bLoadSuccess
            Else
                Return False
            End If

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            Return False

        End Try

    End Function
    Private Function bSaveStyleTricoatEnbToPLC() As Boolean
        '********************************************************************************************
        'Description:  Save data stored on PLC.
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/25/13  BTK     Tricoat By Style
        '********************************************************************************************
        Dim bSaveSuccess As Boolean = False
        Dim nStyleCount As Integer = 0
        Dim nIndex As Integer = 0
        Dim sData() As String
        Dim sTag As String
        Dim ub%

        Try
            moPLC.ZoneName = mcolZones.ActiveZone.Name
            ub% = mcolStyles.MaxStyles - 1

            ReDim sData(ub%)
            Dim nMask As Integer = CType(((2 ^ mcolArms.Count) - 1) * (2 ^ mcolZones.ActiveZone.RobotsRequiredStartingBit), Integer)

            For nIndex = 0 To ub%
                mcolStyles(nIndex).TricoatEnable.Value = mcolStyles(nIndex).TricoatEnable.Value And nMask
                sData(nIndex) = mcolStyles(nIndex).TricoatEnable.Value.ToString
            Next

            sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleTricoatEnabled"
            moPLC.TagName = sTag
            moPLC.PLCData = sData
            'room to pop up error if it happens


            Application.DoEvents()

            If Not mbPLCFail Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bSaveStyleTricoatEnbToSQLDB() As Boolean
        '********************************************************************************************
        'Description:  Save data stored on GUI in database, xml file etc.
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/25/13  BTK     Tricoat By Style
        '********************************************************************************************
        Return bSaveStylesToSQLDB()

    End Function
    Private Function bValidateStyleTricoatEnbData() As Boolean
        '********************************************************************************************
        'Description:  Don't check anything for enable checkboxes.  Here for consistency.
        '
        'Parameters: none
        'Returns:    false if bum data so it can be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 07/08/13  BTK     Tricoat By Style
        '********************************************************************************************
        Dim nItem As Integer = 0
        Dim sColumn As String = String.Empty
        Dim sKey As String = String.Empty
        Dim sReason As String = gcsRM.GetString("csVAL_BELOW_MIN", DisplayCulture)
        Dim oS As clsSysStyle = Nothing
        Dim oFText As FocusedTextBox.FocusedTextBox = Nothing
        Dim bValid As Boolean = True

        'Do nothing for checkboxes.

        If bValid = False Then
            'display bad data field and shift focus to it
            MessageBox.Show(sReason, gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                 MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Return True

    End Function
    Private Sub clbStyleTricoatEnb_ItemCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs)
        '********************************************************************************************
        'Description:  A style checked listbox item has been checked
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/23/09  BTK     Added code so we can change which bit the robot required data starts at.
        ' 11/15/11  BTK     Need more then 15 robots.
        '********************************************************************************************
        Dim sStyleIndex As Integer = CInt(DirectCast(sender, CheckedListBox).Tag)  ' style index stored in .tag
        Dim nValueToSet As Integer
        Dim oS As clsSysStyle = mcolStyles.Item(sStyleIndex - 1) ' ALSO BASE 0   TAGS ARE BASED 1

        Dim nArmBit As Integer = mcolArms(e.Index).RobotNumber - 1

        If e.NewValue = 1 Then
            nValueToSet = mMathFunctions.SetBit(oS.TricoatEnable.Value, nArmBit + mcolZones.ActiveZone.RobotsRequiredStartingBit, True)
        Else
            nValueToSet = mMathFunctions.ReSetBit(oS.TricoatEnable.Value, nArmBit + mcolZones.ActiveZone.RobotsRequiredStartingBit, True)
        End If

        oS.TricoatEnable.Value = nValueToSet
        If oS.TricoatEnable.Changed Then
            EditsMade = True
            DirectCast(sender, CheckedListBox).ForeColor = Color.Red
        End If
    End Sub
    Private Sub clbStyleTricoatEnb_MenuSelect(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  context select all/none handler
        '
        'Parameters: sender is menuitem
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oMenuItem As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim oClb As CheckedListBox = _
                    DirectCast(pnlStyleTricoatEnb.Controls.Item(oMenuItem.Owner.Name), CheckedListBox)
        Dim bTmp As Boolean


        If Privilege = ePrivilege.None Then Exit Sub

        Call clbAll_GotFocus(oClb, e)

        Select Case oMenuItem.Text
            Case gcsRM.GetString("csSELECT_ALL", DisplayCulture)
                bTmp = True
            Case Else
                bTmp = False
        End Select

        For i As Integer = 0 To oClb.Items.Count - 1
            oClb.SetItemChecked(i, bTmp)
        Next

        Call clbAll_LostFocus(oClb, e)

    End Sub
    Private Sub subLoadStyleTricoatEnbTabData(ByVal oStyle As clsSysStyle, ByVal Index As String)
        '********************************************************************************************
        'Description:  Load Intrusion Tab data
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sName As String = "lblTricoatPlantStyle" & Index

        pnlStyleTricoatEnb.Controls.Item(sName).Text = oStyle.PlantNumber.Text

        sName = "lblTricoatStyleDesc" & Index
        pnlStyleTricoatEnb.Controls.Item(sName).Text = oStyle.Description.Text

        sName = "clbStyleTricoatEnb" & Index
        Dim clb As CheckedListBox = DirectCast(pnlStyleTricoatEnb.Controls.Item(sName), CheckedListBox)
        Dim nValue As Integer = oStyle.TricoatEnable.Value

        clb.ForeColor = Color.Black
        If mbReLoad Then
            Dim nCurrentValue As Integer

            'For i As Integer = 0 To clb.Items.Count - 1
            '    'lets hope there are no more than 30 robots in a zone....
            '    If i > 30 Then Exit Sub
            '    If clb.GetItemCheckState(i) = CheckState.Checked Then
            '        nCurrentValue += gnBitVal(i + mcolZones.ActiveZone.RobotsRequiredStartingBit)
            '    End If
            'Next 'i
            For nArm As Integer = 0 To mcolArms.Count - 1
                'lets hope there are no more than 15 robots in a zone....
                If clb.GetItemCheckState(nArm) = CheckState.Checked Then
                    nCurrentValue += gnBitVal(mcolArms(nArm).RobotNumber + mcolZones.ActiveZone.RobotsRequiredStartingBit - 1)
                End If
            Next 'i
            If nCurrentValue <> nValue Then
                clb.ForeColor = Color.Red
                EditsMade = True
            End If
        End If 'mbReLoad

        'For i As Integer = 0 To clb.Items.Count - 1
        '    'lets hope there are no more than 30 robots in a zone....
        '    If i > 30 Then Exit Sub
        '    clb.SetItemChecked(i, (nValue And gnBitVal(i + mcolZones.ActiveZone.RobotsRequiredStartingBit)) > 0)
        'Next 'i
        For nArm As Integer = 0 To mcolArms.Count - 1
            'lets hope there are no more than 15 robots in a zone....
            clb.SetItemChecked(nArm, (nValue And gnBitVal(mcolArms(nArm).RobotNumber + mcolZones.ActiveZone.RobotsRequiredStartingBit - 1)) > 0)
        Next
    End Sub
    Private Sub subPrintStyleTricoatEnb(ByRef mPrintHtml As clsPrintHtml, ByVal sTitle() As String, ByVal sSubTitle As String())
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
        Dim nRobot As Integer = 0
        Dim nMask As Integer = 0

        Try

            Dim sText(1) As String

            sText(0) = Trim(gpsRM.GetString("psITEM_CAP", DisplayCulture) & vbTab & _
                        lblPlantStyleCap7.Text & vbTab & lblDescCap7.Text)

            For i As Integer = 0 To clbStyleTricoatEnb001.Items.Count - 1
                sText(0) = sText(0) & vbTab & _
                        clbStyleTricoatEnb001.GetItemText(clbStyleTricoatEnb001.Items(i))
            Next

            ReDim Preserve sText(mcolStyles.Count)
            For Each oThisStyle As clsSysStyle In mcolStyles
                With oThisStyle
                    sText(.ItemNumber + 1) = (.ItemNumber + 1).ToString & vbTab & _
                                .PlantString & vbTab & .Description.Text
                    For nRobot = 0 To mnNumberofRobots - 1
                        nMask = CType(2 ^ (nRobot + mcolZones.ActiveZone.RobotsRequiredStartingBit), Integer)
                        If (.RobotsRequired.Value And nMask) > 0 Then
                            sText(.ItemNumber + 1) = sText(.ItemNumber + 1) & vbTab & " X "
                        Else
                            sText(.ItemNumber + 1) = sText(.ItemNumber + 1) & vbTab & " - "
                        End If
                    Next 'nRobot
                    sText(.ItemNumber + 1) = sText(.ItemNumber + 1).Replace("<", "")
                    sText(.ItemNumber + 1) = sText(.ItemNumber + 1).Replace(">", "")
                    sText(.ItemNumber + 1) = Trim(sText(.ItemNumber + 1))
                End With

            Next oThisStyle

            Progress = 30
            '
            mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle)

            '
            Progress = 55
            '
            Status = gcsRM.GetString("csPRINT_FORMATTING", DisplayCulture)
            '

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                               Status, MessageBoxButtons.OK)

        Finally

        End Try

    End Sub
    Private Sub subShowNewStyleTricoatEnbTab()
        '********************************************************************************************
        'Description:  Data is all loaded or changed and screen needs to be refreshed
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 07/08/13  BTK     Tricoat By Style
        '********************************************************************************************
        Dim oStyle As clsSysStyle
        Dim sName As String = String.Empty
        Dim nHorizontalOffset As Integer = pnlStyleTricoatEnb.HorizontalScroll.Value
        Dim nRow1Top As Integer = lblTricoatPlantStyle001.Top + pnlStyleTricoatEnb.VerticalScroll.Value
        Dim nPlantStyleLeft As Integer = lblTricoatPlantStyle001.Left + nHorizontalOffset
        Dim nFanucStyleLeft As Integer = ftbFanucStyle001.Left + nHorizontalOffset
        Dim nDescLeft As Integer = lblTricoatStyleDesc001.Left + nHorizontalOffset
        Dim nRobotsLeft As Integer = clbStyleTricoatEnb001.Left + nHorizontalOffset
        Dim mnuSelect As ContextMenuStrip
        Dim imgVoid As Image = Nothing

        mnDataRows = mcolStyles.MaxStyles

        Try
            Call subUnloadPanel(pnlStyleTricoatEnb)

            'Create controls 
            With pnlStyleTricoatEnb
                .SuspendLayout()
                .AutoScroll = True
                .AutoScrollMargin = New Size(10, mnROWSPACE)
                .VerticalScroll.SmallChange = mnROWSPACE
            End With


            subSetUpRobotCheckBox(clbStyleTricoatEnb001)

            AddHandler clbStyleTricoatEnb001.ItemCheck, AddressOf clbStyleTricoatEnb_ItemCheck
            AddHandler clbStyleTricoatEnb001.LostFocus, AddressOf clbAll_LostFocus
            AddHandler clbStyleTricoatEnb001.GotFocus, AddressOf clbAll_GotFocus


            mnuSelect = New ContextMenuStrip
            With mnuSelect
                .Name = "clbStyleTricoatEnb001"
                .Items.Add(gcsRM.GetString("csSELECT_ALL", DisplayCulture), imgVoid, AddressOf clbStyleTricoatEnb_MenuSelect)
                .Items.Add(gcsRM.GetString("csSELECT_NONE", DisplayCulture), imgVoid, AddressOf clbStyleTricoatEnb_MenuSelect)
            End With
            clbStyleTricoatEnb001.ContextMenuStrip = mnuSelect


            For nIndex As Integer = 2 To mcolStyles.MaxStyles
                Dim sIndex As String = Strings.Format(nIndex, "000")
                Dim nTop As Integer = nRow1Top + ((nIndex - 1) * mnROWSPACE)

                'Plant Style Column
                sName = "lblTricoatPlantStyle" & sIndex
                Dim lblPS As Label = mScreenSetup.CloneLabel(lblTricoatPlantStyle001, sName)
                lblPS.Tag = sIndex
                pnlStyleTricoatEnb.Controls.Add(lblPS)
                lblPS.Left = nPlantStyleLeft
                lblPS.Top = nTop
                'SA
                lblPS.Visible = Not mcolZones.StandAlone


                'Description Column
                sName = "lblTricoatStyleDesc" & sIndex
                Dim lblD As Label = mScreenSetup.CloneLabel(lblTricoatStyleDesc001, sName)

                lblD.Tag = sIndex
                pnlStyleTricoatEnb.Controls.Add(lblD)
                lblD.Left = nDescLeft
                lblD.Top = nTop
                lblD.Visible = True

                'Robots Required Column
                sName = "clbStyleTricoatEnb" & sIndex
                Dim clbRR As CheckedListBox = mScreenSetup.CloneCheckedListBox(clbStyleTricoatEnb001, sName)
                clbRR.Tag = sIndex
                pnlStyleTricoatEnb.Controls.Add(clbRR)
                clbRR.Left = nRobotsLeft
                clbRR.Top = nTop
                'SA
                clbRR.Visible = Not mcolZones.StandAlone

                AddHandler clbRR.ItemCheck, AddressOf clbStyleTricoatEnb_ItemCheck
                AddHandler clbRR.LostFocus, AddressOf clbAll_LostFocus
                AddHandler clbRR.GotFocus, AddressOf clbAll_GotFocus


                mnuSelect = New ContextMenuStrip
                With mnuSelect
                    .Name = "clbStyleTricoatEnb" & sIndex
                    .Items.Add(gcsRM.GetString("csSELECT_ALL", DisplayCulture), imgVoid, AddressOf clbStyleTricoatEnb_MenuSelect)
                    .Items.Add(gcsRM.GetString("csSELECT_NONE", DisplayCulture), imgVoid, AddressOf clbStyleTricoatEnb_MenuSelect)
                End With
                clbRR.ContextMenuStrip = mnuSelect
            Next 'nIndex

            mScreenSetup.LoadCheckedListBoxCollection(Me, "pnlStyleTricoatEnb", mcolStyleTricoatEnbCLBoxes)

            lblTricoatRobotsReqCap.Left = nRobotsLeft + 50

            mbStyleTricoatEnbTabInitialized = True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        Finally
            pnlStyleTricoatEnb.ResumeLayout()
            If DataLoaded And EditsMade = True Then EditsMade = False
            'Cleanup
            oStyle = Nothing
        End Try

    End Sub
    Private Function bReadTricoatRbtsEnbStylesFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the Styles Tricoat Enable data from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '    07/08/13   BTK     Tricoat By Style
        '********************************************************************************************
        Dim sPLCData() As String
        Dim sTag As String = String.Empty
        Dim nItem As Integer = 0

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleTricoatEnabled"

        Try
            'Read robots required data from PLC
            moPLC.TagName = sTag
            sPLCData = moPLC.PLCData
            If sPLCData Is Nothing Then Return False

            'Load Collection
            For nItem = 0 To UBound(sPLCData)
                mcolStyles(nItem).TricoatEnable.Value = CType(sPLCData(nItem), Integer)
            Next
            Application.DoEvents()

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            Return False
        End Try

    End Function
    Private Function bLoadOptionCollectionFromDB() As Boolean
        '********************************************************************************************
        'Description:  Creates a new Option collection
        '
        'Parameters: none
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bTmp As Boolean = False
        Dim oZone As clsZone = mcolZones.ActiveZone

        'Clear out the previous styles collection
        mcolOptions = Nothing

        Try
            mcolOptions = New clsSysOptions(mcolZones.ActiveZone)

            Return True

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            Return False
        End Try

    End Function
    Private Function bLoadOptionsFromGUI() As Boolean
        '********************************************************************************************
        'Description:  Load data stored on GUI in database, xml file etc.
        '
        'Parameters: none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            Return bLoadOptionCollectionFromDB()


        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bLoadOptionsFromPLC() As Boolean
        '********************************************************************************************
        'Description:  Load data stored on PLC.
        '
        'Parameters: none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bLoadSuccess As Boolean = False

        Try
            'Build the styles collection
            bLoadSuccess = bLoadOptionCollectionFromDB()
            
            moPLC.ZoneName = mcolZones.ActiveZone.Name

            If bLoadSuccess Then bLoadSuccess = bReadPlantOptionFromPLC()

            If bLoadSuccess Then bLoadSuccess = bReadFanucOptionFromPLC()

            If bLoadSuccess Then bLoadSuccess = bReadRbtsReqdOptionFromPLC()

            If mcolOptions.OptionRegisters Then
                If bLoadSuccess Then bLoadSuccess = bReadOptionRegistersFromPLC()
            End If

            If Not mbPLCFail Then
                Return bLoadSuccess
            Else
                Return False
            End If

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            Return False

        End Try

    End Function
    Private Function bLoadRepairsFromGUI() As Boolean
        '********************************************************************************************
        'Description:  Load data stored on GUI in database, xml file etc.
        '
        'Parameters: none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            mcolStyles.LoadRepairPanelDescriptions()
            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try


    End Function

    Private Function bLoadStyleCollectionFromDB() As Boolean
        '********************************************************************************************
        'Description:  Creates a new style collection
        '
        'Parameters: none
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bTmp As Boolean = False
        Dim oZone As clsZone = mcolZones.ActiveZone

        'Clear out the previous styles collection
        mcolStyles = Nothing

        Try
          

            mcolStyles = New clsSysStyles(oZone)
            cboStyle.Items.Clear()
            mcolStyles.LoadStyleBoxFromCollection(cboStyle, , , mcolZones.ActiveZone.DegradeStyleRbtsReq, mcolZones.ActiveZone.NumberOfDegrades)
            'clear out repair tab
            mbRepairsTabInitialized = False

            Return True

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            Return False
        End Try

    End Function
    Private Function bLoadRepairsFromPLC() As Boolean
        '********************************************************************************************
        'Description:  Load data stored on PLC.
        '
        'Parameters: none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bLoadSuccess As Boolean = False

        Try

            Return bReadRbtsReqdRepairFromPLC()

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            Return False

        End Try

    End Function
    Private Function bLoadStyleIDCollectionFromDB() As Boolean
        '********************************************************************************************
        'Description:  Creates a new style collection
        '
        'Parameters: none
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 07/08/13  MSW     Move the style ID tables to XML
        '********************************************************************************************
        Dim bTmp As Boolean = False
        Dim oZone As clsZone = mcolZones.ActiveZone
        Dim oDS As New DataSet
        Dim sTable As String = "Style ID Setup"
        Dim nNumberOfConveyorSnapshots As Integer

        'Clear out the previous styles collection
        'mcolStyleID = Nothing

        Try

            'We need the number of conveyor snapshots before we validate the database.  This isn't loaded
            'into the collection until after validating the database.

            'set property
            Const sXMLFILE As String = "StyleIDSetup"
            Const sXMLTABLE As String = "StyleIDSetup"

            Dim sTopic As String = String.Empty
            Dim oMainNode As XmlNode = Nothing
            Dim oNodeList As XmlNodeList = Nothing
            Dim oXMLDoc As New XmlDocument
            Dim sXMLFilePath As String = String.Empty
            If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLFILE & ".XML") Then
                Try
                    oXMLDoc.Load(sXMLFilePath)
                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    If oMainNode IsNot Nothing Then
                        nNumberOfConveyorSnapshots = CType(oMainNode.Item("NumberStyleIDConveyorSnapshots").InnerXml, Integer)
                    End If

                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: bLoadStyleIDCollectionFromDB", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
            End If



            
            ''first validate the database
            'With DB
            '    .DBFileName = gsPROCESS_DBNAME
            '    .Zone = oZone
            '    Dim ds As DataSet = mSysStyleID.GetStyleIDDataset(DB, oZone.Name)
            '    If ds Is Nothing Then
            '        Return False
            '    End If

            '    'validate me
            '    If bValidateStyleIDDataset(oZone.Name, oZone.MaxStyles, ds, bTmp, nNumberOfConveyorSnapshots) Then

            '        If bTmp Then
            '            .UpdateDataSet(ds, .DBTableName)
            '            ' this should ONLY happen once when setting up a new site
            '            ' so is not localized
            '            MessageBox.Show("Adjusted the Style ID table to match the number of Styles", _
            '                            "Styles = " & oZone.MaxStyles)

            '        End If

            '    Else
            '        Throw New Data.ConstraintException(gpsRM.GetString("psERR_STYLES_TABLE"))
            '    End If


            '    .Close()
            'End With
            ''The Style Collection loads itself in the next line
            ''TODO - Is this going to be a problem if it's not Styles by zone???

            If mcolStyleID Is Nothing Then
                cboStyleID.Items.Clear()
                mcolStyles.LoadStyleBoxFromCollection(cboStyleID, , , mcolZones.ActiveZone.DegradeStyleRbtsReq, mcolZones.ActiveZone.NumberOfDegrades)
            End If

            mcolStyleID = New clsSysStylesID(oZone, mnStyleIDIndex + 1)

            Return True

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            Return False
        End Try

    End Function
    Private Function bValidateStyleDataset(ByVal ZoneName As String, ByVal NumStyles As Integer, _
                                            ByRef ds As DataSet, ByRef UpdateFile As Boolean) As Boolean
        '********************************************************************************************
        'Description:  Make sure we have the right number of styles and fields are full
        '
        'Parameters:  
        'Returns:     True if  success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim dtStyle As DataTable = ds.Tables("[" & gsSYSS_DS_TABLENAME & "]")
            Dim oRows As DataRowCollection = dtStyle.Rows()
            Dim nUB As Integer = dtStyle.Rows.Count - 1

            'when we recurse, the rows are only marked for deletion, need to account for this
            For i As Integer = 0 To dtStyle.Rows.Count - 1
                Dim r As DataRow = dtStyle.Rows(i)
                If r.RowState = DataRowState.Deleted Then nUB -= 1
            Next

            Select Case nUB
                Case Is > (NumStyles - 1)
                    'whack the extras
                    For i As Integer = nUB To (NumStyles) Step -1
                        Dim r As DataRow = dtStyle.Rows(i)
                        r.Delete()
                        Status = "Delete Style"
                    Next

                    UpdateFile = True
                    'recursion - danger will robinson!
                    If bValidateStyleDataset(ZoneName, NumStyles, ds, _
                                                                UpdateFile) = False Then
                        Return False
                    End If

                Case Is < (NumStyles - 1)
                    'Add the extras
                    For i As Integer = (nUB + 1) To (NumStyles - 1)
                        Dim r As DataRow = dtStyle.NewRow
                        bValidateStyleDataRow(ZoneName, (i + 1), r)
                        dtStyle.Rows.Add(r)
                        Status = "Add Style"
                    Next

                    UpdateFile = True
                    'recursion - danger will robinson!
                    If bValidateStyleDataset(ZoneName, NumStyles, ds, _
                                                                UpdateFile) = False Then
                        Return False
                    End If

                Case Is = (NumStyles - 1)
                    'this is the right number - should be 1 thru max_valves
                    'verify the data
                    For i As Integer = 0 To nUB
                        Dim r As DataRow = oRows(i)
                        If bValidateStyleDataRow(ZoneName, (i + 1), r) Then
                            'changed
                            UpdateFile = True
                        End If

                    Next
            End Select


            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try


    End Function
    Private Function bValidateStyleIDDataset(ByVal ZoneName As String, ByVal NumStyles As Integer, _
                                        ByRef ds As DataSet, ByRef UpdateFile As Boolean, _
                                        ByVal NumberOfConveyorSnapshots As Integer) As Boolean
        '********************************************************************************************
        'Description:  Make sure we have the right number of styles and fields are full
        '
        'Parameters:  
        'Returns:     True if  success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim dtStyleID As DataTable = ds.Tables("[" & gsSYSID_DS_TABLENAME & "]")
            Dim oRows As DataRowCollection = dtStyleID.Rows()
            Dim nUB As Integer = dtStyleID.Rows.Count - 1

            'when we recurse, the rows are only marked for deletion, need to account for this
            For i As Integer = 0 To dtStyleID.Rows.Count - 1
                Dim r As DataRow = dtStyleID.Rows(i)
                If r.RowState = DataRowState.Deleted Then nUB -= 1
            Next

            Select Case (nUB \ NumberOfConveyorSnapshots)
                Case Is > (NumStyles - 1)
                    'whack the extras
                    For i As Integer = nUB To (NumStyles * NumberOfConveyorSnapshots) Step -1
                        Dim r As DataRow = dtStyleID.Rows(i)
                        r.Delete()
                        Status = "Delete Style"
                    Next

                    UpdateFile = True
                    'recursion - danger will robinson!
                    If bValidateStyleIDDataset(ZoneName, NumStyles, ds, _
                                                                UpdateFile, NumberOfConveyorSnapshots) = False Then
                        Return False
                    End If

                Case Is < (NumStyles - 1)
                    'Add the extras
                    For i As Integer = (nUB + 1) To ((NumStyles * NumberOfConveyorSnapshots) - 1)
                        Dim r As DataRow = dtStyleID.NewRow
                        bValidateStyleIDDataRow(ZoneName, (i + 1), r)
                        dtStyleID.Rows.Add(r)
                        Status = "Add Style"
                    Next

                    UpdateFile = True
                    'recursion - danger will robinson!
                    If bValidateStyleIDDataset(ZoneName, NumStyles, ds, _
                                                                UpdateFile, NumberOfConveyorSnapshots) = False Then
                        Return False
                    End If

                Case Is = (NumStyles - 1)
                    'this is the right number - should be 1 thru max_valves
                    'verify the data
                    For i As Integer = 0 To nUB
                        Dim r As DataRow = oRows(i)
                        If bValidateStyleIDDataRow(ZoneName, (i + 1), r) Then
                            'changed
                            UpdateFile = True
                        End If

                    Next
            End Select


            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try


    End Function

    Private Shared Function bValidateStyleDataRow(ByVal ZoneName As String, _
                                               ByVal StyleNumber As Integer, _
                                               ByRef dRow As DataRow) As Boolean
        '*********************************************************************************************
        'Description:  Check datarow for integrity
        '
        'Parameters:
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*********************************************************************************************
        Dim bChanged As Boolean = False

        'check for nulls
        If dRow.IsNull(gsSYSS_COL_PLANTNUM) Then
            bChanged = True
            dRow(gsSYSS_COL_PLANTNUM) = 0
        End If

        If dRow.IsNull(gsSYSS_COL_FANUCNUM) Then
            bChanged = True
            dRow(gsSYSS_COL_FANUCNUM) = 0
        End If


        If dRow.IsNull(gsZONE_NAME) Then
            bChanged = True
            dRow(gsZONE_NAME) = ZoneName
        End If
        If CType(dRow(gsZONE_NAME), String) <> ZoneName Then
            bChanged = True
            dRow(gsZONE_NAME) = ZoneName
        End If

        If dRow.IsNull(gsSYSS_COL_DESC) Then
            bChanged = True
            dRow(gsSYSS_COL_DESC) = gpsRM.GetString("psSTYLE") & " " & _
                                                StyleNumber.ToString(mLanguage.FixedCulture)
        End If


        If dRow.IsNull(gsCOL_SORTORDER) Then
            bChanged = True
            dRow(gsCOL_SORTORDER) = StyleNumber
        Else
            If CType(dRow(gsCOL_SORTORDER), Integer) <> StyleNumber Then
                dRow(gsCOL_SORTORDER) = StyleNumber
                bChanged = True
            End If
        End If

        If dRow.IsNull(gsSYSS_COL_IMG_KEY) Then
            bChanged = True
            dRow(gsSYSS_COL_IMG_KEY) = "default"
        End If

        If dRow.IsNull(gsSYSS_COL_ENABLE) Then
            bChanged = True
            dRow(gsSYSS_COL_ENABLE) = 0
        End If

        Return bChanged

    End Function

    Private Shared Function bValidateStyleIDDataRow(ByVal ZoneName As String, _
                                           ByVal StyleNumber As Integer, _
                                           ByRef dRow As DataRow) As Boolean
        '*********************************************************************************************
        'Description:  Check datarow for integrity
        '
        'Parameters:
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*********************************************************************************************
        Dim bChanged As Boolean = False

        'check for nulls
        If dRow.IsNull(gsSYSID_COL_CONVEYOR_COUNT) Then
            bChanged = True
            dRow(gsSYSID_COL_CONVEYOR_COUNT) = 0
        End If

        If dRow.IsNull(gsSYSID_COL_PHOTOEYE_IGNORE) Then
            bChanged = True
            dRow(gsSYSID_COL_PHOTOEYE_IGNORE) = 0
        End If

        If dRow.IsNull(gsSYSID_COL_PHOTOEYE_PATTERN) Then
            bChanged = True
            dRow(gsSYSID_COL_PHOTOEYE_PATTERN) = 0
        End If

        If dRow.IsNull(gsSYSID_COL_SUNROOF_PHOTOEYE_IGNORE) Then
            bChanged = True
            dRow(gsSYSID_COL_SUNROOF_PHOTOEYE_IGNORE) = 0
        End If

        If dRow.IsNull(gsSYSID_COL_SUNROOF_PHOTOEYE_PATTERN) Then
            bChanged = True
            dRow(gsSYSID_COL_SUNROOF_PHOTOEYE_PATTERN) = 0
        End If

        If dRow.IsNull(gsZONE_NAME) Then
            bChanged = True
            dRow(gsZONE_NAME) = ZoneName
        End If
        If CType(dRow(gsZONE_NAME), String) <> ZoneName Then
            bChanged = True
            dRow(gsZONE_NAME) = ZoneName
        End If

        If dRow.IsNull(gsCOL_SORTORDER) Then
            bChanged = True
            dRow(gsCOL_SORTORDER) = StyleNumber
        Else
            If CType(dRow(gsCOL_SORTORDER), Integer) <> StyleNumber Then
                dRow(gsCOL_SORTORDER) = StyleNumber
                bChanged = True
            End If
        End If

        Return bChanged

    End Function
    Private Function bValidateOptionData(ByVal ZoneName As String, ByVal NumOptions As Integer, _
                                            ByRef ds As DataSet, ByRef UpdateFile As Boolean) As Boolean
        '********************************************************************************************
        'Description:  Make sure we have the right number of Options and fields are full
        '
        'Parameters:  
        'Returns:     True if  success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim dtOption As DataTable = ds.Tables("[" & gsSYSO_DS_TABLENAME & "]")
            Dim oRows As DataRowCollection = dtOption.Rows()
            Dim nUB As Integer = dtOption.Rows.Count - 1

            'when we recurse, the rows are only marked for deletion, need to account for this
            For i As Integer = 0 To dtOption.Rows.Count - 1
                Dim r As DataRow = dtOption.Rows(i)
                If r.RowState = DataRowState.Deleted Then nUB -= 1
            Next

            Select Case nUB
                Case Is > (NumOptions - 1)
                    'whack the extras
                    For i As Integer = nUB To (NumOptions) Step -1
                        Dim r As DataRow = dtOption.Rows(i)
                        r.Delete()
                        Status = "Delete Option"
                    Next

                    UpdateFile = True
                    'recursion - danger will robinson!
                    If bValidateOptionData(ZoneName, NumOptions, ds, _
                                                                UpdateFile) = False Then
                        Return False
                    End If

                Case Is < (NumOptions - 1)
                    'Add the extras
                    For i As Integer = (nUB + 1) To (NumOptions - 1)
                        Dim r As DataRow = dtOption.NewRow
                        bValidateOptionDataRow(ZoneName, (i + 1), r)
                        dtOption.Rows.Add(r)
                        Status = "Add Option"
                    Next

                    UpdateFile = True
                    'recursion - danger will robinson!
                    If bValidateOptionData(ZoneName, NumOptions, ds, _
                                                                UpdateFile) = False Then
                        Return False
                    End If

                Case Is = (NumOptions - 1)
                    'this is the right number - should be 1 thru max_valves
                    'verify the data
                    For i As Integer = 0 To nUB
                        Dim r As DataRow = oRows(i)
                        If bValidateOptionDataRow(ZoneName, (i + 1), r) Then
                            'changed
                            UpdateFile = True
                        End If

                    Next
            End Select


            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try


    End Function
    Private Shared Function bValidateOptionDataRow(ByRef ZoneName As String, _
                                               ByVal OptionNumber As Integer, _
                                               ByRef dRow As DataRow) As Boolean
        '*********************************************************************************************
        'Description:  Check datarow for integrity
        '
        'Parameters:
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*********************************************************************************************
        Dim bChanged As Boolean = False

        'check for nulls
        If dRow.IsNull(gsSYSO_COL_PLANTNUM) Then
            bChanged = True
            dRow(gsSYSS_COL_PLANTNUM) = 0
        End If

        If dRow.IsNull(gsSYSO_COL_FANUCNUM) Then
            bChanged = True
            dRow(gsSYSS_COL_FANUCNUM) = 0
        End If

        If dRow.IsNull(gsSYSO_COL_ENABLE) Then
            bChanged = True
            dRow(gsSYSO_COL_ENABLE) = 0
        End If

        If dRow.IsNull(gsZONE_NAME) Then
            bChanged = True
            dRow(gsZONE_NAME) = ZoneName
        End If
        If CType(dRow(gsZONE_NAME), String) <> ZoneName Then
            bChanged = True
            dRow(gsZONE_NAME) = ZoneName
        End If

        If dRow.IsNull(gsSYSO_COL_DESC) Then
            bChanged = True
            dRow(gsSYSS_COL_DESC) = gpsRM.GetString("psOPTION") & " " & _
                                                OptionNumber.ToString(mLanguage.FixedCulture)
        End If


        If dRow.IsNull(gsCOL_SORTORDER) Then
            bChanged = True
            dRow(gsCOL_SORTORDER) = OptionNumber
        Else
            If CType(dRow(gsCOL_SORTORDER), Integer) <> OptionNumber Then
                dRow(gsCOL_SORTORDER) = OptionNumber
                bChanged = True
            End If
        End If


        Return bChanged

    End Function
    Private Function bLoadStylesFromGUI() As Boolean
        '********************************************************************************************
        'Description:  Load data stored on GUI in database, xml file etc.
        '
        'Parameters: none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try


            Return bLoadStyleCollectionFromDB()

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bLoadStylesFromPLC() As Boolean
        '********************************************************************************************
        'Description:  Load data stored on PLC.
        '
        'Parameters: none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/22/11  JBW     Added reading two coat style data from PLC
        '********************************************************************************************
        Dim bLoadSuccess As Boolean = False

        Try
            'Build the styles collection
            bLoadSuccess = bLoadStyleCollectionFromDB()

            moPLC.ZoneName = mcolZones.ActiveZone.Name

            If mcolStyles.UseAscii Then
                If bLoadSuccess Then bLoadSuccess = bReadPlantAsciiStylesFromPLC()
            Else
                If bLoadSuccess Then bLoadSuccess = bReadPlantStylesFromPLC()
            End If

            If bLoadSuccess Then bLoadSuccess = bReadFanucStylesFromPLC()

            If mcolStyles.TwoCoat Then
                If bLoadSuccess Then bLoadSuccess = bReadTwoCoatStylesFromPLC()
            End If

            If mcolStyles.StyleRegisters Then
                If bLoadSuccess Then bLoadSuccess = bReadStyleRegistersFromPLC()
            End If

            If bLoadSuccess Then bLoadSuccess = bReadRbtsReqdStylesFromPLC()


            If Not mbPLCFail Then
                Return bLoadSuccess
            Else
                Return False
            End If

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            Return False

        End Try

    End Function
    Private Function bLoadStyleIDFromPLC() As Boolean
        '********************************************************************************************
        'Description:  Load data stored on PLC.
        '
        'Parameters: none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bLoadSuccess As Boolean = False

        Try
            'Build the styles collection
            bLoadSuccess = bLoadStyleIDCollectionFromDB()

            moPLC.ZoneName = mcolZones.ActiveZone.Name
            If mcolStyleID.NumberOfConveyorSnapshots > 0 Then
                If bLoadSuccess Then bLoadSuccess = bReadConveyorPositionsFromPLC()

                If bLoadSuccess Then bLoadSuccess = bReadPhotoeyePatternFromPLC()

                If bLoadSuccess Then bLoadSuccess = bReadPhotoeyeIgnoreFromPLC()
            Else
                If bLoadSuccess Then bLoadSuccess = bReadNonTrkStyleIDFromPLC()
            End If




            If Not mbPLCFail Then
                Return bLoadSuccess
            Else
                Return False
            End If

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            Return False

        End Try

    End Function
    Private Function bReadFanucOptionFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the Fanuc Option numbers from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        ' 6/1/07        Geo     .net added tab
        Dim sPLCData() As String
        Dim sTag As String = String.Empty
        Dim nItem As Integer = 0

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "FanucOptions"

        Try
            'Read Plant Style data from PLC
            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
            sPLCData = moPLC.PLCData
            If sPLCData Is Nothing Then Return False
            'Load Collection
            Dim nMax As Integer = mcolOptions.Count - 1
            If UBound(sPLCData) < nMax Then
                nMax = UBound(sPLCData)
            End If
            For nItem = 0 To nMax
                mcolOptions(nItem).FanucNumber.Value = CType(sPLCData(nItem), Integer)
            Next
            Application.DoEvents()

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bReadFanucStylesFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the Fanuc Style numbers from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************

        Dim sPLCData() As String
        Dim sPLCData2() As String 'NVSP 1000 Styles
        Dim sPLCData3() As String 'NVSP 1000 Styles
        Dim sPLCData4() As String 'NVSP 1000 Styles
        Dim sPLCData5() As String 'NVSP 1000 Styles
        Dim sPLCData6() As String 'NVSP 1000 Styles
        Dim sPLCData7() As String 'NVSP 1000 Styles
        Dim sPLCData8() As String 'NVSP 1000 Styles
        Dim sPLCData9() As String 'NVSP 1000 Styles
        Dim sPLCData10() As String 'NVSP 1000 Styles
        Dim sTag As String = String.Empty
        Dim nItem As Integer = 0

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "FanucStyles1"
        moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
        sPLCData = moPLC.PLCData

        'NVSP to fix when requested for more than 100 styles
        sTag = mcolZones.ActiveZone.PLCTagPrefix & "FanucStyles2"
        moPLC.TagName = sTag
        sPLCData2 = moPLC.PLCData

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "FanucStyles3"
        moPLC.TagName = sTag
        sPLCData3 = moPLC.PLCData

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "FanucStyles4"
        moPLC.TagName = sTag
        sPLCData4 = moPLC.PLCData

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "FanucStyles5"
        moPLC.TagName = sTag
        sPLCData5 = moPLC.PLCData

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "FanucStyles6"
        moPLC.TagName = sTag
        sPLCData6 = moPLC.PLCData

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "FanucStyles7"
        moPLC.TagName = sTag
        sPLCData7 = moPLC.PLCData

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "FanucStyles8"
        moPLC.TagName = sTag
        sPLCData8 = moPLC.PLCData

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "FanucStyles9"
        moPLC.TagName = sTag
        sPLCData9 = moPLC.PLCData

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "FanucStyles10"
        moPLC.TagName = sTag
        sPLCData10 = moPLC.PLCData

        sPLCData = CType(MergeArrays(sPLCData, sPLCData2), String())
        sPLCData = CType(MergeArrays(sPLCData, sPLCData3), String())
        sPLCData = CType(MergeArrays(sPLCData, sPLCData4), String())
        sPLCData = CType(MergeArrays(sPLCData, sPLCData5), String())
        sPLCData = CType(MergeArrays(sPLCData, sPLCData6), String())
        sPLCData = CType(MergeArrays(sPLCData, sPLCData7), String())
        sPLCData = CType(MergeArrays(sPLCData, sPLCData8), String())
        sPLCData = CType(MergeArrays(sPLCData, sPLCData9), String())
        sPLCData = CType(MergeArrays(sPLCData, sPLCData10), String())

        Try
            'Read Plant Style data from PLC
            If sPLCData Is Nothing Then Return False
            'Load Collection
            Dim nMax As Integer = mcolStyles.Count - 1
            If UBound(sPLCData) < nMax Then
                nMax = UBound(sPLCData)
            End If
            For nItem = 0 To nMax
                mcolStyles(nItem).FanucNumber.Value = CType(sPLCData(nItem), Integer)
            Next
            Application.DoEvents()

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bReadTwoCoatStylesFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the Fanuc Style numbers from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        ' 02/22/11      JBW     Copy-paste modified for two coats
        '********************************************************************************************

        Dim sPLCData() As String
        Dim sTag As String = String.Empty
        Dim nItem As Integer = 0

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "TwoCoatStyle"

        Try
            'Read Plant Style data from PLC
            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
            sPLCData = moPLC.PLCData
            If sPLCData Is Nothing Then Return False
            'Load Collection

            For nItem = 0 To UBound(sPLCData)
                mcolStyles(nItem).TwoCoatStyle.Value = CType(sPLCData(nItem), Boolean)
            Next
            Application.DoEvents()

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function

    Private Function bReadConveyorPositionsFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the conveyor position numbers from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date      By      Reason
        '    07/08/13  MSW     Move the style ID tables to XML
        '********************************************************************************************

        Dim sPLCData() As String
        Dim sTag As String = String.Empty
        Dim nItem As Integer = 0
        Dim nDataOffset As Integer

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "ConveyorPositions" & mnStyleIDIndex + 1

        Try
            'Read conveyor position data from PLC
            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
            sPLCData = moPLC.PLCData
            If sPLCData Is Nothing Then Return False
            'Load Collection
            For nItem = 0 To (mcolStyleID.NumberOfConveyorSnapshots - 1)
                nDataOffset = (mnStyleIDIndex * mcolStyleID.NumberOfConveyorSnapshots)
                mcolStyleID(nItem + nDataOffset).ConveyorCount.Value = CType(sPLCData(nItem), Integer)
            Next 'nItem
            Application.DoEvents()

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function

    Private Function bReadOptionRegistersFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the option Registers from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************

        Dim sPLCData() As String
        Dim sTag As String = String.Empty
        Dim nItem As Integer = 0

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "OptionRegisters"

        Try
            'Read Plant Style data from PLC
            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
            sPLCData = moPLC.PLCData
            If sPLCData Is Nothing Then Return False
            'Load Collection
            Dim nMax As Integer = mcolOptions.Count - 1
            If UBound(sPLCData) < nMax Then
                nMax = UBound(sPLCData)
            End If
            For nItem = 0 To nMax
                mcolOptions(nItem).OptionRegister.Value = CType(sPLCData(nItem), Integer)
            Next
            Application.DoEvents()

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function

    Private Function bReadPhotoeyeIgnoreFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the photoeye ignore numbers from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '    07/08/13  MSW     Move the style ID tables to XML
        '********************************************************************************************

        Dim sPLCData() As String
        Dim sTag As String = String.Empty
        Dim nItem As Integer = 0
        Dim nDataOffset As Integer

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "PhotoeyeIgnorePattern" & mnStyleIDIndex + 1

        Try
            'Read conveyor position data from PLC
            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
            sPLCData = moPLC.PLCData
            If sPLCData Is Nothing Then Return False
            'Load Collection
            For nItem = 0 To (mcolStyleID.NumberOfConveyorSnapshots - 1)
                nDataOffset = (mnStyleIDIndex * mcolStyleID.NumberOfConveyorSnapshots)
                mcolStyleID(nItem + nDataOffset).PhotoeyeIgnore.Value = CType(sPLCData(nItem), Integer)
            Next
            Application.DoEvents()

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function

    Private Function bReadStyleRegistersFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the style Registers from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************

        Dim sPLCData() As String
        Dim sTag As String = String.Empty
        Dim nItem As Integer = 0

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRegisters"

        Try
            'Read Plant Style data from PLC
            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
            sPLCData = moPLC.PLCData
            If sPLCData Is Nothing Then Return False
            'Load Collection
            For nItem = 0 To UBound(sPLCData)
                mcolStyles(nItem).StyleRegister.Value = CType(sPLCData(nItem), Integer)
            Next
            Application.DoEvents()

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function

    Private Function bReadSunroofPhotoeyeIgnoreFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the photoeye ignore numbers from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '    07/08/13  MSW     Move the style ID tables to XML
        '********************************************************************************************

        Dim sPLCData() As String
        Dim sTag As String = String.Empty
        Dim nItem As Integer = 0
        Dim nDataOffset As Integer

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "SunroofPhotoeyeIgnorePattern" & mnStyleIDIndex + 1

        Try
            'Read conveyor position data from PLC
            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
            sPLCData = moPLC.PLCData
            If sPLCData Is Nothing Then Return False
            'Load Collection
            For nItem = 0 To (mcolStyleID.NumberOfConveyorSnapshots - 1)
                nDataOffset = (mnStyleIDIndex * mcolStyleID.NumberOfConveyorSnapshots)
                mcolStyleID(nItem + nDataOffset).SunroofPhotoeyeIgnore.Value = CType(sPLCData(nItem), Integer)
            Next
            Application.DoEvents()

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bReadNonTrkStyleIDFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the photoeye pattern numbers from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************

        Dim sPLCData() As String
        Dim sTag As String = String.Empty
        Dim nItem As Integer = 0
        sTag = mcolZones.ActiveZone.PLCTagPrefix & "PhotoeyePattern"
        Try
            'Read conveyor position data from PLC
            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form

            sPLCData = moPLC.PLCData
            If sPLCData Is Nothing Then Return False
            'Load Collection
            mcolStyleID(0).PhotoeyePattern.Value = CType(sPLCData(mnStyleIDIndex), Integer)
            Application.DoEvents()

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function

    Private Function bReadPhotoeyePatternFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the photoeye pattern numbers from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '    07/08/13  MSW     Move the style ID tables to XML
        '********************************************************************************************

        Dim sPLCData() As String
        Dim sTag As String = String.Empty
        Dim nItem As Integer = 0
        Dim nDataOffset As Integer

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "PhotoeyePattern" & mnStyleIDIndex + 1

        Try
            'Read conveyor position data from PLC
            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form

            sPLCData = moPLC.PLCData
            If sPLCData Is Nothing Then Return False
            'Load Collection
            For nItem = 0 To (mcolStyleID.NumberOfConveyorSnapshots - 1)
                nDataOffset = (mnStyleIDIndex * mcolStyleID.NumberOfConveyorSnapshots)
                mcolStyleID(nItem + nDataOffset).PhotoeyePattern.Value = CType(sPLCData(nItem), Integer)
            Next
            Application.DoEvents()

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function

    Private Function bReadSunroofPhotoeyePatternFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the photoeye pattern numbers from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '    07/08/13  MSW     Move the style ID tables to XML
        '********************************************************************************************

        Dim sPLCData() As String
        Dim sTag As String = String.Empty
        Dim nItem As Integer = 0
        Dim nDataOffset As Integer

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "SunroofPhotoeyePattern" & mnStyleIDIndex + 1

        Try
            'Read conveyor position data from PLC
            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form

            sPLCData = moPLC.PLCData
            If sPLCData Is Nothing Then Return False
            'Load Collection
            For nItem = 0 To (mcolStyleID.NumberOfConveyorSnapshots - 1)
                nDataOffset = (mnStyleIDIndex * mcolStyleID.NumberOfConveyorSnapshots)
                mcolStyleID(nItem + nDataOffset).SunroofPhotoeyePattern.Value = CType(sPLCData(nItem), Integer)
            Next
            Application.DoEvents()

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function

    Private Function bReadPlantAsciiStylesFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the plant Style numbers from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************

        Dim sPLCData() As String
        Dim sTag As String = String.Empty
        Dim nItem As Integer = 0

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "PlantAsciiStyles"

        Try
            'Read Plant Style data from PLC
            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
            sPLCData = moPLC.PLCData
            If sPLCData Is Nothing Then Return False
            'Load Collection

            Dim nMax As Integer = mcolStyles.Count - 1
            If UBound(sPLCData) < nMax Then
                nMax = UBound(sPLCData)
            End If
            For nItem = 0 To nMax
                mcolStyles(nItem).PlantNumber.Text = _
                            mMathFunctions.CvIntegerToASCII(CInt(sPLCData(nItem)), mcolStyles.PlantAsciiMaxLength)
            Next

            Application.DoEvents()

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bReadPlantOptionFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the plant Option numbers from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Dim sPLCData() As String
        Dim sTag As String = String.Empty
        Dim nItem As Integer = 0

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "PlantOptions"

        Try
            'Read Plant Style data from PLC
            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
            sPLCData = moPLC.PLCData
            If sPLCData Is Nothing Then Return False
            'Load Collection

            Dim nMax As Integer = mcolOptions.Count - 1
            If UBound(sPLCData) < nMax Then
                nMax = UBound(sPLCData)
            End If
            For nItem = 0 To nMax
                mcolOptions(nItem).PlantNumber.Text = sPLCData(nItem)
            Next
            Application.DoEvents()

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bReadPlantStylesFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the plant Style numbers from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        ' 12/13/11      RJO     Added support for Sealer combined Style/Option
        '********************************************************************************************

        Dim sPLCData() As String
        Dim sPLCData2() As String 'NVSP 1000 Styles
        Dim sPLCData3() As String 'NVSP 1000 Styles
        Dim sPLCData4() As String 'NVSP 1000 Styles
        Dim sPLCData5() As String 'NVSP 1000 Styles
        Dim sPLCData6() As String 'NVSP 1000 Styles
        Dim sPLCData7() As String 'NVSP 1000 Styles
        Dim sPLCData8() As String 'NVSP 1000 Styles
        Dim sPLCData9() As String 'NVSP 1000 Styles
        Dim sPLCData10() As String 'NVSP 1000 Styles

        Dim sTag As String = String.Empty
        Dim nItem As Integer = 0

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "PlantStyles1"
        moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
        sPLCData = moPLC.PLCData

        'NVSP to fix when requested for more than 100 styles
        sTag = mcolZones.ActiveZone.PLCTagPrefix & "PlantStyles2"
        moPLC.TagName = sTag
        sPLCData2 = moPLC.PLCData

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "PlantStyles3"
        moPLC.TagName = sTag
        sPLCData3 = moPLC.PLCData

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "PlantStyles4"
        moPLC.TagName = sTag
        sPLCData4 = moPLC.PLCData

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "PlantStyles5"
        moPLC.TagName = sTag
        sPLCData5 = moPLC.PLCData

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "PlantStyles6"
        moPLC.TagName = sTag
        sPLCData6 = moPLC.PLCData

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "PlantStyles7"
        moPLC.TagName = sTag
        sPLCData7 = moPLC.PLCData

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "PlantStyles8"
        moPLC.TagName = sTag
        sPLCData8 = moPLC.PLCData

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "PlantStyles9"
        moPLC.TagName = sTag
        sPLCData9 = moPLC.PLCData

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "PlantStyles10"
        moPLC.TagName = sTag
        sPLCData10 = moPLC.PLCData

        sPLCData = CType(MergeArrays(sPLCData, sPLCData2), String())
        sPLCData = CType(MergeArrays(sPLCData, sPLCData3), String())
        sPLCData = CType(MergeArrays(sPLCData, sPLCData4), String())
        sPLCData = CType(MergeArrays(sPLCData, sPLCData5), String())
        sPLCData = CType(MergeArrays(sPLCData, sPLCData6), String())
        sPLCData = CType(MergeArrays(sPLCData, sPLCData7), String())
        sPLCData = CType(MergeArrays(sPLCData, sPLCData8), String())
        sPLCData = CType(MergeArrays(sPLCData, sPLCData9), String())
        sPLCData = CType(MergeArrays(sPLCData, sPLCData10), String())

        Try
            'Read Plant Style data from PLC
            If sPLCData Is Nothing Then Return False
            'Load Collection
            Dim nMax As Integer = mcolStyles.Count - 1
            If UBound(sPLCData) < nMax Then
                nMax = UBound(sPLCData)
            End If
            For nItem = 0 To nMax
                If mcolStyles.CombinedStyleOption Then 'RJO 12/13/11
                    Dim nStyle As Integer = CType(sPLCData(nItem), Integer) \ 100
                    Dim nOption As Integer = CType(sPLCData(nItem), Integer) Mod 100

                    mcolStyles(nItem).PlantNumber.Text = nStyle.ToString
                    mcolStyles(nItem).OptionNumber.Text = nOption.ToString
                Else
                    mcolStyles(nItem).PlantNumber.Text = sPLCData(nItem).ToString
                End If
            Next
            Application.DoEvents()

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bReadRbtsReqdOptionFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the Options Robots required data from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Dim sPLCData() As String
        Dim sTag As String = String.Empty
        Dim nItem As Integer = 0

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "OptionsRobotsRequired"

        Try
            'Read robots required data from PLC
            moPLC.TagName = sTag
            sPLCData = moPLC.PLCData
            If sPLCData Is Nothing Then Return False

            'Load Collection
            Dim nMax As Integer = mcolOptions.Count - 1
            If UBound(sPLCData) < nMax Then
                nMax = UBound(sPLCData)
            End If
            For nItem = 0 To nMax
                mcolOptions(nItem).RobotsRequired.Value = CType(sPLCData(nItem), Integer)
            Next
            Application.DoEvents()

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            Return False
        End Try

    End Function
    Private Function bReadRbtsReqdRepairFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the Repairs Robots required data from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Dim sPLCData() As String
        Dim nUB As Integer = mcolControllers.Count - 1
        Dim sTag As String = String.Empty
        Dim oZone As clsZone = mcolZones.ActiveZone
        Dim nRobot As Integer = 0
        Dim nPanel As Integer
        Dim sPLCData1 As String()
        Dim sPLCData2 As String()
        Try
            Dim nLength As Integer = 0
            If mcolZones.ActiveZone.DegradeRepair Then
                nLength = cboRobot.Items.Count * mcolZones.ActiveZone.RepairPanels
            Else
                nLength = mcolZones.ActiveZone.RepairPanels
            End If
            For nStyle As Integer = 0 To cboStyle.Items.Count - 1
                sTag = oZone.PLCTagPrefix & "S" & nStyle.ToString & "RepairRobotsRequired"

                moPLC.TagName = sTag
                sPLCData1 = moPLC.PLCData
                Application.DoEvents()
                sTag = sTag & "2"
                ReDim mnRepairLinkLength(0)
                mnRepairLinkLength(0) = sPLCData1.GetUpperBound(0) + 1
                If mnRepairLinkLength(0) < nLength Then
                    moPLC.TagName = sTag
                    sPLCData2 = moPLC.PLCData
                    Application.DoEvents()
                    sPLCData = CType(MergeArrays(sPLCData1, sPLCData2), String())
                    ReDim Preserve mnRepairLinkLength(1)
                    mnRepairLinkLength(1) = sPLCData2.GetUpperBound(0) + 1
                Else
                    sPLCData = sPLCData1
                End If

                If sPLCData Is Nothing Then Return False
                If UBound(sPLCData) < (nLength - 1) Then Return False

                Dim nMaxDegrade As Integer = 0
                If mcolZones.ActiveZone.DegradeRepair Then
                    nMaxDegrade = cboRobot.Items.Count - 1
                End If
                For nDegrade As Integer = 0 To nMaxDegrade
                    Dim nDegradeOffset As Integer = nDegrade * mcolZones.ActiveZone.RepairPanels
                    For nPanel = 0 To mcolZones.ActiveZone.RepairPanels - 1
                        mcolStyles(nStyle).PlantSpecific.DegradeRepairPanels.Item(nDegrade).Item(nPanel).RobotsRequired = CInt(sPLCData(nDegradeOffset + nPanel))
                    Next

                Next

            Next

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            Return False
        End Try

    End Function
    Private Function bReadRbtsReqdStylesFromPLC() As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the Styles Robots required data from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        ' 07/23/14      RJO     Big Changes to support more degrade modes than robots
        '********************************************************************************************
        Dim nData() As Integer
        Dim sPLCData() As String
        Dim sTag As String = String.Empty
        Dim nItem As Integer = 0
        Dim sPLCData2() As String 'NVSP 1000 Styles
        Dim sPLCData3() As String 'NVSP 1000 Styles
        Dim sPLCData4() As String 'NVSP 1000 Styles
        Dim sPLCData5() As String 'NVSP 1000 Styles
        Dim sPLCData6() As String 'NVSP 1000 Styles
        Dim sPLCData7() As String 'NVSP 1000 Styles
        Dim sPLCData8() As String 'NVSP 1000 Styles
        Dim sPLCData9() As String 'NVSP 1000 Styles
        Dim sPLCData10() As String 'NVSP 1000 Styles

        sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired1"

        Try
            Dim nLength As Integer = 0
            Dim nNumOfDegrades As Integer = mcolZones.ActiveZone.NumberOfDegrades
            Dim nMaxStyles As Integer = mcolStyles.Count

            If mcolZones.ActiveZone.DegradeStyleRbtsReq Then
                nLength = nNumOfDegrades * nMaxStyles
            Else
                nLength = nMaxStyles ' - 1 'RJO 07/23/14
            End If

            'RJO 07/23/14
            'sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired"

            'moPLC.TagName = sTag
            'sPLCData1 = moPLC.PLCData
            'Application.DoEvents()
            'sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired2"
            'ReDim mnStyleRbtRqdLinkLength(0)
            'mnStyleRbtRqdLinkLength(0) = sPLCData1.GetUpperBound(0) + 1
            'If mnStyleRbtRqdLinkLength(0) < nLength Then
            '    moPLC.TagName = sTag
            '    sPLCData2 = moPLC.PLCData
            '    Application.DoEvents()
            '    sPLCData = CType(MergeArrays(sPLCData1, sPLCData2), String())
            '    ReDim Preserve mnStyleRbtRqdLinkLength(1)
            '    mnStyleRbtRqdLinkLength(1) = sPLCData2.GetUpperBound(0) + 1
            '    sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired3"
            '    mnStyleRbtRqdLinkLength(1) = sPLCData2.GetUpperBound(0) + 1
            '    If mnStyleRbtRqdLinkLength(0) + mnStyleRbtRqdLinkLength(1) < nLength Then
            '        moPLC.TagName = sTag
            '        sPLCData3 = moPLC.PLCData
            '        Application.DoEvents()
            '        sPLCData = CType(MergeArrays(sPLCData, sPLCData3), String())
            '        ReDim Preserve mnStyleRbtRqdLinkLength(2)
            '        mnStyleRbtRqdLinkLength(2) = sPLCData3.GetUpperBound(0) + 1

            '        sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired4"
            '        mnStyleRbtRqdLinkLength(2) = sPLCData3.GetUpperBound(0) + 1
            '        If mnStyleRbtRqdLinkLength(0) + mnStyleRbtRqdLinkLength(1) + mnStyleRbtRqdLinkLength(2) < nLength Then
            '            moPLC.TagName = sTag
            '            sPLCData4 = moPLC.PLCData
            '            Application.DoEvents()
            '            sPLCData = CType(MergeArrays(sPLCData, sPLCData4), String())
            '            ReDim Preserve mnStyleRbtRqdLinkLength(3)
            '            mnStyleRbtRqdLinkLength(3) = sPLCData4.GetUpperBound(0) + 1


            '            sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired5"
            '            mnStyleRbtRqdLinkLength(3) = sPLCData4.GetUpperBound(0) + 1
            '            If mnStyleRbtRqdLinkLength(0) + mnStyleRbtRqdLinkLength(1) + mnStyleRbtRqdLinkLength(2) + _
            '                mnStyleRbtRqdLinkLength(3) < nLength Then
            '                moPLC.TagName = sTag
            '                sPLCData5 = moPLC.PLCData
            '                Application.DoEvents()
            '                sPLCData = CType(MergeArrays(sPLCData, sPLCData5), String())
            '                ReDim Preserve mnStyleRbtRqdLinkLength(4)
            '                mnStyleRbtRqdLinkLength(4) = sPLCData5.GetUpperBound(0) + 1

            '            Else
            '                'sPLCData = sPLCData4
            '            End If

            '        Else
            '            'sPLCData = sPLCData3
            '        End If

            '    Else
            '        'sPLCData = sPLCData2
            '    End If

            'Else
            '    sPLCData = sPLCData1
            'End If

            'RJO 07/23/14
            ReDim nData(nLength - 1)

            If mcolZones.ActiveZone.DegradeStyleRbtsReq Then
                For nItem = 1 To nMaxStyles
                    'Read the robot enables for each degrade mode for this style
                    sPLCData = Nothing
                    moPLC.TagName = sTag & nItem.ToString
                    sPLCData = moPLC.PLCData
                    Application.DoEvents()

                    'Make sure we have good data
                    If sPLCData Is Nothing Then Return False
                    If UBound(sPLCData) < (nNumOfDegrades - 1) Then Return False

                    'Convert the data we read to integers and copy it to the proper place in the nData array
                    Dim nPointer As Integer = (nItem - 1) * nNumOfDegrades

                    For nIndex As Integer = 0 To nNumOfDegrades - 1
                        nData(nPointer + nIndex) = CType(sPLCData(nIndex), Integer)
                    Next 'nIndex
                Next 'nItem
            Else
                'Read the robot enables
                moPLC.TagName = sTag
                sPLCData = moPLC.PLCData
                Application.DoEvents()

                sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired2"
                moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
                sPLCData2 = moPLC.PLCData

                'NVSP to fix when requested for more than 100 styles
                sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired3"
                moPLC.TagName = sTag
                sPLCData3 = moPLC.PLCData

                sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired4"
                moPLC.TagName = sTag
                sPLCData4 = moPLC.PLCData

                sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired5"
                moPLC.TagName = sTag
                sPLCData5 = moPLC.PLCData

                sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired6"
                moPLC.TagName = sTag
                sPLCData6 = moPLC.PLCData

                sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired7"
                moPLC.TagName = sTag
                sPLCData7 = moPLC.PLCData

                sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired8"
                moPLC.TagName = sTag
                sPLCData8 = moPLC.PLCData

                sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired9"
                moPLC.TagName = sTag
                sPLCData9 = moPLC.PLCData

                sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired10"
                moPLC.TagName = sTag
                sPLCData10 = moPLC.PLCData

                sPLCData = CType(MergeArrays(sPLCData, sPLCData2), String())
                sPLCData = CType(MergeArrays(sPLCData, sPLCData3), String())
                sPLCData = CType(MergeArrays(sPLCData, sPLCData4), String())
                sPLCData = CType(MergeArrays(sPLCData, sPLCData5), String())
                sPLCData = CType(MergeArrays(sPLCData, sPLCData6), String())
                sPLCData = CType(MergeArrays(sPLCData, sPLCData7), String())
                sPLCData = CType(MergeArrays(sPLCData, sPLCData8), String())
                sPLCData = CType(MergeArrays(sPLCData, sPLCData9), String())
                sPLCData = CType(MergeArrays(sPLCData, sPLCData10), String())

                'Make sure we have good data
                If sPLCData Is Nothing Then Return False
                If UBound(sPLCData) < (nMaxStyles - 1) Then Return False

                'Convert the data we read to integers and copy it to the nData array
                For nIndex As Integer = 0 To nMaxStyles - 1
                    nData(nIndex) = CType(sPLCData(nIndex), Integer)
                Next 'nIndex
            End If

            'Update the Styles collection
            If mcolZones.ActiveZone.DegradeStyleRbtsReq Then
                For nItem = 0 To nLength - 1
                    Dim nStyle As Integer = nItem \ nNumOfDegrades
                    Dim nDegrade As Integer = nItem - ((nStyle) * (nNumOfDegrades))
                    mcolStyles(nStyle).DegradeRobotsRequired(nDegrade).Value = nData(nItem)
                Next
            Else
                For nItem = 0 To nLength - 1 ' MAS 20160906
                    mcolStyles(nItem).RobotsRequired.Value = nData(nItem)
                Next
            End If

            Application.DoEvents()

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            Return False
        End Try

    End Function
    Private Function bSaveOptionsToSQLDB() As Boolean
        '********************************************************************************************
        'Description:  Save data to SQL Database
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/04/12  RJO     Bug fix to change log reporting for Robots Required.
        '********************************************************************************************
        Dim oO As clsSysOption = Nothing
        Dim nZone As Integer = 0
        Dim nItem As Integer = 0
        Dim oZone As clsZone = mcolZones.ActiveZone

        Try
            'If change was due to Restore from DB or PLC, note this in the change log
            If Strings.Len(msRestoreString) > 0 Then
                Call subUpdateChangeLog(oZone.ZoneNumber)
                msRestoreString = String.Empty
            End If
            Const sXMLFILE As String = "SystemOptions"
            Const sXMLTABLE As String = "SystemOptions"
            Const sXMLNODE As String = "Option"

            Dim sTopic As String = String.Empty
            Dim oMainNode As XmlNode = Nothing
            Dim oZoneNode As XmlNode = Nothing
            Dim oNodeList As XmlNodeList = Nothing
            Dim oXMLDoc As New XmlDocument
            Dim bRemoteZone As Boolean = False
            Dim sXMLFilePath As String = String.Empty
            Try
                If GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, sXMLFILE & ".XML") Then

                    oXMLDoc.Load(sXMLFilePath)

                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    oZoneNode = oMainNode.SelectSingleNode("//" & oZone.Name)
                    If oZoneNode Is Nothing Then
                        oZoneNode = oXMLDoc.CreateElement(oZone.Name)
                        oMainNode.AppendChild(oZoneNode)
                        oZoneNode = oMainNode.SelectSingleNode("//" & oZone.Name)
                    End If
                Else
                    oXMLDoc = New XmlDocument
                    oXMLDoc.CreateElement(sXMLTABLE)
                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    oXMLDoc.CreateElement(oZone.Name)
                    oZoneNode = oXMLDoc.SelectSingleNode("//" & oZone.Name)
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":bLoadCollection", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
                ShowErrorMessagebox("Invalid XPath syntax: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)

            End Try
            oZoneNode.RemoveAll()
            For nItem = 0 To (mcolOptions.Count - 1)
                oO = mcolOptions.Item(nItem)

                'Build XML node
                Dim oNode As XmlNode = oXMLDoc.CreateElement(sXMLNODE)
                Dim oPNNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSO_COL_PLANTNUM, Nothing)
                oPNNode.InnerXml = oO.PlantNumber.Text
                oNode.AppendChild(oPNNode)
                Dim oFNNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSO_COL_FANUCNUM, Nothing)
                oFNNode.InnerXml = oO.FanucNumber.Value.ToString
                oNode.AppendChild(oFNNode)
                Dim oEnabNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSO_COL_ENABLE, Nothing)
                oEnabNode.InnerXml = oO.RobotsRequired.Value.ToString
                oNode.AppendChild(oEnabNode)
                Dim oDescNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSO_COL_DESC, Nothing)
                oDescNode.InnerXml = oO.Description.Text
                oNode.AppendChild(oDescNode)
                Dim oRegNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSO_REGISTER, Nothing)
                oRegNode.InnerXml = oO.OptionRegister.Value.ToString
                oNode.AppendChild(oRegNode)
                oZoneNode.AppendChild(oNode)

                'write all info

                If oO.PlantNumber.Changed Then
                    'update change log
                    Call subUpdateChangeLog(oO.PlantNumber.Text, oO.PlantNumber.OldText, _
                                           String.Empty, oZone, _
                                           lblPlantOptionCap.Text & " #" & (nItem + 1).ToString, _
                                           String.Empty)
                End If

                If oO.FanucNumber.Changed Then
                    'update change log
                    Call subUpdateChangeLog(oO.FanucNumber.Value.ToString, _
                                        oO.FanucNumber.OldValue.ToString, String.Empty, oZone, _
                                        lblFanucOptionCap.Text & " #" & (nItem + 1).ToString, _
                                        String.Empty)
                End If

                If oO.Description.Changed Then
                    'update change log
                    Call subUpdateChangeLog(oO.Description.Text, oO.Description.OldText, _
                                          String.Empty, oZone, _
                                          lblOptionDescCap.Text & " #" & (nItem + 1).ToString, _
                                          String.Empty)
                End If

                If oO.RobotsRequired.Changed Then
                    Dim sNewValue As String = StrReverse(mMathFunctions.CvBin(oO.RobotsRequired.Value, _
                            mnNumberofRobots + 1))
                    Dim sOldValue As String = StrReverse(mMathFunctions.CvBin(oO.RobotsRequired.OldValue, _
                            mnNumberofRobots + 1))
                    'Make sure we get the proper bits 'RJO 10/04/12
                    If mcolZones.ActiveZone.RobotsRequiredStartingBit = 1 Then
                        sNewValue = Strings.Right(sNewValue, mnNumberofRobots)
                        sOldValue = Strings.Right(sOldValue, mnNumberofRobots)
                    Else
                        sNewValue = Strings.Left(sNewValue, mnNumberofRobots)
                        sOldValue = Strings.Left(sOldValue, mnNumberofRobots)
                    End If

                    'update change log
                    Call subUpdateChangeLog(sNewValue, sOldValue, String.Empty, oZone, _
                                        lblOptionRobotsReqCap.Text & " #" & (nItem + 1).ToString, _
                                        String.Empty)
                End If
                If oO.OptionRegister.Changed Then
                    'update change log
                    Call subUpdateChangeLog(oO.OptionRegister.Value.ToString, oO.OptionRegister.OldValue.ToString, _
                                        String.Empty, oZone, _
                                        lblOptionRegister.Text & " #" & (nItem + 1).ToString, _
                                        String.Empty)
                End If

            Next

            mcolOptions.Update()

            oXMLDoc.Save(sXMLFilePath)

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bSaveOptionsToPLC() As Boolean
        '********************************************************************************************
        'Description:  Save data stored on PLC.
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/30/10  MSW     Change to look at ascii enable in option/style/color config
        ' 10/05/10  MSW     Clear out extra bits in the robots required word
        ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
        '********************************************************************************************
        Dim bSaveSuccess As Boolean = False
        Dim nOptionCount As Integer = 0
        Dim nIndex As Integer = 0
        Dim sData() As String
        Dim sTag As String = String.Empty
        Dim nUpperBound As Integer = 0

        Try

            moPLC.ZoneName = mcolZones.ActiveZone.Name
            nUpperBound = mcolOptions.MaxOptions - 1

            ReDim sData(nUpperBound)

            For nIndex = 0 To nUpperBound
                sData(nIndex) = mcolOptions(nIndex).PlantNumber.Text
            Next

            sTag = mcolZones.ActiveZone.PLCTagPrefix & "PlantOptions"
            moPLC.TagName = sTag
            ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
            Dim sTmp() As String = moPLC.PLCData
            If sTmp.GetUpperBound(0) >= sData.GetUpperBound(0) Then
                For nItem As Integer = 0 To sData.GetUpperBound(0)
                    sTmp(nItem) = sData(nItem)
                Next
                moPLC.PLCData = sTmp
            Else
                'Let the PLC form complain about it if the PLC link is shorter than the DB setup
                moPLC.PLCData = sData
            End If

            'room to pop up error if it happens
            Application.DoEvents()

            ReDim sData(nUpperBound)
            For nIndex = 0 To nUpperBound
                sData(nIndex) = mcolOptions(nIndex).FanucNumber.Value.ToString
            Next
            If mcolOptions.UseAscii Then
                For nCounter As Integer = 0 To sData.GetUpperBound(0)
                    'If sData(nCounter).Length < 4 Then
                    '    sData(nCounter) = "    " & sData(nCounter)
                    'End If
                    'If sData(nCounter).Length > 4 Then
                    '    sData(nCounter) = sData(nCounter).Substring(sData(nCounter).Length - 4)
                    'End If
                    'Dim nDigit As Integer = sData(nCounter).Length - 1
                    ''Assume DINT type, max 4 char
                    'Dim nMult As Integer = 1
                    'Dim nTmp As Integer = 0
                    'Do While nDigit >= 0
                    '    nTmp = nTmp + Asc(sData(nCounter).Substring(nDigit, 1)) * nMult
                    '    If nDigit > 0 Then
                    '        nMult = nMult * 256
                    '    End If
                    '    nDigit = nDigit - 1
                    'Loop
                    'sData(nCounter) = nTmp.ToString
                    sData(nCounter) = mMathFunctions.CvASCIIToInteger(sData(nCounter), mcolOptions.PlantAsciiMaxLength).ToString
                Next
            End If
            sTag = mcolZones.ActiveZone.PLCTagPrefix & "FanucOptions"
            moPLC.TagName = sTag
            ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
            sTmp = moPLC.PLCData
            If sTmp.GetUpperBound(0) >= sData.GetUpperBound(0) Then
                For nItem As Integer = 0 To sData.GetUpperBound(0)
                    sTmp(nItem) = sData(nItem)
                Next
                moPLC.PLCData = sTmp
            Else
                'Let the PLC form complain about it if the PLC link is shorter than the DB setup
                moPLC.PLCData = sData
            End If
            'room to pop up error if it happens
            Application.DoEvents()

            ReDim sData(nUpperBound)
            Dim nMax As Integer = mcolArms(mcolArms.Count - 1).RobotNumber
            Dim nMask As Integer = CType(((2 ^ nMax) - 1) * (2 ^ mcolZones.ActiveZone.RobotsRequiredStartingBit), Integer)
            'Dim nMask As Integer = CType(2 ^ (mcolArms.Count + mcolZones.ActiveZone.RobotsRequiredStartingBit), Integer) - 1
            For nIndex = 0 To nUpperBound
                mcolOptions(nIndex).RobotsRequired.Value = mcolOptions(nIndex).RobotsRequired.Value And nMask
                sData(nIndex) = mcolOptions(nIndex).RobotsRequired.Value.ToString
            Next
            sTag = mcolZones.ActiveZone.PLCTagPrefix & "OptionsRobotsRequired"
            moPLC.TagName = sTag
            ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
            sTmp = moPLC.PLCData
            If sTmp.GetUpperBound(0) >= sData.GetUpperBound(0) Then
                For nItem As Integer = 0 To sData.GetUpperBound(0)
                    sTmp(nItem) = sData(nItem)
                Next
                moPLC.PLCData = sTmp
            Else
                'Let the PLC form complain about it if the PLC link is shorter than the DB setup
                moPLC.PLCData = sData
            End If

            If mcolOptions.OptionRegisters Then
                ReDim sData(nUpperBound)
                For nIndex = 0 To nUpperBound
                    sData(nIndex) = mcolOptions(nIndex).OptionRegister.Value.ToString
                Next
                sTag = mcolZones.ActiveZone.PLCTagPrefix & "OptionRegisters"
                moPLC.TagName = sTag
                ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
                sTmp = moPLC.PLCData
                If sTmp.GetUpperBound(0) >= sData.GetUpperBound(0) Then
                    For nItem As Integer = 0 To sData.GetUpperBound(0)
                        sTmp(nItem) = sData(nItem)
                    Next
                    moPLC.PLCData = sTmp
                Else
                    'Let the PLC form complain about it if the PLC link is shorter than the DB setup
                    moPLC.PLCData = sData
                End If
            End If

            'room to pop up error if it happens
            Application.DoEvents()


            If Not mbPLCFail Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bSaveDegradeToGUI() As Boolean
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
        'no degrade in database for now
        Return True
    End Function
    Private Function bSaveIntrusionToGUI() As Boolean
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
        bSaveStylesToSQLDB()
        Return True
    End Function
    Private Function bSaveDegradeRbtsReqToGUI() As Boolean
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
        Dim nZone As Integer = 0
        Dim nItem As Integer = 0
        Dim oZone As clsZone = mcolZones.ActiveZone

        Try
            'If change was due to Restore from DB or PLC, note this in the change log
            If Strings.Len(msRestoreString) > 0 Then
                Call subUpdateChangeLog(oZone.ZoneNumber)
                msRestoreString = String.Empty
            End If
            Const sXMLFILE As String = "SystemDegrade"
            Const sXMLTABLE As String = "SystemDegrade"
            Const sXMLNODE As String = "Degrade"

            Dim sTopic As String = String.Empty
            Dim oMainNode As XmlNode = Nothing
            Dim oZoneNode As XmlNode = Nothing
            Dim oXMLDoc As New XmlDocument
            Dim bRemoteZone As Boolean = False
            Dim sXMLFilePath As String = String.Empty
            Try
                If GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, sXMLFILE & ".XML") Then

                    oXMLDoc.Load(sXMLFilePath)

                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    oZoneNode = oMainNode.SelectSingleNode("//" & oZone.Name)
                    If oZoneNode Is Nothing Then
                        oZoneNode = oXMLDoc.CreateElement(oZone.Name)
                        oMainNode.AppendChild(oZoneNode)
                        oZoneNode = oMainNode.SelectSingleNode("//" & oZone.Name)
                    End If
                Else
                    oXMLDoc = New XmlDocument
                    oXMLDoc.CreateElement(sXMLTABLE)
                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    oZoneNode = oXMLDoc.CreateNode(XmlNodeType.Element, oZone.Name, Nothing)
                    oMainNode.AppendChild(oZoneNode)
                End If

            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":bSaveRepairsToGUI", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
                ShowErrorMessagebox("Invalid XPath syntax: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)
            End Try
            oZoneNode.RemoveAll()
            For Each oStyle As clsSysStyle In mcolStyles
                'Build XML node
                Dim oStyleNode As XmlNode = oXMLDoc.CreateElement("Style" & oStyle.PlantNumber.Text)
                For nDegrade As Integer = 0 To oZone.NumberOfDegrades - 1
                    Dim oPanelNode As XmlNode = oXMLDoc.CreateElement(sXMLNODE)

                    Dim oPNNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "DegradeNumber", Nothing)
                    oPNNode.InnerXml = nDegrade.ToString
                    oPanelNode.AppendChild(oPNNode)
                    Dim oDescNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Enable", Nothing)
                    oDescNode.InnerXml = oStyle.DegradeRobotsRequired(nDegrade).Value.ToString
                    oPanelNode.AppendChild(oDescNode)

                    oStyleNode.AppendChild(oPanelNode)

                    If oStyle.DegradeRobotsRequired(nDegrade).Changed Then
                        'update change log
                        Call subUpdateChangeLog(oStyle.DegradeRobotsRequired(nDegrade).Value.ToString, _
                                        oStyle.DegradeRobotsRequired(nDegrade).OldValue.ToString, _
                                        String.Empty, oZone, _
                                        gpsRM.GetString("psPLANT_STYLE_CAP", DisplayCulture) & " " & oStyle.PlantNumber.Text & _
                                        gpsRM.GetString("psDEGRADE_NUMBER_CAP", DisplayCulture) & _
                                        " " & nDegrade.ToString & " " & lblRepairDescCap.Text, _
                                        String.Empty)

                        oStyle.DegradeRobotsRequired(nDegrade).Update()
                    End If

                Next
                oZoneNode.AppendChild(oStyleNode)
            Next


            'write all info


            oXMLDoc.Save(sXMLFilePath)

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try




    End Function

    Private Function bSaveRepairsToGUI() As Boolean
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
        Dim nZone As Integer = 0
        Dim nItem As Integer = 0
        Dim oZone As clsZone = mcolZones.ActiveZone

        Try
            'If change was due to Restore from DB or PLC, note this in the change log
            If Strings.Len(msRestoreString) > 0 Then
                Call subUpdateChangeLog(oZone.ZoneNumber)
                msRestoreString = String.Empty
            End If
            Const sXMLFILE As String = "SystemRepairPanels"
            Const sXMLTABLE As String = "SystemRepairPanels"
            Const sXMLNODE As String = "SystemRepairPanel"

            Dim sTopic As String = String.Empty
            Dim oMainNode As XmlNode = Nothing
            Dim oZoneNode As XmlNode = Nothing
            Dim oXMLDoc As New XmlDocument
            Dim bRemoteZone As Boolean = False
            Dim sXMLFilePath As String = String.Empty
            Try
                If GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, sXMLFILE & ".XML") Then

                    oXMLDoc.Load(sXMLFilePath)

                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    oZoneNode = oMainNode.SelectSingleNode("//" & oZone.Name)
                    If oZoneNode Is Nothing Then
                        oZoneNode = oXMLDoc.CreateElement(oZone.Name)
                        oMainNode.AppendChild(oZoneNode)
                        oZoneNode = oMainNode.SelectSingleNode("//" & oZone.Name)
                    End If
                Else
                    oXMLDoc = New XmlDocument
                    oXMLDoc.CreateElement(sXMLTABLE)
                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    oZoneNode = oXMLDoc.CreateNode(XmlNodeType.Element, oZone.Name, Nothing)
                    oMainNode.AppendChild(oZoneNode)
                End If

            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":bSaveRepairsToGUI", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
                ShowErrorMessagebox("Invalid XPath syntax: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)
            End Try
            oZoneNode.RemoveAll()
            For Each oStyle As clsSysStyle In mcolStyles
                'Build XML node
                Dim oStyleNode As XmlNode = oXMLDoc.CreateElement("Style" & oStyle.PlantNumber.Text)
                For nPanel As Integer = 1 To oZone.RepairPanels
                    Dim oPanelNode As XmlNode = oXMLDoc.CreateElement(sXMLNODE)

                    Dim oPNNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "PanelNumber", Nothing)
                    oPNNode.InnerXml = nPanel.ToString
                    oPanelNode.AppendChild(oPNNode)
                    Dim oDescNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSO_COL_DESC, Nothing)
                    oDescNode.InnerXml = oStyle.RepairPanelDescription(nPanel).Text
                    oPanelNode.AppendChild(oDescNode)

                    oStyleNode.AppendChild(oPanelNode)

                    If oStyle.RepairPanelDescription(nPanel).Changed Then
                        'update change log
                        Call subUpdateChangeLog(oStyle.RepairPanelDescription(nPanel).Text, _
                                        oStyle.RepairPanelDescription(nPanel).OldText, _
                                        String.Empty, oZone, _
                                        gpsRM.GetString("psPLANT_STYLE_CAP", DisplayCulture) & " " & oStyle.PlantNumber.Text & _
                                        gpsRM.GetString("psREPAIR_PANEL", DisplayCulture) & _
                                        " " & nPanel.ToString & " " & lblRepairDescCap.Text, _
                                        String.Empty)

                        oStyle.RepairPanelDescription(nPanel).Update()
                    End If

                Next
                oZoneNode.AppendChild(oStyleNode)
            Next


            'write all info
            

            oXMLDoc.Save(sXMLFilePath)

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try


        

    End Function
    Private Function bSaveDegradeToPLC() As Boolean
        '********************************************************************************************
        'Description:  Save data stored on PLC.
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
        '********************************************************************************************
        ' this is done on a per controller basis
        Dim nC As Integer() = CType(cboController.Tag, Integer())
        Dim nCont As Integer = nC(cboController.SelectedIndex)
        Dim oZone As clsZone = mcolZones.ActiveZone
        Dim sTag As String
        Dim sPLCData(99) As String ' 100 word link data
        Dim nPtr As Integer = 0
        Dim Culture As System.Globalization.CultureInfo = DisplayCulture

        Try


            For nStyle As Integer = 0 To 9
                For nRDown As Integer = 1 To mcolStyles.Item(nStyle).PlantSpecific.MaxDegrades
                    sPLCData(nPtr) = mcolStyles.Item(nStyle).PlantSpecific.DegradeNumber(nCont, _
                                                                            nRDown).Value.ToString
                    nPtr += 1
                Next
            Next 'nstyle


            If oZone.UseSplitShell Then
                ' by(arm)
                sTag = oZone.PLCTagPrefix & _
                                mcolArms(nCont - 1).PLCTagPrefix & "DegradeNumbers"
            Else
                'use arm 1 on controller
                sTag = oZone.PLCTagPrefix & _
                                mcolControllers(nCont - 1).Arms(0).PLCTagPrefix & "DegradeNumbers"
            End If


            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form

            ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
            Dim sTmpArray() As String = moPLC.PLCData
            If sTmpArray.GetUpperBound(0) >= sPLCData.GetUpperBound(0) Then
                For nItem As Integer = 0 To sPLCData.GetUpperBound(0)
                    If sPLCData(nItem) IsNot Nothing AndAlso sPLCData(nItem) <> String.Empty Then
                        sTmpArray(nItem) = sPLCData(nItem)
                    End If
                Next
                moPLC.PLCData = sTmpArray
            Else
                'Let the PLC form complain about it if the PLC link is shorter than the DB setup
                moPLC.PLCData = sPLCData
            End If
            '2nd 100 word link
            nPtr = 0
            For nStyle As Integer = 10 To 19
                For nRDown As Integer = 1 To mcolStyles.Item(nStyle).PlantSpecific.MaxDegrades
                    sPLCData(nPtr) = mcolStyles.Item(nStyle).PlantSpecific.DegradeNumber(nCont, _
                                                                            nRDown).Value.ToString
                    nPtr += 1
                Next
            Next 'nstyle


            If oZone.UseSplitShell Then
                ' by(arm)
                sTag = oZone.PLCTagPrefix & _
                                mcolArms(nCont - 1).PLCTagPrefix & "DegradeNumbers2"
            Else
                'use arm 1 on controller
                sTag = oZone.PLCTagPrefix & _
                                mcolControllers(nCont - 1).Arms(0).PLCTagPrefix & "DegradeNumbers2"
            End If


            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
            ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
            sTmpArray = moPLC.PLCData
            If sTmpArray.GetUpperBound(0) >= sPLCData.GetUpperBound(0) Then
                For nItem As Integer = 0 To sPLCData.GetUpperBound(0)
                    If sPLCData(nItem) IsNot Nothing AndAlso sPLCData(nItem) <> String.Empty Then
                        sTmpArray(nItem) = sPLCData(nItem)
                    End If
                Next
                moPLC.PLCData = sTmpArray
            Else
                'Let the PLC form complain about it if the PLC link is shorter than the DB setup
                moPLC.PLCData = sPLCData
            End If

            Dim sTmp As String
            If oZone.UseSplitShell Then
                sTmp = mcolArms(nCont - 1).Name
            Else
                sTmp = mcolControllers(nCont - 1).Name
            End If

            For nStyle As Integer = 0 To mcolStyles.Count - 1
                For nRDown As Integer = 1 To mcolStyles.Item(nStyle).PlantSpecific.MaxDegrades
                    If mcolStyles.Item(nStyle).PlantSpecific.DegradeNumber(nCont, nRDown).Changed Then
                        'change log
                        Dim oFTB As FocusedTextBox.FocusedTextBox = _
                            DirectCast(pnlDegrade.Controls(("ftbRobotDown" & Format(nRDown, "000"))),  _
                            FocusedTextBox.FocusedTextBox)

                        Call subUpdateChangeLog(oFTB.Text, _
                            mcolStyles.Item(nStyle).PlantSpecific.DegradeNumber(nCont, nRDown).Value.ToString, _
                            mcolStyles.Item(nStyle).PlantSpecific.DegradeNumber(nCont, nRDown).OldValue.ToString, _
                            cboPlantStyle.Text, oZone.ZoneNumber, gsCHANGE_AREA, sTmp)

                    End If
                    mcolStyles.Item(nStyle).PlantSpecific.DegradeNumber(nCont, nRDown).Update()
                Next
            Next


            Return True

        Catch ex As Exception

            Trace.WriteLine(ex.Message)
            Return False
        End Try

    End Function
    Private Function bSaveIntrusionToPLC() As Boolean
        '********************************************************************************************
        'Description:  Save data stored on PLC.
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
        ' 09/13/12  RJO     nIdx loops were using (nItem) for pointer inside loops. Update changelog
        '                   code in this routine will never execute because mcolStyles has already
        '                   been updated. Moved to bSaveStylesToSQLDB.
        '********************************************************************************************
        Dim nItem As Integer
        Dim sTag As String = String.Empty
        Dim oZone As clsZone = mcolZones.ActiveZone
        Dim nUb As Integer = oZone.MaxStyles - 1
        Dim sPLCData(nUb) As String
        'Dim sVoid As String = String.Empty 'RJO 09/13/12

        Try

            ' Mute Len data to PLC
            For nItem = 0 To nUb
                sPLCData(nItem) = mcolStyles(nItem).MuteLength.Value.ToString
            Next

            sTag = oZone.PLCTagPrefix & "PECMuteLength"
            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
            ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
            Dim sTmpArray() As String = moPLC.PLCData
            If sTmpArray.GetUpperBound(0) >= sPLCData.GetUpperBound(0) Then
                For nIdx As Integer = 0 To sPLCData.GetUpperBound(0)
                    If sPLCData(nIdx) IsNot Nothing AndAlso sPLCData(nIdx) <> String.Empty Then 'RJO 09/13/12
                        sTmpArray(nIdx) = sPLCData(nIdx)
                    End If
                Next
                moPLC.PLCData = sTmpArray
            Else
                'Let the PLC form complain about it if the PLC link is shorter than the DB setup
                moPLC.PLCData = sPLCData
            End If

            'RJO 09/13/12
            'For nItem = 0 To nUb
            '    If mcolStyles(nItem).MuteLength.Changed Then
            '        subUpdateChangeLog(mcolStyles(nItem).MuteLength.Value.ToString, _
            '            mcolStyles(nItem).MuteLength.OldValue.ToString, _
            '            mcolStyles(nItem).DisplayName, _
            '            oZone, lblEntStartCap.Text, sVoid)
            '        mcolStyles(nItem).MuteLength.Update()
            '    End If
            'Next

            '  Entrance Start data to PLC
            For nItem = 0 To nUb
                sPLCData(nItem) = mcolStyles(nItem).EntranceMuteStartCount.Value.ToString
            Next

            sTag = oZone.PLCTagPrefix & "PECEntMuteStart"
            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
            ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
            sTmpArray = moPLC.PLCData
            If sTmpArray.GetUpperBound(0) >= sPLCData.GetUpperBound(0) Then
                For nIdx As Integer = 0 To sPLCData.GetUpperBound(0)
                    If sPLCData(nIdx) IsNot Nothing AndAlso sPLCData(nIdx) <> String.Empty Then 'RJO 09/13/12
                        sTmpArray(nIdx) = sPLCData(nIdx)
                    End If
                Next
                moPLC.PLCData = sTmpArray
            Else
                'Let the PLC form complain about it if the PLC link is shorter than the DB setup
                moPLC.PLCData = sPLCData
            End If

            'RJO 09/13/12
            'For nItem = 0 To nUb
            '    If mcolStyles(nItem).EntranceMuteStartCount.Changed Then
            '        subUpdateChangeLog( _
            '            mcolStyles(nItem).EntranceMuteStartCount.Value.ToString, _
            '            mcolStyles(nItem).EntranceMuteStartCount.OldValue.ToString, _
            '            mcolStyles(nItem).DisplayName, _
            '            oZone, lblExitStartCap.Text, sVoid)
            '        mcolStyles(nItem).EntranceMuteStartCount.Update()
            '    End If
            'Next

            'Read exit Start data from PLC
            For nItem = 0 To nUb
                sPLCData(nItem) = mcolStyles(nItem).ExitMuteStartCount.Value.ToString
            Next
            sTag = oZone.PLCTagPrefix & "PECExitMuteStart"
            moPLC.TagName = sTag ' pass the tag name to public property in plccomm form
            ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
            sTmpArray = moPLC.PLCData
            If sTmpArray.GetUpperBound(0) >= sPLCData.GetUpperBound(0) Then
                For nIdx As Integer = 0 To sPLCData.GetUpperBound(0)
                    If sPLCData(nIdx) IsNot Nothing AndAlso sPLCData(nIdx) <> String.Empty Then 'RJO 09/13/11
                        sTmpArray(nIdx) = sPLCData(nIdx)
                    End If
                Next
                moPLC.PLCData = sTmpArray
            Else
                'Let the PLC form complain about it if the PLC link is shorter than the DB setup
                moPLC.PLCData = sPLCData
            End If

            'RJO 09/13/12
            'For nItem = 0 To nUb
            '    If mcolStyles(nItem).ExitMuteStartCount.Changed Then
            '        subUpdateChangeLog( _
            '            mcolStyles(nItem).ExitMuteStartCount.Value.ToString, _
            '            mcolStyles(nItem).ExitMuteStartCount.OldValue.ToString, _
            '            mcolStyles(nItem).DisplayName, _
            '            oZone, lblMuteLenCap.Text, sVoid)
            '        mcolStyles(nItem).ExitMuteStartCount.Update()
            '    End If
            'Next

            Application.DoEvents()

            Return True

        Catch ex As Exception

            Trace.WriteLine(ex.Message)
            Return False
        End Try

    End Function
    Friend Function bSaveRepairsToPLC(Optional ByVal bLog As Boolean = True) As Boolean
        '********************************************************************************************
        'Description:  Save data stored on PLC.
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/08/10  MSW     save to integer data type
        '********************************************************************************************
        Dim sTag As String = String.Empty
        Dim oZone As clsZone = mcolZones.ActiveZone
        Dim nPanel As Integer = 0
        Dim nDegrade As Integer = 0
        Dim sPLCData1 As String() = Nothing
        Dim sPLCData2 As String() = Nothing
        Dim nStyle As Integer = 0
        Try
            Dim nLength As Integer = 0
            If mcolZones.ActiveZone.DegradeRepair Then
                nLength = cboRobot.Items.Count * mcolZones.ActiveZone.RepairPanels
            Else
                nLength = mcolZones.ActiveZone.RepairPanels
            End If
            ReDim sPLCData1(mnRepairLinkLength(0) - 1)
            If mnRepairLinkLength.GetUpperBound(0) > 0 Then
                ReDim sPLCData2(mnRepairLinkLength(1) - 1)
            End If
            For nStyle = 0 To cboStyle.Items.Count - 1
                'Debug.Print("Style: " & nStyle)
                Dim nMaxDegrade As Integer = 0
                If mcolZones.ActiveZone.DegradeRepair Then
                    nMaxDegrade = cboRobot.Items.Count - 1
                End If
                For nDegrade = 0 To nMaxDegrade
                    'Debug.Print("Degrade: " & nDegrade)
                    Dim nDegradeOffset As Integer = nDegrade * mcolZones.ActiveZone.RepairPanels
                    Dim sTmp As String = String.Empty
                    For nPanel = 0 To mcolZones.ActiveZone.RepairPanels - 1
                        Dim nPLCDataIndex As Integer = nDegradeOffset + nPanel
                        If nPLCDataIndex < mnRepairLinkLength(0) Then
                            sPLCData1(nPLCDataIndex) = mcolStyles(nStyle).PlantSpecific.DegradeRepairPanels.Item(nDegrade).Item(nPanel).RobotsRequired.ToString
                        Else
                            sPLCData2(nPLCDataIndex - mnRepairLinkLength(0)) = mcolStyles(nStyle).PlantSpecific.DegradeRepairPanels.Item(nDegrade).Item(nPanel).RobotsRequired.ToString
                        End If
                        sTmp = sTmp & " " & mcolStyles(nStyle).PlantSpecific.DegradeRepairPanels.Item(nDegrade).Item(nPanel).RobotsRequired.ToString()
                    Next
                    'Debug.Print(stmp)
                Next

                sTag = oZone.PLCTagPrefix & "S" & nStyle.ToString & "RepairRobotsRequired"
                moPLC.TagName = sTag
                ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
                Dim sTmpArray() As String = moPLC.PLCData
                If sTmpArray.GetUpperBound(0) >= sPLCData1.GetUpperBound(0) Then
                    For nIdx As Integer = 0 To sPLCData1.GetUpperBound(0)
                        If sPLCData1(nIdx) IsNot Nothing AndAlso sPLCData1(nIdx) <> String.Empty Then
                            sTmpArray(nIdx) = sPLCData1(nIdx)
                        End If
                    Next
                    moPLC.PLCData = sTmpArray
                Else
                    'Let the PLC form complain about it if the PLC link is shorter than the DB setup
                    moPLC.PLCData = sPLCData1
                End If
                Application.DoEvents()
                If mnRepairLinkLength.GetUpperBound(0) > 0 Then
                    sTag = sTag & "2"
                    moPLC.TagName = sTag
                    ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
                    sTmpArray = moPLC.PLCData
                    If sTmpArray.GetUpperBound(0) >= sPLCData2.GetUpperBound(0) Then
                        For nIdx As Integer = 0 To sPLCData2.GetUpperBound(0)
                            If sPLCData2(nIdx) IsNot Nothing AndAlso sPLCData2(nIdx) <> String.Empty Then
                                sTmpArray(nIdx) = sPLCData2(nIdx)
                            End If
                        Next
                        moPLC.PLCData = sTmpArray
                    Else
                        'Let the PLC form complain about it if the PLC link is shorter than the DB setup
                        moPLC.PLCData = sPLCData2
                    End If

                    Application.DoEvents()
                End If
            Next




            'change log thingy
            Dim sName As String
            Dim o As clsSysRepairPanel
            Dim nNumRobots As Integer = 0
            If oZone.UseSplitShell Then
                nNumRobots = mcolArms.Count - 1
            Else
                nNumRobots = mcolControllers.Count - 1
            End If

            For nStyle = 0 To mcolStyles.Count - 1
                Dim nMaxDegrade As Integer = 0
                If mcolZones.ActiveZone.DegradeRepair Then
                    nMaxDegrade = cboRobot.Items.Count
                End If
                For nDegrade = 0 To nMaxDegrade
                    For nPanel = 0 To oZone.RepairPanels - 1
                        o = mcolStyles(nStyle).PlantSpecific.DegradeRepairPanels.Item(nDegrade).Item(nPanel)

                        If o.Changed Then
                            For nRobot As Integer = 0 To nNumRobots
                                If o.RobotRequired(nRobot).Changed Then
                                    If oZone.UseSplitShell Then
                                        sName = mcolArms(nRobot).Name
                                    Else
                                        sName = mcolControllers(nRobot).Name
                                    End If
                                    If bLog Then
                                        subUpdateChangeLog(o.RobotRequired(nRobot).Value.ToString, _
                                                    o.RobotRequired(nRobot).OldValue.ToString, _
                                                    mcolStyles(nStyle).DisplayName, oZone, _
                                                    gpsRM.GetString("psROB_REQ_FOR_PANEL", DisplayCulture) & " '" & _
                                                     mcolStyles(nStyle).RepairPanelDescription(nPanel + 1).Text & "' ", sName)
                                    End If
                                    o.RobotRequired(nRobot).Update()
                                End If
                            Next '' nrobot
                        End If ' o.changed
                    Next  'nPanel
                Next
            Next    'nstyle

            Return True


        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            Return False
        End Try

    End Function
    Private Function bSaveOptionsToGUI() As Boolean
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
        Try
            If Not bSaveOptionsToSQLDB() Then

            End If

            'temp till access is gone --------------
            'If Not bSaveOptionsToAccessDB() Then

            'End If
            '---------------------------------------

            Return True

        Catch ex As Exception
            Return False
        End Try
    End Function
    Private Function bSaveStylesToGUI() As Boolean
        '********************************************************************************************
        'Description:  Save data stored on GUI in database, xml file etc.
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 07/08/13  MSW     Move the style ID tables to XML
        '********************************************************************************************
        Try
            If Not bSaveStylesToSQLDB() Then

            End If

            If mcolZones.ActiveZone.DegradeStyleRbtsReq Then
                If Not bSaveDegradeRbtsReqToGUI() Then

                End If
            End If
            'If mcolZones.SaveAccessData Then
            '    'temp till access is gone --------------
            '    If Not bSaveStylesToAccessDB() Then

            '    End If
            'End If
            '---------------------------------------

            Return True

        Catch ex As Exception
            Return False
        End Try
    End Function
    Private Function bSaveStylesToSQLDB() As Boolean
        '********************************************************************************************
        'Description:  Save data stored on GUI in database, xml file etc.
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/22/11  JBW     Added two coats by style
        ' 12/13/11  RJO     Added support for Sealer combined Style/Option
        ' 04/09/12  MSW     Move to XML
        ' 09/13/12  RJO     Added code to log changes to EntranceMuteStartCount, ExitMuteStartCount,
        '                   and MuteLength.
        ' 10/04/12  RJO     Bug fix to change log reporting for Robots Required.
        ' 10/29/13  BTK     Wrong labels for change log.
        '********************************************************************************************
        Dim nItem As Integer = 0
        Dim oZone As clsZone = mcolZones.ActiveZone
        Dim oS As clsSysStyle = Nothing
        Try

        

            'If change was due to Restore from DB or PLC, note this in the change log
            If Strings.Len(msRestoreString) > 0 Then
                Call subUpdateChangeLog(oZone.ZoneNumber)
                msRestoreString = String.Empty
            End If
            Const sXMLFILE As String = "SystemStyles"
            Const sXMLTABLE As String = "SystemStyles"
            Const sXMLNODE As String = "Style"

            Dim sTopic As String = String.Empty
            Dim oMainNode As XmlNode = Nothing
            Dim oZoneNode As XmlNode = Nothing
            Dim oNodeList As XmlNodeList = Nothing
            Dim oXMLDoc As New XmlDocument
            Dim bRemoteZone As Boolean = False
            Dim sXMLFilePath As String = String.Empty
            Try
                If GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, sXMLFILE & ".XML") Then

                    oXMLDoc.Load(sXMLFilePath)

                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    oZoneNode = oMainNode.SelectSingleNode("//" & oZone.Name)
                    If oZoneNode Is Nothing Then
                        oZoneNode = oXMLDoc.CreateElement(oZone.Name)
                        oMainNode.AppendChild(oZoneNode)
                        oZoneNode = oMainNode.SelectSingleNode("//" & oZone.Name)
                    End If
                Else
                    oXMLDoc = New XmlDocument
                    oXMLDoc.CreateElement(sXMLTABLE)
                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    oXMLDoc.CreateElement(oZone.Name)
                    oZoneNode = oXMLDoc.SelectSingleNode("//" & oZone.Name)
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":bLoadCollection", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
                ShowErrorMessagebox("Invalid XPath syntax: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)

            End Try
            oZoneNode.RemoveAll()

            For nItem = 0 To (mcolStyles.Count - 1)
                oS = mcolStyles.Item(nItem)


                'Build XML node
                Dim oNode As XmlNode = oXMLDoc.CreateElement(sXMLNODE)
                Dim oPNNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSS_COL_PLANTNUM, Nothing)
                If mcolStyles.CombinedStyleOption Then 'RJO 12/13/11
                    Dim nStyle As Integer = CType(oS.PlantNumber.Text, Integer)
                    Dim nStyleOpt As Integer = (nStyle * 100) + CType(oS.OptionNumber.Text, Integer)
                    oPNNode.InnerXml = nStyleOpt.ToString
                Else
                    oPNNode.InnerXml = oS.PlantNumber.Text
                End If

                oNode.AppendChild(oPNNode)
                Dim oFNNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSS_COL_FANUCNUM, Nothing)
                oFNNode.InnerXml = oS.FanucNumber.Value.ToString
                oNode.AppendChild(oFNNode)
                Dim oEnabNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSS_COL_ENABLE, Nothing)
                oEnabNode.InnerXml = oS.RobotsRequired.Value.ToString
                oNode.AppendChild(oEnabNode)
                Dim oDescNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSS_COL_DESC, Nothing)
                oDescNode.InnerXml = oS.Description.Text
                oNode.AppendChild(oDescNode)
                Dim oRegNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSO_REGISTER, Nothing)
                oRegNode.InnerXml = oS.StyleRegister.Value.ToString
                oNode.AppendChild(oRegNode)
                Dim oTCNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSS_COL_TWOCOATS, Nothing)
                oTCNode.InnerXml = oS.TwoCoatStyle.Value.ToString
                oNode.AppendChild(oTCNode)
                Dim oIKNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSS_COL_IMG_KEY, Nothing)
                oIKNode.InnerXml = oS.ImageKey
                oNode.AppendChild(oIKNode)
                Dim oMEntNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSS_COL_ENT_MUTE, Nothing)
                oMEntNode.InnerXml = oS.EntranceMuteStartCount.Value.ToString
                oNode.AppendChild(oMEntNode)
                Dim oMExitNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSS_COL_EXIT_MUTE, Nothing)
                oMExitNode.InnerXml = oS.ExitMuteStartCount.Value.ToString
                oNode.AppendChild(oMExitNode)
                Dim oMLenNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSS_COL_MUTE_LEN, Nothing)
                oMLenNode.InnerXml = oS.MuteLength.Value.ToString
                oNode.AppendChild(oMLenNode)
                Dim oTCEnbNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSS_COL_TRICOATENB, Nothing)
                oTCEnbNode.InnerXml = oS.TricoatEnable.Value.ToString
                oNode.AppendChild(oTCEnbNode)

                oZoneNode.AppendChild(oNode)

                If oS.PlantNumber.Changed Then
                    'update change log
                    Call subUpdateChangeLog(oS.PlantNumber.Text.ToString, _
                                oS.PlantNumber.OldText.ToString, _
                                String.Empty, _
                                oZone, lblPlantStyleCap.Text & " #" & (nItem + 1).ToString, _
                                String.Empty)

                End If

                If mcolStyles.CombinedStyleOption Then 'RJO 12/13/11
                    If oS.OptionNumber.Changed Then
                        'update change log
                        Call subUpdateChangeLog(oS.OptionNumber.Text.ToString, _
                                    oS.OptionNumber.OldText.ToString, _
                                    String.Empty, _
                                    oZone, lblStyleOptCap.Text & " #" & (nItem + 1).ToString, _
                                    String.Empty)

                    End If
                End If

                If oS.FanucNumber.Changed Then
                    'update change log
                    Call subUpdateChangeLog(oS.FanucNumber.Value.ToString, _
                               oS.FanucNumber.OldValue.ToString, _
                              String.Empty, _
                              oZone, lblFanucStyleCap.Text & " #" & (nItem + 1).ToString, _
                              String.Empty)
                End If

                If oS.Description.Changed Then
                    'update change log
                    Call subUpdateChangeLog(oS.Description.Text, _
                             oS.Description.OldText, _
                            String.Empty, _
                            oZone, lblStyleDescCap.Text & " #" & (nItem + 1).ToString, _
                            String.Empty)
                End If

                If oS.RobotsRequired.Changed Then

                    Dim sNewValue As String = String.Empty 'StrReverse(mMathFunctions.CvBin(oS.RobotsRequired.Value, mnNumberofRobots + 1))
                    Dim sOldValue As String = String.Empty 'StrReverse(mMathFunctions.CvBin(oS.RobotsRequired.OldValue, mnNumberofRobots + 1))
                    For nArm As Integer = 0 To mcolArms.Count - 1
                        Dim nArmIdx As Integer = mcolArms(nArm).RobotNumber + mcolZones.ActiveZone.RobotsRequiredStartingBit - 1
                        If ((oS.RobotsRequired.Value And gnBitVal(nArmIdx)) <> 0) Then
                            sNewValue = "1" & sNewValue
                        Else
                            sNewValue = "0" & sNewValue
                        End If
                        If ((oS.RobotsRequired.OldValue And gnBitVal(nArmIdx)) <> 0) Then
                            sOldValue = "1" & sOldValue
                        Else
                            sOldValue = "0" & sOldValue
                        End If
                    Next 'i
                    'Make sure we get the proper bits 'RJO 10/04/12
                    If mcolZones.ActiveZone.RobotsRequiredStartingBit = 1 Then
                        sNewValue = Strings.Right(sNewValue, mnNumberofRobots)
                        sOldValue = Strings.Right(sOldValue, mnNumberofRobots)
                    Else
                        sNewValue = Strings.Left(sNewValue, mnNumberofRobots)
                        sOldValue = Strings.Left(sOldValue, mnNumberofRobots)
                    End If

                    'update change log
                    Call subUpdateChangeLog(sNewValue, sOldValue, String.Empty, oZone, _
                                            lblStyleRobotsReqCap.Text & " #" & (nItem + 1).ToString, _
                                            String.Empty)
                End If

                If mcolStyles.TwoCoat Then
                    'jbw added for two coats by style
                    If oS.TwoCoatStyle.Changed Then
                        Call subUpdateChangeLog(oS.TwoCoatStyle.Value.ToString, oS.TwoCoatStyle.OldValue.ToString, _
                                String.Empty, oZone, lbltwocoats.Text & " #" & (nItem + 1).ToString, String.Empty)
                    End If

                End If

                'BTK 10/29/13 Wrong labels for change log.
                If mcolStyles(nItem).MuteLength.Changed Then
                    subUpdateChangeLog(mcolStyles(nItem).MuteLength.Value.ToString, _
                        mcolStyles(nItem).MuteLength.OldValue.ToString, _
                        mcolStyles(nItem).DisplayName, _
                        oZone, lblMuteLenCap.Text, String.Empty)
                    mcolStyles(nItem).MuteLength.Update()
                End If

                If mcolStyles(nItem).EntranceMuteStartCount.Changed Then
                    subUpdateChangeLog( _
                        mcolStyles(nItem).EntranceMuteStartCount.Value.ToString, _
                        mcolStyles(nItem).EntranceMuteStartCount.OldValue.ToString, _
                        mcolStyles(nItem).DisplayName, _
                        oZone, lblEntStartCap.Text, String.Empty)
                    mcolStyles(nItem).EntranceMuteStartCount.Update()
                End If

                If mcolStyles(nItem).ExitMuteStartCount.Changed Then
                    subUpdateChangeLog( _
                        mcolStyles(nItem).ExitMuteStartCount.Value.ToString, _
                        mcolStyles(nItem).ExitMuteStartCount.OldValue.ToString, _
                        mcolStyles(nItem).DisplayName, _
                        oZone, lblExitStartCap.Text, String.Empty)
                    mcolStyles(nItem).ExitMuteStartCount.Update()
                End If

                If mcolZones.ActiveZone.TricoatByStyle Then
                    If oS.TricoatEnable.Changed Then
                        Dim sNewValue As String = StrReverse(mMathFunctions.CvBin(oS.TricoatEnable.Value, _
                            mnNumberofRobots + 1))
                        Dim sOldValue As String = StrReverse(mMathFunctions.CvBin(oS.TricoatEnable.OldValue, _
                            mnNumberofRobots + 1))
                        'Make sure we get the proper bits 'RJO 10/04/12
                        If mcolZones.ActiveZone.RobotsRequiredStartingBit = 1 Then
                            sNewValue = Strings.Right(sNewValue, mnNumberofRobots)
                            sOldValue = Strings.Right(sOldValue, mnNumberofRobots)
                        Else
                            sNewValue = Strings.Left(sNewValue, mnNumberofRobots)
                            sOldValue = Strings.Left(sOldValue, mnNumberofRobots)
                        End If

                        'update change log
                        Call subUpdateChangeLog(sNewValue, sOldValue, String.Empty, _
                               oZone, TabPage7.Text & " " & lblTricoatRobotsReqCap.Text & " #" & (nItem + 1).ToString, _
                               String.Empty)
                    End If
                End If

                If oZone.DegradeStyleRbtsReq Then
                    Dim nDegrade As Integer
                    For nDegrade = 0 To oZone.NumberOfDegrades - 1
                        If oS.DegradeRobotsRequired(nDegrade).Changed Then
                            Dim sNewValue As String = StrReverse(mMathFunctions.CvBin(oS.DegradeRobotsRequired(nDegrade).Value, _
                                    mnNumberofRobots + 1))
                            Dim sOldValue As String = StrReverse(mMathFunctions.CvBin(oS.DegradeRobotsRequired(0).OldValue, _
                                    mnNumberofRobots + 1))
                            'Make sure we get the proper bits 'RJO 10/04/12
                            If mcolZones.ActiveZone.RobotsRequiredStartingBit = 1 Then
                                sNewValue = Strings.Right(sNewValue, mnNumberofRobots)
                                sOldValue = Strings.Right(sOldValue, mnNumberofRobots)
                            Else
                                sNewValue = Strings.Left(sNewValue, mnNumberofRobots)
                                sOldValue = Strings.Left(sOldValue, mnNumberofRobots)
                            End If

                            'update change log
                            Call subUpdateChangeLog(sNewValue, sOldValue, String.Empty, oZone, _
                                                    lblStyleRobotsReqCap.Text & " #" & (nItem + 1).ToString, _
                                                    String.Empty)
                        End If
                    Next
                End If
            Next

            mcolStyles.Update()
            oXMLDoc.Save(sXMLFilePath)
            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bSaveStyleIDToSQLDB() As Boolean
        '********************************************************************************************
        'Description:  Save data stored on GUI in database, xml file etc.
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 07/08/13  MSW     Move the style ID tables to XML
        '********************************************************************************************
        'Dim DB As clsSQLAccess = New clsSQLAccess
        'Dim sTableName As String = gsSYSID_DS_TABLENAME
        'Dim DR As DataRow = Nothing
        'Dim oS As clsSysStyleID = Nothing
        'Dim DT As DataTable = New DataTable
        'Dim nItem As Integer = 0
        'Dim oZone As clsZone = mcolZones.ActiveZone

        'Try

        '    With DB

        '        .DBFileName = gsPROCESS_DBNAME
        '        .Zone = oZone

        '        Dim ds As DataSet = Nothing 'mSysStyleID.GetStyleIDDataset(DB, oZone.Name)
        '        If ds Is Nothing Then
        '            Return False
        '        End If

        '        DT = ds.Tables("[" & sTableName & "]")


        '        'If change was due to Restore from DB or PLC, note this in the change log
        '        If Strings.Len(msRestoreString) > 0 Then
        '            Call subUpdateChangeLog(oZone.ZoneNumber)
        '            msRestoreString = String.Empty
        '        End If

        '        For nItem = (mnStyleIDIndex * mcolStyleID.NumberOfConveyorSnapshots) To _
        '            ((mnStyleIDIndex * mcolStyleID.NumberOfConveyorSnapshots) + mcolStyleID.NumberOfConveyorSnapshots - 1)
        '            oS = mcolStyleID.Item(nItem)
        '            'write all info
        '            DR = DT.Rows(nItem)
        '            DR.BeginEdit()

        '            DR.Item(gsSYSID_COL_CONVEYOR_COUNT) = CType(oS.ConveyorCount.Value, Integer)
        '            DR.Item(gsSYSID_COL_PHOTOEYE_PATTERN) = CType(oS.PhotoeyePattern.Value, Integer)
        '            DR.Item(gsSYSID_COL_PHOTOEYE_IGNORE) = CType(oS.PhotoeyeIgnore.Value, Integer)
        '            DR.Item(gsSYSID_COL_SUNROOF_PHOTOEYE_PATTERN) = CType(oS.SunroofPhotoeyePattern.Value, Integer)
        '            DR.Item(gsSYSID_COL_SUNROOF_PHOTOEYE_IGNORE) = CType(oS.SunroofPhotoeyeIgnore.Value, Integer)



        '            DR.EndEdit()

        '        Next
        '        .UpdateDataSet(ds, sTableName)
        '        .Close()
        '    End With

        '    mcolStyleID.Update()

        '    Return True

        'Catch ex As Exception

        '    ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED", DisplayCulture), ex, msSCREEN_NAME, _
        '                        Status, MessageBoxButtons.OK)

        '    Return False

        'End Try

        Dim nItem As Integer = 0
        Dim oZone As clsZone = mcolZones.ActiveZone
        Dim oS As clsSysStyleID = Nothing
        Try



            'If change was due to Restore from DB or PLC, note this in the change log
            If Strings.Len(msRestoreString) > 0 Then
                Call subUpdateChangeLog(oZone.ZoneNumber)
                msRestoreString = String.Empty
            End If
            Const sXMLFILE As String = gsSYSID_DS_TABLENAME
            Const sXMLTABLE As String = gsSYSID_DS_TABLENAME
            Const sXMLNODE As String = gsSYSID_DS_ITEMNAME

            Dim sTopic As String = String.Empty
            Dim oMainNode As XmlNode = Nothing
            Dim oZoneNode As XmlNode = Nothing
            Dim oNodeList As XmlNodeList = Nothing
            Dim oXMLDoc As New XmlDocument
            Dim bRemoteZone As Boolean = False
            Dim sXMLFilePath As String = String.Empty
            Try
                If GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, sXMLFILE & ".XML") Then

                    oXMLDoc.Load(sXMLFilePath)

                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    oZoneNode = oMainNode.SelectSingleNode("//" & oZone.Name)
                    If oZoneNode Is Nothing Then
                        oZoneNode = oXMLDoc.CreateElement(oZone.Name)
                        oMainNode.AppendChild(oZoneNode)
                        oZoneNode = oMainNode.SelectSingleNode("//" & oZone.Name)
                    End If
                Else
                    oXMLDoc = New XmlDocument
                    oXMLDoc.CreateElement(sXMLTABLE)
                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    oXMLDoc.CreateElement(oZone.Name)
                    oZoneNode = oXMLDoc.SelectSingleNode("//" & oZone.Name)
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(msMODULE & ":bLoadCollection", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
                ShowErrorMessagebox("Invalid XPath syntax: [" & sXMLFilePath & "] - ", ex, msMODULE, String.Empty)

            End Try
            If mcolStyleID.NumberOfConveyorSnapshots = 0 Then

                oZoneNode = oMainNode.SelectSingleNode("//" & oZone.Name)
                oS = mcolStyleID.Item(0)
                Dim sStyle As String = (mnStyleIDIndex + 1).ToString
                Dim bFound As Boolean = False
                For Each oNodeTmp As XmlNode In oZoneNode.ChildNodes
                    If oNodeTmp.Item(gsSYSID_COL_STYLEINDEX).InnerText = sStyle Then
                        oNodeTmp.Item(gsSYSID_COL_PHOTOEYE_PATTERN).InnerText = oS.PhotoeyePattern.Value.ToString
                        bFound = True
                        Exit For
                    End If
                Next
                If bFound = False Then
                    'Build XML node
                    Dim oNode As XmlNode = oXMLDoc.CreateElement(sXMLNODE)

                    Dim oSINode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSID_COL_STYLEINDEX, Nothing)
                    oSINode.InnerXml = sStyle
                    oNode.AppendChild(oSINode)

                    Dim oCCNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSID_COL_CONVEYOR_COUNT, Nothing)
                    oCCNode.InnerXml = "0"
                    oNode.AppendChild(oCCNode)

                    Dim oPENode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSID_COL_PHOTOEYE_PATTERN, Nothing)
                    oPENode.InnerXml = oS.PhotoeyePattern.Value.ToString
                    oNode.AppendChild(oPENode)

                    Dim oPINode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSID_COL_PHOTOEYE_IGNORE, Nothing)
                    oPINode.InnerXml = "0"
                    oNode.AppendChild(oPINode)

                    Dim oSPENode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSID_COL_SUNROOF_PHOTOEYE_PATTERN, Nothing)
                    oSPENode.InnerXml = "0"
                    oNode.AppendChild(oSPENode)

                    Dim oSPINode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSID_COL_SUNROOF_PHOTOEYE_IGNORE, Nothing)
                    oSPINode.InnerXml = "0"
                    oNode.AppendChild(oSPINode)

                    oZoneNode.AppendChild(oNode)

                End If

                If oS.PhotoeyePattern.Changed Then
                    'update change log
                    Dim sOldVal As String = String.Empty
                    Dim sNewVal As String = String.Empty
                    For nPE As Integer = 0 To mcolStyleID.NumberOfPhotoeyes - 1
                        If ((nBitMask(nPE) And oS.PhotoeyePattern.OldValue) <> 0) Then
                            sOldVal = sOldVal & "1"
                        Else
                            sOldVal = sOldVal & "0"
                        End If
                        If ((nBitMask(nPE) And oS.PhotoeyePattern.Value) <> 0) Then
                            sNewVal = sNewVal & "1"
                        Else
                            sNewVal = sNewVal & "0"
                        End If

                    Next
                    Call subUpdateChangeLog(sNewVal, sOldVal, String.Empty, oZone, _
                        cboStyleID.Text & ", " & lblPhotoeyePattern.Text, String.Empty)
                End If
            Else
                oZoneNode.RemoveAll()

                For nStyleIndex As Integer = 0 To mcolStyles.Count - 1
                    For nItem = 0 To (mcolStyleID.NumberOfConveyorSnapshots - 1)
                        oS = mcolStyleID.Item(mcolStyleID.NumberOfConveyorSnapshots * nStyleIndex + nItem)
                        'Build XML node
                        Dim oNode As XmlNode = oXMLDoc.CreateElement(sXMLNODE)
                        'moConveyorCount = New clsIntValue
                        'moPhotoeyePattern = New clsIntValue
                        'moSunroofPhotoeyePattern = New clsIntValue
                        'moPhotoeyeIgnore = New clsIntValue
                        'moSunroofPhotoeyeIgnore = New clsIntValue
                        'moConveyorCount.Value = 0
                        'moPhotoeyePattern.Value = 0
                        'moSunroofPhotoeyePattern.Value = 0
                        'moPhotoeyeIgnore.Value = 0
                        ''moSunroofPhotoeyeIgnore.Value = 0
                        'Friend Const gsSYSID_COL_CONVEYOR_COUNT As String = "ConveyorCount"
                        'Friend Const gsSYSID_COL_PHOTOEYE_PATTERN As String = "PhotoeyePattern"
                        'Friend Const gsSYSID_COL_SUNROOF_PHOTOEYE_PATTERN As String = "SunroofPhotoeyePattern"
                        'Friend Const gsSYSID_COL_PHOTOEYE_IGNORE As String = "PhotoeyeIgnore"
                        'Friend Const gsSYSID_COL_SUNROOF_PHOTOEYE_IGNORE As String = "SunroofPhotoeyeIgnore"
                        Dim oSINode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSID_COL_STYLEINDEX, Nothing)
                        oSINode.InnerXml = nStyleIndex.ToString
                        oNode.AppendChild(oSINode)

                        Dim oCCNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSID_COL_CONVEYOR_COUNT, Nothing)
                        oCCNode.InnerXml = oS.ConveyorCount.Value.ToString
                        oNode.AppendChild(oCCNode)

                        Dim oPENode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSID_COL_PHOTOEYE_PATTERN, Nothing)
                        oPENode.InnerXml = oS.PhotoeyePattern.Value.ToString
                        oNode.AppendChild(oPENode)

                        Dim oPINode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSID_COL_PHOTOEYE_IGNORE, Nothing)
                        oPINode.InnerXml = oS.PhotoeyeIgnore.Value.ToString
                        oNode.AppendChild(oPINode)

                        Dim oSPENode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSID_COL_SUNROOF_PHOTOEYE_PATTERN, Nothing)
                        oSPENode.InnerXml = oS.SunroofPhotoeyePattern.Value.ToString
                        oNode.AppendChild(oSPENode)

                        Dim oSPINode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsSYSID_COL_SUNROOF_PHOTOEYE_IGNORE, Nothing)
                        oSPINode.InnerXml = oS.SunroofPhotoeyeIgnore.Value.ToString
                        oNode.AppendChild(oSPINode)
                        oZoneNode.AppendChild(oNode)

                        If oS.ConveyorCount.Changed Then
                            'update change log
                            Call subUpdateChangeLog(oS.ConveyorCount.Value.ToString, _
                                        oS.ConveyorCount.OldValue.ToString, _
                                        String.Empty, _
                                        oZone, cboStyleID.Text & " - " & lblConveyorPosCap.Text & " #" & (nItem + 1).ToString, _
                                        String.Empty)

                        End If

                        If oS.PhotoeyePattern.Changed Then
                            'update change log
                            Call subUpdateChangeLog(oS.PhotoeyePattern.Value.ToString, _
                                       oS.PhotoeyePattern.OldValue.ToString, _
                                      String.Empty, _
                                      oZone, cboStyleID.Text & ", " & lblPhotoeyePattern.Text & " #" & (nItem + 1).ToString, _
                                      String.Empty)
                        End If

                        If oS.PhotoeyeIgnore.Changed Then
                            'update change log
                            Call subUpdateChangeLog(oS.PhotoeyeIgnore.Value.ToString, _
                                       oS.PhotoeyeIgnore.OldValue.ToString, _
                                      String.Empty, _
                                      oZone, cboStyleID.Text & ", " & lblPhotoeyePattern.Text & " #" & (nItem + 1).ToString, _
                                      String.Empty)
                        End If

                        If oS.SunroofPhotoeyePattern.Changed Then
                            'update change log
                            Call subUpdateChangeLog(oS.SunroofPhotoeyePattern.Value.ToString, _
                                       oS.SunroofPhotoeyePattern.OldValue.ToString, _
                                      String.Empty, _
                                      oZone, cboStyleID.Text & ", " & lblPhotoeyePattern.Text & " #" & (nItem + 1).ToString, _
                                      String.Empty)
                        End If

                        If oS.SunroofPhotoeyeIgnore.Changed Then
                            'update change log
                            Call subUpdateChangeLog(oS.SunroofPhotoeyeIgnore.Value.ToString, _
                                       oS.SunroofPhotoeyeIgnore.OldValue.ToString, _
                                      String.Empty, _
                                      oZone, cboStyleID.Text & ", " & lblPhotoeyePattern.Text & " #" & (nItem + 1).ToString, _
                                      String.Empty)
                        End If

                    Next

                Next
            End If

            mcolStyles.Update()
            oXMLDoc.Save(sXMLFilePath)
            Return True


        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bSaveStylesToPLC() As Boolean
        '********************************************************************************************
        'Description:  Save data stored on PLC.
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/30/10  MSW     Change to look at ascii enable in option/style/color config
        ' 10/05/10  MSW     Clear out extra bits in the robots required word
        ' 02/22/11  JBW     Added two coat by style
        ' 12/13/11  RJO     Added support for Sealer combined Style/Option
        ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
        ' 07/23/14  RJO     Big Changes to support more degrade modes than robots
        '********************************************************************************************
        Dim bSaveSuccess As Boolean = False
        Dim nStyleCount As Integer = 0
        Dim nIndex As Integer = 0
        Dim sData() As String
        Dim sTag As String
        Dim ub%
        Dim i As Integer
        Dim x As Integer 'NVSP 1000 Styles
        Dim y As Integer 'NVSP 1000 Styles
        Dim sPLCDataStyles() As String 'NVSP 1000 Styles
        Dim sPrefix As String = mcolZones.ActiveZone.PLCTagPrefix()
        Dim nLength As Integer = 0

        Try

            moPLC.ZoneName = mcolZones.ActiveZone.Name
            ub% = mcolStyles.MaxStyles - 1

            ReDim sData(ub%)
            ' Ascii data is store in a separate data table so lets keep it separate from the integer style number.
            ' Ascii stuff at end of this sub...
            For nIndex = 0 To ub%
                If mcolStyles.CombinedStyleOption Then 'RJO 12/13/11
                    Dim nStyle As Integer = CType(mcolStyles(nIndex).PlantNumber.Text, Integer)
                    Dim nStyleOpt As Integer = (nStyle * 100) + CType(mcolStyles(nIndex).OptionNumber.Text, Integer)

                    sData(nIndex) = nStyleOpt.ToString
                Else
                    sData(nIndex) = mcolStyles(nIndex).PlantNumber.Text
                End If
            Next
            If mcolStyles.UseAscii Then
                For nCounter As Integer = 0 To sData.GetUpperBound(0)
                    sData(nCounter) = mMathFunctions.CvASCIIToInteger(sData(nCounter), mcolStyles.PlantAsciiMaxLength).ToString
                Next
            End If

            'NVSP 1000 plant styles
            For x = 1 To 10
                ReDim sPLCDataStyles(99)
                For i = 0 To 99
                    y = (x - 1) * 100 + i
                    sPLCDataStyles(i) = sData(y)
                Next
                sTag = sPrefix & "PlantStyles" & x.ToString
                moPLC.TagName = sTag
                moPLC.PLCData = sPLCDataStyles
                Application.DoEvents()
            Next x

            For nIndex = 0 To ub%
                sData(nIndex) = mcolStyles(nIndex).FanucNumber.Value.ToString
            Next

            'NVSP 1000 fanuc styles
            For x = 1 To 10
                ReDim sPLCDataStyles(99)
                For i = 0 To 99
                    y = (x - 1) * 100 + i
                    sPLCDataStyles(i) = sData(y)
                Next
                sTag = sPrefix & "FanucStyles" & x.ToString
                moPLC.TagName = sTag
                moPLC.PLCData = sPLCDataStyles
                Application.DoEvents()
            Next x

            'NVSP NVSP Robots Required for 1000 styles 
            ReDim sData(ub%)
            Dim nMax As Integer = mcolArms(mcolArms.Count - 1).RobotNumber
            Dim nMask As Integer = CType(((2 ^ nMax) - 1) * (2 ^ mcolZones.ActiveZone.RobotsRequiredStartingBit), Integer)
            For nIndex = 0 To ub%
                mcolStyles(nIndex).RobotsRequired.Value = mcolStyles(nIndex).RobotsRequired.Value And nMask
                sData(nIndex) = mcolStyles(nIndex).RobotsRequired.Value.ToString
            Next

            For x = 1 To 10
                ReDim sPLCDataStyles(99)
                For i = 0 To 99
                    y = (x - 1) * 100 + i
                    sPLCDataStyles(i) = sData(y)
                Next

                sTag = sPrefix & "StyleRobotsRequired" & x.ToString
                moPLC.TagName = sTag
                moPLC.PLCData = sPLCDataStyles
                Application.DoEvents()
            Next x

            'NVSP rewrote the function as simple as possible.
            'Too many conditions and looked like junk commented out.

            '''''''''''Dim sTmp() As String = moPLC.PLCData

            '''''''''''ReDim sData(ub%)
            '''''''''''Dim nMax As Integer = mcolArms(mcolArms.Count - 1).RobotNumber
            '''''''''''Dim nMask As Integer = CType(((2 ^ nMax) - 1) * (2 ^ mcolZones.ActiveZone.RobotsRequiredStartingBit), Integer)

            '''''''''''For nIndex = 0 To ub%
            '''''''''''    sData(nIndex) = mcolStyles(nIndex).FanucNumber.Value.ToString
            '''''''''''Next

            '''''''''''sTag = mcolZones.ActiveZone.PLCTagPrefix & "FanucStyles"
            '''''''''''moPLC.TagName = sTag
            '''''''''''' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
            '''''''''''sTmp = moPLC.PLCData
            '''''''''''If sTmp.GetUpperBound(0) >= sData.GetUpperBound(0) Then
            '''''''''''    For nItem As Integer = 0 To sData.GetUpperBound(0)
            '''''''''''        sTmp(nItem) = sData(nItem)
            '''''''''''    Next
            '''''''''''    moPLC.PLCData = sTmp
            '''''''''''Else
            '''''''''''    'Let the PLC form complain about it if the PLC link is shorter than the DB setup
            '''''''''''    moPLC.PLCData = sData
            '''''''''''End If

            ''''''''''''room to pop up error if it happens
            '''''''''''Application.DoEvents()

            ''''''''''''RJO 07/23/14
            ''''''''''''ReDim sPLCData1(mnStyleRbtRqdLinkLength(0) - 1)
            ''''''''''''If mnStyleRbtRqdLinkLength.GetUpperBound(0) > 0 Then
            ''''''''''''    ReDim sPLCData2(mnStyleRbtRqdLinkLength(1) - 1)
            ''''''''''''    If mnStyleRbtRqdLinkLength.GetUpperBound(0) > 1 Then
            ''''''''''''        ReDim sPLCData3(mnStyleRbtRqdLinkLength(2) - 1)
            ''''''''''''        If mnStyleRbtRqdLinkLength.GetUpperBound(0) > 2 Then
            ''''''''''''            ReDim sPLCData4(mnStyleRbtRqdLinkLength(3) - 1)
            ''''''''''''            If mnStyleRbtRqdLinkLength.GetUpperBound(0) > 3 Then
            ''''''''''''                ReDim sPLCData5(mnStyleRbtRqdLinkLength(4) - 1)
            ''''''''''''            End If
            ''''''''''''        End If
            ''''''''''''    End If
            ''''''''''''End If

            '''''''''''nMask = CType(((2 ^ mcolArms.Count) - 1) * (2 ^ mcolZones.ActiveZone.RobotsRequiredStartingBit), Integer)
            '''''''''''Dim nNumberOfDegrades As Integer = (mcolStyles.MaxStyles * nNumOfDegrades)
            '''''''''''If mcolZones.ActiveZone.DegradeStyleRbtsReq Then
            '''''''''''    Dim nStyle As Integer
            '''''''''''    ReDim sData((mcolStyles.MaxStyles * nNumOfDegrades) - 1)
            '''''''''''    For nIndex = 0 To UBound(sData)
            '''''''''''        nStyle = nIndex \ nNumOfDegrades
            '''''''''''        nDegrade = nIndex - ((nStyle) * (nNumOfDegrades))
            '''''''''''        mcolStyles(nStyle).DegradeRobotsRequired(nDegrade).Value = mcolStyles(nStyle).DegradeRobotsRequired(nDegrade).Value And nMask
            '''''''''''        sData(nIndex) = mcolStyles(nStyle).DegradeRobotsRequired(nDegrade).Value.ToString

            '''''''''''        'RJO 07/23/14
            '''''''''''        '***************
            '''''''''''        'Dim nPLCDataIndex As Integer = (nStyle * nNumOfDegrades) + nDegrade
            '''''''''''        'If nPLCDataIndex < mnStyleRbtRqdLinkLength(0) Then
            '''''''''''        '    sPLCData1(nPLCDataIndex) = mcolStyles(nStyle).DegradeRobotsRequired(nDegrade).Value.ToString
            '''''''''''        'ElseIf nPLCDataIndex > mnStyleRbtRqdLinkLength(0) - 1 And nPLCDataIndex < mnStyleRbtRqdLinkLength(0) + mnStyleRbtRqdLinkLength(1) Then
            '''''''''''        '    sPLCData2(nPLCDataIndex - mnStyleRbtRqdLinkLength(0)) = mcolStyles(nStyle).DegradeRobotsRequired(nDegrade).Value.ToString
            '''''''''''        'ElseIf nPLCDataIndex > mnStyleRbtRqdLinkLength(0) + mnStyleRbtRqdLinkLength(1) - 1 And nPLCDataIndex < mnStyleRbtRqdLinkLength(0) + mnStyleRbtRqdLinkLength(1) + mnStyleRbtRqdLinkLength(2) Then
            '''''''''''        '    sPLCData3(nPLCDataIndex - mnStyleRbtRqdLinkLength(0) - mnStyleRbtRqdLinkLength(1)) = mcolStyles(nStyle).DegradeRobotsRequired(nDegrade).Value.ToString
            '''''''''''        'ElseIf nPLCDataIndex > mnStyleRbtRqdLinkLength(0) + mnStyleRbtRqdLinkLength(1) + mnStyleRbtRqdLinkLength(2) - 1 And nPLCDataIndex < mnStyleRbtRqdLinkLength(0) + mnStyleRbtRqdLinkLength(1) + mnStyleRbtRqdLinkLength(2) + mnStyleRbtRqdLinkLength(3) Then
            '''''''''''        '    sPLCData4(nPLCDataIndex - mnStyleRbtRqdLinkLength(0) - mnStyleRbtRqdLinkLength(1) - mnStyleRbtRqdLinkLength(2)) = mcolStyles(nStyle).DegradeRobotsRequired(nDegrade).Value.ToString
            '''''''''''        'ElseIf nPLCDataIndex > mnStyleRbtRqdLinkLength(0) + mnStyleRbtRqdLinkLength(1) + mnStyleRbtRqdLinkLength(2) + mnStyleRbtRqdLinkLength(3) - 1 And nPLCDataIndex < mnStyleRbtRqdLinkLength(0) + mnStyleRbtRqdLinkLength(1) + mnStyleRbtRqdLinkLength(2) + mnStyleRbtRqdLinkLength(3) + mnStyleRbtRqdLinkLength(4) Then
            '''''''''''        '    sPLCData5(nPLCDataIndex - mnStyleRbtRqdLinkLength(0) - mnStyleRbtRqdLinkLength(1) - mnStyleRbtRqdLinkLength(2) - mnStyleRbtRqdLinkLength(3)) = mcolStyles(nStyle).DegradeRobotsRequired(nDegrade).Value.ToString
            '''''''''''        'End If
            '''''''''''        '***************

            '''''''''''    Next
            '''''''''''Else
            '''''''''''    ReDim sData(ub%)

            '''''''''''    For nIndex = 0 To ub%
            '''''''''''        mcolStyles(nIndex).RobotsRequired.Value = mcolStyles(nIndex).RobotsRequired.Value And nMask
            '''''''''''        sData(nIndex) = mcolStyles(nIndex).RobotsRequired.Value.ToString
            '''''''''''    Next

            '''''''''''End If

            ''''''''''''RJO 07/23/14
            ''''''''''''*************
            ''''''''''''sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired"
            ''''''''''''moPLC.TagName = sTag
            ''''''''''''moPLC.PLCData = sPLCData1
            ''''''''''''Application.DoEvents()
            ''''''''''''If mnStyleRbtRqdLinkLength.GetUpperBound(0) > 0 Then
            ''''''''''''    sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired2"
            ''''''''''''    moPLC.TagName = sTag
            ''''''''''''    moPLC.PLCData = sPLCData2
            ''''''''''''    Application.DoEvents()
            ''''''''''''    If mnStyleRbtRqdLinkLength.GetUpperBound(0) > 1 Then
            ''''''''''''        moPLC.TagName = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired3"
            ''''''''''''        moPLC.PLCData = sPLCData3
            ''''''''''''        Application.DoEvents()
            ''''''''''''        If mnStyleRbtRqdLinkLength.GetUpperBound(0) > 2 Then
            ''''''''''''            moPLC.TagName = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired4"
            ''''''''''''            moPLC.PLCData = sPLCData4
            ''''''''''''            Application.DoEvents()
            ''''''''''''            If mnStyleRbtRqdLinkLength.GetUpperBound(0) > 3 Then
            ''''''''''''                moPLC.TagName = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired5"
            ''''''''''''                moPLC.PLCData = sPLCData5
            ''''''''''''                Application.DoEvents()
            ''''''''''''            End If
            ''''''''''''        End If
            ''''''''''''    End If
            ''''''''''''End If
            ''''''''''''*************

            ''''''''''''RJO 07/23/14
            ''''''''''''Write data to PLC
            '''''''''''sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRobotsRequired"

            '''''''''''If mcolZones.ActiveZone.DegradeStyleRbtsReq Then
            '''''''''''    Dim nPointer As Integer

            '''''''''''    For nStyle As Integer = 1 To mcolStyles.MaxStyles
            '''''''''''        Dim sPLCData(nNumOfDegrades - 1) As String

            '''''''''''        nPointer = (nStyle - 1) * nNumOfDegrades
            '''''''''''        For nDegrade = 0 To nNumOfDegrades - 1
            '''''''''''            sPLCData(nDegrade) = sData(nPointer + nDegrade)
            '''''''''''        Next 'nDegrade

            '''''''''''        moPLC.TagName = sTag & nStyle.ToString
            '''''''''''        moPLC.PLCData = sPLCData
            '''''''''''        Application.DoEvents()
            '''''''''''    Next 'nStyle
            '''''''''''Else
            '''''''''''    moPLC.TagName = sTag
            '''''''''''    moPLC.PLCData = sData
            '''''''''''End If


            ''''''''''''room to pop up error if it happens
            '''''''''''Application.DoEvents()
            '''''''''''''
            ''''''''''''' ASCII data store in a separate PLC data 
            ''''''''''''' use a global or use a variable set in the collection ???
            ''''''''''''' 
            ''''''''''''If mcolStyles.UseAscii = True Then
            ''''''''''''    ReDim sData(ub%)

            ''''''''''''    For nIndex = 0 To ub%
            ''''''''''''        sData(nIndex) = mMathFunctions.CvASCIIToInteger( _
            ''''''''''''                 mcolStyles.Item(nIndex).PlantNumber.Text).ToString
            ''''''''''''    Next

            ''''''''''''    sTag = mcolZones.ActiveZone.PLCTagPrefix & "PlantAsciiStyles"
            ''''''''''''    moPLC.TagName = sTag
            ''''''''''''    moPLC.PLCData = sData
            ''''''''''''    'room to pop up error if it happens
            ''''''''''''    Application.DoEvents()

            ''''''''''''End If

            '''''''''''If mcolStyles.TwoCoat Then
            '''''''''''    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''''''''''    'jbw added for two coats 2/22/11
            '''''''''''    'jbw FIXED conversion from BOOL to STRING to INT
            '''''''''''    Dim tempint As Integer
            '''''''''''    ReDim sData(ub%)
            '''''''''''    For nIndex = 0 To ub%
            '''''''''''        If (mcolStyles(nIndex).TwoCoatStyle.Value) = False Then
            '''''''''''            tempint = 0
            '''''''''''        Else
            '''''''''''            tempint = 1
            '''''''''''        End If
            '''''''''''        sData(nIndex) = tempint.ToString(mLanguage.CurrentCulture)
            '''''''''''    Next

            '''''''''''    sTag = mcolZones.ActiveZone.PLCTagPrefix & "TwoCoatStyle"
            '''''''''''    moPLC.TagName = sTag
            '''''''''''    ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
            '''''''''''    sTmp = moPLC.PLCData
            '''''''''''    If sTmp.GetUpperBound(0) >= sData.GetUpperBound(0) Then
            '''''''''''        For nItem As Integer = 0 To sData.GetUpperBound(0)
            '''''''''''            sTmp(nItem) = sData(nItem)
            '''''''''''        Next
            '''''''''''        moPLC.PLCData = sTmp
            '''''''''''    Else
            '''''''''''        'Let the PLC form complain about it if the PLC link is shorter than the DB setup
            '''''''''''        moPLC.PLCData = sData
            '''''''''''    End If

            '''''''''''End If

            '''''''''''If mcolStyles.StyleRegisters Then
            '''''''''''    ReDim sData(ub%)
            '''''''''''    For nIndex = 0 To ub%
            '''''''''''        sData(nIndex) = mcolStyles(nIndex).StyleRegister.Value.ToString
            '''''''''''    Next
            '''''''''''    sTag = mcolZones.ActiveZone.PLCTagPrefix & "StyleRegisters"
            '''''''''''    moPLC.TagName = sTag
            '''''''''''    ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
            '''''''''''    sTmp = moPLC.PLCData
            '''''''''''    If sTmp.GetUpperBound(0) >= sData.GetUpperBound(0) Then
            '''''''''''        For nItem As Integer = 0 To sData.GetUpperBound(0)
            '''''''''''            sTmp(nItem) = sData(nItem)
            '''''''''''        Next
            '''''''''''        moPLC.PLCData = sTmp
            '''''''''''    Else
            '''''''''''        'Let the PLC form complain about it if the PLC link is shorter than the DB setup
            '''''''''''        moPLC.PLCData = sData
            '''''''''''    End If

            '''''''''''End If

            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            Application.DoEvents()

            If Not mbPLCFail Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bSaveStyleIDToPLC() As Boolean
        '********************************************************************************************
        'Description:  Save data stored on PLC.
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
        '********************************************************************************************
        Dim bSaveSuccess As Boolean = False
        Dim nStyleCount As Integer = 0
        Dim nIndex As Integer = 0
        Dim nDataOffset As Integer
        Dim sData() As String
        Dim sTag As String
        Dim ub%

        Try

            moPLC.ZoneName = mcolZones.ActiveZone.Name

            If mcolStyleID.NumberOfConveyorSnapshots > 0 Then
                ub% = mcolStyleID.NumberOfConveyorSnapshots - 1
                nDataOffset = mnStyleIDIndex * mcolStyleID.NumberOfConveyorSnapshots

                ReDim sData(ub%)

                For nIndex = 0 To ub%
                    sData(nIndex) = mcolStyleID(nIndex + nDataOffset).ConveyorCount.Value.ToString
                Next

                sTag = mcolZones.ActiveZone.PLCTagPrefix & "ConveyorPositions" & mnStyleIDIndex + 1

                moPLC.TagName = sTag
                ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
                Dim sTmp() As String = moPLC.PLCData
                If sTmp.GetUpperBound(0) >= sData.GetUpperBound(0) Then
                    For nItem As Integer = 0 To sData.GetUpperBound(0)
                        sTmp(nItem) = sData(nItem)
                    Next
                    moPLC.PLCData = sTmp
                Else
                    'Let the PLC form complain about it if the PLC link is shorter than the DB setup
                    moPLC.PLCData = sData
                End If

                'room to pop up error if it happens
                Application.DoEvents()

                ReDim sData(ub%)

                For nIndex = 0 To ub%
                    sData(nIndex) = mcolStyleID(nIndex + nDataOffset).PhotoeyePattern.Value.ToString
                Next

                sTag = mcolZones.ActiveZone.PLCTagPrefix & "PhotoeyePattern" & mnStyleIDIndex + 1
                moPLC.TagName = sTag
                ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
                sTmp = moPLC.PLCData
                If sTmp.GetUpperBound(0) >= sData.GetUpperBound(0) Then
                    For nItem As Integer = 0 To sData.GetUpperBound(0)
                        sTmp(nItem) = sData(nItem)
                    Next
                    moPLC.PLCData = sTmp
                Else
                    'Let the PLC form complain about it if the PLC link is shorter than the DB setup
                    moPLC.PLCData = sData
                End If

                'room to pop up error if it happens
                Application.DoEvents()

                ReDim sData(ub%)

                For nIndex = 0 To ub%
                    sData(nIndex) = mcolStyleID(nIndex + nDataOffset).PhotoeyeIgnore.Value.ToString
                Next

                sTag = mcolZones.ActiveZone.PLCTagPrefix & "PhotoeyeIgnorePattern" & mnStyleIDIndex + 1
                moPLC.TagName = sTag
                ' 04/04/12  MSW     Handle plc link lengths larger than required, only fault if it's shorter
                sTmp = moPLC.PLCData
                If sTmp.GetUpperBound(0) >= sData.GetUpperBound(0) Then
                    For nItem As Integer = 0 To sData.GetUpperBound(0)
                        sTmp(nItem) = sData(nItem)
                    Next
                    moPLC.PLCData = sTmp
                Else
                    'Let the PLC form complain about it if the PLC link is shorter than the DB setup
                    moPLC.PLCData = sData
                End If

                'room to pop up error if it happens
                Application.DoEvents()

                'ReDim sData(ub%)

                'For nIndex = 0 To ub%
                '    sData(nIndex) = mcolStyleID(nIndex + nDataOffset).SunroofPhotoeyePattern.Value.ToString
                'Next

                'sTag = mcolZones.ActiveZone.PLCTagPrefix & "SunroofPhotoeyePattern" & mnStyleIDIndex + 1
                'moPLC.TagName = sTag
                'moPLC.PLCData = sData
                ''room to pop up error if it happens
                'Application.DoEvents()

                'ReDim sData(ub%)

                'For nIndex = 0 To ub%
                '    sData(nIndex) = mcolStyleID(nIndex + nDataOffset).SunroofPhotoeyeIgnore.Value.ToString
                'Next

                'sTag = mcolZones.ActiveZone.PLCTagPrefix & "SunRoofPhotoeyeIgnorePattern" & mnStyleIDIndex + 1
                'moPLC.TagName = sTag
                'moPLC.PLCData = sData
                ''room to pop up error if it happens
                'Application.DoEvents()

                If Not mbPLCFail Then
                    Return True
                Else
                    Return False
                End If

            Else

                sTag = mcolZones.ActiveZone.PLCTagPrefix & "PhotoeyePattern"
                moPLC.TagName = sTag
                sData = moPLC.PLCData

                sData(mnStyleIDIndex) = mcolStyleID(0).PhotoeyePattern.Value.ToString

                moPLC.PLCData = sData
                Return True
            End If

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

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
        ' 10/25/13  BTK     Tricoat By Style
        '********************************************************************************************
        Select Case msCurrentTabName
            Case "TabPage1"
                Return bSaveStylesToGUI()
            Case "TabPage2"
                Return bSaveOptionsToGUI()
            Case "TabPage3"
                Return bSaveRepairsToGUI()
            Case "TabPage4"
                Return bSaveDegradeToGUI()
            Case "TabPage5"
                Return bSaveIntrusionToGUI()
            Case "TabPage6"
                Return bSaveStyleIDToSQLDB()
            Case "TabPage7" ' 10/25/13  BTK     Tricoat By Style
                Return bSaveStyleTricoatEnbToSQLDB()
        End Select

    End Function
    Private Function bSaveToPLC() As Boolean
        '********************************************************************************************
        'Description:  Save data stored on PLC.
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/25/13  BTK     Tricoat By Style
        '********************************************************************************************

        mbPLCFail = False

        Select Case msCurrentTabName
            Case "TabPage1"
                Return bSaveStylesToPLC()
            Case "TabPage2"
                Return bSaveOptionsToPLC()
            Case "TabPage3"
                Return bSaveRepairsToPLC()
            Case "TabPage4"
                Return bSaveDegradeToPLC()
            Case "TabPage5"
                Return bSaveIntrusionToPLC()
            Case "TabPage6"
                Return bSaveStyleIDToPLC()
            Case "TabPage7" ' 10/25/13  BTK     Tricoat By Style
                Return bSaveStyleTricoatEnbToPLC()
        End Select

    End Function
    Private Function bValidateData() As Boolean
        '********************************************************************************************
        'Description:  Check each Integer Value field of each Style in the Styles Collection for 
        '              valid data. They may contain invalid data from the DB or PLC if they haven't
        '              been shown on the screen yet.
        '
        'Parameters: none
        'Returns:    false if bum data so it can be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        ' temp hack to fire validate
        cboZone.Focus()

        Application.DoEvents()

        Select Case msCurrentTabName
            Case "TabPage1"
                Return bValidateStyleData()
            Case "TabPage2"
                Return bValidateOptionData()
            Case "TabPage3"
                Return bValidateRepairData()
            Case "TabPage4"
                Return bValidateDegradeData()
            Case "TabPage5"
                Return bValidateIntrusionData()
            Case "TabPage6"
                Return bValidateStyleIDData()
            Case "TabPage7"
                Return bValidateStyleTricoatEnbData()
        End Select

    End Function
    Private Function bValidateOptionData() As Boolean
        '********************************************************************************************
        'Description:  Check each Integer Value field of each Option in the Options Collection for 
        '              valid data. They may contain invalid data from the DB or PLC if they haven't
        '              been shown on the screen yet.
        '
        'Parameters: none
        'Returns:    false if bum data so it can be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nItem As Integer = 0
        Dim sColumn As String = String.Empty
        Dim sKey As String = String.Empty
        Dim sReason As String = gcsRM.GetString("csVAL_BELOW_MIN", DisplayCulture)
        Dim oO As clsSysOption = Nothing
        Dim oFText As FocusedTextBox.FocusedTextBox = Nothing

        Dim bValid As Boolean = True
        For Each oO In mcolOptions
            nItem = oO.ItemNumber
            If Not mcolOptions.UseAscii Then
                'Bad data
                sColumn = "ftbPlantOption"
                If CType(oO.PlantNumber.Text, Integer) < mcolOptions.PlantOptionMinValue Then
                    bValid = False
                    Exit For
                End If
                If CType(oO.PlantNumber.Text, Integer) > mcolOptions.PlantOptionMaxValue Then
                    bValid = False
                    sReason = gcsRM.GetString("csVAL_ABOVE_MAX", DisplayCulture)
                    Exit For
                End If
            End If 'mScreenData.AsciiOption = False
            If oO.FanucNumber.Valid = False Then
                'Bad data
                sColumn = "ftbTxtFanucOption"
                bValid = False
                If oO.FanucNumber.Value > oO.FanucNumber.MaxValue Then
                    sReason = gcsRM.GetString("csVAL_ABOVE_MAX", DisplayCulture)
                End If
                Exit For
            End If
        Next 'oS

        If bValid = False Then
            'display bad data field and shift focus to it
            sKey = sColumn & Format((nItem + 1), "000")
            oFText = DirectCast(pnlOptions.Controls(sKey), FocusedTextBox.FocusedTextBox)
            oFText.Focus()
            MessageBox.Show(sReason, gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                 MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Return True

    End Function
    Private Function bValidateDegradeData() As Boolean
        '********************************************************************************************
        'Description:  Check each Integer Value field of each Repair in the Repairs Collection for 
        '              valid data. They may contain invalid data from the DB or PLC if they haven't
        '              been shown on the screen yet.
        '
        'Parameters: none
        'Returns:    false if bum data so it can be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nItem As Integer = 0
        Dim sColumn As String = String.Empty
        Dim sKey As String = String.Empty
        Dim sReason As String = gcsRM.GetString("csVAL_BELOW_MIN", DisplayCulture)
        Dim oO As clsSysOption = Nothing
        Dim oFText As FocusedTextBox.FocusedTextBox = Nothing

        'this only validates what is on screen
        Dim bValid As Boolean = True

        For nItem = 1 To mcolArms.Count
            sColumn = "ftbDegrade" & Format(nItem, "000")
            oFText = DirectCast(pnlDegrade.Controls(sColumn), FocusedTextBox.FocusedTextBox)
            If oFText.Text = String.Empty Then
                bValid = False
            End If
            If CType(oFText.Text, Integer) < 0 Then
                bValid = False

            End If
            If CType(oFText.Text, Integer) > mcolArms.Count Then
                bValid = False
                sReason = gcsRM.GetString("csVAL_ABOVE_MAX", DisplayCulture)
            End If
            If bValid = False Then
                'display bad data field and shift focus to it
                oFText.Focus()
                MessageBox.Show(sReason, gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                     MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Return False
            End If


        Next


        Return True

    End Function
    Private Function bValidateIntrusionData() As Boolean
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    false if bum data so it can be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/22/10  BTK     We can't use one job space for exit start muting upper limit.
        '********************************************************************************************
        Dim nItem As Integer = 0
        Dim sColumn As String = String.Empty
        Dim sKey As String = String.Empty
        Dim sReason As String = gcsRM.GetString("csVAL_BELOW_MIN", DisplayCulture)
        Dim oO As clsSysOption = Nothing
        Dim oFText As FocusedTextBox.FocusedTextBox = Nothing

        'this only validates what is on screen
        Dim bValid As Boolean = True

        For nItem = 1 To mcolArms.Count

            sColumn = "ftbEntStart" & Format(nItem, "000")
            oFText = DirectCast(pnlIntrusion.Controls(sColumn), FocusedTextBox.FocusedTextBox)
            If oFText.Text = String.Empty Then
                bValid = False
            End If
            If CType(oFText.Text, Integer) < 0 Then
                bValid = False
            End If

            If CType(oFText.Text, Integer) > mnDogSpace Then
                bValid = False
                sReason = gcsRM.GetString("csVAL_ABOVE_MAX", DisplayCulture)
            End If
            If bValid = False Then
                'display bad data field and shift focus to it
                oFText.Focus()
                MessageBox.Show(sReason, gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                     MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Return False
            End If


            sColumn = "ftbExitStart" & Format(nItem, "000")
            oFText = DirectCast(pnlIntrusion.Controls(sColumn), FocusedTextBox.FocusedTextBox)
            If oFText.Text = String.Empty Then
                bValid = False
            End If
            If CType(oFText.Text, Integer) < 0 Then
                bValid = False
            End If
            'BTK 02/22/10  We can't use one dog space for upper limit.
            'This can't be the upper limit.  How do you set the values for photoeyes that are further
            'then a job space.
            If CType(oFText.Text, Integer) > mnDogSpace * 3 Then
                bValid = False
                sReason = gcsRM.GetString("csVAL_ABOVE_MAX", DisplayCulture)
            End If
            If bValid = False Then
                'display bad data field and shift focus to it
                oFText.Focus()
                MessageBox.Show(sReason, gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                     MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Return False
            End If

            sColumn = "ftbMuteLen" & Format(nItem, "000")
            oFText = DirectCast(pnlIntrusion.Controls(sColumn), FocusedTextBox.FocusedTextBox)
            If oFText.Text = String.Empty Then
                bValid = False
            End If
            If CType(oFText.Text, Integer) < 0 Then
                bValid = False
            End If
            If CType(oFText.Text, Integer) > mnDogSpace Then
                bValid = False
                sReason = gcsRM.GetString("csVAL_ABOVE_MAX", DisplayCulture)
            End If
            If bValid = False Then
                'display bad data field and shift focus to it
                oFText.Focus()
                MessageBox.Show(sReason, gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                     MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Return False
            End If

        Next


        Return True

    End Function
    Private Function bValidateRepairData() As Boolean
        '********************************************************************************************
        'Description:  Check each Integer Value field of each Repair in the Repairs Collection for 
        '              valid data. They may contain invalid data from the DB or PLC if they haven't
        '              been shown on the screen yet.
        '
        'Parameters: none
        'Returns:    false if bum data so it can be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nItem As Integer = 0
        Dim sColumn As String = String.Empty
        Dim sKey As String = String.Empty
        Dim sReason As String = gcsRM.GetString("csVAL_BELOW_MIN", DisplayCulture)
        Dim obR As clsSysRepairPanel = Nothing
        Dim oFText As FocusedTextBox.FocusedTextBox = Nothing

        bValidateRepairData = True

        ' Nothing to validate this can be removed

        If bValidateRepairData = False Then
            'display bad data field and shift focus to it
            sKey = sColumn & Format((nItem + 1), "000")
            oFText = DirectCast(GetControlByName(sKey, Me), FocusedTextBox.FocusedTextBox)
            oFText.Focus()
            MessageBox.Show(sReason, gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                 MessageBoxButtons.OK, MessageBoxIcon.Warning)

        End If

        ' End If
        'TODO - Lop off any extra bits from Robots required before saving

    End Function
    Private Function bValidateStyleData() As Boolean
        '********************************************************************************************
        'Description:  Check each Integer Value field of each Style in the Styles Collection for 
        '              valid data. They may contain invalid data from the DB or PLC if they haven't
        '              been shown on the screen yet.
        '
        'Parameters: none
        'Returns:    false if bum data so it can be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/13/11  RJO     Added support for Sealer combined Style/Option
        '********************************************************************************************
        Dim nItem As Integer = 0
        Dim sColumn As String = String.Empty
        Dim sKey As String = String.Empty
        Dim sReason As String = gcsRM.GetString("csVAL_BELOW_MIN", DisplayCulture)
        Dim oS As clsSysStyle = Nothing
        Dim oFText As FocusedTextBox.FocusedTextBox = Nothing
        Dim bValid As Boolean = True

        For Each oS In mcolStyles
            nItem = oS.ItemNumber
            If mcolStyles.UseAscii = False Then
                'Bad data
                sColumn = "ftbPlantStyle"
                If CType(oS.PlantNumber.Text, Integer) > mcolStyles.PlantStyleMaxValue Then
                    sReason = gcsRM.GetString("csVAL_ABOVE_MAX", DisplayCulture)
                    bValid = False
                    Exit For
                End If
                If CType(oS.PlantNumber.Text, Integer) < mcolStyles.PlantStyleMinValue Then
                    bValid = False
                    Exit For
                End If
                If mcolStyles.CombinedStyleOption Then 'RJO 12/13/11
                    sColumn = "ftbStyleOpt"
                    If CType(oS.OptionNumber.Text, Integer) > mcolStyles.PlantOptionMaxValue Then
                        sReason = gcsRM.GetString("csVAL_ABOVE_MAX", DisplayCulture)
                        bValid = False
                        Exit For
                    End If
                    If CType(oS.OptionNumber.Text, Integer) < mcolStyles.PlantOptionMinValue Then
                        bValid = False
                        Exit For
                    End If
                End If 'mcolStyles.CombinedStyleOption
            End If 'mScreenData.AsciiStyle = False

            If oS.FanucNumber.Valid = False Then
                'Bad data
                sColumn = "ftbFanucStyle"
                bValid = False
                If oS.FanucNumber.Value > oS.FanucNumber.MaxValue Then
                    sReason = gcsRM.GetString("csVAL_ABOVE_MAX", DisplayCulture)
                End If
                Exit For
            End If
        Next 'oS

        If bValid = False Then
            'display bad data field and shift focus to it
            sKey = sColumn & Format((nItem + 1), "000")
            oFText = DirectCast(pnlStyles.Controls(sKey), FocusedTextBox.FocusedTextBox)
            'oFText = DirectCast(GetControlByName(sKey, Me), FocusedTextBox.FocusedTextBox)
            oFText.Focus()
            MessageBox.Show(sReason, gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                 MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Return True

    End Function

    Private Function bValidateStyleIDData() As Boolean
        '********************************************************************************************
        'Description:  Check each Integer Value field of each Style in the Styles Collection for 
        '              valid data. They may contain invalid data from the DB or PLC if they haven't
        '              been shown on the screen yet.
        '
        'Parameters: none
        'Returns:    false if bum data so it can be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nItem As Integer = 0
        Dim nTag As Integer
        Dim sColumn As String = String.Empty
        Dim sKey As String = String.Empty
        Dim sReason As String = gcsRM.GetString("csVAL_BELOW_MIN", DisplayCulture)
        Dim oS As clsSysStyleID = Nothing
        Dim oFText As FocusedTextBox.FocusedTextBox = Nothing
        Dim bValid As Boolean = True

        For nItem = (mnStyleIDIndex * mcolStyleID.NumberOfConveyorSnapshots) To _
            (mnStyleIDIndex * mcolStyleID.NumberOfConveyorSnapshots) + mcolStyleID.NumberOfConveyorSnapshots - 1
            oS = mcolStyleID(nItem)
            nTag = nItem - (mnStyleIDIndex * mcolStyleID.NumberOfConveyorSnapshots)
            'Bad data
            sColumn = "ftbIDPosition"
            If CType(oS.ConveyorCount.Value, Integer) > mcolStyleID.IDPositionUpperLimit Then
                sReason = gcsRM.GetString("csVAL_ABOVE_MAX", DisplayCulture)
                bValid = False
                Exit For
            End If
            If CType(oS.ConveyorCount.Value, Integer) < mcolStyleID.IDPositionLowerLimit Then
                bValid = False
                Exit For
            End If
        Next 'oS

        If bValid = False Then
            'display bad data field and shift focus to it
            sKey = sColumn & Format((nTag + 1), "000")
            oFText = DirectCast(pnlStyleID.Controls(sKey), FocusedTextBox.FocusedTextBox)
            'oFText = DirectCast(GetControlByName(sKey, Me), FocusedTextBox.FocusedTextBox)
            oFText.Focus()
            MessageBox.Show(sReason, gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                 MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Return True

    End Function
    Private Function oGetDegradeStyle() As clsSysStyle
        '********************************************************************************************
        'Description:  New Parameter selected
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'check both combos have a selection
        If cboPlantStyle.Text = String.Empty Then Return Nothing
        If cboController.Text = String.Empty Then Return Nothing
        Try
            Dim nP As String() = CType(cboPlantStyle.Tag, String())

            Dim o As clsSysStyle = mcolStyles(nP(cboPlantStyle.SelectedIndex), False)
            Return o

        Catch ex As Exception
            Return Nothing
        End Try

    End Function
    Private Function nGetDegradeControllerNum() As Integer
        '********************************************************************************************
        'Description:  New Parameter selected
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            'check both combos have a selection
            If cboPlantStyle.Text = String.Empty Then Return -1
            If cboController.Text = String.Empty Then Return -1

            Dim nC As Integer() = CType(cboController.Tag, Integer())

            Return (nC(cboController.SelectedIndex))

        Catch ex As Exception
            Return -1
        End Try
    End Function
    Private Sub subChangeParameter()
        '********************************************************************************************
        'Description:  New Parameter selected
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'not used
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
        ' 12/13/11  RJO     Reloacated visibility control for TwoCoat option.
        ' 07/08/13  BTK     Tricoat By Style
        ' 10/30/13  BTK     Degrade Style Robots Required
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
        mcolControllers = New clsControllers(mcolZones, False)
        mcolArms = mPWRobotCommon.LoadArmCollection(mcolControllers)

        'RJO 07/23/14 Added to support 16+ robots. Making the assumption that systems with 16+
        'robots store Robots Required in DINTs.
        mb32Bit = False
        For Each oArm As clsArm In mcolArms
            If oArm.RobotNumber >= 15 Then
                mb32Bit = True
                Exit For
            End If
        Next

        'MSW 9/7/11 store the names we load this way so cboRobot can be used on the screen
        If mcolZones.ActiveZone.UseSplitShell Then
            mnNumberofRobots = mcolArms.Count
            ReDim msRobotClbNames(mnNumberofRobots - 1)
            For nRobot As Integer = 0 To mcolArms.Count - 1
                msRobotClbNames(nRobot) = mcolArms(nRobot).Name
            Next
        Else
            mnNumberofRobots = mcolControllers.Count
            ReDim msRobotClbNames(mnNumberofRobots - 1)
            For nRobot As Integer = 0 To mcolControllers.Count - 1
                msRobotClbNames(nRobot) = mcolControllers(nRobot).Name
            Next
        End If
        mbStylesTabInitialized = False
        mbOptionsTabInitialized = False
        mbRepairsTabInitialized = False
        mbDegradeTabInitialized = False
        mbStyleIDTabInitialized = False
        ' 07/08/13  BTK     Tricoat By Style
        mbStyleTricoatEnbTabInitialized = False

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subEnableControls(False)
            EditsMade = False
            DataLoaded = False

            tabMain.TabPages.Item(0).Select()

            If mcolZones.ActiveZone.MaxOptions > 0 Then
                If tabMain.TabPages.ContainsKey("TabPage2") = False Then
                    Dim t1 As TabPage = mHiddenTabs.TabPages.Item("TabPage2") 'Options
                    mHiddenTabs.TabPages.RemoveByKey("TabPage2")
                    tabMain.TabPages.Add(t1)
                End If
            Else
                'no Options
                If tabMain.TabPages.ContainsKey("TabPage2") Then
                    Dim t1 As TabPage = tabMain.TabPages.Item("TabPage2") 'Options
                    tabMain.TabPages.RemoveByKey("TabPage2")
                    mHiddenTabs.TabPages.Add(t1)
                End If

            End If

            If mcolZones.ActiveZone.RepairPanels > 0 Then
                If tabMain.TabPages.ContainsKey("TabPage3") = False Then
                    Dim t1 As TabPage = mHiddenTabs.TabPages.Item("TabPage3") 'repair
                    mHiddenTabs.TabPages.RemoveByKey("TabPage3")
                    tabMain.TabPages.Add(t1)
                End If
            Else
                'no repairs
                If tabMain.TabPages.ContainsKey("TabPage3") Then
                    Dim t1 As TabPage = tabMain.TabPages.Item("TabPage3") 'repair
                    tabMain.TabPages.RemoveByKey("TabPage3")
                    mHiddenTabs.TabPages.Add(t1)
                End If

            End If

            If mcolZones.ActiveZone.DegradeEnabled Then
                If tabMain.TabPages.ContainsKey("TabPage4") = False Then
                    Dim t1 As TabPage = mHiddenTabs.TabPages.Item("TabPage4") 'degrade
                    mHiddenTabs.TabPages.RemoveByKey("TabPage4")
                    tabMain.TabPages.Add(t1)
                End If
            Else
                'no degrade
                If tabMain.TabPages.ContainsKey("TabPage4") Then
                    Dim t1 As TabPage = tabMain.TabPages.Item("TabPage4") 'degrade
                    tabMain.TabPages.RemoveByKey("TabPage4")
                    mHiddenTabs.TabPages.Add(t1)
                End If

            End If

            If mcolZones.ActiveZone.IntrusionSetupEnabled Then
                If tabMain.TabPages.ContainsKey("TabPage5") = False Then
                    Dim t1 As TabPage = mHiddenTabs.TabPages.Item("TabPage5") 'Intrusion
                    mHiddenTabs.TabPages.RemoveByKey("TabPage5")
                    tabMain.TabPages.Add(t1)
                End If
            Else
                'no degrade
                If tabMain.TabPages.ContainsKey("TabPage5") Then
                    Dim t1 As TabPage = tabMain.TabPages.Item("TabPage5") 'Intrusion
                    tabMain.TabPages.RemoveByKey("TabPage5")
                    mHiddenTabs.TabPages.Add(t1)
                End If

            End If

            If mcolZones.ActiveZone.StyleIDEnabled Then
                If tabMain.TabPages.ContainsKey("TabPage6") = False Then
                    Dim t1 As TabPage = mHiddenTabs.TabPages.Item("TabPage6") 'Style ID
                    mHiddenTabs.TabPages.RemoveByKey("TabPage6")
                    tabMain.TabPages.Add(t1)
                End If
            Else
                'no style id
                If tabMain.TabPages.ContainsKey("TabPage6") Then
                    Dim t1 As TabPage = tabMain.TabPages.Item("TabPage6") 'Style ID
                    tabMain.TabPages.RemoveByKey("TabPage6")
                    mHiddenTabs.TabPages.Add(t1)
                End If

            End If

            ' 07/08/13  BTK     Tricoat By Style
            If mcolZones.ActiveZone.TricoatByStyle Then
                If tabMain.TabPages.ContainsKey("TabPage7") = False Then
                    Dim t1 As TabPage = mHiddenTabs.TabPages.Item("TabPage7") 'tricoat by style
                    mHiddenTabs.TabPages.RemoveByKey("TabPage7")
                    tabMain.TabPages.Add(t1)
                End If
            Else
                'no tricoat by style
                If tabMain.TabPages.ContainsKey("TabPage7") Then
                    Dim t1 As TabPage = tabMain.TabPages.Item("TabPage7") 'tricoat by style
                    tabMain.TabPages.RemoveByKey("TabPage7")
                    mHiddenTabs.TabPages.Add(t1)
                End If

            End If

            If mbScreenLoaded = True Then
                'For SA only load from the database
                If mcolZones.StandAlone Then
                    subLoadData(eLoadModes.LoadFromDB)
                Else
                    subLoadData(eLoadModes.LoadFromPLC)
                End If
            End If

            'This doesn't belong here. Commented out. 'RJO 12/13/11
            'If lbltwocoats.Visible <> mcolStyles.TwoCoat Then
            '    Dim bTmp As Boolean = mcolStyles.TwoCoat
            '    lbltwocoats.Visible = bTmp
            '    cbTwoCoats001.Visible = bTmp
            'End If

            If lblStyleRegister.Visible <> mcolStyles.StyleRegisters Then
                Dim bTmp As Boolean = mcolStyles.StyleRegisters
                lblStyleRegister.Visible = bTmp
                ftbStyleRegister001.Visible = bTmp
            End If

            Dim mnuLoad As ToolStripDropDownMenu = CType(btnUpload.DropDown, ToolStripDropDownMenu)
            Dim nItem As Integer = 0
            Dim sText As String = String.Empty
            Dim void As System.Drawing.Image = Nothing

            With mnuLoad
                .Name = "mnuLoad"
                .Items.Clear()
                sText = gcsRM.GetString("csLOAD_FROM") & " " & gcsRM.GetString("csPLC")
                .Items.Add(sText, void, AddressOf subLoadMenuHandler)
                .Items(nItem).Name = gcsRM.GetString("csPLC")
                nItem += 1
                sText = gcsRM.GetString("csLOAD_FROM") & " " & gcsRM.GetString("csDATABASE")
                .Items.Add(sText, void, AddressOf subLoadMenuHandler)
                .Items(nItem).Name = gcsRM.GetString("csDATABASE")
            End With
            btnUpload.DropDown = mnuLoad

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try


    End Sub
    Private Sub subEnableControls(ByVal bEnable As Boolean)
        '********************************************************************************************
        'Description: Disable or enable controls. check privileges and edits etc. 
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
        Dim bEnableCopy As Boolean = (tabMain.SelectedTab.Name = "TabPage3")
        btnCopy.Visible = bEnableCopy
        If bEnable = False Then
            btnSave.Enabled = False
            btnUndo.Enabled = False
            btnPrint.Enabled = False
            btnChangeLog.Enabled = False
            btnUpload.Enabled = False
            btnStatus.Enabled = True
            btnCopy.Enabled = False
            bRestOfControls = False
            PnlMain.Enabled = False
            mnuPrintFile.Enabled = False
            btnUtilities.Enabled = False
            'cboRobot.Enabled = False
        Else
            Select Case Privilege
                Case ePrivilege.None
                    btnSave.Enabled = False
                    btnUndo.Enabled = False
                    btnPrint.Enabled = (True And DataLoaded)
                    btnChangeLog.Enabled = (True And DataLoaded)
                    btnUpload.Enabled = (True And DataLoaded)
                    btnStatus.Enabled = True
                    btnCopy.Enabled = False
                    bRestOfControls = False
                    PnlMain.Enabled = True
                    mnuPrintFile.Enabled = False
                    btnUtilities.Enabled = False
                    cboRobot.Enabled = True
                Case ePrivilege.Edit
                    btnSave.Enabled = (True And EditsMade)
                    btnUndo.Enabled = (True And EditsMade)
                    btnPrint.Enabled = (True And DataLoaded)
                    btnChangeLog.Enabled = (True And DataLoaded)
                    btnUpload.Enabled = (True And DataLoaded)
                    btnStatus.Enabled = True
                    btnCopy.Enabled = (bEnableCopy And DataLoaded)
                    bRestOfControls = (True And DataLoaded)
                    PnlMain.Enabled = (True And DataLoaded)
                    mnuPrintFile.Enabled = (True And DataLoaded)
                    Select Case tabMain.SelectedTab.Name
                        Case "TabPage6"
                            btnUtilities.Enabled = False
                        Case Else
                            btnUtilities.Enabled = (True And DataLoaded)
                    End Select
                    cboRobot.Enabled = (True And DataLoaded)
            End Select
        End If

        '*************************************************************************
        'restof controls here
        LockAllTextBoxes(mcolStyleTextboxes, (Not bRestOfControls))
        LockAllTextBoxes(mcolOptionTextboxes, (Not bRestOfControls))
        LockAllTextBoxes(mcolDegradeTextboxes, (Not bRestOfControls))
        LockAllTextBoxes(mcolIntrusionTextboxes, (Not bRestOfControls))
        LockAllTextBoxes(mcolRepairTextboxes, (Not bRestOfControls))
        LockAllTextBoxes(mcolStyleIDPositionTextboxes, (Not bRestOfControls))
        'jbw 03/03/11 checkbox stuff
        LockAllCheckBoxes(mcolStyleCheckBoxes, (bRestOfControls))

        LockAllCheckedListBoxes(mcolStyleCLBoxes, (Not bRestOfControls))
        LockAllCheckedListBoxes(mcolOptionCLBoxes, (Not bRestOfControls))
        LockAllCheckedListBoxes(mcolRepairCLBoxes, (Not bRestOfControls))
        LockAllButtons(mcolStyleIDPEButtons, (Not bRestOfControls))


    End Sub
    Private Sub subEnableRestore(ByVal bEnable As Boolean)
        '********************************************************************************************
        'Description: Disable or enable Restore Button on main menu only. 
        '
        'Parameters: bEnable - false can be used to disable during load etc 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        btnRestore.Enabled = bEnable
        btnRestore.Visible = bEnable
        tlsMain.Refresh()

    End Sub
    Private Sub subFtbAddHandlers(ByRef Col As Collection(Of FocusedTextBox.FocusedTextBox), _
                                  ByVal ContainerName As String)
        '********************************************************************************************
        'Description:  Add event handlers for the newly created controls
        '
        'Parameters:
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/18/09  MSW     subTextboxKeypressHandler, subFtbAddHandlers - Offer a login if they try to enter data without a login        
        ' 06/30/10  MSW     handle page up, page down, home, end
        '********************************************************************************************
        Dim oFtb As FocusedTextBox.FocusedTextBox

        For Each oFtb In Col
            AddHandler oFtb.UpArrow, AddressOf ftbAll_UpArrow
            AddHandler oFtb.DownArrow, AddressOf ftbAll_DownArrow
            AddHandler oFtb.LeftArrow, AddressOf ftbAll_LeftArrow
            AddHandler oFtb.RightArrow, AddressOf ftbAll_RightArrow
            AddHandler oFtb.TextChanged, AddressOf ftbAll_TextChanged
            AddHandler oFtb.KeyPress, AddressOf subTextboxKeypressHandler
            AddHandler oFtb.KeyUp, AddressOf subTextboxKeyUpHandler '' 06/30/10  MSW     handle page up, page down, home, end
            Select Case ContainerName
                Case "pnlStyles"
                    AddHandler oFtb.Validating, AddressOf ftbStyle_Validating
                    AddHandler oFtb.Validated, AddressOf ftbStyle_Validated
                Case "pnlOptions"
                    AddHandler oFtb.Validating, AddressOf ftbOption_Validating
                    AddHandler oFtb.Validated, AddressOf ftbOption_Validated
                Case "pnlRepairs"
                    AddHandler oFtb.Validating, AddressOf ftbRepair_Validating
                    AddHandler oFtb.Validated, AddressOf ftbRepair_Validated
                Case "pnlDegrade"
                    AddHandler oFtb.Validating, AddressOf ftbDegrade_Validating
                    AddHandler oFtb.Validated, AddressOf ftbDegrade_Validated
                Case "pnlIntrusion"
                    AddHandler oFtb.Validating, AddressOf ftbIntrusion_Validating
                    AddHandler oFtb.Validated, AddressOf ftbIntrusion_Validated
                Case "pnlStyleID"
                    AddHandler oFtb.Validating, AddressOf ftbStyleID_Validating
                    AddHandler oFtb.Validated, AddressOf ftbStyleID_Validated
            End Select

        Next 'oFtb

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
        '02/22/11   JBW     Added twocoats.text stuff
        '12/13/11   RJO     Added support for Sealer combined Style/Option
        '10/25/13   BTK     Tricoat By Style
        '********************************************************************************************

        With gpsRM
            ' set labels for tab #1
            TabPage1.Text = .GetString("psTAB1CAP", DisplayCulture)
            lblPlantStyleCap.Text = .GetString("psPLANT_STYLE_CAP", DisplayCulture)
            lblStyleOptCap.Text = .GetString("psPLANT_OPTION_CAP", DisplayCulture) 'RJO 12/13/11
            lblFanucStyleCap.Text = .GetString("psFANUC_STYLE_CAP", DisplayCulture)
            lblStyleDescCap.Text = .GetString("psDESC_CAP", DisplayCulture)
            lblStyleRobotsReqCap.Text = .GetString("psROBS_REQD_CAP", DisplayCulture)
            lbltwocoats.Text = .GetString("psTWO_COATS_CAP", DisplayCulture)
            lblStyleRegister.Text = .GetString("psREGISTER_INIT_CAP", DisplayCulture)

            ' set labels for tab #2
            TabPage2.Text = .GetString("psTAB2CAP", DisplayCulture)
            lblPlantOptionCap.Text = .GetString("psPLANT_OPTION_CAP", DisplayCulture)
            lblFanucOptionCap.Text = .GetString("psFANUC_OPTION_CAP", DisplayCulture)
            lblOptionDescCap.Text = .GetString("psDESC_CAP", DisplayCulture)
            lblOptionRobotsReqCap.Text = .GetString("psROBS_REQD_CAP", DisplayCulture)
            lblOptionRegister.Text = .GetString("psREGISTER_INIT_CAP", DisplayCulture)

            ' set labels for tab #3
            TabPage3.Text = .GetString("psTAB3CAP", DisplayCulture)
            lblPanelCap.Text = .GetString("psPANEL_CAP", DisplayCulture)
            lblRepairDescCap.Text = .GetString("psDESC_CAP", DisplayCulture)
            lbRepairRobotsReqCap.Text = .GetString("psROBS_REQD_CAP", DisplayCulture)

            ' set labels for tab #4
            TabPage4.Text = .GetString("psTAB4CAP", DisplayCulture)
            lblPlantStyleCap01.Text = .GetString("psPLANT_STYLE_CAP", DisplayCulture)
            lblRobotDownCap.Text = .GetString("psROBOT_DOWN_CAP", DisplayCulture)
            lblDegradeStyleCap.Text = .GetString("psDEGRADE_NUMBER_CAP", DisplayCulture)

            ' set labels for tab #5
            TabPage5.Text = .GetString("psTAB5CAP", DisplayCulture)
            lblPlantStyleCap5.Text = .GetString("psPLANT_STYLE_CAP", DisplayCulture)
            lblDescCap5.Text = .GetString("psDESC_CAP", DisplayCulture)
            lblEntStartCap.Text = .GetString("psENT_START_CAP", DisplayCulture)
            lblExitStartCap.Text = .GetString("psEXIT_START_CAP", DisplayCulture)
            lblMuteLenCap.Text = .GetString("psMUTE_LEN_CAP", DisplayCulture)

            ' set labels for tab #6
            TabPage6.Text = .GetString("psTAB6CAP", DisplayCulture)
            lblConveyorPosCap.Text = .GetString("psConveyorIDPosition", DisplayCulture)
            lblPhotoeyePattern.Text = .GetString("psPhotoeyePattern", DisplayCulture)
            lblPlantStyleCap06.Text = .GetString("psPLANT_STYLE_CAP", DisplayCulture)

            '10/25/13   BTK     Tricoat By Style
            'set labels for tab #7
            TabPage7.Text = .GetString("psTAB7CAP", DisplayCulture)
            lblPlantStyleCap7.Text = .GetString("psPLANT_STYLE_CAP", DisplayCulture)
            lblDescCap7.Text = .GetString("psDESC_CAP", DisplayCulture)
            lblTricoatRobotsReqCap.Text = .GetString("psROBS_REQD_CAP", DisplayCulture)

        End With

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
        Dim lReply As System.Windows.Forms.DialogResult = Response.OK
        'Dim cachePriviledge As mPassword.ePrivilege 'RJO 03/21/12

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

        Status = gcsRM.GetString("csINITALIZING", DisplayCulture)
        Progress = 1

        Try
            DataLoaded = False
            EditsMade = False
            mbScreenLoaded = False

            subProcessCommandLine()
            Me.Show()

            mcolZones = New clsZones(String.Empty)


            mPrintHtml = New clsPrintHtml(msSCREEN_NAME, True, 30)

            'hide tabs until we need then
            mHiddenTabs = New TabControl
            Dim t1 As TabPage = tabMain.TabPages.Item("TabPage2") 'Options
            tabMain.TabPages.RemoveByKey("TabPage2")
            mHiddenTabs.TabPages.Add(t1)
            Dim t2 As TabPage = tabMain.TabPages.Item("TabPage3") 'repair
            tabMain.TabPages.RemoveByKey("TabPage3")
            mHiddenTabs.TabPages.Add(t2)
            Dim t3 As TabPage = tabMain.TabPages.Item("TabPage4") 'degrade
            tabMain.TabPages.RemoveByKey("TabPage4")
            mHiddenTabs.TabPages.Add(t3)
            Dim t4 As TabPage = tabMain.TabPages.Item("TabPage5") 'degrade
            tabMain.TabPages.RemoveByKey("TabPage5")
            mHiddenTabs.TabPages.Add(t4)
            Dim t5 As TabPage = tabMain.TabPages.Item("TabPage6") 'degrade
            tabMain.TabPages.RemoveByKey("TabPage6")
            mHiddenTabs.TabPages.Add(t5)

            ''For SA hide the option tab
            'If mcolZones.StandAlone Then
            '    Dim t6 As TabPage = tabMain.TabPages.Item("TabPage2") 'options
            '    tabMain.TabPages.RemoveByKey("TabPage2")
            '    mHiddenTabs.TabPages.Add(t6)
            'End If

            Dim t6 As TabPage = tabMain.TabPages.Item("TabPage7") 'degrade
            tabMain.TabPages.RemoveByKey("TabPage7")
            mHiddenTabs.TabPages.Add(t6)

            'hide Two Coat Options until we need it
            lbltwocoats.Visible = False
            cbTwoCoats001.Visible = False
            lblStyleRegister.Visible = False
            ftbStyleRegister001.Visible = False
            lblOptionRegister.Visible = False
            clbOptionRegister001.Visible = False

            Progress = 5

            Me.Text = gpsRM.GetString("psSCREENCAPTION", DisplayCulture)

            mScreenSetup.LoadZoneBox(cboZone, mcolZones, False)

            Progress = 30

            'more SA fun
            If mcolZones.StandAlone Then
                lblStyleRobotsReqCap.Visible = False
                clbStyleRobotsReq001.Visible = False
                lblPlantStyleCap.Visible = False
                ftbPlantStyle001.Visible = False
            End If

            'init new IPC and new Password 'RJO 03/21/12
            oIPC = New Paintworks_IPC.clsInterProcessComm
            moPassword = New clsPWUser
            Progress = 50

            mScreenSetup.InitializeForm(Me)
            Call subInitFormText()

            Progress = 70

            'init the old password for now
            'moPassword = New PWPassword.PasswordObject  'RJO 03/21/12
            'mPassword.InitPassword(moPassword, moPrivilege, LoggedOnUser, msSCREEN_NAME)
            'If LoggedOnUser <> String.Empty Then

            '    If moPrivilege.ActionAllowed Then
            '        cachePriviledge = mPassword.PrivilegeStringToEnum(moPrivilege.Privilege)
            '    Else
            '        cachePriviledge = ePrivilege.None
            '    End If

            '    Privilege = cachePriviledge

            'Else
            '    Privilege = ePrivilege.None
            'End If
            subEnableControls(True)

            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound

            'statusbar text
            If cboZone.Text = String.Empty Then
                Status(True) = gcsRM.GetString("csSELECT_ZONE", DisplayCulture)
            End If

            Progress = 98


            mbScreenLoaded = True
            If cboZone.Items.Count = 1 Then
                'this selects the zone in cases of just one zone. fires event to call subchangezone
                cboZone.Text = cboZone.Items(0).ToString
                subChangeZone()
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
    Private Sub subLoadData(ByVal vLoadMode As eLoadModes)
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

            Select Case vLoadMode
                Case eLoadModes.LoadFromDB
                    DataLoaded = bLoadFromGUI()
                Case eLoadModes.LoadFromPLC
                    DataLoaded = bLoadFromPLC()
            End Select

            Progress = 98

            If DataLoaded Then
                Status = gcsRM.GetString("csLOADDONE", DisplayCulture)

                If mcolStyles Is Nothing = False Then mcolStyles.Update()
                If mcolOptions Is Nothing = False Then mcolOptions.Update()


                subShowNewPage(True)

                'this resets edit flag
                DataLoaded = True

                If cboRobot.Visible Then subEnableControls(False)

                If cboRobot.Visible Then
                    cboRobot.Text = cboRobot.Items(0).ToString
                End If

                Status = gcsRM.GetString("csLOADDONE", DisplayCulture)

            Else
                Status = gcsRM.GetString("csLOADFAILED", DisplayCulture)
            End If

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default

        End Try

    End Sub
    Private Sub subLoadMenuHandler(ByVal sender As Object, _
                                    ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  One of Load sub menus clicked. mRobot should exist and have the working data
        '               because it should be loaded to be here
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)

        Try
            mbReLoad = True
            Select Case o.Name
                Case gcsRM.GetString("csPLC")
                    If bLoadFromPLC() = False Then

                        MessageBox.Show(gcsRM.GetString("csLOADFAILED"), o.Name, _
                                MessageBoxButtons.OK, MessageBoxIcon.Information, _
                                MessageBoxDefaultButton.Button1, _
                                MessageBoxOptions.DefaultDesktopOnly)
                    End If
                Case gcsRM.GetString("csDATABASE")
                    If bLoadFromGUI() = False Then

                        MessageBox.Show(gcsRM.GetString("csLOADFAILED"), o.Name, _
                                MessageBoxButtons.OK, MessageBoxIcon.Information, _
                                MessageBoxDefaultButton.Button1, _
                                MessageBoxOptions.DefaultDesktopOnly)

                    End If
                Case Else
                    'musta got lost, somewhere down the line

            End Select

        Catch ex As Exception

        Finally
            subShowNewPage(False)
            EditsMade = True
            mbReLoad = False
            Progress = 100

        End Try

    End Sub
    Private Sub subLoadDegradeTabData(ByVal nControllerSelected As Integer, ByRef oStyle As clsSysStyle)
        '********************************************************************************************
        'Description:  Load Degrade Tab data
        '
        'Parameters:  
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sName As String

        mbEventBlocker = True

        For Index As Integer = 1 To mcolArms.Count
            sName = "ftbDegrade" & Format(Index, "000")
            pnlDegrade.Controls.Item(sName).Text = _
                        oStyle.PlantSpecific.DegradeNumber(nControllerSelected, Index).Value.ToString
            If oStyle.PlantSpecific.DegradeNumber(nControllerSelected, Index).Changed Then
                pnlDegrade.Controls.Item(sName).ForeColor = Color.Red
            Else
                pnlDegrade.Controls.Item(sName).ForeColor = Color.Black
            End If

        Next 'nInx

        mbEventBlocker = False

    End Sub
    Private Sub subLoadOptionTabData(ByRef oOption As clsSysOption, ByVal Index As String)
        '********************************************************************************************
        'Description:  Load Option Tab data
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/23/09  BTK     Added code so we can change which bit the robot required data starts at.
        '********************************************************************************************
        Dim sName As String = String.Empty


        sName = "ftbPlantOption" & Index
        pnlOptions.Controls.Item(sName).Text = oOption.PlantNumber.Text
        If Not mbReLoad Then
            If oOption.PlantNumber.Changed Then
                pnlOptions.Controls.Item(sName).ForeColor = Color.Red
            Else
                pnlOptions.Controls.Item(sName).ForeColor = Color.Black
            End If
        End If 'not mbreload

        sName = "ftbFanucOption" & Index
        pnlOptions.Controls.Item(sName).Text = oOption.FanucNumber.Value.ToString
        If Not mbReLoad Then
            If oOption.FanucNumber.Changed Then
                pnlOptions.Controls.Item(sName).ForeColor = Color.Red
            Else
                pnlOptions.Controls.Item(sName).ForeColor = Color.Black
            End If
        End If 'not mbreload

        sName = "ftbOptionDesc" & Index
        pnlOptions.Controls.Item(sName).Text = oOption.Description.Text
        If Not mbReLoad Then
            If oOption.Description.Changed Then
                pnlOptions.Controls.Item(sName).ForeColor = Color.Red
            Else
                pnlOptions.Controls.Item(sName).ForeColor = Color.Black
            End If
        End If 'not mbreload

        Dim clb As CheckedListBox
        Dim nValue As Integer

        sName = "clbOptionRegister" & Index
        pnlOptions.Controls.Item(sName).Text = oOption.OptionRegister.Value.ToString
        clb = DirectCast(pnlOptions.Controls.Item(sName), CheckedListBox)
        nValue = oOption.OptionRegister.Value

        'If Not mbReLoad Then
        '    If oOption.OptionRegister.Changed Then
        '        pnlOptions.Controls.Item(sName).ForeColor = Color.Red
        '        EditsMade = True
        '    Else
        '        pnlOptions.Controls.Item(sName).ForeColor = Color.Black
        '    End If
        'End If 'Not mbReLoad 
        clb.ForeColor = Color.Black
        If mbReLoad Then
            Dim nCurrentValue As Integer

            For i As Integer = 0 To clb.Items.Count - 1
                'lets hope there are no more than 15 robots in a zone....
                If i > 15 Then Exit Sub
                If clb.GetItemCheckState(i) = CheckState.Checked Then
                    nCurrentValue += gnBitVal(i)
                End If
            Next 'i
            If nCurrentValue <> nValue Then
                clb.ForeColor = Color.Red
                EditsMade = True
            End If
        End If 'mbReLoad

        For i As Integer = 0 To clb.Items.Count - 1
            'lets hope there are no more than 15 robots in a zone....
            If i > 15 Then Exit Sub
            clb.SetItemChecked(i, (nValue And gnBitVal(i)) > 0)
        Next 'i

        sName = "clbOptionRobotsReq" & Index
        clb = DirectCast(pnlOptions.Controls.Item(sName), CheckedListBox)
        nValue = oOption.RobotsRequired.Value

        clb.ForeColor = Color.Black
        If mbReLoad Then
            Dim nCurrentValue As Integer

            'For i As Integer = 0 To clb.Items.Count - 1
            '    'lets hope there are no more than 15 robots in a zone....
            '    If i > 15 Then Exit Sub
            '    If clb.GetItemCheckState(i) = CheckState.Checked Then
            '        nCurrentValue += gnBitVal(i + mcolZones.ActiveZone.RobotsRequiredStartingBit)
            '    End If
            'Next 'i
            For nArm As Integer = 0 To mcolArms.Count - 1
                'lets hope there are no more than 15 robots in a zone....
                If clb.GetItemCheckState(nArm) = CheckState.Checked Then
                    nCurrentValue += gnBitVal(mcolArms(nArm).RobotNumber + mcolZones.ActiveZone.RobotsRequiredStartingBit - 1)

                End If
            Next 'i
            If nCurrentValue <> nValue Then
                clb.ForeColor = Color.Red
                EditsMade = True
            End If
        End If 'mbReLoad

        'For i As Integer = 0 To clb.Items.Count - 1
        '    'lets hope there are no more than 15 robots in a zone....
        '    If i > 15 Then Exit Sub
        '    clb.SetItemChecked(i, (nValue And gnBitVal(i + mcolZones.ActiveZone.RobotsRequiredStartingBit)) > 0)
        'Next 'i
        For nArm As Integer = 0 To mcolArms.Count - 1
            'lets hope there are no more than 15 robots in a zone....
            clb.SetItemChecked(nArm, (nValue And gnBitVal(mcolArms(nArm).RobotNumber + mcolZones.ActiveZone.RobotsRequiredStartingBit - 1)) > 0)
        Next

    End Sub

    Private Sub subLoadRepairTabData(ByRef oRepair As clsSysRepairPanel, ByVal Index As String, ByRef oDescription As clsTextValue)
        '********************************************************************************************
        'Description:  Load Repair Tab data
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sName As String = String.Empty
        Dim sPanel As String = CType(Index, Integer).ToString

        sName = "lblRepairPanel" & Index
        pnlRepairs.Controls.Item(sName).Text = oRepair.PanelNumber.ToString

        sName = "ftbRepairDesc" & Index
        pnlRepairs.Controls.Item(sName).Text = oDescription.Text
        If Not mbReLoad Then
            If oDescription.Changed Then
                pnlRepairs.Controls.Item(sName).ForeColor = Color.Red
            Else
                pnlRepairs.Controls.Item(sName).ForeColor = Color.Black
            End If
        End If 'Not mbReLoad

        sName = "clbRepairRobotsReq" & Index
        Dim clb As CheckedListBox = DirectCast(pnlRepairs.Controls.Item(sName), CheckedListBox)

        clb.ForeColor = Color.Black

        Dim nUB As Integer = mcolControllers.Count - 1
        If mcolZones.ActiveZone.UseSplitShell Then
            nUB = mcolArms.Count - 1
        End If

        For nRobot As Integer = 0 To nUB
            If oRepair.RobotRequired(nRobot).Value Then
                clb.SetItemChecked(nRobot, True)
            Else
                clb.SetItemChecked(nRobot, False)
            End If
            If oRepair.RobotRequired(nRobot).Changed Then clb.ForeColor = Color.Red
        Next

    End Sub
    Private Sub subLoadIntrusionTabData(ByRef oStyle As clsSysStyle, ByRef Index As String)
        '********************************************************************************************
        'Description:  Load Intrusion Tab data
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sName As String = "lblPlantStyle" & Index

        pnlIntrusion.Controls.Item(sName).Text = oStyle.PlantNumber.Text

        sName = "lblDesc" & Index
        pnlIntrusion.Controls.Item(sName).Text = oStyle.Description.Text

        sName = "ftbEntStart" & Index
        pnlIntrusion.Controls.Item(sName).Text = oStyle.EntranceMuteStartCount.Value.ToString
        If Not mbReLoad Then
            If oStyle.EntranceMuteStartCount.Changed Then
                pnlIntrusion.Controls.Item(sName).ForeColor = Color.Red
                EditsMade = True
            Else
                pnlIntrusion.Controls.Item(sName).ForeColor = Color.Black
            End If
        End If 'Not mbReLoad

        sName = "ftbExitStart" & Index
        pnlIntrusion.Controls.Item(sName).Text = oStyle.ExitMuteStartCount.Value.ToString
        If Not mbReLoad Then
            If oStyle.ExitMuteStartCount.Changed Then
                pnlIntrusion.Controls.Item(sName).ForeColor = Color.Red
                EditsMade = True
            Else
                pnlIntrusion.Controls.Item(sName).ForeColor = Color.Black
            End If
        End If 'Not mbReLoad

        sName = "ftbMuteLen" & Index
        pnlIntrusion.Controls.Item(sName).Text = oStyle.MuteLength.Value.ToString
        If Not mbReLoad Then
            If oStyle.MuteLength.Changed Then
                pnlIntrusion.Controls.Item(sName).ForeColor = Color.Red
                EditsMade = True
            Else
                pnlIntrusion.Controls.Item(sName).ForeColor = Color.Black
            End If
        End If 'Not mbReLoad

    End Sub
    Private Sub subLoadStyleTabData(ByRef oStyle As clsSysStyle, ByRef Index As String, ByVal Degrade As Integer)
        '********************************************************************************************
        'Description:  Load Style Tab data
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/23/09  BTK     Added code so we can change which bit the robot required data starts at.
        ' 12/13/11  RJO     Added support for Sealer combined Style/Option
        '********************************************************************************************
        Dim sName As String = String.Empty


        sName = "ftbPlantStyle" & Index
        pnlStyles.Controls.Item(sName).Text = oStyle.PlantNumber.Text
        If Not mbReLoad Then
            If oStyle.PlantNumber.Changed Then
                pnlStyles.Controls.Item(sName).ForeColor = Color.Red
                EditsMade = True
            Else
                pnlStyles.Controls.Item(sName).ForeColor = Color.Black
            End If
        End If 'Not mbReLoad

        If mcolStyles.CombinedStyleOption Then 'RJO 12/13/11
            sName = "ftbStyleOpt" & Index
            pnlStyles.Controls.Item(sName).Text = oStyle.OptionNumber.Text
            If Not mbReLoad Then
                If oStyle.OptionNumber.Changed Then
                    pnlStyles.Controls.Item(sName).ForeColor = Color.Red
                    EditsMade = True
                Else
                    pnlStyles.Controls.Item(sName).ForeColor = Color.Black
                End If
            End If 'Not mbReLoad
        End If

        sName = "ftbFanucStyle" & Index
        pnlStyles.Controls.Item(sName).Text = oStyle.FanucNumber.Value.ToString
        If Not mbReLoad Then
            If oStyle.FanucNumber.Changed Then
                pnlStyles.Controls.Item(sName).ForeColor = Color.Red
                EditsMade = True
            Else
                pnlStyles.Controls.Item(sName).ForeColor = Color.Black
            End If
        End If 'Not mbReLoad 

        sName = "ftbStyleDesc" & Index
        pnlStyles.Controls.Item(sName).Text = oStyle.Description.Text
        If Not mbReLoad Then
            If oStyle.Description.Changed Then
                pnlStyles.Controls.Item(sName).ForeColor = Color.Red
                EditsMade = True
            Else
                pnlStyles.Controls.Item(sName).ForeColor = Color.Black
            End If
        End If 'Not mbReLoad

        sName = "ftbStyleRegister" & Index
        pnlStyles.Controls.Item(sName).Text = oStyle.StyleRegister.Value.ToString
        If Not mbReLoad Then
            If oStyle.StyleRegister.Changed Then
                pnlStyles.Controls.Item(sName).ForeColor = Color.Red
                EditsMade = True
            Else
                pnlStyles.Controls.Item(sName).ForeColor = Color.Black
            End If
        End If 'Not mbReLoad 

        sName = "cbtwocoats" & Index
        Dim tempcheckbox As CheckBox
        tempcheckbox = DirectCast(pnlStyles.Controls.Item(sName), CheckBox)
        If mcolStyles.TwoCoat Then
            tempcheckbox.Checked = oStyle.TwoCoatStyle.Value
            If Not mbReLoad Then
                If oStyle.TwoCoatStyle.Changed Then
                    pnlStyles.Controls.Item(sName).ForeColor = Color.Red
                    EditsMade = True
                Else
                    pnlStyles.Controls.Item(sName).ForeColor = Color.Black
                End If
            End If 'Not mbReload
        Else
            tempcheckbox.Visible = False
        End If

        sName = "clbStyleRobotsReq" & Index
        Dim clb As CheckedListBox = DirectCast(pnlStyles.Controls.Item(sName), CheckedListBox)
        Dim nValue As Integer

        If mcolZones.ActiveZone.DegradeStyleRbtsReq Then
            nValue = oStyle.DegradeRobotsRequired(Degrade).Value
        Else
            nValue = oStyle.RobotsRequired.Value
        End If

        clb.ForeColor = Color.Black
        If mbReLoad Then
            Dim nCurrentValue As Integer

            'For i As Integer = 0 To clb.Items.Count - 1
            '   'lets hope there are no more than 15 robots in a zone....
            '   If i > 15 Then Exit Sub
            '   If clb.GetItemCheckState(i) = CheckState.Checked Then
            '        nCurrentValue += gnBitVal(i + mcolZones.ActiveZone.RobotsRequiredStartingBit)
            '   End If
            For nArm As Integer = 0 To mcolArms.Count - 1
                'lets hope there are no more than 15 robots in a zone....
                If clb.GetItemCheckState(nArm) = CheckState.Checked Then
                    nCurrentValue += gnBitVal(mcolArms(nArm).RobotNumber + mcolZones.ActiveZone.RobotsRequiredStartingBit - 1)

                End If
            Next 'i
            If nCurrentValue <> nValue Then
                clb.ForeColor = Color.Red
                EditsMade = True
            End If
        End If 'mbReLoad

        'For i As Integer = 0 To clb.Items.Count - 1
        ''    'lets hope there are no more than 15 robots in a zone....
        '    If i > 15 Then Exit Sub
        '    clb.SetItemChecked(i, (nValue And gnBitVal(i + mcolZones.ActiveZone.RobotsRequiredStartingBit)) > 0)
        'Next 'i
        For nArm As Integer = 0 To mcolArms.Count - 1
            'lets hope there are no more than 15 robots in a zone....
            clb.SetItemChecked(nArm, (nValue And gnBitVal(mcolArms(nArm).RobotNumber + mcolZones.ActiveZone.RobotsRequiredStartingBit - 1)) > 0)
        Next

    End Sub
    Private Sub subLocateStyleTabControls()
        '********************************************************************************************
        'Description:  Jockey around the first row of controls on pnlStyles and their captions to  
        '              accomodate Combined Style/Option and TwoCoat options.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/13/11  RJO     WYSIWYG wasn't gettin' it.
        '********************************************************************************************
        Dim nSpacer As Integer = 14
        Dim nLeft As Integer = ftbPlantStyle001.Left + ftbPlantStyle001.Width + nSpacer
        Dim nOffset As Integer = pnlStyles.Left
        Dim bCombinedStyleOption As Boolean = mcolStyles.CombinedStyleOption
        Dim bTwoCoat As Boolean = mcolStyles.TwoCoat
        Dim bStyleRegisters As Boolean = mcolStyles.StyleRegisters

        'Plant Style
        lblPlantStyleCap.Left = ((ftbPlantStyle001.Width - lblPlantStyleCap.Width) \ 2) + ftbPlantStyle001.Left + nOffset

        lblStyleOptCap.Visible = bCombinedStyleOption
        ftbStyleOpt001.Visible = bCombinedStyleOption

        'Plant Option
        If bCombinedStyleOption Then
            ftbStyleOpt001.Left = nLeft
            nLeft += (ftbStyleOpt001.Width + nSpacer)
            lblStyleOptCap.Left = ((ftbStyleOpt001.Width - lblStyleOptCap.Width) \ 2) + ftbStyleOpt001.Left + nOffset
        End If

        'FANUC Style
        ftbFanucStyle001.Left = nLeft
        nLeft += (ftbFanucStyle001.Width + nSpacer)
        lblFanucStyleCap.Left = ((ftbFanucStyle001.Width - lblFanucStyleCap.Width) \ 2) + ftbFanucStyle001.Left + nOffset

        'Description
        ftbStyleDesc001.Left = nLeft
        nLeft += (ftbStyleDesc001.Width + nSpacer)
        lblStyleDescCap.Left = ((ftbStyleDesc001.Width - lblStyleDescCap.Width) \ 2) + ftbStyleDesc001.Left + nOffset

        'TwoCoat
        lbltwocoats.Visible = bTwoCoat
        cbTwoCoats001.Visible = bTwoCoat

        If bTwoCoat Then
            cbTwoCoats001.Left = nLeft
            nLeft += (cbTwoCoats001.Width + nSpacer)
            lbltwocoats.Left = ((cbTwoCoats001.Width - lbltwocoats.Width) \ 2) + cbTwoCoats001.Left + nOffset
        End If

        'Style Registers
        If bStyleRegisters Then
            ftbStyleRegister001.Left = nLeft
            nLeft += (ftbStyleRegister001.Width + nSpacer)
            lblStyleRegister.Left = ((ftbStyleRegister001.Width - lblStyleRegister.Width) \ 2) + ftbStyleRegister001.Left + nOffset
        End If

        'Robots Required
        clbStyleRobotsReq001.Left = nLeft
        lblStyleRobotsReqCap.Left = ((clbStyleRobotsReq001.Width - lblStyleRobotsReqCap.Width) \ 2) + nLeft + nOffset

    End Sub
    Private Sub subLoadStyleIDNonTrkTabData(ByVal Index As Integer)
        '********************************************************************************************
        'Description:  Load Style Tab data
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sName As String = String.Empty
        Dim oStyleID As clsSysStyleID = Nothing
        Dim nItem As Integer = 0
        Dim nPhotoeyeIndex As Integer

        oStyleID = mcolStyleID.Item(0)

        'Photoeye pattern
        Dim nPhotoeyePatteren As Integer
        Dim nPhotoeyeIgnore As Integer

        nPhotoeyePatteren = oStyleID.PhotoeyePattern.Value
        nPhotoeyeIgnore = oStyleID.PhotoeyeIgnore.Value

        For nIndex As Integer = 1 To mcolStyleID.NumberOfPhotoeyes
            nPhotoeyeIndex = nIndex
            sName = "btnPhotoeye" & Strings.Format(nPhotoeyeIndex, "000")
            pnlStyleID.Controls.Item(sName).Text = nIndex.ToString
            'If (nPhotoeyeIgnore And gnBitVal(nIndex - 1)) > 0 Then
            '    'Make the textbox Gray
            '    pnlStyleID.Controls.Item(sName).BackColor = Color.Gray
            'Else
            If (nPhotoeyePatteren And gnBitVal(nIndex - 1)) > 0 Then
                'Make the textbox yellow
                pnlStyleID.Controls.Item(sName).BackColor = Color.Yellow
            Else
                'Make the textbox green
                pnlStyleID.Controls.Item(sName).BackColor = Color.GreenYellow
            End If
            'End If
            If Not mbReLoad Then
                If oStyleID.PhotoeyePattern.Changed Or oStyleID.PhotoeyeIgnore.Changed Then
                    DirectCast(pnlStyleID.Controls.Item(sName), Button).FlatAppearance.BorderColor = Color.Red
                    EditsMade = True
                Else
                    DirectCast(pnlStyleID.Controls.Item(sName), Button).FlatAppearance.BorderColor = Color.Black
                End If
            End If 'Not mbReLoad
        Next 'nIndex
        DataLoaded = True

    End Sub
    Private Sub subLoadStyleIDTrackingTabData(ByVal Index As Integer)
        '********************************************************************************************
        'Description:  Load Style Tab data
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sName As String = String.Empty
        Dim oStyleID As clsSysStyleID = Nothing
        Dim nItem As Integer = 0
        Dim nPhotoeyeIndex As Integer

        For nItem = 1 To (mcolStyleID.NumberOfConveyorSnapshots)
            oStyleID = mcolStyleID.Item(nItem + (Index * mcolStyleID.NumberOfConveyorSnapshots) - 1)

            '**********************
            sName = "ftbIDPosition" & Strings.Format(nItem, "000")
            pnlStyleID.Controls.Item(sName).Text = oStyleID.ConveyorCount.Value.ToString
            pnlStyleID.Controls.Item(sName).ForeColor = Color.Black
            If Not mbReLoad Then
                If oStyleID.ConveyorCount.Changed Then
                    pnlStyleID.Controls.Item(sName).ForeColor = Color.Red
                    EditsMade = True
                Else
                    pnlStyleID.Controls.Item(sName).ForeColor = Color.Black
                End If
            End If 'Not mbReLoad


            'Photoeye pattern
            Dim nPhotoeyePatteren As Integer
            Dim nPhotoeyeIgnore As Integer

            nPhotoeyePatteren = oStyleID.PhotoeyePattern.Value
            nPhotoeyeIgnore = oStyleID.PhotoeyeIgnore.Value

            For nIndex As Integer = 1 To mcolStyleID.NumberOfPhotoeyes
                nPhotoeyeIndex = nIndex + ((nItem - 1) * mcolStyleID.NumberOfPhotoeyes)
                sName = "btnPhotoeye" & Strings.Format(nPhotoeyeIndex, "000")
                pnlStyleID.Controls.Item(sName).Text = oStyleID.PhotoeyePattern.Value.ToString
                If (nPhotoeyeIgnore And gnBitVal(nIndex - 1)) > 0 Then
                    'Make the textbox Gray
                    pnlStyleID.Controls.Item(sName).BackColor = Color.Gray
                Else
                    If (nPhotoeyePatteren And gnBitVal(nIndex - 1)) > 0 Then
                        'Make the textbox yellow
                        pnlStyleID.Controls.Item(sName).BackColor = Color.Yellow
                    Else
                        'Make the textbox green
                        pnlStyleID.Controls.Item(sName).BackColor = Color.GreenYellow
                    End If
                End If
                If Not mbReLoad Then
                    If oStyleID.PhotoeyePattern.Changed Or oStyleID.PhotoeyeIgnore.Changed Then
                        DirectCast(pnlStyleID.Controls.Item(sName), Button).FlatAppearance.BorderColor = Color.Red
                        EditsMade = True
                    Else
                        DirectCast(pnlStyleID.Controls.Item(sName), Button).FlatAppearance.BorderColor = Color.Black
                    End If
                End If 'Not mbReLoad
            Next 'nIndex
        Next 'nItem
        DataLoaded = True

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

        'mPWCommon.SaveToChangeLog(mcolZones.ActiveZone.DatabasePath)
        'For SQL database - remove above eventually
        mPWCommon.SaveToChangeLog(mcolZones.ActiveZone)

    End Sub
    Private Function bPrintdoc(ByVal bPrint As Boolean, Optional ByVal bExportCSV As Boolean = False) As Boolean
        '********************************************************************************************
        'Description:  Data Print Routine
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/04/10  MSW     Move return(true) so status and enable get updated
        ' 04/14/11  MSW     CSV export
        ' 07/08/13  BTK     Tricoat By Style     
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

            Dim sPageTitle As String = gpsRM.GetString("psSCREENCAPTION")
            Dim sTitle(1) As String
            sTitle(0) = oTab.Text
            sTitle(1) = String.Empty

            Dim sSubTitle(0) As String
            sSubTitle(0) = mcolZones.SiteName & " - " & mcolZones.ActiveZone.Name

            If mcolZones.ActiveZone.DegradeStyleRbtsReq Then
                sSubTitle(0) = sSubTitle(0) & " " & lblRobot.Text & " - " & cboRobot.Text
            End If

            Progress = 10
            Dim bCancel As Boolean = False
            mPrintHtml.subStartDoc(Status, sPageTitle & " - " & oTab.Text, bExportCSV, bCancel)
            If bCancel = False Then
                Select Case oTab.Name
                    Case "TabPage1"
                        subPrintStyles(mPrintHtml, sTitle, sSubTitle)
                    Case "TabPage2"
                        subPrintOptions(mPrintHtml, sTitle, sSubTitle)
                    Case "TabPage3"
                        subPrintRepairs(mPrintHtml, sTitle, sSubTitle)
                    Case "TabPage4"
                        subPrintDegrade(mPrintHtml, sTitle, sSubTitle)
                    Case "TabPage5"
                        subPrintIntrusion(mPrintHtml, sTitle, sSubTitle)
                    Case "TabPage6"
                        subPrintStyleID(mPrintHtml, sTitle, sSubTitle)
                    Case "TabPage7"  '07/08/13  BTK     Tricoat By Style
                        subPrintStyleTricoatEnb(mPrintHtml, sTitle, sSubTitle)
                End Select

                Progress = 80
                '
                Status = gcsRM.GetString("csPRINT_SENDING", DisplayCulture)
                mPrintHtml.subCloseFile(Status)
                If bPrint Then
                    mPrintHtml.subPrintDoc()
                End If
            End If
            Progress = 0
            Status = gcsRM.GetString("csREADY", DisplayCulture)
            Me.Cursor = System.Windows.Forms.Cursors.Default
            btnPrint.Enabled = True
            Return (Not (bCancel))
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
    Private Sub subPrintOptions(ByRef mPrintHtml As clsPrintHtml, ByVal sTitle() As String, ByVal sSubTitle As String())
        '********************************************************************************************
        'Description:  Data Print Routine
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 6/7/07    GEO     .net
        ' 10/23/09  BTK     Added code so we can change which bit the robot required data starts at.
        '********************************************************************************************

        Dim nRobot As Integer = 0
        Dim nMask As Integer = 0


        Try



            Progress = 10
            Dim sText(1) As String
            sText(0) = gpsRM.GetString("psITEM_CAP", DisplayCulture) & vbTab & _
                            lblPlantOptionCap.Text & vbTab & lblFanucOptionCap.Text & vbTab & _
                            lblStyleDescCap.Text

            If mcolOptions.OptionRegisters Then
                For i As Integer = 0 To clbOptionRegister001.Items.Count - 1
                    sText(0) = sText(0) & vbTab & _
                            clbOptionRegister001.GetItemText(clbOptionRegister001.Items(i))
                Next
            End If
            For i As Integer = 0 To clbOptionRobotsReq001.Items.Count - 1
                sText(0) = sText(0) & vbTab & _
                        clbOptionRobotsReq001.GetItemText(clbOptionRobotsReq001.Items(i))
            Next

            sText(0) = Strings.Trim(sText(0))

            ReDim Preserve sText(mcolOptions.Count)
            For Each oThisOption As clsSysOption In mcolOptions

                With oThisOption

                    sText(.ItemNumber + 1) = (.ItemNumber + 1).ToString & vbTab & _
                                    .PlantString & vbTab & .FanucNumber.Value.ToString & _
                                    vbTab & .Description.Text

                    If mcolOptions.OptionRegisters Then
                        For nRobot = 0 To mcolOptions.RegisterMaxValue
                            nMask = CType(2 ^ (nRobot), Integer)
                            If (oThisOption.OptionRegister.Value And nMask) > 0 Then
                                sText(.ItemNumber + 1) = sText(.ItemNumber + 1) & vbTab & " X "
                            Else
                                sText(.ItemNumber + 1) = sText(.ItemNumber + 1) & vbTab & " - "
                            End If
                        Next 'nRobot
                    End If
                    For nRobot = 0 To mnNumberofRobots - 1
                        nMask = CType(2 ^ (nRobot + mcolZones.ActiveZone.RobotsRequiredStartingBit), Integer)
                        If (oThisOption.RobotsRequired.Value And nMask) > 0 Then
                            sText(.ItemNumber + 1) = sText(.ItemNumber + 1) & vbTab & " X "
                        Else
                            sText(.ItemNumber + 1) = sText(.ItemNumber + 1) & vbTab & " - "
                        End If
                    Next 'nRobot
                    sText(.ItemNumber + 1) = sText(.ItemNumber + 1).Replace("<", "")
                    sText(.ItemNumber + 1) = sText(.ItemNumber + 1).Replace(">", "")
                    sText(.ItemNumber + 1) = Strings.Trim(sText(.ItemNumber + 1))
                End With
                '
            Next 'oThisOption

            Progress = 30
            '
            mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle, sTitle(0))
            '
            Progress = 55
            '


        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                               Status, MessageBoxButtons.OK)

        Finally

        End Try


    End Sub
    Private Sub subPrintDegrade(ByRef mPrintHtml As clsPrintHtml, ByVal sTitle() As String, ByVal sSubTitle As String())
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



        Try

            For Each oThisStyle As clsSysStyle In mcolStyles
                With oThisStyle

                    ReDim Preserve sSubTitle(2)
                    sSubTitle(1) = .DisplayName

                    'column header part
                    Dim sText(2) As String
                    sText(0) = lblControllerCap.Text
                    For i As Integer = 0 To mcolArms.Count - 1
                        sText(0) = sText(0) & vbTab & mcolArms(i).Name
                    Next

                    sText(0) = Strings.Trim(sText(0))

                    'degrade data part

                    For i As Integer = 1 To cboController.Items.Count
                        sText(1) = cboController.GetItemText(cboController.Items(i - 1))
                        For nArm As Integer = 1 To mcolArms.Count
                            sText(1) = sText(1) & vbTab & _
                                .PlantSpecific.DegradeNumber(i, nArm).Value.ToString()
                        Next
                        sText(1) = Trim(sText(1))
                    Next


                    mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle, sTitle(0))
                End With
            Next oThisStyle
            '
            Progress = 55
            '
            'Status = gcsRM.GetString("csPRINT_FORMATTING", DisplayCulture)


        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                               Status, MessageBoxButtons.OK)

        Finally

        End Try




    End Sub
    Private Sub subPrintIntrusion(ByRef mPrintHtml As clsPrintHtml, ByVal sTitle() As String, ByVal sSubTitle As String())
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

        Try

            Dim sText(1) As String
            sText(0) = Trim(gpsRM.GetString("psITEM_CAP", DisplayCulture) & vbTab & _
                        lblPlantStyleCap5.Text & vbTab & lblDescCap5.Text & vbTab & _
                        lblEntStartCap.Text & vbTab & lblExitStartCap.Text & vbTab & lblMuteLenCap.Text)

            sText(0) = Strings.Trim(sText(0))

            ReDim Preserve sText(mcolStyles.Count)
            For Each oThisStyle As clsSysStyle In mcolStyles
                With oThisStyle
                    sText(.ItemNumber + 1) = (.ItemNumber + 1).ToString & vbTab & _
                                    .PlantNumber.Text & vbTab & _
                                    .Description.Text & vbTab & _
                                    .EntranceMuteStartCount.Value.ToString & vbTab & _
                                    .ExitMuteStartCount.Value.ToString & vbTab & _
                                    .MuteLength.Value.ToString
                    sText(.ItemNumber + 1) = sText(.ItemNumber + 1).Replace("<", "")
                    sText(.ItemNumber + 1) = sText(.ItemNumber + 1).Replace(">", "")
                    sText(.ItemNumber + 1) = Trim(sText(.ItemNumber + 1))
                End With

            Next oThisStyle

            Progress = 30
            '
            mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle, sTitle(0))

            '
            Progress = 55
            '
            Status = gcsRM.GetString("csPRINT_FORMATTING", DisplayCulture)
            '

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                               Status, MessageBoxButtons.OK)

        Finally

        End Try



    End Sub
    Private Sub subPrintRepairs(ByRef mPrintHtml As clsPrintHtml, ByVal sTitle() As String, ByVal sSubTitle As String())
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

        Dim sBodyString As String = String.Empty
        Dim nRobots As Integer = clbRepairRobotsReq001.Items.Count - 1

        Try
            If mcolZones.ActiveZone.RepairPanels > 0 Then
                Dim bDegradeRepair As Boolean = mcolZones.ActiveZone.DegradeRepair
                If cboStyle.SelectedIndex >= 0 And ((cboRobot.SelectedIndex >= 0) Or (mcolZones.ActiveZone.DegradeRepair = False)) Then
                    Dim oRepairs As clsSysRepairPanels = Nothing
                    If mcolZones.ActiveZone.DegradeRepair Then
                        oRepairs = mcolStyles.Item(cboStyle.SelectedIndex).PlantSpecific.DegradeRepairPanels.Item(cboRobot.SelectedIndex)
                    Else
                        oRepairs = mcolStyles.Item(cboStyle.SelectedIndex).PlantSpecific.DegradeRepairPanels.Item(0)
                    End If

                    Dim oDescriptions() As clsTextValue = mcolStyles.Item(cboStyle.SelectedIndex).RepairPanelDescriptions

                    ReDim Preserve sSubTitle(4)
                    sSubTitle(1) = ""
                    sSubTitle(2) = cboStyle.Text

                    sSubTitle(3) = lblRobot.Text & ":"
                    sSubTitle(4) = cboRobot.Text

                    'column header part
                    Dim sText(1) As String
                    sText(0) = lblPanelCap.Text & vbTab & lblRepairDescCap.Text

                    For i As Integer = 0 To nRobots
                        sText(0) = sText(0) & vbTab & _
                            clbRepairRobotsReq001.GetItemText(clbRepairRobotsReq001.Items(i))
                    Next

                    sText(0) = Strings.Trim(sText(0))
                    ReDim Preserve sText(oRepairs.Count)
                    'degrade data part
                    For i As Integer = 1 To oRepairs.Count
                        Dim o As clsSysRepairPanel = oRepairs.Item(i - 1)

                        sText(i) = o.PanelNumber.ToString & vbTab & oDescriptions(i - 1).Text
                        For nArm As Integer = 0 To nRobots
                            If o.RobotRequired(nArm).Value = True Then
                                sText(i) = sText(i) & vbTab & " X "
                            Else
                                sText(i) = sText(i) & vbTab & " - "
                            End If
                        Next
                        sText(i) = sText(i).Replace("<", "")
                        sText(i) = sText(i).Replace(">", "")
                        sText(i) = Trim(sText(i))
                    Next
                    mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle, sTitle(0))

                End If 'cboStyle.SelectedIndex > 0 
            End If
            'Status = gcsRM.GetString("csPRINT_FORMATTING", DisplayCulture)


        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                               Status, MessageBoxButtons.OK)

        Finally

        End Try




    End Sub
    Private Sub subPrintStyles(ByRef mPrintHtml As clsPrintHtml, ByVal sTitle() As String, ByVal sSubTitle As String())
        '********************************************************************************************
        'Description:  Data Print Routine
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 6/7/07    GEO     .net
        ' 10/23/09  BTK     Added code so we can change which bit the robot required data starts at.
        ' 12/13/11  RJO     Added support for Sealer combined Style/Option
        '********************************************************************************************

        Dim nRobot As Integer = 0
        Dim nMask As Integer = 0


        Try

            Dim sText(1) As String

            If mcolStyles.CombinedStyleOption Then 'RJO 12/13/11
                sText(0) = gpsRM.GetString("psITEM_CAP", DisplayCulture) & vbTab & _
                           lblPlantStyleCap.Text & vbTab & lblStyleOptCap.Text & vbTab & _
                           lblFanucStyleCap.Text & vbTab & lblStyleDescCap.Text
            Else
                sText(0) = gpsRM.GetString("psITEM_CAP", DisplayCulture) & vbTab & _
                           lblPlantStyleCap.Text & vbTab & lblFanucStyleCap.Text & vbTab & _
                           lblStyleDescCap.Text
            End If


            For i As Integer = 0 To clbStyleRobotsReq001.Items.Count - 1
                sText(0) = sText(0) & vbTab & _
                        clbStyleRobotsReq001.GetItemText(clbStyleRobotsReq001.Items(i))
            Next

            ReDim Preserve sText(mcolStyles.Count + 1) 'NVSP One row for header
            For Each oThisStyle As clsSysStyle In mcolStyles
                With oThisStyle
                    If mcolStyles.CombinedStyleOption Then 'RJO 12/13/11
                        sText(.ItemNumber + 1) = (.ItemNumber + 1).ToString & vbTab & _
                                                 .PlantString & vbTab & .OptionString & vbTab & _
                                                 .FanucNumber.Value.ToString & vbTab & _
                                                 .Description.Text & vbTab
                    Else
                        sText(.ItemNumber + 1) = (.ItemNumber + 1).ToString & vbTab & _
                                                 .PlantString & vbTab & .FanucNumber.Value.ToString & vbTab & _
                                                 .Description.Text
                    End If
                    
                    For nRobot = 0 To mnNumberofRobots - 1
                        nMask = CType(2 ^ (nRobot + mcolZones.ActiveZone.RobotsRequiredStartingBit), Integer)
                        If mcolZones.ActiveZone.DegradeStyleRbtsReq Then
                            If (.DegradeRobotsRequired(cboRobot.SelectedIndex).Value And nMask) > 0 Then
                                sText(.ItemNumber + 1) = sText(.ItemNumber + 1) & vbTab & " X "
                            Else
                                sText(.ItemNumber + 1) = sText(.ItemNumber + 1) & vbTab & " - "
                            End If
                        Else
                            If (.RobotsRequired.Value And nMask) > 0 Then
                                sText(.ItemNumber + 1) = sText(.ItemNumber + 1) & vbTab & " X "
                            Else
                                sText(.ItemNumber + 1) = sText(.ItemNumber + 1) & vbTab & " - "
                            End If
                        End If
                    Next 'nRobot
                    sText(.ItemNumber + 1) = sText(.ItemNumber + 1).Replace("<", "")
                    sText(.ItemNumber + 1) = sText(.ItemNumber + 1).Replace(">", "")
                    sText(.ItemNumber + 1) = Trim(sText(.ItemNumber + 1))
                End With

            Next oThisStyle

            Progress = 30
            '
            mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle, sTitle(0))

            '
            Progress = 55
            '
            Status = gcsRM.GetString("csPRINT_FORMATTING", DisplayCulture)
            '



        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                               Status, MessageBoxButtons.OK)

        Finally


        End Try


    End Sub

    Private Sub subPrintStyleID(ByRef mPrintHtml As clsPrintHtml, ByVal sTitle() As String, ByVal sSubTitle As String())
        '********************************************************************************************
        'Description:  Data Print Routine
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 6/7/07    GEO     .net
        ' 10/23/09  BTK     Added code so we can change which bit the robot required data starts at.
        '********************************************************************************************

        Dim nItem As Integer
        Dim oStyleID As clsSysStyleID = Nothing
        Dim nMask As Integer = 0


        Try

            If mnStyleIDIndex = -1 Then Exit Sub

            Dim sText(1) As String
            For Each oThisStyle As clsSysStyle In mcolStyles
                With oThisStyle
                    If .ItemNumber = mnStyleIDIndex Then
                        ReDim Preserve sSubTitle(2)
                        sSubTitle(1) = lblPlantStyleCap06.Text & " - " & .DisplayName
                        Exit For
                    End If
                End With
            Next oThisStyle

            sText(0) = gpsRM.GetString("psITEM_CAP", DisplayCulture) & vbTab & _
                            lblConveyorPosCap.Text

            For i As Integer = 0 To mcolStyleID.NumberOfPhotoeyes - 1
                sText(0) = sText(0) & vbTab & _
                        "PE #" & i + 1
            Next

            sText(0) = Strings.Trim(sText(0))
            ReDim Preserve sText(mcolStyleID.NumberOfConveyorSnapshots)
            For nItem = 1 To (mcolStyleID.NumberOfConveyorSnapshots)
                oStyleID = mcolStyleID.Item(((mnStyleIDIndex * mcolStyleID.NumberOfConveyorSnapshots) + (nItem - 1)))

                sText(oStyleID.ItemNumber + 1) = (oStyleID.ItemNumber + 1).ToString & vbTab & _
                    oStyleID.ConveyorCount.Value.ToString

                'Photoeye pattern
                Dim nPhotoeyePatteren As Integer
                Dim nPhotoeyeIgnore As Integer

                nPhotoeyePatteren = oStyleID.PhotoeyePattern.Value
                nPhotoeyeIgnore = oStyleID.PhotoeyeIgnore.Value

                For nIndex As Integer = 1 To mcolStyleID.NumberOfPhotoeyes
                    If (nPhotoeyeIgnore And gnBitVal(nIndex - 1)) > 0 Then
                        'Make the textbox Gray
                        sText(oStyleID.ItemNumber + 1) = sText(oStyleID.ItemNumber + 1) & vbTab & "?"
                    Else
                        If (nPhotoeyePatteren And gnBitVal(nIndex - 1)) > 0 Then
                            'Make the textbox yellow
                            sText(oStyleID.ItemNumber + 1) = sText(oStyleID.ItemNumber + 1) & vbTab & "X"
                        Else
                            'Make the textbox green
                            sText(oStyleID.ItemNumber + 1) = sText(oStyleID.ItemNumber + 1) & vbTab & "-"
                        End If
                    End If
                Next 'nIndex
                sText(oStyleID.ItemNumber + 1) = sText(oStyleID.ItemNumber + 1).Replace("<", "")
                sText(oStyleID.ItemNumber + 1) = sText(oStyleID.ItemNumber + 1).Replace(">", "")
                sText(oStyleID.ItemNumber + 1) = Strings.Trim(sText(oStyleID.ItemNumber + 1))
            Next 'nItem

            Progress = 30
            '
            mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle, sTitle(0))
            '
            Progress = 55
            '
            Status = gcsRM.GetString("csPRINT_FORMATTING", DisplayCulture)
            '

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED", DisplayCulture), ex, msSCREEN_NAME, _
                               Status, MessageBoxButtons.OK)

        Finally


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

            'make sure its good data
            If Not bValidateData() Then Exit Sub

            ' do save
            If bSaveToGUI() Then
                Dim bSavedToPLC As Boolean = False

                'SA
                If Not mcolZones.StandAlone Then
                    ' If eLoadModes.LoadFromPLC <> eLoadModes.LoadFromPLC Then
                    bSavedToPLC = bSaveToPLC()
                    ' End If

                    If bSavedToPLC = False Then
                        MsgBox(gcsRM.GetString("csSAVEFAILEDPLC"), MsgBoxStyle.Information)
                    End If    ' bSaveToPLC()
                End If

                subLogChanges()
                ' save done
                EditsMade = False

                Select Case msCurrentTabName
                    Case "TabPage1"
                        mcolStyles.Update()
                    Case "TabPage2"
                        mcolOptions.Update()
                    Case "TabPage3"

                    Case "TabPage4"

                    Case "TabPage5"

                    Case "TabPage6"
                        mcolStyleID.Update()

                End Select


                subSetAllForeColorBlack()
                subShowNewPage(True)
                Status = gcsRM.GetString("csSAVE_DONE", DisplayCulture)

            Else
                'save to DB failed
                MsgBox(gcsRM.GetString("csSAVEFAILED"), MsgBoxStyle.Information)
            End If      'bSaveToGUI()

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally

            Me.Cursor = System.Windows.Forms.Cursors.Default

        End Try

    End Sub
    Private Sub subSetAllForeColorBlack()
        '********************************************************************************************
        'Description: set the forecolor of all tab panel controls to Black. 
        '
        'Parameters: none 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim ctrl As Control

        For Each ctrl In pnlStyles.Controls 'pnlStyles.Controls
            ' ALL TEXT BOXES IN PANEL
            If TypeOf ctrl Is FocusedTextBox.FocusedTextBox Then
                CType(ctrl, FocusedTextBox.FocusedTextBox).ForeColor = Color.Black
                'Exit For
            End If
            '            ALL CHECK LIST BOXES ITEMS
            If TypeOf ctrl Is CheckedListBox Then
                CType(ctrl, CheckedListBox).ForeColor = Color.Black
                ' Exit For
            End If
        Next 'ctrl

        For Each ctrl In pnlOptions.Controls
            ' ALL TEXT BOXES IN PANEL
            If TypeOf ctrl Is FocusedTextBox.FocusedTextBox Then
                CType(ctrl, FocusedTextBox.FocusedTextBox).ForeColor = Color.Black
                'Exit For
            End If
            '            ALL CHECK LIST BOXES ITEMS
            If TypeOf ctrl Is CheckedListBox Then
                CType(ctrl, CheckedListBox).ForeColor = Color.Black
                ' Exit For
            End If
        Next 'ctrl

        For Each ctrl In pnlRepairs.Controls
            ' ALL TEXT BOXES IN PANEL
            If TypeOf ctrl Is FocusedTextBox.FocusedTextBox Then
                CType(ctrl, FocusedTextBox.FocusedTextBox).ForeColor = Color.Black
                'Exit For
            End If
            '            ALL CHECK LIST BOXES ITEMS
            If TypeOf ctrl Is CheckedListBox Then
                CType(ctrl, CheckedListBox).ForeColor = Color.Black
                ' Exit For
            End If
        Next 'ctrl
        For Each ctrl In pnlDegrade.Controls
            ' ALL TEXT BOXES IN PANEL
            If TypeOf ctrl Is FocusedTextBox.FocusedTextBox Then
                CType(ctrl, FocusedTextBox.FocusedTextBox).ForeColor = Color.Black
                'Exit For
            End If
        Next 'ctrl

        For Each ctrl In pnlIntrusion.Controls
            ' ALL TEXT BOXES IN PANEL
            If TypeOf ctrl Is FocusedTextBox.FocusedTextBox Then
                CType(ctrl, FocusedTextBox.FocusedTextBox).ForeColor = Color.Black
                'Exit For
            End If
        Next 'ctrl

        For Each ctrl In pnlStyleID.Controls
            ' ALL TEXT BOXES IN PANEL
            If TypeOf ctrl Is FocusedTextBox.FocusedTextBox Then
                CType(ctrl, FocusedTextBox.FocusedTextBox).ForeColor = Color.Black
                'Exit For
            End If
        Next 'ctrl

    End Sub
    Private Sub subSetUpRobotCheckBox(ByRef oCB As CheckedListBox)
        '********************************************************************************************
        'Description:  set up the checkbox with robot names
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 07/28/10  MSW     Prevent sloppy robot checkboxes
        '********************************************************************************************
        oCB.Items.Clear()
        Dim nWidth As Integer
        Dim nLargeWidth As Integer
        For i As Integer = 0 To (msRobotClbNames.GetUpperBound(0))
            Dim sArmName As String = msRobotClbNames(i)
            Dim g As Graphics = oCB.CreateGraphics

            oCB.Items.Add(sArmName)
            ' Determine the width of the items in the list to get the best column width setting.
            nWidth = CInt(g.MeasureString(sArmName, oCB.Font).Width)
            If nWidth > nLargeWidth Then nLargeWidth = nWidth
        Next 'i
        oCB.ColumnWidth = nLargeWidth + 20
        oCB.Width = ((nLargeWidth + 20) * oCB.Items.Count) + 5 ' 07/28/10  MSW     Prevent sloppy robot checkboxes

    End Sub
    Private Sub subSetUpOptionRepairCheckBox(ByRef oCB As CheckedListBox)
        '********************************************************************************************
        'Description:  set up the checkbox with robot names
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 07/28/10  MSW     Prevent sloppy robot checkboxes
        '********************************************************************************************
        oCB.Items.Clear()
        Dim nWidth As Integer
        Dim nLargeWidth As Integer
        For i As Integer = 0 To (mcolOptions.RegisterMaxValue)
            Dim sName As String = "Register " & 18 + i
            Dim g As Graphics = oCB.CreateGraphics

            oCB.Items.Add(sName)
            ' Determine the width of the items in the list to get the best column width setting.
            nWidth = CInt(g.MeasureString(sName, oCB.Font).Width)
            If nWidth > nLargeWidth Then nLargeWidth = nWidth
        Next 'i
        oCB.ColumnWidth = nLargeWidth + 20
        oCB.Width = ((nLargeWidth + 20) * oCB.Items.Count) + 5 ' 07/28/10  MSW     Prevent sloppy robot checkboxes

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
        ' 04/23/10  MSW     Change ByArm to True
        ' 04/16/13  MSW     Standalone ChangeLog                                            4.1.5.0
        ' 02/03/14  RJO     Changed sScreenName argument to mPWCommon.subShowChangeLog from 
        '                   msSCREEN_NAME to gsChangeLogArea for consistency between this screen
        '                   and Change Log Reports screen.
        '********************************************************************************************

        Try
            mPWCommon.subShowChangeLog(mcolZones.CurrentZone, gsChangeLogArea, gsChangeLogArea, gcsRM.GetString("csALL"), _
                                          gcsRM.GetString("csALL"), mDeclares.eParamType.None, nIndex, False, False, oIPC)  'JZ 12172016 - Don't need Param dropbox.

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
        End Try

    End Sub
    Private Sub subShowNewOptionsTab()
        '********************************************************************************************
        'Description:  Data is all loaded or changed and screen needs to be refreshed
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/30/10  MSW     Change to look at ascii enable in option/style/color config
        '********************************************************************************************
        Dim oOption As clsSysOption
        Dim sName As String = String.Empty
        Dim nHorizontalOffset As Integer = pnlOptions.HorizontalScroll.Value
        Dim nRow1Top As Integer = ftbPlantOption001.Top + pnlOptions.VerticalScroll.Value
        Dim nPlantOptionLeft As Integer = ftbPlantOption001.Left + nHorizontalOffset
        Dim nFanucOptionLeft As Integer = ftbFanucOption001.Left + nHorizontalOffset
        Dim nDescLeft As Integer = ftbOptionDesc001.Left + nHorizontalOffset
        Dim nRobotsLeft As Integer = clbOptionRobotsReq001.Left + nHorizontalOffset
        Dim mnuSelect As ContextMenuStrip
        Dim imgVoid As Image = Nothing
        Dim nORLeft As Integer = clbOptionRegister001.Left + nHorizontalOffset
        Dim nOptionRepairOffset As Integer = 20

        mnDataRows = mcolOptions.MaxOptions

        Try
            Call subUnloadPanel(pnlOptions)

            'Create controls 
            With pnlOptions
                .SuspendLayout()
                .AutoScroll = True
                .AutoScrollMargin = New Size(10, mnROWSPACE)
                .VerticalScroll.SmallChange = mnROWSPACE
            End With

            ftbPlantOption001.NumericOnly = (mcolOptions.UseAscii = False)
            ftbFanucOption001.NumericOnly = True

            If lblOptionRegister.Visible <> mcolOptions.OptionRegisters Then
                Dim bTmp As Boolean = mcolOptions.OptionRegisters
                lblOptionRegister.Visible = bTmp
                clbOptionRegister001.Visible = bTmp
                subSetUpOptionRepairCheckBox(clbOptionRegister001)
                AddHandler clbOptionRegister001.ItemCheck, AddressOf clbOptionRegister_ItemCheck
                AddHandler clbOptionRegister001.LostFocus, AddressOf clbAll_LostFocus
                AddHandler clbOptionRegister001.GotFocus, AddressOf clbAll_GotFocus

                mnuSelect = New ContextMenuStrip
                With mnuSelect
                    .Name = "clbOptionRegister001"
                    .Items.Add(gcsRM.GetString("csSELECT_ALL", DisplayCulture), imgVoid, AddressOf clbOption_MenuSelect)
                    .Items.Add(gcsRM.GetString("csSELECT_NONE", DisplayCulture), imgVoid, AddressOf clbOption_MenuSelect)
                End With
                clbOptionRegister001.ContextMenuStrip = mnuSelect

            End If

            subSetUpRobotCheckBox(clbOptionRobotsReq001)

            clbOptionRobotsReq001.Left = nORLeft + clbOptionRegister001.Width + nOptionRepairOffset + nHorizontalOffset

            AddHandler clbOptionRobotsReq001.ItemCheck, AddressOf clbOption_ItemCheck
            AddHandler clbOptionRobotsReq001.LostFocus, AddressOf clbAll_LostFocus
            AddHandler clbOptionRobotsReq001.GotFocus, AddressOf clbAll_GotFocus

            mnuSelect = New ContextMenuStrip
            With mnuSelect
                .Name = "clbOptionRobotsReq001"
                .Items.Add(gcsRM.GetString("csSELECT_ALL", DisplayCulture), imgVoid, AddressOf clbOption_MenuSelect)
                .Items.Add(gcsRM.GetString("csSELECT_NONE", DisplayCulture), imgVoid, AddressOf clbOption_MenuSelect)
            End With
            clbOptionRobotsReq001.ContextMenuStrip = mnuSelect

            For nIndex As Integer = 2 To mcolOptions.MaxOptions
                Dim sIndex As String = Strings.Format(nIndex, "000")
                Dim nTop As Integer = nRow1Top + ((nIndex - 1) * mnROWSPACE)

                'Plant Option Column
                sName = "ftbPlantOption" & sIndex
                Dim ftbPO As FocusedTextBox.FocusedTextBox = mScreenSetup.CloneFocusedTextBox(ftbPlantOption001, sName)
                ftbPO.Tag = sIndex
                pnlOptions.Controls.Add(ftbPO)
                ftbPO.Left = nPlantOptionLeft
                ftbPO.Top = nTop
                ftbPO.Visible = True

                'FANUC Option Column
                sName = "ftbFanucOption" & sIndex
                Dim ftbFO As FocusedTextBox.FocusedTextBox = mScreenSetup.CloneFocusedTextBox(ftbFanucOption001, sName)
                ftbFO.Tag = sIndex
                pnlOptions.Controls.Add(ftbFO)
                ftbFO.Left = nFanucOptionLeft
                ftbFO.Top = nTop
                ftbFO.Visible = True

                'Description Column
                sName = "ftbOptionDesc" & sIndex
                Dim ftbOD As FocusedTextBox.FocusedTextBox = mScreenSetup.CloneFocusedTextBox(ftbOptionDesc001, sName)
                ftbOD.Tag = sIndex
                pnlOptions.Controls.Add(ftbOD)
                ftbOD.Left = nDescLeft
                ftbOD.Top = nTop
                ftbOD.Visible = True

                'Option Register Column
                sName = "clbOptionRegister" & sIndex
                Dim clbOR As CheckedListBox = mScreenSetup.CloneCheckedListBox(clbOptionRegister001, sName)
                clbOR.Tag = sIndex
                pnlOptions.Controls.Add(clbOR)
                clbOR.Left = nORLeft
                clbOR.Top = nTop
                clbOR.Visible = mcolOptions.OptionRegisters

                AddHandler clbOR.ItemCheck, AddressOf clbOptionRegister_ItemCheck
                AddHandler clbOR.LostFocus, AddressOf clbAll_LostFocus
                AddHandler clbOR.GotFocus, AddressOf clbAll_GotFocus


                mnuSelect = New ContextMenuStrip
                With mnuSelect
                    .Name = "clbOptionRobotsReq" & sIndex
                    .Items.Add(gcsRM.GetString("csSELECT_ALL", DisplayCulture), imgVoid, AddressOf clbOption_MenuSelect)
                    .Items.Add(gcsRM.GetString("csSELECT_NONE", DisplayCulture), imgVoid, AddressOf clbOption_MenuSelect)
                End With
                clbOR.ContextMenuStrip = mnuSelect


                'Robots Required Column
                sName = "clbOptionRobotsReq" & sIndex
                Dim clbRR As CheckedListBox = mScreenSetup.CloneCheckedListBox(clbOptionRobotsReq001, sName)
                clbRR.Tag = sIndex
                pnlOptions.Controls.Add(clbRR)
                clbRR.Left = nORLeft + clbOR.Width + nOptionRepairOffset + nHorizontalOffset
                clbRR.Top = nTop
                clbRR.Visible = True

                AddHandler clbRR.ItemCheck, AddressOf clbOption_ItemCheck
                AddHandler clbRR.LostFocus, AddressOf clbAll_LostFocus
                AddHandler clbRR.GotFocus, AddressOf clbAll_GotFocus

                mnuSelect = New ContextMenuStrip
                With mnuSelect
                    .Name = "clbOptionRobotsReq" & sIndex
                    .Items.Add(gcsRM.GetString("csSELECT_ALL", DisplayCulture), imgVoid, AddressOf clbOption_MenuSelect)
                    .Items.Add(gcsRM.GetString("csSELECT_NONE", DisplayCulture), imgVoid, AddressOf clbOption_MenuSelect)
                End With
                clbRR.ContextMenuStrip = mnuSelect
            Next 'nIndex

            mScreenSetup.LoadTextBoxCollection(Me, "pnlOptions", mcolOptionTextboxes)
            Call subFtbAddHandlers(mcolOptionTextboxes, "pnlOptions")

            mScreenSetup.LoadCheckedListBoxCollection(Me, "pnlOptions", mcolOptionCLBoxes)

            lblOptionRobotsReqCap.Left = nRobotsLeft + 50

            mbOptionsTabInitialized = True

        Catch ex As Exception

        Finally
            pnlOptions.ResumeLayout()
            If DataLoaded And EditsMade = True Then EditsMade = False
            'Cleanup
            oOption = Nothing
        End Try

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
        ' 12/13/11  RJO     Added support for Sealer combined Style/Option
        ' 10/25/13  BTK     Tricoat By Style
        ' 10/30/13  BTK     Degrade Style Robots Required
        '********************************************************************************************
        mbEventBlocker = BlockEvents
        Dim bUseStyleCbo As Boolean = False
        Dim bUseRobotCbo As Boolean = False
        Dim sRobotLabel As String = gcsRM.GetString("csROBOT_CAP")
        Select Case msCurrentTabName
            Case "TabPage1"  '<<< Styles Tab
                If Not mbStylesTabInitialized Then
                    Call subLocateStyleTabControls() 'RJO 12/13/11
                    Call subShowNewStylesTab()
                End If
                ' 10/30/13  BTK     Degrade Style Robots Required
                bUseRobotCbo = mcolZones.ActiveZone.DegradeStyleRbtsReq
                If bUseRobotCbo Then
                    sRobotLabel = gpsRM.GetString("psDEGRADE", DisplayCulture)
                    If (cboRobot.SelectedIndex >= 0) Then

                        For nIndex As Integer = 1 To mcolStyles.MaxStyles
                            Dim oStyle As clsSysStyle
                            Dim sIndex As String = Strings.Format(nIndex, "000")

                            oStyle = mcolStyles(nIndex - 1)
                            Call subLoadStyleTabData(oStyle, sIndex, cboRobot.SelectedIndex)
                        Next  'nIndex
                        subEnableControls(True)
                    End If 'cboStyle.SelectedIndex > 0 
                Else
                    For nIndex As Integer = 1 To mcolStyles.MaxStyles
                        Dim oStyle As clsSysStyle
                        Dim sIndex As String = Strings.Format(nIndex, "000")

                        oStyle = mcolStyles(nIndex - 1)
                        Call subLoadStyleTabData(oStyle, sIndex, 0)
                    Next  'nIndex
                End If

            Case "TabPage2"   '<<< Options Tab
                If Not mbOptionsTabInitialized Then Call subShowNewOptionsTab()
                For nIndex As Integer = 1 To mcolOptions.MaxOptions
                    Dim oOption As clsSysOption
                    Dim sIndex As String = Strings.Format(nIndex, "000")

                    oOption = mcolOptions(nIndex - 1)
                    Call subLoadOptionTabData(oOption, sIndex)
                Next  'nIndex

            Case "TabPage3"   '<<< Repair Panels Tab
                If mcolZones.ActiveZone.RepairPanels > 0 Then
                    Dim bDegradeRepair As Boolean = mcolZones.ActiveZone.DegradeRepair
                    bUseRobotCbo = bDegradeRepair
                    If bUseRobotCbo Then
                        sRobotLabel = gpsRM.GetString("psDEGRADE", DisplayCulture)
                    End If
                    If Not mbRepairsTabInitialized Then Call subShowNewRepairsTab()
                    If cboStyle.SelectedIndex >= 0 And ((cboRobot.SelectedIndex >= 0) Or (mcolZones.ActiveZone.DegradeRepair = False)) Then
                        Dim oRepairs As clsSysRepairPanels = Nothing
                        If mcolZones.ActiveZone.DegradeRepair Then
                            oRepairs = mcolStyles.Item(cboStyle.SelectedIndex).PlantSpecific.DegradeRepairPanels.Item(cboRobot.SelectedIndex)
                        Else
                            oRepairs = mcolStyles.Item(cboStyle.SelectedIndex).PlantSpecific.DegradeRepairPanels.Item(0)
                        End If

                        For nIndex As Integer = 1 To oRepairs.Count

                            Dim oRepair As clsSysRepairPanel = oRepairs.Item(nIndex - 1)

                            Dim sIndex As String = Strings.Format(nIndex, "000")

                            Call subLoadRepairTabData(oRepair, sIndex, mcolStyles.Item(cboStyle.SelectedIndex).RepairPanelDescription(nIndex))

                        Next  'nIndex
                    End If 'cboStyle.SelectedIndex > 0 
                    bUseStyleCbo = True
                End If
            Case "TabPage4"  '<<< Degrade Tab
                If Not mbDegradeTabInitialized Then Call subShowNewDegradeTab()

            Case "TabPage5"  '<<< intrusion Tab
                If Not mbIntrusionTabInitialized Then Call subShowNewIntrusionTab()
                For nIndex As Integer = 1 To mcolStyles.MaxStyles
                    Dim oStyle As clsSysStyle
                    Dim sIndex As String = Strings.Format(nIndex, "000")

                    oStyle = mcolStyles(nIndex - 1)
                    Call subLoadIntrusionTabData(oStyle, sIndex)
                Next  'nIndex
            Case "TabPage6"
                If mcolStyleID.NumberOfConveyorSnapshots = 0 Then
                    If mnStyleIDIndex >= 0 Then
                        Call subLoadStyleIDNonTrkTabData(mnStyleIDIndex)
                    End If
                Else
                    If mnStyleIDIndex >= 0 Then
                        Call subLoadStyleIDTrackingTabData(mnStyleIDIndex)
                    End If
                End If
                '10/25/13 BTK Tricoat By Style
            Case "TabPage7"  '<<< Style Tricoat Tab
                If mcolZones.ActiveZone.TricoatByStyle Then
                    If Not mbStyleTricoatEnbTabInitialized Then Call subShowNewStyleTricoatEnbTab()
                    For nIndex As Integer = 1 To mcolStyles.MaxStyles
                        Dim oStyle As clsSysStyle
                        Dim sIndex As String = Strings.Format(nIndex, "000")

                        oStyle = mcolStyles(nIndex - 1)
                        Call subLoadStyleTricoatEnbTabData(oStyle, sIndex)
                    Next  'nIndex
                End If
        End Select
        If Not bUseRobotCbo Then subEnableControls(True)
        mbEventBlocker = False

        cboStyle.Visible = bUseStylecbo
        lblStyle.Visible = bUseStylecbo
        cboRobot.Visible = bUseRobotCbo
        lblRobot.Visible = bUseRobotCbo
        lblRobot.Text = sRobotLabel
    End Sub
    Private Sub subShowNewRepairsTab()
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
        Dim sName As String = String.Empty
        Dim nHorizontalOffset As Integer = pnlRepairs.HorizontalScroll.Value
        Dim nRow1Top As Integer = lblRepairPanel001.Top + pnlRepairs.VerticalScroll.Value
        Dim nRepairPanelLeft As Integer = lblRepairPanel001.Left + nHorizontalOffset
        Dim nDescLeft As Integer = ftbRepairDesc001.Left + nHorizontalOffset
        Dim nRobotsLeft As Integer = clbRepairRobotsReq001.Left + nHorizontalOffset
        Dim mnuSelect As ContextMenuStrip
        Dim imgVoid As Image = Nothing


        Try
            Call subUnloadPanel(pnlRepairs)

            'Create controls 
            With pnlRepairs
                .SuspendLayout()
                .AutoScroll = True
                .AutoScrollMargin = New Size(10, mnROWSPACE)
                .VerticalScroll.SmallChange = mnROWSPACE
            End With

            subSetUpRobotCheckBox(clbRepairRobotsReq001)

            AddHandler clbRepairRobotsReq001.ItemCheck, AddressOf clbRepair_ItemCheck
            AddHandler clbRepairRobotsReq001.LostFocus, AddressOf clbAll_LostFocus
            AddHandler clbRepairRobotsReq001.GotFocus, AddressOf clbAll_GotFocus

            mnuSelect = New ContextMenuStrip
            With mnuSelect
                .Name = "clbRepairRobotsReq001"
                .Items.Add(gcsRM.GetString("csSELECT_ALL", DisplayCulture), imgVoid, AddressOf clbRepair_MenuSelect)
                .Items.Add(gcsRM.GetString("csSELECT_NONE", DisplayCulture), imgVoid, AddressOf clbRepair_MenuSelect)
            End With
            clbRepairRobotsReq001.ContextMenuStrip = mnuSelect
            mnDataRows = mcolZones.ActiveZone.RepairPanels
            For nIndex As Integer = 2 To mcolZones.ActiveZone.RepairPanels
                Dim sIndex As String = Strings.Format(nIndex, "000")
                Dim nTop As Integer = nRow1Top + ((nIndex - 1) * mnROWSPACE)

                'Panel Column
                sName = "lblRepairPanel" & sIndex
                Dim lblRP As Label = mScreenSetup.CloneLabel(lblRepairPanel001, sName)
                lblRP.Tag = sIndex
                pnlRepairs.Controls.Add(lblRP)
                lblRP.Left = nRepairPanelLeft
                lblRP.Top = nTop
                lblRP.Visible = True

                'Description Column
                sName = "ftbRepairDesc" & sIndex
                Dim ftbRD As FocusedTextBox.FocusedTextBox = mScreenSetup.CloneFocusedTextBox(ftbRepairDesc001, sName)
                ftbRD.Tag = sIndex
                pnlRepairs.Controls.Add(ftbRD)
                ftbRD.Left = nDescLeft
                ftbRD.Top = nTop
                ftbRD.Visible = True

                'Robots Required Column
                sName = "clbRepairRobotsReq" & sIndex
                Dim clbRR As CheckedListBox = mScreenSetup.CloneCheckedListBox(clbRepairRobotsReq001, sName)
                clbRR.Tag = sIndex
                pnlRepairs.Controls.Add(clbRR)
                clbRR.Left = nRobotsLeft
                clbRR.Top = nTop
                clbRR.Visible = True

                AddHandler clbRR.ItemCheck, AddressOf clbRepair_ItemCheck
                AddHandler clbRR.LostFocus, AddressOf clbAll_LostFocus
                AddHandler clbRR.GotFocus, AddressOf clbAll_GotFocus

                mnuSelect = New ContextMenuStrip
                With mnuSelect
                    .Name = "clbRepairRobotsReq" & sIndex
                    .Items.Add(gcsRM.GetString("csSELECT_ALL", DisplayCulture), imgVoid, AddressOf clbRepair_MenuSelect)
                    .Items.Add(gcsRM.GetString("csSELECT_NONE", DisplayCulture), imgVoid, AddressOf clbRepair_MenuSelect)
                End With
                clbRR.ContextMenuStrip = mnuSelect
            Next 'nIndex

            If mcolZones.ActiveZone.DegradeRepair Then
                mPWRobotCommon.LoadRobotBoxFromCollection(cboRobot, mcolArms, False, , , True, True)
            End If

            mScreenSetup.LoadTextBoxCollection(Me, "pnlRepairs", mcolRepairTextboxes)
            Call subFtbAddHandlers(mcolRepairTextboxes, "pnlRepairs")

            mScreenSetup.LoadCheckedListBoxCollection(Me, "pnlRepairs", mcolRepairCLBoxes)

            lbRepairRobotsReqCap.Left = nRobotsLeft + 50

            mbRepairsTabInitialized = True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        Finally
            pnlRepairs.ResumeLayout()
            If DataLoaded And EditsMade = True Then EditsMade = False
        End Try

    End Sub
    Private Sub subShowNewIntrusionTab()
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
        Dim Culture As System.Globalization.CultureInfo = DisplayCulture
        Dim sName As String = String.Empty
        Dim nHorizontalOffset As Integer = pnlIntrusion.HorizontalScroll.Value
        Dim nRow1Top As Integer = ftbEntStart001.Top + pnlIntrusion.VerticalScroll.Value
        Dim nEntStartLeft As Integer = ftbEntStart001.Left + nHorizontalOffset
        Dim nExitLeft As Integer = ftbExitStart001.Left + nHorizontalOffset
        Dim nMuteLeft As Integer = ftbMuteLen001.Left + nHorizontalOffset
        Dim nDescLeft As Integer = lblDesc001.Left + nHorizontalOffset
        Dim nStyleLeft As Integer = lblPlantStyle001.Left + nHorizontalOffset


        Try
            Call subUnloadPanel(pnlIntrusion)

            mcolIntrusionTextboxes = New Collection(Of FocusedTextBox.FocusedTextBox)
            mnDataRows = mcolStyles.MaxStyles
            'Create controls 
            With pnlIntrusion
                .SuspendLayout()
                .AutoScroll = True
                .AutoScrollMargin = New Size(10, mnROWSPACE)
                .VerticalScroll.SmallChange = mnROWSPACE
            End With

            lblPlantStyle001.Text = String.Empty
            lblDesc001.Text = String.Empty

            ftbEntStart001.NumericOnly = True
            ftbEntStart001.Text = String.Empty
            ftbEntStart001.ForeColor = Color.Black


            ftbExitStart001.NumericOnly = True
            ftbExitStart001.Text = String.Empty
            ftbExitStart001.ForeColor = Color.Black


            ftbMuteLen001.NumericOnly = True
            ftbMuteLen001.Text = String.Empty
            ftbMuteLen001.ForeColor = Color.Black


            For nIndex As Integer = 2 To mcolStyles.MaxStyles
                Dim sIndex As String = Strings.Format(nIndex, "000")
                Dim nTop As Integer = nRow1Top + ((nIndex - 1) * mnROWSPACE)


                'Plant style Column
                sName = "lblPlantStyle" & sIndex
                Dim lblPS As Label = _
                            mScreenSetup.CloneLabel(lblPlantStyle001, sName)
                lblPS.Tag = sIndex
                pnlIntrusion.Controls.Add(lblPS)
                lblPS.Left = nStyleLeft
                lblPS.Top = nTop
                lblPS.Visible = True

                'DESC Column
                sName = "lblDesc" & sIndex
                Dim lblD As Label = _
                            mScreenSetup.CloneLabel(lblDesc001, sName)
                lblD.Tag = sIndex
                pnlIntrusion.Controls.Add(lblD)
                lblD.Left = nDescLeft
                lblD.Top = nTop
                lblD.Visible = True

                'entrance start Column
                sName = "ftbEntStart" & sIndex
                Dim ftbES As FocusedTextBox.FocusedTextBox = _
                            mScreenSetup.CloneFocusedTextBox(ftbEntStart001, sName)
                ftbES.Tag = sIndex
                pnlIntrusion.Controls.Add(ftbES)
                ftbES.Left = nEntStartLeft
                ftbES.Top = nTop
                ftbES.Visible = True

                'exit start Column
                sName = "ftbExitStart" & sIndex
                Dim ftbEx As FocusedTextBox.FocusedTextBox = _
                            mScreenSetup.CloneFocusedTextBox(ftbExitStart001, sName)
                ftbEx.Tag = sIndex
                pnlIntrusion.Controls.Add(ftbEx)
                ftbEx.Left = nExitLeft
                ftbEx.Top = nTop
                ftbEx.Visible = True

                'Mute Len Column
                sName = "ftbMuteLen" & sIndex
                Dim ftbML As FocusedTextBox.FocusedTextBox = _
                            mScreenSetup.CloneFocusedTextBox(ftbMuteLen001, sName)
                ftbML.Tag = sIndex
                pnlIntrusion.Controls.Add(ftbML)
                ftbML.Left = nMuteLeft
                ftbML.Top = nTop
                ftbML.Visible = True

            Next 'nIndex

            mScreenSetup.LoadTextBoxCollection(Me, "pnlIntrusion", mcolIntrusionTextboxes)

            Call subFtbAddHandlers(mcolIntrusionTextboxes, "pnlIntrusion")

            mbIntrusionTabInitialized = True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        Finally
            pnlIntrusion.ResumeLayout()
            If DataLoaded And EditsMade = True Then EditsMade = False
            'Cleanup
        End Try


    End Sub

    Private Sub subShowNewDegradeTab()
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
        Dim sName As String = String.Empty
        Dim nHorizontalOffset As Integer = pnlDegrade.HorizontalScroll.Value
        Dim nRow1Top As Integer = ftbRobotDown001.Top + pnlDegrade.VerticalScroll.Value
        Dim nRobotDownLeft As Integer = ftbRobotDown001.Left + nHorizontalOffset
        Dim nDegradeLeft As Integer = ftbDegrade001.Left + nHorizontalOffset


        Try
            Call subUnloadPanel(pnlDegrade)

            mcolDegradeTextboxes = New Collection(Of FocusedTextBox.FocusedTextBox)

            'Create controls 
            With pnlDegrade
                .SuspendLayout()
                .AutoScroll = True
                .AutoScrollMargin = New Size(10, mnROWSPACE)
                .VerticalScroll.SmallChange = mnROWSPACE
            End With

            ftbDegrade001.NumericOnly = True
            ftbRobotDown001.ReadOnly = True
            ftbRobotDown001.Text = mcolArms.Item(0).Name
            ftbRobotDown001.ForeColor = Color.Black
            ftbRobotDown001.BackColor = System.Drawing.SystemColors.Control

            mcolDegradeTextboxes.Add(ftbDegrade001)

            For nIndex As Integer = 2 To mcolArms.Count
                Dim sIndex As String = Strings.Format(nIndex, "000")
                Dim nTop As Integer = nRow1Top + ((nIndex - 1) * mnROWSPACE)

                'Degrade Number Column
                sName = "ftbDegrade" & sIndex
                Dim ftbPS As FocusedTextBox.FocusedTextBox = _
                            mScreenSetup.CloneFocusedTextBox(ftbDegrade001, sName)
                ftbPS.Tag = sIndex
                pnlDegrade.Controls.Add(ftbPS)
                ftbPS.Left = nDegradeLeft ' 
                ftbPS.Top = nTop
                ftbPS.Visible = True

                mcolDegradeTextboxes.Add(ftbPS)

                'Robot Down Column
                sName = "ftbRobotDown" & sIndex
                Dim ftbFS As FocusedTextBox.FocusedTextBox = _
                            mScreenSetup.CloneFocusedTextBox(ftbRobotDown001, sName)
                ftbFS.Tag = sIndex
                pnlDegrade.Controls.Add(ftbFS)
                ftbFS.Left = nRobotDownLeft
                ftbFS.Top = nTop
                ftbFS.Visible = True
                ftbFS.Text = mcolArms.Item(nIndex - 1).Name
                ftbFS.ForeColor = Color.Black

            Next 'nIndex

            Call subFtbAddHandlers(mcolDegradeTextboxes, "pnlDegrade")
            mbDegradeTabInitialized = True

        Catch ex As Exception

        Finally
            pnlDegrade.ResumeLayout()
            If DataLoaded And EditsMade = True Then EditsMade = False
            'Cleanup
        End Try

    End Sub
    Private Sub subShowNewIDTrackingTab()
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
        Dim Culture As System.Globalization.CultureInfo = DisplayCulture
        Dim sName As String = String.Empty
        Dim nHorizontalOffset As Integer = pnlStyleID.HorizontalScroll.Value
        Dim nRow1Top As Integer = ftbIDPosition001.Top + pnlStyleID.VerticalScroll.Value
        Dim nIDPosLeft As Integer = ftbIDPosition001.Left + nHorizontalOffset
        Dim nPhotoeyeLeft As Integer = btnPhotoeye001.Left + nHorizontalOffset
        Dim nPhotoeyeIndex As Integer
        Dim nItem As Integer


        Try
            Call subUnloadPanel(pnlStyleID)

            mcolStyleIDPositionTextboxes = New Collection(Of FocusedTextBox.FocusedTextBox)
            mcolStyleIDPEButtons = New Collection(Of Button)

            'Create controls 
            With pnlStyleID
                .SuspendLayout()
                .AutoScroll = True
                .AutoScrollMargin = New Size(10, mnROWSPACE)
                .VerticalScroll.SmallChange = mnROWSPACE
                .HorizontalScroll.SmallChange = mnCOLUMNSPACE
            End With

            ftbIDPosition001.NumericOnly = True
            ftbIDPosition001.Text = String.Empty
            ftbIDPosition001.ForeColor = Color.Black
            btnPhotoeye001.Text = String.Empty
            btnPhotoeye001.FlatAppearance.BorderColor = Color.Black
            If mcolStyleID.NumberOfConveyorSnapshots = 0 Then
                'No tracking
                ftbIDPosition001.Visible = False
                lblConveyorPosCap.Visible = False
            Else
                ftbIDPosition001.Visible = True
                lblConveyorPosCap.Visible = True
            End If

            'Fill out the first row of the photoeyes
            'Photoeye pattern
            For nItem = 2 To mcolStyleID.NumberOfPhotoeyes
                Dim sIndex As String = Strings.Format(nItem, "000")
                sName = "btnPhotoeye" & sIndex
                Dim btnEx As Button = _
                            CloneButton(btnPhotoeye001, sName)
                btnEx.Tag = sIndex
                pnlStyleID.Controls.Add(btnEx)
                btnEx.Left = nPhotoeyeLeft + (mnCOLUMNSPACE * (nItem - 1))
                btnEx.Top = btnPhotoeye001.Top
                btnEx.Visible = True
                btnEx.FlatAppearance.BorderColor = Color.Black
            Next 'nItem

            For nIndex As Integer = 2 To mcolStyleID.NumberOfConveyorSnapshots
                Dim sIndex As String = Strings.Format(nIndex, "000")
                Dim nTop As Integer = nRow1Top + ((nIndex - 1) * mnROWSPACE)

                'Conveyor ID Position
                sName = "ftbIDPosition" & sIndex
                Dim ftbES As TextBox = _
                            mScreenSetup.CloneFocusedTextBox(ftbIDPosition001, sName)
                ftbES.Tag = sIndex
                pnlStyleID.Controls.Add(ftbES)
                ftbES.Left = nIDPosLeft
                ftbES.Top = nTop
                ftbES.Visible = True

                'Photoeye pattern
                For nItem = 1 To mcolStyleID.NumberOfPhotoeyes
                    nPhotoeyeIndex = nItem + ((nIndex - 1) * mcolStyleID.NumberOfPhotoeyes)
                    sIndex = Strings.Format(nPhotoeyeIndex, "000")
                    sName = "btnPhotoeye" & sIndex
                    Dim btnEx As Button = _
                                CloneButton(btnPhotoeye001, sName)
                    btnEx.Tag = sIndex
                    pnlStyleID.Controls.Add(btnEx)
                    btnEx.Left = nPhotoeyeLeft + (mnCOLUMNSPACE * (nItem - 1))
                    btnEx.Top = nTop
                    btnEx.Visible = True
                    btnEx.FlatAppearance.BorderColor = Color.Black
                Next 'nItem

            Next 'nIndex

            mScreenSetup.LoadTextBoxCollection(Me, "pnlStyleID", mcolStyleIDPositionTextboxes)

            Call subFtbAddHandlers(mcolStyleIDPositionTextboxes, "pnlStyleID")

            Dim oB As Control = Nothing
            mcolStyleIDPEButtons = New Collection(Of Button)
            Dim o As Panel = DirectCast(GetControlByName("pnlStyleID", Me), Panel)

            For Each oB In o.Controls
                If TypeOf (oB) Is Button Then
                    mcolStyleIDPEButtons.Add(DirectCast(oB, Button))
                End If
            Next

            Dim oBtn As Button
            For Each oBtn In mcolStyleIDPEButtons
                AddHandler oBtn.MouseClick, AddressOf btnPhotoeyeAll_MouseClick
            Next 'oTb

            mbStyleIDTabInitialized = True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        Finally
            pnlStyleID.ResumeLayout()
            If DataLoaded And EditsMade = True Then EditsMade = False
            'Cleanup
        End Try


    End Sub
    Private Function CloneButton(ByVal ExistingButton As Button, _
                                        ByVal NewButtonName As String) As Button
        '********************************************************************************************
        'Description:  Make a TextBox just like the other one
        '
        'Parameters: TextBox to clone, and name for the new one
        'Returns:    TextBox - you do the text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        Dim f As New Button

        With f
            .Name = NewButtonName
            .Font = ExistingButton.Font
            .BackColor = ExistingButton.BackColor
            .ForeColor = ExistingButton.ForeColor
            .Height = ExistingButton.Height
            .Size = ExistingButton.Size
            .TabStop = ExistingButton.TabStop
            .TextAlign = ExistingButton.TextAlign
            .Width = ExistingButton.Width
            .Text = String.Empty
            .FlatStyle = ExistingButton.FlatStyle
            .FlatAppearance.MouseDownBackColor = ExistingButton.FlatAppearance.MouseDownBackColor
            'anchor property not set here - it messes with the autoscroll in the page layout
            '.Anchor = ExistingFTextBox.Anchor
            'existing is probably invisible at this point, but if you are cloning it, you probably
            'want to see it
            .Visible = True 'ExistingFTextBox.Visible
            'existing is probably disabled at this point, but enable here anyway
            .Enabled = True 'ExistingFTextBox.Enabled
        End With

        Return f

    End Function
    Private Sub subShowNewStylesTab()
        '********************************************************************************************
        'Description:  Data is all loaded or changed and screen needs to be refreshed
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/30/10  MSW     Change to look at ascii enable in option/style/color config
        ' 02/22/11  JBW     Added two coats by style checkboxes
        ' 12/13/11  RJO     Added support for Sealer combined Style/Option
        ' 10/30/13  BTK     Degrade Style Robots Required
        '********************************************************************************************
        Dim oStyle As clsSysStyle
        Dim sName As String = String.Empty
        Dim nHorizontalOffset As Integer = pnlStyles.HorizontalScroll.Value
        Dim nRow1Top As Integer = ftbPlantStyle001.Top + pnlStyles.VerticalScroll.Value
        Dim nPlantStyleLeft As Integer = ftbPlantStyle001.Left + nHorizontalOffset
        Dim nPlantOptionLeft As Integer = ftbStyleOpt001.Left + nHorizontalOffset 'RJO 12/13/11
        Dim nFanucStyleLeft As Integer = ftbFanucStyle001.Left + nHorizontalOffset
        Dim nDescLeft As Integer = ftbStyleDesc001.Left + nHorizontalOffset
        Dim nRobotsLeft As Integer = clbStyleRobotsReq001.Left + nHorizontalOffset
        Dim mnuSelect As ContextMenuStrip
        Dim imgVoid As Image = Nothing
        Dim nTCLeft As Integer = cbTwoCoats001.Left + nHorizontalOffset
        Dim nSRLeft As Integer = ftbStyleRegister001.Left + nHorizontalOffset

        mnDataRows = mcolStyles.MaxStyles

        Try
            Call subUnloadPanel(pnlStyles)

            'Create controls 
            With pnlStyles
                .SuspendLayout()
                .AutoScroll = True
                .AutoScrollMargin = New Size(10, mnROWSPACE)
                .VerticalScroll.SmallChange = mnROWSPACE
            End With

            ftbPlantStyle001.NumericOnly = (mcolStyles.UseAscii = False)
            ftbStyleOpt001.NumericOnly = True 'RJO 12/13/11
            ftbFanucStyle001.NumericOnly = True

            subSetUpRobotCheckBox(clbStyleRobotsReq001)

            AddHandler clbStyleRobotsReq001.ItemCheck, AddressOf clbStyle_ItemCheck
            AddHandler clbStyleRobotsReq001.LostFocus, AddressOf clbAll_LostFocus
            AddHandler clbStyleRobotsReq001.GotFocus, AddressOf clbAll_GotFocus
            'jbw added for two coat process 02/23/11
            AddHandler cbTwoCoats001.CheckedChanged, AddressOf TwoCoats_CheckedChanged


            mnuSelect = New ContextMenuStrip
            With mnuSelect
                .Name = "clbStyleRobotsReq001"
                .Items.Add(gcsRM.GetString("csSELECT_ALL", DisplayCulture), imgVoid, AddressOf clbStyle_MenuSelect)
                .Items.Add(gcsRM.GetString("csSELECT_NONE", DisplayCulture), imgVoid, AddressOf clbStyle_MenuSelect)
            End With
            clbStyleRobotsReq001.ContextMenuStrip = mnuSelect


            For nIndex As Integer = 2 To mcolStyles.MaxStyles
                Dim sIndex As String = Strings.Format(nIndex, "000")
                Dim nTop As Integer = nRow1Top + ((nIndex - 1) * mnROWSPACE)

                'Plant Style Column
                sName = "ftbPlantStyle" & sIndex
                Dim ftbPS As FocusedTextBox.FocusedTextBox = mScreenSetup.CloneFocusedTextBox(ftbPlantStyle001, sName)
                ftbPS.Tag = sIndex
                pnlStyles.Controls.Add(ftbPS)
                ftbPS.Left = nPlantStyleLeft
                ftbPS.Top = nTop
                'SA
                ftbPS.Visible = Not mcolZones.StandAlone

                'Plant Option Column 'RJO 12/13/11
                sName = "ftbStyleOpt" & sIndex
                Dim ftbPO As FocusedTextBox.FocusedTextBox = mScreenSetup.CloneFocusedTextBox(ftbStyleOpt001, sName)
                ftbPO.Tag = sIndex
                pnlStyles.Controls.Add(ftbPO)
                ftbPO.Left = nPlantOptionLeft
                ftbPO.Top = nTop
                ftbPO.Visible = mcolStyles.CombinedStyleOption

                'FANUC Style Column
                sName = "ftbFanucStyle" & sIndex
                Dim ftbFS As FocusedTextBox.FocusedTextBox = mScreenSetup.CloneFocusedTextBox(ftbFanucStyle001, sName)
                ftbFS.Tag = sIndex
                pnlStyles.Controls.Add(ftbFS)
                ftbFS.Left = nFanucStyleLeft
                ftbFS.Top = nTop
                ftbFS.Visible = True

                'Description Column
                sName = "ftbStyleDesc" & sIndex
                Dim ftbSD As FocusedTextBox.FocusedTextBox = mScreenSetup.CloneFocusedTextBox(ftbStyleDesc001, sName)
                ftbSD.Tag = sIndex
                pnlStyles.Controls.Add(ftbSD)
                ftbSD.Left = nDescLeft
                ftbSD.Top = nTop
                ftbSD.Visible = True

                'Robots Required Column
                sName = "clbStyleRobotsReq" & sIndex
                Dim clbRR As CheckedListBox = mScreenSetup.CloneCheckedListBox(clbStyleRobotsReq001, sName)
                clbRR.Tag = sIndex
                pnlStyles.Controls.Add(clbRR)
                clbRR.Left = nRobotsLeft
                clbRR.Top = nTop
                'SA
                clbRR.Visible = Not mcolZones.StandAlone

                AddHandler clbRR.ItemCheck, AddressOf clbStyle_ItemCheck
                AddHandler clbRR.LostFocus, AddressOf clbAll_LostFocus
                AddHandler clbRR.GotFocus, AddressOf clbAll_GotFocus

                'jbw added two coats checkbox
                sName = "cbTwoCoats" & sIndex
                Dim cbTC As CheckBox = mScreenSetup.CloneCheckBox(cbTwoCoats001, sName)
                cbTC.Tag = sIndex
                pnlStyles.Controls.Add(cbTC)
                cbTC.Left = nTCLeft
                cbTC.Top = nTop
                cbTC.Visible = mcolStyles.TwoCoat

                AddHandler cbTC.CheckedChanged, AddressOf TwoCoats_CheckedChanged

                'Style Register Column
                ftbStyleRegister001.NumericOnly = True
                sName = "ftbStyleRegister" & sIndex
                Dim ftbSR As FocusedTextBox.FocusedTextBox = mScreenSetup.CloneFocusedTextBox(ftbStyleRegister001, sName)
                ftbSR.Tag = sIndex
                pnlStyles.Controls.Add(ftbSR)
                ftbSR.Left = nSRLeft
                ftbSR.Top = nTop
                ftbSR.Visible = mcolStyles.StyleRegisters

                mnuSelect = New ContextMenuStrip
                With mnuSelect
                    .Name = "clbStyleRobotsReq" & sIndex
                    .Items.Add(gcsRM.GetString("csSELECT_ALL", DisplayCulture), imgVoid, AddressOf clbStyle_MenuSelect)
                    .Items.Add(gcsRM.GetString("csSELECT_NONE", DisplayCulture), imgVoid, AddressOf clbStyle_MenuSelect)
                End With
                clbRR.ContextMenuStrip = mnuSelect
            Next 'nIndex

            ' 10/30/13  BTK     Degrade Style Robots Required
            If mcolZones.ActiveZone.DegradeStyleRbtsReq Then
                'mPWRobotCommon.LoadRobotBoxFromCollection(cboRobot, mcolArms, False, , , True, True) 'RJO 07/23/14
                'RJO 07/23/14 Populate the list of valid degrade modes from SystemStylesRobotRequiredByDegrade.XML
                Dim sXMLPath As String = String.Empty
                Dim DS As New DataSet

                mPWCommon.GetDefaultFilePath(sXMLPath, eDir.XML, String.Empty, String.Empty)
                DS.ReadXmlSchema(sXMLPath & "SystemDegradeModes.XSD")
                DS.ReadXml(sXMLPath & "SystemDegradeModes.XML")

                cboRobot.Items.Clear()

                Dim DRs() As DataRow = DS.Tables("Item").Select("Enable > 0", "TableIndex ASC")

                For Each DR As DataRow In DRs
                    cboRobot.Items.Add(DR.Item("Description").ToString)
                Next
            End If

            mScreenSetup.LoadTextBoxCollection(Me, "pnlStyles", mcolStyleTextboxes)
            'jbw 03/03/11 checkbox stuff
            mScreenSetup.LoadCheckBoxCollection(Me, "pnlstyles", mcolStyleCheckBoxes)
            Call subFtbAddHandlers(mcolStyleTextboxes, "pnlStyles")

            mScreenSetup.LoadCheckedListBoxCollection(Me, "pnlStyles", mcolStyleCLBoxes)

            lblStyleRobotsReqCap.Left = nRobotsLeft + 50

            mbStylesTabInitialized = True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        Finally
            pnlStyles.ResumeLayout()
            If DataLoaded And EditsMade = True Then EditsMade = False
            'Cleanup
            oStyle = Nothing
        End Try

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

        If MessageBox.Show(gcsRM.GetString("csUNDOMSG", DisplayCulture), _
                                        msSCREEN_NAME, MessageBoxButtons.OKCancel, _
                                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) _
                                                                    = Response.OK Then
            subLoadData(eLoadModes.LoadFromDB)
            'SA
            If Not mcolZones.StandAlone Then
                subLoadData(eLoadModes.LoadFromPLC)
            End If
        End If

    End Sub
    Private Sub subUnloadPanel(ByRef oPanel As Panel)
        '********************************************************************************************
        'Description:  Clear out oPanel (except for the first row)
        '
        'Parameters: Panel to clear
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 07/29/13  HGB     There was no code to handle the two coats check box. On a zone change an 
        '                   exception occurred since the default type was a label.
        '********************************************************************************************
        Dim nCount As Integer = oPanel.Controls.Count

        For nItem As Integer = nCount - 1 To 0 Step -1
            Dim oControl As Control = oPanel.Controls.Item(nItem)

            If oControl.Tag.ToString <> "001" Then
                If TypeOf (oControl) Is FocusedTextBox.FocusedTextBox Then
                    Dim o As FocusedTextBox.FocusedTextBox = DirectCast(oControl, FocusedTextBox.FocusedTextBox)

                    RemoveHandler o.DownArrow, AddressOf ftbAll_DownArrow
                    RemoveHandler o.LeftArrow, AddressOf ftbAll_LeftArrow
                    RemoveHandler o.RightArrow, AddressOf ftbAll_RightArrow
                    RemoveHandler o.TextChanged, AddressOf ftbAll_TextChanged
                    RemoveHandler o.UpArrow, AddressOf ftbAll_UpArrow

                    Select Case oPanel.Name
                        Case "pnlStyles"
                            RemoveHandler o.Validated, AddressOf ftbStyle_Validated
                            RemoveHandler o.Validating, AddressOf ftbStyle_Validating
                        Case "pnlOptions"
                            RemoveHandler o.Validated, AddressOf ftbOption_Validated
                            RemoveHandler o.Validating, AddressOf ftbOption_Validating
                        Case "pnlRepairs"
                            RemoveHandler o.Validated, AddressOf ftbRepair_Validated
                            RemoveHandler o.Validating, AddressOf ftbRepair_Validating
                        Case "pnlDegrade"
                            RemoveHandler o.Validated, AddressOf ftbDegrade_Validated
                            RemoveHandler o.Validating, AddressOf ftbDegrade_Validating
                        Case "pnlStyleID"
                            RemoveHandler o.Validated, AddressOf ftbStyleID_Validated
                            RemoveHandler o.Validating, AddressOf ftbStyleID_Validating
                    End Select

                    oPanel.Controls.Remove(o)
                ElseIf TypeOf (oControl) Is CheckedListBox Then
                    Dim o As CheckedListBox = DirectCast(oControl, CheckedListBox)

                    RemoveHandler o.LostFocus, AddressOf clbAll_LostFocus

                    Select Case oPanel.Name
                        Case "pnlStyles"
                            RemoveHandler o.ItemCheck, AddressOf clbStyle_ItemCheck
                            RemoveHandler o.GotFocus, AddressOf clbAll_GotFocus
                        Case "pnlOptions"
                            RemoveHandler o.ItemCheck, AddressOf clbOption_ItemCheck
                            RemoveHandler o.GotFocus, AddressOf clbAll_GotFocus
                        Case "pnlRepairs"
                            RemoveHandler o.ItemCheck, AddressOf clbRepair_ItemCheck
                            RemoveHandler o.GotFocus, AddressOf clbAll_GotFocus
                    End Select

                    oPanel.Controls.Remove(o)
                ElseIf TypeOf (oControl) Is Button Then
                    Dim o As Button = DirectCast(oControl, Button)

                    Select Case oPanel.Name
                        Case "pnlStyleID"
                            RemoveHandler o.MouseClick, AddressOf btnPhotoeyeAll_MouseClick
                    End Select

                    oPanel.Controls.Remove(o)
                    'HGB add code to clear out the two coats checkbox 07/29/13
                ElseIf TypeOf (oControl) Is CheckBox Then
                    Dim o As CheckBox = DirectCast(oControl, CheckBox)

                    Select Case oPanel.Name
                        Case "pnlStyles"
                            RemoveHandler o.CheckedChanged, AddressOf TwoCoats_CheckedChanged

                    End Select
                    oPanel.Controls.Remove(o)
                Else
                    'Must be a label
                    Dim o As Label = DirectCast(oControl, Label)

                    oPanel.Controls.Remove(o)
                End If
            End If
        Next 'nItem

        'get things back to their normal positions
        oPanel.HorizontalScroll.Value = 0
        oPanel.Invalidate()
        oPanel.Update()

    End Sub
    Private Overloads Sub subUpdateChangeLog(ByRef NewValue As String, _
                                        ByRef OldValue As String, ByRef ParamName As String, _
                                        ByRef oZone As clsZone, ByRef WhatChanged As String, _
                                        ByRef DeviceName As String)
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
    Friend Overloads Sub subUpdateChangeLogForCopy(ByRef oZone As clsZone, ByRef WhatChanged As String, _
                                        ByRef DeviceName As String)
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
        Dim sTmp As String = WhatChanged

        Call mPWCommon.AddChangeRecordToCollection(gsChangeLogArea, LoggedOnUser, _
                                        oZone.ZoneNumber, DeviceName, String.Empty, sTmp, oZone.Name)

    End Sub

    Private Overloads Sub subUpdateChangeLog(ByRef ArmdownString As String, ByRef NewValue As String, _
                                            ByRef OldValue As String, ByRef StyleName As String, _
                                            ByVal ZoneNumber As Integer, ByRef Area As String, _
                                            ByRef Device As String)
        '********************************************************************************************
        'Description:  Build up the change text for a degrade number change
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim Culture As System.Globalization.CultureInfo = DisplayCulture
        Dim sTmp As String = ArmdownString & " " & gpsRM.GetString("psDEGRADE_CHGLOG", Culture) & " "


        sTmp = sTmp & gcsRM.GetString("csCHANGED_FROM", Culture) & " " & OldValue & " " & _
                        gcsRM.GetString("csTO", Culture) & " " & NewValue

        Call mPWCommon.AddChangeRecordToCollection(gsChangeLogArea, LoggedOnUser, _
                                        ZoneNumber, Device, StyleName, sTmp, mcolZones.ActiveZone.Name)

    End Sub
    Private Overloads Sub subUpdateChangeLog(ByVal ZoneNumber As Integer)
        '********************************************************************************************
        'Description:  build up the change text for Restore from DB or PLC
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Call mPWCommon.AddChangeRecordToCollection(gsChangeLogArea, LoggedOnUser, ZoneNumber, _
                                                " ", " ", msRestoreString, mcolZones.ActiveZone.Name)

    End Sub

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
        '    09/30/13   MSW     Save screenshots as jpegs
        '********************************************************************************************
        Dim sKeyValue As String = String.Empty

        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode
                Case Keys.F1
                    'Help Screen request
                    sKeyValue = "btnHelp"
                    Select Case tabMain.SelectedTab.Name
                        Case "TabPage1"
                            If mcolStyles.CombinedStyleOption Then
                                subLaunchHelp(gs_HELP_JOBSETUP_STYLEOPTION, oIPC)
                            Else
                                subLaunchHelp(gs_HELP_JOBSETUP_STYLES, oIPC)
                            End If
                        Case "TabPage2"
                            subLaunchHelp(gs_HELP_JOBSETUP_OPTIONS, oIPC)
                        Case "TabPage3"
                            subLaunchHelp(gs_HELP_JOBSETUP_REPAIR, oIPC)
                        Case "TabPage4"
                            subLaunchHelp(gs_HELP_JOBSETUP_DEGRADE, oIPC)
                        Case "TabPage5"
                            subLaunchHelp(gs_HELP_JOBSETUP_INTRUSION, oIPC)
                        Case "TabPage6"
                            If mcolStyleID.NumberOfConveyorSnapshots > 0 Then
                                subLaunchHelp(gs_HELP_JOBSETUP_STYLEID, oIPC)
                            Else
                                subLaunchHelp(gs_HELP_JOBSETUP_STYLEIDSS, oIPC)
                            End If
                    End Select


                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty
                    Dim sName As String = msSCREEN_DUMP_NAME
                    Select Case tabMain.SelectedTab.Name
                        Case "TabPage1"
                            If mcolStyles.CombinedStyleOption Then
                                sName = sName & "_StyleOptions"
                            Else
                                sName = sName & "_Styles"
                            End If
                        Case "TabPage2"
                            sName = sName & "_Options"
                        Case "TabPage3"
                            sName = sName & "_Repair"
                        Case "TabPage4"
                            sName = sName & "_Degrade"
                        Case "TabPage5"
                            sName = sName & "_Intrusion"
                        Case "TabPage6"
                            If mcolStyleID.NumberOfConveyorSnapshots > 0 Then
                                sName = sName & "_StyleID"
                            Else
                                sName = sName & "_StyleIDSS"
                            End If
                    End Select
                    sName = sName & ".jpg"
                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath & sName, Imaging.ImageFormat.Jpeg)

                Case Keys.Escape
                    Me.Close()

                Case Else

            End Select
        End If

        'TODO - Add HotKeys and (invisible) Help button
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
    Private Sub frmMain_Layout(Optional ByVal sender As Object = Nothing, _
                    Optional ByVal e As System.Windows.Forms.LayoutEventArgs = Nothing) Handles MyBase.Layout
        '********************************************************************************************
        'Description:  Form needs a redraw due to resize
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/09/11  MSW     Fix sizing for PEC tab
        '********************************************************************************************

        'todo find out what this -70 is from.
        Dim nHeight As Integer = tscMain.ContentPanel.Height - 70
        If nHeight < 100 Then nHeight = 100
        Dim nWidth As Integer = tscMain.ContentPanel.Width - (PnlMain.Left * 2)
        If nWidth < 100 Then nWidth = 100
        PnlMain.Height = nHeight
        PnlMain.Width = nWidth
        nWidth = nWidth - 2 * tabMain.Left
        nHeight = nHeight - 2 * tabMain.Top
        tabMain.Width = nWidth
        tabMain.Height = nHeight
        Select Case tabMain.SelectedTab.Name
            Case "TabPage1"
                pnlStyles.Width = TabPage1.Width - (pnlStyles.Left + 10)
                pnlStyles.Height = TabPage1.Height - pnlStyles.Top - 5
            Case "TabPage2"
                pnlOptions.Width = TabPage2.Width - (pnlOptions.Left + 10)
                pnlOptions.Height = TabPage2.Height - pnlOptions.Top - 5
            Case "TabPage3"
                pnlRepairs.Width = TabPage3.Width - (pnlRepairs.Left + 10)
                pnlRepairs.Height = TabPage3.Height - pnlRepairs.Top - 5
            Case "TabPage4"
                pnlDegrade.Width = TabPage4.Width - (pnlDegrade.Left + 10)
                pnlDegrade.Height = TabPage4.Height - pnlDegrade.Top - 5
            Case "TabPage5"
                pnlIntrusion.Width = TabPage5.Width - (pnlIntrusion.Left + 10)
                pnlIntrusion.Height = TabPage5.Height - pnlIntrusion.Top - 5
        End Select

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

        subInitializeForm()

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
                                            msROBOT_ASSEMBLY_LOCAL)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

#End Region

#Region " Temp Stuff for Old Password Object "


    Private Sub moPassword_LogIn() Handles moPassword.LogIn
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/15/10  MSW     handle cross-thread calls
        ' 08/19/10  MSW     Fix the last fix so it''s not asking for execute or copyauthority 
        '********************************************************************************************
        'This can get called by the password thread
        If Me.stsStatus.InvokeRequired Then
            Dim dLogin As New LoginCallBack(AddressOf moPassword_LogIn)
            Me.BeginInvoke(dLogin)
        Else
            Me.LoggedOnUser = moPassword.UserName
            Status = Me.LoggedOnUser & " " & gcsRM.GetString("csLOGGED_IN")
            'Privilege = ePrivilege.Execute 'don't allow Override Access without this priviledge 01/05/10 RJO
            'If Privilege <> ePrivilege.Execute Then
            'Privilege = ePrivilege.Copy ' extra for buttons etc.
            'If Privilege <> ePrivilege.Copy Then
            'didn't have clearance
            Privilege = ePrivilege.Edit
            'End If
            'End If 
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/21/12
        End If
    End Sub
    Private Sub moPassword_LogOut() Handles moPassword.LogOut
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/15/10  MSW     handle cross-thread calls
        '********************************************************************************************
        'This can get called by the password thread
        If Me.stsStatus.InvokeRequired Then
            Dim dLogin As New LogOutCallBack(AddressOf moPassword_LogOut)
            Me.BeginInvoke(dLogin)
        Else
            Status = Me.LoggedOnUser & " " & gcsRM.GetString("csLOGGED_OUT")
            Me.LoggedOnUser = String.Empty
            Me.Privilege = ePrivilege.None
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/21/12
        End If

    End Sub


#End Region

#Region " Focused Text Box Events "

    Private Sub ftbAll_DownArrow(ByRef sender As FocusedTextBox.FocusedTextBox, _
                ByVal bAlt As Boolean, ByVal bShift As Boolean, ByVal bControl As Boolean)
        '********************************************************************************************
        'Description:  someone hit the down arrow key empty text should be checked for by sender
        '              
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'TODO - This business below could be done more efficiently
        Dim sNumber As String = Strings.Right(sender.Name, 3)
        Dim sName As String = Strings.Left(sender.Name, (Len(sender.Name) - 3))
        Dim sNewName As String = String.Empty
        Dim nTmp As Integer = 0
        Dim o As FocusedTextBox.FocusedTextBox
        Dim oContainer As Control = Nothing

        nTmp = CType(sNumber, Integer) + 1
        If nTmp > mnDataRows Then
            'last one - back to top
            nTmp = 1
        End If ' nTmp = 11

        sNewName = sName & Strings.Format(nTmp, "000")

        Select Case tabMain.SelectedTab.Name
            Case "TabPage1"
                oContainer = pnlStyles
            Case "TabPage2"
                oContainer = pnlOptions
            Case "TabPage3"
                oContainer = pnlRepairs
            Case "TabPage4"
                oContainer = pnlDegrade
            Case "TabPage5"
                oContainer = pnlIntrusion
            Case "TabPage6"
                oContainer = pnlStyleID
        End Select

        o = DirectCast(oContainer.Controls(sNewName), FocusedTextBox.FocusedTextBox)
        If IsNothing(o) Then Exit Sub

        o.Focus()

    End Sub
    Private Sub ftbAll_LeftArrow(ByRef sender As FocusedTextBox.FocusedTextBox, _
                    ByVal bAlt As Boolean, ByVal bShift As Boolean, ByVal bControl As Boolean)
        '********************************************************************************************
        'Description:  someone hit the left arrow key empty text should be checked for by sender
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sNumber As String = Strings.Right(sender.Name, 3)
        Dim sName As String = Strings.Left(sender.Name, (Len(sender.Name) - 3))
        Dim sNewName As String = String.Empty
        Dim o As FocusedTextBox.FocusedTextBox
        Dim oContainer As Control = Nothing

        Select Case tabMain.SelectedTab.Name
            Case "TabPage1"
                oContainer = pnlStyles
                If mcolStyles.CombinedStyleOption Then
                    Select Case sName
                        ' set name of control to left
                        Case "ftbPlantStyle"
                            sNewName = "ftbStyleDesc" & sNumber
                        Case "ftbStyleOpt"
                            sNewName = "ftbPlantStyle" & sNumber
                        Case "ftbFanucStyle"
                            sNewName = "ftbStyleOpt" & sNumber
                        Case "ftbStyleDesc"
                            sNewName = "ftbFanucStyle" & sNumber
                    End Select
                Else
                    Select Case sName
                        ' set name of control to left
                        Case "ftbPlantStyle"
                            sNewName = "ftbStyleDesc" & sNumber
                        Case "ftbFanucStyle"
                            sNewName = "ftbPlantStyle" & sNumber
                        Case "ftbStyleDesc"
                            sNewName = "ftbFanucStyle" & sNumber
                    End Select
                End If

            Case "TabPage2"
                oContainer = pnlOptions
                Select Case sName
                    ' set name of control to left
                    Case "ftbPlantOption"
                        sNewName = "ftbOptionDesc" & sNumber
                    Case "ftbFanucOption"
                        sNewName = "ftbPlantOption" & sNumber
                    Case "ftbOptionDesc"
                        sNewName = "ftbFanucOption" & sNumber
                End Select

            Case "TabPage3"
                oContainer = pnlRepairs
                'TODO

            Case "TabPage4"
                oContainer = pnlDegrade

            Case "TabPage5"
                oContainer = pnlIntrusion
                Select Case sName
                    ' set name of control to left
                    Case "ftbEntStart"
                        sNewName = "ftbMuteLen" & sNumber
                    Case "ftbExitStart"
                        sNewName = "ftbEntStart" & sNumber
                    Case "ftbMuteLen"
                        sNewName = "ftbExitStart" & sNumber
                End Select
            Case "TabPage6"
                oContainer = pnlStyleID
                Select Case sName
                    Case "ftbIDPosition"
                        sNewName = "ftbIDPosition" & sNumber
                End Select

        End Select

        o = DirectCast(oContainer.Controls(sNewName), FocusedTextBox.FocusedTextBox)
        If IsNothing(o) Then Exit Sub

        o.Focus()

    End Sub
    Private Sub ftbAll_RightArrow(ByRef sender As FocusedTextBox.FocusedTextBox, _
                    ByVal bAlt As Boolean, ByVal bShift As Boolean, ByVal bControl As Boolean)
        '********************************************************************************************
        'Description:  someone hit the right arrow key empty text should be checked for by sender
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sNumber As String = Strings.Right(sender.Name, 3)
        Dim sName As String = Strings.Left(sender.Name, (Len(sender.Name) - 3))
        Dim sNewName As String = String.Empty
        Dim o As FocusedTextBox.FocusedTextBox
        Dim oContainer As Control = Nothing

        Select Case tabMain.SelectedTab.Name
            Case "TabPage1"
                oContainer = pnlStyles
                If mcolStyles.CombinedStyleOption Then
                    Select Case sName
                        ' set name of control to left
                        Case "ftbPlantStyle"
                            sNewName = "ftbStyleOpt" & sNumber
                        Case "ftbStyleOpt"
                            sNewName = "ftbFanucStyle" & sNumber
                        Case "ftbFanucStyle"
                            sNewName = "ftbStyleDesc" & sNumber
                        Case "ftbStyleDesc"
                            sNewName = "ftbPlantStyle" & sNumber
                    End Select
                Else
                    Select Case sName
                        ' set name of control to right
                        Case "ftbPlantStyle"
                            sNewName = "ftbFanucStyle" & sNumber
                        Case "ftbFanucStyle"
                            sNewName = "ftbStyleDesc" & sNumber
                        Case "ftbStyleDesc"
                            sNewName = "ftbPlantStyle" & sNumber
                    End Select
                End If

            Case "TabPage2"
                oContainer = pnlOptions
                Select Case sName
                    ' set name of control to right
                    Case "ftbPlantOption"
                        sNewName = "ftbFanucOption" & sNumber
                    Case "ftbFanucOption"
                        sNewName = "ftbOptionDesc" & sNumber
                    Case "ftbOptionDesc"
                        sNewName = "ftbPlantOption" & sNumber
                End Select

            Case "TabPage3"
                oContainer = pnlRepairs
                'TODO

            Case "TabPage4"
                oContainer = pnlDegrade

            Case "TabPage5"
                oContainer = pnlIntrusion
                Select Case sName
                    ' set name of control to right
                    Case "ftbEntStart"
                        sNewName = "ftbExitStart" & sNumber
                    Case "ftbExitStart"
                        sNewName = "ftbMuteLen" & sNumber
                    Case "ftbMuteLen"
                        sNewName = "ftbEntStart" & sNumber
                End Select
            Case "TabPage6"
                oContainer = pnlStyleID
                Select Case sName
                    Case "ftbIDPosition"
                        sNewName = "ftbIDPosition" & sNumber
                End Select
        End Select

        o = DirectCast(oContainer.Controls(sNewName), FocusedTextBox.FocusedTextBox)
        If IsNothing(o) Then Exit Sub

        o.Focus()

    End Sub
    Private Sub ftbAll_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  generic textbox change Routine
        '
        'Parameters: event parameters
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        DirectCast(sender, FocusedTextBox.FocusedTextBox).ForeColor = Color.Red
        If mbEventBlocker Then Exit Sub
        If DataLoaded And EditsMade = False Then EditsMade = True

    End Sub

    Private Sub subTextboxKeypressHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        '********************************************************************************************
        'Description: Check for keypress that requires login
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/18/09  MSW     subTextboxKeypressHandler, subSetTextBoxProperties - Offer a login if they try to enter data without a login
        '********************************************************************************************
        Dim oFTB As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sName As String = Strings.Left(oFTB.Name, (Len(oFTB.Name) - 3))
        If Char.IsNumber(e.KeyChar) OrElse Char.IsLetter(e.KeyChar) OrElse Char.IsWhiteSpace(e.KeyChar) Then
            If oFTB.ReadOnly Then
                If Privilege = ePrivilege.None Then
                    Privilege = ePrivilege.Edit
                End If
            Else
                'if using ascii plant numbers cap the length
                If (sName = "ftbPlantStyle") And mcolStyles.UseAscii Then
                    If (oFTB.Text.Length - oFTB.SelectionLength) >= mcolStyles.PlantAsciiMaxLength OrElse Char.IsWhiteSpace(e.KeyChar) Then
                        e.Handled = True
                    End If
                End If
            End If
        Else
            If Char.IsPunctuation(e.KeyChar) OrElse Char.IsSymbol(e.KeyChar) Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub subTextboxKeyUpHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        '********************************************************************************************
        'Description: Check for keyup from navigate keys
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/30/10  MSW     handle page up, page down, home, end
        '********************************************************************************************
        Dim oFTB As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sNumber As String = Strings.Right(oFTB.Name, 3)
        Dim sName As String = Strings.Left(oFTB.Name, (Len(oFTB.Name) - 3))
        Dim sNewName As String = String.Empty
        Dim nTmp As Integer = 0
        Dim o As FocusedTextBox.FocusedTextBox
        Dim oContainer As Control = Nothing
        Dim nMax As Integer = mnDataRows
        Select Case tabMain.SelectedTab.Name
            Case "TabPage1"
                oContainer = pnlStyles
            Case "TabPage2"
                oContainer = pnlOptions
            Case "TabPage3"
                oContainer = pnlRepairs
            Case "TabPage4"
                oContainer = pnlDegrade
            Case "TabPage5"
                oContainer = pnlIntrusion
            Case "TabPage6"
                oContainer = pnlStyleID
        End Select
        nTmp = CType(sNumber, Integer)
        Select Case e.KeyCode
            Case Keys.Home
                'validate the current box before shifting it around
                cboZone.Focus()
                Application.DoEvents()
                Select Case tabMain.SelectedTab.Name
                    Case "TabPage1"
                        sNewName = "ftbPlantStyle" & Strings.Format(nTmp, "000")
                    Case "TabPage2"
                        sNewName = "ftbPlantOption" & Strings.Format(nTmp, "000")
                    Case "TabPage3"
                        sNewName = "ftbRepairDesc" & Strings.Format(nTmp, "000")
                    Case "TabPage4"
                        sNewName = sNewName & Strings.Format(nTmp, "000")
                    Case "TabPage5"
                        sNewName = "ftbEntStart" & Strings.Format(nTmp, "000")
                    Case "TabPage6"
                        sNewName = "ftbIDPosition"
                End Select
            Case Keys.End
                'validate the current box before shifting it around
                cboZone.Focus()
                Application.DoEvents()
                Select Case tabMain.SelectedTab.Name
                    Case "TabPage1"
                        sNewName = "ftbStyleDesc" & Strings.Format(nTmp, "000")
                    Case "TabPage2"
                        sNewName = "ftbOptionDesc" & Strings.Format(nTmp, "000")
                    Case "TabPage3"
                        sNewName = "ftbRepairDesc" & Strings.Format(nTmp, "000")
                    Case "TabPage4"
                        sNewName = sNewName & Strings.Format(nTmp, "000")
                    Case "TabPage5"
                        sNewName = "ftbMuteLen" & Strings.Format(nTmp, "000")
                    Case "TabPage6"
                        sNewName = "ftbIDPosition"
                End Select
            Case Keys.PageUp
                'validate the current box before shifting it around
                cboZone.Focus()
                Application.DoEvents()
                nTmp = nTmp - 10
                If nTmp < 1 Then
                    If nTmp <= -9 Then
                        nTmp = nMax
                    Else
                        nTmp = 1
                    End If
                End If

                sNewName = sName & Strings.Format(nTmp, "000")

            Case Keys.PageDown
                'validate the current box before shifting it around
                cboZone.Focus()
                Application.DoEvents()
                nTmp = nTmp + 10
                If nTmp > nMax Then
                    If nTmp >= nMax + 10 Then
                        nTmp = 1
                    Else
                        nTmp = nMax
                    End If
                End If

                sNewName = sName & Strings.Format(nTmp, "000")

            Case Else
        End Select
        o = DirectCast(oContainer.Controls(sNewName), FocusedTextBox.FocusedTextBox)
        If IsNothing(o) Then Exit Sub
        o.Focus()
    End Sub
    Private Sub ftbAll_UpArrow(ByRef sender As FocusedTextBox.FocusedTextBox, _
                ByVal bAlt As Boolean, ByVal bShift As Boolean, ByVal bControl As Boolean)
        '********************************************************************************************
        'Description:  someone hit the up arrow key empty text should be checked for by sender
        '              
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sNumber As String = Strings.Right(sender.Name, 3)
        Dim sName As String = Strings.Left(sender.Name, (Len(sender.Name) - 3))
        Dim sNewName As String = String.Empty
        Dim nTmp As Integer = 0
        Dim o As FocusedTextBox.FocusedTextBox
        Dim oContainer As Control = Nothing

        nTmp = CType(sNumber, Integer) - 1
        If nTmp < 1 Then
            'first one - back to bottom
            nTmp = mnDataRows
        End If ' nTmp = 

        sNewName = sName & Strings.Format(nTmp, "000")

        Select Case tabMain.SelectedTab.Name
            Case "TabPage1"
                oContainer = pnlStyles
            Case "TabPage2"
                oContainer = pnlOptions
            Case "TabPage3"
                oContainer = pnlRepairs
            Case "TabPage4"
                oContainer = pnlDegrade
            Case "TabPage5"
                oContainer = pnlIntrusion
            Case "TabPage6"
                oContainer = pnlStyleID

        End Select

        o = DirectCast(oContainer.Controls(sNewName), FocusedTextBox.FocusedTextBox)
        If IsNothing(o) Then Exit Sub

        o.Focus()

    End Sub
    Private Sub ftbIntrusion_Validated(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validated Routine
        '
        'Parameters: textbox to check, which column of textboxex
        'Returns:    true if bum data so it cam be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oFtb As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sIndex As String = oFtb.Tag.ToString
        Dim nItem As Integer = CType(sIndex, Integer) - 1
        Dim oStyle As clsSysStyle

        If DataLoaded = False Then Exit Sub

        oStyle = mcolStyles.Item(nItem)

        Select Case oFtb.Name
            ' Styles Tab
            Case "ftbEntStart" & sIndex
                oStyle.EntranceMuteStartCount.Value = CType(oFtb.Text, Integer)
                If oStyle.EntranceMuteStartCount.Changed Then EditsMade = True
            Case "ftbExitStart" & sIndex
                oStyle.ExitMuteStartCount.Value = CType(oFtb.Text, Integer)
                If oStyle.ExitMuteStartCount.Changed Then EditsMade = True
            Case "ftbMuteLen" & sIndex
                oStyle.MuteLength.Value = CType(oFtb.Text, Integer)
                If oStyle.MuteLength.Changed Then EditsMade = True
        End Select


    End Sub
    Private Sub ftbIntrusion_Validating(ByVal sender As Object, _
                        ByVal e As System.ComponentModel.CancelEventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validating Routine
        '
        'Parameters: textbox to check, which column of textboxex
        'Returns:    true if bum data so it cam be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim Culture As System.Globalization.CultureInfo = DisplayCulture

        'if no edit priv and bad data this locks us up
        If Privilege = ePrivilege.None Then
            e.Cancel = True
            Exit Sub
        End If

        Dim oFtb As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sIndex As String = oFtb.Tag.ToString
        Dim nItem As Integer = CType(sIndex, Integer) - 1
        Dim bBadData As Boolean = False
        Dim oStyle As clsSysStyle
        Dim nMin As Integer = 0
        Dim nMax As Integer = mnDogSpace
        Dim nOld As Integer = 0

        oFtb.BackColor = Color.White

        'no value?
        If Strings.Len(oFtb.Text) = 0 Then
            bBadData = True
            MessageBox.Show(gcsRM.GetString("csNO_NULL", Culture), gcsRM.GetString("csINVALID_DATA", Culture), _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

        oStyle = mcolStyles.Item(nItem)


        If bBadData = False Then

            If Not (IsNumeric(oFtb.Text)) Then
                bBadData = True
                MessageBox.Show(gcsRM.GetString("csBAD_ENTRY", Culture), gcsRM.GetString("csINVALID_DATA", Culture), _
                               MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            ' limit check
            If bBadData = False Then

                'low limit
                If CInt(oFtb.Text) < nMin Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csVAL_BELOW_MIN", Culture) & vbCrLf & _
                                    gcsRM.GetString("csMINIMUM_EQ", Culture) & nMin, _
                                    gcsRM.GetString("csINVALID_DATA", Culture), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                'hi limit
                If CInt(oFtb.Text) > 1000000 Then 'nMax Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csVAL_ABOVE_MAX", Culture) & vbCrLf & _
                                    gcsRM.GetString("csMAXIMUM_EQ", Culture) & nMax, _
                                    gcsRM.GetString("csINVALID_DATA", Culture), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

            End If '  bBadData = False

        End If ' bBadData = False

        If bBadData Then
            oFtb.Undo()
            e.Cancel = True
        End If

    End Sub
    Private Sub ftbDegrade_Validated(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validated Routine
        '
        'Parameters: textbox to check, which column of textboxex
        'Returns:    true if bum data so it cam be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oFtb As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sIndex As String = oFtb.Tag.ToString
        Dim nItem As Integer = CType(sIndex, Integer)
        Dim oStyle As clsSysStyle
        If DataLoaded = False Then Exit Sub

        oStyle = oGetDegradeStyle()
        oStyle.PlantSpecific.DegradeNumber(nGetDegradeControllerNum, nItem).Value = _
                                                                        CType(oFtb.Text, Integer)
        If oStyle.PlantSpecific.DegradeNumber(nGetDegradeControllerNum, nItem).Changed _
                                                                        Then EditsMade = True

    End Sub
    Private Sub ftbDegrade_Validating(ByVal sender As Object, _
                            ByVal e As System.ComponentModel.CancelEventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validating Routine
        '
        'Parameters: textbox to check, which column of textboxex
        'Returns:    true if bum data so it cam be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'if no edit priv and bad data this locks us up
        If Privilege = ePrivilege.None Then
            e.Cancel = True
            Exit Sub
        End If

        Dim oFtb As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sIndex As String = oFtb.Tag.ToString
        'Dim nItem As Integer = CType(sIndex, Integer) - 1
        Dim bBadData As Boolean = False
        Dim nMin As Integer = 0
        Dim nMax As Integer = mcolArms.Count

        oFtb.BackColor = Color.White

        'no value?
        If Strings.Len(oFtb.Text) = 0 Then
            bBadData = True
            MessageBox.Show(gcsRM.GetString("csNO_NULL", DisplayCulture), gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If




        If bBadData = False Then

            If Not (IsNumeric(oFtb.Text)) Then
                bBadData = True
                MessageBox.Show(gcsRM.GetString("csBAD_ENTRY", DisplayCulture), gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                               MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            ' limit check
            If bBadData = False Then

                'low limit
                If CInt(oFtb.Text) < nMin Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csVAL_BELOW_MIN", DisplayCulture) & vbCrLf & _
                                    gcsRM.GetString("csMINIMUM_EQ", DisplayCulture) & nMin, _
                                    gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                'hi limit
                If CInt(oFtb.Text) > nMax Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csVAL_ABOVE_MAX", DisplayCulture) & vbCrLf & _
                                    gcsRM.GetString("csMAXIMUM_EQ", DisplayCulture) & nMax, _
                                    gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

            End If '  bBadData = False

        End If ' bBadData = False

        If bBadData Then
            'oFtb.Text = nOld.ToString
            oFtb.Undo()
            e.Cancel = True
        End If

    End Sub
    Private Sub ftbOption_Validated(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validated Routine
        '
        'Parameters: textbox to check, which column of textboxex
        'Returns:    true if bum data so it cam be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oFtb As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sIndex As String = oFtb.Tag.ToString
        Dim nItem As Integer = CType(sIndex, Integer) - 1
        Dim oOption As clsSysOption

        If DataLoaded = False Then Exit Sub

        oOption = mcolOptions.Item(nItem)
        Select Case oFtb.Name
            ' Styles Tab
            Case "ftbPlantOption" & sIndex
                oOption.PlantNumber.Text = oFtb.Text
                If oOption.PlantNumber.Changed Then EditsMade = True
            Case "ftbFanucOption" & sIndex
                oOption.FanucNumber.Value = CType(oFtb.Text, Integer)
                If oOption.PlantNumber.Changed Then EditsMade = True
            Case "ftbOptionDesc" & sIndex
                oOption.Description.Text = oFtb.Text
                If oOption.Description.Changed Then EditsMade = True
                'Case "ftbOptionRegister" & sIndex
                '    oOption.OptionRegister.Value = CType(oFtb.Text, Integer)
                '    If oOption.OptionRegister.Changed Then EditsMade = True
        End Select

    End Sub
    Private Sub ftbOption_Validating(ByVal sender As Object, _
                            ByVal e As System.ComponentModel.CancelEventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validating Routine
        '
        'Parameters: textbox to check, which column of textboxex
        'Returns:    true if bum data so it cam be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/12/09  MSW     Make duplicates a warning so it doesn't lock up the program
        '********************************************************************************************
        'if no edit priv and bad data this locks us up
        If Privilege = ePrivilege.None Then
            e.Cancel = True
            Exit Sub
        End If

        Dim oFtb As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sIndex As String = oFtb.Tag.ToString
        Dim nItem As Integer = CType(sIndex, Integer) - 1
        Dim bBadData As Boolean = False
        Dim oOption As clsSysOption
        Dim nMin As Integer = mcolOptions.PlantOptionMinValue
        Dim nMax As Integer = mcolOptions.PlantOptionMaxValue
        Dim nOld As Integer = 0

        oFtb.BackColor = Color.White

        'no value?
        If Strings.Len(oFtb.Text) = 0 Then
            bBadData = True
            MessageBox.Show(gcsRM.GetString("csNO_NULL", DisplayCulture), _
                            gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

        oOption = mcolOptions.Item(nItem)

        Select Case oFtb.Name
            Case "ftbPlantOption" & sIndex

                If IsNumeric(oFtb.Text) Then
                    'Check for duplicate Plant Style numbers
                    For nIndex As Integer = 0 To mcolOptions.Count - 1
                        If nIndex <> nItem Then
                            If oFtb.Text = mcolOptions.Item(nIndex).PlantNumber.Text Then
                                bBadData = True
                                MessageBox.Show(gpsRM.GetString("psDUPLICATE_OPTION", DisplayCulture), _
                                                gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                'oFtb.Undo()
                                e.Cancel = True
                                Exit For
                            End If
                        End If
                    Next 'nIndex
                    If bBadData Then Exit Sub
                End If

            Case "ftbFanucOption" & sIndex
                nMin = oOption.FanucNumber.MinValue
                nMax = oOption.FanucNumber.MaxValue
                nOld = oOption.FanucNumber.Value

                'Case "ftbOptionRegister" & sIndex
                '    nMin = mcolOptions.RegisterMinValue
                '    nMax = mcolOptions.RegisterMaxValue
                '    nOld = oOption.OptionRegister.Value

            Case "ftbOptionDesc" & sIndex
                If Strings.Len(oFtb.Text) > oOption.Description.MaxLength Then  ' the class object MaxLength does not work returning 255
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csBAD_ENTRY", DisplayCulture), _
                                    gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    oFtb.Undo()
                    e.Cancel = True
                End If
                Exit Sub

        End Select

        If bBadData = False Then

            If Not (IsNumeric(oFtb.Text)) Then
                bBadData = True
                MessageBox.Show(gcsRM.GetString("csBAD_ENTRY", DisplayCulture), _
                                gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                               MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            ' limit check
            If bBadData = False Then

                'low limit
                If CInt(oFtb.Text) < nMin Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csVAL_BELOW_MIN", DisplayCulture) & vbCrLf & _
                                    gcsRM.GetString("csMINIMUM_EQ", DisplayCulture) & nMin, _
                                    gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                'hi limit
                If CInt(oFtb.Text) > nMax Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csVAL_ABOVE_MAX", DisplayCulture) & vbCrLf & _
                                    gcsRM.GetString("csMAXIMUM_EQ", DisplayCulture) & nMax, _
                                    gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

            End If '  bBadData = False

        End If ' bBadData = False

        If bBadData Then
            oFtb.Undo()
            e.Cancel = True
        End If

    End Sub
    Private Sub ftbRepair_Validated(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validated Routine
        '
        'Parameters: textbox to check, which column of textboxex
        'Returns:    true if bum data so it cam be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If DataLoaded = False Then Exit Sub
        If cboStyle.Text = String.Empty Then Exit Sub

        Dim oFtb As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sIndex As String = oFtb.Tag.ToString
        Dim nItem As Integer = CType(sIndex, Integer)

        mcolStyles.Item(cboStyle.SelectedIndex).RepairPanelDescription(nItem).Text = oFtb.Text
        If mcolStyles.Item(cboStyle.SelectedIndex).RepairPanelDescription(nItem).Changed Then EditsMade = True

    End Sub
    Private Sub ftbRepair_Validating(ByVal sender As Object, _
                          ByVal e As System.ComponentModel.CancelEventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validating Routine
        '
        'Parameters: textbox to check
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'if no edit priv and bad data this locks us up
        If Privilege = ePrivilege.None Then
            e.Cancel = True
            Exit Sub
        End If

        Dim oFtb As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim bBadData As Boolean = False

        oFtb.BackColor = Color.White

        'The only FTB on the RepairPanel Panel is the Description field.

        'no value?
        If Strings.Len(oFtb.Text) = 0 Then
            bBadData = True
            MessageBox.Show(gcsRM.GetString("csNO_NULL", DisplayCulture), _
                            gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

        'max length should be same for all
        Dim oDescriptions() As clsTextValue = mcolStyles.Item(cboStyle.SelectedIndex).RepairPanelDescriptions
 
        If Strings.Len(oFtb.Text) > _
                    oDescriptions(0).MaxLength Then

            bBadData = True
            MessageBox.Show(gcsRM.GetString("csBAD_ENTRY", DisplayCulture), _
                            gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            oFtb.Undo()
            e.Cancel = True
        End If

    End Sub
    Private Sub ftbStyle_Validated(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validated Routine
        '
        'Parameters: textbox to check, which column of textboxex
        'Returns:    true if bum data so it cam be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/13/11  RJO     Added support for Sealer combined Style/Option
        '********************************************************************************************
        Dim oFtb As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sIndex As String = oFtb.Tag.ToString
        Dim nItem As Integer = CType(sIndex, Integer) - 1
        Dim oStyle As clsSysStyle

        If DataLoaded = False Then Exit Sub

        oStyle = mcolStyles.Item(nItem)
        Select Case oFtb.Name
            ' Styles Tab
            Case "ftbPlantStyle" & sIndex
                oStyle.PlantNumber.Text = oFtb.Text
                If oStyle.PlantNumber.Changed Then EditsMade = True
            Case "ftbStyleOpt" & sIndex 'RJO 12/13/11
                oStyle.OptionNumber.Text = oFtb.Text
                If oStyle.OptionNumber.Changed Then EditsMade = True
            Case "ftbFanucStyle" & sIndex
                oStyle.FanucNumber.Value = CType(oFtb.Text, Integer)
                If oStyle.PlantNumber.Changed Then EditsMade = True
            Case "ftbStyleDesc" & sIndex
                oStyle.Description.Text = oFtb.Text
                If oStyle.Description.Changed Then EditsMade = True
            Case "ftbStyleRegister" & sIndex
                oStyle.StyleRegister.Value = CType(oFtb.Text, Integer)
                If oStyle.StyleRegister.Changed Then EditsMade = True
        End Select

    End Sub
    Private Sub ftbStyleID_Validated(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validated Routine
        '
        'Parameters: textbox to check, which column of textboxex
        'Returns:    true if bum data so it cam be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oFtb As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sIndex As String = oFtb.Tag.ToString
        Dim nItem As Integer = CType(sIndex, Integer) - 1
        Dim oStyle As clsSysStyleID

        If DataLoaded = False Then Exit Sub

        oStyle = mcolStyleID.Item(nItem + (mnStyleIDIndex * mcolStyleID.NumberOfConveyorSnapshots))
        Select Case oFtb.Name
            ' Style ID Tab
            Case "ftbIDPosition" & sIndex
                oStyle.ConveyorCount.Value = CType(oFtb.Text, Integer)
                If oStyle.ConveyorCount.Changed Then EditsMade = True
        End Select

    End Sub
    Private Sub ftbStyleID_Validating(ByVal sender As Object, _
                        ByVal e As System.ComponentModel.CancelEventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validating Routine
        '
        'Parameters: textbox to check, which column of textboxex
        'Returns:    true if bum data so it cam be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'if no edit priv and bad data this locks us up
        If Privilege = ePrivilege.None Then
            e.Cancel = True
            Exit Sub
        End If

        Dim oFtb As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sIndex As String = oFtb.Tag.ToString
        Dim nItem As Integer = CType(sIndex, Integer) - 1
        Dim bBadData As Boolean = False
        Dim oStyle As clsSysStyleID
        Dim nMin As Integer = 0
        Dim nMax As Integer = 0
        Dim nOld As Integer = 0

        oFtb.BackColor = Color.White

        'no value?
        If Strings.Len(oFtb.Text) = 0 Then
            bBadData = True
            MessageBox.Show(gcsRM.GetString("csNO_NULL", DisplayCulture), _
                            gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

        oStyle = mcolStyleID.Item(nItem)

        Select Case oFtb.Name
            Case "ftbIDPosition" & sIndex
                'Plant Style is numberic
                nMin = mcolStyleID.IDPositionLowerLimit
                nMax = mcolStyleID.IDPositionUpperLimit
                nOld = CType(oStyle.ConveyorCount.Value, Integer)
            Case Else
                bBadData = True
        End Select

        If bBadData = False Then

            If Not (IsNumeric(oFtb.Text)) Then
                bBadData = True
                MessageBox.Show(gcsRM.GetString("csBAD_ENTRY", DisplayCulture), _
                                gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                               MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            ' limit check
            If bBadData = False Then

                'low limit
                If CInt(oFtb.Text) < nMin Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csVAL_BELOW_MIN", DisplayCulture) & vbCrLf & _
                                    gcsRM.GetString("csMINIMUM_EQ", DisplayCulture) & nMin, _
                                    gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                'hi limit
                If CInt(oFtb.Text) > nMax Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csVAL_ABOVE_MAX", DisplayCulture) & vbCrLf & _
                                    gcsRM.GetString("csMAXIMUM_EQ", DisplayCulture) & nMax, _
                                    gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

            End If '  bBadData = False

        End If ' bBadData = False

        If bBadData Then
            oFtb.Text = nOld.ToString
            e.Cancel = True
        End If

    End Sub

    Private Sub ftbStyle_Validating(ByVal sender As Object, _
                            ByVal e As System.ComponentModel.CancelEventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validating Routine
        '
        'Parameters: textbox to check, which column of textboxes
        'Returns:    true if bum data so it cam be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/13/11  RJO     Added support for Sealer combined Style/Option
        '********************************************************************************************

        'if no edit priv and bad data this locks us up
        If Privilege = ePrivilege.None Then
            e.Cancel = True
            Exit Sub
        End If

        Dim oFtb As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sIndex As String = oFtb.Tag.ToString
        Dim nItem As Integer = CType(sIndex, Integer) - 1
        Dim bBadData As Boolean = False
        Dim oStyle As clsSysStyle
        Dim nMin As Integer = 0
        Dim nMax As Integer = 0
        Dim nOld As Integer = 0

        oFtb.BackColor = Color.White

        'no value?
        If Strings.Len(oFtb.Text) = 0 Then
            bBadData = True
            MessageBox.Show(gcsRM.GetString("csNO_NULL", DisplayCulture), _
                            gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

        oStyle = mcolStyles.Item(nItem)

        Select Case oFtb.Name
            Case "ftbPlantStyle" & sIndex
                If mcolStyles.UseAscii Then
                    'geo if the controls max lenght is set when the controls are created then this should never happen
                    If Strings.Len(oFtb.Text) > oStyle.Description.MaxLength Then  ' the class object MaxLength does not work returning 255
                        bBadData = True
                        MessageBox.Show(gcsRM.GetString("csBAD_ENTRY", DisplayCulture), _
                                        gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oFtb.Undo()
                        lblStyleDescCap.Focus()
                        e.Cancel = True
                        Exit Sub
                    End If

                    'Check for duplicate Plant Ascii Style numbers
                    For nIndex As Integer = 0 To mcolStyles.Count - 1
                        If nIndex <> nItem Then
                            If oFtb.Text = mcolStyles.Item(nIndex).PlantNumber.Text Then
                                bBadData = True
                                MessageBox.Show(gpsRM.GetString("psDUPLICATE_STYLE", DisplayCulture), _
                                                gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                'oFtb.Undo()
                                lblStyleDescCap.Focus()
                                e.Cancel = True
                                Exit For
                            End If
                        End If
                    Next

                    Exit Sub
                Else 'Plant Style is numeric
                    nMin = mcolStyles.PlantStyleMinValue
                    nMax = mcolStyles.PlantStyleMaxValue
                    nOld = CType(oStyle.PlantNumber.Text, Integer)

                    If Not mcolStyles.CombinedStyleOption Then 'RJO 12/13/11
                        If IsNumeric(oFtb.Text) And CType(oFtb.Text, Integer) <> 0 Then
                            'Check for duplicate Plant Style numbers
                            For nIndex As Integer = 0 To mcolStyles.Count - 1
                                If nIndex <> nItem Then
                                    If oFtb.Text = mcolStyles.Item(nIndex).PlantNumber.Text Then
                                        bBadData = True
                                        MessageBox.Show(gpsRM.GetString("psDUPLICATE_STYLE", DisplayCulture), _
                                                            gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                                                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                        'oFtb.Undo()
                                        e.Cancel = True
                                        Exit For
                                    End If
                                End If
                            Next 'nIndex
                            If bBadData Then Exit Sub
                        End If
                    End If 'mcolStyles.UseAscii
                End If 'Not mcolStyles.CombinedStyleOption

            Case "ftbStyleOpt" & sIndex 'RJO 12/13/11
                nMin = mcolStyles.PlantOptionMinValue
                nMax = mcolStyles.PlantOptionMaxValue
                nOld = CType(oStyle.OptionNumber.Text, Integer)

            Case "ftbFanucStyle" & sIndex
                nMin = oStyle.FanucNumber.MinValue
                nMax = oStyle.FanucNumber.MaxValue
                nOld = oStyle.FanucNumber.Value

            Case "ftbStyleRegister" & sIndex
                nMin = mcolStyles.RegisterMinValue
                nMax = mcolStyles.RegisterMaxValue
                nOld = oStyle.StyleRegister.Value

            Case "ftbStyleDesc" & sIndex
                If Strings.Len(oFtb.Text) > oStyle.Description.MaxLength Then  ' the class object MaxLength does not work returning 255
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csBAD_ENTRY", DisplayCulture), _
                                    gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    oFtb.Undo()
                    e.Cancel = True
                End If
                Exit Sub

        End Select

        If bBadData = False Then

            If Not (IsNumeric(oFtb.Text)) Then
                bBadData = True
                MessageBox.Show(gcsRM.GetString("csBAD_ENTRY", DisplayCulture), _
                                gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                               MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            ' limit check
            If bBadData = False Then

                'low limit
                If CInt(oFtb.Text) < nMin Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csVAL_BELOW_MIN", DisplayCulture) & vbCrLf & _
                                    gcsRM.GetString("csMINIMUM_EQ", DisplayCulture) & nMin, _
                                    gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                'hi limit
                If CInt(oFtb.Text) > nMax Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csVAL_ABOVE_MAX", DisplayCulture) & vbCrLf & _
                                    gcsRM.GetString("csMAXIMUM_EQ", DisplayCulture) & nMax, _
                                    gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

            End If '  bBadData = False

        End If ' bBadData = False

        If mcolStyles.CombinedStyleOption And (Not bBadData) Then 'RJO 12/13/11
            Dim bDoCheck As Boolean = True
            Dim nStyleOpt As Integer

            'Check for duplicate Style/Option combination
            Select Case oFtb.Name
                Case "ftbPlantStyle" & sIndex
                    If CType(oFtb.Text, Integer) = 0 Then
                        bDoCheck = False
                    Else
                        nStyleOpt = CType(oFtb.Text, Integer) * 100
                        nStyleOpt += CType(mcolStyles.Item(nItem).OptionNumber.Text, Integer)
                    End If

                Case "ftbStyleOpt" & sIndex
                    If CType(mcolStyles.Item(nItem).PlantNumber.Text, Integer) = 0 Then
                        bDoCheck = False
                    Else
                        nStyleOpt = CType(mcolStyles.Item(nItem).PlantNumber.Text, Integer) * 100
                        nStyleOpt += CType(oFtb.Text, Integer)
                    End If

                Case Else
                    bDoCheck = False

            End Select

            If bDoCheck Then
                'Check for duplicate Plant Style numbers
                For nIndex As Integer = 0 To mcolStyles.Count - 1
                    If nIndex <> nItem Then
                        Dim nCheckStyleOpt As Integer = CType(mcolStyles.Item(nIndex).PlantNumber.Text, Integer) * 100

                        nCheckStyleOpt += CType(mcolStyles.Item(nIndex).OptionNumber.Text, Integer)
                        If nCheckStyleOpt = nStyleOpt Then
                            bBadData = True
                            MessageBox.Show(gpsRM.GetString("psDUPLICATE_STYLE_OPT", DisplayCulture), _
                                                gcsRM.GetString("csINVALID_DATA", DisplayCulture), _
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            oFtb.Undo()
                            e.Cancel = True
                            Exit For
                        End If
                    End If
                Next 'nIndex
            End If 'bDoCheck

        End If 'mcolStyles.CombinedStyleOption

        If bBadData Then
            oFtb.Text = nOld.ToString
            e.Cancel = True
        End If

    End Sub

#End Region

#Region "Checked ListBox Events"

    Private Sub clbAll_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description: The checked listbox got focus. Change the BackColor to yellow.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        DirectCast(sender, CheckedListBox).BackColor = Color.Yellow

    End Sub
    Private Sub clbAll_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description: The checked listbox lost focus. Change the BackColor to White.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        DirectCast(sender, CheckedListBox).BackColor = Color.White
        DirectCast(sender, CheckedListBox).ClearSelected()

    End Sub
    Private Sub clbOption_ItemCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs)
        '********************************************************************************************
        'Description: An Option checked listbox item was just checked.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/23/09  BTK     Added code so we can change which bit the robot required data starts at.
        '********************************************************************************************
        Dim sOptionIndex As Integer = CInt(DirectCast(sender, CheckedListBox).Tag)  ' option index stored in .tag
        Dim nValueToSet As Integer
        Dim oOption As clsSysOption = mcolOptions.Item(sOptionIndex - 1) ' ALSO BASE 0   TAGS ARE BASED 1
        Dim nArmBit As Integer = mcolArms(e.Index).RobotNumber - 1

        If e.NewValue = 1 Then
            nValueToSet = mMathFunctions.SetBit(oOption.RobotsRequired.Value, nArmBit + mcolZones.ActiveZone.RobotsRequiredStartingBit, False)
        Else
            nValueToSet = mMathFunctions.ReSetBit(oOption.RobotsRequired.Value, nArmBit + mcolZones.ActiveZone.RobotsRequiredStartingBit, False)
        End If

        oOption.RobotsRequired.Value = nValueToSet

        If oOption.RobotsRequired.Changed Then
            EditsMade = True
            CType(sender, CheckedListBox).ForeColor = Color.Red
        End If


    End Sub
    Private Sub clbOption_MenuSelect(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  context select all/none handler
        '
        'Parameters: sender is menuitem
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oMenuItem As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim oClb As CheckedListBox = DirectCast(pnlOptions.Controls.Item(oMenuItem.Owner.Name), CheckedListBox)
        Dim bTmp As Boolean

        If Privilege = ePrivilege.None Then Exit Sub

        Call clbAll_GotFocus(oClb, e)

        Select Case oMenuItem.Text
            Case gcsRM.GetString("csSELECT_ALL", DisplayCulture)
                bTmp = True
            Case Else
                bTmp = False
        End Select

        For i As Integer = 0 To oClb.Items.Count - 1
            oClb.SetItemChecked(i, bTmp)
        Next

        Call clbAll_LostFocus(oClb, e)

    End Sub
    Private Sub clbOptionRegister_ItemCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs)
        '********************************************************************************************
        'Description: An Option checked listbox item was just checked.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/23/09  BTK     Added code so we can change which bit the robot required data starts at.
        '********************************************************************************************
        Dim sOptionIndex As Integer = CInt(DirectCast(sender, CheckedListBox).Tag)  ' option index stored in .tag
        Dim nValueToSet As Integer
        Dim oOption As clsSysOption = mcolOptions.Item(sOptionIndex - 1) ' ALSO BASE 0   TAGS ARE BASED 1

        If e.NewValue = 1 Then
            nValueToSet = mMathFunctions.SetBit(oOption.OptionRegister.Value, e.Index, False)
        Else
            nValueToSet = mMathFunctions.ReSetBit(oOption.OptionRegister.Value, e.Index, False)
        End If

        oOption.OptionRegister.Value = nValueToSet

        If oOption.OptionRegister.Changed Then
            EditsMade = True
            CType(sender, CheckedListBox).ForeColor = Color.Red
        End If


    End Sub
    Private Sub clbOptionRegister_MenuSelect(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  context select all/none handler
        '
        'Parameters: sender is menuitem
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oMenuItem As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim oClb As CheckedListBox = DirectCast(pnlOptions.Controls.Item(oMenuItem.Owner.Name), CheckedListBox)
        Dim bTmp As Boolean

        If Privilege = ePrivilege.None Then Exit Sub

        Call clbAll_GotFocus(oClb, e)

        Select Case oMenuItem.Text
            Case gcsRM.GetString("csSELECT_ALL", DisplayCulture)
                bTmp = True
            Case Else
                bTmp = False
        End Select

        For i As Integer = 0 To oClb.Items.Count - 1
            oClb.SetItemChecked(i, bTmp)
        Next

        Call clbAll_LostFocus(oClb, e)

    End Sub

    Private Sub clbRepair_ItemCheck(ByVal sender As Object, _
                                            ByVal e As System.Windows.Forms.ItemCheckEventArgs)
        '********************************************************************************************
        'Description:  A repair checked listbox item has been checked
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If Not DataLoaded Then Exit Sub
        If cboStyle.Text = String.Empty Then Exit Sub

        Dim nRobotIndex As Integer = e.Index
        Dim nPanelIndex As Integer = CInt(DirectCast(sender, CheckedListBox).Tag) - 1 '  index stored in .tag

        Dim oRepairs As clsSysRepairPanels = Nothing
        If mcolZones.ActiveZone.DegradeRepair Then
            oRepairs = mcolStyles.Item(cboStyle.SelectedIndex).PlantSpecific.DegradeRepairPanels.Item(cboRobot.SelectedIndex)
        Else
            oRepairs = mcolStyles.Item(cboStyle.SelectedIndex).PlantSpecific.DegradeRepairPanels.Item(0)
        End If
        Dim oRepair As clsSysRepairPanel = _
                    oRepairs.Item(nPanelIndex)

        If e.NewValue = 1 Then
            oRepair.RobotRequired(nRobotIndex).Value = True
        Else
            oRepair.RobotRequired(nRobotIndex).Value = False
        End If

        If oRepair.RobotRequired(nRobotIndex).Changed Then
            EditsMade = True
            DirectCast(sender, CheckedListBox).ForeColor = Color.Red
        End If
    End Sub
    Private Sub clbRepair_MenuSelect(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  context select all/none handler
        '
        'Parameters: sender is menuitem
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oMenuItem As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim oClb As CheckedListBox = DirectCast(pnlRepairs.Controls.Item(oMenuItem.Owner.Name), CheckedListBox)
        Dim bTmp As Boolean

        If Privilege = ePrivilege.None Then Exit Sub

        Call clbAll_GotFocus(oClb, e)

        Select Case oMenuItem.Text
            Case gcsRM.GetString("csSELECT_ALL", DisplayCulture)
                bTmp = True
            Case Else
                bTmp = False
        End Select

        For i As Integer = 0 To oClb.Items.Count - 1
            oClb.SetItemChecked(i, bTmp)
        Next

        Call clbAll_LostFocus(oClb, e)

    End Sub
    Private Sub clbStyle_ItemCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs)
        '********************************************************************************************
        'Description:  A style checked listbox item has been checked
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/23/09  BTK     Added code so we can change which bit the robot required data starts at.
        ' 07/23/14  RJO     Changed call to SetBit and ResetBit to use 32Bit mode if 16+ robots
        '********************************************************************************************
        Dim sStyleIndex As Integer = CInt(DirectCast(sender, CheckedListBox).Tag)  ' style index stored in .tag
        Dim nValueToSet As Integer
        Dim oS As clsSysStyle = mcolStyles.Item(sStyleIndex - 1) ' ALSO BASE 0   TAGS ARE BASED 1
        Dim nArmBit As Integer = mcolArms(e.Index).RobotNumber - 1

        If mcolZones.ActiveZone.DegradeStyleRbtsReq Then
            If e.NewValue = 1 Then
                nValueToSet = mMathFunctions.SetBit(oS.DegradeRobotsRequired(cboRobot.SelectedIndex).Value, nArmBit + mcolZones.ActiveZone.RobotsRequiredStartingBit, mb32Bit)
            Else
                nValueToSet = mMathFunctions.ReSetBit(oS.DegradeRobotsRequired(cboRobot.SelectedIndex).Value, nArmBit + mcolZones.ActiveZone.RobotsRequiredStartingBit, mb32Bit)
            End If

            oS.DegradeRobotsRequired(cboRobot.SelectedIndex).Value = nValueToSet
            If oS.DegradeRobotsRequired(cboRobot.SelectedIndex).Changed Then
                EditsMade = True
                DirectCast(sender, CheckedListBox).ForeColor = Color.Red
            End If
        Else
            If e.NewValue = 1 Then
                nValueToSet = mMathFunctions.SetBit(oS.RobotsRequired.Value, nArmBit + mcolZones.ActiveZone.RobotsRequiredStartingBit, mb32Bit)
            Else
                nValueToSet = mMathFunctions.ReSetBit(oS.RobotsRequired.Value, nArmBit + mcolZones.ActiveZone.RobotsRequiredStartingBit, mb32Bit)
            End If

            oS.RobotsRequired.Value = nValueToSet
            If oS.RobotsRequired.Changed Then
                EditsMade = True
                DirectCast(sender, CheckedListBox).ForeColor = Color.Red
            End If
        End If

    End Sub
    Private Sub clbStyle_MenuSelect(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  context select all/none handler
        '
        'Parameters: sender is menuitem
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oMenuItem As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim oClb As CheckedListBox = _
                    DirectCast(pnlStyles.Controls.Item(oMenuItem.Owner.Name), CheckedListBox)
        Dim bTmp As Boolean


        If Privilege = ePrivilege.None Then Exit Sub

        Call clbAll_GotFocus(oClb, e)

        Select Case oMenuItem.Text
            Case gcsRM.GetString("csSELECT_ALL", DisplayCulture)
                bTmp = True
            Case Else
                bTmp = False
        End Select

        For i As Integer = 0 To oClb.Items.Count - 1
            oClb.SetItemChecked(i, bTmp)
        Next

        Call clbAll_LostFocus(oClb, e)

    End Sub


#End Region

#Region " Table and Button Events "

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

        Dim cachePriviledge As ePrivilege

        If LoggedOnUser <> String.Empty Then

            'If moPrivilege.Privilege = String.Empty Then 'RJO 03/21/12
            '    cachePriviledge = ePrivilege.None
            'Else
            '    If moPrivilege.ActionAllowed Then
            '        cachePriviledge = mPassword.PrivilegeStringToEnum(moPrivilege.Privilege)
            '    Else
            '        cachePriviledge = ePrivilege.None
            '    End If
            'End If

            If moPassword.Privilege = ePrivilege.None Then
                cachePriviledge = ePrivilege.None
            Else
                If moPassword.ActionAllowed Then
                    cachePriviledge = moPassword.Privilege
                Else
                    cachePriviledge = ePrivilege.None
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
            Privilege = cachePriviledge

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
        '        RemotePWServer = String.Empty

        If mcolArms Is Nothing = False Then
            For i As Integer = mcolArms.Count - 1 To 0 Step -1
                mcolArms.Remove(mcolArms(i))
            Next
            mcolArms = Nothing
        End If
        If mcolControllers Is Nothing = False Then
            For i As Integer = mcolControllers.Count - 1 To 0 Step -1
                mcolControllers.Remove(mcolControllers(i))
            Next
            mcolControllers = Nothing
        End If
        GC.Collect()
        Application.DoEvents()
        mPWRobotCommon.UrbanRenewal()
        Application.DoEvents()
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
        'Description:  print the table
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If DataLoaded Then
            bPrintdoc(True)
        End If
    End Sub
    Private Sub mnuExport_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                            Handles mnuExport.Click
        '********************************************************************************************
        'Description:  export the table to a csv file
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        bPrintdoc(True, True)

    End Sub
    Private Sub mnuImport_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                Handles mnuImport.Click
        '********************************************************************************************
        'Description:  Import settings from a csv file
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/30/11  MSW     1st version - main style page, options.
        '********************************************************************************************
        Const nITEM As Integer = 1
        Const nFANUC_STYLE As Integer = 2
        Const nPLANT_STYLE As Integer = 3
        Const nFANUC_OPTION As Integer = 4
        Const nPLANT_OPTION As Integer = 5
        Const nDESCRIPTION As Integer = 6
        Const nENT_START As Integer = 7
        Const nEXIT_START As Integer = 8
        Const nMUTE_LENGTH As Integer = 9
        Const nARM_1 As Integer = 10
        Const nARM_MAX As Integer = 11
        Const nPANEL As Integer = 12
        'Dim sFile As String = String.Empty
        'Try
        '    Const sCSV_EXT As String = "csv"
        '    Dim o As New OpenFileDialog
        '    With o
        '        .Filter = gcsRM.GetString("csSAVE_CSV_CDFILTER")
        '        .Title = gcsRM.GetString("csOPEN_FILE_DLG_CAP")
        '        .AddExtension = True
        '        .CheckPathExists = True
        '        .DefaultExt = "." & sCSV_EXT
        '        If .ShowDialog() = System.Windows.Forms.DialogResult.OK Then
        '            sFile = .FileName
        '        End If
        '    End With
        'Catch ex As Exception

        'End Try
        ' Open the file to read from.
        'Dim sr As System.IO.StreamReader = Nothing
        Try
            'process the file
            'If sFile <> String.Empty Then
            ' Open the file to read from.
            Dim sTitleReq As String = gpsRM.GetString("psSCREENCAPTION")
            Dim sTableStart(3) As String
            sTableStart(0) = gpsRM.GetString("psTAB1CAP", DisplayCulture)
            sTableStart(1) = gpsRM.GetString("psTAB2CAP", DisplayCulture)
            sTableStart(2) = gpsRM.GetString("psTAB3CAP", DisplayCulture)
            sTableStart(3) = gpsRM.GetString("psTAB5CAP", DisplayCulture)
            Dim sHeader As String = Nothing
            Dim oDT As DataTable = Nothing
            ImportExport.GetDTFromCSV(sTitleReq, sTableStart, sHeader, oDT)
            If sHeader IsNot Nothing AndAlso oDT IsNot Nothing Then
                Dim nStep As Integer = 0
                Dim nTab As Integer = 0
                If InStr(sHeader, gpsRM.GetString("psTAB1CAP", DisplayCulture)) > 0 Then
                    nStep = 1
                    nTab = 1
                ElseIf InStr(sHeader, gpsRM.GetString("psTAB2CAP", DisplayCulture)) > 0 Then
                    nStep = 1
                    nTab = 2
                ElseIf InStr(sHeader, gpsRM.GetString("psTAB3CAP", DisplayCulture)) > 0 Then
                    nStep = 1
                    nTab = 3
                ElseIf InStr(sHeader, gpsRM.GetString("psTAB5CAP", DisplayCulture)) > 0 Then
                    nStep = 1
                    nTab = 5
                End If
                If nStep = 0 Or nTab = 0 Then
                    Exit Sub
                End If

                'Open the stream and read it back.
                'sr = System.IO.File.OpenText(sFile)
                Dim nColumns() As Integer = Nothing
                Dim nMaxCol As Integer = 0
                For Each oRow As DataRow In oDT.Rows
                    Dim sTmp As String = String.Empty
                    For Each oItem As Object In oRow.ItemArray
                        sTmp = sTmp & vbTab & oItem.ToString
                    Next
                    sTmp = sTmp.Substring(1)
                    Dim sText() As String = Split(sTmp, vbTab)
                    nMaxCol = sText.GetUpperBound(0)
                    Select Case nStep
                        Case 1
                            'column headers
                            If InStr(sText(0), gpsRM.GetString("psITEM_CAP", DisplayCulture)) > 0 Or _
                            InStr(sText(0), gpsRM.GetString("psPANEL_CAP", DisplayCulture)) > 0 Then
                                ReDim nColumns(nMaxCol)
                                For Each nColNum As Integer In nColumns
                                    nColNum = -1
                                Next
                                'TODO >>> RJO 12/13/11 This will need debug
                                For nCol As Integer = 0 To nMaxCol
                                    Select Case sText(nCol)
                                        Case gpsRM.GetString("psITEM_CAP", DisplayCulture)
                                            nColumns(nCol) = nITEM
                                        Case lblPlantStyleCap.Text
                                            nColumns(nCol) = nPLANT_STYLE
                                        Case lblFanucStyleCap.Text
                                            nColumns(nCol) = nFANUC_STYLE
                                        Case lblPlantOptionCap.Text
                                            nColumns(nCol) = nPLANT_OPTION
                                        Case lblFanucOptionCap.Text
                                            nColumns(nCol) = nFANUC_OPTION
                                        Case lblStyleDescCap.Text
                                            nColumns(nCol) = nDESCRIPTION
                                        Case lblDescCap5.Text
                                            nColumns(nCol) = nDESCRIPTION
                                        Case lblEntStartCap.Text
                                            nColumns(nCol) = nENT_START
                                        Case lblExitStartCap.Text
                                            nColumns(nCol) = nEXIT_START
                                        Case lblMuteLenCap.Text
                                            nColumns(nCol) = nMUTE_LENGTH
                                        Case msRobotClbNames(0)
                                            nColumns(nCol) = nARM_1
                                            Dim nCol2 As Integer = nCol
                                            Dim nColMax As Integer = nCol
                                            Dim bDone As Boolean = False
                                            Do While nCol2 <= nMaxCol And bDone = False
                                                bDone = True
                                                For Each sName As String In msRobotClbNames
                                                    If sName = sText(nCol2) Then
                                                        bDone = False
                                                        nColMax = nCol2
                                                    End If
                                                Next
                                                nCol2 = nCol2 + 1
                                            Loop
                                            nColumns(nColMax) = nARM_MAX
                                        Case lblPanelCap.Text
                                            nColumns(nCol) = nPANEL
                                        Case Else
                                    End Select
                                Next
                                nStep = 2
                            End If
                        Case 2
                            Dim nItemNumber As Integer = -1
                            Dim nFanucStyle As Integer = -1
                            Dim sPlantStyle As String = String.Empty
                            Dim nFanucOption As Integer = -1
                            Dim sPlantOption As String = String.Empty
                            Dim sDescription As String = String.Empty
                            Dim nEntStart As Integer = -1
                            Dim nExitStart As Integer = -1
                            Dim nMuteLength As Integer = -1
                            Dim nArmEnable() As Integer = Nothing
                            For nCol As Integer = 0 To nMaxCol
                                Try
                                    Select Case nColumns(nCol)
                                        Case nITEM
                                            If sText(nCol).Length > 0 And IsNumeric(sText(nCol)) Then
                                                nItemNumber = CInt(sText(nCol))
                                            End If
                                        Case nPANEL
                                            If sText(nCol).Length > 0 And IsNumeric(sText(nCol)) Then
                                                nItemNumber = CInt(sText(nCol)) - 1
                                            End If
                                        Case nFANUC_STYLE
                                            If sText(nCol).Length > 0 And IsNumeric(sText(nCol)) Then
                                                nFanucStyle = CInt(sText(nCol))
                                            End If
                                        Case nFANUC_OPTION
                                            If sText(nCol).Length > 0 And IsNumeric(sText(nCol)) Then
                                                nFanucOption = CInt(sText(nCol))
                                            End If
                                        Case nPLANT_STYLE
                                            If mcolStyles.UseAscii Then
                                                If sText(nCol).Length > 0 Then
                                                    sPlantStyle = sText(nCol)
                                                End If
                                            Else
                                                If sText(nCol).Length > 0 And IsNumeric(sText(nCol)) Then
                                                    sPlantStyle = sText(nCol)
                                                End If
                                            End If
                                        Case nPLANT_OPTION
                                            If mcolStyles.UseAscii Then
                                                If sText(nCol).Length > 0 Then
                                                    sPlantOption = sText(nCol)
                                                End If
                                            Else
                                                If sText(nCol).Length > 0 And IsNumeric(sText(nCol)) Then
                                                    sPlantOption = sText(nCol)
                                                End If
                                            End If
                                        Case nDESCRIPTION
                                            If sText(nCol).Length > 0 Then
                                                sDescription = sText(nCol)
                                            End If
                                        Case nENT_START
                                            If sText(nCol).Length > 0 And IsNumeric(sText(nCol)) Then
                                                nEntStart = CInt(sText(nCol))
                                            End If
                                        Case nEXIT_START
                                            If sText(nCol).Length > 0 And IsNumeric(sText(nCol)) Then
                                                nExitStart = CInt(sText(nCol))
                                            End If
                                        Case nMUTE_LENGTH
                                            If sText(nCol).Length > 0 And IsNumeric(sText(nCol)) Then
                                                nMuteLength = CInt(sText(nCol))
                                            End If
                                        Case nARM_1
                                            ReDim nArmEnable(mcolArms.Count - 1)
                                            For nTmp As Integer = 0 To (mcolArms.Count - 1)
                                                nArmEnable(nTmp) = -1
                                            Next
                                            Dim bDone As Boolean = False
                                            Dim nArm As Integer = 0
                                            Do While (nArm < mcolArms.Count) And ((nCol + nArm) <= nMaxCol) And bDone = False
                                                If sText(nCol + nArm).Trim = "X" Then
                                                    nArmEnable(nArm) = 1
                                                ElseIf sText(nCol + nArm).Trim = "." Then
                                                    nArmEnable(nArm) = 0
                                                ElseIf sText(nCol + nArm).Trim = "-" Then
                                                    nArmEnable(nArm) = 0
                                                End If
                                                If nColumns(nCol) = nARM_MAX Then
                                                    bDone = True
                                                End If
                                                nArm = nArm + 1
                                            Loop
                                        Case Else
                                            'ignore unknown columns
                                    End Select
                                Catch ex As Exception
                                End Try
                            Next
                            Try
                                Select Case nTab
                                    Case 1
                                        If nItemNumber > 0 And nItemNumber <= mcolStyles.Count Then
                                            Dim oStyle As clsSysStyle = mcolStyles.Item(nItemNumber - 1)
                                            If nFanucStyle <> -1 Then
                                                oStyle.FanucNumber.Value = nFanucStyle
                                            End If
                                            If sPlantStyle <> String.Empty Then
                                                oStyle.PlantNumber.Text = (sPlantStyle)
                                            End If
                                            If sDescription <> String.Empty Then
                                                oStyle.Description.Text = (sDescription)
                                            End If
                                            If nArmEnable IsNot Nothing Then
                                                Dim nValue As Integer
                                                If mcolZones.ActiveZone.DegradeStyleRbtsReq Then
                                                    nValue = oStyle.DegradeRobotsRequired(cboRobot.SelectedIndex).Value
                                                Else
                                                    nValue = oStyle.RobotsRequired.Value
                                                End If
                                                Dim nBitOffset As Integer = mcolZones.ActiveZone.RobotsRequiredStartingBit
                                                Dim nArmMax As Integer = mcolArms.Count - 1
                                                If nArmMax > nArmEnable.GetUpperBound(0) Then
                                                    nArmMax = nArmEnable.GetUpperBound(0)
                                                End If
                                                For nArmIdx As Integer = 0 To nArmMax
                                                    Dim nBot As Integer = CType(2 ^ (mcolArms.Item(nArmIdx).RobotNumber - 1 + nBitOffset), Integer)
                                                    If (nValue And nBot) = nBot Then 'Currently Enbaled
                                                        If nArmEnable(nArmIdx) = 0 Then ' import disable
                                                            nValue = nValue - nBot
                                                        End If
                                                    Else 'Currently Disabled
                                                        If nArmEnable(nArmIdx) = 1 Then ' import enable
                                                            nValue = nValue + nBot
                                                        End If
                                                    End If
                                                Next
                                                If mcolZones.ActiveZone.DegradeStyleRbtsReq Then
                                                    oStyle.DegradeRobotsRequired(cboRobot.SelectedIndex).Value = nValue
                                                Else
                                                    oStyle.RobotsRequired.Value = nValue
                                                End If
                                            End If
                                                EditsMade = True
                                            End If
                                    Case 2
                                            If nItemNumber > 0 And nItemNumber <= mcolOptions.Count Then
                                                Dim oOption As clsSysOption = mcolOptions.Item(nItemNumber - 1)
                                                If nFanucOption <> -1 Then
                                                    oOption.FanucNumber.Value = nFanucOption
                                                End If
                                                If sPlantOption <> String.Empty Then
                                                    oOption.PlantNumber.Text = (sPlantOption)
                                                End If
                                                If sDescription <> String.Empty Then
                                                    oOption.Description.Text = (sDescription)
                                                End If
                                                If nArmEnable IsNot Nothing Then
                                                    Dim nValue As Integer = oOption.RobotsRequired.Value
                                                    Dim nBitOffset As Integer = mcolZones.ActiveZone.RobotsRequiredStartingBit
                                                    Dim nArmMax As Integer = mcolArms.Count - 1
                                                    If nArmMax > nArmEnable.GetUpperBound(0) Then
                                                        nArmMax = nArmEnable.GetUpperBound(0)
                                                    End If
                                                    For nArmIdx As Integer = 0 To nArmMax
                                                        Dim nBot As Integer = CType(2 ^ (mcolArms.Item(nArmIdx).RobotNumber - 1 + nBitOffset), Integer)
                                                        If (nValue And nBot) = nBot Then 'Currently Enbaled
                                                            If nArmEnable(nArmIdx) = 0 Then ' import disable
                                                                nValue = nValue - nBot
                                                            End If
                                                        Else 'Currently Disabled
                                                            If nArmEnable(nArmIdx) = 1 Then ' import enable
                                                                nValue = nValue + nBot
                                                            End If
                                                        End If
                                                    Next
                                                    oOption.RobotsRequired.Value = nValue
                                                End If
                                                EditsMade = True
                                            End If
                                    Case 3
                                            If msCurrentTabName = "TabPage3" And cboStyle.SelectedIndex >= 0 And _
                                            ((cboRobot.SelectedIndex >= 0) Or (mcolZones.ActiveZone.DegradeRepair = False)) Then
                                                Dim nStyle As Integer = cboStyle.SelectedIndex
                                                Dim nDegrade As Integer = cboRobot.SelectedIndex
                                                If mcolZones.ActiveZone.DegradeRepair = False Then
                                                    nDegrade = 0
                                                End If
                                                If sDescription <> String.Empty Then
                                                    mcolStyles(nStyle).RepairPanelDescription(nItemNumber).Text = (sDescription)
                                                End If
                                                If nArmEnable IsNot Nothing Then
                                                    Dim nValue As Integer = mcolStyles(nStyle).PlantSpecific.DegradeRepairPanels.Item(nDegrade).Item(nItemNumber).RobotsRequired
                                                    Dim nBitOffset As Integer = mcolZones.ActiveZone.RobotsRequiredStartingBit
                                                    Dim nArmMax As Integer = mcolArms.Count - 1
                                                    If nArmMax > nArmEnable.GetUpperBound(0) Then
                                                        nArmMax = nArmEnable.GetUpperBound(0)
                                                    End If
                                                    For nArmIdx As Integer = 0 To nArmMax
                                                        Dim nBot As Integer = CType(2 ^ (mcolArms.Item(nArmIdx).RobotNumber - 1 + nBitOffset), Integer)
                                                        If (nValue And nBot) = nBot Then 'Currently Enbaled
                                                            If nArmEnable(nArmIdx) = 0 Then ' import disable
                                                                nValue = nValue - nBot
                                                            End If
                                                        Else 'Currently Disabled
                                                            If nArmEnable(nArmIdx) = 1 Then ' import enable
                                                                nValue = nValue + nBot
                                                            End If
                                                        End If
                                                    Next
                                                    mcolStyles(nStyle).PlantSpecific.DegradeRepairPanels.Item(nDegrade).Item(nItemNumber).RobotsRequired = nValue
                                                End If
                                                EditsMade = True
                                            End If
                                    Case 5
                                            If nItemNumber > 0 And nItemNumber <= mcolStyles.Count Then
                                                Dim oStyle As clsSysStyle = mcolStyles.Item(nItemNumber - 1)
                                                If nEntStart >= 0 Then
                                                    oStyle.EntranceMuteStartCount.Value = nEntStart
                                                End If
                                                If nExitStart >= 0 Then
                                                    oStyle.ExitMuteStartCount.Value = nExitStart
                                                End If
                                                If nMuteLength >= 0 Then
                                                    oStyle.MuteLength.Value = nMuteLength
                                                End If
                                                EditsMade = True
                                            End If
                                End Select

                            Catch ex As Exception

                            End Try
                    End Select
                Next
                'sr.Close()

            End If
            subShowNewPage(True)
        Catch ex As Exception

        Finally
            'If (sr IsNot Nothing) Then
            '    sr.Close()
            'End If

        End Try
    End Sub


    Private Sub mnuPrintFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintFile.Click
        '********************************************************************************************
        'Description:  print the table
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
        'Description:  print the table
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

            If mcolArms Is Nothing = False Then
                For i As Integer = mcolArms.Count - 1 To 0 Step -1
                    mcolArms.Remove(mcolArms(i))
                Next
                mcolArms = Nothing
            End If 'mcolArms Is Nothing = False

            If mcolControllers Is Nothing = False Then
                For i As Integer = mcolControllers.Count - 1 To 0 Step -1
                    mcolControllers.Remove(mcolControllers(i))
                Next
                mcolControllers = Nothing
            End If 'mcolControllers Is Nothing = False

            GC.Collect()
            Application.DoEvents()
            mPWRobotCommon.UrbanRenewal()
            Application.DoEvents()
            subInitializeForm()
        Else
            mbRemoteZone = bTmp
        End If 'SetRemoteServer()

        mbEventBlocker = False

    End Sub

    Private Sub oIPC_NewMessage(ByVal Schema As String, ByVal DS As System.Data.DataSet) Handles oIPC.NewMessage
        '********************************************************************************************
        'Description:  A new message has been received from another Paintworks Application
        '
        'Parameters: DS - PWUser Dataset, ProcessName Paintworks screen process name
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If Me.InvokeRequired Then
            Dim dNewMessage As New NewMessage_Callback(AddressOf oIPC_NewMessage)
            Me.BeginInvoke(dNewMessage, New Object() {Schema, DS})
        Else
            Dim DR As DataRow = DS.Tables(Paintworks_IPC.clsInterProcessComm.sTABLE).Rows(0)

            Select Case Schema.ToLower
                Case oIPC.CONTROL_MSG_SCHEMA.ToLower
                    'TODO - Handle language change requests here.
                    'Call subDoControlAction(DR)
                Case oIPC.PASSWORD_MSG_SCHEMA.ToLower
                    Call moPassword.ProcessPasswordMessage(DR)
                Case Else
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: oIPC_NewMessage", _
                                           "Unrecognized schema [" & Schema & "].")
            End Select
        End If 'Me.InvokeRequired

    End Sub

    Private Sub pnlStyles_Scroll(ByVal sender As Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles pnlStyles.Scroll
        '********************************************************************************************
        'Description:  Move the Style Data Column Captions when the page is scrolled horizontally. 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case e.ScrollOrientation
            Case ScrollOrientation.HorizontalScroll
                Dim nDiff As Integer = e.OldValue - e.NewValue

                lblPlantStyleCap.Left += nDiff
                lblStyleOptCap.Left += nDiff 'RJO 12/13/11
                lblFanucStyleCap.Left += nDiff
                lblStyleDescCap.Left += nDiff
                lblStyleRobotsReqCap.Left += nDiff
                lbltwocoats.Left += nDiff  'RJO 12/13/11
        End Select

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

        msCurrentTabName = e.TabPage.Name
        Select Case msCurrentTabName
            Case "TabPage1"
                'Add any special init code here
            Case "TabPage2"
                'Add any special init code here
            Case "TabPage3"
                subLoadData(eLoadModes.LoadFromDB)
            Case "TabPage4"
                'load style combo
                subLoadData(eLoadModes.LoadFromDB)
            Case "TabPage5"
                'Add any special init code here
            Case "TabPage6"
                'load style combo
                subLoadData(eLoadModes.LoadFromDB)
        End Select
        ' added to build the page controls first then load from PLC
        frmMain_Layout()

        'SA
        If mcolZones.StandAlone Then
            subLoadData(eLoadModes.LoadFromDB)
        Else
            subLoadData(eLoadModes.LoadFromPLC)
        End If

    End Sub
    Private Sub tabMain_Selecting(ByVal sender As Object, _
                ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles tabMain.Selecting
        '********************************************************************************************
        'Description:  a tab page is being selected
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
                If DataLoaded Then
                    bPrintdoc(True)
                End If

            Case "btnUndo"
                Select Case Privilege
                    Case ePrivilege.None
                        'logout occured while screen open
                        subEnableControls(False)
                    Case Else
                        subUndoData()
                End Select
            Case "btnCopy"
                Select Case Privilege
                    Case ePrivilege.None
                        'logout occured while screen open
                        subEnableControls(False)
                    Case Else
                        subCopyButtonPressed()
                End Select

            Case "btnChangeLog"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                subShowChangeLog(CType(eChangeMnu.Hours24, Integer))
            Case btnUtilities.Name
                btnUtilities.ShowDropDown()
        End Select

    End Sub
    Private Sub subCopyButtonPressed()
        '********************************************************************************************
        'Description: Copy button pressed check for edits first
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'make sure edits are saved
        If EditsMade Then
            Dim bSaveCheck As Boolean = (bAskForSave() = False)
            If bSaveCheck = True Then '<< CANCEL OR NO SELECTED
                Exit Sub
            End If 'bSaveCheck = True
        End If 'EditsMade

        'launch copy screen

        Dim frmCopy As New frmCopy

        Try
            With frmCopy

                If mcolZones.PaintShopComputer Then
                    '   force reload from zoneinfo table on this computer
                    .DatabasePath = String.Empty
                Else
                    .DatabasePath = mcolZones.DatabasePath
                End If

                .ScreenName = msSCREEN_NAME
                .UseRobot = False
                .ParameterType = Job_Setup.frmCopy.eParamType.Degrade
                .ParamName = gpsRM.GetString("psDEGRADE", DisplayCulture)
                '.SubParamName = gpsRM.GetString("psDEGRADE", DisplayCulture)
                .UseSubParam = mcolZones.ActiveZone.DegradeRepair
                .UseParam = False
                .UseStyle = True
                .FancyCopyButtons = False
                .ShowDialog()
            End With

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            frmCopy = Nothing
        Finally
            subShowNewPage(False)
        End Try


    End Sub
#End Region

#Region " Combo box events "
    Private Sub cboParam_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                            Handles cboParam.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Parameter Combo Changed 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ''********************************************************************************************

        If cboParam.Text <> msOldParam Then
            subChangeParameter()
        End If

    End Sub

    Private Sub cbo_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                Handles cboStyle.SelectionChangeCommitted, cboRobot.SelectionChangeCommitted
        ''********************************************************************************************
        ''Description:  Style combo selection changed 
        ''
        ''Parameters: none
        ''Returns:    None
        ''
        ''Modification history:
        ''
        '' Date      By      Reason
        '' 09/08/11  MSW     combine cboRobot with cboStyle, check for valied data before load
        '' 10/30/13  BTK     Degrade Style Robots Required
        ''********************************************************************************************        
        'Dim oTab As TabPage = tabMain.SelectedTab
        'Select Case oTab.Name
        '    Case "TabPage1"
        '        'styles
        '        '10/30/13 BTK Degrade Style Robots Required
        '        If mcolZones.ActiveZone.DegradeStyleRbtsReq Then
        '            If cboRobot.SelectedIndex < 0 Then
        '                Return
        '            End If
        '        End If
        '    Case "TabPage2"
        '        'Options
        '    Case "TabPage3"
        '        'Repairs
        '        If cboStyle.SelectedIndex < 0 Then
        '            Return
        '        End If
        '        If mcolZones.ActiveZone.DegradeRepair Then
        '            If cboRobot.SelectedIndex < 0 Then
        '                Return
        '            End If
        '        End If
        '    Case "TabPage4"
        '        'Degrade
        '    Case "TabPage5"
        '        'Intrusion
        '    Case "TabPage6"
        '        'StyleID
        'End Select
        'subShowNewPage(True)

    End Sub

    Private Sub subSelectionsChanged()
        '********************************************************************************************
        'Description:  Combined selection changed
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************        

        If EditsMade Then
            Dim bSaveCheck As Boolean = (bAskForSave() = False)
            'If bSaveCheck = True Then '<< CANCEL OR NO SELECTED
            '    DirectCast(sender, ComboBox).SelectedIndex = mcolRepairs.StyleIndex - 1
            '    Exit Sub
            'End If 'bSaveCheck = True
        End If 'EditsMade

        'subLoadData(eLoadModes.LoadFromPLC)

        subShowNewPage(True)

    End Sub
    Private Sub cboZone_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboZone.SelectedValueChanged

    End Sub
    Private Sub cboZone_SelectionChangeCommitted1(ByVal sender As Object, ByVal e As System.EventArgs) _
                                            Handles cboZone.SelectionChangeCommitted
        '********************************************************************************************
        'Description:  Zone Combo Changed 
        '
        'Parameters: none
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
    Private Sub cboController_DropDown(ByVal sender As Object, ByVal e As System.EventArgs) _
                                    Handles cboController.DropDown
        '********************************************************************************************
        'Description:  degrade is saved by controller
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If EditsMade Then
            If bAskForSave() Then
                subSaveData()
            Else
                EditsMade = False
            End If
        End If

    End Sub
    Private Sub DegradeSelectionMade(ByVal sender As Object, ByVal e As System.EventArgs) _
                Handles cboPlantStyle.SelectionChangeCommitted, cboController.SelectionChangeCommitted
        '********************************************************************************************
        'Description:  new degrade selection made on degrade tab
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim nC As Integer = nGetDegradeControllerNum()
        If nC = -1 Then Exit Sub

        Dim o As clsSysStyle = oGetDegradeStyle()
        If o Is Nothing Then Exit Sub


        Call subLoadDegradeTabData(nC, o)

        msOldController = cboController.Text

    End Sub

    Private Sub StyleIDSelectionMade(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles cboStyleID.SelectionChangeCommitted
        '********************************************************************************************
        'Description:  new style ID selection made on Style ID tab
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'BTK used mbReLoad flag to prevent changing the backgrounds to red when loading data.
        'Some work needs to be done to fix this problem.
        Dim bSave As Boolean = True
        If mnStyleIDIndex > -1 AndAlso DataLoaded AndAlso EditsMade Then
            bSave = bAskForSave()
        End If
        If bSave Then
            mbReLoad = True
            mnStyleIDIndex = cboStyleID.SelectedIndex
            subLoadData(eLoadModes.LoadFromPLC)
            subShowNewPage(True)
            mbReLoad = False
        Else
            cboStyleID.SelectedIndex = mnStyleIDIndex
        End If

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
                Privilege = ePrivilege.Edit
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

#Region " Controller Events "

    Private Sub mcolControllers_BumpProgress() Handles mcolControllers.BumpProgress
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If (Progress > 0) And (Progress < 95) Then Progress += 5

    End Sub
    Private Sub colControllers_ConnectionStatusChange(ByVal Controller As clsController) _
                                          Handles mcolControllers.ConnectionStatusChange
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'this is needed with old RCM object raising events
        'and can hopefully be removed when I learn how to program
        'Control.CheckForIllegalCrossThreadCalls = False

        'Trace.WriteLine("frmmain connection status change event - " & Controller.Name & " " & _
        '                        Controller.RCMConnectStatus.ToString)


        'Check for call from the robot object thread
        If Me.stsStatus.InvokeRequired Then
            Dim dCntrStat As New ControllerStatusChange(AddressOf colControllers_ConnectionStatusChange)
            Me.BeginInvoke(dCntrStat, New Object() {Controller})
        Else
            Me.stsStatus.Refresh()
        End If
        'Control.CheckForIllegalCrossThreadCalls = True

    End Sub

#End Region

#Region " PLCComm Events "

    Private Sub moPLC_ModuleError(ByVal nErrNum As Integer, ByVal sErrDesc As String, _
                    ByVal sModule As String, ByVal AdditionalInfo As String) Handles moPLC.ModuleError
        '********************************************************************************************
        'Description:  PLC Read/Write error has occurred in the PLCComm class
        '
        'Parameters: Error parameters/description
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        mbPLCFail = True
        MessageBox.Show(sErrDesc & vbCrLf & AdditionalInfo, _
                    gcsRM.GetString("csPLC_COMM_ERROR", DisplayCulture) & ":" & _
                    Err.Number.ToString, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

    End Sub

#End Region

    Private Sub TwoCoats_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  Updates value in ostyle based on checkbox state
        '
        'Parameters: checkbox
        'Returns:
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/22/11  JBW     Created for two coat checkboxen
        '********************************************************************************************
        Dim tempcheckbox As CheckBox = DirectCast(sender, CheckBox)
        'parse sender.name and get last 3 characters, convert to integer
        Dim tempnum As Integer = CType(tempcheckbox.Name.Substring(10, 3), Integer)
        Dim ostyle As clsSysStyle = mcolStyles.Item(tempnum - 1)
        ostyle.TwoCoatStyle.Value = tempcheckbox.Checked
        EditsMade = True
    End Sub

    Private Sub cboStyle_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboStyle.SelectedIndexChanged

    End Sub

    Private Sub cboRobot_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboRobot.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Style combo selection changed 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/08/11  MSW     combine cboRobot with cboStyle, check for valied data before load
        ' 10/30/13  BTK     Degrade Style Robots Required
        '********************************************************************************************        
        Dim oTab As TabPage = tabMain.SelectedTab
        Select Case oTab.Name
            Case "TabPage1"
                'styles
                '10/30/13 BTK Degrade Style Robots Required
                If mcolZones.ActiveZone.DegradeStyleRbtsReq Then
                    If cboRobot.SelectedIndex < 0 Then
                        Return
                    End If
                End If
            Case "TabPage2"
                'Options
            Case "TabPage3"
                'Repairs
                If cboStyle.SelectedIndex < 0 Then
                    Return
                End If
                If mcolZones.ActiveZone.DegradeRepair Then
                    If cboRobot.SelectedIndex < 0 Then
                        Return
                    End If
                End If
            Case "TabPage4"
                'Degrade
            Case "TabPage5"
                'Intrusion
            Case "TabPage6"
                'StyleID
        End Select
        subShowNewPage(True)
    End Sub

    Private Sub ftbExitStart001_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ftbExitStart001.TextChanged

    End Sub
End Class
Module mJobSetupCommonRtns
    Friend Function LoadCopyScreenSubParamBox(ByRef oRobot As clsArm, _
                        ByRef clbBox As CheckedListBox, ByVal ParamName As String, ByVal Style As Integer) As Boolean
        '********************************************************************************************
        'Description:  load degrade box from style copy screen
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/16/13  MSW     Added (Unused) Style  parameter for compatibility with frmCopy v4.01.01.03
        '********************************************************************************************
        clbBox.Items.Clear()
        clbBox.BeginUpdate()

        For nItem As Integer = 0 To frmMain.cboRobot.Items.Count - 1
            clbBox.Items.Add(frmMain.cboRobot.Items(nItem))
        Next
        ParamName = gpsRM.GetString("psDEGRADE", frmMain.DisplayCulture)
        clbBox.EndUpdate()

        Return True

    End Function
    Friend Function DoCopy(ByRef colZoneFrom As clsZones, ByRef colZoneTo As clsZones, _
                            ByRef colRobotFrom As clsArms, ByRef colRobotTo As clsArms, _
                            ByRef colParamFrom As Collection(Of String), _
                            ByRef colParamTo As Collection(Of String), _
                            ByRef colSubParamFrom As Collection(Of String), _
                            ByRef colSubParamTo As Collection(Of String), _
                            ByVal CopyType As eCopyType, ByRef oCopy As frmCopy, _
                            Optional ByRef colStyleFrom As Collection(Of String) = Nothing, _
                            Optional ByRef colStyleTo As Collection(Of String) = Nothing) As Boolean
        '********************************************************************************************
        'Description:  load degrade box from style copy screen
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            Dim oZone As clsZone = frmMain.mcolZones.ActiveZone
            Dim bMatchStyles As Boolean = (colStyleFrom.Count > 1) And oZone.DegradeRepair
            Dim bMatchDegrades As Boolean = (colSubParamFrom.Count > 1) And oZone.DegradeRepair
            Dim nIndex As Integer = 0
            Dim nDegradeFrom As Integer = -1
            Dim sStyleSource As String = String.Empty
            Dim sDegradeSource As String = String.Empty
            Dim sStyleDest As String = String.Empty
            Dim sDegradeDest As String = String.Empty
            Dim oStyleSource As clsSysStyle = Nothing
            If Not (bMatchStyles) Then
                '1 style source
                For Each oStyleTmp As clsSysStyle In frmMain.mcolStyles
                    If oStyleTmp.FanucNumber.Value.ToString = colStyleFrom(0).ToString Then
                        oStyleSource = DirectCast(oStyleTmp, clsSysStyle)
                        sStyleSource = oStyleSource.DisplayName
                    End If
                Next
                If (oStyleSource Is Nothing) Then
                    'post an error message
                    Debug.Assert(False)
                End If
            End If

            If oZone.DegradeRepair AndAlso Not (bMatchDegrades) Then
                '1 degrade source
                nIndex = 0
                Do While (nDegradeFrom = -1) And (nIndex < frmMain.cboRobot.Items.Count)
                    If colSubParamFrom(0).ToString.Trim = frmMain.cboRobot.Items(nIndex).ToString.Trim Then
                        nDegradeFrom = nIndex
                        sDegradeSource = frmMain.cboRobot.Items(nIndex).ToString
                    Else
                        nIndex = nIndex + 1
                    End If
                Loop
                If (nDegradeFrom = -1) Then
                    'post an error message
                    Debug.Assert(False)
                End If
            End If

            For Each sStyle As String In colStyleTo
                Dim oStyleDest As clsSysStyle = Nothing
                For Each oStyleTmp As clsSysStyle In frmMain.mcolStyles
                    If oStyleTmp.FanucNumber.Value.ToString = sStyle Then
                        oStyleDest = oStyleTmp
                        sStyleDest = oStyleDest.DisplayName
                    End If
                Next
                If (oStyleDest Is Nothing) Then
                    'post an error message
                    Debug.Assert(False)
                End If
                If (oStyleDest IsNot Nothing) Then
                    If oZone.DegradeRepair Then
                        If bMatchStyles Then
                            oStyleSource = oStyleDest
                            sStyleSource = sStyleDest
                        End If
                        'Debug.Print("Style: " & sStyleSource & " -- " & sStyleDest)

                        For Each sDegrade As String In colSubParamTo
                            '1 degrade source
                            nIndex = 0
                            Dim nDegradeTo As Integer = -1
                            Do While (nDegradeTo = -1) And (nIndex < frmMain.cboRobot.Items.Count)
                                If sDegrade.Trim = frmMain.cboRobot.Items(nIndex).ToString.Trim Then
                                    nDegradeTo = nIndex
                                    sDegradeDest = frmMain.cboRobot.Items(nIndex).ToString
                                Else
                                    nIndex = nIndex + 1
                                End If
                            Loop
                            If (nDegradeTo = -1) Then
                                'post an error message
                                Debug.Assert(False)
                            End If
                            If bMatchDegrades Then
                                nDegradeFrom = nDegradeTo
                                sDegradeSource = sDegradeDest
                            End If
                            'Debug.Print("Degrade: " & nDegradeFrom & " -- " & nDegradeTo)

                            'Dim sTmp1 As String = String.Empty
                            'Dim sTmp2 As String = String.Empty
                            For nPanel As Integer = 0 To oZone.RepairPanels - 1
                                oStyleDest.PlantSpecific.DegradeRepairPanels.Item(nDegradeTo).Item(nPanel).RobotsRequired = _
                                    oStyleSource.PlantSpecific.DegradeRepairPanels.Item(nDegradeFrom).Item(nPanel).RobotsRequired
                                oStyleDest.RepairPanelDescription(nPanel + 1).Text = oStyleSource.RepairPanelDescription(nPanel + 1).Text
                                'sTmp1 = sTmp1 & " " & oStyleSource.PlantSpecific.DegradeRepairPanels.Item(nDegradeFrom).Item(nPanel).RobotsRequired.ToString()
                                'sTmp2 = sTmp2 & " " & oStyleDest.PlantSpecific.DegradeRepairPanels.Item(nDegradeTo).Item(nPanel).RobotsRequired.ToString()
                            Next
                            'Debug.Print(sTmp1)
                            'Debug.Print(sTmp2)
                            'Debug.Print("Next")
                            Dim oTmp1 As String = gpsRM.GetString("psROB_REQ_FOR_PANEL") & " " & gcsRM.GetString("csCOPIED") & " " & gpsRM.GetString("psSTYLE") & " " & sStyleSource & " " & _
                                            gpsRM.GetString("psDEGRADE") & " " & sDegradeSource & " " & gcsRM.GetString("csTO") & " " & _
                                            gpsRM.GetString("psSTYLE") & " " & sStyleDest & "  " & _
                                            gpsRM.GetString("psDEGRADE") & " " & sDegradeDest
                            oCopy.Status = oTmp1
                            frmMain.subUpdateChangeLogForCopy(oZone, oTmp1, oZone.Name)

                        Next
                    Else
                        For nPanel As Integer = 0 To oZone.RepairPanels - 1
                            oStyleDest.PlantSpecific.DegradeRepairPanels.Item(0).Item(nPanel).RobotsRequired = _
                                oStyleSource.PlantSpecific.DegradeRepairPanels.Item(0).Item(nPanel).RobotsRequired
                        Next
                        Dim oTmp1 As String = gpsRM.GetString("psROB_REQ_FOR_PANEL") & " " & gcsRM.GetString("csCOPIED") & " " & gpsRM.GetString("psSTYLE") & " " & sStyleSource & " " & _
                                        gcsRM.GetString("csTO") & " " & _
                                        gpsRM.GetString("psSTYLE") & " " & sStyleDest
                        oCopy.Status = oTmp1
                        frmMain.subUpdateChangeLogForCopy(oZone, oTmp1, oZone.Name)
                    End If
                End If
            Next
            frmMain.bSaveRepairsToPLC(False)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

End Module