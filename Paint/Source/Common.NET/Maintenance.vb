' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' America or FANUC LTD Japan immediately upon request. This material
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
' Form/Module: Maintenance (Maintenance.vb)
'
' Description:
' This class module contains the code to read the number of days to keep logged data.
'
' Dependencies:  
'
' Language: Microsoft Visual Basic 2005 .NET
'
' Author: R. Olejniczak
' FANUC Robotics America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'

'    Date       By      Reason                                                        Version
'    11/20/11   MSW     Add edit and save for DMON screen                            4.01.01.00
'    03/28/12   MSW     Move to XML                                                  4.01.03.00
'    02/17/12   MSW     Add PaintWorksBackups separate from RobotBackups             4.01.04.00
'    08/30/13   MSW     Move some DB access code to common files                     4.01.05.00
'    05/16/14   MSW     Add weekly reports to scheduler                              4.01.07.00
'*************************************************************************

Option Compare Text
Option Explicit On
Option Strict On
Imports System.Xml
Imports System.Xml.XPath
Friend Class clsAdditionalFolders
    Private Const msMODULE As String = "AdditionalFolders"
    'Additional Folder(s) Configuration
    Friend Structure udsFolders
        Dim FolderName As String
        Dim FolderPath As String
        Dim IncludeSubfolders As Boolean
    End Structure

    Private mFolders() As udsFolders
    Private mnFolders As Integer
    Friend Property NumFolders() As Integer
        Get
            Return mnFolders
        End Get
        Set(ByVal value As Integer)
            mnFolders = value
        End Set
    End Property
    Friend Property Folders() As udsFolders()
        Get
            Return mFolders
        End Get
        Set(ByVal value As udsFolders())
            mFolders = value
        End Set
    End Property
    Private Sub subGetFolders()
        '********************************************************************************************
        'Description:   Retrieves the additional folders to backup from the Schedule database.
        '
        'Parameters:    None
        'Returns:       None
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         03/28/12      Move DB to XML
        '********************************************************************************************

        Const sXMLTABLE As String = "SchedAdditionalFolders"
        Const sXMLNODE As String = "AdditionalFolders"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
            Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)
                mnFolders = 0
                If oNodeList.Count > 0 Then
                    For Each oNode As XmlNode In oNodeList
                        If oNode.Item("Name").InnerXml <> String.Empty And oNode.Item("FolderPath").InnerXml <> String.Empty Then
                            ReDim Preserve mFolders(mnFolders)
                            With mFolders(mnFolders)
                                .FolderName = oNode.Item("Name").InnerXml
                                .FolderPath = oNode.Item("FolderPath").InnerXml
                                .IncludeSubfolders = CType(oNode.Item("IncludeSubfolders").InnerXml, Boolean)
                            End With

                            mnFolders += 1

                        End If
                    Next
                Else

                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: bLoadFolderDataFromXML", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If
    End Sub

    Public Sub New()
        subGetFolders()
    End Sub
