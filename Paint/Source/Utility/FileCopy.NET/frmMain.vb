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
' Description: File Copy Screen
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
' 09/22/2009    MSW     first draft
' 11/11/09      MSW     subEnableControls - revise enables for command buttons
' 10/11/10      MSW     add missing psSCREENCAPTION	to ProjectStrings.resx
' 09/14/11      MSW     Assemble a standard version of everything                       4.1.0.0
'    12/02/11   MSW     Add Dmoncfg reference                                            4.1.1.0
'    01/18/12   MSW     Clean up old printsettings object                               4.1.1.1
'    01/24/12   MSW     Placeholder include updates                                   4.01.01.02
'    02/15/12   MSW     Force 32 bit build, print management updates                  4.01.01.03
'    03/23/12   RJO     modified for .NET Password and IPC                            4.01.02.00
'    04/11/12   MSW     Change CommonStrings setup so it builds correctly             4.01.03.00
'    04/11/12   MSW     Change compare form so it's OK with the new commonstrings setup 4.01.03.01
'    06/07/12   MSW     frmMain_KeyDown-Simplify help links                           4.01.03.02
'    06/12/12   MSW     Add file device to change log for copy from the robots         4.01.03.03
'    10/24/12   RJO     Added StartupModule to project to prevent multiple instances  4.01.03.04
'    01/03/13   RJO     Added Copy Fail status to subCopyToControllers and            4.01.03.05
'                       subCopyFromControllers if robot is offline.
'    04/16/13   MSW     Add Canadian language files                                   4.01.05.00
'                       Standalone Changelog
'    06/01/13   AC      Bug fix to SubCopyToControllers.                              4.01.05.01
'    07/09/13   MSW     Update and standardize logos                                  4.01.05.02
'    07/11/13   MSW     Track errors for improved status display                      4.01.05.03
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up  4.01.05.04
'    09/30/13   MSW     Save screenshots as jpegs                                     4.01.06.00
'    01/07/14   MSW     Remove ControlBox from main form                              4.01.06.01
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call              4.01.07.00
'    04/21/14   MSW     SubCopyToControllers - Fix some lengths for file type checking 4.01.07.01
'    08/05/14   MSW     subProcessSourceSelection - Short file name fix from inovision 4.01.07.02
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On
Imports System.IO
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Configuration.ConfigurationSettings
Imports Response = System.Windows.Forms.DialogResult
Imports ConnStat = FRRobotNeighborhood.FRERNConnectionStatusConstants
'dll for zip access: C:\paint\Vbapps\Ionic.Zip.dll
'See License at C:\paint\Source\Common.NET\DotNetZip License.txt
'Website http://dotnetzip.codeplex.com/
Imports Ionic.Zip

