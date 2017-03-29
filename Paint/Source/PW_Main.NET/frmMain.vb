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
' Form/Module: frmMain
'
' Description: PAINTworks IV Main Menu
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
'    11/30/09   MSW     LaunchPWApp - Skip the duplicate window check for rundll32, 
'                       add command line for non-FRWM apps
'    01/29/01   RJO     Changes to subCheckForPreviousInstance and mPWDesktop.bHideNonPWApps
'    07/01/10   MSW     Add some error management to connect with a restarted alarmman or alarmman running in debug
'    07/01/10   MSW     Add some tools to the right-click on the banner
'    08/18/10   MSW     Process #ZONE# tag in program name in XML to manage zone specific .EXEs 
'                       in identical vbapps folders
'    09/14/11   MSW     Assemble a standard version of everything                       4.1.0.0
'   09/27/11    MSW     Standalone changes round 1 - HGB SA paintshop computer changes	4.1.0.1
'    10/06/11   AM      Same PW Main for Paint and Sealer.                              4.1.0.2
'                       Added all the sealer texts and icons
'    11/11/11   MSW			Add right-click option to shutdown while keeping loggers open   4.1.1.0
'                       Fix versions launch from about form
'    01/18/12   MSW     Add default printer to menu, some debug desktop functions         4.01.01.01
'    01/24/12   MSW     Change to new Interprocess Communication in clsInterProcessComm   4.01.01.02
'    02/15/12   MSW     Force 32 bit build for compatability with PCDK                  4.01.01.03
'    03/14/12   RJO     Modifications for new clsPWUser and Password.NET                4.01.02.00
'    03/28/12   MSW     update to scattered access.net, remove PW messenger             4.01.03.00
'                       Switch to the current MSInfo App.  It doesn't need all the menu mods
'    04/19/12   MSW     Add lots of error traps                                         4.01.03.01
'    06/15/12   RJO     Minor property change to frmMenu grpbStaus control.             4.01.03.02
'    09/21/12   AM      Added support to MIS computer no robots                         4.01.03.03
'                       mcolZones.MISComputer
'    10/24/12   RJO     In frmAbout cboLanguage_SelectedValueChanged, removed code to   4.01.03.04
'                       set "CurrentLanguage" value in registry. No longer needed.
'    11/06/12   JBW/RJO Added "LaunchApp" case to subDoScreenAction.                    4.01.03.05
'    12/13/12   MSW     mPWDesktop.StartPWApp - Tweak multistart feature to take        4.01.04.00
'                       advantage of single instance logic in browser
'    04/16/13   MSW     Add Canadian language files                                     4.01.05.00
'    05/09/13   MSW     Add psHELOG_START_MSG:"Starting Hot Edit Logger"                4.01.05.01
'    06/05/13   MSW     Add psVISIONLOGGER_START_MSG:"Starting VIsion Logger"           4.01.05.01
'                       Add message handling for vision and hot edit logger startup
'    07/09/13   MSW     Update and standardize logos                                    4.01.05.02
'    07/19/13   MSW     initialize applicator settings                                  4.01.05.03
'    07/25/13   MSW     menu updates for sealer                                         4.01.05.04
'    08/30/13   MSW     more menu updates for sealer                                    4.01.05.05
'    09/13/13   MSW     Add devices control panel page for USB eject                    4.01.05.06
'    09/30/13   MSW     Save screenshots as jpegs, PLC DLL                              4.01.06.00
'    12/03/13   MSW     Deal with screen start failures                                 4.01.06.01
'    01/06/14   MSW     More screen management                                          4.01.06.02
'    02/12/14   MSW     More KTP changes                                                4.01.06.03
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call                4.01.07.00
'                       Updates from KTP for Weekly reports
'    02/18/14   MSW     subStartPaintworks - Password update from inovision             4.01.07.01
'    05/05/14   MSW     mPWDesktop - StartPWApp - find hidden windows                   4.01.07.02
'********************************************************************************************

Option Compare Text
Option Explicit On
Option Strict On

Imports ConnStat = FRRobotNeighborhood.FRERNConnectionStatusConstants
Imports System.Collections.ObjectModel
Imports System.Xml
Imports System.Xml.XPath
Imports clsPLCComm = PLCComm.clsPLCComm

'TODO - Look at all the places that the path to a file is parsed and try to use the Split function
'       with "/" as the delimiter.
'TODO - Code to notify Password application when selected language changes

Public Class frmMain

#Region " Declares "

    '******** Form Constants   **********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Friend Const gsSCREEN_NAME As String = "PW4_Main"   ' <-- For password area change log etc.
    Friend msSCREEN_NAME As String = gsSCREEN_NAME
    Private Const msMODULE As String = "frmMain"
    Private Const msSCREEN_DUMP_NAME As String = "Menus_MainMenu.jpg"
    Private Const msBASE_ASSEMBLY_COMMON As String = gsSCREEN_NAME & ".CommonStrings"
    Private Const msBASE_ASSEMBLY_LOCAL As String = gsSCREEN_NAME & ".ProjectStrings"
    Private Const msROBOT_ASSEMBLY_LOCAL As String = gsSCREEN_NAME & ".RobotStrings"
    Private Const msMENU_ASSEMBLY_LOCAL As String = gsSCREEN_NAME & ".MenuStrings"
    '******** End Form Constants    *****************************************************************

    'Set to False to hide PAINTworks registration features.
    Public Const gbREGISTRATION_ENABLED As Boolean = False


    '******** Structures   **************************************************************************
    Friend Structure udsAppConfig
        Friend LaunchString As String
        Friend ProcessName As String
        Friend ProcessId As Integer
        Friend hWnd As Integer
        Friend WindowStyle As AppWinStyle
        Friend Delay As Integer
        Friend StartupTimeout As Integer
        Friend StandAlone As Boolean
        Friend AllowMultiple As Boolean
        Friend IsVB6App As Boolean
        Friend StartFlag As Boolean
        Friend CloseApp As Boolean
        Friend Message As String
    End Structure

    Friend Structure udsCultureInfo
        Friend CultureText As String
        Friend FlagImage As String
        Friend CultureName As String
        Friend DefaultCulture As Boolean
        Friend CurrentLanguage As String 'For Legacy PW3 (VB6) Apps.
    End Structure

    Private Structure udsStartupMsg
        Friend Message As String
        Friend Posted As Boolean
    End Structure
    '******** End Structures   **********************************************************************

    Private msCulture As String = "en-US" 'Default to english
    Private mbAlarmBellState As Boolean
    Private mbInIDE As Boolean
    Private mbLoadError As Boolean
    Private mbPLCDisconnected As Boolean
    Private mbBackgroundAppStarted As Boolean
    Private mbFrmFirstVisible As Boolean
    Private mnConnAcks As Integer
    Private mnLastWatchdogTime As Integer
    Private mnPW4_MainHwnd As Integer
    Private mnTileMode As eTile
    Private msStatus As String = String.Empty

    Friend gcolBackgroundApps As Collection(Of udsAppConfig)
    Friend gcolCultureInfo As Collection(Of udsCultureInfo)
    Private WithEvents mcolControllers As clsControllers = Nothing
    Friend mcolZones As clsZones = Nothing
    Private mdtStartList As DataTable
    Private mxndStartList As XmlNodeList
    Private WithEvents mpdScreenShot As Printing.PrintDocument
    Private mStatusMsgs() As udsStartupMsg
    Private mbmpScreen As Bitmap
    Private mLoginTime As Date = Now
    Private mbKeepDesktopOpen As Boolean = False
    Private mbInShutdown As Boolean = False
    'Password Class - New
    Private WithEvents moPassword As clsPWUser 'RJO 03/09/12
    Private mbPrivilegeChecked As Boolean 'RJO 03/13/12
    Private mLaunchConfig As frmMenu.udsButtonConfig 'RJO 03/13/12
    'Password object
    'Private WithEvents moPassword As PWPassword.PasswordObject 'RJO 03/09/12
    'Private mScreenPrivs As PWPassword.cPrivilegeObject 'RJO 03/09/12
    Private msUserName As String

    'PLC Communication
    Private WithEvents moPLC As clsPLCComm

    ''ActiveX exe used for passing messages between PW apps.
    'Public WithEvents mPWMessenger As PWMessenger.clsPWMessenger

    'Windows Messaging variables
    Private msWinMsg As String = String.Empty
    Private mnMsgCount As Integer = 0
    Private mnMsgLen As Integer = 0

    Friend grecWorkingArea As Rectangle

    Private Enum eStartGroup
        PreFRWM = 0
        PostFRWM_PrePLC = 1
        PostPLC_PreFRRN = 2
        PostFRRN = 3
    End Enum

    '********New program-to-program communication object******************************************
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm 'RJO 03/09/12
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet)
    '********************************************************************************************

#End Region

#Region " Properties"

    Friend WriteOnly Property Abort() As Boolean
        '********************************************************************************************
        'Description: Provides an attempt at orderly shutdown when Abort button is clicked on frmFirst.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As Boolean)
            If value Then

                On Error Resume Next

                Call subShutDownPaintworks()

                'restore the desktop
                If mPWDesktop.PWDesktopExists Then
                    Call mPWDesktop.DestroyPWDesktop()
                End If

                Dim oProcs() As Process = Process.GetProcessesByName("explorer")

                If oProcs.Length = 0 Then
                    'Launch an explorer because Paintworks in running in strict mode
                    Shell("explorer.exe", AppWinStyle.NormalFocus)
                End If

                Me.Close()

            End If
            
        End Set

    End Property

    Friend WriteOnly Property AlarmSeverity() As String
        '********************************************************************************************
        'Description: Change the image on the Alarms button to indicate the current Alarm Severity
        '             Level.
        '
        'Parameters: Severity Level ("red", "yellow", or "green")
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As String)
            Try
                tmrRingBell.Enabled = False

                With tlsMain.Items("btnAlarms")
                    Select Case value.ToLower
                        Case "red"
                            mbAlarmBellState = False
                            .Image = DirectCast(gpsRM.GetObject("Redbell1", DisplayCulture), Drawing.Image)
                            tmrRingBell.Enabled = True
                        Case "yellow"
                            .Image = DirectCast(gpsRM.GetObject("Yellowbell", DisplayCulture), Drawing.Image)
                        Case "green"
                            .Image = DirectCast(gpsRM.GetObject("Greenbell", DisplayCulture), Drawing.Image)
                        Case Else
                            .Image = DirectCast(gpsRM.GetObject("Bell", DisplayCulture), Drawing.Image)
                    End Select
                End With
            Catch ex As Exception
                mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: AlarmSeverity", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set

    End Property

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
        ' 07/01/10  MSW     Add some error management to connect with a restarted alarmman or alarmman running in debug
        ' 03/14/12  RJO     New IPC won't throw an exception if the app it's writing to isn't running.
        '                   Also added culture message to Pasword.NET.
        '********************************************************************************************

        Set(ByVal value As String)

            Try
                msCulture = value
                mLanguage.DisplayCultureString = value

                'Use current language text for screen labels
                If mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, msBASE_ASSEMBLY_LOCAL, msROBOT_ASSEMBLY_LOCAL) Then
                    Call subLocalizeFormText()
                    Call subShowNewUser()
                End If

                'A separate resource file for SubMenus
                Dim Void As Boolean = mLanguage.GetAuxResourceManager(msMENU_ASSEMBLY_LOCAL)

                'Tell AlarmMan that the Display Language has changed
                'Also tell Password.NET 'RJO 03/14/12
                For Each BackgroundApp As udsAppConfig In gcolBackgroundApps
                    Select Case BackgroundApp.ProcessName.ToLower
                        Case "alarmman", "password"
                            Dim sMsg() As String = {"culturestring", msCulture}

                            oIPC.WriteControlMsg(BackgroundApp.ProcessName.ToLower, sMsg)
                        Case Else
                            'no user interface - do nothing
                    End Select
                    'If BackgroundApp.ProcessName.ToLower = "alarmman" Then
                    '    Try
                    '        'mWorksComm.SendFRWMMessage("culturestring," & msCulture & ",0,0,0,0", BackgroundApp.ProcessId)
                    '        Dim sMessage(1) As String
                    '        sMessage(0) = "culturestring"
                    '        sMessage(1) = msCulture
                    '        oIPC.WriteControlMsg(gs_COM_ID_ALARM_MAN, sMessage)

                    '    Catch ex As Exception
                    '        subStartAlarmManager()
                    '        'mWorksComm.SendFRWMMessage("culturestring," & msCulture & ",0,0,0,0", BackgroundApp.ProcessId)
                    '        Dim sMessage(1) As String
                    '        sMessage(0) = "culturestring"
                    '        sMessage(1) = msCulture
                    '        oIPC.WriteControlMsg(gs_COM_ID_ALARM_MAN, sMessage)
                    '    End Try
                    '    Exit For
                    'End If
                Next 'BackgroundApp
            Catch ex As Exception
                mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: Culture", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try

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
            Try
                Return New Globalization.CultureInfo(msCulture)
            Catch ex As Exception
                mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: DisplayCulture", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                Return Nothing
            End Try
        End Get

    End Property

    Friend ReadOnly Property InIDE() As Boolean
        '********************************************************************************************
        'Description: Test to determine if running from the Visual Studio IDE
        '
        'Parameters: None
        'Returns:    True if running in Visual Studio IDE
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            mbInIDE = False
            Debug.Assert(IsInIDE)
            InIDE = mbInIDE
        End Get

    End Property

    Private ReadOnly Property IsInIDE() As Boolean
        '********************************************************************************************
        'Description: The actual test to detrmine if running from the Visual Studio IDE. This called 
        '             from the InIDE Property via a Debug.Assert so it'll never get here if the
        '             compiled EXE is running.
        '
        'Parameters: None
        'Returns:    True if running in Visual Studio IDE
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            mbInIDE = True
            IsInIDE = True
        End Get

    End Property

    Friend Property LoggedOnUser() As String
        '********************************************************************************************
        'Description:   Gets/Sets the user logged on to password object
        '
        'Parameters:    none
        'Returns:       username
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Return msUserName
        End Get

        Set(ByVal value As String)
            msUserName = value
        End Set

    End Property

    Friend Property Status(Optional ByVal StatusStrip As Boolean = False) As String
        '********************************************************************************************
        'Description: Added to jive with common modules
        '
        'Parameters: 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msStatus
        End Get

        Set(ByVal Value As String)
            msStatus = Value
        End Set

    End Property

