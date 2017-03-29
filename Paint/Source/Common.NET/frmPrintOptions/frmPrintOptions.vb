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
' Form/Module: frmPrintOptions
'
' Description: print options form
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
'    12/01/11   MSW     Mark a version                                                4.01.01.00
'    01/06/14   MSW     Disable the form controlbox                                   4.01.06.00
'********************************************************************************************

Imports System.Windows.Forms

Public Class frmPrintOptions
    Friend Property SplitTableCheckBoxLabel() As String
        Get
            Return chkSplitLargeTables.Text
        End Get
        Set(ByVal value As String)
            chkSplitLargeTables.Text = value
        End Set
    End Property
    Friend Property SplitTableCheckBoxState() As Boolean
        Get
            Return chkSplitLargeTables.Checked
        End Get
        Set(ByVal value As Boolean)
            chkSplitLargeTables.Checked = value
        End Set
    End Property
    Friend Property MaxRowsLabel() As String
        Get
            Return lblMaxRows.Text
        End Get
        Set(ByVal value As String)
            lblMaxRows.Text = value
        End Set
    End Property
    Friend Property MaxRowsValue() As Integer
        Get
            Return CInt(nudMaxRows.Value)
        End Get
        Set(ByVal value As Integer)
            nudMaxRows.Value = value
        End Set
    End Property
    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        '********************************************************************************************
        'Description:  OK selected
        ' 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        '********************************************************************************************
        'Description:  give up, close the window
        ' 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub frmPrintOptions_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description:  init text on load
        ' 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        chkSplitLargeTables.Text = gcsRM.GetString("csSPLIT_LARGE_TABLES")
        lblMaxRows.Text = gcsRM.GetString("csMAX_ROWS")
        btnCancel.Text = gcsRM.GetString("csCANCEL")
        btnOK.Text = gcsRM.GetString("csOK")
        lblMaxRows.Enabled = chkSplitLargeTables.Checked
        nudMaxRows.Enabled = chkSplitLargeTables.Checked
        btnOK.Enabled = False
    End Sub

    Private Sub chkSplitLargeTables_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkSplitLargeTables.CheckedChanged
        '********************************************************************************************
        'Description:  value changed, enable OK button, max rows 
        ' 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        lblMaxRows.Enabled = chkSplitLargeTables.Checked
        nudMaxRows.Enabled = chkSplitLargeTables.Checked
        btnOK.Enabled = True
    End Sub

    Private Sub nudMaxRows_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles nudMaxRows.KeyUp
        '********************************************************************************************
        'Description:  value changed, enable OK button
        ' 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        btnOK.Enabled = True 'Enable OK button
    End Sub

    Private Sub nudMaxRows_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles nudMaxRows.ValueChanged
        '********************************************************************************************
        'Description:  value changed, enable OK button
        ' 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        btnOK.Enabled = True
    End Sub

End Class
