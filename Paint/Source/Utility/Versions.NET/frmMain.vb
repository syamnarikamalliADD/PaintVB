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
' Description: Versions
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
'    11/30/2011 MSW     first draft                                                   4.01.01.00
'    01/03/2012 MSW     better btnOpen image                                          4.01.01.01
'    01/18/12   MSW     Clean up old printsettings object                             4.01.01.02
'    01/24/12   MSW     Placeholder include updates                                   4.01.01.03
'    01/25/12   MSW     Check version from frmMain comments against project version   4.01.01.04
'    02/15/12   MSW     Force 32 bit build, print management udpates                  4.01.01.05
'    03/23/12   RJO     modifed for .NET Password and IPC                             4.01.02.00
'    04/11/12   MSW     Change CommonStrings setup so it builds correctly             4.01.03.00
'    02/22/13   MSW     Figure out an appropriate change date based on the whole      4.01.04.00
'                       folder for .NET projects
'    04/16/13   MSW     Add Canadian language files                                   4.01.05.00
'                       Standalong Changelog
'    07/09/13   MSW     Update and standardize logos                                  4.01.05.01
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up  4.01.05.02
'    09/30/13   MSW     Save screenshots as jpegs                                     4.01.06.00
'                       Deal with BSD and PLCIO common folders
'    01/07/14   MSW     Remove ControlBox from main form                              4.01.06.01
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call              4.01.07.00
'    04/21/14   MSW     Add psSCREENCAPTION to project strings                        4.01.07.01
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Configuration.ConfigurationSettings
Imports Response = System.Windows.Forms.DialogResult
Imports ConnStat = FRRobotNeighborhood.FRERNConnectionStatusConstants

