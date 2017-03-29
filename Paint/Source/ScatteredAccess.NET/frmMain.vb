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
' Description: Scattered Access Manager for .NET 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: White
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                    Version
'    03/28/12   MSW     First version in .NET                                     4.01.03.00
'    04/19/12   MSW     Add a lot of error trapping                               4.01.03.01
'    05/02/12   MSW     Worked on comm failure management.                        4.01.03.02
'    09/20/12   MSW     Prevent duplicate instance from running                   4.01.03.03
'    10/10/12   MSW     colControllers_ConnectionStatusChange - uncomment         4.01.03.04
'                       crossthread check.  Remove PLC form references
'    10/16/12   MSW     Better error handling                                     4.01.03.05
'    04/16/13   MSW     Add Canadian language files                               4.01.05.00
'                       Standalone Changelog 
'    05/09/13   MSW     Robot Connection Status                                   4.01.05.01
'                       Looks like it's better to check the events than the current status,
'                       look at oCntr.RCMConnectStatus instead of just oCntr.Robot.IsConnected
'    05/31/13   MSW     Init with BSD active, since it usually is.
'    05/31/13   MSW     Update SA active tags after connect
'    06/05/13   MSW     Some more error handling updates                          4.01.05.02
'    06/24/13   MSW     Support reading variable structures                       4.01.05.03

'    07/09/13   MSW     Update and standardize logos                              4.01.05.04
'                       Skip display in this screen for faster startup
'    07/12/13   MSW     Check for robot object failure                            4.01.05.05
'    07/19/13   MSW     Bring back display grid, but delay it so it doesn't hold  4.01.05.06
'                       up startup
'    07/25/13   MSW     Clean up display grid                                     4.01.05.07
'    09/13/13   MSW     Still trying to make the error handling a little better.  4.01.05.08
'                       improve the management of the timers
'                       Get rid of the invoke from the robot object thread, just
'                       ignore the cross-thread stuff
'                       Also changed to just always read from the robots.  It's easier than
'                       trying to coordinate the reconnect with whichever screen wants it.
'    09/16/13   MSW     Cleanup shutdown, make sure no timeres trigger            4.01.05.09
'    09/30/13   MSW     Save screenshots as jpegs                                 4.01.06.00
'    11/21/13   MSW     Support out of sequence robot numbers                     4.01.06.01
'    01/06/14   MSW     fairfax mod build debug                                   4.01.06.02
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call          4.01.07.00
'    05/16/14   MSW     tmrDisplay_Tick-Only update display when visible          4.01.07.01
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Configuration.ConfigurationSettings
Imports Response = System.Windows.Forms.DialogResult
Imports ConnStat = FRRobotNeighborhood.FRERNConnectionStatusConstants
Imports System.Xml
Imports System.Xml.XPath
Imports FRRobot
Imports FRRobotNeighborhood


Module StartupModule
    '<dllimport("user32.dll")> _
    'Public Shared Function ShowWindowAsync(ByVal Hwnd As IntPtr, ByVal swCommand As Integer) As Integer

    'End Function
    Declare Function ShowWindowAsync Lib "user32.dll" ( _
                ByVal Hwnd As IntPtr, _
                ByVal swCommand As Int32) As IntPtr
    Public Sub Main()

        Dim oProcs() As Process = Process.GetProcessesByName("ScatteredAccess")
        If oProcs.Length <= 1 Then
            Application.Run(New frmMain)
        Else
            Dim oIPC As New Paintworks_IPC.clsInterProcessComm

            Dim sMessage(1) As String
            sMessage(1) = String.Empty
            sMessage(0) = "show"
            oIPC.WriteControlMsg(gs_COM_ID_SA, sMessage)

        End If


    End Sub
End Module


Friend Class frmMain
#Region " Declares "

    '******** Form Constants   **********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Friend msSCREEN_NAME As String = "ScatteredAccess"   ' <-- For password area change log etc.
    Friend msSCREEN_DUMP_NAME As String = "ScatteredAccess"
    Friend Const msSCREEN_DUMP_EXT As String = ".jpg"
    Friend Const msSCREEN_NAME_C As String = "ScatteredAccess"
    Private Const mnMAININX As Integer = 100
    Private Const mnSCREENINX As Integer = 101
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME_C & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME_C & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME_C & ".RobotStrings"
    Friend Const mbUSEROBOTS As Boolean = True
    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    Friend gsChangeLogArea As String = msSCREEN_NAME_C
    Private mnOldTab As Integer = 0
    Private colZones As clsZones = Nothing
    Private WithEvents colControllers As clsControllers = Nothing
    Private WithEvents colArms As clsArms = Nothing
    Private mRobot As clsArm = Nothing
    Private mbEventBlocker As Boolean = False
    Private mbRemoteZone As Boolean = False
    Private mbScreenLoaded As Boolean = False
    Private mbAbortForShutdown As Boolean = False
    Private mbDontChangeTabs As Boolean = False
    Private mbRobotNotAvailable() As Boolean = Nothing
    Private mPrintHtml As clsPrintHtml
    Private mbSARequestedExt() As Boolean = Nothing
    Private mbSARequestedCLB() As Boolean = Nothing
    Private mbSAActive() As Boolean = Nothing
    Private mbAbortAll As Boolean = False
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean = False
    Private mbEditsMade As Boolean = False
    Private mnProgress As Integer = 0
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    Private msCulture As String = "en-US" 'Default to english

    '******** End Property Variables    *************************************************************

    Private mSAConfig As clsScatteredAccessGlobal

    Const mnDGVCS_COL_NAME As Integer = 0
    Const mnDGVCS_COL_COMSTAT As Integer = 1
    Const mnDGVCS_COL_SA_STAT As Integer = 2
    Private mbResetFormat As Boolean = False
    Private mbStartupMessage2Sent As Boolean = False
    Private mbTmrDisplayBusy As Boolean = False
    Private mbTmrConnectBusy As Boolean = False
    Private mbTmrSAReadBusy As Boolean = False

    Friend WithEvents moPassword As clsPWUser

    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    Delegate Sub ControllerStatusChange(ByVal Controller As clsController)

    Private colSARequests As Collection = New Collection
    '********New program-to-program communication object******************************************
    Public WithEvents oIPC As Paintworks_IPC.clsInterProcessComm
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet)
    '********************************************************************************************
    'Message formats supported
    'Code in the sending porgram
    'Dim sMessage(1) As String
    'sMessage(0) = msSCREEN_NAME & ";SET"    '"SET" = turn on listed, turn the rest off
    '                       '"ON"  = Turn on specified robots (or ALL), leave the rest the same
    '                       '"OFF"  = Turn off specified robots (or ALL), leave the rest the same
    'sMessage(1) = "P1;P2;O3" ' Either semicolon separated list or "ALL"
    'oIPC.WriteControlMsg(gs_COM_ID_SA, sMessage)
    '
    ' Turn on P1,P2,O3, turn the rest off
    '   sMessage(0) = msSCREEN_NAME & ";SET"
    '   sMessage(1) = "P1;P2;O3"
    ' Turn on P1, don't change the rest
    '   sMessage(0) = msSCREEN_NAME & ";ON"
    '   sMessage(1) = "P1"
    ' Turn off all of them
    '   sMessage(0) = msSCREEN_NAME & ";OFF"
    '   sMessage(1) = "ALL"

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
            Try
                msCulture = value
                mLanguage.DisplayCultureString = value

                'Use current language text for screen labels
                Dim Void As Boolean = mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, _
                                                                    msBASE_ASSEMBLY_LOCAL, _
                                                                    String.Empty)

            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: Culture", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
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
            Try
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


                'If mPassword.CheckPassword(msSCREEN_NAME, Value, moPassword, moPrivilege) Then 'RJO 03/21/12
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
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: Privilege", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
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
            Try
                mbDataLoaded = Value
                If mbDataLoaded Then
                    'just finished loading reset & refresh
                    subShowNewPage()
                End If
                subEnableControls(True)

            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: DataLoaded", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
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
            Try
                mbEditsMade = Value
                subEnableControls(True)
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: Privilege", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
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
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: Progress", _
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
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: Status", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
    End Property

