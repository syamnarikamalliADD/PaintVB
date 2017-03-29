' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2008
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
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Fanuc Programmer
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    01/07/10   MSW     dtGetProductionSummDataTable - Merge entries per job
'    01/08/10   MSW     dtGetProductionSummDataTable - Switch from Carrier to Sequence number for merging job reports
'    01/11/10   MSW     strip out commas for csv export
'    02/02/10   RJO     In subBuildReport Summary rows found was sometimes wrong depending on report  
'                       and summary type. Added code to calculate Summary Rows Found.
'    09/14/11   MSW     Assemble a standard version of everything                       4.1.0.0
'    09/30/11   MSW     call me.show earlier to make startup seem quicker               4.1.0.1
'    10/07/11   MSW     Held by fanuc OS alarms were recording some extra events.       4.1.1.0
'    01/13/12   MSW     Support printing of help page, extract the specific alarm       4.01.01.01
'                       from the help file to keep it from getting too big.
'    01/24/12   MSW     Updates to placeholder include files                            4.01.01.02
'    02/15/12   MSW     Force 32 bit build for PCDK, update save button with common export code 4.01.01.03
'    03/23/12   RJO     modified for .NET Password and IPC                              4.01.02.00
'    03/28/12   MSW     move more DB tables to XML                                      4.01.03.00
'    04/19/12   MSW     Remove a lot of .ToString calls so the datagrid gets a date 
'                       instead of a string, merge BK's fixes                           4.01.03.01
'    04/30/12   MSW     tlsMain_ItemClicked - Remove submenus from save button          4.01.03.02
'    05/08/12   MSW     remove linked DBs for job status, deal with the bitmasked data  4.01.03.03
'    06/07/12   MSW     Change charts to output a picture without linking to excel      4.01.04.00
'    11/15/12   HGB     Modified frmCriteria to accommodate a multi-zone SA along with  4.01.04.01  
'                       changes to only show the relevant devices for the zone(s) in 
'                       the device combo.
'    01/15/13   RJO     Experienced trouble starting this app with "application         4.01.04.02
'                       framework" enabled and "Make single instance application" 
'                       selected. Added StartupModule.vb to project and made it a 
'                       single instance application that way.
'    03/12/13   RJO     Modified dgvMain_CellClick to read Cause/Remedy text for Robot  4.01.04.03
'                       Alarms from a robot controller rather than from the GUI. Added 
'                       initialization code for this to subInitializeForm.
'    04/16/13   MSW     Add Canadian language files                                     4.01.05.00
'    06/05/13   MSW     Hotedit and vision reports                                      4.01.05.01
'    07/09/13   MSW     Update and standardize logos                                    4.01.05.02
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up  4.01.05.03
'    09/30/13   MSW     Save screenshots as jpegs                                       4.01.06.00
'    10/16/13   MSW     Add index time for sealer                                       4.01.06.01
'    01/06/14   MSW     prevent hang-up when PW isnt running                            4.01.06.02
'    01/14/14   MSW     Replace "PaintTotal 1" with "Paint Total (cc)"                  4.01.06.03
'                       frmHelp - Add some error handling when reading wbHelp.DocumentText
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call                4.01.07.00
'    02/26/14   CBZ     minigrid - Add close menu, fix completion status compare        4.01.07.01
'    04/08/14   MSW     dtGetDowntimeData change from Fairfax. Take a masked alarm if   4.01.07.02
'                       no alarm has been selected yet so conv status alarms can be masked
'               MSW     Add search function to data grid
'    05/20/14   MSW     Some weekly report cleanup                                      4.01.07.03
'                       Change SaveAs calls so they use .XLSX files that can be opened on other versions
'                       Catch the case where charts calls TopxxRecords and gets a different table
'                       sGetCompStat - Unfix - this may depend on the PLC setup. 
'    07/30/14   MSW     Autorun and daily option development                            4.01.07.04
'******************************************************************************************************
Option Compare Text
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Configuration.ConfigurationSettings
Imports Response = System.Windows.Forms.DialogResult
Imports System.Xml
Imports System.Xml.XPath

Imports System
Imports System.IO

Imports System.text

Friend Class frmMain

#Region " Declares "
    '******** Form Constants   **********************************************************************
    '  msSCREEN_NAME = root namespace or you won't find the resources
    Public Const msSCREEN_NAME As String = "Reports"   ' <-- For password area change log etc.
    Public Const msSCREEN_DUMP_NAME As String = "Reports"
    Private Const mnMAININX As Integer = 100
    Private Const mnSCREENINX As Integer = 101
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    Friend Const mbUSEROBOTS As Boolean = False
    Friend Const mnMAX_TITLES As Integer = 11
    Private Const msHOURCOL As String = "HourColumn"
    Private Const msCOUNTCOL As String = "CountColumn"
    Private msCulture As String = "en-US" 'Default to English
    Friend mPrintHtml As clsPrintHtml
    Private DTChart0 As DataTable = Nothing
    Private DTChart1 As DataTable = Nothing
    Private DTChart2 As DataTable = Nothing
    Friend gCompStats As New Collection(Of clsCompStat)
    Friend gbTimePickersInited As Boolean = False
    Private mbFillScreen As Boolean = True
    Private mnShowPie As Integer = 0 '0=no,1=DT,2=single Chart 3=both charts
    Friend Enum eReportType
        Unknown
        Alarm
        Change
        Conveyor
        Downtime
        Production
        RMCharts
        RMChartsAuto
        Vision
    End Enum
    '******** End Form Constants    *****************************************************************

    '******** Form Types       **********************************************************************
    Friend Structure tConvAlm
        Dim Running As String
        Dim RunningOS As String
        Dim HeldByOthers As String
        Dim HeldByOthersOS As String
        Dim HeldByFanuc As String
        Dim HeldByFanucOS As String
        Dim BreakActive As String
        Dim BreakActiveOS As String
        Dim HoldForHome() As String
        Dim Bypassed() As String
    End Structure
    Friend gtConvAlarms As tConvAlm

    Private Structure tSummary
        Dim Alarm As String
        Dim Description As String
        Dim Zone As String
        Dim Device As String
        Dim Duration As Long
        Dim Occurrences As Integer
        Dim OverAllSec As Long
    End Structure
    '******** End Form Types   **********************************************************************

    '******** Form Variables   **********************************************************************
    Friend Shared gsChangeLogArea As String = Strings.Replace(msSCREEN_NAME, "_", " ")
    Friend colZones As clsZones = Nothing
    Friend mtControllerArray() As tController = Nothing
    Private mbRemoteZone As Boolean = False
    Private mbEventBlocker As Boolean = False
    Private mbScreenLoaded As Boolean = False
    Private mClickedCell As DataGridViewCell
    Private msDownTimeMask As String()  'MSW 8/28/09 - mask out some alarms that aren't really  the first one
    Private mdCmdDate As Date
    Private mbGotScreenList As Boolean 'RJO 03/23/12
    Private mbGotUserList As Boolean 'RJO 03/23/12
    Private msHelpPath As String 'RJO 03/12/13
    Private msFanucName As String 'RJO 03/12/13
    Private msAutoGenFormat As String = ".CSV"
    Public gbAutoRun As Boolean = False
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private mbEditsMade As Boolean = False
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean = False
    Private mnProgress As Integer = 0
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    Private mReportType As eReportType
    Private mStartingReportType As eReportType
    Private mnTotalBreakTime As Long
    Private mfrmCauseRemedy As frmHelp = Nothing
    Private msHelpFormTitle As String = "Alarm Cause/Remedy Information" 'Default to English
    Private msScreenNames(0) As String 'RJO 03/23/12
    Private msScreenDisplayNames(0) As String 'RJO 03/23/12
    Private msUserNames(0) As String 'RJO 03/23/12
    Private moChartType As eChartTypes = eChartTypes.Pie
    Private moChart2Type As eChartTypes = eChartTypes.Pie
    Private msChartTitle(2) As String
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
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet) 'RJO 03/23/12


    Private mnSearchCol As Integer = 0
    Private mnSearchRow As Integer = 0
    Private msSearchText As String = String.Empty
    Private mbSearchBackward As Boolean = False

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
    Friend ReadOnly Property ScreenNames() As String()
        '********************************************************************************************
        'Description:  List of screens registered in Password.NET database
        '
        'Parameters: none
        'Returns:    ScreenNames Array
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msScreenNames
        End Get
    End Property
    Friend ReadOnly Property ScreenDisplayNames() As String()
        '********************************************************************************************
        'Description:  List of screens registered in Password.NET database
        '
        'Parameters: none
        'Returns:    ScreenNames Array
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msScreenDisplayNames
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

    Friend Property ReportType() As eReportType
        '********************************************************************************************
        'Description: This must be set at startup
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mReportType
        End Get
        Set(ByVal value As eReportType)
            mReportType = value
        End Set
    End Property
    Friend ReadOnly Property UserNames() As String()
        '********************************************************************************************
        'Description:  List of users registered in Password.NET database
        '
        'Parameters: none
        'Returns:    UserNames Array
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msUserNames
        End Get
    End Property

#End Region

