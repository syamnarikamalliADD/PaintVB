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
' Description: RobotVariables
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Matt White
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
' Date          By      Reason                                                        Version
' 03/03/2012    MSW     1st draft                                                     4.01.01.00
' 03/12/2012    RJO     modifed for .NET Password and IPC                             4.01.02.00
' 04/11/12      MSW     Changed CommonStrings setup so it builds correctly            4.01.03.00
' 04/20/12      MSW     Disable save button, since it doesn't do anything             4.01.03.01
'                       frmMain_KeyDown-Add excape key handler
' 10/24/12      RJO     Added StartupModule to project to prevent multiple instances  4.01.03.02
' 12/13/12      MSW     btnMonitor_Click, subGetSignalDetails -                           4.01.04.00
' 04/16/13      MSW     Add Canadian language files                                     4.01.05.00
'                       Standalone Changelog
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up  4.01.05.01
'    09/30/13   MSW     Save screenshots as jpegs                                       4.01.06.00
'    01/07/14   MSW     remove controlbox from main form                                4.01.06.01
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call                4.01.07.00
'    04/21/14   MSW     subShowNewPage - Add refresh                                    4.01.07.01
'**************************************************************************************************
Imports Response = System.Windows.Forms.DialogResult

Imports ConnStat = FRRobotNeighborhood.FRERNConnectionStatusConstants

Friend Class frmMain

