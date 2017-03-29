' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2012
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: clsInterProcessComm
'
' Description: Cheap and dirty interprocess communication
' 
'
' Dependencies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Rick Olejniczak
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
' 12/10/2011    RJO     Initial code                                                  4.1.1.0
' 02/10/2012    RJO     Cleanup                                                       4.1.1.1
' 03/15/2012    MSW     New - Add the "*.XML" if a name is passed in without it.      4.1.1.2	                             
' 03/19/2012    RJO     Added code to include the Process ID in IPC file names to     4.1.1.3
'                       handle the issue of multiple instances of the same process.
' 09/07/2012    RJO     Corrected issues with declaration, setup and event handling   4.1.1.4
'                       of mWatcher.
' 10/07/12      MSW     Add screenname to some error messages                         4.1.1.5
'                       WriteControlMsg check for uninit Element
' 04/26/13      MSW     WriteControlMsg - Add the calling process id to the file name 4.1.5.0
'                       so all the background programs don't step on each other at startup.
' 05/16/13      MSW     remove references to frmMain, use the process name instead    4.1.5.1
' 10/29/13      RJO     Change to Sub New to handle Param Setup screen special case.  4.1.5.2
'********************************************************************************************

Option Explicit On
Option Strict On

Imports System.IO
Imports System.Diagnostics

Namespace Paintworks_IPC

    Friend Class clsInterProcessComm

#Region " Declares "

        '******** Constants *****************************************************************************
        Private Const msMODULE As String = "clsInterProcessComm"

        Friend Const sCOL_ACTION As String = "Action"
        Friend Const sCOL_PARAMETER As String = "Parameter"
        Friend Const sCOL_LEFT As String = "Left"
        Friend Const sCOL_TOP As String = "Top"
        Friend Const sCOL_WIDTH As String = "Width"
        Friend Const sCOL_HEIGHT As String = "Height"
        Friend Const sTABLE As String = "Command"
        Friend Const sCOL_PROCNAME As String = "ProcName"
        Friend Const sCOL_PRIVILEGE As String = "Privilege"
        Friend Const sCOL_PASSWORD As String = "Password"
        Friend Const sCOL_USERNAME As String = "UserName"
        Friend Const sCOL_LASTLOGIN As String = "LastLogin"
        Friend Const sCOL_GROUP As String = "Group"
        '******** End Constants *************************************************************************

        '******** Property Variables ********************************************************************
        Friend CONTROL_MSG_SCHEMA As String = "ControlMsg.xsd"
        Friend DATA_MSG_SCHEMA As String = "DataMsg.xsd"
        Friend PASSWORD_MSG_SCHEMA As String = "PasswordMsg.xsd"
        '******** End Property Variables ****************************************************************

        '******** Form Variables ************************************************************************
        Private WithEvents mWatcher As FileSystemWatcher 'RJO 09/07/12
        Private mbEnableEvents As Boolean
        Private msFilter As String
        Private msPath As String
        Private msXMLPath As String
        Private msProcecssName As String
        '******** End Form Variables ********************************************************************

        '******** Events ********************************************************************************
        Public Event NewMessage(ByVal Schema As String, ByVal DS As DataSet)
        '******** End Events ****************************************************************************
#End Region

#Region " Properties "

        Private ReadOnly Property Filter() As String
            '********************************************************************************************
            'Description: The filter for the files of interest. Example: AppName*.xml
            '
            'Parameters: None
            'Returns:    None
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                If msFilter = String.Empty Then
                    'Set to default 
                    msFilter = "*.xml"
                End If
                Return msFilter
            End Get

        End Property

        Friend Property EnableEvents() As Boolean
            '********************************************************************************************
            'Description: Enables/Disables the raising of the NewMessage event when a file change is 
            '             detected
            '
            'Parameters: None
            'Returns:    Enable Events State
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                Return mbEnableEvents
            End Get

            Set(ByVal value As Boolean)
                mbEnableEvents = value
            End Set

        End Property

        Private ReadOnly Property Path() As String
            '********************************************************************************************
            'Description: The full path to the folder that will contain the IPC files
            '
            'Parameters: None
            'Returns:    The path
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                If msPath = String.Empty Then
                    'Set to default
                    mPWCommon.GetDefaultFilePath(msPath, eDir.IPC, String.Empty, String.Empty)
                End If
                Return msPath
            End Get

        End Property

        Private ReadOnly Property XMLPath() As String
            '********************************************************************************************
            'Description: The full path to the folder that will contain the IPC files
            '
            'Parameters: None
            'Returns:    The path
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            Get
                If msXMLPath = String.Empty Then
                    'Set to default
                    mPWCommon.GetDefaultFilePath(msXMLPath, eDir.XML, String.Empty, String.Empty)
                End If
                Return msXMLPath
            End Get

        End Property

