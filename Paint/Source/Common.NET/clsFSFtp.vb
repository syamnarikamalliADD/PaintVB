' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2009
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: clsFSFtp
'
' Description: FTP File Transfer Class using FANUC FSFtp.dll
'
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Rick Olejniczak
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    11/21/11   MSW     GetFile - Add noWriteCache flag to keep it from copying an    4.01.01.00
'                       old diagnostic file 
'    04/30/16   MSW     GetFile - Move error message from constant to resource file   4.01.05.00
'    12/03/13   MSW     Add some error handling                                       4.01.06.00
'******************************************************************************************************

Imports System.Globalization
Imports XferFlags = FSFTPLib.FSEFTPtransferFlags

Friend Class clsFSFtp

#Region " Declares "

    '******** Property Variables   **********************************************************************
    Private mbConnected As Boolean
    Private mbEnableOverwrite As Boolean = True
    Private msDestDir As String = String.Empty
    Private msErrorMsg As String = String.Empty
    Private msHost As String = String.Empty
    Private msPassword As String = String.Empty
    Private msSourceDir As String = String.Empty
    Private msUserName As String = "anonymous"
    Private msWorkingDir As String = String.Empty
    Private Const msMODULE As String = "clsFSFtp"
    '******** End Property Variables   ******************************************************************

    '******** Class Variables   *************************************************************************
    Private moFTP As FSFTPLib.FSCFtp = Nothing
    '******** End Class Variables   *********************************************************************

    '******** Constants *********************************************************************************
    Private nPORT As Short = 21 'FTP Port
    '******** End Constants *****************************************************************************

#End Region

#Region " Properties "

    Friend ReadOnly Property Connected() As Boolean
        '********************************************************************************************
        'Description:  Returns True if FTP connected to Host.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return moFTP.IsFtpConnected
        End Get

    End Property

    Friend WriteOnly Property DestDir() As String
        '********************************************************************************************
        'Description:  The Directory/Folder we want to Write/Copy/Put files to.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As String)
            If Strings.Right(value, 1) <> "\" Then
                value = value & "\"
            End If
            msDestDir = value
        End Set

    End Property

    Friend WriteOnly Property EnableOverWrite() As Boolean
        '********************************************************************************************
        'Description:  Enable Overwrite of destination file during Get/put operations
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As Boolean)
            mbEnableOverwrite = value
        End Set

    End Property

    Friend ReadOnly Property ErrorMsg() As String
        '********************************************************************************************
        'Description:  Returns the last error message
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msErrorMsg
        End Get

    End Property

    Friend Property Host() As String
        '********************************************************************************************
        'Description:  Hostname of who we are connecting/connected to
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msHost
        End Get

        Set(ByVal value As String)
            msHost = value
        End Set

    End Property

    Friend WriteOnly Property Password() As String
        '********************************************************************************************
        'Description:  In case we ever have to supply credentials
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As String)
            msPassword = value
        End Set

    End Property

    Friend WriteOnly Property SourceDir() As String
        '********************************************************************************************
        'Description:  The Directory/Folder we want to Read/Copy/Get files from.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As String)
            If Strings.Right(value, 1) <> "\" Then
                value = value & "\"
            End If
            msSourceDir = value
        End Set

    End Property

    Friend WriteOnly Property UserName() As String
        '********************************************************************************************
        'Description:  In sase we ever have to supply credentials
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As String)
            msUserName = value
        End Set

    End Property

    Friend Property WorkingDir() As String
        '********************************************************************************************
        'Description:  Gets/Sets the working directory on the Host
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Dim sDir As String = String.Empty

            With moFTP
                If .IsInternetOpen AndAlso .IsFtpConnected Then
                    sDir = .Dir
                End If
            End With 'moFTP

            Return sDir
        End Get

        Set(ByVal value As String)

            Try

                msErrorMsg = String.Empty

                With moFTP
                    If .IsInternetOpen AndAlso .IsFtpConnected Then
                        .Dir = value
                    End If
                End With 'moFTP

            Catch ex As Exception

                msErrorMsg = ex.Message

            End Try

        End Set

    End Property

#End Region

