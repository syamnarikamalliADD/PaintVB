' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2006
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: frmMain
'
' Description: View and edit Calibration data
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
'    Date       By      Reason                                                        Version
' 04/24/2008    gks     Convert to use SQL database "Process Data"                  4.00.01.00
' 06/20/2008    gks     Add Estat Descriptions                                      4.00.02.00
' 06/01/2009    MSW     convert to VB2008.  Handle class changes made for Fluid Maint,
'                       handle cross-thread calls in colControllers_ConnectionStatusChange(),
'                       moPassword_LogIn(), and moPassword_LogOut()
'                       subInitFormText() - removed ApplicatorType, use ColorChangeType instead
' 07/07/2009    MSW     Start the cal screen
' 11/19/09      MSW     subTextboxKeypressHandler, subShowNewPage - Offer a login if they try to enter data without a login
' 11/19/09      MSW     tlsMain_ItemClicked - it was closing strangely.  Exit the sub after the close command
' 09/14/11      MSW     Assemble a standard version of everything                       4.1.0.0
'    12/02/11   MSW     Add DMONCfg placeholder reference                               4.1.1.0
 
'    01/24/12   MSW     Applicator Updates                                            4.01.01.01
'    02/16/12   MSW     Print/import/export updates, force 32 bit build               4.01.01.02
'    03/22/12   RJO     Modified for .NET Password and IPC                            4.01.02.00
'    04/11/12   MSW     Change CommonStrings setup so it builds correctly             4.01.03.00 
'    04/23/12   MSW     Reload param list with robot change                           4.01.03.01
'    10/24/12   RJO     Added StartupModule to project to prevent multiple instances  4.01.03.02
'    01/01/13   RJO     Added (Unused) Style parameter to mCalCommonRtns              4.01.03.03
'                       LoadCopyScreenSubParamBox for compatibility with frmCopy 
'                       v4.01.01.03
'    01/12/12   MSW     Allow entries longer than 4 characters                        4.01.03.04
'    04/16/13   MSW     Add Canadian language files                                   4.01.05.00
'                       Standalone Changelog
'    07/09/13   MSW     Update and standardize logos                                  4.01.05.01
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up  4.01.05.02
'    09/30/13   MSW     Save screenshots as jpegs                                     4.01.06.00
'    01/07/14   MSW     Remove controlbox from main for                               4.01.06.01
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call              4.01.07.00
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Configuration.ConfigurationSettings
Imports Response = System.Windows.Forms.DialogResult
Imports ConnStat = FRRobotNeighborhood.FRERNConnectionStatusConstants
'

