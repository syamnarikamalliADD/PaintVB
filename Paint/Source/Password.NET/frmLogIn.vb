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
' Form/Module: Password frmLogin
'
' Description: Password application Login form
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

Option Compare Binary
Option Explicit On
Option Strict On

Public Class frmLogIn

#Region " Declares "

#End Region

#Region " Properties "

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

        btnCancel.Text = gcsRM.GetString("csCANCEL")
        btnOK.Text = gcsRM.GetString("csOK")
        lblMessage.Text = gpsRM.GetString("psLOGIN_MSG")
        Me.Text = gpsRM.GetString("psLOGIN_CAP")
        txtPassword.Text = String.Empty

    End Sub

#End Region

#Region " Events "

    Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        '********************************************************************************************
        'Description: log in canceled. close this form.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Me.Close()

    End Sub

    Private Sub btnOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOK.Click
        '********************************************************************************************
        'Description: Password entered. Pass it to frmMain and close this form.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        frmMain.Password = txtPassword.Text

    End Sub

    Private Sub frmLogIn_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description: initialize this form
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

    Private Sub txtPassword_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPassword.TextChanged
        '********************************************************************************************
        'Description: The password text box content changed. If it is not blank, enable the OK button.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        btnOK.Enabled = Strings.Len(txtPassword.Text) > 0

    End Sub

#End Region

End Class
