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
' Description: Notepad
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
'    11/12/09   MSW     manage the DataLoaded property so delete clears out the rtb and 
'                       gets disabled.  Also load a selection on startup
'    11/16/09   MSW     subEnableControls, bLoadList - handle enables, clear list if all docs 
'                       deleted - first attempt didn't quite handle everything.
'    09/14/11   MSW     Assemble a standard version of everything                      4.01.00.00
'    12/02/11   MSW     Add Dmoncfg reference                                          4.01.01.00
'    01/18/12   MSW     Clean up old printsettings object                              4.01.01.01
'    01/24/12   MSW     Placeholder include updates                                    4.01.01.02
'    02/15/12   MSW     Force 32 bit build, print management udpates                   4.01.01.03
'    03/23/12   RJO     modified for .NET Password and IPC                             4.01.02.00
'    03/28/12   MSW     get rid of SQL DB for notepad.  Use an XML list with rtf files 4.01.03.00
'    10/24/12   RJO     Added StartupModule to project to prevent multiple instances   4.01.03.01
'    12/13/12   MSW     subSaveData - Clear modified status after save                 4.01.04.00
'    04/16/13   MSW     Add Canadian language files                                    4.01.05.00
'    07/09/13   MSW     Update and standardize logos                                   4.01.05.01
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up 4.01.05.02
'    09/12/13   MSW     Cleanup unused robot code                                      4.01.05.03
'    09/30/13   MSW     Save screenshots as jpegs                                      4.01.06.00
'    01/07/14   MSW     Remove ControlBox from main form                               4.01.06.01
'    02/03/14   RJO     Fix to subInitFormText to set tlbProp.ToolTipText. Added      4.01.06.02
'                       "psPROPERTIES" tag to .resx files.
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call               4.01.07.00
'    04/21/14   MSW     Move some tool bar setup to common files                       4.01.07.01
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Configuration.ConfigurationSettings
Imports Response = System.Windows.Forms.DialogResult
Imports System.Xml
Imports System.Xml.XPath

