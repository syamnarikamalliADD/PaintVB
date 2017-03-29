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
' Description: Network Maintenance
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
'    09/22/2009 MSW     first draft
'    11/11/09   MSW     subEnableControls - revise enables for command buttons
'    04/22/10   MSW     mnuSelectAll_Click, mnuUnselectAll_Click - add call to subEnableControls
'    11/24/10   MSW     Allow access to tftp and bootp config files
'    09/14/11   MSW     Assemble a standard version of everything                       4.1.0.0
'    09/30/11   MSW     Add bootp and tftp utilities                                    4.1.0.1
'    12/02/11   MSW     Add status bar robot update, DMONCfg placeholder reference      4.1.1.0
'    01/18/12   MSW     Clean out old printsettings object                              4.1.1.1
'    01/24/12   MSW     Placeholder include updates                                   4.01.01.02
'    02/15/12   MSW     Print handling updates and force 32 bit build on 64 bit systems   4.01.01.03
'    03/21/12   RJO     Modified to use .NET Password and IPC                           4.1.2.0
'    03/28/12   MSW     subInitializeForm - Clean up separators when hiding image backup 4.01.03.00
'    04/04/12   MSW     Manage Bootp setup better.  don't depend on the batch file       4.01.03.00
'    06/07/12   MSW     subSaveData - Add waitcursor for delays                          4.01.03.01
'    10/24/12   RJO     Added StartupModule to project to prevent multiple instances     4.01.03.02
'    04/16/13   MSW     Add Canadian language files                                      4.01.05.00
'                       Standalone Changelog 
'    07/09/13   MSW     Update and standardize logos                                     4.01.05.01
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up   4.01.05.02
'    09/30/13   MSW     Save screenshots as jpegs                                        4.01.05.03
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call                 4.01.07.00
'    05/16/14   MSW     Fix ID# in BOOTP autoconfig                                      4.01.07.01
'    07/24/14   RJO     Robot Name must be the HostName in BOOTP autoconfig              4.01.07.02
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Configuration.ConfigurationSettings
Imports Response = System.Windows.Forms.DialogResult
Imports ConnStat = FRRobotNeighborhood.FRERNConnectionStatusConstants
Imports FRRobot
Imports FRRobotNeighborhood


