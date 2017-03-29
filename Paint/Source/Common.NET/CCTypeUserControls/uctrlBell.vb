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
' Form/Module: uctrlBell.vb'
' Description: Bell 1k cartoon control
' 
'
' Dependancies:  mMathFunctions.vb, uctrValve.vb
'           System.Drawing.Color
' Language: Microsoft Visual Basic .Net 2008
'
' Author: HGB
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    04/23/09   MSW     first draft
'    04/13/11   HGB     Created from uctrlGun
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Public Class uctrlBell
    Implements CCToonUctrl
    'Private Enum ValveNum
    '    'Array starts at 0
    '    Trigger0 = 0
    '    Valve1 = 1
    '    Valve2 = 2
    '    ColorEnable3 = 3
    '    PaintEnable4 = 4
    '    InjectorWash5 = 5
    '    BellWash6 = 6
    '    PurgeSolvent7 = 7
    '    PurgeAir8 = 8
    '    PurgeColChg9 = 9
    '    Dump10 = 10
    '    PumpFlush11 = 11
    '    SealAir12 = 12
    '    DumpToo13 = 13
    '    Vent14 = 14
    '    Valve15 = 15
    '    Valve16 = 16
    '    Valve17 = 17
    '    Valve18 = 18
    '    Valve19 = 19
    '    LastValve = 19
    'End Enum
    'Max valve constants, counting from 0
    Const mnMaxShared As Integer = 3
    Const mnMaxGroup As Integer = 15
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

            Dim nColStkValveColor As uctrlValve.eValveColor
            Dim nColStkBackColor As System.Drawing.Color

            'Get valves attacched to a header first

            'App Cleaner Air
            If (GetBitState(mnGroupValveStates, 14) = 1) Then
                UctrlGroupValve14.ValvState = True
                lblLineACA.BackColor = Color.Blue
            Else
                UctrlGroupValve14.ValvState = False
                lblLineACA.BackColor = Color.Black
            End If
            'App Cleaner Solvent
            If (GetBitState(mnGroupValveStates, 15) = 1) Then
                UctrlGroupValve15.ValvState = True
                lblLineACS.BackColor = Color.Red
            Else
                UctrlGroupValve15.ValvState = False
                lblLineACS.BackColor = Color.Black
            End If

            'Purge Air
            UctrlGroupValve4.ValvState = (GetBitState(mnGroupValveStates, 4) = 1)
            'Purge Solvent
            UctrlGroupValve3.ValvState = (GetBitState(mnGroupValveStates, 3) = 1)
            'Color Enable
            UctrlSharedValve3.ValvState = (GetBitState(mnSharedValveStates, 3) = 1)
            nColStkValveColor = uctrlValve.eValveColor.Purple
            nColStkBackColor = Color.FromArgb(255, 0, 255)
            If (UctrlSharedValve3.ValvState) Then 'Color enable
                nColStkValveColor = uctrlValve.eValveColor.Purple
                nColStkBackColor = Color.FromArgb(255, 0, 255)
            Else
                If UctrlGroupValve3.ValvState Then 'Solvent
                    nColStkValveColor = uctrlValve.eValveColor.Red
                    nColStkBackColor = Color.Red
                Else
                    If UctrlGroupValve4.ValvState Then 'Air
                        nColStkValveColor = uctrlValve.eValveColor.Blue
                        nColStkBackColor = Color.Blue
                    Else 'All off
                        nColStkValveColor = uctrlValve.eValveColor.Empty
                        nColStkBackColor = Color.Black
                    End If
                End If
            End If

            'Lines
            'Regulator Override
            If (GetBitState(mnGroupValveStates, 7) = 1) Then
                UctrlGroupValve7.ValveColor = nColStkValveColor
                UctrlGroupValve7.ValvState = True
            Else
                UctrlGroupValve7.ValvState = False
            End If

            'color stack and paint line
            lblPaintLine.BackColor = nColStkBackColor
            lblColorStack.BackColor = nColStkBackColor
            'Trigger
            UctrlSharedValve0.ValveColor = nColStkValveColor

            If (GetBitState(mnSharedValveStates, 0) = 1) Then
                UctrlSharedValve0.ValvState = True
            Else
                UctrlSharedValve0.ValvState = False
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
        UctrlGroupValve11.ValveClick, UctrlGroupValve12.ValveClick, UctrlGroupValve13.ValveClick, UctrlGroupValve14.ValveClick, UctrlGroupValve15.ValveClick
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
    Private Sub UctrlSharedValve_Click(ByVal sender As Object) Handles UctrlSharedValve0.ValveClick, _
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
            UctrlGroupValve4.ValveColor = uctrlValve.eValveColor.Blue
            UctrlGroupValve3.ValveColor = uctrlValve.eValveColor.Red
            UctrlGroupValve14.ValveColor = uctrlValve.eValveColor.Blue
            UctrlGroupValve15.ValveColor = uctrlValve.eValveColor.Red
            UctrlGroupValve8.ValveColor = uctrlValve.eValveColor.Blue
            UctrlSharedValve3.ValveColor = uctrlValve.eValveColor.Purple
            lblFlow.Visible = mbFeedBackLabels
            lblFlowTag.Visible = mbFeedBackLabels
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub

End Class

