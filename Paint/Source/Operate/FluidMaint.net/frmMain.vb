' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2006 - 2009
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: frmMain
'
' Description: Fluid maint screen
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Speedy
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                      Version
'    05/29/09   MSW     PW4 development                                               4.00.00.00
'    04/21/11   RJO     Added feature to disable certain manual CC cycles based       4.00.00.01
'                       on line state for WB versabells.
'    09/14/11   MSW     Assemble a standard version of everything                     4.01.00.00
'    12/02/11   MSW     Add DMONCfg reference                                         4.01.01.00
'    01/18/12   MSW     Clean up old printsettings object                             4.01.01.01
'    01/24/12   MSW     Applicator Updates                                            4.01.01.02
'    02/15/12   MSW     Force 32 bit build                                           4.01.01.03
'    03/22/12   RJO     Modified for .NET Password and IPC                            4.01.02.00
'    03/28/12   MSW     Modified for .NET Scattered Access                            4.01.03.00
'    04/19/12   MSW     Added handling to separate single and dual shaping air        4.01.03.01
'    04/27/12   MSW     Improve WB cal status display                                 4.01.03.02
'    06/07/12   MSW     subUpdateScatteredAccess - fix cal status for accuair,        4.01.03.03
'                       add waitcursor for change log
'    09/24/12   BTK     Added missing eColorChangeType cases to Applicator/CC type    4.01.03.04
'                       specific labels section of subUpdateScatteredAccess 
'    10/10/12   MSW     subUpdateScatteredAccess - fix inletpressure2 display         4.01.03.05
'    10/24/12   RJO     Added StartupModule to project to prevent multiple instances  4.01.03.06
'    11/01/12   RJO     Moved screen close code to frmMain_FormClosing event to make  4.01.03.07
'                       sure the screen cleans up when the user exits the screen
'                       using the "X" button in the control box.
'    04/16/13   MSW     Add Canadian language files                                   4.01.05.00
'                       Merge Honda Canada back to standard
'                       Standalone Changelog
'    06/21/13   RJO     Modifications to gpbViewAll setup and update to handle zones  4.01.05.01
'                       with disparate applicators better.
'    07/09/13   MSW     Update and standardize logos, PLCComm.Dll                     4.01.05.02
'    07/11/13   RJO     Bug fixes? to feature that disables certain manual CC cycles  4.01.05.03
'                       based on line state for WB versabells.
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up  4.01.05.04
'    09/30/13   MSW     Save screenshots as jpegs, PLC DLL                            4.01.06.00
'    10/11/13   MSW     add clsVersabell3WBCartoon                                    4.01.06.01
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call              4.01.07.00
'                       P1000/VB3 updates from KTP
'    07/16/14   RJO     Added support for 16 arms                                     4.01.07.01
'********************************************************************************************

Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Configuration.ConfigurationSettings
Imports Response = System.Windows.Forms.DialogResult
Imports Connstat = FRRobotNeighborhood.FRERNConnectionStatusConstants


Imports eZLink = FluidMaint.ePLCMANUAL_ZONE_LINK
Imports eRLink = FluidMaint.ePLCMANUAL_ROBOT_LINK
Imports clsPLCComm = PLCComm.clsPLCComm


' The delegates (function pointers) enable asynchronous calls from the password object.
Delegate Sub LoginCallBack()
Delegate Sub LogOutCallBack()
Delegate Sub ControllerStatusChange(ByVal Controller As clsController)
Delegate Sub NewMessage_Callback(ByVal Schema As String, ByVal DS As DataSet)


Friend Class frmMain

#Region " Declares "
    '******** Form Constants   **********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Public Const msSCREEN_NAME As String = "FluidMaint"   ' <-- For password area change log etc.
    Public Const msSCREEN_DUMP_NAME As String = "Operate_FluidMaint.jpg"
    Private Const mnMAININX As Integer = 100
    Private Const mnSCREENINX As Integer = 101
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    Friend Const mbUSEROBOTS As Boolean = True
    Private Const mnLASTROBOT As Integer = 10 ' most robots allowed by law
    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    Friend Shared gsChangeLogArea As String = msSCREEN_NAME
    Private msOldZone As String = String.Empty
    Private msOldApplicator As String = String.Empty
    Private meOldApplicator As eColorChangeType = eColorChangeType.NOT_SELECTED
    Private msOldRobot As String = String.Empty
    Private msOldColor As String = String.Empty
    Private mnOldRobotNum As Integer = 0   ' 0=no robot selected
    Private colZones As clsZones = Nothing
    Private WithEvents colControllers As clsControllers = Nothing
    Friend WithEvents colArms As clsArms = Nothing
    Friend WithEvents colApplicators As clsApplicators = Nothing
    Private mbOfflineMessage As Boolean() '1 pop-up at a time when controller goes offline
    Private mbSAMessage As Boolean() 'write SA status update to status bar
    Private mRobot As clsArm = Nothing
    Private colTextboxes As Collection(Of FocusedTextBox.FocusedTextBox) = _
                            New Collection(Of FocusedTextBox.FocusedTextBox)
    Private mbRemoteZone As Boolean = False
    Private WithEvents mPLC As clsPLCComm = Nothing
    Private msZonePLCData As String() = Nothing
    Private msOldZonePLCData As String() = Nothing
    Private msRobotPLCData As String() = Nothing
    Private mnManualStatus As Integer
    Private mbBlockItemCheckEvent As Boolean = False
    Private mnFASACalChannel As Integer
    Private mnDockPosition As Integer = 0 '0=Unknown
    Private masCCItems() As String 'RJO 04/21/11
    Private mlstDisabledCCItems As New List(Of String) 'RJO 04/21/11
    '******** End Form Variables    *****************************************************************

    '******** Scattered Access/Robot Communication****************************************************
    'MSW Moved Scattered access names and indexes into subChangeRobot instead of searching each time the timer ticks
    Private msControllerName As String = String.Empty
    Private msArmSelected As String = String.Empty
    Private meUnitLabelSelect As eParamID = eParamID.Time
    Private gSAConfig As clsScatteredAccessGlobal
    Private mnArmNumber As Integer = 0
    Private mnRobIdx As Integer = 0
    Private mnValve As Integer = -1
    Private Structure tSAIndex
        Dim Indexes() As Integer
    End Structure
    Private mtScatteredAccessIndexes As tSAIndex()
    Private bIndexScatteredAccess() As Boolean

    Private msSAData(1) As String  'Scattered access data 
    '******** Operations status  ************************************************************
    Private mnCalActive As Integer = 0
    Private mbCalActive As Boolean = False
    Private mnFlowTestActive As Integer = 0
    Private mbFlowTestActive As Boolean = False
    Private mbCCInProgress As Boolean = False
    Private Const mnMaxFlowTestTime As Integer = 300 'Maximum time for flow tests
    Private mbEngUnits(5) As Boolean
    Private mbBeakerMode As Boolean = False
    Private Enum eCalType
        AutoCal
        ScaleCal
        DQCal
        Dq2Cal
        DockCal
    End Enum
    Private mnGetPntCalStatus() As Integer
    Private msPntCalStatus() As String
    Private mnGetCalStatus() As Integer
    Private mnGetCalStatus2() As Integer
    Private mnLineState() As Integer 'RJO 04/21/11
    Private msCalStatus() As String
    Private msCalStatus2() As String
    Private mbShowDQCalActive() As Boolean
    'Private mbShowDQCalActive2() As Boolean
    Private mbShowCalActive() As Boolean
    Private mbShowCalActive2() As Boolean
    '******** Property Variables    *****************************************************************
    Private mbEditsMade As Boolean = False
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean = False
    Private mbBigZone As Boolean  = False 'Kludge for Honda basecoat zone with 12 painters 06/02/10 RJO
    Private mnProgress As Integer = 0
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    Private msCulture As String = "en-US" 'Default to english
    '******** End Property Variables    *************************************************************

    '******** This is the old pw3 password object interop  *****************************************
    'Friend WithEvents moPassword As PWPassword.PasswordObject 'RJO 03/22/12
    'Friend moPrivilege As PWPassword.cPrivilegeObject 'RJO 03/22/12

    Friend WithEvents moPassword As clsPWUser 'RJO 03/22/12
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm 'RJO 03/22/12

    'Enable asynchronous calls from the IPC class.
    Delegate Sub NewMessage_Callback(ByVal Schema As String, ByVal DS As DataSet) 'RJO 03/22/12

    '******** Flow diagran/detailed view selection   *****************************************
    Private Enum eViewSelect
        Toon
        Paint
        AA_BS
        FA_SA
        Estat
        CalStat
        DQCalStat
        PressureTest  '12/03/09 MSW Pressure Test Change
        DQCalStat2
        DockCalStat '01/27/10 RJO for Honda S-Unit
    End Enum
    Private meViewSelect As eViewSelect = eViewSelect.Toon
    '******** End Flow diagran/detailed view selection   *****************************************

    '******** Cartoon Variables   *******************************************************************
    'CC type class from ColorChange.vb This'll get assigned a type-specific class inherited from clsColorChangeCartoon
    Private WithEvents mCCToon As clsColorChangeCartoon = Nothing
    'This'll get assigned to a cctype specific user-control that has the actual drawing.
    Friend uctrlCartoon As UserControl = Nothing
    'Saved copy of the current valve states
    Private mnSharedValvesState As Integer
    Private mnGroupValvesState As Integer
    Private Const bCartoonDebugLoop As Boolean = False 'True - run the valve-click logic without the PLC for debug
    '******** End Cartoon Variables   ****************************************************************

    '12/03/09 MSW Pressure Test Change
    Private mePressureTestStep As mCCToonRoutines.ePressureTestStep = mCCToonRoutines.ePressureTestStep.Abort
    Private Const mnPressureTestFillTime As Integer = 5000
    Private Const mnPressureTestMeasureTime As Integer = 5000
    Private msPressureTestText() As String = Nothing
    Private msPressureTestSolv() As String = Nothing
    Private msPressureTestAir() As String = Nothing
    Private mbAsciiColor As Boolean = False
    Private mnAsciiCharacters As Integer = 0
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

            If Value = mPrivilegeGranted Then
                Exit Property
            End If
            mPrivilegeRequested = Value


            'handle logout
            If mPrivilegeRequested = ePrivilege.None Then
                If mPrivilegeGranted = ePrivilege.None Then
                    Exit Property
                End If
                mPrivilegeGranted = ePrivilege.None
                'Trick parameter here.  false is only for a busy screen.  It'll handle the password in the routine
                subEnableControls(True)
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
                'this is needed with old password object raising events
                'and can hopefully be removed when password is redone
                ' If msZonePLCData Is Nothing = False Then subUpdateZonelink(msZonePLCData)
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


            'If mPassword.CheckPassword(msSCREEN_NAME, Value, moPassword, moPrivilege) Then 'RJO 03/22/12
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
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
            End If

            'If msZonePLCData Is Nothing = False Then subUpdateZonelink(msZonePLCData)

            'this is needed with old password object raising events
            'and can hopefully be removed when password is redone
           'Control.CheckForIllegalCrossThreadCalls = True

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


#End Region

