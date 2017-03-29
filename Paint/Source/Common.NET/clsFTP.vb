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
' Form/Module: clsFTP
'
' Description: FTP File Transfer Class
'
'   --> this is several orders of magnitude slower than the command prompt. anybody know why? 
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
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'******************************************************************************************************
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Diagnostics

Friend Class clsFTP

#Region " Declares "

    '******** Property Variables   **********************************************************************
    Private msSourceDir As String
    Private msSourceFileName As String
    Private msTargetDir As String
    Private msTargetFileName As String
    Private mbAnonymous As Boolean = True  ' default to anonymous user
    Private msUser As String
    Private msPassword As String
    Private msHost As String
    '******** End Property Variables   ******************************************************************
    '******** Class Variables   *************************************************************************
    Dim mURI As UriBuilder
    ' Dim mFTPRequest As FtpWebRequest
    Dim sw As New Stopwatch
    '******** End Class Variables   *********************************************************************
    '******** Events ************************************************************************************
    Friend Event FtpError(ByVal ErrorString As String)
    Friend Event FtpStatus(ByVal StatusString As String, ByVal ElapsedMS As Long)
    '******** End Events ********************************************************************************

#End Region
#Region " Properties "

    Friend Property IsAnonymous() As Boolean
        '********************************************************************************************
        'Description:  is this an anonymous logon? the default condition is yes
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbAnonymous
        End Get
        Set(ByVal value As Boolean)
            mbAnonymous = value
        End Set
    End Property
    Friend WriteOnly Property Password() As String
        '********************************************************************************************
        'Description:  if we ever have to supply credentials
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As String)
            'mURI.Password = value
            msPassword = value
        End Set
    End Property
    Friend WriteOnly Property UserName() As String
        '********************************************************************************************
        'Description:  if we ever have to supply credentials
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As String)
            'mURI.UserName = value
            msUser = value
        End Set
    End Property
    Private ReadOnly Property Source() As String
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return SourceFileDir & SourceFileName
        End Get
    End Property
    Friend Property SourceFileDir() As String
        '********************************************************************************************
        'Description:  Filename parts - no checking syntax here, so send the right stuff....
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If Right(msSourceDir, 1) <> "\" Then msSourceDir = msSourceDir & "\"
            Return msSourceDir
        End Get
        Set(ByVal value As String)
            msSourceDir = value
        End Set
    End Property
    Friend Property SourceFileName() As String
        '********************************************************************************************
        'Description:  Filename parts - no checking syntax here, so send the right stuff....
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msSourceFileName
        End Get
        Set(ByVal value As String)
            msSourceFileName = value
        End Set
    End Property
    Private ReadOnly Property Target() As String
        '********************************************************************************************
        'Description:  
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return TargetFileDir & TargetFileName
        End Get
    End Property
    Friend Property TargetFileDir() As String
        '********************************************************************************************
        'Description:  Filename parts - no checking syntax here, so send the right stuff....
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If Right(msTargetDir, 1) <> "\" Then msTargetDir = msTargetDir & "\"
            Return msTargetDir
        End Get
        Set(ByVal value As String)
            msTargetDir = value
        End Set
    End Property
    Friend Property TargetFileName() As String
        '********************************************************************************************
        'Description:  Filename parts - no checking syntax here, so send the right stuff....
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            'if blank assume the name stays the same
            If msTargetFileName = String.Empty Then Return msSourceFileName
            Return msTargetFileName
        End Get
        Set(ByVal value As String)
            msTargetFileName = value
        End Set
    End Property

