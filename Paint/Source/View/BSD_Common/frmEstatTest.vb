' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2012
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: frmEstatTest
'
' Description: E-Stat high voltage cable test function
' 
'
' Dependencies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Rick Olejniczak
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                              
' 11/30/2012    RJO     Initial code.
'***************************************************************************************************

'******** Unfinished business***********************************************************************
'TODO - No strings on this form have been localized
'***************************************************************************************************

Option Compare Text
Option Explicit On
Option Strict On

Friend Class frmEstatTest

#Region " Declares "

    '******** Form Constants ************************************************************************
    Private Const msMODULE As String = "frmEstatTest"
    Private mCOLOR_FAULT As System.Drawing.Color = Color.Tomato
    Private mCOLOR_OK As System.Drawing.Color = Color.LimeGreen
    Private mCOLOR_WARN As System.Drawing.Color = Color.Yellow
    '******** End Form Constants ********************************************************************

    '******** Form Variables ************************************************************************
    Private maTestData() As udsTestData
    Private mbExecutionStarted As Boolean
    Private mbExecutingTestPath As Boolean
    Private mDS As DataSet
    Private mDRs() As DataRow
    Private mbMitsubishiPLC As Boolean
    '******** End Form Variables ********************************************************************

    '******** Property Variables ********************************************************************
    Private mnAwayMaxCurrent As Integer
    Private mnAwayMinCurrent As Integer
    Private mnBaseRegister As Integer
    Private mnColor As Integer
    Private mnWarnHours As Integer
    Private mnMaxHours As Integer
    Private mnOption As Integer
    Private mnStyle As Integer
    Private mnTargetMaxCurrent As Integer
    Private mnTargetMinCurrent As Integer
    Private mnTestMaxCurrent As Integer
    Private mnTestMinCurrent As Integer
    Private msXmlPath As String
    '******** End Property Variables ****************************************************************

    '******** Enumerations **************************************************************************
    Private Enum eRefCol As Integer
        Robot = 0
        TestDate = 1
        OpHours = 2
        Away = 3
        AtTarget = 4
        Test = 5
    End Enum

    Private Enum eTestCol As Integer
        Robot = 0
        TestDate = 1
        Away = 2
        AtTarget = 3
        Test = 4
        Record = 5
    End Enum
    '******** End Enumerations **********************************************************************

    '******** Structures ****************************************************************************
    Private Structure udsTestData
        Public ArmDisplayName As String
        Public ControllerNumber As Integer
        Public EquipNumber As Integer
        Public CalDate As String
        Public TestDate As String
        Public OpHours As Integer
        Public RefAwayCurrent As Integer
        Public RefAwayCurrentOld As Integer
        Public RefTargetCurrent As Integer
        Public RefTargetCurrentOld As Integer
        Public RefTestCurrent As Integer
        Public RefTestCurrentOld As Integer
        Public AwayCurrent As Integer
        Public AwayCurrentOld As Integer
        Public TargetCurrent As Integer
        Public TargetCurrentOld As Integer
        Public TestCurrent As Integer
        Public TestCurrentOld As Integer
        Public TestDataOk As Boolean
        Public RobotDataUpdated As Boolean
    End Structure
    '******** End Structures ************************************************************************

#End Region

