'
' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 1999-2007
' FANUC Robotics America
' FANUC LTD Japan
'
' Form: frmMain.frm
'
' NOTE: If the filename is the same as the module name, only the file's extension is shown.  RJK
' Dependencies:  
'
' Description:   PAINTworks IV Schedule Manager
'
' The purpose of this form is to check the current time against the schedule each time the event tick
' timer fires and carry out the appropriate actions each time a scheduled event occurs.
'
' Author: R. Olejniczak
' FANUC Robotics America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    09/14/11   MSW     Assemble a standard version of everything                       4.1.0.0
'    01/17/12   MSW     Disable task bar so it doesn't get shut down accidentally       4.1.1.1
'    01/24/12   MSW     Change to new Interprocess Communication in clsInterProcessComm   4.01.01.02
'    02/15/12   MSW     Force 32 bit build for compatability with PCDK                  4.01.01.03
'    03/09/12   RJO     modified init for oIPC (Inter Process Comm)                     4.01.02.00
'    03/28/12   MSW     Move DB from SQL to XML                                         4.01.03.00
'    11/29/12   MSW     bReadMaintDataFromXML - Robot and file flags got mixed          4.01.04.00
'                       up at some point
'    04/28/13   MSW     Honda Canada Updates                                            4.01.05.00
'    07/09/13   MSW     Update and standardize logos                                    4.01.05.01
'    09/30/13   MSW     PLC DLL                                                         4.01.06.00
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call                4.01.07.00
'    05/16/14   MSW     Add weekly reports to scheduler                                 4.01.07.01
'***************************************************************************************************

Option Compare Text
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Configuration.ConfigurationSettings
'dll for zip access: C:\paint\Vbapps\Ionic.Zip.dll
'See License at C:\paint\Source\Common.NET\DotNetZip License.txt
'Website http://dotnetzip.codeplex.com/
Imports Ionic.Zip
Imports System.Xml
Imports System.Xml.XPath
Imports clsPLCComm = PLCComm.clsPLCComm

Public Class frmMain

#Region " Declares "
    '******** Form Constants   **********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Public Const msSCREEN_NAME As String = "SchedMan"   ' <-- For password area change log etc.
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    Friend Const mbUSEROBOTS As Boolean = False

    Private Const mnPOLL_INTERVAL As Integer = 25000    'Check schedule at 25 second intervals
    '******** End Form Constants    *****************************************************************

    '******** Enumerations    ***********************************************************************
    Private Enum eSchedule
        Shift_1 = 1
        Shift_2 = 7
        Shift_3 = 13
        Clean_Backup = 19
    End Enum

    Private Enum eFlags
        Cleanup = 1
        Robot_Backup = 2
        File_Backup = 4
    End Enum
    '******** End Enumerations    *******************************************************************

    '******** Structures    *************************************************************************
    Private Structure mScheduleEvent
        Friend StartTime As Integer
        Friend EndTime As Integer
        Friend Enabled As Boolean
        Friend Active As Boolean
        Friend Flags As Integer
    End Structure
    '******** End Structures    *********************************************************************

    '******** Form Variables    *********************************************************************
    Private mbBreakActiveMemory As Boolean
    Private mbFirstPassComplete As Boolean
    Private mbNewDayMemory As Boolean
    Private mbNotifyStartOfShift As Boolean
    Private mbNotifyEndOfShift As Boolean
    Private mcolZones As clsZones = Nothing
    Private mnDay As Integer
    Private WithEvents moPLC As clsPLCComm
    Private mScheduleEvents(19) As mScheduleEvent
    Private mbWeeklyReports As Boolean = False
    Private msDmonPath As String = String.Empty
    Private msDmonArchivePath As String = String.Empty
    Private mbDoDmonArchive As Boolean = False
    Private moMaint As Maintenance = Nothing

    'Windows Messaging variables
    Private msWinMsg As String = String.Empty
    Private mnMsgCount As Integer = 0
    Private mnMsgLen As Integer = 0
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private msCulture As String = "en-US" 'Default to english
    '******** End Property Variables    *************************************************************

    '********New program-to-program communication object******************************************
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet)
    '********************************************************************************************

#End Region

