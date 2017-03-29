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
' Description: View the robot web pages
' closing

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
' 07/27/2009    MSW     first draft
' 04/20/10      MSW     Easier navigation
' 09/14/11      MSW     Assemble a standard version of everything                       4.1.0.0
' 09/27/11      MSW     Standalone changes round 1 - HGB SA paintshop computer changes  4.1.0.1
'                       There's nothing for standalone in here, but I'm hoping Harvey 
'                       fixed something that's been bugging me with frmMain_FormClosing
' 09/30/11      MSW     Move me.show before zone init to give a quicker response at     4.1.0.2
'                       startup.  Use new controller array from PWcommon (4.1.0.2) 
'                       instead of loading collections
' 10/14/11      MSW     Read from XML                                                   4.1.1.0
' 01/09/12      MP/RJO  Modified subAddZonetoList to use IP address instead of Host     4.1.1.1
'                       name to fix a problem with vision setup screens.
' 01/24/12      MSW     Placeholder include file updates                                4.1.1.2
' 02/15/12      MSW     dealing with 64 bit windows quirks                              4.1.1.3
' 03/20/12      RJO     Modified to use .NET Password and IPC                           4.1.2.0
' 04/11/12      MSW     Change CommonStrings setup so it'll build correctly             4.1.3.0
' 04/19/12      MSW     bAddNodesToTree - Differentiate between arms and painters for robot lists 4.1.3.1
' 04/27/12      MSW     Turn on KeyPreview so Fkeys work                                 4.1.3.2
' 06/07/12      MSW     webMain_PreviewKeyDown - form keydown wasn't getting called      4.1.3.3
'                       when the browser was active
' 10/23/12      RJO     Added StartupModule to project to prevent multiple instances     4.1.3.4
' 11/05/12      MSW     Make a custom startup so the command line gets passed to the     4.1.3.5
'                       already running process
' 11/15/12      HGB     In subProcessCommandLine, show only the arms in each zone, not   4.1.3.6
'                       all arms in all zones.
' 01/16/13      MSW     Add some strings to projectstrings.resx for standard manuals     4.1.3.7
'                       and project customer service manual
' 04/16/13      MSW     Add Canadian language files                                      4.1.5.0
'                       Standalone Changelog
' 07/09/13      MSW     Update and standardize logos                                     4.01.05.01
' 07/19/13      MSW     Add version selection for robot links                            4.01.05.02
' 08/15/13      MSW     Fix some occasional errors by setting Progess to 0 instead of 1  4.01.05.03
'                       Progress, Status - Add error handler so it doesn't get hung up
' 09/12/13      MSW     Add psPRGSTATE to project strings                                4.01.05.04
' 09/30/13      MSW     Save screenshots as jpegs                                        4.01.05.05
'                       subProcessCommandLine - Log files not found for passed in links
' 10/08/13      MSW     Add Labels for sealer manuals in projectstrings                  4.01.06.00
' 01/06/14      MSW     More updates from fairfax sealer                                 4.01.06.01
' 02/03/14      RJO     Fix some errors in webMain_ProgressChanged                       4.01.06.02
' 02/13/14      MSW     Switch cross-thread handling to BeginInvoke call                 4.01.07.00
' 02/17/14      MSW     Resource string updates from inovision                           4.01.07.01
' 04/04/14      MSW     Consolidate some screen icon settings, labels                    4.01.07.02
' 05/16/14      MSW     Add a resource string for ZDT help                               4.01.07.03
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On


Imports Response = System.Windows.Forms.DialogResult
Imports ConnStat = FRRobotNeighborhood.FRERNConnectionStatusConstants

Imports System
Imports System.Collections
Imports System.Xml
Imports System.Xml.XPath

Imports System.IO
'Custom StartupModule to pass command line to the already running screen
Module StartupModule
    Private mFileWriter As System.IO.StreamWriter

    Declare Function ShowWindowAsync Lib "user32.dll" ( _
            ByVal Hwnd As IntPtr, _
            ByVal swCommand As Int32) As IntPtr

    Declare Function SetForegroundWindow Lib "user32.dll" ( _
        ByVal Hwnd As IntPtr) As Boolean

    
    Public Sub Main()
        Dim oProc As Process = Process.GetCurrentProcess
        Dim sName As String = oProc.ProcessName
        Dim nDbg As Integer = InStr(sName, ".vshost", CompareMethod.Text)
        Dim nCnt As Integer = 1
        If nDbg > 0 Then
            sName = sName.Substring(0, nDbg - 1)
            nCnt = 0
        End If
        Dim oProcs() As Process = Process.GetProcessesByName(sName)

        If oProcs.Length <= nCnt Then
            Application.Run(frmMain)
        Else
            Dim nProc As integer = 0
            Dim nThisProc As System.IntPtr = oProc.Handle
            For nIndex As Integer = oProcs.GetLowerBound(0) To oProcs.GetUpperBound(0)
                If nThisProc <> oProcs(nIndex).Handle Then
                    nProc = nIndex
                End If
            Next
            Dim sCmd As String = String.Empty
            For Each s As String In My.Application.CommandLineArgs
                sCmd = sCmd & " " & s
            Next
            If sCmd <> String.Empty Then
                Dim sPath As String = String.Empty
                mPWCommon.GetDefaultFilePath(sPath, eDir.IPC, String.Empty, String.Empty)
                sPath = sGetTmpFileName(sPath, "ShowBrowser", ".ShowBrowser")
                Application.DoEvents()
                mFileWriter = New System.IO.StreamWriter(sPath)
                mFileWriter.WriteLine(sCmd)
                mFileWriter.Close()
                Application.DoEvents()
            End If
            'This is probably overkill, just hitting it with everything we can to get the desired program in front
            ShowWindowAsync(oProcs(nProc).MainWindowHandle, 5) 'SW_SHOW
            Threading.Thread.Sleep(10)
            'ShowWindowAsync(hWnd, SW_RESTORE);
            ShowWindowAsync(oProcs(nProc).MainWindowHandle, 9) 'SW_RESTORE
            Threading.Thread.Sleep(10)
            SetForegroundWindow(oProcs(nProc).MainWindowHandle)

        End If
    End Sub

