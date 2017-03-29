'
' This material is the joint property of FANUC Robotics North America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics North America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2009
' FANUC Robotics North America
' FANUC LTD Japan
'
' Form: frmMain.frm
'
' Dependencies: 
'
' Description:   PAINTworks Scheduled Maintenance
'
' The purpose of this form is to control, monitor and log the execution of PAINTworks GUI
' computer schedled maintenance operations.
'
' Author: R. Olejniczak
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
' Date        By      Reason                                                              Version
' 11/12/09    MSW     subBackupDB - Copy servername code from SQLClass - VB had an attitude 
'                     about using the same variable from clsSQLAccess.vb
' 02/02/10    RJO     Added status for waiting for internal robot backup. Added detail to 
'                     subFileBackup error status.
' BTK      02/09/10   subRobotBackup set bNoFilter to false so we filter out undesired files.
'                     bFilter added _backup_ and klevaxdf.vr so they are filtered out.
' BTK      03/22/11   subFileBackup added code to delete old backups from c:\paint\PW Robot Backups.
'                     Added DisplayCulture property due to change is clsSQLAccess referencing
'                     frmMain.DisplayCulture.  bFilter changed to not backup -bckedt-.tp or 
'                     -bcked8-.tp or -bcked9-.tp
' 09/14/11    MSW     Assemble a standard version of everything                           4.1.0.0
' 12/2/11     MSW     SQL named instance updates, named robot folder option               4.1.1.0
' 01/24/12    MSW     Change to new Interprocess Communication in clsInterProcessComm   4.01.01.01
' 02/15/12    MSW     Print handling updates and force 32 bit build on 64 bit systems   4.01.01.02
' 03/22/12    RJO     Removed reference to mWorksComm                                   4.01.02.00
' 03/28/12    MSW     SQL to XML changes                                                4.01.03.00
'                     Add some DoEvent calls to smooth out the messages
' 09/19/12    MSW     copy flexible server name code from clsSQLAccess                  4.01.03.01
' 11/29/12    MSW     subFileBackup - Change DB copy to maintain file structure in      4.01.04.00
'                     backups, also copy *.mdf since we're stopping the server now.
' 02/17/12    MSW     Add PaintWorksBackups separate from RobotBackups                  4.01.04.01
' 04/01/13    RJO     Modified event reporting to differentiate between auto backup to  4.01.04.02
'                     FRA: timeout vs. error. Added code to read timeout value in ms
'                     from plc tag Z<ZoneNumber>AutoBackupTimeout. The value in this
'                     plc register should also be used in the PLC program to load the
'                     presets of the auto backup timers.
' 04/16/13    MSW     Add Canadian language files                                       4.01.05.00
' 06/05/13    MSW     Use fixed culture to write the date                               4.01.05.01
' 07/09/13    MSW     Update and standardize logos                                      4.01.05.02
' 08/20/13    MSW     Progress, Status - Add error handler so it doesn't get hung up    4.01.05.03
' 08/30/13    MSW     Move some DB access code to common files                          4.01.05.04
' 09/30/13    MSW     Save screenshots as jpegs, PLC DLL                                4.01.06.00
' 11/21/13    MSW     Add error message details to event log                            4.01.06.01
' 02/13/14    MSW     Switch cross-thread handling to BeginInvoke call                  4.01.07.00
'************************************************************************************************


Imports System.Data.Sql
Imports System.Data.SqlClient
Imports Microsoft.SqlServer.Management.Smo.Wmi

Imports Microsoft.SqlServer.Management.Smo
Imports Microsoft.SqlServer.Management.Common
Imports ConnStat = FRRobotNeighborhood.FRERNConnectionStatusConstants
Imports System.Xml
Imports System.Xml.XPath
Imports clsPLCComm = PLCComm.clsPLCComm

Public Class frmMain

#Region " Declares "

    '#If PaintWorksSA Then
    Private msSQLSERVERNAME As String = "PAINTWORKS"
    '#Else
    '    Private msSQLSERVERNAME As String = "SQLEXPRESS"
    '#End If

    Private Const mnAUTO_SHUTDOWN_DELAY As Integer = 10
    Private Const msMODULE As String = "frmMain"
    Private Const msSCREEN_DUMP_NAME As String = "Maint_PWMaint.jpg"
    Friend Const msSCREEN_NAME As String = "PWMaint"

    '03/22/11 BTK Added for clsSQLAccess change.
    Private msCulture As String = "en-US" 'Default to English

    Private WithEvents mcolControllers As clsControllers = Nothing
    Private mcolZones As clsZones = Nothing
    Private moMaint As Maintenance = Nothing
    Private moAdditionalFolders As clsAdditionalFolders = Nothing
    Private moExtraCopies As clsExtraCopies = Nothing
    Private moBackupOptions As clsBackupOptions = Nothing

    Private WithEvents moPLC As clsPLCComm = Nothing
    Private msLatestRobtBackups As String = String.Empty
    Private mbAutoReportDone As Boolean
    Private mbBackupErr() As Boolean
    Private mbBackupTimeout() As Boolean
    Private mbInternalBackupReqd As Boolean
    Private mbShowRobots As Boolean
    Private mnAutoBackupTimeout As Integer 'RJO 04/01/13
    Private mnElapsedTime As Integer
    Private mnProgress As Integer
    Private msAppFilePath As String = String.Empty
    Private msDatabasePath As String = String.Empty
    Private msLogFilePath As String = String.Empty
    Private mPrintHtml As clsPrintHtml

    Shared oMC As ManagedComputer = Nothing

    '********New program-to-program communication object******************************************
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet)
    Private Delegate Sub ControllerStatusChange(ByVal Controller As clsController)

    '********************************************************************************************


    Public Enum eFlags
        Cleanup = 1
        RobotBackup = 2
        FileBackup = 4
        ReportGen = 8
    End Enum

    'this is some hokeyness for statusstrip sizing and hopefully will be replaced
    Friend Structure tContSize
        Dim ProgBarVisibleSize As Integer
        Dim ProgBarInvisSize As Integer
        Dim SpaceLabelVisbleSize As Integer
        Dim SpaceLabelInvisSize As Integer
    End Structure

    Friend gtSSSize As tContSize



#End Region

#Region " Properties "

    Friend Property Progress() As Integer
        '********************************************************************************************
        'Description:  run the progress bar
        '
        'Parameters: 1 to 100 percent
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/20/13  MSW     Add error handler so it doesn't get hung up during exit
        ' 09/30/13  MSW     Use the control's min and max instead of literal constants
        '********************************************************************************************
        Set(ByVal Value As Integer)
            Try
                If Value < tspProgress.Minimum Then Value = tspProgress.Minimum
                If Value > tspProgress.Maximum Then Value = tspProgress.Minimum
                mnProgress = Value
                tspProgress.Value = mnProgress
                If mnProgress > 0 And mnProgress < 100 Then
                    lblSpacer.Width = gtSSSize.SpaceLabelInvisSize
                    tspProgress.Visible = True
                Else
                    lblSpacer.Width = gtSSSize.SpaceLabelVisbleSize
                    tspProgress.Visible = False
                End If
                stsStatus.Invalidate()
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Property: Progress", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
        Get
            Return mnProgress
        End Get
    End Property

    Friend Property Status(Optional ByVal StatusStripOnly As Boolean = False) As String
        '********************************************************************************************
        'Description:  write status messages to listbox and statusbar
        '
        'Parameters: If StatusbarOnly is true, doesn't write to listbox
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/20/13  MSW     Add error handler so it doesn't get hung up during exit
        '********************************************************************************************

        Get
            Return stsStatus.Text
        End Get

        Set(ByVal Value As String)
            Try
                If StatusStripOnly = False Then
                    mPWCommon.AddToStatusBox(lstStatus, Value)
                End If
                stsStatus.Items("lblStatus").Text = Value 'Strings.Replace(Value, vbTab, "  ")
                stsStatus.Items("lblStatus").Invalidate()
                Application.DoEvents()
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Property: Status", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set

    End Property

#End Region