#Region " Properties "

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

        End Set

    End Property

    Friend ReadOnly Property DisplayCulture() As Globalization.CultureInfo
        '********************************************************************************************
        'Description:  The Culture Club
        '
        'Parameters: None
        'Returns:    CultureInfo for current culture.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return New Globalization.CultureInfo(msCulture)
        End Get

    End Property

    Friend Property Status(Optional ByVal void As Boolean = False) As String
        '********************************************************************************************
        'Description: just here for compatability
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return String.Empty
        End Get

        Set(ByVal value As String)

        End Set

    End Property

#End Region

#Region " Routines "

    Private Function bCheckEventActive(ByVal PWEvent As Integer, ByVal Time As Integer) As Boolean
        '********************************************************************************************
        'Description: Checks the supplied time against the start and end time of the supplied event
        '             to determine if the event is active.
        '
        'Parameters: Event number, Time (Integer formatted 'hhmm')
        'Returns:    True if event is active, false otherwise
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Try

            bCheckEventActive = False

            With mScheduleEvents(PWEvent)

                If .StartTime < .EndTime Then
                    bCheckEventActive = ((Time >= .StartTime) And (Time <= .EndTime))
                Else
                    'Event passes through midnight
                    bCheckEventActive = ((Time >= .StartTime) Or (Time <= .EndTime))
                End If

            End With

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bCheckEventActive", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Function

    Private Function bNewDay() As Boolean
        '********************************************************************************************
        'Description: Checks if the day changed
        '
        'Parameters: none
        'Returns:    True if day changed, false otherwise
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Try
            Dim nToday As Integer = DatePart(DateInterval.Weekday, Now)

            bNewDay = False

            If mnDay <> nToday Then
                mnDay = nToday
                bNewDay = True
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bNewDay", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Function

    Private Function bReadMaintDataFromXML() As Boolean
        '********************************************************************************************
        'Description:   Loads Cleanup/Backup times into the Schedule array from the database.
        '
        'Parameters:    None
        'Returns:       True if read is successful, False otherwise
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         03/28/12      Move DB to XML
        ' MSW         11/29/12      bReadMaintDataFromXML - Robot and file flags got mixed up at some point
        '********************************************************************************************
        Dim oZone As clsZone = mcolZones(mcolZones.CurrentZone)
        Const sXMLTABLE As String = "SchedBackup"
        Const sXMLNODE As String = "Backup"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty




                    Dim sHour As String = String.Empty
                    Dim sMin As String = String.Empty
                    Dim sSec As String = String.Empty
                    Dim nTime As Integer = 0
        Dim nFlags As Integer = 0
        Dim bSuccess As Boolean = False

        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
            Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)
                If oNodeList.Count > 0 Then
                    For Each oNode As XmlNode In oNodeList
                        Dim nID As Integer = CType(oNode.Item("ID").InnerXml, Integer)
                        If nID = mnDay Then
                            sHour = oNode.Item("StartHour").InnerXml
                            sMin = oNode.Item("StartMinute").InnerXml
                            sSec = oNode.Item("StartSecond").InnerXml

                    nTime = nGetTime(sHour, sMin, sSec, 0)
                    mScheduleEvents(eSchedule.Clean_Backup).StartTime = nTime

                    nTime = nGetTime(sHour, sMin, sSec, 1)
                    mScheduleEvents(eSchedule.Clean_Backup).EndTime = nTime

                            If CType(oNode.Item("CleanupEnabled").InnerXml, Boolean) Then
                                nFlags += eFlags.Cleanup
                            End If
                            'MSW 11/29/12 - robot and file flags got mixed up at some point
                            If CType(oNode.Item("BackupEnabled").InnerXml, Boolean) Then
                                nFlags += eFlags.File_Backup
                            End If
                            If CType(oNode.Item("RobotBackupEnabled").InnerXml, Boolean) Then
                                nFlags += eFlags.Robot_Backup
                            End If

                    'Save Flags
                    mScheduleEvents(eSchedule.Clean_Backup).Flags = nFlags
                    'Set Enable
                    mScheduleEvents(eSchedule.Clean_Backup).Enabled = (nFlags > 0)

                            bSuccess = True
                            Exit For
                End If



                    Next
            End If

            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bReadMaintDataFromXML", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
        End If

        Return bSuccess

    End Function

    Private Function bReadOptionDataFromXML() As Boolean
        '********************************************************************************************
        'Description:   Reads settings from the Options table in the Schedule database.
        '
        'Parameters:    None
        'Returns:       True if read is successful, false otherwise
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         03/28/12      Move DB to XML
        '********************************************************************************************
        Dim oZone As clsZone = mcolZones.ActiveZone
        Const sXMLTABLE As String = "SchedOptions"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Dim bSuccess As Boolean = False





        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
            Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                If oMainNode IsNot Nothing Then
                    mbNotifyStartOfShift = CType(oMainNode.Item("NotifyStartShift").InnerXml, Boolean)
                    mbNotifyEndOfShift = CType(oMainNode.Item("NotifyEndShift").InnerXml, Boolean)
                    bSuccess = True
                End If

        Catch ex As Exception

                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bReadOptionDataFromXML", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
            End If
        Return True


        Return bSuccess

    End Function

    Private Function bReadShiftDataFromXML() As Boolean
        '********************************************************************************************
        'Description:   Loads Shift times into the Schedule array from the Schedule database.
        '
        'Parameters:    None
        'Returns:       True if read is successful, false otherwise
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         03/28/12      Move DB to XML
        '********************************************************************************************
        Dim oZone As clsZone = mcolZones.ActiveZone
        Const sXMLTABLE As String = "Schedules"
        Const sXMLNODE As String = "Schedule"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Dim bSuccess As Boolean = True
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
        Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)




                If oNodeList.Count > 0 Then
                    For Each oNode As XmlNode In oNodeList
                        Dim sHour As String = String.Empty
                        Dim sMin As String = String.Empty
                        Dim sSec As String = String.Empty


                        Dim nEvent As Integer = CType(oNode.Item("ID").InnerXml, Integer)

                        'Load Start Time
                        sHour = oNode.Item("StartHour").InnerXml
                        sMin = oNode.Item("StartMinute").InnerXml
                        sSec = oNode.Item("StartSecond").InnerXml
                        mScheduleEvents(nEvent).StartTime = nGetTime(sHour, sMin, sSec, 0)

                        'Load End Time
                        sHour = oNode.Item("EndHour").InnerXml
                        sMin = oNode.Item("EndMinute").InnerXml
                        sSec = oNode.Item("EndSecond").InnerXml
                        mScheduleEvents(nEvent).EndTime = nGetTime(sHour, sMin, sSec, 0)

                        'Load Enable
                        mScheduleEvents(nEvent).Enabled = CType(oNode.Item("Enabled").InnerXml, Boolean)

                        'Load Flags, in this case we'll use this field to hold the zone
                        'number that this schedule data belongs to.
                        mScheduleEvents(nEvent).Flags = CType(oNode.Item("Zone").InnerXml, Integer)

                    Next
                End If
        Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bLoadFolderDataFromXML", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            bSuccess = False

            End Try
            End If


        Return bSuccess

    End Function

    Private Function nGetTime(ByRef Hour As String, ByRef Minute As String, ByRef Second As String, ByVal Offset As Integer) As Integer
        '********************************************************************************************
        'Description: Convert the supplied Hour and Minute to an Integer (format 'hhmm').
        '
        'Parameters: Hour, Minute
        '            Offset in Minutes
        'Returns:    6 digit (maximum) Integer Time value
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Try
            Dim sHour As String = Hour.PadLeft(2, "0"c)
            Dim sMin As String = Minute.PadLeft(2, "0"c)
            Dim sSec As String = Second.PadLeft(2, "0"c)

            If Offset <> 0 Then
                'Adjust the time by the offset
                Offset = Offset Mod 1440 'Don't allow an offset greater than 1 day

                Dim nHour As Integer = (Offset \ 60) + CType(sHour, Integer)
                Dim nMin As Integer = (Offset Mod 60) + CType(sMin, Integer)

                'Handle Minute Rollover
                If nMin > 59 Then
                    nMin -= 60
                    nHour += 1
                ElseIf nMin < 0 Then
                    nMin += 60
                    nHour -= 1
                End If

                'Handle Hour Rollover
                If nHour > 23 Then
                    nHour -= 24
                ElseIf nHour < 0 Then
                    nHour += 24
                End If

                sHour = Strings.Format(nHour, "00")
                sMin = Strings.Format(nMin, "00")

            End If

            nGetTime = CType(sHour & sMin & sSec, Integer)

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: nGetTime", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Function

    Private Sub subDoScreenAction(ByRef DR As DataRow)
        '********************************************************************************************
        'Description:  PW4_Main has received a command or notification from another Paintworks 
        '              application.
        '
        'Parameters: Command or Notification
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 

        Try


            Select Case DR.Item(Paintworks_IPC.clsInterProcessComm.sCOL_ACTION).ToString.ToLower

                Case "close"
                    'Get outta Dodge
                    Application.Exit()
                    Me.Close()

                Case "update"
                    mbFirstPassComplete = False

                    'Reload the schedule from the database
                    Call bReadShiftDataFromXML()
                    Call bReadMaintDataFromXML()
                    Call bReadOptionDataFromXML()

                Case "setproperties"
                    'Future - for whatever commands we may need to pass to this app.

            End Select

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoScreenAction", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub


    'Private Sub subDoScreenAction(ByVal ScreenCommand As mWorksComm.udsFRWM)
    '    '********************************************************************************************
    '    'Description:  PW4_Main has received a command or notification from another Paintworks 
    '    '              application.
    '    '
    '    'Parameters: Command or Notification
    '    'Returns:    None
    '    '
    '    'Modification history:
    '    '
    '    ' Date      By      Reason
    '    '******************************************************************************************** 

    '    Try

    '        '>>> DEBUG
    '        mDebug.WriteEventToLog("SchedMan", "Message {" & ScreenCommand.ScreenAction & "} received.")
    '        '<<< DEBUG

    '        Select Case ScreenCommand.ScreenAction.ToLower

    '            Case "close"
    '                'Get outta Dodge
    '                Application.Exit()
    '                Me.Close()

    '            Case "update"
    '                mbFirstPassComplete = False

    '                'Reload the schedule from the database
    '                Call bReadShiftDataFromXML()
    '                Call bReadMaintDataFromDB()
    '                Call bReadOptionDataFromXML()

    '            Case "setproperties"
    '                'Future - for whatever commands we may need to pass to this app.

    '        End Select

    '    Catch ex As Exception
    '        mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoScreenAction", _
    '                               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
    '    End Try

    'End Sub

    Private Sub subInitializeForm()
        '********************************************************************************************
        'Description: Load up the schedule and put the schedule monitor in motion.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim bSuccess As Boolean

        Try
            Call mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, String.Empty, String.Empty)

            'Create the Zones Colection
            mcolZones = New clsZones(String.Empty)
            moPLC = New clsPLCComm
            moPLC.ZoneName = mcolZones.CurrentZone
            With tmrEventTick
                .Enabled = False
                .Interval = mnPOLL_INTERVAL
            End With


            '********New program-to-program communication object*****************************************
            oIPC = New Paintworks_IPC.clsInterProcessComm() '(gs_COM_ID_SCHEDMAN, , , True) 'RJO 03/09/12
            '********************************************************************************************

            mnDay = DatePart(DateInterval.Weekday, Now)
            If GetDefaultFilePath(msDmonPath, eDir.DmonData, String.Empty, String.Empty) Then
                mbDoDmonArchive = True
                If Not (GetDefaultFilePath(msDmonArchivePath, eDir.DmonArchive, String.Empty, String.Empty)) Then
                    msDmonArchivePath = msDmonPath
                End If
            Else
                msDmonPath = String.Empty
                mbDoDmonArchive = False
            End If

            'Reset the Break Active bit in the PLC DataTable
            Call subWriteToPLC("Z1BreakActive", 0)

            'Read shift and maintenance schedules from schedule.mdb
            bSuccess = bReadMaintDataFromXML()
            If bSuccess Then bSuccess = bReadOptionDataFromXML()
            If bSuccess Then bSuccess = bReadShiftDataFromXML()
            Dim moBackupOptions As New clsBackupOptions
            mbWeeklyReports = moBackupOptions.WeeklyReports
        Catch ex As Exception
            bSuccess = False
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitializeForm", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        Finally
            'Call mWorksComm.SendFRWMMessage("schedmanload," & bSuccess.ToString & ",0,0,0,0", "PW4_Main")
            Dim sMessage(1) As String
            sMessage(0) = "schedmanload"
            sMessage(1) = bSuccess.ToString
            oIPC.WriteControlMsg(gs_COM_ID_PW_MAIN, sMessage)

            If bSuccess Then
                tmrEventTick.Enabled = True
            Else
                'Abort this app
                Application.Exit()
                Me.Close()
            End If
        End Try

    End Sub

    Private Sub subWriteToPLC(ByVal TagName As String, ByVal Data As Integer)
        '********************************************************************************************
        'Description: Writes a single integer value to the PLC data table address corresponding to
        '             TagName (which is all that is required for this app).
        '
        'Parameters: TagName, Data
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sData(0) As String

        Try
            sData(0) = Data.ToString

            With moPLC
                .TagName = TagName
                .PLCData = sData
            End With

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subWriteToPLC", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

