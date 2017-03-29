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
' Form/Module: uctrlVersabell3.vb'
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
'    11/14/13   BTK     Mark a version                                                4.01.01.00
'    05/29/14   CBZ     Added design-time switch bP250InUse. True = P-250,            4.01.01.01
'                       False = P700. Removes p2T valve for P-250. Hide pSEAL.
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Public Class uctrlVersabell3
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
            'CBZ 5/29/14 P250 don't have P2T valves for VB3
            Dim bP250InUse As Boolean = True

            'Get valves attacched to a header first

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
            If (GetBitState(mnGroupValveStates, 8) = 1) Then
                UctrlGroupValve8.ValvState = True
                lblLineSealAir.BackColor = Color.Blue
            Else
                UctrlGroupValve8.ValvState = False
                lblLineSealAir.BackColor = Color.Black
            End If

            'Color Enable
            If (GetBitState(mnSharedValveStates, 3) = 1) Then
                UctrlSharedValve3.ValvState = True
                nColStkValveColor = uctrlValve.eValveColor.Purple
                nColStkBackColor = Color.FromArgb(255, 0, 255)
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
            UctrlGroupValve13.ValveColor = nSolvAirValveColor 'p2T
            'Lines
            lblLineSolvAir.BackColor = nSolvAirBackColor
            lblLineSolvAir1.BackColor = nSolvAirBackColor
            lblLineSolvAir2.BackColor = nSolvAirBackColor

            'p2T - What's this stand for shouldn't be pWash or something like that?
            'CBZ 5/29/14 P250 don't have P2T valves for VB3
            If bP250InUse Then
                UctrlGroupValve13.Visible = False
                lblGroup13.Visible = False

                Dim sName As String = "lblLineSolvAir"
                Dim pnlTmp As Panel = DirectCast(Me.Controls("pnlMain"), Panel)
                Dim oLine As Label = Nothing

                oLine = DirectCast(Me.Controls(sName), Label)
                oLine.Height = 140
                nSolvAirOutValveColor = nSolvAirValveColor
                nSolvAirOutBackColor = nSolvAirBackColor
            Else
                If (GetBitState(mnGroupValveStates, 13) = 1) Then
                    UctrlGroupValve13.ValvState = True
                    nSolvAirOutValveColor = nSolvAirValveColor
                    nSolvAirOutBackColor = nSolvAirBackColor
                Else
                    UctrlGroupValve13.ValvState = False
                    nSolvAirOutValveColor = uctrlValve.eValveColor.Empty
                    nSolvAirOutBackColor = Color.Black
                End If
            End If
            'Everything attached to p2T
            UctrlGroupValve7.ValveColor = nSolvAirOutValveColor 'Pump Flush
            UctrlGroupValve1.ValveColor = nSolvAirOutValveColor 'Inj Wash
            UctrlGroupValve2.ValveColor = nSolvAirOutValveColor 'Bell Wash
            lblLineSolvAirOut1.BackColor = nSolvAirOutBackColor
            lblLineSolvAirOut2.BackColor = nSolvAirOutBackColor
            lblLineSolvAirout3.BackColor = nSolvAirOutBackColor

            'Pump Flush
            If (GetBitState(mnGroupValveStates, 7) = 1) Then
                UctrlGroupValve7.ValvState = True
                lblLinePF.BackColor = nSolvAirOutBackColor
            Else
                UctrlGroupValve7.ValvState = False
                lblLinePF.BackColor = Color.Black
            End If
            'Bell Wash
            If (GetBitState(mnGroupValveStates, 2) = 1) Then
                UctrlGroupValve2.ValvState = True
                lblLineBW1.BackColor = nSolvAirOutBackColor
                lblLineBW2.BackColor = nSolvAirOutBackColor
            Else
                UctrlGroupValve2.ValvState = False
                lblLineBW1.BackColor = Color.Black
                lblLineBW2.BackColor = Color.Black
            End If

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
            UctrlGroupValve9.ValveColor = nColStkValveColor
            UctrlGroupValve6.ValveColor = nColStkValveColor
            UctrlGroupValve10.ValveColor = nColStkValveColor
            'Dump
            If (GetBitState(mnGroupValveStates, 6) = 1) Then
                UctrlGroupValve6.ValvState = True
                lblLineDump1.BackColor = nColStkBackColor
            Else
                UctrlGroupValve6.ValvState = False
                lblLineDump1.BackColor = Color.Black
            End If
            'Vent
            If (GetBitState(mnGroupValveStates, 10) = 1) Then 'Dump
                UctrlGroupValve10.ValvState = True
                lblVentLine1.BackColor = nColStkBackColor
                lblVentLine2.BackColor = nColStkBackColor
                lblVentLine3.BackColor = nColStkBackColor
            Else
                UctrlGroupValve10.ValvState = False
                lblVentLine1.BackColor = Color.Black
                lblVentLine2.BackColor = Color.Black
                lblVentLine3.BackColor = Color.Black
            End If
            'Dump 2
            If (GetBitState(mnGroupValveStates, 9) = 1) Then
                UctrlGroupValve9.ValvState = True
                lblLineDump2_1.BackColor = nColStkBackColor
                lblLineDump2_2.BackColor = nColStkBackColor
            Else
                UctrlGroupValve9.ValvState = False
                lblLineDump2_1.BackColor = lblVentLine1.BackColor
                lblLineDump2_2.BackColor = lblVentLine1.BackColor
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
                nColStkBackColor = nSolvAirOutBackColor
                nColStkValveColor = nSolvAirOutValveColor
                UctrlGroupValve0.ValveColor = nColStkValveColor 'pPE
            Else
                UctrlGroupValve1.ValvState = False
                'PE set to color stack color
                UctrlGroupValve0.ValveColor = nColStkValveColor
                If UctrlGroupValve0.ValvState = False Then
                    'Both off, set lines and trigger to black
                    nColStkValveColor = uctrlValve.eValveColor.Empty
                    nColStkBackColor = Color.Black
                End If
            End If
            lblLineBellMan1.BackColor = nColStkBackColor
            lblLineBellMan2.BackColor = nColStkBackColor
            'Trigger
            UctrlSharedValve0.ValveColor = nColStkValveColor
            UctrlSharedValve0a.ValveColor = nColStkValveColor
            UctrlSharedValve0b.ValveColor = nColStkValveColor
            If (GetBitState(mnSharedValveStates, 0) = 1) Then
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
    Private Sub UctrlSharedValve_Click(ByVal sender As Object) Handles UctrlSharedValve0.ValveClick, UctrlSharedValve0b.ValveClick, _
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
            lblFlowTag.Visible = mbFeedBackLabels
            lblOutPressure.Visible = mbFeedBackLabels
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
            UctrlGroupValve4.ValveColor = uctrlValve.eValveColor.Blue 'pAIR
            UctrlGroupValve3.ValveColor = uctrlValve.eValveColor.Red 'pSOL
            UctrlGroupValve28.ValveColor = uctrlValve.eValveColor.Blue 'ACA
            UctrlGroupValve29.ValveColor = uctrlValve.eValveColor.Red 'ACS
            UctrlGroupValve30.ValveColor = uctrlValve.eValveColor.Blue 'ACVA
            UctrlGroupValve8.ValveColor = uctrlValve.eValveColor.Blue 'Seal
            UctrlSharedValve3.ValveColor = uctrlValve.eValveColor.Purple 'CE
            lblFlow.Visible = mbFeedBackLabels
            lblFlowTag.Visible = mbFeedBackLabels
            lblOutPressure.Visible = mbFeedBackLabels
            lblOutPressureTag.Visible = mbFeedBackLabels
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub

End Class

