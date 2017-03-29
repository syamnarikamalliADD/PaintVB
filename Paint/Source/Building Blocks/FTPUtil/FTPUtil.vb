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
' Form/Module: FTPUtil
'
' Description: console app to run simple FTP get from a robot
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
'    04/03/14   MSW     New                                                          4.01.07.00
'********************************************************************************************
Module FTPUtil


    Public Function sGetFileName(ByVal sPath As String, ByVal sFile As String, ByVal sExt As String) As String
        '********************************************************************************************
        'Description:  generate an available temp file name 
        '
        'Parameters: sFile - starting name, sExt extension if required
        'Returns:    %TEMP%/sFile{number if needed}.sExt
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/30/12  MSW     Make sure it gets a unique directory name if there's no extension
        ' 12/13/12  MSW     prevent "\\"
        '********************************************************************************************
        Dim nTmpFileIndex As Integer = 0
        If sPath.Substring(sPath.Length - 1) = "\" Then
            sPath = sPath.Substring(0, sPath.Length - 1)
        End If
        Dim sTmpFile As String = sPath & "\" & sFile & sExt
        Do While My.Computer.FileSystem.FileExists(sTmpFile) OrElse My.Computer.FileSystem.DirectoryExists(sTmpFile)
            sTmpFile = sPath & "\" & sFile & nTmpFileIndex.ToString & sExt
            nTmpFileIndex += 1
        Loop
        Return sTmpFile
    End Function

    Sub Main()
        Dim sHostname As String = String.Empty
        Dim sDevice As String = String.Empty
        Dim sFile As String = String.Empty
        Dim sPath As String = IO.Path.GetDirectoryName(Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
        Dim sOutputFile As String = String.Empty
        Dim FileWriter As System.IO.StreamWriter = Nothing
        Dim bOutput As Boolean = False
        Dim bContinue As Boolean = True
        Dim bWait As Boolean = True
        Try
            For Each sTmp In My.Application.CommandLineArgs
                If sTmp.StartsWith("-") Then
                    If sTmp.Substring(2, 1) = "=" Then
                        Select Case sTmp.Substring(1, 1)
                            Case "d"
                                sDevice = sTmp.Substring(3)
                            Case "f"
                                sFile = sTmp.Substring(3)
                            Case "p"
                                sPath = sTmp.Substring(3)
                            Case "n"
                                bWait = False
                        End Select
                    End If
                Else
                    sHostname = sTmp
                End If
            Next
        Catch ex As Exception
            Console.WriteLine("Error reading command line arguments")
            Console.WriteLine("FTPUtil HOSTNAME {-d=DEVICE} -f=FILENAME {-p=PATH}")
            bContinue = False
        End Try
        If bContinue Then
            Try
                If (IO.Directory.Exists(sPath) = False) Then
                    Console.WriteLine("Creating path:" & sPath)
                    IO.Directory.CreateDirectory(sPath)
                End If
            Catch ex As Exception
                Console.WriteLine("ERROR: Could not access path:" & sPath)
                bContinue = False
            End Try
        End If
        If bContinue Then
            Try
                sOutputFile = sGetFileName(sPath, "FTPResult", ".txt")
                FileWriter = New System.IO.StreamWriter(sOutputFile)
                bOutput = True
            Catch ex As Exception
                Console.WriteLine("Could not open results file:" & sOutputFile)
            End Try
            Try
                If (sHostname <> String.Empty) AndAlso (sFile <> String.Empty) Then
                    Dim oFTP As clsFSFtp = Nothing
                    Try

                        oFTP = New clsFSFtp(sHostname)
                        With oFTP
                            If .Connected Then
                                If sDevice <> String.Empty Then
                                    .WorkingDir = sDevice
                                End If
                                .DestDir = sPath
                                If .GetFile(sFile, sFile) Then
                                    'Good
                                    Console.WriteLine("SUCCESS:")
                                    Console.WriteLine("HOSTNAME=" & sHostname)
                                    Console.WriteLine("DEVICE=" & sDevice)
                                    Console.WriteLine("FILENAME=" & sFile)
                                    Console.WriteLine("PATH=" & sPath)
                                    If bOutput Then
                                        FileWriter.WriteLine("SUCCESS:")
                                        FileWriter.WriteLine("HOSTNAME=" & sHostname)
                                        FileWriter.WriteLine("DEVICE=" & sDevice)
                                        FileWriter.WriteLine("FILENAME=" & sFile)
                                        FileWriter.WriteLine("PATH=" & sPath)
                                    End If
                                Else
                                    Console.WriteLine("ERROR: Could not get file")
                                    Console.WriteLine("FTP ERROR: " & .ErrorMsg)
                                    Console.WriteLine("HOSTNAME=" & sHostname)
                                    Console.WriteLine("DEVICE=" & sDevice)
                                    Console.WriteLine("FILENAME=" & sFile)
                                    Console.WriteLine("PATH=" & sPath)
                                    If bOutput Then
                                        FileWriter.WriteLine("ERROR: Could not get file")
                                        FileWriter.WriteLine("FTP ERROR: " & .ErrorMsg)
                                        FileWriter.WriteLine("HOSTNAME=" & sHostname)
                                        FileWriter.WriteLine("DEVICE=" & sDevice)
                                        FileWriter.WriteLine("FILENAME=" & sFile)
                                        FileWriter.WriteLine("PATH=" & sPath)
                                    End If
                                End If
                            End If
                        End With
                    Catch ex As Exception
                        Console.WriteLine("ERROR: " & ex.Message)
                        Console.WriteLine("HOSTNAME=" & sHostname)
                        Console.WriteLine("DEVICE=" & sDevice)
                        Console.WriteLine("FILENAME=" & sFile)
                        Console.WriteLine("PATH=" & sPath)
                        If bOutput Then
                            FileWriter.WriteLine("ERROR: " & ex.Message)
                            FileWriter.WriteLine("HOSTNAME=" & sHostname)
                            FileWriter.WriteLine("DEVICE=" & sDevice)
                            FileWriter.WriteLine("FILENAME=" & sFile)
                            FileWriter.WriteLine("PATH=" & sPath)
                        End If
                    End Try
                Else
                    Console.WriteLine("ERROR: Could not process command line arguments")
                    Console.WriteLine("FTPUtil HOSTNAME {-d=DEVICE} -f=FILENAME {-p=PATH}")
                    Console.WriteLine("HOSTNAME=" & sHostname)
                    Console.WriteLine("DEVICE=" & sDevice)
                    Console.WriteLine("FILENAME=" & sFile)
                    Console.WriteLine("PATH=" & sPath)
                    If bOutput Then
                        FileWriter.WriteLine("ERROR: Could not process command line arguments")
                        FileWriter.WriteLine("FTPUtil HOSTNAME {-d=DEVICE} -f=FILENAME {-p=PATH}")
                        FileWriter.WriteLine("HOSTNAME=" & sHostname)
                        FileWriter.WriteLine("DEVICE=" & sDevice)
                        FileWriter.WriteLine("FILENAME=" & sFile)
                        FileWriter.WriteLine("PATH=" & sPath)
                    End If
                End If

            Catch ex As Exception
                Console.WriteLine("ERROR: " & ex.Message)
                Console.WriteLine("HOSTNAME=" & sHostname)
                Console.WriteLine("DEVICE=" & sDevice)
                Console.WriteLine("FILENAME=" & sFile)
                Console.WriteLine("PATH=" & sPath)
                If bOutput Then
                    FileWriter.WriteLine("ERROR: " & ex.Message)
                    FileWriter.WriteLine("HOSTNAME=" & sHostname)
                    FileWriter.WriteLine("DEVICE=" & sDevice)
                    FileWriter.WriteLine("FILENAME=" & sFile)
                    FileWriter.WriteLine("PATH=" & sPath)
                End If
            End Try
        End If
        If FileWriter IsNot Nothing Then
            Try
                FileWriter.Close()
            Catch ex As Exception
            End Try
        End If
        If bWait Then
            Console.ReadKey()
        End If
    End Sub


End Module