#End Region

#Region " Routines "

    Private Function bGetCultureInfo() As Boolean
        '********************************************************************************************
        'Description:  This Function populates the collection of udsCultureInfo.
        '
        'Parameters: None
        'Returns:    True if Success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Const sXMLFILE As String = "CultureInfo"
        Const sXMLTABLE As String = "CultureInfo"
        Const sXMLNODE As String = "Culture"
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Try
            If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLFILE & ".XML") Then


                oXMLDoc.Load(sXMLFilePath)

                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)
                Try
                    If oNodeList.Count = 0 Then
                        mDebug.WriteEventToLog("PW_Main:frmMain:bGetCultureInfo", sXMLFilePath & " not found.")
                    Else
                        For Each oNode As XmlNode In oNodeList
                            Dim oCultureInfo As New udsCultureInfo
                            Dim sTemp As String = oNode.Item("DefaultCulture").InnerXml

                            oCultureInfo.CultureText = oNode.Item("CultureText").InnerXml
                            oCultureInfo.FlagImage = oNode.Item("FlagImage").InnerXml
                            oCultureInfo.CultureName = oNode.Item("CultureName").InnerXml
                            'Legacy for PW3 (VB6) Apps
                            oCultureInfo.CurrentLanguage = oNode.Item("CurrentLanguage").InnerXml

                            If sTemp.ToLower = "true" Then
                                oCultureInfo.DefaultCulture = True
                                'Legacy for PW3 (VB6) Apps
                                'My.Computer.Registry.SetValue("HKEY_LOCAL_MACHINE\Software\FANUC\PAINTworks", "DefaultLanguage", _
                                '                              DR.Item("CurrentLanguage").ToString)
                            End If

                            gcolCultureInfo.Add(oCultureInfo)

                        Next
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog("PW_Main:frmMain:bGetCultureInfo", "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                End Try
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog("PW_Main:frmMain:bGetCultureInfo", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
        End Try
    End Function

    Private Function bGetStartList() As Boolean
        '********************************************************************************************
        'Description:  This Function populates the DataTable that will be used by subStarPWApps.
        '
        'Parameters: None
        'Returns:    True if Success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Const sXMLFILE As String = "StartupList"
        Const sXMLTABLE As String = "StartupList"
        Const sXMLNODE As String = "Program"
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Try
            If GetDefaultFilePath(sXMLFilePath, eDir.XML, String.Empty, sXMLFILE & ".XML") Then


                oXMLDoc.Load(sXMLFilePath)

                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                mxndStartList = oMainNode.SelectNodes("//" & sXMLNODE)
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog("PW_Main:frmMain:bGetStartList", "Invalid XPath syntax: " & sXMLFilePath & " - " & ex.Message)
        End Try

        'Dim DS As DataSet = New DataSet
        'mdtStartList = New DataTable

        'Try
        '    DS.Locale = mLanguage.FixedCulture
        '    mdtStartList.Locale = mLanguage.FixedCulture

        '    'Read Start List info from config DataBase
        '    Dim DB As clsSQLAccess = New clsSQLAccess
        '    Dim sTableName As String = gsSTARTUP_TABLENAME

        '    With DB

        '        .DBFileName = gsCONFIG_DBNAME
        '        .DBTableName = sTableName

        '        .Zone = mcolZones(mcolZones.CurrentZone)
        '        .SQLString = "SELECT * FROM [" & sTableName & "] ORDER BY [" & _
        '                     sTableName & "].[StartIndex] ASC"

        '        DS = .GetDataSet(False)
        '        If DS.Tables.Contains("[" & sTableName & "]") Then
        '            mdtStartList = DS.Tables("[" & sTableName & "]")
        '            .Close()
        '            Return True
        '        Else
        '            .Close()
        '            Return False
        '        End If

        '    End With 'DB

        'Catch ex As Exception
        '    mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: bGetStartList", _
        '                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        '    Return False
        'End Try

    End Function

    'RJO 03/14/12 - Removed
    'Friend Function CheckPassword(ByVal sScreenName As String, ByVal sPrivilege As String) As Boolean
    '    '********************************************************************************************
    '    'Description: Check if user is logged on, and if he is, check if he can administrate.
    '    '
    '    '
    '    'Parameters: sPrivilege  the requested action - edit, copy, administrate etc.
    '    'Returns: true if ok, false if not.
    '    '
    '    'Modification history:
    '    '
    '    ' By          Date          Reason
    '    '********************************************************************************************

    '    CheckPassword = False

    '    If LoggedOnUser = "" And sPrivilege.ToLower = "execute" Then
    '        'no one logged on
    '        Call moPassword.DisplayLogin()
    '        Exit Function
    '    End If

    '    If LoggedOnUser <> "" Then

    '        With mScreenPrivs
    '            .ScreenName = sScreenName
    '            .Privilege = sPrivilege

    '            Application.DoEvents()

    '            If .ActionAllowed Then
    '                CheckPassword = True
    '                'reset timer
    '                Call moPassword.ResetAutoLogoutTime()
    '            Else
    '                If sPrivilege.ToLower = "execute" Then
    '                    MessageBox.Show(gpsRM.GetString("psNO_ACCESS", DisplayCulture) & sScreenName, _
    '                                    gsSCREEN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '                End If
    '            End If '.ActionAllowed
    '        End With

    '    End If ' LoggedOnUser <> ""

    'End Function

    Private Function eGetAppWinStyle(ByVal WindowStyle As Integer) As AppWinStyle
        '********************************************************************************************
        'Description:  This Function returns the AppWinStyle corresponding to the supplied Integer
        '              value. This is used when an application is not managed by Paintworks and is
        '              launched using the Shell command.
        '
        'Parameters: Window Style
        'Returns:    AppWinStyle
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Select Case WindowStyle

                Case 0
                    Return AppWinStyle.Hide
                Case 1
                    Return AppWinStyle.NormalFocus
                Case 2
                    Return AppWinStyle.MinimizedFocus
                Case 3
                    Return AppWinStyle.MaximizedFocus
                Case 4
                    Return AppWinStyle.NormalNoFocus
                Case 6
                    Return AppWinStyle.MinimizedNoFocus
                Case Else
                    mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: eGetAppWinStyle", _
                                           "Error: an unrecognized WindowStyle [" & WindowStyle.ToString & _
                                           "] was supplied for conversion. Defaulting to AppWinStyle.NormalFocus.")

                    Return AppWinStyle.NormalFocus

            End Select
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: eGetAppWinStyle", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Function

    Friend Sub DoButtonPressFunction(ByVal Command As String, Optional ByVal sChangeLogDesc As String = "")
        '********************************************************************************************
        ' Description:  This Sub is called when a frmMenu button has a "ButtonPress"
        '               Action attached to it. The Command that is passed in comes from
        '               the .Commandline property of the ButtonConfig.
        '
        ' Parameters: Command
        ' Returns: None
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         09/13/13      Add devices control panel page for USB eject
        '********************************************************************************************
        If sChangeLogDesc = "" Then
            sChangeLogDesc = Command
        End If
        Dim sChangeLogText As String = gpsRM.GetString("psEXECUTED", DisplayCulture) & _
                                       sChangeLogDesc

        Try
            Select Case Command.ToLower
                Case "usb", "devices"
                    'Win7 "Devices and printers" control panel object,
                    'Can be used for USB eject, printer select
                    'shortcut:%25systemroot%25\system32\control.exe /name Microsoft.DevicesAndPrinters
                    Shell(Environment.ExpandEnvironmentVariables("%systemroot%\system32\control.exe /name Microsoft.DevicesAndPrinters"), AppWinStyle.NormalFocus)
                Case "OpenFolder"
                    'KDJ 01/14/2014 Added folder access for RM Reports
                    If LoggedOnUser = String.Empty Then
                        'No current user - Access Denied
                        MessageBox.Show("Must be logged in to access explorer folders", _
                                        gsSCREEN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Else
                        Dim sRMPath As String = mcolZones.ActiveZone.DatabasePath & "WeeklyReports"
                        Dim bAdmin As Boolean = moPassword.CheckPassword(ePrivilege.Administrate) 'CheckPassword(gsSCREEN_NAME, "Administrate") 'RJO 03/12/12
                        If Not bAdmin Then
                            'Only Admins should be able to access explorer folders
                            MessageBox.Show("Only Admins can open explorer folders", _
                                    gsSCREEN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        Else
                            Process.Start("explorer.exe", sRMPath)
                        End If

                    End If
                Case Else

            End Select
            'Log execution of App (that requires a password) in Change Log
            mPWCommon.AddChangeRecordToCollection(gsSCREEN_NAME, LoggedOnUser, _
                                                  mcolZones.CurrentZoneNumber, _
                                                  String.Empty, String.Empty, _
                                                  sChangeLogText, mcolZones.CurrentZone)
            mPWCommon.SaveToChangeLog(mcolZones.ActiveZone)

        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: DoButtonPressFunction", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Friend Sub LaunchPWApp(ByVal LaunchString As String, ByVal UseFRWM As Boolean, _
                           ByVal LaunchFlags As Integer, ByVal Commandline As String, _
                           ByVal IsVB6App As Boolean, ByVal UsePassword As Boolean)
        '********************************************************************************************
        ' Description:  This Sub is called when a frmMenu button has a "LaunchFile"
        '               Action attached to it. 
        '
        ' Parameters: LaunchString, UseFRWM, LaunchFlags
        ' Returns: None
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         11/30/09      Skip the duplicate window check for rundll32, add command line for non-FRWM apps
        ' MSW         11/21/13      Deal with screen start failures
        '********************************************************************************************
        Const nDOUBLE_QUOTE As Integer = 34

        Try
            Dim nResultCode As Integer
            Dim sEventMsg As String = String.Empty
            Dim sProcessName As String = String.Empty
            'MSW 02/04/14 - limit retrys to vbapps folder
            Dim bVBApps As Boolean = LaunchString.ToLower.Contains("vbapps")

            For nChar As Integer = 6 To Strings.Len(LaunchString)
                sProcessName = Strings.Right(LaunchString, nChar)
                If Strings.Left(sProcessName, 1) = "\" Then
                    sProcessName = Strings.Right(sProcessName, nChar - 1)
                    sProcessName = Strings.Left(sProcessName, Strings.Len(sProcessName) - 4)
                    Exit For
                End If
            Next 'nChar

            If UsePassword Then
                'If Not CheckPassword(sProcessName, "execute") Then Exit Sub  'RJO 03/13/12
                If mbPrivilegeChecked Then   'RJO 03/13/12
                    'User has privilege to execute app
                    mbPrivilegeChecked = False
                Else
                    'Need to check privilege for this user to execute app. Save the parameters 
                    'this sub was called with so it can be called again.
                    With mLaunchConfig
                        .CommandLine = Commandline
                        .IsVB6App = IsVB6App
                        .LaunchFile = LaunchString
                        .LaunchFlags = LaunchFlags
                        .UseFRWM = UseFRWM
                        .UsePassword = UsePassword
                    End With
                    mbPrivilegeChecked = moPassword.CheckPassword(sProcessName.ToLower, ePrivilege.Execute)
                    Exit Sub
                End If
            End If

            If UseFRWM Then
                'Start the screen in the current language
                If Not IsVB6App Then
                    If Commandline <> String.Empty Then
                        Commandline = Commandline & " "
                    End If
                    Commandline = Commandline & "/culture=" & msCulture
                End If

                nResultCode = mPWDesktop.StartPWApp(Strings.Chr(nDOUBLE_QUOTE) & LaunchString & _
                                                    Strings.Chr(nDOUBLE_QUOTE) & " " & Commandline, _
                                                    LaunchFlags)
                Select Case nResultCode
                    Case 0 'Success
                        If UsePassword Then
                            Dim sChangeLogText As String = gpsRM.GetString("psEXECUTED", DisplayCulture) & _
                                                           LaunchString

                            'Log execution of App (that requires a password) in Change Log
                            mPWCommon.AddChangeRecordToCollection(gsSCREEN_NAME, LoggedOnUser, _
                                                                  mcolZones.CurrentZoneNumber, _
                                                                  String.Empty, String.Empty, _
                                                                  sChangeLogText, mcolZones.CurrentZone)
                            'mPWCommon.SaveToChangeLog(mcolZones.ActiveZone.DatabasePath)
                            mPWCommon.SaveToChangeLog(mcolZones.ActiveZone)
                        End If
                    Case 1
                        sEventMsg = "LaunchString is empty."
                    Case 2
                        sEventMsg = "Maximum number of applications already running."
                    Case 3
                        sEventMsg = sProcessName & " must run in Exclusive mode. Other app(s) running."
                    Case 4, 5
                        sEventMsg = sProcessName & " is already running."
                    Case Else
                        'Do nothing, error was already logged in Event Log
                End Select

                If sEventMsg <> String.Empty Then
                    'Log this event in the event log
                    mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: LaunchPWApp", _
                                           "Managed app " & sProcessName & " launch failed - " & sEventMsg)
                End If

            Else
                'This application is not managed by Paintworks
                Dim oProcs() As Process = Process.GetProcessesByName(sProcessName)
                'MSW 11/21/13 Check for no main window handle if process already running.  kill it
                '   if there's no main.  Also don't tell it to hide if there are no launch flags
                Dim eWinStyle As AppWinStyle = eGetAppWinStyle(LaunchFlags)

                ' MSW         11/30/09      Skip the duplicate window check for rundll32, add command line for non-FRWM apps
                Dim bStart As Boolean = True
                If oProcs.Length > 0 And (InStr(sProcessName.ToLower, "rundll") = 0) Then
                    'The application is already running. Make sure it's visible
                    For Each oProc As Process In oProcs
                        If CInt(oProc.MainWindowHandle) = 0 Then
                            oProc.Kill()
                        Else
                            Call mPWDesktop.ShowWindow(oProc.MainWindowHandle.ToInt32, SW_RESTORE)
                            Call mPWDesktop.SetWindowPos(oProc.MainWindowHandle.ToInt32, HWND_TOPMOST, _
                                                                                      0, 0, 0, 0, 531)
                            bStart = False
                        End If
                    Next
                End If
                If eWinStyle = AppWinStyle.Hide Then
                    eWinStyle = AppWinStyle.NormalFocus
                End If
                Dim nProcessId As Integer
                If bStart Then
                    Do While bStart = True
                        ' MSW         11/30/09      Skip the duplicate window check for rundll32, add command line for non-FRWM apps
                        nProcessId = Shell(LaunchString & " " & Commandline, eWinStyle, False, -1)
                        Dim nCount As Integer = 0
                        Do While nCount <= 5 And bStart
                            Threading.Thread.Sleep(100)
                            oProcs = Process.GetProcessesByName(sProcessName)
                            If oProcs.Length > 0 And (InStr(sProcessName.ToLower, "rundll") = 0) Then
                                'The application is already running. Make sure it's visible
                                For Each oProc As Process In oProcs
                                    If CInt(oProc.MainWindowHandle) = 0 Then
                                        If nCount = 50 Then
                                            oProc.Kill()
                                            'limit retries to the simple choice
                                            eWinStyle = AppWinStyle.NormalFocus
                                        End If
                                    Else
                                        Call mPWDesktop.ShowWindow(oProc.MainWindowHandle.ToInt32, SW_RESTORE)
                                        Call mPWDesktop.SetWindowPos(oProc.MainWindowHandle.ToInt32, HWND_TOPMOST, _
                                                                                                  0, 0, 0, 0, 531)
                                        bStart = False
                                    End If
                                Next
                            End If

                            If (bVBApps = False) Or (mLaunchConfig.IsVB6App = False) Then
                                bStart = False
                            End If
                            nCount += 1
                        Loop
                    Loop

                    If nProcessId <> 0 Then
                        If UsePassword Then
                            Dim sChangeLogText As String = gpsRM.GetString("psEXECUTED", DisplayCulture) & _
                                                           LaunchString

                            'Log execution of App (that requires a password) in Change Log
                            mPWCommon.AddChangeRecordToCollection(gsSCREEN_NAME, LoggedOnUser, _
                                                                  mcolZones.CurrentZoneNumber, _
                                                                  String.Empty, String.Empty, _
                                                                  sChangeLogText, mcolZones.CurrentZone)
                            'mPWCommon.SaveToChangeLog(mcolZones.ActiveZone.DatabasePath)
                            mPWCommon.SaveToChangeLog(mcolZones.ActiveZone)
                        End If
                    End If
                End If

                End If

        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: LaunchPWApp", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Function sGetConnStat(ByRef Controller As clsController) As String
        '********************************************************************************************
        ' Description:  Returns a connection status string for Controller that is shown on frmFirst
        '               when Paintworks starts.
        '
        ' Parameters: Controller
        ' Returns: Connection Status string
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Try
            Dim sConnStat As String = String.Empty

            Select Case Controller.RCMConnectStatus
                Case ConnStat.frRNAvailable
                    sConnStat = grsRM.GetString("rsCONNSTAT_AVAIL")
                Case ConnStat.frRNConnected
                    sConnStat = grsRM.GetString("rsCONNSTAT_CONNECTED")
                Case ConnStat.frRNUnavailable, ConnStat.frRNConnecting
                    sConnStat = grsRM.GetString("rsCONNSTAT_UNAVAIL")
                Case ConnStat.frRNDisconnecting
                    sConnStat = grsRM.GetString("rsCONNSTAT_DISCONNECTING")
                Case Else
                    sConnStat = grsRM.GetString("rsCONNSTAT_UNKNOWN")
            End Select

            sGetConnStat = Controller.Name & gpsRM.GetString("psCONNSTAT_MSG") & sConnStat
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: sGetConnStat", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            sGetConnStat = Nothing
        End Try
    End Function

    Friend Sub ShutDownPaintworks(Optional ByVal bKeepLogging As Boolean = False, Optional ByRef bCancel As Boolean = False)
        '********************************************************************************************
        ' Description:  This Sub is called when a frmMenu button has a "ShutdownRequest"
        '               Action attached to it. 
        '
        ' Parameters: none
        ' Returns: none
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         09/04/09      psPW_SHUTDN_RESTART_MSG had an extra _ in the resx file
        '********************************************************************************************
        Try
            If LoggedOnUser = String.Empty Then
                'No current user - Access Denied
                MessageBox.Show(gpsRM.GetString("psPW_SHUTDN_NOUSER_MSG", DisplayCulture), _
                                gsSCREEN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                bCancel = True
            Else
                Dim Response As DialogResult

                Response = MessageBox.Show(gpsRM.GetString("psPW_SHUTDN_CONFIRM_MSG", DisplayCulture), _
                                           gsSCREEN_NAME, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, _
                                           MessageBoxDefaultButton.Button2)

                If Response = Windows.Forms.DialogResult.OK Then
                    'Check user privilege level
                    Dim ShutDownOK As DialogResult = Windows.Forms.DialogResult.OK
                    Dim bAdmin As Boolean = moPassword.CheckPassword(ePrivilege.Administrate) 'CheckPassword(gsSCREEN_NAME, "Administrate") 'RJO 03/12/12
                    mbInShutdown = True
                    If Not bAdmin Then
                        'The computer will be restarted if the user does not have Administrate privilege
                        ShutDownOK = MessageBox.Show(gpsRM.GetString("psPW_SHUTDN_RESTART_MSG", DisplayCulture), gsSCREEN_NAME, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
                    End If

                    If ShutDownOK = Windows.Forms.DialogResult.OK Then

                        Call mPWDesktop.CloseAllPWApps()

                        Call subShutDownPaintworks(bKeepLogging)
                        If bAdmin Then
                            'restore the desktop
                            If mPWDesktop.PWDesktopExists Then
                                Call mPWDesktop.DestroyPWDesktop()
                            End If

                            Dim oProcs() As Process = Process.GetProcessesByName("explorer")

                            If oProcs.Length = 0 Then
                                'Launch an explorer because Paintworks in running in strict mode
                                Shell("explorer.exe", AppWinStyle.NormalFocus)
                            End If

                            Me.Close()

                        Else

                            If InIDE Then
                                Me.Close()
                            Else
                                Call mPWDesktop.RestartWindows()
                            End If

                        End If

                    End If 'ShutDownOK
                Else
                    bCancel = True
                End If 'ShutDown PW Response = OK

            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: ShutDownPaintworks", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    'Private Sub subCheckDbase()  'RJO 03/14/12 No longer required
    '    '********************************************************************************************
    '    ' Description:  This Sub calls subTestDatabase for each MS Access database that 
    '    '               resides in the Paintworks database folder.
    '    '
    '    ' Parameters: None
    '    ' Returns: None
    '    '
    '    'Modification history:
    '    '
    '    ' By          Date          Reason
    '    '********************************************************************************************

    '    'MSW Debug 9/14/10
    '    If mcolZones Is Nothing Then
    '        mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitializeForm", _
    '               "mcolZones is Nothing ")
    '    ElseIf mcolZones.CurrentZone Is Nothing Then
    '        mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitializeForm", _
    '            "mcolZones.CurrentZone is Nothing ")
    '    ElseIf mcolZones.Item(mcolZones.CurrentZone) Is Nothing Then
    '        mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitializeForm", _
    '            "mcolZones.Item(mcolZones.CurrentZone) is Nothing ")
    '    ElseIf mcolZones.Item(mcolZones.CurrentZone).DatabasePath Is Nothing Then
    '        mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitializeForm", _
    '            "mcolZones.Item(mcolZones.CurrentZone).DatabasePath is Nothing ")
    '    End If

    '    Dim sDBPath As String = mcolZones.Item(mcolZones.CurrentZone).DatabasePath
    '    Dim sDBFiles() As String = IO.Directory.GetFiles(sDBPath, "*.mdb")
    '    Dim sDBName As String = String.Empty

    '    Me.Cursor = Cursors.WaitCursor

    '    For Each sDBFile As String In sDBFiles
    '        'sDBFile includes the full path. Parse the database file name.
    '        For nChar As Integer = 6 To Strings.Len(sDBFile)
    '            sDBName = Strings.Right(sDBFile, nChar)
    '            If Strings.Left(sDBName, 1) = "\" Then
    '                sDBName = Strings.Right(sDBFile, nChar - 1)
    '                Exit For
    '            End If
    '        Next

    '        Try
    '            Dim dbTest As New clsDBAccess
    '            Dim nRetVal As Integer

    '            With dbTest
    '                Dim bUsePassword As Boolean

    '                .DBFilePath = sDBPath
    '                .DBFileName = sDBName
    '                If sDBName.ToLower = "security.mdb" Then
    '                    bUsePassword = True
    '                    .DBPassword = Crypto.Crypto.Decrypt("125 227 162 176 46 139 44 248 74 126 146 101 39 80 206 4")
    '                End If

    '                nRetVal = .TestDB(bUsePassword)
    '                Select Case nRetVal
    '                    Case 0 'Success
    '                        .Close()
    '                    Case 5 'Password Error
    '                        Call mPWDesktop.PostStatusMsg(gpsRM.GetString("psCHECK_DB_PWD_ERR_MSG", DisplayCulture) _
    '                                                      & " - [" & sDBName & "]")
    '                        'Write this error to event log
    '                        mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subCheckDbase", _
    '                                               "Database [" & sDBName & "] has an unknown password and cannot be opened.")
    '                    Case Else
    '                        If Not bUsePassword Then
    '                            'attempt to repair db
    '                            If .RepairDB Then
    '                                Call mPWDesktop.PostStatusMsg(gpsRM.GetString("psCHECK_DB_REP_SUCCESS_MSG", DisplayCulture) _
    '                                                              & " - [" & sDBName & "]")
    '                                'Write this error to event log
    '                                mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subCheckDbase", _
    '                                                       "Database [" & sDBName & "] has been repaired.")

    '                                'Replace <dbname>.mdb with <dbName>REP.mdb
    '                                Dim sRepDBName As String = Strings.Left(sDBName, Strings.Len(sDBName) - 4) & "REP.mdb"

    '                                If IO.File.Exists(sDBPath & sRepDBName) Then
    '                                    If IO.File.Exists(sDBPath & sDBName) Then
    '                                        Dim oFI1 As New IO.FileInfo(sDBPath & sDBName)

    '                                        oFI1.MoveTo(sDBPath & Strings.Left(sDBName, Strings.Len(sDBName) - 3) & "BAK")
    '                                    End If
    '                                    Dim oFI2 As New IO.FileInfo(sDBPath & sRepDBName)

    '                                    oFI2.MoveTo(sDBPath & sDBName)
    '                                End If
    '                            Else
    '                                Call mPWDesktop.PostStatusMsg(gpsRM.GetString("psCHECK_DB_REP_FAIL_MSG", DisplayCulture) _
    '                                                              & " - [" & sDBName & "]")
    '                                'Write this error to event log
    '                                mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subCheckDbase", _
    '                                                       "Database [" & sDBName & "] repair attempt failed.")

    '                            End If
    '                        End If 'Not bUsePassword
    '                End Select

    '            End With 'dbTest

    '        Catch ex As Exception
    '            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subCheckDbase", _
    '                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
    '        End Try

    '    Next 'sDBName

    '    Me.Cursor = Cursors.Default

    'End Sub

    Private Sub subCheckForPreviousInstance()
        '********************************************************************************************
        'Description:  This sub is the VB.NET equivalent of App.PrevInstance in VB 6
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/29/10  RJO     This bombed out on the MessageBox because te resource mangers weren't
        '                   instantiated yet. Then Application.Exit threw the running PW4_Main a
        '                   curve ball.
        '********************************************************************************************

        Try

            If Process.GetProcessesByName _
                      (Process.GetCurrentProcess.ProcessName).Length > 1 Then

                'MessageBox.Show(gpsRM.GetString("psPREV_INSTANCE_MSG", DisplayCulture), _
                '                gcsRM.GetString("psERROR", DisplayCulture), _
                '                MessageBoxButtons.OK, _
                '                MessageBoxIcon.Exclamation)
                'Application.Exit() 'RJO 01/29/10
                Me.Close()
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subCheckForPreviousInstance", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

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
        ' 11/06/12  JBW/RJO Added "launchapp" case
        '******************************************************************************************** 

        Try
            Debug.Print(DR.Item(Paintworks_IPC.clsInterProcessComm.sCOL_ACTION).ToString)
            Debug.Print(DR.Item(Paintworks_IPC.clsInterProcessComm.sCOL_PARAMETER).ToString)
            Select Case DR.Item(Paintworks_IPC.clsInterProcessComm.sCOL_ACTION).ToString.ToLower

                Case "alarmlevel"
                    AlarmSeverity = DR.Item(Paintworks_IPC.clsInterProcessComm.sCOL_PARAMETER).ToString

                Case "close"
                    'PW_Maint is asking PW4_Main to close

                    mbInShutdown = True
                    Call subRemoteShutDown()

                Case "hwnd"
                    'TODO - Will This be needed for anything
                    'This is how PW4_Main gets the MainWindowHandle from a VB6 App. It is stored for
                    'future use in the StartInfo.Verb property on the Process because it can be hijacked
                    'easily. The ScreenParams portion of the message is formatted <VB6 hWnd>:<ProcessName>
                    'Dim sParams() As String = Split(ScreenCommand.ScreenParams, ":")

                    'For Each oProc As Process In mcolPWProcs
                    '    If oProc.ProcessName.ToLower = sParams(1).ToLower Then
                    '        If oProc.StartInfo.Verb = String.Empty Then
                    '            oProc.StartInfo.Verb = sParams(0) 'MainWindowHandle
                    '            'Call subTile()
                    '            Exit For
                    '        End If
                    '    End If
                    'Next

                Case "alarmmanload", "prodlogrload", "schedmanload", "passwordload", "scattaccessmanload", _
                    "hoteditloggerload", "visionloggerload" 'RJO 03/09/12 'MSW 3/28/12, 5/28/13
                    mbBackgroundAppStarted = True
                    If DR.Item(Paintworks_IPC.clsInterProcessComm.sCOL_PARAMETER).ToString.ToLower = "false" Then
                        mbLoadError = True
                    End If
                Case "poststatusmsg"
                    If mPWDesktop.FormFirstVisible Then
                        Call mPWDesktop.PostStatusMsg(DR.Item(Paintworks_IPC.clsInterProcessComm.sCOL_PARAMETER).ToString)
                    End If

                Case "launchapp" 'JBW/RJO 11/06/12
                    Dim sLaunchFile As String = String.Empty
                    Dim sParams() As String = Strings.Split(DR.Item(Paintworks_IPC.clsInterProcessComm.sCOL_PARAMETER).ToString, ",")

                    If mPWCommon.GetDefaultFilePath(sLaunchFile, eDir.VBApps, String.Empty, sParams(0)) Then
                        With mLaunchConfig
                            .CommandLine = sParams(1)
                            .IsVB6App = False
                            .LaunchFile = sLaunchFile
                            .LaunchFlags = eStartupFlags.MultiStartSingleInstance
                            .UseFRWM = True
                            .UsePassword = False
                            Call LaunchPWApp(.LaunchFile, .UseFRWM, .LaunchFlags, .CommandLine, .IsVB6App, .UsePassword)
                        End With
                    End If

            End Select

        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoScreenAction", _
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

    '        Select Case ScreenCommand.ScreenAction.ToLower

    '            Case "alarmlevel"
    '                AlarmSeverity = ScreenCommand.ScreenParams.ToLower

    '            Case "close"
    '                'PW_Maint is asking PW4_Main to close

    '                mbInShutdown = True
    '                Call subRemoteShutDown()

    '            Case "hwnd"
    '                'TODO - Will This be needed for anything
    '                'This is how PW4_Main gets the MainWindowHandle from a VB6 App. It is stored for
    '                'future use in the StartInfo.Verb property on the Process because it can be hijacked
    '                'easily. The ScreenParams portion of the message is formatted <VB6 hWnd>:<ProcessName>
    '                'Dim sParams() As String = Split(ScreenCommand.ScreenParams, ":")

    '                'For Each oProc As Process In mcolPWProcs
    '                '    If oProc.ProcessName.ToLower = sParams(1).ToLower Then
    '                '        If oProc.StartInfo.Verb = String.Empty Then
    '                '            oProc.StartInfo.Verb = sParams(0) 'MainWindowHandle
    '                '            'Call subTile()
    '                '            Exit For
    '                '        End If
    '                '    End If
    '                'Next

    '            Case "alarmmanload", "prodlogrload"
    '                mbBackgroundAppStarted = True
    '                If ScreenCommand.ScreenParams.ToLower = "false" Then
    '                    mbLoadError = True
    '                End If

    '            Case "poststatusmsg"
    '                If mPWDesktop.FormFirstVisible Then
    '                    Call mPWDesktop.PostStatusMsg(ScreenCommand.ScreenParams)
    '                End If

    '        End Select

    '    Catch ex As Exception
    '        mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subDoScreenAction", _
    '                               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
    '    End Try

    'End Sub

    Private Sub subInitializeForm()
        '********************************************************************************************
        'Description: Called on form load
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/05/10  MSW     Disable some buttons in teach
        ' 09/14/10  MSW     pause to allow services to startup
        ' 11/08/10  MSW     Move pause after frmFirst.show
        ' 01/21/11  MSW     Shuffle the pauses some more in an attempt to deal with GM's junker PC
        '********************************************************************************************
        Dim recScreenBounds As Rectangle = Screen.GetBounds(tlsMain)
        Dim objLocation As Drawing.Point

        Try
            ''Start PWMessenger
            'mPWMessenger = New PWMessenger.clsPWMessenger

            ''TODO - the client should be gsSCREEN_NAME but then VB6 apps may need to change?
            'mPWMessenger.Register("Main")


            Application.DoEvents()
            'Threading.Thread.Sleep(1000)

            Dim bResources As Boolean = mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, msBASE_ASSEMBLY_LOCAL, msROBOT_ASSEMBLY_LOCAL)
            My.Application.DoEvents()
            'Create the Zones Colection
            'MSW Debug 9/14/10
            Dim bRetry As Boolean = True
            Do While bRetry = True
                My.Application.DoEvents()
                mcolZones = New clsZones(String.Empty)
                'MSW Debug 9/14/10
                If mcolZones Is Nothing Then
                    mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitializeForm", _
                           "mcolZones is Nothing ")
                ElseIf mcolZones.CurrentZone Is Nothing Then
                    mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitializeForm", _
                        "mcolZones.CurrentZone is Nothing ")
                ElseIf mcolZones.Item(mcolZones.CurrentZone) Is Nothing Then
                    mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitializeForm", _
                        "mcolZones.Item(mcolZones.CurrentZone) is Nothing ")
                ElseIf mcolZones.Item(mcolZones.CurrentZone).DatabasePath Is Nothing Then
                    mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitializeForm", _
                        "mcolZones.Item(mcolZones.CurrentZone).DatabasePath is Nothing ")
                ElseIf mcolZones.CurrentZone = String.Empty Then
                    mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitializeForm", _
                        "mcolZones.CurrentZone = String.Empty ")
                Else
                    bRetry = False
                End If
                If bRetry Then
                    My.Application.DoEvents()
                    'Let some startup tasks finish
                    Threading.Thread.Sleep(5000)
                End If
                My.Application.DoEvents()
            Loop

            '********New program-to-program communication object******************************************
            oIPC = New Paintworks_IPC.clsInterProcessComm() '(gs_COM_ID_PW_MAIN, , , True)'RJO 03/09/11
            '********************************************************************************************

            'Size and locate the form
            objLocation.X = 0
            objLocation.Y = 0
            Me.Height = tlsMain.Height
            Me.Width = Screen.GetBounds(Screen.PrimaryScreen.Bounds).Width 'recScreenBounds.Width
            Me.Location = objLocation

            'Get the working area of the screen for frmFirst
            grecWorkingArea = SystemInformation.WorkingArea
            grecWorkingArea.Y = tlsMain.Height
            grecWorkingArea.Height -= tlsMain.Height

            'Use default language text for screen labels
            gcolCultureInfo = New Collection(Of udsCultureInfo)

            If (Not bGetCultureInfo()) Or (gcolCultureInfo.Count = 0) Then
                'No culture info retrieved, default to US english
                Dim oCultureInfo As New udsCultureInfo

                With oCultureInfo
                    .CultureText = "psLANG_EN_US"
                    .FlagImage = "Flgusa"
                    .CultureName = "en-US"
                    .DefaultCulture = True
                End With
                gcolCultureInfo.Add(oCultureInfo)
            End If

            For Each oCultureInfo As udsCultureInfo In gcolCultureInfo
                If oCultureInfo.DefaultCulture Then
                    msCulture = oCultureInfo.CultureName
                    Exit For
                End If
            Next

            If bResources Then
                My.Application.DoEvents()
                frmFirst.Show()
                My.Application.DoEvents()
                Call subLocalizeFormText()
                My.Application.DoEvents()
            End If

            'Configure the Main Menu based on type of GUI this iss
            If mcolZones.StandAlone Or mcolZones.PaintShopComputer Or _
                mcolZones.CurrentZone.ToLower.Contains("teach") Or mcolZones.MISComputer Then
                With tlsMain
                    .Items("btnOperate").Enabled = False
                    .Items("btnOperate").Visible = False
                End With
            End If
            If mcolZones.CurrentZone.ToLower.Contains("teach") Or mcolZones.MISComputer Then
                With tlsMain
                    .Items("btnProcess").Enabled = False
                    .Items("btnProcess").Visible = False
                End With
            End If

            'HGB changes for standalone paintshop computer
            If (Not mcolZones.StandAlone) And mcolZones.PaintShopComputer Then
                With tlsMain
                    .Items("btnAlarms").Enabled = False
                    .Items("btnAlarms").Visible = False
                    'NRU 161214 Syscol only PSC
                    .Items("btnProcess").Visible = False
                    '' ''.Items("btnView").Visible = False
                    '' ''.Items("btnOperate").Visible = False
                    '' ''.Items("btnReports").Visible = False
                    '' ''.Items("btnUtilities").Visible = False
                    '' ''.Items("btnHelp").Visible = False
                End With
            End If

            'A separate resource file for SubMenus
            Dim Void As Boolean = mLanguage.GetAuxResourceManager(msMENU_ASSEMBLY_LOCAL)

        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitializeForm", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subKillOrphanProcs(ByVal ProcName As String)
        '********************************************************************************************
        'Description: Clean up any processes that an aborted Paintworks may have left running.
        '
        'Parameters: ProcName - the name of the process to Kill
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************

        Try
            Dim oProcs() As Process = Process.GetProcessesByName(ProcName)

            If oProcs.Length > 0 Then
                For Each oProc As Process In oProcs
                    oProc.Kill()
                    oProc.WaitForExit()
                Next 'oProc
                oProcs = Nothing
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subKillOrphanProcs", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subLaunchBackgroundApp(ByRef AppConfig As udsAppConfig)
        '********************************************************************************************
        'Description: Launch the specified background application.
        '
        'Parameters: AppConfig - Launch configuration for background app
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        'TODO - need to turn this into a function that returns true for success so we don't add AppConfig
        '       to the app collection if the startup fails
        Try
            mbBackgroundAppStarted = False

            With AppConfig

                If .Message <> String.Empty Then
                    Call mPWDesktop.PostStatusMsg(gpsRM.GetString(.Message, DisplayCulture))
                End If

                .ProcessId = Shell(.LaunchString, .WindowStyle, False, -1)

                Dim DoneTime As Date = DateTime.Now

                If .StartFlag Then

                    DoneTime = DoneTime.AddSeconds(CType(.StartupTimeout, Double))
                Else
                    If .Delay > 0 Then DoneTime = DoneTime.AddMilliseconds(CType(.Delay, Double))
                End If

                Do While DateTime.Now < DoneTime
                    Application.DoEvents()
                    If mbBackgroundAppStarted Then Exit Do
                Loop
                If Not mbBackgroundAppStarted Then
                    mDebug.WriteEventToLog("PW4_Main frmMain.subLaunchBackgroundApp", _
                                           .ProcessName & " startup timed out.")
                End If
                mbBackgroundAppStarted = True

                'Get the MainWindowHandle for messaging
                If .ProcessId > 0 Then
                    Dim oProc As Process = Process.GetProcessById(.ProcessId)
                    .hWnd = oProc.MainWindowHandle.ToInt32
                End If

            End With 'AppConfig

        Catch ex As Exception
            mbLoadError = True

            If mPWDesktop.FormFirstVisible Then
                Call mPWDesktop.PostStatusMsg(gpsRM.GetString("psCOULD_NOT_START", DisplayCulture) & AppConfig.ProcessName)
            End If

            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subLaunchBackgroundApp", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
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
        Dim oCulture As Globalization.CultureInfo = DisplayCulture

        With mLanguage.gpsRM
            tlsMain.Items("btnConfig").Text = .GetString("psCONFIG_BTN_TXT", oCulture)
            tlsMain.Items("btnConfig").Tag = .GetString("psCONFIG_BTN_TAG", oCulture)
            tlsMain.Items("btnConfig").ToolTipText = .GetString("psCONFIG_BTN_TT", oCulture)
            tlsMain.Items("btnProcess").Text = .GetString("psPROCESS_BTN_TXT", oCulture)
            tlsMain.Items("btnProcess").Tag = .GetString("psPROCESS_BTN_TAG", oCulture)
            tlsMain.Items("btnProcess").ToolTipText = .GetString("psPROCESS_BTN_TT", oCulture)
            tlsMain.Items("btnView").Text = .GetString("psVIEW_BTN_TXT", oCulture)
            tlsMain.Items("btnView").Tag = .GetString("psVIEW_BTN_TAG", oCulture)
            tlsMain.Items("btnView").Tag = .GetString("psVIEW_BTN_TT", oCulture)
            tlsMain.Items("btnOperate").Text = .GetString("psOPERATE_BTN_TXT", oCulture)
            tlsMain.Items("btnOperate").Tag = .GetString("psOPERATE_BTN_TAG", oCulture)
            tlsMain.Items("btnOperate").ToolTipText = .GetString("psOPERATE_BTN_TT", oCulture)
            tlsMain.Items("btnReports").Text = .GetString("psREPORTS_BTN_TXT", oCulture)
            tlsMain.Items("btnReports").Tag = .GetString("psREPORTS_BTN_TAG", oCulture)
            tlsMain.Items("btnReports").ToolTipText = .GetString("psREPORTS_BTN_TT", oCulture)
            tlsMain.Items("btnUtilities").Text = .GetString("psUTILITIES_BTN_TXT", oCulture)
            tlsMain.Items("btnUtilities").Tag = .GetString("psUTILITIES_BTN_TAG", oCulture)
            tlsMain.Items("btnUtilities").ToolTipText = .GetString("psUTILITIES_BTN_TT", oCulture)
            tlsMain.Items("btnMaintenance").Text = .GetString("psMAINT_BTN_TXT", oCulture)
            tlsMain.Items("btnMaintenance").Tag = .GetString("psMAINT_BTN_TAG", oCulture)
            tlsMain.Items("btnMaintenance").ToolTipText = .GetString("psMAINT_BTN_TT", oCulture)
            tlsMain.Items("btnHelp").Text = .GetString("psHELP_BTN_TXT", oCulture)
            tlsMain.Items("btnHelp").Tag = .GetString("psHELP_BTN_TAG", oCulture)
            tlsMain.Items("btnHelp").ToolTipText = .GetString("psHELP_BTN_TT", oCulture)
            tlsMain.Items("btnAlarms").Text = .GetString("psALARMS_BTN_TXT", oCulture)
            tlsMain.Items("btnAlarms").Tag = .GetString("psALARMS_BTN_TAG", oCulture)
            tlsMain.Items("btnAlarms").ToolTipText = .GetString("psALARMS_BTN_TT", oCulture)

            lblUserName.Text = .GetString("psUSER", oCulture) & .GetString("psNONE", oCulture)
            lblFanuc.Text = .GetString("psFANUC_LBL_TXT", oCulture)
            lblWindowTile.Text = .GetString("psTILE_MODE_LBL_TXT", oCulture)
            optTileMode0.Text = .GetString("psCASCADE_OPT_TXT", oCulture)
            optTileMode1.Text = .GetString("psHORIZ_OPT_TXT", oCulture)
            optTileMode2.Text = .GetString("psVERT_OPT_TXT", oCulture)

            ToolTip1.SetToolTip(optTileMode0, .GetString("psCASCADE_OPT_TT", oCulture))
            ToolTip1.SetToolTip(optTileMode1, .GetString("psHORIZ_OPT_TT", oCulture))
            ToolTip1.SetToolTip(optTileMode2, .GetString("psVERT_OPT_TT", oCulture))
            ToolTip1.SetToolTip(btnPrintScreen, .GetString("psPRINTSCREEN_BTN_TT", oCulture))

        End With

    End Sub

    Private Sub subRemoteShutDown()
        '*******************************************************************************
        'Description: This Sub allows PW_Maint to shutdown Paintworks. 
        '
        'Parameters: None
        'Returns: None
        '
        'Modification history:
        '
        ' By          Date          Reason
        '*******************************************************************************
        Try
            Call mPWDesktop.CloseAllPWApps()

            Call subShutDownPaintworks()

            If mPWDesktop.PWDesktopExists Then
                Call mPWDesktop.DestroyPWDesktop()
            End If

            Me.Close()
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subRemoteShutDown", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub subShowNewUser()
        '*******************************************************************************
        'Description: This Sub updates the current user display and displays the 
        '             correct text on the the User Log On/Off command button
        '
        'Parameters: None
        'Returns: None
        '
        'Modification history:
        '
        ' By          Date          Reason
        '*******************************************************************************

        Try
            Dim oCulture As Globalization.CultureInfo = DisplayCulture
            Dim sToolTip As String = String.Empty

            With gpsRM

                sToolTip = .GetString("psNO_USER_TT", oCulture)
                LoggedOnUser = moPassword.UserName

                If msUserName = String.Empty Then
                    'User logged off
                    lblUserName.Text = .GetString("psUSER", oCulture) & .GetString("psNONE", oCulture)
                    btnUserLogOn.Text = .GetString("psLOGON_BTN_TXT", oCulture)
                    ToolTip1.SetToolTip(btnUserLogOn, .GetString("psLOGON_BTN_TT", oCulture))
                Else
                    'User logged on
                    lblUserName.Text = .GetString("psUSER", oCulture) & msUserName
                    btnUserLogOn.Text = .GetString("psLOGOFF_BTN_TXT", oCulture)
                    ToolTip1.SetToolTip(btnUserLogOn, .GetString("psLOGOFF_BTN_TT", oCulture))
                    sToolTip = lblUserName.Text & .GetString("psCUR_LOGIN_STATUS", oCulture) & mLoginTime & ". "
                    sToolTip = sToolTip & .GetString("psLAST_LOGIN_STATUS", oCulture) & moPassword.LastLogIn & "."
                End If
            End With 'gpsRM

            ToolTip1.SetToolTip(lblUserName, sToolTip)

        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subShowNewUser", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

        End Try

    End Sub

    Private Sub subShutDownPaintworks(Optional ByVal bKeepLogging As Boolean = False)
        '********************************************************************************************
        'Description: Shuts down Alarm Manager and the background applications in the Startup List
        '             that have their CloseApp flags set to True.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/04/09  msw     Add timeout to WaitForExit so PW4 doesn't hang up on the shutdown.
        '********************************************************************************************
        Try
            Dim sProcName As String = String.Empty
            Dim oProcs() As Process = Nothing
            For Each BackgroundApp As udsAppConfig In gcolBackgroundApps

                'Only close the background apps with the CloseApp flag set to True
                If BackgroundApp.CloseApp Then
                    sProcName = BackgroundApp.ProcessName

                    oProcs = Process.GetProcessesByName(sProcName)

                    If oProcs.Length > 0 Then

                        Select Case sProcName.ToLower

                            Case "alarmman", "password", "prodlogger", "schedman", "scatteredaccess" 'MSW 3/28/12
                                If bKeepLogging = False Then
                                    mDebug.WriteEventToLog("PW4_Main frmMain.subShuDownPaintworks", "Sending <close> to " & BackgroundApp.ProcessName & ", Process ID = " & BackgroundApp.ProcessId.ToString)
                                    'mWorksComm.SendFRWMMessage("close,0,0,0,0,0", BackgroundApp.ProcessId)
                                    Dim sMessage(0) As String
                                    sMessage(0) = "close"
                                    oIPC.WriteControlMsg(sProcName.ToLower, sMessage) 'gs_COM_ID_ALARM_MAN, sMessage)

                                    If Not (oProcs(0).WaitForExit(5000)) Then
                                        oProcs(0).Kill()
                                        If Not (oProcs(0).WaitForExit(5000)) Then
                                            mDebug.WriteEventToLog("PW4_Main frmMain.subShuDownPaintworks", "Shutdown " & BackgroundApp.ProcessName & ", Process ID = " & BackgroundApp.ProcessId.ToString & " Failed to shutdown")
                                        End If
                                    End If
                                End If

                                'Case "prodlogger"
                                '    If bKeepLogging = False Then
                                '        mDebug.WriteEventToLog("PW4_Main frmMain.subShuDownPaintworks", "Sending <close> to " & BackgroundApp.ProcessName & ", Process ID = " & BackgroundApp.ProcessId.ToString)
                                '        'mWorksComm.SendFRWMMessage("close,0,0,0,0,0", BackgroundApp.ProcessId)
                                '        Dim sMessage(0) As String
                                '        sMessage(0) = "close"
                                '        oIPC.WriteControlMsg(gs_COM_ID_PROD_LOG, sMessage)

                                '        If Not (oProcs(0).WaitForExit(5000)) Then
                                '            oProcs(0).Kill()
                                '            If Not (oProcs(0).WaitForExit(5000)) Then
                                '                mDebug.WriteEventToLog("PW4_Main frmMain.subShuDownPaintworks", "Shutdown " & BackgroundApp.ProcessName & ", Process ID = " & BackgroundApp.ProcessId.ToString & " Failed to shutdown")
                                '            End If
                                '        End If
                                '    End If

                            Case Else
                                oProcs(0).Kill()
                                If Not (oProcs(0).WaitForExit(5000)) Then
                                    mDebug.WriteEventToLog("PW4_Main frmMain.subShuDownPaintworks", "Shutdown " & BackgroundApp.ProcessName & ", Process ID = " & BackgroundApp.ProcessId.ToString & " Failed to shutdown")
                                End If

                        End Select


                    End If 'Not oProc Is Nothing

                End If 'BackgroundApp.CloseApp

                Application.DoEvents()

            Next 'BackgroundApp
            'Special cases
            'oProcs = Process.GetProcessesByName("PWMessenger")
            'If oProcs.Length > 0 Then
            '    oProcs(0).Kill()
            '    If Not (oProcs(0).WaitForExit(5000)) Then
            '        mDebug.WriteEventToLog("PW4_Main frmMain.subShuDownPaintworks", "Shutdown PWMessenger.exe, Process ID = " & oProcs(0).Id.ToString & " Failed to shutdown")
            '    End If
            'End If
            'oProcs = Process.GetProcessesByName("ScatteredAccessDB")
            'If oProcs.Length > 0 Then
            '    oProcs(0).Kill()
            '    If Not (oProcs(0).WaitForExit(5000)) Then
            '        mDebug.WriteEventToLog("PW4_Main frmMain.subShuDownPaintworks", "Shutdown ScatteredAccessDB.exe, Process ID = " & oProcs(0).Id.ToString & " Failed to shutdown")
            '    End If
            'End If
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subShutDownPaintworks", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub subStartAlarmManager()
        '********************************************************************************************
        'Description: Starts Up the Paintworks Alarm Manger background application. This one is  
        '             treated as a special case because there is a visual interface that Paintworks 
        '             needs to manage.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 07/01/10  MSW     Add some error management to connect with a restarted alarmman or alarmman running in debug
        '********************************************************************************************
        Try

        Dim AppConfig As udsAppConfig
            Dim sExePath As String = String.Empty

            mPWCommon.GetDefaultFilePath(sExePath, eDir.VBApps, String.Empty, String.Empty)

            With AppConfig
                .AllowMultiple = False
                .CloseApp = True
                .Delay = 500
                .IsVB6App = False
                .LaunchString = sExePath & "AlarmMan.exe /culture=" & msCulture
                .Message = "psALARMMAN_START_MSG"
                .ProcessName = "AlarmMan"
                .StandAlone = True
                .StartFlag = True
                .StartupTimeout = 20
                .WindowStyle = AppWinStyle.MinimizedNoFocus

                Dim oProcs() As Process = Process.GetProcessesByName(.ProcessName)

                If oProcs.Length > 0 Then
                    'AlarmMan is already running. Fill in the blanks in AppConfig
                    Dim oProc As Process = oProcs(0)

                    .ProcessId = oProc.Id
                    .hWnd = oProc.MainWindowHandle.ToInt32
                    'CreatePWDesktop will have already hidden this window
                    Call mPWDesktop.ShowWindow(.hWnd, mPWDesktop.SW_SHOWMINIMIZED)
                Else
                    'Try to get in touch with a program running in visual studio
                    oProcs = Process.GetProcessesByName(.ProcessName & ".vshost")
                    If oProcs.Length > 0 Then
                        'AlarmMan is already running. Fill in the blanks in AppConfig
                        Dim oProc As Process = oProcs(0)

                        .ProcessId = oProc.Id
                        .hWnd = oProc.MainWindowHandle.ToInt32
                        'CreatePWDesktop will have already hidden this window
                        Call mPWDesktop.ShowWindow(.hWnd, mPWDesktop.SW_SHOWMINIMIZED)
                    Else
                        'OK for Launch
                        Call subLaunchBackgroundApp(AppConfig)
                    End If
                End If

                If .ProcessId <> 0 Then
                    Dim hWnd As IntPtr = Process.GetCurrentProcess.MainWindowHandle
                    'Dim sRec As String = String.Empty

                    Dim sMessage(5) As String
                    sMessage(0) = "setmaximizedbounds"
                    sMessage(1) = "0"
                    With grecWorkingArea
                        'sRec = .Left.ToString & "," & .Top.ToString & "," & .Width.ToString & "," & .Height.ToString
                        sMessage(2) = .Left.ToString
                        sMessage(3) = .Top.ToString
                        sMessage(4) = .Width.ToString
                        sMessage(5) = .Height.ToString
                    End With

                    'mWorksComm.SendFRWMMessage("setmaximizedbounds,0," & sRec, .ProcessId)

                    oIPC.WriteControlMsg(gs_COM_ID_ALARM_MAN, sMessage)
                    mPWDesktop.AlarmManHwnd = hWnd.ToInt32

                End If

            End With
            For nIndex As Integer = gcolBackgroundApps.Count - 1 To 0 Step -1
                If gcolBackgroundApps(nIndex).ProcessName.ToLower = "alarmman" Then
                    gcolBackgroundApps.RemoveAt(nIndex)
                End If
            Next
            gcolBackgroundApps.Add(AppConfig)
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subStartAlarmManager", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub subStartHotLinks()
        '********************************************************************************************
        'Description: Starts Up hotlink for PLC Watchdog. 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            moPLC = New clsPLCComm

            With moPLC
                .ZoneName = mcolZones.CurrentZone
                .TagName = "PLCWatchDog"
                'Jump start the HotLink
                Dim sVoid() As String = .PLCData
            End With
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subStartHotLinks", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub subStartPaintworks()
        '********************************************************************************************
        'Description: Starts Up all of the applications in the Startup List. 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/14/12  RJO     Removed check for Access Databases. Removed reference to PWPassword.exe.
        '********************************************************************************************
        Try

            Dim bOkToCreateDesktop As Boolean = True
            Dim bSuccess As Boolean

            Call mPWDesktop.PostStatusMsg(gpsRM.GetString("psINITIALIZING_MSG", DisplayCulture))

            'Shut down "PWPassword.exe" if it running
            'Call subKillOrphanProcs("PWPassword") 'RJO 03/14/12

            'Shut down any orphan instances of the robot object "FRROBOT.EXE"
            Call subKillOrphanProcs("frrobot")

            'Shut down any orpan instances of the Robot Neighborhood "FRROBO~n.EXE"
            For nIndex As Integer = 1 To 9
                Call subKillOrphanProcs("FRROBO~" & nIndex.ToString)
            Next

            'Check for corrupted MS Access databases
            'Call mPWDesktop.PostStatusMsg(gpsRM.GetString("psCHECK_DB_MSG", DisplayCulture)) 'RJO 03/14/12
            'Call subCheckDbase() 'RJO 03/14/12

            'Get the list of background applications to be started
            bSuccess = bGetStartList()
            gcolBackgroundApps = New Collection(Of udsAppConfig)

            'Do this so other apps can get PW4_Main's MainWindowHandle
            Me.Show()
            Do While Process.GetCurrentProcess.MainWindowHandle.ToInt32 = 0
                Application.DoEvents()
            Loop

            'Start any applications that need to start before the PW Desktop is created
            Call subStartPWApps(eStartGroup.PreFRWM)

            'Create the PW Desktop
            Call mPWDesktop.Initialize()

            'If PW4_Main is running in the development environment, let the developer decide to create the
            'desktop.
            If InIDE Then
                Dim oResponse As DialogResult
                oResponse = MessageBox.Show("OK to create desktop?", "Debug", MessageBoxButtons.YesNo, _
                                                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
                If oResponse = Windows.Forms.DialogResult.No Then
                    mPWDesktop.OkToCreateDesktop = False
                    mbKeepDesktopOpen = True
                End If
            End If

            Call mPWDesktop.PostStatusMsg(gpsRM.GetString("psCREATE_DESKTOP_MSG", DisplayCulture))
            Call mPWDesktop.CreatePWDesktop()

            'Initial Tile mode is Cascade
            optTileMode0.Checked = True

            'Start any applications that need to start before PLC Communications are started
            Call subStartPWApps(eStartGroup.PostFRWM_PrePLC)

            '04/05/11 HGB Don't start the PLC watchdog for SA
            If (Not mcolZones.PaintShopComputer) And (Not mcolZones.StandAlone) Then
                Call subStartHotLinks()
            End If

            'Start the Password object
            'mPWDesktop.PostStatusMsg(gpsRM.GetString("psPWD_MGR_START_MSG", DisplayCulture))'RJO 03/09/12
            'moPassword = New PWPassword.PasswordObject 'RJO 03/09/12
            'Call mPassword.InitPassword(moPassword, mScreenPrivs, msUserName, gsSCREEN_NAME) 'RJO 03/09/12

            'Create the new PWUser (Password interface) Class
            moPassword = New clsPWUser 'RJO 03/09/12
            moPassword.LoadLogin() 'kdp 02/05/14

            'Display the current user
            Call subShowNewUser()

            'Start any applications that need to start before Robot Communications are started
            Call subStartPWApps(eStartGroup.PostPLC_PreFRRN)

            'Create the Controllers collection and set up the Robot Neighborhood if necessary
            Dim bMultiZone As Boolean = (mcolZones.Count > 1)

            '04/05/11 HGB Add multizone support for SA
            If mcolZones.MISComputer = False Then
                If bMultiZone And mcolZones.StandAlone Then
                    Dim sSaveZoneName As String
                    sSaveZoneName = mcolZones.CurrentZone
                    mcolControllers = New clsControllers(mcolZones, bMultiZone)
                    Dim colControllers As clsControllers
                    For Each z As clsZone In mcolZones
                        If z.Name <> sSaveZoneName Then
                            mcolZones.SetCurrentZone(z.Name)
                            colControllers = New clsControllers(mcolZones, bMultiZone)
                            For Each c As clsController In colControllers
                                mcolControllers.Add(c)
                            Next
                            colControllers = Nothing
                        End If
                    Next
                    mcolZones.SetCurrentZone(sSaveZoneName)
                Else
                    mcolControllers = New clsControllers(mcolZones, bMultiZone)
                End If

                For Each oController As clsController In mcolControllers
                    oController.subConnect()
                    Application.DoEvents()
                Next 'oController

                Dim nBail As Integer
                Do While mnConnAcks < mcolControllers.Count
                    Application.DoEvents()
                    Threading.Thread.Sleep(100)
                    Application.DoEvents()
                    nBail += 100
                    If nBail > (mcolControllers.Count * 2000) Then Exit Do
                Loop
                For Each oController As clsController In mcolControllers
                    Call mPWDesktop.PostStatusMsg(sGetConnStat(oController))
                Next 'oController
                'Initialize applicators
                Dim colArms As clsArms = LoadArmCollection(mcolControllers, False)
                Dim colApplicators As clsApplicators = New clsApplicators(colArms, mcolZones.ActiveZone, True)

            End If
            'Start the Alarm Manager
            'NVSP 12/14/2016 AlarmManager need not get started in paintshop compmuter
            If (Not mcolZones.StandAlone) And mcolZones.PaintShopComputer Then

            Else
                Call subStartAlarmManager()
            End If


            'Start any applications that need to start after the Robot Neighborhood is started
            Call subStartPWApps(eStartGroup.PostFRRN)
            'Save startup messages
            Dim sLogPath As String = String.Empty
            If GetDefaultFilePath(sLogPath, eDir.PAINTworks, String.Empty, gpsRM.GetString("psSTARTUPLOGTXT", DisplayCulture)) Then
            End If
            Call mPWDesktop.PostStatusMsg(gpsRM.GetString("psSTARTUP_CMP_MSG", DisplayCulture))

            Dim fileWriter As System.IO.StreamWriter = Nothing
            Try
                fileWriter = My.Computer.FileSystem.OpenTextFileWriter(sLogPath, False)
                For nItem As Integer = 0 To frmFirst.lstDebugOutput.Items.Count - 1
                    fileWriter.WriteLine(frmFirst.lstDebugOutput.Items(nItem).ToString)
                Next
                fileWriter.Close()
            Catch ex As Exception
                mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subStartPaintworks", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try

        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subStartPaintworks", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub subStartPWApps(ByVal Group As eStartGroup)
        '********************************************************************************************
        'Description: Starts any PW background applications belonging to the specified Group in
        '             the manner specified in the Configuration database's Startup List table.
        '
        'Parameters: Group - Start Group
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            If mxndStartList.Count > 0 Then
                Dim bStandalone As Boolean = mcolZones.StandAlone
                Dim bStartEnable As Boolean = True

                For Each oNode As XmlNode In mxndStartList
                    If CType(oNode.Item("StartGroup").InnerXml, eStartGroup) = Group Then
                        Dim AppConfig As udsAppConfig

                        With AppConfig
                            .LaunchString = oNode.Item("LaunchString").InnerXml
                            .ProcessName = String.Empty
                            .WindowStyle = eGetAppWinStyle(CType(oNode.Item("WindowStyle").InnerXml, Integer))
                            .Delay = CType(oNode.Item("Delay").InnerXml, Integer)
                            .StartupTimeout = CType(oNode.Item("StartupTimeout").InnerXml, Integer)
                            .IsVB6App = CType(oNode.Item("IsVB6App").InnerXml, Boolean)
                            .StandAlone = CType(oNode.Item("StandAlone").InnerXml, Boolean)
                            .AllowMultiple = CType(oNode.Item("AllowMultiple").InnerXml, Boolean)
                            .StartFlag = CType(oNode.Item("StartFlag").InnerXml, Boolean)
                            .CloseApp = CType(oNode.Item("CloseApp").InnerXml, Boolean)
                            .Message = oNode.Item("MessageTag").InnerXml

                            If .StartFlag Then
                                If Strings.InStr(.LaunchString, ":\") = 0 Then
                                    Dim sPath As String = String.Empty

                                    If .IsVB6App Then
                                        Call mPWCommon.GetDefaultFilePath(sPath, eDir.VB6Apps, String.Empty, String.Empty)
                                    Else
                                        Call mPWCommon.GetDefaultFilePath(sPath, eDir.VBApps, String.Empty, String.Empty)
                                    End If

                                    .ProcessName = Strings.Left(.LaunchString, Strings.Len(.LaunchString) - 4)
                                    .LaunchString = sPath & .LaunchString

                                    If Not .IsVB6App Then
                                        'Pass the culture in as a commandline parameter to .NET background apps
                                        .LaunchString = .LaunchString & " /culture=" & msCulture
                                    End If
                                Else
                                    'Non-Paintworks app. Parse the executable name
                                    For nLen As Integer = 6 To Strings.Len(.LaunchString)
                                        If Strings.Left(Strings.Right(.LaunchString, nLen), 1) = "\" Then
                                            .ProcessName = Strings.Right(.LaunchString, nLen - 1)
                                            .ProcessName = Strings.Left(.ProcessName, Strings.Len(.ProcessName) - 4)
                                        End If
                                    Next
                                End If

                                bStartEnable = (Not bStandalone) Or (.StandAlone And bStandalone)
                                If bStartEnable Then
                                    If Not .AllowMultiple Then
                                        Dim oProcs() As Process = Process.GetProcessesByName(.ProcessName)

                                        If oProcs.Length > 0 Then
                                            'fill in the blanks in AppConfig with info from the running App
                                            Dim oProc As Process = oProcs(0)

                                            .ProcessId = oProc.Id
                                            .hWnd = oProc.MainWindowHandle.ToInt32
                                            bStartEnable = False
                                        End If
                                    End If 'Not .AllowMultiple
                                End If

                                If bStartEnable Then
                                    'OK for Launch
                                    Call subLaunchBackgroundApp(AppConfig)
                                    gcolBackgroundApps.Add(AppConfig)
                                End If

                            End If

                        End With 'StartConfig

                    End If 'CType(DR.Item...
                Next 'DR

            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subStartPWApps", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subUpdateDateTime()
        '********************************************************************************************
        'Description: Updates the displayed Time and Date. 
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        tmrClock.Enabled = False
        lblDate.Text = Format(TimeString, "Short Time") & vbCr & Format(DateString, "Long Date")
        tmrClock.Enabled = True

    End Sub

#End Region

#Region " Event Handlers "

    Private Sub btnPrintScreen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrintScreen.Click
        '********************************************************************************************
        'Description: Capture an image of the entire screen and send it to the printer.
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Try

            Dim oSC As New ScreenShot.ScreenCapture
            Dim imgScreen As Image = oSC.CaptureScreen

            mbmpScreen = CType(imgScreen, Bitmap)
            mpdScreenShot = New Printing.PrintDocument

            With mpdScreenShot
                .DefaultPageSettings.Margins.Top = 50
                .DefaultPageSettings.Margins.Bottom = 50
                .DefaultPageSettings.Margins.Left = 50
                .DefaultPageSettings.Margins.Right = 50
                .DefaultPageSettings.Landscape = True
                .Print()
            End With

        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: btnPrintScreen_Click", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub btnUserLogOn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUserLogOn.Click
        '********************************************************************************************
        'Description: Log the user in or out.
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Try
            Select Case btnUserLogOn.Text

                Case gpsRM.GetString("psLOGON_BTN_TXT", DisplayCulture)
                    moPassword.DisplayLogin()
                Case gpsRM.GetString("psLOGOFF_BTN_TXT", DisplayCulture)
                    moPassword.LogUserOut()

            End Select

        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: btnUserLogOn_Click", _
    "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '********************************************************************************************
        'Description: trap close from taskbar
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Try

            Debug.Print(e.CloseReason.ToString)
            Select Case e.CloseReason
                Case CloseReason.UserClosing
                    If mbInShutdown = False Then
                        Dim bCancel As Boolean = False
                        ShutDownPaintworks(False, bCancel)
                        e.Cancel = bCancel
                    End If
            End Select
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: frmMain_FormClosing", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
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
        Try

            Dim sKeyValue As String = String.Empty

            'Trap Function Key presses
            If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
                Select Case e.KeyCode
                    Case Keys.F1
                        'Help Screen request
                        sKeyValue = "btnHelp"
                        subLaunchHelp(gs_HELP_MAIN, oIPC)
                    Case Keys.F2
                        'Screen Dump request
                        Dim oSC As New ScreenShot.ScreenCapture
                        Dim sDumpPath As String = String.Empty

                        mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                        oSC.CaptureWindowToFile(Me.Handle, sDumpPath & msSCREEN_DUMP_NAME, Imaging.ImageFormat.Jpeg)


                    Case Else

                End Select
            End If

            'Trap Print Screen request
            If e.Control And (Not (e.Alt Or e.Shift)) And e.KeyCode = Keys.P Then
                btnPrintScreen.PerformClick()
            End If

            If sKeyValue = String.Empty Then
                'Trap Main Menu HotKey presses
                If e.Alt And (Not (e.Control Or e.Shift)) Then
                    For Each oMenuButton As Windows.Forms.ToolStripButton In tlsMain.Items
                        If oMenuButton.Tag.ToString = e.KeyCode.ToString Then
                            sKeyValue = oMenuButton.Name
                            Exit For
                        End If
                    Next 'oMenuButton
                End If
            End If 'sKeyValue = String.Empty

            If sKeyValue <> String.Empty Then
                'Click the Menu Button
                If tlsMain.Items(sKeyValue).Enabled Then
                    tlsMain.Items(sKeyValue).PerformClick()
                End If
            End If

            e.SuppressKeyPress = True
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: frmMain_KeyDown", _
                "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description: Load Event
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '    07/01/10   MSW     Add some error management to connect with a restarted alarmman or alarmman running in debug
        '    09/14/10   MSW     pause to allow services to startup
        '    11/08/10   MSW     Move pause after frmFirst.show
        '********************************************************************************************
        Try

            Call subCheckForPreviousInstance()
            Call subInitializeForm()
            tmrClock.Enabled = True
            Call subStartPaintworks()

            If mbLoadError Then
                frmFirst.LoadError = True
            Else
                frmFirst.Close()
            End If

            For Each BackgroundApp As udsAppConfig In gcolBackgroundApps
                If BackgroundApp.ProcessName.ToLower = "alarmman" Then
                    Try
                        'mWorksComm.SendFRWMMessage("initseverity,0,0,0,0,0", BackgroundApp.ProcessId)

                        Dim sMessage(0) As String
                        sMessage(0) = "initseverity"
                        oIPC.WriteControlMsg(gs_COM_ID_ALARM_MAN, sMessage)
                    Catch ex As Exception
                        subStartAlarmManager()
                        'mWorksComm.SendFRWMMessage("initseverity,0,0,0,0,0", BackgroundApp.ProcessId)
                        Dim sMessage(0) As String
                        sMessage(0) = "initseverity"
                        oIPC.WriteControlMsg(gs_COM_ID_ALARM_MAN, sMessage)
                    End Try

                    Exit For
                End If
            Next
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: frmMain_Load", _
                "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub frmMain_LocationChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LocationChanged
        '********************************************************************************************
        'Description: Resize the ToolStrip to fill in the blank space after the form has been
        '             sized and located.
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************

        tlsMain.Width = pnlToolBar.Left + 36 'magic number

    End Sub

    Private Sub frmMain_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        '********************************************************************************************
        'Description: Store the PW4_Main MainWindowHandle away for future use.
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Try

            Dim oProc As Process = Process.GetCurrentProcess

            mnPW4_MainHwnd = oProc.MainWindowHandle.ToInt32
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: frmMain_Shown", _
                "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub lblDate_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lblDate.MouseMove
        '********************************************************************************************
        'Description: Makes the Window Tile panel visible (temporarily)
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            tmrHideTilePanel.Enabled = False
            pnlWindowTile.Visible = True
            tmrHideTilePanel.Enabled = True
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: lblDate_MouseMove", _
                "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub lblFanuc_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lblFanuc.MouseDown
        '********************************************************************************************
        'Description:  This Sub allows an operator with administrator level priviledge to access the
        '              Windows desktop (and any other neat features we choose to incorporate in the
        '              future) while Paintworks is running. To access a feature, the operator types 
        '              the feature name in the textbox that is displayed when the InputBox is shown.
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' MSW         04/23/10      Add a command prompt
        ' MSW         07/01/10      Add some tools to the right-click on the banner
        ' MSW         09/13/13      Add devices control panel page for USB eject
        '********************************************************************************************


        Try
            If e.Button = MouseButtons.Right Then
                If InIDE Then
                    'Dim oResponse As DialogResult

                    'oResponse = MessageBox.Show("OK to create desktop?", "Debug", MessageBoxButtons.YesNo, _
                    '                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
                    'If oResponse = Windows.Forms.DialogResult.No Then
                    mPWDesktop.OkToCreateDesktop = False
                    mbKeepDesktopOpen = True
                    'End If
                End If
                If (mPWDesktop.PWDesktopExists) Or (mbKeepDesktopOpen = True) Then
                    'Make sure this user has Administrate priviledge before showing the inputbox
                    'If CheckPassword(gsSCREEN_NAME, "Administrate") Then 'RJO 03/14/12
                    If moPassword.CheckPassword(ePrivilege.Administrate) Then
                        Dim sReply As String = InputBox(gpsRM.GetString("psPW_ADMIN_FEATURES_TXT", _
                                                        DisplayCulture), String.Empty)
                        Dim sChangeLogText As String = gpsRM.GetString("psEXECUTED", DisplayCulture)
                        Dim sSplitReply() As String = Split(sReply, " ")
                        'TODO - it would be nice if this was configurable
                        Select Case sSplitReply(0).ToLower
                            Case "desktop", "explorer"
                                If Not (mPWDesktop.PWDesktopExists) Then 'close it
                                    'ReCreate the PWdesktop. Make sure the Login window isn't showing.
                                    moPassword.HideLogin()
                                    'TODO - Code to close all of the open Explorer windows
                                    Call mPWDesktop.CreatePWDesktop()
                                    mbKeepDesktopOpen = False
                                Else
                                    Call mPWDesktop.DestroyPWDesktop()
                                    Shell("explorer.exe", AppWinStyle.NormalFocus)
                                    sChangeLogText = sChangeLogText & "explorer.exe"
                                    If sSplitReply.GetUpperBound(0) > 0 Then
                                        If sSplitReply(1).ToLower = "keepopen" Or sSplitReply(1).ToLower = "ko" Then
                                            mbKeepDesktopOpen = True
                                        End If
                                    End If
                                End If
                            Case "close", "hide"
                                If Not (mPWDesktop.PWDesktopExists) Then 'close it
                                    'ReCreate the PWdesktop. Make sure the Login window isn't showing.
                                    moPassword.HideLogin()
                                    'TODO - Code to close all of the open Explorer windows
                                    Call mPWDesktop.CreatePWDesktop()
                                    mbKeepDesktopOpen = False
                                End If
                            Case "menueditor"
                                'TODO - Need a menu editor for PW4
                                'Dim sPath As String = String.Empty

                                'If mPWCommon.GetDefaultFilePath(sPath, eDir.VB6Apps, String.Empty, "menu editor.exe") Then
                                '    Shell(sPath & "menu editor.exe", AppWinStyle.NormalFocus)
                                '    sChangeLogText = sChangeLogText & "menu editor.exe"
                                'Else
                                sChangeLogText = String.Empty
                                'End If
                            Case "telnet"
                                Dim sTmp As String = String.Empty
                                If sSplitReply.GetUpperBound(0) > 0 Then
                                    For nIdx As Integer = 1 To sSplitReply.GetUpperBound(0)
                                        sTmp = sTmp & " " & sSplitReply(nIdx)
                                    Next
                                End If
                                Shell("telnet.exe" & sTmp, AppWinStyle.NormalFocus)
                                sChangeLogText = sChangeLogText & "telnet.exe"
                            Case "scatteredaccess", "sa"
                                If sSplitReply.GetUpperBound(0) > 0 Then
                                    Dim sMessage(1) As String
                                    sMessage(1) = String.Empty
                                    Select Case sSplitReply(0).ToLower
                                        Case "open", "show"
                                            ScatteredAccessRoutines.subStartupSA()
                                            sMessage(0) = "show"
                                            oIPC.WriteControlMsg(gs_COM_ID_SA, sMessage)
                                        Case "hide"
                                            ScatteredAccessRoutines.subStartupSA()
                                            sMessage(0) = "hide"
                                            oIPC.WriteControlMsg(gs_COM_ID_SA, sMessage)
                                        Case "close", "shutdown"
                                            sMessage(0) = "close"
                                            oIPC.WriteControlMsg(gs_COM_ID_SA, sMessage)
                                        Case "restart"
                                            sMessage(0) = "close"
                                            oIPC.WriteControlMsg(gs_COM_ID_SA, sMessage)
                                            Threading.Thread.Sleep(1000)
                                            ScatteredAccessRoutines.subStartupSA()
                                        Case Else
                                            ScatteredAccessRoutines.subStartupSA()
                                            sMessage(0) = "show"
                                            oIPC.WriteControlMsg(gs_COM_ID_SA, sMessage)
                                    End Select
                                Else
                                    ScatteredAccessRoutines.subStartupSA()
                                    Dim sMessage(1) As String
                                    sMessage(0) = "show"
                                    sMessage(1) = String.Empty
                                    oIPC.WriteControlMsg(gs_COM_ID_SA, sMessage)
                                End If
                            Case "usb", "devices"
                                'Win7 "Devices and printers" control panel object,
                                'Can be used for USB eject, printer select
                                'shortcut:%25systemroot%25\system32\control.exe /name Microsoft.DevicesAndPrinters
                                Shell(Environment.ExpandEnvironmentVariables("%systemroot%\system32\control.exe /name Microsoft.DevicesAndPrinters"), AppWinStyle.NormalFocus)
                            Case "services"
                                Shell(Environment.ExpandEnvironmentVariables("%windir%\system32\services.msc"), AppWinStyle.NormalFocus)
                                sChangeLogText = sChangeLogText & "services.msc"
                            Case "command", "cmd"
                                Shell(Environment.ExpandEnvironmentVariables("%ComSpec%"), AppWinStyle.NormalFocus)
                                sChangeLogText = sChangeLogText & "cmd.exe"
                            Case "sql"
                                Shell("%ProgramFiles%\Microsoft SQL Server\90\Tools\Binn\VSShell\Common7\IDE\ssmsee.exe", AppWinStyle.NormalFocus)
                                sChangeLogText = sChangeLogText & "SQL Server manager"
                            Case "events"
                                Shell(Environment.ExpandEnvironmentVariables("%SystemRoot%\system32\eventvwr.exe"), AppWinStyle.NormalFocus)
                                sChangeLogText = sChangeLogText & "Event Viewer"
                            Case "versions", "version"
                                Shell(Environment.ExpandEnvironmentVariables("%ProgramFiles%\FANUC\Shared\Utilities\frversioninfo.exe"), AppWinStyle.NormalFocus)
                                sChangeLogText = sChangeLogText & "FANUC Software Versions"
                            Case "license"
                                Shell(Environment.ExpandEnvironmentVariables("%ProgramFiles%\FANUC\Shared\Utilities\FRLicenseManager.exe"), AppWinStyle.NormalFocus)
                                sChangeLogText = sChangeLogText & "FANUC Software License"
                            Case "anybus", "SMC"
                                Shell(Environment.ExpandEnvironmentVariables("%ProgramFiles%\HMS\Anybus IPconfig\Anybus IPconfig.exe"), AppWinStyle.NormalFocus)
                                sChangeLogText = sChangeLogText & "Anybus IP Config"
                            Case "hirschmann", "discovery", "switch"
                                Shell(Environment.ExpandEnvironmentVariables("%ProgramFiles%\Hirschmann\HiDiscovery 2.04\bin\HiDiscovery.exe"), AppWinStyle.NormalFocus)
                                sChangeLogText = sChangeLogText & "Hirschmann Discovery"
                            Case "keeplogging"
                                ShutDownPaintworks(True)
                            Case "init", "robot", "applicator", "applicators", "appl"
                                Dim colControllers As clsControllers = New clsControllers(mcolZones, False)
                                Dim colArms As clsArms = LoadArmCollection(colControllers, False)
                                Dim colApplicators As clsApplicators = New clsApplicators(colArms, mcolZones.ActiveZone, True)
                            Case "maint", "pwmaint"
                                Dim sPath As String = String.Empty
                                If mPWCommon.GetDefaultFilePath(sPath, eDir.VBApps, String.Empty, "PW_Maint.exe") Then
                                    Shell(sPath & " manual", AppWinStyle.NormalFocus)
                                    sChangeLogText = sChangeLogText & "PW_Maint.exe"
                                Else
                                    sChangeLogText = String.Empty
                                End If
                            Case "shutdown"
                                Dim sTmp As String = String.Empty
                                If sSplitReply.GetUpperBound(0) > 0 Then
                                    For nIdx As Integer = 1 To sSplitReply.GetUpperBound(0)
                                        Dim sMessage(0) As String
                                        sMessage(0) = "close"
                                        oIPC.WriteControlMsg(sSplitReply(nIdx), sMessage) 
                                    Next
                                End If
                            Case Else
                                sChangeLogText = String.Empty
                        End Select

                        If sChangeLogText <> String.Empty Then
                            mPWCommon.AddChangeRecordToCollection(gsSCREEN_NAME, LoggedOnUser, _
                                                                  mcolZones.CurrentZoneNumber, _
                                                                  String.Empty, String.Empty, _
                                                                  sChangeLogText, mcolZones.CurrentZone)
                            'mPWCommon.SaveToChangeLog(mcolZones.ActiveZone.DatabasePath)
                            mPWCommon.SaveToChangeLog(mcolZones.ActiveZone)
                        End If
                    Else
                        'ReCreate the PWdesktop. Make sure the Login window isn't showing.
                        moPassword.HideLogin()
                        'TODO - Code to close all of the open Explorer windows
                        Call mPWDesktop.CreatePWDesktop()
                        mbKeepDesktopOpen = False
                    End If 'CheckPassword
                Else
                    'ReCreate the PWdesktop. Make sure the Login window isn't showing.
                    moPassword.HideLogin()
                    'TODO - Code to close all of the open Explorer windows
                    Call mPWDesktop.CreatePWDesktop()
                End If
            Else
                frmAbout.SiteName = mcolZones.SiteName
                frmAbout.Show()
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: lblFanuc_MouseDown", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub mcolControllers_ConnectionStatusChange(ByVal Controller As clsController) Handles mcolControllers.ConnectionStatusChange
        '********************************************************************************************
        'Description:  When the number of controller connection status change events matches the 
        '              number of controllers, subStartPaintworks can then get connection status
        '              for display on frmFirst.
        '
        'Parameters: Controller whose connection status changed
        'Returns: None
        '
        'Modification history:
        '
        'Date      By      Reason
        '******************************************************************************************** 
        If Controller.RCMConnectStatus = FRRobotNeighborhood.FRERNConnectionStatusConstants.frRNConnected Then
            mnConnAcks += 1
        End If

    End Sub

    Private Sub moPassword_LogIn() Handles moPassword.LogIn
        '********************************************************************************************
        'Description: A new Paintworks User Logged In.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Try
            Dim sRunningApps As String = mPWDesktop.RunningApps

            If sRunningApps <> String.Empty Then
                Dim sApps() As String = Strings.Split(sRunningApps, ",")
                'Let running PW Apps know that a user just logged in 'RJO 03/14/12
                For Each sProcName As String In sApps
                    Dim sMessage() As String = {moPassword.ProcessName, "login", "", "", moPassword.UserName, moPassword.LastLogIn}

                    oIPC.WritePasswordMsg(sProcName, sMessage)
                    Threading.Thread.Sleep(200)
                Next 'sProcName 
            End If
            'This is needed with old password object raising events and can hopefully be removed when 
            'password is redone
            'Control.CheckForIllegalCrossThreadCalls = False
            mLoginTime = Now
            Call subShowNewUser()
            'Control.CheckForIllegalCrossThreadCalls = True
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: moPassword_LogIn", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub moPassword_LogOut() Handles moPassword.Logout
        '********************************************************************************************
        'Description: The current Paintworks User Logged Out.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Try
            Dim sRunningApps As String = mPWDesktop.RunningApps

            If sRunningApps <> String.Empty Then
                Dim sApps() As String = Strings.Split(sRunningApps, ",")
                'Let running PW Apps know that a user just logged out 'RJO 03/14/12
                For Each sProcName As String In sApps
                    Dim sMessage() As String = {moPassword.ProcessName, "logout"}

                    oIPC.WritePasswordMsg(sProcName, sMessage)
                    Threading.Thread.Sleep(200)
                Next 'sProcName 
            End If
            'This is needed with old password object raising events and can hopefully be removed when 
            'password is redone
            'Control.CheckForIllegalCrossThreadCalls = False
            Call subShowNewUser()
            'Control.CheckForIllegalCrossThreadCalls = True

        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: moPassword_LogOut", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

        End Try

    End Sub

    Private Sub moPassword_Result(ByVal Action As String, ByVal ReturnValue As String) Handles moPassword.Result
        '********************************************************************************************
        'Description: Result returned by Password.NET for Check 3rd party application Launch 
        '             Privilege operation
        '
        'Parameters: Action - operation name, ReturnValue - returned status/data
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Select Case Action.ToLower
                Case "launchpriv"
                    If ReturnValue.ToLower = "none" Then
                        'no privilege.
                        mbPrivilegeChecked = False
                        MessageBox.Show(gpsRM.GetString("psNO_ACCESS", DisplayCulture) & mLaunchConfig.LaunchFile, _
                                                        gsSCREEN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Else
                        With mLaunchConfig
                            Call LaunchPWApp(.LaunchFile, .UseFRWM, .LaunchFlags, .CommandLine, .IsVB6App, .UsePassword)
                        End With
                    End If

                Case Else

            End Select


        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: moPassword_Result", _
           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub moPLC_ModuleError(ByVal nErrNum As Integer, ByVal sErrDesc As String, ByVal sModule As String, ByVal AdditionalInfo As String) Handles moPLC.ModuleError
        '********************************************************************************************
        'Description: A PLC Communication error has occurred.
        '
        'Parameters: ZoneName, TagName, Data
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        mDebug.WriteEventToLog(sModule, "Error: " & nErrNum.ToString & " - " & sErrDesc & " [" & AdditionalInfo & "]")

    End Sub

    Private Sub moPLC_NewData(ByVal ZoneName As String, ByVal TagName As String, ByVal Data() As String) Handles moPLC.NewData
        '********************************************************************************************
        'Description: PLC Hotlink data has changed.
        '
        'Parameters: ZoneName, TagName, Data
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Select Case TagName

                Case "PLCWatchdog"
                    If mbPLCDisconnected Then
                        'Reset the error message
                        With lblFanuc
                            .Text = gpsRM.GetString("psFANUC_LBL_TXT", DisplayCulture)
                            .BackColor = Color.Yellow
                            .ForeColor = Color.Red
                        End With
                    End If
                    mbPLCDisconnected = False
                    'reset the watchdog timer
                    tmrPLCWatchDog.Enabled = False
                    tmrPLCWatchDog.Enabled = True

                Case Else

            End Select
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: moPLC_NewData", _
                "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub mpdScreenShot_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles mpdScreenShot.PrintPage
        '********************************************************************************************
        'Description:  Sizes the Screen Shot bitmap to the paper size (minus the border widths) and
        '              prints it.
        '
        'Parameters: Sender, PrintPageEventArgs
        'Returns:    none
        '
        'Modification(history)
        '
        'Date      By      Reason
        '******************************************************************************************** 
        Try
            Dim recPrint As Rectangle

            With recPrint
                .X = (e.PageBounds.Width - e.MarginBounds.Width) \ 2
                .Y = (e.PageBounds.Height - e.MarginBounds.Height) \ 2
                .Width = e.MarginBounds.Width
                .Height = e.MarginBounds.Height
            End With

            e.Graphics.DrawImage(mbmpScreen, recPrint)

            'There's only one page
            e.HasMorePages = False
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: mpdScreenShot_PrintPage", _
                "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    'Private Sub mPWMessenger_NewMessage(ByVal Message As String) Handles mPWMessenger.NewMessage
    '    '********************************************************************************************
    '    'Description:   Event that is raised when a message is received.
    '    '
    '    'Parameters:    None
    '    'Returns:       None
    '    '
    '    'Modification history:
    '    '
    '    ' By          Date          Reason
    '    '********************************************************************************************
    '    Dim nDelimPos As Integer
    '    Dim sCommand As String = String.Empty
    '    Dim sArgument As String = String.Empty

    '    mPWMessenger.SendAcknowledge(Message)

    '    Application.DoEvents()

    '    'Parse the message
    '    nDelimPos = Strings.InStr(Message, ",")

    '    If nDelimPos > 0 Then
    '        sCommand = Strings.Trim(Strings.Mid(Message, 1, nDelimPos - 1))
    '        sArgument = Strings.Trim(Strings.Mid(Message, nDelimPos + 1))
    '    Else
    '        sCommand = Strings.Trim(Message)
    '    End If 'nDelimPos > 0

    '    Trace.WriteLine("mPWMessenger.NewMessage: Command = [" & sCommand & "], Argument = [" & sArgument & "]")

    '    Select Case sCommand.ToLower

    '        Case "schedmanload", "hoteditlogrload", "scattaccessmanload", _
    '             "sealerdiagschedulerload", "dmonlogrload", "visnlogrload"
    '            'Set load complete flag and load-error status flag
    '            mbBackgroundAppStarted = True
    '            If sArgument.ToLower = "false" Then
    '                mbLoadError = True
    '            End If

    '        Case "poststatusmsg"
    '            'Queue up status message to frmFirst.lstDebugOutput
    '            Dim oMsg As New udsStartupMsg

    '            oMsg.Message = sArgument
    '            oMsg.Posted = False
    '            If mStatusMsgs Is Nothing Then
    '                ReDim mStatusMsgs(0)
    '            Else
    '                ReDim Preserve mStatusMsgs(mStatusMsgs.GetUpperBound(0) + 1)
    '            End If
    '            mStatusMsgs(mStatusMsgs.GetUpperBound(0)) = oMsg

    '        Case Else
    '            'Test message for DEBUG
    '            If InIDE Then
    '                MessageBox.Show("NewMessage from PWMessenger: Command = [" & sCommand & "], Argement = [" & sArgument & "]", "Debug", MessageBoxButtons.OK)
    '            End If

    '    End Select

    'End Sub

    Public Sub New()
        '********************************************************************************************
        'Description: Form Class constuctor.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        'Call subCheckForPreviousInstance()

    End Sub

    Private Sub optTileMode_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                           Handles optTileMode0.CheckedChanged, _
                                                   optTileMode1.CheckedChanged, _
                                                   optTileMode2.CheckedChanged
        '********************************************************************************************
        'Description: Set the Window Tile Mode according to the user's selection.
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            Dim oControl As RadioButton = DirectCast(sender, RadioButton)

            If oControl.Checked Then
                Select Case oControl.Name
                    Case "optTileMode0"
                        mPWDesktop.TileMode = mPWDesktop.eTile.Cascade
                    Case "optTileMode1"
                        mPWDesktop.TileMode = mPWDesktop.eTile.Horizontal
                    Case "optTileMode2"
                        mPWDesktop.TileMode = mPWDesktop.eTile.Vertical
                End Select
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: optTileMode_CheckedChanged", _
                "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    'Private Sub tlsMain_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tlsMain.ItemClicked
    '    '********************************************************************************************
    '    'Description: A Menu Button was clicked. Show the submenu or Alarms screen.
    '    '
    '    'Parameters: sender, eventargs
    '    'Returns:    none
    '    '
    '    'Modification history:
    '    '
    '    ' Date      By      Reason
    '    '******************************************************************************************** 
    '    Try
    '        If frmMenu.Visible Then
    '            frmMenu.Close()
    '        End If

    '        Select Case e.ClickedItem.Name

    '            Case "btnAlarms"
    '                'Show/Hide Alarm Screen
    '                For Each BackgroundApp As udsAppConfig In gcolBackgroundApps
    '                    If BackgroundApp.ProcessName.ToLower = "alarmman" Then
    '                        mWorksComm.SendFRWMMessage("maximize,0,0,0,0,0", BackgroundApp.ProcessId)
    '                        Exit For
    '                    End If
    '                Next

    '            Case Else
    '                'Show the SubMenu for the button that was clicked
    '                Dim oMenuLocation As Drawing.Point
    '                Dim sAppFilePath As String = String.Empty

    '                With frmMenu

    '                    'Set the Menu Location
    '                    oMenuLocation.X = e.ClickedItem.Bounds.Left
    '                    oMenuLocation.Y = grecWorkingArea.Top 'e.ClickedItem.Bounds.Bottom
    '                    .MenuLocation = oMenuLocation

    '                    'Specify the paths to the SubMenu Configuration file and the EXE files
    '                    .MenuConfigFilePath = mcolZones.Item(mcolZones.CurrentZone).XMLPath

    '                    Call mPWCommon.GetDefaultFilePath(sAppFilePath, eDir.VBApps, String.Empty, String.Empty)
    '                    .AppFilePath = sAppFilePath
    '                    Call mPWCommon.GetDefaultFilePath(sAppFilePath, eDir.VB6Apps, String.Empty, String.Empty)
    '                    .VB6AppFilePath = sAppFilePath

    '                    'Specify which SubMenu to show
    '                    .MenuName = e.ClickedItem.Name

    '                    .Show()

    '                End With 'frmMenu

    '        End Select

    '    Catch ex As Exception
    '        mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: tlsMain_ItemClicked", _
    '                               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
    '    End Try

    'End Sub

    Private Sub tmrCheckTile_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrCheckTile.Tick
        '********************************************************************************************
        'Description: If apps are tiled, there is no way to intercept the close message from the
        '             child app. This is to check if a re-tile is required.
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            tmrCheckTile.Enabled = False
            Call mPWDesktop.CheckWindowTile()
            tmrCheckTile.Enabled = True
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: tmrCheckTile_Tick", _
                "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub tmrClock_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrClock.Tick
        '********************************************************************************************
        'Description: Periodically updates the displayed Time and Date. Hijacked to also display
        '             Startup status messages from PWMessenger NewMessage event. This is a work
        '             around because NewMessage event handler can't post messages directly. Different 
        '             thread?   
        '
        'Parameters: sender, eventargs
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If (mPWDesktop.FormFirstVisible) And (Not mStatusMsgs Is Nothing) Then
                Dim nItems As Integer = mStatusMsgs.GetUpperBound(0)

                'Post application Status messages
                For nItem As Integer = 0 To nItems
                    If Not mStatusMsgs(nItem).Posted Then
                        Call mPWDesktop.PostStatusMsg(mStatusMsgs(nItem).Message)
                        mStatusMsgs(nItem).Posted = True
                    End If
                Next
            End If

            Call subUpdateDateTime()
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: tmrClock_Tick", _
                "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub tmrHideTilePanel_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrHideTilePanel.Tick
        '********************************************************************************************
        'Description: Makes the Window Tile panel invisible after the timeout interval has elapsed
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        pnlWindowTile.Visible = False

    End Sub

    Private Sub tmrPLCWatchDog_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrPLCWatchDog.Tick
        '********************************************************************************************
        'Description: PLC Communication has timed out. Show error message in banner.
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            mbPLCDisconnected = True
            With lblFanuc
                .Text = gpsRM.GetString("psPLC_COMM_DISC_TXT", DisplayCulture)
                .BackColor = Color.Black
                .ForeColor = Color.Red
            End With
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: tmrPLCWatchDog_Tick", _
                "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub tmrRingBell_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrRingBell.Tick
        '********************************************************************************************
        'Description: Alarm Severity Level is "red". Alternate the Alarm Bell picture to get the
        '             operator's attention.
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            tmrRingBell.Enabled = False

            If mbAlarmBellState Then
                tlsMain.Items("btnAlarms").Image = DirectCast(gpsRM.GetObject("Redbell2", DisplayCulture), Drawing.Image)
            Else
                tlsMain.Items("btnAlarms").Image = DirectCast(gpsRM.GetObject("Redbell1", DisplayCulture), Drawing.Image)
            End If

            mbAlarmBellState = Not mbAlarmBellState
            tmrRingBell.Enabled = True
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: tmrRingBell_Tick", _
                "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    '********New program-to-program communication object******************************************
    Private Sub oIPC_NewMessage(ByVal Schema As String, ByVal DS As DataSet) Handles oIPC.NewMessage
        '********************************************************************************************
        'Description:  A new message has been received from another Paintworks Application
        '
        'Parameters: DS - PWUser Dataset, ProcessName Paintworks screen process name
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If Me.InvokeRequired Then
                Dim dNewMessage As New NewMessage_CallBack(AddressOf oIPC_NewMessage)
                Me.BeginInvoke(dNewMessage, New Object() {Schema, DS})
            Else
                Dim DR As DataRow = DS.Tables(Paintworks_IPC.clsInterProcessComm.sTABLE).Rows(0)

                Select Case Schema.ToLower
                    Case oIPC.CONTROL_MSG_SCHEMA
                        Call subDoScreenAction(DR)
                    Case oIPC.PASSWORD_MSG_SCHEMA
                        Call moPassword.ProcessPasswordMessage(DR)
                    Case Else
                        mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: oIPC_NewMessage", _
                           "Unrecognized schema [" & Schema & "].")
                End Select
            End If 'Me.InvokeRequired
        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: oIPC_NewMessage", _
                "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
    '********************************************************************************************

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
    '                    mDebug.WriteEventToLog("PW4_Main frmMain.WndProc", "Received message [" & msWinMsg & "]")
    '                    mnMsgLen = 0
    '                End If
    '            End If

    '        End If 'WinMessage.Msg = WM_NULL
    '        MyBase.WndProc(WinMessage)

    '    Catch ex As Exception
    '        mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: WndProc", _
    '                               "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
    '    End Try

    'End Sub

#End Region

    Private Sub btnAlarms_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles btnAlarms.MouseDown, _
        btnConfig.MouseDown, btnHelp.MouseDown, btnMaintenance.MouseDown, _
        btnOperate.MouseDown, btnProcess.MouseDown, btnReports.MouseDown, _
        btnUtilities.MouseDown, btnView.MouseDown
        '********************************************************************************************
        'Description: A Menu Button was clicked. Show the submenu or Alarms screen.
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/04/09  MSW     Switch from tlsMain_ItemClicked to btn..._MouseDown event for smoother menus
        '                   It helped a little, I ended up using btn..._MouseEnter to grab the focus.  
        '                   That seems to help
        ' 07/01/10  MSW     Add some error management to connect with a restarted alarmman or alarmman running in debug
        '******************************************************************************************** 
        Try
            If frmMenu.Visible Then
                frmMenu.Close()
            End If
            Dim btnTmp As ToolStripItem = DirectCast(sender, ToolStripItem)
            Select Case btnTmp.Name

                Case "btnAlarms"
                    'Show/Hide Alarm Screen
                    For Each BackgroundApp As udsAppConfig In gcolBackgroundApps
                        If BackgroundApp.ProcessName.ToLower = "alarmman" Then
                            Try
                                'mWorksComm.SendFRWMMessage("maximize,0,0,0,0,0", BackgroundApp.ProcessId)
                                Dim sMessage(0) As String
                                sMessage(0) = "maximize"
                                oIPC.WriteControlMsg(gs_COM_ID_ALARM_MAN, sMessage)
                            Catch ex As Exception
                                subStartAlarmManager()
                                'mWorksComm.SendFRWMMessage("maximize,0,0,0,0,0", BackgroundApp.ProcessId)
                                Dim sMessage(0) As String
                                sMessage(0) = "maximize"
                                oIPC.WriteControlMsg(gs_COM_ID_ALARM_MAN, sMessage)
                            End Try
                            Exit For
                        End If
                    Next

                Case Else
                    'Show the SubMenu for the button that was clicked
                    Dim oMenuLocation As Drawing.Point
                    Dim sAppFilePath As String = String.Empty

                    With frmMenu

                        'Set the Menu Location
                        oMenuLocation.X = btnTmp.Bounds.Left
                        oMenuLocation.Y = grecWorkingArea.Top 'e.ClickedItem.Bounds.Bottom
                        .MenuLocation = oMenuLocation

                        Dim sXMLFilePath As String = Nothing
                        GetDefaultFilePath(sXMLFilePath, mPWCommon.eDir.XML, String.Empty, "")
                        .MenuConfigFilePath = sXMLFilePath
                         'Specify the paths to the SubMenu Configuration file and the EXE files
                        '.MenuConfigFilePath = mcolZones.Item(mcolZones.CurrentZone).XMLPath

                        Call mPWCommon.GetDefaultFilePath(sAppFilePath, eDir.VBApps, String.Empty, String.Empty)
                        .AppFilePath = sAppFilePath
                        Call mPWCommon.GetDefaultFilePath(sAppFilePath, eDir.VB6Apps, String.Empty, String.Empty)
                        .VB6AppFilePath = sAppFilePath

                        'Specify which SubMenu to show
                        .MenuName = btnTmp.Name

                        .Show()

                    End With 'frmMenu

            End Select

        Catch ex As Exception
            mDebug.WriteEventToLog(gsSCREEN_NAME & " Module: " & msMODULE & " Routine: tlsMain_ItemClicked", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try


    End Sub


    Private Sub btnAlarms_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAlarms.MouseEnter, _
        btnConfig.MouseEnter, btnHelp.MouseEnter, btnMaintenance.MouseEnter, _
        btnOperate.MouseEnter, btnProcess.MouseEnter, btnReports.MouseEnter, _
        btnUtilities.MouseEnter, btnView.MouseEnter
        '********************************************************************************************
        'Description: The mouse moves over a button
        '
        'Parameters: NA
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/04/09  MSW     Switch from tlsMain_ItemClicked to btn..._MouseDown event for smoother menus
        '                   It helped a little, I ended up using btn..._MouseEnter to grab the focus.  
        '                   That seems to help
        '******************************************************************************************** 
        Me.Focus()
    End Sub

End Class
