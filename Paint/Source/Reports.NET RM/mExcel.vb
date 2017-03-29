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
' Form/Module: mExcel
'
' Description:  Gathers all the routines that use excel
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
'    11/23/09   MSW     subDoManualChart - Handle small tables, allow "TableIn.Rows.Count < 1" instead of "TableIn.Rows.Count < 3"
'    05/16/14   MSW     Some weekly report cleanup                                      4.01.07.03
'                       Change SaveAs calls so they use .XLSX files that can be opened on other versions
'                       Catch the case where charts calls TopxxRecords and gets a different table
'******************************************************************************************************
'
'******************************************************************************************************
'Conditional compilation for excel versions.
'EXCEL12 = True for silly ribbon versions
'EXCEL12 = False for Excel 2003 (11)
'******************************************************************************************************
#Const EXCEL12 = True
'******************************************************************************************************

Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Configuration.ConfigurationSettings
Imports Response = System.Windows.Forms.DialogResult
Imports Microsoft.Office.Interop
Imports MSOCore = Microsoft.Office.Core
Imports System.Text
Friend Module mExcel
    Private Const msMODULE_NAME As String = "mExcel.vb"
    Friend Structure tChartConfig
        Dim TableRow As Integer
        Dim TableCol As Integer
        Dim ChartTop As Integer
        Dim ChartLeft As Integer
        Dim ChartHeight As Integer
        Dim ChartWidth As Integer
        Dim TableSheet As String
        Dim ChartSheet As String
        Dim AlarmsRow1 As Integer
        Dim ByOccurrence As Boolean
        Dim NumAlarms As Integer
        Dim TopxxReq As Integer
        Dim OccurCol As Integer
        Dim DurCol As Integer
        Dim AlarmLabelCol As Integer
        Dim Title As String
        Dim UptimePct As Double
        Dim MTTR As Integer
        Dim MTBF As Integer
        Dim StartTime As Date
        Dim EndTime As Date
        Dim ChartType As Excel.XlChartType
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
    Friend Function bSaveCsvToXls(ByRef sSource As String, ByRef sTarget As String, Optional ByVal sSheetName As String = "") As Boolean
        '********************************************************************************************
        'Description:  returns the excel column letter from a number up to two characters
        '
        'Parameters: sSource,sTarger
        'Returns:    True if successful
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'start excel to open the csv and convert it.
        Dim oXl As Excel.Application
        Dim oWb As Excel.Workbook
        Dim oSheet As Excel.Worksheet

        oXl = DirectCast(CreateObject("Excel.Application"), Excel.Application)

        oWb = oXl.Workbooks.Open(sSource)

        'In case excel doesn't want to overwrite same file
        Try
            oXl.Visible = mPWCommon.InIDE
            'Prevent excel from asking too many questions.  The save dialog should ask if they want to overwrite
            If My.Computer.FileSystem.FileExists(sTarget) Then
                My.Computer.FileSystem.DeleteFile(sTarget, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin, FileIO.UICancelOption.DoNothing)
            End If
#If EXCEL12 Then
            oWb.SaveAs(Filename:=sTarget, FileFormat:=Excel.XlFileFormat.xlOpenXMLWorkbook)
#Else
            oWb.SaveAs(Filename:=sTarget, FileFormat:=Excel.XlFileFormat.xlWorkbookNormal)