#Region " Declares "
    '******** Form Constants   *********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Public Const msSCREEN_NAME As String = "Robot_IO"   ' <-- For password area change log etc.
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    Friend Const mbUSEROBOTS As Boolean = True
    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    Friend gsChangeLogArea As String = msSCREEN_NAME
    Private msCulture As String = "en-US" 'Default to English
    Private msOldZone As String = String.Empty
    Private msOldRobot As String = String.Empty
    Private mcolZones As clsZones = Nothing
    Private mController As clsController = Nothing
    Private mbLocal As Boolean
    Private mbScreenLoaded As Boolean
    Private mbEventBlocker As Boolean
    Private mbRemoteZone As Boolean
    Delegate Sub ControllerStatusChange(ByVal Controller As clsController)
    Private msSCREEN_DUMP_NAME As String = "View_Robot_IO.jpg" 'changes with selected tab
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private mbEditsMade As Boolean
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean
    Private mnProgress As Integer
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    '******** End Property Variables    *************************************************************

    '******** Structures    *************************************************************************
    '******** End Structures    *********************************************************************

    Private mbAutoRefresh As Boolean = True
    Private WithEvents colControllers As clsControllers = Nothing
    Private moAllIO As FRRobot.FRCIOTypes
    Private moAIOType As FRRobot.FRCAnalogIOType
    Private moDIOType As FRRobot.FRCDigitalIOType
    Private moFlagType As FRRobot.FRCFlagType
    Private moGIOType As FRRobot.FRCGroupIOType
    Private moMarkerType As FRRobot.FRCMarkerType
    Private moPIOType As FRRobot.FRCPLCIOType
    Private moRIOType As FRRobot.FRCRobotIOType
    Private moSIOType As FRRobot.FRCSOPIOType
    Private moTPIOType As FRRobot.FRCTPIOType
    Private moUIOType As FRRobot.FRCUOPIOType
    Private moWIOType As FRRobot.FRCWeldDigitalIOType
    Private moIOType As FRRobot.FREIOTypeConstants
    Private moIOSignals As FRRobot.FRCIOSignals
    Private moIOConfigs As FRRobot.FRCIOConfigs
    Private msIOCOMMENT As String = String.Empty
    Private mbOutputActive As Boolean = False
    Private mnDetailItem As Integer = -1
    Private moPointDetailSignal As Object
    Private mbSimButtonSim As Boolean = True
    Private mnDetailPoint As Integer = -1
    Private msLabel As String = String.Empty
    Private msValue As String = String.Empty
    Private msSim As String = String.Empty
    Private moColMonitor As New Collection
    Private mnMonitorCol As Integer = -1
    Private mnMonitorRow As Integer = -1
    '******** This is the old pw3 password object interop  ******************************************
    'Friend WithEvents moPassword As PWPassword.PasswordObject 'RJO 03/23/12
    'Friend moPrivilege As PWPassword.cPrivilegeObject 'RJO 03/23/12
    '******** This is the old pw3 password object interop  *****************************************

    Friend WithEvents moPassword As clsPWUser 'RJO 03/23/12
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm 'RJO 03/23/12

    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet)  'RJO 03/23/12
    'Delegate Sub ControllerStatusChange(ByVal Controller As clsController)
    '******** This is the old pw3 password object interop  ******************************************


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


            'If mPassword.CheckPassword(msSCREEN_NAME, Value, moPassword, moPrivilege) Then 'RJO 03/23/12
            If moPassword.CheckPassword(Value) Then
                'passed
                mPrivilegeGranted = mPrivilegeRequested
                'if privilege changed may have to enable controls
                subEnableControls(True)
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
                Select Case Privilege
                    Case ePrivilege.None
                        'false aborts closing form , changing zone etc
                        Status = gcsRM.GetString("csSAVE_CANCEL", oCulture)
                        Privilege = ePrivilege.Edit 'popup a login
                        Return False
                    Case Else
                        Call subSaveData()
                        Return True
                End Select
            Case Response.Cancel
                'false aborts closing form , changing zone etc
                Status = gcsRM.GetString("csSAVE_CANCEL", oCulture)
                Return False
            Case Else
                Call subLoadData()
                Return True
        End Select

    End Function
    Private Sub subFormatScreenLayout()
        '********************************************************************************************
        'Description: Update the robot list for the screen
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subEnableControls(True)
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
        'just in case
        If cboRobot.Text = String.Empty Then
            Exit Sub
        Else
            If cboRobot.Text = msOldRobot Then Exit Sub
        End If
        If msOldRobot <> String.Empty Then _
                                colControllers.Item(msOldRobot).Selected = False

        msOldRobot = cboRobot.Text

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            Progress = 30

            Status = gpsRM.GetString("psSELECTING") & msOldRobot
            mController = colControllers.Item(cboRobot.Text)
            colControllers.Item(cboRobot.Text).Selected = True

            'set the IO Type CBO
            mbEventBlocker = True


            pnlPointDetail.Visible = False
            dgIOPoints.Rows.Clear()

            pnlSim.Visible = False
            pnlSetVal.Visible = False
            mbOutputActive = False
            mnDetailItem = -1
            moAllIO = mController.Robot.IOTypes
            cboIOType.Items.Clear()
            Dim oTag(moAllIO.Count - 1) As FRRobot.FREIOTypeConstants
            For nItem As Integer = 0 To moAllIO.Count - 1
                Dim oObj As FRRobot.FRCIOType = moAllIO(, nItem)
                Dim oType As FRRobot.FREIOTypeConstants = DirectCast(CInt(oObj.Type), FRRobot.FREIOTypeConstants)
                oTag(nItem) = oType
                Dim sLabel As String = String.Empty
                Select Case oType
                    Case FRRobot.FREIOTypeConstants.frAInType
                        sLabel = gpsRM.GetString("psAI")
                    Case FRRobot.FREIOTypeConstants.frAOutType
                        sLabel = gpsRM.GetString("psAO")
                    Case FRRobot.FREIOTypeConstants.frDInType
                        sLabel = gpsRM.GetString("psDI")
                    Case FRRobot.FREIOTypeConstants.frDOutType
                        sLabel = gpsRM.GetString("psDO")
                    Case FRRobot.FREIOTypeConstants.frFlagType
                        sLabel = gpsRM.GetString("psFL")
                    Case FRRobot.FREIOTypeConstants.frGPInType
                        sLabel = gpsRM.GetString("psGI")
                    Case FRRobot.FREIOTypeConstants.frGPOutType
                        sLabel = gpsRM.GetString("psGO")
                        'Case FRRobot.FREIOTypeConstants.frLAInType
                        'Case FRRobot.FREIOTypeConstants.frLAOutType
                        'Case FRRobot.FREIOTypeConstants.frLDInType
                        'Case FRRobot.FREIOTypeConstants.frLDOutType
                    Case FRRobot.FREIOTypeConstants.frMarkerType
                        sLabel = gpsRM.GetString("psMARKER")
                    Case FRRobot.FREIOTypeConstants.frPLCInType
                        sLabel = gpsRM.GetString("psPLCI")
                    Case FRRobot.FREIOTypeConstants.frPLCOutType
                        sLabel = gpsRM.GetString("psPLCO")
                    Case FRRobot.FREIOTypeConstants.frRDInType
                        sLabel = gpsRM.GetString("psRI")
                    Case FRRobot.FREIOTypeConstants.frRDOutType
                        sLabel = gpsRM.GetString("psRO")
                    Case FRRobot.FREIOTypeConstants.frSOPInType
                        sLabel = gpsRM.GetString("psSI")
                    Case FRRobot.FREIOTypeConstants.frSOPOutType
                        sLabel = gpsRM.GetString("psSO")
                    Case FRRobot.FREIOTypeConstants.frTPInType
                        sLabel = gpsRM.GetString("psTPI")
                    Case FRRobot.FREIOTypeConstants.frTPOutType
                        sLabel = gpsRM.GetString("psTPO")
                    Case FRRobot.FREIOTypeConstants.frUOPInType
                        sLabel = gpsRM.GetString("psUI")
                    Case FRRobot.FREIOTypeConstants.frUOPOutType
                        sLabel = gpsRM.GetString("psUO")
                    Case FRRobot.FREIOTypeConstants.frWDInType
                        sLabel = gpsRM.GetString("psWO")
                    Case FRRobot.FREIOTypeConstants.frWDOutType
                        sLabel = gpsRM.GetString("psWO")
                        'Case FRRobot.FREIOTypeConstants.frWSTKInType
                        'Case FRRobot.FREIOTypeConstants.frWSTKOutType
                    Case Else
                        sLabel = gpsRM.GetString("psNA")
                End Select
                cboIOType.Items.Add(sLabel)
            Next
            cboIOType.Tag = oTag
            subShowNewPage(True)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Progress = 70
            'setup limits:
            'statusbar text

            DataLoaded = True
            subEnableControls(True)
            mbEventBlocker = False

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
            If cboZone.Text = msOldZone Then Exit Sub
        End If
        msOldZone = cboZone.Text
        mcolZones.CurrentZone = cboZone.Text

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subEnableControls(False)
            EditsMade = False
            DataLoaded = False

            colControllers = New clsControllers(mcolZones, False)
            SetUpStatusStrip(Me, colControllers)
            mPWRobotCommon.LoadRobotBoxFromCollection(cboRobot, colControllers, False)
            'set the robot checkbox
            mbEventBlocker = True
            subShowNewPage(True)
            mbEventBlocker = False

            'make sure the robot cartoons get the right color
            colControllers.RefreshConnectionStatus()
            msOldRobot = String.Empty


            If cboRobot.Items.Count = 1 Then
                'this selects the robot in cases of just one zone. fires event to call subchangerobot
                cboRobot.Text = cboRobot.Items(0).ToString
            Else
                'statusbar text
                Status(True) = gcsRM.GetString("csSELECT_ROBOT")
            End If

            Me.stsStatus.Refresh()

            subFormatScreenLayout()

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
        'Description: Disable or enable controls. Check privileges and edits etc. 
        '
        'Parameters: bEnable - false can be used to disable during load etc 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/02/10  RJO     If user logged in and had Copy privilege, controls would not be enabled.
        '                   Added ePrivilege.Copy to Case statement
        '********************************************************************************************
        Dim bRestOfControls As Boolean = False
        Dim bEditControls As Boolean = False
        btnClose.Enabled = True

        If bEnable = False Then
            btnChangeLog.Enabled = False
            bRestOfControls = False
            btnSave.Enabled = False
            bEditControls = False
        Else
            Select Case Privilege
                Case ePrivilege.None
                    btnChangeLog.Enabled = DataLoaded
                    bRestOfControls = DataLoaded
                    btnSave.Enabled = False
                    bEditControls = False
                Case ePrivilege.Edit, ePrivilege.Copy
                    btnChangeLog.Enabled = DataLoaded
                    bRestOfControls = DataLoaded
                    btnSave.Enabled = EditsMade
                    bEditControls = True
            End Select
        End If

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
                ' set labels 
                lblIOType.Text = .GetString("psIOTYPE")
                btnAutoRefresh.Text = .GetString("psAUTOREFRESH")
                mnuRefresh.Text = tmrRefresh.Interval & .GetString("psMS")
                btnAutoRefresh.Image = DirectCast(.GetObject("LIGHTON"), Image)
                btnSetVal.Text = .GetString("psSET")
                btnSetComment.Text = .GetString("psSETCOMMENT")
                lblSim.Text = gpsRM.GetString("psSIM")
                btnSim.Text = gpsRM.GetString("psSIMULATE")
                btnUnsimAll.Text = gpsRM.GetString("psUNSIMALL")
                btnMonitor.Text = gpsRM.GetString("psMONITOR")
                mnuEndMonitor.Text = gpsRM.GetString("psEND_MON")
                mnuClearAllMon.Text = gpsRM.GetString("psEND_ALL_MON")
            End With 'gpsRM

            lblZone.Text = gcsRM.GetString("csZONE_CAP")
            lblRobot.Text = gcsRM.GetString("csROBOT_CAP")
            pnlPointDetail.Visible = False
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
        'Dim cachePrivilege As mPassword.ePrivilege 'RJO 03/23/12

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
            'lblZone.Visible = Not mbLocal
            'cboZone.Visible = Not mbLocal

            'init the old password for now
            'moPassword = New PWPassword.PasswordObject 'RJO 03/23/12
            'mPassword.InitPassword(moPassword, moPrivilege, LoggedOnUser, msSCREEN_NAME)
            'If LoggedOnUser <> String.Empty Then

            '    If moPrivilege.ActionAllowed Then
            '        cachePrivilege = mPassword.PrivilegeStringToEnum(moPrivilege.Privilege)
            '    Else
            '        cachePrivilege = ePrivilege.None
            '    End If

            '    Privilege = cachePrivilege

            'Else
            '    Privilege = ePrivilege.None
            'End If

            Progress = 5

            Me.Text = gpsRM.GetString("psSCREENCAPTION", DisplayCulture)

            mScreenSetup.LoadZoneBox(cboZone, mcolZones, False)

            Progress = 30

            'init new IPC and new Password 'RJO 03/23/12
            oIPC = New Paintworks_IPC.clsInterProcessComm
            moPassword = New clsPWUser
            Progress = 50

            mScreenSetup.InitializeForm(Me)
            Call subInitFormText()
            Me.Show()

            'Handle user selection change events with common event handler routines

            Progress = 70

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
            'If mbLocal Then
            '    cboZone.Text = cboZone.Items(0).ToString
            '    Call subChangeZone()
            'End If
            If cboZone.Items.Count = 1 Then
                'this selects the zone in cases of just one zone. fires event to call subchangezone
                cboZone.Text = cboZone.Items(0).ToString
                subChangeZone()
            Else
                'statusbar text
                Status(True) = gcsRM.GetString("csSELECT_ZONE")
                subEnableControls(True)
            End If
            Me.stsStatus.Refresh()

            ' refresh lock pic
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
    Private Function bSaveToRobots() As Boolean
        '********************************************************************************************
        'Description:  Update all the robot controllers
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

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

            Progress = 98

            If DataLoaded Then
                tmrRefresh.Enabled = mbAutoRefresh
                Status = gcsRM.GetString("csLOADDONE", DisplayCulture)

                subShowNewPage(True)

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

    Private Sub tmrRefresh_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrRefresh.Tick
        '********************************************************************************************
        'Description:  Update GUI and robot times
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            tmrRefresh.Enabled = False

            subRefresh(True)

            tmrRefresh.Enabled = mbAutoRefresh


        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: Tick", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

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

        'mPWCommon.SaveToChangeLog(mcolZones.ActiveZone.DatabasePath) 'Access Database
        mPWCommon.SaveToChangeLog(mcolZones.ActiveZone) 'SQL database

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
            EditsMade = False

        Catch ex As Exception

            mDebug.ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED", DisplayCulture), ex, msSCREEN_NAME, _
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
        ' 04/16/13  MSW     Standalone ChangeLog                                          4.01.05.00
        '********************************************************************************************

        Try

           subLogChanges()
            mPWCommon.subShowChangeLog(mcolZones.CurrentZone, gsChangeLogArea, msSCREEN_NAME, gcsRM.GetString("csALL"), _
                                        gcsRM.GetString("csALL"), mDeclares.eParamType.None, nIndex, True, False, oIPC)
        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        End Try

    End Sub

    Private Function PrintVal(ByVal bTmp As Boolean) As String
        '********************************************************************************************
        'Description:  Get text for a boolean value
        '
        'Parameters: boolean 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If bTmp Then
            Return gpsRM.GetString("psON")
        Else
            Return gpsRM.GetString("psOFF")
        End If
    End Function

    Private Function PrintVal(ByVal nTmp As Integer) As String
        '********************************************************************************************
        'Description:  Get text for an integer value
        '
        'Parameters: boolean 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return nTmp.ToString
    End Function
    Private Sub subGetSignalDetails(ByRef oPoint As Object, ByRef sValue As String, ByRef sSim As String, ByRef nSim As Integer, Optional ByVal bCmt As Boolean = False, Optional ByRef sComment As String = "", Optional ByRef nPoint As Integer = -1)
        '********************************************************************************************
        'Description:  Get details for an I/O point
        '
        'Parameters: boolean 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/30/12  MSW     btnMonitor_Click, subGetSignalDetails - 
        '                   Add error checking for robot IO unassigned or offline
        '********************************************************************************************
        If TypeOf (oPoint) Is FRRobot.FRCAnalogIOSignal Then
            Dim oIOPoint As FRRobot.FRCAnalogIOSignal = DirectCast(oPoint, FRRobot.FRCAnalogIOSignal)
            If bCmt Then
                sComment = oIOPoint.Comment
                nPoint = oIOPoint.LogicalNum
            End If
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
            sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
            If oIOPoint.Simulate Then
                nSim = 1
            Else
                nSim = 0
            End If
            sSim = PrintVal(oIOPoint.Simulate)
        ElseIf TypeOf (oPoint) Is FRRobot.FRCDigitalIOSignal Then
            Dim oIOPoint As FRRobot.FRCDigitalIOSignal = DirectCast(oPoint, FRRobot.FRCDigitalIOSignal)
            If bCmt Then
                sComment = oIOPoint.Comment
                nPoint = oIOPoint.LogicalNum
            End If
            Debug.Print(oIOPoint.IsOffline.ToString)
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
            sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
            If oIOPoint.Simulate Then
                nSim = 1
            Else
                nSim = 0
            End If
            sSim = PrintVal(oIOPoint.Simulate)
        ElseIf TypeOf (oPoint) Is FRRobot.FRCFlagSignal Then
            Dim oIOPoint As FRRobot.FRCFlagSignal = DirectCast(oPoint, FRRobot.FRCFlagSignal)
            If bCmt Then
                sComment = oIOPoint.Comment
                nPoint = oIOPoint.LogicalNum
            End If
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
            sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
            If oIOPoint.Simulate Then
                nSim = 1
            Else
                nSim = 0
            End If
            sSim = PrintVal(oIOPoint.Simulate)
        ElseIf TypeOf (oPoint) Is FRRobot.FRCGroupIOSignal Then
            Dim oIOPoint As FRRobot.FRCGroupIOSignal = DirectCast(oPoint, FRRobot.FRCGroupIOSignal)
            If bCmt Then
                sComment = oIOPoint.Comment
                nPoint = oIOPoint.LogicalNum
            End If
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
            sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
            If oIOPoint.Simulate Then
                nSim = 1
            Else
                nSim = 0
            End If
            sSim = PrintVal(oIOPoint.Simulate)
        ElseIf TypeOf (oPoint) Is FRRobot.FRCMarkerSignal Then
            Dim oIOPoint As FRRobot.FRCMarkerSignal = DirectCast(oPoint, FRRobot.FRCMarkerSignal)
            If bCmt Then
                sComment = oIOPoint.Comment
                nPoint = oIOPoint.LogicalNum
            End If
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
            sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
            If oIOPoint.Simulate Then
                nSim = 1
            Else
                nSim = 0
            End If
            sSim = PrintVal(oIOPoint.Simulate)
        ElseIf TypeOf (oPoint) Is FRRobot.FRCPLCIOSignal Then
            Dim oIOPoint As FRRobot.FRCPLCIOSignal = DirectCast(oPoint, FRRobot.FRCPLCIOSignal)
            If bCmt Then
                sComment = oIOPoint.Comment
                nPoint = oIOPoint.LogicalNum
            End If
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
            sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
            nSim = -1
            sSim = gpsRM.GetString("psNA")
        ElseIf TypeOf (oPoint) Is FRRobot.FRCRobotIOSignal Then
            Dim oIOPoint As FRRobot.FRCRobotIOSignal = DirectCast(oPoint, FRRobot.FRCRobotIOSignal)
            If bCmt Then
                sComment = oIOPoint.Comment
                nPoint = oIOPoint.LogicalNum
            End If
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
            sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
            If oIOPoint.Simulate Then
                nSim = 1
            Else
                nSim = 0
            End If
            sSim = PrintVal(oIOPoint.Simulate)
        ElseIf TypeOf (oPoint) Is FRRobot.FRCSOPIOSignal Then
            Dim oIOPoint As FRRobot.FRCSOPIOSignal = DirectCast(oPoint, FRRobot.FRCSOPIOSignal)
            If bCmt Then
                sComment = oIOPoint.Comment
                nPoint = oIOPoint.LogicalNum
            End If
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
            sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
            nSim = -1
            sSim = gpsRM.GetString("psNA")
        ElseIf TypeOf (oPoint) Is FRRobot.FRCTPIOSignal Then
            Dim oIOPoint As FRRobot.FRCTPIOSignal = DirectCast(oPoint, FRRobot.FRCTPIOSignal)
            If bCmt Then
                sComment = oIOPoint.Comment
                nPoint = oIOPoint.LogicalNum
            End If
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
            sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
            nSim = -1
            sSim = gpsRM.GetString("psNA")
        ElseIf TypeOf (oPoint) Is FRRobot.FRCUOPIOSignal Then
            Dim oIOPoint As FRRobot.FRCUOPIOSignal = DirectCast(oPoint, FRRobot.FRCUOPIOSignal)
            If bCmt Then
                sComment = oIOPoint.Comment
                nPoint = oIOPoint.LogicalNum
            End If
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
            sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
            nSim = -1
            sSim = gpsRM.GetString("psNA")
        ElseIf TypeOf (oPoint) Is FRRobot.FRCWeldDigitalIOSignal Then
            Dim oIOPoint As FRRobot.FRCWeldDigitalIOSignal = DirectCast(oPoint, FRRobot.FRCWeldDigitalIOSignal)
            If bCmt Then
                sComment = oIOPoint.Comment
                nPoint = oIOPoint.LogicalNum
            End If
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
            sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
            If oIOPoint.Simulate Then
                nSim = 1
            Else
                nSim = 0
            End If
            sSim = PrintVal(oIOPoint.Simulate)
        End If
        'Case FRRobot.FREIOTypeConstants.frLAInType
        'Case FRRobot.FREIOTypeConstants.frLAOutType
        'Case FRRobot.FREIOTypeConstants.frLDInType
        'Case FRRobot.FREIOTypeConstants.frLDOutType

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
        ' 04/21/14  MSW     subShowNewPage - Add refresh
        '********************************************************************************************
        mbEventBlocker = BlockEvents

        tmrRefresh.Enabled = False

        Application.DoEvents()
        If moIOSignals IsNot Nothing Then
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            dgIOPoints.Columns.Clear()
            dgIOPoints.Rows.Clear()

            dgIOPoints.Columns.Add("LABEL", gpsRM.GetString("psCOL_LABEL"))
            dgIOPoints.Columns.Add("SIM", gpsRM.GetString("psCOL_SIM"))
            dgIOPoints.Columns.Add("VALUE", gpsRM.GetString("psCOL_VALUE"))

            moIOSignals.Refresh()
            Dim nCount As Integer = moIOSignals.Count
            For nItem As Integer = 0 To nCount - 1
                Dim sComment As String = String.Empty
                Dim nPoint As Integer = 0
                Dim sValue As String = ""
                Dim bSim As Boolean = False
                Dim sSim As String = ""
                Dim nSim As Integer = -1
                subGetSignalDetails(moIOSignals(, nItem), sValue, sSim, nSim, True, sComment, nPoint)
                sComment = String.Format(msIOCOMMENT, nPoint, sComment)
                dgIOPoints.Rows.Add()
                dgIOPoints.Rows.Item(nItem).Cells(0).Value = sComment
                dgIOPoints.Rows.Item(nItem).Cells(1).Value = sSim
                dgIOPoints.Rows.Item(nItem).Cells(2).Value = sValue
            Next
        Else
            dgIOPoints.RowCount = 0
        End If
        subRefresh(True) 'MSW 4/21/14 add refresh
        Me.Cursor = System.Windows.Forms.Cursors.Default
        subEnableControls(True)
        mbEventBlocker = False

        tmrRefresh.Enabled = mbAutoRefresh

    End Sub
    Private Sub subRefresh(ByVal BlockEvents As Boolean)
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

        Application.DoEvents()
        If moIOSignals IsNot Nothing Then
            Dim nRow As Integer = -1
            If dgIOPoints.SelectedRows.Count > 0 Then

                nRow = dgIOPoints.SelectedRows(0).Index
            End If
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            moIOSignals.Refresh()
            Dim nCount As Integer = moIOSignals.Count
            For nItem As Integer = 0 To nCount - 1
                Dim sValue As String = ""
                Dim sSim As String = ""
                Dim nSim As Integer = -1

                If nRow = nItem Then
                    moPointDetailSignal = moIOSignals(, nItem)
                    Dim sComment As String = String.Empty
                    Dim nPoint As Integer = 0
                    subGetSignalDetails(moIOSignals(, nItem), sValue, sSim, nSim, True, sComment, nPoint)
                    msLabel = String.Format(msIOCOMMENT, nPoint, sComment)
                    msValue = sValue
                    msSim = sSim
                    lblIO.Text = msLabel & " = " & sValue
                    mnDetailPoint = nPoint

                    If mnDetailItem <> nItem Then
                        'Changed
                        pnlSetVal.Visible = mbOutputActive Or (nSim = 1)
                        mnDetailItem = nItem
                        txtSetVal.Text = sValue
                    End If
                    If pnlSim.Visible Then
                        lblSim.Text = gpsRM.GetString("psSIM") & sSim
                        If nSim = 0 Then
                            btnSim.Text = gpsRM.GetString("psSIMULATE")
                            mbSimButtonSim = True
                        Else
                            btnSim.Text = gpsRM.GetString("psUNSIMULATE")
                            mbSimButtonSim = False
                        End If
                    End If
                    pnlPointDetail.Visible = True
                Else
                    subGetSignalDetails(moIOSignals(, nItem), sValue, sSim, nSim)
                End If
                dgIOPoints.Rows.Item(nItem).Cells(1).Value = sSim
                dgIOPoints.Rows.Item(nItem).Cells(2).Value = sValue

            Next
        Else
            dgIOPoints.Rows.Clear()
        End If
        Me.Cursor = System.Windows.Forms.Cursors.Default
        subEnableControls(True)
        mbEventBlocker = False

    End Sub


    Private Overloads Sub subUpdateChangeLog(Byref NewValue As String, Byref OldValue As String, _
                                        Byref oZone As clsZone, Byref DeviceName As String, _
                                        Byref sDesc As String)
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
        Dim sTmp As String = sDesc & " " & gcsRM.GetString("csCHANGED_FROM", DisplayCulture) & " " & OldValue & " " & _
                        gcsRM.GetString("csTO", DisplayCulture) & " " & NewValue

        Call mPWCommon.AddChangeRecordToCollection(gsChangeLogArea, LoggedOnUser, _
                                        oZone.ZoneNumber, DeviceName, String.Empty, sTmp, oZone.Name)

    End Sub
    Private Overloads Sub subUpdateChangeLog(Byref oZone As clsZone, Byref DeviceName As String, _
                                    Byref sDesc As String)
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

        Call mPWCommon.AddChangeRecordToCollection(gsChangeLogArea, LoggedOnUser, _
                                        oZone.ZoneNumber, DeviceName, String.Empty, sDesc, oZone.Name)

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
                    subLaunchHelp(gs_HELP_VIEW_ROBOT_IO, oIPC)

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
        Dim nHeight As Integer = tscMain.ContentPanel.Height
        Dim nWidth As Integer = tscMain.ContentPanel.Width

        If nHeight < 100 Then nHeight = 100
        If nWidth < 100 Then nWidth = 100

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