Friend Class frmMain
#Region " Declares "

    '******** Form Constants   **********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    'Not a constant here, we'll try to get help in the same executable
    Friend Const msSCREEN_NAME As String = "Network"   ' <-- For password area change log etc.
    Private Const msSCREEN_DUMP_NAME As String = "Maint_Network_Main.jpg"
    Private Const mnMAININX As Integer = 100
    Private Const mnSCREENINX As Integer = 101
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"

    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    Friend Shared gsChangeLogArea As String = msSCREEN_NAME
    Private msOldZone As String = String.Empty
    Private colZones As clsZones = Nothing
    Private WithEvents colControllers As clsControllers = Nothing
    Private WithEvents colArms As clsArms = Nothing
    Private mbEventBlocker As Boolean = False
    Private mbRemoteZone As Boolean = False
    Private mbScreenLoaded As Boolean = False
    Private msLastPage As String = String.Empty
    Private Const msHOSTS_LOCATION As String = "%SYSTEMROOT%\system32\drivers\etc\hosts"
    Private msECBR_LOCATION As String = String.Empty
    Private Const msTFTP_CFG As String = "tftp.cfg"
    Private Const msBOOTP_CFG As String = "bootp.cfg"
    Private msHostFileName As String = String.Empty
    Private Const msLAUNCH_COMMAND As String = "cmd.exe /A  /C "
    Private Const msOUTPUT_FILE As String = "NWMNTout"
    Private Const msTXT_EXT As String = ".TXT"
    Private Const msCMD_PING As String = "ping "
    Private Const msCMD_ARP_ALL As String = "arp -a"
    Private Const msCMD_ARP_ONE As String = "arp -a "
    Private Const msCMD_TRACEROUTE As String = "tracert "
    Private Const msCMD_IPCONFIG As String = "ipconfig /all"
    Private Const msCMD_ECBRUPDATE As String = "ECBR_Register.bat"
    Private Const msCMD_BOOTP_INSTALL As String = "bootp.exe"
    Private Const msCMD_TFTP_INSTALL As String = "tftp.exe"
    Private Const msTFTP_STATUS As String = "sc query " & Chr(34) & "FANUC Robotics TFTP Server" & Chr(34)
    Private Const msBOOTP_STATUS As String = "sc query " & Chr(34) & "FANUC Robotics BOOTP Server" & Chr(34)
    Private Const msTFTP_START As String = "sc start " & Chr(34) & "FANUC Robotics TFTP Server" & Chr(34)
    Private Const msBOOTP_START As String = "sc start " & Chr(34) & "FANUC Robotics BOOTP Server" & Chr(34)
    Private Const msTFTP_STOP As String = "sc stop " & Chr(34) & "FANUC Robotics TFTP Server" & Chr(34)
    Private Const msBOOTP_STOP As String = "sc stop " & Chr(34) & "FANUC Robotics BOOTP Server" & Chr(34)
    Private mbCancel As Boolean = False
    Private nTmpFileIndex As Integer = 0 'In case there are access problems, add to the file name
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean = False
    Private mnProgress As Integer = 0
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    Private msCulture As String = "en-US" 'Default to english
    '******** End Property Variables    *************************************************************

    '******** This is the old pw3 password object interop  *****************************************
    'Friend WithEvents moPassword As PWPassword.PasswordObject 'RJO 03/21/12
    'Friend moPrivilege As PWPassword.cPrivilegeObject 'RJO 03/21/12
    '******** This is the old pw3 password object interop  *****************************************

    Friend WithEvents moPassword As clsPWUser  'RJO 03/21/12
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm  'RJO 03/21/12

    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    Delegate Sub ControllerStatusChange(ByVal Controller As clsController)
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet)  'RJO 03/21/12
    Private mnMaxProgress As Integer = 100
    Private mPrintHtml As clsPrintHtml
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
        ' 11/11/09  MSW     revise enables for command buttons
        ' 10/07/10  MSW	    Print function clean up
        '********************************************************************************************
        Dim bRestOfControls As Boolean = False
        Dim bEditsMade As Boolean = False
        btnClose.Enabled = True

        If bEnable = False Then
            tlsList.Enabled = False
            tlsHosts.Enabled = False
            tlsCmdsTop.Enabled = False
            tlsCmdsLeft.Enabled = False
            btnChangeLog.Enabled = False
            btnBootP.Enabled = False
            btnTFTP.Enabled = False
            btnHosts.Enabled = False
            btnAutoConfig.Enabled = False
            bRestOfControls = False
            tlsbECBRUpdate.Enabled = False
            tlsbBOOTPStatus.Enabled = False
            tlsbBOOTPStart.Enabled = False
            tlsbBOOTPStop.Enabled = False
            tlsbTFTPStatus.Enabled = False
            tlsbTFTPStart.Enabled = False
            tlsbTFTPStop.Enabled = False
        Else
            Select Case Privilege
                Case ePrivilege.None
                    tlsList.Enabled = True
                    tlsHosts.Enabled = True
                    tlsCmdsTop.Enabled = True
                    tlsCmdsLeft.Enabled = True
                    btnChangeLog.Enabled = True
                    bRestOfControls = False
                    mnuPrintFileHosts.Enabled = False
                    mnuPrintFileCmds.Enabled = False
                    btnBootP.Enabled = True
                    btnTFTP.Enabled = True
                    btnHosts.Enabled = True
                    btnAutoConfig.Enabled = False
                    tlsbECBRUpdate.Enabled = False
                    tlsbBOOTPStatus.Enabled = True
                    tlsbBOOTPStart.Enabled = False
                    tlsbBOOTPStop.Enabled = False
                    tlsbTFTPStatus.Enabled = True
                    tlsbTFTPStart.Enabled = False
                    tlsbTFTPStop.Enabled = False
                Case Else
                    tlsList.Enabled = True
                    tlsHosts.Enabled = True
                    tlsCmdsTop.Enabled = True
                    tlsCmdsLeft.Enabled = True
                    btnChangeLog.Enabled = True
                    bRestOfControls = True
                    mnuPrintFileHosts.Enabled = True
                    mnuPrintFileCmds.Enabled = True
                    btnBootP.Enabled = True
                    btnTFTP.Enabled = True
                    btnHosts.Enabled = True
                    btnAutoConfig.Enabled = True
                    tlsbECBRUpdate.Enabled = True
                    tlsbBOOTPStatus.Enabled = True
                    tlsbBOOTPStart.Enabled = True
                    tlsbBOOTPStop.Enabled = True
                    tlsbTFTPStatus.Enabled = True
                    tlsbTFTPStart.Enabled = True
                    tlsbTFTPStop.Enabled = True
            End Select
        End If
        'Always leave print enabled
        tlspPrintCmds.Enabled = True
        tlspPrintHosts.Enabled = True

        tlspSaveCmds.Enabled = bRestOfControls And rtbCmds.Modified
        tlspSaveHosts.Enabled = bRestOfControls 'And rtbHosts.Modified 'Allow save of a copy without changes
        'Enable cut copy when there's an active selection
        Dim bCmdSel As Boolean = rtbCmds.SelectionLength > 0
        Dim bHostSel As Boolean = rtbHosts.SelectionLength > 0
        tlspCutCmds.Enabled = bRestOfControls And bCmdSel
        tlspCutHosts.Enabled = bRestOfControls And bHostSel
        tlspCopyCmds.Enabled = bRestOfControls And bCmdSel
        tlspCopyHosts.Enabled = bRestOfControls And bHostSel
        'No event for this, so it'll just stay enabled
        'Dim bPaste As Boolean = Clipboard.ContainsText
        tlspPasteCmds.Enabled = bRestOfControls 'And bPaste
        tlspPasteHosts.Enabled = bRestOfControls 'And bPaste

        'Find out if there are any addresses selected for the comands
        'MSW 11/11/09 - revise enables for command buttons
        Dim bAddressesSelected As Boolean = (chkSelectAddresses.CheckedItems.Count > 0) Or _
           (txtAddress.Text <> String.Empty)
        tlsbPing.Enabled = bAddressesSelected
        tlsbArp.Enabled = True
        tlsbTraceRoute.Enabled = bAddressesSelected
        tlsbIPConfig.Enabled = True
        'see if a robot is selected
        If bAddressesSelected Then
            bAddressesSelected = False
            Dim sTmp As String = String.Empty
            For Each sDevice As String In chkSelectAddresses.CheckedItems
                sTmp = sDevice.ToLower
                For Each oRobot As clsController In colControllers
                    If (sTmp = oRobot.FanucName.ToLower) Or (sTmp = oRobot.Name.ToLower) Or (sTmp = oRobot.IPAddress.ToLower) Then
                        bAddressesSelected = True
                        Exit For
                    End If
                Next
                If bAddressesSelected Then
                    Exit For
                End If
            Next
            If (txtAddress.Text <> String.Empty) And (bAddressesSelected = False) Then
                sTmp = txtAddress.Text.ToLower
                For Each oRobot As clsController In colControllers
                    If (sTmp = oRobot.FanucName.ToLower) Or (sTmp = oRobot.Name.ToLower) Or (sTmp = oRobot.IPAddress.ToLower) Then
                        bAddressesSelected = True
                        Exit For
                    End If
                Next
            End If
        End If
        tlsbFanucPing.Enabled = bAddressesSelected
    End Sub
    Private Sub subFormatScreenLayout()
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
        ' 10/07/10  MSW	    Print function clean up
        '********************************************************************************************

        With gpsRM
            tlsbControllers.Text = .GetString("psCONTROLLERS")
            tlsbControllers.ToolTipText = .GetString("psCONTROLLERS")
            tlsbHostFileItems.Text = .GetString("psHOSTFILEITEMS")
            tlsbHostFileItems.ToolTipText = .GetString("psHOSTFILEITEMS")
            tlsbFanucPing.Text = .GetString("psFANUC_PING")
            tlsbFanucPing.ToolTipText = .GetString("psFANUC_PING")
            tlsbPing.Text = .GetString("psPING")
            tlsbPing.ToolTipText = .GetString("psPING")
            tlsbArp.Text = .GetString("psARP")
            tlsbArp.ToolTipText = .GetString("psARP")
            tlsbTraceRoute.Text = .GetString("psTRACE_ROUTE")
            tlsbTraceRoute.ToolTipText = .GetString("psTRACE_ROUTE")
            tlsbIPConfig.Text = .GetString("psIP_CONFIG")
            tlsbIPConfig.ToolTipText = .GetString("psIP_CONFIG")
            tlsbECBRUpdate.Text = .GetString("psECBR_UPDATE")
            tlsbECBRUpdate.ToolTipText = .GetString("psECBR_UPDATE_SVR_CFG")
            tlsbTFTPStatus.Text = .GetString("psTFTP_STATUS")
            tlsbTFTPStatus.ToolTipText = .GetString("psTFTP_STATUS")
            tlsbBOOTPStatus.Text = .GetString("psBOOTP_STATUS")
            tlsbBOOTPStatus.ToolTipText = .GetString("psBOOTP_STATUS")
            tlsbTFTPStart.Text = .GetString("psTFTP_START")
            tlsbTFTPStart.ToolTipText = .GetString("psTFTP_START")
            tlsbBOOTPStart.Text = .GetString("psBOOTP_START")
            tlsbBOOTPStart.ToolTipText = .GetString("psBOOTP_START")
            tlsbTFTPStop.Text = .GetString("psTFTP_STOP")
            tlsbTFTPStop.ToolTipText = .GetString("psTFTP_STOP")
            tlsbBOOTPStop.Text = .GetString("psBOOTP_STOP")
            tlsbBOOTPStop.ToolTipText = .GetString("psBOOTP_STOP")

            lblManualAddress.Text = .GetString("psMANUAL_ADDRESS")

            tlspSaveCmds.ToolTipText = .GetString("psSAVE_DOCUMENT")
            tlspPrintCmds.ToolTipText = .GetString("psPRINT_DOCUMENT")
            tlspCutCmds.ToolTipText = .GetString("psCUT_TO_CLIPBOARD")
            tlspCopyCmds.ToolTipText = .GetString("psCOPY_TO_CLIPBOARD")
            tlspPasteCmds.ToolTipText = .GetString("psPASTE_FROM_CLIPBOARD")
            tlspClearCmds.ToolTipText = .GetString("psCLEAR_OUTPUT_WINDOW")
            tlspSaveHosts.ToolTipText = .GetString("psSAVE_DOCUMENT")
            tlspPrintHosts.ToolTipText = .GetString("psPRINT_DOCUMENT")
            tlspCutHosts.ToolTipText = .GetString("psCUT_TO_CLIPBOARD")
            tlspCopyHosts.ToolTipText = .GetString("psCOPY_TO_CLIPBOARD")
            tlspPasteHosts.ToolTipText = .GetString("psPASTE_FROM_CLIPBOARD")
            tlsLblCmds.Text = .GetString("psOUTPUT_WINDOW")
            btnHosts.Text = .GetString("psHOST_FILE")
            btnTFTP.Text = .GetString("psTFTP_CFG")
            btnBootP.Text = .GetString("psBOOTP_CFG")
            btnAutoConfig.Text = .GetString("psAUTO_CFG")
        End With
        btnCancel.Text = gcsRM.GetString("csCANCEL")
        mnuSelectAll.Text = gcsRM.GetString("csSELECT_ALL")
        mnuUnselectAll.Text = gcsRM.GetString("csUNSELECT_ALL")
        mnuPrintCmds.Text = gcsRM.GetString("csPRINT")
        MnuPrintHosts.Text = gcsRM.GetString("csPRINT")
        MnuPreviewCmds.Text = gcsRM.GetString("csPRINT_PREVIEW")
        MnuPreviewHosts.Text = gcsRM.GetString("csPRINT_PREVIEW")
        MnuPageSetupCmds.Text = gcsRM.GetString("csPAGE_SETUP")
        mnuPageSetupHosts.Text = gcsRM.GetString("csPAGE_SETUP")
        mnuPrintFileCmds.Text = gcsRM.GetString("csPRINT_FILE")
        mnuPrintFileHosts.Text = gcsRM.GetString("csPRINT_FILE")
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


        Try

            Status = gcsRM.GetString("csINITALIZING")
            Progress = 1

            DataLoaded = False
            mbScreenLoaded = False

            subProcessCommandLine()

            'init the old password for now 'RJO 03/21/12
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

            'init new IPC and new Password 'RJO 03/21/12
            oIPC = New Paintworks_IPC.clsInterProcessComm
            moPassword = New clsPWUser
            Progress = 50

            colZones = New clsZones(String.Empty)
            mScreenSetup.InitializeForm(Me)
            subInitFormText()
            Me.Show()
            colControllers = New clsControllers(colZones, True)
            SetUpStatusStrip(Me, colControllers)
            colControllers.RefreshConnectionStatus()
            subListControllers()
            Progress = 70

            mPrintHtml = New clsPrintHtml(msSCREEN_NAME)

            Progress = 98
            subEnableControls(True)

            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound


            'statusbar text
            Status(True) = gpsRM.GetString("psSELECT_PAGE")

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subClearScreen()

            subEnableControls(False)

            subEnableControls(True)

            Me.stsStatus.Refresh()

            ' refresh lock pic
            DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
            mbScreenLoaded = True

            btnBootP.Visible = GetDefaultFilePath(msECBR_LOCATION, mPWCommon.eDir.ECBR, String.Empty, msBOOTP_CFG)
            btnTFTP.Visible = GetDefaultFilePath(msECBR_LOCATION, mPWCommon.eDir.ECBR, String.Empty, msTFTP_CFG)
            tlsbECBRUpdate.Visible = GetDefaultFilePath(msECBR_LOCATION, mPWCommon.eDir.ECBR, String.Empty, msCMD_ECBRUPDATE)
            tlsbTFTPStatus.Visible = tlsbECBRUpdate.Visible
            tlsbBOOTPStatus.Visible = tlsbECBRUpdate.Visible
            tlsbTFTPStart.Visible = tlsbECBRUpdate.Visible
            tlsbBOOTPStart.Visible = tlsbECBRUpdate.Visible
            tlsbTFTPStop.Visible = tlsbECBRUpdate.Visible
            tlsbBOOTPStop.Visible = tlsbECBRUpdate.Visible
            'Clean up toolstrip separators when hiding image backup features
            ToolStripSeparator8.Visible = tlsbECBRUpdate.Visible
            ToolStripSeparator10.Visible = tlsbECBRUpdate.Visible
            ToolStripSeparator11.Visible = tlsbECBRUpdate.Visible
            ToolStripSeparator12.Visible = tlsbECBRUpdate.Visible
            ToolStripSeparator13.Visible = tlsbECBRUpdate.Visible
            ToolStripSeparator14.Visible = tlsbECBRUpdate.Visible
            ToolStripSeparator15.Visible = tlsbECBRUpdate.Visible
            
            'Load the host file text box
            msHostFileName = Environment.ExpandEnvironmentVariables(msHOSTS_LOCATION)
            rtbHosts.LoadFile(msHostFileName, RichTextBoxStreamType.PlainText)
            rtbHosts.Modified = False
            btnHosts.Checked = True
            btnTFTP.Checked = False
            btnBootP.Checked = False
            btnAutoConfig.Checked = False
            btnAutoConfig.Visible = False
            Application.DoEvents()

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
                Exit For
            End If
        Next

    End Sub
    Private Sub subSaveData(ByRef rtbTextBox As RichTextBox, Optional ByRef sPath As String = "")
        '********************************************************************************************
        'Description:  Data Save Routine
        '
        'Parameters: rich text box object to save
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/07/12  MSW     subSaveData - Add waitcursor for delays
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


        Try
            Dim o As New SaveFileDialog
            Dim sChangeLogText As String
            With o
                .Filter = gcsRM.GetString("csSAVE_TXT_RTF_ALL_FILTER")
                .Title = gcsRM.GetString("csSAVE_FILE_DLG_CAP")
                .CheckPathExists = True
                If sPath <> String.Empty Then
                    .FileName = sPath
                End If
                If rtbTextBox.Name = "rtbHosts" Then
                    sChangeLogText = gpsRM.GetString("psSAVED_") & msHostFileName
                    .AddExtension = False
                    .DefaultExt = ""
                    .FilterIndex = 3
                Else
                    sChangeLogText = gpsRM.GetString("psSAVED_OUTPUT")
                    .AddExtension = True
                    .DefaultExt = ".TXT"
                    .FilterIndex = 1
                End If
                .ShowDialog()
                sPath = .FileName
            End With
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            If sPath <> String.Empty Then
                sChangeLogText = sChangeLogText & sPath
                'There are a couple bold lines in there, so let them save it that way
                If sPath.Substring(sPath.Length - 4, 4).ToLower = ".rtf" Then
                    rtbTextBox.SaveFile(sPath, RichTextBoxStreamType.RichText)
                Else
                    rtbTextBox.SaveFile(sPath, RichTextBoxStreamType.PlainText)
                End If
                mPWCommon.AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                                          colZones.CurrentZoneNumber, _
                                          String.Empty, String.Empty, _
                                          sChangeLogText, colZones.CurrentZone)
                mPWCommon.SaveToChangeLog(colZones.ActiveZone)
                rtbTextBox.Modified = False
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally

            Me.Cursor = System.Windows.Forms.Cursors.Default

            subEnableControls(True)
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
        mbEventBlocker = True


        mbEventBlocker = False

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
            mPWCommon.subShowChangeLog(colZones.CurrentZone, gsChangeLogArea, msSCREEN_NAME, gcsRM.GetString("csALL"), _
                                          gcsRM.GetString("csALL"), mDeclares.eParamType.None, nIndex, False, False, oIPC)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
        End Try

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
                'End
            Case "btnChangeLog"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                subShowChangeLog(CType(eChangeMnu.Hours24, Integer))

            Case "btnCancel"
                mbCancel = True
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
        'The panel objects handle everything with this screen
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
                    subLaunchHelp(gs_HELP_MAINT_NET, oIPC)

                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty

                    Dim sName As String = msSCREEN_DUMP_NAME


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
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/21/12
        End If

    End Sub

