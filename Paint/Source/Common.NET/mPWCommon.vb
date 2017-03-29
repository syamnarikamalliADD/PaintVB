' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2006 - 2007
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: mPWCommon
'
' Description: Common Routines for paintworks
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Speedy
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    03/17/07   gks     Onceover Cleanup
'    06/27/07   gks     Redo of the getdefaultfilepath thing
'    08/15/07   gks     Typed change collection
'    05/29/09   MSW     Added access to color change database for applicator setup
'    08/28/09   MSW     Add subLaunchHelp
'    11/05/09   MSW     LoadRobotBoxFromDB - Add CC and opener conditions
'    10/14/11   MSW     Read from XML                                                      4.01.01.00
'    01/03/12   MSW     Changes for speed - move applicator tables to XML                  4.01.01.01
'    01/24/12   MSW     Applicator Updates                                                 4.01.01.02
'    01/24/12   MSW     Change to new Interprocess Communication in clsInterProcessComm    4.01.01.02
'    01/30/12   MSW     Make sure it gets a unique directory name if there's no extension  4.01.01.03
'                       Add a routine to make a legal filename by stripping out punctuation
'    03/18/12   MSW     Add a notepad folder, scattered access setup                       4.01.03.00
'    04/11/12   RJO     Added code to support dispensers in sealer applications            4.01.03.01
'    06/07/12   MSW     SaveToChangeLog - Change cursor and call doevents to deal with     4.01.03.02
'                       slow SQL access
'    06/12/12   MSW     Change so screens write change log items to XML and ProdLogger     4.01.04.00
'                       moves to XML to remove 5 second delay for first SQL access on most screens
'    10/01/12   RJO     Added some string correction code to SaveToChangeLog(oZone). XML   4.01.04.01
'                       frowns on strings that contain "<" or ">".
'    11/15/12   HGB     SaveToChangeLog bug fix. Added new overload for LoadRobotBoxFromDB 4.01.04.02
'    12/13/12   MSW     Support for standalone programs without DB                         4.01.04.03
'    02/27/13   MSW     Support standalone programs without SQL                            4.01.04.04
'                       by declaring tChange and colChangesSQL as Friend instead of Private
'						Move Routine SaveToChangeLogSqlOld to mChangeLogger.vb
'                       Requires ...\Source\ProdLogger.NET\mChangeLogger.vb v4.01.04.04
'                       UsePaintWorksDB - Check the path directly instead of calling GetDefaultFilePath
'                       GetDefaultFilePath - More standalone support.  Find executables and 
'                       data files for App installed by the publish feature.              
'                       make the path search more specific - "/Paint/" instead of "Paint"  
'    04/16/13   MSW     bLocateDirectory - Don't create DMON Viewer folder automatically,  4.01.05.00
'                       it's not used anymore
'                       Standalone Changelog - move the rest of the DB access and SQL 
'                       routines to DB or SQL classes
'                       Add ShowChangeLog routine to launch standalone changelog viewer
'    05/02/13   MSW     SaveToChangeLog - save the dat in American, not Canadian
'    05/09/13   MSW     Add Hot Edit Logger DO[] for sealer                                4.01.05.01
'    05/16/13   ASD/TSB Added code to Function IPFromHostName that tries a second time     4.01.05.02
'                       when Dns.GetHostEntry fails to return a host name, using the 
'                       obsolete Dns.GetHostByName method.
'    05/16/13   MSW     remove references to frmMain, use the process name instead         4.01.05.03
'    05/16/13   MSW     Add vision controller item to controller settings
'    07/08/13   MSW     run screen names through language files                            4.01.05.04
'    08/20/13   MSW     PSC remote PC support                                              4.01.05.05
'    08/30/13   MSW     SaveToChangeLog - Error Handling                                   4.01.05.06
'    12/03/13   MSW     Added code to support materials in sealer applications             4.01.06.00
'    02/13/14   MSW     Fairfax sealer updates                                             4.01.07.00
'    02/18/14   MSW     GetArmArray - Add error handling to station number                 4.01.07.01
'    03/18/14   MSW     ZDT Changes - bLocateDirectory - ZDT folder                        4.01.07.02
'    04/03/14   MSW     ZDT Changes - GetDefaultFilePath - keep capitalization             4.01.07.03
'    04/03/14   MSW     bLocateDirectory - Handle DB without zone subfolder
'    03/24/14   DJM     Added HasManualCycles property to filter arms in sealer appl.      4.01.07.04
'    05/20/14   MSW     IPFromHostName - Don't return 0000 for the first disabled adapter  4.01.07.05
'    06/14/14   MSW     Add some error traps                                               4.01.07.05
'    07/30/14   MSW     Add Reports folder, changelog error handling                       4.01.07.06
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System
Imports System.Net
Imports System.IO
Imports System.Resources
Imports System.Windows.Forms.Application
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Xml
Imports System.Xml.XPath

Friend Module mPWCommon

#Region " Declares "

    Declare Function ShowWindowAsync Lib "user32.dll" ( _
        ByVal Hwnd As IntPtr, _
        ByVal swCommand As Int32) As IntPtr

    Declare Function SetForegroundWindow Lib "user32.dll" ( _
        ByVal Hwnd As IntPtr) As Boolean

    Private Const msPAINTDIRECTORY As String = "\Paint\"
    Private Const msPAINTNETDIRECTORY As String = "\Paint.Net\"
    Private Const msSEALERDIRECTORY As String = "\Sealer\"
    Private Const msSEALERNETDIRECTORY As String = "\SEALER.Net\"

    Friend Const gnALLZONES As Integer = 0
    'Friend bNoRobotDebug As Boolean = True 'NRU 161103
    '9.5.07 remove initializations per fxCop
    Friend colChanges As Collection(Of tChange) ' = Nothing
    '
    Friend colChangesSQL As Collection(Of tChange) ' = Nothing
    Private msRemotePWpath As String = String.Empty
    Private msDBPathCache As String = String.Empty
    Private mbInIDE As Boolean ' = False
    'for debug
    Friend oWatch As Stopwatch
    Friend gbUseWatch As Boolean ' = False
    '    11/15/12   MSW     Support for standalone programs without DB                         4.01.05.00
    Dim mbUsePaintWorksDB As Boolean = False
    Dim mbUsePaintWorksDBInit As Boolean = False
    Friend mnMinimumZoneNumber As Integer = 1

    Friend Structure tChange
        Dim Area As String
        Dim User As String
        Dim Zone As Integer ' for backwards compatibility
        Dim ZoneName As String
        Dim Device As String
        Dim Parm1 As String
        Dim StartSerial As DateTime
        Dim Specific As String
    End Structure

    Friend Structure tController
        Dim DisplayName As String
        Dim HostName As String
        Dim ControllerNumber As Integer
        Dim SortOrder As Integer
        Dim ControllerType As Integer
        Dim InternalBackup As Boolean
        Dim BackupPath As String
        Dim HotEditLoggerDO As Integer
        Dim VisionController As Boolean
    End Structure
    Friend Structure tArm
        Dim ArmDisplayName As String
        Dim RobotNumber As Integer
        Dim StationNumber As Integer
        Dim ArmSortOrder As Integer
        Dim ArmControllerNumber As Integer
        Dim ArmNumber As Integer
        Dim ArmType As Integer
        Dim ColorChangeType As Integer
        Dim Dispensers As Integer '04/11/12 RJO 
        Dim DispenserType As Integer '04/11/12 RJO 
        Dim Materials As Integer '12/03/13 MSW 
        Dim EstatType As Integer
        Dim EstatHostName As String
        Dim IsOpener As Boolean
        Dim PLCTagPrefix As String
        Dim PaintToolAlias As String
        Dim Controller As tController
        Dim ScatteredAccessGroups As String
        Dim HasManualCycles As Boolean
        Dim PigSystems As Integer 'NRU 161215 Piggable
    End Structure
#End Region
#Region " Enumerations "

    ' 08/30/13  Add Other
    'show which subdirectory we are looking for
    Friend Enum eDir As Integer
        PAINTworks
        XML
        Database
        VBApps
        VB6Apps
        ScreenDumps
        Help
        MasterBackups
        PWRobotBackups
        TempBackups
        WeeklyReports
        ECBR
        RobotImageBackups
        DBBackup
        DmonData
        DmonArchive
        Profile
        Source
        DmonViewer
        FanucManuals
        IPC
        Notepad
        ChangeLogger
        Reports  '    07/30/14   MSW     Add Reports folder, changelog error handling
        Other
        ZDT   '    03/18/14   MSW     ZDT Changes
    End Enum
    ' enums for PLC communication
    Friend Enum PLCCommErrors
        ConfigFileNotFound = vbObjectError + 1000  ' GEFPlcTags.xml not found
        ConfigItemNotDefined                       ' Tag does not exist
        TagNameNotSet                              ' TagName property = ""
        ZoneNumberNotSet                             ' ZoneName property = ""
        InvalidData                                ' Invalid data supplied for PLC data write
        CommLinkError                              ' PLC Communication error (hotlink)
        ReadFailed
        WriteFailed
        WrongArraySize
    End Enum