End Module
Friend Class frmMain
#Region " Declares "

    '******** Form Constants   **********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    'Not a constant here, we'll try to get help in the same executable
    Friend Const msSCREEN_NAME As String = "PWBrowser"   ' <-- For password area change log etc.
    Private Const msSCREEN_DUMP_NAME_WEB As String = "View_RobotWeb_Main.jpg"
    Private Const msSCREEN_DUMP_NAME_MANUALS As String = "Help_OnlineManuals.jpg"
    Private Const msSCREEN_DUMP_NAME_HELP As String = "Help_OnlineHELP.jpg"
    Private Const mnMAININX As Integer = 100
    Private Const mnSCREENINX As Integer = 101
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    Friend Const msHTTP_Prefix As String = "http://"
    Private Const msONLINE_MANUALS As String = "OnlineManuals"
    Private Const msPW_HELP As String = "PWHelp"
    Private Const msWEB_SERVERS As String = "WebServers"
    Private Const msHELP_DBNAME As String = "Help"
    Private Const msROBOT_PAGES As String = "RobotPages"
    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    Friend Shared gsChangeLogArea As String = msSCREEN_NAME
    Private msOldZone As String = String.Empty
    Private colZones As clsZones = Nothing
    Private tControllerArray() As tController = Nothing
    Private tArmArray() As tArm = Nothing
    Private mbEventBlocker As Boolean = False
    Private mbRemoteZone As Boolean = False
    Private mbScreenLoaded As Boolean = False
    Private msLastPage As String = String.Empty
    Private msHomePage As String = "C:\Paint\Vbapps\Robot Web Page.htm"
    Private msLaunchScreen As String = String.Empty
    Private mbSkipFirstTreeViewSelect As Boolean = False
    Private mbRobots As Boolean = False
    Private mbManuals As Boolean = False
    Private mbPWHelp As Boolean = False
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean = False
    Private mbPresetsLoaded As Boolean = False
    Private mnProgress As Integer = 0
    Private mnPopupMenuStep As Integer = 0
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    Private msCulture As String = "en-US" 'Default to english
    Friend Structure tNode
        Dim Name As String
        Dim Link As String
        Dim Parent As String
        Dim Key As String
        Dim Mask As String
        Dim Version As String
    End Structure
    '******** End Property Variables    *************************************************************

    '******** This is the old pw3 password object interop  *****************************************
    'Friend WithEvents moPassword As PWPassword.PasswordObject
    'Friend moPrivilege As PWPassword.cPrivilegeObject
    'Private Shared mbNoRecursion As Boolean = False
    'Private msColorCache As String = String.Empty
    '******** This is the old pw3 password object interop  *****************************************
    Friend WithEvents moPassword As clsPWUser
    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    'Delegate Sub ControllerStatusChange(ByVal Controller As clsController)
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm
    Delegate Sub NewMessage_Callback(ByVal Schema As String, ByVal DS As DataSet)
    Delegate Sub OnChange_Callback(ByVal source As Object, ByVal e As FileSystemEventArgs)

    Private mnMaxProgress As Integer = 100
    Private mNavigatedCount As Integer = 0

    Private msIPCPath As String = String.Empty
    Private mWatcher As FileSystemWatcher
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


            'If mPassword.CheckPassword(msSCREEN_NAME, Value, moPassword, moPrivilege) Then 'RJO 03/15/12
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
    
    Private Sub subAddZonetoList()
        '********************************************************************************************
        'Description: New Zone Selected - check for save then load new info
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/09/12  MP/RJO  Modified to accommodate a quirk when communicating to the vision master 
        '                   robot in a Sealer system.  If the HostName is used to resolve to the 
        '                   IPAddress, then the vision setup screens will not display.  Everything 
        '                   works, however, when the IPAddress is used instead.
        '********************************************************************************************
        Progress = 10

        msOldZone = colZones.CurrentZone

        'msColorCache = String.Empty 'RJO 03/15/12

        Try
            Dim nodeZone As New Windows.Forms.TreeNode(colZones.ActiveZone.Name)
            nodeZone.Name = colZones.ActiveZone.Name
            nodeZone.Text = colZones.ActiveZone.Name
            nodeZone.Tag = msHomePage
            'Add the PLC if it's supported and any other automation web servers
            Dim oNodes As tNode() = Nothing
            Dim sTable As String = msWEB_SERVERS
            Dim sPath As String = String.Empty
            If (bGetNodeTreeFromDB(sTable, oNodes) = False) OrElse _
               (bAddNodesToTree(oNodes, sPath, nodeZone.Nodes) = False) Then
                WriteEventToLog(msSCREEN_NAME, (gcsRM.GetString("csDB_ERROR") & sTable))
            End If
            Dim oRobotPageNodes As tNode() = Nothing
            sTable = msROBOT_PAGES
            If (bGetNodeTreeFromDB(sTable, oRobotPageNodes) = False) Then
                WriteEventToLog(msSCREEN_NAME, (gcsRM.GetString("csDB_ERROR") & sTable))
            End If
            For Each oController As tController In tControllerArray
                Dim rcNode As New Windows.Forms.TreeNode(oController.DisplayName)
                rcNode.Name = oController.DisplayName
                rcNode.Text = oController.DisplayName
                'sPath = msHTTP_Prefix & oController.HostName 'RJO 01/09/12
                sPath = msHTTP_Prefix & Dns.GetHostEntry(oController.HostName).AddressList(0).ToString 'RJO 01/09/12
                rcNode.Tag = sPath
                Dim nVersion As Single = 0
                Select Case oController.ControllerType
                    Case eControllerType.RJ3
                        nVersion = 5
                    Case eControllerType.RJ3iB
                        nVersion = 6
                    Case eControllerType.RJ3iB_P500
                        nVersion = 6.3
                    Case eControllerType.R30iA
                        nVersion = 7
                    Case eControllerType.R30iB
                        nVersion = 8
                    Case Else
                        nVersion = 0
                End Select
                bAddNodesToTree(oRobotPageNodes, sPath, rcNode.Nodes, oController.HostName, nVersion)
                nodeZone.Nodes.Add(rcNode)
            Next
            trvwPages.Nodes.Add(nodeZone)
            Status(True) = gcsRM.GetString("csSELECT_ROBOT")

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
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
        lblTitle.Text = String.Empty
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
        btnClose.Enabled = True

        If bEnable = False Then
            btnSave.Enabled = False
            btnPrint.Enabled = False
            btnMultiView.Enabled = True
            bRestOfControls = False
        Else
            Select Case Privilege
                Case ePrivilege.None
                    btnSave.Enabled = False
                    btnPrint.Enabled = True
                    btnMultiView.Enabled = True
                    bRestOfControls = False
                    cboAddress.Enabled = False
                Case ePrivilege.Execute
                    btnSave.Enabled = True
                    btnPrint.Enabled = True
                    btnMultiView.Enabled = True
                    bRestOfControls = False
                    cboAddress.Enabled = True
                Case Else
                    btnSave.Enabled = True
                    btnPrint.Enabled = True
                    btnMultiView.Enabled = True
                    bRestOfControls = False
                    cboAddress.Enabled = False
            End Select
        End If

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
        '********************************************************************************************

        With gpsRM
            
        End With
    End Sub
    Private Function bfindNode(ByVal sParent As String, ByRef ndParent As Windows.Forms.TreeNode, ByRef colNodes As Windows.Forms.TreeNodeCollection) As Boolean
        '********************************************************************************************
        'Description:   Search through treeview for sParent
        '
        'Parameters:  sParent - name of the node to find
        'Returns:    true when found, ndParent
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim aNode As TreeNode
        For Each aNode In colNodes
            If aNode.Name = sParent Then
                ndParent = aNode
                Return True
            Else
                If bfindNode(sParent, ndParent, aNode.Nodes) Then
                    Return True
                End If
            End If
        Next
        Return False
    End Function


    Private Function bAddNodesToTree(ByRef oNodes As tNode(), ByRef sPath As String, _
                                     ByRef oRootNodeList As TreeNodeCollection, _
                                     Optional ByVal sMask As String = "", _
                                     Optional ByVal nVersion As Single = 0) As Boolean
        '********************************************************************************************
        'Description: Add node trees from DB
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/3/11   MSW     move to XML
        ' 04/19/12  MSW     bAddNodesToTree - Differentiate between arms and painters for robot lists
        '********************************************************************************************
        Dim sName As String = String.Empty
        Dim sParent As String = String.Empty
        Dim bSuccess As Boolean = True
        Dim sTopNode As String = String.Empty
        Const sARM_TAG As String = "#ARM#"
        Const sPAINTER_TAG As String = "#PAINTER#"
        Const sCONTROLLER_TAG As String = "#CONTROLLER#"
        Try
            If oNodes.Count > 0 Then
                For Each oNode As tNode In oNodes
                    Dim bSkip As Boolean = False
                    If (sMask <> String.Empty) Then
                        Dim sTestMask As String = sMask.ToLower
                        Try
                            'If there's DB trouble, just skip the mask
                            If oNode.Mask <> String.Empty Then
                                sTestMask = oNode.Mask.ToLower
                                bSkip = InStr(sTestMask, sMask.ToLower) = 0
                            End If
                        Catch ex As Exception
                        End Try
                    End If
                    If (bSkip = False) And (nVersion > 0) Then
                        Try
                            'If there's DB trouble, just skip the mask
                            If oNode.Version <> String.Empty Then
                                Dim nDash As Integer = InStr(oNode.Version, "-")
                                If nDash = 1 Then
                                    Dim nVer As Double = CDbl(oNode.Version.Substring(nDash))
                                    bSkip = nVersion >= nVer
                                ElseIf nDash > 1 Then
                                    Dim nVer1 As Double = CDbl(oNode.Version.Substring(0, nDash - 1))
                                    Dim nVer2 As Double = CDbl(oNode.Version.Substring(nDash))
                                    bSkip = (nVersion > nVer1) And (nVersion < nVer2)
                                Else
                                    Dim nVer As Double = CDbl(oNode.Version)
                                    bSkip = nVersion < nVer
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                    End If
                    If bSkip = False Then
                        Dim nodeTmp As New Windows.Forms.TreeNode(colZones.ActiveZone.Name)
                        'An attempt top make the custom stuff a little easier
                        'if it starts with "ps", read from the resource file, otherwise take the name right from the DB
                        sName = oNode.Name
                        If sName.Substring(0, 2) = "ps" Then
                            nodeTmp.Text = gpsRM.GetString(sName)
                        Else
                            nodeTmp.Text = sName
                        End If
                        nodeTmp.Name = oNode.Key
                        nodeTmp.Tag = sPath & oNode.Link
                        sParent = oNode.Parent
                        If sParent = "#ROOT#" Then
                            If (InStr(nodeTmp.Name, sARM_TAG) > 0) Then
                                For Each oArm As tArm In tArmArray
                                    Dim nodeTmp2 As New Windows.Forms.TreeNode(nodeTmp.Text.Replace(sARM_TAG, oArm.ArmDisplayName))
                                    nodeTmp2.Name = nodeTmp.Name.Replace(sARM_TAG, oArm.ArmDisplayName)
                                    nodeTmp2.Tag = nodeTmp.Tag.ToString.Replace(sARM_TAG, oArm.ArmDisplayName)
                                    oRootNodeList.Add(nodeTmp2)
                                Next
                            ElseIf (InStr(nodeTmp.Name, sPAINTER_TAG) > 0) Then
                                For Each oArm As tArm In tArmArray
                                    If oArm.IsOpener = False And oArm.ColorChangeType <> eColorChangeType.NONE Then
                                        Dim nodeTmp2 As New Windows.Forms.TreeNode(nodeTmp.Text.Replace(sPAINTER_TAG, oArm.ArmDisplayName))
                                        nodeTmp2.Name = nodeTmp.Name.Replace(sPAINTER_TAG, oArm.ArmDisplayName)
                                        nodeTmp2.Tag = nodeTmp.Tag.ToString.Replace(sPAINTER_TAG, oArm.ArmDisplayName)
                                        oRootNodeList.Add(nodeTmp2)
                                        Exit For
                                    End If
                                Next
                            ElseIf (InStr(nodeTmp.Name, sCONTROLLER_TAG) > 0) Then
                                For Each oController As tController In tControllerArray
                                    Dim nodeTmp2 As New Windows.Forms.TreeNode(nodeTmp.Text.Replace(sCONTROLLER_TAG, oController.HostName))
                                    nodeTmp2.Name = nodeTmp.Name.Replace(sCONTROLLER_TAG, oController.HostName)
                                    nodeTmp2.Tag = nodeTmp.Tag.ToString.Replace(sCONTROLLER_TAG, oController.HostName)
                                    oRootNodeList.Add(nodeTmp2)
                                Next
                            Else
                                oRootNodeList.Add(nodeTmp)
                            End If
                            sTopNode = nodeTmp.Name
                        Else
                            Dim ndParent As Windows.Forms.TreeNode = Nothing
                            'generate nodes per arm or controller as needed
                            If bfindNode(sParent, ndParent, oRootNodeList) Then
                                If (InStr(nodeTmp.Name, sARM_TAG) > 0) Then
                                    For Each oArm As tArm In tArmArray
                                        Dim nodeTmp2 As New Windows.Forms.TreeNode(nodeTmp.Text.Replace(sARM_TAG, oArm.ArmDisplayName))
                                        nodeTmp2.Name = nodeTmp.Name.Replace(sARM_TAG, oArm.ArmDisplayName)
                                        nodeTmp2.Tag = nodeTmp.Tag.ToString.Replace(sARM_TAG, oArm.ArmDisplayName)
                                        ndParent.Nodes.Add(nodeTmp2)
                                    Next
                                ElseIf (InStr(nodeTmp.Name, sCONTROLLER_TAG) > 0) Then
                                    For Each oController As tController In tControllerArray
                                        Dim nodeTmp2 As New Windows.Forms.TreeNode(nodeTmp.Text.Replace(sCONTROLLER_TAG, oController.HostName))
                                        nodeTmp2.Name = nodeTmp.Name.Replace(sCONTROLLER_TAG, oController.HostName)
                                        nodeTmp2.Tag = nodeTmp.Tag.ToString.Replace(sCONTROLLER_TAG, oController.HostName)
                                        ndParent.Nodes.Add(nodeTmp2)
                                    Next
                                Else
                                    ndParent.Nodes.Add(nodeTmp)
                                End If
                            Else
                                mDebug.WriteEventToLog("PWBrowser:bAddNodesToTree", sParent & " not found.")
                                bSuccess = False
                            End If
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            bSuccess = False
        Finally
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try
        Return bSuccess
    End Function
    Private Function bGetNodeTreeFromDB(ByVal sDBName As String, ByRef oNodes As tNode()) As Boolean
        '********************************************************************************************
        'Description:  This Function populates the DataTable that will be used by subStarPWApps.
        '
        'Parameters: None
        'Returns:    True if Success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/3/11   MSW     move to XML
        '********************************************************************************************
        Const sBROWSER_XMLFILE As String = "Browser"
        Const sBROWSER_XMLNODE As String = "WebLink"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Dim tControllerArray() As tController = Nothing


        Try
            If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sBROWSER_XMLFILE & ".XML") Then


                oXMLDoc.Load(sXMLFilePath)

                oMainNode = oXMLDoc.SelectSingleNode("//" & sBROWSER_XMLFILE)
                oNodeList = oMainNode.SelectNodes("//" & sDBName & "//" & sBROWSER_XMLNODE)
                Try
                    If oNodeList.Count = 0 Then
                        mDebug.WriteEventToLog("PWBrowser:bGetNodeTreeFromDB", sXMLFilePath & " not found.")
                        mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bGetNodeTreeFromDB", _
                                                sXMLFilePath & " not found.")
                        Return False
                    Else
                        Dim nNodeOffset As Integer = 0
                        Dim nMax As Integer = oNodeList.Count - 1
                        ReDim oNodes(nMax)
                        For nNode As Integer = 0 To nMax
                            Try
                                oNodes(nNode).Name = oNodeList(nNode).Item("Name").InnerXml
                                oNodes(nNode).Link = oNodeList(nNode).Item("Link").InnerXml
                                oNodes(nNode).Key = oNodeList(nNode).Item("Key").InnerXml
                                oNodes(nNode).Parent = oNodeList(nNode).Item("Parent").InnerXml
                                Try
                                    oNodes(nNode).Mask = oNodeList(nNode).Item("Mask").InnerXml
                                Catch ex As Exception
                                    oNodes(nNode).Mask = String.Empty
                                End Try
                                If sDBName.ToLower.Contains("robot") Then
                                    Try
                                        oNodes(nNode).Version = oNodeList(nNode).Item("Version").InnerXml
                                    Catch ex As Exception
                                        oNodes(nNode).Version = String.Empty
                                    End Try
                                Else
                                    oNodes(nNode).Version = String.Empty
                                End If
                            Catch ex As Exception
                                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bGetNodeTreeFromDB", _
                                                        "Invalid Data: " & sXMLFilePath & " - " & _
                                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                Return False
                            End Try
                        Next
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bGetNodeTreeFromDB", _
                                           "Invalid XML Data: " & sXMLFilePath & " - " & _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    Return False
                End Try
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bGetNodeTreeFromDB", _
                                   "Invalid XPath syntax: " & sXMLFilePath & " - " & _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Return False
        End Try
        Return True
    End Function
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
        ' 09/30/11  MSW     Move me.show before zone init to give a quicker response at startup
        '********************************************************************************************
        Dim lReply As Windows.Forms.DialogResult = Response.OK

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor


        Try

            Status = gcsRM.GetString("csINITALIZING")
            Progress = 1

            DataLoaded = False
            mbScreenLoaded = False



            Me.Show()

            colZones = New clsZones(String.Empty)

            Progress = 5

            Me.Text = gpsRM.GetString("psSCREENCAPTION")

            'init new IPC and new Password 'RJO 03/15/12
            oIPC = New Paintworks_IPC.clsInterProcessComm
            moPassword = New clsPWUser
            Progress = 50

            mScreenSetup.InitializeForm(Me)
            subInitFormText()
            Progress = 70

            Progress = 98
            subEnableControls(True)

            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound


            'statusbar text
            Status(True) = gpsRM.GetString("psSELECT_PAGE")

            mNavigatedCount = 0
            Progress = 1
            webMain.Navigate(msHomePage)
            msLastPage = msHomePage
            Application.DoEvents()
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subClearScreen()

            subEnableControls(False)

            trvwPages.Nodes.Clear()

            mbRobots = False
            mbPWHelp = False
            mbManuals = False
            msLaunchScreen = String.Empty
            Application.DoEvents()
            subProcessCommandLine()


            Application.DoEvents()
            If msLaunchScreen = String.Empty Then
                Dim sTmp As String = DirectCast(trvwPages.Nodes(0).Tag, String)
                trvwPages.SelectedNode = trvwPages.Nodes(0)
                mNavigatedCount = 0

                If msLastPage <> sTmp Then
                    Progress = 1
                    msLastPage = sTmp
                    webMain.Navigate(msLastPage)
                End If
            Else
                'msLastPage = msLaunchScreen
                'mbSkipFirstTreeViewSelect = True
                'mNavigatedCount = 0
                'Progress = 1
                'webMain.Navigate(msLastPage)
                'Debug.Print(msLastPage)
            End If

            mPWCommon.GetDefaultFilePath(msIPCPath, eDir.IPC, String.Empty, String.Empty)

            mWatcher = New FileSystemWatcher

            With mWatcher
                .Path = msIPCPath
                'Watch for changes in LastWrite times 
                .NotifyFilter = NotifyFilters.LastWrite
                .Filter = "ShowBrowser*.ShowBrowser"

                'Add event handler
                AddHandler .Changed, AddressOf OnChanged

                ' Begin watching.
                .EnableRaisingEvents = True
            End With
            Dim sFiles As String() = Directory.GetFiles(msIPCPath)
            For Each sFile As String In sFiles
                If My.Computer.FileSystem.FileExists(sFile) Then
                    My.Computer.FileSystem.DeleteFile(sFile)
                End If
            Next


            subEnableControls(True)

            Me.stsStatus.Refresh()

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
    Private Sub OnChanged(ByVal source As Object, ByVal e As FileSystemEventArgs)
        '********************************************************************************************
        'Description: A text file of interest was written to. Process, then delete the file.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If Me.InvokeRequired Then
            Dim dOnChanged As New OnChange_Callback(AddressOf OnChanged)
            Me.BeginInvoke(dOnChanged, New Object() {source, e})
        Else
            Dim sCmd As String = String.Empty
            Try
                Select Case e.ChangeType
                    Case WatcherChangeTypes.Changed
                        If My.Computer.FileSystem.FileExists(e.FullPath) Then
                            Dim sr As System.IO.StreamReader = Nothing
                            Try
                                sr = System.IO.File.OpenText(e.FullPath)
                                sCmd = sr.ReadLine
                                subProcessCommandLine(Split(sCmd, " "))
                                Me.BringToFront()
                            Catch ex As Exception
                                Trace.WriteLine(ex.Message)
                                Trace.WriteLine(ex.StackTrace)
                            Finally
                                sr.Close()
                            End Try
                            My.Computer.FileSystem.DeleteFile(e.FullPath)
                        End If
                End Select
            Catch ex As Exception
                Trace.WriteLine(ex.Message)
                Trace.WriteLine(ex.StackTrace)
            End Try
        End If 'Me.InvokeRequired
    End Sub
    Private Sub subPrintData()
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

            webMain.Print()


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)


        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
            btnPrint.Enabled = True
            btnClose.Enabled = True
        End Try
    End Sub
    Private Sub subProcessCommandLine(Optional ByRef sCmd As String() = Nothing)
        '********************************************************************************************
        'Description: If there are command line parameters - process here
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/15/12  HGB     Show only the arms in each zone, not all arms in all zones.
        ' 09/30/13  MSW     subProcessCommandLine - Log files not found for passed in links
        '********************************************************************************************
        Try
            If sCmd Is Nothing Then
                If My.Application.CommandLineArgs.Count > 0 Then
                    Dim nIndex As Integer = 0
                    For Each sItem As String In My.Application.CommandLineArgs
                        ReDim Preserve sCmd(nIndex)
                        sCmd(nIndex) = sItem
                        sCmd(nIndex).Replace("""", "")
                        nIndex = nIndex + 1
                    Next
                Else
                    ReDim sCmd(2)
                    sCmd(0) = gs_HELP_MANUALS ' Add online manuals to the treeview
                    sCmd(1) = gs_HELP_ROBOTS ' Add robot web pages to the treeview
                    sCmd(2) = gs_HELP_PWHELP ' Add PW help to the treeview
                End If
            End If
        Catch ex As Exception
        End Try
        Dim bRobots As Boolean = False
        Dim bManuals As Boolean = False
        Dim bPWHelp As Boolean = False
        Dim sTmpUC As String
        msLaunchScreen = String.Empty
        Dim sCultureArg As String = "/culture="

        'If a culture string has been passed in, set the current culture (display language)
        For Each s As String In sCmd
            Application.DoEvents()
            If s.ToLower.StartsWith(sCultureArg) Then
                Culture = s.Remove(0, sCultureArg.Length)
            Else
                sTmpUC = s.ToUpper
                Select Case sTmpUC
                    Case gs_HELP_MANUALS ' Add online manuals to the treeview
                        bManuals = True
                    Case gs_HELP_ROBOTS ' Add robot web pages to the treeview
                        bRobots = True
                    Case gs_HELP_PWHELP ' Add PW help to the treeview
                        bPWHelp = True
                    Case Else 'Assume it's a file, try to piece it all together, including spaces
                        If msLaunchScreen = String.Empty Then
                            msLaunchScreen = sTmpUC
                        Else
                            msLaunchScreen = msLaunchScreen & " " & sTmpUC
                        End If
                End Select
            End If
        Next
        If (mbManuals = False) And (mbRobots = False) And (mbPWHelp = False) And _
           (bManuals = False) And (bRobots = False) And (bPWHelp = False) Then
            bManuals = True
            bRobots = True
            bPWHelp = True
        End If
        If (mbRobots = False) And (bRobots = True) Then
            'robot web pages 
            'tArmArray = GetArmArray(colZones, tControllerArray)
            'HGB for SA multizone, only grab the arms for the zone
            For Each oZone As clsZone In colZones
                colZones.CurrentZone = oZone.Name
                tArmArray = GetArmArray(oZone, tControllerArray)
                subAddZonetoList()
            Next
            mbRobots = True
        End If
        Application.DoEvents()
        If (mbManuals = False) And (bManuals = True) Then
            'Add manuals to the tree
            Dim oNodes As tNode() = Nothing
            Dim sTable As String = msONLINE_MANUALS
            Dim sPath As String = String.Empty
            If (bGetNodeTreeFromDB(sTable, oNodes) = False) OrElse _
               (GetDefaultFilePath(sPath, eDir.Help, String.Empty, String.Empty) = False) OrElse _
               (bAddNodesToTree(oNodes, sPath, trvwPages.Nodes) = False) Then
                WriteEventToLog(msSCREEN_NAME, (gcsRM.GetString("csDB_ERROR") & sTable))
                Status = gcsRM.GetString("csERROR")
                MessageBox.Show((gcsRM.GetString("csDB_ERROR") & sTable), msSCREEN_NAME, MessageBoxButtons.OK, _
                                 MessageBoxIcon.Exclamation)
            End If
            mbManuals = True
        End If
        Application.DoEvents()
        If (mbPWHelp = False) And (bPWHelp = True) Then
            'add PWIV help to the tree
            Dim oNodes As tNode() = Nothing
            Dim sTable As String = msPW_HELP
            Dim sPath As String = String.Empty
            If (bGetNodeTreeFromDB(sTable, oNodes) = False) OrElse _
               (GetDefaultFilePath(sPath, eDir.Help, String.Empty, String.Empty) = False) OrElse _
               (bAddNodesToTree(oNodes, sPath, trvwPages.Nodes) = False) Then
                WriteEventToLog(msSCREEN_NAME, (gcsRM.GetString("csDB_ERROR") & sTable))
                Status = gcsRM.GetString("csERROR")
                MessageBox.Show((gcsRM.GetString("csDB_ERROR") & sTable), msSCREEN_NAME, MessageBoxButtons.OK, _
                                 MessageBoxIcon.Exclamation)
            End If
            mbPWHelp = True
        End If

        If Not (msLaunchScreen Is Nothing) Then
            If msLaunchScreen <> String.Empty Then
                Dim spath As String = String.Empty
                If GetDefaultFilePath(spath, eDir.Help, String.Empty, msLaunchScreen) Then
                    msLaunchScreen = spath
                Else
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subProcessCommandLine", _
                                           "Can not open link: " & msLaunchScreen)
                    msLaunchScreen = String.Empty
                End If
                msLastPage = msLaunchScreen
                mbSkipFirstTreeViewSelect = True
                mNavigatedCount = 0
                Progress = 1
                webMain.Navigate(msLastPage)
                Debug.Print(msLastPage)
                For Each oNode As Windows.Forms.TreeNode In trvwPages.Nodes
                    If oNode.Tag.ToString.ToLower.Contains(msLaunchScreen.ToLower) Then
                        Debug.Print(oNode.Tag.ToString)
                        oNode.EnsureVisible()
                        oNode.Expand()
                        trvwPages.SelectedNode = oNode
                    End If
                Next
            End If
        End If

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
            webMain.ShowSaveAsDialog()
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
        Dim nBox As Integer = 1
        Dim oT As TextBox = Nothing
        Dim sName As String = String.Empty
        Dim nPtr As Integer = 0

        mbEventBlocker = True


        mbEventBlocker = False

    End Sub

#End Region
#Region " Events "

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '********************************************************************************************
        'Description: Give things a chance to settle out.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Application.DoEvents()

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
        ' 04/20/10  MSW     Easier navigation
        '********************************************************************************************
        Try

            Select Case e.ClickedItem.Name ' case sensitive
                Case "btnClose"
                    'Check to see if we need to save is performed in  bAskForSave
                    Me.Close()
                    Exit Sub
                    'End
                Case "btnStatus"
                    lstStatus.Visible = Not lstStatus.Visible
                    btnStatus.Checked = lstStatus.Visible
                Case "btnSave"
                    'give validate a chance to fire
                    Application.DoEvents()
                    'privilege check done in subroutine
                    subSaveData()
                Case "btnPrint"
                    Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                    If o.DropDownButtonPressed Then Exit Sub
                    subPrintData()
                Case "btnMultiView"
                    'subShowMultiView()
                Case "btnShow"
                    btnShow.Checked = True
                    btnHide.Checked = False
                    splMain.Panel1Collapsed = False
                    tlpRight.RowStyles.Item(0).Height = 35
                    tlpRight.RowStyles.Item(1).Height = 32
                Case "btnHide"
                    btnHide.Checked = True
                    btnShow.Checked = False
                    splMain.Panel1Collapsed = True
                    tlpRight.RowStyles.Item(0).Height = 0
                    tlpRight.RowStyles.Item(1).Height = 0
                Case "btnForward"
                    webMain.GoForward()
                Case "btnBack"
                    webMain.GoBack()
                Case "btnPrevious"
                    'It gets a little convoluted, but this does the trick
                    If trvwPages.SelectedNode Is Nothing Then
                        'Nothing selected, start at the zone.
                        trvwPages.SelectedNode = trvwPages.Nodes(0)
                    ElseIf trvwPages.SelectedNode.PrevVisibleNode Is Nothing Then
                        'Previous visible node will trace back through the branching, so this should only happen with the 1st root-level node.
                        'Does the last root-level node have child nodes?
                        If trvwPages.Nodes(trvwPages.Nodes.Count - 1).Nodes.Count > 0 Then
                            '                                        Last root-level node, to         Last child node of that one
                            '                                        handle multiple zones       
                            trvwPages.SelectedNode = trvwPages.Nodes(trvwPages.Nodes.Count - 1).Nodes(trvwPages.Nodes(trvwPages.Nodes.Count - 1).Nodes.Count - 1)
                        Else
                            'No child nodes, select the last root-level node
                            trvwPages.SelectedNode = trvwPages.Nodes(trvwPages.Nodes.Count - 1)
                        End If
                    Else
                        'If PrevVisibleNode is available, it shortcuts all the logic needed to navigate the branches.
                        'robot nav trick - if it's on a robot, go to the same page on the next one.
                        Dim sTmp As String = webMain.Url.ToString
                        Dim bFromRobot As Boolean = False
                        Dim bToRobot As Boolean = False
                        If tControllerArray IsNot Nothing Then
                            For Each oController As tController In tControllerArray
                                bFromRobot = bFromRobot OrElse (trvwPages.SelectedNode.Name = oController.DisplayName)
                                bToRobot = bToRobot OrElse (trvwPages.SelectedNode.PrevVisibleNode.Name = oController.DisplayName)
                            Next
                        End If
                        If (bFromRobot And bToRobot) Then
                            sTmp = sTmp.ToLower.Replace(trvwPages.SelectedNode.Tag.ToString.ToLower, trvwPages.SelectedNode.PrevVisibleNode.Tag.ToString.ToLower)
                            'Select the next one, but don't navigate
                            mbSkipFirstTreeViewSelect = True
                            lblTitle.Text = trvwPages.SelectedNode.PrevVisibleNode.Text
                            msLastPage = DirectCast(trvwPages.SelectedNode.PrevVisibleNode.Tag, String)
                            mNavigatedCount = 2 'Enable a click immediately
                            trvwPages.SelectedNode = trvwPages.SelectedNode.PrevVisibleNode
                            Progress = 1
                            webMain.Navigate(sTmp)
                            If mbRobots Then
                                subFindTheRobotNode(sTmp)
                            End If
                        Else
                            trvwPages.SelectedNode = trvwPages.SelectedNode.PrevVisibleNode
                        End If
                    End If
                Case "btnNext"
                    If trvwPages.SelectedNode Is Nothing Then
                        'Nothing selected, start with the first zone
                        trvwPages.SelectedNode = trvwPages.Nodes(0)
                    ElseIf trvwPages.SelectedNode.NextVisibleNode Is Nothing Then
                        'No more nodes in this branch, check for child nodes
                        If trvwPages.SelectedNode.Nodes.Count > 0 Then
                            'This node has child nodes, select the first one
                            trvwPages.SelectedNode = trvwPages.SelectedNode.Nodes(0)
                        Else
                            'No child nodes, go back to the parent or top
                            If trvwPages.SelectedNode.Parent IsNot Nothing Then
                                trvwPages.SelectedNode = trvwPages.SelectedNode.Parent
                            Else
                                trvwPages.SelectedNode = trvwPages.Nodes(0)
                            End If
                        End If
                    Else
                        'robot nav trick - if it's on a robot, go to the same page on the next one.
                        Dim sTmp As String = webMain.Url.ToString
                        Dim bFromRobot As Boolean = False
                        Dim bToRobot As Boolean = False
                        If tControllerArray IsNot Nothing Then
                            For Each oController As tController In tControllerArray
                                bFromRobot = bFromRobot OrElse (trvwPages.SelectedNode.Name = oController.DisplayName)
                                bToRobot = bToRobot OrElse (trvwPages.SelectedNode.NextVisibleNode.Name = oController.DisplayName)
                            Next
                        End If
                        If (bFromRobot And bToRobot) Then
                            sTmp = sTmp.ToLower.Replace(trvwPages.SelectedNode.Tag.ToString.ToLower, trvwPages.SelectedNode.NextVisibleNode.Tag.ToString.ToLower)
                            'Select the next one, but don't navigate
                            mbSkipFirstTreeViewSelect = True
                            lblTitle.Text = trvwPages.SelectedNode.NextVisibleNode.Text
                            msLastPage = DirectCast(trvwPages.SelectedNode.NextVisibleNode.Tag, String)
                            mNavigatedCount = 2 'Enable a click immediately
                            trvwPages.SelectedNode = trvwPages.SelectedNode.NextVisibleNode
                            Progress = 1
                            webMain.Navigate(sTmp)
                        Else
                            trvwPages.SelectedNode = trvwPages.SelectedNode.NextVisibleNode
                        End If
                    End If
            End Select
            tlsMain.Refresh()
        Catch ex As Exception

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

            mbEventBlocker = True
            'webMain.Stop()
            'webMain.Dispose()
            'Dim DoneTime As Date = DateTime.Now
            'Me.Visible = False
            'Dim oProcesses() As Process = Process.GetProcesses()
            'For Each oProcess As Process In oProcesses
            '    If InStr(oProcess.ProcessName.ToLower, "adobe") > 0 Then
            '        Debug.Print(oProcess.ProcessName)
            '        oProcess.Kill()
            '    End If
            '    If InStr(oProcess.ProcessName.ToLower, "acro") > 0 Then
            '        Debug.Print(oProcess.ProcessName)
            '        oProcess.Kill()
            '    End If
            'Next
            ''Web browser needs some time, especially if adobe got involved
            'DoneTime = DoneTime.AddSeconds(5)
            'Do While DateTime.Now < DoneTime
            '    Application.DoEvents()
            'Loop
            Dim oThisProcess As Process = Process.GetCurrentProcess
            oThisProcess.Kill()
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
                Privilege = ePrivilege.Execute
                If Privilege = ePrivilege.None Then
                    Privilege = ePrivilege.Edit
                End If
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
    Private Sub mnuPrint_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                    Handles mnuPrint.Click
        '********************************************************************************************
        'Description:  call print routine
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        webMain.ShowPrintPreviewDialog()
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
                    subLaunchHelp(gs_HELP_VIEW_WEB, oIPC)

                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty

                    Dim sName As String = msSCREEN_DUMP_NAME_WEB
                    If mbPWHelp Then
                        sName = msSCREEN_DUMP_NAME_HELP
                    ElseIf mbManuals Then
                        sName = msSCREEN_DUMP_NAME_MANUALS
                    End If

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
            Privilege = ePrivilege.Execute ' extra for buttons etc.
            If Privilege <> ePrivilege.Execute Then
                'didn't have clearance
                Privilege = ePrivilege.Edit
            End If
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
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
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
        End If

    End Sub

#End Region

    Private Sub trvwPages_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles trvwPages.AfterSelect
        '********************************************************************************************
        'Description:  treeview node selected, tell the browser where to go
        '                   
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/20/10  MSW     Easier navigation
        '********************************************************************************************
        If mbEventBlocker Then
            Exit Sub
        End If
        'using two events to catch selections, if they navigate away from a page and click on the same 
        'node we need NodeMouseClick to manage it, keyboard selections use AfterSelect
        Try
            'Don't run this when we're filling the tree
            If mbSkipFirstTreeViewSelect Then
                mbSkipFirstTreeViewSelect = False
                Exit Sub
            End If
            lblTitle.Text = e.Node.Text
            Dim sTmp As String = DirectCast(e.Node.Tag, String)
            'A new keyboard selection will be different.  If they click a new node it will come in 
            'here after NodeMouseClick, so skip it
            If msLastPage <> sTmp Then
                msLastPage = sTmp
                mNavigatedCount = 0
                Progress = 1
                webMain.Navigate(msLastPage)
            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                        Status, MessageBoxButtons.OK)
        End Try
    End Sub

    Private Sub trvwPages_NodeMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles trvwPages.NodeMouseClick
        '********************************************************************************************
        'Description:  treeview node clicked, tell the browser where to go
        '                   
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/20/10  MSW     Easier navigation
        ' 08/15/13  MSW     Fix some occasional errors by setting Progess to 0 instead of 1
        '********************************************************************************************
        'using two events to catch selections, if they navigate away from a page and click on the same 
        'node we need NodeMouseClick to manage it, keyboard selections use AfterSelect
        If mbEventBlocker Then
            Exit Sub
        End If
        Try
            lblTitle.Text = e.Node.Text
            msLastPage = DirectCast(e.Node.Tag, String)
            mNavigatedCount = 0
            Progress = 0
            webMain.Navigate(msLastPage)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                        Status, MessageBoxButtons.OK)
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
        Try
            webMain.ShowPageSetupDialog()
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                        Status, MessageBoxButtons.OK)
        End Try

    End Sub

    Private Sub mnuPrintPreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintPreview.Click
        '********************************************************************************************
        'Description:  show print preview
        '
        'Parameters: none
        'Returns:    Print preview
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            webMain.ShowPrintPreviewDialog()
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                        Status, MessageBoxButtons.OK)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                        Status, MessageBoxButtons.OK)
        End Try
    End Sub

    Private Sub webMain_CanGoBackChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles webMain.CanGoBackChanged
        '********************************************************************************************
        'Description:  web page controls
        '
        'Parameters: none
        'Returns:    Print preview
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            btnBack.Enabled = webMain.CanGoBack
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                        Status, MessageBoxButtons.OK)
        End Try

    End Sub

    Private Sub webMain_CanGoForwardChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles webMain.CanGoForwardChanged
        '********************************************************************************************
        'Description:  web page controls
        '
        'Parameters: none
        'Returns:    Print preview
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            btnForward.Enabled = webMain.CanGoForward
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
        End Try

    End Sub

    Private Sub webMain_DocumentCompleted(ByVal sender As Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles webMain.DocumentCompleted
        '********************************************************************************************
        'Description:  web page controls
        '
        'Parameters: none
        'Returns:    Print preview
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Progress = 0
            DataLoaded = True
            cboAddress.Text = webMain.Url.ToString
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
        End Try

    End Sub

    Private Sub webMain_PreviewKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles webMain.PreviewKeyDown
        '********************************************************************************************
        'Description: Trap alt - key  combinations to simulate menu button clicks
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '    06/04/12   MSW     webMain_PreviewKeyDown - form keydown wasn't getting called when the browser was active
		'    09/30/13   MSW     Save screenshots as jpegs
        '********************************************************************************************
        Dim sKeyValue As String = String.Empty

        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode
                Case Keys.F1
                    'Help Screen request
                    sKeyValue = "btnHelp"
                    subLaunchHelp(gs_HELP_VIEW_WEB, oIPC)

                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty

                    Dim sName As String = msSCREEN_DUMP_NAME_WEB
                    If mbPWHelp Then
                        sName = msSCREEN_DUMP_NAME_HELP
                    ElseIf mbManuals Then
                        sName = msSCREEN_DUMP_NAME_MANUALS
                    End If

                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath & sName, Imaging.ImageFormat.Jpeg)

                Case Keys.Escape
                    Me.Close()

                Case Else

            End Select
        End If
    End Sub

    Private Sub webMain_ProgressChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.WebBrowserProgressChangedEventArgs) Handles webMain.ProgressChanged
        '********************************************************************************************
        'Description:  web page controls
        '
        'Parameters: none
        'Returns:    Print preview
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/15/13  MSW     Fix some occasional errors by setting Progess to 0 instead of 1
        ' 02/03/14  RJO     Fix some recurring errors by making sure that mnMaxProgress is never set to 0.
        '********************************************************************************************
        Try
            If mnMaxProgress <> e.MaximumProgress Then
                If CInt(e.MaximumProgress) > 0 Then mnMaxProgress = CInt(e.MaximumProgress) 'RJO 02/03/14
                tspProgress.Maximum = mnMaxProgress
            End If
            If e.CurrentProgress = 0 Or e.MaximumProgress = 0 Then
                Progress = 0
            Else
                Progress = CInt(100 * e.CurrentProgress / e.MaximumProgress)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
        End Try

    End Sub

    Private Sub webMain_StatusTextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles webMain.StatusTextChanged
        '********************************************************************************************
        'Description:  web page controls
        '
        'Parameters: none
        'Returns:    Print preview
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/01/12  MSW     dealing with 64 bit windows quirks 
        '********************************************************************************************
        Try
            If webMain IsNot Nothing AndAlso webMain.StatusText IsNot Nothing Then
                Status = webMain.StatusText
            End If
        Catch ex As Exception
            'Win7 64 bit bombs out here sometimes.  Other times it works, so I'm going to let the error 
            ' handling deal with it.  No log, though
            'Trace.WriteLine(ex.Message)
            'Trace.WriteLine(ex.StackTrace)
        End Try

    End Sub
    Private Sub subFindTheRobotNode(ByVal sPage As String)
        '********************************************************************************************
        'Description:  web page controls
        '
        'Parameters: none
        'Returns:    Print preview
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bDone As Boolean = False
        'See if they found another robot
        mbEventBlocker = True
        For Each oController As tController In tControllerArray
            Debug.Print(sPage.ToLower & "  :  " & oController.HostName.ToLower)
            If InStr(sPage.ToLower, oController.HostName.ToLower) > 0 Then
                'Select this robot
                For Each oNode As TreeNode In trvwPages.Nodes
                    If oNode.Name = oController.DisplayName Then
                        lblTitle.Text = oNode.Text
                        trvwPages.SelectedNode = oNode
                        bDone = True
                        Exit For
                    Else
                        'Search one extra layer to get to the robots
                        For Each oNode2 As TreeNode In oNode.Nodes
                            If oNode2.Name = oController.DisplayName Then
                                lblTitle.Text = oNode2.Text
                                trvwPages.SelectedNode = oNode2
                                Exit For
                            End If
                        Next
                        If bDone Then
                            Exit For
                        End If
                    End If
                Next
            End If
            If bDone Then
                Exit For
            End If
        Next
        mbEventBlocker = False

    End Sub
    Private Sub subNavigateCboAddress(ByVal sText As String)
        '********************************************************************************************
        'Description:  web page controls
        '
        'Parameters: none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbEventBlocker = True
        msLastPage = sText
        mNavigatedCount += 1
        Progress = 1
        webMain.Navigate(msLastPage)
        If mbRobots Then
            subFindTheRobotNode(msLastPage)
        End If
        cboAddress.Items.Insert(0, msLastPage)
        For nIdx1 As Integer = cboAddress.Items.Count - 1 To 0 Step -1
            Dim sTmp As String = cboAddress.Items(nIdx1).ToString.ToLower
            For nIdx2 As Integer = 0 To nIdx1 - 1
                If sTmp = cboAddress.Items(nIdx2).ToString.ToLower Then
                    cboAddress.Items.RemoveAt(nIdx1)
                    Exit For
                End If
            Next
        Next
        mbEventBlocker = False
    End Sub
    Private Sub cboAddress_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles cboAddress.KeyDown
        '********************************************************************************************
        'Description:  web page controls
        '
        'Parameters: none
        'Returns:    Print preview
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEventBlocker Then
            Exit Sub
        End If
        If e.KeyCode = Keys.Enter Then
            subNavigateCboAddress(cboAddress.Text)
        End If
    End Sub

    Private Sub cboAddress_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboAddress.SelectedIndexChanged
        '********************************************************************************************
        'Description:  web page controls
        '
        'Parameters: none
        'Returns:    Print preview
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEventBlocker Then
            Exit Sub
        End If
        'subNavigateCboAddress(cboAddress.Text)
    End Sub

End Class