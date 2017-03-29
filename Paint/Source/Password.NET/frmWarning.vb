' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2012
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: Password frmWarning
'
' Description: Password application Auto Logout Warning form
' 
'
' Dependencies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Rick Olejniczak
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason
' 02/XX/2012    RJO     Initial code

Option Compare Text
Option Explicit On
Option Strict On

Public Class frmWarning

#Region " Declares "

#End Region

#Region " Properties "

    Friend WriteOnly Property Clock() As Long
        '********************************************************************************************
        'Description: Format and display the amount of time left before auto logout
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As Long)
            Dim lHours As Long = value \ 3600
            Dim lMinutes As Long = (value - (lHours * 3600)) \ 60
            Dim lSeconds As Long = value Mod 60

            lblClock.Text = Format(lHours, "00") & ":" & Format(lMinutes, "00") & ":" & Format(lSeconds, "00")
        End Set
    End Property

#End Region

#Region " Routines "

    Private Sub subInitFormText()
        '********************************************************************************************
        'Description: load text for form labels etc
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Me.Text = gpsRM.GetString("psWARN_CAP")
        btnReset.Text = gpsRM.GetString("psRESET")
        lblClock.Text = "00:00:00"
        lblMessage.Text = gpsRM.GetString("psRESET_MSG")
        lblTimeRemaining.Text = gpsRM.GetString("psWARN_MSG")

    End Sub

#End Region

#Region " Events "

    Private Sub btnReset_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReset.Click
        '********************************************************************************************
        'Description: The user clicked the Reset button. Reset the AutoLogout time.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Call frmMain.ResetAutoLogOut()
        Me.Close()

    End Sub

    Private Sub frmWarning_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description: Warn the user that Auto Logout will occur shortly unles Reset.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Call subInitFormText()

    End Sub

#End Region


End Class