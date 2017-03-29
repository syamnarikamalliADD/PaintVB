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
' Form/Module: frmCriteria
'
' Description: make selections for sql query
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
'    11/23/09   MSW     ShowMe - change .show to .showdialog, it'll grab the focus better.
'    01/08/10  MSW     subBuildProductionFieldParams - Add sequence number, needed for summaries
'    03/28/12   MSW     move more DB tables to XML                                      4.01.03.00
'    05/08/12   MSW     remove linked DBs for job status, deal with the bitmasked data  4.01.03.03
'    06/07/12   MSW     Change charts to output a picture without linking to excel      4.01.04.00
'    11/15/12   HGB     Modified subInitializeForm, subSetZoneForRobotCollections,      4.01.04.01
'                       subChangeZone, oGetDeviceBox, and subCLBCheckHandler to 
'                       accommodate a multi-zone SA along with changes to only show the 
'                       relevant devices for the zone(s) in the device combo. Added
'                       subUpdateDeviceList and subLoadDeviceBoxSA.
'    10/16/13   MSW     Add index time for sealer                                       4.01.06.01   
'    07/30/14   MSW     Autorun and daily option development                            4.01.07.04
'******************************************************************************************************
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Xml
Imports System.Xml.XPath

Friend Class frmCriteria

#Region " Declares "

    '********Debug Constants   **********************************************************************
    'Set this to true for use in debugging to load dates for the data you have
    Private Const mbDEBUGDATES As Boolean = False
    Private Const mdDATE As DateTime = #11/26/2008#
    Private Const msSCREEN_DUMP_NAME As String = "Reports_Criteria"

    '************************************************************************************************

    '******** Form Constants   **********************************************************************
    Private Const msZONELISTBOX As String = "lsbZone"
    Private Const msSCREENLISTBOX As String = "lsbScreen"
    Private Const msUSERLISTBOX As String = "lsbUser"
    Private Const msDEVICELISTBOX As String = "lsbDevice"
    Private Const msSTYLELISTBOX As String = "lsbStyle"
    Private Const msCOLORLISTBOX As String = "lsbColor"
    Private Const msOPTIONLISTBOX As String = "lsbOption"
    Private Const msTUTONELISTBOX As String = "lsbTutone"
    Private Const msPARAMLISTBOX As String = "lsbParam"
    Private Const msJOBSTATUSLISTBOX As String = "lsbJobStatus"
    Private Const msSEVERITYLISTBOX As String = "lsbSeverity"
    Private Const msCATEGORYLISTBOX As String = "lsbCategory"
    Private Const msALARMNUMLISTBOX As String = "lsbAlarmNum"
    Private Const msVINCOMBOBOX As String = "cboVIN"
    Private Const msCARRIERCOMBOBOX As String = "cboCarrier"
    Friend Const mnLISTBOXHEIGHT As Integer = 150
    Private Const mnZONE As Integer = 0
    Private Const mnSTART As Integer = 1
    Private Const mnEND As Integer = 2
    '3/7/14 CBZ add sort by sealer style/option
    Private mbUseCombinedStyleOption As Boolean
    
    '*****End Form Constants   **********************************************************************

    '******** Form Variables   **********************************************************************
    Friend Structure tSeverity
        Dim Text As String
        Dim Color As Long
    End Structure
    Private mSeverity As Collection(Of tSeverity)
    Private colControllers As clsControllers = Nothing
    Private colArms As clsArms = Nothing
    Private mbScreenLoaded As Boolean
    Private mbExternalTimeSet As Boolean
    Private mcolShifts As clsShifts
    Private mbBreakAlarmsActive As Boolean
    Private mbGetLastConvOSAlarm As Boolean = False
    Private mbAutoRun As Boolean = False
    Friend Enum eShowCol As Long
        Sequence = CInt(2 ^ 0)
        VIN = CInt(2 ^ 1)
        Color = CInt(2 ^ 2)
        Optioncol = CInt(2 ^ 3)
        Tutone = CInt(2 ^ 4)
        Repair = CInt(2 ^ 5)
        Purge = CInt(2 ^ 6)
        Degrade = CInt(2 ^ 7)
        PaintTotal = CInt(2 ^ 8)
        PaintTotal2 = CInt(2 ^ 9)
        Ratio = CInt(2 ^ 10)
        MaterialLearned = CInt(2 ^ 11)
        MaterialLearned2 = CInt(2 ^ 12)
        MaterialTemp = CInt(2 ^ 13)
        MaterialTemp2 = CInt(2 ^ 14)
        InitString = CInt(2 ^ 15)
        CCTime = CInt(2 ^ 16)
        TPSU = CInt(2 ^ 17)
        Carrier = CInt(2 ^ 18)
        Ghost = CInt(2 ^ 19)
        IndexTime = CInt(2 ^ 20)
    End Enum
    Friend mlShowColumns As Long

    'Private Structure tShift
    '    Dim StartTime As Date
    '    Dim EndTime As Date
    '    Dim Enabled As Boolean
    'End Structure
    'Private mShiftTimes(2) As tShift

    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private colZones As clsZones
    Private mReportType As frmMain.eReportType
    Private mDS As DataSet
    Private msTitles(frmMain.mnMAX_TITLES) As String
    '******** End Property Variables    *************************************************************



    Private Const mbShowDaily as boolean  = false
    Shared Event RunReport(ByVal bAutoSave As Boolean)
    Shared Event QueryFailed(ByVal ErrString As String)

#End Region

#Region " Properties "
    Friend Property TopxxNum() As Decimal
        '********************************************************************************************
        'Description:  Access to topx/lastx records
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If chkQuick.Checked Then
                Return udQuick.Value
            Else
                Return -1
            End If
        End Get
        Set(ByVal value As Decimal)
            udQuick.Value = value
            If value > 0 Then
                chkQuick.Checked = True
            End If
        End Set
    End Property
    Friend ReadOnly Property Daily() As Boolean
        Get
            Return chkDaily.Checked
        End Get
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
            Dim d As New Date(dtpEndDate.Value.Year, _
                                dtpEndDate.Value.Month, _
                                dtpEndDate.Value.Day, _
                                dtpEndTime.Value.Hour, _
                                dtpEndTime.Value.Minute, _
                                dtpEndTime.Value.Second)
            Return d

        End Get
        Set(ByVal value As Date)

            mbExternalTimeSet = True
            dtpEndDate.Value = value
            dtpEndTime.Value = value

        End Set

    End Property
    Friend ReadOnly Property ReportSummaryType() As String
        Get
            If chkSummary.Checked = False Then Return String.Empty

            Dim sTemp As String = String.Empty

            If rbOpt01.Checked Then sTemp = CType(rbOpt01.Tag, String)
            If rbOpt02.Checked Then sTemp = CType(rbOpt02.Tag, String)
            If rbOpt03.Checked Then sTemp = CType(rbOpt03.Tag, String)
            If rbOpt04.Checked Then sTemp = CType(rbOpt04.Tag, String)
            If rbOpt05.Checked Then sTemp = CType(rbOpt05.Tag, String)
            If rbOpt06.Checked Then sTemp = CType(rbOpt06.Tag, String)
            If rbOpt07.Checked Then sTemp = CType(rbOpt07.Tag, String)
            If rbOpt08.Checked Then sTemp = CType(rbOpt08.Tag, String)

            Return sTemp

        End Get
    End Property
    Friend Property ReportType() As frmMain.eReportType
        '********************************************************************************************
        'Description: Pick a report. Any report
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mReportType
        End Get
        Set(ByVal value As frmMain.eReportType)
            mReportType = value
        End Set
    End Property
    Friend ReadOnly Property ReportData() As DataSet
        '********************************************************************************************
        'Description:  this is the dataset based on the criteria
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mDS
        End Get
    End Property
    Friend ReadOnly Property ReportTitles() As String()
        Get
            If msTitles Is Nothing Then
                ReDim msTitles(0)
            End If
            Return msTitles
        End Get
    End Property
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
            Dim d As New Date(dtpStartDate.Value.Year, _
                                dtpStartDate.Value.Month, _
                                dtpStartDate.Value.Day, _
                                dtpStartTime.Value.Hour, _
                                dtpStartTime.Value.Minute, _
                                dtpStartTime.Value.Second)
            Return d

        End Get

        Set(ByVal value As Date)

            mbExternalTimeSet = True
            dtpStartDate.Value = value
            dtpStartTime.Value = value

        End Set
    End Property
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
            LoadZoneBox(cboZone, colZones)
            cboZone.Text = colZones.ActiveZone.Name
        End Set
    End Property

#End Region