#Region " Routines "
    Friend Sub subSearch(ByRef sSearchText As String, ByVal bBackwards As Boolean)
        '********************************************************************************************
        'Description: find function
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '    04/17/14   MSW     Add find function
        '********************************************************************************************
        Try
            msSearchText = sSearchText
            mbSearchBackward = bBackwards
            If dgvMain.CurrentCell IsNot Nothing Then
                mnSearchRow = dgvMain.CurrentCell.RowIndex
                mnSearchCol = dgvMain.CurrentCell.ColumnIndex
            End If
            Dim nRowCount As Integer = dgvMain.RowCount
            Dim nColCount As Integer = dgvMain.ColumnCount
            Dim bFound As Boolean = False
            Dim nRow As Integer
            Dim nCol As Integer
            For nRowOffset As Integer = 1 To nRowCount
                If bBackwards Then
                    nRow = mnSearchRow - nRowOffset
                    If nRow < 0 Then
                        nRow = nRow + nRowCount
                    End If
                Else
                    nRow = mnSearchRow + nRowOffset
                    If nRow > nRowCount - 1 Then
                        nRow = nRow - nRowCount
                    End If
                End If
                Dim oRow As DataGridViewRow = dgvMain.Rows(nRow)
                For nColOffset As Integer = 1 To nColCount
                    If bBackwards Then
                        nCol = mnSearchCol - nColOffset
                        If nCol < 0 Then
                            nCol = nCol + nColCount
                        End If
                    Else
                        nCol = mnSearchCol + nColOffset
                        If nCol > nColCount - 1 Then
                            nCol = nCol - nColCount
                        End If
                    End If
                    Dim sText As String = oRow.Cells(nCol).Value.ToString
                    If InStr(sText, sSearchText, CompareMethod.Text) > 0 Then
                        mnSearchCol = nCol
                        mnSearchRow = nRow
                        bFound = True
                        dgvMain.CurrentCell = oRow.Cells(nCol)
                        Exit For
                    End If
                Next
                If bFound Then
                    Exit For
                End If
            Next
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmMain" & " Routine: subSearch", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
    Private Sub subFind(Optional ByVal bF3 As Boolean = False)
        '********************************************************************************************
        'Description: find function
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '    04/17/14   MSW     Add find function
        '********************************************************************************************
        Try
            If bF3 Then
                If msSearchText <> String.Empty Then
                    subSearch(msSearchText, mbSearchBackward)
                ElseIf frmFind.Visible AndAlso frmFind.SearchText <> String.Empty Then
                    subSearch(frmFind.SearchText, frmFind.Backwards)
                Else
                    If frmFind.Visible Then
                        frmFind.BringToFront()
                    Else
                        frmFind.Show()
                    End If
                End If
            Else
                If frmFind.Visible Then
                    frmFind.BringToFront()
                Else
                    frmFind.Show()
                End If
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmMain" & " Routine: subFind", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
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
        '    04/17/14   MSW     Add find function
        '********************************************************************************************
        Dim sKeyValue As String = String.Empty

        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode
                Case Keys.F1
                    'Help Screen request
                    sKeyValue = "btnHelp"
                    Select Case ReportType
                        Case eReportType.Change
                            subLaunchHelp(gs_HELP_REPORTS_CHANGE, oIPC)
                        Case eReportType.Alarm
                            subLaunchHelp(gs_HELP_REPORTS_ALARM, oIPC)
                        Case eReportType.Conveyor
                            subLaunchHelp(gs_HELP_REPORTS_DOWNTIME, oIPC)
                        Case eReportType.Production
                            subLaunchHelp(gs_HELP_REPORTS_PRODUCTION, oIPC)
                        Case eReportType.Downtime
                            subLaunchHelp(gs_HELP_REPORTS_DOWNTIME, oIPC)
                        Case eReportType.RMCharts, eReportType.RMChartsAuto
                            subLaunchHelp(gs_HELP_REPORTS_RMCHARTS, oIPC)
                        Case eReportType.Vision
                            subLaunchHelp(gs_HELP_REPORTS_VISION, oIPC)
                    End Select
                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty
                    Dim sName As String = msSCREEN_DUMP_NAME
                    Select Case ReportType
                        Case eReportType.Change
                            sName = sName & "_ChangeLog"
                        Case eReportType.Alarm
                            sName = sName & "_Alarms"
                        Case eReportType.Conveyor
                            sName = sName & "_Conveyor"
                        Case eReportType.Production
                            sName = sName & "_Production"
                        Case eReportType.Downtime
                            sName = sName & "_Downtime"
                        Case eReportType.RMCharts, eReportType.RMChartsAuto
                            sName = sName & "_Charts"
                        Case eReportType.Vision
                            sName = sName & "_Vision"
                    End Select
                    If frmCriteria.ReportSummaryType <> String.Empty Then
                        sName = sName & "_" & frmCriteria.ReportSummaryType
                    End If
                    sName = sName & ".jpg"
                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath & sName, Imaging.ImageFormat.Jpeg)
                Case Keys.F3
                    subFind(True)
                Case Keys.F5
                    btnRefresh.Enabled = btnCriteria.Enabled
                    frmCriteria.subExit(True)
                Case Keys.Escape
                    Me.Close()
                Case Else

            End Select
        Else
            If e.Control And e.KeyCode = Keys.F Then
                subFind()
            End If
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
    Private Function bCheckForSplit(ByRef OrigAlarm As DataRow, ByRef SplitAlarm As DataRow, _
                                                            ByVal BreakAlarm As DataRow) As Boolean
        '********************************************************************************************
        'Description: adjust times for break and split alarm around break if needed  
        '
        'Parameters:  alarm, alarm variable to hold split, and the current break alarm
        'Returns:   true if alarm is changed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim dtStart As DateTime = CType(OrigAlarm.Item(gpsRM.GetString("psSTART_TIME_CAP")), DateTime)
        Dim dtEnd As DateTime = CType(OrigAlarm.Item(gpsRM.GetString("psEND_TIME_CAP")), DateTime)
        Dim dtStartB As DateTime = CType(BreakAlarm.Item(gpsRM.GetString("psSTART_TIME_CAP")), DateTime)
        Dim dtEndB As DateTime = CType(BreakAlarm.Item(gpsRM.GetString("psEND_TIME_CAP")), DateTime)

        If (dtStart < dtStartB) And (dtEnd > dtEndB) Then
            'alarm overlaps break - need to split
            SplitAlarm.ItemArray = CType(OrigAlarm.ItemArray.Clone, [Object]())

            OrigAlarm.Item(gpsRM.GetString("psEND_TIME_CAP")) = dtStartB
            OrigAlarm.Item(gpsRM.GetString("psDURATION_CAP")) = _
                            sGetDurationString(dtStart, dtStartB)

            SplitAlarm.Item(gpsRM.GetString("psSTART_TIME_CAP")) = dtEndB
            SplitAlarm.Item(gpsRM.GetString("psDURATION_CAP")) = _
                            sGetDurationString(dtEndB, dtEnd)

            Return True
        End If

        If (dtStart < dtStartB) And (dtEnd >= dtStartB) Then
            ' need to adjust back of alarm
            OrigAlarm.Item(gpsRM.GetString("psEND_TIME_CAP")) = dtStartB
            OrigAlarm.Item(gpsRM.GetString("psDURATION_CAP")) = _
                           sGetDurationString(dtStart, dtStartB)
            SplitAlarm = Nothing
            Return True
        End If

        If (dtEnd > dtEndB) And (dtStart >= dtStartB) Then
            ' need to adjust front of alarm
            OrigAlarm.Item(gpsRM.GetString("psSTART_TIME_CAP")) = dtEndB
            OrigAlarm.Item(gpsRM.GetString("psDURATION_CAP")) = _
                           sGetDurationString(dtEndB, dtEnd)
            SplitAlarm = Nothing
            Return True

        End If

        SplitAlarm = Nothing
        Return False

    End Function
    Private Function drGetBreakDatarow(ByVal Breaktable As DataTable, ByRef StartTime As DateTime, _
                                ByRef EndTime As DateTime, ByVal nPointer As Integer) As DataRow
        '********************************************************************************************
        'Description: get a break alarm at pointer position, if pointer to high return last alarm
        '               'should already be a break table here
        'Parameters:  
        'Returns:   datarow
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/25/09  MSW     Deal with no break messages
        '********************************************************************************************
        Dim drBreak As DataRow
        Dim bTmp As Boolean
        If Breaktable.Rows.Count < 1 Then
            drBreak = Nothing
            Return drBreak  ' 08/25/09  MSW     Deal with no break messages
        End If
        If Breaktable.Rows.Count > nPointer Then
            drBreak = Breaktable.Rows(nPointer)
        Else
            'returns the last break in table if pointer is too high
            drBreak = Breaktable.Rows(Breaktable.Rows.Count - 1)
        End If

        StartTime = CType(drBreak.Item(gpsRM.GetString("psSTART_TIME_CAP")), DateTime)
        ' adjust if necessary
        If StartTime < frmCriteria.StartTime Then
            StartTime = frmCriteria.StartTime
            drBreak.Item(gpsRM.GetString("psSTART_TIME_CAP")) = StartTime
            bTmp = True
        End If

        EndTime = CType(drBreak.Item(gpsRM.GetString("psEND_TIME_CAP")), DateTime)
        ' adjust if necessary
        If EndTime > frmCriteria.EndTime Then
            EndTime = frmCriteria.EndTime
            drBreak.Item(gpsRM.GetString("psEND_TIME_CAP")) = EndTime
            bTmp = True
        End If

        If bTmp Then
            ' adjust if necessary
            drBreak.Item(gpsRM.GetString("psDURATION_CAP")) = sGetDurationString( _
                            CType(drBreak.Item(gpsRM.GetString("psSTART_TIME_CAP")), DateTime), _
                            CType(drBreak.Item(gpsRM.GetString("psEND_TIME_CAP")), DateTime))

        End If

        Return drBreak

    End Function
    Friend Sub subGetDTMask()
        '********************************************************************************************
        'Description: get list of alarms to mask out of downtime cause code
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Move to XML
        '********************************************************************************************
        Const sXMLFILE As String = "ReportConfig"
        Const sXMLTABLE As String = "DownTimeMask"
        Const sXMLNODE As String = "Item"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Dim bSuccess As Boolean = True
        Dim nNode As Integer = 0
        Dim nIndex As Integer = 0
        Dim sTmp As String = String.Empty

        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLFILE & ".XML") Then
            Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)
                ReDim msDownTimeMask(oNodeList.Count - 1)

                For Each oNode As XmlNode In oNodeList
                    sTmp = oNode.Item("Alarm").InnerXml
                    msDownTimeMask(nIndex) = sTmp
                    nIndex += 1
                Next

            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmMain" & " Routine: subGetDTMask", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

            End Try
        End If

    End Sub
    Private Function DTAlarmNotMasked(ByVal sAlarm As String) As Boolean
        '********************************************************************************************
        'Description: check for dt alarm mask
        '
        'Parameters: Name of alarm
        'Returns:    true = use for downtime log
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        sAlarm = Trim(sAlarm)
        For Each sTmp As String In msDownTimeMask
            If sAlarm = sTmp Then
                Return False
            End If
        Next
        Return True
    End Function

    Private Function dtGetConveyorData(ByVal TableName As String) As DataTable
        '********************************************************************************************
        'Description: get conveyor alarms and mask with break alarms
        '
        'Parameters: Name of alarm table
        'Returns:    datatable
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim dtBreak As DataTable
        Dim dtNewTable As New DataTable

        Try
            Dim dtConveyor As DataTable = dtGetConveyorDataTable(TableName)

            If dtConveyor.Rows.Count > 0 Then
                If frmCriteria.GetBreakDataSet Then
                    Status = gpsRM.GetString("psGETTING_BREAK_ALRM")
                    dtBreak = frmCriteria.ReportData.Tables(TableName)
                    If dtBreak.Rows.Count = 0 Then
                        Status = gpsRM.GetString("psNO_BREAK_FOUND")
                        'no breaks found
                        Return dtConveyor
                    End If
                Else
                    'something went south
                    Throw New DataException("Could Not Get Break Alarms")
                End If
            Else
                Return dtConveyor
            End If   'dtConveyor.Rows.Count > 0 

            dtNewTable = dtMergeBreakTable(dtConveyor, dtBreak, True)

            Return dtNewTable

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            dtNewTable = New DataTable
            Return dtNewTable
        End Try

    End Function
    Private Function dtGetConveyorDataTable(ByVal TableName As String) As DataTable
        '********************************************************************************************
        'Description: massage the result set to get rid of oneshot alarms etc.
        '
        'Parameters:  Alarm table name
        'Returns:    DataTable
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/27/09  MSW     Fix up the unknown and oneshot records
        '********************************************************************************************
        Dim dtConveyor As DataTable
        Dim dtFCStart As DateTime = frmCriteria.StartTime
        Dim dtFCEnd As DateTime = frmCriteria.EndTime
        Dim dtNewTable As New DataTable
        Dim dtStart As DateTime
        Dim dtEnd As DateTime
        Dim dtEndLast As DateTime

        Try

            If frmCriteria.GetConveyorDataSet Then
                Status = gpsRM.GetString("psGETTING_CONV_ALARMS")
                dtConveyor = frmCriteria.ReportData.Tables(TableName)
            Else
                Throw New DataException("Could not get conveyor data")
            End If

            dtNewTable = dtConveyor.Clone

            If dtConveyor.Rows.Count < 1 Then Return dtNewTable

            Status = gpsRM.GetString("psFILTER_ONESHOT_ALARMS")
            'Debug.Print("In starting Table")
            For Each dr As DataRow In dtConveyor.Rows

                dtStart = CType(dr.Item(gpsRM.GetString("psSTART_TIME_CAP")), DateTime)
                dtEnd = CType(dr.Item(gpsRM.GetString("psEND_TIME_CAP")), DateTime)
                'trim overhanging alarm times
                If dtStart < dtFCStart Then
                    dtStart = dtFCStart
                    dr.Item(gpsRM.GetString("psSTART_TIME_CAP")) = dtFCStart '.ToString
                    dr.Item(gpsRM.GetString("psDURATION_CAP")) = _
                                       sGetDurationString(dtFCStart, dtEnd)
                End If
                'Special case for OS alarm before the start
                If dtEnd < dtFCStart Then
                    dtEnd = DateAdd(DateInterval.Second, 5, dtFCStart)
                    dr.Item(gpsRM.GetString("psEND_TIME_CAP")) = dtEnd '.ToString
                    dr.Item(gpsRM.GetString("psDURATION_CAP")) = _
                                       sGetDurationString(dtFCStart, dtEnd)
                End If
                If dtEnd > dtFCEnd Then
                    dtEnd = dtFCEnd
                    dr.Item(gpsRM.GetString("psEND_TIME_CAP")) = dtFCEnd '.ToString
                    dr.Item(gpsRM.GetString("psDURATION_CAP")) = _
                                       sGetDurationString(dtStart, dtFCEnd)
                End If

                If dtNewTable.Rows.Count = 0 Then
                    'first record. need to determine state of conveyor and adjust to start time of report
                    Select Case Trim(dr.Item(gpsRM.GetString("psALARM_CAP")).ToString)
                        Case gtConvAlarms.HeldByFanuc, gtConvAlarms.HeldByOthers, gtConvAlarms.Running
                            'this is state at start of report - maybe
                            If dtStart > frmCriteria.StartTime Then
                                'we are missing status - add missing status message
                                Dim o As DataRow = dtNewTable.NewRow
                                o.Item(gpsRM.GetString("psALARM_CAP")) = gpsRM.GetString("psUNKNOWN")
                                o.Item(gpsRM.GetString("psSTART_TIME_CAP")) = dtFCStart
                                o.Item(gpsRM.GetString("psEND_TIME_CAP")) = dtStart '.ToString
                                o.Item(gpsRM.GetString("psDURATION_CAP")) = _
                                        sGetDurationString(dtFCStart, dtStart)
                                o.Item(gpsRM.GetString("psDESC_CAP")) = gpsRM.GetString("psUNKNOWN_CONV_STAT")

                                dtNewTable.Rows.Add(o)

                            End If  'dtStart > frmCriteria.StartTime

                            'add first record
                            dtNewTable.ImportRow(dr)

                        Case Else
                            ' dont want it
                    End Select

                Else
                    'rest of rows

                    Select Case Trim(dr.Item(gpsRM.GetString("psALARM_CAP")).ToString)

                        Case gtConvAlarms.HeldByFanuc, gtConvAlarms.HeldByOthers, gtConvAlarms.Running

                            'make sure we picked up where we left off, or we need an unknown status alarm
                            dtEndLast = CType(dtNewTable.Rows( _
                                    dtNewTable.Rows.Count - 1).Item(gpsRM.GetString("psEND_TIME_CAP")), DateTime)

                            'check if we need unknown
                            Select Case DateDiff(DateInterval.Second, dtEndLast, dtStart)
                                Case 0, 1
                                    'were good
                                Case Is < 0
                                    'this   flags negative values - PLC bug!
                                    Dim o As DataRow = dtNewTable.NewRow
                                    o.Item(gpsRM.GetString("psALARM_CAP")) = "PLC"
                                    o.Item(gpsRM.GetString("psSTART_TIME_CAP")) = dtEndLast '.ToString
                                    o.Item(gpsRM.GetString("psEND_TIME_CAP")) = dtStart '.ToString
                                    o.Item(gpsRM.GetString("psDURATION_CAP")) = _
                                            sGetDurationString(dtEndLast, dtStart)
                                    o.Item(gpsRM.GetString("psDESC_CAP")) = "Check PLC Conveyor Logic"

                                    dtNewTable.Rows.Add(o)

                                Case Else
                                    'need unknown -
                                    Dim o As DataRow = dtNewTable.NewRow
                                    o.Item(gpsRM.GetString("psALARM_CAP")) = gpsRM.GetString("psUNKNOWN")
                                    o.Item(gpsRM.GetString("psSTART_TIME_CAP")) = dtEndLast '.ToString
                                    o.Item(gpsRM.GetString("psEND_TIME_CAP")) = dtStart '.ToString
                                    o.Item(gpsRM.GetString("psDURATION_CAP")) = _
                                            sGetDurationString(dtEndLast, dtStart)
                                    o.Item(gpsRM.GetString("psDESC_CAP")) = gpsRM.GetString("psUNKNOWN_CONV_STAT")

                                    dtNewTable.Rows.Add(o)

                            End Select

                            dtNewTable.ImportRow(dr)

                        Case Else
                            ' dont want it
                    End Select

                End If  'dtNewTable.Rows.Count = 0  

            Next   'dr As DataRow In dtConveyor.Rows
            'Debug.Print("In New Table")
            For Each dr As DataRow In dtNewTable.Rows

                dtStart = CType(dr.Item(gpsRM.GetString("psSTART_TIME_CAP")), DateTime)
                dtEnd = CType(dr.Item(gpsRM.GetString("psEND_TIME_CAP")), DateTime)

            Next
            If dtNewTable.Rows.Count > 0 Then
                'been thru all the records, did we make it to end of report time?  
                dtEndLast = CType(dtNewTable.Rows(dtNewTable.Rows.Count - 1).Item( _
                                                gpsRM.GetString("psEND_TIME_CAP")), DateTime)
            Else
                dtEndLast = dtFCStart
            End If

            If dtEndLast < dtFCEnd Then
                'need to iterate back to last oneshot alarm - assume this is the current state of conveyor
                'but not logged yet
                Dim sDesc As String = String.Empty
                Dim sAlarm As String = String.Empty
                Dim nAlarmIndex As Integer = -1
                For i As Integer = dtConveyor.Rows.Count To 1 Step -1
                    sAlarm = Trim(dtConveyor.Rows(i - 1).Item(gpsRM.GetString("psALARM_CAP")).ToString)
                    Select Case sAlarm
                        Case gtConvAlarms.RunningOS
                            sDesc = gpsRM.GetString("psCUR_CONV_RUNNING")
                            nAlarmIndex = i - 1
                        Case gtConvAlarms.HeldByOthersOS
                            sDesc = gpsRM.GetString("psCUR_CONV_HELD_OTHERS")
                            nAlarmIndex = i - 1
                        Case gtConvAlarms.HeldByFanucOS
                            sDesc = gpsRM.GetString("psCUR_CONV_HELD_FANUC")
                            nAlarmIndex = i - 1
                    End Select

                    If sDesc <> String.Empty Then Exit For
                Next
                If nAlarmIndex > 0 Then
                    'generate record - this should only happen for current conditions so we use current time
                    Dim o As DataRow = dtNewTable.NewRow
                    o.Item(gpsRM.GetString("psSTART_TIME_CAP")) = dtEndLast '.ToString
                    o.Item(gpsRM.GetString("psALARM_CAP")) = sAlarm
                    Dim dtTmp As Date = dtFCEnd
                    If dtFCEnd > Now Then dtTmp = Now

                    o.Item(gpsRM.GetString("psEND_TIME_CAP")) = dtTmp '.ToString
                    o.Item(gpsRM.GetString("psDURATION_CAP")) = _
                            sGetDurationString(dtEndLast, dtTmp)
                    o.Item(gpsRM.GetString("psDESC_CAP")) = dtConveyor.Rows(nAlarmIndex).Item(gpsRM.GetString("psDESC_CAP"))
                    o.Item(gcsRM.GetString("csZONE_CAP")) = dtConveyor.Rows(nAlarmIndex).Item(gcsRM.GetString("csZONE_CAP")).ToString()
                    dtNewTable.Rows.Add(o)
                Else
                    'No downtime
                End If
            End If  ' dtEndLast < dtFCEnd 

            'Debug.Print("In New Table after catching endtime")
            'For Each dr As DataRow In dtNewTable.Rows

            '    dtStart = CType(dr.Item(gpsRM.GetString("psSTART_TIME_CAP")), DateTime)
            '    dtEnd = CType(dr.Item(gpsRM.GetString("psEND_TIME_CAP")), DateTime)
            '    Debug.Print(dr.Item(gpsRM.GetString("psALARM_CAP")).ToString & _
            '        dr.Item(gpsRM.GetString("psDESC_CAP")).ToString & _
            '        "  Start:" & dtStart.ToString & "  End:" & dtEnd.ToString)
            'Next
            Return dtNewTable

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            dtNewTable = New DataTable
            Return dtNewTable
        End Try

    End Function
    Friend Function dtGetDowntimeData(ByVal TableName As String, _
                                                ByVal bIncludeHBO As Boolean) As DataTable
        '********************************************************************************************
        'Description: Get the downtime info, take out the breaks and return a table
        '
        'Parameters:  
        'Returns:    Datatable
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/27/09  MSW     Fix up the unknown and oneshot records
        ' 01/10/10  MSW     Tweaked the search a bit to favor zone errors over robot errors as the DT cause
        ' 10/07/11  MSW     Held by fanuc OS alarms were recording some extra events.  Hack away at it bit
        ' 04/25/12  MSW     Copy details to downtime alarms 
        ' 04/08/14  MSW     dtGetDowntimeData change from Fairfax. Take a masked alarm if
        '                       no alarm has been selected yet so conv status alarms can be masked
        '********************************************************************************************
        Dim dtConveyor As DataTable
        Dim dtBreak As DataTable
        Dim dtNewTable As DataTable
        Dim dtStart As DateTime
        Dim dtEnd As DateTime = frmCriteria.StartTime
        Dim dtAlmStart As DateTime
        Dim nDTPtr As Integer = 0
        Dim drDT As DataRow
        Dim dtFCStart As DateTime = frmCriteria.StartTime

        Try

            Status = gpsRM.GetString("psGETTING_DT_DATA")

            Dim dtDown As DataTable = frmCriteria.ReportData.Tables(TableName)
            'debug
            'Return dtDown

            dtConveyor = dtGetConveyorDataTable(TableName)

            If frmCriteria.GetBreakDataSet Then
                Status = gpsRM.GetString("psGETTING_BREAK_ALRM")
                dtBreak = frmCriteria.ReportData.Tables(TableName)
                'Debug.Print(dtBreak.Rows.Count.ToString)
                'For Each dr As DataRow In dtBreak.Rows
                '    Dim dtT1 As Date = CType(dr.Item(gpsRM.GetString("psSTART_TIME_CAP")), DateTime)
                '    Dim dtT2 As Date = CType(dr.Item(gpsRM.GetString("psEND_TIME_CAP")), DateTime)
                '    Debug.Print(dr.Item(gpsRM.GetString("psALARM_CAP")).ToString & _
                '        dr.Item(gpsRM.GetString("psDESC_CAP")).ToString & _
                '        "  Start:" & dtT1.ToString & "  End:" & dtT2.ToString)
                'Next

            Else
                'something went south
                frmCriteria.ReportType = eReportType.Downtime
                Throw New DataException("Could Not Get Break Alarms")
            End If

            If dtDown.Rows.Count = 0 Or dtConveyor.Rows.Count = 0 Then
                dtNewTable = New DataTable
                Return dtNewTable
            End If

            'table to hold results
            dtNewTable = dtConveyor.Clone

            ' alarm table is latest first
            nDTPtr = dtDown.Rows.Count - 1
            Dim bFirst As Boolean = True
            'Debug.Print("In get downtime data")
            For Each dr As DataRow In dtConveyor.Rows
                'Debug.Print("New Conv Alarmm")
                'Debug.Print(dr.Item(gpsRM.GetString("psALARM_CAP")).ToString & "::" & _
                '    dr.Item(gpsRM.GetString("psDESC_CAP")).ToString & "  " & dr.Item(gpsRM.GetString("psSTART_TIME_CAP")).ToString)
                Dim sAlarmCode As String = Trim(dr.Item(gpsRM.GetString("psALARM_CAP")).ToString)

                Select Case sAlarmCode
                    Case gtConvAlarms.HeldByFanuc, gtConvAlarms.HeldByFanucOS
                        'Prevent double records from OS alarms
                        If (sAlarmCode = gtConvAlarms.HeldByFanuc) Or bFirst Or _
                           Math.Abs(DateDiff(DateInterval.Second, CType(dr.Item(gpsRM.GetString("psSTART_TIME_CAP")), DateTime), dtEnd)) > 2 Then
                            bFirst = False
                            'start of fanuc downtime
                            dtStart = CType(dr.Item(gpsRM.GetString("psSTART_TIME_CAP")), DateTime)
                            dtEnd = CType(dr.Item(gpsRM.GetString("psEND_TIME_CAP")), DateTime)
                            If dtEnd > frmCriteria.EndTime Then
                                dtEnd = frmCriteria.EndTime
                            End If
                            nDTPtr = dtDown.Rows.Count - 1
                            Dim bFoundABetterAlarm As Boolean = False
                            'there should be a downtime alarm within 2 seconds of this alarm in dttable
                            'you're dreaming if you think that's true
                            Do While True
                                ' out of alarms?
                                If nDTPtr < 0 Then
                                    Exit Do
                                End If
                                drDT = dtDown.Rows(nDTPtr)
                                dtAlmStart = CType(drDT.Item(gpsRM.GetString("psSTART_TIME_CAP")), DateTime)
                                'Debug.Print(dtAlmStart.ToString)
                                ' alarm table not checked for time out of range
                                If dtAlmStart < dtFCStart Then
                                    dtAlmStart = dtFCStart
                                End If

                                ' if no alarm is found above - it stays logged as held by fanuc
                                'MSW 8/27/09 - need more slop.  I'm trying 5 seconds instead of 2
                                If Math.Abs(DateDiff(DateInterval.Second, dtStart, dtAlmStart)) <= 5 Then
                                    ''we will call this the downtime alarm
                                    'Debug.Print(drDT.Item(gpsRM.GetString("psALARM_CAP")).ToString & _
                                    '    " - " & drDT.Item(gpsRM.GetString("psDESC_CAP")).ToString & "  " & dr.Item(gpsRM.GetString("psSTART_TIME_CAP")).ToString)
                                    ''MSW 8/28/09 - mask out some alarms that aren't reallythe first one
                                    Dim sAlarm As String = CStr(drDT.Item(gpsRM.GetString("psALARM_CAP")))
                                    If DTAlarmNotMasked(sAlarm) Or (bFoundABetterAlarm = False) Then  'MSW 4/8/14
                                        Debug.Print(" Found")
                                        'Take it if it's the first one found, or it's a PLCZ  ...
                                        If (bFoundABetterAlarm = False) Or (InStr(sAlarm, "PLCZ") > 0) Then
                                            dr.Item(gpsRM.GetString("psALARM_CAP")) = sAlarm
                                            dr.Item(gpsRM.GetString("psDESC_CAP")) = drDT.Item(gpsRM.GetString("psDESC_CAP"))
                                            'dr.Item(gpsRM.GetString("csZONE_CAP")) = drDT.Item(gpsRM.GetString("csZONE_CAP"))
                                            'dr.Item(gpsRM.GetString("csROBOT_CAP")) = drDT.Item(gpsRM.GetString("csROBOT_CAP"))
                                            If frmCriteria.bShowColumn(frmCriteria.eShowCol.VIN) Then
                                                dr.Item(gpsRM.GetString("psVIN_CAP")) = drDT.Item(gpsRM.GetString("psVIN_CAP"))
                                            End If

                                            If frmCriteria.bShowColumn(frmCriteria.eShowCol.Carrier) Then
                                                dr.Item(gpsRM.GetString("psCARRIER_CAP")) = drDT.Item(gpsRM.GetString("psCARRIER_CAP"))
                                            End If

                                            dr.Item(gpsRM.GetString("psSTYLE_CAP")) = drDT.Item(gpsRM.GetString("psSTYLE_CAP"))

                                            If frmCriteria.bShowColumn(frmCriteria.eShowCol.Color) Then
                                                dr.Item(gpsRM.GetString("psCOLOR_CAP")) = drDT.Item(gpsRM.GetString("psCOLOR_CAP"))
                                            End If

                                            bFoundABetterAlarm = True
                                        End If
                                    End If
                                End If


                                'If (DateDiff(DateInterval.Second, dtStart, dtAlmStart)) > 5 And dtAlmStart < dtStart Then
                                '    'past current fault
                                '    Exit Do
                                'End If

                                nDTPtr -= 1
                            Loop
                            'If bFoundABetterAlarm = False Then
                            '    Debug.Print("No alarm found")
                            'End If
                            'For Each oItem As Object In dr.ItemArray

                            '    Debug.Print(oItem.ToString)
                            'Next
                            dtNewTable.ImportRow(dr)
                        End If

                    Case gtConvAlarms.HeldByOthers, gtConvAlarms.HeldByOthersOS

                        If bIncludeHBO Then
                            dtNewTable.ImportRow(dr)
                        End If

                    Case gpsRM.GetString("psUNKNOWN")
                        'start of unknown
                        'Need to work on the PLC or alarm logger
                        dtNewTable.ImportRow(dr)

                    Case Else
                        'dont care
                End Select
                'dtNewTable.ImportRow(dr)
            Next  ' dr


            Return dtMergeBreakTable(dtNewTable, dtBreak, False)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            dtNewTable = New DataTable
            Return dtNewTable
        End Try

    End Function
    Private Function dtMergeBreakTable(ByVal dtAlarms As DataTable, _
                    ByVal dtBreak As DataTable, ByVal bIncludeBreakRecords As Boolean) As DataTable
        '********************************************************************************************
        'Description: massage the alarm table to take out break time from the break table
        '
        'Parameters:  Alarm datatable and break datatable - want the break records included for
        '               conveyor report but not downtime report
        'Returns:    combined datatable
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/25/09  MSW     Deal with no break messages
        '********************************************************************************************

        Dim dtStart As DateTime
        Dim dtEnd As DateTime
        Dim dtStartB As DateTime
        Dim dtEndB As DateTime
        Dim nPtr As Integer = 0
        Dim dtNewTable As New DataTable
        Dim drBreak As DataRow


        Try

            subValidateBreakTable(dtBreak)

            Status = gpsRM.GetString("psADJUST_FOR_BREAK")

            'go thru conveyor alarms and mask out break alarms. data should be sequential at this
            'point, should be no overlapping times in conveyor alarms datatable
            dtNewTable = dtAlarms.Clone

            'get first break
            drBreak = drGetBreakDatarow(dtBreak, dtStartB, dtEndB, 0)

            For Each dr As DataRow In dtAlarms.Rows
                dtStart = CType(dr.Item(gpsRM.GetString("psSTART_TIME_CAP")), DateTime)
                dtEnd = CType(dr.Item(gpsRM.GetString("psEND_TIME_CAP")), DateTime)

                'Advance break pointer?
                If dtStart >= dtEndB Then
                    nPtr += 1
                    If nPtr <= dtBreak.Rows.Count Then
                        'log the last downtime alarm
                        If bIncludeBreakRecords And Not (drBreak Is Nothing) Then dtNewTable.ImportRow(drBreak)
                    End If
                    drBreak = drGetBreakDatarow(dtBreak, dtStartB, dtEndB, nPtr)
                End If

                If (dtStartB <= dtEnd) And (dtEndB >= dtStart) Then
                    'break and alaarm overlap
                    If (dtStart >= dtStartB) And (dtEnd <= dtEndB) Then
                        'toss whole alarm
                    Else
                        'alarm needs to be adjusted or split
                        Dim drOrig As DataRow = dtNewTable.NewRow
                        Dim drNew As DataRow = dtNewTable.NewRow
                        drOrig.ItemArray = CType(dr.ItemArray.Clone, [Object]())

                        Do While Not (drNew Is Nothing)