#End If
            'Do the formatting
            oSheet = DirectCast(oWb.ActiveSheet, Excel.Worksheet)
            If sSheetName <> String.Empty Then
                oSheet.Name = sSheetName
            End If
            oSheet.Cells.Select()
            oSheet.Cells.EntireColumn.AutoFit()
            oSheet.Range("A1").Select()
            'Save formatted xls
            oWb.Close(Excel.XlSaveAction.xlSaveChanges)


            'oXl.Application.Quit()
            oXl.Quit()
            oSheet = Nothing
            oWb = Nothing
            oXl = Nothing

            'Not saving csv so delete it
            Application.DoEvents()
            My.Computer.FileSystem.DeleteFile(sSource, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently, FileIO.UICancelOption.DoNothing)
            Return True
        Catch ex As Exception
            'Error saving in excel
            Dim lReply As Windows.Forms.DialogResult = Response.OK
            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msMODULE_NAME, _
            frmMain.Status, MessageBoxButtons.OK)

            oWb.Close(Excel.XlSaveAction.xlDoNotSaveChanges)

            oXl.Application.Quit()
            oXl.Quit()
            oSheet = Nothing
            oWb = Nothing
            oXl = Nothing

            Application.DoEvents()
            My.Computer.FileSystem.DeleteFile(sSource, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently, FileIO.UICancelOption.DoNothing)
            Return False
        End Try

    End Function

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
    Friend Sub subBuildChart(ByRef oXl As Excel.Application, _
                                                   ByRef oWB As Excel.Workbook, _
                                                   ByRef oTableSheet As Excel.Worksheet, _
                                                   ByRef oChartSheet As Excel.Worksheet, _
                                                  ByRef ChartConfig As tChartConfig)
        '********************************************************************************************
        'Description:  create a chart in an excel sheet
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            'Insert a new chart
            'oChartSheet.Select()
            'TODO -Excel 2007 Only oChartSheet.Shapes.AddChart.Select()

            Dim oChartObjects As Excel.ChartObjects = DirectCast(oChartSheet.ChartObjects, Excel.ChartObjects)
            oChartObjects.Add(ChartConfig.ChartLeft, ChartConfig.ChartTop, ChartConfig.ChartWidth, ChartConfig.ChartHeight)
            Dim oChartObject As Excel.ChartObject = DirectCast(oChartObjects.Item(oChartObjects.Count), Excel.ChartObject)
            'position it 
            oChartObject.Top = ChartConfig.ChartTop
            oChartObject.Left = ChartConfig.ChartLeft
            oChartObject.Height = ChartConfig.ChartHeight
            oChartObject.Width = ChartConfig.ChartWidth
            Dim oChart As Excel.Chart = DirectCast(oChartObject.Chart, Excel.Chart)
            oChart.ChartType = ChartConfig.ChartType
            'oChart.Name = ChartConfig.ChartSheet
            'Set the source data
            Dim sDataCol As String
            If ChartConfig.ByOccurrence Then
                sDataCol = sGetColumnLetterFromNumber(ChartConfig.OccurCol)
            Else
                sDataCol = sGetColumnLetterFromNumber(ChartConfig.DurCol)
            End If
            'Set the source data
            Dim sRow1 As String = ChartConfig.AlarmsRow1.ToString
            Dim sRowLast As String = (ChartConfig.AlarmsRow1 + ChartConfig.NumAlarms - 1).ToString
            'The chart starts at the bottom, so select in reverse order
            oChart.SetSourceData(Source:=oTableSheet.Range(sDataCol & sRowLast & ":" & sDataCol & sRow1))
            Dim oSeriesCol As Excel.SeriesCollection = DirectCast(oChart.SeriesCollection, Excel.SeriesCollection)
            'Alarm labels
            Dim sAlarmLabelCol As String
            sAlarmLabelCol = sGetColumnLetterFromNumber(ChartConfig.AlarmLabelCol)
            'Excel 2007 only
            'oSeriesCol.Item(1).XValues = "=" & ChartConfig.TableSheet & "!$" & sAlarmLabelCol _
            '        & "$" & sRowLast & ":$" & sAlarmLabelCol & "$" & sRow1
            'FOR Excel 2003 - must be in this format "=DurationSummary!R5C7:R16C7"
            oSeriesCol.Item(1).XValues = "=" & ChartConfig.TableSheet & "!R" & sRowLast & "C" & ChartConfig.AlarmLabelCol.ToString _
                    & ":R" & sRow1 & "C" & ChartConfig.AlarmLabelCol.ToString

            If ChartConfig.ByOccurrence Then
                oSeriesCol.Item(1).Name = gpsRM.GetString("psOCCURRENCES_CAP")
            Else
                oSeriesCol.Item(1).Name = gpsRM.GetString("psDURATION_CAP")
            End If
            'Remove the legend since it's only one series
            Dim oLegend As Excel.Legend = DirectCast(oChart.Legend, Excel.Legend)
            oLegend.Delete()
            'Set the title text
            oChart.ChartTitle.Font.Size = 14
            oChart.ChartTitle.Text = ChartConfig.Title
            'Apply value labels to the bars
            'Excel 2007 only - oSeriesCol.Item(1).Select()
            oSeriesCol.Item(1).ApplyDataLabels()

            'Make sure the alarm names are all listed
            Dim oAxes As Excel.Axes = DirectCast(oChart.Axes, Excel.Axes)
            Dim oAxis As Excel.Axis = DirectCast(oAxes.Item(Excel.XlAxisType.xlCategory), Excel.Axis)
            oAxis.TickLabelSpacing = 1
            'TODO - Excel 2007 only? oAxis.TickLabelSpacingIsAuto = False
            oAxis.CategoryType = Excel.XlCategoryType.xlCategoryScale
            oAxis.Crosses = Excel.XlAxisCrosses.xlAxisCrossesMaximum
            oAxis.ReversePlotOrder = True
            oAxis = DirectCast(oAxes.Item(Excel.XlAxisType.xlValue), Excel.Axis)
            oAxis.TickLabelPosition = Excel.XlTickLabelPosition.xlTickLabelPositionHigh
            If ChartConfig.ByOccurrence = False Then
                'Big labels for the time, hide the seconds so it's not so crowded
                oAxis.TickLabels.NumberFormat = "[h]:mm"
                'Adjust the label interval - this isn't some well thought out formula
                ' I just threw all the factors together and messed with it until it worked
                ' with the sample data I had
                'Also, it sometimes gets goofy errors, so the local error handler will just skip it
                Try
                    Dim fPitch As Double
                    If ChartConfig.ChartType = Excel.XlChartType.xlBarClustered Then
                        fPitch = ChartConfig.MTTR / ChartConfig.ChartWidth
                        If fPitch > 2 Then 'MTTR > 1 hour
                            oAxis.MajorUnit = CInt(fPitch / 1.5) / 8 ' stick close sort of round numbers
                        End If
                    ElseIf ChartConfig.ChartType = Excel.XlChartType.xlColumnClustered Then
                        fPitch = ChartConfig.MTTR / ChartConfig.ChartHeight
                        If fPitch > 2 Then 'MTTR > 1 hour
                            oAxis.MajorUnit = CInt(fPitch) / 2
                        End If
                    End If

                Catch ex As Exception
                End Try
                If ChartConfig.ChartType = Excel.XlChartType.xlColumnClustered Then
                    Dim oDatalabels As Excel.DataLabels = DirectCast(oSeriesCol.Item(1).DataLabels, Excel.DataLabels)
                    oDatalabels.Orientation = 45
                End If
            End If
            'changed my mind after I set this up, but if you want to move the chart into it's own tab
            'in excel, this is how it's done
            ''Move to it's own tab and resize 
            'oChart.Location(Where:=Excel.XlChartLocation.xlLocationAsNewSheet)
        Catch ex As Exception
            'Error selecting the file
            Dim lReply As Windows.Forms.DialogResult = Response.OK
            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msMODULE_NAME, _
            frmMain.Status, MessageBoxButtons.OK)
            Exit Sub

        End Try

    End Sub


    Friend Sub subBuildDTAlarmSheetFromDataSet(ByRef oXl As Excel.Application, _
                                                  ByRef oWB As Excel.Workbook, _
                                                  ByRef oSheet As Excel.Worksheet, _
                                                  ByRef DT As DataTable, _
                                                  ByRef ChartConfig As tChartConfig)
        '********************************************************************************************
        'Description:  create a table in an excel sheet from a dataset
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'title data first
        Try
            Dim nRow As Integer
            Dim oRange As Excel.Range
            Dim oCell1 As Object
            Dim oCell2 As Object
            'Title row
            oSheet.Cells(ChartConfig.TableRow, ChartConfig.TableCol) = frmMain.colZones.SiteName & ": " & frmMain.colZones.CurrentZone
            oSheet.Cells(ChartConfig.TableRow, ChartConfig.TableCol + 2) = gpsRM.GetString("psDOWNTIME_ALARMS")
            'Bold for the first row of titles
            oCell1 = oSheet.Cells(ChartConfig.TableRow, ChartConfig.TableCol)
            oCell2 = oSheet.Cells(ChartConfig.TableRow, ChartConfig.TableCol + 2)
            oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
            oRange.Font.Bold = True
            'Times
            oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol) = gpsRM.GetString("psSTART") & ": "
            oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol + 1) = ChartConfig.StartTime.ToString("d")
            oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol + 2) = gpsRM.GetString("psEND") & ": "
            oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol + 3) = ChartConfig.EndTime.ToString("d")
            'Left align the row
            oCell1 = oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol)
            oCell2 = oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol + 3)
            oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
            oRange.HorizontalAlignment = Excel.Constants.xlLeft
            'Then right align the "Start" and "End" labels so they meet up with the times
            oCell1 = oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol)
            oRange = DirectCast(oSheet.Range(oCell1, oCell1), Excel.Range)
            oRange.HorizontalAlignment = Excel.Constants.xlRight
            oCell1 = oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol + 2)
            oRange = DirectCast(oSheet.Range(oCell1, oCell1), Excel.Range)
            oRange.HorizontalAlignment = Excel.Constants.xlRight

            'Column labels
            nRow = ChartConfig.TableRow + 2
            'More column Config
            ChartConfig.AlarmsRow1 = nRow + 1
            ChartConfig.NumAlarms = DT.Rows.Count 'Includes two summary lines, they'll be subtracted from this var on the way out of this routine
            Dim nAlmCol As Integer = 0
            Dim nDescCol As Integer = 0
            For nCol As Integer = ChartConfig.TableCol To DT.Columns.Count + ChartConfig.TableCol - 1
                oSheet.Cells(nRow, nCol) = DT.Columns.Item(nCol - ChartConfig.TableCol).Caption
            Next
            'We need to add Start Date
            oSheet.Cells(nRow, DT.Columns.Count + ChartConfig.TableCol) = gpsRM.GetString("psSTART_DATE")
            'Label the columns we're adding in
            'Bold font for the column titles
            If DT.Columns.Count > 0 Then


                oCell1 = oSheet.Cells(nRow, ChartConfig.TableCol)
                oCell2 = oSheet.Cells(nRow, ChartConfig.TableCol + DT.Columns.Count)
                oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
                oRange.Font.Bold = True
                Dim StartDate As Date
                'For Each dr As DataRow In DT.Rows
                '    Dim sTmp As String = String.Empty
                '    For nCol As Integer = ChartConfig.TableCol To DT.Columns.Count + ChartConfig.TableCol - 1
                '        sTmp = sTmp & ", " & dr.Item(nCol - 1).ToString
                '    Next
                '    Debug.Print(sTmp)
                'Next

                'For nCol As Integer = ChartConfig.TableCol To DT.Columns.Count + ChartConfig.TableCol - 1
                '    Debug.Print("Col " & nCol.ToString & ": " & DT.Columns(nCol - 1).ColumnName)
                'Next
                For Each dr As DataRow In DT.Rows
                    nRow += 1
                    'MSW 5/14/14 Get this by name.  In rare cases it'll call getTopxx... whcih gives a different column order
                    StartDate = CType(dr.Item(gpsRM.GetString("psSTART_TIME_CAP")), Date)
                    For nCol As Integer = ChartConfig.TableCol To DT.Columns.Count + ChartConfig.TableCol - 1
                        'Debug.Print(dr.Item(nCol - 1).ToString)
                        'Some extra spaces are showing up in the alarm descriptions
                        'Clean them out, but let the numeric data get set as numbers - It might not matter, but I don't know.
                        If (nCol = nDescCol) Or (nCol = nAlmCol) Then
                            oSheet.Cells(nRow, nCol) = Trim(dr.Item(nCol - ChartConfig.TableCol).ToString)
                        Else
                            oSheet.Cells(nRow, nCol) = dr.Item(nCol - 1)
                            'If nCol = 3 Then StartDate = CType(dr.Item(nCol - 1), Date)
                        End If
                    Next
                    oSheet.Cells(nRow, DT.Columns.Count + ChartConfig.TableCol) = Format(StartDate, "Short Date")
                Next
                'Select everything except the big titles then do the autofit
                nRow = ChartConfig.TableRow + 1
                oCell1 = oSheet.Cells(nRow, ChartConfig.TableCol)
                nRow = ChartConfig.AlarmsRow1 + ChartConfig.NumAlarms - 1
                oCell2 = oSheet.Cells(nRow, ChartConfig.TableCol + DT.Columns.Count - 1)
                oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
                oRange.Columns.AutoFit()
            End If

        Catch ex As Exception
            'Error selecting the file
            Dim lReply As Windows.Forms.DialogResult = Response.OK
            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msMODULE_NAME, _
            frmMain.Status, MessageBoxButtons.OK)
            Exit Sub

        End Try
    End Sub
    Friend Sub subBuildDTSummarySheetFromDataSet(ByRef oXl As Excel.Application, _
                                                  ByRef oWB As Excel.Workbook, _
                                                  ByRef oSheet As Excel.Worksheet, _
                                                  ByRef DT As DataTable, _
                                                  ByRef sTitle As String, _
                                                  ByRef ChartConfig As tChartConfig, _
                                                  Optional ByVal bStats As Boolean = True, _
                                                  Optional ByVal bCombDesc As Boolean = True, _
                                                  Optional ByVal bDurTotal As Boolean = False)
        '********************************************************************************************
        'Description:  create a table in an excel sheet from a dataset
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'title data first
        Try
            Dim nRow As Integer
            Dim oRange As Excel.Range
            Dim oCell1 As Object
            Dim oCell2 As Object
            'Title row
            oSheet.Cells(ChartConfig.TableRow, ChartConfig.TableCol) = frmMain.colZones.SiteName & ": " & frmMain.colZones.CurrentZone
            oSheet.Cells(ChartConfig.TableRow, ChartConfig.TableCol + 2) = sTitle
            'Bold for the first row of titles
            oCell1 = oSheet.Cells(ChartConfig.TableRow, ChartConfig.TableCol)
            oCell2 = oSheet.Cells(ChartConfig.TableRow, ChartConfig.TableCol + 2)
            oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
            oRange.Font.Bold = True

            'Times
            oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol) = gpsRM.GetString("psSTART") & ": "
            oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol + 1) = ChartConfig.StartTime.ToString
            oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol + 2) = gpsRM.GetString("psEND") & ": "
            oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol + 3) = ChartConfig.EndTime.ToString

            'Stats
            If bStats Then
                nRow = ChartConfig.TableRow + 2
                oSheet.Cells(nRow, ChartConfig.TableCol) = gpsRM.GetString("psUPTIME") & ": "
                oSheet.Cells(nRow, ChartConfig.TableCol + 1) = ChartConfig.UptimePct.ToString("00.0") & "%"
                oSheet.Cells(nRow, ChartConfig.TableCol + 2) = gpsRM.GetString("psMTBF") & ": "
                oSheet.Cells(nRow, ChartConfig.TableCol + 3) = " " & frmMain.sGetDurationString(ChartConfig.MTBF)
                oSheet.Cells(nRow, ChartConfig.TableCol + 4) = gpsRM.GetString("psMTTR") & ": "
                oSheet.Cells(nRow, ChartConfig.TableCol + 5) = " " & frmMain.sGetDurationString(ChartConfig.MTTR)
            Else
                nRow = ChartConfig.TableRow + 1
            End If

            'Left align the rows
            oCell1 = oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol)
            oCell2 = oSheet.Cells(nRow, ChartConfig.TableCol + 5)
            oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
            oRange.HorizontalAlignment = Excel.Constants.xlLeft
            'Then right align the labels so they meet up with the data
            oCell1 = oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol)
            oCell2 = oSheet.Cells(nRow, ChartConfig.TableCol)
            oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
            oRange.HorizontalAlignment = Excel.Constants.xlRight
            oCell1 = oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol + 2)
            oCell2 = oSheet.Cells(nRow, ChartConfig.TableCol + 2)
            oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
            oRange.HorizontalAlignment = Excel.Constants.xlRight
            oCell1 = oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol + 4)
            oCell2 = oSheet.Cells(nRow, ChartConfig.TableCol + 4)
            oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
            oRange.HorizontalAlignment = Excel.Constants.xlRight

            'Column labels
            nRow += 1
            'More column Config
            ChartConfig.AlarmsRow1 = nRow + 1
            ChartConfig.NumAlarms = DT.Rows.Count 'Includes two summary lines, they'll be subtracted from this var on the way out of this routine
            ChartConfig.AlarmLabelCol = DT.Columns.Count + ChartConfig.TableCol
            Dim nAlmCol As Integer = 0
            Dim nDescCol As Integer = 0
            Dim nMaxWeekCol As Integer = 0
            Dim nMinWeekCol As Integer = 999999
            Dim nTotalCol As Integer = 0
            For nCol As Integer = ChartConfig.TableCol To DT.Columns.Count + ChartConfig.TableCol - 1
                oSheet.Cells(nRow, nCol) = DT.Columns.Item(nCol - ChartConfig.TableCol).Caption
                Dim sCol As String = DT.Columns.Item(nCol - ChartConfig.TableCol).Caption
                Select Case sCol
                    Case gpsRM.GetString("psPERCENT_CAP"), gpsRM.GetString("psOCCURRENCES_PERC_CAP"), gpsRM.GetString("psDURATION_PERC_CAP")
                        'Percentages stored as decimal value (10% = 0.0), set the format in excel
                        oCell1 = oSheet.Cells(ChartConfig.AlarmsRow1, nCol)
                        oCell2 = oSheet.Cells(ChartConfig.AlarmsRow1 + ChartConfig.NumAlarms - 1, nCol)
                        oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
                        oRange.NumberFormat = "0.00%"
                    Case gpsRM.GetString("psDURATION_CAP")
                        ChartConfig.DurCol = nCol
                    Case gpsRM.GetString("psOCCURRENCES_CAP")
                        ChartConfig.OccurCol = nCol
                    Case gpsRM.GetString("psALARM_CAP")
                        nAlmCol = nCol
                    Case gpsRM.GetString("psDESC_CAP")
                        nDescCol = nCol
                    Case gpsRM.GetString("psTOTAL_CAP")
                        nTotalCol = nCol
                    Case Else
                        Dim sTmp1 As String() = Split(DT.Columns.Item(nCol - ChartConfig.TableCol).ColumnName.Substring(1), "_")
                        Dim bWeekly As Boolean = True
                        For Each sTmp2 As String In sTmp1
                            If IsNumeric(sTmp2) = False Then
                                bWeekly = False
                            End If
                        Next
                        nMaxWeekCol = nCol
                        If bWeekly Then
                            If nMaxWeekCol < nCol Then
                                nMaxWeekCol = nCol
                            ElseIf nMinWeekCol > nCol Then
                                nMinWeekCol = nCol
                            End If
                        End If
                End Select
            Next
            'Label the columns we're adding in
            If bCombDesc Then
                'Alarm-Desc - combine these columns into a 3rd so the chart can access it.
                oSheet.Cells(nRow, ChartConfig.AlarmLabelCol) = gpsRM.GetString("psALARM_CAP") & " " & gpsRM.GetString("psDESC_CAP")
            End If
            'Bold font for the column titles
            oCell1 = oSheet.Cells(nRow, ChartConfig.TableCol)
            oCell2 = oSheet.Cells(nRow, ChartConfig.AlarmLabelCol)
            oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
            oRange.Font.Bold = True

            For Each dr As DataRow In DT.Rows
                nRow += 1
                For nCol As Integer = ChartConfig.TableCol To DT.Columns.Count + ChartConfig.TableCol - 1

                    Select Case nCol
                        Case ChartConfig.DurCol
                            'Can't get excel to format this right, so add another row as a text
                            'oSheet.Cells(nRow, nCol) = " " & frmMain.sGetDurationString(CInt(dr.Item(nCol - ChartConfig.TableCol))) '
                            'MSW 6/17/14 - That messed up the charts.  We need to let excel treat this as a date instead of a string
                            oSheet.Cells(nRow, nCol) = frmMain.sGetDurationString(CInt(dr.Item(nCol - ChartConfig.TableCol)))
                        Case nAlmCol
                            oSheet.Cells(nRow, nCol) = Trim(dr.Item(nCol - ChartConfig.TableCol).ToString)
                        Case nDescCol
                            oSheet.Cells(nRow, nCol) = Trim(dr.Item(nCol - ChartConfig.TableCol).ToString)
                        Case Else
                            If bDurTotal AndAlso ((nCol >= nMinWeekCol AndAlso nCol <= nMaxWeekCol) Or (nCol = nTotalCol)) Then
                                Dim sNum As String = dr.Item(nCol - ChartConfig.TableCol).ToString
                                Dim nNum As Integer
                                If sNum = String.Empty Then
                                    nNum = 0
                                    oSheet.Cells(nRow, nCol) = " "
                                Else
                                    nNum = CInt(sNum)
                                    oSheet.Cells(nRow, nCol) = " " & frmMain.sGetDurationString(nNum)
                                End If
                            Else
                                'Some extra spaces are showing up in the alarm descriptions
                                'Clean them out, but let the numeric data get set as numbers - It might not matter, but I don't know.
                                If (nCol = nDescCol) Or (nCol = nAlmCol) Then
                                    oSheet.Cells(nRow, nCol) = Trim(dr.Item(nCol - ChartConfig.TableCol).ToString)
                                Else
                                    oSheet.Cells(nRow, nCol) = dr.Item(nCol - ChartConfig.TableCol)
                                End If
                            End If
                    End Select
                Next
                If bCombDesc Then
                    'Add combined alarm # and description column
                    oSheet.Cells(nRow, ChartConfig.AlarmLabelCol) = Trim(dr.Item(nAlmCol - ChartConfig.TableCol).ToString) & " " & Trim(dr.Item(nDescCol - ChartConfig.TableCol).ToString)
                End If
            Next

            'Select everything except the big titles then do the autofit
            nRow = ChartConfig.TableRow + 1
            oCell1 = oSheet.Cells(nRow, ChartConfig.TableCol)
            nRow = ChartConfig.AlarmsRow1 + ChartConfig.NumAlarms - 1
            oCell2 = oSheet.Cells(nRow, ChartConfig.AlarmLabelCol)
            oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
            oRange.Columns.AutoFit()
            'Take the total and overall rows out of the alarm count
            ChartConfig.NumAlarms -= 2

        Catch ex As Exception
            'Error selecting the file
            Dim lReply As Windows.Forms.DialogResult = Response.OK
            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msMODULE_NAME, _
            frmMain.Status, MessageBoxButtons.OK)
            Exit Sub

        End Try
    End Sub
    Friend Sub subBuildDTSummarySheetFromDataSet(ByRef oXl As Excel.Application, _
                                                  ByRef oWB As Excel.Workbook, _
                                                  ByRef oSheet As Excel.Worksheet, _
                                                  ByRef DT As DataTable, _
                                                  ByRef DR() As DataRow, _
                                                  ByRef sTitle As String, _
                                                  ByRef ChartConfig As tChartConfig, _
                                                  Optional ByVal bStats As Boolean = True, _
                                                  Optional ByVal bCombDesc As Boolean = True, _
                                                  Optional ByVal bDurTotal As Boolean = False)
        '********************************************************************************************
        'Description:  create a table in an excel sheet from a datarow array,
        '               DT parameter is used only for column labels
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'title data first
        Try
            Dim nRow As Integer
            Dim oRange As Excel.Range
            Dim oCell1 As Object
            Dim oCell2 As Object
            'Title row
            oSheet.Cells(ChartConfig.TableRow, ChartConfig.TableCol) = frmMain.colZones.SiteName & ": " & frmMain.colZones.CurrentZone
            oSheet.Cells(ChartConfig.TableRow, ChartConfig.TableCol + 2) = sTitle
            'Bold for the first row of titles
            oCell1 = oSheet.Cells(ChartConfig.TableRow, ChartConfig.TableCol)
            oCell2 = oSheet.Cells(ChartConfig.TableRow, ChartConfig.TableCol + 2)
            oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
            oRange.Font.Bold = True
            'Times

            oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol) = gpsRM.GetString("psSTART") & ": "
            oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol + 1) = ChartConfig.StartTime.ToString("d")
            oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol + 2) = gpsRM.GetString("psEND") & ": "
            oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol + 3) = ChartConfig.EndTime.ToString("d")

            'Stats
            If bStats Then
                nRow = ChartConfig.TableRow + 2
                oSheet.Cells(nRow, ChartConfig.TableCol) = gpsRM.GetString("psUPTIME") & ": "
                oSheet.Cells(nRow, ChartConfig.TableCol + 1) = ChartConfig.UptimePct.ToString("00.0") & "%"
                oSheet.Cells(nRow, ChartConfig.TableCol + 2) = gpsRM.GetString("psMTBF") & ": "
                oSheet.Cells(nRow, ChartConfig.TableCol + 3) = " " & frmMain.sGetDurationString(ChartConfig.MTBF)
                oSheet.Cells(nRow, ChartConfig.TableCol + 4) = gpsRM.GetString("psMTTR") & ": "
                oSheet.Cells(nRow, ChartConfig.TableCol + 5) = " " & frmMain.sGetDurationString(ChartConfig.MTTR)
            Else
                nRow = ChartConfig.TableRow + 1
            End If

            'Left align the rows
            oCell1 = oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol)
            oCell2 = oSheet.Cells(nRow, ChartConfig.TableCol + 5)
            oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
            oRange.HorizontalAlignment = Excel.Constants.xlLeft
            'Then right align the labels so they meet up with the data
            oCell1 = oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol)
            oCell2 = oSheet.Cells(nRow, ChartConfig.TableCol)
            oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
            oRange.HorizontalAlignment = Excel.Constants.xlRight
            oCell1 = oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol + 2)
            oCell2 = oSheet.Cells(nRow, ChartConfig.TableCol + 2)
            oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
            oRange.HorizontalAlignment = Excel.Constants.xlRight
            oCell1 = oSheet.Cells(ChartConfig.TableRow + 1, ChartConfig.TableCol + 4)
            oCell2 = oSheet.Cells(nRow, ChartConfig.TableCol + 4)
            oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
            oRange.HorizontalAlignment = Excel.Constants.xlRight

            'Column labels
            nRow += 1
            'More column Config
            ChartConfig.AlarmsRow1 = nRow + 1
            ChartConfig.NumAlarms = DR.GetUpperBound(0) + 1
            ChartConfig.AlarmLabelCol = DT.Columns.Count + ChartConfig.TableCol
            Dim nAlmCol As Integer = 0
            Dim nDescCol As Integer = 0
            Dim nMaxWeekCol As Integer = 0
            Dim nMinWeekCol As Integer = 999999
            Dim nTotalCol As Integer = 0
            For nCol As Integer = ChartConfig.TableCol To DT.Columns.Count + ChartConfig.TableCol - 1
                oSheet.Cells(nRow, nCol) = DT.Columns.Item(nCol - ChartConfig.TableCol).Caption
                Dim sCol As String = DT.Columns.Item(nCol - ChartConfig.TableCol).Caption
                Select Case sCol
                    Case gpsRM.GetString("psPERCENT_CAP"), gpsRM.GetString("psOCCURRENCES_PERC_CAP"), gpsRM.GetString("psDURATION_PERC_CAP")
                        'Percentages stored as decimal value (10% = 0.0), set the format in excel
                        oCell1 = oSheet.Cells(ChartConfig.AlarmsRow1, nCol)
                        oCell2 = oSheet.Cells(ChartConfig.AlarmsRow1 + ChartConfig.NumAlarms - 1, nCol)
                        oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
                        oRange.NumberFormat = "0.0%"
                    Case gpsRM.GetString("psDURATION_CAP")
                        ChartConfig.DurCol = nCol
                    Case gpsRM.GetString("psOCCURRENCES_CAP")
                        ChartConfig.OccurCol = nCol
                    Case gpsRM.GetString("psALARM_CAP")
                        nAlmCol = nCol
                    Case gpsRM.GetString("psDESC_CAP")
                        nDescCol = nCol
                    Case gpsRM.GetString("psTOTAL_CAP")
                        nTotalCol = nCol
                    Case Else
                        Dim sTmp1 As String() = Split(DT.Columns.Item(nCol - ChartConfig.TableCol).ColumnName.Substring(1), "_")
                        Dim bWeekly As Boolean = True
                        For Each sTmp2 As String In sTmp1
                            If IsNumeric(sTmp2) = False Then
                                bWeekly = False
                            End If
                        Next
                        nMaxWeekCol = nCol
                        If bWeekly Then
                            If nMaxWeekCol < nCol Then
                                nMaxWeekCol = nCol
                            ElseIf nMinWeekCol > nCol Then
                                nMinWeekCol = nCol
                            End If
                        End If
                End Select
            Next
            'Label the columns we're adding in
            If bCombDesc Then
                'Alarm-Desc - combine these columns into a 3rd so the chart can access it.
                oSheet.Cells(nRow, ChartConfig.AlarmLabelCol) = gpsRM.GetString("psALARM_CAP") & " " & gpsRM.GetString("psDESC_CAP")
            End If
            'Bold font for the column titles
            oCell1 = oSheet.Cells(nRow, ChartConfig.TableCol)
            oCell2 = oSheet.Cells(nRow, ChartConfig.AlarmLabelCol)
            oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
            oRange.Font.Bold = True

            For nDRindex As Integer = 0 To DR.GetUpperBound(0)
                nRow += 1
                For nCol As Integer = ChartConfig.TableCol To DT.Columns.Count + ChartConfig.TableCol - 1

                    Select Case nCol
                        Case ChartConfig.DurCol
                            'Can't get excel to format this right, so add another row as a text
                            'oSheet.Cells(nRow, nCol) = " " & frmMain.sGetDurationString(CInt(DR(nDRindex).Item(nCol - ChartConfig.TableCol)))
                            'MSW 6/17/14 - That messed up the charts.  We need to let excel treat this as a date instead of a string
                            oSheet.Cells(nRow, nCol) = frmMain.sGetDurationString(CInt(DR(nDRindex).Item(nCol - ChartConfig.TableCol)))

                        Case Else
                            If bDurTotal AndAlso ((nCol >= nMinWeekCol AndAlso nCol <= nMaxWeekCol) Or (nCol = nTotalCol)) Then
                                Dim sNum As String = DR(nDRindex).Item(nCol - ChartConfig.TableCol).ToString
                                Dim nNum As Integer
                                If sNum = String.Empty Then
                                    nNum = 0
                                    oSheet.Cells(nRow, nCol) = " "
                                Else
                                    nNum = CInt(sNum)
                                    oSheet.Cells(nRow, nCol) = " " & frmMain.sGetDurationString(nNum)
                                End If
                            Else

                                'Some extra spaces are showing up in the alarm descriptions
                                'Clean them out, but let the numeric data get set as numbers - It might not matter, but I don't know.
                                If (nCol = nDescCol) Or (nCol = nAlmCol) Then
                                    oSheet.Cells(nRow, nCol) = Trim(DR(nDRindex).Item(nCol - ChartConfig.TableCol).ToString)
                                Else
                                    oSheet.Cells(nRow, nCol) = DR(nDRindex).Item(nCol - ChartConfig.TableCol)
                                End If
                            End If
                    End Select
                Next

                If bCombDesc Then
                    'Add combined alarm # and description column
                    oSheet.Cells(nRow, ChartConfig.AlarmLabelCol) = Trim(DR(nDRindex).Item(nAlmCol - ChartConfig.TableCol).ToString) & " " & Trim(DR(nDRindex).Item(nDescCol - ChartConfig.TableCol).ToString)
                End If
            Next

            'Select everything except the big titles then do the autofit
            nRow = ChartConfig.TableRow + 2
            oCell1 = oSheet.Cells(nRow, ChartConfig.TableCol)
            nRow = ChartConfig.AlarmsRow1 + ChartConfig.NumAlarms + 1
            oCell2 = oSheet.Cells(nRow, ChartConfig.AlarmLabelCol)
            oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
            oRange.Columns.AutoFit()
            ''Take the total and overall rows out of the alarm count
            'Not if it comes in as a sorted array
            'ChartConfig.NumAlarms -= 2
            If nTotalCol > 0 Then
                nRow = ChartConfig.TableRow + 2
                oCell1 = oSheet.Cells(nRow, nTotalCol)
                nRow = ChartConfig.AlarmsRow1 + ChartConfig.NumAlarms + 1
                oCell2 = oSheet.Cells(nRow, nTotalCol + 1)
                oRange = DirectCast(oSheet.Range(oCell1, oCell2), Excel.Range)
                oRange.Font.Bold = True
            End If
        Catch ex As Exception
            'Error selecting the file
            Dim lReply As Windows.Forms.DialogResult = Response.OK
            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msMODULE_NAME, _
            frmMain.Status, MessageBoxButtons.OK)
            Exit Sub

        End Try
    End Sub
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
    Friend Function subDoManualChart(ByVal TableIn As DataTable, _
                                          ByRef ChartConfig As tChartConfig, _
                                          Optional ByRef sSummType As String = "") As DataTable
        '********************************************************************************************
        'Description:  create a chart from the current display data 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/23/09  MSW     subDoManualChart - Handle small tables, allow "TableIn.Rows.Count < 1" instead of "TableIn.Rows.Count < 3"
        '********************************************************************************************
        Const sXLS_EXT As String = "xlsx"
        Dim sTarget As String = String.Empty

        Try
            If TableIn.Rows.Count < 1 Then
                'msw  was < 3 due to the following
                '   'There is a row for totals and a row for the overall time,
                '   'so this table has no alarm records
                'It's not true, it just needs one row
                frmMain.Progress = 0
                'subBuildReport will throw up a pop-up box with a message
                Return Nothing
            End If
            'select the file name
            Dim o As New SaveFileDialog
            With o
                .Filter = gcsRM.GetString("csSAVE_CSV_CDFILTER")
                .Title = gcsRM.GetString("csSAVE_FILE_DLG_CAP")
                .AddExtension = True
                .CheckPathExists = True
                .Filter = "All Files|*.*|Excel File|*.xlsx"
                .DefaultExt = "." & sXLS_EXT
                .FilterIndex = 3
                .ShowDialog()
                sTarget = .FileName
            End With
            Application.DoEvents()
            'user pressed cancel
            If sTarget = String.Empty Then
                frmMain.Progress = 0
                Return Nothing
            End If
            'Force the extension name

            frmMain.Cursor = System.Windows.Forms.Cursors.AppStarting
            Dim sTargetSplit As String() = Split(sTarget, ".")
            'If there's no extension, add xls.
            If sTargetSplit.GetUpperBound(0) < 1 Then
                sTarget = sTarget & "." & sXLS_EXT
            Else
                If sTargetSplit(sTargetSplit.GetUpperBound(0)).ToLower <> sXLS_EXT Then
                    sTarget = sTarget & "." & sXLS_EXT
                End If
            End If
        Catch ex As Exception
            'Error selecting the file
            Dim lReply As Windows.Forms.DialogResult = Response.OK
            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msMODULE_NAME, _
            frmMain.Status, MessageBoxButtons.OK)
            frmMain.Progress = 0
            Return Nothing
        End Try
        Application.DoEvents()
        '********************************************************************************************
        'Excel objects
        '********************************************************************************************
        Dim oXl As Excel.Application = Nothing
        Dim oWb As Excel.Workbook = Nothing
        Dim oTableSheet As Excel.Worksheet = Nothing
        Dim oChartSheet As Excel.Worksheet = Nothing
        Try
            'Start up excel
            frmMain.Progress = 1
            frmMain.Status = gpsRM.GetString("psCREATING_SPREADSHEET_FILE")

            frmMain.Progress = 10

            '********************************************************************************************
            'Start excel up and create a file (workbook)
            'Default creates 3 sheets.  I'm sure that could change if we left that assumption, so any extras
            'are getting deleted and if we're short, make sure they get added
            '********************************************************************************************
            oXl = DirectCast(CreateObject("Excel.Application"), Excel.Application)
            oWb = oXl.Workbooks.Add()