#Region " Screen management "

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '********************************************************************************************
        'Description: Perform some cleanup before closing this screen.
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        ' 11/01/12      RJO     Moved this code here from tlsMain_ItemClicked to make sure this happens
        '                       when the user clicks the close button and also when user closes the 
        '                       screen using the "X" button in the control box.
        '********************************************************************************************
        Dim sData(0) As String

        'NRU 161004 If you are closing fluid maint before you selected a zone, then there is nothing to clean up.  Exit.
        If IsNothing(mPLC) Then Exit Sub

        mPWCommon.SaveToChangeLog(colZones.ActiveZone)
        sData(0) = "0"
        With mPLC
            .TagName = "Z" & colZones.CurrentZoneNumber.ToString & "SelectRobotWord"
            .PLCData = sData
            Application.DoEvents()
            .TagName = "Z" & colZones.CurrentZoneNumber.ToString & "FluidMaintScreenActive"
            .PLCData = sData
        End With

        subSetPumpEnable(chkPump1.Checked, chkPump2.Checked)

    End Sub
    '********************************************************************************************
    'Screen Management
    '******************************************************************************************** 
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
        '********************************************************************************************
        Dim sKeyValue As String = String.Empty

        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode
                Case Keys.F1
                    'Help Screen request
                    sKeyValue = "btnHelp"
                    subLaunchHelp(gs_HELP_FLUIDMAINT, oIPC)
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
        Finally

        End Try

    End Sub
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
        ' 03/28/12  MSW     Modified for .NET Scattered Access                            4.01.03.00
        '********************************************************************************************

        Try

            'Shut down any scattered access connections that are updating
            Dim sMessage(1) As String
            sMessage(0) = msSCREEN_NAME & ";OFF"
            sMessage(1) = "ALL"
            oIPC.WriteControlMsg(gs_COM_ID_SA, sMessage)
            If Not (colControllers Is Nothing) Then
                'For Each Controller As clsController In colControllers
                '    If Controller.Robot.IsConnected Then
                '        If oScatteredAccess.IsSetUp Then
                '            oScatteredAccess.UpdateActive(Controller.Name) = False
                '        End If 'oSA.IsSetUp
                '    End If
                'Next
                For i As Integer = colControllers.Count - 1 To 0
                    colControllers.Remove(colControllers(i))
                Next
            End If

            If colArms Is Nothing Then Exit Sub
            For i As Integer = colArms.Count - 1 To 0
                colArms.Remove(colArms(i))
            Next

        Catch ex As Exception

        End Try

    End Sub
    Public Sub New()
        'for language support
        mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, msBASE_ASSEMBLY_LOCAL, _
                                            msROBOT_ASSEMBLY_LOCAL)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

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

        Dim nHeight As Integer = tscMain.ContentPanel.Height - (lblZone.Top * 2)
        If nHeight < 100 Then nHeight = 100
        Dim nWidth As Integer = tscMain.ContentPanel.Width - (gpbRobots.Left * 2)
        If nWidth < 100 Then nWidth = 100
        Const nMINSTATUS_LEFT As Integer = 500
        Const nMAXSTATUS_LEFT As Integer = 1050
        Dim nStatusLeft As Integer = nWidth - lstStatus.Width
        If nStatusLeft < nMINSTATUS_LEFT Then
            nStatusLeft = nMINSTATUS_LEFT
        End If
        If nStatusLeft > nMAXSTATUS_LEFT Then
            nStatusLeft = nMAXSTATUS_LEFT
        End If
        lstStatus.Left = nStatusLeft


    End Sub
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
        Privilege = ePrivilege.None
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
        ' 11/01/12  RJO     Moved "btnClose" code to frmMain_FormClosing event to make sure this 
        '                   happens when user closes the screen using the "X" button in the control 
        '                   box.
        '********************************************************************************************
        Select Case e.ClickedItem.Name ' case sensitive
            Case "btnClose"
                'RJO 11/01/12 - move to frmMain_FormClosing event
                ''Check to see if we need to save is performed in  bAskForSave
                ''mPWCommon.SaveToChangeLog(colZones.DatabasePath)
                'mPWCommon.SaveToChangeLog(colZones.ActiveZone)
                'Dim sData(0) As String
                'sData(0) = "0"
                'With mPLC
                '    .TagName = "Z" & colZones.CurrentZoneNumber.ToString & "SelectRobotWord"
                '    .PLCData = sData
                '    Application.DoEvents()
                '    .TagName = "Z" & colZones.CurrentZoneNumber.ToString & "DQCalScreenActive"
                '    .PLCData = sData
                'End With

                'subSetPumpEnable(chkPump1.Checked, chkPump2.Checked)
                Me.Close()
                'End
            Case "btnStatus"
                lstStatus.Visible = Not lstStatus.Visible

            Case "btnChangeLog"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                subShowChangeLog(CType(eChangeMnu.Hours24, Integer))

            Case "btnDevice"
                'is hotlink started?
                If msZonePLCData Is Nothing Then
                    'set up plc
                    mManFlowCommon.InitPlc(mPLC, colZones.ActiveZone, _
                                         "Z" & colZones.CurrentZoneNumber.ToString, msZonePLCData)

                    System.Windows.Forms.Application.DoEvents()
                    System.Windows.Forms.Application.DoEvents()
                End If
                frmDevices.Robots = colArms.Count
                frmDevices.RobotItems = 8
                frmDevices.ZoneItems = 8
                Select Case colZones.ActiveZone.PLCType
                    Case ePLCType.Mitsubishi
                        frmDevices.UseRobotNumber = False
                        frmDevices.PLCDataStart = ePLCMANUAL_ZONE_LINK.MitsBBRobotWord

                    Case Else
                        frmDevices.UseRobotNumber = True
                        frmDevices.PLCDataStart = ePLCMANUAL_ZONE_LINK.BBRobotWord
                End Select
                For Each oArm As clsArm In colArms
                    Debug.Print(oArm.RobotNumber.ToString)
                Next
                frmDevices.Show(msZonePLCData, colArms)
            Case "btnBeakerMode"
                Try
                    'Enable/Disable beaker mode
                    If mbBeakerMode Then
                        'Beaker mode enabled.  Disable beaker mode, resume bell spin?
                        If MessageBox.Show(gpsRM.GetString("psDISABLE_BEAKER_TEXT"), gpsRM.GetString("psDISABLE_BEAKER_TITLE"), _
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.OK Then
                            Debug.Print("Disable Beaker mode OK")
                            WriteBeakerModeToPLC(0)
                        Else
                            Debug.Print("Disable Beaker mode Cancel")
                        End If
                    Else
                        'Beaker mode disabled - Enable beaker mode, stop bell spin?
                        If MessageBox.Show(gpsRM.GetString("psENABLE_BEAKER_TEXT"), gpsRM.GetString("psENABLE_BEAKER_TITLE"), _
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.OK Then
                            Debug.Print("Enable Beaker mode OK")
                            WriteBeakerModeToPLC(1)
                        Else
                            Debug.Print("Enable Beaker mode Cancel")
                        End If
                    End If
                Catch ex As Exception
                    DialogResult = ShowErrorMessagebox(msSCREEN_NAME & "frmMain:tlsMain_ItemClicked(btnBeakerMode)", ex, msSCREEN_NAME, Status)
                End Try

        End Select
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
    Private Sub btnFunction_DropDownOpening(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFunction.DropDownOpening
        '********************************************************************************************
        'Description:  this was needed to enable the menus for some reason
        '                  now handled in dostatusbar 2
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Dim cachePriviledge As ePrivilege
        Dim bRem As Boolean = False
        Dim bAllow As Boolean = False


        'If LoggedOnUser <> String.Empty Then

        '    If moPrivilege.Privilege = String.Empty Then
        '        cachePriviledge = ePrivilege.None
        '    Else
        '        If moPrivilege.ActionAllowed Then
        '            cachePriviledge = mPassword.PrivilegeStringToEnum(moPrivilege.Privilege)
        '        Else
        '            cachePriviledge = ePrivilege.None
        '        End If
        '    End If

        '    'Privilege = ePrivilege.Remote

        '    'thou shall not remote
        '    If Privilege = ePrivilege.Remote Then
        '        If colZones.PaintShopComputer = False Then
        '            bAllow = True
        '        End If
        '    End If


        '    Privilege = cachePriviledge

        'Else
        '    Privilege = ePrivilege.None
        'End If

        If colZones.PaintShopComputer = False Then
            bRem = mbRemoteZone
        End If
        bAllow = ((Privilege And ePrivilege.Remote) = ePrivilege.Remote)
        DoStatusBar2(stsStatus, LoggedOnUser, Privilege, bRem, bAllow)


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
        If SetRemoteServer() Then
            mbRemoteZone = True
            moPassword = Nothing

            If colArms Is Nothing = False Then
                For i As Integer = colArms.Count - 1 To 0 Step -1
                    colArms.Remove(colArms(i))
                Next
                colArms = Nothing
            End If
            If colControllers Is Nothing = False Then
                For i As Integer = colControllers.Count - 1 To 0 Step -1
                    colControllers.Remove(colControllers(i))
                Next
                colControllers = Nothing
            End If
            GC.Collect()
            System.Windows.Forms.Application.DoEvents()
            mPWRobotCommon.UrbanRenewal()
            System.Windows.Forms.Application.DoEvents()
            subInitializeForm()
        Else
            mbRemoteZone = bTmp
        End If
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

        If colArms Is Nothing = False Then
            For i As Integer = colArms.Count - 1 To 0 Step -1
                colArms.Remove(colArms(i))
            Next
            colArms = Nothing
        End If
        If colControllers Is Nothing = False Then
            For i As Integer = colControllers.Count - 1 To 0 Step -1
                colControllers.Remove(colControllers(i))
            Next
            colControllers = Nothing
        End If
        GC.Collect()
        System.Windows.Forms.Application.DoEvents()
        mPWRobotCommon.UrbanRenewal()
        System.Windows.Forms.Application.DoEvents()
        subInitializeForm()

    End Sub
    Private Sub subClearScreen()
        '********************************************************************************************
        'Description:  Clear out textboxes reset colors etc. here
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/03/09  MSW     Pressure Test Change
        '********************************************************************************************
        Dim sData(4) As String

        subEnableControls(False)

        'write min to plc
        Try

            ClearAllTextBoxes(colTextboxes)

            ftbTotal.Text = "0"
            ftbActualFlow.Text = "0"
            ftbActualAtom.Text = "0"
            ftbActualFan.Text = "0"
            ftbActualFan2.Text = "0"
            ftbActualEStatKv.Text = "0"
            ftbActualEstatUA.Text = "0"
            ftbActualTime.Text = "0"
            ftbTime.Text = "30"
            ftbTotal.ForeColor = Color.Black
            ftbActualFlow.ForeColor = Color.Black
            ftbActualAtom.ForeColor = Color.Black
            ftbActualFan.ForeColor = Color.Black
            ftbActualFan2.ForeColor = Color.Black
            ftbActualEStatKv.ForeColor = Color.Black
            ftbActualEstatUA.ForeColor = Color.Black
            ftbActualTime.ForeColor = Color.Black
            ftbTime.ForeColor = Color.Black
            'Cal status view is used by scale or paint cal
            mnuViewSelectCalStat.Visible = False
            mnuViewSelectDockCalStat.Visible = False
            mnuViewSelectDQCalStat.Visible = False
            mnuViewSelectDQCalStat2.Visible = False
            mnuViewSelectToon.Visible = False
            mnuViewSelectPressureTest.Visible = False '12/03/09 MSW Pressure Test Change
            gpbCalibration.Visible = False
            gpbViewAll.Visible = False
            If Not (uctrlCartoon Is Nothing) Then
                uctrlCartoon.Dispose()
            End If

        Catch ex As Exception
            Status = gpsRM.GetString("psCOULDNOTWRITEFTPARAMS")
            Trace.WriteLine(ex.Message)
            WriteEventToLog(msSCREEN_NAME & ":frmMain:subClearScreen", ex.StackTrace & vbCrLf & ex.Message)
        Finally

        End Try

    End Sub
    Private Sub subDoStatusBar(ByVal Controller As clsController)
        '********************************************************************************************
        'Description: do the icons on the status bar - set up for up to 10 robots
        '
        'Parameters: Controller that raised the event
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTipText As String = String.Empty
        Dim sImgKey As String = String.Empty

        'find the label we want
        Dim sName As String = "lbl" & Controller.Name

        Dim l As ToolStripStatusLabel = DirectCast(stsStatus.Items(sName), ToolStripStatusLabel)
        If l Is Nothing Then Exit Sub

        Select Case (Controller.RCMConnectStatus)
            Case Connstat.frRNConnecting
                sImgKey = "imgSBRYellow"
                sTipText = gcsRM.GetString("csCONNECTING")
            Case Connstat.frRNDisconnecting
                sImgKey = "imgSBRYellow"
                sTipText = gcsRM.GetString("csDISCONNECTING")
            Case Connstat.frRNAvailable
                sImgKey = "imgSBRBlue"
                sTipText = gcsRM.GetString("csAVAILABLE")
            Case Connstat.frRNConnected
                sImgKey = "imgSBRGreen"
                sTipText = gcsRM.GetString("csCONNECTED")
            Case Connstat.frRNUnavailable
                sImgKey = "imgSBRRed"
                sTipText = gcsRM.GetString("csUNAVAILABLE")
            Case Connstat.frRNUnknown
                sImgKey = "imgSBRGrey"
                sTipText = gcsRM.GetString("csUNKNOWN")
            Case Connstat.frRNHeartbeatLost
                sImgKey = "imgSBRRed"
                sTipText = gcsRM.GetString("csHBLOST")
        End Select

        l.ToolTipText = Controller.Name & " " & _
                            gcsRM.GetString("csCONNECTION_STAT") & " " & sTipText

        Try
            l.Image = DirectCast(gcsRM.GetObject(sImgKey), Image)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            WriteEventToLog(msSCREEN_NAME & ":frmMain:subDoStatusBar", ex.StackTrace & vbCrLf & ex.Message)
        End Try

        DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)

    End Sub
    Private Sub subFormatScreenLayout()
        '********************************************************************************************
        'Description:  set up the screen layout
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

    End Sub
    Private Sub subInitFormText(Optional ByVal Applicator As clsApplicator = Nothing)
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
        Try
            gpbRobots.Text = gcsRM.GetString("csROBOTS_OPERATE")
            gpbCCControl.Text = gcsRM.GetString("csCOLOR_CHG_CYCLE")
            mnuSelectCounts.Text = grsRM.GetString("rsCOUNTS")
            mnuSelectEngUnits.Text = grsRM.GetString("rsENG_UNITS")
            mnuSelectAllToolStripMenuItem.Text = gcsRM.GetString("csSELECT_ALL")
            mnuUnSelectAllToolStripMenuItem.Text = gcsRM.GetString("csUNSELECT_ALL")
            If Applicator Is Nothing Then
                subSetApplicatorLabels()
            Else
                subSetApplicatorLabels(Applicator)
            End If
            With gpsRM
                lblValuesCap.Text = .GetString("psTEST")

                lblTotalCap.Text = .GetString("psTOTAL")
                lblRobotView.Text = .GetString("psROBOTTOVIEW")
                lblTimeCap.Text = .GetString("psTIME_SECONDS")
                btnRun.Text = .GetString("psRUN")
                btnFlowTest.Text = .GetString("psSTART_TEST_BTN")
                lblApplicator.Text = .GetString("psAPPLICATOR_OPERATE")

                lblCylPos.Text = "0"
                lblCylPosCap.Text = .GetString("psCYLINDER_POS")

                gpbCalibration.Text = .GetString("psCALIBRATION")
                btnDQCal.Text = .GetString("psDQ") & " " & .GetString("psCAL")
                btnDQ2Cal.Text = .GetString("psDQ2") & " " & .GetString("psCAL")
                btnAutoCal.Text = .GetString("psPAINT_CAL")
                btnPressTest.Text = .GetString("psPAINT_CAL")
                lblParamOp.Text = .GetString("psCOLOR_OPERATE")
                btnBeakerMode.Text = .GetString("psBEAKER_MODE")
                btnBeakerMode.ToolTipText = .GetString("psBEAKER_TOOLTIPTEXT")
                btnDevice.ToolTipText = .GetString("psDEVICE_TOOLTIPTEXT")
                ToolTipMain.SetToolTip(gpbViewAll, .GetString("psVIEWDETAIL_TOOLTIP"))
                ToolTipMain.SetToolTip(clbRobot, .GetString("psCLBROBOT_TOOLTIP"))
                'ToolTipMain.SetToolTip(Me.Controls("btnDevice"), .GetString("psDEVICE_TOOLTIPTEXT"))
                gpbDockControl.Text = .GetString("psDOCK_CONTROL")
                gpbTPR.Text = .GetString("psTPR")
                gpbCurrentColor.Text = .GetString("psCUR_COLOR_CAP")
                gpbPumpEnables.Text = .GetString("psPUMP_ENABLES")
                chkPump1.Text = .GetString("psPUMP_1_ENABLE")
                chkPump2.Text = .GetString("psPUMP_2_ENABLE")
                lblHintAllRobots.Text = .GetString("psHINT_ALL_ROBOTS")
            End With
        Catch ex As Exception
            WriteEventToLog(msSCREEN_NAME & ":frmMain:subInitFormText", ex.StackTrace & vbCrLf & ex.Message)
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
        Dim lReply As Windows.Forms.DialogResult = Response.OK

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

        Status = gcsRM.GetString("csINITALIZING")
        Progress = 1


        Try

            subProcessCommandLine()

            Progress = 5

            subClearScreen()

            Me.Text = gpsRM.GetString("psSCREENCAPTION")

            'init new IPC and new Password 'RJO 03/22/12
            oIPC = New Paintworks_IPC.clsInterProcessComm
            moPassword = New clsPWUser
            Progress = 50

            colZones = New clsZones(String.Empty)
            mScreenSetup.LoadZoneBox(cboZone, colZones, False)
            mScreenSetup.InitializeForm(Me)
            Me.Refresh()
            System.Windows.Forms.Application.DoEvents()

            subInitFormText()

            Progress = 70
            'need to load before checking password to unlock if logged in
            mScreenSetup.LoadTextBoxCollection(Me, "gpbFlowParams", colTextboxes)
            subSetTextBoxProperties()

            'Allow control of the text color in the CC Cycle combobox 'RJO 04/21/11
            cboCycle.DrawMode = DrawMode.OwnerDrawFixed

            'init the old password for now
            'moPassword = New PWPassword.PasswordObject 'RJO 03/22/12
            'mPassword.InitPassword(moPassword, moPrivilege, LoggedOnUser, msSCREEN_NAME)
            'If LoggedOnUser <> String.Empty Then
            '    If moPrivilege.ActionAllowed Then
            '        Privilege = mPassword.PrivilegeStringToEnum(moPrivilege.Privilege)
            '    Else
            '        Privilege = ePrivilege.None
            '    End If
            'Else
            '    Privilege = ePrivilege.None
            'End If

            Progress = 98
            System.Windows.Forms.Application.DoEvents()
            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound

            'this was taking forever to show up
            Me.Show()
            System.Windows.Forms.Application.DoEvents()
            'statusbar text
            If cboZone.Text = String.Empty Then
                Status(True) = gcsRM.GetString("csSELECT_ZONE")
            Else
                If cboRobot.Text = String.Empty Then
                    Status(True) = gcsRM.GetString("csSELECT_ROBOT")
                End If
            End If

            If cboZone.Items.Count = 1 Then
                'this selects the zone in cases of just one zone. fires event to call subchangezone
                cboZone.Text = cboZone.Items(0).ToString
            Else
                cboApplicator.Enabled = False
            End If

            'Kludge for Lab Test of Honda Waterorne Applicator
            'With mPLC
            '    Dim sData(0) As String

            '    sData(0) = "1"
            '    .TagName = "Z1FluidMaintScreenActive"
            '    .PLCData = sData
            'End With

        Catch ex As Exception

            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
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
        ' 04/16/13  MSW     Standalone ChangeLog                                            4.1.5.0
        '********************************************************************************************

        Try
            mPWCommon.SaveToChangeLog(colZones.ActiveZone)


            mPWCommon.subShowChangeLog(colZones.CurrentZone, gsChangeLogArea, msSCREEN_NAME, cboColor.Text, _
                                          gcsRM.GetString("csALL"), mDeclares.eParamType.Colors, nIndex, False, False, oIPC)
        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
        End Try

    End Sub
#End Region

#Region " Zone, app and robot select"

    Private Sub cboZone_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                        Handles cboZone.SelectedIndexChanged
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
            System.Windows.Forms.Application.DoEvents()
            subChangeZone()
        End If

    End Sub
    Private Sub cboApplicator_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboApplicator.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Applicator Combo Changed 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If cboApplicator.Text <> msOldApplicator Then
            ' so we don't go off half painted
            Me.Refresh()
            System.Windows.Forms.Application.DoEvents()
            subChangeApplicator()
        End If

    End Sub


    Private Sub cboRobot_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                        Handles cboRobot.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Robot Combo Changed 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************


        If cboRobot.Text <> msOldRobot Then
            ' so we don't go off half painted
            Me.Refresh()
            System.Windows.Forms.Application.DoEvents()
            subChangeRobot()
        End If

    End Sub
    Private Sub cboColor_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                        Handles cboColor.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Color Combo Changed 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If cboColor.Text <> msOldColor Then
            ' so we don't go off half painted
            Me.Refresh()
            System.Windows.Forms.Application.DoEvents()
            subChangeColor()
        End If

    End Sub

    Private Sub clbRobot_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles clbRobot.SelectedIndexChanged
        '********************************************************************************************
        'Description:  robot clicked
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If mbBlockItemCheckEvent = True Then Exit Sub
            mbBlockItemCheckEvent = True

            Dim nTmp As Integer() = DirectCast(clbRobot.Tag, Integer())

            Dim sData(0) As String
            Dim bSelectRobot As Boolean = False
            'Get enables from PLC
            Dim nBoothStatus As Integer
            Dim nSelected As Integer = CInt(msOldZonePLCData(eZLink.RobotsSelectedWd))
            Try
                If (msZonePLCData Is Nothing) Then
                    nBoothStatus = 0
                    nSelected = 0
                Else
                    nBoothStatus = CInt(msZonePLCData(eZLink.RobotsReadyWd))
                    nSelected = CInt(msZonePLCData(eZLink.RobotsSelectedWd))
                End If
            Catch ex As Exception
                nBoothStatus = 0
                nSelected = 0
            End Try

            'this is the state before the click happened

            Dim nRobot As Integer = clbRobot.SelectedIndex
            'For i As Integer = 0 To clbRobot.Items.Count - 1
            '    If clbRobot.GetItemChecked(i) Then
            '        If ((nBoothStatus And gnBitVal(nTmp(i))) <> 0) Then
            '            'enabled in PLC
            '            nSelected = nSelected Or gnBitVal(nTmp(i))
            '        Else
            '            'Not enabled, turn it off
            '            clbRobot.SetItemChecked(i, False)
            '        End If
            '    End If
            'Next
            'this is the new value
            If ((nBoothStatus And gnBitVal(nTmp(nRobot))) <> 0) Then
                'Robot is ready
                If ((nSelected And gnBitVal(nTmp(nRobot))) <> 0) Then
                    'It's selected, turn it off
                    If clbRobot.CheckedItems.Count = 2 Then
                        'It was 2 robots, now only 1, so select it for the view
                        bSelectRobot = True
                        If clbRobot.CheckedItems(0).ToString = clbRobot.Items(nRobot).ToString Then
                            cboRobot.Text = clbRobot.CheckedItems(1).ToString
                        Else
                            cboRobot.Text = clbRobot.CheckedItems(0).ToString
                        End If
                    End If
                    nSelected = nSelected Xor gnBitVal(nTmp(nRobot))
                    clbRobot.SetItemCheckState(nRobot, CheckState.Unchecked)
                Else
                    'Not selected, turn it on
                    If nSelected = 0 Then
                        'Nothing was selected, so put the newly selected robot in cboRobot
                        bSelectRobot = True
                        cboRobot.Text = clbRobot.Items(nRobot).ToString
                    End If
                    nSelected = nSelected Or gnBitVal(nTmp(nRobot))
                    clbRobot.SetItemCheckState(nRobot, CheckState.Checked)
                End If
            Else
                'Robot not ready
                ToolTipMain.Show(gpsRM.GetString("psCLBROBOT_TOOLTIP2"), clbRobot)
                If ((nSelected And gnBitVal(nTmp(nRobot))) <> 0) Then
                    nSelected = nSelected Xor gnBitVal(nTmp(nRobot))
                End If
                clbRobot.SetItemCheckState(nRobot, CheckState.Unchecked)
            End If

            subEnableGroupboxes(clbRobot.Enabled, (nSelected > 0))
            subSetScreenActive(True)
            subSetPumpEnable(chkPump1.Checked, chkPump2.Checked)

            sData(0) = nSelected.ToString
            With mPLC
                .TagName = "Z" & colZones.CurrentZoneNumber.ToString & "SelectRobotWord"
                .PLCData = sData
            End With
            'This should make it behave better with fast clickers
            msZonePLCData(eZLink.RobotsSelectedWd) = nSelected.ToString
            'if we have 1 robot selected view that one
            If bSelectRobot Then
                subChangeRobot()
            End If
            mbBlockItemCheckEvent = False
        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
              Status, MessageBoxButtons.OK)
        End Try

    End Sub

    Private Sub mnuUnSelectAllToolStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuUnSelectAllToolStripMenuItem.Click
        '********************************************************************************************
        'Description:  Unselect all pop-up menu for robot list box
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            ' Block the check event from sending data to PLC when Item Checked
            mbBlockItemCheckEvent = True ' Used in clbRobot Event
            'Uncheck each robot
            For nLoop As Integer = 0 To clbRobot.Items.Count - 1
                clbRobot.SetItemCheckState(nLoop, CheckState.Unchecked)
            Next nLoop
            subEnableGroupboxes(clbRobot.Enabled, False)
            Dim sData(0) As String
            sData(0) = "0"
            With mPLC
                .TagName = "Z" & colZones.CurrentZoneNumber.ToString & "SelectRobotWord"
                .PLCData = sData
            End With
        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
              Status, MessageBoxButtons.OK)
        Finally
            mbBlockItemCheckEvent = False ' Used in clbRobot Event
        End Try
    End Sub
    Private Sub mnuSelectAllToolStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuSelectAllToolStripMenuItem.Click
        '********************************************************************************************
        'Description:  Select all pop-up menu for robot list box
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            ' Block the check event from sending data to PLC when Item Checked
            mbBlockItemCheckEvent = True ' Used in clbRobot Event
            'Get enables from PLC
            Dim nBoothStatus As Integer
            Try
                If (msZonePLCData Is Nothing) Then
                    nBoothStatus = 0
                Else
                    nBoothStatus = CInt(msZonePLCData(eZLink.RobotsReadyWd))

                End If
            Catch ex As Exception
                nBoothStatus = 0
            End Try
            'Check each robot
            Dim nTmp As Integer() = DirectCast(clbRobot.Tag, Integer())
            Dim nSelected As Integer
            For nLoop As Integer = 0 To clbRobot.Items.Count - 1
                If ((nBoothStatus And gnBitVal(nTmp(nLoop))) <> 0) Then
                    clbRobot.SetItemCheckState(nLoop, CheckState.Checked)
                    nSelected = nSelected Or gnBitVal(nTmp(nLoop))
                Else
                    clbRobot.SetItemCheckState(nLoop, CheckState.Unchecked)
                End If
            Next nLoop

            subEnableGroupboxes(clbRobot.Enabled, True)
            'build up a bit mask.  Have to account for not all robots being in the checkbox based on cc type
            Dim sData(0) As String
            sData(0) = nSelected.ToString
            With mPLC
                .TagName = "Z" & colZones.CurrentZoneNumber.ToString & "SelectRobotWord"
                .PLCData = sData
            End With
            'if we have had no robot selected view the first one
            If cboRobot.Text = String.Empty Then
                'Nothing was selected, so put a newly selected robot in cboRobot
                cboRobot.Text = clbRobot.Items(0).ToString
                subChangeRobot()
            End If
        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
              Status, MessageBoxButtons.OK)
        Finally
            mbBlockItemCheckEvent = False ' Used in clbRobot Event
        End Try
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

    Private Sub subSetScreenActive(ByVal bScreenActive As Boolean)
        '********************************************************************************************
        'Description: set the screen sctive bit
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/02/10  MSW     added a call to set the screen sctive bit
        '********************************************************************************************
        Dim sData(0) As String
        If bScreenActive Then
            sData(0) = "1"
        Else
            sData(0) = "0"
        End If

        With mPLC
            .TagName = "Z" & colZones.CurrentZoneNumber.ToString & "FluidMaintScreenActive"
            .PLCData = sData
        End With

    End Sub
    Private Sub chkPump_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkPump1.CheckedChanged, chkPump2.CheckedChanged
        '********************************************************************************************
        'Description: set the pump enable bits
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/08/10  MSW     1st draft
        '********************************************************************************************
        subSetPumpEnable(chkPump1.Checked, chkPump2.Checked)
    End Sub
    Private Sub subSetPumpEnable(ByVal nPumpEnable As Integer)
        '********************************************************************************************
        'Description: set the pump enable bits
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/08/10  MSW     1st draft
        '********************************************************************************************
        subSetPumpEnable(((nPumpEnable And 1) > 0), ((nPumpEnable And 2) > 0))
    End Sub
    Private Sub subSetPumpEnable(ByVal bPump1Enable As Boolean, ByVal bPump2Enable As Boolean)
        '********************************************************************************************
        'Description: set the pump enable bits
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/08/10  MSW     1st draft
        '********************************************************************************************

        If meOldApplicator <= eColorChangeType.NOT_SELECTED OrElse colApplicators Is Nothing Then
            Exit Sub
        End If
        Try
            Select Case colApplicators.Item(meOldApplicator).ColorChangeType
                Case eColorChangeType.BELL_2K, eColorChangeType.GUN_2K, eColorChangeType.GUN_2K_PIG, eColorChangeType.VERSABELL_2K, _
                     eColorChangeType.VERSABELL2_2K, eColorChangeType.VERSABELL2_2K_1PLUS1, eColorChangeType.VERSABELL2_2K_MULTIRESIN, _
                     eColorChangeType.GUN_2K_PIG, eColorChangeType.GUN_2K_Mica   'JZ 12062016 - Piggable stack only.
                    Dim sData(0) As String
                    If bPump1Enable Then
                        sData(0) = "1"
                    Else
                        sData(0) = "0"
                    End If

                    With mPLC
                        .TagName = "Z" & colZones.CurrentZoneNumber.ToString & "Pump1Enable"
                        .PLCData = sData
                    End With
                    If bPump2Enable Then
                        sData(0) = "1"
                    Else
                        sData(0) = "0"
                    End If

                    With mPLC
                        .TagName = "Z" & colZones.CurrentZoneNumber.ToString & "Pump2Enable"
                        .PLCData = sData
                    End With

                Case Else
            End Select

        Catch ex As Exception

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
        ' 11/23/09  RJO     Changed call to LoadArmCollection to not include openers.
        ' 11/19/09  MSW     set cboApplicator index to 0, not 1
        ' 11/19/09  MSW     replace string literals with resource strings
        ' 03/28/12  MSW     Modified for .NET Scattered Access                            4.01.03.00
        '********************************************************************************************
        Progress = 10

        'just in case
        If cboZone.Text = String.Empty Then
            Exit Sub
        Else
            If cboZone.Text = msOldZone Then Exit Sub
        End If

        msOldZone = cboZone.Text
        colZones.CurrentZone = cboZone.Text

        System.Windows.Forms.Application.DoEvents()
        Try
            'NRU 161004 Destroy old hotlinks when changing zones
            If Not IsNothing(mPLC) Then mPLC.Dispose()
            mPLC = Nothing

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

            'this disables controls
            subClearScreen()
            cboApplicator.Enabled = False
            cboRobot.Enabled = False


            'Shut down any scattered access connections that are updating
            'If Not (colArms Is Nothing) Then
            '    For Each Controller As clsController In colControllers
            '        If Controller.Robot.IsConnected Then
            '            If oScatteredAccess.IsSetUp Then
            '                oScatteredAccess.UpdateActive(Controller.Name) = False
            '            End If 'oSA.IsSetUp
            '        End If
            '    Next
            'End If
            Dim sMessage(1) As String
            sMessage(0) = msSCREEN_NAME & ";OFF"
            sMessage(1) = "ALL"
            oIPC.WriteControlMsg(gs_COM_ID_SA, sMessage)

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
              Status, MessageBoxButtons.OK)
        End Try
        Try

            System.Windows.Forms.Application.DoEvents()
            Progress = 20
            'Load up controller/arm info
            colControllers = New clsControllers(colZones, False)
            'Reset error message latches
            SetUpStatusStrip(Me, colControllers)
            colArms = LoadArmCollection(colControllers, False) 'rjo 11/23/09
            Progress = 30
            'Check up on the robot connections
            colControllers.RefreshConnectionStatus()
            subInitScatteredAccess()
            'Setup dq cal status
            ReDim mnGetPntCalStatus(colArms.Count)
            ReDim msPntCalStatus(colArms.Count)
            ReDim mnGetCalStatus(colArms.Count)
            ReDim mnGetCalStatus2(colArms.Count)
            ReDim mnLineState(colArms.Count) 'RJO 04/21/11
            ReDim msCalStatus(colArms.Count)
            ReDim msCalStatus2(colArms.Count)
            ReDim mbShowDQCalActive(colArms.Count)
            'ReDim mbShowDQCalActive2(colArms.Count)
            ReDim mbShowCalActive(colArms.Count)
            ReDim mbShowCalActive2(colArms.Count)
            For nArm As Integer = 0 To colArms.Count
                mnGetCalStatus(nArm) = 0
                mnGetCalStatus2(nArm) = 0
                mnLineState(nArm) = eLineState.ArmNotSelected 'RJO 04/21/11
                msCalStatus(nArm) = String.Empty
                msCalStatus2(nArm) = String.Empty
                mnGetPntCalStatus(nArm) = 0
                msPntCalStatus(nArm) = String.Empty
            Next
            Application.DoEvents()
            'Load applicator/CC type details for arms in the zone
            colApplicators = New clsApplicators(colArms, colZones.ActiveZone)
            'MSW WB sets the min to -1000, exclude that here.
            For Each oApp As clsApplicator In colApplicators
                If oApp.MinEngUnit(0) < 0 Then
                    oApp.MinEngUnit(0) = 0
                End If
            Next
            LoadApplBoxFromCollection(cboApplicator, colApplicators, False)
            'Clear out the detail box on the right
            subResetViewAllBox(colArms)
            Progress = 40
            'enable the beaker mode button if there are any bells in the zone.
            btnBeakerMode.Visible = False
            For Each applicator As clsApplicator In colApplicators
                If applicator.BellApplicator Then
                    btnBeakerMode.Visible = True
                    Exit For
                End If
            Next
            Progress = 50
            'Reset the other cbos
            cboColor.Items.Clear()
            msOldColor = String.Empty
            msOldRobot = String.Empty

            Progress = 60
            If (mPLC Is Nothing) Then
                mPLC = New clsPLCComm
                mPLC.ZoneName = colZones.ActiveZone.Name
            End If
            'Make sure PLC hotlink is running
            mManFlowCommon.InitPlc(mPLC, colZones.ActiveZone, _
               "Z" & colZones.CurrentZoneNumber.ToString, msZonePLCData)
            If (msZonePLCData Is Nothing) = False Then
                subUpdateZoneHotlink(msZonePLCData, True)
            End If
            'More than one applicator?
            If cboApplicator.Items.Count > 1 Then
                'statusbar text
                If cboRobot.Text = String.Empty Then
                    Status(True) = gcsRM.GetString("csSELECT_APPLICATOR")
                End If
                cboApplicator.Enabled = True
            Else
                'This will trigger an event on the applicator cbo and end up calling subChangeApplicator
                cboApplicator.Enabled = False
                'NRU 161004 Clear oldapplicator to enable loading up the robot box with new robots for a new zone.
                msOldApplicator = Nothing
                'Prevent this from barfing if there aren't any online robots 05/28/10 RJO
                If cboApplicator.Items.Count > 0 Then cboApplicator.SelectedIndex = 0
                'cboApplicator.SelectedIndex = 0
            End If

            Progress = 90

            'Call subSelectCalState(False) 'MSW 4/29/10 False) 'RJO 12/02/09

            Dim sData(0) As String

            sData(0) = "1"

            With mPLC
                .TagName = "Z" & colZones.CurrentZoneNumber.ToString & "FluidMaintScreenActive"
                .PLCData = sData
            End With

            subSetPumpEnable(chkPump1.Checked, chkPump2.Checked)
            mbBlockItemCheckEvent = False


        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
              Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try


    End Sub
    Private Sub subChangeApplicator()
        '********************************************************************************************
        'Description: New Applicator Selected
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/15/10  MSW     reset view all box after applicator selection
        '********************************************************************************************
        Try
            If cboApplicator.Text = String.Empty Then
                Exit Sub
            Else
                If cboApplicator.Text = msOldApplicator Then Exit Sub
            End If
            'The applicator cbo tag property has an array of app types that matches the drop-down descriptions
            'This'll get the app/cc type in a variable we can work with
            Dim eTmp As eColorChangeType() = DirectCast(cboApplicator.Tag, eColorChangeType())
            If (UBound(eTmp) < cboApplicator.SelectedIndex) Or (LBound(eTmp) > cboApplicator.SelectedIndex) Then
                Exit Sub 'or it'll give up
            End If

            tmrUpdateSA.Enabled = False
            meOldApplicator = eTmp(cboApplicator.SelectedIndex)
            msOldApplicator = cboApplicator.Text
            Progress = 10

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            'this disables controls
            subClearScreen()
            cboRobot.Enabled = False


            'We'll leave the full controller and alarm collections, but load up the robot boxes
            'with an applicator mask.  These routines match by applicator name, by default we're using the 
            'short name, so similar applicators may be selected together.
            'It'll split big differences like guns and bells, though.
            'First one with a mix of applicators in PW4 gets to see if it works
            Dim bType As String
            If (meOldApplicator = eColorChangeType.NOT_SELECTED) Then
                bType = ""
            Else
                bType = cboApplicator.Text
            End If
            LoadRobotBoxFromCollectionByCCTypeName(cboRobot, colArms, False, colApplicators, cboApplicator.Text)
            LoadRobotBoxFromCollectionByCCTypeName(clbRobot, colArms, False, colApplicators, cboApplicator.Text)

            'Bad Honda
            If colZones.ActiveZone.PLCType = ePLCType.Mitsubishi Then
                ''Kludge for Honda 12 Painter arm cell
                Dim nTag(clbRobot.Items.Count - 1) As Integer
                For nIndex As Integer = 0 To (clbRobot.Items.Count - 1)
                    nTag(nIndex) = nIndex + 1
                Next
                clbRobot.Tag = nTag
            End If
            msOldRobot = String.Empty

            'Clear out robot selection
            Dim sData(0) As String
            sData(0) = "0"
            With mPLC
                .TagName = "Z" & colZones.CurrentZoneNumber.ToString & "SelectRobotWord"
                .PLCData = sData
            End With

            Progress = 30

            'Setup the flow test box
            If meOldApplicator <> eColorChangeType.NOT_SELECTED Then
                subInitFlowTest(colApplicators(meOldApplicator))
            End If

            'statusbar text
            If cboRobot.Text = String.Empty Then
                Status(True) = gcsRM.GetString("csSELECT_ROBOT")
            End If

            'sometimes we dont catch the hotlink update the first time
            'Make sure PLC data is updated after everything is set up.
            If (msZonePLCData Is Nothing) = False Then
                subUpdateZoneHotlink(msZonePLCData, True)
            End If
            Progress = 80

            'Enable the controls and get out
            subEnableControls(True)
            Dim eTmpView As eViewSelect = meViewSelect
            meViewSelect = eViewSelect.Toon
            subResetViewAllBox(colArms)
            If eTmpView > eViewSelect.Toon Then
                If (eTmpView = eViewSelect.DQCalStat2) And (colApplicators(meOldApplicator).NumParms <= eParamID.Fan2) Then
                    eTmpView = eViewSelect.DQCalStat
                End If
                subSelectView(eTmpView)
            End If
            'Select a robot to view if there's already some selected for control
            If cboRobot.Text = String.Empty And clbRobot.CheckedItems.Count > 0 Then
                cboRobot.Text = clbRobot.CheckedItems(0).ToString
                subChangeRobot()
            End If
        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
              Status, MessageBoxButtons.OK)
            cboRobot.Enabled = True
        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try


    End Sub
    Private Sub subChangeRobot()
        '********************************************************************************************
        'Description:  New robot selected
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Modified for .NET Scattered Access                            4.01.03.00
        '********************************************************************************************
        Dim nViewRobotIndex As Integer = -1

        'just in case
        If cboRobot.Text = String.Empty Then
            Exit Sub
        Else
            If cboRobot.Text = msOldRobot Then Exit Sub
        End If
        Try

            tmrUpdateSA.Enabled = False
            ''Stop scattered access update if it's only using one
            If meViewSelect = eViewSelect.Toon Then
                Dim sMessage(1) As String
                sMessage(0) = msSCREEN_NAME & ";SET"
                sMessage(1) = cboRobot.Text
                oIPC.WriteControlMsg(gs_COM_ID_SA, sMessage)
                '    'Shut down any scattered access connections that are updating
                '    If Not (colArms Is Nothing) Then
                '        For Each Controller As clsController In colControllers
                '            If Controller.Robot.IsConnected Then
                '                If oScatteredAccess.IsSetUp Then
                '                    oScatteredAccess.UpdateActive(Controller.Name) = False
                '                End If 'oSA.IsSetUp
                '            End If
                '        Next
                '    End If
            Else
                Dim sMessage(1) As String
                sMessage(0) = msSCREEN_NAME & ";SET"
                sMessage(1) = "ALL"
                oIPC.WriteControlMsg(gs_COM_ID_SA, sMessage)
            End If
        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
              Status, MessageBoxButtons.OK)
        End Try

        Try
            msOldRobot = cboRobot.Text

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            Progress = 5

            'Get the robot number
            Dim nTmp As Integer() = DirectCast(cboRobot.Tag, Integer())
            If UBound(nTmp) >= cboRobot.SelectedIndex Then
                nViewRobotIndex = nTmp(cboRobot.SelectedIndex)
            End If
            mRobot = colArms.Item(cboRobot.Text)
            colArms.Item(cboRobot.Text).Selected = True
            mRobot.SystemColors.Load(mRobot)
            'Load the system colors
            mSysColorCommon.LoadColorBoxFromCollection(mRobot.SystemColors, cboColor, True, True)
            mbAsciiColor = mRobot.SystemColors.IsAscii
            mnAsciiCharacters = mRobot.SystemColors.PlantAsciiMaxLength
            'Set up the flow test labels if they aren't done yet
            If meOldApplicator = eColorChangeType.NOT_SELECTED Then
                subInitFlowTest(colApplicators.Item(mRobot.ColorChangeType))
            End If
            mRobot.Applicator = colApplicators(mRobot.ColorChangeType)
            'Setup the fluid diagram display
            subChangeCCType(mRobot)
            'show the correct color in cboColor
            If Not (mPLC Is Nothing) Then
                'set to current manual color
                Dim sCol As String = mManFlowCommon.CurrentManualColor(mPLC, _
                 "Z" & colZones.CurrentZoneNumber.ToString)
                If sCol <> "0" Then
                    Dim s As String() = DirectCast(cboColor.Tag, String())
                    For i As Integer = 0 To UBound(s)
                        If s(i) = sCol Then
                            cboColor.SelectedIndex = i
                            Exit For
                        End If
                    Next
                End If
                ' NEW Update Robot Hot Link 1 Link per Robot 
                'mManFlowCommon.RobotFlowDataHotLink(mPLC, colZones.ActiveZone, nViewRobotIndex, mnOldRobotNum, msRobotPLCData)
                'subUpdateRobotHotlink(msRobotPLCData, mRobot.RobotNumber)
            End If
            msOldColor = cboColor.Text
            mnOldRobotNum = nViewRobotIndex


            subEnableControls(True)

            mnuViewSelectToon.Visible = True

            'MSW Moved Scattered access names and indexes into subChangeRobot instead of searching each time the timer ticks
            For Each o As clsArm In colArms
                msControllerName = o.Name
                If o.Name = cboRobot.Text Then
                    msControllerName = o.Controller.Name
                    msArmSelected = cboRobot.Text
                    mnArmNumber = o.ArmNumber '1=EQ1 2=EQ2
                    mnRobIdx = colArms.IndexOf(o)
                    bIndexScatteredAccess(mnRobIdx) = True
                    Exit For
                End If
            Next

            'Pop up a message if the selected robot is offline.
            'I guess the retry thing is a waste, but it doesn't hurt anything.
            Dim reply As Windows.Forms.DialogResult = Windows.Forms.DialogResult.Retry
            Do While (mRobot.IsOnLine = False) And (reply = Windows.Forms.DialogResult.Retry)
                reply = MessageBox.Show((msControllerName & gcsRM.GetString("csIS_OFFLINE")), _
                                       (msSCREEN_NAME & " - " & msControllerName), _
                                       MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, _
                                       MessageBoxDefaultButton.Button2)
                mbOfflineMessage(colControllers.IndexOf(mRobot.Controller)) = False
                If reply = Windows.Forms.DialogResult.Cancel Then
                    cboRobot.Text = String.Empty
                    cboRobot.SelectedIndex = -1
                    msOldRobot = String.Empty
                    subCleanUpRobotLabels(mRobot, True)
                End If
            Loop

            If mRobot.IsOnLine AndAlso (mRobot.ColorChangeType = eColorChangeType.HONDA_WB Or _
                     mRobot.ColorChangeType = eColorChangeType.VERSABELL2_PLUS_WB Or _
                     mRobot.ColorChangeType = eColorChangeType.VERSABELL2_WB Or _
                     mRobot.ColorChangeType = eColorChangeType.ACCUSTAT Or _
                     mRobot.ColorChangeType = eColorChangeType.AQUABELL Or _
                     mRobot.ColorChangeType = eColorChangeType.VERSABELL3_WB Or _
                     mRobot.ColorChangeType = eColorChangeType.VERSABELL3_DUAL_WB) Then
                txtTPR.Text = mManFlowCommon.GetTPR(mRobot)
            End If

            'Enable scattered access update
            tmrUpdateSA.Enabled = True
            Status(True) = gcsRM.GetString("csSELECT_COLOR")

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
              Status, MessageBoxButtons.OK)

        Finally
            Me.Cursor = System.Windows.Forms.Cursors.Default
            Progress = 100
        End Try


    End Sub
    Private Sub subChangeCCType(ByRef mRobot As clsArm)
        '********************************************************************************************
        'Description:  set up the screen per cc type
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 8/10/10   MSW     Load cycle lists
        ' 04/21/11  RJO     Added code to load CC Cycle name array for WB cycle disable.
        '********************************************************************************************
        Dim nVis As Integer = 0
        Dim sName As String = String.Empty
        Dim oL As Label = Nothing
        Try
            Dim cc As clsColorChange = mRobot.SystemColors(0).Valve.ColorChange
            'I guess this actually reads the names from the robot
            'Nice for now, but it'll need to come from the PC for multilanguage support
            mCCCommon.LoadCCCycleBox(cc, cboCycle, True) 'MSW 8/10/10 - load cycle lists

            'Robot doesn't store Standard and Full cycle times.
            nVis = 8 'cboCycle.Items.Count

            'Initially clear the list of disabled CC Cycles 'RJO 04/21/11
            mlstDisabledCCItems.Clear()

            'Load array of CC Cycle Names 'RJO 04/21/11
            ReDim masCCItems(nVis)

            For nCycle As Integer = 1 To nVis
                masCCItems(nCycle) = cboCycle.Items(nCycle - 1).ToString
            Next

            'load cccyclename labels
            For i As Integer = 1 To 10
                sName = "lblCycleName" & Format(i, "00")
                oL = DirectCast(gpbCCCycle.Controls(sName), Label)
                If i <= nVis Then
                    oL.Text = cboCycle.GetItemText(cboCycle.Items(i - 1))
                    oL.Visible = True
                Else
                    oL.Visible = False
                End If
                sName = "lblCCTime" & Format(i, "00")
                oL = DirectCast(gpbCCCycle.Controls(sName), Label)
                If i <= nVis Then
                    oL.Text = "0.0"
                    oL.Visible = True
                Else
                    oL.Visible = False
                End If
            Next

            'Set up the fancy graphics
            SetCCType(colZones, Me.tscMain.ContentPanel.Controls, uctrlCartoon, mCCToon, mRobot)

            If ((uctrlCartoon Is Nothing) = False) And ((mCCToon Is Nothing) = False) Then
                'Set the size and location on the form.
                'NVSP 11/03/2016 increase the size to the form size now...
                'uctrlCartoon.Size = New Size(316, uctrlCartoon.Height) '556)
                uctrlCartoon.Size = New Size(694, uctrlCartoon.Height) '556) 
                uctrlCartoon.Location = New Point(694, 0)
                lblHintAllRobots.Top = uctrlCartoon.Bottom
                lblHintAllRobots.Left = uctrlCartoon.Left
                ToolTipMain.SetToolTip(uctrlCartoon, gpsRM.GetString("psVIEWDETAIL_TOOLTIP"))
                Me.Refresh()
            End If
            'Hide the cal/flow status box
            gpbViewAll.Visible = (meViewSelect > eViewSelect.Toon)
            'Show the fluid diagram
            If Not (uctrlCartoon Is Nothing) Then
                uctrlCartoon.Visible = (meViewSelect = eViewSelect.Toon)
                uctrlCartoon.ContextMenuStrip = mnuViewSelect
            End If

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
  Status, MessageBoxButtons.OK)
        End Try

    End Sub
    Private Sub subChangeColor()
        '********************************************************************************************
        'Description:  New Color selected
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'just in case
        If cboColor.Text = String.Empty Then
            Exit Sub
        Else
            If cboColor.Text = msOldColor Then Exit Sub
        End If
        msOldColor = cboColor.Text


        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

            Status = gpsRM.GetString("psSEND_TO_PLC")
            Dim s As String() = CType(cboColor.Tag, String())
            If mRobot.SystemColors.IsAscii Then
                mManFlowCommon.CurrentManualColor(mPLC, _
                   "Z" & colZones.CurrentZoneNumber.ToString) = mMathFunctions.CvASCIIToInteger(s(cboColor.SelectedIndex), mRobot.SystemColors.PlantAsciiMaxLength).ToString

            Else
                mManFlowCommon.CurrentManualColor(mPLC, _
                   "Z" & colZones.CurrentZoneNumber.ToString) = s(cboColor.SelectedIndex)
            End If

            For nArm As Integer = 0 To colArms.Count
                'Reset cal status displays
                mnGetPntCalStatus(nArm) = 0
                msPntCalStatus(nArm) = String.Empty
            Next

            For Each oSysColor As clsSysColor In mRobot.SystemColors
                If s(cboColor.SelectedIndex) = oSysColor.PlantNumber.Text Then
                    mnValve = oSysColor.Valve.Number.Value
                End If
            Next

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                 Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try


    End Sub
    Private Sub subEnableCCCycles()
        '********************************************************************************************
        'Description: Disable or enable controls color change cycles in the combo box based on the
        '             worst case line state for all selected robots. WB bells only.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/21/01  RJO     Initial code
        ' 07/11/13  RJO     mnLineState data starts at index 0 for the first arm, not 1.
        '********************************************************************************************

        If cboCycle.Items.Count = 0 Then Exit Sub

        'Determine the worst case
        Dim nWorstCase As Integer = eLineState.ArmNotSelected

        'For nPaintArm As Integer = 1 To colArms.Count
        For nPaintArm As Integer = 0 To colArms.Count - 1 'RJO 07/11/13            
            If mnLineState(nPaintArm) > nWorstCase Then
                nWorstCase = mnLineState(nPaintArm)
            End If
        Next

        'Add the appropriate cycles to the disabled cycles list
        mlstDisabledCCItems.Clear()

        Select Case nWorstCase
            Case eLineState.CleanedOut
                'All cycles OK to run
            Case eLineState.PushedOut
                'All cycles OK to run
            Case eLineState.Filled
                mlstDisabledCCItems.Add(masCCItems(4)) 'Fill Cycle
            Case eLineState.Unknown
                mlstDisabledCCItems.Add(masCCItems(4)) 'Fill Cycle
                mlstDisabledCCItems.Add(masCCItems(5)) 'ReFill Cycle
            Case Else 'eLineState.ArmNotSelected - No robots selected
                'All cycles OK to run
        End Select

        btnRun.Enabled = gpbCCControl.Enabled AndAlso EnableRunButton()

    End Sub
    Private Sub subEnableControls(ByVal bNotBusy As Boolean)
        '********************************************************************************************
        'Description: Disable or enable controls. check privileges and edits etc. 
        '
        'Parameters: bNotBusy - false should only be used when the screen is busy.
        '           passwords, eveything else will be checked in here.
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/27/09  MSW     move some conditions into this routine instead of trying to pass them in.
        '********************************************************************************************
        Dim bRestOfControls As Boolean = False

        Dim bZoneSelected As Boolean = (cboZone.Text <> String.Empty)
        Dim bApplicatorSelected As Boolean = (cboApplicator.Text <> String.Empty)

        'Always enabled
        btnClose.Enabled = True
        btnStatus.Enabled = True
        'Robot view select is always available if the applicator has been selected
        cboRobot.Enabled = bApplicatorSelected

        If bNotBusy = False Then
            'Shut it all down - for when it's busy or no zone selected.  
            'even logged out gets more access than this
            bRestOfControls = False
            btnBeakerMode.Enabled = False
            cboApplicator.Enabled = False
            btnChangeLog.Enabled = False
            btnDevice.Enabled = False
        Else
            'No password required for these
            btnChangeLog.Enabled = bZoneSelected
            btnDevice.Enabled = bZoneSelected
            cboApplicator.Enabled = bZoneSelected And (cboApplicator.Items.Count > 1)
            Select Case Privilege
                Case ePrivilege.None
                    'Disable group boxes
                    bRestOfControls = False
                    'Disable beaker mode
                    btnBeakerMode.Enabled = False
                Case ePrivilege.Edit
                    'Enable the group boxes
                    bRestOfControls = bZoneSelected And bApplicatorSelected
                    If btnBeakerMode.Visible Then
                        ' If it's visible, then it's enabled for the zone
                        'PLC will give the OK to use beaker mode based when we're in manual
                        If (msZonePLCData Is Nothing) Then
                            btnBeakerMode.Enabled = False
                        Else
                            Dim nManualStatus As Integer = CInt(msZonePLCData(eZLink.ManualStatusWd))
                            If ((nManualStatus And gnBitVal(eZLink.BeakerEnabBit)) <> 0) Then
                                btnBeakerMode.Enabled = True
                            Else
                                btnBeakerMode.Enabled = False
                            End If
                        End If
                    End If
            End Select
        End If

        'restof controls here
        LockAllTextBoxes(colTextboxes, (Not bRestOfControls))
        subEnableGroupboxes(bRestOfControls, (clbRobot.CheckedItems.Count > 0))

        'special cases for Dock Move To functions
        'TODO - Determine when to disable Dock Move To functions
        btnMoveClean.Enabled = bRestOfControls
        btnMoveDeDock.Enabled = bRestOfControls
        btnMoveDock.Enabled = bRestOfControls

    End Sub
    Private Sub subEnableGroupboxes(ByVal bEnable As Boolean, _
                                        Optional ByVal bRobotsChecked As Boolean = False)
        '********************************************************************************************
        'Description: 
        '
        'Parameters: bCheckedStateOnPending - this is a quick hack because if responding to a check
        '           event we may still have a checkitemcount of 0
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/11/11  MSW     Track CC status and manage enables better
        ' 04/21/11  RJO     Revised to call EnableRunButton function for btnRun State.
        ' 04/26/11  MSW     CC cycle disable debug
        '********************************************************************************************
        'Booth OK?
        Try
            If (msZonePLCData Is Nothing) Then
                bEnable = False
            Else
                Dim nBoothStatus As Integer = CInt(msZonePLCData(eZLink.RobotsReadyWd))
                If ((nBoothStatus And gnBitVal(eZLink.BoothReadyBit)) = 0) Then
                    bEnable = False
                End If
            End If
        Catch ex As Exception
            bEnable = False
        End Try
        Dim bTmp As Boolean = (bEnable And (Privilege >= ePrivilege.Edit))
        'Have to leave the group box enabled so the status box will scroll
        ' deal with the robot checkboxes separately
        gpbRobots.Enabled = True
        'Robot checkboxes
        If bTmp Then
            clbRobot.Enabled = bTmp
        Else
            clbRobot.Enabled = bTmp
        End If
        'The rest of these need a robot selected
        bTmp = bTmp And bRobotsChecked And (mbCalActive = False) And (mbFlowTestActive = False) And (mbCCInProgress = False)

        btnRun.Enabled = (bTmp And EnableRunButton()) 'RJO 04/21/11
        'btnRun.Enabled = (bTmp And (cboCycle.Text <> String.Empty)) 'RJO 04/21/11
        gpbCCControl.Enabled = bTmp ' 04/26/11  MSW     CC cycle disable debug
        gpbFlowParams.Enabled = bTmp Or mbFlowTestActive ' allow cancel
        gpbCalibration.Enabled = bTmp 'RJO 12/02/09
        'gpbCalibration.Enabled = CType(meViewSelect And eViewSelect.DQCalStat, Integer) > 0 'RJO 12/02/09
        gpbDockControl.Enabled = bTmp 'And (meViewSelect = eViewSelect.Toon)
        gpbPumpEnables.Enabled = bTmp
        gpbTPR.Enabled = bTmp
        gpbCurrentColor.Enabled = bTmp
        cboColor.Enabled = bTmp
        lblParamOp.Enabled = bTmp
        bTmp = (mbCalActive = False) And (mbFlowTestActive = False) And (mbCCInProgress = False)
        cboApplicator.Enabled = bTmp
        cboZone.Enabled = bTmp
    End Sub
    Private Sub subSetApplicatorLabels(Optional ByVal Applicator As clsApplicator = Nothing)
        '********************************************************************************************
        'Description: Setup all the labels that depend on applicator type
        '       also enables some buttons and pop-up menus
        'Parameters: Applicator - determines which labels to use
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/03/09  MSW     Pressure Test Change
        ' 03/31/11  MSW     Autocal for IPC
        '********************************************************************************************
        Try
            'Common items for now
            lblUACap.Text = grsRM.GetString("rsUA")
            lblKVCap.Text = grsRM.GetString("rsKV")
            mnuViewSelectToon.Text = gpsRM.GetString("psFLUID_DIAGRAM")
            gpbCalibration.Visible = False
            'Setup defaults if there isn't an applicator selection
            If Applicator Is Nothing Then
                'Setup defaults if there isn't an applicator selection
                lblFlowCap.Text = gpsRM.GetString("psFLOWRATE")
                lblAtomCap.Text = gpsRM.GetString("psATOMAIR")
                lblFanCap.Text = gpsRM.GetString("psFANAIR")
                lblEstatCap.Text = gpsRM.GetString("psESTATKV")
                lblActualCap.Text = gpsRM.GetString("psACTUAL")
                ftbActualAtom.Visible = False
                lblAtomFBCap.Visible = False
                ftbActualFan.Visible = False
                lblFanFBCap.Visible = False
                lblFan2Cap.Visible = False
                lblFan2ActCap.Visible = False
                lblAtomFBCap.Text = grsRM.GetString("rsPSI")
                mnuViewSelectCalStat.Visible = False
                mnuViewSelectDockCalStat.Visible = False
                mnuViewSelectDQCalStat.Visible = False
                mnuViewSelectDQCalStat2.Visible = False
                mnuViewSelectPressureTest.Visible = False '12/03/09 MSW Pressure Test Change
                mnuViewSelectPaint.Text = grsRM.GetString("psPAINT") & " " & gpsRM.GetString("psAll_ROBOTS")
                mnuViewSelectAA_BS.Text = grsRM.GetString("rsATOMAIR") & " " & gpsRM.GetString("psAll_ROBOTS")
                mnuViewSelectFA_SA.Text = grsRM.GetString("rsFANAIR") & " " & gpsRM.GetString("psAll_ROBOTS")
                mnuViewSelectEstat.Text = grsRM.GetString("rsESTAT") & " " & gpsRM.GetString("psAll_ROBOTS")
                mnuViewSelectPressureTest.Text = gpsRM.GetString("psPRESSURE_TEST") & " " & gpsRM.GetString("psAll_ROBOTS")
            Else
                'The applicator object will build up the flow test parameter labels for us.
                'JZ 12072016 - Make labels pretty.
                If mbEngUnits(eParamID.Flow) = True Then
                    'lblFlowCap.Text = Applicator.FlowTestLabel(eParamID.Flow)
                    lblFlowCap.Text = gpsRM.GetString("psFLOWRATE")
                Else
                    'lblFlowCap.Text = Applicator.FlowTestCountsLabel(eParamID.Flow)
                    lblFlowCap.Text = gpsRM.GetString("psFLOWRATE")
                End If
                If mbEngUnits(eParamID.Atom) = True Then
                    'lblAtomCap.Text = Applicator.FlowTestLabel(eParamID.Atom)
                    lblAtomCap.Text = gpsRM.GetString("psATOMAIR")
                Else
                    'lblAtomCap.Text = Applicator.FlowTestCountsLabel(eParamID.Atom)
                    lblAtomCap.Text = gpsRM.GetString("psATOMAIR")
                End If
                If mbEngUnits(eParamID.Fan) = True Then
                    'lblFanCap.Text = Applicator.FlowTestLabel(eParamID.Fan)
                    lblFanCap.Text = gpsRM.GetString("psFANAIR")
                Else
                    'lblFanCap.Text = Applicator.FlowTestCountsLabel(eParamID.Fan)
                    lblFanCap.Text = gpsRM.GetString("psFANAIR")
                End If
                If mbEngUnits(eParamID.Estat) = True Then
                    'lblEstatCap.Text = Applicator.FlowTestLabel(eParamID.Estat)
                    lblEstatCap.Text = gpsRM.GetString("psESTATKV")
                Else
                    'lblEstatCap.Text = Applicator.FlowTestCountsLabel(eParamID.Estat)
                    lblEstatCap.Text = gpsRM.GetString("psESTATKV")
                End If
                btnDQCal.Visible = False
                btnDQ2Cal.Visible = False
                mnuViewSelectDQCalStat.Visible = False
                mnuViewSelectDQCalStat2.Visible = False
                'If it's a bell, enable bell speed feedback
                'If it has DQ, enable DQ feedback
                'Also enable the DQ cal button and the DQ cal status display on the view menu
                Select Case Applicator.CntrType(eParamID.Atom)
                    Case eCntrType.APP_CNTR_BS, eCntrType.APP_CNTR_TS
                        lblAtomFBCap.Text = Applicator.ParamUnits(eParamID.Atom)
                        lblFanCap.Text = gpsRM.GetString("psSHAPING_AIR")
                        ftbActualAtom.Visible = True
                        lblAtomFBCap.Visible = True
                    Case eCntrType.APP_CNTR_DQ
                        btnDQCal.Text = gpsRM.GetString("psDQ") & " " & gpsRM.GetString("psCAL")
                        lblAtomFBCap.Text = grsRM.GetString("rsPSI")
                        lblFanCap.Text = gpsRM.GetString("psSHAPING_AIR")
                        ftbActualAtom.Visible = True
                        lblAtomFBCap.Visible = True
                        btnDQCal.Visible = True
                        btnDQ2Cal.Visible = False
                        lblFan1Cap.Visible = False
                        lblFan2Cap.Visible = False
                        lblFan2ActCap.Visible = False
                        gpbCalibration.Visible = True
                        mnuViewSelectDQCalStat.Visible = True
                        mnuViewSelectDQCalStat.Text = lblFanCap.Text & " " & gpsRM.GetString("psCAL_STAT") & " " & gpsRM.GetString("psAll_ROBOTS")
                    Case eCntrType.APP_CNTR_AA
                        lblFanFBCap.Text = Applicator.ParamUnits(eParamID.Atom)
                        lblFanCap.Text = gpsRM.GetString("psSHAPING_AIR")
                        ftbActualFan.Visible = True
                        ftbActualFan2.Visible = True
                        ftbFan2.Visible = True
                        lblFanFBCap.Visible = True
                        lblFan1Cap.Text = gpsRM.GetString("psDQ")
                        lblFan1Cap.Visible = True
                        lblFan2Cap.Text = gpsRM.GetString("psDQ2")
                        lblFan2Cap.Visible = True
                        lblFan2ActCap.Text = grsRM.GetString("rsSA_UNITS")
                        lblFan2ActCap.Visible = True
                        btnDQCal.Visible = True
                        btnDQ2Cal.Visible = True
                        gpbCalibration.Visible = True
                        mnuViewSelectDQCalStat.Visible = True
                        mnuViewSelectDQCalStat2.Visible = True
                        mnuViewSelectDQCalStat.Text = lblFan1Cap.Text & " " & gpsRM.GetString("psCAL_STAT") & " " & gpsRM.GetString("psAll_ROBOTS")
                        mnuViewSelectDQCalStat2.Text = lblFan2Cap.Text & " " & gpsRM.GetString("psCAL_STAT") & " " & gpsRM.GetString("psAll_ROBOTS")
                    Case Else
                        ftbActualAtom.Visible = False
                        lblAtomFBCap.Visible = False
                End Select
                Select Case Applicator.CntrType(eParamID.Fan)
                    Case eCntrType.APP_CNTR_BS, eCntrType.APP_CNTR_TS
                        lblAtomFBCap.Text = Applicator.ParamUnits(eParamID.Fan)
                        lblFanCap.Text = gpsRM.GetString("psSHAPING_AIR")
                        ftbActualAtom.Visible = True
                        lblAtomFBCap.Visible = True
                    Case eCntrType.APP_CNTR_DQ
                        btnDQCal.Text = gpsRM.GetString("psDQ") & " " & gpsRM.GetString("psCAL")
                        lblFanFBCap.Text = grsRM.GetString("rsPSI")
                        lblFanCap.Text = gpsRM.GetString("psSHAPING_AIR")
                        ftbActualFan.Visible = True
                        ftbActualFan2.Visible = False
                        lblFanFBCap.Visible = True
                        btnDQCal.Visible = True
                        gpbCalibration.Visible = True
                        mnuViewSelectDQCalStat.Visible = True
                        lblFan1Cap.Visible = False
                        If Applicator.NumParms <= eParamID.Fan2 Then
                            lblFan2Cap.Visible = False
                            lblFan2ActCap.Visible = False
                            mnuViewSelectDQCalStat2.Visible = False
                            btnDQ2Cal.Visible = False
                            ftbActualFan2.Visible = False
                            ftbFan2.Visible = False
                        End If
                        mnuViewSelectDQCalStat.Text = lblFanCap.Text & " " & gpsRM.GetString("psCAL_STAT") & " " & gpsRM.GetString("psAll_ROBOTS")
                    Case eCntrType.APP_CNTR_AA
                        lblFanFBCap.Text = Applicator.ParamUnits(eParamID.Fan)
                        lblFanCap.Text = gpsRM.GetString("psSHAPING_AIR")
                        ftbActualFan.Visible = True
                        lblFanFBCap.Visible = True
                        btnDQCal.Visible = True
                        gpbCalibration.Visible = True
                        mnuViewSelectDQCalStat.Visible = True
                        lblFan1Cap.Text = gpsRM.GetString("psDQ")
                        lblFan1Cap.Visible = True
                        If Applicator.NumParms <= eParamID.Fan2 Then
                            lblFan2Cap.Visible = False
                            lblFan2ActCap.Visible = False
                            mnuViewSelectDQCalStat2.Visible = False
                            btnDQ2Cal.Visible = False
                            ftbActualFan2.Visible = False
                            ftbFan2.Visible = False
                        End If
                        mnuViewSelectDQCalStat.Text = lblFan1Cap.Text & " " & gpsRM.GetString("psCAL_STAT") & " " & gpsRM.GetString("psAll_ROBOTS")
                    Case Else
                        ftbActualFan.Visible = False
                        ftbFan2.Visible = False
                        ftbActualFan2.Visible = False
                        lblFanFBCap.Visible = False
                        lblFan1Cap.Visible = False
                End Select
                If Applicator.NumParms > eParamID.Fan2 Then
                    Select Case Applicator.CntrType(eParamID.Fan2)
                        Case eCntrType.APP_CNTR_BS, eCntrType.APP_CNTR_TS
                            lblAtomFBCap.Text = Applicator.ParamUnits(eParamID.Fan2)
                            lblFanCap.Text = gpsRM.GetString("psSHAPING_AIR")
                            ftbActualAtom.Visible = True
                            lblAtomFBCap.Visible = True
                        Case eCntrType.APP_CNTR_DQ
                            btnDQCal.Text = gpsRM.GetString("psDQ") & " " & gpsRM.GetString("psCAL")
                            lblFanFBCap.Text = grsRM.GetString("rsPSI")
                            lblFanCap.Text = gpsRM.GetString("psSHAPING_AIR")
                            ftbActualFan.Visible = True
                            ftbActualFan2.Visible = False
                            ftbFan2.Visible = False
                            lblFanFBCap.Visible = True
                            btnDQCal.Visible = True
                            btnDQ2Cal.Visible = False
                            gpbCalibration.Visible = True
                            mnuViewSelectDQCalStat.Visible = True
                            lblFan1Cap.Visible = False
                            lblFan2Cap.Text = gpsRM.GetString("psDQ2")
                            lblFan2Cap.Visible = False
                            lblFan2ActCap.Text = gpsRM.GetString("psDQ2")
                            lblFan2ActCap.Visible = False
                            mnuViewSelectDQCalStat.Text = lblFanCap.Text & " " & gpsRM.GetString("psCAL_STAT") & " " & gpsRM.GetString("psAll_ROBOTS")
                        Case eCntrType.APP_CNTR_AA
                            lblFanFBCap.Text = Applicator.ParamUnits(eParamID.Fan2)
                            lblFanCap.Text = gpsRM.GetString("psSHAPING_AIR")
                            ftbActualFan.Visible = True
                            ftbActualFan2.Visible = True
                            ftbFan2.Visible = True
                            lblFanFBCap.Visible = True
                            btnDQCal.Visible = True
                            btnDQ2Cal.Visible = True
                            gpbCalibration.Visible = True
                            mnuViewSelectDQCalStat.Visible = True
                            mnuViewSelectDQCalStat2.Visible = True
                            lblFan1Cap.Text = gpsRM.GetString("psDQ")
                            lblFan1Cap.Visible = True
                            lblFan2Cap.Text = gpsRM.GetString("psDQ2")
                            lblFan2Cap.Visible = True
                            lblFan2ActCap.Text = Applicator.ParamUnits(eParamID.Fan2)
                            lblFan2ActCap.Visible = True
                            mnuViewSelectDQCalStat.Text = lblFan1Cap.Text & " " & gpsRM.GetString("psCAL_STAT") & " " & gpsRM.GetString("psAll_ROBOTS")
                            mnuViewSelectDQCalStat2.Text = lblFan2Cap.Text & " " & gpsRM.GetString("psCAL_STAT") & " " & gpsRM.GetString("psAll_ROBOTS")
                        Case Else
                            'ftbActualFan.Visible = False
                            ftbFan2.Visible = False
                            ftbActualFan2.Visible = False
                            'lblFanFBCap.Visible = False
                            lblFan1Cap.Visible = False

                    End Select
                End If
                'Setup pop-up menu for detail views
                mnuViewSelectPaint.Text = Applicator.ParamName(eParamID.Flow) & " " & gpsRM.GetString("psAll_ROBOTS")
                mnuViewSelectAA_BS.Text = Applicator.ParamName(eParamID.Atom) & " " & gpsRM.GetString("psAll_ROBOTS")
                mnuViewSelectFA_SA.Text = lblFanCap.Text & " " & gpsRM.GetString("psAll_ROBOTS")
                mnuViewSelectEstat.Text = Applicator.ParamName(eParamID.Estat) & " " & gpsRM.GetString("psAll_ROBOTS")
                mnuViewSelectCalStat.Text = gpsRM.GetString("psCAL_STAT") & " " & gpsRM.GetString("psAll_ROBOTS")
                mnuViewSelectDockCalStat.Text = gpsRM.GetString("psDOCK_CAL_STAT") & " " & gpsRM.GetString("psAll_ROBOTS")
                mnuViewSelectDQCalStat.Text = lblFanCap.Text & " " & gpsRM.GetString("psCAL_STAT") & " " & gpsRM.GetString("psAll_ROBOTS")

                mnuViewSelectDockCalStat.Visible = False
                gpbDockControl.Visible = False
                gpbPumpEnables.Visible = False
                gpbTPR.Visible = False

                Select Case Applicator.CntrType(eParamID.Flow)
                    Case eCntrType.APP_CNTR_AS  'Accustat, Accuair
                        'Special case for accustat "Actual flow" becomes "Paint in Can"
                        lblActualCap.Text = gpsRM.GetString("psPAINTINCAN")
                        'Accustat gets both cal buttons
                        btnAutoCal.Visible = True
                        btnPressTest.Visible = True
                        gpbCalibration.Visible = True
                        'Cal status view on the pop-up menu
                        mnuViewSelectCalStat.Visible = True
                    Case eCntrType.APP_CNTR_AF
                        'Normal "Actual Flow" label
                        lblActualCap.Text = gpsRM.GetString("psACTUAL")
                        btnAutoCal.Visible = False
                        btnPressTest.Visible = True
                        gpbCalibration.Visible = True
                        'Cal status view on the pop-up menu
                        mnuViewSelectCalStat.Visible = True
                    Case Else
                        '12/03/09 MSW Pressure Test Change
                        'IPC cntr type didn't cooperate, so just check the CC type here
                        Select Case Applicator.ColorChangeType
                            Case eColorChangeType.VERSABELL, _
                            eColorChangeType.HONDA_1K, eColorChangeType.VERSABELL2, _
                            eColorChangeType.VERSABELL2_PLUS, eColorChangeType.VERSABELL2_32, _
                            eColorChangeType.VERSABELL3
                                'Normal "Actual Flow" label
                                lblActualCap.Text = gpsRM.GetString("psACTUAL")
                                btnAutoCal.Visible = False
                                ' 11/30/09  MSW     Set up the pressure test (add psPRESSURE_TEST to proj.resx)
                                'use paint cal button for pressure test
                                'btnPressTest.Visible = True
                                'btnPressTest.Text = gpsRM.GetString("psPRESS_TEST")
                                btnPressTest.Visible = False
                                'Cal status view on the pop-up menu
                                mnuViewSelectPressureTest.Visible = True
                                mnuViewSelectCalStat.Visible = True
                                gpbCalibration.Visible = True
                            Case eColorChangeType.SERVOBELL, eColorChangeType.VERSABELL2_PLUS_WB, _
                                    eColorChangeType.VERSABELL2_WB, eColorChangeType.VERSABELL3_WB, _
                                    eColorChangeType.VERSABELL3_DUAL_WB
                                'Normal "Actual Flow" label
                                lblActualCap.Text = gpsRM.GetString("psACTUAL")
                                btnPressTest.Visible = False
                                'Cal status view on the pop-up menu
                                mnuViewSelectCalStat.Visible = False
                                'Scale cal button for servobell, integrated canister
                                btnAutoCal.Visible = True
                                gpbCalibration.Visible = True
                                'Cal status view on the pop-up menu
                                mnuViewSelectCalStat.Visible = True
                                'TPR
                                gpbTPR.Visible = True
                            Case eColorChangeType.HONDA_WB
                                'Normal "Actual Flow" label
                                lblActualCap.Text = gpsRM.GetString("psACTUAL")
                                btnPressTest.Visible = True
                                btnPressTest.Text = gpsRM.GetString("psSUNIT_CAL")
                                'Scale cal button for servobell, integrated canister
                                btnAutoCal.Visible = True
                                btnAutoCal.Text = gpsRM.GetString("psCAN_CAL")
                                gpbCalibration.Visible = True
                                'Cal status view on the pop-up menu
                                mnuViewSelectCalStat.Visible = True
                                'S-Unit Cal status view on the pop-up menu
                                mnuViewSelectDockCalStat.Visible = True
                                'Dock control group box
                                gpbDockControl.Visible = True
                                gpbDockControl.Location = gpbPumpEnables.Location
                                btnMoveClean.Text = gpsRM.GetString("psMOVE_CLEAN")
                                btnMoveDeDock.Text = gpsRM.GetString("psMOVE_DEDOCK")
                                btnMoveDock.Text = gpsRM.GetString("psMOVE_DOCK")
                                'TPR
                                gpbTPR.Visible = True
                            Case eColorChangeType.BELL_2K, eColorChangeType.GUN_2K, eColorChangeType.VERSABELL_2K, _
                                 eColorChangeType.VERSABELL2_2K, eColorChangeType.VERSABELL2_2K_1PLUS1, eColorChangeType.VERSABELL2_2K_MULTIRESIN, _
                                 eColorChangeType.GUN_2K_PIG, eColorChangeType.GUN_2K_Mica
                                gpbPumpEnables.Visible = True
                                'Normal "Actual Flow" label
                                lblActualCap.Text = gpsRM.GetString("psACTUAL")
                                btnAutoCal.Visible = False
                                'btnAutoCal.Text = gpsRM.GetString("psPAINT_CAL")
                                ' 11/30/09  MSW     Set up the pressure test (add psPRESSURE_TEST to proj.resx)
                                'use paint cal button for pressure test
                                'btnPressTest.Visible = True
                                'btnPressTest.Text = gpsRM.GetString("psPRESS_TEST")
                                btnPressTest.Visible = False
                                'Cal status view on the pop-up menu
                                mnuViewSelectPressureTest.Visible = True
                                mnuViewSelectCalStat.Visible = True
                                gpbCalibration.Visible = False  'JZ 12122016
                            Case Else
                                'Normal "Actual Flow" label
                                lblActualCap.Text = gpsRM.GetString("psACTUAL")
                                btnPressTest.Visible = False
                                'Cal status view on the pop-up menu
                                mnuViewSelectCalStat.Visible = False
                                btnAutoCal.Visible = False

                        End Select
                End Select
            End If

        Catch ex As Exception
            Dim lReply As Windows.Forms.DialogResult = Response.OK
            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.AbortRetryIgnore)

            Select Case lReply
                Case Response.Abort
                    Me.Cursor = System.Windows.Forms.Cursors.Default
                    End
                Case Response.Ignore
                Case Response.Retry
                    subInitializeForm()
            End Select
        End Try
    End Sub