#End Region

#Region " Events "

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '********************************************************************************************
        'Description: dont close from the x
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Select Case e.CloseReason
            Case CloseReason.UserClosing
                e.Cancel = True
        End Select
    End Sub

    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description: Runs after class constructor (new)
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim sCultureArg As String = "/culture="

            'If a culture string has been passed in, set the current culture (display language)
            For Each s As String In My.Application.CommandLineArgs
                If s.ToLower.StartsWith(sCultureArg) Then
                    Culture = s.Remove(0, sCultureArg.Length)
                    Exit For
                End If
            Next

            Call subInitializeForm()

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: frmMain_Load", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        Finally

            Me.Left = 111000

        End Try

    End Sub

    Private Sub moPLC_ModuleError(ByVal nErrNum As Integer, ByVal sErrDesc As String, ByVal sModule As String, ByVal AdditionalInfo As String) Handles moPLC.ModuleError
        '********************************************************************************************
        'Description: Log errors from the PLC Comm class.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & sModule & " Routine: moPLC_ModuleError", _
                       "Error: " & nErrNum.ToString & " - " & sErrDesc & vbCrLf & AdditionalInfo)

    End Sub

    Private Sub subArchiveDMONData()
        '********************************************************************************************
        'Description: Periodically check the schedule against the current time.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         09/14/11      Archive DMON files
        '********************************************************************************************
        Dim sDMON_ARCHIVE As String = "DMON_Archive_"
        Try
            If IO.Directory.Exists(msDmonPath) Then
                moMaint = New Maintenance(mcolZones.ActiveZone)
                Dim dtArchiveZipDate As Date = DateAdd(DateInterval.Day, -1, Now)
                Dim sArchiveZipDate As String = Format(dtArchiveZipDate, "yyyyMMdd")
                Dim nArchiveZipDate As Integer = CType(sArchiveZipDate, Integer)
                Dim dtArchiveDelDate As Date = DateAdd(DateInterval.Day, (-1 * moMaint.DiagnosticArchiveDaysToKeep), Now)
                Dim sArchiveDelDate As String = Format(dtArchiveDelDate, "yyyyMMdd")
                Dim nArchiveDelDate As Integer = CType(sArchiveDelDate, Integer)
                Dim dtDmonDelDate As Date = DateAdd(DateInterval.Day, (-1 * moMaint.DiagnosticDaysToKeep), Now)
                Dim sDmonDelDate As String = Format(dtDmonDelDate, "yyyyMMdd")
                Dim nDmonDelDate As Integer = CType(sDmonDelDate, Integer)
                Dim oZip As New ZipFile()
                Dim nFolders As Integer = 1
                Dim sFolder As String = String.Empty
                If (msDmonPath <> msDmonArchivePath) Then
                    nFolders = 2
                End If
                For nFolder As Integer = 1 To nFolders
                    Select Case nFolder
                        Case 1
                            sFolder = msDmonPath
                        Case 2
                            sFolder = msDmonArchivePath
                    End Select
                    Dim sDmonFiles() As String = IO.Directory.GetFiles(sFolder)
                    Dim nDate As Integer = 0
                    'Create a zip file
                    For Each sFileName As String In sDmonFiles
                        Try
                            If InStr(sFileName, sDMON_ARCHIVE) > 0 Then
                                'archive zip file
                                Dim sTmp As String() = sFileName.Split("_".ToCharArray)
                                If sTmp.GetUpperBound(0) > 0 Then
                                    Dim sTmp1 As String() = sTmp(sTmp.GetUpperBound(0)).Split(".".ToCharArray)
                                    If IsNumeric(sTmp1(0)) Then
                                        nDate = CType(sTmp1(0), Integer)
                                        If nDate < nArchiveDelDate Then
                                            IO.File.Delete(sFileName)
                                        End If
                                    End If
                                End If

                            Else
                                'dt file from robot
                                Dim sTmp As String() = sFileName.Split("\".ToCharArray)
                                If sTmp.GetUpperBound(0) > 0 Then
                                    Dim sTmp1 As String() = sFileName.Split("-".ToCharArray)
                                    If sTmp1.GetUpperBound(0) > 0 Then
                                        Dim sTmp2 As String() = sTmp1(1).Split(" ".ToCharArray)
                                        If sTmp2.GetUpperBound(0) > 0 AndAlso sTmp2(0) <> String.Empty AndAlso IsNumeric(sTmp2(0)) Then
                                            Dim nTmp As Integer = CType(sTmp2(0), Integer)
                                            nDate = CType(sTmp2(0), Integer)
                                            If nDate = nArchiveZipDate Then
                                                oZip.UpdateFile(sFileName, String.Empty)
                                            End If
                                            If nDate < nDmonDelDate Then
                                                IO.File.Delete(sFileName)
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        Catch ex As Exception

                        End Try

                    Next
                    oZip.Save(msDmonArchivePath & sDMON_ARCHIVE & sArchiveZipDate & ".zip")

                Next

            End If
        Catch ex As Exception

        Finally
            moMaint = Nothing
        End Try


    End Sub

    Private Sub tmrEventTick_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrEventTick.Tick
        '********************************************************************************************
        'Description: Periodically check the schedule against the current time.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         01/08/10      Make shift active work for any shift, default to true
        ' MSW         05/16/14      Add weekly reports to scheduler 
        '********************************************************************************************
        Dim bBackupActive As Boolean
        Dim bBreakActive As Boolean
        Dim bShiftActive As Boolean
        Dim nEvent As Integer
        Dim nTimeNow As Integer

        tmrEventTick.Enabled = False

        Try
            'Lock in the current time
            Dim sHour As String = CType(DatePart(DateInterval.Hour, Now), String)
            Dim sMin As String = CType(DatePart(DateInterval.Minute, Now), String)
            Dim sSec As String = CType(DatePart(DateInterval.Second, Now), String)
            nTimeNow = nGetTime(sHour, sMin, sSec, 0)

            'Check for day change
            If mbNewDayMemory = False Then
                mbNewDayMemory = bNewDay()
            End If

            If mbWeeklyReports AndAlso mbNewDayMemory Then
                Dim nDay As Integer = DatePart(DateInterval.Weekday, Now)
                If nDay = 1 Then
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: tmrEventTick_Tick", _
                           "Ran Weekly Reports")
                    'Run Weekly Reports
                    Dim sExecPath As String = String.Empty
                    Call mPWCommon.GetDefaultFilePath(sExecPath, eDir.VBApps, String.Empty, String.Empty)
                    Shell(sExecPath & "Reports.exe weekly" & " /culture=" & msCulture, AppWinStyle.Hide)
                End If
            End If

            'Update the cleanup/backup schedule for today if the day changed
            If (mbNewDayMemory = True) And (mScheduleEvents(eSchedule.Clean_Backup).Active) = False Then
                Call bReadMaintDataFromXML()
                Call bReadOptionDataFromXML()
                If mbDoDmonArchive Then
                    Call subArchiveDMONData()
                End If

                mbNewDayMemory = False
            End If
            bBreakActive = False
            bShiftActive = False

            'Check the current time against the schedule to determine if we need to do something
            For nEvent = eSchedule.Shift_1 To eSchedule.Clean_Backup

                Select Case nEvent

                    Case eSchedule.Shift_1, eSchedule.Shift_2, eSchedule.Shift_3
                        'Determine if the shift is active
                        If mScheduleEvents(nEvent).Enabled Then
                            Dim bThisShiftActive As Boolean = bCheckEventActive(nEvent, nTimeNow)
                            bShiftActive = bShiftActive Or bThisShiftActive
                            If bThisShiftActive = True And mScheduleEvents(nEvent).Active = False Then
                                'set bit in plc to reset job count
                                If mbFirstPassComplete Then
                                    Call subWriteToPLC("Z1ResetJobCount", 1)
                                    If mbNotifyStartOfShift Then Call subWriteToPLC("Z1ShiftStartNotify", 1)
                                End If
                            Else
                                'Is it the end of a shift?
                                If mScheduleEvents(nEvent).Active = True And bThisShiftActive = False Then
                                    If mbNotifyEndOfShift Then Call subWriteToPLC("Z1ShiftEndNotify", 1)
                                End If
                            End If
                            mScheduleEvents(nEvent).Active = bThisShiftActive
                        End If 'mSchedule(nEvent).Enabled

                    Case eSchedule.Clean_Backup
                        'Determine if there's anything to do today
                        If mScheduleEvents(nEvent).Flags > 0 Then
                            bBackupActive = bCheckEventActive(nEvent, nTimeNow)
                            If bBackupActive = True Then
                                If mScheduleEvents(nEvent).Active = False Then
                                    mScheduleEvents(nEvent).Active = True
                                    'Shell out the PAINTworks Scheduled Maintenance exe
                                    Dim sExecPath As String = String.Empty

                                    Call mPWCommon.GetDefaultFilePath(sExecPath, eDir.VBApps, String.Empty, String.Empty)
                                    Shell(sExecPath & "PW_Maint.exe " & mScheduleEvents(nEvent).Flags.ToString & _
                                          " /culture=" & msCulture, AppWinStyle.NormalFocus)
                                End If
                            Else
                                mScheduleEvents(nEvent).Active = False
                            End If

                        End If 'mSchedule(nEvent).Flags > 0

                    Case Else ' Breaks
                        '11/23/09 RJO turn BreakActive on if there is no active shift so we don't record downtime.
                        'If Not bShiftActive Then bBreakActive = True

                        If bShiftActive AndAlso mScheduleEvents(nEvent).Enabled Then
                            If bCheckEventActive(nEvent, nTimeNow) Then bBreakActive = True
                        End If

                End Select

            Next 'nEvent

            '01/09/10 RJO turn BreakActive on if there is no active shift so we don't record downtime.
            If Not bShiftActive Then bBreakActive = True

            'Set/Reset the Break Active bit in the PLC if break status changed
            If bBreakActive <> mbBreakActiveMemory Then
                If bBreakActive Then
                    'Set bit
                    Call subWriteToPLC("Z1BreakActive", 1)
                Else
                    'Reset bit
                    Call subWriteToPLC("Z1BreakActive", 0)
                End If
                mbBreakActiveMemory = bBreakActive
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: tmrEventTick_Tick", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

        tmrEventTick.Enabled = True
        mbFirstPassComplete = True

    End Sub

    'Protected Overrides Sub WndProc(ByRef WinMessage As Message)
    '    '********************************************************************************************
    '    'Description:  Intercepts windows messages to this screen and acts on those generated by
    '    '              Paintworks for application screen control.
    '    '
    '    'Parameters: Windows(Message)
    '    'Returns: None()
    '    '
    '    'Modification(history)
    '    '
    '    'Date      By      Reason
    '    '******************************************************************************************** 

    '    Try

    '        If WinMessage.Msg = WM_NULL Then

    '            'This is an Message from a Paintworks application
    '            If WinMessage.LParam.ToInt32 > 0 Then
    '                'This is the header message. LParam holds the character count.
    '                mnMsgLen = WinMessage.LParam.ToInt32
    '                msWinMsg = String.Empty
    '                mnMsgCount = 0
    '            Else
    '                msWinMsg = msWinMsg & Chr(WinMessage.WParam.ToInt32)
    '                mnMsgCount += 1
    '                If mnMsgCount = mnMsgLen Then
    '                    Call subDoScreenAction(mWorksComm.GetScreenCommand(msWinMsg))
    '                    mnMsgLen = 0
    '                End If
    '            End If

    '        End If 'WinMessage.Msg = WM_NULL
    '        MyBase.WndProc(WinMessage)

    '    Catch ex As Exception
    '        mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: WndProc", _
    '                               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
    '    End Try

    'End Sub

    '********New program-to-program communication object******************************************
    Private Sub oIPC_NewMessage(ByVal Schema As String, ByVal DS As DataSet) Handles oIPC.NewMessage
        If Me.InvokeRequired Then
            Dim dNewMessage As New NewMessage_CallBack(AddressOf oIPC_NewMessage)
            Me.BeginInvoke(dNewMessage, New Object() {Schema, DS})
        Else
            Dim DR As DataRow = Nothing

            Select Case Schema.ToLower
                Case oIPC.CONTROL_MSG_SCHEMA.ToLower
                    DR = DS.Tables(Paintworks_IPC.clsInterProcessComm.sTABLE).Rows(0)
                    Call subDoScreenAction(DR)
                Case Else
            End Select
        End If
    End Sub
    '********************************************************************************************


#End Region

End Class
