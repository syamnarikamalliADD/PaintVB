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
' Description:
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Fanuc Programmer
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    08/24/10   MSW     handle different scattered access lists - force resize to smaller list, too
'    09/14/11   MSW     Assemble a standard version of everything                       4.1.0.0
'    12/02/11   MSW     Add dmoncfg reference                                          4.1.1.0
'    02/15/12   MSW     force 32 bit build, print management updates                   4.1.1.1
'    03/23/12   RJO     modifed for .NET Password and IPC                              4.01.02.00
'    03/28/12   MSW     modifed for .NET Scattered Access                              4.01.03.00
'    04/20/12   MSW     frmMain_KeyDown-Add excape key handler                         4.01.03.01
'                       Pass arm collection to devices screen
'    09/14/12   RJO     Bug fix to frmEdit.subSetRepairPanelNames                      4.01.03.02
'    10/16/12   MSW     Better error hanling for scattered access                      4.01.03.03
'    11/09/12   MSW     Bug fixes to uctlLifeCycles                                    4.01.03.04
'    05/06/13   MSW     Honda Canada Updates                                           4.01.05.00
'    03/05/14   MSW     ZDT Changes                                                     4.01.07.00
'********************************************************************************************
Imports System.Windows.Forms.Application
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
    Public Const msSCREEN_NAME As String = "BSD"   ' <-- For password area change log etc.
    Private msSCREEN_DUMP_NAME As String = "View_BSD"
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
    Friend WithEvents colControllers As clsControllers = Nothing
    Friend WithEvents colArms As clsArms = Nothing
    Friend WithEvents colApplicators As clsApplicators = Nothing
    Private mRobot As clsArm = Nothing
    Private mApplicator As clsApplicator = Nothing
    Private mbRemoteZone As Boolean = False
    Private mbEventBlocker As Boolean = False
    Private mbScreenLoaded As Boolean = False
    'The booth and queue screens get variables of the base classes so they can be assigned to the different views dynamically
    'they inherit from both UserControl and BSDForm, but each base class needs a seperate variable to get to it's own routines
    'We could turn off the option strict stuff, but I think it's worth a little extra trouble to get the error checking
    Private mUctlBooth As UserControl = Nothing
    Private mUctlQueue As UserControl = Nothing
    Private mBSDFormBooth As BSDForm = Nothing
    Private mBSDFormQueue As BSDForm = Nothing
    'Process always uses the same UserControl, so we can assign the specific type
    Private mUctlProcess As uctlProcessScreen = Nothing
    'Likewise with LifeCycles
    Private mUctlLifeCycles As uctlLifeCycles = Nothing
    'These get assigned to the current screen on the fly
    Public gBSDFormCurrentScreen As BSDForm = Nothing
    Public gUctlCurrentScreen As UserControl = Nothing

    'More vars to manage the current screen, remember if they've been set up
    Private mbInitScreen(eBSDScreenSelect.LifeCycles) As Boolean
    Private meCurrentScreen As eBSDScreenSelect

    ' 03/05/14  MSW     ZDT Changes
    Friend WithEvents oZDT As clsZDTMonitor = Nothing
    Friend WithEvents oOVC As clsOVCMonitor = Nothing

    Private WithEvents mbgZDTWorker As System.ComponentModel.BackgroundWorker

    Friend mbSAInitComplete As Boolean = False

    'Communication vars
    Friend WithEvents mPLC As clsPLCComm = Nothing
    Private msSAData(1) As String  'Scattered access data 
    Friend msZoneData As String() = Nothing
    Friend msRobotData As String() = Nothing
    Friend msRobotTemp1 As String() = Nothing
    Friend msRobotTemp2 As String() = Nothing
    Friend msQueueData As String() = Nothing
    Friend msQueueTemp1 As String() = Nothing
    Friend msQueueTemp2 As String() = Nothing
    Friend msDevData As String() = Nothing
    Private mbOfflineMessage As Boolean() '1 pop-up at a time when controller goes offline
    Private mbSAMessage As Boolean() 'write SA status update to status bar
    Private mbSAException As Boolean() '1 pop-up at a time when exception occurs rjo 03/29/11
    'Languange
    Private msCulture As String = "en-US" 'Default to english
    Private mbShutDown As Boolean = False
    Public Structure tSAIndex   'Index scattered access arrays
        Dim Indexes() As Integer
    End Structure
    Public gtScatteredAccessIndexes As tSAIndex()
    Private bIndexScatteredAccess() As Boolean
    Friend gSAConfig As clsScatteredAccessGlobal
    Private msSAEnableString As String = String.Empty
    'ID's for each full screen that is launched with a UserControl
    'Conveyor, change log are pop-up forms
    'However many BSD or Queue controls are used, they share IDs
    Public Enum eBSDScreenSelect
        Booth = 0
        Queue = 1
        Process = 2
        LifeCycles = 3
    End Enum
    'Tracking setup read from PLC, share with controls
    Public gnQueueShiftPoint As Integer = 0
    Public gnMMperPulse As Integer = 0
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private mbEditsMade As Boolean = False
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean = False
    Private mnProgress As Integer = 0
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    Private mnSAMask As Integer = 0
    Private mnSAEnableArm() As Boolean
    Private mbLine2 As Boolean = False
    '******** End Property Variables    *************************************************************

    '******** This is the old pw3 password object interop  *****************************************
    'Friend WithEvents moPassword As PWPassword.PasswordObject 'RJO 03/23/12
    'Friend moPrivilege As PWPassword.cPrivilegeObject 'RJO 03/23/12
    '******** This is the old pw3 password object interop  *****************************************

    Friend WithEvents moPassword As clsPWUser 'RJO 03/23/12
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm 'RJO 03/23/12
    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet)  'RJO 03/23/12
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
            Control.CheckForIllegalCrossThreadCalls = False

            mPrivilegeRequested = Value

            'handle logout
            If mPrivilegeRequested = ePrivilege.None Then
                mPrivilegeGranted = ePrivilege.None
                subEnableControls(True)
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
                'this is needed with old password object raising events
                'and can hopefully be removed when password is redone
                Control.CheckForIllegalCrossThreadCalls = True
                If gBSDFormCurrentScreen Is Nothing = False Then
                    gBSDFormCurrentScreen.PrivilegeChange(mPrivilegeGranted)
                End If
                Exit Property
            End If

            'prevent recursion
            If mPrivilegeGranted = mPrivilegeRequested Then
                'this is needed with old password object raising events
                'and can hopefully be removed when password is redone
                Control.CheckForIllegalCrossThreadCalls = True
                If gBSDFormCurrentScreen Is Nothing = False Then
                    gBSDFormCurrentScreen.PrivilegeChange(mPrivilegeGranted)
                End If
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
                If moPassword.UserName <> String.Empty Then _
                        mPrivilegeRequested = mPrivilegeGranted
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
            End If

            If gBSDFormCurrentScreen Is Nothing = False Then
                gBSDFormCurrentScreen.PrivilegeChange(mPrivilegeGranted)
            End If

            'this is needed with old password object raising events
            'and can hopefully be removed when password is redone
            Control.CheckForIllegalCrossThreadCalls = True

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
                'subShowNewPage()
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

    Friend Property Line2() As Boolean
        '********************************************************************************************
        'Description:  Flags Line 1 or Line 2
        '
        'Parameters: set when form is loaded
        'Returns:    True if Line 2
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbLine2
        End Get
        Set(ByVal Value As Boolean)
            mbLine2 = Value
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
    Public Property Progress() As Integer
        '********************************************************************************************
        'Description:  run the progress bar
        '
        'Parameters: 1 to 100 percent
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal Value As Integer)
            If Value < 0 Then Value = 0
            If Value >= 100 Then Value = 0
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
        End Set
        Get
            Return mnProgress
        End Get
    End Property
    Public Property Status(Optional ByVal StatusStrip As Boolean = False) As String
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
            Return lblStatus.Text
        End Get
        Set(ByVal Value As String)
            If StatusStrip = False Then
                Dim sTmp As String = Value & vbTab & Format(Now, "Long time")
                lstStatus.Items.Add(sTmp)
            End If
            lblStatus.Text = Strings.Replace(Value, vbTab, "  ")
        End Set
    End Property
    Friend Property ScatteredAccessEnableArm(ByVal Index As Integer) As Boolean
        '********************************************************************************************
        'Description:  set bits in an array to determine which robots to read scattered access data from
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnSAEnableArm(Index)
        End Get
        Set(ByVal value As Boolean)
            mnSAEnableArm(Index) = value
        End Set
    End Property
    Friend Property ScatteredAccessEnableController(ByVal Index As Integer) As Boolean
        '********************************************************************************************
        'Description:  get or set scattered access enable for all arms on a control
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            'Return true if any arm is enabled
            For Each oArm As clsArm In colControllers(Index).Arms
                If mnSAEnableArm(oArm.ArmNumber) Then
                    Return True
                End If
            Next
            Return False
        End Get
        Set(ByVal value As Boolean)
            'set all the arms on the controller
            For Each oArm As clsArm In colControllers(Index).Arms
                mnSAEnableArm(oArm.ArmNumber) = value
            Next
        End Set
    End Property