#End Region

#Region " Control Events "


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
        ' 11/12/09  MSW     add delegate declarations tohandle cross-thread calls
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
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/23/12
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
        ' 11/12/09  MSW     add delegate declarations tohandle cross-thread calls
        '********************************************************************************************
        'This can get called by the password thread
        If Me.stsStatus.InvokeRequired Then
            Dim dLogin As New LogOutCallBack(AddressOf moPassword_LogOut)
            Me.BeginInvoke(dLogin)
        Else
            Status = Me.LoggedOnUser & " " & gcsRM.GetString("csLOGGED_OUT")
            Me.LoggedOnUser = String.Empty
            Me.Privilege = ePrivilege.None
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/23/12
        End If

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
            Dim dNewMessage As New NewMessage_CallBack(AddressOf oIPC_NewMessage)
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

            'If moPrivilege.Privilege = String.Empty Then
            '    cachePrivilege = ePrivilege.None
            'Else
            '    If moPrivilege.ActionAllowed Then
            '        cachePrivilege = mPassword.PrivilegeStringToEnum(moPrivilege.Privilege)
            '    Else
            '        cachePrivilege = ePrivilege.None
            '    End If
            'End If

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

            Case "btnChangeLog"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                subShowChangeLog(CType(eChangeMnu.Hours24, Integer))
            Case "btnAutoRefresh"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                If mbAutoRefresh Then
                    mbAutoRefresh = False
                    btnAutoRefresh.Image = DirectCast(gpsRM.GetObject("LIGHTOFF"), Image)
                Else
                    mbAutoRefresh = True
                    btnAutoRefresh.Image = DirectCast(gpsRM.GetObject("LIGHTON"), Image)
                End If
                tmrRefresh.Enabled = mbAutoRefresh
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

    Private Sub cboIOType_DropDown(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboIOType.DropDown, cboRobot.DropDown, cboZone.DropDown
        '********************************************************************************************
        'Description:  Smooth operation by disabling timer
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        tmrRefresh.Enabled = False
    End Sub

    Private Sub cboIOType_DropDownClosed(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboIOType.DropDownClosed, cboRobot.DropDownClosed, cboZone.DropDownClosed
        '********************************************************************************************
        'Description:  Smooth operation by disabling timer
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        tmrRefresh.Enabled = mbAutoRefresh
    End Sub

    Private Sub cboIOType_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboIOType.SelectionChangeCommitted
        '********************************************************************************************
        'Description:  IO Type Selected
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nItem As Integer = cboIOType.SelectedIndex
        Dim oTag() As FRRobot.FREIOTypeConstants = DirectCast(cboIOType.Tag, FRRobot.FREIOTypeConstants())
        pnlPointDetail.Visible = False
        moIOType = oTag(nItem)
        Dim oIOTypeObj As Object = moAllIO(moIOType)
        mnDetailItem = -1
        Select Case moIOType
            Case FRRobot.FREIOTypeConstants.frAInType
                moAIOType = DirectCast(oIOTypeObj, FRRobot.FRCAnalogIOType)
                msIOCOMMENT = gpsRM.GetString("psAI_CMT")
                moIOSignals = moAIOType.Signals
                moIOConfigs = moAIOType.Configs
                pnlSim.Visible = True
                pnlSetVal.Visible = False
                mbOutputActive = False
            Case FRRobot.FREIOTypeConstants.frAOutType
                moAIOType = DirectCast(oIOTypeObj, FRRobot.FRCAnalogIOType)
                msIOCOMMENT = gpsRM.GetString("psAO_CMT")
                moIOSignals = moAIOType.Signals
                moIOConfigs = moAIOType.Configs
                pnlSim.Visible = True
                pnlSetVal.Visible = True
                mbOutputActive = True
            Case FRRobot.FREIOTypeConstants.frDInType
                moDIOType = DirectCast(oIOTypeObj, FRRobot.FRCDigitalIOType)
                msIOCOMMENT = gpsRM.GetString("psDI_CMT")
                moIOSignals = moDIOType.Signals
                moIOConfigs = moDIOType.Configs
                pnlSim.Visible = True
                pnlSetVal.Visible = False
                mbOutputActive = False
            Case FRRobot.FREIOTypeConstants.frDOutType
                moDIOType = DirectCast(oIOTypeObj, FRRobot.FRCDigitalIOType)
                msIOCOMMENT = gpsRM.GetString("psDO_CMT")
                moIOSignals = moDIOType.Signals
                moIOConfigs = moDIOType.Configs
                pnlSim.Visible = True
                pnlSetVal.Visible = True
                mbOutputActive = True
            Case FRRobot.FREIOTypeConstants.frFlagType
                moFlagType = DirectCast(oIOTypeObj, FRRobot.FRCFlagType)
                msIOCOMMENT = gpsRM.GetString("psFL_CMT")
                moIOSignals = moFlagType.Signals
                moIOConfigs = Nothing
                pnlSim.Visible = False
                pnlSetVal.Visible = False
                mbOutputActive = False
            Case FRRobot.FREIOTypeConstants.frGPInType
                moGIOType = DirectCast(oIOTypeObj, FRRobot.FRCGroupIOType)
                msIOCOMMENT = gpsRM.GetString("psGI_CMT")
                moIOSignals = moGIOType.Signals
                moIOConfigs = moGIOType.Configs
                pnlSim.Visible = True
                pnlSetVal.Visible = False
                mbOutputActive = False
            Case FRRobot.FREIOTypeConstants.frGPOutType
                moGIOType = DirectCast(oIOTypeObj, FRRobot.FRCGroupIOType)
                msIOCOMMENT = gpsRM.GetString("psGO_CMT")
                moIOSignals = moGIOType.Signals
                moIOConfigs = moGIOType.Configs
                pnlSim.Visible = True
                pnlSetVal.Visible = True
                mbOutputActive = True
                'Case FRRobot.FREIOTypeConstants.frLAInType
                'Case FRRobot.FREIOTypeConstants.frLAOutType
                'Case FRRobot.FREIOTypeConstants.frLDInType
                'Case FRRobot.FREIOTypeConstants.frLDOutType
            Case FRRobot.FREIOTypeConstants.frMarkerType
                moMarkerType = DirectCast(oIOTypeObj, FRRobot.FRCMarkerType)
                msIOCOMMENT = gpsRM.GetString("psMARKER_CMT")
                moIOSignals = moMarkerType.Signals
                moIOConfigs = Nothing
                pnlSim.Visible = False
                pnlSetVal.Visible = False
                mbOutputActive = False
            Case FRRobot.FREIOTypeConstants.frPLCInType
                moPIOType = DirectCast(oIOTypeObj, FRRobot.FRCPLCIOType)
                msIOCOMMENT = gpsRM.GetString("psPI_CMT")
                moIOSignals = moPIOType.Signals
                moIOConfigs = moPIOType.Configs
                pnlSim.Visible = False
                pnlSetVal.Visible = False
                mbOutputActive = False
            Case FRRobot.FREIOTypeConstants.frPLCOutType
                moPIOType = DirectCast(oIOTypeObj, FRRobot.FRCPLCIOType)
                msIOCOMMENT = gpsRM.GetString("psPO_CMT")
                moIOSignals = moPIOType.Signals
                moIOConfigs = moPIOType.Configs
                pnlSim.Visible = False
                pnlSetVal.Visible = False
                mbOutputActive = False
            Case FRRobot.FREIOTypeConstants.frRDInType
                moRIOType = DirectCast(oIOTypeObj, FRRobot.FRCRobotIOType)
                msIOCOMMENT = gpsRM.GetString("psRI_CMT")
                moIOSignals = moRIOType.Signals
                moIOConfigs = Nothing
                pnlSim.Visible = True
                pnlSetVal.Visible = False
                mbOutputActive = False
            Case FRRobot.FREIOTypeConstants.frRDOutType
                moRIOType = DirectCast(oIOTypeObj, FRRobot.FRCRobotIOType)
                msIOCOMMENT = gpsRM.GetString("psRO_CMT")
                moIOSignals = moRIOType.Signals
                moIOConfigs = Nothing
                pnlSim.Visible = True
                pnlSetVal.Visible = True
                mbOutputActive = True
            Case FRRobot.FREIOTypeConstants.frSOPInType
                moSIOType = DirectCast(oIOTypeObj, FRRobot.FRCSOPIOType)
                msIOCOMMENT = gpsRM.GetString("psSI_CMT")
                moIOSignals = moSIOType.Signals
                moIOConfigs = Nothing
                pnlSim.Visible = False
                pnlSetVal.Visible = False
                mbOutputActive = False
            Case FRRobot.FREIOTypeConstants.frSOPOutType
                moSIOType = DirectCast(oIOTypeObj, FRRobot.FRCSOPIOType)
                msIOCOMMENT = gpsRM.GetString("psSO_CMT")
                moIOSignals = moSIOType.Signals
                moIOConfigs = Nothing
                pnlSim.Visible = False
                pnlSetVal.Visible = False
                mbOutputActive = False
            Case FRRobot.FREIOTypeConstants.frTPInType
                moTPIOType = DirectCast(oIOTypeObj, FRRobot.FRCTPIOType)
                msIOCOMMENT = gpsRM.GetString("psTPI_CMT")
                moIOSignals = moTPIOType.Signals
                moIOConfigs = Nothing
                pnlSim.Visible = False
                pnlSetVal.Visible = False
                mbOutputActive = False
            Case FRRobot.FREIOTypeConstants.frTPOutType
                moTPIOType = DirectCast(oIOTypeObj, FRRobot.FRCTPIOType)
                msIOCOMMENT = gpsRM.GetString("psTPO_CMT")
                moIOSignals = moTPIOType.Signals
                moIOConfigs = Nothing
                pnlSim.Visible = False
                pnlSetVal.Visible = False
                mbOutputActive = False
            Case FRRobot.FREIOTypeConstants.frUOPInType
                moUIOType = DirectCast(oIOTypeObj, FRRobot.FRCUOPIOType)
                msIOCOMMENT = gpsRM.GetString("psUI_CMT")
                moIOSignals = moUIOType.Signals
                moIOConfigs = moUIOType.Configs
                pnlSim.Visible = False
                pnlSetVal.Visible = False
                mbOutputActive = False
            Case FRRobot.FREIOTypeConstants.frUOPOutType
                moUIOType = DirectCast(oIOTypeObj, FRRobot.FRCUOPIOType)
                msIOCOMMENT = gpsRM.GetString("psUO_CMT")
                moIOSignals = moUIOType.Signals
                moIOConfigs = moUIOType.Configs
                pnlSim.Visible = False
                pnlSetVal.Visible = False
                mbOutputActive = False
            Case FRRobot.FREIOTypeConstants.frWDInType
                moWIOType = DirectCast(oIOTypeObj, FRRobot.FRCWeldDigitalIOType)
                msIOCOMMENT = gpsRM.GetString("psWI_CMT")
                moIOSignals = moWIOType.Signals
                moIOConfigs = moWIOType.Configs
                pnlSim.Visible = False
                pnlSetVal.Visible = False
                mbOutputActive = False
            Case FRRobot.FREIOTypeConstants.frWDOutType
                moWIOType = DirectCast(oIOTypeObj, FRRobot.FRCWeldDigitalIOType)
                msIOCOMMENT = gpsRM.GetString("psWO_CMT")
                moIOSignals = moWIOType.Signals
                moIOConfigs = moWIOType.Configs
                pnlSim.Visible = False
                pnlSetVal.Visible = False
                mbOutputActive = False
                'Case FRRobot.FREIOTypeConstants.frWSTKInType
                'Case FRRobot.FREIOTypeConstants.frWSTKOutType
            Case Else
                pnlSim.Visible = False
                pnlSetVal.Visible = False
                mbOutputActive = False
        End Select
        If moIOSignals IsNot Nothing Then
            moIOSignals.NoRefresh = True
        End If
        subShowNewPage(True)

    End Sub

    Private Sub mnuRefresh_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuRefresh.Click
        '********************************************************************************************
        'Description:  AutoRefresh dropdown menu
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        tmrRefresh.Enabled = False
        dlgRefreshRate.Text = gpsRM.GetString("psDLG_REF_TITLE")
        dlgRefreshRate.lblDlgRefreshRate.Text = gpsRM.GetString("psDLG_REF_LABEL")
        dlgRefreshRate.numRefreshRate.Value = tmrRefresh.Interval
        Dim odr As DialogResult = dlgRefreshRate.ShowDialog
        If odr = Windows.Forms.DialogResult.OK Then
            tmrRefresh.Interval = CType(dlgRefreshRate.numRefreshRate.Value, Integer)
            If mbAutoRefresh = False Then
                mbAutoRefresh = True
                btnAutoRefresh.Image = DirectCast(gpsRM.GetObject("LIGHTON"), Image)
                mnuRefresh.Text = tmrRefresh.Interval & gpsRM.GetString("psMS")
            End If
        End If
        tmrRefresh.Enabled = mbAutoRefresh
    End Sub

    Private Sub btnSim_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSim.Click
        '********************************************************************************************
        'Description:  Simulate button
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        tmrRefresh.Enabled = False
        Dim sNewVal As String = String.Empty
        Dim sOldVal As String = String.Empty
        If mbSimButtonSim Then
            'Warn before commiting
            Dim oDR As DialogResult = MessageBox.Show(gpsRM.GetString("psSIM_WARN_TXT"), gpsRM.GetString("psSIM_WARN_CAP"), _
                          MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            If oDR <> Windows.Forms.DialogResult.OK Then
                Exit Sub
            End If
            sOldVal = gpsRM.GetString("psOFF")
            sNewVal = gpsRM.GetString("psON")
        Else
            'No warning to unsim
            sOldVal = gpsRM.GetString("psON")
            sNewVal = gpsRM.GetString("psOFF")
        End If
        Dim bOK As Boolean = True
        Try
            If TypeOf (moPointDetailSignal) Is FRRobot.FRCAnalogIOSignal Then
                Dim oIOPoint As FRRobot.FRCAnalogIOSignal = DirectCast(moPointDetailSignal, FRRobot.FRCAnalogIOSignal)
                Try
                    oIOPoint.Simulate = mbSimButtonSim
                Catch ex As Exception
                    MessageBox.Show(gpsRM.GetString("psSET_SIM_FAILED_TEXT"), gpsRM.GetString("psSET_SIM_FAILED_CAP"), _
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
                    bOK = False
                End Try
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCDigitalIOSignal Then
                Dim oIOPoint As FRRobot.FRCDigitalIOSignal = DirectCast(moPointDetailSignal, FRRobot.FRCDigitalIOSignal)
                Try
                    oIOPoint.Simulate = mbSimButtonSim
                Catch ex As Exception
                    MessageBox.Show(gpsRM.GetString("psSET_SIM_FAILED_TEXT"), gpsRM.GetString("psSET_SIM_FAILED_CAP"), _
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
                    bOK = False
                End Try
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCFlagSignal Then
                Dim oIOPoint As FRRobot.FRCFlagSignal = DirectCast(moPointDetailSignal, FRRobot.FRCFlagSignal)
                Try
                    oIOPoint.Simulate = mbSimButtonSim
                Catch ex As Exception
                    MessageBox.Show(gpsRM.GetString("psSET_SIM_FAILED_TEXT"), gpsRM.GetString("psSET_SIM_FAILED_CAP"), _
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
                    bOK = False
                End Try
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCGroupIOSignal Then
                Dim oIOPoint As FRRobot.FRCGroupIOSignal = DirectCast(moPointDetailSignal, FRRobot.FRCGroupIOSignal)
                Try
                    oIOPoint.Simulate = mbSimButtonSim
                Catch ex As Exception
                    MessageBox.Show(gpsRM.GetString("psSET_SIM_FAILED_TEXT"), gpsRM.GetString("psSET_SIM_FAILED_CAP"), _
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
                    bOK = False
                End Try
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCMarkerSignal Then
                Dim oIOPoint As FRRobot.FRCMarkerSignal = DirectCast(moPointDetailSignal, FRRobot.FRCMarkerSignal)
                Try
                    oIOPoint.Simulate = mbSimButtonSim
                Catch ex As Exception
                    MessageBox.Show(gpsRM.GetString("psSET_SIM_FAILED_TEXT"), gpsRM.GetString("psSET_SIM_FAILED_CAP"), _
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
                    bOK = False
                End Try
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCPLCIOSignal Then
                Dim oIOPoint As FRRobot.FRCPLCIOSignal = DirectCast(moPointDetailSignal, FRRobot.FRCPLCIOSignal)
                'No Sim for this type
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCRobotIOSignal Then
                Dim oIOPoint As FRRobot.FRCRobotIOSignal = DirectCast(moPointDetailSignal, FRRobot.FRCRobotIOSignal)
                Try
                    oIOPoint.Simulate = mbSimButtonSim
                Catch ex As Exception
                    MessageBox.Show(gpsRM.GetString("psSET_SIM_FAILED_TEXT"), gpsRM.GetString("psSET_SIM_FAILED_CAP"), _
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
                    bOK = False
                End Try
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCSOPIOSignal Then
                Dim oIOPoint As FRRobot.FRCSOPIOSignal = DirectCast(moPointDetailSignal, FRRobot.FRCSOPIOSignal)
                'No Sim for this type
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCTPIOSignal Then
                Dim oIOPoint As FRRobot.FRCTPIOSignal = DirectCast(moPointDetailSignal, FRRobot.FRCTPIOSignal)
                'No Sim for this type
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCUOPIOSignal Then
                Dim oIOPoint As FRRobot.FRCUOPIOSignal = DirectCast(moPointDetailSignal, FRRobot.FRCUOPIOSignal)
                'No Sim for this type
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCWeldDigitalIOSignal Then
                Dim oIOPoint As FRRobot.FRCWeldDigitalIOSignal = DirectCast(moPointDetailSignal, FRRobot.FRCWeldDigitalIOSignal)
                Try
                    oIOPoint.Simulate = mbSimButtonSim
                Catch ex As Exception
                    MessageBox.Show(gpsRM.GetString("psSET_SIM_FAILED_TEXT"), gpsRM.GetString("psSET_SIM_FAILED_CAP"), _
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
                    bOK = False
                End Try
            End If
            If bOK Then
                subUpdateChangeLog(sNewVal, sOldVal, mcolZones.ActiveZone, mController.Name, gpsRM.GetString("psCL_SET_SIM") & msLabel)
            End If
            If mbSimButtonSim And bOK Then
                btnSim.Text = gpsRM.GetString("psUNSIMULATE")
                mbSimButtonSim = False
                pnlSetVal.Visible = True
            Else
                btnSim.Text = gpsRM.GetString("psSIMULATE")
                mbSimButtonSim = True
                pnlSetVal.Visible = mbOutputActive
            End If
        Catch ex As Exception

        End Try
        subRefresh(True)
        tmrRefresh.Enabled = mbAutoRefresh
        'If mcolZones.SaveAccessData Then
        '    mPWCommon.SaveToChangeLog(mcolZones.ActiveZone.DatabasePath)
        'End If
        mPWCommon.SaveToChangeLog(mcolZones.ActiveZone)


    End Sub

    Private Sub btnSetVal_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetVal.Click
        '********************************************************************************************
        'Description:  Set value button
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        tmrRefresh.Enabled = False
        'Warn before commiting
        Dim oDR As DialogResult = MessageBox.Show(gpsRM.GetString("psSET_WARN_TXT"), gpsRM.GetString("psSET_WARN_CAP"), _
                      MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        If oDR <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim sNewVal As String = String.Empty
        Dim sOldVal As String = String.Empty
        Try
            If TypeOf (moPointDetailSignal) Is FRRobot.FRCAnalogIOSignal Then
                Dim oIOPoint As FRRobot.FRCAnalogIOSignal = DirectCast(moPointDetailSignal, FRRobot.FRCAnalogIOSignal)
                Dim nTmp As Integer = nGetIntValFromText(txtSetVal.Text)
                If nTmp >= 0 Then
                    Try
                        sOldVal = PrintVal(oIOPoint.Value)
                        oIOPoint.Value = nTmp
                        sNewVal = PrintVal(nTmp)
                        subUpdateChangeLog(sNewVal, sOldVal, mcolZones.ActiveZone, mController.Name, gpsRM.GetString("psCL_SET_VAL") & msLabel)
                    Catch ex As Exception
                        MessageBox.Show(gpsRM.GetString("psSET_FAILED_TEXT"), gpsRM.GetString("psSET_FAILED_CAP"), _
                              MessageBoxButtons.OK, MessageBoxIcon.Error)
                        txtSetVal.Text = oIOPoint.Value.ToString
                    End Try
                Else
                    txtSetVal.Text = oIOPoint.Value.ToString
                End If
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCDigitalIOSignal Then
                Dim oIOPoint As FRRobot.FRCDigitalIOSignal = DirectCast(moPointDetailSignal, FRRobot.FRCDigitalIOSignal)
                Dim nTmp As Integer = nGetBoolValFromText(txtSetVal.Text)
                Select Case nTmp
                    Case 0, 1
                        Dim bTmp As Boolean = (nTmp = 1)
                        Try
                            sOldVal = PrintVal(oIOPoint.Value)
                            oIOPoint.Value = bTmp
                            sNewVal = PrintVal(bTmp)
                            subUpdateChangeLog(sNewVal, sOldVal, mcolZones.ActiveZone, mController.Name, gpsRM.GetString("psCL_SET_VAL") & msLabel)
                        Catch ex As Exception
                            MessageBox.Show(gpsRM.GetString("psSET_FAILED_TEXT"), gpsRM.GetString("psSET_FAILED_CAP"), _
                                  MessageBoxButtons.OK, MessageBoxIcon.Error)
                            txtSetVal.Text = oIOPoint.Value.ToString
                        End Try
                    Case -1
                        txtSetVal.Text = oIOPoint.Value.ToString
                End Select
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCFlagSignal Then
                Dim oIOPoint As FRRobot.FRCFlagSignal = DirectCast(moPointDetailSignal, FRRobot.FRCFlagSignal)
                Dim nTmp As Integer = nGetBoolValFromText(txtSetVal.Text)
                Select Case nTmp
                    Case 0, 1
                        Dim bTmp As Boolean = (nTmp = 1)
                        Try
                            sOldVal = PrintVal(oIOPoint.Value)
                            oIOPoint.Value = bTmp
                            sNewVal = PrintVal(bTmp)
                            subUpdateChangeLog(sNewVal, sOldVal, mcolZones.ActiveZone, mController.Name, gpsRM.GetString("psCL_SET_VAL") & msLabel)
                        Catch ex As Exception
                            MessageBox.Show(gpsRM.GetString("psSET_FAILED_TEXT"), gpsRM.GetString("psSET_FAILED_CAP"), _
                                  MessageBoxButtons.OK, MessageBoxIcon.Error)
                            txtSetVal.Text = oIOPoint.Value.ToString
                        End Try
                    Case -1
                        txtSetVal.Text = oIOPoint.Value.ToString
                End Select
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCGroupIOSignal Then
                Dim oIOPoint As FRRobot.FRCGroupIOSignal = DirectCast(moPointDetailSignal, FRRobot.FRCGroupIOSignal)
                Dim nTmp As Integer = nGetIntValFromText(txtSetVal.Text)
                If nTmp >= 0 Then
                    Try
                        sOldVal = PrintVal(nTmp)
                        oIOPoint.Value = nTmp
                        sNewVal = PrintVal(nTmp)
                        subUpdateChangeLog(sNewVal, sOldVal, mcolZones.ActiveZone, mController.Name, gpsRM.GetString("psCL_SET_VAL") & msLabel)
                    Catch ex As Exception
                        MessageBox.Show(gpsRM.GetString("psSET_FAILED_TEXT"), gpsRM.GetString("psSET_FAILED_CAP"), _
                              MessageBoxButtons.OK, MessageBoxIcon.Error)
                        txtSetVal.Text = oIOPoint.Value.ToString
                    End Try
                Else
                    txtSetVal.Text = oIOPoint.Value.ToString
                End If
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCMarkerSignal Then
                Dim oIOPoint As FRRobot.FRCMarkerSignal = DirectCast(moPointDetailSignal, FRRobot.FRCMarkerSignal)
                Dim nTmp As Integer = nGetBoolValFromText(txtSetVal.Text)
                Select Case nTmp
                    Case 0, 1
                        Dim bTmp As Boolean = (nTmp = 1)
                        Try
                            sOldVal = PrintVal(oIOPoint.Value)
                            oIOPoint.Value = bTmp
                            sNewVal = PrintVal(bTmp)
                            subUpdateChangeLog(sNewVal, sOldVal, mcolZones.ActiveZone, mController.Name, gpsRM.GetString("psCL_SET_VAL") & msLabel)
                        Catch ex As Exception
                            MessageBox.Show(gpsRM.GetString("psSET_FAILED_TEXT"), gpsRM.GetString("psSET_FAILED_CAP"), _
                                  MessageBoxButtons.OK, MessageBoxIcon.Error)
                            txtSetVal.Text = oIOPoint.Value.ToString
                        End Try
                    Case -1
                        txtSetVal.Text = oIOPoint.Value.ToString
                End Select
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCPLCIOSignal Then
                Dim oIOPoint As FRRobot.FRCPLCIOSignal = DirectCast(moPointDetailSignal, FRRobot.FRCPLCIOSignal)
                'No Sim for this type
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCRobotIOSignal Then
                Dim oIOPoint As FRRobot.FRCRobotIOSignal = DirectCast(moPointDetailSignal, FRRobot.FRCRobotIOSignal)
                Dim nTmp As Integer = nGetBoolValFromText(txtSetVal.Text)
                Select Case nTmp
                    Case 0, 1
                        Dim bTmp As Boolean = (nTmp = 1)
                        Try
                            sOldVal = PrintVal(oIOPoint.Value)
                            oIOPoint.Value = bTmp
                            sNewVal = PrintVal(bTmp)
                            subUpdateChangeLog(sNewVal, sOldVal, mcolZones.ActiveZone, mController.Name, gpsRM.GetString("psCL_SET_VAL") & msLabel)
                        Catch ex As Exception
                            MessageBox.Show(gpsRM.GetString("psSET_FAILED_TEXT"), gpsRM.GetString("psSET_FAILED_CAP"), _
                                  MessageBoxButtons.OK, MessageBoxIcon.Error)
                            txtSetVal.Text = oIOPoint.Value.ToString
                        End Try
                    Case -1
                        txtSetVal.Text = oIOPoint.Value.ToString
                End Select
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCSOPIOSignal Then
                Dim oIOPoint As FRRobot.FRCSOPIOSignal = DirectCast(moPointDetailSignal, FRRobot.FRCSOPIOSignal)
                'No Sim for this type
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCTPIOSignal Then
                Dim oIOPoint As FRRobot.FRCTPIOSignal = DirectCast(moPointDetailSignal, FRRobot.FRCTPIOSignal)
                'No Sim for this type
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCUOPIOSignal Then
                Dim oIOPoint As FRRobot.FRCUOPIOSignal = DirectCast(moPointDetailSignal, FRRobot.FRCUOPIOSignal)
                'No Sim for this type
            ElseIf TypeOf (moPointDetailSignal) Is FRRobot.FRCWeldDigitalIOSignal Then
                Dim oIOPoint As FRRobot.FRCWeldDigitalIOSignal = DirectCast(moPointDetailSignal, FRRobot.FRCWeldDigitalIOSignal)
                Dim nTmp As Integer = nGetBoolValFromText(txtSetVal.Text)
                Select Case nTmp
                    Case 0, 1
                        Dim bTmp As Boolean = (nTmp = 1)
                        Try
                            sOldVal = PrintVal(oIOPoint.Value)
                            oIOPoint.Value = bTmp
                            sNewVal = PrintVal(bTmp)
                            subUpdateChangeLog(sNewVal, sOldVal, mcolZones.ActiveZone, mController.Name, gpsRM.GetString("psCL_SET_VAL") & msLabel)
                        Catch ex As Exception
                            MessageBox.Show(gpsRM.GetString("psSET_FAILED_TEXT"), gpsRM.GetString("psSET_FAILED_CAP"), _
                                  MessageBoxButtons.OK, MessageBoxIcon.Error)
                            txtSetVal.Text = oIOPoint.Value.ToString
                        End Try
                    Case -1
                        txtSetVal.Text = oIOPoint.Value.ToString
                End Select
            End If
        Catch ex As Exception

        End Try
        subRefresh(True)
        tmrRefresh.Enabled = mbAutoRefresh
        'If mcolZones.SaveAccessData Then
        '    mPWCommon.SaveToChangeLog(mcolZones.ActiveZone.DatabasePath)
        'End If
        mPWCommon.SaveToChangeLog(mcolZones.ActiveZone)
    End Sub
    Private Function nGetBoolValFromText(ByVal sText As String) As Integer
        '********************************************************************************************
        'Description:  Get boolean value from textbox
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If IsNumeric(sText) Then
                Dim nTmp As Integer = CType(sText, Integer)
                If nTmp = 0 Then
                    Return nTmp
                ElseIf nTmp = 1 Then
                    Return nTmp
                End If
            ElseIf (sText.Trim.ToLower = "on") OrElse (sText.Trim.ToLower = "true") Then
                Return 1
            ElseIf (sText.Trim.ToLower = "off") OrElse (sText.Trim.ToLower = "false") Then
                Return 0
            End If

        Catch ex As Exception

        End Try
        'Couldn't figure it out
        Dim oDR As DialogResult = MessageBox.Show(gpsRM.GetString("psINV_BOOL_TEXT"), gpsRM.GetString("psINV_BOOL_CAP"), _
              MessageBoxButtons.OK, MessageBoxIcon.Error)
        Return -1

    End Function
    Private Function nGetIntValFromText(ByVal sText As String) As Integer
        '********************************************************************************************
        'Description:  Get integer value from textbox
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If IsNumeric(sText) Then
                Dim nTmp As Integer = CType(sText, Integer)

                Return nTmp

            End If

        Catch ex As Exception

        End Try
        'Couldn't figure it out
        Dim oDR As DialogResult = MessageBox.Show(gpsRM.GetString("psINV_INT_TEXT"), gpsRM.GetString("psINV_INT_CAP"), _
              MessageBoxButtons.OK, MessageBoxIcon.Error)
        Return -1


    End Function

    Private Sub btnSetComment_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetComment.Click
        '********************************************************************************************
        'Description:  Change the comment
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        tmrRefresh.Enabled = False
        dlgSetComment.Text = gpsRM.GetString("psDLG_CMT_TITLE")
        dlgSetComment.lblSetComment.Text = gpsRM.GetString("psDLG_CMT_LABEL")
        Dim oSignal As FRRobot.FRCIOSignal = DirectCast(moPointDetailSignal, FRRobot.FRCIOSignal)
        Dim sOldVal As String = oSignal.Comment
        dlgSetComment.txtSetComment.Text = sOldVal
        Dim odr As DialogResult = dlgSetComment.ShowDialog
        If odr = Windows.Forms.DialogResult.OK Then
            Try
                oSignal.Comment = dlgSetComment.txtSetComment.Text
                Dim sLabel As String = String.Format(msIOCOMMENT, mnDetailPoint, dlgSetComment.txtSetComment.Text)
                subUpdateChangeLog(dlgSetComment.txtSetComment.Text, sOldVal, mcolZones.ActiveZone, mController.Name, gpsRM.GetString("psCL_SET_CMT") & sLabel)

            Catch ex As Exception
                MessageBox.Show(gpsRM.GetString("psSET_CMT_FAIL_TEXT"), gpsRM.GetString("psSET_CMT_FAIL_CAP"), _
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
        subShowNewPage(True)
        tmrRefresh.Enabled = mbAutoRefresh
        'If mcolZones.SaveAccessData Then
        '    mPWCommon.SaveToChangeLog(mcolZones.ActiveZone.DatabasePath)
        'End If
        mPWCommon.SaveToChangeLog(mcolZones.ActiveZone)

    End Sub

    Private Sub btnUnsimAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUnsimAll.Click
        '********************************************************************************************
        'Description:  Unsimulate all I/O
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If moAllIO IsNot Nothing Then
            moAllIO.Unsimulate()
            subUpdateChangeLog(mcolZones.ActiveZone, mController.Name, gpsRM.GetString("psCL_UNSIMALL"))
        Else
            Debug.Assert(False)
        End If
        'If mcolZones.SaveAccessData Then
        '    mPWCommon.SaveToChangeLog(mcolZones.ActiveZone.DatabasePath)
        'End If
        mPWCommon.SaveToChangeLog(mcolZones.ActiveZone)
    End Sub

    Private Sub btnMonitor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMonitor.Click
        '********************************************************************************************
        'Description:  Unsimulate all I/O
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/11/12  MSW     btnMonitor_Click, subGetSignalDetails - 
        '                   Add error checking for robot IO unassigned or offline
        '********************************************************************************************
        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        If ((dgMonitor.RowCount = 0) Or (dgMonitor.ColumnCount < 3)) Then

            dgMonitor.Columns.Clear()
            dgMonitor.Rows.Clear()

            dgMonitor.Columns.Add("ROBOT", gcsRM.GetString("csROBOT"))
            dgMonitor.Columns.Add("LABEL", gpsRM.GetString("psCOL_LABEL"))
            dgMonitor.Columns.Add("SIM", gpsRM.GetString("psCOL_SIM"))
            dgMonitor.Columns.Add("VALUE", gpsRM.GetString("psCOL_VALUE"))


        End If
        dgMonitor.Rows.Add()
        dgMonitor.Rows(dgMonitor.RowCount - 1).Cells(0).Value = mController.Name
        dgMonitor.Rows(dgMonitor.RowCount - 1).Cells(1).Value = msLabel
        dgMonitor.Rows(dgMonitor.RowCount - 1).Cells(2).Value = msSim
        dgMonitor.Rows(dgMonitor.RowCount - 1).Cells(3).Value = msValue
        Dim oMonitor As New clsIOMonitor
        oMonitor.oPoint = DirectCast(moPointDetailSignal, FRRobot.FRCIOSignal)
        oMonitor.nPoint = mnDetailPoint
        oMonitor.sLabel = msLabel
        oMonitor.oDG = dgMonitor
        oMonitor.nRow = dgMonitor.RowCount - 1
        oMonitor.nSimColumn = 2
        oMonitor.nValColumn = 3
        If oMonitor.oPoint.IsAssigned And Not oMonitor.oPoint.IsOffline Then
        oMonitor.oPoint.StartMonitor(100)
        End If

        moColMonitor.Add(oMonitor)

        Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub

    Private Sub dgMonitor_CellMouseEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgMonitor.CellMouseEnter
        mnMonitorCol = e.ColumnIndex
        mnMonitorRow = e.RowIndex
    End Sub

    Private Sub mnuClearAllMon_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuClearAllMon.Click
        moColMonitor.Clear()
        dgMonitor.Rows.Clear()
    End Sub

    Private Sub mnuEndMonitor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuEndMonitor.Click
        For nColIdx As Integer = moColMonitor.Count To 1 Step -1
            Dim oMon As clsIOMonitor = DirectCast(moColMonitor.Item(nColIdx), clsIOMonitor)
            If oMon.nRow = mnMonitorRow Then
                moColMonitor.Remove(nColIdx)
            ElseIf oMon.nRow > mnMonitorRow Then
                oMon.nRow = oMon.nRow - 1
            End If
        Next
        If mnMonitorRow < dgMonitor.Rows.Count Then
            dgMonitor.Rows.RemoveAt(mnMonitorRow)
        End If
    End Sub

    
    Private Sub dgIOPoints_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgIOPoints.Click
        subRefresh(True)
    End Sub
End Class

Friend Class clsIOMonitor
    Public WithEvents oPoint As FRRobot.FRCIOSignal
    Public sLabel As String
    Public nPoint As Integer
    Public nRow As Integer
    Public nSimColumn As Integer
    Public nValColumn As Integer
    Public oDG As DataGridView

    Private Function PrintVal(ByVal bTmp As Boolean) As String
        '********************************************************************************************
        'Description:  Get text for a boolean value
        '
        'Parameters: boolean 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If bTmp Then
            Return gpsRM.GetString("psON")
        Else
            Return gpsRM.GetString("psOFF")
        End If
    End Function

    Private Function PrintVal(ByVal nTmp As Integer) As String
        '********************************************************************************************
        'Description:  Get text for an integer value
        '
        'Parameters: boolean 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return nTmp.ToString
    End Function

    Private Sub oIO_Change() Handles oPoint.Change, oPoint.Simulate, oPoint.unSimulate
        Try
            If TypeOf (oPoint) Is FRRobot.FRCAnalogIOSignal Then
                Dim oIOPoint As FRRobot.FRCAnalogIOSignal = DirectCast(oPoint, FRRobot.FRCAnalogIOSignal)
                oDG.Rows(nRow).Cells(nValColumn).Value = PrintVal(oIOPoint.Value)
                oDG.Rows(nRow).Cells(nSimColumn).Value = PrintVal(oIOPoint.Simulate)
            ElseIf TypeOf (oPoint) Is FRRobot.FRCDigitalIOSignal Then
                Dim oIOPoint As FRRobot.FRCDigitalIOSignal = DirectCast(oPoint, FRRobot.FRCDigitalIOSignal)
                oDG.Rows(nRow).Cells(nValColumn).Value = PrintVal(oIOPoint.Value)
                oDG.Rows(nRow).Cells(nSimColumn).Value = PrintVal(oIOPoint.Simulate)
            ElseIf TypeOf (oPoint) Is FRRobot.FRCFlagSignal Then
                Dim oIOPoint As FRRobot.FRCFlagSignal = DirectCast(oPoint, FRRobot.FRCFlagSignal)
                oDG.Rows(nRow).Cells(nValColumn).Value = PrintVal(oIOPoint.Value)
                oDG.Rows(nRow).Cells(nSimColumn).Value = PrintVal(oIOPoint.Simulate)
            ElseIf TypeOf (oPoint) Is FRRobot.FRCGroupIOSignal Then
                Dim oIOPoint As FRRobot.FRCGroupIOSignal = DirectCast(oPoint, FRRobot.FRCGroupIOSignal)
                oDG.Rows(nRow).Cells(nValColumn).Value = PrintVal(oIOPoint.Value)
                oDG.Rows(nRow).Cells(nSimColumn).Value = PrintVal(oIOPoint.Simulate)
            ElseIf TypeOf (oPoint) Is FRRobot.FRCMarkerSignal Then
                Dim oIOPoint As FRRobot.FRCMarkerSignal = DirectCast(oPoint, FRRobot.FRCMarkerSignal)
                oDG.Rows(nRow).Cells(nValColumn).Value = PrintVal(oIOPoint.Value)
                oDG.Rows(nRow).Cells(nSimColumn).Value = PrintVal(oIOPoint.Simulate)
            ElseIf TypeOf (oPoint) Is FRRobot.FRCPLCIOSignal Then
                Dim oIOPoint As FRRobot.FRCPLCIOSignal = DirectCast(oPoint, FRRobot.FRCPLCIOSignal)
                oDG.Rows(nRow).Cells(nValColumn).Value = PrintVal(oIOPoint.Value)
                oDG.Rows(nRow).Cells(nSimColumn).Value = gpsRM.GetString("psNA")
            ElseIf TypeOf (oPoint) Is FRRobot.FRCRobotIOSignal Then
                Dim oIOPoint As FRRobot.FRCRobotIOSignal = DirectCast(oPoint, FRRobot.FRCRobotIOSignal)
                oDG.Rows(nRow).Cells(nValColumn).Value = PrintVal(oIOPoint.Value)
                oDG.Rows(nRow).Cells(nSimColumn).Value = PrintVal(oIOPoint.Simulate)
            ElseIf TypeOf (oPoint) Is FRRobot.FRCSOPIOSignal Then
                Dim oIOPoint As FRRobot.FRCSOPIOSignal = DirectCast(oPoint, FRRobot.FRCSOPIOSignal)
                oDG.Rows(nRow).Cells(nValColumn).Value = PrintVal(oIOPoint.Value)
                oDG.Rows(nRow).Cells(nSimColumn).Value = gpsRM.GetString("psNA")
            ElseIf TypeOf (oPoint) Is FRRobot.FRCTPIOSignal Then
                Dim oIOPoint As FRRobot.FRCTPIOSignal = DirectCast(oPoint, FRRobot.FRCTPIOSignal)
                oDG.Rows(nRow).Cells(nValColumn).Value = PrintVal(oIOPoint.Value)
                oDG.Rows(nRow).Cells(nSimColumn).Value = gpsRM.GetString("psNA")
            ElseIf TypeOf (oPoint) Is FRRobot.FRCUOPIOSignal Then
                Dim oIOPoint As FRRobot.FRCUOPIOSignal = DirectCast(oPoint, FRRobot.FRCUOPIOSignal)
                oDG.Rows(nRow).Cells(nValColumn).Value = PrintVal(oIOPoint.Value)
                oDG.Rows(nRow).Cells(nSimColumn).Value = gpsRM.GetString("psNA")
            ElseIf TypeOf (oPoint) Is FRRobot.FRCWeldDigitalIOSignal Then
                Dim oIOPoint As FRRobot.FRCWeldDigitalIOSignal = DirectCast(oPoint, FRRobot.FRCWeldDigitalIOSignal)
                oDG.Rows(nRow).Cells(nValColumn).Value = PrintVal(oIOPoint.Value)
                oDG.Rows(nRow).Cells(nSimColumn).Value = PrintVal(oIOPoint.Simulate)
            End If
        Catch ex As Exception

        End Try

    End Sub
End Class