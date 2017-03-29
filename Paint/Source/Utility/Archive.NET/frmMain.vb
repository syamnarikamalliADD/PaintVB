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
' Description: Archive Screen
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Rick Olejniczak
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    12/22/09   MSW     1st version
'    02/15/11   MSW     Add some error handling to zip code
'    09/14/11   MSW     Assemble a standard version of everything                       4.1.0.0
'    09/27/11   MSW     Standalone changes round 1 - HGB SA paintshop computer changes	4.1.0.1
'    10/14/11   MSW     Add some sections to make a full backup                         4.1.1.0
'    01/18/12   MSW     Clean up old printsettings object                               4.1.1.1
'    01/24/12   MSW     Placeholder include updates                                   4.01.01.02
'    02/15/12   MSW     Force 32 bit build for PCDK compatability                     4.01.01.03
'    03/23/12   RJO     modifed for .NET Password and IPC                             4.01.02.00
'    03/28/12   MSW     Added notepad folder                                          4.01.03.00
'    04/20/12   MSW     frmMain_KeyDown-Add excape key handler                        4.01.03.01
'    06/07/12   MSW     frmMain_KeyDown-Simplify help links                           4.01.03.02
'    06/15/12   MP      Added code to tabMain_Selected update the screen dump name    4.01.03.03
'                       based on the selected tab.
'    11/15/12   HGB     In bLoadListBoxes, hide various items that are not currently  4.01.03.04
'                       supported in SA and show all robots.
'    04/16/13   MSW     Add Canadian language files                                   4.01.05.00
'                       Standalone Changelog
'    07/09/13   MSW     Update and standardize logos                                  4.01.05.01
'    08/20/13   MSW     Manage robot selected count better                            4.01.05.03
'                       Progress, Status - Add error handler so it doesn't get hung up
'    08/30/13   MSW     Robot count still needed work for select all and unselect all 4.01.05.04
'                       Add the additional folders from the backup config, use text compare
'    09/30/13   MSW     Save screenshots as jpegs                                     4.01.06.00
'    01/07/14   MSW     Remove ControlBox from main form                              4.01.06.01
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call                4.01.07.00
'**************************************************************************************************


'******** Unfinished Business   *******************************************************************
'******** End Unfinished Business   ***************************************************************

Option Compare Binary
Option Explicit On
Option Strict On
Imports System.IO
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Configuration.ConfigurationSettings
Imports Response = System.Windows.Forms.DialogResult
'dll for zip access: C:\paint\Vbapps\Ionic.Zip.dll
'See License at C:\paint\Source\Common.NET\DotNetZip License.txt
'Website http://dotnetzip.codeplex.com/
Imports Ionic.Zip


Friend Class frmMain

