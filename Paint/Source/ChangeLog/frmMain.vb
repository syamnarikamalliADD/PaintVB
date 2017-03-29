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
' Form/Module: frmChangeLog
'
' Description: Display changes from database
' 
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
'    12/03/09   MSW     Switch to HTML print class
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'    04/16/13   MSW     Standalone ChangeLog form, Isolate DB,SQL dependencies        4.01.05.00
'                       Consolidate eParamType in mDeclares
'    06/05/13   RJO     Bug fix in subSaveData. Empty fields in some data rows were   4.01.05.01
'                       DBNull causing .Append operation to barf.
'    07/08/13   MSW     Run screen names through language files                       4.01.05.02
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up 4.01.05.03
'    09/30/13   MSW     Save screenshots as jpegs                                     4.01.05.04
'    10/08/13   MSW     Change screenshot name                                        4.01.06.00
'    12/18/13   MSW     Enable save and print to file if this screen gets opened      4.01.06.01
'                       under any password
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call              4.01.07.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports Response = System.Windows.Forms.DialogResult
Imports System.Configuration.ConfigurationSettings
Imports System.Text
Imports System.IO
'Custom StartupModule to pass command line to the already running screen
Friend Class frmMain

#Region " Declares "
    '******** Form Constants   **********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Friend Const msSCREEN_NAME As String = "ChangeLog"   ' <-- For password area change log etc.
    Public Const msSCREEN_DUMP_NAME As String = "ChangeLogSummary"
    Private Const mnMAININX As Integer = 100
    Private Const mnSCREENINX As Integer = 101
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    Private msALL As String = "All"
    Private msCulture As String = "en-US" 'Default to English

    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    '9.5.07 remove unnecessary initializations per fxCop
    Private mbRemoteZone As Boolean = False
    Private msOldRobot As String = String.Empty
    Private msOldParam As String = String.Empty
    Private mdsChangeData As DataSet ' = Nothing
    Private mbShowParamBox As Boolean ' = False
    Private colZones As clsZones ' = Nothing
    Private mParamsForLookup() As String
    Private msOldZone As String = String.Empty
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private mbDataLoaded As Boolean ' = False
    Private mnProgress As Integer ' = 0
    Private msScreen As String = msALL
    Private msUser As String = msALL
    Private msDevice As String = msALL
    Private mbByArm As Boolean = True
    Private msParameter As String = msALL
    Private mnParamType As eParamType = eParamType.None
    Private mnPeriod As Integer ' = 0
    Private msScreenName As String
    Private mbUseRobots As Boolean
    Private msDatabasepath As String = String.Empty
    Private mPrintHtml As clsPrintHtml
    Private msUserName As String = String.Empty

    '******** End Property Variables    *************************************************************
    Friend WithEvents moPassword As clsPWUser 'RJO 03/23/12
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm 'RJO 03/23/12

    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet) 'RJO 03/23/12


    'New commands
    Delegate Sub OnChange_Callback(ByVal source As Object, ByVal e As FileSystemEventArgs)

    Private msIPCPath As String = String.Empty
    Private mWatcher As FileSystemWatcher
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

            'Use current language text for screen labels
            Dim Void As Boolean = mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, _
                                                                msBASE_ASSEMBLY_LOCAL, _
                                                                String.Empty)

        End Set

    End Property
    Friend ReadOnly Property DisplayCulture() As System.Globalization.CultureInfo
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
            Return New System.Globalization.CultureInfo(msCulture)
        End Get

    End Property

    Friend Property Privilege() As ePrivilege
        '********************************************************************************************
        'Description:  What can a user do - for now take privilege from password object and massage 
        '               it to work - need this property for future 
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mPrivilegeGranted
        End Get
        Set(ByVal Value As ePrivilege)

            'this is needed with old password object raising events
            'and can hopefully be removed when password is redone
            'Control.CheckForIllegalCrossThreadCalls = False

            mPrivilegeRequested = Value

            'handle logout
            If mPrivilegeRequested = ePrivilege.None Then
                mPrivilegeGranted = ePrivilege.None
                subEnableControls(True)
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
                'this is needed with old password object raising events
                'and can hopefully be removed when password is redone
                'Control.CheckForIllegalCrossThreadCalls = True
                Exit Property
            End If

            'prevent recursion
            If mPrivilegeGranted = mPrivilegeRequested Then
                'this is needed with old password object raising events
                'and can hopefully be removed when password is redone
               'Control.CheckForIllegalCrossThreadCalls = True
                Exit Property
            End If


            'If mPassword.CheckPassword(msSCREEN_NAME, Value, moPassword, moPrivilege) Then 'RJO 03/23/12
            If moPassword.CheckPassword(Value) Then
                'passed
                mPrivilegeGranted = mPrivilegeRequested
                'if privilege changed may have to enable controls
                subEnableControls(True)
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
            Else
                'denied
                'hack me
                If moPassword.UserName <> String.Empty Then _
                        mPrivilegeRequested = mPrivilegeGranted
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
            End If

            'this is needed with old password object raising events
            'and can hopefully be removed when password is redone
           'Control.CheckForIllegalCrossThreadCalls = True

        End Set
    End Property

    Public Property LoggedOnUser() As String
        '********************************************************************************************
        'Description:  Who's Logged on
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msUserName
        End Get
        Set(ByVal Value As String)
            msUserName = Value
        End Set
    End Property
    Friend Property DataLoaded() As Boolean
        '********************************************************************************************
        'Description:  Data loaded flag for form
        '
        'Parameters: Set to true when done loading data
        'Returns:    True if data is loaded
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbDataLoaded
        End Get
        Set(ByVal Value As Boolean)
            mbDataLoaded = Value
            If mbDataLoaded Then
                'just finished loading reset & refresh
                subShowNewPage()
            End If
            'if not loaded disable all controls
            subEnableControls(Value)
        End Set
    End Property
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
                stsStatus.Items("lblStatus").Text = Strings.Replace(Value, vbTab, "  ")
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Property: Status", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set
    End Property
    Friend Property ScreenName() As String
        '********************************************************************************************
        'Description:  allow frmMain to pass in it's screen name (msSCREEN_NAME)
        '
        'Parameters: none
        'Returns:    The screen name associated with frmMain
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msScreenName
        End Get
        Set(ByVal value As String)
            msScreenName = value
        End Set
    End Property
    Friend Property UseRobots() As Boolean
        '********************************************************************************************
        'Description:  allow frmMain to pass in whether a Robot Select box should be shown (mbUSEROBOTS)
        '
        'Parameters: none
        'Returns:    True if Robot Select box should be shown
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbUseRobots
        End Get
        Set(ByVal value As Boolean)
            mbUseRobots = value
        End Set
    End Property
    Friend WriteOnly Property ZoneCollection() As clsZones
        '********************************************************************************************
        'Description:  What zone
        '
        'Parameters: zonename
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal Value As clsZones)
            colZones = Value
        End Set
    End Property
    Friend WriteOnly Property Screen() As String
        '********************************************************************************************
        'Description:  What Screen
        '
        'Parameters: screenname
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal Value As String)
            msScreen = Value
        End Set
    End Property
    Friend WriteOnly Property ByArm() As Boolean
        '********************************************************************************************
        'Description:  What user
        '
        'Parameters: username
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal Value As Boolean)
            mbByArm = Value
        End Set
    End Property
    Friend WriteOnly Property User() As String
        '********************************************************************************************
        'Description:  What user
        '
        'Parameters: username
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal Value As String)
            msUser = Value
        End Set
    End Property
    Friend WriteOnly Property Device() As String
        '********************************************************************************************
        'Description:  What arm
        '
        'Parameters: armname
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal Value As String)
            If Value = String.Empty Then Exit Property
            msDevice = Value
        End Set
    End Property
    Friend WriteOnly Property ParameterType() As eParamType
        '********************************************************************************************
        'Description:  who the heck
        '
        'Parameters: Parametername
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As eParamType)
            mnParamType = value
            Select Case value
                Case eParamType.None
                    mbShowParamBox = False
                Case eParamType.Colors
                    mbShowParamBox = True
                Case eParamType.Styles
                    mbShowParamBox = True
                Case eParamType.Valves
                    mbShowParamBox = True
            End Select
        End Set
    End Property
    Friend WriteOnly Property Parameter() As String
        '********************************************************************************************
        'Description:  What the heck
        '
        'Parameters: Parametername
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal Value As String)

            If Value = String.Empty Then Exit Property

            msParameter = Value
            If Strings.InStr(msParameter, "'", CompareMethod.Text) <> 0 Then
                msParameter = Strings.Replace(msParameter, "'", "%")
            End If
        End Set
    End Property
    Friend WriteOnly Property PeriodIndex() As Integer
        '********************************************************************************************
        'Description:  what time period to look
        '
        'Parameters: index of menu clicked
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal Value As Integer)
            mnPeriod = Value
            Select Case mnPeriod
                Case 0
                    btn24Hour.Select()
                Case 1
                    btn7Day.Select()
                Case 2
                    btnAll.Select()

            End Select

            'load on button click
            ''if everything is selected it must be a button push from screen - load data
            'If Me.Visible Then
            '    If cboZone.Text <> String.Empty Then
            '        If cboRobot.Text <> String.Empty Then
            '            If cboParam.Visible Then
            '                If cboParam.Text <> String.Empty Then
            '                    subLoadData()
            '                End If
            '            Else
            '                'no param box
            '                If msParameter = String.Empty Then
            '                    subLoadData()
            '                End If
            '            End If
            '        End If
            '    End If
            'End If

        End Set
    End Property

