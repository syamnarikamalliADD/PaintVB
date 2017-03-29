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
' Form/Module: mAreaVolumes.vb
'
' Description: access area DB data 
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Rick O.
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    08/23/13   MSW     First version, move area logic from volume screen             4.01.05.00
'                       so I can use it in manual cycle screen
'    09/30/13   MSW     PLC DLL                                                       4.01.06.00
'    12/03/13   MSW     Add nAVStatusInt                                              4.01.06.01
'    02/13/14   MSW     Fairfax sealer updates                                        4.01.07.00
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On


Imports clsPLCComm = PLCComm.clsPLCComm

Friend Module mAreaVolumes

#Region " Declares "


    Friend Enum eAreaVolCmds
        GetData = 1
        SaveData = 2
        ResetCurrentRobot = 4
        ResetAllRobots = 8
        SetWarnTolCurrentRobot = 16
        SetWarnTolAllRobots = 32
        SetFaultTolCurrentRobot = 64
        SetFaultTolAllRobots = 128
    End Enum


    Friend Structure udsAreaSettings
        Public MaxPads As Integer
        Public SysFaultOldValue As Boolean
        Public SysFaultNewValue As Boolean
        Public UseCommonDescOldValue As Boolean
        Public UseCommonDescNewValue As Boolean
    End Structure


    Friend Structure udsArea
        Public Area As Integer
        Public DescriptionOldValue As String
        Public DescriptionNewValue As String
        Public Status As String
        Public StatusBackColor As Color
        Public Actual As Integer
        Public LearnedOldValue As Integer
        Public LearnedNewValue As Integer
        Public WarnTolOldValue As Integer
        Public WarnTolNewValue As Integer
        Public FaultTolOldValue As Integer
        Public FaultTolNewValue As Integer
    End Structure
#End Region