#If EXCEL12 Then
            oWb.SaveAs(Filename:=sTarget, FileFormat:=Excel.XlFileFormat.xlOpenXMLWorkbook, AccessMode:=Excel.XlSaveAsAccessMode.xlExclusive, ConflictResolution:=Excel.XlSaveConflictResolution.xlOtherSessionChanges)
#Else
            oWb.SaveAs(Filename:=sTarget, FileFormat:=Excel.XlFileFormat.xlWorkbookNormal, AccessMode:=Excel.XlSaveAsAccessMode.xlExclusive, ConflictResolution:=Excel.XlSaveConflictResolution.xlOtherSessionChanges)
#End If

            oXl.Visible = mPWCommon.InIDE

            frmMain.Progress = 15
            Application.DoEvents()
            '********************************************************************************************
            'set up config data to pass through all the chart subroutines 
            '********************************************************************************************
            'configure some info for putting the data in the excel file

            '********************************************************************************************
            'set up config data to pass through all the chart subroutines 
            '********************************************************************************************

            'Check each chart type
            'Do the charts in aloop, first pass for duration, 2nd for Occurrence
            Dim nLoopStart As Integer = 1
            Dim nLoopStop As Integer = 2
            Dim nNumSheets As Integer = 3
            Const nBY_DUR As Integer = 1
            Const nBY_OCCUR As Integer = 2
            Select Case sSummType
                Case gpsRM.GetString("psDURATION_CAP")
                    nLoopStart = nBY_DUR
                    nLoopStop = nBY_DUR
                    ChartConfig.ChartHeight = 550
                    nNumSheets = 3
                Case gpsRM.GetString("psOCCURRENCES_CAP")
                    nLoopStart = nBY_OCCUR
                    nLoopStop = nBY_OCCUR
                    ChartConfig.ChartHeight = 550
                    nNumSheets = 3
                Case gpsRM.GetString("psBOTH_CAP")
                    ChartConfig.ChartHeight = 300
                    nLoopStart = nBY_DUR
                    nLoopStop = nBY_OCCUR
                    nNumSheets = 4
            End Select
            Do While oWb.Worksheets.Count > nNumSheets
                Dim oSheet As Excel.Worksheet = DirectCast(oWb.Worksheets.Item(oWb.Worksheets.Count), Excel.Worksheet)
                oSheet.Delete()
                oSheet = Nothing
            Loop
            Do While oWb.Worksheets.Count < nNumSheets
                oWb.Worksheets.Add(Count:=1, Type:=Excel.XlSheetType.xlWorksheet)
            Loop
            ChartConfig.ChartSheet = gpsRM.GetString("psGRAPH_SHEET_NAME")
            oChartSheet = DirectCast(oWb.Worksheets.Item(1), Excel.Worksheet)
            oChartSheet.Name = ChartConfig.ChartSheet
            'Tag the 2 worksheets we're using
            ChartConfig.TableRow = 1
            ChartConfig.TableCol = 1
            ChartConfig.ChartTop = 1
            ChartConfig.ChartLeft = 1
            ChartConfig.ChartWidth = 900
            ChartConfig.Title = String.Empty
            ChartConfig.ChartType = Excel.XlChartType.xlBarClustered
            Dim DT As DataTable = Nothing
            Dim sTitle As String = String.Empty
            Dim nSheetNum As Integer = nNumSheets
            '********************************************************************************************
            'Fill in the alarm table on sheet nNumSheets
            'Basically another save or print routine, but speciallized for DT summaries
            '********************************************************************************************
            ChartConfig.TableSheet = gpsRM.GetString("psALARMS_SHEET_NAME")
            oTableSheet = DirectCast(oWb.Worksheets.Item(nSheetNum), Excel.Worksheet)
            oTableSheet.Name = ChartConfig.TableSheet
            'Add in some more details for the title section before the table

            subBuildDTAlarmSheetFromDataSet(oXl, oWb, oTableSheet, TableIn, ChartConfig)


            Application.DoEvents()
            For nLoop As Integer = nLoopStart To nLoopStop
                Select Case nLoop
                    Case nBY_DUR 'duration
                        ChartConfig.ByOccurrence = False
                        ChartConfig.TableSheet = gpsRM.GetString("psDURATION_SUMMARY_SHEET_NAME")
                        nSheetNum = 2
                        oTableSheet = DirectCast(oWb.Worksheets.Item(nSheetNum), Excel.Worksheet)
                        oTableSheet.Name = ChartConfig.TableSheet
                    Case nBY_OCCUR ' Occurrence
                        ChartConfig.ByOccurrence = True
                        'If it's doing both, shift it down
                        If nLoop <> nLoopStart Then
                            nSheetNum = 3
                            ChartConfig.ChartTop = ChartConfig.ChartTop + ChartConfig.ChartHeight + 2
                            'ChartConfig.TableRow = ChartConfig.AlarmsRow1 + ChartConfig.NumAlarms + 5
                        Else
                            nSheetNum = 2
                        End If
                        ChartConfig.TableSheet = gpsRM.GetString("psOCCURRENCE_SUMMARY_SHEET_NAME")
                        oTableSheet = DirectCast(oWb.Worksheets.Item(nSheetNum), Excel.Worksheet)
                        oTableSheet.Name = ChartConfig.TableSheet
                End Select
                '********************************************************************************************
                'Load summary data and titles
                '********************************************************************************************
                frmMain.Status = gpsRM.GetString("psFINDING_CHART_DATA")
                subGetChartData(TableIn, DT, sTitle, ChartConfig)
                Application.DoEvents()
                frmMain.Progress = 35 + 20 * (nLoop - 1)

                '********************************************************************************************
                'Fill in the summary table on sheet 2 or 3
                'Basically another save or print routine, but speciallized for DT summaries
                '********************************************************************************************
                frmMain.Status = gpsRM.GetString("psBUILDING_TABLE")
                subBuildDTSummarySheetFromDataSet(oXl, oWb, oTableSheet, DT, sTitle, ChartConfig)

                '********************************************************************************************
                'Put the chart in sheet 1
                '********************************************************************************************
                frmMain.Status = gpsRM.GetString("psBUILDING_CHART")

                'Manual charts use a the long detailed title text
                ChartConfig.Title = frmMain.colZones.SiteName & ": " & frmMain.colZones.CurrentZone & vbTab & sTitle & Chr(13) & _
                    gpsRM.GetString("psSTART") & ": " & ChartConfig.StartTime.ToString & vbTab & _
                    gpsRM.GetString("psEND") & ": " & ChartConfig.EndTime.ToString & Chr(13) & _
                    gpsRM.GetString("psUPTIME") & ": " & ChartConfig.UptimePct.ToString("00.0") & "%" & vbTab & _
                    gpsRM.GetString("psMTBF") & ": " & " " & frmMain.sGetDurationString(ChartConfig.MTBF) & vbTab & _
                    gpsRM.GetString("psMTTR") & ": " & " " & frmMain.sGetDurationString(ChartConfig.MTTR)
                subBuildChart(oXl, oWb, oTableSheet, oChartSheet, ChartConfig)

            Next

            frmMain.Progress = 75

            Application.DoEvents()
            oWb.Save()
            oXl.Visible = True
            'oWb.Close(Excel.XlSaveAction.xlSaveChanges)

            Application.DoEvents()

            'oXl.Application.Quit()
            'oXl.Quit()
            oTableSheet = Nothing
            oChartSheet = Nothing
            oWb = Nothing
            oXl = Nothing

            Application.DoEvents()
            frmMain.Progress = 100
            Return DT
        Catch ex As Exception
            Application.DoEvents()
            Dim lReply As Windows.Forms.DialogResult = Response.OK
            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msMODULE_NAME, _
            frmMain.Status, MessageBoxButtons.OK)
            oWb.Close(Excel.XlSaveAction.xlSaveChanges)

            oXl.Application.Quit()
            oXl.Quit()
            oTableSheet = Nothing
            oChartSheet = Nothing
            oWb = Nothing
            oXl = Nothing
            Application.DoEvents()
            frmMain.Progress = 0
            Return Nothing
        End Try
        frmMain.Status = gcsRM.GetString("csREADY")
        frmMain.Cursor = System.Windows.Forms.Cursors.Default

    End Function
    Friend Sub subAddArrayToTotalsDT(ByRef drArray As DataRow(), ByRef DT_Totals As DataTable, _
                                     Optional ByVal bOccurances As Boolean = True, _
                                     Optional ByVal bDuration As Boolean = True, _
                                     Optional ByRef sWeeklyCol As String = "")
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
        Dim nWeekCol As Integer = 0
        Dim nDurCol As Integer = 0
        Dim nOccCol As Integer = 0
        Dim nPercCol As Integer = 0
        For nItem As Integer = 0 To DT_Totals.Columns.Count() - 1
            Debug.Print(nItem.ToString & " :  " & DT_Totals.Columns(nItem).ColumnName & "  :   " & DT_Totals.Columns(nItem).Caption)
            If DT_Totals.Columns(nItem).ColumnName = sWeeklyCol Then
                nWeekCol = nItem
            End If
            If DT_Totals.Columns(nItem).ColumnName = gpsRM.GetString("psOCCURRENCES_CAP") Then
                nOccCol = nItem
            End If
            If DT_Totals.Columns(nItem).ColumnName = gpsRM.GetString("psDURATION_CAP") Then
                nDurCol = nItem
            End If
            If DT_Totals.Columns(nItem).ColumnName = gpsRM.GetString("psPERCENT_CAP") Then
                nPercCol = nItem
            End If

        Next
        For Each DR_Alarm As DataRow In drArray
            sAlarm = DR_Alarm.Item(gpsRM.GetString("psALARM_CAP")).ToString
            If bOccurances Then
                If (sAlarm <> gpsRM.GetString("psOVERALL")) Then
                    nOccurrences = CInt(DR_Alarm.Item(gpsRM.GetString("psOCCURRENCES_CAP")))
                Else
                    nOccurrences = 0
                End If
            End If
            If bDuration Then
                nDuration = CInt(DR_Alarm.Item(gpsRM.GetString("psDURATION_CAP")))
            End If
            bFound = False
            For Each DR_TotAlarm As DataRow In DT_Totals.Rows
                If sAlarm = DR_TotAlarm.Item(gpsRM.GetString("psALARM_CAP")).ToString Then
                    bFound = True
                    If bOccurances Then
                        If sWeeklyCol <> String.Empty Then
                            DR_TotAlarm.Item(nWeekCol) = nOccurrences
                        End If
                        If (sAlarm <> gpsRM.GetString("psOVERALL")) Then
                            nOccurrences += CInt(DR_TotAlarm.Item(gpsRM.GetString("psOCCURRENCES_CAP")))
                        Else
                            nOccurrences = 0
                        End If
                        DR_TotAlarm.Item(nOccCol) = nOccurrences
                    End If
                    If bDuration Then
                        If sWeeklyCol <> String.Empty Then
                            DR_TotAlarm.Item(nWeekCol) = nDuration
                        End If
                        nDuration += CInt(DR_TotAlarm.Item(gpsRM.GetString("psDURATION_CAP")))
                        DR_TotAlarm.Item(nDurCol) = nDuration
                    End If
                End If
            Next
            If bFound = False Then
                'Not found in the total table, add a record
                Dim DR_Add As DataRow = DT_Totals.NewRow
                DR_Add.BeginEdit()
                DR_Add.Item(DT_Totals.Columns.Item(gpsRM.GetString("psALARM_CAP")).Caption) = _
                        DR_Alarm.Item(DT_Totals.Columns.Item(gpsRM.GetString("psALARM_CAP")).Caption)
                DR_Add.Item(DT_Totals.Columns.Item(gpsRM.GetString("psDESC_CAP")).Caption) = _
                        DR_Alarm.Item(DT_Totals.Columns.Item(gpsRM.GetString("psDESC_CAP")).Caption)
                DR_Add.Item(DT_Totals.Columns.Item(gpsRM.GetString("psALARM_CAP")).Caption) = _
                        DR_Alarm.Item(DT_Totals.Columns.Item(gpsRM.GetString("psALARM_CAP")).Caption)
                If bOccurances Then
                    If sWeeklyCol <> String.Empty Then
                        DR_Add.Item(nWeekCol) = nOccurrences
                    End If
                    DR_Add.Item(nOccCol) = nOccurrences
                End If
                If bDuration Then
                    If sWeeklyCol <> String.Empty Then
                        DR_Add.Item(nWeekCol) = nDuration
                    End If
                    DR_Add.Item(nDurCol) = nDuration
                End If
                'For nColNum As Integer = 0 To DT_Totals.Columns.Count - 1
                '    DR_Add.Item(DT_Totals.Columns.Item(nColNum).Caption) = DR_Alarm.Item(DT_Totals.Columns.Item(nColNum).Caption)
                'Next
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
    Friend Sub subSetBorders(ByRef oRange As Excel.Range, ByVal bAll As Boolean)
        '********************************************************************************************
        'Description: put borders on an excel selection
        '
        'Parameters:  bAll - true borders everything - in and out.
        '                   false leaves internal horizontal clear
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        With oRange.Borders(Excel.XlBordersIndex.xlEdgeLeft)
            .LineStyle = Excel.XlLineStyle.xlContinuous
            .ColorIndex = Excel.Constants.xlAutomatic
            '.TintAndShade = 0
            .Weight = Excel.XlBorderWeight.xlThin
        End With
        With oRange.Borders(Excel.XlBordersIndex.xlEdgeTop)
            .LineStyle = Excel.XlLineStyle.xlContinuous
            .ColorIndex = Excel.Constants.xlAutomatic
            '.TintAndShade = 0
            .Weight = Excel.XlBorderWeight.xlThin
        End With
        With oRange.Borders(Excel.XlBordersIndex.xlEdgeBottom)
            .LineStyle = Excel.XlLineStyle.xlContinuous
            .ColorIndex = Excel.Constants.xlAutomatic
            '.TintAndShade = 0
            .Weight = Excel.XlBorderWeight.xlThin
        End With
        With oRange.Borders(Excel.XlBordersIndex.xlEdgeRight)
            .LineStyle = Excel.XlLineStyle.xlContinuous
            .ColorIndex = Excel.Constants.xlAutomatic
            '.TintAndShade = 0
            .Weight = Excel.XlBorderWeight.xlThin
        End With
        With oRange.Borders(Excel.XlBordersIndex.xlInsideVertical)
            .LineStyle = Excel.XlLineStyle.xlContinuous
            .ColorIndex = Excel.Constants.xlAutomatic
            '.TintAndShade = 0
            .Weight = Excel.XlBorderWeight.xlThin
        End With

        With oRange.Borders(Excel.XlBordersIndex.xlInsideHorizontal)
            If bAll Then
                .LineStyle = Excel.XlLineStyle.xlContinuous
                .ColorIndex = Excel.Constants.xlAutomatic
                '.TintAndShade = 0
                .Weight = Excel.XlBorderWeight.xlThin
            Else
                .LineStyle = Excel.Constants.xlNone
            End If

        End With

    End Sub



    Friend Sub subWriteWeeklyTableToYearlySheet(ByRef oYearlySheet As Excel.Worksheet, _
                                       ByRef nYearlyTableRow As Integer, _
                                       ByRef DR_Occurrence() As DataRow, _
                                       ByRef DR_Duration() As DataRow, _
                                       ByRef dTableDate As Date, ByVal nMTBF As Integer, _
                                       ByVal nMTTR As Integer, ByVal fUptime As Double)
        '********************************************************************************************
        'Description: write the weekly tables on the yearly sheet
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim oRange As Excel.Range
        Dim oCell1 As Object
        Dim oCell2 As Object

        'Common text first
        oYearlySheet.Cells(nYearlyTableRow, 1) = gpsRM.GetString("psWEEK_OF") & " " & dTableDate.Date.ToShortDateString

        oYearlySheet.Cells(nYearlyTableRow, 4) = gpsRM.GetString("psUPTIME") & ": " & fUptime.ToString("00.0") & "%"


        oYearlySheet.Cells(nYearlyTableRow + 1, 1) = gpsRM.GetString("psMTBF") & ": " & " " & frmMain.sGetDurationString(nMTBF)
        oYearlySheet.Cells(nYearlyTableRow + 1, 4) = gpsRM.GetString("psMTTR") & ": " & " " & frmMain.sGetDurationString(nMTTR)
        'Center most of it and format the ntext
        oCell1 = oYearlySheet.Cells(nYearlyTableRow, 1)
        oCell2 = oYearlySheet.Cells(nYearlyTableRow + 1, 5)
        oRange = DirectCast(oYearlySheet.Range(oCell1, oCell2), Excel.Range)
        oRange.HorizontalAlignment = Excel.Constants.xlCenter
        oRange.Font.Italic = True
        'Go back and move the date left and make it bold
        oRange = DirectCast(oYearlySheet.Range(oCell1, oCell1), Excel.Range)
        oRange.Font.Bold = True
        oRange.Font.Italic = False
        oRange.HorizontalAlignment = Excel.Constants.xlLeft


        'Title the tables
        oYearlySheet.Cells(nYearlyTableRow + 2, 1) = gpsRM.GetString("psOCCURRENCES_CAP")
        oYearlySheet.Cells(nYearlyTableRow + 2, 2) = dTableDate.Date.ToShortDateString
        oYearlySheet.Cells(nYearlyTableRow + 2, 4) = gpsRM.GetString("psDURATION_CAP")
        oYearlySheet.Cells(nYearlyTableRow + 2, 5) = dTableDate.Date.ToShortDateString
        'Format the titles
        oCell1 = oYearlySheet.Cells(nYearlyTableRow + 2, 1)
        oRange = DirectCast(oYearlySheet.Range(oCell1, oCell1), Excel.Range)
        oRange.Font.Bold = True
        oRange.Font.Italic = True
        oRange.HorizontalAlignment = Excel.Constants.xlLeft
        oCell1 = oYearlySheet.Cells(nYearlyTableRow + 2, 4)
        oRange = DirectCast(oYearlySheet.Range(oCell1, oCell1), Excel.Range)
        oRange.Font.Bold = True
        oRange.Font.Italic = True
        oRange.HorizontalAlignment = Excel.Constants.xlLeft

        oYearlySheet.Cells(nYearlyTableRow + 3, 1) = gpsRM.GetString("psDESC_CAP")
        oYearlySheet.Cells(nYearlyTableRow + 3, 2) = gpsRM.GetString("psTOTAL_CAP")
        oYearlySheet.Cells(nYearlyTableRow + 3, 4) = gpsRM.GetString("psDESC_CAP")
        oYearlySheet.Cells(nYearlyTableRow + 3, 5) = gpsRM.GetString("psTOTAL_CAP")


        Dim nMax As Integer = DR_Occurrence.GetUpperBound(0)
        If DR_Duration.GetUpperBound(0) > nMax Then
            nMax = DR_Duration.GetUpperBound(0)
        End If
        Dim nTotalOccur As Integer = 0
        Dim nTotalDuration As Integer = 0
        For nRow As Integer = 0 To nMax
            If DR_Occurrence.GetUpperBound(0) >= nRow Then
                oYearlySheet.Cells(nYearlyTableRow + 4 + nRow, 1) = DR_Occurrence(nRow).Item(gpsRM.GetString("psDESC_CAP")).ToString
                Dim nOccur As Integer = CInt(DR_Occurrence(nRow).Item(gpsRM.GetString("psOCCURRENCES_CAP")))
                nTotalOccur += nOccur
                oYearlySheet.Cells(nYearlyTableRow + 4 + nRow, 2) = nOccur.ToString
            End If
            If DR_Duration.GetUpperBound(0) >= nRow Then
                oYearlySheet.Cells(nYearlyTableRow + 4 + nRow, 4) = DR_Duration(nRow).Item(gpsRM.GetString("psDESC_CAP")).ToString
                Dim nDuration As Integer = CInt(DR_Duration(nRow).Item(gpsRM.GetString("psDURATION_CAP")))
                nTotalDuration += nDuration
                oYearlySheet.Cells(nYearlyTableRow + 4 + nRow, 5) = " " & frmMain.sGetDurationString(nDuration)
            End If
        Next

        oYearlySheet.Cells(nYearlyTableRow + nMax + 5, 1) = gpsRM.GetString("psTOTAL_CAP")
        oYearlySheet.Cells(nYearlyTableRow + nMax + 5, 2) = nTotalOccur.ToString
        oYearlySheet.Cells(nYearlyTableRow + nMax + 5, 4) = gpsRM.GetString("psTOTAL_CAP")
        oYearlySheet.Cells(nYearlyTableRow + nMax + 5, 5) = " " & frmMain.sGetDurationString(nTotalDuration)



        'put some lines on the tables
        oCell1 = oYearlySheet.Cells(nYearlyTableRow + 3, 1)
        oCell2 = oYearlySheet.Cells(nYearlyTableRow + nMax + 5, 2)
        oRange = DirectCast(oYearlySheet.Range(oCell1, oCell2), Excel.Range)
        subSetBorders(oRange, True)
        oCell1 = oYearlySheet.Cells(nYearlyTableRow + 4, 1)
        oCell2 = oYearlySheet.Cells(nYearlyTableRow + nMax + 4, 2)
        oRange = DirectCast(oYearlySheet.Range(oCell1, oCell2), Excel.Range)
        subSetBorders(oRange, False)
        oCell1 = oYearlySheet.Cells(nYearlyTableRow + 3, 4)
        oCell2 = oYearlySheet.Cells(nYearlyTableRow + nMax + 5, 5)
        oRange = DirectCast(oYearlySheet.Range(oCell1, oCell2), Excel.Range)
        subSetBorders(oRange, True)
        oCell1 = oYearlySheet.Cells(nYearlyTableRow + 4, 4)
        oCell2 = oYearlySheet.Cells(nYearlyTableRow + nMax + 4, 5)
        oRange = DirectCast(oYearlySheet.Range(oCell1, oCell2), Excel.Range)
        subSetBorders(oRange, False)
        'Put a gap in before the next table
        nYearlyTableRow = nYearlyTableRow + nMax + 8

    End Sub
    Friend Sub subFinishYearlysheet(ByRef oXl As Excel.Application, _
                                                ByRef oWB As Excel.Workbook, _
                                                ByRef oYearlysheet As Excel.Worksheet, _
                                                ByVal nYear As Integer, _
                                                ByVal nYearlyTableRow As Integer, _
                                                ByRef DT_Occurrence_Year As DataTable, _
                                                ByRef DT_Duration_Year As DataTable, _
                                                ByRef oYearlyStatsSheet As Excel.Worksheet, _
                                                ByVal nYearlyStatsRow As Integer)
        Dim oRange As Excel.Range
        Dim oCell1 As Object
        Dim oCell2 As Object

        'yearly stats sheet
        '********************************************************************************************
        'Bold for the date column
        oCell1 = oYearlyStatsSheet.Cells(1, 1)
        oCell2 = oYearlyStatsSheet.Cells(nYearlyStatsRow, 1)
        oRange = DirectCast(oYearlyStatsSheet.Range(oCell1, oCell2), Excel.Range)
        oRange.Font.Bold = True
        'Format the percentages
        oCell1 = oYearlyStatsSheet.Cells(1, 4)
        oCell2 = oYearlyStatsSheet.Cells(nYearlyStatsRow, 4)
        oRange = DirectCast(oYearlyStatsSheet.Range(oCell1, oCell2), Excel.Range)
        oRange.NumberFormat = "0.00%"
        oCell1 = oYearlyStatsSheet.Cells(1, 1)
        oCell2 = oYearlyStatsSheet.Cells(nYearlyStatsRow, 4)
        oRange = DirectCast(oYearlyStatsSheet.Range(oCell1, oCell2), Excel.Range)
        oRange.Columns.AutoFit()
        '********************************************************************************************

        'Autofit the column width
        oCell1 = oYearlysheet.Cells(2, 1)
        oCell2 = oYearlysheet.Cells(nYearlyTableRow - 3, 5)
        oRange = DirectCast(oYearlysheet.Range(oCell1, oCell2), Excel.Range)
        oRange.Columns.AutoFit()

        'Print yearly summary tables
        Dim ChartConfig As tChartConfig
        'common chart and table config - location, size
        ChartConfig.TableRow = 1
        ChartConfig.TableCol = 8
        'New height and width
        ChartConfig.TopxxReq = 5
        ChartConfig.StartTime = New Date(nYear, 1, 1)
        ChartConfig.EndTime = New Date(nYear, 12, 31)

        'Duration summary
        ChartConfig.ByOccurrence = False
        ChartConfig.TableSheet = oYearlysheet.Name
        ChartConfig.ChartSheet = oYearlysheet.Name
        Dim sSummType As String = gpsRM.GetString("psDURATION_CAP")
        'Add in some more details for the title section before the table
        ChartConfig.Title = gpsRM.GetString("psTOT_EACH_WEEK") & gpsRM.GetString("psTOP_X") & ChartConfig.TopxxReq.ToString & " " & _
                gpsRM.GetString("psPIE_TITLE_DT") & " " & sSummType & " - " & oYearlysheet.Name
        'Add total row, percent columns
        subAddTotals(DT_Duration_Year)
        'Dim drDurationTotals() As DataRow = DT_Duration_Year.Select(String.Empty, gpsRM.GetString("psDURATION_CAP"), DataViewRowState.CurrentRows)
        Dim drDurationTotals() As DataRow = DT_Duration_Year.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows)
        'Note this routine that takes datarow arrays is a little different from the ones that get datatables passed in
        subBuildDTSummarySheetFromDataSet(oXl, oWB, oYearlysheet, DT_Duration_Year, drDurationTotals, ChartConfig.Title, ChartConfig, False, False, True)


        'Select everything except the big titles then do the autofit
        Dim nRow As Integer = ChartConfig.TableRow + 2
        oCell1 = oYearlysheet.Cells(nRow, ChartConfig.TableCol + 2)
        nRow = ChartConfig.AlarmsRow1 + ChartConfig.NumAlarms
        oCell2 = oYearlysheet.Cells(nRow, ChartConfig.AlarmLabelCol)
        Dim oRange1 As Excel.Range = DirectCast(oYearlysheet.Range(oCell1, oCell2), Excel.Range)
        'Change to manual width, I don't know if autofit can deal with two seperate blocks.
        oRange1.ColumnWidth = 12
        'Bold Totals
        nRow -= 1
        oCell1 = oYearlysheet.Cells(nRow, ChartConfig.TableCol)
        oCell2 = oYearlysheet.Cells(nRow, ChartConfig.AlarmLabelCol)
        oRange = DirectCast(oYearlysheet.Range(oCell1, oCell2), Excel.Range)
        oRange.Font.Bold = True

        'Occurrence summary
        ChartConfig.TableRow = DT_Duration_Year.Rows.Count + 12
        ChartConfig.ByOccurrence = True
        sSummType = gpsRM.GetString("psOCCURRENCES_CAP")
        ChartConfig.Title = gpsRM.GetString("psTOT_EACH_WEEK") & gpsRM.GetString("psTOP_X") & ChartConfig.TopxxReq.ToString & " " & _
                gpsRM.GetString("psPIE_TITLE_DT") & " " & sSummType & " - " & oYearlysheet.Name
        'Add total row, percent columns
        subAddTotals(DT_Occurrence_Year)
        'Dim drOccurrenceTotals() As DataRow = DT_Occurrence_Year.Select(String.Empty, gpsRM.GetString("psOCCURRENCES_CAP"), DataViewRowState.CurrentRows)
        Dim drOccurrenceTotals() As DataRow = DT_Occurrence_Year.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows)
        'Note this routine that takes datarow arrays is a little different from the ones that get datatables passed in
        subBuildDTSummarySheetFromDataSet(oXl, oWB, oYearlysheet, DT_Occurrence_Year, drOccurrenceTotals, ChartConfig.Title, ChartConfig, False, False)

        'Select everything except the big titles then do the autofit
        nRow = ChartConfig.TableRow + 2
        oCell1 = oYearlysheet.Cells(nRow, ChartConfig.TableCol + 2)
        nRow = ChartConfig.AlarmsRow1 + ChartConfig.NumAlarms
        oCell2 = oYearlysheet.Cells(nRow, ChartConfig.AlarmLabelCol)
        Dim oRange2 As Excel.Range = DirectCast(oYearlysheet.Range(oCell1, oCell2), Excel.Range)
        'Change to manual width, I don't know if autofit can deal with two seperate blocks.
        oRange2.ColumnWidth = 12
        'Bold Totals
        nRow -= 1
        oCell1 = oYearlysheet.Cells(nRow, ChartConfig.TableCol)
        oCell2 = oYearlysheet.Cells(nRow, ChartConfig.AlarmLabelCol)
        oRange = DirectCast(oYearlysheet.Range(oCell1, oCell2), Excel.Range)
        oRange.Font.Bold = True



    End Sub
    Friend Sub subAddTotals(ByRef DT As DataTable)
        '********************************************************************************************
        'Description: Add a total row and fill out the percent column 
        'Parameters:
        'Returns:
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        'Column labels
        Dim nAlmCol As Integer = 0
        Dim nDescCol As Integer = 0
        Dim nMaxWeekCol As Integer = 0
        Dim nMinWeekCol As Integer = 999999
        Dim nTotalCol As Integer = 0
        Dim nPercentCol As Integer = 0
        For nCol As Integer = 0 To DT.Columns.Count - 1
            Dim sCol As String = DT.Columns.Item(nCol).Caption
            Debug.Print(DT.Columns.Item(nCol).ColumnName & "  -  " & DT.Columns.Item(nCol).Caption)
            Select Case sCol
                Case gpsRM.GetString("psPERCENT_CAP"), gpsRM.GetString("psOCCURRENCES_PERC_CAP"), gpsRM.GetString("psDURATION_PERC_CAP")
                    nPercentCol = nCol
                Case gpsRM.GetString("psALARM_CAP")
                    nAlmCol = nCol
                Case gpsRM.GetString("psDESC_CAP")
                    nDescCol = nCol
                Case gpsRM.GetString("psTOTAL_CAP")
                    nTotalCol = nCol
                Case Else
                    Dim sTmp1 As String() = Split(DT.Columns.Item(nCol).ColumnName.Substring(1), "_")
                    Dim bWeekly As Boolean = True
                    For Each sTmp2 As String In sTmp1
                        If IsNumeric(sTmp2) = False Then
                            bWeekly = False
                        End If
                    Next
                    nMaxWeekCol = nCol
                    If bWeekly Then
                        If nMaxWeekCol < nCol Then
                            nMaxWeekCol = nCol
                        ElseIf nMinWeekCol > nCol Then
                            nMinWeekCol = nCol
                        End If
                    End If
            End Select
        Next
        'Init Total Row
        Dim nNewRow As Integer = DT.Rows.Count
        DT.Rows.Add()
        DT.Rows(nNewRow).Item(nAlmCol) = String.Empty
        DT.Rows(nNewRow).Item(nDescCol) = gpsRM.GetString("psTOTAL_CAP")
        For nCol As Integer = nMinWeekCol To nMaxWeekCol
            Dim nRowTotal As Integer = 0
            For nRow As Integer = 0 To nNewRow - 1
                Dim sNum As String = DT.Rows(nRow).Item(nCol).ToString
                Dim nNum As Integer = 0
                If IsNumeric(sNum) Then
                    nNum = CInt(sNum)
                End If
                nRowTotal = nRowTotal + nNum
            Next
            DT.Rows(nNewRow).Item(nCol) = nRowTotal
        Next
        Dim nTotalTotal As Integer = 0
        For nRow As Integer = 0 To nNewRow - 1
            Dim sNum As String = DT.Rows(nRow).Item(nTotalCol).ToString
            Dim nNum As Integer = 0
            If IsNumeric(sNum) Then
                nNum = CInt(sNum)
            End If
            nTotalTotal = nTotalTotal + nNum
        Next
        DT.Rows(nNewRow).Item(nTotalCol) = nTotalTotal

        For nRow As Integer = 0 To nNewRow - 1
            Dim sNum As String = DT.Rows(nRow).Item(nTotalCol).ToString
            Dim nColTotal As Integer = 0
            If IsNumeric(sNum) Then
                nColTotal = CInt(sNum)
            End If
            DT.Rows(nRow).Item(nPercentCol) = nColTotal / nTotalTotal
        Next
        DT.Rows(nNewRow).Item(nPercentCol) = String.Empty

    End Sub
    Private Sub subMoveChart(ByRef oXl As Excel.Application, _
                                ByRef oWB As Excel.Workbook, _
                                ByRef oSheet As Excel.Worksheet, ByVal ChartIndex As Integer, ByVal FullPage As Boolean)
        '********************************************************************************************
        'Description:
        'Parameters:
        'Returns:
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' AM          09/04/2008    Chart was over the MTTR and MTBR. It was "+ 2 * nRow1" Changed it to
        '                           "+ 3 * nRow1"
        '********************************************************************************************

        Const nCol1 As Integer = 0
        Dim nRow1 As Integer
        Const nWidth As Integer = 700
        Const nHeight As Integer = 535
        Dim nCol2 As Integer
        Dim nRow2 As Integer
        Dim nRow3 As Integer
        Dim nRow4 As Integer
        Dim nRow5 As Integer

        Dim nC1Width As Integer
        Dim nC1Height As Integer
        Dim nTmp As Integer
        Dim sChart As String = "Chart"
        Dim oCell1 As Object
        Dim oRange As Excel.Range
        Dim oChartObjects As Excel.ChartObjects


        oCell1 = oSheet.Cells(6, 2)
        oRange = DirectCast(oSheet.Range(oCell1, oCell1), Excel.Range)
        nRow1 = CInt(oRange.Top)

        If FullPage Then
            ' this is a one chart deal
            With oSheet
                .Shapes.Item(1).Left = nCol1
                .Shapes.Item(1).Top = nRow1
                .Shapes.Item(1).Width = nWidth
                .Shapes.Item(1).Height = nHeight
            End With

        Else
            oChartObjects = DirectCast(oSheet.ChartObjects, Excel.ChartObjects)
            If oChartObjects.Count < ChartIndex Then
                nTmp = oChartObjects.Count
            Else
                nTmp = ChartIndex
            End If

            nCol2 = CInt(nWidth / 2) + 100
            nRow2 = CInt((nHeight / 3)) + 3 * nRow1 - 70
            nRow3 = 2 * CInt((nHeight / 3)) + 3 * nRow1 - 60
            nRow4 = 3 * CInt((nHeight / 3)) + 3 * nRow1
            nRow5 = 6 * CInt((nHeight / 3)) + 3 * nRow1
            nC1Height = CInt(((nHeight - (2 * 12.75)) / 3))
            nC1Width = CInt(nWidth / 2)



            Select Case ChartIndex
                Case 1
                    With oSheet
                        .Shapes.Item(nTmp).Left = nCol1
                        .Shapes.Item(nTmp).Top = nRow1
                        .Shapes.Item(nTmp).Width = nWidth
                        .Shapes.Item(nTmp).Height = nC1Height
                    End With

                Case 2
                    With oSheet
                        .Shapes.Item(nTmp).Left = nCol1
                        .Shapes.Item(nTmp).Top = nRow2
                        .Shapes.Item(nTmp).Width = nC1Width
                        .Shapes.Item(nTmp).Height = nC1Height
                    End With

                Case 3
                    With oSheet
                        .Shapes.Item(nTmp).Left = nCol1
                        .Shapes.Item(nTmp).Top = nRow3
                        .Shapes.Item(nTmp).Width = nC1Width
                        .Shapes.Item(nTmp).Height = nC1Height
                    End With

                Case 4
                    With oSheet
                        .Shapes.Item(nTmp).Left = nCol2
                        .Shapes.Item(nTmp).Top = nRow2
                        .Shapes.Item(nTmp).Width = nC1Width
                        .Shapes.Item(nTmp).Height = nC1Height
                    End With

                Case 5
                    With oSheet
                        .Shapes.Item(nTmp).Left = nCol2
                        .Shapes.Item(nTmp).Top = nRow3
                        .Shapes.Item(nTmp).Width = nC1Width
                        .Shapes.Item(nTmp).Height = nC1Height
                    End With

                Case 8
                    With oSheet
                        .Shapes.Item(nTmp).Left = nCol1
                        .Shapes.Item(nTmp).Top = nRow4