#End Region
    Friend ReadOnly Property ScatteredAccessEnableString() As String
        '********************************************************************************************
        'Description:  get or set scattered access enable for all arms on a control
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Dim sTmp As String = String.Empty
            For Each oArm As clsArm In colArms
                If mnSAEnableArm(oArm.ArmNumber) Then
                    sTmp = sTmp & oArm.Name & ";"
                End If
            Next
            Return sTmp
        End Get
    End Property
#Region " Routines "

    Private Sub subToolStripButtonClicked(ByVal ButtonName As String)
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
        Select Case ButtonName ' case sensitive

            Case "btnBooth"
                'Set the button status
                btnBooth.CheckState = CheckState.Checked
                btnQueue.CheckState = CheckState.Unchecked
                btnGhost.CheckState = CheckState.Unchecked
                btnZDT.CheckState = CheckState.Unchecked
                btnConv.CheckState = CheckState.Unchecked
                'Check if it needs an init
                If mbInitScreen(eBSDScreenSelect.Booth) Then
                    mBSDFormBooth.Initialize()
                    mbInitScreen(eBSDScreenSelect.Booth) = False
                End If
                meCurrentScreen = eBSDScreenSelect.Booth
                'Show the control
                subSizeUserControl(mUctlBooth)

            Case "btnQueue"
                'Set the button status
                btnBooth.CheckState = CheckState.Unchecked
                btnQueue.CheckState = CheckState.Checked
                btnGhost.CheckState = CheckState.Unchecked
                btnZDT.CheckState = CheckState.Unchecked
                btnConv.CheckState = CheckState.Unchecked
                'Check if it needs an init
                If mbInitScreen(eBSDScreenSelect.Queue) Then
                    mBSDFormQueue.Initialize()
                    mbInitScreen(eBSDScreenSelect.Queue) = False
                End If
                meCurrentScreen = eBSDScreenSelect.Queue
                'Show the control
                subSizeUserControl(mUctlQueue)

            Case "btnGhost"
                mBSDCommon.subRunGhost()

            Case "btnZDT"
                mBSDCommon.subRunZDT()

            Case "btnProcess"
                If mbSAInitComplete Then
                'If the dropdown is selected, it's handled in btnProcess_DropDownItemClicked
                If btnProcess.DropDownButtonPressed Then Exit Sub
                'Set the button status
                btnBooth.CheckState = CheckState.Unchecked
                btnQueue.CheckState = CheckState.Unchecked
                btnGhost.CheckState = CheckState.Unchecked
                    btnZDT.CheckState = CheckState.Unchecked
                btnConv.CheckState = CheckState.Unchecked
                'See if the control has been created yet
                If (mUctlProcess Is Nothing) Then
                    mUctlProcess = New uctlProcessScreen
                    tscMain.ContentPanel.Controls.Add(mUctlProcess)
                End If
                'Check if it needs an init
                If mbInitScreen(eBSDScreenSelect.Process) Then 'If mbInitScreen(eBSDScreenSelect.LifeCycles) Then
                    mUctlProcess.Initialize("")
                    mbInitScreen(eBSDScreenSelect.Process) = False
                Else
                    mUctlProcess.SelectedRobot = String.Empty
                End If
                meCurrentScreen = eBSDScreenSelect.Process
                'Show the control
                subSizeUserControl(mUctlProcess)
                End If


            Case "btnConv"
                'Not a user control, launch pop-up
                Try
                    mBSDCommon.conveyorBingBoard()
                Catch

                End Try

                frmDevices.Robots = colArms.Count

                'frmDevices.PLCDataStart = 0
                'frmDevices.ZoneItems = 11
                'frmDevices.RobotItems = 10
                'Dim bHide(16) As Boolean
                'Dim sName As String = colZones.ActiveZone.Name.ToLower
                'For nItem As Integer = 0 To 16
                '    bHide(nItem) = False
                'Next

                'If InStr(sName, "teach") > 0 Then
                '    bHide(0) = True
                '    bHide(3) = True
                'End If
                'frmDevices.HideZoneItems = bHide

                'For nItem As Integer = 0 To 16
                '    bHide(nItem) = False
                'Next
                'If InStr(sName, "teach") > 0 Then
                '    bHide(2) = True
                'End If
                'frmDevices.HideRobotItems = bHide

                frmDevices.Show(msDevData, colArms)
        End Select

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
        ' 3/31/10   MSW     Support ascii color and style
        ' 03/05/14  MSW     ZDT Changes
        '********************************************************************************************

        'just in case
        If cboZone.Text = String.Empty Then
            Exit Sub
        Else
            If cboZone.Text = msOldZone Then Exit Sub
        End If

        Progress = 10
        msOldZone = cboZone.Text
        colZones.CurrentZone = cboZone.Text

        'Shut down any scattered access connections that are updating
        'Try
        '    If Not (colArms Is Nothing) Then
        '        For Each Controller As clsController In colControllers
        '            If Controller.Robot.IsConnected Then
        '                If oScatteredAccess.IsSetUp Then
        '                    oScatteredAccess.UpdateActive(Controller.Name) = False
        '                End If 'oSA.IsSetUp
        '            End If
        '        Next
        '    End If
        'Catch ex As Exception
        '    'Don't worry about it, just go on
        'End Try

        Dim sMessage(1) As String

        sMessage(0) = msSCREEN_NAME & ";OFF"
        sMessage(1) = "ALL"
        oIPC.WriteControlMsg(gs_COM_ID_SA, sMessage)

        Try
            Me.Cursor = System.Windows.Forms.Cursors.Default
            'Disable everything
            tmrSA.Enabled = False

            subEnableControls(False)
            'load the collections
            colControllers = New clsControllers(colZones, False)
            SetUpStatusStrip(Me, colControllers)
            colArms = LoadArmCollection(colControllers)
            'Load applicator/CC type details for arms in the zone
            colApplicators = New clsApplicators(colArms, colZones.ActiveZone)

            'Start up communication
            colControllers.RefreshConnectionStatus()
            'Clear out the previous styles collection
            colStyles = Nothing
            colStyles = New clsSysStyles(colZones.ActiveZone)

            'Deal with the UserControls that display the queue and booth screen
            'get rid of the old zone's screens
            If Not (mUctlBooth Is Nothing) Then
                tscMain.ContentPanel.Controls.Remove(mUctlBooth)
                mUctlBooth.Dispose()
            End If
            If Not (mUctlQueue Is Nothing) Then
                tscMain.ContentPanel.Controls.Remove(mUctlQueue)
                mUctlQueue.Dispose()
            End If

            ' 3/31/10   MSW     Support ascii color and style
            Dim sValves() As String = Nothing
            Dim oColors As XMLNodeList = Nothing
            Dim bColorsByStyle As Boolean
            Dim bUse2K As Boolean
            Dim bUseTricoat As Boolean
            Dim bTwoCoats As Boolean
            GetSystemColorInfoFromDB( colZones.ActiveZone, oColors, _
                                        sValves, gbAsciiColor, bColorsByStyle, bUse2K, bUseTricoat, gnAsciiColorNumChar, bTwoCoats)

            'Set up for zone just selected. This is your custom setup for left and right 
            'zones or as many screens as you need to set up. Make the Usercontrol you 
            'need, and assign the type in this case statement.
            Dim sName As String = colZones.ActiveZone.Name.ToLower

            Select Case sName
                Case "dap_prmint"
                    mUctlBooth = New uctlPrimeIntLeft
                Case "dap_prmext"
                    mUctlBooth = New uctlPrimeExt
                Case "dap_ccextl1"
                    mUctlBooth = New uctlCCExtRight
                Case "dap_ccextl2"
                    mUctlBooth = New uctlCCExtLeft
                Case Else
            End Select

            'Add the new controls to the collection,
            tscMain.ContentPanel.Controls.Add(mUctlBooth)
            'tscMain.ContentPanel.Controls.Add(mUctlQueue)
            'Hide them until they're needed
            mUctlBooth.Hide()
            'mUctlQueue.Hide()
            'Set up the pointers
            mBSDFormBooth = DirectCast(mUctlBooth, BSDForm)
            'mBSDFormQueue = DirectCast(mUctlQueue, BSDForm)

            'This tells it to init everything the first time each screen is selected for this zone
            For nScreen As Integer = mbInitScreen.GetLowerBound(0) To mbInitScreen.GetUpperBound(0)
                mbInitScreen(nScreen) = True
            Next

            'Add robots to drop-down menu for process button
            btnProcess.DropDownItems.Clear()
            For Each o As clsArm In colArms
                btnProcess.DropDownItems.Add(o.Name)
            Next

            subEnableControls(True)

            'Start up scan for booth, robot , and zone data
            subStartHotlinks()
            'Get tracking setup for booth screens
            subGetTrackingDataFromPLC()

            Dim sPrefix As String = "Z" & colZones.CurrentZoneNumber.ToString

            'Force a screen refresh
            subToolStripButtonClicked("btnBooth")

            Status = gpsRM.GetString("psINIT_SA")
            Dim oStart As DateTime = Now
            subInitScatteredAccess()
            Debug.Print("SA Startup" & (Now - oStart).ToString)
            Status = gpsRM.GetString("psINIT_SA_COMP")
            ' 03/05/14  MSW     ZDT Changes
            Status = gpsRM.GetString("psINIT_ZDT")
            oStart = Now
            Application.DoEvents()
            oZDT = New clsZDTMonitor(colZones.ActiveZone, colArms)
            Application.DoEvents()
            oOVC = New clsOVCMonitor(colZones.ActiveZone, colArms)
            Application.DoEvents()
            Debug.Print("ZDT Startup" & (Now - oStart).ToString)

            If mBSDCommon.oZDTBtn IsNot Nothing AndAlso oZDT IsNot Nothing AndAlso oZDT.Enabled Then
                oZDTBtn.Visible = True
                'Click handler for detail screen
                AddHandler oZDTBtn.Click, AddressOf ZDTAlertClick
                'Update text, color
                subUpdateZDT(oZDT.StatusText, oZDT.StatusColor)
            End If
            Status = gpsRM.GetString("psINIT_ZDT_COMP")


            Status = gcsRM.GetString("csREADY")
            DoEvents()
            tmrStartupDelay.Enabled = True

            'Enable scattered access update
            tmrSA.Enabled = True

            Status = gcsRM.GetString("csREADY")
        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csERROR") & " - frmMain.SubChangeZone", ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try


    End Sub
    Private Sub subGetTrackingDataFromPLC()
        '********************************************************************************************
        'Description: One time reads from the PLC at startup or zone selection
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim sPrefix As String = "Z" & colZones.CurrentZoneNumber.ToString

            With mPLC
                .ZoneName = colZones.ActiveZone.Name
                .TagName = sPrefix & "QueShiftPoint"
                Dim sTmp() As String = .PLCData
                gnQueueShiftPoint = CInt(sTmp(0))
                DoEvents()

                .TagName = sPrefix & "MMperPulse"
                sTmp = .PLCData
                gnMMperPulse = CInt(sTmp(0))

            End With
        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR") & " - frmMain.subGetTrackingDataFromPLC", ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        End Try

    End Sub
    Private Sub subStartHotlinks()
        '********************************************************************************************
        'Description: Links to plc data that are on all the time
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim sPrefix As String = "Z" & colZones.CurrentZoneNumber.ToString

        Status = gcsRM.GetString("csLINK_TO_PLC")

        Try
            If mPLC Is Nothing Then
                mPLC = New clsPLCComm
            Else
                'that didn't work
                ' mPLC.RemoveHotLinks()
                mPLC = Nothing
                DoEvents()
                mPLC = New clsPLCComm
            End If

            With mPLC
                .ZoneName = colZones.ActiveZone.Name

                .TagName = sPrefix & "BoothData"
                msZoneData = .PLCData

                DoEvents()

                .TagName = sPrefix & "RobotData"
                msRobotTemp1 = .PLCData

                DoEvents()
                If colArms.Count > 8 Then
                    .TagName = sPrefix & "RobotData2"
                    msRobotTemp2 = .PLCData
                    DoEvents()
                End If

                .TagName = sPrefix & "BoothQueue1"
                msQueueTemp1 = .PLCData

                DoEvents()

                .TagName = sPrefix & "BoothQueue2"
                msQueueTemp2 = .PLCData

                DoEvents()

                .TagName = sPrefix & "ConveyorBingoBoard"
                msDevData = .PLCData
            End With

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR") & " - frmMain.subStartHotlinks", ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)

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
            ShowErrorMessagebox(gcsRM.GetString("csERROR") & " - frmMain.subDoStatusBar", ex, msSCREEN_NAME, _
        Status, MessageBoxButtons.OK)
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
        Dim bData As Boolean = (bEnable And (cboZone.SelectedIndex > -1))

        btnClose.Enabled = True

        If bEnable = False Then
            btnChangeLog.Enabled = False
            btnStatus.Enabled = True
            btnQueue.Enabled = False
            btnBooth.Enabled = False
            btnProcess.Enabled = False
            btnUtil.Enabled = False
            btnConv.Enabled = False
            btnGhost.Enabled = False
            btnZDT.Enabled = False
            bRestOfControls = False
        Else
            Select Case Privilege
                Case ePrivilege.None
                    btnChangeLog.Enabled = bData
                    btnStatus.Enabled = True
                    bRestOfControls = False
                    btnQueue.Enabled = bData
                    btnBooth.Enabled = bData
                    btnProcess.Enabled = bData
                    btnUtil.Enabled = False
                    btnConv.Enabled = bData
                    btnGhost.Enabled = False
                    btnZDT.Enabled = False
                Case ePrivilege.Edit
                    btnChangeLog.Enabled = bData
                    btnStatus.Enabled = True
                    bRestOfControls = True
                    btnQueue.Enabled = bData
                    btnBooth.Enabled = bData
                    btnProcess.Enabled = bData
                    btnUtil.Enabled = (bData And (Not mbRemoteZone))
                    btnConv.Enabled = bData
                    btnGhost.Enabled = (bData And (Not mbRemoteZone))
                    btnZDT.Enabled = (bData And (Not mbRemoteZone))
            End Select
        End If

        'restof controls here


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
        lblZone.Text = gcsRM.GetString("csZONE_CAP")

        With gpsRM
            btnBooth.Text = .GetString("psBOOTH")
            btnQueue.Text = .GetString("psQUEUE")
            btnGhost.Text = .GetString("psGHOST")
            btnZDT.Text = .GetString("psZDT")
            btnProcess.Text = .GetString("psPROCESS")
            btnConv.Text = .GetString("psCONVEYOR")
            btnUtil.Text = .GetString("psUTILITIES")
            btnBooth.Image = DirectCast(.GetObject("imgBooth"), Image)
            btnQueue.Image = DirectCast(.GetObject("imgQueue"), Image)
            btnGhost.Image = DirectCast(.GetObject("imgGhost"), Image)
            btnProcess.Image = DirectCast(.GetObject("imgProcess"), Image)
            btnConv.Image = DirectCast(.GetObject("imgConv"), Image)
            btnUtil.Image = DirectCast(.GetObject("imgUtil"), Image)
            mnuResetJobCount.Text = .GetString("psRESET_JOB_COUNT")
            mnuLampTest.Text = .GetString("psLAMP_TEST")
        End With

    End Sub
    Private Sub subInitScatteredAccess()
        '********************************************************************************************
        'Description: set up scattered access for the zone
        '           Called by subChangeZone
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            'New scattered access
            gSAConfig = New clsScatteredAccessGlobal
            tmrSA.Interval = gSAConfig.UpdateRatems
            'Check for SA running
            ScatteredAccessRoutines.subStartupSA()

            For Each oArm As clsArm In colArms
                Dim oSAItems As clsScatteredAccessItems = New clsScatteredAccessItems(gSAConfig, oArm)
                oArm.Tag = oSAItems
                Application.DoEvents()
            Next
            'Track the Offline/Online status so it can throw up some messages: "No really, I'm not locked up"
            'These vars get toggled with the controller online status and the scattered access setup status
            ' so a message is displayed each time the status changes
            ReDim mbOfflineMessage(colControllers.Count - 1)
            ReDim mbSAMessage(colControllers.Count - 1)
            For iTmp As Integer = mbOfflineMessage.GetLowerBound(0) To mbOfflineMessage.GetUpperBound(0)
                mbOfflineMessage(iTmp) = True
                mbSAMessage(iTmp) = True
            Next

            'Reset scattered access indexes
            'Clear out the indexes on startup
            'The indexes get set for each robot the first time it gets scattered access data
            ReDim gtScatteredAccessIndexes(colArms.Count - 1)
            ReDim bIndexScatteredAccess(colArms.Count - 1)
            ReDim mnSAEnableArm(colArms.Count) '- 1)     BTK Arm Index is not zero indexed
            ReDim mbSAException(colControllers.Count - 1)

            For nRobot As Integer = 0 To (colArms.Count - 1)
                ReDim gtScatteredAccessIndexes(nRobot).Indexes(eSAIndex.Max)
                For Each i As Integer In gtScatteredAccessIndexes(nRobot).Indexes
                    i = -1 'Start with the index not found
                Next
                bIndexScatteredAccess(nRobot) = True
            Next

            'For Each o As clsController In colControllers
            '    'This enable variable can get updated by each screen that needs scattered access
            '    'Right now it's only used by the process screen
            '    ScatteredAccessEnableController(colControllers.IndexOf(o)) = False
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
            mbSAInitComplete = True
        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR") & " - frmMain.subInitScatteredAccess", ex, msSCREEN_NAME, _