#End Region
#Region " Routines "

    Friend Function GetDirectory() As String()
        '********************************************************************************************
        'Description:  get a list of files
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Return sGetDirectory(False)

    End Function
    Friend Function GetDirectoryDetails() As String()
        '********************************************************************************************
        'Description:  get a list of files
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Return sGetDirectory(True)

    End Function
    Friend Function GetFile() As Boolean
        '********************************************************************************************
        'Description:  download a file from target to this computer
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Const nBUFFERSIZE As Integer = 1024
        Dim stream As Stream = Nothing
        Dim sw1 As Stopwatch = Nothing

        Try
            sw1 = Stopwatch.StartNew()

            'point to desired file
            mURI.Path = Source

            ' Set up the request
            Dim ftpRequest As FtpWebRequest = oGetWebRequest(mURI)

            'download
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile

            ' get the response object
            Dim ftpResponse As FtpWebResponse = DirectCast(ftpRequest.GetResponse, FtpWebResponse)

            'create the empty file (overwrites existing)
            Dim fs As New FileStream(Target, FileMode.Create)

            'buffer me
            Dim fileContents(nBUFFERSIZE) As Byte
            Dim bytes_total As Long = 0
            Dim bytes_read As Integer

            'get stream object
            stream = ftpResponse.GetResponseStream
            'initial read
            bytes_read = stream.Read(fileContents, 0, fileContents.Length)
            ' read till end
            While bytes_read > 0
                bytes_total += bytes_read
                fs.Write(fileContents, 0, bytes_read)
                bytes_read = stream.Read(fileContents, 0, nBUFFERSIZE)
            End While

            fs.Close()
            stream.Close()

            Select Case ftpResponse.StatusCode
                Case FtpStatusCode.ClosingData
                    'normal route = success
                    Dim sTmp As String = TargetFileName & " " & gcsRM.GetString("csFTP_DLOAD_RESULT")
                    sTmp = Strings.Replace(sTmp, "%s", bytes_total.ToString)
                    sTmp = Strings.Replace(sTmp, "%r", (sw1.ElapsedMilliseconds / 1000).ToString)

                    RaiseEvent FtpStatus(sTmp, sw.ElapsedMilliseconds)
                    Return True

                Case Else
                    Dim sTmp As String = TargetFileName & _
                            gcsRM.GetString("csFTP_DLOAD_FAIL") & ftpResponse.StatusCode.ToString
                    RaiseEvent FtpError(sTmp)
                    Return False
            End Select

        Catch ex As Exception
            Dim sTmp As String = TargetFileName & gcsRM.GetString("csFTP_DLOAD_FAIL") & ex.Message
            RaiseEvent FtpError(sTmp)
            Return False
        Finally
            'make sure closed in case of error or??
            If Not stream Is Nothing Then stream.Close()
            sw1.Stop()

        End Try

    End Function
    Private Function oGetWebRequest(ByVal oUri As UriBuilder) As FtpWebRequest
        '********************************************************************************************
        'Description:  set up a request object
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        ' Set up the request
        Dim FTPRequest As FtpWebRequest = DirectCast(WebRequest.Create(oUri.Uri), FtpWebRequest)
        With FTPRequest
            ' (default) keep connection open for doing multiple files if needed
            .KeepAlive = True
            .ReadWriteTimeout = 5000
            .Timeout = 5000
            .UseBinary = True
        End With
        ' use the provided credentials
        If Not mbAnonymous Then
            FTPRequest.Credentials = New NetworkCredential(msUser, msPassword)
        End If

        Return FTPRequest

    End Function
    Friend Function OpenConnection() As Boolean
        '********************************************************************************************
        'Description:  Open connection to server
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            sw.Reset()
            sw.Start()
            'who?
            If msHost = String.Empty Then
                RaiseEvent FtpError(gcsRM.GetString("csFTP_ERR_NO_HOST"))
                Return False
            End If

            'can we see target?
            Dim p As New NetworkInformation.Ping
            Dim reply As NetworkInformation.PingReply = p.Send(msHost)
            If reply.Status <> NetworkInformation.IPStatus.Success Then
                RaiseEvent FtpError(msHost & " " & gcsRM.GetString("csFTP_ERR_PING_FAIL"))
                Return False
            End If

            'set up the basics
            mURI = New UriBuilder
            With mURI
                .Host = msHost
                .Path = "ftp://" & msHost
                .Scheme = Uri.UriSchemeFtp
                .UserName = ""
                .Password = ""
            End With

            RaiseEvent FtpStatus(gcsRM.GetString("csFTP_CONNECTING") & " " & msHost, sw.ElapsedMilliseconds)

            Return True

        Catch ex As Exception
            RaiseEvent FtpError(ex.Message)
            Return False
        End Try

    End Function
    Friend Function PutFile() As Boolean
        '********************************************************************************************
        'Description:  Upload a file from  this computer to target
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sw1 As Stopwatch = Nothing
        Dim stream As Stream = Nothing
        Dim reader As FileStream = Nothing

        Try
            sw1 = Stopwatch.StartNew()

            'point to desired file
            mURI.Path = Target

            ' Set up the request
            Dim ftpRequest As FtpWebRequest = oGetWebRequest(mURI)

            'upload a file
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile

            'open file to upload
            reader = New FileStream(Source, FileMode.Open, FileAccess.Read)

            'buffer me
            Dim fileContents(CInt(reader.Length - 1)) As Byte

            'Copy the contents of the file to the request stream.
            reader.Read(fileContents, 0, fileContents.Length)

            'always close
            reader.Close()

            'size me
            ftpRequest.ContentLength = fileContents.Length

            'get the stream to write to target
            stream = ftpRequest.GetRequestStream
            'I don't have all day
            stream.WriteTimeout = 5000

            'fill stream
            stream.Write(fileContents, 0, fileContents.Length)

            'size me
            stream.Close()

            'did it work?
            Dim ftpResponse As FtpWebResponse = CType(ftpRequest.GetResponse, FtpWebResponse)

            Select Case ftpResponse.StatusCode
                Case FtpStatusCode.ClosingData
                    'normal route = success
                    Dim sTmp As String = TargetFileName & " " & gcsRM.GetString("csFTP_ULOAD_RESULT")
                    sTmp = Strings.Replace(sTmp, "%s", fileContents.Length.ToString)
                    sTmp = Strings.Replace(sTmp, "%r", (sw1.ElapsedMilliseconds / 1000).ToString)

                    RaiseEvent FtpStatus(sTmp, sw.ElapsedMilliseconds)
                    Return True

                Case Else
                    Dim sTmp As String = TargetFileName & " " & _
                                gcsRM.GetString("csFTP_ULOAD_FAIL") & " " & ftpResponse.StatusCode.ToString
                    RaiseEvent FtpError(sTmp)
                    Return False
            End Select

        Catch ex As Exception
            Dim sTmp As String = TargetFileName & " " _
                                    & gcsRM.GetString("csFTP_ULOAD_FAIL2") & " " & ex.Message
            RaiseEvent FtpError(sTmp)
            Return False
        Finally
            'make sure closed in case of error or??
            If Not reader Is Nothing Then reader.Close()
            If Not stream Is Nothing Then stream.Close()
            sw1.Stop()
        End Try

    End Function
    Private Function sGetDirectory(ByVal bDetails As Boolean) As String()
        '********************************************************************************************
        'Description:  get a list of files
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim stream As Stream = Nothing
        Dim reader As StreamReader = Nothing

        Try

            'point to desired file
            mURI.Path = Source

            ' Set up the request
            Dim ftpRequest As FtpWebRequest = oGetWebRequest(mURI)

            'need to lengthen the timeouts
            ftpRequest.ReadWriteTimeout = 30000
            ftpRequest.Timeout = 30000

            If bDetails Then
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails
            Else
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory
            End If

            ' get the response object
            Dim ftpResponse As FtpWebResponse = CType(ftpRequest.GetResponse, FtpWebResponse)

            stream = ftpResponse.GetResponseStream
            reader = New StreamReader(stream, Encoding.UTF8)
            Dim sDir As String() = Strings.Split(reader.ReadToEnd, vbCrLf)

            'this returns a trailing delimiter - need to get rid of empty position
            Dim i As Integer = UBound(sDir)
            If sDir(i) = String.Empty Then
                If i > 0 Then
                    ReDim Preserve sDir(i - 1)
                Else
                    'no files found
                    Dim sTmp As String = gcsRM.GetString("csFTP_DIR_NO_FILES")
                    sTmp = Replace(sTmp, "%s", SourceFileName)
                    RaiseEvent FtpStatus(sTmp, sw.ElapsedMilliseconds)
                End If
            End If

            Select Case ftpResponse.StatusCode
                Case FtpStatusCode.ClosingData
                    'normal = success
                    If i > 0 Then
                        Dim sTmp As String = gcsRM.GetString("csFTP_DIR_FILES")
                        sTmp = Replace(sTmp, "%s", SourceFileName)
                        RaiseEvent FtpStatus(sTmp & UBound(sDir).ToString, sw.ElapsedMilliseconds)
                    End If
                Case Else
                    RaiseEvent FtpError(ftpResponse.StatusCode.ToString)
            End Select

            Return sDir

        Catch ex As Exception
            Dim s(0) As String
            RaiseEvent FtpError(ex.Message)
            Return s
        Finally
            ' Allways close all streams
            If Not stream Is Nothing Then stream.Close()
            If Not reader Is Nothing Then reader.Close()
        End Try

    End Function

#End Region
#Region " Events "

    Friend Sub New(ByVal HostName As String)
        '********************************************************************************************
        'Description:  Hellow world
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        msHost = HostName

    End Sub
    Protected Overrides Sub Finalize()
        '********************************************************************************************
        'Description:  I'm outta here
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        RaiseEvent FtpStatus("close " & msHost, sw.ElapsedMilliseconds)
        sw.Stop()
        MyBase.Finalize()
    End Sub

#End Region


End Class