Friend Class frmMain
#Region " Declares "

    '******** Form Constants   **********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    'Not a constant here, we'll try to get help in the same executable
    Friend Const msSCREEN_NAME As String = "Notepad"   ' <-- For password area change log etc.
    Private Const msSCREEN_DUMP_NAME As String = "Utility_Notepad_Main.jpg"
    Private Const mnMAININX As Integer = 100
    Private Const mnSCREENINX As Integer = 101
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    Friend Const gsNOTEPAD_DATABASENAME As String = "Notepad"
    Friend Const gsNOTEPAD_DS_TABLENAME As String = "Notepad"
    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    Friend Shared gsChangeLogArea As String = msSCREEN_NAME
    Private msOldZone As String = String.Empty
    Private colZones As clsZones = Nothing
    Private mbEventBlocker As Boolean = False
    Private mbRemoteZone As Boolean = False
    Private mbScreenLoaded As Boolean = False
    Private msLastPage As String = String.Empty
    Private Const msHOSTS_LOCATION As String = "%SYSTEMROOT%\system32\drivers\etc\hosts"

    Private msHostFileName As String = String.Empty
    Private Const msLAUNCH_COMMAND As String = "cmd.exe /A  /C "
    Private Const msOUTPUT_FILE As String = "NWMNTout"
    Private Const msTXT_EXT As String = ".TXT"
    Private Const msCMD_PING As String = "ping "
    Private Const msCMD_ARP_ALL As String = "arp -a"
    Private Const msCMD_ARP_ONE As String = "arp "
    Private Const msCMD_TRACEROUTE As String = "tracert "
    Private Const msCMD_IPCONFIG As String = "ipconfig /all"
    Private mbCancel As Boolean = False
    Private nTmpFileIndex As Integer = 0 'In case there are access problems, add to the file name
    Private mnSplitterDistance As Integer
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
    Private mnMaxProgress As Integer = 100
    Private mPrintHtml As clsPrintHtml
    'Private dtList As DataTable = Nothing
    'Private dtDoc As DataTable = Nothing
    Private mnMaxID As Integer = 0
    Private mnSelectedRow As Integer = 0
    Private mbDontUpdate As Boolean = False
    Private mbEditsMade As Boolean = False
    Private mnFindNextIndex As Integer = -1
    Private mbSearchBack As Boolean = False
    Private mbInAskForSave As Boolean = False
    Private mcolNotes As New Collection
    Private moNote As Note
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
        ' 11/12/09  MSW     clear the list selection so the next click in the list loads a file
        '********************************************************************************************
        Get
            Return mbDataLoaded
        End Get
        Set(ByVal Value As Boolean)
            mbDataLoaded = Value
            If mbDataLoaded Then
                'just finished loading reset & refresh
                subShowNewPage()
            Else
                rtbDoc.Clear()
                dgvList.ClearSelection() ' make sure the next click in the list loads a file
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
        ' 11/16/09  MSW     subEnableControls, bLoadList - handle enables, clear list if all docs deleted
        ' 10/07/10  MSW	    Print function clean up
        '********************************************************************************************
        Dim bRestOfControls As Boolean = False
        Dim mbEditsMade As Boolean = rtbDoc.Modified Or mbEditsMade
        btnClose.Enabled = True
        Dim bDataLoaded As Boolean = DataLoaded
        If bEnable = False Then
            tlsDoc.Enabled = False
            btnChangeLog.Enabled = False
            bRestOfControls = False
        Else
            Select Case Privilege
                Case ePrivilege.None
                    tlsDoc.Enabled = True
                    btnChangeLog.Enabled = True
                    bRestOfControls = False
                    tlsClear.Enabled = False
                    tlbClear.Enabled = False
                    tlbNew.Enabled = False
                    tlsNew.Enabled = False
                    btnNew.Enabled = False
                    tlbOpen.Enabled = False
                    rtbDoc.ReadOnly = True
                    mnuPrintFile.Enabled = False
                    tlmPrintFile.Enabled = False
                Case Else
                    tlsDoc.Enabled = True
                    btnChangeLog.Enabled = True
                    bRestOfControls = True
                    tlsClear.Enabled = bDataLoaded
                    tlbClear.Enabled = bDataLoaded
                    tlbNew.Enabled = True
                    tlsNew.Enabled = True
                    btnNew.Enabled = True
                    tlbOpen.Enabled = True
                    rtbDoc.ReadOnly = False
                    mnuPrintFile.Enabled = True
                    tlmPrintFile.Enabled = True
            End Select
        End If
        rtbDoc.Enabled = DataLoaded
        tlbSave.Enabled = bRestOfControls
        btnSave.Enabled = bRestOfControls
        mnuSaveDB.Enabled = bRestOfControls And mbEditsMade
        mnuSaveDBDoc.Enabled = bRestOfControls And mbEditsMade
        mnuSaveFileTXT.Enabled = bRestOfControls
        mnuSaveFileTXTDoc.Enabled = bRestOfControls
        mnuSaveFileRTF.Enabled = bRestOfControls
        mnuSaveFileRTFDoc.Enabled = bRestOfControls

        'Enable cut copy when there's an active selection
        Dim bCmdSel As Boolean = rtbDoc.SelectionLength > 0
        tlbCut.Enabled = bRestOfControls And bCmdSel
        tlbCopy.Enabled = bRestOfControls And bCmdSel
        'No event for this, so it'll just stay enabled
        'Dim bPaste As Boolean = Clipboard.ContainsText
        tlbPaste.Enabled = bRestOfControls 'And bPaste
        tlbAlignLeft.Enabled = bRestOfControls
        tlbAlignRight.Enabled = bRestOfControls
        tlbAlignCenter.Enabled = bRestOfControls
        tlbBold.Enabled = bRestOfControls
        tlbItalic.Enabled = bRestOfControls
        tlbUnderline.Enabled = bRestOfControls

        'No password, just the passed in enable
        tlbFilter.Enabled = bEnable
        tlbFindDoc.Enabled = bEnable
        tlsbFindList.Enabled = bEnable
        tlbPrev.Enabled = bEnable
        tlbNext.Enabled = bEnable
        btnPrevious.Enabled = bEnable
        btnNext.Enabled = bEnable
        tlbPrint.Enabled = bEnable
        btnPrint.Enabled = bEnable
        btnShow.Enabled = bEnable
        btnHide.Enabled = bEnable

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
        ' 02/03/14  RJO     Code to set tlsbProp.ToolTipText was missing
        '********************************************************************************************

        With gpsRM
            tlbSave.ToolTipText = .GetString("psSAVE_DOCUMENT")
            tlbPrint.ToolTipText = .GetString("psPRINT_DOCUMENT")
            tlbNew.ToolTipText = .GetString("psNEW_DOCUMENT")
            tlsNew.ToolTipText = .GetString("psNEW_DOCUMENT")
            btnNew.ToolTipText = .GetString("psNEW_DOCUMENT")
            tlbOpen.ToolTipText = .GetString("psOPEN_DOCUMENT")
            tlbClear.ToolTipText = .GetString("psDELETE_DOCUMENT")
            tlsClear.ToolTipText = .GetString("psDELETE_DOCUMENT")
            tlbProp.ToolTipText = .GetString("psPROPERTIES") ' RJO 02/03/14
            tlbCut.ToolTipText = .GetString("psCUT_TO_CLIPBOARD")
            tlbCopy.ToolTipText = .GetString("psCOPY_TO_CLIPBOARD")
            tlbPaste.ToolTipText = .GetString("psPASTE_FROM_CLIPBOARD")
            tlbAlignCenter.ToolTipText = .GetString("psALIGN_CENTER")
            tlbAlignLeft.ToolTipText = .GetString("psALIGN_LEFT")
            tlbAlignRight.ToolTipText = .GetString("psALIGN_RIGHT")
            tlbBold.ToolTipText = .GetString("psBOLD")
            tlbItalic.ToolTipText = .GetString("psITALIC")
            tlbUnderline.ToolTipText = .GetString("psUNDERLINE")
            tlbFilter.Text = .GetString("psFILTER")
            tlbFilter.ToolTipText = .GetString("psFILTER_LIST")
            tlbFindDoc.ToolTipText = .GetString("psFIND_TOOLTIP")
            tlsbFindList.ToolTipText = .GetString("psFIND_TOOLTIP")
            tlbPrev.ToolTipText = .GetString("psPREV_DOC")
            tlbNext.ToolTipText = .GetString("psNEXT_DOC")
            btnPrevious.ToolTipText = .GetString("psPREV_DOC")
            btnNext.ToolTipText = .GetString("psNEXT_DOC")
            'btnPrevious.Text = .GetString("psPREV")
            'btnNext.Text = .GetString("psNEXT")
            btnShow.Text = .GetString("psSHOW")
            btnHide.Text = .GetString("psHIDE")
            btnShow.ToolTipText = .GetString("psSHOW_LIST")
            btnHide.ToolTipText = .GetString("psHIDE_LIST")
            mnuSaveDBDoc.Text = .GetString("psSAVE_DOC_TO") & .GetString("psDB")
            mnuSaveFileRTFDoc.Text = .GetString("psSAVE_DOC_TO") & .GetString("psRTF_FILR")
            mnuSaveFileTXTDoc.Text = .GetString("psSAVE_DOC_TO") & .GetString("psTXT_FILE")
            mnuSaveDB.Text = .GetString("psSAVE_DOC_TO") & .GetString("psDB")
            mnuSaveFileRTF.Text = .GetString("psSAVE_DOC_TO") & .GetString("psRTF_FILR")
            mnuSaveFileTXT.Text = .GetString("psSAVE_DOC_TO") & .GetString("psTXT_FILE")
            lblTopic.Text = .GetString("psTOPIC") & ": "
            lblAuthor.Text = .GetString("psAUTHOR") & ": "
            lblCreated.Text = .GetString("psCREATED") & ": "
            lblModified.Text = .GetString("psMODIFIED") & ": "
            dgvList.Columns.Add("ID", gpsRM.GetString("psID_CAP"))
            dgvList.Columns.Add("TOPIC", gpsRM.GetString("psTOPIC_CAP"))
            dgvList.Columns.Add("NAME", gpsRM.GetString("psAUTHOR_CAP"))
            dgvList.Columns.Add("CREATED", gpsRM.GetString("psCREATED_CAP"))
            dgvList.Columns.Add("MODIFIED", gpsRM.GetString("psMODIFIED_CAP"))
        End With
        tlmPrint.Text = gcsRM.GetString("csPRINT")
        tlmPrintPreview.Text = gcsRM.GetString("csPRINT_PREVIEW")
        tlmPageSetup.Text = gcsRM.GetString("csPAGE_SETUP")
        tlmPrintFile.Text = gcsRM.GetString("csPRINT_FILE")
        rtbDoc.Modified = False
        mbEditsMade = False
        DataLoaded = False
        mnFindNextIndex = -1
    End Sub

    Private Function bDeleteCurrentDoc() As Boolean
        '********************************************************************************************
        'Description: Delete the current document from the DB
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            If moNote IsNot Nothing Then
                If IO.File.Exists(moNote.Path) Then
                    IO.File.Delete(moNote.Path)
                End If
                mcolNotes.Remove(moNote.ID.ToString)
                bSaveCollection()
            End If

            Dim sChangeLogText As String = gpsRM.GetString("psDELETE_DOC_CHGLOG") & _
                        """" & moNote.Topic & """"
            mPWCommon.AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                                      colZones.CurrentZoneNumber, _
                                      String.Empty, String.Empty, _
                                      sChangeLogText, colZones.CurrentZone)
            mPWCommon.SaveToChangeLog(colZones.ActiveZone)

            bLoadDGV((tlbFilter.Checked = False))
            DataLoaded = False
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)

            Return False
        End Try

    End Function
    Private Function nGetUnusedID(Optional ByVal nId As Integer = 1) As Integer
        Dim bOK As Boolean = False
        Do While bOK = False
            bOK = True
            For Each oNote As Note In mcolNotes
                If oNote.ID = nId Then
                    bOK = False
                    Exit For
                End If
            Next
            If bOK = False Then
                If nId = Integer.MaxValue Then
                    nId = Integer.MinValue
                Else
                    nId += 1
                End If
            End If
        Loop
        Return (nId)
    End Function
    Private Function bNewDocument(ByRef sTitle As String, ByRef sText As String) As Boolean
        '********************************************************************************************
        'Description: Add a new document to the DB and open it for edit
        '
        'Parameters: create a new document and 
        'Returns:    True if the list isn't empty
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim dtNow As DateTime = Now
        Try
            moNote = New Note
            moNote.Name = LoggedOnUser
            moNote.Topic = sTitle
            moNote.ID = nGetUnusedID()
            Dim sPath As String = String.Empty
            Dim sFileName As String = sMakeValidFileName(sTitle) & ".rtf"
            GetDefaultFilePath(sPath, eDir.Notepad, String.Empty, sFileName)
            moNote.Path = sPath
            moNote.Created = dtNow
            moNote.Modified = dtNow
            moNote.Text = sText
            rtbDoc.Text = sText
            mcolNotes.Add(moNote, moNote.ID.ToString)
            Dim sChangeLogText As String = gpsRM.GetString("psNEW_DOC_CHGLOG") & """" & sTitle & """"

            mPWCommon.AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                                      colZones.CurrentZoneNumber, _
                                      String.Empty, String.Empty, _
                                      sChangeLogText, colZones.CurrentZone)
            mPWCommon.SaveToChangeLog(colZones.ActiveZone)
            bSaveCollection()
            subSaveToFile(rtbDoc, True, moNote.Path)

            bLoadDGV(True, moNote.ID)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)

            Return False
        End Try

    End Function
    Private Function bReadDocumentFromDB(ByRef nID As Integer, ByRef oNote As Note) As Boolean
        '********************************************************************************************
        'Description: Read a document from the DB and open it for edit
        '
        'Parameters: ID
        'Returns:    True if the list isn't empty
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            For Each oNoteTmp As Note In mcolNotes
                If oNoteTmp.ID = nID Then
                    oNote = oNoteTmp
                    Exit For
                End If
            Next
            If oNote IsNot Nothing Then
                oNote.Text = IO.File.ReadAllText(oNote.Path)
            Else
                MessageBox.Show(gpsRM.GetString("psRECORD_NOT_FOUND"), msSCREEN_NAME, MessageBoxButtons.OK, _
                MessageBoxIcon.Exclamation)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)

            Return False
        End Try

    End Function
    Friend Function sConvertWildcards(ByRef sStart As String) As String
        '****************************************************************************************
        'Description: convert normal wildcards to the ones used by SQL server
        '
        'Parameters: source string with * and ? as wildcard characters
        'Returns:   string with % and _ as wildcards
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim sReturn As String = sStart.Replace("*", "%")
        Return (sReturn.Replace("?", "_"))
    End Function
    Private Function bLoadCollection() As Boolean
        '********************************************************************************************
        'Description: load list from XML
        '
        'Parameters: 
        'Returns:    True if the list isn't empty
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Load collection of note details from XML
        '********************************************************************************************

        Try
            Const sNOTEPAD_XMLTABLE As String = "Notepad"
            Const sNOTEPAD_XMLNODE As String = "Note"

            Dim sTopic As String = String.Empty
            Dim oMainNode As XmlNode = Nothing
            Dim oNode As XmlNode = Nothing
            Dim oNodeList As XmlNodeList = Nothing
            Dim oXMLDoc As New XmlDocument
            Dim sXMLFilePath As String = String.Empty
            Try
                If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sNOTEPAD_XMLTABLE & ".XML") Then


                    oXMLDoc.Load(sXMLFilePath)

                    oMainNode = oXMLDoc.SelectSingleNode("//" & sNOTEPAD_XMLTABLE)
                    oNodeList = oMainNode.SelectNodes("//" & sNOTEPAD_XMLNODE)
                    mcolNotes.Clear()
                    Try
                        If oNodeList.Count = 0 Then
                            mDebug.WriteEventToLog("frmMain:bLoadList", sXMLFilePath & " not found.")
                        Else
                            Dim nMax As Integer = oNodeList.Count - 1
                            For nNode As Integer = 0 To nMax
                                Try
                                    Dim oNote As New Note
                                    Try
                                        oNote.ID = CType(oNodeList(nNode).Item("ID").InnerXml, Integer)
                                    Catch ex As Exception
                                        oNote.ID = nGetUnusedID(1)
                                    End Try
                                    oNote.Name = oNodeList(nNode).Item("Name").InnerXml
                                    oNote.Topic = oNodeList(nNode).Item("Topic").InnerXml
                                    oNote.Path = oNodeList(nNode).Item("Path").InnerXml
                                    Try
                                        oNote.Created = CType(oNodeList(nNode).Item("Created").InnerXml, Date)
                                    Catch ex As Exception
                                        oNote.Created = Now
                                    End Try
                                    Try
                                        oNote.Modified = CType(oNodeList(nNode).Item("Modified").InnerXml, Date)
                                    Catch ex As Exception
                                        oNote.Modified = Now
                                    End Try
                                    oNote.Text = String.Empty

                                    mcolNotes.Add(oNote, oNote.ID.ToString)
                                Catch ex As Exception
                                    ShowErrorMessagebox(sXMLFilePath & "  : Invalid Data", ex, "frmMain:bLoadList", String.Empty)
                                End Try
                            Next
                        End If
                    Catch ex As Exception
                        mDebug.WriteEventToLog("frmMain:bLoadList", "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                    End Try
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog("frmMain:bLoadList", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
            End Try
            Return (mcolNotes.Count > 0)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            Return False
        End Try

    End Function
    Private Function bSaveCollection() As Boolean
        '********************************************************************************************
        'Description: save list to XML
        '
        'Parameters: 
        'Returns:    True if successful
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     save collection of note details to XML
        '********************************************************************************************

        Try
            Const sNOTEPAD_XMLTABLE As String = "Notepad"
            Const sNOTEPAD_XMLNODE As String = "Note"

            Dim sTopic As String = String.Empty
            Dim oMainNode As XmlNode = Nothing
            Dim oNode As XmlNode = Nothing
            Dim oNodeList As XmlNodeList = Nothing
            Dim oXMLDoc As New XmlDocument
            Dim sXMLFilePath As String = String.Empty
            Try
                If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sNOTEPAD_XMLTABLE & ".XML") Then


                    If (IO.File.Exists(sXMLFilePath) = False) Then
                        IO.File.Create(sXMLFilePath)
                        oXMLDoc = New XmlDocument
                        oXMLDoc.CreateElement(sNOTEPAD_XMLTABLE)
                        oMainNode = oXMLDoc.SelectSingleNode("//" & sNOTEPAD_XMLTABLE)
                    Else
                        Try
                            oXMLDoc.Load(sXMLFilePath)
                            oMainNode = oXMLDoc.SelectSingleNode("//" & sNOTEPAD_XMLTABLE)
                        Catch ex As Exception
                            oXMLDoc = New XmlDocument
                            oMainNode = oXMLDoc.CreateElement(sNOTEPAD_XMLTABLE)
                            oXMLDoc.AppendChild(oMainNode)
                            oMainNode = oXMLDoc.SelectSingleNode("//" & sNOTEPAD_XMLTABLE)
                        End Try
                    End If
                    oMainNode.RemoveAll()
                    For Each oNote As Note In mcolNotes
                        oNode = oXMLDoc.CreateElement(sNOTEPAD_XMLNODE)
                        Dim oIDNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "ID", Nothing)
                        Dim oNameNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Name", Nothing)
                        Dim oTopicNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Topic", Nothing)
                        Dim oPathNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Path", Nothing)
                        Dim oCreatedNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Created", Nothing)
                        Dim oModifiedNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Modified", Nothing)

                        oIDNode.InnerXml = oNote.ID.ToString
                        oNameNode.InnerXml = oNote.Name
                        oTopicNode.InnerXml = oNote.Topic
                        oPathNode.InnerXml = oNote.Path
                        oCreatedNode.InnerXml = oNote.Created.ToString
                        oModifiedNode.InnerXml = oNote.Modified.ToString

                        oNode.AppendChild(oIDNode)
                        oNode.AppendChild(oNameNode)
                        oNode.AppendChild(oTopicNode)
                        oNode.AppendChild(oPathNode)
                        oNode.AppendChild(oCreatedNode)
                        oNode.AppendChild(oModifiedNode)
                        oMainNode.AppendChild(oNode)
                    Next
                    oXMLDoc.Save(sXMLFilePath)
                    Return (True)
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog("bSaveCollection:bLoadList", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
            End Try
            Return (False)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            Return (False)
        End Try
        Return (False)
    End Function
    Private Function oFilterCollection(ByRef oColIn As Collection) As Collection
        Dim oColOut As Collection = New Collection
        For Each oNote As Note In oColIn
            Dim bAdd As Boolean = True
            Dim sTmp As String = frmFilter.Topic
            If sTmp <> gcsRM.GetString("csALL") And _
                sTmp <> String.Empty Then
                If frmFilter.EnableWildCards Then
                    bAdd = oNote.Topic Like sTmp
                Else
                    bAdd = oNote.Topic = sTmp
                End If
            End If
            sTmp = frmFilter.Author
            If sTmp <> gcsRM.GetString("csALL") And _
                sTmp <> String.Empty And bAdd Then
                If frmFilter.EnableWildCards Then
                    bAdd = oNote.Name Like sTmp
                Else
                    bAdd = oNote.Name = sTmp
                End If
            End If
            If frmFilter.CreatedStartEnable And bAdd Then
                bAdd = oNote.Created > frmFilter.CreatedStart
            End If
            If frmFilter.CreatedEndEnable And bAdd Then
                bAdd = oNote.Created < frmFilter.CreatedEnd
            End If
            If frmFilter.ModifiedStartEnable And bAdd Then
                bAdd = oNote.Created > frmFilter.ModifiedStart
            End If
            If frmFilter.ModifiedEndEnable And bAdd Then
                bAdd = oNote.Created < frmFilter.ModifiedEnd
            End If
            If bAdd Then
                oColOut.Add(oNote, oNote.ID.ToString)
            End If
        Next
        Return oColOut
    End Function
    Private Function bLoadDGV(Optional ByVal bAll As Boolean = False, Optional ByVal nIDSelect As Integer = -1) As Boolean
        '********************************************************************************************
        'Description: load list from DBinto DataGridView
        '
        'Parameters: reads filter parameters from frmFilter, bAll  = true gets everythying
        'Returns:    True if the list isn't empty
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     load collection into datagrid
        '********************************************************************************************

        Try
            'Load up the listview
            Dim oColNotes As Collection = Nothing
            If tlbFilter.Checked And (bAll = False) Then
                oColNotes = oFilterCollection(mcolNotes)
            Else
                oColNotes = mcolNotes
            End If
            If oColNotes.Count = 0 Then
                'No records
                ' "No records found"
                dgvList.RowCount = 0
                DataLoaded = False
                MessageBox.Show(gpsRM.GetString("psNO_RECORDS_FOUND"), msSCREEN_NAME, MessageBoxButtons.OK, _
                    MessageBoxIcon.Exclamation)
                tlbFilter.Checked = False
            Else
                mbDontUpdate = True

                dgvList.RowCount = oColNotes.Count
                For nItem As Integer = 0 To oColNotes.Count - 1
                    Dim oNote As Note = DirectCast(oColNotes.Item(nItem + 1), Note)
                    dgvList.Rows(nItem).Cells(0).Value = oNote.ID
                    dgvList.Rows(nItem).Cells(1).Value = oNote.Topic
                    dgvList.Rows(nItem).Cells(2).Value = oNote.Name
                    dgvList.Rows(nItem).Cells(3).Value = oNote.Created
                    dgvList.Rows(nItem).Cells(4).Value = oNote.Modified
                Next
                mbDontUpdate = False
                dgvList.Columns(gpsRM.GetString("psID_CAP")).Visible = False
                Dim dtTmpDate1 As New DateTime(1993, 1, 1, 0, 0, 0)
                Dim dtTmpDate2 As New DateTime
                Dim nRowSelect As Integer = 0
                If Not (mbInAskForSave) Then 'Select a new doc
                    If nIDSelect = -1 Then
                        'Select the newest row
                        For nRow As Integer = 0 To dgvList.RowCount - 1
                            dtTmpDate2 = CType(dgvList.Rows.Item(nRow).Cells.Item(gpsRM.GetString("psMODIFIED_CAP")).Value, DateTime)
                            If dtTmpDate2 > dtTmpDate1 Then
                                dtTmpDate1 = dtTmpDate2
                                nRowSelect = nRow
                            End If
                        Next
                        dgvList.Rows.Item(nRowSelect).Selected = True
                        dgvList_SelectionChanged()
                    Else
                        'Select based on ID
                        Dim nTmpFileIndex As Integer = 0
                        For nRow As Integer = 0 To dgvList.RowCount - 1
                            nTmpFileIndex = CInt(dgvList.Rows.Item(nRow).Cells.Item(gpsRM.GetString("psID_CAP")).Value)
                            If nIDSelect = nTmpFileIndex Then
                                dgvList.Rows.Item(nRow).Selected = True
                                dgvList_SelectionChanged()
                                Exit For
                            End If
                        Next
                    End If
                End If
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
        End Try
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
        '********************************************************************************************
        Dim lReply As Windows.Forms.DialogResult = Response.OK

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor


        Try

            Status = gcsRM.GetString("csINITALIZING")
            Progress = 1

            DataLoaded = False
            mbScreenLoaded = False

            subProcessCommandLine()

            'init the old password for now
            'moPassword = New PWPassword.PasswordObject 'RJO 03/23/12
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

            'init new IPC and new Password 'RJO 03/23/12
            oIPC = New Paintworks_IPC.clsInterProcessComm
            moPassword = New clsPWUser
            Progress = 50
            colZones = New clsZones(String.Empty)
            mScreenSetup.InitializeForm(Me)
            subInitFormText()
            mScreenSetup.subSetUpToolStrip(tlsDoc)
            mScreenSetup.subSetUpToolStrip(tlsList)


            mPrintHtml = New clsPrintHtml(msSCREEN_NAME)
            Progress = 70
            bLoadCollection()
            bLoadDGV(True)
            Progress = 98
            subEnableControls(True)

            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound


            'statusbar text
            Status(True) = gpsRM.GetString("psSELECT_DOC")

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subClearScreen()
            btnShow.Checked = True
            subEnableControls(False)

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
    Private Sub subSaveData(Optional ByVal bFile As Boolean = False, Optional ByVal bRTF As Boolean = True)
        '********************************************************************************************
        'Description:  Data Save Routine
        '
        'Parameters: rich text box object to save
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/13/12  MSW     subSaveData - Clear modified status after save
        '********************************************************************************************
        If bFile Then
            subSaveToFile(rtbDoc, bRTF)
        Else
            subSaveToFile(rtbDoc, bRTF, moNote.path)
            bSaveCollection()
            rtbDoc.Modified = False
        End If
        subEnableControls(True)
    End Sub
    Private Sub subSaveToFile(ByRef rtbTextBox As RichTextBox, Optional ByVal bRTF As Boolean = True, Optional ByRef sPath As String = "")
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

        Application.DoEvents()

        Select Case Privilege
            Case ePrivilege.None
                ' shouldnt be here
                subEnableControls(False)
                Exit Sub

            Case Else
                'ok
        End Select
        moNote.Text = rtbDoc.Rtf

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        Try
            If sPath = String.Empty Then
                Dim o As New SaveFileDialog
                With o
                    .Filter = gcsRM.GetString("csSAVE_TXT_RTF_ALL_FILTER")
                    .Title = gcsRM.GetString("csSAVE_FILE_DLG_CAP")
                    .CheckPathExists = True

                    .AddExtension = True
                    If bRTF Then
                        .DefaultExt = ".RTF"
                        .FilterIndex = 2
                    Else
                        .DefaultExt = ".TXT"
                        .FilterIndex = 1
                    End If
                    .ShowDialog()
                    sPath = .FileName
                End With
            Else
                If IO.File.Exists(sPath) Then
                    IO.File.Delete(sPath)
                End If
            End If
            If sPath <> String.Empty Then
                Dim sChangeLogText As String = gpsRM.GetString("psSAVED_DOC_TO1") & _
                        moNote.Topic & gpsRM.GetString("psSAVED_DOC_TO2") & sPath
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
    Private Sub subFilterList()
        '********************************************************************************************
        'Description: show filter window and refresh the list
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim lStatus As System.Windows.Forms.DialogResult = frmFilter.ShowDialog()

        If lStatus = Windows.Forms.DialogResult.OK Then
            tlbFilter.Checked = True
            bLoadDGV(False)
        Else
            tlbFilter.Checked = False
            bLoadDGV(True)
        End If

    End Sub

    Private Sub subShowList(ByVal bShow As Boolean)
        '********************************************************************************************
        'Description: show or hide the document list window
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        splcMain.Panel1Collapsed = Not (bShow)
        If splcMain.SplitterDistance = 0 Then
            splcMain.SplitterDistance = splcMain.Width \ 2
        End If
    End Sub
    Private Sub tlsMain_ItemClicked(ByVal sender As Object, _
               ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tlsMain.ItemClicked, tlsDoc.ItemClicked, tlsList.ItemClicked
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
        subEnableControls(False)

        Select Case e.ClickedItem.Name ' case sensitive
            Case "btnClose"
                'Check to see if we need to save is performed in  bAskForSave
                Me.Close()
                Exit Sub

            Case "btnChangeLog"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If Not (o.DropDownButtonPressed) Then
                    subShowChangeLog(CType(eChangeMnu.Hours24, Integer))
                End If
            Case "btnStatus"
                btnStatus.Checked = Not (btnStatus.Checked)
                lstStatus.Visible = btnStatus.Checked
            Case "btnHide"
                btnHide.Checked = True
                btnShow.Checked = False
                subShowList(False)
            Case "btnShow"
                btnShow.Checked = True
                btnHide.Checked = False
                subShowList(True)
            Case "tlbSave", "btnSave"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If Not (o.DropDownButtonPressed) Then
                    If (rtbDoc.Modified Or mbEditsMade) Then
                        subSaveData(False)
                    Else
                        subSaveData(True) 'No edits, offer to save to a file
                    End If
                End If
            Case "tlbPrint", "btnPrint"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If Not (o.DropDownButtonPressed) Then
                    mPrintHtml.subCreateSimpleDoc(rtbDoc, Status, msSCREEN_NAME & " - " & moNote.Topic)
                    mPrintHtml.subPrintDoc(False)
                End If
            Case "tlbOpen"
                subOpenDoc()
            Case "tlsNew", "tlbNew", "btnNew"
                subNewDoc()
            Case "tlbFilter"
                If tlbFilter.Checked Then 'restore the full list
                    tlbFilter.Checked = False
                    bLoadDGV(True)
                Else 'pop-up the filter window
                    tlbFilter.Checked = True
                    subFilterList()
                End If

            Case "tlbProp"
                subOpenPropertiesWindow()
            Case "tlbFindList"
                frmFind.SearchTopics = True
                subFind()
            Case "tlbFindDoc"
                frmFind.SearchThisDoc = True
                subFind()
            Case "tlbClear", "tlsClear"
                Dim lReply As System.Windows.Forms.DialogResult = _
                    MessageBox.Show(gpsRM.GetString("psDELETE_DOC1") & moNote.Topic & gpsRM.GetString("psDELETE_DOC2"), _
                                    msSCREEN_NAME, MessageBoxButtons.YesNo, _
                                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                If lReply = Windows.Forms.DialogResult.Yes Then
                    bDeleteCurrentDoc()
                End If
            Case "tlbNext", "btnNext"
                Dim nSelectedRow As Integer = dgvList.SelectedRows.Item(0).Index
                nSelectedRow += 1
                If nSelectedRow >= dgvList.RowCount Then
                    nSelectedRow = 0
                End If
                dgvList.Rows.Item(nSelectedRow).Selected = True
            Case "tlbPrev", "btnPrevious"
                Dim nSelectedRow As Integer = dgvList.SelectedRows.Item(0).Index
                If nSelectedRow <= 0 Then
                    nSelectedRow = dgvList.RowCount
                End If
                nSelectedRow -= 1
                dgvList.Rows.Item(nSelectedRow).Selected = True
            Case "tlbCut"
                rtbDoc.Cut()
            Case "tlbCopy"
                rtbDoc.Copy()
            Case "tlbPaste"
                rtbDoc.Paste()
            Case "tlbClear"
                Dim lReply As System.Windows.Forms.DialogResult = _
                    MessageBox.Show(gpsRM.GetString("psCLEAR_WINDOW_QUES"), msSCREEN_NAME, MessageBoxButtons.YesNo, _
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                If lReply = Windows.Forms.DialogResult.Yes Then
                    rtbDoc.Clear()
                End If
            Case "tlbBold"
                tlbBold.Checked = Not (rtbDoc.SelectionFont.Bold)
                rtbDoc.SelectionFont = New Font(rtbDoc.SelectionFont.Name, rtbDoc.SelectionFont.Size, CType(nGetFontStyle(), FontStyle))
            Case "tlbItalic"
                tlbItalic.Checked = Not (rtbDoc.SelectionFont.Italic)
                rtbDoc.SelectionFont = New Font(rtbDoc.SelectionFont.Name, rtbDoc.SelectionFont.Size, CType(nGetFontStyle(), FontStyle))
            Case "tlbUnderline"
                tlbUnderline.Checked = Not (rtbDoc.SelectionFont.Underline)
                rtbDoc.SelectionFont = New Font(rtbDoc.SelectionFont.Name, rtbDoc.SelectionFont.Size, CType(nGetFontStyle(), FontStyle))
            Case "tlbAlignLeft"
                rtbDoc.SelectionAlignment = HorizontalAlignment.Left
                tlbAlignLeft.Checked = True
                tlbAlignCenter.Checked = False
                tlbAlignRight.Checked = False
            Case "tlbAlignCenter"
                rtbDoc.SelectionAlignment = HorizontalAlignment.Center
                tlbAlignLeft.Checked = True
                tlbAlignCenter.Checked = False
                tlbAlignRight.Checked = False
            Case "tlbAlignRight"
                rtbDoc.SelectionAlignment = HorizontalAlignment.Right
                tlbAlignLeft.Checked = True
                tlbAlignCenter.Checked = False
                tlbAlignRight.Checked = False
        End Select

        subEnableControls(True)
        tlsMain.Refresh()
        tlsDoc.Refresh()
        tlsList.Refresh()

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


            GC.Collect()
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
                    subLaunchHelp(gs_HELP_UTILITIES_NOTEPAD, oIPC)

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

    Private Function nGetFontStyle() As Integer
        '********************************************************************************************
        'Description:  sum up font style buttons
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim nFontStyle As Integer = FontStyle.Regular
        If tlbBold.Checked Then
            nFontStyle += FontStyle.Bold
        End If
        If tlbItalic.Checked Then
            nFontStyle += FontStyle.Italic
        End If
        If tlbUnderline.Checked Then
            nFontStyle += FontStyle.Underline
        End If
        Return nFontStyle
    End Function

    Private Sub mnuPreviewCmds_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintPreview.Click, tlmPrintPreview.Click
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
        mPrintHtml.subCreateSimpleDoc(rtbDoc, Status, msSCREEN_NAME & " - " & moNote.Topic)
        mPrintHtml.subShowPrintPreview()
    End Sub

    Private Sub mnuPageSetup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPageSetup.Click, tlmPageSetup.Click
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
        mPrintHtml.subCreateSimpleDoc(rtbDoc, Status, msSCREEN_NAME & " - " & moNote.Topic)
        mPrintHtml.subShowPageSetup()
    End Sub

    Private Sub mnuPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrint.Click, tlmPrint.Click
        '********************************************************************************************
        'Description:  print
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mPrintHtml.subCreateSimpleDoc(rtbDoc, Status, msSCREEN_NAME & " - " & moNote.Topic)
        mPrintHtml.subPrintDoc(False)
    End Sub
    Private Sub mnuPrintFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintFile.Click, tlmPrintFile.Click
        '********************************************************************************************
        'Description:  print the table
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/07/10  MSW	    Print function clean up
        '********************************************************************************************
        mPrintHtml.subCreateSimpleDoc(rtbDoc, Status, msSCREEN_NAME & " - " & moNote.Topic)
        mPrintHtml.subSaveAs()
    End Sub
    Private Sub mnuSaveDB_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuSaveDB.Click, mnuSaveDBDoc.Click
        '********************************************************************************************
        'Description:  toolbar drop-down menu click handler
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        subSaveData(False)
    End Sub

    Private Sub mnuSaveFileRTF_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuSaveFileRTF.Click, mnuSaveFileRTFDoc.Click
        '********************************************************************************************
        'Description:  toolbar drop-down menu click handler
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        subSaveData(True, True)

    End Sub

    Private Sub mnuSaveFileTXT_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuSaveFileTXT.Click, mnuSaveFileTXTDoc.Click
        '********************************************************************************************
        'Description:  toolbar drop-down menu click handler
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        subSaveData(True, False)

    End Sub

    Private Sub rtbDoc_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles rtbDoc.KeyDown
        '********************************************************************************************
        'Description:  process special characters
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Duplicating the main menu functions here in case the rtb intercepts them
        If Not (e.Control) Then
            Select Case e.KeyCode
                Case Keys.F3
                    subFind((mnFindNextIndex > -1))
                Case Else

            End Select
        Else
            Select Case e.KeyCode  'Ctrl-...
                Case Keys.B
                    tlbBold.Checked = Not (rtbDoc.SelectionFont.Bold)
                    rtbDoc.SelectionFont = New Font(rtbDoc.SelectionFont.Name, rtbDoc.SelectionFont.Size, CType(nGetFontStyle(), FontStyle))
                    e.SuppressKeyPress = True
                Case Keys.I
                    tlbItalic.Checked = Not (rtbDoc.SelectionFont.Italic)
                    rtbDoc.SelectionFont = New Font(rtbDoc.SelectionFont.Name, rtbDoc.SelectionFont.Size, CType(nGetFontStyle(), FontStyle))
                    e.SuppressKeyPress = True
                Case Keys.U
                    tlbUnderline.Checked = Not (rtbDoc.SelectionFont.Underline)
                    rtbDoc.SelectionFont = New Font(rtbDoc.SelectionFont.Name, rtbDoc.SelectionFont.Size, CType(nGetFontStyle(), FontStyle))
                    e.SuppressKeyPress = True
                Case Keys.S
                    subSaveData(False)
                    e.SuppressKeyPress = True
                Case Keys.P
                    mPrintHtml.subCreateSimpleDoc(rtbDoc, Status, msSCREEN_NAME & " - " & moNote.Topic)
                    mPrintHtml.subPrintDoc(False)
                    e.SuppressKeyPress = True
                Case Keys.F
                    frmFind.SearchThisDoc = True
                    subFind()
            End Select
        End If
    End Sub

    Private Sub rtb_ModifiedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rtbDoc.ModifiedChanged
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

    Private Sub rtb_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rtbDoc.SelectionChanged
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
        mnFindNextIndex = -1
        Dim bCmdSel As Boolean = rtbDoc.SelectionLength > 0
        Select Case Privilege
            Case ePrivilege.None
                tlbCut.Enabled = False
                tlbCopy.Enabled = False
            Case Else
                tlbCut.Enabled = bCmdSel
                tlbCopy.Enabled = bCmdSel
        End Select
        Try
            If rtbDoc.SelectionFont Is Nothing Then
                tlbBold.Checked = False
                tlbItalic.Checked = False
                tlbUnderline.Checked = False
            Else
                tlbBold.Checked = rtbDoc.SelectionFont.Bold
                tlbItalic.Checked = rtbDoc.SelectionFont.Italic
                tlbUnderline.Checked = rtbDoc.SelectionFont.Underline
            End If
            Select Case rtbDoc.SelectionAlignment
                Case HorizontalAlignment.Left
                    tlbAlignLeft.Checked = True
                    tlbAlignCenter.Checked = False
                    tlbAlignRight.Checked = False
                Case HorizontalAlignment.Center
                    tlbAlignLeft.Checked = False
                    tlbAlignCenter.Checked = True
                    tlbAlignRight.Checked = False
                Case HorizontalAlignment.Right
                    tlbAlignLeft.Checked = False
                    tlbAlignCenter.Checked = False
                    tlbAlignRight.Checked = True
            End Select
        Catch ex As Exception
            tlbBold.Checked = False
            tlbItalic.Checked = False
            tlbUnderline.Checked = False
            tlbAlignLeft.Checked = False
            tlbAlignCenter.Checked = False
            tlbAlignRight.Checked = False
        End Try

    End Sub
    Private Sub rtbDoc_DragDrop(ByVal sender As Object, _
            ByVal e As System.Windows.Forms.DragEventArgs) Handles rtbDoc.DragDrop

        Dim i As Integer
        Dim s As String
        '********************************************************************************************
        'Description:  drag and drop handlers
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        ' Get start position to drop the text.
        i = rtbDoc.SelectionStart
        s = rtbDoc.Text.Substring(i)
        rtbDoc.Text = rtbDoc.Text.Substring(0, i)

        ' Drop the text on to the RichTextBox.
        rtbDoc.Text = rtbDoc.Text + _
           e.Data.GetData(DataFormats.Text).ToString()
        rtbDoc.Text = rtbDoc.Text + s
    End Sub
    Private Sub rtbDoc_DragEnter(ByVal sender As Object, _
            ByVal e As System.Windows.Forms.DragEventArgs) Handles rtbDoc.DragEnter
        '********************************************************************************************
        'Description:  drag and drop handlers
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************   
        If (e.Data.GetDataPresent(DataFormats.Text)) Then
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If
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
        If (rtbDoc.Modified Or mbEditsMade) Then
            Dim lRet As DialogResult

            lRet = MessageBox.Show(gcsRM.GetString("csSAVEMSG1") & vbCrLf & _
                                gcsRM.GetString("csSAVEMSG"), gcsRM.GetString("csSAVE_CHANGES"), _
                                MessageBoxButtons.YesNoCancel, _
                                MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

            Select Case lRet
                Case Response.Yes 'Response.Yes
                    mbInAskForSave = True
                    subSaveData()
                    mbInAskForSave = False
                    Return True
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
    Private Function nSearchNextDoc(ByRef sFind As String, ByVal eCompareMethod As CompareMethod) As Integer
        '********************************************************************************************
        'Description:  Open find window
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  

        Try
            If bAskForSave() Then
                Dim dtTmp As DataTable = Nothing
                Dim nRow As Integer = dgvList.SelectedRows.Item(0).Index
                Dim nStartRow As Integer = nRow
                Dim nFind As Integer = 0
                Do
                    If mbSearchBack Then
                        nRow += -1
                        If nRow < 0 Then
                            nRow = dgvList.RowCount - 1
                        End If
                    Else
                        nRow += 1
                        If nRow >= dgvList.RowCount Then
                            nRow = 0
                        End If
                    End If
                    Dim nID As Integer = CInt(dgvList.Rows.Item(nRow).Cells.Item(gpsRM.GetString("psID_CAP")).Value)
                    For Each oNote As Note In mcolNotes
                        If oNote.ID = nID Then
                            bReadDocumentFromDB(nID, oNote)
                            Application.DoEvents()
                            nFind = InStr(oNote.Text, sFind, eCompareMethod)
                            Application.DoEvents()
                            Exit For
                        End If
                    Next
                Loop Until ((nRow = nStartRow) Or (nFind > 0))
                If nFind > 0 Then
                    dgvList.Rows.Item(nRow).Selected = True
                    Application.DoEvents()
                    'refind in text characters instead of RTF data
                    If mbSearchBack Then
                        'Start from the current selection
                        nFind = InStrRev(rtbDoc.Text, sFind, rtbDoc.Text.Length, eCompareMethod)
                    Else
                        'Start from the current selection
                        nFind = InStr(rtbDoc.Text, sFind, eCompareMethod)
                    End If
                End If
                Return (nFind)
            Else
                Return (0)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
        End Try
    End Function
    Private Sub subFind(Optional ByVal bSkipDialog As Boolean = False)
        '********************************************************************************************
        'Description:  Open find window
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        subEnableControls(False)
        Try
            Dim lStatus As System.Windows.Forms.DialogResult = Windows.Forms.DialogResult.OK
            If bSkipDialog Then
                If frmFind.SearchText.Length = 0 Then
                    lStatus = frmFind.ShowDialog()
                    mbSearchBack = frmFind.Backwards

                End If
            Else
                lStatus = frmFind.ShowDialog()
                mbSearchBack = frmFind.Backwards
            End If
            If lStatus = Windows.Forms.DialogResult.OK Then
                Dim eCompareMethod As CompareMethod
                If frmFind.CaseSensitive Then
                    eCompareMethod = CompareMethod.Binary
                Else
                    eCompareMethod = CompareMethod.Text
                End If
                Dim sFind As String = frmFind.SearchText
                'Topics search
                If frmFind.SearchTopics Then
                    mnFindNextIndex = -1 'Clear this if they switch to topics
                    If bAskForSave() Then
                        Dim nRow As Integer = dgvList.SelectedRows.Item(0).Index
                        Dim nStartRow As Integer = nRow
                        Dim bFound As Boolean = False
                        Do
                            If mbSearchBack Then
                                nRow += -1
                                If nRow < 0 Then
                                    nRow = dgvList.RowCount - 1
                                End If
                            Else
                                nRow += 1
                                If nRow >= dgvList.RowCount Then
                                    nRow = 0
                                End If
                            End If
                            If (InStr(dgvList.Rows.Item(nRow).Cells("TOPIC").Value.ToString, sFind, eCompareMethod) > 0) Then
                                bFound = True
                                dgvList.Rows.Item(nRow).Selected = True
                            End If
                        Loop Until ((nRow = nStartRow) Or (bFound = True))
                        If ((bFound = False) Or (nRow = nStartRow)) Then
                            MessageBox.Show(gpsRM.GetString("psTEXT_NOT_FOUND"), msSCREEN_NAME, MessageBoxButtons.OK)
                        End If
                    End If
                Else
                    'Text search
                    'Last search location result
                    Dim nIndex As Integer = mnFindNextIndex
                    If nIndex = -1 Then 'Nothing saved, start from the selection
                        nIndex = rtbDoc.SelectionStart + 1
                    End If
                    Dim nFind As Integer = 0
                    If mbSearchBack Then
                        'Start from the current selection
                        nFind = InStrRev(rtbDoc.Text, sFind, nIndex, eCompareMethod)
                        'If it's not found, start from the end and go back
                        If nFind = 0 Then
                            If frmFind.SearchAllDocs Then
                                nFind = nSearchNextDoc(sFind, eCompareMethod)
                            Else
                                nFind = InStrRev(rtbDoc.Text, sFind, rtbDoc.Text.Length, eCompareMethod)
                            End If
                        End If
                    Else
                        'Start from the current selection
                        nFind = InStr(nIndex, rtbDoc.Text, sFind, eCompareMethod)
                        'If it's not found, start from the beginning
                        If nFind = 0 Then
                            If frmFind.SearchAllDocs Then
                                nFind = nSearchNextDoc(sFind, eCompareMethod)
                            Else
                                nFind = InStr(rtbDoc.Text, sFind, eCompareMethod)
                            End If
                        End If
                    End If
                    If nFind > 0 Then
                        rtbDoc.SelectionStart = nFind - 1
                        rtbDoc.SelectionLength = sFind.Length
                        mnFindNextIndex = nFind + 1
                    Else
                        mnFindNextIndex = -1
                        MessageBox.Show(gpsRM.GetString("psTEXT_NOT_FOUND"), msSCREEN_NAME, MessageBoxButtons.OK)
                    End If
                End If
            End If
            subEnableControls(True)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        End Try

    End Sub
    Private Sub subOpenPropertiesWindow()
        '********************************************************************************************
        'Description:  rename the document
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        subEnableControls(False)
        Try
            frmTopic.FormTitle = gpsRM.GetString("psPROP_FRM_CAP")
            frmTopic.Topic = moNote.Topic
            Dim lStatus As System.Windows.Forms.DialogResult = frmTopic.ShowDialog()
            If lStatus = Windows.Forms.DialogResult.OK Then

                moNote.Topic = frmTopic.Topic
                lblTopic.Text = gpsRM.GetString("psTOPIC") & ": " & frmTopic.Topic

                For Each oNote As Note In mcolNotes
                    If oNote.ID = moNote.ID Then
                        oNote.Topic = moNote.Topic
                    End If
                Next
                mbEditsMade = True
            End If
            subEnableControls(True)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        End Try
    End Sub
    Private Sub subNewDoc()
        '********************************************************************************************
        'Description:  Start a new document
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        subEnableControls(False)
        Try
            If bAskForSave() Then
                frmTopic.FormTitle = gpsRM.GetString("psNEW_FRM_CAP")
                frmTopic.Topic = String.Empty
                Dim lStatus As System.Windows.Forms.DialogResult = frmTopic.ShowDialog()
                If lStatus = Windows.Forms.DialogResult.OK Then
                    'Make a new Document
                    bNewDocument(frmTopic.Topic, Now.ToString)
                    rtbDoc.Text = Now.ToString 'dtDoc.Rows(0).Item(gpsRM.GetString("psTEXT_CAP")).ToString
                    rtbDoc.Enabled = True
                    rtbDoc.Modified = False
                    mbEditsMade = False
                    DataLoaded = True
                End If
            End If
            subEnableControls(True)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        End Try
    End Sub

    Private Sub subOpenDoc()
        '********************************************************************************************
        'Description:  Start a new document
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        subEnableControls(False)
        Try
            If bAskForSave() Then
                '''''''Open the doc and load into rtbDoc
                Dim sPath As String = String.Empty
                Dim o As New OpenFileDialog
                With o
                    .Filter = gcsRM.GetString("csSAVE_TXT_RTF_ALL_FILTER")
                    .Title = gcsRM.GetString("csOPEN_FILE_DLG_CAP")
                    .CheckPathExists = True

                    .FilterIndex = 1

                    .ShowDialog()
                    sPath = .FileName
                End With
                If sPath <> String.Empty Then
                    If sPath.Substring((sPath.Length - 3)).ToLower = "rtf" Then
                        rtbDoc.LoadFile(sPath, RichTextBoxStreamType.RichText)
                    Else
                        rtbDoc.LoadFile(sPath, RichTextBoxStreamType.PlainText)
                    End If
                    frmTopic.FormTitle = gpsRM.GetString("psNAME_FRM_CAP")
                    frmTopic.Topic = String.Empty
                    Dim lStatus As System.Windows.Forms.DialogResult = frmTopic.ShowDialog()
                    If lStatus = Windows.Forms.DialogResult.OK Then
                        'Make a new Document
                        bNewDocument(frmTopic.Topic, rtbDoc.Rtf)
                        rtbDoc.Enabled = True
                        rtbDoc.Modified = False
                        mbEditsMade = False
                        Dim sChangeLogText As String = gpsRM.GetString("psOPEN_DOC_TO1") & sPath & _
                                gpsRM.GetString("psOPEN_DOC_TO2") & frmTopic.Topic & gpsRM.GetString("psOPEN_DOC_TO3")
                        mPWCommon.AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                                                  colZones.CurrentZoneNumber, _
                                                  String.Empty, String.Empty, _
                                                  sChangeLogText, colZones.CurrentZone)
                        mPWCommon.SaveToChangeLog(colZones.ActiveZone)
                        DataLoaded = True
                    End If
                End If
            End If
            subEnableControls(True)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        End Try
    End Sub
    Private Sub splcMain_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles splcMain.DoubleClick
        '********************************************************************************************
        'Description:  format the window
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        splcMain.SplitterDistance = dgvList.PreferredSize.Width
    End Sub


    Private Sub dgvList_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles dgvList.KeyDown
        '********************************************************************************************
        'Description:  process special characters
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Duplicating the main menu functions here in case the rtb intercepts them
        If Not (e.Control) Then
            Select Case e.KeyCode
                Case Keys.F3
                    subFind((mnFindNextIndex > -1))
                Case Else

            End Select
        Else
            Select Case e.KeyCode  'Ctrl-...
                Case Keys.F
                    frmFind.SearchTopics = True
                    subFind()
            End Select
        End If

    End Sub

    Private Sub dgvList_SelectionChanged(Optional ByVal sender As Object = Nothing, _
                                         Optional ByVal e As System.EventArgs = Nothing) Handles dgvList.SelectionChanged
        '********************************************************************************************
        'Description:  new doc selected
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        mnFindNextIndex = -1
        If mbDontUpdate = False Then
            Try
                If dgvList.SelectedRows.Count > 0 Then
                    mnSelectedRow = dgvList.SelectedRows.Item(0).Index
                    If bAskForSave() Then
                        'bAskForSave will move the selection back if it gets saved, so first reselect in dgvlist
                        mbDontUpdate = True         'Without going through all this logic again
                        dgvList.Rows.Item(mnSelectedRow).Selected = True
                        mbDontUpdate = False

                        'Read the text into dtDoc, then load into rtbDoc
                        Dim sID As String = dgvList.Rows.Item(mnSelectedRow).Cells.Item(gpsRM.GetString("psID_CAP")).Value.ToString
                        If IsNumeric(sID) Then
                            Dim nID As Integer = CType(sID, Integer)
                            For Each oNote As Note In mcolNotes
                                If nID = oNote.ID Then
                                    moNote = oNote
                                    'Get the list items for the labels
                                    lblTopic.Text = oNote.Topic
                                    lblAuthor.Text = oNote.Name
                                    lblCreated.Text = oNote.Created.ToString
                                    lblModified.Text = oNote.Modified.ToString

                                    bReadDocumentFromDB(nID, moNote)
                                    If oNote.Text.Length > 0 Then
                                        If oNote.Text.StartsWith("{\rtf1") Then
                                            rtbDoc.Rtf = oNote.Text
                                        Else
                                            rtbDoc.Text = oNote.Text
                                        End If
                                    Else
                                        rtbDoc.Text = String.Empty
                                    End If
                                    rtbDoc.Enabled = True
                                    rtbDoc.Modified = False
                                    mbEditsMade = False
                                    DataLoaded = True
                                    mnFindNextIndex = -1
                                    Exit For
                                End If
                            Next
                        End If
                    Else
                        mbDontUpdate = True

                        dgvList.Rows.Item(mnSelectedRow).Selected = True

                        mbDontUpdate = False
                    End If
                End If
            Catch ex As Exception
                Trace.WriteLine(ex.Message)
                Trace.WriteLine(ex.StackTrace)
                ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                    Status, MessageBoxButtons.OK)

            End Try

        End If
    End Sub

End Class
Class Note
    Public ID As Integer
    Public Name As String
    Public Topic As String
    Public Path As String
    Public Created As Date
    Public Modified As Date
    Public Text As String
End Class