#If EXCEL12 Then
                        .Shapes.Item(nTmp).ScaleWidth(1.8, CType(False, Microsoft.Office.Core.MsoTriState), 0)
#Else
                        .Shapes.Item(nTmp).Width = CType((.Shapes.Item(nTmp).Width * 1.8), Single)
#End If
                        .Shapes.Item(nTmp).Height = .Shapes.Item(nTmp).Height + 200
                    End With

                Case 9
                    With oSheet
                        .Shapes.Item(nTmp).Left = nCol1
                        .Shapes.Item(nTmp).Top = nRow5
#If EXCEL12 Then
                        .Shapes.Item(nTmp).ScaleWidth(1.8, CType(False, Microsoft.Office.Core.MsoTriState), 0)
#Else
                        .Shapes.Item(nTmp).Width = CType((.Shapes.Item(nTmp).Width * 1.8), Single)
#End If
                        .Shapes.Item(nTmp).Height = .Shapes.Item(nTmp).Height + 200
                    End With

            End Select


        End If

        'subFormatChart(sTmp, FullPage)

    End Sub
    Friend Sub subDoAutoCharts(Optional ByRef dDate As Date = Nothing, _
                               Optional ByRef dBeginDate As Date = Nothing)
        '********************************************************************************************
        'Description: Run auto charts
        '
        'Parameters:  dDate - EndDate for chart
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim dEndTime As Date
        Dim dStartTime As Date
        Dim DT1 As DataTable
        Dim ChartConfig As tChartConfig
        Dim DTList As DataTable = Nothing

        Try

            '**********************************
            'Optional date settings
            If (dDate = Nothing) Then
                dEndTime = Now
                'We're expecting this around midnight saturday.  If it's close, round it off
                If dEndTime.DayOfWeek = DayOfWeek.Sunday Then
                    dEndTime = DateAdd(DateInterval.Second, -1, dEndTime.Date)
                ElseIf dEndTime.DayOfWeek = DayOfWeek.Saturday Then
                    dEndTime = DateAdd(DateInterval.Day, 1, dEndTime.Date)
                    dEndTime = DateAdd(DateInterval.Second, -1, dEndTime.Date)
                Else
                    'Some other time, just take what's given
                End If
            Else
                dEndTime = dDate
                'We're taking the passed in date until 11:59 PM
                'Get the next day, ignoring time of day
                dEndTime = DateAdd(DateInterval.Day, 1, dEndTime.Date)
                'Subtract 1 second from tommorow to get the end.
                dEndTime = DateAdd(DateInterval.Second, -1, dEndTime.Date)
            End If

            If (dDate = Nothing) Then
                'Subtract a week from the end, then add 1 second to get back to midnight
                dStartTime = DateAdd(DateInterval.Month, -2, dEndTime)
                dStartTime = DateAdd(DateInterval.Second, 1, dStartTime)
            Else
                dStartTime = dBeginDate.Date
            End If

            frmMain.subGetDTMask()
            frmCriteria.subSetupChartCriteria()
        Catch ex As Exception
            Dim lReply As Windows.Forms.DialogResult = Response.OK
            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msMODULE_NAME, _
            frmMain.Status, MessageBoxButtons.OK)
        End Try
        '********************************************************************************************
        'Excel objects
        '********************************************************************************************
        Dim oXl As Excel.Application = Nothing
        Dim oWb As Excel.Workbook = Nothing
        Dim oWbYearly As Excel.Workbook = Nothing
        Dim oYearlySheet As Excel.Worksheet = Nothing
        Dim oYearlyStatsSheet As Excel.Worksheet = Nothing
        Dim oTableSheet As Excel.Worksheet = Nothing
        Dim oChartSheet As Excel.Worksheet = Nothing
        Dim oRange As Excel.Range
        Dim oCell1 As Object
        Dim oCell2 As Object
        Dim nChartNumber As Integer

        Try
            '********************************************************************************************
            'Get the directory first
            '********************************************************************************************
            Dim sTarget As String = String.Empty
            Dim sTargetPath As String = String.Empty
            If Not (mPWCommon.GetDefaultFilePath(sTargetPath, eDir.WeeklyReports, String.Empty, String.Empty)) Then
                'Can't get directory
                MessageBox.Show(gpsRM.GetString("psWEEKLY_REP_DIR_ERR"), frmMain.msSCREEN_NAME, MessageBoxButtons.OK, _
                 MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, _
                MessageBoxOptions.DefaultDesktopOnly)
                Exit Sub
            End If
            'Build a file and tbalename from the week
            'printing out the month name so we don't get confused with the places that do it backwards
            '
            Dim sTableName As String = frmMain.colZones.CurrentZone & " " & _
                            dEndTime.Date.ToString("MMMM_dd_yyyy")
            Dim sFileName As String = frmMain.colZones.CurrentZone & " - " & _
                            gpsRM.GetString("psWEEKLY_REPORT_FILENAME") & " - " & _
                            dEndTime.Date.ToString("MMMM-dd-yyyy")
            sTarget = sTargetPath & sFileName
            'Delete the file if it's already there
            Try
                'MSW 5/16/14 Excel File Type Experiment

                For Each sFile As String In My.Computer.FileSystem.GetFiles(sTargetPath, FileIO.SearchOption.SearchTopLevelOnly, sFileName & ".XL*")
                    My.Computer.FileSystem.DeleteFile(sFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin, FileIO.UICancelOption.DoNothing)
                Next
                If My.Computer.FileSystem.FileExists(sTarget) Then
                    My.Computer.FileSystem.DeleteFile(sTarget, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin, FileIO.UICancelOption.DoNothing)
                End If
            Catch ex As Exception

                ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msMODULE_NAME, _
                                    frmMain.Status, MessageBoxButtons.OK)
                Exit Sub
            End Try
            '********************************************************************************************
            'Start excel up and create a file (workbook)
            'Default creates 3 sheets.  I'm sure that could change if we left that assumption, so any extras
            'are getting deleted and if we're short, make sure they get added
            '********************************************************************************************
            oXl = DirectCast(CreateObject("Excel.Application"), Excel.Application)
            oWb = oXl.Workbooks.Add()
            oXl.Visible = mPWCommon.InIDE
            'MSW 5/16/14 Excel File Type Experiment
            'oWb.SaveAs(Filename:=sTarget, FileFormat:=Excel.XlFileFormat.xlOpenXMLWorkbook, _
            '           AccessMode:=Excel.XlSaveAsAccessMode.xlExclusive, _
            '           ConflictResolution:=Excel.XlSaveConflictResolution.xlOtherSessionChanges)
            ' 1 chart sheet+ 2 weeks *  (alarmslist + duration summary + Occurrence summary)
            ' + 2 for total summaries
            Dim nNumSheets As Integer = 10
            nChartNumber = 1
            Do While oWb.Worksheets.Count > nNumSheets
                Dim oSheet As Excel.Worksheet = DirectCast(oWb.Worksheets.Item(3), Excel.Worksheet)
                oSheet.Delete()
                oSheet = Nothing
            Loop
            Do While oWb.Worksheets.Count < nNumSheets
                oWb.Worksheets.Add(Count:=1, Type:=Excel.XlSheetType.xlWorksheet)
            Loop


            '********************************************************************************************
            'setup the chart sheet
            'setup chart config for the location of the first chart
            '********************************************************************************************
            ChartConfig.ChartSheet = gpsRM.GetString("psGRAPH_SHEET_NAME")
            oChartSheet = DirectCast(oWb.Worksheets.Item(1), Excel.Worksheet)

            oChartSheet.Name = ChartConfig.ChartSheet

            'Chart config setup - location for the first one, sizes will all be the same
            ChartConfig.TableRow = 1
            ChartConfig.TableCol = 1
            ChartConfig.ChartLeft = 1
            ChartConfig.ChartWidth = 400
            ChartConfig.ChartHeight = 200
            ChartConfig.TopxxReq = 5
            ChartConfig.Title = String.Empty
            ChartConfig.TableSheet = String.Empty
            ChartConfig.ChartSheet = String.Empty
            ChartConfig.ChartType = Excel.XlChartType.xlBarClustered

            Const nTHIS_WEEK As Integer = 1
            Const nLAST_WEEK As Integer = 2
            Dim DTsumm As DataTable = Nothing
            Dim sTitle As String = String.Empty
            Dim nSheetNum As Integer = nNumSheets
            Dim sWeekName As String = String.Empty
            Dim sWeekTitlePrefix As String = String.Empty
            Dim sWeekLabelPrefix As String = String.Empty
            Dim nCol As Integer = 0
            Dim nRow As Integer = 0

            oCell1 = oChartSheet.Cells(1, 1)
            oCell2 = oChartSheet.Cells(50, 4)
            oRange = DirectCast(oChartSheet.Range(oCell1, oCell2), Excel.Range)
            oRange.ColumnWidth = 10
            oCell1 = oChartSheet.Cells(1, 10)
            oCell2 = oChartSheet.Cells(50, 13)
            oRange = DirectCast(oChartSheet.Range(oCell1, oCell2), Excel.Range)
            oRange.ColumnWidth = 10

            '*******************************************************************************
            'Two Month Chart
            Call OccurrenceDuration(oXl, oWb, oChartSheet, ChartConfig)

            nSheetNum = 3

            ''********************************************************************************************
            ''Put the chart in sheet 1
            ''********************************************************************************************

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

            '********************************************************************************************
            'Using alarm log data, we loop through this week and last week
            '********************************************************************************************

            For nWeek As Integer = nTHIS_WEEK To nLAST_WEEK
                Select Case nWeek
                    Case nTHIS_WEEK 'This week, already setup the dates
                        nSheetNum = 5   'Combined alarm table
                        sWeekName = gpsRM.GetString("psTHIS_WEEK")
                        sWeekLabelPrefix = gpsRM.GetString("psTHISWEEK")
                        sWeekTitlePrefix = gpsRM.GetString("psTHIS_WEEK_TITLE_PRE")
                        nCol = 1
                        ChartConfig.StartTime = dStartTime
                        ChartConfig.EndTime = dEndTime
                    Case nLAST_WEEK 'Last week, adjust the dates
                        nSheetNum = 8   'Combined alarm table

                        sWeekName = gpsRM.GetString("psLAST_WEEK")
                        sWeekLabelPrefix = gpsRM.GetString("psLASTWEEK")
                        sWeekTitlePrefix = gpsRM.GetString("psLAST_WEEK_TITLE_PRE")
                        ChartConfig.StartTime = DateAdd(DateInterval.WeekOfYear, -1, dStartTime)
                        ChartConfig.EndTime = DateAdd(DateInterval.WeekOfYear, -1, dEndTime)
                        nCol = 10
                End Select
                'frmCriteria manages the db access, so we need to set its date even though it's invisible
                frmCriteria.EndTime = ChartConfig.EndTime
                frmCriteria.StartTime = ChartConfig.StartTime
                frmCriteria.bGetDataSet()
                DT1 = frmMain.dtGetDowntimeData(gsALARM_DS_TABLENAME, False)

                '********************************************************************************************
                'set up config data to pass through all the chart subroutines 
                '********************************************************************************************
                nRow = 16
                '********************************************************************************************
                'Fill in the alarm table on sheet nNumSheets
                'Basically another save or print routine, but speciallized for DT summaries
                '********************************************************************************************
                ChartConfig.TableSheet = sWeekLabelPrefix & gpsRM.GetString("psALARMS_SHEET_NAME")
                ChartConfig.ChartSheet = ChartConfig.TableSheet

                oTableSheet = DirectCast(oWb.Worksheets.Item(nSheetNum), Excel.Worksheet)
                oTableSheet.Name = ChartConfig.TableSheet
                'Add in some more details for the title section before the table

                oCell1 = oChartSheet.Cells(nRow, nCol)
                oRange = DirectCast(oChartSheet.Range(oCell1, oCell1), Excel.Range)
                'configure some info for putting the data in the excel file
                ChartConfig.ChartTop = CInt(oRange.Height) + CInt(oRange.Top)
                ChartConfig.ChartLeft = CInt(oRange.Left)

                '********************************************************************************************
                'Each week's alarms get listed in one of the sheets
                '********************************************************************************************
                subBuildDTAlarmSheetFromDataSet(oXl, oWb, oTableSheet, DT1, ChartConfig)

                If nWeek = nTHIS_WEEK Then
                    '********************************************************************************************
                    'save current week to WeeklyReports DB
                    '********************************************************************************************
                    ChartConfig.TopxxReq = 0
                    subGetChartData(DT1, DTsumm, sTitle, ChartConfig)
                    'Also deletes old data and returns a list of which summaries are available
                    DTList = dtSaveWeeklyDataToDB(DTsumm, sTableName, ChartConfig)
                    ChartConfig.TopxxReq = 5
                End If
                Const nBY_DUR As Integer = 1
                Const nBY_OCCUR As Integer = 2
                '********************************************************************************************
                'Each week gets 2 charts - summary by occurrence and summary by duration
                'Each chart also gets a summary table sheet
                '********************************************************************************************
                For nTable As Integer = nBY_DUR To nBY_OCCUR
                    Select Case nTable
                        Case nBY_DUR 'duration
                            ChartConfig.ByOccurrence = False
                            ChartConfig.TableSheet = sWeekLabelPrefix & gpsRM.GetString("psDURATION_SUMMARY_SHEET_NAME")
                            nSheetNum -= 2 '2 sheets before the alarm table
                        Case 2 ' Occurrence
                            ChartConfig.ByOccurrence = True
                            'If it's doing both, shift it down
                            nSheetNum += 1 'nextr sheet
                            ChartConfig.ChartTop = ChartConfig.ChartTop + ChartConfig.ChartHeight + 15
                            ChartConfig.TableSheet = sWeekLabelPrefix & gpsRM.GetString("psOCCURRENCE_SUMMARY_SHEET_NAME")
                    End Select
                    oTableSheet = DirectCast(oWb.Worksheets.Item(nSheetNum), Excel.Worksheet)
                    oTableSheet.Name = ChartConfig.TableSheet
                    '********************************************************************************************
                    'Load summary data and titles
                    '********************************************************************************************
                    subGetChartData(DT1, DTsumm, sTitle, ChartConfig)
                    Application.DoEvents()

                    '********************************************************************************************
                    'Fill in the summary table on a seperate sheet
                    'Basically another save or print routine, but speciallized for DT summaries
                    '********************************************************************************************
                    subBuildDTSummarySheetFromDataSet(oXl, oWb, oTableSheet, DTsumm, sTitle, ChartConfig)

                    '********************************************************************************************
                    'Put the chart in sheet 1
                    '********************************************************************************************

                    'Manual charts use a the long detailed title text
                    ChartConfig.Title = sWeekTitlePrefix & sTitle
                    'oChartSheet.Name = sWeekLabelPrefix & gpsRM.GetString("psCHART")
                    ChartConfig.ChartSheet = gpsRM.GetString("psGRAPH_SHEET_NAME")
                    oChartSheet = DirectCast(oWb.Worksheets.Item(1), Excel.Worksheet)

                    subBuildChart(oXl, oWb, oTableSheet, oChartSheet, ChartConfig)
                    Application.DoEvents()
                    nChartNumber = nChartNumber + 1
                    subMoveChart(oXl, oWb, oChartSheet, nChartNumber, False)
                Next 'nTable
                '********************************************************************************************
                'add some details to the chart worksheet
                '********************************************************************************************
                nRow = 20
                'subtitles
                oChartSheet.Cells(nRow, nCol) = sWeekName
                nRow = 21
                'Times
                oChartSheet.Cells(nRow, nCol) = gpsRM.GetString("psSTART") & ": "
                oChartSheet.Cells(nRow, nCol + 1) = ChartConfig.StartTime.Date.ToString("d")
                oChartSheet.Cells(nRow, nCol + 2) = gpsRM.GetString("psEND") & ": "
                oChartSheet.Cells(nRow, nCol + 3) = ChartConfig.EndTime.Date.ToString("d")
                'Stats
                oChartSheet.Cells(nRow + 1, nCol) = gpsRM.GetString("psUPTIME") & ": "
                oChartSheet.Cells(nRow + 1, nCol + 1) = ChartConfig.UptimePct.ToString("00.0") & "%"
                oChartSheet.Cells(nRow + 2, nCol) = gpsRM.GetString("psMTBF") & ": "
                oChartSheet.Cells(nRow + 2, nCol + 1) = " " & frmMain.sGetDurationString(ChartConfig.MTBF)
                oChartSheet.Cells(nRow + 2, nCol + 2) = gpsRM.GetString("psMTTR") & ": "
                oChartSheet.Cells(nRow + 2, nCol + 3) = " " & frmMain.sGetDurationString(ChartConfig.MTTR)

                'Left align the rows
                oCell1 = oChartSheet.Cells(nRow, nCol)
                oCell2 = oChartSheet.Cells(nRow + 2, nCol + 3)
                oRange = DirectCast(oChartSheet.Range(oCell1, oCell2), Excel.Range)
                oRange.HorizontalAlignment = Excel.Constants.xlLeft
                'This flattens out the graph labels.  We'll do it the ugly way instead
                'Manually set the widths before we build the charts
                'oRange.Columns.AutoFit()
                'Then right align the labels so they meet up with the data
                oCell1 = oChartSheet.Cells(nRow, nCol)
                oCell2 = oChartSheet.Cells(nRow + 2, nCol)
                oRange = DirectCast(oChartSheet.Range(oCell1, oCell2), Excel.Range)
                oRange.HorizontalAlignment = Excel.Constants.xlRight
                oCell1 = oChartSheet.Cells(nRow, nCol + 2)
                oCell2 = oChartSheet.Cells(nRow + 2, nCol + 2)
                oRange = DirectCast(oChartSheet.Range(oCell1, oCell2), Excel.Range)
                oRange.HorizontalAlignment = Excel.Constants.xlRight


            Next 'nWeek


            '********************************************************************************************
            'Done with current alarm log data, now we go to the the yearly data
            'DTlist has the tablelist database from weekly reports already.
            'first thing to do is sort it.
            '********************************************************************************************
            'select routine returns an array of datarows sorted by date, index 0 is the oldest
            Dim drTableList() As DataRow = DTList.Select(String.Empty, gsWEEKLY_LIST_COL_DATE, DataViewRowState.CurrentRows)
            Debug.Print("DTList after " & DTList.Rows.Count.ToString)
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
            Dim sYearlyTargetPath As String = String.Empty
            If Not (mPWCommon.GetDefaultFilePath(sYearlyTargetPath, eDir.WeeklyReports, String.Empty, String.Empty)) Then
                'Can't get directory
                MessageBox.Show(gpsRM.GetString("psWEEKLY_REP_DIR_ERR"), frmMain.msSCREEN_NAME, MessageBoxButtons.OK, _
                 MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, _
                MessageBoxOptions.DefaultDesktopOnly)
                Exit Sub
            End If
            'Build a file and tablename from the week
            'printing out the month name so we don't get confused with the places that do it backwards
            '
            Dim sYearlyFileName As String = frmMain.colZones.CurrentZone & " " & _
                            gpsRM.GetString("psYEARLY_DATA_FILENAME")
            sYearlyTarget = sYearlyTargetPath & sYearlyFileName
            'Delete the file if it's already there
            Try
                'MSW 5/16/14 Excel File Type Experiment
                For Each sFile As String In My.Computer.FileSystem.GetFiles(sYearlyTargetPath, FileIO.SearchOption.SearchTopLevelOnly, sYearlyFileName & ".XL*")
                    My.Computer.FileSystem.DeleteFile(sFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin, FileIO.UICancelOption.DoNothing)
                Next

                If My.Computer.FileSystem.FileExists(sYearlyTarget) Then
                    My.Computer.FileSystem.DeleteFile(sYearlyTarget, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin, FileIO.UICancelOption.DoNothing)
                End If

            Catch ex As Exception

                ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msMODULE_NAME, _
                                    frmMain.Status, MessageBoxButtons.OK)
                Exit Sub
            End Try
            oWbYearly = oXl.Workbooks.Add()
            oXl.Visible = mPWCommon.InIDE
            'oWbYearly.SaveAs(Filename:=sYearlyTarget, FileFormat:=Excel.XlFileFormat.xlOpenXMLWorkbook, _
            '           AccessMode:=Excel.XlSaveAsAccessMode.xlExclusive, _
            '           ConflictResolution:=Excel.XlSaveConflictResolution.xlOtherSessionChanges)
            ' 1 chart sheet+ 2 weeks *  (alarmslist + duration summary + Occurrence summary)
            ' + 2 for total summaries
            Dim nNumYrlySheets As Integer = 0
            Dim nYearlyTableRow As Integer = 1
            Dim nYearlyStatsRow As Integer = 1

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
            'The list already cleared out old data, so the table summaries will get 
            'everything, but the chart can have a different date range 
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

            ChartConfig.TopxxReq = 5

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
                Debug.Print(sName & " " & dTableDate.ToString & " MTBF:" & _
                            " " & frmMain.sGetDurationString(nMTBF) & " MTTR:" & _
                            " " & frmMain.sGetDurationString(nMTTR) & " Uptime:" & _
                            (fUptime / 100).ToString("00.0%"))
                '********************************************************************************************
                'Get the summary table for 1 week
                '********************************************************************************************
                DT_Week = dtGetWeeklyDataFromDB(sName)

                'Get the top 5s for duration and occurrence
                subGetTopxTotals(DT_Week, DR_Occurrence, DR_Duration, ChartConfig.TopxxReq)

                '********************************************************************************************
                'Add up summary totals for the year
                '********************************************************************************************
                If DT_Occurrence_Year Is Nothing Then
                    '1st week, just clone the table config
                    DT_Occurrence_Year = DT_Week.Clone
                    DT_Occurrence_Year.Columns.Remove(gpsRM.GetString("psOCCURRENCES_PERC_CAP"))
                    DT_Occurrence_Year.Columns.Remove(gpsRM.GetString("psDURATION_PERC_CAP"))
                    DT_Occurrence_Year.Columns.Remove(gpsRM.GetString("psOCCURRENCES_CAP"))
                    DT_Occurrence_Year.Columns.Remove(gpsRM.GetString("psDURATION_CAP"))
                    DT_Occurrence_Year.Columns.Remove(gsDTMASK_COL_ID)
                    'End If
                    'If DT_Duration_Year Is Nothing Then
                    '1st week, just clone the table config
                    DT_Duration_Year = DT_Week.Clone
                    DT_Duration_Year.Columns.Remove(gpsRM.GetString("psOCCURRENCES_PERC_CAP"))
                    DT_Duration_Year.Columns.Remove(gpsRM.GetString("psDURATION_PERC_CAP"))
                    DT_Duration_Year.Columns.Remove(gpsRM.GetString("psDURATION_CAP"))
                    DT_Duration_Year.Columns.Remove(gpsRM.GetString("psOCCURRENCES_CAP"))
                    DT_Duration_Year.Columns.Remove(gsDTMASK_COL_ID)
                    For nTable2 As Integer = 0 To drTableList.GetUpperBound(0)
                        Dim DR2 As DataRow = drTableList(nTable2)
                        Dim dWeekDate2 As Date = CDate(DR2.Item(gsWEEKLY_LIST_COL_DATE))
                        Dim sWeeklyCol2(1) As String
                        sWeeklyCol2(0) = "W" & dWeekDate2.ToString("MM_dd_yyyy")
                        sWeeklyCol2(1) = dWeekDate2.ToString("d")
                        DT_Occurrence_Year.Columns.Add(sWeeklyCol2(0))
                        DT_Occurrence_Year.Columns(sWeeklyCol2(0)).Caption = sWeeklyCol2(1)
                        DT_Duration_Year.Columns.Add(sWeeklyCol2(0))
                        DT_Duration_Year.Columns(sWeeklyCol2(0)).Caption = sWeeklyCol2(1)
                    Next
                    DT_Occurrence_Year.Columns.Add(gpsRM.GetString("psOCCURRENCES_CAP"))
                    DT_Occurrence_Year.Columns(gpsRM.GetString("psOCCURRENCES_CAP")).Caption = gpsRM.GetString("psTOTAL_CAP")
                    DT_Duration_Year.Columns.Add(gpsRM.GetString("psDURATION_CAP"))
                    DT_Duration_Year.Columns(gpsRM.GetString("psDURATION_CAP")).Caption = gpsRM.GetString("psTOTAL_CAP")
                    DT_Occurrence_Year.Columns.Add(gpsRM.GetString("psPERCENT_CAP"))
                    DT_Occurrence_Year.Columns(gpsRM.GetString("psPERCENT_CAP")).Caption = gpsRM.GetString("psPERCENT_CAP")
                    DT_Duration_Year.Columns.Add(gpsRM.GetString("psPERCENT_CAP"))
                    DT_Duration_Year.Columns(gpsRM.GetString("psPERCENT_CAP")).Caption = gpsRM.GetString("psPERCENT_CAP")
                End If
                Dim sWeeklyCol(1) As String
                sWeeklyCol(0) = "W" & dTableDate.ToString("MM_dd_yyyy")
                sWeeklyCol(1) = dTableDate.ToString("d")
                'and add to the total datatable
                subAddArrayToTotalsDT(DR_Occurrence, DT_Occurrence_Year, True, False, sWeeklyCol(0))
                'and add to the total datatable
                subAddArrayToTotalsDT(DR_Duration, DT_Duration_Year, False, True, sWeeklyCol(0))
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
                'Check the year, each year get's a new sheet for the mini summary tables
                '********************************************************************************************
                If nYear <> dTableDate.Year Then
                    If Not (oYearlySheet Is Nothing) Then
                        subFinishYearlysheet(oXl, oWb, oYearlySheet, nYear, nYearlyTableRow, DT_Occurrence_Year, DT_Duration_Year, _
                                             oYearlyStatsSheet, nYearlyStatsRow)
                    End If
                    nYear = dTableDate.Year
                    nNumYrlySheets += 2
                    Do While oWbYearly.Worksheets.Count > nNumYrlySheets
                        Dim oSheet As Excel.Worksheet = DirectCast(oWbYearly.Worksheets.Item(oWbYearly.Worksheets.Count), Excel.Worksheet)
                        oSheet.Delete()
                        oSheet = Nothing
                    Loop

                    If oWbYearly.Worksheets.Count < nNumYrlySheets Then
                        If Not (oYearlySheet Is Nothing) Then
                            'Add the new sheet after the last one
                            oWbYearly.Worksheets.Add(After:=oYearlySheet, Count:=1, Type:=Excel.XlSheetType.xlWorksheet)
                        Else
                            oWbYearly.Worksheets.Add(Count:=1, Type:=Excel.XlSheetType.xlWorksheet)
                        End If
                    End If
                    oYearlySheet = DirectCast(oWbYearly.Worksheets.Item(nNumYrlySheets - 1), Excel.Worksheet)
                    oYearlySheet.Name = nYear.ToString

                    nYearlyTableRow = 1
                    oYearlySheet.Cells(nYearlyTableRow, 1) = frmMain.colZones.CurrentZone & " " & _
                        gpsRM.GetString("psWEEKLY_TOP") & ChartConfig.TopxxReq.ToString & " " & _
                        gpsRM.GetString("psSUMM_FOR") & nYear.ToString

                    'Left align the rows
                    oCell1 = oYearlySheet.Cells(nYearlyTableRow, 1)
                    oCell2 = oYearlySheet.Cells(nYearlyTableRow, 10)
                    oRange = DirectCast(oYearlySheet.Range(oCell1, oCell2), Excel.Range)
                    oRange.Font.Bold = True
                    nYearlyTableRow += 2

                    nYearlyStatsRow = 1
                    oYearlyStatsSheet = DirectCast(oWbYearly.Worksheets.Item(nNumYrlySheets), Excel.Worksheet)
                    oYearlyStatsSheet.Name = nYear.ToString & " Stats"
                    oYearlyStatsSheet.Cells(nYearlyStatsRow, 2) = gpsRM.GetString("psMTTR")
                    oYearlyStatsSheet.Cells(nYearlyStatsRow, 3) = gpsRM.GetString("psMTBF")
                    oYearlyStatsSheet.Cells(nYearlyStatsRow, 4) = gpsRM.GetString("psUPTIME")
                    oCell1 = oYearlyStatsSheet.Cells(nYearlyStatsRow, 1)
                    oCell2 = oYearlyStatsSheet.Cells(nYearlyStatsRow, 4)
                    oRange = DirectCast(oYearlyStatsSheet.Range(oCell1, oCell2), Excel.Range)
                    oRange.Font.Bold = True
                    nYearlyStatsRow += 1
                End If
                subWriteWeeklyTableToYearlySheet(oYearlySheet, nYearlyTableRow, DR_Occurrence, DR_Duration, _
                                        dTableDate, nMTBF, nMTTR, fUptime)
                oYearlyStatsSheet.Cells(nYearlyStatsRow, 1) = dTableDate.Date.ToShortDateString
                oYearlyStatsSheet.Cells(nYearlyStatsRow, 2) = " " & frmMain.sGetDurationString(nMTTR)
                oYearlyStatsSheet.Cells(nYearlyStatsRow, 3) = " " & frmMain.sGetDurationString(nMTBF)
                oYearlyStatsSheet.Cells(nYearlyStatsRow, 4) = fUptime / 100
                nYearlyStatsRow += 1
            Next
            'Clean up the last year sheet
            If Not (oYearlySheet Is Nothing) Then
                subFinishYearlysheet(oXl, oWb, oYearlySheet, nYear, nYearlyTableRow, DT_Occurrence_Year, DT_Duration_Year, oYearlyStatsSheet, nYearlyStatsRow)
            End If

            '********************************************************************************************
            'Print out the summary for the totals chart
            '********************************************************************************************
            If Not (DT_Duration_Totals Is Nothing) And Not (DT_Occurrence_Totals Is Nothing) Then
                'common chart and table config - location, size
                ChartConfig.TableRow = 1
                ChartConfig.TableCol = 1
                'Use the dimensions from the last (lower right) chart 
                'configure some info for putting the data in the excel file
                'Start by getting the location of the last chart
                'oChartSheet.Select()
                Dim oChartObjects As Excel.ChartObjects = DirectCast(oChartSheet.ChartObjects, Excel.ChartObjects)
                Dim oChartObject As Excel.ChartObject

                oChartObject = DirectCast(oChartObjects.Item(oChartObjects.Count), Excel.ChartObject)
                'position it 
                ChartConfig.ChartWidth = CInt(oChartObject.Width) + CInt(oChartObject.Left)
                ChartConfig.ChartTop = CInt(oChartObject.Top) + CInt(oChartObject.Height) + 15
                'New height and width
                ChartConfig.ChartLeft = 1
                ChartConfig.ChartHeight = 450
                ChartConfig.TopxxReq = 5
                ChartConfig.StartTime = dChartCutoffDate
                ChartConfig.EndTime = dEndTime
                'Switch to columns
                ChartConfig.ChartType = Excel.XlChartType.xlColumnClustered
                'Duration summary
                ChartConfig.ByOccurrence = False
                ChartConfig.TableSheet = gpsRM.GetString("psTOTAL_CAP") & gpsRM.GetString("psDURATION_SUMMARY_SHEET_NAME")
                oTableSheet = DirectCast(oWb.Worksheets.Item(nNumSheets), Excel.Worksheet)
                oTableSheet.Name = ChartConfig.TableSheet
                Dim sSummType As String = gpsRM.GetString("psDURATION_CAP")
                'Add in some more details for the title section before the table
                sTitle = gpsRM.GetString("psTOT_EACH_WEEK") & gpsRM.GetString("psTOP_X") & ChartConfig.TopxxReq.ToString & " " & _
                        gpsRM.GetString("psPIE_TITLE_DT") & " " & sSummType
                ChartConfig.Title = sTitle
                Dim drDurationTotals() As DataRow = DT_Duration_Totals.Select(String.Empty, gpsRM.GetString("psDURATION_CAP"), DataViewRowState.CurrentRows)
                'Note this routine that takes datarow arrays is a little different from the ones that get datatables passed in
                subBuildDTSummarySheetFromDataSet(oXl, oWb, oTableSheet, DT_Duration_Totals, drDurationTotals, sTitle, ChartConfig, False)
                ChartConfig.Title = sTitle & Chr(13) & gpsRM.GetString("psFROM") & " " & _
                    ChartConfig.StartTime.ToShortDateString & " " & gpsRM.GetString("psTO") & " " & _
                    ChartConfig.EndTime.ToShortDateString()
                subBuildChart(oXl, oWb, oTableSheet, oChartSheet, ChartConfig)

                'Occurrence summary
                ChartConfig.ByOccurrence = True
                ChartConfig.TableSheet = gpsRM.GetString("psTOTAL_CAP") & gpsRM.GetString("psOCCURRENCE_SUMMARY_SHEET_NAME")
                oTableSheet = DirectCast(oWb.Worksheets.Item(nNumSheets - 1), Excel.Worksheet)
                oTableSheet.Name = ChartConfig.TableSheet
                ChartConfig.ChartTop = ChartConfig.ChartTop + ChartConfig.ChartHeight + 15
                sSummType = gpsRM.GetString("psOCCURRENCES_CAP")
                sTitle = gpsRM.GetString("psTOT_EACH_WEEK") & gpsRM.GetString("psTOP_X") & ChartConfig.TopxxReq.ToString & " " & _
                        gpsRM.GetString("psPIE_TITLE_DT") & " " & sSummType
                ChartConfig.Title = sTitle
                Dim drOccurrenceTotals() As DataRow = DT_Occurrence_Totals.Select(String.Empty, gpsRM.GetString("psOCCURRENCES_CAP"), DataViewRowState.CurrentRows)
                'Note this routine that takes datarow arrays is a little different from the ones that get datatables passed in
                subBuildDTSummarySheetFromDataSet(oXl, oWb, oTableSheet, DT_Occurrence_Totals, drOccurrenceTotals, sTitle, ChartConfig, False)

                ChartConfig.Title = sTitle & Chr(13) & gpsRM.GetString("psFROM") & " " & _
                    ChartConfig.StartTime.ToShortDateString & " " & gpsRM.GetString("psTO") & " " & _
                    ChartConfig.EndTime.ToShortDateString()
                subBuildChart(oXl, oWb, oTableSheet, oChartSheet, ChartConfig)

            End If
            Application.DoEvents()
            'If (mPWCommon.GetDefaultFilePath(sTarget, eDir.WeeklyReports, String.Empty, String.Empty)) Then
            '    oXl.SaveWorkspace(sTarget & sFileName & ".XLW")
            'End If
            'MSW 5/16/14 Excel File Type Experiment