#End Region
#Region " Routines "
    Public Sub UpdateStatus(ByVal nprogress As Integer, ByVal sStatus As String)
        Try
            Status(True) = sStatus
            Progress = nprogress
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: UpdateStatus", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

        End Try
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


        'If EditsMade Then
        '    Dim lRet As DialogResult

        '    lRet = MessageBox.Show(gcsRM.GetString("csSAVEMSG1") & vbCrLf & _
        '                        gcsRM.GetString("csSAVEMSG"), gcsRM.GetString("csSAVE_CHANGES"), _
        '                        MessageBoxButtons.YesNoCancel, _
        '                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

        '    Select Case lRet
        '        Case Response.Yes 'Response.Yes
        '            subSaveData()
        '            Return True
        '        Case Response.Cancel
        '            'false aborts closing form , changing zone etc
        '            Status = gcsRM.GetString("csSAVE_CANCEL")
        '            Return False
        '        Case Else
        '            Return True
        '    End Select
        'Else
        Return True
        'End If
    End Function


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
        Try
            'this should be handled better
            Dim bTmp As Boolean = mbEventBlocker
            mbEventBlocker = True

            DataLoaded = False ' needs to be first
            subEnableControls(False)
            mbEventBlocker = bTmp
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subClearScreen", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
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
        ' 03/26/12  MSW     customize for scattered access management
        '********************************************************************************************
        Try
            Dim sTipText As String = String.Empty
            Dim sImgKey As String = String.Empty
            Dim bConnected As Boolean = False
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
                    bConnected = True
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
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoStatusBar", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
            For Each oDR As DataGridViewRow In dgvCntrStatus.Rows
                Try
                    If oDR.Cells(mnDGVCS_COL_NAME).Value.ToString = Controller.Name Then
                        If sTipText.Trim = "" Then
                            Debug.Print(Controller.RCMConnectStatus.ToString)
                        End If
                        oDR.Cells(mnDGVCS_COL_COMSTAT).Value = sTipText
                        If bConnected Then
                            Dim sMessage(1) As String
                            sMessage(0) = "poststatusmsg"
                            sMessage(1) = gpsRM.GetString("psSTARTUP_MSG_2") & Controller.Name
                            Status = sMessage(1)
                            oIPC.WriteControlMsg(gs_COM_ID_PW_MAIN, sMessage)
                            Dim nStillToSetup As Integer = 0
                            For Each oCntr As clsController In colControllers
                                If oCntr.Tag Is Nothing Then
                                    nStillToSetup = nStillToSetup + 1
                                End If
                            Next
                            'Setup scattered access
                            Dim oSAItems As clsScatteredAccessItems = New clsScatteredAccessItems(mSAConfig, Controller, eSAReadOrDisplay.eSAReadOnly)
                            Controller.Tag = oSAItems
                            oDR.Cells(mnDGVCS_COL_SA_STAT).Value = gcsRM.GetString("csAVAILABLE")
                            'Dim oRows As Collection = New Collection
                            'For Each oArm As clsArm In colArms
                            '    Dim oSAItemsArm As clsScatteredAccessItems = New clsScatteredAccessItems(mSAConfig, oArm, eSAReadOrDisplay.eSADisplayOnly)
                            '    oArm.Tag = oSAItemsArm
                            '    For Each oItem As clsScatteredAccessItem In oSAItemsArm.Items
                            '        Dim bFound As Boolean = False
                            '        If oRows.Count > 0 Then
                            '            Try
                            '                For Each oRow As String In oRows
                            '                    If oRow = oItem.Name Then
                            '                        bFound = True
                            '                    End If
                            '                Next
                            '            Catch ex As Exception
                            '            End Try
                            '        End If
                            '        If bFound = False Then
                            '            oRows.Add(oItem.Name)
                            '        End If
                            '    Next
                            'Next

                            Progress = 85
                            'dgvData.RowCount = oRows.Count + 1
                            'For nRow As Integer = 0 To oRows.Count - 1
                            '    Dim sName2 As String = oRows(nRow + 1).ToString
                            '    dgvData.Rows(nRow).HeaderCell.Value = sName2
                            '    For Each oArm As clsArm In colArms
                            '        Dim oSAItemsArm As clsScatteredAccessItems = DirectCast(oArm.Tag, clsScatteredAccessItems)
                            '        If oSAItemsArm.Items.Count > 0 Then
                            '            Try
                            '                If oSAItemsArm.Item(sName2) IsNot Nothing Then
                            '                    oSAItemsArm.Item(sName2).Tag = nRow
                            '                End If
                            '            Catch ex As Exception
                            '            End Try
                            '        End If
                            '    Next
                            'Next
                            'Wait for it to finish - it's more stable
                            If nStillToSetup = 1 Then
                                'Let main continue while setting up the last one.
                                sMessage(0) = "scattaccessmanload"
                                sMessage(1) = True.ToString
                                oIPC.WriteControlMsg(gs_COM_ID_PW_MAIN, sMessage)
                                mbStartupMessage2Sent = True
                            End If
                        Else
                            oDR.Cells(mnDGVCS_COL_SA_STAT).Value = gcsRM.GetString("csUNAVAILABLE")
                            Controller.Tag = Nothing
                        End If
                        Exit For
                    End If

                Catch ex As Exception
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoStatusBar", _
                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
            Next
            DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoStatusBar", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
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
        Try
            Dim bRestOfControls As Boolean = False
            Dim bEditsMade As Boolean = EditsMade
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
                btnAdd.Enabled = False
                btnClear.Enabled = False
                mnuPrintFile.Enabled = False

                btnUtilities.Enabled = False
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
                        mnuPrintFile.Enabled = False
                        btnClear.Enabled = DataLoaded
                        btnUtilities.Enabled = False
                    Case ePrivilege.Edit
                        btnSave.Enabled = bEditsMade
                        btnUndo.Enabled = bEditsMade
                        btnPrint.Enabled = DataLoaded
                        btnMultiView.Enabled = True
                        btnChangeLog.Enabled = True
                        btnStatus.Enabled = True
                        bRestOfControls = True
                        btnCopy.Enabled = False
                        btnClear.Enabled = DataLoaded
                        mnuPrintFile.Enabled = DataLoaded
                        btnUtilities.Enabled = (True And DataLoaded)
                    Case ePrivilege.Copy
                        btnCopy.Enabled = DataLoaded
                        btnSave.Enabled = bEditsMade
                        btnUndo.Enabled = bEditsMade
                        btnPrint.Enabled = DataLoaded
                        btnMultiView.Enabled = True
                        btnChangeLog.Enabled = True
                        btnStatus.Enabled = True
                        bRestOfControls = True
                        btnClear.Enabled = DataLoaded
                        mnuPrintFile.Enabled = DataLoaded
                        btnUtilities.Enabled = (True And DataLoaded)
                End Select
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subEnableControls", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

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

        Try

            With gpsRM
                dgvCntrStatus.ColumnCount = 3
                dgvCntrStatus.Columns.Item(mnDGVCS_COL_NAME).HeaderText = gpsRM.GetString("psCONTROLLER")
                dgvCntrStatus.Columns.Item(mnDGVCS_COL_COMSTAT).HeaderText = gpsRM.GetString("psCOMM_STAT")
                dgvCntrStatus.Columns.Item(mnDGVCS_COL_SA_STAT).HeaderText = gpsRM.GetString("psSA_STATUS")
            End With
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
        ' 05/31/13  MSW     Init with BSD active, since it usually is.
        ' 11/21/13  MSW     Support out of sequence robot numbers
        '********************************************************************************************
        Dim bAbort As Boolean = False
        Dim sMessage(1) As String
        Try

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            DataLoaded = False
            mbScreenLoaded = False

            Status = gcsRM.GetString("csINITALIZING")
            Progress = 1

            Me.Text = gpsRM.GetString("psSCREENCAPTION")

            colZones = New clsZones(String.Empty)

            subProcessCommandLine()


            Progress = 5

            'init the old password for now
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

            Progress = 10

            mPrintHtml = New clsPrintHtml(msSCREEN_NAME, True, 30)

            '********New program-to-program communication object******************************************
            oIPC = New Paintworks_IPC.clsInterProcessComm(gs_COM_ID_SA)
            '********************************************************************************************


            mScreenSetup.InitializeForm(Me)
            'Call once to clean up the table
            subFormatScreenLayout()
            Progress = 15
            subInitFormText()


            Progress = 20
            subEnableControls(True)
            colControllers = New clsControllers(colZones, True)
            SetUpStatusStrip(Me, colControllers)
            If mPWCommon.InIDE = False Then
                Me.WindowState = FormWindowState.Minimized
            End If
            colArms = LoadArmCollection(colControllers)
            Progress = 35

            dgvCntrStatus.Rows.Clear()
            ReDim mbSAActive(colControllers.Count - 1)
            For nCntr As Integer = 0 To (colControllers.Count - 1)
                Dim sParamArray(2) As String
                sParamArray(mnDGVCS_COL_NAME) = colControllers(nCntr).Name
                sParamArray(mnDGVCS_COL_COMSTAT) = gcsRM.GetString("csUNKNOWN")
                sParamArray(mnDGVCS_COL_SA_STAT) = gcsRM.GetString("csUNKNOWN")
                dgvCntrStatus.Rows.Add(sParamArray)
                Dim bRead As Boolean = False
                colControllers(nCntr).Tag2 = bRead
                mbSAActive(nCntr) = False
            Next
            Progress = 50
            Dim nMaxArm As Integer = colArms(colArms.Count - 1).RobotNumber
            ReDim mbSARequestedExt(nMaxArm - 1)
            ReDim mbSARequestedCLB(nMaxArm - 1)
            dgvData.Columns.Clear()
            dgvData.Rows.Clear()

            Dim sParamArrray(nMaxArm - 1) As String
            For Each oArm As clsArm In colArms
                clbArms.Items.Add(oArm.Name)
                'MSW force the list
                mbSARequestedExt(oArm.RobotNumber - 1) = True
                mbSARequestedCLB(oArm.RobotNumber - 1) = False
                dgvData.Columns.Add(oArm.Name, oArm.Name & "             ")
                sParamArrray(oArm.RobotNumber - 1) = String.Empty
            Next
            Progress = 55

            sMessage(0) = "poststatusmsg"
            sMessage(1) = gpsRM.GetString("psSTARTUP_MSG_1")
            Status = sMessage(1)
            Debug.Print(sMessage(1))
            oIPC.WriteControlMsg(gs_COM_ID_PW_MAIN, sMessage)

            mSAConfig = New clsScatteredAccessGlobal


            Application.DoEvents()
            colControllers.RefreshConnectionStatus()
            Application.DoEvents()

            sMessage(0) = "scattaccessmanload"
            sMessage(1) = True.ToString
            Debug.Print(sMessage(1))
            oIPC.WriteControlMsg(gs_COM_ID_PW_MAIN, sMessage)

            Progress = 60
            tmrSARead.Interval = mSAConfig.UpdateRatems
            tmrSARead.Enabled = True
            tmrConnect.Enabled = True

            dgvCntrStatus.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells)
            dgvCntrStatus.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders)
            dgvCntrStatus.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
            dgvData.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells)
            dgvData.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders)
            dgvData.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)

            tmrDisplay.Interval = mSAConfig.UpdateRatems
            '            tmrDisplay.Enabled = True
            clbArms.Enabled = True

            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound

            'Init with BSD active, since it usually is.
            subDoScreenAction()
            ' refresh lock pic
            DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
            mbScreenLoaded = True

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitializeForm", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        Finally
            If Not bAbort Then
                Progress = 100

            End If
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
        Try
            Dim sCultureArg As String = "/culture="

            'If a culture string has been passed in, set the current culture (display language)
            For Each s As String In My.Application.CommandLineArgs
                If s.ToLower.StartsWith(sCultureArg) Then
                    Culture = s.Remove(0, sCultureArg.Length)
                End If
            Next
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subProcessCommandLine", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
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
        ' 11/01/10  MSW     Don't try to pick a device for the change log.  
        ' 04/16/13  MSW     Standalone ChangeLog                                          4.01.05.00
        '********************************************************************************************

        Try
            mPWCommon.subShowChangeLog(colZones.CurrentZone, gsChangeLogArea, msSCREEN_NAME, gcsRM.GetString("csALL"), _
                                       gcsRM.GetString("csALL"), mDeclares.eParamType.Valves, nIndex, mbUSEROBOTS, True, oIPC)
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subShowChangeLog", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

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
        '********************************************************************************************
        'Not needed for this screen

    End Sub



