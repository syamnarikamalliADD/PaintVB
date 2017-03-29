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
' Form/Module: uctrlVersabell2_2k.vb'
' Description: Versabell 2k cartoon control
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
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Public Class uctrlGun_2k_Pig
    Implements CCToonUctrl
    'Max valve constants, counting from 0
    Const mnMaxShared As Integer = 5
    Const mnMaxGroup As Integer = 31
    'Events passed back to the form
    Event GroupValveClick(ByVal nValve As Integer, ByVal nCurState As Boolean) Implements CCToonUctrl.GroupValveClick
    Event SharedValveClick(ByVal nValve As Integer, ByVal nCurState As Boolean) Implements CCToonUctrl.SharedValveClick
    'Current valve status
    Private mnGroupValveStates As Integer = 0
    Private mnSharedValveStates As Integer = 0
    Private mbFeedBackLabels As Boolean = True 'CC screen will set false to hide flow and pressure labels
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
            Dim nSolvAirOutValveColor As uctrlValve.eValveColor
            Dim nSolvAirOutBackColor As System.Drawing.Color
            Dim nWashValveColor As uctrlValve.eValveColor
            Dim nWashBackColor As System.Drawing.Color
            Dim nHdrStkValveColor As uctrlValve.eValveColor
            Dim nHdrStkBackColor As System.Drawing.Color
            Dim nMixValveColor As uctrlValve.eValveColor
            Dim nMixBackColor As System.Drawing.Color
            Dim nBellManValveColor As uctrlValve.eValveColor
            Dim nBellManBackColor As System.Drawing.Color

            'NVSP 11/03/2016 piggable stack changes
            'start
            Dim nStkA_BackColor As System.Drawing.Color = Color.Black
            Dim nStkA_ValveColor As uctrlValve.eValveColor

            'Get valves attached to a header first
            'NVSP 11/03/2016
            'Stack A

            'NRU 161215 Fake pig CE echo on group valve not shared valve
            UctrlGroupValve16.ValvState = (GetBitState(mnGroupValveStates, 16) = 1) 'Stack A CE
            UctrlGroupValve17.ValvState = (GetBitState(mnGroupValveStates, 17) = 1) 'Stack A Air
            UctrlGroupValve18.ValvState = (GetBitState(mnGroupValveStates, 18) = 1) 'Stack A Sol
            'UctrlGroupValve19.ValvState = (GetBitState(mnGroupValveStates, 19) = 1) 'Stack A EN
            UctrlGroupValve20.ValvState = (GetBitState(mnGroupValveStates, 20) = 1) 'Stack A Dump

            'Stack A Air
            If (GetBitState(mnGroupValveStates, 17) = 1) Then
                nStkA_ValveColor = uctrlValve.eValveColor.Blue
                nStkA_BackColor = Color.Blue
                UctrlGroupValve17.ValveColor = nStkA_ValveColor
            End If
            'Stack A Solvent
            If (GetBitState(mnGroupValveStates, 18) = 1) Then
                nStkA_ValveColor = uctrlValve.eValveColor.Red
                nStkA_BackColor = Color.Red
                UctrlGroupValve18.ValveColor = nStkA_ValveColor
            End If

            'Stack A CE
            If (GetBitState(mnGroupValveStates, 16) = 1) Then
                nStkA_ValveColor = uctrlValve.eValveColor.Purple
                nStkA_BackColor = Color.FromArgb(255, 0, 255)
                UctrlGroupValve16.ValveColor = nStkA_ValveColor
            End If
            lblLineColStkA_1.BackColor = nStkA_BackColor
            lblLineColStkA_2.BackColor = nStkA_BackColor
            lblLineColStkA_3.BackColor = nStkA_BackColor
            lblLineColStkA_4.BackColor = nStkA_BackColor
            lblLineColStkA_Dmp1.BackColor = nStkA_BackColor

            'Stack A Dump
            If (GetBitState(mnGroupValveStates, 20) = 1) Then
                UctrlGroupValve20.ValveColor = nStkA_ValveColor
                lblLineColStkA_Dmp2.BackColor = nStkA_BackColor
            Else
                UctrlGroupValve20.ValveColor = uctrlValve.eValveColor.Empty
                lblLineColStkA_Dmp2.BackColor = Color.Black
            End If
            'NRU 161215 No Enable, just the standard robot CE which includes the pig slice
            'Stack A Enable
            'UctrlSharedValve3.ValvState = (GetBitState(mnSharedValveStates, 3) = 1)
            'If (GetBitState(mnGroupValveStates, 19) = 1) Then
            '    UctrlGroupValve19.ValveColor = nStkA_ValveColor
            '    nColStkValveColor = nStkA_ValveColor
            '    nColStkBackColor = nStkA_BackColor
            '    lblLineColStkA_3.BackColor = nStkA_BackColor
            'End If

            ''Color Enable
            'If (GetBitState(mnSharedValveStates, 3) = 1) Then
            '    nColStkValveColor = uctrlValve.eValveColor.Purple
            '    nColStkBackColor = Color.FromArgb(255, 0, 255)
            '    lblLineColStkA_3.BackColor = Color.FromArgb(255, 0, 255)
            'Else
            '    UctrlSharedValve3.ValvState = False
            'End If

            'If Not (GetBitState(mnGroupValveStates, 19) = 1) And Not (GetBitState(mnSharedValveStates, 3) = 1) Then
            '    UctrlGroupValve19.ValveColor = uctrlValve.eValveColor.Empty
            '    lblLineColStkA_3.BackColor = Color.Black
            '    nColStkValveColor = uctrlValve.eValveColor.Empty
            '    nColStkBackColor = Color.Black
            'End If

            'NVSP 11/03/2016 piggable stack changes end 

            'App Cleaner Air
            If (GetBitState(mnGroupValveStates, 28) = 1) Then
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
            If (GetBitState(mnGroupValveStates, 29) = 1) Then
                UctrlGroupValve29.ValvState = True
                lblLineACS.BackColor = Color.Red
            Else
                UctrlGroupValve29.ValvState = False
                lblLineACS.BackColor = Color.Black
            End If
            'ACVA
            If (GetBitState(mnGroupValveStates, 30) = 1) Then
                UctrlGroupValve30.ValvState = True
                lblLineACVA.BackColor = Color.Blue
            Else
                UctrlGroupValve30.ValvState = False
                lblLineACVA.BackColor = Color.Black
            End If
            'Seal Air
            If (GetBitState(mnGroupValveStates, 14) = 1) Then
                UctrlGroupValve14.ValvState = True
                lblLineSealAir.BackColor = Color.Blue
            Else
                UctrlGroupValve14.ValvState = False
                lblLineSealAir.BackColor = Color.Black
            End If


            'Pump Flush
            If (GetBitState(mnGroupValveStates, 7) = 1) Then
                UctrlGroupValve7.ValvState = True
                lblLineFlush.BackColor = Color.Red
            Else
                UctrlGroupValve7.ValvState = False
                lblLineFlush.BackColor = Color.Black
            End If
            'Pump Flush 2
            If (GetBitState(mnGroupValveStates, 11) = 1) Then
                UctrlGroupValve11.ValvState = True
                lblLineFlush2_1.BackColor = Color.Red
                lblLineFlush2_2.BackColor = Color.Red
            Else
                UctrlGroupValve11.ValvState = False
                lblLineFlush2_1.BackColor = Color.Black
                lblLineFlush2_2.BackColor = Color.Black
            End If
            'Color Enable
            If (GetBitState(mnSharedValveStates, 3) = 1 Or GetBitState(mnGroupValveStates, 19) = 1) Then 'NVSP 11/03/2016 piggable changes
                If GetBitState(mnSharedValveStates, 3) = 1 Then
                    UctrlSharedValve3.ValvState = True
                    nColStkValveColor = uctrlValve.eValveColor.Purple
                    nColStkBackColor = Color.FromArgb(255, 0, 255)
                End If
            Else
                UctrlSharedValve3.ValvState = False
                nColStkValveColor = uctrlValve.eValveColor.Empty
                nColStkBackColor = Color.Black
            End If

            'Purge Air
            UctrlGroupValve4.ValvState = (GetBitState(mnGroupValveStates, 4) = 1)
            'Purge Solvent
            UctrlGroupValve3.ValvState = (GetBitState(mnGroupValveStates, 3) = 1)
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
            'Everything attached to the solvair
            UctrlGroupValve5.ValveColor = nSolvAirValveColor 'PCC
            UctrlGroupValve10.ValveColor = nSolvAirValveColor 'p2T
            'Lines
            lblLineSolvAir.BackColor = nSolvAirBackColor
            lblLineSolvAir1.BackColor = nSolvAirBackColor
            lblLineSolvAir2.BackColor = nSolvAirBackColor

            'p2T - 
            If (GetBitState(mnGroupValveStates, 10) = 1) Then
                UctrlGroupValve10.ValvState = True
                nSolvAirOutValveColor = nSolvAirValveColor
                nSolvAirOutBackColor = nSolvAirBackColor
            Else
                UctrlGroupValve10.ValvState = False
                nSolvAirOutValveColor = uctrlValve.eValveColor.Empty
                nSolvAirOutBackColor = Color.Black
            End If
            'Everything attached to p2T
            UctrlGroupValve13.ValveColor = nSolvAirOutValveColor 'Pump Flush
            UctrlGroupValve12.ValveColor = nSolvAirOutValveColor 'Inj Wash
            lblLineSolvAirOut1.BackColor = nSolvAirOutBackColor
            lblLineSolvAirOut2.BackColor = nSolvAirOutBackColor
            'Dump 3
            If (GetBitState(mnGroupValveStates, 12) = 1) Then
                UctrlGroupValve12.ValvState = True
                lblLineDump3_1.BackColor = nSolvAirOutBackColor
                lblLineDump3_2.BackColor = nSolvAirOutBackColor
            Else
                UctrlGroupValve12.ValvState = False
                lblLineDump3_1.BackColor = Color.Black
                lblLineDump3_2.BackColor = Color.Black
            End If
            'Wash 
            If (GetBitState(mnGroupValveStates, 13) = 1) Then
                UctrlGroupValve13.ValvState = True
                nWashValveColor = nSolvAirValveColor
                nWashBackColor = nSolvAirBackColor
            Else
                UctrlGroupValve13.ValvState = False
                nWashValveColor = uctrlValve.eValveColor.Empty
                nWashBackColor = Color.Black
            End If
            'Everything attached to Wash
            UctrlGroupValve1.ValveColor = nWashValveColor 'Inj Wash
            UctrlGroupValve2.ValveColor = nWashValveColor 'Bell Wash
            lblLineWashOut1.BackColor = nWashBackColor
            lblLineWashOut2.BackColor = nWashBackColor

            'pCC and color stack paint line
            If (GetBitState(mnGroupValveStates, 5) = 1) Then
                UctrlGroupValve5.ValvState = True
                nColStkBackColor = nSolvAirBackColor
                nColStkValveColor = nSolvAirValveColor
            Else
                UctrlGroupValve5.ValvState = False
                'Colors set in color enable logic near the top
            End If
            'Paint line and valves attached to it
            lblLineColStk1.BackColor = nColStkBackColor
            lblLineColStk2.BackColor = nColStkBackColor
            lblLineColStk3.BackColor = nColStkBackColor
            lblLineColStk4.BackColor = nColStkBackColor
            lblLineColStk5.BackColor = nColStkBackColor

            'Dump 2, Dump, vent on the color stack line
            UctrlGroupValve8.ValveColor = nColStkValveColor

            'Hdr Stack
            If (GetBitState(mnSharedValveStates, 2) = 1) Then
                UctrlSharedValve2.ValvState = True
                nHdrStkValveColor = uctrlValve.eValveColor.Purple
                nHdrStkBackColor = Color.FromArgb(255, 0, 255)
            Else
                UctrlSharedValve2.ValvState = False
                If (GetBitState(mnGroupValveStates, 9) = 1) Then
                    UctrlGroupValve9.ValvState = True
                    nHdrStkValveColor = uctrlValve.eValveColor.Red
                    nHdrStkBackColor = Color.Red
                Else
                    UctrlGroupValve9.ValvState = False
                    nHdrStkValveColor = uctrlValve.eValveColor.Empty
                    nHdrStkBackColor = Color.Black
                End If
            End If
            linHdrStack01.BackColor = nHdrStkBackColor
            linHdrStack02.BackColor = nHdrStkBackColor
            linHdrStack03.BackColor = nHdrStkBackColor

            'Dump 2
            If (GetBitState(mnGroupValveStates, 8) = 1) Then
                UctrlGroupValve8.ValvState = True
                lblLineDump2_1.BackColor = nColStkBackColor
                lblLineDump2_2.BackColor = nColStkBackColor

            Else
                UctrlGroupValve8.ValvState = False
                lblLineDump2_1.BackColor = Color.Black
                lblLineDump2_2.BackColor = Color.Black
            End If
            'Mix tube
            If nColStkBackColor = Color.Black Then
                nMixValveColor = nHdrStkValveColor
                nMixBackColor = nHdrStkBackColor
            Else
                nMixValveColor = nColStkValveColor
                nMixBackColor = nColStkBackColor
            End If
            lblLinMix01.BackColor = nMixBackColor
            lblLinMix02.BackColor = nMixBackColor

            UctrlGroupValve6.ValveColor = nMixValveColor
            UctrlGroupValve0.ValveColor = nMixValveColor
            'Dump
            If (GetBitState(mnGroupValveStates, 6) = 1) Then
                UctrlGroupValve6.ValvState = True
                lblLineDump1.BackColor = nMixBackColor
            Else
                UctrlGroupValve6.ValvState = False
                lblLineDump1.BackColor = Color.Black
            End If

            'PE valve state
            If (GetBitState(mnGroupValveStates, 0) = 1) Then
                UctrlGroupValve0.ValvState = True
            Else
                UctrlGroupValve0.ValvState = False
            End If
            'IW Valve state
            If (GetBitState(mnGroupValveStates, 1) = 1) Then
                UctrlGroupValve1.ValvState = True
                'IW and PE go to the same line.  If their both on I'm assuming IW wins
                nBellManBackColor = nWashBackColor
                nBellManValveColor = nWashValveColor
            Else
                UctrlGroupValve1.ValvState = False
                If (GetBitState(mnGroupValveStates, 0) = 0) Then
                    'Both off, set lines and trigger to black
                    nBellManBackColor = Color.Black
                    nBellManValveColor = uctrlValve.eValveColor.Empty
                Else
                    nBellManBackColor = nMixBackColor
                    nBellManValveColor = nMixValveColor
                End If
            End If
            lblLineBellMan1.BackColor = nBellManBackColor
            lblLineBellMan2.BackColor = nBellManBackColor
            'Trigger
            UctrlSharedValve0.ValveColor = nBellManValveColor
            UctrlSharedValve0a.ValveColor = nBellManValveColor
            UctrlSharedValve0b.ValveColor = nBellManValveColor
            If (GetBitState(mnSharedValveStates, 0) = 1) Then
                UctrlSharedValve0.ValvState = True
                UctrlSharedValve0a.ValvState = True
                UctrlSharedValve0b.ValvState = True
            Else
                UctrlSharedValve0.ValvState = False
                UctrlSharedValve0a.ValvState = False
                UctrlSharedValve0b.ValvState = False
            End If

            'NVSP 11/04/2016 Piggable stack - Line colors loading in white 
            For Each oControl As Control In Me.Controls
                If TypeOf oControl Is Label Then
                    Try
                        If String.Equals("lblLineColStk1", oControl.Name) Or String.Equals("lblLineColStk2", oControl.Name) Or String.Equals("lblLineColStk3", oControl.Name) _
                        Or String.Equals("lblLineColStk4", oControl.Name) Or String.Equals("lblLineColStk5", oControl.Name) Or String.Equals("lblLinMix01", oControl.Name) _
                        Or String.Equals("lblLinMix02", oControl.Name) Or String.Equals("lblLineColStkA_1", oControl.Name) Or String.Equals("lblLineColStkA_2", oControl.Name) _
                        Or String.Equals("lblLineColStkA_3", oControl.Name) Or String.Equals("lblLineColStkA_Dmp1", oControl.Name) _
                        Or String.Equals("lblLineColStkA_Dmp2", oControl.Name) Then
                            If (oControl.BackColor = Color.FromArgb(255, 255, 0, 0) Or oControl.BackColor = Color.Red _
                                Or oControl.BackColor = Color.FromArgb(255, 255, 0, 255) Or oControl.BackColor = Color.Magenta _
                                Or oControl.BackColor = Color.FromArgb(255, 0, 255, 0) Or oControl.BackColor = Color.Green _
                                Or oControl.BackColor = Color.FromArgb(255, 0, 0, 255) Or oControl.BackColor = Color.Blue) Then
                                'Do Nothing Leave the color as it is
                            Else
                                oControl.BackColor = Color.Black
                            End If
                        End If
                    Catch ex As Exception
                        Trace.WriteLine(ex.Message)
                        Exit For
                    End Try
                End If
            Next
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
        UctrlGroupValve4.ValveClick, UctrlGroupValve5.ValveClick, UctrlGroupValve6.ValveClick, UctrlGroupValve7.ValveClick, UctrlGroupValve14.ValveClick, UctrlGroupValve8.ValveClick, UctrlGroupValve9.ValveClick, _
        UctrlGroupValve11.ValveClick, UctrlGroupValve12.ValveClick, UctrlGroupValve10.ValveClick, UctrlGroupValve13.ValveClick, UctrlGroupValve15.ValveClick, _
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
    Private Sub UctrlSharedValve_Click(ByVal sender As Object) Handles UctrlSharedValve0.ValveClick, UctrlSharedValve0b.ValveClick, _
     UctrlSharedValve1.ValveClick, UctrlSharedValve2.ValveClick, UctrlSharedValve3.ValveClick, UctrlSharedValve4.ValveClick, UctrlSharedValve5.ValveClick 'NVSP 11/03/2016 Piggable stack changes Added 4 and 5 shared valves 
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
            lblFlowTag.Visible = mbFeedBackLabels
            lblOutPressure.Visible = mbFeedBackLabels
            lblOutPressureTag.Visible = mbFeedBackLabels
            lblOutPressure2.Visible = mbFeedBackLabels
            lblOutPressure2Tag.Visible = mbFeedBackLabels
            lblInPressure.Visible = mbFeedBackLabels
            lblInPressureTag.Visible = mbFeedBackLabels
            lblInPressure2.Visible = mbFeedBackLabels
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
            UctrlGroupValve4.ValveColor = uctrlValve.eValveColor.Blue 'pAIR
            UctrlGroupValve3.ValveColor = uctrlValve.eValveColor.Red 'pSOL
            UctrlGroupValve28.ValveColor = uctrlValve.eValveColor.Blue 'ACA
            UctrlGroupValve29.ValveColor = uctrlValve.eValveColor.Red 'ACS
            UctrlGroupValve30.ValveColor = uctrlValve.eValveColor.Blue 'ACVA
            UctrlGroupValve14.ValveColor = uctrlValve.eValveColor.Blue 'Seal
            UctrlSharedValve3.ValveColor = uctrlValve.eValveColor.Purple 'CE
            UctrlSharedValve2.ValveColor = uctrlValve.eValveColor.Purple 'HE
            UctrlGroupValve7.ValveColor = uctrlValve.eValveColor.Red 'Flush
            UctrlGroupValve11.ValveColor = uctrlValve.eValveColor.Red 'Flush2
            UctrlGroupValve9.ValveColor = uctrlValve.eValveColor.Red 'pSOL2
            lblFlow.Visible = mbFeedBackLabels
            lblFlowTag.Visible = mbFeedBackLabels
            lblOutPressure.Visible = mbFeedBackLabels
            lblOutPressureTag.Visible = mbFeedBackLabels
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub

End Class