#If EXCEL12 Then
            oWb.SaveAs(Filename:=sTarget, FileFormat:=Excel.XlFileFormat.xlOpenXMLWorkbook, _
                       AccessMode:=Excel.XlSaveAsAccessMode.xlExclusive, _
                       ConflictResolution:=Excel.XlSaveConflictResolution.xlOtherSessionChanges)
            oWbYearly.SaveAs(Filename:=sYearlyTarget, FileFormat:=Excel.XlFileFormat.xlOpenXMLWorkbook, _
                       AccessMode:=Excel.XlSaveAsAccessMode.xlExclusive, _
                       ConflictResolution:=Excel.XlSaveConflictResolution.xlOtherSessionChanges)
#Else
            oWb.SaveAs(Filename:=sTarget, FileFormat:=Excel.XlFileFormat.xlWorkbookNormal, _
                       AccessMode:=Excel.XlSaveAsAccessMode.xlExclusive, _
                       ConflictResolution:=Excel.XlSaveConflictResolution.xlOtherSessionChanges)
            oWbYearly.SaveAs(Filename:=sYearlyTarget, FileFormat:=Excel.XlFileFormat.xlWorkbookNormal, _
                       AccessMode:=Excel.XlSaveAsAccessMode.xlExclusive, _
                       ConflictResolution:=Excel.XlSaveConflictResolution.xlOtherSessionChanges)
