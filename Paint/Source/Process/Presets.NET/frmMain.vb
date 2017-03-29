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
' Form/Module: frmMain
'
' Description: View and edit preset data
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
'    04/24/2008 gks     Convert to use SQL database "Process Data"                  4.00.01.00
'    06/20/2008 gks     Add Estat Descriptions                                      4.00.02.00
'    06/01/2009 MSW     convert to VB2008.  Handle class changes made for Fluid Maint,
'                       handle cross-thread calls in colControllers_ConnectionStatusChange(),
'                       moPassword_LogIn(), and moPassword_LogOut()
'                       subInitFormText() - removed ApplicatorType, use ColorChangeType instead
'    06/01/09   MSW     support color change presets - enabled by command-line "ColorChange"
'                       also read in zone and robot, to support a call directly from the color
'                       change screen.
'    11/05/09   RJO     Modified calls to mPWRobotCommon.LoadRobotBoxFromCollection to not
'                       include openers in arm list.
'    11/10/09   MSW     Modifications for PaintTool 7.50 presets
'                       Reads preset config from robot.
'                       parm name on the robot is used for resource lookup:
'                       for APP_CON_NAM= "FF", rsFF = "Paint", rsFF_CAP = "Paint cc/min", rsFF_UNITS ="cc/min"
'                       supports 2nd shaping air, volume on CC screen
'    11/12/09   MSW     subInitializeForm - Move LoadTextBoxCollection and subSetTextBoxProperties to 
'                       before the layout, it needs to get called for the collection before we start 
'                       adding individual boxes so they don't get addHandler twice
'    11/18/09   MSW     subTextboxKeypressHandler, subSetTextBoxProperties - Offer a login if they try to enter data without a login
'    04/09/10   RJO     subInitializeForm - copy save button image to special save button. Image
'                       was blank.
'    05/07/10   MSW     subTextboxKeyUpHandler - handle page up, page down
'    05/07/10   MSW     Validate the current box before shifting it around
'    12/01/10   MSW     allow import from and export to csv
'    09/14/11   MSW     Assemble a standard version of everything                       4.1.0.0
'    09/27/11   MSW     Standalone changes round 1 - HGB SA paintshop computer changes	4.1.0.1
'    12/02/11   MSW     Add DMONCfg placeholder reference                               4.1.1.0
'    01/09/12   MSW     Import/export improvements                                      4.1.1.1
'    01/24/12   MSW     Applicator Updates                                            4.01.01.02
'    02/16/12   MSW     Print/import/export updates, force 32 bit build               4.01.01.03
'    03/23/12   RJO     modifed for .NET Password and IPC                             4.01.01.04
'    03/28/12   MSW     prevent multiple IPC objects when switching between preset screens  4.01.02.01
'    04/11/12   MSW     Changed CommonStrings setup so it builds correctly            4.01.03.00
'    04/27/12   MSW     make utilities menu open easier                               4.01.03.01
'    04/30/12   RJO     Added code in subCopyButtonPressed to set new frmCopy         4.01.03.02
'                       ParamDetails property to support copy confimation message 
'                       boxes for all copy buttons.
'    07/27/12   JBW     Change to subTextboxKeypressHandler that will disallow any    4.01.03.03
'                       and all "<" and ">" characters, they will crash changelog from 
'                       logging anything.
'    10/19/12   BTK/    Bug fixes for Presets by Style/Color. Changes to bLoadFromGUI 4.01.03.04
'               KDJ     and mPresetCommon (v4.1.3.1) ValidatePresetDatatable, 
'                       ValidatePresetDataRow, and GetPresetDataset.
'               RJO     Added StartupModule.vb to project to prevent multiple 
'                       instances from running. Changed project "Startup object:" 
'                       setting to "StartupModule".
'    04/02/13   DE      Added missing localization code to Culture Property and       4.01.03.05
'                       subInitFormText. Added subInitFormText to frmOverrides.
'    04/16/13   MSW     Add Canadian language files                                   4.01.05.00
'                       Standalone Changelog
'    07/09/13   MSW     Update and standardize logos                                  4.01.05.01
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up  4.01.05.02
'    09/30/13   MSW     Save screenshots as jpegs                                     4.01.06.00
'    01/07/14   MSW     Remove controlbox from main for                               4.01.06.01
'    11/13/13   BTK     subInitFormText Override labels not place correctly.
'                       Shaping air 2 not displayed.                                  4.01.06.02
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call              4.01.07.00
'                       Fixes from KTP
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On

Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Configuration.ConfigurationSettings
Imports Response = System.Windows.Forms.DialogResult
Imports ConnStat = FRRobotNeighborhood.FRERNConnectionStatusConstants

Friend Class frmMain
#Region " Declares "

    '******** Form Constants   **********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Friend Const msSCREEN_NAME As String = "Presets"   ' <-- For password area change log etc.
    Friend Const msSCREEN_DUMP_NAME As String = "Process_Presets_Fluid"
    Friend Const msSCREEN_DUMP_NAME_CC As String = "Process_Presets_CC"
    Friend Const msSCREEN_DUMP_NAME_ESTAT As String = "Process_Presets_Estat"
    Friend Const msSCREEN_DUMP_EXT As String = ".jpg"
    Private Const mnMAININX As Integer = 100
    Private Const mnSCREENINX As Integer = 101
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    Friend Const mbUSEROBOTS As Boolean = True
    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    Friend Shared gsChangeLogArea As String = msSCREEN_NAME
    Private msOldZone As String = String.Empty
    Private msOldRobot As String = String.Empty
    Private msOldColor As String = String.Empty
    Private mnOldStyle As Integer = 0
    Private colZones As clsZones = Nothing
    Private WithEvents colControllers As clsControllers = Nothing
    Private WithEvents colArms As clsArms = Nothing
    Private mRobot As clsArm = Nothing
    Private colTextboxes As Collection(Of FocusedTextBox.FocusedTextBox) = _
                            New Collection(Of FocusedTextBox.FocusedTextBox)
    
    Private frmOver As frmOverrides = Nothing
    Private frmStp As frmSteps = Nothing
    Private mbEventBlocker As Boolean = False
    Private mbRemoteZone As Boolean = False
    Private mbScreenLoaded As Boolean = False
    Private msEstatSteps(7) As String
    Friend Enum eScreenSelect
        FluidPresets
        EstatPresets
        CCPresets
    End Enum
    Private meScreen As eScreenSelect = eScreenSelect.FluidPresets
    Private meOldScreen As eScreenSelect = eScreenSelect.FluidPresets
    Private msCmdLineZone As String = String.Empty
    Private msCmdLineRobot As String = String.Empty
    'Applicator config

    'screen formatting
    Private mnITEMS_PER_PAGE As Integer = 10 'Number of items displayed at a time on one page
    Private mnOldItemsPerPage As Integer = 10
    Private mnRowHeight As Integer = 41
    Private mnLblHeight As Integer = 23
    Private mnNumberofpresets As Integer = 40
    Private mbFirstPass As Boolean = True
    Friend mnMaxParms As Integer = 3 'This gets updated as extra columns are made
    Friend mnNumParms As Integer = 1 'Current number of columns visible

    Friend mParmDetail() As tParmDetail = Nothing
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private mbEditsMade As Boolean = False
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean = False
    Private mnProgress As Integer = 0
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    Private msCulture As String = "en-US" 'Default to english
    Private mbSkipMinCheck As Boolean = False
    '******** End Property Variables    *************************************************************

    '******** This is the old pw3 password object interop  *****************************************
    'Friend WithEvents moPassword As PWPassword.PasswordObject 'RJO 03/23/12
    'Friend moPrivilege As PWPassword.cPrivilegeObject 'RJO 03/23/12
    '******** This is the old pw3 password object interop  *****************************************

    Friend WithEvents moPassword As clsPWUser 'RJO 03/23/12
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm 'RJO 03/23/12

    Private Shared mbNoRecursion As Boolean = False
    Private msColorCache As String = String.Empty
    Friend mbPresetsByStyle As Boolean = False
    Friend mnNumStyles As Integer = 1
    Private mPrintHtml As clsPrintHtml

    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet)  'RJO 03/23/12
    Delegate Sub ControllerStatusChange(ByVal Controller As clsController)

    Friend WithEvents colApplicators As clsApplicators = Nothing
    Private meOldApplicator As eColorChangeType = eColorChangeType.NOT_SELECTED
    Friend mApplicator As clsApplicator = Nothing

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
        ' 04/02/13  DE      Get resource managers for new culture
        '********************************************************************************************

        Set(ByVal value As String)

            msCulture = value
            mLanguage.DisplayCultureString = value
            mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, msBASE_ASSEMBLY_LOCAL, _
                                          msROBOT_ASSEMBLY_LOCAL)

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


            'If mPassword.CheckPassword(msSCREEN_NAME, Value, moPassword, moPrivilege) Then 'RJO 03/23 12
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
                EditsMade = False
                subInitFormText()
                subShowNewPage()
            End If
            'if not loaded disable all controls
            subEnableControls(Value)
        End Set
    End Property
    Friend ReadOnly Property ScreenSelect() As eScreenSelect
        Get
            Return meScreen
        End Get
    End Property

    Friend Property EditsMade() As Boolean
        '********************************************************************************************
        'Description:  Edit flag for form
        '
        'Parameters: set when somebody changed something
        'Returns:    True if active edits
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbEditsMade
        End Get
        Set(ByVal Value As Boolean)
            Dim bOld As Boolean = mbEditsMade
            mbEditsMade = Value
            If mbEditsMade <> bOld Then subEnableControls(True)
        End Set
    End Property
    Friend Property LoggedOnUser() As String
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

    Friend ReadOnly Property ActiveZone() As clsZone
        Get
            Return colZones.ActiveZone
        End Get
    End Property