End Class
Friend Class clsExtraCopies
    Private Const msMODULE As String = "ExtraCopies"
    'Additional Folder(s) Configuration
    Friend Structure udsExtraCopies
        Dim FolderFrom As String
        Dim FolderTo As String
        Dim IncludeSubfolders As Boolean
    End Structure
    Private mExtraCopies() As udsExtraCopies
    Private mnExtraCopies As Integer
    Friend Property NumExtraCopies() As Integer
        Get
            Return mnExtraCopies
        End Get
        Set(ByVal value As Integer)
            mnExtraCopies = value
        End Set
    End Property
    Friend Property ExtraCopies() As udsExtraCopies()
        Get
            Return mExtraCopies
        End Get
        Set(ByVal value As udsExtraCopies())
            mExtraCopies = value
        End Set
    End Property
    Private Sub subGetExtraCopies()
        '********************************************************************************************
        'Description:   Retrieves the extra copy settings to backup from the Schedule database.
        '
        'Parameters:    None
        'Returns:       None
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         03/28/12      Move DB to XML
        '********************************************************************************************
        Const sXMLTABLE As String = "SchedExtraCopies"
        Const sXMLNODE As String = "ExtraCopy"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
            Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)
                mnExtraCopies = 0
                If oNodeList.Count > 0 Then
                    For Each oNode As XmlNode In oNodeList
                        If oNode.Item("From").InnerXml <> String.Empty And oNode.Item("To").InnerXml <> String.Empty Then
                            ReDim Preserve mExtraCopies(mnExtraCopies)
                            With mExtraCopies(mnExtraCopies)
                                .FolderFrom = oNode.Item("From").InnerXml
                                .FolderTo = oNode.Item("To").InnerXml
                                .IncludeSubfolders = CType(oNode.Item("IncludeSubfolders").InnerXml, Boolean)
                            End With

                            mnExtraCopies += 1
                        End If
                    Next
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: bLoadFolderDataFromXML", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If

    End Sub

    Public Sub New()
        subGetExtraCopies()
    End Sub
End Class
Friend Class clsBackupOptions
    Private Const msMODULE As String = "clsBackupOptions"
    'File backup options
    Private mbDatabase As Boolean
    Private mbNewFolder As Boolean
    Private mbRobotMaster As Boolean
    Private mbRobotScheduled As Boolean
    Private mbRobotTemp As Boolean
    Private mbRobotImage As Boolean
    Private mbFolders As Boolean
    Private mbExtraCopies As Boolean
    Private msPath As String
    Private mbNotifyStartOfShift As Boolean
    Private mbNotifyEndOfShift As Boolean
    Private mbLogFile As Boolean
    Private mbWeeklyReports As Boolean
    Friend Property Database() As Boolean
        Get
            Return mbDatabase
        End Get
        Set(ByVal value As Boolean)
            mbDatabase = value
        End Set
    End Property
    Friend Property NewFolder() As Boolean
        Get
            Return mbNewFolder
        End Get
        Set(ByVal value As Boolean)
            mbNewFolder = value
        End Set
    End Property
    Friend Property RobotMaster() As Boolean
        Get
            Return mbRobotMaster
        End Get
        Set(ByVal value As Boolean)
            mbRobotMaster = value
        End Set
    End Property
    Friend Property RobotScheduled() As Boolean
        Get
            Return mbRobotScheduled
        End Get
        Set(ByVal value As Boolean)
            mbRobotScheduled = value
        End Set
    End Property
    Friend Property RobotTemp() As Boolean
        Get
            Return mbRobotTemp
        End Get
        Set(ByVal value As Boolean)
            mbRobotTemp = value
        End Set
    End Property
    Friend Property RobotImage() As Boolean
        Get
            Return mbRobotImage
        End Get
        Set(ByVal value As Boolean)
            mbRobotImage = value
        End Set
    End Property
    Friend Property Folders() As Boolean
        Get
            Return mbFolders
        End Get
        Set(ByVal value As Boolean)
            mbFolders = value
        End Set
    End Property
    Friend Property ExtraCopies() As Boolean
        Get
            Return mbExtraCopies
        End Get
        Set(ByVal value As Boolean)
            mbExtraCopies = value
        End Set
    End Property
    Friend Property Path() As String
        Get
            Return msPath
        End Get
        Set(ByVal value As String)
            msPath = value
        End Set
    End Property
    Friend Property NotifyStartOfShift() As Boolean
        Get
            Return mbNotifyStartOfShift
        End Get
        Set(ByVal value As Boolean)
            mbNotifyStartOfShift = value
        End Set
    End Property
    Friend Property NotifyEndOfShift() As Boolean
        Get
            Return mbNotifyEndOfShift
        End Get
        Set(ByVal value As Boolean)
            mbNotifyEndOfShift = value
        End Set
    End Property
    Friend Property WeeklyReports() As Boolean
        Get
            Return mbWeeklyReports
        End Get
        Set(ByVal value As Boolean)
            mbWeeklyReports = value
        End Set
    End Property
    Friend Property LogFile() As Boolean
        Get
            Return mbLogFile
        End Get
        Set(ByVal value As Boolean)
            mbLogFile = value
        End Set
    End Property

    Private Sub subGetBackupOptions()
        '********************************************************************************************
        'Description:   Reads the Options table in the Schedule databse and sets modular variables
        '
        'Parameters:  None
        '
        'Returns:    None
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         03/28/12      Move DB to XML
        ' MSW         05/16/14      Add weekly reports to scheduler 
        '********************************************************************************************
        Const sXMLTABLE As String = "SchedOptions"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
            Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                If oMainNode IsNot Nothing Then
                    mbDatabase = CType(oMainNode.Item("Database").InnerXml, Boolean)
                    mbFolders = CType(oMainNode.Item("AdditionalFolders").InnerXml, Boolean)
                    mbExtraCopies = CType(oMainNode.Item("ExtraCopies").InnerXml, Boolean)
                    mbNewFolder = CType(oMainNode.Item("NewFolder").InnerXml, Boolean)
                    msPath = oMainNode.Item("Path").InnerXml
                    mbRobotMaster = CType(oMainNode.Item("RobotMaster").InnerXml, Boolean)
                    mbRobotScheduled = CType(oMainNode.Item("RobotScheduled").InnerXml, Boolean)
                    mbRobotImage = CType(oMainNode.Item("RobotImage").InnerXml, Boolean)
                    mbRobotTemp = CType(oMainNode.Item("RobotTemp").InnerXml, Boolean)
                    mbNotifyStartOfShift = CType(oMainNode.Item("NotifyStartShift").InnerXml, Boolean)
                    mbNotifyEndOfShift = CType(oMainNode.Item("NotifyEndShift").InnerXml, Boolean)
                    mbLogFile = CType(oMainNode.Item("Logfile").InnerXml, Boolean)
                    If oMainNode.InnerXml.Contains("WeeklyReports") Then
                        Try
                            mbWeeklyReports = CType(oMainNode.Item("WeeklyReports").InnerXml, Boolean) 'MSW 5/14/14
                        Catch ex As Exception
                            mbWeeklyReports = False
                        End Try
                    Else
                        mbWeeklyReports = False
                    End If
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subGetBackupOptions", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If



    End Sub

    Public Sub New()
        subGetBackupOptions()
    End Sub
