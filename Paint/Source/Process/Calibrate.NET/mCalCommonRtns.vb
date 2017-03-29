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
' Form/Module: mCCCommonRtns
'
' Description: Routines for Color Change
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
'   Date        By      Reason                                                        Version
'   06/02/09    MSW     1st PW4 version
'   01/01/13    RJO     Added (Unused) Style parameter to LoadCopyScreenSubParamBox 
'                       for compatibility with frmCopy v4.01.01.03
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Collections.Generic
Imports System.Collections.ObjectModel


Module mCalCommonRtns

#Region " Declares "

    Friend Const gsPRESET_TABLE_PREFIX As String = "Presets "
    Friend Const gsFANUC_COLOR_COL As String = "Fanuc Color"
    Friend Const gsPRESET_COL As String = "Preset"
    Friend Const gsDESC_COL As String = "Description"
    Friend Const gsESTATDESC_COL As String = "Estat Description"    '6.20.08
    Friend Const gsVALVE_COL As String = "Valve"
    Friend Const gnSTEP_START_INDEX As Integer = 23
    Friend Const gnSTEP_OFFSET As Integer = 3
    Friend gStyleNameArray() As String
    Private moSWriter As StreamWriter = Nothing
    Private Const sLOGFILE As String = "C:\Temp\Presetcopy.log"
    Private Const bLOG As Boolean = False
    Delegate Sub UpdateStatus(ByVal progress As Integer, ByVal sStatus As String)
    Private mViewAllRobot As clsArm = Nothing
    Private mViewAllApplicator As clsApplicator
    Private mnCopyParm As Integer = 0
#End Region