#Region " Routines "

    Friend Sub Close()
        '********************************************************************************************
        'Description:  Close the connection to the host.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        With moFTP
            If .IsFtpConnected Then .Disconnect()
            If .IsInternetOpen Then .Close()
        End With

    End Sub

    Friend Sub Connect()
        '********************************************************************************************
        'Description:  Connect to the host.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            With moFTP

                msErrorMsg = String.Empty

                'If we're already connected, disconnect first
                If .IsFtpConnected Then .Disconnect()
                
                If Not Ping(msHost) Then
                    msErrorMsg = "Cannot connect to host " & msHost & "."
                    Exit Sub
                End If

                .Connect(msHost, msUserName, msPassword, nPORT, False)

            End With 'moFTP

        Catch ex As Exception

            msErrorMsg = ex.Message

        End Try

    End Sub

    Friend Function Delete(ByVal FileName As String) As Boolean
        '********************************************************************************************
        'Description:  Deletes FileName from the current working directory on the host.
        '
        'Parameters: FileName to delete
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            msErrorMsg = String.Empty

            moFTP.Delete(FileName)
            Return True

        Catch ex As Exception

            msErrorMsg = ex.Message

        End Try

        Return False

    End Function

    Friend Function Directory(ByVal Mask As String, Optional ByVal bIncludeDirs As Boolean = False, Optional ByVal bIncludeOther As Boolean = True) As String()
        '********************************************************************************************
        'Description:  Returns a directory listing of the current working directory on the host.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/18/10  MSW     filter out subdirectories
        ' 01/07/11  MSW     add trim to the comparison to remove leading spaces
        '********************************************************************************************
        Dim sDir(0) As String
        Dim nIndex As Integer = 0

        If Mask = String.Empty Then Mask = "*.*"
        msErrorMsg = String.Empty

        Try
            With moFTP
                If .IsInternetOpen AndAlso .IsFtpConnected Then
                    Dim Flags As XferFlags = XferFlags.fsFTPtransferFlagBinaryMode Or _
                                             XferFlags.fsFTPtransferFlagUnknownMode
                    Dim sCmd As String = String.Format(CultureInfo.InvariantCulture, _
                                                       "LIST {0} \r\n", Mask)
                    Dim sFiles As String = String.Empty

                    .Command(sCmd, Flags, True, sFiles)

                    Dim sList() As String = sFiles.Split(New Char() {Chr(13)}, StringSplitOptions.RemoveEmptyEntries)
                    Dim sFile As String = String.Empty
                    Dim bIncludeFile As Boolean = False
                    For Each sFileDetail As String In sList
                        Dim sFileDetails() As String = Split(sFileDetail, " ")
                        sFile = sFileDetails(sFileDetails.GetUpperBound(0)).Trim
                        Debug.Print(sFile)
                        If (Strings.Right(sFile, 1) <> ".") AndAlso Not String.IsNullOrEmpty(sFile) Then
                            'Check for subdirectory
                            If Strings.Left(sFileDetail.Trim, 1).ToLower = "d" Then
                                bIncludeFile = bIncludeDirs
                            Else
                                bIncludeFile = bIncludeOther
                            End If
                            If bIncludeFile Then
                                ReDim Preserve sDir(nIndex)

                                sDir(nIndex) = sFile
                                nIndex += 1
                            End If
                        End If
                    Next 'sFile
                End If
            End With 'moFTP

        Catch ex As Exception

            msErrorMsg = ex.Message

        End Try

        Return sDir

    End Function

    Friend Function GetFile(ByVal SourceFileName As String, ByVal DestFileName As String) As Boolean
        '********************************************************************************************
        'Description:  Get SourcefileName from the Host and copy it to DestFileName in the DestDir
        '              folder on this computer. If DestFileName is empty, the file will retain the 
        '              same name.
        '
        'Parameters: SourceFileName, SourceFileName
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/21/11  MSW     Add noWriteCache flag to keep it from copying an old diagnostic file
        ' 04/30/16  MSW     GetFile - Move error message from constant to resource file
        '********************************************************************************************
        If msDestDir = String.Empty Then
            msErrorMsg = gcsRM.GetString("csFTP_ERR_DEST_NOT_SPEC") '"Error: Destination Folder not specified."
            Return False
        End If

        If SourceFileName = String.Empty Then
            msErrorMsg = gcsRM.GetString("csFTP_ERR_SRC_NOT_SPEC") '"Error: Source File not specified."
            Return False
        End If

        Try

            msErrorMsg = String.Empty
            If DestFileName = String.Empty Then DestFileName = SourceFileName
            Dim Flags As XferFlags = XferFlags.fsFTPtransferFlagBinaryMode Or _
                         XferFlags.fsFTPtransferFlagNoWriteCache

            With moFTP
                If .IsInternetOpen AndAlso .IsFtpConnected Then
                    .Get(SourceFileName, msDestDir & DestFileName, mbEnableOverwrite, Flags)
                End If
            End With

            Return True

        Catch ex As Exception

            msErrorMsg = ex.Message

        End Try

        Return False

    End Function

    Private Function Ping(ByVal hostname As String) As Boolean
        '********************************************************************************************
        'Description:  Make sure a connection can be made to the host.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If My.Computer.Network.IsAvailable Then

            If My.Computer.Network.Ping(hostname) Then

                Return True

            End If

        End If

        Return False

    End Function

    Friend Function PutFile(ByVal SourceFileName As String, ByVal DestFileName As String) As Boolean
        '********************************************************************************************
        'Description:  Get SourcefileName from SourceDir on this computer and copy it to DestFileName 
        '              in the WorkingDir of the host. If DestFileName is empty, the file will retain 
        '              the same name.
        '
        'Parameters: SourceFileName, SourceFileName
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If msSourceDir = String.Empty Then
            msErrorMsg = "Error: Source Folder not specified."
            Return False
        End If

        If SourceFileName = String.Empty Then
            msErrorMsg = "Error: Source File not specified."
            Return False
        End If

        Try

            msErrorMsg = String.Empty
            If DestFileName = String.Empty Then DestFileName = SourceFileName

            With moFTP
                If .IsInternetOpen AndAlso .IsFtpConnected Then
                    If Not mbEnableOverwrite Then
                        Dim sDir() As String = Directory(DestFileName)

                        If sDir(0) = DestFileName Then
                            msErrorMsg = "Error: File exists, overwrite disabled."
                            Return False
                        End If
                    End If

                    .Put(msSourceDir & SourceFileName, DestFileName, XferFlags.fsFTPtransferFlagBinaryMode)
                End If
            End With 'moFTP

            Return True

        Catch ex As Exception

            msErrorMsg = ex.Message

        End Try

        Return False

    End Function

#End Region

#Region " Events "

    Public Sub New(ByVal HostName As String)
        '********************************************************************************************
        'Description:  Class constructor.
        '
        'Parameters: HostName - If not empty then this class will auto connect.
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/03/13  MSW     Add some error handling
        '********************************************************************************************
        Try
            moFTP = New FSFTPLib.FSCFtp

        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subShutDown", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

        If HostName <> String.Empty Then
            Host = HostName
            Call Connect()
        End If

    End Sub

#End Region

End Class