#End Region

#Region " Data and communication "
    '********************************************************************************************
    'collections/data events
    '******************************************************************************************** 
    Private Sub colControllers_BumpProgress() Handles colControllers.BumpProgress
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
                                          Handles colControllers.ConnectionStatusChange
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

        'Check for call from the robot object thread
        If Me.stsStatus.InvokeRequired Then
            Dim dCntrStat As New ControllerStatusChange(AddressOf colControllers_ConnectionStatusChange)
            Me.BeginInvoke(dCntrStat, New Object() {Controller})
        Else
            subDoStatusBar(Controller)
        End If

        'Trace.WriteLine("frmmain connection status change event - " & Controller.Name & " " & _
        '                        Controller.RCMConnectStatus.ToString

       'Control.CheckForIllegalCrossThreadCalls = True

    End Sub
    Private Sub tmrUpdateSA_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                            Handles tmrUpdateSA.Tick
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/26/11  MSW     CC cycle disable debug
        ' 03/28/12  MSW     Modified for .NET Scattered Access                            4.01.03.00
        '********************************************************************************************

        Dim bSelected As Boolean
        Dim iCntrIndx As Integer
        Try
            tmrUpdateSA.Enabled = False
            For Each Controller As clsController In colControllers
                Try
                    iCntrIndx = colControllers.IndexOf(Controller)
                    'Update SA for this controller if we're updating everyone or
                    'one of the arms is selected
                    bSelected = False
                    If meViewSelect > eViewSelect.Toon Then
                        bSelected = True
                    Else
                        For Each Arm As clsArm In Controller.Arms
                            If Arm.Name = msArmSelected Then
                                bSelected = True
                                Exit For
                            End If
                            Dim nRobotIdx As Integer = colArms.IndexOf(Arm)
                            Dim nSelected As Integer = CInt(msZonePLCData(eZLink.RobotsSelectedWd))
                            If (nSelected And gnBitVal(nRobotIdx + 1)) <> 0 Then
                                bSelected = True
                                Exit For
                            End If
                        Next
                    End If
                    'If it's offline while we're trying to talk, through a pop-op message once
                    'update the status bar and box when it goes on or offline
                    'syam

                    If bSelected Then
                        If (Controller.Robot.IsConnected = False) Then
                            bSelected = False
                            If mbOfflineMessage(iCntrIndx) Then
                                Status = Controller.Name & gcsRM.GetString("csIS_OFFLINE")
                                lstStatus.Visible = True
                                MessageBox.Show((Controller.Name & gcsRM.GetString("csIS_OFFLINE")), _
                                   (msSCREEN_NAME & " - " & Controller.Name), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                mbOfflineMessage(iCntrIndx) = False
                                'Clean up the labels
                                For Each Arm As clsArm In Controller.Arms
                                    If Not Arm.IsOpener Then subCleanUpRobotLabels(Arm, (Arm.Name = msArmSelected))
                                Next
                            End If
                        Else
                            If mbOfflineMessage(iCntrIndx) = False Then
                                mbOfflineMessage(iCntrIndx) = True
                                Status = Controller.Name & gcsRM.GetString("csIS_ONLINE")
                                'It's about to get slow, so try to warn them
                                ' or it could already be slow, hopefully this'll pop up before they get to mad.
                                lstStatus.Visible = True
                                Application.DoEvents()
                            End If
                        End If
                    End If
                    If bSelected Then
                        For Each oArm As clsArm In Controller.Arms
                            If Not oArm.IsOpener Then 'RJO 11/24/09
                                bSelected = (oArm.Name = msArmSelected) 'update more for robot selected in cbo
                                Dim nRobotIdx As Integer = colArms.IndexOf(oArm)
                                Dim nSelected As Integer = CInt(msZonePLCData(eZLink.RobotsSelectedWd))
                                Dim bChecked As Boolean = (nSelected And gnBitVal(nRobotIdx + 1)) <> 0
                                If (meViewSelect > eViewSelect.Toon) Or (bSelected) Or (bChecked) Then
                                    Dim oSAItems As clsScatteredAccessItems = DirectCast(oArm.Tag, clsScatteredAccessItems)
                                    If oSAItems Is Nothing Then
                                        oSAItems = New clsScatteredAccessItems(gSAConfig, oArm)
                                        oArm.Tag = oSAItems
                                    End If
                                    If oSAItems IsNot Nothing Then
                                        msSAData = oSAItems.Data
                                        'Get the indexes for all the data first time in on each robot
                                        If bIndexScatteredAccess(colArms.IndexOf(oArm)) Then
                                            mPWRobotCommon.subIndexScatteredAccess(msSAData, oArm.ArmNumber, _
                                                mtScatteredAccessIndexes(colArms.IndexOf(oArm)).Indexes)
                                            bIndexScatteredAccess(colArms.IndexOf(oArm)) = False
                                        End If
                                        'Update for each arm if required
                                        subUpdateScatteredAccess(msSAData, oArm, bSelected)
                                    Else
                                        mbSAMessage(iCntrIndx) = True
                                    End If
                                Else
                                    ' 04/26/11  MSW     CC cycle disable debug
                                    mnLineState(nRobotIdx) = eLineState.ArmNotSelected
                                End If
                            End If

                    'Not set up.  Throw up a sttatus message on the first pass
                    If mbSAMessage(iCntrIndx) = True Then
                        Status = gcsRM.GetString("csSET_UP_SA_FOR") & Controller.Name
                        mbSAMessage(iCntrIndx) = False
                    End If
                        Next
                    End If
                Catch ex As Exception
                End Try
            Next
        Catch ex As Exception

        Finally
            tmrUpdateSA.Enabled = True
        End Try
    End Sub
    Private Sub subInitScatteredAccess()
        '********************************************************************************************
        'Description: 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        ' 3/22/09  GEO.    MODIFIED TO USE SA DB REFERANCE

        Try
            'New scattered access
            gSAConfig = New clsScatteredAccessGlobal
            tmrUpdateSA.Interval = gSAConfig.UpdateRatems
            'Check for SA running
            ScatteredAccessRoutines.subStartupSA()

            For Each oArm As clsArm In colArms
                Dim oSAItems As clsScatteredAccessItems = New clsScatteredAccessItems(gSAConfig, oArm)
                oArm.Tag = oSAItems
            Next

            ReDim mbOfflineMessage(colControllers.Count - 1)
            ReDim mbSAMessage(colControllers.Count - 1)
            For iTmp As Integer = LBound(mbOfflineMessage) To UBound(mbOfflineMessage)
                mbOfflineMessage(iTmp) = True
                mbSAMessage(iTmp) = True
            Next
            'Reset scattered access indexes
            ReDim mtScatteredAccessIndexes(colArms.Count)
            ReDim bIndexScatteredAccess(colArms.Count)
            For nRobot As Integer = 0 To (colArms.Count - 1)
                ReDim mtScatteredAccessIndexes(nRobot).Indexes(eSAIndex.Max)
                For Each i As Integer In mtScatteredAccessIndexes(nRobot).Indexes
                    i = -1 'Start with the index not found
                Next
                bIndexScatteredAccess(nRobot) = True
            Next
            'For Each o As clsController In colControllers
            '    Status = gcsRM.GetString("csSET_UP_SA_FOR") & o.Name
            '    If o.RCMConnectStatus = FRRobotNeighborhood.FRERNConnectionStatusConstants.frRNConnected Then
            '        ' O.NAME is the "Display name in sql data base this must match the
            '        ' Robot name used by scattered access which gets its data from MS access DB. Geo.
            '        oScatteredAccess.RobotName = o.Name
            '        If oScatteredAccess Is Nothing Then
            '            'something bad happened
            '            Debug.Assert(False)
            '        Else
            '            If oScatteredAccess.IsSetUp = False Then
            '                If oScatteredAccess.SetupInProg = "False" Then
            '                    oScatteredAccess.SetUp()
            '                    System.Windows.Forms.Application.DoEvents()
            '                End If
            '            End If
            '        End If

            '    End If
            'Next
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub

    Private Sub subCleanUpRobotLabels(ByRef Arm As clsArm, ByVal bSelected As Boolean)
        '********************************************************************************************
        'Description:  Clear out all the labels that scattered access should be writing
        '               Called when a robot goes offline
        'Parameters:    Arm - robot to update,  
        '               bSelected - True if this robot is selected in cboRobot,
        '                           if false, just update gpbViewAll
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nRobotIdx As Integer = colArms.IndexOf(Arm)
        Dim sRobNumSfx As String = Format(nRobotIdx + 1, "00")
        Dim oLbl As Label
        oLbl = DirectCast(gpbViewAll.Controls.Item("lblCalStat" & sRobNumSfx), Label)
        oLbl.Text = ""
        oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatA" & sRobNumSfx), Label)
        oLbl.Text = ""
        oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatB" & sRobNumSfx), Label)
        oLbl.Text = ""
        If bSelected Then
            ftbActualFlow.Text = ""
            ftbActualFlow.ForeColor = Color.Black
            ftbTotal.Text = ""
            ftbTotal.ForeColor = Color.Black
            ftbActualAtom.Text = ""
            ftbActualAtom.ForeColor = Color.Black
            ftbActualFan.Text = ""
            ftbActualFan.ForeColor = Color.Black
            ftbActualFan2.Text = ""
            ftbActualFan2.ForeColor = Color.Black
            ftbActualEStatKv.Text = ""
            ftbActualEStatKv.ForeColor = Color.Black
            ftbActualEstatUA.Text = ""
            ftbActualEstatUA.ForeColor = Color.Black
            txtTPR.Text = ""
            txtTPR.ForeColor = Color.Black
            lblCurrentColor.Text = ""
            lblCurrentColor.ForeColor = Color.Black
            If Not (mCCToon Is Nothing) And Not (uctrlCartoon Is Nothing) Then
                With mCCToon
                    ' Update class module with valve integer array
                    .SharedValveStates = mnSharedValvesState
                    .GroupValveStates = mnGroupValvesState
                    Select Case Arm.ColorChangeType
                        Case eColorChangeType.ACCUSTAT
                        Case eColorChangeType.SINGLE_PURGE
                        Case eColorChangeType.VERSABELL2, eColorChangeType.VERSABELL2_PLUS, _
                            eColorChangeType.VERSABELL3
                            Dim sLabels As String()
                            ReDim sLabels(4)
                            sLabels(0) = "lblOutPressure"
                            sLabels(1) = ""
                            sLabels(2) = "lblFlow"
                            sLabels(3) = ""

                            .subUpdateAdditionalParams(sLabels)
                        Case eColorChangeType.VERSABELL_2K, eColorChangeType.VERSABELL2_2K, _
                            eColorChangeType.VERSABELL2_2K_1PLUS1, eColorChangeType.VERSABELL2_2K_MULTIRESIN
                            Dim sLabels As String()
                            ReDim sLabels(10)
                            sLabels(0) = "lblOutPressure"
                            sLabels(1) = ""
                            sLabels(2) = "lblFlow"
                            sLabels(3) = ""
                            sLabels(4) = "lblOutPressure2"
                            sLabels(5) = ""
                            sLabels(6) = "lblInPressure"
                            sLabels(7) = ""
                            sLabels(8) = "lblInPressure2"
                            sLabels(9) = ""

                            .subUpdateAdditionalParams(sLabels)

                    End Select
                    ' update cartoons
                    .subUpdateValveCartoon()
                End With

            End If
        End If
    End Sub

    Private Sub subUpdateScatteredAccess(ByRef saData() As String, _
                 ByRef Arm As clsArm, ByVal bSelected As Boolean)
        '********************************************************************************************
        'Description:  Update the robot data
        '
        'Parameters:    sData - scattered access data array, 
        '               Arm - robot to update,  
        '               bSelected - True if this robot is selected in cboRobot,
        '                           if false, just update gpbViewAll
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/03/09  MSW     Pressure Test Change
        ' 04/26/11  MSW     CC cycle disable 
        ' 06/07/12  MSW     subUpdateScatteredAccess - fix cal status for accuair, add waitcursor for change log
        ' 09/24/12  BTK     Added missing eColorChangeType cases to Applicator/CC type specific labels section
        ' 10/10/12  MSW     subUpdateScatteredAccess = From Wentsville - fix inletpressure2
        ' 06/21/13  RJO     Set column header labels text based on the currently selected applicator. 
        ' 07/11/13  RJO     Apparently WB Pushed Out, Cleaned Out and Filled changed from a Boolean to an Integer.
        '********************************************************************************************

        'sData should look like this...
        '		(0,0)	"EQ1: Requested Bell Speed" {String}	Object
        '		(0,1)	0.0 {Single}	Object
        '		(1,0)	"EQ1: Requested Shaping Air" {String}	Object
        '		(1,1)	"" {String}	Object
        '		(2,0)	"EQ1: Fluid Preset Number" {String}	Object
        ' ect...

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'convert to a single dimensional array of strings...
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim sSAVal As String
        Dim sFlow As String = ""
        Dim sFlowAct As String = ""
        Dim sFlowReq As String = ""
        Dim sTotal As String = ""
        Dim sPaintInCan As String = ""
        Dim sOutletPressure As String = ""
        Dim sOutletPressure2 As String = ""
        Dim sInletPressure As String = ""
        Dim sInletPressure2 As String = ""
        Dim sCanPos As String = "0"
        Dim sCanTorque As String = "0.0"
        Dim sDockTorque As String = "0.0"
        Dim sDockPosn As String = "0"
        Dim fVal As Single
        Dim oLbl As Label
        Dim nEquipNumber As Integer = Arm.ArmNumber
        Dim nRobotIdx As Integer = colArms.IndexOf(Arm)
        Dim bLineStateChange As Boolean = False 'RJO 04/21/11

        'the SAdata should look like this....
        '		(0)	"EQ1: Requested Bell Speed"	String
        '		(1)	"0"	String
        '		(2)	"EQ1: Requested Shaping Air"	String
        '		(3)	""	String
        'ect...

        'Get the indexes for all the data first time in on each robot
        If bIndexScatteredAccess(nRobotIdx) Then
            mPWRobotCommon.subIndexScatteredAccess(saData, nEquipNumber, mtScatteredAccessIndexes(nRobotIdx).Indexes)
            bIndexScatteredAccess(nRobotIdx) = False
        End If

        'Robot number suffix for label names
        Dim sRobNumSfx As String = Format(nRobotIdx + 1, "00")
        'get the applicator type for setup details
        Dim Applicator As clsApplicator = colApplicators.Item(Arm.ColorChangeType)


        'Enter each section if that detail view is selected or it's updating the robot
        'selected in the cbo
        'Paint flow and totals
        If (meViewSelect = eViewSelect.CalStat) Or (meViewSelect = eViewSelect.Paint) Or bSelected Then
            'commanded and actual flow
            Select Case Arm.ColorChangeType
                'Flow Rate 
                Case eColorChangeType.ACCUSTAT, eColorChangeType.AQUABELL ' Paint in can for accustat
                    If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.PaintInCan) > 0 Then
                        sPaintInCan = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.PaintInCan))
                        sFlow = sPaintInCan
                    End If
                    'Setpoint, they should all have this
                    If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqFlow) > 0 Then
                        sFlowReq = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqFlow))
                    End If
                Case eColorChangeType.VERSABELL2_PLUS_WB, _
                     eColorChangeType.VERSABELL2_WB, _
                     eColorChangeType.HONDA_WB, eColorChangeType.VERSABELL3_WB, _
                     eColorChangeType.VERSABELL3_DUAL_WB
                    If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CanisterPos) > 0 Then
                        fVal = CType(saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CanisterPos)), Single)
                        sCanPos = fVal.ToString("###0")
                    End If
                    If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CanisterTorque) > 0 Then
                        fVal = CType(saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CanisterTorque)), Single)
                        sCanTorque = Math.Abs(fVal).ToString("###0.0")
                    End If
                    'Setpoint, they should all have this
                    If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqFlow) > 0 Then
                        sFlowReq = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqFlow))
                        sFlow = sFlowReq
                    End If
                    'If there's an actual flow, use it
                    If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActFlow) > 0 Then
                        sFlowAct = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActFlow))
                        sFlow = sFlowAct
                    End If
                Case Else
                    'Setpoint, they should all have this
                    If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqFlow) > 0 Then
                        sFlowReq = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqFlow))
                        sFlow = sFlowReq
                    End If
                    'If there's an actual flow, use it
                    If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActFlow) > 0 Then
                        sFlowAct = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActFlow))
                        sFlow = sFlowAct
                    End If
            End Select

            ' Total Flow
            If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.TotFlow) > 0 Then
                sTotal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.TotFlow))
                'We get really tiny totals coming in from the robot instead of 0, so round it off
                sTotal = (CInt(CType(sTotal, Single) * 10) / 10).ToString
            End If

            ' Outlet Pressure
            If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ResinOutletPressure) > 0 Then
                sOutletPressure = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ResinOutletPressure))
            End If
            If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.HardenerOutletPressure) > 0 Then
                sOutletPressure2 = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.HardenerOutletPressure))
            End If
            ' Inlet Pressure
            If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ResinInletPressure) > 0 Then
                sInletPressure = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ResinInletPressure))
            End If
            If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.HardenerInletPressure) > 0 Then
                sInletPressure2 = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.HardenerInletPressure))
            End If
        End If
        If (meViewSelect = eViewSelect.Paint) Or bSelected Then
            If meViewSelect = eViewSelect.Paint Then 'Paint list in the view all box
                'Grab the first label
                oLbl = DirectCast(gpbViewAll.Controls.Item("lblCalStat" & sRobNumSfx), Label)
                If Arm.ColorChangeType = meOldApplicator Then '06/21/13 RJO
                    'Then fill out all the labels for each flow type
                    Select Case Arm.ColorChangeType
                        Case eColorChangeType.ACCUSTAT, eColorChangeType.AQUABELL
                            'Cmd, Paint in can, Paint used
                            oLbl.Text = sFlowReq
                            oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatA" & sRobNumSfx), Label)
                            oLbl.Text = sPaintInCan
                            oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatB" & sRobNumSfx), Label)
                            oLbl.Text = sTotal
                        Case eColorChangeType.DUAL_PURGE, eColorChangeType.SINGLE_PURGE, eColorChangeType.SINGLE_PURGE_BELL
                            'Cmd, Actual, Total
                            oLbl.Text = sFlowReq
                            oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatA" & sRobNumSfx), Label)
                            oLbl.Text = sFlowAct
                            oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatB" & sRobNumSfx), Label)
                            oLbl.Text = sTotal
                            'Any other special cases, add here
                        Case eColorChangeType.HONDA_1K, eColorChangeType.VERSABELL, _
                             eColorChangeType.VERSABELL2, eColorChangeType.VERSABELL2_32, eColorChangeType.VERSABELL2_PLUS, _
                             eColorChangeType.VERSABELL3, eColorChangeType.VERSABELL3P1000
                            '12/03/09 MSW Pressure Test Change
                            'Flow, Total, outlet pressure
                            oLbl.Text = sFlow
                            lblStatACap.Text = gpsRM.GetString("psTOTAL")
                            oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatA" & sRobNumSfx), Label)
                            oLbl.Text = sTotal
                            oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatB" & sRobNumSfx), Label)
                            oLbl.Text = sOutletPressure
                        Case eColorChangeType.BELL_2K, eColorChangeType.GUN_2K, _
                             eColorChangeType.VERSABELL_2K, eColorChangeType.GUN_2K_Mica, eColorChangeType.GUN_2K_PIG, _
                             eColorChangeType.VERSABELL2_2K, eColorChangeType.VERSABELL2_2K_1PLUS1, _
                             eColorChangeType.VERSABELL2_2K_MULTIRESIN, eColorChangeType.VERSABELL2_PLUS, _
                             eColorChangeType.VERSABELL3, eColorChangeType.VERSABELL3P1000
                            '12/03/09 MSW Pressure Test Change
                            'Flow, Total, outlet pressure
                            oLbl.Text = sFlow
                            lblStatACap.Text = gpsRM.GetString("psTOTAL")
                            oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatA" & sRobNumSfx), Label)
                            oLbl.Text = sTotal
                            oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatB" & sRobNumSfx), Label)
                            oLbl.Text = sInletPressure & "/" & sOutletPressure
                            oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatC" & sRobNumSfx), Label)
                            oLbl.Text = sInletPressure2 & "/" & sOutletPressure2
                        Case eColorChangeType.VERSABELL2_PLUS_WB, _
                             eColorChangeType.VERSABELL2_WB, _
                             eColorChangeType.HONDA_WB, eColorChangeType.VERSABELL3_WB, _
                             eColorChangeType.VERSABELL3_DUAL_WB
                            'Flow, Total
                            oLbl.Text = sFlow
                            lblStatACap.Text = gpsRM.GetString("psTOTAL")
                            oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatA" & sRobNumSfx), Label)
                            oLbl.Text = sTotal
                            oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatB" & sRobNumSfx), Label)
                            oLbl.Text = sCanPos
                            oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatC" & sRobNumSfx), Label)
                            oLbl.Text = sCanTorque
                        Case Else
                            'Flow, Total
                            oLbl.Text = sFlow
                            lblStatACap.Text = gpsRM.GetString("psTOTAL")
                            oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatA" & sRobNumSfx), Label)
                            oLbl.Text = sTotal
                    End Select
                End If 'Arm.ColorChangeType = meOldApplicator
            End If
            'Update flow test box
            If bSelected Then 'cboRobot
                ftbActualFlow.Text = sFlow
                ftbActualFlow.ForeColor = Color.Black
                ftbTotal.Text = sTotal
                ftbTotal.ForeColor = Color.Black
            End If
        End If 'Paint flow and totals

        '12/03/09 MSW Pressure Test Change
        If (meViewSelect = eViewSelect.PressureTest) Then
            ' Outlet Pressure
            If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ResinOutletPressure) > 0 Then
                sOutletPressure = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ResinOutletPressure))
            End If
            'Init module vars if needed
            If (msPressureTestText Is Nothing) OrElse (nRobotIdx > msPressureTestText.GetUpperBound(0)) Then
                ReDim msPressureTestText(colArms.Count)
                ReDim msPressureTestSolv(colArms.Count)
                ReDim msPressureTestAir(colArms.Count)
                For nIdx As Integer = 0 To colArms.Count
                    msPressureTestText(nIdx) = String.Empty
                    msPressureTestSolv(nIdx) = String.Empty
                    msPressureTestAir(nIdx) = String.Empty
                Next
            End If
            Select Case mePressureTestStep
                Case ePressureTestStep.MeasureSolv
                    msPressureTestText(nRobotIdx) = gpsRM.GetString("psPT_FILL_SOLV") & sOutletPressure & _
                        gpsRM.GetString("psPT_PSI")
                Case ePressureTestStep.FillAir
                    msPressureTestText(nRobotIdx) = gpsRM.GetString("psPT_MEAS_SOLV") & sOutletPressure & _
                        gpsRM.GetString("psPT_PSI")
                    msPressureTestSolv(nRobotIdx) = sOutletPressure
                Case ePressureTestStep.MeasureAir
                    msPressureTestText(nRobotIdx) = gpsRM.GetString("psPT_SOLV_PRES") & _
                        msPressureTestSolv(nRobotIdx) & gpsRM.GetString("psPT_FILL_AIR") & sOutletPressure & _
                        gpsRM.GetString("psPT_PSI")
                Case ePressureTestStep.Finish
                    msPressureTestText(nRobotIdx) = gpsRM.GetString("psPT_SOLV_PRES") & _
                        msPressureTestSolv(nRobotIdx) & gpsRM.GetString("psPT_MEAS_AIR") & sOutletPressure & _
                        gpsRM.GetString("psPT_PSI")
                    msPressureTestAir(nRobotIdx) = sOutletPressure
                Case Else
                    If (msPressureTestSolv(nRobotIdx) = String.Empty) Or (msPressureTestAir(nRobotIdx) = String.Empty) Then
                        msPressureTestText(nRobotIdx) = String.Empty
                    Else
                        msPressureTestText(nRobotIdx) = gpsRM.GetString("psPT_SOLV_PRES") & _
                            msPressureTestSolv(nRobotIdx) & gpsRM.GetString("psPT_AIR_PRES") & _
                            msPressureTestAir(nRobotIdx) & _
                        gpsRM.GetString("psPT_PSI")
                    End If
            End Select
            oLbl = DirectCast(gpbViewAll.Controls.Item("lblCalStat" & sRobNumSfx), Label)
            oLbl.Text = msPressureTestText(nRobotIdx)
        End If

        'Atomizing air or bell speed
        If (meViewSelect = eViewSelect.AA_BS) Or bSelected Then
            If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActBSAA) > 0 Then 'Feedback
                sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActBSAA))
                fVal = CSng(sSAVal)
                If Applicator.CntrType(eParamID.Atom) = eCntrType.APP_CNTR_DQ Then
                    'This is a DQ feedback, scale it
                    fVal = (fVal - Applicator.DQOffset) * Applicator.DQScale
                    If fVal < 0 Then
                        fVal = 0
                    End If
                End If
                sSAVal = fVal.ToString("##0.0")
                If bSelected Then 'flow test box
                    ftbActualAtom.Text = sSAVal
                    ftbActualAtom.ForeColor = Color.Black
                End If
                If (meViewSelect = eViewSelect.AA_BS) Then 'detail view AA or BS selected
                    oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatA" & sRobNumSfx), Label)
                    oLbl.Text = sSAVal
                End If
            End If
            'Requested - detail view only
            If (mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqBSAA) > 0) And (meViewSelect = eViewSelect.AA_BS) Then
                sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqBSAA))
                oLbl = DirectCast(gpbViewAll.Controls.Item("lblCalStat" & sRobNumSfx), Label)
                oLbl.Text = sSAVal
            End If

        End If 'Atomizing air or bell speed

        'Fan or shaping air
        If (meViewSelect = eViewSelect.FA_SA) Or bSelected Then
            If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActSAFA) > 0 Then 'Feedback
                sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActSAFA))
                If Applicator.CntrType(eParamID.Fan) = eCntrType.APP_CNTR_DQ Then
                    'This is a DQ feedback, scale it
                    fVal = CSng(sSAVal)
                    fVal = (fVal - Applicator.DQOffset) * Applicator.DQScale
                    If fVal < 0 Then
                        fVal = 0
                    End If
                    sSAVal = fVal.ToString("##0.0")
                End If
                If Applicator.CntrType(eParamID.Fan) = eCntrType.APP_CNTR_AA Then
                    'This is a AA feedback, scale it
                    fVal = CSng(sSAVal)
                    sSAVal = fVal.ToString("##0.0")
                End If
                If bSelected Then 'flow test box
                    ftbActualFan.Text = sSAVal
                    ftbActualFan.ForeColor = Color.Black
                End If
                If (meViewSelect = eViewSelect.FA_SA) Then 'Fan or shaping selected in detail view
                    oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatA" & sRobNumSfx), Label)
                    oLbl.Text = sSAVal
                End If
            End If
            'Requested - detail view only
            If (mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqSAFA) > 0) And (meViewSelect = eViewSelect.FA_SA) Then
                sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqSAFA))
                oLbl = DirectCast(gpbViewAll.Controls.Item("lblCalStat" & sRobNumSfx), Label)
                oLbl.Text = sSAVal
            End If
            'Fan or shaping air 2
            If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActSAFA2) > 0 Then 'Feedback
                sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActSAFA2))
                If Applicator.CntrType(eParamID.Fan) = eCntrType.APP_CNTR_DQ Then
                    'This is a DQ2 feedback, scale it
                    fVal = CSng(sSAVal)
                    fVal = (fVal - Applicator.DQOffset) * Applicator.DQScale
                    If fVal < 0 Then
                        fVal = 0
                    End If
                    sSAVal = fVal.ToString("##0.0")
                End If
                If Applicator.CntrType(eParamID.Fan) = eCntrType.APP_CNTR_AA Then
                    fVal = CSng(sSAVal)
                    sSAVal = fVal.ToString("##0")
                End If
                If bSelected Then 'flow test box
                    ftbActualFan2.Text = sSAVal
                    ftbActualFan2.ForeColor = Color.Black
                End If
                If (meViewSelect = eViewSelect.FA_SA) Then 'Fan or shaping selected in detail view
                    oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatC" & sRobNumSfx), Label)
                    oLbl.Text = sSAVal
                End If
            End If
            'Requested - detail view only
            If (mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqSAFA2) > 0) And (meViewSelect = eViewSelect.FA_SA) Then
                sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqSAFA2))
                oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatB" & sRobNumSfx), Label)
                oLbl.Text = sSAVal
            End If
        End If 'Fan or shaping air

        'Estats
        If (meViewSelect = eViewSelect.Estat) Or bSelected Then

            'Estat KV
            If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActKV) > 0 Then
                sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActKV))
                fVal = CSng(sSAVal)
                fVal = (fVal - Applicator.KVOffset) * Applicator.KVScale
                If fVal < 0 Then
                    fVal = 0
                End If
                sSAVal = fVal.ToString("##0")
                If bSelected Then 'flow test box
                    ftbActualEStatKv.Text = sSAVal
                    ftbActualEStatKv.ForeColor = Color.Black
                End If
                If (meViewSelect = eViewSelect.Estat) Then 'detail view
                    oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatA" & sRobNumSfx), Label)
                    oLbl.Text = sSAVal
                End If

            End If

            'Estat uA
            If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActUA) > 0 Then
                sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActUA))
                fVal = CSng(sSAVal)
                fVal = (fVal - Applicator.uAOffset) * Applicator.uAScale
                sSAVal = fVal.ToString("##0")
                If bSelected Then 'flow test box
                    ftbActualEstatUA.Text = sSAVal
                    ftbActualEstatUA.ForeColor = Color.Black
                End If
                If (meViewSelect = eViewSelect.Estat) Then 'detail view
                    oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatB" & sRobNumSfx), Label)
                    oLbl.Text = sSAVal
                End If
            End If
            'requested - detail view only
            If (mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqES) > 0) And (meViewSelect = eViewSelect.Estat) Then
                sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ReqES))
                oLbl = DirectCast(gpbViewAll.Controls.Item("lblCalStat" & sRobNumSfx), Label)
                oLbl.Text = sSAVal
            End If
        End If 'Estats

        'Cal status selected
        If (meViewSelect = eViewSelect.CalStat) Then
            oLbl = DirectCast(gpbViewAll.Controls.Item("lblCalStat" & sRobNumSfx), Label)
            Select Case Arm.ColorChangeType
                Case eColorChangeType.VERSABELL2_WB, eColorChangeType.HONDA_WB, eColorChangeType.VERSABELL2_PLUS_WB, _
                        eColorChangeType.VERSABELL3_WB, eColorChangeType.VERSABELL3_DUAL_WB
                    'Scale Cal/Canister Cal status
                    'Check for cal active output
                    Dim bTmp As Boolean = False
                    If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ICCalActive) > 0 Then
                        sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ICCalActive))
                        bTmp = CInt(sSAVal) > 0
                    End If
                    If bTmp Then
                        sSAVal = gpsRM.GetString("psCALIBRATING") & " " & _
                            sCanPos & grsRM.GetString("rsCCS") & " " & _
                            sCanTorque & gpsRM.GetString("psAMPS")
                        oLbl.Text = sSAVal
                        mbShowCalActive(nRobotIdx) = True
                    Else
                        'If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ICCalStatus) > 0 Then
                        '    sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ICCalStatus))
                        '    If CInt(sSAVal) = 0 Then
                        '        oLbl.Text = gpsRM.GetString("psCALDONE")
                        '    Else
                        '        'Could use an error message lookup, should be able to use the alarm database
                        '        oLbl.Text = gpsRM.GetString("psCAL_NOT_DONE")
                        '    End If
                        'Else
                        '    oLbl.Text = "???"
                        'End If
                        If mbShowCalActive(nRobotIdx) Then
                            mbShowCalActive(nRobotIdx) = False
                            Dim oRobot As FRRobot.FRCRobot = Arm.Controller.Robot
                            Dim nAlarm As Integer = -1
                            Dim nExit As Integer = oRobot.Alarms.Count
                            For nAlarmIdx As Integer = 0 To oRobot.Alarms.Count - 1
                                If oRobot.Alarms.Item(nAlarmIdx).ErrorFacility = nPNT1 Then
                                    Dim nTmpAlarm As Integer = oRobot.Alarms.Item(nAlarmIdx).ErrorNumber
                                    If ((nTmpAlarm = nIC_CAL_ABORT_Eq1) And (Arm.ArmNumber = 1)) Or _
                                       ((nTmpAlarm = nIC_CAL_ABORT_Eq2) And (Arm.ArmNumber = 2)) Then
                                        nAlarm = nTmpAlarm
                                        nExit = nAlarmIdx + 5 ' save "aborted" in case it's alone, but keep looking for a few
                                        msPntCalStatus(nRobotIdx) = oRobot.Alarms.Item(nAlarmIdx).ErrorMessage
                                    ElseIf ((nTmpAlarm = nIC_CAL_SUCCESS_Eq1) And (Arm.ArmNumber = 1)) Or _
                                       ((nTmpAlarm = nIC_CAL_SUCCESS_Eq2) And (Arm.ArmNumber = 2)) Then
                                        nAlarm = nTmpAlarm
                                        nExit = nAlarmIdx ' Success, take it and go
                                        msPntCalStatus(nRobotIdx) = oRobot.Alarms.Item(nAlarmIdx).ErrorMessage
                                    ElseIf ((nTmpAlarm >= nIC_CAL_MSG_LOW_Eq1) And (nTmpAlarm <= nIC_CAL_MSG_HIGH_Eq1) And _
                                                (Arm.ArmNumber = 1)) Or _
                                      ((nTmpAlarm >= nIC_CAL_MSG_LOW_Eq2) And (nTmpAlarm <= nIC_CAL_MSG_HIGH_Eq2) And _
                                                (Arm.ArmNumber = 1)) Then
                                        nAlarm = nTmpAlarm
                                        nExit = nAlarmIdx
                                        msPntCalStatus(nRobotIdx) = oRobot.Alarms.Item(nAlarmIdx).ErrorMessage
                                    End If
                                End If
                                If nExit = nAlarmIdx Then Exit For
                            Next 'nAlarmIdx
                            'todo - switch over to PC sourced strings for multilanguage
                            mManFlowCommon.UpdateChangeLog(gpsRM.GetString("psPERFORMED_SCALE_CAL") & ", " & _
                                    msPntCalStatus(nRobotIdx), cboColor.Text, Arm.Name, colZones.CurrentZoneNumber, colZones.ActiveZone.Name)
                            'mPWCommon.SaveToChangeLog(colZones.DatabasePath)
                            mPWCommon.SaveToChangeLog(colZones.ActiveZone)
                            Me.Cursor = Cursors.Default
                            Application.DoEvents()

                            oLbl.Text = msPntCalStatus(nRobotIdx)
                        End If
                    End If
                    'btnAutoCal.Enabled = Not mbShowCalActive(nRobotIdx)

                Case eColorChangeType.ACCUSTAT, eColorChangeType.AQUABELL
                Case eColorChangeType.BELL_2K, eColorChangeType.GUN_2K, eColorChangeType.VERSABELL_2K, _
                     eColorChangeType.VERSABELL2_2K, eColorChangeType.VERSABELL2_2K_1PLUS1, _
                     eColorChangeType.VERSABELL2_2K_MULTIRESIN, eColorChangeType.GUN_2K_PIG, eColorChangeType.GUN_2K_Mica
                    'Scale Cal/Canister Cal status
                    'Check for cal active output
                    Dim bTmp As Boolean = False
                    If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.IPCCalActive) > 0 Then
                        sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.IPCCalActive))
                        bTmp = CInt(sSAVal) > 0
                    End If
                    If bTmp Then
                        'oLbl.Text = gpsRM.GetString("psCALIBRATING")
                        '04/04/11 ipc  AL STAT
                        'Flow, pressure
                        oLbl.Text = gpsRM.GetString("psCALIBRATING") & ": " & sFlow & " " & gpsRM.GetString("psCCMIN") & _
                            gpsRM.GetString("psP1") & sInletPressure & "/" & sOutletPressure & " " & gpsRM.GetString("psPSI") & _
                            gpsRM.GetString("psP2") & sInletPressure & "/" & sOutletPressure2 & " " & gpsRM.GetString("psPSI")
                        mbShowCalActive(nRobotIdx) = True
                    Else
                        If mbShowCalActive(nRobotIdx) Then
                            mbShowCalActive(nRobotIdx) = False
                            Dim oRobot As FRRobot.FRCRobot = Arm.Controller.Robot
                            Dim nAlarm As Integer = -1
                            Dim nExit As Integer = oRobot.Alarms.Count
                            For nAlarmIdx As Integer = 0 To oRobot.Alarms.Count - 1
                                If oRobot.Alarms.Item(nAlarmIdx).ErrorFacility = nPNT1 Then
                                    Dim nTmpAlarm As Integer = oRobot.Alarms.Item(nAlarmIdx).ErrorNumber
                                    If ((nTmpAlarm = nIPC_CAL_ABORT_Eq1) And (Arm.ArmNumber = 1)) Or _
                                       ((nTmpAlarm = nIPC_CAL_ABORT_Eq2) And (Arm.ArmNumber = 2)) Then
                                        nAlarm = nTmpAlarm
                                        nExit = nAlarmIdx + 5 ' save "aborted" in case it's alone, but keep looking for a few
                                        msPntCalStatus(nRobotIdx) = oRobot.Alarms.Item(nAlarmIdx).ErrorMessage
                                    ElseIf ((nTmpAlarm = nIPC_CAL_SUCCESS_Eq1) And (Arm.ArmNumber = 1)) Or _
                                       ((nTmpAlarm = nIPC_CAL_SUCCESS_Eq2) And (Arm.ArmNumber = 2)) Then
                                        nAlarm = nTmpAlarm
                                        nExit = nAlarmIdx ' Success, take it and go
                                        msPntCalStatus(nRobotIdx) = oRobot.Alarms.Item(nAlarmIdx).ErrorMessage
                                    ElseIf ((nTmpAlarm >= nIPC_CAL_ABORT_Eq1) And (nTmpAlarm <= nIPC_CAL_0_PSI_Eq1) And _
                                                (Arm.ArmNumber = 1)) Or _
                                      ((nTmpAlarm >= nIPC_CAL_ABORT_Eq2) And (nTmpAlarm <= nIPC_CAL_0_PSI_Eq2) And _
                                                (Arm.ArmNumber = 1)) Then
                                        nAlarm = nTmpAlarm
                                        nExit = nAlarmIdx
                                        msPntCalStatus(nRobotIdx) = oRobot.Alarms.Item(nAlarmIdx).ErrorMessage
                                    End If
                                End If
                                If nExit = nAlarmIdx Then Exit For
                            Next 'nAlarmIdx
                            'todo - switch over to PC sourced strings for multilanguage
                            mManFlowCommon.UpdateChangeLog(gpsRM.GetString("psPERFORMED_AUTO_CAL") & ", " & _
                                    msPntCalStatus(nRobotIdx), cboColor.Text, Arm.Name, colZones.CurrentZoneNumber, colZones.ActiveZone.Name)
                            'mPWCommon.SaveToChangeLog(colZones.DatabasePath)
                            mPWCommon.SaveToChangeLog(colZones.ActiveZone)
                            oLbl.Text = msPntCalStatus(nRobotIdx)
                        End If
                    End If
                    'btnAutoCal.Enabled = Not mbShowCalActive(nRobotIdx)
                Case Else
                    Select Case Applicator.CntrType(eParamID.Flow)
                        Case eCntrType.APP_CNTR_AF
                            Dim nTmp As Integer() = DirectCast(clbRobot.Tag, Integer())
                            If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.AFCalActive) > 0 Then
                                mbShowCalActive(nRobotIdx) = CInt(saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.AFCalActive))) > 0
                            End If
                            If mbShowCalActive(nRobotIdx) Then
                                'Cal Active
                                mnGetPntCalStatus(nRobotIdx) = 1
                                Dim sTmp As String = String.Empty
                                If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.PaintAout) > 0 Then
                                    sTmp = gpsRM.GetString("psAFLOWCAL1") & _
                                      saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.PaintAout))
                                End If
                                If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActFlow) > 0 Then
                                    sTmp = sTmp & ", " & gpsRM.GetString("psAFLOWCAL2") & _
                                      saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ActFlow)) & _
                                      Applicator.ParamUnits(eParamID.Flow)
                                End If
                                oLbl.Text = sTmp

                            Else
                                'Cal Not Active
                                Select Case mnGetPntCalStatus(nRobotIdx)
                                    Case 1
                                        '1st pass after cal active, read the status
                                        mnGetPntCalStatus(nRobotIdx) = 2
                                        msPntCalStatus(nRobotIdx) = String.Empty
                                        oLbl.Text = msPntCalStatus(nRobotIdx)
                                        Dim oRobot As FRRobot.FRCRobot = Arm.Controller.Robot
                                        Dim nAlarm As Integer = -1
                                        Dim nExit As Integer = oRobot.Alarms.Count
                                        For nAlarmIdx As Integer = 0 To oRobot.Alarms.Count - 1
                                            If oRobot.Alarms.Item(nAlarmIdx).ErrorFacility = nPNT1 Then
                                                Select Case oRobot.Alarms.Item(nAlarmIdx).ErrorNumber
                                                    Case nAF_CAL_SUCCESS_Eq1, nAF_CAL_LOWRST_Eq1, _
                                                       nAF_CAL_HIRST_Eq1, nAF_CAL_HI_TO_Eq1, nAF_CAL_LO_TO_Eq1, _
                                                       nAF_CAL_0_DET_Eq1, nAF_CAL_NONINC_Eq1, nAF_CAL_TU_TO_Eq1, nAF_CAL_BADMID_Eq1
                                                        nAlarm = oRobot.Alarms.Item(nAlarmIdx).ErrorNumber
                                                        nExit = nAlarmIdx
                                                        msPntCalStatus(nRobotIdx) = oRobot.Alarms.Item(nAlarmIdx).ErrorMessage
                                                    Case nAF_CAL_ABORT_Eq1
                                                        nAlarm = nSAFA_CAL_ABORT_Eq1
                                                        nExit = nAlarmIdx + 5 ' save "aborted" in case it's alone, but keep looking for a few
                                                        msPntCalStatus(nRobotIdx) = oRobot.Alarms.Item(nAlarmIdx).ErrorMessage
                                                    Case Else
                                                End Select 'oRobot.Alarms.Item(nAlarmIdx).ErrorNumber
                                            End If
                                            If nExit = nAlarmIdx Then Exit For
                                        Next 'nAlarmIdx
                                        'todo - switch over to PC sourced strings for multilanguage
                                        'msCalStatus(nRobotIdx) = GetCalStatusString(nAlarm)
                                        mManFlowCommon.UpdateChangeLog(gpsRM.GetString("psPERFORMED_AF_CAL") & ", " & _
                                                msPntCalStatus(nRobotIdx), cboColor.Text, Arm.Name, colZones.CurrentZoneNumber, colZones.ActiveZone.Name)
                                        'mPWCommon.SaveToChangeLog(colZones.DatabasePath)
                                        mPWCommon.SaveToChangeLog(colZones.ActiveZone)
                                        oLbl.Text = msPntCalStatus(nRobotIdx)
                                    Case 2
                                        'hold alarm message
                                        mnGetPntCalStatus(nRobotIdx) = 2
                                        oLbl.Text = msPntCalStatus(nRobotIdx)
                                    Case Else
                                        'Read robot vars
                                        oLbl.Text = String.Empty
                                        Dim oRobot As FRRobot.FRCRobot = Arm.Controller.Robot
                                        Arm.ProgramName = "PAVRDBCV"

                                        Dim oFile As FRRobot.FRCVars = Arm.ProgramVars

                                        If mnValve < 1 Then
                                            mnValve = 1
                                        End If
                                        Dim sVar As String = "COLOR_DATA"
                                        'Muiltiarm
                                        If Arm.ArmNumber > 1 Then
                                            'COLOR_DATA{Equip Num}
                                            sVar = sVar & Arm.ArmNumber.ToString
                                        End If
                                        'COLOR_DATA{Equip Num}.NODEDATA[{VALVE_NO}]
                                        'sVar = sVar & ".NODEDATA[" & nValve.ToString & "]"
                                        '01/20/10 BTK The code didn't work when sVar was defined like it is in the line
                                        'above.  Needed to break out the data.
                                        'Valve data
                                        Dim o As FRRobot.FRCVars = _
                                            DirectCast(oFile.Item(sVar), FRRobot.FRCVars)
                                        Dim oo As FRRobot.FRCVars = _
                                                DirectCast(o.Item("NODEDATA"), FRRobot.FRCVars)
                                        Dim oValveData As FRRobot.FRCVars = _
                                                    DirectCast(oo.Item(mnValve.ToString), FRRobot.FRCVars)
                                        'read from the robot once
                                        oValveData.NoRefresh = True
                                        'Cal status
                                        Dim oAF_DATA As FRRobot.FRCVars = CType(oValveData.Item("AF_DATA"), FRRobot.FRCVars)
                                        Dim oVar As FRRobot.FRCVar = CType(oAF_DATA.Item("CAL_STATUS"), FRRobot.FRCVar)
                                        Dim nCalStatus As Integer = CInt(oVar.Value)
                                        msPntCalStatus(nRobotIdx) = GetAFCalStatusText(nCalStatus)
                                        oLbl.Text = msPntCalStatus(nRobotIdx)
                                        mnGetPntCalStatus(nRobotIdx) = 2
                                End Select
                            End If
                        Case Else
                            oLbl.Text = String.Empty

                    End Select
            End Select
        End If

        'Dock Cal status selected (Honda S-Unit)
        If (meViewSelect = eViewSelect.DockCalStat) Then
            oLbl = DirectCast(gpbViewAll.Controls.Item("lblCalStat" & sRobNumSfx), Label)

            'Honda S-Unit Cal status
            'Check for cal active output
            If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ICCalActive) > 0 Then
                sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.ICCalActive))
                mbShowCalActive(nRobotIdx) = CInt(sSAVal) > 0
            End If
            If mbShowCalActive(nRobotIdx) Then
                oLbl.Text = gpsRM.GetString("psCALIBRATING")
            Else
                If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.SUnitCalStatus) > 0 Then
                    sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.SUnitCalStatus))
                    If CInt(sSAVal) = 0 Then
                        oLbl.Text = gpsRM.GetString("psCALDONE")
                    Else
                        'Could use an error message lookup, should be able to use the alarm database
                        oLbl.Text = gpsRM.GetString("psCAL_NOT_DONE")
                    End If
                Else
                    oLbl.Text = "???"
                End If
            End If
            'btnPressTest.Enabled = Not bShowCalActive
        End If

        'DQ Cal status selected
        If (meViewSelect = eViewSelect.DQCalStat) Or (meViewSelect = eViewSelect.DQCalStat2) Then

            'This section assumes a bell, only shows shaping air status
            oLbl = DirectCast(gpbViewAll.Controls.Item("lblCalStat" & sRobNumSfx), Label)
            'Check for call active output
            mbShowDQCalActive(nRobotIdx) = False
            'mbShowDQCalActive2(nRobotIdx) = False
            'If Arm.ArmNumber = 1 Then
            If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.DQCalActive) > 0 Then
                sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.DQCalActive))
                mbShowDQCalActive(nRobotIdx) = CInt(sSAVal) > 0
            End If
            'ElseIf Arm.ArmNumber = 2 Then
            '    If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.DQCalActive) > 0 Then
            '        sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.DQCalActive))
            '        mbShowDQCalActive2(nRobotIdx) = CInt(sSAVal) > 0
            '    End If
            'End If
            If (meViewSelect = eViewSelect.DQCalStat2) And (Applicator.NumParms <= eParamID.Fan2) Then
                oLbl.Text = String.Empty
            ElseIf mbShowDQCalActive(nRobotIdx) Then
                'Put together some details to show during the calibration
                Dim nOutputIndex As Integer = eSAIndex.DQOutput
                Dim nActualIndex As Integer = eSAIndex.ActSAFA
                Dim nParmIndex As Integer = eParamID.Fan
                Select Case meViewSelect
                    Case eViewSelect.DQCalStat
                        nOutputIndex = eSAIndex.DQOutput
                        nActualIndex = eSAIndex.ActSAFA
                        nParmIndex = eParamID.Fan
                        mnGetCalStatus(nRobotIdx) = 1
                    Case eViewSelect.DQCalStat2
                        nOutputIndex = eSAIndex.DQ2Output
                        nActualIndex = eSAIndex.ActSAFA2
                        nParmIndex = eParamID.Fan2
                        mnGetCalStatus2(nRobotIdx) = 1
                End Select
                Dim sCalStatus As String = ""
                If mtScatteredAccessIndexes(nRobotIdx).Indexes(nOutputIndex) > 0 Then
                    sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(nOutputIndex))
                Else
                    sSAVal = "???"
                End If
                If Applicator.CntrType(nParmIndex) = eCntrType.APP_CNTR_DQ Then
                    sCalStatus = gpsRM.GetString("psDQCALACT1") & sSAVal & gpsRM.GetString("psDQCALACT2")
                Else
                    sCalStatus = gpsRM.GetString("psSAFACALACT1") & sSAVal & gpsRM.GetString("psSAFACALACT2")
                End If
                If mtScatteredAccessIndexes(nRobotIdx).Indexes(nActualIndex) > 0 Then
                    sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(nActualIndex))
                    If (Applicator.CntrType(nParmIndex) = eCntrType.APP_CNTR_DQ) Then
                        'This is a DQ feedback, scale it
                        fVal = CSng(sSAVal)
                        fVal = (fVal - Applicator.DQOffset) * Applicator.DQScale
                        If fVal < 0 Then
                            fVal = 0
                        End If
                        sSAVal = fVal.ToString("##0.0")
                    End If
                    If (Applicator.CntrType(eParamID.Fan) = eCntrType.APP_CNTR_AA) Then
                        'This is a DQ feedback, scale it
                        fVal = CSng(sSAVal)
                        fVal = (fVal - Applicator.DQOffset) * Applicator.DQScale
                        If fVal < 0 Then
                            fVal = 0
                        End If
                        sSAVal = fVal.ToString("##0")
                    End If
                Else
                    sSAVal = "???"
                End If
                If Applicator.CntrType(nParmIndex) = eCntrType.APP_CNTR_DQ Then
                    sCalStatus = sCalStatus & sSAVal & gpsRM.GetString("psDQCALACT3")
                Else
                    sCalStatus = sCalStatus & sSAVal & " " & gpsRM.GetString("psSAFACALACT3")
                End If
                oLbl.Text = sCalStatus

            Else
                Dim nCalStatusIndex As Integer = eSAIndex.DQCalStatus
                Dim nGetCalStatus As Integer = mnGetCalStatus2(nRobotIdx)
                Dim sCalStatus As String = msCalStatus(nRobotIdx)
                Select Case meViewSelect
                    Case eViewSelect.DQCalStat
                        nCalStatusIndex = eSAIndex.DQCalStatus
                        nGetCalStatus = mnGetCalStatus(nRobotIdx)
                    Case eViewSelect.DQCalStat2
                        nCalStatusIndex = eSAIndex.DQ2CalStatus
                        nGetCalStatus = mnGetCalStatus2(nRobotIdx)
                End Select
                Select Case nGetCalStatus
                    Case 0 'read the cal status vars
                        'Cal not active, show the status
                        If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.DQCalStatus) > 0 Then
                            sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.DQCalStatus))
                            If Applicator.CntrType(eParamID.Fan) = eCntrType.APP_CNTR_AA Then
                                If CInt(sSAVal) = 3 Then
                                    oLbl.Text = gpsRM.GetString("psCALDONE")
                                Else
                                    oLbl.Text = gpsRM.GetString("psCAL_NOT_DONE")
                                End If
                            Else
                                If CInt(sSAVal) = 0 Then
                                    oLbl.Text = gpsRM.GetString("psCALDONE")
                                Else
                                    oLbl.Text = gpsRM.GetString("psCAL_NOT_DONE")
                                End If
                            End If
                        Else
                            oLbl.Text = "???"
                        End If
                    Case 1 'get the alarm from the last autocal

                        Dim oRobot As FRRobot.FRCRobot = Arm.Controller.Robot
                        Dim nAlarm As Integer = -1
                        Dim nExit As Integer = oRobot.Alarms.Count
                        For nAlarmIdx As Integer = 0 To oRobot.Alarms.Count - 1
                            If oRobot.Alarms.Item(nAlarmIdx).ErrorFacility = nPNT1 Then
                                Select Case Arm.ArmNumber
                                    Case 1
                                        Select Case oRobot.Alarms.Item(nAlarmIdx).ErrorNumber
                                            Case nSAFA_CAL_SUCCESS_Eq1, nSAFA_CAL_NON_INC_Eq1, nSAFA_CAL_ZERO_PRES_Eq1
                                                nAlarm = oRobot.Alarms.Item(nAlarmIdx).ErrorNumber
                                                nExit = nAlarmIdx
                                                sCalStatus = oRobot.Alarms.Item(nAlarmIdx).ErrorMessage
                                            Case nDQ_CAL_SUCCESS_Eq1, nDQ_CAL_NON_INC_Eq1, nDQ_CAL_ZERO_PRES_Eq1
                                                nAlarm = oRobot.Alarms.Item(nAlarmIdx).ErrorNumber
                                                nExit = nAlarmIdx
                                                sCalStatus = oRobot.Alarms.Item(nAlarmIdx).ErrorMessage
                                            Case nSAFA_CAL_ABORT_Eq1
                                                nAlarm = nSAFA_CAL_ABORT_Eq1
                                                nExit = nAlarmIdx + 5 ' save "aborted" in case it's alone, but keep looking for a few
                                                sCalStatus = oRobot.Alarms.Item(nAlarmIdx).ErrorMessage
                                            Case nDQ_CAL_ABORT_Eq1
                                                nAlarm = nDQ_CAL_ABORT_Eq1
                                                nExit = nAlarmIdx + 5 ' save "aborted" in case it's alone, but keep looking for a few
                                                sCalStatus = oRobot.Alarms.Item(nAlarmIdx).ErrorMessage
                                            Case Else
                                        End Select 'oRobot.Alarms.Item(nAlarmIdx).ErrorNumber
                                    Case 2
                                        Select Case oRobot.Alarms.Item(nAlarmIdx).ErrorNumber
                                            Case nSAFA_CAL_SUCCESS_Eq2, nSAFA_CAL_NON_INC_Eq2, nSAFA_CAL_ZERO_PRES_Eq2
                                                nAlarm = oRobot.Alarms.Item(nAlarmIdx).ErrorNumber
                                                nExit = nAlarmIdx
                                                sCalStatus = oRobot.Alarms.Item(nAlarmIdx).ErrorMessage
                                            Case nSAFA_CAL_ABORT_Eq2
                                                nAlarm = nSAFA_CAL_ABORT_Eq2
                                                nExit = nAlarmIdx + 5 ' save "aborted" in case it's alone, but keep looking for a few
                                                msCalStatus(nRobotIdx) = oRobot.Alarms.Item(nAlarmIdx).ErrorMessage
                                            Case nDQ_CAL_SUCCESS_Eq2, nDQ_CAL_NON_INC_Eq2, nDQ_CAL_ZERO_PRES_Eq2
                                                nAlarm = oRobot.Alarms.Item(nAlarmIdx).ErrorNumber
                                                nExit = nAlarmIdx
                                                sCalStatus = oRobot.Alarms.Item(nAlarmIdx).ErrorMessage
                                            Case nDQ_CAL_ABORT_Eq2
                                                nAlarm = nDQ_CAL_ABORT_Eq2
                                                nExit = nAlarmIdx + 5 ' save "aborted" in case it's alone, but keep looking for a few
                                                sCalStatus = oRobot.Alarms.Item(nAlarmIdx).ErrorMessage
                                            Case Else
                                        End Select 'oRobot.Alarms.Item(nAlarmIdx).ErrorNumber
                                    Case Else 'todo
                                End Select 'Arm.ArmNumber
                            End If
                            If nExit = nAlarmIdx Then Exit For
                        Next 'nAlarmIdx
                        'todo - switch over to PC sourced strings for multilanguage
                        'msCalStatus(nRobotIdx) = GetDQCalStatusString(nAlarm)
                        mManFlowCommon.UpdateChangeLog(gpsRM.GetString("psPERFORMED_DQ_CAL") & ", " & _
                                sCalStatus, String.Empty, Arm.Name, colZones.CurrentZoneNumber, colZones.ActiveZone.Name)
                        'mPWCommon.SaveToChangeLog(colZones.DatabasePath)
                        mPWCommon.SaveToChangeLog(colZones.ActiveZone)

                        oLbl.Text = sCalStatus
                        nGetCalStatus = 2
                        Select Case meViewSelect
                            Case eViewSelect.DQCalStat
                                nCalStatusIndex = eSAIndex.DQCalStatus
                                mnGetCalStatus(nRobotIdx) = nGetCalStatus
                                msCalStatus(nRobotIdx) = sCalStatus
                            Case eViewSelect.DQCalStat2
                                nCalStatusIndex = eSAIndex.DQ2CalStatus
                                mnGetCalStatus2(nRobotIdx) = nGetCalStatus
                                msCalStatus2(nRobotIdx) = sCalStatus
                        End Select
                    Case 2 'hold the alarm from the last autocal
                        Select Case meViewSelect
                            Case eViewSelect.DQCalStat
                                oLbl.Text = msCalStatus(nRobotIdx)
                            Case eViewSelect.DQCalStat2
                                oLbl.Text = msCalStatus2(nRobotIdx)
                        End Select
                End Select 'mnGetCalStatus(nRobotIdx)
            End If

        End If

        Dim nLoop As Integer
        For nLoop = 0 To colArms.Count - 1
            If mbShowDQCalActive(nLoop) Or mbShowCalActive(nLoop) Or mbShowCalActive2(nLoop) Then
                btnDQCal.Enabled = False
                btnDQ2Cal.Enabled = False
                btnAutoCal.Enabled = False
                btnPressTest.Enabled = False
                Exit For
            Else
                btnDQCal.Enabled = True
                btnDQ2Cal.Enabled = True
                btnAutoCal.Enabled = True
                btnPressTest.Enabled = True
            End If
        Next nLoop

        'More stuff only for the robot selected in cboRobot
        If bSelected Then
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Color change cycle box
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim oLabel As Label
            Dim i As Integer
            Dim nCurrentCycle As Integer = 0
            If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CCCycleNumber) > 0 Then
                nCurrentCycle = CInt(saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CCCycleNumber)))
            End If

            For i = 1 To 10
                If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.LastCCTime + i) > 0 Then
                    sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.LastCCTime + i))
                    oLabel = DirectCast(gpbCCCycle.Controls.Item("lblCCTime" & i.ToString("00")), Label)
                    oLabel.Text = sSAVal
                    oLabel.ForeColor = Color.Black
                    If i = nCurrentCycle Then
                        oLabel.BackColor = Color.Green
                    Else
                        oLabel.BackColor = Color.Transparent
                    End If
                    gpbCCCycle.Controls.Item("lblCCTime" & i.ToString("00")).Text = sSAVal
                    gpbCCCycle.Controls.Item("lblCCTime" & i.ToString("00")).ForeColor = Color.Black
                End If
            Next
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Current Color
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            If (mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentColorName) > 0) And _
               (mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentColor) > 0) Then
                lblCurrentColor.Text = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentColor)) & " - " & _
                                       saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentColorName))
            End If
            If (meViewSelect = eViewSelect.Toon) Then
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Color change valves 
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                ' 4 shared valves init to 0
                Dim nSharedValves() As Integer = {0, 0, 0, 0}
                'shared valve 1 TRIGGER
                If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.Trigger_Shared0) > 0 Then
                    sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.Trigger_Shared0))
                    nSharedValves(0) = CInt(sSAVal)
                End If
                'shared valve 2 Spare 1
                If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.Shared1) > 0 Then
                    sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.Shared1))
                    nSharedValves(1) = CInt(sSAVal)
                End If
                'shared valve 3 Hardner 
                If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.HE_Shared2) > 0 Then
                    sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.HE_Shared2))
                    nSharedValves(2) = CInt(sSAVal)
                End If
                'shared valve 4 Color Enable 
                If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CE_Shared3) > 0 Then
                    sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CE_Shared3))
                    nSharedValves(3) = CInt(sSAVal)
                End If

                'Color Valves
                Dim sCCValves As String = Nothing
                Dim sCCValves2 As String = Nothing
                If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CCValves1) > 0 Then
                    sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CCValves1))
                    sCCValves = sSAVal
                End If
                If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CCValves2) > 0 Then
                    sSAVal = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CCValves2))
                    sCCValves2 = sSAVal
                End If


                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'if in cartoon debug, skip the update from SA vars
                If Not (bCartoonDebugLoop) Then
                    mnSharedValvesState = nSharedValves(0) + 2 * nSharedValves(1) + 4 * nSharedValves(2) + 8 * nSharedValves(3)
                    mnGroupValvesState = CInt(sCCValves) + 65536 * CInt(sCCValves2)
                End If
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Update the fluid diagrams
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                If Not (mCCToon Is Nothing) Then
                    With mCCToon
                        ' Update class module with valve integer array
                        .SharedValveStates = mnSharedValvesState
                        .GroupValveStates = mnGroupValvesState

                        'color header label
                        sSAVal = ""
                        If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentColor) > 0 Then

                            sSAVal = gpsRM.GetString("psSYSCOL") & saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentColor))
                        End If
                        If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentColorName) > 0 Then
                            sSAVal = sSAVal & " - " & saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentColorName))
                        End If
                        If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentValve) > 0 Then
                            If sSAVal <> "" Then
                                sSAVal = sSAVal & ", "
                            End If
                            sSAVal = sSAVal & gpsRM.GetString("psVALVE") & saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CurrentValve))
                        End If
                        If sSAVal <> "" Then
                            mCCToon.PaintHeader = sSAVal
                        End If

                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        'Applicator/CC type specific labels
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        Select Case Arm.ColorChangeType
                            Case eColorChangeType.ACCUSTAT
                                'If IsNumeric(sProcess(eSAProcessVarPos.CanPosition)) Then
                                ' mAccustatToon.AdditionalParams( _
                                '             clsColorChangeCartoon.eAddParmID.CanisterPos) = _
                                ' CSng(sProcess(eSAProcessVarPos.CanPosition))
                                ' End If
                                ' If IsNumeric(sProcess(eSAProcessVarPos.PaintInCan)) Then
                                ' mAccustatToon.AdditionalParams( _
                                '             clsColorChangeCartoon.eAddParmID.PaintInCan) = _
                                ' CSng(sProcess(eSAProcessVarPos.PaintInCan))

                                ' End If
                                'ftbActualFlow.Text = sProcess(eSAProcessVarPos.Flow)
                                'ftbTotal.Text = sProcess(eSAProcessVarPos.Total)
                                'ftbActualFlow.ForeColor = Color.Black
                                'ftbTotal.ForeColor = Color.Black

                                'lblCylPos.Text = sProcess(eSAProcessVarPos.CanPosition)
                                'mAccustatToon.PaintColorName = gpsRM.GetString("psCURRENT_COLOR") _
                                '                & " " & sProcess(eSAProcessVarPos.CurrentColorName)


                            Case eColorChangeType.SINGLE_PURGE
                            Case eColorChangeType.VERSABELL2
                                Dim sLabels As String()
                                ReDim sLabels(2)
                                sLabels(0) = "lblFlow"
                                sLabels(1) = sFlow
                                .subUpdateAdditionalParams(sLabels)

                            Case eColorChangeType.VERSABELL2_PLUS, eColorChangeType.VERSABELL2_32, _
                                 eColorChangeType.HONDA_1K, eColorChangeType.VERSABELL3, eColorChangeType.VERSABELL3P1000
                                Dim sLabels As String()
                                ReDim sLabels(4)
                                sLabels(0) = "lblOutPressure"
                                sLabels(1) = sOutletPressure
                                sLabels(2) = "lblFlow"
                                sLabels(3) = sFlow
                                .subUpdateAdditionalParams(sLabels)

                            Case eColorChangeType.VERSABELL_2K, eColorChangeType.VERSABELL2_2K_MULTIRESIN, _
                            eColorChangeType.VERSABELL2_2K, eColorChangeType.VERSABELL2_2K_1PLUS1
                                Dim sLabels As String()
                                ReDim sLabels(10)
                                sLabels(0) = "lblOutPressure"
                                sLabels(1) = sOutletPressure
                                sLabels(2) = "lblFlow"
                                sLabels(3) = sFlow
                                sLabels(4) = "lblOutPressure2"
                                sLabels(5) = sOutletPressure2
                                sLabels(6) = "lblInPressure"
                                sLabels(7) = sInletPressure
                                sLabels(8) = "lblInPressure2"
                                sLabels(9) = sInletPressure2
                                .subUpdateAdditionalParams(sLabels)

                            Case eColorChangeType.VERSABELL2_PLUS_WB, _
                                 eColorChangeType.VERSABELL2_WB, eColorChangeType.VERSABELL3_WB, _
                                 eColorChangeType.VERSABELL3_DUAL_WB
                                Dim sLabels As String()
                                ReDim sLabels(6)
                                sLabels(0) = "lblFlow"
                                sLabels(1) = sFlow
                                sLabels(2) = "lblCanPos"
                                sLabels(3) = sCanPos
                                sLabels(4) = "lblCanTorque"
                                sLabels(5) = sCanTorque
                                .subUpdateAdditionalParams(sLabels)

                            Case eColorChangeType.HONDA_WB
                                Dim sLabels As String()
                                ReDim sLabels(10)

                                If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.SUnitTorque) > 0 Then
                                    fVal = CType(saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.SUnitTorque)), Single)
                                    sDockTorque = Math.Abs(fVal).ToString("###0.0")
                                End If
                                If mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.SUnitPos) > 0 Then
                                    fVal = CType(saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.SUnitPos)), Single)
                                    sDockPosn = fVal.ToString("#0")
                                    If bSelected Then mnDockPosition = CType(fVal, Integer)
                                    Select Case mnDockPosition
                                        Case 1 'Dock
                                            btnMoveDock.Enabled = False
                                            btnMoveClean.Enabled = True
                                            btnMoveDeDock.Enabled = False
                                        Case 2 'Clean
                                            btnMoveDock.Enabled = True
                                            btnMoveClean.Enabled = False
                                            btnMoveDeDock.Enabled = True
                                        Case 3 'DeDock
                                            btnMoveDock.Enabled = True
                                            btnMoveClean.Enabled = True
                                            btnMoveDeDock.Enabled = False
                                        Case Else 'unKnown
                                            btnMoveDock.Enabled = True
                                            btnMoveClean.Enabled = True
                                            btnMoveDeDock.Enabled = True
                                    End Select


                                End If

                                sLabels(0) = "lblFlow"
                                sLabels(1) = sFlow
                                sLabels(2) = "lblCanPos"
                                sLabels(3) = sCanPos
                                sLabels(4) = "lblCanTorque"
                                sLabels(5) = sCanTorque
                                sLabels(6) = "lblSUnitTorque"
                                sLabels(7) = sDockTorque
                                sLabels(8) = "lblSUnitPosition"
                                sLabels(9) = sDockPosn
                                .subUpdateAdditionalParams(sLabels)

                        End Select
                        ' update cartoons
                        .subUpdateValveCartoon()
                    End With
                End If
            End If


        End If
        If Arm.ColorChangeType = eColorChangeType.VERSABELL2_WB Or Arm.ColorChangeType = eColorChangeType.VERSABELL2_PLUS_WB Or _
            Arm.ColorChangeType = eColorChangeType.VERSABELL3_WB Or Arm.ColorChangeType = eColorChangeType.VERSABELL3_DUAL_WB Then
            'Update Array for Line State 'RJO 04/21/11
            ' 04/26/11  MSW     CC cycle disable debug
            ' bSelected is from cboRobot, this needs the clbRobot selection
            Dim nSelected As Integer = CInt(msZonePLCData(eZLink.RobotsSelectedWd))
            If ((nSelected And gnBitVal(nRobotIdx + 1)) <> 0) Then
                Dim nState As Integer = eLineState.Unknown
                Dim bKnown As Boolean = mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.PushedOut) > 0 AndAlso _
                                        mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CleanedOut) > 0 AndAlso _
                                        mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.Filled) > 0

                If bKnown Then
                    Dim sValue As String 'RJO 07/11/13 checl either True or 1. Apparently "TRUE" worked at one time, but not anymore...

                    sValue = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.PushedOut))
                    If (sValue.ToUpper = "TRUE") Or (sValue = "1") Then nState = eLineState.PushedOut

                    sValue = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.CleanedOut))
                    If (sValue.ToUpper = "TRUE") Or (sValue = "1") Then nState = eLineState.CleanedOut

                    sValue = saData(mtScatteredAccessIndexes(nRobotIdx).Indexes(eSAIndex.Filled))
                    If (sValue.ToUpper = "TRUE") Or (sValue = "1") Then nState = eLineState.Filled
                End If

                bLineStateChange = (mnLineState(nRobotIdx) <> nState)
                mnLineState(nRobotIdx) = nState
            Else
                bLineStateChange = (mnLineState(nRobotIdx) <> eLineState.ArmNotSelected)
                mnLineState(nRobotIdx) = eLineState.ArmNotSelected
            End If 'bSelected

            If bLineStateChange AndAlso cboCycle.Items.Count > 0 Then
                Call subEnableCCCycles()
            End If
        End If

    End Sub
    Private Sub mPLC_NewData(ByVal ZoneName As String, ByVal TagName As String, _
                                                    ByVal Data() As String) Handles mPLC.NewData
        '********************************************************************************************
        'Description:  Incoming Hotlink Data
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sZone As String = "Z" & colZones.CurrentZoneNumber.ToString

        Select Case TagName
            Case sZone & "ManualFuncHotlink"
                If frmDevices.Visible = False Then
                    'Turns out this will get called in the creation of the hotlink, need to init msZonePLCData
                    If msZonePLCData Is Nothing Then
                        msZonePLCData = Data
                        subUpdateZoneHotlink(Data, True)
                    Else
                        subUpdateZoneHotlink(Data, False)
                    End If
                Else
                    frmDevices.UpdateHotlinkData(Data)
                End If
                msZonePLCData = Data


                ' Give Each Robot its own Link seletect by the View Robot Combo.
            Case Else 'sZone & "FluidMaintHotLink_R" & mPLC.RobotIndex.ToString
                'NRU 161004 Do not process old zone hotlinks as robot hotlinks.  Should not happen now that I destroy old hotlinks, but just in case.
                If Strings.Right(TagName, 17) <> "ManualFuncHotlink" Then
                    Dim nRobotNumber As Integer = CType(TagName.Substring((TagName.Length - 3), 2), Integer)
                    subUpdateRobotHotlink(Data, nRobotNumber, False)
                    msRobotPLCData = Data
                End If

        End Select

    End Sub

    Private Sub subUpdateZoneHotlink(ByVal sData As String(), Optional ByVal bUpdateAll As Boolean = False)
        '********************************************************************************************
        'Description:  New Data from hotlink
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/27/09  MSW     Add some compares so we aren't calling as many routines every scan, add beaker mode
        ' 11/19/09  MSW     replace string literals with resource strings
        ' 02/11/11  MSW     Track CC status and manage enables better
        '********************************************************************************************
        Dim nOffset As Integer = 0
        Dim nWorkingWord As Integer = 0

        Try
            msZonePLCData = sData
            If msOldZonePLCData Is Nothing Then
                ReDim msOldZonePLCData(msZonePLCData.GetUpperBound(0))
                For Each sItem As String In msOldZonePLCData
                    sItem = "-1"
                Next
            End If
            Dim nRobotsSelected As Integer = CInt(msOldZonePLCData(eZLink.RobotsSelectedWd))
            If (msZonePLCData(eZLink.RobotsReadyWd) <> msOldZonePLCData(eZLink.RobotsReadyWd)) Or _
                (msZonePLCData(eZLink.RobotsSelectedWd) <> msOldZonePLCData(eZLink.RobotsSelectedWd)) Or bUpdateAll Then
                'Booth OK?
                Dim nBoothStatus As Integer = CInt(msZonePLCData(eZLink.RobotsReadyWd))
                'whos's selected

                'whos's selected
                Dim nselected As Integer = CInt(msZonePLCData(eZLink.RobotsSelectedWd))
                ' msw moved this back into the hotlink 
                ' Check the robot check boxes based on what the PLC has checked in PLC Register 
                ' Block the check event from sending data to PLC when Item Checked
                mbBlockItemCheckEvent = True ' Used in clbRobot Event
                Dim nTmp As Integer() = DirectCast(clbRobot.Tag, Integer())
                For nLoop As Integer = 0 To clbRobot.Items.Count - 1
                    If GetBitState(nselected, nTmp(nLoop)) = 0 Then
                        'Not selected
                        If clbRobot.GetItemCheckState(nLoop) <> CheckState.Unchecked Then
                            clbRobot.SetItemCheckState(nLoop, CheckState.Unchecked)
                        End If
                    Else
                        If GetBitState(nBoothStatus, nTmp(nLoop)) = 1 Then
                            'Selected
                            If clbRobot.GetItemCheckState(nLoop) <> CheckState.Checked Then
                                clbRobot.SetItemCheckState(nLoop, CheckState.Checked)
                            End If
                        Else
                            'Selected, but disabled
                            If clbRobot.GetItemCheckState(nLoop) <> CheckState.Indeterminate Then
                                clbRobot.SetItemCheckState(nLoop, CheckState.Indeterminate)
                            End If
                        End If
                    End If
                Next nLoop
                mbBlockItemCheckEvent = False
                If (nBoothStatus And gnBitVal(eZLink.BoothReadyBit)) = _
                                                    gnBitVal(eZLink.BoothReadyBit) Then
                    subEnableGroupboxes(True, (nselected <> 0))
                Else
                    subEnableGroupboxes(False)
                End If
            End If

            If (msZonePLCData(eZLink.ManualStatusWd) <> msOldZonePLCData(eZLink.ManualStatusWd)) Or _
                (msZonePLCData(eZLink.TestTimeRemainingWd) <> msOldZonePLCData(eZLink.TestTimeRemainingWd)) Or _
                bUpdateAll Or (msZonePLCData(eZLink.TestTimeRemainingWd) <> "0") Or (mnCalActive > 0) Or _
                (mnFlowTestActive > 0) Or mbFlowTestActive Then
                nWorkingWord = CInt(msZonePLCData(eZLink.ManualStatusWd))
                Dim nCalActive As Integer = (nWorkingWord And (gnBitVal(eZLink.AutoCalActivBit) Or _
                            gnBitVal(eZLink.ScaleCalActivBit) Or gnBitVal(eZLink.DQCalActivBit)))
                If mnCalActive > 0 Then
                    'calibration is underway
                    If nCalActive > 0 Then
                        mbCalActive = True
                        subEnableGroupboxes(False)
                        mnCalActive = 0
                        lstStatus.Visible = True
                        Status(False) = gpsRM.GetString("psCAL_STARTED")
                    End If
                End If
                ''test over?
                If mbCalActive And (nCalActive = 0) Then
                    'subUpdateScaleCalStatus(True)
                    'Handle this with the scattered access update
                    mbCalActive = False
                    subEnableGroupboxes(True, (nRobotsSelected > 0))
                    Status(False) = gpsRM.GetString("psCAL_FINISHED")
                End If

                'MSW 2/11/11 - track CC status and manage enables better
                Dim bCCActive As Boolean = ((nWorkingWord And (gnBitVal(eZLink.CCActiveBit))) > 0)

                If bCCActive <> mbCCInProgress Then
                    mbCCInProgress = bCCActive
                    subEnableGroupboxes(True, (nRobotsSelected > 0))
                End If

                Dim bFlowTest As Boolean = ((nWorkingWord And gnBitVal(eZLink.FlowTestStatusBit)) > 0)

                'flowtest
                nWorkingWord = CInt(msZonePLCData(eZLink.TestTimeWd))

                Dim nTimeLeft As Single = CSng(mPLC.FormatFromTimerValue( _
                                            CInt(msZonePLCData(eZLink.TestTimeRemainingWd))))
                If bFlowTest Then
                    If mnFlowTestActive > 0 Then
                        proFlow.Visible = True
                        proFlow.Value = 0
                        'FlowTest is underway
                        If nTimeLeft > 0 Then
                            mbFlowTestActive = True
                            subEnableGroupboxes(False)
                            mnFlowTestActive = 0
                            btnFlowTest.Enabled = True
                            btnFlowTest.Text = gcsRM.GetString("csCANCEL")
                        End If
                    End If
                    If mbFlowTestActive Then
                        If colZones.ActiveZone.PLCType = ePLCType.Mitsubishi Then
                            nWorkingWord = CType(nTimeLeft * 100, Integer)
                        Else
                            nWorkingWord = CType(nTimeLeft * 1000, Integer)
                        End If
                        proFlow.Value = nWorkingWord
                        ftbActualTime.Text = mPLC.FormatFromTimerValue(nWorkingWord, "#0")
                        ftbActualTime.ForeColor = Color.Black
                    Else


                    End If

                Else
                    'test over?
                    If mbFlowTestActive Or bUpdateAll Or (mnFlowTestActive > 0) Then
                        ftbActualTime.Text = "0"
                        ftbActualTime.ForeColor = Color.Black
                        btnFlowTest.Enabled = True
                        btnFlowTest.Text = gpsRM.GetString("psSTART_TEST_BTN")
                    End If
                    If mbFlowTestActive Or (mnFlowTestActive > 0) Then
                        mbFlowTestActive = False
                        mnFlowTestActive = 0
                        subEnableGroupboxes(True, (nRobotsSelected > 0))
                        proFlow.Visible = False
                        proFlow.Value = 0
                    End If
                End If

            End If
            If (msZonePLCData(eZLink.ManualStatusWd) <> msOldZonePLCData(eZLink.ManualStatusWd)) Or bUpdateAll Then
                Dim nManualStatus As Integer = CInt(msZonePLCData(eZLink.ManualStatusWd))
                If nManualStatus <> mnManualStatus Then 'Update
                    mnManualStatus = nManualStatus
                    'Enable when the PLC says to
                    If (nManualStatus And gnBitVal(eZLink.BeakerEnabBit)) = _
                                                        gnBitVal(eZLink.BeakerEnabBit) Then
                        btnBeakerMode.Enabled = (Privilege >= ePrivilege.Edit) 'only if logged in
                    Else
                        btnBeakerMode.Enabled = False
                    End If
                    Dim bBeakerMode As Boolean = (nManualStatus And gnBitVal(eZLink.BeakerActiveBit)) = _
                                                        gnBitVal(eZLink.BeakerActiveBit)
                    If (bBeakerMode <> mbBeakerMode) Or bUpdateAll Then
                        mbBeakerMode = bBeakerMode
                        If meOldApplicator <> eColorChangeType.NOT_SELECTED Then
                            subResetFlowTextBox(eParamID.Flow)
                            subResetFlowTextBox(eParamID.Atom)
                            subResetFlowTextBox(eParamID.Fan)
                            subResetFlowTextBox(eParamID.Estat)
                        End If
                    End If
                    btnBeakerMode.Checked = bBeakerMode
                End If
            End If


            For nItem As Integer = 0 To msZonePLCData.GetUpperBound(0)
                msOldZonePLCData(nItem) = msZonePLCData(nItem)
            Next

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub
    Private Sub subUpdateRobotHotlink(ByVal sData As String(), ByVal nRobotNumber As Integer, Optional ByVal bUpdateAll As Boolean = False)
        '********************************************************************************************
        'Description:  New Data from hotlink
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Nothing to do at the moment, but it can't last
        'Dim i%
        'Dim nWorkingWord As Integer = 0


        Try
            'PLC estat feedback
            'Dim sngKvMultiplier As Single = 100 / 30840 '4-20ma = 0-30840 counts = 20-100KV
            'Dim sngUaMultiplier As Single = 200 / 30840 '4-20ma = 0-30840 counts = 0-200Ma
            'Dim nValue As Integer = 0
            'Dim sngValue As Single = 0

            ''update robot plc data
            'nValue = CType(sData(eRLink.CurrentKV), Integer)
            'sngValue = (nValue * sngKvMultiplier) + 3 'Fudge
            'If nValue < 4000 Then sngValue = 0 'Fudge
            'If sngValue > 100 Then sngValue = 100 'and more Fudge
            'ftbActualEStatKv.Text = Format(sngValue, "##0")
            'ftbActualEStatKv.ForeColor = Color.Black

            'nValue = CType(sData(eRLink.CurrentUA), Integer)
            'sngValue = (nValue * sngUaMultiplier) + 7 'Fudge
            'If nValue < 150 Then sngValue = 0 'Fudge
            'If sngValue > 200 Then sngValue = 200 'and more Fudge
            'ftbActualEstatUA.Text = Format(sngValue, "##0")
            'ftbActualEstatUA.ForeColor = Color.Black
            'End PLC Estat feedback


            'cctime
            'Dim sName As String
            'Dim oL As Label
            'For i = 1 To 10
            '    sName = "lblCCTime" & Format(i, "00")
            '    oL = DirectCast(gpbCCCycle.Controls(sName), Label)
            '    If oL.Visible = False Then Exit For
            '    ' if the current color change cycle is running then change the color of box
            '    ' Too much magic for me consider removal geo. 3/19/09
            '    If i = CInt(sData(eRLink.CurrentCCCycle)) Then
            '        oL.BackColor = Color.White
            '    Else
            '        oL.BackColor = System.Drawing.SystemColors.Control
            '    End If
            '    oL.Text = mPLC.FormatFromTimerValue(CInt(sData(eRLink.CurrentCCTime + (i - 1))))
            'Next
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        Finally
            msRobotPLCData = sData
        End Try
    End Sub
