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
' Form/Module: uctrlHonda1K.vb'
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
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Public Class uctrlHonda1K
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
    Private Enum eHonda1KShared
        Trigger = 0
        Reserved2 = 1
        Reserved3 = 2
        ColorEnable = 3
    End Enum
    Private Enum eHonda1KGroup
        GunDump = 0
        Reserved2 = 1
        DumpGate = 2
        PurgeSolv = 3
        PurgeAir = 4
        Reserved6 = 5
        Dump = 6
        Flush = 7
        Reserved9 = 8
        Dump2 = 9
        Vent = 10
        Cup_SP = 11
        Cup_AH = 12
        Cup_AL = 13
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
            Dim nGDValveColor As uctrlValve.eValveColor
            Dim nGDBackColor As System.Drawing.Color
            Dim nTrigValveColor As uctrlValve.eValveColor
            Dim nTrigBackColor As System.Drawing.Color

            'Get valves attacched to a header first

            'App Cleaner Air
            If (GetBitState(mnGroupValveStates, eHonda1KGroup.ACA) = 1) Then
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
            If (GetBitState(mnGroupValveStates, eHonda1KGroup.ACS) = 1) Then
                UctrlGroupValve29.ValvState = True
                lblLineACS.BackColor = Color.Red
            Else
                UctrlGroupValve29.ValvState = False
                lblLineACS.BackColor = Color.Black
            End If
            'ACVA
            If (GetBitState(mnGroupValveStates, eHonda1KGroup.ACVA) = 1) Then
                UctrlGroupValve30.ValvState = True
                lblLineACVA.BackColor = Color.Blue
            Else
                UctrlGroupValve30.ValvState = False
                lblLineACVA.BackColor = Color.Black
            End If
            'Purge Air
            UctrlGroupValve4.ValvState = (GetBitState(mnGroupValveStates, eHonda1KGroup.PurgeAir) = 1)
            'Purge Solvent
            UctrlGroupValve3.ValvState = (GetBitState(mnGroupValveStates, eHonda1KGroup.PurgeSolv) = 1)
            'Select the valve and line colors after the solvair
            If UctrlGroupValve3.ValvState Then
                nColStkValveColor = uctrlValve.eValveColor.Red
                nColStkBackColor = Color.Red
            Else
                If UctrlGroupValve4.ValvState Then
                    nColStkValveColor = uctrlValve.eValveColor.Blue
                    nColStkBackColor = Color.Blue
                Else
                    nColStkValveColor = uctrlValve.eValveColor.Empty
                    nColStkBackColor = Color.Black
                End If
            End If
            'Color Enable
            If (GetBitState(mnSharedValveStates, eHonda1KShared.ColorEnable) = 1) Then
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

            'Cup A-H
            UctrlGroupValve12.ValvState = (GetBitState(mnGroupValveStates, eHonda1KGroup.Cup_AH) = 1)
            'Cup A-L
            UctrlGroupValve13.ValvState = (GetBitState(mnGroupValveStates, eHonda1KGroup.Cup_AL) = 1)
            'Cup S-P
            UctrlGroupValve11.ValvState = (GetBitState(mnGroupValveStates, eHonda1KGroup.Cup_SP) = 1)
            'Select the valve and line colors after the solvair
            If UctrlGroupValve11.ValvState Then
                nSolvAirValveColor = uctrlValve.eValveColor.Red
                nSolvAirBackColor = Color.Red
            Else
                nSolvAirValveColor = uctrlValve.eValveColor.Empty
                nSolvAirBackColor = Color.Black
                If UctrlGroupValve12.ValvState Or UctrlGroupValve13.ValvState Then
                    nSolvAirValveColor = uctrlValve.eValveColor.Blue
                    nSolvAirBackColor = Color.Blue
                End If
            End If
            'Everything attached to the solvair
            UctrlGroupValve0.ValveColor = nSolvAirValveColor 'Gun Dump
            UctrlGroupValve2.ValveColor = nSolvAirValveColor 'Dump Gate
            'Lines
            lblLineSolvAirOut1.BackColor = nSolvAirBackColor
            lblLineSolvAirOut2.BackColor = nSolvAirBackColor
            lblLineSolvAirout3.BackColor = nSolvAirBackColor
            lblLineSolvAirout3.BackColor = nSolvAirBackColor

            'Dump Gate
            If (GetBitState(mnGroupValveStates, eHonda1KGroup.DumpGate) = 1) Then
                UctrlGroupValve2.ValvState = True
                lblLineDG1.BackColor = nSolvAirBackColor
                lblLineDG2.BackColor = nSolvAirBackColor
            Else
                UctrlGroupValve2.ValvState = False
                lblLineDG1.BackColor = Color.Black
                lblLineDG2.BackColor = Color.Black
            End If

            'Pump Flush
            If (GetBitState(mnGroupValveStates, eHonda1KGroup.Flush) = 1) Then
                UctrlGroupValve7.ValvState = True
                lblLinePF.BackColor = Color.Red
            Else
                UctrlGroupValve7.ValvState = False
                lblLinePF.BackColor = Color.Black
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
            'Vent
            If (GetBitState(mnGroupValveStates, eHonda1KGroup.Vent) = 1) Then 'Dump
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
            If (GetBitState(mnGroupValveStates, eHonda1KGroup.Dump2) = 1) Then
                UctrlGroupValve9.ValvState = True
                lblLineDump2_1.BackColor = nColStkBackColor
                lblLineDump2_2.BackColor = nColStkBackColor
            Else
                UctrlGroupValve9.ValvState = False
                lblLineDump2_1.BackColor = lblVentLine1.BackColor
                lblLineDump2_2.BackColor = lblVentLine1.BackColor
            End If
            'Gun Dump Valve state
            If (GetBitState(mnGroupValveStates, eHonda1KGroup.GunDump) = 1) Then
                UctrlGroupValve0.ValvState = True
                'IW and PE go to the same line.  If their both on I'm assuming IW wins
                nGDBackColor = nSolvAirBackColor
                nGDValveColor = nSolvAirValveColor
                UctrlGroupValve0.ValveColor = nGDValveColor 'Gun Dump
            Else
                UctrlGroupValve0.ValvState = False
                'PE set to color stack color
                UctrlGroupValve0.ValveColor = nSolvAirValveColor
                If UctrlSharedValve3.ValvState = False And UctrlGroupValve3.ValvState = False And _
                    UctrlGroupValve4.ValvState = False Then
                    'Both off, set lines and trigger to black
                    nGDValveColor = uctrlValve.eValveColor.Empty
                    nGDBackColor = Color.Black
                End If
            End If
            If UctrlSharedValve3.ValvState = False And UctrlGroupValve3.ValvState = False And _
                    UctrlGroupValve4.ValvState = False Then
                lblLineBellMan1.BackColor = nGDBackColor
                UctrlGroupValve6.ValveColor = nGDValveColor
                lblLineColStk2.BackColor = nGDBackColor
                lblLineColStk3.BackColor = nGDBackColor
                lblLineColStk4.BackColor = nGDBackColor
                lblLineDump1.BackColor = nGDBackColor
                nTrigValveColor = nGDValveColor
                nTrigBackColor = nGDBackColor
            Else
                lblLineBellMan1.BackColor = nColStkBackColor
                UctrlGroupValve6.ValveColor = nColStkValveColor
                lblLineColStk2.BackColor = nColStkBackColor
                lblLineColStk3.BackColor = nColStkBackColor
                lblLineColStk4.BackColor = nColStkBackColor
                lblLineDump1.BackColor = nColStkBackColor
                nTrigValveColor = nColStkValveColor
                nTrigBackColor = nColStkBackColor
            End If
            'Dump
            If (GetBitState(mnGroupValveStates, eHonda1KGroup.Dump) = 1) Then
                UctrlGroupValve6.ValvState = True
            Else
                UctrlGroupValve6.ValvState = False
                lblLineDump1.BackColor = Color.Black
            End If

            'Trigger
            UctrlSharedValve0.ValveColor = nTrigValveColor
            UctrlSharedValve0a.ValveColor = nTrigValveColor
            UctrlSharedValve0b.ValveColor = nTrigValveColor
            If (GetBitState(mnSharedValveStates, eHonda1KShared.Trigger) = 1) Then
                UctrlSharedValve0.ValvState = True
                UctrlSharedValve0a.ValvState = True
                UctrlSharedValve0b.ValvState = True
                lblLineInjector.BackColor = nTrigBackColor
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
    Private Sub UctrlGroupValve_Click(ByVal sender As Object) Handles UctrlGroupValve0.ValveClick, UctrlGroupValve1.ValveClick, UctrlGroupValve2.ValveClick, UctrlGroupValve11.ValveClick, _
        UctrlGroupValve4.ValveClick, UctrlGroupValve12.ValveClick, UctrlGroupValve6.ValveClick, UctrlGroupValve7.ValveClick, UctrlGroupValve8.ValveClick, UctrlGroupValve9.ValveClick, UctrlGroupValve10.ValveClick, _
        UctrlGroupValve3.ValveClick, UctrlGroupValve5.ValveClick, UctrlGroupValve13.ValveClick, UctrlGroupValve14.ValveClick, UctrlGroupValve15.ValveClick, _
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
            UctrlGroupValve3.ValveColor = uctrlValve.eValveColor.Red 'Purge Solvent
            UctrlGroupValve4.ValveColor = uctrlValve.eValveColor.Blue 'Purge Air
            UctrlGroupValve7.ValveColor = uctrlValve.eValveColor.Red 'Pump Flush
            UctrlGroupValve13.ValveColor = uctrlValve.eValveColor.Blue 'pCup A-L
            UctrlGroupValve12.ValveColor = uctrlValve.eValveColor.Blue 'pCup A-H
            UctrlGroupValve11.ValveColor = uctrlValve.eValveColor.Red 'pCup S-P
            UctrlGroupValve28.ValveColor = uctrlValve.eValveColor.Blue 'ACA
            UctrlGroupValve29.ValveColor = uctrlValve.eValveColor.Red 'ACS
            UctrlGroupValve30.ValveColor = uctrlValve.eValveColor.Blue 'ACVA
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