Friend Class frmMain
#Region " Declares "

    '******** Form Constants   **************************************jpg********************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    'Not a constant here, we'll try to get help in the same executable
    Friend Const msSCREEN_NAME As String = "FileCopy"   ' <-- For password area change log etc.
    Private Const msSCREEN_DUMP_NAME As String = "Utility_FileCopy_Main.bmp"
    Private Const msSCREEN_DUMP_NAME_PRE As String = "Utility_FileCopy_"
    Private Const msSCREEN_DUMP_NAME_EXT As String = ".jpg"
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
    Private mcolTempFolders As New Collection(Of String) 'track temp folders for celanup
    Private mbCancel As Boolean = False
    Private mbCopyActive As Boolean = False
    Private nTmpFileIndex As Integer = 0 'In case there are access problems, add to the file name
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean = False
    Private mnProgress As Integer = 0
    Private msCulture As String = "en-US" 'Default to english
    '******** End Property Variables    *************************************************************

    '******** This is the old pw3 password object interop  *****************************************
    'Friend WithEvents moPassword As PWPassword.PasswordObject 'RJO 03/23/12
    'Friend moPrivilege As PWPassword.cPrivilegeObject 'RJO 03/23/12

    Friend WithEvents moPassword As clsPWUser 'RJO 03/23/12
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm 'RJO 03/23/12

    'Custom password actions
    Private Enum eScreenPrivilege As Integer
        None = 0            'No privilege at all, logged out
        SaveBackup = 1      'Save or backup files
        BrowseFolder = 2    'Browse to a source or destination folder
        RestoreTP = 3       'Restore files to the robot
        RestoreVAR = 4      'Restore variables (vr,sv,io) to the robot
        Delete = 4          'Delete files
        MasterBackups = 4   'Write to master backups
        RobotDevices = 5    'Select alternate robot devices
        'Used for array length, so if you add one keep SuperUser at the end
        SuperUser = 6
    End Enum
    Private mScreenPrivilege(eScreenPrivilege.SuperUser) As Boolean
    Private msActions(eScreenPrivilege.SuperUser) As String
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None 'Keep the common password settings handy for shared code
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None 'Keep the common password settings handy for shared code

    '******** This is the old pw3 password object interop  *****************************************

    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet)  'RJO 03/23/12
    Delegate Sub ControllerStatusChange(ByVal Controller As clsController)

    Private mnMaxProgress As Integer = 100
    Private mPrintHtml As  clsPrintHtml
    Private moMouseDownObject As Object
    Private mnPopupMenuItem As Integer = -1
    Private mbRobotNotAvailable() As Boolean = Nothing
    Private mnSourceSelctAFolderCboIndex As Integer = -1
    Private mnSourceSelctAZipCboIndex As Integer = -1
    Private mnSourcePWBackupsIndex As Integer = -1
    Private mnSourceBrowsedFoldersCboIndex As Integer = 999
    Private mnDestSelctAFolderCboIndex As Integer = -1
    Private mndestSelctAZipCboIndex As Integer = -1
    Private mnDestPWBackupsIndex As Integer = -1
    Private mnDestBrowsedFoldersCboIndex As Integer = 999
    Private mbProcessSourceSelection As Boolean = True
    Private msSourceFolder As String = String.Empty
    Private msSourceSubFolders() As String = Nothing
    Private mnSourceSubFolderCount As Integer = 0
    Private Const msROBOTS_TAG As String = "#ROBOTS#"
    Private mbReadyToCopy As Boolean = False
    Private WithEvents mRtbPrint As RichTextBox = Nothing
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

    'This screen uses custom password levels, but leave the old property in there for the changelog
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
            'Check them all at once
            'Added custom password levels (screenPrivilege)
            'this is needed with old password object raising events
            'and can hopefully be removed when password is redone
            'Control.CheckForIllegalCrossThreadCalls = False

            mPrivilegeRequested = Value

            'handle logout
            If mPrivilegeRequested = ePrivilege.None Then
                mPrivilegeGranted = ePrivilege.None
                For Each bTmp As Boolean In mScreenPrivilege
                    bTmp = False
                Next
                mScreenPrivilege(eScreenPrivilege.None) = True
                subUpdateCbosByPassword()
                subEnableControls(True)
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
                'this is needed with old password object raising events
                'and can hopefully be removed when password is redone
                'Control.CheckForIllegalCrossThreadCalls = True
                Exit Property
            End If

            'prevent recursion
            If mPrivilegeGranted >= mPrivilegeRequested Then
                'this is needed with old password object raising events
                'and can hopefully be removed when password is redone
                'Control.CheckForIllegalCrossThreadCalls = True
                Exit Property
            End If
            'Get the custom actions
            If moPassword.UserName = String.Empty Then
                For Each bTmp As Boolean In mScreenPrivilege
                    bTmp = False
                Next
                mScreenPrivilege(eScreenPrivilege.None) = True
            Else
                'Check for super user first
                'moPrivilege.ScreenName = msSCREEN_NAME  'RJO 03/23/12
                'moPrivilege.Privilege = msActions(eScreenPrivilege.SuperUser)
                'mScreenPrivilege(eScreenPrivilege.SuperUser) = moPrivilege.ActionAllowed
                'mScreenPrivilege(eScreenPrivilege.None) = Not (mScreenPrivilege(eScreenPrivilege.SuperUser))
                'For nIndex As Integer = eScreenPrivilege.None + 1 To eScreenPrivilege.SuperUser - 1
                '    If mScreenPrivilege(eScreenPrivilege.SuperUser) Then
                '        mScreenPrivilege(nIndex) = True
                '    Else
                '        moPrivilege.Privilege = msActions(nIndex)
                '        mScreenPrivilege(nIndex) = moPrivilege.ActionAllowed
                '        If mScreenPrivilege(nIndex) Then
                '            mScreenPrivilege(eScreenPrivilege.None) = False
                '        End If
                '    End If
                'Next

                'Revised code above for .NET Password 'RJO 03/23/12
                mScreenPrivilege(eScreenPrivilege.SuperUser) = moPassword.CheckPassword(msActions(eScreenPrivilege.SuperUser))
                mScreenPrivilege(eScreenPrivilege.None) = Not (mScreenPrivilege(eScreenPrivilege.SuperUser))

                For nIndex As Integer = eScreenPrivilege.None + 1 To eScreenPrivilege.SuperUser - 1
                    If mScreenPrivilege(eScreenPrivilege.SuperUser) Then
                        mScreenPrivilege(nIndex) = True
                    Else
                        mScreenPrivilege(nIndex) = moPassword.CheckPassword(msActions(nIndex))
                        If mScreenPrivilege(nIndex) Then
                            mScreenPrivilege(eScreenPrivilege.None) = False
                        End If
                    End If
                Next 'nIndex

                subUpdateCbosByPassword()
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
    Private Sub subEnableControls(ByVal bEnable As Boolean, _
                                  Optional ByVal nChkSourceOride As Integer = -1, _
                                  Optional ByVal nChkDestOride As Integer = -1, _
                                  Optional ByVal nChkFilesOride As Integer = -1, _
                                  Optional ByVal bNoSummaryRefresh As Boolean = False, _
                                  Optional ByVal bCancelOnly As Boolean = False)
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
        '********************************************************************************************
        Dim bRestOfControls As Boolean = False
        Dim bEditsMade As Boolean = False
        btnClose.Enabled = True
        Dim bFilesPanel As Boolean = False
        Dim bDestPanel As Boolean = False
        Dim bDeleteFiles As Boolean = btnDeleteFiles.Checked
        Dim bReadyToCopy As Boolean = False

        'Determine if it's ready to copy regardless of enable argument or login
        'Item check events pass in a number for these because the property isn't updated yet
        If nChkSourceOride = -1 Then
            nChkSourceOride = chkSourceRobots.CheckedItems.Count
        End If
        If nChkDestOride = -1 Then
            nChkDestOride = chkDestRobots.CheckedItems.Count
        End If
        'Stumble through the tabs and enable based on what is selected
        If (((cboSourceType.SelectedIndex >= 0) And (nChkSourceOride > 0)) Or _
            (cboSourceType.SelectedIndex >= mnSourceBrowsedFoldersCboIndex)) And _
           (rdoSourceSelectFiles.Checked Or rdoSourceAllFiles.Checked) Then
            'Source selected, allow file select tab if not "All files selcted"
            bFilesPanel = rdoSourceSelectFiles.Checked
            If (nChkFilesOride = -1) And bFilesPanel Then
                nChkFilesOride = chkFiles.CheckedItems.Count
            End If
            'Allow dest tab if all files selected or some files selected and not in delete mode
            bDestPanel = ((bFilesPanel = False) Or (nChkFilesOride > 0)) And (bDeleteFiles = False)
            'Allow Do Copy tab if dest selected or in delete files mode and files are selected
            If ((cboDestType.SelectedIndex >= 0) And (nChkDestOride > 0)) Or _
               (bDeleteFiles And (nChkFilesOride > 0)) Then
                'identical check is counterproductive here, maybe we can do it at the time of the copy?
                mbReadyToCopy = True
                ''check for identical source and destination, prevent copy to itself
                'If (cboDestType.Text <> cboSourceType.Text) Or _
                '   (nChkSourceOride <> nChkDestOride) Then
                '    '(chkDestRobots.CheckedItems <> chkSourceRobots.CheckedItems) Then
                '    mbReadyToCopy = True
                'Else
                '    mbReadyToCopy = False
                'End If
            Else
                mbReadyToCopy = False
            End If
        Else
            bFilesPanel = False
            bDestPanel = False
            mbReadyToCopy = False
        End If
        '
        If bEnable = False Then
            btnChangeLog.Enabled = False
            btnDeleteFiles.Enabled = False
            btnUtilities.Enabled = False
            bRestOfControls = False
            bFilesPanel = False
            bDestPanel = False
            bReadyToCopy = False
        Else
            Select Case Privilege
                Case ePrivilege.None
                    btnChangeLog.Enabled = True
                    btnDeleteFiles.Enabled = True
                    btnUtilities.Enabled = True
                    bRestOfControls = True
                    bFilesPanel = True
                    bDestPanel = True
                    bReadyToCopy = False
                Case Else
                    btnChangeLog.Enabled = True
                    bReadyToCopy = mbReadyToCopy
                    If btnDeleteFiles.Checked Then
                        bReadyToCopy = bReadyToCopy And mScreenPrivilege(eScreenPrivilege.Delete)
                    Else
                        Select Case cboDestType.Text
                            'Standard destinations
                            Case gpsRM.GetString("psROBOT_CONTROLLERS")
                                bReadyToCopy = bReadyToCopy And mScreenPrivilege(eScreenPrivilege.RestoreTP) Or mScreenPrivilege(eScreenPrivilege.RestoreVAR)
                                If cboDestRobotDev.Text.Substring(0, 2).ToLower <> "md" Then
                                    'Alternate robot devices
                                    bReadyToCopy = bReadyToCopy And mScreenPrivilege(eScreenPrivilege.RobotDevices)
                                End If
                            Case gpsRM.GetString("psTEMP_BACKUPS")
                                bReadyToCopy = bReadyToCopy And mScreenPrivilege(eScreenPrivilege.SaveBackup)
                            Case gpsRM.GetString("psMASTER_BACKUPS")
                                bReadyToCopy = bReadyToCopy And mScreenPrivilege(eScreenPrivilege.MasterBackups)
                            Case Else 'browsed folder
                                bReadyToCopy = mScreenPrivilege(eScreenPrivilege.BrowseFolder)
                        End Select
                    End If
                    If cboSourceType.Text = gpsRM.GetString("psROBOT_CONTROLLERS") Then
                        If cboSourceRobotDev.Text.Substring(0, 2).ToLower <> "md" Then
                            'Alternate robot devices
                            bReadyToCopy = bReadyToCopy And mScreenPrivilege(eScreenPrivilege.RobotDevices)
                        End If
                    End If
                    btnDeleteFiles.Enabled = True
                    btnUtilities.Enabled = True
                    bRestOfControls = True
            End Select
        End If
        btnSourceNext.Enabled = (bFilesPanel And rdoSourceSelectFiles.Checked) Or (bDestPanel And rdoSourceAllFiles.Checked)
        chkSourceRobots.Enabled = cboSourceType.SelectedIndex > -1
        cboSourceRobotDev.Visible = (cboSourceType.Text = gpsRM.GetString("psROBOT_CONTROLLERS"))
        lblSourceRobotDev.Visible = cboSourceRobotDev.Visible
        cboSourceRobotDev.Enabled = bRestOfControls And (cboSourceType.Text = gpsRM.GetString("psROBOT_CONTROLLERS"))
        cboDestRobotDev.Visible = (cboDestType.Text = gpsRM.GetString("psROBOT_CONTROLLERS"))
        lblDestRobotDev.Visible = cboDestRobotDev.Visible
        cboDestRobotDev.Enabled = bRestOfControls And (cboDestType.Text = gpsRM.GetString("psROBOT_CONTROLLERS")) And _
                                mScreenPrivilege(eScreenPrivilege.RobotDevices)
        btnFilesPrev.Enabled = bRestOfControls
        btnFilesNext.Enabled = bDestPanel Or (bDeleteFiles And (nChkFilesOride > 0))
        btnDestPrev.Enabled = bRestOfControls
        btnDestNext.Enabled = mbReadyToCopy
        btnDoCopyPrev.Enabled = bRestOfControls
        btnDoCopy.Enabled = bReadyToCopy Or bCancelOnly
        cboZone.Enabled = bRestOfControls
        tabMain.Enabled = bRestOfControls Or bCancelOnly
        tabFiles.Enabled = bFilesPanel
        tabDest.Enabled = bDestPanel
        tabDoCopy.Enabled = bRestOfControls Or bCancelOnly
        chkDestRobots.Enabled = bDestPanel And cboDestType.SelectedIndex > -1 And _
                                (mnSourceSubFolderCount = 1)
        tabFiles.Enabled = bFilesPanel
        tabDest.Enabled = bDestPanel
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
    Private Sub subUpdateCbosByPassword()
        '********************************************************************************************
        'Description: setup cbo choices by password
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbEventBlocker = True
        With gpsRM
            'Source and destination folders/robots
            'Clear out unwanted selections
            If Not (mScreenPrivilege(eScreenPrivilege.BrowseFolder)) Then
                'Always have the first 3 items (Robots, temp, master backups)
                For nIndex As Integer = (cboSourceType.Items.Count - 1) To 3 Step -1
                    If (cboSourceType.Items(nIndex).ToString = .GetString("psPW_BACKUPS")) Or _
                        (cboSourceType.Items(nIndex).ToString = .GetString("psSELECT_A_FOLDER")) Or _
                        (cboSourceType.Items(nIndex).ToString = .GetString("psSELECT_A_ZIP")) Then
                        cboSourceType.Items.RemoveAt(nIndex)
                    End If
                Next
            End If
            For nIndex As Integer = (cboDestType.Items.Count - 1) To 0 Step -1
                If (Not (mScreenPrivilege(eScreenPrivilege.BrowseFolder))) And _
                    ((cboDestType.Items(nIndex).ToString = .GetString("psPW_BACKUPS")) Or _
                    (cboDestType.Items(nIndex).ToString = .GetString("psSELECT_A_FOLDER")) Or _
                        (cboDestType.Items(nIndex).ToString = .GetString("psSELECT_A_ZIP"))) Then
                    cboDestType.Items.RemoveAt(nIndex)
                ElseIf (Not (mScreenPrivilege(eScreenPrivilege.RestoreTP) Or mScreenPrivilege(eScreenPrivilege.RestoreVAR))) And _
                        (cboDestType.Items(nIndex).ToString = .GetString("psROBOT_CONTROLLERS")) Then
                    cboDestType.Items.RemoveAt(nIndex)
                ElseIf (Not (mScreenPrivilege(eScreenPrivilege.MasterBackups))) And _
                    (cboDestType.Items(nIndex).ToString = .GetString("psMASTER_BACKUPS")) Then
                    cboDestType.Items.RemoveAt(nIndex)
                End If
            Next
            'Check for missing items
            Dim nCount As Integer = 0
            If (mScreenPrivilege(eScreenPrivilege.RestoreTP) Or mScreenPrivilege(eScreenPrivilege.RestoreVAR)) Then
                If (cboDestType.Items.Count <= nCount) Then
                    cboDestType.Items.Add(.GetString("psROBOT_CONTROLLERS"))
                ElseIf (cboDestType.Items(nCount).ToString <> .GetString("psROBOT_CONTROLLERS")) Then
                    cboDestType.Items.Insert(nCount, .GetString("psROBOT_CONTROLLERS"))
                End If
                nCount += 1
            End If
            nCount += 1 'Always have temp backups enabled, count past them here
            If mScreenPrivilege(eScreenPrivilege.MasterBackups) Then
                If (cboDestType.Items.Count <= nCount) Then
                    cboDestType.Items.Add(.GetString("psMASTER_BACKUPS"))
                ElseIf (cboDestType.Items(nCount).ToString <> .GetString("psMASTER_BACKUPS")) Then
                    cboDestType.Items.Insert(nCount, .GetString("psMASTER_BACKUPS"))
                End If
                nCount += 1
            End If
            If mScreenPrivilege(eScreenPrivilege.BrowseFolder) Then
                If (cboDestType.Items.Count <= nCount) Then
                    cboDestType.Items.Add(.GetString("psPW_BACKUPS"))
                    cboDestType.Items.Add(.GetString("psSELECT_A_FOLDER"))
                    cboDestType.Items.Add(.GetString("psSELECT_A_ZIP"))
                ElseIf (cboDestType.Items(nCount).ToString <> .GetString("psPW_BACKUPS")) Then
                    cboDestType.Items.Insert(nCount, .GetString("psPW_BACKUPS"))
                    cboDestType.Items.Insert(nCount + 1, .GetString("psSELECT_A_FOLDER"))
                    cboDestType.Items.Insert(nCount + 1, .GetString("psSELECT_A_ZIP"))
                End If
                mnDestPWBackupsIndex = nCount
                nCount += 1
                mnDestSelctAFolderCboIndex = nCount
                mnDestSelctAZipCboIndex = nCount + 1
                mnDestBrowsedFoldersCboIndex = mnDestSelctAZipCboIndex + 1
                If (cboSourceType.Items.Count <= 3) Then
                    cboSourceType.Items.Add(.GetString("psPW_BACKUPS"))
                    cboSourceType.Items.Add(.GetString("psSELECT_A_FOLDER"))
                    cboSourceType.Items.Add(.GetString("psSELECT_A_ZIP"))
                ElseIf (cboSourceType.Items(3).ToString <> .GetString("psPW_BACKUPS")) Then
                    cboSourceType.Items.Insert(3, .GetString("psPW_BACKUPS"))
                    cboSourceType.Items.Insert(4, .GetString("psSELECT_A_FOLDER"))
                    cboSourceType.Items.Insert(5, .GetString("psSELECT_A_ZIP"))
                End If
                mnSourcePWBackupsIndex = 3
                mnSourceSelctAFolderCboIndex = 4
                mnSourceSelctAZipCboIndex = 5
                mnDestBrowsedFoldersCboIndex = 6
            Else
                mnDestPWBackupsIndex = -1
                mnDestSelctAFolderCboIndex = -1
                mnDestSelctAZipCboIndex = -1
                mnDestBrowsedFoldersCboIndex = nCount
                mnSourcePWBackupsIndex = -1
                mnSourceSelctAFolderCboIndex = -1
                mnSourceSelctAZipCboIndex = -1
                mnDestBrowsedFoldersCboIndex = 3
            End If
            'cboSourceType.SelectedIndex = -1
            'cboDestType.SelectedIndex = -1
            'cboSourceType.Items.Clear()
            'cboDestType.Items.Clear()
            'cboSourceType.Items.Add(.GetString("psROBOT_CONTROLLERS"))
            'cboDestType.Items.Add(.GetString("psROBOT_CONTROLLERS"))
            'cboSourceType.Items.Add(.GetString("psTEMP_BACKUPS"))
            'cboDestType.Items.Add(.GetString("psTEMP_BACKUPS"))
            'cboSourceType.Items.Add(.GetString("psMASTER_BACKUPS"))
            'cboDestType.Items.Add(.GetString("psMASTER_BACKUPS"))
            'mnPWBackupsIndex = cboSourceType.Items.Count
            'cboSourceType.Items.Add(.GetString("psPW_BACKUPS"))
            'cboDestType.Items.Add(.GetString("psPW_BACKUPS"))
            'mnSelctAFolderCboIndex = cboSourceType.Items.Count
            'cboSourceType.Items.Add(.GetString("psSELECT_A_FOLDER"))
            'cboDestType.Items.Add(.GetString("psSELECT_A_FOLDER"))

            'Robot devices
            If cboSourceRobotDev.Items.Count > 2 And Not (mScreenPrivilege(eScreenPrivilege.RobotDevices)) Then
                If cboSourceRobotDev.SelectedIndex > 1 Then
                    cboSourceRobotDev.SelectedIndex = 0
                End If
                cboSourceRobotDev.Items.Remove(.GetString("psFRA"))
                cboSourceRobotDev.Items.Remove(.GetString("psRD"))
                cboSourceRobotDev.Items.Remove(.GetString("psMC"))
                cboSourceRobotDev.Items.Remove(.GetString("psUD1"))
                cboSourceRobotDev.Items.Remove(.GetString("psUT1"))
                cboSourceRobotDev.Items.Remove(.GetString("psFR"))
                If cboSourceRobotDev.SelectedIndex > 0 Then
                    cboSourceRobotDev.SelectedIndex = 0
                End If
                cboDestRobotDev.Items.Remove(.GetString("psRD"))
                cboDestRobotDev.Items.Remove(.GetString("psMC"))
                cboDestRobotDev.Items.Remove(.GetString("psUD1"))
                cboDestRobotDev.Items.Remove(.GetString("psUT1"))
                cboDestRobotDev.Items.Remove(.GetString("psFR"))
            End If
            If cboSourceRobotDev.Items.Count = 2 And mScreenPrivilege(eScreenPrivilege.RobotDevices) Then
                cboSourceRobotDev.Items.Add(.GetString("psFRA"))
                cboSourceRobotDev.Items.Add(.GetString("psRD"))
                cboSourceRobotDev.Items.Add(.GetString("psMC"))
                cboSourceRobotDev.Items.Add(.GetString("psUD1"))
                cboSourceRobotDev.Items.Add(.GetString("psUT1"))
                cboSourceRobotDev.Items.Add(.GetString("psFR"))
                cboDestRobotDev.Items.Add(.GetString("psRD"))
                cboDestRobotDev.Items.Add(.GetString("psMC"))
                cboDestRobotDev.Items.Add(.GetString("psUD1"))
                cboDestRobotDev.Items.Add(.GetString("psUT1"))
                cboDestRobotDev.Items.Add(.GetString("psFR"))
            End If

            mbEventBlocker = False
        End With
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
        ' 08/23/10  MSW     Aff image files
        '********************************************************************************************


        lblZone.Text = gcsRM.GetString("csZONE_CAP")
        mnuSelectAll.Text = gcsRM.GetString("csSELECT_ALL")
        mnuUnselectAll.Text = gcsRM.GetString("csUNSELECT_ALL")
        With gpsRM
            btnSourceNext.Text = .GetString("psNEXT")
            btnDestNext.Text = .GetString("psNEXT")
            btnFilesNext.Text = .GetString("psNEXT")
            btnDestPrev.Text = .GetString("psBACK")
            btnFilesPrev.Text = .GetString("psBACK")
            btnDoCopyPrev.Text = .GetString("psBACK")
            btnDoCopy.Text = .GetString("psDO_COPY")
            btnDeleteFiles.Text = .GetString("psDELETE_FILES")
            btnUtilities.Text = .GetString("psUTILITIES")
            mnuBackupAllTemp.Text = .GetString("psBACKUP_ALL_TO") & gpsRM.GetString("psTEMP_BACKUPS")
            mnuBackupAllMaster.Text = .GetString("psBACKUP_ALL_TO") & gpsRM.GetString("psMASTER_BACKUPS")
            mnuBackupAllTempWithTextFiles.Text = .GetString("psBACKUP_ALL_TO") & gpsRM.GetString("psTEMP_BACKUPS") & gpsRM.GetString("ps_WITH_TEXT")
            mnuBackupAllMasterWithTextFiles.Text = .GetString("psBACKUP_ALL_TO") & gpsRM.GetString("psMASTER_BACKUPS") & gpsRM.GetString("ps_WITH_TEXT")
            mnuGetDiag.Text = .GetString("psGET_DIAG")
            mnuBackupTextFiles.Text = .GetString("psBACKUP_TEXT")
            mnuCompare.Text = .GetString("psVIEW_COMPARE")
            tabSource.Text = .GetString("psSOURCE")
            tabFiles.Text = .GetString("psFILES")
            tabDest.Text = .GetString("psDEST")
            tabDoCopy.Text = .GetString("psDO_COPY")
            lblFileSelect.Text = .GetString("psSELECT_FILES")
            lblSourceType.Text = .GetString("psSOURCE")
            rdoSourceAllFiles.Text = .GetString("psALL_FILES")
            rdoSourceSelectFiles.Text = .GetString("psSELECT_FILES")
            lblSourceFileType.Text = .GetString("psFILE_TYPE_EXP")
            lblSourceRobotDev.Text = .GetString("psROBOT_DEV_EXP")
            lblDestRobotDev.Text = .GetString("psROBOT_DEV_EXP")
            lblDestType.Text = .GetString("psDEST")
            chkOverWriteFiles.Text = .GetString("psOVERWRITE_FILES")

            mnuViewCompare.Text = .GetString("psVIEW_COMPARE")
            mnuPrint.Text = .GetString("psPRINT")
            frmOverWrite.caption = .GetString("psOVERWRITE_FILES")
            frmOverWrite.btnNo.Text = .GetString("psNO")
            frmOverWrite.btnYes.Text = .GetString("psYES")
            frmOverWrite.btnYesAll.Text = .GetString("psYES_TO_ALL")

            mbEventBlocker = True

            cboSourceFileType.SelectedIndex = -1
            cboSourceFileType.Items.Clear()
            cboSourceFileType.Items.Add(.GetString("psALL_FILE_MASK"))
            cboSourceFileType.Items.Add(.GetString("psTP_FILE_MASK"))
            cboSourceFileType.Items.Add(.GetString("psVR_FILE_MASK"))
            cboSourceFileType.Items.Add(.GetString("psSV_FILE_MASK"))
            cboSourceFileType.Items.Add(.GetString("psIO_FILE_MASK"))
            cboSourceFileType.Items.Add(.GetString("psDG_FILE_MASK"))
            cboSourceFileType.Items.Add(.GetString("psLS_FILE_MASK"))
            cboSourceFileType.Items.Add(.GetString("psVA_FILE_MASK"))
            cboSourceFileType.Items.Add(.GetString("psDT_FILE_MASK"))
            cboSourceFileType.Items.Add(.GetString("psIMG_FILE_MASK"))
            cboSourceFileType.Items.Add(.GetString("psTEXT_FILE_MASK"))
            cboSourceFileType.SelectedIndex = 0

            cboSourceRobotDev.SelectedIndex = -1
            cboSourceRobotDev.Items.Clear()
            cboSourceRobotDev.Items.Add(.GetString("psMDB"))
            cboSourceRobotDev.Items.Add(.GetString("psMD"))
            cboSourceRobotDev.Items.Add(.GetString("psFRA"))
            cboSourceRobotDev.Items.Add(.GetString("psRD"))
            cboSourceRobotDev.Items.Add(.GetString("psMC"))
            cboSourceRobotDev.Items.Add(.GetString("psUD1"))
            cboSourceRobotDev.Items.Add(.GetString("psUT1"))
            cboSourceRobotDev.Items.Add(.GetString("psFR"))
            cboSourceRobotDev.SelectedIndex = 0

            cboDestRobotDev.SelectedIndex = -1
            cboDestRobotDev.Items.Clear()
            cboDestRobotDev.Items.Add(.GetString("psMD"))
            cboDestRobotDev.Items.Add(.GetString("psRD"))
            cboDestRobotDev.Items.Add(.GetString("psMC"))
            cboDestRobotDev.Items.Add(.GetString("psUD1"))
            cboDestRobotDev.Items.Add(.GetString("psUT1"))
            cboDestRobotDev.Items.Add(.GetString("psFR"))
            cboDestRobotDev.SelectedIndex = 0



            cboSourceType.SelectedIndex = -1
            cboDestType.SelectedIndex = -1
            cboSourceType.Items.Clear()
            cboDestType.Items.Clear()
            cboSourceType.Items.Add(.GetString("psROBOT_CONTROLLERS"))
            cboDestType.Items.Add(.GetString("psROBOT_CONTROLLERS"))
            cboSourceType.Items.Add(.GetString("psTEMP_BACKUPS"))
            cboDestType.Items.Add(.GetString("psTEMP_BACKUPS"))
            cboSourceType.Items.Add(.GetString("psMASTER_BACKUPS"))
            cboDestType.Items.Add(.GetString("psMASTER_BACKUPS"))
            mnSourcePWBackupsIndex = cboSourceType.Items.Count
            cboSourceType.Items.Add(.GetString("psPW_BACKUPS"))
            cboDestType.Items.Add(.GetString("psPW_BACKUPS"))
            mnSourceSelctAFolderCboIndex = cboSourceType.Items.Count
            mnSourceSelctAZipCboIndex = mnSourceSelctAFolderCboIndex + 1
            mnSourceBrowsedFoldersCboIndex = mnSourceSelctAZipCboIndex + 1
            mnDestSelctAFolderCboIndex = cboDestType.Items.Count
            mnDestSelctAZipCboIndex = mnDestSelctAFolderCboIndex + 1
            mnDestBrowsedFoldersCboIndex = mnDestSelctAZipCboIndex + 1
            cboSourceType.Items.Add(.GetString("psSELECT_A_FOLDER"))
            cboDestType.Items.Add(.GetString("psSELECT_A_FOLDER"))
            cboSourceType.Items.Add(.GetString("psSELECT_A_ZIP"))
            cboDestType.Items.Add(.GetString("psSELECT_A_ZIP"))
            mbEventBlocker = False
        End With
        subUpdateCbosByPassword()
        'psMASTER_BACKUPS	Master Backups	
        'psPW_BACKUPS	PAINTworks Auto Backups	
        'psROBOT_CONTROLLERS	Robot Controllers	
        'psSELECT_FILES	Select Files	
        'psTEMP_BACKUPS	Temporary Backups	
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

            'Custom password init
            msActions(eScreenPrivilege.None) = "None"
            msActions(eScreenPrivilege.SaveBackup) = "Save-Backup from robot"
            msActions(eScreenPrivilege.MasterBackups) = "Write master backups"
            msActions(eScreenPrivilege.RestoreTP) = "Write TP files to robot"
            msActions(eScreenPrivilege.RestoreVAR) = "Write variable files to robot"
            msActions(eScreenPrivilege.Delete) = "Delete files"
            msActions(eScreenPrivilege.RobotDevices) = "Advanced robot devices"
            msActions(eScreenPrivilege.BrowseFolder) = "Browse for folders"
            msActions(eScreenPrivilege.SuperUser) = "Super User"

            For nIndex As Integer = (eScreenPrivilege.None + 1) To eScreenPrivilege.SuperUser
                mScreenPrivilege(nIndex) = False
            Next
            mScreenPrivilege(eScreenPrivilege.None) = True

            'init the old password for now
            'moPassword = New PWPassword.PasswordObject
            'mPassword.InitPassword(moPassword, moPrivilege, LoggedOnUser, msSCREEN_NAME)
            'mcolTempFolders = New Collection(Of String)
            'If LoggedOnUser <> String.Empty Then
            '    Privilege = ePrivilege.Edit
            'End If

            Progress = 5

            Me.Text = gpsRM.GetString("psSCREENCAPTION")

            'init new IPC and new Password 'RJO 03/23/12
            oIPC = New Paintworks_IPC.clsInterProcessComm
            moPassword = New clsPWUser
            Progress = 50

            colZones = New clsZones(String.Empty)
            mScreenSetup.InitializeForm(Me)
            mScreenSetup.LoadZoneBox(cboZone, colZones, False)
            subInitFormText()
            Progress = 70
            Me.Show()
            Progress = 98

            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound


            'statusbar text
            Status(True) = gpsRM.GetString("psSELECT_PAGE")

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

            mPrintHtml = New clsPrintHtml(msSCREEN_NAME)
            subClearScreen()

            If True Then 'NRU 161005 Always select a zone.  was cboZone.Items.Count = 1 Then
                'this selects the zone in cases of just one zone. fires event to call subchangezone
                cboZone.Text = cboZone.Items(0).ToString
            Else
                'statusbar text
                Status(True) = gcsRM.GetString("csSELECT_ZONE")
                subEnableControls(True)
            End If

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
    Private Sub subSaveData()
        '********************************************************************************************
        'Description:  Data Save Routine
        '
        'Parameters: rich text box object to save
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************



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
        Return True

    End Function
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
            mPWCommon.subShowChangeLog(colZones.CurrentZone, gsChangeLogArea, msSCREEN_NAME, gcsRM.GetString("csALL"), _
                                           gcsRM.GetString("csALL"), mDeclares.eParamType.None, nIndex, False, False, oIPC)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

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
        ' false means user pressed cancel
        If bAskForSave() = False Then
            cboZone.Text = msOldZone
            Progress = 100
            Exit Sub
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

        Try

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subClearScreen()

            subEnableControls(False)

            colControllers = New clsControllers(colZones, False)
            SetUpStatusStrip(Me, colControllers)
            colArms = LoadArmCollection(colControllers)
            'make sure the robot cartoons get the right color
            colControllers.RefreshConnectionStatus()
            'subListControllers(chkSourceRobots)
            'subListControllers(chkDestRobots)


            ReDim mbRobotNotAvailable(colControllers.Count - 1)
            For Each bTmp As Boolean In mbRobotNotAvailable
                bTmp = False
            Next

            'Select tab 1
            tabMain.SelectedIndex = 0
            ' copy button
            subEnableControls(True)

            'NRU 161005 Clear the previous selection which forces refreshing upon reselection
            Me.cboSourceType.SelectedIndex = -1

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
    Private Sub subListZipSourceFolders(ByRef chkList As CheckedListBox, ByRef sZipFile As String)
        '********************************************************************************************
        'Description:  load folder names in listbox from a zip file that is the copy source
        '
        'Parameters: listbox to fill
        '            zip file
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'It's existence was already checked
        'First build a list of folders in the zip file
        Dim sFolders() As String = Nothing
        Dim bAddRoot As Boolean = False 'See if there's a file with no path before we offer it on the list
        Dim oZip As ZipFile = ZipFile.Read(sZipFile)
        Try
            Using oZip
                'look through the zip file to get a folder listing
                For Each sTmp As String In oZip.EntryFileNames
                    Dim sTmpSplit() As String = Split(sTmp, "/")
                    Dim sPath As String = String.Empty
                    If sTmpSplit.GetUpperBound(0) > 0 Then
                        For nIndex As Integer = 0 To sTmpSplit.GetUpperBound(0) - 1
                            sPath = sPath & sTmpSplit(nIndex) + "/"
                        Next
                        sPath = sPath.Substring(0, sPath.Length - 1)
                        'add path to list
                        If sFolders Is Nothing Then
                            ReDim sFolders(0)
                            sFolders(0) = sPath
                        Else
                            Dim bSkip As Boolean = False
                            For Each sTmpFolder As String In sFolders
                                If sTmpFolder.ToLower = sPath.ToLower Then
                                    bSkip = True
                                    Exit For
                                End If
                            Next
                            If bSkip = False Then
                                Dim nTmp As Integer = sFolders.GetUpperBound(0) + 1
                                ReDim Preserve sFolders(nTmp)
                                sFolders(nTmp) = sPath
                            End If
                        End If
                    Else
                        bAddRoot = True
                    End If
                Next
            End Using
            oZip.Dispose()
            oZip = Nothing
        Catch ex As Exception
        Finally
            If oZip IsNot Nothing Then
                oZip.Dispose()
                oZip = Nothing
            End If
        End Try

        Dim bTag(0) As Boolean
        bTag(0) = bAddRoot
        chkList.Items.Clear()
        chkList.Tag = bTag
        If bAddRoot Then
            'There are files in the root directory
            chkList.Items.Add(sZipFile)
        End If
        If sFolders IsNot Nothing Then
            For Each sTmp As String In sFolders
                chkList.Items.Add(sTmp)
            Next
        End If
        'sDestFolders(nFolder) & "/" 
    End Sub
    Private Sub subListControllers(ByRef chkList As CheckedListBox, Optional ByRef sRootPath As String = "", _
                                    Optional ByVal bAddRoot As Boolean = False, _
                                   Optional ByRef sRequiredFolders() As String = Nothing)
        '********************************************************************************************
        'Description:  load controller names in listbox
        '
        'Parameters: listbox to fill
        '            optional root folder - If there's a folder, only load the subfolders if they exist
        '            optional bAddRoot - set true for destination box, adds the root folder even if it's empty
        '            optional required folders - For dest list, must load subfolders to match source
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            mbEventBlocker = True
            chkList.Items.Clear()
            'Not using the common routine because it wants to format for a short and wide list
            'LoadRobotBoxFromCollection(chkSelectAddresses, colControllers, True)
            Dim sNames() As String
            ReDim sNames(colControllers.Count - 1)
            Dim sD() As String = Nothing
            Dim bAddAll As Boolean = True
            Dim bTag(0) As Boolean
            bTag(0) = False
            If sRootPath <> "" And Not (sRootPath.ToLower.Contains(".zip")) Then
                bAddAll = False
                If Not bAddRoot Then
                    Dim sF() As String = IO.Directory.GetFiles(sRootPath)
                    bAddRoot = (UBound(sF) > 0)
                End If
                If bAddRoot Then
                    'There are files in the root directory
                    Dim sFolder() As String = Split(sRootPath, "\")
                    Dim nIndex As Integer = sFolder.GetUpperBound(0)
                    chkList.Items.Add(sRootPath)
                    bTag(0) = True
                End If
                sD = IO.Directory.GetDirectories(sRootPath)
                If UBound(sD) = -1 Then
                    'no subdir
                End If
            ElseIf sRootPath.ToLower.Contains(".zip") Then
                If bAddRoot Then
                    chkList.Items.Add(sRootPath)
                    bTag(0) = True
                End If
            End If
            chkList.Tag = bTag 'Keep a record of this for later
            For i As Integer = 0 To colControllers.Count - 1
                If bAddAll Then
                    chkList.Items.Add(colControllers.Item(i).Name)
                Else
                    If sD IsNot Nothing Then
                        For Each sTmp As String In sD
                            If sTmp.Substring(sTmp.Length - colControllers.Item(i).Name.Length) = colControllers.Item(i).Name Then
                                chkList.Items.Add(colControllers.Item(i).Name)
                                Exit For
                            End If
                        Next
                    End If
                End If
            Next
            If sRequiredFolders IsNot Nothing AndAlso (sRequiredFolders.GetUpperBound(0) >= 0) Then
                'Required list, add and select them all
                For Each sTmp As String In sRequiredFolders
                    Dim bAdd As Boolean = True
                    For nItem As Integer = 0 To chkList.Items.Count - 1
                        If chkList.Items(nItem).ToString = sTmp Then
                            bAdd = False
                            chkList.SetItemChecked(nItem, True)
                            Exit For
                        End If
                    Next
                    If bAdd Then
                        chkList.Items.Add(sTmp)
                        chkList.SetItemChecked((chkList.Items.Count - 1), True)
                    End If
                Next
            End If
            If chkList.Items.Count = 1 Then
                chkList.SetItemChecked(0, True)
            End If
            mbEventBlocker = False
        Catch ex As Exception
        End Try
    End Sub

#End Region
#Region " Events "

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
            mbCancel = True
            If colArms Is Nothing Then Exit Sub
            If colArms.Count = 0 Then Exit Sub
            For i As Integer = colArms.Count - 1 To 0
                colArms.Remove(colArms(i))
            Next
            Dim oThisProcess As Process = Process.GetCurrentProcess
            oThisProcess.Kill()
        Catch ex As Exception

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
        '    06/07/12   MSW     frmMain_KeyDown-Simplify help links
		'    09/30/13   MSW     Save screenshots as jpegs
        '********************************************************************************************
        Dim sKeyValue As String = String.Empty

        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode
                Case Keys.F1
                    ''Help Screen request
                    'sKeyValue = "btnHelp"
                    'Select Case tabMain.SelectedTab.Name
                    '    Case "tabSource"
                    '        subLaunchHelp(gs_HELP_UTILITIES_FILECOPY_SOURCE)
                    '    Case "tabFiles"
                    '        subLaunchHelp(gs_HELP_UTILITIES_FILECOPY_FILES)
                    '    Case "tabDest"
                    '        subLaunchHelp(gs_HELP_UTILITIES_FILECOPY_DEST)
                    '    Case "tabDoCopy"
                    '        subLaunchHelp(gs_HELP_UTILITIES_FILECOPY_DOCOPY)
                    '    Case Else
                    subLaunchHelp(gs_HELP_UTILITIES_FILECOPY, oIPC)
                    'End Select
                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty

                    Dim sName As String = msSCREEN_DUMP_NAME_PRE & tabMain.SelectedTab.Name & msSCREEN_DUMP_NAME_EXT

                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)

                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath & sName, Imaging.ImageFormat.Bmp)

                Case Keys.Escape
                    If mbCopyActive Then
                        Dim sMsg As String = gpsRM.GetString("psABORT_COPY_MESSAGE")
                        Dim sCaption As String = gpsRM.GetString("psABORT_COPY_CAPTION")
                        Dim oVal As DialogResult = MessageBox.Show(sMsg, sCaption, MessageBoxButtons.OKCancel, _
                                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2, _
                                        MessageBoxOptions.DefaultDesktopOnly)
                        If oVal = Windows.Forms.DialogResult.OK Then
                            Me.Close()
                            Exit Sub
                        End If
                    Else
                        Me.Close()
                        Exit Sub
                    End If
                Case Else

            End Select
        End If

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
        'clean up temp folders
        For Each sTmp As String In mcolTempFolders
            Try
                IO.Directory.Delete(sTmp, True)
            Catch ex As Exception
                'Don't worry about, it'll get cleaned up by PWMaint
            End Try
        Next
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
                If mbCopyActive Then
                    Dim sMsg As String = gpsRM.GetString("psABORT_COPY_MESSAGE")
                    Dim sCaption As String = gpsRM.GetString("psABORT_COPY_CAPTION")
                    Dim oVal As DialogResult = MessageBox.Show(sMsg, sCaption, MessageBoxButtons.OKCancel, _
                                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2, _
                                    MessageBoxOptions.DefaultDesktopOnly)
                    If oVal = Windows.Forms.DialogResult.OK Then
                        Me.Close()
                        Exit Sub
                    End If
                Else
                    Me.Close()
                    Exit Sub
                End If
            Case "btnChangeLog"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                subShowChangeLog(CType(eChangeMnu.Hours24, Integer))
            Case "btnStatus"
                lstStatus.Visible = btnStatus.Checked
            Case "btnUtilities"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then
                    Exit Sub
                Else
                    btnUtilities.ShowDropDown()
                End If
        End Select

        tlsMain.Refresh()

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
    Private Sub rdo_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles rdoSourceAllFiles.CheckedChanged, rdoSourceSelectFiles.CheckedChanged
        '********************************************************************************************
        'Description:  rdo selection change.  see if it changes the enables
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbProcessSourceSelection = True
        If mbEventBlocker = True Then
            Exit Sub
        End If
        If rdoSourceSelectFiles.Checked Then
            Dim nCount As Integer = chkSourceRobots.CheckedItems.Count
            If nCount > 1 Then
                Dim bFoundOne As Boolean = False
                mbEventBlocker = True
                For nIdx As Integer = 0 To nCount - 1
                    If bFoundOne Then
                        chkSourceRobots.SetItemChecked(nIdx, False)
                    Else
                        bFoundOne = chkSourceRobots.GetItemChecked(nIdx)
                    End If
                Next
                mbEventBlocker = False
            End If
        End If
        subEnableControls(True)
    End Sub
    Private Sub cbo_SelectedIndexChanged(ByVal sender As Object, Optional ByVal e As System.EventArgs = Nothing) _
        Handles cboSourceType.SelectedIndexChanged, cboDestType.SelectedIndexChanged, _
        cboSourceRobotDev.SelectedIndexChanged, cboSourceFileType.SelectedIndexChanged, _
        cboDestRobotDev.SelectedIndexChanged
        '********************************************************************************************
        'Description:  cbo selection change.  see if it changes the enables
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEventBlocker = True Then
            Exit Sub
        End If
        Dim oCbo As ComboBox = DirectCast(sender, ComboBox)
        Dim sName As String = oCbo.Name
        Dim bSourceType As Boolean = (sName = "cboSourceType")
        Dim bDestType As Boolean = (sName = "cboDestType")
        Dim bAbort As Boolean = False
        Dim nSelctAFolderCboIndex As Integer = -1
        Dim nSelctAZipCboIndex As Integer = -1
        Dim nPWBackupsIndex As Integer = -1
        Dim nBrowsedFoldersCboIndex As Integer = 999

        Dim bMatchSelection As Boolean = False
        If (bDestType And (mnSourceSubFolderCount > 0)) Then
            If mnSourceSubFolderCount > 1 Then
                bMatchSelection = True
            Else
                '1 selection, make sure it's not a named folder
                If chkSourceRobots.Tag IsNot Nothing Then
                    Dim bTest() As Boolean = DirectCast(chkSourceRobots.Tag, Boolean())
                    If bTest IsNot Nothing Then
                        If bTest(0) Then
                            'First item is the browsed folder
                            bMatchSelection = Not (chkSourceRobots.GetItemChecked(0))
                        Else
                            bMatchSelection = True
                        End If
                    End If
                End If
            End If
        End If
        'source or destination type folder, figure out the path if controllers aren't selected
        If bSourceType Or bDestType Then
            'Clear out the robot checklist box
            If bSourceType Then
                chkSourceRobots.Items.Clear()
                nSelctAFolderCboIndex = mnSourceSelctAFolderCboIndex
                nSelctAZipCboIndex = mnSourceSelctAZipCboIndex
                nPWBackupsIndex = mnSourcePWBackupsIndex
                nBrowsedFoldersCboIndex = mnSourceBrowsedFoldersCboIndex
            Else
                chkDestRobots.Items.Clear()
                nSelctAFolderCboIndex = mnDestSelctAFolderCboIndex
                nSelctAZipCboIndex = mndestSelctAZipCboIndex
                nPWBackupsIndex = mnDestPWBackupsIndex
                nBrowsedFoldersCboIndex = mnDestBrowsedFoldersCboIndex
            End If
            Dim nIndex As Integer = oCbo.SelectedIndex
            Dim sPath As String = String.Empty

            'NRU 161005 Allow resetting the screen when changing zones
            If oCbo.SelectedIndex = -1 Then
                oCbo.Text = ""
                Exit Sub
            End If

            'A browsed folder is selected
            If (nIndex = nSelctAFolderCboIndex) Or (nIndex = nPWBackupsIndex) Or _
                (nIndex = nSelctAZipCboIndex) Or (nIndex >= nBrowsedFoldersCboIndex) Then
                Dim bZip As Boolean = cboDestType.Text.ToLower.Contains(".zip")
                ' Previously added folder
                If (nIndex >= nBrowsedFoldersCboIndex) And (bZip = False) Then
                    sPath = oCbo.SelectedItem.ToString
                    If (IO.Directory.Exists(sPath) = False) Then
                        sPath = String.Empty
                    End If
                End If

                sPath = oCbo.SelectedItem.ToString
                If (IO.Directory.Exists(sPath) = False) Then
                    sPath = String.Empty
                End If

                '"Select a folder" or "PW Backups", browse to it now.
                If (nIndex = nSelctAFolderCboIndex) Or (nIndex = nPWBackupsIndex) Or _
                    (nIndex = nSelctAZipCboIndex) Then
                    bZip = (nIndex = nSelctAZipCboIndex)
                    If bZip Then
                        If bSourceType Then
                            'Open a zip
                            Dim oOFD As New OpenFileDialog
                            With oOFD
                                Dim sPathTmp As String = String.Empty
                                If mPWCommon.GetDefaultFilePath(sPathTmp, eDir.PAINTworks, String.Empty, String.Empty) Then
                                    oOFD.InitialDirectory = sPathTmp
                                End If
                                oOFD.CheckFileExists = True
                                oOFD.DefaultExt = gpsRM.GetString("psZIP_EXT")
                                oOFD.Filter = gpsRM.GetString("psZIPMASK")
                                oOFD.FilterIndex = 1
                                If (oOFD.ShowDialog() = System.Windows.Forms.DialogResult.OK) AndAlso ZipFile.IsZipFile(oOFD.FileName) Then
                                    sPath = oOFD.FileName
                                Else
                                    sPath = String.Empty
                                End If
                            End With
                        Else
                            'Save a zip
                            Dim oSFD As New SaveFileDialog
                            With oSFD
                                Dim sPathTmp As String = String.Empty
                                If mPWCommon.GetDefaultFilePath(sPathTmp, eDir.PAINTworks, String.Empty, String.Empty) Then
                                    oSFD.InitialDirectory = sPathTmp
                                End If
                                oSFD.CheckPathExists = True
                                oSFD.AddExtension = True
                                oSFD.DefaultExt = gpsRM.GetString("psZIP_EXT")
                                oSFD.Filter = gpsRM.GetString("psZIPMASK")
                                oSFD.FilterIndex = 1
                                If (oSFD.ShowDialog() = System.Windows.Forms.DialogResult.OK) Then
                                    sPath = oSFD.FileName
                                Else
                                    sPath = String.Empty
                                End If
                            End With
                        End If
                    Else
                        'Find a folder
                        Dim oFB As New FolderBrowserDialog
                        With oFB
                            'Offer to make a new folder, but only for a destination
                            .ShowNewFolderButton = bDestType
                            Dim sPathTmp As String = String.Empty
                            If (nIndex = nPWBackupsIndex) Then
                                If mPWCommon.GetDefaultFilePath(sPathTmp, eDir.PWRobotBackups, String.Empty, String.Empty) Then
                                    .SelectedPath = sPathTmp
                                End If
                                .Description = gpsRM.GetString("psSELECT_PWBACKUP")
                            Else
                                If mPWCommon.GetDefaultFilePath(sPathTmp, eDir.PAINTworks, String.Empty, String.Empty) Then
                                    .SelectedPath = sPathTmp
                                End If
                                .Description = gpsRM.GetString("psSELECT_FOLDER")
                            End If
                            .ShowDialog()
                            sPath = .SelectedPath
                        End With
                    End If
                    If sPath = String.Empty Then
                        'bad selection, clear out the lists and unselect it.
                        mbEventBlocker = True
                        oCbo.SelectedIndex = -1
                        oCbo.SelectedText = String.Empty
                        bAbort = True
                        mbEventBlocker = False
                    Else
                        'select the folder if it's already in the list
                        mbEventBlocker = True
                        Dim bAdd As Boolean = True
                        For nIndex = 0 To oCbo.Items.Count - 1
                            If oCbo.Items(nIndex).ToString = sPath Then
                                oCbo.SelectedIndex = nIndex
                                bAdd = False
                            End If
                        Next
                        If bAdd Then
                            oCbo.Items.Add(sPath)
                            oCbo.SelectedIndex = oCbo.Items.Count - 1
                        End If
                        mbEventBlocker = False
                    End If
                End If
                'Back to all browsed folder selections
                If sPath <> String.Empty Then
                    Dim sPathTmp As String = String.Empty
                    Dim bAddRoot As Boolean = bZip Or (nIndex = nSelctAFolderCboIndex) Or (nIndex = nPWBackupsIndex) Or (nIndex >= nBrowsedFoldersCboIndex)
                    If bAddRoot AndAlso (bZip = False) AndAlso _
                        mPWCommon.GetDefaultFilePath(sPathTmp, eDir.PWRobotBackups, String.Empty, String.Empty) AndAlso _
                        (InStr(sPath.ToLower, sPathTmp.ToLower) > 0) Then
                        bAddRoot = False
                    End If
                    If bSourceType Then
                        'Source, get whatever subfolders are available
                        If bZip Then
                            subListZipSourceFolders(chkSourceRobots, sPath)
                            If oCbo.Items.Count = 0 Then
                                'empty zip
                                mbEventBlocker = True
                                oCbo.SelectedIndex = -1
                                oCbo.SelectedText = String.Empty
                                bAbort = True
                                mbEventBlocker = False
                            End If
                        Else
                            subListControllers(chkSourceRobots, sPath, bAddRoot)
                        End If
                        Status = gpsRM.GetString("psSOURCE_SELECTED") & sPath
                    Else
                        'Destination, match selection to source if appropriate
                        If bMatchSelection Then
                            'For a browsed destination excetp autobackups, offer the chance to select the root, but 
                            'preselect the single robot folder
                            subListControllers(chkDestRobots, sPath, bAddRoot, msSourceSubFolders)
                        Else
                            subListControllers(chkDestRobots, sPath, bAddRoot)
                        End If
                        Status = gpsRM.GetString("psDEST_SELECTED") & sPath
                    End If
                End If
            Else 'If (nIndex >= mnSelctAFolderCboIndex) Or (nIndex = mnPWBackupsIndex) Then
                'Standard folders, just list the robot names
                If bSourceType Then
                    subListControllers(chkSourceRobots)
                    Status = gpsRM.GetString("psSOURCE_SELECTED") & oCbo.Text
                Else
                    'Destination, match selection to source if appropriate
                    If bMatchSelection Then
                        subListControllers(chkDestRobots, , False, msSourceSubFolders)
                    Else
                        subListControllers(chkDestRobots, , True)
                    End If
                    Status = gpsRM.GetString("psDEST_SELECTED") & oCbo.Text
                End If
            End If
        End If
        If InStr(sName, "Source") > 0 Then
            mbProcessSourceSelection = True
        End If
        subEnableControls(True)
    End Sub

    Private Sub chkFiles_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkFiles.DoubleClick
        '********************************************************************************************
        'Description:  Double-click in view list, open view window
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mnuViewComparePrint_Click(mnuViewCompare)
    End Sub
    Private Sub chkFiles_ItemCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) _
        Handles chkFiles.ItemCheck
        '********************************************************************************************
        'Description:  Checklist selection change.  see if it changes the enables
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEventBlocker = True Then
            Exit Sub
        End If
        Dim nCount As Integer = chkFiles.CheckedItems.Count

        'I can't find an event for after it handles the checks, so I'll force an updated item count
        If (e.NewValue = CheckState.Checked) Then
            nCount += 1
        Else
            nCount += -1
        End If
        subEnableControls(True, , , nCount)
    End Sub
    Private Sub chklst_ItemCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) _
        Handles chkSourceRobots.ItemCheck, chkDestRobots.ItemCheck
        '********************************************************************************************
        'Description:  Checklist selection change.  see if it changes the enables
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEventBlocker = True Then
            Exit Sub
        End If

        Dim bDeleteFiles As Boolean = btnDeleteFiles.Checked
        Dim oChklst As CheckedListBox = DirectCast(sender, CheckedListBox)
        Dim sName As String = oChklst.Name
        Dim nCount As Integer = oChklst.CheckedItems.Count

        'I can't find an event for after it handles the checks, so I'll force an updated item count
        If (e.NewValue = CheckState.Checked) Then
            nCount += 1
        Else
            nCount += -1
        End If
        'If a root folder is added, it's not allowed in a multiple selection
        If nCount > 1 And (e.NewValue = CheckState.Checked) Then
            If oChklst.Tag IsNot Nothing Then
                Dim bTest() As Boolean = DirectCast(oChklst.Tag, Boolean())
                If bTest IsNot Nothing Then
                    If bTest(0) Then
                        'Make sure the root directory isn't selected
                        mbEventBlocker = True
                        If e.Index = 0 Then
                            'Selecting the root directory, unselect the rest
                            For nIdx As Integer = 1 To oChklst.Items.Count - 1
                                oChklst.SetItemChecked(nIdx, False)
                                nCount -= 1
                            Next
                        Else
                            'Selecting another, deselect the root.
                            oChklst.SetItemChecked(0, False)
                        End If
                        Application.DoEvents()
                        mbEventBlocker = False
                    End If
                End If
            End If
        End If

        Select Case oChklst.Name
            Case "chkSourceRobots"
                If nCount > 1 Then
                    If btnDeleteFiles.Checked Then
                        'If delete button is pressed, limit to 1 checkbox
                        'Delete all, since it's added after this event.
                        For nIdx As Integer = 0 To chkSourceRobots.Items.Count - 1
                            chkSourceRobots.SetItemChecked(nIdx, False)
                        Next
                    Else
                        'No delete button, multiple robots, force "Select All"
                        mbEventBlocker = True
                        rdoSourceAllFiles.Checked = True
                        mbEventBlocker = False
                    End If
                End If
                'Watch for dest = source
                If (cboDestType.Text = cboSourceType.Text) And (e.NewValue = CheckState.Checked) Then
                    'Do something about this
                    mbEventBlocker = True
                    For nIndex As Integer = 0 To chkDestRobots.Items.Count - 1
                        If chkDestRobots.Items(nIndex).ToString = oChklst.Items(e.Index).ToString Then
                            'This one is in the destination, disable it
                            chkDestRobots.SetItemChecked(nIndex, False)
                            nCount -= 1
                        End If
                    Next
                    mbEventBlocker = False
                End If
                subEnableControls(True, nCount)
                mbProcessSourceSelection = True
            Case "chkDestRobots"
                'Watch for dest = source
                If (cboDestType.Text = cboSourceType.Text) And (e.NewValue = CheckState.Checked) Then
                    'Do something about this
                    mbEventBlocker = True
                    For Each oItem As Object In chkSourceRobots.CheckedItems
                        If oItem.ToString = oChklst.Items(e.Index).ToString Then
                            'This one is in the source, disable it
                            oChklst.SetItemChecked(e.Index, False)
                            nCount -= 1
                        End If
                    Next
                    mbEventBlocker = False
                End If
                subEnableControls(True, , nCount)
        End Select
    End Sub


    Private Sub chklst_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
        Handles chkSourceRobots.MouseDown, chkFiles.MouseDown, chkDestRobots.MouseDown
        '********************************************************************************************
        'Description:  Checklist mouse-down - catch which check list box is opening a pop-up menu
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        moMouseDownObject = sender
        'Right-click on a file
        Dim oChkLst As CheckedListBox = DirectCast(sender, CheckedListBox)
        If (oChkLst IsNot Nothing) AndAlso (oChkLst.Name = "chkFiles") Then
            mnuViewCompare.Visible = True
            mnuPrint.Visible = True
            'The bastard won't just give me the index for where the mouse is down at, have to calculate, and account for the scrollbar
            If oChkLst.MultiColumn Then
                mnPopupMenuItem = -1
                Dim nGreaterThan As Integer = -1
                Dim nLessThan As Integer = oChkLst.Items.Count
                Dim nItem As Integer = (nGreaterThan + nLessThan) \ 2
                Do Until mnPopupMenuItem >= 0 Or (nLessThan - nGreaterThan <= 1)
                    Dim oRect As Rectangle = oChkLst.GetItemRectangle(nItem)
                    If ((oRect.X < e.X) And ((oRect.X + oRect.Width) >= e.X)) Then
                        'correct column, check Y
                        If ((oRect.Y < e.Y) And ((oRect.Y + oRect.Height) >= e.Y)) Then
                            'correct item
                            mnPopupMenuItem = nItem
                        Else
                            'Wrong item, move up or down one
                            If (oRect.Y >= e.Y) Then
                                'go up
                                nLessThan = nItem
                                nItem -= 1
                            Else
                                'go down
                                nGreaterThan = nItem
                                nItem += 1
                            End If
                        End If
                    Else
                        'Wrong column, make a big move
                        If (oRect.X >= e.X) Then
                            'need to shift left a column
                            nLessThan = nItem
                            nItem = (nGreaterThan + nLessThan) \ 2
                        Else
                            'need to shift right by a column
                            nGreaterThan = nItem
                            nItem = (nGreaterThan + nLessThan) \ 2
                        End If
                    End If
                    'click in white space:
                    If nItem >= oChkLst.Items.Count Then
                        mnPopupMenuItem = oChkLst.Items.Count - 1
                    End If
                Loop
            Else
                Dim nItem As Integer = -1
                nItem = (e.Location.Y \ oChkLst.ItemHeight) + 1
                If nItem >= 1 And nItem <= oChkLst.Items.Count Then
                    'Account for the scrollbar
                    Dim Rect As Rectangle = oChkLst.GetItemRectangle(nItem - 1)
                    If e.Location.Y > Rect.Bottom Then
                        nItem = nItem + ((e.Location.Y - Rect.Bottom) \ oChkLst.ItemHeight) + 1
                    End If
                Else
                    'In whitespace underneath? take the last step
                    If nItem > oChkLst.Items.Count - 1 Then
                        nItem = oChkLst.Items.Count
                    End If
                    If nItem < 0 Then
                        nItem = 1
                    End If
                End If
                mnPopupMenuItem = nItem
            End If
            If mnPopupMenuItem < 0 Then
                mnPopupMenuItem = 1
            End If
            'Debug.Print(oChkLst.Items(mnPopupMenuItem).ToString)
        Else
            mnuViewCompare.Visible = False
            mnuPrint.Visible = False
        End If
    End Sub
    Private Sub mnuCompare_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuCompare.Click
        '********************************************************************************************
        'Description:  utility menu compare files/folders
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        frmFileViewCompare.Show()
    End Sub

    Private Sub mnuViewComparePrint_Click(ByVal sender As System.Object, Optional ByVal e As System.EventArgs = Nothing) Handles mnuViewCompare.Click, mnuPrint.Click
        '********************************************************************************************
        'Description:  pop-up menu view file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim oMnu As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
            If oMnu IsNot Nothing Then
                    'Pop-up on the file listbox
                Select Case tabMain.SelectedTab.Name
                    Case "tabFiles"
                        'File name
                        If mnPopupMenuItem >= 0 And mnPopupMenuItem < chkFiles.Items.Count Then

                            Dim sPath(0) As String
                            sPath(0) = String.Empty
                            Dim sFile(0) As String
                            sFile(0) = chkFiles.Items(mnPopupMenuItem).ToString
                            Dim sLabel As String = String.Empty
                            If msSourceFolder = msROBOTS_TAG Then
                                'Load from the robot into a temp folder
                                Dim sSourceFolders(0) As String
                                Dim sSourceDevice(0) As String 'Path for temp folder
                                SubCopyFromControllers(sFile, 0, sSourceFolders, sSourceDevice)
                                If Strings.Right(sSourceFolders(0), 1) = "\" Then
                                    sPath(0) = sSourceFolders(0) & sFile(0)
                                Else
                                    sPath(0) = sSourceFolders(0) & "\" & sFile(0)
                                End If
                                sLabel = sSourceDevice(0) & "\" & "MD" & ":" & sFile(0)
                            Else
                                If msSourceFolder.ToLower.Contains(".zip") Then
                                    'Zip file
                                    Dim sSourceFolders(0) As String 'Path for temp folder
                                    SubExtractFromZip(msSourceFolder, msSourceSubFolders(0), sFile, sSourceFolders(0))
                                    If Strings.Right(sSourceFolders(0), 1) = "\" Then
                                        sPath(0) = sSourceFolders(0) & sFile(0)
                                    Else
                                        sPath(0) = sSourceFolders(0) & "\" & sFile(0)
                                    End If
                                    sLabel = msSourceFolder & "\" & " - " & msSourceSubFolders(0) & "/" & sFile(0)
                                Else
                                    'Put the path name together to read the file directly
                                    If Strings.Right(msSourceSubFolders(0), 1) = "\" Then
                                        sPath(0) = msSourceFolder & msSourceSubFolders(0) & chkFiles.Items(mnPopupMenuItem).ToString
                                    Else
                                        sPath(0) = msSourceFolder & msSourceSubFolders(0) & "\" & chkFiles.Items(mnPopupMenuItem).ToString
                                    End If
                                    sLabel = sPath(0)
                                End If
                            End If
                            Select Case oMnu.Name
                                Case mnuPrint.Name
                                    'Make a RichTextBox and have it load the file, it's easier
                                    If mRtbPrint Is Nothing Then
                                        mRtbPrint = New RichTextBox
                                        Me.Controls.Add(mRtbPrint)
                                    End If
                                    mRtbPrint.LoadFile(sPath(0), RichTextBoxStreamType.PlainText)
                                    mPrintHtml.subSetPageFormat()
                                    mPrintHtml.subClearTableFormat()
                                    Dim sTitle As String = String.Empty
                                    Dim sSubTitle(0) As String
                                    sSubTitle(0) = String.Empty
                                    If msSourceFolder = msROBOTS_TAG Then
                                        sSubTitle(0) = gcsRM.GetString("csROBOT_CAP") & ":  " & msSourceSubFolders(0)
                                        mPrintHtml.subCreateSimpleDoc(mRtbPrint.Text, Status, colZones.SiteName & "  " & colZones.CurrentZone, sSubTitle, sFile)
                                    Else
                                        mPrintHtml.subCreateSimpleDoc(mRtbPrint.Text, Status, colZones.SiteName & "  " & colZones.CurrentZone, sSubTitle, sPath)
                                    End If
                                    mPrintHtml.subShowPrintPreview()
                                Case mnuViewCompare.Name
                                    frmFileViewCompare.Show()
                                    Dim oForm As New frmFileViewCompareChild
                                    oForm.FolderView = False
                                    oForm.subLoadFile(sPath(0))
                                    oForm.FileName = sFile(0)
                                    oForm.Title = sLabel
                                    oForm.MdiParent = frmFileViewCompare
                                    oForm.Show()
                                Case Else
                                    'oops
                                    Debug.Assert(False)
                            End Select
                            'Open the window
                            If msSourceFolder = msROBOTS_TAG Then
                                IO.File.Delete(sPath(0))
                            End If
                        End If
                    Case Else
                        'Not used for now
                End Select

            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        End Try


    End Sub

    Private Sub mnuSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSelectAll.Click
        '********************************************************************************************
        'Description:  pop-up menu select all
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************


        Dim oChkLst As CheckedListBox = DirectCast(moMouseDownObject, CheckedListBox)
        If oChkLst IsNot Nothing Then
            Dim nStart As Integer = 0
            'Check the tag that means the root directory was added to the list box
            If oChkLst.Tag IsNot Nothing And (oChkLst.Items.Count > 1) Then
                Dim bTest() As Boolean = DirectCast(oChkLst.Tag, Boolean())
                If bTest IsNot Nothing Then
                    If bTest(0) Then
                        'Root directory was added.  Skip it and select all robots
                        nStart = 1
                    End If
                End If
            End If
            For nItem As Integer = nStart To oChkLst.Items.Count - 1
                oChkLst.SetItemCheckState(nItem, CheckState.Checked)
            Next
            If InStr(oChkLst.Name, "Source") > 0 Then
                mbProcessSourceSelection = True
            End If
        End If
    End Sub
    Private Sub mnuUnselectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuUnselectAll.Click
        '********************************************************************************************
        'Description:  pop-up menu unselect all
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oChkLst As CheckedListBox = DirectCast(moMouseDownObject, CheckedListBox)
        If oChkLst IsNot Nothing Then
            For nItem As Integer = 0 To oChkLst.Items.Count - 1
                oChkLst.SetItemCheckState(nItem, CheckState.Unchecked)
            Next
            If InStr(oChkLst.Name, "Source") > 0 Then
                mbProcessSourceSelection = True
            End If
        End If
    End Sub
    Private Sub mnuBackupAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
            mnuBackupAllTemp.Click, mnuBackupAllMaster.Click, mnuBackupTextFiles.Click, _
            mnuGetDiag.Click, mnuBackupAllMasterWithTextFiles.Click, mnuBackupAllTempWithTextFiles.Click
        '********************************************************************************************
        'Description:  Backup all function selected from toolbar
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oMnu As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        tabMain.SelectTab(tabSource)

        'Set Source type to robot controllers
        For nIndex As Integer = 0 To cboSourceType.Items.Count - 1
            If cboSourceType.Items(nIndex).ToString = gpsRM.GetString("psROBOT_CONTROLLERS") Then
                cboSourceType.SelectedIndex = nIndex
                cboSourceType.Text = gpsRM.GetString("psROBOT_CONTROLLERS")
                Exit For
            End If
        Next
        'Select all robots
        Dim nStart As Integer = 0
        If chkSourceRobots.Tag IsNot Nothing Then
            Dim bTest() As Boolean = DirectCast(chkSourceRobots.Tag, Boolean())
            If bTest IsNot Nothing Then
                If bTest(0) Then
                    nStart = 1
                End If
            End If
        End If
        For nItem As Integer = nStart To chkSourceRobots.Items.Count - 1
            chkSourceRobots.SetItemCheckState(nItem, CheckState.Checked)
        Next
        If oMnu.Name = mnuBackupTextFiles.Name Then
            'Device = md:
            For nIndex As Integer = 0 To cboSourceRobotDev.Items.Count - 1
                If cboSourceRobotDev.Items(nIndex).ToString = gpsRM.GetString("psMD") Then
                    cboSourceRobotDev.SelectedIndex = nIndex
                    cboSourceRobotDev.Text = gpsRM.GetString("psMD")
                    Exit For
                End If
            Next
            'Mask = *.*
            For nIndex As Integer = 0 To cboSourceFileType.Items.Count - 1
                If cboSourceFileType.Items(nIndex).ToString = gpsRM.GetString("psTEXT_FILE_MASK") Then
                    cboSourceFileType.SelectedIndex = nIndex
                    Exit For
                End If
            Next
            cboSourceFileType.Text = gpsRM.GetString("psTEXT_FILE_MASK")
            tabMain.SelectTab(tabSource)
        ElseIf oMnu.Name = mnuGetDiag.Name Then
            'Device = md:
            For nIndex As Integer = 0 To cboSourceRobotDev.Items.Count - 1
                If cboSourceRobotDev.Items(nIndex).ToString = gpsRM.GetString("psMD") Then
                    cboSourceRobotDev.SelectedIndex = nIndex
                    cboSourceRobotDev.Text = gpsRM.GetString("psMD")
                    Exit For
                End If
            Next
            'Mask = *.*
            For nIndex As Integer = 0 To cboSourceFileType.Items.Count - 1
                If cboSourceFileType.Items(nIndex).ToString = gpsRM.GetString("psZIPMASK") Then
                    cboSourceFileType.SelectedIndex = nIndex
                    Exit For
                End If
            Next
            cboSourceFileType.Text = gpsRM.GetString("psZIPMASK")
            tabMain.SelectTab(tabSource)
        Else
            If (oMnu.Name = mnuBackupAllMasterWithTextFiles.Name) Or _
               (oMnu.Name = mnuBackupAllTempWithTextFiles.Name) Then
                'Device = md:
                For nIndex As Integer = 0 To cboSourceRobotDev.Items.Count - 1
                    If cboSourceRobotDev.Items(nIndex).ToString = gpsRM.GetString("psMD") Then
                        cboSourceRobotDev.SelectedIndex = nIndex
                        cboSourceRobotDev.Text = gpsRM.GetString("psMD")
                        Exit For
                    End If
                Next
            Else
                'Device = mdb:
                For nIndex As Integer = 0 To cboSourceRobotDev.Items.Count - 1
                    If cboSourceRobotDev.Items(nIndex).ToString = gpsRM.GetString("psMDB") Then
                        cboSourceRobotDev.SelectedIndex = nIndex
                        cboSourceRobotDev.Text = gpsRM.GetString("psMDB")
                        Exit For
                    End If
                Next
            End If
            'Mask = *.*
            For nIndex As Integer = 0 To cboSourceFileType.Items.Count - 1
                If cboSourceFileType.Items(nIndex).ToString = gpsRM.GetString("psALL_FILE_MASK") Then
                    cboSourceFileType.SelectedIndex = nIndex
                    Exit For
                End If
            Next
            cboSourceFileType.Text = gpsRM.GetString("psALL_FILE_MASK")
        End If
        tabMain.SelectTab(tabDest)
        'Set Dest type
        Dim sTmp As String = String.Empty
        If (oMnu.Name = mnuBackupAllMasterWithTextFiles.Name) Or _
           (oMnu.Name = mnuBackupAllMaster.Name) Then
            sTmp = gpsRM.GetString("psMASTER_BACKUPS")
        Else
            sTmp = gpsRM.GetString("psTEMP_BACKUPS")
        End If
        For nIndex As Integer = 0 To cboDestType.Items.Count - 1
            If cboDestType.Items(nIndex).ToString = sTmp Then
                cboDestType.SelectedIndex = nIndex
                cboDestType.Text = sTmp
                Exit For
            End If
        Next
        'Select All robot folders
        For nItem As Integer = 0 To chkDestRobots.Items.Count - 1
            chkDestRobots.SetItemCheckState(nItem, CheckState.Checked)
        Next
        tabMain.SelectTab(tabDoCopy)
    End Sub
    Private Sub btnDeleteFiles_Click(Optional ByVal sender As System.Object = Nothing, Optional ByVal e As System.EventArgs = Nothing) Handles btnDeleteFiles.Click
        '********************************************************************************************
        'Description:  Delete files mode selected from toolbar
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Debug.Print(btnDeleteFiles.Checked.ToString)
        mbEventBlocker = True
        If btnDeleteFiles.Checked Then
            rdoSourceSelectFiles.Checked = True
            rdoSourceSelectFiles.Enabled = False
            rdoSourceAllFiles.Enabled = False
            Dim nCount As Integer = chkSourceRobots.Items.Count
            If nCount > 1 Then
                Dim bFoundOne As Boolean = False
                For nIdx As Integer = 0 To nCount - 1
                    If bFoundOne Then
                        chkSourceRobots.SetItemChecked(nIdx, False)
                    Else
                        bFoundOne = chkSourceRobots.GetItemChecked(nIdx)
                    End If
                Next
            End If
            If chkFiles.Items.Count > 0 Then
                chkFiles.Items.Clear()
            End If
            btnDoCopy.Text = gpsRM.GetString("psDO_DELETE")
            tabDoCopy.Text = gpsRM.GetString("psDO_DELETE")
            Select Case tabMain.SelectedTab.Name
                Case "tabSource"
                    'If it's OK for the files list tab, make sure it reloads the file list when it gets there
                    mbProcessSourceSelection = btnSourceNext.Enabled
                    Status(False) = gpsRM.GetString("psSELECT_SOURCE")
                Case "tabFiles"
                    'Reprpocess the source selection
                    subProcessSourceSelection()
                Case "tabDest", "tabDoCopy"
                    mbProcessSourceSelection = True
                    tabMain.SelectTab(tabFiles)
            End Select
        Else
            rdoSourceSelectFiles.Enabled = True
            rdoSourceAllFiles.Enabled = True
            If chkFiles.Items.Count > 0 Then
                chkFiles.Items.Clear()
            End If
            btnDoCopy.Text = gpsRM.GetString("psDO_COPY")
            tabDoCopy.Text = gpsRM.GetString("psDO_COPY")
            Select Case tabMain.SelectedTab.Name
                Case "tabSource"
                    'If it's OK for the files list tab, make sure it reloads the file list when it gets there
                    mbProcessSourceSelection = btnSourceNext.Enabled
                    Status(False) = gpsRM.GetString("psSELECT_SOURCE")
                Case "tabFiles"
                    'Reprpocess the source selection
                    subProcessSourceSelection()
                Case "tabDest", "tabDoCopy"
                    mbProcessSourceSelection = True
                    tabMain.SelectTab(tabSource)
            End Select
        End If
        subEnableControls(True) 'See what's enabled after the changes
        mbEventBlocker = False
    End Sub
    Private Sub subDeleteFromController(ByRef sFiles() As String, ByRef bError As Boolean)
        '********************************************************************************************
        'Description:  Delete files from a robot controller
        '
        'Parameters: sFiles - list of files from chkFiles, or empty for *.* transfers
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 07/11/13  MSW     Track errors for improved status display
        '********************************************************************************************
        Dim sChangeLogText As String = String.Empty
        Dim sChangeLogDevice As String = String.Empty
        'Delete from a robot
        'Need the FanucName for FTP, we're working with {nothing}Name in the list
        For Each oController As clsController In colControllers
            If oController.Name = msSourceSubFolders(0) Then
                sChangeLogDevice = oController.FanucName
            End If
        Next

        Dim oFTP As clsFSFtp = Nothing
        Try
            oFTP = New clsFSFtp(sChangeLogDevice)
            With oFTP
                If .Connected Then
                    'set the working dir on the host robot controller
                    Dim sDevName() As String = Split(cboSourceRobotDev.Text, " ")
                    .WorkingDir = sDevName(0)
                    For Each sFile As String In sFiles
                        Application.DoEvents()
                        If mbCancel Then
                            Exit For
                        End If
                        ' "Copying " & sFile & " from " & sSourceFolders(nFolder) & "\ to " & sDestFolders(nFolder)
                        Status = gpsRM.GetString("psDELETED") & sFile & gpsRM.GetString("psFROM") & _
                            gpsRM.GetString("psCONTROLLER") & sChangeLogDevice
                        Try
                            If .Delete(sFile) Then
                                'ChangeLog
                                sChangeLogText = gpsRM.GetString("psDELETED_FILE") & sFile & gpsRM.GetString("psFROM") & _
                                    gpsRM.GetString("psCONTROLLER") & sChangeLogDevice
                                mPWCommon.AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                                                          colZones.CurrentZoneNumber, _
                                                          sChangeLogDevice, String.Empty, _
                                                          sChangeLogText, colZones.CurrentZone)
                            Else
                                sChangeLogText = gpsRM.GetString("psFAILED_TO_DELETE") & sFile & gpsRM.GetString("psFROM") & _
                                    gpsRM.GetString("psCONTROLLER") & sChangeLogDevice
                                bError = True
                            End If
                            '.Close()
                        Catch ex As Exception
                            sChangeLogText = gpsRM.GetString("psFAILED_TO_DELETE") & sFile & gpsRM.GetString("psFROM") & _
                                    gpsRM.GetString("psCONTROLLER") & sChangeLogDevice
                            bError = True
                        Finally
                            Status = sChangeLogText
                            lstSummary.Items.Add(sChangeLogText)
                        End Try
                    Next
                End If
                .Close()
            End With

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            bError = True
        Finally
            If oFTP IsNot Nothing Then
                oFTP.Close()
            End If
        End Try

    End Sub
    Private Sub subDeleteFromFolder(ByRef sFiles() As String, ByRef bError As Boolean)
        '********************************************************************************************
        'Description:  Delete files from a folder
        'v
        'Parameters: sFiles - list of files from chkFiles, or empty for *.* transfers
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/11/10  MSW     folder name confusion during file delete
        ' 07/11/13  MSW     Track errors for improved status display
        '********************************************************************************************
        Dim sChangeLogText As String = String.Empty
        Dim sChangeLogDevice As String = String.Empty
        'Delete from a file folder
        Dim sFolder As String = msSourceSubFolders(0)

        If InStr(sFolder, msSourceFolder) > 0 Then
        Else
            If Strings.Right(msSourceFolder, 1) = "\" Then
                sFolder = msSourceFolder & sFolder
            Else
                sFolder = msSourceFolder & "\" & sFolder
            End If
        End If
        If Strings.Right(sFolder, 1) <> "\" Then
            sFolder = sFolder & "\"
        End If
        For Each sFile As String In sFiles
            Application.DoEvents()
            If mbCancel Then
                Exit For
            End If
            ' "Copying " & sFile & " from " & sSourceFolders(nFolder) & "\ to " & sDestFolders(nFolder)
            Status = gpsRM.GetString("psDELETING") & sFile & gpsRM.GetString("psFROM") & sFolder
            Try
                File.Delete(sFolder & sFile)
                sChangeLogText = gpsRM.GetString("psDELETED_FILE") & sFolder & sFile

                'ChangeLog
                mPWCommon.AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                                          colZones.CurrentZoneNumber, _
                                          sChangeLogDevice, String.Empty, _
                                          sChangeLogText, colZones.CurrentZone)
            Catch ex As Exception
                sChangeLogText = gpsRM.GetString("psFAILED_TO_DELETE") & sFile & gpsRM.GetString("psFROM") & sFolder
                bError = True
            Finally
                Status = sChangeLogText
                lstSummary.Items.Add(sChangeLogText)
            End Try
        Next

    End Sub
    Private Sub SubExtractFromZip(ByRef sZipFile As String, ByRef sSubFolder As String, _
                                        ByVal sFiles() As String, _
                                        ByRef sTempFolder As String, _
                                        Optional ByRef bError As Boolean = False)
        '********************************************************************************************
        'Description:  extract files from zip archives
        '
        'Parameters: sFiles - list of files from chkFiles, or empty for *.* transfers
        '            nMaxFolderIdx - max folder index (folder count-1)
        '            sTempFolder - temp folder to extract to
        '            sSubFolder - sub folder in zip file
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            'Make a temp folder
            sTempFolder = sGetTmpFileName("TempFolderForZip", String.Empty)
            If sSubFolder <> String.Empty Then
                IO.Directory.CreateDirectory(sTempFolder)
                mcolTempFolders.Add(sTempFolder)
                sTempFolder = sTempFolder & "\" & sSubFolder
            End If
            IO.Directory.CreateDirectory(sTempFolder)
            mcolTempFolders.Add(sTempFolder)
            Dim oZip As ZipFile = Nothing

            Try
                oZip = ZipFile.Read(sZipFile)
                Using oZip
                    oZip.FlattenFoldersOnExtract = True 'We're managing our own directory structure

                    'Different methods depending on a file list or mask
                    If sFiles Is Nothing Then
                        'No list, build from mask
                        ReDim sFiles(0)
                        Dim sMask As String = cboSourceFileType.Text.ToLower
                        sMask = Replace(sMask, ",", " ")
                        sMask = Replace(sMask, ";", " ")
                        Dim sMaskList() As String = Split(sMask, " ")
                        'Go through all the available files
                        For Each oZipEntry As ZipEntry In oZip
                            Dim sFile As String = oZipEntry.FileName
                            If sSubFolder = String.Empty Then
                                'Root folder only
                                If (InStr(sFile, "/") = 0) Then
                                    'Check the mask
                                    For Each sTmp As String In sMaskList
                                        If (InStr(sTmp, "*") > 0) Or (InStr(sTmp, ".") > 0) Then
                                            If sFile Like sTmp Then
                                                sFiles(sFiles.GetUpperBound(0)) = sFile
                                                ReDim Preserve sFiles(sFiles.GetUpperBound(0) + 1)
                                                oZipEntry.Extract(sTempFolder, ExtractExistingFileAction.OverwriteSilently)
                                            End If
                                        End If
                                    Next
                                End If
                            Else
                                'See if the path is right
                                If sSubFolder.Length < sFile.Length AndAlso _
                                    sFile.Substring(0, sSubFolder.Length) = sSubFolder Then
                                    'It's under the correct path, make sure it's not farther down another subdirectory
                                    Dim sFileTmp As String = sFile.Substring(sSubFolder.Length + 1)
                                    If (InStr(sFileTmp, "/") = 0) Then
                                        'Check the mask
                                        For Each sTmp As String In sMaskList
                                            If (InStr(sTmp, "*") > 0) Or (InStr(sTmp, ".") > 0) Then
                                                If sFileTmp Like sTmp Then
                                                    sFiles(sFiles.GetUpperBound(0)) = sFile
                                                    ReDim Preserve sFiles(sFiles.GetUpperBound(0) + 1)
                                                    oZipEntry.Extract(sTempFolder, ExtractExistingFileAction.OverwriteSilently)
                                                End If
                                            End If
                                        Next
                                    End If
                                End If
                            End If
                        Next
                        ReDim Preserve sFiles(sFiles.GetUpperBound(0) - 1)
                    Else
                        For Each oZipEntry As ZipEntry In oZip
                            For Each sTmpFile As String In sFiles
                                sTmpFile = sSubFolder & "/" & sTmpFile
                                If oZipEntry.FileName.ToLower = sTmpFile.ToLower Then
                                    oZipEntry.Extract(sTempFolder, ExtractExistingFileAction.OverwriteSilently)
                                End If
                            Next
                        Next
                        'We have a list, merge with the path
                        For Each sTmp As String In sFiles
                            sTmp = sSubFolder & "/" & sTmp
                        Next
                    End If
                    'Got the directory list. get the files

                End Using
            Catch ex As Exception
                bError = True
            Finally
                If oZip IsNot Nothing Then
                    oZip.Dispose()
                    oZip = Nothing
                End If
            End Try
        Catch ex As Exception
            bError = True
        End Try
    End Sub
    Private Sub SubCopyFromControllers(ByRef sFiles() As String, _
                                       ByRef nMaxFolderIdx As Integer, _
                                       ByRef sSourceFolders() As String, _
                                       ByRef sSourceDevice() As String, _
                                       Optional ByRef bError As Boolean = False)
        '********************************************************************************************
        'Description:  copy files from robot controllers
        '
        'Parameters: sFiles - list of files from chkFiles, or empty for *.* transfers
        '            nMaxFolderIdx - max folder index (folder count-1)
        '            sSourceFolders - folder to copy from (if it's a controller, it'll be the controller name)
        '            sSourceDevice - controller name if it's a controller or folder assigned to a controller
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/03/10  MSW     Reconnect FTP for error recovery
        ' 01/03/13  RJO     Added Copy Fail status if robot is offline.
        ' 07/11/13  MSW     Track errors for improved status display
        '********************************************************************************************
        Dim sFTPDevice As String = String.Empty
        'Get file mask from the cbo.  They can type it in, too.
        Dim sMask As String = cboSourceFileType.Text.ToLower
        sMask = Replace(sMask, ",", " ")
        sMask = Replace(sMask, ";", " ")
        Dim sMaskList() As String = Split(sMask, " ")
        For nFolder As Integer = 0 To nMaxFolderIdx
            'Map the global folder names to the local vars
            sSourceDevice(nFolder) = msSourceSubFolders(nFolder)
            'Need the FanucName for FTP, we're working with {nothing}Name in the list
            For Each oController As clsController In colControllers
                If oController.Name = sSourceDevice(nFolder) Then
                    sFTPDevice = oController.FanucName
                    Exit For
                End If
            Next
            Dim oFTP As clsFSFtp = Nothing
            Try
                'Make a temp folder
                sSourceFolders(nFolder) = sGetTmpFileName(sFTPDevice, String.Empty)
                IO.Directory.CreateDirectory(sSourceFolders(nFolder))
                mcolTempFolders.Add(sSourceFolders(nFolder))
                'Open a connection to the robot
                oFTP = New clsFSFtp(sFTPDevice)
                With oFTP
                    If .Connected Then
                        'set the working dir on the host robot controller
                        Dim sDevName() As String = Split(cboSourceRobotDev.Text, " ")
                        .WorkingDir = sDevName(0)
                        If rdoSourceSelectFiles.Checked = False Then
                            Dim sTemp2 As String = gpsRM.GetString("psREADING_DIR_FROM") & gpsRM.GetString("psCONTROLLER") & sFTPDevice
                            Status = sTemp2
                            lstSummary.Items.Add(sTemp2)
                            sFiles = Nothing
                            Application.DoEvents()
                            If mbCancel Then
                                Exit Sub
                            End If
                            ReDim sFiles(1)
                            'Get a directory listing
                            Dim nIndex As Integer = 0
                            For Each sTmp As String In sMaskList
                                If (InStr(sTmp, "*") > 0) Or (InStr(sTmp, ".") > 0) Then
                                    Dim sDir() As String = .Directory(sTmp)
                                    ReDim Preserve sFiles(sFiles.GetUpperBound(0) + sDir.GetUpperBound(0) + 2)
                                    If sDir IsNot Nothing Then
                                        For Each sFile As String In sDir
                                            If sFile IsNot Nothing AndAlso sFile <> String.Empty Then
                                                sFiles(nIndex) = sFile
                                                nIndex += 1
                                            End If
                                        Next
                                    End If
                                    ReDim Preserve sFiles(nIndex - 1)
                                End If
                            Next
                        Else
                            'Use the sFiles filled in at the top of the routine
                        End If
                        Application.DoEvents()
                        If mbCancel Then
                            Exit Sub
                        End If
                        Dim sTemp3 As String = gpsRM.GetString("psCOPYING_FILES_FROM") & gpsRM.GetString("psCONTROLLER") & sFTPDevice
                        Status = sTemp3
                        lstSummary.Items.Add(sTemp3)
                        'Run through the file list and get each file
                        .DestDir = sSourceFolders(nFolder)
                        For Each sFile As String In sFiles
                            Application.DoEvents()
                            If mbCancel Then
                                Exit For
                            End If
                            ' "Copying " & sFile & " from " & sSourceFolders(nFolder) & "\ to " & sDestFolders(nFolder)
                            Status = gpsRM.GetString("psCOPYING") & sFile & gpsRM.GetString("psFROM") & _
                                gpsRM.GetString("psCONTROLLER") & sFTPDevice
                            Try
                                If .GetFile(sFile, sFile) Then
                                    'OK, get the next one
                                Else
                                    Dim sTmp As String = gpsRM.GetString("psFAILED_TO_COPY") & sFile & gpsRM.GetString("psFROM") & _
                                            gpsRM.GetString("psCONTROLLER") & sFTPDevice
                                    Status = sTmp
                                    lstSummary.Items.Add(sTmp)
                                    sTmp = .ErrorMsg
                                    Status = sTmp
                                    lstSummary.Items.Add(sTmp)
                                    oFTP.Close() 'MSW 11/19/10 - need more work on error recovery
                                    oFTP = New clsFSFtp(sFTPDevice)
                                    .Connect()
                                    Application.DoEvents()
                                    .WorkingDir = sDevName(0)
                                    .DestDir = sSourceFolders(nFolder)
                                End If
                            Catch ex As Exception
                                Dim sTmp As String = gpsRM.GetString("psFAILED_TO_COPY") & sFile & gpsRM.GetString("psFROM") & _
                                        gpsRM.GetString("psCONTROLLER") & sFTPDevice
                                Status = sTmp
                                lstSummary.Items.Add(sTmp)
                                oFTP.Close() 'MSW 11/19/10 - need more work on error recovery
                                oFTP = New clsFSFtp(sFTPDevice)
                                .Connect()
                                Application.DoEvents()
                                .WorkingDir = sDevName(0)
                                .DestDir = sSourceFolders(nFolder)
                                bError = True
                            End Try
                        Next
                    Else
                        Status = sFTPDevice & gpsRM.GetString("psIS_OFFLINE")
                        lstSummary.Items.Add(gpsRM.GetString("psCOPY_FAIL") & " " & _
                                             sFTPDevice & gpsRM.GetString("psIS_OFFLINE"))
                        bError = True
                    End If
                End With
            Catch ex As Exception
                Trace.WriteLine(ex.Message)
                Trace.WriteLine(ex.StackTrace)
                ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                    Status, MessageBoxButtons.OK)
                bError = True
            Finally
                If oFTP IsNot Nothing Then
                    oFTP.Close()
                End If
            End Try
        Next

    End Sub
    Private Sub subCopyToFolders(ByRef sFiles() As String, _
                                       ByRef nMaxFolderIdx As Integer, _
                                       ByRef sSourceFolders() As String, _
                                       ByRef sSourceDevice() As String, _
                                       ByRef bError As Boolean)
        '********************************************************************************************
        'Description:  copy files to folders
        '
        'Parameters: sFiles - list of files from chkFiles, or empty for *.* transfers
        '            nMaxFolderIdx - max folder index (folder count-1)
        '            sSourceFolders - folder to copy from (if it's a controller, it'll be the controller name)
        '            sSourceDevice - controller name if it's a controller or folder assigned to a controller
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sChangeLogText As String = String.Empty
        Dim sChangeLogDevice As String = String.Empty
        'Get file mask from the cbo.  They can type it in, too.
        Dim sMask As String = cboSourceFileType.Text.ToLower
        sMask = Replace(sMask, ",", " ")
        sMask = Replace(sMask, ";", " ")
        Dim sMaskList() As String = Split(sMask, " ")
        'Copy to folders
        'Figure out the folder names (like the source does in subProcessSource)
        Dim bGotPath As Boolean = False
        Dim sPath As String = String.Empty
        Dim sDestFolders(nMaxFolderIdx) As String
        frmOverWrite.caption = gpsRM.GetString("psOVERWRITE_FILES")
        frmOverWrite.btnNo.Text = gpsRM.GetString("psNO")
        frmOverWrite.btnYes.Text = gpsRM.GetString("psYES")
        frmOverWrite.btnYesAll.Text = gpsRM.GetString("psYES_TO_ALL")
        Application.DoEvents()
        If mbCancel Then
            Exit Sub
        End If
        sChangeLogDevice = String.Empty
        Select Case cboDestType.Text
            'Standard PW folders
            Case gpsRM.GetString("psTEMP_BACKUPS")
                If mPWCommon.GetDefaultFilePath(sPath, eDir.TempBackups, String.Empty, String.Empty) Then
                    bGotPath = True
                End If
            Case gpsRM.GetString("psMASTER_BACKUPS")
                If mPWCommon.GetDefaultFilePath(sPath, eDir.MasterBackups, String.Empty, String.Empty) Then
                    bGotPath = True
                End If
            Case Else
                'Browsed folder - including PW backups and "select a folder"
                'The root path is the cbo text
                If Strings.Right(cboDestType.Text, 1) = "\" Then
                    sPath = cboDestType.Text
                Else
                    sPath = cboDestType.Text & "\"
                End If
                bGotPath = True
        End Select
        Application.DoEvents()
        If mbCancel Then
            Exit Sub
        End If
        If bGotPath Then
            If (IO.Directory.Exists(sPath) = False) Then
                IO.Directory.CreateDirectory(sPath)
            End If
            For nFolder As Integer = 0 To nMaxFolderIdx
                'For browsed folders
                ' The checklist includes the full path of the root folder if it has files available
                ' plus each robot subfolder if it exists, only really needed for nFolder = 0
                If Strings.Right(chkDestRobots.CheckedItems.Item(0).ToString, 1) = "\" Then
                    If ((chkDestRobots.CheckedItems.Item(0).ToString) = sPath) Then
                        sDestFolders(nFolder) = sPath
                    Else
                        sDestFolders(nFolder) = sPath & chkDestRobots.CheckedItems.Item(nFolder).ToString
                    End If
                Else
                    If ((chkDestRobots.CheckedItems.Item(0).ToString & "\") = sPath) Then
                        sDestFolders(nFolder) = sPath
                    Else
                        sDestFolders(nFolder) = sPath & chkDestRobots.CheckedItems.Item(nFolder).ToString & "\"
                    End If
                End If
                If (IO.Directory.Exists(sDestFolders(nFolder)) = False) Then
                    IO.Directory.CreateDirectory(sDestFolders(nFolder))
                End If
            Next
        Else
            'Don't want to be here
        End If
        Application.DoEvents()
        If mbCancel Then
            Exit Sub
        End If
        For nFolder As Integer = 0 To nMaxFolderIdx
            Dim sChangeLogSource As String = String.Empty
            If msSourceFolder = msROBOTS_TAG Then
                Dim sDevName() As String = Split(cboSourceRobotDev.Text, " ")
                sChangeLogSource = gpsRM.GetString("psCONTROLLER") & msSourceSubFolders(nFolder) & " " & sDevName(0)
            ElseIf msSourceFolder.ToLower.Contains(".zip") Then
                sChangeLogSource = msSourceFolder & " - " & msSourceSubFolders(nFolder)
            Else
                sChangeLogSource = sSourceFolders(nFolder)
            End If
            'Folders identified, do the copy
            If (rdoSourceSelectFiles.Checked) Then
                'Selected files
                Dim bOverWriteAll As Boolean = chkOverWriteFiles.Checked
                For Each sFile As String In chkFiles.CheckedItems
                    Application.DoEvents()
                    If mbCancel Then
                        Exit Sub
                    End If
                    ' "Copying " & sFile & " from " & sSourceFolders(nFolder) & "\ to " & sDestFolders(nFolder)
                    Status = gpsRM.GetString("psCOPYING") & sFile & gpsRM.GetString("psFROM") & sChangeLogSource & _
                             gpsRM.GetString("psTO") & sDestFolders(nFolder)
                    Dim bCopy As Boolean = True
                    If File.Exists(sDestFolders(nFolder) & sFile) Then
                        If bOverWriteAll Then
                            File.Delete(sDestFolders(nFolder) & sFile)
                            bCopy = True
                        Else
                            frmOverWrite.label = String.Format(gpsRM.GetString("psFILE_EXISTS_DELETE_PROMPT"), sFile, sDestFolders(nFolder))
                            Dim lResult As DialogResult = frmOverWrite.ShowDialog()
                            Select Case lResult
                                Case Windows.Forms.DialogResult.OK, Windows.Forms.DialogResult.Yes
                                    File.Delete(sDestFolders(nFolder) & sFile)
                                    bCopy = True
                                Case Windows.Forms.DialogResult.Ignore
                                    File.Delete(sDestFolders(nFolder) & sFile)
                                    bCopy = True
                                    bOverWriteAll = True
                                Case Else
                                    bCopy = False
                            End Select
                        End If
                    End If
                    If bCopy Then
                        If Strings.Right(sSourceFolders(nFolder), 1) = "\" Then
                            File.Copy(sSourceFolders(nFolder) & sFile, sDestFolders(nFolder) & sFile)
                        Else
                            File.Copy(sSourceFolders(nFolder) & "\" & sFile, sDestFolders(nFolder) & sFile)
                        End If
                    End If

                    sChangeLogText = gpsRM.GetString("psCOPIED") & sFile & gpsRM.GetString("psFROM") & sChangeLogSource & _
                             gpsRM.GetString("psTO") & sDestFolders(nFolder)
                    mPWCommon.AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                              colZones.CurrentZoneNumber, _
                              sChangeLogDevice, String.Empty, _
                              sChangeLogText, colZones.CurrentZone)
                Next
            Else
                'Get a directory listing
                For Each sTmp As String In sMaskList
                    If (InStr(sTmp, "*") > 0) Or (InStr(sTmp, ".") > 0) Then
                        'Copy all
                        sFiles = IO.Directory.GetFiles(sSourceFolders(nFolder), sTmp)
                        Dim nStartFileName As Integer = sSourceFolders(nFolder).Length + 1
                        Dim sFile As String
                        Dim bOverWriteAll As Boolean = chkOverWriteFiles.Checked
                        For Each sFileLong As String In sFiles
                            Application.DoEvents()
                            If mbCancel Then
                                Exit Sub
                            End If
                            sFile = sFileLong.Substring(nStartFileName)
                            If (sFile.Substring(0, 1) = "\") Or (sFile.Substring(0, 1) = "/") Then
                                sFile = sFile.Substring(1)
                            End If
                            ' "Copying " & sFile & " from " & sSourceFolders(nFolder) & "\ to " & sDestFolders(nFolder)
                            Status = gpsRM.GetString("psCOPYING") & sFile & gpsRM.GetString("psFROM") & sChangeLogSource & _
                                     gpsRM.GetString("psTO") & sDestFolders(nFolder)
                            Try
                                Dim bCopy As Boolean = True
                                If File.Exists(sDestFolders(nFolder) & sFile) Then
                                    If bOverWriteAll Then
                                        File.Delete(sDestFolders(nFolder) & sFile)
                                        bCopy = True
                                    Else
                                        frmOverWrite.label = String.Format(gpsRM.GetString("psFILE_EXISTS_DELETE_PROMPT"), sFile, sDestFolders(nFolder))
                                        Dim lResult As DialogResult = frmOverWrite.ShowDialog()
                                        Select Case lResult
                                            Case Windows.Forms.DialogResult.OK, Windows.Forms.DialogResult.Yes
                                                File.Delete(sDestFolders(nFolder) & sFile)
                                                bCopy = True
                                            Case Windows.Forms.DialogResult.Ignore
                                                File.Delete(sDestFolders(nFolder) & sFile)
                                                bCopy = True
                                                bOverWriteAll = True
                                            Case Else
                                                bCopy = False
                                        End Select
                                    End If
                                End If
                                If bCopy Then
                                    If Strings.Right(sSourceFolders(nFolder), 1) = "\" Then
                                        File.Copy(sSourceFolders(nFolder) & sFile, sDestFolders(nFolder) & sFile)
                                    Else
                                        File.Copy(sSourceFolders(nFolder) & "\" & sFile, sDestFolders(nFolder) & sFile)
                                    End If
                                End If

                            Catch ex As Exception
                                Trace.WriteLine(ex.Message)
                                Trace.WriteLine(ex.StackTrace)
                                Debug.Print(ex.Message)
                                ' "Copying " & sFile & " from " & sSourceFolders(nFolder) & "\ to " & sDestFolders(nFolder)
                                Dim sTemp As String = gpsRM.GetString("psFAILED_TO_COPY") & sFile & gpsRM.GetString("psFROM") & sChangeLogSource & _
                                         gpsRM.GetString("psTO") & sDestFolders(nFolder)
                                Status = sTemp
                                lstSummary.Items.Add(sTemp)
                                bError = True
                            End Try
                        Next

                    End If
                Next
                sChangeLogText = gpsRM.GetString("psCOPIED") & cboSourceFileType.Text & gpsRM.GetString("psFROM") & sChangeLogSource & _
                         gpsRM.GetString("psTO") & sDestFolders(nFolder)
                mPWCommon.AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                          colZones.CurrentZoneNumber, _
                          sChangeLogDevice, String.Empty, _
                          sChangeLogText, colZones.CurrentZone)
            End If
        Next
    End Sub
    Private Sub subCopyToZip(ByRef sFiles() As String, _
                                       ByRef nMaxFolderIdx As Integer, _
                                       ByRef sSourceFolders() As String, _
                                       ByRef sSourceDevice() As String, _
                                       ByRef bError As Boolean)
        '********************************************************************************************
        'Description:  copy files to zip files
        '
        'Parameters: sFiles - list of files from chkFiles, or empty for *.* transfers
        '            nMaxFolderIdx - max folder index (folder count-1)
        '            sSourceFolders - folder to copy from (if it's a controller, it'll be the controller name)
        '            sSourceDevice - controller name if it's a controller or folder assigned to a controller
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/03/10  MSW     Add details to change log
        '********************************************************************************************
        Dim sChangeLogText As String = String.Empty
        Dim sChangeLogDevice As String = String.Empty
        'Get file mask from the cbo.  They can type it in, too.
        Dim sMask As String = cboSourceFileType.Text.ToLower
        sMask = Replace(sMask, ",", " ")
        sMask = Replace(sMask, ";", " ")
        Dim sMaskList() As String = Split(sMask, " ")
        Dim sPath As String = String.Empty
        Dim sDestFolders(nMaxFolderIdx) As String
        Application.DoEvents()
        If mbCancel Then
            Exit Sub
        End If
        sChangeLogDevice = String.Empty
        'Browsed folder - including PW backups and "select a folder"
        'The root path is the cbo text
        sPath = cboDestType.Text
        Application.DoEvents()
        If mbCancel Then
            Exit Sub
        End If
        For nFolder As Integer = 0 To nMaxFolderIdx
            'For browsed folders
            ' The checklist includes the full path of the root folder if it has files available
            ' plus each robot subfolder if it exists
            'If the full name is checked, don't do subdirectories in the zip
            If InStr(chkDestRobots.CheckedItems.Item(0).ToString, "\") > 0 Then
                sDestFolders(nFolder) = ""
            Else 'If individual robots are checked, use subdirectories in the zip
                sDestFolders(nFolder) = chkDestRobots.CheckedItems.Item(nFolder).ToString
            End If
        Next
        Dim bNew As Boolean = True
        If File.Exists(sPath) Then
            frmOverWrite.caption = gpsRM.GetString("psZIP_FILE_EXISTS")
            frmOverWrite.btnNo.Text = gpsRM.GetString("psUPDATE")
            frmOverWrite.btnYes.Text = gpsRM.GetString("psREPLACE")
            frmOverWrite.btnYesAll.Text = gpsRM.GetString("psCANCEL")
            frmOverWrite.label = String.Format(gpsRM.GetString("psZIP_FILE_EXISTS_PROMPT"), sPath)
            Dim lResult As DialogResult = frmOverWrite.ShowDialog()
            Select Case lResult
                Case Windows.Forms.DialogResult.Yes
                    File.Delete(sPath)
                Case Windows.Forms.DialogResult.No
                    bNew = False
                Case Windows.Forms.DialogResult.Ignore 'Really yes to all
                    File.Delete(sPath)
                Case Else
            End Select
            frmOverWrite.caption = gpsRM.GetString("psOVERWRITE_FILES")
            frmOverWrite.btnNo.Text = gpsRM.GetString("psNO")
            frmOverWrite.btnYes.Text = gpsRM.GetString("psYES")
            frmOverWrite.btnYesAll.Text = gpsRM.GetString("psYES_TO_ALL")
        End If
        Application.DoEvents()
        If mbCancel Then
            Exit Sub
        End If
        Dim oZip As ZipFile = Nothing
        Try
            If bNew Then
                oZip = New ZipFile
            Else
                oZip = ZipFile.Read(sPath)
            End If
            Dim nFileCount As Integer = oZip.EntryFileNames.Count
            For nFolder As Integer = 0 To nMaxFolderIdx
                Dim sChangeLogSource As String = String.Empty
                If msSourceFolder = msROBOTS_TAG Then
                    Dim sDevName() As String = Split(cboSourceRobotDev.Text, " ")
                    sChangeLogSource = gpsRM.GetString("psCONTROLLER") & msSourceSubFolders(nFolder) & " " & sDevName(0)
                Else
                    sChangeLogSource = sSourceFolders(nFolder)
                End If
                'Folders identified, do the copy
                If (rdoSourceSelectFiles.Checked) Then
                    'Selected files
                    Dim bOverWriteAll As Boolean = chkOverWriteFiles.Checked
                    For Each sFile As String In chkFiles.CheckedItems
                        Application.DoEvents()
                        If mbCancel Then
                            Exit Sub
                        End If
                        ' "Copying " & sFile & " from " & sSourceFolders(nFolder) & "\ to " & sDestFolders(nFolder)
                        Status = gpsRM.GetString("psCOPYING") & sFile & gpsRM.GetString("psFROM") & sChangeLogSource & _
                                 gpsRM.GetString("psTO") & sPath & "\" & sDestFolders(nFolder)
                        Dim bCopy As Boolean = True
                        If (bOverWriteAll = False) AndAlso oZip.ContainsEntry(sDestFolders(nFolder) & "/" & sFile) Then
                            frmOverWrite.label = String.Format(gpsRM.GetString("psFILE_EXISTS_DELETE_PROMPT"), sFile, sPath & "\" & sDestFolders(nFolder))
                            Dim lResult As DialogResult = frmOverWrite.ShowDialog()
                            Select Case lResult
                                Case Windows.Forms.DialogResult.OK, Windows.Forms.DialogResult.Yes
                                    bCopy = True
                                Case Windows.Forms.DialogResult.Ignore
                                    bCopy = True
                                    bOverWriteAll = True
                                Case Else
                                    bCopy = False
                            End Select
                        End If

                        If bCopy Then
                            If Strings.Right(sSourceFolders(nFolder), 1) = "\" Then
                                oZip.UpdateFile(sSourceFolders(nFolder) & sFile, sDestFolders(nFolder))
                            Else
                                oZip.UpdateFile(sSourceFolders(nFolder) & "\" & sFile, sDestFolders(nFolder))
                            End If
                        End If
                        sChangeLogText = gpsRM.GetString("psCOPIED") & sFile & gpsRM.GetString("psFROM") & sChangeLogSource & _
                                 gpsRM.GetString("psTO") & sPath & "\" & sDestFolders(nFolder)
                        mPWCommon.AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                                  colZones.CurrentZoneNumber, _
                                  sChangeLogDevice, String.Empty, _
                                  sChangeLogText, colZones.CurrentZone)
                    Next
                Else
                    'Get a directory listing
                    For Each sTmp As String In sMaskList
                        If (InStr(sTmp, "*") > 0) Or (InStr(sTmp, ".") > 0) Then
                            'Copy all w/ mask
                            sFiles = IO.Directory.GetFiles(sSourceFolders(nFolder), sTmp)
                            Dim nStartFileName As Integer = sSourceFolders(nFolder).Length + 1
                            Dim sFile As String
                            Dim bOverWriteAll As Boolean = chkOverWriteFiles.Checked
                            For Each sFileLong As String In sFiles
                                Application.DoEvents()
                                If mbCancel Then
                                    Exit Sub
                                End If
                                sFile = sFileLong.Substring(nStartFileName)
                                If (sFile.Substring(0, 1) = "\") Or (sFile.Substring(0, 1) = "/") Then
                                    sFile = sFile.Substring(1)
                                End If
                                ' "Copying " & sFile & " from " & sSourceFolders(nFolder) & "\ to " & sDestFolders(nFolder)
                                Status = gpsRM.GetString("psCOPYING") & sFile & gpsRM.GetString("psFROM") & sChangeLogSource & _
                                         gpsRM.GetString("psTO") & sPath & "\" & sDestFolders(nFolder)
                                Try
                                    Dim bCopy As Boolean = True
                                    If (bOverWriteAll = False) AndAlso oZip.ContainsEntry(sDestFolders(nFolder) & "/" & sFile) Then
                                        frmOverWrite.label = String.Format(gpsRM.GetString("psFILE_EXISTS_DELETE_PROMPT"), sFile, sPath & "\" & sDestFolders(nFolder))
                                        Dim lResult As DialogResult = frmOverWrite.ShowDialog()
                                        Select Case lResult
                                            Case Windows.Forms.DialogResult.OK, Windows.Forms.DialogResult.Yes
                                                bCopy = True
                                            Case Windows.Forms.DialogResult.Ignore
                                                bCopy = True
                                                bOverWriteAll = True
                                            Case Else
                                                bCopy = False
                                        End Select
                                    End If

                                    If bCopy Then
                                        oZip.UpdateFile(sFileLong, sDestFolders(nFolder))
                                    End If

                                Catch ex As Exception
                                    Trace.WriteLine(ex.Message)
                                    Trace.WriteLine(ex.StackTrace)
                                    Debug.Print(ex.Message)
                                    ' "Copying " & sFile & " from " & sSourceFolders(nFolder) & "\ to " & sDestFolders(nFolder)
                                    Dim sTemp As String = gpsRM.GetString("psFAILED_TO_COPY") & sFile & gpsRM.GetString("psFROM") & sChangeLogSource & _
                                             gpsRM.GetString("psTO") & sDestFolders(nFolder)
                                    Status = sTemp
                                    lstSummary.Items.Add(sTemp)
                                    bError = True
                                End Try
                            Next

                        End If
                    Next
                    sChangeLogText = gpsRM.GetString("psCOPIED") & cboSourceFileType.Text & gpsRM.GetString("psFROM") & sChangeLogSource & _
                             gpsRM.GetString("psTO") & sPath
                    If sDestFolders(nFolder) <> String.Empty Then
                        sChangeLogText = sChangeLogText & ", " & gpsRM.GetString("psPATH") & sDestFolders(nFolder)
                    End If
                    mPWCommon.AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                              colZones.CurrentZoneNumber, _
                              sChangeLogDevice, String.Empty, _
                              sChangeLogText, colZones.CurrentZone)
                End If
            Next
            oZip.Save(sPath)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            Debug.Print(ex.Message)
            ' "Copying " & sFile & " from " & sSourceFolders(nFolder) & "\ to " & sDestFolders(nFolder)
            Dim sTemp As String = gpsRM.GetString("psFAILED_TO_COPY") & sPath
            Status = sTemp
            lstSummary.Items.Add(sTemp)
            bError = True
        Finally
            If oZip IsNot Nothing Then
                oZip.Dispose()
                oZip = Nothing
            End If
        End Try
    End Sub
    Private Sub SubCopyToControllers(ByRef sFiles() As String, _
                                   ByRef nMaxFolderIdx As Integer, _
                                   ByRef sSourceFolders() As String, _
                                   ByRef sSourceDevice() As String, _
                                   ByRef bError As Boolean)
        '********************************************************************************************
        'Description:  copy files to robot controllers
        '
        'Parameters: sFiles - list of files from chkFiles, or empty for *.* transfers
        '            nMaxFolderIdx - max folder index (folder count-1)
        '            sSourceFolders - folder to copy from (if it's a controller, it'll be the controller name)
        '            sSourceDevice - controller name if it's a controller or folder assigned to a controller
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/03/13  RJO     Added Copy Fail status if robot is offline.
        ' 06/01/13  AC      If (sFile.Substring(0, 6).ToLower = "-bcked") failed if sFile was lees
        '                   than 6 chars.
        ' 07/11/13  MSW     Track errors for improved status display
        ' 04/21/14  MSW     Fix some lengths for file type checking
        '********************************************************************************************
        Dim sChangeLogText As String = String.Empty
        Dim sChangeLogDevice As String = String.Empty
        Dim sFTPDevice As String = String.Empty
        'Get file mask from the cbo.  They can type it in, too.
        Dim sMask As String = cboSourceFileType.Text.ToLower
        sMask = Replace(sMask, ",", " ")
        sMask = Replace(sMask, ";", " ")
        Dim sMaskList() As String = Split(sMask, " ")
        'copy to robots
        Dim sDestFolders(nMaxFolderIdx) As String
        sChangeLogDevice = String.Empty
        Dim bCopyVAR As Boolean = mScreenPrivilege(eScreenPrivilege.RestoreVAR)
        Dim bCopyTP As Boolean = mScreenPrivilege(eScreenPrivilege.RestoreTP)
        For nFolder As Integer = 0 To nMaxFolderIdx
            'Map the global folder names to the local vars
            sDestFolders(nFolder) = chkDestRobots.CheckedItems(nFolder).ToString
            'Need the FanucName for FTP, we're working with {nothing}Name in the list
            For Each oController As clsController In colControllers
                If oController.Name = sDestFolders(nFolder) Then
                    sFTPDevice = oController.FanucName
                    Exit For
                End If
            Next
            sChangeLogDevice = sFTPDevice
            Dim oFTP As clsFSFtp = Nothing
            Try
                'Make a temp folder
                'Open a connection to the robot
                oFTP = New clsFSFtp(sFTPDevice)
                With oFTP
                    If .Connected Then
                        'set the working dir on the host robot controller
                        Dim sDevName() As String = Split(cboDestRobotDev.Text, " ")
                        .WorkingDir = sDevName(0)
                        'Folders identified, do the copy
                        If (rdoSourceSelectFiles.Checked) Then
                            'Selected files
                            ReDim sFiles(chkFiles.CheckedItems.Count - 1)
                            For nFile As Integer = 0 To (chkFiles.CheckedItems.Count - 1)
                                sFiles(nFile) = chkFiles.CheckedItems(nFile).ToString
                            Next
                        Else
                            'Copy all
                            'Get a directory listing
                            Dim sFilesTmp() As String = Nothing
                            Dim nIndex As Integer = 0
                            For Each sTmp As String In sMaskList
                                If (InStr(sTmp, "*") > 0) Or (InStr(sTmp, ".") > 0) Then
                                    sFilesTmp = IO.Directory.GetFiles(sSourceFolders(nFolder), sTmp)
                                    Dim nStartFileName As Integer = sSourceFolders(nFolder).Length + 1
                                    Dim bOverWriteAll As Boolean = chkOverWriteFiles.Checked
                                    ReDim Preserve sFiles(nIndex + sFilesTmp.GetUpperBound(0))
                                    For nFile As Integer = 0 To sFilesTmp.GetUpperBound(0)
                                        sFilesTmp(nFile) = sFilesTmp(nFile).Substring(nStartFileName)
                                        If (sFilesTmp(nFile).Substring(0, 1) = "\") Or (sFilesTmp(nFile).Substring(0, 1) = "/") Then
                                            sFilesTmp(nFile) = sFilesTmp(nFile).Substring(1)
                                        End If
                                        sFiles(nIndex) = sFilesTmp(nFile)
                                        nIndex += 1
                                    Next
                                End If
                            Next
                        End If
                        'Run through the file list and send each file
                        .SourceDir = sSourceFolders(nFolder)
                        For Each sFile As String In sFiles
                            'Mask files for a copy to a robot only, not copy from a robot
                            Dim bCopy As Boolean = False
                            'Mask copy to MD: or MDB:
                            If sDevName(0).ToLower.StartsWith("md") Then
                                'If (sFile.Substring(0, 6).ToLower = "-bcked") Then
                                If (sFile.Contains("-bcked")) Then 'AC 06/01/13
                                    bCopy = False 'Skip current background edit
                                ElseIf (bCopyVAR And ((sFile.Substring(sFile.Length - 2).ToLower = "tp") Or _
                                   (sFile.Substring(sFile.Length - 2).ToLower = "vr") Or _
                                   (sFile.Substring(sFile.Length - 2).ToLower = "sv") Or _
                                   (sFile.Substring(sFile.Length - 2).ToLower = "io") Or _
                                   ((sFile.Substring(sFile.Length - 2).ToLower = "ls") And (rdoSourceSelectFiles.Checked)) Or _
                                   (sFile.Substring(sFile.Length - 2).ToLower = "pc") Or _
                                   (sFile.Substring(sFile.Length - 2).ToLower = "mn") Or _
                                   (sFile.Substring(sFile.Length - 2).ToLower = "ml") Or _
                                   (sFile.Substring(sFile.Length - 3).ToLower = "pmc") Or _
                                   (sFile.Substring(sFile.Length - 3).ToLower = "vis") Or _
                                   (sFile.Substring(sFile.Length - 3).ToLower = "clb") Or _
                                   (sFile.Substring(sFile.Length - 3).ToLower = "mdl") Or _
                                   (sFile.Substring(sFile.Length - 3).ToLower = "prm"))) Or _
                                   (bCopyTP And ((sFile.Substring(sFile.Length - 2).ToLower = "tp") Or _
                                   ((sFile.Substring(sFile.Length - 2).ToLower = "ls") And (rdoSourceSelectFiles.Checked)))) Then
                                    'Still doing the paranoid masking for transfers to the robot.
                                    'Checking all the file types I think you can load.
                                    ' *.ls can be loaded with ASCII upload option, but this screen is requiring that they're 
                                    ' selected individually to prevent err*.ls from being uploaded with a full backup
                                    bCopy = True
                                Else
                                    bCopy = False
                                End If
                            Else
                                bCopy = mScreenPrivilege(eScreenPrivilege.RobotDevices) 'other devices, do whatever they want
                            End If
                            If bCopy Then
                                ' "Copying " & sFile & " from " & sSourceFolders(nFolder) & "\ to " & sDestFolders(nFolder)
                                Status = gpsRM.GetString("psCOPYING") & sSourceFolders(nFolder) & "\" & sFile & gpsRM.GetString("psTO") & _
                                    gpsRM.GetString("psCONTROLLER") & sFTPDevice
                                Try
                                    'Set error text here for exception handling
                                    sChangeLogText = gpsRM.GetString("psFAILED_TO_COPY") & sSourceFolders(nFolder) & "\" & sFile & gpsRM.GetString("psTO") & _
                                                gpsRM.GetString("psCONTROLLER") & sFTPDevice
                                    If .PutFile(sFile, sFile) Then
                                        'OK, set the good status here
                                        sChangeLogText = gpsRM.GetString("psCOPIED") & sFile & gpsRM.GetString("psFROM") & sSourceFolders(nFolder) & _
                                            gpsRM.GetString("psTO") & sFTPDevice
                                    Else
                                        bError = True
                                        Status = .ErrorMsg
                                        lstSummary.Items.Add(.ErrorMsg)
                                    End If
                                Catch ex As Exception
                                    Status = .ErrorMsg
                                    lstSummary.Items.Add(.ErrorMsg)
                                    bError = True
                                Finally
                                    mPWCommon.AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                                        colZones.CurrentZoneNumber, _
                                        sChangeLogDevice, String.Empty, _
                                        sChangeLogText, colZones.CurrentZone)
                                    Status = sChangeLogText
                                End Try
                            End If
                        Next
                    Else
                        'RJO 01/03/13
                        Status = sFTPDevice & gpsRM.GetString("psIS_OFFLINE")
                        lstSummary.Items.Add(gpsRM.GetString("psCOPY_FAIL") & " " & _
                                             sFTPDevice & gpsRM.GetString("psIS_OFFLINE"))
                        bError = True
                    End If '.Connected
                End With
            Catch ex As Exception
                Trace.WriteLine(ex.Message)
                Trace.WriteLine(ex.StackTrace)
                ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                    Status, MessageBoxButtons.OK)
                bError = True
            Finally
                If oFTP IsNot Nothing Then
                    oFTP.Close()
                End If
            End Try
        Next

    End Sub
    Private Sub subDoCopy()
        '********************************************************************************************
        'Description:  Do the copy
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/11/10  MSW     Redo a directory after deleting files
        ' 12/16/10  MSW     Fix some weak spots in the multifolder zip copy

        '********************************************************************************************
        Try
            If mbCopyActive Then
                mbCancel = True
                Exit Sub
            Else
                mbCopyActive = True
                mbCancel = False
            End If
            Dim bError As Boolean = False
            btnDoCopy.Text = gcsRM.GetString("csCANCEL")
            subEnableControls(False, , , , , True)
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            Dim sChangeLogText As String = String.Empty
            Dim sChangeLogDevice As String = String.Empty


            btnStatus.CheckState = CheckState.Checked
            lstStatus.Visible = True

            'If we use a file list, move it to a local string array here
            Dim sFiles() As String = Nothing
            If rdoSourceSelectFiles.Checked = True Then
                ReDim sFiles(chkFiles.CheckedItems.Count - 1)
                For nFile As Integer = 0 To (chkFiles.CheckedItems.Count - 1)
                    sFiles(nFile) = chkFiles.CheckedItems.Item(nFile).ToString
                Next
            End If

            Application.DoEvents()
            If mbCancel Then
                Exit Sub
            End If
            If btnDeleteFiles.Checked Then
                'Run delete operation
                If msSourceFolder = msROBOTS_TAG Then
                    subDeleteFromController(sFiles, bError)
                Else
                    subDeleteFromFolder(sFiles, bError)
                End If
                If mbCancel Then
                    lstSummary.Items.Add(gpsRM.GetString("psOPERATION_CANCELLED"))
                    mbCancel = False
                Else
                    If bError Then
                        lstSummary.Items.Add(gpsRM.GetString("psOPERATION_COMPLETE_WITHERR"))
                    Else
                        lstSummary.Items.Add(gpsRM.GetString("psOPERATION_COMPLETE"))
                    End If
                End If
                mbProcessSourceSelection = True ' Redo a directory after deleting files
            Else
                'Run a copy operation
                'Start by identifying all the source folders
                Dim nMaxFolderIdx As Integer = mnSourceSubFolderCount - 1
                Dim sSourceFolders(nMaxFolderIdx) As String
                Dim sSourceDevice(nMaxFolderIdx) As String
                If msSourceFolder = msROBOTS_TAG Then
                    'Copy to temp folders
                    SubCopyFromControllers(sFiles, nMaxFolderIdx, sSourceFolders, sSourceDevice, bError)
                ElseIf msSourceFolder.ToLower.Contains(".zip") Then
                    Dim sTmpEnd As String = msSourceFolder.Substring(msSourceFolder.Length - 1)
                    If sTmpEnd = "\" Or sTmpEnd = "/" Then
                        msSourceFolder = msSourceFolder.Substring(0, msSourceFolder.Length - 1)
                    End If
                    'Zip file
                    For nFolder As Integer = 0 To nMaxFolderIdx
                        'If msSourceSubFolders(nFolder).ToLower.Contains(".zip") Then
                        '    SubExtractFromZip(msSourceFolder, String.Empty, sFiles, sSourceFolders(0))
                        'Else
                        SubExtractFromZip(msSourceFolder, msSourceSubFolders(nFolder), sFiles, sSourceFolders(nFolder), bError)
                        'End If
                    Next
                Else
                    'Map the global folder names to the local vars
                    For nFolder As Integer = 0 To nMaxFolderIdx
                        If msSourceSubFolders(nFolder).Contains("\") Or msSourceSubFolders(nFolder).Contains("/") Or msSourceSubFolders(nFolder).Contains(":") Then
                            sSourceFolders(nFolder) = msSourceSubFolders(nFolder)
                        Else
                            If msSourceFolder.Substring(msSourceFolder.Length - 1) = "\" Then
                                sSourceFolders(nFolder) = msSourceFolder & msSourceSubFolders(nFolder)
                            Else
                                sSourceFolders(nFolder) = msSourceFolder & "\" & msSourceSubFolders(nFolder)
                            End If
                        End If
                        sSourceDevice(nFolder) = msSourceSubFolders(nFolder)
                    Next
                End If
                Application.DoEvents()
                If mbCancel Then
                    lstSummary.Items.Add(gpsRM.GetString("psCOPY_CANCELLED"))
                Else
                    'For the case of 1 source to multiple destinations, repeat the source folder names to match
                    If nMaxFolderIdx = 0 And chkDestRobots.CheckedItems.Count > 1 Then
                        nMaxFolderIdx = chkDestRobots.CheckedItems.Count - 1
                        ReDim Preserve sSourceFolders(nMaxFolderIdx)
                        ReDim Preserve sSourceDevice(nMaxFolderIdx)
                        For nFolder As Integer = 1 To nMaxFolderIdx
                            sSourceFolders(nFolder) = sSourceFolders(0)
                            sSourceDevice(nFolder) = sSourceFolders(0)
                        Next
                    End If

                    'Destinations
                    If cboDestType.Text = gpsRM.GetString("psROBOT_CONTROLLERS") Then
                        SubCopyToControllers(sFiles, nMaxFolderIdx, sSourceFolders, sSourceDevice, bError)
                    Else
                        If cboDestType.Text.ToLower.Contains(".zip") Then
                            subCopyToZip(sFiles, nMaxFolderIdx, sSourceFolders, sSourceDevice, bError)
                        Else
                            subCopyToFolders(sFiles, nMaxFolderIdx, sSourceFolders, sSourceDevice, bError)
                        End If

                    End If
                    If mbCancel Then
                        lstSummary.Items.Add(gpsRM.GetString("psCOPY_CANCELLED"))
                        mbCancel = False
                    Else
                        If bError Then
                            lstSummary.Items.Add(gpsRM.GetString("psCOPY_COMPLETE_WITHERR"))
                        Else
                            lstSummary.Items.Add(gpsRM.GetString("psCOPY_COMPLETE"))
                        End If
                    End If
                End If
                If msSourceFolder = msROBOTS_TAG Then
                    'Copy to temp folders
                    For Each sTmp As String In sSourceFolders
                        Try
                            IO.Directory.Delete(sTmp, True)
                            If mcolTempFolders.Contains(sTmp) Then
                                mcolTempFolders.Remove(sTmp)
                            End If
                        Catch ex As Exception
                            'Don't worry about, it'll get cleaned up by PWMaint
                        End Try
                    Next
                End If
            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
        Finally
            mPWCommon.SaveToChangeLog(colZones.ActiveZone)
            Status(True) = gpsRM.GetString("psREADY")
            btnDoCopy.Text = gpsRM.GetString("psDO_COPY")
            subEnableControls(True, , , , True)
            mbCopyActive = False
        End Try
        Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub
    Private Sub subDoSummary()
        '********************************************************************************************
        'Description:  Display a summary of the operation on the Do Copy tab
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            lstSummary.Items.Clear()
            If mbReadyToCopy = False Then
                Exit Sub
            End If
            Dim sTemp As String
            Dim bNotJustTPFiles As Boolean = False
            btnStatus.CheckState = CheckState.Checked
            lstStatus.Visible = True

            'First select delete or copy and list the source folders or robots
            If btnDeleteFiles.Checked Then
                chkOverWriteFiles.Visible = False
                If msSourceFolder = msROBOTS_TAG Then
                    sTemp = gpsRM.GetString("psDELETE_FILES_CNTR") & msSourceSubFolders(0)
                Else
                    sTemp = gpsRM.GetString("psDELETE_FILES_FROM") & msSourceFolder & msSourceSubFolders(0)
                End If
                lstSummary.Items.Add(sTemp)
                'Check the password
                If (moPassword.UserName <> String.Empty) OrElse (mScreenPrivilege(eScreenPrivilege.Delete) = False) Then
                    lstSummary.Items.Add(gpsRM.GetString("psLOGIN_REQUIRED"))
                End If
            Else
                lstSummary.Items.Add(gpsRM.GetString("psCOPY_FILES"))
                If msSourceFolder = msROBOTS_TAG Then
                    lstSummary.Items.Add(gpsRM.GetString("psCOPY_SOURCE"))
                    If mnSourceSubFolderCount = 1 Then
                        sTemp = gpsRM.GetString("psCONTROLLER") & msSourceSubFolders(0)
                    Else
                        sTemp = gpsRM.GetString("psCONTROLLERS")
                        For Each sFldr As String In msSourceSubFolders
                            sTemp = sTemp & sFldr & ", "
                        Next
                        sTemp = sTemp.Substring(0, sTemp.Length - 2)
                    End If
                    lstSummary.Items.Add(sTemp)
                    lstSummary.Items.Add("  " & gpsRM.GetString("psDEVICE") & cboSourceRobotDev.Text)
                Else
                    lstSummary.Items.Add(gpsRM.GetString("psCOPY_SOURCE") & cboSourceType.Text)
                    If mnSourceSubFolderCount = 1 Then
                        sTemp = gpsRM.GetString("psFOLDER") & msSourceSubFolders(0)
                    Else
                        sTemp = gpsRM.GetString("psFOLDERS")
                        For Each sFldr As String In msSourceSubFolders
                            sTemp = sTemp & sFldr & ", "
                        Next
                        sTemp = sTemp.Substring(0, sTemp.Length - 2)
                    End If
                    lstSummary.Items.Add("  " & sTemp)
                End If
            End If
            'List the files if there is a list
            If (mnSourceSubFolderCount = 1) And (rdoSourceSelectFiles.Checked) Then
                lstSummary.Items.Add(gpsRM.GetString("psFILES_HEADING"))
                Dim iCount As Integer = 0
                sTemp = "  "
                For Each sFile As String In chkFiles.CheckedItems
                    sTemp = sTemp & sFile
                    If (sFile.Length > 3) AndAlso (sFile.Substring(sFile.Length - 2).ToLower <> "tp") Then
                        bNotJustTPFiles = True
                    End If

                    If iCount > 8 Then
                        lstSummary.Items.Add(sTemp)
                        iCount = 0
                        sTemp = "  "
                    Else
                        iCount += 1
                        sTemp = sTemp & ", "
                    End If
                Next
                If iCount > 0 Then
                    lstSummary.Items.Add(sTemp.Substring(0, sTemp.Length - 2))
                End If
            Else
                'No list, just print the mask
                lstSummary.Items.Add("  " & gpsRM.GetString("psFILE_MASK") & cboSourceFileType.Text)
                'Get file mask from the cbo.  They can type it in, too.
                Dim sMask As String = cboSourceFileType.Text.ToLower
                sMask = Replace(sMask, ",", " ")
                sMask = Replace(sMask, ";", " ")
                Dim sMaskList() As String = Split(sMask, " ")
                bNotJustTPFiles = False
                For Each sTmp As String In sMaskList
                    If sTmp.Contains("*") Or sTmp.Contains(".") Then
                        If (sTmp.Contains(".tp") = False) Then
                            bNotJustTPFiles = True
                        End If
                    End If
                Next
            End If
            If Not (btnDeleteFiles.Checked) Then
                'Destination
                lstSummary.Items.Add(gpsRM.GetString("psDESTINATION") & cboDestType.Text)
                If cboDestType.Text = gpsRM.GetString("psROBOT_CONTROLLERS") Then
                    chkOverWriteFiles.Visible = False
                    If chkDestRobots.CheckedItems.Count = 1 Then
                        sTemp = gpsRM.GetString("psCONTROLLER") & chkDestRobots.CheckedItems(0).ToString
                    Else
                        sTemp = gpsRM.GetString("psCONTROLLERS")
                        For Each oFldr As Object In chkDestRobots.CheckedItems
                            sTemp = sTemp & oFldr.ToString & ", "
                        Next
                        sTemp = sTemp.Substring(0, sTemp.Length - 2)
                    End If
                    lstSummary.Items.Add("  " & sTemp)
                    lstSummary.Items.Add("  " & gpsRM.GetString("psDEVICE") & cboDestRobotDev.Text)
                    If (moPassword.UserName = String.Empty) Then
                        lstSummary.Items.Add(gpsRM.GetString("psLOGIN_REQUIRED"))
                    Else
                        If (cboDestRobotDev.Text.Substring(0, 2).ToLower <> "md") And (mScreenPrivilege(eScreenPrivilege.RobotDevices) = False) Then
                            lstSummary.Items.Add(gpsRM.GetString("psLOGIN_FOR_ACCESS_TO") & cboDestRobotDev.Text)
                        Else
                            If mScreenPrivilege(eScreenPrivilege.RestoreVAR) = False Then
                                If mScreenPrivilege(eScreenPrivilege.RestoreTP) Then
                                    If bNotJustTPFiles Then
                                        lstSummary.Items.Add(gpsRM.GetString("psLOGIN_FOR_VAR_WRITE"))
                                    End If
                                Else
                                    lstSummary.Items.Add(gpsRM.GetString("psLOGIN_WRITE_TO_ROBOT"))
                                End If
                            End If
                        End If
                    End If
                Else
                    chkOverWriteFiles.Visible = True
                    If chkDestRobots.CheckedItems.Count = 1 Then
                        sTemp = gpsRM.GetString("psFOLDER") & chkDestRobots.CheckedItems(0).ToString
                    Else
                        sTemp = gpsRM.GetString("psFOLDERS")
                        For Each oFldr As Object In chkDestRobots.CheckedItems
                            sTemp = sTemp & oFldr.ToString & ", "
                        Next
                        sTemp = sTemp.Substring(0, sTemp.Length - 2)
                    End If
                    lstSummary.Items.Add("  " & sTemp)
                    If (moPassword.UserName = String.Empty) Then
                        lstSummary.Items.Add(gpsRM.GetString("psLOGIN_REQUIRED"))
                    Else
                        Select Case cboDestType.Text
                            'Standard PW folders
                            Case gpsRM.GetString("psTEMP_BACKUPS")
                                If (mScreenPrivilege(eScreenPrivilege.SaveBackup) = False) Then
                                    lstSummary.Items.Add(gpsRM.GetString("psLOGIN_SAVE_BACKUP"))
                                End If
                            Case gpsRM.GetString("psMASTER_BACKUPS")
                                If (mScreenPrivilege(eScreenPrivilege.MasterBackups) = False) Then
                                    lstSummary.Items.Add(gpsRM.GetString("psLOGIN_MASTER_BACKUPS"))
                                End If
                            Case Else
                                'browsed folder
                                If (mScreenPrivilege(eScreenPrivilege.BrowseFolder) = False) Then
                                    lstSummary.Items.Add(gpsRM.GetString("psLOGIN_BROWSE"))
                                End If
                        End Select

                    End If
                End If
            End If
            'Check file source device
            If (msSourceFolder = msROBOTS_TAG) And (cboSourceRobotDev.Text.Substring(0, 2).ToLower <> "md") And _
                    (mScreenPrivilege(eScreenPrivilege.RobotDevices) = False) Then
                lstSummary.Items.Add(gpsRM.GetString("psLOGIN_FOR_ACCESS_TO") & cboSourceRobotDev.Text)
            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
        Finally
            Status(True) = gpsRM.GetString("psREADY")
        End Try
    End Sub
    Private Sub subProcessSourceSelection()
        '********************************************************************************************
        'Description:  Use all the choices on the source tab to select the folders or robots 
        '               and fill in the file list box if needed.
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/05/14  MSW     subProcessSourceSelection - Short file name fix from inovision
        '********************************************************************************************

        'Process source selection
        Dim sFiles() As String = Nothing
        Dim sPath As String = String.Empty
        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

        'Clear out all the details first
        msSourceFolder = String.Empty
        msSourceSubFolders = Nothing
        mnSourceSubFolderCount = 0
        chkFiles.Items.Clear()
        'Two main choices - robot or file
        Try
            btnStatus.CheckState = CheckState.Checked
            lstStatus.Visible = True
            If rdoSourceAllFiles.Checked Then
                lblFileSelect.Text = gpsRM.GetString("psNO_SEL_COPY_ALL")
            End If
            mbProcessSourceSelection = False
            Status = gpsRM.GetString("psSOURCE_SELECTED") & cboSourceType.Text
            If cboSourceType.Text = gpsRM.GetString("psROBOT_CONTROLLERS") Then
                'Robot controllers for the source, how many?
                mnSourceSubFolderCount = chkSourceRobots.CheckedItems.Count
                If mnSourceSubFolderCount > 0 Then
                    ' Not none, tag the source folder variable and put the robot names in the subfolder array
                    msSourceFolder = msROBOTS_TAG
                    ReDim msSourceSubFolders(mnSourceSubFolderCount - 1)
                    Dim sRobots As String = gpsRM.GetString("psSELECTED_ROBOTS")
                    For nIdx As Integer = 0 To mnSourceSubFolderCount - 1
                        msSourceSubFolders(nIdx) = chkSourceRobots.CheckedItems.Item(nIdx).ToString
                        If nIdx > 0 Then
                            sRobots = sRobots & ", "
                        End If
                        sRobots = sRobots & chkSourceRobots.CheckedItems.Item(nIdx).ToString
                    Next
                    'One source, maybe fill the file message
                    If mnSourceSubFolderCount = 1 Then
                        Status = gpsRM.GetString("psSELECTED_ROBOT") & sRobots
                        If rdoSourceSelectFiles.Checked Then
                            Status = gpsRM.GetString("psREADING_DIRECTORY") & msSourceSubFolders(0)
                            If btnDeleteFiles.Checked Then
                                lblFileSelect.Text = gpsRM.GetString("psSELECT_FILES_DEL") & gpsRM.GetString("psCONTROLLER") & msSourceSubFolders(0)
                            Else
                                lblFileSelect.Text = gpsRM.GetString("psSEL_FILES_COP_FRM") & gpsRM.GetString("psCONTROLLER") & msSourceSubFolders(0)
                            End If
                            'Need the FanucName for FTP, we're working with {nothing}Name in the list
                            Dim sDevice As String = String.Empty
                            For Each oController As clsController In colControllers
                                If oController.Name = msSourceSubFolders(0) Then
                                    sDevice = oController.FanucName
                                End If
                            Next
                            Dim oFTP As clsFSFtp = Nothing
                            Try
                                oFTP = New clsFSFtp(sDevice)
                                With oFTP
                                    If .Connected Then
                                        'set the working dir on the host robot controller
                                        Dim sDevName() As String = Split(cboSourceRobotDev.Text, " ")
                                        .WorkingDir = sDevName(0)

                                        'Get file mask from the cbo.  They can type it in, too.
                                        Dim sMask As String = cboSourceFileType.Text.ToLower
                                        sMask = Replace(sMask, ",", " ")
                                        sMask = Replace(sMask, ";", " ")
                                        Dim sMaskList() As String = Split(sMask, " ")
                                        For Each sTmp As String In sMaskList
                                            If sTmp.Contains("*") Or sTmp.Contains(".") Then
                                                'Get a directory listing
                                                Dim sDir() As String = .Directory(sTmp)

                                                'It returns the full path, so filter that out and add to the list box
                                                If sDir IsNot Nothing Then
                                                    For Each sFile As String In sDir
                                                        If sFile IsNot Nothing AndAlso sFile <> String.Empty Then
                                                            If btnDeleteFiles.Checked Then
                                                                'Only allow TP files to be deleted from the controller memory
                                                                If sDevName(0).ToLower.StartsWith("md") Then
                                                                    Dim nLength As Integer = sFile.Length
                                                                    If (sFile.Length >= 6) Then
                                                                        nLength = 6
                                                                    End If
                                                                    If (sFile.Substring(sFile.Length - 2).ToLower = "tp") And _
                                                                       (sFile.Substring(0, nLength).ToLower <> "-bcked") Then
                                                                        chkFiles.Items.Add(sFile)
                                                                    End If
                                                                Else
                                                                    chkFiles.Items.Add(sFile)
                                                                End If
                                                            Else
                                                                chkFiles.Items.Add(sFile)
                                                            End If
                                                        End If
                                                    Next
                                                End If

                                            End If
                                        Next
                                    End If
                                End With
                            Catch ex As Exception
                            Finally
                                If oFTP IsNot Nothing Then
                                    oFTP.Close()
                                End If
                            End Try
                        End If
                    Else
                        Status = gpsRM.GetString("psSELECTED_ROBOTS") & sRobots
                    End If
                End If
            Else 'If cboSourceType.Text = gpsRM.GetString("psROBOT_CONTROLLERS") Then
                'First find the root folder
                Dim bGotPath As Boolean = False
                Dim bZip As Boolean = False
                Select Case cboSourceType.Text
                    'Standard PW folders
                    Case gpsRM.GetString("psTEMP_BACKUPS")
                        If mPWCommon.GetDefaultFilePath(sPath, eDir.TempBackups, String.Empty, String.Empty) Then
                            bGotPath = True
                        End If
                    Case gpsRM.GetString("psMASTER_BACKUPS")
                        If mPWCommon.GetDefaultFilePath(sPath, eDir.MasterBackups, String.Empty, String.Empty) Then
                            bGotPath = True
                        End If
                    Case Else
                        'Browsed folder - including PW backups and "select a folder"
                        'The root path is the cbo text
                        If Strings.Right(cboSourceType.Text, 1) = "\" Then
                            sPath = cboSourceType.Text
                        Else
                            sPath = cboSourceType.Text & "\"
                        End If
                        bZip = sPath.ToLower.Contains(".zip")
                        bGotPath = True
                End Select
                If bGotPath Then
                    'Got the root directory, first check for multiple source folders
                    mnSourceSubFolderCount = chkSourceRobots.CheckedItems.Count
                    ReDim msSourceSubFolders(mnSourceSubFolderCount - 1)
                    mbProcessSourceSelection = False
                    If mnSourceSubFolderCount = 1 Then
                        If bZip Then
                            'Remove the "\" added earlier
                            If Strings.Right(sPath, 1) = "\" Then
                                sPath = sPath.Substring(0, sPath.Length - 1)
                            Else
                                sPath = sPath.Substring(0, sPath.Length - 1)
                            End If
                            msSourceFolder = sPath
                            msSourceSubFolders(0) = chkSourceRobots.CheckedItems.Item(0).ToString
                            If msSourceSubFolders(0).ToLower.Contains(".zip") Then
                                'Root directory.  Clear out the subdirectory name
                                msSourceSubFolders(0) = String.Empty
                            End If
                        Else
                            'For browsed folders
                            ' The checklist includes the full path of the root folder if it has files available
                            ' plus each robot subfolder if it exists
                            If ((chkSourceRobots.CheckedItems.Item(0).ToString & "\") = sPath) Then
                                msSourceFolder = String.Empty
                            Else
                                Status = gpsRM.GetString("psSELECTED_ROBOT") & chkSourceRobots.CheckedItems.Item(0).ToString
                                If Strings.Right(sPath, 1) = "\" Then
                                    msSourceFolder = sPath
                                Else
                                    msSourceFolder = sPath & "\"
                                End If
                            End If
                            If chkSourceRobots.CheckedItems.Item(0).ToString <> msSourceFolder Then
                                msSourceSubFolders(0) = chkSourceRobots.CheckedItems.Item(0).ToString
                            Else
                                msSourceSubFolders(0) = ""
                            End If
                        End If

                        If rdoSourceSelectFiles.Checked Then
                            Status = gpsRM.GetString("psREADING_DIRECTORY") & msSourceFolder
                            If btnDeleteFiles.Checked Then
                                lblFileSelect.Text = gpsRM.GetString("psSELECT_FILES_DEL") & msSourceFolder
                            Else
                                lblFileSelect.Text = gpsRM.GetString("psSEL_FILES_COP_FRM") & msSourceFolder
                            End If
                            'Get file mask from the cbo.  They can type it in, too.
                            Dim sMask As String = cboSourceFileType.Text.ToLower
                            sMask = Replace(sMask, ",", " ")
                            sMask = Replace(sMask, ";", " ")
                            Dim sMaskList() As String = Split(sMask, " ")
                            If bZip Then
                                Dim oZip As ZipFile = Nothing
                                Try
                                    oZip = ZipFile.Read(sPath)
                                    Using oZip
                                        For Each sFile As String In oZip.EntryFileNames
                                            If msSourceSubFolders(0) = String.Empty Then
                                                'Root folder only
                                                If (InStr(sFile, "/") = 0) Then
                                                    'Check the mask
                                                    For Each sTmp As String In sMaskList
                                                        If sTmp.Contains("*") Or sTmp.Contains(".") Then
                                                            If sFile Like sTmp Then
                                                                chkFiles.Items.Add(sFile)
                                                            End If
                                                        End If
                                                    Next
                                                End If
                                            Else
                                                'See if the path is right
                                                If msSourceSubFolders(0).Length < sFile.Length AndAlso _
                                                    sFile.Substring(0, msSourceSubFolders(0).Length) = msSourceSubFolders(0) Then
                                                    'It's under the correct path, make sure it's not farther down another subdirectory
                                                    Dim sFileTmp As String = sFile.Substring(msSourceSubFolders(0).Length + 1)
                                                    If (InStr(sFileTmp, "/") = 0) Then
                                                        'Check the mask
                                                        For Each sTmp As String In sMaskList
                                                            If sTmp.Contains("*") Or sTmp.Contains(".") Then
                                                                If sFileTmp Like sTmp Then
                                                                    chkFiles.Items.Add(sFileTmp)
                                                                End If
                                                            End If
                                                        Next
                                                    End If
                                                End If
                                            End If
                                        Next
                                    End Using

                                Catch ex As Exception
                                Finally
                                    If oZip IsNot Nothing Then
                                        oZip.Dispose()
                                        oZip = Nothing
                                    End If
                                End Try
                            Else
                                For Each sTmp As String In sMaskList
                                    If sTmp.Contains("*") Or sTmp.Contains(".") Then
                                        'Get a directory listing
                                        sFiles = IO.Directory.GetFiles(msSourceFolder & msSourceSubFolders(0), sTmp)
                                        'It returns the full path, so filter that out and add to the list box
                                        Dim nStartFileName As Integer = msSourceFolder.Length + msSourceSubFolders(0).Length + 1

                                        'Subtract 1 from nStartFileName if msSourceSubFolders(0) is the root folder RJO 08/31/10
                                        If Strings.Right(msSourceSubFolders(0), 1) = "\" Then nStartFileName -= 1

                                        'Subtract 1 from nStartFileName if msSourceSubFolders(0) is the root folder RJO 08/31/10
                                        If msSourceSubFolders(0) = "" Then nStartFileName -= 1
                                        If sFiles IsNot Nothing Then
                                            For Each sFile As String In sFiles
                                                chkFiles.Items.Add(sFile.Substring(nStartFileName))
                                            Next
                                        End If
                                    End If
                                Next
                            End If
                        End If
                    ElseIf chkSourceRobots.CheckedItems.Count > 1 Then
                        'Identify all the source folders, but don't do anything with them yet.
                        msSourceFolder = sPath
                        ReDim msSourceSubFolders(mnSourceSubFolderCount - 1)
                        Dim sRobots As String = gpsRM.GetString("psSELECTED_ROBOTS")
                        For nIdx As Integer = 0 To mnSourceSubFolderCount - 1
                            msSourceSubFolders(nIdx) = chkSourceRobots.CheckedItems.Item(nIdx).ToString
                            If nIdx > 0 Then
                                sRobots = sRobots & ", "
                            End If
                            sRobots = sRobots & chkSourceRobots.CheckedItems.Item(nIdx).ToString
                        Next
                        Status = sRobots
                    End If
                End If
            End If
            If cboDestType.SelectedIndex >= 0 Then
                'Reselect the dest type to recheck the checklist
                cbo_SelectedIndexChanged(cboDestType)
            End If
        Catch ex As Exception

            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            msSourceFolder = String.Empty
            msSourceSubFolders = Nothing
            mnSourceSubFolderCount = 0
            chkFiles.Items.Clear()
        Finally
            Status(True) = gpsRM.GetString("psREADY")
        End Try
        Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub

    Private Sub tabMain_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabMain.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Tab Selected
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Select Case tabMain.SelectedTab.Name
            Case "tabSource"
                Status(False) = gpsRM.GetString("psSELECT_SOURCE")
            Case "tabFiles"
                If mbProcessSourceSelection Then
                    subProcessSourceSelection()
                End If
                Status(False) = gpsRM.GetString("psSELECT_FILES")
            Case "tabDest"
                If mbProcessSourceSelection Then
                    subProcessSourceSelection()
                End If
                Status(False) = gpsRM.GetString("psSELECT_DEST")
            Case "tabDoCopy"
                subDoSummary()
                Status(False) = gpsRM.GetString("psVERIFY_DO_COPY")
        End Select
    End Sub
    Private Sub btn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSourceNext.Click, _
                btnFilesPrev.Click, btnFilesNext.Click, btnDestPrev.Click, btnDestNext.Click, btnDoCopyPrev.Click, btnDoCopy.Click
        '********************************************************************************************
        'Description:  prev/next buttons handler
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oBtn As Button = DirectCast(sender, Button)
        'Block extras in tabMain_TabIndexChanged
        Select Case oBtn.Name
            Case "btnSourceNext"
                If rdoSourceAllFiles.Checked Then
                    'To destination tab
                    tabMain.SelectTab(tabDest)
                Else
                    'Select files
                    tabMain.SelectTab(tabFiles)
                End If
            Case "btnFilesPrev"
                'To source tab
                tabMain.SelectTab(tabSource)
            Case "btnFilesNext"
                If btnDeleteFiles.Checked Then
                    'To Copy tab
                    tabMain.SelectTab(tabDoCopy)
                Else
                    'To dest tab
                    tabMain.SelectTab(tabDest)
                End If
            Case "btnDestPrev"
                If rdoSourceAllFiles.Checked Then
                    'To source tab
                    tabMain.SelectTab(tabSource)
                Else
                    'Select files
                    tabMain.SelectTab(tabFiles)
                End If
            Case "btnDestNext"
                'To Copy tab
                tabMain.SelectTab(tabDoCopy)
            Case "btnDoCopyPrev"
                If btnDeleteFiles.Checked Then
                    'To Copy tab
                    tabMain.SelectTab(tabFiles)
                Else
                    'To dest tab
                    tabMain.SelectTab(tabDest)
                End If
            Case "btnDoCopy"
                'Do Copy
                subDoCopy()
        End Select
    End Sub
#End Region
#Region "Password - Status bar menu - "
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
        Dim cachePrivilege As ePrivilege
        Dim bRem As Boolean = False
        Dim bAllow As Boolean = False


        If LoggedOnUser <> String.Empty Then

            'If moPrivilege.Privilege = String.Empty Then 'RJO 03/23/12
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
                If colZones.PaintShopComputer = False Then
                    bAllow = True
                End If
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
            'subEnableControls(cboZone.Enabled) 'Enable if it's not all disabled right now
            subDoSummary()
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
            'subEnableControls(cboZone.Enabled) 'Enable if it's not all disabled right now
            subDoSummary()
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

    Private Sub mnuCheckList_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles mnuCheckList.Opening

    End Sub
End Class