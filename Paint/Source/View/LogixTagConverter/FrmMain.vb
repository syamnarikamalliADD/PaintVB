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
' Form/Module: frmMain
'
' Description: LogixTagConverter
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
'    Date       By      Reason                                                        Version
'    03/06/2011 MSW     first draft                                                   4.01.01.00
'    04/11/12   MSW     Update some of the indexing                                   4.01.03.00
'    06/25/13   MSW     Change >=6 to >=7                                             4.01.05.00
'********************************************************************************************
Public Class frmMain
    Dim colRacks As New Collection
    Dim nDisplayRack As Integer 'Index to rack collection
    Dim nDisplaySlot As Integer 'Index to slot collection
    Dim nRackRowEntered As Integer
    Dim nSlotRowEntered As Integer
    Private Function nAddRack(ByVal sRack As String, Optional ByVal nSlots As Integer = -1) As Integer
        Dim bFound As Boolean = False
        Dim nRackNumber As Integer = 0
        If sRack = "Local" Then
            nRackNumber = 1
        ElseIf sRack.Length >= 7 AndAlso sRack.Substring(0, 7) = "ENET_RC" Then
            'TAG		ENET_RC2:I		AB:ETHERNET_MODULE_INT_56Bytes:I:0		(Class := Standard)	
            Dim sTmp() As String = Split(sRack, ":")
            Dim nRobot As Integer = sTmp(0).Substring(7)
            nRackNumber = 48 + 2 * nRobot
            If sTmp(1) = "I" Then
                nRackNumber = nRackNumber + 1
            End If
        Else
            For nIdx As Integer = 0 To sRack.Length - 1
                If Char.IsDigit(sRack, nIdx) Then
                    nRackNumber = nRackNumber * 10 + CType(sRack.Substring(nIdx, 1), Integer)
                End If
            Next
        End If
        Dim nInsert As Integer = -1
        If colRacks.Count > 0 Then
            For nIdx As Integer = 1 To colRacks.Count
                Dim oRackTmp As clsRack = colRacks(nIdx)
                If oRackTmp.nRackNumber = nRackNumber Then
                    bFound = True
                    If nSlots > 0 Then
                        oRackTmp.nSlots = nSlots
                    End If
                    Exit For
                ElseIf nRackNumber < oRackTmp.nRackNumber Then
                    'Insert
                    nInsert = nIdx
                    Exit For
                End If
            Next
        End If
        If bFound = False Then
            Dim oRackTmp2 As New clsRack
            If nSlots > 0 Then
                oRackTmp2.nSlots = nSlots
            End If
            oRackTmp2.sRackName = sRack
            oRackTmp2.nRackNumber = nRackNumber
            oRackTmp2.nAutoFill = 0
            oRackTmp2.sRackLabels = String.Empty
            oRackTmp2.colSlots = New Collection
            If nInsert > 0 Then
                colRacks.Add(oRackTmp2, , nInsert)
            Else
                colRacks.Add(oRackTmp2, , , colRacks.Count)
            End If
        End If
        Return (nRackNumber)

    End Function

    Private Sub subAddSlot(ByRef oSlot As clsSlot)
        With oSlot
            Dim oRack As New clsRack
            Dim colSlots As Collection = Nothing
            For nRack As Integer = 1 To colRacks.Count
                oRack = colRacks(nRack)
                If oRack.nRackNumber = .nRackNumber Then
                    colSlots = oRack.colSlots
                End If
            Next
            Dim bFound As Boolean = False
            Dim nInsert As Integer = -1
            If colSlots IsNot Nothing AndAlso colSlots.Count > 0 Then
                For nIdx As Integer = 1 To colSlots.Count
                    Dim oSlot2 As clsSlot = DirectCast(colSlots(nIdx), clsSlot)
                    If oSlot2.nDataIndex = .nDataIndex Then
                        bFound = True
                        If .nDensity > 0 Then
                            oSlot2.nDensity = .nDensity
                        End If
                        If .nDisplayIndex > 0 Then
                            oSlot2.nDisplayIndex = .nDisplayIndex
                        End If
                        If .bPlcInput = True Then
                            oSlot2.bPlcInput = .bPlcInput
                        End If
                        If .bAnalog = True Then
                            oSlot2.bAnalog = .bAnalog
                        End If
                        Exit For
                    ElseIf oSlot2.nDataIndex > .nDataIndex Then
                        nInsert = nIdx
                        Exit For
                    End If
                Next
            End If
            If bFound = False Then
                If nInsert > 0 Then
                    colSlots.Add(oSlot, , nInsert)
                Else
                    colSlots.Add(oSlot, , , colSlots.Count)
                End If
            End If
        End With
    End Sub
    Private Sub subFillPointGrid(ByVal nSlot As Integer)
        nDisplaySlot = nSlot
        If colRacks(nDisplayRack).colSlots(nSlot).colPoints.Count = 0 Then
            dgPoints.RowCount = 1
            With dgPoints.Rows(0)
                .Cells.Item(0).Value = ""
                .Cells.Item(1).Value = 0
                .Cells.Item(2).Value = ""
                .Cells.Item(3).Value = 0
                .Cells.Item(4).Value = 0
                .Cells.Item(5).Value = ""
                .Cells.Item(6).Value = 0
                .Cells.Item(7).Value = ""
                .Cells.Item(8).Value = ""
            End With
        Else
            dgPoints.RowCount = colRacks(nDisplayRack).colSlots(nSlot).colPoints.Count
            For nPoint As Integer = 1 To colRacks(nDisplayRack).colSlots(nSlot).colPoints.Count
                With dgPoints.Rows(nPoint - 1)
                    .Cells.Item(0).Value = colRacks(nDisplayRack).colSlots(nSlot).colPoints(nPoint).sRack
                    .Cells.Item(1).Value = colRacks(nDisplayRack).colSlots(nSlot).colPoints(nPoint).nRackNumber
                    .Cells.Item(2).Value = colRacks(nDisplayRack).colSlots(nSlot).colPoints(nPoint).sSlot
                    .Cells.Item(3).Value = colRacks(nDisplayRack).colSlots(nSlot).colPoints(nPoint).nSlotNumber
                    .Cells.Item(4).Value = colRacks(nDisplayRack).colSlots(nSlot).colPoints(nPoint).nModuleNumber
                    .Cells.Item(5).Value = colRacks(nDisplayRack).colSlots(nSlot).colPoints(nPoint).sPoint
                    .Cells.Item(6).Value = colRacks(nDisplayRack).colSlots(nSlot).colPoints(nPoint).nPoint
                    .Cells.Item(7).Value = colRacks(nDisplayRack).colSlots(nSlot).colPoints(nPoint).sDescription
                    .Cells.Item(8).Value = colRacks(nDisplayRack).colSlots(nSlot).colPoints(nPoint).sLabel
                End With
            Next
        End If
    End Sub
    Private Sub subFillSlotGrid(ByVal nrack As Integer)
        nDisplayRack = nrack
        dgSlots.RowCount = colRacks(nrack).colSlots.Count
        For nSlot As Integer = 1 To colRacks(nrack).colSlots.Count
            With dgSlots.Rows(nSlot - 1)
                .Cells.Item(0).Value = colRacks(nrack).colSlots(nSlot).sRack
                .Cells.Item(1).Value = colRacks(nrack).colSlots(nSlot).nRackNumber
                .Cells.Item(2).Value = colRacks(nrack).colSlots(nSlot).sSlot
                .Cells.Item(3).Value = colRacks(nrack).colSlots(nSlot).nSlotNumber
                .Cells.Item(4).Value = colRacks(nrack).colSlots(nSlot).nModuleNumber
                .Cells.Item(5).Value = colRacks(nrack).colSlots(nSlot).sModuleType
                .Cells.Item(6).Value = colRacks(nrack).colSlots(nSlot).nDensity
                .Cells.Item(7).Value = colRacks(nrack).colSlots(nSlot).nDisplayIndex
                .Cells.Item(8).Value = colRacks(nrack).colSlots(nSlot).bView
                .Cells.Item(9).Value = colRacks(nrack).colSlots(nSlot).bPlcInput
                .Cells.Item(10).Value = colRacks(nrack).colSlots(nSlot).bAnalog
                .Cells.Item(11).Value = colRacks(nrack).colSlots(nSlot).nDataIndex
                .Cells.Item(12).Value = colRacks(nrack).colSlots(nSlot).nDataLength
                .Cells.Item(13).Value = colRacks(nrack).colSlots(nSlot).sURL
                .Cells.Item(14).Value = colRacks(nrack).colSlots(nSlot).bEnableToolTips
            End With
            Debug.Print(colRacks(nrack).colSlots(nSlot).colpoints.count.ToString)
        Next
    End Sub
    Private Sub btnOpen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpen.Click
        Dim sFile As String = String.Empty
        Try
            Dim o As New OpenFileDialog
            With o
                .Filter = "Logix Tag Export file |*.csv"
                .Title = "Open Logix Tag Export File"
                .AddExtension = True
                .CheckPathExists = True
                .DefaultExt = ".csv"
                If .ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                    sFile = .FileName
                End If
            End With

        Catch ex As Exception

        End Try
        If sFile = String.Empty Then
            Exit Sub
        End If
        colRacks.Clear()

        Dim sr As System.IO.StreamReader = Nothing
        Try
            sr = System.IO.File.OpenText(sFile)

            Do While sr.Peek() >= 0
                Try
                    Dim sLine As String = sr.ReadLine()
                    Dim sColumns() As String = Split(sLine, ",")
                    For nItem As Integer = 0 To sColumns.GetUpperBound(0)
                        sColumns(nItem) = sColumns(nItem).Replace("""", "")
                        sColumns(nItem) = sColumns(nItem).Replace("'", "")
                    Next
                    If sColumns.GetUpperBound(0) >= 5 Then
                        Dim sNAME() As String = Split(sColumns(2), ":")
                        Dim sDESCRIPTION As String = sColumns(3).Replace("$N", " ")
                        Dim sDATATYPE() As String = Split(sColumns(4), ":")
                        Dim sSPECIFIER() As String = Split(sColumns(5), ":")
                        Select Case sColumns(0)
                            Case "TAG", "ALIAS"
                                Dim sRack As String = String.Empty
                                Dim sSlot As String = String.Empty
                                Dim sTmp As String = String.Empty
                                Dim nSlots As Integer = -1
                                If sDATATYPE.GetUpperBound(0) > 2 AndAlso sDATATYPE(0) = "AB" Then
                                    'Some sort of IO device is identified here
                                    If sNAME.GetUpperBound(0) = 1 Then
                                        'May be a rack
                                        'TAG		SCC02:I	Chassis 02$NInputs	AB:1734_11SLOT:I:0		(Class := Standard)																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																									
                                        'TAG		ENET_RC2:I		AB:ETHERNET_MODULE_INT_56Bytes:I:0		(Class := Standard)	
                                        If sDATATYPE(1).Substring(0, 5) = "1734_" AndAlso _
                                           sDATATYPE(1).Substring(sDATATYPE(1).Length - 4) = "SLOT" Then
                                            'Number of slots in data type name - save as input slots for 1734 racks
                                            sTmp = sDATATYPE(1).Substring(5, sDATATYPE(1).Length - 9)
                                            If IsNumeric(sTmp) Then
                                                nSlots = CType(sTmp, Integer)
                                            End If
                                            nAddRack(sNAME(0), nSlots)
                                        ElseIf sDATATYPE(1).StartsWith("ETHERNET_MODULE_") Then
                                            '' Do something with the robot racks?
                                            'If sNAME(1) = "I" Then
                                            '    Dim sTmpSplit() As String = Split(sDATATYPE(1), "_")
                                            '    Select Case sTmpSplit(2)
                                            '        Case "INT"
                                            '            sTmp = sTmpSplit(3).Replace("Bytes", "")
                                            '            If IsNumeric(sTmp) Then
                                            '                nSlots = CType(sTmp, Integer) / 2
                                            '            End If
                                            '    End Select
                                            '    nAddRack(sColumns(2))
                                            'ElseIf sNAME(1) = "O" Then
                                            '    Dim sTmpSplit() As String = Split(sDATATYPE(1), "_")
                                            '    Select Case sTmpSplit(2)
                                            '        Case "INT"
                                            '            sTmp = sTmpSplit(3).Replace("Bytes", "")
                                            '            If IsNumeric(sTmp) Then
                                            '                nSlots = CType(sTmp, Integer) / 2
                                            '            End If
                                            '    End Select
                                            '    nAddRack(sColumns(2))
                                            'End If
                                            nAddRack(sColumns(2))
                                        End If
                                    ElseIf sNAME.GetUpperBound(0) = 2 Then
                                        'May be a slot
                                        'TAG		SCC02:9:O	Chassis 02$NSlot 9$NOutputs	AB:1734_IB8S:O:0		(Class := Safety)																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																									
                                        'TAG		Local:6:I		AB:1756_HSC:I:0		(Class := Standard)	
                                        Dim oSlot As New clsSlot
                                        With oSlot
                                            .sRack = sNAME(0)
                                            .nRackNumber = nAddRack(sNAME(0))
                                            .sSlot = sNAME(1)
                                            If IsNumeric(sNAME(1)) Then
                                                .nSlotNumber = CType(sNAME(1), Integer)
                                            Else
                                                .nSlotNumber = -1
                                            End If
                                            .nModuleNumber = 0
                                            .sModuleType = sDATATYPE(1)
                                            .nDensity = -1
                                            If .nRackNumber > 1 Then
                                                .nDisplayIndex = .nSlotNumber - 1
                                            Else
                                                .nDisplayIndex = .nSlotNumber
                                            End If
                                            .bView = True
                                            .bPlcInput = False
                                            .bAnalog = False
                                            .nDataIndex = 20 * (.nRackNumber - 1) + .nDisplayIndex
                                            .nDataLength = 1
                                            .sURL = "none"
                                            .bEnableToolTips = False
                                            .colPoints = New Collection
                                            If sDATATYPE(1).StartsWith("1734_IB8S") Then
                                                .nDensity = 16
                                                .bPlcInput = True
                                                .sModuleType = "1734-IB8S"
                                            ElseIf sDATATYPE(1).StartsWith("1734_OB8S") Then
                                                .nDensity = 12
                                                .bPlcInput = False
                                                .sModuleType = "1734-OB8S"
                                            ElseIf sDATATYPE(1).StartsWith("1734_DI8") Then
                                                .nDensity = 8
                                                .bPlcInput = True
                                                .sModuleType = "1734-IB8"
                                            ElseIf sDATATYPE(1).StartsWith("1734_DO8_NoDiag") Then
                                                .nDensity = 8
                                                .bPlcInput = False
                                                .sModuleType = "1734-OB8"
                                            ElseIf sDATATYPE(1).StartsWith("1756_HSC") Then
                                                .nDensity = 2
                                                .nDataLength = 2
                                                .bPlcInput = True
                                                .bAnalog = True
                                                .sModuleType = "1756-HSC"
                                                .nDataIndex = 0
                                                For nPt As Integer = 0 To 1
                                                    Dim oPoint As New clsPoint
                                                    oPoint.sRack = .sRack
                                                    oPoint.nRackNumber = .nRackNumber
                                                    oPoint.sSlot = .sSlot
                                                    oPoint.nSlotNumber = .nSlotNumber
                                                    oPoint.nModuleNumber = .nModuleNumber
                                                    oPoint.sPoint = "I.PresentValue[" & nPt.ToString & "]"
                                                    oPoint.nPoint = nPt
                                                    oPoint.sDescription = "Encoder " & (nPt + 1).ToString & " Current Counts"
                                                    oPoint.sLabel = .sRack & ":" & .sSlot & ":I.PresentValue[" & nPt.ToString & "]"
                                                    .colPoints.Add(oPoint)
                                                Next
                                            End If
                                            subAddSlot(oSlot)
                                        End With
                                    End If
                                End If
                            Case "COMMENT"
                                If sSPECIFIER.GetUpperBound(0) >= 2 Then
                                    Dim oPoint As New clsPoint
                                    With oPoint
                                        .sRack = sNAME(0)
                                        .nRackNumber = nAddRack(sNAME(0))
                                        .sSlot = sNAME(1)
                                        If IsNumeric(sNAME(1)) Then
                                            .nSlotNumber = CType(sNAME(1), Integer)
                                        Else
                                            .nSlotNumber = -1
                                        End If
                                        .nModuleNumber = 0
                                        .sPoint = sSPECIFIER(2)
                                        If IsNumeric(.sPoint) Then
                                            .nPoint = CType(.sPoint, Integer)
                                        ElseIf IsNumeric(.sPoint.Substring(2)) Then
                                            .nPoint = CType(.sPoint.Substring(2), Integer)
                                        ElseIf .sPoint.Substring(2, 2) = "PT" Then
                                            .nPoint = CType(.sPoint.Substring(4, 2), Integer)
                                        ElseIf .sPoint.Substring(2, 4) = "TEST" Then
                                            .nPoint = 12 + CType(.sPoint.Substring(6, 2), Integer)
                                        Else
                                            .nPoint = -1
                                        End If
                                        .sDescription = sDESCRIPTION
                                        .sLabel = sColumns(5)
                                        If .nPoint > -1 Then
                                            Dim oRack As clsRack
                                            Dim colSlots As Collection = Nothing
                                            For nRack As Integer = 1 To colRacks.Count
                                                oRack = colRacks(nRack)
                                                If oRack.nRackNumber = .nRackNumber Then
                                                    colSlots = oRack.colSlots
                                                End If
                                            Next
                                            Dim oSlot As clsSlot
                                            Dim colPoints As Collection = Nothing
                                            For nSlot As Integer = 1 To colSlots.Count
                                                oSlot = colSlots(nSlot)
                                                If oSlot.nSlotNumber = .nSlotNumber Then
                                                    colPoints = oSlot.colPoints
                                                End If
                                            Next
                                            colPoints.Add(oPoint)

                                        End If
                                    End With
                                End If
                            Case Else
                                'Nothing to do here for now
                        End Select

                    End If

                Catch ex As Exception

                End Try
            Loop

        Catch ex As Exception

        End Try
        dgRacks.ColumnCount = 0
        dgRacks.Columns.Add("Name", "Name")
        dgRacks.Columns.Add("Number", "Number")
        dgRacks.Columns.Add("Slots", "Slots")
        dgSlots.ColumnCount = 0
        dgSlots.Columns.Add("Rack", "Rack")
        dgSlots.Columns.Add("RackNumber", "RackNumber")
        dgSlots.Columns.Add("Slot", "Slot")
        dgSlots.Columns.Add("SlotNumber", "SlotNumber")
        dgSlots.Columns.Add("Module", "Module")
        dgSlots.Columns.Add("Type", "Type")
        dgSlots.Columns.Add("Density", "Density")
        dgSlots.Columns.Add("DisplayIndex", "DisplayIndex")
        dgSlots.Columns.Add("View", "View")
        dgSlots.Columns.Add("Input", "Input")
        dgSlots.Columns.Add("Analog", "Analog")
        dgSlots.Columns.Add("DataIndex", "DataIndex")
        dgSlots.Columns.Add("DataLength", "DataLength")
        dgSlots.Columns.Add("URL", "URL")
        dgSlots.Columns.Add("ToolTips", "ToolTips")
        dgPoints.ColumnCount = 0
        dgPoints.Columns.Add("Rack", "Rack")
        dgPoints.Columns.Add("RackNumber", "RackNumber")
        dgPoints.Columns.Add("Slot", "Slot")
        dgPoints.Columns.Add("SlotNumber", "SlotNumber")
        dgPoints.Columns.Add("Module", "Module")
        dgPoints.Columns.Add("Pointstr", "Pointstr")
        dgPoints.Columns.Add("Point", "Point")
        dgPoints.Columns.Add("Description", "Description")
        dgPoints.Columns.Add("Label", "Label")

        dgRacks.RowCount = colRacks.Count
        For nRack As Integer = 1 To colRacks.Count
            Dim oRack As clsRack = colRacks(nRack)
            Dim oSlot As clsSlot = Nothing
            'oRack.colSlots(nSlot)
            Dim nNeedSlot As Integer = 1
            If nRack = 1 Then
                nNeedSlot = 0
            End If
            Dim bDone As Boolean = False
            Dim nSlot As Integer = 1
            While bDone = False
                If oRack.colSlots Is Nothing Then
                    oRack.colSlots = New Collection
                    oSlot = New clsSlot
                    oSlot.nSlotNumber = 99
                ElseIf oRack.colSlots.Count = 0 Then
                    oSlot = New clsSlot
                    oSlot.nSlotNumber = 99
                ElseIf nSlot > oRack.colSlots.Count Then
                    bDone = True
                Else
                    oSlot = oRack.colSlots(nSlot)
                End If
                If oSlot.nSlotNumber > nNeedSlot Then
                    Dim oSlotNew As New clsSlot
                    With oSlotNew
                        .sRack = oRack.sRackName
                        .nRackNumber = oRack.nRackNumber
                        .sSlot = nNeedSlot.ToString
                        .nSlotNumber = nNeedSlot
                        .nModuleNumber = 0
                        .sModuleType = "Empty"
                        .nDensity = 0
                        If .nRackNumber > 1 Then
                            .nDisplayIndex = .nSlotNumber - 1
                        Else
                            .nDisplayIndex = .nSlotNumber
                        End If
                        .bView = True
                        .bPlcInput = False
                        .bAnalog = False
                        .nDataIndex = 20 * (.nRackNumber - 1) + .nDisplayIndex
                        .nDataLength = 0
                        .sURL = "none"
                        .bEnableToolTips = False
                        .colPoints = New Collection
                        If nRack = 1 Then
                            'Default settings for local rack
                            Select Case nNeedSlot
                                Case 0
                                    .sModuleType = "1756-L61S"
                                Case 1
                                    .sModuleType = "1756-LSP"
                                Case 2
                                    .sModuleType = "1756-ENBT"
                                Case 3
                                    .sModuleType = "1756-ENBT"
                                Case 4
                                    .sModuleType = "Empty" ' "1756-DH/RIO"
                                Case 5
                                    .sModuleType = "Empty"
                            End Select
                        End If
                    End With
                    oRack.colSlots.Add(oSlotNew, , nSlot)
                    nNeedSlot = nNeedSlot + 1
                    nSlot = nSlot + 1
                Else
                    nNeedSlot = nNeedSlot + 1
                    nSlot = nSlot + 1
                End If
            End While
            oRack.nSlots = oRack.colSlots.Count
            With dgRacks.Rows(nRack - 1)
                .Cells.Item(0).Value = oRack.sRackName
                .Cells.Item(1).Value = oRack.nRackNumber
                .Cells.Item(2).Value = oRack.nSlots
            End With
            For Each oSlotTmp As clsSlot In oRack.colSlots
                'For known configs, make sure the points are set up right
                Dim oCol As Collection = oSlotTmp.colPoints
                Dim sFormat As String = String.Empty
                Dim nNumPoints As Integer = 0
                Select Case oSlotTmp.sModuleType
                    Case "1734-IB8S"
                        sFormat = "I.PT0#DATA"
                        nNumPoints = 8
                    Case "1734-OB8S"
                        sFormat = "O.PT0#DATA"
                        nNumPoints = 8
                    Case "1734-IB8"
                        sFormat = "I.#"
                        nNumPoints = 8
                    Case "1734-OB8"
                        sFormat = "O.#"
                        nNumPoints = 8
                End Select
                Dim nPtPtr As Integer = 1
                For nPt As Integer = 0 To nNumPoints - 1
                    Dim bAdd As Boolean = False
                    If oCol Is Nothing Then
                        oCol = New Collection
                    End If
                    If oCol.Count = 0 Then
                        bAdd = True
                    Else
                        Dim oPt As clsPoint = oCol(nPtPtr)
                        While oPt.nPoint < nPt And nPtPtr < oCol.Count
                            nPtPtr = nPtPtr + 1
                            oPt = oCol(nPtPtr)
                        End While
                        If oPt.nPoint <> nPt Then
                            bAdd = True
                            nPtPtr = nPtPtr + 1
                        End If
                    End If
                    If bAdd Then
                        Dim oNewPoint As New clsPoint
                        oNewPoint.sRack = oRack.sRackName
                        oNewPoint.nRackNumber = oRack.nRackNumber
                        oNewPoint.sSlot = oSlotTmp.sSlot
                        oNewPoint.nSlotNumber = oSlotTmp.nSlotNumber
                        oNewPoint.nModuleNumber = oSlotTmp.nModuleNumber
                        oNewPoint.sPoint = String.Format(sFormat, nPt)
                        oNewPoint.nPoint = nPt
                        oNewPoint.sDescription = "Spare"
                        oNewPoint.sLabel = oRack.sRackName & ":" & oSlotTmp.sSlot & ":" & String.Format(sFormat, nPt)
                        If nPtPtr > oCol.Count Then
                            oCol.Add(oNewPoint)
                        Else
                            oCol.Add(oNewPoint, , nPtPtr)
                        End If
                    End If
                Next 'End standard points
                'Add Status Points
                Select Case oSlotTmp.sModuleType
                    Case "1734-IB8S", "1734-OB8S"
                        Dim oNewPoint As New clsPoint
                        oNewPoint.sRack = oRack.sRackName
                        oNewPoint.nRackNumber = oRack.nRackNumber
                        oNewPoint.sSlot = oSlotTmp.sSlot
                        oNewPoint.nSlotNumber = oSlotTmp.nSlotNumber
                        oNewPoint.nModuleNumber = oSlotTmp.nModuleNumber
                        oNewPoint.nPoint = 8
                        oNewPoint.sDescription = "RunMode"
                        oNewPoint.sPoint = "I." & oNewPoint.sDescription
                        oNewPoint.sLabel = oRack.sRackName & ":" & oSlotTmp.sSlot & ":" & oNewPoint.sPoint
                        If oCol.Count < 9 Then
                            oCol.Add(oNewPoint)
                        Else
                            oCol.Add(oNewPoint, , 9)
                        End If
                        oNewPoint = New clsPoint
                        oNewPoint.sRack = oRack.sRackName
                        oNewPoint.nRackNumber = oRack.nRackNumber
                        oNewPoint.sSlot = oSlotTmp.sSlot
                        oNewPoint.nSlotNumber = oSlotTmp.nSlotNumber
                        oNewPoint.nModuleNumber = oSlotTmp.nModuleNumber
                        oNewPoint.nPoint = 9
                        oNewPoint.sDescription = "ConnectionFaulted"
                        oNewPoint.sPoint = "I." & oNewPoint.sDescription
                        oNewPoint.sLabel = oRack.sRackName & ":" & oSlotTmp.sSlot & ":" & oNewPoint.sPoint
                        If oCol.Count < 10 Then
                            oCol.Add(oNewPoint)
                        Else
                            oCol.Add(oNewPoint, , 10)
                        End If
                        oNewPoint = New clsPoint
                        oNewPoint.sRack = oRack.sRackName
                        oNewPoint.nRackNumber = oRack.nRackNumber
                        oNewPoint.sSlot = oSlotTmp.sSlot
                        oNewPoint.nSlotNumber = oSlotTmp.nSlotNumber
                        oNewPoint.nModuleNumber = oSlotTmp.nModuleNumber
                        If oSlotTmp.bPlcInput Then
                            oNewPoint.sDescription = "InputPowerStatus"
                        Else
                            oNewPoint.sDescription = "OutputPowerStatus"
                        End If
                        oNewPoint.nPoint = 10
                        oNewPoint.sPoint = "I." & oNewPoint.sDescription
                        oNewPoint.sLabel = oRack.sRackName & ":" & oSlotTmp.sSlot & ":" & oNewPoint.sPoint
                        If oCol.Count < 11 Then
                            oCol.Add(oNewPoint)
                        Else
                            oCol.Add(oNewPoint, , 11)
                        End If
                        oNewPoint = New clsPoint
                        oNewPoint.sRack = oRack.sRackName
                        oNewPoint.nRackNumber = oRack.nRackNumber
                        oNewPoint.sSlot = oSlotTmp.sSlot
                        oNewPoint.nSlotNumber = oSlotTmp.nSlotNumber
                        oNewPoint.nModuleNumber = oSlotTmp.nModuleNumber
                        If oSlotTmp.bPlcInput Then
                            oNewPoint.sDescription = "CombinedInputStatus"
                        Else
                            oNewPoint.sDescription = "CombinedOutputStatus"
                        End If
                        oNewPoint.nPoint = 11
                        oNewPoint.sPoint = "I." & oNewPoint.sDescription
                        oNewPoint.sLabel = oRack.sRackName & ":" & oSlotTmp.sSlot & ":" & oNewPoint.sPoint
                        If oCol.Count < 12 Then
                            oCol.Add(oNewPoint)
                        Else
                            oCol.Add(oNewPoint, , 12)
                        End If
                    Case Else
                End Select
            Next
        Next
    End Sub

    Private Sub dgRacks_CellMouseEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgRacks.CellMouseEnter
        If e.RowIndex >= 0 Then
            nRackRowEntered = e.RowIndex + 1
        End If
    End Sub

    Private Sub dgRacks_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles dgRacks.MouseClick
        subFillSlotGrid(nRackRowEntered)
    End Sub
    Private Sub dgSlots_CellMouseEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgSlots.CellMouseEnter
        If e.RowIndex >= 0 Then
            nSlotRowEntered = e.RowIndex + 1
        End If
    End Sub

    Private Sub dgSlots_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles dgSlots.MouseClick
        subFillPointGrid(nSlotRowEntered)
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Dim sFile As String = String.Empty
        Try
            Dim o As New SaveFileDialog
            With o
                .FileName = "IO Labels.csv"
                .Filter = "PLCIO Res file cut and paste |*.csv"
                .Title = "Select a CSV to write labels that can be copied into the PLC IO VB Project"
                .AddExtension = True
                .CheckPathExists = True
                .DefaultExt = ".csv"
                If .ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                    sFile = .FileName
                End If
            End With

        Catch ex As Exception

        End Try
        If sFile = String.Empty Then
            Exit Sub
        End If
        Dim sFileResx As String = String.Empty
        Try
            Dim o As New SaveFileDialog
            With o
                .FileName = "IOStrings.resx"
                .Filter = "IO Labels Resourcec file |*.resx"
                .Title = "Write the IO labels to the VB resource file"
                .AddExtension = True
                .CheckPathExists = True
                .DefaultExt = ".resx"
                If .ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                    sFileResx = .FileName
                End If
            End With

        Catch ex As Exception

        End Try
        If sFileResx = String.Empty Then
            Exit Sub
        End If

        Dim fileWriter As System.IO.StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(sFile, False)
        fileWriter.WriteLine("lsALIAS,Address:")
        fileWriter.WriteLine("lsDELIMITER, - ")
        fileWriter.WriteLine("lsEXPAND,<Click to Expand>")
        fileWriter.WriteLine("lsMODULE,Section:")
        fileWriter.WriteLine("lsRACK,Rack")
        fileWriter.WriteLine("lsSLOT,Module:")
        fileWriter.WriteLine("lsTAG_name,Section:")

        Dim fileWriterResx As System.IO.StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(sFileResx, False)
        fileWriterResx.WriteLine(lblResX1.Text)

        'Go through the racks
        Dim nNumRobots As Integer = 0
        For Each oRack As clsRack In colRacks
            'Write rack labels
            Dim sRackName As String = oRack.sRackName
            If oRack.nRackNumber = 1 Then
                sRackName = "SCC Logix Chassis (" & oRack.sRackName & ")"
            ElseIf sRackName.Substring(0, 3) = "SCC" Then
                sRackName = "SCC Rack " & oRack.nRackNumber.ToString & " (" & oRack.sRackName & ")"
            ElseIf sRackName.Substring(0, 2) = "JB" Then
                sRackName = "J-Box Rack " & oRack.nRackNumber.ToString & " (" & oRack.sRackName & ")"
            ElseIf sRackName.Substring(0, 7) = "ENET_RC" Then
                Dim sTmp() As String = Split(sRackName, ":")
                Dim nRobot As Integer = sTmp(0).Substring(7)
                If nRobot > nNumRobots Then
                    nNumRobots = nRobot
                End If
                Select Case sTmp(1)
                    Case "I"
                        sRackName = "Controller " & nRobot.ToString & " Inputs (From PLC)"
                    Case "O"
                        sRackName = "Controller " & nRobot.ToString & " Outputs (To PLC)"
                End Select
            End If
            Dim sRackNumber As String = oRack.nRackNumber.ToString
            fileWriter.WriteLine("lsR" & sRackNumber & "_TT," & sRackName)
            fileWriter.WriteLine("lsRACK" & sRackNumber & "," & sRackName)
            fileWriterResx.WriteLine("<data name=""lsR" & sRackNumber & "_TT"" xml:space=""preserve"">")
            fileWriterResx.WriteLine("<value>" & sRackName & "</value>")
            fileWriterResx.WriteLine("</data>")
            fileWriterResx.WriteLine("<data name=""lsRACK" & sRackNumber & """ xml:space=""preserve"">")
            fileWriterResx.WriteLine("<value>" & sRackName & "</value>")
            fileWriterResx.WriteLine("</data>")
            If sRackNumber.Length = 1 Then
                fileWriter.WriteLine("lsR0" & sRackNumber & "_TT," & sRackName)
                fileWriter.WriteLine("lsRACK0" & sRackNumber & "," & sRackName)
                fileWriterResx.WriteLine("<data name=""lsR0" & sRackNumber & "_TT"" xml:space=""preserve"">")
                fileWriterResx.WriteLine("<value>" & sRackName & "</value>")
                fileWriterResx.WriteLine("</data>")
                fileWriterResx.WriteLine("<data name=""lsRACK0" & sRackNumber & """ xml:space=""preserve"">")
                fileWriterResx.WriteLine("<value>" & sRackName & "</value>")
                fileWriterResx.WriteLine("</data>")
            End If
            'Go through each slot
            For Each oSlot As clsSlot In oRack.colSlots
                'No slot -specific labels, just scroll through the points
                Dim sSlotNumber As String = oSlot.nSlotNumber.ToString
                If sSlotNumber.Length = 1 Then
                    sSlotNumber = "0" & sSlotNumber
                End If
                If oSlot.colPoints.Count > 0 Then
                    For Each oPoint As clsPoint In oSlot.colPoints
                        Dim sPointNumber As String = oPoint.nPoint.ToString
                        If sPointNumber.Length = 1 Then
                            sPointNumber = "0" & sPointNumber
                        End If
                        Dim sTag As String = "lsR" & sRackNumber & "S" & sSlotNumber & "M0_" & sPointNumber
                        Dim sValue As String = oPoint.sLabel & " - " & oPoint.sDescription
                        sValue = sValue.Replace("<", "&lt;")
                        sValue = sValue.Replace(">", "&gt;")
                        sValue = sValue.Replace("&", "&amp;")
                        sValue = sValue.Replace("""", "&quot;")
                        fileWriter.WriteLine(sTag & "," & sValue)
                        fileWriterResx.WriteLine("<data name=""" & sTag & """ xml:space=""preserve"">")
                        fileWriterResx.WriteLine("<value>" & sValue & "</value>")
                        fileWriterResx.WriteLine("</data>")
                    Next 'Point
                End If
            Next 'Slot
        Next ' rack
        fileWriter.WriteLine(lblRobotIO.Text)
        fileWriterResx.WriteLine(lblResx2.Text)
        fileWriter.Close()
        fileWriterResx.Close()

        'Assume Robots start at data index 100
        Dim nRobotStartData As Integer = 100
        'Robots get 32 words each on inputs and outputs
        Dim nRobotLinkLength As Integer = 32
        Dim nPLCLinkLength As Integer = (nNumRobots * nRobotLinkLength * 2) + nRobotStartData
        Dim nRobot1RackNumber As Integer = 50
        'PLC_IO.XML
        sFile = String.Empty
        Try
            Dim o As New SaveFileDialog
            With o
                .FileName = "PLC_IO.XML"
                .Filter = "PLC_IO.XML|*.XML"
                .Title = "Select a file to save PLC_IO.XML for the database folder"
                .AddExtension = True
                .CheckPathExists = True
                .DefaultExt = ".XML"
                If .ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                    sFile = .FileName
                End If
            End With

        Catch ex As Exception

        End Try
        If sFile = String.Empty Then
            Exit Sub
        End If

        fileWriter = My.Computer.FileSystem.OpenTextFileWriter(sFile, False)
        fileWriter.WriteLine("<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>")
        fileWriter.WriteLine("<PLC_IO xmlns=""http://tempuri.org/PLC_IO.xsd"">")
        fileWriter.WriteLine("  <Config>")
        fileWriter.WriteLine("    <DataLength>" & nPLCLinkLength.ToString & "</DataLength>")
        fileWriter.WriteLine("    <HorizontalMargin>10</HorizontalMargin>")
        fileWriter.WriteLine("    <VerticalMargin>10</VerticalMargin>")
        fileWriter.WriteLine("    <InputTitleBackColor>blue</InputTitleBackColor>")
        fileWriter.WriteLine("    <OutputTitleBackColor>green</OutputTitleBackColor>")
        fileWriter.WriteLine("    <AnalogTitleBackColor>orange</AnalogTitleBackColor>")
        fileWriter.WriteLine("    <OtherTitleBackColor>violet</OtherTitleBackColor>")
        fileWriter.WriteLine("  </Config>")

        For Each oRack As clsRack In colRacks
            fileWriter.WriteLine("  <Racks>")
            fileWriter.WriteLine("    <RackNumber>" & oRack.nRackNumber & "</RackNumber>")
            fileWriter.WriteLine("    <View>1</View>")
            fileWriter.WriteLine("  </Racks>")
            If oRack.nRackNumber < nRobot1RackNumber Then
                'PLC Rack
                For Each oslot As clsSlot In oRack.colSlots
                    fileWriter.WriteLine("  <Modules>")
                    fileWriter.WriteLine("    <RackNumber>" & oslot.nRackNumber & "</RackNumber>")
                    fileWriter.WriteLine("    <SlotNumber>" & oslot.nSlotNumber & "</SlotNumber>")
                    fileWriter.WriteLine("    <ModuleNumber>" & oslot.nModuleNumber & "</ModuleNumber>")
                    fileWriter.WriteLine("    <ModuleType>" & oslot.sModuleType & "</ModuleType>")
                    fileWriter.WriteLine("    <Density>" & oslot.nDensity & "</Density>")
                    fileWriter.WriteLine("    <DisplayIndex>" & oslot.nDisplayIndex & "</DisplayIndex>")
                    fileWriter.WriteLine("    <View>true</View>")
                    fileWriter.WriteLine("    <PlcInput>" & oslot.bPlcInput.ToString.ToLower & "</PlcInput>")
                    fileWriter.WriteLine("    <Analog>" & oslot.bAnalog.ToString.ToLower & "</Analog>")
                    fileWriter.WriteLine("    <DataIndex>" & oslot.nDataIndex & "</DataIndex>")
                    fileWriter.WriteLine("    <DataLength>" & oslot.nDataLength & "</DataLength>")
                    fileWriter.WriteLine("    <URL>" & oslot.sURL & "</URL>")
                    fileWriter.WriteLine("    <EnableToolTips>false</EnableToolTips>")
                    fileWriter.WriteLine("    <AutoFill>0</AutoFill>")
                    fileWriter.WriteLine("    <RackLabels></RackLabels>")
                    fileWriter.WriteLine("  </Modules>")
                Next
            Else
                'Robot
                fileWriter.WriteLine("  <Modules>")
                fileWriter.WriteLine("    <RackNumber>" & oRack.nRackNumber & "</RackNumber>")
                fileWriter.WriteLine("    <SlotNumber>0</SlotNumber>")
                fileWriter.WriteLine("    <ModuleNumber>0</ModuleNumber>")
                fileWriter.WriteLine("    <ModuleType>" & oRack.sRackName & "</ModuleType>")
                fileWriter.WriteLine("    <Density>16</Density>")
                fileWriter.WriteLine("    <DisplayIndex>0</DisplayIndex>")
                fileWriter.WriteLine("    <View>true</View>")
                Dim sTmp() As String = Split(oRack.sRackName, ":")
                If sTmp(1) = "I" Then
                    fileWriter.WriteLine("    <PlcInput>true</PlcInput>")
                Else
                    fileWriter.WriteLine("    <PlcInput>false</PlcInput>")
                End If
                fileWriter.WriteLine("    <Analog>false</Analog>")
                fileWriter.WriteLine("    <DataIndex>" & ((oRack.nRackNumber - nRobot1RackNumber) * nRobotLinkLength) + nRobotStartData & "</DataIndex>")
                fileWriter.WriteLine("    <DataLength>1</DataLength>")
                fileWriter.WriteLine("    <URL>none</URL>")
                fileWriter.WriteLine("    <EnableToolTips>false</EnableToolTips>")
                fileWriter.WriteLine("    <AutoFill>27</AutoFill>")
                fileWriter.WriteLine("    <RackLabels>" & (((oRack.nRackNumber - nRobot1RackNumber) Mod 2) + nRobot1RackNumber).ToString & "</RackLabels>")
                fileWriter.WriteLine("  </Modules>")
            End If
        Next
        fileWriter.WriteLine("</PLC_IO>")
        fileWriter.Close()

        'PLC_Logic
        sFile = String.Empty
        Try
            Dim o As New SaveFileDialog
            With o
                .FileName = "IO Mapping Logic.txt"
                .Filter = "IO Mapping Logic.txt|*.Txt"
                .Title = "Select a file to save IO Mapping Logic"
                .AddExtension = True
                .CheckPathExists = True
                .DefaultExt = ".Txt"
                If .ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                    sFile = .FileName
                End If
            End With

        Catch ex As Exception

        End Try
        If sFile = String.Empty Then
            Exit Sub
        End If

        fileWriter = My.Computer.FileSystem.OpenTextFileWriter(sFile, False)
        fileWriter.WriteLine("GUIMappingOut:")
        fileWriter.Write("BST XIC u_Zone.InitOS FLL 0 gd_Z1GUIIOMonitor[0] 64 ")

        For Each oRack As clsRack In colRacks
            If oRack.nRackNumber < nRobot1RackNumber Then
                'PLC Rack
                For Each oslot As clsSlot In oRack.colSlots
                    If oslot.nDensity > 0 Then
                        Dim sTmp As String = oRack.sRackName & ":" & oslot.sSlot & ":"
                        Select Case oslot.sModuleType
                            Case "1734-IB8S"
                                fileWriter.Write("NXB JSR Z008_Map_IB8S 2 " & sTmp & "I " & sTmp & "O gd_Z1GUIIOMonitor[" & oslot.nDataIndex.ToString & "] ")
                            Case "1734-OB8S"
                                fileWriter.Write("NXB JSR Z009_Map_OB8S 2 " & sTmp & "I " & sTmp & "O gd_Z1GUIIOMonitor[" & oslot.nDataIndex.ToString & "] ")
                            Case "1734-IB8"
                                fileWriter.Write("NXB MOV " & sTmp & "I gd_Z1GUIIOMonitor[" & oslot.nDataIndex.ToString & "] ")
                            Case "1734-OB8"
                                fileWriter.Write("NXB MOV " & sTmp & "O gd_Z1GUIIOMonitor[" & oslot.nDataIndex.ToString & "] ")
                            Case "1756-HSC"
                                sTmp = sTmp & "I.PresentValue["
                                Dim sTmp1 As String = oslot.nDataIndex.ToString
                                Dim sTmp2 As String = (oslot.nDataIndex + 1).ToString
                                fileWriter.Write("NXB MOV " & sTmp & "0] gd_Z1GUIIOMonitor[" & sTmp1 & "] NXB MOV " & sTmp & "1] gd_Z1GUIIOMonitor[" & sTmp2 & "] ")

                        End Select
                    End If
                Next
            End If
        Next
        fileWriter.WriteLine("")
        fileWriter.WriteLine("ModuleStatus (earlier version):")
        For Each oRack As clsRack In colRacks
            If oRack.nRackNumber < nRobot1RackNumber Then
                'PLC Rack
                For Each oslot As clsSlot In oRack.colSlots
                    If oslot.nDensity > 0 Then
                        Dim sTmp As String = oRack.sRackName & ":" & oslot.sSlot & ":"
                        Select Case oslot.sModuleType
                            Case "1734-IB8S"
                                fileWriter.WriteLine("BST XIC " & sTmp & "I.ConnectionFaulted NXB XIC " & sTmp & "I.InputPowerStatus NXB XIO " & sTmp & "I.CombinedInputStatus BND OTE SZ_ENetFault.Chassis" & oRack.nRackNumber.ToString("00") & "SlotFault[" & oslot.nSlotNumber.ToString & "] ")
                            Case "1734-OB8S"
                                fileWriter.WriteLine("BST XIC " & sTmp & "I.ConnectionFaulted NXB XIC " & sTmp & "I.OutputPowerStatus NXB XIO " & sTmp & "I.CombinedOutputStatus BND OTE SZ_ENetFault.Chassis" & oRack.nRackNumber.ToString("00") & "SlotFault[" & oslot.nSlotNumber.ToString & "] ")
                        End Select
                    End If
                Next
            End If
        Next
        'New version
        fileWriter.WriteLine("")
        fileWriter.WriteLine("ModuleStatus (newer version):")
        'BST NEQ SZ_ENetFault.Rack[2].ConnectionFaulted 0 NXB NEQ SZ_ENetFault.Rack[3].ConnectionFaulted 0 NXB NEQ SZ_ENetFault.Rack[2].NotInRunMode 0 NXB NEQ SZ_ENetFault.Rack[3].NotInRunMode 0 BND OTL SZ_ENetFault.Connection 
        Dim sConnStat As String = "BST "
        'BST NEQ SZ_ENetFault.Rack[2].ConnectionFaulted 0 NXB NEQ SZ_ENetFault.Rack[3].ConnectionFaulted 0 BND OTL SZ_ENetFault.Input 
        Dim sInStat As String = "BST "
        'BST XIC SZ_ENetFault.ModuleOutputFault[81] NXB XIC SZ_ENetFault.ModuleOutputFault[82] NXB XIC SZ_ENetFault.ModuleOutputFault[83] BND OTL SZ_ENetFault.Output 
        Dim sOutStat As String = "BST "
        For Each oRack As clsRack In colRacks
            If oRack.nRackNumber < nRobot1RackNumber Then
                'PLC Rack
                Dim sRack As String = oRack.nRackNumber.ToString
                sConnStat = sConnStat & "NEQ SZ_ENetFault.Rack[" & sRack & "].ConnectionFaulted 0 NXB NEQ SZ_ENetFault.Rack[" & sRack & "].NotInRunMode 0 NXB "
                For Each oslot As clsSlot In oRack.colSlots
                    If oslot.nDensity > 0 Then
                        Dim sModule As String = oRack.sRackName & ":" & oslot.sSlot & ":"
                        Select Case oslot.sModuleType
                            Case "1734-IB8S"
                                fileWriter.Write("BST BST XIO " & sModule & "I.RunMode NXB XIC SZ_ENetFault.Rack[" & sRack & "].NotInRunMode." & oslot.sSlot & " XIO b_FaultReset BND OTE SZ_ENetFault.Rack[" & sRack & "].NotInRunMode." & oslot.sSlot)
                                fileWriter.Write(" NXB BST XIC " & sModule & "I.ConnectionFaulted NXB XIC SZ_ENetFault.Rack[" & sRack & "].ConnectionFaulted." & oslot.sSlot & " XIO b_FaultReset BND OTE SZ_ENetFault.Rack[" & sRack & "].ConnectionFaulted." & oslot.sSlot)
                                fileWriter.Write(" NXB BST XIC " & sModule & "I.InputPowerStatus NXB XIC SZ_ENetFault.Rack[" & sRack & "].IOPowerStatusFaulted." & oslot.sSlot & " XIO b_FaultReset BND OTE SZ_ENetFault.Rack[" & sRack & "].IOPowerStatusFaulted." & oslot.sSlot)
                                fileWriter.WriteLine(" NXB BST XIO " & sModule & "I.CombinedInputStatus NXB XIC SZ_ENetFault.Rack[" & sRack & "].CombinedStatusFaulted." & oslot.sSlot & " XIO b_FaultReset BND OTE SZ_ENetFault.Rack[" & sRack & "].CombinedStatusFaulted." & oslot.sSlot & "  BND OTE SZ_ENetFault.Rack[" & sRack & "].FaultSummary." & oslot.sSlot)
                                sInStat = sInStat & "XIC SZ_ENetFault.Rack[" & sRack & "].CombinedStatusFaulted." & oslot.sSlot & " NXB XIC SZ_ENetFault.Rack[" & sRack & "].IOPowerStatusFaulted." & oslot.sSlot & " NXB "
                            Case "1734-OB8S"
                                fileWriter.Write("BST BST XIO " & sModule & "I.RunMode NXB XIC SZ_ENetFault.Rack[" & sRack & "].NotInRunMode." & oslot.sSlot & " XIO b_FaultReset BND OTE SZ_ENetFault.Rack[" & sRack & "].NotInRunMode." & oslot.sSlot)
                                fileWriter.Write(" NXB BST XIC " & sModule & "I.ConnectionFaulted NXB XIC SZ_ENetFault.Rack[" & sRack & "].ConnectionFaulted." & oslot.sSlot & " XIO b_FaultReset BND OTE SZ_ENetFault.Rack[" & sRack & "].ConnectionFaulted." & oslot.sSlot)
                                fileWriter.Write(" NXB BST XIC " & sModule & "I.OutputPowerStatus NXB XIC SZ_ENetFault.Rack[" & sRack & "].IOPowerStatusFaulted." & oslot.sSlot & " XIO b_FaultReset BND OTE SZ_ENetFault.Rack[" & sRack & "].IOPowerStatusFaulted." & oslot.sSlot)
                                fileWriter.WriteLine(" NXB BST XIO " & sModule & "I.CombinedOutputStatus NXB XIC SZ_ENetFault.Rack[" & sRack & "].CombinedStatusFaulted." & oslot.sSlot & " XIO b_FaultReset BND OTE SZ_ENetFault.Rack[" & sRack & "].CombinedStatusFaulted." & oslot.sSlot & "  BND OTE SZ_ENetFault.Rack[" & sRack & "].FaultSummary." & oslot.sSlot)
                                sOutStat = sOutStat & "XIC SZ_ENetFault.Rack[" & sRack & "].CombinedStatusFaulted." & oslot.sSlot & " NXB XIC SZ_ENetFault.Rack[" & sRack & "].IOPowerStatusFaulted." & oslot.sSlot & " NXB "
                        End Select
                    End If
                Next
            End If
        Next
        fileWriter.WriteLine(sConnStat.Substring(0, sConnStat.Length - 4) & " BND OTE SZ_ENetFault.Connection")
        fileWriter.WriteLine(sInStat.Substring(0, sInStat.Length - 4) & " BND OTE SZ_ENetFault.Input")
        fileWriter.WriteLine(sOutStat.Substring(0, sOutStat.Length - 4) & " BND OTE SZ_ENetFault.Output")


        'Alarms
        fileWriter.WriteLine("")
        fileWriter.WriteLine("Alarms (older version):")
        Dim nStartWord As Integer = 0
        Dim nStartBit As Integer = 0
        Dim nEndWord As Integer = 0
        Dim nEndBit As Integer = 0
        Dim nWord As Integer = 0
        Dim nBit As Integer = 0
        For Each oRack As clsRack In colRacks
            If (oRack.nRackNumber > 1) And (oRack.nRackNumber < nRobot1RackNumber) Then
                'PLC Rack
                Dim sRack As String = oRack.nRackNumber.ToString
                Dim bFirst As Boolean = True
                fileWriter.Write("BST ")
                Select Case oRack.nRackNumber
                    Case 2
                        nStartWord = 15
                        nStartBit = 3
                        nEndWord = 16
                        nEndBit = 9
                    Case 3
                        nStartWord = 16
                        nStartBit = 9
                        nEndWord = 17
                        nEndBit = 6
                    Case 4
                        nStartWord = 17
                        nStartBit = 6
                        nEndWord = 17
                        nEndBit = 12
                    Case Else
                End Select
                For Each oslot As clsSlot In oRack.colSlots
                    nBit = nStartBit + oslot.nSlotNumber
                    If nBit > 31 Then
                        nWord = nStartWord + 2
                        nBit = nBit - 32
                    ElseIf nBit > 15 Then
                        nWord = nStartWord + 1
                        nBit = nBit - 16
                    Else
                        nWord = nStartWord
                    End If
                    If bFirst Then
                        bFirst = False
                    Else
                        fileWriter.Write(" NXB ")
                    End If
                    Dim sAlarmTag As String = "d_AlarmActive[" & nWord.ToString & "]." & nBit.ToString & " "
                    Select Case oslot.sModuleType
                        Case "1734-IB8S", "1734-OB8S"

                            Dim sTmp As String = oRack.sRackName & ":" & oslot.sSlot & ":"
                            fileWriter.Write("BST XIC " & sTmp & "I.ConnectionFaulted NXB XIC " & sAlarmTag & " XIO b_FaultResetPBOS BND OTE " & sAlarmTag)
                        Case Else
                            fileWriter.Write(" OTU " & sAlarmTag)
                    End Select
                Next
                Do While nWord < nEndWord Or nBit < nEndBit
                    nBit = nBit + 1
                    If nBit > 15 Then
                        nWord = nWord + 1
                        nBit = nBit - 16
                    End If
                    Dim sAlarmTag As String = "d_AlarmActive[" & nWord.ToString & "]." & nBit.ToString & " "
                    fileWriter.Write("NXB OTU " & sAlarmTag)
                Loop
                fileWriter.WriteLine("BND ")
            End If
        Next


        'Alarms
        fileWriter.WriteLine("")
        fileWriter.WriteLine("Alarms (newer version):")
        For Each oRack As clsRack In colRacks
            If (oRack.nRackNumber > 1) And (oRack.nRackNumber < nRobot1RackNumber) Then
                'PLC Rack
                Dim sRack As String = oRack.nRackNumber.ToString
                Dim bFirst As Boolean = True
                fileWriter.Write("BST ")
                Select Case oRack.nRackNumber
                    Case 2
                        nStartWord = 15
                        nStartBit = 3
                        nEndWord = 16
                        nEndBit = 9
                    Case 3
                        nStartWord = 16
                        nStartBit = 9
                        nEndWord = 17
                        nEndBit = 6
                    Case 4
                        nStartWord = 17
                        nStartBit = 6
                        nEndWord = 17
                        nEndBit = 12
                    Case Else
                End Select
                For Each oslot As clsSlot In oRack.colSlots
                    nBit = nStartBit + oslot.nSlotNumber
                    If nBit > 31 Then
                        nWord = nStartWord + 2
                        nBit = nBit - 32
                    ElseIf nBit > 15 Then
                        nWord = nStartWord + 1
                        nBit = nBit - 16
                    Else
                        nWord = nStartWord
                    End If
                    If bFirst Then
                        bFirst = False
                    Else
                        fileWriter.Write(" NXB ")
                    End If
                    Dim sAlarmTag As String = "d_AlarmActive[" & nWord.ToString & "]." & nBit.ToString & " "
                    Select Case oslot.sModuleType
                        Case "1734-IB8S", "1734-OB8S"
                            fileWriter.Write("BST XIC SZ_ENetFault.Rack[" & sRack & "].NotInRunMode." & oslot.sSlot & " NXB XIC SZ_ENetFault.Rack[" & sRack & "].ConnectionFaulted." & oslot.sSlot & " BND OTE " & sAlarmTag)
                        Case Else
                            fileWriter.Write(" OTU " & sAlarmTag)
                    End Select
                Next
                Do While nWord < nEndWord Or nBit < nEndBit
                    nBit = nBit + 1
                    If nBit > 15 Then
                        nWord = nWord + 1
                        nBit = nBit - 16
                    End If
                    Dim sAlarmTag As String = "d_AlarmActive[" & nWord.ToString & "]." & nBit.ToString & " "
                    fileWriter.Write("NXB OTU " & sAlarmTag)
                Loop
                fileWriter.WriteLine("BND ")
            End If
        Next


        fileWriter.Close()
    End Sub

    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        tlpMain.Dock = DockStyle.Fill
    End Sub
End Class
Class clsPoint
    Public sRack As String
    Public nRackNumber As Integer
    Public sSlot As String
    Public nSlotNumber As Integer
    Public nModuleNumber As Integer
    Public sPoint As String
    Public nPoint As Integer
    Public sDescription As String
    Public sLabel As String
End Class
Class clsSlot
    Public sRack As String
    Public nRackNumber As Integer
    Public sSlot As String
    Public nSlotNumber As Integer
    Public nModuleNumber As Integer
    Public sModuleType As String
    Public nDensity As Integer
    Public nDisplayIndex As Integer
    Public bView As Boolean
    Public bPlcInput As Boolean
    Public bAnalog As Boolean
    Public nDataIndex As Integer
    Public nDataLength As Integer
    Public sURL As String
    Public bEnableToolTips As Boolean
    Public colPoints As Collection
End Class
Class clsRack
    Public nRackNumber As Integer
    Public sRackName As String
    Public nSlots As Integer
    Public nAutoFill As Integer
    Public sRackLabels As String
    Public colSlots As Collection
End Class
