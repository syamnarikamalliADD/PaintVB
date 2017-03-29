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
' Form/Module: mCharts
'
' Description:  New chart routines made to work without Excel.  
'                
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: FANUC Programmer
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    06/07/12   MSW     Change charts to output a picture without linking to excel      4.01.04.00
'******************************************************************************************************
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Configuration.ConfigurationSettings
Imports Response = System.Windows.Forms.DialogResult
Imports System.Text
Friend Module mCharts
    Private Const msMODULE_NAME As String = "mCharts.vb"
    Friend Structure tChartConfig
        '        Dim TableRow As Integer
        '        Dim TableCol As Integer
        '        Dim ChartTop As Integer
        '        Dim ChartLeft As Integer
        '        Dim ChartHeight As Integer
        '        Dim ChartWidth As Integer
        Dim TableSheet As String
        Dim ChartSheet As String
        'Dim AlarmsRow1 As Integer
        Dim ByOccurrence As Boolean
        Dim NumAlarms As Integer
        Dim TopxxReq As Integer
        'Dim OccurCol As Integer
        'Dim DurCol As Integer
        'Dim AlarmLabelCol As Integer
        Dim Title As String
        Dim UptimePct As Double
        Dim MTTR As Integer
        Dim MTBF As Integer
        Dim StartTime As Date
        Dim EndTime As Date
        Dim Daily As Boolean
    End Structure

    Friend Enum eChartTitles
        Zone = 0
        Table = 1
        StartTime = 2
        EndTime = 3
        UptimePct = 4
        MTBF = 5
        MTTR = 6
        Max = 6
    End Enum
    Friend Enum eChartTypes
        Bar = 0
        Pie = 1
        NoChange = 2
    End Enum
    Private meWeeklyChartLengthUnits As DateInterval = DateInterval.WeekOfYear
    Private mnWeeklyChartLength As Integer = 26
    Private meWeeklyStorageLengthUnits As DateInterval = DateInterval.WeekOfYear
    Private mnWeeklyStorageLength As Integer = 52

    Friend Function sGetColumnLetterFromNumber(ByVal nCol As Integer) As String
        '********************************************************************************************
        'Description:  returns the excel column letter from a number up to two characters
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sColLetter As String = String.Empty
        Dim nCol2 As Integer = 0
        Do While nCol > 26
            nCol2 += 1
            nCol -= 26
        Loop
        If nCol2 > 0 Then
            sColLetter = Chr(64 + nCol2)
        End If
        sColLetter = sColLetter & Chr(64 + nCol)
        Return sColLetter
    End Function
   

    Friend Sub subGetChartData(ByRef DTin As DataTable, ByRef DTout As DataTable, ByRef sTitle As String, ByRef ChartConfig As tChartConfig)
        '********************************************************************************************
        'Description:  Get chart data from DB, frmCriteria
        '
        'Parameters: ChartCriteria selects Occurrence or Duration,  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'grab the initial table from the criteria form
        frmCriteria.bGetDataSet()

        frmMain.Progress = 25
        Application.DoEvents()
        ' summarize to get time values
        Dim sSummType As String = String.Empty
        If ChartConfig.ByOccurrence Then
            sSummType = gpsRM.GetString("psOCCURRENCES_CAP")
        Else
            sSummType = gpsRM.GetString("psDURATION_CAP")
        End If
        DTout = frmMain.dtSummarizeDataTable(DTin, ChartConfig, sSummType)
        'Add in some more details for the title section before the table
        If (ChartConfig.TopxxReq > 0) Then
            sTitle = gpsRM.GetString("psTOP_X") & ChartConfig.TopxxReq.ToString & " " & gpsRM.GetString("psPIE_TITLE_DT") & " " & sSummType
        Else
            sTitle = gpsRM.GetString("psPIE_TITLE_DT") & " " & sSummType
        End If

    End Sub
    Friend Sub subAddArrayToTotalsDT(ByRef drArray As DataRow(), ByRef DT_Totals As DataTable)
        '********************************************************************************************
        'Description: Add a topx table to a total table
        '
        'Parameters:  drArray topx as array of datarows
        '           DT_Totals - add alarms to this table
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sAlarm As String = String.Empty
        Dim nOccurrences As Integer = 0
        Dim nDuration As Integer = 0
        Dim bFound As Boolean = False

        For Each DR_Alarm As DataRow In drArray
            sAlarm = DR_Alarm.Item(gpsRM.GetString("psALARM_CAP")).ToString
            If (sAlarm <> gpsRM.GetString("psOVERALL")) Then
                nOccurrences = CInt(DR_Alarm.Item(gpsRM.GetString("psOCCURRENCES_CAP")))
            Else
                nOccurrences = 0
            End If
            nDuration = CInt(DR_Alarm.Item(gpsRM.GetString("psDURATION_CAP")))
            bFound = False
            For Each DR_TotAlarm As DataRow In DT_Totals.Rows
                If sAlarm = DR_TotAlarm.Item(gpsRM.GetString("psALARM_CAP")).ToString Then
                    bFound = True
                    If (sAlarm <> gpsRM.GetString("psOVERALL")) Then
                        nOccurrences += CInt(DR_TotAlarm.Item(gpsRM.GetString("psOCCURRENCES_CAP")))
                    Else
                        nOccurrences = 0
                    End If
                    nDuration += CInt(DR_TotAlarm.Item(gpsRM.GetString("psDURATION_CAP")))
                    DR_TotAlarm.Item(gpsRM.GetString("psOCCURRENCES_CAP")) = nOccurrences
                    DR_TotAlarm.Item(gpsRM.GetString("psDURATION_CAP")) = nDuration
                End If
            Next
            If bFound = False Then
                'Not found in the total table, add a record
                Dim DR_Add As DataRow = DT_Totals.NewRow
                DR_Add.BeginEdit()
                For nColNum As Integer = 0 To DT_Totals.Columns.Count - 1
                    DR_Add.Item(DT_Totals.Columns.Item(nColNum).Caption) = DR_Alarm.Item(DT_Totals.Columns.Item(nColNum).Caption)
                Next
                DR_Add.EndEdit()
                DT_Totals.Rows.Add(DR_Add)
            End If
        Next
    End Sub
    Friend Sub subGetTopxTotals(ByRef DT_Week As DataTable, ByRef DR_Duration() As DataRow, _
                                   ByRef DR_Occurrence() As DataRow, ByVal nTopx As Integer)
        '********************************************************************************************
        'Description: Take a weekly summary table, take the top 5 by duration and occurrence and add
        '               to the total tables
        '
        'Parameters:  DT_Week - one week summary, source data
        '           DT_Occurrence_Totals - add top x occurrence alarms to this table
        '           DT_Duration_Totals - add top x duration alarms to this table
        '           nTopx - how many alarms to take off the top
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'strip out the extra rows
        Dim sAlarm As String = String.Empty
        For nRow As Integer = DT_Week.Rows.Count - 1 To 0 Step -1
            Dim DR As DataRow = DT_Week.Rows(nRow)
            sAlarm = DR.Item(gpsRM.GetString("psALARM_CAP")).ToString
            If (sAlarm = gpsRM.GetString("psOVERALL")) Or (sAlarm = gpsRM.GetString("psTOTAL_CAP")) Then
                DT_Week.Rows.Remove(DR)
            End If
        Next

        'sort the current week by occurrence
        Dim sFilter As String = gpsRM.GetString("psOCCURRENCES_CAP") & " <> Null"
        Dim drArray() As DataRow = DT_Week.Select(String.Empty, gpsRM.GetString("psOCCURRENCES_CAP"), DataViewRowState.CurrentRows)
        Dim nMax As Integer = nTopx - 1
        If nMax > drArray.GetUpperBound(0) Then
            nMax = drArray.GetUpperBound(0)
        End If
        ReDim DR_Occurrence(nMax)
        'Take the topx into a new array
        For nRow As Integer = 0 To nMax
            DR_Occurrence(nRow) = drArray(nRow + drArray.GetUpperBound(0) - DR_Occurrence.GetUpperBound(0))
        Next

        'sort the current week by duration
        sFilter = gpsRM.GetString("psDURATION_CAP") & " <> Null"
        drArray = DT_Week.Select(String.Empty, gpsRM.GetString("psDURATION_CAP"), DataViewRowState.CurrentRows)
        'Take the topx into a new array
        nMax = nTopx - 1
        If nMax > drArray.GetUpperBound(0) Then
            nMax = drArray.GetUpperBound(0)
        End If
        ReDim DR_Duration(nMax)
        For nRow As Integer = 0 To nMax
            DR_Duration(nRow) = drArray(nRow + drArray.GetUpperBound(0) - DR_Duration.GetUpperBound(0))
        Next

    End Sub
    Friend Function sGetChartTimeUnit(ByVal sUnit As String) As DateInterval
        '********************************************************************************************
        'Description: convert db text to enumerated value
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Select Case Trim(sUnit.ToLower)
            Case "day"
                Return DateInterval.Day
            Case "week"
                Return DateInterval.WeekOfYear
            Case "month"
                Return DateInterval.Month
            Case "year"
                Return DateInterval.Year
        End Select
    End Function
    Friend Function dtGetWeeklyDataFromDB(ByRef sTableName As String) As DataTable
        '********************************************************************************************
        'Description: Get 1 weekly downtime table from DB
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sDB As String = gsWEEKLY_REPORT_DBNAME
        Dim DS As New DataSet
        Dim DT As DataTable = Nothing
        Try

            DS.Locale = mLanguage.FixedCulture

            Dim DB As New clsSQLAccess
            '********************************************************************************************
            'Start by saving this week's summary
            With DB
                .Zone = frmMain.colZones.ActiveZone
                .DBFileName = gsWEEKLY_REPORT_DBNAME

                .DBTableName = sTableName
                .SQLString = "SELECT * FROM [" & sTableName & "]"
                DS = .GetDataSet(False)
            End With

            If DS.Tables.Contains("[" & sTableName & "]") Then
                DT = DS.Tables("[" & sTableName & "]")
                Return DT
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Dim lReply As Windows.Forms.DialogResult = Response.OK
            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msMODULE_NAME, _
            frmMain.Status, MessageBoxButtons.OK)
            Return Nothing
        End Try
    End Function
    Friend Function dtSaveWeeklyDataToDB(ByRef DTSource As DataTable, ByRef sTableName As String, _
                                          ByRef ChartConfig As tChartConfig) As DataTable
        '********************************************************************************************
        'Description: Save weekly downtime data to DB
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sDB As String = gsWEEKLY_REPORT_DBNAME
        Dim DS As New DataSet
        Dim DT_DB_Table As DataTable = Nothing
        Dim s As SqlClient.SqlCommand
        Try

            DS.Locale = mLanguage.FixedCulture

            Dim DB As New clsSQLAccess
            '********************************************************************************************
            'Start by saving this week's summary
            With DB
                .Zone = frmMain.colZones.ActiveZone
                .DBFileName = gsWEEKLY_REPORT_DBNAME

                .DBTableName = sTableName
                .SQLString = "SELECT * FROM [" & sTableName & "]"

                s = .GetStoredProcedureCommand("CreateWeeklyTable")
                s.Parameters.Add(New SqlClient.SqlParameter("@TableName", _
                    SqlDbType.NVarChar)).Value = sTableName
                'do it
                Dim returnrows As Integer = s.ExecuteNonQuery()
                DS = .GetDataSet(False)
            End With

            If DS.Tables.Contains("[" & sTableName & "]") Then
                DT_DB_Table = DS.Tables("[" & sTableName & "]")
                'Should be empty, but deal with a redo by deleting all the rows
                Do While DT_DB_Table.Rows.Count > 0
                    DT_DB_Table.Rows.RemoveAt(DT_DB_Table.Rows.Count - 1)
                Loop
            Else
                Return Nothing
            End If

            Dim DR1 As DataRow
            Dim DR2 As DataRow
            For nRow As Integer = 0 To DTSource.Rows.Count - 1
                DR1 = DTSource.Rows.Item(nRow)
                DR2 = DT_DB_Table.NewRow
                DR2.BeginEdit()
                DR2.Item(gsWEEKLY_COL_ID) = nRow
                For nCol As Integer = 0 To DTSource.Columns.Count - 1
                    DR2.Item(DTSource.Columns.Item(nCol).Caption) = DR1.Item(DTSource.Columns.Item(nCol).Caption)
                Next
                DR2.EndEdit()
                DT_DB_Table.Rows.Add(DR2)
            Next

            DB.UpdateDataSet(DS, sTableName)

            '********************************************************************************************
            'Read some options
            Dim DT_Options As DataTable
            With DB
                .Zone = frmMain.colZones.ActiveZone
                .DBFileName = gsWEEKLY_REPORT_DBNAME
                .DBTableName = gsWEEKLY_OPT_TBLNAME
                .SQLString = "SELECT * FROM [" & gsWEEKLY_OPT_TBLNAME & "]"
                DS = .GetDataSet(False)
            End With

            If DS.Tables.Contains("[" & gsWEEKLY_OPT_TBLNAME & "]") Then
                DT_Options = DS.Tables("[" & gsWEEKLY_OPT_TBLNAME & "]")
                Dim DR As DataRow = DT_Options.Rows(0)
                meWeeklyChartLengthUnits = sGetChartTimeUnit(DR.Item(gsWEEKLY_OPT_COL_CHART_UNIT).ToString)
                mnWeeklyChartLength = CInt(DR.Item(gsWEEKLY_OPT_COL_CHART_LEN).ToString)
                meWeeklyStorageLengthUnits = sGetChartTimeUnit(DR.Item(gsWEEKLY_OPT_COL_STOR_UNIT).ToString)
                mnWeeklyStorageLength = CInt(DR.Item(gsWEEKLY_OPT_COL_STOR_LEN).ToString)
            End If

            '********************************************************************************************
            'Update tablelist
            With DB
                .DBTableName = gsWEEKLY_LIST_TBLNAME
                .SQLString = "SELECT * FROM [" & gsWEEKLY_LIST_TBLNAME & "]"
                DS = .GetDataSet(False)
            End With
            Dim DTList As DataTable
            'Delete old files before the cutoff date
            Dim dDeletionDate As Date = #1/1/2001#
            Dim dDate As Date = ChartConfig.EndTime.Date
            'for month or year intervals, round the time down
            Select Case meWeeklyStorageLengthUnits
                Case DateInterval.Year
                    dDate = DateAdd(DateInterval.Day, ((dDate.DayOfYear * -1) + 1), dDate)
                Case DateInterval.Month
                    dDate = DateAdd(DateInterval.Day, ((dDate.Day * -1) + 1), dDate)
                Case Else
                    'No need to round for day or week
            End Select
            dDeletionDate = DateAdd(meWeeklyStorageLengthUnits, (-1 * mnWeeklyStorageLength), dDate)
            If DS.Tables.Contains("[" & gsWEEKLY_LIST_TBLNAME & "]") Then
                DTList = DS.Tables("[" & gsWEEKLY_LIST_TBLNAME & "]")
                Dim bAddThisWeek As Boolean = True
                'Track the unique primary keys in case we have to add one
                Dim nID As Integer = 0
                Dim nIDTmp As Integer = 0
                'Count down so we can delete records on the way through
                For nRow As Integer = DTList.Rows.Count - 1 To 0 Step -1
                    Dim DR As DataRow = DTList.Rows.Item(nRow)
                    'Track the primary key
                    nIDTmp = CInt(DR.Item(gsWEEKLY_COL_ID).ToString)
                    If nIDTmp >= nID Then
                        nID = nIDTmp + 1
                    End If

                    'Look for a match to this week
                    Dim sTmpName As String = DR.Item(gsWEEKLY_LIST_COL_NAME).ToString
                    Dim dTmpDate As Date = CDate(DR.Item(gsWEEKLY_LIST_COL_DATE).ToString)
                    If sTmpName = sTableName Or dTmpDate.Date = ChartConfig.EndTime.Date Then
                        'Match - don't add this week, just make sure it's correct
                        DR.BeginEdit()
                        DR.Item(gsWEEKLY_LIST_COL_DATE) = ChartConfig.EndTime
                        DR.Item(gsWEEKLY_LIST_COL_NAME) = sTableName
                        DR.Item(gsWEEKLY_LIST_COL_MTBF) = ChartConfig.MTBF
                        DR.Item(gsWEEKLY_LIST_COL_MTTR) = ChartConfig.MTTR
                        DR.Item(gsWEEKLY_LIST_COL_UPTIME) = ChartConfig.UptimePct
                        DR.EndEdit()
                        bAddThisWeek = False
                        'If the dates match but the table name didn't, delete the table
                        If (sTableName <> sTmpName) Then
                            s = DB.GetStoredProcedureCommand("DeleteWeeklyTable")
                            s.Parameters.Add(New SqlClient.SqlParameter("@TableName", _
                                SqlDbType.NVarChar)).Value = sTmpName
                            'do it
                            s.ExecuteNonQuery()
                        End If
                    Else
                        'Look for tables to delete because they're too old
                        If (dTmpDate < dDeletionDate) Then
                            DTList.Rows.Remove(DR)
                            s = DB.GetStoredProcedureCommand("DeleteWeeklyTable")
                            s.Parameters.Add(New SqlClient.SqlParameter("@TableName", _
                                SqlDbType.NVarChar)).Value = sTmpName
                            'do it
                            s.ExecuteNonQuery()
                        End If
                    End If
                Next
                If bAddThisWeek Then
                    Dim DR As DataRow = DTList.NewRow
                    DR.BeginEdit()
                    DR.Item(gsWEEKLY_COL_ID) = nID
                    DR.Item(gsWEEKLY_LIST_COL_DATE) = ChartConfig.EndTime
                    DR.Item(gsWEEKLY_LIST_COL_NAME) = sTableName
                    DR.Item(gsWEEKLY_LIST_COL_MTBF) = ChartConfig.MTBF
                    DR.Item(gsWEEKLY_LIST_COL_MTTR) = ChartConfig.MTTR
                    DR.Item(gsWEEKLY_LIST_COL_UPTIME) = ChartConfig.UptimePct
                    DR.EndEdit()
                    DTList.Rows.Add(DR)
                End If
                'Delete old records from the tablelist
                'DB.UpdateDataSet does not delete records,
                ' but it will overwrite existing records, so this will shorten the list
                ' and allow DB.UpdateDataSet to make WeeklyReports.TableList match DTList
                s = DB.GetStoredProcedureCommand("DeleteOldRecords")
                s.Parameters.Add(New SqlClient.SqlParameter("@DeleteFromDate", _
                    SqlDbType.NVarChar)).Value = dDeletionDate
                'do it
                s.ExecuteNonQuery()
            Else
                Return Nothing
            End If
            DB.UpdateDataSet(DS, gsWEEKLY_LIST_TBLNAME)
            DB.Close()
            Return DTList
        Catch ex As Exception
            Dim lReply As Windows.Forms.DialogResult = Response.OK
            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msMODULE_NAME, _
            frmMain.Status, MessageBoxButtons.OK)
            Return Nothing
        End Try
    End Function

    Friend Sub subDoAutoCharts(Optional ByRef dDate As Date = Nothing, _
                               Optional ByRef dBeginDate As Date = Nothing, _
                               Optional ByRef dPreviousDate As Date = Nothing, _
                               Optional ByRef dPreviousBeginDate As Date = Nothing, _
                               Optional ByVal bDoYearly As Boolean = True)
        '********************************************************************************************
        'Description: Run auto charts
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/23/12  MSW     Try to do this by drawing pictures instead of running excel
        '********************************************************************************************
        Dim dEndTime As Date
        Dim dStartTime As Date
        Dim ChartConfigTW As tChartConfig
        Dim ChartConfigLW As tChartConfig
        ChartConfigTW.ByOccurrence = False
        ChartConfigTW.ChartSheet = String.Empty
        ChartConfigTW.EndTime = Now
        ChartConfigTW.MTBF = 0
        ChartConfigTW.MTTR = 0
        ChartConfigTW.NumAlarms = 0
        ChartConfigTW.StartTime = Now
        ChartConfigTW.TableSheet = String.Empty
        ChartConfigTW.Title = String.Empty
        ChartConfigTW.TopxxReq = 5
        ChartConfigTW.UptimePct = 0

        ChartConfigLW.ByOccurrence = False
        ChartConfigLW.ChartSheet = String.Empty
        ChartConfigLW.EndTime = Now
        ChartConfigLW.MTBF = 0
        ChartConfigLW.MTTR = 0
        ChartConfigLW.NumAlarms = 0
        ChartConfigLW.StartTime = Now
        ChartConfigLW.TableSheet = String.Empty
        ChartConfigLW.Title = String.Empty
        ChartConfigLW.TopxxReq = 5
        ChartConfigLW.UptimePct = 0

        Dim DTList As DataTable = Nothing
        Try

            'Optional date settings
            If (dDate = Nothing) Then
                dEndTime = Now
                'We're expecting this around midnight saturday.  If it's close, round it off
                If dEndTime.DayOfWeek = DayOfWeek.Sunday Then
                    dEndTime = DateAdd(DateInterval.Second, -1, dEndTime.Date)
                ElseIf dEndTime.DayOfWeek = DayOfWeek.Saturday Then
                    dEndTime = DateAdd(DateInterval.Day, 1, dEndTime.Date)
                    dEndTime = DateAdd(DateInterval.Second, -1, dEndTime.Date)
                    'else 
                    'Some other time, just take what's given
                End If
            Else
                dEndTime = dDate
                'We're taking the passed in date until 11:59 PM
                'Get the next day, ignoring time of day
                dEndTime = DateAdd(DateInterval.Day, 1, dEndTime.Date)
                'Subtract 1 second from to tommorow to get the end.
                dEndTime = DateAdd(DateInterval.Second, -1, dEndTime.Date)
            End If
            'Subtract a week from the end, then add 1 second to get back to midnight
            dStartTime = DateAdd(DateInterval.WeekOfYear, -1, dEndTime)
            dStartTime = DateAdd(DateInterval.Second, 1, dStartTime)
            frmMain.subGetDTMask()
            frmCriteria.subSetupChartCriteria()
        Catch ex As Exception
            Dim lReply As Windows.Forms.DialogResult = Response.OK
            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msMODULE_NAME, _
            frmMain.Status, MessageBoxButtons.OK)
        End Try
        '********************************************************************************************
        'Probably make the format selectable at some point, should work for XLSX or ODS
        '********************************************************************************************
        Dim sSPR_EXT As String = ""
        Dim oExportFormat As clsPrintHtml.eExportFormat = clsPrintHtml.eExportFormat.nXLSX

        Select Case oExportFormat
            Case clsPrintHtml.eExportFormat.nXLSX
                sSPR_EXT = ".XLSX"
            Case clsPrintHtml.eExportFormat.nODS
                sSPR_EXT = ".ODS"
            Case clsPrintHtml.eExportFormat.nXLS
                sSPR_EXT = ".XLS"
            Case clsPrintHtml.eExportFormat.nXML
                sSPR_EXT = ".XML"
            Case clsPrintHtml.eExportFormat.nCSV
                sSPR_EXT = ".CSV"
            Case clsPrintHtml.eExportFormat.nTXT
                sSPR_EXT = ".TXT"
        End Select
        '********************************************************************************************
        Try
            '********************************************************************************************
            'Get the directory first
            '********************************************************************************************
            Dim sTarget As String = String.Empty
            If Not (mPWCommon.GetDefaultFilePath(sTarget, eDir.WeeklyReports, String.Empty, String.Empty)) Then
                'Can't get directory
                MessageBox.Show(gpsRM.GetString("psWEEKLY_REP_DIR_ERR"), frmMain.msSCREEN_NAME, MessageBoxButtons.OK, _
                 MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, _
                MessageBoxOptions.DefaultDesktopOnly)
                Exit Sub
            End If
            'Build a file and tablename from the week
            'printing out the month name so we don't get confused with the places that do it backwards
            '
            Dim sTableName As String = frmMain.colZones.CurrentZone & " " & _
                            dEndTime.Date.ToString("MMMM_dd_yyyy")
            Dim sFileName As String = frmMain.colZones.CurrentZone & " - " & _
                            gpsRM.GetString("psWEEKLY_REPORT_FILENAME") & " - " & _
                            dEndTime.Date.ToString("MMMM-dd-yyyy")
            sTarget = sTarget & sFileName & sSPR_EXT
            'Delete the file if it's already there
            Try
                If My.Computer.FileSystem.FileExists(sTarget) Then
                    My.Computer.FileSystem.DeleteFile(sTarget, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin, FileIO.UICancelOption.DoNothing)
                End If
            Catch ex As Exception

                ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msMODULE_NAME, _
                                    frmMain.Status, MessageBoxButtons.OK)
                Exit Sub
            End Try


            '********************************************************************************************
            'Get data tables for this week and last week's summaries off all DT and top 5 by occurance and duration
            '********************************************************************************************
            Dim dtThisWkAlarms As DataTable = Nothing
            Dim dtLastWkAlarms As DataTable = Nothing
            Dim dtThisWkSumm As DataTable = Nothing
            Dim dtLastWkSumm As DataTable = Nothing
            Dim dtThisWkOccTop5 As DataTable = Nothing
            Dim dtLastWkOccTop5 As DataTable = Nothing
            Dim dtThisWkDurTop5 As DataTable = Nothing
            Dim dtLastWkDurTop5 As DataTable = Nothing
            Dim sChartTitles(6) As String
            '********************************************************************************************
            'This Week
            '********************************************************************************************
            ChartConfigTW.StartTime = dStartTime
            ChartConfigTW.EndTime = dEndTime

            'frmCriteria manages the db access, so we need to set its date even though it's invisible
            frmCriteria.StartTime = ChartConfigTW.StartTime
            frmCriteria.EndTime = ChartConfigTW.EndTime
            frmCriteria.bGetDataSet()
            dtThisWkAlarms = frmMain.dtGetDowntimeData(gsALARM_DS_TABLENAME, False)

            '********************************************************************************************
            'save current week to WeeklyReports DB
            '********************************************************************************************
            ChartConfigTW.TopxxReq = 0
            subGetChartData(dtThisWkAlarms, dtThisWkSumm, sChartTitles(0), ChartConfigTW)
            'Also deletes old data and returns a list of which summaries are available
            DTList = dtSaveWeeklyDataToDB(dtThisWkSumm, sTableName, ChartConfigTW)
            ChartConfigTW.TopxxReq = 5
            '********************************************************************************************
            'Get Duration top 5
            '********************************************************************************************
            ChartConfigTW.ByOccurrence = False
            'ChartConfigTW.TableSheet = sWeekLabelPrefix & gpsRM.GetString("psDURATION_SUMMARY_SHEET_NAME")
            subGetChartData(dtThisWkAlarms, dtThisWkDurTop5, sChartTitles(1), ChartConfigTW)
            '********************************************************************************************
            'Get occurance top 5
            '********************************************************************************************
            ChartConfigTW.ByOccurrence = True
            'ChartConfigTW.TableSheet = sWeekLabelPrefix & gpsRM.GetString("psOCCURRENCE_SUMMARY_SHEET_NAME")
            subGetChartData(dtThisWkAlarms, dtThisWkOccTop5, sChartTitles(2), ChartConfigTW)



            '********************************************************************************************
            'Last Week
            '********************************************************************************************
            dStartTime = DateAdd(DateInterval.WeekOfYear, -1, dStartTime)
            dEndTime = DateAdd(DateInterval.WeekOfYear, -1, dEndTime)
            ChartConfigLW.StartTime = dStartTime
            ChartConfigLW.EndTime = dEndTime

            'frmCriteria manages the db access, so we need to set its date even though it's invisible
            frmCriteria.StartTime = ChartConfigLW.StartTime
            frmCriteria.EndTime = ChartConfigLW.EndTime
            frmCriteria.bGetDataSet()
            dtLastWkAlarms = frmMain.dtGetDowntimeData(gsALARM_DS_TABLENAME, False)


            '********************************************************************************************
            'Get last week summaries
            '********************************************************************************************
            ChartConfigLW.TopxxReq = 0
            subGetChartData(dtLastWkAlarms, dtLastWkSumm, sChartTitles(3), ChartConfigLW)
            ChartConfigLW.TopxxReq = 5
            '********************************************************************************************
            'Get Duration top 5
            '********************************************************************************************
            ChartConfigLW.ByOccurrence = False
            'ChartConfigLW.TableSheet = sWeekLabelPrefix & gpsRM.GetString("psDURATION_SUMMARY_SHEET_NAME")
            subGetChartData(dtLastWkAlarms, dtLastWkDurTop5, sChartTitles(4), ChartConfigLW)
            '********************************************************************************************
            'Get occurance top 5
            '********************************************************************************************
            ChartConfigLW.ByOccurrence = True
            'ChartConfigLW.TableSheet = sWeekLabelPrefix & gpsRM.GetString("psOCCURRENCE_SUMMARY_SHEET_NAME")
            subGetChartData(dtLastWkAlarms, dtLastWkOccTop5, sChartTitles(5), ChartConfigLW)

            Application.DoEvents()



            '********************************************************************************************
            'Done with current alarm log data, now we go to the the yearly data
            'DTlist has the tablelist database from weekly reports already.
            'first thing to do is sort it.
            '********************************************************************************************
            'select routine returns an array of datarows sorted by date, index 0 is the oldest
            Dim drTableList() As DataRow = DTList.Select(String.Empty, gsWEEKLY_LIST_COL_DATE, DataViewRowState.CurrentRows)
            '********************************************************************************************
            'next, loop through the list from newest to oldest
            '********************************************************************************************
            ' year for the current records
            Dim nYear As Integer = 0
            'Declare some variables for use during the loops
            Dim dTableDate As Date = Nothing
            Dim sName As String = String.Empty
            Dim nMTBF As Integer = 0
            Dim nMTTR As Integer = 0
            Dim fUptime As Double = 0.0
            Dim DR As DataRow = Nothing

            'open another excel document
            Dim sYearlyTarget As String = String.Empty
            If Not (mPWCommon.GetDefaultFilePath(sYearlyTarget, eDir.WeeklyReports, String.Empty, String.Empty)) Then
                'Can't get directory
                MessageBox.Show(gpsRM.GetString("psWEEKLY_REP_DIR_ERR"), frmMain.msSCREEN_NAME, MessageBoxButtons.OK, _
                 MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, _
                MessageBoxOptions.DefaultDesktopOnly)
                Exit Sub
            End If
            'Build a file and tablename
            'printing out the month name so we don't get confused with the places that do it backwards
            '
            Dim sYearlyFileName As String = frmMain.colZones.CurrentZone & " - " & _
                            gpsRM.GetString("psYEARLY_DATA_FILENAME") & " - " & _
                            dEndTime.Date.ToString("MMMM-dd-yyyy")
            sYearlyTarget = sYearlyTarget & sYearlyFileName & sSPR_EXT
            'Delete the file if it's already there
            Try
                If My.Computer.FileSystem.FileExists(sYearlyTarget) Then
                    My.Computer.FileSystem.DeleteFile(sYearlyTarget, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin, FileIO.UICancelOption.DoNothing)
                End If
            Catch ex As Exception

                ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msMODULE_NAME, _
                                    frmMain.Status, MessageBoxButtons.OK)
                Exit Sub
            End Try

            'Downtime for 1 week
            Dim DT_Week As DataTable
            'Downtime summary totals
            Dim DT_Occurrence_Totals As DataTable = Nothing
            Dim DT_Duration_Totals As DataTable = Nothing
            Dim DT_Occurrence_Year As DataTable = Nothing
            Dim DT_Duration_Year As DataTable = Nothing
            'Each week's top 5 gets put in temp arrays
            Dim DR_Occurrence() As DataRow = Nothing
            Dim DR_Duration() As DataRow = Nothing

            'Table for spreadsheet Printout
            Dim DT_Weekly_Tables As DataTable = New DataTable()
            DT_Weekly_Tables.Columns.Add()
            DT_Weekly_Tables.Columns.Add()
            DT_Weekly_Tables.Columns.Add()
            DT_Weekly_Tables.Columns.Add()
            DT_Weekly_Tables.Columns.Add()

            'The list already cleared out old data, so the table summaries will get 
            'everything, but the chart can have a different date range 
            Dim oChartConfig As tChartConfig
            Dim dChartCutoffDate As Date = #1/1/2001#
            Dim dTmpDate As Date = dEndTime.Date
            'for month or year intervals, round the time down
            Select Case meWeeklyChartLengthUnits
                Case DateInterval.Year
                    dTmpDate = DateAdd(DateInterval.Day, ((dDate.DayOfYear * -1) + 1), dTmpDate)
                Case DateInterval.Month
                    dTmpDate = DateAdd(DateInterval.Day, ((dDate.Day * -1) + 1), dTmpDate)
                Case Else
                    'No need to round for day or week
            End Select
            dChartCutoffDate = DateAdd(meWeeklyChartLengthUnits, (-1 * mnWeeklyChartLength), dTmpDate)
            oChartConfig.EndTime = dEndTime.Date
            oChartConfig.StartTime = dTmpDate.Date
            oChartConfig.TopxxReq = 5

            '********************************************************************************************
            For nTable As Integer = drTableList.GetUpperBound(0) To 0 Step -1
                '********************************************************************************************
                'read the data for 1 row
                '********************************************************************************************
                DR = drTableList(nTable)
                dTableDate = CDate(DR.Item(gsWEEKLY_LIST_COL_DATE))
                sName = DR.Item(gsWEEKLY_LIST_COL_NAME).ToString
                nMTBF = CInt(DR.Item(gsWEEKLY_LIST_COL_MTBF))
                nMTTR = CInt(DR.Item(gsWEEKLY_LIST_COL_MTTR))
                fUptime = CSng(DR.Item(gsWEEKLY_LIST_COL_UPTIME))
                '********************************************************************************************
                'Get the summary table for 1 week
                '********************************************************************************************
                DT_Week = dtGetWeeklyDataFromDB(sName)

                'Get the top 5s for duration and occurrence
                subGetTopxTotals(DT_Week, DR_Occurrence, DR_Duration, oChartConfig.TopxxReq)

                '********************************************************************************************
                'Add up summary totals for the year
                '********************************************************************************************
                If DT_Occurrence_Year Is Nothing Then
                    '1st week, just clone the table config
                    DT_Occurrence_Year = DT_Week.Clone
                    DT_Occurrence_Year.Columns.Remove(gpsRM.GetString("psOCCURRENCES_PERC_CAP"))
                    DT_Occurrence_Year.Columns.Remove(gpsRM.GetString("psDURATION_PERC_CAP"))
                    DT_Occurrence_Year.Columns.Remove(gsDTMASK_COL_ID)
                End If
                If DT_Duration_Year Is Nothing Then
                    '1st week, just clone the table config
                    DT_Duration_Year = DT_Week.Clone
                    DT_Duration_Year.Columns.Remove(gpsRM.GetString("psOCCURRENCES_PERC_CAP"))
                    DT_Duration_Year.Columns.Remove(gpsRM.GetString("psDURATION_PERC_CAP"))
                    DT_Duration_Year.Columns.Remove(gsDTMASK_COL_ID)
                End If
                'and add to the total datatable
                subAddArrayToTotalsDT(DR_Occurrence, DT_Occurrence_Year)
                'and add to the total datatable
                subAddArrayToTotalsDT(DR_Duration, DT_Duration_Year)

                '********************************************************************************************
                'Add up summary totals for charts if it's in range
                '********************************************************************************************
                If (dTableDate > dChartCutoffDate) And (dTableDate <= dEndTime) Then
                    If DT_Occurrence_Totals Is Nothing Then
                        '1st week, just clone the table config
                        DT_Occurrence_Totals = DT_Week.Clone
                        DT_Occurrence_Totals.Columns.Remove(gpsRM.GetString("psOCCURRENCES_PERC_CAP"))
                        DT_Occurrence_Totals.Columns.Remove(gpsRM.GetString("psDURATION_PERC_CAP"))
                        DT_Occurrence_Totals.Columns.Remove(gsDTMASK_COL_ID)
                    End If
                    If DT_Duration_Totals Is Nothing Then
                        '1st week, just clone the table config
                        DT_Duration_Totals = DT_Week.Clone
                        DT_Duration_Totals.Columns.Remove(gpsRM.GetString("psOCCURRENCES_PERC_CAP"))
                        DT_Duration_Totals.Columns.Remove(gpsRM.GetString("psDURATION_PERC_CAP"))
                        DT_Duration_Totals.Columns.Remove(gsDTMASK_COL_ID)
                    End If
                    'and add to the total datatable
                    subAddArrayToTotalsDT(DR_Occurrence, DT_Occurrence_Totals)
                    'and add to the total datatable
                    subAddArrayToTotalsDT(DR_Duration, DT_Duration_Totals)

                End If

                '********************************************************************************************
                'Build spreadsheet of weekly top 5s
                '********************************************************************************************
                'frmMain.colZones.CurrentZone & " " & _
                '        gpsRM.GetString("psWEEKLY_TOP") & ChartConfig.TopxxReq.ToString & " " & _
                '        gpsRM.GetString("psSUMM_FOR") & nYear.ToString
                Dim sRowArray(4) As String
                sRowArray(0) = gpsRM.GetString("psWEEK_OF")
                sRowArray(1) = dTableDate.ToShortDateString
                sRowArray(2) = ""
                sRowArray(3) = gpsRM.GetString("psUPTIME") & ":"
                sRowArray(4) = (fUptime / 100).ToString("00.0%")
                DT_Weekly_Tables.Rows.Add(sRowArray)

                sRowArray(0) = gpsRM.GetString("psMTBF") & ":"
                sRowArray(1) = frmMain.sGetDurationString(nMTBF)
                sRowArray(3) = gpsRM.GetString("psMTTR") & ":"
                sRowArray(4) = frmMain.sGetDurationString(nMTTR)
                DT_Weekly_Tables.Rows.Add(sRowArray)

                sRowArray(0) = gpsRM.GetString("psOCCURRENCES_CAP")  'Occurrences	
                sRowArray(1) = dTableDate.ToShortDateString
                sRowArray(3) = gpsRM.GetString("psDURATION_CAP")  'Duration
                sRowArray(4) = dTableDate.ToShortDateString
                DT_Weekly_Tables.Rows.Add(sRowArray)

                sRowArray(0) = gpsRM.GetString("psDESC_CAP")  'Description
                sRowArray(1) = gpsRM.GetString("psTOTAL_CAP")  'Total
                sRowArray(3) = gpsRM.GetString("psDESC_CAP")  'Description
                sRowArray(4) = gpsRM.GetString("psTOTAL_CAP")  'Total
                DT_Weekly_Tables.Rows.Add(sRowArray)
                Dim nTotalOccur As Integer = 0
                Dim nTotalDuration As Integer = 0

                Dim nRow As Integer = 0
                While ((nRow <= DR_Duration.GetUpperBound(0)) Or (nRow <= DR_Occurrence.GetUpperBound(0)))
                    If nRow <= DR_Occurrence.GetUpperBound(0) Then
                        Dim nOccur As Integer = CInt(DR_Occurrence(nRow).Item(gpsRM.GetString("psOCCURRENCES_CAP")))
                        nTotalOccur += nOccur
                        sRowArray(0) = DR_Occurrence(nRow).Item(gpsRM.GetString("psDESC_CAP")).ToString
                        sRowArray(1) = nOccur.ToString

                    Else
                        sRowArray(0) = ""  'Description
                        sRowArray(1) = ""  'Count
                    End If
                    If nRow <= DR_Duration.GetUpperBound(0) Then
                        Dim nDuration As Integer = CInt(DR_Duration(nRow).Item(gpsRM.GetString("psDURATION_CAP")))
                        nTotalDuration += nDuration
                        sRowArray(3) = DR_Duration(nRow).Item(gpsRM.GetString("psDESC_CAP")).ToString
                        sRowArray(4) = frmMain.sGetDurationString(nDuration)
                    Else
                        sRowArray(3) = ""  'Description
                        sRowArray(4) = ""  'Time
                    End If
                    DT_Weekly_Tables.Rows.Add(sRowArray)
                    nRow += 1
                End While
                sRowArray(0) = gpsRM.GetString("psTOTAL_CAP")
                sRowArray(1) = nTotalOccur.ToString
                sRowArray(3) = gpsRM.GetString("psTOTAL_CAP")
                sRowArray(4) = frmMain.sGetDurationString(nTotalDuration)
                DT_Weekly_Tables.Rows.Add(sRowArray)

                sRowArray(0) = ""
                sRowArray(1) = ""
                sRowArray(2) = ""
                sRowArray(3) = ""
                sRowArray(4) = ""
                DT_Weekly_Tables.Rows.Add(sRowArray)
            Next



            '********************************************************************************************
            'Make Yearly file
            '********************************************************************************************
            frmMain.mPrintHtml.subSetPageFormat()
            frmMain.mPrintHtml.subClearTableFormat()
            Dim nColWidths(3) As Single
            nColWidths(0) = 0.2
            nColWidths(1) = 1.1
            nColWidths(2) = 1.6
            nColWidths(3) = 3.7

            frmMain.mPrintHtml.ColumnWidthsList = nColWidths

            Dim bCancelled As Boolean = False
            frmMain.mPrintHtml.subStartDoc(frmMain.Status, gpsRM.GetString("psWEEKLY_REPORT_FILENAME"), _
                    True, bCancelled, sYearlyTarget, oExportFormat)
            Dim sTitle(0) As String
            sTitle(0) = frmMain.colZones.SiteName & ": " & frmMain.colZones.CurrentZone & " - " & gpsRM.GetString("psYEARLY_DATA_FILENAME")
            Dim sSubTitle(0) As String
            sSubTitle(0) = String.Empty

            '********************************************************************************************
            'Draw Charts
            '********************************************************************************************
            frmMain.subSizeGrid(False, 5)
            frmMain.imgPie.Visible = True
            frmMain.imgKey.Visible = True
            frmMain.imgPie2.Visible = True
            frmMain.imgKey2.Visible = True
            frmMain.imgPie3.Visible = False
            frmMain.imgKey3.Visible = False
            frmMain.imgPie4.Visible = False
            frmMain.imgKey4.Visible = False
            frmMain.tlpChart.Visible = True
            frmMain.dgvMain.Visible = False

            Application.DoEvents()
            Dim sChartTitle As String = gpsRM.GetString("psTOT_EACH_WEEK") & gpsRM.GetString("psTOP_X") & "5 "
            frmMain.subDoChart(frmMain.imgPie, frmMain.imgKey, DT_Occurrence_Totals, eChartTypes.Bar, gpsRM.GetString("psOCCURRENCES_CAP"), sChartTitle & gpsRM.GetString("psBY_OCCURANCE"), 99)
            frmMain.subDoChart(frmMain.imgPie2, frmMain.imgKey2, DT_Duration_Totals, eChartTypes.Bar, gpsRM.GetString("psDURATION_CAP"), sChartTitle & gpsRM.GetString("psBY_DURATION"), 99)
            Application.DoEvents()
            Dim oSC As New ScreenShot.ScreenCapture
            Select Case oExportFormat
                Case clsPrintHtml.eExportFormat.nXLS
                    'Supports pictures
                    Dim sTmpFile As String = mPWCommon.sGetTmpFileName("ChartPic2", "jpg")
                    oSC.CaptureWindowToFile(frmMain.tlpChart.Handle, sTmpFile, Imaging.ImageFormat.Jpeg)
                    frmMain.mPrintHtml.subAddPicture(sTmpFile, frmMain.Status, sTitle, sSubTitle, gpsRM.GetString("psCHART"))
                Case clsPrintHtml.eExportFormat.nODS
                    'Supports pictures
                    Dim sTmpFile As String = mPWCommon.sGetTmpFileName("ChartPic2", "jpg")
                    oSC.CaptureWindowToFile(frmMain.tlpChart.Handle, sTmpFile, Imaging.ImageFormat.Jpeg)
                    frmMain.mPrintHtml.subAddPicture(sTmpFile, frmMain.Status, sTitle, sSubTitle, gpsRM.GetString("psCHART"), "A", "7", "M", "38")
                Case clsPrintHtml.eExportFormat.nXLSX
                    'Supports pictures
                    Dim sTmpFile As String = mPWCommon.sGetTmpFileName("ChartPic2", "png")
                    oSC.CaptureWindowToFile(frmMain.tlpChart.Handle, sTmpFile, Imaging.ImageFormat.Png)
                    frmMain.mPrintHtml.subAddPicture(sTmpFile, frmMain.Status, sTitle, sSubTitle, gpsRM.GetString("psCHART"), "A", "7", "S", "39")
                    'no pictures
            End Select

            '********************************************************************************************
            'Weekly top5 tables
            '********************************************************************************************
            frmMain.mPrintHtml.PrintColumnNames = False
            Dim nColWidthIndex(4) As Integer
            nColWidthIndex(0) = 3
            nColWidthIndex(1) = 1
            nColWidthIndex(2) = 0
            nColWidthIndex(3) = 3
            nColWidthIndex(4) = 1
            frmMain.mPrintHtml.ColumnWidthIndex = nColWidthIndex
            frmMain.mPrintHtml.subAddObject(DT_Weekly_Tables, frmMain.Status, sTitle, sSubTitle, gpsRM.GetString("psWEEKLY_REPORT_FILENAME"))
            frmMain.mPrintHtml.PrintColumnNames = True
            '********************************************************************************************
            'Total Occurance Summary
            '********************************************************************************************
            Dim DT_Occurrence_Output As DataTable = frmMain.dtSummarizeDataTablePrint(DT_Occurrence_Totals)
            sTitle(0) = frmMain.colZones.SiteName & ": " & frmMain.colZones.CurrentZone & " - " & gpsRM.GetString("psTOT_EACH_WEEK") & _
                    gpsRM.GetString("psTOP_X") & oChartConfig.TopxxReq.ToString & " " & _
                    gpsRM.GetString("psPIE_TITLE_DT") & " " & gpsRM.GetString("psOCCURRENCES_CAP")
            ReDim sSubTitle(3)
            sSubTitle(0) = gpsRM.GetString("psFROM")
            sSubTitle(1) = oChartConfig.StartTime.ToShortDateString
            sSubTitle(2) = gpsRM.GetString("psTO")
            sSubTitle(3) = oChartConfig.EndTime.ToShortDateString
            ReDim nColWidthIndex(3)
            nColWidthIndex(0) = 1
            nColWidthIndex(1) = 3
            nColWidthIndex(2) = 1
            nColWidthIndex(3) = 1
            frmMain.mPrintHtml.ColumnWidthIndex = nColWidthIndex
            frmMain.mPrintHtml.subAddObject(DT_Occurrence_Output, frmMain.Status, sTitle, sSubTitle, gpsRM.GetString("psOCCURRENCES_CAP"))
            '********************************************************************************************
            'Total Duration Summary
            '********************************************************************************************
            Dim DT_Duration_Output As DataTable = frmMain.dtSummarizeDataTablePrint(DT_Duration_Totals)
            sTitle(0) = frmMain.colZones.SiteName & ": " & frmMain.colZones.CurrentZone & " - " & gpsRM.GetString("psTOT_EACH_WEEK") & _
                    gpsRM.GetString("psTOP_X") & oChartConfig.TopxxReq.ToString & " " & _
                    gpsRM.GetString("psPIE_TITLE_DT") & " " & gpsRM.GetString("psDURATION_CAP")
            nColWidthIndex(0) = 1
            nColWidthIndex(1) = 3
            nColWidthIndex(2) = 1
            nColWidthIndex(3) = 1
            frmMain.mPrintHtml.ColumnWidthIndex = nColWidthIndex
            frmMain.mPrintHtml.subAddObject(DT_Duration_Output, frmMain.Status, sTitle, sSubTitle, gpsRM.GetString("psDURATION_CAP"))
            frmMain.mPrintHtml.subCloseFile(frmMain.Status)





            '********************************************************************************************
            'Make This Week's file
            '********************************************************************************************
            frmMain.mPrintHtml.subSetPageFormat()
            frmMain.mPrintHtml.subClearTableFormat()
            bCancelled = False
            frmMain.mPrintHtml.subStartDoc(frmMain.Status, gpsRM.GetString("psWEEKLY_REPORT_FILENAME"), _
                    True, bCancelled, sTarget, oExportFormat)
            Dim sPageTitle As String = gpsRM.GetString("psGRAPH_SHEET_NAME")
            sTitle(0) = frmMain.colZones.SiteName & ": " & frmMain.colZones.CurrentZone
            'sTitle(1) = gpsRM.GetString("psWEEKLY_REPORT") & gpsRM.GetString("psWEEK_OF") & " " & ChartConfigTW.EndTime.ToShortDateString
            ReDim sSubTitle(12)
            'sWeekName = gpsRM.GetString("psTHIS_WEEK")
            'sWeekLabelPrefix = gpsRM.GetString("psTHISWEEK")
            'sWeekTitlePrefix = gpsRM.GetString("psTHIS_WEEK_TITLE_PRE")
            'sSheetName = sWeekLabelPrefix & gpsRM.GetString("psALARMS_SHEET_NAME")
            'Times
            sSubTitle(2) = gpsRM.GetString("psSTART") & ": "
            sSubTitle(3) = ChartConfigTW.StartTime.ToString
            sSubTitle(4) = gpsRM.GetString("psEND") & ": "
            sSubTitle(5) = ChartConfigTW.EndTime.ToString
            'Stats
            sSubTitle(6) = gpsRM.GetString("psUPTIME") & ": "
            sSubTitle(7) = ChartConfigTW.UptimePct.ToString("00.0") & "%"
            sSubTitle(8) = gpsRM.GetString("psMTBF") & ": "
            sSubTitle(9) = frmMain.sGetDurationString(ChartConfigTW.MTBF)
            sSubTitle(10) = gpsRM.GetString("psMTTR") & ": "
            sSubTitle(11) = frmMain.sGetDurationString(ChartConfigTW.MTTR)
            frmMain.mPrintHtml.SubTitleCols = 6


            '********************************************************************************************
            'Draw Charts
            '********************************************************************************************
            frmMain.subSizeGrid(False, 4)
            frmMain.imgPie.Visible = True
            frmMain.imgKey.Visible = True
            frmMain.imgPie2.Visible = True
            frmMain.imgKey2.Visible = True
            frmMain.imgPie3.Visible = True
            frmMain.imgKey3.Visible = True
            frmMain.imgPie4.Visible = True
            frmMain.imgKey4.Visible = True
            frmMain.tlpChart.Visible = True
            frmMain.dgvMain.Visible = False

            Application.DoEvents()
            sChartTitle = gpsRM.GetString("psTHIS_WEEK_TITLE_PRE") & gpsRM.GetString("psTOP_X") & "5 "
            frmMain.subDoChart(frmMain.imgPie, frmMain.imgKey, dtThisWkOccTop5, eChartTypes.Bar, gpsRM.GetString("psOCCURRENCES_CAP"), sChartTitle & gpsRM.GetString("psBY_OCCURANCE"))
            frmMain.subDoChart(frmMain.imgPie2, frmMain.imgKey2, dtThisWkDurTop5, eChartTypes.Bar, gpsRM.GetString("psDURATION_CAP"), sChartTitle & gpsRM.GetString("psBY_DURATION"))
            sChartTitle = gpsRM.GetString("psLAST_WEEK_TITLE_PRE") & gpsRM.GetString("psTOP_X") & "5 "
            frmMain.subDoChart(frmMain.imgPie3, frmMain.imgKey3, dtLastWkOccTop5, eChartTypes.Bar, gpsRM.GetString("psOCCURRENCES_CAP"), sChartTitle & gpsRM.GetString("psBY_OCCURANCE"))
            frmMain.subDoChart(frmMain.imgPie4, frmMain.imgKey4, dtLastWkDurTop5, eChartTypes.Bar, gpsRM.GetString("psDURATION_CAP"), sChartTitle & gpsRM.GetString("psBY_DURATION"))
            Application.DoEvents()
            Select Case oExportFormat
                Case clsPrintHtml.eExportFormat.nXLS
                    'Supports pictures
                    Dim sTmpFile As String = mPWCommon.sGetTmpFileName("ChartPic", "jpg")
                    oSC.CaptureWindowToFile(frmMain.tlpChart.Handle, sTmpFile, Imaging.ImageFormat.Jpeg)
                    frmMain.mPrintHtml.subAddPicture(sTmpFile, frmMain.Status, sTitle, sSubTitle, gpsRM.GetString("psCHART"))
                Case clsPrintHtml.eExportFormat.nODS
                    'Supports pictures
                    Dim sTmpFile As String = mPWCommon.sGetTmpFileName("ChartPic", "jpg")
                    oSC.CaptureWindowToFile(frmMain.tlpChart.Handle, sTmpFile, Imaging.ImageFormat.Jpeg)
                    frmMain.mPrintHtml.subAddPicture(sTmpFile, frmMain.Status, sTitle, sSubTitle, gpsRM.GetString("psCHART"), "A", "7", "M", "38")
                Case clsPrintHtml.eExportFormat.nXLSX
                    'Supports pictures
                    Dim sTmpFile As String = mPWCommon.sGetTmpFileName("ChartPic", "png")
                    oSC.CaptureWindowToFile(frmMain.tlpChart.Handle, sTmpFile, Imaging.ImageFormat.Png)
                    frmMain.mPrintHtml.subAddPicture(sTmpFile, frmMain.Status, sTitle, sSubTitle, gpsRM.GetString("psCHART"), "A", "7", "S", "39")
                    'no pictures
            End Select

            Dim dtThisWkDurTop5Out As DataTable = frmMain.dtSummarizeDataTablePrint(dtThisWkDurTop5)
            ReDim nColWidthIndex(5)
            nColWidthIndex(0) = 1
            nColWidthIndex(1) = 3
            nColWidthIndex(2) = 1
            nColWidthIndex(3) = 2
            nColWidthIndex(4) = 1
            nColWidthIndex(5) = 2
            frmMain.mPrintHtml.ColumnWidthIndex = nColWidthIndex
            Dim sSheetName As String = gpsRM.GetString("psTHISWEEK") & gpsRM.GetString("psDURATION_SUMMARY_SHEET_NAME")
            frmMain.mPrintHtml.subAddObject(dtThisWkDurTop5Out, frmMain.Status, sTitle, sSubTitle, sSheetName)
            Dim dtThisWkOccTop5Out As DataTable = frmMain.dtSummarizeDataTablePrint(dtThisWkOccTop5)
            sSheetName = gpsRM.GetString("psTHISWEEK") & gpsRM.GetString("psOCCURRENCE_SUMMARY_SHEET_NAME")
            frmMain.mPrintHtml.subAddObject(dtThisWkOccTop5Out, frmMain.Status, sTitle, sSubTitle, sSheetName)
            Dim dtThisWkSummOut As DataTable = frmMain.dtSummarizeDataTablePrint(dtThisWkSumm)
            sSheetName = gpsRM.GetString("psTHISWEEK") & gpsRM.GetString("psALARM_SUMMARY_SHEET_NAME")
            frmMain.mPrintHtml.subAddObject(dtThisWkSummOut, frmMain.Status, sTitle, sSubTitle, sSheetName)

            'sWeekName = gpsRM.GetString("psLAST_WEEK")
            'sWeekLabelPrefix = gpsRM.GetString("psLASTWEEK")
            'sWeekTitlePrefix = gpsRM.GetString("psLAST_WEEK_TITLE_PRE")
            'sSheetName = sWeekLabelPrefix & gpsRM.GetString("psALARMS_SHEET_NAME")
            'Times
            sSubTitle(2) = gpsRM.GetString("psSTART") & ": "
            sSubTitle(3) = ChartConfigLW.StartTime.ToString
            sSubTitle(4) = gpsRM.GetString("psEND") & ": "
            sSubTitle(5) = ChartConfigLW.EndTime.ToString
            'Stats
            sSubTitle(6) = gpsRM.GetString("psUPTIME") & ": "
            sSubTitle(7) = ChartConfigLW.UptimePct.ToString("00.0") & "%"
            sSubTitle(8) = gpsRM.GetString("psMTBF") & ": "
            sSubTitle(9) = frmMain.sGetDurationString(ChartConfigLW.MTBF)
            sSubTitle(10) = gpsRM.GetString("psMTTR") & ": "
            sSubTitle(11) = frmMain.sGetDurationString(ChartConfigLW.MTTR)
            frmMain.mPrintHtml.SubTitleCols = 6
            Dim dtLastWkDurTop5Out As DataTable = frmMain.dtSummarizeDataTablePrint(dtLastWkDurTop5)
            sSheetName = gpsRM.GetString("psLASTWEEK") & gpsRM.GetString("psDURATION_SUMMARY_SHEET_NAME")
            frmMain.mPrintHtml.subAddObject(dtLastWkDurTop5Out, frmMain.Status, sTitle, sSubTitle, sSheetName)
            Dim dtLastWkOccTop5Out As DataTable = frmMain.dtSummarizeDataTablePrint(dtLastWkOccTop5)
            sSheetName = gpsRM.GetString("psLASTWEEK") & gpsRM.GetString("psOCCURRENCE_SUMMARY_SHEET_NAME")
            frmMain.mPrintHtml.subAddObject(dtLastWkOccTop5Out, frmMain.Status, sTitle, sSubTitle, sSheetName)
            Dim dtLastWkSummOut As DataTable = frmMain.dtSummarizeDataTablePrint(dtLastWkSumm)
            sSheetName = gpsRM.GetString("psLASTWEEK") & gpsRM.GetString("psALARM_SUMMARY_SHEET_NAME")
            frmMain.mPrintHtml.subAddObject(dtLastWkSummOut, frmMain.Status, sTitle, sSubTitle, sSheetName)
            frmMain.mPrintHtml.subCloseFile(frmMain.Status)

            Application.DoEvents()
            frmMain.Progress = 100
        Catch ex As Exception
            Application.DoEvents()
            Dim lReply As Windows.Forms.DialogResult = Response.OK
            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msMODULE_NAME, _
            frmMain.Status, MessageBoxButtons.OK)
            Application.DoEvents()
        End Try
        frmMain.Cursor = System.Windows.Forms.Cursors.Default

    End Sub



End Module
