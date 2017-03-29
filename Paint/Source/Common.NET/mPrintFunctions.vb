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
' Form/Module: mPrintFunctions
'
' Description: Printer Routines
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
'    Date       By      Reason                                                        Version
'    03/17/07   gks     Onceover Cleanup
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Friend Module mPrintFunctions

#Region " Declares "

    Private msScreenName As String

#End Region
#Region " Properties "

    Friend Property ScreenName() As String
        '********************************************************************************************
        'Description:  allow frmMain to pass in it's screen name (msSCREEN_NAME)
        '
        'Parameters: none
        'Returns:    The screen name associated with frmMain
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msScreenName
        End Get
        Set(ByVal value As String)
            msScreenName = value
        End Set
    End Property

#End Region
#Region " Routines "

    Friend Function SelectPrinter() As Printing.PrinterSettings
        '********************************************************************************************
        'Description:  show Printer dialog
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim oPrint As New System.Windows.Forms.PrintDialog
        Dim oSettings As New Printing.PrinterSettings

        Try
            With oSettings
                .Copies = 1
                .PrintToFile = False
            End With
            With oPrint
                .PrinterSettings = oSettings
                .AllowPrintToFile = False
                .AllowSelection = False
                .ShowNetwork = True

                If .ShowDialog() = DialogResult.OK Then
                    Return oSettings
                Else
                    Return Nothing
                End If
            End With

        Catch ex As Exception
            Trace.WriteLine("Module: mPrintFunctions, Routine: SelectPrinter, Error: " & ex.Message)
            Trace.WriteLine("Module: mPrintFunctions, Routine: SelectPrinter, StackTrace: " & ex.StackTrace)

            Dim sTmp As String = gcsRM.GetString("csERROR") & vbCrLf & ex.Message
            Dim sVoid As String = String.Empty ' to fill the part where it should be writing to statusbar
            'on form main

            ShowErrorMessagebox(sTmp, ex, msScreenName, sVoid)

            Return Nothing
        End Try

    End Function

#End Region

End Module