Friend Class frmMain
#Region " Declares "

    '******** Form Constants   **********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Friend Const msSCREEN_NAME As String = "Calibration"   ' <-- For password area change log etc.
    Friend Const msSCREEN_DUMP_NAME As String = "Process_Calibration"
    Friend Const msSCREEN_DUMP_EXT As String = ".jpg"
    Private Const mnMAININX As Integer = 100
    Private Const mnSCREENINX As Integer = 101
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    Friend Const mbUSEROBOTS As Boolean = True
    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    Friend Shared gsChangeLogArea As String = msSCREEN_NAME
    Private msOldZone As String = String.Empty
    Private msOldRobot As String = String.Empty
    Private msOldColor As String = String.Empty
    Private msOldParm As String = String.Empty
    Private colZones As clsZones = Nothing
    Private WithEvents colControllers As clsControllers = Nothing
    Private WithEvents colArms As clsArms = Nothing
    Private mRobot As clsArm = Nothing
    Private mbEventBlocker As Boolean = False
    Private mbRemoteZone As Boolean = False
    Private mbScreenLoaded As Boolean = False
    Private mCalTable As clsCalibration
    'Applicator config
    Friend mApplicator As clsApplicator
    Private Structure tColumnCfg
        Dim UseDecimal As Boolean
        Dim min As Single
        Dim max As Single
    End Structure
    Private mtColumnCfg() As tColumnCfg
    Private Const mnMAX_PARM_COL As Integer = 3
    Private Const mnMAX_PARM_ITEMS As Integer = 1
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean = False
    Private mbEditsMade As Boolean = False
    Private mnProgress As Integer = 0
    Private mnPopupMenuStep As Integer = 0
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    Private msCulture As String = "en-US" 'Default to english
    Private mbShutDownNow As Boolean = False
    Private mEditCol As Integer = -1
    Private mEditRow As Integer = -1
    Private mPrintHtml As clsPrintHtml
    Private mFTB As FocusedTextBox.FocusedTextBox = Nothing
    '******** End Property Variables    *************************************************************

    '******** This is the old pw3 password object interop  *****************************************
    'Friend WithEvents moPassword As PWPassword.PasswordObject 'RJO 03/22/12
    'Friend moPrivilege As PWPassword.cPrivilegeObject 'RJO 03/22/12
    '******** This is the old pw3 password object interop  *****************************************
    Private msColorCache As String = String.Empty

    Friend WithEvents moPassword As clsPWUser 'RJO 03/22/12
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm 'RJO 03/22/12

    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    Delegate Sub NewMessage_Callback(ByVal Schema As String, ByVal DS As DataSet) 'RJO 03/22/12
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
                subEnableControls(True)
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


            'If mPassword.CheckPassword(msSCREEN_NAME, Value, moPassword, moPrivilege) Then
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

            'this is needed with old password object raising events
            'and can hopefully be removed when password is redone
            'Control.CheckForIllegalCrossThreadCalls = True

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
                subShowNewPage()
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
            mbEditsMade = Value
            subEnableControls(True)
        End Set
    End Property
    Friend Property LoggedOnUser() As String
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
#Region " Routines "
    Public Sub UpdateStatus(ByVal nprogress As Integer, ByVal sStatus As String)
        Status(True) = sStatus
        Progress = nprogress
    End Sub
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

        lRet = MessageBox.Show(gcsRM.GetString("csSAVEMSG1") & vbCrLf & _
                            gcsRM.GetString("csSAVEMSG"), gcsRM.GetString("csSAVE_CHANGES"), _
                            MessageBoxButtons.YesNoCancel, _
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

        Select Case lRet
            Case Response.Yes 'Response.Yes
                subSaveData()
                Return True
            Case Response.Cancel
                'false aborts closing form , changing zone etc
                Status = gcsRM.GetString("csSAVE_CANCEL")
                Return False
            Case Else
                Return True
        End Select

    End Function
    Friend Function bLoadFromGUI(ByVal Zone As clsZone, ByRef rRobot As clsArm, _
                                                    ByRef rCalTable As clsCalibration) As Boolean
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

            'Nothing for now
            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

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

        Try
            'no preset data in plc, just return
            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            Return False

        End Try

    End Function
    Friend Function bLoadFromRobot(ByRef rRobot As clsArm, ByRef applicator As clsApplicator, ByRef rCalTable As clsCalibration) As Boolean
        '********************************************************************************************
        'Description:  Load data stored on Robot
        '
        'Parameters: none
        'Returns:    True if load successg
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            If rRobot.IsOnLine Then
                Return rCalTable.LoadFromRobot(rRobot)
            Else
                ' cant talk to robot
                Dim sTmp As String = gcsRM.GetString("csLOADFAILED") & vbCrLf & _
                                            gcsRM.GetString("csCOULD_NOT_CONNECT")
                Status = gcsRM.GetString("csLOADFAILED")
                MessageBox.Show(sTmp, rRobot.Name, _
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If



        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Friend Function bSaveToGUI(ByRef rRobot As clsArm, ByRef Applicator As clsApplicator, ByRef rCalTable As clsCalibration) As Boolean
        '********************************************************************************************
        'Description:  Save data stored on GUI in database, xml file etc.
        '
        'Parameters: Nothing for now, leaving the structure in here in case something gets added.
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            'Everything we need from the DB should be handled by the color change classes

            Return True


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bSaveToPLC() As Boolean
        '********************************************************************************************
        'Description:  Save data stored on PLC.
        '
        'Parameters: nothing for now, leaving the calls in in case something gets added.
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Friend Function bSaveToRobot(ByRef rRobot As clsArm, ByRef applicator As clsApplicator, ByRef rCalTable As clsCalibration) As Boolean
        '********************************************************************************************
        'Description:  Save data stored on Robot
        '
        'Parameters: none
        'Returns:    True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            If rRobot.IsOnLine Then
                Return rCalTable.SaveToRobot(rRobot)
            Else
                Return False
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bValidateData() As Boolean
        '********************************************************************************************
        'Description:  Data Validate Routine
        '
        'Parameters: none
        'Returns:    false if bum data so it cam be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        ' temp hack to fire validate
        cboZone.Focus()

        Return True ' temp
    End Function

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
        If Not (mCalTable Is Nothing) Then
            If mCalTable.changed Then
                ' false means user pressed cancel
                If bAskForSave() = False Then
                    cboZone.Text = msOldZone
                    Progress = 100
                    Exit Sub
                End If
            End If
        End If

        'just in case
        If cboZone.Text = String.Empty Then
            Exit Sub
        Else
            If cboZone.Text = msOldZone Then Exit Sub
        End If


        If colZones.SetCurrentZone(cboZone.Text) = False Then
            If msOldZone <> String.Empty Then
                cboZone.Text = msOldZone
            Else
                cboZone.SelectedIndex = -1
            End If

            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default

            Exit Sub
        End If

        msOldZone = cboZone.Text

        msColorCache = String.Empty

        Try

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subClearScreen()

            subEnableControls(False)

            colControllers = New clsControllers(colZones, False)
            SetUpStatusStrip(Me, colControllers)
            colArms = LoadArmCollection(colControllers)
            mPWRobotCommon.LoadRobotBoxFromCollection(cboRobot, colArms, False, eColorChangeType.NOT_NONE, , , False)
            'make sure the robot cartoons get the right color
            colControllers.RefreshConnectionStatus()
            cboColor.Items.Clear()
            msOldColor = String.Empty
            msOldRobot = String.Empty

            If cboRobot.Items.Count = 1 Then
                'this selects the robot in cases of just one zone. fires event to call subchangerobot
                cboRobot.Text = cboRobot.Items(0).ToString
            Else
                'statusbar text
                Status(True) = gcsRM.GetString("csSELECT_ROBOT")
            End If

            ' copy button
            subEnableControls(True)

            Me.stsStatus.Refresh()

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try

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
        '********************************************************************************************
        'this should be handled better
        Dim bTmp As Boolean = mbEventBlocker
        mbEventBlocker = True

        DataLoaded = False ' needs to be first
        subEnableControls(False)
        mbEventBlocker = bTmp

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
        ' 06/28/11  MSW     Change .show() to .showdialog(), so the main program waits for the copy to finish
        '********************************************************************************************
        'make sure edits are saved
        If Not (mCalTable Is Nothing) Then
            If mCalTable.changed Then
                ' false means user pressed cancel
                If bAskForSave() = False Then
                    cboZone.Text = msOldZone
                    Progress = 100
                    Exit Sub
                End If
            End If
        End If


        'launch copy screen

        Dim frmCopy As New frmCopy

        Try
            With frmCopy

                If colZones.PaintShopComputer Then
                    '   force reload from zoneinfo table on this computer
                    .DatabasePath = String.Empty
                Else
                    .DatabasePath = colZones.DatabasePath
                End If

                .ScreenName = msSCREEN_NAME
                .FormCaption = gpsRM.GetString("psCOPY") & " " & cboParm.Text & " " & msSCREEN_NAME
                .ParameterType = Calibration.frmCopy.eParamType.Valves
                .ParamName = gcsRM.GetString("csCOLOR_CAP")
                .SubParamName = String.Empty
                .UseParam = cboColor.Visible
                .UseSubParam = False
                .SubParamToFromMustMatch = True
                .FancyCopyButtons = False
                .LoadSubParamByRobot = True
                .ByArm = True
                mCalCommonRtns.CopyParm = mCalTable.ParmNum 'save a copy of this on the copy form
                .CCType1 = mRobot.ColorChangeType
                .CCType2 = eColorChangeType.NOT_SELECTED
                .IncludeOpeners = False
                .ShowDialog() 'MSW wait for the copy to finish
            End With

            'Refresh the current display
            subLoadData()
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            frmCopy = Nothing
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
            Case ConnStat.frRNConnecting
                sImgKey = "imgSBRYellow"
                sTipText = gcsRM.GetString("csCONNECTING")
            Case ConnStat.frRNDisconnecting
                sImgKey = "imgSBRYellow"
                sTipText = gcsRM.GetString("csDISCONNECTING")
            Case ConnStat.frRNAvailable
                sImgKey = "imgSBRBlue"
                sTipText = gcsRM.GetString("csAVAILABLE")
            Case ConnStat.frRNConnected
                sImgKey = "imgSBRGreen"
                sTipText = gcsRM.GetString("csCONNECTED")
            Case ConnStat.frRNUnavailable
                sImgKey = "imgSBRRed"
                sTipText = gcsRM.GetString("csUNAVAILABLE")
            Case ConnStat.frRNUnknown
                sImgKey = "imgSBRGrey"
                sTipText = gcsRM.GetString("csUNKNOWN")
            Case ConnStat.frRNHeartbeatLost
                sImgKey = "imgSBRRed"
                sTipText = gcsRM.GetString("csHBLOST")
        End Select

        l.ToolTipText = Controller.Name & " " & _
                            gcsRM.GetString("csCONNECTION_STAT") & " " & sTipText

        Try
            l.Image = DirectCast(gcsRM.GetObject(sImgKey), Image)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

        DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)

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
        Dim bEditsMade As Boolean = False
        If Not (mCalTable Is Nothing) Then
            bEditsMade = mCalTable.changed
        End If
        btnClose.Enabled = True

        If bEnable = False Then
            btnSave.Enabled = False
            btnUndo.Enabled = False
            btnCopy.Enabled = False
            btnPrint.Enabled = False
            btnChangeLog.Enabled = False
            btnStatus.Enabled = True
            btnMultiView.Enabled = True
            bRestOfControls = False
            dgTable.Enabled = False

            mnuPrintFile.Enabled = False
        Else
            Select Case Privilege
                Case ePrivilege.None
                    btnSave.Enabled = False
                    btnUndo.Enabled = False
                    btnCopy.Enabled = False
                    btnPrint.Enabled = DataLoaded
                    btnChangeLog.Enabled = True
                    btnStatus.Enabled = True
                    bRestOfControls = False
                    btnMultiView.Enabled = True
                    dgTable.Enabled = False
                    mnuPrintFile.Enabled = False
                Case ePrivilege.Edit
                    btnSave.Enabled = bEditsMade
                    btnUndo.Enabled = bEditsMade
                    btnPrint.Enabled = DataLoaded
                    btnMultiView.Enabled = True
                    btnChangeLog.Enabled = True
                    btnStatus.Enabled = True
                    bRestOfControls = True
                    btnCopy.Enabled = False
                    dgTable.Enabled = True
                    mnuPrintFile.Enabled = DataLoaded
                Case ePrivilege.Copy
                    btnCopy.Enabled = DataLoaded
                    btnSave.Enabled = bEditsMade
                    btnUndo.Enabled = bEditsMade
                    btnPrint.Enabled = DataLoaded
                    btnMultiView.Enabled = True
                    btnChangeLog.Enabled = True
                    btnStatus.Enabled = True
                    bRestOfControls = True
                    dgTable.Enabled = True
                    mnuPrintFile.Enabled = DataLoaded
            End Select
        End If
        ftbCol0Item0.ReadOnly = Not (bRestOfControls)
        ftbCol1Item0.ReadOnly = Not (bRestOfControls)
        ftbCol2Item0.ReadOnly = Not (bRestOfControls)
        ftbCol0Item1.ReadOnly = Not (bRestOfControls)
        ftbCol1Item1.ReadOnly = Not (bRestOfControls)
        ftbCol2Item1.ReadOnly = Not (bRestOfControls)
    End Sub
    Private Function subFormatScreenLayout() As Integer
        '********************************************************************************************
        'Description:  new data selected - set up the screen layout
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        With gpsRM
            If mApplicator Is Nothing Or mCalTable Is Nothing Then
                For nCol As Integer = 0 To dgTable.ColumnCount - 1
                    dgTable.Columns.Item(nCol).HeaderText = String.Empty
                    dgTable.Columns.Item(nCol).Visible = True
                Next
                Return (0)
            Else
                Dim nCols As Integer = 2
                ReDim mtColumnCfg(nCols - 1)
                mtColumnCfg(0).UseDecimal = True
                mtColumnCfg(0).min = mCalTable.CMD(0).MinValue
                mtColumnCfg(0).max = mCalTable.CMD(0).MaxValue
                mtColumnCfg(1).UseDecimal = False
                mtColumnCfg(1).min = mCalTable.OUTPUT(0).MinValue
                mtColumnCfg(1).max = mCalTable.OUTPUT(0).MaxValue
                dgTable.ColumnCount = nCols
                dgTable.Columns.Item(0).HeaderText = mApplicator.FlowTestLabel(mCalTable.ParmNum)
                If mCalTable.IPC2KTable Then
                    nCols = 4
                    dgTable.ColumnCount = nCols
                    dgTable.Columns.Item(0).HeaderText = .GetString("psRESIN") & " " & mApplicator.FlowTestLabel(mCalTable.ParmNum)
                    dgTable.Columns.Item(1).HeaderText = .GetString("psRESIN") & " " & .GetString("psOUTLET_PRESSURE") & " (" & gpsRM.GetString("psPSI") & ")"
                    dgTable.Columns.Item(2).HeaderText = .GetString("psHARDENER") & " " & mApplicator.FlowTestLabel(mCalTable.ParmNum)
                    dgTable.Columns.Item(3).HeaderText = .GetString("psHARDENER") & " " & .GetString("psOUTLET_PRESSURE") & " (" & gpsRM.GetString("psPSI") & ")"
                    ReDim Preserve mtColumnCfg(nCols - 1)
                    mtColumnCfg(2).UseDecimal = True
                    mtColumnCfg(2).min = mCalTable.SCALE(0).MinValue
                    mtColumnCfg(2).max = mCalTable.SCALE(0).MaxValue
                    mtColumnCfg(3).UseDecimal = False
                    mtColumnCfg(3).min = mCalTable.REF_OUT(0).MinValue
                    mtColumnCfg(3).max = mCalTable.REF_OUT(0).MaxValue
                Else
                    If mCalTable.DynamicTable Then
                        nCols = 3
                        dgTable.ColumnCount = nCols
                        dgTable.Columns.Item(1).HeaderText = .GetString("psDYNAMIC") & " " & .GetString("psCOUNTS")
                        dgTable.Columns.Item(2).HeaderText = .GetString("psREFERENCE") & " " & .GetString("psCOUNTS")
                        ReDim Preserve mtColumnCfg(nCols - 1)
                        mtColumnCfg(2).UseDecimal = False
                        mtColumnCfg(2).min = mCalTable.REF_OUT(0).MinValue
                        mtColumnCfg(2).max = mCalTable.REF_OUT(0).MaxValue
                    Else
                        dgTable.Columns.Item(1).HeaderText = .GetString("psOUTPUT") & " " & .GetString("psCOUNTS")
                        If mCalTable.OutletPressure Then
                            nCols += 1
                            dgTable.ColumnCount = nCols
                            dgTable.Columns.Item(nCols - 1).HeaderText = .GetString("psOUTLET_PRES")
                            ReDim Preserve mtColumnCfg(nCols - 1)
                            mtColumnCfg(nCols - 1).UseDecimal = False
                            mtColumnCfg(nCols - 1).min = mCalTable.DQMinPSI
                            mtColumnCfg(nCols - 1).max = mCalTable.DQMaxPSI
                        End If
                        ReDim Preserve mtColumnCfg(nCols)
                        If mCalTable.ManifoldPressure Then
                            nCols += 1
                            dgTable.ColumnCount = nCols
                            dgTable.Columns.Item(nCols - 1).HeaderText = .GetString("psMANIFOLD_PRES")
                            ReDim Preserve mtColumnCfg(nCols - 1)
                            mtColumnCfg(nCols - 1).UseDecimal = False
                            mtColumnCfg(nCols - 1).min = mCalTable.DQMinPSI
                            mtColumnCfg(nCols - 1).max = mCalTable.DQMaxPSI
                        End If

                    End If
                End If
                For nCol As Integer = 0 To dgTable.ColumnCount - 1
                    dgTable.Columns.Item(nCol).Visible = nCol < nCols
                Next
                Return nCols
            End If
        End With
    End Function
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

        With gpsRM
            lblParm.Text = .GetString("psAPP_PARAM")
            lblColor.Text = .GetString("psVALVE")
            lblCalStatusCap.Text = .GetString("psCAL_STATUS")
            lblCalStatus.Text = String.Empty
            lblCol0.Text = .GetString("psDQ_SETTINGS")
            lblCol1.Text = .GetString("psDQ_MP")
            lblCol2.Text = .GetString("psDQ_OP")
            lblCol0Item0.Text = .GetString("psDQ_GRACE_TIME")
            lblCol0Item1.Text = .GetString("psDQ_CAL_STEP_TIME")
            lblCol1Item0.Text = .GetString("psDQ_WARNING")
            lblCol1Item1.Text = .GetString("psDQ_ALARM")
            lblCol2Item0.Text = .GetString("psDQ_WARNING")
            lblCol2Item1.Text = .GetString("psDQ_ALARM")
            mnuYes.Text = .GetString("psYES")
            mnuNo.Text = .GetString("psNO")
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


        Dim lReply As Windows.Forms.DialogResult = Response.OK
        Try

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

            DataLoaded = False
            mbScreenLoaded = False

            Status = gcsRM.GetString("csINITALIZING")
            Progress = 1


            subProcessCommandLine()

            'init the old password for now 'RJO 03/22/12
            'moPassword = New PWPassword.PasswordObject
            'mPassword.InitPassword(moPassword, moPrivilege, LoggedOnUser, msSCREEN_NAME)
            'If LoggedOnUser <> String.Empty Then
            '    Privilege = ePrivilege.Copy ' extra for buttons etc.
            '    If Privilege <> ePrivilege.Copy Then
            '        'didn't have clearance
            '        Privilege = ePrivilege.Edit
            '        If moPrivilege.ActionAllowed Then
            '            Privilege = mPassword.PrivilegeStringToEnum(moPrivilege.Privilege)
            '        Else
            '            Privilege = ePrivilege.None
            '        End If
            '    End If
            'Else
            '    Privilege = ePrivilege.None
            'End If

            Progress = 5

            Me.Text = gpsRM.GetString("psSCREENCAPTION")

            'init new IPC and new Password 'RJO 03/22/12
            oIPC = New Paintworks_IPC.clsInterProcessComm
            moPassword = New clsPWUser
            Progress = 50

            mPrintHtml = New clsPrintHtml(msSCREEN_NAME, False)
            colZones = New clsZones(String.Empty)
            mScreenSetup.LoadZoneBox(cboZone, colZones, False)
            mScreenSetup.InitializeForm(Me)
            subInitFormText()
            'Call once to clean up the table
            subFormatScreenLayout()
            Progress = 70

            Progress = 98
            subEnableControls(True)

            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound

            If cboZone.Items.Count = 1 Then
                'this selects the zone in cases of just one zone. fires event to call subchangezone
                cboZone.Text = cboZone.Items(0).ToString
            Else
                'statusbar text
                Status(True) = gcsRM.GetString("csSELECT_ZONE")
            End If
            ' refresh lock pic
            DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
            mbScreenLoaded = True


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
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

        'give whatever called us time to repaint
        System.Windows.Forms.Application.DoEvents()

        DataLoaded = False

        Try
            Progress = 0
            Status = gpsRM.GetString("psLOAD_CAL_DATA") & mRobot.Name
            subClearScreen()

            Progress = 20

            mCalTable.ZoneName = colZones.ActiveZone.Name
            mCalTable.ZoneNumber = colZones.ActiveZone.ZoneNumber
            mCalTable.Color = msOldColor
            If bLoadFromRobot(mRobot, mApplicator, mCalTable) Then
                mCalTable.Name = cboParm.Text 'RJO 11/30/09
                Progress = 30
                If bLoadFromPLC() Then
                    If bLoadFromGUI(mRobot.Controller.Zone, mRobot, mCalTable) Then

                        Progress = 80

                        subFormatScreenLayout()

                        'this refreshes screen & sets edit flag
                        DataLoaded = True
                        Status = gcsRM.GetString("csLOADDONE")

                    Else
                        'Load Failed
                        DataLoaded = False
                        mCalTable = Nothing
                    End If    'bLoadFromGUI()
                Else
                    'Load Failed
                    DataLoaded = False
                    mCalTable = Nothing
                End If  'bLoadFromPLC()
            Else
                'Load Failed
                DataLoaded = False
                mCalTable = Nothing
            End If  ' bLoadFromRobot()

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default

        End Try
    End Sub
    Private Sub subChangeColor()
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
        If Not (mCalTable Is Nothing) Then
            If mCalTable.changed Then

                ' false means user pressed cancel
                If bAskForSave() = False Then
                    cboColor.Text = msOldColor
                    Exit Sub
                End If
            End If  'EditsMade
        End If

        'just in case
        If cboColor.Text = String.Empty Then
            Exit Sub
        Else
            If cboColor.Text = msOldColor Then Exit Sub
        End If
        msOldColor = cboColor.Text
        msColorCache = cboColor.Text
        Dim bStatus As Boolean
        Try

            bStatus = lstStatus.Visible
            lstStatus.Visible = True
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

            Progress = 5
            Dim nTag() As Integer = DirectCast(cboParm.Tag, Integer())

            'BTK 01/20/10 Need to use color selected not nTag(cboParm.SelectedIndex)
            If Not nTag Is Nothing Then 'RJO 11/30/09
                mCalTable.Valve = cboColor.SelectedIndex + 1 'nTag(cboParm.SelectedIndex)
                subLoadData()
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
            lstStatus.Visible = bStatus
        End Try


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
        If (dgTable IsNot Nothing) AndAlso DataLoaded Then
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
        If (dgTable IsNot Nothing) AndAlso DataLoaded Then
            bPrintdoc(True)
        End If
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
        If (dgTable IsNot Nothing) AndAlso DataLoaded Then
            If bPrintdoc(False) Then
                mPrintHtml.subShowPrintPreview()
            End If
        End If
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
    Private Function bPrintdoc(ByVal bPrint As Boolean) As Boolean
        '********************************************************************************************
        'Description:  build the print document
        '
        'Parameters:  bPrint - send to printer
        'Returns:   true if successful, OK to continue with preview or page setup
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/07/10  MSw     Change "ReDim sText" to "ReDim Preserve sText" so it'll print everything
        '********************************************************************************************
        If (dgTable IsNot Nothing) AndAlso DataLoaded Then
            mPrintHtml.subSetPageFormat()
            mPrintHtml.subClearTableFormat()
            mPrintHtml.subSetColumnCfg("Bold=on,align=right", 0)
            mPrintHtml.subSetRowcfg("Bold=on", 0, 0)
            Dim sTitle(0) As String
            sTitle(0) = gcsRM.GetString("csROBOT_CAP") & ":  " & cboRobot.Text
            Dim sSubTitle(0) As String
            sSubTitle(0) = gpsRM.GetString("psAPP_PARAM") & ":  " & cboParm.Text
            If cboColor.Visible Then
                sSubTitle(0) = sSubTitle(0) & ", " & gcsRM.GetString("csCOLOR_CAP") & ":  " & cboColor.Text
            End If
            Dim bCancel as boolean = false
            mPrintHtml.subStartDoc(Status, gpsRM.GetString("psSCREENCAPTION") & ":  " & colZones.SiteName & " - " & colZones.CurrentZone, false, bCancel)
            if bCancel then
              return (false)
            end if
            mPrintHtml.subAddObject(dgTable, Status, sTitle, sSubTitle)

            Dim lblTmp1 As Label
            Dim lblTmp2 As Label
            Dim ftbTmp As FocusedTextBox.FocusedTextBox
            Dim sText(0) As String
            Dim bShow As Boolean = False
            Dim nTextLength As Integer = 0
            sText(nTextLength) = gpsRM.GetString("psCAL_STATUS") & vbTab & lblCalStatus.Text
            mPrintHtml.subAddParagraph(Status)
            mPrintHtml.subSetPageFormat()
            mPrintHtml.subClearTableFormat()
            mPrintHtml.subSetColumnCfg("Bold=on,align=right", 0)
            mPrintHtml.subSetRowcfg("Bold=on", 0, 0)
            For nCol As Integer = 0 To mnMAX_PARM_COL 'They're arranged in 3 columns 
                'Label at the top of the column
                lblTmp1 = DirectCast(pnlParams.Controls("lblCol" & nCol.ToString), Label)

                If lblTmp1.Visible Then
                    nTextLength += 1
                    ReDim Preserve sText(nTextLength)
                    sText(nTextLength) = lblTmp1.Text & vbTab & "&nbsp;"
                    mPrintHtml.subSetRowcfg("Bold=on", nTextLength, nTextLength)
                End If
                'hide each item before adding what we need
                For nItem As Integer = 0 To mnMAX_PARM_ITEMS
                    lblTmp2 = DirectCast(pnlParams.Controls("lblCol" & nCol.ToString & "Item" & nItem.ToString), Label)
                    ftbTmp = DirectCast(pnlParams.Controls("ftbCol" & nCol.ToString & "Item" & nItem.ToString), FocusedTextBox.FocusedTextBox)
                    If lblTmp2.Visible Then
                        nTextLength += 1
                        ReDim Preserve sText(nTextLength)
                        sText(nTextLength) = lblTmp2.Text & vbTab & ftbTmp.Text
                    End If
                Next
            Next
            mPrintHtml.subAddObject(sText, Status)
            mPrintHtml.subCloseFile(Status)
            If bPrint Then
                mPrintHtml.subPrintDoc()
            End If
            Return (True)
        Else
            Return (False)
        End If
    End Function
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
        '********************************************************************************************
        If Not (mCalTable Is Nothing) Then
            If mCalTable.changed Then
                ' false means user pressed cancel
                If bAskForSave() = False Then
                    cboRobot.Text = msOldRobot
                    Exit Sub
                End If
            End If  'EditsMade
        End If
        'just in case
        If cboRobot.Text = String.Empty Then
            Exit Sub
        Else
            If cboRobot.Text = msOldRobot Then Exit Sub
        End If
        If msOldRobot <> String.Empty Then _
                                colArms.Item(msOldRobot).Selected = False

        msOldRobot = cboRobot.Text

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            Progress = 5

            subClearScreen()
            Status = gpsRM.GetString("psSELECTING") & msOldRobot
            mRobot = colArms.Item(cboRobot.Text)
            colArms.Item(cboRobot.Text).Selected = True

            mApplicator = New clsApplicator(mRobot, colZones.ActiveZone)

            Progress = 15
            mRobot.SystemColors.Load(mRobot)

            mSysColorCommon.LoadValveBoxFromCollection(mRobot.SystemColors, cboColor, True)

            msOldColor = String.Empty

            Progress = 30
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            msOldParm = String.Empty
            Dim sRestoreParm As String = cboParm.Text
            cboParm.Items.Clear()
            mApplicator.LoadParameterBox(cboParm, True)

            Progress = 70
            'setup limits:
            'statusbar text

            If cboParm.Items.Count = 1 Then
                'this selects the parm in cases of just one zone. fires event to call subchangeParm
                cboParm.Text = cboParm.Items(0).ToString
            Else
                If sRestoreParm = String.Empty Then
                    Status(True) = gpsRM.GetString("psSELECT_PARM")
                Else
                    cboParm.Text = sRestoreParm

                End If
            End If
            Progress = 96


            subEnableControls(True)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Me.Cursor = System.Windows.Forms.Cursors.Default
            Progress = 100
        End Try


    End Sub
    Private Sub subChangeParm()
        '********************************************************************************************
        'Description:  New robot selected
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If Not (mCalTable Is Nothing) Then
            If mCalTable.changed Then
                ' false means user pressed cancel
                If bAskForSave() = False Then
                    cboParm.Text = msOldParm
                    Exit Sub
                End If
            End If  'EditsMade
        End If
        'just in case
        If cboParm.Text = String.Empty Then
            Exit Sub
        Else
            If cboParm.Text = msOldParm Then Exit Sub
        End If

        msOldParm = cboParm.Text

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            Progress = 5

            subClearScreen()
            Status = gpsRM.GetString("psSELECTING") & msOldParm
            Dim nTag() As Integer = DirectCast(cboParm.Tag, Integer())
            'The tag is the robot's parm number starting with 1, all the data starts at 0
            Dim nParam As Integer = nTag(cboParm.SelectedIndex) - 1
            If mCalTable Is Nothing Then
                mCalTable = New clsCalibration
            End If
            mCalTable.ParmNum = nParam

            If (mApplicator.CalSource(nParam) = eCalSource.CAL_SRC_BC) Or _
               (mApplicator.CalSource(nParam) = eCalSource.CAL_SRC_IPC_CAL) Then
                'The color box got loaded when the robot was selected
                cboColor.Visible = True
                lblColor.Visible = True
                If cboColor.Items.Count = 1 Then
                    'this selects the parm in cases of just one zone. fires event to call subchangeParm
                    cboColor.Text = cboColor.Items(0).ToString
                Else
                    Status(True) = gcsRM.GetString("csSELECT_VALVE")
                End If
                If cboColor.SelectedIndex >= 0 Then
                    subLoadData()
                End If
            Else
                cboColor.Visible = False
                lblColor.Visible = False
                'Load the cal table
                subLoadData()

            End If
            If mCalTable Is Nothing Then
                Debug.Print("Why?")
            End If


            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''


            Progress = 96


            subEnableControls(True)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Me.Cursor = System.Windows.Forms.Cursors.Default
            Progress = 100
        End Try


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

        Application.DoEvents()

        Select Case Privilege
            Case ePrivilege.None
                ' shouldnt be here
                subEnableControls(False)
                Exit Sub

            Case Else
                'ok
        End Select


        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        mScreenSetup.MoveCursorOffButton(DirectCast(btnSave, ToolStripItem))

        Try

            'make sure its good data
            If Not bValidateData() Then Exit Sub

            ' do save
            Progress = 1
            If bSaveToGUI(mRobot, mApplicator, mCalTable) Then
                Progress = 10
                If bSaveToPLC() Then
                    Progress = 20
                    If bSaveToRobot(mRobot, mApplicator, mCalTable) Then
                        Progress = 70
                        'mPWCommon.SaveToChangeLog(colZones.DatabasePath)
                        'For SQL database - remove above eventually
                        mPWCommon.SaveToChangeLog(colZones.ActiveZone)
                        ' save done
                        Progress = 90
                        Status = gcsRM.GetString("csSAVE_DONE")
                        EditsMade = mCalTable.changed
                        subShowNewPage()
                        Progress = 0

                    Else
                        'save failed

                    End If  'SaveToRobot()
                Else
                    'save failed

                End If    ' bSaveToPLC()
            Else
                'save failed

            End If      'bSaveToGUI()



        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally

            Me.Cursor = System.Windows.Forms.Cursors.Default

        End Try


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
        ' 04/16/13  MSW     Standalone ChangeLog                                         4.01.05.00
        '********************************************************************************************

        Try
            If cboColor.Visible Then
                mPWCommon.subShowChangeLog(colZones.CurrentZone, gsChangeLogArea, msSCREEN_NAME, cboRobot.Text, _
                              cboColor.Text, mDeclares.eParamType.Valves, nIndex, mbUSEROBOTS, True, oIPC)
            Else
                mPWCommon.subShowChangeLog(colZones.CurrentZone, gsChangeLogArea, msSCREEN_NAME, cboRobot.Text, _
                                              "", mDeclares.eParamType.None, nIndex, mbUSEROBOTS, True, oIPC)
            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
        End Try

    End Sub

    Private Sub subShowMultiView()
        '********************************************************************************************
        'Description:  show the multi view form
        '
        'Parameters: how many changes to show
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim frmAll As New frmAll

        Try
            With frmAll
                .DatabasePath = colZones.DatabasePath
                .InitialZone = cboZone.Text
                .ScreenName = msSCREEN_NAME
                .ParameterType = Calibration.mDeclares.eParamType.Valves
                .ShowDeviceCombo = mbUSEROBOTS
                .ShowParamCombo = cboColor.Visible
                .ShowSubParamCombo = True
                .ResetSubParamOnNewParam = True 'MSW 4/23/12
                .SubParamName = gpsRM.GetString("psAPP_PARAM")
                .ParamName = gcsRM.GetString("csCOLOR_CAP")
                .UseOneGrid = False
                .CCType1 = eColorChangeType.NOT_NONE
                .CCType2 = eColorChangeType.NOT_SELECTED
                .IncludeOpeners = False
                .Show()

            End With

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                 Status, MessageBoxButtons.OK)
            frmAll = Nothing
        End Try

    End Sub
    Private Sub subShowNewPage()
        '********************************************************************************************
        'Description:  Data is all loaded or changed and screen needs to be refreshed
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/19/09  MSW     subTextboxKeypressHandler, subShowNewPage - Offer a login if they try to enter data without a login
        '********************************************************************************************
        Dim nBox As Integer = 1
        Dim oT As TextBox = Nothing
        Dim sName As String = String.Empty
        Dim nPtr As Integer = 0

        mbEventBlocker = True
        Dim nCols As Integer = subFormatScreenLayout()

        Dim nPoints As Integer = mCalTable.NumCalPoints
        If dgTable.RowCount <> nPoints Then
            dgTable.RowCount = nPoints
        End If

        Dim bChanged As Boolean
        Dim sText As String
        For nRow As Integer = 0 To nPoints - 1
            dgTable.Rows.Item(nRow).HeaderCell.Value = (nRow + 1).ToString
            For nCol As Integer = 0 To nCols - 1
                Select Case nCol
                    Case 0
                        'Command
                        bChanged = mCalTable.CMD(nRow).Changed
                        sText = mCalTable.CMD(nRow).Value.ToString
                    Case 1
                        'Output
                        bChanged = mCalTable.OUTPUT(nRow).Changed
                        sText = mCalTable.OUTPUT(nRow).Value.ToString
                    Case 2
                        If mCalTable.IPC2KTable Then
                            '2k
                            bChanged = mCalTable.SCALE(nRow).Changed
                            sText = mCalTable.SCALE(nRow).Value.ToString
                        Else
                            If mCalTable.DynamicTable Then
                                'Dynamic
                                bChanged = mCalTable.REF_OUT(nRow).Changed
                                sText = mCalTable.REF_OUT(nRow).Value.ToString
                            Else
                                If mCalTable.OutletPressure Then
                                    'DQ Outlet
                                    bChanged = mCalTable.OutletPressure(nRow).Changed
                                    sText = mCalTable.OutletPressurePSI(nRow).ToString
                                Else
                                    'DQ Manifold
                                    bChanged = mCalTable.ManifoldPressure(nRow).Changed
                                    sText = mCalTable.ManifoldPressurePSI(nRow).ToString
                                End If
                            End If
                        End If
                    Case 3
                        If mCalTable.IPC2KTable Then
                            '2k
                            bChanged = mCalTable.REF_OUT(nRow).Changed
                            sText = mCalTable.REF_OUT(nRow).Value.ToString
                        Else
                            'DQ Manifold
                            bChanged = mCalTable.ManifoldPressure(nRow).Changed
                            sText = mCalTable.ManifoldPressurePSI(nRow).ToString
                        End If
                    Case Else
                        bChanged = False
                        sText = String.Empty
                End Select
                If bChanged Then
                    dgTable.Rows.Item(nRow).Cells(nCol).Style.ForeColor = Color.Red
                    dgTable.Rows.Item(nRow).Cells(nCol).Style.SelectionForeColor = Color.Red
                Else
                    dgTable.Rows.Item(nRow).Cells(nCol).Style.ForeColor = Color.Black
                    dgTable.Rows.Item(nRow).Cells(nCol).Style.SelectionForeColor = Color.White
                End If
                dgTable.Rows.Item(nRow).Cells(nCol).Style.BackColor = Color.White
                dgTable.Rows.Item(nRow).Cells(nCol).Style.SelectionBackColor = Color.Blue

                dgTable.Rows.Item(nRow).Cells(nCol).Value = sText

            Next
        Next
        dgTable.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
        dgTable.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders)
        'Cal parameter box
        'Cal status:
        lblCalStatus.Text = mCalTable.CalStatusText
        lblCalStatus.Visible = True
        lblCalStatusCap.Visible = True
        'Application specific - details in clscalibration, this just takes the list and throws it on the screen
        Dim lblTmp As Label
        Dim ftbTmp As FocusedTextBox.FocusedTextBox
        For nCol As Integer = 0 To mnMAX_PARM_COL 'They're arranged in 3 columns 
            'Label at the top of the column
            lblTmp = DirectCast(pnlParams.Controls("lblCol" & nCol.ToString), Label)
            If mCalTable.NumCols > nCol Then
                lblTmp.Text = mCalTable.ColLabel(nCol)
                lblTmp.Visible = True
            Else
                lblTmp.Visible = False
            End If
            'hide each item before adding what we need
            For nItem As Integer = 0 To mnMAX_PARM_Items
                lblTmp = DirectCast(pnlParams.Controls("lblCol" & nCol.ToString & "Item" & nItem.ToString), Label)
                ftbTmp = DirectCast(pnlParams.Controls("ftbCol" & nCol.ToString & "Item" & nItem.ToString), FocusedTextBox.FocusedTextBox)
                lblTmp.Visible = False
                ftbTmp.Visible = False
            Next
        Next
        'Get each Calparam from mCalTable
        For nParam As Integer = 0 To mCalTable.NumCalParams - 1
            'Get the label and text box for this param
            Dim ncol As Integer = mCalTable.CalParam(nParam).nCol
            Dim nItem As Integer = mCalTable.CalParam(nParam).nItem
            lblTmp = DirectCast(pnlParams.Controls("lblCol" & ncol.ToString & "Item" & nItem.ToString), Label)
            ftbTmp = DirectCast(pnlParams.Controls("ftbCol" & ncol.ToString & "Item" & nItem.ToString), FocusedTextBox.FocusedTextBox)
            lblTmp.Text = mCalTable.CalParam(nParam).CaptionText
            lblTmp.Visible = True
            Select Case mCalTable.CalParam(nParam).Valuetype
                Case TypeCode.Int32
                    ftbTmp.Text = mCalTable.CalParam(nParam).IntValue.Value.ToString
                    bChanged = mCalTable.CalParam(nParam).IntValue.Changed
                    ftbTmp.NumericOnly = True
                    ftbTmp.ContextMenuStrip = Nothing
                Case TypeCode.Single
                    ftbTmp.Text = mCalTable.CalParam(nParam).SngValue.Value.ToString()
                    bChanged = mCalTable.CalParam(nParam).SngValue.Changed
                    ftbTmp.NumericOnly = True
                    ftbTmp.ContextMenuStrip = Nothing
                Case TypeCode.String
                    ftbTmp.Text = mCalTable.CalParam(nParam).txtValue.Text.ToString()
                    bChanged = mCalTable.CalParam(nParam).IntValue.Changed
                    ftbTmp.NumericOnly = False
                    ftbTmp.ContextMenuStrip = Nothing
                Case TypeCode.Boolean
                    'This may need to be dealt with later
                    ftbTmp.NumericOnly = False
                    If mCalTable.CalParam(nParam).BoolValue.Value Then
                        ftbTmp.Text = mnuYes.ToString()
                    Else
                        ftbTmp.Text = mnuNo.ToString()
                    End If
                    bChanged = mCalTable.CalParam(nParam).BoolValue.Changed
                    ftbTmp.ReadOnly = True
                    ftbTmp.ContextMenuStrip = mnuYesNo
            End Select
            If bChanged Then
                ftbTmp.ForeColor = Color.Red
            Else
                ftbTmp.ForeColor = Color.Black
            End If
            ftbTmp.Visible = True
            'save a pointer back to the cal params when a text box gets edits
            ftbTmp.Tag = nParam
            'We'll be in and out of here, so clean up before adding handlers
            Try
                RemoveHandler ftbTmp.UpArrow, AddressOf subTextBoxUpArrowHandler
                RemoveHandler ftbTmp.DownArrow, AddressOf subTextBoxDownArrowHandler
                RemoveHandler ftbTmp.LeftArrow, AddressOf subTextBoxLeftArrowHandler
                RemoveHandler ftbTmp.RightArrow, AddressOf subTextBoxRightArrowHandler
                RemoveHandler ftbTmp.Validating, AddressOf subTextboxValidatingHandler
                RemoveHandler ftbTmp.Validated, AddressOf subTextboxValidatedHandler
                RemoveHandler ftbTmp.TextChanged, AddressOf subTextboxChangeHandler
                RemoveHandler ftbTmp.KeyPress, AddressOf subTextboxKeypressHandler
                RemoveHandler ftbTmp.KeyPress, AddressOf subTextboxKeypressHandler
                RemoveHandler ftbTmp.MouseDown, AddressOf subTextboxMousedownHandler
            Catch ex As Exception

            End Try
            AddHandler ftbTmp.UpArrow, AddressOf subTextBoxUpArrowHandler
            AddHandler ftbTmp.DownArrow, AddressOf subTextBoxDownArrowHandler
            AddHandler ftbTmp.LeftArrow, AddressOf subTextBoxLeftArrowHandler
            AddHandler ftbTmp.RightArrow, AddressOf subTextBoxRightArrowHandler
            AddHandler ftbTmp.Validating, AddressOf subTextboxValidatingHandler
            AddHandler ftbTmp.Validated, AddressOf subTextboxValidatedHandler
            AddHandler ftbTmp.TextChanged, AddressOf subTextboxChangeHandler
            AddHandler ftbTmp.KeyPress, AddressOf subTextboxKeypressHandler
            AddHandler ftbTmp.MouseDown, AddressOf subTextboxMousedownHandler
        Next
        pnlGraph.Invalidate()
        mbEventBlocker = False

    End Sub

    Private Sub subTextboxMousedownHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        '********************************************************************************************
        'Description:  Identify the ftp launching the context menu
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mFTB = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        If (e.Button = Windows.Forms.MouseButtons.Left) AndAlso (mFTB.ContextMenuStrip IsNot Nothing) Then
            Dim pt As Point = e.Location
            pt.X = pt.X
            pt.Y = pt.Y
            mFTB.ContextMenuStrip.Show(mFTB.PointToScreen(pt))
        End If
    End Sub

    Private Sub mnu_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles mnuYesNo.Opening
        '********************************************************************************************
        'Description:  Menu handlers - boolean pop-up is attached at design time, custom menus get added when they're made
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If Privilege = ePrivilege.None Then
            e.Cancel = True
        End If
    End Sub

    Private Sub mnu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuYes.Click, mnuNo.Click
        '********************************************************************************************
        'Description:  Menu handlers - boolean pop-up is attached at design time, custom menus get added when they're made
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oMnu As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim sText As String = oMnu.Text
        mFTB.Text = oMnu.Text

        If mFTB Is Nothing Then Exit Sub

        If DataLoaded = False Then Exit Sub
        Dim nParam As Integer = CInt(mFTB.Tag)
        Select Case sText
            Case mnuYes.Text
                mCalTable.CalParam(nParam).BoolValue.Value = True
            Case mnuNo.Text
                mCalTable.CalParam(nParam).BoolValue.Value = False
        End Select

        If mCalTable.changed Then EditsMade = True

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

        If MessageBox.Show(gcsRM.GetString("csUNDOMSG"), msSCREEN_NAME, MessageBoxButtons.OKCancel, _
                                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) _
                                                                    = Response.OK Then

            subLoadData()
            Status(True) = gpsRM.GetString("csREADY")
        End If

    End Sub