#End Region
#Region " Routines "

    Private Function bLoadFromGUI() As Boolean
        '********************************************************************************************
        'Description:  Load data stored on GUI in database, xml file etc.
        '
        'Parameters: none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            'get start and end times
            Dim dtEnd As System.DateTime = System.DateTime.Now
            Dim dtStart As System.DateTime
            Select Case mnPeriod
                Case 0
                    dtStart = DateAdd(DateInterval.Hour, -24, dtEnd)
                Case 1
                    dtStart = DateAdd(DateInterval.Day, -7, dtEnd)
                Case 2
                    dtStart = DateAdd(DateInterval.Year, -1, dtEnd)


            End Select


            mdsChangeData = bGetChangeLog(cboZone.Text, ScreenName, "ALL", _
                            msDevice, msParameter, dtStart, dtEnd)

            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Sub subChangeZone()
        '********************************************************************************************
        'Description: New Zone Selected - check for save then load new info
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Progress = 10

        'just in case
        If cboZone.Text = String.Empty Then
            Exit Sub
        Else

            If colZones.SetCurrentZone(cboZone.Text, True) = False Then
                If msOldZone <> String.Empty Then
                    cboZone.Text = msOldZone
                Else
                    cboZone.SelectedIndex = -1
                End If

                Progress = 100
                Me.Cursor = System.Windows.Forms.Cursors.Default

                Exit Sub
            End If

        End If

        msOldZone = cboZone.Text

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subClearScreen()

            subEnableControls(False)

            subFormatScreenLayout()

            If UseRobots Then
                If mbByArm Then
                    LoadRobotBoxFromDB(cboRobot, colZones.ActiveZone, False, True)
                Else
                    LoadRobotBoxFromDB(cboRobot, colZones.ActiveZone, True, True)
                End If
                cboParam.Items.Clear()
                msOldParam = String.Empty
                msOldRobot = String.Empty
                cboRobot.Visible = True
                lblRobot.Visible = True

                'statusbar text
                If cboRobot.Text = String.Empty Then
                    Status(True) = gcsRM.GetString("csSELECT_ROBOT")
                End If
            Else
                'no robots - just load data
                cboRobot.Visible = False
                lblRobot.Visible = False
                'load on button click
                'subLoadData()
            End If  'mbUSEROBOTS

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)

            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try


    End Sub
    Private Sub subClearScreen()
        '********************************************************************************************
        'Description:  Clear out textboxes reset colors etc. here
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        subEnableControls(False)
        DataLoaded = False
        dgChange.DataSource = Nothing
        dgChange.DataMember = String.Empty
        dgChange.Refresh()

    End Sub
    Private Sub subEnableControls(ByVal bEnable As Boolean)
        '********************************************************************************************
        'Description: Disable or enable controls. check privileges and edits etc. 
        '
        'Parameters: bEnable - false can be used to disable during load etc 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/18/13  MSW     Enable save and print to file if this screen gets opened under any password
        '********************************************************************************************
        Dim bRestOfControls As Boolean = False

        btnClose.Enabled = True

        If bEnable = False Then
            btnSave.Enabled = False
            btnPrint.Enabled = False
            btnStatus.Enabled = True
            bRestOfControls = False
            pnlMain.Enabled = False
        Else
            Select Case Me.Privilege
                Case ePrivilege.None
                    btnSave.Enabled = DataLoaded
                    btnPrint.Enabled = DataLoaded
                    btnStatus.Enabled = True
                    bRestOfControls = False
                    pnlMain.Enabled = True
                    bRestOfControls = True
                    mnuPrintFile.Enabled = DataLoaded
                Case Else
                    btnSave.Enabled = DataLoaded
                    btnPrint.Enabled = DataLoaded
                    btnStatus.Enabled = True
                    bRestOfControls = False
                    pnlMain.Enabled = True
                    bRestOfControls = True
                    mnuPrintFile.Enabled = DataLoaded
            End Select
        End If

        'restof controls here

    End Sub
    Private Sub subFormatScreenLayout()
        '********************************************************************************************
        'Description:  set up the screen layout
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
    End Sub
    Private Function bGetChangeLog(ByVal ZoneName As String, ByVal Area As String, _
                ByVal sUser As String, ByVal sDevice As String, ByVal sParameter As String, _
                ByVal dtStart As DateTime, _
                ByVal dtEnd As DateTime) As DataSet
        '********************************************************************************************
        'Description:  Read from database
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim DB As clsSQLAccess = New clsSQLAccess
        Dim DS As DataSet = New DataSet
        Dim s As SqlClient.SqlCommand

        Try
            ' build the sql statement
            DS.Locale = mLanguage.FixedCulture

            With DB
                .DBFileName = gsCHANGE_DATABASENAME
                .Zone = colZones.ActiveZone
                .DBTableName = gsCHANGE_DS_TABLENAME
                s = .GetStoredProcedureCommand("GetChangeRecords")
            End With

            With s.Parameters

                .Add(New SqlClient.SqlParameter("@Zone", _
                    SqlDbType.NVarChar)).Value = ZoneName
                .Add(New SqlClient.SqlParameter("@Area", _
                    SqlDbType.NVarChar)).Value = Area
                .Add(New SqlClient.SqlParameter("@PWUser", _
                    SqlDbType.NVarChar)).Value = sUser
                .Add(New SqlClient.SqlParameter("@Device", _
                    SqlDbType.NVarChar)).Value = sDevice
                .Add(New SqlClient.SqlParameter("@Parameter", _
                    SqlDbType.NVarChar)).Value = sParameter
                .Add(New SqlClient.SqlParameter("@StartQuery_Time", _
                    SqlDbType.DateTime)).Value = dtStart
                .Add(New SqlClient.SqlParameter("@EndQuery_Time", _
                    SqlDbType.DateTime)).Value = dtEnd
                'column captions

                .Add(New SqlClient.SqlParameter("@Zone_Cap", _
                    SqlDbType.NVarChar)).Value = gcsRM.GetString("csZONE_CAP")
                .Add(New SqlClient.SqlParameter("@Area_Cap", _
                    SqlDbType.NVarChar)).Value = gcsRM.GetString("csAREA_CAP")
                .Add(New SqlClient.SqlParameter("@PWUser_Cap", _
                    SqlDbType.NVarChar)).Value = gcsRM.GetString("csUSER_CAP")
                .Add(New SqlClient.SqlParameter("@Device_Cap", _
                    SqlDbType.NVarChar)).Value = gcsRM.GetString("csROBOT_CAP")
                .Add(New SqlClient.SqlParameter("@Parameter_Cap", _
                    SqlDbType.NVarChar)).Value = gcsRM.GetString("csPARAMETER_CAP")
                .Add(New SqlClient.SqlParameter("@Description_Cap", _
                    SqlDbType.NVarChar)).Value = gcsRM.GetString("csSPECIFIC_CAP")
                .Add(New SqlClient.SqlParameter("@Time_Cap", _
                    SqlDbType.NVarChar)).Value = gcsRM.GetString("csTIME_CAP")


            End With

            DS = DB.GetDataSet(s)

            If DS.Tables.Contains(gsCHANGE_DS_TABLENAME) Then
                Return DS
            Else
                Return Nothing
            End If


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            Return Nothing
        End Try

    End Function
    Private Function sProcessCommandLine() As String
        '********************************************************************************************
        'Description: If there are command line parameters - process here
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/01/09  MSW     Color Change preset support
        '********************************************************************************************
        'Color change presets select
        Dim sArg As String = "/culture="
        Dim sPath As String = String.Empty
        For Each s As String In My.Application.CommandLineArgs
            'If a culture string has been passed in, set the current culture (display language)
            If s.ToLower.StartsWith(sArg) Then
                Culture = s.Remove(0, sArg.Length)
            Else
                sPath = sPath & s & " "
            End If
        Next
        Return sPath.Trim
    End Function
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
        '********************************************************************************************
        Dim lReply As System.Windows.Forms.DialogResult = Response.OK

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

        Status = gcsRM.GetString("csINITALIZING")
        Progress = 1

        Try
            DataLoaded = False

            Progress = 5


            colZones = New clsZones(String.Empty)

            '4/21/08 only local to server selected in the main form
            'NVSP 12/19/2016 Paint shop computer change log 
            If colZones.PaintShopComputer Then
                mScreenSetup.LoadZoneBox(cboZone, colZones, False)
            Else
                mScreenSetup.LoadZoneBox(cboZone, colZones, False, colZones.ActiveZone.ServerName)
            End If
            ' mScreenSetup.LoadZoneBox(cboZone, colZones, False, colZones.ActiveZone.ServerName)

            mScreenSetup.InitializeForm(Me)

            ' this is after initializeform to overwrite version number
            Me.Text = gcsRM.GetString("csCHANGELOGFORM")
            'other screen specific
            lblParam.Text = gcsRM.GetString("csPARAMETER_CAP")
            btn24Hour.Text = gcsRM.GetString("cs24HRS")
            btn7Day.Text = gcsRM.GetString("cs7DAYS")
            btnAll.Text = gcsRM.GetString("csALLCHG")

            Progress = 98

            Me.Show()

            subEnableControls(True)

            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound

            mPrintHtml = New clsPrintHtml(msSCREEN_NAME, True, 20)

            subProcessFile(sProcessCommandLine)

            mPWCommon.GetDefaultFilePath(msIPCPath, eDir.IPC, String.Empty, String.Empty)
            mWatcher = New FileSystemWatcher

            With mWatcher
                .Path = msIPCPath
                'Watch for changes in LastWrite times 
                .NotifyFilter = NotifyFilters.LastWrite
                .Filter = "ShowChangeLog*.ShowChangeLog"

                'Add event handler
                AddHandler .Changed, AddressOf OnChanged

                ' Begin watching.
                .EnableRaisingEvents = True
            End With
            Dim sFiles As String() = Directory.GetFiles(msIPCPath)
            For Each sFile As String In sFiles
                If My.Computer.FileSystem.FileExists(sFile) Then
                    My.Computer.FileSystem.DeleteFile(sFile)
                End If
            Next

            
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            lReply = ShowErrorMessagebox(gcsRM.GetString("ERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.AbortRetryIgnore)

            Select Case lReply
                Case Response.Abort
                    Me.Cursor = System.Windows.Forms.Cursors.Default
                    End
                Case Response.Ignore
                Case Response.Retry
                    subInitializeForm()
            End Select
        Finally
            Status = gcsRM.GetString("csDONE")
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try


    End Sub
    Private Sub subProcessFile(ByRef sFileName As String)
        '********************************************************************************************
        'Description: Get the screen parameters from a text file
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sr As System.IO.StreamReader = Nothing
        Dim sTmp As String = String.Empty
        Try
            sr = System.IO.File.OpenText(sFileName)
            Do While sr.Peek() >= 0
                Try
                    Dim sLine As String = sr.ReadLine()
                    Dim sColumns() As String = Split(sLine, "=")
                    If sColumns.GetUpperBound(0) > 0 Then
                        Select Case sColumns(0)
                            Case "Zone"
                                colZones.CurrentZone = sColumns(1)
                            Case "Parameter"
                                msParameter = sColumns(1)
                            Case "Device"
                                msDevice = sColumns(1)
                            Case "ParameterType"
                                ParameterType = CType(CType(sColumns(1), Integer), eParamType)
                            Case "PeriodIndex"
                                PeriodIndex = CType(sColumns(1), Integer)
                            Case "Screen"
                                msScreen = sColumns(1)
                            Case "ScreenName"
                                msScreenName = sColumns(1)
                        End Select
                    Else
                        Select Case sLine
                            Case "Robots", "Arms"
                                mbUseRobots = True
                                mbByArm = True
                            Case "Controllers"
                                mbUseRobots = True
                                mbByArm = False
                            Case "NoRobots"
                                mbUseRobots = False
                                mbByArm = False
                            Case Else
                        End Select
                    End If
                Catch ex As Exception
                    Trace.WriteLine(ex.Message)
                    Trace.WriteLine(ex.StackTrace)
                End Try
            Loop
            ShowChangeLogData()

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
        Finally
            sr.Close()
            My.Computer.FileSystem.DeleteFile(sFileName)
        End Try
    End Sub
    Private Sub OnChanged(ByVal source As Object, ByVal e As FileSystemEventArgs)
        '********************************************************************************************
        'Description: A text file of interest was written to. Process, then delete the file.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If Me.InvokeRequired Then
            Dim dOnChanged As New OnChange_Callback(AddressOf OnChanged)
            Me.BeginInvoke(dOnChanged, New Object() {source, e})
        Else
            Dim sCmd As String = String.Empty
            Try
                Select Case e.ChangeType
                    Case WatcherChangeTypes.Changed
                        If My.Computer.FileSystem.FileExists(e.FullPath) Then
                            subProcessFile(e.FullPath)
                        End If
                End Select
            Catch ex As Exception
                Trace.WriteLine(ex.Message)
                Trace.WriteLine(ex.StackTrace)
            End Try
        End If 'Me.InvokeRequired
    End Sub

    Private Sub subLoadData()
        '********************************************************************************************
        'Description:  Data Load Routine
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        DataLoaded = False

        Try
            Dim sTmp As String = gcsRM.GetString("csLOADINGDATA") & vbTab & _
                                                cboRobot.Text & ";" & vbTab & _
                                                cboParam.Text & ";"
            Status = sTmp
            Progress = 5

            If bLoadFromGUI() Then

                'this refreshes screen 
                DataLoaded = True
                Status = gcsRM.GetString("csLOADDONE")

            Else
                'Load Failed
                subEnableControls(False)
            End If  ' bLoadFromGUI()


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            subEnableControls(False)

            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default

        End Try
    End Sub
    Private Sub subChangeParameter()
        '********************************************************************************************
        'Description:  New Parameter selected
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************


        'just in case
        If cboParam.Text = String.Empty Then
            Exit Sub
        Else
            If cboParam.Text = msOldParam Then Exit Sub
        End If
        msOldParam = cboParam.Text
        Parameter = cboParam.Text

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

            subClearScreen()
            'load on button click
            'subLoadData()


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)

            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try


    End Sub
    Private Sub mnuPageSetup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPageSetup.Click
        '********************************************************************************************
        'Description:  show page setup dialog 
        '
        'Parameters: none
        'Returns:    Print settings to use in printing
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If (dgChange IsNot Nothing) AndAlso DataLoaded Then
            If bPrintdoc(False) Then
                mPrintHtml.subShowPageSetup()
            End If
        End If
    End Sub

    Private Sub mnuPrint_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                    Handles mnuPrint.Click
        '********************************************************************************************
        'Description:  print the table
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If (dgChange IsNot Nothing) AndAlso DataLoaded Then
            bPrintdoc(True)
        End If
    End Sub
    Private Sub mnuPrintFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintFile.Click
        '********************************************************************************************
        'Description:  print the table
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If (dgChange IsNot Nothing) AndAlso DataLoaded Then
            If bPrintdoc(False) Then
                mPrintHtml.subSaveAs()
            End If
        End If

    End Sub

    Private Sub mnuPrintPreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintPreview.Click
        '********************************************************************************************
        'Description:  print the table
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If (dgChange IsNot Nothing) AndAlso DataLoaded Then
            If bPrintdoc(False) Then
                mPrintHtml.subShowPrintPreview()
            End If
        End If
    End Sub
    Private Sub mnuPrintOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintOptions.Click
        '********************************************************************************************
        'Description:  offer options for printout table setup.
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mPrintHtml.subShowOptions()
    End Sub
    Private Function bPrintdoc(ByVal bPrint As Boolean, Optional ByVal bExport As Boolean = False) As Boolean
        '********************************************************************************************
        'Description:  build the print document
        '
        'Parameters:  bPrint - send to printer
        'Returns:   true if successful, OK to continue with preview or page setup
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If (dgChange IsNot Nothing) AndAlso DataLoaded Then
            mPrintHtml.subSetPageFormat()
            mPrintHtml.subClearTableFormat()
            mPrintHtml.subSetColumnCfg("align=right", 0)
            mPrintHtml.subSetRowcfg("Bold=on", 0, 0)

            mPrintHtml.HeaderRowsPerTable = 1

            Dim sTitle(0) As String
            sTitle(0) = colZones.SiteName & "  " & colZones.CurrentZone
            If cboRobot.Visible Then
                sTitle(0) = sTitle(0) & gcsRM.GetString("csROBOT_CAP") & ":  " & cboRobot.Text
            End If
            If cboParam.Visible Then
                Dim sTmp(0) As String
                sTmp(0) = lblParam.Text & ":  " & cboParam.Text
                mPrintHtml.subCreateSimpleDoc(dgChange, Status, gcsRM.GetString("csCHANGE_SUM_CAP") & " - " & msScreen, sTitle, sTmp, bExport)
            Else
                mPrintHtml.subCreateSimpleDoc(dgChange, Status, gcsRM.GetString("csCHANGE_SUM_CAP") & " - " & msScreen, sTitle, , bExport)
            End If
            If bPrint And (bExport = False) Then
                mPrintHtml.subPrintDoc()
            End If
            Return (True)
        Else
            Return (False)
        End If
    End Function
    Private Sub subChangeRobot()
        '********************************************************************************************
        'Description:  New robot selected
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'just in case
        If cboRobot.Text = String.Empty Then
            Exit Sub
        Else
            If cboRobot.Text = msOldRobot Then Exit Sub
        End If
        msOldRobot = cboRobot.Text
        Device = cboRobot.Text

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            Progress = 5

            subClearScreen()

            ' if there is no parameter combo, load data here
            If mbShowParamBox = False Then
                'load data on button click
                'subLoadData()
            Else 'load parameter box
                msOldParam = String.Empty
                Select Case mnParamType
                    Case eParamType.Colors
                        LoadColorBoxFromDB(cboParam, colZones, mParamsForLookup, True)
                    Case eParamType.Valves
                        LoadValveBoxFromDB(cboParam, colZones, mParamsForLookup, True)
                    Case eParamType.Styles
                        Dim o As New clsSysStyles(colZones.ActiveZone)
                        o.LoadStyleBoxFromCollection(cboParam)
                        cboParam.Items.Insert(0, gcsRM.GetString("csALL"))
                End Select

                If msParameter <> String.Empty Then
                    lblParam.Visible = True
                    cboParam.Visible = True
                    cboParam.Text = msParameter
                End If

            End If


            Status(True) = gcsRM.GetString("csSELECT_COLOR")

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)

            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Me.Cursor = System.Windows.Forms.Cursors.Default
            Progress = 100
        End Try


    End Sub

    Private Sub subSaveData()
        '********************************************************************************************
        'Description:  Data Save Routine
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/05/13  RJO     Empty fields in some rows were DBNull causing .Append to barf.
        '********************************************************************************************
        bPrintdoc(False, True)

        'Dim nCol As Integer = 0
        'Dim nRow As Integer = 0
        'Dim dt As DataTable = Nothing
        'Dim sTmp As New StringBuilder(String.Empty)

        ''Select Case Me.Privilege
        ''    Case ePrivilege.None
        ''        ' shouldnt be here
        ''        subEnableControls(False)
        ''        Exit Sub
        ''    Case Else
        ''        'ok
        ''End Select

        'Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

        'Dim oStream As System.IO.Stream
        'Dim oSaveDialog As New SaveFileDialog
        'Try

        '    With oSaveDialog
        '        .Filter = gcsRM.GetString("csSAVE_CSV_CDFILTER")
        '        .FilterIndex = 1
        '        .RestoreDirectory = True
        '        .AddExtension = True
        '        .FileName = Me.Text & ".csv"
        '    End With

        '    dt = CType(dgChange.DataSource, DataTable)

        '    If oSaveDialog.ShowDialog() = Response.OK Then
        '        oStream = oSaveDialog.OpenFile()
        '        If Not (oStream Is Nothing) Then
        '            Dim sw As New System.IO.StreamWriter(oStream)
        '            'header text
        '            For nCol = 0 To dt.Columns.Count - 1
        '                'sTmp = sTmp & dt.Columns(nCol).Caption & ","
        '                sTmp.Append(dt.Columns(nCol).Caption & ",")
        '            Next
        '            sw.WriteLine(sTmp)
        '            'data
        '            For nRow = 0 To dt.Rows.Count - 1
        '                sTmp = New StringBuilder(String.Empty)
        '                For nCol = 0 To dt.Columns.Count - 1
        '                    'sTmp = sTmp & CType(dt.Rows(nRow).Item(nCol), String) & ","
        '                    If Not IsDBNull(dt.Rows(nRow).Item(nCol)) Then 'RJO 06/05/13
        '                        sTmp.Append(CType(dt.Rows(nRow).Item(nCol), String) & ",")
        '                    Else
        '                        sTmp.Append(" ,")
        '                    End If
        '                Next
        '                sw.WriteLine(sTmp)
        '            Next
        '            sw.Close()
        '            oStream.Close()
        '            sw = Nothing
        '        End If
        '    End If

        'Catch ex As Exception
        '    Trace.WriteLine(ex.Message)
        '    Trace.WriteLine(ex.StackTrace)
        '    ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
        '                        Status, MessageBoxButtons.OK)

        'Finally

        '    oStream = Nothing
        '    oSaveDialog = Nothing

        '    Me.Cursor = System.Windows.Forms.Cursors.Default
        'End Try

    End Sub

    Private Sub subShowNewPage()
        '********************************************************************************************
        'Description:  Data is all loaded or changed and screen needs to be refreshed
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************


        If mdsChangeData Is Nothing Then Exit Sub
        Dim dt As DataTable = mdsChangeData.Tables(gsCHANGE_DS_TABLENAME)

        With dgChange
            .SuspendLayout()

            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToResizeColumns = True
            .AllowUserToResizeRows = False
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            .RowHeadersVisible = True

            .DataSource = dt


            For i As Integer = 0 To .Columns.Count - 1
                .Columns(i).AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            Next

            For i As Integer = 0 To .Rows.Count - 1
                .Rows(i).HeaderCell.Value = (i + 1).ToString(mLanguage.FixedCulture)
            Next
            .RowHeadersWidth = 50

            .ResumeLayout()
            .Show()

            If .Rows.Count = 0 Then
                MessageBox.Show(gcsRM.GetString("csNO_CHANGES_FOUND"), _
                                gcsRM.GetString("csPAINTWORKS"), MessageBoxButtons.OK, _
                                MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, _
                                MessageBoxOptions.DefaultDesktopOnly)
            End If

        End With

    End Sub
    Friend Sub ShowChangeLogData()
        '********************************************************************************************
        'Description: properties are set - go to work
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************


        cboZone.Text = colZones.ActiveZone.Name
        'this is blocked in change event
        subChangeZone()



        If msDevice = String.Empty Then
            msDevice = gcsRM.GetString("csALL")
        End If
        cboRobot.Text = msDevice

        'If param is empty assume it is not required. This has to be first
        If mbShowParamBox Then
            cboParam.Visible = True
            lblParam.Visible = True
        Else
            cboParam.Visible = False
            lblParam.Visible = False
            msDevice = gcsRM.GetString("csALL")
        End If

        If msParameter = String.Empty Then
            msParameter = gcsRM.GetString("csALL")
        End If
        If mbUseRobots Then
            lblRobot.Visible = True
            cboRobot.Visible = True
            cboRobot.Text = msDevice    'this calls subChangeRobot
        Else
            lblRobot.Visible = False
            cboRobot.Visible = False
            msParameter = gcsRM.GetString("csALL")
        End If

        subLoadData()

        Me.Show()
        Me.BringToFront()

    End Sub