Status, MessageBoxButtons.OK)

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

        Me.Cursor = System.Windows.Forms.Cursors.Default

        Status = gcsRM.GetString("csINITALIZING")
        Progress = 1


        Try
            DataLoaded = False
            EditsMade = False
            mbScreenLoaded = False

            subProcessCommandLine()
            'this was taking forever to show up
            Me.Show()
            Application.DoEvents()

            'init the old password for now
            'moPassword = New PWPassword.PasswordObject
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

            Progress = 5

            Me.Text = gpsRM.GetString("psSCREENCAPTION")

            'init new IPC and new Password 'RJO 03/23/12
            oIPC = New Paintworks_IPC.clsInterProcessComm
            moPassword = New clsPWUser
            Progress = 50
            colZones = New clsZones(String.Empty)

            mScreenSetup.LoadZoneBox(cboZone, colZones, False)
            mScreenSetup.InitializeForm(Me)

            subInitFormText()

            Progress = 70

            mScreenSetup.FinalizeToolStrip(tlsMain)

            'to get at images
            colImages = New Collection
            With colImages
                'P700s
                .Add(imlP700BL_Normal, "imlP700BL_Normal")
                .Add(imlP700BR_Normal, "imlP700BR_Normal")
                .Add(imlP700TL_Normal, "imlP700TL_Normal")
                .Add(imlP700TR_Normal, "imlP700TR_Normal")

                .Add(imlP700BL_Mouseover, "imlP700BL_Mouseover")
                .Add(imlP700BR_Mouseover, "imlP700BR_Mouseover")
                .Add(imlP700TL_Mouseover, "imlP700TL_Mouseover")
                .Add(imlP700TR_Mouseover, "imlP700TR_Mouseover")

                .Add(imlP700BL_Selected, "imlP700BL_Selected")
                .Add(imlP700BR_Selected, "imlP700BR_Selected")
                .Add(imlP700TL_Selected, "imlP700TL_Selected")
                .Add(imlP700TR_Selected, "imlP700TR_Selected")

                'P20s
                .Add(imlP20BL_Normal, "imlP20BL_Normal")
                .Add(imlP20BR_Normal, "imlP20BR_Normal")
                .Add(imlP20TL_Normal, "imlP20TL_Normal")
                .Add(imlP20TR_Normal, "imlP20TR_Normal")

                .Add(imlP20BL_Mouseover, "imlP20BL_Mouseover")
                .Add(imlP20BR_Mouseover, "imlP20BR_Mouseover")
                .Add(imlP20TL_Mouseover, "imlP20TL_Mouseover")
                .Add(imlP20TR_Mouseover, "imlP20TR_Mouseover")

                .Add(imlP20BL_Selected, "imlP20BL_Selected")
                .Add(imlP20BR_Selected, "imlP20BR_Selected")
                .Add(imlP20TL_Selected, "imlP20TL_Selected")
                .Add(imlP20TR_Selected, "imlP20TR_Selected")

                'P25s
                .Add(imlP25BL_Normal, "imlP25BL_Normal")
                .Add(imlP25BR_Normal, "imlP25BR_Normal")
                .Add(imlP25TL_Normal, "imlP25TL_Normal")
                .Add(imlP25TR_Normal, "imlP25TR_Normal")

                .Add(imlP25BL_Mouseover, "imlP25BL_Mouseover")
                .Add(imlP25BR_Mouseover, "imlP25BR_Mouseover")
                .Add(imlP25TL_Mouseover, "imlP25TL_Mouseover")
                .Add(imlP25TR_Mouseover, "imlP25TR_Mouseover")

                .Add(imlP25BL_Selected, "imlP25BL_Selected")
                .Add(imlP25BR_Selected, "imlP25BR_Selected")
                .Add(imlP25TL_Selected, "imlP25TL_Selected")
                .Add(imlP25TR_Selected, "imlP25TR_Selected")

                'P500s
                .Add(imlP500B_Normal, "imlP500B_Normal")
                .Add(imlP500T_Normal, "imlP500T_Normal")

                .Add(imlP500B_Mouseover, "imlP500B_Mouseover")
                .Add(imlP500T_Mouseover, "imlP500T_Mouseover")

                .Add(imlP500B_Selected, "imlP500B_Selected")
                .Add(imlP500T_Selected, "imlP500T_Selected")

                'P200s
                .Add(imlP200BL_Normal, "imlP200BL_Normal")
                .Add(imlP200BR_Normal, "imlP200BR_Normal")
                .Add(imlP200TL_Normal, "imlP200TL_Normal")
                .Add(imlP200TR_Normal, "imlP200TR_Normal")

                .Add(imlP200BL_Mouseover, "imlP200BL_Mouseover")
                .Add(imlP200BR_Mouseover, "imlP200BR_Mouseover")
                .Add(imlP200TL_Mouseover, "imlP200TL_Mouseover")
                .Add(imlP200TR_Mouseover, "imlP200TR_Mouseover")

                .Add(imlP200BL_Selected, "imlP200BL_Selected")
                .Add(imlP200BR_Selected, "imlP200BR_Selected")
                .Add(imlP200TL_Selected, "imlP200TL_Selected")
                .Add(imlP200TR_Selected, "imlP200TR_Selected")

                'Switches, estops, disconnects, pendants
                .Add(imlDisc, "imlDisc")
                .Add(imlEstat, "imlEstat")
                .Add(imlEstop_Cstop, "imlEstop_Cstop")
                .Add(imlPendant, "imlPendant")
                .Add(imlSelSwitch, "imlSelSwitch")
                .Add(imlLed, "imlLed")
                .Add(imlLimitSwitches, "imlLimitSwitches")
                .Add(imlPEC, "imlPEC")
                .Add(imlProxSwitch, "imlProxSwitch")
                .Add(imlMaintDoor, "imlMaintDoor")
                .Add(imlMaintDoor, "imlMaint")
                .Add(imlEnabDev, "imlEnabDev")

            End With

            Progress = 98
            subEnableControls(True)

            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound

            mbScreenLoaded = True
            If cboZone.Items.Count = 1 Then
                'this selects the zone in cases of just one zone. fires event to call subchangezone
                cboZone.Text = cboZone.Items(0).ToString
            Else
                Status(True) = gcsRM.GetString("csSELECT_ZONE")
            End If



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
        Dim sSeparators As String = " "
        Dim sCommands As String = Microsoft.VisualBasic.Command()
        Dim sArgs() As String = sCommands.Split(sSeparators.ToCharArray)

    End Sub

    Private Sub subSizeUserControl(ByVal uControl As UserControl)

        '********************************************************************************************
        'Description:  size and show one of our "subforms"
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim pt As Point
        pt.X = 0
        pt.Y = 0
        uControl.Location = pt
        Dim sz As Size = tscMain.ContentPanel.Size
        uControl.Size = sz
        uControl.Anchor = AnchorStyles.Bottom Or AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        If Not (gUctlCurrentScreen Is Nothing) Then
            gUctlCurrentScreen.Hide()
        End If
        gBSDFormCurrentScreen = DirectCast(uControl, BSDForm)
        gUctlCurrentScreen = uControl
        With gBSDFormCurrentScreen
            Select Case uControl.Name
                Case Else
                    .Show(msZoneData)
            End Select
        End With
        gBSDFormCurrentScreen.InitPLCData()
        If Not (msZoneData Is Nothing) Then
            gBSDFormCurrentScreen.LinkIndex = ePLCLink.Zone
            gBSDFormCurrentScreen.PLCData = msZoneData
        End If
        If Not (msQueueData Is Nothing) Then
            gBSDFormCurrentScreen.LinkIndex = ePLCLink.Queue
            gBSDFormCurrentScreen.PLCData = msQueueData
        End If
        If Not (msRobotData Is Nothing) Then
            gBSDFormCurrentScreen.LinkIndex = ePLCLink.Robot
            gBSDFormCurrentScreen.PLCData = msRobotData
        End If

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
            mPWCommon.SaveToChangeLog(colZones.ActiveZone)


            mPWCommon.subShowChangeLog(colZones.CurrentZone, gsChangeLogArea, msSCREEN_NAME, gcsRM.GetString("csALL"), _
                                       gcsRM.GetString("csALL"), mDeclares.eParamType.None, nIndex, False, False, oIPC)
        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR") & " - frmMain.subShowChangeLog", ex, msSCREEN_NAME, _
                Status, MessageBoxButtons.OK)
                
        End Try

    End Sub
    Friend Function GetCachedData(ByVal LinkIndex As ePLCLink) As String()
        '********************************************************************************************
        'Description: Return the last data from the hotlink 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Select Case LinkIndex
            Case ePLCLink.None
                Return Nothing
            Case ePLCLink.Queue
                msQueueData = CType(MergeArrays(msQueueTemp1, msQueueTemp2), String())
                Return msQueueData
            Case ePLCLink.Robot
                msRobotData = CType(MergeArrays(msRobotTemp1, msRobotTemp2), String())
                Return msRobotData
            Case ePLCLink.Zone
                Return msZoneData
            Case ePLCLink.Device
                Return msDevData
        End Select
        Return Nothing
    End Function