#Region " Declares "
    '******** Form Constants   *********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Public Const msSCREEN_NAME As String = "Archive"   ' <-- For password area change log etc.
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    Friend Const mbUSEROBOTS As Boolean = False
    Friend Const mbUSEPLC As Boolean = False
    'Private Const mnEVENT_COUNT As Integer = 18 'RJO 03/23/12
    'Private Const mnROWSPACE As Integer = 40 ' interval for rows of textboxes etc

    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    Friend gsChangeLogArea As String = msSCREEN_NAME
    Private msCulture As String = "en-US" 'Default to English
    Private msOldZone As String = String.Empty
    Private mcolZones As clsZones = Nothing

    Private mbLocal As Boolean
    Private mbScreenLoaded As Boolean
    Private mbEventBlocker As Boolean
    Private mbRemoteZone As Boolean
    Private mbAddFoldersModified As Boolean
    Private mbPathDNE_Warn As Boolean
    Private msCurrentTabName As String = "tabDBSQL" ' always start on first page
    Private msSCREEN_DUMP_NAME As String = "Utilities_Archive" 'changes with selected tab
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private mbEditsMade As Boolean
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean
    Private mnProgress As Integer
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    '******** End Property Variables    *************************************************************


    Dim msDatabasePath As String = String.Empty
    Dim msXMLPath As String = String.Empty
    Dim msNotepadPath As String = String.Empty
    Dim msECBRPath As String = String.Empty
    Dim msTempBackupPath As String = String.Empty
    Dim msMasterBackupPath As String = String.Empty
    Dim msImageBackupPath As String = String.Empty
    Dim msPWBackupPath As String = String.Empty
    Dim msSourcePath As String = String.Empty
    Dim msProfilePath As String = String.Empty
    Dim msVbappsPath As String = String.Empty
    Dim msDmonViewerPath As String = String.Empty
    Dim msPAINTworksPath As String = String.Empty
    Dim msHelpPath As String = String.Empty
    Dim msFanucManualsPath As String = String.Empty
    Dim msDmonPath As String = String.Empty
    Dim msDmonArchivePath As String = String.Empty

    Private moAdditionalFolders As clsAdditionalFolders = Nothing
    'Constants for the treeview 
    Private Const msTEMP_BACKUPS As String = "TempBackups"
    Private Const msMASTER_BACKUPS As String = "MasterBackups"
    Private Const msIMAGE_BACKUPS As String = "ImageBackups"
    Private Const msPW_BACKUPS As String = "PWBackups"

    Dim moCurTab As TabPage = Nothing
    Dim moCurList As ListBox = Nothing
    Dim msCurList As String = String.Empty
    Dim msDefaultFolder As String = String.Empty
    Dim mnSelectedRobotFolderCount As Integer = 0
    Dim mnSelectedRobotSelectionCount As Integer = 0
    Dim msMask As String = String.Empty
    '******** This is the old pw3 password object interop  ******************************************
    'Friend WithEvents moPassword As PWPassword.PasswordObject
    'Friend moPrivilege As PWPassword.cPrivilegeObject

    Friend WithEvents moPassword As clsPWUser 'RJO 03/23/12
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm  'RJO 03/23/12

    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet)  'RJO 03/23/12

    'Delegate Sub ControllerStatusChange(ByVal Controller As clsController)
    '******** This is the old pw3 password object interop  ******************************************
    Private Enum eBackupSourceType
        None
        DBSQL
        DBAccess
        DBXML
        Notepad
        Image
        Robot
        Application
        DMON
        DMONArchive
    End Enum
    Private Enum eBackupOutputType
        None
        Folder
        Zip
        CSV
        CSVZIP
    End Enum

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
            tabMain_Selected()
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
        btnClose.Enabled = True

        If bEnable = False Then
            btnChangeLog.Enabled = False
            bRestOfControls = False
            PnlMain.Enabled = False
        Else
            Select Case Privilege
                Case ePrivilege.None
                    btnChangeLog.Enabled = (True And DataLoaded)
                    bRestOfControls = False
                    PnlMain.Enabled = True

                Case ePrivilege.Edit, ePrivilege.Copy '02/02/10 RJO
                    btnChangeLog.Enabled = (True And DataLoaded)
                    bRestOfControls = (True And DataLoaded)
                    PnlMain.Enabled = (True And DataLoaded)

            End Select
        End If
        btnBackupToFolderDBSql.Enabled = bRestOfControls
        btnBackupToZipDBSql.Enabled = bRestOfControls
        btnExportToCSVDBSql.Enabled = bRestOfControls
        btnExportToCSVZIPDBSql.Enabled = bRestOfControls
        btnBackupToFolderDBAccess.Enabled = bRestOfControls
        btnBackupToZipDBAccess.Enabled = bRestOfControls
        btnBackupToFolderDBXML.Enabled = bRestOfControls
        btnBackupToZipDBXML.Enabled = bRestOfControls
        btnBackupToFolderNotepad.Enabled = bRestOfControls
        btnBackupToZipNotepad.Enabled = bRestOfControls
        btnBackupToFolderImage.Enabled = bRestOfControls
        btnBackupToZipImage.Enabled = bRestOfControls
        btnBackupToFolderDMON.Enabled = bRestOfControls
        btnBackupToZipDMON.Enabled = bRestOfControls
        btnBackupToFolderDMONArchive.Enabled = bRestOfControls
        btnBackupToZipDMONArchive.Enabled = bRestOfControls
        btnUtilities.Enabled = bRestOfControls
        cboSourceFileType.Enabled = bRestOfControls
        btnBackupToFolderApplication.Enabled = bRestOfControls
        btnBackupToZipsApplication.Enabled = bRestOfControls
        bRestOfControls = (mnSelectedRobotSelectionCount > 0) And (mnSelectedRobotFolderCount > 0)
        btnBackupToFolderRobots.Enabled = bRestOfControls
        btnBackupToZipRobots.Enabled = bRestOfControls
        bRestOfControls = (mnSelectedRobotSelectionCount = 1) And (mnSelectedRobotFolderCount = 1)
        rdoSourceAllFiles.Enabled = bRestOfControls
        rdoSourceSelectFiles.Enabled = bRestOfControls
        lstRobotFiles.Enabled = bRestOfControls

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
            lblZone.Text = gcsRM.GetString("csZONE_CAP")
            mnuSelectAll.Text = gcsRM.GetString("csSELECT_ALL")
            mnuUnselectAll.Text = gcsRM.GetString("csUNSELECT_ALL")

            With gpsRM
                ' set labels for tab #1 [Shift 1]
                tabDBSQL.Text = .GetString("psTABDBSQL", oCulture)
                tabDBAccess.Text = .GetString("psTABDBACCESS", oCulture)
                tabDBXML.Text = .GetString("psTABDBXML", oCulture)
                tabNotepad.Text = .GetString("psTABNOTEPAD", oCulture)
                tabImage.Text = .GetString("psTABIMAGE", oCulture)
                tabRobots.Text = .GetString("psTABROBOTS", oCulture)
                tabApplication.Text = .GetString("psTABAPPLICATION", oCulture)
                tabDMON.Text = .GetString("psTABDMON", oCulture)
                tabDMONArchive.Text = .GetString("psTABDMONARCHIVE", oCulture)
                lblDBSQL.Text = .GetString("psTABDBSQL", oCulture)
                lblDBAccess.Text = .GetString("psTABDBACCESS", oCulture)
                lblDBXML.Text = .GetString("psTABDBXML", oCulture)
                lblImage.Text = .GetString("psTABImage", oCulture)
                lblApplication.Text = .GetString("psTABAPPLICATION", oCulture)
                lblDMON.Text = .GetString("psTABDMON", oCulture)
                lblDMONArchive.Text = .GetString("psTABDMONARCHIVE", oCulture)
                btnBackupToFolderDBSql.Text = .GetString("psBACKUP_TO_FOLDER_BTN", oCulture)
                btnBackupToZipDBSql.Text = .GetString("psBACKUP_TO_ZIP_BTN", oCulture)
                btnExportToCSVDBSql.Text = .GetString("psEXPORT_TO_CSV_BTN", oCulture)
                btnExportToCSVZIPDBSql.Text = .GetString("psEXPORT_TO_CSV_ZIP_BTN", oCulture)
                btnBackupToFolderDBAccess.Text = .GetString("psBACKUP_TO_FOLDER_BTN", oCulture)
                btnBackupToZipDBAccess.Text = .GetString("psBACKUP_TO_ZIP_BTN", oCulture)
                btnBackupToFolderDBXML.Text = .GetString("psBACKUP_TO_FOLDER_BTN", oCulture)
                btnBackupToZipDBXML.Text = .GetString("psBACKUP_TO_ZIP_BTN", oCulture)
                btnBackupToFolderNotepad.Text = .GetString("psBACKUP_TO_FOLDER_BTN", oCulture)
                btnBackupToZipNotepad.Text = .GetString("psBACKUP_TO_ZIP_BTN", oCulture)
                btnBackupToFolderImage.Text = .GetString("psBACKUP_TO_FOLDER_BTN", oCulture)
                btnBackupToZipImage.Text = .GetString("psBACKUP_TO_ZIP_BTN", oCulture)
                mnuBackupAllDBToFolder.Text = .GetString("psBACKUP_ALL_DB_TO_FOLDER_MNU", oCulture)
                mnuBackupAllDBToZip.Text = .GetString("psBACKUP_ALL_DB_TO_ZIP_MNU", oCulture)
                lblRobots.Text = .GetString("psROBOTS_CAP", oCulture)
                lblBackupFolders.Text = .GetString("psBACKUP_FOLDERS_LBL", oCulture)
                btnBackupToFolderRobots.Text = .GetString("psBACKUP_TO_FOLDER_BTN", oCulture)
                btnBackupToZipRobots.Text = .GetString("psBACKUP_TO_ZIP_BTN", oCulture)
                rdoSourceAllFiles.Text = .GetString("psALL_FILES")
                rdoSourceSelectFiles.Text = .GetString("psSELECT_FILES")

                btnBackupToFolderDMON.Text = .GetString("psBACKUP_TO_FOLDER_BTN", oCulture)
                btnBackupToZipDMON.Text = .GetString("psBACKUP_TO_ZIP_BTN", oCulture)
                btnBackupToFolderDMONArchive.Text = .GetString("psBACKUP_TO_FOLDER_BTN", oCulture)
                btnBackupToZipDMONArchive.Text = .GetString("psBACKUP_TO_ZIP_BTN", oCulture)


                btnBackupToFolderApplication.Text = .GetString("psBACKUP_TO_FOLDER_BTN", oCulture)
                btnBackupToZipsApplication.Text = .GetString("psBACKUP_TO_ZIP_BTN", oCulture)
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
            lblZone.Visible = Not mbLocal
            cboZone.Visible = Not mbLocal

            mPrintHtml = New clsPrintHtml(frmMain.msSCREEN_NAME & "_" & msSCREEN_NAME, False, 0)

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


            Progress = 70

            'subEnableControls(True)

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
    Private Sub subLoadListBox(ByRef sList() As String, ByRef oBox As ListBox)
        '********************************************************************************************
        'Description:  Load a string array into a listbox
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        For Each sItem As String In sList
            Dim sTmp() As String = Split(sItem, "\")
            oBox.Items.Add(sTmp(sTmp.GetUpperBound(0)))
        Next
    End Sub
    Private Sub subHideTab(ByRef oTab As TabPage)
        '********************************************************************************************
        'Description:  hide a tab
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bDone As Boolean = False
        For Each oTab1 As TabPage In tabMain.TabPages
            If oTab Is oTab1 Then
                tabMain.TabPages.Remove(oTab)
                Exit Sub
            End If
        Next
    End Sub
    Private Sub subShowTab(ByRef oTab As TabPage)
        '********************************************************************************************
        'Description:  show a tab
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bDone As Boolean = False
        For Each oTab1 As TabPage In tabMain.TabPages
            If oTab Is oTab1 Then
                Exit Sub
            End If
        Next
    End Sub
    Private Function bLoadListBoxes() As Boolean
        '********************************************************************************************
        'Description:  Load file list boxes
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/15/12  HGB     Hide various items that are not currently supported in SA and show all 
        '                   robots.
        ' 08/30/13  MSW     Add the additional folders from the backup config, use text compare
        '********************************************************************************************
        Dim bReturn As Boolean = True

        Dim oCulture As CultureInfo = DisplayCulture
        Try
            'all SQL Server Databases
            Call mPWCommon.GetDefaultFilePath(msDatabasePath, eDir.Database, String.Empty, String.Empty)
            Dim sSqlDbFiles() As String = IO.Directory.GetFiles(msDatabasePath, "*.mdf")
            If sSqlDbFiles Is Nothing OrElse sSqlDbFiles.GetUpperBound(0) < 0 Then
                subHideTab(tabDBSQL)
            Else
                subLoadListBox(sSqlDbFiles, lstDBSQL)
                subShowTab(tabDBSQL)
            End If
        Catch ex As Exception
            bReturn = False
            subHideTab(tabDBSQL)
        End Try

        Try
            'MS Access Databases
            Dim sMSAccessDbFiles() As String = IO.Directory.GetFiles(msDatabasePath, "*.mdb")
            If sMSAccessDbFiles Is Nothing OrElse sMSAccessDbFiles.GetUpperBound(0) < 0 Then
                subHideTab(tabDBAccess)
            Else
                subLoadListBox(sMSAccessDbFiles, lstDBAccess)
                subShowTab(tabDBAccess)
            End If
        Catch ex As Exception
            bReturn = False
            subHideTab(tabDBAccess)
        End Try

        Try
            'XML Data
            Call mPWCommon.GetDefaultFilePath(msXMLPath, eDir.XML, String.Empty, String.Empty)
            Dim sXMLDbFiles() As String = IO.Directory.GetFiles(msXMLPath, "*.*")
            If sXMLDbFiles Is Nothing OrElse sXMLDbFiles.GetUpperBound(0) < 0 Then
                subHideTab(tabDBXML)
            Else
                subLoadListBox(sXMLDbFiles, lstDBXML)
                subShowTab(tabDBXML)
            End If
        Catch ex As Exception
            bReturn = False
            subHideTab(tabDBXML)
        End Try

        Try
            'Notepad
            Call mPWCommon.GetDefaultFilePath(msNotepadPath, eDir.Notepad, String.Empty, String.Empty)
            Dim sNotepadFiles() As String = IO.Directory.GetFiles(msNotepadPath, "*.*")
            If sNotepadFiles Is Nothing OrElse sNotepadFiles.GetUpperBound(0) < 0 Then
                subHideTab(tabNotepad)
            Else
                subLoadListBox(sNotepadFiles, lstNotepad)
                subShowTab(tabNotepad)
            End If
        Catch ex As Exception
            bReturn = False
            subHideTab(tabNotepad)
        End Try

        Try
            'Image Backup config
            Call mPWCommon.GetDefaultFilePath(msECBRPath, eDir.ECBR, String.Empty, String.Empty)
            Dim sImageCfgFiles() As String = IO.Directory.GetFiles(msECBRPath, "*.*")
            If sImageCfgFiles Is Nothing OrElse sImageCfgFiles.GetUpperBound(0) < 0 Then
                subHideTab(tabImage)
            Else
                subLoadListBox(sImageCfgFiles, lstImage)
                subShowTab(tabImage)
            End If
        Catch ex As Exception
            bReturn = False
            subHideTab(tabImage)
        End Try

        'HGB for SA (no image backup support)  
        If mcolZones.StandAlone Then
            subHideTab(tabImage)
        End If

        'Robot Tab
        Try
            'HGB for SA show robots from all zones
            If mcolZones.StandAlone Then
                LoadRobotBoxFromDB(chkRobots, mcolZones, True)
            Else
                LoadRobotBoxFromDB(chkRobots, mcolZones.ActiveZone, True)
            End If
        Catch ex As Exception
            bReturn = False
            subHideTab(tabRobots)
        End Try
        Try
            'Temp Backup config
            Call mPWCommon.GetDefaultFilePath(msTempBackupPath, eDir.TempBackups, String.Empty, String.Empty)
            Dim sRobotFolders() As String = IO.Directory.GetDirectories(msTempBackupPath)
            If sRobotFolders IsNot Nothing AndAlso sRobotFolders.GetUpperBound(0) > -1 Then
                'Add to Treeview
                Dim rcNode As New Windows.Forms.TreeNode(msTEMP_BACKUPS)
                rcNode.Name = msTEMP_BACKUPS
                rcNode.Text = gpsRM.GetString("psTEMP_BACKUPS", oCulture)
                rcNode.Tag = msTempBackupPath
                trvFolders.Nodes.Add(rcNode)
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuBackupAll_Click", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        End Try
        Try
            'Master Backup config
            Call mPWCommon.GetDefaultFilePath(msMasterBackupPath, eDir.MasterBackups, String.Empty, String.Empty)
            Dim sRobotFolders() As String = IO.Directory.GetDirectories(msMasterBackupPath)
            If sRobotFolders IsNot Nothing AndAlso sRobotFolders.GetUpperBound(0) > -1 Then
                'Add to Treeview
                Dim rcNode As New Windows.Forms.TreeNode(msMASTER_BACKUPS)
                rcNode.Name = msMASTER_BACKUPS
                rcNode.Text = gpsRM.GetString("psMASTER_BACKUPS", oCulture)
                rcNode.Tag = msMasterBackupPath
                trvFolders.Nodes.Add(rcNode)
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuBackupAll_Click", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        End Try
        Try
            'Image Backup config
            Call mPWCommon.GetDefaultFilePath(msImageBackupPath, eDir.RobotImageBackups, String.Empty, String.Empty)
            Dim sRobotFolders() As String = IO.Directory.GetDirectories(msImageBackupPath)
            If sRobotFolders IsNot Nothing AndAlso sRobotFolders.GetUpperBound(0) > -1 Then
                'Add to Treeview
                Dim rcNode As New Windows.Forms.TreeNode(msIMAGE_BACKUPS)
                rcNode.Name = msIMAGE_BACKUPS
                rcNode.Text = gpsRM.GetString("psIMAGE_BACKUPS", oCulture)
                rcNode.Tag = msImageBackupPath
                trvFolders.Nodes.Add(rcNode)
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuBackupAll_Click", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        End Try
        Try
            'PW Backup config
            Call mPWCommon.GetDefaultFilePath(msPWBackupPath, eDir.PWRobotBackups, String.Empty, String.Empty)
            Dim sBackupFolders() As String = IO.Directory.GetDirectories(msPWBackupPath)
            'HGB Would not display folders if there was only one folder (Upper Bound = 0) Changed to look at the Length
            '    since this is a one dimensional array.
            If sBackupFolders IsNot Nothing AndAlso sBackupFolders.Length > 0 Then ' GetUpperBound(0) > 0 Then
                'Add to Treeview
                Dim pwNode As New Windows.Forms.TreeNode(msPW_BACKUPS)
                pwNode.Name = msPW_BACKUPS
                pwNode.Text = gpsRM.GetString("psPW_BACKUPS", oCulture)
                pwNode.Tag = String.Empty
                For Each sTmp As String In sBackupFolders
                    Dim sRobotFolders() As String = IO.Directory.GetDirectories(sTmp)
                    'HGB Would not display folders if there was only one folder (Upper Bound = 0) Changed to look at the Length
                    '    since this is a one dimensional array.
                    If sRobotFolders IsNot Nothing AndAlso sRobotFolders.Length > 0 Then 'GetUpperBound(0) > 0 Then
                        'Add to Treeview
                        Dim sTmpSplit() As String = Split(sTmp, "\")
                        Dim sTmp2 As String = sTmpSplit(sTmpSplit.GetUpperBound(0))
                        Dim rcNode As New Windows.Forms.TreeNode(sTmp2)
                        rcNode.Name = sTmp2
                        rcNode.Text = sTmp2
                        rcNode.Tag = sTmp & "\"
                        pwNode.Nodes.Add(rcNode)
                    End If
                Next
                trvFolders.Nodes.Add(pwNode)
            End If
            mnSelectedRobotFolderCount = 0
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuBackupAll_Click", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        End Try

        Try
            'DMON Data
            Call mPWCommon.GetDefaultFilePath(msDmonPath, eDir.DmonData, String.Empty, String.Empty)
            Dim sDMONDataFiles() As String = IO.Directory.GetFiles(msDmonPath, "*.*")
            If sDMONDataFiles Is Nothing OrElse sDMONDataFiles.GetUpperBound(0) < -1 Then
                subHideTab(tabDMON)
            Else
                subLoadListBox(sDMONDataFiles, lstDMON)
                subShowTab(tabDMON)
            End If
        Catch ex As Exception
            bReturn = False
            subHideTab(tabDMON)
        End Try

        'HGB SA - No DMON in SA yet
        If mcolZones.StandAlone Then
            subHideTab(tabDMON)
        End If

        Try
            'DMON Archive
            Call mPWCommon.GetDefaultFilePath(msDmonArchivePath, eDir.DmonArchive, String.Empty, String.Empty)
            Dim sDMONDataArchiveFiles() As String = IO.Directory.GetFiles(msDmonArchivePath, "*.*")
            If sDMONDataArchiveFiles Is Nothing OrElse sDMONDataArchiveFiles.GetUpperBound(0) < -1 Then
                subHideTab(tabDMONArchive)
            Else
                subLoadListBox(sDMONDataArchiveFiles, lstDMONArchive)
                subShowTab(tabDMONArchive)
            End If
        Catch ex As Exception
            bReturn = False
            subHideTab(tabDMONArchive)
        End Try

        'HGB SA - No DMON in SA yet
        If mcolZones.StandAlone Then
            subHideTab(tabDMONArchive)
        End If

        'Application Tab
        Dim bHide As Boolean = True
        Dim eTag(4) As eDir
        Try
            Dim nTag As Integer = 0
            If mPWCommon.GetDefaultFilePath(msProfilePath, eDir.Profile, String.Empty, String.Empty) Or _
               mPWCommon.GetDefaultFilePath(msVbappsPath, eDir.VBApps, String.Empty, String.Empty) Or _
               mPWCommon.GetDefaultFilePath(msDmonViewerPath, eDir.DmonViewer, String.Empty, String.Empty) Or _
               mPWCommon.GetDefaultFilePath(msPAINTworksPath, eDir.PAINTworks, String.Empty, String.Empty) Then
                'Add paint common folders
                lstApplication.Items.Add(gpsRM.GetString("psPAINT_COMMON", oCulture))
                bHide = False
                eTag(nTag) = eDir.PAINTworks
                nTag = nTag + 1
                'Add paint common folders
            End If
            'HGB SA - added standalone (no source with SA)
            If mPWCommon.GetDefaultFilePath(msSourcePath, eDir.Source, String.Empty, String.Empty) And Not mcolZones.StandAlone Then
                'Add source
                lstApplication.Items.Add(gpsRM.GetString("psSOURCE", oCulture))
                bHide = False
                eTag(nTag) = eDir.Source
                nTag = nTag + 1
            End If
            If mPWCommon.GetDefaultFilePath(msHelpPath, eDir.Help, String.Empty, String.Empty) Then
                'Add help
                lstApplication.Items.Add(gpsRM.GetString("psHELP", oCulture))
                bHide = False
                eTag(nTag) = eDir.Help
                nTag = nTag + 1
                If mPWCommon.GetDefaultFilePath(msFanucManualsPath, eDir.FanucManuals, String.Empty, String.Empty) Then
                    'Add FANUC Manuals
                    lstApplication.Items.Add(gpsRM.GetString("psFANUCMANUALS", oCulture))
                    eTag(nTag) = eDir.FanucManuals
                    nTag = nTag + 1
                End If
            End If
            moAdditionalFolders = New clsAdditionalFolders
            ReDim Preserve eTag(moAdditionalFolders.NumFolders + nTag)
            For nItem As Integer = 0 To moAdditionalFolders.NumFolders - 1
                If IO.Directory.Exists(moAdditionalFolders.Folders(nItem).FolderPath) Then
                    lstApplication.Items.Add(moAdditionalFolders.Folders(nItem).FolderName)
                    eTag(nTag) = eDir.Other
                    nTag = nTag + 1
                End If
            Next
        Catch ex As Exception
            bReturn = False
        Finally
            If bHide Then
                subHideTab(tabApplication)
            Else
                subShowTab(tabApplication)
                lstApplication.Tag = eTag
            End If
        End Try

        Return (bReturn)
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
        mbAddFoldersModified = False

        Try
            Dim sTmp As String = gcsRM.GetString("csLOADINGDATA", DisplayCulture)

            Status = sTmp
            Progress = 50

            DataLoaded = bLoadListBoxes()

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
        Dim bTmp As Boolean = False


        subEnableControls(True)
        mbEventBlocker = False

    End Sub

    Private Function GetCheckBox(ByRef Container As TabPage, ByVal Index As Integer) As CheckBox
        '********************************************************************************************
        'Description:  Returns the CheckBox control that matches the Index
        '
        'Parameters: Container (Current TabPage), Index (Number in control's Tag field)
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sIndex As String = Index.ToString

        For Each oControl As Control In Container.Controls
            If TypeOf oControl Is CheckBox Then
                If oControl.Tag.ToString = sIndex Then
                    Dim oCheckBox As CheckBox = DirectCast(oControl, CheckBox)

                    Return oCheckBox
                End If
            End If
        Next

        Return Nothing

    End Function

    Private Function GetDTPicker(ByRef Container As TabPage, ByVal Type As String, ByVal Index As Integer) As DateTimePicker
        '********************************************************************************************
        'Description:  Returns the DateTimePicker control that matches the Type and Index
        '
        'Parameters: Container (Current TabPage), Type ("Start" or "End"), 
        '            Index (Number in control's Tag field)
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sIndex As String = Index.ToString

        For Each oControl As Control In Container.Controls
            If TypeOf oControl Is DateTimePicker Then
                If oControl.Tag.ToString = sIndex Then
                    If Strings.Right(oControl.Name, Strings.Len(Type)).ToLower = Type.ToLower Then
                        Dim oDTPicker As DateTimePicker = DirectCast(oControl, DateTimePicker)

                        Return oDTPicker
                    End If
                End If
            End If
        Next

        Return Nothing

    End Function

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
            subLoadData()
        End If

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

    Private Overloads Sub subUpdateChangeLog(ByVal ZoneNumber As Integer)
        '********************************************************************************************
        'Description:  build up the change text for Add folders changed event
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sAddFolderChange As String = gpsRM.GetString("psADD_FOLDER_CHG_MSG", DisplayCulture)

        Call mPWCommon.AddChangeRecordToCollection(gsChangeLogArea, LoggedOnUser, ZoneNumber, _
                                                " ", " ", sAddFolderChange, mcolZones.ActiveZone.Name)

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
        Dim oThisProcess As Process = Process.GetCurrentProcess
        oThisProcess.Kill()
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
        '    04/20/12   MSW     frmMain_KeyDown-Add excape key handler                        4.01.03.01
        '    06/07/12   MSW     frmMain_KeyDown-Simplify help links                           4.01.03.02
		'    09/30/13   MSW     Save screenshots as jpegs
        '********************************************************************************************
        Dim sKeyValue As String = String.Empty

        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode
                Case Keys.F1
                    'Help Screen request
                    sKeyValue = "btnHelp"
                    'Select Case msCurrentTabName
                    '    Case tabDBSQL.Name
                    subLaunchHelp(gs_HELP_UTILITIES_ARCHIVE, oIPC)
                    '    Case tabDBAccess.Name
                    '        subLaunchHelp(gs_HELP_UTILITIES_ARCHIVE_ACCESS)
                    '    Case tabDBXML.Name
                    '        subLaunchHelp(gs_HELP_UTILITIES_ARCHIVE_XML)
                    '    Case tabNotepad.Name
                    '        subLaunchHelp(gs_HELP_UTILITIES_ARCHIVE_NOTEPAD)
                    '    Case tabImage.Name
                    '        subLaunchHelp(gs_HELP_UTILITIES_ARCHIVE_IMAGE)
                    '    Case tabRobots.Name
                    '        subLaunchHelp(gs_HELP_UTILITIES_ARCHIVE_ROBOTS)
                    '    Case tabApplication.Name
                    '        subLaunchHelp(gs_HELP_UTILITIES_ARCHIVE_APP)
                    '    Case tabDMON.Name
                    '        subLaunchHelp(gs_HELP_UTILITIES_ARCHIVE_DMON)
                    '    Case tabDMONArchive.Name
                    '        subLaunchHelp(gs_HELP_UTILITIES_ARCHIVE_DMON_ARCHIVE)
                    'End Select
                 
                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty

                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    Dim sTabName As String = String.Empty
                    Select Case msCurrentTabName
                        Case tabDBSQL.Name
                            sTabName = "_SQL"
                        Case tabDBAccess.Name
                            sTabName = "_Access"
                        Case tabDBXML.Name
                            sTabName = "_XML"
                        Case tabNotepad.Name
                            sTabName = "_Notepad"
                        Case tabImage.Name
                            sTabName = "_Image"
                        Case tabRobots.Name
                            sTabName = "_Robots"
                        Case tabApplication.Name
                            sTabName = "_Application"
                        Case tabDMON.Name
                            sTabName = "_DMON"
                        Case tabDMONArchive.Name
                            sTabName = "_DMONArchive"
                    End Select

                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath & msSCREEN_DUMP_NAME & sTabName & ".jpg", Imaging.ImageFormat.Jpeg)

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

    Private Sub tabMain_Selected(Optional ByVal sender As Object = Nothing, _
                Optional ByVal e As System.Windows.Forms.TabControlEventArgs = Nothing) Handles tabMain.Selected
        '********************************************************************************************
        'Description:  a tab page was just selected
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/15/12  MP      Added code to update the screen dump name based on the selected tab.
		'    09/30/13   MSW     Save screenshots as jpegs
        '********************************************************************************************
        moCurTab = tabMain.SelectedTab
        msCurrentTabName = moCurTab.Name
        msSCREEN_DUMP_NAME = "Utilities_Archive_" & tabMain.SelectedTab.Name & ".jpg"
        Select Case msCurrentTabName
            Case tabDBSQL.Name
                moCurList = lstDBSQL
                msCurList = moCurList.Name
            Case tabDBAccess.Name
                moCurList = lstDBAccess
                msCurList = moCurList.Name
            Case tabDBXML.Name
                moCurList = lstDBXML
                msCurList = moCurList.Name
            Case tabDBXML.Name
                moCurList = lstNotepad
                msCurList = moCurList.Name
            Case tabImage.Name
                moCurList = lstImage
                msCurList = moCurList.Name
            Case tabRobots.Name
                moCurList = Nothing
                msCurList = String.Empty
            Case tabApplication.Name
                moCurList = lstApplication
                msCurList = moCurList.Name
            Case tabDMON.Name
                moCurList = lstDMON
                msCurList = moCurList.Name
            Case tabDMONArchive.Name
                moCurList = lstDMONArchive
                msCurList = moCurList.Name
        End Select
        If moCurList IsNot Nothing Then
            moCurList.Focus()
        End If
        Call subShowNewPage(True)

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
        ' 08/30/13  MSW     Robot count still needed work for select all and unselect all 4.01.05.04
        '********************************************************************************************
        Try

            Select Case msCurrentTabName
                Case tabDBSQL.Name, tabDBAccess.Name, tabDBXML.Name, tabNotepad.Name, tabImage.Name, _
                                tabApplication.Name, tabDMON.Name, tabDMONArchive.Name
                    If moCurList IsNot Nothing Then
                        For nIndex As Integer = 0 To moCurList.Items.Count - 1
                            Dim bDone As Boolean = False
                            For Each oItem As Object In moCurList.SelectedItems
                                If moCurList.Items(nIndex) Is oItem Then
                                    bDone = True
                                    Exit For
                                End If
                            Next
                            If bDone = False Then
                                moCurList.SelectedItems.Add(moCurList.Items(nIndex))
                            End If
                        Next
                    End If
                Case tabRobots.Name
                    Select Case msCurList
                        Case chkRobots.Name
                            mbEventBlocker = True
                            For nIndex As Integer = 0 To chkRobots.Items.Count - 1
                                chkRobots.SetItemCheckState(nIndex, CheckState.Checked)
                            Next
                            mnSelectedRobotSelectionCount = chkRobots.Items.Count
                        Case trvFolders.Name
                            mbEventBlocker = True
                            mnSelectedRobotFolderCount = 0
                            For Each oNode As Windows.Forms.TreeNode In trvFolders.Nodes
                                oNode.Checked = True
                                If oNode.Nodes.Count > 0 Then
                                    'no need to get recursive here, there's only one more layer.
                                    For Each oNode2 As Windows.Forms.TreeNode In oNode.Nodes
                                        oNode2.Checked = True
                                        mnSelectedRobotFolderCount = mnSelectedRobotFolderCount + 1
                                    Next
                                Else
                                    mnSelectedRobotFolderCount = mnSelectedRobotFolderCount + 1
                                End If
                            Next
                        Case lstRobotFiles.Name
                            For nIndex As Integer = 0 To lstRobotFiles.Items.Count - 1
                                Dim bDone As Boolean = False
                                For Each oItem As Object In lstRobotFiles.SelectedItems
                                    If lstRobotFiles.Items(nIndex) Is oItem Then
                                        bDone = True
                                        Exit For
                                    End If
                                Next
                                If bDone = False Then
                                    lstRobotFiles.SelectedItems.Add(lstRobotFiles.Items(nIndex))

                                End If
                            Next
                    End Select
            End Select
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuSelectAll_Click", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        Finally
            mbEventBlocker = False
            subEnableControls(True)
        End Try
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
        ' 08/30/13  MSW     Robot count still needed work for select all and unselect all 4.01.05.04
        '********************************************************************************************
        Try

            Select Case msCurrentTabName
                Case tabDBSQL.Name, tabDBAccess.Name, tabDBXML.Name, tabNotepad.Name, tabImage.Name, tabApplication.Name
                    If moCurList IsNot Nothing Then
                        Do While (moCurList.SelectedItems.Count > 0)
                            moCurList.SelectedItems.Remove(moCurList.SelectedItems(0))
                        Loop
                    End If
                Case tabRobots.Name
                    Select Case msCurList
                        Case chkRobots.Name
                            mbEventBlocker = True
                            For nIndex As Integer = 0 To chkRobots.Items.Count - 1
                                chkRobots.SetItemCheckState(nIndex, CheckState.Unchecked)
                            Next
                            mnSelectedRobotSelectionCount = 0
                        Case trvFolders.Name
                            mbEventBlocker = True
                            mnSelectedRobotFolderCount = 0
                            For Each oNode As Windows.Forms.TreeNode In trvFolders.Nodes
                                oNode.Checked = False
                                If oNode.Nodes.Count > 0 Then
                                    'no need to get recursive here, there's only one more layer.
                                    For Each oNode2 As Windows.Forms.TreeNode In oNode.Nodes
                                        oNode2.Checked = False
                                    Next
                                End If
                            Next
                        Case lstRobotFiles.Name
                            Do While (lstRobotFiles.SelectedItems.Count > 0)
                                lstRobotFiles.SelectedItems.Remove(lstRobotFiles.SelectedItems(0))
                            Loop
                    End Select
            End Select
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuUnselectAll_Click", _
   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        Finally
            mbEventBlocker = False
            subEnableControls(True)
        End Try
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