#Region " Routines "

    Private Function bCheckHours() As Boolean
        '********************************************************************************************
        'Description: Some summarys are limited to 100 hours due to sql recursion
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If DateDiff(DateInterval.Hour, StartTime, EndTime) > 99 Then
            Dim sMsg As String = gpsRM.GetString("psLIMIT_TO_100_HOURS")

            Dim dr As DialogResult = MessageBox.Show(sMsg, gpsRM.GetString("psADJUST_TIME"), _
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information, _
                        MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)

            If dr = Windows.Forms.DialogResult.Yes Then
                EndTime = DateAdd(DateInterval.Hour, 99, StartTime)
                Return True
            End If

            Return False
        Else
            Return True
        End If

    End Function
    Friend Function bGetDataSet() As Boolean
        '********************************************************************************************
        'Description:  open the data base and run a stored procedure. the command is built by 
        '               adding the parameters desired. add caption parameters to show the columns
        '               add criteria params to look for desired items
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        frmMain.Cursor = Cursors.WaitCursor
        Application.DoEvents()
        Dim DB As clsSQLAccess = New clsSQLAccess
        Dim s As SqlClient.SqlCommand = Nothing
        Dim sTable As String

        mDS = New DataSet
        mDS.Locale = mLanguage.FixedCulture

        Try

            'clear out old titles
            ReDim msTitles(frmMain.mnMAX_TITLES)

            frmMain.Status = gpsRM.GetString("psGETTING_DATA")

            Select Case ReportType

                Case frmMain.eReportType.Downtime  '----------------------------------------------------

                    sTable = gsALARM_DS_TABLENAME

                    With DB
                        .DBFileName = gsALARM_DATABASENAME
                        .Zone = colZones.ActiveZone
                        .DBTableName = sTable
                    End With

                    If chkSummary.Checked Then
                        s = DB.GetStoredProcedureCommand("GetDownTimeRecords")
                        subBuildDownTimeProcedure(s)
                    Else
                        s = DB.GetStoredProcedureCommand("GetDownTimeRecords")
                        subBuildDownTimeProcedure(s)
                    End If
                Case frmMain.eReportType.RMCharts, frmMain.eReportType.RMChartsAuto  '----------------------------------------------------

                    sTable = gsALARM_DS_TABLENAME

                    With DB
                        .DBFileName = gsALARM_DATABASENAME
                        .Zone = colZones.ActiveZone
                        .DBTableName = sTable
                    End With


                    s = DB.GetStoredProcedureCommand("GetDownTimeRecords")
                    subBuildDownTimeProcedure(s)

                Case frmMain.eReportType.Change   '----------------------------------------------------

                    sTable = gsCHANGE_DS_TABLENAME

                    With DB
                        .DBFileName = gsCHANGE_DATABASENAME
                        .Zone = colZones.ActiveZone
                        .DBTableName = sTable
                    End With

                    If chkQuick.Checked Then
                        s = DB.GetStoredProcedureCommand("GetTopxxRecords")
                        subBuildTOPChangeProcedure(s)
                        msTitles(0) = udQuick.Value.ToString & " " & gpsRM.GetString("psMOST_RECENT")

                    ElseIf chkSummary.Checked Then
                        s = DB.GetStoredProcedureCommand("GetChangeSummaryRecords")
                        subBuildChangeSummProcedure(s)
                    Else
                        s = DB.GetStoredProcedureCommand("GetChangeRecords")
                        subBuildChangeProcedure(s)
                    End If


                Case frmMain.eReportType.Alarm   '----------------------------------------------------

                    sTable = gsALARM_DS_TABLENAME

                    With DB
                        .DBFileName = gsALARM_DATABASENAME
                        .Zone = colZones.ActiveZone
                        .DBTableName = sTable
                    End With

                    If chkQuick.Checked Then
                        s = DB.GetStoredProcedureCommand("GetTopxxRecords")
                        subBuildTOPAlarmProcedure(s)
                        msTitles(0) = udQuick.Value.ToString & " " & gpsRM.GetString("psMOST_RECENT")

                    ElseIf chkSummary.Checked Then
                        s = DB.GetStoredProcedureCommand("GetAlarmSummaryRecords")
                        subBuildAlarmSummProcedure(s)
                    Else
                        s = DB.GetStoredProcedureCommand("GetAlarmRecords")
                        subBuildAlarmProcedure(s)
                    End If

                Case frmMain.eReportType.Production  '----------------------------------------------------

                    sTable = gsPROD_DS_TABLENAME

                    With DB
                        .DBFileName = gsPRODLOG_DBNAME
                        .Zone = colZones.ActiveZone
                        .DBTableName = sTable
                    End With

                    If chkQuick.Checked Then
                        s = DB.GetStoredProcedureCommand("GetTopxxRecords")
                        subBuildTOPProductionProcedure(s)
                        msTitles(0) = udQuick.Value.ToString & " " & gpsRM.GetString("psMOST_RECENT")

                    ElseIf chkSummary.Checked Then
                        'should be 1 and only 1 checked
                        Dim sTag As String = String.Empty
                        If rbOpt01.Checked Then sTag = rbOpt01.Tag.ToString
                        If rbOpt02.Checked Then sTag = rbOpt02.Tag.ToString
                        If rbOpt03.Checked Then sTag = rbOpt03.Tag.ToString
                        If rbOpt04.Checked Then sTag = rbOpt04.Tag.ToString
                        If rbOpt05.Checked Then sTag = rbOpt05.Tag.ToString
                        If rbOpt06.Checked Then sTag = rbOpt06.Tag.ToString
                        If rbOpt07.Checked Then sTag = rbOpt07.Tag.ToString
                        If rbOpt08.Checked Then sTag = rbOpt08.Tag.ToString

                        Select Case sTag
                            Case gpsRM.GetString("psJOB_REPORT_CAP"), _
                                    gpsRM.GetString("psSTYLE_CAP"), gpsRM.GetString("psCOLOR_CAP")
                                s = DB.GetStoredProcedureCommand("GetProductionJobSummRecords")
                                subBuildProductionJobSummProcedure(s)

                                'Case gpsRM.GetString("psSTYLE_CAP"), gpsRM.GetString("psCOLOR_CAP"), _
                                '        gpsRM.GetString("psPAINT_TOTAL_CAP")

                                '    If bCheckHours() = False Then Return False

                                '    s = DB.GetStoredProcedureCommand("GetProductionHourSummRecords")
                                '    subBuildProductionHourSummProcedure(s, sTag)
                            Case gpsRM.GetString("psPAINT_TOTAL_CAP")
                                s = DB.GetStoredProcedureCommand("GetProductionRecords")
                                subBuildProductionProcedure(s)
                        End Select


                    Else
                        s = DB.GetStoredProcedureCommand("GetProductionRecords")
                        subBuildProductionProcedure(s)
                    End If

                Case frmMain.eReportType.Vision  '----------------------------------------------------

                    sTable = gsVISN_TABLENAME

                    With DB
                        .DBFileName = gsVISN_DATABASENAME
                        .Zone = colZones.ActiveZone
                        .DBTableName = sTable
                    End With

                    If chkQuick.Checked Then
                        s = DB.GetStoredProcedureCommand("GetTopxxRecords")
                        subBuildTOPVisionProcedure(s)
                        msTitles(0) = udQuick.Value.ToString & " " & gpsRM.GetString("psMOST_RECENT")

                    ElseIf chkSummary.Checked Then
                        s = DB.GetStoredProcedureCommand("GetVisionSummRecords")
                        subBuildVisionSummProcedure(s)
                    Else
                        s = DB.GetStoredProcedureCommand("GetVisionRecords")
                        subBuildVisionProcedure(s)
                    End If

                Case frmMain.eReportType.Conveyor

                    sTable = gsALARM_DS_TABLENAME

                    With DB
                        .DBFileName = gsALARM_DATABASENAME
                        .Zone = colZones.ActiveZone
                        .DBTableName = sTable
                    End With

                    ' chkQuick not valid for conveyor alarms, summary done by 
                    ' frmmain because of need to filter in the break alarms
                    'MSW 8/27/09 - get the last conveyorOS alarm
                    If mbGetLastConvOSAlarm Then
                        s = DB.GetStoredProcedureCommand("GetTopxxRecords")
                        subBuildConveyorProcedure(s)
                    Else
                        s = DB.GetStoredProcedureCommand("GetAlarmRecords")
                        subBuildConveyorProcedure(s)
                    End If

                Case Else   '-----------------------------------------------------------------------

                    'just to make the green squigglys go away
                    s = Nothing
                    sTable = Nothing
                    Return False
            End Select
            'Debug.Print(s.CommandText)

            'For Each oItem As System.Data.SqlClient.SqlParameter In s.Parameters
            '    Debug.Print(oItem.ParameterName & " - " & oItem.SqlValue.ToString & " - " & oItem.Value.ToString)
            'Next
            mDS = DB.GetDataSet(s)
            'Debug.Print(s.CommandText)
            'For Each oTable As DataTable In mDS.Tables
            '    Debug.Print(oTable.Namespace)
            'Next
            If mDS.Tables.Contains(sTable) Then
                frmMain.Status = gpsRM.GetString("psGOT_DATA")
                'Debug.Print(mDS.Tables.Item(sTable).Rows.Count.ToString)

                Return True
            Else
                frmMain.Status = gpsRM.GetString("psGOT_NO_DATA")
                Return False
            End If


        Catch ex As Exception
            ShowErrorMessagebox("", ex, frmMain.Text, frmMain.Status, _
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

    End Function
    Friend Function bShowColumn(ByVal Col As eShowCol) As Boolean
        '********************************************************************************************
        'Description:  check flag to determine if column is shown
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Return (Col And mlShowColumns) = Col

    End Function
    Protected Friend Function GetBreakDataSet() As Boolean
        '********************************************************************************************
        'Description:  This should follow a getconveyor alarms dataset, we use the same times to 
        '               get the break alarms  - this is a behind the scenes thing
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim eTmp As frmMain.eReportType = ReportType
        Try

            mbBreakAlarmsActive = True

            ReportType = frmMain.eReportType.Conveyor

            Dim bTmp As Boolean = bGetDataSet()

            mbBreakAlarmsActive = False

            Return bTmp

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        Finally
            ReportType = eTmp
        End Try

    End Function
    Protected Friend Function GetConveyorDataSet() As Boolean
        '********************************************************************************************
        'Description:  get the conveyor records for the downtime report
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim eTmp As frmMain.eReportType = ReportType
        Try

            mbBreakAlarmsActive = False
            mbGetLastConvOSAlarm = False
            ReportType = frmMain.eReportType.Conveyor
            subGetConvAlarmConfig()

            Dim bTmp As Boolean = bGetDataSet()

            If bTmp Then
                'If it's empty, get the last alarm
                If Me.ReportData.Tables(gsALARM_DS_TABLENAME).Rows.Count < 1 Then
                    mbGetLastConvOSAlarm = True
                    bTmp = bGetDataSet()
                    mbGetLastConvOSAlarm = False
                End If
            End If
            Return bTmp

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        Finally
            ReportType = eTmp
        End Try
    End Function
    Protected Friend Function GetAlarmDataFromProd(ByVal StartTime As DateTime, _
                                                    ByVal Endtime As DateTime) As Boolean
        '********************************************************************************************
        'Description:  get Alarm records from times of Production record clicked on in grid
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim DB As clsSQLAccess = New clsSQLAccess
        Dim s As SqlClient.SqlCommand

        mDS = New DataSet
        mDS.Locale = mLanguage.FixedCulture

        Try
            With DB
                .DBFileName = gsALARM_DATABASENAME
                .Zone = colZones.ActiveZone
                .DBTableName = gsALARM_DS_TABLENAME
            End With

            s = DB.GetStoredProcedureCommand("GetAlarmRecords")

            'column captions -------------------------------------------------------
            subBuildAlarmFieldParams(s)

            'times  ----------------------------------------------------------------
            s.Parameters.Add(New SqlClient.SqlParameter("@StartQuery_Time", _
                SqlDbType.DateTime)).Value = StartTime


            s.Parameters.Add(New SqlClient.SqlParameter("@EndQuery_Time", _
                SqlDbType.DateTime)).Value = Endtime



            mDS = DB.GetDataSet(s)

            If mDS.Tables.Contains(gsALARM_DS_TABLENAME) Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception

            Return False

        End Try
    End Function
    Protected Friend Function GetProductionDataFromAlarm(ByVal StartTime As DateTime, _
                                                    ByVal Endtime As DateTime) As Boolean
        '********************************************************************************************
        'Description:  get production records from times of alarm clicked on in grid
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim DB As clsSQLAccess = New clsSQLAccess
        Dim s As SqlClient.SqlCommand

        mDS = New DataSet
        mDS.Locale = mLanguage.FixedCulture

        Try
            With DB
                .DBFileName = gsPRODLOG_DBNAME
                .Zone = colZones.ActiveZone
                .DBTableName = gsPROD_DS_TABLENAME
            End With

            s = DB.GetStoredProcedureCommand("GetProductionRecords")

            'column captions -------------------------------------------------------
            subBuildProductionFieldParams(s)

            'times  ----------------------------------------------------------------
            s.Parameters.Add(New SqlClient.SqlParameter("@StartQuery_Time", _
                SqlDbType.DateTime)).Value = StartTime


            s.Parameters.Add(New SqlClient.SqlParameter("@EndQuery_Time", _
                SqlDbType.DateTime)).Value = Endtime



            mDS = DB.GetDataSet(s)

            If mDS.Tables.Contains(gsPROD_DS_TABLENAME) Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception

            Return False

        End Try
    End Function
    Private Sub cboBox_LostFocusHandler(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  Not a good place for a breakpoint
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim cbo As ComboBox = DirectCast(sender, ComboBox)

        Dim sTmp As String = cbo.Text

        If cbo.Name = msCARRIERCOMBOBOX Then
            If Not IsNumeric(sTmp) Then
                cbo.SelectedIndex = 0
                Exit Sub
            End If
        End If

        'need to check for people with ill intent....
        If Strings.InStr(sTmp, "drop", CompareMethod.Text) > 0 Then
            cbo.SelectedIndex = 0
            Exit Sub
        End If
        If Strings.InStr(sTmp, "--", CompareMethod.Text) > 0 Then
            cbo.SelectedIndex = 0
            Exit Sub
        End If
        If Strings.InStr(sTmp, ";", CompareMethod.Text) > 0 Then
            cbo.SelectedIndex = 0
            Exit Sub
        End If
        If Strings.InStr(sTmp, "union", CompareMethod.Text) > 0 Then
            cbo.SelectedIndex = 0
            Exit Sub
        End If
        If Strings.InStr(sTmp, "from", CompareMethod.Text) > 0 Then
            cbo.SelectedIndex = 0
            Exit Sub
        End If
        If Strings.InStr(sTmp, "*/", CompareMethod.Text) > 0 Then
            cbo.SelectedIndex = 0
            Exit Sub
        End If
        If Strings.InStr(sTmp, "admin", CompareMethod.Text) > 0 Then
            cbo.SelectedIndex = 0
            Exit Sub
        End If
        If Strings.InStr(sTmp, "set", CompareMethod.Text) > 0 Then
            cbo.SelectedIndex = 0
            Exit Sub
        End If
        If Strings.InStr(sTmp, "password", CompareMethod.Text) > 0 Then
            cbo.SelectedIndex = 0
            Exit Sub
        End If
        If Strings.InStr(sTmp, Chr(34), CompareMethod.Text) > 0 Then
            cbo.SelectedIndex = 0
            Exit Sub
        End If
        If Strings.InStr(sTmp, Chr(39), CompareMethod.Text) > 0 Then
            cbo.SelectedIndex = 0
            Exit Sub
        End If


        If cbo.Items.Contains(sTmp) = False Then
            cbo.Items.Add(sTmp)
        End If

    End Sub
    Private Function oGetLabel() As Label
        '********************************************************************************************
        'Description:  listbox labels
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim l As New Label
        l.Font = New Font("Arial", 12, FontStyle.Bold)
        l.Visible = True
        l.AutoSize = True
        l.Anchor = AnchorStyles.None
        Return l

    End Function
    Friend Function oGetCheckListBox(ByVal vName As String) As CheckedListBox
        '********************************************************************************************
        'Description:  where homeless checked lists live
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As New CheckedListBox
        o.Visible = True
        o.Enabled = True
        o.Font = New Font("Arial", 10)
        o.Name = vName
        o.Anchor = AnchorStyles.None
        o.CheckOnClick = True
        Return o

    End Function
    Private Function oGetAlarmBox() As CheckedListBox
        '********************************************************************************************
        'Description:  get and fill the Alarm listbox
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            Dim o As CheckedListBox = oGetCheckListBox(msALARMNUMLISTBOX)
            o.Size = New Size(150, mnLISTBOXHEIGHT)
            o.Items.Add(gcsRM.GetString("csALL"))
            o.SetItemChecked(0, True)

            For i As Integer = 0 To 999
                o.Items.Add(Format(i, "000"))
            Next

            Return o

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Dim x As New CheckedListBox
            Return x
        End Try

    End Function
    Private Function oGetCarrierBox() As ComboBox
        '********************************************************************************************
        'Description:  get and fill the carrier Combo
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            Dim o As New ComboBox
            o.Visible = True
            o.Enabled = True
            o.Font = New Font("Arial", 10)
            o.Name = msCARRIERCOMBOBOX
            o.Anchor = AnchorStyles.Top

            o.Size = New Size(150, mnLISTBOXHEIGHT)
            o.Items.Add(gcsRM.GetString("csALL"))
            o.SelectedIndex = 0

            Return o

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Dim x As New ComboBox
            Return x
        End Try

    End Function
    Private Function oGetCategoryBox() As CheckedListBox
        '********************************************************************************************
        'Description:  get and fill the Category listbox
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        ' Date      By      Reason
        ' 03/23/12  MSW     Move to XML
        '********************************************************************************************
        Const sXMLFILE As String = "AlarmCategories"
        Const sXMLTABLE As String = "AlarmCategories"
        Const sXMLNODE As String = "AlarmCategory"


        Try

            Dim o As CheckedListBox = oGetCheckListBox(msCATEGORYLISTBOX)
            o.Size = New Size(150, mnLISTBOXHEIGHT)
            o.Items.Add(gcsRM.GetString("csALL"))
            o.SetItemChecked(0, True)

            Dim oMainNode As XmlNode = Nothing
            Dim oNodeList As XmlNodeList = Nothing
            Dim oXMLDoc As New XmlDocument
            Dim sXMLFilePath As String = String.Empty
            Dim tControllerArray() As tController = Nothing
            Dim sTmp As String
            Try
                If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLFILE & ".XML") Then


                    oXMLDoc.Load(sXMLFilePath)

                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)

                    mSeverity = New Collection(Of tSeverity)

                    For Each oNode As XmlNode In oNodeList
                        sTmp = gpsRM.GetString(oNode.Item("ResourceString").InnerText)
                        If sTmp = String.Empty Then
                            sTmp = oNode.Item("Categories").InnerText
                        End If

                        o.Items.Add(sTmp)
                    Next

                End If

            Catch ex As Exception
                Trace.WriteLine(ex.Message)
                Dim x As New CheckedListBox
                Return x
            End Try

            Return o

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Dim x As New CheckedListBox
            Return x
        End Try

    End Function
    Private Function oGetColorBox() As CheckedListBox
        '********************************************************************************************
        'Description:  get and fill the Style listbox
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim DB As New clsSQLAccess

            Dim o As CheckedListBox = oGetCheckListBox(msCOLORLISTBOX)
            o.Size = New Size(170, mnLISTBOXHEIGHT)

            ' !! What if more than 1 zone is selected??
            mSysColorCommon.LoadColorBoxFromDB(o, colZones.ActiveZone, True)

            o.SetItemChecked(0, True)

            Return o

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Dim x As New CheckedListBox
            Return x
        End Try

    End Function
    Private Function oGetDeviceBox(ByVal bAddControllers As Boolean, Optional ByVal bAddArms As Boolean = True) As CheckedListBox
        '********************************************************************************************
        'Description:  get and fill the Device listbox
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/02/09  MSW     Make the arms optional
        ' 11/15/12  HGB     Don't display both arms and controllers in SA since we name them the same 
        '                   on the one arm systems.
        '********************************************************************************************
        Try

            Dim o As CheckedListBox = oGetCheckListBox(msDEVICELISTBOX)
            o.Size = New Size(150, mnLISTBOXHEIGHT)
            o.Items.Add(gcsRM.GetString("csALL"))
            o.SetItemChecked(0, True)
            If bAddArms And Not (colZones.StandAlone And bAddControllers) Then 'HGB 11/15/12
                subAddArmsToCLB(o, False)
            End If
            If bAddControllers Then
                With o
                    For Each c As clsController In colControllers
                        .Items.Add(c.Name)
                    Next
                End With
            End If

            Return o

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Dim x As New CheckedListBox
            Return x
        End Try

    End Function

    Private Function oGetJobStatusBox() As CheckedListBox
        '********************************************************************************************
        'Description:  get and fill the Job Status listbox. this also copies the current language
        '               Strings for job status to the production log database
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/26/09  MSW     Handle duplicate entries in theproduction job status DB
        '********************************************************************************************
        ' 03/28/12  MSW     Move to XML
        '********************************************************************************************
        Const sXMLFILE As String = "ReportConfig"
        Const sXMLTABLE As String = "CompletionStatus"
        Const sXMLNODE As String = "CompItem"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Dim bSuccess As Boolean = True
        Dim nNode As Integer = 0
        Dim nIndex As Integer = 0
        ReDim frmMain.gtConvAlarms.HoldForHome(6)
        ReDim frmMain.gtConvAlarms.Bypassed(6)
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLFILE & ".XML") Then
            Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)

                For Each oNode As XmlNode In oNodeList
                    Debug.Print(oNode.Item("Description").InnerXml)
                    Dim oCompStat As New clsCompStat
                    Try
                        oCompStat.bitValue = CType(oNode.Item("BitValue").InnerXml, Integer)
                    Catch ex As Exception
                        oCompStat.bitValue = 0
                    End Try
                    Try
                        oCompStat.Description = oNode.Item("Description").InnerXml
                    Catch ex As Exception
                        oCompStat.Description = String.Empty
                    End Try
                    Try
                        If oNode.InnerXml.Contains("ResourceTag") Then
                            Dim sTmpTag As String = gpsRM.GetString(oNode.Item("ResourceTag").InnerXml)
                            If sTmpTag <> String.Empty Then
                                oCompStat.Description = sTmpTag
                            End If
                        End If
                    Catch ex As Exception
                        'Leave description from XML
                    End Try
                    Select Case oNode.Item("Operation").InnerXml
                        Case "REPLACE"
                            oCompStat.Operation = clsCompStat.eCompStatOp.REPLACE
                        Case "SUFFIX"
                            oCompStat.Operation = clsCompStat.eCompStatOp.SUFFIX
                        Case "PREFIX"
                            oCompStat.Operation = clsCompStat.eCompStatOp.PREFIX
                        Case Else
                            oCompStat.Operation = clsCompStat.eCompStatOp.REPLACE
                    End Select
                    frmMain.gCompStats.Add(oCompStat)
                Next
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmCriteria" & " Routine: oGetJobStatusBox", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If

        Dim sTag As String()

        Try

            Dim o As CheckedListBox = oGetCheckListBox(msJOBSTATUSLISTBOX)
            o.Size = New Size(170, mnLISTBOXHEIGHT)
            o.Items.Add(gcsRM.GetString("csALL"))
            o.SetItemChecked(0, True)

            ReDim sTag(0)
            sTag(0) = gcsRM.GetString("csALL")
            Dim nTagIndex As Integer = 0
            For Each oCompStat As clsCompStat In frmMain.gCompStats
                nTagIndex = nTagIndex + 1
                ReDim Preserve sTag(nTagIndex)
                sTag(nTagIndex) = oCompStat.bitValue.ToString
                o.Items.Add(oCompStat.Description)
            Next

            o.Tag = sTag

            Return o

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Dim x As New CheckedListBox
            Return x
        End Try

    End Function
    Private Function oGetOptionBox() As CheckedListBox
        '********************************************************************************************
        'Description:  get and fill the Option listbox
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim DB As New clsSQLAccess

            Dim o As CheckedListBox = oGetCheckListBox(msOPTIONLISTBOX)
            o.Size = New Size(170, mnLISTBOXHEIGHT)

            ' !! What if more than 1 zone is selected??
            Dim opt As New clsSysOptions(colZones.ActiveZone)
            opt.LoadOptionBoxFromCollection(o, True)
            o.SetItemChecked(0, True)

            Return o

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Dim x As New CheckedListBox
            Return x
        End Try

    End Function
    Private Function oGetParameterBox() As CheckedListBox
        '********************************************************************************************
        'Description:  get and fill the Parameter listbox
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim o As CheckedListBox = oGetCheckListBox(msPARAMLISTBOX)
            o.Size = New Size(350, mnLISTBOXHEIGHT)

            If colArms.Count > 0 Then
                Select Case ReportType
                    Case frmMain.eReportType.Change
                        Dim oBot As clsArm = colArms(0)
                        'load colors
                        Dim oCol As clsSysColors = oBot.SystemColors
                        LoadColorBoxFromCollection(oCol, o)
                        'some screens use styles as parameters
                        Dim oStys As clsSysStyles = New clsSysStyles(colZones.ActiveZone)
                        For Each oSty As clsSysStyle In oStys
                            o.Items.Add(oSty.DisplayName)
                        Next
                        'Add none
                        o.Items.Add(gcsRM.GetString("csNO_PARAMETER"))
                End Select
            End If

            o.Items.Insert(0, gcsRM.GetString("csALL"))
            o.SetItemChecked(0, True)

            Return o

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Dim x As New CheckedListBox
            Return x
        End Try

    End Function
    Private Function oGetSeverityBox() As CheckedListBox
        '********************************************************************************************
        'Description:  get and fill the Severity listbox
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/23/12  MSW     Move to XML
        '********************************************************************************************
        Const sXMLFILE As String = "AlarmSeverities"
        Const sXMLTABLE As String = "AlarmSeverities"
        Const sXMLNODE As String = "AlarmSeverity"


        Try

            Dim o As CheckedListBox = oGetCheckListBox(msSEVERITYLISTBOX)
            o.Size = New Size(150, mnLISTBOXHEIGHT)
            o.Items.Add(gcsRM.GetString("csALL"))
            o.SetItemChecked(0, True)

            Dim oMainNode As XmlNode = Nothing
            Dim oNodeList As XmlNodeList = Nothing
            Dim oXMLDoc As New XmlDocument
            Dim sXMLFilePath As String = String.Empty
            Dim tControllerArray() As tController = Nothing
            Dim sTmp As String
            Try
                If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLFILE & ".XML") Then


                    oXMLDoc.Load(sXMLFilePath)

                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)

                    mSeverity = New Collection(Of tSeverity)

                    For Each oNode As XmlNode In oNodeList
                        sTmp = gpsRM.GetString(oNode.Item("ResourceString").InnerText)
                        If sTmp = String.Empty Then
                            sTmp = oNode.Item("Severity").InnerText
                        End If

                        Dim oS As New tSeverity
                        oS.Text = sTmp
                        oS.Color = CLng(oNode.Item("Color").InnerText)

                        mSeverity.Add(oS)

                        o.Items.Add(sTmp)
                    Next

                End If

            Catch ex As Exception
                Trace.WriteLine(ex.Message)
                Dim x As New CheckedListBox
                Return x
            End Try

            Return o

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Dim x As New CheckedListBox
            Return x
        End Try

    End Function
    Private Function oGetStyleBox() As CheckedListBox
        '********************************************************************************************
        'Description:  get and fill the Style listbox
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim DB As New clsSQLAccess

            Dim o As CheckedListBox = oGetCheckListBox(msSTYLELISTBOX)
            o.Size = New Size(170, mnLISTBOXHEIGHT)

            ' !! What if more than 1 zone is selected??
            Dim s As New clsSysStyles(colZones.ActiveZone)

            '3/7/14 CBZ add sort by sealer style/option
            mbUseCombinedStyleOption = s.CombinedStyleOption

            s.LoadStyleBoxFromCollection(o, True)
            o.SetItemChecked(0, True)

            Return o

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Dim x As New CheckedListBox
            Return x
        End Try

    End Function
    Private Function oGetScreenBox() As CheckedListBox
        '********************************************************************************************
        'Description:  get and fill the screens listbox
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim o As CheckedListBox = oGetCheckListBox(msSCREENLISTBOX)
            o.Size = New Size(350, mnLISTBOXHEIGHT)
            o.Items.Add(gcsRM.GetString("csALL"))
            o.SetItemChecked(0, True)

            'Dim oInfo As String() = DirectCast(frmMain.moPassword.Information.Screens, String())
            Dim oTmpTag As String() = frmMain.ScreenNames
            Dim oTag(frmMain.ScreenNames.GetUpperBound(0)+1) As String
            oTag(0) = gcsRM.GetString("csALL")
            Dim oInfo As String() = frmMain.ScreenDisplayNames

            For i As Integer = 0 To UBound(oInfo)
                o.Items.Add(oInfo(i))
                oTag(i + 1) = oTmpTag(i)
            Next
            o.Tag = oTag
            Return o

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Dim x As New CheckedListBox
            Return x
        End Try

    End Function
    Private Function oGetUserBox() As CheckedListBox
        '********************************************************************************************
        'Description:  get and fill the Users listbox
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim o As CheckedListBox = oGetCheckListBox(msUSERLISTBOX)
            o.Size = New Size(150, mnLISTBOXHEIGHT)
            o.Items.Add(gcsRM.GetString("csALL"))
            o.SetItemChecked(0, True)

            'Dim oInfo As String() = DirectCast(frmMain.moPassword.Information.Users, String())
            Dim oInfo As String() = frmMain.UserNames 'RJO 03/23/12

            For i As Integer = 0 To UBound(oInfo)
                o.Items.Add(oInfo(i))
            Next

            Return o

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Dim x As New CheckedListBox
            Return x
        End Try

    End Function
    Private Function oGetVINBox() As ComboBox
        '********************************************************************************************
        'Description:  get and fill the Vin Combo
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            Dim o As New ComboBox
            o.Visible = True
            o.Enabled = True
            o.Font = New Font("Arial", 10)
            o.Name = msVINCOMBOBOX
            o.Anchor = AnchorStyles.Top

            o.Size = New Size(150, mnLISTBOXHEIGHT)
            o.Items.Add(gcsRM.GetString("csALL"))
            o.SelectedIndex = 0

            Return o

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Dim x As New ComboBox
            Return x
        End Try

    End Function
    Private Function oGetZoneBox() As CheckedListBox
        '********************************************************************************************
        'Description:  get and fill the zone listbox
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim o As CheckedListBox = oGetCheckListBox(msZONELISTBOX)
            o.Size = New Size(150, mnLISTBOXHEIGHT)

            LoadZoneBox(o, colZones, False)
            If colZones.Count > 1 Then
                o.Items.Insert(0, gcsRM.GetString("csALL"))
            End If

            o.SetItemChecked(0, True)

            Return o

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Dim x As New CheckedListBox
            Return x
        End Try
    End Function
    Private Sub subAddArmsToCLB(ByRef oCLB As CheckedListBox, ByVal bClearFirst As Boolean)
        '********************************************************************************************
        'Description:  Add the arms in the arm collection to the listbox
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        With oCLB
            If bClearFirst Then .Items.Clear()
            For Each o As clsArm In colArms
                If Not o.IsOpener Then
                .Items.Add(o.Name)
                End If
            Next
        End With

    End Sub
    Private Sub subBuildAlarmFieldParams(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description:  Select the Alarm report fields that go into the stored procedure
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        With s.Parameters

            .Add(New SqlClient.SqlParameter("@Alarm_Number_Cap", _
                 SqlDbType.NVarChar)).Value = gpsRM.GetString("psALARM_CAP")
            .Add(New SqlClient.SqlParameter("@Zone_Cap", _
                 SqlDbType.NVarChar)).Value = gcsRM.GetString("csZONE_CAP")
            .Add(New SqlClient.SqlParameter("@Device_Cap", _
                SqlDbType.NVarChar)).Value = gcsRM.GetString("csROBOT_CAP")
            .Add(New SqlClient.SqlParameter("@Description_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psDESC_CAP")
            .Add(New SqlClient.SqlParameter("@Severity_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psSEVERITY_CAP")
            .Add(New SqlClient.SqlParameter("@Category_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psCATEGORY_CAP")
            .Add(New SqlClient.SqlParameter("@Start_Time_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psSTART_TIME_CAP")
            .Add(New SqlClient.SqlParameter("@End_Time_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psEND_TIME_CAP")
            .Add(New SqlClient.SqlParameter("@Duration_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psDURATION_CAP")
            .Add(New SqlClient.SqlParameter("@Item_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psITEM_CAP")

            If bShowColumn(eShowCol.VIN) Then
                .Add(New SqlClient.SqlParameter("@VIN_Number_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psVIN_CAP")
            End If

            If bShowColumn(eShowCol.Carrier) Then
                .Add(New SqlClient.SqlParameter("@Carrier_Number_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psCARRIER_CAP")
            End If

            .Add(New SqlClient.SqlParameter("@Plant_Style_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psSTYLE_CAP")

            '.Add(New SqlClient.SqlParameter("@Job_Name_Cap", _
            '    SqlDbType.NVarChar)).Value = gpsRM.GetString("psJOBNAME_CAP")

            If bShowColumn(eShowCol.Color) Then
                .Add(New SqlClient.SqlParameter("@Plant_Color_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psCOLOR_CAP")
            End If

            '.Add(New SqlClient.SqlParameter("@Node_Cap", _
            '    SqlDbType.NVarChar)).Value = gpsRM.GetString("psNODE_CAP")

            '.Add(New SqlClient.SqlParameter("@Process_Name_Cap", _
            '    SqlDbType.NVarChar)).Value = gpsRM.GetString("psPROCESSNAME_CAP")

        End With
    End Sub
    Private Sub subBuildAlarmCriteriaParams(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description:  Add the parameters that may be changed from the checked list boxes
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sSel As String

        With s.Parameters

            'zone box
            sSel = sGetCLBSelections(msZONELISTBOX)
            .Add(New SqlClient.SqlParameter("@Zone", _
                SqlDbType.NVarChar)).Value = sSel

            msTitles(mnZONE) = sGetTitleCaption(gcsRM.GetString("csZONE_CAP"), sSel)

            'device box
            sSel = sGetCLBSelections(msDEVICELISTBOX)
            .Add(New SqlClient.SqlParameter("@Device", _
                    SqlDbType.NVarChar)).Value = sSel

            msTitles(5) = sGetTitleCaption(gcsRM.GetString("csROBOT_CAP"), sSel)

            'Severity
            sSel = sGetCLBSelections(msSEVERITYLISTBOX)
            .Add(New SqlClient.SqlParameter("@Severity", _
                    SqlDbType.NVarChar)).Value = sSel

            msTitles(3) = sGetTitleCaption(gpsRM.GetString("psSEVERITY_CAP"), sSel)

            'Category
            sSel = sGetCLBSelections(msCATEGORYLISTBOX)
            .Add(New SqlClient.SqlParameter("@Category", _
                    SqlDbType.NVarChar)).Value = sSel

            msTitles(6) = sGetTitleCaption(gpsRM.GetString("psCATEGORY_CAP"), sSel)

            'alarm # 
            sSel = sGetCLBSelections(msALARMNUMLISTBOX)
            .Add(New SqlClient.SqlParameter("@Alarm_Number", _
                    SqlDbType.NVarChar)).Value = sSel

            msTitles(4) = sGetTitleCaption(gpsRM.GetString("psALARM_CAP"), sSel)

            'VIN Number
            If bShowColumn(eShowCol.VIN) Then
                Dim c As ComboBox = DirectCast(tlpMain.Controls(msVINCOMBOBOX), ComboBox)
                .Add(New SqlClient.SqlParameter("@VIN_Number", _
                        SqlDbType.NVarChar)).Value = c.Text
            End If

            If bShowColumn(eShowCol.Carrier) Then
                Dim c As ComboBox = DirectCast(tlpMain.Controls(msCARRIERCOMBOBOX), ComboBox)
                .Add(New SqlClient.SqlParameter("@Carrier_Number", _
                        SqlDbType.NVarChar)).Value = c.Text
            End If

            'times  --------------------------------------------------------------------
            sSel = String.Empty

            .Add(New SqlClient.SqlParameter("@StartQuery_Time", _
                SqlDbType.DateTime)).Value = StartTime

            sSel = StartTime.ToString
            msTitles(mnSTART) = sGetTitleCaption(gpsRM.GetString("psSTART"), sSel)

            .Add(New SqlClient.SqlParameter("@EndQuery_Time", _
                SqlDbType.DateTime)).Value = EndTime

            sSel = EndTime.ToString
            msTitles(mnEND) = sGetTitleCaption(gpsRM.GetString("psEND"), sSel)

            If mbShowDaily then
              If chkDaily.Enabled And chkDaily.Checked Then
                  .Add(New SqlClient.SqlParameter("@Daily", _
                      SqlDbType.Bit)).Value = 1
              Else
                  .Add(New SqlClient.SqlParameter("@Daily", _
                      SqlDbType.Bit)).Value = 0
              End If
            End If
        End With

    End Sub
    Private Sub subBuildConveyorProcedure(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description:  build stored procedure command and title strings for the Conveyor report
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sAll As String = gcsRM.GetString("csALL")
        Dim sSel As String = String.Empty

        With s.Parameters

            With frmMain.gtConvAlarms
                'first time thru we get conveyor alarms, then behind the scenes we get the break 
                'alarms and use that result set to mask out break times
                If mbBreakAlarmsActive Then
                    'Break alarms
                    sSel = .BreakActive & "," & .BreakActiveOS
                Else
                    'conveyor alarms
                    sSel = .HeldByFanuc & "," & .HeldByFanucOS & "," & .HeldByOthers & "," & _
                    .HeldByOthersOS & "," & .Running & "," & .RunningOS
                End If
            End With

            'alarms
            .Add(New SqlClient.SqlParameter("@Alarm_Number", _
                SqlDbType.NVarChar)).Value = sSel
            If (mbGetLastConvOSAlarm) Then
                .Add(New SqlClient.SqlParameter("@Number_Of_Records", _
                    SqlDbType.Int)).Value = 1
            Else
                'times  --------------------------------------------------------------------
                .Add(New SqlClient.SqlParameter("@StartQuery_Time", _
                    SqlDbType.DateTime)).Value = StartTime

                sSel = StartTime.ToString
                msTitles(mnSTART) = sGetTitleCaption(gpsRM.GetString("psSTART"), sSel)

                .Add(New SqlClient.SqlParameter("@EndQuery_Time", _
                    SqlDbType.DateTime)).Value = EndTime

                sSel = EndTime.ToString
                msTitles(mnEND) = sGetTitleCaption(gpsRM.GetString("psEND"), sSel)
            End If
            'now add column captions -------------------------------------------------------
            .Add(New SqlClient.SqlParameter("@Alarm_Number_Cap", _
                 SqlDbType.NVarChar)).Value = gpsRM.GetString("psALARM_CAP")
            .Add(New SqlClient.SqlParameter("@Zone_Cap", _
                 SqlDbType.NVarChar)).Value = gcsRM.GetString("csZONE_CAP")
            '.Add(New SqlClient.SqlParameter("@Device_Cap", _
            '    SqlDbType.NVarChar)).Value = gcsRM.GetString("csROBOT_CAP")
            .Add(New SqlClient.SqlParameter("@Description_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psDESC_CAP")
            .Add(New SqlClient.SqlParameter("@Start_Time_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psSTART_TIME_CAP")
            .Add(New SqlClient.SqlParameter("@End_Time_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psEND_TIME_CAP")
            .Add(New SqlClient.SqlParameter("@Duration_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psDURATION_CAP")
            '.Add(New SqlClient.SqlParameter("@Item_Cap", _
            '    SqlDbType.NVarChar)).Value = gpsRM.GetString("psITEM_CAP")
            'MSW 9/2/09 - add a few columns to the downtime reports
            If bShowColumn(eShowCol.VIN) Then
                .Add(New SqlClient.SqlParameter("@VIN_Number_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psVIN_CAP")
            End If

            If bShowColumn(eShowCol.Carrier) Then
                .Add(New SqlClient.SqlParameter("@Carrier_Number_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psCARRIER_CAP")
            End If

            .Add(New SqlClient.SqlParameter("@Plant_Style_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psSTYLE_CAP")


            If bShowColumn(eShowCol.Color) Then
                .Add(New SqlClient.SqlParameter("@Plant_Color_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psCOLOR_CAP")
            End If


            If mbShowDaily then
              If chkDaily.Enabled And chkDaily.Checked Then
                  .Add(New SqlClient.SqlParameter("@Daily", _
                      SqlDbType.Bit)).Value = 1
              Else
                  .Add(New SqlClient.SqlParameter("@Daily", _
                      SqlDbType.Bit)).Value = 0
              End If
            End IF
        End With

    End Sub
    Private Sub subBuildDownTimeProcedure(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description:  build stored procedure command and title strings for the Downtime report
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sAll As String = gcsRM.GetString("csALL")
        Dim sSel As String = String.Empty


        With s.Parameters

            ' For now there is no criteria but time and zone
            'zone box
            sSel = sGetCLBSelections(msZONELISTBOX)
            .Add(New SqlClient.SqlParameter("@Zone", _
                SqlDbType.NVarChar)).Value = sSel

            msTitles(mnZONE) = sGetTitleCaption(gcsRM.GetString("csZONE_CAP"), sSel)

            'times  --------------------------------------------------------------------

            .Add(New SqlClient.SqlParameter("@StartQuery_Time", _
                SqlDbType.DateTime)).Value = StartTime

            sSel = StartTime.ToString
            msTitles(mnSTART) = sGetTitleCaption(gpsRM.GetString("psSTART"), sSel)

            .Add(New SqlClient.SqlParameter("@EndQuery_Time", _
                SqlDbType.DateTime)).Value = EndTime

            sSel = EndTime.ToString
            msTitles(mnEND) = sGetTitleCaption(gpsRM.GetString("psEND"), sSel)

            'now add column captions -------------------------------------------------------
            .Add(New SqlClient.SqlParameter("@Alarm_Number_Cap", _
                 SqlDbType.NVarChar)).Value = gpsRM.GetString("psALARM_CAP")
            .Add(New SqlClient.SqlParameter("@Zone_Cap", _
                 SqlDbType.NVarChar)).Value = gcsRM.GetString("csZONE_CAP")
            .Add(New SqlClient.SqlParameter("@Device_Cap", _
                SqlDbType.NVarChar)).Value = gcsRM.GetString("csROBOT_CAP")
            .Add(New SqlClient.SqlParameter("@Description_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psDESC_CAP")
            .Add(New SqlClient.SqlParameter("@Start_Time_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psSTART_TIME_CAP")
            .Add(New SqlClient.SqlParameter("@End_Time_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psEND_TIME_CAP")
            .Add(New SqlClient.SqlParameter("@Duration_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psDURATION_CAP")
            .Add(New SqlClient.SqlParameter("@Item_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psITEM_CAP")

            If bShowColumn(eShowCol.VIN) Then
                .Add(New SqlClient.SqlParameter("@VIN_Number_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psVIN_CAP")
            End If

            If bShowColumn(eShowCol.Carrier) Then
                .Add(New SqlClient.SqlParameter("@Carrier_Number_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psCARRIER_CAP")
            End If

            .Add(New SqlClient.SqlParameter("@Plant_Style_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psSTYLE_CAP")


            If bShowColumn(eShowCol.Color) Then
                .Add(New SqlClient.SqlParameter("@Plant_Color_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psCOLOR_CAP")
            End If

            ''.Add(New SqlClient.SqlParameter("@Node_Cap", _
            ''    SqlDbType.NVarChar)).Value = gpsRM.GetString("psNODE_CAP")

            ''.Add(New SqlClient.SqlParameter("@Process_Name_Cap", _
            ''    SqlDbType.NVarChar)).Value = gpsRM.GetString("psPROCESSNAME_CAP")

            ''.Add(New SqlClient.SqlParameter("@Job_Name_Cap", _
            ''    SqlDbType.NVarChar)).Value = gpsRM.GetString("psJOBNAME_CAP")

            ''.Add(New SqlClient.SqlParameter("@Severity_Cap", _
            ''    SqlDbType.NVarChar)).Value = gpsRM.GetString("psSEVERITY_CAP")
            ''.Add(New SqlClient.SqlParameter("@Category_Cap", _
            ''    SqlDbType.NVarChar)).Value = gpsRM.GetString("psCATEGORY_CAP")

            If mbShowDaily then
              If chkDaily.Enabled And chkDaily.Checked Then
                  .Add(New SqlClient.SqlParameter("@Daily", _
                      SqlDbType.Bit)).Value = 1
              Else
                  .Add(New SqlClient.SqlParameter("@Daily", _
                      SqlDbType.Bit)).Value = 0
              End If
            End If
        End With

    End Sub
    Private Sub subBuildAlarmProcedure(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description:  build stored procedure command and title strings for the Alarm report
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sAll As String = gcsRM.GetString("csALL")


        With s.Parameters

            ' add the parameters selected from the listboxes
            subBuildAlarmCriteriaParams(s)


            ' now add the extra columns that have no criteria


            'Description 
            .Add(New SqlClient.SqlParameter("@Description", _
                    SqlDbType.NVarChar)).Value = sAll


            'style Number
            .Add(New SqlClient.SqlParameter("@Plant_Style", _
                        SqlDbType.NVarChar)).Value = sAll

            'Color Number
            If bShowColumn(eShowCol.Color) Then
                .Add(New SqlClient.SqlParameter("@Plant_Color", _
                            SqlDbType.NVarChar)).Value = sAll
            End If

            ''Job Name
            '.Add(New SqlClient.SqlParameter("@Job_Name", _
            '            SqlDbType.NVarChar)).Value = sAll


            ''Process Name
            '.Add(New SqlClient.SqlParameter("@Process_Name", _
            '            SqlDbType.NVarChar)).Value = sAll


            ''NOde
            '.Add(New SqlClient.SqlParameter("@Node", _
            '            SqlDbType.Int)).Value = 0



        End With

        'now add column captions -------------------------------------------------------
        subBuildAlarmFieldParams(s)

    End Sub
    Private Sub subBuildAlarmSummProcedure(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description: Get Alarm summary parameters
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sAll As String = gcsRM.GetString("csALL")

        ' add the parameters selected from the listboxes
        subBuildAlarmCriteriaParams(s)

        'column captions
        With s.Parameters

            .Add(New SqlClient.SqlParameter("@Alarm_Number_Cap", _
                 SqlDbType.NVarChar)).Value = gpsRM.GetString("psALARM_CAP")
            .Add(New SqlClient.SqlParameter("@Description_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psDESC_CAP")
            .Add(New SqlClient.SqlParameter("@Total_Cap", SqlDbType.NVarChar)).Value = _
                    gpsRM.GetString("psOCCURRENCES_CAP")
            .Add(New SqlClient.SqlParameter("@Duration_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psDURATION_CAP")

            ' sort on
            Dim sSort As String = String.Empty

            'alarm number
            If rbOpt01.Checked Then sSort = CType(rbOpt01.Tag, String)

            'device
            If rbOpt02.Checked Then
                sSort = CType(rbOpt02.Tag, String)
                .Add(New SqlClient.SqlParameter("@Device_Cap", _
                    SqlDbType.NVarChar)).Value = gcsRM.GetString("csROBOT_CAP")
            End If

            'plant style
            If rbOpt03.Checked Then
                sSort = CType(rbOpt03.Tag, String)
                .Add(New SqlClient.SqlParameter("@Plant_Style_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psSTYLE_CAP")
            End If

            'Plant color
            If rbOpt04.Checked Then
                sSort = CType(rbOpt04.Tag, String)
                .Add(New SqlClient.SqlParameter("@Plant_Color_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psCOLOR_CAP")
            End If

            .Add(New SqlClient.SqlParameter("@Sort_On", SqlDbType.NVarChar)).Value = sSort

        End With

    End Sub
    Private Sub subBuildChangeFieldParams(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description:  Select the change report fields that go into the stored procedure
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        With s.Parameters

            .Add(New SqlClient.SqlParameter("@Zone_Cap", _
                 SqlDbType.NVarChar)).Value = gcsRM.GetString("csZONE_CAP")
            .Add(New SqlClient.SqlParameter("@Area_Cap", _
                SqlDbType.NVarChar)).Value = gcsRM.GetString("csAREA_CAP")
            .Add(New SqlClient.SqlParameter("@PWUser_Cap", _
                SqlDbType.NVarChar)).Value = gcsRM.GetString("csUSER_CAP")
            .Add(New SqlClient.SqlParameter("@Device_Cap", _
                SqlDbType.NVarChar)).Value = gcsRM.GetString("csROBOT_CAP")
            .Add(New SqlClient.SqlParameter("@Parameter_Cap", _
                SqlDbType.NVarChar)).Value = gcsRM.GetString("csPARAMETER_CAP")
            .Add(New SqlClient.SqlParameter("@Description_Cap", _
                SqlDbType.NVarChar)).Value = gcsRM.GetString("csSPECIFIC_CAP")
            .Add(New SqlClient.SqlParameter("@Time_Cap", _
                SqlDbType.NVarChar)).Value = gcsRM.GetString("csTIME_CAP")
            '.Add(New SqlClient.SqlParameter("@Item_Cap", _
            '    SqlDbType.NVarChar)).Value = gpsRM.GetString("psITEM_CAP")


        End With
    End Sub
    Private Sub subBuildChangeCriteriaParams(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description:  Add the parameters that may be changed from the checked list boxes
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sSel As String

        With s.Parameters
            'zone box
            sSel = sGetCLBSelections(msZONELISTBOX)
            .Add(New SqlClient.SqlParameter("@Zone", _
                SqlDbType.NVarChar)).Value = sSel

            msTitles(mnZONE) = sGetTitleCaption(gcsRM.GetString("csZONE_CAP"), sSel)

            'area box
            sSel = sGetCLBTagSelections(msSCREENLISTBOX)
            .Add(New SqlClient.SqlParameter("@Area", _
                SqlDbType.NVarChar)).Value = sSel

            msTitles(4) = sGetTitleCaption(gpsRM.GetString("psSCREENS_CAP"), sSel)

            'user box
            sSel = sGetCLBSelections(msUSERLISTBOX)
            .Add(New SqlClient.SqlParameter("@PWUser", _
                SqlDbType.NVarChar)).Value = sSel

            msTitles(3) = sGetTitleCaption(gcsRM.GetString("csUSER_CAP"), sSel)

            'device box
            sSel = sGetCLBSelections(msDEVICELISTBOX)
            .Add(New SqlClient.SqlParameter("@Device", _
                    SqlDbType.NVarChar)).Value = sSel

            msTitles(5) = sGetTitleCaption(gcsRM.GetString("csROBOT_CAP"), sSel)

            'param box
            sSel = sGetCLBSelections(msPARAMLISTBOX)
            .Add(New SqlClient.SqlParameter("@Parameter", _
                 SqlDbType.NVarChar)).Value = sSel

            msTitles(6) = sGetTitleCaption(gcsRM.GetString("csPARAMETER_CAP"), sSel)

            'times
            sSel = String.Empty

            .Add(New SqlClient.SqlParameter("@StartQuery_Time", _
                SqlDbType.DateTime)).Value = StartTime

            sSel = StartTime.ToString
            msTitles(mnSTART) = sGetTitleCaption(gpsRM.GetString("psSTART"), sSel)

            .Add(New SqlClient.SqlParameter("@EndQuery_Time", _
                SqlDbType.DateTime)).Value = EndTime

            sSel = EndTime.ToString
            msTitles(mnEND) = sGetTitleCaption(gpsRM.GetString("psEND"), sSel)

            If mbShowDaily then
              If chkDaily.Enabled And chkDaily.Checked Then
                  .Add(New SqlClient.SqlParameter("@Daily", _
                      SqlDbType.Bit)).Value = 1
              Else
                  .Add(New SqlClient.SqlParameter("@Daily", _
                      SqlDbType.Bit)).Value = 0
              End If
            End If
        End With

    End Sub
    Private Sub subBuildChangeProcedure(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description:  build stored procedure command and title strings for the change report
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        subBuildChangeCriteriaParams(s)

        'column captions
        subBuildChangeFieldParams(s)


    End Sub
    Private Sub subBuildChangeSummProcedure(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description: Get change summary parameters
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        ' add the parameters selected from the listboxes
        subBuildChangeCriteriaParams(s)

        With s.Parameters

            'column captions

            .Add(New SqlClient.SqlParameter("@Area_Cap", _
                    SqlDbType.NVarChar)).Value = gcsRM.GetString("csAREA_CAP")
            .Add(New SqlClient.SqlParameter("@Device_Cap", _
                    SqlDbType.NVarChar)).Value = gcsRM.GetString("csROBOT_CAP")
            .Add(New SqlClient.SqlParameter("@PWUser_Cap", _
                    SqlDbType.NVarChar)).Value = gcsRM.GetString("csUSER_CAP")
            .Add(New SqlClient.SqlParameter("@Total_Cap", SqlDbType.NVarChar)).Value = _
                    gpsRM.GetString("psOCCURRENCES_CAP")

            ' sort on
            Dim sSort As String = String.Empty

            If rbOpt01.Checked Then sSort = CType(rbOpt01.Tag, String)
            If rbOpt02.Checked Then sSort = CType(rbOpt02.Tag, String)
            If rbOpt03.Checked Then sSort = CType(rbOpt03.Tag, String)

            .Add(New SqlClient.SqlParameter("@Sort_On", SqlDbType.NVarChar)).Value = sSort

        End With

    End Sub
    Private Sub subBuildProductionFieldParams(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description:  Select the production report fields that go into the stored procedure
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/08/10  MSW     subBuildProductionFieldParams - Add sequence number, needed for summaries
        '********************************************************************************************
        With s.Parameters


            'Carrier
            If bShowColumn(eShowCol.Carrier) Then
                .Add(New SqlClient.SqlParameter("@Carrier_Number_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psCARRIER_CAP")
            End If

            'VIN
            If bShowColumn(eShowCol.VIN) Then
                .Add(New SqlClient.SqlParameter("@VIN_Number_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psVIN_CAP")
            End If

            'Sequence
            .Add(New SqlClient.SqlParameter("@Sequence_Number_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psSEQUENCE_CAP")


            'Device
            .Add(New SqlClient.SqlParameter("@Device_Cap", _
                SqlDbType.NVarChar)).Value = gcsRM.GetString("csROBOT_CAP")

            'Plant style
            .Add(New SqlClient.SqlParameter("@Plant_Style_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psSTYLE_CAP")

            'Plant Color
            If bShowColumn(eShowCol.Color) Then
                .Add(New SqlClient.SqlParameter("@Plant_Color_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psCOLOR_CAP")
            End If

            'Option
            If bShowColumn(eShowCol.Optioncol) Then
                .Add(New SqlClient.SqlParameter("@Plant_Option_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psOPTION_CAP")
            End If

            'Tutone
            If bShowColumn(eShowCol.Tutone) Then
                .Add(New SqlClient.SqlParameter("@Plant_Tutone_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psTUTONE_CAP")
            End If


            'repairs
            If bShowColumn(eShowCol.Repair) Then
                .Add(New SqlClient.SqlParameter("@Plant_Repair_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psPLANT_REPAIR_CAP")
                .Add(New SqlClient.SqlParameter("@Robot_Repair_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psROBOT_REPAIR_CAP")
            End If

            'Color Change Status
            If bShowColumn(eShowCol.Purge) Then
                .Add(New SqlClient.SqlParameter("@Purge_Status_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psPURGE_STATUS_CAP")
            End If

            '' Job Status
            '.Add(New SqlClient.SqlParameter("@Completion_Status_Cap", _
            '    SqlDbType.NVarChar)).Value = gpsRM.GetString("psJOB_STATUS_CAP") & " Text"
            'MSW 5/03/12 Numeric Completion Status
            ' Job Status
            .Add(New SqlClient.SqlParameter("@Completion_Status_Numeric_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psJOB_STATUS_CAP")


            'degrade
            If bShowColumn(eShowCol.Degrade) Then
                .Add(New SqlClient.SqlParameter("@Degrade_Status_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psDEGRADE_STATUS_CAP")
            End If

            'Ghost
            If bShowColumn(eShowCol.Ghost) Then
                .Add(New SqlClient.SqlParameter("@Ghost_Status_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psGHOST_STATUS_CAP")
            End If

            ' Cycle Time
            .Add(New SqlClient.SqlParameter("@Cycle_Time_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psCYCLE_TIME_CAP")


            'Paint Total
            If bShowColumn(eShowCol.PaintTotal) Then
                .Add(New SqlClient.SqlParameter("@Paint_Total_1_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psPAINT_TOTAL_CAP")
            End If

            'Paint Total 2
            If bShowColumn(eShowCol.PaintTotal2) Then
                .Add(New SqlClient.SqlParameter("@Paint_Total_2_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psPAINT_TOTAL2_CAP")
            End If

            'material Learned
            If bShowColumn(eShowCol.MaterialLearned) Then
                .Add(New SqlClient.SqlParameter("@Material_Learned_1_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psMATERIAL_LEARNED_CAP")
            End If

            'material Learned 2
            If bShowColumn(eShowCol.MaterialLearned2) Then
                .Add(New SqlClient.SqlParameter("@Material_Learned_2_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psMATERIAL_LEARNED2_CAP")
            End If


            'material Temp
            If bShowColumn(eShowCol.MaterialTemp) Then
                .Add(New SqlClient.SqlParameter("@Material_Temperature_1_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psMATERIAL_TEMP_CAP")
            End If

            'material Temp 2
            If bShowColumn(eShowCol.MaterialTemp2) Then
                .Add(New SqlClient.SqlParameter("@Material_Temperature_2_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psMATERIAL_TEMP2_CAP")
            End If

            'Init String
            If bShowColumn(eShowCol.InitString) Then
                .Add(New SqlClient.SqlParameter("@Init_String_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psINIT_STRING_CAP")
            End If

            'CC Time
            If bShowColumn(eShowCol.CCTime) Then
                .Add(New SqlClient.SqlParameter("@Color_Change_Time_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psCC_TIME_CAP")
            End If

            'Index Time
            If bShowColumn(eShowCol.IndexTime) Then
                .Add(New SqlClient.SqlParameter("@Index_Time_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psINDEX_TIME_CAP")
            End If

            'Canister tpsu
            If bShowColumn(eShowCol.TPSU) Then
                .Add(New SqlClient.SqlParameter("@Cannister_TPSU_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psCANNISTER_TPSU_CAP")
            End If

            'Zone
            .Add(New SqlClient.SqlParameter("@Zone_Cap", _
                SqlDbType.NVarChar)).Value = gcsRM.GetString("csZONE_CAP")

            'Ratio
            If bShowColumn(eShowCol.Ratio) Then
                .Add(New SqlClient.SqlParameter("@Ratio_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psRATIO_CAP")
            End If

            'Complete time
            .Add(New SqlClient.SqlParameter("@Complete_Time_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psCOMPLETE_TIME_CAP")


        End With

    End Sub
    Private Sub subBuildProductionProcedure(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description:  build stored procedure command and title strings for the Production report
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sSel As String
        Dim sAll As String = gcsRM.GetString("csALL")


        With s.Parameters

            'VIN Number
            If bShowColumn(eShowCol.VIN) Then
                Dim c As ComboBox = DirectCast(tlpMain.Controls(msVINCOMBOBOX), ComboBox)
                .Add(New SqlClient.SqlParameter("@VIN_Number", _
                        SqlDbType.NVarChar)).Value = c.Text
            End If

            'Carrier Number
            If bShowColumn(eShowCol.Carrier) Then
                Dim c As ComboBox = DirectCast(tlpMain.Controls(msCARRIERCOMBOBOX), ComboBox)
                .Add(New SqlClient.SqlParameter("@Carrier_Number", _
                        SqlDbType.NVarChar)).Value = c.Text
            End If

            'device box
            sSel = sGetCLBSelections(msDEVICELISTBOX)
            .Add(New SqlClient.SqlParameter("@Device", _
                    SqlDbType.NVarChar)).Value = sSel

            msTitles(5) = sGetTitleCaption(gcsRM.GetString("csROBOT_CAP"), sSel)

            'style Number
            sSel = sGetCLBTagSelections(msSTYLELISTBOX)
            .Add(New SqlClient.SqlParameter("@Plant_Style", _
                        SqlDbType.NVarChar)).Value = sSel

            msTitles(3) = sGetTitleCaption(gpsRM.GetString("psSTYLE_CAP"), sSel)

            'Color Number
            If bShowColumn(eShowCol.Color) Then
                sSel = sGetCLBTagSelections(msCOLORLISTBOX)
                .Add(New SqlClient.SqlParameter("@Plant_Color", _
                            SqlDbType.NVarChar)).Value = sSel

                msTitles(4) = sGetTitleCaption(gpsRM.GetString("psCOLOR_CAP"), sSel)
            End If

            'Option 
            If bShowColumn(eShowCol.Optioncol) Then
                '3/7/14 CBZ add sort by sealer style/option
                If mbUseCombinedStyleOption And sSel <> "All" Then
                    sSel = sGetCLBIndexSelections(msSTYLELISTBOX)
                    If sSel = "All" Then
                        sSel = sGetCLBTagSelections(msOPTIONLISTBOX)
                    Else
                        Dim sStyles As String() = Split(sSel, ",", 99)
                        Dim oS As clsSysStyle = Nothing
                        Dim oStyles As New clsSysStyles(colZones.ActiveZone)
                        Dim sTmp() As String
                        ReDim sTmp(0)
                        For i As Integer = 0 To sStyles.Length - 1
                            oS = oStyles.Item(CInt(sStyles(i)) - 1)
                            ReDim Preserve sTmp(i)
                            sTmp(i) = oS.OptionNumber.Text
                        Next
                        sSel = Strings.Join(sTmp, ",")
                    End If
                Else
                sSel = sGetCLBTagSelections(msOPTIONLISTBOX)
                End If
                .Add(New SqlClient.SqlParameter("@Plant_Option", _
                        SqlDbType.NVarChar)).Value = sSel

                msTitles(6) = sGetTitleCaption(gpsRM.GetString("psOPTION_CAP"), sSel)
            End If

            'Tutone 
            If bShowColumn(eShowCol.Tutone) Then
                sSel = sGetCLBTagSelections(msTUTONELISTBOX)
                .Add(New SqlClient.SqlParameter("@Plant_Tutone", _
                        SqlDbType.NVarChar)).Value = sAll 'sSel

                'msTitles(6) = sGetTitleCaption(gpsRM.GetString("psTUTONE_CAP"), sSel)
            End If

            'Repair 
            If bShowColumn(eShowCol.Repair) Then
                .Add(New SqlClient.SqlParameter("@Plant_Repair", _
                        SqlDbType.NVarChar)).Value = sAll
                .Add(New SqlClient.SqlParameter("@Robot_Repair", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'color change cycles 
            If bShowColumn(eShowCol.Purge) Then
                .Add(New SqlClient.SqlParameter("@Purge_Status", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            ''Job Status
            'sSel = sGetCLBTagSelections(msJOBSTATUSLISTBOX)
            '.Add(New SqlClient.SqlParameter("@Completion_Status", _
            '        SqlDbType.NVarChar)).Value = sSel

            'msTitles(7) = sGetTitleCaption(gpsRM.GetString("psJOB_STATUS_CAP"), sSel)
            'MSW 5/03/12 Numeric Completion Status
            'Job Status
            sSel = sGetCLBTagSelectionsMasked(msJOBSTATUSLISTBOX)
            .Add(New SqlClient.SqlParameter("@Completion_Status_Numeric", _
                    SqlDbType.NVarChar)).Value = sSel

            msTitles(7) = sGetTitleCaption(gpsRM.GetString("psJOB_STATUS_CAP"), sSel)

            'Degrade 
            If bShowColumn(eShowCol.Degrade) Then
                .Add(New SqlClient.SqlParameter("@Degrade_Status", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Ghost
            If bShowColumn(eShowCol.Ghost) Then
                .Add(New SqlClient.SqlParameter("@Ghost_Status", _
                    SqlDbType.NVarChar)).Value = sAll
            End If
            'Cycle Time
            .Add(New SqlClient.SqlParameter("@Cycle_Time", _
                    SqlDbType.NVarChar)).Value = sAll


            'Paint totals 
            If bShowColumn(eShowCol.PaintTotal) Then
                .Add(New SqlClient.SqlParameter("@Paint_Total_1", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Paint totals 2
            If bShowColumn(eShowCol.PaintTotal2) Then
                .Add(New SqlClient.SqlParameter("@Paint_Total_2", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Material Learned
            If bShowColumn(eShowCol.MaterialLearned) Then
                .Add(New SqlClient.SqlParameter("@Material_Learned_1", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Material Learned 2
            If bShowColumn(eShowCol.MaterialLearned2) Then
                .Add(New SqlClient.SqlParameter("@Material_Learned_2", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Material Temp
            If bShowColumn(eShowCol.MaterialTemp) Then
                .Add(New SqlClient.SqlParameter("@Material_Temperature_1", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Material Temp 2
            If bShowColumn(eShowCol.MaterialTemp2) Then
                .Add(New SqlClient.SqlParameter("@Material_Temperature_2", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Init String
            If bShowColumn(eShowCol.InitString) Then
                .Add(New SqlClient.SqlParameter("@Init_String", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Color Change Time
            If bShowColumn(eShowCol.CCTime) Then
                .Add(New SqlClient.SqlParameter("@Color_Change_Time", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Index Time
            If bShowColumn(eShowCol.IndexTime) Then
                .Add(New SqlClient.SqlParameter("@Index_Time", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Cannister TPSU
            If bShowColumn(eShowCol.TPSU) Then
                .Add(New SqlClient.SqlParameter("@Cannister_TPSU", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Ratio
            If bShowColumn(eShowCol.Ratio) Then
                .Add(New SqlClient.SqlParameter("@Ratio", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'zone box
            sSel = sGetCLBSelections(msZONELISTBOX)
            .Add(New SqlClient.SqlParameter("@Zone", _
                SqlDbType.NVarChar)).Value = sSel

            msTitles(mnZONE) = sGetTitleCaption(gcsRM.GetString("csZONE_CAP"), sSel)


            'times  --------------------------------------------------------------------

            sSel = String.Empty

            .Add(New SqlClient.SqlParameter("@StartQuery_Time", _
                SqlDbType.DateTime)).Value = StartTime

            sSel = StartTime.ToString
            msTitles(mnSTART) = sGetTitleCaption(gpsRM.GetString("psSTART"), sSel)

            .Add(New SqlClient.SqlParameter("@EndQuery_Time", _
                SqlDbType.DateTime)).Value = EndTime

            sSel = EndTime.ToString
            msTitles(mnEND) = sGetTitleCaption(gpsRM.GetString("psEND"), sSel)


            If mbShowDaily then
              If chkDaily.Enabled And chkDaily.Checked Then
                   .Add(New SqlClient.SqlParameter("@Daily", _
                      SqlDbType.Bit)).Value = 1
              Else
                  .Add(New SqlClient.SqlParameter("@Daily", _
                      SqlDbType.Bit)).Value = 0
              End If
            End If

        End With

        'column captions -------------------------------------------------------
        subBuildProductionFieldParams(s)

    End Sub
    Private Sub subBuildProductionHourSummProcedure(ByRef s As SqlClient.SqlCommand, _
                                                            ByVal sSort As String)
        '********************************************************************************************
        'Description:  build the how many per hour production report summary
        '               NOTE: all the params are here if we want to use in the future, but now we
        '               Are just using the time
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sSel As String
        Dim sAll As String = gcsRM.GetString("csALL")

        With s.Parameters

            If bShowColumn(eShowCol.Carrier) Then
                'carrier
                .Add(New SqlClient.SqlParameter("@Carrier_Number", _
                            SqlDbType.NVarChar)).Value = sAll
            End If

            'VIN Number
            If bShowColumn(eShowCol.VIN) Then
                Dim c As ComboBox = DirectCast(tlpMain.Controls(msVINCOMBOBOX), ComboBox)
                .Add(New SqlClient.SqlParameter("@VIN_Number", _
                        SqlDbType.NVarChar)).Value = c.Text
            End If

            'device box
            sSel = sGetCLBSelections(msDEVICELISTBOX)
            .Add(New SqlClient.SqlParameter("@Device", _
                    SqlDbType.NVarChar)).Value = sSel

            msTitles(5) = sGetTitleCaption(gcsRM.GetString("csROBOT_CAP"), sSel)

            'style Number
            sSel = sGetCLBTagSelections(msSTYLELISTBOX)
            .Add(New SqlClient.SqlParameter("@Plant_Style", _
                        SqlDbType.NVarChar)).Value = sSel

            msTitles(3) = sGetTitleCaption(gpsRM.GetString("psSTYLE_CAP"), sSel)

            'Color Number
            If bShowColumn(eShowCol.Color) Then
                sSel = sGetCLBTagSelections(msCOLORLISTBOX)
                .Add(New SqlClient.SqlParameter("@Plant_Color", _
                            SqlDbType.NVarChar)).Value = sSel

                msTitles(4) = sGetTitleCaption(gpsRM.GetString("psCOLOR_CAP"), sSel)
            End If

            'Option 
            If bShowColumn(eShowCol.Optioncol) Then
                '3/7/14 CBZ add sort by sealer style/option
                If mbUseCombinedStyleOption And sSel <> "All" Then
                    sSel = sGetCLBIndexSelections(msSTYLELISTBOX)
                    If sSel = "All" Then
                sSel = sGetCLBTagSelections(msOPTIONLISTBOX)
                    Else
                        Dim sStyles As String() = Split(sSel, ",", 99)
                        Dim oS As clsSysStyle = Nothing
                        Dim oStyles As New clsSysStyles(colZones.ActiveZone)
                        Dim sTmp() As String
                        ReDim sTmp(0)
                        For i As Integer = 0 To sStyles.Length - 1
                            oS = oStyles.Item(CInt(sStyles(i)) - 1)
                            ReDim Preserve sTmp(i)
                            sTmp(i) = oS.OptionNumber.Text
                        Next
                        sSel = Strings.Join(sTmp, ",")
                    End If
                Else
                    sSel = sGetCLBTagSelections(msOPTIONLISTBOX)
                End If
                .Add(New SqlClient.SqlParameter("@Plant_Option", _
                        SqlDbType.NVarChar)).Value = sSel

                msTitles(6) = sGetTitleCaption(gpsRM.GetString("psOPTION_CAP"), sSel)
            End If

            'Tutone 
            If bShowColumn(eShowCol.Tutone) Then
                sSel = sGetCLBTagSelections(msTUTONELISTBOX)
                .Add(New SqlClient.SqlParameter("@Plant_Tutone", _
                        SqlDbType.NVarChar)).Value = sAll 'sSel

                'msTitles(6) = sGetTitleCaption(gpsRM.GetString("psTUTONE_CAP"), sSel)
            End If

            'Repair 
            If bShowColumn(eShowCol.Repair) Then
                .Add(New SqlClient.SqlParameter("@Plant_Repair", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'color change cycles 
            If bShowColumn(eShowCol.Purge) Then
                .Add(New SqlClient.SqlParameter("@Purge_Status", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            ''Job Status
            'sSel = sGetCLBTagSelections(msJOBSTATUSLISTBOX)
            '.Add(New SqlClient.SqlParameter("@Completion_Status", _
            '        SqlDbType.NVarChar)).Value = sSel

            'msTitles(7) = sGetTitleCaption(gpsRM.GetString("psJOB_STATUS_CAP"), sSel)
            'MSW 5/03/12 Numeric Completion Status
            'Job Status
            sSel = sGetCLBTagSelectionsMasked(msJOBSTATUSLISTBOX)
            .Add(New SqlClient.SqlParameter("@Completion_Status_Numeric", _
                    SqlDbType.NVarChar)).Value = sSel

            msTitles(7) = sGetTitleCaption(gpsRM.GetString("psJOB_STATUS_CAP"), sSel)

            'Degrade 
            If bShowColumn(eShowCol.Degrade) Then
                .Add(New SqlClient.SqlParameter("@Degrade_Status", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Ghost
            If bShowColumn(eShowCol.Ghost) Then
                .Add(New SqlClient.SqlParameter("@Ghost_Status", _
                    SqlDbType.NVarChar)).Value = sAll
            End If
            'Cycle Time
            .Add(New SqlClient.SqlParameter("@Cycle_Time", _
                    SqlDbType.NVarChar)).Value = sAll


            'Paint totals 
            If bShowColumn(eShowCol.PaintTotal) Then
                .Add(New SqlClient.SqlParameter("@Paint_Total_1", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Paint totals 2
            If bShowColumn(eShowCol.PaintTotal2) Then
                .Add(New SqlClient.SqlParameter("@Paint_Total_2", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Material Learned
            If bShowColumn(eShowCol.MaterialLearned) Then
                .Add(New SqlClient.SqlParameter("@Material_Learned_1", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Material Learned 2
            If bShowColumn(eShowCol.MaterialLearned2) Then
                .Add(New SqlClient.SqlParameter("@Material_Learned_2", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Material Temp
            If bShowColumn(eShowCol.MaterialTemp) Then
                .Add(New SqlClient.SqlParameter("@Material_Temperature_1", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Material Temp 2
            If bShowColumn(eShowCol.MaterialTemp2) Then
                .Add(New SqlClient.SqlParameter("@Material_Temperature_2", _
                        SqlDbType.NVarChar)).Value = sAll
            End If


            'Color Change Time
            If bShowColumn(eShowCol.CCTime) Then
                .Add(New SqlClient.SqlParameter("@Color_Change_Time", _
                        SqlDbType.NVarChar)).Value = sAll
            End If


            'Index Time
            If bShowColumn(eShowCol.IndexTime) Then
                .Add(New SqlClient.SqlParameter("@Index_Time", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Cannister TPSU
            If bShowColumn(eShowCol.TPSU) Then
                .Add(New SqlClient.SqlParameter("@Cannister_TPSU", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Ratio
            If bShowColumn(eShowCol.Ratio) Then
                .Add(New SqlClient.SqlParameter("@Ratio", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'zone box
            sSel = sGetCLBSelections(msZONELISTBOX)
            .Add(New SqlClient.SqlParameter("@Zone", _
                SqlDbType.NVarChar)).Value = sSel

            msTitles(mnZONE) = sGetTitleCaption(gcsRM.GetString("csZONE_CAP"), sSel)


            'times  --------------------------------------------------------------------

            sSel = String.Empty

            .Add(New SqlClient.SqlParameter("@StartQuery_Time", _
                SqlDbType.DateTime)).Value = StartTime

            sSel = StartTime.ToString
            msTitles(mnSTART) = sGetTitleCaption(gpsRM.GetString("psSTART"), sSel)

            .Add(New SqlClient.SqlParameter("@EndQuery_Time", _
                SqlDbType.DateTime)).Value = EndTime

            sSel = EndTime.ToString
            msTitles(mnEND) = sGetTitleCaption(gpsRM.GetString("psEND"), sSel)


            'column captions -------------------------------------------------------
            subBuildProductionFieldParams(s)

            'sort on
            Select Case sSort
                Case gpsRM.GetString("psCOLOR_CAP")
                    .Add(New SqlClient.SqlParameter("@Sort_On", _
                        SqlDbType.NVarChar)).Value = gpsRM.GetString("psCOLOR_CAP")
                Case gpsRM.GetString("psSTYLE_CAP")
                    .Add(New SqlClient.SqlParameter("@Sort_On", _
                       SqlDbType.NVarChar)).Value = gpsRM.GetString("psSTYLE_CAP")
                Case gpsRM.GetString("psPAINT_TOTAL_CAP")
                    .Add(New SqlClient.SqlParameter("@Sort_On", _
                      SqlDbType.NVarChar)).Value = gpsRM.GetString("psPAINT_TOTAL_CAP")
            End Select

            If mbShowDaily then
              If chkDaily.Enabled And chkDaily.Checked Then
                  .Add(New SqlClient.SqlParameter("@Daily", _
                      SqlDbType.Bit)).Value = 1
              Else
                  .Add(New SqlClient.SqlParameter("@Daily", _
                      SqlDbType.Bit)).Value = 0
              End If
            End If
        End With

    End Sub
    Private Sub subBuildProductionJobSummProcedure(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description:  build stored procedure command and title strings for the Production report
        '               This is the Job overview report 1 record per job
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sSel As String
        Dim sAll As String = gcsRM.GetString("csALL")

        With s.Parameters

            If bShowColumn(eShowCol.Carrier) Then
                'carrier
                .Add(New SqlClient.SqlParameter("@Carrier_Number", _
                            SqlDbType.NVarChar)).Value = sAll
            End If

            'VIN Number
            If bShowColumn(eShowCol.VIN) Then
                Dim c As ComboBox = DirectCast(tlpMain.Controls(msVINCOMBOBOX), ComboBox)
                .Add(New SqlClient.SqlParameter("@VIN_Number", _
                        SqlDbType.NVarChar)).Value = c.Text
            End If

            'device box
            sSel = sGetCLBSelections(msDEVICELISTBOX)
            .Add(New SqlClient.SqlParameter("@Device", _
                    SqlDbType.NVarChar)).Value = sSel

            msTitles(5) = sGetTitleCaption(gcsRM.GetString("csROBOT_CAP"), sSel)

            'style Number
            sSel = sGetCLBTagSelections(msSTYLELISTBOX)
            .Add(New SqlClient.SqlParameter("@Plant_Style", _
                        SqlDbType.NVarChar)).Value = sSel

            msTitles(3) = sGetTitleCaption(gpsRM.GetString("psSTYLE_CAP"), sSel)

            'Color Number
            If bShowColumn(eShowCol.Color) Then
                sSel = sGetCLBTagSelections(msCOLORLISTBOX)
                .Add(New SqlClient.SqlParameter("@Plant_Color", _
                            SqlDbType.NVarChar)).Value = sSel

                msTitles(4) = sGetTitleCaption(gpsRM.GetString("psCOLOR_CAP"), sSel)
            End If

            'Option 
            If bShowColumn(eShowCol.Optioncol) Then
                '3/7/14 CBZ add sort by sealer style/option
                If mbUseCombinedStyleOption And sSel <> "All" Then
                    sSel = sGetCLBIndexSelections(msSTYLELISTBOX)
                    If sSel = "All" Then
                        sSel = sGetCLBTagSelections(msOPTIONLISTBOX)
                    Else
                        Dim sStyles As String() = Split(sSel, ",", 99)
                        Dim oS As clsSysStyle = Nothing
                        Dim oStyles As New clsSysStyles(colZones.ActiveZone)
                        Dim sTmp() As String
                        ReDim sTmp(0)
                        For i As Integer = 0 To sStyles.Length - 1
                            oS = oStyles.Item(CInt(sStyles(i)) - 1)
                            ReDim Preserve sTmp(i)
                            sTmp(i) = oS.OptionNumber.Text
                        Next
                        sSel = Strings.Join(sTmp, ",")
                    End If
                Else
                sSel = sGetCLBTagSelections(msOPTIONLISTBOX)
                End If
                .Add(New SqlClient.SqlParameter("@Plant_Option", _
                        SqlDbType.NVarChar)).Value = sSel

                msTitles(6) = sGetTitleCaption(gpsRM.GetString("psOPTION_CAP"), sSel)
            End If

            'Tutone 
            If bShowColumn(eShowCol.Tutone) Then
                sSel = sGetCLBTagSelections(msTUTONELISTBOX)
                .Add(New SqlClient.SqlParameter("@Plant_Tutone", _
                        SqlDbType.NVarChar)).Value = sAll 'sSel

                'msTitles(6) = sGetTitleCaption(gpsRM.GetString("psTUTONE_CAP"), sSel)
            End If

            'Repair 
            If bShowColumn(eShowCol.Repair) Then
                .Add(New SqlClient.SqlParameter("@Plant_Repair", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'color change cycles 
            If bShowColumn(eShowCol.Purge) Then
                .Add(New SqlClient.SqlParameter("@Purge_Status", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            ''Job Status
            'sSel = sGetCLBTagSelections(msJOBSTATUSLISTBOX)
            '.Add(New SqlClient.SqlParameter("@Completion_Status", _
            '        SqlDbType.NVarChar)).Value = sSel

            'msTitles(7) = sGetTitleCaption(gpsRM.GetString("psJOB_STATUS_CAP"), sSel)
            'MSW 5/03/12 Numeric Completion Status
            'Job Status
            sSel = sGetCLBTagSelectionsMasked(msJOBSTATUSLISTBOX)
            .Add(New SqlClient.SqlParameter("@Completion_Status_Numeric", _
                    SqlDbType.NVarChar)).Value = sSel

            msTitles(7) = sGetTitleCaption(gpsRM.GetString("psJOB_STATUS_CAP"), sSel)

            'Degrade 
            If bShowColumn(eShowCol.Degrade) Then
                .Add(New SqlClient.SqlParameter("@Degrade_Status", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Ghost
            If bShowColumn(eShowCol.Ghost) Then
                .Add(New SqlClient.SqlParameter("@Ghost_Status", _
                    SqlDbType.NVarChar)).Value = sAll
            End If

            'Cycle Time
            .Add(New SqlClient.SqlParameter("@Cycle_Time", _
                    SqlDbType.NVarChar)).Value = sAll


            'Paint totals 
            If bShowColumn(eShowCol.PaintTotal) Then
                .Add(New SqlClient.SqlParameter("@Paint_Total_1", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Paint totals 2
            If bShowColumn(eShowCol.PaintTotal2) Then
                .Add(New SqlClient.SqlParameter("@Paint_Total_2", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Material Learned
            If bShowColumn(eShowCol.MaterialLearned) Then
                .Add(New SqlClient.SqlParameter("@Material_Learned_1", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Material Learned 2
            If bShowColumn(eShowCol.MaterialLearned2) Then
                .Add(New SqlClient.SqlParameter("@Material_Learned_2", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Material Temp
            If bShowColumn(eShowCol.MaterialTemp) Then
                .Add(New SqlClient.SqlParameter("@Material_Temperature_1", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Material Temp 2
            If bShowColumn(eShowCol.MaterialTemp2) Then
                .Add(New SqlClient.SqlParameter("@Material_Temperature_2", _
                        SqlDbType.NVarChar)).Value = sAll
            End If


            'Color Change Time
            If bShowColumn(eShowCol.CCTime) Then
                .Add(New SqlClient.SqlParameter("@Color_Change_Time", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Index Time
            If bShowColumn(eShowCol.IndexTime) Then
                .Add(New SqlClient.SqlParameter("@Index_Time", _
                        SqlDbType.NVarChar)).Value = sAll
            End If


            'Cannister TPSU
            If bShowColumn(eShowCol.TPSU) Then
                .Add(New SqlClient.SqlParameter("@Cannister_TPSU", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'Ratio
            If bShowColumn(eShowCol.Ratio) Then
                .Add(New SqlClient.SqlParameter("@Ratio", _
                        SqlDbType.NVarChar)).Value = sAll
            End If

            'zone box
            sSel = sGetCLBSelections(msZONELISTBOX)
            .Add(New SqlClient.SqlParameter("@Zone", _
                SqlDbType.NVarChar)).Value = sSel

            msTitles(mnZONE) = sGetTitleCaption(gcsRM.GetString("csZONE_CAP"), sSel)


            'times  --------------------------------------------------------------------

            sSel = String.Empty

            .Add(New SqlClient.SqlParameter("@StartQuery_Time", _
                SqlDbType.DateTime)).Value = StartTime

            sSel = StartTime.ToString
            msTitles(mnSTART) = sGetTitleCaption(gpsRM.GetString("psSTART"), sSel)

            .Add(New SqlClient.SqlParameter("@EndQuery_Time", _
                SqlDbType.DateTime)).Value = EndTime

            sSel = EndTime.ToString
            msTitles(mnEND) = sGetTitleCaption(gpsRM.GetString("psEND"), sSel)


            'column captions -------------------------------------------------------
            subBuildProductionFieldParams(s)



            If mbShowDaily then
              If chkDaily.Enabled And chkDaily.Checked Then
                  .Add(New SqlClient.SqlParameter("@Daily", _
                      SqlDbType.Bit)).Value = 1
              Else
                  .Add(New SqlClient.SqlParameter("@Daily", _
                      SqlDbType.Bit)).Value = 0
              End If
            End If

            ' for debug-------------------------------------------------------------------------

            '.Add(New SqlClient.SqlParameter("@Sequence_Number_Cap", SqlDbType.NVarChar)).Value = "Sequence Number"

        End With

    End Sub
    Private Sub subBuildTOPAlarmProcedure(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description:  get most recent records
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'column captions 
        subBuildAlarmFieldParams(s)
        s.Parameters.Add(New SqlClient.SqlParameter("@Number_Of_Records", SqlDbType.Int)).Value = _
                CType(udQuick.Value, Integer)
    End Sub
    Private Sub subBuildTOPChangeProcedure(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description:  get most recent records
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'column captions 
        subBuildChangeFieldParams(s)
        s.Parameters.Add(New SqlClient.SqlParameter("@Number_Of_Records", SqlDbType.Int)).Value = _
                CType(udQuick.Value, Integer)

    End Sub
    Private Sub subBuildTOPProductionProcedure(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description:  get most recent records
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'column captions 
        subBuildProductionFieldParams(s)
        s.Parameters.Add(New SqlClient.SqlParameter("@Number_Of_Records", SqlDbType.Int)).Value = _
                CType(udQuick.Value, Integer)


    End Sub
    Private Sub subBuildVisionProcedure(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description:  build stored procedure command and title strings for the vision report
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sSel As String
        Dim sAll As String = gcsRM.GetString("csALL")


        With s.Parameters

            'VIN Number
            If bShowColumn(eShowCol.VIN) Then
                Dim c As ComboBox = DirectCast(tlpMain.Controls(msVINCOMBOBOX), ComboBox)
                .Add(New SqlClient.SqlParameter("@VIN_Number", _
                        SqlDbType.NVarChar)).Value = c.Text
            End If

            'Carrier Number
            If bShowColumn(eShowCol.Carrier) Then
                Dim c As ComboBox = DirectCast(tlpMain.Controls(msCARRIERCOMBOBOX), ComboBox)
                .Add(New SqlClient.SqlParameter("@Carrier_Number", _
                        SqlDbType.NVarChar)).Value = c.Text
            End If

            'device box
            sSel = sGetCLBSelections(msDEVICELISTBOX)
            .Add(New SqlClient.SqlParameter("@Device", _
                    SqlDbType.NVarChar)).Value = sSel

            msTitles(5) = sGetTitleCaption(gcsRM.GetString("csROBOT_CAP"), sSel)

            'style Number
            sSel = sGetCLBTagSelections(msSTYLELISTBOX)
            .Add(New SqlClient.SqlParameter("@Plant_Style", _
                        SqlDbType.NVarChar)).Value = sSel

            msTitles(3) = sGetTitleCaption(gpsRM.GetString("psSTYLE_CAP"), sSel)

            'Option 
            If bShowColumn(eShowCol.Optioncol) Then
                sSel = sGetCLBTagSelections(msOPTIONLISTBOX)
                .Add(New SqlClient.SqlParameter("@Plant_Option", _
                        SqlDbType.NVarChar)).Value = sSel

                msTitles(6) = sGetTitleCaption(gpsRM.GetString("psOPTION_CAP"), sSel)
            End If

            'Vision Status
            sSel = sGetCLBTagSelections(msVISIONTATUSLISTBOX)
            .Add(New SqlClient.SqlParameter("@Vision_Status_Numeric", _
                    SqlDbType.NVarChar)).Value = sSel

            msTitles(7) = sGetTitleCaption(gpsRM.GetString("psVISION_STATUS_CAP"), sSel)

            'zone box
            sSel = sGetCLBSelections(msZONELISTBOX)
            .Add(New SqlClient.SqlParameter("@Zone", _
                SqlDbType.NVarChar)).Value = sSel

            msTitles(mnZONE) = sGetTitleCaption(gcsRM.GetString("csZONE_CAP"), sSel)


            'times  --------------------------------------------------------------------

            sSel = String.Empty

            .Add(New SqlClient.SqlParameter("@StartQuery_Time", _
                SqlDbType.DateTime)).Value = StartTime

            sSel = StartTime.ToString
            msTitles(mnSTART) = sGetTitleCaption(gpsRM.GetString("psSTART"), sSel)

            .Add(New SqlClient.SqlParameter("@EndQuery_Time", _
                SqlDbType.DateTime)).Value = EndTime

            sSel = EndTime.ToString
            msTitles(mnEND) = sGetTitleCaption(gpsRM.GetString("psEND"), sSel)



            If mbShowDaily then
              If chkDaily.Enabled And chkDaily.Checked Then
                  .Add(New SqlClient.SqlParameter("@Daily", _
                      SqlDbType.Bit)).Value = 1
              Else
                  .Add(New SqlClient.SqlParameter("@Daily", _
                      SqlDbType.Bit)).Value = 0
              End If
            End If
        End With

        'column captions -------------------------------------------------------
        subBuildVisionFieldParams(s)

    End Sub

    Private Sub subBuildVisionSummProcedure(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description:  build stored procedure command and title strings for the vision report
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sSel As String
        Dim sAll As String = gcsRM.GetString("csALL")


        With s.Parameters

            'VIN Number
            If bShowColumn(eShowCol.VIN) Then
                Dim c As ComboBox = DirectCast(tlpMain.Controls(msVINCOMBOBOX), ComboBox)
                .Add(New SqlClient.SqlParameter("@VIN_Number", _
                        SqlDbType.NVarChar)).Value = c.Text
            End If

            'Carrier Number
            If bShowColumn(eShowCol.Carrier) Then
                Dim c As ComboBox = DirectCast(tlpMain.Controls(msCARRIERCOMBOBOX), ComboBox)
                .Add(New SqlClient.SqlParameter("@Carrier_Number", _
                        SqlDbType.NVarChar)).Value = c.Text
            End If

            'device box
            sSel = sGetCLBSelections(msDEVICELISTBOX)
            .Add(New SqlClient.SqlParameter("@Device", _
                    SqlDbType.NVarChar)).Value = sSel

            msTitles(5) = sGetTitleCaption(gcsRM.GetString("csROBOT_CAP"), sSel)

            'style Number
            sSel = sGetCLBTagSelections(msSTYLELISTBOX)
            .Add(New SqlClient.SqlParameter("@Plant_Style", _
                        SqlDbType.NVarChar)).Value = sSel

            msTitles(3) = sGetTitleCaption(gpsRM.GetString("psSTYLE_CAP"), sSel)

            'Option 
            If bShowColumn(eShowCol.Optioncol) Then
                sSel = sGetCLBTagSelections(msOPTIONLISTBOX)
                .Add(New SqlClient.SqlParameter("@Plant_Option", _
                        SqlDbType.NVarChar)).Value = sSel

                msTitles(6) = sGetTitleCaption(gpsRM.GetString("psOPTION_CAP"), sSel)
            End If

            'Vision Status
            sSel = sGetCLBTagSelections(msVISIONTATUSLISTBOX)
            .Add(New SqlClient.SqlParameter("@Vision_Status_Numeric", _
                    SqlDbType.NVarChar)).Value = sSel

            msTitles(7) = sGetTitleCaption(gpsRM.GetString("psVISION_STATUS_CAP"), sSel)

            'zone box
            sSel = sGetCLBSelections(msZONELISTBOX)
            .Add(New SqlClient.SqlParameter("@Zone", _
                SqlDbType.NVarChar)).Value = sSel

            msTitles(mnZONE) = sGetTitleCaption(gcsRM.GetString("csZONE_CAP"), sSel)


            'times  --------------------------------------------------------------------

            sSel = String.Empty

            .Add(New SqlClient.SqlParameter("@StartQuery_Time", _
                SqlDbType.DateTime)).Value = StartTime

            sSel = StartTime.ToString
            msTitles(mnSTART) = sGetTitleCaption(gpsRM.GetString("psSTART"), sSel)

            .Add(New SqlClient.SqlParameter("@EndQuery_Time", _
                SqlDbType.DateTime)).Value = EndTime

            sSel = EndTime.ToString
            msTitles(mnEND) = sGetTitleCaption(gpsRM.GetString("psEND"), sSel)



            'column captions -------------------------------------------------------
            subBuildVisionFieldParams(s)

            'sort on
            Dim sSort As String = String.Empty
            If rbOpt01.Checked Then sSort = rbOpt01.Tag.ToString
            If rbOpt02.Checked Then sSort = rbOpt02.Tag.ToString

            Select Case sSort
                Case gpsRM.GetString("psSTYLE_CAP")
                    .Add(New SqlClient.SqlParameter("@Sort_On", _
                       SqlDbType.NVarChar)).Value = gpsRM.GetString("psSTYLE_CAP")
                Case gpsRM.GetString("psVISION_STATUS_CAP")
                    .Add(New SqlClient.SqlParameter("@Sort_On", _
                       SqlDbType.NVarChar)).Value = gpsRM.GetString("psVISION_STATUS_CAP")
            End Select

            If mbShowDaily then
              If chkDaily.Enabled And chkDaily.Checked Then
                  .Add(New SqlClient.SqlParameter("@Daily", _
                      SqlDbType.Bit)).Value = 1
              Else
                  .Add(New SqlClient.SqlParameter("@Daily", _
                      SqlDbType.Bit)).Value = 0
              End If
            End If
        End With
    End Sub

    Private Sub subBuildVisionFieldParams(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description:  Select the Vision report fields that go into the stored procedure
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        With s.Parameters


            'Carrier
            If bShowColumn(eShowCol.Carrier) Then
                .Add(New SqlClient.SqlParameter("@Carrier_Number_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psCARRIER_CAP")
            End If

            'VIN
            If bShowColumn(eShowCol.VIN) Then
                .Add(New SqlClient.SqlParameter("@VIN_Number_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psVIN_CAP")
            End If

            'Device
            .Add(New SqlClient.SqlParameter("@Device_Cap", _
                SqlDbType.NVarChar)).Value = gcsRM.GetString("csROBOT_CAP")

            'Plant style
            .Add(New SqlClient.SqlParameter("@Plant_Style_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psSTYLE_CAP")

            'Option
            If bShowColumn(eShowCol.Optioncol) Then
                .Add(New SqlClient.SqlParameter("@Plant_Option_Cap", _
                    SqlDbType.NVarChar)).Value = gpsRM.GetString("psOPTION_CAP")
            End If

            ' Vision Status
            .Add(New SqlClient.SqlParameter("@Vision_Status_Numeric_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psVISION_STATUS_CAP")

            'Zone
            .Add(New SqlClient.SqlParameter("@Zone_Cap", _
                SqlDbType.NVarChar)).Value = gcsRM.GetString("csZONE_CAP")



            'Complete time
            .Add(New SqlClient.SqlParameter("@Complete_Time_Cap", _
                SqlDbType.NVarChar)).Value = gpsRM.GetString("psCOMPLETE_TIME_CAP")


        End With

    End Sub

    Private Sub subBuildTOPVisionProcedure(ByRef s As SqlClient.SqlCommand)
        '********************************************************************************************
        'Description:  get most recent records
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'column captions 
        subBuildVisionFieldParams(s)
        s.Parameters.Add(New SqlClient.SqlParameter("@Number_Of_Records", SqlDbType.Int)).Value = _
                CType(udQuick.Value, Integer)


    End Sub
    Private Sub subCLBCheckHandler(ByVal sender As Object, _
                                                ByVal e As System.Windows.Forms.ItemCheckEventArgs)
        '********************************************************************************************
        'Description:  if all is selected, deselect others, if other is selected deselect all
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/15/12  HGB     Update the device list based on the zone(s) selected.
        '********************************************************************************************
        Dim clb As CheckedListBox = DirectCast(sender, CheckedListBox)

        If e.NewValue = CheckState.Checked Then
            If e.Index = 0 Then
                'all always first, or you write the code
                For i As Integer = 1 To clb.Items.Count - 1
                    clb.SetItemChecked(i, False)
                Next
            Else
                'some other item - deselect all
                clb.SetItemChecked(0, False)
            End If
        End If
        'HGB - handle the 'zone change' and update the device list
        If ZoneCollection.StandAlone AndAlso clb.Name = msZONELISTBOX Then
            subUpdateDeviceList(sender, e)
        End If

    End Sub
    Private Sub subChangeZone()
        '********************************************************************************************
        'Description:  set up for zone
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/15/12  HGB     Don't load the robot collection here for SA, we will do it from the zone 
        '                   checklistbox.
        '********************************************************************************************
        Try

            If cboZone.Text = String.Empty Then Exit Sub

            If Not colZones.StandAlone Then 'HGB 11/15/12
                subSetZoneForRobotCollections(cboZone.Text)
            End If

            subGetShiftTimes()
            subGetColumnConfig()

        Catch ex As Exception

        End Try

    End Sub
    Private Function bGetXMLBoolVal(ByRef oNode As XmlNode, ByRef sName As String, ByVal bDefault As Boolean) As Boolean
        Try
            Return CType(oNode.Item(sName).InnerXml, Boolean)
        Catch ex As Exception
            Return bDefault
        End Try
    End Function

    Private Sub subGetColumnConfig()
        '********************************************************************************************
        'Description:  What are you going to show me?
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Move to XML
        '********************************************************************************************
        Const sXMLFILE As String = "ReportConfig"
        Const sXMLTABLE As String = "ReportParameters"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Dim bSuccess As Boolean = True
        Dim nNode As Integer = 0
        Dim nIndex As Integer = 0
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLFILE & ".XML") Then
            Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)

                If bGetXMLBoolVal(oMainNode, "ShowSequenceColumn", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.Sequence
                End If
                If bGetXMLBoolVal(oMainNode, "ShowCarrierColumn", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.Carrier
                End If
                If bGetXMLBoolVal(oMainNode, "ShowVINColumn", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.VIN
                End If
                If bGetXMLBoolVal(oMainNode, "ShowColorColumn", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.Color
                End If
                If bGetXMLBoolVal(oMainNode, "ShowOptionColumn", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.Optioncol
                End If
                If bGetXMLBoolVal(oMainNode, "ShowTutoneColumn", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.Tutone
                End If
                If bGetXMLBoolVal(oMainNode, "ShowRepairColumn", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.Repair
                End If
                If bGetXMLBoolVal(oMainNode, "ShowPurgeStatusColumn", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.Purge
                End If
                If bGetXMLBoolVal(oMainNode, "ShowDegradeColumn", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.Degrade
                End If
                If bGetXMLBoolVal(oMainNode, "ShowGhostColumn", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.Ghost
                End If
                If bGetXMLBoolVal(oMainNode, "ShowPaintTotalsColumn", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.PaintTotal
                End If
                If bGetXMLBoolVal(oMainNode, "ShowPaintTotals2Column", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.PaintTotal2
                End If
                If bGetXMLBoolVal(oMainNode, "ShowRatioColumn", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.Ratio
                End If
                If bGetXMLBoolVal(oMainNode, "ShowMaterialLearnedColumn", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.MaterialLearned
                End If
                If bGetXMLBoolVal(oMainNode, "ShowMaterialLearned2Column", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.MaterialLearned2
                End If
                If bGetXMLBoolVal(oMainNode, "ShowMaterialTempColumn", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.MaterialTemp
                End If
                If bGetXMLBoolVal(oMainNode, "ShowMaterialTemp2Column", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.MaterialTemp2
                End If
                If bGetXMLBoolVal(oMainNode, "ShowInitStringColumn", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.InitString
                End If
                If bGetXMLBoolVal(oMainNode, "ShowCCTimeColumn", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.CCTime
                End If
                If bGetXMLBoolVal(oMainNode, "ShowIndexTimeColumn", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.IndexTime
                End If
                If bGetXMLBoolVal(oMainNode, "ShowTPSUColumn", False) Then
                    mlShowColumns = mlShowColumns Or eShowCol.TPSU
                End If

            Catch ex As Exception

                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmMain" & " Routine: subGetColumnConfig", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

            End Try
        End If



    End Sub
    Private Sub subGetConvAlarmConfig()
        '********************************************************************************************
        'Description:  Fill the conveyor alarm structure - this reads the table in the config
        '               database and relys on the descriptions not to change - just the alarm numbers
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Move to XML
        '********************************************************************************************
        Const sXMLFILE As String = "ReportConfig"
        Const sXMLTABLE As String = "ReportConveyorAlarms"
        Const sXMLNODE As String = "ConvItem"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Dim bSuccess As Boolean = True
        Dim nNode As Integer = 0
        Dim nIndex As Integer = 0
        ReDim frmMain.gtConvAlarms.HoldForHome(6)
        ReDim frmMain.gtConvAlarms.Bypassed(6)
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLFILE & ".XML") Then
            Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)
                With frmMain.gtConvAlarms
                    For Each oNode As XmlNode In oNodeList
                        Debug.Print(oNode.Item("Description").InnerXml)
                        Select Case oNode.Item("Description").InnerXml
                            Case "Held By Fanuc"
                                .HeldByFanuc = oNode.Item("Alarm").InnerXml
                            Case "Held By Fanuc OS"
                                .HeldByFanucOS = oNode.Item("Alarm").InnerXml
                            Case "Held By Others"
                                .HeldByOthers = oNode.Item("Alarm").InnerXml
                            Case "Held By Others OS"
                                .HeldByOthersOS = oNode.Item("Alarm").InnerXml
                            Case "Running"
                                .Running = oNode.Item("Alarm").InnerXml
                            Case "Running OS"
                                .RunningOS = oNode.Item("Alarm").InnerXml
                            Case "Break Active"
                                .BreakActive = oNode.Item("Alarm").InnerXml
                            Case "Break Active OS"
                                .BreakActiveOS = oNode.Item("Alarm").InnerXml
                            Case "EQ1 Hold Until Home"
                                .HoldForHome(1) = oNode.Item("Alarm").InnerXml
                            Case "EQ2 Hold Until Home"
                                .HoldForHome(2) = oNode.Item("Alarm").InnerXml
                            Case "EQ3 Hold Until Home"
                                .HoldForHome(3) = oNode.Item("Alarm").InnerXml
                            Case "EQ4 Hold Until Home"
                                .HoldForHome(4) = oNode.Item("Alarm").InnerXml
                            Case "EQ5 Hold Until Home"
                                .HoldForHome(5) = oNode.Item("Alarm").InnerXml
                            Case "EQ6 Hold Until Home"
                                .HoldForHome(6) = oNode.Item("Alarm").InnerXml
                            Case "EQ1 Bypassed"
                                .Bypassed(1) = oNode.Item("Alarm").InnerXml
                            Case "EQ2 Bypassed"
                                .Bypassed(2) = oNode.Item("Alarm").InnerXml
                            Case "EQ3 Bypassed"
                                .Bypassed(3) = oNode.Item("Alarm").InnerXml
                            Case "EQ4 Bypassed"
                                .Bypassed(4) = oNode.Item("Alarm").InnerXml
                            Case "EQ5 Bypassed"
                                .Bypassed(5) = oNode.Item("Alarm").InnerXml
                            Case "EQ6 Bypassed"
                                .Bypassed(6) = oNode.Item("Alarm").InnerXml
                        End Select

                    Next
                End With
            Catch ex As Exception

                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & "frmMain" & " Routine: subGetColumnConfig", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

            End Try
        End If



    End Sub

    Private Sub subSetZoneForRobotCollections(ByVal Zonename As String)
        '********************************************************************************************
        'Description:  set up for zone
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '10/05/12  HGB      Load all zones for SA
        '********************************************************************************************
        'this is wasteful but its easy
        colZones.CurrentZone = cboZone.Text
        colControllers = New clsControllers(colZones, colZones.StandAlone)
        colArms = mPWRobotCommon.LoadArmCollection(colControllers)

    End Sub
    Private Sub subUpdateDeviceList(ByVal sender As Object, _
                                    ByVal e As System.Windows.Forms.ItemCheckEventArgs)
        '********************************************************************************************
        'Description:  Update the device list on a zone change
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '10/05/12   HGB    Created
        '********************************************************************************************
        Dim clbZone As CheckedListBox = DirectCast(sender, CheckedListBox)
        Dim bZones() As Boolean
        ReDim bZones(1) ' keep the compiler happy
        If clbZone.Items.Count > 0 Then
            Dim nLoop As Integer
            For nLoop = 0 To clbZone.Items.Count - 1
                ReDim Preserve bZones(nLoop)
                If nLoop = e.Index Then
                    bZones(nLoop) = (e.NewValue = CheckState.Checked)
                Else
                    bZones(nLoop) = clbZone.GetItemChecked(nLoop)
                End If

            Next

            Select Case mReportType

                Case frmMain.eReportType.Change, frmMain.eReportType.Alarm
                    Dim clb As CheckedListBox = TryCast(tlpMain.Controls(msDEVICELISTBOX), CheckedListBox)
                    If clb IsNot Nothing Then
                        subLoadDeviceBoxSA(clb, bZones, True, False)
                    End If

                Case frmMain.eReportType.Production

                Case frmMain.eReportType.Downtime

                Case frmMain.eReportType.Conveyor

                Case frmMain.eReportType.RMCharts

                Case frmMain.eReportType.RMChartsAuto

                Case Else
                    Debug.Assert(False)
            End Select
        End If

    End Sub

    Private Function sGetTitleCaption(ByVal Caption As String, ByVal sDataIn As String) As String
        '********************************************************************************************
        'Description:  construct string for title cap
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sData() As String
        Dim sTmp As String = Caption & ": "

        sData = Split(sDataIn, ",")

        If UBound(sData) = 0 Then
            sTmp = sTmp & sData(0)
        Else
            sTmp = sTmp & gpsRM.GetString("psUSER_SELECTED")
        End If

        Return sTmp

    End Function
    Private Function sGetCLBSelections(ByVal clbName As String) As String
        '********************************************************************************************
        'Description:  Get a text array of the selections in the listboxes
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '09/12/08   gks     change from returning array to return comma separated string
        '********************************************************************************************
        Dim s() As String
        ReDim s(0)
        Dim i As Integer = 0

        Dim clb As CheckedListBox = TryCast(tlpMain.Controls(clbName), CheckedListBox)
        If clb Is Nothing Then Return String.Empty

        'this also returns indeterminate states. should not be any
        For Each o As Object In clb.CheckedItems
            ReDim Preserve s(i)

            'special case for alarmnumber box, need wildcards
            Select Case clbName
                Case msALARMNUMLISTBOX
                    If o.ToString <> gcsRM.GetString("csALL") Then
                        s(i) = "%" & o.ToString & "%"
                    Else
                        s(i) = o.ToString
                    End If

                Case Else
                    s(i) = o.ToString

            End Select

            i += 1
        Next

        Dim sRet As String = Strings.Join(s, ",")

        Return sRet

    End Function
    Private Function sGetCLBTagSelectionsMasked(ByVal clbName As String) As String
        '********************************************************************************************
        'Description:  using the selections in the listbox, get the items stored in the tag array
        '           return combined bitmask instead of list
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTag As String()
        Dim nMask As Integer = 0
        Dim nNotMask As Integer = 0
        Dim clb As CheckedListBox = TryCast(tlpMain.Controls(clbName), CheckedListBox)
        If clb Is Nothing Then Return String.Empty

        Try
            sTag = CType(clb.Tag, String())
            For i As Integer = 0 To clb.Items.Count - 1
                If clb.GetItemChecked(i) Then
                    If i = 0 Then
                        ' all - special case
                        Return gcsRM.GetString("csALL")
                    End If
                    Try
                        nMask = nMask Or CType(sTag(i), Integer)
                    Catch ex As Exception
                    End Try
                End If
            Next

            Return nMask.ToString

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return String.Empty
        End Try


    End Function
    Private Function sGetCLBTagSelections(ByVal clbName As String) As String
        '********************************************************************************************
        'Description:  using the selections in the listbox, get the items stored in the tag array
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim s() As String
        ReDim s(0)
        Dim nPtr As Integer = 0
        Dim sTag As String()

        Dim clb As CheckedListBox = TryCast(tlpMain.Controls(clbName), CheckedListBox)
        If clb Is Nothing Then Return String.Empty

        Try
            sTag = CType(clb.Tag, String())
            For i As Integer = 0 To clb.Items.Count - 1
                If clb.GetItemChecked(i) Then
                    ReDim Preserve s(nPtr)
                    If i = 0 Then
                        ' all - special case
                        s(nPtr) = gcsRM.GetString("csALL")
                        Return s(nPtr)
                    End If

                    s(nPtr) = sTag(i)
                    nPtr += 1
                End If
            Next

            Dim sRet As String = Strings.Join(s, ",")

            Return sRet

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return String.Empty
        End Try

    End Function

    Private Function sGetCLBIndexSelections(ByVal clbName As String) As String
        '********************************************************************************************
        'Description:  using the selections in the listbox, get the items index
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim s() As String
        ReDim s(0)
        Dim nPtr As Integer = 0
        Dim sTag As String()

        Dim clb As CheckedListBox = TryCast(tlpMain.Controls(clbName), CheckedListBox)
        If clb Is Nothing Then Return String.Empty

        Try
            sTag = CType(clb.Tag, String())
            For i As Integer = 0 To clb.Items.Count - 1
                If clb.GetItemChecked(i) Then
                    ReDim Preserve s(nPtr)
                    If i = 0 Then
                        ' all - special case
                        s(nPtr) = gcsRM.GetString("csALL")
                        Return s(nPtr)
                    End If

                    s(nPtr) = i.ToString
                    nPtr += 1
                End If
            Next

            Dim sRet As String = Strings.Join(s, ",")

            Return sRet

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return String.Empty
        End Try

    End Function

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
        ' 01/11/11  RJO     Modified code to adjust start and end date for shift wrap. 
        '********************************************************************************************

        Try

            mcolShifts = New clsShifts(colZones.ActiveZone)

            btnShift1.Enabled = mcolShifts.Item(1).Enabled
            btnShift2.Enabled = mcolShifts.Item(2).Enabled
            btnShift3.Enabled = mcolShifts.Item(3).Enabled
            If Not (mbExternalTimeSet) Then
                For i As Integer = 1 To 3
                    If mcolShifts(i).IsActive Then
                        dtpStartTime.Value = mcolShifts(i).StartTime
                        dtpEndTime.Value = mcolShifts(i).EndTime
                        If dtpEndTime.Value.Hour < dtpStartTime.Value.Hour Then 'RJO 01/11/2011
                            'Check to see if its the 2nd day of the shift and adjust the start date if necessary
                            If Now.Hour < dtpStartTime.Value.Hour Then
                                dtpStartDate.Value = DateAdd(DateInterval.Day, -1, Now)
                            End If
                            dtpEndDate.Value = DateAdd(DateInterval.Day, 1, dtpStartDate.Value)
                        Else
                            'Otherwise, keep it the same as the start date
                            dtpStartDate.Value = Now
                            dtpEndDate.Value = dtpStartDate.Value
                        End If
                        Exit For
                    End If
                Next
            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub
    Private Sub subInitializeForm()
        '********************************************************************************************
        'Description:  Set me up
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/15/12  HGB     Load the default robot collections for SA
        '********************************************************************************************

        Try
            subInitFormText()
            If frmMain.gbTimePickersInited = False Then
            subInitDTPickers()
                frmMain.gbTimePickersInited = True
            End If
            subChangeZone()
            If colZones.StandAlone Then 'HGB 11/15/12
                subSetZoneForRobotCollections(colZones.CurrentZone)
            End If

            Select Case mReportType

                Case frmMain.eReportType.Change
                    subSetupChangeCriteria()
                Case frmMain.eReportType.Alarm
                    subSetupAlarmCriteria()
                Case frmMain.eReportType.Production
                    subSetupProductionCriteria()
                Case frmMain.eReportType.Downtime
                    subSetupDownTimeCriteria()
                Case frmMain.eReportType.Conveyor
                    subSetupConveyorCriteria()
                Case frmMain.eReportType.RMCharts
                    subSetupChartCriteria()
                Case frmMain.eReportType.RMChartsAuto
                    subSetupChartCriteria()
                Case frmMain.eReportType.Vision
                    subSetupVisionCriteria()
                Case Else
                    Debug.Assert(False)
            End Select

            For Each sTmp As String In My.Application.CommandLineArgs
                If sTmp.ToLower = "autogenbutton" Then
                    btnAutoGen.Visible = True
                End If
                If sTmp.ToLower = "shift1" Then
                    btnShift_Click(btnShift1)
                End If
                If sTmp.ToLower = "shift2" Then
                    btnShift_Click(btnShift2)
                End If
                If sTmp.ToLower = "shift3" Then
                    btnShift_Click(btnShift3)
                End If
                If sTmp.ToLower = "autorun" Then
                    mbAutoRun = True
                End If
            Next

            mbScreenLoaded = True

        Catch ex As Exception

            Trace.WriteLine(ex.Message)

        End Try

    End Sub
    Private Sub subInitFormText()
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

        With gcsRM

            btnOK.Text = .GetString("csOK")
            btnCancel.Text = .GetString("csCANCEL")

        End With

        With gpsRM

            lblStartCap.Text = .GetString("psSTART")
            lblEndCap.Text = .GetString("psEND")

            gpbShift.Text = .GetString("psSHIFT_BOX_CAP")
            btnShift1.Text = .GetString("psSHIFT1")
            btnShift2.Text = .GetString("psSHIFT2")
            btnShift3.Text = .GetString("psSHIFT3")

            gpbOptions.Text = .GetString("psOPTIONS")
            chkSummary.Text = .GetString("psSUMMARY_REPORT")
            chkSummary.Visible = True
            chkQuick.Text = .GetString("psMOST_RECENT")
            udQuick.Left = chkQuick.Left + chkQuick.Width + 4
            Me.Text = .GetString("psSELECT_CRITERIA")
            btnAutoGen.Text = .GetString("psAUTO_GEN")
        End With

    End Sub
    Private Sub subInitDTPickers()
        '********************************************************************************************
        'Description:  initial setup of dtpickers
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim d As Date = DateAdd(DateInterval.Hour, -1, Now)

        'for debugging with old data can use so dont have to set dates every time
        If mbDEBUGDATES Then
            d = mdDATE
        End If

        If Not mbExternalTimeSet Then
            ' initially go back an hour. set to shift time after
            dtpStartDate.Value = d
            dtpStartTime.Value = d

            d = Now
            'for debugging with old data can use so dont have to set dates every time
            If mbDEBUGDATES Then
                d = mdDATE
            End If

            dtpEndDate.Value = d
            dtpEndTime.Value = d
        End If

        'min values
        d = DateAdd(DateInterval.Year, -1, Now)
        dtpStartDate.MinDate = d
        dtpStartTime.MinDate = d
        dtpEndDate.MinDate = d
        dtpEndTime.MinDate = d

        'max values
        d = DateAdd(DateInterval.Day, 2, Now)
        dtpStartDate.MaxDate = d
        dtpStartTime.MaxDate = d
        dtpEndDate.MaxDate = d
        dtpEndTime.MaxDate = d

    End Sub
    Private Sub subLoadDeviceBoxSA(ByRef lb As CheckedListBox, ByVal bZones() As Boolean, ByVal bAddControllers As Boolean, Optional ByVal bAddArms As Boolean = True)
        '********************************************************************************************
        'Description:  get and fill the Device listbox
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/05/12  HGB     Created
        '********************************************************************************************
        Try
            lb.Items.Clear()
            lb.Items.Add(gcsRM.GetString("csALL"))
            lb.SetItemChecked(0, True)

            If bZones(0) Then 'All was selected
                colControllers = New clsControllers(ZoneCollection, True)
            Else
                Dim sSavedZone As String = colZones.CurrentZone
                Dim nLoop As Integer
                colControllers = Nothing
                For nLoop = 1 To bZones.GetUpperBound(0)
                    If bZones(nLoop) Then
                        colZones.CurrentZone = colZones(nLoop).Name
                        If colControllers Is Nothing Then
                            colControllers = New clsControllers(colZones, False)
                        Else
                            Dim cc As New clsControllers(colZones, False)
                            For Each cont As clsController In cc
                                colControllers.Add(cont)
                            Next
                        End If
                    End If
                Next
                If colControllers IsNot Nothing Then
                    colArms = mPWRobotCommon.LoadArmCollection(colControllers)
                End If
                colZones.CurrentZone = sSavedZone
            End If
            If bAddControllers AndAlso colControllers IsNot Nothing Then
                With lb
                    For Each c As clsController In colControllers
                        .Items.Add(c.Name)
                    Next
                End With
            End If



        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub

    Private Sub subSetupAlarmCriteria()
        '********************************************************************************************
        'Description:  Set me up some more
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        subSetUpTLP(frmMain.eReportType.Alarm)

        Dim l As Label = oGetLabel()
        l.Text = gcsRM.GetString("csZONE_CAP")
        tlpMain.Controls.Add(l, 0, 0)

        Dim lb As CheckedListBox = oGetZoneBox()
        AddHandler lb.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb, 0, 1)

        Dim l1 As Label = oGetLabel()
        l1.Text = gcsRM.GetString("csROBOT_CAP")
        tlpMain.Controls.Add(l1, 1, 0)

        Dim lb1 As CheckedListBox = oGetDeviceBox(True, False)
        AddHandler lb1.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb1, 1, 1)

        Dim l2 As Label = oGetLabel()
        l2.Text = gpsRM.GetString("psSEVERITY_CAP")
        tlpMain.Controls.Add(l2, 2, 0)

        Dim lb2 As CheckedListBox = oGetSeverityBox()
        AddHandler lb2.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb2, 2, 1)

        Dim l3 As Label = oGetLabel()
        l3.Text = gpsRM.GetString("psCATEGORY_CAP")
        tlpMain.Controls.Add(l3, 0, 2)

        Dim lb3 As CheckedListBox = oGetCategoryBox()
        AddHandler lb3.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb3, 0, 3)

        Dim l4 As Label = oGetLabel()
        l4.Text = gpsRM.GetString("psALARM_CAP")
        tlpMain.Controls.Add(l4, 1, 2)

        Dim lb4 As CheckedListBox = oGetAlarmBox()
        AddHandler lb4.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb4, 1, 3)

        If bShowColumn(eShowCol.VIN) Then

            Dim l5 As Label = oGetLabel()
            l5.Text = gpsRM.GetString("psVIN_CAP")
            tlpMain.Controls.Add(l5, 2, 2)

            Dim c1 As ComboBox = oGetVINBox()
            AddHandler c1.LostFocus, AddressOf cboBox_LostFocusHandler
            tlpMain.Controls.Add(c1, 2, 3)

        End If

        If bShowColumn(eShowCol.Carrier) Then

            Dim l6 As Label = oGetLabel()
            l6.Text = gpsRM.GetString("psCARRIER_CAP")
            tlpMain.Controls.Add(l6, 3, 2)

            Dim c2 As ComboBox = oGetCarrierBox()
            AddHandler c2.LostFocus, AddressOf cboBox_LostFocusHandler
            tlpMain.Controls.Add(c2, 3, 3)

        End If

        chkHBO.Visible = False
        chkQuick.Visible = True
        udQuick.Visible = True
        If chkQuick.Enabled = False Then
            udQuick.Value = 50
        End If
        ' setup option radio buttons for summary reports
        rbOpt01.Text = gpsRM.GetString("psALARM_CAP")
        rbOpt01.Tag = gpsRM.GetString("psALARM_CAP")
        rbOpt01.Visible = True

        rbOpt02.Text = gcsRM.GetString("csROBOT_CAP")
        rbOpt02.Tag = gcsRM.GetString("csROBOT_CAP")
        rbOpt02.Visible = True

        rbOpt03.Text = gpsRM.GetString("psSTYLE_CAP")
        rbOpt03.Tag = gpsRM.GetString("psSTYLE_CAP")
        rbOpt03.Visible = True

        If bShowColumn(eShowCol.Color) Then
            rbOpt04.Text = gpsRM.GetString("psCOLOR_CAP")
            rbOpt04.Tag = gpsRM.GetString("psCOLOR_CAP")
            rbOpt04.Visible = True
        Else
            rbOpt04.Text = String.Empty
            rbOpt04.Tag = String.Empty
            rbOpt04.Visible = False
        End If

        rbOpt05.Text = String.Empty
        rbOpt05.Tag = String.Empty
        rbOpt05.Visible = False

        rbOpt06.Text = String.Empty
        rbOpt06.Tag = String.Empty
        rbOpt06.Visible = False

        rbOpt07.Text = String.Empty
        rbOpt07.Tag = String.Empty
        rbOpt07.Visible = False

        rbOpt08.Text = String.Empty
        rbOpt08.Tag = String.Empty
        rbOpt08.Visible = False

        rbOpt01.Checked = True

        chkDaily.Text = gpsRM.GetString("psDAILY")
        chkDaily.Visible = mbShowDaily
    End Sub
    Private Sub subSetupChangeCriteria()
        '********************************************************************************************
        'Description:  Set me up some more
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        subSetUpTLP(frmMain.eReportType.Change)

        Dim l As Label = oGetLabel()
        l.Text = gcsRM.GetString("csZONE_CAP")
        tlpMain.Controls.Add(l, 0, 0)

        Dim lb As CheckedListBox = oGetZoneBox()
        AddHandler lb.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb, 0, 1)

        Dim l1 As Label = oGetLabel()
        l1.Text = gpsRM.GetString("psSCREENS_CAP")
        tlpMain.Controls.Add(l1, 1, 0)

        Dim lb1 As CheckedListBox = oGetScreenBox()
        AddHandler lb1.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb1, 1, 1)


        Dim l2 As Label = oGetLabel()
        l2.Text = gcsRM.GetString("csUSER_CAP")
        tlpMain.Controls.Add(l2, 2, 0)

        Dim lb2 As CheckedListBox = oGetUserBox()
        AddHandler lb2.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb2, 2, 1)


        Dim l3 As Label = oGetLabel()
        l3.Text = gcsRM.GetString("csROBOT_CAP")
        tlpMain.Controls.Add(l3, 0, 2)

        Dim lb3 As CheckedListBox = oGetDeviceBox(True, True)
        AddHandler lb3.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb3, 0, 3)


        Dim l4 As Label = oGetLabel()
        l4.Text = gcsRM.GetString("csPARAMETER_CAP")
        tlpMain.Controls.Add(l4, 1, 2)

        Dim lb4 As CheckedListBox = oGetParameterBox()
        AddHandler lb4.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb4, 1, 3)

        chkHBO.Visible = False
        chkQuick.Visible = True
        udQuick.Visible = True
        If chkQuick.Enabled = False Then
            udQuick.Value = 50
        End If

        ' setup option radio buttons for summary reports
        rbOpt01.Visible = True
        rbOpt02.Visible = True
        rbOpt03.Visible = True
        rbOpt04.Visible = False
        rbOpt05.Visible = False
        rbOpt06.Visible = False
        rbOpt07.Visible = False
        rbOpt08.Visible = False

        rbOpt01.Text = gcsRM.GetString("csUSER_CAP")
        rbOpt02.Text = gpsRM.GetString("psSCREENS_CAP")
        rbOpt03.Text = gcsRM.GetString("csROBOT_CAP")
        rbOpt04.Text = String.Empty
        rbOpt05.Text = String.Empty
        rbOpt06.Text = String.Empty
        rbOpt07.Text = String.Empty
        rbOpt08.Text = String.Empty

        rbOpt01.Tag = gcsRM.GetString("csUSER_CAP")
        rbOpt02.Tag = gcsRM.GetString("csAREA_CAP")
        rbOpt03.Tag = gcsRM.GetString("csROBOT_CAP")
        rbOpt04.Tag = String.Empty
        rbOpt05.Tag = String.Empty
        rbOpt06.Tag = String.Empty
        rbOpt07.Tag = String.Empty
        rbOpt08.Tag = String.Empty

        rbOpt01.Checked = True

        chkDaily.Text = gpsRM.GetString("psDAILY")
        chkDaily.Visible = mbShowDaily

    End Sub
    Private Sub subSetupConveyorCriteria()
        '********************************************************************************************
        'Description:  Set me up some more
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        subSetUpTLP(frmMain.eReportType.Conveyor)

        Call subGetConvAlarmConfig()

        Dim l As Label = oGetLabel()
        l.Text = gcsRM.GetString("csZONE_CAP")
        tlpMain.Controls.Add(l, 0, 0)

        Dim lb As CheckedListBox = oGetZoneBox()
        AddHandler lb.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb, 0, 1)

        chkHBO.Visible = False
        chkQuick.Visible = True
        udQuick.Visible = True
        If chkQuick.Enabled = False Then
            udQuick.Value = 50
        End If

        ' setup option radio buttons for summary reports
        rbOpt01.Text = String.Empty
        rbOpt01.Tag = String.Empty
        rbOpt01.Visible = False

        rbOpt02.Text = String.Empty
        rbOpt02.Tag = String.Empty
        rbOpt02.Visible = False

        rbOpt03.Text = String.Empty
        rbOpt03.Tag = String.Empty
        rbOpt03.Visible = False

        rbOpt04.Text = String.Empty
        rbOpt04.Tag = String.Empty
        rbOpt04.Visible = False

        rbOpt05.Text = String.Empty
        rbOpt05.Tag = String.Empty
        rbOpt05.Visible = False

        rbOpt06.Text = String.Empty
        rbOpt06.Tag = String.Empty
        rbOpt06.Visible = False

        rbOpt07.Text = String.Empty
        rbOpt07.Tag = String.Empty
        rbOpt07.Visible = False

        rbOpt08.Text = String.Empty
        rbOpt08.Tag = String.Empty
        rbOpt08.Visible = False

        rbOpt01.Checked = True

    End Sub
    Friend Sub subSetupChartCriteria()
        '********************************************************************************************
        'Description:  Set me up some more
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        subSetUpTLP(frmMain.eReportType.RMCharts)
        Dim bFound As Boolean = False
        For Each oControl As Control In tlpMain.Controls
            If oControl.Text = gcsRM.GetString("csZONE_CAP") Then
                bFound = True
            End If
        Next
        If (bFound <> True) Then
            Dim l As Label = oGetLabel()
            l.Text = gcsRM.GetString("csZONE_CAP")
            tlpMain.Controls.Add(l, 0, 0)

            Dim lb As CheckedListBox = oGetZoneBox()
            AddHandler lb.ItemCheck, AddressOf subCLBCheckHandler
            tlpMain.Controls.Add(lb, 0, 1)
        End If
        frmMain.subGetDTMask()

        'Include held by others
        chkHBO.Text = gpsRM.GetString("psINCLUDE_HBO")
        chkHBO.Visible = True
        'enable top x
        chkQuick.Visible = True
        udQuick.Visible = True
        chkQuick.Text = gpsRM.GetString("psTOP_X")
        udQuick.Left = chkQuick.Left + chkQuick.Width + 4
        If chkQuick.Enabled = False Then
            udQuick.Value = 10
        End If
        'Hide summary - charts are always a summary
        chkSummary.Checked = True
        chkSummary.Visible = False

        ' setup option radio buttons for summary reports
        rbOpt01.Text = gpsRM.GetString("psDURATION_CAP")
        rbOpt01.Tag = gpsRM.GetString("psDURATION_CAP")
        rbOpt01.Visible = True

        rbOpt02.Text = gpsRM.GetString("psOCCURRENCES_CAP")
        rbOpt02.Tag = gpsRM.GetString("psOCCURRENCES_CAP")
        rbOpt02.Visible = True

        rbOpt03.Text = gpsRM.GetString("psBOTH_CAP")
        rbOpt03.Tag = gpsRM.GetString("psBOTH_CAP")
        rbOpt03.Visible = True

        If bShowColumn(eShowCol.Color) Then
            rbOpt04.Text = String.Empty
            rbOpt04.Tag = String.Empty
            rbOpt04.Visible = False
        Else
            rbOpt04.Text = String.Empty
            rbOpt04.Tag = String.Empty
            rbOpt04.Visible = False
        End If

        rbOpt05.Text = String.Empty
        rbOpt05.Tag = String.Empty
        rbOpt05.Visible = False

        rbOpt06.Text = String.Empty
        rbOpt06.Tag = String.Empty
        rbOpt06.Visible = False

        rbOpt07.Text = String.Empty
        rbOpt07.Tag = String.Empty
        rbOpt07.Visible = False

        rbOpt08.Text = String.Empty
        rbOpt08.Tag = String.Empty
        rbOpt08.Visible = False

        rbOpt01.Checked = True

    End Sub
    Private Sub subSetupDownTimeCriteria()
        '********************************************************************************************
        'Description:  Set me up some more
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        subSetUpTLP(frmMain.eReportType.Downtime)

        Dim bFound As Boolean = False
        For Each oControl As Control In tlpMain.Controls
            If oControl.Text = gcsRM.GetString("csZONE_CAP") Then
                bFound = True
            End If
        Next
        If (bFound <> True) Then
            Dim l As Label = oGetLabel()
            l.Text = gcsRM.GetString("csZONE_CAP")
            tlpMain.Controls.Add(l, 0, 0)

            Dim lb As CheckedListBox = oGetZoneBox()
            AddHandler lb.ItemCheck, AddressOf subCLBCheckHandler
            tlpMain.Controls.Add(lb, 0, 1)
        End If

        frmMain.subGetDTMask()

        chkHBO.Text = gpsRM.GetString("psINCLUDE_HBO")
        chkHBO.Visible = True
        'enable top x
        chkQuick.Visible = True
        udQuick.Visible = True
        chkQuick.Text = gpsRM.GetString("psTOP_X")
        udQuick.Left = chkQuick.Left + chkQuick.Width + 4
        If chkQuick.Enabled = False Then
            udQuick.Value = 10
        End If
        ' setup option radio buttons for summary reports
        rbOpt01.Text = gpsRM.GetString("psDURATION_CAP")
        rbOpt01.Tag = gpsRM.GetString("psDURATION_CAP")
        rbOpt01.Visible = True

        rbOpt02.Text = gpsRM.GetString("psOCCURRENCES_CAP")
        rbOpt02.Tag = gpsRM.GetString("psOCCURRENCES_CAP")
        rbOpt02.Visible = True

        rbOpt03.Text = String.Empty
        rbOpt03.Tag = String.Empty
        rbOpt03.Visible = False

        If bShowColumn(eShowCol.Color) Then
            rbOpt04.Text = String.Empty
            rbOpt04.Tag = String.Empty
            rbOpt04.Visible = False
        Else
            rbOpt04.Text = String.Empty
            rbOpt04.Tag = String.Empty
            rbOpt04.Visible = False
        End If

        rbOpt05.Text = String.Empty
        rbOpt05.Tag = String.Empty
        rbOpt05.Visible = False

        rbOpt06.Text = String.Empty
        rbOpt06.Tag = String.Empty
        rbOpt06.Visible = False

        rbOpt07.Text = String.Empty
        rbOpt07.Tag = String.Empty
        rbOpt07.Visible = False

        rbOpt08.Text = String.Empty
        rbOpt08.Tag = String.Empty
        rbOpt08.Visible = False

        rbOpt01.Checked = True

        chkDaily.Text = gpsRM.GetString("psDAILY")
        chkDaily.Visible = mbShowDaily
    End Sub
    Private Sub subSetupProductionCriteria()
        '********************************************************************************************
        'Description:  Set me up some more
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/26/09  MSW     Add carrier, use column enable for VIN
        '********************************************************************************************

        subSetUpTLP(frmMain.eReportType.Production)

        Dim l As Label = oGetLabel()
        l.Text = gcsRM.GetString("csZONE_CAP")
        tlpMain.Controls.Add(l, 0, 0)

        Dim lb As CheckedListBox = oGetZoneBox()
        AddHandler lb.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb, 0, 1)

        Dim l1 As Label = oGetLabel()
        l1.Text = gcsRM.GetString("csROBOT_CAP")
        tlpMain.Controls.Add(l1, 1, 0)

        Dim lb1 As CheckedListBox = oGetDeviceBox(False, True)
        AddHandler lb1.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb1, 1, 1)

        Dim l2 As Label = oGetLabel()
        l2.Text = gpsRM.GetString("psJOB_STATUS_CAP")
        tlpMain.Controls.Add(l2, 2, 0)

        Dim lb2 As CheckedListBox = oGetJobStatusBox()
        AddHandler lb2.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb2, 2, 1)

        Dim l3 As Label = oGetLabel()
        l3.Text = gpsRM.GetString("psSTYLE_CAP")
        tlpMain.Controls.Add(l3, 0, 2)

        Dim lb3 As CheckedListBox = oGetStyleBox()
        AddHandler lb3.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb3, 0, 3)

        If bShowColumn(eShowCol.Color) Then
        Dim l4 As Label = oGetLabel()
        l4.Text = gpsRM.GetString("psCOLOR_CAP")
        tlpMain.Controls.Add(l4, 2, 2)

        Dim lb4 As CheckedListBox = oGetColorBox()
        AddHandler lb4.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb4, 2, 3)
        End If

        If bShowColumn(eShowCol.Optioncol) Then
        Dim l5 As Label = oGetLabel()
        l5.Text = gpsRM.GetString("psOPTION_CAP")
        tlpMain.Controls.Add(l5, 1, 2)
            '3/7/14 CBZ add sort by sealer style/option
            If mbUseCombinedStyleOption Then
                l5.Visible = False
            End If
        Dim lb5 As CheckedListBox = oGetOptionBox()
        AddHandler lb5.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb5, 1, 3)
            '3/7/14 CBZ add sort by sealer style/option
            If mbUseCombinedStyleOption Then
                lb5.Visible = False
            End If
        End If

        If bShowColumn(eShowCol.VIN) Then

            Dim l6 As Label = oGetLabel()
            l6.Text = gpsRM.GetString("psVIN_CAP")
            tlpMain.Controls.Add(l6, 3, 2)

            Dim c1 As ComboBox = oGetVINBox()
            AddHandler c1.LostFocus, AddressOf cboBox_LostFocusHandler
            tlpMain.Controls.Add(c1, 3, 3)

        End If

        If bShowColumn(eShowCol.Carrier) Then

            Dim l7 As Label = oGetLabel()
            l7.Text = gpsRM.GetString("psCARRIER_CAP")
            tlpMain.Controls.Add(l7, 3, 0)

            Dim c2 As ComboBox = oGetCarrierBox()
            AddHandler c2.LostFocus, AddressOf cboBox_LostFocusHandler
            tlpMain.Controls.Add(c2, 3, 1)

        End If
        chkHBO.Visible = False
        chkQuick.Visible = True
        udQuick.Visible = True
        If chkQuick.Enabled = False Then
            udQuick.Value = 50
        End If
        ' setup option radio buttons for summary reports
        rbOpt01.Text = gpsRM.GetString("psJOB_REPORT_CAP")
        rbOpt01.Tag = gpsRM.GetString("psJOB_REPORT_CAP")
        rbOpt01.Visible = True

        rbOpt02.Text = gpsRM.GetString("psSTYLE_CAP")
        rbOpt02.Tag = gpsRM.GetString("psSTYLE_CAP")
        rbOpt02.Visible = True

        If bShowColumn(eShowCol.Color) Then
            rbOpt03.Text = gpsRM.GetString("psCOLOR_CAP")
            rbOpt03.Tag = gpsRM.GetString("psCOLOR_CAP")
            rbOpt03.Visible = True
            rbOpt04.Text = gpsRM.GetString("psPAINT_TOTAL_CAP")
            rbOpt04.Tag = gpsRM.GetString("psPAINT_TOTAL_CAP")
            rbOpt04.Visible = True
        Else
            rbOpt03.Text = String.Empty
            rbOpt03.Tag = String.Empty
            rbOpt03.Visible = False
            rbOpt04.Text = String.Empty
            rbOpt04.Tag = String.Empty
            rbOpt04.Visible = False
        End If

        rbOpt05.Text = String.Empty
        rbOpt05.Tag = String.Empty
        rbOpt05.Visible = False

        rbOpt06.Text = String.Empty
        rbOpt06.Tag = String.Empty
        rbOpt06.Visible = False

        rbOpt07.Text = String.Empty
        rbOpt07.Tag = String.Empty
        rbOpt07.Visible = False

        rbOpt08.Text = String.Empty
        rbOpt08.Tag = String.Empty
        rbOpt08.Visible = False

        rbOpt01.Checked = True

        chkDaily.Text = gpsRM.GetString("psDAILY")
        chkDaily.Visible = mbShowDaily
    End Sub
    Private Sub subSetupVisionCriteria()
        '********************************************************************************************
        'Description:  Set me up some more
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        subSetUpTLP(frmMain.eReportType.Vision)

        Dim l As Label = oGetLabel()
        l.Text = gcsRM.GetString("csZONE_CAP")
        tlpMain.Controls.Add(l, 0, 0)

        Dim lb As CheckedListBox = oGetZoneBox()
        AddHandler lb.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb, 0, 1)

        Dim l1 As Label = oGetLabel()
        l1.Text = gcsRM.GetString("csROBOT_CAP")
        tlpMain.Controls.Add(l1, 1, 0)

        Dim lb1 As CheckedListBox = oGetDeviceBox(True, False)
        AddHandler lb1.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb1, 1, 1)

        Dim l2 As Label = oGetLabel()
        l2.Text = gpsRM.GetString("psVISION_STATUS_CAP")
        tlpMain.Controls.Add(l2, 2, 0)

        Dim lb2 As CheckedListBox = mVision.oGetVisionStatusBox()
        AddHandler lb2.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb2, 2, 1)

        Dim l3 As Label = oGetLabel()
        l3.Text = gpsRM.GetString("psSTYLE_CAP")
        tlpMain.Controls.Add(l3, 0, 2)

        Dim lb3 As CheckedListBox = oGetStyleBox()
        AddHandler lb3.ItemCheck, AddressOf subCLBCheckHandler
        tlpMain.Controls.Add(lb3, 0, 3)

        If bShowColumn(eShowCol.Optioncol) Then
            Dim l5 As Label = oGetLabel()
            l5.Text = gpsRM.GetString("psOPTION_CAP")
            tlpMain.Controls.Add(l5, 1, 2)

            Dim lb5 As CheckedListBox = oGetOptionBox()
            AddHandler lb5.ItemCheck, AddressOf subCLBCheckHandler
            tlpMain.Controls.Add(lb5, 1, 3)
        End If

        If bShowColumn(eShowCol.VIN) Then

            Dim l6 As Label = oGetLabel()
            l6.Text = gpsRM.GetString("psVIN_CAP")
            tlpMain.Controls.Add(l6, 3, 2)

            Dim c1 As ComboBox = oGetVINBox()
            AddHandler c1.LostFocus, AddressOf cboBox_LostFocusHandler
            tlpMain.Controls.Add(c1, 3, 3)

        End If

        If bShowColumn(eShowCol.Carrier) Then

            Dim l7 As Label = oGetLabel()
            l7.Text = gpsRM.GetString("psCARRIER_CAP")
            tlpMain.Controls.Add(l7, 3, 0)

            Dim c2 As ComboBox = oGetCarrierBox()
            AddHandler c2.LostFocus, AddressOf cboBox_LostFocusHandler
            tlpMain.Controls.Add(c2, 3, 1)

        End If
        chkHBO.Visible = False
        chkQuick.Visible = True
        udQuick.Visible = True
        If chkQuick.Enabled = False Then
            udQuick.Value = 50
        End If
        ' setup option radio buttons for summary reports
        rbOpt01.Text = gpsRM.GetString("psSTYLE_CAP")
        rbOpt01.Tag = gpsRM.GetString("psSTYLE_CAP")
        rbOpt01.Visible = True

        rbOpt02.Text = gpsRM.GetString("psVISION_STATUS_CAP")
        rbOpt02.Tag = gpsRM.GetString("psVISION_STATUS_CAP")
        rbOpt02.Visible = True

            rbOpt03.Text = String.Empty
            rbOpt03.Tag = String.Empty
            rbOpt03.Visible = False
            rbOpt04.Text = String.Empty
            rbOpt04.Tag = String.Empty
            rbOpt04.Visible = False

        rbOpt05.Text = String.Empty
        rbOpt05.Tag = String.Empty
        rbOpt05.Visible = False

        rbOpt06.Text = String.Empty
        rbOpt06.Tag = String.Empty
        rbOpt06.Visible = False

        rbOpt07.Text = String.Empty
        rbOpt07.Tag = String.Empty
        rbOpt07.Visible = False

        rbOpt08.Text = String.Empty
        rbOpt08.Tag = String.Empty
        rbOpt08.Visible = False

        rbOpt01.Checked = True

        chkDaily.Text = gpsRM.GetString("psDAILY")
        chkDaily.Visible = mbShowDaily

    End Sub
    Private Sub subSetUpTLP(ByVal vReportType As frmMain.eReportType)
        '********************************************************************************************
        'Description:  setup the table layout panel
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case vReportType

            Case frmMain.eReportType.Change
                With tlpMain
                    .ColumnCount = 3
                    .ColumnStyles(0).SizeType = SizeType.Percent
                    .ColumnStyles(1).SizeType = SizeType.Percent
                    .ColumnStyles(2).SizeType = SizeType.Percent
                    .ColumnStyles(0).Width = 25
                    .ColumnStyles(1).Width = 50
                    .ColumnStyles(2).Width = 25
                    .CellBorderStyle = TableLayoutPanelCellBorderStyle.InsetDouble
                End With

            Case frmMain.eReportType.Alarm
                With tlpMain
                    .ColumnCount = 4
                    .ColumnStyles(0).SizeType = SizeType.Percent
                    .ColumnStyles(1).SizeType = SizeType.Percent
                    .ColumnStyles(2).SizeType = SizeType.Percent
                    .ColumnStyles(3).SizeType = SizeType.Percent
                    .ColumnStyles(0).Width = 25
                    .ColumnStyles(1).Width = 25
                    .ColumnStyles(2).Width = 25
                    .ColumnStyles(3).Width = 25
                    .CellBorderStyle = TableLayoutPanelCellBorderStyle.InsetDouble
                End With

            Case frmMain.eReportType.Production, frmMain.eReportType.Vision
                With tlpMain
                    .ColumnCount = 4
                    .ColumnStyles(0).SizeType = SizeType.Percent
                    .ColumnStyles(1).SizeType = SizeType.Percent
                    .ColumnStyles(2).SizeType = SizeType.Percent
                    .ColumnStyles(3).SizeType = SizeType.Percent
                    .ColumnStyles(0).Width = 25
                    .ColumnStyles(1).Width = 25
                    .ColumnStyles(2).Width = 28
                    .ColumnStyles(3).Width = 22
                    .CellBorderStyle = TableLayoutPanelCellBorderStyle.InsetDouble
                End With

            Case frmMain.eReportType.Downtime

                With tlpMain
                    .ColumnCount = 2
                    .RowCount = 2
                    .ColumnStyles(0).SizeType = SizeType.Percent
                    .ColumnStyles(1).SizeType = SizeType.Percent
                    .ColumnStyles(0).Width = 50
                    .ColumnStyles(1).Width = 50
                    .CellBorderStyle = TableLayoutPanelCellBorderStyle.InsetDouble
                End With

            Case frmMain.eReportType.Conveyor

                With tlpMain
                    .ColumnCount = 2
                    .RowCount = 2
                    .ColumnStyles(0).SizeType = SizeType.Percent
                    .ColumnStyles(1).SizeType = SizeType.Percent
                    .ColumnStyles(0).Width = 50
                    .ColumnStyles(1).Width = 50
                    .CellBorderStyle = TableLayoutPanelCellBorderStyle.InsetDouble
                End With

            Case frmMain.eReportType.RMCharts

                With tlpMain
                    .ColumnCount = 2
                    .RowCount = 2
                    .ColumnStyles(0).SizeType = SizeType.Percent
                    .ColumnStyles(1).SizeType = SizeType.Percent
                    .ColumnStyles(0).Width = 50
                    .ColumnStyles(1).Width = 50
                    .CellBorderStyle = TableLayoutPanelCellBorderStyle.InsetDouble
                End With
        End Select

    End Sub
    Friend Sub ShowMe(Optional ByVal ForceReinit As Boolean = False)
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/23/09  MSW     ShowMe - change .show to .showdialog, it'll grab the focus better.
        '********************************************************************************************

        Me.Size = New Size(910, 590)
        If (Not mbScreenLoaded) Or ForceReinit Then

            subInitializeForm()


        End If

        mbBreakAlarmsActive = False

        If Me.Visible = False Then
            Try
                Me.ShowDialog()
            Catch ex As Exception
            End Try
        End If
        Me.Focus()

    End Sub
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

            'get the report data
            If bGetDataSet() Then
                RaiseEvent RunReport(False)
            Else
                RaiseEvent QueryFailed(String.Empty)
            End If
        Else
            Me.Hide()
            frmMain.Cursor = Cursors.Default
        End If


    End Sub

#End Region

#Region " Events "

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

        subExit(False)

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

        subExit(True)

    End Sub
    Private Sub btnShift_Click(ByVal sender As System.Object, Optional ByVal e As System.EventArgs = Nothing) _
                Handles btnShift1.Click, btnShift2.Click, btnShift3.Click
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/11/11  RJO     Modified code to adjust start and end date for shift wrap.
        '********************************************************************************************
        Dim b As Button = DirectCast(sender, Button)
        Select Case b.Name
            Case "btnShift1"
                dtpStartTime.Value = mcolShifts(1).StartTime
                dtpEndTime.Value = mcolShifts(1).EndTime
            Case "btnShift2"
                dtpStartTime.Value = mcolShifts(2).StartTime
                dtpEndTime.Value = mcolShifts(2).EndTime
            Case "btnShift3"
                dtpStartTime.Value = mcolShifts(3).StartTime
                dtpEndTime.Value = mcolShifts(3).EndTime
        End Select
        'check for wrap
        If dtpStartTime.Value.Hour > dtpEndTime.Value.Hour Then
            'for now assume we want to change the day forward
            'Check to see if its the 2nd day of the shift and adjust the start date if necessary
            If Now.Hour < dtpStartTime.Value.Hour Then
                dtpStartDate.Value = DateAdd(DateInterval.Day, -1, Now)
            Else
                dtpStartDate.Value = Now
            End If
            dtpEndDate.Value = DateAdd(DateInterval.Day, 1, dtpStartDate.Value)
        Else
            If dtpStartTime.Value.Hour > Now.Hour Then

                dtpStartDate.Value = DateAdd(DateInterval.Day, -1, Now)
            Else
            'Otherwise, keep the start date the same as the end date
            dtpStartDate.Value = Now
            End If
            dtpEndDate.Value = dtpStartDate.Value
        End If
    End Sub
    Private Sub chkSummary_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                            Handles chkSummary.CheckStateChanged
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
        Dim bEnb As Boolean = (DirectCast(sender, CheckBox).Checked)
        Dim nOffset As Integer

        Select Case ReportType
            Case frmMain.eReportType.Alarm
                If bEnb Then
                    nOffset = 50
                Else
                    nOffset = -50
                End If
            Case frmMain.eReportType.Change
                If bEnb Then
                    nOffset = 50
                Else
                    nOffset = -50
                End If
            Case frmMain.eReportType.Production
                If bEnb Then
                    nOffset = 50
                Else
                    nOffset = -50
                End If
            Case frmMain.eReportType.Conveyor
                Exit Sub
            Case frmMain.eReportType.Downtime, frmMain.eReportType.RMCharts, frmMain.eReportType.RMChartsAuto
                If bEnb Then
                    nOffset = 50
                Else
                    nOffset = -50
                End If
            Case frmMain.eReportType.Vision
                If bEnb Then
                    nOffset = 50
                Else
                    nOffset = -50
                End If
        End Select

        Dim sz As New Size
        sz = Me.Size
        sz.Height = sz.Height + nOffset
        Me.Size = sz

        tlpMain.Top = tlpMain.Top + nOffset
        btnCancel.Top = btnCancel.Top + nOffset
        btnOK.Top = btnOK.Top + nOffset
        btnAutoGen.Top = btnAutoGen.Top + nOffset

        gpbSummaryOpt.Visible = bEnb

    End Sub
    Private Sub chkQuick_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                  Handles chkQuick.CheckStateChanged
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
        Dim bEnb As Boolean = Not (DirectCast(sender, CheckBox).Checked)
        If (ReportType = frmMain.eReportType.RMCharts) Or (ReportType = frmMain.eReportType.RMChartsAuto) Then
            Exit Sub
        End If
        dtpStartDate.Enabled = bEnb
        dtpEndDate.Enabled = bEnb
        dtpStartTime.Enabled = bEnb
        dtpEndTime.Enabled = bEnb

        gpbShift.Enabled = bEnb

        If (chkSummary.Checked And (Not bEnb)) Then
            chkSummary.Checked = False
        End If
        tlpMain.Enabled = bEnb
        chkSummary.Enabled = bEnb

    End Sub
    Private Sub cboZone_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                              Handles cboZone.SelectionChangeCommitted
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

        subChangeZone()
    End Sub
    Protected Overrides Sub Finalize()
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
            For Each o As Control In tlpMain.Controls
                If TypeOf o Is CheckedListBox Then
                    Dim clb As CheckedListBox = DirectCast(o, CheckedListBox)
                    RemoveHandler clb.ItemCheck, AddressOf subCLBCheckHandler
                End If
            Next
        Catch ex As Exception

        End Try
        MyBase.Finalize()
    End Sub

#End Region

    Private Sub frmCriteria_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
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
                    Select Case ReportType
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
                        Case frmMain.eReportType.Vision
                            subLaunchHelp(gs_HELP_REPORTS_VISION, frmMain.oIPC)
                    End Select
                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty
                    Dim sName As String = msSCREEN_DUMP_NAME
                    Select Case ReportType
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
                        Case frmMain.eReportType.Vision
                            sName = sName & "_Vision"
                    End Select
                    If ReportSummaryType <> String.Empty Then
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
    End Sub

    Private Sub frmCriteria_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        '********************************************************************************************
        'Description: double check the size - there's some strange code messing with the form height
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        'btn...loc = 257, 519
        'btn...size = 123, 39
        'Me.Size  = 892, 591
        If Me.Height < (btnOK.Top + 50) Then
            'Debug.Assert(False)
            Me.Height = btnOK.Top + 72
        End If
    End Sub

    Private Sub btnAutoGen_Click(Optional ByVal sender As System.Object = Nothing, Optional ByVal e As System.EventArgs = Nothing) Handles btnAutoGen.Click
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
        If EndTime <= StartTime Then
            MessageBox.Show(gpsRM.GetString("psEND_TIME_LATER"), "", MessageBoxButtons.OK, _
                            MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, _
                            MessageBoxOptions.DefaultDesktopOnly)
            Exit Sub
        End If

        chkSummary.Checked = False
        Application.DoEvents()
        'get the report data
        If bGetDataSet() Then
            Me.Visible = False
            RaiseEvent RunReport(True)
            Me.Visible = True
        End If
        chkSummary.Checked = True
        Application.DoEvents()

        If rbOpt01.Visible Then
            rbOpt01.Checked = True
            Application.DoEvents()
            'get the report data
            If bGetDataSet() Then
                Me.Visible = False
                RaiseEvent RunReport(True)
                Me.Visible = True
            End If
            If rbOpt02.Visible Then
                rbOpt02.Checked = True
                Application.DoEvents()
                'get the report data
                If bGetDataSet() Then
                    Me.Visible = False
                    RaiseEvent RunReport(True)
                    Me.Visible = True
                End If

                If rbOpt03.Visible Then
                    rbOpt03.Checked = True
                    Application.DoEvents()
                    'get the report data
                    If bGetDataSet() Then
                        Me.Visible = False
                        RaiseEvent RunReport(True)
                        Me.Visible = True
                    End If

                    If rbOpt04.Visible Then
                        rbOpt04.Checked = True
                        Application.DoEvents()
                        'get the report data
                        If bGetDataSet() Then
                            Me.Visible = False
                            RaiseEvent RunReport(True)
                            Me.Visible = True
                        End If
                        If rbOpt05.Visible Then
                            rbOpt05.Checked = True
                            Application.DoEvents()
                            'get the report data
                            If bGetDataSet() Then
                                Me.Visible = False
                                RaiseEvent RunReport(True)
                                Me.Visible = True
                            End If
                            If rbOpt06.Visible Then
                                rbOpt06.Checked = True
                                Application.DoEvents()
                                'get the report data
                                If bGetDataSet() Then
                                    Me.Visible = False
                                    RaiseEvent RunReport(True)
                                    Me.Visible = True
                                End If
                                If rbOpt07.Visible Then
                                    rbOpt07.Checked = True
                                    Application.DoEvents()
                                    'get the report data
                                    If bGetDataSet() Then
                                        Me.Visible = False
                                        RaiseEvent RunReport(True)
                                        Me.Visible = True
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If


        frmMain.Cursor = Cursors.WaitCursor


    End Sub


    Private Sub frmCriteria_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        If mbAutoRun Then
            btnAutoGen_Click()
            Me.Hide()
            frmMain.Cursor = Cursors.Default
        End If
    End Sub
End Class