#End Region
#Region " Routines "

    Private Function bAskForSave() As Boolean
        '********************************************************************************************
        'Description:  Check to see if we need to save when screen is closing 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 1/18/11   JBW     Save messagebox opened again after responding "no" when changing robots
        '********************************************************************************************
        Dim lRet As DialogResult

        lRet = MessageBox.Show(gcsRM.GetString("csSAVEMSG1") & vbCrLf & _
                            gcsRM.GetString("csSAVEMSG"), gcsRM.GetString("csSAVE_CHANGES"), _
                            MessageBoxButtons.YesNoCancel, _
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

        Select Case lRet
            Case Response.Yes 'Response.Yes
                subSaveData()
                Return True
            Case Response.Cancel
                'false aborts closing form , changing zone etc
                Status = gcsRM.GetString("csSAVE_CANCEL")
                Return False
            Case Else 'no
                EditsMade = False  'jbw edited
                Return True
        End Select

    End Function
    Friend Function bLoadFromGUI(ByVal Zone As clsZone, ByRef rRobot As clsArm, _
                                                    ByRef rPresets As clsPresets, _
                                                    ByRef rDataset As DataSet) As Boolean
        '********************************************************************************************
        'Description:  Load data stored on GUI in database, xml file etc.
        '
        'Parameters: none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/01/09  MSW     CC preset support
        ' 4/30/10   MSW     check for style selection first
        ' 10/19/12  BTK/KDJ Bug fixes for Presets by Style/Color. Requires mPresetCommon v4.1.3.1 
        '********************************************************************************************
        Dim DB As clsSQLAccess = New clsSQLAccess
        Dim sTableName As String = gsPRESET_TABLE_PREFIX & rRobot.Name
        Dim DR As DataRow = Nothing
        Dim oP As clsPreset = Nothing
        Dim DT As DataTable = New DataTable
        Dim bIsCopy As Boolean = False
        Dim bTmp As Boolean = False
        Dim nColor As Integer
        Dim nStyle As Integer
        Try

            ' we may be only getting descriptions for copy screen
            If rPresets.Count = 0 Then bIsCopy = True

            With DB

                .DBFileName = gsPROCESS_DBNAME
                .Zone = Zone
                If rPresets.ColorChangePresets Then
                    nColor = 0
                Else
                    nColor = rPresets.FanucColor
                End If
                ' 4/30/10   MSW     check for style selection first
                If mbPresetsByStyle Then
                    'BTK & KDJ 10/19/12  Added Style number.  mPresetCommon_ValidatePresetDataRow 
                    '                    had the style hard coded to 1.
                    nStyle = rPresets.StyleNumber
                    'BTK & KDJ 10/19.12 Added code to generate all presets for each color and style
                    '          when a new robot is selected only when there is no database table.
                    rDataset = GetPresetDataset(DB, sTableName, nColor, rPresets.StyleNumber, _
                                                rPresets.Count, colZones.ActiveZone.MaxColors, _
                                                colZones.ActiveZone.MaxStyles)
                Else
                    'rDataset = GetPresetDataset(DB, sTableName, nColor)
                    'KDJ & QAP 07/22/2014 Fixed code to create the dynamic max colors and max presets
                    rDataset = GetPresetDataset(DB, sTableName, nColor, 1, _
                                                rPresets.Count, colZones.ActiveZone.MaxColors, _
                                                1)
                End If
                If rDataset.Tables.Contains("[" & sTableName & "]") Then
                    DT = rDataset.Tables("[" & sTableName & "]")

                    If DT.Rows.Count = 0 And rPresets.Count = 0 Then
                        'entered from a copy - don't know how many presets to create
                        MessageBox.Show(gpsRM.GetString("psCANT_CREATE_RECORDS_COPY"), _
                                        gcsRM.GetString("csERROR"), MessageBoxButtons.OK, _
                                        MessageBoxIcon.Error)

                        Return False
                    End If

                    If Not bIsCopy Then
                        'BTK & KDJ 10/19/12  Added Style number.  ValidatePresetDataRow had the style hard coded to 1.
                        If ValidatePresetDatatable(rPresets.Count, rPresets.FanucColor, nColor, DT, bTmp, nStyle) Then
                            If bTmp Then
                                .UpdateDataSet(rDataset, sTableName)
                            End If
                        Else
                            Throw New Data.ConstraintException("Problem with preset datatable")
                        End If

                    End If

                    For Each DR In DT.Rows
                        If bIsCopy Then
                            'just getting desc
                            oP = New clsPreset
                            oP.Description.Text = DR.Item(gsDESC_COL).ToString
                            oP.EstatDescription.Text = DR.Item(gsESTATDESC_COL).ToString  '6.20.08
                            oP.Index = (CType(DR.Item(gsPRESET_COL), Integer))
                            rPresets.Add(oP)
                        Else
                            'presets already exist
                            Dim nPresetNum As Integer = CType(DR.Item(gsPRESET_COL), Integer)
                            If nPresetNum <= rPresets.Count Then
                                oP = rPresets.Item(nPresetNum)
                                oP.EstatDescription.Text = DR.Item(gsESTATDESC_COL).ToString  '6.20.08
                                oP.Description.Text = DR.Item(gsDESC_COL).ToString
                            End If
                        End If
                    Next
                Else
                    .Close()
                    Return False
                End If

                .Close()
            End With


            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bLoadFromPLC() As Boolean
        '********************************************************************************************
        'Description:  Load data stored on PLC.
        '
        'Parameters: none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            'no preset data in plc, just return
            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            Return False

        End Try


    End Function
    Friend Function bLoadFromRobot(ByRef rRobot As clsArm, ByRef rPresets As clsPresets) As Boolean
        '********************************************************************************************
        'Description:  Load data stored on Robot
        '
        'Parameters: none
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/01/09  MSW     CC preset support
        '********************************************************************************************

        Try

            If rRobot.IsOnLine Then
                If mPresetCommon.LoadPresetsFromRobot(rRobot, rPresets, mParmDetail, mnNumParms) Then
                    Return True
                Else
                    Return False
                End If
            Else
                ' cant talk to robot
                Dim sTmp As String = gcsRM.GetString("csLOADFAILED") & vbCrLf & _
                                            gcsRM.GetString("csCOULD_NOT_CONNECT")
                Status = gcsRM.GetString("csLOADFAILED")
                MessageBox.Show(sTmp, rRobot.Name, _
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If



        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Friend Function bSaveToGUI(ByRef rRobot As clsArm, ByRef rPresets As clsPresets, _
                                                    ByRef rDataset As DataSet) As Boolean
        '********************************************************************************************
        'Description:  Save data stored on GUI in database, xml file etc.
        '
        'Parameters: The preset collection, the dataset to write back to database
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/01/09  MSW     CC preset support
        ' 4/30/10   MSW     check for style selection first
        '********************************************************************************************
        Dim DB As clsSQLAccess = New clsSQLAccess
        Dim sTableName As String = gsPRESET_TABLE_PREFIX & rRobot.Name
        Dim DR As DataRow = Nothing
        Dim oP As clsPreset = Nothing
        Dim DT As DataTable = New DataTable
        Dim nColor As Integer
        Dim i%
        Dim sName As String
        Status = gcsRM.GetString("csSAVE_GUI")
        Try

            With DB
                .DBFileName = gsPROCESS_DBNAME
                .DBTableName = sTableName
                .Zone = colZones.ActiveZone
                If rPresets.ColorChangePresets Then
                    nColor = 0
                    sName = rRobot.Controller.Name
                Else
                    nColor = rPresets.FanucColor
                    sName = rRobot.Name
                End If
                ' 4/30/10   MSW     check for style selection first
                If mbPresetsByStyle Then
                    rDataset = GetPresetDataset(DB, sTableName, nColor, rPresets.StyleNumber)
                Else
                    rDataset = GetPresetDataset(DB, sTableName, nColor)
                End If

                
                If rDataset.Tables.Contains("[" & sTableName & "]") Then
                    DT = rDataset.Tables("[" & sTableName & "]")

                    For i% = 1 To rPresets.Count
                        oP = rPresets.Item(i%)
                        If i% > DT.Rows.Count Then

                            DR = DT.NewRow
                            DR.BeginEdit()
                            DR.Item(gsPRESET_COL) = i%
                            DR.Item(gsFANUC_COLOR_COL) = nColor
                            DR.Item(gsFANUC_STYLE_COL) = rPresets.StyleNumber
                            DR.Item(gsVALVE_COL) = 0 'looks like this was put in there, but not used
                            DR.Item(gsDESC_COL) = oP.Description.Text
                            DR.Item(gsESTATDESC_COL) = oP.EstatDescription.Text

                            DR.EndEdit()
                            DT.Rows.Add(DR)
                        Else
                            'Temp save to add style number
                            'DR = DT.Rows(i% - 1)
                            'DR.BeginEdit()
                            'DR.Item(gsFANUC_STYLE_COL) = rPresets.StyleNumber
                            'DR.EndEdit()
                        End If

                        If oP.Description.Changed Then
                            DR = DT.Rows(i% - 1)
                            DR.BeginEdit()
                            DR.Item(gsDESC_COL) = oP.Description.Text
                            'update change log
                            UpdateChangeLog(oP.Index, oP.Description.Text, _
                                             oP.Description.OldText, _
                                             rPresets.ParameterName(ePresetParam.Description), _
                                             sName, rPresets.DisplayName, rRobot.ZoneNumber, _
                                             rRobot.Controller.Zone.Name)
                            oP.Description.Update()
                            DR.EndEdit()
                        End If

                        '6/20/08
                        If oP.EstatDescription.Changed Then
                            DR = DT.Rows(i% - 1)
                            DR.BeginEdit()
                            DR.Item(gsESTATDESC_COL) = oP.EstatDescription.Text
                            'update change log
                            UpdateChangeLog(oP.Index, oP.EstatDescription.Text, _
                                             oP.EstatDescription.OldText, _
                                             rPresets.ParameterName(ePresetParam.EstatDescription), _
                                             sName, rPresets.DisplayName, rRobot.ZoneNumber, _
                                             rRobot.Controller.Zone.Name)
                            oP.EstatDescription.Update()
                            DR.EndEdit()
                        End If

                    Next
                Else
                    .Close()
                    Return False
                End If
                .UpdateDataSet(rDataset, sTableName)
                .Close()
            End With


            Return True


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bSaveToPLC() As Boolean
        '********************************************************************************************
        'Description:  Save data stored on PLC.
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Friend Function bSaveToRobot(ByRef rRobot As clsArm, ByRef rPresets As clsPresets) As Boolean
        '********************************************************************************************
        'Description:  Save data stored on Robot
        '
        'Parameters: none
        'Returns:    True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/01/09  MSW     CC preset support
        '********************************************************************************************

        Try

            Status = gcsRM.GetString("csSAVE_ROBOT")
            If rRobot.IsOnLine Then
                If mPresetCommon.SavePresetsToRobot(rRobot, rPresets) Then
                    Return True
                Else
                    Return False
                End If

            Else
                Return False
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bValidateData() As Boolean
        '********************************************************************************************
        'Description:  Data Validate Routine
        '
        'Parameters: none
        'Returns:    false if bum data so it cam be fixed
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        ' temp hack to fire validate
        cboZone.Focus()

        Return True ' temp
    End Function
    Private Sub subTextboxChangeHandler(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  generic textbox change Routine
        '
        'Parameters: event parameters
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEventBlocker Then Exit Sub
        If DataLoaded And EditsMade = False Then EditsMade = True

    End Sub
    Private Sub subTextboxKeypressHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        '********************************************************************************************
        'Description: Check for keypress that requires login
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/18/09  MSW     subTextboxKeypressHandler, subSetTextBoxProperties - Offer a login if they try to enter data without a login
        ' 07/27/12  JBW     Change to disallow "<" and ">" characters
        '********************************************************************************************
        Dim oFTB As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)

        If oFTB.ReadOnly Then
            If Char.IsNumber(e.KeyChar) OrElse Char.IsLetter(e.KeyChar) Then
                If Privilege = ePrivilege.None Then
                    Privilege = ePrivilege.Edit
                    'make sure its good data
                    If Not bValidateData() Then Exit Sub
                End If
            End If
        End If

        'JBW disallow any and all "<" and ">" characters, they will crash changelog from logging anything
        If oFTB.Name.Contains("txtDesc") = True Then
            If e.KeyChar = "<" Or e.KeyChar = ">" Then
                e.KeyChar = Nothing
            End If
        End If

    End Sub

    Private Sub subTextboxKeyUpHandler(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        '********************************************************************************************
        'Description: Check for keyup from navigate keys
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/07/10  MSW     handle page up, page down
        '********************************************************************************************
        Dim oFTB As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)

        Select Case e.KeyCode
            Case Keys.PageDown
                'validate the current box before shifting it around
                cboZone.Focus()
                Application.DoEvents()
                Dim nTmp As Integer = vsbItems.Value + vsbItems.LargeChange
                If nTmp > (vsbItems.Maximum + 1 - vsbItems.LargeChange) Then
                    nTmp = (vsbItems.Maximum + 1 - vsbItems.LargeChange)
                    Dim sName As String = Strings.Left(oFTB.Name, (Len(oFTB.Name) - 2))
                    oFTB = DirectCast(pnlMain.Controls(sName & mnITEMS_PER_PAGE.ToString("00")), FocusedTextBox.FocusedTextBox)
                End If
                vsbItems.Value = nTmp
                oFTB.Focus()
            Case Keys.PageUp
                'validate the current box before shifting it around
                cboZone.Focus()
                Application.DoEvents()
                Dim nTmp As Integer = vsbItems.Value - vsbItems.LargeChange
                If nTmp < vsbItems.Minimum Then
                    nTmp = vsbItems.Minimum
                    Dim sName As String = Strings.Left(oFTB.Name, (Len(oFTB.Name) - 2))
                    oFTB = DirectCast((pnlMain.Controls(sName & "01")), FocusedTextBox.FocusedTextBox)
                End If
                vsbItems.Value = nTmp
                oFTB.Focus()
            Case Else
        End Select
    End Sub

    Private Sub subTextboxValidatedHandler(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validated Routine
        '
        'Parameters: event parameters
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/01/09  MSW     CC preset support
        '********************************************************************************************
        Dim oT As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim nBoxPtr As Integer = CType(Strings.Right(oT.Name, 2), Integer)
        Dim nOffset As Integer = 0
        Dim sText As String = oT.Text
        Dim bBadData As Boolean = False
        Dim sName As String = Strings.Left(oT.Name, (Len(oT.Name) - 2))
        Dim Parm As ePresetParam
        Dim lblTmp As Label = Nothing


        If DataLoaded = False Then Exit Sub

        Select Case sName
            Case "txtDesc"
                '6/20/08
                If meScreen <> eScreenSelect.EstatPresets Then
                    Parm = ePresetParam.Description
                Else
                    Parm = ePresetParam.EstatDescription
                End If
            Case Else
                Parm = CType(CInt(sName.Substring(6, 2)) - 1, ePresetParam) ' 0 = first column
                lblTmp = DirectCast(pnlMain.Controls("lbl" & oT.Name.Substring(3)), Label)
        End Select


        'collection pointer
        nOffset = vsbItems.Value + nBoxPtr

        'Select CC or color presets
        Dim oPresets As clsPresets
        If meScreen = eScreenSelect.CCPresets Then
            oPresets = mRobot.SystemColors(1, False).Presets
        Else
            oPresets = mRobot.SystemColors(msOldColor).Presets
        End If

        If Parm = ePresetParam.Description Then
            'description boxes
            With oPresets.Item(nOffset).Description
                .Text = oT.Text
                If Not EditsMade Then
                    If .Changed Then EditsMade = True
                End If
            End With
        ElseIf Parm = ePresetParam.EstatDescription Then   '6/20/08
            With oPresets.Item(nOffset).EstatDescription
                .Text = oT.Text
                If Not EditsMade Then
                    If .Changed Then EditsMade = True
                End If
            End With
        Else
            'everybody else
            With oPresets.Item(nOffset)
                .Param(Parm).Value = CSng(oT.Text)
                If Not EditsMade Then
                    If .Changed Then EditsMade = True
                End If
                lblTmp.Text = .EffectiveValue(Parm).ToString
            End With

        End If

    End Sub
    Private Sub subTextboxValidatingHandler(ByVal sender As Object, _
                                ByVal e As System.ComponentModel.CancelEventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validating Routine
        '
        'Parameters: event parametersx
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/01/09  MSW     CC preset support
        '********************************************************************************************
        Dim oT As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim nBoxPtr As Integer = CType(Strings.Right(oT.Name, 2), Integer)
        Dim nOffset As Integer = 0
        Dim sText As String = oT.Text
        Dim bBadData As Boolean = False
        Dim sName As String = Strings.Left(oT.Name, (Len(oT.Name) - 2))
        Dim Parm As ePresetParam

        Select Case sName
            Case "txtDesc"
                If meScreen = eScreenSelect.EstatPresets Then
                    Parm = ePresetParam.EstatDescription
                Else
                    Parm = ePresetParam.Description
                End If
            Case Else
                Parm = CType(CInt(sName.Substring(6, 2)) - 1, ePresetParam) ' 0 = first column
        End Select

        'if no edit priv and bad data this locks us up
        If Privilege = ePrivilege.None Then

            e.Cancel = True
            Exit Sub
        End If

        'no value?
        If Strings.Len(sText) = 0 Then
            bBadData = True
            MessageBox.Show(gcsRM.GetString("csNO_NULL"), gcsRM.GetString("csINVALID_DATA"), _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

        'collection pointer
        nOffset = vsbItems.Value + nBoxPtr

        'description boxes
        If (Parm = ePresetParam.Description) Or (Parm = ePresetParam.EstatDescription) Then
            'only check for blank
            If bBadData Then
                oT.Undo()
                e.Cancel = True
            End If
            Exit Sub
        End If

        'Select CC or color presets
        Dim oPresets As clsPresets
        If meScreen = eScreenSelect.CCPresets Then
            oPresets = mRobot.SystemColors(1, False).Presets
        Else
            oPresets = mRobot.SystemColors(msOldColor).Presets
        End If

        With oPresets.Item(nOffset).Param(Parm)

            'numeric
            If bBadData = False Then
                If oT.NumericOnly Then
                    If Not (IsNumeric(sText)) Then
                        bBadData = True
                        MessageBox.Show(gcsRM.GetString("csBAD_ENTRY"), gcsRM.GetString("csINVALID_DATA"), _
                                       MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If

                    ' limit check
                    If bBadData = False Then

                        'low limit
                        If CSng(sText) < .MinValue And Not (mbSkipMinCheck) Then
                            bBadData = True
                            MessageBox.Show(gcsRM.GetString("csVAL_BELOW_MIN") & vbCrLf & _
                                            gcsRM.GetString("csMINIMUM_EQ") & .MinValue, _
                                            gcsRM.GetString("csINVALID_DATA"), _
                                           MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        End If

                        'hi limit
                        If CSng(sText) > .MaxValue Then
                            bBadData = True
                            MessageBox.Show(gcsRM.GetString("csVAL_ABOVE_MAX") & vbCrLf & _
                                            gcsRM.GetString("csMAXIMUM_EQ") & .MaxValue, _
                                            gcsRM.GetString("csINVALID_DATA"), _
                                           MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        End If

                    End If '  bBadData = False 
                End If ' oT.NumericOnly
            End If ' bBadData = False

            If bBadData Then
                oT.Text = .Value.ToString
                e.Cancel = True
            End If

        End With

    End Sub
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
        ' 06/01/09  MSW     CC preset support
        '********************************************************************************************
        Progress = 10

        If EditsMade Then
            ' false means user pressed cancel
            If bAskForSave() = False Then
                cboZone.Text = msOldZone
                Progress = 100
                Exit Sub
            End If
        End If  'EditsMade

        'just in case
        If cboZone.Text = String.Empty Then
            Exit Sub
        Else
            If cboZone.Text = msOldZone Then Exit Sub
        End If


        If colZones.SetCurrentZone(cboZone.Text) = False Then
            If msOldZone <> String.Empty Then
                cboZone.Text = msOldZone
            Else
                cboZone.SelectedIndex = -1
            End If

            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default

            Exit Sub
        End If

        msOldZone = cboZone.Text

        '        msColorCache = String.Empty

        Try

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subClearScreen()

            subEnableControls(False)

            colControllers = New clsControllers(colZones, False)
            SetUpStatusStrip(Me, colControllers)
            colArms = LoadArmCollection(colControllers)

            If meScreen = eScreenSelect.CCPresets Then
                mPWRobotCommon.LoadRobotBoxFromCollection(cboRobot, colControllers, False, IncludeOpeners:=False)
            Else
                mPWRobotCommon.LoadRobotBoxFromCollection(cboRobot, colArms, False, IncludeOpeners:=False)
            End If
            'make sure the robot cartoons get the right color
            colControllers.RefreshConnectionStatus()
            colApplicators = New clsApplicators(colArms, colZones.ActiveZone)
            meOldApplicator = eColorChangeType.NOT_SELECTED
            If colApplicators.Count = 1 Then
                mApplicator = colApplicators.Item(0)
            End If
            cboColor.Items.Clear()
            msOldColor = String.Empty
            msOldRobot = String.Empty
            mnOldStyle = 0
            'statusbar text
            If cboRobot.Text = String.Empty Then
                Status(True) = gcsRM.GetString("csSELECT_ROBOT")
            End If
            'Get the first robot's settings for presets by style, so copy can get set up
            If meScreen <> frmMain.eScreenSelect.CCPresets And colArms(0).Controller.Version >= 7.5 Then
                '[PAVROPTN] byOptn[1].byOp[78]	(Presets by JOB/STYLE, 0 = NO – 1 = YES)
                colArms(0).ProgramName = "PAVROPTN"
                colArms(0).VariableName = "byOptn[1].byOp[78]"
                mbPresetsByStyle = (CInt(colArms(0).VarValue) = 1)
                colArms(0).ProgramName = "PAVRSYSC"
                '[PAVRSYSC] qty_prststyl	(Number of jobs/styles)
                colArms(0).VariableName = "qty_prststyl"
                mnNumStyles = CInt(colArms(0).VarValue)
                If mbPresetsByStyle Then
                    lblStyle.Text = gpsRM.GetString("psSTYLE_CAP")
                    cboStyle.Items.Clear()
                    cboStyle.Text = String.Empty
                    Dim oStyles As New clsSysStyles(colZones.ActiveZone)
                    oStyles.LoadStyleBoxFromCollection(cboStyle, False, True)
                End If
            Else
                mbPresetsByStyle = False
                mnNumStyles = 1
            End If
            subInitFormText()

            If cboRobot.Items.Count = 1 Then
                'this selects the robot in cases of just one robot. fires event to call subchangerobot
                cboRobot.Text = cboRobot.Items(0).ToString
            ElseIf msCmdLineRobot <> String.Empty Then
                'Robot name from command line
                If meScreen = eScreenSelect.CCPresets Then
                    For Each oc As clsController In colControllers
                        If oc.Name = msCmdLineRobot Then
                            Exit For
                        End If
                        For Each oa As clsArm In oc.Arms
                            If oa.Name = msCmdLineRobot Then
                                msCmdLineRobot = oc.Name
                                Exit For
                            End If
                        Next
                    Next
                End If
                cboRobot.Text = msCmdLineRobot
            End If

            'If colZones.Item(colZones.CurrentZone).PresetsByStyle Then
            '    cboParam.Width = 380
            'Else
            '    cboParam.Width = 280
            'End If

            ' copy button
            subEnableControls(True)

            Me.stsStatus.Refresh()

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
        'this should be handled better
        Dim bTmp As Boolean = mbEventBlocker
        mbEventBlocker = True

        DataLoaded = False ' needs to be first
        subEnableControls(False)
        ClearAllTextBoxes(colTextboxes)
        mbEventBlocker = bTmp

    End Sub
    Private Sub subCopyButtonPressed()
        '********************************************************************************************
        'Description: Copy button pressed check for edits first
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/30/12  RJO     Added code to set new frmCopy ParamDetails property to support copy 
        '                   confimation message boxes for all copy buttons.
        '********************************************************************************************
        'make sure edits are saved
        If EditsMade Then
            Dim lRet As Response

            lRet = MessageBox.Show(gcsRM.GetString("csSAVEMSG3") & vbCrLf & _
                                gcsRM.GetString("csSAVEMSG"), msSCREEN_NAME, _
                                MessageBoxButtons.OKCancel, _
                                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)

            Select Case lRet
                Case Response.OK
                    subSaveData()
                Case Else
                    Exit Sub
            End Select
        End If

        'launch copy screen

        Dim frmCopy As New frmCopy

        Try
            With frmCopy

                If colZones.PaintShopComputer Then
                    '   force reload from zoneinfo table on this computer
                    .DatabasePath = String.Empty
                Else
                    .DatabasePath = colZones.DatabasePath
                End If

                .ScreenName = msSCREEN_NAME
                .ParamDetails = mParmDetail 'RJO 04/30/12

                .ByArm = Not (meScreen = eScreenSelect.CCPresets)
                .IncludeOpeners = False
                If mRobot IsNot Nothing Then
                    .CCType1 = mRobot.ColorChangeType
                Else
                    If (colArms IsNot Nothing) AndAlso (colArms.Count > 0) Then
                        .CCType1 = colArms(0).ColorChangeType
                    Else
                        .CCType1 = eColorChangeType.NOT_NONE
                    End If
                End If
                Dim nTmp As eCopyType
                .UseSubParam = True
                .UseStyle = mbPresetsByStyle
                Dim nCopyParm As Integer = 0
                For nParm As Integer = 0 To mnNumParms - 1
                    nCopyParm = nCopyParm + CInt(2 ^ nParm)
                Next
                nCopyParm = nCopyParm + eCopyType.CopyDesc
                Select Case meScreen
                    Case eScreenSelect.CCPresets
                        .UseParam = False
                        nTmp = CType(nCopyParm, eCopyType)
                    Case eScreenSelect.EstatPresets
                        .UseParam = True
                        nTmp = CType(nCopyParm, eCopyType)
                    Case eScreenSelect.FluidPresets
                        .UseParam = True
                        nTmp = CType(nCopyParm, eCopyType)
                End Select
                .btnCopy.Tag = nTmp

                Dim nTmp1 As eCopyType = eCopyType.CopyDesc
                .btnCopyD.Tag = nTmp1


                With .btnCopyV
                    Dim dd As New ToolStripMenuItem
                    dd.Text = gpsRM.GetString("psSELECTCOPY_PARM")
                    .DropDownItems.Add(dd)
                    'plant request they all start disabled
                    With dd
                        Dim sb As ToolStripButton


                        For nParm As Integer = 1 To mnNumParms
                            sb = New ToolStripButton
                            sb.CheckOnClick = True
                            sb.CheckState = CheckState.Unchecked 'CheckState.Checked
                            sb.Text = mParmDetail(nParm - 1).sLblName
                            sb.Tag = CInt(2 ^ (nParm - 1))
                            .DropDownItems.Add(sb)
                        Next


                    End With
                End With

                .ShowDialog()
                If DataLoaded Then
                    subLoadData()
                End If
            End With

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            frmCopy = Nothing
        End Try


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

        DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)

    End Sub
    Private Sub subEffectiveLabelsVisible(ByVal Param As Integer, ByVal bVisible As Boolean)
        '********************************************************************************************
        'Description: show or hide effective labels 
        '
        'Parameters: bEnable - hide, which param (1 = 1st column)
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sName As String = String.Empty
        Dim sFullName As String = String.Empty
        Dim i%
        Select Case meScreen
            Case eScreenSelect.FluidPresets, eScreenSelect.EstatPresets
                sName = "lblCol" & Param.ToString("00") & "Prst"
            Case Else
                'No overrides for cc presets
                Exit Sub
        End Select

        For i% = 0 To mnITEMS_PER_PAGE
            sFullName = sName & Format(i%, "00")
            pnlMain.Controls(sFullName).Visible = bVisible
        Next

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
        ' 10/07/10  MSW     Remove save by valve for now
        '********************************************************************************************
        Dim bRestOfControls As Boolean = False


        btnClose.Enabled = True

        If bEnable = False Then
            btnSave.Enabled = False
            btnSaveNoMenu.Enabled = False
            btnUndo.Enabled = False
            btnCopy.Enabled = False
            btnPrint.Enabled = False
            btnChangeLog.Enabled = False
            btnStatus.Enabled = True
            btnMultiView.Enabled = True
            bRestOfControls = False
            pnlMain.Enabled = False
            btnOverride.Enabled = False
            btnUtilities.Enabled = False
        Else
            Select Case Privilege
                Case ePrivilege.None
                    'btnSave.Enabled = False
                    btnSaveNoMenu.Enabled = False
                    btnUndo.Enabled = False
                    btnCopy.Enabled = False
                    btnPrint.Enabled = (True And DataLoaded)
                    btnChangeLog.Enabled = (True And DataLoaded)
                    btnStatus.Enabled = True
                    bRestOfControls = False
                    pnlMain.Enabled = True
                    btnOverride.Enabled = False
                    btnCopy.Enabled = False
                    btnMultiView.Enabled = True
                    mnuPrintFile.Enabled = False
                    btnUtilities.Enabled = False
                Case ePrivilege.Edit
                    'btnSave.Enabled = (True And EditsMade) And Not (meScreen = eScreenSelect.CCPresets)
                    btnSaveNoMenu.Enabled = (True And EditsMade) 'And (meScreen = eScreenSelect.CCPresets)
                    btnUndo.Enabled = (True And EditsMade)
                    btnPrint.Enabled = (True And DataLoaded)
                    btnMultiView.Enabled = True
                    btnChangeLog.Enabled = (True And DataLoaded)
                    btnStatus.Enabled = True
                    bRestOfControls = True
                    pnlMain.Enabled = (True And DataLoaded)
                    btnOverride.Enabled = False
                    btnCopy.Enabled = False
                    mnuPrintFile.Enabled = (True And DataLoaded)
                    btnUtilities.Enabled = (True And DataLoaded)
                Case ePrivilege.Copy
                    btnCopy.Enabled = (True And DataLoaded) 'True 01/02/10 RJO
                    'btnSave.Enabled = (True And EditsMade) And Not (meScreen = eScreenSelect.CCPresets)
                    btnSaveNoMenu.Enabled = (True And EditsMade) 'And (meScreen = eScreenSelect.CCPresets)
                    btnUndo.Enabled = (True And EditsMade)
                    btnPrint.Enabled = (True And DataLoaded)
                    btnMultiView.Enabled = True
                    btnChangeLog.Enabled = (True And DataLoaded)
                    btnStatus.Enabled = True
                    bRestOfControls = True
                    pnlMain.Enabled = (True And DataLoaded)
                    btnOverride.Enabled = False '(True And DataLoaded) 01/05/10 RJO
                    mnuPrintFile.Enabled = (True And DataLoaded)
                    btnUtilities.Enabled = (True And DataLoaded)
                Case ePrivilege.Execute '01/05/10 RJO
                    btnCopy.Enabled = (True And DataLoaded)
                    'btnSave.Enabled = (True And EditsMade) And Not (meScreen = eScreenSelect.CCPresets)
                    btnSaveNoMenu.Enabled = (True And EditsMade) 'And (meScreen = eScreenSelect.CCPresets)
                    btnUndo.Enabled = (True And EditsMade)
                    btnPrint.Enabled = (True And DataLoaded)
                    btnMultiView.Enabled = True
                    btnChangeLog.Enabled = (True And DataLoaded)
                    btnStatus.Enabled = True
                    bRestOfControls = True
                    pnlMain.Enabled = (True And DataLoaded)
                    btnOverride.Enabled = (True And DataLoaded)
                    mnuPrintFile.Enabled = (True And DataLoaded)
                    btnUtilities.Enabled = (True And DataLoaded)
            End Select
        End If

        'btnsave.BackColor

        'restof controls here
        LockAllTextBoxes(colTextboxes, (Not bRestOfControls))


    End Sub
    Private Sub subFormatScreenLayout(Optional ByVal NumberOfPresets As Integer = 0)
        '********************************************************************************************
        'Description:  new data selected - set up the screen layout
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If NumberOfPresets > 0 Then
            mnNumberofpresets = NumberOfPresets
        End If
        With vsbItems
            If mnNumberofpresets > mnITEMS_PER_PAGE Then
                .Maximum = mnNumberofpresets - 1 'mnITEMS_PER_PAGE
            Else
                .Maximum = mnITEMS_PER_PAGE
            End If
            .Minimum = 0
            .LargeChange = mnITEMS_PER_PAGE
            .SmallChange = 1
            '.Value = 0
            If .Value > (.Maximum + 1 - mnITEMS_PER_PAGE) Then
                .Value = .Maximum + 1 - mnITEMS_PER_PAGE
            End If
        End With

    End Sub
    Private Sub subInitFormText()
        '********************************************************************************************
        'Description: load text for form labels etc
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/01/09  MSW     removed ApplicatorType, use ColorChangeType instead
        ' 10/07/10  MSW     Remove save by valve for now
        ' 04/02/13  DE      Localize lblPage and lblItem text
        ' 11/13/13  BTK     Override labels not place correctly.  Shaping air 2 not displayed.
        '********************************************************************************************

        With gpsRM
            Dim bTmp As Boolean = True
            Dim bBell As Boolean = True
            Dim eEstat As eEstatType

            'DE 04/02/13
            lblPage.Text = .GetString("psPAGE_CAP")
            lblItem.Text = .GetString("psITEM_CAP")

            If ((mRobot Is Nothing) = False) Then
                eEstat = mRobot.EstatType
                'Init Columns
                Dim ParmSelect As mPWRobotCommon.eParmSelect = eParmSelect.All
                Select Case meScreen
                    Case frmMain.eScreenSelect.CCPresets
                        ParmSelect = eParmSelect.CCPresets
                    Case frmMain.eScreenSelect.EstatPresets
                        ParmSelect = eParmSelect.EstatPresets
                    Case frmMain.eScreenSelect.FluidPresets
                        ParmSelect = eParmSelect.FluidPresets
                End Select
                subInitParmConfig(mRobot, ParmSelect, mParmDetail, mnNumParms, mapplicator)
                If mnNumParms > mnMaxParms Then
                    mnMaxParms = mnNumParms
                End If
            Else
                'Init columns w/o robot info
                Select Case meScreen
                    Case eScreenSelect.FluidPresets, eScreenSelect.CCPresets
                        mnNumParms = 3
                        ReDim mParmDetail(mnNumParms - 1)
                        mParmDetail(0).sLblCap = .GetString("psFLUIDCAP")
                        mParmDetail(0).nParmNum = 1
                        mParmDetail(1).nParmNum = 2
                        mParmDetail(2).nParmNum = 3
                        If bBell Then
                            mParmDetail(1).sLblCap = .GetString("psBELLSPDCAP")
                            mParmDetail(2).sLblCap = .GetString("psSHAPCAP")
                        Else
                            mParmDetail(1).sLblCap = .GetString("psATOMCAP")
                            mParmDetail(2).sLblCap = .GetString("psFANCAP")
                        End If
                    Case eScreenSelect.EstatPresets
                        mnNumParms = 1
                        eEstat = eEstatType.None
                        ReDim mParmDetail(mnNumParms - 1)
                        mParmDetail(0).nParmNum = 1
                        Select Case eEstat
                            Case eEstatType.FB200, eEstatType.Honda
                                mParmDetail(0).sLblCap = .GetString("psESTATSTEPCAP")
                            Case Else
                                mParmDetail(0).sLblCap = .GetString("psESTATCAP")
                        End Select
                End Select
            End If

            Dim txtTmp As FocusedTextBox.FocusedTextBox
            Dim lblTmp As Label = Nothing
            Dim nlblCapXDiff As Integer = lblCol02PrstCap.Left - lblCol01PrstCap.Left
            Dim ptCap As Point = lblCol01PrstCap.Location
            Dim ptLbl00 As Point = lblCol01Prst00.Location
            Dim ptOrid As Point = lblOrid01.Location
            mnRowHeight = txtCol01Prst02.Top - txtCol01Prst01.Top
            mnLblHeight = txtCol01Prst01.Height
            lblDescCap.Left = lblCol01Prst00.Left + (nlblCapXDiff * mnNumParms)
            lblCol01Prst01.Visible = False

            frmMain_Layout()

            'hide for cc presets
            Dim bOrideEnable As Boolean = True
            If (meScreen = eScreenSelect.CCPresets) Then
                bOrideEnable = False
            ElseIf (meScreen = eScreenSelect.EstatPresets) And ((eEstat = eEstatType.FB200) Or (eEstat = eEstatType.Honda)) Then
                bOrideEnable = False
            End If
            btnOverride.Visible = bOrideEnable   '% override
            lblOverride.Visible = bOrideEnable
            For nCol As Integer = 1 To mnMaxParms
                Dim bCol As Boolean = (nCol <= mnNumParms)
                For nRow As Integer = 1 To mnITEMS_PER_PAGE
                    If nCol = 1 Then
                        ShowNewRow(nRow)
                    End If
                    txtTmp = DirectCast(pnlMain.Controls("txtCol" & nCol.ToString("00") & "Prst" & nRow.ToString("00")), FocusedTextBox.FocusedTextBox)
                    txtTmp.Visible = bCol
                Next

                lblTmp = (DirectCast(pnlMain.Controls("lblCol" & nCol.ToString("00") & "Prst00"), Label))
                MakeLabel(lblTmp, ("lblCol" & nCol.ToString("00") & "Prst00"), ptLbl00, lblCol01Prst00)
                ptLbl00.X = ptLbl00.X + nlblCapXDiff
                lblTmp.Text = .GetString("psEFFECTIVE")
                lblTmp.TextAlign = ContentAlignment.TopCenter
                lblTmp.Visible = False

                lblTmp = (DirectCast(pnlMain.Controls("lblCol" & nCol.ToString("00") & "PrstCap"), Label))
                MakeLabel(lblTmp, ("lblCol" & nCol.ToString("00") & "PrstCap"), ptCap, lblCol01PrstCap)
                lblTmp = DirectCast(pnlMain.Controls("lblCol" & nCol.ToString("00") & "PrstCap"), Label)
                lblTmp.TextAlign = ContentAlignment.TopCenter
                If bCol Then
                    lblTmp.Text = mParmDetail(nCol - 1).sLblCap
                End If
                lblTmp.Visible = bCol
                ptCap.X = ptCap.X + nlblCapXDiff


                lblTmp = DirectCast(tscMain.ContentPanel.Controls("lblOrid" & nCol.ToString("00")), Label)
                '11/15/13 BTK No need to make a label that already exists or move it dynamically.
                'MakeLabel(lblTmp, ("lblOrid" & nCol.ToString("00")), ptOrid, lblCol01PrstCap)
                'Me.tscMain.ContentPanel.Controls.Add(lblTmp)
                lblTmp.TextAlign = ContentAlignment.BottomCenter
                lblTmp.Visible = nCol <= mnNumParms And bOrideEnable
                lblTmp.Text = String.Empty

                'ptOrid.X = ptOrid.X + lblTmp.Height
            Next
            lblDescCap.Visible = True
            lblDescCap.Text = .GetString("psDESCCAP")

            'menu buttons
            btnEstatPresets.Text = .GetString("psESTAT_PRESETS_BUTTON")
            btnFluidPresets.Text = .GetString("psFLUID_PRESETS_BUTTON")
            btnCCPresets.Text = .GetString("psCC_PRESETS_BUTTON")

            btnOverride.Text = .GetString("psOVERRIDES_BUTTON")

            lblColor.Visible = (meScreen <> eScreenSelect.CCPresets)        'Color select
            cboColor.Visible = (meScreen <> eScreenSelect.CCPresets)
            If mbPresetsByStyle Then
                lblStyle.Text = .GetString("psSTYLE_CAP")
                lblOverride.Left = cboStyle.Left + cboStyle.Width + 10
                lblStyle.Visible = True
                cboStyle.Visible = True
            Else
                lblStyle.Visible = False
                cboStyle.Visible = False
                lblOverride.Left = cboStyle.Left
            End If
            lblOrid01.Left = lblOverride.Left + 38
            lblOrid02.Left = lblOverride.Left + 38
            lblOrid03.Left = lblOverride.Left + 38
            lblOrid04.Left = lblOverride.Left + 38


            'Remove save by valve for now
            'Save by valve
            'mnuSaveValve.Visible = (meScreen <> eScreenSelect.CCPresets)
            'btnSave.Visible = (meScreen <> eScreenSelect.CCPresets)
            btnSaveNoMenu.Visible = True '(meScreen = eScreenSelect.CCPresets)
            ' mnuCtxSave.MenuItems(0).Text = .GetString("psSAVE_BY_VALVE")
        End With
    End Sub
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
        ' 06/01/09  MSW     Color Change preset support
        ' 11/12/09  MSW     subInitializeForm - Move LoadTextBoxCollection and subSetTextBoxProperties to before the layout,
        '   it needs to get called for the collection before we start adding individual boxes so they
        '   don't get addHandler twice
        '********************************************************************************************
        Dim lReply As Windows.Forms.DialogResult = Response.OK

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor


        Status = gcsRM.GetString("csINITALIZING")
        Progress = 1

        Try
            DataLoaded = False
            EditsMade = False
            mbScreenLoaded = False
            If mbFirstPass Then
                subProcessCommandLine(meScreen, msCmdLineZone, msCmdLineRobot)
                Select Case meScreen
                    Case eScreenSelect.CCPresets
                        btnCCPresets.Checked = True
                        btnEstatPresets.Checked = False
                        btnFluidPresets.Checked = False
                    Case eScreenSelect.EstatPresets
                        btnCCPresets.Checked = False
                        btnEstatPresets.Checked = True
                        btnFluidPresets.Checked = False
                    Case eScreenSelect.FluidPresets
                        btnCCPresets.Checked = False
                        btnEstatPresets.Checked = False
                        btnFluidPresets.Checked = True
                    Case Else
                        btnCCPresets.Checked = False
                        btnEstatPresets.Checked = False
                        btnFluidPresets.Checked = False
                End Select
                mbFirstPass = False
            Else
                msOldZone = String.Empty
                msOldRobot = String.Empty
                msOldColor = String.Empty
                mnOldStyle = 0
                mRobot = Nothing
            End If
            mPresetCommon.Screen = meScreen

            'MSW 11/12/09 move LoadTextBoxCollection and subSetTextBoxProperties to before the layout,
            '   it needs to get called for the collection before we start adding individual boxes so they
            '   don't get addHandler twice
            mScreenSetup.LoadTextBoxCollection(Me, "pnlMain", colTextboxes)
            subSetTextBoxProperties()

            subInitFormText()
            'init the old password for now
            'moPassword = New PWPassword.PasswordObject 'RJO 03/23/12
            'mPassword.InitPassword(moPassword, moPrivilege, LoggedOnUser, msSCREEN_NAME)
            'If LoggedOnUser <> String.Empty Then
            '    Privilege = ePrivilege.Execute 'don't allow Override Access without this priviledge 01/05/10 RJO
            '    If Privilege <> ePrivilege.Execute Then
            '        Privilege = ePrivilege.Copy ' extra for buttons etc.
            '        If Privilege <> ePrivilege.Copy Then
            '            'didn't have clearance
            '            Privilege = ePrivilege.Edit
            '            If moPrivilege.ActionAllowed Then
            '                Privilege = mPassword.PrivilegeStringToEnum(moPrivilege.Privilege)
            '            Else
            '                Privilege = ePrivilege.None
            '            End If
            '        End If
            '    End If
            'Else
            '    Privilege = ePrivilege.None
            'End If

            Progress = 5

            Me.Text = gpsRM.GetString("psSCREENCAPTION")

            'init new IPC and new Password 'RJO 03/23/12
            'MSW 03/28/12 prevent multiple objects when switching between preset screens.
            If oIPC Is Nothing Then
                oIPC = New Paintworks_IPC.clsInterProcessComm
            End If
            If moPassword Is Nothing Then
                moPassword = New clsPWUser
            End If
            Progress = 50

            colZones = New clsZones(String.Empty)
            mScreenSetup.LoadZoneBox(cboZone, colZones, False)
            mScreenSetup.InitializeForm(Me)
            'Custom button, copy text from the normal button
            btnSaveNoMenu.Text = btnSave.Text
            btnSaveNoMenu.Image = btnSave.Image
            'Custom scrolling, disable autoscroll set in common routine
            pnlMain.AutoScroll = False

            mPrintHtml = New clsPrintHtml(msSCREEN_NAME, True, 30)

            Progress = 85

            subEnableControls(True)

            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound

            'statusbar text
            If cboZone.Text = String.Empty Then
                Status(True) = gcsRM.GetString("csSELECT_ZONE")
            Else
                If cboRobot.Text = String.Empty Then
                    Status(True) = gcsRM.GetString("csSELECT_ROBOT")
                End If
            End If


            If cboZone.Items.Count = 1 Then
                'this selects the zone in cases of just one zone. fires event to call subchangezone
                cboZone.Text = cboZone.Items(0).ToString
            ElseIf msCmdLineZone <> String.Empty Then
                'Zone name from command line
                cboZone.Text = msCmdLineZone
                msCmdLineZone = String.Empty
            End If

            ' refresh lock pic
            DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
            mbScreenLoaded = True


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
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
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try


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
        ' 06/01/09  MSW     CC preset support
        '********************************************************************************************

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

        'give whatever called us time to repaint
        System.Windows.Forms.Application.DoEvents()

        DataLoaded = False
        Dim bStatus As Boolean = lstStatus.Visible
        lstStatus.Visible = True

        Try
            Dim sTmp As String = gcsRM.GetString("csLOADINGDATA") & vbTab & _
                                                cboRobot.Text & ";" & vbTab & _
                                                cboColor.Text & ";"
            Status = sTmp
            Progress = 15
            Dim oPresets As clsPresets
            'set up new preset collection
            If meScreen = frmMain.eScreenSelect.CCPresets Then
                oPresets = mRobot.SystemColors(1, False).Presets
                oPresets.DisplayName = grsRM.GetString("rsCC_PRESETS")
                oPresets.FanucColor = 1
                oPresets.ColorChangePresets = True
            Else
                oPresets = mRobot.SystemColors(msOldColor).Presets
                oPresets.DisplayName = cboColor.Text
                oPresets.FanucColor = mRobot.SystemColors(msOldColor).FanucNumber
                If mbPresetsByStyle Then
                    oPresets.StyleNumber = mnOldStyle
                End If
                If mRobot.SystemColors.ColorsByStyle Then
                    sTmp = "FANUC Color " & oPresets.FanucColor
                    Status = sTmp
                End If
            End If
            Progress = 20


            oPresets.EstatType = mRobot.EstatType
            Dim dsDescriptions As DataSet = New DataSet

            If bLoadFromRobot(mRobot, oPresets) Then
                Progress = 30
                If mPresetCommon.LoadOverRidesFromRobot(mRobot, oPresets) Then
                    Progress = 40
                    If bLoadFromPLC() Then
                        If bLoadFromGUI(mRobot.Controller.Zone, mRobot, oPresets, dsDescriptions) Then

                            Progress = 80
                            If (meScreen <> frmMain.eScreenSelect.CCPresets) And ((mRobot.EstatType = eEstatType.FB200) Or (mRobot.EstatType = eEstatType.Honda)) Then
                                If msEstatSteps(1) <> String.Empty Then
                                    For i As Integer = 1 To 7
                                        oPresets.EstatStepValue(i).Value = CType(msEstatSteps(i), Integer)
                                        oPresets.EstatStepValue(i).Update()
                                    Next
                                    oPresets.EstatStepValuesLoaded = True
                                Else
                                    oPresets.EstatStepValuesLoaded = False
                                End If
                            End If

                            Progress = 95

                            subFormatScreenLayout(oPresets.Count)

                            'this refreshes screen & sets edit flag
                            DataLoaded = True
                            Status = gcsRM.GetString("csLOADDONE")

                        Else
                            'Load Failed
                        End If    'bLoadFromRobot()
                    Else
                        'Load Failed
                    End If  'bLoadFromPLC()
                Else
                    'Load Failed
                End If 'LoadOverRidesFromRobot
            Else
                'Load Failed
            End If  ' bLoadFromGUI()


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default

        End Try
        lstStatus.Visible = bStatus
    End Sub
    Private Sub subChangeColor()
        '********************************************************************************************
        'Description:  New Color selected
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If EditsMade Then
            ' false means user pressed cancel
            If bAskForSave() = False Then
                cboColor.Text = msOldColor
                Exit Sub
            End If
        End If  'EditsMade

        'just in case
        If cboColor.Text = String.Empty Then
            Exit Sub
        Else
            If cboColor.Text = msOldColor Then Exit Sub
        End If
        msOldColor = cboColor.Text
        msColorCache = cboColor.Text

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

            subClearScreen()
            If mbPresetsByStyle Then
                If mnOldStyle = 0 Then
                    Progress = 100
                    Me.Cursor = System.Windows.Forms.Cursors.Default
                    Exit Sub
                End If
            End If
            subLoadData()


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
    Private Sub subChangeStyle()
        '********************************************************************************************
        'Description:  New Style selected
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTag As String() = DirectCast(cboStyle.Tag, String())
        If EditsMade Then
            ' false means user pressed cancel
            If bAskForSave() = False Then
                For nIdx As Integer = 0 To sTag.GetUpperBound(0)
                    If CInt(sTag(nIdx)) = mnOldStyle Then
                        cboStyle.SelectedIndex = nIdx
                        Exit Sub
                    End If
                Next
            End If
        End If  'EditsMade
        If cboStyle.SelectedIndex >= 0 Then
            Dim nStyle As Integer = CInt(sTag(cboStyle.SelectedIndex))
            If nStyle <> mnOldStyle Then
                mnOldStyle = nStyle
            Else
                Exit Sub
            End If
        Else
            Exit Sub
        End If

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

            subClearScreen()
            If msOldColor <> String.Empty Then
                subLoadData()
            End If
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
    Private Sub subOverrideButtonPressed()
        '********************************************************************************************
        'Description: Someone clicked override button
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        frmOver = New frmOverrides

        frmOver.Show(mRobot, mRobot.SystemColors(msOldColor).Presets)
        AddHandler frmOver.UpdateInfo, AddressOf subOverRideUpdate

    End Sub
    Private Sub subOverRideUpdate(ByVal bClosing As Boolean)
        '********************************************************************************************
        'Description: event sink for frmOverride to update screen
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subShowNewPage()
        If bClosing Then RemoveHandler frmOver.UpdateInfo, AddressOf subOverRideUpdate
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
        If DataLoaded Then
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
        If DataLoaded Then
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
        If DataLoaded Then
            If bPrintdoc(False) Then
                mPrintHtml.subSaveAs()
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
        If DataLoaded Then
            If bPrintdoc(False) Then
                mPrintHtml.subShowPrintPreview()
            End If
        End If
    End Sub
    Private Function bPrintdoc(ByVal bPrint As Boolean, Optional ByVal bExportCSV As Boolean = False) As Boolean
        '********************************************************************************************
        'Description:  build the print document
        '
        'Parameters:  bPrint - send to printer
        'Returns:   true if successful, OK to continue with preview or page setup
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/07/10  MSW     HTML print doesn't like brackets in the descriptions
        ' 12/01/10  MSW     allow import from and export to csv
        ' 05/03/11  RJO     Bugfix: Estat screen was printing Fluid Preset description
        '********************************************************************************************
        Try

            mPrintHtml.subSetPageFormat()
            mPrintHtml.subClearTableFormat()
            mPrintHtml.subSetColumnCfg("align=right", 0)
            mPrintHtml.subSetRowcfg("Bold=on", 0, 0)

            mPrintHtml.HeaderRowsPerTable = 1

            Dim sPageTitle As String = gpsRM.GetString("psSCREENCAPTION")
            Dim sTitle(1) As String
            sTitle(0) = colZones.SiteName & " - " & colZones.CurrentZone
            sTitle(1) = gcsRM.GetString("csROBOT_CAP") & ":  " & cboRobot.Text

            Dim sSubTitle(0) As String
            sSubTitle(0) = String.Empty
            'Select CC or color presets
            Dim oPresets As clsPresets = Nothing

            Select Case meScreen
                Case eScreenSelect.FluidPresets
                    sPageTitle = gpsRM.GetString("psFLUID_PRESETS_BUTTON")
                    If mbPresetsByStyle Then
                        sSubTitle(0) = gcsRM.GetString("csCOLOR_CAP") & ":  " & cboColor.Text & vbTab & _
                            gpsRM.GetString("psSTYLE_CAP") & ":  " & cboStyle.Text
                    Else
                        sSubTitle(0) = gcsRM.GetString("csCOLOR_CAP") & ":  " & cboColor.Text
                    End If
                    oPresets = mRobot.SystemColors(msOldColor).Presets
                Case eScreenSelect.CCPresets
                    sPageTitle = gpsRM.GetString("psCC_PRESETS")
                    sSubTitle(0) = String.Empty
                    oPresets = mRobot.SystemColors(1, False).Presets
                Case eScreenSelect.EstatPresets
                    sPageTitle = gpsRM.GetString("psESTAT_PRESETS_BUTTON")
                    sSubTitle(0) = gcsRM.GetString("csCOLOR_CAP") & ":  " & cboColor.Text
                    oPresets = mRobot.SystemColors(msOldColor).Presets
            End Select

            'column headers
            Dim sText(0) As String
            sText(0) = lblItem.Text
            For nCol As Integer = 1 To mnNumParms
                Dim sName As String = "lblCol" & nCol.ToString("00") & "PrstCap"
                Dim oLbl As Label = DirectCast(pnlMain.Controls(sName), Label)
                sText(0) = sText(0) & vbTab & oLbl.Text
            Next
            sText(0) = sText(0) & vbTab & lblDescCap.Text
            sText(0) = sText(0).Replace("<", "")
            sText(0) = sText(0).Replace(">", "")
            ReDim Preserve sText(oPresets.Count)
            For nPreset As Integer = 1 To oPresets.Count
                With oPresets.Item(nPreset)
                    sText(nPreset) = nPreset.ToString
                    For nCol As Integer = 1 To mnNumParms
                        sText(nPreset) = sText(nPreset) & vbTab & .Param(CType(nCol - 1, ePresetParam)).Value
                    Next
                    Select Case meScreen
                        Case eScreenSelect.FluidPresets
                            sText(nPreset) = sText(nPreset) & vbTab & .Description.Text
                        Case eScreenSelect.CCPresets
                            sText(nPreset) = sText(nPreset) & vbTab & .Description.Text
                        Case eScreenSelect.EstatPresets
                            sText(nPreset) = sText(nPreset) & vbTab & .EstatDescription.Text
                    End Select
                    sText(nPreset) = sText(nPreset).Replace("<", "")
                    sText(nPreset) = sText(nPreset).Replace(">", "")
                End With
            Next
            Dim bCancel As Boolean = False
            mPrintHtml.subStartDoc(Status, sPageTitle, bExportCSV, bCancel)
            If bCancel = False Then
                mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle, sPageTitle)
                mPrintHtml.subCloseFile(Status)
                If bPrint Then
                    mPrintHtml.subPrintDoc()
                End If
            End If
            Return (bCancel = False)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)

            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Status = gcsRM.GetString("csPRINTFAILED")
            Return (False)

        End Try

    End Function
    Private Sub mnuExport_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                            Handles mnuExport.Click
        '********************************************************************************************
        'Description:  export the table to a csv file
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/01/10  MSW     allow import from and export to csv
        '********************************************************************************************
        bPrintdoc(True, True)

    End Sub
    Private Sub mnuImport_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                Handles mnuImport.Click
        '********************************************************************************************
        'Description:  Import settings from a csv file
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/01/10  MSW     allow import from and export to csv
        '********************************************************************************************
        Dim sFile As String = String.Empty
        Try

            Dim sTitleReq As String = gpsRM.GetString("psSCREENCAPTION")
            Select Case meScreen
                Case eScreenSelect.FluidPresets
                    sTitleReq = gpsRM.GetString("psFLUID_PRESETS_BUTTON")
                Case eScreenSelect.CCPresets
                    sTitleReq = gpsRM.GetString("psCC_PRESETS")
                Case eScreenSelect.EstatPresets
                    sTitleReq = gpsRM.GetString("psESTAT_PRESETS_BUTTON")
            End Select
            Dim sTableStart(0) As String
            sTableStart(0) = lblItem.Text
            Dim sHeader As String = Nothing
            Dim oDT As DataTable = Nothing
            ImportExport.GetDTFromCSV(sTitleReq, sTableStart, sHeader, oDT)
            If (sHeader IsNot Nothing) AndAlso (oDT IsNot Nothing) AndAlso _
                (oDT.Columns.Count > mnNumParms) Then
                'Select CC or color presets
                Dim oPresets As clsPresets
                If meScreen = eScreenSelect.CCPresets Then
                    oPresets = mRobot.SystemColors(1, False).Presets
                Else
                    oPresets = mRobot.SystemColors(msOldColor).Presets
                End If
                For nRow As Integer = 0 To oDT.Rows.Count - 1

                    If IsNumeric(oDT.Rows(nRow).Item(0)) Then
                        'Validate row
                        Dim nPresetNum As Integer = CType(oDT.Rows(nRow).Item(0), Integer)
                        If nPresetNum > mnNumberofpresets Then
                            Exit For
                        End If
                        Dim bValidRow As Boolean = True

                        For nParm As Integer = 1 To mnNumParms
                            bValidRow = bValidRow And IsNumeric(oDT.Rows(nRow).Item(nParm))
                        Next

                        If bValidRow Then
                            mbSkipMinCheck = True
                            Dim nOffset As Integer = vsbItems.Value
                            If (nPresetNum <= nOffset) Or (nPresetNum > mnITEMS_PER_PAGE + nOffset) Then
                                If nPresetNum > vsbItems.Maximum Then
                                    vsbItems.Value = vsbItems.Maximum
                                Else
                                    vsbItems.Value = nPresetNum - 1
                                End If
                                If vsbItems.Value > (vsbItems.Maximum + 1 - mnITEMS_PER_PAGE) Then
                                    vsbItems.Value = vsbItems.Maximum + 1 - mnITEMS_PER_PAGE
                                End If
                                nOffset = vsbItems.Value
                                Application.DoEvents()
                            End If
                            For nParm As Integer = 1 To mnNumParms
                                Dim sName As String = "txtCol" & nParm.ToString("00") & "Prst" & Strings.Format((nPresetNum - nOffset), "00")
                                Dim oT As FocusedTextBox.FocusedTextBox = DirectCast(pnlMain.Controls(sName), FocusedTextBox.FocusedTextBox)
                                oT.Focus()
                                oT.Text = oDT.Rows(nRow).Item(nParm).ToString
                                Application.DoEvents()
                                cboZone.Focus()
                                Application.DoEvents()
                            Next

                            If oDT.Columns.Count > (mnNumParms + 1) Then
                                Dim sName As String = "txtDesc" & Strings.Format((nPresetNum - nOffset), "00")
                                Dim oT As FocusedTextBox.FocusedTextBox = DirectCast(pnlMain.Controls(sName), FocusedTextBox.FocusedTextBox)
                                oT.Focus()
                                oT.Text = oDT.Rows(nRow).Item(mnNumParms + 1).ToString
                                Application.DoEvents()
                                cboZone.Focus()
                                Application.DoEvents()
                            End If
                            mbSkipMinCheck = False
                        End If
                    End If
                Next
            Else
                'No import found message
                Dim lRet As Response = MessageBox.Show(gpsRM.GetString("psVALID_IMPORT_DATA_NOT_FOUND_TEXT"), gpsRM.GetString("psVALID_IMPORT_DATA_NOT_FOUND_CAP"), _
                           MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If


        Catch ex As Exception

        End Try
    End Sub

    Private Sub subProcessCommandLine(Optional ByRef eScreen As eScreenSelect = eScreenSelect.FluidPresets, _
                                      Optional ByRef sZone As String = "", _
                                      Optional ByRef sRobot As String = "")
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
        Const msCOLORCHANGE_CMDLINE As String = "colorchange"
        Const msESTAT_CMDLINE As String = "estat"
        eScreen = eScreenSelect.FluidPresets
        Dim sArg As String

        For Each s As String In My.Application.CommandLineArgs
            'If a culture string has been passed in, set the current culture (display language)

            sArg = "/culture="
            If s.ToLower.StartsWith(sArg) Then
                Culture = s.Remove(0, sArg.Length)
            End If
            'Color change presets?
            If s.ToLower = msCOLORCHANGE_CMDLINE Then
                eScreen = eScreenSelect.CCPresets
            End If
            'Estat presets
            If s.ToLower = msESTAT_CMDLINE Then
                eScreen = eScreenSelect.EstatPresets
            End If
            'Robot and zone select
            sArg = "/robot="
            If s.ToLower.StartsWith(sArg) Then
                sRobot = s.Remove(0, sArg.Length)
            End If
            sArg = "/zone="
            If s.ToLower.StartsWith(sArg) Then
                sZone = s.Remove(0, sArg.Length)
            End If
        Next

    End Sub
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
        If EditsMade Then
            ' false means user pressed cancel
            If bAskForSave() = False Then
                cboRobot.Text = msOldRobot
                Exit Sub
            End If
        End If  'EditsMade

        'just in case
        If cboRobot.Text = String.Empty Then
            Exit Sub
        Else
            If cboRobot.Text = msOldRobot Then Exit Sub
        End If
        If meScreen = frmMain.eScreenSelect.CCPresets Then
            If msOldRobot <> String.Empty Then _
                        colControllers.Item(msOldRobot).Selected = False
        Else
            If msOldRobot <> String.Empty Then _
                                    colArms.Item(msOldRobot).Selected = False
        End If
        Dim sTmp As String = msOldRobot
        msOldRobot = cboRobot.Text

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            Progress = 5

            subClearScreen()

            For i As Integer = 1 To 7
                msEstatSteps(i) = String.Empty
            Next

            If meScreen = frmMain.eScreenSelect.CCPresets Then
                mRobot = colControllers.Item(cboRobot.Text).Arms(0)
                colControllers.Item(cboRobot.Text).Selected = True
                colArms.Item(mRobot.Name).Selected = True
            Else
                mRobot = colArms.Item(cboRobot.Text)
                colArms.Item(cboRobot.Text).Selected = True
            End If

            meOldApplicator = mRobot.ColorChangeType
            mApplicator = colApplicators(meOldApplicator)

            If mRobot.IsOnLine = False Then
                Status = mRobot.Controller.Name & gcsRM.GetString("csIS_OFFLINE")
                lstStatus.Visible = True
                MessageBox.Show((mRobot.Controller.Name & gcsRM.GetString("csIS_OFFLINE")), _
                   (msSCREEN_NAME & " - " & mRobot.Controller.Name), _
                   MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                msOldRobot = String.Empty
                cboRobot.Text = sTmp
                Exit Sub
            End If
            If meScreen <> frmMain.eScreenSelect.CCPresets And mRobot.Controller.Version >= 7.5 Then
                '[PAVROPTN] byOptn[1].byOp[78]	(Presets by JOB/STYLE, 0 = NO – 1 = YES)
                mRobot.ProgramName = "PAVROPTN"
                mRobot.VariableName = "byOptn[1].byOp[78]"
                mbPresetsByStyle = (CInt(mRobot.VarValue) = 1)
                mRobot.ProgramName = "PAVRSYSC"
                '[PAVRSYSC] qty_prststyl	(Number of jobs/styles)
                mRobot.VariableName = "qty_prststyl"
                mnNumStyles = CInt(mRobot.VarValue)
                If mbPresetsByStyle Then
                    lblStyle.Text = gpsRM.GetString("psSTYLE_CAP")
                    If cboStyle.Items.Count = 0 Then '01/05/10 RJO
                        cboStyle.Items.Clear()
                        cboStyle.Text = String.Empty
                        mnOldStyle = 0 '12/31/09 RJO
                        Dim oStyles As New clsSysStyles(colZones.ActiveZone)
                        oStyles.LoadStyleBoxFromCollection(cboStyle, False, True)
                    End If
                End If
            Else
                mbPresetsByStyle = False
                mnNumStyles = 1
            End If
            subInitFormText()

            If meScreen = eScreenSelect.EstatPresets Then
                If mRobot.EstatType = eEstatType.FB200 Then
                    ' get preset values from estat unit by ftp
                    Try
                        If mPresetCommon.LoadEstatStepValues(mRobot, msEstatSteps) Then
                            btnSteps.Visible = True
                        Else
                            Status = gpsRM.GetString("psCOULD_NOT_LOAD_ESTAT")
                            btnSteps.Visible = False
                        End If
                    Catch ex As Exception
                        ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                            Status, MessageBoxButtons.OK)
                    End Try

                Else
                    btnSteps.Visible = False
                End If  'mRobot.EstatType = eEstatType.FB200 
            End If

            If meScreen = frmMain.eScreenSelect.CCPresets Then
                btnSteps.Visible = False
                Try
                    Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

                    subClearScreen()
                    subLoadData()

                Catch ex As Exception
                    Trace.WriteLine(ex.Message)
                    Trace.WriteLine(ex.StackTrace)
                    ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                        Status, MessageBoxButtons.OK)

                Finally
                    Me.Cursor = System.Windows.Forms.Cursors.Default
                End Try

            Else

                mSysColorCommon.LoadColorBoxFromCollection(mRobot.SystemColors, cboColor)

                'HGB 04/18/11 - Fix null referance
                'In the case of one color, mSysColorCommon.LoadColorBoxFromCollection will select the color and cause msOldColor to be set to the proper value.
                'The fixes the NullReferanceException in subTextboxValidatingHandler when only one color is in use.

                If cboColor.Items.Count > 1 Then
                    msOldColor = String.Empty
                End If

                If cboColor.Items.Contains(msColorCache) Then
                    cboColor.Text = msColorCache
                    'msColorCache = String.Empty
                Else
                    Status(True) = gcsRM.GetString("csSELECT_COLOR")
                End If


            End If
            ' copy button
            subEnableControls(True)

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
        ' 06/01/09  MSW     CC preset support
        '********************************************************************************************

        Application.DoEvents()

        Select Case Privilege
            Case ePrivilege.None
                ' shouldnt be here
                subEnableControls(False)
                Exit Sub

            Case Else
                'ok
        End Select


        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        mScreenSetup.MoveCursorOffButton(DirectCast(btnSave, ToolStripItem))
        mScreenSetup.MoveCursorOffButton(DirectCast(btnSaveNoMenu, ToolStripItem))
        Dim bStatus As Boolean = lstStatus.Visible
        lstStatus.Visible = True
        Try

            'make sure its good data
            If Not bValidateData() Then Exit Sub

            ' do save
            Dim dsDescriptions As DataSet = New DataSet

            'Select CC or color presets
            Dim oPresets As clsPresets
            If meScreen = frmMain.eScreenSelect.CCPresets Then
                oPresets = mRobot.SystemColors(1, False).Presets
            Else
                oPresets = mRobot.SystemColors(msOldColor).Presets
            End If

            If bSaveToGUI(mRobot, oPresets, dsDescriptions) Then
                If bSaveToPLC() Then
                    If bSaveToRobot(mRobot, oPresets) Then
                        Status = gcsRM.GetString("csUPD_CHANGELOG")
                        mPWCommon.SaveToChangeLog(colZones.ActiveZone)
                        'For SQL database - remove above eventually
                        mPWCommon.SaveToChangeLog(colZones.ActiveZone)
                        ' save done
                        EditsMade = False
                        subShowNewPage()
                        Status = gcsRM.GetString("csSAVE_DONE")

                    Else
                        'save failed
                        Status = gcsRM.GetString("csSAVEFAILED")
                    End If  'SaveToRobot()
                Else
                    'save failed
                    Status = gcsRM.GetString("csSAVEFAILEDPLC")

                End If    ' bSaveToPLC()
            Else
                'save failed
                Status = gcsRM.GetString("csSAVEFAILED")

            End If      'bSaveToGUI()


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally

            Me.Cursor = System.Windows.Forms.Cursors.Default
            lstStatus.Visible = bStatus
        End Try


    End Sub
    Private Sub subSaveByValve(ByVal ValveNumber As Integer)
        '********************************************************************************************
        'Description:  Data Save Routine save to all colors with same valve #
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/01/09  MSW     CC preset support
        '********************************************************************************************

        Select Case Privilege
            Case ePrivilege.None
                ' shouldnt be here
                subEnableControls(False)
                Exit Sub

            Case Else
                'ok
        End Select


        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

        Try

            'make sure its good data
            If Not bValidateData() Then Exit Sub

            ' save it because copy reloads whats there
            subSaveData()

            ' do save - pretend its a copy
            Dim oSC As clsSysColor
            Dim TargetParam As New Collection(Of String)
            ' load collection with colors that match valve
            For Each oSC In mRobot.SystemColors
                If oSC.Valve.Number.Value = ValveNumber Then
                    If oSC.DisplayName <> cboColor.Text Then
                        TargetParam.Add(oSC.DisplayName)
                    End If
                End If ' oSC.Valve.Number = ValveNumber
            Next 'oSC

            Dim SourceParam As New Collection(Of String)
            SourceParam.Add(cboColor.Text)
            Dim SourceSubParam As New Collection(Of String)
            Dim TargetSubParam As New Collection(Of String)

            Dim o As clsPreset

            'Select CC or color presets
            Dim oPresets As clsPresets
            If meScreen = frmMain.eScreenSelect.CCPresets Then
                oPresets = mRobot.SystemColors(1, False).Presets
            Else
                oPresets = mRobot.SystemColors(msOldColor).Presets
            End If

            For Each o In oPresets
                SourceSubParam.Add(o.DisplayName)
                TargetSubParam.Add(o.DisplayName)
            Next

            Dim void As New frmCopy

            If mPresetCommon.DoCopy(colZones, colZones, colArms, colArms, _
                               SourceParam, TargetParam, SourceSubParam, TargetSubParam, _
                               eCopyType.CopyAll, void) Then

                mPWCommon.SaveToChangeLog(colZones.ActiveZone)
            End If

            ' reload the data selected on the screen
            subLoadData()

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally

            Me.Cursor = System.Windows.Forms.Cursors.Default

        End Try


    End Sub
    Private Sub subSetTextBoxProperties(Optional ByRef oFTB As FocusedTextBox.FocusedTextBox = Nothing)
        '********************************************************************************************
        'Description:  
        '
        'Parameters:
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/18/09  MSW     subTextboxKeypressHandler, subSetTextBoxProperties - Offer a login if they try to enter data without a login
        '********************************************************************************************
        If oFTB Is Nothing Then
            Dim o As FocusedTextBox.FocusedTextBox

            For Each o In colTextboxes
                Dim sTmp As String = Strings.Left(o.Name, 6)
                Select Case sTmp
                    Case "txtCol"
                        o.NumericOnly = True
                    Case "txtDes"
                        o.NumericOnly = False
                End Select
                AddHandler o.UpArrow, AddressOf subTextBoxUpArrowHandler
                AddHandler o.DownArrow, AddressOf subTextBoxDownArrowHandler
                AddHandler o.LeftArrow, AddressOf subTextBoxLeftArrowHandler
                AddHandler o.RightArrow, AddressOf subTextBoxRightArrowHandler
                AddHandler o.Validating, AddressOf subTextboxValidatingHandler
                AddHandler o.Validated, AddressOf subTextboxValidatedHandler
                AddHandler o.TextChanged, AddressOf subTextboxChangeHandler
                AddHandler o.KeyPress, AddressOf subTextboxKeypressHandler
                AddHandler o.KeyUp, AddressOf subTextboxKeyUpHandler
            Next
        Else
            Dim sTmp As String = Strings.Left(oFTB.Name, 6)
            Select Case sTmp
                Case "txtCol"
                    oFTB.NumericOnly = True
                Case "txtDes"
                    oFTB.NumericOnly = False
            End Select
            AddHandler oFTB.UpArrow, AddressOf subTextBoxUpArrowHandler
            AddHandler oFTB.DownArrow, AddressOf subTextBoxDownArrowHandler
            AddHandler oFTB.LeftArrow, AddressOf subTextBoxLeftArrowHandler
            AddHandler oFTB.RightArrow, AddressOf subTextBoxRightArrowHandler
            AddHandler oFTB.Validating, AddressOf subTextboxValidatingHandler
            AddHandler oFTB.Validated, AddressOf subTextboxValidatedHandler
            AddHandler oFTB.TextChanged, AddressOf subTextboxChangeHandler
            AddHandler oFTB.KeyPress, AddressOf subTextboxKeypressHandler
            AddHandler oFTB.KeyUp, AddressOf subTextboxKeyUpHandler
        End If
    End Sub
    Private Sub subShowChangeLog(ByVal nIndex As Integer)
        '********************************************************************************************
        'Description:  show the change log form
        '
        'Parameters: how many changes to show
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/16/13  MSW     Standalone ChangeLog                                         4.01.05.00
        '********************************************************************************************

        Try
            mPWCommon.subShowChangeLog(colZones.CurrentZone, gsChangeLogArea, msSCREEN_NAME, cboRobot.Text, _
              cboColor.Text, mDeclares.eParamType.Colors, nIndex, mbUSEROBOTS, (meScreen <> frmMain.eScreenSelect.CCPresets), oIPC)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        End Try

    End Sub
    Private Sub subShowMultiView()
        '********************************************************************************************
        'Description:  show the multi view form
        '
        'Parameters: how many changes to show
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason        
        ' 06/01/09  MSW     CC preset support
        '********************************************************************************************
        Dim frmAll As New frmAll

        Try
            With frmAll
                .DatabasePath = colZones.DatabasePath
                .InitialZone = cboZone.Text
                .ScreenName = msSCREEN_NAME
                .ShowDeviceCombo = mbUSEROBOTS
                .ShowParamCombo = (meScreen <> frmMain.eScreenSelect.CCPresets)
                .ShowSubParamCombo = mbPresetsByStyle
                .ResetSubParamOnNewParam = False
                .SubParamName = gpsRM.GetString("psSTYLE_CAP")
                .ByArm = (meScreen <> frmMain.eScreenSelect.CCPresets)
                .IncludeOpeners = False
                .Show()

            End With

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                 Status, MessageBoxButtons.OK)
            frmAll = Nothing
        End Try

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
        ' 06/01/09  MSW     CC preset support
        '********************************************************************************************
        Dim nOffset As Integer = vsbItems.Value 'preset number that goes in top row
        Dim nBox As Integer = 1
        Dim oT As TextBox = Nothing
        Dim sName As String = String.Empty
        Dim nPtr As Integer = 0

        mbEventBlocker = True

        'Select CC or color presets
        Dim oPresets As clsPresets
        If meScreen = frmMain.eScreenSelect.CCPresets Then
            oPresets = mRobot.SystemColors(1, False).Presets
        Else
            oPresets = mRobot.SystemColors(msOldColor).Presets 'use msOldcolor in case we're saveing after a new selection
        End If

        With oPresets
            For nBox = 1 To mnITEMS_PER_PAGE
                ' collection pointer
                nPtr = nOffset + nBox
                sName = "lblItem" & Strings.Format(nBox, "00")
                pnlMain.Controls(sName).Text = nPtr.ToString
                'row of textboxes loop
                For nCol As Integer = 1 To .NumParms
                    Dim eParam As ePresetParam = CType(nCol - 1, ePresetParam)

                    sName = "txtCol" & nCol.ToString("00") & "Prst" & Strings.Format(nBox, "00")
                    oT = DirectCast(pnlMain.Controls(sName), FocusedTextBox.FocusedTextBox) 'mScreenSetup.GetTextboxByName(colTextboxes, sName)

                    'fix for bad index then presets < items on a page
                    If nPtr > mnNumberofpresets Then
                        oT.Visible = False
                    Else
                        oT.Text = .Item(nPtr).Param(eParam).Value.ToString
                        oT.Visible = True
                        If .Item(nPtr).Param(eParam).Changed Then
                            oT.ForeColor = Color.Red
                        Else
                            oT.ForeColor = Color.Black
                        End If
                        'effective
                        sName = "lblCol" & nCol.ToString("00") & "Prst" & Strings.Format(nBox, "00")
                        pnlMain.Controls(sName).Text = _
                                        .Item(nPtr).EffectiveValue(eParam).ToString
                    End If

                Next

                sName = "txtDesc" & Strings.Format(nBox, "00")
                'oT = DirectCast(colTextboxes(sName), TextBox)
                oT = mScreenSetup.GetTextboxByName(colTextboxes, sName)


                If nPtr > mnNumberofpresets Then
                    oT.Visible = False
                Else
                    '6/20/08
                    If meScreen <> eScreenSelect.EstatPresets Then
                        oT.Text = .Item(nPtr).Description.Text
                        If .Item(nPtr).Description.Changed Then
                            oT.ForeColor = Color.Red
                        Else
                            oT.ForeColor = Color.Black
                        End If
                    Else
                        oT.Text = .Item(nPtr).EstatDescription.Text
                        If .Item(nPtr).EstatDescription.Changed Then
                            oT.ForeColor = Color.Red
                        Else
                            oT.ForeColor = Color.Black
                        End If
                    End If
                    oT.Visible = True
                End If
            Next

            Dim bTmp As Boolean
            Dim bOrideEnable As Boolean = True
            If meScreen = eScreenSelect.EstatPresets Then
                ' estat display based on type
                Select Case mRobot.EstatType
                    Case eEstatType.None

                        bTmp = False
                        lblOrid01.Text = String.Empty

                    Case eEstatType.FB200, eEstatType.Honda

                        bTmp = .EstatStepValuesLoaded
                        lblCol01PrstCap.Text = gpsRM.GetString("psESTATSTEPCAP")
                        lblCol01Prst00.Text = gpsRM.GetString("psEFFECTIVE_KV")
                        bOrideEnable = False
                    Case Else
                        bTmp = .OverRideEnabled.Value And (.OverRidePercent(0).Value <> 100)
                        lblOrid01.Text = .ParameterName(0) & " - " & _
                                 .OverRidePercent(ePresetParam.Estat).Value.ToString & "%"

                        lblCol01Prst00.Text = gpsRM.GetString("psEFFECTIVE")

                End Select

                subEffectiveLabelsVisible(1, bTmp)

            Else
                'display / hide override labels
                bOrideEnable = (meScreen <> eScreenSelect.CCPresets)
                For nCol As Integer = 1 To mnNumParms
                    Dim eParam As ePresetParam = CType(nCol - 1, ePresetParam)

                    If bOrideEnable Then
                        bTmp = .OverRideEnabled.Value And (.OverRidePercent(eParam).Value <> 100)
                    Else
                        bTmp = False
                    End If
                    subEffectiveLabelsVisible(nCol, bTmp)
                Next
            End If


            If .OverRideEnabled.Value Then
                lblOverride.Text = gpsRM.GetString("psOVERRIDE_ENABLED")
                For nCol As Integer = 1 To mnNumParms
                    Dim eParam As ePresetParam = CType(nCol - 1, ePresetParam)

                    tscMain.ContentPanel.Controls("lblOrid" & nCol.ToString("00")).Text = _
                                     .ParameterName(eParam) & " - " & _
                                            .OverRidePercent(eParam).Value.ToString & "%"
                    tscMain.ContentPanel.Controls("lblOrid" & nCol.ToString("00")).Visible = bOrideEnable
                Next
            Else
                lblOverride.Text = gpsRM.GetString("psOVERRIDE_DISABLED")
                For nCol As Integer = 1 To mnNumParms
                    tscMain.ContentPanel.Controls("lblOrid" & nCol.ToString("00")).Text = String.Empty
                    tscMain.ContentPanel.Controls("lblOrid" & nCol.ToString("00")).Visible = False
                Next
            End If
            If mnNumParms < mnMaxParms Then
                For nCol As Integer = mnNumParms + 1 To mnMaxParms
                    subEffectiveLabelsVisible(nCol, False)
                    tscMain.ContentPanel.Controls("lblOrid" & nCol.ToString("00")).Text = String.Empty
                    tscMain.ContentPanel.Controls("lblOrid" & nCol.ToString("00")).Visible = False
                Next

            End If

        End With

        mbEventBlocker = False

    End Sub
    Private Sub subStepButtonPressed()
        '********************************************************************************************
        'Description:  Show the steps form
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        frmStp = New frmSteps
        frmStp.Show(mRobot, msEstatSteps)

    End Sub
    Private Sub subTextBoxUpArrowHandler(ByRef sender As FocusedTextBox.FocusedTextBox, _
                        ByVal bAlt As Boolean, ByVal bShift As Boolean, ByVal bControl As Boolean)
        '********************************************************************************************
        'Description:  someone hit the up arrow key empty text should be checked for by sender
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/07/10  MSW     Validate the current box before shifting it around
        '********************************************************************************************
        Dim sNumber As String = Strings.Right(sender.Name, 2)
        Dim sName As String = Strings.Left(sender.Name, (Len(sender.Name) - 2))
        Dim sNewName As String = String.Empty
        Dim sNewNum As String = String.Empty
        Dim nTmp As Integer = 0
        Dim o As FocusedTextBox.FocusedTextBox
        Dim sText As String = sender.Text

        nTmp = CInt(sNumber) - 1
        If nTmp = 0 Then
            If vsbItems.Visible Then
                'validate the current box before shifting it around
                cboZone.Focus()
                Application.DoEvents()
                If vsbItems.Value = (vsbItems.Minimum) Then
                    ' at top do wrap
                    vsbItems.Value = vsbItems.Maximum + 1 - vsbItems.LargeChange
                    nTmp = mnITEMS_PER_PAGE
                Else
                    vsbItems.Value -= 1
                    nTmp = 1
                End If ' vsbItems.Value = vsbItems.Minimum
            Else
                nTmp = mnITEMS_PER_PAGE
            End If ' vsbItems.Visible
        End If ' nTmp = 0

        sNewNum = Strings.Format(nTmp, "00")

        sNewName = sName & sNewNum
        'I don't know how this one is supposed to work, but it's not working on the added controls
        'o = DirectCast(GetControlByName(sNewName, Me), FocusedTextBox.FocusedTextBox)
        'This how the help files told me to do it.
        o = DirectCast(pnlMain.Controls(sNewName), FocusedTextBox.FocusedTextBox)
        o.Focus()

        If bControl Then
            If Privilege = ePrivilege.Edit Then
                o.Text = sText
                ' need to fire validation to save here 
            End If
        End If


    End Sub
    Private Sub subTextBoxDownArrowHandler(ByRef sender As FocusedTextBox.FocusedTextBox, _
                    ByVal bAlt As Boolean, ByVal bShift As Boolean, ByVal bControl As Boolean)
        '********************************************************************************************
        'Description:  someone hit the down arrow key empty text should be checked for by sender
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 05/07/10  MSW     Validate the current box before shifting it around
        '********************************************************************************************
        Dim sNumber As String = Strings.Right(sender.Name, 2)
        Dim sName As String = Strings.Left(sender.Name, (Len(sender.Name) - 2))
        Dim sNewName As String = String.Empty
        Dim sNewNum As String = String.Empty
        Dim nTmp As Integer = 0
        Dim o As FocusedTextBox.FocusedTextBox
        Dim sText As String = sender.Text

        nTmp = CInt(sNumber) + 1
        If nTmp = 1 + mnITEMS_PER_PAGE Then
            'validate the current box before shifting it around
            cboZone.Focus()
            Application.DoEvents()
            If vsbItems.Visible Then
                If vsbItems.Value = (vsbItems.Maximum + 1 - vsbItems.LargeChange) Then
                    ' at bottom do wrap
                    vsbItems.Value = vsbItems.Minimum
                    nTmp = 1
                Else
                    vsbItems.Value += 1
                    nTmp = mnITEMS_PER_PAGE
                End If ' vsbItems.Value = vsbItems.Maximum
            Else
                nTmp = 1
            End If ' vsbItems.Visible
        End If ' nTmp = 11

        sNewNum = Strings.Format(nTmp, "00")

        sNewName = sName & sNewNum
        'I don't know how this one is supposed to work, but it's not working on the added controls
        'o = DirectCast(GetControlByName(sNewName, Me), FocusedTextBox.FocusedTextBox)
        'This how the help files told me to do it.
        o = DirectCast(pnlMain.Controls(sNewName), FocusedTextBox.FocusedTextBox)
        o.Focus()

        If bControl Then
            If Privilege = ePrivilege.Edit Then
                o.Text = sText
                ' need to fire validation to save here 
            End If
        End If

    End Sub
    Private Sub subTextBoxRightArrowHandler(ByRef sender As FocusedTextBox.FocusedTextBox, _
                        ByVal bAlt As Boolean, ByVal bShift As Boolean, ByVal bControl As Boolean)
        '********************************************************************************************
        'Description:  someone hit the right arrow key empty text should be checked for by sender
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sNumber As String = Strings.Right(sender.Name, 2)
        Dim sName As String = Strings.Left(sender.Name, (Len(sender.Name) - 2))
        Dim sNewName As String = String.Empty
        Dim o As FocusedTextBox.FocusedTextBox

        Select Case sName.Substring(0, 6)
            Case "txtCol"
                Dim nCol As Integer = CInt(sName.Substring(6, 2))
                If nCol = mnNumParms Then
                    sNewName = "txtDesc" & sNumber
                Else
                    sNewName = "txtCol" & (nCol + 1).ToString("00") & "Prst" & sNumber
                End If
            Case "txtDes"
                sNewName = "txtCol01Prst" & sNumber
        End Select

        o = DirectCast(pnlMain.Controls(sNewName), FocusedTextBox.FocusedTextBox)
        o.Focus()


    End Sub
    Private Sub subTextBoxLeftArrowHandler(ByRef sender As FocusedTextBox.FocusedTextBox, _
                        ByVal bAlt As Boolean, ByVal bShift As Boolean, ByVal bControl As Boolean)
        '********************************************************************************************
        'Description:  someone hit the left arrow key empty text should be checked for by sender
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sNumber As String = Strings.Right(sender.Name, 2)
        Dim sName As String = Strings.Left(sender.Name, (Len(sender.Name) - 2))
        Dim sNewName As String = String.Empty
        Dim o As FocusedTextBox.FocusedTextBox

        Select Case sName.Substring(0, 6)
            Case "txtCol"
                Dim nCol As Integer = CInt(sName.Substring(6, 2))
                If nCol = 1 Then
                    sNewName = "txtDesc" & sNumber
                Else
                    sNewName = "txtCol" & (nCol - 1).ToString("00") & "Prst" & sNumber
                End If
            Case "txtDes"
                sNewName = "txtCol" & (mnNumParms).ToString("00") & "Prst" & sNumber
        End Select

        o = DirectCast(pnlMain.Controls(sNewName), FocusedTextBox.FocusedTextBox)
        o.Focus()


    End Sub
    Private Sub subUndoData()
        '********************************************************************************************
        'Description:  Undo Button Pressed
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If MessageBox.Show(gcsRM.GetString("csUNDOMSG"), msSCREEN_NAME, MessageBoxButtons.OKCancel, _
                                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) _
                                                                    = Response.OK Then

            subLoadData()

        End If

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
    Private Sub subSelectCCPresets()
        '********************************************************************************************
        'Description: switch between color presets and CC presets
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If meScreen = eScreenSelect.CCPresets Then Exit Sub
        'make sure edits are saved
        If EditsMade Then
            If Not (bAskForSave()) Then
                Exit Sub
            End If
        End If

        'switch screens
        meOldScreen = meScreen
        meScreen = eScreenSelect.CCPresets
        btnCCPresets.Checked = True
        btnFluidPresets.Checked = False
        btnEstatPresets.Checked = False
        msCmdLineZone = cboZone.Text
        msCmdLineRobot = cboRobot.Text
        subInitializeForm()
    End Sub
    Private Sub subSelectFluidPresets()
        '********************************************************************************************
        'Description: switch between color presets and CC presets
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If meScreen = eScreenSelect.FluidPresets Then Exit Sub
        'make sure edits are saved
        If EditsMade Then
            If Not (bAskForSave()) Then
                Exit Sub
            End If
        End If

        'switch screens
        meOldScreen = meScreen
        meScreen = eScreenSelect.FluidPresets
        btnCCPresets.Checked = False
        btnFluidPresets.Checked = True
        btnEstatPresets.Checked = False
        msCmdLineZone = cboZone.Text
        msCmdLineRobot = cboRobot.Text
        subInitializeForm()
    End Sub
    Private Sub subSelectEstatPresets()
        '********************************************************************************************
        'Description: switch between color presets and CC presets
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If meScreen = eScreenSelect.EstatPresets Then Exit Sub
        'make sure edits are saved
        If EditsMade Then
            If Not (bAskForSave()) Then
                Exit Sub
            End If

        End If

        'switch screens
        meOldScreen = meScreen
        meScreen = eScreenSelect.EstatPresets
        btnCCPresets.Checked = False
        btnFluidPresets.Checked = False
        btnEstatPresets.Checked = True
        msCmdLineZone = cboZone.Text
        msCmdLineRobot = cboRobot.Text
        subInitializeForm()
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
        Select Case e.ClickedItem.Name ' case sensitive
            Case "btnClose"
                'Check to see if we need to save is performed in  bAskForSave
                Me.Close()
                'End
            Case "btnStatus"
                lstStatus.Visible = Not lstStatus.Visible

            Case "btnSave"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                'give validate a chance to fire
                Application.DoEvents()
                'privilege check done in subroutine
                subSaveData()
            Case "btnSaveNoMenu"
                'give validate a chance to fire
                Application.DoEvents()
                'privilege check done in subroutine
                subSaveData()
            Case "btnPrint"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                If DataLoaded Then
                    bPrintdoc(True)
                End If

            Case "btnUndo"
                Select Case Privilege
                    Case ePrivilege.None
                        'logout occured while screen open
                        subEnableControls(False)
                    Case Else
                        subUndoData()
                End Select

            Case "btnChangeLog"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                subShowChangeLog(CType(eChangeMnu.Hours24, Integer))

            Case "btnCopy"
                Select Case Privilege
                    Case ePrivilege.None
                        'logout occured while screen open
                        subEnableControls(False)
                    Case Else
                        subCopyButtonPressed()
                End Select
            Case "btnMultiView"
                subShowMultiView()

            Case "btnOverride"
                subOverrideButtonPressed()

            Case "btnSteps"
                subStepButtonPressed()
            Case "btnCCPresets"
                subSelectCCPresets()
            Case "btnFluidPresets"
                subSelectFluidPresets()
            Case "btnEstatPresets"
                subSelectEstatPresets()
            Case btnUtilities.Name
                btnUtilities.ShowDropDown()
        End Select

        tlsMain.Refresh()

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

        If EditsMade Then
            e.Cancel = (bAskForSave() = False)
        End If

        Try
            If colArms Is Nothing Then Exit Sub
            If colArms.Count = 0 Then Exit Sub
            For i As Integer = colArms.Count - 1 To 0
                colArms.Remove(colArms(i))
            Next

        Catch ex As Exception

        End Try

    End Sub
    Public Sub New()
        'for language support
        mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, msBASE_ASSEMBLY_LOCAL, _
                                            msROBOT_ASSEMBLY_LOCAL)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Private Sub colControllers_BumpProgress() Handles colControllers.BumpProgress
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If Progress > 0 Then Progress += 5

    End Sub
    Private Sub colControllers_ConnectionStatusChange(ByVal Controller As clsController) _
                                        Handles colControllers.ConnectionStatusChange
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/01/09  MSW     handle cross-thread calls
        '********************************************************************************************
        'this is needed with old RCM object raising events
        'and can hopefully be removed when I learn how to program
       'Control.CheckForIllegalCrossThreadCalls = False

        'Check for call from the robot object thread
        If Me.stsStatus.InvokeRequired Then
            Dim dCntrStat As New ControllerStatusChange(AddressOf colControllers_ConnectionStatusChange)
            Me.BeginInvoke(dCntrStat, New Object() {Controller})
        Else
            subDoStatusBar(Controller)
        End If

        'Trace.WriteLine("frmmain connection status change event - " & Controller.Name & " " & _
        '                        Controller.RCMConnectStatus.ToString

       'Control.CheckForIllegalCrossThreadCalls = True

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
        If cboZone.Text <> msOldZone Then
            ' so we don't go off half painted
            Me.Refresh()
            System.Windows.Forms.Application.DoEvents()
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
            ' so we don't go off half painted
            Me.Refresh()
            System.Windows.Forms.Application.DoEvents()
            subChangeRobot()
        End If

    End Sub
    Private Sub cboParam_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                Handles cboColor.SelectedIndexChanged
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
        If cboColor.Text <> msOldColor Then
            ' so we don't go off half painted
            Me.Refresh()
            System.Windows.Forms.Application.DoEvents()
            subChangeColor()
        End If

    End Sub
    Private Sub cboStyle_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboStyle.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Style Combo Changed 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subChangeStyle()
    End Sub
    Private Sub MakeFTB(ByRef rControl As FocusedTextBox.FocusedTextBox, _
                        ByVal sName As String, ByVal rLoc As Point, _
                        ByVal rModel As FocusedTextBox.FocusedTextBox)
        '********************************************************************************************
        'Description:  Make a new FTB and add it to pnlMain
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        rControl = (DirectCast(pnlMain.Controls(sName), FocusedTextBox.FocusedTextBox))
        If rControl Is Nothing Then
            rControl = New FocusedTextBox.FocusedTextBox
            rControl.Name = sName
            rControl.Text = String.Empty
            rControl.Font = rModel.Font
            rControl.BackColor = rModel.BackColor
            rControl.BorderStyle = rModel.BorderStyle
            rControl.CreateControl()
            rControl.Show()
            rControl.Parent = Me.pnlMain
            Me.pnlMain.Controls.Add(rControl)
            colTextboxes.Add(rControl)
            subSetTextBoxProperties(rControl)
        End If
        rControl.TextAlign = rModel.TextAlign
        rControl.Location = rLoc
        rControl.Size = rModel.Size
        rControl.Visible = rModel.Visible
    End Sub
    Private Sub MakeLabel(ByRef rControl As Label, ByVal sName As String, _
                          ByVal rLoc As Point, ByVal rModel As Label)
        '********************************************************************************************
        'Description:  Make a new Label and add it to pnlMain
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        rControl = (DirectCast(pnlMain.Controls(sName), Label))
        If rControl Is Nothing Then
            rControl = New Label
            rControl.Name = sName
            rControl.Text = String.Empty
            rControl.Location = rLoc
            rControl.BackColor = rModel.BackColor
            rControl.BorderStyle = rModel.BorderStyle
            rControl.CreateControl()
            rControl.Show()
            rControl.Parent = Me.pnlMain
            Me.pnlMain.Controls.Add(rControl)
        End If
        rControl.TextAlign = rModel.TextAlign
        rControl.Size = rModel.Size
        rControl.Font = rModel.Font
        rControl.Visible = rModel.Visible
    End Sub
    Private Sub ShowNewRow(ByVal nRow As Integer)
        '********************************************************************************************
        'Description:  The form got bigger, show another row.  Add the controls if they don't exist
        '   <System.Diagnostics.DebuggerStepThrough()>
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sLbl As String = (nRow).ToString("00")
        Dim lblTmp As Label = Nothing
        Dim txtTmp As FocusedTextBox.FocusedTextBox = Nothing
        Dim bVisibleOrides As Boolean = lblCol01Prst01.Visible
        Dim ptLbl As Point
        Dim ptTxt As Point
        Try
            Dim nColDiff As Integer = lblCol02Prst01.Left - lblCol01Prst01.Left
            Dim nRowDiff As Integer = lblCol01Prst02.Top - lblCol01Prst01.Top
            If nRow > 1 Then
                'Preset number label
                lblTmp = DirectCast(pnlMain.Controls("lblItem" & (nRow - 1).ToString("00")), Label)
                ptLbl.Y = mnRowHeight + lblTmp.Top
                ptLbl.X = lblTmp.Left
                MakeLabel(lblTmp, ("lblItem" & sLbl), ptLbl, lblItem01)
                lblTmp.Text = sLbl
                'Effective 
                lblTmp = DirectCast(pnlMain.Controls("lblCol01Prst" & (nRow - 1).ToString("00")), Label)
                ptLbl.Y = mnRowHeight + lblTmp.Top
                'Textbox
                txtTmp = DirectCast(pnlMain.Controls("txtCol01Prst" & (nRow - 1).ToString("00")), FocusedTextBox.FocusedTextBox)
                ptTxt.Y = mnRowHeight + txtTmp.Top

            Else
                lblTmp = DirectCast(pnlMain.Controls("lblCol01Prst01"), Label)
                ptLbl.Y = lblTmp.Top
                txtTmp = DirectCast(pnlMain.Controls("txtCol01Prst01"), FocusedTextBox.FocusedTextBox)
                ptTxt.Y = txtTmp.Top
                txtTmp.Visible = True
                txtDesc01.Visible = True
            End If
            ptTxt.Y = ptLbl.Y
            ptLbl.X = lblTmp.Left
            ptTxt.X = txtTmp.Left
            For nCol As Integer = 1 To mnMaxParms
                MakeLabel(lblTmp, ("lblCol" & nCol.ToString("00") & "Prst" & sLbl), ptLbl, lblCol01Prst01)
                MakeFTB(txtTmp, ("txtCol" & nCol.ToString("00") & "Prst" & sLbl), ptTxt, txtCol01Prst01)
                '10/13/10 BTK Set tabindex. If we don't set it and mnITEMS_PER_PAGE is more then
                '10 then the tab key doesn't work right.
                txtTmp.TabIndex = (nRow * 5) + nCol

                txtTmp.Top = lblTmp.Top

                ptLbl.X = ptLbl.X + nColDiff
                ptTxt.X = ptTxt.X + nColDiff
            Next
            ptTxt.X = lblDescCap.Left
            MakeFTB(txtTmp, ("txtDesc" & sLbl), ptTxt, txtDesc01)
            '10/13/10 BTK Set tabindex. If we don't set it and mnITEMS_PER_PAGE is more then
            '10 then the tab key doesn't work right.
            txtTmp.TabIndex = (nRow * 5) + 5

        Catch ex As Exception

        End Try
    End Sub
    Private Sub frmMain_Layout(Optional ByVal sender As Object = Nothing, _
                            Optional ByVal e As System.Windows.Forms.LayoutEventArgs = Nothing) Handles MyBase.Layout
        '********************************************************************************************
        'Description:  Form needs a redraw due to resize
        '   <System.Diagnostics.DebuggerStepThrough()>
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
        Const nMINSTATUS_LEFT As Integer = 500
        Const nMAXSTATUS_LEFT As Integer = 1000
        Dim nStatusLeft As Integer = nWidth - lstStatus.Width
        If nStatusLeft < nMINSTATUS_LEFT Then
            nStatusLeft = nMINSTATUS_LEFT
        End If
        If nStatusLeft > nMAXSTATUS_LEFT Then
            nStatusLeft = nMAXSTATUS_LEFT
        End If
        lstStatus.Left = nStatusLeft
        vsbItems.Height = nHeight - vsbItems.Top - 5
        mnITEMS_PER_PAGE = (vsbItems.Height - mnLblHeight) \ mnRowHeight

        subFormatScreenLayout()
        If mnOldItemsPerPage < mnITEMS_PER_PAGE Then
            'Show more presets
            For nRow As Integer = mnOldItemsPerPage + 1 To mnITEMS_PER_PAGE
                ShowNewRow(nRow)
            Next
            If mbDataLoaded Then
                subShowNewPage()
            End If
        ElseIf mnOldItemsPerPage > mnITEMS_PER_PAGE Then
            'hide a row
            'validate first
            vsbItems.Focus()
            Dim sLbl As String = (mnITEMS_PER_PAGE + 1).ToString("00")
            Dim lblTmp As Label = DirectCast(pnlMain.Controls("lblItem" & sLbl), Label)
            If Not (lblTmp Is Nothing) Then
                lblTmp.Visible = False
                Dim txtTmp As FocusedTextBox.FocusedTextBox = DirectCast(pnlMain.Controls("txtDesc" & sLbl), FocusedTextBox.FocusedTextBox)
                txtTmp.Visible = False
                For nCol As Integer = 1 To mnMaxParms
                    txtTmp = DirectCast(pnlMain.Controls("txtCol" & nCol.ToString("00") & "Prst" & sLbl), FocusedTextBox.FocusedTextBox)
                    txtTmp.Visible = False
                    lblTmp = DirectCast(pnlMain.Controls("lblCol" & nCol.ToString("00") & "Prst" & sLbl), Label)
                    lblTmp.Visible = False
                Next
            End If
        End If
        mnOldItemsPerPage = mnITEMS_PER_PAGE
        stsStatus.Refresh()
    End Sub
    Private Sub vsbItems_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles vsbItems.ValueChanged
        '********************************************************************************************
        'Description:  scroll the data
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbEventBlocker = True

        If DataLoaded Then subShowNewPage()

        mbEventBlocker = False
    End Sub
    Private Sub colRobots_BumpProgress() Handles colArms.BumpProgress
        '********************************************************************************************
        'Description:  bump me!
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If Progress > 0 Then Progress += 5

    End Sub
    Private Sub mnuLogIn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuLogin.Click
        '********************************************************************************************
        'Description:  someone clicked on lock panel
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If LoggedOnUser = String.Empty Then
            If Privilege = ePrivilege.None Then
                Privilege = ePrivilege.Edit
            End If
        End If

    End Sub
    Private Sub mnuLogOut_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuLogOut.Click
        '********************************************************************************************
        'Description:  someone clicked on lock panel
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If LoggedOnUser <> String.Empty Then
            moPassword.LogUserOut()
        End If

    End Sub
    Private Sub mnuLast7_Click(ByVal sender As Object, _
                                                    ByVal e As System.EventArgs) Handles mnuLast7.Click
        '********************************************************************************************
        'Description:  show changes for last 7 days
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        subShowChangeLog(o.Owner.Items.IndexOf(o))

    End Sub
    Private Sub mnuAllChanges_Click(ByVal sender As Object, _
                                            ByVal e As System.EventArgs) Handles mnuAllChanges.Click
        '********************************************************************************************
        'Description:  show changes for last 7 days
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        subShowChangeLog(o.Owner.Items.IndexOf(o))

    End Sub
    Private Sub mnuLast24_Click(ByVal sender As Object, _
                                            ByVal e As System.EventArgs) Handles mnuLast24.Click
        '********************************************************************************************
        'Description:  show changes for last 7 days
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        subShowChangeLog(o.Owner.Items.IndexOf(o))


    End Sub

    Private Sub mnuSaveValve_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                    Handles mnuSaveValve.Click
        '********************************************************************************************
        'Description:  Save by valve choice
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim lRet As DialogResult
        Dim nValve As Integer = mRobot.SystemColors(cboColor.Text).Valve.Number.Value
        Dim sTmp As String = gpsRM.GetString("psSAVE_BY_VALVE_MSG") & " " & _
                            nValve.ToString & "?"



        lRet = MessageBox.Show(sTmp, gcsRM.GetString("csSAVE_CHANGES"), _
                            MessageBoxButtons.OKCancel, _
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)

        Select Case lRet
            Case Response.OK 'Response.Yes
                subSaveByValve(nValve)

        End Select
    End Sub
    Private Sub mnuRemote_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                   Handles mnuRemote.Click
        '********************************************************************************************
        'Description:  Select a remote zone
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim bTmp As Boolean = mbRemoteZone
        mbEventBlocker = True
        If SetRemoteServer() Then
            mbRemoteZone = True
            moPassword = Nothing

            If colArms Is Nothing = False Then
                For i As Integer = colArms.Count - 1 To 0 Step -1
                    colArms.Remove(colArms(i))
                Next
                colArms = Nothing
            End If
            If colControllers Is Nothing = False Then
                For i As Integer = colControllers.Count - 1 To 0 Step -1
                    colControllers.Remove(colControllers(i))
                Next
                colControllers = Nothing
            End If
            GC.Collect()
            System.Windows.Forms.Application.DoEvents()
            mPWRobotCommon.UrbanRenewal()
            System.Windows.Forms.Application.DoEvents()
            subInitializeForm()
        Else
            mbRemoteZone = bTmp
        End If
        mbEventBlocker = False
    End Sub
    Private Sub mnuLocal_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuLocal.Click
        '********************************************************************************************
        'Description:  Select a local zone
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbRemoteZone = False
        moPassword = Nothing
        RemotePWPath = String.Empty

        If colArms Is Nothing = False Then
            For i As Integer = colArms.Count - 1 To 0 Step -1
                colArms.Remove(colArms(i))
            Next
            colArms = Nothing
        End If
        If colControllers Is Nothing = False Then
            For i As Integer = colControllers.Count - 1 To 0 Step -1
                colControllers.Remove(colControllers(i))
            Next
            colControllers = Nothing
        End If
        GC.Collect()
        System.Windows.Forms.Application.DoEvents()
        mPWRobotCommon.UrbanRenewal()
        System.Windows.Forms.Application.DoEvents()
        subInitializeForm()

    End Sub

    Private Sub oIPC_NewMessage(ByVal Schema As String, ByVal DS As System.Data.DataSet) Handles oIPC.NewMessage
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

        If Me.InvokeRequired Then
            Dim dNewMessage As New NewMessage_CallBack(AddressOf oIPC_NewMessage)
            Me.BeginInvoke(dNewMessage, New Object() {Schema, DS})
        Else
            Dim DR As DataRow = DS.Tables(Paintworks_IPC.clsInterProcessComm.sTABLE).Rows(0)

            Select Case Schema.ToLower
                Case oIPC.CONTROL_MSG_SCHEMA.ToLower
                    'TODO - Handle language change requests here.
                    'Call subDoControlAction(DR)
                Case oIPC.PASSWORD_MSG_SCHEMA.ToLower
                    Call moPassword.ProcessPasswordMessage(DR)
                Case Else
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: oIPC_NewMessage", _
                                           "Unrecognized schema [" & Schema & "].")
            End Select
        End If 'Me.InvokeRequired

    End Sub

    Private Sub btnFunction_DropDownOpening(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFunction.DropDownOpening
        '********************************************************************************************
        'Description:  this was needed to enable the menus for some reason
        '                  now handled in dostatusbar 2
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim cachePrivilege As ePrivilege
        Dim bRem As Boolean = False
        Dim bAllow As Boolean = False


        If LoggedOnUser <> String.Empty Then

            'If moPrivilege.Privilege = String.Empty Then 'RJO 03/23/12
            '    cachePriviledge = ePrivilege.None
            'Else
            '    If moPrivilege.ActionAllowed Then
            '        cachePriviledge = mPassword.PrivilegeStringToEnum(moPrivilege.Privilege)
            '    Else
            '        cachePriviledge = ePrivilege.None
            '    End If
            'End If

            If moPassword.Privilege = ePrivilege.None Then
                cachePrivilege = ePrivilege.None
            Else
                If moPassword.ActionAllowed Then
                    cachePrivilege = moPassword.Privilege
                Else
                    cachePrivilege = ePrivilege.None
                End If
            End If

            Privilege = ePrivilege.Remote

            If Privilege = ePrivilege.Remote Then
                If colZones.PaintShopComputer = False Then
                    bAllow = True
                End If
            End If

            Privilege = cachePrivilege

        Else
            Privilege = ePrivilege.None
        End If

        If colZones.PaintShopComputer = False Then
            bRem = mbRemoteZone
        End If

        DoStatusBar2(stsStatus, LoggedOnUser, Privilege, bRem, bAllow)


    End Sub
    Private Sub rb_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)

        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'this doevents gives any changed textbox time to update its new value
        Application.DoEvents()
        If DataLoaded Then Call subShowNewPage()
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
        '********************************************************************************************
        Dim sKeyValue As String = String.Empty

        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode
                Case Keys.F1
                    'Help Screen request
                    sKeyValue = "btnHelp"
                    Select Case meScreen
                        Case eScreenSelect.CCPresets
                            subLaunchHelp(gs_HELP_CCPRESETS, oIPC)
                        Case eScreenSelect.EstatPresets
                            subLaunchHelp(gs_HELP_ESTATPRESETS, oIPC)
                        Case eScreenSelect.FluidPresets
                            subLaunchHelp(gs_HELP_PRESETS, oIPC)
                        Case Else
                            subLaunchHelp(gs_HELP_PRESETS, oIPC)
                    End Select
                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty

                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    Select Case meScreen
                        Case eScreenSelect.CCPresets
                            oSC.CaptureWindowToFile(Me.Handle, sDumpPath & msSCREEN_DUMP_NAME_CC & msSCREEN_DUMP_EXT, Imaging.ImageFormat.Jpeg)
                        Case eScreenSelect.EstatPresets
                            oSC.CaptureWindowToFile(Me.Handle, sDumpPath & msSCREEN_DUMP_NAME_ESTAT & msSCREEN_DUMP_EXT, Imaging.ImageFormat.Jpeg)
                        Case eScreenSelect.FluidPresets
                            oSC.CaptureWindowToFile(Me.Handle, sDumpPath & msSCREEN_DUMP_NAME & msSCREEN_DUMP_EXT, Imaging.ImageFormat.Jpeg)
                        Case Else
                            oSC.CaptureWindowToFile(Me.Handle, sDumpPath & msSCREEN_DUMP_NAME & msSCREEN_DUMP_EXT, Imaging.ImageFormat.Jpeg)
                    End Select

                Case Keys.Escape
                    Me.Close()

                Case Else

            End Select
        End If

        'TODO - Add HotKeys and (invisible) Help button
        'If sKeyValue = String.Empty Then
        '    'Trap Main Menu HotKey presses
        '    If e.Alt And (Not (e.Control Or e.Shift)) Then
        '        For Each oMenuButton As Windows.Forms.ToolStripButton In tlsMain.Items
        '            If oMenuButton.Tag.ToString = e.KeyCode.ToString Then
        '                sKeyValue = oMenuButton.Name
        '                Exit For
        '            End If
        '        Next 'oMenuButton
        '    End If
        'End If 'sKeyValue = String.Empty

        'If sKeyValue <> String.Empty Then
        '    'Click the Menu Button
        '    If tlsMain.Items(sKeyValue).Enabled Then
        '        tlsMain.Items(sKeyValue).PerformClick()
        '    End If
        'End If

    End Sub
#End Region
#Region " Temp stuff for old password object "

    Private Sub moPassword_LogIn() Handles moPassword.LogIn
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/01/09  MSW     handle cross-thread calls
        '********************************************************************************************
        'This can get called by the password thread
        If Me.stsStatus.InvokeRequired Then
            Dim dLogin As New LoginCallBack(AddressOf moPassword_LogIn)
            Me.BeginInvoke(dLogin)
        Else
            Me.LoggedOnUser = moPassword.UserName
            Status = Me.LoggedOnUser & " " & gcsRM.GetString("csLOGGED_IN")
            Privilege = ePrivilege.Execute 'don't allow Override Access without this priviledge 01/05/10 RJO
            If Privilege <> ePrivilege.Execute Then
                Privilege = ePrivilege.Copy ' extra for buttons etc.
                If Privilege <> ePrivilege.Copy Then
                    'didn't have clearance
                    Privilege = ePrivilege.Edit
                End If
            End If
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/23/12
        End If
    End Sub
    Private Sub moPassword_LogOut() Handles moPassword.LogOut
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/01/09  MSW     handle cross-thread calls
        '********************************************************************************************
        'This can get called by the password thread
        If Me.stsStatus.InvokeRequired Then
            Dim dLogin As New LogOutCallBack(AddressOf moPassword_LogOut)
            Me.BeginInvoke(dLogin)
        Else
            Status = Me.LoggedOnUser & " " & gcsRM.GetString("csLOGGED_OUT")
            Me.LoggedOnUser = String.Empty
            Me.Privilege = ePrivilege.None
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/23/12
        End If

    End Sub

#End Region


End Class