#End Region
#Region " Properties "
    Public Property UsePaintWorksDB() As Boolean
        '****************************************************************************************
        'Description: Check for the PW database folder so screens can support standalone operation
        '             Only used for DMON viewer as far as I know.
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/15/12  MSW     Support for standalone programs without DB                   
        ' 02/27/13  MSW     UsePaintWorksDB - Check the path directly instead of calling 
        '                   GetDefaultFilePath and make it more specific - "\Paint\" instead of "Paint"
        '*****************************************************************************************
        Get
            If Not (mbUsePaintWorksDBInit) Then
                Dim sPath As String = String.Empty
                sPath = StartupPath.ToLower(mLanguage.FixedCulture)
                'determine the part of the path we started in that we want
                mbUsePaintWorksDB = (InStr(sPath, msPAINTDIRECTORY.ToLower(mLanguage.FixedCulture)) > 0) OrElse _
                                    (InStr(sPath, msSEALERDIRECTORY.ToLower(mLanguage.FixedCulture)) > 0)
                mbUsePaintWorksDBInit = True
            End If
            Return mbUsePaintWorksDB
        End Get
        Set(ByVal value As Boolean)
            mbUsePaintWorksDB = True
        End Set
    End Property
    Private ReadOnly Property bInIDE() As Boolean
        Get
            mbInIDE = True
            Return True
        End Get
    End Property
    Friend Property InIDE() As Boolean
        '****************************************************************************************
        'Description: This will come in handy if we figure out how to set it...
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Debug.Assert(bInIDE)

            Return mbInIDE
        End Get
        Set(ByVal value As Boolean)
            mbInIDE = value
        End Set
    End Property
    Friend Property RemotePWPath() As String
        '****************************************************************************************
        'Description: Follow the yellow brick road - this is for a browsed to path only 
        '              mnuremote on frmmain -  not normal mode of operation
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Get
            Return msRemotePWpath
        End Get
        Set(ByVal value As String)
            ' this is only for clearing path - seting should be done by calling setremoteserver
            msRemotePWpath = String.Empty
        End Set
    End Property