#End Region

#Region " Flow Test box "
    Private Sub WriteBeakerModeToPLC(ByVal nBeakerModeEnable As Integer)
        '********************************************************************************************
        'Description:  Enable or disable beaker mode
        '
        'Parameters: nBeakerModeEnable 1 = enable
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sData(0) As String
        Dim sTag As String = "BeakerModeSelected"

        sData(0) = nBeakerModeEnable.ToString
        With mPLC
            .TagName = "Z" & colZones.CurrentZoneNumber.ToString & sTag
            .PLCData = sData
        End With

    End Sub

    Private Sub btnFlowTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                                        Handles btnFlowTest.Click
        '********************************************************************************************
        'Description:  flow test button pressed
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sZone As String = "Z" & colZones.CurrentZoneNumber.ToString
        If mbFlowTestActive Then
            SendCancelTestToPLC(mPLC, sZone)
        Else
            subSetScreenActive(True)
            subSetPumpEnable(chkPump1.Checked, chkPump2.Checked)
            btnFlowTest.Enabled = False
            Dim sData(eParamID.Time) As String
            Dim nSelected As Integer = 0
            Dim nTmp As Integer() = DirectCast(clbRobot.Tag, Integer())
            Dim sChangeLog As String = gpsRM.GetString("psSTART_FLOW_TEST")
            For nIndex As Integer = 0 To clbRobot.Items.Count - 1
                If clbRobot.GetItemChecked(nIndex) Then
                    nSelected = nSelected Or gnBitVal(nTmp(nIndex) - 1)
                    AddChangeRecordToCollection(frmMain.gsChangeLogArea, LoggedOnUser, _
                        colZones.CurrentZoneNumber, clbRobot.Items(nIndex).ToString, cboColor.Text, sChangeLog, colZones.CurrentZone)
                End If
            Next
            For nIndex As Integer = 0 To clbRobot.Items.Count - 1
                If clbRobot.GetItemChecked(nIndex) Then
                End If
            Next

            If nSelected = 0 Then Exit Sub

            mnFlowTestActive = nSelected

            'Check for estat enable
            If CSng(ftbEStat.Text) > colApplicators.Item(meOldApplicator).SelectedMin((eParamID.Estat), mbEngUnits(eParamID.Estat), mbBeakerMode) Then
                Dim reply As Windows.Forms.DialogResult = _
                                        MessageBox.Show(gpsRM.GetString("psESTAT_QUESTION"), _
                                        gpsRM.GetString("psESTATS_ENABLED"), _
                                        MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, _
                                        MessageBoxDefaultButton.Button2)

                If reply = Windows.Forms.DialogResult.Cancel Then
                    btnFlowTest.Enabled = True 'RJO 12/17/09
                    Exit Sub
                End If
            End If

            'Build  arrays of flow test data to send to robot and PLC
            'PLC expects time at the first spot
            'Offset the other parameters (eParamID is setup with flow at 0)
            sData(eParamID.Flow) = ftbFlow.Text
            sData(eParamID.Atom) = ftbAtom.Text
            sData(eParamID.Fan) = ftbFan.Text
            sData(eParamID.Estat) = ftbEStat.Text
            sData(eParamID.Fan2) = ftbFan2.Text
            sData(eParamID.Time) = mPLC.FormatToTimerValue(CInt(ftbTime.Text))

            proFlow.Maximum = CInt(mPLC.FormatToTimerValue(CInt(ftbTime.Text))) + 1

            SendManualFlowToRobots(colControllers, sData, mbEngUnits)
            SendManualFlowToPLC(mPLC, sData, sZone, mbEngUnits)
            'mPWCommon.SaveToChangeLog(colZones.DatabasePath)
            mPWCommon.SaveToChangeLog(colZones.ActiveZone)

        End If

    End Sub
    Private Sub lblxCap_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lblFlowCap.MouseDown, lblAtomCap.MouseDown, lblFanCap.MouseDown, lblEstatCap.MouseDown
        '********************************************************************************************
        'Description:  Flow parameter label mouse down
        '           Catch which label is used to open the units pop-up menu
        '
        'Parameters: Event data
        'Returns:    none, sets modulw level meUnitLabelSelect for use in mnuSelectCounts_Click, mnuSelectEngUnits_Click
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim olbl As Label = DirectCast(sender, Label)
        Dim eTag As eParamID = DirectCast(olbl.Tag, eParamID)
        meUnitLabelSelect = eTag ' Module level, which parameter is selected
        mnuSelectEngUnits.Text = colApplicators.Item(meOldApplicator).ParamUnits(eTag) 'label the menu
    End Sub
    Private Sub mnuSelectCounts_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuSelectCounts.Click
        '********************************************************************************************
        'Description:  Pop-up menu selects counts instead of engineering units
        '
        'Parameters: Event data, uses meUnitLabelSelect set in lblxCap_MouseDown
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEngUnits(meUnitLabelSelect) = True Then
            mbEngUnits(meUnitLabelSelect) = False
            subSetApplicatorLabels(colApplicators.Item(meOldApplicator))
            subResetFlowTextBox(meUnitLabelSelect)
        End If
    End Sub
    Private Sub mnuSelectEngUnits_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuSelectEngUnits.Click
        '********************************************************************************************
        'Description:  Pop-up menu selects engineering units instead of counts
        '
        'Parameters: Event data, uses meUnitLabelSelect set in lblxCap_MouseDown
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEngUnits(meUnitLabelSelect) = False Then
            mbEngUnits(meUnitLabelSelect) = True
            subSetApplicatorLabels(colApplicators.Item(meOldApplicator))
            subResetFlowTextBox(meUnitLabelSelect)
        End If
    End Sub

    Private Sub subResetFlowTextBox(ByVal eIndex As eParamID)
        '********************************************************************************************
        'Description:  reset a text box to the minimum
        '
        'Parameters: parameter number to check
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim fMinVal As Single
        'Get limits from applicator data
        fMinVal = colApplicators.Item(meOldApplicator).SelectedMin((eIndex), mbEngUnits(eIndex), mbBeakerMode)
        Select Case eIndex
            Case eParamID.Flow
                ftbFlow.Text = AdjustFlowMinVal(fMinVal.ToString) 'RJO 12/31/10
            Case eParamID.Atom
                ftbAtom.Text = fMinVal.ToString
            Case eParamID.Fan
                ftbFan.Text = fMinVal.ToString
            Case eParamID.Estat
                ftbEStat.Text = fMinVal.ToString
            Case eParamID.Fan2
                ftbFan2.Text = fMinVal.ToString
            Case Else
        End Select

    End Sub
    Private Sub subInitFlowTest(ByRef Applicator As clsApplicator)
        '********************************************************************************************
        'Description:  Setup the flow test box for a new applicator or robot
        '
        'Parameters: Applicator
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sData(eParamID.Time) As String
        'Reset defaults to use engineering units
        mbEngUnits(eParamID.Flow) = True
        mbEngUnits(eParamID.Atom) = True
        mbEngUnits(eParamID.Fan) = True
        mbEngUnits(eParamID.Estat) = True
        mbEngUnits(eParamID.Fan2) = True

        'Setup labels
        subSetApplicatorLabels(Applicator)

        'Init flow test valuies in text boxes
        ftbFlow.Text = AdjustFlowMinVal(Applicator.SelectedMin(eParamID.Flow, mbEngUnits(eParamID.Flow), mbBeakerMode).ToString) 'RJO 12/31/10
        ftbAtom.Text = Applicator.SelectedMin(eParamID.Atom, mbEngUnits(eParamID.Atom), mbBeakerMode).ToString
        ftbFan.Text = Applicator.SelectedMin(eParamID.Fan, mbEngUnits(eParamID.Fan), mbBeakerMode).ToString
        ftbEStat.Text = Applicator.SelectedMin(eParamID.Estat, mbEngUnits(eParamID.Estat), mbBeakerMode).ToString
        ftbFan2.Text = Applicator.SelectedMin((eParamID.Fan2), mbEngUnits(eParamID.Fan2), mbBeakerMode).ToString

        ftbTime.Text = "30"
        proFlow.Maximum = CInt(ftbTime.Text) + 1

        'Unit select pop-ups, ennable for each parameter that can switch to counts
        If Applicator.UseCounts(eParamID.Flow) Then
            lblFlowCap.ContextMenuStrip = mnuUnitPopup
            'lblFlowCap. = gpsRM.GetString("psUNIT_MENU_TOOLTIP")
        Else
            lblFlowCap.ContextMenuStrip = Nothing
        End If
        If Applicator.UseCounts(eParamID.Atom) Then
            lblAtomCap.ContextMenuStrip = mnuUnitPopup
        Else
            lblAtomCap.ContextMenuStrip = Nothing
        End If
        If Applicator.UseCounts(eParamID.Fan) Then
            lblFanCap.ContextMenuStrip = mnuUnitPopup
        Else
            lblFanCap.ContextMenuStrip = Nothing
        End If
        If Applicator.UseCounts(eParamID.Estat) Then
            lblEstatCap.ContextMenuStrip = mnuUnitPopup
        Else
            lblEstatCap.ContextMenuStrip = Nothing
        End If
        lblFlowCap.Tag = eParamID.Flow
        lblAtomCap.Tag = eParamID.Atom
        lblFanCap.Tag = eParamID.Fan
        lblEstatCap.Tag = eParamID.Estat

        'Build  arrays of flow test data to send to robot and PLC
        sData(eParamID.Flow) = ftbFlow.Text
        sData(eParamID.Atom) = ftbAtom.Text
        sData(eParamID.Fan) = ftbFan.Text
        sData(eParamID.Estat) = ftbEStat.Text
        sData(eParamID.Fan2) = ftbFan2.Text
        sData(eParamID.Time) = mPLC.FormatToTimerValue(CInt(ftbTime.Text))

        SendManualFlowToRobots(colControllers, sData, mbEngUnits)
        SendFlowParamsToPLC(mPLC, sData, colZones.ActiveZone.PLCTagPrefix)

    End Sub
    Private Sub subValidatedTextboxHandler(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validated Routine
        '
        'Parameters: textbox to check, which column of textboxex
        'Returns:    true if bum data so it can be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
    End Sub
    Private Sub subValidatingTextboxHandler(ByVal sender As Object, _
                                ByVal e As System.ComponentModel.CancelEventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validating Routine
        '
        'Parameters: textbox to check
        'Returns:    true if bum data so it cam be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oT As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sText As String = oT.Text
        Dim bBadData As Boolean = False
        Dim sName As String = oT.Name
        Dim nIndex As Integer
        Dim fMinVal As Single
        Dim fMaxVal As Single
        Select Case sName
            Case "ftbFlow"
                nIndex = CInt(eParamID.Flow)
            Case "ftbAtom"
                nIndex = CInt(eParamID.Atom)
            Case "ftbFan"
                nIndex = CInt(eParamID.Fan)
            Case "ftbFan2"
                nIndex = CInt(eParamID.Fan2)
            Case "ftbEstat"
                nIndex = CInt(eParamID.Estat)
            Case "ftbTime"
                nIndex = CInt(eParamID.Time)
            Case Else
                Exit Sub
        End Select
        'Get the range
        If nIndex = CInt(eParamID.Time) Then
            fMinVal = 0
            fMaxVal = mnMaxFlowTestTime
        Else
            fMinVal = colApplicators.Item(meOldApplicator).SelectedMin((nIndex), mbEngUnits(nIndex), mbBeakerMode)
            fMaxVal = colApplicators.Item(meOldApplicator).SelectedMax((nIndex), mbEngUnits(nIndex), mbBeakerMode)
        End If

        'if no edit priv and bad data this locks us up
        If Privilege = ePrivilege.None Then
            e.Cancel = True
            Exit Sub
        End If

        'no value?
        If Strings.Len(sText) = 0 Then
            bBadData = True
            MessageBox.Show(gcsRM.GetString("csNO_NULL"), gcsRM.GetString("csINVALID_DATA"), _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

        If bBadData = False Then
            'numeric
            If oT.NumericOnly Then
                If Not (IsNumeric(sText)) Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csBAD_ENTRY"), gcsRM.GetString("csINVALID_DATA"), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    oT.Undo()
                End If
            End If
            ' limit check
            If bBadData = False Then

                'low limit
                If CSng(sText) < fMinVal Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csVAL_BELOW_MIN") & vbCrLf & _
                                    gcsRM.GetString("csMINIMUM_EQ") & fMinVal, _
                                    gcsRM.GetString("csINVALID_DATA"), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    oT.Undo()
                End If

                'hi limit
                If CSng(sText) > fMaxVal Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csVAL_ABOVE_MAX") & vbCrLf & _
                                    gcsRM.GetString("csMAXIMUM_EQ") & fMaxVal, _
                                    gcsRM.GetString("csINVALID_DATA"), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    oT.Undo()
                End If

                'integers only
                If nIndex > 0 Then 'Don't check for time box
                    If colApplicators.Item(meOldApplicator).ForceInteger(nIndex - 1) Or _
                        (mbEngUnits(nIndex) = False) Then
                        If CSng(sText) <> CSng(CInt(sText)) Then
                            bBadData = True
                            MessageBox.Show(gcsRM.GetString("csINTEGER_REQ"), _
                                            gcsRM.GetString("csINVALID_DATA"), _
                                           MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            oT.Undo()
                        End If
                    End If
                End If
            End If ' limit check
        End If 'bBadData = False

        If bBadData Then
            e.Cancel = True
        End If

    End Sub
    Private Sub subSetTextBoxProperties()
        '********************************************************************************************
        'Description:  
        '                   
        '
        'Parameters:
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As FocusedTextBox.FocusedTextBox

        For Each o In colTextboxes

            AddHandler o.UpArrow, AddressOf subTextBoxUpArrowHandler
            AddHandler o.DownArrow, AddressOf subTextBoxDownArrowHandler
            AddHandler o.LeftArrow, AddressOf subTextBoxLeftArrowHandler
            AddHandler o.RightArrow, AddressOf subTextBoxRightArrowHandler
            AddHandler o.Validating, AddressOf subValidatingTextboxHandler
            AddHandler o.Validated, AddressOf subValidatedTextboxHandler
            AddHandler o.TextChanged, AddressOf subTextboxChangeHandler

        Next

    End Sub
    Private Sub subTextBoxUpArrowHandler(ByRef sender As FocusedTextBox.FocusedTextBox, _
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
        ' boy, didn't think this one thru....
        Dim sName As String = sender.Name

        Select Case sName
            Case "ftbAtom"
                ftbFlow.Focus()
            Case "ftbFlow"
                ftbTime.Focus()
            Case "ftbTime"
                ftbEStat.Focus()
            Case "ftbEStat"
                ftbFan.Focus()
            Case "ftbFan"
                ftbAtom.Focus()

        End Select

    End Sub
    Private Sub subTextboxChangeHandler(ByVal sender As Object, ByVal e As System.EventArgs)
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
        'dont need red for this screen
        DirectCast(sender, FocusedTextBox.FocusedTextBox).ForeColor = Color.Black

    End Sub
    Private Sub subTextBoxDownArrowHandler(ByRef sender As FocusedTextBox.FocusedTextBox, _
                    ByVal bAlt As Boolean, ByVal bShift As Boolean, ByVal bControl As Boolean)
        '********************************************************************************************
        'Description:  someone hit the down arrow key empty text should be checked for by sender
        '               - Based on 10 textboxes in column
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        ' boy, didn't think this one thru....
        Dim sName As String = sender.Name

        Select Case sName
            Case "ftbAtom"
                ftbFan.Focus()
            Case "ftbFlow"
                ftbAtom.Focus()
            Case "ftbTime"
                ftbFlow.Focus()
            Case "ftbEStat"
                ftbTime.Focus()
            Case "ftbFan"
                ftbEStat.Focus()

        End Select
    End Sub
    Private Sub subTextBoxRightArrowHandler(ByRef sender As FocusedTextBox.FocusedTextBox, _
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


    End Sub
    Private Sub subTextBoxLeftArrowHandler(ByRef sender As FocusedTextBox.FocusedTextBox, _
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


    End Sub

    Private Function AdjustFlowMinVal(ByVal sValue As String) As String
        '********************************************************************************************
        'Description: Make sure we don't get any negative numbers in ftbFlow.
        '
        'Parameters: nBeakerModeEnable 1 = enable
        'Returns: none
        '
        'Modification history:
        '
        ' Date By Reason
        ' 12/31/10 RJO Created.
        '********************************************************************************************
        If IsNumeric(sValue) Then
            Dim fVal As Single = CType(sValue, Single)
            If fVal < 0 Then
                Return "0"
            Else
                Return sValue
            End If
        Else
            Return sValue
        End If
    End Function

    Private Function EnableRunButton() As Boolean
        '********************************************************************************************
        'Description: Determine how the .Enabled property of btnRun should be set based on the 
        '             selected cycle.
        '
        'Parameters: none
        'Returns:    True if btnRun should be enabled
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/21/11  RJO     Initial code
        '********************************************************************************************
        Dim bEnable As Boolean = True

        If cboCycle.Text = String.Empty Then
            bEnable = False
        Else
            If mlstDisabledCCItems.Contains(cboCycle.Text) Then bEnable = False
        End If

        Return bEnable

    End Function
#End Region

#Region " Color change box "

    Private Sub cboCycle_DrawItem(ByVal sender As Object, ByVal e As System.Windows.Forms.DrawItemEventArgs) Handles cboCycle.DrawItem
        '********************************************************************************************
        'Description: Show disabled cycles in the list of CC Cycles in gray text
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/21/11  RJO     Initial code
        ' 04/26/11  MSW     CC cycle disable debug
        '********************************************************************************************
        Dim ItemFont As Font = cboCycle.Font
        Dim CurrentItem As String = String.Empty
        Dim oBrush As Brush = Brushes.Black
        'Draw the background for the item
        e.DrawBackground()
        e.DrawFocusRectangle()
        If e.Index = -1 Then
            CurrentItem = cboCycle.Text
            If cboCycle.Focused Then
                oBrush = Brushes.White
            Else
                oBrush = Brushes.Black
            End If
        Else
            CurrentItem = DirectCast(sender, ComboBox).Items(e.Index).ToString
            'If the item is in the disabled list then draw the item with a Gray brush
            If (mlstDisabledCCItems.Contains(CurrentItem)) Then
                'oBrush = New SolidBrush(Color.Gray)
                'If ((e.State And DrawItemState.HotLight) = DrawItemState.HotLight) Or _
                '   ((e.State And DrawItemState.Selected) = DrawItemState.Selected) Then
                oBrush = Brushes.DimGray
                'Else
                '    oBrush = Brushes.WhiteSmoke
                'End If
            Else
                'The item was not in the disabled list so we draw it with a black brush as normal
                If ((e.State And DrawItemState.HotLight) = DrawItemState.HotLight) Or _
                   ((e.State And DrawItemState.Selected) = DrawItemState.Selected) Then
                    oBrush = Brushes.White
                Else
                    oBrush = Brushes.Black
                End If
            End If
        End If
        e.Graphics.DrawString(CurrentItem, ItemFont, oBrush, e.Bounds.X, e.Bounds.Y)

    End Sub

    Private Sub cboCycle_SelectedIndexChanged(ByVal sender As System.Object, _
                            ByVal e As System.EventArgs) Handles cboCycle.SelectedIndexChanged
        '********************************************************************************************
        'Description: run a cc cycle 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/21/11  RJO     Revised to call EnableRunButton function.
        '********************************************************************************************
        'If cboCycle.Text <> String.Empty Then
        '    If btnRun.Enabled = False Then btnRun.Enabled = True
        'End If
        btnRun.Enabled = EnableRunButton() 'RJO 04/21/11
    End Sub
    Private Sub btnRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                                    Handles btnRun.Click
        '********************************************************************************************
        'Description: run a cc cycle 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nTmp As Integer() = CType(cboCycle.Tag, Integer())

        If cboCycle.SelectedIndex > UBound(nTmp) Then Exit Sub
        Dim sChangeLog As String = gpsRM.GetString("psSTART_CC_CYCLE") & ": " & cboCycle.Text
        For nIndex As Integer = 0 To clbRobot.Items.Count - 1
            If clbRobot.GetItemChecked(nIndex) Then
                AddChangeRecordToCollection(frmMain.gsChangeLogArea, LoggedOnUser, _
                    colZones.CurrentZoneNumber, clbRobot.Items(nIndex).ToString, cboColor.Text, sChangeLog, colZones.CurrentZone)
            End If
        Next

        Try
            subSetScreenActive(True)
            subSetPumpEnable(chkPump1.Checked, chkPump2.Checked)

            'Make sure txtTPR has a number in it
            Dim nTpr As Integer = 0
            If txtTPR.Text <> String.Empty Then
                nTpr = CType(txtTPR.Text, Integer)
                Call mManFlowCommon.SendTPRToRobots(colControllers, txtTPR.Text)
            End If


        Catch ex As Exception
            'txtTPR didn't contain a number
        End Try

        mManFlowCommon.RunColorChangeCycle(nTmp(cboCycle.SelectedIndex), mPLC, _
                                        "Z" & colZones.CurrentZoneNumber.ToString)
        'mPWCommon.SaveToChangeLog(colZones.DatabasePath)
        mPWCommon.SaveToChangeLog(colZones.ActiveZone)

    End Sub

    Private Sub txtTPR_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtTPR.TextChanged
        '********************************************************************************************
        'Description:  txtTPR changed Routine
        '
        'Parameters: event parameters
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'dont need red for this screen
        txtTPR.ForeColor = Color.Black

    End Sub

    Private Sub txtTPR_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtTPR.Validating
        '********************************************************************************************
        'Description:  txtTPR Validating Routine
        '
        'Parameters: textbox to check
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bBadData As Boolean
        'TODO - Can these be read from the robot?
        Dim fMinVal As Single = 50
        Dim fMaxVal As Single = 610

        Dim sText As String = txtTPR.Text

        'if no edit priv and bad data this locks us up
        If Privilege = ePrivilege.None Then
            e.Cancel = True
            Exit Sub
        End If

        'no value?
        If Strings.Len(sText) = 0 Then
            bBadData = True
            MessageBox.Show(gcsRM.GetString("csNO_NULL"), gcsRM.GetString("csINVALID_DATA"), _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

        If bBadData = False Then
            'numeric
            If txtTPR.NumericOnly Then
                If Not (IsNumeric(sText)) Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csBAD_ENTRY"), gcsRM.GetString("csINVALID_DATA"), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTPR.Undo()
                End If
            End If
            ' limit check
            If bBadData = False Then

                'low limit
                If CSng(sText) < fMinVal Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csVAL_BELOW_MIN") & vbCrLf & _
                                    gcsRM.GetString("csMINIMUM_EQ") & fMinVal, _
                                    gcsRM.GetString("csINVALID_DATA"), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTPR.Undo()
                End If

                'hi limit
                If CSng(sText) > fMaxVal Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csVAL_ABOVE_MAX") & vbCrLf & _
                                    gcsRM.GetString("csMAXIMUM_EQ") & fMaxVal, _
                                    gcsRM.GetString("csINVALID_DATA"), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTPR.Undo()
                End If

                'integers only
                If CSng(sText) <> CSng(CInt(sText)) Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csINTEGER_REQ"), _
                                    gcsRM.GetString("csINVALID_DATA"), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTPR.Undo()
                End If
            End If ' limit check
        End If 'bBadData = False

        If bBadData Then
            e.Cancel = True
        End If

    End Sub

#End Region

#Region " cartoon and list display "
    Private Sub ValveClicked(ByVal nValve As Integer, ByVal bCurState As Boolean, ByVal bSharedValve As Boolean)
        '********************************************************************************************
        'Description:  color change cartoon click handlers
        '   Get the click event from each color change type
        '    - each cc type cartoon needs to be added to the "Handles" statement and the case statements
        '
        'Parameters: valve number, current state, shared valve = True
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'First check if it should be enabled.  The user control doesn't actually get disabled
        If (clbRobot.Enabled = False) Or (clbRobot.CheckedItems.Count = 0) Then
            Exit Sub
        End If
        'Get the current valve states
        Dim nSharedValvesState As Integer
        Dim nGroupValvesState As Integer

        'Get the valve states from the cc type-specific class
        nSharedValvesState = mCCToon.SharedValveStates
        nGroupValvesState = mCCToon.GroupValveStates

        'Make the requested change to the local variables
        If bSharedValve Then
            If bCurState Then
                nSharedValvesState = ReSetBit(nSharedValvesState, nValve, True)
            Else
                nSharedValvesState = SetBit(nSharedValvesState, nValve, True)
            End If
        Else
            If bCurState Then
                nGroupValvesState = ReSetBit(nGroupValvesState, nValve, True)
            Else
                nGroupValvesState = SetBit(nGroupValvesState, nValve, True)
            End If
        End If

        ''Validate the valve change
        If (mCCToon.ValidateValveSelection(nSharedValvesState, nGroupValvesState) = True) Then
            'Write change to PLC
            WriteValveChangeToPLC(mPLC, ("Z" & colZones.CurrentZoneNumber.ToString), nValve, bSharedValve)
        Else
            'Invalid valve state
            MessageBox.Show(gcsRM.GetString("csINVALID_VALVESTATE"), gcsRM.GetString("csINVALID_DATA"), _
                MessageBoxButtons.OK, MessageBoxIcon.Warning)

        End If
        'Update the cartoons
        mnSharedValvesState = nSharedValvesState
        mnGroupValvesState = nGroupValvesState
        mCCToon.SharedValveStates = nSharedValvesState
        mCCToon.GroupValveStates = nGroupValvesState
        'This causes the valve to flicker when you change state. If you leave it alone, 
        'subUpdateScatteredAccess will take care of it.
        'mCCToon.subUpdateValveCartoon() 'RJO 06/02/10
    End Sub
    Private Sub SharedValveClicked(ByVal nValve As Integer, ByVal bCurState As Boolean) Handles mCCToon.SharedValveClicked
        '********************************************************************************************
        'Description:  color change cartoon click handlers
        '   Get the click event from each color change type
        '    - each cc type cartoon needs to be added to the "Handles" statement and the case statements
        '
        'Parameters: valve number, current state
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        ValveClicked(nValve, bCurState, True)
    End Sub

    Private Sub GroupValveClicked(ByVal nValve As Integer, ByVal bCurState As Boolean) Handles mCCToon.GroupValveClicked
        '********************************************************************************************
        'Description:  color change cartoon click handlers
        '   Get the click event from each color change type
        '    - each cc type cartoon needs to be added to the "Handles" statement and the case statements
        '
        'Parameters: valve number, current state
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        ValveClicked(nValve, bCurState, False)
    End Sub

    Private Sub mnuViewSelect_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuViewSelectToon.Click, _
     mnuViewSelectPaint.Click, mnuViewSelectAA_BS.Click, mnuViewSelectFA_SA.Click, mnuViewSelectEstat.Click, _
     mnuViewSelectCalStat.Click, mnuViewSelectDockCalStat.Click, mnuViewSelectDQCalStat.Click, _
     mnuViewSelectDQCalStat2.Click, mnuViewSelectPressureTest.Click
        Dim eNewView As eViewSelect
        Dim o As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)

        Select Case o.Name
            Case "mnuViewSelectToon"
                eNewView = eViewSelect.Toon
            Case "mnuViewSelectPaint"
                eNewView = eViewSelect.Paint
            Case "mnuViewSelectAA_BS"
                eNewView = eViewSelect.AA_BS
            Case "mnuViewSelectFA_SA"
                eNewView = eViewSelect.FA_SA
            Case "mnuViewSelectEstat"
                eNewView = eViewSelect.Estat
            Case "mnuViewSelectCalStat"
                eNewView = eViewSelect.CalStat
            Case "mnuViewSelectDockCalStat"
                eNewView = eViewSelect.DockCalStat
            Case "mnuViewSelectDQCalStat"
                eNewView = eViewSelect.DQCalStat
            Case "mnuViewSelectDQCalStat2"
                eNewView = eViewSelect.DQCalStat2
            Case "mnuViewSelectPressureTest"  '12/03/09 MSW Pressure Test Change   
                eNewView = eViewSelect.PressureTest
            Case Else
        End Select

        subSelectView(eNewView)

    End Sub

    Private Sub subSelectCalState(ByVal Enable As Boolean)
        '********************************************************************************************
        'Description:  When we're doing a calibration, nothing else (i.e. flow test) should be 
        '              available.
        '
        'Parameters: Enabled
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'btnFlowTest.Enabled = Not Enable
        'btnDQCal.Enabled = Enable
        'btnDQ2Cal.Enabled = Enable

        If Enable Then
            WriteBeakerModeToPLC(1)
        Else
            WriteBeakerModeToPLC(0)
        End If


    End Sub

    Private Sub subSelectView(ByVal eNewView As eViewSelect)
        '********************************************************************************************
        'Description:  select a detail view
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/05/09  MSW     Check for openers
        ' 12/03/09  MSW     Pressure Test Change   
        ' 03/28/12  MSW     Modified for .NET Scattered Access                            4.01.03.00
        ' 06/21/13  RJO     Set column visibility based on the currently selected applicator/CC type
        '********************************************************************************************
        tmrUpdateSA.Enabled = False
        Dim eOldView As eViewSelect
        Dim nCols As Integer = 1
        If (meViewSelect <> eNewView) And meOldApplicator > eColorChangeType.NONE Then
            eOldView = meViewSelect

            'These will be re-enabled as necessary below
            btnDQCal.Enabled = False
            btnDQ2Cal.Enabled = False
            btnPressTest.Enabled = False
            btnAutoCal.Enabled = False

            If eNewView = eViewSelect.Toon Then
                'Stop scattered access update
                'It'll start back up the single one when it updates
                'If Not (colControllers Is Nothing) Then
                '    For Each Controller As clsController In colControllers
                '        If Controller.Robot.IsConnected Then
                '            If oScatteredAccess.IsSetUp Then
                '                oScatteredAccess.UpdateActive(Controller.Name) = False
                '            End If 'oSA.IsSetUp
                '        End If
                '    Next
                'End If
                'Scattered Access for 1
                Dim sMessage(1) As String
                sMessage(0) = msSCREEN_NAME & ";SET"
                sMessage(1) = cboRobot.Text
                oIPC.WriteControlMsg(gs_COM_ID_SA, sMessage)

                'Hide the cal/flow status box
                gpbViewAll.Visible = False
                'Show the fluid diagram
                If Not (uctrlCartoon Is Nothing) Then
                    uctrlCartoon.Visible = True
                End If
                'clear out the labels in the detail panel
                For Each Arm As clsArm In colArms
                    subCleanUpRobotLabels(Arm, False)
                Next
            Else
                'Scattered access for all
                Dim sMessage(1) As String
                sMessage(0) = msSCREEN_NAME & ";ON"
                sMessage(1) = "ALL"
                oIPC.WriteControlMsg(gs_COM_ID_SA, sMessage)

                'Hide the fluid diagram
                If Not (uctrlCartoon Is Nothing) Then
                    uctrlCartoon.Visible = False
                End If
                Dim sz As Size = lblCalStatCap.Size
                'Write the labels and set the number of columns to view in nCols
                Select Case eNewView
                    Case eViewSelect.Paint
                        lblCalStatCap.Text = colApplicators(meOldApplicator).FlowTestLabel(eParamID.Flow)
                        Select Case meOldApplicator
                            Case eColorChangeType.ACCUSTAT, eColorChangeType.AQUABELL
                                'Cmd, Paint used, Paint in can
                                lblStatACap.Text = gpsRM.GetString("psPAINTINCAN")
                                lblStatBCap.Text = gpsRM.GetString("psTOTAL")
                                nCols = 3
                                'Going from wide to narrow columns
                                sz.Width = lblStatBCap.Width
                            Case eColorChangeType.DUAL_PURGE, eColorChangeType.SINGLE_PURGE, eColorChangeType.SINGLE_PURGE_BELL
                                'Cmd, Actual, Total
                                nCols = 3
                                lblStatACap.Text = gpsRM.GetString("psACTUAL")
                                lblStatBCap.Text = gpsRM.GetString("psTOTAL")
                            Case eColorChangeType.HONDA_1K, eColorChangeType.VERSABELL, _
                                 eColorChangeType.VERSABELL2, eColorChangeType.VERSABELL2_PLUS, eColorChangeType.VERSABELL2_32, _
                                 eColorChangeType.VERSABELL3
                                '12/03/09 MSW Pressure Test Change
                                'Cmd, Total
                                lblStatACap.Text = gpsRM.GetString("psTOTAL")
                                lblStatBCap.Text = gpsRM.GetString("psOUTLET_PRESSURE")
                                nCols = 3
                            Case eColorChangeType.BELL_2K, eColorChangeType.GUN_2K, _
                                eColorChangeType.VERSABELL_2K, eColorChangeType.VERSABELL2_2K, eColorChangeType.VERSABELL2_2K_1PLUS1, _
                                eColorChangeType.VERSABELL2_2K_MULTIRESIN, eColorChangeType.GUN_2K_PIG, eColorChangeType.GUN_2K_Mica
                                '12/03/09 MSW Pressure Test Change
                                'Cmd, Total
                                lblStatACap.Text = gpsRM.GetString("psTOTAL")
                                lblStatBCap.Text = gpsRM.GetString("psRESIN_IN_OUT")
                                lblStatCCap.Text = gpsRM.GetString("psHDR_IN_OUT")
                                nCols = 4
                            Case eColorChangeType.VERSABELL2_PLUS_WB, _
                                 eColorChangeType.VERSABELL2_WB, _
                                 eColorChangeType.HONDA_WB, eColorChangeType.VERSABELL3_WB, _
                                 eColorChangeType.VERSABELL3_DUAL_WB
                                'Cmd, Total
                                lblStatACap.Text = gpsRM.GetString("psTOTAL")
                                lblStatBCap.Text = grsRM.GetString("rsWB_CAN_POS")
                                lblStatCCap.Text = grsRM.GetString("rsWB_CAN_TORQUE")
                                nCols = 4
                            Case Else
                                'Cmd, Total
                                lblStatACap.Text = gpsRM.GetString("psTOTAL")
                                nCols = 2
                        End Select
                        sz.Width = lblStatBCap.Width
                    Case eViewSelect.AA_BS
                        nCols = 1
                        lblCalStatCap.Text = colApplicators(meOldApplicator).FlowTestLabel(eParamID.Atom)
                        Select Case colApplicators(meOldApplicator).CntrType(eParamID.Atom)
                            Case eCntrType.APP_CNTR_BS, eCntrType.APP_CNTR_TS, eCntrType.APP_CNTR_AA
                                nCols = 2
                                lblStatACap.Text = gpsRM.GetString("psACTUAL") & _
                                " (" & colApplicators(meOldApplicator).ParamUnits(eParamID.Atom) & ")"
                            Case eCntrType.APP_CNTR_DQ
                                nCols = 2
                                lblStatACap.Text = gpsRM.GetString("psACTUAL") & _
                                " (" & grsRM.GetString("rsPSI") & ")"
                            Case Else
                        End Select
                        sz.Width = lblStatBCap.Width
                    Case eViewSelect.FA_SA
                        nCols = 1
                        lblCalStatCap.Text = colApplicators(meOldApplicator).FlowTestLabel(eParamID.Fan)
                        Select Case colApplicators(meOldApplicator).CntrType(eParamID.Fan)
                            Case eCntrType.APP_CNTR_BS, eCntrType.APP_CNTR_TS
                                nCols = 2
                                lblStatACap.Text = gpsRM.GetString("psACTUAL") & _
                                " (" & colApplicators(meOldApplicator).ParamUnits(eParamID.Atom) & ")"
                            Case eCntrType.APP_CNTR_DQ
                                nCols = 2
                                lblStatACap.Text = gpsRM.GetString("psACTUAL") & _
                                " (" & grsRM.GetString("rsPSI") & ")"
                            Case eCntrType.APP_CNTR_AA
                                If colApplicators(meOldApplicator).NumParms <= eParamID.Fan2 Then
                                    nCols = 2
                                    lblStatACap.Text = gpsRM.GetString("psACTUAL") & _
                                    " (" & grsRM.GetString("rsSA2_UNITS") & ")"
                                Else
                                    nCols = 4
                                    lblStatACap.Text = gpsRM.GetString("psACTUAL") & _
                                    " (" & grsRM.GetString("rsSA2_UNITS") & ")"
                                    lblStatBCap.Text = grsRM.GetString("rsSA2") & _
                                    " (" & grsRM.GetString("rsSA2_UNITS") & ")"
                                    lblStatCCap.Text = gpsRM.GetString("psACTUAL") & _
                                    " (" & grsRM.GetString("rsSA2_UNITS") & ")"
                                End If
                            Case Else
                        End Select
                        sz.Width = lblStatBCap.Width
                    Case eViewSelect.Estat
                        nCols = 3
                        lblCalStatCap.Text = colApplicators(meOldApplicator).FlowTestLabel(eParamID.Estat)
                        lblStatACap.Text = gpsRM.GetString("psACTUAL") & " " & gpsRM.GetString("psKV")
                        lblStatBCap.Text = gpsRM.GetString("psACTUAL") & " " & gpsRM.GetString("psUA")
                        sz.Width = lblStatBCap.Width
                    Case eViewSelect.CalStat
                        nCols = 1
                        sz.Width = lblStatCCap.Left - lblCalStatCap.Left + lblStatCCap.Width
                        lblCalStatCap.Text = gpsRM.GetString("psCAL_STAT")
                    Case eViewSelect.DockCalStat
                        nCols = 1
                        sz.Width = lblStatBCap.Left - lblCalStatCap.Left + lblStatBCap.Width
                        lblCalStatCap.Text = gpsRM.GetString("psDOCK_CAL_STAT")
                    Case eViewSelect.DQCalStat
                        'Select Case colApplicators(meOldApplicator).CntrType(eParamID.Fan)
                        '    Case eCntrType.APP_CNTR_AA
                        '        nCols = 2
                        '        sz.Width = lblStatACap.Left - lblCalStatCap.Left + lblStatACap.Width
                        '        lblStatACap.Left = lblStatBCap.Left
                        '        lblStatACap.Width = lblStatCCap.Left - lblStatACap.Left + lblStatCCap.Width
                        '        lblCalStatCap.Text = gpsRM.GetString("psDQCAL_STAT")
                        '        lblStatACap.Text = gpsRM.GetString("psDQ2CAL_STAT")
                        '    Case eCntrType.APP_CNTR_DQ
                        nCols = 1
                        sz.Width = lblStatCCap.Left - lblCalStatCap.Left + lblStatCCap.Width
                        lblCalStatCap.Text = gpsRM.GetString("psDQCAL_STAT")
                        '    Case Else
                        'End Select
                    Case eViewSelect.DQCalStat2
                        nCols = 1
                        sz.Width = lblStatCCap.Left - lblCalStatCap.Left + lblStatCCap.Width
                        lblCalStatCap.Text = gpsRM.GetString("psDQ2CAL_STAT")
                    Case eViewSelect.PressureTest  '12/03/09 MSW Pressure Test Change
                        nCols = 1
                        sz.Width = lblStatCCap.Left - lblCalStatCap.Left + lblStatCCap.Width
                        lblCalStatCap.Text = gpsRM.GetString("psPRESSURE_TEST")
                    Case Else
                End Select
                lblCalStatCap.Size = sz

                'Dim oLbl2 As Label
                'Select Case eNewView
                '    Case eViewSelect.DQCalStat
                '        Select Case colApplicators(meOldApplicator).CntrType(eParamID.Fan)
                '            Case eCntrType.APP_CNTR_AA
                '                For nRobot As Integer = 1 To colArms.Count ' Just go to current # of robots, we'll clear them all out whenever the robot collection changes.
                '                    If Not (colArms.Item((nRobot - 1)).IsOpener) Then '11/05/09   MSW     Check for openers
                '                        'Fetch each label
                '                        oLbl2 = DirectCast(gpbViewAll.Controls.Item("lblStatA" & nRobot.ToString("00")), Label)
                '                        oLbl2.Width = lblStatCCap.Left - lblStatACap.Left + lblStatCCap.Width
                '                        oLbl2.Left = lblStatBCap.Left
                '                    End If
                '                Next
                '                lblStatACap.Width = lblStatA01.Width
                '                lblStatACap.Left = lblStatBCap.Left
                '                lblCalStatCap.Text = gpsRM.GetString("psDQCAL_STAT")
                '                lblStatACap.Text = gpsRM.GetString("psDQ2CAL_STAT")
                '            Case Else
                '        End Select 'colApplicators(meOldApplicator).CntrType(eParamID.Fan)
                '    Case Else
                '        For nRobot As Integer = 1 To colArms.Count ' Just go to current # of robots, we'll clear them all out whenever the robot collection changes.
                '            If Not (colArms.Item((nRobot - 1)).IsOpener) Then '11/05/09   MSW     Check for openers
                '                'Fetch each label
                '                oLbl2 = DirectCast(gpbViewAll.Controls.Item("lblStatA" & nRobot.ToString("00")), Label)
                '                oLbl2.Width = lblStatBCap.Width
                '                oLbl2.Left = ((lblStatCCap.Left - lblStatBCap.Left) * 2) + (lblStatCCap.Left - lblStatBCap.Left - lblStatBCap.Width)
                '            End If
                '        Next
                '        lblStatACap.Width = lblStatBCap.Width
                '        lblStatACap.Left = ((lblStatCCap.Left - lblStatBCap.Left) * 2) + (lblStatCCap.Left - lblStatBCap.Left - lblStatBCap.Width)
                'End Select 'eNewView

                'Set which ones should be visible
                lblCalStatCap.Visible = (nCols > 0)
                lblStatACap.Visible = (nCols > 1)
                lblStatBCap.Visible = (nCols > 2)
                lblStatCCap.Visible = (nCols > 3)
                sz.Height = lblCalStat01.Height

                Dim oLbl As Label
                For nRobot As Integer = 1 To colArms.Count ' Just go to current # of robots, we'll clear them all out whenever the robot collection changes.
                    If Not (colArms.Item((nRobot - 1)).IsOpener) Then '11/05/09   MSW     Check for openers
                        If colArms.Item(nRobot - 1).ColorChangeType = meOldApplicator Then '06/21/13 RJO Display only applies to applicators of the selected type
                            'Fetch each label
                            oLbl = DirectCast(gpbViewAll.Controls.Item("lblCalStat" & nRobot.ToString("00")), Label)
                            oLbl.Size = sz ' First column can be full width or half for cal status, or normal for others
                            oLbl.Text = ""
                            oLbl.Visible = True
                            oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatA" & nRobot.ToString("00")), Label)
                            oLbl.Text = ""
                            oLbl.Visible = (nCols > 1)
                            oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatB" & nRobot.ToString("00")), Label)
                            oLbl.Text = ""
                            oLbl.Visible = (nCols > 2)
                            oLbl = DirectCast(gpbViewAll.Controls.Item("lblStatC" & nRobot.ToString("00")), Label)
                            oLbl.Text = ""
                            oLbl.Visible = (nCols > 3)
                        End If 'colArms.Item(nRobot - 1).ColorChangeType = meOldApplicator
                    End If
                Next
                'Show the cal/flow status box
                gpbViewAll.Visible = True
            End If
            meViewSelect = eNewView

            End If
        tmrUpdateSA.Enabled = True
    End Sub
    Private Sub subResetViewAllBox(ByRef colArms As clsArms)
        '********************************************************************************************
        'Description:  reset the detail view box
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '11/05/09   MSW     Check for openers
        '********************************************************************************************
        For Each oLbl As Label In gpbViewAll.Controls
            oLbl.Visible = False
        Next
        Dim ol As Label
        For nRobot As Integer = 1 To colArms.Count
            ol = DirectCast(gpbViewAll.Controls("lblRobot" & nRobot.ToString("00")), Label)
            ol.Text = colArms.Item((nRobot - 1)).Name
            ol.Visible = Not (colArms.Item((nRobot - 1)).IsOpener)
        Next
        gpbViewAll.Visible = True
    End Sub
#End Region

#Region " Temp stuff for old password object "
    Private Sub moPassword_LogIn() Handles moPassword.LogIn
        'This can get called by the password thread
        If Me.stsStatus.InvokeRequired Then
            Dim dLogin As New LoginCallBack(AddressOf moPassword_LogIn)
            Me.BeginInvoke(dLogin)
        Else
            Me.LoggedOnUser = moPassword.UserName

            Status = Me.LoggedOnUser & " " & gcsRM.GetString("csLOGGED_IN")
            Me.Privilege = ePrivilege.Edit
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/22/12
        End If
    End Sub
    Private Sub moPassword_LogOut() Handles moPassword.LogOut
        'This can get called by the password thread
        If Me.stsStatus.InvokeRequired Then
            Dim dLogin As New LogOutCallBack(AddressOf moPassword_LogOut)
            Me.BeginInvoke(dLogin)
        Else
            Status = Me.LoggedOnUser & " " & gcsRM.GetString("csLOGGED_OUT")
            Me.LoggedOnUser = String.Empty
            Me.Privilege = ePrivilege.None
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/22/12
        End If

    End Sub
#End Region

#Region " autocal box "
    Private Sub btnDQCal_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDQCal.Click
        '********************************************************************************************
        'Description:  Start a calibration
        '
        'Parameters: which cal type, to select the PLC tag
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************        
        Call subSelectCalState(True)
        subSelectView(eViewSelect.DQCalStat)
        WriteCalRequestToPLC(eCalType.DQCal)
    End Sub
    Private Sub btnDQ2Cal_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDQ2Cal.Click
        '********************************************************************************************
        'Description:  DQ2 Cal Button clicked
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Call subSelectCalState(True)
        subSelectView(eViewSelect.DQCalStat2)
        WriteCalRequestToPLC(eCalType.Dq2Cal)
    End Sub
    Private Sub btnAutoCal_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAutoCal.Click
        '********************************************************************************************
        'Description:  Start a calibration
        '
        'Parameters: which cal type, to select the PLC tag
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************       
        subSelectView(eViewSelect.CalStat)
        WriteCalRequestToPLC(eCalType.ScaleCal)
        Select Case colApplicators.Item(meOldApplicator).ColorChangeType
            Case eColorChangeType.VERSABELL, eColorChangeType.BELL_2K, eColorChangeType.GUN_2K, _
            eColorChangeType.HONDA_1K, eColorChangeType.VERSABELL_2K, eColorChangeType.VERSABELL2, _
            eColorChangeType.VERSABELL2_2K, eColorChangeType.VERSABELL2_PLUS, _
            eColorChangeType.VERSABELL2_2K_1PLUS1, eColorChangeType.VERSABELL2_2K_MULTIRESIN, _
            eColorChangeType.VERSABELL3, eColorChangeType.GUN_2K_PIG, eColorChangeType.GUN_2K_Mica
                subSelectView(eViewSelect.CalStat)
                WriteCalRequestToPLC(eCalType.AutoCal)
            Case Else
                subSelectView(eViewSelect.CalStat)
                WriteCalRequestToPLC(eCalType.ScaleCal)
        End Select
    End Sub
    Private Sub subMoveSUnit(ByVal Position As Integer)
        '********************************************************************************************
        'Description:  Move the S-Unit (Dock device) to a selected position.
        '              1=Dock, 2=Clean, 3=De-Dock
        '
        'Parameters: Position to move to
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        For nIndex As Integer = 0 To clbRobot.Items.Count - 1
            If clbRobot.GetItemChecked(nIndex) Then
                Dim sName As String = clbRobot.GetItemText(clbRobot.Items(nIndex))
                Dim oArm As clsArm = colArms.Item(sName)

                With oArm
                    .ProgramName = "paictask"
                    .VariableName = "ticsunitwrk[" & oArm.ArmNumber.ToString & "].isunitmanpos"
                    .VarValue = Position.ToString
                End With
            End If
        Next 'nIndex

    End Sub
    Private Sub subPressureTest(Optional ByVal eStep As mCCToonRoutines.ePressureTestStep = mCCToonRoutines.ePressureTestStep.No_Parameter)
        '********************************************************************************************
        'Description:  run a pressure test
        '
        'Parameters: which cal type, to select the PLC tag
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '12/03/09 MSW Pressure Test Change
        '********************************************************************************************
        If eStep = mCCToonRoutines.ePressureTestStep.No_Parameter Then
            eStep = mePressureTestStep
        End If
        Dim nBoothStatus As Integer = CInt(msZonePLCData(eZLink.RobotsReadyWd))
        If (nBoothStatus And gnBitVal(eZLink.BoothReadyBit)) <> gnBitVal(eZLink.BoothReadyBit) Then
            eStep = mCCToonRoutines.ePressureTestStep.Abort
        End If
        tmrPressureUpdate.Enabled = False
        Dim nShared As Integer = 0
        Dim nGroup As Integer = 0
        Dim bWriteValves As Boolean = False
        Dim bEnableTimer As Boolean = False
        Select Case eStep
            Case mCCToonRoutines.ePressureTestStep.Abort
                nGroup = 0
                nShared = 0
                bWriteValves = True
                bEnableTimer = False
                eStep = mCCToonRoutines.ePressureTestStep.No_Parameter
                Status = gpsRM.GetString("psPRES_TST_ABORT")
                btnPressTest.Text = gpsRM.GetString("psPRESS_TEST")
            Case mCCToonRoutines.ePressureTestStep.Start, mCCToonRoutines.ePressureTestStep.FillSolv
                btnPressTest.Text = gcsRM.GetString("csCANCEL")
                mCCToon.subGetPressureTestValveMask(eStep, nGroup, nShared)
                bWriteValves = True
                bEnableTimer = True
                tmrPressureUpdate.Interval = mnPressureTestFillTime
                eStep = mCCToonRoutines.ePressureTestStep.MeasureSolv
                Status = gpsRM.GetString("psPRES_TST_FILL_SOL")
            Case mCCToonRoutines.ePressureTestStep.MeasureSolv
                mCCToon.subGetPressureTestValveMask(eStep, nGroup, nShared)
                bWriteValves = True
                bEnableTimer = True
                tmrPressureUpdate.Interval = mnPressureTestMeasureTime
                eStep = mCCToonRoutines.ePressureTestStep.FillAir
                Status = gpsRM.GetString("psPRES_TST_MEAS_SOL")
            Case mCCToonRoutines.ePressureTestStep.FillAir
                mCCToon.subGetPressureTestValveMask(eStep, nGroup, nShared)
                bWriteValves = True
                bEnableTimer = True
                tmrPressureUpdate.Interval = mnPressureTestFillTime
                eStep = mCCToonRoutines.ePressureTestStep.MeasureAir
                Status = gpsRM.GetString("psPRES_TST_FILL_AIR")
            Case mCCToonRoutines.ePressureTestStep.MeasureAir
                mCCToon.subGetPressureTestValveMask(eStep, nGroup, nShared)
                bWriteValves = True
                bEnableTimer = True
                tmrPressureUpdate.Interval = mnPressureTestMeasureTime
                eStep = mCCToonRoutines.ePressureTestStep.Finish
                Status = gpsRM.GetString("psPRES_TST_MEAS_AIR")
            Case mCCToonRoutines.ePressureTestStep.Finish
                nGroup = 0
                nShared = 0
                bWriteValves = True
                bEnableTimer = False
                eStep = mCCToonRoutines.ePressureTestStep.No_Parameter
                Status = gpsRM.GetString("psPRES_TST_FINISH")
                btnPressTest.Text = gpsRM.GetString("psPRESS_TEST")
            Case Else
                nGroup = 0
                nShared = 0
                bWriteValves = True
                bEnableTimer = False
                eStep = mCCToonRoutines.ePressureTestStep.No_Parameter
                Status = gpsRM.GetString("psPRES_TST_ABORT")
                btnPressTest.Text = gpsRM.GetString("psPRESS_TEST")
        End Select
        If bWriteValves Then
            nShared = nShared Xor mnSharedValvesState
            nGroup = nGroup Xor mnGroupValvesState
            If nShared > 0 Then
                WriteValveWordToPLC(mPLC, ("Z" & colZones.CurrentZoneNumber.ToString), nShared, True)
            End If
            If nGroup > 0 Then
                WriteValveWordToPLC(mPLC, ("Z" & colZones.CurrentZoneNumber.ToString), nGroup, False)
            End If
        End If
        mePressureTestStep = eStep
        tmrPressureUpdate.Enabled = bEnableTimer
    End Sub
    Private Sub tmrPressureUpdate_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrPressureUpdate.Tick
        '********************************************************************************************
        'Description:  run the next step of a pressure test
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '12/03/09 MSW Pressure Test Change
        '********************************************************************************************
        subPressureTest()
    End Sub
    Private Sub btnPressTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPressTest.Click
        '********************************************************************************************
        'Description:  Start a calibration
        '
        'Parameters: which cal type, to select the PLC tag
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '12/03/09 MSW Pressure Test Change
        '********************************************************************************************
        Select Case colApplicators.Item(meOldApplicator).ColorChangeType
            Case eColorChangeType.VERSABELL, eColorChangeType.BELL_2K, eColorChangeType.GUN_2K, _
            eColorChangeType.HONDA_1K, eColorChangeType.VERSABELL_2K, eColorChangeType.VERSABELL2, _
            eColorChangeType.VERSABELL2_2K, eColorChangeType.VERSABELL2_PLUS, _
            eColorChangeType.VERSABELL2_2K_1PLUS1, eColorChangeType.VERSABELL2_2K_MULTIRESIN, _
            eColorChangeType.VERSABELL3, eColorChangeType.GUN_2K_PIG, eColorChangeType.GUN_2K_Mica
                If (mePressureTestStep = mCCToonRoutines.ePressureTestStep.Finish) Or _
                   (mePressureTestStep = mCCToonRoutines.ePressureTestStep.No_Parameter) Or _
                   (mePressureTestStep = mCCToonRoutines.ePressureTestStep.Abort) Then
                    subSelectView(eViewSelect.PressureTest)
                    subPressureTest(ePressureTestStep.Start)
                Else
                    subPressureTest(ePressureTestStep.Abort)
                End If
            Case eColorChangeType.HONDA_WB
                subSelectView(eViewSelect.DockCalStat)
                WriteCalRequestToPLC(eCalType.DockCal)
            Case Else
                subSelectView(eViewSelect.CalStat)
                WriteCalRequestToPLC(eCalType.AutoCal)
        End Select
    End Sub
    Private Sub WriteCalRequestToPLC(ByVal nCalType As eCalType)
        '********************************************************************************************
        'Description:  Start a calibration
        '
        'Parameters: which cal type, to select the PLC tag
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nSelected As Integer = 0
        Dim sData(0) As String
        Dim nTmp As Integer() = DirectCast(clbRobot.Tag, Integer())
        Dim sCalTag As String = String.Empty
        Dim sChangeLog As String
        subSetScreenActive(True)
        subSetPumpEnable(chkPump1.Checked, chkPump2.Checked)

        Select Case nCalType
            Case eCalType.AutoCal
                sCalTag = "StartCalibration"
                sChangeLog = gpsRM.GetString("psSTART_AUTO_CAL")
            Case eCalType.ScaleCal
                sCalTag = "StartScaleCal"
                sChangeLog = gpsRM.GetString("psSTART_SCALE_CAL")
            Case eCalType.DQCal
                'sCalTag = "DQCalScreenActive" 'RJO 12/02/09
                sChangeLog = gpsRM.GetString("psSTART_DQ_CAL")
            Case eCalType.Dq2Cal
                'sCalTag = "DQCalScreenActive"  'RJO 12/02/09
                sChangeLog = gpsRM.GetString("psSTART_DQ2_CAL")
            Case eCalType.DockCal
                sCalTag = "StartDockCal"
                sChangeLog = gpsRM.GetString("psSTART_DOCK_CAL")

            Case Else
                sCalTag = "StartCalibration"
                sChangeLog = gpsRM.GetString("psSTART_AUTO_CAL")
        End Select
        Dim sCntr As String = String.Empty
        For nIndex As Integer = 0 To clbRobot.Items.Count - 1
            If clbRobot.GetItemChecked(nIndex) Then
                nSelected = nSelected Or gnBitVal(nTmp(nIndex) - 1)
                AddChangeRecordToCollection(frmMain.gsChangeLogArea, LoggedOnUser, _
                    colZones.CurrentZoneNumber, clbRobot.Items(nIndex).ToString, cboColor.Text, sChangeLog, colZones.CurrentZone)
            End If
        Next

        If nSelected = 0 Then Exit Sub

        mnCalActive = nSelected

        'bit 0 request bit
        nSelected = nSelected + 1
        If sCalTag <> String.Empty Then  'RJO 12/02/09
            sData(0) = "1"
            With mPLC
                .TagName = "Z" & colZones.CurrentZoneNumber.ToString & sCalTag
                .PLCData = sData
            End With
        End If

        For nIndex As Integer = 0 To clbRobot.Items.Count - 1
            If clbRobot.GetItemChecked(nIndex) Then
                If (nCalType = eCalType.DQCal) Or (nCalType = eCalType.Dq2Cal) Then
                    Dim oRobot As clsArm = colArms(clbRobot.Items(nIndex).ToString)
                    If colApplicators(oRobot.ColorChangeType).CntrType(eParamID.Fan) = eCntrType.APP_CNTR_DQ Then
                        If sCntr <> oRobot.Controller.Name Then
                            oRobot.ProgramName = "pavrdq"
                            oRobot.VariableName = "TDQ_SETUP.BCONDHANDCAL"
                            oRobot.VarValue = "True"
                            sCntr = oRobot.Controller.Name
                            mnFASACalChannel = 1
                        End If
                    Else
                        If sCntr <> oRobot.Controller.Name Then
                            'Tell the RC which channel we're calibrating first - RJO 11/28/09
                            oRobot.ProgramName = "patsctl"
                            oRobot.VariableName = "iAACalChan"
                            If nCalType = eCalType.DQCal Then
                                oRobot.VarValue = "3"
                                mnFASACalChannel = 1
                                btnDQCal.Enabled = False
                                btnDQ2Cal.Enabled = False
                            Else
                                oRobot.VarValue = "5"
                                mnFASACalChannel = 2
                                btnDQCal.Enabled = False
                                btnDQ2Cal.Enabled = False
                            End If

                        End If
                    End If
                End If
            End If
        Next
        'mPWCommon.SaveToChangeLog(colZones.DatabasePath)
        mPWCommon.SaveToChangeLog(colZones.ActiveZone)
        Me.Cursor = Cursors.Default
    End Sub
#End Region

#Region " Dock Move To "

    Private Sub btnMoveClean_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnMoveClean.Click
        '********************************************************************************************
        'Description:  Move S-Unit on selected robots to Clean position
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Call subMoveSUnit(2)
    End Sub

    Private Sub btnMoveDeDock_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnMoveDeDock.Click
        '********************************************************************************************
        'Description:  Move S-Unit on selected robots to De-Dock position
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Call subMoveSUnit(3)
    End Sub

    Private Sub btnMoveDock_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnMoveDock.Click
        '********************************************************************************************
        'Description:  Move S-Unit on selected robots to Dock position
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Call subMoveSUnit(1)
    End Sub

#End Region

#Region "holding area"
    Public Sub subUpdateScaleCalStatus(ByVal bLogToChangeLog As Boolean)
        'If we get accuflow or accustat running in PW4 this may be useful
        '********************************************************************************************
        'Description: get calibration status of current valve
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim i%
        Dim sName As String = String.Empty
        Dim sResult As String = String.Empty
        Dim sVar As String = "cal_err2gui"
        Dim o As clsArm

        Status = gpsRM.GetString("psREAD_CAL_STATUS")
        For i = 0 To clbRobot.CheckedItems.Count - 1
            sName = clbRobot.CheckedItems(i).ToString
            o = colArms.Item(sName)
            o.ProgramName = "PAAFLOW"
            o.VariableName = sVar
            Select Case o.VarValue
                Case "261"
                    sResult = gpsRM.GetString("psCANISTER_FILL_ERR")
                Case "262"
                    sResult = gpsRM.GetString("psSCALE_CAL_ERROR")
                Case "263"
                    sResult = gpsRM.GetString("psNO_CHANGE_CAN_POS")
                Case "268"
                    sResult = gpsRM.GetString("psSCALE_CAL_ABORTED")
                Case "269"
                    sResult = gpsRM.GetString("psSCALE_CAL_SUCCESS")
                Case Else
                    sResult = gpsRM.GetString("psSCALE_CAL_ERROR")

            End Select

            If bLogToChangeLog Then
                mManFlowCommon.UpdateChangeLog(gpsRM.GetString("psPERFORMED_SCALE_CAL") _
                            & " " & sResult, "", sName, colZones.CurrentZoneNumber, colZones.ActiveZone.Name)
            End If
        Next

        'mPWCommon.SaveToChangeLog(colZones.DatabasePath)
        mPWCommon.SaveToChangeLog(colZones.ActiveZone)
    End Sub

#End Region

End Class

