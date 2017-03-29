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
' Form/Module: frmMultipleChoiceDlg
'
' Description: multiple choice dialog form
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: 
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'    08/30/13   MSW     Change the start position                                     4.01.05.00
'********************************************************************************************
Imports System.Windows.Forms

Public Class frmMultipleChoiceDlg
    Private mnButtonSelected As Integer = -1
    Private mnNumButtons As Integer = 0
    'Multiple choice dialog selection
    Private Sub btn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn0.Click, _
            btn1.Click, btn2.Click, btn3.Click
        Dim btnTmp As Button = DirectCast(sender, Button)
        mnButtonSelected = CInt(btnTmp.Name.Substring(3))
        Me.Close()
    End Sub

    Property label() As String
        Get
            Return lblText.Text
        End Get
        Set(ByVal value As String)
            lblText.Text = value
        End Set
    End Property
    Property caption() As String
        Get
            Return Me.Text
        End Get
        Set(ByVal value As String)
            Me.Text = value
        End Set
    End Property
    Property SelectedButton() As Integer
        Get
            Return mnButtonSelected
        End Get
        Set(ByVal value As Integer)
            If value < mnNumButtons And value >= -1 Then
                mnButtonSelected = value
            End If
        End Set
    End Property
    Property ButtonText(ByVal index As Integer) As String
        Get
            Dim nBtnIndex As Integer = index
            Dim btnTmp As Button = DirectCast(tlpButtons.Controls("btn" & nBtnIndex.ToString), Button)
            If btnTmp Is Nothing Then
                Debug.Assert(False) 'Shouldn't be here
                Return String.Empty
            Else
                Return btnTmp.Text
            End If
        End Get
        Set(ByVal value As String)
            Dim nBtnIndex As Integer = index
            Dim btnTmp As Button = DirectCast(tlpButtons.Controls("btn" & nBtnIndex.ToString), Button)
            If btnTmp Is Nothing Then
                Debug.Assert(False) 'Shouldn't be here
            Else
                btnTmp.Text = value
            End If
        End Set
    End Property
    Property NumButtons() As Integer
        Get
            Return mnNumButtons
        End Get
        Set(ByVal value As Integer)
            If value > 4 Then
                mnNumButtons = 4
            ElseIf value < 1 Then
                mnNumButtons = 1
            Else
                mnNumButtons = value
            End If
            For Each btnTmp As Button In tlpButtons.Controls
                Dim nIndex As Integer = CInt(btnTmp.Name.Substring(3))
                btnTmp.Visible = (nIndex < mnNumButtons)                
            Next
        End Set
    End Property
End Class