#Region " Properties "

    Private ReadOnly Property AwayMaxCurrent() As Integer
        '********************************************************************************************
        'Description:  Returns the maximum in-range Away from Target current value (uA). 
        '
        'Parameters: None
        'Returns:    Current value in uA
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnAwayMaxCurrent
        End Get
    End Property

    Private ReadOnly Property AwayMinCurrent() As Integer
        '********************************************************************************************
        'Description:  Returns the minmum in-range Away from Target current value (uA). 
        '
        'Parameters: None
        'Returns:    Current value in uA
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnAwayMinCurrent
        End Get
    End Property

    Private ReadOnly Property BaseRegister() As Integer
        '********************************************************************************************
        'Description:  Returns the base register number for storing Test Results. 
        '              (Base = Eq1 Away current, Base+1 = Eq1 Target current, Base+2 = Eq2 Away 
        '              current, Base+3 = Eq2 Target current)
        '
        'Parameters: None
        'Returns:    Base register number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnBaseRegister
        End Get
    End Property

    Private ReadOnly Property GhostColor() As Integer
        '********************************************************************************************
        'Description:  Returns the Color number the robot controller will be initialized with when 
        '              the test path is executed.
        '
        'Parameters: None
        'Returns:    Color number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnColor
        End Get
    End Property

    Private ReadOnly Property GhostOption() As Integer
        '********************************************************************************************
        'Description:  Returns the Option number the robot controller will be initialized with when 
        '              the test path is executed.
        '
        'Parameters: None
        'Returns:    Option number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnOption
        End Get
    End Property

    Private ReadOnly Property GhostStyle() As Integer
        '********************************************************************************************
        'Description:  Returns the Style number the robot controller will be initialized with when 
        '              the test path is executed.
        '
        'Parameters: None
        'Returns:    Style number
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnStyle
        End Get
    End Property

    Private ReadOnly Property MaxHours() As Integer
        '********************************************************************************************
        'Description:  Returns the maximum in-range Cumulative Operation Hours value. 
        '
        'Parameters: None
        'Returns:    Cumulative Operation Hours
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnMaxHours
        End Get
    End Property

    Private WriteOnly Property Status() As String
        '********************************************************************************************
        'Description:  Updates the Status label at the bottom of the form. 
        '
        'Parameters: Status Message
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As String)
            lblStatus.Text = value
        End Set
    End Property

    Private ReadOnly Property TargetMaxCurrent() As Integer
        '********************************************************************************************
        'Description:  Returns the maximum in-range At Target current value (uA). 
        '
        'Parameters: None
        'Returns:    Current value in uA
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnTargetMaxCurrent
        End Get
    End Property

    Private ReadOnly Property TargetMinCurrent() As Integer
        '********************************************************************************************
        'Description:  Returns the minmum in-range At Target current value (uA). 
        '
        'Parameters: None
        'Returns:    Current value in uA
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnTargetMinCurrent
        End Get
    End Property

    Private ReadOnly Property TestMaxCurrent() As Integer
        '********************************************************************************************
        'Description:  Returns the maximum in-range Test Result current value (uA). 
        '
        'Parameters: None
        'Returns:    Current value in uA
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnTestMaxCurrent
        End Get
    End Property

    Private ReadOnly Property TestMinCurrent() As Integer
        '********************************************************************************************
        'Description:  Returns the minmum in-range Test Result current value (uA). 
        '
        'Parameters: None
        'Returns:    Current value in uA
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnTestMinCurrent
        End Get
    End Property

    Private ReadOnly Property WarnHours() As Integer
        '********************************************************************************************
        'Description:  Returns the Cumulative Operation Hours Warning value. 
        '
        'Parameters: None
        'Returns:    Cumulative Operation Hours
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnWarnHours
        End Get
    End Property

    Private ReadOnly Property XMLPath() As String
        '********************************************************************************************
        'Description:  Returns the path to the XML database files folder.
        '
        'Parameters: None
        'Returns:    Path
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msXmlPath
        End Get
    End Property

#End Region