End Class
Friend Class Maintenance
#Region " Declarations "

    Private mbLoadError As Boolean
    Private mnAlarmDaysToKeep As New clsIntValue
    Private mnChangeDaysToKeep As New clsIntValue
    Private mnDiagnosticArchiveDaysToKeep As New clsIntValue
    Private mnDiagnosticDaysToKeep As New clsIntValue
    Private mnDriveFullThreshold As New clsIntValue
    Private mnProdDaysToKeep As New clsIntValue
    Private mnRobotBackupsToKeep As New clsIntValue
    Private mnPaintWorksBackupsToKeep As New clsIntValue
    Private mnVisionDaysToKeep As New clsIntValue
    Private moZone As clsZone
    Private Const mn_MAX_DMON_DAYS As Integer = 7
    Private Const mn_MAX_ARCHIVE_DAYS As Integer = 365
    Private Const mn_MAX_LOG_DAYS As Integer = 365
    Private Const mn_MAX_ROBOT_BACKUPS As Integer = 30
    Private Const msMODULE As String = "Maintenance"


#End Region

#Region " Properties "
    Friend Property AlarmDaysToKeep() As Integer
        '********************************************************************************************
        'This property gets/sets the number of days to keep alarm records.
        '
        'Parameters: none
        'Returns: The current value of AlarmDaysToKeep
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mnAlarmDaysToKeep.Value
        End Get

        Set(ByVal value As Integer)
            mnAlarmDaysToKeep.Value = value
        End Set

    End Property

    Friend Property ChangeDaysToKeep() As Integer
        '********************************************************************************************
        'This property gets/sets the number of days to keep change log records.
        '
        'Parameters: none
        'Returns: The current value of ChangeDaysToKeep
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mnChangeDaysToKeep.Value
        End Get

        Set(ByVal value As Integer)
            mnChangeDaysToKeep.Value = value
        End Set

    End Property

    Friend Property DiagnosticArchiveDaysToKeep() As Integer
        '********************************************************************************************
        'This property gets/sets the number of days to keep archived diagnostic (DMON) files.
        '
        'Parameters: none
        'Returns: The current value of DiagnosticArchiveDaysToKeep
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mnDiagnosticArchiveDaysToKeep.Value
        End Get

        Set(ByVal value As Integer)
            mnDiagnosticArchiveDaysToKeep.Value = value
        End Set

    End Property

    Friend Property DiagnosticDaysToKeep() As Integer
        '********************************************************************************************
        'This property gets/sets the number of days to keep diagnostic (DMON) data files.
        '
        'Parameters: none
        'Returns: The current value of DiagnosticDaysToKeep
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mnDiagnosticDaysToKeep.Value
        End Get

        Set(ByVal value As Integer)
            mnDiagnosticDaysToKeep.Value = value
        End Set

    End Property

    Friend Property DriveFullThreshold() As Integer
        '********************************************************************************************
        'This property gets/sets the minimum percent free disk space required to perform a PWMaint 
        'backup operation.
        '
        'Parameters: none
        'Returns: The current value of DiagnosticDaysToKeep
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mnDriveFullThreshold.Value
        End Get

        Set(ByVal value As Integer)
            mnDriveFullThreshold.Value = value
        End Set

    End Property

    Friend Property ProdDaysToKeep() As Integer
        '********************************************************************************************
        'This property gets/sets the number of days to keep production log records.
        '
        'Parameters: none
        'Returns: The current value of ProdDaysToKeep
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mnProdDaysToKeep.Value
        End Get

        Set(ByVal value As Integer)
            mnProdDaysToKeep.Value = value
        End Set

    End Property

    Friend Property RobotBackupsToKeep() As Integer
        '********************************************************************************************
        'This property gets/sets the number of robot backups to keep.
        '
        'Parameters: none
        'Returns: The current value of RobotBackupsToKeep
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mnRobotBackupsToKeep.Value
        End Get

        Set(ByVal value As Integer)
            mnRobotBackupsToKeep.Value = value
        End Set

    End Property
    Friend Property PaintWorksBackupsToKeep() As Integer
        '********************************************************************************************
        'This property gets/sets the number of robot backups to keep.
        '
        'Parameters: none
        'Returns: The current value of RobotBackupsToKeep
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         02/17/12      Add PaintWorksBackups separate from RobotBackups
        '********************************************************************************************

        Get
            Return mnPaintWorksBackupsToKeep.Value
        End Get

        Set(ByVal value As Integer)
            mnPaintWorksBackupsToKeep.Value = value
        End Set

    End Property

    Friend Property VisionDaysToKeep() As Integer
        '********************************************************************************************
        'This property gets/sets the number of days to keep vision log records.
        '
        'Parameters: none
        'Returns: The current value of VisionDaysToKeep
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mnVisionDaysToKeep.Value
        End Get

        Set(ByVal value As Integer)
            mnVisionDaysToKeep.Value = value
        End Set

    End Property
    '********************************************************************************************
    Friend Property oAlarmDaysToKeep() As clsIntValue
        '********************************************************************************************
        'This property gets/sets the number of days to keep alarm records.
        '
        'Parameters: none
        'Returns: The current value of AlarmDaysToKeep
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mnAlarmDaysToKeep
        End Get

        Set(ByVal value As clsIntValue)
            mnAlarmDaysToKeep = value
        End Set

    End Property

    Friend Property oChangeDaysToKeep() As clsIntValue
        '********************************************************************************************
        'This property gets/sets the number of days to keep change log records.
        '
        'Parameters: none
        'Returns: The current value of ChangeDaysToKeep
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mnChangeDaysToKeep
        End Get

        Set(ByVal value As clsIntValue)
            mnChangeDaysToKeep = value
        End Set

    End Property

    Friend Property oDiagnosticArchiveDaysToKeep() As clsIntValue
        '********************************************************************************************
        'This property gets/sets the number of days to keep archived diagnostic (DMON) files.
        '
        'Parameters: none
        'Returns: The current value of DiagnosticArchiveDaysToKeep
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mnDiagnosticArchiveDaysToKeep
        End Get

        Set(ByVal value As clsIntValue)
            mnDiagnosticArchiveDaysToKeep = value
        End Set

    End Property

    Friend Property oDiagnosticDaysToKeep() As clsIntValue
        '********************************************************************************************
        'This property gets/sets the number of days to keep diagnostic (DMON) data files.
        '
        'Parameters: none
        'Returns: The current value of DiagnosticDaysToKeep
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mnDiagnosticDaysToKeep
        End Get

        Set(ByVal value As clsIntValue)
            mnDiagnosticDaysToKeep = value
        End Set

    End Property

    Friend Property oDriveFullThreshold() As clsIntValue
        '********************************************************************************************
        'This property gets/sets the minimum percent free disk space required to perform a PWMaint 
        'backup operation.
        '
        'Parameters: none
        'Returns: The current value of DiagnosticDaysToKeep
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mnDriveFullThreshold
        End Get

        Set(ByVal value As clsIntValue)
            mnDriveFullThreshold = value
        End Set

    End Property

    Friend Property oProdDaysToKeep() As clsIntValue
        '********************************************************************************************
        'This property gets/sets the number of days to keep production log records.
        '
        'Parameters: none
        'Returns: The current value of ProdDaysToKeep
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mnProdDaysToKeep
        End Get

        Set(ByVal value As clsIntValue)
            mnProdDaysToKeep = value
        End Set

    End Property


    Friend Property oPaintWorksBackupsToKeep() As clsIntValue
        '********************************************************************************************
        'This property gets/sets the number of robot backups to keep.
        '
        'Parameters: none
        'Returns: The current value of RobotBackupsToKeep
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         02/17/12      Add PaintWorksBackups separate from RobotBackups
        '********************************************************************************************

        Get
            Return mnPaintWorksBackupsToKeep
        End Get

        Set(ByVal value As clsIntValue)
            mnPaintWorksBackupsToKeep = value
        End Set

    End Property
    Friend Property oRobotBackupsToKeep() As clsIntValue
        '********************************************************************************************
        'This property gets/sets the number of robot backups to keep.
        '
        'Parameters: none
        'Returns: The current value of RobotBackupsToKeep
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mnRobotBackupsToKeep
        End Get

        Set(ByVal value As clsIntValue)
            mnRobotBackupsToKeep = value
        End Set

    End Property

    Friend Property oVisionDaysToKeep() As clsIntValue
        '********************************************************************************************
        'This property gets/sets the number of days to keep vision log records.
        '
        'Parameters: none
        'Returns: The current value of VisionDaysToKeep
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mnVisionDaysToKeep
        End Get

        Set(ByVal value As clsIntValue)
            mnVisionDaysToKeep = value
        End Set

    End Property


    '********************************************************************************************
    Friend Property LoadError() As Boolean
        '********************************************************************************************
        'This property gets/sets a flag that indicates a database read/data error occurred during
        'the call to RefreshInfo.
        '
        'Parameters: none
        'Returns: The current value of ProdDaysToKeep
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return mbLoadError
        End Get

        Set(ByVal value As Boolean)
            mbLoadError = value
        End Set

    End Property