#End Region
#Region " Events "
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
        '********************************************************************************************
        Dim sKeyValue As String = String.Empty

        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode
                Case Keys.F1
                    'Help Screen request
                    sKeyValue = "btnHelp"
                    subLaunchHelp(gs_HELP_VIEW_BSD, oIPC)

                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty
                    Dim sScreenSubName As String = String.Empty
                    Select Case meCurrentScreen
                        Case eBSDScreenSelect.Process
                            sScreenSubName = "_Process"
                        Case eBSDScreenSelect.Queue
                            sScreenSubName = "_Queue"
                        Case eBSDScreenSelect.Booth
                            sScreenSubName = "_Booth"
                    End Select
                    sScreenSubName = sScreenSubName & ".jpg "
                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath & msSCREEN_DUMP_NAME & sScreenSubName, Imaging.ImageFormat.Jpeg)

                Case Keys.Escape
                    Me.Close()

                Case Else

            End Select
        End If

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
        '********************************************************************************************

        Try
            'Stop Scattered Access
            Dim sMessage(1) As String
            sMessage(0) = msSCREEN_NAME & ";OFF"
            sMessage(1) = "ALL"
            oIPC.WriteControlMsg(gs_COM_ID_SA, sMessage)

            'Shut down any scattered access connections that are updating
            If Not (colControllers Is Nothing) Then
                'For Each Controller As clsController In colControllers
                '    If Controller IsNot Nothing AndAlso Controller.Robot IsNot Nothing Then
                '        If Controller.Robot.IsConnected Then
                '            If oScatteredAccess.IsSetUp Then
                '                oScatteredAccess.UpdateActive(Controller.Name) = False
                '            End If 'oSA.IsSetUp
                '        End If
                '    End If
                'Next
                If colControllers.Count > 0 Then
                    For i As Integer = colControllers.Count - 1 To 0
                        colControllers.Remove(colControllers(i))
                    Next
                End If
            End If

            If colArms Is Nothing Then Exit Sub
            For i As Integer = colArms.Count - 1 To 0
                colArms.Remove(colArms(i))
            Next
        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR") & " - frmMain.frmMain_Closing", ex, msSCREEN_NAME, _
                Status, MessageBoxButtons.OK)

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


    End Sub

    ' 03/20/14  MSW     ZDT Changes
    Private Sub subUpdateZDT(ByRef sText As String, ByRef sColor As String) Handles oZDT.Update
        '********************************************************************************************
        'Description:  ZDT class found updates.
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/05/14  MSW     ZDT Changes
        '********************************************************************************************
        mBSDCommon.subUpdateZDT(sText, sColor)
        oZDT.subDisplayGrid(frmMiniGrid.dgvMini, frmMiniGrid.mnuResetStatus)
    End Sub

    ' 03/20/14  MSW     ZDT Changes
    Private Sub subOVCChanged() Handles oOVC.Changed
        '********************************************************************************************
        'Description:  OVC class found updates.
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/19/14  MSW     OVC Changes
        '********************************************************************************************
        If gBSDFormCurrentScreen IsNot Nothing Then
            'Force update
            Dim nIdx As Integer = eRobot.Spare
            Do Until nIdx > msRobotData.GetUpperBound(0)
                If msRobotData(nIdx) = "0" Then
                    msRobotData(nIdx) = "1"
                Else
                    msRobotData(nIdx) = "0"
                End If
                nIdx += eRobot.WordsPerRobot
            Loop
            gBSDFormCurrentScreen.LinkIndex = ePLCLink.Robot
            gBSDFormCurrentScreen.PLCData = msRobotData
        End If
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
        ' 10/16/12  MSW     Invoke doesn't work anymore
        '*******************************************************************************************

        ''Check for call from the robot object thread
        'If Me.stsStatus.InvokeRequired Then
        '    Dim dCntrStat As New ControllerStatusChange(AddressOf colControllers_ConnectionStatusChange)
        '    Me.Invoke(dCntrStat, New Object() {Controller})
        'Else
        Control.CheckForIllegalCrossThreadCalls = False
        Call subDoStatusBar(Controller)
        Control.CheckForIllegalCrossThreadCalls = True
        'End If

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

    End Sub
    Private Sub tlsMain_ItemClicked(ByVal sender As Object, _
                            ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) _
                            Handles tlsMain.ItemClicked
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
        Select Case e.ClickedItem.Name
            Case "btnClose"
                'Check to see if we need to save is performed in  bAskForSave
                Me.Close()
                'End
            Case "btnStatus"
                lstStatus.Visible = Not lstStatus.Visible


            Case "btnChangeLog"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                subShowChangeLog(CType(eChangeMnu.Hours24, Integer))

            Case Else
                subToolStripButtonClicked(e.ClickedItem.Name) ' case sensitive

        End Select




    End Sub
    Private Sub mnuLast7_Click(ByVal sender As Object, _
                                                ByVal e As System.EventArgs) _
                                                Handles mnuLast7.Click
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
                                            ByVal e As System.EventArgs) _
                                            Handles mnuAllChanges.Click
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
                                            ByVal e As System.EventArgs) _
                                            Handles mnuLast24.Click
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
        Dim cachePrivilege As ePrivilege
        Dim bRem As Boolean = False
        Dim bAllow As Boolean = False


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

            Privilege = ePrivilege.Remote

            If Privilege = ePrivilege.Remote Then
                'no remote at this time
                'If colZones.PaintShopComputer = False Then
                '    bAllow = True
                'End If
            End If


            Privilege = cachePrivilege

        Else
            Privilege = ePrivilege.None
        End If

        If colZones.PaintShopComputer = False Then
            bRem = mbRemoteZone
        End If

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
        mbEventBlocker = True
        If SetRemoteServer() Then
            mbRemoteZone = True
            moPassword = Nothing

            tmrSA.Enabled = False
            DoEvents()
            'oScatteredAccess = Nothing
            'Stop Scattered Access
            Dim sMessage(1) As String
            sMessage(0) = msSCREEN_NAME & ";OFF"
            sMessage(1) = "ALL"
            oIPC.WriteControlMsg(gs_COM_ID_SA, sMessage)

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
    Private Sub mnuLocal_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                            Handles mnuLocal.Click
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
    Private Sub mPLC_NewData(ByVal ZoneName As String, ByVal TagName As String, _
                                                    ByVal Data() As String) Handles mPLC.NewData
        '********************************************************************************************
        'Description:  New Hotlink Data
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sPrefix As String = "Z" & colZones.CurrentZoneNumber.ToString

        If Data Is Nothing Then Exit Sub
        If gBSDFormCurrentScreen Is Nothing Then Exit Sub

        Dim i As Integer = 0
        Dim nUB As Integer = 0

        Select Case TagName
            Case sPrefix & "BoothData"
                gBSDFormCurrentScreen.LinkIndex = ePLCLink.Zone
                gBSDFormCurrentScreen.PLCData = Data
                nUB = Data.GetUpperBound(0)
                ReDim msZoneData(nUB)
                For i = 0 To nUB
                    msZoneData(i) = Data(i)
                Next
                If frmGhostStatus.Visible Then
                    frmGhostStatus.UpdateHotlinkData(msZoneData)
                End If
            Case sPrefix & "RobotData", sPrefix & "RobotData2"
                If TagName = sPrefix & "RobotData" Then
                    msRobotTemp1 = Data
                Else
                    msRobotTemp2 = Data
                End If
                msRobotData = CType(MergeArrays(msRobotTemp1, msRobotTemp2), String())
                gBSDFormCurrentScreen.LinkIndex = ePLCLink.Robot
                gBSDFormCurrentScreen.PLCData = msRobotData
            Case sPrefix & "BoothQueue1", sPrefix & "BoothQueue2"
                If TagName = sPrefix & "BoothQueue1" Then
                    msQueueTemp1 = Data
                Else
                    msQueueTemp2 = Data
                End If
                msQueueData = CType(MergeArrays(msQueueTemp1, msQueueTemp2), String())
                gBSDFormCurrentScreen.LinkIndex = ePLCLink.Queue
                gBSDFormCurrentScreen.PLCData = msQueueData
            Case sPrefix & "ConveyorBingoBoard"
                nUB = Data.GetUpperBound(0)
                ReDim msDevData(nUB)
                For i = 0 To nUB
                    msDevData(i) = Data(i)
                Next
                If frmDevices.Visible Then
                    frmDevices.UpdateHotlinkData(msDevData)
                End If
        End Select



    End Sub
    Private Sub tmrSA_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrSA.Tick
        '********************************************************************************************
        'Description:  read scattered access data
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/24/10  MSW     handle different scattered access lists - force resize to smaller list, too
        ' 03/17/11  MSW     put a 10 second delay on the errors, so they'll know there's as problem, but 
        '                   there is time to shut down the screen between messages.
        ' 10/16/12  MSW     Clean up scattered access after disconnect
        '********************************************************************************************

        If gBSDFormCurrentScreen Is Nothing Then Exit Sub
        If gSAConfig Is Nothing Then Exit Sub
        Dim bSelected As Boolean
        Dim iCntrIndx As Integer
        Dim bException As Boolean = False
        Dim sErr As String = String.Empty
        Try
            Dim sTmpEnable As String = ScatteredAccessEnableString
            If msSAEnableString <> sTmpEnable Then
                'set enables
                msSAEnableString = sTmpEnable
                Dim sMessage(1) As String
                sMessage(0) = msSCREEN_NAME & ";SET"
                sMessage(1) = msSAEnableString
                oIPC.WriteControlMsg(gs_COM_ID_SA, sMessage)
            End If

            tmrSA.Enabled = False
            tmrSA.Interval = 500
            For Each oController As clsController In colControllers
                bException = False
                iCntrIndx = colControllers.IndexOf(oController)
                bSelected = ScatteredAccessEnableController(iCntrIndx)
                If bSelected Then
                    If (oController.Robot.IsConnected = False) Then
                        bSelected = False
                        If mbOfflineMessage(iCntrIndx) Then
                            Status = oController.Name & gcsRM.GetString("csIS_OFFLINE")
                            lstStatus.Visible = True
                            MessageBox.Show((oController.Name & gcsRM.GetString("csIS_OFFLINE")), _
                               (msSCREEN_NAME & " - " & oController.Name), _
                               MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            mbOfflineMessage(iCntrIndx) = False
                            'Clean up the labels
                            For Each Arm As clsArm In oController.Arms
                                gBSDFormCurrentScreen.subCleanUpRobotLabels(Arm)
                            Next
                        End If
                    Else
                        If mbOfflineMessage(iCntrIndx) = False Then
                            mbOfflineMessage(iCntrIndx) = True
                            Status = oController.Name & gcsRM.GetString("csIS_ONLINE")
                            'It's about to get slow, so try to warn them
                            ' or it could already be slow, hopefully this'll pop up before they get too mad.
                            lstStatus.Visible = True
                            Application.DoEvents()
                        End If
                    End If
                End If
                If bSelected Then
                    For Each oArm As clsArm In oController.Arms
                        If ScatteredAccessEnableArm(oArm.RobotNumber - 1) Then
                            Dim oSAItems As clsScatteredAccessItems = DirectCast(oArm.Tag, clsScatteredAccessItems)

                            If oSAItems Is Nothing Then
                                oSAItems = New clsScatteredAccessItems(gSAConfig, oArm)
                                oArm.Tag = oSAItems
                            End If
                            If oSAItems IsNot Nothing Then
                                If oController.Robot.IsConnected = False Then
                                    oArm.Tag = Nothing
                                Else
                                    msSAData = oSAItems.Data
                                    gBSDFormCurrentScreen.ScatteredAccessData = msSAData
                                    'Get the indexes for all the data first time in on each robot
                                    If bIndexScatteredAccess(colArms.IndexOf(oArm)) Then
                                        mPWRobotCommon.subIndexScatteredAccess(msSAData, oArm.ArmNumber, _
                                            gtScatteredAccessIndexes(colArms.IndexOf(oArm)).Indexes)
                                        bIndexScatteredAccess(colArms.IndexOf(oArm)) = False
                                    End If
                                    'Update for each arm if required
                                    gBSDFormCurrentScreen.RobotIndex = colArms.IndexOf(oArm)
                                    Try
                                        gBSDFormCurrentScreen.subUpdateSAData()
                                    Catch ex As Exception
                                        'Clean up scattered access after disconnect
                                        oArm.Tag = Nothing
                                    End Try
                                End If
                            Else
                                bException = True
                                If sErr = String.Empty Then
                                    sErr = oArm.Name
                                Else
                                    sErr = sErr & ", " & oArm.Name
                                End If
                            End If
                        End If
                    Next
                End If
            Next
        Catch ex As Exception
            bException = True
            If (Not mbSAException(iCntrIndx)) Then
                ShowErrorMessagebox(gcsRM.GetString("csERROR") & " - frmMain.tmrSA_Tick", ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
                tmrSA.Enabled = False
            End If
        Finally
            If bException And (Not mbSAException(iCntrIndx)) Then
                WriteEventToLog(msSCREEN_NAME, gcsRM.GetString("csERROR") & " - frmMain.tmrSA_Tick " & sErr)

                MessageBox.Show(gcsRM.GetString("csERROR") & " - frmMain.tmrSA_Tick " & sErr, msSCREEN_NAME, MessageBoxButtons.OK, _
                                MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, _
                                MessageBoxOptions.DefaultDesktopOnly)
                tmrSA.Enabled = False
            End If
            mbSAException(iCntrIndx) = bException
            tmrSA.Enabled = True
        End Try

    End Sub
    Private Sub btnProcess_DropDownItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles btnProcess.DropDownItemClicked
        '********************************************************************************************
        'Description:  robot selected from dropdown, start the process screen
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Clear out the other checkboxes
        btnBooth.CheckState = CheckState.Unchecked
        btnQueue.CheckState = CheckState.Unchecked
        btnGhost.CheckState = CheckState.Unchecked
        btnConv.CheckState = CheckState.Unchecked
        'Check if the control needs to be created
        If (mUctlProcess Is Nothing) Then
            mUctlProcess = New uctlProcessScreen
            tscMain.ContentPanel.Controls.Add(mUctlProcess)
        End If
        'Check if it needs an init
        If mbInitScreen(eBSDScreenSelect.Process) Then
            mUctlProcess.Initialize(e.ClickedItem.Text)
            mbInitScreen(eBSDScreenSelect.Process) = False
        Else
            mUctlProcess.SelectedRobot = e.ClickedItem.Text
        End If
        meCurrentScreen = eBSDScreenSelect.Process
        'Show the control
        subSizeUserControl(mUctlProcess)

    End Sub
    Friend Sub subLaunchProcessScreen(ByVal srobot As String)
        '********************************************************************************************
        'Description:  Select the process screen from another
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        btnBooth.CheckState = CheckState.Unchecked
        btnQueue.CheckState = CheckState.Unchecked
        btnGhost.CheckState = CheckState.Unchecked
        btnConv.CheckState = CheckState.Unchecked
        If (mUctlProcess Is Nothing) Then
            mUctlProcess = New uctlProcessScreen
            tscMain.ContentPanel.Controls.Add(mUctlProcess)
        End If
        If mbInitScreen(eBSDScreenSelect.Process) Then
            mUctlProcess.Initialize(srobot)
            mbInitScreen(eBSDScreenSelect.Process) = False
        Else
            mUctlProcess.SelectedRobot = srobot
        End If
        meCurrentScreen = eBSDScreenSelect.Process
        subSizeUserControl(mUctlProcess)

    End Sub
    Friend Sub subLaunchLifeCyclesScreen(ByVal Robot As String)
        '********************************************************************************************
        'Description:  Launch the Life Cycles screen from the Robot Context menus
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        btnBooth.CheckState = CheckState.Unchecked
        btnQueue.CheckState = CheckState.Unchecked
        btnGhost.CheckState = CheckState.Unchecked
        btnConv.CheckState = CheckState.Unchecked

        If (mUctlLifeCycles Is Nothing) Then
            mUctlLifeCycles = New uctlLifeCycles
            tscMain.ContentPanel.Controls.Add(mUctlLifeCycles)
        End If
        If mbInitScreen(eBSDScreenSelect.LifeCycles) Then
            mUctlLifeCycles.Initialize(Robot)
            mbInitScreen(eBSDScreenSelect.LifeCycles) = False
        Else
            mUctlLifeCycles.SelectedRobot = Robot
        End If
        meCurrentScreen = eBSDScreenSelect.LifeCycles
        subSizeUserControl(mUctlLifeCycles)

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
            Privilege = ePrivilege.Edit

            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/23/12
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

    Private Sub mnuResetJobCount_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                            Handles mnuResetJobCount.Click
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
        Try

            mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "ResetJobCount"
            Dim s(0) As String
            s(0) = "1"
            mPLC.PLCData = s

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR") & " - frmMain.mnuResetJobCount_Click", ex, msSCREEN_NAME, _
                Status, MessageBoxButtons.OK)
        End Try

    End Sub

    Private Sub mnuLampTest_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuLampTest.Click
        '********************************************************************************************
        'Description: lamp test drop-down menu
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "LampTest"
            Dim s(0) As String
            s(0) = "1"
            mPLC.PLCData = s

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR") & " - frmMain.mnuLampTest_Click", ex, msSCREEN_NAME, _
                Status, MessageBoxButtons.OK)
        End Try
    End Sub

    Private Sub tscMain_ContentPanel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tscMain.ContentPanel.Load

    End Sub


    Private Sub btnUtil_DropDownItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles btnUtil.DropDownItemClicked

    End Sub

    Private Sub mnuEstatTest_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuEstatTest.Click
        '********************************************************************************************
        'Description: Estat Test drop-down menu 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        frmEstatTest.Show()

    End Sub
End Class