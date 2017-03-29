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
' Form/Module: ZipUtil.vb
'
' Description: Provide a utility program to zip and unzip files with a simple command
'              so we don't have to link to it in projects with simple needs
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
'    04/03/14   MSW     Mark a version, split up folders for building block versions  4.01.07.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On
Imports System
Imports System.IO
'dll for zip access: C:\paint\Vbapps\Ionic.Zip.dll
'See License at C:\paint\Source\Common.NET\DotNetZip License.txt
'Website http://dotnetzip.codeplex.com/
Imports Ionic.Zip
Imports System.Windows.Forms.Application

Module ZipUtil

    Sub Main()
        Try
            'Start by getting the command passed in.
            Dim nCommand As Integer = 0
            Dim sZip As String = String.Empty
            Dim sFolder As String = String.Empty
            For Each sTmp As String In My.Application.CommandLineArgs
                Select Case nCommand
                    Case 0
                        If sTmp = gs_ZIP_ALL Then
                            nCommand = 1
                        ElseIf sTmp = gs_UNZIP_ALL Then
                            nCommand = 11
                        End If
                    Case 1
                        sFolder = sTmp
                        nCommand = 2
                    Case 2
                        sZip = sTmp
                        Exit For
                    Case 11
                        sZip = sTmp
                        nCommand = 12
                    Case 12
                        sFolder = sTmp
                        Exit For
                    Case Else
                End Select
            Next
            Dim oZip As ZipFile = Nothing
            sZip = sZip.Replace("""", "")
            sFolder = sFolder.Replace("""", "")
            Select Case nCommand
                Case 2
                    oZip = New ZipFile
                    oZip.AddDirectory(sFolder)
                    oZip.Save(sZip)
                Case 12
                    oZip = New ZipFile(sZip)
                    oZip.ExtractAll(sFolder)
                Case Else
            End Select
            oZip.Dispose()
            oZip = Nothing
        Catch ex As Exception

            Debug.Print(ex.Message)
            Dim oFileWriter As System.IO.StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(StartupPath & "\" & gs_ZIP_UTIL_ERRLOG, False)
            oFileWriter.WriteLine(ex.Message)
            oFileWriter.Close()
        End Try

    End Sub

End Module