#Region " Routines "
    Friend Function nAVStatusInt(ByRef sStatus As String) As Integer
        '********************************************************************************************
        'Description: Return  Status number
        '
        'Parameters: Status (number), Status Text to be returned, BackColor to be returned
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        With gcsRM
            Select Case sStatus
                Case .GetString("csAV_STATUS_PASS")
                    Return 1
                Case .GetString("csAV_STATUS_HIFLT")
                    Return 2
                Case  .GetString("csAV_STATUS_HIWARN")
                    Return 4
                Case  .GetString("csAV_STATUS_LOFLT")
                    Return 8
                Case  .GetString("csAV_STATUS_LOWARN")
                    Return 16
                Case Else
                    Return 0
            End Select
        End With

    End Function
    Friend Sub subGetAVStatus(ByVal Status As String, ByRef StatusText As String, ByRef BackColor As Color)
        '********************************************************************************************
        'Description: Return Text and BackColor for Area Volume Status
        '
        'Parameters: Status (number), Status Text to be returned, BackColor to be returned
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        With gcsRM
            Select Case Status
                Case "1"
                    StatusText = .GetString("csAV_STATUS_PASS")
                    BackColor = Color.LimeGreen
                Case "2"
                    StatusText = .GetString("csAV_STATUS_HIFLT")
                    BackColor = Color.Red
                Case "4"
                    StatusText = .GetString("csAV_STATUS_HIWARN")
                    BackColor = Color.Yellow
                Case "8"
                    StatusText = .GetString("csAV_STATUS_LOFLT")
                    BackColor = Color.Red
                Case "16"
                    StatusText = .GetString("csAV_STATUS_LOWARN")
                    BackColor = Color.Yellow
                Case Else
                    StatusText = .GetString("csAV_STATUS_UNKNOWN")
                    BackColor = Color.White
            End Select
        End With

    End Sub

    Friend Function bReadAreaVolsFromPLC(ByRef oZone As clsZone, ByRef oPLC As clsPLCComm, ByRef oAreaSettings As udsAreaSettings, _
                                                ByRef oAreas() As udsArea, ByVal Robot As Integer, ByVal Style As Integer) As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the Area Volume Alarm data for the selected 
        '             Robot and Style from the PLC.
        '
        'Parameters: Robot (number) and Style (number)
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Dim sPLCData(4) As String
        Dim sTag As String = oZone.PLCTagPrefix

        Try
            With oPLC
                'Send Fetch Command
                sPLCData(0) = Robot.ToString
                sPLCData(1) = Style.ToString
                sPLCData(2) = CType(eAreaVolCmds.GetData, Integer).ToString
                sPLCData(3) = "0"
                sPLCData(4) = "0"
                .TagName = sTag & "AreaVolumeCmd"
                .PLCData = sPLCData
                Thread.Sleep(100) 'TODO - May need to verify command accepted

                'Read Area Volume Data
                sPLCData = Nothing
                .TagName = sTag & "AreaVolumeData"
                sPLCData = .PLCData
            End With

            'Populate mAreas
            ReDim oAreas(oAreaSettings.MaxPads)

            For nArea As Integer = 1 To oAreaSettings.MaxPads
                Dim nIndex As Integer = (nArea - 1) * 5 '5 PLC registers per Area

                With oAreas(nArea)
                    .Area = nArea
                    Call subGetAVStatus(sPLCData(nIndex), .Status, .StatusBackColor)
                    .Actual = CType(sPLCData(nIndex + 1), Integer)
                    .LearnedNewValue = CType(sPLCData(nIndex + 2), Integer)
                    .LearnedOldValue = .LearnedNewValue
                    .WarnTolNewValue = CType(sPLCData(nIndex + 3), Integer)
                    .WarnTolOldValue = .WarnTolNewValue
                    .FaultTolNewValue = CType(sPLCData(nIndex + 4), Integer)
                    .FaultTolOldValue = .FaultTolNewValue
                End With
            Next 'nArea

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", frmMain.DisplayCulture), ex, frmMain.msSCREEN_NAME, _
                                frmMain.Status, MessageBoxButtons.OK)
            Return False
        End Try

        Return True

    End Function

    Friend Function bReadAVDescriptionsFromGUI(ByRef oZone As clsZone, ByRef oAreaSettings As udsAreaSettings, _
                                                ByRef oAreas() As udsArea, ByRef oAreaDS As DataSet, ByVal Style As Integer) As Boolean
        '********************************************************************************************
        'Description: Popluate the Description fields in mAreas from AreaVolumes.XML.
        '
        'Parameters: Style (number)
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************

        If oAreaSettings.UseCommonDescNewValue Then Style = 0

        Try
            Dim DRs() As DataRow = oAreaDS.Tables("Pad").Select("StyleNumber = " & Style.ToString, "PadNumber ASC")
            Dim nRows As Integer = DRs.GetLength(0)
            Dim nPadsFound As Integer = 0

            For nPad As Integer = 1 To oAreaSettings.MaxPads

                oAreas(nPad).DescriptionOldValue = String.Empty
                oAreas(nPad).DescriptionNewValue = String.Empty
                If nPadsFound < nRows Then
                    For Each DR As DataRow In DRs
                        If CType(DR.Item("PadNumber"), Integer) = nPad Then
                            oAreas(nPad).DescriptionNewValue = DR.Item("Description").ToString
                            oAreas(nPad).DescriptionOldValue = oAreas(nPad).DescriptionNewValue
                            nPadsFound += 1
                            Exit For
                        End If
                    Next 'DR
                End If 'nPadsFound < nRows

                If oAreas(nPad).DescriptionOldValue = String.Empty Then
                    'Make up a default description
                    If Style > 0 Then oAreas(nPad).DescriptionNewValue = gpsRM.GetString("psSTYLE") & " " & _
                                                                         Style.ToString & " - "
                    oAreas(nPad).DescriptionNewValue += gpsRM.GetString("psPAD") & " " & nPad.ToString
                End If
            Next 'nPad

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", frmMain.DisplayCulture), ex, frmMain.msSCREEN_NAME, _
                                frmMain.Status, MessageBoxButtons.OK)
            Return False
        End Try

        Return True

    End Function
    Friend Function bInitAreaDataSet(ByRef oZone As clsZone, ByRef oAreaSettings As udsAreaSettings, _
                                     ByRef dtAreaDS As DataSet) As Boolean
        '********************************************************************************************
        'Description: This function sets up tthe area colume data set
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************

        Try
            dtAreaDS = New DataSet
            dtAreaDS.ReadXmlSchema(oZone.XMLPath & "AreaVolumes.xsd")
            dtAreaDS.ReadXml(oZone.XMLPath & "AreaVolumes.XML")
            oAreaSettings.UseCommonDescOldValue = CType(dtAreaDS.Tables("Settings").Rows(0).Item("CommonDesc"), Boolean)
            oAreaSettings.UseCommonDescNewValue = oAreaSettings.UseCommonDescOldValue

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", frmMain.DisplayCulture), ex, frmMain.msSCREEN_NAME, _
                                frmMain.Status, MessageBoxButtons.OK)
            Return False
        End Try

        Return True
    End Function
    Friend Function bReadAVSettingsFromPLC(ByRef oZone As clsZone, ByRef oAreaSettings As udsAreaSettings, _
                                                ByRef oPLC As clsPLCComm) As Boolean
        '********************************************************************************************
        'Description: This function attempts to read the Area Volume Alarm settings from the PLC.
        '
        'Parameters: None
        'Returns:    True if the read is successful, false otherwise
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Dim sPLCData() As String
        Dim sTag As String = oZone.PLCTagPrefix

        Try
            oPLC.TagName = sTag & "AreaMaxPads"
            sPLCData = oPLC.PLCData
            oAreaSettings.MaxPads = CType(sPLCData(0), Integer)

            sPLCData = Nothing
            oPLC.TagName = sTag & "AreaAlarmsEnabled"
            sPLCData = oPLC.PLCData
            If sPLCData(0) = "1" Then
                oAreaSettings.SysFaultNewValue = True
            Else
                oAreaSettings.SysFaultNewValue = False
            End If
            oAreaSettings.SysFaultOldValue = oAreaSettings.SysFaultNewValue

            Return True

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED", frmMain.DisplayCulture), ex, frmMain.msSCREEN_NAME, _
                                frmMain.Status, MessageBoxButtons.OK)
            Return False
        End Try

    End Function

#End Region

End Module