#Region " Routines "
    Friend ReadOnly Property DisplayCulture() As CultureInfo
        '********************************************************************************************
        'Description:  The Culture Club
        '
        'Parameters: None
        'Returns:    CultureInfo for current culture.
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/22/11  BTK     Added due to change is clsSQLAccess referencing frmMain.DisplayCulture
        '********************************************************************************************

        Get
            Return New CultureInfo(msCulture)
        End Get

    End Property

    Private Function bFilter(ByVal FileName As String) As Boolean
        '********************************************************************************************
        'Description:  This function weeds out the files like .VA and .LS files that don't need to be 
        '              backed up.
        '
        'Parameters:   FileName (from a robot controller)
        'Returns:      True if FileName should be backed up
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' BTK       02/09/10        Can't backup _backup_ or klevaxdf.vr
        ' MSW       01/05/11        Only exclude specific files.
        ' BTK      03/22/11     Don't backup -bckedt-.tp or -bcked8-.tp
        '                       or -bcked9-.tp
        '********************************************************************************************
        Dim sExtension As String = Strings.Right(FileName, Strings.Len(FileName) - Strings.InStr(FileName, "."))

        'Do the exceptions first
        Select Case FileName.ToLower
            Case "erract.ls", "errall.ls", "errapp.ls", "errcomm.ls", "errcurr.ls", "errext.ls", _
                 "errmot.ls", "errpwd.ls", "errsys.ls", "hist.ls", "logbook.ls", "memcheck.dg", _
                 "summary.dg"

                Return True
            Case "_backup_", "klevaxdf.vr", "-bckedt-.tp", "-bcked8-.tp", "-bcked9-.tp"
                Return False
        End Select

        'Select Case sExtension.ToLower
        '    Case "cm", "dat", "df", "dt", "io", "sv", "tp", "vr", "zip"
        Return True
        '    Case Else
        '        Return False
        'End Select

    End Function

    Private Function nRobotBackupCount(ByRef OldestFolder As String, _
                                       ByVal Path As String) As Integer
        '********************************************************************************************
        'Description:  Returns the number of dated robot backup folders
        '
        'Parameters:   OldestFolder - string to hold the oldest folder name
        '              Path - string to tell nRobotBackupCount where to look for backup folders
        'Returns:      Backup folder count, name of the oldest folder
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Try
            Dim sDirs() As String = System.IO.Directory.GetDirectories(Path, "????_??_??_????")
            Dim dtOldest As DateTime = Now

            nRobotBackupCount = sDirs.GetLength(0)

            If nRobotBackupCount > 0 Then
                For Each sDir As String In sDirs
                    Dim sParts() As String = Split(sDir, "_")
                    Dim nYear As Integer = CType(Strings.Right(sParts(0), 4), Integer)
                    Dim nMonth As Integer = CType(sParts(1), Integer)
                    Dim nDay As Integer = CType(sParts(2), Integer)
                    Dim nHour As Integer = CType(Strings.Left(sParts(3), 2), Integer)
                    Dim nMinute As Integer = CType(Strings.Right(sParts(3), 2), Integer)
                    Dim dtBackup As New System.DateTime(nYear, nMonth, nDay, nHour, nMinute, 0)

                    If dtBackup < dtOldest Then
                        OldestFolder = sDir
                        dtOldest = dtBackup
                    End If
                Next 'sDir
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: nRobotBackupCount", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Function

    Private Function sNewFolder() As String
        '********************************************************************************************
        'Description:   Returns a backup folder name based on the current date and time
        '               Format = yyyy_mm_dd_hhnn
        'Parameters:   none
        'Returns:    foldername
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim sTemp As String = String.Empty
        Dim dtNow As DateTime = Now

        sTemp = DatePart(DateInterval.Year, dtNow).ToString & "_" & _
                Strings.Format(DatePart(DateInterval.Month, dtNow), "00") & "_" & _
                Strings.Format(DatePart(DateInterval.Day, dtNow), "00") & "_" & _
                Strings.Format(DatePart(DateInterval.Hour, dtNow), "00") & _
                Strings.Format(DatePart(DateInterval.Minute, dtNow), "00")

        Return sTemp

    End Function

    Private Function sRemoveSlash(ByVal Path As String) As String
        '********************************************************************************************
        'Description:  Removes the trailing slash ("\") from the supplied path if it exists
        'Parameters:   Path
        'Returns:      Path without trailing slash
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        If Strings.right(Path, 1) = "\" Then
            sRemoveSlash = Strings.Left(Path, Strings.Len(Path) - 1)
        Else
            sRemoveSlash = Path
        End If

    End Function

    Private Sub subAutoGenReports()
        '********************************************************************************************
        'Description: Calls Automatic Report Generation exe.
        '
        'Parameters:  None
        '
        'Returns:    None
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim nProcID As Integer

        Call subPostStatusMsg(gpsRM.GetString("psAUTO_REPORT_MSG", frmStart.DisplayCulture))
        nProcID = Shell(msAppFilePath & "PWReportGen.exe", AppWinStyle.MinimizedNoFocus, False, -1)
        'TODO - Are we even planning on using this stuff?
        'TODO - Maybe need to use the Shell Wait and Timeout features here to clean this up?

    End Sub

    Private Sub subBackupDB(ByVal DBName As String)
        '********************************************************************************************
        'Description: Creates a backup of the (DBName) SQL server database.
        '
        'Parameters: DBName - The database file name (ex. "Alarm Log.mdf")
        'Returns:    none
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         11/12/09      Copy servername code from SQLClass - VB had an attitude about using the same variable
        ' MSW         05/06/11      Detach sql DBs so the files can be copied
        ' MSW         09/19/12      copy flexible server name code from clsSQLAccess
        '********************************************************************************************
        Dim oCulture As CultureInfo = frmStart.DisplayCulture

        Try
            Dim dtExpire As Date
            Dim oBackup As New Backup
            Dim oBdi As BackupDeviceItem
            Dim oBuilder As New SqlClient.SqlConnectionStringBuilder("Network Address=(local)")

            Dim oServer As New Server
            Dim sBackupFileName As String = "dbo." & Strings.Replace(DBName.ToLower, "mdf", "bak")
            Dim sBackupPath As String = msDatabasePath & "Backup"
            Dim sDBName As String = Strings.Left(DBName, Strings.Len(DBName) - 4)
            If IO.Directory.Exists(sBackupPath) = False Then
                IO.Directory.CreateDirectory(sBackupPath)
            End If
            'Backup DBName to backup folder
            'Connect to the local, default instance of SQL Server.
            With oBuilder
                .IntegratedSecurity = True
                .InitialCatalog = sDBName
                .ContextConnection = False 'default
                .UserInstance = False
                .MultipleActiveResultSets = True ' not default
                .LoadBalanceTimeout = 30 ' not default
                .AsynchronousProcessing = True ' not default
                .AttachDBFilename = msDatabasePath & DBName
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'MSW 9/19/12 - copy flexible server name code from clsSQLAccess
                'Declare and create an instance of the ManagedComputer object that 
                'represents the WMI Provider services.
                If oMC Is Nothing Then
                    ''Dim mc As ManagedComputer
                    ' No remote maintenance
                    'If mcolZones.ActiveZone.IsRemoteZone Then
                    '    oMC = New ManagedComputer(mcolZones.ActiveZone.ServerName)
                    'Else
                    oMC = New ManagedComputer()
                    'End If
                End If
                'Reference the Microsoft SQL Server express 2005 service.
                'Somebody forgot to use the constant
                'Deal with GM's setup.  Force the DB name
                Dim sTmpString As String = String.Empty
                For Each tmpsvc As Service In oMC.Services
                    'MSW 12/06/11 - Use prefered server if available, if not, try anything that's there
                    If tmpsvc.Name.StartsWith("MSSQL$" & msSQLSERVERNAME) Then
                        sTmpString = msSQLSERVERNAME
                        Exit For
                    End If
                    If tmpsvc.Name.StartsWith("MSSQL$") Then
                        sTmpString = tmpsvc.Name.Substring(6)
                    End If
                Next
                If sTmpString <> String.Empty Then
                    msSQLSERVERNAME = sTmpString
                End If

                .DataSource = "(local)\" & msSQLSERVERNAME
                oServer.ConnectionContext.ConnectionString = .ConnectionString

            End With

            Call WriteMaintLogEntry(gpsRM.GetString("psBACKUP_MSG", oCulture) & DBName)

            'Define a BackupDeviceItem by supplying the backup device file name in the constructor, and the type of device is a file.
            oBdi = New BackupDeviceItem(sBackupPath & "\" & sBackupFileName, DeviceType.File)

            'Define the backup expiration date.
            dtExpire = DateAdd(DateInterval.Year, 1, Now)

            'Define the Backup object variable. 
            Try
                With oBackup
                    'Specify the type of backup, the description, the name, and the database to be backed up.
                    .Action = BackupActionType.Database
                    .BackupSetDescription = "Full backup of " & sDBName
                    .BackupSetName = sDBName & " Backup"
                    .Database = sDBName
                    'Add the backup device to the Backup object.
                    .Devices.Add(oBdi)
                    'Set the Incremental property to False to specify that this is a full database backup.
                    .Incremental = False
                    'Set the backup expiration date
                    .ExpirationDate = dtExpire
                    'Specify that the log must be truncated after the backup is complete.
                    .LogTruncation = BackupTruncateLogType.Truncate
                    'Run SqlBackup to perform the full database backup on the instance of SQL Server.
                    .SqlBackup(oServer)
                    'Remove the backup device from the Backup object.
                    .Devices.Remove(oBdi)
                End With
            Catch ex As Exception
                mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subBackupDB", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                Call subPostStatusMsg(gpsRM.GetString("psERROR", oCulture) & ex.Message)
                Call WriteMaintLogEntry(gpsRM.GetString("psERROR", oCulture) & ex.Message)
            End Try
            'MSW 5/6/11 Detach sql DBs so the files can be copied
            oServer.KillAllProcesses(sDBName)
            oServer.DetachDatabase(sDBName, True)
            Application.DoEvents()
            oServer = Nothing

        Catch ex As Exception
            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subBackupDB", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Call subPostStatusMsg(gpsRM.GetString("psERROR", oCulture) & ex.Message)
            Call WriteMaintLogEntry(gpsRM.GetString("psERROR", oCulture) & ex.Message)

        End Try

    End Sub

    Private Sub subCleanUp()
        '********************************************************************************************
        'Description:   Sequences database cleanup and compression operations
        '
        'Parameters:  None
        '
        'Returns:    None
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim oCulture As CultureInfo = frmStart.DisplayCulture

        Try
            Dim bAccessDBExists As Boolean

            Call subPostStatusMsg(gpsRM.GetString("psCLEANUP_MSG", oCulture) & " ...")
            Me.Status(True) = gpsRM.GetString("psCLEANUP_MSG", oCulture)
            Application.DoEvents()

            'delete the Backup Directory if it exists
            If Directory.Exists(msDatabasePath & "Backup") Then
                Directory.Delete(msDatabasePath & "Backup", True)
            End If
            Application.DoEvents()

            'create the Backup Directory
            Directory.CreateDirectory(msDatabasePath & "Backup")
            Application.DoEvents()

            'backup all SQL Server Databases prior to purging old records
            Dim sSqlDbFiles() As String = IO.Directory.GetFiles(msDatabasePath, "*.mdf")

            For Each sFile As String In sSqlDbFiles
                Dim sParts() As String = Strings.Split(sFile, "\")
                Application.DoEvents()

                Call subBackupDB(sParts(sParts.GetUpperBound(0)))
            Next 'sFile

            Application.DoEvents()



            'backup all MS Access Databases prior to purging old records
            Dim sMSAccessDbFiles() As String = IO.Directory.GetFiles(msDatabasePath, "*.mdb")

            For Each sFile As String In sMSAccessDbFiles
                Dim sParts() As String = Strings.Split(sFile, "\")
                Application.DoEvents()

                File.Copy(sFile, msDatabasePath & "Backup\" & sParts(sParts.GetUpperBound(0)), True)
            Next 'sFile

            With moMaint
                'SQL Server Log Databases
                Call subCleanupDB(gsALARM_DATABASENAME, .AlarmDaysToKeep)
                Me.Progress = 2
                Application.DoEvents()

                Call subCleanupDB(gsCHANGE_DATABASENAME, .ChangeDaysToKeep)
                Me.Progress = 4
                Application.DoEvents()

                Call subCleanupDB(gsPRODLOG_DBNAME, .ProdDaysToKeep)
                Me.Progress = 6
                Application.DoEvents()

                If mcolZones.ActiveZone.VisionEnabled Then
                    Call subCleanupDB(gsVISN_DATABASENAME, .VisionDaysToKeep)
                    Me.Progress = 8
                    Application.DoEvents()

                End If

                'Legacy MS Access Log Databases
                If File.Exists(msDatabasePath & "Alarms.mdb") Then
                    bAccessDBExists = True
                    Call subCleanupDB("Alarms.mdb", "Alarmlog", .AlarmDaysToKeep)
                    Me.Progress = 10
                End If

                If File.Exists(msDatabasePath & "Change.mdb") Then
                    bAccessDBExists = True
                    Call subCleanupDB("Change.mdb", "Data Change Log", .ChangeDaysToKeep)
                    Me.Progress = 12
                End If

                If File.Exists(msDatabasePath & "Prodlog.mdb") Then
                    bAccessDBExists = True
                    Call subCleanupDB("Prodlog.mdb", "Prodlog", .ProdDaysToKeep)
                    Me.Progress = 14
                End If

                If mcolZones.ActiveZone.VisionEnabled Then
                    If File.Exists(msDatabasePath & "Visnlog.mdb") Then
                        bAccessDBExists = True
                        Call subCleanupDB("Visnlog.mdb", "Visnlog", .VisionDaysToKeep)
                        Me.Progress = 16
                    End If
                End If
            End With 'moMaint

            If File.Exists(msDatabasePath & "Notepad.mdb") Then
                bAccessDBExists = True
            End If

            If bAccessDBExists Then
                'Only do this for MS Access DBs
                Call subPostStatusMsg(gpsRM.GetString("psCOMPACT_MSG", oCulture) & " ...")
                If File.Exists(msDatabasePath & "Alarms.mdb") Then Call subCompactDB("Alarms")
                Me.Progress = 18
                If File.Exists(msDatabasePath & "Change.mdb") Then Call subCompactDB("Change")
                Me.Progress = 20
                If File.Exists(msDatabasePath & "Prodlog.mdb") Then Call subCompactDB("Prodlog")
                Me.Progress = 22
                If File.Exists(msDatabasePath & "Notepad.mdb") Then Call subCompactDB("Notepad")
                Me.Progress = 24

                If mcolZones.ActiveZone.VisionEnabled Then
                    If File.Exists(msDatabasePath & "Visnlog.mdb") Then Call subCompactDB("Visnlog")
                    Me.Progress = 26
                End If

                Call subPostStatusMsg(gpsRM.GetString("psDELTEMP_MSG", oCulture) & " ...")
                Call subDeleteTempFiles()
            Else
                Me.Progress = 26
            End If
            'Clean up orphaned IPC messages
            Dim sIPCPath As String = String.Empty
            If mPWCommon.GetDefaultFilePath(sIPCPath, eDir.IPC, String.Empty, String.Empty) Then
                Dim sIPCFiles() As String = IO.Directory.GetFiles(sIPCPath, "*.*")
                For Each sIPCFile As String In sIPCFiles
                    Try
                        IO.File.Delete(sIPCFile)
                    Catch ex As Exception
                    End Try
                Next
            End If
            Me.Status(True) = String.Empty

        Catch ex As Exception
            Call subPostStatusMsg(gpsRM.GetString("psERROR", oCulture) & " - " & ex.Message)
            Call WriteMaintLogEntry(gpsRM.GetString("psERROR", oCulture) & ex.Message)
        End Try

    End Sub

    Private Sub subCleanupDB(ByVal DBName As String, ByVal TableName As String, _
                             ByVal DaysToKeep As Integer)
        '********************************************************************************************
        'Description:   This routine purges MS Access log database records that are older than the 
        '               "DaysToKeep" value.
        '
        'Parameters:  Database Name, Table Name, Number of Days to Keep
        '
        'Returns:    None
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Try
            Dim oDB As New clsDBAccess
            Dim dtCutoffDate As Date = DateAdd(DateInterval.Day, -DaysToKeep, Now)
            Dim nRecords As Integer
            Application.DoEvents()

            'log this operation
            Call WriteMaintLogEntry(gpsRM.GetString("psDEL_RECS_STATUS", frmStart.DisplayCulture) & _
                                    gpsRM.GetString("psDEL_AGE_STATUS", frmStart.DisplayCulture) & _
                                    DaysToKeep.ToString & _
                                    gpsRM.GetString("psDEL_DAYS_STATUS", frmStart.DisplayCulture) & _
                                    DBName)
            Application.DoEvents()

            With oDB
                .DBFileName = DBName
                .DBFilePath = msDatabasePath
                .DBTableName = TableName
                nRecords = .DeleteOldRecords(dtCutoffDate)
                .Close()
            End With 'oDB
            Application.DoEvents()

        Catch ex As Exception
            Call subPostStatusMsg(gpsRM.GetString("psERROR", frmStart.DisplayCulture) & DBName & " - " & ex.Message)
            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subCleanupDB", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Application.DoEvents()
        End Try

    End Sub

    Private Sub subCleanupDB(ByVal DBName As String, ByVal DaysToKeep As Integer)
        '********************************************************************************************
        'Description:   This routine purges SQL Server log database records that are older than the 
        '               "DaysToKeep" value. 
        '
        'Parameters:  Database Name, Number of Days to Keep
        '
        'Returns:    None
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Try
            Dim DB As clsSQLAccess = New clsSQLAccess
            Dim dtCutoffDate As Date = DateAdd(DateInterval.Day, -DaysToKeep, Now)
            Application.DoEvents()

            'log this operation
            Call WriteMaintLogEntry(gpsRM.GetString("psDEL_RECS_STATUS", frmStart.DisplayCulture) & _
                                    gpsRM.GetString("psDEL_AGE_STATUS", frmStart.DisplayCulture) & _
                                    DaysToKeep.ToString & _
                                    gpsRM.GetString("psDEL_DAYS_STATUS", frmStart.DisplayCulture) & _
                                    DBName)
            Application.DoEvents()

            With DB
                .DBFileName = DBName
                .Zone = mcolZones.ActiveZone

                If Not .CompactDatabase(dtCutoffDate) Then
                    'log failed message
                    Call WriteMaintLogEntry(gpsRM.GetString("psERROR", frmStart.DisplayCulture) & DBName & " - " & _
                                            gpsRM.GetString("psCLEANUP_DB_ERR", frmStart.DisplayCulture))
                    'post failed message to Status box
                    Call subPostStatusMsg(gpsRM.GetString("psERROR", frmStart.DisplayCulture) & DBName & " - " & _
                                          gpsRM.GetString("psCLEANUP_DB_ERR", frmStart.DisplayCulture))
                End If
                Application.DoEvents()

                .Close()
            End With 'DB

        Catch ex As Exception
            Call subPostStatusMsg(gpsRM.GetString("psERROR", frmStart.DisplayCulture) & DBName & " - " & ex.Message)
            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subCleanupDB", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Application.DoEvents()
        End Try

    End Sub

    Private Sub subCompactDB(ByVal DBName As String)
        '********************************************************************************************
        'Description: Compacts the (DBName) MS Access database after creating a backup copy with .bak
        '             extension
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim oCulture As CultureInfo = frmStart.DisplayCulture
        Dim oDB As New clsDBAccess
        Dim sDbPath As String = msDatabasePath & DBName
        Dim sDbBackupPath As String = msDatabasePath & "Backup\" & DBName

        Try

            Me.Status(True) = gpsRM.GetString("psCOMPACT_STATUS", oCulture) & _
                              msDatabasePath & DBName & ".mdb"

            'if a backup copy of the database file exists, delete it
            File.Delete(sDbBackupPath & ".bak")

            'create a backup copy of the current database file
            Call WriteMaintLogEntry(gpsRM.GetString("psCOPY_STATUS", oCulture) & _
                                    sDbPath & ".mdb" & gpsRM.GetString("psDEST_MSG", oCulture) & _
                                    sDbBackupPath & ".bak")
            File.Copy(sDbPath & ".mdb", sDbBackupPath & ".bak")

            'Compact and replace the currrent database
            Call WriteMaintLogEntry(gpsRM.GetString("psCOMPACT_STATUS", oCulture) & _
                                    sDbPath & ".mdb" & _
                                    gpsRM.GetString("psDEST_MSG", oCulture) & _
                                    sDbPath & "REP.mdb")
            With oDB
                .DBFileName = DBName & ".mdb"
                .DBFilePath = msDatabasePath
                If .RepairDB() Then
                    'compact operation was a success, overwrite the old file with the new file
                    Call WriteMaintLogEntry(gpsRM.GetString("psCOPY_STATUS", oCulture) & _
                                            sDbPath & "REP.mdb" & gpsRM.GetString("psDEST_MSG", oCulture) & _
                                            sDbPath & ".mdb")
                    File.Copy(sDbPath & "REP.mdb", sDbPath & ".mdb", True)
                    'delete the new file
                    Call WriteMaintLogEntry(gpsRM.GetString("psDELETE_STATUS", oCulture) & _
                            sDbPath & "REP.mdb")
                    File.Delete(sDbPath & "REP.mdb")
                Else
                    Call subPostStatusMsg(gpsRM.GetString("psCOMPACT_ERR", oCulture) & " - " & sDbPath & ".mdb")
                    Call WriteMaintLogEntry(gpsRM.GetString("psCOMPACT_ERR", oCulture) & " - " & sDbPath & ".mdb")
                End If
            End With

        Catch ex As Exception
            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subCompactDB", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Call subPostStatusMsg(gpsRM.GetString("psERROR", oCulture) & ex.Message)
            Call WriteMaintLogEntry(gpsRM.GetString("psERROR", oCulture) & ex.Message)

        End Try

    End Sub

    Private Sub subDeleteTempFiles()
        '********************************************************************************************
        'Description: Prevents hard drive from filling up with temp files.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim dtCutOff As Date = DateAdd(DateInterval.Day, -7, Now)
        Dim sTempPath As String = Environment.GetEnvironmentVariable("temp")
        Dim sTempDir() As String = System.IO.Directory.GetFiles(sTempPath, "*.tmp")

        On Error Resume Next

        For Each sFile As String In sTempDir
            If System.IO.File.GetLastWriteTime(sFile) < dtCutOff Then
                Trace.WriteLine(sFile)
                System.IO.File.Delete(sFile)
            End If
        Next 'sFile

        On Error GoTo 0

    End Sub

    Private Sub subDoMaintenance()
        '********************************************************************************************
        'Description: Based on the value of the command argument, perform the requested maintenance
        '             functions.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' RJO         02/02/10      Added waiting for auto backup to complete status post.
        ' MSW         05/06/11      Stop and start sql server for file copies
        ' MSW         11/08/11      Adjust the server stopping logic to manage multiple instances.
        ' RJO         04/01/13      Added code to read the auto backup timeout value from the PLC.
        '********************************************************************************************
        Dim nFlags As Integer = frmStart.Flags
        Dim oCulture As CultureInfo = frmStart.DisplayCulture

        If (nFlags And eFlags.RobotBackup) > 0 Then
            'set the auto backup requests, this takes about awhile to complete so we
            'will cleanup and then do the backup
            mcolControllers = New clsControllers(mcolZones, True)
            moPLC = New clsPLCComm

            For Each oRC As clsController In mcolControllers
                If oRC.InternalBackupEnabled Then
                    Dim nTimeOut As Integer
                    Dim oZone As clsZone = oRC.Zone
                    Dim sData(0) As String
                    Dim sTag As String = "Z" & oZone.ZoneNumber.ToString & "AutoBackupTimeout"

                    'Read the auto backup timeout value from the PLC 'RJO 04/01/13
                    With moPLC
                        .ZoneName = oZone.Name
                        .TagName = sTag
                        sData = .PLCData
                    End With
                    Try
                        nTimeOut = CType(sData(0), Integer)
                        mnAutoBackupTimeout = nTimeOut \ 1000 'Convert to seconds
                    Catch ex As Exception

                    End Try
                    Exit For
                End If
            Next

            Call subSetBackupBits()
        End If

        Me.Progress = 5

        If (nFlags And eFlags.Cleanup) > 0 Then
            'Database Cleanup selected
            Call WriteMaintLogEntry(gpsRM.GetString("psCLEANUP_OP", oCulture) & _
                                    gpsRM.GetString("psSELECTED_OP", oCulture))
            Application.DoEvents()
            Call subCleanUp()
            Application.DoEvents()
            Call subUpdateMaintDate("Cleanup")
            Application.DoEvents()
            'turn off the screen saver (in case it is running)
            Call subWakeUpScreenSaver()
            Application.DoEvents()
            Call WriteMaintLogEntry(gpsRM.GetString("psCLEANUP_OP", oCulture) & _
                                    gpsRM.GetString("psMAINT_DONE", oCulture))
            Application.DoEvents()
        Else
            Call WriteMaintLogEntry(gpsRM.GetString("psCLEANUP_OP", oCulture) & _
                                    gpsRM.GetString("psNOT_SELECTED_OP", oCulture))
            Call subPostStatusMsg(gpsRM.GetString("psCLEANUP_OP", oCulture) & _
                                    gpsRM.GetString("psNOT_SELECTED_OP", oCulture))
            Application.DoEvents()
        End If '(nFlags And eFlags.Cleanup) > 0

        Me.Progress = 33

        If (nFlags And eFlags.RobotBackup) > 0 Then
            If mbInternalBackupReqd Then
                '02/02/10 RJO Let Operator know we're waiting for auto backup to complete
                Call subPostStatusMsg(gpsRM.GetString("psROB_BACKUP_OP", oCulture) & _
                                      gpsRM.GetString("psAUTO_BKP_WAIT", oCulture))
            End If
            Application.DoEvents()

            'loop till robots are done with internal backup to FRA:\
            Call subReadBackupBits()
            Call WriteMaintLogEntry(gpsRM.GetString("psROB_BACKUP_OP", oCulture) & _
                                    gpsRM.GetString("psSELECTED_OP", oCulture))
            Application.DoEvents()
            Call subRobotBackup()
            Call subUpdateMaintDate("Robot Backup")
            Call WriteMaintLogEntry(gpsRM.GetString("psROB_BACKUP_OP", oCulture) & _
                                    gpsRM.GetString("psMAINT_DONE", oCulture))
        Else
            Call WriteMaintLogEntry(gpsRM.GetString("psROB_BACKUP_OP", oCulture) & _
                                    gpsRM.GetString("psNOT_SELECTED_OP", oCulture))
            Call subPostStatusMsg(gpsRM.GetString("psROB_BACKUP_OP", oCulture) & _
                                  gpsRM.GetString("psNOT_SELECTED_OP", oCulture))
        End If
        Application.DoEvents()

        Me.Progress = 67

        If (nFlags And eFlags.FileBackup) > 0 Then
            ' MSW         05/06/11      Stop and start sql server for file copies
            ''get the user's backup option selections
            'Call subGetBackupOptions()
            'Application.DoEvents()

            ''get the additional folders configured for backup
            'Call subGetFolders()
            'Application.DoEvents()

            'Call subGetExtraCopies()
            'Application.DoEvents()

            Call subUpdateMaintDate("File Backup")
            Application.DoEvents()

            Dim mc As ManagedComputer
            mc = New ManagedComputer()
            mc.Refresh()
            Application.DoEvents()
            Debug.Print(mc.Name)
            Dim tmpsvc As Service
            Dim svc As Service = Nothing
            Try
                'Reference the Microsoft SQL Server express 2005 service.
                For Each tmpsvc In mc.Services
                    Debug.Print(tmpsvc.Name)
                    If InStr(tmpsvc.Name, msSQLSERVERNAME) > 0 Then
                        svc = tmpsvc
                        Exit For  'Take this one and go
                    ElseIf tmpsvc.Name.StartsWith("MSSQL$") Then
                        svc = tmpsvc 'remember this, but keep looking for the right name
                    End If
                    Application.DoEvents()
                Next

                If svc IsNot Nothing Then
                    svc.Stop()
                    Application.DoEvents()
                End If
            Catch ex As Exception

            End Try

            'PAINTworks files backup selected
            Call WriteMaintLogEntry(gpsRM.GetString("psFILE_BACKUP_OP", oCulture) & _
                                    gpsRM.GetString("psSELECTED_OP", oCulture))
            Application.DoEvents()
            Call subFileBackup()
            If svc IsNot Nothing Then
                svc.Start()
                Application.DoEvents()
            End If
            Application.DoEvents()
            'turn off the screen saver (in case it is running)
            Call subWakeUpScreenSaver()
            Application.DoEvents()

            Call WriteMaintLogEntry(gpsRM.GetString("psFILE_BACKUP_OP", oCulture) & _
                                    gpsRM.GetString("psMAINT_DONE", oCulture))
        Else
            Call WriteMaintLogEntry(gpsRM.GetString("psFILE_BACKUP_OP", oCulture) & _
                                    gpsRM.GetString("psNOT_SELECTED_OP", oCulture))
            Call subPostStatusMsg(gpsRM.GetString("psFILE_BACKUP_OP", oCulture) & _
                                  gpsRM.GetString("psNOT_SELECTED_OP", oCulture))
        End If '(nFlags And eFlags.FileBackup) > 0

        Me.Progress = 95

        If (nFlags And eFlags.ReportGen) > 0 Then
            'Auto Report Generator
            Call WriteMaintLogEntry(gpsRM.GetString("psREPORT_GENERATOR_OP", oCulture) & _
                                    gpsRM.GetString("psSELECTED_OP", oCulture))
            Call subAutoGenReports()
            Application.DoEvents()

            Do Until mbAutoReportDone = True
                Application.DoEvents()
            Loop

            Call subUpdateMaintDate("Report Generator")
            Call WriteMaintLogEntry(gpsRM.GetString("psREPORT_GENERATOR_OP", oCulture) & _
                                    gpsRM.GetString("psMAINT_DONE", oCulture))
            Application.DoEvents()
        End If

        Me.Progress = 100
        Me.Status(True) = gpsRM.GetString("psPW4_SCHED_MAINT", oCulture) & _
                          gpsRM.GetString("psMAINT_DONE", oCulture)
        Application.DoEvents()

        If InIDE = True Then
            btnRestartPW.Text = gpsRM.GetString("psCLOSE", oCulture)
        End If

        'Maintenance complete. Let user decide what to do
        btnPrintLog.Visible = True
        btnViewLog.Visible = True
        btnRestartPW.Visible = True

        If Not InIDE Then
            'start auto shutdown delay timer
            mnElapsedTime = 0
            tmrClose.Enabled = True
        End If

    End Sub

    Private Sub subDoStatusBar(ByVal Controller As clsController)
        '********************************************************************************************
        'Description: do the icons on the status bar - set up for up to 10 robots
        '
        'Parameters: Controller that raised the event
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTipText As String = String.Empty
        Dim sImgKey As String = String.Empty

        'find the label we want
        Dim sName As String = "lbl" & Controller.Name

        Dim l As ToolStripStatusLabel = DirectCast(stsStatus.Items(sName), ToolStripStatusLabel)
        If l Is Nothing Then Exit Sub

        Select Case (Controller.RCMConnectStatus)
            Case ConnStat.frRNConnecting
                sImgKey = "imgSBRYellow"
                sTipText = gcsRM.GetString("csCONNECTING")
            Case ConnStat.frRNDisconnecting
                sImgKey = "imgSBRYellow"
                sTipText = gcsRM.GetString("csDISCONNECTING")
            Case ConnStat.frRNAvailable
                sImgKey = "imgSBRBlue"
                sTipText = gcsRM.GetString("csAVAILABLE")
            Case ConnStat.frRNConnected
                sImgKey = "imgSBRGreen"
                sTipText = gcsRM.GetString("csCONNECTED")
            Case ConnStat.frRNUnavailable
                sImgKey = "imgSBRRed"
                sTipText = gcsRM.GetString("csUNAVAILABLE")
            Case ConnStat.frRNUnknown
                sImgKey = "imgSBRGrey"
                sTipText = gcsRM.GetString("csUNKNOWN")
            Case ConnStat.frRNHeartbeatLost
                sImgKey = "imgSBRRed"
                sTipText = gcsRM.GetString("csHBLOST")
        End Select

        l.ToolTipText = Controller.Name & " " & _
                            gcsRM.GetString("csCONNECTION_STAT") & " " & sTipText

        Try
            l.Image = DirectCast(gcsRM.GetObject(sImgKey), Image)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

        'TODO - Check if the next line is required
        'DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)

    End Sub

    Private Sub subFileBackup()
        '********************************************************************************************
        'Description:   Sequences file backup operations
        '
        'Parameters:  None
        '
        'Returns:    None
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' RJO         02/02/10      Added exception detail to Status and Maint Log posts when an 
        '                           error occurs.
        ' MSW         05/14/10      Added extra try/catch blocks so errors don't have to abort the whole thing.
        '                           Split up some error messages into two lines.
        ' MSW         11/01/10      Add overwrite=true to each FileIO.FileSystem.CopyDirectory call.  It's needed
        '                           When Maint doesn't make a new directory for each backup.
        ' MSW         03/23/11      For individual folder optnion, add a "latest" folder and backup to both so the
        '                           latest backup can always be found in the same subfolder
        ' MSW         11/29/12      subFileBackup - Change DB copy to maintain file structure in backups, 
        '                           also copy *.mdf since we're stopping the server now.
        ' MSW         02/17/12      Add PaintWorksBackups separate from RobotBackups
        ' MSW         11/21/13      Add error message details to event log
        '********************************************************************************************
        Dim oCulture As CultureInfo = frmStart.DisplayCulture
        Dim sBackupFolder As String = String.Empty
        Dim sBackupFolderLatest As String = String.Empty
        Dim sErrorMsg(1) As String
        sErrorMsg(0) = String.Empty
        sErrorMsg(1) = String.Empty
        Dim sFileDirRoot As String = String.Empty
        Dim sSourceFolder As String = String.Empty
        Dim sPaintFolder As String = String.Empty
        'Make DB Folder
        Dim sDBLatestFolder As String = String.Empty
        Dim sDBFolder As String = String.Empty

        Try
            Me.Status(True) = gpsRM.GetString("psBACKUP_MSG", oCulture) & _
                              gpsRM.GetString("psPW_FILES", oCulture)
            Application.DoEvents()


            'Private moAdditionalFolders As AdditionalFolders = Nothing
            'Private moExtraCopies As ExtraCopies = Nothing
            'Private moBackupOptions As BackupOptions = Nothing
            moBackupOptions = New clsBackupOptions
            moAdditionalFolders = New clsAdditionalFolders
            moExtraCopies = New clsExtraCopies
            ''get the user's backup option selections
            'Call subGetBackupOptions()
            'Application.DoEvents()

            ''get the additional folders configured for backup
            'Call subGetFolders()
            'Application.DoEvents()

            'Call subGetExtraCopies()
            'Application.DoEvents()

            Call mPWCommon.GetDefaultFilePath(sSourceFolder, eDir.PAINTworks, String.Empty, String.Empty)
            sPaintFolder = sSourceFolder
            sBackupFolder = Strings.Trim(moBackupOptions.Path)
            sBackupFolder = sRemoveSlash(sBackupFolder)

            sFileDirRoot = sBackupFolder & "\"
            Application.DoEvents()

            'check available space on destination drive. exit if there isn't enough free space.
            If moMaint.DriveFull(Strings.Left(sBackupFolder, 1)) Then
                Call subPostStatusMsg(gpsRM.GetString("psERROR", oCulture) & _
                                      gpsRM.GetString("psDISK_LOW_SPACE_ERR", oCulture))
                Call WriteMaintLogEntry(gpsRM.GetString("psERROR", oCulture) & _
                                        gpsRM.GetString("psDISK_LOW_SPACE_ERR", oCulture))
                Exit Sub
            End If
            Application.DoEvents()

            sErrorMsg(0) = gpsRM.GetString("psCREATE_FOLDER_ERR", oCulture)
            If moBackupOptions.NewFolder Then
                Application.DoEvents()
                sBackupFolder = sFileDirRoot & sNewFolder()
                sErrorMsg(0) = sErrorMsg(0) & sBackupFolder
                Call WriteMaintLogEntry(gpsRM.GetString("psFILE_BACKUP_FOLDER_MSG", oCulture) & sBackupFolder)
                'Attempt to create a folder with a time stamp folder name for this backup
                Application.DoEvents()
                Directory.CreateDirectory(sBackupFolder)
                sBackupFolderLatest = sFileDirRoot & gpsRM.GetString("psLATEST", oCulture)
                Try
                    Application.DoEvents()
                    Directory.Delete(sBackupFolderLatest, True)
                Catch ex As Exception
                End Try
                Application.DoEvents()

                Directory.CreateDirectory(sBackupFolderLatest)
            Else
                'make sure backup folder exists
                sErrorMsg(0) = sErrorMsg(0) & sBackupFolder
                Directory.CreateDirectory(sBackupFolder)
            End If

            'delete any old backup folders that are in excess of the amount we want to keep
            Dim sOldestFolder As String = String.Empty

            sErrorMsg(0) = String.Empty
            sErrorMsg(1) = String.Empty
            Do While moMaint.PaintWorksBackupsToKeep < nRobotBackupCount(sOldestFolder, sFileDirRoot)
                Call subPostStatusMsg(gpsRM.GetString("psDELBACKUP_MSG", oCulture) & sOldestFolder & " ...")
                Call WriteMaintLogEntry(gpsRM.GetString("psDELBACKUP_MSG", oCulture) & sOldestFolder)
                Me.Status(True) = gpsRM.GetString("psDELFILES_STATUS", oCulture)
                Application.DoEvents()

                Directory.Delete(sOldestFolder, True)
            Loop

            Me.Status(True) = gpsRM.GetString("psBACKUP_MSG", oCulture)

            With moBackupOptions

                Try
                    If .Database Then
                        Application.DoEvents()
                        'Make DB Folder
                        Call mPWCommon.GetDefaultFilePath(sSourceFolder, eDir.Database, String.Empty, String.Empty)
                        Dim sDBSuffix As String = sSourceFolder.Substring(sPaintFolder.Length - 1)
                        sDBFolder = sBackupFolder & sDBSuffix
                        sDBLatestFolder = sBackupFolderLatest & sDBSuffix
                        sSourceFolder = sRemoveSlash(sSourceFolder)

                        Call subPostStatusMsg(gpsRM.GetString("psBACKUP_MSG", oCulture) & sSourceFolder & _
                                              gpsRM.GetString("psDEST_MSG", oCulture) & _
                                              " " & sBackupFolder & " ...")
                        Call WriteMaintLogEntry(gpsRM.GetString("psBACKUP_MSG", oCulture) & sSourceFolder & _
                                                gpsRM.GetString("psDEST_MSG", oCulture) & " " & sBackupFolder)

                        sErrorMsg(0) = gpsRM.GetString("psCOPY_FOLDER_ERR", oCulture) & sSourceFolder & "\Backup"
                        Application.DoEvents()
                        Try
                            sErrorMsg(1) = "  " & gpsRM.GetString("psDEST_MSG", oCulture) & sDBFolder
                            FileIO.FileSystem.CopyDirectory(sSourceFolder, sDBFolder, True)
                        Catch ex As Exception
                            Call subPostStatusMsg(sErrorMsg(0) & " - " & ex.Message)
                            Call WriteMaintLogEntry(sErrorMsg(0) & " - " & ex.Message)
                            Call subPostStatusMsg(sErrorMsg(1) & " - " & ex.Message)
                            Call WriteMaintLogEntry(sErrorMsg(1) & " - " & ex.Message)
                            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                       sErrorMsg(0) & " - " & sErrorMsg(1) & " - " & "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                        End Try
                        If moBackupOptions.NewFolder Then
                            Try
                                sErrorMsg(1) = "  " & gpsRM.GetString("psDEST_MSG", oCulture) & sDBLatestFolder
                                FileIO.FileSystem.CopyDirectory(sSourceFolder, sDBLatestFolder, True)
                            Catch ex As Exception
                                Call subPostStatusMsg(sErrorMsg(0) & " - " & ex.Message)
                                Call WriteMaintLogEntry(sErrorMsg(0) & " - " & ex.Message)
                                Call subPostStatusMsg(sErrorMsg(1) & " - " & ex.Message)
                                Call WriteMaintLogEntry(sErrorMsg(1) & " - " & ex.Message)
                                mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                           sErrorMsg(0) & " - " & sErrorMsg(1) & " - " & "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                            Application.DoEvents()
                        End If
                        sErrorMsg(0) = gpsRM.GetString("psCOPY_FOLDER_ERR", oCulture) & sSourceFolder & "\Backup"
                        Try
                            sErrorMsg(1) = "  " & gpsRM.GetString("psDEST_MSG", oCulture) & sDBLatestFolder & "\Backup"
                            FileIO.FileSystem.CopyDirectory(sSourceFolder & "\Backup", sDBFolder & "\Backup", True)
                        Catch ex As Exception
                            Call subPostStatusMsg(sErrorMsg(0) & " - " & ex.Message)
                            Call WriteMaintLogEntry(sErrorMsg(0) & " - " & ex.Message)
                            Call subPostStatusMsg(sErrorMsg(1) & " - " & ex.Message)
                            Call WriteMaintLogEntry(sErrorMsg(1) & " - " & ex.Message)
                            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                       sErrorMsg(0) & " - " & sErrorMsg(1) & " - " & "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                        End Try
                        Application.DoEvents()
                        If moBackupOptions.NewFolder Then
                            Try
                                sErrorMsg(1) = "  " & gpsRM.GetString("psDEST_MSG", oCulture) & sDBLatestFolder & "\Backup"
                                FileIO.FileSystem.CopyDirectory(sSourceFolder & "\Backup", sDBLatestFolder & "\Backup", True)
                            Catch ex As Exception
                                Call subPostStatusMsg(sErrorMsg(0) & " - " & ex.Message)
                                Call WriteMaintLogEntry(sErrorMsg(0) & " - " & ex.Message)
                                Call subPostStatusMsg(sErrorMsg(1) & " - " & ex.Message)
                                Call WriteMaintLogEntry(sErrorMsg(1) & " - " & ex.Message)
                                mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                           sErrorMsg(0) & " - " & sErrorMsg(1) & " - " & "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                        End If

                        Application.DoEvents()

                        'XML
                        Try
                            Call mPWCommon.GetDefaultFilePath(sSourceFolder, eDir.XML, String.Empty, String.Empty)
                            sErrorMsg(0) = gpsRM.GetString("psCOPY_FOLDER_ERR", oCulture) & sSourceFolder
                            sErrorMsg(1) = "  " & gpsRM.GetString("psDEST_MSG", oCulture) & sDBFolder & "\Xml"
                            sSourceFolder = sRemoveSlash(sSourceFolder)

                            FileIO.FileSystem.CopyDirectory(sSourceFolder, sDBFolder & "\Xml", True)
                            Application.DoEvents()
                        Catch ex As Exception
                            Call subPostStatusMsg(sErrorMsg(0) & " - " & ex.Message)
                            Call WriteMaintLogEntry(sErrorMsg(0) & " - " & ex.Message)
                            Call subPostStatusMsg(sErrorMsg(1) & " - " & ex.Message)
                            Call WriteMaintLogEntry(sErrorMsg(1) & " - " & ex.Message)
                            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                       sErrorMsg(0) & " - " & sErrorMsg(1) & " - " & "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                        End Try
                        Try
                            If moBackupOptions.NewFolder Then
                                sErrorMsg(1) = "  " & gpsRM.GetString("psDEST_MSG", oCulture) & sDBLatestFolder & "\Xml"
                                FileIO.FileSystem.CopyDirectory(sSourceFolder, sDBLatestFolder & "\Xml", True)
                                Application.DoEvents()
                            End If
                        Catch ex As Exception
                            Call subPostStatusMsg(sErrorMsg(0) & " - " & ex.Message)
                            Call WriteMaintLogEntry(sErrorMsg(0) & " - " & ex.Message)
                            Call subPostStatusMsg(sErrorMsg(1) & " - " & ex.Message)
                            Call WriteMaintLogEntry(sErrorMsg(1) & " - " & ex.Message)
                            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                       sErrorMsg(0) & " - " & sErrorMsg(1) & " - " & "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                        End Try
                        'Notepad
                        Try
                            Call mPWCommon.GetDefaultFilePath(sSourceFolder, eDir.Notepad, String.Empty, String.Empty)
                            sErrorMsg(0) = gpsRM.GetString("psCOPY_FOLDER_ERR", oCulture) & sSourceFolder
                            sErrorMsg(1) = "  " & gpsRM.GetString("psDEST_MSG", oCulture) & sDBFolder & "\Notepad"
                            sSourceFolder = sRemoveSlash(sSourceFolder)

                            FileIO.FileSystem.CopyDirectory(sSourceFolder, sDBFolder & "\Notepad", True)
                            Application.DoEvents()
                        Catch ex As Exception
                            Call subPostStatusMsg(sErrorMsg(0) & " - " & ex.Message)
                            Call WriteMaintLogEntry(sErrorMsg(0) & " - " & ex.Message)
                            Call subPostStatusMsg(sErrorMsg(1) & " - " & ex.Message)
                            Call WriteMaintLogEntry(sErrorMsg(1) & " - " & ex.Message)
                            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                       sErrorMsg(0) & " - " & sErrorMsg(1) & " - " & "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                        End Try
                        Try
                            If moBackupOptions.NewFolder Then
                                sErrorMsg(1) = "  " & gpsRM.GetString("psDEST_MSG", oCulture) & sDBLatestFolder & "\Notepad"
                                FileIO.FileSystem.CopyDirectory(sSourceFolder, sDBLatestFolder & "\Notepad", True)
                                Application.DoEvents()
                            End If
                        Catch ex As Exception
                            Call subPostStatusMsg(sErrorMsg(0) & " - " & ex.Message)
                            Call WriteMaintLogEntry(sErrorMsg(0) & " - " & ex.Message)
                            Call subPostStatusMsg(sErrorMsg(1) & " - " & ex.Message)
                            Call WriteMaintLogEntry(sErrorMsg(1) & " - " & ex.Message)
                            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                       sErrorMsg(0) & " - " & sErrorMsg(1) & " - " & "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                        End Try

                        'ECBR
                        Try

                            Call mPWCommon.GetDefaultFilePath(sSourceFolder, eDir.ECBR, String.Empty, String.Empty)
                            sErrorMsg(0) = gpsRM.GetString("psCOPY_FOLDER_ERR", oCulture) & sSourceFolder
                            sErrorMsg(1) = "  " & gpsRM.GetString("psDEST_MSG", oCulture) & sBackupFolder & "\ECBR"
                            sSourceFolder = sRemoveSlash(sSourceFolder)

                            Application.DoEvents()
                            FileIO.FileSystem.CopyDirectory(sSourceFolder, sDBFolder & "\ECBR", True)
                        Catch ex As Exception
                            Call subPostStatusMsg(sErrorMsg(0) & " - " & ex.Message)
                            Call WriteMaintLogEntry(sErrorMsg(0) & " - " & ex.Message)
                            Call subPostStatusMsg(sErrorMsg(1) & " - " & ex.Message)
                            Call WriteMaintLogEntry(sErrorMsg(1) & " - " & ex.Message)
                            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                       sErrorMsg(0) & " - " & sErrorMsg(1) & " - " & "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                        End Try
                        Try
                            Application.DoEvents()
                            If moBackupOptions.NewFolder Then
                                sErrorMsg(1) = "  " & gpsRM.GetString("psDEST_MSG", oCulture) & sDBLatestFolder & "\ECBR"
                                FileIO.FileSystem.CopyDirectory(sSourceFolder, sDBLatestFolder & "\ECBR", True)
                                Application.DoEvents()
                            End If
                        Catch ex As Exception
                            Call subPostStatusMsg(sErrorMsg(0) & " - " & ex.Message)
                            Call WriteMaintLogEntry(sErrorMsg(0) & " - " & ex.Message)
                            Call subPostStatusMsg(sErrorMsg(1) & " - " & ex.Message)
                            Call WriteMaintLogEntry(sErrorMsg(1) & " - " & ex.Message)
                            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                       sErrorMsg(0) & " - " & sErrorMsg(1) & " - " & "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                        End Try
                    End If '.Database

                    Me.Progress = 75

                Catch ex As Exception
                    If sErrorMsg(0) = String.Empty Then
                        Call subPostStatusMsg(gpsRM.GetString("psERROR", oCulture) & ex.Message)
                        Call WriteMaintLogEntry(gpsRM.GetString("psERROR", oCulture) & ex.Message)
                        mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    Else
                        '02/02/10 RJO Added exception detail to Status and Maint Log posts.
                        Call subPostStatusMsg(sErrorMsg(0) & " - " & ex.Message)
                        Call WriteMaintLogEntry(sErrorMsg(0) & " - " & ex.Message)
                        Call subPostStatusMsg(sErrorMsg(1) & " - " & ex.Message)
                        Call WriteMaintLogEntry(sErrorMsg(1) & " - " & ex.Message)
                        mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                   sErrorMsg(0) & " - " & sErrorMsg(1) & " - " & "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    End If

                    Application.DoEvents()

                End Try
                Try
                    sErrorMsg(0) = String.Empty
                    sErrorMsg(1) = String.Empty
                    If .RobotMaster Then
                        Application.DoEvents()
                        Call mPWCommon.GetDefaultFilePath(sSourceFolder, eDir.MasterBackups, String.Empty, String.Empty)
                        sSourceFolder = sRemoveSlash(sSourceFolder)

                        sErrorMsg(0) = gpsRM.GetString("psCOPY_FOLDER_ERR", oCulture) & sSourceFolder
                        sErrorMsg(1) = "  " & gpsRM.GetString("psDEST_MSG", oCulture) & sBackupFolder
                        Call subPostStatusMsg(gpsRM.GetString("psBACKUP_MSG", oCulture) & sSourceFolder & _
                                              gpsRM.GetString("psDEST_MSG", oCulture) & _
                                              " " & sBackupFolder & " ...")
                        Call WriteMaintLogEntry(gpsRM.GetString("psBACKUP_MSG", oCulture) & sSourceFolder & _
                                                gpsRM.GetString("psDEST_MSG", oCulture) & " " & sBackupFolder)

                        Application.DoEvents()
                        FileIO.FileSystem.CopyDirectory(sSourceFolder, sBackupFolder & "\Master Backups", True)
                        Application.DoEvents()
                        If moBackupOptions.NewFolder Then
                            FileIO.FileSystem.CopyDirectory(sSourceFolder, sBackupFolderLatest & "\Master Backups", True)
                            Application.DoEvents()
                        End If
                    End If '.RobotMaster

                    Me.Progress = 80

                Catch ex As Exception

                    If sErrorMsg(0) = String.Empty Then
                        Call subPostStatusMsg(gpsRM.GetString("psERROR", oCulture) & ex.Message)
                        Call WriteMaintLogEntry(gpsRM.GetString("psERROR", oCulture) & ex.Message)
                        mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    Else
                        '02/02/10 RJO Added exception detail to Status and Maint Log posts.
                        Call subPostStatusMsg(sErrorMsg(0) & " - " & ex.Message)
                        Call WriteMaintLogEntry(sErrorMsg(0) & " - " & ex.Message)
                        Call subPostStatusMsg(sErrorMsg(1) & " - " & ex.Message)
                        Call WriteMaintLogEntry(sErrorMsg(1) & " - " & ex.Message)
                        mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                   sErrorMsg(0) & " - " & sErrorMsg(1) & " - " & "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    End If
                    Application.DoEvents()

                End Try
                Try
                    sErrorMsg(0) = String.Empty
                    sErrorMsg(1) = String.Empty
                    If .RobotScheduled Then
                        Application.DoEvents()

                        Call mPWCommon.GetDefaultFilePath(sSourceFolder, eDir.PWRobotBackups, String.Empty, String.Empty)
                        sSourceFolder = sRemoveSlash(sSourceFolder)

                        sErrorMsg(0) = gpsRM.GetString("psCOPY_FOLDER_ERR", oCulture) & sSourceFolder
                        sErrorMsg(1) = "  " & gpsRM.GetString("psDEST_MSG", oCulture) & sBackupFolder
                        Call subPostStatusMsg(gpsRM.GetString("psBACKUP_MSG", oCulture) & sSourceFolder & _
                                              gpsRM.GetString("psDEST_MSG", oCulture) & _
                                              " " & sBackupFolder & " ...")
                        Call WriteMaintLogEntry(gpsRM.GetString("psBACKUP_MSG", oCulture) & sSourceFolder & _
                                                gpsRM.GetString("psDEST_MSG", oCulture) & " " & sBackupFolder)

                        Application.DoEvents()
                        FileIO.FileSystem.CopyDirectory(sSourceFolder, sBackupFolder & "\PW Robot Backups", True)
                        Application.DoEvents()
                        If moBackupOptions.NewFolder Then
                            FileIO.FileSystem.CopyDirectory(sSourceFolder & "\" & gpsRM.GetString("psLATEST", oCulture), sBackupFolderLatest & "\PW Robot Backups", True)
                            Application.DoEvents()
                        End If
                    End If '.RobotScheduled

                    Me.Progress = 85

                Catch ex As Exception

                    If sErrorMsg(0) = String.Empty Then
                        Call subPostStatusMsg(gpsRM.GetString("psERROR", oCulture) & ex.Message)
                        Call WriteMaintLogEntry(gpsRM.GetString("psERROR", oCulture) & ex.Message)
                        mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    Else
                        '02/02/10 RJO Added exception detail to Status and Maint Log posts.
                        Call subPostStatusMsg(sErrorMsg(0) & " - " & ex.Message)
                        Call WriteMaintLogEntry(sErrorMsg(0) & " - " & ex.Message)
                        Call subPostStatusMsg(sErrorMsg(1) & " - " & ex.Message)
                        Call WriteMaintLogEntry(sErrorMsg(1) & " - " & ex.Message)
                        mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                          sErrorMsg(0) & " - " & sErrorMsg(1) & " - " & "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    End If

                    Application.DoEvents()

                End Try
                'BTK 03/22/11 Added code to delete old backups from PW Robot Backups.
                Try
                    sErrorMsg(0) = String.Empty
                    sErrorMsg(1) = String.Empty
                    Dim sBackupPath As String = String.Empty

                    Call mPWCommon.GetDefaultFilePath(sBackupPath, eDir.PWRobotBackups, String.Empty, String.Empty)
                    Do While moMaint.RobotBackupsToKeep < nRobotBackupCount(sOldestFolder, sBackupPath)
                        sErrorMsg(0) = gpsRM.GetString("psDEL_FOLDER_ERR", oCulture) & sOldestFolder
                        Call subPostStatusMsg(gpsRM.GetString("psDELBACKUP_MSG", oCulture) & sOldestFolder & " ...")
                        Call WriteMaintLogEntry(gpsRM.GetString("psDELBACKUP_MSG", oCulture) & sOldestFolder)
                        Me.Status(True) = gpsRM.GetString("psDELFILES_STATUS", oCulture)

                        Directory.Delete(sOldestFolder, True)
                        Application.DoEvents()

                    Loop

                Catch ex As Exception

                    If sErrorMsg(0) = String.Empty Then
                        Call subPostStatusMsg(gpsRM.GetString("psERROR", oCulture) & ex.Message)
                        Call WriteMaintLogEntry(gpsRM.GetString("psERROR", oCulture) & ex.Message)
                        mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    Else
                        '02/02/10 RJO Added exception detail to Status and Maint Log posts.
                        Call subPostStatusMsg(sErrorMsg(0) & " - " & ex.Message)
                        Call WriteMaintLogEntry(sErrorMsg(0) & " - " & ex.Message)
                        Call subPostStatusMsg(sErrorMsg(1) & " - " & ex.Message)
                        Call WriteMaintLogEntry(sErrorMsg(1) & " - " & ex.Message)
                        mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                          sErrorMsg(0) & " - " & sErrorMsg(1) & " - " & "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    End If
                End Try


                Try
                    sErrorMsg(0) = String.Empty
                    sErrorMsg(1) = String.Empty
                    If .RobotTemp Then
                        Application.DoEvents()
                        Call mPWCommon.GetDefaultFilePath(sSourceFolder, eDir.TempBackups, String.Empty, String.Empty)
                        sSourceFolder = sRemoveSlash(sSourceFolder)

                        sErrorMsg(0) = gpsRM.GetString("psCOPY_FOLDER_ERR", oCulture) & sSourceFolder
                        sErrorMsg(1) = "  " & gpsRM.GetString("psDEST_MSG", oCulture) & sBackupFolder
                        Call subPostStatusMsg(gpsRM.GetString("psBACKUP_MSG", oCulture) & sSourceFolder & _
                                              gpsRM.GetString("psDEST_MSG", oCulture) & _
                                              " " & sBackupFolder & " ...")
                        Call WriteMaintLogEntry(gpsRM.GetString("psBACKUP_MSG", oCulture) & sSourceFolder & _
                                                gpsRM.GetString("psDEST_MSG", oCulture) & " " & sBackupFolder)

                        Application.DoEvents()
                        FileIO.FileSystem.CopyDirectory(sSourceFolder, sBackupFolder & "\Temp Backups", True)
                        Application.DoEvents()
                        If moBackupOptions.NewFolder Then
                            FileIO.FileSystem.CopyDirectory(sSourceFolder, sBackupFolderLatest & "\Temp Backups", True)
                            Application.DoEvents()
                        End If

                    End If '.RobotTemp

                    Me.Progress = 90

                Catch ex As Exception

                    If sErrorMsg(0) = String.Empty Then
                        Call subPostStatusMsg(gpsRM.GetString("psERROR", oCulture) & ex.Message)
                        Call WriteMaintLogEntry(gpsRM.GetString("psERROR", oCulture) & ex.Message)
                        mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    Else
                        '02/02/10 RJO Added exception detail to Status and Maint Log posts.
                        Call subPostStatusMsg(sErrorMsg(0) & " - " & ex.Message)
                        Call WriteMaintLogEntry(sErrorMsg(0) & " - " & ex.Message)
                        Call subPostStatusMsg(sErrorMsg(1) & " - " & ex.Message)
                        Call WriteMaintLogEntry(sErrorMsg(1) & " - " & ex.Message)
                        mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                          sErrorMsg(0) & " - " & sErrorMsg(1) & " - " & "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    End If
                    Application.DoEvents()

                End Try
                Try
                    sErrorMsg(0) = String.Empty
                    sErrorMsg(1) = String.Empty
                    If .RobotImage Then
                        Application.DoEvents()
                        Call mPWCommon.GetDefaultFilePath(sSourceFolder, eDir.RobotImageBackups, String.Empty, String.Empty)
                        sSourceFolder = sRemoveSlash(sSourceFolder)

                        sErrorMsg(0) = gpsRM.GetString("psCOPY_FOLDER_ERR", oCulture) & sSourceFolder
                        sErrorMsg(1) = "  " & gpsRM.GetString("psDEST_MSG", oCulture) & sBackupFolder
                        Call subPostStatusMsg(gpsRM.GetString("psBACKUP_MSG", oCulture) & sSourceFolder & _
                                              gpsRM.GetString("psDEST_MSG", oCulture) & _
                                              " " & sBackupFolder & " ...")
                        Call WriteMaintLogEntry(gpsRM.GetString("psBACKUP_MSG", oCulture) & sSourceFolder & _
                                                gpsRM.GetString("psDEST_MSG", oCulture) & " " & sBackupFolder)

                        Application.DoEvents()
                        FileIO.FileSystem.CopyDirectory(sSourceFolder, sBackupFolder & "\Robot Image Backups", True)
                        Application.DoEvents()
                        If moBackupOptions.NewFolder Then
                            FileIO.FileSystem.CopyDirectory(sSourceFolder, sBackupFolderLatest & "\Robot Image Backups", True)
                            Application.DoEvents()
                        End If

                    End If '.RobotTemp

                    Me.Progress = 95

                Catch ex As Exception

                    If sErrorMsg(0) = String.Empty Then
                        Call subPostStatusMsg(gpsRM.GetString("psERROR", oCulture) & ex.Message)
                        Call WriteMaintLogEntry(gpsRM.GetString("psERROR", oCulture) & ex.Message)
                        mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    Else
                        '02/02/10 RJO Added exception detail to Status and Maint Log posts.
                        Call subPostStatusMsg(sErrorMsg(0) & " - " & ex.Message)
                        Call WriteMaintLogEntry(sErrorMsg(0) & " - " & ex.Message)
                        Call subPostStatusMsg(sErrorMsg(1) & " - " & ex.Message)
                        Call WriteMaintLogEntry(sErrorMsg(1) & " - " & ex.Message)
                        mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                          sErrorMsg(0) & " - " & sErrorMsg(1) & " - " & "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                    End If
                    Application.DoEvents()

                End Try

                If .Folders And (moAdditionalFolders.NumFolders > 0) Then
                    For Each oFolder As clsAdditionalFolders.udsFolders In moAdditionalFolders.Folders
                        Try
                            Application.DoEvents()

                            sErrorMsg(0) = String.Empty
                            sErrorMsg(1) = String.Empty
                            Dim sDestFolder As String = sBackupFolder & "\" & oFolder.FolderName

                            sSourceFolder = sRemoveSlash(oFolder.FolderPath)
                            Application.DoEvents()

                            sErrorMsg(0) = gpsRM.GetString("psCOPY_FOLDER_ERR", oCulture) & sSourceFolder
                            sErrorMsg(1) = "  " & gpsRM.GetString("psDEST_MSG", oCulture) & sBackupFolder
                            Call subPostStatusMsg(gpsRM.GetString("psBACKUP_MSG", oCulture) & sSourceFolder & _
                                                  gpsRM.GetString("psDEST_MSG", oCulture) & _
                                                  " " & sBackupFolder & " ...")
                            Call WriteMaintLogEntry(gpsRM.GetString("psBACKUP_MSG", oCulture) & sSourceFolder & _
                                                    gpsRM.GetString("psDEST_MSG", oCulture) & " " & sBackupFolder)

                            Application.DoEvents()
                            If oFolder.IncludeSubfolders Then
                                Application.DoEvents()
                                FileIO.FileSystem.CopyDirectory(sSourceFolder, sDestFolder, True)
                                If moBackupOptions.NewFolder Then
                                    Application.DoEvents()
                                    FileIO.FileSystem.CopyDirectory(sSourceFolder, sBackupFolderLatest & "\" & oFolder.FolderName, True)
                                    Application.DoEvents()
                                End If
                            Else
                                Application.DoEvents()
                                Dim sFiles() As String = System.IO.Directory.GetFiles(sSourceFolder & "\", "*.*", IO.SearchOption.TopDirectoryOnly)
                                For Each sFile As String In sFiles
                                    Application.DoEvents()
                                    Dim sParts() As String = Strings.Split(sFile, "\")
                                    Dim sFileName As String = sParts(sParts.GetUpperBound(0))

                                    Application.DoEvents()
                                    System.IO.File.Copy(sFile, sDestFolder & "\" & sFileName)
                                    Application.DoEvents()
                                    If moBackupOptions.NewFolder Then
                                        System.IO.File.Copy(sFile, sBackupFolderLatest & "\" & sFileName)
                                        Application.DoEvents()
                                    End If

                                Next 'sFile
                            End If


                        Catch ex As Exception

                            If sErrorMsg(0) = String.Empty Then
                                Call subPostStatusMsg(gpsRM.GetString("psERROR", oCulture) & ex.Message)
                                Call WriteMaintLogEntry(gpsRM.GetString("psERROR", oCulture) & ex.Message)
                                mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            Else
                                '02/02/10 RJO Added exception detail to Status and Maint Log posts.
                                Call subPostStatusMsg(sErrorMsg(0) & " - " & ex.Message)
                                Call WriteMaintLogEntry(sErrorMsg(0) & " - " & ex.Message)
                                Call subPostStatusMsg(sErrorMsg(1) & " - " & ex.Message)
                                Call WriteMaintLogEntry(sErrorMsg(1) & " - " & ex.Message)
                                mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                  sErrorMsg(0) & " - " & sErrorMsg(1) & " - " & "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End If
                        End Try
                    Next 'oFolder
                End If 'moMaint.FileBackupOptions

                If .ExtraCopies And (moExtraCopies.NumExtraCopies > 0) Then
                    For Each oExtraCopy As clsExtraCopies.udsExtraCopies In moExtraCopies.ExtraCopies
                        Try
                            Application.DoEvents()

                            sErrorMsg(0) = String.Empty
                            sErrorMsg(1) = String.Empty
                            Dim sDestFolder As String = sRemoveSlash(oExtraCopy.FolderTo)

                            sSourceFolder = sRemoveSlash(oExtraCopy.FolderFrom)
                            Application.DoEvents()

                            sErrorMsg(0) = gpsRM.GetString("psCOPY_FOLDER_ERR", oCulture) & sSourceFolder
                            sErrorMsg(1) = "  " & gpsRM.GetString("psDEST_MSG", oCulture) & sBackupFolder
                            Call subPostStatusMsg(gpsRM.GetString("psBACKUP_MSG", oCulture) & sSourceFolder & _
                                                  gpsRM.GetString("psDEST_MSG", oCulture) & _
                                                  " " & sDestFolder & " ...")
                            Call WriteMaintLogEntry(gpsRM.GetString("psBACKUP_MSG", oCulture) & sSourceFolder & _
                                                    gpsRM.GetString("psDEST_MSG", oCulture) & " " & sDestFolder)

                            Application.DoEvents()
                            If oExtraCopy.IncludeSubfolders Then
                                Application.DoEvents()
                                FileIO.FileSystem.CopyDirectory(sSourceFolder, sDestFolder, True)
                            Else
                                Application.DoEvents()
                                Dim sFiles() As String = System.IO.Directory.GetFiles(sSourceFolder & "\", "*.*", IO.SearchOption.TopDirectoryOnly)
                                If Not (Directory.Exists(sDestFolder)) Then
                                    Directory.CreateDirectory(sDestFolder)
                                End If
                                For Each sFile As String In sFiles
                                    Application.DoEvents()
                                    Dim sParts() As String = Strings.Split(sFile, "\")
                                    Dim sFileName As String = sParts(sParts.GetUpperBound(0))

                                    Application.DoEvents()
                                    System.IO.File.Copy(sFile, sDestFolder & "\" & sFileName, True)
                                    Application.DoEvents()
                                Next 'sFile
                            End If


                        Catch ex As Exception

                            If sErrorMsg(0) = String.Empty Then
                                Call subPostStatusMsg(gpsRM.GetString("psERROR", oCulture) & ex.Message)
                                Call WriteMaintLogEntry(gpsRM.GetString("psERROR", oCulture) & ex.Message)
                                mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            Else
                                '02/02/10 RJO Added exception detail to Status and Maint Log posts.
                                Call subPostStatusMsg(sErrorMsg(0) & " - " & ex.Message)
                                Call WriteMaintLogEntry(sErrorMsg(0) & " - " & ex.Message)
                                Call subPostStatusMsg(sErrorMsg(1) & " - " & ex.Message)
                                Call WriteMaintLogEntry(sErrorMsg(1) & " - " & ex.Message)
                                mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                                  sErrorMsg(0) & " - " & sErrorMsg(1) & " - " & "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End If
                        End Try
                    Next 'oExtraCopy
                End If 'If .ExtraCopies And (mnExtraCopies > 0) Then

                sErrorMsg(0) = String.Empty
                sErrorMsg(1) = String.Empty
            End With

        Catch ex As Exception

            If sErrorMsg(0) = String.Empty Then
                Call subPostStatusMsg(gpsRM.GetString("psERROR", oCulture) & ex.Message)
                Call WriteMaintLogEntry(gpsRM.GetString("psERROR", oCulture) & ex.Message)
                mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Else
                '02/02/10 RJO Added exception detail to Status and Maint Log posts.
                Call subPostStatusMsg(sErrorMsg(0) & " - " & ex.Message)
                Call WriteMaintLogEntry(sErrorMsg(0) & " - " & ex.Message)
                Call subPostStatusMsg(sErrorMsg(1) & " - " & ex.Message)
                Call WriteMaintLogEntry(sErrorMsg(1) & " - " & ex.Message)
                mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subFileBackup", _
                  sErrorMsg(0) & " - " & sErrorMsg(1) & " - " & "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End If

        End Try

    End Sub


    Private Sub subInitializeForm()
        '********************************************************************************************
        'Description: Put all startup code here, not in the form load event. this is called
        '             right after form is shown.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************

        Try
            Me.Cursor = Cursors.WaitCursor

            Me.Status(True) = gcsRM.GetString("csINITALIZING", frmStart.DisplayCulture)

            mcolZones = New clsZones(String.Empty)
            moMaint = New Maintenance(mcolZones.ActiveZone)

            Dim sPath As String = String.Empty

            Call mPWCommon.GetDefaultFilePath(msAppFilePath, eDir.VBApps, String.Empty, String.Empty)
            Call mPWCommon.GetDefaultFilePath(msDatabasePath, eDir.Database, String.Empty, String.Empty)
            Call mPWCommon.GetDefaultFilePath(sPath, eDir.PAINTworks, String.Empty, String.Empty)
            msLogFilePath = sPath & "PWMaint.log"

            lstStatus.Items.Clear()
            Call subLocalizeFormText()

            Call subUpdateDateTime()

            'initialize time and date display update timer
            tmrEvent.Interval = 1000
            tmrEvent.Enabled = True

            'initialize auto shutdown delay timer
            tmrClose.Interval = 1000
            tmrClose.Enabled = False

            '********New program-to-program communication object******************************************
            oIPC = New Paintworks_IPC.clsInterProcessComm(gs_COM_ID_MAINT, , , False)
            '********************************************************************************************

            'turn off the screen saver (in case it is running)
            Call subWakeUpScreenSaver()

            frmStart.Hide()

            'shutdown Paintworks
            Call subShutDown()

        Catch ex As Exception
            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: frmMain_Load", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Call subPostStatusMsg(gpsRM.GetString("psERROR", frmStart.DisplayCulture) & ex.Message)
            Call WriteMaintLogEntry(gpsRM.GetString("psERROR", frmStart.DisplayCulture) & ex.Message)
            'TODO - Keep this thing running or bail?
        Finally
            Me.Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub subLocalizeFormText()
        '********************************************************************************************
        'Description: Show the form text in the current language.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Dim oCulture As CultureInfo = frmStart.DisplayCulture
        Dim sVer As String = String.Empty

        sVer = CType(System.Diagnostics.FileVersionInfo.GetVersionInfo( _
                     System.Reflection.Assembly.GetExecutingAssembly.Location).FileMajorPart, String)
        sVer = sVer & "." & Format(System.Diagnostics.FileVersionInfo.GetVersionInfo( _
                    System.Reflection.Assembly.GetExecutingAssembly.Location).FileMinorPart, "00")
        sVer = sVer & "." & Format(System.Diagnostics.FileVersionInfo.GetVersionInfo( _
                    System.Reflection.Assembly.GetExecutingAssembly.Location).FileBuildPart, "00")
        sVer = sVer & "." & Format(System.Diagnostics.FileVersionInfo.GetVersionInfo( _
                    System.Reflection.Assembly.GetExecutingAssembly.Location).FilePrivatePart, "00")
        Me.Text = gpsRM.GetString("psPW4_SCHED_MAINT", oCulture) & " " & gcsRM.GetString("csVERSION", oCulture) & "  " & sVer

        With gpsRM
            btnViewLog.Text = .GetString("psVIEW_LOG_CAP", oCulture)
            btnPrintLog.Text = .GetString("psPRINT_LOG_CAP", oCulture)
            btnRestartPW.Text = .GetString("psRESTART_PW_CAP", oCulture)
            lblTitle.Text = .GetString("psSCREEN_TITLE", oCulture)
        End With

    End Sub

    Private Sub subPostStatusMsg(ByVal Message As String)
        '********************************************************************************************
        'Description: Display and manage status information in the Status list box
        '
        '
        'Parameters: Status Message text
        'Returns:    None
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        If lstStatus.Items.Count = 32767 Then lstStatus.Items.RemoveAt(0)
        lstStatus.Items.Add(Message)
        lstStatus.TopIndex = lstStatus.Items.Count - 1
        lstStatus.Refresh()

    End Sub

    Private Sub subPrintLog()
        '********************************************************************************************
        'Description: Maintenance Log Print routine.
        '
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         12/04/09      Convert to HTML printing
        '********************************************************************************************

        Try
            Dim oCulture As CultureInfo = frmStart.DisplayCulture

            Cursor = Cursors.WaitCursor
            btnPrintLog.Enabled = False
            Me.Status(True) = gcsRM.GetString("csPRINTING", oCulture)


            'set titles & headers
            Dim sSite As String = mcolZones.SiteName
            Dim sZone As String = mcolZones.ActiveZone.Name

            Dim sTitle As String = gpsRM.GetString("psPRINT_SECT_HDR", oCulture)
            Dim sSubTitles(0) As String
            sSubTitles(0) = gpsRM.GetString("psPRINT_SITE", oCulture) & ": " & sSite & vbTab & _
                         gpsRM.GetString("psPRINT_ZONE", oCulture) & ": " & sZone
            Dim sHeader As String = gpsRM.GetString("psPRINT_DATE_COL_HDR", oCulture) & vbTab & _
                                   gpsRM.GetString("psPRINT_TIME_COL_HDR", oCulture) & vbTab & _
                                   gpsRM.GetString("psPRINT_EVENT_COL_HDR", oCulture)
            rtbPrintLog.LoadFile(msLogFilePath)
            rtbPrintLog.Text = sHeader & rtbPrintLog.Text

            mPrintHtml = New clsPrintHtml(msSCREEN_NAME)
            mPrintHtml.subCreateSimpleDoc(rtbPrintLog, Status, sTitle, sSubTitles)

            mPrintHtml.subPrintDoc(False)
            mPrintHtml.subShowPrintPreview()

        Catch ex As Exception
            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: frmMain_Load", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            'TODO - Messagebox?
        Finally
            Me.Status(True) = gcsRM.GetString("csREADY", frmStart.DisplayCulture)
            btnPrintLog.Enabled = True
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub subReadBackupBits()
        '********************************************************************************************
        'Description: Loop until the robots that are required to do an internal backup to FRA:\ are 
        '             done.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         12/04/09      Wait for all robots
        ' RJO         04/01/13      Keep track of auto backup to FRA:\ timeout vs. error for better
        '                           error logging. Use timeout value from PLC.
        '********************************************************************************************

        Dim bIntBackupReqd As Boolean
        Dim nBailOut As Integer
        Dim bDone As Boolean = False
        Dim nHung As Integer = 600 'RJO 04/01/13

        Try
            If mnAutoBackupTimeout > nHung Then
                nHung = mnAutoBackupTimeout 'RJO 04/01/13
            End If

            Do While Not (bDone)
                Dim dtResume As DateTime = DateAdd(DateInterval.Second, 1, Now)

                Do While Now < dtResume
                    Application.DoEvents()
                Loop
                nBailOut += 1
                bDone = True
                For Each oRC As clsController In mcolControllers

                    If oRC.InternalBackupEnabled Then
                        Dim oZone As clsZone = oRC.Zone
                        Dim sTag As String = "Z" & oZone.ZoneNumber.ToString & oRC.FanucName & "BackupStat"
                        Dim sData() As String

                        bIntBackupReqd = True

                        With moPLC
                            .ZoneName = oZone.Name
                            .TagName = sTag
                            sData = .PLCData
                        End With

                        Select Case sData(0)
                            Case "1", "3"
                                ' Error bit (0) is ON
                                mbBackupErr(oRC.ControllerNumber) = True
                            Case "2"
                                ' Backup still in progress
                                bDone = False 'Not done, make them all wait
                            Case "0"
                                'Done
                        End Select
                    End If 'oRC.InternalBackupEnabled

                    'If nBailOut = nHUNG Then mbBackupErr(oRC.ControllerNumber) = True 'RJO 04/01/13
                    If nBailOut = nHung Then mbBackupTimeout(oRC.ControllerNumber) = True

                Next 'oRC

                'If (nBailOut = nHUNG) Or (Not bIntBackupReqd) Then bInProg = False
                If (nBailOut = nHung) Then bDone = True
            Loop

            'Internal Backup compete. Turn off DoBackup command bits
            For Each oRC As clsController In mcolControllers
                If oRC.InternalBackupEnabled Then
                    Dim oZone As clsZone = oRC.Zone
                    Dim sTag As String = "Z" & oZone.ZoneNumber.ToString & oRC.FanucName & "DoBackup"
                    Dim sData(0) As String

                    sData(0) = "0"
                    With moPLC
                        .ZoneName = oZone.Name
                        .TagName = sTag
                        .PLCData = sData
                    End With

                    Application.DoEvents()
                End If
            Next 'oRc

        Catch ex As Exception
            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subReadBackupBits", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subRestartPW()
        '********************************************************************************************
        'Description: Shuts down, then re-starts Windows (and the PAINTworks GUI)
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        If Not InIDE Then Call subWin32Restart()

        End

    End Sub

    Private Sub subRestoreDB(ByVal DbBackupFileName As String, ByVal DbBackupFilePath As String)
        '********************************************************************************************
        'Description: Restores a SQL server database.
        '
        'Parameters: DbBackupFileName - The backup file name (ex. "dbo.Alarm Log.back")
        '            DbBackupFilePath - The path to the backup file (ex. "C:\Database\Zone1\Backup")
        'Returns:    none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        'Note: This routine is not used in PW_Maint.NET but is stored here for future use in a
        '      Paintworks databse restore utility.
        Try
            Dim oBdi As BackupDeviceItem
            Dim oBuilder As New SqlClient.SqlConnectionStringBuilder
            Dim oDB As Database
            Dim oRecoveryModel As RecoveryModel
            Dim oRestore As New Restore
            Dim oServer As New Server
            Dim sDBName As String = String.Empty
            Dim sParts() As String = Strings.Split(DbBackupFileName, ".")

            'This assumes that the DbBackupFileName is dbo.<DatabseName>.bak
            sDBName = sParts(1)

            'Connect to the local, default instance of SQL Server.
            With oBuilder
                .IntegratedSecurity = True
                .InitialCatalog = sDBName
                .ContextConnection = False 'default
                .UserInstance = False
                .MultipleActiveResultSets = True ' not default
                .LoadBalanceTimeout = 30 ' not default
                .AsynchronousProcessing = True ' not default
                .AttachDBFilename = msDatabasePath & sDBName & ".mdf"
                'TODO - Where will the servername (ex. "SQLEXPRESS") be stored? This needs to come from there.
                .DataSource = "(local)\" & msSQLSERVERNAME


                oServer.ConnectionContext.ConnectionString = .ConnectionString
            End With

            'Reference the sDBName database.
            oDB = oServer.Databases(sDBName)

            'Store the current recovery model in a variable.
            oRecoveryModel = oDB.DatabaseOptions.RecoveryModel

            'Define a BackupDeviceItem by supplying the backup device file name in the constructor, and the type of device is a file.
            oBdi = New BackupDeviceItem(DbBackupFilePath & "\" & DbBackupFileName, DeviceType.File)

            'Delete the sDBName database before restoring it.
            oServer.Databases(sDBName).Drop()

            'Define a Restore object variable and restore the database.
            With oRestore
                'Set the NoRecovery property to False, so the transactions are recovered.
                .NoRecovery = False
                'Add the device that contains the full database backup to the Restore object.
                .Devices.Add(oBdi)
                'Specify the database name.
                .Database = sDBName
                'Restore the full database backup with no recovery.
                .SqlRestore(oServer)
                'Wait for it...
                .Wait()
                'Remove the device from the Restore object.
                .Devices.Remove(oBdi)
            End With

            'Set the database recovery mode back to its original value.
            oServer.Databases(sDBName).DatabaseOptions.RecoveryModel = oRecoveryModel
            oServer = Nothing

        Catch ex As Exception
            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subRestoreDB", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subRobotBackup()
        '********************************************************************************************
        'Description:   Sequences robot backup operations
        '
        'Parameters:  None
        '
        'Returns:    None
        '
        'Modification history:
        '
        ' By       Date         Reason
        ' BTK      02/09/10     Set bNoFilter to false so we can filter out _backup_, -bcked or klevaxdf.vr
        ' MSW      05/14/10     Handle missing autobackup files.
        ' MSW      03/23/11     For individual folder optnion, add a "latest" folder and backup to both so the
        '                       latest backup can always be found in the same subfolder
        ' MSW      11/03/11     Support optional path for auto backups
        ' RJO      04/01/13     Additional Internal Backup to FRA: error detail
        '********************************************************************************************
        Dim nProgressStep As Integer
        Dim nRetryCount As Integer
        Dim oCulture As CultureInfo = frmStart.DisplayCulture
        Dim sBackupPath As String = String.Empty
        Dim sBackupFolder As String = String.Empty
        Dim sMsg As String = String.Empty
        Dim sBackupFolderLatest As String = String.Empty

        Try
            Me.Status(True) = gpsRM.GetString("psBACKUP_MSG", oCulture) & _
                              gpsRM.GetString("psCONTROLLER_FILES", oCulture)
            Call mPWCommon.GetDefaultFilePath(sBackupPath, eDir.PWRobotBackups, String.Empty, String.Empty)

            Application.DoEvents()
            'check available space on destination drive. exit if there isn't enough free space.
            If moMaint.DriveFull(Strings.Left(sBackupPath, 1)) Then
                Call subPostStatusMsg(gpsRM.GetString("psERROR", oCulture) & _
                                      gpsRM.GetString("psDISK_LOW_SPACE_ERR", oCulture))
                Call WriteMaintLogEntry(gpsRM.GetString("psERROR", oCulture) & _
                                        gpsRM.GetString("psDISK_LOW_SPACE_ERR", oCulture))
                Application.DoEvents()

                Exit Sub
            End If

            'make sure the backup path exists, if not then try to create it
            If Not Directory.Exists(sBackupPath) Then Directory.CreateDirectory(sBackupPath)

            'Set up "latest" directory
            sBackupFolderLatest = sBackupPath & "\" & gpsRM.GetString("psLATEST", oCulture)
            msLatestRobtBackups = sBackupFolderLatest
            Application.DoEvents()
            Try
                Directory.Delete(sBackupFolderLatest, True)
            Catch ex As Exception
            End Try
            sBackupFolder = sNewFolder()
            'create backup folder for the current date and time
            sBackupPath = sBackupPath & sBackupFolder

            sMsg = gpsRM.GetString("psROB_BACKUP_FOLDER_MSG", oCulture) & sBackupPath
            Call subPostStatusMsg(sMsg & " ...")
            Call WriteMaintLogEntry(sMsg)
            Application.DoEvents()

            Directory.CreateDirectory(sBackupPath)

            If mcolControllers.Count < 1 Then
                'log this
                sMsg = gpsRM.GetString("psNO_ROB_ERR", oCulture)
                Call subPostStatusMsg(sMsg)
                Call WriteMaintLogEntry(sMsg)
                Application.DoEvents()

                Exit Sub
            ElseIf mcolControllers.Count < 11 Then
                'TODO - need code for the robot panel updates on comm status change
                mbShowRobots = True
                Application.DoEvents()
            End If

            ' Figure out how much to bump the progress bar each time a robot backup is done
            nProgressStep = (1 \ mcolControllers.Count) * 33

        Catch ex As Exception
            sMsg = gpsRM.GetString("psINTERNAL_ERROR", oCulture)
            Call subPostStatusMsg(sMsg)
            Call WriteMaintLogEntry(sMsg)

            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subRobotBackup", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Application.DoEvents()
        End Try

        'Loop through the robot controllers and try to backup the files
        For Each oRC As clsController In mcolControllers
            Dim bIntBackup As Boolean = oRC.InternalBackupEnabled
            Dim bNofilter As Boolean = False 'BTK 02/09/10 Need to filter
            Dim nIndex As Integer = oRC.ControllerNumber
            Dim sDevice As String = oRC.FanucName
            Dim sName As String = oRC.Name
            Application.DoEvents()

            ' make sure internal backup was a success
            If oRC.InternalBackupEnabled And (mbBackupErr(oRC.ControllerNumber) Or mbBackupTimeout(oRC.ControllerNumber)) Then 'RJO 04/01/13
                If mbBackupErr(oRC.ControllerNumber) Then
                    sMsg = gpsRM.GetString("psINT_BACKUP_ERR_MSG", oCulture) & sName
                Else
                    sMsg = gpsRM.GetString("psAUTO_BKP_TIMEOUT", oCulture) & sName 'RJO 04/01/13
                End If

                Call subPostStatusMsg(sMsg)
                Call WriteMaintLogEntry(sMsg)
                Application.DoEvents()
            Else
                If oRC.RCMConnectStatus = FRRobotNeighborhood.FRERNConnectionStatusConstants.frRNConnected Then
                    'log this event
                    sMsg = gpsRM.GetString("psBACKUP_MSG", oCulture) & sName & _
                           gpsRM.GetString("psDEST_MSG", oCulture) & _
                                            sBackupPath & "\" & sName

                    Call subPostStatusMsg(sMsg & " ...")
                    Call WriteMaintLogEntry(sMsg)
                    Application.DoEvents()

                    nRetryCount = 0

                    oRC.HeartbeatEnable = False

                    Me.Status(True) = gpsRM.GetString("psGET_DIR_STATUS", oCulture) & sName

                    Dim oFTP As New clsFSFtp(sDevice)

                    With oFTP
                        If .Connected Then
                            'set the working dir on the host robot controller
                            If bIntBackup Then
                                .WorkingDir = "FRA:\"
                            Else
                                If oRC.BackupPath.Trim = String.Empty Then
                                    If oRC.Version >= 5.3 Then
                                        .WorkingDir = "MDB:\"
                                    Else
                                        .WorkingDir = "MD:\"
                                        bNofilter = False
                                    End If
                                Else
                                    .WorkingDir = oRC.BackupPath
                                    If oRC.BackupPath = "MD:\" Then
                                        bNofilter = False
                                    End If
                                End If
                            End If

                            'get directory
                            Dim sDir() As String = .Directory("*.*")

                            If (sDir(0) = String.Empty) And (bIntBackup) Then
                                'No autobackup available, get the direct backup
                                sMsg = "  " & gpsRM.GetString("psERROR", oCulture) & _
                                       gpsRM.GetString("psROB_DIR_ERR1", oCulture) & sName & "."
                                Call subPostStatusMsg(sMsg)
                                Call WriteMaintLogEntry(sMsg)
                                sMsg = gpsRM.GetString("psROB_DIR_ERR2", oCulture) & ""
                                If oRC.Version >= 5.3 Then
                                    .WorkingDir = "MDB:\"
                                    sMsg = sMsg & "(MDB:\)."
                                Else
                                    .WorkingDir = "MD:\"
                                    bNofilter = False
                                    sMsg = sMsg & "(MD:\)."
                                End If
                                Call subPostStatusMsg(sMsg)
                                Call WriteMaintLogEntry(sMsg)
                                sDir = .Directory("*.*")
                                Application.DoEvents()
                            End If
                            If sDir(0) <> String.Empty Then
                                'make a place to put the backed up files
                                Directory.CreateDirectory(sBackupPath & "\" & sName)

                                'backup all of the files in the directory
                                For Each sFile As String In sDir
                                    If bNofilter OrElse bFilter(sFile) Then
                                        sMsg = gpsRM.GetString("psBACKUP_MSG", oCulture) & sName & " " & sFile
                                        Me.Status(True) = sMsg
                                        Call WriteMaintLogEntry(sMsg)

                                        .DestDir = sBackupPath & "\" & sName & "\"
                                        If Not .GetFile(sFile, sFile) Then
                                            sMsg = gpsRM.GetString("psROB_FILE_BACKUP_ERR", oCulture) & sFile & " - " & .ErrorMsg
                                            Call WriteMaintLogEntry(sMsg)
                                        End If
                                        Application.DoEvents()
                                    End If
                                Next 'sFile
                            Else
                                'didn't get dir  - log this
                                sMsg = gpsRM.GetString("psERROR", oCulture) & _
                                       gpsRM.GetString("psROB_DIR_ERR", oCulture) & sName
                                Call subPostStatusMsg(sMsg)
                                Call WriteMaintLogEntry(sMsg)
                                Application.DoEvents()
                            End If

                            .Close()
                        Else
                            'can't connect - log this
                            sMsg = gpsRM.GetString("psERROR", oCulture) & _
                                   gpsRM.GetString("psNO_CONNECT_ERR", oCulture) & sName
                            Call subPostStatusMsg(sMsg)
                            Call WriteMaintLogEntry(sMsg)
                            Application.DoEvents()
                        End If
                    End With 'oFTP

                    oRC.HeartbeatEnable = True

                End If 'frRNConnected
            End If

            Me.Progress += nProgressStep
            Call subWakeUpScreenSaver()
            Application.DoEvents()

        Next 'oRC

        FileIO.FileSystem.CopyDirectory(sBackupPath, sBackupFolderLatest, True)

    End Sub

    Private Sub subShutDown()
        '********************************************************************************************
        'Description: Shuts down all PAINTworks apps including PW4_Main to unlock databases for
        '             cleanup and compression.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' By           Date         Reason
        ' MSW          01/20/12     use common routine so it'll work in IDE
        ' RJO          03/22/12     removed reference to mWorksComm
        '********************************************************************************************
        Dim oCulture As CultureInfo = frmStart.DisplayCulture

        Try
            'Dim procMain As Process = mWorksComm.GetProcess("PW4_Main") 'RJO 03/22/12
            Dim procMain As Process = oIPC.GetProcess(gs_COM_ID_PW_MAIN)
            Const nTIMEOUT As Integer = 60

            If procMain IsNot Nothing Then
                Dim bExit As Boolean = False
                Dim nSeconds As Integer = 0

                Me.Status(True) = gpsRM.GetString("psSHUTDOWN_STATUS", oCulture) & "PW4_Main"
                Call WriteMaintLogEntry(gpsRM.GetString("psSHUTDOWN_STATUS", oCulture) & "PW4_Main")
                Call subPostStatusMsg(gpsRM.GetString("psSHUTDOWN_MSG", oCulture))
                'send shut down message to PW4_Main
                'mWorksComm.SendFRWMMessage("close,0,0,0,0,0", procMain.Id)
                Dim sMessage(0) As String
                sMessage(0) = "close"
                oIPC.WriteControlMsg(gs_COM_ID_PW_MAIN, sMessage)
                'wait until PW4_Main shutsdown or timeout
                Do While Not bExit
                    Threading.Thread.Sleep(1000)
                    If procMain.HasExited Then Exit Do
                    nSeconds += 1
                    If nSeconds > nTIMEOUT Then
                        mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subShutDown", _
                                               "PW4_Main shutdown timed out.")

                        Exit Do
                    End If
                Loop
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subShutDown", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try


    End Sub

    Private Sub subSetBackupBits()
        '********************************************************************************************
        'Description: Tell controllers that are required to do an internal backup to FRA:\ to start
        '             the internal backup.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' RJO       04/01/13        Added separate error message for auto backup to FRA:\ timeout
        '********************************************************************************************

        Try
            Dim nMaxControllerNum As Integer = 0

            For Each oRC As clsController In mcolControllers
                If oRC.ControllerNumber > nMaxControllerNum Then
                    nMaxControllerNum = oRC.ControllerNumber
                    ReDim Preserve mbBackupErr(nMaxControllerNum)
                    ReDim Preserve mbBackupTimeout(nMaxControllerNum) 'RJO 04/01/13
                End If

                If oRC.InternalBackupEnabled Then
                    Dim oZone As clsZone = oRC.Zone
                    Dim sZone As String = oZone.ZoneNumber.ToString
                    Dim sTag As String = "Z" & sZone & oRC.FanucName & "DoBackup"
                    Dim sData(0) As String

                    mbInternalBackupReqd = True
                    sData(0) = "1"

                    With moPLC
                        .ZoneName = oZone.Name
                        .TagName = sTag
                        .PLCData = sData
                    End With
                End If
            Next 'oRC

        Catch ex As Exception
            mDebug.WriteEventToLog(frmStart.msSCREEN_NAME & " Module: " & msMODULE & " Routine: subSetBackupBits", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subUpdateDateTime()
        '********************************************************************************************
        'Description:   Updates the Time and Date fields on the main form.
        '
        'Parameters:    None
        'Returns:       None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        lblDate.Text = Now.ToShortDateString
        lblDate.Refresh()
        lblTime.Text = Now.ToShortTimeString
        lblTime.Refresh()

    End Sub

    Private Sub subUpdateMaintDate(ByVal Item As String)
        '********************************************************************************************
        'Description:   This Sub will update the last performed date in the Schedule database
        '
        'Parameters:   name of database field update
        'Returns:    None
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         03/28/12      Move DB to XML
        ' MSW         06/05/13      Use fixed culture to write the date
        '********************************************************************************************
        Dim oZone As clsZone = mcolZones.ActiveZone
        Const sXMLFILE As String = "SchedLastPerformed"
        Const sXMLTABLE As String = "SchedLastPerformed"
        Const sXMLNODE As String = "LastPerformedEntry"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Dim bSuccess As Boolean = False
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLFILE & ".XML") Then
            Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)
                If oNodeList.Count > 0 Then
                    For Each oNode As XmlNode In oNodeList
                        If oNode.Item("Item").InnerXml = Item Then
                            oNode.Item("LastPerformed").InnerXml = Now.ToString(mLanguage.FixedCulture)
                            bSuccess = True
                            Exit For
                        End If
                    Next
                End If
            Catch ex As Exception
                bSuccess = False
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subUpdateMaintDate", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If

        oXMLDoc.Save(sXMLFilePath)

        If Not bSuccess Then
            Dim sMsg As String = gpsRM.GetString("psLAST_PERF_ERR", frmStart.DisplayCulture) & Item

            Call subPostStatusMsg(sMsg)
            Call WriteMaintLogEntry(sMsg)
        End If
    End Sub

    Private Sub subWakeUpScreenSaver()
        '********************************************************************************************
        'Description:   Nudge the cursor a few times to turn off the screen saver
        '
        'Parameters:   None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim CurrentCursorPos As System.Drawing.Point = Cursor.Position

        For nLoop As Integer = 0 To 20    'this should wake up the screen saver
            CurrentCursorPos.X += 1
            CurrentCursorPos.Y += 1
            Cursor.Position = CurrentCursorPos
            Application.DoEvents()
            CurrentCursorPos.X -= 1
            CurrentCursorPos.Y -= 1
            Cursor.Position = CurrentCursorPos
            Application.DoEvents()
        Next 'nLoop

    End Sub

    Private Sub subWin32Restart()
        '********************************************************************************************
        'Description:   This Sub will Reboot the computer (without any warning).
        '
        'Parameters:   None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        System.Diagnostics.Process.Start("ShutDown", "-r -t 00")

    End Sub

    Friend Sub WriteMaintLogEntry(ByVal LogText As String)
        '********************************************************************************************
        'Description:   This Sub will write one line of text into the PWMaint.log log file along with
        '               a time/date stamp.
        '
        'Parameters: Text string to write to log
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        File.AppendAllText(msLogFilePath, DateTime.Now.ToString & " - " & LogText & vbCrLf)

    End Sub

#End Region

#Region " Events "

    Private Sub btnPrintLog_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrintLog.Click
        '********************************************************************************************
        'Description: Prints scheduled maintenace log file PWMAINT.LOG.
        '
        'Parameters: none
        'Returns:   none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        tmrClose.Enabled = False
        Call subPrintLog()

    End Sub

    Private Sub btnRestartPW_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRestartPW.Click
        '********************************************************************************************
        'Description: User has indicated status/log viewing complete. Restart computer
        '
        'Parameters: none
        'Returns:   none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Call subRestartPW()

    End Sub

    Private Sub btnViewLog_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnViewLog.Click
        '********************************************************************************************
        'Description: Shows scheduled maintenace log file PWMAINT.LOG in lstStatus list box.
        '
        'Parameters: none
        'Returns:   none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        tmrClose.Enabled = False
        btnViewLog.Enabled = False
        lblTitle.Text = gpsRM.GetString("psLOG_TITLE", frmStart.DisplayCulture)
        lstStatus.Items.Clear()

        ' Read the lines from the maintenance log file until the end 
        ' of the file is reached.
        Dim oSR As StreamReader = New StreamReader(msLogFilePath)
        Dim sBuffer As String

        Do While Not oSR.EndOfStream
            sBuffer = oSR.ReadLine
            lstStatus.Items.Add(sBuffer)
        Loop

        oSR.Close()

    End Sub

    Private Sub frmMain_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        '********************************************************************************************
        'Description: Trap alt - key  combinations to simulate menu button clicks
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
		'    09/30/13   MSW     Save screenshots as jpegs
        '********************************************************************************************
        Dim sKeyValue As String = String.Empty

        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode

                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty

                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath & msSCREEN_DUMP_NAME, Imaging.ImageFormat.Jpeg)

                Case Keys.Escape
                    If btnRestartPW.Visible Then
                        Call subRestartPW()
                    End If
                Case Else

            End Select
        End If

    End Sub

    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description:   Shows, positions and initializes frmMain
        '
        'Parameters:  None
        '
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'put startup code in subInitializeForm
        Call subInitializeForm()

        Me.Show()
        Application.DoEvents()

        'Delete the old log file. If it doesn't exist, no error will occur.
        File.Delete(msLogFilePath)

        'TODO - There's no PW4 utility to write an anonymous alarm to the alarm log.
        'Call pw3api.LogPWMsg("GUI", msSCREEN_NAME & Localize.GetLocalString(PString.psMAINT_START), MAINT_START_MSG)
        Call WriteMaintLogEntry(gpsRM.GetString("psPW4_SCHED_MAINT", frmStart.DisplayCulture) & _
                                gpsRM.GetString("psMAINT_START", frmStart.DisplayCulture))

        'do maintenance operations
        Call subDoMaintenance()

        'log operation complete
        'TODO - There's no PW4 utility to write an anonymous alarm to the alarm log.
        'Call pw3api.LogPWMsg("GUI", msScreenCaption & Localize.GetLocalString(PString.psMAINT_DONE), MAINT_DONE_MSG)
        Call WriteMaintLogEntry(gpsRM.GetString("psPW4_SCHED_MAINT", frmStart.DisplayCulture) & _
                                gpsRM.GetString("psMAINT_DONE", frmStart.DisplayCulture))
        Call subPostStatusMsg(gpsRM.GetString("psPW4_SCHED_MAINT", frmStart.DisplayCulture) & _
                              gpsRM.GetString("psMAINT_DONE", frmStart.DisplayCulture))

    End Sub

    Private Sub colControllers_ConnectionStatusChange(ByVal Controller As clsController) Handles mcolControllers.ConnectionStatusChange
        '********************************************************************************************
        'Description: Robot Controller connection status changed. Update the status bar.
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'this is needed with old RCM object raising events
        'and can hopefully be removed when I learn how to program
        'Control.CheckForIllegalCrossThreadCalls = False
        If Me.stsStatus.InvokeRequired Then
            Dim dCntrStat As New ControllerStatusChange(AddressOf colControllers_ConnectionStatusChange)
            Me.BeginInvoke(dCntrStat, New Object() {Controller})
        Else
            Call subDoStatusBar(Controller)
        End If
        'Control.CheckForIllegalCrossThreadCalls = True

    End Sub

    Private Sub tmrClose_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrClose.Tick
        '********************************************************************************************
        'Description:   Waits mnAUTO_SHUTDOWN_DELAY seconds, then closes the main form and restarts
        '               PAINTworks.
        '
        'Parameters:    None
        'Returns:       None
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        mnElapsedTime = mnElapsedTime + 1

        If mnElapsedTime >= mnAUTO_SHUTDOWN_DELAY Then
            Call subRestartPW()
        End If

    End Sub

    Private Sub tmrEvent_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrEvent.Tick
        '********************************************************************************************
        'Description:   Update the Time and Date fields on the main form
        '
        'Parameters:    None
        'Returns:       None
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        tmrEvent.Enabled = False
        Call subUpdateDateTime()
        tmrEvent.Enabled = True

    End Sub

#End Region

End Class