Friend Class frmMain
#Region " Declares "

    '******** Form Constants   **********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    'Not a constant here, we'll try to get help in the same executable
    Friend Const msSCREEN_NAME As String = "Versions"   ' <-- For password area change log etc.
    Private Const msSCREEN_DUMP_NAME As String = "Utility_Versions_Main.jpg"
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
    Private dtList As DataTable = Nothing
    Private dtDoc As DataTable = Nothing
    Private mnMaxID As Integer = 0
    Private mnSelectedRow As Integer = 0
    Private mbDontUpdate As Boolean = False
    Private mbEditsMade As Boolean = False
    Private mnFindNextIndex As Integer = -1
    Private mbSearchBack As Boolean = False
    Private mbInAskForSave As Boolean = False
    Private mbCompareOpen As Boolean = False
    Private msCompareFolder As String = String.Empty
    Public Structure VersionInfo
        Dim Name As String
        Dim Version As String
        Dim Type As String
        Dim Location As String
        Dim dtDate As Date
        Dim Red As Boolean
    End Structure

    Private mtVersionInfo() As VersionInfo = Nothing
    Private mnItemCount As Integer = 0
    Private mtCmpVersionInfo() As VersionInfo = Nothing
    Private mnCmpItemCount As Integer = 0
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
        '********************************************************************************************
        Get
            Return mbDataLoaded
        End Get
        Set(ByVal Value As Boolean)
            mbDataLoaded = Value
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

        lblPanel1.Text = String.Empty
        lblPanel2.Text = String.Empty
        dgPanel1.Columns.Clear()
        dgPanel1.Rows.Clear()
        dgPanel2.Columns.Clear()
        dgPanel2.Rows.Clear()

        dgPanel1.Columns.Add("colItem", " ")
        dgPanel1.Columns.Add("colFileName", gpsRM.GetString("psCOL_NAME"))
        dgPanel1.Columns.Add("colVersion", gpsRM.GetString("psCOL_VERSION"))
        dgPanel1.Columns.Add("colType", gpsRM.GetString("psCOL_TYPE"))
        dgPanel1.Columns.Add("colLocation", gpsRM.GetString("psCOL_LOCATION"))
        dgPanel1.Columns.Add("colDate", gpsRM.GetString("psCOL_DATE"))
        dgPanel2.Columns.Add("colItem", " ")
        dgPanel2.Columns.Add("colFileName", gpsRM.GetString("psCOL_NAME"))
        dgPanel2.Columns.Add("colVersion", gpsRM.GetString("psCOL_VERSION"))
        dgPanel2.Columns.Add("colType", gpsRM.GetString("psCOL_TYPE"))
        dgPanel2.Columns.Add("colLocation", gpsRM.GetString("psCOL_LOCATION"))
        dgPanel2.Columns.Add("colDate", gpsRM.GetString("psCOL_DATE"))
        dgPanel1.Columns(0).ValueType = GetType(Integer)
        dgPanel2.Columns(0).ValueType = GetType(Integer)
        dgPanel1.Columns(5).ValueType = GetType(Date)
        dgPanel2.Columns(5).ValueType = GetType(Date)
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
        btnClose.Enabled = True
        Dim bDataLoaded As Boolean = DataLoaded
        If bEnable = False Then
            bRestOfControls = False
        Else
            Select Case Privilege
                Case ePrivilege.None
                    bRestOfControls = False
                Case Else
                    bRestOfControls = bDataLoaded
            End Select
        End If
        btnSave.Enabled = bRestOfControls
        btnPrint.Enabled = bRestOfControls
        mnuPrintFile.Enabled = bRestOfControls
        mnuSaveCSV.Enabled = bRestOfControls
        mnuSaveCmpCSV.Enabled = bRestOfControls And mbCompareOpen
        If bEnable Then
            Me.Cursor = System.Windows.Forms.Cursors.Default
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
        mbEditsMade = False
        DataLoaded = False
        btnRefresh.Text = gpsRM.GetString("psREFRESH")
        btnOpen.Text = gpsRM.GetString("psCOMPARE")
        btnRefresh.ToolTipText = gpsRM.GetString("psREFRESH")
        btnOpen.ToolTipText = gpsRM.GetString("psCOMPARE")
        mnuSaveCSV.Text = gpsRM.GetString("psSAVE_CSV")
        mnuSaveCmpCSV.Text = gpsRM.GetString("psSAVE_CMP_CSV")
    End Sub
    Private Sub subCompareTables()
        '********************************************************************************************
        'Description: Compare the two tables
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        dgPanel1.Sort(dgPanel1.Columns(4), System.ComponentModel.ListSortDirection.Ascending)
        dgPanel2.Sort(dgPanel2.Columns(4), System.ComponentModel.ListSortDirection.Ascending)

        Dim nSourceIndex As Integer = 0
        Dim nCompareIndex As Integer = 0
        Dim nNewIndex As Integer = 0
        'Guessing this is more efficient thatn a redim on each step
        ReDim mtVersionInfo(mnCmpItemCount + mnItemCount)
        ReDim mtCmpVersionInfo(mnCmpItemCount + mnItemCount)
        Dim sName(1) As String
        Dim sVersion(1) As String
        Dim sType(1) As String
        Dim sLocation(1) As String
        Dim dtDates(1) As Date
        Do While (nSourceIndex < mnItemCount) Or (nCompareIndex < mnCmpItemCount)
            If nSourceIndex < mnItemCount Then
                sName(0) = dgPanel1.Rows.Item(nSourceIndex).Cells(1).Value.ToString
                sVersion(0) = dgPanel1.Rows.Item(nSourceIndex).Cells(2).Value.ToString
                sType(0) = dgPanel1.Rows.Item(nSourceIndex).Cells(3).Value.ToString
                sLocation(0) = dgPanel1.Rows.Item(nSourceIndex).Cells(4).Value.ToString
                dtDates(0) = DirectCast(dgPanel1.Rows.Item(nSourceIndex).Cells(5).Value, Date)
            End If
            If nCompareIndex < mnCmpItemCount Then
                sName(1) = dgPanel2.Rows.Item(nCompareIndex).Cells(1).Value.ToString
                sVersion(1) = dgPanel2.Rows.Item(nCompareIndex).Cells(2).Value.ToString
                sType(1) = dgPanel2.Rows.Item(nCompareIndex).Cells(3).Value.ToString
                sLocation(1) = dgPanel2.Rows.Item(nCompareIndex).Cells(4).Value.ToString
                dtDates(1) = DirectCast(dgPanel2.Rows.Item(nCompareIndex).Cells(5).Value, Date)
            End If
            Dim sLocSplit1() As String = Split(sLocation(0), "\")
            Dim sLocSplit2() As String = Split(sLocation(1), "\")
            Dim nCompare As Integer
            'Special case at the end of the tables:
            If nSourceIndex > mnItemCount Then
                nCompare = 1
            ElseIf nCompareIndex > mnCmpItemCount Then
                nCompare = -1
            Else
                nCompare = StrComp(sName(0), sName(1), CompareMethod.Text)
            End If

            Select Case nCompare
                Case 0
                    'Names match.  Compare and add to tables
                    'Same Name and foldername
                    Dim bDifferent As Boolean = False
                    Dim nDateDiff As Long = DateDiff(DateInterval.Minute, dtDates(0), dtDates(1))

                    If sVersion(0) <> sVersion(1) Or _
                        nDateDiff <> 0 Then
                        bDifferent = True
                    End If
                    With mtVersionInfo(nNewIndex)
                        .Name = sName(0)
                        .Version = sVersion(0)
                        .Type = sType(0)
                        .Location = sLocation(0)
                        .dtDate = dtDates(0)
                        .Red = bDifferent
                    End With
                    'Same Name and foldername
                    With mtCmpVersionInfo(nNewIndex)
                        .Name = sName(1)
                        .Version = sVersion(1)
                        .Type = sType(1)
                        .Location = sLocation(1)
                        .dtDate = dtDates(1)
                        .Red = bDifferent
                    End With
                    nSourceIndex += 1
                    nCompareIndex += 1
                    nNewIndex += 1
                Case -1
                    'Names match.  Compare and add to tables
                    'Same Name and foldername
                    With mtVersionInfo(nNewIndex)
                        .Name = sName(0)
                        .Version = sVersion(0)
                        .Type = sType(0)
                        .Location = sLocation(0)
                        .dtDate = dtDates(0)
                        .Red = True
                    End With
                    'Same Name and foldername
                    With mtCmpVersionInfo(nNewIndex)
                        .Name = sName(0)
                        .Version = String.Empty
                        .Type = String.Empty
                        .Location = String.Empty
                        .dtDate = Now
                        .Red = True
                    End With
                    nSourceIndex += 1
                    nNewIndex += 1
                Case 1
                    'Names match.  Compare and add to tables
                    'Same Name and foldername
                    With mtVersionInfo(nNewIndex)
                        .Name = sName(1)
                        .Version = String.Empty
                        .Type = String.Empty
                        .Location = String.Empty
                        .dtDate = Now
                        .Red = True
                    End With
                    'Same Name and foldername
                    With mtCmpVersionInfo(nNewIndex)
                        .Name = sName(1)
                        .Version = sVersion(1)
                        .Type = sType(1)
                        .Location = sLocation(1)
                        .dtDate = dtDates(1)
                        .Red = True
                    End With
                    nCompareIndex += 1
                    nNewIndex += 1
            End Select
        Loop
        mnItemCount = nNewIndex
        mnCmpItemCount = nNewIndex
        ReDim Preserve mtVersionInfo(nNewIndex - 1)
        ReDim Preserve mtCmpVersionInfo(nNewIndex - 1)
        Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub

    Private Sub subLoadPanel(ByRef dgPanel As DataGridView, ByRef tVersionInfo As VersionInfo(), ByRef nItemCount As Integer)
        '********************************************************************************************
        'Description: Load version list into datagridview
        '
        'Parameters: DataGridView control, 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        dgPanel.Rows.Clear()
        dgPanel.RowCount = nItemCount
        For nItem As Integer = 1 To nItemCount
            With tVersionInfo(nItem - 1)
                Dim sRow(5) As String
                sRow(0) = nItem.ToString
                sRow(1) = .Name
                sRow(2) = .Version
                sRow(3) = .Type
                sRow(4) = .Location
                sRow(5) = .dtDate.ToString
                For nCell As Integer = 0 To 5
                    If .Red Then
                        dgPanel.Rows.Item(nItem - 1).Cells(nCell).Style.ForeColor = Color.Red
                    Else
                        dgPanel.Rows.Item(nItem - 1).Cells(nCell).Style.ForeColor = Color.Black
                    End If
                    If nCell = 0 Then
                        'Integer
                        dgPanel.Rows.Item(nItem - 1).Cells(nCell).Value = nItem
                    ElseIf nCell = 5 Then
                        dgPanel.Rows.Item(nItem - 1).Cells(nCell).Value = .dtDate
                    Else
                        dgPanel.Rows.Item(nItem - 1).Cells(nCell).Value = sRow(nCell)
                    End If
                Next

            End With

        Next
    End Sub
    Private Sub subLoadVersions(ByRef tVersionInfo As VersionInfo(), ByRef nItemCount As Integer, ByRef sSourcePath As String)
        '********************************************************************************************
        'Description: Load version list into array
        '
        'Parameters: Data array, source path
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/25/12  MSW     Check version from frmMain comments against project version
        ' 09/30/13  MSW     Deal with BSD and PLCIO common folders
        '********************************************************************************************
        Dim sName As String
        Dim sLocation As String = String.Empty
        Dim sType As String = String.Empty
        Dim dtDate As Date = Now
        Dim sVersion As String = String.Empty
        Dim sVersionAlt As String = String.Empty
        Dim bAddInfo As Boolean
        Dim bDoSubFolders As Boolean = False
        Dim sFiles() As String = IO.Directory.GetFiles(sSourcePath)
        Dim dtProjectDate As Date = Date.MinValue
        Dim sr As System.IO.StreamReader = Nothing

        Status = gpsRM.GetString("psSEARCHING") & sSourcePath
        System.Windows.Forms.Application.DoEvents() 'Allow the statusbar to update
        For Each sFilePath As String In sFiles
            sVersion = String.Empty
            sVersionAlt = String.Empty
            bAddInfo = False
            Dim sFileTmp As String = sFilePath.Replace("/", "\")
            Dim sFolderTree() As String = Split(sFileTmp, "\")
            Dim sFileName As String = sFolderTree(sFolderTree.GetUpperBound(0))
            Dim sTmpSplit() As String = Split(sFileName, ".")
            Dim sExt As String = sTmpSplit(sTmpSplit.GetUpperBound(0))
            Dim bCommon As Boolean = False
            Dim bSubCommon As Boolean = False
            Dim sSubCommon As String = String.Empty
            sName = sFolderTree(sFolderTree.GetUpperBound(0))
            For Each sFolder As String In sFolderTree
                If sFolder.Trim.ToLower = "common" Or _
                    sFolder.Trim.ToLower = "common.net" Then
                    bCommon = True
                    Exit For
                End If
                If sFolder.ToLower.Contains("_common") Then
                    bSubCommon = True
                    sSubCommon = sFolder
                    Exit For
                End If
            Next

            Select Case sExt
                Case "frm", "cls", "bas"
                    'common files
                    dtDate = IO.File.GetLastWriteTime(sFilePath)
                    If bCommon Then
                        sLocation = sSourcePath
                        sType = "Common"
                        sVersion = String.Empty

                        sr = System.IO.File.OpenText(sFilePath)
                        Do While sr.Peek() >= 0
                            Dim sLine As String = sr.ReadLine()
                            Dim nCol As Integer = InStr(1, sLine, "PW_VERSION")
                            If nCol > 0 Then
                                sLine = sLine.Substring(nCol + 10)
                                sVersion = Trim(sLine)
                                Exit Do
                            End If
                        Loop
                        bAddInfo = True

                        sr.Close()
                    End If
                Case "vb"
                    'common files
                    Dim bSkip As Boolean = False
                    If sTmpSplit.GetUpperBound(0) > 1 Then
                        Dim sTmp1 As String = sTmpSplit(sTmpSplit.GetUpperBound(0) - 1)
                        If sTmp1.ToLower = "designer" Then
                            bSkip = True
                        End If
                    End If

                    dtDate = IO.File.GetLastWriteTime(sFilePath)
                    If (bSkip = False) Then

                        If bSubCommon And sName.ToLower.Contains("frmmain") Then
                            sName = sSubCommon
                            sLocation = sSourcePath
                            Dim sList As String() = IO.Directory.GetFiles(sSourcePath, sTmpSplit(0) & "*", IO.SearchOption.TopDirectoryOnly)
                            For Each sFileTmp1 As String In sList
                                Dim dtTmpDate As Date = IO.File.GetLastWriteTime(sFileTmp1)
                                If dtTmpDate > dtDate Then
                                    dtDate = dtTmpDate
                                End If
                            Next
                            sType = "Project(VB.NET)"
                            sVersion = String.Empty
                            sr = System.IO.File.OpenText(sFilePath)
                            Dim nSearch As Integer = 0
                            Do While sr.Peek() >= 0 And (nSearch < 2)
                                Dim sLine As String = sr.ReadLine()
                                sLine = sLine.Replace(vbTab, " ")
                                Dim sSplit() As String = Split(sLine.Trim, " ")
                                Dim nLastNonEmpty As Integer = -1
                                For nIndex As Integer = 0 To sSplit.Length - 1
                                    If sSplit(nIndex) <> "" Then
                                        nLastNonEmpty += 1
                                        sSplit(nLastNonEmpty) = sSplit(nIndex)
                                    End If
                                Next
                                If nLastNonEmpty > -1 Then
                                    ReDim Preserve sSplit(nLastNonEmpty)
                                    Select Case nSearch
                                        Case 0
                                            If sSplit(nLastNonEmpty).ToLower = "version" Then
                                                nSearch = 1
                                            End If
                                        Case 1
                                            If sSplit(0) = "Option" Or sSplit(0) = "Imports" Or _
                                               sSplit(0) = "Public" Or sSplit(0) = "Friend" Then
                                                nSearch = 2
                                            Else
                                                Dim sVer() As String = Split(sSplit(nLastNonEmpty), ".")
                                                If sVer.Length > 2 AndAlso IsNumeric(sVer(0)) AndAlso IsNumeric(sVer(1)) AndAlso IsNumeric(sVer(2)) Then
                                                    sVersion = sSplit(nLastNonEmpty)
                                                End If
                                            End If
                                    End Select
                                End If
                            Loop
                            bAddInfo = True

                            sr.Close()

                        End If
                        If bCommon Then
                            sLocation = sSourcePath
                            Dim sList As String() = IO.Directory.GetFiles(sSourcePath, sTmpSplit(0) & "*", IO.SearchOption.TopDirectoryOnly)
                            For Each sFileTmp1 As String In sList
                                Dim dtTmpDate As Date = IO.File.GetLastWriteTime(sFileTmp1)
                                If dtTmpDate > dtDate Then
                                    dtDate = dtTmpDate
                                End If
                            Next
                            sType = "Common(VB.NET)"
                            sVersion = String.Empty
                            sr = System.IO.File.OpenText(sFilePath)
                            Dim nSearch As Integer = 0
                            Do While sr.Peek() >= 0 And (nSearch < 2)
                                Dim sLine As String = sr.ReadLine()
                                sLine = sLine.Replace(vbTab, " ")
                                Dim sSplit() As String = Split(sLine.Trim, " ")
                                Dim nLastNonEmpty As Integer = -1
                                For nIndex As Integer = 0 To sSplit.Length - 1
                                    If sSplit(nIndex) <> "" Then
                                        nLastNonEmpty += 1
                                        sSplit(nLastNonEmpty) = sSplit(nIndex)
                                    End If
                                Next
                                If nLastNonEmpty > -1 Then
                                    ReDim Preserve sSplit(nLastNonEmpty)
                                    Select Case nSearch
                                        Case 0
                                            If sSplit(nLastNonEmpty).ToLower = "version" Then
                                                nSearch = 1
                                            End If
                                        Case 1
                                            If sSplit(0) = "Option" Or sSplit(0) = "Imports" Or _
                                               sSplit(0) = "Public" Or sSplit(0) = "Friend" Then
                                                nSearch = 2
                                            Else
                                                Dim sVer() As String = Split(sSplit(nLastNonEmpty), ".")
                                                If sVer.Length > 2 AndAlso IsNumeric(sVer(0)) AndAlso IsNumeric(sVer(1)) AndAlso IsNumeric(sVer(2)) Then
                                                    sVersion = sSplit(nLastNonEmpty)
                                                End If
                                            End If
                                    End Select
                                End If
                            Loop
                            bAddInfo = True

                            sr.Close()
                        End If
                    End If
                Case "vbp"
                    'VB6 Project
                    sLocation = sSourcePath
                    dtDate = IO.File.GetLastWriteTime(sFilePath)
                    sType = "Project(VB6)"
                    sVersion = String.Empty

                    Dim sMajor As String = String.Empty
                    Dim sMinor As String = String.Empty
                    Dim sRevision As String = String.Empty

                    sr = System.IO.File.OpenText(sFilePath)
                    Do While sr.Peek() >= 0
                        Dim sLine As String = sr.ReadLine().ToLower
                        Dim nCol As Integer = InStr(1, sLine, "majorver=")
                        If nCol > 0 Then
                            sMajor = Trim(sLine.Substring(nCol + 8))
                        End If
                        nCol = InStr(1, sLine, "minorver=")
                        If nCol > 0 Then
                            sMinor = Trim(sLine.Substring(nCol + 8))
                        End If
                        nCol = InStr(1, sLine, "revisionver=")
                        If nCol > 0 Then
                            sRevision = Trim(sLine.Substring(nCol + 11))
                        End If
                        If sMajor <> "" And sMinor <> "" And sRevision <> "" Then
                            sVersion = Trim(sMajor) & "." & Trim(sMinor) & "." & Trim(sRevision)

                            'Found the entire revision
                            Exit Do
                        End If
                    Loop
                    bAddInfo = True
                    sr.Close()
                    Dim sList As String() = IO.Directory.GetFiles(sSourcePath, "*.frm", IO.SearchOption.TopDirectoryOnly)
                    For Each sFileTmp1 As String In sList
                        Dim dtTmpDate As Date = IO.File.GetLastWriteTime(sFileTmp1)
                        If dtTmpDate > dtDate Then
                            dtDate = dtTmpDate
                        End If
                    Next
                    sList = IO.Directory.GetFiles(sSourcePath, "*.bas", IO.SearchOption.TopDirectoryOnly)
                    For Each sFileTmp1 As String In sList
                        Dim dtTmpDate As Date = IO.File.GetLastWriteTime(sFileTmp1)
                        If dtTmpDate > dtDate Then
                            dtDate = dtTmpDate
                        End If
                    Next
                    sList = IO.Directory.GetFiles(sSourcePath, "*.cls", IO.SearchOption.TopDirectoryOnly)
                    For Each sFileTmp1 As String In sList
                        Dim dtTmpDate As Date = IO.File.GetLastWriteTime(sFileTmp1)
                        If dtTmpDate > dtDate Then
                            dtDate = dtTmpDate
                        End If
                    Next

                Case "vbproj"
                    '.Net Projects
                    sLocation = sSourcePath
                    sName = sFolderTree(sFolderTree.GetUpperBound(0))
                    dtDate = IO.File.GetLastWriteTime(sFilePath)

                    sType = "Project(VB.NET)"
                    sVersion = String.Empty
                    Dim sAssemblyInfo As String = sSourcePath & "\AssemblyInfo.vb"

                    'From Project file
                    sr = System.IO.File.OpenText(sFilePath)
                    Do While sr.Peek() >= 0
                        Dim sLine As String = sr.ReadLine()
                        Dim nLeft As Integer = InStr(1, sLine, "<AssemblyName>")
                        Dim nRight As Integer = InStr(1, sLine, "</AssemblyName>")
                        If (nLeft > 0) And (nRight > 0) Then
                            sName = Trim(sLine.Substring(nLeft + 13, ((nRight - nLeft) - 14)))
                        End If
                        If InStr(1, sLine.ToLower, "assemblyinfo.vb") > 0 Then
                            nLeft = InStr(1, sLine, """")
                            nRight = InStr(nLeft + 1, sLine, """")
                            sAssemblyInfo = sSourcePath & "\" & sLine.Substring(nLeft, ((nRight - nLeft) - 1))
                        End If
                    Loop
                    bAddInfo = True
                    sr.Close()

                    'From AssemblyInfo.vb
                    If IO.File.Exists(sAssemblyInfo) Then
                        sr = System.IO.File.OpenText(sAssemblyInfo)
                        Do While sr.Peek() >= 0
                            Dim sLine As String = sr.ReadLine()
                            Dim nCol As Integer = InStr(1, sLine, "AssemblyVersion")
                            Dim nComment As Integer = InStr(1, sLine, "'")
                            Dim nLeft As Integer = InStr(1, sLine, "(")
                            Dim nRight As Integer = InStr(1, sLine, ")")
                            If (nComment = 0) And (nCol > 0) And (nLeft > 0) And (nRight > 0) Then
                                sVersion = Trim(sLine.Substring(nLeft + 1, ((nRight - nLeft) - 3)))
                            End If
                        Loop
                        bAddInfo = True
                        sr.Close()
                    End If

                    'From frmMain.vb
                    ' 01/25/12  MSW     Check version from frmMain comments against project version
                    Dim sfrmMain As String = sSourcePath & "\frmMain.vb"
                    If IO.File.Exists(sfrmMain) Then
                        sr = System.IO.File.OpenText(sfrmMain)
                        Dim nSearch As Integer = 0
                        Do While sr.Peek() >= 0 And (nSearch < 2)
                            Dim sLine As String = sr.ReadLine()
                            sLine = sLine.Replace(vbTab, " ")
                            Dim sSplit() As String = Split(sLine.Trim, " ")
                            Dim nLastNonEmpty As Integer = -1
                            For nIndex As Integer = 0 To sSplit.Length - 1
                                If sSplit(nIndex) <> "" Then
                                    nLastNonEmpty += 1
                                    sSplit(nLastNonEmpty) = sSplit(nIndex)
                                End If
                            Next
                            If nLastNonEmpty > -1 Then
                                ReDim Preserve sSplit(nLastNonEmpty)
                                Select Case nSearch
                                    Case 0
                                        If sSplit(nLastNonEmpty).ToLower = "version" Then
                                            nSearch = 1
                                        End If
                                    Case 1
                                        If sSplit(0) = "Option" Or sSplit(0) = "Imports" Or _
                                           sSplit(0) = "Public" Or sSplit(0) = "Friend" Then
                                            nSearch = 2
                                        Else
                                            Dim sVer() As String = Split(sSplit(nLastNonEmpty), ".")
                                            If sVer.Length > 2 AndAlso IsNumeric(sVer(0)) AndAlso IsNumeric(sVer(1)) AndAlso IsNumeric(sVer(2)) Then
                                                sVersionAlt = sSplit(nLastNonEmpty)
                                            End If
                                        End If
                                End Select
                            End If
                        Loop
                        bAddInfo = True
                        sr.Close()
                        Dim sList As String() = IO.Directory.GetFiles(sSourcePath, "*.vb", IO.SearchOption.TopDirectoryOnly)
                        For Each sFileTmp1 As String In sList
                            Dim dtTmpDate As Date = IO.File.GetLastWriteTime(sFileTmp1)
                            If dtTmpDate > dtDate Then
                                dtDate = dtTmpDate
                            End If
                        Next
                        sList = IO.Directory.GetFiles(sSourcePath, "*.resx", IO.SearchOption.TopDirectoryOnly)
                        For Each sFileTmp1 As String In sList
                            Dim dtTmpDate As Date = IO.File.GetLastWriteTime(sFileTmp1)
                            If dtTmpDate > dtDate Then
                                dtDate = dtTmpDate
                            End If
                        Next
                        sList = IO.Directory.GetFiles(sSourcePath, "*.cls", IO.SearchOption.TopDirectoryOnly)
                        For Each sFileTmp1 As String In sList
                            Dim dtTmpDate As Date = IO.File.GetLastWriteTime(sFileTmp1)
                            If dtTmpDate > dtDate Then
                                dtDate = dtTmpDate
                            End If
                        Next
                    End If
                Case Else
            End Select


            If bAddInfo Then
                Dim sVerSplit() As String = Split(sVersion, ".")
                sVersion = sVerSplit(0)
                Dim bRed As Boolean = False
                For nIndex As Integer = 1 To sVerSplit.GetUpperBound(0)
                    If sVerSplit(nIndex).Length = 1 Then
                        sVerSplit(nIndex) = "0" & sVerSplit(nIndex)
                    End If
                    sVersion = sVersion & "." & sVerSplit(nIndex)
                Next
                ' 01/25/12  MSW     Check version from frmMain comments against project version
                If sVersionAlt <> String.Empty Then
                    Dim sVerSplitAlt() As String = Split(sVersionAlt, ".")
                    sVersionAlt = sVerSplitAlt(0)
                    For nIndexAlt As Integer = 1 To sVerSplitAlt.GetUpperBound(0)
                        If sVerSplitAlt(nIndexAlt).Length = 1 Then
                            sVerSplitAlt(nIndexAlt) = "0" & sVerSplitAlt(nIndexAlt)
                        End If
                        sVersionAlt = sVersionAlt & "." & sVerSplitAlt(nIndexAlt)
                    Next
                    If sVersionAlt <> sVersion Then
                        sVersion = (sVersion & " (" & sVersionAlt & ")")
                        bRed = True
                    End If
                End If
                nItemCount = nItemCount + 1

                ReDim Preserve tVersionInfo(nItemCount - 1)

                With tVersionInfo(nItemCount - 1)
                    .Name = sName
                    .Type = sType
                    .Version = sVersion
                    .Location = sLocation
                    .dtDate = dtDate
                    .Red = bRed
                End With
            End If
        Next
            Dim sDirectories() As String = IO.Directory.GetDirectories(sSourcePath)

            Status = gpsRM.GetString("psSEARCHING") & sSourcePath
            For Each sDirectory As String In sDirectories
                subLoadVersions(tVersionInfo, nItemCount, sDirectory)
            Next


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

            Progress = 70

            mPrintHtml = New clsPrintHtml(msSCREEN_NAME)
            Progress = 98
            subEnableControls(True)

            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound

            'statusbar text
            Status(True) = gpsRM.GetString("psSELECT_DOC")

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subEnableControls(False)

            ''''''''''''''''''''
            'No compare just yet.
            splMain.Panel1Collapsed = False
            splMain.Panel2Collapsed = True
            subClearScreen()
            Dim sSourcePath As String = String.Empty
            mPWCommon.GetDefaultFilePath(sSourcePath, eDir.Source, String.Empty, String.Empty)
            mnItemCount = 0
            ReDim mtVersionInfo(0)
            subLoadVersions(mtVersionInfo, mnItemCount, sSourcePath)
            subLoadPanel(dgPanel1, mtVersionInfo, mnItemCount)
            lblPanel1.Text = sSourcePath
            DataLoaded = True
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
                                                ByVal e As System.EventArgs)
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
                                            ByVal e As System.EventArgs)
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
                                            ByVal e As System.EventArgs)
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
            Case "btnSave"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If Not (o.DropDownButtonPressed) Then
                    If (DataLoaded) Then
                        subMakePrintDoc(-1, True)
                        'mPrintHtml.subCreateSimpleDoc(dgPanel1, Status, msSCREEN_NAME & " - " & lblPanel1.Text, , , True)
                    End If
                End If
            Case "btnPrint"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If Not (o.DropDownButtonPressed) Then
                    subMakePrintDoc()
                    mPrintHtml.subPrintDoc(False)
                End If
            Case "btnRefresh"
                Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
                subClearScreen()
                splMain.Panel1Collapsed = False
                Dim sSourcePath As String = String.Empty
                mPWCommon.GetDefaultFilePath(sSourcePath, eDir.Source, String.Empty, String.Empty)
                mnItemCount = 0
                ReDim mtVersionInfo(0)
                subLoadVersions(mtVersionInfo, mnItemCount, sSourcePath)
                subLoadPanel(dgPanel1, mtVersionInfo, mnItemCount)
                lblPanel1.Text = sSourcePath
                If mbCompareOpen Then
                    mnCmpItemCount = 0
                    ReDim mtCmpVersionInfo(0)
                    subLoadVersions(mtCmpVersionInfo, mnCmpItemCount, msCompareFolder)
                    subLoadPanel(dgPanel2, mtCmpVersionInfo, mnCmpItemCount)
                    lblPanel2.Text = msCompareFolder
                    splMain.Panel2Collapsed = False
                    subCompareTables()
                    subLoadPanel(dgPanel1, mtVersionInfo, mnItemCount)
                    subLoadPanel(dgPanel2, mtCmpVersionInfo, mnCmpItemCount)
                Else
                    msCompareFolder = String.Empty
                    splMain.Panel2Collapsed = True
                End If
                DataLoaded = True
                subEnableControls(True)
            Case "btnOpen"
                Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
                'Compare
                If mbCompareOpen Then
                    mbCompareOpen = False
                    msCompareFolder = String.Empty
                    splMain.Panel2Collapsed = True
                Else
                    Dim oFB As New FolderBrowserDialog
                    Dim sPath As String = String.Empty

                    With oFB
                        .ShowNewFolderButton = False
                        .Description = gpsRM.GetString("psSELECT_COMPARE_FOLDER")
                        .SelectedPath = sPath
                        .ShowDialog()
                        sPath = .SelectedPath
                    End With

                    If sPath <> String.Empty Then
                        Dim sF() As String = IO.Directory.GetDirectories(sPath)
                        If UBound(sF) > 1 Then
                            'Limited restrictions for a valid folder to search
                            mbCompareOpen = True
                            msCompareFolder = sPath
                            mnCmpItemCount = 0
                            ReDim mtCmpVersionInfo(0)
                            subLoadVersions(mtCmpVersionInfo, mnCmpItemCount, msCompareFolder)
                            subLoadPanel(dgPanel2, mtCmpVersionInfo, mnCmpItemCount)
                            lblPanel2.Text = msCompareFolder
                            splMain.Panel2Collapsed = False
                            subCompareTables()
                            subLoadPanel(dgPanel1, mtVersionInfo, mnItemCount)
                            subLoadPanel(dgPanel2, mtCmpVersionInfo, mnCmpItemCount)
                        End If
                    End If
                End If
                DataLoaded = True
                subEnableControls(True)
        End Select

        subEnableControls(True)
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
                    subLaunchHelp(gs_HELP_UTILITIES_VERSIONS, oIPC)

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

    Private Sub subMakePrintDoc(Optional ByVal nPanel As Integer = -1, Optional ByVal bExport As Boolean = False)
        '********************************************************************************************
        'Description:  common print code
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bCancel As Boolean = False
        mPrintHtml.subStartDoc(Status, msSCREEN_NAME, bExport, bCancel)
        if bCancel = false then
          Dim sTitle(0) As String
          Dim sSubTitle(0) As String
          Select Case nPanel
              Case -1
                  sTitle(0) = gpsRM.GetString("psVERSIONS_CAP")
                  sSubTitle(0) = lblPanel1.Text
                  mPrintHtml.subAddObject(dgPanel1, Status, sTitle, sSubTitle, sTitle(0))
                  If mbCompareOpen Then
                      sTitle(0) = gpsRM.GetString("psVERSIONS_CMP_CAP")
                      sSubTitle(0) = lblPanel2.Text
                      mPrintHtml.subAddObject(dgPanel2, Status, sTitle, sSubTitle, sTitle(0))
                  End If
              Case 1
                  sTitle(0) = gpsRM.GetString("psVERSIONS_CAP")
                  sSubTitle(0) = lblPanel1.Text
                  mPrintHtml.subAddObject(dgPanel1, Status, sTitle, sSubTitle, sTitle(0))
              Case 2
                  sTitle(0) = gpsRM.GetString("psVERSIONS_CMP_CAP")
                  sSubTitle(0) = lblPanel2.Text
                  mPrintHtml.subAddObject(dgPanel2, Status, sTitle, sSubTitle, sTitle(0))
  
          End Select
  
          mPrintHtml.subCloseFile(Status)
        end if
    End Sub
    Private Sub mnuPreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintPreview.Click
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
        subMakePrintDoc()
        mPrintHtml.subShowPrintPreview()
    End Sub

    Private Sub mnuPageSetup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPageSetup.Click
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
        subMakePrintDoc()
        mPrintHtml.subShowPageSetup()
    End Sub

    Private Sub mnuPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrint.Click
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
        subMakePrintDoc()
        mPrintHtml.subPrintDoc(False)
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
        ' 10/07/10  MSW	    Print function clean up
        '********************************************************************************************
        subMakePrintDoc()
        mPrintHtml.subSaveAs()
    End Sub


    Private Sub mnuSaveCmpCSV_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuSaveCmpCSV.Click
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
        subMakePrintDoc(2, True)
        'mPrintHtml.subCreateSimpleDoc(dgPanel2, Status, msSCREEN_NAME & " - " & lblPanel2.Text, , , True)
    End Sub

    Private Sub mnuSaveCSV_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuSaveCSV.Click
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
        subMakePrintDoc(1, True)
        'mPrintHtml.subCreateSimpleDoc(dgPanel1, Status, msSCREEN_NAME & " - " & lblPanel1.Text, , , True)
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
        If (DataLoaded) Then
            Dim lRet As DialogResult

            lRet = MessageBox.Show(gcsRM.GetString("csSAVEMSG1") & vbCrLf & _
                                gcsRM.GetString("csSAVEMSG"), gcsRM.GetString("csSAVE_CHANGES"), _
                                MessageBoxButtons.YesNoCancel, _
                                MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

            Select Case lRet
                Case Response.Yes 'Response.Yes
                    mbInAskForSave = True
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

    Private Sub dgPanel_Scroll(ByVal sender As Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles dgPanel1.Scroll, dgPanel2.Scroll
        '********************************************************************************************
        'Description:  sync scrollbars
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If DataLoaded = False Then Exit Sub
        If mbCompareOpen = False Then Exit Sub
        Try
            Dim odgScroller As DataGridView = DirectCast(sender, DataGridView)

            Select Case odgScroller.Name
                Case dgPanel1.Name
                    dgPanel2.FirstDisplayedScrollingColumnIndex = odgScroller.FirstDisplayedScrollingColumnIndex
                    dgPanel2.FirstDisplayedScrollingRowIndex = odgScroller.FirstDisplayedScrollingRowIndex
                Case dgPanel2.Name
                    dgPanel1.FirstDisplayedScrollingColumnIndex = odgScroller.FirstDisplayedScrollingColumnIndex
                    dgPanel1.FirstDisplayedScrollingRowIndex = odgScroller.FirstDisplayedScrollingRowIndex
            End Select
        Catch ex As Exception

        End Try
    End Sub

End Class