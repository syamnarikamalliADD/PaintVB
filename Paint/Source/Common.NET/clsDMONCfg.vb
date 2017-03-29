' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2009
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: clsDMONCfg (clsDMONCfg.vb)
'
' Description:
' This class module contains the code to store robot DMON Data.
'
' Dependencies:  
'
' Language: Microsoft Visual Basic 2008 .NET
'
' Author: Matt White
' FANUC Robotics America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'  By           Date                                                    Version
'  MSW          10/17/11  First draft                                   4.01.01.00
'  MSW          12/22/11  Add support for version < 6.3 cc variables    4.01.01.01
'  MSW          02/16/12  Error Handling updates                        4.01.01.02
'  MSW          04/24/12  SaveToRobot - Handle openers without throwing an error.  4.01.03.00
'  BTK          9/25/2012 LoadFromRobot we still need to dimension      4.01.03.01
'                         mbEnableCCDiag when we have an opener only
'                         controller or the screen doesn't load correctly.
'  MSW          10/10/12  SaveToRobot - Indexing change to mbEnableCCDiag 4.01.03.02
'                         so it changes back to black after a save 
'  MSW          03/06/13  sGetIOTYPEString - Fix a couple typos that    4.01.04.00
'                         prevented returning DO or GO      
'  MSW          04/16/13  clsDMONCfg - LoadFromRobot -                  4.01.05.00
'                         Adjust the error handling a bit so we don't   
'                         fill the event log with items the robot
'                         leaves uninit
'  MSW          12/10/13  Modify to support the parts that work without 4.01.06.00
'                         painttool
'  MSW          01/06/14  Deal with changes in R30iB sysvars            4.01.06.01
'*************************************************************************

Option Compare Text
Option Explicit On
Option Strict On
Imports FRRobot
Imports FRRobotNeighborhood