#End Region
#Region " Events "




    '********New program-to-program communication object******************************************
    Private Sub oIPC_NewMessage(ByVal Schema As String, ByVal DS As DataSet) Handles oIPC.NewMessage
        Try
            If Me.InvokeRequired Then
                Dim dNewMessage As New NewMessage_CallBack(AddressOf oIPC_NewMessage)
                Me.BeginInvoke(dNewMessage, New Object() {Schema, DS})
            Else
                Dim DR As DataRow = Nothing

                Select Case Schema
                    Case oIPC.CONTROL_MSG_SCHEMA
                        DR = DS.Tables(Paintworks_IPC.clsInterProcessComm.sTABLE).Rows(0)
                        Call subDoScreenAction(DR)
                    Case Else
                End Select
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: oIPC_NewMessage", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
    '********************************************************************************************
    Private Sub subInitRequest(ByRef oRequest As SARequests, ByRef sProcName As String)
        Try
            If oRequest Is Nothing Then
                oRequest = New SARequests
            End If
            oRequest.sProcName = sProcName
            Dim oProcs() As Process = Process.GetProcessesByName(sProcName)
            If oProcs.Length > 0 Then
                oRequest.nProcID = oProcs(0).Id
            Else
                oProcs = Process.GetProcessesByName(sProcName & ".vshost")
                If oProcs.Length > 0 Then
                    oRequest.nProcID = oProcs(0).Id
                Else
                    oRequest.nProcID = 0
                End If
            End If
            oRequest.sRequests = String.Empty

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitRequest", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
    Private Function sGetAllParam() As String
        '********************************************************************************************
        'Description: build param string for all arms
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            Dim sParam As String = String.Empty
            For Each oArm As clsArm In colArms
                sParam = sParam & oArm.Name & ";"
            Next
            Return sParam
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: sGetAllParam", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Return Nothing
        End Try
    End Function
    Private Sub subDoScreenAction(Optional ByRef oDR As DataRow = Nothing)
        '********************************************************************************************
        'Description: Get messages from client programs 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/31/13  MSW     Init with BSD active, since it usually is.
        '********************************************************************************************
        Try
            Dim sAction() As String = Nothing
            Dim sParam As String = String.Empty
            If oDR Is Nothing Then
                ReDim sAction(1)
                sAction(0) = "KEEPALIVE"
                sAction(1) = "ON"
                sParam = "ALL"
            Else
                sAction = Split(oDR.Item(Paintworks_IPC.clsInterProcessComm.sCOL_ACTION).ToString, ";")
                sParam = oDR.Item(Paintworks_IPC.clsInterProcessComm.sCOL_PARAMETER).ToString
            End If
            If sAction.GetUpperBound(0) > 0 Then
                Dim sFrom As String = sAction(0)
                Dim oRequest As SARequests = Nothing
                Dim nColIndex As Integer = -1
                For nItem As Integer = 1 To colSARequests.Count
                    Dim oReqTmp As SARequests = DirectCast(colSARequests(nItem), SARequests)
                    If sFrom = oReqTmp.sProcName Then
                        oRequest = oReqTmp
                        nColIndex = nItem
                        Exit For
                    End If
                Next
                Dim sReq() As String = Split(sParam, ";")
                Dim bAll As Boolean = (sParam = "ALL")
                Select Case sAction(1)
                    Case "SET"
                        If oRequest Is Nothing Then
                            subInitRequest(oRequest, sFrom)
                        End If
                        If bAll Then
                            oRequest.sRequests = sGetAllParam()
                        Else
                            oRequest.sRequests = sParam
                        End If
                        If nColIndex = -1 Then
                            colSARequests.Add(oRequest)
                        End If
                    Case "ON"
                        If oRequest Is Nothing Then
                            subInitRequest(oRequest, sFrom)
                        End If
                        If bAll Then
                            oRequest.sRequests = sGetAllParam()
                        Else
                            If oRequest.sRequests = String.Empty Then
                                oRequest.sRequests = sParam
                            Else
                                Dim sOld() As String = Split(oRequest.sRequests, ";")
                                For Each sStrReq As String In sReq
                                    Dim bAdd As Boolean = True
                                    For Each oStrOld As String In sOld
                                        If oStrOld = sStrReq Then
                                            bAdd = False
                                            Exit For
                                        End If
                                    Next
                                    oRequest.sRequests = oRequest.sRequests & sStrReq & ";"
                                Next
                            End If
                        End If
                        If nColIndex = -1 Then
                            colSARequests.Add(oRequest)
                        End If
                    Case "OFF"
                        If (oRequest IsNot Nothing) And nColIndex <> -1 Then
                            'Only if it's already in the collection
                            If bAll Then
                                colSARequests.Remove(nColIndex)
                            Else
                                Dim sOld() As String = Split(oRequest.sRequests, ";")
                                For Each oStrOld As String In sOld
                                    Dim bAdd As Boolean = True
                                    For Each sStrReq As String In sReq
                                        If oStrOld = sStrReq Then
                                            bAdd = False
                                            Exit For
                                        End If
                                    Next
                                    If bAdd Then
                                        oRequest.sRequests = oRequest.sRequests & oStrOld & ";"
                                    End If
                                Next
                            End If
                        End If
                End Select
            Else
                Select Case oDR.Item(Paintworks_IPC.clsInterProcessComm.sCOL_ACTION).ToString.ToLower
                    Case "culturestring"
                        Culture = oDR.Item(Paintworks_IPC.clsInterProcessComm.sCOL_PARAMETER).ToString.ToLower
                    Case "show"
                        Me.WindowState = FormWindowState.Normal
                    Case "hide"
                        Me.WindowState = FormWindowState.Minimized
                    Case "close"
                        'Get outta Dodge
                        mbAbortAll = True
                        tmrDisplay.Enabled = False
                        tmrConnect.Enabled = False
                        tmrSARead.Enabled = False



                        Application.Exit()
                        Me.Close()

                End Select
            End If
            subSummarizeRequests()
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoScreenAction", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subSummarizeRequests()
        '********************************************************************************************
        'Description: summarize external requests
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/21/13  MSW     Support out of sequence robot numbers
        '********************************************************************************************
        Try
            For Each oArm As clsArm In colArms
                Dim bAdd As Boolean = False
                For Each oSAReq As SARequests In colSARequests
                    Dim sReq() As String = Split(oSAReq.sRequests, ";")
                    For Each sTmp As String In sReq
                        If oArm.Name = sTmp Then
                            bAdd = True
                            Exit For
                        End If

                    Next
                    If bAdd Then
                        Exit For
                    End If
                Next
                mbSARequestedExt(oArm.RobotNumber - 1) = bAdd
            Next

            For nItem As Integer = 0 To clbArms.Items.Count - 1
                If mbSARequestedCLB(colArms(nItem).RobotNumber - 1) Then
                    clbArms.SetItemCheckState(nItem, CheckState.Checked)
                ElseIf mbSARequestedExt(colArms(nItem).RobotNumber - 1) Then
                    clbArms.SetItemCheckState(nItem, CheckState.Indeterminate)
                Else
                    clbArms.SetItemCheckState(nItem, CheckState.Unchecked)
                End If
            Next
            subSetControllerReadRequests()
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subSummarizeRequests", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub


    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '********************************************************************************************
        'Description: Shutdown.  See if it needs a save
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Select Case e.CloseReason
                Case CloseReason.UserClosing
                    e.Cancel = True
                    Me.WindowState = FormWindowState.Minimized
            End Select

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: frmMain_FormClosing", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
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
        '********************************************************************************************
        Try
            Select Case e.ClickedItem.Name ' case sensitive
                Case "btnClose"
                    'Check to see if we need to save is performed in  bAskForSave
                    Me.WindowState = FormWindowState.Minimized
                    'End
                Case "btnStatus"
                    lstStatus.Visible = Not lstStatus.Visible


                Case "btnPrint"
                    Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                    If o.DropDownButtonPressed Then Exit Sub
                    'bPrintdoc(True)

            End Select

            tlsMain.Refresh()

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: tlsMain_ItemClicked", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
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
        '********************************************************************************************


        Try
            If colArms Is Nothing Then Exit Sub
            If colArms.Count = 0 Then Exit Sub
            For i As Integer = colArms.Count - 1 To 0
                colArms.Remove(colArms(i))
            Next
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: frmMain_Closing", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Public Sub New()
        Try
            'for language support
            mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, msBASE_ASSEMBLY_LOCAL, _
                                                msROBOT_ASSEMBLY_LOCAL)
            mbEventBlocker = True
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            mbEventBlocker = False
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: New", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
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
        ' 10/16/12  MSW     Now we ignore them instead
        '********************************************************************************************
        'this is needed with old RCM object raising events
        'and can hopefully be removed when I learn how to program
        'Control.CheckForIllegalCrossThreadCalls = False
        Try
            tmrStartup.Enabled = False
            Debug.Print(Controller.Name & ": " & Controller.RCMConnectStatus.ToString)
            'Check for call from the robot object thread
            If Me.stsStatus.InvokeRequired Then
                Dim dCntrStat As New ControllerStatusChange(AddressOf colControllers_ConnectionStatusChange)
                Me.BeginInvoke(dCntrStat, New Object() {Controller})
            Else
            subDoStatusBar(Controller)
            subSummarizeRequests()
            subSetControllerReadRequests()
            End If

            'Trace.WriteLine("frmmain connection status change event - " & Controller.Name & " " & _
            '                        Controller.RCMConnectStatus.ToString


            'Control.CheckForIllegalCrossThreadCalls = True
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: colControllers_ConnectionStatusChange", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

        tmrStartup.Enabled = Not (mbStartupMessage2Sent)
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
    Private Sub btnFunction_DropDownOpening(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFunction.DropDownOpening
        ''********************************************************************************************
        ''Description:  this was needed to enable the menus for some reason
        ''                  now handled in dostatusbar 2
        ''
        ''Parameters: none
        ''Returns:    none
        ''
        ''Modification history:
        ''
        '' Date      By      Reason
        ''********************************************************************************************
        Try
            Dim cachePriviledge As ePrivilege
            Dim bRem As Boolean = False
            Dim bAllow As Boolean = False


            If LoggedOnUser <> String.Empty Then

                'If moPrivilege.Privilege = String.Empty Then 'RJO 03/15/12
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
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: btnFunction_DropDownOpening", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

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
        End If
    End Sub
    Private Sub moPassword_LogOut() Handles moPassword.Logout
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
        Try
            Dim sKeyValue As String = String.Empty

            'Trap Function Key presses
            If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
                Select Case e.KeyCode
                    Case Keys.F1
                        'Help Screen request
                        sKeyValue = "btnHelp"
                        subLaunchHelp(gs_HELP_SCATTEREDACCESS, oIPC)
                    Case Keys.F2
                        'Screen Dump request
                        Dim oSC As New ScreenShot.ScreenCapture
                        Dim sDumpPath As String = String.Empty
                        mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                        sDumpPath = sDumpPath & msSCREEN_DUMP_NAME & msSCREEN_DUMP_EXT
                        oSC.CaptureWindowToFile(Me.Handle, sDumpPath, Imaging.ImageFormat.Jpeg)

                    Case Keys.Escape

                        If mPWCommon.InIDE = False Then
                            Me.WindowState = FormWindowState.Minimized
                        End If

                    Case Else

                End Select
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: frmMain_KeyDown", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub tmrDisplay_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrDisplay.Tick
        '********************************************************************************************
        'Description: Update scattered access dispkay (client side)
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '    05/09/13   MSW     Robot Connection Status
        '                       Looks like it's better to check the events than the current status,
        '                       look at oCntr.RCMConnectStatus instead of just oCntr.Robot.IsConnected
        '    07/12/13   MSW     Check for robot object failure
        '    11/21/13   MSW     Support out of sequence robot numbers
        '    05/16/14   MSW     tmrDisplay_Tick-Only update display when visible
        '********************************************************************************************
        Try
            If mbTmrDisplayBusy Then
                Exit Sub
            End If
            If Me.WindowState <> FormWindowState.Minimized Then
                Try
            mbTmrDisplayBusy = True
            tmrDisplay.Enabled = False
            For nArm As Integer = 0 To colArms.Count - 1
                If colArms(nArm).Controller.RCMConnectStatus = ConnStat.frRNConnected Then
                    '    07/12/13   MSW     Check for robot object failure
                    Dim bConnected As Boolean = False
                    Try
                        bConnected = colArms(nArm).Controller.Robot.IsConnected
                    Catch ex As Exception
                        mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: tmrDisplay_Tick - moRobot reconnect", _
                               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                        Application.DoEvents()
                        Threading.Thread.Sleep(500)
                        Application.DoEvents()
                        colArms(nArm).Controller.subConnect()
                        Application.DoEvents()
                        If mbAbortAll Then
                            Exit Sub
                        End If
                    End Try
                    Dim nRobotNumber As Integer = colArms(nArm).RobotNumber
                    If bConnected And (mbSARequestedCLB(nRobotNumber - 1) Or mbSARequestedExt(nRobotNumber - 1)) Then
                        Dim oSAItems As clsScatteredAccessItems = DirectCast(colArms(nArm).Tag, clsScatteredAccessItems)
                        If oSAItems IsNot Nothing AndAlso oSAItems.SetupOK Then
                            For Each oSAItem As clsScatteredAccessItem In oSAItems.Items
                                Dim nRow As Integer = DirectCast(oSAItem.Tag, Integer)
                                dgvData.Rows(nRow).Cells(nArm).Value = oSAItem.Value
                            Next
                        End If
                    End If
                End If
                Application.DoEvents()
                If mbAbortAll Then
                    Exit Sub
                End If
            Next
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: tmrDisplay_Tick", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
        tmrDisplay.Enabled = True
        mbTmrDisplayBusy = False
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: tmrDisplay_Tick", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
    Private Sub tmrSARead_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrSARead.Tick
        '********************************************************************************************
        'Description: Scattered access read and setup timer event
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '    05/09/13   MSW     Robot Connection Status
        '                       Looks like it's better to check the events than the current status,
        '                       look at oCntr.RCMConnectStatus instead of just oCntr.Robot.IsConnected
        '    07/12/13   MSW     Check for robot object failure
        '********************************************************************************************
        Try
            If mbTmrSAReadBusy Then
                Exit Sub
            End If
            mbTmrSAReadBusy = True
            tmrSARead.Enabled = False
            For Each oCntr As clsController In colControllers
                Dim oSAItems As clsScatteredAccessItems = DirectCast(oCntr.Tag, clsScatteredAccessItems)
                Dim bRead As Boolean = False
                If oCntr.Tag2 IsNot Nothing Then
                    Try
                        bRead = DirectCast(oCntr.Tag2, Boolean)
                    Catch ex As Exception
                        bRead = False
                    End Try
                    If oCntr.RCMConnectStatus = ConnStat.frRNConnected Then
                        '    07/12/13   MSW     Check for robot object failure
                        Try
                            bRead = oCntr.Robot.IsConnected
                        Catch ex As Exception
                            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: tmrSARead_Tick - moRobot reconnect", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            Application.DoEvents()
                            Threading.Thread.Sleep(1000)
                            Application.DoEvents()
                            oCntr.subConnect()
                            Application.DoEvents()
                            bRead = oCntr.Robot.IsConnected
                            If mbAbortAll Then
                                Exit Sub
                            End If
                        End Try
                        If bRead And (oSAItems Is Nothing OrElse oSAItems.SetupOK = False OrElse oSAItems.ObjectOK = False) Then
                            bRead = False
                        End If
                    End If

                    If bRead Then
                        oSAItems.Refresh()
                        'mbSAActive(nCntr) = True
                    End If
                End If
                Application.DoEvents()
                If mbAbortAll Then
                    Exit Sub
                End If
            Next

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: tmrSARead_Tick", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
        mbTmrSAReadBusy = False
        tmrSARead.Enabled = True
    End Sub
    Private Sub clbArms_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles clbArms.SelectedIndexChanged
        '********************************************************************************************
        'Description: robot checkbox selection management
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Try
            For nItem As Integer = 0 To clbArms.Items.Count - 1
                Select Case clbArms.GetItemCheckState(nItem)
                    Case CheckState.Checked
                        mbSARequestedCLB(colArms(nItem).RobotNumber - 1) = True
                    Case CheckState.Indeterminate
                        mbSARequestedCLB(colArms(nItem).RobotNumber - 1) = False
                        If mbSARequestedExt(colArms(nItem).RobotNumber - 1) = False Then
                            clbArms.SetItemCheckState(nItem, CheckState.Unchecked)
                            'ClearColumn
                            subClearColumn(nItem)
                        End If
                    Case CheckState.Unchecked
                        mbSARequestedCLB(colArms(nItem).RobotNumber - 1) = False
                        If mbSARequestedExt(colArms(nItem).RobotNumber - 1) Then
                            clbArms.SetItemCheckState(nItem, CheckState.Indeterminate)
                        Else
                            'ClearColumn
                            subClearColumn(nItem)
                        End If
                End Select
            Next

            mbResetFormat = True
            subSetControllerReadRequests()
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: clbArms_SelectedIndexChanged", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
    Private Sub subClearColumn(ByVal nColumn As Integer)
        '********************************************************************************************
        'Description: Clear out a column when the robot is deselected
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Try
            For Each oDR As DataGridViewRow In dgvData.Rows
                oDR.Cells(nColumn).Value = String.Empty
            Next
            dgvData.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells)
            dgvData.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders)
            dgvData.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subClearColumn", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
    Private Sub subSetControllerReadRequests()
        '********************************************************************************************
        'Description: combine checkbox and remote requests and set status in the controller object
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '    10/16/12   MSW     Better error handling
        '    05/09/13   MSW     Robot Connection Status
        '                       Looks like it's better to check the events than the current status,
        '                       look at oCntr.RCMConnectStatus instead of just oCntr.Robot.IsConnected
        '    07/12/13   MSW     Check for robot object failure
        '    11/21/13   MSW     Support out of sequence robot numbers
        '********************************************************************************************
        Try
            For Each oCntr As clsController In colControllers
                Dim bOn As Boolean = False
                If oCntr.Tag2 IsNot Nothing Then
                    Try
                        bOn = DirectCast(oCntr.Tag2, Boolean)
                    Catch ex As Exception
                    End Try
                End If
                bOn = False
                Dim oSAItems As clsScatteredAccessItems = DirectCast(oCntr.Tag, clsScatteredAccessItems)
                If oCntr.RCMConnectStatus = ConnStat.frRNConnected AndAlso _
                    oSAItems IsNot Nothing AndAlso oSAItems.ObjectOK Then
                    '    07/12/13   MSW     Check for robot object failure
                    Dim bConnected As Boolean = True
                    Try
                        bConnected = oCntr.Robot.IsConnected
                    Catch ex As Exception
                        mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subSetControllerReadRequests - moRobot reconnect", _
                               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                        Application.DoEvents()
                        Threading.Thread.Sleep(1000)
                        Application.DoEvents()
                        oCntr.subConnect()
                        Application.DoEvents()
                        bConnected = oCntr.Robot.IsConnected
                    End Try
                    If bConnected Then
                        For Each oArm As clsArm In oCntr.Arms
                            If mbSARequestedCLB(oArm.RobotNumber - 1) Or mbSARequestedExt(oArm.RobotNumber - 1) Then
                                bOn = True
                                Exit For
                            End If
                        Next
                    End If
                    oCntr.Tag2 = bOn
                Else
                    For Each oArm As clsArm In oCntr.Arms

                        mbSARequestedCLB(oArm.RobotNumber - 1) = False
                    Next
                    oCntr.Tag2 = False
                End If
            Next
            For nArm As Integer = 0 To colArms.Count - 1
                If mbSARequestedCLB(colArms(nArm).RobotNumber - 1) Then
                    clbArms.SetItemCheckState(nArm, CheckState.Checked)
                Else
                    If mbSARequestedExt(colArms(nArm).RobotNumber - 1) Then
                        clbArms.SetItemCheckState(nArm, CheckState.Indeterminate)
                    Else
                        clbArms.SetItemCheckState(nArm, CheckState.Unchecked)
                    End If
                End If
            Next
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subSetControllerReadRequests", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub


    Private Sub tmrConnect_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrConnect.Tick
        '********************************************************************************************
        'Description: Scattered access connection timer event
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '    05/09/13   MSW     Robot Connection Status
        '                       Looks like it's better to check the events than the current status,
        '                       look at oCntr.RCMConnectStatus instead of just oCntr.Robot.IsConnected
        '    05/31/13   MSW     Update SA active tags after connect
        '    07/12/13   MSW     Check for robot object failure
        '    07/25/13   MSW     Clean up display grid                                     4.01.05.07
        '********************************************************************************************
        Try
            If mbTmrConnectBusy Then
                Exit Sub
            End If
            mbTmrConnectBusy = True
            tmrDisplay.Enabled = False
            tmrConnect.Enabled = False
            tmrSARead.Enabled = False
            ReDim mbSAActive(colControllers.Count - 1)
            For nCntr As Integer = 0 To (colControllers.Count - 1)
                Dim oCntr As clsController = colControllers(nCntr)
                Dim nDataRow As Integer = -1
                For nRow As Integer = 0 To dgvCntrStatus.RowCount - 1
                    Try
                        If dgvCntrStatus.Rows(nRow).Cells(mnDGVCS_COL_NAME).Value.ToString = oCntr.Name Then
                            nDataRow = nRow
                            Exit For
                        End If
                    Catch ex As Exception
                    End Try
                Next
                Dim oSAItems As clsScatteredAccessItems = DirectCast(oCntr.Tag, clsScatteredAccessItems)
                Dim bRead As Boolean = False
                If oCntr.Tag2 IsNot Nothing Then
                    Try
                        bRead = DirectCast(oCntr.Tag2, Boolean)
                    Catch ex As Exception
                        bRead = False
                    End Try
                End If
                If oCntr.RCMConnectStatus = ConnStat.frRNConnected Then
                    '    07/12/13   MSW     Check for robot object failure
                    Dim bConnected As Boolean = False
                    Try
                        bConnected = oCntr.Robot.IsConnected
                    Catch ex As Exception
                        mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: tmrConnect_Tick - moRobot reconnect", _
                               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                        Application.DoEvents()
                        Threading.Thread.Sleep(1000)
                        Application.DoEvents()
                        oCntr.subConnect()
                        Application.DoEvents()
                        bConnected = oCntr.Robot.IsConnected
                    End Try
                    If bConnected AndAlso oSAItems Is Nothing Then
                        Status = gpsRM.GetString("psSETUPSA") & oCntr.Name
                        'Setup scattered access
                        dgvCntrStatus.Rows(nDataRow).Cells(mnDGVCS_COL_SA_STAT).Value = gcsRM.GetString("csCONNECTING")
                        oSAItems = New clsScatteredAccessItems(mSAConfig, oCntr, eSAReadOrDisplay.eSAReadOnly)
                        Application.DoEvents()
                        oSAItems.SetupSA(oCntr.Robot)
                        Application.DoEvents()
                        oSAItems.Refresh()
                        Application.DoEvents()
                        oCntr.Tag = oSAItems
                        If oSAItems.SetupOK And oSAItems.ObjectOK Then
                            dgvCntrStatus.Rows(nDataRow).Cells(mnDGVCS_COL_SA_STAT).Value = gcsRM.GetString("csCONNECTED")
                            Status = gpsRM.GetString("psSETUPSA_CONNECTED") & oCntr.Name
                            mbSAActive(oCntr.ControllerNumber - 1) = True
                        Else
                            dgvCntrStatus.Rows(nDataRow).Cells(mnDGVCS_COL_SA_STAT).Value = gcsRM.GetString("csERROR")
                            Status = gpsRM.GetString("psSETUPSA_FAILED") & oCntr.Name
                            mbSAActive(oCntr.ControllerNumber - 1) = False
                        End If
                        Application.DoEvents()
                    Else
                        Application.DoEvents()
                        If oSAItems.ObjectOK = False Then
                            Application.DoEvents()
                            oSAItems.SetupSA(oCntr.Robot)
                            Application.DoEvents()
                            oSAItems.Refresh()
                            Application.DoEvents()
                            oCntr.Tag = oSAItems
                            If oSAItems.SetupOK And oSAItems.ObjectOK Then
                                dgvCntrStatus.Rows(nDataRow).Cells(mnDGVCS_COL_SA_STAT).Value = gcsRM.GetString("csCONNECTED")
                                Status = gpsRM.GetString("psSETUPSA_CONNECTED") & oCntr.Name
                                mbSAActive(nCntr) = True
                            Else
                                dgvCntrStatus.Rows(nDataRow).Cells(mnDGVCS_COL_SA_STAT).Value = gcsRM.GetString("csERROR")
                                Status = gpsRM.GetString("psSETUPSA_FAILED") & oCntr.Name
                                mbSAActive(nCntr) = False
                            End If
                            Application.DoEvents()
                        Else
                            'See if the arms are set up for display
                            Dim oRows As Collection = New Collection
                            For Each oArm As clsArm In colArms
                                Dim oSAItemsArm As clsScatteredAccessItems = DirectCast(oArm.Tag, clsScatteredAccessItems)
                                If oSAItemsArm Is Nothing Then
                                    oSAItemsArm = New clsScatteredAccessItems(mSAConfig, oArm, eSAReadOrDisplay.eSADisplayOnly)
                                    oArm.Tag = oSAItemsArm
                                    For Each oItem As clsScatteredAccessItem In oSAItemsArm.Items
                                        Dim bFound As Boolean = False
                                        If oRows.Count > 0 Then
                                            Try
                                                For Each oRow As String In oRows
                                                    If oRow = oItem.Name Then
                                                        bFound = True
                                                    End If
                                                Next
                                            Catch ex As Exception
                                            End Try
                                        End If
                                        If bFound = False Then
                                            oRows.Add(oItem.Name)
                                        End If
                                    Next
                                End If
                            Next
                            '    07/25/13   MSW     Clean up display grid                                     4.01.05.07
                            dgvData.RowCount = dgvData.RowCount + oRows.Count '+1 
                            For nRow As Integer = 0 To oRows.Count - 1
                                Dim sName2 As String = oRows(nRow + 1).ToString
                                dgvData.Rows(nRow).HeaderCell.Value = sName2
                                For Each oArm As clsArm In colArms
                                    Dim oSAItemsArm As clsScatteredAccessItems = DirectCast(oArm.Tag, clsScatteredAccessItems)
                                    If oSAItemsArm.Items.Count > 0 Then
                                        Try
                                            If oSAItemsArm.Item(sName2) IsNot Nothing Then
                                                oSAItemsArm.Item(sName2).Tag = nRow
                                            End If
                                        Catch ex As Exception
                                        End Try
                                    End If
                                Next
                            Next
                            If oRows.Count > 0 Then
                                dgvData.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells)
                                dgvData.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders)
                                dgvData.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
                            End If
                        End If
                    End If

                    If bRead Then
                        If oSAItems.SetupOK Then
                            If oSAItems.ObjectOK Then
                                oSAItems.Refresh()
                                Application.DoEvents()
                                If mbSAActive(nCntr) = False Then
                                    Status = gpsRM.GetString("psSETUPSA_CONNECTED") & oCntr.Name
                                    dgvCntrStatus.Rows(nDataRow).Cells(mnDGVCS_COL_SA_STAT).Value = gcsRM.GetString("csCONNECTED")
                                    mbSAActive(nCntr) = True
                                End If
                            Else
                                Status = gpsRM.GetString("psSETUPSA_CONN") & oCntr.Name
                                dgvCntrStatus.Rows(nDataRow).Cells(mnDGVCS_COL_SA_STAT).Value = gcsRM.GetString("csCONNECTING")
                                Application.DoEvents()
                                oSAItems.SetupSA(oCntr.Robot)
                                Application.DoEvents()
                                If oSAItems.ObjectOK Then
                                    Application.DoEvents()
                                    oSAItems.Refresh()
                                    Application.DoEvents()
                                    Status = gpsRM.GetString("psSETUPSA_COMP") & oCntr.Name
                                    Status = gpsRM.GetString("psSETUPSA_CONNECTED") & oCntr.Name
                                    dgvCntrStatus.Rows(nDataRow).Cells(mnDGVCS_COL_SA_STAT).Value = gcsRM.GetString("csCONNECTED")
                                    mbSAActive(nCntr) = True
                                Else
                                    Status = gpsRM.GetString("psSETUPSA_FAILED") & oCntr.Name
                                    dgvCntrStatus.Rows(nDataRow).Cells(mnDGVCS_COL_SA_STAT).Value = gcsRM.GetString("csERROR")
                                    mbSAActive(nCntr) = False
                                End If
                            End If
                        End If
                    Else
                        If mbSAActive(nCntr) Then
                            mbSAActive(nCntr) = False
                            dgvCntrStatus.Rows(nDataRow).Cells(mnDGVCS_COL_SA_STAT).Value = gcsRM.GetString("csAVAILABLE")
                            Status = gpsRM.GetString("psSETUPSA_IDLE") & oCntr.Name
                        End If
                    End If
                Else
                    If oSAItems IsNot Nothing Then
                        dgvCntrStatus.Rows(nDataRow).Cells(mnDGVCS_COL_SA_STAT).Value = gcsRM.GetString("csUNAVAILABLE")
                        Status = gpsRM.GetString("psSETUPSA_CONLOST") & oCntr.Name
                        oSAItems.DisconnectSA()
                    End If
                End If
                Application.DoEvents()
            Next

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: tmrConnect_Tick", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
        'Update SA active tags after connect
        subSetControllerReadRequests()
        mbTmrConnectBusy = False
        tmrDisplay.Enabled = True
        tmrConnect.Enabled = True
        tmrSARead.Enabled = True
    End Sub

    Private Sub tmrStartup_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrStartup.Tick
        '********************************************************************************************
        'Description: Let main continue after we stopped connecting to robots
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '    11/21/13   MSW     Disable startup timer
        '********************************************************************************************
        Dim sMessage(1) As String
        'Let main continue while setting up the last one.
        sMessage(0) = "scattaccessmanload"
        sMessage(1) = True.ToString
        oIPC.WriteControlMsg(gs_COM_ID_PW_MAIN, sMessage)
        mbStartupMessage2Sent = True
        tmrStartup.Enabled = False
    End Sub
End Class
Friend Class SARequests
    Public sProcName As String
    Public nProcID As Integer
    Public sRequests As String
End Class