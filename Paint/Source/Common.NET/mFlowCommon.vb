' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2007
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: mflowcommon
'
' Description: Operate screen stuff - with plcComm dependencies
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
'    03/17/07   gks     Onceover Cleanup
'    05/29/09   MSW     switch to VB 2008, Fluid maintenance PW4 development and debug
'    11/23/09   BTK     Changed mManFlowCommon_SendManualFlowToPLC and 
'                       mManFlowCommon_SendManualFlowToPLC for shaping air 2.		
'    12/03/09   MSW     WriteValveWordToPLC - Add a routine to write the whole word at once
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'    09/30/13   MSW     PLC DLL                                                       4.01.06.00
'    02/12/14   MSW     KTP Changes                                                   4.01.07.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports clsPLCComm = PLCComm.clsPLCComm

Friend Module mManFlowCommon
    Friend Const nPNT1 As Integer = 86
    Friend Const nSAFA_CAL_SUCCESS_Eq1 As Integer = 616
    Friend Const nSAFA_CAL_SUCCESS_Eq2 As Integer = 647
    Friend Const nSAFA_CAL_ABORT_Eq1 As Integer = 617
    Friend Const nSAFA_CAL_ABORT_Eq2 As Integer = 648
    Friend Const nSAFA_CAL_NON_INC_Eq1 As Integer = 618
    Friend Const nSAFA_CAL_NON_INC_Eq2 As Integer = 649
    Friend Const nSAFA_CAL_ZERO_PRES_Eq1 As Integer = 619
    Friend Const nSAFA_CAL_ZERO_PRES_Eq2 As Integer = 650
    Friend Const nDQ_CAL_SUCCESS_Eq1 As Integer = 851
    Friend Const nDQ_CAL_SUCCESS_Eq2 As Integer = 867
    Friend Const nDQ_CAL_ABORT_Eq1 As Integer = 852
    Friend Const nDQ_CAL_ABORT_Eq2 As Integer = 868
    Friend Const nDQ_CAL_NON_INC_Eq1 As Integer = 853
    Friend Const nDQ_CAL_NON_INC_Eq2 As Integer = 869
    Friend Const nDQ_CAL_ZERO_PRES_Eq1 As Integer = 854
    Friend Const nDQ_CAL_ZERO_PRES_Eq2 As Integer = 870
    Friend Const nAF_CAL_SUCCESS_Eq1 As Integer = 558
    Friend Const nAF_CAL_ABORT_Eq1 As Integer = 559
    Friend Const nAF_CAL_LOWRST_Eq1 As Integer = 560
    Friend Const nAF_CAL_HIRST_Eq1 As Integer = 561
    Friend Const nAF_CAL_HI_TO_Eq1 As Integer = 562
    Friend Const nAF_CAL_LO_TO_Eq1 As Integer = 563
    Friend Const nAF_CAL_0_DET_Eq1 As Integer = 564
    Friend Const nAF_CAL_NONINC_Eq1 As Integer = 565
    Friend Const nAF_CAL_TU_TO_Eq1 As Integer = 566
    Friend Const nAF_CAL_BADMID_Eq1 As Integer = 576
    Friend Const nIC_CAL_ABORT_Eq1 As Integer = 825
    Friend Const nIC_CAL_ABORT_Eq2 As Integer = 835
    Friend Const nIC_CAL_SUCCESS_Eq1 As Integer = 827
    Friend Const nIC_CAL_SUCCESS_Eq2 As Integer = 837
    Friend Const nIC_CAL_MSG_LOW_Eq1 As Integer = 889
    Friend Const nIC_CAL_MSG_LOW_Eq2 As Integer = 901
    Friend Const nIC_CAL_MSG_HIGH_Eq1 As Integer = 900
    Friend Const nIC_CAL_MSG_HIGH_Eq2 As Integer = 912

    Friend Const nIPC_CAL_VLV_NOT_DEF_Eq1 As Integer = 486
    Friend Const nIPC_CAL_VLV_NOT_DEF_Eq2 As Integer = 517
    Friend Const nIPC_CAL_ABORT_Eq1 As Integer = 767
    Friend Const nIPC_CAL_ABORT_Eq2 As Integer = 812
    Friend Const nIPC_CAL_NON_INC_Eq1 As Integer = 768
    Friend Const nIPC_CAL_NON_INC_Eq2 As Integer = 813
    Friend Const nIPC_CAL_0_PSI_Eq1 As Integer = 769
    Friend Const nIPC_CAL_0_PSI_Eq2 As Integer = 814
    Friend Const nIPC_CAL_SUCCESS_Eq1 As Integer = 558
    Friend Const nIPC_CAL_SUCCESS_Eq2 As Integer = 587