#End Region

#Region " Routines "

        Public Function GetProcess(ByVal AppName As String) As Process
            '********************************************************************************************
            'Description:  Return the Process associated with the supplied Application Name
            '
            'Parameters: AppName
            'Returns:    AppName's Process
            '
            'Modification history:
            '
            ' Date      By      Reason
            ' 11/19/09  MSW     Try to get in touch with a program running in visual studio
            '********************************************************************************************
            Try
                Dim procByName As Process() = Process.GetProcessesByName(AppName)

                If procByName.Length > 0 Then
                    'Running
                    Return procByName(0)
                Else
                    'Try to get in touch with a program running in visual studio
                    procByName = Process.GetProcessesByName(AppName & ".vshost")
                    If procByName.Length > 0 Then
                        'Running
                        Return procByName(0)
                    Else
                        Return Nothing
                    End If
                End If

            Catch ex As Exception
                mDebug.WriteEventToLog("Module: mWorksComm, Routine: GetProcess", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                Throw ex
                Return Nothing
            End Try

        End Function

        Private Function GetSchema(ByVal FileName As String) As String
            '********************************************************************************************
            'Description: Returns the name of the Schema file for the XML data in Data().
            '
            'Parameters: Data - XML Data
            'Returns:    Schema (.xsd) filename
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************
            Dim nIndex As Integer
            Dim sMessage(1) As String
            Dim sSchema As String = String.Empty

            Try
                Using oSR As New StreamReader(FileName)
                    Dim sLine As String
                    Do
                        sLine = oSR.ReadLine
                        If Not (sLine Is Nothing) Then
                            sMessage(nIndex) = sLine
                            nIndex += 1
                            If nIndex > 1 Then Exit Do
                        End If
                    Loop Until sLine Is Nothing
                End Using

                Dim sData() As String = Split(sMessage(1), "/")
                Dim nUB As Integer = sData.GetUpperBound(0)

                sSchema = Strings.Left(sData(nUB), Strings.Len(sData(nUB)) - 2)

            Catch ex As Exception
                mDebug.WriteEventToLog("Module: " & msMODULE & " Routine: GetSchema", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try

            Return sSchema

        End Function

        Friend Sub WriteControlMsg(ByVal ProcName As String, ByVal ParamArray Elements() As String)
            '********************************************************************************************
            'Description: Writes an XML file containing ControlMsg Elements to be read by ProcName
            '
            'Parameters: ProcName - The name of the process that will listen for this message
            '            Elements - ControlMessage DataRow elements to send as XML
            'Returns:    None
            '
            'Modification history:
            '
            ' Date      By      Reason
            ' 10/10/12  MSW     An empty message is throwing up an error, so init the value
            ' 04/26/13  MSW     WriteControlMsg - Add the calling process id to the file name 
            '                   so all the background programs don't step on each other at startup.
            '********************************************************************************************
            Dim sFileName As String = msPath & ProcName
            Dim nTime As Integer = CType(DateTime.Now.Ticks Mod Int32.MaxValue, Integer)
            Dim DS As New DataSet
            Dim DR As DataRow = Nothing
            Dim oThisProc As Process = Process.GetCurrentProcess
            Try
                If Strings.InStr(ProcName, "(") = 0 Then
                    Dim oProc As Process = GetProcess(ProcName)
                    'append process ID to ProcName 'RJ0 03/19/12
                    If Not (oProc Is Nothing) Then
                        sFileName = sFileName & "(" & oProc.Id.ToString & ")"
                    End If
                End If

                DS.ReadXmlSchema(msXMLPath & CONTROL_MSG_SCHEMA)
                DR = DS.Tables(sTABLE).NewRow

                For nElement As Integer = 0 To Elements.GetUpperBound(0)
                    If Elements(nElement) Is Nothing OrElse Elements(nElement) = String.Empty Then
                        If nElement > 1 Then
                            Elements(nElement) = "0"
                        Else
                            Elements(nElement) = " "
                        End If
                    End If
                    Select Case nElement
                        Case 0
                            DR.Item(sCOL_ACTION) = Elements(0)
                        Case 1
                            DR.Item(sCOL_PARAMETER) = Elements(1)
                        Case 2
                            DR.Item(sCOL_LEFT) = CType(Elements(2), Integer)
                        Case 3
                            DR.Item(sCOL_TOP) = CType(Elements(3), Integer)
                        Case 4
                            DR.Item(sCOL_WIDTH) = CType(Elements(4), Integer)
                        Case 5
                            DR.Item(sCOL_HEIGHT) = CType(Elements(5), Integer)
                    End Select
                Next 'nElement

                DS.Tables(sTABLE).Rows.Add(DR)
                DS.AcceptChanges()

                sFileName = sFileName & nTime.ToString & "-" & oThisProc.Id.ToString & ".xml"
                DS.WriteXml(sFileName)

            Catch ex As Exception
                mDebug.WriteEventToLog(msProcecssName & ": " & msMODULE & " Routine: WriteControlMsg", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try

        End Sub

        Friend Sub WriteDataMsg(ByVal ProcName As String, ByVal DataMsg As DataSet)
            '********************************************************************************************
            'Description: Writes the contents of DataMsg to an XML file to be read by ProcName
            '
            'Parameters: AppName - The name of the application that will listen for this message
            '            DataMsg - DataMsg DataSet to send as XML
            'Returns:    None
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************
            Dim sFileName As String = msPath & ProcName
            Dim nTime As Integer = CType(DateTime.Now.Ticks Mod Int32.MaxValue, Integer)

            'TODO - Code here. 
            'Note: This is intended for use by ScatteredAccessManager.NET if we decide to go that route.

            sFileName = sFileName & nTime.ToString & ".xml"
            DataMsg.WriteXml(sFileName)

        End Sub

        Friend Sub WritePasswordMsg(ByVal ProcName As String, ByVal ParamArray Elements() As String)
            '********************************************************************************************
            'Description: Writes an XML file containing PasswordMsg Elements to be read by ProcName
            '
            'Parameters: ProcName - The name of the application that will listen for this message
            '            Elements - ControlMessage DataSet elements to send as XML
            'Returns:    None
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************
            Dim sFileName As String = msPath & ProcName
            Dim nTime As Integer = CType(DateTime.Now.Ticks Mod Int32.MaxValue, Integer)
            Dim DS As New DataSet
            Dim DR As DataRow = Nothing

            Try
                If Strings.InStr(ProcName, "(") = 0 Then
                    Dim oProc As Process = GetProcess(ProcName)
                    'append process ID to ProcName 'RJ0 03/19/12
                    If Not (oProc Is Nothing) Then
                        sFileName = sFileName & "(" & oProc.Id.ToString & ")"
                    End If
                End If

                DS.ReadXmlSchema(msXMLPath & PASSWORD_MSG_SCHEMA)
                DR = DS.Tables(sTABLE).NewRow

                For nElement As Integer = 0 To Elements.GetUpperBound(0)
                    Select Case nElement
                        Case 0
                            DR.Item(sCOL_PROCNAME) = Elements(0)
                        Case 1
                            DR.Item(sCOL_ACTION) = Elements(1)
                        Case 2
                            DR.Item(sCOL_PRIVILEGE) = Elements(2)
                        Case 3
                            DR.Item(sCOL_PASSWORD) = Elements(3)
                        Case 4
                            DR.Item(sCOL_USERNAME) = Elements(4)
                        Case 5
                            DR.Item(sCOL_LASTLOGIN) = Elements(5)
                        Case 6
                            DR.Item(sCOL_GROUP) = Elements(6)
                        Case 7
                            DR.Item(sCOL_PARAMETER) = Elements(7)
                    End Select
                Next 'nElement

                DS.Tables(sTABLE).Rows.Add(DR)
                DS.AcceptChanges()

                sFileName = sFileName & nTime.ToString & ".xml"
                DS.WriteXml(sFileName)

            Catch ex As Exception
                mDebug.WriteEventToLog(msProcecssName & ": " & msMODULE & " Routine: WritePasswordMsg", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try

        End Sub

#End Region

#Region " Events "

        Private Sub mWatcher_Changed(ByVal sender As Object, ByVal e As System.IO.FileSystemEventArgs) Handles mWatcher.Changed
            '********************************************************************************************
            'Description: The Size of an XML file of interest has just Changed. If EnableEvents, raise an  
            '             event to the parent application, then delete the file.
            '
            'Parameters: None
            'Returns:    None
            '
            'Modification history:
            '
            ' Date      By      Reason
            '********************************************************************************************

            If My.Computer.FileSystem.FileExists(e.FullPath) Then
                Dim sSchema As String = GetSchema(e.FullPath)
                Dim DS As New DataSet

                If Strings.Right(sSchema, 4).ToLower = ".xsd" Then
                    DS.ReadXmlSchema(msXMLPath & sSchema)
                    DS.ReadXml(e.FullPath)
                    If EnableEvents Then RaiseEvent NewMessage(sSchema, DS)
                Else
                    mDebug.WriteEventToLog(msProcecssName & ": " & msMODULE & " Routine: mWatcher_Changed", _
                                           "Unable to find schema filename in " & e.FullPath.ToString)
                End If
                Try
                    My.Computer.FileSystem.DeleteFile(e.FullPath)
                Catch ex As Exception
                End Try
            End If
        End Sub

        Public Sub New(Optional ByVal FileFilter As String = "", Optional ByVal IPCPath As String = "", _
                       Optional ByVal XMLPath As String = "", _
                       Optional ByVal EventsEnable As Boolean = True)
            '********************************************************************************************
            'Description: Watch for changes in files that match sFilter in the folder specified in sPath.
            '
            'Parameters: FileFilter - Filename pattern to match (ex. AlarmMan*.XML)
            '            IPCPath - Path to InterProcess Comm folder (ex. C:\Paint\VBApps\IPC)
            '            XMLPath - Path to XML schema file
            '            EventsEnable - True = Enable Raising events when a file of interest was crested
            'Returns:    None
            '
            'Modification history:
            '
            ' Date      By      Reason
            ' 03/15/12  MSW     Add the "*.XML" if a name is passed in without it.
            ' 03/19/12  RJO     Added "(ProcID)" to FileFilter because Paintworks will launch more than
            '                   one instance of some applications.
            ' 09/07/12  RJO     Corrected issues with setup and event handling for mWatcher.
            ' 10/29/13  RJO     Special case handling for Param SEtup screen
            '********************************************************************************************

            If (FileFilter = String.Empty) Or (FileFilter = "?") Then '10/29/13  RJO
                msProcecssName = Process.GetCurrentProcess.ProcessName
                Dim sProcID As String = "(" & Process.GetCurrentProcess.Id.ToString & ")"
                Dim nDot As Integer = Strings.InStr(msProcecssName, ".")

                If FileFilter = String.Empty Then ' 10/29/13  RJO
                    'Use the process name for the Filter. Strip off ".vshost" if running in development mode.
                    If nDot > 0 Then
                        msFilter = Strings.Left(msProcecssName, nDot - 1) & sProcID & "*.XML"
                    Else
                        msFilter = msProcecssName & sProcID & "*.XML"
                    End If
                Else
                    ' ? = Param setup screen which can be launched with multiple screen names. look at proc id only
                    msFilter = "*" & sProcID & "*.XML" ' 10/29/13  RJO
                End If

            Else
                msFilter = FileFilter
                If msFilter.Substring(msFilter.Length - 5).ToLower <> "*.xml" Then
                    msFilter = msFilter & "*.XML"
                End If
            End If

            If Not (IPCPath = String.Empty) Then
                If Strings.Right(IPCPath, 1) <> "\" Then
                    IPCPath = IPCPath & "\"
                End If
                msPath = IPCPath
            ElseIf msPath = String.Empty Then
                mPWCommon.GetDefaultFilePath(msPath, eDir.IPC, String.Empty, String.Empty)
            End If

            If Not (XMLPath = String.Empty) Then
                If Strings.Right(XMLPath, 1) <> "\" Then
                    XMLPath = XMLPath & "\"
                End If
                msXMLPath = XMLPath
            ElseIf msXMLPath = String.Empty Then
                mPWCommon.GetDefaultFilePath(msXMLPath, eDir.XML, String.Empty, String.Empty)
            End If

            If My.Computer.FileSystem.DirectoryExists(msPath) Then
                'clean up junk files
                For Each sJunkFile As String In My.Computer.FileSystem.GetFiles(msPath, FileIO.SearchOption.SearchTopLevelOnly, Filter)
                    My.Computer.FileSystem.DeleteFile(sJunkFile)
                Next
            Else
                'folder doesn't exist, so create it
                My.Computer.FileSystem.CreateDirectory(msPath)
            End If

            mWatcher = New FileSystemWatcher

            With mWatcher
                .Path = msPath
                'Watch for changes in File Size
                .NotifyFilter = NotifyFilters.Size 'NotifyFilters.LastWrite  'RJO 09/07/12
                .Filter = Filter

                'Add event handler
                'AddHandler .Changed, AddressOf OnChanged  'RJO 09/07/12 (see mWatcher_Changed)

                ' Begin watching.
                EnableEvents = EventsEnable
                .EnableRaisingEvents = True
            End With

        End Sub

#End Region

    End Class
End Namespace