Public Class clsDMONCfg

    Enum eTYPE
        None = -1
        Real = 17
        Int = 16
        IO = 33
        Register = 38
        Axis = 216
    End Enum

    Enum eIO_TYPE
        None = -1
        IO_DI = 1
        IO_DO = 2
        IO_AI = 3
        IO_AO = 4
        IO_RI = 8
        IO_RO = 9
        IO_WI = 16
        IO_WO = 17
        IO_GI = 18
        IO_GO = 19
    End Enum

    Friend Structure tDmonItem
        Dim PRG_NAME As clsTextValue
        Dim VAR_NAME As clsTextValue
        Dim DESC As clsTextValue
        Dim UNITS As clsTextValue
        Dim TYPE As clsIntValue
        Dim IO_TYPE As clsIntValue
        Dim PORT_NUM As clsIntValue
        Dim SLOPE As clsSngValue
        Dim INTERCEPT As clsSngValue
        Dim SQUARE As clsSngValue
        Dim GROUP_NUM As clsIntValue
        Dim AXIS_NUM As clsIntValue
    End Structure

    Friend Structure tDmonSchedule
        Dim COMMENT As clsTextValue
        Dim FILE_NAME As clsTextValue
        Dim DEVICE_NAME As clsTextValue
        Dim DEVICE As clsIntValue
        Dim NUM_ITEMS As clsIntValue

        Dim FILE_INDEX As clsIntValue
        Dim FILE_SIZE As clsIntValue
        Dim SAMP_REQ As clsSngValue
        Dim SAMP_RATE As Single
        Dim MNTR_REQ As clsSngValue
        Dim MNTR_RATE As Single
        Dim REC_REQ As clsSngValue
        Dim REC_PER_SEC As Single
        Dim REC_MODE As clsIntValue

        Dim START_ITEM As clsIntValue
        Dim START_ENBL As clsBoolValue
        Dim START_COND As clsIntValue
        Dim START_VALUE As clsSngValue
        Dim STOP_ITEM As clsIntValue
        Dim STOP_ENBL As clsBoolValue
        Dim STOP_COND As clsIntValue
        Dim STOP_VALUE As clsSngValue
        Dim ITEM() As clsIntValue
    End Structure
    'PLC data
    Private mbPLCEnable As New clsBoolValue

    'Editable robot settings
    Private mbAutoSample As New clsBoolValue
    Private mbAsyncEnab As New clsBoolValue
    Private mnSchedule As New clsIntValue
    Private mbEnableCCDiag() As clsBoolValue
    Private mtSchedules() As tDmonSchedule
    Private mtItems() As tDmonItem

    'Read only robot settings
    Private msngRobotVersion As Single
    Private mbUsePT As Boolean
    Private mbUseCC As Boolean
    Private mnNumSchedules As Integer
    Private mnNumItems As Integer
    Private mbDisplayItemSquare As Boolean = False
    Private mbDisplayItemAXIS As Boolean = False
    Private mnMaxItemsPerSchedule As Integer = 0
    Private mbLoaded As Boolean = False


    Friend ReadOnly Property Changed() As Boolean
        '********************************************************************************************
        'This property stores the PLC diag enable bit.
        '
        'Parameters: none
        'Returns: The PLC diag enable bit
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Get
            If mbAutoSample.Changed OrElse mbAsyncEnab.Changed OrElse mnSchedule.Changed OrElse EnableCCDiagChanged Then
                Return True
            End If
            For nSchedule As Integer = 0 To mnNumSchedules - 1
                If mtSchedules(nSchedule).COMMENT.Changed OrElse _
                mtSchedules(nSchedule).DEVICE.Changed OrElse _
                mtSchedules(nSchedule).DEVICE_NAME.Changed OrElse _
                mtSchedules(nSchedule).FILE_INDEX.Changed OrElse _
                mtSchedules(nSchedule).FILE_NAME.Changed OrElse _
                mtSchedules(nSchedule).FILE_SIZE.Changed OrElse _
                mtSchedules(nSchedule).SAMP_REQ.Changed OrElse _
                mtSchedules(nSchedule).MNTR_REQ.Changed OrElse _
                mtSchedules(nSchedule).REC_REQ.Changed OrElse _
                mtSchedules(nSchedule).NUM_ITEMS.Changed OrElse _
                mtSchedules(nSchedule).REC_MODE.Changed OrElse _
                mtSchedules(nSchedule).START_COND.Changed OrElse _
                mtSchedules(nSchedule).START_ENBL.Changed OrElse _
                mtSchedules(nSchedule).START_ITEM.Changed OrElse _
                mtSchedules(nSchedule).START_VALUE.Changed OrElse _
                mtSchedules(nSchedule).STOP_COND.Changed OrElse _
                mtSchedules(nSchedule).STOP_ENBL.Changed OrElse _
                mtSchedules(nSchedule).STOP_ITEM.Changed OrElse _
                mtSchedules(nSchedule).STOP_VALUE.Changed Then
                    Return True
                End If
            Next
            If mtItems IsNot Nothing Then
                For nItem As Integer = 0 To mnNumItems - 1
                    If mtItems(nItem).DESC.Changed OrElse _
                    mtItems(nItem).TYPE.Changed OrElse _
                    mtItems(nItem).IO_TYPE.Changed OrElse _
                    mtItems(nItem).PORT_NUM.Changed OrElse _
                    mtItems(nItem).PRG_NAME.Changed OrElse _
                    mtItems(nItem).VAR_NAME.Changed OrElse _
                    mtItems(nItem).AXIS_NUM.Changed OrElse _
                    mtItems(nItem).GROUP_NUM.Changed OrElse _
                    mtItems(nItem).SLOPE.Changed OrElse _
                    mtItems(nItem).SQUARE.Changed OrElse _
                    mtItems(nItem).INTERCEPT.Changed OrElse _
                    mtItems(nItem).UNITS.Changed Then
                        Return True
                    End If
                Next
            End If
            Return False
        End Get
    End Property
    Friend Property PLCEnable() As clsBoolValue
        '********************************************************************************************
        'This property stores the PLC diag enable bit.
        '
        'Parameters: none
        'Returns: The PLC diag enable bit
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mbPLCEnable
        End Get

        Set(ByVal value As clsBoolValue)
            mbPLCEnable = value
        End Set

    End Property
    Friend Property AutoSample() As clsBoolValue
        '********************************************************************************************
        'This property stores the robot autosample bit.
        '
        'Parameters: none
        'Returns: The robot autosample bit
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mbAutoSample
        End Get

        Set(ByVal value As clsBoolValue)
            mbAutoSample = value
        End Set

    End Property
    Friend Property AsyncEnab() As clsBoolValue
        '********************************************************************************************
        'This property stores the asyncronous sample enable.
        '
        'Parameters: none
        'Returns: The syncronous sample enable bit
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mbAsyncEnab
        End Get

        Set(ByVal value As clsBoolValue)
            mbAsyncEnab = value
        End Set

    End Property
    Friend Property Schedule() As clsIntValue
        '********************************************************************************************
        'This property stores the selected schedule number.
        '
        'Parameters: none
        'Returns: The selected schedule number
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mnSchedule
        End Get

        Set(ByVal value As clsIntValue)
            mnSchedule = value
        End Set

    End Property
    Friend Property EnableCCDiag(ByVal index As Integer) As clsBoolValue
        '********************************************************************************************
        'This property stores the selected schedule number.
        '
        'Parameters: none
        'Returns: The selected schedule number
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            If index = 0 And (mbEnableCCDiag(0).Changed = False) Then
                'Check status of changed bit
                Dim bChanged As Boolean = False
                Dim bEnabled As Boolean = False
                For nIndex2 As Integer = 1 To mbEnableCCDiag.GetUpperBound(0)
                    bChanged = bChanged Or mbEnableCCDiag(nIndex2).Changed
                    bEnabled = bEnabled Or mbEnableCCDiag(nIndex2).Value
                Next
                mbEnableCCDiag(0).Value = bEnabled
            End If
            Return mbEnableCCDiag(index)
        End Get

        Set(ByVal value As clsBoolValue)
            mbEnableCCDiag(index) = value

        End Set

    End Property
    Friend Property EnableCCDiag() As Boolean
        '********************************************************************************************
        'This property true if any eq was is to true for cc diag
        '
        'Parameters: none
        'Returns:  true if any eq was is to true for cc diag
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            'Check status of changed bit
            Dim bEnabled As Boolean = False
            For nIndex2 As Integer = 1 To mbEnableCCDiag.GetUpperBound(0)
                If mbEnableCCDiag(nIndex2).Value Then
                    Return True
                End If
            Next
            Return False
        End Get
        Set(ByVal value As Boolean)
            'Check status of changed bit
            Dim nEnabEq As Integer = 0
            If value And mnSchedule.Value > 0 Then
                nEnabEq = 1 + ((mnSchedule.Value - 1) \ 5)
            End If
            mbEnableCCDiag(0).Value = value
            For nIndex2 As Integer = 1 To mbEnableCCDiag.GetUpperBound(0)
                mbEnableCCDiag(nIndex2).Value = (nEnabEq = nIndex2)
            Next
        End Set
    End Property
    Friend ReadOnly Property EnableCCDiagOldValue() As Boolean
        '********************************************************************************************
        'This property true if any eq was set to true for cc diag
        '
        'Parameters: none
        'Returns: true if any eq was set to true for cc diag
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            'Check status of changed bit
            Dim bEnabled As Boolean = False
            For nIndex2 As Integer = 1 To mbEnableCCDiag.GetUpperBound(0)
                If mbEnableCCDiag(nIndex2).OldValue Then
                    Return True
                End If
            Next
            Return False
        End Get

    End Property
    Friend ReadOnly Property EnableCCDiagChanged() As Boolean
        '********************************************************************************************
        'This property returns  true if any eq changed cc diag
        '
        'Parameters: none
        'Returns:   true if any eq changed cc diag
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            'Check status of changed bit
            Dim bEnabled As Boolean = False
            For nIndex2 As Integer = 1 To mbEnableCCDiag.GetUpperBound(0)
                If mbEnableCCDiag(nIndex2).Changed Then
                    Return True
                End If
            Next
            Return False
        End Get

    End Property
    Friend ReadOnly Property Loaded() As Boolean
        '********************************************************************************************
        'This property keep track of a successful load from the robot.
        '
        'Parameters: none
        'Returns: true if data loaded
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mbLoaded
        End Get

    End Property
    Friend ReadOnly Property RobotVersion() As Single
        '********************************************************************************************
        'This property stores the PaintTool version number.
        '
        'Parameters: none
        'Returns: The PaintTool version number
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return msngRobotVersion
        End Get

    End Property
    Friend ReadOnly Property NumSchedules() As Integer
        '********************************************************************************************
        'This property stores the number of DMON schedules available on this controller.
        '
        'Parameters: none
        'Returns: The the number of DMON schedules available on this controller.
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mnNumSchedules
        End Get

    End Property
    Friend ReadOnly Property MaxItemsPerSchedule() As Integer
        '********************************************************************************************
        'This property stores the number of items available in a DMON schedule.
        '
        'Parameters: none
        'Returns: The the number of the number of items available in a DMON schedule.
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Get
            Return mnMaxItemsPerSchedule
        End Get
    End Property
    Friend ReadOnly Property NumItems() As Integer
        '********************************************************************************************
        'This property stores the number of DMON items available on this controller.
        '
        'Parameters: none
        'Returns: The the number of DMON items available on this controller.
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mnNumItems
        End Get

    End Property

    Friend ReadOnly Property UseCC() As Boolean
        '********************************************************************************************
        'This property stores the CC option enable.
        '
        'Parameters: none
        'Returns: The CC option enable
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mbUseCC
        End Get

    End Property
    Friend ReadOnly Property UsePT() As Boolean
        '********************************************************************************************
        'This property Shows if we found PT variables (false for sealer
        '
        'Parameters: none
        'Returns: true for PT robot
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mbUsePT
        End Get

    End Property
    Friend ReadOnly Property DisplayItemSquare() As Boolean
        '********************************************************************************************
        'This property Shows if the SQUARE variable exists in the item setup
        '
        'Parameters: none
        'Returns: true for newer versions
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mbDisplayItemSquare
        End Get

    End Property
    Friend ReadOnly Property DisplayItemAxis() As Boolean
        '********************************************************************************************
        'This property Shows if the AXIS variables exist in the item setup
        '
        'Parameters: none
        'Returns: true for newer versions
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mbDisplayItemAXIS
        End Get

    End Property
    Friend Property Items(ByVal index As Integer) As tDmonItem
        '********************************************************************************************
        'This property stores the dmon Item.
        '
        'Parameters: none
        'Returns: The selected Item
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mtItems(index)
        End Get

        Set(ByVal value As tDmonItem)
            mtItems(index) = value
        End Set

    End Property
    Friend Property Schedules(ByVal index As Integer) As tDmonSchedule
        '********************************************************************************************
        'This property stores the dmon schedule.
        '
        'Parameters: none
        'Returns: The selected schedule
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mtSchedules(index)
        End Get

        Set(ByVal value As tDmonSchedule)
            mtSchedules(index) = value
        End Set

    End Property
    Friend Sub LoadFileDeviceBox(ByVal rCbo As ComboBox)
        '********************************************************************************************
        'Description:  Load a file device cbo 
        ' simple for now, but it's here to support version changes
        '
        'Parameters: cbo
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        rCbo.Items.Clear()
        rCbo.Items.Add(gpsRM.GetString("psFILEDEVICE1"))
        rCbo.Items.Add(gpsRM.GetString("psFILEDEVICE2"))
        rCbo.Items.Add(gpsRM.GetString("psFILEDEVICE3"))
        rCbo.Items.Add(gpsRM.GetString("psFILEDEVICE4"))
        rCbo.Items.Add(gpsRM.GetString("psFILEDEVICE5"))
        rCbo.Items.Add(gpsRM.GetString("psFILEDEVICE6"))
        Dim nTag(5) As Integer
        nTag(0) = 1
        nTag(1) = 2
        nTag(2) = 3
        nTag(3) = 4
        nTag(4) = 5
        nTag(5) = 6
        rCbo.Tag = nTag
    End Sub
    Friend Sub LoadStartStopConditionBox(ByVal rCbo As ComboBox)
        '********************************************************************************************
        'Description:  Load a start/stop condition cbo 
        ' simple for now, but it's here to support version changes
        '
        'Parameters: cbo
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        rCbo.Items.Clear()
        rCbo.Items.Add(gpsRM.GetString("psSTARTSTOPCOND1"))
        rCbo.Items.Add(gpsRM.GetString("psSTARTSTOPCOND2"))
        rCbo.Items.Add(gpsRM.GetString("psSTARTSTOPCOND3"))
        rCbo.Items.Add(gpsRM.GetString("psSTARTSTOPCOND4"))
        rCbo.Items.Add(gpsRM.GetString("psSTARTSTOPCOND5"))
        rCbo.Items.Add(gpsRM.GetString("psSTARTSTOPCOND6"))
        Dim nTag(5) As Integer
        nTag(0) = 1
        nTag(1) = 2
        nTag(2) = 3
        nTag(3) = 4
        nTag(4) = 5
        nTag(5) = 6
        rCbo.Tag = nTag

    End Sub

    Friend Sub LoadIO_TYPEBox(ByVal rCbo As ComboBox)
        '********************************************************************************************
        'Description:  Load IO_TYPE cbo 
        '
        'Parameters: cbo
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        rCbo.Items.Clear()
        rCbo.Items.Add(gpsRM.GetString("psDI"))
        rCbo.Items.Add(gpsRM.GetString("psDO"))
        rCbo.Items.Add(gpsRM.GetString("psAI"))
        rCbo.Items.Add(gpsRM.GetString("psAO"))
        rCbo.Items.Add(gpsRM.GetString("psRI"))
        rCbo.Items.Add(gpsRM.GetString("psRO"))
        'rCbo.Items.Add(gpsRM.GetString("psWI"))
        'rCbo.Items.Add(gpsRM.GetString("psWO"))
        rCbo.Items.Add(gpsRM.GetString("psGI"))
        rCbo.Items.Add(gpsRM.GetString("psGO"))
        Dim nTag(7) As eIO_TYPE
        nTag(0) = eIO_TYPE.IO_DI
        nTag(1) = eIO_TYPE.IO_DO
        nTag(2) = eIO_TYPE.IO_AI
        nTag(3) = eIO_TYPE.IO_AO
        nTag(4) = eIO_TYPE.IO_RI
        nTag(5) = eIO_TYPE.IO_RO
        'nTag(6) = eIO_TYPE.IO_WI
        'nTag(7) = eIO_TYPE.IO_WO
        'nTag(8) = eIO_TYPE.IO_GI
        'nTag(9) = eIO_TYPE.IO_GO
        nTag(6) = eIO_TYPE.IO_GI
        nTag(7) = eIO_TYPE.IO_GO
        rCbo.Tag = nTag
    End Sub

    Friend Sub LoadTYPEBox(ByVal rCbo As ComboBox)
        '********************************************************************************************
        'Description:  Load TYPE cbo 
        '
        'Parameters: cbo
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        rCbo.Items.Clear()
        rCbo.Items.Add(gpsRM.GetString("psREAL"))
        rCbo.Items.Add(gpsRM.GetString("psINTEGER"))
        rCbo.Items.Add(gpsRM.GetString("psIO"))
        rCbo.Items.Add(gpsRM.GetString("psREGISTER"))
        Dim nTag(3) As eTYPE
        nTag(0) = eTYPE.Real
        nTag(1) = eTYPE.Int
        nTag(2) = eTYPE.IO
        nTag(3) = eTYPE.Register

        If msngRobotVersion > 7.4 Then 'This is a guess, but the vars exist in the old stuff
            rCbo.Items.Add(gpsRM.GetString("psAXIS"))
            ReDim Preserve nTag(4)
            nTag(4) = eTYPE.Axis
        End If
        rCbo.Tag = nTag
    End Sub
    Friend Function sGetTYPEString(ByVal oType As eTYPE) As String
        '********************************************************************************************
        'Description: get description of type value
        '
        'Parameters: type

        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Select Case oType
            Case eTYPE.Axis
                Return (gpsRM.GetString("psAXIS"))
            Case eTYPE.Int
                Return (gpsRM.GetString("psINTEGER"))
            Case eTYPE.IO
                Return (gpsRM.GetString("psIO"))
            Case eTYPE.Real
                Return (gpsRM.GetString("psREAL"))
            Case eTYPE.Register
                Return (gpsRM.GetString("psREGISTER"))
            Case Else
                Return String.Empty
        End Select
    End Function
    Friend Function sGetIOTYPEString(ByVal oIOType As eIO_TYPE) As String
        '********************************************************************************************
        'Description: get description of IO type value
        '
        'Parameters: IO type
        'Returns:    none
        '
        'Modification history:
        '
        '  By           Date      Reason
        '  MSW          03/06/13  sGetIOTYPEString - Fix a couple typos that    4.01.04.00
        '                         prevented returning DO or GO      
        '********************************************************************************************
        Select Case oIOType
            Case eIO_TYPE.IO_AI
                Return (gpsRM.GetString("psAI"))
            Case eIO_TYPE.IO_AO
                Return (gpsRM.GetString("psAO"))
            Case eIO_TYPE.IO_DI
                Return (gpsRM.GetString("psDI"))
            Case eIO_TYPE.IO_DO
                Return (gpsRM.GetString("psDO"))
            Case eIO_TYPE.IO_RI
                Return (gpsRM.GetString("psRI"))
            Case eIO_TYPE.IO_RO
                Return (gpsRM.GetString("psRO"))
            Case eIO_TYPE.IO_WI
                Return (gpsRM.GetString("psWI"))
            Case eIO_TYPE.IO_WO
                Return (gpsRM.GetString("psWO"))
            Case eIO_TYPE.IO_GI
                Return (gpsRM.GetString("psGI"))
            Case eIO_TYPE.IO_GO
                Return (gpsRM.GetString("psGO"))
            Case Else
                Return String.Empty
        End Select
    End Function

    Friend Sub LoadScheduleBox(ByVal rCbo As ComboBox)
        '********************************************************************************************
        'Description:  Load a schedule cbo
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        rCbo.Items.Clear()
        For nSchedule As Integer = 0 To mnNumSchedules - 1
            rCbo.Items.Add((nSchedule + 1).ToString & " - " & mtSchedules(nSchedule).COMMENT.Text)
        Next
    End Sub
    Friend Sub LoadItemBox(ByVal rCbo As ComboBox, Optional ByVal bAddNone As Boolean = True)
        '********************************************************************************************
        'Description:  Load an item cbo
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        rCbo.Items.Clear()
        Dim nTag() As Integer

        Dim nTagOffset As Integer = 0
        If bAddNone Then
            ReDim nTag(mnNumItems)
            rCbo.Items.Add(gcsRM.GetString("csNONE"))
            nTag(0) = 0
            nTagOffset = 1
        Else
            ReDim nTag(mnNumItems - 1)
            nTagOffset = 0
        End If
        For nItem As Integer = 0 To mnNumItems - 1
            rCbo.Items.Add((nItem + 1).ToString & " - " & mtItems(nItem).DESC.Text)
            nTag(nTagOffset + nItem) = nItem + 1
        Next
        rCbo.Tag = nTag
    End Sub
    Friend Function LoadFromRobot(ByRef oController As clsController, ByVal bDetails As Boolean) As Boolean
        '********************************************************************************************
        'Load data from robot controller
        '
        'Parameters: none
        'Returns: true if successful
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         12/22/11      Add support for version < 6.3 cc variables
        ' BTK         9/25/2012     Still need to dimension mbEnableCCDiag when we have an opener only controller
        '                           or the screen doesn't load correctly.
        ' MSW         04/16/13      Adjust the error handling a bit so we don't fill the event log 
        '                           with items the robot leaves uninit
        ' MSW         12/10/13      Modify to support the parts that work without painttool
        '********************************************************************************************
        Try
            Dim sVarName As String
            Dim sProgName As String
            msngRobotVersion = oController.Version
            Dim oFRCProg As FRRobot.FRCProgram = Nothing
            Dim oFRCVars As FRRobot.FRCVars = Nothing
            Dim oFRCItemVars As FRRobot.FRCVars = Nothing
            Dim oFRCArray As FRRobot.FRCVars = Nothing
            Dim oFRCStruct As FRRobot.FRCVars = Nothing
            Dim oFRCVar As FRRobot.FRCVar = Nothing

            Dim sApp As String = oController.Application
            If sApp.ToLower.Contains("paint") Then
                'PaintTool interface
                'Due to supporting different configs, failure when reading some variables will assume they aren't supposed to be there.  
                '  Only missing core DMON variables will cause it to return false.  All failures get logged to event logger, though
                Dim oFRCPrograms As FRRobot.FRCPrograms = oController.Robot.Programs
                Try
                    'App selection, CC Option
                    If msngRobotVersion >= 7.3 Then
                        sVarName = "ByOptn[1].ByOp[9]"
                        sProgName = "PAVROPTN"
                    ElseIf msngRobotVersion >= 6.3 Then
                        sVarName = "ByOp[9]"
                        sProgName = "PAVROPTN"
                    Else
                        sVarName = "rob_cc_use"
                        sProgName = "PAVRSHIO"
                    End If
                    oFRCProg = DirectCast(oFRCPrograms(sProgName), FRRobot.FRCProgram)
                    If oFRCProg IsNot Nothing Then
                        oFRCVars = oFRCProg.Variables
                        mbUsePT = True
                        oFRCVar = DirectCast(oFRCVars(sVarName), FRRobot.FRCVar)
                        Dim bCC As Boolean = False
                        If msngRobotVersion >= 6.3 Then
                            bCC = (CType(oFRCVar.Value, Integer) = 1)
                        Else
                            bCC = CType(oFRCVar.Value, Boolean)
                        End If
                        If bCC Then
                            Try
                                ReDim mbEnableCCDiag(oController.Arms.Count)
                                mbEnableCCDiag(0) = New clsBoolValue
                                mbEnableCCDiag(0).Value = False
                                mbEnableCCDiag(0).Update()
                                If msngRobotVersion >= 6.3 Then
                                    sProgName = "PAVRCCEX"
                                    oFRCProg = DirectCast(oFRCPrograms(sProgName), FRRobot.FRCProgram)
                                    oFRCVars = oFRCProg.Variables
                                    oFRCArray = DirectCast(oFRCVars("tCCEqCmos"), FRRobot.FRCVars)
                                Else
                                    sProgName = "PACCRUNC"
                                    oFRCProg = DirectCast(oFRCPrograms(sProgName), FRRobot.FRCProgram)
                                    oFRCVars = oFRCProg.Variables
                                    oFRCVar = DirectCast(oFRCVars("bCCDiag"), FRRobot.FRCVar)
                                    'May be uninit in the old versions
                                    If oFRCVar.IsInitialized = False Then
                                        oFRCVar.Value = False
                                    End If
                                End If
                                mbUseCC = False 'Disable unless at least one arm is enabled
                                For nArm As Integer = 1 To oController.Arms.Count
                                    mbEnableCCDiag(nArm) = New clsBoolValue
                                    If oController.Arms.Item(nArm - 1).IsOpener Then
                                        mbEnableCCDiag(nArm).Value = False
                                        mbEnableCCDiag(nArm).Update()
                                    Else
                                        Try
                                            If msngRobotVersion >= 6.3 Then
                                                oFRCStruct = DirectCast(oFRCArray(nArm.ToString), FRRobot.FRCVars)
                                                sVarName = "tCCEqCmos[" & nArm.ToString & "].bCCDiag"
                                                oFRCVar = DirectCast(oFRCVars(sVarName), FRRobot.FRCVar)
                                            End If
                                            If (CType(oFRCVar.Value, Boolean)) Then
                                                mbEnableCCDiag(nArm).Value = True
                                                mbEnableCCDiag(nArm).Update()
                                                mbEnableCCDiag(0).Value = True
                                                mbEnableCCDiag(0).Update()
                                            Else
                                                mbEnableCCDiag(nArm).Value = False
                                                mbEnableCCDiag(nArm).Update()
                                            End If
                                            mbUseCC = True
                                        Catch ex As Exception
                                            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot(tCCEqCmos[" & nArm.ToString & "].bCCDiag)", _
                                            "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                        End Try
                                    End If
                                Next
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot(bCCDiag)", _
                                "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                mbUseCC = False
                            End Try
                        Else
                            mbUseCC = False
                            'BTK 9/25/2012 Still need to dimension mbEnableCCDiag when we have an opener only controller
                            'or the screen doesn't load correctly.
                            ReDim mbEnableCCDiag(oController.Arms.Count)
                            For nArm As Integer = 0 To oController.Arms.Count
                                mbEnableCCDiag(nArm) = New clsBoolValue
                                mbEnableCCDiag(nArm).Value = False
                                mbEnableCCDiag(nArm).Update()
                            Next
                        End If
                        mbUsePT = True
                    Else
                        mbUseCC = False
                        mbUsePT = False
                    End If
                Catch ex As Exception
                    mbUseCC = False
                    mbUsePT = False
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot(App selection)", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
                'PT Diag vars
                If mbUsePT Then
                    Try
                        sProgName = "PAVRPADG"
                        oFRCProg = DirectCast(oFRCPrograms(sProgName), FRRobot.FRCProgram)
                        oFRCVars = oFRCProg.Variables
                        Try
                            sVarName = "dg_def_sched"
                            oFRCVar = DirectCast(oFRCVars(sVarName), FRRobot.FRCVar)
                            mnSchedule.Value = (CType(oFRCVar.Value, Integer))
                            mnSchedule.Update()
                        Catch ex As Exception
                            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot([PAVRPADG]dg_def_sched)", _
                                "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            mbUseCC = False
                            mbUsePT = False
                        End Try


                        Try
                            sVarName = "auto_sample"
                            oFRCVar = DirectCast(oFRCVars(sVarName), FRRobot.FRCVar)
                            mbAutoSample.Value = (CType(oFRCVar.Value, Boolean))
                            mbAutoSample.Update()
                        Catch ex As Exception
                            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot([PAVRPADG]auto_sample)", _
                                "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            mbUseCC = False
                            mbUsePT = False
                        End Try

                        Try
                            sVarName = "dg_asyn_enbl"
                            oFRCVar = DirectCast(oFRCVars(sVarName), FRRobot.FRCVar)
                            mbAsyncEnab.Value = (CType(oFRCVar.Value, Boolean))
                            mbAsyncEnab.Update()
                        Catch ex As Exception
                            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot([PAVRPADG]dg_asyn_enbl)", _
                                "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            mbUseCC = False
                            mbUsePT = False
                        End Try
                    Catch ex As Exception
                        mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot(PAVRPADG)", _
                            "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                        mbUseCC = False
                        mbUsePT = False
                    End Try
                End If  'If mbUsePT Then

            Else
                'No PaintTool interface
                mbUseCC = False
                mbUsePT = False
            End If

            'DMON sysvars
            Try
                sVarName = "$DMONCFG"
                oFRCVars = DirectCast(oController.Robot.SysVariables(sVarName), FRRobot.FRCVars)
                oFRCVars.NoRefresh = True
                Try
                    sVarName = "$NUM_DM_ITMS"
                    oFRCVar = DirectCast(oFRCVars(sVarName), FRRobot.FRCVar)
                    mnNumItems = (CType(oFRCVar.Value, Integer))
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONCFG.$NUM_DM_ITMS)", _
                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    mbLoaded = False
                    Return False ' Real problem, don't ignore this one
                End Try
                Try
                    sVarName = "$NUM_DM_SCH"
                    oFRCVar = DirectCast(oFRCVars(sVarName), FRRobot.FRCVar)
                    mnNumSchedules = (CType(oFRCVar.Value, Integer))
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONCFG.$NUM_DM_SCH)", _
                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    mbLoaded = False
                    Return False ' Real problem, don't ignore this one
                End Try
                oFRCVars.NoRefresh = False
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONCFG)", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                mbLoaded = False
                Return False ' Real problem, don't ignore this one
            End Try

            'Schedules
            Try
                sVarName = "$DMONSCH"
                oFRCVars = DirectCast(oController.Robot.SysVariables(sVarName), FRRobot.FRCVars)
                oFRCVars.NoRefresh = True
                If oFRCVars.Count < mnNumSchedules Then
                    mnNumSchedules = oFRCVars.Count
                End If
                If bDetails Then
                    sVarName = "$DMONITEM"
                    oFRCItemVars = DirectCast(oController.Robot.SysVariables(sVarName), FRRobot.FRCVars)
                    oFRCItemVars.NoRefresh = True
                    If oFRCVars.Count < mnNumItems Then
                        mnNumItems = oFRCItemVars.Count
                    End If
                End If
                ReDim mtSchedules(mnNumSchedules - 1)
                For nSched As Integer = 1 To mnNumSchedules
                    oFRCStruct = DirectCast(oFRCVars(nSched.ToString), FRRobot.FRCVars)
                    With mtSchedules(nSched - 1)
                        .COMMENT = New clsTextValue
                        .FILE_NAME = New clsTextValue
                        .DEVICE_NAME = New clsTextValue
                        .DEVICE = New clsIntValue
                        .NUM_ITEMS = New clsIntValue
                        .FILE_INDEX = New clsIntValue
                        .FILE_SIZE = New clsIntValue
                        .SAMP_REQ = New clsSngValue
                        .SAMP_RATE = New Single
                        .MNTR_REQ = New clsSngValue
                        .MNTR_RATE = New Single
                        .REC_REQ = New clsSngValue
                        .REC_PER_SEC = New Single
                        .REC_MODE = New clsIntValue
                        .START_ITEM = New clsIntValue
                        .START_ENBL = New clsBoolValue
                        .START_COND = New clsIntValue
                        .START_VALUE = New clsSngValue
                        .STOP_ITEM = New clsIntValue
                        .STOP_ENBL = New clsBoolValue
                        .STOP_COND = New clsIntValue
                        .STOP_VALUE = New clsSngValue
                        Try
                            sVarName = "$COMMENT"
                            oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                            .COMMENT.Text = (CType(oFRCVar.Value, String))
                            .COMMENT.MaxLength = oFRCVar.MaxStringLen
                            .COMMENT.Update()
                        Catch ex As Exception
                            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$COMMENT)", _
                                "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                        End Try
                        If bDetails Then
                            Try
                                sVarName = "$FILE_NAME"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .FILE_NAME.Text = (CType(oFRCVar.Value, String))
                                .FILE_NAME.MaxLength = oFRCVar.MaxStringLen
                                .FILE_NAME.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$FILE_NAME)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            If msngRobotVersion >= 8.0 Then
                                Try
                                    sVarName = "$DEVICE_NAME"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    .DEVICE_NAME.Text = (CType(oFRCVar.Value, String))
                                    .DEVICE_NAME.Update()
                                    .DEVICE.MaxValue = -1
                                    .DEVICE.MinValue = -1
                                    .DEVICE.Value = -1
                                    .DEVICE.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$DEVICE_NAME)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                End Try
                            Else
                                Try
                                    sVarName = "$DEVICE"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    .DEVICE.Value = (CType(oFRCVar.Value, Integer))
                                    .DEVICE.MaxValue = CType(oFRCVar.MaxValue, Integer)
                                    .DEVICE.MinValue = 1 'Force this    CType(oFRCVar.MinValue, Integer)
                                    .DEVICE.Update()
                                    .DEVICE_NAME.Text = ""
                                    .DEVICE_NAME.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$DEVICE)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                End Try
                            End If
                            Try
                                sVarName = "$NUM_ITEMS"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .NUM_ITEMS.Value = (CType(oFRCVar.Value, Integer))
                                .NUM_ITEMS.MaxValue = CType(oFRCVar.MaxValue, Integer)
                                .NUM_ITEMS.MinValue = CType(oFRCVar.MinValue, Integer)
                                .NUM_ITEMS.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$NUM_ITEMS)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            Try
                                sVarName = "$FILE_INDEX"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .FILE_INDEX.Value = (CType(oFRCVar.Value, Integer))
                                .FILE_INDEX.MaxValue = CType(oFRCVar.MaxValue, Integer)
                                .FILE_INDEX.MinValue = CType(oFRCVar.MinValue, Integer)
                                .FILE_INDEX.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$FILE_INDEX)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            Try
                                sVarName = "$FILE_SIZE"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .FILE_SIZE.Value = (CType(oFRCVar.Value, Integer))
                                .FILE_SIZE.MaxValue = CType(oFRCVar.MaxValue, Integer)
                                .FILE_SIZE.MinValue = CType(oFRCVar.MinValue, Integer)
                                .FILE_SIZE.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$FILE_SIZE)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            Try
                                sVarName = "$SAMP_REQ"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .SAMP_REQ.Value = (CType(oFRCVar.Value, Single))
                                .SAMP_REQ.MaxValue = CType(oFRCVar.MaxValue, Single)
                                .SAMP_REQ.MinValue = CType(oFRCVar.MinValue, Single)
                                .SAMP_REQ.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$SAMP_REQ)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            Try
                                sVarName = "$SAMP_RATE"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                If oFRCVar.IsInitialized Then
                                    .SAMP_RATE = (CType(oFRCVar.Value, Single))
                                Else
                                    .SAMP_RATE = 0
                                End If
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$SAMP_RATE)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            Try
                                sVarName = "$MNTR_REQ"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .MNTR_REQ.Value = (CType(oFRCVar.Value, Single))
                                .MNTR_REQ.MaxValue = CType(oFRCVar.MaxValue, Single)
                                .MNTR_REQ.MinValue = CType(oFRCVar.MinValue, Single)
                                .MNTR_REQ.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$MNTR_REQ)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            Try
                                sVarName = "$MNTR_RATE"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                If oFRCVar.IsInitialized Then
                                    .MNTR_RATE = (CType(oFRCVar.Value, Single))
                                Else
                                    .MNTR_RATE = 0
                                End If
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$MNTR_RATE)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            Try
                                sVarName = "$REC_REQ"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .REC_REQ.Value = (CType(oFRCVar.Value, Single))
                                .REC_REQ.MaxValue = CType(oFRCVar.MaxValue, Single)
                                .REC_REQ.MinValue = CType(oFRCVar.MinValue, Single)
                                .REC_REQ.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$REC_REQ)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            Try
                                sVarName = "$REC_PER_SEC"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                If oFRCVar.IsInitialized Then
                                    .REC_PER_SEC = (CType(oFRCVar.Value, Single))
                                Else
                                    .REC_PER_SEC = 0
                                End If
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$REC_PER_SEC)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            Try
                                sVarName = "$REC_MODE"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .REC_MODE.Value = (CType(oFRCVar.Value, Integer))
                                .REC_MODE.MaxValue = CType(oFRCVar.MaxValue, Integer)
                                .REC_MODE.MinValue = 1 'Force this    CType(oFRCVar.MinValue, Integer)
                                .REC_MODE.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$REC_MODE)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            Try
                                sVarName = "$START_ITEM"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .START_ITEM.Value = (CType(oFRCVar.Value, Integer))
                                .START_ITEM.MaxValue = mnNumItems
                                .START_ITEM.MinValue = 0 'Force this    CType(oFRCVar.MinValue, Integer)
                                .START_ITEM.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$START_ITEM)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            Try
                                sVarName = "$START_ENBL"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .START_ENBL.Value = (CType(oFRCVar.Value, Boolean))
                                .START_ENBL.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$START_ENBL)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            Try
                                sVarName = "$START_COND"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .START_COND.Value = (CType(oFRCVar.Value, Integer))
                                .START_COND.MaxValue = CType(oFRCVar.MaxValue, Integer)
                                .START_COND.MinValue = 1
                                .START_COND.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$START_COND)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            Try
                                sVarName = "$START_VALUE"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .START_VALUE.Value = (CType(oFRCVar.Value, Single))
                                .START_VALUE.MaxValue = CType(oFRCVar.MaxValue, Single)
                                .START_VALUE.MinValue = CType(oFRCVar.MinValue, Single)
                                .START_VALUE.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$START_VALUE)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            Try
                                sVarName = "$STOP_ITEM"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .STOP_ITEM.Value = (CType(oFRCVar.Value, Integer))
                                .STOP_ITEM.MaxValue = mnNumItems
                                .STOP_ITEM.MinValue = 0 'Force this    CType(oFRCVar.MinValue, Integer)
                                .STOP_ITEM.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$STOP_ITEM)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            Try
                                sVarName = "$STOP_ENBL"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .STOP_ENBL.Value = (CType(oFRCVar.Value, Boolean))
                                .STOP_ENBL.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$STOP_ENBL)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            Try
                                sVarName = "$STOP_COND"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .STOP_COND.Value = (CType(oFRCVar.Value, Integer))
                                .STOP_COND.MaxValue = CType(oFRCVar.MaxValue, Integer)
                                .STOP_COND.MinValue = 1
                                .STOP_COND.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$STOP_COND)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            Try
                                sVarName = "$STOP_VALUE"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .STOP_VALUE.Value = (CType(oFRCVar.Value, Single))
                                .STOP_VALUE.MaxValue = CType(oFRCVar.MaxValue, Single)
                                .STOP_VALUE.MinValue = CType(oFRCVar.MinValue, Single)
                                .STOP_VALUE.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$STOP_VALUE)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            '      Dim ITEM() As clsIntValue
                            Try
                                sVarName = "$ITEM"
                                oFRCArray = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVars)
                                Dim nNumItems As Integer = oFRCArray.Count
                                ReDim .ITEM(nNumItems - 1)
                                For nItem As Integer = 1 To nNumItems
                                    .ITEM(nItem - 1) = New clsIntValue
                                    oFRCVar = DirectCast(oFRCArray(nItem.ToString), FRRobot.FRCVar)
                                    .ITEM(nItem - 1).Value = (CType(oFRCVar.Value, Integer))
                                    .ITEM(nItem - 1).MaxValue = mnNumItems
                                    .ITEM(nItem - 1).MinValue = 0 'Force this    CType(oFRCVar.MinValue, Integer)
                                    .ITEM(nItem - 1).Update()
                                Next
                                mnMaxItemsPerSchedule = nNumItems
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH[" & nSched.ToString & "].$ITEM)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                        End If
                    End With
                Next
                oFRCVars.NoRefresh = False
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONSCH)", _
               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                mbLoaded = False
                Return False ' Real problem, don't ignore this one
            End Try




            'DMONITEMS
            If bDetails Then
                mbDisplayItemAXIS = msngRobotVersion > 7.0
                mbDisplayItemSquare = msngRobotVersion > 7.0
                Try
                    ReDim mtItems(mnNumItems - 1)
                    For nItem As Integer = 1 To mnNumItems
                        oFRCStruct = DirectCast(oFRCItemVars(nItem.ToString), FRRobot.FRCVars)
                        With mtItems(nItem - 1)
                            .PRG_NAME = New clsTextValue
                            .VAR_NAME = New clsTextValue
                            .DESC = New clsTextValue
                            .UNITS = New clsTextValue
                            .TYPE = New clsIntValue
                            .IO_TYPE = New clsIntValue
                            .PORT_NUM = New clsIntValue
                            .SLOPE = New clsSngValue
                            .INTERCEPT = New clsSngValue
                            .SQUARE = New clsSngValue
                            .GROUP_NUM = New clsIntValue
                            .AXIS_NUM = New clsIntValue
                            Try
                                sVarName = "$DESC"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .DESC.Text = (CType(oFRCVar.Value, String))
                                .DESC.MaxLength = 33  'Robot isn't reporting this correctly oFRCVar.MaxStringLen
                                .DESC.Update()
                            Catch ex As Exception
                                'mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONITEM[" & nItem.ToString & "].$DESC)", _
                                '    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                .DESC.Text = ""
                                .DESC.MaxLength = 33  'Robot isn't reporting this correctly oFRCVar.MaxStringLen
                                .DESC.Update()
                            End Try
                            Try
                                sVarName = "$UNITS"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .UNITS.Text = (CType(oFRCVar.Value, String))
                                .UNITS.MaxLength = 17  'Robot isn't reporting this correctly oFRCVar.MaxStringLen
                                .UNITS.Update()
                            Catch ex As Exception
                                'mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONITEM[" & nItem.ToString & "].$UNITS)", _
                                '    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                .UNITS.Text = ""
                                .UNITS.MaxLength = 17  'Robot isn't reporting this correctly oFRCVar.MaxStringLen
                                .UNITS.Update()
                            End Try
                            Try
                                sVarName = "$PRG_NAME"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .PRG_NAME.Text = (CType(oFRCVar.Value, String))
                                .PRG_NAME.MaxLength = 37 'Robot isn't reporting this correctly oFRCVar.MaxStringLen
                                .PRG_NAME.Update()
                            Catch ex As Exception
                                'mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONITEM[" & nItem.ToString & "].$PRG_NAME)", _
                                '    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                .PRG_NAME.Text = ""
                                .PRG_NAME.MaxLength = 37 'Robot isn't reporting this correctly oFRCVar.MaxStringLen
                                .PRG_NAME.Update()
                            End Try
                            Try
                                sVarName = "$VAR_NAME"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .VAR_NAME.Text = (CType(oFRCVar.Value, String))
                                .VAR_NAME.MaxLength = 61  'Robot isn't reporting this correctly oFRCVar.MaxStringLen
                                .VAR_NAME.Update()
                            Catch ex As Exception
                                'mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONITEM[" & nItem.ToString & "].$VAR_NAME)", _
                                '    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                .VAR_NAME.Text = ""
                                .VAR_NAME.MaxLength = 61  'Robot isn't reporting this correctly oFRCVar.MaxStringLen
                                .VAR_NAME.Update()
                            End Try
                            Try
                                sVarName = "$TYPE"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .TYPE.Value = (CType(oFRCVar.Value, Integer))
                                .TYPE.MaxValue = CType(oFRCVar.MaxValue, Integer)
                                .TYPE.MinValue = CType(oFRCVar.MinValue, Integer)
                                .TYPE.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONITEM[" & nItem.ToString & "].$TYPE)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                .TYPE.Value = 16
                                .TYPE.MaxValue = 216
                                .TYPE.MinValue = -1
                                .TYPE.Update()
                            End Try
                            Try
                                sVarName = "$IO_TYPE"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .IO_TYPE.Value = (CType(oFRCVar.Value, Integer))
                                .IO_TYPE.MaxValue = CType(oFRCVar.MaxValue, Integer)
                                .IO_TYPE.MinValue = CType(oFRCVar.MinValue, Integer)
                                .IO_TYPE.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONITEM[" & nItem.ToString & "].$IO_TYPE)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                .IO_TYPE.Value = -1
                                .IO_TYPE.MaxValue = 19
                                .IO_TYPE.MinValue = -1
                                .IO_TYPE.Update()
                            End Try
                            Try
                                sVarName = "$PORT_NUM"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .PORT_NUM.Value = (CType(oFRCVar.Value, Integer))
                                .PORT_NUM.MaxValue = CType(oFRCVar.MaxValue, Integer)
                                .PORT_NUM.MinValue = CType(oFRCVar.MinValue, Integer)
                                .PORT_NUM.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONITEM[" & nItem.ToString & "].$PORT_NUM)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                .PORT_NUM.Value = 0
                                .PORT_NUM.Update()
                            End Try
                            Try
                                sVarName = "$SLOPE"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .SLOPE.Value = (CType(oFRCVar.Value, Single))
                                .SLOPE.MaxValue = CType(oFRCVar.MaxValue, Single)
                                .SLOPE.MinValue = CType(oFRCVar.MinValue, Single)
                                .SLOPE.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONITEM[" & nItem.ToString & "].$SLOPE)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                .SLOPE.Value = 0
                                .SLOPE.Update()
                            End Try
                            Try
                                sVarName = "$INTERCEPT"
                                oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                .INTERCEPT.Value = (CType(oFRCVar.Value, Single))
                                .INTERCEPT.MaxValue = CType(oFRCVar.MaxValue, Single)
                                .INTERCEPT.MinValue = CType(oFRCVar.MinValue, Single)
                                .INTERCEPT.Update()
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONITEM[" & nItem.ToString & "].$INTERCEPT)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                .INTERCEPT.Value = 0
                                .INTERCEPT.Update()
                            End Try
                            If mbDisplayItemSquare Then
                                Try
                                    sVarName = "$SQUARE"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    .SQUARE.Value = (CType(oFRCVar.Value, Single))
                                    .SQUARE.MaxValue = CType(oFRCVar.MaxValue, Single)
                                    .SQUARE.MinValue = CType(oFRCVar.MinValue, Single)
                                    .SQUARE.Update()
                                    mbDisplayItemSquare = True
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONITEM[" & nItem.ToString & "].$SQUARE)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    .SQUARE.Value = 0
                                    .SQUARE.Update()
                                    mbDisplayItemSquare = False
                                End Try
                            End If
                            If mbDisplayItemAXIS Then
                                Try
                                    sVarName = "$GROUP_NUM"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    .GROUP_NUM.Value = (CType(oFRCVar.Value, Integer))
                                    .GROUP_NUM.MaxValue = CType(oFRCVar.MaxValue, Integer)
                                    .GROUP_NUM.MinValue = CType(oFRCVar.MinValue, Integer)
                                    .GROUP_NUM.Update()
                                    sVarName = "$AXIS_NUM"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    .AXIS_NUM.Value = (CType(oFRCVar.Value, Integer))
                                    .AXIS_NUM.MaxValue = CType(oFRCVar.MaxValue, Integer)
                                    .AXIS_NUM.MinValue = CType(oFRCVar.MinValue, Integer)
                                    .AXIS_NUM.Update()
                                    mbDisplayItemAXIS = True
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONITEM[" & nItem.ToString & "].$AXIS_NUM)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    .GROUP_NUM.Value = 0
                                    .GROUP_NUM.Update()
                                    .AXIS_NUM.Value = 0
                                    .AXIS_NUM.Update()
                                    mbDisplayItemAXIS = False
                                End Try
                            End If
                        End With
                    Next
                    oFRCVars.NoRefresh = False
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot($DMONITEM)", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

                    Return False ' Real problem, don't ignore this one
                End Try

            End If

            mbLoaded = True
            Return True ' No problems that we won't try to ignore
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: LoadFromRobot", _
           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            mbLoaded = False
            Return False
        End Try

    End Function

    Friend Function SaveToRobot(ByRef oController As clsController, ByVal bDetails As Boolean, _
                                ByRef oZone As clsZone) As Boolean
        '********************************************************************************************
        'Save data to robot controller
        '
        'Parameters: none
        'Returns: true if successful
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         12/22/11      Add support for version < 6.3 cc variables
        ' MSW         04/24/12      SaveToRobot - Handle openers without throwing an error.
        ' MSW         10/10/12      Indexing change to mbEnableCCDiag so it changes back to black after a save 
        '********************************************************************************************
        Try
            Dim sVarName As String
            Dim sProgName As String
            Dim oFRCProg As FRRobot.FRCProgram
            Dim oFRCVars As FRRobot.FRCVars
            Dim oFRCArray As FRRobot.FRCVars
            Dim oFRCStruct As FRRobot.FRCVars
            Dim oFRCVar As FRRobot.FRCVar

            Dim oFRCPrograms As FRRobot.FRCPrograms = oController.Robot.Programs

            Dim bOK As Boolean = True 'Return status
            Dim sOldVal As String = String.Empty
            Dim sNewVal As String = String.Empty
            Dim bSkipEnableChangeLog As Boolean = False
            If mbUseCC Then
                'Force an update to which equipment is selected for cc diag
                EnableCCDiag = EnableCCDiag
                If mnSchedule.Changed And (mbAutoSample.Changed = False) And _
                    (mbAsyncEnab.Changed = False) And EnableCCDiagChanged Then
                    bSkipEnableChangeLog = True
                End If
            End If

            'PT Diag vars
            If mbUsePT Then
                If Changed Then
                    'Check PT DMON setup screen
                    Dim oCurScreen As FRRobot.FRCTPScreen = oController.Robot.TPScreen
                    Dim nSoftPartID As Integer = -1
                    Dim nScreenID As Integer = -1
                    Dim sTitle As String = String.Empty
                    oCurScreen.GetCurScreen(nSoftPartID, nScreenID, sTitle)
                    Debug.Print("nSoftPartID: " & nSoftPartID.ToString)
                    Debug.Print("nScreenID: " & nScreenID.ToString)
                    Debug.Print("sTitle: " & sTitle)
                End If
                Try
                    sProgName = "PAVRPADG"
                    oFRCProg = DirectCast(oFRCPrograms(sProgName), FRRobot.FRCProgram)
                    oFRCVars = oFRCProg.Variables
                    If mnSchedule.Changed Then
                        Try
                            sOldVal = mnSchedule.OldValue.ToString
                            sNewVal = mnSchedule.Value.ToString
                            sVarName = "dg_def_sched"
                            oFRCVar = DirectCast(oFRCVars(sVarName), FRRobot.FRCVar)
                            oFRCVar.Value = mnSchedule.Value
                            mnSchedule.Update()

                            frmMain.subUpdateChangeLog(sNewVal, sOldVal, oZone, oController.Name, _
                                                       gpsRM.GetString("psDMON_SCHEDULE"))
                        Catch ex As Exception
                            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot([PAVRPADG]dg_def_sched)", _
                                "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            bOK = False
                        End Try
                    End If

                    Try
                        'Combine all the auto sample vars into one change log entry
                        If mbAutoSample.Changed Or mbAsyncEnab.Changed Or _
                            (mbUseCC And EnableCCDiagChanged) Then
                            If mbAutoSample.OldValue Then
                                If mbUseCC Then
                                    sOldVal = gcsRM.GetString("csJOB")
                                Else
                                    sOldVal = gcsRM.GetString("csON")
                                End If
                            ElseIf EnableCCDiagOldValue Then
                                sOldVal = gcsRM.GetString("csCCCYCLE")
                            Else
                                sOldVal = gcsRM.GetString("csOFF")
                            End If
                            If mbAutoSample.Value Then
                                If mbUseCC Then
                                    sNewVal = gcsRM.GetString("csJOB")
                                Else
                                    sNewVal = gcsRM.GetString("csON")
                                End If
                            ElseIf EnableCCDiag Then
                                sNewVal = gcsRM.GetString("csCCCYCLE")
                            Else
                                sNewVal = gcsRM.GetString("csOFF")
                            End If

                            sVarName = "auto_sample"
                            oFRCVar = DirectCast(oFRCVars(sVarName), FRRobot.FRCVar)
                            oFRCVar.Value = mbAutoSample.Value
                            mbAutoSample.Update()

                            sVarName = "dg_asyn_enbl"
                            oFRCVar = DirectCast(oFRCVars(sVarName), FRRobot.FRCVar)
                            oFRCVar.Value = mbAsyncEnab.Value
                            mbAsyncEnab.Update()

                            If mbUseCC Then
                                If msngRobotVersion >= 6.3 Then
                                    sProgName = "PAVRCCEX"
                                    oFRCProg = DirectCast(oFRCPrograms(sProgName), FRRobot.FRCProgram)
                                    oFRCVars = oFRCProg.Variables
                                    oFRCArray = DirectCast(oFRCVars("tCCEqCmos"), FRRobot.FRCVars)
                                    For nArm As Integer = 1 To oController.Arms.Count
                                        If oController.Arms(nArm - 1).IsOpener = False Then
                                            oFRCStruct = DirectCast(oFRCArray(nArm.ToString), FRRobot.FRCVars)
                                            sVarName = "tCCEqCmos[" & nArm.ToString & "].bCCDiag"
                                            oFRCVar = DirectCast(oFRCVars(sVarName), FRRobot.FRCVar)
                                            oFRCVar.Value = mbEnableCCDiag(nArm).Value
                                            mbEnableCCDiag(nArm).Update()
                                        End If
                                    Next
                                Else
                                    sProgName = "PACCRUNC"
                                    oFRCProg = DirectCast(oFRCPrograms(sProgName), FRRobot.FRCProgram)
                                    oFRCVars = oFRCProg.Variables
                                    oFRCVar = DirectCast(oFRCVars("bCCDiag"), FRRobot.FRCVar)
                                    oFRCVar.Value = mbEnableCCDiag(1).Value
                                End If
                                mbEnableCCDiag(0).Update()
                            End If
                            If bSkipEnableChangeLog = False Then
                                frmMain.subUpdateChangeLog(sNewVal, sOldVal, oZone, oController.Name, _
                                                           gpsRM.GetString("psDMON_AUTO_SAMPLE"))
                            End If
                        End If
                    Catch ex As Exception
                        mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot([PAVRPADG]auto_sample)", _
                            "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                        bOK = False
                    End Try

                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot(PAVRPADG)", _
                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    bOK = False
                End Try
            End If  'If mbUsePT Then


            'Schedules
            If bDetails Then
                Try
                    sVarName = "$DMONSCH"
                    oFRCVars = DirectCast(oController.Robot.SysVariables(sVarName), FRRobot.FRCVars)
                    If oFRCVars.Count < mnNumSchedules Then
                        mnNumSchedules = oFRCVars.Count
                    End If
                    For nSched As Integer = 1 To mnNumSchedules
                        oFRCStruct = DirectCast(oFRCVars(nSched.ToString), FRRobot.FRCVars)
                        With mtSchedules(nSched - 1)
                            If .COMMENT.Changed Then
                                Try
                                    sVarName = "$COMMENT"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .COMMENT.Text
                                    frmMain.subUpdateChangeLog(.COMMENT.Text, .COMMENT.OldText, oZone, oController.Name, _
                                           String.Format(gpsRM.GetString("psDMON_SCHEDULE_COMMENT"), nSched.ToString))
                                    .COMMENT.Update()

                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$COMMENT)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .FILE_NAME.Changed Then
                                Try
                                    sVarName = "$FILE_NAME"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .FILE_NAME.Text
                                    frmMain.subUpdateChangeLog(.FILE_NAME.Text, .FILE_NAME.OldText, oZone, oController.Name, _
                                            String.Format(gpsRM.GetString("psDMON_SCHEDULE_FILE_NAME"), nSched.ToString))
                                    .FILE_NAME.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$FILE_NAME)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If msngRobotVersion >= 8.0 Then
                                If .DEVICE_NAME.Changed Then
                                    Try
                                        sVarName = "$DEVICE_NAME"
                                        oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                        oFRCVar.Value = .DEVICE_NAME.Text
                                        frmMain.subUpdateChangeLog(gpsRM.GetString("psFILEDEVICE" & .DEVICE_NAME.Text), _
                                                gpsRM.GetString("psFILEDEVICE" & .DEVICE_NAME.OldText), oZone, oController.Name, _
                                                String.Format(gpsRM.GetString("psDMON_SCHEDULE_DEVICE"), nSched.ToString))
                                        .DEVICE.Update()
                                    Catch ex As Exception
                                        mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$DEVICE_NAME)", _
                                            "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                        bOK = False
                                    End Try
                                End If
                            Else
                                If .DEVICE.Changed Then
                                    Try
                                        sVarName = "$DEVICE"
                                        oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                        oFRCVar.Value = .DEVICE.Value
                                        frmMain.subUpdateChangeLog(gpsRM.GetString("psFILEDEVICE" & .DEVICE.Value.ToString), _
                                                gpsRM.GetString("psFILEDEVICE" & .DEVICE.OldValue.ToString), oZone, oController.Name, _
                                                String.Format(gpsRM.GetString("psDMON_SCHEDULE_DEVICE"), nSched.ToString))
                                        .DEVICE.Update()
                                    Catch ex As Exception
                                        mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$DEVICE)", _
                                            "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                        bOK = False
                                    End Try
                                End If
                            End If
                            If .NUM_ITEMS.Changed Then
                                Try
                                    sVarName = "$NUM_ITEMS"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .NUM_ITEMS.Value
                                    frmMain.subUpdateChangeLog(.NUM_ITEMS.Value.ToString, .NUM_ITEMS.OldValue.ToString, oZone, oController.Name, _
                                        String.Format(gpsRM.GetString("psDMON_SCHEDULE_NUM_ITEMS"), nSched.ToString))
                                    .NUM_ITEMS.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$NUM_ITEMS)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .FILE_INDEX.Changed Then
                                Try
                                    sVarName = "$FILE_INDEX"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .FILE_INDEX.Value
                                    frmMain.subUpdateChangeLog(.FILE_INDEX.Value.ToString, .FILE_INDEX.OldValue.ToString, oZone, oController.Name, _
                                        String.Format(gpsRM.GetString("psDMON_SCHEDULE_FILE_INDEX"), nSched.ToString))
                                    .FILE_INDEX.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$FILE_INDEX)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .FILE_SIZE.Changed Then
                                Try
                                    sVarName = "$FILE_SIZE"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .FILE_SIZE.Value
                                    frmMain.subUpdateChangeLog(.FILE_SIZE.Value.ToString, .FILE_SIZE.OldValue.ToString, oZone, oController.Name, _
                                        String.Format(gpsRM.GetString("psDMON_SCHEDULE_FILE_SIZE"), nSched.ToString))
                                    .FILE_SIZE.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$FILE_SIZE)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .SAMP_REQ.Changed Then
                                Try
                                    sVarName = "$SAMP_REQ"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .SAMP_REQ.Value
                                    frmMain.subUpdateChangeLog(.SAMP_REQ.Value.ToString, .SAMP_REQ.OldValue.ToString, oZone, oController.Name, _
                                        String.Format(gpsRM.GetString("psDMON_SCHEDULE_SAMP_REQ"), nSched.ToString))
                                    .SAMP_REQ.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$SAMP_REQ)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .MNTR_REQ.Changed Then
                                Try
                                    sVarName = "$MNTR_REQ"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .MNTR_REQ.Value
                                    frmMain.subUpdateChangeLog(.MNTR_REQ.Value.ToString, .MNTR_REQ.OldValue.ToString, oZone, oController.Name, _
                                        String.Format(gpsRM.GetString("psDMON_SCHEDULE_MNTR_REQ"), nSched.ToString))
                                    .MNTR_REQ.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$MNTR_REQ)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .REC_REQ.Changed Then
                                Try
                                    sVarName = "$REC_REQ"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .REC_REQ.Value
                                    frmMain.subUpdateChangeLog(.REC_REQ.Value.ToString, .REC_REQ.OldValue.ToString, oZone, oController.Name, _
                                        String.Format(gpsRM.GetString("psDMON_SCHEDULE_REC_REQ"), nSched.ToString))
                                    .REC_REQ.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$REC_REQ)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .REC_MODE.Changed Then
                                Try
                                    sVarName = "$REC_MODE"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .REC_MODE.Value
                                    frmMain.subUpdateChangeLog(gpsRM.GetString("psRECORDMODE" & .REC_MODE.Value.ToString), _
                                            gpsRM.GetString("psRECORDMODE" & .REC_MODE.OldValue.ToString), oZone, oController.Name, _
                                            String.Format(gpsRM.GetString("psDMON_SCHEDULE_REC_MODE"), nSched.ToString))
                                    .REC_MODE.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$REC_MODE)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .START_ITEM.Changed Then
                                Try
                                    sVarName = "$START_ITEM"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .START_ITEM.Value
                                    frmMain.subUpdateChangeLog(.START_ITEM.Value.ToString, .START_ITEM.OldValue.ToString, oZone, oController.Name, _
                                        String.Format(gpsRM.GetString("psDMON_SCHEDULE_START_ITEM"), nSched.ToString))
                                    .START_ITEM.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$START_ITEM)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .START_ENBL.Changed Then
                                Try
                                    sVarName = "$START_ENBL"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .START_ENBL.Value
                                    frmMain.subUpdateChangeLog(.START_ENBL.Value.ToString, .START_ENBL.OldValue.ToString, oZone, oController.Name, _
                                        String.Format(gpsRM.GetString("psDMON_SCHEDULE_START_ENBL"), nSched.ToString))
                                    .START_ENBL.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$START_ENBL)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .START_COND.Changed Then
                                Try
                                    sVarName = "$START_COND"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .START_COND.Value
                                    frmMain.subUpdateChangeLog(gpsRM.GetString("psSTARTSTOPCOND" & .START_COND.Value.ToString), _
                                            gpsRM.GetString("psSTARTSTOPCOND" & .START_COND.OldValue.ToString), oZone, oController.Name, _
                                            String.Format(gpsRM.GetString("psDMON_SCHEDULE_START_COND"), nSched.ToString))
                                    .START_COND.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$START_COND)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .START_VALUE.Changed Then
                                Try
                                    sVarName = "$START_VALUE"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .START_VALUE.Value
                                    frmMain.subUpdateChangeLog(.START_VALUE.Value.ToString, .START_VALUE.OldValue.ToString, oZone, oController.Name, _
                                        String.Format(gpsRM.GetString("psDMON_SCHEDULE_START_VALUE"), nSched.ToString))
                                    .START_VALUE.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$START_VALUE)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .STOP_ITEM.Changed Then
                                Try
                                    sVarName = "$STOP_ITEM"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .STOP_ITEM.Value
                                    frmMain.subUpdateChangeLog(.STOP_ITEM.Value.ToString, .STOP_ITEM.OldValue.ToString, oZone, oController.Name, _
                                        String.Format(gpsRM.GetString("psDMON_SCHEDULE_STOP_ITEM"), nSched.ToString))
                                    .STOP_ITEM.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$STOP_ITEM)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .STOP_ENBL.Changed Then
                                Try
                                    sVarName = "$STOP_ENBL"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .STOP_ENBL.Value
                                    frmMain.subUpdateChangeLog(.STOP_ENBL.Value.ToString, .STOP_ENBL.OldValue.ToString, oZone, oController.Name, _
                                        String.Format(gpsRM.GetString("psDMON_SCHEDULE_STOP_ENBL"), nSched.ToString))
                                    .STOP_ENBL.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$STOP_ENBL)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .STOP_COND.Changed Then
                                Try
                                    sVarName = "$STOP_COND"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .STOP_COND.Value
                                    frmMain.subUpdateChangeLog(gpsRM.GetString("psSTARTSTOPCOND" & .STOP_COND.Value.ToString), _
                                            gpsRM.GetString("psSTARTSTOPCOND" & .STOP_COND.OldValue.ToString), oZone, oController.Name, _
                                            String.Format(gpsRM.GetString("psDMON_SCHEDULE_STOP_COND"), nSched.ToString))
                                    .STOP_COND.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$STOP_COND)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .STOP_VALUE.Changed Then
                                Try
                                    sVarName = "$STOP_VALUE"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .STOP_VALUE.Value
                                    frmMain.subUpdateChangeLog(.STOP_VALUE.Value.ToString, .STOP_VALUE.OldValue.ToString, oZone, oController.Name, _
                                        String.Format(gpsRM.GetString("psDMON_SCHEDULE_STOP_VALUE"), nSched.ToString))
                                    .STOP_VALUE.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$STOP_VALUE)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            Try
                                sVarName = "$ITEM"
                                oFRCArray = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVars)
                                Dim nNumItems As Integer = oFRCArray.Count
                                For nItem As Integer = 1 To nNumItems
                                    If .ITEM(nItem - 1).Changed Then
                                        oFRCVar = DirectCast(oFRCArray(nItem.ToString), FRRobot.FRCVar)
                                        oFRCVar.Value = .ITEM(nItem - 1).Value
                                        frmMain.subUpdateChangeLog(.ITEM(nItem - 1).Value.ToString, .ITEM(nItem - 1).OldValue.ToString, oZone, oController.Name, _
                                            String.Format(gpsRM.GetString("psDMON_SCHEDULE_ITEM"), nSched.ToString, nItem.ToString))
                                        .ITEM(nItem - 1).Update()
                                    End If
                                Next
                            Catch ex As Exception
                                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH[" & nSched.ToString & "].$ITEM)", _
                                    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                bOK = False
                            End Try
                        End With
                    Next
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONSCH)", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    bOK = False
                End Try




                'DMONITEMS
                Try
                    sVarName = "$DMONITEM"
                    oFRCVars = DirectCast(oController.Robot.SysVariables(sVarName), FRRobot.FRCVars)
                    If oFRCVars.Count < mnNumItems Then
                        mnNumItems = oFRCVars.Count
                    End If
                    For nItem As Integer = 1 To mnNumItems
                        oFRCStruct = DirectCast(oFRCVars(nItem.ToString), FRRobot.FRCVars)
                        With mtItems(nItem - 1)
                            If .DESC.Changed Then
                                Try
                                    sVarName = "$DESC"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .DESC.Text
                                    frmMain.subUpdateChangeLog(.DESC.Text, .DESC.OldText, oZone, oController.Name, _
                                            String.Format(gpsRM.GetString("psDMON_ITEM_DESC"), nItem.ToString))
                                    .DESC.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONITEM[" & nItem.ToString & "].$DESC)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .UNITS.Changed Then
                                Try
                                    sVarName = "$UNITS"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .UNITS.Text
                                    frmMain.subUpdateChangeLog(.UNITS.Text, .UNITS.OldText, oZone, oController.Name, _
                                            String.Format(gpsRM.GetString("psDMON_ITEM_UNITS"), nItem.ToString))
                                    .UNITS.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONITEM[" & nItem.ToString & "].$UNITS)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .PRG_NAME.Changed Then
                                Try
                                    sVarName = "$PRG_NAME"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .PRG_NAME.Text
                                    frmMain.subUpdateChangeLog(.PRG_NAME.Text, .PRG_NAME.OldText, oZone, oController.Name, _
                                           String.Format(gpsRM.GetString("psDMON_ITEM_PRG_NAME"), nItem.ToString))
                                    .PRG_NAME.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONITEM[" & nItem.ToString & "].$PRG_NAME)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .VAR_NAME.Changed Then
                                Try
                                    sVarName = "$VAR_NAME"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .VAR_NAME.Text
                                    frmMain.subUpdateChangeLog(.VAR_NAME.Text, .VAR_NAME.OldText, oZone, oController.Name, _
                                           String.Format(gpsRM.GetString("psDMON_ITEM_VAR_NAME"), nItem.ToString))
                                    .VAR_NAME.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONITEM[" & nItem.ToString & "].$VAR_NAME)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .TYPE.Changed Then
                                Try
                                    sVarName = "$TYPE"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .TYPE.Value
                                    frmMain.subUpdateChangeLog(.TYPE.Value.ToString, .TYPE.OldValue.ToString, oZone, oController.Name, _
                                          String.Format(gpsRM.GetString("psDMON_ITEM_TYPE"), nItem.ToString))
                                    .TYPE.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONITEM[" & nItem.ToString & "].$TYPE)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .IO_TYPE.Changed Then
                                Try
                                    sVarName = "$IO_TYPE"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .IO_TYPE.Value
                                    frmMain.subUpdateChangeLog(.IO_TYPE.Value.ToString, .IO_TYPE.OldValue.ToString, oZone, oController.Name, _
                                           String.Format(gpsRM.GetString("psDMON_ITEM_IO_TYPE"), nItem.ToString))
                                    .IO_TYPE.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONITEM[" & nItem.ToString & "].$IO_TYPE)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .PORT_NUM.Changed Then
                                Try
                                    sVarName = "$PORT_NUM"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .PORT_NUM.Value
                                    frmMain.subUpdateChangeLog(.PORT_NUM.Value.ToString, .PORT_NUM.OldValue.ToString, oZone, oController.Name, _
                                          String.Format(gpsRM.GetString("psDMON_ITEM_PORT_NUM"), nItem.ToString))
                                    .PORT_NUM.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONITEM[" & nItem.ToString & "].$PORT_NUM)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .SLOPE.Changed Then
                                Try
                                    sVarName = "$SLOPE"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .SLOPE.Value
                                    frmMain.subUpdateChangeLog(.SLOPE.Value.ToString, .SLOPE.OldValue.ToString, oZone, oController.Name, _
                                          String.Format(gpsRM.GetString("psDMON_ITEM_SLOPE"), nItem.ToString))
                                    .SLOPE.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONITEM[" & nItem.ToString & "].$SLOPE)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If .INTERCEPT.Changed Then
                                Try
                                    sVarName = "$INTERCEPT"
                                    oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                    oFRCVar.Value = .INTERCEPT.Value
                                    frmMain.subUpdateChangeLog(.INTERCEPT.Value.ToString, .INTERCEPT.OldValue.ToString, oZone, oController.Name, _
                                          String.Format(gpsRM.GetString("psDMON_ITEM_INTERCEPT"), nItem.ToString))
                                    .INTERCEPT.Update()
                                Catch ex As Exception
                                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONITEM[" & nItem.ToString & "].$INTERCEPT)", _
                                        "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                    bOK = False
                                End Try
                            End If
                            If mbDisplayItemSquare Then
                                If .SQUARE.Changed Then
                                    Try
                                        sVarName = "$SQUARE"
                                        oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                        oFRCVar.Value = .SQUARE.Value
                                        frmMain.subUpdateChangeLog(.SQUARE.Value.ToString, .SQUARE.OldValue.ToString, oZone, oController.Name, _
                                            String.Format(gpsRM.GetString("psDMON_ITEM_SQUARE"), nItem.ToString))
                                        .SQUARE.Update()
                                    Catch ex As Exception
                                        mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONITEM[" & nItem.ToString & "].$SQUARE)", _
                                            "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                        bOK = False
                                    End Try
                                End If
                            End If
                            If mbDisplayItemAXIS Then
                                If .GROUP_NUM.Changed Then
                                    Try
                                        sVarName = "$GROUP_NUM"
                                        oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                        oFRCVar.Value = .GROUP_NUM.Value
                                        frmMain.subUpdateChangeLog(.GROUP_NUM.Value.ToString, .GROUP_NUM.OldValue.ToString, oZone, oController.Name, _
                                                   String.Format(gpsRM.GetString("psDMON_ITEM_GROUP"), nItem.ToString))
                                        .GROUP_NUM.Update()
                                    Catch ex As Exception
                                        mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONITEM[" & nItem.ToString & "].$GROUP)", _
                                            "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                        bOK = False
                                    End Try
                                End If
                                If .AXIS_NUM.Changed Then
                                    Try
                                        sVarName = "$AXIS_NUM"
                                        oFRCVar = DirectCast(oFRCStruct(sVarName), FRRobot.FRCVar)
                                        oFRCVar.Value = .AXIS_NUM.Value
                                        frmMain.subUpdateChangeLog(.AXIS_NUM.Value.ToString, .AXIS_NUM.OldValue.ToString, oZone, oController.Name, _
                                            String.Format(gpsRM.GetString("psDMON_ITEM_AXIS"), nItem.ToString))
                                        .AXIS_NUM.Update()
                                    Catch ex As Exception
                                        mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONITEM[" & nItem.ToString & "].$AXIS)", _
                                            "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                        bOK = False
                                    End Try
                                End If
                            End If
                        End With
                    Next
                Catch ex As Exception
                    mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot($DMONITEM)", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    bOK = False
                End Try

            End If
            Return bOK
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module:clsDMONCfg Routine: SaveToRobot", _
           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Return False
        End Try

    End Function
End Class
