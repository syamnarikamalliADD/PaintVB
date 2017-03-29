' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2006 - 2007
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: frmFind
'
' Description: Pop up box for a find function
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Speedy
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                                Version
'    04/21/14   MSW     1st version                                                           4.01.07.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Public Class frmFind
    Private mbPrevious As Boolean = False
    ReadOnly Property Backwards() As Boolean
        '********************************************************************************************
        'Description:  Backwards status.  
        '
        'Parameters: none
        'Returns:    True if previous button was pressed 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get
            Return mbPrevious
        End Get
    End Property

    Property SearchText() As String
        '********************************************************************************************
        'Description:  Access to the search text
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Get
            Return txtSearchText.Text
        End Get
        Set(ByVal value As String)
            If value = String.Empty Then
                btnPrev.Enabled = False
                btnNext.Enabled = False
            Else
                btnPrev.Enabled = True
                btnNext.Enabled = True
            End If
            txtSearchText.Text = value
        End Set
    End Property
    Private Sub btnPrev_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrev.Click
        '********************************************************************************************
        'Description:  Previous button.  Set the backwards status  and return
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        mbPrevious = True
        frmMain.subSearch(SearchText, mbPrevious)
    End Sub
    Private Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        '********************************************************************************************
        'Description:  Previous button.  Set the backwards status  and return
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        mbPrevious = False
        frmMain.subSearch(SearchText, mbPrevious)
    End Sub
    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        '********************************************************************************************
        'Description:  cancel button.  
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
    Private Sub subInitializeForm()
        '********************************************************************************************
        'Description:  Previous button.  Set the backwards status  and return
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Me.Text = gpsRM.GetString("psFIND_FRM_CAP")
        lblFind.Text = gpsRM.GetString("psFIND_FRM_CAP")
        btnCancel.Enabled = True
        btnNext.Enabled = (txtSearchText.Text <> String.Empty)
        btnNext.Text = gpsRM.GetString("psNEXT_BTN")
        btnPrev.Enabled = (txtSearchText.Text <> String.Empty)
        btnPrev.Text = gpsRM.GetString("psPREV_BTN")
        btnCancel.Text = gcsRM.GetString("csCANCEL")
    End Sub
    Private Sub frm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description:  init form
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        subInitializeForm()
        txtSearchText.Select()
    End Sub

    Private Sub txtTitle_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtSearchText.TextChanged
        '********************************************************************************************
        'Description:  text changed - enable buttons
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 
        btnNext.Enabled = (txtSearchText.Text <> String.Empty)
        btnPrev.Enabled = (txtSearchText.Text <> String.Empty)
    End Sub
End Class