#Region " Routines "

    Friend Sub UpdateChangeLog(ByVal Item As Integer, ByVal NewValue As String, _
                                                ByVal OldValue As String, ByVal ParamName As String, _
                                                ByVal Device As String, ByVal Valve As String, _
                                                ByVal ZoneNumber As Integer, ByVal ZoneName As String)
        '********************************************************************************************
        'Description:  build up the change text
        '
        'Parameters: 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp As String = String.Empty

        If Item > 0 Then
            ' its a calibration Point
            sTmp = ParamName & " " & gpsRM.GetString("psPOINT") & " " & Item.ToString & " "

        Else
            'its an parameter
            sTmp = ParamName & " "
        End If

        sTmp = sTmp & " " & gcsRM.GetString("csCHANGED_FROM") & " " & OldValue & " " & _
                 gcsRM.GetString("csTO") & " " & NewValue

        AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
                    ZoneNumber, Device, Valve, sTmp, ZoneName)
    End Sub
    Friend Sub UpdateChangeLog(ByVal CalStatus As String, ByVal ParamName As String, _
                ByVal Device As String, ByVal ZoneNumber As Integer, ByVal ZoneName As String)
        '********************************************************************************************
        'Description:  build up the change text for calibration
        '
        'Parameters: 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp As String = gpsRM.GetString("psCAL_RESULT_TEXT") & " "

        sTmp = sTmp & CalStatus

        AddChangeRecordToCollection(frmMain.gsChangeLogArea, frmMain.LoggedOnUser, _
            ZoneNumber, Device, ParamName, sTmp, ZoneName)

    End Sub
    Friend Sub InitPlc(ByRef rPLC As clsPLCComm, ByVal oZone As clsZone, _
             ByVal sZonePrefix As String, ByRef sHotLinkData As String())
        '********************************************************************************************
        'Description: set up plc comm and set values to minimum
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTag As String
        Dim sData(4) As String


        Try
            If rPLC Is Nothing Then
                rPLC = New clsPLCComm
            End If

            With rPLC
                .ZoneName = oZone.Name
                sTag = sZonePrefix & "ManualFuncHotlink"
                .TagName = sTag
                sHotLinkData = .PLCData
            End With

            'let hotlink fire
            System.Windows.Forms.Application.DoEvents()

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Sub SendManualFlowToRobot(ByRef Robot As clsArm, ByRef sData As String(), ByRef bEngUnits As Boolean())
        '********************************************************************************************
        'Description: Send flow data to robot vars
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/23/09  BTK     Changed for shaping air 2.
        '********************************************************************************************
        Dim nParm As Integer
        Robot.ProgramName = "PAMANUAL"
        'send flow vars
        'Robots count from 1, like people, not c programmers
        For nParm = (CInt(eParamID.Flow) + 1) To (CInt(eParamID.Fan2) + 1)
            Robot.VariableName = "APP_TEST_DAT[" & nParm.ToString & "].ENG_UNIT_USD"
            'incoming data is 0 indexed
            Robot.VarValue = bEngUnits(nParm - 1).ToString
            Robot.VariableName = "APP_TEST_DAT[" & nParm.ToString & "].TEST_VALUE"
            Robot.VarValue = sData(nParm - 1)
        Next

    End Sub
    Friend Sub SendManualFlowToRobots(ByRef Controllers As clsControllers, ByRef sData As String(), ByRef bEngUnits As Boolean())
        For Each o As clsController In Controllers
            SendManualFlowToRobot(o.Arms(0), sData, bEngUnits)
        Next
    End Sub
    Friend Sub SendTPRToRobots(ByRef Controllers As clsControllers, ByVal TPR As String)
        '********************************************************************************************
        'Description: Send the selected default TPR value to all online robots.
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/02/10  MSW     Write to all the waterborn types
        '********************************************************************************************

        For Each oController As clsController In Controllers
            Dim oRobot As clsArm = oController.Arms(0)

            With oRobot
                If .IsOnLine AndAlso _
                    (.ColorChangeType = eColorChangeType.HONDA_WB Or _
                     .ColorChangeType = eColorChangeType.VERSABELL2_PLUS_WB Or _
                     .ColorChangeType = eColorChangeType.VERSABELL2_WB Or _
                     .ColorChangeType = eColorChangeType.ACCUSTAT Or _
                     .ColorChangeType = eColorChangeType.AQUABELL Or _
                     .ColorChangeType = eColorChangeType.VERSABELL3_WB Or _
                     .ColorChangeType = eColorChangeType.VERSABELL3_DUAL_WB) Then
                    .ProgramName = "pavraccu"
                    .VariableName = "tpr_default"
                    .VarValue = TPR
                End If
            End With

        Next 'oController

    End Sub
    Friend Sub RobotFlowDataHotLink(ByRef rPLC As clsPLCComm, ByVal oZone As clsZone, _
    ByVal nRobotNum As Integer, ByVal nOldRobot As Integer, ByRef sHotLinkData As String())
        '********************************************************************************************
        'Description: Setup New Hot Link to Robot Flow data CC time Ect.
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        '3/20/09  GEO.     Original code.

        Dim sTag As String

        Dim ZonePrefix As String = "Z" & oZone.ZoneNumber.ToString

        Try
            If rPLC Is Nothing Then
                rPLC = New clsPLCComm
                rPLC.ZoneName = oZone.Name
            End If

            With rPLC
                ' Remove Previous Robot PLC Link...
                If nOldRobot <> 0 Then
                    sTag = ZonePrefix & "FluidMaintHotLink_R" & nOldRobot.ToString("00")
                    rPLC.RemoveHotLink(sTag, oZone.Name)
                End If

                sTag = ZonePrefix & "FluidMaintHotLink_R" & nRobotNum.ToString("00")

                .TagName = sTag
                sHotLinkData = .PLCData
            End With

            'let hotlink fire
            System.Windows.Forms.Application.DoEvents()

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Sub SendManualFlowToPLC(ByRef rPLC As clsPLCComm, ByVal sData As String(), _
                                    ByVal sZonePrefix As String, ByVal bUseEngUnits() As Boolean)
        '********************************************************************************************
        'Description: 
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/23/09  BTK     Added shaping air 2.
        ' 03/30/11  MSW     disable eng units write to PLC.  PLC is isn't using it for now.
        '********************************************************************************************
        'set not engineering units
        Dim sTag As String = String.Empty
        Dim s(0) As String
        'sTag = sZonePrefix & "SelectEngUnits"
        'Dim nEngUnits As Integer = 0
        'If bUseEngUnits(eParamID.Flow) Then
        '    nEngUnits = 1
        'End If
        'If bUseEngUnits(eParamID.Atom) Then
        '    nEngUnits = nEngUnits + 2
        'End If
        'If bUseEngUnits(eParamID.Fan) Then
        '    nEngUnits = nEngUnits + 4
        'End If
        'If bUseEngUnits(eParamID.Estat) Then
        '    nEngUnits = nEngUnits + 8
        'End If
        'If bUseEngUnits(eParamID.Fan2) Then
        '    nEngUnits = nEngUnits + 16
        'End If
        's(0) = nEngUnits.ToString

        With rPLC
            '.TagName = sTag
            '.PLCData = s

            SendFlowParamsToPLC(rPLC, sData, sZonePrefix)

            sTag = sZonePrefix & "StartFlowTest"
            .TagName = sTag
            s(0) = "1"
            .PLCData = s
        End With
    End Sub
    Friend Sub SendCancelTestToPLC(ByRef rPLC As clsPLCComm, ByVal sZonePrefix As String)
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
        Dim sTag As String
        Dim s(0) As String
        sTag = sZonePrefix & "AbortFlowTest"

        With rPLC
            .TagName = sTag
            s(0) = "1"
            .PLCData = s
        End With
    End Sub
    Friend Sub SendFlowParamsToPLC(ByRef rPLC As clsPLCComm, ByVal sData As String(), _
      ByVal sZonePrefix As String)
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

        With rPLC

            .TagName = sZonePrefix & "ManualFlowTestParam"
            .PLCData = sData

        End With

    End Sub
    Friend Function GetCalibrationStatusString(ByVal CalCode As Integer) As String
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

        Select Case CalCode
            Case 0
                Return gpsRM.GetString("psNOT_CALIBRATED")
            Case 1
                Return gpsRM.GetString("psSUCCESSFUL")
            Case 2
                Return gpsRM.GetString("psSUCCESSFUL_NO_UPPER")
            Case 3
                Return gpsRM.GetString("psSUCCESSFUL_NO_LOWER")
            Case 16, 17 '16 Adapt out of tolerance bit, 17 Adapted out of tolerance
                Return gpsRM.GetString("psSUCCESSFUL_ADAPTED_OUT")
            Case 18
                Return gpsRM.GetString("psSUCCESSFUL_UPPER_LIMIT")
            Case 19
                Return gpsRM.GetString("psSUCCESSFUL_LOWER_LIMIT")
            Case 33
                Return gpsRM.GetString("psCALIBRATION_COPIED")
            Case 49
                Return gpsRM.GetString("psCALIBRATION_COPY_ERR")
            Case 255
                Return gpsRM.GetString("psCALIBRATION_ABORTED")
            Case Else
                Return gpsRM.GetString("psCALIBRATION_ERROR")
        End Select

    End Function
    Friend Function GetDQCalStatusString(ByVal CalCode As Integer) As String
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
        'Friend Const nPNT1 As Integer = 86
        'Friend Const nDQ_CAL_SUCCESS_Eq1 As Integer = 851
        'Friend Const nDQ_CAL_SUCCESS_Eq2 As Integer = 867
        'Friend Const nDQ_CAL_ABORT_Eq1 As Integer = 852
        'Friend Const nDQ_CAL_ABORT_Eq2 As Integer = 868
        'Friend Const nDQ_CAL_NON_INC_Eq1 As Integer = 853
        'Friend Const nDQ_CAL_NON_INC_Eq2 As Integer = 869
        'Friend Const nDQ_CAL_ZERO_PRES_Eq1 As Integer = 854
        'Friend Const nDQ_CAL_ZERO_PRES_Eq2 As Integer = 870
        Select Case CalCode
            Case nDQ_CAL_SUCCESS_Eq1, nDQ_CAL_SUCCESS_Eq1
                Return gpsRM.GetString("psSUCCESSFUL")
            Case nDQ_CAL_ABORT_Eq1, nDQ_CAL_ABORT_Eq2
                Return gpsRM.GetString("psNOT_CALIBRATED")
            Case nDQ_CAL_NON_INC_Eq1, nDQ_CAL_NON_INC_Eq2
                Return gpsRM.GetString("psNON_INCREASING") & " ( " & CalCode.ToString & ")"
            Case nDQ_CAL_ZERO_PRES_Eq1, nDQ_CAL_ZERO_PRES_Eq2
                Return gpsRM.GetString("psZERO_PRESSURE") & " ( " & CalCode.ToString & ")"
            Case Else
                Return gpsRM.GetString("psCALIBRATION_ERROR")
        End Select

    End Function
    Friend Function GetTPR(ByRef Robot As clsArm) As String
        '********************************************************************************************
        'Description:  Get Default TPR stored on Robot
        '
        'Parameters: Robot as clsArm
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTPR As String = String.Empty

        With Robot
            .ProgramName = "pavraccu"
            .VariableName = "tpr_default"
            sTPR = .VarValue
        End With

        Return sTPR

    End Function
    Friend Function GetTPRUpperLimit(ByRef Robot As clsArm) As Single
        '********************************************************************************************
        'Description:  Get Default TPR stored on Robot
        '
        'Parameters: Robot as clsArm
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim fUpperLim As Single = 0

        With Robot
            .ProgramName = "pavric"
            .VariableName = "ticsetup.imaxcanvol"
            fUpperLim = CType(.VarValue, Single)
        End With

        Return fUpperLim
    End Function
    Friend Function GetViewAllData(ByVal ZoneName As String, ByVal RobotName As String, _
                                ByVal ParamName As String, ByVal SubParamName As String, _
                                ByVal DatabasePath As String, ByRef rArm As clsArm) As DataSet
        '********************************************************************************************
        'Description:  Load data stored on Robot
        '
        'Parameters: none
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oZones As New clsZones(DatabasePath)
        oZones.CurrentZone = ZoneName

        Dim oControllers As clsControllers = New clsControllers(oZones, False)
        Dim oArms As clsArms = LoadArmCollection(oControllers)
        Dim oRobot As clsArm = oArms.Item(RobotName)

        Dim ds As New DataSet

        If oRobot.IsOnLine Then



            Return ds
        Else
            MessageBox.Show(gcsRM.GetString("csCOULD_NOT_CONNECT"), _
                    oRobot.Name, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return Nothing
        End If

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

            Return True
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try
    End Function
    Friend Sub RunColorChangeCycle(ByVal CycleNumber As Integer, ByVal oPLC As clsPLCComm, _
                                                                ByVal ZonePrefix As String)
        '********************************************************************************************
        'Description:  run forest run
        '
        'Parameters: 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim sTmp(0) As String
        sTmp(0) = CycleNumber.ToString

        With oPLC
            .TagName = ZonePrefix & "CCCycleSelected"
            .PLCData = sTmp
        End With

    End Sub
    Friend Sub WriteValveChangeToPLC(ByRef rPLC As clsPLCComm, ByVal sZonePrefix As String, ByVal nValve As Integer, ByVal bShared As Boolean)
        '********************************************************************************************
        'Description: valve in one of the cartoon boxes clicked
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        ' 4/10/09  GEO      Modified to use separate PLC Integer words to make compatable with all PLC not just RSLogix.

        Dim sPLCDataToSend() As String = {"0"}
        Dim bSharedWordChanged As Boolean = False
        Dim bWord1Changed As Boolean = False
        Dim bWord2Changed As Boolean = False
        Select Case nValve
            Case 0 To 14   ' Word 1 
                sPLCDataToSend(0) = CStr(gnBitVal(nValve))
                bWord1Changed = True        ' set Flag to direct to plc word
            Case 15       ' Word 1 CC Valves 16 bit 
                sPLCDataToSend(0) = CStr(-32768)
                bWord1Changed = True         ' set Flag to direct to plc word
            Case 16 To 30 ' Word 2
                sPLCDataToSend(0) = CStr(gnBitVal(nValve - 16))
                bWord2Changed = True         ' set Flag to direct to plc word
            Case 31       ' Word 2 16 Bit
                sPLCDataToSend(0) = CStr(-32768)
                bWord2Changed = True         ' set Flag to direct to plc word
        End Select

        ''''''''''''''''''''''''''''
        ' NEW PLC TAGS '''''''''''''''''
        'Z1CCSharedValveReqWord
        'Z1CCValveReqWord1
        'Z1CCValveReqWord2
        ''''''''''''''''''''''''''''
        ''''''''''''''''''''''''''''
        Try
            With rPLC
                If bShared Then
                    .TagName = sZonePrefix & "CCSharedValveReqWord"
                    .PLCData = sPLCDataToSend

                Else

                    If bWord1Changed = True Then
                        .TagName = sZonePrefix & "CCValveReqWord1"
                        .PLCData = sPLCDataToSend
                    End If

                    If bWord2Changed = True Then
                        .TagName = sZonePrefix & "CCValveReqWord2"
                        .PLCData = sPLCDataToSend
                    End If
                End If
            End With


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Sub WriteValveWordToPLC(ByRef rPLC As clsPLCComm, ByVal sZonePrefix As String, ByVal nValves As Integer, ByVal bShared As Boolean, Optional ByVal nWord As Integer = 1)
        '********************************************************************************************
        'Description: valve in one of the cartoon boxes clicked
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 4/10/09  GEO      Modified to use separate PLC Integer words to make compatable with all PLC not just RSLogix.
        ' 12/01/09  MSW     WriteValveWordToPLC - Add a routine to write the whole word at once
        '********************************************************************************************

        Dim sPLCDataToSend() As String = {"0"}

        If nValves > 32767 Then
            sPLCDataToSend(0) = CStr(nValves - 32768)
        Else
            sPLCDataToSend(0) = CStr(nValves)
        End If
        ''''''''''''''''''''''''''''
        ' NEW PLC TAGS '''''''''''''''''
        'Z1CCSharedValveReqWord
        'Z1CCValveReqWord1
        'Z1CCValveReqWord2
        ''''''''''''''''''''''''''''
        ''''''''''''''''''''''''''''
        Try
            With rPLC
                If bShared Then
                    .TagName = sZonePrefix & "CCSharedValveReqWord"
                    .PLCData = sPLCDataToSend

                Else

                    .TagName = sZonePrefix & "CCValveReqWord" & nWord.ToString
                    .PLCData = sPLCDataToSend

                End If
            End With


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
#End Region
#Region " Properties "

    Friend Property CurrentManualColor(ByVal oPLC As clsPLCComm, ByVal sZonePrefix As String) As String
        '********************************************************************************************
        'Description:  Manual color to/from plc
        '
        'Parameters: none
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get

            Try
                Dim sTmp() As String

                With oPLC
                    .TagName = sZonePrefix & "CurrentManualColorWord"
                    sTmp = .PLCData
                End With

                If sTmp Is Nothing Then
                    Return "0"
                Else
                    Return sTmp(0)
                End If

            Catch ex As Exception
                Trace.WriteLine(ex.Message)
                Return "0"
            End Try

        End Get
        Set(ByVal value As String)
            Try

                Dim sTmp(0) As String
                sTmp(0) = value

                If oPLC Is Nothing Then
                    MessageBox.Show(gpsRM.GetString("psFAIL_SEND_COLOR"), "", _
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning)

                    Exit Property
                End If

                With oPLC
                    .TagName = sZonePrefix & "SetManualColorWord"
                    .PLCData = sTmp
                End With


            Catch ex As Exception
                Trace.WriteLine(ex.Message)
            End Try

        End Set
    End Property

#End Region

End Module 'mManFlowCommon