#Region " Routines "

    Private Function GetDataSet() As DataSet
        '****************************************************************************************
        'Description: Returns the entire EstatTest dataset.
        '
        'Parameters: none
        'Returns:    EstatTest DataSet
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim DS As New DataSet

        DS.ReadXmlSchema(XMLPath & "EstatTest.XSD")
        DS.ReadXml(XMLPath & "EstatTest.XML")

        Return DS

    End Function

    Private Sub subCheckLimits()
        '********************************************************************************************
        'Description: Check if test values are in-range and update the grid accordingly.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oBackColor As System.Drawing.Color
        Dim bOK As Boolean

        For nRow As Integer = 0 To maTestData.GetUpperBound(0)

            'Cumulative Operation Hours
            bOK = True
            oBackColor = Color.White

            If maTestData(nRow).OpHours < WarnHours Then
                oBackColor = mCOLOR_OK
            ElseIf maTestData(nRow).OpHours < MaxHours Then
                oBackColor = mCOLOR_WARN
            Else
                oBackColor = mCOLOR_FAULT
                bOK = False
            End If
            dgvRefData.Item(eRefCol.OpHours, nRow).Style.BackColor = oBackColor
            If bOK Then
                dgvRefData.Item(eRefCol.OpHours, nRow).ToolTipText = String.Empty
            Else
                dgvRefData.Item(eRefCol.OpHours, nRow).ToolTipText = "Please replace cable"
            End If

            'Away From Target Actual
            maTestData(nRow).TestDataOk = True
            dgvTestData.Item(eTestCol.Away, nRow).ToolTipText = String.Empty
            oBackColor = Color.White

            If (maTestData(nRow).AwayCurrent < AwayMinCurrent) Or (maTestData(nRow).AwayCurrent > AwayMaxCurrent) Then
                maTestData(nRow).TestDataOk = False
                oBackColor = mCOLOR_WARN
                dgvTestData.Item(eTestCol.Away, nRow).ToolTipText = "Please inspect system"
            End If
            dgvTestData.Item(eTestCol.Away, nRow).Style.BackColor = oBackColor

            'At Target Actual
            dgvTestData.Item(eTestCol.AtTarget, nRow).ToolTipText = String.Empty
            oBackColor = Color.White

            If (maTestData(nRow).TargetCurrent < TargetMinCurrent) Or (maTestData(nRow).TargetCurrent > TargetMaxCurrent) Then
                maTestData(nRow).TestDataOk = False
                'oBackColor = mCOLOR_WARN
                'dgvTestData.Item(eTestCol.AtTarget, nRow).ToolTipText = "Please inspect system"
            End If
            'dgvTestData.Item(eTestCol.AtTarget, nRow).Style.BackColor = oBackColor

            Dim sReason As String = ""
            'HV Cable Test Result
            If Not maTestData(nRow).TestDataOk Then

                'Test data is invalid, put reason in TestCurrent column
                If (maTestData(nRow).AwayCurrent < AwayMinCurrent) Or (maTestData(nRow).AwayCurrent > AwayMaxCurrent) Then
                    sReason = "Fix Leakage Current"
                    maTestData(nRow).TestDataOk = False
                    dgvTestData.Item(eTestCol.Test, nRow).Value = sReason
                Else
                    'Test data is invalid, put reason in TestCurrent column
                    If (maTestData(nRow).TargetCurrent < TargetMinCurrent) Or (maTestData(nRow).TargetCurrent > TargetMaxCurrent) Then
                        sReason = "Fail"
                        oBackColor = mCOLOR_FAULT
                        maTestData(nRow).TestDataOk = False
                        dgvTestData.Item(eTestCol.AtTarget, nRow).ToolTipText = "Replace Cable and re-run test"
                        dgvTestData.Item(eTestCol.AtTarget, nRow).Style.BackColor = oBackColor
                        dgvTestData.Item(eTestCol.Test, nRow).Value = sReason
                    End If
                End If
            Else
                oBackColor = Color.White
                dgvTestData.Item(eTestCol.Away, nRow).ToolTipText = String.Empty
                dgvTestData.Item(eTestCol.Away, nRow).Style.BackColor = oBackColor
                oBackColor = mCOLOR_OK
                sReason = "Pass"
                dgvTestData.Item(eTestCol.AtTarget, nRow).ToolTipText = String.Empty
                dgvTestData.Item(eTestCol.AtTarget, nRow).Style.BackColor = oBackColor
                dgvTestData.Item(eTestCol.Test, nRow).Value = sReason
            End If


            'Set tooltip on Cable Replaced button
            If maTestData(nRow).TestDataOk Then
                dgvTestData.Item(eTestCol.Record, nRow).ToolTipText = "Click to Record as Reference"
                dgvTestData.Item(eTestCol.Record, nRow).Style.ForeColor = Color.Black
            Else
                dgvTestData.Item(eTestCol.Record, nRow).ToolTipText = "Test Data Out of Tolerance"
                dgvTestData.Item(eTestCol.Record, nRow).Style.ForeColor = Color.Gray
            End If

        Next 'nRow

    End Sub

    Private Sub subExecuteTest()
        '********************************************************************************************
        'Description: Allow a user with execute privilege to run the estat test ghost path if the 
        '             zone is ready.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bContinue As Boolean = True

        btnRefresh.Enabled = False
        btnStart.Enabled = False

        Try
            If frmMain.moPassword.CheckPassword(ePrivilege.Edit) Then
                'Ready for ghost?
                If (CInt(frmMain.msZoneData(eBooth.GhostInfoWd)) And eBooth.EstatZoneOK) <> eBooth.EstatZoneOK Then
                    'Not ready, try the GhostStatus screen
                    bContinue = frmGhostStatus.Show(frmMain.msZoneData)
                    frmGhostStatus.Dispose()
                End If 'status form

                'Allow screen to redraw
                Application.DoEvents()
            Else
                bContinue = False
            End If 'CheckPassword

        Catch ex As Exception
            ShowErrorMessagebox(msMODULE & ".subExecuteTest - Unable to verify ready for ghost ", ex, frmMain.msSCREEN_NAME, frmMain.Status)
            bContinue = False
        End Try

        'allow it to redraw 
        Application.DoEvents()

        Try
            If bContinue Then 'write the ghost
                Dim sSetting(0) As String
                Dim sData(eQueueEdit.Max) As String

                'Write track sim setting - 1=True
                frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "GhostSimConv"
                sSetting(0) = "1"
                frmMain.mPLC.PLCData = sSetting

                'Build edit data array
                For nItem As Integer = 0 To sData.GetUpperBound(0)
                    sData(nItem) = "0"
                Next 'n Item

                sData(eQueueEdit.StatusWd) = CInt(eJobStatus.Ghost).ToString
                sData(eQueueEdit.StyleWd) = GhostStyle.ToString
                sData(eQueueEdit.OptionWd) = GhostOption.ToString
                sData(eQueueEdit.ColorWd) = GhostColor.ToString
                sData(eQueueEdit.CarrierWd) = "99"

                'Write edit data to PLC
                frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "GhostQue"
                frmMain.mPLC.PLCData = sData

                'Write edit Position to PLC
                sSetting(0) = "11" 'Hard coded to position 11
                frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "QueueEditPosition"
                frmMain.mPLC.PLCData = sSetting
                Application.DoEvents()
                System.Threading.Thread.Sleep(500)
                Application.DoEvents()

                'Write complete flag to PLC
                sSetting(0) = "1"
                frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "QueEditComplete"
                frmMain.mPLC.PLCData = sSetting
                Application.DoEvents()

                'Write Change log
                AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                            colZones.CurrentZoneNumber, "All", String.Empty, _
                                            "Estat Test Execution Started", colZones.CurrentZone)
                'mPWCommon.SaveToChangeLog(colZones.DatabasePath)
                mPWCommon.SaveToChangeLog(colZones.ActiveZone)

                mbExecutionStarted = True
                tmrPoll.Enabled = True
                Status = "Executing Test"
            Else
                btnRefresh.Enabled = True
                btnStart.Enabled = True
            End If

        Catch ex As Exception
            ShowErrorMessagebox(msMODULE & ".subExecuteTest - Unable to write ghost to PLC", ex, _
                                frmMain.msSCREEN_NAME, frmMain.Status)
        End Try

    End Sub

    Private Sub subFormatGrids()
        '********************************************************************************************
        'Description: Set data grid properties
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nCols As Integer = 6
        Dim nCol As Integer

        dgvRefData.ColumnCount = nCols - 1
        dgvTestData.ColumnCount = nCols

        For nCol = 0 To nCols - 1
            If nCol < 5 Then
                With dgvRefData
                    .Columns(nCol).Width = (dgvRefData.Width - 5) \ 5
                    .Columns(nCol).SortMode = DataGridViewColumnSortMode.NotSortable
                End With
            End If
            With dgvTestData
                .Columns(nCol).Width = (dgvTestData.Width - 5) \ 6
                .Columns(nCol).SortMode = DataGridViewColumnSortMode.NotSortable
            End With
        Next

        With dgvRefData
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True
            .RowsDefaultCellStyle.SelectionBackColor = Color.Transparent
            .RowsDefaultCellStyle.SelectionForeColor = Color.Black
            .SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect
        End With

        With dgvTestData
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True
            .RowsDefaultCellStyle.SelectionBackColor = Color.Transparent
            .RowsDefaultCellStyle.SelectionForeColor = Color.Black
            .SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect
        End With

    End Sub

    Private Sub subInitFormText()
        '********************************************************************************************
        'Description: Init Common text
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        With dgvRefData
            .Columns(0).HeaderText = "Robot"
            .Columns(1).HeaderText = "Reference Calibration Date"
            .Columns(2).HeaderText = "Cumulative Operation Hours"
            .Columns(3).HeaderText = "Away From Target (uA)"
            .Columns(4).HeaderText = "At Target (uA)"
        End With

        With dgvTestData
            .Columns(0).HeaderText = "Robot"
            .Columns(1).HeaderText = "Last Cable Test Date"
            .Columns(2).HeaderText = "Away From Target (uA)"
            .Columns(3).HeaderText = "At Target (uA)"
            .Columns(4).HeaderText = "HV Cable Test Result"
            .Columns(5).HeaderText = "Record As Reference"
        End With

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

        Status = "Initializing"

        'get path toxml data files
        mPWCommon.GetDefaultFilePath(msXmlPath, eDir.XML, String.Empty, String.Empty)
        mbMitsubishiPLC = (BSD.colZones.ActiveZone.PLCType = ePLCType.Mitsubishi)

        Try
            mDS = GetDataSet()
            Dim DR As DataRow = mDS.Tables("ConfigData").Rows(0)

            'Populate Configuration Property Data
            mnBaseRegister = CType(DR.Item("BaseRegister"), Integer)
            mnColor = CType(DR.Item("GhostColor"), Integer)
            mnOption = CType(DR.Item("GhostOption"), Integer)
            mnStyle = CType(DR.Item("GhostStyle"), Integer)
            mnWarnHours = CType(DR.Item("WarnHours"), Integer)
            mnMaxHours = CType(DR.Item("MaxHours"), Integer)
            mnAwayMaxCurrent = CType(DR.Item("AwayMaxCurrent"), Integer)
            mnAwayMinCurrent = CType(DR.Item("AwayMinCurrent"), Integer)
            mnTargetMaxCurrent = CType(DR.Item("TargetMaxCurrent"), Integer)
            mnTargetMinCurrent = CType(DR.Item("TargetMinCurrent"), Integer)
            mnTestMaxCurrent = CType(DR.Item("TestMaxCurrent"), Integer)
            mnTestMinCurrent = CType(DR.Item("TestMinCurrent"), Integer)

            'Populate Test Data structures
            mDRs = mDS.Tables("TestData").Select("Enabled = true", "DisplayIndex ASC")
            ReDim maTestData(mDRs.GetUpperBound(0))

            For nIndex As Integer = 0 To mDRs.GetUpperBound(0)
                maTestData(nIndex).ArmDisplayName = mDRs(nIndex).Item("ArmDisplayName").ToString
                maTestData(nIndex).ControllerNumber = CType(mDRs(nIndex).Item("ControllerNumber"), Integer)
                maTestData(nIndex).EquipNumber = CType(mDRs(nIndex).Item("EquipNumber"), Integer)
                maTestData(nIndex).CalDate = mDRs(nIndex).Item("CalDate").ToString
                maTestData(nIndex).TestDate = mDRs(nIndex).Item("TestDate").ToString
                maTestData(nIndex).OpHours = CType(mDRs(nIndex).Item("OpHours"), Integer)
                maTestData(nIndex).RefAwayCurrent = CType(mDRs(nIndex).Item("RefAwayCurrent"), Integer)
                maTestData(nIndex).RefTargetCurrent = CType(mDRs(nIndex).Item("RefTargetCurrent"), Integer)
                maTestData(nIndex).RefTestCurrent = CType(mDRs(nIndex).Item("RefTestCurrent"), Integer)
                maTestData(nIndex).AwayCurrent = CType(mDRs(nIndex).Item("AwayCurrent"), Integer)
                maTestData(nIndex).TargetCurrent = CType(mDRs(nIndex).Item("TargetCurrent"), Integer)
                maTestData(nIndex).TestCurrent = CType(mDRs(nIndex).Item("TestCurrent"), Integer)

                maTestData(nIndex).RefAwayCurrentOld = maTestData(nIndex).RefAwayCurrent
                maTestData(nIndex).RefTargetCurrentOld = maTestData(nIndex).RefTargetCurrent
                maTestData(nIndex).RefTestCurrentOld = maTestData(nIndex).RefTestCurrent
                maTestData(nIndex).AwayCurrentOld = maTestData(nIndex).AwayCurrent
                maTestData(nIndex).TargetCurrentOld = maTestData(nIndex).TargetCurrent
                maTestData(nIndex).TestCurrentOld = maTestData(nIndex).TestCurrent
            Next

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitializeForm", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

        'Update the Cumulative Operation Hours Warning and Fault limits in the PLC
        Call subWriteHourLimitsToPLC()

        'Read the Cumulative Operation Hours from the PLC and update maTestData
        Call subReadDataFromPLC()

        'Read the current Away and At Target values from the robot controllers and update maTestData
        Call subReadDataFromRobots()

        'Prepare the grids for data
        Call subFormatGrids()
        Call subInitFormText()

        'Populate the grids
        Call subUpdateDataSet()

        Status = "Ready"

    End Sub

    Private Sub subLogTestResults()
        '********************************************************************************************
        'Description: Log the Estat Test results upon completion of the Estat Test path.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sDate As String = Now.ToShortDateString

        'Read the current Away and At Target values from the robot controllers and update maTestData
        Call subReadDataFromRobots()

        'Update Test Date
        For nIndex As Integer = 0 To maTestData.GetUpperBound(0)
            With maTestData(nIndex)
                If .RobotDataUpdated Then
                    .TestDate = sDate
                    .RobotDataUpdated = False
                End If
            End With
        Next

        'Refresh the Test Data grid and update the EstatTest.xml file
        Call subUpdateDataSet()

    End Sub

    Private Sub subLoadGrids()
        '********************************************************************************************
        'Description: Populate the grids with data stored in maTestData.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        dgvRefData.Rows.Clear()
        dgvTestData.Rows.Clear()

        For nRow As Integer = 0 To maTestData.GetUpperBound(0)
            With dgvRefData
                .Rows.Add()
                .Rows(nRow).Height = 40
                .Item(eRefCol.Robot, nRow).Value = maTestData(nRow).ArmDisplayName
                .Item(eRefCol.TestDate, nRow).Value = maTestData(nRow).CalDate
                .Item(eRefCol.OpHours, nRow).Value = maTestData(nRow).OpHours.ToString
                .Item(eRefCol.Away, nRow).Value = maTestData(nRow).RefAwayCurrent.ToString
                .Item(eRefCol.AtTarget, nRow).Value = maTestData(nRow).RefTargetCurrent.ToString
            End With

            With dgvTestData
                .Rows.Add()
                .Rows(nRow).Height = 40
                .Item(eTestCol.Robot, nRow).Value = maTestData(nRow).ArmDisplayName
                .Item(eTestCol.TestDate, nRow).Value = maTestData(nRow).TestDate
                .Item(eTestCol.Away, nRow).Value = maTestData(nRow).AwayCurrent.ToString
                .Item(eTestCol.AtTarget, nRow).Value = maTestData(nRow).TargetCurrent.ToString
                .Item(eTestCol.Test, nRow).Value = maTestData(nRow).TestCurrent.ToString
                .Item(eTestCol.Record, nRow).Value = "<= Cable Replaced"
            End With
        Next 'nRow

    End Sub

    Private Sub subReadDataFromPLC()
        '********************************************************************************************
        'Description: Read Cumulative Operation Hours from the PLC and update data structures.
        '             
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            For nIndex As Integer = 0 To maTestData.GetUpperBound(0)
                With maTestData(nIndex)
                    'frmMain.mPLC.TagName = "RC_" & .ControllerNumber.ToString & "A" & .EquipNumber.ToString & "EstatHours"
                    frmMain.mPLC.TagName = "RC" & .ControllerNumber.ToString & "A" & .EquipNumber.ToString & "EstatHours"

                    Dim sData() As String = frmMain.mPLC.PLCData


                    If mbMitsubishiPLC Then
                        If Not sData(0) Is Nothing Then
                            Dim sDWordData() As String = sDwordFromMitsubishi(sData)
                            .OpHours = CType(sDWordData(0), Integer)
                        End If
                    Else

                        If Not sData(0) Is Nothing Then
                            .OpHours = CType(sData(0), Integer)
                        End If
                    End If
                End With
                Application.DoEvents()
            Next 'nIndex

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subReadDataFromPLC", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subReadDataFromRobots()
        '********************************************************************************************
        'Description: Read Test Data from robot controllers and update data structures.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sProgram As String = "*numreg*"
        Dim sVar As String
        Dim sVal As String

        Try
            For nIndex As Integer = 0 To maTestData.GetUpperBound(0)
                With maTestData(nIndex)
                    Dim oArm As clsArm = frmMain.colArms.Item(.ArmDisplayName)

                    If oArm.IsOnLine Then
                        Select Case sProgram
                            Case "*numreg*"
                                Dim oFRCRobott As FRRobot.FRCRobot = oArm.Controller.Robot
                                Dim oVars As FRRobot.FRCVars = oFRCRobott.RegNumerics
                                Dim nReg As Integer = ((.EquipNumber - 1) * 2) + BaseRegister

                                'Away from Target current
                                sVal = mRegVal.ReadNumReg(oVars, nReg)
                                .AwayCurrent = CType(sVal, Integer)

                                'At Target current
                                nReg += 1
                                sVal = mRegVal.ReadNumReg(oVars, nReg)
                                .TargetCurrent = CType(sVal, Integer)

                            Case Else 'Program variable
                                sVar = "EstatTest["
                                oArm.ProgramName = sProgram

                                'Away from Target current
                                oArm.VariableName = sVar & .EquipNumber & "].AwayCurr"
                                .AwayCurrent = CType(oArm.VarValue, Integer)

                                'At Target current
                                oArm.VariableName = sVar & .EquipNumber & "].TargCurr"
                                .TargetCurrent = CType(oArm.VarValue, Integer)
                        End Select

                        .RobotDataUpdated = True
                    Else
                        .RobotDataUpdated = False
                    End If 'oArm.IsOnLine
                End With 'maTestData(nIndex)
            Next 'nIndex

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subReadDataFromRobots", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subResetOpHours(ByVal ArmName As String)
        '********************************************************************************************
        'Description: Reset Cumulative Operation Hours for the specified arm in the PLC.
        '
        'Parameters: ArmName
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sData(1) As String

        sData(0) = "0"
        sData(1) = "0"

        Try
            For nIndex As Integer = 0 To maTestData.GetUpperBound(0)
                With maTestData(nIndex)
                    If .ArmDisplayName = ArmName Then
                        Dim sTag As String = "RC_" & .ControllerNumber.ToString & "A" & .EquipNumber.ToString

                        frmMain.mPLC.TagName = sTag & "EstatHours"
                        frmMain.mPLC.PLCData = sData

                        Application.DoEvents()

                        frmMain.mPLC.TagName = sTag & "EstatTimer"
                        frmMain.mPLC.PLCData = sData

                        Exit For
                    End If
                End With 'maTestData(nIndex)
            Next 'nIndex

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subResetOpHours", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subUpdateDataSet()
        '********************************************************************************************
        'Description: Data in maTestData has been updated. Compare to old data and log
        '             changes if necessary. Then update EstatTest.xml.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sChange As String

        For nIndex As Integer = 0 To mDRs.GetUpperBound(0)
            With maTestData(nIndex)
                'Log changes and update maTestData
                If .RefAwayCurrent <> .RefAwayCurrentOld Then
                    sChange = "Ref Away From Target current changed from "
                    sChange = sChange & .RefAwayCurrentOld.ToString & "uA to " & .RefAwayCurrent
                    AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                colZones.CurrentZoneNumber, .ArmDisplayName, _
                                                String.Empty, sChange, colZones.CurrentZone)
                    .RefAwayCurrentOld = .RefAwayCurrent
                End If
                If .RefTargetCurrent <> .RefTargetCurrentOld Then
                    sChange = "Ref At Target current changed from "
                    sChange = sChange & .RefTargetCurrentOld.ToString & "uA to " & .RefTargetCurrent & " uA"
                    AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                colZones.CurrentZoneNumber, .ArmDisplayName, _
                                                String.Empty, sChange, colZones.CurrentZone)
                    .RefTargetCurrentOld = .RefTargetCurrent
                End If

                If .AwayCurrent <> .AwayCurrentOld Then
                    sChange = "Away From Target current changed from "
                    sChange = sChange & .AwayCurrentOld.ToString & "uA to " & .AwayCurrent & " uA"
                    AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                colZones.CurrentZoneNumber, .ArmDisplayName, _
                                                String.Empty, sChange, colZones.CurrentZone)
                    .AwayCurrentOld = .AwayCurrent
                End If
                If .TargetCurrent <> .TargetCurrentOld Then
                    sChange = "At Target current changed from "
                    sChange = sChange & .TargetCurrentOld.ToString & "uA to " & .TargetCurrent & " uA"
                    AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                colZones.CurrentZoneNumber, .ArmDisplayName, _
                                                String.Empty, sChange, colZones.CurrentZone)
                    .TargetCurrentOld = .TargetCurrent
                End If


                'Update DataRow
                mDRs(nIndex).BeginEdit()
                mDRs(nIndex).Item("Caldate") = .CalDate
                mDRs(nIndex).Item("OpHours") = .OpHours
                mDRs(nIndex).Item("RefAwayCurrent") = .RefAwayCurrent
                mDRs(nIndex).Item("RefTargetCurrent") = .RefTargetCurrent
                mDRs(nIndex).Item("RefTestCurrent") = .RefTestCurrent
                mDRs(nIndex).Item("TestDate") = .TestDate
                mDRs(nIndex).Item("AwayCurrent") = .AwayCurrent
                mDRs(nIndex).Item("TargetCurrent") = .TargetCurrent
                mDRs(nIndex).EndEdit()
                mDRs(nIndex).AcceptChanges()
            End With 'maTestData(nIndex)
        Next

        'Update EstatTest.xml
        mDS.Tables("TestData").AcceptChanges()
        mDS.WriteXml(msXmlPath & "EstatTest.xml")

        'Update Change log
        'mPWCommon.SaveToChangeLog(colZones.DatabasePath)
        mPWCommon.SaveToChangeLog(colZones.ActiveZone)

        'Update grids 
        Call subLoadGrids()
        Call subCheckLimits()

        dgvRefData.Item(0, 0).Selected = False
        dgvTestData.Item(0, 0).Selected = False

        Status = "Ready"

    End Sub

    Private Sub subUpdateGrids()
        '********************************************************************************************
        'Description: Update the grids with data read from robot controllers and PLC.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Call subReadDataFromPLC()
        Call subReadDataFromRobots()
        Call subUpdateDataSet()
        Status = "Ready"

    End Sub

    Private Sub subWriteHourLimitsToPLC()
        '********************************************************************************************
        'Description: Write Cumulative Operation Hours Fault and Warning settings the PLC.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sData(0) As String
        Dim nPreset As Integer
        Dim nWarn As Integer

        Try
            For nIndex As Integer = 0 To maTestData.GetUpperBound(0)
                With maTestData(nIndex)

                    Dim sTag As String = "RC_" & .ControllerNumber.ToString & "A" & .EquipNumber.ToString

                    sData(0) = WarnHours.ToString
                    frmMain.mPLC.TagName = sTag & "EstatHoursWarn"
                    frmMain.mPLC.PLCData = sData

                    nPreset = CType(MaxHours, Integer)
                    nWarn = CType(WarnHours, Integer)

                    If mbMitsubishiPLC Then
                        sData(0) = (nPreset - nWarn).ToString
                        Dim sDWordData() As String = sDwordToMitsubishi(sData)

                        frmMain.mPLC.PLCData = sDWordData
                    Else
                        frmMain.mPLC.PLCData = sData
                    End If

                    Application.DoEvents()

                    sData(0) = MaxHours.ToString
                    frmMain.mPLC.TagName = sTag & "EstatHoursFault"
                    'frmMain.mPLC.PLCData = sData

                    If mbMitsubishiPLC Then
                        Dim sDWordData() As String = sDwordToMitsubishi(sData)

                        frmMain.mPLC.PLCData = sDWordData
                    Else
                        frmMain.mPLC.PLCData = sData
                    End If

                    Application.DoEvents()
                End With 'maTestData(nIndex)
            Next 'nIndex

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subWriteHourLimitsToPLC", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