BadProgramming:
                            bCheckForSplit(drOrig, drNew, drBreak)

                            If (drNew Is Nothing) Then
                                'alarm not split, just adjusted
                                Dim dtStartTmp As Date = CType( _
                                            drOrig.Item(gpsRM.GetString("psSTART_TIME_CAP")), Date)
                                If dtStartTmp < dtStartB Then
                                    'log alarm first
                                    dtNewTable.Rows.Add(drOrig)
                                Else
                                    'log break first
                                    'Advance break pointer?
                                    If CType(drOrig.Item( _
                                            gpsRM.GetString("psEND_TIME_CAP")), DateTime) >= dtEndB Then
                                        nPtr += 1
                                        If nPtr <= dtBreak.Rows.Count Then
                                            'log the last downtime alarm
                                            If bIncludeBreakRecords Then dtNewTable.ImportRow(drBreak)
                                            drBreak = drGetBreakDatarow(dtBreak, dtStartB, dtEndB, nPtr)
                                            'if we just advanced the break, the alarm may span the new break
                                            drNew = dtAlarms.NewRow
                                            GoTo BadProgramming
                                        End If
                                    End If
                                    dtNewTable.Rows.Add(drOrig)
                                End If

                            Else
                                'Alarm was split - log alarm, break - check for resplit 
                                dtNewTable.Rows.Add(drOrig)
                                'Advance break pointer?
                                If CType(drOrig.Item(gpsRM.GetString("psEND_TIME_CAP")), DateTime) >= dtEndB Then
                                    nPtr += 1
                                    If nPtr <= dtBreak.Rows.Count Then
                                        'log the last downtime alarm
                                        If bIncludeBreakRecords Then dtNewTable.ImportRow(drBreak)
                                    End If
                                    drBreak = drGetBreakDatarow(dtBreak, dtStartB, dtEndB, nPtr)
                                End If
                                'move new split to orig variable
                                drOrig = dtNewTable.NewRow
                                drOrig.ItemArray = CType(drNew.ItemArray.Clone, [Object]())
                                drNew = dtAlarms.NewRow
                            End If  '(drNew Is Nothing)

                        Loop

                    End If  '(dtStart >= dtStartB) And (dtEnd <= dtEndB) 

                Else

                    'nothin new here
                    dtNewTable.ImportRow(dr)

                End If 'dtStartB <= dtStart
            Next

            'did we log last break? nptr should already be advanced
            If nPtr < dtBreak.Rows.Count Then
                'log the last downtime alarm
                drBreak = drGetBreakDatarow(dtBreak, dtStartB, dtEndB, nPtr)
                If bIncludeBreakRecords Then dtNewTable.ImportRow(drBreak)
            End If

            Return dtNewTable

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            dtNewTable = New DataTable
            Return dtNewTable
        End Try

    End Function
    Private Function dtGetProductionSummDataTable(ByVal TableName As String, _
                                                            ByVal SummType As String) As DataTable
        '********************************************************************************************
        'Description: until we can figure out t-sql better we need to massage the result set for
        '               some of the production summary reports. create and populate it here
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/07/10  MSW     dtGetProductionSummDataTable - Merge entries per job
        ' 01/08/10  MSW     dtGetProductionSummDataTable - Switch from Carrier to Sequence number for merging job reports
        ' 01/13/11  RJO     handle time exactly on the hour
        '********************************************************************************************
        Dim sHourCaption As String = "Plant Style"
        Dim sFieldName As String = "Plant Style"
        Dim sFormat As String = gpsRM.GetString("psDATETIME_FORMAT2")
        Dim lTmp As Long
        Dim dtNewTable As New DataTable

        Try

            'Merge all entries for each job.

            Dim DT As DataTable = frmCriteria.ReportData.Tables(TableName)
            Dim nRowNum As Integer = 1
            Dim nMaxRow As Integer = DT.Rows.Count - 1

            Dim sIndexCol As String = gpsRM.GetString("psSEQUENCE_CAP")
            Do While nRowNum < nMaxRow
                Dim nRowloop As Integer = 0
                Do While nRowloop <= nRowNum - 1
                    If DT.Rows(nRowNum).Item(sIndexCol).ToString = DT.Rows(nRowloop).Item(sIndexCol).ToString Then
                        'Same carrier number
                        Dim dtCompTime1 As DateTime = CType(DT.Rows(nRowNum).Item(gpsRM.GetString("psCOMPLETE_TIME_CAP")), DateTime)
                        Dim dtCompTime2 As DateTime = CType(DT.Rows(nRowloop).Item(gpsRM.GetString("psCOMPLETE_TIME_CAP")), DateTime)
                        If DateDiff(DateInterval.Hour, dtCompTime1, dtCompTime2) <= 1 Then
                            'merge these entries
                            If frmCriteria.bShowColumn(frmCriteria.eShowCol.PaintTotal) Then
                                Dim nPaintTotal As Integer = CInt(DT.Rows(nRowloop).Item(gpsRM.GetString("psPAINT_TOTAL_CAP")))
                                nPaintTotal += CInt(DT.Rows(nRowNum).Item(gpsRM.GetString("psPAINT_TOTAL_CAP")))
                                DT.Rows(nRowloop).Item(gpsRM.GetString("psPAINT_TOTAL_CAP")) = nPaintTotal.ToString
                            End If

                            If dtCompTime1 > dtCompTime2 Then
                                'take the later completion time
                                DT.Rows(nRowloop).Item(gpsRM.GetString("psCOMPLETE_TIME_CAP")) = dtCompTime1 '.ToString
                            End If
                            Dim sCompStat1 As String = CStr(DT.Rows(nRowNum).Item(gpsRM.GetString("psJOB_STATUS_CAP")))
                            Dim sCompStat2 As String = CStr(DT.Rows(nRowloop).Item(gpsRM.GetString("psJOB_STATUS_CAP")))
                            'BTK 11/22/10 Prioritized the job complete status.  If one robot is bypassed or in maintenance show the status of the other robots.
                            If sCompStat1 <> "Bypassed" And sCompStat1 <> "Ghost - Bypassed" And sCompStat1 <> "Maintenance Mode" And sCompStat1 <> "Ghost - Maintenance Mode" Then
                                DT.Rows(nRowloop).Item(gpsRM.GetString("psJOB_STATUS_CAP")) = sCompStat1.ToString
                            Else
                                DT.Rows(nRowNum).Item(gpsRM.GetString("psJOB_STATUS_CAP")) = sCompStat2.ToString
                            End If
                            DT.Rows.RemoveAt(nRowNum)
                            'Reset max count here instead of incrementing current count
                            nMaxRow = DT.Rows.Count - 1
                            nRowNum = nRowNum - 1
                        Else
                            'Next 
                            nRowloop += 1
                        End If
                    Else
                        'Next 
                        nRowloop += 1
                    End If
                Loop
                nRowNum += 1
            Loop

            Select Case SummType
                Case gpsRM.GetString("psSTYLE_CAP")
                    sHourCaption = gpsRM.GetString("psSTYLE_CAP")
                    sFieldName = gpsRM.GetString("psSTYLE_CAP")
                Case gpsRM.GetString("psCOLOR_CAP")
                    sHourCaption = gpsRM.GetString("psCOLOR_CAP")
                    sFieldName = gpsRM.GetString("psCOLOR_CAP")
                Case gpsRM.GetString("psPAINT_TOTAL_CAP")
                    sHourCaption = gpsRM.GetString("psCOLOR_CAP")
                    sFieldName = gpsRM.GetString("psCOLOR_CAP")
            End Select

            Dim bFoundCol As Boolean
            Dim bFoundRow As Boolean
            Select Case SummType
                Case gpsRM.GetString("psSTYLE_CAP"), gpsRM.GetString("psCOLOR_CAP"), _
                        gpsRM.GetString("psPAINT_TOTAL_CAP")
                    Dim bDaily As Boolean = frmCriteria.Daily
                    Dim tsStartTime As TimeSpan = frmCriteria.StartTime.TimeOfDay
                    Dim tsEndTime As TimeSpan = frmCriteria.EndTime.TimeOfDay
                    Dim bOvernight As Boolean = (tsEndTime < tsStartTime)
                    '  style or color summary report
                    dtNewTable.Columns.Add(sHourCaption, GetType(DateTime))
                    Dim dtStart As DateTime = frmCriteria.StartTime
                    Dim dtEnd As DateTime = frmCriteria.EndTime
                    Dim nHours As Integer = CInt(DateDiff(DateInterval.Hour, dtStart, frmCriteria.EndTime))
                    Dim dtTimes(nHours + 1) As DateTime
                    dtTimes(0) = dtStart

                    dtNewTable.Rows.Add(dtTimes(0)) '.ToShortDateString & " " & dtTimes(0).ToShortTimeString)

                    Dim nHourIdx As Integer = 0

                    For nHour As Integer = 1 To nHours
                        nHourIdx += 1
                        dtTimes(nHourIdx) = DateAdd(DateInterval.Hour, 1, dtTimes(nHourIdx - 1))
                        If bDaily Then
                            If dtTimes(nHourIdx) >= frmCriteria.EndTime Then
                                Exit For
                            Else
                                If bOvernight Then
                                    If (dtTimes(nHourIdx).TimeOfDay >= tsEndTime) And (dtTimes(nHourIdx).TimeOfDay < tsStartTime) Then
                                        dtTimes(nHourIdx) = New DateTime(dtTimes(nHourIdx - 1).Year, dtTimes(nHourIdx - 1).Month, dtTimes(nHourIdx - 1).Day, _
                                                                frmCriteria.StartTime.Hour, frmCriteria.StartTime.Minute, frmCriteria.StartTime.Second)
                                    End If
                                Else
                                    If (dtTimes(nHourIdx).TimeOfDay >= tsEndTime) Or (dtTimes(nHourIdx).TimeOfDay < tsStartTime) Then
                                        Dim dtTemp As DateTime = DateAdd(DateInterval.Day, 1, dtTimes(nHourIdx - 1))
                                        dtTimes(nHourIdx) = New DateTime(dtTemp.Year, dtTemp.Month, dtTemp.Day, _
                                                                frmCriteria.StartTime.Hour, frmCriteria.StartTime.Minute, frmCriteria.StartTime.Second)
                                    End If
                                End If

                            End If
                        End If
                        dtNewTable.Rows.Add(dtTimes(nHourIdx)) '.ToShortDateString & " " & dtTimes(nHourIdx).ToShortTimeString)
                    Next
                    nHours = nHourIdx
                    dtTimes(nHours + 1) = dtEnd
                    ReDim Preserve dtTimes(nHours + 1)
                    dtNewTable.AcceptChanges()

                    For Each dr As DataRow In DT.Rows
                        bFoundRow = False
                        bFoundCol = False

                        'figure out which row it should be in
                        Dim dtCompTime As DateTime = CType(dr.Item(gpsRM.GetString("psCOMPLETE_TIME_CAP")), DateTime)
                        Dim nRow As Integer = -1
                        For nHour As Integer = 0 To nHours
                            If dtCompTime >= dtTimes(nHour) And dtCompTime < dtTimes(nHour + 1) Then
                                nRow = nHour
                                Exit For
                            End If
                        Next
                        Dim sCurItem As String = dr.Item(sFieldName).ToString
                        For Each c As DataColumn In dtNewTable.Columns
                            If sCurItem = c.ColumnName Then
                                bFoundCol = True
                                Exit For
                            End If
                        Next  'c As DataColumn In dtNewTable.Columns
                        If Not bFoundCol Then
                            dtNewTable.Columns.Add(sCurItem)
                            For Each r As DataRow In dtNewTable.Rows
                                r.Item(sCurItem) = 0
                            Next   'r As DataRow In dtNewTable.Rows
                        End If
                        Dim nval As Integer = 0
                        Try
                            nval = CInt(dtNewTable.Rows.Item(nRow).Item(sCurItem))
                        Catch
                            nval = 0 'Catch null value
                        End Try


                        Select Case SummType
                            Case gpsRM.GetString("psSTYLE_CAP")
                                nval += 1
                            Case gpsRM.GetString("psCOLOR_CAP")
                                nval += 1
                            Case gpsRM.GetString("psPAINT_TOTAL_CAP")
                                nval += CInt(dr.Item(gpsRM.GetString("psPAINT_TOTAL_CAP")))
                        End Select
                        dtNewTable.Rows.Item(nRow).Item(sCurItem) = nval
                        'For Each r As DataRow In dtNewTable.Rows
                        '    If r.Item(sHourCaption).ToString = sCurHour Then
                        '        bFoundRow = True
                        '        r.Item(sCurItem) = dr.Item(msCOUNTCOL).ToString
                        '        Exit For
                        '    End If
                        'Next   'r As DataRow In dtNewTable.Rows

                        'If Not bFoundRow Then
                        '    Dim NewRow As DataRow = dtNewTable.Rows.Add
                        '    NewRow.Item(sHourCaption) = sCurHour
                        '    NewRow.Item(sCurItem) = dr.Item(msCOUNTCOL).ToString
                        'End If
                    Next  'dr As DataRow In DT.Rows

                    dtNewTable.AcceptChanges()

                    'total me up, buttercup -----------------------------------------------------
                    dtNewTable.Columns.Add(gpsRM.GetString("psTOTAL_CAP"))

                    For Each r As DataRow In dtNewTable.Rows
                        lTmp = 0
                        'first col should always be time
                        For i As Integer = 1 To dtNewTable.Columns.Count - 2
                            If Not r.Item(i) Is DBNull.Value Then
                                lTmp += CType(r.Item(i), Long)
                            End If
                        Next
                        r.Item(gpsRM.GetString("psTOTAL_CAP")) = lTmp
                    Next   'r As DataRow In dtNewTable.Rows

                    Dim NewRow1 As DataRow = dtNewTable.Rows.Add
                    NewRow1.Item(sHourCaption) = frmCriteria.EndTime 'gpsRM.GetString("psTOTAL_CAP")
                    For Each c As DataColumn In dtNewTable.Columns
                        lTmp = 0
                        If c.Caption <> sHourCaption Then
                            For Each r As DataRow In dtNewTable.Rows
                                If Not r.Item(c) Is DBNull.Value Then
                                    lTmp += CType(r.Item(c), Long)
                                End If
                            Next   'r As DataRow In dtNewTable.Rows
                            NewRow1.Item(c) = lTmp
                        End If
                    Next

                    dtNewTable.AcceptChanges()

                    Return dtNewTable

                Case Else
                    ' take merged table
                    Return DT

            End Select

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            dtNewTable = New DataTable
            Return dtNewTable
        End Try

    End Function
    Private Function dtGetVisionSummDataTable(ByVal TableName As String, _
                                                            ByVal SummType As String) As DataTable
        '********************************************************************************************
        'Description: until we can figure out t-sql better we need to massage the result set for
        '               some of the production summary reports. create and populate it here
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sHourCaption As String = "Plant Style"
        Dim sFieldName As String = "Plant Style"
        Dim sFormat As String = gpsRM.GetString("psDATETIME_FORMAT2")
        Dim lTmp As Long
        Dim dtNewTable As New DataTable

        Try

            'Merge all entries for each job.

            Dim DT As DataTable = frmCriteria.ReportData.Tables(TableName)
            Dim nRowNum As Integer = 1
            Dim nMaxRow As Integer = DT.Rows.Count - 1


            Select Case SummType
                Case gpsRM.GetString("psSTYLE_CAP")
                    sHourCaption = gpsRM.GetString("psSTYLE_CAP")
                    sFieldName = "Plant Style"
                Case gpsRM.GetString("psVISION_STATUS_CAP")
                    sHourCaption = gpsRM.GetString("psVISION_STATUS_CAP")
                    sFieldName = "Part Status"
            End Select

            Dim bFoundCol As Boolean
            Dim bFoundRow As Boolean
            Select Case SummType
                Case gpsRM.GetString("psSTYLE_CAP"), gpsRM.GetString("psVISION_STATUS_CAP")
                    '  style or status summary report
                    dtNewTable.Columns.Add(sHourCaption)
                    Dim dtStart As DateTime = frmCriteria.StartTime
                    dtStart = New DateTime(dtStart.Year, dtStart.Month, dtStart.Day, dtStart.Hour, 0, 0)
                    Dim dtEnd As DateTime = frmCriteria.EndTime
                    If dtEnd.Minute > 0 Then
                        dtEnd = New DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, dtEnd.Hour + 1, 0, 0)
                    Else
                        dtEnd = New DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, dtEnd.Hour, 0, 0)
                    End If
                    Dim nHours As Integer = CInt(DateDiff(DateInterval.Hour, dtStart, frmCriteria.EndTime))
                    Dim dtTimes(nHours + 1) As DateTime
                    dtTimes(0) = dtStart
                    dtNewTable.Rows.Add(dtTimes(0).ToShortTimeString)
                    dtTimes(nHours + 1) = dtEnd
                    For nHour As Integer = 1 To nHours
                        dtTimes(nHour) = DateAdd(DateInterval.Hour, 1, dtTimes(nHour - 1))
                        dtNewTable.Rows.Add(dtTimes(nHour).ToShortTimeString)
                    Next

                    dtNewTable.AcceptChanges()
                    For Each dc As DataColumn In DT.Columns
                        Debug.Print(dc.ColumnName)
                    Next
                    For Each dr As DataRow In DT.Rows
                        bFoundRow = False
                        bFoundCol = False

                        'figure out which row it should be in
                        Dim dtHour As DateTime = CType(dr.Item(msHOURCOL), DateTime)
                        Dim nRow As Integer = -1
                        For nHour As Integer = 0 To nHours
                            If dtHour >= dtTimes(nHour) And dtHour < dtTimes(nHour + 1) Then
                                nRow = nHour
                                Exit For
                            End If
                        Next
                        Dim sCurItem As String = dr.Item(sFieldName).ToString
                        Select Case SummType
                            Case gpsRM.GetString("psSTYLE_CAP")
                                sCurItem = dr.Item(sFieldName).ToString
                            Case gpsRM.GetString("psVISION_STATUS_CAP")
                                sCurItem = sGetVisionStat(dr.Item(sFieldName).ToString)
                        End Select
                        If sCurItem <> String.Empty Then
                            For Each c As DataColumn In dtNewTable.Columns
                                If sCurItem = c.ColumnName Then
                                    bFoundCol = True
                                    Exit For
                                End If
                            Next  'c As DataColumn In dtNewTable.Columns
                            If Not bFoundCol Then
                                dtNewTable.Columns.Add(sCurItem)
                                For Each r As DataRow In dtNewTable.Rows
                                    r.Item(sCurItem) = 0
                                Next   'r As DataRow In dtNewTable.Rows
                            End If
                            Dim nval As Integer = 0
                            Try
                                nval = CInt(dtNewTable.Rows.Item(nRow).Item(sCurItem))
                            Catch
                                nval = 0 'Catch null value
                            End Try
                            Try
                                nval += CType(dr.Item(msCOUNTCOL), Integer)
                            Catch
                                nval = 0 'Catch null value
                            End Try

                            dtNewTable.Rows.Item(nRow).Item(sCurItem) = nval

                        End If
                    Next  'dr As DataRow In DT.Rows

                    dtNewTable.AcceptChanges()

                    'total me up, buttercup -----------------------------------------------------
                    dtNewTable.Columns.Add(gpsRM.GetString("psTOTAL_CAP"))

                    For Each r As DataRow In dtNewTable.Rows
                        lTmp = 0
                        'first col should always be time
                        For i As Integer = 1 To dtNewTable.Columns.Count - 2
                            If Not r.Item(i) Is DBNull.Value Then
                                lTmp += CType(r.Item(i), Long)
                            End If
                        Next
                        r.Item(gpsRM.GetString("psTOTAL_CAP")) = lTmp
                    Next   'r As DataRow In dtNewTable.Rows

                    Dim NewRow1 As DataRow = dtNewTable.Rows.Add
                    NewRow1.Item(sHourCaption) = gpsRM.GetString("psTOTAL_CAP")
                    For Each c As DataColumn In dtNewTable.Columns
                        lTmp = 0
                        If c.Caption <> sHourCaption Then
                            For Each r As DataRow In dtNewTable.Rows
                                If Not r.Item(c) Is DBNull.Value Then
                                    lTmp += CType(r.Item(c), Long)
                                End If
                            Next   'r As DataRow In dtNewTable.Rows
                            NewRow1.Item(c) = lTmp
                        End If
                    Next

                    dtNewTable.AcceptChanges()

                    Return dtNewTable

                Case Else
                    ' take merged table
                    Return DT

            End Select

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            dtNewTable = New DataTable
            Return dtNewTable
        End Try

    End Function
    Friend Function dtSummarizeDataTable(ByVal TableIn As DataTable, _
                                          ByRef ChartConfig As tChartConfig, _
                                          Optional ByRef sSummType As String = "") As DataTable
        '********************************************************************************************
        'Description: summarize an alarm datatable - percent col always there, used to sort
        '
        'Parameters:  
        'Returns:    Datatable
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim tSumm() As tSummary
        Dim bFound As Boolean
        Dim nUB As Integer = 0
        Dim i As Integer
        Dim lTotal As Long
        Dim fPerCentO As Single
        Dim fPerCentD As Single
        Dim fPerCentCmp As Single
        Dim fPerCent2 As Single
        Dim nOccur As Integer = TableIn.Rows.Count
        Dim lEndTime As Date

        ReDim tSumm(nUB)
        If frmCriteria.EndTime > Now Then
            lEndTime = Now
        Else
            lEndTime = frmCriteria.EndTime
        End If
        'tSumm(0).OverAllSec = DateDiff(DateInterval.Second, frmCriteria.StartTime, lEndTime) - mnTotalBreakTime
        Dim TimeDiff As Long = DateDiff(DateInterval.Second, frmCriteria.StartTime, lEndTime)
        If TimeDiff > mnTotalBreakTime Then
            tSumm(0).OverAllSec = TimeDiff - mnTotalBreakTime
        Else
            tSumm(0).OverAllSec = TimeDiff
        End If
        ' In case there are no items:
        ChartConfig.UptimePct = 100
        For Each dr As DataRow In TableIn.Rows
            bFound = False
            For i = 1 To nUB
                If tSumm(i).Alarm = dr.Item(gpsRM.GetString("psALARM_CAP")).ToString Then
                    'alarms match - check zone
                    If tSumm(i).Zone = dr.Item(gcsRM.GetString("csZONE_CAP")).ToString Then
                        bFound = True
                        Exit For
                    End If
                End If
            Next

            If Not bFound Then
                'alarm not in summary yet
                nUB += 1
                ReDim Preserve tSumm(nUB)
                i = nUB
                With tSumm(i)
                    .Alarm = dr.Item(gpsRM.GetString("psALARM_CAP")).ToString
                    .Zone = dr.Item(gcsRM.GetString("csZONE_CAP")).ToString
                    .Description = dr.Item(gpsRM.GetString("psDESC_CAP")).ToString
                    .Duration = 0
                    .OverAllSec = tSumm(0).OverAllSec
                End With
            End If

            'add the times
            Dim lDur As Long = DateDiff(DateInterval.Second, _
                                        CType(dr.Item(gpsRM.GetString("psSTART_TIME_CAP")), DateTime), _
                                        CType(dr.Item(gpsRM.GetString("psEND_TIME_CAP")), DateTime))

            tSumm(i).Duration += lDur
            lTotal += lDur

            tSumm(i).Occurrences += 1

        Next

        Dim dtNewTable As DataTable
        dtNewTable = TableIn.Clone

        If TableIn.Rows.Count = 0 Then Return dtNewTable

        If dtNewTable.Columns.Contains(gpsRM.GetString("psSTART_TIME_CAP")) Then
            dtNewTable.Columns.Remove(gpsRM.GetString("psSTART_TIME_CAP"))
        End If
        If dtNewTable.Columns.Contains(gpsRM.GetString("psEND_TIME_CAP")) Then
            dtNewTable.Columns.Remove(gpsRM.GetString("psEND_TIME_CAP"))
        End If
        If dtNewTable.Columns.Contains(gpsRM.GetString("psCOLOR_CAP")) Then
            dtNewTable.Columns.Remove(gpsRM.GetString("psCOLOR_CAP"))
        End If
        If dtNewTable.Columns.Contains(gpsRM.GetString("psCARRIER_CAP")) Then
            dtNewTable.Columns.Remove(gpsRM.GetString("psCARRIER_CAP"))
        End If
        If dtNewTable.Columns.Contains(gpsRM.GetString("psSTYLE_CAP")) Then
            dtNewTable.Columns.Remove(gpsRM.GetString("psSTYLE_CAP"))
        End If
        If dtNewTable.Columns.Contains(gcsRM.GetString("csZONE_CAP")) Then
            dtNewTable.Columns.Remove(gcsRM.GetString("csZONE_CAP"))
        End If

        dtNewTable.Columns.Add(gpsRM.GetString("psDURATION_PERC_CAP"))
        dtNewTable.Columns.Add(gpsRM.GetString("psOCCURRENCES_CAP"))
        dtNewTable.Columns.Add(gpsRM.GetString("psOCCURRENCES_PERC_CAP"))

        'Stoere these columns as numbers so they're sortable
        dtNewTable.Columns.Item(gpsRM.GetString("psOCCURRENCES_CAP")).DataType = GetType(Integer)
        dtNewTable.Columns.Item(gpsRM.GetString("psDURATION_PERC_CAP")).DataType = GetType(Single)
        dtNewTable.Columns.Item(gpsRM.GetString("psOCCURRENCES_PERC_CAP")).DataType = GetType(Single)
        dtNewTable.Columns.Item(gpsRM.GetString("psDURATION_CAP")).DataType = GetType(Long)

        Dim o As DataRow
        '0 was never used
        For i = 1 To nUB
            o = dtNewTable.NewRow
            o.Item(gpsRM.GetString("psALARM_CAP")) = tSumm(i).Alarm
            'o.Item(gcsRM.GetString("csZONE_CAP")) = tSumm(i).Zone
            o.Item(gpsRM.GetString("psDESC_CAP")) = tSumm(i).Description
            o.Item(gpsRM.GetString("psOCCURRENCES_CAP")) = tSumm(i).Occurrences
            'o.Item(gpsRM.GetString("psDURATION_CAP")) = sGetDurationString(tSumm(i).Duration)
            o.Item(gpsRM.GetString("psDURATION_CAP")) = tSumm(i).Duration

            'If frmCriteria.ReportSummaryType = gpsRM.GetString("psOCCURRENCES_CAP") Then
            If nOccur > 0 Then 'occurrences total
                fPerCentO = CSng(tSumm(i).Occurrences / nOccur)
            Else
                fPerCentO = 0
            End If
            o.Item(gpsRM.GetString("psOCCURRENCES_PERC_CAP")) = fPerCentO 'Format(fPerCent.ToString, "Percent")
            'Else
            'TODO - this is a percent - what do we really want it to be a percent of?
            If tSumm(i).OverAllSec = 0 Then
                fPerCentD = 0
            Else
                ' Changed to use %DT instead of % total, so it's more like the occurrence number
                'fPerCent = CSng(tSumm(i).Duration / tSumm(i).OverAllSec)
                fPerCentD = CSng(tSumm(i).Duration / lTotal)
            End If
            o.Item(gpsRM.GetString("psDURATION_PERC_CAP")) = fPerCentD 'Format(fPerCent.ToString, "Percent")            'End If
            If sSummType = gpsRM.GetString("psOCCURRENCES_CAP") Then
                fPerCentCmp = fPerCentO
            Else
                fPerCentCmp = fPerCentD
            End If


            'lame sort routine follows  most to least
            Dim bFnd As Boolean = False
            If dtNewTable.Rows.Count = 0 Then
                dtNewTable.Rows.Add(o)
                bFnd = True
            Else
                For j As Integer = 0 To dtNewTable.Rows.Count - 1
                    'get the percent val from string in row
                    'fPerCent2 = (CSng(dtNewTable.Rows(j).Item(gpsRM.GetString("psPERCENT_CAP")).ToString))
                    If sSummType = gpsRM.GetString("psOCCURRENCES_CAP") Then
                        fPerCent2 = (CSng(dtNewTable.Rows(j).Item(gpsRM.GetString("psOCCURRENCES_PERC_CAP")).ToString))
                    Else
                        fPerCent2 = (CSng(dtNewTable.Rows(j).Item(gpsRM.GetString("psDURATION_PERC_CAP")).ToString))
                    End If
                    If fPerCentCmp >= fPerCent2 Then
                        dtNewTable.Rows.InsertAt(o, j)
                        bFnd = True
                        Exit For
                    End If
                Next
            End If

            If Not bFnd Then dtNewTable.Rows.Add(o)

        Next

        If ChartConfig.TopxxReq > 0 Then
            Do While dtNewTable.Rows.Count > ChartConfig.TopxxReq
                dtNewTable.Rows.RemoveAt(dtNewTable.Rows.Count - 1)
            Loop
        End If
        ChartConfig.NumAlarms = dtNewTable.Rows.Count
        o = dtNewTable.NewRow
        o.Item(gpsRM.GetString("psALARM_CAP")) = gpsRM.GetString("psTOTAL_CAP")
        'o.Item(gpsRM.GetString("psDURATION_CAP")) = sGetDurationString(lTotal)
        o.Item(gpsRM.GetString("psDURATION_CAP")) = lTotal
        o.Item(gpsRM.GetString("psOCCURRENCES_CAP")) = nOccur
        o.Item(gpsRM.GetString("psOCCURRENCES_PERC_CAP")) = 1
        o.Item(gpsRM.GetString("psDURATION_PERC_CAP")) = 1
        dtNewTable.Rows.Add(o)

        'debug
        o = dtNewTable.NewRow
        o.Item(gpsRM.GetString("psALARM_CAP")) = gpsRM.GetString("psOVERALL")
        'o.Item(gpsRM.GetString("psDURATION_CAP")) = sGetDurationString(DateDiff(DateInterval.Second, _
        '                        frmCriteria.StartTime, lEndTime))
        o.Item(gpsRM.GetString("psDURATION_CAP")) = tSumm(0).OverAllSec
        dtNewTable.Rows.Add(o)
        If tSumm(0).OverAllSec > 0 Then
            ChartConfig.UptimePct = 100 * (tSumm(0).OverAllSec - lTotal) / tSumm(0).OverAllSec
        Else
            ChartConfig.UptimePct = 0
        End If
        If nOccur > 1 Then
            'Mean time to recover = total DT / num of occurrences rounded to sec
            ChartConfig.MTTR = CInt(lTotal \ nOccur)
            'Mean time between failures = total time / num occurancces rounded to sec
            ChartConfig.MTBF = CInt(tSumm(0).OverAllSec \ nOccur)
        Else
            ChartConfig.MTTR = CInt(lTotal)
            ChartConfig.MTBF = CInt(tSumm(0).OverAllSec - lTotal)
        End If
        Return dtNewTable

    End Function
    Friend Function dtConvertVisionTable(ByVal TableIn As DataTable) As DataTable
        '********************************************************************************************
        'Description: convert print values for a vision table
        '
        'Parameters:  
        'Returns:    Datatable
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nUB As Integer = 0
        Dim dtNewTable As DataTable

        dtNewTable = TableIn.Clone

        If TableIn.Rows.Count = 0 Then Return dtNewTable

        dtNewTable.Columns.Item(gpsRM.GetString("psVISION_STATUS_CAP")).DataType = GetType(String)

        Dim o As DataRow
        '0 was never used
        For nRow As Integer = 0 To TableIn.Rows.Count - 1
            o = dtNewTable.NewRow
            For nCol As Integer = 0 To TableIn.Columns.Count - 1
                If dtNewTable.Columns(nCol).ColumnName.Equals(gpsRM.GetString("psVISION_STATUS_CAP")) Then
                    o.Item(nCol) = sGetVisionStat(TableIn.Rows.Item(nRow).Item(gpsRM.GetString("psVISION_STATUS_CAP")).ToString)
                Else
                    o.Item(nCol) = TableIn.Rows.Item(nRow).Item(dtNewTable.Columns(nCol).ColumnName)
                End If
            Next
            dtNewTable.Rows.Add(o)
        Next


        Return dtNewTable
    End Function

    Friend Function dtConvertProdTable(ByVal TableIn As DataTable) As DataTable
        '********************************************************************************************
        'Description: convert print values for a production table
        '
        'Parameters:  
        'Returns:    Datatable
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/03/12  MSW     convert to numeric completion status
        '********************************************************************************************
        Dim nUB As Integer = 0
        Dim dtNewTable As DataTable

        dtNewTable = TableIn.Clone

        If TableIn.Rows.Count = 0 Then Return dtNewTable

        dtNewTable.Columns.Item(gpsRM.GetString("psJOB_STATUS_CAP")).DataType = GetType(String)

        Dim o As DataRow
        '0 was never used
        For nRow As Integer = 0 To TableIn.Rows.Count - 1
            o = dtNewTable.NewRow
            For nCol As Integer = 0 To TableIn.Columns.Count - 1
                If dtNewTable.Columns(nCol).ColumnName.Equals(gpsRM.GetString("psJOB_STATUS_CAP")) Then
                    o.Item(nCol) = sGetCompStat(CType(TableIn.Rows.Item(nRow).Item(gpsRM.GetString("psJOB_STATUS_CAP")), Integer))
                Else
                    o.Item(nCol) = TableIn.Rows.Item(nRow).Item(dtNewTable.Columns(nCol).ColumnName)
                End If
            Next
            dtNewTable.Rows.Add(o)
        Next


        Return dtNewTable
    End Function
    Friend Function dtSummarizeDataTablePrint(ByVal TableIn As DataTable, _
                                          Optional ByRef sSummType As String = "") As DataTable

        '********************************************************************************************
        'Description: summarize an alarm datatable - percent col always there, used to sort
        '
        'Parameters:  
        'Returns:    Datatable
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/17/12  BTK     Added routine to print downtime summary correctly.
        ' 05/24/12  MSW     Adapted for charts
        '********************************************************************************************
        Dim nUB As Integer = 0
        Dim i As Integer
        Dim dtNewTable As DataTable

        dtNewTable = TableIn.Clone

        If TableIn.Rows.Count = 0 Then Return dtNewTable

        dtNewTable.Columns.Item(gpsRM.GetString("psDURATION_CAP")).DataType = GetType(String)
        dtNewTable.Columns.Item(gpsRM.GetString("psOCCURRENCES_CAP")).DataType = GetType(Integer)
        If TableIn.Columns.Contains(gpsRM.GetString("psDURATION_PERC_CAP")) Then
            dtNewTable.Columns.Item(gpsRM.GetString("psDURATION_PERC_CAP")).DataType = GetType(String)
        End If
        If TableIn.Columns.Contains(gpsRM.GetString("psOCCURRENCES_PERC_CAP")) Then
            dtNewTable.Columns.Item(gpsRM.GetString("psOCCURRENCES_PERC_CAP")).DataType = GetType(String)
        End If

        Dim o As DataRow
        '0 was never used
        For i = 0 To TableIn.Rows.Count - 1
            o = dtNewTable.NewRow
            o.Item(gpsRM.GetString("psALARM_CAP")) = TableIn.Rows.Item(i).Item(gpsRM.GetString("psALARM_CAP"))
            o.Item(gpsRM.GetString("psDESC_CAP")) = TableIn.Rows.Item(i).Item(gpsRM.GetString("psDESC_CAP"))
            o.Item(gpsRM.GetString("psDURATION_CAP")) = sGetDurationString(CLng(TableIn.Rows.Item(i).Item(gpsRM.GetString("psDURATION_CAP"))))
            o.Item(gpsRM.GetString("psOCCURRENCES_CAP")) = TableIn.Rows.Item(i).Item(gpsRM.GetString("psOCCURRENCES_CAP"))
            If TableIn.Columns.Contains(gpsRM.GetString("psVIN_CAP")) Then
                If frmCriteria.bShowColumn(frmCriteria.eShowCol.VIN) Then
                    o.Item(gpsRM.GetString("psVIN_CAP")) = TableIn.Rows.Item(i).Item(gpsRM.GetString("psVIN_CAP"))
                End If
            End If
            If TableIn.Columns.Contains(gpsRM.GetString("psOCCURRENCES_PERC_CAP")) Then
                If TableIn.Rows.Item(i).Item(gpsRM.GetString("psOCCURRENCES_PERC_CAP")) IsNot DBNull.Value Then
                    o.Item(gpsRM.GetString("psOCCURRENCES_PERC_CAP")) = Format(TableIn.Rows.Item(i).Item(gpsRM.GetString("psOCCURRENCES_PERC_CAP")), "Percent")
                Else
                    o.Item(gpsRM.GetString("psOCCURRENCES_PERC_CAP")) = Format(0, "Percent")
                End If
            End If
            If TableIn.Columns.Contains(gpsRM.GetString("psDURATION_PERC_CAP")) Then
                If TableIn.Rows.Item(i).Item(gpsRM.GetString("psDURATION_PERC_CAP")) IsNot DBNull.Value Then
                    o.Item(gpsRM.GetString("psDURATION_PERC_CAP")) = Format(TableIn.Rows.Item(i).Item(gpsRM.GetString("psDURATION_PERC_CAP")), "Percent")
                Else
                    o.Item(gpsRM.GetString("psDURATION_PERC_CAP")) = Format(0, "Percent")
                End If
            End If

            dtNewTable.Rows.Add(o)

        Next


        Return dtNewTable

    End Function

    Friend Function sGetDurationString(ByVal dtStart As Date, ByVal dtEnd As Date) As String
        '********************************************************************************************
        'Description: return as string for the difference in incoming times in HH:MM:SS
        '
        'Parameters:  times
        'Returns:     HH:MM:SS
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim lSec As Long = DateDiff(DateInterval.Second, dtStart, dtEnd)
        Return sGetDurationString(lSec)

    End Function
    Friend Function sGetDurationString(ByVal lSeconds As Long) As String
        '********************************************************************************************
        'Description: return as string for the   incoming times in HH:MM:SS
        '
        'Parameters:  seconds
        'Returns:     HH:MM:SS
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sHour As String = String.Empty
        Dim sMin As String = String.Empty
        Dim sSec As String = String.Empty


        If lSeconds > 3559 Then
            Dim ltmp As Long = lSeconds \ 3600
            sHour = ltmp.ToString & ":"
            lSeconds -= (ltmp * 3600)
        Else
            sHour = "0:"
        End If

        If lSeconds > 59 Then
            Dim ltmp As Long = lSeconds \ 60
            sMin = ltmp.ToString
            lSeconds -= (ltmp * 60)
            If Len(sMin) = 1 Then
                sMin = "0" & sMin & ":"
            Else
                sMin = sMin & ":"
            End If
        Else
            sMin = "00:"
        End If

        If lSeconds >= 0 Then
            sSec = lSeconds.ToString

            If Len(sSec) = 1 Then
                sSec = "0" & sSec
            End If

            Return sHour & sMin & sSec
        Else
            ' this is just to flag a bug
            Return "-" & sHour & sMin & System.Math.Abs(lSeconds).ToString
        End If

    End Function
    Private Function GetDurationDate(ByVal lSeconds As Long) As Date
        '********************************************************************************************
        'Description: return as Date for the   incoming times in HH:MM:SS
        '
        'Parameters:  seconds
        'Returns:     HH:MM:SS
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nHour As Integer
        Dim nMin As Integer
        Dim nSec As Integer


        If lSeconds > 3559 Then
            Dim ltmp As Long = lSeconds \ 3600
            nHour = CInt(ltmp)
            lSeconds -= (ltmp * 3600)
        Else
            nHour = 0
        End If

        If lSeconds > 59 Then
            Dim ltmp As Long = lSeconds \ 60
            nMin = CInt(ltmp)
            lSeconds -= (ltmp * 60)
        Else
            nMin = 0
        End If

        If lSeconds >= 0 Then
            nSec = CInt(lSeconds)
            Return TimeSerial(nHour, nMin, nSec)
        Else
            Return TimeSerial(0, 0, 0)
        End If

    End Function
    Private Sub subBuildReport(Optional ByVal bAutosave As Boolean = False)
        '********************************************************************************************
        'Description: respond to ok button on frmCriteria
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/02/10  RJO     Summary rows found was sometimes wrong depending on report and summary 
        '                   type. Added code to calculate Summary Rows Found.
        ' 07/30/14  MSW     Add autosave function to support auto report generation.
        '********************************************************************************************
        Dim DT As DataTable = Nothing



        Cursor = Cursors.WaitCursor

        If frmCriteria.ReportData Is Nothing Then
            Cursor = Cursors.Default
            DataLoaded = False
            Exit Sub
        End If

        'some titles getting lost in the sauce
        Dim sTitles() As String = frmCriteria.ReportTitles()


        Select Case ReportType

            Case eReportType.Change
                DT = frmCriteria.ReportData.Tables(gsCHANGE_DS_TABLENAME)
            Case eReportType.Alarm
                DT = frmCriteria.ReportData.Tables(gsALARM_DS_TABLENAME)
            Case eReportType.Conveyor
                DT = dtGetConveyorData(gsALARM_DS_TABLENAME)

                If frmCriteria.chkSummary.Checked Then
                    'massage for summary
                    ' summarize to get time values
                    Dim ChartConfig As tChartConfig
                    ChartConfig.Title = String.Empty
                    ChartConfig.TableSheet = String.Empty
                    ChartConfig.ChartSheet = String.Empty
                    ChartConfig.TopxxReq = CInt(frmCriteria.TopxxNum)
                    DT = dtSummarizeDataTable(DT, ChartConfig, frmCriteria.ReportSummaryType)
                    mbFillScreen = False
                    mnShowPie = 1 '0=no,1=DT,2=single Chart 3=both charts
                End If

            Case eReportType.Production
                If frmCriteria.ReportSummaryType = String.Empty Then
                    ' not a summary report
                    DT = frmCriteria.ReportData.Tables(gsPROD_DS_TABLENAME)
                Else
                    'summary report
                    DT = dtGetProductionSummDataTable(gsPROD_DS_TABLENAME, _
                                        frmCriteria.ReportSummaryType)
                End If

            Case eReportType.Vision
                If frmCriteria.ReportSummaryType = String.Empty Then
                    ' not a summary report
                    DT = frmCriteria.ReportData.Tables(gsVISN_TABLENAME)
                Else
                    'summary report
                    DT = dtGetVisionSummDataTable(gsVISN_TABLENAME, _
                                        frmCriteria.ReportSummaryType)
                End If

            Case eReportType.Downtime, eReportType.RMCharts, eReportType.RMChartsAuto


                Dim DT1 As DataTable = dtGetDowntimeData(gsALARM_DS_TABLENAME, _
                                                        frmCriteria.chkHBO.Checked)
                ' summarize to get time values
                Dim ChartConfig As tChartConfig
                ChartConfig.Title = String.Empty
                ChartConfig.TableSheet = String.Empty
                ChartConfig.ChartSheet = String.Empty
                ChartConfig.TopxxReq = CInt(frmCriteria.TopxxNum)
                ChartConfig.StartTime = frmCriteria.StartTime
                ChartConfig.EndTime = frmCriteria.EndTime
                If ReportType = eReportType.RMCharts Then
                    DTChart0 = DT1
                    Select Case frmCriteria.ReportSummaryType
                        Case gpsRM.GetString("psDURATION_CAP"), gpsRM.GetString("psOCCURRENCES_CAP")
                            DTChart1 = dtSummarizeDataTable(DT1, ChartConfig, frmCriteria.ReportSummaryType)
                            mnShowPie = 2 '0=no,1=DT,2=Single Chart 3=Both Charts
                        Case gpsRM.GetString("psBOTH_CAP")
                            DTChart1 = dtSummarizeDataTable(DT1, ChartConfig, gpsRM.GetString("psDURATION_CAP"))
                            DTChart2 = dtSummarizeDataTable(DT1, ChartConfig, gpsRM.GetString("psOCCURRENCES_CAP"))
                            mnShowPie = 3 '0=no,1=DT,2=Single Chart 3=Both Charts
                    End Select
                    DT = DTChart1
                    mbFillScreen = False
                    'DT2 = mExcel.subDoManualChart(DT1, ChartConfig, frmCriteria.ReportSummaryType)
                Else
                    DTChart1 = dtSummarizeDataTable(DT1, ChartConfig, frmCriteria.ReportSummaryType)
                    If frmCriteria.ReportSummaryType = String.Empty Then
                        ' not a summary report
                        DT = DT1
                        mbFillScreen = True
                        mnShowPie = 0 '0=no,1=DT,2=single Chart 3=both charts
                    Else
                        'summary report
                        DT = DTChart1
                        mbFillScreen = False
                        mnShowPie = 1 '0=no,1=DT,2=single Chart 3=both charts
                    End If
                End If

                sTitles(6) = gpsRM.GetString("psUPTIME") & ": " & _
                                        ChartConfig.UptimePct.ToString("00.0") & "%"
                'If (frmCriteria.ReportSummaryType <> String.Empty) Then
                sTitles(7) = gpsRM.GetString("psMTBF") & ": " & sGetDurationString(ChartConfig.MTBF)
                sTitles(8) = gpsRM.GetString("psMTTR") & ": " & sGetDurationString(ChartConfig.MTTR)
                'End If
        End Select

        subDoTitles(sTitles)

        With dgvMain
            .SuspendLayout()
            .DataSource = Nothing
            subClearScreen()
            .DataSource = DT

            ' Report specific Formatting
            subFormatReport(dgvMain)

            .ResumeLayout()
            .Show()

            If .Rows.Count = 0 Then
                MessageBox.Show(gpsRM.GetString("psNO_RECORDS"), _
                                gcsRM.GetString("csPAINTWORKS"), MessageBoxButtons.OK, _
                                MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, _
                                MessageBoxOptions.DefaultDesktopOnly)
                mnShowPie = 0 '0=no,1=DT,2=single Chart 3=both charts
            End If

            If frmCriteria.chkSummary.Checked Then
                'dont count total and overall rows
                'Dim nTmp As Integer = .RowCount - 2 'RJO 02/02/10
                Dim nTmp As Integer = .RowCount

                Select Case ReportType 'RJO 02/02/10
                    Case eReportType.Downtime
                        nTmp -= 2
                    Case eReportType.Production
                        If frmCriteria.ReportSummaryType <> gpsRM.GetString("psJOB_REPORT_CAP") Then nTmp -= 1
                    Case eReportType.Vision
                        nTmp -= 1
                    Case Else
                        'Rows found = .RowCount
                End Select

                If nTmp < 0 Then nTmp = 0
                lblTitle09.Text = nTmp.ToString & " " & gpsRM.GetString("psSUMMARY_ROWS_FOUND")
            Else
                lblTitle09.Text = .RowCount.ToString & " " & gpsRM.GetString("psROWS_FOUND")
            End If


        End With

        subSizeGrid(mbFillScreen, mnShowPie)
        '0=no,1=DT,2=single Chart 3=both charts, 4=AutoChart1, 5 = AutoChart3
        Select Case mnShowPie
            Case 0
                imgPie.Visible = False
                imgKey.Visible = False
                imgPie2.Visible = False
                imgKey2.Visible = False
                imgPie3.Visible = False
                imgKey3.Visible = False
                imgPie4.Visible = False
                imgKey4.Visible = False
                tlpChart.Visible = False
                btnChart.Visible = (mStartingReportType = eReportType.Downtime)
                dgvMain.Visible = True
            Case 1 'DT summary
                DTChart1 = CType(dgvMain.DataSource, DataTable)
                moChartType = eChartTypes.Pie
                imgPie.Visible = True
                imgKey.Visible = True
                imgPie2.Visible = False
                imgKey2.Visible = False
                imgPie3.Visible = False
                imgKey3.Visible = False
                imgPie4.Visible = False
                imgKey4.Visible = False
                tlpChart.Visible = True
                btnChart.Visible = True
                dgvMain.Visible = True
                msChartTitle(0) = frmCriteria.ReportSummaryType
                subDoChart(imgPie, imgKey, DTChart1, eChartTypes.Pie, frmCriteria.ReportSummaryType)
            Case 2 'single chart
                moChartType = eChartTypes.Bar
                imgPie.Visible = True
                imgKey.Visible = True
                imgPie2.Visible = False
                imgKey2.Visible = False
                imgPie3.Visible = False
                imgKey3.Visible = False
                imgPie4.Visible = False
                imgKey4.Visible = False
                tlpChart.Visible = True
                btnChart.Visible = True
                dgvMain.Visible = False
                msChartTitle(0) = frmCriteria.ReportSummaryType
                subDoChart(imgPie, imgKey, DTChart1, eChartTypes.Bar, frmCriteria.ReportSummaryType)
            Case 3 'Both charts
                moChartType = eChartTypes.Bar
                moChart2Type = eChartTypes.Bar
                imgPie.Visible = True
                imgKey.Visible = True
                imgPie2.Visible = True
                imgKey2.Visible = True
                imgPie3.Visible = False
                imgKey3.Visible = False
                imgPie4.Visible = False
                imgKey4.Visible = False
                tlpChart.Visible = True
                btnChart.Visible = True
                dgvMain.Visible = False
                msChartTitle(0) = gpsRM.GetString("psDURATION_CAP")
                msChartTitle(1) = gpsRM.GetString("psOCCURRENCES_CAP")
                subDoChart(imgPie, imgKey, DTChart1, eChartTypes.Bar, gpsRM.GetString("psDURATION_CAP"))
                subDoChart(imgPie2, imgKey2, DTChart2, eChartTypes.Bar, gpsRM.GetString("psOCCURRENCES_CAP"))
            Case 4 'Auto Charts 1
                moChartType = eChartTypes.Bar
                moChart2Type = eChartTypes.Bar
                imgPie.Visible = True
                imgKey.Visible = True
                imgPie2.Visible = True
                imgKey2.Visible = True
                imgPie3.Visible = True
                imgKey3.Visible = True
                imgPie4.Visible = True
                imgKey4.Visible = True
                tlpChart.Visible = True
                btnChart.Visible = True
                dgvMain.Visible = False
                msChartTitle(0) = gpsRM.GetString("psDURATION_CAP")
                msChartTitle(1) = gpsRM.GetString("psOCCURRENCES_CAP")
                subDoChart(imgPie, imgKey, DTChart1, eChartTypes.Bar, gpsRM.GetString("psDURATION_CAP"))
                subDoChart(imgPie2, imgKey2, DTChart2, eChartTypes.Bar, gpsRM.GetString("psOCCURRENCES_CAP"))
        End Select

        Status = lblTitle09.Text

        DataLoaded = True
        If bAutosave Then
            bPrintdoc(False, True, True)
        End If
        Cursor = Cursors.Default

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
        dgvMain.Rows.Clear()
        dgvMain.Columns.Clear()
        DataLoaded = False ' needs to be first
        subEnableControls(False)
        mbEventBlocker = bTmp

    End Sub
    Private Sub subDoTitles(ByVal vTitles As String())
        '********************************************************************************************
        'Description:  fill in text on titles above grid
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sName As String = "lblTitle"

        For i As Integer = 0 To UBound(vTitles)
            Dim l As Label = TryCast(pnlMain.Controls(sName & Format(i, "00")), Label)
            If l Is Nothing = False Then
                l.Text = vTitles(i)
            End If
        Next

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
        Dim bTmp As Boolean = (True And DataLoaded And (dgvMain.Rows.Count > 0))

        btnClose.Enabled = True

        If bEnable = False Then
            btnSave.Enabled = False
            btnPrint.Enabled = False
            btnStatus.Enabled = True
            bRestOfControls = False
            mnuPrintFile.Enabled = False
        Else
            Select Case Privilege
                Case ePrivilege.None
                    btnSave.Enabled = False
                    btnPrint.Enabled = bTmp
                    btnStatus.Enabled = True
                    bRestOfControls = False
                    mnuPrintFile.Enabled = False

                Case ePrivilege.Edit
                    btnSave.Enabled = bTmp
                    btnPrint.Enabled = bTmp
                    btnStatus.Enabled = True
                    bRestOfControls = True
                    mnuPrintFile.Enabled = True

                Case ePrivilege.Copy
                    btnSave.Enabled = bTmp
                    btnPrint.Enabled = bTmp
                    btnStatus.Enabled = True
                    bRestOfControls = True
                    mnuPrintFile.Enabled = True

                Case ePrivilege.Execute 'RJO 10/24/10
                    btnSave.Enabled = bTmp
                    btnPrint.Enabled = bTmp
                    btnStatus.Enabled = True
                    bRestOfControls = True
                    mnuPrintFile.Enabled = True

            End Select
        End If

        'restof controls here


        pnlMain.Enabled = True

    End Sub
    Private Sub subFormatReport(ByRef oReportGrid As DataGridView)
        '********************************************************************************************
        'Description: make it pretty
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/10/09  MSW     Add a default sort for alarm logs
        '********************************************************************************************
        If oReportGrid.Rows.Count = 0 Then Exit Sub

        With oReportGrid
            Try

                Status = gpsRM.GetString("psFORMATTING")

                .AllowUserToAddRows = False
                .AllowUserToDeleteRows = False
                .AllowUserToResizeColumns = True
                .AllowUserToResizeRows = False
                .RowHeadersVisible = True
                .ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True
                .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

                For i As Integer = 0 To .Columns.Count - 1
                    .Columns(i).AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells

                Next

                .RowHeadersWidth = 20


                Select Case ReportType
                    Case eReportType.Alarm

                        If frmCriteria.chkSummary.Checked Then

                        Else
                            .Columns(gpsRM.GetString("psDURATION_CAP")).DefaultCellStyle _
                                        .Alignment = DataGridViewContentAlignment.MiddleRight

                            .Columns(gpsRM.GetString("psSTART_TIME_CAP")).DefaultCellStyle _
                                        .Format = "G"  'short date long time
                            .Columns(gpsRM.GetString("psEND_TIME_CAP")).DefaultCellStyle _
                                        .Format = "G"  'short date long time
                            'It was coming up sorted by end time, default to start time.  It's easier to use that way
                            .Sort(.Columns(gpsRM.GetString("psSTART_TIME_CAP")), System.ComponentModel.ListSortDirection.Ascending)

                            .Columns(gpsRM.GetString("psSTART_TIME_CAP")).Width = 150
                            .Columns(gpsRM.GetString("psEND_TIME_CAP")).Width = 150
                            .Columns(gpsRM.GetString("psDESC_CAP")).Width = 300
                        End If
                    Case eReportType.Downtime, eReportType.RMCharts, eReportType.RMChartsAuto

                        If frmCriteria.chkSummary.Checked Then
                            .Columns(gpsRM.GetString("psOCCURRENCES_PERC_CAP")).DefaultCellStyle _
                                        .Format = "0.00%"
                            .Columns(gpsRM.GetString("psDURATION_PERC_CAP")).DefaultCellStyle _
                                        .Format = "0.00%"

                        Else
                            .Columns(gpsRM.GetString("psDURATION_CAP")).DefaultCellStyle _
                                        .Alignment = DataGridViewContentAlignment.MiddleRight

                            .Columns(gpsRM.GetString("psSTART_TIME_CAP")).DefaultCellStyle _
                                        .Format = "G"  'short date long time
                            .Columns(gpsRM.GetString("psEND_TIME_CAP")).DefaultCellStyle _
                                        .Format = "G"  'short date long time

                        End If

                    Case eReportType.Production

                        If frmCriteria.chkSummary.Checked Then

                            '.Columns(msHOURCOL).DefaultCellStyle _
                            '            .Format = "G"  'short date long time
                            If frmCriteria.ReportSummaryType = gpsRM.GetString("psJOB_REPORT_CAP") Then
                                .Columns(gpsRM.GetString("psCOMPLETE_TIME_CAP")).DefaultCellStyle _
                                .Format = "G"  'short date long time
                                .Columns(gpsRM.GetString("psCOMPLETE_TIME_CAP")).Width = 150
                            Else
                                .Columns(0).DefaultCellStyle _
                                .Format = "g"  'short date long time
                                .Columns(0).Width = 150
                            End If
                        Else
                            If frmCriteria.bShowColumn(frmCriteria.eShowCol.CCTime) Then
                                .Columns(gpsRM.GetString("psCC_TIME_CAP")).DefaultCellStyle _
                                            .Alignment = DataGridViewContentAlignment.MiddleRight
                            End If
                            If frmCriteria.bShowColumn(frmCriteria.eShowCol.PaintTotal) Then
                                .Columns(gpsRM.GetString("psPAINT_TOTAL_CAP")).DefaultCellStyle _
                                            .Alignment = DataGridViewContentAlignment.MiddleRight
                            End If
                            If frmCriteria.bShowColumn(frmCriteria.eShowCol.PaintTotal2) Then
                                .Columns(gpsRM.GetString("psPAINT_TOTAL2_CAP")).DefaultCellStyle _
                                            .Alignment = DataGridViewContentAlignment.MiddleRight
                            End If

                            .Columns(gpsRM.GetString("psCYCLE_TIME_CAP")).DefaultCellStyle _
                                        .Alignment = DataGridViewContentAlignment.MiddleRight

                            .Columns(gpsRM.GetString("psCOMPLETE_TIME_CAP")).DefaultCellStyle _
                                        .Format = "G"  'short date long time
                            .Columns(gpsRM.GetString("psCOMPLETE_TIME_CAP")).Width = 150
                        End If

                    Case eReportType.Vision

                        If frmCriteria.chkSummary.Checked Then

                            '.Columns(msHOURCOL).DefaultCellStyle _
                            '            .Format = "G"  'short date long time
                        Else
                            .Columns(gpsRM.GetString("psCOMPLETE_TIME_CAP")).DefaultCellStyle _
                                        .Format = "G"  'short date long time
                            .Columns(gpsRM.GetString("psVISION_STATUS_CAP")).Width = 200
                        End If

                    Case eReportType.Change

                        If frmCriteria.chkSummary.Checked Then
                            .Columns(gcsRM.GetString("csAREA_CAP")).Width = 200
                        Else
                            .Columns(gcsRM.GetString("csSPECIFIC_CAP")).Width = 400
                            .Columns(gcsRM.GetString("csTIME_CAP")).Width = 150
                            .Columns(gcsRM.GetString("csAREA_CAP")).Width = 200
                            .Columns(gcsRM.GetString("csPARAMETER_CAP")).Width = 200
                        End If

                    Case eReportType.Conveyor

                        If frmCriteria.chkSummary.Checked Then

                        Else
                            .Columns(gpsRM.GetString("psSTART_TIME_CAP")).DefaultCellStyle _
                                        .Format = gpsRM.GetString("psDATETIME_FORMAT1")
                            .Columns(gpsRM.GetString("psEND_TIME_CAP")).DefaultCellStyle _
                                        .Format = gpsRM.GetString("psDATETIME_FORMAT1")
                        End If

                End Select

            Catch ex As Exception
                'just bail
            End Try

        End With


    End Sub
    Friend Sub subSizeGrid(ByVal FillScreen As Boolean, ByVal nShowPie As Integer)
        '********************************************************************************************
        'Description:  set up the grid layout
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nWidth As Integer
        Dim nHeight As Integer
        Dim z As Size = Me.pnlMain.Size
        Dim nWBorders As Integer = 25
        Const nHBorders As Integer = 155
        Dim nDTChartWidth As Integer = CInt(z.Width * 0.4)
        '0=no,1=DT,2=single Chart 3=both charts, 4 = Auto Charts

        Select Case nShowPie
            Case 0
            Case 1
                nWBorders = 13 'size of a single border
                tlpChart.Width = nDTChartWidth
                tlpChart.Left = z.Width - (nWBorders + nDTChartWidth)
                nWBorders = (nWBorders * 3) + tlpChart.Width
                tlpChart.ColumnStyles(0).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(1).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(2).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(3).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(0).Width = 100
                tlpChart.ColumnStyles(1).Width = 0
                tlpChart.ColumnStyles(2).Width = 0
                tlpChart.ColumnStyles(3).Width = 0
                tlpChart.RowStyles(0).SizeType = SizeType.Percent
                tlpChart.RowStyles(1).SizeType = SizeType.Percent
                tlpChart.RowStyles(0).Height = 75
                tlpChart.RowStyles(1).Height = 25
            Case 2
                nWBorders = 13 'size of a single border
                tlpChart.Left = nWBorders
                tlpChart.Width = z.Width - (2 * nWBorders)
                tlpChart.ColumnStyles(0).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(1).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(2).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(3).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(0).Width = 100
                tlpChart.ColumnStyles(1).Width = 0
                tlpChart.ColumnStyles(2).Width = 0
                tlpChart.ColumnStyles(3).Width = 0
                tlpChart.RowStyles(0).SizeType = SizeType.Percent
                tlpChart.RowStyles(1).SizeType = SizeType.Percent
                tlpChart.RowStyles(0).Height = 60
                tlpChart.RowStyles(1).Height = 40
            Case 3
                nWBorders = 13 'size of a single border
                tlpChart.Left = nWBorders
                tlpChart.Width = z.Width - (2 * nWBorders)
                tlpChart.ColumnStyles(0).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(1).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(2).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(3).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(0).Width = 50
                tlpChart.ColumnStyles(1).Width = 50
                tlpChart.ColumnStyles(2).Width = 0
                tlpChart.ColumnStyles(3).Width = 0
                tlpChart.RowStyles(0).SizeType = SizeType.Percent
                tlpChart.RowStyles(1).SizeType = SizeType.Percent
                tlpChart.RowStyles(0).Height = 60
                tlpChart.RowStyles(1).Height = 40
            Case 4
                nWBorders = 13 'size of a single border
                tlpChart.Left = nWBorders
                tlpChart.Width = z.Width - (2 * nWBorders)
                tlpChart.ColumnStyles(0).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(1).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(2).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(3).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(0).Width = 25
                tlpChart.ColumnStyles(1).Width = 25
                tlpChart.ColumnStyles(2).Width = 25
                tlpChart.ColumnStyles(3).Width = 25
                tlpChart.RowStyles(0).SizeType = SizeType.Percent
                tlpChart.RowStyles(1).SizeType = SizeType.Percent
                tlpChart.RowStyles(0).Height = 70
                tlpChart.RowStyles(1).Height = 30
            Case 5
                nWBorders = 13 'size of a single border
                tlpChart.Left = nWBorders
                tlpChart.Width = z.Width - (2 * nWBorders)
                tlpChart.ColumnStyles(0).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(1).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(2).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(3).SizeType = SizeType.Percent
                tlpChart.ColumnStyles(0).Width = 50
                tlpChart.ColumnStyles(1).Width = 50
                tlpChart.ColumnStyles(2).Width = 0
                tlpChart.ColumnStyles(3).Width = 0
                tlpChart.RowStyles(0).SizeType = SizeType.Percent
                tlpChart.RowStyles(1).SizeType = SizeType.Percent
                tlpChart.RowStyles(0).Height = 70
                tlpChart.RowStyles(1).Height = 30
        End Select
        Application.DoEvents()
        Try
            If nShowPie <= 1 Then
                With dgvMain

                    Dim sz As Size = .Size

                    'allow the manual resize after the formatting was done.
                    For i As Integer = 0 To .Columns.Count - 1
                        .Columns(i).AutoSizeMode = DataGridViewAutoSizeColumnMode.None

                    Next
                    If FillScreen Then
                        sz.Height = z.Height - nHBorders
                        sz.Width = z.Width - nWBorders
                        .Size = sz
                        Exit Sub
                    End If

                    For Each c As DataGridViewColumn In .Columns
                        nWidth += c.Width
                    Next
                    nWidth += 50
                    If nWidth > (z.Width - nWBorders) Then nWidth = (z.Width - nWBorders)
                    sz.Width = nWidth

                    For Each r As DataGridViewRow In .Rows
                        nHeight += r.Height
                    Next
                    nHeight += 50
                    If nHeight > (z.Height - nHBorders) Then nHeight = (z.Height - nHBorders)
                    sz.Height = nHeight

                    .Size = sz
                End With


            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub subGridContextMenuClicked(Optional ByVal sender As Object = Nothing, Optional ByVal e As System.EventArgs = Nothing)
        '********************************************************************************************
        'Description:  user clicked on context menu after right clicking on grid
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim dtStart As DateTime
        Dim dtEnd As DateTime

        Try
            Select Case ReportType
                Case eReportType.Alarm
                    ' get production records
                    Dim dr As DataGridViewRow = mClickedCell.OwningRow
                    dtStart = CType(dr.Cells.Item(gpsRM.GetString("psSTART_TIME_CAP")).Value, DateTime)
                    dtEnd = CType(dr.Cells.Item(gpsRM.GetString("psEND_TIME_CAP")).Value, DateTime)

                    If frmCriteria.GetProductionDataFromAlarm(dtStart, dtEnd) Then
                        Dim DT As DataTable = frmCriteria.ReportData.Tables(gsPROD_DS_TABLENAME)
                        frmMiniGrid.DataTableIn = DT
                    End If

                Case eReportType.Production
                    ' get alarm records
                    Dim dr As DataGridViewRow = mClickedCell.OwningRow
                    dtEnd = CType(dr.Cells.Item(gpsRM.GetString("psCOMPLETE_TIME_CAP")).Value, DateTime)
                    Dim nSec As Integer = _
                        CType(dr.Cells.Item(gpsRM.GetString("psCYCLE_TIME_CAP")).Value, Integer)
                    dtStart = DateAdd(DateInterval.Second, -nSec, dtEnd)
                    '
                    If frmCriteria.GetAlarmDataFromProd(dtStart, dtEnd) Then
                        Dim DT As DataTable = frmCriteria.ReportData.Tables(gsALARM_DS_TABLENAME)
                        frmMiniGrid.DataTableIn = DT
                    End If
            End Select
        Catch ex As Exception

        End Try
    End Sub
    Friend Sub subDoChart(ByRef oPicChart As PictureBox, ByRef oPicKey As PictureBox, ByRef oDT As DataTable, _
                    Optional ByVal oChartType As eChartTypes = eChartTypes.Pie, _
                    Optional ByRef sSummType As String = "", _
                    Optional ByRef sTitle As String = "", Optional ByVal oItems As Integer = -1)
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
        Dim oRenderer As New PWGraph.Render
        Dim sPieTitle As String
        Dim fTmp As Single
        Dim nTmp As Long
        Dim oBar As PWGraph.Bar.BarChart = Nothing
        Dim oPie As PWGraph.Pie.PieChart = Nothing
        Dim nItems As Integer = 5
        Select Case ReportType
            Case eReportType.Conveyor
                sPieTitle = gpsRM.GetString("psPIE_TITLE_CONV")
                nItems = 5
            Case eReportType.Downtime
                sPieTitle = gpsRM.GetString("psPIE_TITLE_DT") & sSummType
                nItems = 5
            Case eReportType.RMCharts
                sPieTitle = gpsRM.GetString("psPIE_TITLE_DT") & sSummType
                nItems = 10
            Case eReportType.RMChartsAuto
                sPieTitle = sTitle
                nItems = 5
            Case Else
                sPieTitle = "You should not see this"
        End Select
        If oItems > 0 Then
            nItems = oItems
        End If
        Select Case oChartType
            Case eChartTypes.Bar
                oBar = New PWGraph.Bar.BarChart
                With oBar
                    .ImageSize = New Size(oPicChart.Width, oPicChart.Height)
                    'Or Auto size
                    '.AutoSize = True
                    '.Diameter = imgPie.Height - 30
                    '.Thickness = 6
                    'Background color for the chart
                    .Color = Me.BackColor
                    'Adds a border around the pie.
                    .GraphBorder = True
                    .KeyTitle = sPieTitle
                    .KeyTitleFontStyle = FontStyle.Bold
                    .KeyBackColor = Me.BackColor
                    .Alignment = Base.eBarTypes.VerticalBottom
                    .GraphAlign = Base.eBarTypes.VerticalBottom
                    If ReportType = eReportType.RMChartsAuto Then
                        .KeyFontSize = 10
                        .KeyTitleFontSize = 12
                    Else
                        .KeyTitleFontSize = 15
                        .KeyFontSize = 10
                    End If
                End With
            Case eChartTypes.Pie
                oPie = New PWGraph.Pie.PieChart
                With oPie
                    .ImageSize = New Size(oPicChart.Width, oPicChart.Height)
                    'Or Auto size
                    '.AutoSize = True
                    .Diameter = oPicChart.Height - 30
                    .Thickness = 6
                    'Background color for the chart
                    .Color = Me.BackColor
                    'Adds a border around the pie.
                    .GraphBorder = True
                    .KeyTitle = sPieTitle
                    .KeyTitleFontStyle = FontStyle.Bold
                    .KeyBackColor = Me.BackColor
                    If ReportType = eReportType.RMChartsAuto Then
                        .KeyFontSize = 10
                        .KeyTitleFontSize = 12
                    Else
                        .KeyTitleFontSize = 15
                        .KeyFontSize = 10
                    End If
                End With
        End Select
        Dim bPct As Boolean = oDT.Columns.Contains(gpsRM.GetString("psOCCURRENCES_PERC_CAP"))

        Dim nCollectionCount As Integer = 0
        For i As Integer = 0 To oDT.Rows.Count - 1

            Dim r As DataRow = oDT.Rows.Item(i)
            Dim bAdd As Boolean = True
            If r.Item(gpsRM.GetString("psDESC_CAP")).ToString <> String.Empty Then
                'leave room for 5 pieces. if more, combine all left into 5th piece, and
                'call it other
                Dim fPct As Single ' = CSng(Strings.Replace(r.Cells.Item( _
                'gpsRM.GetString("psPERCENT_CAP")).Value.ToString, "%", String.Empty))
                Dim sVal As String = String.Empty
                Dim nVal As Long = 0
                'fPerCent2 = (CSng(dtNewTable.Rows(j).Item(gpsRM.GetString("psPERCENT_CAP")).ToString))
                If sSummType = gpsRM.GetString("psOCCURRENCES_CAP") Then
                    sVal = r.Item(gpsRM.GetString("psOCCURRENCES_CAP")).ToString
                    nVal = CInt(sVal)
                    If bPct Then
                        fPct = CSng(Strings.Replace(r.Item( _
                        gpsRM.GetString("psOCCURRENCES_PERC_CAP")).ToString, "%", String.Empty))
                    Else
                        fPct = nVal
                    End If
                Else
                    nVal = CLng(r.Item(gpsRM.GetString("psDURATION_CAP")))
                    sVal = sGetDurationString(nVal)
                    If bPct Then
                        fPct = CSng(Strings.Replace(r.Item( _
                        gpsRM.GetString("psDURATION_PERC_CAP")).ToString, "%", String.Empty))
                    Else
                        fPct = nVal
                    End If
                End If

                Dim sDesc As String = Strings.Trim(r.Item(gpsRM.GetString("psDESC_CAP")).ToString)
                sDesc = sDesc & "  -  " & sVal
                If bPct Then
                    sDesc = sDesc & ", " & Format((fPct), "Percent")

                End If
                'o.Item(gpsRM.GetString("psDURATION_CAP")) = sGetDurationString(CLng(TableIn.Rows.Item(i).Item(gpsRM.GetString("psDURATION_CAP"))))
                'o.Item(gpsRM.GetString("psOCCURRENCES_CAP")) = TableIn.Rows.Item(i).Item(gpsRM.GetString("psOCCURRENCES_CAP"))

                'in case of conveyor report, fpct could be negative, for missing conv status. dont count that
                If fPct >= 0 Then
                    Dim cColor As System.Drawing.Color = Color.Yellow
                    Select Case (nCollectionCount Mod 10)
                        Case 0
                            cColor = Color.Yellow
                        Case 1
                            cColor = Color.MediumSlateBlue
                        Case 2
                            cColor = Color.Magenta
                        Case 3
                            cColor = Color.SaddleBrown
                        Case 4
                            cColor = Color.Aqua
                        Case 5
                            cColor = Color.Red
                        Case 6
                            cColor = Color.Blue
                        Case 7
                            cColor = Color.Purple
                        Case 8
                            cColor = Color.Beige
                        Case 9
                            cColor = Color.BurlyWood
                    End Select
                    If nCollectionCount = nItems - 1 Then
                        fTmp += fPct
                        nTmp += nVal
                        If i = dgvMain.Rows.Count - 3 Then
                            If sSummType = gpsRM.GetString("psOCCURRENCES_CAP") Then
                                sDesc = gpsRM.GetString("psOTHER") & "  -  " & nTmp.ToString
                            Else
                                sDesc = gpsRM.GetString("psOTHER") & "  -  " & sGetDurationString(nTmp)
                            End If
                            If bPct Then
                                If sSummType = gpsRM.GetString("psOCCURRENCES_CAP") Then
                                    sDesc = sDesc & ", " & Format((fTmp), "Percent")
                                Else
                                    sDesc = sDesc & ", " & Format((fTmp), "Percent")
                                End If
                            End If
                            fPct = fTmp
                            bAdd = True
                        Else
                            bAdd = False
                        End If
                    End If
                    If bAdd Then
                        nCollectionCount = nCollectionCount + 1
                        Select Case oChartType
                            Case eChartTypes.Bar
                                Dim obs As New Bar.BarSlice(CDec(fPct), cColor, sDesc)
                                oBar.BarSliceCollection.Add(obs)
                            Case eChartTypes.Pie
                                Dim opp As New Pie.PieSlice(CDec(fPct), cColor, sDesc)
                                oPie.PieSliceCollection.Add(opp)
                        End Select
                    End If
                End If  'fPct > 0
            End If  'r.Cells.Item(gpsRM.GetString("psDESC_CAP")).Value.ToString <> String.Empty

        Next
        Select Case oChartType
            Case eChartTypes.Bar
                oPicChart.Image = oRenderer.DrawChart(oBar)
                oPicKey.Image = oRenderer.DrawKey(oBar, oPicKey.Size)
            Case eChartTypes.Pie
                oPicChart.Image = oRenderer.DrawChart(oPie)
                oPicKey.Image = oRenderer.DrawKey(oPie, oPicKey.Size)
        End Select


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
        ' 01/21/11  MSW     bring up the main form first
        ' 09/30/11  MSW     call me.show earlier to make startup seem quicker
        ' 03/12/13  RJO     Added initialization for msFanucName and msHelpPath, used for Robot alarm
        '                   Cause/Remedy display.
        '********************************************************************************************
        Dim lReply As Windows.Forms.DialogResult = Response.OK

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

        Status = gcsRM.GetString("csINITALIZING")
        Progress = 1


        Try
            DataLoaded = False
            EditsMade = False
            mbScreenLoaded = False

            Me.Text = gpsRM.GetString("psSCREENCAPTION")

            subProcessCommandLine()

            Dim s(mnMAX_TITLES) As String
            subDoTitles(s)

            If ReportType <> eReportType.RMChartsAuto Then
            Me.Show()
            End If

            'init the old password for now
            'moPassword = New PWPassword.PasswordObject 'RJO 03/23/12
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

            If ReportType = eReportType.Unknown Then
                btnSelect.DropDownItems.Add(gpsRM.GetString("psALARM_REPORT"))
                btnSelect.DropDownItems.Add(gpsRM.GetString("psCHANGE_REPORT"))
                btnSelect.DropDownItems.Add(gpsRM.GetString("psPRODUCTION_REPORT"))
                btnSelect.DropDownItems.Add(gpsRM.GetString("psDOWNTIME_REPORT"))
                btnSelect.DropDownItems.Add(gpsRM.GetString("psRMCHARTS"))
                btnSelect.DropDownItems.Add(gpsRM.GetString("psCONVEYOR_REPORT"))
                btnSelect.DropDownItems.Add(gpsRM.GetString("psWEEKLY_REPORT"))
                btnSelect.DropDownItems.Add(gpsRM.GetString("psVISION_REPORT"))
                btnSelect.Visible = True
                btnCriteria.Enabled = False
            End If

            btnRefresh.Text = gcsRM.GetString("csREFRESH")
            btnRefresh.Enabled = btnCriteria.Enabled
            btnChart.Text = gpsRM.GetString("psCHART")
            colZones = New clsZones(String.Empty)
            mtControllerArray = GetControllerArray(colZones)
            msHelpFormTitle = gpsRM.GetString("psHELP_FORM_CAP")

            'init new IPC and new Password 'RJO 03/23/12
            oIPC = New Paintworks_IPC.clsInterProcessComm
            moPassword = New clsPWUser
            Progress = 50

            mScreenSetup.InitializeForm(Me)

            'get Screen and User lists from Password.NET for criteria  'RJO 03/23/12
            Dim sScreenMsg() As String = {moPassword.ProcessName, "getscreennames"}

            msScreenNames(0) = String.Empty
            oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sScreenMsg)
            Dim oDate As Date = New Date()
            oDate = Now
            Do While Not mbGotScreenList
                Application.DoEvents()
                If DateDiff(DateInterval.Second, oDate, Now) > 30 Then
                    'This should only happen if password isn't running.  don't hang up
                    Exit Do
                End If
            Loop

            Progress = 60

            Dim sUserMsg() As String = {moPassword.ProcessName, "getusernames"}

            msUserNames(0) = String.Empty
            oIPC.WritePasswordMsg(gs_COM_ID_PASSWORD, sUserMsg)

            oDate = Now
            Do While Not mbGotUserList
                Application.DoEvents()
                If DateDiff(DateInterval.Second, oDate, Now) > 30 Then
                    'This should only happen if password isn't running.  don't hang up
                    Exit Do
                End If
            Loop

            Progress = 70

            AddHandler frmCriteria.RunReport, AddressOf subBuildReport
            AddHandler frmCriteria.QueryFailed, AddressOf subReportFailed

            Progress = 98
            subEnableControls(True)

            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound

            subSizeGrid(dgvMain)

            'statusbar text
            'Status(True) = gcsRM.GetString("csSELECT_ZONE")


            mbScreenLoaded = True

            If (ReportType <> eReportType.Unknown) Then
                frmCriteria.ZoneCollection = colZones
                frmCriteria.ReportType = mReportType
                'Use form title for printout title
                mPrintHtml = New clsPrintHtml(Me.Text, True, 30, , , , , True, True)

                If ReportType = eReportType.RMChartsAuto Then
                    mExcel.subDoAutoCharts(mdCmdDate)
                    'mCharts.subDoAutoCharts(mdCmdDate)
                    Me.Close()
                Else
                    'Me.Show() 'MSW 1/21/11 - bring up the main form first
                    frmCriteria.ShowMe()
                End If
            End If

            'Get values for a few items that will allow the user to view Cause/Remedy text for Robot Alarms 'RJO 03/12/13
            Dim colControllers As New clsControllers(colZones, False)
            Dim oController As clsController = colControllers.Item(0)

            msFanucName = oController.FanucName
            Call mPWCommon.GetDefaultFilePath(msHelpPath, eDir.Help, String.Empty, String.Empty)

            'Clean up
            oController = Nothing
            colControllers = Nothing

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
            If ReportType <> eReportType.RMChartsAuto Then
                If colZones.PaintShopComputer Then  '6/1/07
                    DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, _
                            colZones.ActiveZone.IsRemoteZone, False)
                Else
                    DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, _
                            mbRemoteZone, True)
                End If

                Progress = 100
            End If
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try
        If gbAutoRun Then
            subDoAutoRun()
        End If

    End Sub
    Private Sub subDoAutoRun()
        '********************************************************************************************
        'Description: Autogenerate shift reports
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim sTitle As String = gpsRM.GetString("psALARM_REPORT")
        'Use form title for printout title
        mPrintHtml = New clsPrintHtml(sTitle, True, 30, True, , , , True, True)
        Me.Text = sTitle
        ReportType = eReportType.Alarm
        mStartingReportType = ReportType
        frmCriteria = Nothing
        If frmCriteria.ZoneCollection Is Nothing Then
            frmCriteria.ZoneCollection = colZones
        End If
        frmCriteria.ReportType = mReportType
        frmCriteria.ShowMe(True)


        sTitle = gpsRM.GetString("psPRODUCTION_REPORT")
        'Use form title for printout title
        mPrintHtml = New clsPrintHtml(sTitle, True, 30, True, , , , True, True)
        Me.Text = sTitle
        ReportType = eReportType.Production
        mStartingReportType = ReportType
        frmCriteria = Nothing
        If frmCriteria.ZoneCollection Is Nothing Then
            frmCriteria.ZoneCollection = colZones
        End If
        frmCriteria.ReportType = mReportType
        frmCriteria.ShowMe(True)


        sTitle = gpsRM.GetString("psDOWNTIME_REPORT")
        'Use form title for printout title
        mPrintHtml = New clsPrintHtml(sTitle, True, 30, True, , , , True, True)
        Me.Text = sTitle
        ReportType = eReportType.Downtime
        mStartingReportType = ReportType
        frmCriteria = Nothing
        If frmCriteria.ZoneCollection Is Nothing Then
            frmCriteria.ZoneCollection = colZones
        End If
        frmCriteria.ReportType = mReportType
        frmCriteria.ShowMe(True)



        Me.Close()

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
        'If a culture string has been passed in, set the current culture (display language)
        For Each s As String In My.Application.CommandLineArgs
            Debug.Print(s)
        Next
        ReportType = eReportType.Unknown
        mdCmdDate = Nothing
        For Each sTmp As String In My.Application.CommandLineArgs
            Select Case Strings.LCase(sTmp)
                Case "change"
                    ReportType = eReportType.Change
                    Me.Text = gpsRM.GetString("psCHANGE_REPORT")
                Case "production"
                    ReportType = eReportType.Production
                    Me.Text = gpsRM.GetString("psPRODUCTION_REPORT")
                Case "alarm"
                    ReportType = eReportType.Alarm
                    Me.Text = gpsRM.GetString("psALARM_REPORT")
                Case "downtime"
                    ReportType = eReportType.Downtime
                    Me.Text = gpsRM.GetString("psDOWNTIME_REPORT")
                Case "conveyor"
                    ReportType = eReportType.Conveyor
                    Me.Text = gpsRM.GetString("psCONVEYOR_REPORT")
                Case "charts", "rmcharts"
                    ReportType = eReportType.RMCharts
                    Me.Text = gpsRM.GetString("psRMCHARTS")
                Case "weekly", "auto"
                    ReportType = eReportType.RMChartsAuto
                    Me.Text = gpsRM.GetString("psWEEKLY_REPORT")
                Case "vision"
                    ReportType = eReportType.Vision
                    Me.Text = gpsRM.GetString("psVISION_REPORT")
                Case Else
                    'Check for culture

                    Dim sArg As String = "/culture="
                    'If a culture string has been passed in, set the current culture (display language)
                    If sTmp.ToLower.StartsWith(sArg) Then
                        Culture = sTmp.Remove(0, sArg.Length)
                    End If

                    'Check for date
                    Try
                        sArg = "/date="
                        If sTmp.ToLower.StartsWith(sArg) Then
                            Dim sDate As String = sTmp.Remove(0, sArg.Length)
                            mdCmdDate = CDate(sDate)
                        End If
                    Catch ex As Exception
                        Dim lReply As Windows.Forms.DialogResult = Response.OK
                        lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                        Status, MessageBoxButtons.OK)
                        Application.DoEvents()
                    End Try


                    'Check for file format (for autogen)
                    If sTmp.ToLower = "xlsx" Then
                        msAutoGenFormat = ".XLSX"
                    ElseIf sTmp.ToLower = "ods" Then
                        msAutoGenFormat = ".ODS"
                    ElseIf sTmp.ToLower = "csv" Then
                        msAutoGenFormat = ".CSV"
                    End If

                    If sTmp.ToLower = "autorun" Then
                        gbAutoRun = True
                    End If
            End Select

        Next
        mStartingReportType = ReportType
    End Sub
    Private Sub subReportFailed(ByVal ErrString As String)
        '********************************************************************************************
        'Description: no results from query or err processing
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        MessageBox.Show(gpsRM.GetString("psQUERY_FAIL_MSG"), String.Empty, _
                        MessageBoxButtons.OK, MessageBoxIcon.Information, _
                        MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
        Me.Cursor = Cursors.Default

    End Sub
    Private Sub subSaveData()
        'Private Sub subSaveData(Optional ByVal bXLS As Boolean = False)
        '********************************************************************************************
        'Description:  Data Save Routine
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/15/09  MSW     add save as xls
        ' 01/11/10  MSW     strip out commas for csv export
        '********************************************************************************************
        bPrintdoc(False, True)
        Progress = 100
    End Sub
    Friend Sub subSizeGrid(ByRef oReportGrid As DataGridView)
        '********************************************************************************************
        'Description:  
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        With oReportGrid
            Dim nSide As Integer = .Left * 2
            Dim nTop As Integer = .Top + 25
            Dim s As New Size
            s.Height = tscMain.ContentPanel.Height - nTop
            s.Width = tscMain.ContentPanel.Width - nSide
            .Size = s
        End With

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
    Private Sub subValidateBreakTable(ByRef dtBreak As DataTable)
        '********************************************************************************************
        'Description:  remove oneshot alarms and check times for the break/strip data table
        '
        'Parameters:  break datatable
        'Returns:     none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim dr As DataRow
            Dim nUb As Integer = dtBreak.Rows.Count - 1
            Dim bFixOS As Boolean = False
            Dim dtStart As DateTime
            Dim dtEnd As DateTime
            Dim nTotalBreakTime As Long = 0
            For i As Integer = nUb To 0 Step -1
                Select Case Trim(dtBreak.Rows(i).Item(gpsRM.GetString("psALARM_CAP")).ToString)
                    Case gtConvAlarms.BreakActiveOS
                        If i = nUb Then
                            ' the last record is oneshot, so break must be active but not yet
                            ' recorded, or is just last record. check record before
                            If i > 0 Then
                                dr = dtBreak.Rows(i - 1)
                                If dr.Item(gpsRM.GetString("psALARM_CAP")).ToString = _
                                                    gtConvAlarms.BreakActive Then
                                    'next alarm is break active - check if its start time = 
                                    'start time of oneshot - if so discard oneshot
                                    If DateDiff(DateInterval.Second, _
                                                CType(gpsRM.GetString("psSTART_TIME_CAP"), Date), _
                                                CType(dtBreak.Rows(i).Item( _
                                                gpsRM.GetString("psSTART_TIME_CAP")), Date)) < 2 Then
                                        dtBreak.Rows.RemoveAt(i)
                                    Else
                                        ' oneshot is for new alarm
                                        bFixOS = True
                                    End If
                                Else
                                    ' oneshot following a oneshot - must be new alarm
                                    bFixOS = True
                                End If
                            Else
                                ' only 1 record and its a oneshot - break here to end of report
                                ' change to break alarm
                                bFixOS = True
                            End If
                        Else
                            dtBreak.Rows.RemoveAt(i)
                        End If
                    Case Else
                        'leave alone
                        'get total break time
                        dtStart = CType(dtBreak.Rows(i).Item(gpsRM.GetString("psSTART_TIME_CAP")), DateTime)
                        dtEnd = CType(dtBreak.Rows(i).Item(gpsRM.GetString("psEND_TIME_CAP")), DateTime)
                        If dtStart < frmCriteria.StartTime Then
                            dtStart = frmCriteria.StartTime
                        End If
                        If dtEnd > frmCriteria.EndTime Then
                            dtEnd = frmCriteria.EndTime
                        End If
                        nTotalBreakTime += DateDiff(DateInterval.Second, dtStart, dtEnd)
                End Select
            Next

            If bFixOS Then
                'last record a oneshot - break here till end of report - this should only happen when
                'running a report for current time period and break record not logged
                dr = dtBreak.Rows(dtBreak.Rows.Count - 1)
                dr.Item(gpsRM.GetString("psALARM_CAP")) = _
                                                    gtConvAlarms.BreakActive
                dr.Item(gpsRM.GetString("psDESC_CAP")) = _
                                         gpsRM.GetString("psBREAK_ACTIVE_DESC")
                dr.Item(gpsRM.GetString("psEND_TIME_CAP")) = _
                                         frmCriteria.EndTime
                dr.Item(gpsRM.GetString("psDURATION_CAP")) = _
                        sGetDurationString( _
                        CType(dr.Item(gpsRM.GetString("psSTART_TIME_CAP")), DateTime), _
                        CType(dr.Item(gpsRM.GetString("psEND_TIME_CAP")), DateTime))

                nTotalBreakTime += DateDiff(DateInterval.Second, _
                        CType(dr.Item(gpsRM.GetString("psSTART_TIME_CAP")), DateTime), frmCriteria.EndTime)

            End If
            mnTotalBreakTime = nTotalBreakTime

        Catch ex As Exception

            Trace.WriteLine(ex.Message)

        End Try
    End Sub

#End Region

#Region " Events "

    Protected Overrides Sub Finalize()

        Try
            RemoveHandler frmCriteria.RunReport, AddressOf subBuildReport
            RemoveHandler frmCriteria.QueryFailed, AddressOf subReportFailed
        Catch ex As Exception

        End Try

        MyBase.Finalize()
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

        subInitializeForm()

    End Sub
    Public Sub New()
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
        mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, msBASE_ASSEMBLY_LOCAL, _
                                            msROBOT_ASSEMBLY_LOCAL)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

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

        'todo find out what this -70 is from.
        Dim nHeight As Integer = tscMain.ContentPanel.Height - 70
        If nHeight < 100 Then nHeight = 100
        Dim nWidth As Integer = tscMain.ContentPanel.Width - (pnlMain.Left * 2)
        If nWidth < 100 Then nWidth = 100
        pnlMain.Height = nHeight
        pnlMain.Width = nWidth
        Application.DoEvents()
        subSizeGrid(mbFillScreen, mnShowPie)
        Application.DoEvents()
        '0=no,1=DT,2=single Chart 3=both charts
        Select Case mnShowPie
            Case 0
            Case 1 'DT summary
                subDoChart(imgPie, imgKey, DTChart1, moChartType, msChartTitle(0))
            Case 2 'single chart
                subDoChart(imgPie, imgKey, DTChart1, moChartType, msChartTitle(0))
            Case 3 'Both charts
                subDoChart(imgPie, imgKey, DTChart1, moChartType, msChartTitle(0))
                subDoChart(imgPie2, imgKey2, DTChart2, moChart2Type, msChartTitle(1))
        End Select
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
        ' 04/30/12  MSW     tlsMain_ItemClicked - Remove submenus from save button
        '********************************************************************************************
        Select Case e.ClickedItem.Name ' case sensitive
            Case "btnClose"
                'Check to see if we need to save is performed in  bAskForSave
                Me.Close()

            Case "btnStatus"
                lstStatus.Visible = Not lstStatus.Visible

            Case "btnSave"
                'privilege check done in subroutine
                'Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                'If o.DropDownButtonPressed Then Exit Sub
                subSaveData()

            Case "btnPrint"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                bPrintdoc(True)

            Case "btnCriteria"
                If frmCriteria.ZoneCollection Is Nothing Then
                    ' initial setup
                    frmCriteria.ZoneCollection = colZones
                End If
                If (mReportType <> mStartingReportType) Then
                    'this is for going from a chart back to downtime
                    mReportType = mStartingReportType
                    frmCriteria.ReportType = mReportType
                    frmCriteria.EndTime = frmCriteria.EndTime 'This saves the existing times
                    frmCriteria.ShowMe(True)
                Else
                    frmCriteria.ShowMe()
                End If
            Case "btnRefresh"
                frmCriteria.subExit(True)
            Case "btnChart"
                ReportType = eReportType.RMCharts
                If frmCriteria.ZoneCollection Is Nothing Then
                    ' initial setup
                    frmCriteria.ZoneCollection = colZones
                End If
                frmCriteria.ReportType = mReportType
                frmCriteria.EndTime = frmCriteria.EndTime 'This saves the existing times
                frmCriteria.ShowMe(True)

        End Select
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

        GC.Collect()
        System.Windows.Forms.Application.DoEvents()
        mPWRobotCommon.UrbanRenewal()
        System.Windows.Forms.Application.DoEvents()
        subInitializeForm()

    End Sub
    Private Sub btnSelect_DropDownItemClicked(ByVal sender As Object, _
         ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles btnSelect.DropDownItemClicked
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

        Select Case e.ClickedItem.Text
            Case gpsRM.GetString("psALARM_REPORT")
                ReportType = eReportType.Alarm
                btnCriteria.Enabled = True
            Case gpsRM.GetString("psCHANGE_REPORT")
                ReportType = eReportType.Change
                btnCriteria.Enabled = True
            Case gpsRM.GetString("psPRODUCTION_REPORT")
                ReportType = eReportType.Production
                btnCriteria.Enabled = True
            Case gpsRM.GetString("psDOWNTIME_REPORT")
                ReportType = eReportType.Downtime
                btnCriteria.Enabled = True
            Case gpsRM.GetString("psCONVEYOR_REPORT")
                ReportType = eReportType.Conveyor
                btnCriteria.Enabled = True
            Case gpsRM.GetString("psRMCHARTS")
                ReportType = eReportType.RMCharts
                btnCriteria.Enabled = True
            Case gpsRM.GetString("psWEEKLY_REPORT")
                ReportType = eReportType.RMChartsAuto
                btnCriteria.Enabled = True
            Case gpsRM.GetString("psVISION_REPORT")
                ReportType = eReportType.Vision
                btnCriteria.Enabled = True
        End Select

        mStartingReportType = ReportType
        btnRefresh.Enabled = btnCriteria.Enabled

        'Use form title for printout title
        mPrintHtml = New clsPrintHtml(e.ClickedItem.Text, True, 30, True, , , , True, True)

        Me.Text = e.ClickedItem.Text

        frmCriteria = Nothing

        If frmCriteria.ZoneCollection Is Nothing Then
            ' initial setup
            frmCriteria.ZoneCollection = colZones
        End If
        frmCriteria.ReportType = mReportType

        If ReportType = eReportType.RMChartsAuto Then
            mExcel.subDoAutoCharts(mdCmdDate)
            'mCharts.subDoAutoCharts(mdCmdDate)
        Else
            frmCriteria.ShowMe(True)
        End If

    End Sub

    Private Sub dgvMain_CellDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvMain.CellDoubleClick
        '********************************************************************************************
        'Description:  user double-clicked on grid
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/18/09  MSW     make it easier to get the pop-up boxes
        ' 11/23/09  MSW     Need to account for clicking on headers (index = -1)
        ' 01/08/10  MSW     Split click and double click functions, now it needs a double click to get 
        '                   the alarm list from most prod report columns, still do the cc cyles on a single click
        '********************************************************************************************
        'no pop-up options for normal summaries, only job summaries
        If frmCriteria.ReportSummaryType <> String.Empty Then
            If ReportType = eReportType.Alarm Then
                Exit Sub
            Else
                If frmCriteria.ReportSummaryType <> gpsRM.GetString("psJOB_REPORT_CAP") Then
                    Exit Sub
                End If
            End If
        End If
        If e.RowIndex < 0 Or e.ColumnIndex < 0 Then
            Exit Sub
        End If
        Select Case ReportType
            Case eReportType.Production
                mClickedCell = dgvMain.Rows(e.RowIndex).Cells(e.ColumnIndex)
                If dgvMain.Columns(e.ColumnIndex).HeaderText = gpsRM.GetString("psPURGE_STATUS_CAP") Then
                Else
                    'Puts up the alarm box for a production report
                    subGridContextMenuClicked()
                End If
            Case Else

        End Select

    End Sub
    Private Sub subDestroyHelpForm()
        '********************************************************************************************
        'Description:  Kill the Cause/Remedy Help Viewer form.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            If Not IsNothing(mfrmCauseRemedy) Then
                mfrmCauseRemedy.Hide()
                mfrmCauseRemedy.Dispose()
                mfrmCauseRemedy = Nothing
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDestroyHelpForm", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Private Function HelpFileName(ByVal AlarmCode As String) As String
        '********************************************************************************************
        'Description:  
        '
        'Parameters: Alarm Mnemonic
        'Returns:    HelpFileName
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 

        Try
            Dim nPos As Integer = Strings.InStr(AlarmCode, "-")

            If nPos > 0 Then
                Return Strings.Trim(Strings.Left(AlarmCode, nPos - 1))
            Else
                Return String.Empty
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: HelpFileName", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Return String.Empty
        End Try

    End Function

    Private Sub dgvMain_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvMain.CellClick
        '********************************************************************************************
        'Description:  user clicked on grid
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/18/09  MSW     make it easier to get the pop-up boxes
        ' 11/23/09  MSW     Need to account for clicking on headers (index = -1)
        ' 01/08/10  MSW     Split click and double click functions, now it needs a double click to get 
        '                   the alarm list from most prod report columns, still do the cc cyles on a single click
        ' 04/29/11  MSW     Add alarm help lookup like the active alarm screen.
        ' 03/12/13  RJO     Modify alarm help lookup to read Cause/Remedy for robot alarms from a robot controller
        '********************************************************************************************
        'no pop-up options for normal summaries, only job summaries
        If frmCriteria.ReportSummaryType <> String.Empty Then
            If ReportType = eReportType.Alarm Then
                Exit Sub
            Else
                If frmCriteria.ReportSummaryType <> gpsRM.GetString("psJOB_REPORT_CAP") Then
                    Exit Sub
                End If
            End If
        End If
        If e.RowIndex < 0 Or e.ColumnIndex < 0 Then
            Exit Sub
        End If
        Select Case ReportType
            Case eReportType.Production
                mClickedCell = dgvMain.Rows(e.RowIndex).Cells(e.ColumnIndex)
                If dgvMain.Columns(e.ColumnIndex).HeaderText = gpsRM.GetString("psPURGE_STATUS_CAP") Then
                    'no job summaries for this one
                    If frmCriteria.ReportSummaryType <> String.Empty Then
                        Exit Sub
                    End If
                    Debug.Print(mClickedCell.Value.ToString)
                    Dim sCycleList() As String = GetCycleNamesFromDB(colZones.ActiveZone, eColorChangeType.NONE)
                    frmMiniGrid.DisplayCycleList(sCycleList, CInt(mClickedCell.Value))
                End If
            Case eReportType.Alarm, eReportType.Downtime, eReportType.Conveyor, eReportType.RMCharts
                If dgvMain.Columns(e.ColumnIndex).Name.Equals(gpsRM.GetString("psALARM_CAP")) Then
                    mClickedCell = dgvMain.Rows(e.RowIndex).Cells(e.ColumnIndex)
                    Debug.Print(mClickedCell.Value.ToString)
                    Try
                        'If the Cause/Remedy form is already showing, dispose of it
                        Call subDestroyHelpForm()

                        'Show the Cause/Remedy form when the user clicks an alarm mnemonic link.
                        Dim sAlarmCode As String = mClickedCell.Value.ToString.Trim

                        If Not (sAlarmCode Is Nothing) Then
                            Dim sHelpFileName As String = HelpFileName(sAlarmCode)
                            Dim hrefAddress As String = String.Empty
                            Dim sBookMark As String = sHelpFileName & Strings.Right(sAlarmCode, 4)

                            Select Case sHelpFileName.ToUpper 'RJO 03/12/13
                                Case "PLCE", "PLCR", "PLCZ", "PWRK"
                                    'This is a PLC or GUI generated alarm. Cause/Remedy is in the Help\AlarmMan folder.
                                    hrefAddress = msHelpPath & "AlarmMan/" & sHelpFileName & ".HTM#" & sBookMark
                                Case Else
                                    'This is a Robot Controller generated alarm. Cause/Remedy is on the robot controller.
                                    hrefAddress = "http://" & msFanucName & "/FRH/DIAGEG/" & sHelpFileName & ".STM#" & sBookMark
                            End Select

                            mfrmCauseRemedy = New frmHelp
                            With mfrmCauseRemedy
                                .Show()
                                .Text = msHelpFormTitle
                                .Target = hrefAddress
                            End With

                        End If 'Not (sAlarmCode Is Nothing)

                    Catch ex As Exception
                        mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: dgvMain_CellClick", _
                                               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    End Try

                End If 'dgvMain.Columns(e.ColumnIndex).Name.Equals(gpsRM.GetString("psALARM_CAP"))

            Case Else

        End Select

    End Sub

    Private Sub dgvMain_MouseUp(ByVal sender As Object, _
                ByVal e As System.Windows.Forms.MouseEventArgs) Handles dgvMain.MouseUp
        '********************************************************************************************
        'Description:  user clicked on grid
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp As String

        'no pop-up options for normal summaries, only job summaries
        If frmCriteria.ReportSummaryType <> String.Empty Then
            If ReportType = eReportType.Alarm Then
                Exit Sub
            Else
                If frmCriteria.ReportSummaryType <> gpsRM.GetString("psJOB_REPORT_CAP") Then
                    Exit Sub
                End If
            End If
        End If

        Select Case ReportType
            Case eReportType.Alarm, eReportType.Production
                If e.Button = Windows.Forms.MouseButtons.Right Then
                    Dim hit As DataGridView.HitTestInfo = dgvMain.HitTest(e.X, e.Y)
                    If hit.Type = DataGridViewHitTestType.Cell Then
                        'select the cell clicked on - clicked cell is stored for use in callback
                        mClickedCell = dgvMain.Rows(hit.RowIndex).Cells(hit.ColumnIndex)
                        dgvMain.ClearSelection()
                        dgvMain.Rows(hit.RowIndex).Selected = True

                        If ReportType = eReportType.Alarm Then
                            sTmp = gpsRM.GetString("psCHECK_PROC_RECORDS")
                        Else
                            sTmp = gpsRM.GetString("psCHECK_ALARM_RECORDS")
                        End If

                        ' Create the menu item.
                        Dim tsmi As New ToolStripMenuItem(sTmp, _
                                            Nothing, AddressOf subGridContextMenuClicked)

                        ' Add the menu item to the shortcut menu.
                        Dim cMenu As New ContextMenuStrip()
                        cMenu.Items.Add(tsmi)
                        cMenu.Show(dgvMain, e.Location)

                    End If
                End If

            Case Else

        End Select
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
        ' 10/27/10  MSW     Remove execute and copychange to only use edit level
        '********************************************************************************************
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
            Call subEnableControls(True) 'RJO 10/24/10
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/23/12
        End If
    End Sub

    Private Sub moPassword_Result(ByVal Action As String, ByVal ReturnValue As String) Handles moPassword.Result
        '********************************************************************************************
        'Description: Result returned by usernames or screennames request
        '
        'Parameters: Action - operation name, ReturnValue - returned data
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case Action
            Case "screenames"
                If ReturnValue.Length > 0 Then
                    Dim sData() As String = Strings.Split(ReturnValue, ",")
                    ReDim msScreenNames(sData.GetUpperBound(0))
                    For nItem As Integer = 0 To sData.GetUpperBound(0)
                        msScreenNames(nItem) = sData(nItem)
                    Next 'nItem

                    'Check for hot edit logger or vision logger
                    Dim bHotEditLogger As Boolean = False
                    Dim bVisionLogger As Boolean = False
                    For Each oController As tController In mtControllerArray
                        If (bHotEditLogger = False) AndAlso (oController.HotEditLoggerDO > 0) Then
                            bHotEditLogger = True
                            ReDim Preserve msScreenNames(msScreenNames.GetUpperBound(0) + 1)
                            msScreenNames(msScreenNames.GetUpperBound(0)) = gs_HOTEDIT_LOGGER_NAME
                        End If
                        If (bVisionLogger = False) AndAlso (oController.VisionController) Then
                            bVisionLogger = True
                            ReDim Preserve msScreenNames(msScreenNames.GetUpperBound(0) + 1)
                            msScreenNames(msScreenNames.GetUpperBound(0)) = gs_VISION_LOGGER_NAME
                        End If
                    Next
                    System.Array.Sort(msScreenNames)
                    ReDim Preserve msScreenDisplayNames(msScreenNames.GetUpperBound(0))
                    For nItem As Integer = 0 To msScreenNames.GetUpperBound(0)
                        msScreenDisplayNames(nItem) = mPWCommon.sGetScreenDisplayName(msScreenNames(nItem))
                        Debug.Print(msScreenNames(nItem) & "  " & msScreenDisplayNames(nItem))
                    Next 'nItem
                Else
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: moPassword_Result", _
                                           "GetScreenNames returned an empty string.")
                End If

                mbGotScreenList = True
            Case "usernames"
                If ReturnValue.Length > 0 Then
                    Dim nItem As Integer = 0
                    Dim sData() As String = Strings.Split(ReturnValue, ",")

                    For nIndex As Integer = 0 To sData.GetUpperBound(0) Step 3
                        ReDim Preserve msUserNames(nItem)

                        msUserNames(nItem) = sData(nIndex)
                        nItem += 1
                    Next 'nIndex
                    ReDim Preserve msUserNames(msUserNames.GetUpperBound(0) + 1)
                    msUserNames(nItem) = gcsRM.GetString("csNO_USER")
                    'Check for hot edit logger or vision logger
                    For Each oController As tController In mtControllerArray
                        If (oController.HotEditLoggerDO > 0) Or oController.VisionController Then
                            ReDim Preserve msUserNames(msUserNames.GetUpperBound(0) + 1)
                            msUserNames(msUserNames.GetUpperBound(0)) = gcsRM.GetString("csTEACH_PENDANT_USER", mLanguage.FixedCulture)
                            Exit For
                        End If
                    Next
                    System.Array.Sort(msUserNames)
                Else
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: moPassword_Result", _
                                           "GetUserNames returned an empty string.")
                End If

                mbGotUserList = True
        End Select

    End Sub