#End Region

    Private Sub subListControllers()
        '********************************************************************************************
        'Description:  load controller names in listbox
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            chkSelectAddresses.Items.Clear()
            'Not using the common routine because it wants to format for a short and wide list
            'LoadRobotBoxFromCollection(chkSelectAddresses, colControllers, True)
            Dim sNames() As String
            ReDim sNames(colControllers.Count - 1)
            For i As Integer = 0 To colControllers.Count - 1
                chkSelectAddresses.Items.Add(colControllers.Item(i).FanucName)
            Next
        Catch ex As Exception
        End Try
        tlsbControllers.CheckState = CheckState.Checked
        tlsbHostFileItems.CheckState = CheckState.Unchecked
    End Sub
    Private Sub subListHostFileItems()
        '********************************************************************************************
        'Description:  load host file items in listbox
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        chkSelectAddresses.Items.Clear()
        Dim nChar As Integer = 0
        Dim bDone As Boolean = False
        Dim sName As String = String.Empty
        For nLine As Integer = 0 To rtbHosts.Lines.GetUpperBound(0)

            Dim sTmpArr As String() = Split(Replace(Trim(rtbHosts.Lines(nLine)), vbTab, " "))
            If sTmpArr.GetUpperBound(0) > 0 Then
                If sTmpArr.GetUpperBound(0) > 1 Then
                    For nSplit As Integer = 2 To sTmpArr.GetUpperBound(0)
                        If sTmpArr(nSplit).Length > 0 Then
                            sTmpArr(1) = sTmpArr(nSplit)
                            Exit For
                        End If
                    Next
                End If
                If sTmpArr(1).Length > 0 And sTmpArr(0).Substring(0, 1) <> "#" Then
                    chkSelectAddresses.Items.Add(sTmpArr(1))
                End If
            End If
        Next
        tlsbHostFileItems.CheckState = CheckState.Checked
        tlsbControllers.CheckState = CheckState.Unchecked

    End Sub

    Private Sub subRunTerminalProgram(ByVal sCmdName As String, Optional ByVal sDevice As String = "")
        '********************************************************************************************
        'Description:  run a terminal program, load the output into rtbCmds
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Const nTIMEOUT As Integer = 45000
        Const bWAIT As Boolean = True
        Dim nResult As Integer = 0
        Dim sShell As String
        Dim sTmpFile As String = String.Empty
        'Private Const msHOSTS_LOCATION As String = "%SYSTEMROOT%\system32\drivers\etc\hosts"
        'Private Const msTEMP_FOLDER As String = "%TEMP%"
        'Private msHostFileName As String = String.Empty
        'Private msTempFileFolder As String = String.Empty
        'Private Const msLAUNCH_COMMAND As String = "command.com /C "
        'Private Const msOUTPUT_FILE As String = "NWMNTout.txt"
        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

            sTmpFile = sGetTmpFileName(msOUTPUT_FILE, msTXT_EXT)
            sShell = msLAUNCH_COMMAND & sCmdName & sDevice & " > " & sTmpFile
            Debug.Print(sShell)
            My.Application.DoEvents()
            nResult = Shell(sShell, AppWinStyle.MinimizedFocus, bWAIT, nTIMEOUT)
            My.Application.DoEvents()
            Status = ">" & sCmdName & sDevice
            rtbCmds.SelectionFont = New Font(rtbCmds.SelectionFont.Name, rtbCmds.SelectionFont.Size, FontStyle.Bold)
            rtbCmds.AppendText(vbCr & ">" & sCmdName & sDevice & vbCr)
            rtbCmds.SelectionFont = New Font(rtbCmds.SelectionFont.Name, rtbCmds.SelectionFont.Size, FontStyle.Regular)
            If nResult = 0 Then
                Dim fileReader As System.IO.StreamReader = Nothing

                Try

                    'Load the file.
                    'rtbTemp.LoadFile(sTmpFile, RichTextBoxStreamType.TextTextOleObjs)
                    'rtbCmds.AppendText(rtbTemp.Text)
                    fileReader = My.Computer.FileSystem.OpenTextFileReader(sTmpFile)
                    Dim sLine As String = String.Empty
                    Do While Not (fileReader.EndOfStream)
                        sLine = fileReader.ReadLine()
                        rtbCmds.AppendText(sLine & vbCr)
                        rtbCmds.ScrollToCaret()
                        '!@*$# help doesn't explain this 
                        If fileReader.Peek() = 13 Then
                            sLine = fileReader.ReadLine()
                        End If
                    Loop
                    fileReader.Close()
                Catch ex As Exception
                    Trace.WriteLine(ex.Message)
                    Trace.WriteLine(ex.StackTrace)
                    ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                         Status, MessageBoxButtons.OK)
                Finally
                    fileReader.Close()
                End Try
            Else
                MessageBox.Show(gpsRM.GetString("psFAILED_TO_RUN"), msSCREEN_NAME, MessageBoxButtons.OK, _
                                MessageBoxIcon.Error)
            End If
            'Delete the temp file
            My.Application.DoEvents()

            My.Computer.FileSystem.DeleteFile(sTmpFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin, FileIO.UICancelOption.DoNothing)
            My.Application.DoEvents()
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                 Status, MessageBoxButtons.OK)
        End Try
        Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub

    Private Sub subRunTerminalProgram(ByVal sCmdName As String, ByVal bByDevice As Boolean)
        '********************************************************************************************
        'Description:  run a terminal program, load the output into rtbCmds
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If bByDevice Then
                Dim nProgress As Integer = 5
                Dim nSteps As Integer = chkSelectAddresses.CheckedItems.Count
                If txtAddress.Text <> String.Empty Then
                    nSteps += 1
                End If
                Dim nProgStep As Integer = 94 \ (nSteps + 1)
                Progress = nProgress
                For Each sDevice As String In chkSelectAddresses.CheckedItems
                    subRunTerminalProgram(sCmdName, sDevice)
                    nProgress += nProgStep
                    Progress = nProgress
                    Application.DoEvents()
                    If mbCancel Then
                        Exit Sub
                    End If
                Next
                If txtAddress.Text <> String.Empty Then
                    subRunTerminalProgram(sCmdName, txtAddress.Text)
                    nProgress += nProgStep
                    Progress = nProgress
                End If
            Else
                subRunTerminalProgram(sCmdName)
            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                 Status, MessageBoxButtons.OK)
        Finally
            Progress = 100
        End Try
    End Sub
    Private Sub subFanucPing(ByVal sRobot As String)
        '********************************************************************************************
        'Description:  run fanuc ping
        '
        'Parameters: none
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            sRobot = sRobot.ToLower
            For Each oRobot As clsController In colControllers
                If (sRobot = oRobot.FanucName.ToLower) Or (sRobot = oRobot.Name.ToLower) Or (sRobot = oRobot.IPAddress.ToLower) Then

                    Status = gpsRM.GetString("psPERF_FANUC_PING_ON") & sRobot

                    rtbCmds.SelectionFont = New Font(rtbCmds.SelectionFont.Name, rtbCmds.SelectionFont.Size, FontStyle.Bold)
                    rtbCmds.AppendText(vbCr & ">" & gpsRM.GetString("psPERF_FANUC_PING_ON") & sRobot & vbCr)
                    rtbCmds.SelectionFont = New Font(rtbCmds.SelectionFont.Name, rtbCmds.SelectionFont.Size, FontStyle.Regular)

                    rtbCmds.AppendText(vbTab & gpsRM.GetString("psNAME") & oRobot.Name & vbCr)
                    rtbCmds.AppendText(vbTab & gpsRM.GetString("psFANUC_NAME") & oRobot.FanucName & vbCr)
                    rtbCmds.AppendText(vbTab & gpsRM.GetString("psIP_ADDRESS") & oRobot.IPAddress & vbCr)
                    If oRobot.Robot.IsConnected Then
                        rtbCmds.AppendText(vbTab & gpsRM.GetString("psFANUC_PING_OK") & vbCr)
                    Else
                        rtbCmds.AppendText(vbTab & gpsRM.GetString("psFANUC_PING_FAILED") & vbCr)
                    End If
                    rtbCmds.ScrollToCaret()
                End If
            Next
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                 Status, MessageBoxButtons.OK)
        End Try


    End Sub
    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '********************************************************************************************
        'Description: intercept the close event        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mPrintHtml.Busy Then
            e.Cancel = True
            Exit Sub
        End If
        If bAskForSave() Then
        Else
            e.Cancel = True
            Exit Sub
        End If
    End Sub

    Private Sub subFanucPing()
        '********************************************************************************************
        'Description:  run fanuc ping
        '
        'Parameters: none
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim nProgress As Integer = 5
            Dim nSteps As Integer = chkSelectAddresses.CheckedItems.Count
            If txtAddress.Text <> String.Empty Then
                nSteps += 1
            End If
            Dim nProgStep As Integer = 94 \ (nSteps + 1)
            Progress = nProgress
            For Each sDevice As String In chkSelectAddresses.CheckedItems
                subFanucPing(sDevice)
                nProgress += nProgStep
                Progress = nProgress
                Application.DoEvents()
                If mbCancel Then
                    Progress = 100
                    Exit Sub
                End If
            Next
            If txtAddress.Text <> String.Empty Then
                subFanucPing(txtAddress.Text)
            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                 Status, MessageBoxButtons.OK)
        Finally
            Progress = 100
        End Try

    End Sub

    Private Function bAskForSave() As Boolean
        '********************************************************************************************
        'Description:  Check to see if we need to save when screen is closing 
        '
        'Parameters: none
        'Returns:    true to continue
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If (rtbHosts.Modified) Then
            Dim lRet As DialogResult

            lRet = MessageBox.Show(gcsRM.GetString("csSAVEMSG1") & vbCrLf & _
                                gcsRM.GetString("csSAVEMSG"), gcsRM.GetString("csSAVE_CHANGES"), _
                                MessageBoxButtons.YesNoCancel, _
                                MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

            Select Case lRet
                Case Response.Yes 'Response.Yes

                    Select Case Privilege
                        Case ePrivilege.None
                            ' shouldnt be here
                            subEnableControls(False)
                            Privilege = ePrivilege.Edit
                            Return False

                        Case Else
                            'ok
                            subSaveData(rtbHosts, msHostFileName)
                            Return True
                    End Select

                Case Response.Cancel
                    'false aborts closing form , changing zone etc
                    Status = gcsRM.GetString("csSAVE_CANCEL")
                    Return False
                Case Else
                    Return True
            End Select
        Else
            Return True
        End If
    End Function

    Private Sub ToolBar_ItemClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tlsHosts.ItemClicked, _
            tlsCmdsTop.ItemClicked, tlsCmdsLeft.ItemClicked, tlsList.ItemClicked
        '********************************************************************************************
        'Description:  Toolbar click handler
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/04/12  MSW     Manage Bootp setup better.  don't depend on the batch file
        '********************************************************************************************
        subEnableControls(False)
        mbCancel = False
        Dim bCancelEnable As Boolean = btnCancel.Enabled
        btnCancel.Enabled = True
        Dim bAddressesSelected As Boolean = (chkSelectAddresses.CheckedItems.Count > 0) Or _
                                               (txtAddress.Text <> String.Empty)

        Select Case e.ClickedItem.Name ' case sensitive
            'List box toolbar
            Case "tlsbControllers"
                'Load controller names in listbox
                subListControllers()
            Case "tlsbHostFileItems"
                'Load names from hostfile in listbox
                subListHostFileItems()

                'text box toolbars
            Case "tlspSaveCmds"
                subSaveData(rtbCmds)
            Case "tlspSaveHosts"
                subSaveData(rtbHosts, msHostFileName)
                btnAutoConfig.Checked = False
            Case "tlspPrintCmds"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If Not (o.DropDownButtonPressed) Then
                    mPrintHtml.subCreateSimpleDoc(rtbCmds, Status, gpsRM.GetString("psSCREENCAPTION") & " - " & gpsRM.GetString("psOUTPUT_WINDOW"))

                    mPrintHtml.subPrintDoc(False)
                End If
            Case "tlspPrintHosts"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If Not (o.DropDownButtonPressed) Then
                    mPrintHtml.subCreateSimpleDoc(rtbHosts, Status, gpsRM.GetString("psSCREENCAPTION") & " - " & msHostFileName)
                    mPrintHtml.subPrintDoc(False)
                End If
            Case "tlspCutCmds"
                rtbCmds.Cut()
            Case "tlspCutHosts"
                rtbHosts.Cut()
            Case "tlspCopyCmds"
                rtbCmds.Copy()
            Case "tlspCopyHosts"
                rtbHosts.Copy()
            Case ("tlspPasteCmds")
                rtbCmds.Paste()
            Case "tlspPasteHosts"
                rtbHosts.Paste()
            Case "tlspClearCmds"
                Dim lReply As System.Windows.Forms.DialogResult = _
                    MessageBox.Show(gpsRM.GetString("psCLEAR_WINDOW_QUES"), msSCREEN_NAME, MessageBoxButtons.YesNo, _
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                If lReply = Windows.Forms.DialogResult.Yes Then
                    rtbCmds.Clear()
                End If

                'network diagnostic commands
            Case "tlsbFanucPing"
                If bAddressesSelected Then
                    subFanucPing()
                End If
            Case "tlsbPing"
                If bAddressesSelected Then
                    subRunTerminalProgram(msCMD_PING, True)
                End If
            Case "tlsbArp"
                'This can be useful using individual addresses or without them, handle both cases
                If bAddressesSelected Then
                    subRunTerminalProgram(msCMD_ARP_ONE, True)
                Else
                    subRunTerminalProgram(msCMD_ARP_ALL, False)
                End If
            Case "tlsbTraceRoute"
                If bAddressesSelected Then
                    subRunTerminalProgram(msCMD_TRACEROUTE, True)
                End If
            Case "tlsbIPConfig"
                subRunTerminalProgram(msCMD_IPCONFIG, False)
            Case "tlsbECBRUpdate"
                'Manage Bootp setup better.  It was dependent on changing the path in the batch file
                If GetDefaultFilePath(msECBR_LOCATION, mPWCommon.eDir.ECBR, String.Empty, msCMD_BOOTP_INSTALL) Then
                    msECBR_LOCATION = """" & msECBR_LOCATION & """"
                    subRunTerminalProgram(msECBR_LOCATION, " -i")
                End If
                If GetDefaultFilePath(msECBR_LOCATION, mPWCommon.eDir.ECBR, String.Empty, msCMD_TFTP_INSTALL) Then
                    msECBR_LOCATION = """" & msECBR_LOCATION & """"
                    subRunTerminalProgram(msECBR_LOCATION, " -i")
                End If
            Case "tlsbTFTPStatus"
                subRunTerminalProgram(msTFTP_STATUS, False)
            Case "tlsbBOOTPStatus"
                subRunTerminalProgram(msBOOTP_STATUS, False)
            Case "tlsbTFTPStart"
                subRunTerminalProgram(msTFTP_START, False)
            Case "tlsbBOOTPStart"
                subRunTerminalProgram(msBOOTP_START, False)
            Case "tlsbTFTPStop"
                subRunTerminalProgram(msTFTP_STOP, False)
            Case "tlsbBOOTPStop"
                subRunTerminalProgram(msBOOTP_STOP, False)
            Case "btnHosts"
                If (btnHosts.Checked = False) AndAlso bAskForSave() Then
                    btnAutoConfig.Checked = False
                    'Load the host file text box
                    msHostFileName = Environment.ExpandEnvironmentVariables(msHOSTS_LOCATION)
                    rtbHosts.LoadFile(msHostFileName, RichTextBoxStreamType.PlainText)
                    rtbHosts.Modified = False
                    btnHosts.Checked = True
                    btnTFTP.Checked = False
                    btnBootP.Checked = False
                    btnAutoConfig.Visible = False
                Else
                    Exit Sub
                End If
            Case "btnTFTP"
                If (btnTFTP.Checked = False) AndAlso bAskForSave() Then
                    btnAutoConfig.Checked = False
                    If GetDefaultFilePath(msECBR_LOCATION, mPWCommon.eDir.ECBR, String.Empty, msTFTP_CFG) Then
                        'Load the host file text box
                        msHostFileName = msECBR_LOCATION
                        rtbHosts.LoadFile(msECBR_LOCATION, RichTextBoxStreamType.PlainText)
                        rtbHosts.Modified = False
                        btnHosts.Checked = False
                        btnTFTP.Checked = True
                        btnBootP.Checked = False
                        btnAutoConfig.Visible = True
                    Else
                        mDebug.WriteEventToLog(msSCREEN_NAME & ":frmMain.vb", "Could not find configuration file: " & _
                                               msTFTP_CFG)
                    End If
                End If
            Case "btnBootP"
                If (btnBootP.Checked = False) AndAlso bAskForSave() Then
                    btnAutoConfig.Checked = False
                    If GetDefaultFilePath(msECBR_LOCATION, mPWCommon.eDir.ECBR, String.Empty, msBOOTP_CFG) Then
                        'Load the host file text box
                        msHostFileName = msECBR_LOCATION
                        rtbHosts.LoadFile(msECBR_LOCATION, RichTextBoxStreamType.PlainText)
                        rtbHosts.Modified = False
                        btnHosts.Checked = False
                        btnTFTP.Checked = False
                        btnBootP.Checked = True
                        btnAutoConfig.Visible = True
                    Else
                        mDebug.WriteEventToLog(msSCREEN_NAME & ":frmMain.vb", "Could not find configuration file: " & _
                                               msTFTP_CFG)
                    End If
                End If
            Case "btnAutoConfig"
                subDoAutoConfig()


        End Select
        If (bCancelEnable = False) Then
            btnCancel.Enabled = False
            subEnableControls(True)
        End If
    End Sub
    Private Sub subDoAutoConfig()
        '********************************************************************************************
        'Description:  auto fill the tftp or bootp config
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/29/11  MSW     1st draft
        ' 05/16/14  MSW     Fix ID# in BOOTP autoconfig
        ' 07/24/14  RJO     Changed sRobotName to be the HostName
        '********************************************************************************************
        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        Try
            If (btnTFTP.Checked) Then
                Dim lRet As DialogResult = MessageBox.Show(gpsRM.GetString("psAUTO_CFG_TFTP_PROMPT1"), _
                    gpsRM.GetString("psAUTO_CFG_TFTP_PROMPT2"), MessageBoxButtons.OKCancel, _
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
                Select Case lRet
                    Case Response.OK
                        Dim sPCName As String = My.Computer.Name
                        Dim sAddress As String = colZones.ActiveZone.IPAddress
                        Dim sFolder As String = String.Empty
                        GetDefaultFilePath(sFolder, mPWCommon.eDir.RobotImageBackups, String.Empty, String.Empty)
                        Dim sText As String = "id=1, name=" & sPCName & ", ip=" & sAddress & ", tr=" & sFolder & ", uu=False"
                        rtbHosts.Clear()
                        rtbHosts.AppendText(sText)
                    Case Else
                End Select
            End If
            If (btnBootP.Checked) Then
                Dim lRet As DialogResult = MessageBox.Show(gpsRM.GetString("psAUTO_CFG_BOOTP_PROMPT1"), _
                    gpsRM.GetString("psAUTO_CFG_BOOTP_PROMPT2"), MessageBoxButtons.OKCancel, _
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
                Select Case lRet
                    Case Response.OK
                        rtbHosts.Clear()
                        Dim sServerAddress As String = colZones.ActiveZone.IPAddress

                        colControllers = New clsControllers(colZones, False)
                        For Each oRC As clsController In colControllers
                            Dim oVar As FRCVar = DirectCast(oRC.Robot.SysVariables.Item("$Version"), FRCVar)
                            Dim sIP As String = oRC.IPAddress
                            Dim sMac As String = sGetMacAddress(sIP) '"00:e0:e4:0f:c6:35"
                            Dim sSubNetMask As String = "255.255.255.0"
                            Dim sRobotName As String = oRC.FanucName '.Name RJO 07/24/14 This needs to be the HostName.
                            Dim stext As String = "id=" & oRC.ControllerNumber.ToString & ", hn=" & sRobotName & ", et=" & sMac & ", ip=" & sIP & ", sn=" & _
                                sSubNetMask & ", gw=" & sServerAddress & ", dn=, dm=, fp=" & sRobotName & _
                                "\, sr=" & sServerAddress & ", bl=\, ll=\" & vbCrLf
                            rtbHosts.AppendText(stext)
                        Next
                    Case Else
                End Select
            End If
        Catch ex As Exception

        End Try
        Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub
    Private Function sGetMacAddress(ByVal sIP As String) As String
        '********************************************************************************************
        'Description:  get a mac address for the bootp config
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/29/11  MSW     1st draft
        '********************************************************************************************
        Const nTIMEOUT As Integer = 45000
        Const bWAIT As Boolean = True
        Dim nResult As Integer = 0
        Dim sShell As String
        Dim sTmpFile As String = String.Empty
        Dim sMac As String = String.Empty
        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

            sTmpFile = sGetTmpFileName(msOUTPUT_FILE, msTXT_EXT)
            sShell = msLAUNCH_COMMAND & msCMD_ARP_ONE & sIP & " > " & sTmpFile
            Debug.Print(sShell)
            My.Application.DoEvents()
            nResult = Shell(sShell, AppWinStyle.Hide, bWAIT, nTIMEOUT)
            My.Application.DoEvents()
            If nResult = 0 Then
                Dim fileReader As System.IO.StreamReader = Nothing
                Try

                    'Load the file.
                    fileReader = My.Computer.FileSystem.OpenTextFileReader(sTmpFile)
                    Dim sLine As String = String.Empty
                    Do While Not (fileReader.EndOfStream) And (sMac = String.Empty)
                        sLine = fileReader.ReadLine().Trim
                        Debug.Print(sLine)
                        Dim sTmpArr As String() = Split(sLine)
                        If sTmpArr.GetUpperBound(0) > 0 AndAlso sTmpArr(0) = sIP Then
                            Dim nIndex As Integer = 0
                            Do While sMac = String.Empty And sTmpArr.GetUpperBound(0) > nIndex
                                nIndex = nIndex + 1
                                If sTmpArr(nIndex) <> String.Empty Then
                                    sMac = sTmpArr(nIndex)
                                    sMac = Strings.Replace(sMac, "-", ":")
                                End If
                            Loop

                        End If
                        '!@*$# help doesn't explain this 
                        If fileReader.Peek() = 13 Then
                            sLine = fileReader.ReadLine()
                        End If
                    Loop
                    fileReader.Close()
                Catch ex As Exception
                    Trace.WriteLine(ex.Message)
                    Trace.WriteLine(ex.StackTrace)
                    ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                         Status, MessageBoxButtons.OK)
                Finally
                    fileReader.Close()
                End Try
            Else
                MessageBox.Show(gpsRM.GetString("psFAILED_TO_RUN"), msSCREEN_NAME, MessageBoxButtons.OK, _
                                MessageBoxIcon.Error)
            End If
            'Delete the temp file
            My.Application.DoEvents()
            My.Computer.FileSystem.DeleteFile(sTmpFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin, FileIO.UICancelOption.DoNothing)
            My.Application.DoEvents()
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                 Status, MessageBoxButtons.OK)
        End Try
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Return sMac
    End Function
    Private Sub mnuSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSelectAll.Click
        '********************************************************************************************
        'Description:  address list select all
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/22/10  MSW     mnuSelectAll_Click, mnuUnselectAll_Click - add call to subEnableControls
        '********************************************************************************************
        For nItem As Integer = 0 To chkSelectAddresses.Items.Count - 1
            chkSelectAddresses.SetItemCheckState(nItem, CheckState.Checked)
        Next
        subEnableControls(True)
    End Sub
    Private Sub mnuUnselectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuUnselectAll.Click
        '********************************************************************************************
        'Description:  address list unselect all
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/22/10  MSW     mnuSelectAll_Click, mnuUnselectAll_Click - add call to subEnableControls
        '********************************************************************************************
        For nItem As Integer = 0 To chkSelectAddresses.Items.Count - 1
            chkSelectAddresses.SetItemCheckState(nItem, CheckState.Unchecked)
        Next
        subEnableControls(True)
    End Sub

    Private Sub rtb_ModifiedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rtbHosts.ModifiedChanged, rtbCmds.ModifiedChanged
        '********************************************************************************************
        'Description:  enable or disable toolbar buttons when rtb status changes
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subEnableControls(True)
    End Sub

    Private Sub rtb_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rtbHosts.SelectionChanged, rtbCmds.SelectionChanged
        '********************************************************************************************
        'Description:  enable or disable toolbar buttons when rtb status changes
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bCmdSel As Boolean = rtbCmds.SelectionLength > 0
        tlspCutCmds.Enabled = bCmdSel
        tlspCopyCmds.Enabled = bCmdSel
        Dim bHostSel As Boolean = rtbHosts.SelectionLength > 0
        tlspCutHosts.Enabled = bHostSel
        tlspCopyHosts.Enabled = bHostSel
    End Sub

    Private Sub chkSelectAddresses_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkSelectAddresses.SelectedIndexChanged
        '********************************************************************************************
        'Description:  enable or disable toolbar buttons when selection changes
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subEnableControls(True)
    End Sub

    Private Sub txtAddress_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtAddress.TextChanged
        '********************************************************************************************
        'Description:  enable or disable toolbar buttons when selection changes
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subEnableControls(True)
    End Sub

    Private Sub MnuPreviewCmds_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MnuPreviewCmds.Click
        '********************************************************************************************
        'Description:  run print preview for output window
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mPrintHtml.subCreateSimpleDoc(rtbCmds, Status, gpsRM.GetString("psSCREENCAPTION") & " - " & gpsRM.GetString("psOUTPUT_WINDOW"))
        mPrintHtml.subShowPrintPreview()
    End Sub

    Private Sub MnuPageSetupCmds_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MnuPageSetupCmds.Click
        '********************************************************************************************
        'Description:  run page setup for output window
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mPrintHtml.subCreateSimpleDoc(rtbCmds, Status, gpsRM.GetString("psSCREENCAPTION") & " - " & gpsRM.GetString("psOUTPUT_WINDOW"))
        mPrintHtml.subShowPageSetup()
    End Sub

    Private Sub MnuPreviewHosts_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MnuPreviewHosts.Click
        '********************************************************************************************
        'Description:  run print preview for host file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/07/10  MSW	    Print function clean up
        '********************************************************************************************

        mPrintHtml.subCreateSimpleDoc(rtbHosts, Status, gpsRM.GetString("psSCREENCAPTION") & " - " & msHostFileName)
        mPrintHtml.subShowPrintPreview()

    End Sub

    Private Sub mnuPageSetupHosts_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPageSetupHosts.Click
        '********************************************************************************************
        'Description:  run page setup for host file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/07/10  MSW	    Print function clean up
        '********************************************************************************************
        mPrintHtml.subCreateSimpleDoc(rtbHosts, Status, gpsRM.GetString("psSCREENCAPTION") & " - " & msHostFileName)
        mPrintHtml.subShowPageSetup()
    End Sub

    Private Sub mnuPrintCmds_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintCmds.Click
        '********************************************************************************************
        'Description:  print the output window
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        mPrintHtml.subCreateSimpleDoc(rtbCmds, Status, gpsRM.GetString("psSCREENCAPTION") & " - " & gpsRM.GetString("psOUTPUT_WINDOW"))
        mPrintHtml.subPrintDoc(False)
    End Sub

    Private Sub MnuPrintHosts_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MnuPrintHosts.Click
        '********************************************************************************************
        'Description:  print the host file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/07/10  MSW	    Print function clean up
        '********************************************************************************************

        mPrintHtml.subCreateSimpleDoc(rtbHosts, Status, gpsRM.GetString("psSCREENCAPTION") & " - " & msHostFileName)
        mPrintHtml.subPrintDoc(False)
    End Sub
    Private Sub mnuPrintFileCmds_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintFileCmds.Click
        '********************************************************************************************
        'Description:  save the output window to a web page file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/07/10  MSW	    Print function clean up
        '********************************************************************************************

        mPrintHtml.subCreateSimpleDoc(rtbCmds, Status, gpsRM.GetString("psSCREENCAPTION") & " - " & gpsRM.GetString("psOUTPUT_WINDOW"))
        mPrintHtml.subSaveAs()
    End Sub
    Private Sub mnuPrintFileHosts_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintFileHosts.Click
        '********************************************************************************************
        'Description:  save the host file to a web page file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        mPrintHtml.subCreateSimpleDoc(rtbHosts, Status, gpsRM.GetString("psSCREENCAPTION") & " - " & msHostFileName)
        mPrintHtml.subSaveAs()
    End Sub

End Class