#End Region

#Region " Mitsubishi PLC Specific Functions "

    Private Function sDwordFromMitsubishi(ByVal PLCData() As String) As String()
        Dim sReturnData(0) As String
        Dim nOutputDataLength As Integer = PLCData.GetLength(0) \ 2

        If nOutputDataLength = 0 Then
            sReturnData(0) = "0"
            Return sReturnData
        Else
            ReDim sReturnData(nOutputDataLength - 1)
            Dim nOffset As Integer
            Dim lDword As Long
            Dim lTemp As Long

            For nIndex As Integer = 0 To nOutputDataLength - 1
                lDword = 0

                'Least significant word
                Try
                    lDword = CType(PLCData(nOffset), Long)
                Catch ex As Exception
                    'Non-numeric data - do nothing
                End Try

                nOffset += 1
                lTemp = 0

                'Most significant word
                Try
                    lTemp = CType(PLCData(nOffset), Long) * CType(2 ^ 16, Long)
                Catch ex As Exception
                    'Non-numeric data - do nothing
                End Try

                lDword += lTemp
                sReturnData(nIndex) = lDword.ToString
                nOffset += 1
            Next
        End If
        Return sReturnData

    End Function

    Private Function sDwordToMitsubishi(ByVal DwordData() As String) As String()
        '********************************************************************************************
        'Description: Registers used as DWords in the Misubishi PLC are not stored as DWords by the
        '             ActEasyIF control. Each Dword is stored as 2 unsigned integers, Least significant 
        '             first - Most significant second.
        '
        'Parameters: DWord values
        'Returns:    DWord Data formatted to be written to the PLC
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sReturnData(0) As String
        Dim nOutputDataLength As Integer = DwordData.GetLength(0) * 2

        If nOutputDataLength = 0 Then
            sReturnData(0) = "0"
            Return sReturnData
        Else
            ReDim sReturnData(nOutputDataLength - 1)
            Dim nOffset As Integer
            Dim lDword As Long
            Dim lTemp As Long

            For nIndex As Integer = 0 To DwordData.GetUpperBound(0)
                lDword = 0

                Try
                    lDword = CType(DwordData(nIndex), Long)
                Catch ex As Exception
                    'Non-numeric data - do nothing
                End Try

                lTemp = lDword And &HFFFF
                sReturnData(nOffset) = lTemp.ToString
                nOffset += 1
                sReturnData(nOffset) = ((lDword - lTemp) \ CType(2 ^ 16, Long)).ToString
                nOffset += 1
            Next

            Return sReturnData

        End If

    End Function