#Region " Debug Stuff "

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'For debug
    End Sub

    Private Sub Button2_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        'For debug
    End Sub

#End Region
    Private Function subGetDataBase(ByRef sDBName As String, ByRef sTableName As String) As DataSet
        '********************************************************************************************
        'Description: Load a database
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim DS As New DataSet

        Try

            Dim DB As New clsSQLAccess

            With DB
                .Zone = mcolZones.ActiveZone
                .DBFileName = sDBName
                .DBTableName = sTableName
                .SQLString = "SELECT * FROM " & .DBTableName
                DS = .GetDataSet
                .Close()
            End With
            Return DS
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Dim x As New CheckedListBox
            Return (Nothing)
        End Try
    End Function
    Private Function subGetTableNames(ByRef sDBName As String) As DataSet
        '********************************************************************************************
        'Description: Load a database
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim DS As New DataSet

        Try

            Dim DB As New clsSQLAccess

            With DB
                .Zone = mcolZones.ActiveZone
                .DBFileName = sDBName
                .DBTableName = "*"
                .SQLString = "SELECT name FROM sys.tables"
                DS = .GetDataSet()
                .Close()
            End With
            Return DS
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Dim x As New CheckedListBox
            Return (Nothing)
        End Try
    End Function
    Private Sub subDoBackup(ByRef oBackupOutputType As eBackupOutputType, ByRef oBackupSourceType As eBackupSourceType, _
                            ByRef sBackupPath As String, ByRef sSubFolder As String, ByRef bAll As Boolean, _
                              Optional ByRef bYesToAll As Boolean = False, Optional ByRef bCancel As Boolean = False, _
                              Optional ByRef sRobotSource As String = "")
        '********************************************************************************************
        'Description:  Run a DB backup
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/15/11  MSW     Add some error handling to zip code
        ' 08/30/13  MSW     Add the additional folders from the backup config, use text compare
        '********************************************************************************************
        Dim sSourceFolder As String = String.Empty
        Dim oListBox As ListBox = Nothing
        Dim sList() As String = Nothing
        Try
            Select Case oBackupSourceType
                Case eBackupSourceType.DBSQL
                    oListBox = lstDBSQL
                    sSourceFolder = msDatabasePath
                    sSubFolder = Replace(sSourceFolder, msPAINTworksPath, "", , , CompareMethod.Text)
                Case eBackupSourceType.DBAccess
                    oListBox = lstDBAccess
                    sSourceFolder = msDatabasePath
                    sSubFolder = Replace(sSourceFolder, msPAINTworksPath, "", , , CompareMethod.Text)
                Case eBackupSourceType.DBXML
                    oListBox = lstDBXML
                    sSourceFolder = msXMLPath
                    sSubFolder = Replace(sSourceFolder, msPAINTworksPath, "", , , CompareMethod.Text)
                Case eBackupSourceType.Notepad
                    oListBox = lstNotepad
                    sSourceFolder = msNotepadPath
                    sSubFolder = Replace(sSourceFolder, msPAINTworksPath, "", , , CompareMethod.Text)
                Case eBackupSourceType.Image
                    oListBox = lstImage
                    sSourceFolder = msECBRPath
                    sSubFolder = Replace(sSourceFolder, msPAINTworksPath, "", , , CompareMethod.Text)
                Case eBackupSourceType.Robot
                    oListBox = lstRobotFiles
                    sSourceFolder = sRobotSource
                Case eBackupSourceType.DMON
                    oListBox = lstDMON
                    sSourceFolder = msDmonPath
                Case eBackupSourceType.DMONArchive
                    oListBox = lstDMONArchive
                    sSourceFolder = msDmonArchivePath
            End Select
            'Process the listbox or select all
            If bAll Then
                If oBackupSourceType = eBackupSourceType.Robot Then
                    Dim sMask As String = cboSourceFileType.Text.ToLower
                    sMask = Replace(sMask, ",", " ")
                    sMask = Replace(sMask, ";", " ")
                    Dim sMaskList() As String = Split(sMask, " ")
                    Dim sFiles() As String = Nothing
                    For Each sTmp As String In sMaskList
                        If (InStr(sTmp, "*") > 0) Or (InStr(sTmp, ".") > 0) Then
                            'Copy all
                            sFiles = IO.Directory.GetFiles(sSourceFolder, sTmp)
                            Dim nStartFileName As Integer = sSourceFolder.Length
                            For Each sFileLong As String In sFiles
                                Application.DoEvents()
                                If sList Is Nothing Then
                                    ReDim sList(0)
                                Else
                                    ReDim Preserve sList(sList.GetUpperBound(0) + 1)
                                End If
                                sList(sList.GetUpperBound(0)) = sFileLong.Substring(nStartFileName)
                            Next
                        End If
                    Next
                Else
                    ReDim sList(oListBox.Items.Count - 1)
                    For nItem As Integer = 0 To oListBox.Items.Count - 1
                        sList(nItem) = oListBox.Items.Item(nItem).ToString
                    Next
                End If
            Else
                ReDim sList(oListBox.SelectedItems.Count - 1)
                For nItem As Integer = 0 To oListBox.SelectedItems.Count - 1
                    sList(nItem) = oListBox.SelectedItems.Item(nItem).ToString
                Next
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoBackup", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        End Try
        Dim sChangeLogText As String
        Dim sTmpPath As String = String.Empty
        Try
            'special logic for sql server
            If oBackupSourceType = eBackupSourceType.DBSQL Then
                Select Case oBackupOutputType
                    Case eBackupOutputType.Folder, eBackupOutputType.Zip
                        Dim DB As clsSQLAccess = New clsSQLAccess
                        'Special backup through sequel server
                        'backup all SQL Server Databases prior to purging old records
                        Dim sTmpFolder As String = String.Empty
                        Call mPWCommon.GetDefaultFilePath(sTmpFolder, eDir.DBBackup, String.Empty, String.Empty)
                        For nIdx As Integer = 0 To sList.GetUpperBound(0)
                            Dim sBackupFileName As String = "dbo." & Strings.Replace(sList(nIdx).ToLower, "mdf", "bak", , , CompareMethod.Text)
                            IO.File.Delete(sTmpFolder & sBackupFileName)
                            Call DB.subBackupDB(sList(nIdx), sSourceFolder, sTmpFolder, Status)
                            Dim sTmp As String = "dbo." & Strings.Replace(sList(nIdx).ToLower, "mdf", "bak")
                            sChangeLogText = String.Format(gpsRM.GetString("psBACKUP_SQL_DB"), sList(nIdx), sTmp)
                            mPWCommon.AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                                      mcolZones.CurrentZoneNumber, _
                                      mcolZones.CurrentZone, String.Empty, _
                                      sChangeLogText, mcolZones.CurrentZone)
                            sList(nIdx) = sTmp
                            Status = sChangeLogText
                            Application.DoEvents()
                        Next 'sList
                        sSourceFolder = sTmpFolder
                    Case eBackupOutputType.CSV, eBackupOutputType.CSVZIP

                        If oBackupOutputType = eBackupOutputType.CSVZIP Then
                            sTmpPath = sGetTmpFileName("TmpCSVExportFolder")
                            IO.Directory.CreateDirectory(sTmpPath)
                            sSourceFolder = sTmpPath & "\"
                        Else
                            sTmpPath = sBackupPath
                        End If
                        Debug.Print(sTmpPath)
                        For nIdx As Integer = 0 To sList.GetUpperBound(0)
                            'start with the DB name
                            'Get a list of tables
                            Dim sDBName As String = sList(nIdx).Substring(0, sList(nIdx).Length - 4)
                            Dim sOutputPath As String = String.Empty
                            If sList.GetUpperBound(0) = 0 Then
                                'Use  selected file name
                                sOutputPath = sTmpPath
                            Else
                                'Use default file name in selected folder
                                sOutputPath = sTmpPath & "\" & sDBName & "." & gpsRM.GetString("psCSV_EXT")
                            End If
                            'Update name for zip feature to use later
                            sList(nIdx) = sDBName & "." & gpsRM.GetString("psCSV_EXT")
                            mPrintHtml.subStartDoc("", sDBName, True, , sOutputPath)

                            Dim dsTableNames As DataSet = subGetTableNames(sDBName)
                            Dim oTableNames As DataTable = dsTableNames.Tables(0)
                            'Go through the list of tables
                            For Each oRow As DataRow In oTableNames.Rows
                                'Get a name from the list.
                                Dim sTableName As String = oRow.Item(0).ToString
                                If sTableName IsNot Nothing AndAlso sTableName <> String.Empty Then
                                    Dim dsTable As DataSet = subGetDataBase(sDBName, sTableName)
                                    For Each oTable As DataTable In dsTable.Tables
                                        If (oTable IsNot Nothing) Then
                                            mPrintHtml.subSetPageFormat()
                                            mPrintHtml.subClearTableFormat()
                                            mPrintHtml.HeaderRowsPerTable = 1
                                            Dim sTitle(0) As String
                                            sTitle(0) = sTableName
                                            mPrintHtml.subAddObject(oTable, "", sTitle)
                                            mPrintHtml.subAddParagraph("")
                                        End If

                                    Next
                                End If
                            Next
                            mPrintHtml.subCloseFile("")
                        Next
                End Select
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoBackup", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        End Try
        Try
            'Select zip or folder output
            Select Case oBackupOutputType
                Case eBackupOutputType.Zip, eBackupOutputType.CSVZIP
                    'Build a path name
                    If sBackupPath.Substring(sBackupPath.Length - 1) = "\" Then
                        sBackupPath = sBackupPath.Substring(0, sBackupPath.Length - 1)
                    End If
                    If sBackupPath.ToLower.Substring(sBackupPath.Length - 4) <> ".zip" Then
                        sBackupPath = sBackupPath & ".zip"
                    End If
                    'Create a zip file
                    Dim oZip As ZipFile = Nothing
                    Try
                        If File.Exists(sBackupPath) Then
                            oZip = ZipFile.Read(sBackupPath)
                        Else
                            oZip = New ZipFile
                        End If
                        'Add each file to the zip file
                        For Each sTmp As String In sList
                            Try
                                If sSubFolder <> String.Empty Then
                                    oZip.UpdateFile(sSourceFolder & sTmp, sSubFolder)
                                    sChangeLogText = gpsRM.GetString("psCOPIED") & sTmp & gpsRM.GetString("psFROM") & sSourceFolder & _
                                             gpsRM.GetString("psTO") & sBackupPath & "\" & sSubFolder
                                Else
                                    oZip.UpdateFile(sSourceFolder & sTmp, String.Empty)
                                    sChangeLogText = gpsRM.GetString("psCOPIED") & sTmp & gpsRM.GetString("psFROM") & sSourceFolder & _
                                             gpsRM.GetString("psTO") & sBackupPath
                                End If
                                mPWCommon.AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                                          mcolZones.CurrentZoneNumber, _
                                          mcolZones.CurrentZone, String.Empty, _
                                          sChangeLogText, mcolZones.CurrentZone)
                                Status = sChangeLogText
                            Catch ex As Exception
                                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoBackup", _
                                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                        Status, MessageBoxButtons.OK)

                            End Try
                            Application.DoEvents()
                        Next
                        oZip.Save(sBackupPath)
                        oZip.Dispose()
                        oZip = Nothing

                    Catch ex As Exception
                        If oZip IsNot Nothing Then
                            oZip.Dispose()
                            oZip = Nothing
                        End If
                    End Try
                Case eBackupOutputType.Folder
                    'Build a path name
                    If sSubFolder <> String.Empty Then
                        sBackupPath = sBackupPath & "\" & sSubFolder
                    End If
                    If Not (IO.Directory.Exists(sBackupPath)) Then
                        IO.Directory.CreateDirectory(sBackupPath)
                    End If

                    Dim bSkip As Boolean = False
                    'Copy each file in the list.
                    If sList IsNot Nothing Then
                        For Each sTmp As String In sList
                            Try
                                bSkip = False
                                If File.Exists(sBackupPath & "\" & sTmp) Then
                                    If bYesToAll Then
                                        File.Delete(sBackupPath & "\" & sTmp)
                                    Else
                                        frmMultipleChoiceDlg.caption = gpsRM.GetString("psFILE_EXISTS")
                                        frmMultipleChoiceDlg.ButtonText(0) = gpsRM.GetString("psREPLACE")
                                        frmMultipleChoiceDlg.ButtonText(1) = gpsRM.GetString("psREPLACE_ALL")
                                        frmMultipleChoiceDlg.ButtonText(2) = gpsRM.GetString("psSKIP")
                                        frmMultipleChoiceDlg.ButtonText(3) = gpsRM.GetString("psCANCEL")
                                        frmMultipleChoiceDlg.NumButtons = 4
                                        frmMultipleChoiceDlg.label = String.Format(gpsRM.GetString("psFILE_EXISTS_PROMPT"), sBackupPath & "\" & sTmp)
                                        frmMultipleChoiceDlg.ShowDialog()
                                        Application.DoEvents()
                                        Select Case frmMultipleChoiceDlg.SelectedButton
                                            Case 0
                                                File.Delete(sBackupPath & "\" & sTmp)
                                            Case 1
                                                File.Delete(sBackupPath & "\" & sTmp)
                                                bYesToAll = True
                                            Case 2
                                                bSkip = True
                                            Case 3
                                                bCancel = True
                                                Exit For
                                            Case Else
                                        End Select
                                    End If
                                End If
                                If bSkip = False Then
                                    File.Copy(sSourceFolder & sTmp, sBackupPath & "\" & sTmp)
                                    sChangeLogText = gpsRM.GetString("psCOPIED") & sTmp & gpsRM.GetString("psFROM") & sSourceFolder & _
                                            gpsRM.GetString("psTO") & sBackupPath
                                    mPWCommon.AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                                              mcolZones.CurrentZoneNumber, _
                                              mcolZones.CurrentZone, String.Empty, _
                                              sChangeLogText, mcolZones.CurrentZone)
                                    Status = sChangeLogText
                                End If

                            Catch ex As Exception
                                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoBackup", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                        Status, MessageBoxButtons.OK)
                            End Try
                            Application.DoEvents()
                        Next
                    End If
            End Select
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoBackup", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        Finally
            subLogChanges()
        End Try
    End Sub
    Private Function sGetFileName(Optional ByRef sDefaultName As String = "", Optional ByRef sSubFolder As String = "", Optional ByVal bZip As Boolean = True) As String
        '********************************************************************************************
        'Description:  Choose a file for the backups
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Save a zip
        Dim sTmpFileName As String = String.Empty
        Dim oSFD As New SaveFileDialog
        Try
            With oSFD
                Dim sPathTmp As String = String.Empty
                If mPWCommon.GetDefaultFilePath(sPathTmp, eDir.PAINTworks, String.Empty, String.Empty) Then
                    oSFD.InitialDirectory = sPathTmp
                End If
                oSFD.CheckPathExists = True
                oSFD.AddExtension = True
                oSFD.OverwritePrompt = False
                oSFD.FileName = sDefaultName
                If bZip Then
                    oSFD.DefaultExt = gpsRM.GetString("psZIP_EXT")
                    oSFD.Filter = gpsRM.GetString("psZIPMASK")
                Else
                    oSFD.DefaultExt = gpsRM.GetString("psCSV_EXT")
                    oSFD.Filter = gpsRM.GetString("psCSVMASK")
                End If
                oSFD.FilterIndex = 1
                Dim oVal As DialogResult = oSFD.ShowDialog
                If (oVal = System.Windows.Forms.DialogResult.OK) Then
                    sTmpFileName = oSFD.FileName
                Else
                    sTmpFileName = String.Empty
                End If
            End With
            If File.Exists(sTmpFileName) Then
                If bZip Then
                    frmMultipleChoiceDlg.caption = gpsRM.GetString("psZIP_FILE_EXISTS")
                    frmMultipleChoiceDlg.ButtonText(0) = gpsRM.GetString("psUPDATE")
                    frmMultipleChoiceDlg.ButtonText(1) = gpsRM.GetString("psREPLACE")
                    frmMultipleChoiceDlg.ButtonText(2) = gpsRM.GetString("psCANCEL")
                    frmMultipleChoiceDlg.ButtonText(3) = String.Empty
                    frmMultipleChoiceDlg.NumButtons = 3
                    frmMultipleChoiceDlg.label = String.Format(gpsRM.GetString("psZIP_FILE_EXISTS_PROMPT"), sTmpFileName)
                    frmMultipleChoiceDlg.ShowDialog()
                    Select Case frmMultipleChoiceDlg.SelectedButton
                        Case 0
                            'OK, continue
                        Case 1
                            File.Delete(sTmpFileName)
                        Case 2
                            sGetFileName = String.Empty
                        Case Else
                    End Select
                Else
                    frmMultipleChoiceDlg.caption = gpsRM.GetString("psFILE_EXISTS")
                    frmMultipleChoiceDlg.ButtonText(0) = gpsRM.GetString("psREPLACE")
                    frmMultipleChoiceDlg.ButtonText(1) = gpsRM.GetString("psCANCEL")
                    frmMultipleChoiceDlg.ButtonText(2) = String.Empty
                    frmMultipleChoiceDlg.ButtonText(3) = String.Empty
                    frmMultipleChoiceDlg.NumButtons = 2
                    frmMultipleChoiceDlg.label = String.Format(gpsRM.GetString("psFILE_EXISTS_PROMPT"), sTmpFileName)
                    frmMultipleChoiceDlg.ShowDialog()
                    Select Case frmMultipleChoiceDlg.SelectedButton
                        Case 0
                            File.Delete(sTmpFileName)
                        Case 11
                            sGetFileName = String.Empty
                        Case Else
                    End Select
                End If
            End If
        Catch ex As Exception
            sTmpFileName = String.Empty
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: sGetFileName", _
                "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        End Try
        Try
            If (sTmpFileName <> String.Empty) And (sSubFolder <> String.Empty) Then
                msDefaultFolder = sTmpFileName
                Dim sMsg As String = String.Format(gpsRM.GetString("psADD_DEFAULT_SUBFOLDER"), sSubFolder)
                Dim sCaption As String = gpsRM.GetString("psADD_DEFAULT_SUBFOLDER_CAPTION")
                Dim oVal As DialogResult = MessageBox.Show(sMsg, sCaption, MessageBoxButtons.YesNoCancel, _
                                MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, _
                                MessageBoxOptions.DefaultDesktopOnly)
                Select Case oVal
                    Case Windows.Forms.DialogResult.Yes
                        'Return subfolder name
                    Case Windows.Forms.DialogResult.No
                        'Clear subfoldername
                        sSubFolder = String.Empty
                    Case Windows.Forms.DialogResult.Cancel
                        sTmpFileName = String.Empty
                End Select
            End If
        Catch ex As Exception
            sTmpFileName = String.Empty
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: sGetFileName", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        End Try
        sGetFileName = sTmpFileName
    End Function
    Private Function sGetFolderName(Optional ByRef sDefaultPath As String = "", Optional ByRef sDefaultSubFolder As String = "") As String
        '********************************************************************************************
        'Description:  Pick out a folder for the backups
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Find a folder
        Dim sTmpFolderName As String = String.Empty
        Dim oFB As New FolderBrowserDialog
        Try
            With oFB
                .ShowNewFolderButton = True
                Dim sPathTmp As String = String.Empty
                If sDefaultPath = String.Empty Then
                    If mPWCommon.GetDefaultFilePath(sPathTmp, eDir.PAINTworks, String.Empty, String.Empty) Then
                        .SelectedPath = sPathTmp
                    End If
                Else
                    .SelectedPath = sDefaultPath
                End If
                .Description = gpsRM.GetString("psSELECT_FOLDER")
                Dim oVal As DialogResult = .ShowDialog()
                If oVal = Windows.Forms.DialogResult.OK Then
                    sTmpFolderName = .SelectedPath
                Else
                    sTmpFolderName = String.Empty
                End If

                If (sTmpFolderName <> String.Empty) And (sDefaultSubFolder = String.Empty) Then
                    'This backup doesn't have a default subfolder, save the default folder
                    sDefaultPath = sTmpFolderName
                End If
            End With
        Catch ex As Exception
            sTmpFolderName = String.Empty
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: sGetFolderName", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        End Try
        Try
            If (sTmpFolderName <> String.Empty) And (sDefaultSubFolder <> String.Empty) Then
                Dim sTmp As String() = Split(sTmpFolderName, "\")
                If sDefaultSubFolder.Trim.ToLower <> sTmp(sTmp.GetUpperBound(0)) Then
                    'Update the default path if it's not the normal subfolder
                    sDefaultPath = sTmpFolderName
                    'Offer to add the standard subfolder
                    Dim sMsg As String = String.Format(gpsRM.GetString("psADD_DEFAULT_SUBFOLDER"), sDefaultSubFolder)
                    Dim sCaption As String = gpsRM.GetString("psADD_DEFAULT_SUBFOLDER_CAPTION")
                    Dim oVal As DialogResult = MessageBox.Show(sMsg, sCaption, MessageBoxButtons.YesNoCancel, _
                                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, _
                                    MessageBoxOptions.DefaultDesktopOnly)
                    Select Case oVal
                        Case Windows.Forms.DialogResult.Yes
                            sTmpFolderName = sTmpFolderName & "\" & sDefaultSubFolder
                            Try
                                If Not (IO.Directory.Exists(sTmpFolderName)) Then
                                    IO.Directory.CreateDirectory(sTmpFolderName)
                                End If
                            Catch ex As Exception
                                sTmpFolderName = String.Empty
                            End Try
                        Case Windows.Forms.DialogResult.No
                            'Nothing to do here
                        Case Windows.Forms.DialogResult.Cancel
                            sTmpFolderName = String.Empty
                    End Select

                End If

            End If
        Catch ex As Exception
            sTmpFolderName = String.Empty
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: sGetFolderName", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        End Try
        sGetFolderName = sTmpFolderName
    End Function
    Private Sub subBackupRobotNode(ByRef oBackupOutputType As eBackupOutputType, ByRef oBackupSourceType As eBackupSourceType, _
                            ByRef sBackupPath As String, ByRef oNode As Windows.Forms.TreeNode, _
                            ByVal bPerFolder As Boolean, ByVal bPerRobot As Boolean, ByVal bAll As Boolean, _
                              Optional ByRef bYesToAll As Boolean = False, Optional ByRef bCancel As Boolean = False, _
                              Optional ByRef bGotPermission As Boolean = False, Optional ByRef bReplace As Boolean = False)
        '********************************************************************************************
        'Description:  Run a backup of robot files from one of folder tree
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If oBackupOutputType = eBackupOutputType.Folder Then
            IO.Directory.CreateDirectory(sBackupPath & "\" & oNode.Text)
        End If
        Dim sTmpBackupPath As String = sBackupPath
        Dim sTmpSubFolder As String = String.Empty
        If bPerFolder Then
            If oBackupOutputType = eBackupOutputType.Zip Then
                If bPerRobot Then
                    sTmpBackupPath = sTmpBackupPath & "\" & mcolZones.CurrentZone & "_" & oNode.Text & "_"
                Else
                    sTmpBackupPath = sTmpBackupPath & "\" & mcolZones.CurrentZone & "_" & oNode.Text
                End If
            Else
                sTmpBackupPath = sTmpBackupPath & "\" & mcolZones.CurrentZone & "_" & oNode.Text & "\"
            End If
        Else
            sTmpSubFolder = oNode.Text & "\"
        End If
        Dim bFirst As Boolean = bPerFolder
        For Each oRobot As Object In chkRobots.CheckedItems

            Dim sTmpBackupPath2 As String = sTmpBackupPath
            Dim sTmpSubFolder2 As String = sTmpSubFolder
            Dim sRobotSource As String = oNode.Tag.ToString & oRobot.ToString & "\"
            If bPerRobot Then
                If (oBackupOutputType = eBackupOutputType.Zip) And bPerFolder = False Then
                    sTmpBackupPath2 = sTmpBackupPath2 & "\" & oRobot.ToString
                Else
                    sTmpBackupPath2 = sTmpBackupPath2 & oRobot.ToString
                    If oBackupOutputType <> eBackupOutputType.Zip Then
                        sTmpBackupPath2 = sTmpBackupPath2 & "\"
                    End If
                End If
            Else
                sTmpSubFolder2 = sTmpSubFolder2 & oRobot.ToString & "\"
            End If
            If (oBackupOutputType = eBackupOutputType.Folder) Then
                If Not (IO.Directory.Exists(sTmpBackupPath2 & "\" & sTmpSubFolder2)) Then
                    IO.Directory.CreateDirectory(sTmpBackupPath2 & "\" & sTmpSubFolder2)
                End If
            Else
                If bFirst Or (bPerRobot And bPerFolder) Then
                    bFirst = False
                    If My.Computer.FileSystem.FileExists(sTmpBackupPath2 & ".zip") Then
                        If bGotPermission Then
                            If bReplace Then
                                File.Delete(sTmpBackupPath2 & ".zip")
                            End If
                        Else
                            bCancel = Not (bCheckOverwrite(sTmpBackupPath2 & ".zip", bGotPermission, bReplace))
                        End If
                    End If
                End If
            End If
            subDoBackup(oBackupOutputType, oBackupSourceType, sTmpBackupPath2, sTmpSubFolder2, bAll, bYesToAll, bCancel, sRobotSource)
            If bCancel Then
                Exit For
            End If
        Next
    End Sub
    Private Sub AddProgressHandler(ByVal sender As Object, ByVal e As AddProgressEventArgs)
        Select Case e.EventType
            Case ZipProgressEventType.Adding_Started
                Status = "Adding files to the zip..."
                Exit Select
            Case ZipProgressEventType.Adding_AfterAddEntry
                Status = String.Format("Adding file {0}", e.CurrentEntry.FileName)
                Exit Select
            Case ZipProgressEventType.Adding_Completed
                Status = "Added all files"
                Exit Select
            Case Else
                Debug.Print(e.EventType.ToString)
        End Select
    End Sub

    Private Sub subDoFolderBackup(ByVal bZip As Boolean, ByRef sFromDir As String, _
                                  ByRef sDest As String, _
                                  Optional ByRef sExclude() As String = Nothing, _
                                  Optional ByRef sZipSubFolder As String = "")
        '********************************************************************************************
        'Description:  Do a simple folder copy or add it to a zip file
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        Dim bExclude As Boolean = (sExclude IsNot Nothing)
        Try
            If bExclude Then
                For nExclude As Integer = 0 To sExclude.GetUpperBound(0)
                    Dim sTmp As String = sExclude(nExclude).Substring(sExclude(nExclude).Length - 1)
                    If sTmp = "\" Or sTmp = "/" Then
                        sExclude(nExclude) = sExclude(nExclude).Substring(0, sExclude(nExclude).Length - 1)
                    End If
                Next
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoFolderBackup", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            bExclude = False
        End Try
        Dim oZip As ZipFile = Nothing
        Try
            If bZip Then
                If File.Exists(sDest) Then
                    oZip = ZipFile.Read(sDest)
                Else
                    oZip = New ZipFile
                End If
                AddHandler oZip.AddProgress, AddressOf AddProgressHandler
            End If
            'Break it up into subfolders and files so it doesn't appear locked up
            Dim sFolders() As String = IO.Directory.GetDirectories(sFromDir)
            Dim sFiles() As String = IO.Directory.GetFiles(sFromDir)
            Dim bExcludeThisOne As Boolean = False
            For Each sFile As String In sFiles
                bExcludeThisOne = False
                If bExclude Then
                    For Each sTmp As String In sExclude
                        If InStr(sFile, sTmp) > 0 Then
                            bExcludeThisOne = True
                            Exit For
                        End If
                    Next
                End If
                If bExcludeThisOne = False Then
                    Dim sDestFile As String = Replace(sFile, sFromDir, sDest)
                    If bZip Then
                        oZip.UpdateFile(sFile, sZipSubFolder)
                    Else
                        My.Computer.FileSystem.CopyFile(sFile, sDestFile, True)
                    End If
                    Status = String.Format(gpsRM.GetString("psCOPIED_1_2"), sFile, sDestFile)
                    Application.DoEvents()
                End If
            Next
            For Each sFolder As String In sFolders
                sFolder = sFolder & "\"
                bExcludeThisOne = False
                If bExclude Then
                    For Each sTmp As String In sExclude
                        If InStr(sFolder, sTmp) > 0 Then
                            bExcludeThisOne = True
                            Exit For
                        End If
                    Next
                End If
                If bExcludeThisOne = False Then
                    Dim sDestFolder As String = String.Empty
                    If bZip Then
                        sDestFolder = Replace(sFolder, sFromDir, String.Empty)
                        oZip.UpdateDirectory(sFolder, sZipSubFolder & sDestFolder)
                        'oZip.AddSelectedFiles("name != */obj* and name != */bin*", sFolder, sZipSubFolder & sDestFolder, True)
                        Status = String.Format(gpsRM.GetString("psCOPIED_1_2"), sFolder, sDest)
                    Else
                        sDestFolder = Replace(sFolder, sFromDir, sDest)
                        My.Computer.FileSystem.CopyDirectory(sFolder, sDestFolder, True)
                        Status = String.Format(gpsRM.GetString("psCOPIED_1_2"), sFolder, sDestFolder)
                    End If
                    Application.DoEvents()
                End If
            Next
            Debug.Print("")
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoFolderBackup", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        Finally
            If oZip IsNot Nothing Then
                oZip.Save(sDest)
                oZip.Dispose()
                oZip = Nothing
            End If
            Me.Cursor = Cursors.Default
        End Try
    End Sub
    Private Function bCheckOverwrite(ByRef sBackupPath As String, ByRef bGotPermission As Boolean, Optional ByRef bReplace As Boolean = False) As Boolean
        Try
            Dim bCheck As Boolean = False
            If My.Computer.FileSystem.FileExists(sBackupPath) Then
                Dim sTmp() As String = Split(sBackupPath, ".")
                If sTmp(sTmp.Length - 1).ToLower = "zip" Then
                    'special handling for zip files
                    frmMultipleChoiceDlg.caption = gpsRM.GetString("psZIP_FILE_EXISTS")
                    frmMultipleChoiceDlg.ButtonText(0) = gpsRM.GetString("psUPDATE")
                    frmMultipleChoiceDlg.ButtonText(1) = gpsRM.GetString("psREPLACE")
                    frmMultipleChoiceDlg.ButtonText(2) = gpsRM.GetString("psCANCEL")
                    frmMultipleChoiceDlg.ButtonText(3) = String.Empty
                    frmMultipleChoiceDlg.NumButtons = 3
                    frmMultipleChoiceDlg.label = String.Format(gpsRM.GetString("psZIP_FILE_EXISTS_PROMPT"), sBackupPath)
                    frmMultipleChoiceDlg.ShowDialog()
                    Select Case frmMultipleChoiceDlg.SelectedButton
                        Case 0
                            bReplace = False
                            bGotPermission = True
                            Return True
                        Case 1
                            bReplace = True
                            File.Delete(sBackupPath)
                            bGotPermission = True
                            Return True
                        Case 2
                            bReplace = False
                            Return False
                        Case Else
                    End Select

                Else
                    bCheck = True
                    frmMultipleChoiceDlg.caption = gpsRM.GetString("psFILE_EXISTS")
                    frmMultipleChoiceDlg.label = String.Format(gpsRM.GetString("psFILE_EXISTS_PROMPT"), sBackupPath)
                End If
            Else
                If IO.Directory.Exists(sBackupPath) Then
                    Dim sFolders() As String = IO.Directory.GetDirectories(sBackupPath)
                    Dim sFiles() As String = IO.Directory.GetFiles(sBackupPath)
                    If (sFolders IsNot Nothing AndAlso sFolders.GetUpperBound(0) > 0) Or _
                       (sFiles IsNot Nothing AndAlso sFiles.GetUpperBound(0) > 0) Then
                        frmMultipleChoiceDlg.caption = gpsRM.GetString("psFOLDER_NOT_EMPTY")
                        frmMultipleChoiceDlg.label = String.Format(gpsRM.GetString("psFOLDER_NOT_EMPTY_PROMPT"), sBackupPath)
                        bCheck = True
                    End If
                End If
            End If
            If bCheck Then
                frmMultipleChoiceDlg.ButtonText(0) = gpsRM.GetString("psOVERWRITE")
                frmMultipleChoiceDlg.ButtonText(1) = gpsRM.GetString("psCANCEL")
                frmMultipleChoiceDlg.ButtonText(2) = String.Empty
                frmMultipleChoiceDlg.ButtonText(3) = String.Empty
                frmMultipleChoiceDlg.NumButtons = 2
                frmMultipleChoiceDlg.ShowDialog()
                Select Case frmMultipleChoiceDlg.SelectedButton
                    Case 0
                        bGotPermission = True
                        Return True
                    Case Else
                        Return False
                End Select
            Else
                Return True
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Function: bCheckOverwrite", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)

        End Try
        Return False
    End Function
    Private Sub subCleanupSource(ByRef sPath As String)
        '********************************************************************************************
        'Description:  Run a backup from the application tab
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/23/12  MSW     Clean up obj and bin folders from source code
        '********************************************************************************************
        'Just ignore errors.  If a file is in use it'll just skip to the next one
        Try
            Dim sFiles() As String = IO.Directory.GetFiles(sPath, "*.bak")
            For Each sFile As String In sFiles
                Try
                    IO.File.Delete(sFile)
                Catch ex As Exception
                End Try
            Next
            Dim sFolders() As String = IO.Directory.GetDirectories(sPath)
            For Each sFolder As String In sFolders
                Dim sTmpFolder As String = sFolder.Substring(sFolder.Length - 3).ToLower
                Select Case sTmpFolder
                    Case "bin", "obj"
                        Try
                            IO.Directory.Delete(sFolder, True)
                        Catch ex As Exception
                        End Try
                    Case Else
                        If mPWCommon.InIDE Then
                            'Special case
                            If InStr(sFolder.ToLower, "archive.net") = 0 Then
                                subCleanupSource(sFolder)
                            End If
                        Else
                            subCleanupSource(sFolder)
                        End If
                End Select
            Next
        Catch ex As Exception
        End Try
    End Sub

    Private Sub subDoApplicationBackup(ByVal bZip As Boolean, ByRef sBackupPath As String, ByRef sDefaultName As String)
        '********************************************************************************************
        'Description:  Run a backup from the application tab
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/30/13  MSW     Add the additional folders from the backup config, use text compare
        '********************************************************************************************
        Dim oCulture As CultureInfo = DisplayCulture
        Dim sPaintFolderName As String = String.Empty
        Dim sDefaultSubFolder As String = String.Empty
        Dim sExludeArray() As String = Nothing
        'Application Backup
        Dim bPerFolder As Boolean = False
        Dim bOverwrite As Boolean = False
        Dim bReplace As Boolean = False
        Try
            If bZip Then
                Dim sTmp() As String = Split(msPAINTworksPath, "\")
                If sTmp.Length >= 2 Then
                    sPaintFolderName = sTmp(sTmp.Length - 2)
                    sPaintFolderName = sPaintFolderName.Substring(0, 1).ToUpper & sPaintFolderName.Substring(1)
                End If
                If (lstApplication.SelectedItems.Count = 1) Then
                    bPerFolder = False
                Else
                    'more complicated choices
                    Dim sMsg As String = gpsRM.GetString("psZIP_PER_FOLDER", oCulture)
                    Dim sCaption As String = gpsRM.GetString("psZIP_PER_FOLDER_CAPTION", oCulture)
                    Dim oVal As DialogResult = MessageBox.Show(sMsg, sCaption, MessageBoxButtons.YesNoCancel, _
                                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, _
                                    MessageBoxOptions.DefaultDesktopOnly)
                    bPerFolder = (oVal = Windows.Forms.DialogResult.Yes)
                End If
                If bPerFolder Then
                    'Get a folder and use default names
                    sBackupPath = sGetFolderName(msDefaultFolder, sDefaultSubFolder)
                    If sBackupPath = String.Empty Then
                        Exit Sub
                    End If
                    sDefaultSubFolder = String.Empty
                    sBackupPath = sBackupPath & "\"
                Else
                    'All in one zip, get a name
                    If (lstApplication.SelectedItems.Count = 1) Then
                        'Change the default name

                        Select Case lstApplication.SelectedItems(0).ToString
                            Case gpsRM.GetString("psPAINT_COMMON", oCulture)
                                sDefaultName = sBackupPath & mcolZones.CurrentZone & "_" & _
                                                        sPaintFolderName & "_" & _
                                                        gpsRM.GetString("psCOMMON_ZIP", oCulture)
                            Case gpsRM.GetString("psSOURCE", oCulture)
                                sDefaultName = sBackupPath & mcolZones.CurrentZone & "_" & _
                                                        gpsRM.GetString("psSOURCE_ZIP", oCulture)
                            Case gpsRM.GetString("psHELP", oCulture)
                                sDefaultName = sBackupPath & mcolZones.CurrentZone & "_" & _
                                                        gpsRM.GetString("psHELP_ZIP", oCulture)
                            Case gpsRM.GetString("psFANUCMANUALS", oCulture)
                                sDefaultName = sBackupPath & mcolZones.CurrentZone & "_" & _
                                                        gpsRM.GetString("psFANUCMANUALS_ZIP", oCulture)
                            Case Else
                                sDefaultName = sBackupPath & mcolZones.CurrentZone & "_" & _
                                                        lstApplication.SelectedItems(0).ToString
                        End Select
                    End If
                    sBackupPath = sGetFileName(sDefaultName, sDefaultSubFolder)
                    If sBackupPath = String.Empty Then
                        Exit Sub
                    End If
                    If (bCheckOverwrite(sBackupPath, bOverwrite, bReplace)) Then
                        bOverwrite = True
                    Else
                        'Cancelled
                        Exit Sub
                    End If
                End If
            Else
                sBackupPath = sGetFolderName(msDefaultFolder, sDefaultSubFolder)
                If sBackupPath = String.Empty Then
                    Exit Sub
                End If
                sDefaultSubFolder = String.Empty
                sBackupPath = sBackupPath & "\"
            End If

            Dim sFromDir As String = String.Empty
            Dim sDest As String = String.Empty
            Dim sZipSubFolder As String = String.Empty
            For Each oItem As Object In lstApplication.SelectedItems
                Select Case oItem.ToString
                    Case gpsRM.GetString("psPAINT_COMMON", oCulture)
                        'Clean up orphaned IPC messages
                        Dim sIPCPath As String = String.Empty
                        If mPWCommon.GetDefaultFilePath(sIPCPath, eDir.IPC, String.Empty, String.Empty) Then
                            Dim sIPCFiles() As String = IO.Directory.GetFiles(sIPCPath, "*.*")
                            For Each sIPCFile As String In sIPCFiles
                                Try
                                    IO.File.Delete(sIPCFile)
                                Catch ex As Exception
                                End Try
                            Next
                        End If
                        If bZip And bPerFolder Then
                            sDest = sBackupPath & mcolZones.CurrentZone & "_" & _
                                    sPaintFolderName & "_" & _
                                    gpsRM.GetString("psCOMMON_ZIP", oCulture)
                            If bOverwrite = False Then
                                If Not (bCheckOverwrite(sDest, bOverwrite, bReplace)) Then
                                    'Cancelled
                                    Exit For
                                End If
                            Else
                                If bReplace Then
                                    File.Delete(sDest)
                                End If
                            End If
                        Else
                            sDest = sBackupPath
                        End If
                        If msVbappsPath <> String.Empty Then
                            sFromDir = msVbappsPath
                            If bZip Then
                                sZipSubFolder = Replace(sFromDir, msPAINTworksPath, String.Empty, , , CompareMethod.Text)
                            Else
                                sDest = Replace(sFromDir, msPAINTworksPath, sBackupPath)
                                If bOverwrite = False Then
                                    If Not (bCheckOverwrite(sDest, bOverwrite)) Then
                                        'Cancelled
                                        Exit For
                                    End If
                                End If
                            End If
                            sExludeArray = Nothing
                            subDoFolderBackup(bZip, sFromDir, sDest, sExludeArray, sZipSubFolder)
                        End If
                        If msProfilePath <> String.Empty Then
                            sFromDir = msProfilePath
                            If bZip Then
                                sZipSubFolder = Replace(sFromDir, msPAINTworksPath, String.Empty, , , CompareMethod.Text)
                            Else
                                sDest = Replace(sFromDir, msPAINTworksPath, sBackupPath, , , CompareMethod.Text)
                                If bOverwrite = False Then
                                    If Not (bCheckOverwrite(sDest, bOverwrite)) Then
                                        'Cancelled
                                        Exit For
                                    End If
                                End If
                            End If
                            sExludeArray = Nothing
                            subDoFolderBackup(bZip, sFromDir, sDest, sExludeArray, sZipSubFolder)
                        End If
                        If msDmonViewerPath <> String.Empty Then
                            sFromDir = msDmonViewerPath
                            If bZip Then
                                sZipSubFolder = Replace(sFromDir, msPAINTworksPath, String.Empty, , , CompareMethod.Text)
                            Else
                                sDest = Replace(sFromDir, msPAINTworksPath, sBackupPath, , , CompareMethod.Text)
                                If bOverwrite = False Then
                                    If Not (bCheckOverwrite(sDest, bOverwrite)) Then
                                        'Cancelled
                                        Exit For
                                    End If
                                End If
                            End If
                            sExludeArray = Nothing
                            subDoFolderBackup(bZip, sFromDir, sDest, sExludeArray, sZipSubFolder)
                        End If
                        Dim sChangeLogText As String = String.Format(gpsRM.GetString("psBACKUP_COMMON_APP"), sBackupPath)
                        mPWCommon.AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                                  mcolZones.CurrentZoneNumber, _
                                  mcolZones.CurrentZone, String.Empty, _
                                  sChangeLogText, mcolZones.CurrentZone)
                    Case gpsRM.GetString("psSOURCE", oCulture)
                        sFromDir = msSourcePath
                        If bZip Then
                            If bPerFolder Then
                                sDest = sBackupPath & mcolZones.CurrentZone & "_" & _
                                                    gpsRM.GetString("psSOURCE_ZIP", oCulture)
                                If bOverwrite = False Then
                                    If Not (bCheckOverwrite(sDest, bOverwrite, bReplace)) Then
                                        'Cancelled
                                        Exit For
                                    End If
                                Else
                                    If bReplace Then
                                        File.Delete(sDest)
                                    End If
                                End If
                            Else
                                sDest = sBackupPath
                            End If
                            sZipSubFolder = Replace(sFromDir, msPAINTworksPath, String.Empty, , , CompareMethod.Text)
                        Else
                            sDest = Replace(sFromDir, msPAINTworksPath, sBackupPath, , , CompareMethod.Text)
                            If bOverwrite = False Then
                                If Not (bCheckOverwrite(sDest, bOverwrite)) Then
                                    'Cancelled
                                    Exit For
                                End If
                            End If
                        End If
                        'Cleanup source code
                        subCleanupSource(msSourcePath)
                        ReDim sExludeArray(1)
                        sExludeArray(0) = "\obj"
                        sExludeArray(1) = "\bin"
                        subDoFolderBackup(bZip, sFromDir, sDest, sExludeArray, sZipSubFolder)
                        Dim sChangeLogText As String = String.Format(gpsRM.GetString("psBACKUP_SOURCE"), sBackupPath)
                        mPWCommon.AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                                  mcolZones.CurrentZoneNumber, _
                                  mcolZones.CurrentZone, String.Empty, _
                                  sChangeLogText, mcolZones.CurrentZone)
                    Case gpsRM.GetString("psHELP", oCulture)
                        sFromDir = msHelpPath
                        If bZip Then
                            If bPerFolder Then
                                sDest = sBackupPath & mcolZones.CurrentZone & "_" & _
                                                    gpsRM.GetString("psHELP_ZIP", oCulture)
                                If bOverwrite = False Then
                                    If Not (bCheckOverwrite(sDest, bOverwrite, bReplace)) Then
                                        'Cancelled
                                        Exit For
                                    End If
                                Else
                                    If bReplace Then
                                        File.Delete(sDest)
                                    End If
                                End If
                            Else
                                sDest = sBackupPath
                            End If
                            sZipSubFolder = Replace(sFromDir, msPAINTworksPath, String.Empty, , , CompareMethod.Text)
                        Else
                            sDest = Replace(sFromDir, msPAINTworksPath, sBackupPath, , , CompareMethod.Text)
                            If bOverwrite = False Then
                                If Not (bCheckOverwrite(sDest, bOverwrite)) Then
                                    'Cancelled
                                    Exit For
                                End If
                            End If
                        End If
                        ReDim sExludeArray(0)
                        sExludeArray(0) = msFanucManualsPath
                        subDoFolderBackup(bZip, sFromDir, sDest, sExludeArray, sZipSubFolder)
                        Dim sChangeLogText As String = String.Format(gpsRM.GetString("psBACKUP_HELP"), sBackupPath)
                        mPWCommon.AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                                  mcolZones.CurrentZoneNumber, _
                                  mcolZones.CurrentZone, String.Empty, _
                                  sChangeLogText, mcolZones.CurrentZone)
                    Case gpsRM.GetString("psFANUCMANUALS", oCulture)
                        sFromDir = msFanucManualsPath
                        If bZip Then
                            If bPerFolder Then
                                sDest = sBackupPath & mcolZones.CurrentZone & "_" & _
                                        gpsRM.GetString("psFANUCMANUALS_ZIP", oCulture)
                                If bOverwrite = False Then
                                    If Not (bCheckOverwrite(sDest, bOverwrite, bReplace)) Then
                                        'Cancelled
                                        Exit For
                                    End If
                                Else
                                    If bReplace Then
                                        File.Delete(sDest)
                                    End If
                                End If
                            Else
                                sDest = sBackupPath
                            End If
                            sZipSubFolder = Replace(sFromDir, msPAINTworksPath, String.Empty, , , CompareMethod.Text)
                        Else
                            sDest = Replace(sFromDir, msPAINTworksPath, sBackupPath, , , CompareMethod.Text)
                            If bOverwrite = False Then
                                If Not (bCheckOverwrite(sDest, bOverwrite)) Then
                                    'Cancelled
                                    Exit For
                                End If
                            End If
                        End If
                        sExludeArray = Nothing
                        subDoFolderBackup(bZip, sFromDir, sDest, sExludeArray, sZipSubFolder)
                        Dim sChangeLogText As String = String.Format(gpsRM.GetString("psBACKUP_MANUALS"), sBackupPath)
                        mPWCommon.AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                                  mcolZones.CurrentZoneNumber, _
                                  mcolZones.CurrentZone, String.Empty, _
                                  sChangeLogText, mcolZones.CurrentZone)
                    Case Else 'Additional Folders
                        For nItem As Integer = 0 To moAdditionalFolders.NumFolders - 1
                            If moAdditionalFolders.Folders(nItem).FolderName = oItem.ToString Then
                                sFromDir = moAdditionalFolders.Folders(nItem).FolderPath

                                If bZip Then
                                    If bPerFolder Then
                                        sDest = sBackupPath & mcolZones.CurrentZone & "_" & oItem.ToString _
                                                & "." & gpsRM.GetString("psZIP_EXT", oCulture)
                                        If bOverwrite = False Then
                                            If Not (bCheckOverwrite(sDest, bOverwrite, bReplace)) Then
                                                'Cancelled
                                                Exit For
                                            End If
                                        Else
                                            If bReplace Then
                                                File.Delete(sDest)
                                            End If
                                        End If
                                    Else
                                        sDest = sBackupPath
                                    End If
                                    sZipSubFolder = Replace(sFromDir, msPAINTworksPath, String.Empty, , , CompareMethod.Text)
                                Else
                                    sDest = Replace(sFromDir, msPAINTworksPath, sBackupPath, , , CompareMethod.Text)
                                    If bOverwrite = False Then
                                        If Not (bCheckOverwrite(sDest, bOverwrite)) Then
                                            'Cancelled
                                            Exit For
                                        End If
                                    End If
                                End If
                                sExludeArray = Nothing
                                subDoFolderBackup(bZip, sFromDir, sDest, sExludeArray, sZipSubFolder)
                                Dim sChangeLogText As String = String.Format(oItem.ToString, sBackupPath)
                                mPWCommon.AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                                          mcolZones.CurrentZoneNumber, _
                                          mcolZones.CurrentZone, String.Empty, _
                                          sChangeLogText, mcolZones.CurrentZone)
                                Exit For
                            End If
                        Next

                End Select
            Next

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoApplicationBackup", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        Finally
            subLogChanges()
        End Try
    End Sub
    Private Sub btnBackupTo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBackupToFolderDBSql.Click, _
    btnBackupToZipDBSql.Click, btnBackupToFolderDBAccess.Click, btnBackupToZipDBAccess.Click, _
    btnBackupToFolderDBXML.Click, btnBackupToZipDBXML.Click, _
    btnBackupToFolderNotepad.Click, btnBackupToZipNotepad.Click, _
    btnBackupToFolderImage.Click, btnBackupToZipImage.Click, _
    btnBackupToFolderRobots.Click, btnBackupToZipRobots.Click, _
    btnExportToCSVDBSql.Click, btnExportToCSVZIPDBSql.Click, _
    btnBackupToFolderApplication.Click, btnBackupToZipsApplication.Click, _
    btnBackupToFolderDMON.Click, btnBackupToZipDMON.Click, _
    btnBackupToFolderDMONArchive.Click, btnBackupToZipDMONArchive.Click
        '********************************************************************************************
        'Description:  Run a backup from one of the DB backup buttons
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim obtn As Button = DirectCast(sender, Button)

        Dim oBackupSourceType As eBackupSourceType = eBackupSourceType.None
        Dim oBackupOutputType As eBackupOutputType = eBackupOutputType.None
        Dim sBackupPath As String = String.Empty
        Dim sDefaultName As String = String.Empty
        Dim sDefaultSubFolder As String = String.Empty
        Dim oListBox As ListBox = Nothing
        Dim bAll As Boolean = False
        Dim oCulture As CultureInfo = DisplayCulture
        Try
            subEnableControls(False)
            Select Case obtn.Name
                Case btnBackupToFolderDBSql.Name
                    oBackupOutputType = eBackupOutputType.Folder
                    oBackupSourceType = eBackupSourceType.DBSQL
                    oListBox = lstDBSQL
                    sDefaultName = String.Empty
                    sDefaultSubFolder = String.Empty
                Case btnBackupToZipDBSql.Name
                    oBackupOutputType = eBackupOutputType.Zip
                    oBackupSourceType = eBackupSourceType.DBSQL
                    sDefaultName = mcolZones.CurrentZone & "_" & "Database"
                    sDefaultSubFolder = String.Empty
                    oListBox = lstDBSQL
                Case btnExportToCSVDBSql.Name
                    oBackupOutputType = eBackupOutputType.CSV
                    oBackupSourceType = eBackupSourceType.DBSQL
                    oListBox = lstDBSQL
                    sDefaultName = String.Empty
                    sDefaultSubFolder = String.Empty
                Case btnExportToCSVZIPDBSql.Name
                    oBackupOutputType = eBackupOutputType.CSVZIP
                    oBackupSourceType = eBackupSourceType.DBSQL
                    oListBox = lstDBSQL
                    sDefaultName = mcolZones.CurrentZone & "_" & "SQL_DB_in_CSV"
                    sDefaultSubFolder = String.Empty
                Case btnBackupToFolderDBAccess.Name
                    oBackupOutputType = eBackupOutputType.Folder
                    oBackupSourceType = eBackupSourceType.DBAccess
                    sDefaultName = String.Empty
                    sDefaultSubFolder = String.Empty
                    oListBox = lstDBAccess
                Case btnBackupToZipDBAccess.Name
                    oBackupOutputType = eBackupOutputType.Zip
                    oBackupSourceType = eBackupSourceType.DBAccess
                    sDefaultName = mcolZones.CurrentZone & "_" & "Database"
                    sDefaultSubFolder = String.Empty
                    oListBox = lstDBAccess
                Case btnBackupToFolderDBXML.Name
                    oBackupOutputType = eBackupOutputType.Folder
                    oBackupSourceType = eBackupSourceType.DBXML
                    sDefaultName = mcolZones.CurrentZone & "_" & "XML"
                    sDefaultSubFolder = String.Empty
                    oListBox = lstDBXML
                Case btnBackupToZipDBXML.Name
                    oBackupOutputType = eBackupOutputType.Zip
                    oBackupSourceType = eBackupSourceType.DBXML
                    sDefaultName = mcolZones.CurrentZone & "_" & "XML"
                    sDefaultSubFolder = "XML"
                    oListBox = lstDBXML
                Case btnBackupToFolderNotepad.Name
                    oBackupOutputType = eBackupOutputType.Folder
                    oBackupSourceType = eBackupSourceType.Notepad
                    sDefaultName = mcolZones.CurrentZone & "_" & "Notepad"
                    sDefaultSubFolder = String.Empty
                    oListBox = lstNotepad
                Case btnBackupToZipNotepad.Name
                    oBackupOutputType = eBackupOutputType.Zip
                    oBackupSourceType = eBackupSourceType.Notepad
                    sDefaultName = mcolZones.CurrentZone & "_" & "Notepad"
                    sDefaultSubFolder = "Notepad"
                    oListBox = lstNotepad
                Case btnBackupToFolderImage.Name
                    oBackupOutputType = eBackupOutputType.Folder
                    oBackupSourceType = eBackupSourceType.Image
                    sDefaultName = mcolZones.CurrentZone & "_" & "ECBR"
                    sDefaultSubFolder = String.Empty
                    oListBox = lstImage
                Case btnBackupToZipImage.Name
                    oBackupOutputType = eBackupOutputType.Zip
                    oBackupSourceType = eBackupSourceType.Image
                    sDefaultName = mcolZones.CurrentZone & "_" & "ECBR"
                    sDefaultSubFolder = "ECBR"
                    oListBox = lstImage
                Case btnBackupToFolderRobots.Name
                    oBackupOutputType = eBackupOutputType.Folder
                    oBackupSourceType = eBackupSourceType.Robot
                    sDefaultName = String.Empty
                    sDefaultSubFolder = String.Empty
                    oListBox = lstRobotFiles
                Case btnBackupToZipRobots.Name
                    oBackupOutputType = eBackupOutputType.Zip
                    oBackupSourceType = eBackupSourceType.Robot
                    sDefaultName = mcolZones.CurrentZone & "_" & gpsRM.GetString("psROBOTS_CAP", oCulture)
                    sDefaultSubFolder = String.Empty
                    oListBox = lstRobotFiles
                Case btnBackupToFolderApplication.Name
                    oBackupOutputType = eBackupOutputType.Folder
                    oBackupSourceType = eBackupSourceType.Application
                    sDefaultName = mcolZones.CurrentZone & "_" & gpsRM.GetString("psPAINTWORKSBACKUP", oCulture)
                    sDefaultSubFolder = String.Empty
                    oListBox = lstApplication
                Case btnBackupToZipsApplication.Name
                    oBackupOutputType = eBackupOutputType.Zip
                    oBackupSourceType = eBackupSourceType.Application
                    sDefaultName = mcolZones.CurrentZone & "_" & gpsRM.GetString("psPAINTWORKSBACKUP", oCulture)
                    sDefaultSubFolder = String.Empty
                    oListBox = lstApplication
                Case btnBackupToFolderDMON.Name
                    oBackupOutputType = eBackupOutputType.Folder
                    oBackupSourceType = eBackupSourceType.DMON
                    sDefaultName = mcolZones.CurrentZone & "_" & "DMON_Data"
                    sDefaultSubFolder = "DMON Data"
                    oListBox = lstDMON
                Case btnBackupToZipDMON.Name
                    oBackupOutputType = eBackupOutputType.Zip
                    oBackupSourceType = eBackupSourceType.DMON
                    sDefaultName = mcolZones.CurrentZone & "_" & "DMON_Data"
                    sDefaultSubFolder = "DMON Data"
                    oListBox = lstDMON
                Case btnBackupToFolderDMONArchive.Name
                    oBackupOutputType = eBackupOutputType.Folder
                    oBackupSourceType = eBackupSourceType.DMON
                    sDefaultName = mcolZones.CurrentZone & "_" & "DMON_Data_Archive"
                    sDefaultSubFolder = "DMON Data Archive"
                    oListBox = lstDMONArchive
                Case btnBackupToZipDMONArchive.Name
                    oBackupOutputType = eBackupOutputType.Zip
                    oBackupSourceType = eBackupSourceType.DMONArchive
                    sDefaultName = mcolZones.CurrentZone & "_" & "DMON_Data_Archive"
                    sDefaultSubFolder = "DMON Data Archive"
                    oListBox = lstDMONArchive
            End Select
            'Check list box.  Offer to select all if nothing is selected
            If oListBox IsNot Nothing Then
                Dim bCheckSelectAll As Boolean = (oListBox.SelectedItems.Count = 0)
                If oBackupSourceType = eBackupSourceType.Robot Then
                    bCheckSelectAll = bCheckSelectAll And rdoSourceSelectFiles.Checked
                End If
                If bCheckSelectAll Then
                    'Nothing selected, offer to select all
                    Dim sMsg As String = gpsRM.GetString("psNO_FILES_SELECTED", oCulture)
                    Dim sCaption As String = gpsRM.GetString("psNO_FILES_SELECTED_CAPTION", oCulture)
                    Dim oVal As DialogResult = MessageBox.Show(sMsg, sCaption, MessageBoxButtons.YesNo, _
                                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, _
                                    MessageBoxOptions.DefaultDesktopOnly)
                    Select Case oVal
                        Case Windows.Forms.DialogResult.Yes
                            For nIndex As Integer = 0 To oListBox.Items.Count - 1
                                Dim bDone As Boolean = False
                                For Each oItem As Object In oListBox.SelectedItems
                                    If oListBox.Items(nIndex) Is oItem Then
                                        bDone = True
                                        Exit For
                                    End If
                                Next
                                If bDone = False Then
                                    oListBox.SelectedItems.Add(oListBox.Items(nIndex))
                                End If
                            Next
                        Case Windows.Forms.DialogResult.No
                            Exit Sub
                    End Select
                End If
            End If
            Application.DoEvents()

            'Robots are a special case
            If oBackupSourceType = eBackupSourceType.Robot Then
                Dim bPerRobot As Boolean = False
                Dim bPerFolder As Boolean = False
                bAll = rdoSourceAllFiles.Checked
                If oBackupOutputType = eBackupOutputType.Zip Then
                    If mnSelectedRobotSelectionCount = 1 Then
                        bPerRobot = False
                    Else

                        '1 zip or zip per robot
                        'more complicated choices
                        Dim sMsg As String = gpsRM.GetString("psZIP_PER_ROBOT", oCulture)
                        Dim sCaption As String = gpsRM.GetString("psZIP_PER_ROBOT_CAPTION", oCulture)
                        Dim oVal As DialogResult = MessageBox.Show(sMsg, sCaption, MessageBoxButtons.YesNoCancel, _
                                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, _
                                        MessageBoxOptions.DefaultDesktopOnly)
                        bPerRobot = (oVal = Windows.Forms.DialogResult.Yes)

                    End If
                    If mnSelectedRobotFolderCount = 1 Then
                        bPerFolder = False
                    Else
                        '1 zip or zip per robot
                        'more complicated choices
                        Dim sMsg As String = gpsRM.GetString("psZIP_PER_FOLDER", oCulture)
                        Dim sCaption As String = gpsRM.GetString("psZIP_PER_FOLDER_CAPTION", oCulture)
                        Dim oVal As DialogResult = MessageBox.Show(sMsg, sCaption, MessageBoxButtons.YesNoCancel, _
                                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, _
                                        MessageBoxOptions.DefaultDesktopOnly)
                        bPerFolder = (oVal = Windows.Forms.DialogResult.Yes)

                    End If
                    If bPerFolder Or bPerRobot Then
                        'Get a folder and use default names
                        sBackupPath = sGetFolderName(msDefaultFolder, sDefaultSubFolder)
                        sDefaultSubFolder = String.Empty
                    Else
                        'All in one zip, get a name
                        sBackupPath = sGetFileName(sDefaultName, sDefaultSubFolder)
                    End If
                Else
                    sBackupPath = sGetFolderName(msDefaultFolder, sDefaultSubFolder)
                    sDefaultSubFolder = String.Empty
                End If
                Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
                Dim bYesToAll As Boolean = False
                Dim bCancel As Boolean = False
                Dim bGotPermission As Boolean = False
                Dim bReplace As Boolean = False
                If sBackupPath <> String.Empty Then
                    For Each oNode As Windows.Forms.TreeNode In trvFolders.Nodes
                        If oNode.Nodes.Count > 0 Then
                            'no need to get recursive here, there's only one more layer.
                            For Each oNode2 As Windows.Forms.TreeNode In oNode.Nodes
                                If oNode2.Checked Then
                                    subBackupRobotNode(oBackupOutputType, oBackupSourceType, sBackupPath, oNode2, bPerFolder, bPerRobot, bAll, bYesToAll, bCancel, bGotPermission, bReplace)
                                    If bCancel Then
                                        Exit For
                                    End If
                                End If
                            Next
                        Else
                            If oNode.Checked Then
                                subBackupRobotNode(oBackupOutputType, oBackupSourceType, sBackupPath, oNode, bPerFolder, bPerRobot, bAll, bYesToAll, bCancel, bGotPermission, bReplace)
                            End If
                        End If
                        If bCancel Then
                            Exit For
                        End If
                    Next
                End If
            ElseIf oBackupSourceType = eBackupSourceType.Application Then
                subDoApplicationBackup((oBackupOutputType = eBackupOutputType.Zip), sBackupPath, sDefaultName)
            Else
                'Not a robot backup
                'DB Backup
                Select Case oBackupOutputType
                    Case eBackupOutputType.Folder
                        sBackupPath = sGetFolderName(msDefaultFolder, sDefaultSubFolder)
                        sDefaultSubFolder = String.Empty
                    Case eBackupOutputType.Zip, eBackupOutputType.CSVZIP
                        sBackupPath = sGetFileName(sDefaultName, sDefaultSubFolder)
                    Case eBackupOutputType.CSV
                        If oListBox.SelectedItems.Count = 1 Then
                            Dim sName As String = oListBox.SelectedItems(0).ToString
                            sDefaultName = sName.Substring(0, sName.Length - 4)
                            sBackupPath = sGetFileName(sDefaultName, sDefaultSubFolder, False)
                            sDefaultSubFolder = String.Empty
                        Else
                            sBackupPath = sGetFolderName(msDefaultFolder, sDefaultSubFolder)
                            sDefaultSubFolder = String.Empty
                        End If
                End Select
                Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
                If sBackupPath <> String.Empty Then
                    subDoBackup(oBackupOutputType, oBackupSourceType, sBackupPath, sDefaultSubFolder, bAll)
                End If
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoBackup", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        Finally
            subEnableControls(True)
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub mnuBackupAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuBackupAllDBToFolder.Click, _
    mnuBackupAllDBToZip.Click
        '********************************************************************************************
        'Description:  Run a backup from one of the utility dropdown menus
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oMnu As System.Windows.Forms.ToolStripMenuItem = DirectCast(sender, System.Windows.Forms.ToolStripMenuItem)
        Dim oBackupSourceType As eBackupSourceType = eBackupSourceType.None
        Dim oBackupOutputType As eBackupOutputType = eBackupOutputType.None
        Dim sBackupPath As String = String.Empty
        Dim sDefaultName As String = String.Empty
        Dim sDefaultSubFolder As String = String.Empty
        Dim oListBox As ListBox = Nothing
        Dim bAll As Boolean = False
        Try
            subEnableControls(False)
            Select Case oMnu.Name
                Case mnuBackupAllDBToFolder.Name
                    oBackupOutputType = eBackupOutputType.Folder
                    sDefaultName = String.Empty
                    sDefaultSubFolder = ""
                Case mnuBackupAllDBToZip.Name
                    oBackupOutputType = eBackupOutputType.Zip
                    oBackupSourceType = eBackupSourceType.DBSQL
                    sDefaultName = mcolZones.CurrentZone & "_" & "Database"
            End Select
            Select Case oBackupOutputType
                Case eBackupOutputType.Folder
                    sBackupPath = sGetFolderName(msDefaultFolder, sDefaultSubFolder)
                    sDefaultSubFolder = String.Empty
                Case eBackupOutputType.Zip
                    sBackupPath = sGetFileName(sDefaultName, sDefaultSubFolder)
            End Select
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            Dim bYesToAll As Boolean = False
            Dim bCancel As Boolean = False
            oBackupSourceType = eBackupSourceType.DBSQL
            sDefaultSubFolder = String.Empty
            Dim sTmpFolderName As String = String.Empty
            oListBox = lstDBSQL
            If sBackupPath <> String.Empty Then
                Dim sTmp As String = sBackupPath
                subDoBackup(oBackupOutputType, oBackupSourceType, sTmp, sDefaultSubFolder, True, bYesToAll, bCancel)
                If bCancel = False Then
                    oBackupSourceType = eBackupSourceType.DBAccess
                    sDefaultSubFolder = String.Empty
                    oListBox = lstDBAccess
                    sTmp = sBackupPath
                    subDoBackup(oBackupOutputType, oBackupSourceType, sTmp, sDefaultSubFolder, True, bYesToAll, bCancel)
                    If bCancel = False Then
                        oBackupSourceType = eBackupSourceType.DBXML
                        sDefaultSubFolder = "XML"
                        oListBox = lstDBXML
                        sTmp = sBackupPath
                        'Select Case oBackupOutputType
                        '    Case eBackupOutputType.Folder
                        '        sTmpFolderName = sTmp & "\" & sDefaultSubFolder
                        '        If Not (IO.Directory.Exists(sTmpFolderName)) Then
                        '            IO.Directory.CreateDirectory(sTmpFolderName)
                        '        End If
                        '        subDoBackup(oBackupOutputType, oBackupSourceType, sTmpFolderName, String.Empty, True, bYesToAll, bCancel)
                        '    Case eBackupOutputType.Zip
                        subDoBackup(oBackupOutputType, oBackupSourceType, sTmp, sDefaultSubFolder, True, bYesToAll, bCancel)
                        'End Select
                        If bCancel = False Then
                            oBackupSourceType = eBackupSourceType.Image
                            sDefaultSubFolder = "ECBR"
                            sTmp = sBackupPath
                            'Select Case oBackupOutputType
                            '    Case eBackupOutputType.Folder
                            '        sTmpFolderName = sTmp & "\" & sDefaultSubFolder
                            '        If Not (IO.Directory.Exists(sTmpFolderName)) Then
                            '            IO.Directory.CreateDirectory(sTmpFolderName)
                            '        End If
                            '        oListBox = lstImage
                            '        subDoBackup(oBackupOutputType, oBackupSourceType, sTmpFolderName, String.Empty, True, bYesToAll, bCancel)
                            '    Case eBackupOutputType.Zip
                            subDoBackup(oBackupOutputType, oBackupSourceType, sTmp, sDefaultSubFolder, True, bYesToAll, bCancel)
                            'End Select
                            If bCancel = False Then
                                oBackupSourceType = eBackupSourceType.Notepad
                                sDefaultSubFolder = "Notepad"
                                oListBox = lstNotepad
                                sTmp = sBackupPath
                                subDoBackup(oBackupOutputType, oBackupSourceType, sTmp, sDefaultSubFolder, True, bYesToAll, bCancel)
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuBackupAll_Click", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        Finally
            subEnableControls(True)
            Me.Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub lstRobotFiles_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lstRobotFiles.MouseDown
        '********************************************************************************************
        'Description:  For robot tab, keep track of which list called the pop-up menu
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        msCurList = lstRobotFiles.Name

    End Sub
    Private Sub subProcessRobotFileBox()
        '********************************************************************************************
        'Description:  Fill or clear the robot file box
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        lstRobotFiles.Items.Clear()
        Dim sPath As String = String.Empty
        If rdoSourceSelectFiles.Checked Then
            For Each oNode As Windows.Forms.TreeNode In trvFolders.Nodes
                If oNode.Nodes.Count > 0 Then
                    'no need to get recursive here, there's only one more layer.
                    For Each oNode2 As Windows.Forms.TreeNode In oNode.Nodes
                        If oNode2.Checked Then
                            sPath = oNode2.Tag.ToString
                            Exit For
                        End If
                    Next
                    If sPath <> String.Empty Then
                        Exit For
                    End If
                Else
                    If oNode.Checked Then
                        sPath = oNode.Tag.ToString
                        Exit For
                    End If
                End If
            Next
            If sPath <> String.Empty Then
                If mnSelectedRobotSelectionCount > 0 Then
                    sPath = sPath & chkRobots.CheckedItems.Item(0).ToString
                End If
            End If
            Dim sMask As String = cboSourceFileType.Text.ToLower
            sMask = Replace(sMask, ",", " ")
            sMask = Replace(sMask, ";", " ")
            Dim sMaskList() As String = Split(sMask, " ")
            Dim sFiles() As String = Nothing
            For Each sTmp As String In sMaskList
                If (InStr(sTmp, "*") > 0) Or (InStr(sTmp, ".") > 0) Then
                    'Copy all
                    sFiles = IO.Directory.GetFiles(sPath, sTmp)
                    Dim nStartFileName As Integer = sPath.Length + 1
                    Dim sFile As String
                    For Each sFileLong As String In sFiles
                        Application.DoEvents()
                        sFile = sFileLong.Substring(nStartFileName)
                        lstRobotFiles.Items.Add(sFile)
                    Next
                End If
            Next

        End If
    End Sub

    Private Sub chkRobots_ItemCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles chkRobots.ItemCheck
        '********************************************************************************************
        'Description:  Robot checklist selection changed
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/08/13  MSW     Manage robot selected count better
        '********************************************************************************************
        If mbEventBlocker Then
            Exit Sub
        End If
        Try
            Application.DoEvents()
            'This event happens befopre the selected list gets updated, figure it out
            mnSelectedRobotSelectionCount = chkRobots.CheckedItems.Count
            If chkRobots.GetItemChecked(e.Index) Then
                mnSelectedRobotSelectionCount -= 1
            Else
                mnSelectedRobotSelectionCount += 1
            End If
            Debug.Print(chkRobots.CheckedItems.Count.ToString & " - " & mnSelectedRobotSelectionCount)
            If rdoSourceSelectFiles.Checked Then
                If (mnSelectedRobotSelectionCount <> 1) Or (mnSelectedRobotFolderCount <> 1) Then
                    mbEventBlocker = True
                    rdoSourceSelectFiles.Checked = False
                    rdoSourceAllFiles.Checked = True
                End If
                subProcessRobotFileBox()
            End If
            subEnableControls(True)
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuSelectAll_Click", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        Finally
            mbEventBlocker = False
            subEnableControls(True)
        End Try

    End Sub
    Private Sub chkRobots_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles chkRobots.MouseDown
        '********************************************************************************************
        'Description:  For robot tab, keep track of which list called the pop-up menu
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        msCurList = chkRobots.Name
    End Sub

    Private Sub subProcessRobotFolderTree(Optional ByRef oChangedNode As Windows.Forms.TreeNode = Nothing)
        '********************************************************************************************
        'Description:  Process robot folder selections
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************        
        Try
            mnSelectedRobotFolderCount = 0
            If oChangedNode IsNot Nothing Then
                If oChangedNode.Nodes.Count > 0 Then
                    'If there are child nodes, pass the check selection along
                    For Each oNode2 As Windows.Forms.TreeNode In oChangedNode.Nodes
                        oNode2.Checked = oChangedNode.Checked
                    Next
                End If
                If oChangedNode.Parent IsNot Nothing Then
                    Dim bCheck As Boolean = False
                    For Each oNode2 As Windows.Forms.TreeNode In oChangedNode.Parent.Nodes
                        If oNode2.Checked Then
                            bCheck = True
                            Exit For
                        End If
                    Next
                    oChangedNode.Parent.Checked = bCheck
                End If
            End If
            For Each oNode As Windows.Forms.TreeNode In trvFolders.Nodes
                If oNode.Nodes.Count > 0 Then
                    'no need to get recursive here, there's only one more layer.
                    For Each oNode2 As Windows.Forms.TreeNode In oNode.Nodes
                        If oNode2.Checked Then
                            mnSelectedRobotFolderCount = mnSelectedRobotFolderCount + 1
                        End If
                    Next
                Else
                    If oNode.Checked Then
                        mnSelectedRobotFolderCount = mnSelectedRobotFolderCount + 1
                    End If
                End If
            Next
            If rdoSourceSelectFiles.Checked Then
                If (mnSelectedRobotSelectionCount <> 1) Or (mnSelectedRobotFolderCount <> 1) Then
                    mbEventBlocker = True
                    rdoSourceSelectFiles.Checked = False
                    rdoSourceAllFiles.Checked = True
                    mbEventBlocker = False
                End If
                subProcessRobotFileBox()
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subProcessRobotFolderTree", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                    Status, MessageBoxButtons.OK)
        Finally
            mbEventBlocker = False
            subEnableControls(True)
        End Try
    End Sub

    Private Sub trvFolders_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles trvFolders.LostFocus
        '********************************************************************************************
        'Description:  Robot folder checklist lost focus
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************        
        Application.DoEvents()
        subProcessRobotFolderTree()
    End Sub

    Private Sub trvFolders_NodeMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles trvFolders.NodeMouseClick
        '********************************************************************************************
        'Description:  Robot folder checklist click
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************     

        subProcessRobotFolderTree(e.Node)
    End Sub
    Private Sub trvFolders_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles trvFolders.KeyPress
        '********************************************************************************************
        'Description:  Robot folder checklist keypress
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************        
        Application.DoEvents()
        subProcessRobotFolderTree(trvFolders.SelectedNode)
    End Sub


    Private Sub trvFolders_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles trvFolders.MouseDown
        '********************************************************************************************
        'Description:  For robot tab, keep track of which list called the pop-up menu
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        msCurList = trvFolders.Name
    End Sub

    Private Sub rdoSource_Files_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoSourceSelectFiles.CheckedChanged, _
     rdoSourceAllFiles.Click
        '********************************************************************************************
        'Description:  select files or all files list box
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEventBlocker Then
            Exit Sub
        End If
        subProcessRobotFileBox()
        subEnableControls(True)
    End Sub

    Private Sub cboSourceFileType_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cboSourceFileType.KeyPress
        '********************************************************************************************
        'Description:  file mask changed
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEventBlocker Then
            Exit Sub
        End If
        If e.KeyChar = Microsoft.VisualBasic.ChrW(Keys.Return) Then
            subProcessRobotFileBox()
            subEnableControls(True)
            msMask = cboSourceFileType.Text
        End If
    End Sub

    Private Sub cboSourceFileType_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboSourceFileType.LostFocus
        '********************************************************************************************
        'Description:  file mask changed
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEventBlocker Then
            Exit Sub
        End If
        If msMask <> cboSourceFileType.Text Then
            subProcessRobotFileBox()
            subEnableControls(True)
            msMask = cboSourceFileType.Text
        End If
    End Sub

    Private Sub cboSourceFileType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboSourceFileType.SelectedIndexChanged
        '********************************************************************************************
        'Description:  file mask changed
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEventBlocker Then
            Exit Sub
        End If
        If msMask <> cboSourceFileType.Text Then
            subProcessRobotFileBox()
            subEnableControls(True)
            msMask = cboSourceFileType.Text
        End If
    End Sub

    Private Sub lstBox_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles lstDBSQL.KeyPress, _
                lstDBAccess.KeyPress, lstDBXML.KeyPress, lstNotepad.KeyPress, lstImage.KeyPress, lstRobotFiles.KeyPress, lstApplication.KeyPress, _
                lstDMON.KeyPress, lstDMONArchive.KeyPress

        '********************************************************************************************
        'Description:  Keypresses for listboxes - use ctrl-a
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If Asc(e.KeyChar) = 1 Then 'There has to be a constant for this somewhere
            subDoSelectAllToggle()
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


    Private Sub subDoSelectAllToggle()
        Dim oList As ListBox = Nothing
        If moCurList IsNot Nothing Then
            oList = moCurList
        Else
            Select Case msCurList
                Case chkRobots.Name
                    mbEventBlocker = True
                    Dim oChkState As CheckState
                    If chkRobots.SelectedItems.Count = chkRobots.Items.Count Then
                        oChkState = CheckState.Unchecked
                    Else
                        oChkState = CheckState.Checked
                    End If
                    For nIndex As Integer = 0 To chkRobots.Items.Count - 1
                        chkRobots.SetItemCheckState(nIndex, oChkState)
                    Next
                    mbEventBlocker = False
                Case trvFolders.Name
                    mbEventBlocker = True
                    mnSelectedRobotFolderCount = 0
                    Dim bCheckAll As Boolean = False
                    For Each oNode As Windows.Forms.TreeNode In trvFolders.Nodes
                        If oNode.Checked <> True Then
                            bCheckAll = True
                            Exit For
                        End If
                        If oNode.Nodes.Count > 0 Then
                            'no need to get recursive here, there's only one more layer.
                            For Each oNode2 As Windows.Forms.TreeNode In oNode.Nodes
                                If oNode2.Checked <> True Then
                                    bCheckAll = True
                                    Exit For
                                End If
                            Next
                        End If
                        If bCheckAll Then
                            Exit For
                        End If
                    Next
                    mnSelectedRobotFolderCount = 0
                    For Each oNode As Windows.Forms.TreeNode In trvFolders.Nodes
                        oNode.Checked = bCheckAll
                        If oNode.Nodes.Count > 0 Then
                            'no need to get recursive here, there's only one more layer.
                            For Each oNode2 As Windows.Forms.TreeNode In oNode.Nodes
                                oNode2.Checked = bCheckAll
                                If bCheckAll Then
                                    mnSelectedRobotFolderCount = mnSelectedRobotFolderCount + 1
                                End If
                            Next
                        Else
                            If bCheckAll Then
                                mnSelectedRobotFolderCount = mnSelectedRobotFolderCount + 1
                            End If
                        End If
                    Next
                    mbEventBlocker = False
                Case lstRobotFiles.Name
                    oList = lstRobotFiles
            End Select
        End If
        If oList IsNot Nothing Then
            Dim bSelect As Boolean = True
            If oList.SelectedItems.Count = oList.Items.Count Then
                bSelect = False
            Else
                bSelect = True
            End If
            'select all
            For nIndex As Integer = 0 To oList.Items.Count - 1
                oList.SetSelected(nIndex, bSelect)
            Next
            oList.Refresh()
        End If
    End Sub


End Class