#End Region

#Region " Routines "

    Friend Function DriveFull(ByVal DriveLetter As String) As Boolean
        '********************************************************************************************
        'Description: This function returns true if the drive is full (the amount of free space is
        '             below the defined threshold (percent of total drive space). 
        '
        'Parameters: Drive Letter
        'Returns: True if disk is full
        '
        'Modification History:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim oDriveInfo As System.IO.DriveInfo
        Dim dPercentFree As Double

        Try
            DriveLetter = Strings.Left(DriveLetter, 1) & ":\"
            oDriveInfo = My.Computer.FileSystem.GetDriveInfo(DriveLetter)
            dPercentFree = (oDriveInfo.AvailableFreeSpace / oDriveInfo.TotalSize) * 100

            DriveFull = DriveFullThreshold > dPercentFree

        Catch ex As Exception
            mDebug.WriteEventToLog(" Module: Maintenance Routine: DriveFull", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            DriveFull = True

        End Try

    End Function

    Friend Sub RefreshInfo()
        '********************************************************************************************
        'Description: This subroutine reads the number of days to keep the log database records and 
        '             diagnostic data files and updates the property variables. 
        '
        'Parameters: None
        'Returns: None
        '
        'Modification History:
        '
        ' By          Date          Reason
        ' MSW         03/28/12      Move to XML
        ' MSW         02/17/12      Add PaintWorksBackups separate from RobotBackups
        '********************************************************************************************
        Const sXMLTABLE As String = "DaysToKeep"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Dim bSuccess As Boolean = False

        Try
            If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
                Try
                    oXMLDoc.Load(sXMLFilePath)
                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    If oMainNode IsNot Nothing Then
                        AlarmDaysToKeep = CType(oMainNode.Item("Alarm").InnerXml, Integer)
                        mnAlarmDaysToKeep.MaxValue = mn_MAX_LOG_DAYS
                        mnAlarmDaysToKeep.MinValue = 1
                        mnAlarmDaysToKeep.Update()
                        ChangeDaysToKeep = CType(oMainNode.Item("Change").InnerXml, Integer)
                        mnChangeDaysToKeep.MaxValue = mn_MAX_LOG_DAYS
                        mnChangeDaysToKeep.MinValue = 1
                        mnChangeDaysToKeep.Update()
                        DiagnosticArchiveDaysToKeep = CType(oMainNode.Item("DiagnosticArchive").InnerXml, Integer)
                        mnDiagnosticArchiveDaysToKeep.MaxValue = mn_MAX_ARCHIVE_DAYS
                        mnDiagnosticArchiveDaysToKeep.MinValue = 1
                        mnDiagnosticArchiveDaysToKeep.Update()
                        DiagnosticDaysToKeep = CType(oMainNode.Item("Diagnostic").InnerXml, Integer)
                        mnDiagnosticDaysToKeep.MaxValue = mn_MAX_DMON_DAYS
                        mnDiagnosticDaysToKeep.MinValue = 1
                        mnDiagnosticDaysToKeep.Update()
                        DriveFullThreshold = CType(oMainNode.Item("DriveFullThreshold").InnerXml, Integer)
                        mnDriveFullThreshold.MaxValue = 99
                        mnDriveFullThreshold.MinValue = 1
                        mnDriveFullThreshold.Update()
                        ProdDaysToKeep = CType(oMainNode.Item("Production").InnerXml, Integer)
                        mnProdDaysToKeep.MaxValue = mn_MAX_LOG_DAYS
                        mnProdDaysToKeep.MinValue = 1
                        mnProdDaysToKeep.Update()
                        RobotBackupsToKeep = CType(oMainNode.Item("RobotBackups").InnerXml, Integer)
                        mnRobotBackupsToKeep.MaxValue = mn_MAX_ROBOT_BACKUPS
                        mnRobotBackupsToKeep.MinValue = 1
                        mnRobotBackupsToKeep.Update()
                        Try
                            PaintWorksBackupsToKeep = CType(oMainNode.Item("PaintWorksBackups").InnerXml, Integer)
                            mnPaintWorksBackupsToKeep.MaxValue = mn_MAX_ROBOT_BACKUPS
                            mnPaintWorksBackupsToKeep.MinValue = 1
                            mnPaintWorksBackupsToKeep.Update()
                        Catch ex As Exception
                            PaintWorksBackupsToKeep = RobotBackupsToKeep
                            mnPaintWorksBackupsToKeep.MaxValue = mn_MAX_ROBOT_BACKUPS
                            mnPaintWorksBackupsToKeep.MinValue = 1
                            mnPaintWorksBackupsToKeep.Update()
                        End Try
                        VisionDaysToKeep = CType(oMainNode.Item("Vision").InnerXml, Integer)
                        mnVisionDaysToKeep.MaxValue = mn_MAX_LOG_DAYS
                        mnVisionDaysToKeep.MinValue = 1
                        mnVisionDaysToKeep.Update()
                    End If
                Catch ex As Exception
                    LoadError = True
                    mDebug.WriteEventToLog(" Module: Maintenance Routine: RefreshInfo", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
            End If


        Catch ex As Exception

            LoadError = True
            mDebug.WriteEventToLog(" Module: Maintenance Routine: RefreshInfo", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

        End Try

    End Sub

    Friend Sub SaveToDatabase()
        '********************************************************************************************
        'Description: Save changes to the SQL DB. 
        '
        'Parameters: None
        'Returns: None
        '
        'Modification History:
        '
        ' By          Date          Reason
        ' MSW         10/17/11      Add save method for DMON setup screen
        ' MSW         03/28/12      Move to XML
        ' MSW         02/17/12      Add PaintWorksBackups separate from RobotBackups
        '********************************************************************************************
        Const sXMLTABLE As String = "DaysToKeep"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Dim bSuccess As Boolean = False

        Try
            If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLTABLE & ".XML") Then
                Try
                    oXMLDoc.Load(sXMLFilePath)
                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    If oMainNode IsNot Nothing Then
                        oMainNode.Item("Alarm").InnerXml = AlarmDaysToKeep.ToString
                        mnAlarmDaysToKeep.Update()
                        oMainNode.Item("Change").InnerXml = ChangeDaysToKeep.ToString
                        mnChangeDaysToKeep.Update()
                        oMainNode.Item("DiagnosticArchive").InnerXml = DiagnosticArchiveDaysToKeep.ToString
                        mnDiagnosticArchiveDaysToKeep.Update()
                        oMainNode.Item("Diagnostic").InnerXml = DiagnosticDaysToKeep.ToString
                        mnDiagnosticDaysToKeep.Update()
                        oMainNode.Item("DriveFullThreshold").InnerXml = DriveFullThreshold.ToString
                        mnDriveFullThreshold.Update()
                        oMainNode.Item("Production").InnerXml = ProdDaysToKeep.ToString
                        mnProdDaysToKeep.Update()
                        oMainNode.Item("RobotBackups").InnerXml = RobotBackupsToKeep.ToString
                        mnRobotBackupsToKeep.Update()
                        oMainNode.Item("PaintWorksBackups").InnerXml = RobotBackupsToKeep.ToString
                        mnPaintWorksBackupsToKeep.Update()
                        oMainNode.Item("Vision").InnerXml = VisionDaysToKeep.ToString
                        mnVisionDaysToKeep.Update()
                    End If
                Catch ex As Exception
                    LoadError = True
                    mDebug.WriteEventToLog(" Module: Maintenance Routine: SaveToDatabase", _
                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
            End If


        Catch ex As Exception

            LoadError = True
            mDebug.WriteEventToLog(" Module: Maintenance Routine: SaveToDatabase", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

        End Try





    End Sub

#End Region

#Region " Events "

    Public Sub New(ByVal Zone As clsZone)
        '********************************************************************************************
        'Description: Class constructor
        '
        'Parameters: 
        'Returns: 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        MyBase.New()

        moZone = Zone
        Call RefreshInfo()

    End Sub

#End Region


End Class