#End Region

#Region "Events "

    Private Sub dgvTestData_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvTestData.CellClick
        '********************************************************************************************
        'Description: Test Data grid cell clicked. Process click if cell is in the Record as 
        '             Reference column.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If (e.ColumnIndex = eTestCol.Record) And (e.RowIndex >= 0) Then
            If maTestData(e.RowIndex).TestDataOk Then
                Dim Result As DialogResult
                Dim sMessage As String = "This action will replace the " & maTestData(e.RowIndex).ArmDisplayName & " "

                Status = "Updating Refernce Data"
                sMessage = sMessage & "Reference Data with the Current Test Data and Reset the Cumulative Operation Hours. "
                sMessage = sMessage & "Click OK to continue."
                Result = MessageBox.Show(sMessage, "Record as Reference", MessageBoxButtons.OKCancel, MessageBoxIcon.Information)

                If Result = Windows.Forms.DialogResult.OK Then
                    'Update reference data 
                    With maTestData(e.RowIndex)
                        .CalDate = .TestDate
                        .OpHours = 0
                        .RefAwayCurrent = .AwayCurrent
                        .RefTargetCurrent = .TargetCurrent
                        .RefTestCurrent = .TestCurrent
                    End With

                    Call subUpdateDataSet()
                    Call subResetOpHours(maTestData(e.RowIndex).ArmDisplayName)

                    'Write Change log
                    AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                                                colZones.CurrentZoneNumber, maTestData(e.RowIndex).ArmDisplayName, _
                                                 String.Empty, "Estat Test data recorded as reference", colZones.CurrentZone)
                    'mPWCommon.SaveToChangeLog(colZones.DatabasePath)
                    mPWCommon.SaveToChangeLog(colZones.ActiveZone)
                End If
                Status = "Ready"
            End If 'maTestData(e.RowIndex).TestDataOk
        End If 'e.ColumnIndex = eTestCol.Record

    End Sub

    Private Sub frmEstatTest_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description: Start the screen
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Call subInitializeForm()

    End Sub

    'Friend Shadows Function Show() As Boolean
    '    '********************************************************************************************
    '    'Description: Start the screen as a Dialog
    '    '
    '    'Parameters: none
    '    'Returns:    none
    '    '
    '    'Modification history:
    '    '
    '    ' Date      By      Reason
    '    '********************************************************************************************

    '    Me.StartPosition = FormStartPosition.CenterScreen
    '    Call subInitializeForm()

    '    Return MyBase.ShowDialog() = Windows.Forms.DialogResult.OK

    'End Function

    Private Sub tlsMain_ItemClicked(ByVal sender As Object, _
                        ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) _
                        Handles tlsMain.ItemClicked
        '********************************************************************************************
        'Description: Tool Bar button clicked
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case e.ClickedItem.Name
            Case "btnClose"
                Me.Close()
            Case "btnRefresh"
                Call subUpdateGrids()
            Case "btnStart"
                Call subExecuteTest()
        End Select

    End Sub

    Private Sub tmrPoll_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrPoll.Tick
        '********************************************************************************************
        'Description: Monitor the status of the Estat Test Ghost Path
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bPollActive As Boolean = True
        Dim nGhostStatus As Integer = CInt(frmMain.msZoneData(eBooth.GhostInfoWd))

        tmrPoll.Enabled = False

        If mbExecutionStarted Then
            If (nGhostStatus And eBooth.GhostAutoModeBit) = eBooth.GhostAutoModeBit Then
                If (nGhostStatus And eBooth.GhostRobotsReady) <> eBooth.GhostRobotsReady Then
                    mbExecutionStarted = False
                    mbExecutingTestPath = True
                    Status = "Executing Estat Test Path"
                End If
            Else
                bPollActive = False
                mbExecutionStarted = False
                Status = "Estat Test Path Aborted"
            End If 'GhostAutoModeBit = True
        End If 'mbExecutionStarted

        If mbExecutingTestPath Then
            If (nGhostStatus And eBooth.GhostAutoModeBit) = eBooth.GhostAutoModeBit Then
                If (nGhostStatus And eBooth.GhostRobotsReady) = eBooth.GhostRobotsReady Then
                    bPollActive = False
                    mbExecutingTestPath = False
                    Status = "Reading Test Results"
                    Call subLogTestResults()
                    Status = "Ready"
                End If
            Else
                bPollActive = False
                mbExecutingTestPath = False
                Status = "Estat Test Path Aborted"
            End If 'GhostAutoModeBit = True
        End If 'mbExecutingTestPath

        If Not bPollActive Then
            btnRefresh.Enabled = True
            btnStart.Enabled = True
        End If

        tmrPoll.Enabled = bPollActive

    End Sub

#End Region

    Private Sub btnStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStart.Click

    End Sub

    Private Sub dgvTestData_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvTestData.CellContentClick

    End Sub
End Class