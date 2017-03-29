﻿' This material is the joint property of FANUC Robotics America and
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
' Form/Module: uctrlACP2KGunFlex.vb'
' Description: ACP 2k Gun cartoon control with PPE
' 
'
' Dependancies:  mMathFunctions.vb, uctrValve.vb
'           System.Drawing.Color
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Harvey Benner 
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    04/24/10   HGB     Code copied from the Honda 1k from MSW
'    08/17/11   HGB     Code copied from the ACP gun module
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Public Class uctrlACP2KGunFlex
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
    Private mbFeedBackLabels As Boolean = True 'CC screen will set false to hide flow and pressure labels
    Private Enum eACP2KGunFlexShared
        Trigger = 0
        Reserved2 = 1
        HardenerEnable = 2
        ColorEnable = 3
    End Enum
    Private Enum eACP2KGunFlexGroup
        PPE = 0
        Reserved2 = 1
        Reserved3 = 2
        PurgeSolv = 3
        PurgeAir = 4
        PCC = 5
        Dump = 6
        Flush = 7
        Reserved9 = 8
        Dump2 = 9
        PurgeSolv2 = 10
        Flush2 = 11
        Dump3 = 12
        P2T = 13
        Reserved15 = 14
        Reserved16 = 15
        Reserved17 = 16
        Reserved18 = 17
        Reserved19 = 18
        Reserved20 = 19
        Reserved21 = 20
        Reserved22 = 21
        Reserved23 = 22
        Reserved24 = 23
        Reserved25 = 24
        Reserved26 = 25
        Reserved27 = 26
        Reserved28 = 27
        ACA = 28
        ACS = 29
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

            Dim nSolvAirValveColor As uctrlValve.eValveColor
            Dim nSolvAirBackColor As System.Drawing.Color
            Dim nColStkValveColor As uctrlValve.eValveColor
            Dim nColStkBackColor As System.Drawing.Color
            Dim nHdrStkValveColor As uctrlValve.eValveColor
            Dim nHdrStkBackColor As System.Drawing.Color
            Dim nMixStkBackColor As System.Drawing.Color
            'Dim nGDValveColor As uctrlValve.eValveColor
            'Dim nGDBackColor As System.Drawing.Color
            Dim nTrigValveColor As uctrlValve.eValveColor
            'Dim nTrigBackColor As System.Drawing.Color

            'Get valves attacched to a header first

            'App Cleaner Air
            If (GetBitState(mnGroupValveStates, eACP2KGunFlexGroup.ACA) = 1) Then
                UctrlGroupValve28.ValvState = True
                lblLineACA.BackColor = Color.Blue
                lblLineACA2.BackColor = Color.Blue
                lblLineACA3.BackColor = Color.Blue
            Else
                UctrlGroupValve28.ValvState = False
                lblLineACA.BackColor = Color.Black
                lblLineACA2.BackColor = Color.Black
                lblLineACA3.BackColor = Color.Black
            End If
            'App Cleaner Solvent
            If (GetBitState(mnGroupValveStates, eACP2KGunFlexGroup.ACS) = 1) Then
                UctrlGroupValve29.ValvState = True
                lblLineACS.BackColor = Color.Red
            Else
                UctrlGroupValve29.ValvState = False
                lblLineACS.BackColor = Color.Black
            End If
            'ACVA
            If (GetBitState(mnGroupValveStates, eACP2KGunFlexGroup.ACVA) = 1) Then
                UctrlGroupValve30.ValvState = True
                lblLineACVA.BackColor = Color.Blue
            Else
                UctrlGroupValve30.ValvState = False
                lblLineACVA.BackColor = Color.Black
            End If
            'Purge Air
            UctrlGroupValve4.ValvState = (GetBitState(mnGroupValveStates, eACP2KGunFlexGroup.PurgeAir) = 1)
            'Purge Solvent
            UctrlGroupValve3.ValvState = (GetBitState(mnGroupValveStates, eACP2KGunFlexGroup.PurgeSolv) = 1)
            'Select the valve and line colors after the solvair
            If UctrlGroupValve3.ValvState Then
                nSolvAirValveColor = uctrlValve.eValveColor.Red
                nSolvAirBackColor = Color.Red
            Else
                If UctrlGroupValve4.ValvState Then
                    nSolvAirValveColor = uctrlValve.eValveColor.Blue
                    nSolvAirBackColor = Color.Blue
                Else
                    nSolvAirValveColor = uctrlValve.eValveColor.Empty
                    nSolvAirBackColor = Color.Black
                End If
            End If

            lblLineSolvAir.BackColor = nSolvAirBackColor
            lblLineSolvAir1.BackColor = nSolvAirBackColor
            lblLineSolvAir2.BackColor = nSolvAirBackColor
            lblLineSolvAir3.BackColor = nSolvAirBackColor

            'pCC
            UctrlGroupValve5.ValvState = (GetBitState(mnGroupValveStates, eACP2KGunFlexGroup.PCC) = 1)

            If (UctrlGroupValve5.ValvState = True) Then
                UctrlGroupValve5.ValveColor = nSolvAirValveColor
                nColStkBackColor = nSolvAirBackColor
                nColStkValveColor = nSolvAirValveColor
            Else
                nColStkValveColor = uctrlValve.eValveColor.Empty
                nColStkBackColor = Color.Black
            End If

            'Color Enable
            If (GetBitState(mnSharedValveStates, eACP2KGunFlexShared.ColorEnable) = 1) Then
                UctrlSharedValve3.ValvState = True
                nColStkValveColor = uctrlValve.eValveColor.Purple
                nColStkBackColor = Color.FromArgb(255, 0, 255)
            Else
                If UctrlGroupValve3.ValvState Or UctrlGroupValve4.ValvState Then
                    'Don't change colors.
                    UctrlSharedValve3.ValvState = False
                Else
                    UctrlSharedValve3.ValvState = False
                    nColStkValveColor = uctrlValve.eValveColor.Empty
                    nColStkBackColor = Color.Black
                End If
            End If

            'Dump2
            UctrlGroupValve9.ValvState = (GetBitState(mnGroupValveStates, eACP2KGunFlexGroup.Dump2) = 1)
            If (UctrlGroupValve9.ValvState = True) Then
                UctrlGroupValve9.ValveColor = nColStkValveColor
                lblLineDump1.BackColor = nColStkBackColor
                lblLineDump2.BackColor = nColStkBackColor
                lblLineDump3.BackColor = nColStkBackColor
            Else
                UctrlGroupValve9.ValveColor = uctrlValve.eValveColor.Empty
                lblLineDump1.BackColor = Color.Black
                lblLineDump2.BackColor = Color.Black
                lblLineDump3.BackColor = Color.Black
            End If


            'p2T
            UctrlGroupValve13.ValvState = (GetBitState(mnGroupValveStates, eACP2KGunFlexGroup.P2T) = 1)
            If (UctrlGroupValve13.ValvState = True) Then
                UctrlGroupValve13.ValveColor = nSolvAirValveColor
            Else
                UctrlGroupValve13.ValveColor = uctrlValve.eValveColor.Empty
            End If

            'Dump3
            UctrlGroupValve12.ValvState = (GetBitState(mnGroupValveStates, eACP2KGunFlexGroup.Dump3) = 1)
            If (UctrlGroupValve12.ValvState = True) Then
                If (UctrlGroupValve13.ValvState = True) Then
                    UctrlGroupValve12.ValveColor = nSolvAirValveColor
                    lblLineDump1.BackColor = nSolvAirBackColor
                    lblLineDump2.BackColor = nSolvAirBackColor
                Else
                    UctrlGroupValve12.ValveColor = uctrlValve.eValveColor.Empty
                    If (UctrlGroupValve9.ValvState = False) Then 'Dump2 is off so clear the lines
                        lblLineDump1.BackColor = Color.Black
                        lblLineDump2.BackColor = Color.Black
                    End If
                End If
            Else
                UctrlGroupValve13.ValveColor = uctrlValve.eValveColor.Empty
                If (UctrlGroupValve9.ValvState = False) Then 'Dump2 is off so clear the lines
                    lblLineDump1.BackColor = Color.Black
                    lblLineDump2.BackColor = Color.Black
                End If

            End If




            'Paint line and valves attached to it
            lblLineColStk1.BackColor = nColStkBackColor

            'Pump Flush
            'UctrlGroupValve7.ValveColor = nSolvAirValveColor
            UctrlGroupValve7.ValveColor = uctrlValve.eValveColor.Red
            If (GetBitState(mnGroupValveStates, eACP2KGunFlexGroup.Flush) = 1) Then
                UctrlGroupValve7.ValvState = True
                lblLinePF.BackColor = Color.Red
                nColStkBackColor = Color.Red
            Else
                UctrlGroupValve7.ValvState = False
                lblLinePF.BackColor = Color.Black
            End If

            'Paint line and valves attached to it
            'lblLineColStk1.BackColor = nColStkBackColor
            lblLineColStk2.BackColor = nColStkBackColor
            lblLineColStk3.BackColor = nColStkBackColor


            'The Hardener side

            'Select the valve and line colors after the solvair

            nHdrStkBackColor = Color.Black
            nHdrStkValveColor = uctrlValve.eValveColor.Empty

            'Purge Solvent 2
            If (GetBitState(mnGroupValveStates, eACP2KGunFlexGroup.PurgeSolv2) = 1) Then
                UctrlGroupValve10.ValvState = True
                nHdrStkBackColor = Color.Red
                nHdrStkValveColor = uctrlValve.eValveColor.Red
            Else
                UctrlGroupValve10.ValvState = False
            End If

            'Hardener Enable
            If (GetBitState(mnSharedValveStates, eACP2KGunFlexShared.HardenerEnable) = 1) Then
                UctrlSharedValve2.ValvState = True
                nHdrStkBackColor = Color.FromArgb(255, 0, 255)
                nHdrStkValveColor = uctrlValve.eValveColor.Purple
            Else
                UctrlSharedValve2.ValvState = False
            End If

            lblLineHdrStk1.BackColor = nHdrStkBackColor

            'Pump Flush 2
            If (GetBitState(mnGroupValveStates, eACP2KGunFlexGroup.Flush2) = 1) Then
                UctrlGroupValve11.ValvState = True
                nHdrStkBackColor = Color.Red
            Else
                UctrlGroupValve11.ValvState = False
            End If

            lblLineHdrStk2.BackColor = nHdrStkBackColor
            lblLineHdrStk3.BackColor = nHdrStkBackColor

            'Mix stack
            nMixStkBackColor = nHdrStkBackColor
            nTrigValveColor = nHdrStkValveColor

            If (UctrlGroupValve5.ValvState = False) And _
               (UctrlGroupValve7.ValvState = False) And _
               (UctrlSharedValve3.ValvState = False) Then
                nMixStkBackColor = nHdrStkBackColor
                nTrigValveColor = nHdrStkValveColor
            ElseIf (UctrlGroupValve7.ValvState = True) Then 'And _
                '((UctrlGroupValve10.ValvState = True) Or _
                '(UctrlGroupValve11.ValvState = True)) Then
                nMixStkBackColor = Color.Red 'nSolvAirBackColor
                nTrigValveColor = uctrlValve.eValveColor.Red ' nSolvAirValveColor
            ElseIf (UctrlGroupValve10.ValvState = True) Or _
                   (UctrlGroupValve11.ValvState = True) Or _
                   ((UctrlGroupValve3.ValvState = True) And (UctrlGroupValve5.ValvState = True)) Then
                nMixStkBackColor = Color.Red
                nTrigValveColor = uctrlValve.eValveColor.Red
            ElseIf (UctrlGroupValve4.ValvState = True) And _
                   (UctrlGroupValve5.ValvState = True) Then
                nMixStkBackColor = Color.Blue
                nTrigValveColor = uctrlValve.eValveColor.Blue
            ElseIf (UctrlSharedValve2.ValvState = True) Or (UctrlSharedValve3.ValvState = True) Then
                nMixStkBackColor = Color.FromArgb(255, 0, 255)
                nTrigValveColor = uctrlValve.eValveColor.Purple
            End If

            lblLineMixed1.BackColor = nMixStkBackColor



            'Dump
            If (GetBitState(mnGroupValveStates, eACP2KGunFlexGroup.Dump) = 1) Then
                UctrlGroupValve6.ValvState = True
                lblLineMixed2.BackColor = nMixStkBackColor
                UctrlGroupValve6.ValveColor = nTrigValveColor
                lblLineDump1.BackColor = nMixStkBackColor
            Else
                UctrlGroupValve6.ValvState = False
                'Only clear the line if the rest of the dumps are off
                If (UctrlGroupValve9.ValvState = True) Or ((UctrlGroupValve12.ValvState = True) And (UctrlGroupValve13.ValvState = True)) Then
                    'leave the last line state set by the other valves
                Else
                    lblLineDump1.BackColor = Color.Black
                End If

            End If


            'Ppe
            UctrlGroupValve0.ValvState = (GetBitState(mnGroupValveStates, eACP2KGunFlexGroup.PPE) = 1)
            UctrlGroupValve0.ValveColor = nTrigValveColor
            If (UctrlGroupValve0.ValvState = True) Then
                lblLineMixedPPE.BackColor = nMixStkBackColor
            Else
                lblLineMixedPPE.BackColor = Color.Black
                nTrigValveColor = uctrlValve.eValveColor.Green
            End If

            'Trigger
            UctrlSharedValve0.ValveColor = nTrigValveColor
            'UctrlSharedValve0a.ValveColor = nTrigValveColor
            'UctrlSharedValve0a.ValveColor = nTrigValveColor
            If (GetBitState(mnSharedValveStates, eACP2KGunFlexShared.Trigger) = 1) Then
                UctrlSharedValve0.ValvState = True
                UctrlSharedValve0.ValveColor = nTrigValveColor
                'UctrlSharedValve0a.ValvState = True
                'UctrlSharedValve0a.ValvState = True
                'lblLineInjector.BackColor = nTrigBackColor
            Else
                UctrlSharedValve0.ValvState = False
                'UctrlSharedValve0a.ValvState = False
                'UctrlSharedValve0a.ValvState = False
                'lblLineInjector.BackColor = Color.Black
            End If
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
    Private Sub UctrlSharedValve_Click(ByVal sender As Object) Handles UctrlSharedValve0.ValveClick, UctrlSharedValve0a.ValveClick, _
     UctrlSharedValve1.ValveClick, UctrlSharedValve2.ValveClick, UctrlSharedValve3.ValveClick
        Try
            Dim oValve As uctrlValve = DirectCast(sender, uctrlValve)
            RaiseEvent SharedValveClick(CInt(oValve.Tag), oValve.ValvState)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub
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
            lblFlow.Visible = mbFeedBackLabels
            lblFlow2.Visible = mbFeedBackLabels
            lblFlowTag.Visible = mbFeedBackLabels
            lblOutPressure.Visible = mbFeedBackLabels
            lblOutPressure2.Visible = mbFeedBackLabels
            lblOutPressureTag.Visible = mbFeedBackLabels
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
            Return lblSolventHeader1.Text
        End Get
        Set(ByVal value As String)
            lblSolventHeader1.Text = value
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
                oLabel.Text = sData(nIndex + 1)
                nIndex = nIndex + 2
            Catch ex As Exception
                Trace.WriteLine(ex.Message)
                Exit While
            End Try
        End While
    End Sub
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
            UctrlGroupValve3.ValveColor = uctrlValve.eValveColor.Red 'Purge Solvent
            UctrlGroupValve4.ValveColor = uctrlValve.eValveColor.Blue 'Purge Air
            'UctrlGroupValve7.ValveColor = uctrlValve.eValveColor.Red 'Pump Flush
            UctrlSharedValve2.ValveColor = uctrlValve.eValveColor.Purple 'hardner
            UctrlGroupValve10.ValveColor = uctrlValve.eValveColor.Red 'pSolv2
            UctrlGroupValve11.ValveColor = uctrlValve.eValveColor.Red 'pflush2
            UctrlGroupValve28.ValveColor = uctrlValve.eValveColor.Blue 'ACA
            UctrlGroupValve29.ValveColor = uctrlValve.eValveColor.Red 'ACS
            UctrlGroupValve30.ValveColor = uctrlValve.eValveColor.Blue 'ACVA
            UctrlSharedValve3.ValveColor = uctrlValve.eValveColor.Purple 'CE
            lblFlow.Visible = mbFeedBackLabels
            lblFlow2.Visible = mbFeedBackLabels
            lblFlowTag.Visible = mbFeedBackLabels
            lblOutPressure.Visible = mbFeedBackLabels
            lblOutPressure2.Visible = mbFeedBackLabels
            lblOutPressureTag.Visible = mbFeedBackLabels
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub

    Private Sub lblOutPressure2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblOutPressure2.Click

    End Sub
    Private Sub lblFlow2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblFlow2.Click

    End Sub
    Private Sub Label10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label10.Click

    End Sub
    Private Sub lblLineSolvAirout3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblLineHdrStk2.Click

    End Sub
    Private Sub Label11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label11.Click

    End Sub
    Private Sub UctrlGroupValve11_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UctrlGroupValve10.Load

    End Sub
    Private Sub UctrlGroupValve13_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UctrlSharedValve2.Load

    End Sub
    Private Sub lblLineSolvAirOut1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblLineHdrStk1.Click

    End Sub
    Private Sub lblSolventHeader3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblSolventHeader3.Click

    End Sub
    Private Sub UctrlSharedValve2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UctrlGroupValve11.Load

    End Sub
    Private Sub picVersabellGearsPump02_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles picVersabellGearsPump02.Click

    End Sub
End Class

