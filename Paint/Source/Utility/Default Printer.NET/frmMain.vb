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
' Form/Module: frmMain
'
' Description: Printer select in VB.NET.  Only tested with windows 7
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
'    01/16/2012 MSW     first draft                                                   4.01.01.01
'    01/24/12   MSW     Add resource files                                            4.01.01.02
'    02/15/12   MSW     Force 32 bit build for PCDK compatability                     4.01.01.03
'    03/28/12   MSW     Add calls to resources                                        4.01.03.00
'    04/20/12   MSW     frmMain_KeyDown-Add excape key handler                        4.01.03.01
'    04/16/13   MSW     Add Canadian language files                                   4.01.05.00
'    07/09/13   MSW     Update and standardize logos                                  4.01.05.01
'    11/21/13   MSW     Call Me.Show() during startup                                 4.01.06.00
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Imports VB = Microsoft.VisualBasic
Imports System.Drawing.Printing
Friend Class frmMain
    Inherits System.Windows.Forms.Form

    '******** Form Constants   *********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Public Const msSCREEN_NAME As String = "DefaultPrint"   ' <-- For password area change log etc.
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    'Private Const mnROWSPACE As Integer = 40 ' interval for rows of textboxes etc

    Private msCulture As String = "en-US" 'Default to English

    Private Sub Command1_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command1.Click

        'Shell(String.Format("PrintUI.EXE /y /n ""{0}""", lstPrinters.SelectedItem.ToString))
        Shell(String.Format("rundll32 printui.dll,PrintUIEntry /y /n ""{0}""", lstPrinters.SelectedItem.ToString))
    End Sub
    Friend WriteOnly Property Culture() As String
        '********************************************************************************************
        'Description:  Write to this property to change the screen language.
        '
        'Parameters: Culture String (ex. "en-US")
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As String)

            msCulture = value
            mLanguage.DisplayCultureString = value

            'Use current language text for screen labels
            Dim Void As Boolean = mLanguage.GetResourceManagers(String.Empty, _
                                                                msBASE_ASSEMBLY_LOCAL, _
                                                                String.Empty)


        End Set

    End Property

    Private Sub frmMain_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Debug.Print("DefaultPrint - frmMain_KeyDown")
        ' 04/20/12  MSW     frmMain_KeyDown-Add excape key handler                        4.01.03.01

        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode

                Case Keys.Escape

                    Me.Close()
                Case Else

            End Select
        End If
    End Sub
    Private Sub frmMain_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
        mLanguage.GetResourceManagers(String.Empty, msBASE_ASSEMBLY_LOCAL, _
                                      String.Empty)

        Label1.Text = gpsRM.GetString("psLABEL")
        Command1.Text = gpsRM.GetString("psCOMMAND")
        Me.Text = gpsRM.GetString("psFORM")
        Dim pkInstalledPrinters As String
        For i As Integer = 0 To PrinterSettings.InstalledPrinters.Count - 1
            pkInstalledPrinters = PrinterSettings.InstalledPrinters.Item(i)
            lstPrinters.Items.Add(pkInstalledPrinters)
            Dim pd As New PrintDocument()
            pd.PrinterSettings.PrinterName = pkInstalledPrinters
            If (pd.PrinterSettings.IsDefaultPrinter()) Then
                lstPrinters.SelectedIndex = i
            End If
        Next
        Me.Show()
    End Sub

End Class