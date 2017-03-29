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
' Form/Module: ExcelUtil.vb
'
' Description: Provide a utility program to convert spreadsheets between XLS and XML formats with a simple command
'              so we don't have to link to excel in most of the projects
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: White
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    02/15/2012 MSW     first draft                                                   4.1.1.1
'    07/09/13   MSW     Update and standardize logos                                  4.01.05.00
'    04/03/14   MSW     Mark a version, split up folders for building block versions  4.01.07.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On
Imports System
Imports System.IO

Imports Microsoft.Office.Interop
Imports System.Windows.Forms.Application
Imports System.Windows.Forms
Imports System.drawing
Module ExcelUtil
    Private Enum eSaveType
        nXML = 1
        nXLS = 2
        nXLSPIC = 3
    End Enum
    Sub Main()
        Try
            'Start by getting the command passed in.
            Dim eOperation As eSaveType = eSaveType.nXLS
            Dim nStep As Integer = 0
            Dim sSource As String = String.Empty
            Dim sDest As String = String.Empty
            Dim sCmd3 As String = String.Empty
            Dim sCmd4 As String = String.Empty
            For Each sTmp As String In My.Application.CommandLineArgs
                Select Case nStep
                    Case 0
                        If sTmp = gs_XLS_TO_XML Then
                            eOperation = eSaveType.nXML
                            nStep = 1
                        ElseIf sTmp = gs_XML_TO_XLS Then
                            eOperation = eSaveType.nXLS
                            nStep = 1
                        ElseIf sTmp = gs_XML_TO_XLS_PIC Then
                            eOperation = eSaveType.nXLSPIC
                            nStep = 1
                        End If
                    Case 1
                        sSource = sTmp.Replace("""", "")
                        nStep = 2
                    Case 2
                        sDest = sTmp.Replace("""", "")
                        If eOperation = eSaveType.nXLS Or eOperation = eSaveType.nXML Then
                            Exit For
                        Else
                            nStep = 3
                        End If
                    Case 3
                        sCmd3 = sTmp.Replace("""", "")
                        nStep = 4
                    Case 4
                        sCmd4 = sTmp.Replace("""", "")
                        Exit For
                    Case Else
                End Select
            Next
            'start excel to open the csv and convert it.
            Dim oXl As Excel.Application
            Dim oWb As Excel.Workbook

            oXl = DirectCast(CreateObject("Excel.Application"), Excel.Application)
            oWb = oXl.Workbooks.Open(sSource)

            'In case excel doesn't want to overwrite same file
            Try

                'Prevent excel from asking too many questions.  The save dialog should ask if they want to overwrite
                oXl.DisplayAlerts = False
                If My.Computer.FileSystem.FileExists(sDest) Then
                    My.Computer.FileSystem.DeleteFile(sDest, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin, FileIO.UICancelOption.DoNothing)
                End If

                Dim oSheet As Excel.Worksheet = Nothing
                For Each oTmp As Excel.Worksheet In oWb.Sheets
                    oTmp.Cells.EntireColumn.AutoFit()
                    If oSheet Is Nothing Then
                        oSheet = oTmp
                    End If
                Next

                Select Case eOperation
                    Case eSaveType.nXLSPIC
                        oSheet.Range(sCmd3).Select()
                        Dim oPic As Image
                        oPic = Image.FromFile(sCmd4)
                        Clipboard.SetImage(oPic)
                        oSheet.Paste()
                        oWb.SaveAs(Filename:=sDest, FileFormat:=Excel.XlFileFormat.xlWorkbookNormal, ReadOnlyRecommended:=False, CreateBackup:=False)
                    Case eSaveType.nXLS
                        oWb.SaveAs(Filename:=sDest, FileFormat:=Excel.XlFileFormat.xlWorkbookNormal, ReadOnlyRecommended:=False, CreateBackup:=False)
                    Case eSaveType.nXML
                        oWb.SaveAs(Filename:=sDest, FileFormat:=Excel.XlFileFormat.xlXMLSpreadsheet, ReadOnlyRecommended:=False, CreateBackup:=False, ConflictResolution:=Excel.XlSaveConflictResolution.xlLocalSessionChanges)
                End Select

                'Save formatted xls
                oWb.Close(Excel.XlSaveAction.xlSaveChanges)
                'oXl.Application.Quit()
                oXl.Quit()
                oWb = Nothing
                oXl = Nothing

            Catch ex As Exception
                'Error saving in excel
                oWb.Close(Excel.XlSaveAction.xlDoNotSaveChanges)

                oXl.Application.Quit()
                oXl.Quit()
                oWb = Nothing
                oXl = Nothing
                Debug.Print(ex.Message)
                Dim oFileWriter As System.IO.StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(StartupPath & "\" & gs_EXCEL_UTIL_ERRLOG, False)
                oFileWriter.WriteLine(ex.Message)
                oFileWriter.Close()
            End Try


        Catch ex As Exception
            Debug.Print(ex.Message)
            Dim oFileWriter As System.IO.StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(StartupPath & "\" & gs_EXCEL_UTIL_ERRLOG, False)
            oFileWriter.WriteLine(ex.Message)
            oFileWriter.Close()
        End Try

    End Sub

End Module