#Region " Robot Communication Routines "
    Property CopyParm() As Integer
        Get
            Return mnCopyParm
        End Get
        Set(ByVal value As Integer)
            mnCopyParm = value
        End Set
    End Property
    Friend Sub subScreenDump(ByRef oForm As Form, ByVal sSubScreen As String)
        '********************************************************************************************
        'Description: screen capture request from copy or multiview
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
		'    09/30/13   MSW     Save screenshots as jpegs
        '********************************************************************************************
        'Screen Dump request
        Dim oSC As New ScreenShot.ScreenCapture
        Dim sDumpPath As String = String.Empty

        If sSubScreen <> String.Empty Then
            sSubScreen = "_" & sSubScreen
        End If
        mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)

        oSC.CaptureWindowToFile(oForm.Handle, sDumpPath & frmMain.msSCREEN_DUMP_NAME & sSubScreen & frmMain.msSCREEN_DUMP_EXT, Imaging.ImageFormat.Jpeg)

    End Sub

    Friend Sub subShowHelp(ByRef sSubScreen As String)
        '********************************************************************************************
        'Description: Help Screen request from copy or multiview
        '
        'Parameters: 
        'Returns:  
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Help Screen request
        'Help Screen request
        Select Case sSubScreen
            Case "Copy"
                subLaunchHelp(gs_HELP_CALIBRATION_COPY, frmMain.oIPC)
            Case "MultiView"
                subLaunchHelp(gs_HELP_CALIBRATION_MV, frmMain.oIPC)
            Case Else
                subLaunchHelp(gs_HELP_CALIBRATION, frmMain.oIPC)
        End Select

    End Sub

    Private Function bGetColorChange(ByRef oRobot As clsArm, ByRef Applicator As clsApplicator, _
                                     ByRef rCalTable As clsCalibration, _
                                    ByRef oCopy As frmCopy, _
                                    ByVal bSourceCC As Boolean) As Boolean
        '********************************************************************************************
        'Description:  get the cal table from the robot
        '
        'Parameters: 
        'Returns:    True if  success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp As String = String.Empty

        Try


            If bSourceCC Then
                sTmp = gcsRM.GetString("csSELECTED_AS_SOURCE")
            Else
                sTmp = gcsRM.GetString("csSELECTED_AS_TARGET")
            End If

            oCopy.Status = oRobot.Name & " " & sTmp

            If bSourceCC Then
                sTmp = gpsRM.GetString("psLOADING_SOURCE")
            Else
                sTmp = gpsRM.GetString("psLOADING_TARGET")
            End If

            'oCopy.Status = sTmp
            'If oRobot.IsOnLine Then
            '    If LoadColorChangeFromRobot(oRobot, oColorChange, , , , sCycle) Then

            '        If frmMain.bLoadFromGUI(oRobot.Controller.Zone, oRobot, oColorChange) = False Then
            '            'something failed bomb out... need continue to next here somehow
            '            Return False
            '        End If
            '    Else
            '        'something failed bomb out... need continue to next here somehow
            '        Return False
            '    End If
            'Else
            '    Return False
            'End If


            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try

    End Function
    Private Function bGetTargetArms(ByRef oZones As clsZones, ByRef oTargetCollection As clsArms, _
                                    ByRef oRefCollection As clsArms) As Boolean
        '********************************************************************************************
        'Description:  get the arms for the passed in zone, and select based on input collection
        '
        'Parameters: 
        'Returns:    True if copy success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/08/10  MSW     Exclude openers
        '********************************************************************************************
        Try

            Dim colControllersTo As New clsControllers(oZones, False)
            oTargetCollection = LoadArmCollection(colControllersTo, False) '4/08/10
            'could be different names, different number of arms etc - just blindly using index
            For i As Integer = 0 To oRefCollection.Count - 1
                If i > oTargetCollection.Count - 1 Then Exit For
                If oRefCollection.Item(i).Selected Then oTargetCollection.Item(i).Selected = True
            Next

            'make sure system colors are loaded in target collection as these are new clsarms
            For i As Integer = 0 To oTargetCollection.Count - 1
                If oTargetCollection.Item(i).Selected Then
                    ' if i ever catch the guy who wrote this @#!@$...
                    oTargetCollection.Item(i).SystemColors.Load(oTargetCollection.Item(i))
                End If
            Next

            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try

    End Function
    Friend Function DoCopy(ByRef colZoneFrom As clsZones, ByRef colZoneTo As clsZones, _
                                ByRef colRobotFrom As clsArms, ByRef colRobotTo As clsArms, _
                                ByRef colParamFrom As Collection(Of String), _
                                ByRef colParamTo As Collection(Of String), _
                                ByRef colSubParamFrom As Collection(Of String), _
                                ByRef colSubParamTo As Collection(Of String), _
                                ByVal CopyType As eCopyType, ByRef oCopy As frmCopy, _
                                Optional ByRef colStyleFrom As Collection(Of String) = Nothing, _
                                Optional ByRef colStyleTo As Collection(Of String) = Nothing) As Boolean
        '********************************************************************************************
        'Description:  Copy Data Selected from copy screen
        '
        'Parameters: collections of item selected
        'Returns:    True if copy success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/21/07  gks     This was originally set up to use the display name to locate the colors
        '                   to try to get away from the index thing. worked fine until the paint shop
        '                   computer tried to copy across mods where system colors had different 
        '                   display names - now trying fanuc number
        '********************************************************************************************
        Dim oSourceRobot As clsArm
        Dim sSourceValveNum As String
        Dim oTargetRobot As clsArm
        Dim sTargetValveNum As String
        Dim colTargetArms As clsArms = Nothing

        oCopy.Status = gcsRM.GetString("csCOPY_STARTED")

        Try

            'Calibration copy
            '  get target zone data
            For Each oZone As clsZone In colZoneTo
                If oZone.Selected Then
                    colZoneTo.CurrentZone = oZone.Name
                    ' get arms from selected zone(s) 1 zone at a time
                    bGetTargetArms(colZoneTo, colTargetArms, colRobotTo)

                    'the source is coming from a single zone
                    For Each oSourceRobot In colRobotFrom
                        If oSourceRobot.Selected Then
                            For Each sSourceValveNum In colParamFrom

                                'Source cal data
                                Dim oSourceCalibration As New clsCalibration()                                'turn the valve number into fanuc color and assign the sys color setup
                                Dim nSourceFanuColor As Integer = CInt(sSourceValveNum)
                                'turn the valve number into fanuc color and assign the sys color setup
                                oSourceCalibration.ParmNum = CopyParm
                                oSourceCalibration.Valve = oSourceRobot.SystemColors(nSourceFanuColor, True).Valve.Number.Value
                                oSourceCalibration.LoadFromRobot(oSourceRobot)

                                'Loop through each target arm
                                For Each oTargetRobot In colTargetArms
                                    ' if there is more than 1 source robot - set equal to source robot index
                                    ' and bail at bottom of for loop. Other wise loop thru all selected
                                    If colRobotFrom.SelectedCount > 1 Then
                                        ' robot index starts at 1 - should be selected from bGetTargetArms
                                        oTargetRobot = colTargetArms(oSourceRobot.Name)
                                    End If

                                    If oTargetRobot.Selected Then
                                        ' loop thru colors - parameter collections are only loaded with parameters that
                                        ' are selected
                                        For Each sTargetValveNum In colParamTo
                                            ' if there is more than 1 source param - set equal to source param index
                                            ' and bail at bottom of for loop. Other wise loop thru all 
                                            If colParamFrom.Count > 1 Then
                                                ' align on index
                                                sTargetValveNum = sSourceValveNum
                                            End If  'colParamFrom.Count > 1

                                            Dim nTargetFanuColor As Integer = CInt(sTargetValveNum)
                                            'Select the target valve
                                            oSourceCalibration.Valve = oTargetRobot.SystemColors(nTargetFanuColor, True).Valve.Number.Value
                                            'save cal data
                                            oCopy.Status = gcsRM.GetString("csWRITING_TARGET")
                                            If oSourceCalibration.SaveToRobot(oTargetRobot, True) Then
                                                UpdateChangeLogCopy(oSourceCalibration.Name & gpsRM.GetString("psCALIBRATION"), oSourceRobot.Name, oTargetRobot.Name, sTargetValveNum, _
                                                                    oTargetRobot.ZoneNumber, oTargetRobot.Controller.Zone.Name)
                                                oCopy.Status = gcsRM.GetString("csWRITE_SUCCESS")
                                                oCopy.Status = String.Empty
                                            Else
                                                Return False
                                            End If
                                            ' multiple from colors - just do 1 color
                                            If colParamFrom.Count > 1 Then Exit For
                                        Next 'sTargetValveNum
                                    End If  'oTargetRobot.Selected

                                    'multiple from selections - just do 1 robot
                                    If colRobotFrom.SelectedCount > 1 Then Exit For
                                Next 'oTargetRobot

                            Next ' sSourceValveNum
                        End If ' oSourceRobot.Selected
                    Next 'oSourceRobot In colRobotFrom
                End If 'oZone.Selected
            Next 'oZone


        Catch ex As Exception

            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            mDebug.WriteEventToLog(ex.Message, frmMain.msSCREEN_NAME)

            Return False

        End Try

        Return True

    End Function
    Private Function bActuallyDoTheCopy(ByRef rSourceRobot As clsArm, ByRef rTargetRobot As clsArm, _
                    ByRef rSourceColorChange As clsCalibration, ByRef clsCalibration As clsColorChange, _
                    ByRef rSourceColorChangeToCopy As Collection(Of String), _
                    ByRef rTargetColorChangeToCopy As Collection(Of String), _
                    ByVal CopyType As eCopyType) As Boolean
        '********************************************************************************************
        'Description:  Copy the data we just loaded
        '
        'Parameters: none
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason      
        '5/8/07     gks     Copy by param 
        '********************************************************************************************
        '        'set desired values
        '        rTargetColorChange.Cycle(nCycleNum).NumberOfSteps = rSourceColorChange.Cycle(nCycleNum).NumberOfSteps
        '        For nStep = 1 To rSourceColorChange.Cycle(nCycleNum).NumberOfSteps
        '            rTargetColorChange.Cycle(nCycleNum).Steps(nStep).Duration = rSourceColorChange.Cycle(nCycleNum).Steps(nStep).Duration
        '            rTargetColorChange.Cycle(nCycleNum).Steps(nStep).DoutDC = rSourceColorChange.Cycle(nCycleNum).Steps(nStep).DoutDC
        '            rTargetColorChange.Cycle(nCycleNum).Steps(nStep).DoutState = rSourceColorChange.Cycle(nCycleNum).Steps(nStep).DoutState
        '            rTargetColorChange.Cycle(nCycleNum).Steps(nStep).GoutDC = rSourceColorChange.Cycle(nCycleNum).Steps(nStep).GoutDC
        '            rTargetColorChange.Cycle(nCycleNum).Steps(nStep).GoutState = rSourceColorChange.Cycle(nCycleNum).Steps(nStep).GoutState
        '            rTargetColorChange.Cycle(nCycleNum).Steps(nStep).Preset = rSourceColorChange.Cycle(nCycleNum).Steps(nStep).Preset
        '            rTargetColorChange.Cycle(nCycleNum).Steps(nStep).StepAction = rSourceColorChange.Cycle(nCycleNum).Steps(nStep).StepAction
        '            rTargetColorChange.Cycle(nCycleNum).Steps(nStep).StepEvent = rSourceColorChange.Cycle(nCycleNum).Steps(nStep).StepEvent
        '        Next
        '        'reset the change status so it doesn't log a change for each bit
        '        rTargetColorChange.Cycle(nCycleNum).Update()
        '        If rTargetRobot.IsOnLine Then
        '            If SaveColorChangeToRobot(rTargetRobot, rTargetColorChange, , , , , True, rTargetColorChange.Cycle(nCycleNum).CycleName) Then
        '                With rTargetColorChange.Cycle(nCycleNum)
        '                    UpdateChangeLogCopy(.CycleName, rSourceRobot.Name, rTargetRobot.Name, rTargetColorChange.ValveDescription, _
        '                                        rTargetRobot.ZoneNumber, rTargetRobot.Controller.Zone.Name)
        '                End With
        '            Else
        '                Return False
        '            End If
        '        Else
        '            Return False
        '        End If
        '    End If
        'Next
        ''changelog
        'mPWCommon.SaveToChangeLog(rTargetRobot.DatabasePath)
        ''For SQL database - remove above eventually
        'mPWCommon.SaveToChangeLog(rTargetRobot.Controller.Zone)


        Return True

    End Function
    Friend Function GetViewAllRobot(ByRef rArm As clsArm, ByVal ZoneName As String, _
                        ByVal RobotName As String, ByVal DatabasePath As String) As Boolean
        '********************************************************************************************
        'Description:  get a robot for the viewall screen
        '
        'Parameters: none
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim oZones As New clsZones(DatabasePath)
            oZones.CurrentZone = ZoneName

            Dim oControllers As clsControllers = New clsControllers(oZones, False)
            Dim oArms As clsArms = LoadArmCollection(oControllers)
            rArm = oArms.Item(RobotName)
            mViewAllRobot = rArm
            Return True

        Catch ex As Exception

            Trace.WriteLine(ex.Message)
            mDebug.WriteEventToLog(ex.Message, frmMain.msSCREEN_NAME)
            Return False

        End Try

    End Function
    Friend Function GetViewAllData(ByVal ZoneName As String, ByVal RobotName As String, _
                                    ByVal rcboParamName As ComboBox, ByVal rcboSubParamName As ComboBox, _
                                    ByVal DatabasePath As String) As DataSet
        '********************************************************************************************
        'Description:  Load data stored on Robot
        '
        'Parameters: Zone name, Robot name, valve description, parameter, DB path, arm
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mViewAllRobot.IsOnLine Then
            Dim ParamName As String = rcboParamName.Text
            Dim SubParamName As String = rcboSubParamName.Text

            'Get the valve and parameter number
            Dim lCalTable As New clsCalibration
            If rcboParamName.SelectedIndex >= 0 Then
                lCalTable.Valve = mViewAllRobot.SystemColors.ByValve(ParamName).Valve.Number.Value 'CInt(sTag(rcboParamName.SelectedIndex))
            Else
                lCalTable.Valve = 1
            End If
            '
            Dim nTag() As Integer = DirectCast(rcboSubParamName.Tag, Integer())
            'The tag is the robot's parm number starting with 1, all the data starts at 0
            Dim nParam As Integer = nTag(rcboSubParamName.SelectedIndex) - 1
            lCalTable.ParmNum = nParam
            If lCalTable.LoadFromRobot(mViewAllRobot) Then
                Return CreateViewAllDataset(lCalTable, mViewAllRobot)
            Else
                Return Nothing
            End If
        Else
            MessageBox.Show(gcsRM.GetString("csCOULD_NOT_CONNECT"), _
                    mViewAllRobot.Name, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return Nothing
        End If

    End Function
    Friend Function ValveChanged(ByVal nValve As Integer, ByVal nOldDC As Integer, ByVal nNewDC As Integer, _
                                 ByVal nOldState As Integer, ByVal nNewState As Integer, _
                                 ByRef sOld As String, ByRef sNew As String) As Boolean
        Dim nBitMask As Integer = CType(2 ^ (nValve - 1), Integer)
        Dim nOldValveState As Integer
        If ((nOldState And nBitMask) > 0) Then
            'select each valve that's on in the step data
            nOldValveState = CheckState.Checked
        ElseIf ((nOldDC And nBitMask) > 0) Then
            nOldValveState = CheckState.Indeterminate
        Else
            nOldValveState = CheckState.Unchecked
        End If
        Dim nNewValveState As Integer
        If ((nNewState And nBitMask) > 0) Then
            'select each valve that's on in the step data
            nNewValveState = CheckState.Checked
        ElseIf ((nNewDC And nBitMask) > 0) Then
            nNewValveState = CheckState.Indeterminate
        Else
            nNewValveState = CheckState.Unchecked
        End If
        If nNewValveState <> nOldValveState Then
            Select Case nNewValveState
                Case CheckState.Checked
                    sNew = gpsRM.GetString("psON")
                Case CheckState.Unchecked
                    sNew = gpsRM.GetString("psOFF")
                Case CheckState.Indeterminate
                    sNew = gpsRM.GetString("psDC")
            End Select
            Select Case nOldValveState
                Case CheckState.Checked
                    sOld = gpsRM.GetString("psON")
                Case CheckState.Unchecked
                    sOld = gpsRM.GetString("psOFF")
                Case CheckState.Indeterminate
                    sOld = gpsRM.GetString("psDC")
            End Select
            Return True
        Else
            Return False
        End If
    End Function
    Friend Function LoadCopyScreenSubParamBox(ByRef oRobot As clsArm, _
                            ByRef clbBox As CheckedListBox, ByVal ParamName As String, ByVal Style As Integer) As Boolean
        '********************************************************************************************
        'Description:  load preset box from preset copy screen - needs to match routine for colorchg
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/01/13  RJO     Added (Unused) Style  parameter for compatibility with frmCopy v4.01.01.03
        '********************************************************************************************
        Dim void As New DataSet
        If oRobot.Applicator Is Nothing Then
            oRobot.Applicator = frmMain.mApplicator
        End If
        Dim oColorChange As clsColorChange = oRobot.SystemColors(0).Valve.ColorChange

        oColorChange.subGetCCInfo()

        clbBox.Items.Clear()
        clbBox.BeginUpdate()

        For nCycle As Integer = 1 To oColorChange.NumberOfCycles
            clbBox.Items.Add(oColorChange.Cycle(nCycle).CycleName)
        Next

        clbBox.EndUpdate()

        Return True

    End Function
    Friend Sub UpdateChangeLog(ByVal sItem As String, _
                               ByRef IntValue As clsIntValue, _
                               ByVal Device As String, ByVal Color As String, _
                               ByVal ZoneNumber As Integer, ByVal ZoneName As String)
        '********************************************************************************************
        'Description:  build up the change text
        '
        'Parameters: if item has a number its a preset - if 0 its an override
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        UpdateChangeLog(sItem, IntValue.Value.ToString, IntValue.OldValue.ToString, _
                             Device, Color, ZoneNumber, ZoneName)
        IntValue.Update()
    End Sub
    Friend Sub UpdateChangeLog(ByVal sItem As String, _
                           ByRef SngValue As clsSngValue, _
                           ByVal Device As String, ByVal Color As String, _
                           ByVal ZoneNumber As Integer, ByVal ZoneName As String)
        '********************************************************************************************
        'Description:  build up the change text
        '
        'Parameters: if item has a number its a preset - if 0 its an override
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        UpdateChangeLog(sItem, SngValue.Value.ToString, SngValue.OldValue.ToString, _
                             Device, Color, ZoneNumber, ZoneName)
        SngValue.Update()
    End Sub

    Friend Sub UpdateChangeLog(ByRef sItem As String, _
                               ByRef NewValue As String, ByRef OldValue As String, _
                               ByRef Device As String, ByRef Color As String, _
                               ByVal ZoneNumber As Integer, ByRef ZoneName As String)
        '********************************************************************************************
        'Description:  build up the change text
        '
        'Parameters: if item has a number its a preset - if 0 its an override
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp As String = String.Empty

        sTmp = sItem & " " & gcsRM.GetString("csCHANGED_FROM") & " " & OldValue & " " & _
                 gcsRM.GetString("csTO") & " " & NewValue
        AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                    ZoneNumber, Device, Color, sTmp, ZoneName)

    End Sub
    Friend Sub UpdateChangeLogCopy(ByVal sCalName As String, _
                               ByVal sSrcDevice As String, _
                               ByVal Device As String, ByVal Color As String, _
                               ByVal ZoneNumber As Integer, ByVal ZoneName As String)
        '********************************************************************************************
        'Description:  build up the change text
        '
        'Parameters: if item has a number its a preset - if 0 its an override
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp As String = String.Empty

        sTmp = gcsRM.GetString("csCOPIED") & " " & sCalName & " " & gcsRM.GetString("csFROM") & _
                sSrcDevice & " " & gcsRM.GetString("csTO") & " " & Device
        AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                    ZoneNumber, Device, Color, sTmp, ZoneName)

    End Sub
    Friend Function CreateViewAllDataset(ByRef rCalTable As clsCalibration, ByRef rArm As clsArm) As DataSet
        '********************************************************************************************
        'Description:  create a dataset from the preset data
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Const sSETPOINT As String = "1"
        Const sOUTPUT As String = "2"
        Const sREFOUT As String = "3"
        Const sOP As String = "3"
        Const sMP As String = "4"

        Dim DT As DataTable = New DataTable
        Dim DR As DataRow = Nothing
        Dim DS As DataSet = New DataSet
        'Title
        If (rCalTable.CalSource = eCalSource.CAL_SRC_BC) Or _
           (rCalTable.CalSource = eCalSource.CAL_SRC_IPC_CAL) Then
            DT.TableName = rArm.Name & "   " & gpsRM.GetString("psVALVE") & " " & rCalTable.Valve
        Else
            DT.TableName = rArm.Name & "   " & rCalTable.Name
        End If
        'label Column
        DT.Columns.Add(gsALL_INDEX)
        For nRow As Integer = 0 To rCalTable.NumCalPoints - 1
            DR = DT.NewRow()
            DT.Rows.Add(DR)
            'Labels 
            DT.Rows(nRow).Item(gsALL_INDEX) = (nRow + 1).ToString
        Next

        'They all get a setpoint column
        If rCalTable.IPC2KTable Then
            DT.Columns.Add("1")
            DT.Columns(1).Caption = gpsRM.GetString("psRESIN") & " " & mViewAllApplicator.FlowTestLabel(rCalTable.ParmNum)
            DT.Columns.Add("2")
            DT.Columns(2).Caption = gpsRM.GetString("psRESIN") & " " & gpsRM.GetString("psOUTLET_PRESSURE") & " (" & gpsRM.GetString("psPSI") & ")"
            DT.Columns.Add("3")
            DT.Columns(3).Caption = gpsRM.GetString("psHARDENER") & " " & mViewAllApplicator.FlowTestLabel(rCalTable.ParmNum)
            DT.Columns.Add("4")
            DT.Columns(4).Caption = gpsRM.GetString("psHARDENER") & " " & gpsRM.GetString("psOUTLET_PRESSURE") & " (" & gpsRM.GetString("psPSI") & ")"
            For nRow As Integer = 0 To rCalTable.NumCalPoints - 1
                DT.Rows(nRow).Item("1") = rCalTable.CMD(nRow).Value.ToString
                DT.Rows(nRow).Item("2") = rCalTable.OUTPUT(nRow).Value.ToString
                DT.Rows(nRow).Item("3") = rCalTable.SCALE(nRow).Value.ToString
                DT.Rows(nRow).Item("4") = rCalTable.REF_OUT(nRow).Value.ToString
            Next
        Else
            DT.Columns.Add(sSETPOINT)
            DT.Columns(1).Caption = mViewAllApplicator.FlowTestLabel(rCalTable.ParmNum)
            'dynamic table?
            If rCalTable.DynamicTable Then
                DT.Columns.Add(sOUTPUT)
                DT.Columns.Add(sREFOUT)
                DT.Columns(2).Caption = gpsRM.GetString("psDYNAMIC") & " " & gpsRM.GetString("psCOUNTS")
                DT.Columns(3).Caption = gpsRM.GetString("psREFERENCE") & " " & gpsRM.GetString("psCOUNTS")
            Else
                DT.Columns.Add(sOUTPUT)
                DT.Columns(2).Caption = gpsRM.GetString("psOUTPUT") & " " & gpsRM.GetString("psCOUNTS")
                If rCalTable.OutletPressure Then
                    DT.Columns.Add(sOP)
                    DT.Columns(3).Caption = gpsRM.GetString("psOUTLET_PRES")
                End If
                If rCalTable.ManifoldPressure Then
                    DT.Columns.Add(sMP)
                    DT.Columns(4).Caption = gpsRM.GetString("psMANIFOLD_PRES")
                End If
            End If
            For nRow As Integer = 0 To rCalTable.NumCalPoints - 1
                DT.Rows(nRow).Item(sSETPOINT) = rCalTable.CMD(nRow).Value.ToString
                If rCalTable.DynamicTable Then
                    DT.Rows(nRow).Item(sOUTPUT) = rCalTable.OUTPUT(nRow).Value.ToString
                    DT.Rows(nRow).Item(sREFOUT) = rCalTable.REF_OUT(nRow).Value.ToString
                Else
                    DT.Rows(nRow).Item(sOUTPUT) = rCalTable.OUTPUT(nRow).Value.ToString
                    If rCalTable.OutletPressure Then
                        DT.Rows(nRow).Item(sOP) = rCalTable.OutletPressurePSI(nRow).ToString
                    End If
                    If rCalTable.ManifoldPressure Then
                        DT.Rows(nRow).Item(sMP) = rCalTable.ManifoldPressurePSI(nRow).ToString
                    End If
                End If
            Next
        End If


        DS.Tables.Add(DT)
        Return DS

    End Function


    Friend Function LoadMultiScreenSubParameterBox(ByRef rCbo As ComboBox, ByRef colZones As clsZones, _
                ByVal ParamName As String, ByVal bAddAll As Boolean) As Boolean
        '********************************************************************************************
        'Description: This Routine is called from frmAll when needed
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'Load  the appliucator parameter list
        mViewAllApplicator = New clsApplicator(mViewAllRobot, colZones.ActiveZone)
        rCbo.Items.Clear()
        mViewAllApplicator.LoadParameterBox(rCbo, True)
    End Function
#End Region

End Module
