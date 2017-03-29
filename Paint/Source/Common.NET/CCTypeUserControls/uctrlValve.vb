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
' Form/Module: uctrlValve.vb
' Description: Valve control
'               handles all the drawing details and adjusts valve orientation at design time for easier setup.
'
' Dependancies:
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Matt White.
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
Public Class uctrlValve
    Public Enum eValveType
        'support off,on,red,blue,purple
        VertLeft
        VertRight
        HorzLeft
        HorzRight
        BellCup
        SprayPattern
        'support off,on,red,blue,purple,red_off,blue_off,purple_off
        VertLeft2
        VertRight2
        Gun
    End Enum
    Public Enum eValveColor
        Green = 0
        Empty = 0
        Red = 1
        Blue = 2
        Purple = 3
    End Enum
    Private mValveType As eValveType = eValveType.VertLeft
    Private mColor As eValveColor = eValveColor.Empty
    Private mState As Boolean = False
    Private msColorTag As String = "off"
    Private mbAutoFit As Boolean = True
    Private mbInAutoResize As Boolean = False
    Friend Event ValveClick(ByVal oMe As Object)


    '********************************************************************************************
    'Description: Property autofit - automatically resize for the picture type
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Property AutoFit() As Boolean
        Get
            Return mbAutoFit
        End Get
        Set(ByVal value As Boolean)
            mbAutoFit = value
            If mbAutoFit Then
                AutoResize()
            End If
        End Set
    End Property

    '********************************************************************************************
    'Description: Property ValveColor - The color going into the valve
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Property ValveColor() As eValveColor
        Get
            Return mColor
        End Get
        Set(ByVal value As eValveColor)
            If (mColor <> value) Then
                mColor = value
                SetTag()
                SelectImage()
            End If
        End Set
    End Property

    '********************************************************************************************
    'Description: Property ValveColor - Valve type - selects which image list to display.
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Property ValveType() As eValveType
        Get
            Return mValveType
        End Get
        Set(ByVal value As eValveType)
            If (mValveType <> value) Then
                mValveType = value
                If mbAutoFit Then
                    AutoResize()
                End If
                SelectImage()
            End If
        End Set
    End Property

    '********************************************************************************************
    'Description: Property ValveState - On or Off
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Property ValvState() As Boolean
        Get
            Return mState
        End Get
        Set(ByVal value As Boolean)
            If (mState <> value) Then
                mState = value
                SetTag()
                SelectImage()
            End If
        End Set
    End Property

    '********************************************************************************************
    'Description: Sub SelectImage update the image when the valve type is changed
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************

    Private Sub SelectImage()
        Try
            Select Case mValveType
                Case (eValveType.VertLeft)
                    picValve.Image = imlVlvVertLt.Images(msColorTag)
                Case (eValveType.VertRight)
                    picValve.Image = imlVlvVertRt.Images(msColorTag)
                Case (eValveType.HorzLeft)
                    picValve.Image = imlVlvHorzLt.Images(msColorTag)
                Case (eValveType.HorzRight)
                    picValve.Image = imlVlvHorzRt.Images(msColorTag)
                Case (eValveType.VertLeft2)
                    picValve.Image = imlVlvVertLt2.Images(msColorTag)
                Case (eValveType.VertRight2)
                    picValve.Image = imlVlvVertRt2.Images(msColorTag)
                Case (eValveType.Gun)
                    picValve.Image = imlGun.Images(msColorTag)
                Case (eValveType.BellCup)
                    picValve.Image = imlBell.Images(msColorTag)
                Case (eValveType.SprayPattern)
                    picValve.Image = imlSpray.Images(msColorTag)
                Case Else 'Just throw a picture up there until it's debugged.
                    picValve.Image = imlVlvVertLt.Images(msColorTag)
            End Select
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub

    '********************************************************************************************
    'Description: Sub SetTag - key used in the imagelists
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Private Sub SetTag()
        Try
            If mState Then
                Select Case mColor
                    Case eValveColor.Green
                        msColorTag = "on"
                    Case eValveColor.Blue
                        msColorTag = "blue"
                    Case eValveColor.Red
                        msColorTag = "red"
                    Case eValveColor.Purple
                        msColorTag = "purple"
                    Case Else
                        msColorTag = "on"
                End Select
            Else
                If (mValveType < eValveType.VertLeft2) Then
                    msColorTag = "off"
                Else
                    Select Case mColor
                        Case eValveColor.Empty
                            msColorTag = "off"
                        Case eValveColor.Blue
                            msColorTag = "blue_off"
                        Case eValveColor.Red
                            msColorTag = "red_off"
                        Case eValveColor.Purple
                            msColorTag = "purple_off"
                        Case Else
                            msColorTag = "off"
                    End Select
                End If
            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub
    '********************************************************************************************
    'Description: Sub AutoResize - Resize the control for each type of valve
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Private Sub AutoResize()
        Try
            mbInAutoResize = True
            Select Case mValveType
                Case (eValveType.VertLeft)
                    Me.Width = 32
                    Me.Height = 32
                Case (eValveType.VertRight)
                    Me.Height = 32
                    Me.Width = 32
                Case (eValveType.HorzLeft)
                    Me.Height = 32
                    Me.Width = 32
                Case (eValveType.HorzRight)
                    Me.Height = 32
                    Me.Width = 32
                Case (eValveType.VertLeft2)
                    Me.Width = 32
                    Me.Height = 32
                Case (eValveType.VertRight2)
                    Me.Height = 32
                    Me.Width = 32
                Case (eValveType.Gun)
                    Me.Height = 60
                    Me.Width = 50
                Case (eValveType.BellCup)
                    Me.Height = 35
                    Me.Width = 63
                Case (eValveType.SprayPattern)
                    Me.Height = 45
                    Me.Width = 85
                Case Else
                    Me.Height = 32
                    Me.Width = 32
            End Select
            mbInAutoResize = False
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub

    '********************************************************************************************
    'Description: load - init the tag, set the size, and set the picture
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Private Sub uctrlValve_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        SetTag()
        If mbAutoFit Then
            AutoResize()
        End If
        SelectImage()
    End Sub

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)

    End Sub

    '********************************************************************************************
    'Description: Mouse click event, pass out to the owner of the user control.
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Private Sub picValve_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles picValve.Click
        Try
            RaiseEvent ValveClick(Me)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub

End Class