#End Region
#Region " Events "

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

            Call subInitializeForm()

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: frmMain_Load", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        Finally

        End Try

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
        ' 11/19/09  MSW     tlsMain_ItemClicked - it was closing strangely.  add a module variable to limit new activity
        '********************************************************************************************
        Select Case e.ClickedItem.Name ' case sensitive
            Case "btnClose"
                'Check to see if we need to save is performed in  bAskForSave
                mbShutDownNow = True
                Me.Close()
                Exit Sub
            Case "btnStatus"
                lstStatus.Visible = Not lstStatus.Visible

            Case "btnSave"
                'give validate a chance to fire
                Application.DoEvents()
                'privilege check done in subroutine
                subSaveData()

            Case "btnPrint"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                If (dgTable IsNot Nothing) AndAlso DataLoaded Then
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

            Case "btnChangeLog"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                subShowChangeLog(CType(eChangeMnu.Hours24, Integer))

            Case "btnCopy"
                Select Case Privilege
                    Case ePrivilege.None
                        'logout occured while screen open
                        subEnableControls(False)
                    Case Else
                        subCopyButtonPressed()
                End Select
            Case "btnMultiView"
                subShowMultiView()
        End Select

        tlsMain.Refresh()

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
        '********************************************************************************************

        'If Not (mColorChange Is Nothing) Then
        '    If mColorChange.Changed Then
        '        e.Cancel = (bAskForSave() = False)
        '    End If
        'End If

        Try
            If colArms Is Nothing Then Exit Sub
            If colArms.Count = 0 Then Exit Sub
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
        mbEventBlocker = True
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        mbEventBlocker = False
    End Sub
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
        If Progress > 0 Then Progress += 5

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
        ' 06/01/09  MSW     handle cross-thread calls
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
    Private Sub cboZone_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboZone.SelectedIndexChanged

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
    Private Sub cboRobot_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboRobot.SelectedIndexChanged

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
            subChangeRobot()
        End If

    End Sub
    Private Sub cboParm_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboParm.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Applicator Parameter Combo Changed 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If cboParm.Text <> msOldParm Then
            ' so we don't go off half painted
            Me.Refresh()
            subChangeParm()
        End If

    End Sub
    Private Sub cboColor_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboColor.SelectedIndexChanged

        '********************************************************************************************
        'Description:  Parameter Combo Changed 
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
            subChangeColor()
        End If

    End Sub
    Private Sub frmMain_Layout(ByVal sender As Object, _
                            ByVal e As System.Windows.Forms.LayoutEventArgs) Handles MyBase.Layout
        '********************************************************************************************
        'Description:  Form needs a redraw due to resize
        '   <System.Diagnostics.DebuggerStepThrough()>
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nHeight As Integer = tscMain.ContentPanel.Height
        If nHeight < 100 Then nHeight = 100
        Dim nWidth As Integer = tscMain.ContentPanel.Width
        If nWidth < 100 Then nWidth = 100
    End Sub

    Private Sub colRobots_BumpProgress() Handles colArms.BumpProgress
        '********************************************************************************************
        'Description:  bump me!
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If Progress > 0 Then Progress += 5

    End Sub
    Private Sub mnuLogIn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuLogin.Click
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
    Private Sub mnuLogOut_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuLogOut.Click
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
        mbEventBlocker = False
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
        RemotePWPath = String.Empty

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
        Dim cachePriviledge As ePrivilege
        Dim bRem As Boolean = False
        Dim bAllow As Boolean = False

        If LoggedOnUser <> String.Empty Then

            'If moPrivilege.Privilege = String.Empty Then 'RJO 03/22/12
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
                If colZones.PaintShopComputer = False Then
                    bAllow = True
                End If
            End If

            Privilege = cachePriviledge

        Else
            Privilege = ePrivilege.None
        End If

        If colZones.PaintShopComputer = False Then
            bRem = mbRemoteZone
        End If

        DoStatusBar2(stsStatus, LoggedOnUser, Privilege, bRem, bAllow)


    End Sub

