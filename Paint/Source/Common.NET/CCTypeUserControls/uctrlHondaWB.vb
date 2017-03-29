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
' Form/Module: uctrlHondaWB.vb'
' Description: Versabell 1k cartoon control
' 
'
' Dependancies:  mMathFunctions.vb, uctrValve.vb
'           System.Drawing.Color
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Matt White
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    04/23/09   MSW     first draft
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'    04/16/13   MSW     Add Iso block                                                 4.01.05.00
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Public Class uctrlHondaWB
    Implements CCToonUctrl
    'Max valve constants, counting from 0
    Const mnMaxShared As Integer = 3
    Const mnMaxGroup As Integer = 31
    'Events passed back to the form
    Event GroupValveClick(ByVal nValve As Integer, ByVal nCurState As Boolean) Implements CCToonUctrl.GroupValveClick
    Event SharedValveClick(ByVal nValve As Integer, ByVal nCurState As Boolean) Implements CCToonUctrl.SharedValveClick
    'Current valve status
    Private mnGroupValveStates As Integer = 0
    Private mnSharedValveStates As Integer = 0
    Private mnDockPosition As Integer = 0
    Private mnCanisterPosition As Integer = 0
    Private mbFeedBackLabels As Boolean = True 'CC screen will set false to hide flow and pressure labels
    Private Enum eHondaWBShared
        pTRIG = 0
        Reserved2 = 1
        Reserved3 = 2
        pC1_24 = 3
    End Enum
    Private Enum eHondaWBGroup
        pDV = 0
        pPI = 1
        pGV = 2
        pSOL = 3
        pAIR = 4
        pCC = 5
        pDUMP = 6
        pCAN = 7
        pIAIR = 8
        pDUMP2 = 9
        pSOL2 = 10
        pPAINT = 11
        pX = 12
        p2T = 13
        pY = 14
        pLA = 15
        pHA = 16
        pHW = 17
        pLW = 18
        pWS = 19
        Reserved21 = 20
        Reserved22 = 21
        Reserved23 = 22
        Reserved24 = 23
        Reserved25 = 24
        Reserved26 = 25
        Reserved27 = 26
        Reserved28 = 27
        ACA = 28
        pACS = 29
        ACVA = 30
        Reserved32 = 31
    End Enum

    '********************************************************************************************
    'Description: Draw it.
    '
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Sub UpdateCartoons() Implements CCToonUctrl.UpdateCartoons

        Try

            Dim nBellWashBackColor As System.Drawing.Color
            Dim nBellWashValveColor As uctrlValve.eValveColor
            Dim nColStkBackColor As System.Drawing.Color
            Dim nSolvAirValveColor As uctrlValve.eValveColor
            Dim nP2TBackColor As System.Drawing.Color
            Dim nP2TValveColor As uctrlValve.eValveColor
            Dim nSolvAirBackColor As System.Drawing.Color
            Dim nColStkValveColor As uctrlValve.eValveColor

            'Get valves attached to a header first

            'App Cleaner Air
            If (GetBitState(mnGroupValveStates, eHondaWBGroup.ACA) = 1) Then
                UctrlGroupValve28.ValvState = True
                lblLineACA1.BackColor = Color.Blue
                lblLineACA2.BackColor = Color.Blue
            Else
                UctrlGroupValve28.ValvState = False
                lblLineACA1.BackColor = Color.Black
                lblLineACA2.BackColor = Color.Black
            End If
            'App Cleaner Solvent
            If (GetBitState(mnGroupValveStates, eHondaWBGroup.pACS) = 1) Then
                UctrlGroupValve29.ValvState = True
                lblLineACS1.BackColor = Color.Salmon
            Else
                UctrlGroupValve29.ValvState = False
                lblLineACS1.BackColor = Color.Black
            End If
            'ACVA
            If (GetBitState(mnGroupValveStates, eHondaWBGroup.ACVA) = 1) Then
                UctrlGroupValve30.ValvState = True
                lblLineACVA1.BackColor = Color.Blue
            Else
                UctrlGroupValve30.ValvState = False
                lblLineACVA1.BackColor = Color.Black
            End If

            'Bell Wash Group
            'Low Air
            UctrlGroupValve15.ValvState = (GetBitState(mnGroupValveStates, eHondaWBGroup.pLA) = 1)
            'High Air
            UctrlGroupValve16.ValvState = (GetBitState(mnGroupValveStates, eHondaWBGroup.pHA) = 1)
            'High Water
            UctrlGroupValve17.ValvState = (GetBitState(mnGroupValveStates, eHondaWBGroup.pHW) = 1)
            'Low Water
            UctrlGroupValve18.ValvState = (GetBitState(mnGroupValveStates, eHondaWBGroup.pLW) = 1)
            'Water/Solvent
            UctrlGroupValve19.ValvState = (GetBitState(mnGroupValveStates, eHondaWBGroup.pWS) = 1)

            'Select the valve and line colors after the bell wash valve block
            nBellWashBackColor = Color.Black
            If UctrlGroupValve15.ValvState Then
                nBellWashValveColor = uctrlValve.eValveColor.Blue
                nBellWashBackColor = Color.CornflowerBlue
            End If
            If UctrlGroupValve16.ValvState Then
                nBellWashValveColor = uctrlValve.eValveColor.Blue
                nBellWashBackColor = Color.Blue
            End If
            If UctrlGroupValve19.ValvState Then
                nBellWashValveColor = uctrlValve.eValveColor.Red
                nBellWashBackColor = Color.Salmon
            End If
            If UctrlGroupValve17.ValvState Or UctrlGroupValve18.ValvState Then
                nBellWashValveColor = uctrlValve.eValveColor.Red
                nBellWashBackColor = Color.Salmon
            End If

            lblLineCupWashSolv1.BackColor = nBellWashBackColor
            lblLineCupWashSolv2.BackColor = nBellWashBackColor

            'Purge Air
            UctrlGroupValve4.ValvState = (GetBitState(mnGroupValveStates, eHondaWBGroup.pAIR) = 1)
            'Purge Solvent
            UctrlGroupValve3.ValvState = (GetBitState(mnGroupValveStates, eHondaWBGroup.pSOL) = 1)

            'Select the valve and line colors after the solvair
            If UctrlGroupValve3.ValvState Then
                nSolvAirValveColor = uctrlValve.eValveColor.Red
                nSolvAirBackColor = Color.Salmon
            Else
                If UctrlGroupValve4.ValvState Then
                    nSolvAirValveColor = uctrlValve.eValveColor.Blue
                    nSolvAirBackColor = Color.Blue
                Else
                    nSolvAirValveColor = uctrlValve.eValveColor.Empty
                    nSolvAirBackColor = Color.Black
                End If
            End If
            'Everything attached to the solvair
            UctrlGroupValve5.ValveColor = nSolvAirValveColor 'PCC
            UctrlGroupValve13.ValveColor = nSolvAirValveColor 'p2T
            'Lines
            lblLineSolvAir1.BackColor = nSolvAirBackColor
            lblLineSolvAir2.BackColor = nSolvAirBackColor
            lblLineSolvAir3.BackColor = nSolvAirBackColor

            'p2T - used as the dock wash valve
            If (GetBitState(mnGroupValveStates, eHondaWBGroup.p2T) = 1) Then
                UctrlGroupValve13.ValvState = True
                nP2TValveColor = nSolvAirValveColor
                nP2TBackColor = nSolvAirBackColor
            Else
                UctrlGroupValve13.ValvState = False
                nP2TValveColor = uctrlValve.eValveColor.Empty
                nP2TBackColor = Color.Black
            End If
            'Everything attached to p2T
            lblLineDockDump1.BackColor = nP2TBackColor
            lblLineDockDump2.BackColor = nP2TBackColor
            lblLineDockDump3.BackColor = nP2TBackColor

            'pSOL2 WaterSol
            UctrlGroupValve10.ValvState = (GetBitState(mnGroupValveStates, eHondaWBGroup.pSOL2) = 1)
            'pCC
            UctrlGroupValve5.ValvState = (GetBitState(mnGroupValveStates, eHondaWBGroup.pCC) = 1)
            'Color Enable, color stack
            If (GetBitState(mnSharedValveStates, 3) = 1) Then
                UctrlSharedValve3.ValvState = True
                nColStkValveColor = uctrlValve.eValveColor.Purple
                nColStkBackColor = Color.Magenta
            Else
                UctrlSharedValve3.ValvState = False
                'pSOL2
                If UctrlGroupValve10.ValvState Then
                    nColStkValveColor = uctrlValve.eValveColor.Red
                    nColStkBackColor = Color.Red
                Else
                    'pCC and color stack paint line
                    If UctrlGroupValve5.ValvState Then
                        nColStkBackColor = nSolvAirBackColor
                        nColStkValveColor = nSolvAirValveColor
                    Else
                        nColStkValveColor = uctrlValve.eValveColor.Empty
                        nColStkBackColor = Color.Black
                    End If
                End If
            End If

            'Paint line and valves attached to it
            lblLineColStk1.BackColor = nColStkBackColor
            lblLineColStk2.BackColor = nColStkBackColor
            lblLineColStk3.BackColor = nColStkBackColor

            'Dock
            'pX
            If (GetBitState(mnGroupValveStates, eHondaWBGroup.pX) = 1) Then
                UctrlGroupValve12.ValvState = True
                'Keep using nColStk...
            Else
                UctrlGroupValve12.ValvState = False
                nColStkValveColor = uctrlValve.eValveColor.Empty
                nColStkBackColor = Color.Black
            End If
            UctrlGroupValve12.ValveColor = nColStkValveColor
            lblLineDock1.BackColor = nColStkBackColor
            UctrlGroupValve14.ValveColor = nColStkValveColor
            'pY
            If (GetBitState(mnGroupValveStates, eHondaWBGroup.pY) = 1) Then
                UctrlGroupValve14.ValvState = True
                'Keep using nColStk...
            Else
                UctrlGroupValve14.ValvState = False
                'set it black
                nColStkValveColor = uctrlValve.eValveColor.Empty
                nColStkBackColor = Color.Black
            End If
            lblLineDockOut1.BackColor = nColStkBackColor
            lblLineDockOut2.BackColor = nColStkBackColor
            UctrlGroupValve7.ValveColor = nColStkValveColor
            UctrlGroupValve9.ValveColor = nColStkValveColor
            'SUnit cartoon
            Select Case mnDockPosition
                Case 1
                    picSUnit.Image = imlSUnit.Images("dock")
                Case 2
                    picSUnit.Image = imlSUnit.Images("clean")
                Case 3
                    picSUnit.Image = imlSUnit.Images("dedock")
                Case Else
                    picSUnit.Image = imlSUnit.Images("unknown")
            End Select

            'Dump 2
            If (GetBitState(mnGroupValveStates, eHondaWBGroup.pDUMP2) = 1) Then
                UctrlGroupValve9.ValvState = True
                'lblLineDump1.BackColor = nColStkBackColor
                lblLineDump2.BackColor = nColStkBackColor
            Else
                UctrlGroupValve9.ValvState = False
                'lblLineDump1.BackColor = Color.Black
                lblLineDump2.BackColor = Color.Black
            End If

            'pCan
            If (GetBitState(mnGroupValveStates, eHondaWBGroup.pCAN) = 1) Then
                UctrlGroupValve7.ValvState = True
                lblLineCanOut1.BackColor = nColStkBackColor
                lblLineCanOut2.BackColor = nColStkBackColor
                lblLineCanOut3.BackColor = nColStkBackColor
                UctrlGroupValve11.ValveColor = nColStkValveColor
                UctrlGroupValve6.ValveColor = nColStkValveColor
            Else
                UctrlGroupValve7.ValvState = False
                nColStkValveColor = uctrlValve.eValveColor.Empty
                nColStkBackColor = Color.Black
            End If
            lblLineCanIn1.BackColor = nColStkBackColor
            lblLineCanIn2.BackColor = nColStkBackColor
            'select canister picture
            Select Case nColStkValveColor
                Case uctrlValve.eValveColor.Empty, uctrlValve.eValveColor.Green
                    If mnCanisterPosition > 1 Then
                        picCan.Image = imlCan.Images("CanisterPaint")
                        If UctrlGroupValve7.ValvState = False Then
                            nColStkBackColor = Color.Magenta
                            nColStkValveColor = uctrlValve.eValveColor.Purple
                        End If
                    Else
                        picCan.Image = imlCan.Images("CanisterEmpty")
                    End If
                Case uctrlValve.eValveColor.Blue
                    picCan.Image = imlCan.Images("CanisterAir")
                Case uctrlValve.eValveColor.Purple
                    picCan.Image = imlCan.Images("CanisterPaint")
                Case uctrlValve.eValveColor.Red
                    picCan.Image = imlCan.Images("CanisterSolv")
            End Select
            lblLineCanOut1.BackColor = nColStkBackColor
            lblLineCanOut2.BackColor = nColStkBackColor
            lblLineCanOut3.BackColor = nColStkBackColor
            UctrlGroupValve11.ValveColor = nColStkValveColor
            UctrlGroupValve6.ValveColor = nColStkValveColor
            'Dump
            If (GetBitState(mnGroupValveStates, eHondaWBGroup.pDUMP) = 1) Then
                UctrlGroupValve6.ValvState = True
                lblLineCanDump1.BackColor = nColStkBackColor
                lblLineCanDump2.BackColor = nColStkBackColor
                UctrlGroupValve1.ValveColor = nColStkValveColor
            Else
                UctrlGroupValve6.ValvState = False
                lblLineCanDump1.BackColor = Color.Black
                If (GetBitState(mnGroupValveStates, eHondaWBGroup.pDUMP2) = 1) Then
                    UctrlGroupValve1.ValveColor = UctrlGroupValve9.ValveColor
                    lblLineCanDump2.BackColor = lblLineDump2.BackColor
                Else
                    UctrlGroupValve1.ValveColor = uctrlValve.eValveColor.Green
                    lblLineCanDump2.BackColor = Color.Black
                End If
            End If
            'pPI
            If (GetBitState(mnGroupValveStates, eHondaWBGroup.pPI) = 1) Then
                UctrlGroupValve1.ValvState = True
                lblLineDumpOut1.BackColor = lblLineCanDump2.BackColor
            Else
                UctrlGroupValve1.ValvState = False
                lblLineDumpOut1.BackColor = Color.Black
            End If

            'Dump
            If (GetBitState(mnGroupValveStates, eHondaWBGroup.pIAIR) = 1) Then
                UctrlGroupValve8.ValvState = True
                lblLineDumpOut2.BackColor = Color.Blue
            Else
                UctrlGroupValve8.ValvState = False
                lblLineDumpOut2.BackColor = lblLineDumpOut1.BackColor
            End If


            'Paint (can out)
            If (GetBitState(mnGroupValveStates, eHondaWBGroup.pPAINT) = 1) Then
                UctrlGroupValve11.ValvState = True
            Else
                UctrlGroupValve11.ValvState = False
            End If

            'pDV - Gun Dump, used as an injector wash valve
            If (GetBitState(mnGroupValveStates, eHondaWBGroup.pDV) = 1) Then
                UctrlGroupValve0.ValvState = True
                UctrlGroupValve0.ValveColor = nBellWashValveColor
                nColStkValveColor = nBellWashValveColor
                nColStkBackColor = nBellWashBackColor
            Else
                UctrlGroupValve0.ValvState = False
            End If

            If (UctrlGroupValve0.ValvState = False) And (UctrlGroupValve11.ValvState = False) Then
                UctrlGroupValve0.ValveColor = uctrlValve.eValveColor.Empty
                nColStkValveColor = uctrlValve.eValveColor.Empty
                nColStkBackColor = Color.Black
            End If

            lblLinePaint1.BackColor = nColStkBackColor
            lblLinePaint2.BackColor = nColStkBackColor
            lblLinePaint3.BackColor = nColStkBackColor

            'Trigger
            UctrlSharedValve0.ValveColor = nColStkValveColor
            UctrlSharedValve0a.ValveColor = nColStkValveColor
            UctrlSharedValve0b.ValveColor = nColStkValveColor
            If (GetBitState(mnSharedValveStates, eHondaWBShared.pTRIG) = 1) Then
                UctrlSharedValve0.ValvState = True
                UctrlSharedValve0a.ValvState = True
                UctrlSharedValve0b.ValvState = True
                lblLineInjector.BackColor = nColStkBackColor
            Else
                UctrlSharedValve0.ValvState = False
                UctrlSharedValve0a.ValvState = False
                UctrlSharedValve0b.ValvState = False
                lblLineInjector.BackColor = Color.Black
            End If

            'pGV - Gate Valve, used as an external bell wash valve
            If (GetBitState(mnGroupValveStates, eHondaWBGroup.pGV) = 1) Then
                UctrlGroupValve2.ValvState = True
            Else
                UctrlGroupValve2.ValvState = False
                nBellWashValveColor = uctrlValve.eValveColor.Empty
                nBellWashBackColor = Color.Black
            End If
            UctrlGroupValve2.ValveColor = nBellWashValveColor
            lblLineWash1.BackColor = nBellWashBackColor
            lblLineWash2.BackColor = nBellWashBackColor

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub

    '********************************************************************************************
    'Description: pass a group valve click back to the owner of this user control
    '
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Private Sub UctrlGroupValve_Click(ByVal sender As Object) Handles UctrlGroupValve0.ValveClick, UctrlGroupValve1.ValveClick, UctrlGroupValve2.ValveClick, UctrlGroupValve3.ValveClick, _
        UctrlGroupValve4.ValveClick, UctrlGroupValve5.ValveClick, UctrlGroupValve6.ValveClick, UctrlGroupValve7.ValveClick, UctrlGroupValve8.ValveClick, UctrlGroupValve9.ValveClick, UctrlGroupValve10.ValveClick, _
        UctrlGroupValve11.ValveClick, UctrlGroupValve12.ValveClick, UctrlGroupValve13.ValveClick, UctrlGroupValve14.ValveClick, UctrlGroupValve15.ValveClick, _
        UctrlGroupValve16.ValveClick, UctrlGroupValve17.ValveClick, UctrlGroupValve18.ValveClick, UctrlGroupValve19.ValveClick, UctrlGroupValve20.ValveClick, _
        UctrlGroupValve21.ValveClick, UctrlGroupValve22.ValveClick, UctrlGroupValve23.ValveClick, UctrlGroupValve24.ValveClick, UctrlGroupValve25.ValveClick, _
        UctrlGroupValve26.ValveClick, UctrlGroupValve27.ValveClick, UctrlGroupValve28.ValveClick, UctrlGroupValve29.ValveClick, UctrlGroupValve30.ValveClick, _
        UctrlGroupValve31.ValveClick

        Try
            Dim oValve As uctrlValve = DirectCast(sender, uctrlValve)

            If mbFeedBackLabels AndAlso (oValve.ValvState = False) Then
                If CheckConflict(oValve.Name) Then Exit Sub
            End If

            RaiseEvent GroupValveClick(CInt(oValve.Tag), oValve.ValvState)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub

    '********************************************************************************************
    'Description: pass a shared valve click back to the owner of this user control
    '
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Private Sub UctrlSharedValve_Click(ByVal sender As Object) Handles UctrlSharedValve0.ValveClick, UctrlSharedValve0b.ValveClick, _
     UctrlSharedValve1.ValveClick, UctrlSharedValve2.ValveClick, UctrlSharedValve3.ValveClick

        Try
            Dim oValve As uctrlValve = DirectCast(sender, uctrlValve)

            If mbFeedBackLabels AndAlso (oValve.ValvState = False) Then
                If CheckConflict(oValve.Name) Then Exit Sub
            End If

            RaiseEvent SharedValveClick(CInt(oValve.Tag), oValve.ValvState)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub

    '********************************************************************************************
    'Description: Check for conflicts. Return false if none.
    '
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Private Function CheckConflict(ByVal ValveName As String) As Boolean

        Dim bConflict As Boolean
        'Fluid outlet through Dump2
        Dim bDumpPathOK As Boolean = (UctrlGroupValve5.ValvState And _
                                      UctrlGroupValve12.ValvState And _
                                      UctrlGroupValve14.ValvState And _
                                      UctrlGroupValve9.ValvState And _
                                      UctrlGroupValve1.ValvState)
        'Fluid outlet through Trigger or Dump
        Dim bTrigPathOK As Boolean = (UctrlGroupValve5.ValvState And _
                                      UctrlGroupValve12.ValvState And _
                                      UctrlGroupValve14.ValvState And _
                                      UctrlGroupValve7.ValvState) And _
                                      ((UctrlGroupValve11.ValvState And _
                                      UctrlSharedValve0.ValvState) Or _
                                      (UctrlGroupValve6.ValvState) And _
                                      (UctrlGroupValve1.ValvState))
        'Ok to clean dock face
        Dim bpP2TOK As Boolean = UctrlGroupValve13.ValvState And (mnDockPosition = 2)

        Try

            Select Case ValveName
                Case "UctrlSharedValve0" 'pTRIG
                Case "UctrlSharedValve3" 'pC1_24
                    bConflict = UctrlGroupValve10.ValvState
                Case "UctrlGroupValve0" 'pDV
                Case "UctrlGroupValve1" 'pPI
                    bConflict = False
                Case "UctrlGroupValve2" 'pGV
                Case "UctrlGroupValve3" 'pSOL
                    bConflict = Not (bDumpPathOK Or bTrigPathOK Or bpP2TOK)
                Case "UctrlGroupValve4" 'pAIR
                    If UctrlGroupValve3.ValvState Then
                        bConflict = Not (bDumpPathOK Or bTrigPathOK Or bpP2TOK)
                    End If
                Case "UctrlGroupValve5" 'pCC
                    bConflict = UctrlSharedValve3.ValvState
                Case "UctrlGroupValve6" 'pDUMP
                Case "UctrlGroupValve7" 'pCAN
                Case "UctrlGroupValve8" 'pIAIR
                    bConflict = False
                Case "UctrlGroupValve9" 'pDUMP2
                Case "UctrlGroupValve10" 'pSOL2
                    bConflict = UctrlSharedValve3.ValvState
                Case "UctrlGroupValve11" 'pPAINT
                Case "UctrlGroupValve12" 'pX
                    bConflict = (mnDockPosition <> 1)
                Case "UctrlGroupValve13" 'p2T
                    bConflict = (mnDockPosition <> 2)
                Case "UctrlGroupValve14" 'pY
                    bConflict = (mnDockPosition <> 1)
                Case "UctrlGroupValve15" 'pLA
                Case "UctrlGroupValve16" 'pHA
                Case "UctrlGroupValve17" 'pHW
                    bConflict = UctrlGroupValve18.ValvState
                Case "UctrlGroupValve18" 'pLW
                    bConflict = UctrlGroupValve17.ValvState
                Case "UctrlGroupValve19" 'pWS
            End Select

        Catch ex As Exception

        End Try

        Return bConflict

    End Function

    '********************************************************************************************
    'Description: This property shows or hides extra labels
    '
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Property ShowFeedBackLabels() As Boolean Implements CCToonUctrl.ShowFeedBackLabels
        Get
            Return mbFeedBackLabels
        End Get
        Set(ByVal value As Boolean)
            mbFeedBackLabels = value
            lblFeedBack.Visible = mbFeedBackLabels
            lblFlow.Visible = mbFeedBackLabels
            lblFlowTag.Visible = mbFeedBackLabels
            lblCanPos.Visible = mbFeedBackLabels
            lblCanPosTag.Visible = mbFeedBackLabels
            lblCanTorque.Visible = mbFeedBackLabels
            lblCanTorqueTag.Visible = mbFeedBackLabels
            lblSUnitTorque.Visible = mbFeedBackLabels
            lblSUnitTorqueTag.Visible = mbFeedBackLabels
            lblSUnitPosition.Visible = mbFeedBackLabels
            lblSUnitPositionTag.Visible = mbFeedBackLabels
            picSUnit.Visible = mbFeedBackLabels
        End Set
    End Property
    '********************************************************************************************
    'Description: This property gives access to the color change shared valves
    '
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Property SharedValveStates() As Integer Implements CCToonUctrl.SharedValveStates
        Get
            Return mnSharedValveStates
        End Get
        Set(ByVal value As Integer)
            mnSharedValveStates = value
        End Set
    End Property
    '********************************************************************************************
    'Description: This property gives access to the color change group valves
    '
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Property GroupValveStates() As Integer Implements CCToonUctrl.GroupValveStates
        Get
            Return mnGroupValveStates
        End Get
        Set(ByVal value As Integer)
            mnGroupValveStates = value
        End Set
    End Property

    '********************************************************************************************
    'Description: This property makes the label on the paint header available
    '
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Property PaintColorName() As String Implements CCToonUctrl.PaintColorName
        Get
            Return lblPntHeader01.Text
        End Get
        Set(ByVal value As String)
            lblPntHeader01.Text = value
        End Set
    End Property

    '********************************************************************************************
    'Description: This property makes the label on the solvent header available
    '
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Property SolvHeaderLabel() As String Implements CCToonUctrl.SolvHeaderLabel
        Get
            Return lblSolventHeader01.Text
        End Get
        Set(ByVal value As String)
            lblSolventHeader01.Text = value
        End Set
    End Property

    '********************************************************************************************
    'Description: This property makes the label on the solvent header available
    '
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Property AirHeaderLabel() As String Implements CCToonUctrl.AirHeaderLabel
        Get
            Return lblAirHeader01.Text
        End Get
        Set(ByVal value As String)
            lblAirHeader01.Text = value
        End Set
    End Property

    '********************************************************************************************
    'Description: Set shared valve labels with a string array
    '
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Public Sub SetSharedValveLabels(ByRef sNames As String()) Implements CCToonUctrl.SetSharedValveLabels
        Dim oLabel As Label
        Dim nOffset As Integer
        nOffset = LBound(sNames)
        For nIndex As Integer = 0 To (UBound(sNames) - nOffset)
            If nIndex > mnMaxShared Then
                Exit For
            End If
            Try
                oLabel = DirectCast(Me.Controls.Item("lblShared" & nIndex.ToString("00")), Label)
                oLabel.Text = sNames(nIndex)
            Catch ex As Exception
                Trace.WriteLine(ex.Message)
                Exit For
            End Try
        Next
    End Sub

    '********************************************************************************************
    'Description: Set group valve labels with a string array
    '
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Public Sub SetGroupValveLabels(ByRef sNames As String()) Implements CCToonUctrl.SetGroupValveLabels
        Dim oLabel As Label
        Dim nOffset As Integer
        nOffset = LBound(sNames)
        For nIndex As Integer = 0 To (UBound(sNames) - nOffset)
            If nIndex > mnMaxGroup Then
                Exit For
            End If
            Try
                oLabel = DirectCast(Me.Controls.Item("lblGroup" & nIndex.ToString("00")), Label)
                oLabel.Text = sNames(nIndex)
            Catch ex As Exception
                Trace.WriteLine(ex.Message)
                Exit For
            End Try
        Next
    End Sub

    '********************************************************************************************
    'Description: Set extra labels
    '
    '
    'Input parameters:
    ' array of strings.  first string is the label name, 2nd is the value.  Repeat for 3rd and 4th ...
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Public Sub SetAdditionalData(ByRef sData As String()) Implements CCToonUctrl.SetAdditionalData
        Dim nIndex As Integer
        Dim oLabel As Label

        nIndex = LBound(sData)
        While (nIndex < UBound(sData))
            Try
                oLabel = DirectCast(Me.Controls.Item(sData(nIndex)), Label)
                Select Case oLabel.Name
                    Case "lblCanPos"
                        'Store away the canister position for use elsewhere
                        mnCanisterPosition = CType(sData(nIndex + 1), Integer)
                        oLabel.Text = sData(nIndex + 1)
                    Case "lblSUnitPosition"
                        'Handle special case for S-Unit Position
                        oLabel.Text = GetDockPosText(sData(nIndex + 1))
                        Try
                            mnDockPosition = CType(sData(nIndex + 1), Integer)
                        Catch ex As Exception
                            mnDockPosition = 0 'Unknown
                        End Try
                    Case Else
                        oLabel.Text = sData(nIndex + 1)
                End Select
                nIndex = nIndex + 2
            Catch ex As Exception
                Trace.WriteLine(ex.Message)
                Exit While
            End Try
        End While
    End Sub
    '********************************************************************************************
    'Description: Convert S-Unit Position Index to position string
    '
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Private Function GetDockPosText(ByVal Index As String) As String
        Dim sPos As String = grsRM.GetString("rsHWB_DOCK_POS_UNK")

        Select Case Index
            Case "1" 'Dock
                sPos = grsRM.GetString("rsHWB_DOCK_POS_DOCK")
            Case "2" 'Clean
                sPos = grsRM.GetString("rsHWB_DOCK_POS_CLN")
            Case "3" 'De-Dock
                sPos = grsRM.GetString("rsHWB_DOCK_POS_DDK")
        End Select

        Return sPos

    End Function
    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Private Sub Me_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            For Each oControl As Control In Me.Controls
                If TypeOf oControl Is uctrlValve Then
                    Dim oValve As uctrlValve = DirectCast(oControl, uctrlValve)
                    oValve.ValveColor = uctrlValve.eValveColor.Empty
                End If
            Next

            'Assign colors to everything on a header
            UctrlSharedValve3.ValveColor = uctrlValve.eValveColor.Purple 'CE
            UctrlGroupValve3.ValveColor = uctrlValve.eValveColor.Red 'pSOL
            UctrlGroupValve4.ValveColor = uctrlValve.eValveColor.Blue 'pAIR
            UctrlGroupValve10.ValveColor = uctrlValve.eValveColor.Red 'pSOL2
            UctrlGroupValve15.ValveColor = uctrlValve.eValveColor.Blue 'pLA
            UctrlGroupValve16.ValveColor = uctrlValve.eValveColor.Blue 'pHA
            UctrlGroupValve17.ValveColor = uctrlValve.eValveColor.Red 'pHW
            UctrlGroupValve18.ValveColor = uctrlValve.eValveColor.Red 'pLW
            UctrlGroupValve19.ValveColor = uctrlValve.eValveColor.Red 'pWA
            UctrlGroupValve28.ValveColor = uctrlValve.eValveColor.Blue 'ACA
            UctrlGroupValve29.ValveColor = uctrlValve.eValveColor.Red 'ACS
            UctrlGroupValve30.ValveColor = uctrlValve.eValveColor.Blue 'ACVA

            lblFeedBack.Visible = mbFeedBackLabels
            lblFlow.Visible = mbFeedBackLabels
            lblFlowTag.Visible = mbFeedBackLabels
            lblCanPos.Visible = mbFeedBackLabels
            lblCanPosTag.Visible = mbFeedBackLabels
            lblCanTorque.Visible = mbFeedBackLabels
            lblCanTorqueTag.Visible = mbFeedBackLabels
            lblSUnitTorque.Visible = mbFeedBackLabels
            lblSUnitTorqueTag.Visible = mbFeedBackLabels
            lblSUnitPosition.Visible = mbFeedBackLabels
            lblSUnitPositionTag.Visible = mbFeedBackLabels

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub

    Private Sub UctrlGroupValve3_Load(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
End Class