#End Region
#Region " Events "

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                                        Handles MyBase.Load
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

        End Try


    End Sub

    Private Sub frmMain_Closing(ByVal sender As Object, _
                            ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        '********************************************************************************************
        'Description:  setting e.cancel to true aborts close
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

    End Sub
    Private Sub cboZone_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                            Handles cboZone.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Zone Combo Changed 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If cboZone.Text <> colZones.ActiveZone.Name Then
            subChangeZone()
        End If

    End Sub
    Private Sub cboRobot_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                            Handles cboRobot.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Robot Combo Changed 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If cboRobot.Text <> msOldRobot Then
            subChangeRobot()
        End If

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
        '    04/20/12   MSW     frmMain_KeyDown-Add excape key handler                        4.01.03.01
        '    06/07/12   MSW     frmMain_KeyDown-Simplify help links                           4.01.03.02
		'    09/30/13   MSW     Save screenshots as jpegs
        '********************************************************************************************
        Dim sKeyValue As String = String.Empty

        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode
                Case Keys.F1
                    'Help Screen request
                    sKeyValue = "btnHelp"
                    If oIPC Is Nothing Then
                        oIPC = New Paintworks_IPC.clsInterProcessComm
                    End If
                    subLaunchHelp(gs_HELP_REPORTS_CHANGE, oIPC)

                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty

                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)

                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath & msSCREEN_DUMP_NAME & ".jpg", Imaging.ImageFormat.Jpeg)

                Case Keys.Escape
                    Me.Close()

                Case Else

            End Select
        End If

    End Sub

    Private Sub frmMain_Layout(ByVal sender As Object, _
                            ByVal e As System.Windows.Forms.LayoutEventArgs) Handles MyBase.Layout
        '********************************************************************************************
        'Description:  Form needs a redraw due to resize
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim nHeight As Integer = tscMain.ContentPanel.Height - pnlMain.Top
        If nHeight < 100 Then nHeight = 100
        Dim nWidth As Integer = tscMain.ContentPanel.Width - (pnlMain.Left * 2)
        If nWidth < 100 Then nWidth = 100
        pnlMain.Height = nHeight
        pnlMain.Width = nWidth
        Const nMINSTATUS_LEFT As Integer = 600
        Const nMAXSTATUS_LEFT As Integer = 1000
        Dim nStatusLeft As Integer = nWidth - lstStatus.Width
        If nStatusLeft < nMINSTATUS_LEFT Then
            nStatusLeft = nMINSTATUS_LEFT
        End If
        If nStatusLeft > nMAXSTATUS_LEFT Then
            nStatusLeft = nMAXSTATUS_LEFT
        End If
        lstStatus.Left = nStatusLeft

    End Sub

    Private Sub cboParam_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                             Handles cboParam.SelectionChangeCommitted
        '********************************************************************************************
        'Description:  Parameter Combo Changed 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If cboParam.Text <> msOldParam Then
            subChangeParameter()
        End If

    End Sub
    Private Sub tlsMain_ItemClicked(ByVal sender As Object, _
     ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tlsMain.ItemClicked
        '********************************************************************************************
        'Description: Tool Bar button clicked - double check privilege here incase it expired
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case e.ClickedItem.Name
            Case "btnClose"

                Me.Close()

            Case "btnStatus"
                lstStatus.Visible = Not lstStatus.Visible

            Case "btnSave"
                'privilege check done in subroutine
                subSaveData()

            Case "btnPrint"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                bPrintdoc(True)

            Case "btn24Hour"
                Me.SuspendLayout()
                'btn24Hour.Checked = True
                'btn24Hour.CheckState = CheckState.Checked
                'btn7Day.Checked = False
                'btn7Day.CheckState = CheckState.Unchecked
                'btnAll.Checked = False
                'btnAll.CheckState = CheckState.Unchecked
                PeriodIndex = 0
                Me.ResumeLayout()
                Me.Refresh()
                DataLoaded = bLoadFromGUI()

            Case "btn7Day"
                Me.SuspendLayout()
                'btn24Hour.Checked = False
                'btn24Hour.CheckState = CheckState.Unchecked
                'btn7Day.Checked = True
                'btn7Day.CheckState = CheckState.Checked
                'btnAll.Checked = False
                'btnAll.CheckState = CheckState.Unchecked
                PeriodIndex = 1
                Me.ResumeLayout()
                Me.Refresh()
                DataLoaded = bLoadFromGUI()

            Case "btnAll"
                Me.SuspendLayout()
                'btn24Hour.Checked = False
                'btn24Hour.CheckState = CheckState.Unchecked
                'btn7Day.Checked = False
                'btn7Day.CheckState = CheckState.Unchecked
                'btnAll.Checked = True
                'btnAll.CheckState = CheckState.Checked
                PeriodIndex = 2
                Me.ResumeLayout()
                Me.Refresh()
                DataLoaded = bLoadFromGUI()

        End Select
    End Sub

#End Region

    Public Sub New()
        '********************************************************************************************
        'Description:  I feel like a new form!
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'for language support
        mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, msBASE_ASSEMBLY_LOCAL, _
                                      String.Empty)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub dgChange_CellFormatting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles dgChange.CellFormatting
        '********************************************************************************************
        'Description:  take care of any special runtime formatting on the grid
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If dgChange.Columns(e.ColumnIndex).Name.Equals(gcsRM.GetString("csAREA_CAP")) Then
            Dim sScreen As String = e.Value.ToString
            e.Value = mPWCommon.sGetScreenDisplayName(sScreen)
        End If
    End Sub
End Class