#End If
            If Not InIDE Then
                oWb.Close(Excel.XlSaveAction.xlSaveChanges)
                oWbYearly.Close(Excel.XlSaveAction.xlSaveChanges)

                Application.DoEvents()

                oXl.Application.Quit()
                oXl.Quit()
                oTableSheet = Nothing
                oChartSheet = Nothing
                oYearlySheet = Nothing
                oWb = Nothing
                oWbYearly = Nothing
                oXl = Nothing

                Application.DoEvents()
            End If
            frmMain.Progress = 100
        Catch ex As Exception
            Application.DoEvents()
            Dim lReply As Windows.Forms.DialogResult = Response.OK
            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msMODULE_NAME, _
            frmMain.Status, MessageBoxButtons.OK)
            oWb.Close(Excel.XlSaveAction.xlSaveChanges)

            oXl.Application.Quit()
            oXl.Quit()
            oTableSheet = Nothing
            oChartSheet = Nothing
            oWb = Nothing
            oXl = Nothing
            Application.DoEvents()
        End Try
        frmMain.Cursor = System.Windows.Forms.Cursors.Default


    End Sub
    Private Sub OccurrenceDuration(ByRef oXl As Excel.Application, _
                                ByRef oWB As Excel.Workbook, _
                                ByRef oChartSheet As Excel.Worksheet, _
                                ByRef ChartConfig As tChartConfig, Optional ByRef dDate As Date = Nothing)
        '********************************************************************************************
        'Description: Two Month Chart
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim dEndTime As Date
        Dim dStartTime As Date
        Dim DT1 As DataTable
        Dim DTList As DataTable = Nothing
        Dim msChartName As String
        Dim oRange As Excel.Range
        Dim oCell1 As Object
        Dim oCell2 As Object
        Dim DTsumm As DataTable = Nothing
        Dim sTitle As String = String.Empty
        Dim nSheetNum As Integer
        Dim sWeekName As String = String.Empty
        Dim sWeekTitlePrefix As String = String.Empty
        Dim sWeekLabelPrefix As String = String.Empty
        Dim nCol As Integer = 0
        Dim nRow As Integer = 0
        Dim oTableSheet As Excel.Worksheet = Nothing
        Dim sSourceString As String
        Dim sDestString As String
        Dim sPivotTable As String
        Dim oChart As Excel.Chart
        Dim sRange As String
        Dim sTitle1 As String
        Dim lLastRow As Long
        Dim nLastCol As Integer
        Dim xlAxisCategory As Excel.Axes
        Dim xlSeries As Excel.SeriesCollection


        '**********************************
        'Optional date settings
        If (dDate = Nothing) Then
            dEndTime = Now
            'We're expecting this around midnight saturday.  If it's close, round it off
            If dEndTime.DayOfWeek = DayOfWeek.Sunday Then
                dEndTime = DateAdd(DateInterval.Second, -1, dEndTime.Date)
            ElseIf dEndTime.DayOfWeek = DayOfWeek.Saturday Then
                dEndTime = DateAdd(DateInterval.Day, 1, dEndTime.Date)
                dEndTime = DateAdd(DateInterval.Second, -1, dEndTime.Date)
            Else
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
        dStartTime = DateAdd(DateInterval.Month, -2, dEndTime)
        dStartTime = DateAdd(DateInterval.Second, 1, dStartTime)


        Dim sTableName As String = frmMain.colZones.CurrentZone & " " & _
                            dEndTime.Date.ToString("MMMM_dd_yyyy")

        'Title row
        oChartSheet.Cells(1, 1) = frmMain.colZones.SiteName & ": " & frmMain.colZones.CurrentZone & "   " & _
            gpsRM.GetString("psPAST_MON_OCR_DUR") & " " & dEndTime.ToShortDateString
        'Bold for the first row of titles
        oCell1 = oChartSheet.Cells(1, 1)
        oRange = DirectCast(oChartSheet.Range(oCell1, oCell1), Excel.Range)
        oRange.Font.Bold = True

        frmMain.subGetDTMask()
        frmCriteria.subSetupChartCriteria()

        'Chart config setup - location for the first one, sizes will all be the same
        ChartConfig.TableRow = 1
        ChartConfig.TableCol = 1
        ChartConfig.ChartLeft = 1
        ChartConfig.ChartWidth = 400
        ChartConfig.ChartHeight = 200
        ChartConfig.TopxxReq = 5
        ChartConfig.Title = String.Empty
        ChartConfig.TableSheet = String.Empty
        ChartConfig.ChartSheet = String.Empty
        ChartConfig.ChartType = Excel.XlChartType.xlBarClustered


        '*******************************************************************************
        'Two Month Chart
        '*******************************************************************************

        '********************************************************************************************
        'setup the chart sheet
        'setup chart config for the location of the first chart
        '********************************************************************************************
        ChartConfig.ChartSheet = gpsRM.GetString("psPAST_MON") & gpsRM.GetString("psALARMS_SHEET_NAME")
        oChartSheet = DirectCast(oWB.Worksheets.Item(2), Excel.Worksheet)



        oChartSheet.Name = ChartConfig.ChartSheet
        'Title row
        oChartSheet.Cells(1, 1) = frmMain.colZones.SiteName & ": " & frmMain.colZones.CurrentZone & "   " & _
            gpsRM.GetString("psPAST_MON") & " " & dEndTime.ToShortDateString
        'Bold for the first row of titles
        oCell1 = oChartSheet.Cells(1, 1)
        oRange = DirectCast(oChartSheet.Range(oCell1, oCell1), Excel.Range)
        oRange.Font.Bold = True


        nSheetNum = 2   'Combined alarm table
        sWeekName = gpsRM.GetString("psPAST_MON")
        sWeekLabelPrefix = gpsRM.GetString("psPAST_MON")
        sWeekTitlePrefix = gpsRM.GetString("psPAST_MON")
        nCol = 1
        ChartConfig.StartTime = dStartTime
        ChartConfig.EndTime = dEndTime

        'frmCriteria manages the db access, so we need to set its date even though it's invisible
        frmCriteria.EndTime = ChartConfig.EndTime
        frmCriteria.StartTime = ChartConfig.StartTime
        frmCriteria.bGetDataSet()
        DT1 = frmMain.dtGetDowntimeData(gsALARM_DS_TABLENAME, False)

        '********************************************************************************************
        'set up config data to pass through all the chart subroutines 
        '********************************************************************************************

        nRow = 5
        oCell1 = oChartSheet.Cells(nRow, nCol)
        oRange = DirectCast(oChartSheet.Range(oCell1, oCell1), Excel.Range)
        'configure some info for putting the data in the excel file
        ChartConfig.ChartTop = CInt(oRange.Height) + CInt(oRange.Top)
        ChartConfig.ChartLeft = CInt(oRange.Left)
        '********************************************************************************************
        'Fill in the alarm table on sheet nNumSheets
        'Basically another save or print routine, but speciallized for DT summaries
        '********************************************************************************************
        ChartConfig.TableSheet = sWeekLabelPrefix & gpsRM.GetString("psALARMS_SHEET_NAME")
        oTableSheet = DirectCast(oWB.Worksheets.Item(nSheetNum), Excel.Worksheet)
        'Add in some more details for the title section before the table

        '********************************************************************************************
        'Each week's alarms get listed in one of the sheets
        '********************************************************************************************

        subBuildDTAlarmSheetFromDataSet(oXl, oWB, oTableSheet, DT1, ChartConfig)

        Application.DoEvents()

        lLastRow = oChartSheet.UsedRange.Rows.Count
        nLastCol = oChartSheet.UsedRange.Columns.Count
        Debug.Print(ChartConfig.Title)
        sSourceString = sWeekLabelPrefix & gpsRM.GetString("psALARMS_SHEET_NAME") & "!R3C1:R" & lLastRow & "C" & nLastCol
        Debug.Print(sSourceString)
        sDestString = sWeekLabelPrefix & gpsRM.GetString("psALARMS_SHEET_NAME") & "!R1C" & (nLastCol + 9)   '"Sheet1!R1C" & (nLastCol + 4)
        Debug.Print(sDestString)
        sPivotTable = "PivotTable" & 1


        oWB.PivotCaches.Add(SourceType:=Excel.XlPivotTableSourceType.xlDatabase, _
                            SourceData:=sSourceString).CreatePivotTable( _
                            TableDestination:=sDestString, _
                            TableName:=sPivotTable, _
                            DefaultVersion:=Excel.XlPivotTableVersionList.xlPivotTableVersion10)


        Dim PvtTable As Excel.PivotTable
        Dim pvtField As Excel.PivotField

        PvtTable = CType(oChartSheet.PivotTables(sPivotTable), Excel.PivotTable)

        Dim RowArray(1) As Object

        RowArray(0) = "Start Date"
        RowArray(1) = "Data"


        PvtTable.AddFields(RowFields:=RowArray, PageFields:="Description")

        PvtTable = CType(oChartSheet.PivotTables(sPivotTable), Excel.PivotTable)
        pvtField = CType(PvtTable.PivotFields("Duration"), Excel.PivotField)
        With pvtField
            .Orientation = Excel.XlPivotFieldOrientation.xlDataField
            .Caption = gpsRM.GetString("psTOTAL_CAP") & " " & gpsRM.GetString("psDURATION_CAP")
            .Position = 1
            .Function = Excel.XlConsolidationFunction.xlSum
            .NumberFormat = "[h]:mm:ss;@"
        End With

        PvtTable = CType(oChartSheet.PivotTables(sPivotTable), Excel.PivotTable)
        pvtField = CType(PvtTable.PivotFields("Duration"), Excel.PivotField)
        With pvtField
            .Orientation = Excel.XlPivotFieldOrientation.xlDataField
            .Caption = gpsRM.GetString("psOCCURRENCES_CAP")
        End With

        sRange = sGetLetter(nLastCol + 9) & 3

        With PvtTable.DataPivotField
            .Position = 1
            .Orientation = Excel.XlPivotFieldOrientation.xlColumnField
        End With

        msChartName = gpsRM.GetString("psGRAPH_SHEET_NAME")

        oChart = CType(oXl.Charts.Add, Excel.Chart)
        With oChart
            .SetSourceData(Source:=oChartSheet.Range(sRange))
            .Location(Where:=Excel.XlChartLocation.xlLocationAsObject, Name:=msChartName)
        End With

        With oXl.ActiveChart
            xlSeries = CType(.SeriesCollection, Excel.SeriesCollection)
            xlSeries.Item(1).Select()
            xlSeries.Item(1).AxisGroup = Excel.XlAxisGroup.xlSecondary
            xlSeries.Item(1).ChartType = Excel.XlChartType.xlLineMarkers
            .HasPivotFields = False
            .SizeWithWindow = True
            .PlotArea.Select()
            .HasTitle = True
            xlAxisCategory = CType(.Axes(, Excel.XlAxisGroup.xlPrimary), Excel.Axes)
            xlAxisCategory.Item(Excel.XlAxisType.xlCategory).HasTitle = True
            xlAxisCategory.Item(Excel.XlAxisType.xlCategory).AxisTitle.Characters.Text = gpsRM.GetString("PString.psDATE")

            xlAxisCategory = CType(.Axes(, Excel.XlAxisGroup.xlPrimary), Excel.Axes)
            xlAxisCategory.Item(Excel.XlAxisType.xlValue).HasTitle = True
            xlAxisCategory.Item(Excel.XlAxisType.xlValue).AxisTitle.Characters.Text = gpsRM.GetString("PString.psOCCURRENCES_CAP")

            xlAxisCategory = CType(.Axes(, Excel.XlAxisGroup.xlSecondary), Excel.Axes)
            xlAxisCategory.Item(Excel.XlAxisType.xlValue).HasTitle = True
            xlAxisCategory.Item(Excel.XlAxisType.xlValue).AxisTitle.Characters.Text = gpsRM.GetString("PString.psDURATION_CAP")

            'xlAxisCategory = CType(.Axes(, Excel.XlAxisGroup.xlSecondary), Excel.Axes)
            'xlAxisCategory.Item(Excel.XlAxisType.xlValue).TickLabels.NumberFormat = "[h]:mm"

            .HasPivotFields = False
            .Deselect()
        End With

        sTitle = gpsRM.GetString("psPAST_MON_OCR_DUR")
        'sTitle1 = gpsRM.GetString("psSTART") & " " & _
        '    Format(dStartTime, "Short Date") & "   " & _
        '    gpsRM.GetString("psEND") & " " & _
        '    Format(dEndTime, "Short Date")
        sTitle1 = gpsRM.GetString("psSTART") & " " & _
                            dStartTime.Date.ToString("d") & "   " & _
                            gpsRM.GetString("psEND") & " " & _
                            dEndTime.Date.ToString("d")
        
        oXl.ActiveChart.ChartTitle.Characters.Text = sTitle & vbLf & sTitle1

        '********************************************************************************************
        '********************************************************************************************
        'save current week to WeeklyReports DB
        '********************************************************************************************
        ChartConfig.TopxxReq = 0
        subGetChartData(DT1, DTsumm, sTitle, ChartConfig)
        'Also deletes old data and returns a list of which summaries are available
        DTList = dtSaveWeeklyDataToDB(DTsumm, sTableName, ChartConfig)
        'ChartConfig.TopxxReq = 0

        oChartSheet = DirectCast(oWB.Worksheets.Item(1), Excel.Worksheet)
        subMoveChart(oXl, oWB, oChartSheet, 1, False)

        nRow = 3
        nCol = 1
        'Times
        oChartSheet.Cells(nRow, nCol) = gpsRM.GetString("psSTART") & ": "
        oChartSheet.Cells(nRow, nCol + 1) = ChartConfig.StartTime.Date.ToString("d")
        oChartSheet.Cells(nRow, nCol + 2) = gpsRM.GetString("psEND") & ": "
        oChartSheet.Cells(nRow, nCol + 3) = ChartConfig.EndTime.Date.ToString("d")
        'Stats
        oChartSheet.Cells(nRow + 1, nCol) = gpsRM.GetString("psUPTIME") & ": "
        oChartSheet.Cells(nRow + 1, nCol + 1) = ChartConfig.UptimePct.ToString("00.0") & "%"
        oChartSheet.Cells(nRow + 2, nCol) = gpsRM.GetString("psMTBF") & ": "
        oChartSheet.Cells(nRow + 2, nCol + 1) = " " & frmMain.sGetDurationString(ChartConfig.MTBF)
        oChartSheet.Cells(nRow + 2, nCol + 2) = gpsRM.GetString("psMTTR") & ": "
        oChartSheet.Cells(nRow + 2, nCol + 3) = " " & frmMain.sGetDurationString(ChartConfig.MTTR)

        'Left align the rows
        oCell1 = oChartSheet.Cells(nRow, 1)
        oCell2 = oChartSheet.Cells(nRow + 2, nCol + 3)
        oRange = DirectCast(oChartSheet.Range(oCell1, oCell2), Excel.Range)
        oRange.HorizontalAlignment = Excel.Constants.xlLeft
        'Then right align the labels so they meet up with the data
        oCell1 = oChartSheet.Cells(nRow, 1)
        oCell2 = oChartSheet.Cells(nRow + 2, 1)
        oRange = DirectCast(oChartSheet.Range(oCell1, oCell2), Excel.Range)
        oRange.HorizontalAlignment = Excel.Constants.xlRight
        oCell1 = oChartSheet.Cells(nRow, 3)
        oCell2 = oChartSheet.Cells(nRow + 2, 3)
        oRange = DirectCast(oChartSheet.Range(oCell1, oCell2), Excel.Range)
        oRange.HorizontalAlignment = Excel.Constants.xlRight

    End Sub



    Private Function sGetLetter(ByVal ColNum As Integer) As String
        '********************************************************************************************
        'Description: Get the letter of the column we want
        'Parameters:
        'Returns:
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim nFirstChar As Integer
        'Dim sFirstChar As String
        Dim nSecChar As Integer
        'Dim sSecChar As String

        If ColNum = 0 Then sGetLetter = "A"

        sGetLetter = " "
        nFirstChar = (ColNum \ 26)
        nSecChar = ColNum Mod 26

        'Letter Z offset
        If nSecChar = 0 Then
            nFirstChar = nFirstChar - 1
            nSecChar = 26
        End If

        nFirstChar = nFirstChar + 64
        nSecChar = nSecChar + 64

        If nFirstChar > 64 Then
            sGetLetter = Chr(nFirstChar) & Chr(nSecChar)
        Else
            sGetLetter = Chr(nSecChar)
        End If

    End Function
End Module
