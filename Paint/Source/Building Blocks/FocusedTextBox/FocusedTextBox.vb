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
' Form/Module: FocusedTextBox
'
' Description: Enhancements on the regular textbox
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
'    01/11/11   MSW     Make negative values a little easier to enter
'    07/09/13   MSW     Update and standardize logos                                  4.01.05.00
'    10/03/13   MSW     Allow commas with numeric entries                             4.01.06.00
'    04/03/14   MSW     Mark a version, split up folders for building block versions  4.01.07.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Public Class FocusedTextBox
    Inherits TextBox

#Region " Declares"
    Private mbIsNumeric As Boolean = False
    Private mbIHaveFocus As Boolean = False
    Private mbAtLeft As Boolean = False
    Private mbAtRight As Boolean = False

    'these must be public
    Public Event UpArrow(ByRef sender As FocusedTextBox, ByVal bAlt As Boolean, _
                                                ByVal bShift As Boolean, ByVal bControl As Boolean)
    Public Event DownArrow(ByRef sender As FocusedTextBox, ByVal bAlt As Boolean, _
                                                ByVal bShift As Boolean, ByVal bControl As Boolean)
    Public Event LeftArrow(ByRef sender As FocusedTextBox, ByVal bAlt As Boolean, _
                                                ByVal bShift As Boolean, ByVal bControl As Boolean)
    Public Event RightArrow(ByRef sender As FocusedTextBox, ByVal bAlt As Boolean, _
                                                ByVal bShift As Boolean, ByVal bControl As Boolean)
#End Region
#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()
        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        MyBase.Text = String.Empty
        MyBase.TabStop = False
    End Sub

    'UserControl1 overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        components = New System.ComponentModel.Container
    End Sub

#End Region
#Region " Properties "
    Public Property NumericOnly() As Boolean
        '********************************************************************************************
        'Description: Allow ascii data?
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbIsNumeric
        End Get
        Set(ByVal Value As Boolean)
            mbIsNumeric = Value
        End Set
    End Property
#End Region
#Region " Routines "

#End Region
#Region " Events "
    Private Sub FocusedTextBox_EnabledChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                    Handles Me.EnabledChanged
        '********************************************************************************************
        'Description: 
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Me.BackColor = System.Drawing.Color.White
    End Sub
    Private Sub FocusedTextBox_Enter(ByVal sender As Object, _
                                ByVal e As System.EventArgs) Handles MyBase.Enter
        '********************************************************************************************
        'Description: 
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If Me.ReadOnly = False Then
            Me.BackColor = System.Drawing.Color.Yellow
            Me.SelectAll()
        End If
    End Sub
    Private Sub FocusedTextBox_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                Handles Me.GotFocus
        '********************************************************************************************
        'Description: 
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As TextBox = DirectCast(sender, TextBox)

        mbAtRight = False
        mbAtLeft = False

        If o.SelectionStart <> 0 Then
            o.SelectionStart = 0
            o.SelectAll()
        End If

    End Sub
    Private Sub FocusedTextBox_KeyUp(ByVal sender As Object, _
                                    ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyUp
        '********************************************************************************************
        'Description: 
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bA As Boolean = e.Alt
        Dim bS As Boolean = e.Shift
        Dim bC As Boolean = e.Control
        Dim o As TextBox = DirectCast(sender, TextBox)

        If Strings.Len(o.Text) = 0 Then Exit Sub

        Select Case e.KeyCode
            Case Keys.Up
                RaiseEvent UpArrow(Me, bA, bS, bC)
                e.Handled = True
            Case Keys.Down
                RaiseEvent DownArrow(Me, bA, bS, bC)
                e.Handled = True
            Case Keys.Right
                If o.SelectionStart = o.Text.Length Then
                    If mbAtRight Then
                        RaiseEvent RightArrow(Me, bA, bS, bC)
                        mbAtRight = False
                        e.Handled = True
                    Else
                        mbAtRight = True
                    End If
                End If
            Case Keys.Left
                If o.SelectionStart = 0 Then
                    If mbAtLeft Then
                        RaiseEvent LeftArrow(Me, bA, bS, bC)
                        mbAtLeft = False
                        e.Handled = True
                    Else
                        mbAtLeft = True
                    End If

                End If

        End Select
    End Sub
    Private Sub FocusedTextBox_KeyPress(ByVal sender As Object, _
            ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles MyBase.KeyPress
        '********************************************************************************************
        'Description: 
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/11/11  MSW     Make negative values a little easier to enter
        ' 10/03/13  MSW     Allow commas with numeric entries
        '********************************************************************************************

        If mbIsNumeric Then
            If Char.IsNumber(e.KeyChar) = False Then
                'look for . or -
                If Char.IsControl(e.KeyChar) = False Then
                    Select Case e.KeyChar.ToString
                        Case "."
                            'This only lets them put one . in at a time
                            If InStr(MyBase.Text, ".") > 0 Then
                                e.Handled = True
                            End If
                        Case ","
                            'This only lets them put one , in at a time
                            If InStr(MyBase.Text, ",") > 0 Then
                                e.Handled = True
                            End If
                        Case "-"
                            'better be first one
                            If Len(MyBase.Text) > 0 And (MyBase.SelectionStart > 0) Then
                                e.Handled = True
                            End If

                        Case Else
                            e.Handled = True

                    End Select
                End If
            End If
        End If

    End Sub
    Private Sub FocusedTextBox_Leave(ByVal sender As Object, _
                                ByVal e As System.EventArgs) Handles MyBase.Leave
        '********************************************************************************************
        'Description: 
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Me.BackColor = System.Drawing.Color.White

    End Sub
    Private Sub FocusedTextBox_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                        Handles Me.LostFocus
        '********************************************************************************************
        'Description: 
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbIHaveFocus = False
    End Sub
    Private Sub FocusedTextBox_MouseUp(ByVal sender As Object, _
                             ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        '********************************************************************************************
        'Description: 
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If Me.ReadOnly = False Then
            Me.BackColor = System.Drawing.Color.Yellow
            If mbIHaveFocus = False Then
                Me.SelectAll()
                mbIHaveFocus = True
            End If
        End If

    End Sub
    Private Sub FocusedTextBox_ReadOnlyChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                Handles Me.ReadOnlyChanged
        '********************************************************************************************
        'Description: 
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Me.BackColor = System.Drawing.Color.White
        Me.TabStop = Not Me.ReadOnly

    End Sub
    Private Sub FocusedTextBox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                    Handles Me.TextChanged
        '********************************************************************************************
        'Description: 
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If Me.ReadOnly = False Then
            Me.ForeColor = System.Drawing.Color.Red

        End If
    End Sub
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
#End Region

End Class