#End Region
#Region " Temp stuff for old password object "

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
        ' 06/01/09  MSW     handle cross-thread calls
        '********************************************************************************************
        'This can get called by the password thread
        If Me.stsStatus.InvokeRequired Then
            Dim dLogin As New LoginCallBack(AddressOf moPassword_LogIn)
            Me.BeginInvoke(dLogin)
        Else
            Me.LoggedOnUser = moPassword.UserName
            Status = Me.LoggedOnUser & " " & gcsRM.GetString("csLOGGED_IN")
            Privilege = ePrivilege.Copy ' extra for buttons etc.
            If Privilege <> ePrivilege.Copy Then
                'didn't have clearance
                Privilege = ePrivilege.Edit
            End If
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/22/12
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
        ' 06/01/09  MSW     handle cross-thread calls
        '********************************************************************************************
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
                    subLaunchHelp(gs_HELP_CALIBRATION, oIPC)
                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty

                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath & msSCREEN_DUMP_NAME & msSCREEN_DUMP_EXT, Imaging.ImageFormat.Jpeg)

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

    Private Sub dgTable_CellBeginEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles dgTable.CellBeginEdit
        '********************************************************************************************
        'Description: Keep track of the current edit cell
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '    04/05/10   MSW     Keep track of the current edit cell
        '********************************************************************************************
        mEditCol = e.ColumnIndex
        mEditRow = e.RowIndex
    End Sub


    Private Sub dgTable_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgTable.CellEndEdit
        '********************************************************************************************
        'Description: Validate edits
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '    04/05/10   MSW     Move some things around to prevent unexpected errors
        '********************************************************************************************
        mEditCol = -1
        mEditRow = -1
        Dim sVal As String = String.Empty
        Dim bAbort As Boolean = False
        Try
            Dim fVal As Single
            Dim nVal As Integer
            Try
                'See if it converts to a number
                sVal = dgTable.Rows.Item(e.RowIndex).Cells.Item(e.ColumnIndex).Value.ToString
                fVal = CSng(sVal)
            Catch ex As Exception
                Dim lRet As DialogResult
                lRet = MessageBox.Show(gcsRM.GetString("csINV_ENTRY"), gcsRM.GetString("csERROR"), _
                                        MessageBoxButtons.OK, _
                                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                bAbort = True
            End Try

            If (mtColumnCfg(e.ColumnIndex).UseDecimal = False) And (bAbort = False) Then
                Try
                    'See if it converts to an integer
                    nVal = CInt(sVal)
                Catch ex As Exception
                    Dim lRet As DialogResult
                    lRet = MessageBox.Show(gcsRM.GetString("csINTEGER_REQ"), gcsRM.GetString("csERROR"), _
                                            MessageBoxButtons.OK, _
                                            MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    bAbort = True
                End Try
            End If
            If (bAbort = False) Then
                'Range check
                If ((fVal > mtColumnCfg(e.ColumnIndex).max) Or (fVal < mtColumnCfg(e.ColumnIndex).min)) Then
                    'Out of range, reset
                    Dim lRet As DialogResult
                    lRet = MessageBox.Show(gpsRM.GetString("psVAL_OUT_OF_RANGE") & mtColumnCfg(e.ColumnIndex).min.ToString & _
                            gpsRM.GetString("ps_TO_") & mtColumnCfg(e.ColumnIndex).max.ToString & _
                            gpsRM.GetString("ps_RETURN_TO_ORG"), gcsRM.GetString("csERROR"), _
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation, _
                            MessageBoxDefaultButton.Button1)

                    bAbort = True
                Else
                    'Made it here, the value is accepted
                    Select Case e.ColumnIndex
                        Case 0 'Command
                            mCalTable.CMD(e.RowIndex).Value = fVal
                        Case 1
                            mCalTable.OUTPUT(e.RowIndex).Value = nVal
                        Case 2
                            If mCalTable.IPC2KTable Then
                                mCalTable.SCALE(e.RowIndex).Value = fVal
                            Else
                                If mCalTable.DynamicTable Then
                                    mCalTable.REF_OUT(e.RowIndex).Value = nVal
                                Else
                                    If mCalTable.OutletPressure Then
                                        mCalTable.OutletPressurePSI(e.RowIndex) = nVal
                                    Else
                                        mCalTable.ManifoldPressurePSI(e.RowIndex) = nVal
                                    End If
                                End If
                            End If
                        Case 3
                            If mCalTable.IPC2KTable Then
                                mCalTable.REF_OUT(e.RowIndex).Value = nVal
                            Else
                                mCalTable.ManifoldPressurePSI(e.RowIndex) = nVal
                            End If
                    End Select
                    dgTable.Rows.Item(e.RowIndex).Cells(e.ColumnIndex).Style.ForeColor = Color.Red
                    dgTable.Rows.Item(e.RowIndex).Cells(e.ColumnIndex).Style.SelectionForeColor = Color.Red
                End If
            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            bAbort = True
        End Try
        Try
            If bAbort Then
                Select Case e.ColumnIndex
                    Case 0 'Command
                        dgTable.Rows.Item(e.RowIndex).Cells.Item(e.ColumnIndex).Value = mCalTable.CMD(e.RowIndex).Value.ToString
                    Case 1
                        dgTable.Rows.Item(e.RowIndex).Cells.Item(e.ColumnIndex).Value = mCalTable.OUTPUT(e.RowIndex).Value.ToString()
                    Case 2
                        If mCalTable.DynamicTable Then
                            dgTable.Rows.Item(e.RowIndex).Cells.Item(e.ColumnIndex).Value = mCalTable.REF_OUT(e.RowIndex).Value.ToString()
                        Else
                            If mCalTable.OutletPressure Then
                                dgTable.Rows.Item(e.RowIndex).Cells.Item(e.ColumnIndex).Value = mCalTable.OutletPressurePSI(e.RowIndex).ToString()
                            Else
                                dgTable.Rows.Item(e.RowIndex).Cells.Item(e.ColumnIndex).Value = mCalTable.ManifoldPressurePSI(e.RowIndex).ToString()
                            End If
                        End If
                    Case 3
                        dgTable.Rows.Item(e.RowIndex).Cells.Item(e.ColumnIndex).Value = mCalTable.ManifoldPressurePSI(e.RowIndex).ToString()
                End Select
            End If
            If mCalTable.changed Then
                EditsMade = True
            End If
        Catch ex As Exception

        End Try
        pnlGraph.Invalidate()
    End Sub
    Private Sub dgTable_DataError(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles dgTable.DataError
        '********************************************************************************************
        'Description: edit error 
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Dim lRet As DialogResult
        lRet = MessageBox.Show(gcsRM.GetString("csINV_ENTRY"), gcsRM.GetString("csERROR"), _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        dgTable.CancelEdit()
 
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
        If mbEventBlocker Then Exit Sub
        If DataLoaded Then EditsMade = True

    End Sub
    Private Sub subTextboxValidatedHandler(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validated Routine
        '
        'Parameters: event parameters
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oT As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sText As String = oT.Text


        If DataLoaded = False Then Exit Sub
        Dim nParam As Integer = CInt(oT.Tag)
        Select Case mCalTable.CalParam(nParam).Valuetype
            Case TypeCode.Int32
                mCalTable.CalParam(nParam).Intvalue.Value = CInt(sText)
            Case TypeCode.Single
                mCalTable.CalParam(nParam).SngValue.Value = CSng(sText)
            Case TypeCode.String
                mCalTable.CalParam(nParam).txtValue.Text = sText
            Case TypeCode.Boolean
                'This may need to be dealt with later
                'mCalTable.BoolValue
        End Select


        If mCalTable.changed Then EditsMade = True
        
    End Sub
    Private Sub subTextboxValidatingHandler(ByVal sender As Object, _
                                ByVal e As System.ComponentModel.CancelEventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validating Routine
        '
        'Parameters: event parameters
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbShutDownNow Then Exit Sub
        Dim oT As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sText As String = oT.Text
        Dim bBadData As Boolean = False
        Dim nMinVal As Single = 0
        Dim nMaxVal As Single = 0
        Dim sOldVal As String = String.Empty
        Dim nParam As Integer = CInt(oT.Tag)
        Dim bIntOnly As Boolean = False
        Select Case mCalTable.CalParam(nParam).Valuetype
            Case TypeCode.Int32
                sOldVal = mCalTable.CalParam(nParam).IntValue.Value.ToString
                nMinVal = mCalTable.CalParam(nParam).IntValue.MinValue
                nMaxVal = mCalTable.CalParam(nParam).IntValue.MaxValue
                bIntOnly = True
            Case TypeCode.Single
                sOldVal = mCalTable.CalParam(nParam).SngValue.Value.ToString
                nMinVal = mCalTable.CalParam(nParam).SngValue.MinValue
                nMaxVal = mCalTable.CalParam(nParam).SngValue.MaxValue
                bIntOnly = False
            Case TypeCode.String
                sOldVal = mCalTable.CalParam(nParam).txtValue.Text.ToString
                nMinVal = 0
                nMaxVal = 0
                bIntOnly = False
            Case TypeCode.Boolean
                'This may need to be dealt with later
                'mCalTable.BoolValue
        End Select

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
            oT.Undo()
            e.Cancel = True
            Exit Sub
        End If


        'numeric
        If bBadData = False Then
            If oT.NumericOnly Then
                If Not (IsNumeric(sText)) Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csBAD_ENTRY"), gcsRM.GetString("csINVALID_DATA"), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                If bIntOnly And Not (bBadData) Then
                    Try
                        If CSng(oT.Text) <> CSng(CInt(oT.Text)) Then
                            bBadData = True
                        End If
                    Catch ex As Exception
                        bBadData = True
                    End Try
                    If bBadData Then
                        Dim lRet As DialogResult
                        lRet = MessageBox.Show(gcsRM.GetString("csINTEGER_REQ"), gcsRM.GetString("csERROR"), _
                                                MessageBoxButtons.OK, _
                                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    End If
                End If
                ' limit check
                If bBadData = False Then

                    'low limit
                    If CSng(sText) < nMinVal Then
                        bBadData = True
                        MessageBox.Show(gcsRM.GetString("csVAL_BELOW_MIN") & vbCrLf & _
                                        gcsRM.GetString("csMINIMUM_EQ") & nMinVal, _
                                        gcsRM.GetString("csINVALID_DATA"), _
                                       MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If

                    'hi limit
                    If CSng(sText) > nMaxVal Then
                        bBadData = True
                        MessageBox.Show(gcsRM.GetString("csVAL_ABOVE_MAX") & vbCrLf & _
                                        gcsRM.GetString("csMAXIMUM_EQ") & nMaxVal, _
                                        gcsRM.GetString("csINVALID_DATA"), _
                                       MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If

                End If '  bBadData = False 
            End If ' oT.NumericOnly
        End If ' bBadData = False

        If bBadData Then
            oT.Text = sOldVal
            e.Cancel = True
        End If


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
        Dim nItem As Integer = CInt(sender.Name.Substring(11, 1))
        Dim nCol As Integer = CInt(sender.Name.Substring(6, 1))
        Dim bDone As Boolean = False
        Do
            nItem -= 1
            Dim ftbTmp As FocusedTextBox.FocusedTextBox = _
                DirectCast(pnlParams.Controls("ftbCol" & nCol.ToString & "Item" & nItem.ToString),  _
                        FocusedTextBox.FocusedTextBox)
            If ftbTmp Is Nothing Then
                'Try again
                nItem = mnMAX_PARM_ITEMS + 1
            Else
                If ftbTmp.Visible = True Then
                    bDone = True
                    ftbTmp.Focus()
                Else
                    'Try again
                End If
            End If
            If nItem = 0 Then 'catch-all
                'If it's in here and didn't set focus above, it's broke
                bDone = True
            End If
        Loop Until bDone = True

    End Sub
    Private Sub subTextBoxDownArrowHandler(ByRef sender As FocusedTextBox.FocusedTextBox, _
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
        Dim nItem As Integer = CInt(sender.Name.Substring(11, 1))
        Dim nCol As Integer = CInt(sender.Name.Substring(6, 1))
        Dim bDone As Boolean = False
        Do
            nItem += 1
            Dim ftbTmp As FocusedTextBox.FocusedTextBox = _
                DirectCast(pnlParams.Controls("ftbCol" & nCol.ToString & "Item" & nItem.ToString),  _
                        FocusedTextBox.FocusedTextBox)
            If ftbTmp Is Nothing Then
                'Try again go to 0
                nItem = -1
            Else
                If ftbTmp.Visible = True Then
                    bDone = True
                    ftbTmp.Focus()
                Else
                    'Try again
                End If
            End If
            If nItem = mnMAX_PARM_ITEMS Then 'catch-all
                'If it's in here and didn't set focus above, it's broke
                bDone = True
            End If
        Loop Until bDone = True

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
        Dim nItem As Integer = CInt(sender.Name.Substring(11, 1))
        Dim nCol As Integer = CInt(sender.Name.Substring(6, 1))
        Dim bDone As Boolean = False
        Do
            nCol += 1
            Dim ftbTmp As FocusedTextBox.FocusedTextBox = _
                DirectCast(pnlParams.Controls("ftbCol" & nCol.ToString & "Item" & nItem.ToString),  _
                        FocusedTextBox.FocusedTextBox)
            If ftbTmp Is Nothing Then
                'Try again go to 0
                nCol = -1
            Else
                If ftbTmp.Visible = True Then
                    bDone = True
                    ftbTmp.Focus()
                Else
                    'Try again
                End If
            End If
            If nCol = mnMAX_PARM_COL Then 'catch-all
                'If it's in here and didn't set focus above, it's broke
                bDone = True
            End If
        Loop Until bDone = True

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
        Dim nItem As Integer = CInt(sender.Name.Substring(11, 1))
        Dim nCol As Integer = CInt(sender.Name.Substring(6, 1))
        Dim bDone As Boolean = False
        Do
            nCol -= 1
            Dim ftbTmp As FocusedTextBox.FocusedTextBox = _
                DirectCast(pnlParams.Controls("ftbCol" & nCol.ToString & "Item" & nItem.ToString),  _
                        FocusedTextBox.FocusedTextBox)
            If ftbTmp Is Nothing Then
                'Try again
                nCol = mnMAX_PARM_COL + 1
            Else
                If ftbTmp.Visible = True Then
                    bDone = True
                    ftbTmp.Focus()
                Else
                    'Try again
                End If
            End If
            If nCol = 0 Then 'catch-all
                'If it's in here and didn't set focus above, it's broke
                bDone = True
            End If
        Loop Until bDone = True

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
        ' 11/19/09  MSW     subTextboxKeypressHandler, subShowNewPage - Offer a login if they try to enter data without a login
        '********************************************************************************************
        Dim oFTB As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        If oFTB.ReadOnly Then
            If Char.IsNumber(e.KeyChar) OrElse Char.IsLetter(e.KeyChar) Then
                If Privilege = ePrivilege.None Then
                    Privilege = ePrivilege.Edit
                End If
            End If
        End If
    End Sub
    Private Sub pnlGraph_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles pnlGraph.Paint
        If mCalTable Is Nothing Then Exit Sub
        If DataLoaded = False Then Exit Sub
        mCalTable.DrawGraph(pnlGraph, e)
    End Sub

    Private Sub pnlGraph_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles pnlGraph.Resize
        pnlGraph.Invalidate()
    End Sub

    Private Sub dgTable_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles dgTable.EditingControlShowing
        AddHandler e.Control.KeyPress, AddressOf dgCell_KeyPress
    End Sub

    Private Sub dgCell_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        '********************************************************************************************
        'Description: check keypresses
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '    04/05/10   MSW     Do some checks here before it gets to endedit
        '    01/12/12   MSW     Allow entries longer than 4 characters
        '********************************************************************************************
        'For some reason the length defaults to 4.  Seems like there shoulf be another place to do this, but I coulcn't find it.
        Dim oSender As DataGridViewTextBoxEditingControl = DirectCast(sender, DataGridViewTextBoxEditingControl)
        oSender.MaxLength = 10
        Dim sVal As String = String.Empty
        Dim bAbort As Boolean = False
        Try
            If mEditCol >= 0 And mEditRow >= 0 Then
                If Char.IsSymbol(e.KeyChar) Or Char.IsLetter(e.KeyChar) Then
                    bAbort = True
                End If
                If (mtColumnCfg(mEditCol).UseDecimal = False) And (bAbort = False) Then
                    If Char.IsPunctuation(e.KeyChar) Then
                        bAbort = True
                    End If
                End If
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            bAbort = True
        End Try
        If bAbort Then
            e.Handled = True
        End If
    End Sub

End Class