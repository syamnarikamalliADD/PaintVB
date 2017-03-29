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
' Form/Module: CleanSourceFolders
'
' Description: console app to clear out obj and bin folders and *.bak files from source folders
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Ricko
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                       Version
'    09/12/13   MSW     New                                                          4.01.05.00
'    04/03/14   MSW     Mark a version, split up folders for building block versions 4.01.07.00
'********************************************************************************************
Module CleanSourceFolders

    Private Sub subCleanupSource(ByRef sPath As String)
        '********************************************************************************************
        'Description:  Run a backup from the application tab
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/23/12  MSW     Clean up obj and bin folders from source code
        '********************************************************************************************
        'Just ignore errors.  If a file is in use it'll just skip to the next one
        Try
            Dim sFiles() As String = IO.Directory.GetFiles(sPath, "*.bak")
            For Each sFile As String In sFiles
                Try
                    Console.WriteLine("Deleting " & sFile)
                    IO.File.Delete(sFile)
                Catch ex As Exception
                End Try
            Next
            Dim sFolders() As String = IO.Directory.GetDirectories(sPath)
            For Each sFolder As String In sFolders
                Dim sTmpFolderSplit() As String = Split(sFolder.ToLower, "\")
                Select Case sTmpFolderSplit(sTmpFolderSplit.GetUpperBound(0))
                    Case "bin", "obj", "publish"
                        Try
                            Console.WriteLine("Deleting " & sFolder)
                            IO.Directory.Delete(sFolder, True)
                        Catch ex As Exception
                        End Try
                    Case Else
                        subCleanupSource(sFolder)
                End Select
            Next
        Catch ex As Exception
        End Try
    End Sub

    Sub Main()
        Dim sPath As String = String.Empty
        Dim bAbort As Boolean = False
        Dim bFound As Boolean = False
        Try
            For Each sTmp As String In My.Application.CommandLineArgs
                If IO.Directory.Exists(sTmp) Then
                    sPath = sTmp
                    Exit For
                Else
                    If sTmp <> String.Empty Then
                        'Error
                        bAbort = True
                        Console.WriteLine("Path Not Found")
                    End If
                End If
            Next
        Catch ex As Exception
            Console.WriteLine("Error reading command line arguments")
            bAbort = True
        End Try
        Try
            If bAbort = False Then
                If bFound = False Then
                    Try
                        Console.WriteLine(sPath)
                        sPath = IO.Path.GetDirectoryName(Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
                        Console.WriteLine(sPath & " " & sPath.ToLower.LastIndexOf("vbapps").ToString)
                        If sPath.ToLower.Contains("vbapps") Then
                            sPath = sPath.Substring(0, sPath.ToLower.LastIndexOf("vbapps"))
                            Console.WriteLine(sPath)
                        End If
                    Catch ex As Exception
                        Console.WriteLine("Error determining path")
                        bAbort = True
                    End Try
                End If
                Console.WriteLine(sPath)
                Try
                    If sPath.ToLower.Contains("source") Then
                        subCleanupSource(sPath)
                    Else
                        Dim sFolders() As String = IO.Directory.GetDirectories(sPath)
                        For Each sFolder As String In sFolders
                            If sFolder.ToLower.Contains("source") Then
                                subCleanupSource(sFolder)
                            End If
                        Next
                    End If
                Catch ex As Exception
                    Console.WriteLine("Error 2")
                    bAbort = True
                End Try
                If bAbort = False Then
                    Console.WriteLine("Complete")
                End If
            Else
                Console.WriteLine("Abort")
            End If
        Catch ex As Exception
            Console.WriteLine("Error 1")
        End Try
        Console.ReadKey()
    End Sub


End Module