#End Region
    Public Function sGetCompStat(ByVal nCompStat As Integer) As String
        '********************************************************************************************
        'Description:  get completion status string from integer
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/26/14  CBZ     fix completion status compare
        ' 05/20/14  MSW     Unfix - this may depend on the PLC setup.
        '********************************************************************************************
        Dim sReturn As String = String.Empty
        For Each oCompStat As clsCompStat In gCompStats
            If (oCompStat.bitValue And nCompStat) <> 0 Then '= nCompStat Then 'was <> 0
                Select Case oCompStat.Operation
                    Case clsCompStat.eCompStatOp.PREFIX
                        sReturn = oCompStat.Description & sReturn
                    Case clsCompStat.eCompStatOp.REPLACE
                        sReturn = oCompStat.Description
                    Case clsCompStat.eCompStatOp.SUFFIX
                        sReturn = sReturn & oCompStat.Description
                    Case Else

                End Select
            End If
        Next
        If sReturn = String.Empty Then
            sReturn = gpsRM.GetString("psJOB_STATUS00")
        End If
        Return (sReturn)
    End Function

    Private Sub dgvMain_CellFormatting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles dgvMain.CellFormatting
        '********************************************************************************************
        'Description:  take care of any special runtime formatting on the grid
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Select Case ReportType
                Case eReportType.Change
                    'Clean up the screen name display
                    If frmCriteria.Visible = False Then
                        If dgvMain.Columns(e.ColumnIndex).Name.Equals(gcsRM.GetString("csAREA_CAP")) Then
                            Dim sScreen As String = e.Value.ToString
                            e.Value = mPWCommon.sGetScreenDisplayName(sScreen)
                        End If
                    End If
                    '                   
                Case eReportType.Alarm
                    '
                Case eReportType.Conveyor
                    '
                Case eReportType.Production
                    'MSW 5/03/12 Numeric Completion Status
                    'Manage completion status as integer and change the display here instead of in DB
                    If frmCriteria.Visible = False AndAlso frmCriteria.ReportSummaryType = String.Empty Then
                        If dgvMain.Columns(e.ColumnIndex).Name.Equals(gpsRM.GetString("psVISION_STATUS_CAP")) Then
                            Dim sVisionStat As String = e.Value.ToString
                            e.Value = sGetVisionStat(sVisionStat)
                        End If
                    End If
                    If frmCriteria.Visible = False Then
                        Select Case frmCriteria.ReportSummaryType
                            Case gpsRM.GetString("psSTYLE_CAP"), gpsRM.GetString("psCOLOR_CAP"), gpsRM.GetString("psPAINT_TOTAL_CAP")
                                If e.ColumnIndex = 0 Then
                                    Select Case e.Value.GetType.ToString
                                        Case GetType(DateTime).ToString
                                            Dim dtVal As DateTime = DirectCast(e.Value, DateTime)
                                            If dtVal = frmCriteria.EndTime Then
                                                e.Value = gpsRM.GetString("psTOTAL_CAP")
                                            End If
                                    End Select
                                End If
                            Case Else
                        If dgvMain.Columns(e.ColumnIndex).Name.Equals(gpsRM.GetString("psJOB_STATUS_CAP")) Then
                            Dim sCompStat As String = e.Value.ToString
                            If IsNumeric(sCompStat) Then
                                Dim nCompStat As Integer = CType(sCompStat, Integer)

                                e.Value = sGetCompStat(nCompStat)
                            End If
                        End If
                        End Select
                    End If

                Case eReportType.Vision
                    'MSW 5/03/12 Numeric Completion Status
                    'Manage completion status as integer and change the display here instead of in DB
                    If frmCriteria.Visible = False AndAlso frmCriteria.ReportSummaryType = String.Empty Then
                        If dgvMain.Columns(e.ColumnIndex).Name.Equals(gpsRM.GetString("psVISION_STATUS_CAP")) Then
                            Dim sVisionStat As String = e.Value.ToString
                            e.Value = sGetVisionStat(sVisionStat)
                        End If
                    End If

                Case eReportType.Downtime, eReportType.RMCharts, eReportType.RMChartsAuto
                    ' Replace long values (seconds) in the Duration column with strings (HH:MM:SS)
                    ' This changes the display to strings, but lets the sort work with seconds as integers.
                    If frmCriteria.Visible = False Then
                        If frmCriteria.ReportSummaryType <> String.Empty Then
                            If dgvMain.Columns(e.ColumnIndex).Name.Equals(gpsRM.GetString("psDURATION_CAP")) Then
                                If InStr(e.Value.ToString, ":") = 0 Then
                                    'Sometimes this gets errors on odd data.  I'm not sure why, but it 
                                    ' seems to work OK if the error handler just ignores it.
                                    Dim longValue As Long = DirectCast(e.Value, Long)
                                    'If longValue Is Nothing Then Return
                                    e.Value = sGetDurationString(longValue)
                                End If
                            End If
                        End If
                    End If
                Case Else
                    'Nothing
            End Select


        Catch ex As Exception

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
        If (dgvMain IsNot Nothing) Then
            bPrintdoc(True)
            mPrintHtml.subShowPageSetup()
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
        bPrintdoc(True)

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

        If DataLoaded Then
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

    Private Function bPrintdoc(ByVal bPrint As Boolean, Optional ByVal bExport As Boolean = False, Optional ByVal bAutoName As Boolean = False) As Boolean
        '********************************************************************************************
        'Description:  build the print document
        '
        'Parameters:  bPrint - send to printer
        'Returns:   true if successful, OK to continue with preview or page setup
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/17/12  BTK     Made a change to for downtime summary formatting.  Call dtSummarizeDataTablePrint
        '                   to format datatable before printing.
        '********************************************************************************************
        Dim sCaption As String = gpsRM.GetString("psSCREENCAPTION")
        Dim sFileName As String = String.Empty
        If bAutoName Then
            Dim sFile As String = String.Empty
            Select Case ReportType
                Case eReportType.Alarm
                    sFile = gpsRM.GetString("psALARM_REPORT")
                Case eReportType.Change
                    sFile = gpsRM.GetString("psCHANGE_REPORT")
                Case eReportType.Production
                    sFile = gpsRM.GetString("psPRODUCTION_REPORT")
                Case eReportType.Downtime
                    sFile = gpsRM.GetString("psDOWNTIME_REPORT")
                Case eReportType.Conveyor
                    sFile = gpsRM.GetString("psCONVEYOR_REPORT")
                Case eReportType.RMCharts
                    sFile = gpsRM.GetString("psRMCHARTS")
                Case eReportType.RMChartsAuto
                    sFile = gpsRM.GetString("psWEEKLY_REPORT")
                Case eReportType.Vision
                    sFile = gpsRM.GetString("psVISION_REPORT")
            End Select
            If frmCriteria.ReportSummaryType <> String.Empty Then
                sFile = sFile & " " & frmCriteria.ReportSummaryType
            End If
            sFile = sFile & " " & frmCriteria.EndTime.Year & "_" & frmCriteria.EndTime.Month.ToString("00") & "_" & _
            frmCriteria.EndTime.Day.ToString("00") & "_" & frmCriteria.EndTime.Hour.ToString("00") & frmCriteria.EndTime.Minute.ToString("00")
            sFile = sFile & msAutoGenFormat
            If GetDefaultFilePath(sFileName, eDir.Reports, String.Empty, sFile) Then
                File.Delete(sFileName)
            End If
        End If
        Select Case ReportType
            Case  eReportType.Alarm
                sCaption = gpsRM.GetString("psALARM_REPORT")
            Case  eReportType.Change
                sCaption = gpsRM.GetString("psCHANGE_REPORT")
            Case  eReportType.Production
                sCaption = gpsRM.GetString("psPRODUCTION_REPORT")
            Case  eReportType.Downtime
                sCaption = gpsRM.GetString("psDOWNTIME_REPORT")
            Case  eReportType.Conveyor
                sCaption = gpsRM.GetString("psCONVEYOR_REPORT")
            Case  eReportType.RMCharts
                sCaption = gpsRM.GetString("psRMCHARTS")
            Case  eReportType.RMChartsAuto
                sCaption = gpsRM.GetString("psWEEKLY_REPORT")
            Case eReportType.Vision
                sCaption = gpsRM.GetString("psVISION_REPORT")
        End Select

        If (dgvMain IsNot Nothing) Then
            If (bExport = False) And (dgvMain.Rows.Count > 200) And (bAutoName = False) Then

                Dim sMsg As String = gpsRM.GetString("psLOTSA_RECORDS")
                sMsg = Replace(sMsg, "%s", dgvMain.Rows.Count.ToString)
                Dim reply As DialogResult = _
                        MessageBox.Show(sMsg, gpsRM.GetString("psSCREENCAPTION"), _
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question, _
                        MessageBoxDefaultButton.Button2, MessageBoxOptions.DefaultDesktopOnly)

                If reply <> Windows.Forms.DialogResult.OK Then Exit Function

            End If  'dgvMain.Rows.Count > 200 

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            btnPrint.Enabled = False
            btnClose.Enabled = False  ' if close before done, bad things happen

            Progress = 10
            Try
                mPrintHtml.subSetPageFormat()
                mPrintHtml.subClearTableFormat()
                mPrintHtml.subSetColumnCfg("Bold=on,align=right", 0)
                ' mPrintHtml.subSetRowcfg("Bold=on", 0, nBoldRows - 1)
                Dim sTitle(1) As String
                sTitle(0) = sCaption
                sTitle(1) = colZones.SiteName
                Dim sSubTitle(mnMAX_TITLES) As String
                If bExport Then
                    ReDim sSubTitle(mnMAX_TITLES * 2 + 1)
                End If
                'Dim sTmp As String = colZones.SiteName '& vbTab & colZones.CurrentZone
                For nTitle As Integer = 0 To mnMAX_TITLES
                    Dim l As Label = DirectCast(pnlMain.Controls("lblTitle" & Format(nTitle, "00")), Label)
                    Dim sTmp As String = " "
                    If l.Text <> String.Empty Then
                        sTmp = l.Text
                    End If
                    If bExport Then
                        sSubTitle(nTitle * 2) = sTmp
                        sSubTitle((nTitle * 2) + 1) = String.Empty
                    Else
                        sSubTitle(nTitle) = sTmp
                    End If
                Next
                If bExport Then
                    mPrintHtml.SubTitleCols = 6
                End If

                'BTK 04/17/12 Added to format downtime summary reports correctly.
                Select Case ReportType
                    Case eReportType.RMCharts
                        'Case gpsRM.GetString("psDURATION_CAP"), gpsRM.GetString("psOCCURRENCES_CAP")
                        'Case gpsRM.GetString("psBOTH_CAP")
                        Dim bBoth As Boolean = (frmCriteria.ReportSummaryType = gpsRM.GetString("psBOTH_CAP"))
                        Dim DT1 As DataTable = dtSummarizeDataTablePrint(DTChart1)
                        Dim DT2 As DataTable = Nothing
                        If bBoth Then
                            DT2 = dtSummarizeDataTablePrint(DTChart2)
                        End If

                        Dim bCancelled As Boolean = False
                        Dim oExportFormat As clsPrintHtml.eExportFormat
                        Dim nColWidths(4) As Single
                        nColWidths(0) = 0.2
                        nColWidths(1) = 1.2
                        nColWidths(2) = 1.5
                        nColWidths(3) = 2
                        nColWidths(4) = 3.8
                        mPrintHtml.ColumnWidthsList = nColWidths
                        Dim nColWidthIndex(4) As Integer
                        nColWidthIndex(0) = 3
                        nColWidthIndex(1) = 2
                        nColWidthIndex(2) = 3
                        nColWidthIndex(3) = 0
                        nColWidthIndex(4) = 3
                        mPrintHtml.ColumnWidthIndex = nColWidthIndex
                        mPrintHtml.subStartDoc(Status, sCaption, bExport, bCancelled, sFileName, oExportFormat)
                        Dim oSC As New ScreenShot.ScreenCapture
                        Application.DoEvents()
                        If (bCancelled = False) Then
                            If bExport Then
                                Select Case oExportFormat
                                    Case clsPrintHtml.eExportFormat.nXLS
                                        'Supports pictures
                                        Dim sTmpFile As String = mPWCommon.sGetTmpFileName("ChartPic", "jpg")
                                        oSC.CaptureWindowToFile(tlpChart.Handle, sTmpFile, Imaging.ImageFormat.Jpeg)
                                        mPrintHtml.subAddPicture(sTmpFile, "", sTitle, sSubTitle, gpsRM.GetString("psCHART"))
                                    Case clsPrintHtml.eExportFormat.nODS
                                        'Supports pictures
                                        Dim sTmpFile As String = mPWCommon.sGetTmpFileName("ChartPic", "jpg")
                                        oSC.CaptureWindowToFile(tlpChart.Handle, sTmpFile, Imaging.ImageFormat.Jpeg)
                                        mPrintHtml.subAddPicture(sTmpFile, "", sTitle, sSubTitle, gpsRM.GetString("psCHART"), "A", "7", "M", "38")
                                    Case clsPrintHtml.eExportFormat.nXLSX
                                        'Supports pictures
                                        Dim sTmpFile As String = mPWCommon.sGetTmpFileName("ChartPic", "png")
                                        oSC.CaptureWindowToFile(tlpChart.Handle, sTmpFile, Imaging.ImageFormat.Png)
                                        mPrintHtml.subAddPicture(sTmpFile, "", sTitle, sSubTitle, gpsRM.GetString("psCHART"), "A", "7", "S", "39")
                                    Case Else
                                        'no pictures
                                End Select
                            Else
                                'Supports pictures
                                Dim sTmpFile As String = mPWCommon.sGetTmpFileName("ChartPic", "jpg")
                                oSC.CaptureWindowToFile(tlpChart.Handle, sTmpFile, Imaging.ImageFormat.Jpeg)
                                mPrintHtml.subAddPicture(sTmpFile, "", sTitle, sSubTitle, gpsRM.GetString("psCHART"))
                                mPrintHtml.subAddPageBreak(Status)
                            End If
                            ReDim nColWidthIndex(5)
                            nColWidthIndex(0) = 2
                            nColWidthIndex(1) = 4
                            nColWidthIndex(2) = 3
                            nColWidthIndex(3) = 1
                            nColWidthIndex(4) = 3
                            nColWidthIndex(5) = 1
                            mPrintHtml.ColumnWidthIndex = nColWidthIndex
                            Select Case frmCriteria.ReportSummaryType
                                Case gpsRM.GetString("psDURATION_CAP"), gpsRM.GetString("psOCCURRENCES_CAP")
                                    mPrintHtml.subAddObject(DT1, Status, sTitle, sSubTitle, frmCriteria.ReportSummaryType)
                                Case gpsRM.GetString("psBOTH_CAP")
                                    mPrintHtml.subAddObject(DT1, Status, sTitle, sSubTitle, gpsRM.GetString("psDURATION_CAP"))
                                    mPrintHtml.subAddObject(DT2, Status, sTitle, sSubTitle, gpsRM.GetString("psOCCURRENCES_CAP"))
                            End Select
                            ReDim Preserve sSubTitle(6)
                            mPrintHtml.subAddObject(DTChart0, Status, sTitle, sSubTitle, gpsRM.GetString("psALARMS_SHEET_NAME"))
                            mPrintHtml.subCloseFile(Status)
                        End If


                    Case eReportType.Downtime
                        Dim DT As DataTable
                        Dim DT1 As DataTable = CType(dgvMain.DataSource, DataTable)

                        If frmCriteria.ReportSummaryType = String.Empty Then
                            DT = DT1
                            mPrintHtml.subCreateSimpleDoc(DT, Status, sCaption, sTitle, sSubTitle, bExport, , sFileName)
                        Else
                            DT = dtSummarizeDataTablePrint(DT1)
                            Dim bCancelled As Boolean = False
                            Dim oExportFormat As clsPrintHtml.eExportFormat
                            mPrintHtml.subStartDoc(Status, sCaption, bExport, bCancelled, sFileName, oExportFormat)
                            Dim oSC As New ScreenShot.ScreenCapture
                            Application.DoEvents()
                            If (bCancelled = False) Then
                                If bExport Then
                                    Select Case oExportFormat
                                        Case clsPrintHtml.eExportFormat.nXLS
                                            'Supports pictures
                                            Dim sTmpFile As String = mPWCommon.sGetTmpFileName("ChartPic", "jpg")
                                            oSC.CaptureWindowToFile(tlpChart.Handle, sTmpFile, Imaging.ImageFormat.Jpeg)
                                            mPrintHtml.subAddPicture(sTmpFile, "", sTitle, sSubTitle, gpsRM.GetString("psCHART"))
                                        Case clsPrintHtml.eExportFormat.nODS
                                            'Supports pictures
                                            Dim sTmpFile As String = mPWCommon.sGetTmpFileName("ChartPic", "jpg")
                                            oSC.CaptureWindowToFile(tlpChart.Handle, sTmpFile, Imaging.ImageFormat.Jpeg)
                                            mPrintHtml.subAddPicture(sTmpFile, "", sTitle, sSubTitle, gpsRM.GetString("psCHART"), "A", "7", "F", "38")
                                        Case clsPrintHtml.eExportFormat.nXLSX
                                            'Supports pictures
                                            Dim sTmpFile As String = mPWCommon.sGetTmpFileName("ChartPic", "png")
                                            oSC.CaptureWindowToFile(tlpChart.Handle, sTmpFile, Imaging.ImageFormat.Png)
                                            mPrintHtml.subAddPicture(sTmpFile, "", sTitle, sSubTitle, gpsRM.GetString("psCHART"), "A", "7", "G", "39")
                                        Case Else
                                            'no pictures
                                    End Select
                                Else
                                    'Supports pictures
                                    Dim sTmpFile As String = mPWCommon.sGetTmpFileName("ChartPic", "jpg")
                                    oSC.CaptureWindowToFile(tlpChart.Handle, sTmpFile, Imaging.ImageFormat.Jpeg)
                                    mPrintHtml.subAddPicture(sTmpFile, "", sTitle, sSubTitle, gpsRM.GetString("psCHART"))
                                    mPrintHtml.subAddPageBreak(Status)
                                End If

                                mPrintHtml.subAddObject(DT, Status, sTitle, sSubTitle, sCaption)
                                mPrintHtml.subCloseFile(Status)
                            End If
                        End If


                    Case eReportType.Production
                        Select Case frmCriteria.ReportSummaryType
                            Case String.Empty, gpsRM.GetString("psJOB_REPORT_CAP")
                                'MSW 5/03/12 Numeric Completion Status
                                'Manage completion status as integer and change the display here instead of in DB
                                Dim DT1 As DataTable = CType(dgvMain.DataSource, DataTable)
                                Dim DT As DataTable = dtConvertProdTable(DT1)
                                mPrintHtml.subCreateSimpleDoc(DT, Status, sCaption, sTitle, sSubTitle, bExport, , sFileName)

                            Case Else
                                mPrintHtml.subCreateSimpleDoc(dgvMain, Status, sCaption, sTitle, sSubTitle, bExport, , sFileName)
                        End Select
                    Case eReportType.Vision
                        Select Case frmCriteria.ReportSummaryType
                            Case String.Empty
                                'Manage completion status as integer and change the display here instead of in DB
                                Dim DT1 As DataTable = CType(dgvMain.DataSource, DataTable)
                                Dim DT As DataTable = dtConvertVisionTable(DT1)
                                mPrintHtml.subCreateSimpleDoc(DT, Status, sCaption, sTitle, sSubTitle, bExport, , sFileName)
                            Case gpsRM.GetString("psSTYLE_CAP")
                                mPrintHtml.subCreateSimpleDoc(dgvMain, Status, sCaption, sTitle, sSubTitle, bExport, , sFileName)
                            Case gpsRM.GetString("psVISION_STATUS_CAP")
                                mPrintHtml.subCreateSimpleDoc(dgvMain, Status, sCaption, sTitle, sSubTitle, bExport, , sFileName)
                            Case Else
                                mPrintHtml.subCreateSimpleDoc(dgvMain, Status, sCaption, sTitle, sSubTitle, bExport, , sFileName)
                        End Select
                    Case Else
                        mPrintHtml.subCreateSimpleDoc(dgvMain, Status, sCaption, sTitle, sSubTitle, bExport, , sFileName)
                End Select
                If bPrint Then
                    mPrintHtml.subPrintDoc()
                End If
                Me.Cursor = System.Windows.Forms.Cursors.Default
                subEnableControls(True)
                Return (True)
            Catch ex As Exception
                subEnableControls(True)
                Return False
            End Try

        Else
            subEnableControls(True)
            Return (False)
        End If
    End Function

    Private Sub imgPie_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles imgPie.Click, imgKey2.Click
        Select Case moChartType
            Case eChartTypes.Bar
                subDoChart(imgPie, imgKey, DTChart1, eChartTypes.Pie, msChartTitle(0))
                moChartType = eChartTypes.Pie
            Case eChartTypes.Pie
                subDoChart(imgPie, imgKey, DTChart1, eChartTypes.Bar, msChartTitle(0))
                moChartType = eChartTypes.Bar
        End Select
    End Sub
    Private Sub imgPie2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles imgPie2.Click, imgPie3.Click
        Select Case moChart2Type
            Case eChartTypes.Bar
                subDoChart(imgPie2, imgKey2, DTChart2, eChartTypes.Pie, msChartTitle(1))
                moChart2Type = eChartTypes.Pie
            Case eChartTypes.Pie
                subDoChart(imgPie2, imgKey2, DTChart2, eChartTypes.Bar, msChartTitle(1))
                moChart2Type = eChartTypes.Bar
            Case eChartTypes.NoChange
                'Leave it alone
        End Select
    End Sub
End Class
Class clsCompStat
    Public Enum eCompStatOp As Integer
        REPLACE
        SUFFIX
        PREFIX
    End Enum
    Public Description As String = String.Empty
    Public bitValue As Integer = 0
    Public Operation As eCompStatOp
End Class
Class clsCboItem
    Public DB As String = String.Empty
    Public Display As String = String.Empty

End Class