#End Region
#Region " Routines "
    Friend Function sGetScreenDisplayName(ByVal sScreenName As String) As String
        '********************************************************************************************
        'Description: Result returned by usernames or screennames request
        '
        'Parameters: Action - operation name, ReturnValue - returned data
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim sKey As String = Replace(sScreenName, " ", "_")
            sKey = Replace(sKey, "-", "_")
            sKey = Replace(sKey, "/", "_")
            sKey = Replace(sKey, "\", "_")
            sKey = "cs" & Trim(sKey).ToUpper
            Dim sDisplayName As String = String.Empty
            sDisplayName = gcsRM.GetString(sKey)
            If sDisplayName = String.Empty Then
                sDisplayName = sScreenName
            End If
            Return (sDisplayName)
        Catch ex As Exception
            Return (sScreenName)
        End Try
    End Function
    Friend Function sMakeValidFileName(ByRef sFileName As String) As String
        '*****************************************************************************************
        'Description: strip out invalid filename characters
        '
        'Parameters: 
        'Returns:   

        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/30/12  MSW     Add a routine to make a legal filename by stripping out punctuation
        '*****************************************************************************************
        Dim sTmpFile As String = sFileName.Replace("#", "_").Trim
        sTmpFile = sTmpFile.Replace("%", "_")
        sTmpFile = sTmpFile.Replace("&", "_")
        sTmpFile = sTmpFile.Replace("*", "_")
        sTmpFile = sTmpFile.Replace("|", "_")
        sTmpFile = sTmpFile.Replace("\", "_")
        sTmpFile = sTmpFile.Replace(":", "_")
        sTmpFile = sTmpFile.Replace("""", "_")
        sTmpFile = sTmpFile.Replace("<", "_")
        sTmpFile = sTmpFile.Replace(">", "_")
        sTmpFile = sTmpFile.Replace("?", "_")
        Return (sTmpFile.Replace("\", "_"))
    End Function
    Friend Function MergeArrays(ByVal sArray1() As String, ByVal sArray2() As String) As Array
        '*****************************************************************************************
        'Description: Merge 2 Arrays Routine
        '
        'Parameters: 
        'Returns:   One big array.  The order the arrays are passed in is important.  Array1 fills
        '           in first then Array2.  
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/07/11  MSW     move to common routines
        '*****************************************************************************************
        Dim i As Integer
        Dim nUB As Integer
        Dim msTempArray() As String

        If Not (sArray2 Is Nothing) Then
            nUB = UBound(sArray1) + UBound(sArray2) + 1
            ReDim msTempArray(nUB)
        Else
            nUB = UBound(sArray1)
            ReDim msTempArray(nUB)
        End If
        For i = 0 To nUB
            If i > UBound(sArray1) Then
                msTempArray(i) = sArray2(i - UBound(sArray1) - 1)
            Else
                msTempArray(i) = sArray1(i)
            End If
        Next

        MergeArrays = msTempArray

    End Function
    Public Function sGetTmpFileName(ByVal sPath As String, ByVal sFile As String, ByVal sExt As String) As String
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

    Public Function sGetTmpFileName(ByVal sFile As String, Optional ByVal sExt As String = "") As String
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
        '********************************************************************************************
        Const sTEMP_FOLDER As String = "%TEMP%"
        'Get the temp path for command file outputs
        Dim sTempFileFolder As String = Environment.ExpandEnvironmentVariables(sTEMP_FOLDER)
        Dim sSuffix As String = String.Empty
        If sExt <> String.Empty Then
            If sExt.Substring(0, 1) = "." Then
                sSuffix = sExt
            Else
                sSuffix = "." & sExt
            End If
        End If
        Return sGetTmpFileName(sTempFileFolder, sFile, sSuffix)
    End Function
    Public Sub subShowChangeLog(ByRef sZone As String, ByRef sScreen As String, ByRef sScreenName As String, _
                                 ByRef sDevice As String, ByRef sParameter As String, ByVal ParameterType As eParamType, _
                                 ByVal PeriodIndex As Integer, ByVal bUseCntr As Boolean, ByVal bByArm As Boolean, _
                                 ByVal oIPC As Paintworks_IPC.clsInterProcessComm)
        '********************************************************************************************
        'Description:  launch the change log form
        '
        'Parameters: none
        'Returns:    none
        '  
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/16/13  MSW     Standalone ChangeLog                                            4.1.5.0
        '********************************************************************************************

        Try
        Dim oProcs() As Process = Process.GetProcessesByName("PW4_Main")

        If oProcs.Length > 0 Then
            ShowWindowAsync(oProcs(0).MainWindowHandle, 5) 'SW_SHOW
            Threading.Thread.Sleep(10)
            'ShowWindowAsync(hWnd, SW_RESTORE);
            ShowWindowAsync(oProcs(0).MainWindowHandle, 9) 'SW_RESTORE
            Threading.Thread.Sleep(10)
            SetForegroundWindow(oProcs(0).MainWindowHandle)
        End If

        Dim sPath As String = String.Empty
        Dim FileWriter As System.IO.StreamWriter
        mPWCommon.GetDefaultFilePath(sPath, eDir.IPC, String.Empty, String.Empty)
        sPath = sGetTmpFileName(sPath, "ShowChangeLog", ".ShowChangeLog")
        Application.DoEvents()
        FileWriter = New System.IO.StreamWriter(sPath)
        FileWriter.WriteLine("Zone=" & sZone)
        FileWriter.WriteLine("Screen=" & sScreen)
        FileWriter.WriteLine("ScreenName=" & sScreenName)
        FileWriter.WriteLine("Device=" & sDevice)
        FileWriter.WriteLine("Parameter=" & sParameter)
        FileWriter.WriteLine("ParameterType=" & CType(ParameterType, Integer).ToString)
        FileWriter.WriteLine("PeriodIndex=" & PeriodIndex.ToString)
        If bUseCntr Then
            If bByArm Then
                FileWriter.WriteLine("Robots")
            Else
                FileWriter.WriteLine("Controllers")
            End If
        Else
            FileWriter.WriteLine("NoRobots")
        End If
        FileWriter.Close()

        Application.DoEvents()
        Dim sLaunchCommand As String = String.Empty
        Try
            sLaunchCommand = gs_CHANGELOG_EXE & "," & sPath
            Call oIPC.WriteControlMsg("pw4_main", "launchapp", sLaunchCommand)
            Threading.Thread.Sleep(500)

            oProcs = Process.GetProcessesByName("ChangeLog")

            If oProcs.Length > 0 Then
                ShowWindowAsync(oProcs(0).MainWindowHandle, 5) 'SW_SHOW
                Threading.Thread.Sleep(10)
                'ShowWindowAsync(hWnd, SW_RESTORE);
                ShowWindowAsync(oProcs(0).MainWindowHandle, 9) 'SW_RESTORE
                Threading.Thread.Sleep(10)
                SetForegroundWindow(oProcs(0).MainWindowHandle)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, "Module: mPWCommon, Routine: subShowChangeLog", _
                                    String.Empty, MessageBoxButtons.OK)
        End Try
        Catch ex As Exception
            mDebug.WriteEventToLog(sScreen & "Module: mPWCommon, Routine: WritePasswordMsg", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
    Public Sub subLaunchHelp(ByRef sLink As String, ByVal oIPC As Paintworks_IPC.clsInterProcessComm, _
                             Optional ByVal bPWHelp As Boolean = True, _
                             Optional ByVal bOnlineManuals As Boolean = False)
        '********************************************************************************************
        'Description:  launch the PW Browser
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sLaunchCommand As String = String.Empty
        Try
            Dim oProcs() As Process = Process.GetProcessesByName("PW4_Main")

            If oProcs.Length > 0 Then
                ShowWindowAsync(oProcs(0).MainWindowHandle, 5) 'SW_SHOW
                Threading.Thread.Sleep(10)
                'ShowWindowAsync(hWnd, SW_RESTORE);
                ShowWindowAsync(oProcs(0).MainWindowHandle, 9) 'SW_RESTORE
                Threading.Thread.Sleep(10)
                SetForegroundWindow(oProcs(0).MainWindowHandle)
            End If
            'Get the path
            If bPWHelp Then
                sLaunchCommand = gs_HELP_EXE & "," & gs_HELP_PWHELP
            End If
            If bOnlineManuals Then
                sLaunchCommand = gs_HELP_EXE & "," & gs_HELP_MANUALS
            End If
            If sLink <> String.Empty Then
                sLaunchCommand = gs_HELP_EXE & "," & sLink
            End If

            Call oIPC.WriteControlMsg("pw4_main", "launchapp", sLaunchCommand)
            Threading.Thread.Sleep(500)

            oProcs = Process.GetProcessesByName("PWBrowser")

            If oProcs.Length > 0 Then
                ShowWindowAsync(oProcs(0).MainWindowHandle, 5) 'SW_SHOW
                Threading.Thread.Sleep(10)
                'ShowWindowAsync(hWnd, SW_RESTORE);
                ShowWindowAsync(oProcs(0).MainWindowHandle, 9) 'SW_RESTORE
                Threading.Thread.Sleep(10)
                SetForegroundWindow(oProcs(0).MainWindowHandle)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, "Module: mPWCommon, Routine: subLaunchHelp", _
                                  sLink, MessageBoxButtons.OK)
        End Try

    End Sub

    Friend Function LoadRobotBoxFromDB(ByRef cboRobot As ComboBox, ByVal Zone As clsZone, _
                            ByVal UseControllerName As Boolean, ByVal bAddAll As Boolean, _
                            Optional ByVal CCType1 As eColorChangeType = eColorChangeType.NOT_SELECTED, _
                            Optional ByVal CCType2 As eColorChangeType = eColorChangeType.NOT_SELECTED, _
                     Optional ByVal bIncludeOpeners As Boolean = True) As Boolean
        '********************************************************************************************
        ' Description:  Load Robot combo with arm names

        'Parameters: box to load, Desired zone, use arm or controller names, add all?
        'Returns: true if success

        'Modification(history)

        'Date      By      Reason
        '11/05/09  MSW     LoadRobotBoxFromDB - Add CC and opener conditions
        '10/20/11  MSW     Move to XML DB
        '********************************************************************************************    
        'get rid of old data if any
        cboRobot.Items.Clear()

        If bAddAll Then cboRobot.Items.Add(gcsRM.GetString("csALL"))

        Try

            Dim tArmArray() As tArm = GetArmArray(Zone)

            Dim nContNum As Integer = -1

            For i As Integer = 0 To tArmArray.GetUpperBound(0)
                Dim bAdd As Boolean = True
                If (CCType1 <> eColorChangeType.NOT_SELECTED) Then
                    Dim nCCType As eColorChangeType = CType(tArmArray(i).ColorChangeType, eColorChangeType)
                    If (CCType1 = eColorChangeType.NOT_NONE) Then
                        If nCCType = eColorChangeType.NONE Then
                            bAdd = False
                        End If
                    Else
                        If (nCCType <> CCType1) And (nCCType <> CCType2) Then
                            bAdd = False
                        End If
                    End If
                End If
                If Not (bIncludeOpeners) Then
                    bAdd = bAdd And Not tArmArray(i).IsOpener
                End If
                If bAdd Then
                    If tArmArray(i).Controller.ControllerNumber <> nContNum Then
                        ' new controller - it adds first arm
                        If UseControllerName = False Then
                            cboRobot.Items.Add(tArmArray(i).ArmDisplayName)
                        Else
                            cboRobot.Items.Add(tArmArray(i).Controller.DisplayName)
                        End If
                    Else
                        If UseControllerName = False Then
                            'additional arm on existing controller
                            cboRobot.Items.Add(tArmArray(i).ArmDisplayName)
                        End If
                    End If
                    nContNum = tArmArray(i).Controller.ControllerNumber
                End If
            Next

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try

    End Function
    Friend Function LoadRobotBoxFromDB(ByRef lstRobot As ListBox, ByVal Zone As clsZone, _
                            ByVal UseControllerName As Boolean, _
                            Optional ByVal CCType1 As eColorChangeType = eColorChangeType.NOT_SELECTED, _
                            Optional ByVal CCType2 As eColorChangeType = eColorChangeType.NOT_SELECTED, _
                     Optional ByVal bIncludeOpeners As Boolean = True) As Boolean
        '********************************************************************************************
        ' Description:  Load Robot combo with arm names

        'Parameters: box to load, Desired zone, use arm or controller names, add all?
        'Returns: true if success

        'Modification(history)

        'Date      By      Reason
        '12/27/10  MSW     LoadRobotBoxFromDB - Add listbox and checklistbox versions
        '10/20/11  MSW     Move to XML DB
        '********************************************************************************************    
        'get rid of old data if any
        lstRobot.Items.Clear()

        Try

            Dim tArmArray() As tArm = GetArmArray(Zone)

            Dim nContNum As Integer = -1

            For i As Integer = 0 To tArmArray.GetUpperBound(0)
                Dim bAdd As Boolean = True
                If (CCType1 <> eColorChangeType.NOT_SELECTED) Then
                    Dim nCCType As eColorChangeType = CType(tArmArray(i).ColorChangeType, eColorChangeType)
                    If (CCType1 = eColorChangeType.NOT_NONE) Then
                        If nCCType = eColorChangeType.NONE Then
                            bAdd = False
                        End If
                    Else
                        If (nCCType <> CCType1) And (nCCType <> CCType2) Then
                            bAdd = False
                        End If
                    End If
                End If
                If Not (bIncludeOpeners) Then
                    bAdd = bAdd And Not tArmArray(i).IsOpener
                End If
                If bAdd Then
                    If tArmArray(i).Controller.ControllerNumber <> nContNum Then
                        ' new controller - it adds first arm
                        If UseControllerName = False Then
                            lstRobot.Items.Add(tArmArray(i).ArmDisplayName)
                        Else
                            lstRobot.Items.Add(tArmArray(i).Controller.DisplayName)
                        End If
                    Else
                        If UseControllerName = False Then
                            'additional arm on existing controller
                            lstRobot.Items.Add(tArmArray(i).ArmDisplayName)
                        End If
                    End If
                    nContNum = tArmArray(i).Controller.ControllerNumber
                End If
            Next

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try

    End Function
    Friend Function LoadRobotBoxFromDB(ByRef chkRobot As CheckedListBox, ByVal Zone As clsZone, _
                            ByVal UseControllerName As Boolean, _
                            Optional ByVal CCType1 As eColorChangeType = eColorChangeType.NOT_SELECTED, _
                            Optional ByVal CCType2 As eColorChangeType = eColorChangeType.NOT_SELECTED, _
                     Optional ByVal bIncludeOpeners As Boolean = True) As Boolean
        '********************************************************************************************
        ' Description:  Load Robot combo with arm names

        'Parameters: box to load, Desired zone, use arm or controller names, add all?
        'Returns: true if success

        'Modification(history)

        'Date      By      Reason
        '12/27/10  MSW     LoadRobotBoxFromDB - Add listbox and checklistbox versions
        '10/20/11  MSW     Move to XML DB
        '********************************************************************************************    
        'get rid of old data if any
        chkRobot.Items.Clear()

        Try

            Dim tArmArray() As tArm = GetArmArray(Zone)

            Dim nContNum As Integer = -1

            For i As Integer = 0 To tArmArray.GetUpperBound(0)
                Dim bAdd As Boolean = True
                If (CCType1 <> eColorChangeType.NOT_SELECTED) Then
                    Dim nCCType As eColorChangeType = CType(tArmArray(i).ColorChangeType, eColorChangeType)
                    If (CCType1 = eColorChangeType.NOT_NONE) Then
                        If nCCType = eColorChangeType.NONE Then
                            bAdd = False
                        End If
                    Else
                        If (nCCType <> CCType1) And (nCCType <> CCType2) Then
                            bAdd = False
                        End If
                    End If
                End If
                If Not (bIncludeOpeners) Then
                    bAdd = bAdd And Not tArmArray(i).IsOpener
                End If
                If bAdd Then
                    If tArmArray(i).Controller.ControllerNumber <> nContNum Then
                        ' new controller - it adds first arm
                        If UseControllerName = False Then
                            chkRobot.Items.Add(tArmArray(i).ArmDisplayName)
                        Else
                            chkRobot.Items.Add(tArmArray(i).Controller.DisplayName)
                        End If
                    Else
                        If UseControllerName = False Then
                            'additional arm on existing controller
                            chkRobot.Items.Add(tArmArray(i).ArmDisplayName)
                        End If
                    End If
                    nContNum = tArmArray(i).Controller.ControllerNumber
                End If
            Next

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try

    End Function

    Friend Function LoadRobotBoxFromDB(ByRef chkRobot As CheckedListBox, ByVal Zones As clsZones, _
            ByVal UseControllerName As Boolean, _
            Optional ByVal CCType1 As eColorChangeType = eColorChangeType.NOT_SELECTED, _
            Optional ByVal CCType2 As eColorChangeType = eColorChangeType.NOT_SELECTED, _
            Optional ByVal bIncludeOpeners As Boolean = True) As Boolean
        '********************************************************************************************
        ' Description:  Load Robot combo with arm names

        'Parameters: box to load, Desired zone, use arm or controller names, add all?
        'Returns: true if success

        'Modification(history)

        'Date      By      Reason
        '12/27/10  MSW     LoadRobotBoxFromDB - Add listbox and checklistbox versions
        '10/20/11  MSW     Move to XML DB
        '11/15/12  HGB     Create a multizone version for SA
        '********************************************************************************************    
        'get rid of old data if any
        chkRobot.Items.Clear()

        Try
            For Each z As clsZone In Zones
                Dim tArmArray() As tArm = GetArmArray(z)

                Dim nContNum As Integer = -1

                For i As Integer = 0 To tArmArray.GetUpperBound(0)
                    Dim bAdd As Boolean = True
                    If (CCType1 <> eColorChangeType.NOT_SELECTED) Then
                        Dim nCCType As eColorChangeType = CType(tArmArray(i).ColorChangeType, eColorChangeType)
                        If (CCType1 = eColorChangeType.NOT_NONE) Then
                            If nCCType = eColorChangeType.NONE Then
                                bAdd = False
                            End If
                        Else
                            If (nCCType <> CCType1) And (nCCType <> CCType2) Then
                                bAdd = False
                            End If
                        End If
                    End If
                    If Not (bIncludeOpeners) Then
                        bAdd = bAdd And Not tArmArray(i).IsOpener
                    End If
                    If bAdd Then
                        If tArmArray(i).Controller.ControllerNumber <> nContNum Then
                            ' new controller - it adds first arm
                            If UseControllerName = False Then
                                chkRobot.Items.Add(tArmArray(i).ArmDisplayName)
                            Else
                                chkRobot.Items.Add(tArmArray(i).Controller.DisplayName)
                            End If
                        Else
                            If UseControllerName = False Then
                                'additional arm on existing controller
                                chkRobot.Items.Add(tArmArray(i).ArmDisplayName)
                            End If
                        End If
                        nContNum = tArmArray(i).Controller.ControllerNumber
                    End If
                Next
            Next
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try
    End Function


    Friend Function IPFromHostName(ByVal HostName As String) As String
        '********************************************************************************************
        'Description:  get an IP address from the host name
        '
        'Parameters: host name
        'Returns:    IP address
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/16/13  TSB/ASD Sometimes Dns.GetHostEntry(HostName) is not working. Added Try
        '                   Dns.GetHostByName(HostName) - the old method to handle this 
        ' 03/09/14  PINEAPPLE   Dns.GetHostEntry issue above needs to have established a network 
        '                   connection once prior to reading hosts file. If gui can ping itself,
        '                   network has been established. Only need to try once.
        ' 05/20/14  MSW     IPFromHostName - Don't return 0000 for the first disabled adapter  
        '********************************************************************************************    

        Try
            ' 03/09/14  PINEAPPLE
            Dim NetworkInfo() As NetworkInformation.NetworkInterface
            NetworkInfo = NetworkInformation.NetworkInterface.GetAllNetworkInterfaces

            For Each oNetworkInterface As NetworkInformation.NetworkInterface In NetworkInfo
                If oNetworkInterface.NetworkInterfaceType = NetworkInformation.NetworkInterfaceType.Ethernet Then
                    If oNetworkInterface.OperationalStatus = NetworkInformation.OperationalStatus.Up Then

                        Dim hostInfo As IPHostEntry = Dns.GetHostEntry(HostName)
                        ' should only be one in our case
                        Return hostInfo.AddressList(0).ToString
                    End If
                End If
            Next

            Return "0.0.0.0"

        Catch sex As Sockets.SocketException
            ' 03/09/14  PINEAPPLE
            ''ASD this dns is not working, going to the old method for a host name
            ''TSB Replaced Dns.GetHostEntry with Dns.GetHostByName 05/16/13
            'Try
            '    Dim hostInfo2 As IPHostEntry = Dns.GetHostEntry(HostName)

            '    Return hostInfo2.AddressList(0).ToString
            'Catch
                Trace.WriteLine("Module: mPWCommon, Routine: IPFromHostName, Error: " & sex.Message)
                Debug.Print(gcsRM.GetString("csHOST_ENTRY_ERR"))
                Debug.Print(gcsRM.GetString("csPAINTWORKS"))
                Debug.Print(HostName)
                Debug.Print(sex.Message)

                MessageBox.Show(gcsRM.GetString("csHOST_ENTRY_ERR") & " " & HostName & vbCrLf & _
                                                sex.Message, gcsRM.GetString("csPAINTWORKS"), MessageBoxButtons.OK, _
                                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, _
                                                MessageBoxOptions.DefaultDesktopOnly)
                Return "0.0.0.0"
            '        End Try

        Catch ex As Exception
            Trace.WriteLine("Module: mPWCommon, Routine: IPFromHostName, Error: " & ex.Message)
            Trace.WriteLine("Module: mPWCommon, Routine: IPFromHostName, StackTrace: " & ex.StackTrace)

            Return "0.0.0.0"
        End Try

    End Function

    Friend Function GetControlByName(ByVal Name As String, ByVal vForm As Form) As Control
        '****************************************************************************************
        'Description: 
        '
        'Parameters: 
        'Returns:  
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Const flags As System.Reflection.BindingFlags = _
                                    System.Reflection.BindingFlags.NonPublic Or _
                                    System.Reflection.BindingFlags.Instance Or _
                                    System.Reflection.BindingFlags.Public Or _
                                    System.Reflection.BindingFlags.IgnoreCase

        Dim info As System.Reflection.FieldInfo = vForm.GetType().GetField("_" & Name, flags)

        If info Is Nothing Then Return Nothing

        Dim o As Control = CType(info.GetValue(vForm), Control)

        Return o

    End Function
    Friend Function GetControlByName(ByVal Name As String, _
                                                    ByVal vUserControl As UserControl) As Control
        '****************************************************************************************
        'Description: 
        '
        'Parameters: 
        'Returns:  
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Const flags As System.Reflection.BindingFlags = _
                                    System.Reflection.BindingFlags.NonPublic Or _
                                    System.Reflection.BindingFlags.Instance Or _
                                    System.Reflection.BindingFlags.Public Or _
                                    System.Reflection.BindingFlags.IgnoreCase

        Dim info As System.Reflection.FieldInfo = vUserControl.GetType().GetField("_" & Name, flags)

        If info Is Nothing Then Return Nothing

        Dim o As Control = CType(info.GetValue(vUserControl), Control)

        Return o

    End Function
    Friend Function GetDefaultFilePath(ByRef PathBuffer As String, ByVal DesiredDir As eDir, _
                            ByVal RemoteServerPath As String, ByVal FileName As String) As Boolean
        '****************************************************************************************
        'Description: This Function gets the path to the desired subdirectory working backwards 
        '              from where we started to avoid the hardcoded "C:\Paint" thing. Checks to 
        '               see that file is there if file name is not null
        '
        'Parameters: buffer for path, which dir, and Filename we are looking for - remote machine
        '               name if needed
        'Returns:   true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/27/13  MSW     GetDefaultFilePath - More standalone support.  Find executables and data files for 
		'                   App installed by the publish feature.
        '                   also make the path search more specific - "/Paint/" instead of "Paint"
        ' 04/03/14  MSW     ZDT Changes - GetDefaultFilePath - keep capitalization
        '*****************************************************************************************
        Dim sPath As String = String.Empty
        Dim nStart As Integer = 0
        Dim nLen As Integer = 0


        If msRemotePWpath <> String.Empty Then
            'this is a browsed to path by the user should be pointing to a paint folder somewhere
            'this is not the normal route
            If bLocateDirectory(PathBuffer, DesiredDir) Then
                'found folder
                If FileName = String.Empty Then Return True
                'check for file at bottom of routine
            Else
                Return False
            End If
        End If 'msRemotePWpath <> String.Empty

        If RemoteServerPath = String.Empty Then
            'Local machine path
			
            'ZDT Changes - GetDefaultFilePath - keep capitalization 
            sPath = StartupPath
            'determine the part of the path we started in that we want
            nStart = InStr(sPath, msPAINTDIRECTORY, CompareMethod.Text)

            'If nStart = 0 Then
            '    'not found
            '    nStart = InStr(sPath, msPAINTNETDIRECTORY.ToLower(mLanguage.FixedCulture))
            '    If nStart <> 0 Then
            '        nLen = (nStart + Len(msPAINTNETDIRECTORY)) - 1
            '        sPath = Left(sPath, nLen)
            '    End If
            'Else
            If nStart <> 0 Then
                'found
                nLen = (nStart + Len(msPAINTDIRECTORY))
                sPath = Left(sPath, nLen - 2)
            End If
            'End If

            If nStart = 0 Then
                'not found
                nStart = InStr(sPath, msSEALERDIRECTORY, CompareMethod.Text)
                If nStart <> 0 Then
                    nLen = (nStart + Len(msSEALERDIRECTORY))
                    sPath = Left(sPath, nLen - 2)
                End If
            End If
            'If nStart = 0 Then
            '    'not found
            '    nStart = InStr(sPath, msSEALERNETDIRECTORY.ToLower(mLanguage.FixedCulture))
            '    If nStart <> 0 Then
            '        nLen = (nStart + Len(msSEALERNETDIRECTORY)) - 1
            '        sPath = Left(sPath, nLen)
            '    End If
            'End If

            If nStart = 0 Then
                If UsePaintWorksDB Then
                    'no paint dir found
                    Return False
                Else
                    Select Case DesiredDir
                        Case eDir.Database, eDir.XML
                            If sPath.Substring(sPath.Length - 3) = "bin" Then
                                sPath = Left(sPath, sPath.Length - 4)
                            Else
                                sPath = UserAppDataPath
                                If Strings.Right(sPath, 1) <> "\" Then sPath = sPath & "\"
                            End If
                        Case eDir.VBApps
                    End Select
                End If
            End If

        Else
            ' this is the paintshop computer selecting a remote zone remoteserverpath should come in 
            ' something like \\server\sharename that would point to a paint dir
            sPath = RemoteServerPath
        End If


        If bLocateDirectory(sPath, DesiredDir) Then
            'found folder
            PathBuffer = sPath
            If FileName = String.Empty Then Return True
            'check for file at bottom of routine
        Else
            Return False
        End If

        If UsePaintWorksDB = False Then
            Select Case DesiredDir
                Case eDir.Database, eDir.XML
                    PathBuffer = UserAppDataPath & FileName
                    If IO.File.Exists(PathBuffer) Then
                        Return True
                    Else
                        If Strings.Right(sPath, 1) <> "\" Then sPath = sPath & "\"
                        PathBuffer = sPath & FileName
                        If IO.File.Exists(PathBuffer) Then
                            Return True
                        Else
                            Return False
                        End If
                    End If
                Case Else

            End Select
        End If
        ' no filename - check directory
        If FileName = String.Empty Then
            Return True
        Else
            PathBuffer = PathBuffer & FileName
            'check if the file is there - this also fails if you don't have permission
            If IO.File.Exists(PathBuffer) Then
                Return True
            Else
                Return False
            End If
        End If  'FileName = String.Empty


    End Function
    Friend Function GetControllerArray(ByRef colZones As clsZones) As tController()
        '********************************************************************************************
        'Description:  Read settings from XML file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/30/11  MSW     Read from XML
        '********************************************************************************************
        Dim tControllerArray() As tController = Nothing
        For Each oZone As clsZone In colZones
            Dim tControllerArrayZone() As tController = GetControllerArray(oZone)
            Dim nStartSize As Integer = 0
            If tControllerArray IsNot Nothing Then
                nStartSize = tControllerArray.Length
            End If
            Dim nNewSize As Integer = nStartSize + tControllerArrayZone.Length - 1
            ReDim Preserve tControllerArray(nNewSize)
            For nIndex As Integer = nStartSize To nNewSize
                tControllerArray(nIndex) = tControllerArrayZone(nIndex - nStartSize)
            Next
        Next
        Return tControllerArray
    End Function


    Friend Function GetControllerArray(ByRef Zone As clsZone) As tController()
        '********************************************************************************************
        'Description:  Read settings from XML file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/30/11  MSW     Read from XML
        ' 11/03/11  MSW     Support optional path for auto backups
        '********************************************************************************************
        Const sCNTR_XMLTABLE As String = "Controllers"
        Dim sCNTR_XMLZONE As String = Zone.Name
        Const sCNTR_XMLNODE As String = "Controller"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Dim tControllerArray() As tController = Nothing
        Try
            If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sCNTR_XMLTABLE & ".XML") Then


                oXMLDoc.Load(sXMLFilePath)

                oMainNode = oXMLDoc.SelectSingleNode("//" & sCNTR_XMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & sCNTR_XMLZONE & "//" & sCNTR_XMLNODE)
                Try
                    If oNodeList.Count = 0 Then
                        mDebug.WriteEventToLog("pwCommon:GetControllerArray", sXMLFilePath & " not found.")
                    Else
                        Dim nMax As Integer = oNodeList.Count - 1
                        ReDim tControllerArray(nMax)
                        For nNode As Integer = 0 To nMax
                            Try
                                tControllerArray(nNode).DisplayName = oNodeList(nNode).Item("DisplayName").InnerXml
                                tControllerArray(nNode).HostName = oNodeList(nNode).Item("HostName").InnerXml
                                tControllerArray(nNode).ControllerNumber = CType(oNodeList(nNode).Item("ControllerNumber").InnerXml, Integer)
                                tControllerArray(nNode).SortOrder = CType(oNodeList(nNode).Item("SortOrder").InnerXml, Integer)
                                tControllerArray(nNode).ControllerType = CType(oNodeList(nNode).Item("ControllerType").InnerXml, Integer)
                                Try
                                    tControllerArray(nNode).InternalBackup = CType(oNodeList(nNode).Item("InternalBackup").InnerXml, Boolean)
                                Catch ex As Exception
                                    tControllerArray(nNode).InternalBackup = False
                                    ShowErrorMessagebox(sXMLFilePath & "  : Invalid InternalBackup", ex, "mPWCommon:GetControllerArray", String.Empty)
                                End Try
                                Try
                                    tControllerArray(nNode).BackupPath = oNodeList(nNode).Item("BackupPath").InnerXml
                                Catch ex As Exception
                                    tControllerArray(nNode).BackupPath = "MD:"
                                    mDebug.WriteEventToLog("mPWCommon:GetControllerArray - Invalid BackupPath", ex.Message)
                                End Try
                                Try
                                    tControllerArray(nNode).HotEditLoggerDO = CType(oNodeList(nNode).Item("HotEditLoggerDO").InnerXml, Integer)
                                Catch ex As Exception
                                    tControllerArray(nNode).HotEditLoggerDO = 0
                                    mDebug.WriteEventToLog("mPWCommon:GetControllerArray - Invalid HotEditLoggerDO", ex.Message)
                                End Try
                                Try
                                    tControllerArray(nNode).VisionController = CType(oNodeList(nNode).Item("VisionController").InnerXml, Boolean)
                                Catch ex As Exception
                                    tControllerArray(nNode).VisionController = False
                                    mDebug.WriteEventToLog("mPWCommon:GetControllerArray - Invalid VisionController", ex.Message)
                                End Try
                            Catch ex As Exception
                                ShowErrorMessagebox(sXMLFilePath & "  : Invalid Data", ex, "mPWCommon:GetControllerArray", String.Empty)
                            End Try
                        Next
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog("pwCommon:GetControllerArray", "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                End Try
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog("pwCommon:GetControllerArray", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
        End Try
        Return tControllerArray
    End Function
    Friend Function GetArmArray(ByRef colZones As clsZones, Optional ByRef oControllers() As tController = Nothing) As tArm()
        '********************************************************************************************
        'Description:  Read settings from XML file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/30/11  MSW     Read from XML
        '********************************************************************************************
        Dim tArmArray() As tArm = Nothing
        For Each oZone As clsZone In colZones
            Dim tControllerArrayZone() As tController = Nothing
            Dim tArmArrayZone() As tArm = GetArmArray(oZone, tControllerArrayZone)
            'Arms
            Dim nStartSize As Integer = 0
            If tArmArray IsNot Nothing Then
                nStartSize = tArmArray.Length
            End If
            Dim nNewSize As Integer = nStartSize + tArmArrayZone.Length - 1
            ReDim Preserve tArmArray(nNewSize)
            For nIndex As Integer = nStartSize To nNewSize
                tArmArray(nIndex) = tArmArrayZone(nIndex - nStartSize)
            Next
            'Controllers
            nStartSize = 0
            If oControllers IsNot Nothing Then
                nStartSize = oControllers.Length
            End If
            nNewSize = nStartSize + tControllerArrayZone.Length - 1
            ReDim Preserve oControllers(nNewSize)
            For nIndex As Integer = nStartSize To nNewSize
                oControllers(nIndex) = tControllerArrayZone(nIndex - nStartSize)
            Next
        Next
        Return tArmArray
    End Function
    Friend Function GetArmArray(ByRef Zone As clsZone, Optional ByRef oControllers() As tController = Nothing) As tArm()
        '********************************************************************************************
        'Description:  Read settings from XML file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/30/11  MSW     Read from XML
        ' 03/18/12  MSW     scattered access setup
        ' 02/18/14  MSW     GetArmArray - Add error handling to station number
        ' 03/24/14  DJM     added HasManualCycles for sealer applications
        '********************************************************************************************
        Const sARM_XMLTABLE As String = "Arms"
        Dim sARM_XMLZONE As String = Zone.Name
        Const sARM_XMLNODE As String = "Arm"


        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Dim tArmArray() As tArm = Nothing
        Try
            oControllers = GetControllerArray(Zone)
            If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sARM_XMLTABLE & ".XML") Then


                oXMLDoc.Load(sXMLFilePath)

                oMainNode = oXMLDoc.SelectSingleNode("//" & sARM_XMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & sARM_XMLZONE & "//" & sARM_XMLNODE)

                Try
                    If oNodeList.Count = 0 Then
                        mDebug.WriteEventToLog("pwCommon:GetArmArray", sXMLFilePath & " not found.")
                    Else
                        Dim nMax As Integer = oNodeList.Count - 1
                        ReDim tArmArray(nMax)
                        For nNode As Integer = 0 To nMax
                            Try
                                tArmArray(nNode).ArmDisplayName = oNodeList(nNode).Item("ArmDisplayName").InnerXml
                                tArmArray(nNode).RobotNumber = CType(oNodeList(nNode).Item("RobotNumber").InnerXml, Integer)
                                tArmArray(nNode).PigSystems = CType(oNodeList(nNode).Item("PigSystems").InnerXml, Integer) 'NRU 161215 Piggable
                                tArmArray(nNode).ArmSortOrder = CType(oNodeList(nNode).Item("ArmSortOrder").InnerXml, Integer)
                                tArmArray(nNode).ArmControllerNumber = CType(oNodeList(nNode).Item("ArmControllerNumber").InnerXml, Integer)
                                tArmArray(nNode).ArmNumber = CType(oNodeList(nNode).Item("ArmNumber").InnerXml, Integer)
                                tArmArray(nNode).ArmType = CType(oNodeList(nNode).Item("ArmType").InnerXml, Integer)
                                Try
                                    If Strings.InStr(oNodeList(nNode).InnerXml, "StationNumber", CompareMethod.Text) > 0 Then
                                tArmArray(nNode).StationNumber = CType(oNodeList(nNode).Item("StationNumber").InnerXml, Integer)
                                    Else
                                        tArmArray(nNode).StationNumber = 1
                                    End If
                                Catch ex As Exception
                                    tArmArray(nNode).StationNumber = 1
                                    ShowErrorMessagebox(sXMLFilePath & "  : Invalid StationNumber", ex, "mPWCommon:GetArmArray", String.Empty)
                                End Try
                                Try
                                    If Strings.InStr(oNodeList(nNode).InnerXml, "ColorChangeType", CompareMethod.Text) > 0 Then
                                    tArmArray(nNode).ColorChangeType = CType(oNodeList(nNode).Item("ColorChangeType").InnerXml, Integer)
                                    Else
                                        tArmArray(nNode).ColorChangeType = 0
                                    End If
                                Catch ex As Exception
                                    tArmArray(nNode).ColorChangeType = 0
                                    ShowErrorMessagebox(sXMLFilePath & "  : Invalid ColorChangeType", ex, "mPWCommon:GetArmArray", String.Empty)
                                End Try
                                Try '04/11/12 RJO
                                    tArmArray(nNode).Dispensers = CType(oNodeList(nNode).Item("Dispensers").InnerXml, Integer)
                                Catch ex As Exception
                                    tArmArray(nNode).Dispensers = 0
                                    ShowErrorMessagebox(sXMLFilePath & "  : Invalid Dispenser Count", ex, "mPWCommon:GetArmArray", String.Empty)
                                End Try
                                Try '04/11/12 RJO
                                    tArmArray(nNode).DispenserType = CType(oNodeList(nNode).Item("DispenserType").InnerXml, Integer)
                                Catch ex As Exception
                                    tArmArray(nNode).DispenserType = 0
                                    ShowErrorMessagebox(sXMLFilePath & "  : Invalid DispenserType", ex, "mPWCommon:GetArmArray", String.Empty)
                                End Try
                                Try '12/03/13 MSW
                                    tArmArray(nNode).Materials = CType(oNodeList(nNode).Item("Materials").InnerXml, Integer)
                                Catch ex As Exception
                                    tArmArray(nNode).Materials = 1
                                    ShowErrorMessagebox(sXMLFilePath & "  : Invalid Material Count", ex, "mPWCommon:GetArmArray", String.Empty)
                                End Try
                                Try
                                    tArmArray(nNode).EstatType = CType(oNodeList(nNode).Item("EstatType").InnerXml, Integer)
                                Catch ex As Exception
                                    tArmArray(nNode).EstatType = 0
                                    ShowErrorMessagebox(sXMLFilePath & "  : Invalid EstatType", ex, "mPWCommon:GetArmArray", String.Empty)
                                End Try
                                Try
                                    tArmArray(nNode).EstatHostName = oNodeList(nNode).Item("EstatHostName").InnerXml
                                Catch ex As Exception
                                    tArmArray(nNode).EstatHostName = String.Empty
                                    ShowErrorMessagebox(sXMLFilePath & "  : Invalid EstatHostName", ex, "mPWCommon:GetArmArray", String.Empty)
                                End Try
                                Try
                                    tArmArray(nNode).IsOpener = CType(oNodeList(nNode).Item("IsOpener").InnerXml, Boolean)
                                Catch ex As Exception
                                    tArmArray(nNode).IsOpener = False
                                    ShowErrorMessagebox(sXMLFilePath & "  : Invalid IsOpener", ex, "mPWCommon:GetArmArray", String.Empty)
                                End Try
                                Try
                                    tArmArray(nNode).PLCTagPrefix = oNodeList(nNode).Item("PLCTagPrefix").InnerXml
                                Catch ex As Exception
                                    tArmArray(nNode).PLCTagPrefix = tArmArray(nNode).ArmDisplayName
                                    ShowErrorMessagebox(sXMLFilePath & "  : Invalid PLCTagPrefix", ex, "mPWCommon:GetArmArray", String.Empty)
                                End Try
                                Try
                                    tArmArray(nNode).PaintToolAlias = oNodeList(nNode).Item("PaintToolAlias").InnerXml
                                Catch ex As Exception
                                    tArmArray(nNode).PaintToolAlias = tArmArray(nNode).ArmDisplayName
                                    ShowErrorMessagebox(sXMLFilePath & "  : Invalid PaintToolAlias", ex, "mPWCommon:GetArmArray", String.Empty)
                                End Try
                                Try
                                    If Strings.InStr(oNodeList(nNode).InnerXml, "HasManualCycles", CompareMethod.Text) > 0 Then
                                    tArmArray(nNode).HasManualCycles = CType(oNodeList(nNode).Item("HasManualCycles").InnerXml, Boolean)
                                    Else
                                        tArmArray(nNode).HasManualCycles = False
                                    End If
                                Catch ex As Exception
                                    tArmArray(nNode).HasManualCycles = False
                                End Try
                                Try
                                    tArmArray(nNode).ScatteredAccessGroups = oNodeList(nNode).Item("ScatteredAccessGroups").InnerXml
                                Catch ex As Exception
                                    tArmArray(nNode).ScatteredAccessGroups = "PainterOpenerCommon"
                                    If tArmArray(nNode).IsOpener = False Then
                                        tArmArray(nNode).ScatteredAccessGroups = "PainterOpenerCommon;PainterCommon;ColorChange;VersaBell"
                                    End If
                                    ShowErrorMessagebox(sXMLFilePath & "  : Invalid ScatteredAccessGroups", ex, "mPWCommon:GetArmArray", String.Empty)
                                End Try
                                For Each oController As tController In oControllers
                                    If tArmArray(nNode).ArmControllerNumber = oController.ControllerNumber Then
                                        tArmArray(nNode).Controller = oController
                                        Exit For
                                    End If
                                Next
                            Catch ex As Exception
                                ShowErrorMessagebox(sXMLFilePath & "  : Invalid Data", ex, "mPWCommon:GetArmArray", String.Empty)
                            End Try
                        Next
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog("pwCommon:GetArmArray", "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                End Try
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog("pwCommon:GetArmArray", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
        End Try
        Return tArmArray
    End Function
    Private Function bLocateDirectory(ByRef PathBuffer As String, _
                                                        ByVal DesiredDir As eDir) As Boolean
        '*****************************************************************************************
        'Description: check for desired directory and modify path buffer - split from
        '               GetDefaultFilePath 6/29/07
        '
        'Parameters: 
        'Returns:   true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/18/12  MSW     Add a notepad folder
		' 02/27/13  msw     support for standalone install w/o DB
		' 04/16/13  MSW     bLocateDirectory - Don't create DMON Viewer folder automatically, it's not used anymore
        ' 03/18/14  MSW     ZDT Changes
        ' 04/03/14  MSW     bLocateDirectory - Handle DB without zone subfolder
        ' 07/30/14  MSW     Add Reports folder, changelog error handling
        '*****************************************************************************************
        Dim sPath As String = PathBuffer
        Dim sPathTmp As String = String.Empty
        'double check

        If Strings.Right(sPath, 1) <> "\" Then sPath = sPath & "\"

        Select Case DesiredDir
            Case eDir.Database, eDir.XML, eDir.WeeklyReports, eDir.ECBR, eDir.DBBackup, eDir.Notepad
                If UsePaintWorksDB Then
                    sPathTmp = sPath + "Database\"
                Else
                    Return True
                End If
                If IO.Directory.Exists(sPathTmp) Then
                    'is it here, or under a mod name
                    Dim sD() As String = IO.Directory.GetDirectories(sPathTmp)
                    Select Case UBound(sD)
                        Case -1
                            'no subdir
                            Dim sF() As String = IO.Directory.GetFiles(sPathTmp)
                            If UBound(sF) = -1 Then
                                'dead end
                                Return False
                            Else
                                'assume the files are here
                                PathBuffer = sPathTmp
                            End If
                        Case 0
                            'Just 1 - if its c:\paint\database\xml then not using mod name
                            If InStr(sD(0), "xml", CompareMethod.Text) = 0 Then
                                sPathTmp = sD(0)
                            End If

                            Dim sF() As String = IO.Directory.GetFiles(sPathTmp)
                            If UBound(sF) = -1 Then
                                'dead end
                                Return False
                            Else
                                'assume the files are here
                                PathBuffer = sPathTmp
                            End If
                        Case Else
                            'more than 1, make user pick - this should only occur for the
                            'developer anyway so no validity checking on returned path...
                            If msDBPathCache = String.Empty Then
								' Handle DB without zone subfolder
                                Dim bAsk As Boolean = True
                                For Each sTmp As String In sD
                                    If sTmp.ToLower.Contains("xml") Then
                                        bAsk = False
                                        PathBuffer = sPathTmp
                                    End If
                                Next
                                If bAsk Then
                                    Dim oFB As New FolderBrowserDialog
                                    Dim sRet As String = String.Empty

                                    With oFB
                                        .ShowNewFolderButton = False
                                        .Description = gcsRM.GetString("csSELECT_PAINTDB_FOLDER")
                                        .SelectedPath = sPathTmp
                                        .ShowDialog()
                                        sRet = .SelectedPath
                                    End With

                                    If sRet <> String.Empty Then
                                        Dim sF() As String = IO.Directory.GetFiles(sRet)
                                        If UBound(sF) > 1 Then
                                            If Strings.Right(sRet, 1) <> "\" Then sRet = sRet & "\"
                                            PathBuffer = sRet

                                        End If
                                    Else
                                        Return False
                                    End If
                                End If

                                'todo: this probably needs to change when switching machines
                                msDBPathCache = PathBuffer
                            Else
                                'this is so you dont have to keep picking
                                PathBuffer = msDBPathCache
                            End If



                    End Select

                    'xml is now a subdir of database
                    If DesiredDir = eDir.XML Then
                        If Strings.Right(PathBuffer, 1) <> "\" Then PathBuffer = PathBuffer & "\"
                        PathBuffer = PathBuffer + "XML\"
                        If Not IO.Directory.Exists(PathBuffer) Then
                            IO.Directory.CreateDirectory(PathBuffer)
                        End If
                    End If

                    'weekly reports is a subdir of database
                    If DesiredDir = eDir.WeeklyReports Then
                        If Strings.Right(PathBuffer, 1) <> "\" Then PathBuffer = PathBuffer & "\"
                        PathBuffer = PathBuffer + "WeeklyReports\"
                        If Not IO.Directory.Exists(PathBuffer) Then
                            IO.Directory.CreateDirectory(PathBuffer)
                        End If
                    End If

                    'ECBR is now a subdir of database
                    If DesiredDir = eDir.ECBR Then
                        If Strings.Right(PathBuffer, 1) <> "\" Then PathBuffer = PathBuffer & "\"
                        PathBuffer = PathBuffer + "ECBR\"
                        If Not IO.Directory.Exists(PathBuffer) Then
                            IO.Directory.CreateDirectory(PathBuffer)
                        End If
                    End If

                    'backup is now a subdir of database
                    If DesiredDir = eDir.DBBackup Then
                        If Strings.Right(PathBuffer, 1) <> "\" Then PathBuffer = PathBuffer & "\"
                        PathBuffer = PathBuffer + "Backup\"
                        If Not IO.Directory.Exists(PathBuffer) Then
                            IO.Directory.CreateDirectory(PathBuffer)
                        End If
                    End If

                    'Notepad is a subdir of database
                    If DesiredDir = eDir.Notepad Then
                        If Strings.Right(PathBuffer, 1) <> "\" Then PathBuffer = PathBuffer & "\"
                        PathBuffer = PathBuffer + "Notepad\"
                        If Not IO.Directory.Exists(PathBuffer) Then
                            IO.Directory.CreateDirectory(PathBuffer)
                        End If
                    End If
                Else
                    'you may be here if you don't have permission IO.Directory.Exists doesn't check
                    Try
                        Dim dInfo As DirectoryInfo = IO.Directory.GetParent(sPath)
                        ' Get a DirectorySecurity object that represents the 
                        ' current security settings.
                        Dim dSecurity As System.Security.AccessControl.DirectorySecurity = _
                                                                dInfo.GetAccessControl()

                    Catch x As UnauthorizedAccessException
                        MessageBox.Show(gcsRM.GetString("csUNAUTH_ACCESS") & vbCrLf & sPath, _
                                            gcsRM.GetString("csERROR"), MessageBoxButtons.OK, _
                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, _
                                            MessageBoxOptions.DefaultDesktopOnly)
                        Return False

                    Catch x1 As InvalidOperationException
                        Dim sTmp As String = sPath & vbCrLf & gcsRM.GetString("csPATHACCESS_ERROR") & _
                                       vbCrLf & x1.Message
                        MessageBox.Show(sTmp, gcsRM.GetString("csERROR"), _
                                             MessageBoxButtons.OK, MessageBoxIcon.Warning, _
                                            MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)

                        Return False

                    Catch ex As Exception
                        Trace.WriteLine(ex.Message)
                        Return False
                    End Try


                    Return False
                End If  'IO.Directory.Exists(sPathTmp)

            Case eDir.VBApps, eDir.VB6Apps, eDir.IPC, eDir.ChangeLogger
                'Removing the double vbapps folders
                If UsePaintWorksDB Then
                    PathBuffer = sPath + "VBApps\"
                Else
                    PathBuffer = sPath
                End If
                If Not IO.Directory.Exists(PathBuffer) Then
                    IO.Directory.CreateDirectory(PathBuffer)
                End If
                'xml is now a subdir of database
                If DesiredDir = eDir.IPC Then
                    If Strings.Right(PathBuffer, 1) <> "\" Then PathBuffer = PathBuffer & "\"
                    PathBuffer = PathBuffer + "IPC\"
                    If Not IO.Directory.Exists(PathBuffer) Then
                        IO.Directory.CreateDirectory(PathBuffer)
                    End If
                ElseIf DesiredDir = eDir.ChangeLogger Then
                    If Strings.Right(PathBuffer, 1) <> "\" Then PathBuffer = PathBuffer & "\"
                    PathBuffer = PathBuffer + "ChangeLogger\"
                    If Not IO.Directory.Exists(PathBuffer) Then
                        IO.Directory.CreateDirectory(PathBuffer)
                    End If
                End If

            Case eDir.Help
                PathBuffer = sPath + "Help\"
                If Not IO.Directory.Exists(PathBuffer) Then
                    Return False
                End If
           '    07/30/14   MSW     Add Reports folder, changelog error handling
           Case eDir.Reports
                PathBuffer = sPath + "Reports\"
                If Not IO.Directory.Exists(PathBuffer) Then
                    Return False
                End If

             Case eDir.FanucManuals
                PathBuffer = sPath + "Help\FANUCManuals\"
                If Not IO.Directory.Exists(PathBuffer) Then
                    Return False
                End If

            Case eDir.Source
                PathBuffer = sPath + "Source\"
                If Not IO.Directory.Exists(PathBuffer) Then
                    Return False
                End If

            Case eDir.DmonViewer
                PathBuffer = sPath + "Dmon Viewer\"
                If Not IO.Directory.Exists(PathBuffer) Then
                    ' IO.Directory.CreateDirectory(PathBuffer)
                    PathBuffer = string.empty
					return false 
			    End If

            Case eDir.Profile
                PathBuffer = sPath + "Profile\"
                If Not IO.Directory.Exists(PathBuffer) Then
                    IO.Directory.CreateDirectory(PathBuffer)
                End If

            Case eDir.ScreenDumps
                'assuming the "Screen Dumps" folder name is a constant
                PathBuffer = sPath + "Screen Dumps\"
                'if it's not there, then create it
                If Not IO.Directory.Exists(PathBuffer) Then
                    IO.Directory.CreateDirectory(PathBuffer)
                End If

            Case eDir.MasterBackups
                PathBuffer = sPath + "Master Backups\"
                'if it's not there, then create it
                If Not IO.Directory.Exists(PathBuffer) Then
                    IO.Directory.CreateDirectory(PathBuffer)
                End If

            Case eDir.PWRobotBackups
                PathBuffer = sPath + "PW Robot Backups\"
                'if it's not there, then create it
                If Not IO.Directory.Exists(PathBuffer) Then
                    IO.Directory.CreateDirectory(PathBuffer)
                End If

            Case eDir.RobotImageBackups
                PathBuffer = sPath + "Robot Image Backups\"
                'if it's not there, then create it
                If Not IO.Directory.Exists(PathBuffer) Then
                    IO.Directory.CreateDirectory(PathBuffer)
                End If

            Case eDir.DmonData
                PathBuffer = sPath + "DMON Data\"
                'if it's not there, then create it
                If Not IO.Directory.Exists(PathBuffer) Then
                    IO.Directory.CreateDirectory(PathBuffer)
                End If

                ' 03/18/14  MSW     ZDT Changes
            Case eDir.ZDT
                PathBuffer = sPath + "ZDT\"
                'if it's not there, then create it
                If Not IO.Directory.Exists(PathBuffer) Then
                    IO.Directory.CreateDirectory(PathBuffer)
                End If

            Case eDir.DmonArchive
                PathBuffer = sPath + "DMON Data Archive\"
                'if it's not there, then create it
                If Not IO.Directory.Exists(PathBuffer) Then
                    IO.Directory.CreateDirectory(PathBuffer)
                End If

            Case eDir.TempBackups
                PathBuffer = sPath + "Temp Backups\"
                'if it's not there, then create it
                If Not IO.Directory.Exists(PathBuffer) Then
                    IO.Directory.CreateDirectory(PathBuffer)
                End If

            Case eDir.PAINTworks
                PathBuffer = sPath

            Case Else
                Return False

        End Select  'DesiredDir

        'double check
        If Strings.Right(PathBuffer, 1) <> "\" Then PathBuffer = PathBuffer & "\"

        Return True

    End Function
    Friend Sub AddChangeRecordToCollection(ByVal Area As String, ByVal User As String, _
                ByVal Zone As Integer, ByVal Device As String, ByVal Parm1 As String, _
                ByVal Specific As String, ByVal ZoneName As String, _
                Optional ByRef sForceTimeStamp As String = "", Optional ByVal bSaveAccess As Boolean = True)
        '*****************************************************************************************
        'Description: Add changes to collection to be saved to change database
        '
        'Parameters: 
        'Returns:   true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '02/28/07   gks     Parm1 cannot be string.empty or DB barks
        '09/18/07   gks     add zonename
        '*****************************************************************************************

        Dim tC As tChange

        If Parm1 = String.Empty Then
            Parm1 = gcsRM.GetString("csNO_PARAMETER")
        End If
        If User = String.Empty Then
            User = gcsRM.GetString("csNO_USER")
        End If
        With tC
            .Area = Area
            .Device = Device
            .Parm1 = Parm1
            If sForceTimeStamp <> "" Then
                Try
                    .StartSerial = CType(sForceTimeStamp, Date)
                Catch ex As Exception
                    .StartSerial = System.DateTime.Now
                End Try
            Else
                .StartSerial = System.DateTime.Now
            End If
            .User = User
            .Zone = Zone
            .Specific = Specific
            .ZoneName = ZoneName
        End With
        If bSaveAccess Then
            If colChanges Is Nothing Then
                colChanges = New Collection(Of tChange)
            End If

            colChanges.Add(tC)
        End If

        ' New for sql server - remove above eventually
        Dim tC1 As tChange

        With tC1
            .Area = tC.Area
            .Device = tC.Device
            .Parm1 = tC.Parm1
            .StartSerial = tC.StartSerial
            .User = tC.User
            .Zone = tC.Zone
            .Specific = tC.Specific
            .ZoneName = tC.ZoneName
        End With

        If colChangesSQL Is Nothing Then
            colChangesSQL = New Collection(Of tChange)
        End If

        colChangesSQL.Add(tC1)

    End Sub
    Friend Sub AddToStatusBox(ByRef oListbox As ListBox, ByVal sTextToAdd As String)
        '*****************************************************************************************
        'Description: put here for consistancy
        '
        'Parameters: 
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        If oListbox Is Nothing Then Exit Sub
        Dim sTmp As String

        If gbUseWatch Then
            Dim fTmp As Double = oWatch.ElapsedMilliseconds / 1000
            sTmp = sTextToAdd & vbTab & Format(fTmp, "0.000")
        Else
            sTmp = sTextToAdd & vbTab & Format(Now, "Long time")
        End If

        If sTextToAdd = String.Empty Then sTmp = String.Empty
        oListbox.Items.Insert(0, sTmp)
        If oListbox.Visible Then oListbox.Refresh()

    End Sub
 
    Friend Function SaveToChangeLog(ByRef oZone As clsZone, Optional ByRef oCursor As System.Windows.Forms.Cursor = Nothing) As Boolean
        '*****************************************************************************************
        'Description: Save collection to CSV
        '
        'Parameters: 
        'Returns:   true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/07/12  MSW     SaveToChangeLog - Change cursor and call doevents to deal with slow SQL access
        ' 10/01/12  RJO     XML frowns on strings that contain "<" or ">".
        ' 11/15/12  HGB     XML also doesn't like "&" and ";"
        ' 05/02/13  MSW     SaveToChangeLog - save the date in American, not Canadian
        ' 08/15/13  MSW     PSC remote PC support
        ' 08/30/13  MSW     SaveToChangeLog - Error Handling
        ' 07/30/14  MSW     Add Reports folder, changelog error handling
        '*****************************************************************************************
        If colChangesSQL Is Nothing Then Return True
        If colChangesSQL.Count = 0 Then Return True
        Try
            oCursor = Cursors.WaitCursor
        Catch
        End Try
        Dim sPath As String = String.Empty
        ' 08/15/13  MSW     PSC remote PC support
        If oZone.IsRemoteZone Then
            mPWCommon.GetDefaultFilePath(sPath, eDir.ChangeLogger, oZone.RemotePath, String.Empty)
        Else
            mPWCommon.GetDefaultFilePath(sPath, eDir.ChangeLogger, String.Empty, String.Empty)
        End If
        Const sXMLFILE As String = "ChangeLog"
        Const sXMLTABLE As String = "ChangeLog"
        Const sXMLNODE As String = "ChangeItem"
        sPath = sGetTmpFileName(sPath, sXMLFILE, ".XML")

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Try
            oXMLDoc = New XmlDocument
            oMainNode = oXMLDoc.CreateElement(sXMLTABLE)
            oXMLDoc.AppendChild(oMainNode)
            oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)

            For Each tC As tChange In colChangesSQL
                'Build XML node
                Dim oNode As XmlNode = oXMLDoc.CreateElement(sXMLNODE)

                Dim oAreaNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsCHANGE_AREA, Nothing)
                oAreaNode.InnerXml = tC.Area.ToString
                oNode.AppendChild(oAreaNode)

                Dim oDevNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsCHANGE_DEVICE, Nothing)
                If tC.Device <> String.Empty Then
                    oDevNode.InnerXml = tC.Device.ToString
                Else
                    oDevNode.InnerXml = String.Empty
                End If
                oNode.AppendChild(oDevNode)

                Dim oParmNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsCHANGE_PARAM, Nothing)
                oParmNode.InnerXml = tC.Parm1.ToString
                oNode.AppendChild(oParmNode)

                Dim oSpecNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsCHANGE_DESC, Nothing)
                '    07/30/14   MSW     Add Reports folder, changelog error handling
                Dim sDesc As String = String.Empty
                If tC.Specific Is Nothing Then
                    sDesc = ""
                Else
                    sDesc = tC.Specific.ToString 'RJO 10/01/12
                End If
				'MSW 8/30/13 - shouldn't have this, but handle it better anyway
                If sDesc IsNot Nothing AndAlso (sDesc <> String.Empty) Then
                    sDesc = Strings.Replace(sDesc, "<", "(")
                    sDesc = Strings.Replace(sDesc, ">", ")")
                    'HGB - Trap for other invalid chars
                    sDesc = Strings.Replace(sDesc, "&", "and")
                    sDesc = Strings.Replace(sDesc, ";", ":")
                End If
                oSpecNode.InnerXml = sDesc 'tC.Specific.ToString 'RJO 10/01/12
                oNode.AppendChild(oSpecNode)

                Dim oStartNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsCHANGE_TIME, Nothing)
                'MSW 5/2/13 save the date in American, not Canadian
                oStartNode.InnerXml = tC.StartSerial.ToString(mLanguage.FixedCulture)
                oNode.AppendChild(oStartNode)

                Dim oUserNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsCHANGE_PWUSER, Nothing)
                oUserNode.InnerXml = tC.User.ToString
                oNode.AppendChild(oUserNode)

                Dim oZoneNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, gsCHANGE_ZONE, Nothing)
                oZoneNode.InnerXml = tC.ZoneName.ToString
                oNode.AppendChild(oZoneNode)

                oMainNode.AppendChild(oNode)

            Next

            Try
                oCursor = Cursors.Default
            Catch
            End Try

            oXMLDoc.Save(sPath)

            Return True


        Catch ex As Exception
            Dim sTmp As String = "Module: mPWCommon, Routine: SaveToChangeLog(oZone), Error: "
            Trace.WriteLine(sTmp & ex.Message)
            Trace.WriteLine("Module: mPWCommon, Routine: SaveToChangeLog, StackTrace: " & ex.StackTrace)
            mDebug.ShowErrorMessagebox(sTmp, ex, System.Windows.Forms.Application.ProductName, _
                                        String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Try
                oCursor = Cursors.Default
            Catch

            End Try
            Return False

        Finally
            colChangesSQL = Nothing
        End Try
        Try
            oCursor = Cursors.Default
        Catch
        End Try
    End Function

    Friend Function PingDevice(ByVal HostName As String) As Boolean
        '*****************************************************************************************
        'Description: Ping Routine
        '
        'Parameters: 
        'Returns:   true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim void As String = String.Empty
        Return PingDevice(HostName, 1000, void)

    End Function
    Friend Function PingDevice(ByVal HostName As String, ByVal TimeoutMS As Integer, _
                                                        ByRef ErrorString As String) As Boolean
        '*****************************************************************************************
        'Description: Ping Routine
        '
        'Parameters: 
        'Returns:   true if success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        Dim bResult As Boolean = False
        Dim sIPAddress As String = String.Empty

        ErrorString = String.Empty

        Try
            Dim oPing As NetworkInformation.Ping = New NetworkInformation.Ping()
            Dim oReply As NetworkInformation.PingReply = oPing.Send(HostName, TimeoutMS)

            If (oReply Is Nothing) = False Then
                If (oReply.Address Is Nothing) = False Then
                    sIPAddress = oReply.Address.ToString

                    If HostName.CompareTo(sIPAddress) = 0 Then
                        sIPAddress = String.Empty
                    End If
                End If
            End If


            bResult = (NetworkInformation.IPStatus.Success = oReply.Status)
            ErrorString = oReply.Status.ToString()


        Catch ex As NetworkInformation.PingException
            ErrorString = ex.InnerException.Message
            Trace.WriteLine("Module: mPWCommon, Routine: PingDevice, Error: " & ErrorString)
        Catch ex As Sockets.SocketException
            ErrorString = ex.Message
            Trace.WriteLine("Module: mPWCommon, Routine: PingDevice, Error: " & ErrorString)
        End Try

        Return bResult


    End Function
    Friend Function SetRemoteServer() As Boolean
        '*****************************************************************************************
        'Description: point to another computer
        '
        'Parameters: 
        'Returns:  
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************

        Dim oFB As New FolderBrowserDialog
        Dim sRet As String = String.Empty
        Dim sMsg As String = gcsRM.GetString("csSELECT_PAINT_FAIL")
        Dim sOldRemote As String = msRemotePWpath

        If My.Computer.Network.IsAvailable = False Then Exit Function

        With oFB
            .ShowNewFolderButton = False
            .Description = gcsRM.GetString("csSELECT_PAINT_FOLDER")
            .ShowDialog()
            sRet = .SelectedPath
        End With

        If sRet <> String.Empty Then
            If Strings.Right(sRet, 1) <> "\" Then
                sRet = sRet & "\"
            End If
        End If

        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor

        Try
            'set remote path and see if its good
            msRemotePWpath = sRet
            Dim sPath As String = String.Empty
            If GetDefaultFilePath(sPath, eDir.Database, String.Empty, gsCONFIG_DBNAME) Then
                'should be good to go
                sMsg = gcsRM.GetString("csSELECT_PAINT_SUCCESS")
                MessageBox.Show(sMsg, gcsRM.GetString("csPAINTWORKS"), MessageBoxButtons.OK, _
                            MessageBoxIcon.Information, _
                            MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default

                Return True
            Else
                ' no good or no permission
                msRemotePWpath = sOldRemote
            End If

        Catch ex As Exception
            Trace.WriteLine("Module: mPWCommon, Routine: SetRemoteServer, Error: " & ex.Message)
            Trace.WriteLine("Module: mPWCommon, Routine: SetRemoteServer, StackTrace: " & ex.StackTrace)

        End Try

        MessageBox.Show(sMsg, gcsRM.GetString("csPAINTWORKS"), MessageBoxButtons.OK, _
                        MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, _
                        MessageBoxOptions.DefaultDesktopOnly)
        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default

        Return False

    End Function

#End Region

End Module
