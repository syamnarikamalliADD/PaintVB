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
' Form/Module: frmSemiAutoDates
'
' Description: make selections for semi-auto chart times
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
'    Date       By      Reason                                                          Version
'    08/11/14   MSW     New form for semi-auto reports                                  4.01.07.00
'******************************************************************************************************
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Xml
Imports System.Xml.XPath

Imports System.Windows.Forms

Public Class frmSemiAutoDates
    '********Debug Constants   **********************************************************************
    'Set this to true for use in debugging to load dates for the data you have
    Private Const mbDEBUGDATES As Boolean = False
    Private Const mdDATE As DateTime = #11/26/2008#
    Private Const msSCREEN_DUMP_NAME As String = "Reports_SemiAuto"
    '************************************************************************************************



    '************************************************************************************************
    Private mbSuppressEvents As Boolean = False
    Private colZones As clsZones
    Private mcolShifts As clsShifts
    '************************************************************************************************

    Dim mdEndDate As Date
    Dim mdStartDate As Date
    Dim mdBeforeEndDate As Date
    Dim mdBeforeStartDate As Date



    Friend Property ZoneCollection() As clsZones
        '********************************************************************************************
        'Description:  Zone collection from main form
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return colZones
        End Get
        Set(ByVal value As clsZones)
            colZones = value
        End Set
    End Property
    Private Sub subGetShiftTimes()
        '********************************************************************************************
        'Description:  Set me up
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            mcolShifts = New clsShifts(colZones.ActiveZone)

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: subGetShiftTimes", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Private Sub subInitFormText()
        '********************************************************************************************
        'Description:  Init labels
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            '********************************************************************************************
            'Init text
            '********************************************************************************************
            chkSyncTimeSpans.Text = gpsRM.GetString("psSYNC_TIME_SPANS")
            lblBeforeSpan.Text = gpsRM.GetString("psBEFORE_SPAN")
            lblBeforeStartCap.Text = gpsRM.GetString("psSTART_TIME_CAP")
            lblBeforeEndCap.Text = gpsRM.GetString("psEND_TIME_CAP")
            lblAfterSpan.Text = gpsRM.GetString("psAFTER_SPAN")
            lblStartCap.Text = gpsRM.GetString("psSTART_TIME_CAP")
            lblEndCap.Text = gpsRM.GetString("psEND_TIME_CAP")
            lblTimeSpan.Text = gpsRM.GetString("psTIME_SPAN_CAP")
            lblHours.Text = gpsRM.GetString("psHOURS_CAP")
            lblDays.Text = gpsRM.GetString("psDAYS_CAP")
            lblMinutes.Text = gpsRM.GetString("psMINUTES_CAP")
            Me.Text = gpsRM.GetString("psSEMI_AUTO_FORM_CAP")
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: subInitFormText", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Private Sub subInitializeForm(Optional ByRef dDate As Date = Nothing)
        '********************************************************************************************
        'Description:  Set me up
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            subInitFormText()
            '********************************************************************************************
            'Set default dates and times to set current span from weekly startup to now, and the
            'before span to the same time before weekly shutdown
            '********************************************************************************************
            subGetShiftTimes()
            '********************************************************************************************
            'Loop through the shifts for each day to find the first shift of the week.
            Dim bSet As Boolean = False
            Dim nDay As Integer = 0
            If dDate = Nothing Then
                mdEndDate = New Date(Now.Year, Now.Month, Now.Day, Now.Hour, Now.Minute, 0)
            Else
                mdEndDate = New Date(dDate.Year, dDate.Month, dDate.Day, dDate.Hour, dDate.Minute, 0)
            End If
            mdStartDate = DateAdd(DateInterval.Day, -1, mdEndDate)
            mdBeforeEndDate = DateAdd(DateInterval.Day, -2, mdEndDate)
            mdBeforeStartDate = DateAdd(DateInterval.Day, -3, mdEndDate)
            Do Until bSet
                'Check each shift
                For nShift As Integer = 1 To 3
                    If mcolShifts(nShift).Enabled Then
                        If mcolShifts(nShift).Days(nDay) Then
                            If bSet Then
                                'If one is found already, take the earlier one
                                If mcolShifts(nShift).StartTime.Hour < mdStartDate.Hour Then
                                    mdStartDate = New Date(mdEndDate.Year, mdEndDate.Month, mdEndDate.Day, mcolShifts(nShift).StartTime.Hour, mcolShifts(nShift).StartTime.Minute, mcolShifts(nShift).StartTime.Second)
                                    mdStartDate = DateAdd(DateInterval.Day, nDay - mdEndDate.DayOfWeek, mdStartDate)
                                End If
                            Else
                                'Nothing found yet, take this one 
                                bSet = True
                                mdStartDate = New Date(mdEndDate.Year, mdEndDate.Month, mdEndDate.Day, mcolShifts(nShift).StartTime.Hour, mcolShifts(nShift).StartTime.Minute, mcolShifts(nShift).StartTime.Second)
                                mdStartDate = DateAdd(DateInterval.Day, nDay - mdEndDate.DayOfWeek, mdStartDate)
                            End If
                        End If
                    End If
                Next
                nDay += 1 'Increment day
            Loop
            'Get time span for the current block
            Dim nDays As Long = DateDiff(DateInterval.Day, mdStartDate, mdEndDate)
            Dim dTmpDate As Date = DateAdd(DateInterval.Day, -1 * nDays, mdEndDate)
            Dim nHours As Long = DateDiff(DateInterval.Hour, mdStartDate, dTmpDate)
            dTmpDate = DateAdd(DateInterval.Hour, -1 * nHours, dTmpDate)
            Dim nMinutes As Long = DateDiff(DateInterval.Minute, mdStartDate, dTmpDate)

            'Loop through the end of the week to get the end of the previous time block.
            bSet = False
            nDay = 6
            Do Until bSet
                For nShift As Integer = 1 To 3
                    If mcolShifts(nShift).Enabled Then
                        If mcolShifts(nShift).Days(nDay) Then
                            If bSet Then
                                'If one is found already, take the earlier one
                                'Check for overnight shift or later end time than what we already saved.
                                If (mcolShifts(nShift).EndTime.Hour < mcolShifts(nShift).StartTime.Hour) Or _
                                        (mcolShifts(nShift).EndTime.Hour > mdBeforeEndDate.Hour) Then
                                    mdBeforeEndDate = New Date(mdEndDate.Year, mdEndDate.Month, mdEndDate.Day, mcolShifts(nShift).EndTime.Hour, mcolShifts(nShift).EndTime.Minute, mcolShifts(nShift).EndTime.Second)
                                    mdBeforeEndDate = DateAdd(DateInterval.Day, (nDay - 7) - mdEndDate.DayOfWeek, mdBeforeEndDate)
                                End If
                            Else
                                'Nothing found yet, take this one 
                                bSet = True
                                mdBeforeEndDate = New Date(mdEndDate.Year, mdEndDate.Month, mdEndDate.Day, mcolShifts(nShift).EndTime.Hour, mcolShifts(nShift).EndTime.Minute, mcolShifts(nShift).EndTime.Second)
                                mdBeforeEndDate = DateAdd(DateInterval.Day, (nDay - 7) - mdEndDate.DayOfWeek, mdBeforeEndDate)
                            End If
                            If mcolShifts(nShift).EndTime.Hour < mcolShifts(nShift).StartTime.Hour Then
                                mdBeforeEndDate = DateAdd(DateInterval.Day, 1, mdBeforeEndDate)
                                'Overnight shift automatically wins, get out of the loop
                                Exit For
                            End If
                        End If
                    End If
                Next
                nDay -= 1
            Loop

            mdBeforeStartDate = DateAdd(DateInterval.Day, -1 * nDays, mdBeforeEndDate)
            mdBeforeStartDate = DateAdd(DateInterval.Hour, -1 * nHours, mdBeforeStartDate)
            mdBeforeStartDate = DateAdd(DateInterval.Minute, -1 * nMinutes, mdBeforeStartDate)

            mbSuppressEvents = True
            dtpEndDate.Value = mdEndDate
            dtpEndTime.Value = mdEndDate
            dtpStartDate.Value = mdStartDate
            dtpStartTime.Value = mdStartDate
            dtpBeforeEndDate.Value = mdBeforeEndDate
            dtpBeforeEndTime.Value = mdBeforeEndDate
            dtpBeforeStartDate.Value = mdBeforeStartDate
            dtpBeforeStartTime.Value = mdBeforeStartDate
            chkSyncTimeSpans.Checked = True
            numDays.Value = nDays
            numHours.Value = nHours
            numMinutes.Value = nMinutes
            numDays.Enabled = True
            numHours.Enabled = True
            numMinutes.Enabled = True

            mbSuppressEvents = False

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: subInitializeForm", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Friend Sub ShowMe(Optional ByRef dDate As Date = Nothing)
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
        Try
            '********************************************************************************************
            subInitializeForm(dDate)
            '********************************************************************************************

            If Me.Visible = False Then
                Try
                    Me.ShowDialog()
                Catch ex As Exception
                End Try
            End If
            '********************************************************************************************

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: ShowMe", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Friend Property StartTime() As Date
        '********************************************************************************************
        'Description:  When to start
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Try
                mdStartDate = New Date(dtpStartDate.Value.Year, _
                                                dtpStartDate.Value.Month, _
                                                dtpStartDate.Value.Day, _
                                                dtpStartTime.Value.Hour, _
                                                dtpStartTime.Value.Minute, _
                                                dtpStartTime.Value.Second)
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Property Get: StartTime", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
            Return mdStartDate
        End Get

        Set(ByVal value As Date)
            Try

                mbSuppressEvents = True
                Application.DoEvents()
                mdStartDate = value
                dtpStartDate.Value = value
                dtpStartTime.Value = value
                Application.DoEvents()
                mbSuppressEvents = False
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Property Set: StartTime", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
    End Property
    Friend Property EndTime() As Date
        '********************************************************************************************
        'Description:  When to stop
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Try
                mdEndDate = New Date(dtpEndDate.Value.Year, _
                                                dtpEndDate.Value.Month, _
                                                dtpEndDate.Value.Day, _
                                                dtpEndTime.Value.Hour, _
                                                dtpEndTime.Value.Minute, _
                                                dtpEndTime.Value.Second)
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Property Get: EndTime", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
            Return mdEndDate
        End Get
        Set(ByVal value As Date)
            Try
                mbSuppressEvents = True
                Application.DoEvents()
                mdEndDate = value
                dtpEndDate.Value = value
                dtpEndTime.Value = value
                Application.DoEvents()
                mbSuppressEvents = False
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Property Set: EndTime", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set

    End Property

    Friend Property BeforeStartTime() As Date
        '********************************************************************************************
        'Description:  When to start
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Try
                mdBeforeStartDate = New Date(dtpBeforeStartDate.Value.Year, _
                                    dtpBeforeStartDate.Value.Month, _
                                    dtpBeforeStartDate.Value.Day, _
                                    dtpBeforeStartTime.Value.Hour, _
                                    dtpBeforeStartTime.Value.Minute, _
                                    dtpBeforeStartTime.Value.Second)
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Property Get: BeforeStartTime", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
            Return mdBeforeStartDate
        End Get

        Set(ByVal value As Date)

            Try
                mbSuppressEvents = True
                Application.DoEvents()
                mdBeforeStartDate = value
                dtpBeforeStartDate.Value = value
                dtpBeforeStartTime.Value = value
                Application.DoEvents()
                mbSuppressEvents = False
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Property Set: BeforeStartTime", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
    End Property
    Friend Property BeforeEndTime() As Date
        '********************************************************************************************
        'Description:  When to stop
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Try
                mdBeforeEndDate = New Date(dtpBeforeEndDate.Value.Year, _
                                            dtpBeforeEndDate.Value.Month, _
                                            dtpBeforeEndDate.Value.Day, _
                                            dtpBeforeEndTime.Value.Hour, _
                                            dtpBeforeEndTime.Value.Minute, _
                                            dtpBeforeEndTime.Value.Second)
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Property Get: BeforeEndTime", _
                               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
            Return mdBeforeEndDate
        End Get
        Set(ByVal value As Date)
            Try
                mbSuppressEvents = True
                Application.DoEvents()
                mdBeforeEndDate = value
                dtpBeforeEndDate.Value = value
                dtpBeforeEndTime.Value = value
                Application.DoEvents()
                mbSuppressEvents = False
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Property Set: BeforeEndTime", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try

        End Set

    End Property
    Friend Sub subExit(ByVal okPressed As Boolean)
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


        Try

            If okPressed Then
                If EndTime <= StartTime Then
                    MessageBox.Show(gpsRM.GetString("psEND_TIME_LATER"), "", MessageBoxButtons.OK, _
                                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, _
                                    MessageBoxOptions.DefaultDesktopOnly)
                    Exit Sub
                End If

                Me.Hide()

                frmMain.Cursor = Cursors.WaitCursor
                Application.DoEvents()
                mCharts.subDoAutoCharts(EndTime, StartTime, BeforeEndTime, BeforeStartTime, False)
            Else
                Me.Hide()
                frmMain.Cursor = Cursors.Default
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: subExit", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                                            Handles btnCancel.Click
        '********************************************************************************************
        'Description:  Bail out
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            subExit(False)
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: btnCancel_Click", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                                            Handles btnOK.Click
        '********************************************************************************************
        'Description:  Run Report
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            subExit(True)
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: btnOK_Click", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub frmSemiAutoDates_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        '********************************************************************************************
        'Description: Trap alt - key  combinations to simulate menu button clicks
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Dim sKeyValue As String = String.Empty
        Try

            'Trap Function Key presses
            If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
                Select Case e.KeyCode
                    Case Keys.F1
                        'Help Screen request
                        sKeyValue = "btnHelp"
                        Select Case frmCriteria.ReportType
                            Case frmMain.eReportType.Change
                                subLaunchHelp(gs_HELP_REPORTS_CHANGE, frmMain.oIPC)
                            Case frmMain.eReportType.Alarm
                                subLaunchHelp(gs_HELP_REPORTS_ALARM, frmMain.oIPC)
                            Case frmMain.eReportType.Conveyor
                                subLaunchHelp(gs_HELP_REPORTS_DOWNTIME, frmMain.oIPC)
                            Case frmMain.eReportType.Production
                                subLaunchHelp(gs_HELP_REPORTS_PRODUCTION, frmMain.oIPC)
                            Case frmMain.eReportType.Downtime
                                subLaunchHelp(gs_HELP_REPORTS_DOWNTIME, frmMain.oIPC)
                            Case frmMain.eReportType.RMCharts, frmMain.eReportType.RMChartsAuto
                                subLaunchHelp(gs_HELP_REPORTS_RMCHARTS, frmMain.oIPC)
                            Case frmMain.eReportType.RMChartsSemiAuto
                                subLaunchHelp(gs_HELP_REPORTS_RMCHARTSSEMIAUTO, frmMain.oIPC)
                            Case frmMain.eReportType.Vision
                                subLaunchHelp(gs_HELP_REPORTS_VISION, frmMain.oIPC)
                        End Select
                    Case Keys.F2
                        'Screen Dump request
                        Dim oSC As New ScreenShot.ScreenCapture
                        Dim sDumpPath As String = String.Empty
                        Dim sName As String = msSCREEN_DUMP_NAME
                        Select Case frmCriteria.ReportType
                            Case frmMain.eReportType.Change
                                sName = sName & "_ChangeLog"
                            Case frmMain.eReportType.Alarm
                                sName = sName & "_Alarms"
                            Case frmMain.eReportType.Conveyor
                                sName = sName & "_Conveyor"
                            Case frmMain.eReportType.Production
                                sName = sName & "_Production"
                            Case frmMain.eReportType.Downtime
                                sName = sName & "_Downtime"
                            Case frmMain.eReportType.RMCharts, frmMain.eReportType.RMChartsAuto
                                sName = sName & "_Charts"
                            Case frmMain.eReportType.RMChartsSemiAuto
                                sName = sName & "_ChartsSemiAuto"
                            Case frmMain.eReportType.Vision
                                sName = sName & "_Vision"
                        End Select
                        If frmCriteria.ReportSummaryType <> String.Empty Then
                            sName = sName & "_Summary"
                        End If
                        sName = sName & ".jpg"
                        mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                        oSC.CaptureWindowToFile(Me.Handle, sDumpPath & sName, Imaging.ImageFormat.Jpeg)
                    Case Keys.Escape
                        subExit(False)
                    Case Keys.Enter
                        subExit(True)
                    Case Else

                End Select
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: frmSemiAutoDates_KeyDown", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub chkSyncTimeSpans_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkSyncTimeSpans.CheckedChanged
        '********************************************************************************************
        'Description: When not synced, disable span display
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Try
            numDays.Enabled = chkSyncTimeSpans.Checked
            numHours.Enabled = chkSyncTimeSpans.Checked
            numMinutes.Enabled = chkSyncTimeSpans.Checked
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: chkSyncTimeSpans_CheckedChanged", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub num_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles numDays.ValueChanged, numHours.ValueChanged, numMinutes.ValueChanged
        '********************************************************************************************
        'Description: If synced, update current end and previous start times
        '
        'Parameters: 
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Try
            If mbSuppressEvents = False Then
                mbSuppressEvents = True
                Try
                    mdEndDate = DateAdd(DateInterval.Day, numDays.Value, StartTime)
                    mdEndDate = DateAdd(DateInterval.Hour, numHours.Value, mdEndDate)
                    mdEndDate = DateAdd(DateInterval.Minute, numMinutes.Value, mdEndDate)
                    dtpEndDate.Value = mdEndDate
                    dtpEndTime.Value = mdEndDate

                    mdBeforeStartDate = DateAdd(DateInterval.Day, -1 * numDays.Value, BeforeEndTime)
                    mdBeforeStartDate = DateAdd(DateInterval.Hour, -1 * numHours.Value, mdBeforeStartDate)
                    mdBeforeStartDate = DateAdd(DateInterval.Minute, -1 * numMinutes.Value, mdBeforeStartDate)
                    dtpBeforeStartDate.Value = mdBeforeStartDate
                    dtpBeforeStartTime.Value = mdBeforeStartDate
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: num_ValueChanged", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
                Application.DoEvents()
                mbSuppressEvents = False
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: num_ValueChanged", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub dtpBeforeEnd_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpBeforeEndTime.ValueChanged, dtpBeforeEndDate.ValueChanged
        '********************************************************************************************
        'Description: If synced, update before start times
        '
        'Parameters: 
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        If mbSuppressEvents Then
            Exit Sub
        End If
        Try
            If mdBeforeEndDate.Hour < 2 And dtpBeforeEndTime.Value.Hour > 21 Then
                Try
                    'saved Hour low, new Hour high = Yesterday
                    mbSuppressEvents = True
                    mdBeforeEndDate = BeforeEndTime
                    mdBeforeEndDate = DateAdd(DateInterval.Day, -1, mdBeforeEndDate)
                    dtpBeforeEndDate.Value = mdBeforeEndDate
                    dtpBeforeEndTime.Value = mdBeforeEndDate
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: dtpBeforeEnd_ValueChanged", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
                Application.DoEvents()
                mbSuppressEvents = False
            ElseIf mdBeforeEndDate.Hour > 21 And dtpBeforeEndTime.Value.Hour < 2 Then
                Try
                    'saved Hour high, new Hour low = tomorrow
                    mbSuppressEvents = True
                    mdBeforeEndDate = BeforeEndTime
                    mdBeforeEndDate = DateAdd(DateInterval.Day, 1, mdBeforeEndDate)
                    dtpBeforeEndDate.Value = mdBeforeEndDate
                    dtpBeforeEndTime.Value = mdBeforeEndDate
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: dtpBeforeEnd_ValueChanged", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
                Application.DoEvents()
                mbSuppressEvents = False
            End If
            mdBeforeEndDate = BeforeEndTime 'Update
            If (chkSyncTimeSpans.Checked) Then

                Try
                    mbSuppressEvents = True
                    mdBeforeStartDate = DateAdd(DateInterval.Day, (-1 * (CType(numDays.Value, Double))), mdBeforeEndDate)
                    mdBeforeStartDate = DateAdd(DateInterval.Hour, (-1 * (CType(numHours.Value, Double))), mdBeforeStartDate)
                    mdBeforeStartDate = DateAdd(DateInterval.Minute, (-1 * (CType(numMinutes.Value, Double))), mdBeforeStartDate)
                    dtpBeforeStartDate.Value = mdBeforeStartDate
                    dtpBeforeStartTime.Value = mdBeforeStartDate
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: dtpBeforeEnd_ValueChanged", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
                Application.DoEvents()
                mbSuppressEvents = False
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: dtpBeforeStart_ValueChanged", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Private Sub dtpStart_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpStartDate.ValueChanged, dtpStartTime.ValueChanged
        '********************************************************************************************
        'Description: If synced, update end times
        '
        'Parameters: 
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        If mbSuppressEvents Then
            Exit Sub
        End If
        Try
            If mdStartDate.Hour < 2 And dtpStartTime.Value.Hour > 21 Then
                Try
                    'saved Hour low, new Hour high = Yesterday
                    mbSuppressEvents = True
                    mdStartDate = StartTime
                    mdStartDate = DateAdd(DateInterval.Day, -1, mdStartDate)
                    dtpStartDate.Value = mdStartDate
                    dtpStartTime.Value = mdStartDate
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: dtpStart_ValueChanged", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
                Application.DoEvents()
                mbSuppressEvents = False
            ElseIf mdStartDate.Hour > 21 And dtpStartTime.Value.Hour < 2 Then
                Try
                    'saved Hour high, new Hour low = tomorrow
                    mbSuppressEvents = True
                    mdStartDate = StartTime
                    mdStartDate = DateAdd(DateInterval.Day, 1, mdStartDate)
                    dtpStartDate.Value = mdStartDate
                    dtpStartTime.Value = mdStartDate
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: dtpStart_ValueChanged", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
                Application.DoEvents()
                mbSuppressEvents = False
            End If
            mdStartDate = StartTime 'Update
            If (chkSyncTimeSpans.Checked) Then
                mbSuppressEvents = False
                Try
                    mdEndDate = DateAdd(DateInterval.Day, (CType(numDays.Value, Double)), mdStartDate)
                    mdEndDate = DateAdd(DateInterval.Hour, (CType(numHours.Value, Double)), mdEndDate)
                    mdEndDate = DateAdd(DateInterval.Minute, (CType(numMinutes.Value, Double)), mdEndDate)
                    dtpEndDate.Value = mdEndDate
                    dtpEndTime.Value = mdEndDate
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: dtpStart_ValueChanged", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
                Application.DoEvents()
                mbSuppressEvents = False

            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: dtpStart_ValueChanged", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Private Sub dtpEnd_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpEndTime.ValueChanged, dtpEndDate.ValueChanged
        '********************************************************************************************
        'Description: If synced, update time span, before start time
        '
        'Parameters: 
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        If mbSuppressEvents Then
            Exit Sub
        End If
        Try
            If mdEndDate.Hour < 2 And dtpEndTime.Value.Hour > 21 Then
                Try
                    'saved Hour low, new Hour high = Yesterday
                    mbSuppressEvents = True
                    mdEndDate = EndTime
                    mdEndDate = DateAdd(DateInterval.Day, -1, mdEndDate)
                    dtpEndTime.Value = mdEndDate
                    dtpEndDate.Value = mdEndDate
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: dtpEnd_ValueChanged", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
                Application.DoEvents()
                mbSuppressEvents = False
            ElseIf mdEndDate.Hour > 21 And dtpEndTime.Value.Hour < 2 Then
                Try
                    'saved Hour high, new Hour low = tomorrow
                    mbSuppressEvents = True
                    mdEndDate = StartTime
                    mdEndDate = DateAdd(DateInterval.Day, 1, mdEndDate)
                    dtpEndTime.Value = mdEndDate
                    dtpEndDate.Value = mdEndDate
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: dtpEnd_ValueChanged", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
                Application.DoEvents()
                mbSuppressEvents = False
            End If
            mdEndDate = EndTime
            If (chkSyncTimeSpans.Checked) Then
                mbSuppressEvents = True
                Try
                    Dim nDays As Long = DateDiff(DateInterval.Day, mdStartDate, mdEndDate)
                    Dim dTmpDate As Date = DateAdd(DateInterval.Day, -1 * nDays, mdEndDate)
                    Dim nHours As Long = DateDiff(DateInterval.Hour, mdStartDate, dTmpDate)
                    dTmpDate = DateAdd(DateInterval.Hour, -1 * nHours, dTmpDate)
                    Dim nMinutes As Long = DateDiff(DateInterval.Minute, mdStartDate, dTmpDate)
                    numDays.Value = nDays
                    numHours.Value = nHours
                    numMinutes.Value = nMinutes
                    mdBeforeStartDate = DateAdd(DateInterval.Day, (-1 * nDays), BeforeEndTime)
                    mdBeforeStartDate = DateAdd(DateInterval.Hour, (-1 * nHours), mdBeforeStartDate)
                    mdBeforeStartDate = DateAdd(DateInterval.Minute, (-1 * nMinutes), mdBeforeStartDate)
                    dtpBeforeStartDate.Value = mdBeforeStartDate
                    dtpBeforeStartTime.Value = mdBeforeStartDate
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: dtpEnd_ValueChanged", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
                Application.DoEvents()
                mbSuppressEvents = False
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: dtpEnd_ValueChanged", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub dtpBeforeStart_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpBeforeStartTime.ValueChanged, dtpBeforeStartDate.ValueChanged
        '********************************************************************************************
        'Description: If synced, update time span, end time
        '
        'Parameters: 
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        If mbSuppressEvents Then
            Exit Sub
        End If
        Try
            If mdBeforeEndDate.Hour < 2 And dtpBeforeEndTime.Value.Hour > 21 Then
                Try
                    'saved Hour low, new Hour high = Yesterday
                    mbSuppressEvents = True
                    mdBeforeEndDate = BeforeEndTime
                    mdBeforeEndDate = DateAdd(DateInterval.Day, -1, mdBeforeEndDate)
                    dtpBeforeEndTime.Value = mdBeforeEndDate
                    dtpBeforeEndDate.Value = mdBeforeEndDate
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: dtpBeforeStart_ValueChanged", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
                Application.DoEvents()
                mbSuppressEvents = False
            ElseIf mdBeforeEndDate.Hour > 21 And dtpBeforeEndTime.Value.Hour < 2 Then
                Try
                    'saved Hour high, new Hour low = tomorrow
                    mbSuppressEvents = True
                    mdBeforeEndDate = BeforeEndTime
                    mdBeforeEndDate = DateAdd(DateInterval.Day, 1, mdBeforeEndDate)
                    dtpBeforeEndTime.Value = mdBeforeEndDate
                    dtpBeforeEndDate.Value = mdBeforeEndDate
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: dtpBeforeStart_ValueChanged", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
                Application.DoEvents()
                mbSuppressEvents = False
            End If
            mdBeforeStartDate = BeforeStartTime
            If (chkSyncTimeSpans.Checked) Then
                mbSuppressEvents = True
                Try
                    Dim nDays As Long = DateDiff(DateInterval.Day, mdBeforeStartDate, mdBeforeEndDate)
                    Dim dTmpDate As Date = DateAdd(DateInterval.Day, -1 * nDays, mdBeforeEndDate)
                    Dim nHours As Long = DateDiff(DateInterval.Hour, mdBeforeStartDate, dTmpDate)
                    dTmpDate = DateAdd(DateInterval.Hour, -1 * nHours, dTmpDate)
                    Dim nMinutes As Long = DateDiff(DateInterval.Minute, mdBeforeStartDate, dTmpDate)
                    numDays.Value = nDays
                    numHours.Value = nHours
                    numMinutes.Value = nMinutes
                    mdEndDate = DateAdd(DateInterval.Day, nDays, StartTime)
                    mdEndDate = DateAdd(DateInterval.Hour, nHours, mdEndDate)
                    mdEndDate = DateAdd(DateInterval.Minute, nMinutes, mdEndDate)
                    dtpEndDate.Value = mdEndDate
                    dtpEndTime.Value = mdEndDate
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: dtpBeforeStart_ValueChanged", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
                Application.DoEvents()
                mbSuppressEvents = False
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmSemiAutoDates" & " Routine: dtpBeforeStart_ValueChanged", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

End Class
