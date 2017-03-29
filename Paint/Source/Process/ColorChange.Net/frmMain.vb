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
' 04/24/2008    gks     Convert to use SQL database "Process Data"                  4.00.01.00
' 06/20/2008    gks     Add Estat Descriptions                                      4.00.02.00
' 06/01/2009    MSW     convert to VB2008.  Handle class changes made for Fluid Maint,
'                       handle cross-thread calls in colControllers_ConnectionStatusChange(),
'                       moPassword_LogIn(), and moPassword_LogOut()
'                       subInitFormText() - removed ApplicatorType, use ColorChangeType instead
'    11/10/09   MSW     Modifications for PaintTool 7.50 presets
'                       Reads preset config from robot.
'                       parm name on the robot is used for resource lookup:
'                       for APP_CON_NAM= "FF", rsFF = "Paint", rsFF_CAP = "Paint cc/min", rsFF_UNITS ="
'                       supports 2nd shaping air, volume on CC screen
'    02/15/10   BTK     Added color change type for Versabell2 32 valves.
'    02/17/10   BTK     Changed dgGridView_MouseDown to force step selection when   4.00.03.00
'                       clicking in the gray area to the right of step data.  Without
'                       this the screen would crash when adding, deleting or
'                       inserting a step.
'    12/13/10   MSW     html print
'    12/13/10   MSW     allow import from and export to csv
'    09/14/11   MSW     Assemble a standard version of everything                       4.1.0.0
'    09/27/11   MSW     Standalone changes round 1 - HGB SA paintshop computer changes	4.1.0.1
'    09/29/11   MSW     DataLoaded - Don't update the display unless everything is selected 4.1.0.1
'    12/02/11   MSW     Add DMONCfg placeholder reference                               4.1.1.0
'    01/03/12   MSW     Update for speed, import/export improvements                    4.1.1.1
'    01/10/12   MSW     More updates for speed, import/export improvements              4.01.01.02
'    01/24/12   MSW     Applicator Updates                                            4.01.01.03
'    02/16/12   MSW     Print/import/export updates, force 32 bit build               4.01.01.04
'    03/22/12   RJO     Modified for .NET Password and IPC                            4.01.02.00
'    04/11/12   MSW     Changed CommonStrings setup so it ubilds correctly            4.01.03.00
'    04/23/12   MSW     Set multiview form to update when param selected              4.01.03.01
'    06/07/12   MSW     Add some subEnableControl calls throughout for edits          4.01.03.02
'    10/24/12   RJO     Added StartupModule to project to prevent multiple instances  4.01.03.03
'    11/06/12   JBW/RJO Modified subShowCCPresets to command PW_Main to show the      4.01.03.04
'                       Presets screen. NOTE: Requires PW_Main v4.01.03.05 or later.
'    01/01/13   RJO     Added (unused) Style param to mCCCommonRtns                   4.01.03.05
'                       LoadCopyScreenSubParamBox for compatibility with frmCopy 
'                       v4.01.01.03
'    01/12/13   MSW     Fix loop range for group valve changelog                      4.01.03.06
'    01/31/13   MSW     Change the way labels are assigned from the applicator object 4.01.03.07
'                       so they don't get extra valve numbers each time a robot is selected
'    03/24/13   MSW     Add mbFormClosing to abort a data load if the screen starts 
'                       to close during a DoEvents call                               4.01.04.00
'    04/16/13   MSW     Add Canadian language files                                   4.01.05.00
'                       Standalone Changelog
'    07/09/13   MSW     Update and standardize logos                                  4.01.05.01
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up  4.01.05.02
'    09/30/13   MSW     Save screenshots as jpegs                                     4.01.06.00
'    10/11/13   MSW     add clsVersabell3WBCartoon                                    4.01.06.01
'    01/07/14   MSW     Remove controlbox from main for                               4.01.06.02
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call              4.01.07.00
'    02/26/14   CBZ     Change frmCopy from .ShowDialog to .Show                      4.01.07.01
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
    Friend Const msSCREEN_NAME As String = "ColorChange"   ' <-- For password area change log etc.
    Friend Const msSCREEN_DUMP_NAME As String = "Process_ColorChange"
    Friend Const msSCREEN_DUMP_EXT As String = ".jpg"
    Private Const mnMAININX As Integer = 100
    Private Const mnSCREENINX As Integer = 101
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    Friend Const mbUSEROBOTS As Boolean = True
    Private Const mnGRID_ROW_TIME As Integer = 0
    Private Const mnGRID_ROW_PRESET As Integer = 1
    Private Const mnGRID_ROW_ACTION As Integer = 2
    Private Const mnGRID_ROW_EVENT As Integer = 3
    Private Const mnGRID_ROW_VALVE1 As Integer = 4
    Private Const mnTOON_WIDTH As Integer = 672 '316    'NVSP 11/03/2016
    Private Const mnTOON_HEIGHT As Integer = 570 '640   'NVSP 11/03/2016 
    Private Const mnMIN_LEFT As Integer = 694
    Private Const mnMaxParmLabels As Integer = 6 ' How many preset labels are on the form
    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    Friend Shared gsChangeLogArea As String = msSCREEN_NAME
    Private msOldZone As String = String.Empty
    Private msOldRobot As String = String.Empty
    Private msOldColor As String = String.Empty
    Private msOldCycle As String = String.Empty
    Private mnOldCycle As Integer = 0
    Private colZones As clsZones = Nothing
    Private WithEvents colControllers As clsControllers = Nothing
    Private WithEvents colArms As clsArms = Nothing
    Private mRobot As clsArm = Nothing
    Private colUpDownBoxes As Collection(Of NumericUpDown) = _
                            New Collection(Of NumericUpDown)
    Private mbEventBlocker As Boolean = False
    Private mbPrintBlocker As Boolean = False
    Private mbRemoteZone As Boolean = False
    Private mbScreenLoaded As Boolean = False
    Private mbFormClosing As Boolean = False
    Private mColorChange As clsColorChange = Nothing
    Private mnStep As Integer = 1
    Private mPrintHtml As clsPrintHtml
    'Applicator config
    Friend WithEvents colApplicators As clsApplicators = Nothing
    Private meOldApplicator As eColorChangeType = eColorChangeType.NOT_SELECTED
    Private moSysColors As clsSysColors
    Friend mApplicator As clsApplicator

    'Preset config
    Friend mnNumParms As Integer = 1 'Current number of columns visible
    Friend mParmDetail() As tParmDetail = Nothing

    'Valve names
    Private msSharedValveNames() As String = Nothing
    Private msGroupValveNames() As String = Nothing
    Private msSharedValveLabels() As String = Nothing
    Private msGroupValveLabels() As String = Nothing
    Private mcSharedValveColors() As Color = Nothing
    Private mcGroupValveColors() As Color = Nothing
    Private mCCPresets As clsPresets = Nothing

    Private mnSaveRowHeight As Single = 0
    Private mnGridDragStartMouse As Single
    Private mnGridDragStartGripPos As Single
    Private mnGridDragActive As Integer = 0
    Private Const mnGridPtsPerSec As Integer = 35
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean = False
    Private mbPresetsLoaded As Boolean = False
    Private mnProgress As Integer = 0
    Private mnPopupMenuStep As Integer = 0
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    Private msCulture As String = "en-US" 'Default to english
    '******** End Property Variables    *************************************************************

    '******** This is the old pw3 password object interop  *****************************************
    'Friend WithEvents moPassword As PWPassword.PasswordObject 'RJO 03/22/12
    'Friend moPrivilege As PWPassword.cPrivilegeObject 'RJO 03/22/12
    '******** This is the old pw3 password object interop  *****************************************
    Friend WithEvents moPassword As clsPWUser 'RJO 03/22/12
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm 'RJO 03/22/12

    'Private Shared mbNoRecursion As Boolean = False 'RJO 03/22/12
    Private msColorCache As String = String.Empty

    '******** Cartoon Variables   *******************************************************************
    'CC type class from ColorChange.vb This'll get assigned a type-specific class inherited from clsColorChangeCartoon
    Private WithEvents mCCToon As clsColorChangeCartoon = Nothing
    'This'll get assigned to a cctype specific user-control that has the actual drawing.
    Friend uctrlCartoon As UserControl = Nothing

    '******** End Cartoon Variables   ****************************************************************

    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    Delegate Sub ControllerStatusChange(ByVal Controller As clsController)
    Delegate Sub NewMessage_Callback(ByVal Schema As String, ByVal DS As DataSet) 'RJO 03/22/12

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
            mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, msBASE_ASSEMBLY_LOCAL, _
                                          msROBOT_ASSEMBLY_LOCAL)
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


            'If mPassword.CheckPassword(msSCREEN_NAME, Value, moPassword, moPrivilege) Then 'RJO 03/22/12
            If moPassword.CheckPassword(Value) Then
                'passed
                mPrivilegeGranted = mPrivilegeRequested
                'if privilege changed may have to enable controls
                subEnableControls(True)
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
            Else
                'denied
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
        ' 09/29/11  MSW     Don't update the display unless everything is selected
        '********************************************************************************************
        Get
            Return mbDataLoaded
        End Get
        Set(ByVal Value As Boolean)
            mbDataLoaded = Value
            If mbDataLoaded Then
                'just finished loading reset & refresh
                If (cboCycle.SelectedIndex >= 0) And (cboColor.SelectedIndex >= 0) And _
                    (cboRobot.SelectedIndex >= 0) Then
                    subShowNewPage()
                End If
            End If
            'if not loaded disable all controls
            subEnableControls(Value)
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

#End Region
#Region " Routines "
    Public Sub UpdateStatus(ByVal nprogress As Integer, ByVal sStatus As String)
        Status(True) = sStatus
        Progress = nprogress
    End Sub
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
            Case Else
                Return True
        End Select

    End Function
    Friend Function bLoadFromGUI(ByVal Zone As clsZone, ByRef rRobot As clsArm, _
                                                    ByRef rColorChange As clsColorChange) As Boolean
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

            'Everything we need from the DB should be handled by the color change classes
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
    Friend Function bLoadFromRobot(ByRef rRobot As clsArm, ByRef rColorChange As clsColorChange) As Boolean
        '********************************************************************************************
        'Description:  Load data stored on Robot
        '
        'Parameters: none
        'Returns:    True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            'If bNoRobotDebug = False Then
            If rRobot.IsOnLine Then
                If LoadColorChangeFromRobot(rRobot, mColorChange, AddressOf UpdateStatus, 20, 90) Then
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

            'End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    ''''''''Friend Function bLoadFromRobotnot(ByRef rRobot As clsArm, ByRef rColorChange As clsColorChange) As Boolean
    ''''''''    '********************************************************************************************
    ''''''''    'Description:  Load data stored on Robot
    ''''''''    'NRU 161103
    ''''''''    'Parameters: none
    ''''''''    'Returns:    True if load success
    ''''''''    '
    ''''''''    'Modification history:
    ''''''''    '
    ''''''''    ' Date      By      Reason
    ''''''''    '********************************************************************************************

    ''''''''    Try


    ''''''''        LoadColorChangeFromRobotNot(rRobot, mColorChange, AddressOf UpdateStatus, 20, 90)

    ''''''''        Return True


    ''''''''    Catch ex As Exception
    ''''''''        Trace.WriteLine(ex.Message)
    ''''''''        Trace.WriteLine(ex.StackTrace)
    ''''''''        ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED"), ex, msSCREEN_NAME, _
    ''''''''                            Status, MessageBoxButtons.OK)

    ''''''''        Return False

    ''''''''    End Try

    ''''''''End Function
    Friend Function bSaveToGUI(ByRef rRobot As clsArm, ByRef rColorChange As clsColorChange) As Boolean
        '********************************************************************************************
        'Description:  Save data stored on GUI in database, xml file etc.
        '
        'Parameters: Nothing for now, leaving the structure in here in case something gets added.
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            'Everything we need from the DB should be handled by the color change classes

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
        'Parameters: nothing for now, leaving the calls in in case something gets added.
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
    Friend Function bSaveToRobot(ByRef rRobot As clsArm, ByRef rColorChange As clsColorChange) As Boolean
        '********************************************************************************************
        'Description:  Save data stored on Robot
        '
        'Parameters: none
        'Returns:    True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try

            If rRobot.IsOnLine Then
                If SaveColorChangeToRobot(rRobot, mColorChange, AddressOf UpdateStatus, 20, 90, mCCToon) Then
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
        ' 11/28/09  RJO     Modified call to LoadArmCollection to not include openers
        '********************************************************************************************
        Progress = 10
        If Not (mColorChange Is Nothing) Then
            If mColorChange.Changed Then
                ' false means user pressed cancel
                If bAskForSave() = False Then
                    cboZone.Text = msOldZone
                    Progress = 100
                    Exit Sub
                End If
            End If
        End If

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

        msColorCache = String.Empty

        Try

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subClearScreen()

            subEnableControls(False)

            colControllers = New clsControllers(colZones, False)
            SetUpStatusStrip(Me, colControllers)
            colArms = LoadArmCollection(colControllers, False)
            colApplicators = New clsApplicators(colArms, colZones.ActiveZone)
            For Each oArm As clsArm In colArms
                oArm.Applicator = colApplicators(oArm.ColorChangeType)
            Next
            meOldApplicator = eColorChangeType.NOT_SELECTED
            If colApplicators.Count = 1 Then
                mApplicator = colApplicators.Item(0)
            End If
            mPWRobotCommon.LoadRobotBoxFromCollection(cboRobot, colArms, False, eColorChangeType.NOT_NONE)
            'make sure the robot cartoons get the right color
            colControllers.RefreshConnectionStatus()
            cboColor.Items.Clear()
            msOldColor = String.Empty
            msOldRobot = String.Empty

            'statusbar text
            If cboRobot.Text = String.Empty Then
                Status(True) = gcsRM.GetString("csSELECT_ROBOT")
            End If

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
    Private Sub ClearCycleData()
        '********************************************************************************************
        'Description:  Clear out everything that displays cycle specific data
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        lstStepActEvt.Items.Clear()
        lstStepActEvt.ForeColor = Color.Black
        dgGridView.SelectionMode = DataGridViewSelectionMode.CellSelect
        dgGridView.ColumnCount = 1
        dgGridView.Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable
        dgGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect
        mbEventBlocker = True
        nudPreset.Value = nudPreset.Minimum
        nudPreset.ForeColor = Color.Black
        nudStepTime.Value = nudStepTime.Minimum
        nudStepTime.ForeColor = Color.Black
        nudPushVol.Value = nudPushVol.Minimum
        nudPushVol.ForeColor = Color.Black
    End Sub
    Private Function ActionEventLine(ByVal nStep As Integer, ByVal nAction As Integer, ByVal nEvent As Integer) As String
        Dim sSpacer As String
        If nStep >= 10 Then
            sSpacer = ""
        Else
            sSpacer = " "
        End If

        Return (sSpacer & nStep.ToString & ":" & _
                                    mColorChange.ActionName(mColorChange.Cycle(mnOldCycle).Steps(nStep).StepAction.Value) & _
                                    "/" & mColorChange.EventName(mColorChange.Cycle(mnOldCycle).Steps(nStep).StepEvent.Value))
    End Function
    Private Sub subDrawSteps()
        '********************************************************************************************
        'Description:  update display when a new cycle is selected or steps are inserted or deleted
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        ClearCycleData()
        If Not (DataLoaded) Or (mnOldCycle = 0) Then
            Exit Sub
        End If
        If mColorChange.Cycle(mnOldCycle).NumberOfSteps = 0 Then
            mnuAction.Enabled = False
            mnuEvent.Enabled = False
            'Who new? you can enter cycles on the robot with 0 steps
            'If we leave it in column select, the columncount call drashes
            dgGridView.SelectionMode = DataGridViewSelectionMode.CellSelect
            dgGridView.ColumnCount = 1
            Dim nStep As Integer = 1
            dgGridView.Columns(nStep - 1).HeaderText = String.Empty
            dgGridView.Columns(nStep - 1).SortMode = DataGridViewColumnSortMode.NotSortable
            dgGridView.Rows(mnGRID_ROW_TIME).Cells(nStep - 1).ValueType = GetType(Single)
            dgGridView.Rows(mnGRID_ROW_PRESET).Cells(nStep - 1).ValueType = GetType(Integer)
            dgGridView.Rows(mnGRID_ROW_ACTION).Cells(nStep - 1).ValueType = GetType(String)
            dgGridView.Rows(mnGRID_ROW_EVENT).Cells(nStep - 1).ValueType = GetType(String)
            subUpdateValveDisplay(nStep, True, (mnStep = nStep), (mnStep = nStep))
            dgGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect
            subUpdateTotalTime()
        Else
            mnuAction.Enabled = True
            mnuEvent.Enabled = True
            'If we leave it in column select, the columncount call drashes
            dgGridView.SelectionMode = DataGridViewSelectionMode.CellSelect
            dgGridView.ColumnCount = mColorChange.Cycle(mnOldCycle).NumberOfSteps
            For nStep As Integer = 1 To mColorChange.Cycle(mnOldCycle).NumberOfSteps
                dgGridView.Columns(nStep - 1).HeaderText = nStep.ToString
                dgGridView.Columns(nStep - 1).SortMode = DataGridViewColumnSortMode.NotSortable
                dgGridView.Rows(mnGRID_ROW_TIME).Cells(nStep - 1).ValueType = GetType(Single)
                dgGridView.Rows(mnGRID_ROW_PRESET).Cells(nStep - 1).ValueType = GetType(Integer)
                dgGridView.Rows(mnGRID_ROW_ACTION).Cells(nStep - 1).ValueType = GetType(String)
                dgGridView.Rows(mnGRID_ROW_EVENT).Cells(nStep - 1).ValueType = GetType(String)

                lstStepActEvt.Items.Add(ActionEventLine(nStep, mColorChange.Cycle(mnOldCycle).Steps(nStep).StepAction.Value, _
                                                        mColorChange.Cycle(mnOldCycle).Steps(nStep).StepEvent.Value))
                subUpdateValveDisplay(nStep, True, (mnStep = nStep), (mnStep = nStep))
            Next
            dgGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect
            subUpdateTotalTime()
        End If
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
        ClearCycleData()
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
        ' 02/26/14  CBZ     Change frmCopy from .ShowDialog to .Show
        '********************************************************************************************
        'make sure edits are saved
        If Not (mColorChange Is Nothing) Then
            If mColorChange.Changed Then



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
                .ParameterType = ColorChange.frmCopy.eParamType.Valves
                .ParamName = gcsRM.GetString("csCOLOR_CAP")
                .SubParamName = gcsRM.GetString("csCYCLE_CAP")
                .UseParam = True
                .UseSubParam = true
                .UseStyle = False
                .SubParamToFromMustMatch = True
                .FancyCopyButtons = False
                .LoadSubParamByRobot = True
                .CCType1 = mRobot.ColorChangeType
                .CCType2 = eColorChangeType.NOT_SELECTED
                .IncludeOpeners = False
                '    02/26/14   CBZ     Change frmCopy from .ShowDialog to .Show
                .Show()  'From Carl - .ShowDialog was doing weird things on the latest PC

            End With

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            frmCopy = Nothing
        End Try
        mColorChange = Nothing
        If cboRobot.Text = String.Empty Then
            Status(True) = gcsRM.GetString("csSELECT_ROBOT")
        ElseIf cboColor.Text = String.Empty Then
            Status(True) = gcsRM.GetString("csSELECT_COLOR")
        Else
            subLoadData()
        End If
        If mnOldCycle > 0 Then
            subChangeCycle(mnOldCycle)
            Status(True) = gpsRM.GetString("csREADY")
        Else
            Status(True) = gpsRM.GetString("csSELECT_CYCLE")
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

        DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)

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
        '********************************************************************************************
        Dim bRestOfControls As Boolean = False
        Dim bEditsMade As Boolean = False
        If Not (mColorChange Is Nothing) Then
            bEditsMade = mColorChange.Changed
        End If
        btnClose.Enabled = True

        If bEnable = False Then
            btnSave.Enabled = False
            btnUndo.Enabled = False
            btnCopy.Enabled = False
            btnPrint.Enabled = False
            btnChangeLog.Enabled = False
            btnStatus.Enabled = True
            btnMultiView.Enabled = True
            bRestOfControls = False
            tblpData.Enabled = False
            nudPushVol.Enabled = False
            nudStepTime.Enabled = False
            nudPreset.Enabled = False
            lstStepActEvt.Enabled = False
            lstValves.Enabled = False
            If Not (uctrlCartoon Is Nothing) Then
                uctrlCartoon.Enabled = False
            End If
            dgGridView.Enabled = False
            dgGridView.AllowUserToResizeColumns = False
            cboCycle.Enabled = False
            btnUtilities.Enabled = False
        Else
            tblpData.Enabled = DataLoaded And mnOldCycle > 0
            lstStepActEvt.Enabled = DataLoaded And mnOldCycle > 0
            lstValves.Enabled = DataLoaded
            dgGridView.Enabled = DataLoaded And mnOldCycle > 0
            cboCycle.Enabled = cboColor.Text <> String.Empty
            Select Case Privilege
                Case ePrivilege.None
                    btnSave.Enabled = False
                    btnUndo.Enabled = False
                    btnCopy.Enabled = False
                    btnPrint.Enabled = DataLoaded
                    btnChangeLog.Enabled = True
                    btnStatus.Enabled = True
                    bRestOfControls = False
                    btnMultiView.Enabled = True
                    nudPushVol.Enabled = False
                    nudStepTime.Enabled = False
                    nudPreset.Enabled = False
                    lstStepActEvt.ContextMenuStrip = Nothing
                    dgGridView.ContextMenuStrip = Nothing
                    If Not (uctrlCartoon Is Nothing) Then
                        uctrlCartoon.Enabled = False
                    End If
                    dgGridView.AllowUserToResizeColumns = rdoSteps.Checked
                    btnUtilities.Enabled = False
                Case ePrivilege.Edit
                    btnSave.Enabled = bEditsMade
                    btnUndo.Enabled = bEditsMade
                    btnPrint.Enabled = DataLoaded
                    btnMultiView.Enabled = True
                    btnChangeLog.Enabled = True
                    btnStatus.Enabled = True
                    bRestOfControls = True
                    btnCopy.Enabled = False
                    nudPushVol.Enabled = True
                    nudStepTime.Enabled = True
                    nudPreset.Enabled = True
                    lstStepActEvt.ContextMenuStrip = mnuStepActEvt
                    dgGridView.ContextMenuStrip = mnuStepActEvt
                    If Not (uctrlCartoon Is Nothing) Then
                        uctrlCartoon.Enabled = True
                    End If
                    dgGridView.AllowUserToResizeColumns = True
                    btnUtilities.Enabled = DataLoaded
                Case ePrivilege.Copy
                    btnCopy.Enabled = (mRobot IsNot Nothing)
                    btnSave.Enabled = bEditsMade
                    btnUndo.Enabled = bEditsMade
                    btnPrint.Enabled = DataLoaded
                    btnMultiView.Enabled = True
                    btnChangeLog.Enabled = True
                    btnStatus.Enabled = True
                    bRestOfControls = True
                    nudPushVol.Enabled = True
                    nudStepTime.Enabled = True
                    nudPreset.Enabled = True
                    If Not (uctrlCartoon Is Nothing) Then
                        uctrlCartoon.Enabled = True
                    End If
                    lstStepActEvt.ContextMenuStrip = mnuStepActEvt
                    dgGridView.ContextMenuStrip = mnuStepActEvt
                    dgGridView.AllowUserToResizeColumns = True
                    btnUtilities.Enabled = DataLoaded
            End Select
        End If

    End Sub
    Private Sub subFormatScreenLayout()
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
        gpbPushVol.Visible = False

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
        '********************************************************************************************

        With gpsRM
            gpbGridView.Text = gpsRM.GetString("psGRID_VIEW")
            gpbPushVol.Text = gpsRM.GetString("psPUSH_VOL")
            gpbStepActEvt.Text = gpsRM.GetString("psSTEP_ACT_EVT")
            gpbStepTime.Text = gpsRM.GetString("psSTEP_TIME")
            gpbTotalTime.Text = gpsRM.GetString("psTOTAL_TIME")
            gpbValves.Text = gpsRM.GetString("psVALVES")
            gpbPreset.Text = gpsRM.GetString("psPRESET")
            mnuAction.Text = gpsRM.GetString("psACTION")
            mnuEvent.Text = gpsRM.GetString("psEVENT")
            mnuInsert.Text = gpsRM.GetString("psINSERT")
            mnuDelete.Text = gpsRM.GetString("psDELETE")
            mnuAppend.Text = gpsRM.GetString("psAPPEND")
            For nParm As Integer = 1 To mnMaxParmLabels
                Dim lblTmp As Label = DirectCast(gpbPreset.Controls("lblParm" & nParm.ToString("00")), Label)
                lblTmp.Text = String.Empty
                lblTmp.Visible = False
            Next
            btnTall.Image = DirectCast(gpsRM.GetObject("Previous"), Image)
            btnTall.Tag = "Previous"
            btnWide.Image = DirectCast(gpsRM.GetObject("Forward"), Image)
            btnWide.Tag = "Forward"
            'Custom print menu
            mnuAllCycles.Text = gpsRM.GetString("psPRINT_ALL")
            'CC presets button
            btnCCPresets.Text = gpsRM.GetString("psCCPRESETS")
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
        '********************************************************************************************
        Dim lReply As Windows.Forms.DialogResult = Response.OK

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor


        Status = gcsRM.GetString("csINITALIZING")
        Progress = 1

        Try
            DataLoaded = False
            mbScreenLoaded = False

            subProcessCommandLine()

            'init the old password for now
            'moPassword = New PWPassword.PasswordObject 'RJO 03/22/12
            'mPassword.InitPassword(moPassword, moPrivilege, LoggedOnUser, msSCREEN_NAME)
            'If LoggedOnUser <> String.Empty Then
            '    Privilege = ePrivilege.Copy ' extra for buttons etc.
            '    If Privilege <> ePrivilege.Copy Then
            '        'didn't have clearance
            '        Privilege = ePrivilege.Edit
            '        If moPrivilege.ActionAllowed Then
            '            Privilege = mPassword.PrivilegeStringToEnum(moPrivilege.Privilege)
            '        Else
            '            Privilege = ePrivilege.None
            '        End If
            '    End If
            'Else
            '    Privilege = ePrivilege.None
            'End If

            Progress = 5

            Me.Text = gpsRM.GetString("psSCREENCAPTION")

            'init new IPC and new Password 'RJO 03/22/12
            oIPC = New Paintworks_IPC.clsInterProcessComm
            moPassword = New clsPWUser
            Progress = 50

            colZones = New clsZones(String.Empty)
            mScreenSetup.LoadZoneBox(cboZone, colZones, False)
            mScreenSetup.InitializeForm(Me)
            mPrintHtml = New clsPrintHtml(msSCREEN_NAME, False, 40)
            subInitFormText()
            Progress = 70

            Progress = 98
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
        ' 10/12/10  MSW     switcvh to byvalve calls for selecting a system color
        '********************************************************************************************

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

        'give whatever called us time to repaint
        System.Windows.Forms.Application.DoEvents()
        If mbFormClosing Then
            Exit Sub
        End If
        DataLoaded = False

        Try
            Progress = 15

            'set up new Color change collection
            mColorChange = mRobot.SystemColors.ByValve(cboColor.Text).Valve.ColorChange
            mColorChange.ValveDescription = cboColor.Text
            mColorChange.Valve = mRobot.SystemColors.ByValve(cboColor.Text).Valve.Number.Value
            Progress = 20
            ''''''''If Not bNoRobotDebug Then 'NRU 161103
            If bLoadFromRobot(mRobot, mColorChange) Then
                Progress = 30
                If bLoadFromPLC() Then
                    If bLoadFromGUI(mRobot.Controller.Zone, mRobot, mColorChange) Then

                        Progress = 80

                        subFormatScreenLayout()
                        Application.DoEvents()
                        If mbFormClosing Then
                            Exit Sub
                        End If

                        If cboCycle.Text = String.Empty Then
                            subChangeCycle(1)
                        End If
                        'this refreshes screen & sets edit flag
                        DataLoaded = True
                        Status = gcsRM.GetString("csLOADDONE")

                    Else
                        'Load Failed
                    End If    'bLoadFromGUI()
                Else
                    'Load Failed
                End If  'bLoadFromPLC()
            Else
                'Load Failed
            End If  ' bLoadFromRobot()
            ''''''''Else 'NRU 161103
            ''''''''bLoadFromRobotnot(mRobot, mColorChange)
            ''''''''bLoadFromGUI(mRobot.Controller.Zone, mRobot, mColorChange)
            ''''''''Progress = 80
            ''''''''subFormatScreenLayout()
            ''''''''Application.DoEvents()
            ''''''''If cboCycle.Text = String.Empty Then
            ''''''''    subChangeCycle(1)
            ''''''''End If
            '''''''''this refreshes screen & sets edit flag
            ''''''''DataLoaded = True
            ''''''''Status = gcsRM.GetString("csLOADDONE")
            ''''''''End If 'mPWCommon.bNoRobotDebug

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
    Private Sub subChangeColor()
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
        If Not (mColorChange Is Nothing) Then
            If mColorChange.Changed Then

                ' false means user pressed cancel
                If bAskForSave() = False Then
                    cboColor.Text = msOldColor
                    Exit Sub
                End If
            End If  'EditsMade
        End If

        'just in case
        If cboColor.Text = String.Empty Then
            Exit Sub
        Else
            If cboColor.Text = msOldColor Then Exit Sub
        End If
        msOldColor = cboColor.Text
        msColorCache = cboColor.Text
        Dim bStatus As Boolean
        Try
            bStatus = lstStatus.Visible
            lstStatus.Visible = True
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            Progress = 0
            Status = gpsRM.GetString("psLOAD_CYCLE_DATA") & msOldRobot & gpsRM.GetString("ps_AND_COLOR") & msOldColor
            subClearScreen()
            Progress = 5
            subLoadData()
            mColorChange.ValveDescription = msOldColor

            If mnOldCycle > 0 Then
                subChangeCycle(mnOldCycle)
                Status(True) = gpsRM.GetString("csREADY")
            Else
                Status(True) = gpsRM.GetString("csSELECT_CYCLE")
            End If
            Progress = 95

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
            lstStatus.Visible = bStatus
        End Try


    End Sub

    Private Sub subProcessCommandLine()
        '********************************************************************************************
        'Description: If there are command line parameters - process here
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sArg As String
        For Each s As String In My.Application.CommandLineArgs
            'If a culture string has been passed in, set the current culture (display language)
            sArg = "/culture="
            If s.ToLower.StartsWith(sArg) Then
                Culture = s.Remove(0, sArg.Length)
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
        ' 01/31/13  MSW     Change the way labels are assigned from the applicator object
        '                   so they don't get extra valve numbers each time a robot is selected
        '********************************************************************************************

        If Not (mColorChange Is Nothing) Then
            If mColorChange.Changed Then
                ' false means user pressed cancel
                If bAskForSave() = False Then
                    cboRobot.Text = msOldRobot
                    Exit Sub
                End If
            End If  'EditsMade
        End If
        'just in case
        If cboRobot.Text = String.Empty Then
            Exit Sub
        Else
            If cboRobot.Text = msOldRobot Then Exit Sub
        End If
        Dim sTmp As String = msOldRobot
        If msOldRobot <> String.Empty Then _
                                colArms.Item(msOldRobot).Selected = False

        msOldRobot = cboRobot.Text

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            Progress = 5

            subClearScreen()
            Status = gpsRM.GetString("psSELECTING") & msOldRobot
            mRobot = colArms.Item(cboRobot.Text)
            colArms.Item(cboRobot.Text).Selected = True
            'If Not bNoRobotDebug Then 'NRU 161103
            If mRobot.IsOnLine = False Then
                Status = mRobot.Controller.Name & gcsRM.GetString("csIS_OFFLINE")
                lstStatus.Visible = True
                MessageBox.Show((mRobot.Controller.Name & gcsRM.GetString("csIS_OFFLINE")), _
                   (msSCREEN_NAME & " - " & mRobot.Controller.Name), _
                   MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                msOldRobot = String.Empty
                cboRobot.Text = sTmp
                Exit Sub
                'End If
            End If
            meOldApplicator = mRobot.ColorChangeType
            mApplicator = colApplicators(meOldApplicator)

            subInitFormText()
            Progress = 15

            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Just accessing mRobot.SystemColors ends up loading the details.  Calling mRobot.SystemColors.load does it again 
            moSysColors = mRobot.SystemColors ' = Nothing
            moSysColors.IgnorePresets = True
            mSysColorCommon.LoadValveBoxFromCollection(moSysColors, cboColor, True)

            msOldColor = String.Empty


            Progress = 30
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Toon stuff mostly copied from FluidMaint
            Dim sName As String = String.Empty

            'mColorChange = moSysColors(0).Valve.ColorChange

            mCCCommon.LoadCCCycleBox(mApplicator, cboCycle)
            'If Not bNoRobotDebug Then subInitParmConfig(mRobot, eParmSelect.CCPresets, mParmDetail, mnNumParms, mApplicator) 'NRU 161103

            Dim lblTmp As Label = DirectCast(gpbPreset.Controls("lblParm" & mnNumParms.ToString("00")), Label)
            gpbPreset.Height = lblTmp.Top + lblTmp.Height + 10
            If mnNumParms < mnMaxParmLabels Then
                For nParm As Integer = mnNumParms + 1 To mnMaxParmLabels
                    lblTmp = DirectCast(gpbPreset.Controls("lblParm" & nParm.ToString("00")), Label)
                    lblTmp.Text = String.Empty
                    lblTmp.Visible = False
                Next
            End If

            'Set up the fancy graphics
            SetCCType(colZones, Me.tscMain.ContentPanel.Controls, uctrlCartoon, mCCToon, mRobot, mApplicator, True)
            Progress = 40

            If ((uctrlCartoon Is Nothing) = False) And ((mCCToon Is Nothing) = False) Then ' no CC type yet, make a new one
                'Set up size and location on the form.
                uctrlCartoon.Size = New Size(mnTOON_WIDTH, mnTOON_HEIGHT)
                'Use tblpData.left for buffer
                Dim nLeft As Integer = tscMain.ContentPanel.Width - mnTOON_WIDTH - tblpData.Left
                If nLeft < mnMIN_LEFT Then
                    nLeft = mnMIN_LEFT
                End If
                uctrlCartoon.Location = New Point(nLeft, 0)
                Me.Refresh()
                uctrlCartoon.Visible = True
            End If
            Progress = 55

            'get the valve names from the cartoon
            ReDim msSharedValveNames(mCCToon.NumberOfSharedValves - 1)
            ReDim msGroupValveNames(mCCToon.NumberOfGroupValves - 1)
            Dim nIndex As Integer = 0
            For nIndex = 1 To mCCToon.NumberOfSharedValves
                msSharedValveNames(nIndex - 1) = gpsRM.GetString("psSH") & CInt(nIndex).ToString & ":" & mCCToon.SharedValveNames(nIndex - 1)
            Next
            For nIndex = 1 To mCCToon.NumberOfGroupValves
                msGroupValveNames(nIndex - 1) = gpsRM.GetString("psGP") & CInt(nIndex).ToString & ":" & mCCToon.GroupValveNames(nIndex - 1)
            Next
            'Get color for on state in gridview
            ReDim mcSharedValveColors(mCCToon.NumberOfSharedValves - 1)
            ReDim mcGroupValveColors(mCCToon.NumberOfGroupValves - 1)
            mCCToon.subGetValveColors(mcSharedValveColors, mcGroupValveColors)
            'get the valve abbreviations from the robot
            'This was getting assigned directly from mApplicator.SharedValvesCAP and GroupValvesCAP,
            ' It ended up adding the valve number in each time a new robot was selected.
            ReDim msSharedValveLabels(mCCToon.NumberOfSharedValves - 1)
            ReDim msGroupValveLabels(mCCToon.NumberOfGroupValves - 1)
            'subGetValveLabels(mRobot, msSharedValveLabels, msGroupValveLabels)
            For nIndex = 0 To mCCToon.NumberOfSharedValves - 1
                If nIndex <= msSharedValveLabels.GetUpperBound(0) Then
                    msSharedValveLabels(nIndex) = gpsRM.GetString("psSH") & CInt(nIndex + 1).ToString & ":" & mApplicator.SharedValvesCAP(nIndex)
                End If
            Next
            For nIndex = 0 To mCCToon.NumberOfGroupValves - 1
                If nIndex <= msGroupValveLabels.GetUpperBound(0) Then
                    msGroupValveLabels(nIndex) = gpsRM.GetString("psGP") & CInt(nIndex + 1).ToString & ":" & mApplicator.GroupValvesCAP(nIndex)
                End If
            Next

            'Get the action and event names
            Dim omnu As ToolStripMenuItem
            For Each omnu In mnuAction.DropDownItems
                nIndex = CInt(omnu.Name.Substring(omnu.Name.Length - 1))
                If nIndex > mApplicator.NumCCActions Then
                    omnu.Visible = False
                ElseIf mApplicator.CCAction(nIndex) = String.Empty Then
                    omnu.Visible = False
                Else
                    omnu.Text = mApplicator.CCAction(nIndex)
                    omnu.Visible = True
                End If
            Next
            For Each omnu In mnuEvent.DropDownItems
                nIndex = CInt(omnu.Name.Substring(omnu.Name.Length - 1))
                If nIndex > mApplicator.NumnCCEvents Then
                    omnu.Visible = False
                ElseIf mApplicator.CCEvent(nIndex) = String.Empty Then
                    omnu.Visible = False
                Else
                    omnu.Text = mApplicator.CCEvent(nIndex)
                    omnu.Visible = True
                End If
            Next

            'setup valve list box
            lstValves.Items.Clear()
            lstValves.Items.AddRange(msSharedValveNames)
            lstValves.Items.AddRange(msGroupValveNames)
            'setup the grid view
            With dgGridView
                .SuspendLayout()
                If rdoSteps.Checked Then
                    If .Top <> pnlScale.Top Then
                        .Top = pnlScale.Top
                        .Height = .Height + pnlScale.Height
                    End If
                    dgGridView.AllowUserToResizeColumns = True
                Else
                    If .Top = pnlScale.Top Then
                        .Top = pnlScale.Top + pnlScale.Height
                        .Height = .Height - pnlScale.Height
                    End If
                    dgGridView.AllowUserToResizeColumns = (Privilege = ePrivilege.Edit) Or (Privilege = ePrivilege.Copy)
                End If

                .Top = pnlScale.Top
                .Font = lstStatus.Font
                .ColumnHeadersVisible = True
                .RowCount = mnGRID_ROW_EVENT + 1 + mCCToon.NumberOfSharedValves + mCCToon.NumberOfGroupValves
                .RowHeadersVisible = True
                .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
                .RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing
                'Only the labels for now
                ClearCycleData()
                'If we leave it in column select, the columncount call drashes
                dgGridView.SelectionMode = DataGridViewSelectionMode.CellSelect
                dgGridView.ColumnCount = 1
                dgGridView.Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable
                dgGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect
                'Rows : Step#, time, preset, action, event,shared + group
                '.Rows(0).Cells(0).Style.WrapMode = DataGridViewTriState.True
                .RowHeadersVisible = True
                .AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders)
                .Rows(mnGRID_ROW_TIME).HeaderCell.ValueType = GetType(String)
                .Rows(mnGRID_ROW_PRESET).HeaderCell.ValueType = GetType(String)
                .Rows(mnGRID_ROW_ACTION).HeaderCell.ValueType = GetType(String)
                .Rows(mnGRID_ROW_EVENT).HeaderCell.ValueType = GetType(String)
                .Rows(mnGRID_ROW_TIME).HeaderCell.Value = gpsRM.GetString("psTIME")
                .Rows(mnGRID_ROW_PRESET).HeaderCell.Value = gpsRM.GetString("psPRESET")
                .Rows(mnGRID_ROW_ACTION).HeaderCell.Value = gpsRM.GetString("psACTION")
                .Rows(mnGRID_ROW_EVENT).HeaderCell.Value = gpsRM.GetString("psEVENT")
                For nIndex = mnGRID_ROW_VALVE1 To mnGRID_ROW_VALVE1 - 1 + mCCToon.NumberOfSharedValves
                    .Rows(nIndex).HeaderCell.ValueType = GetType(String)
                    If nIndex > msSharedValveLabels.GetUpperBound(0) Then
                        'msSharedValveLabels comes right from the robot, mCCToon is hardcoded and it provides msSharedValveNames
                        .Rows(nIndex).HeaderCell.Value = msSharedValveNames(nIndex - mnGRID_ROW_VALVE1)
                    Else
                        .Rows(nIndex).HeaderCell.Value = msSharedValveLabels(nIndex - mnGRID_ROW_VALVE1)
                    End If
                    If (nIndex > 0) And (InStr(.Rows(nIndex).HeaderCell.Value.ToString, grsRM.GetString("rsRESERVED")) > 0) Then
                        .Rows(nIndex).Visible = False
                    Else
                        .Rows(nIndex).Visible = True
                    End If
                Next
                For nIndex = (mnGRID_ROW_VALVE1 + mCCToon.NumberOfSharedValves) To (mnGRID_ROW_VALVE1 - 1 + mCCToon.NumberOfSharedValves + mCCToon.NumberOfGroupValves)
                    .Rows(nIndex).HeaderCell.ValueType = GetType(String)
                    If nIndex > msGroupValveLabels.GetUpperBound(0) Then
                        'msGroupValveLabels comes right from the robot, mCCToon is hardcoded and it provides msGroupValveNames
                        .Rows(nIndex).HeaderCell.Value = msGroupValveNames(nIndex - (mnGRID_ROW_VALVE1 + mCCToon.NumberOfSharedValves))
                    Else
                        .Rows(nIndex).HeaderCell.Value = msGroupValveLabels(nIndex - (mnGRID_ROW_VALVE1 + mCCToon.NumberOfSharedValves))
                    End If
                    If (nIndex > 0) And (InStr(.Rows(nIndex).HeaderCell.Value.ToString, grsRM.GetString("rsRESERVED")) > 0) Then
                        .Rows(nIndex).Visible = False
                    Else
                        .Rows(nIndex).Visible = True
                    End If
                Next
                .AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders)
                pnlScale.Left = .RowHeadersWidth + .Left
                pnlScale.Width = .Width - .RowHeadersWidth
                picScale.Left = 0

                .ResumeLayout()
            End With


            Progress = 70
            'Get preset data
            If mRobot.IsOnLine Then
                Status = gpsRM.GetString("psLOAD_CCPRESETS") & msOldRobot
                mCCPresets = New clsPresets
                mCCPresets.DisplayName = grsRM.GetString("rsCC_PRESETS")
                mCCPresets.FanucColor = 1
                mCCPresets.ColorChangePresets = True
                mbPresetsLoaded = LoadPresetsFromRobot(mRobot, mCCPresets, mParmDetail)
            Else
                mbPresetsLoaded = False
            End If
            Progress = 90
            'setup limits:
            nudStepTime.Minimum = CType(0.0, Decimal)
            nudStepTime.Maximum = CType(999.9, Decimal)
            If mbPresetsLoaded Then
                nudPreset.Minimum = CType(0, Decimal)
                nudPreset.Maximum = CType(mCCPresets.Count, Decimal)
            Else
                nudPreset.Minimum = CType(0, Decimal)
                nudPreset.Maximum = CType(40, Decimal)
            End If

            nudPushVol.Minimum = CType(0.0, Decimal)
            nudPushVol.Maximum = CType(999.9, Decimal)

            Progress = 96
            If cboColor.Items.Contains(msColorCache) Then
                cboColor.Text = msColorCache
                Status(True) = gpsRM.GetString("csSELECT_CYCLE")
            Else
                Status(True) = gcsRM.GetString("csSELECT_COLOR")
            End If

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
        ' 09/27/10  RJO     Added call to subEnableControls after successful save to disable Save and
        '                   Undo buttons.
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

        Try

            'make sure its good data
            If Not bValidateData() Then Exit Sub

            ' do save
            If bSaveToGUI(mRobot, mColorChange) Then
                If bSaveToPLC() Then
                    If bSaveToRobot(mRobot, mColorChange) Then
                        'mPWCommon.SaveToChangeLog(colZones.DatabasePath)
                        'For SQL database - remove above eventually
                        mPWCommon.SaveToChangeLog(colZones.ActiveZone)
                        ' save done
                        Progress = 0
                        Status = gcsRM.GetString("csSAVE_DONE")
                        subEnableControls(True)
                        subShowNewPage()

                    Else
                        'save failed

                    End If  'SaveToRobot()
                Else
                    'save failed

                End If    ' bSaveToPLC()
            Else
                'save failed

            End If      'bSaveToGUI()



        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally

            Me.Cursor = System.Windows.Forms.Cursors.Default

        End Try


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
                                          cboColor.Text, mDeclares.eParamType.Valves, nIndex, mbUSEROBOTS, True, oIPC)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
        End Try

    End Sub
    Private Sub subShowCCPresets()
        '********************************************************************************************
        'Description:  launch the CC presets screen
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/06/12  JBW/RJO PW_Main must launch the CC Presets screen so it will be notified when a 
        '           user logs in to Paintworks.
        '********************************************************************************************
        'Const sAppName As String = "Presets.exe"
        'Const sCmd As String = " ColorChange " 'The spaces are intentional
        'Const Quote As String = """"
        'Const sZoneArg As String = "/zone="
        'Const sRobotArg As String = "/robot="
        'Dim sRemote As String = String.Empty
        Dim sLaunchCommand As String = String.Empty

        Try
            ''Start up the preset screen, pass in "ColorChange", the zone and robot to start with
            'JBW/RJO make PW4_Main call cc presets
            sLaunchCommand = "presets.exe,colorchange /zone=" & cboZone.Text & " /robot=" & cboRobot.Text
            With oIPC
                Call oIPC.WriteControlMsg("pw4_main", "launchapp", sLaunchCommand)
            End With

            Dim oProcs() As Process = Process.GetProcessesByName("PW4_Main")

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
        ' 04/23/12  MSW     Set multiview form to update when param selected
        '********************************************************************************************
        Dim frmAll As New frmAll

        Try
            With frmAll
                .DatabasePath = colZones.DatabasePath
                .InitialZone = cboZone.Text
                .ScreenName = msSCREEN_NAME
                .ParameterType = ColorChange.mDeclares.eParamType.Valves
                .ShowDeviceCombo = mbUSEROBOTS
                .ShowParamCombo = mbUSEROBOTS
                .ResetSubParamOnNewParam = True
                .ParamName = gcsRM.GetString("csCOLOR_CAP")
                .SubParamName = gcsRM.GetString("csCYCLE_CAP")
                .ShowSubParamCombo = True
                .UseOneGrid = False
                .CCType1 = eColorChangeType.NOT_NONE
                .CCType2 = eColorChangeType.NOT_SELECTED
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
        '********************************************************************************************
        Dim nBox As Integer = 1
        Dim oT As TextBox = Nothing
        Dim sName As String = String.Empty
        Dim nPtr As Integer = 0

        mbEventBlocker = True

        subDrawSteps()
        subUpdateTotalTime()
        'Show pushout volume if it's supported
        If mColorChange.ShowPushoutVolume(mnOldCycle) Then
            subSetPushVol(mColorChange.PushOutVolume.Value)
            gpbPushVol.Visible = True
        Else
            gpbPushVol.Visible = False
        End If
        If mColorChange.Cycle(mnOldCycle).NumberOfSteps > 0 Then
            subSelectStep(1, False)
        End If

        mbEventBlocker = False

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
            If mnOldCycle > 0 Then
                subChangeCycle(mnOldCycle)
                Status(True) = gpsRM.GetString("csREADY")
            Else
                Status(True) = gpsRM.GetString("csSELECT_CYCLE")
            End If

        End If

    End Sub

#End Region
#Region " Events "

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '********************************************************************************************
        'Description: Abort routines that do a bunch of stuff after doevent calls
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbFormClosing = True
    End Sub

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
                Exit Sub
                'End
            Case "btnStatus"
                lstStatus.Visible = Not lstStatus.Visible

            Case "btnSave"
                'give validate a chance to fire
                Application.DoEvents()
                'privilege check done in subroutine
                subSaveData()

            Case "btnPrint"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                bPrintdoc(False)

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
            Case "btnCCPresets"
                subShowCCPresets()
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

        If Not (mColorChange Is Nothing) Then
            If mColorChange.Changed Then
                e.Cancel = (bAskForSave() = False)
            End If
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
        mbEventBlocker = False
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
            If mbFormClosing Then
                Exit Sub
            End If
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
            If mbFormClosing Then
                Exit Sub
            End If
            subChangeRobot()
        End If
    End Sub
    Private Sub cboColor_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
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
            If mbFormClosing Then
                Exit Sub
            End If
            subChangeColor()
        End If

    End Sub
    Private Sub frmMain_Layout(ByVal sender As Object, _
                            ByVal e As System.Windows.Forms.LayoutEventArgs) Handles MyBase.Layout
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
        Dim nHeight As Integer = tscMain.ContentPanel.Height - tblpData.Top
        If nHeight < 100 Then nHeight = 100
        Dim nWidth As Integer = tscMain.ContentPanel.Width - (tblpData.Left * 2)
        If nWidth < 100 Then nWidth = 100

        tblpData.Height = tscMain.ContentPanel.Height - tblpData.Top - stsStatus.Height - 3

        'Use tblpData.left for buffer
        Dim nLeft As Integer = tscMain.ContentPanel.Width - mnTOON_WIDTH - tblpData.Left
        If nLeft < mnMIN_LEFT Then
            nLeft = mnMIN_LEFT
        End If
        If Not (uctrlCartoon Is Nothing) Then
            uctrlCartoon.Left = nLeft
        End If
        Dim sTmp As String = DirectCast(btnWide.Tag, String)
        If sTmp = "Back" Then
            tblpData.Width = tscMain.ContentPanel.Width - 2 * tblpData.Left
        Else
            tblpData.Width = nLeft - 2 * tblpData.Left
        End If
        Const nMINSTATUS_LEFT As Integer = 600
        Const nMAXSTATUS_LEFT As Integer = 689
        Dim nStatusLeft As Integer = nWidth - lstStatus.Width
        If nStatusLeft < nMINSTATUS_LEFT Then
            nStatusLeft = nMINSTATUS_LEFT
        End If
        If nStatusLeft > nMAXSTATUS_LEFT Then
            nStatusLeft = nMAXSTATUS_LEFT
        End If
        lstStatus.Left = nStatusLeft
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
        ' 12/13/10  MSW     html print
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
        ' 12/13/10  MSW     html print
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
        ' 12/13/10  MSW     html print
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
        ' 12/13/10  MSW     html print
        '********************************************************************************************
        If DataLoaded Then
            If bPrintdoc(False) Then
                mPrintHtml.subShowPrintPreview()
            End If
        End If
    End Sub
    Private Function bPrintCycle(Optional ByVal bExportCSV As Boolean = False) As Boolean
        '********************************************************************************************
        'Description:  print the table
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/13/10  MSW     html print
        ' 12/22/11  MSW     Add a missing seperator in the title 
        '********************************************************************************************
        mPrintHtml.subSetPageFormat()
        mPrintHtml.subClearTableFormat()
        mPrintHtml.subSetColumnCfg("align=right", 0)
        mPrintHtml.subSetRowcfg("Bold=on", 0, 0)

        mPrintHtml.HeaderRowsPerTable = 1

        Dim sTitle(0) As String
        sTitle(0) = colZones.SiteName & " - " & colZones.CurrentZone
        If cboRobot.Visible Then
            sTitle(0) = sTitle(0) & " - " & gcsRM.GetString("csROBOT_CAP") & ":  " & cboRobot.Text
        End If
        Dim sTmp(1) As String
        sTmp(0) = cboCycle.Text
        sTmp(1) = cboColor.Text
        mPrintHtml.subAddObject(dgGridView, Status, sTitle, sTmp, sTmp(0))
    End Function
    Private Function bPrintdoc(ByVal bPrint As Boolean, Optional ByVal bExportCSV As Boolean = False) As Boolean
        '********************************************************************************************
        'Description:  print the table
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/13/10  MSW     html print
        ' 12/13/10  MSW     allow import from and export to csv
        '********************************************************************************************
        mbPrintBlocker = True
        subEnableControls(False)
        If (dgGridView IsNot Nothing) AndAlso DataLoaded Then
            Dim bCancel As Boolean = False
            mPrintHtml.subStartDoc(Status, msSCREEN_NAME, bExportCSV, bCancel)
            If bCancel = False Then
                If mnuAllCycles.Checked Then
                    For nCycle As Integer = 1 To cboCycle.Items.Count
                        subChangeCycle(nCycle)
                        Application.DoEvents()
                        bPrintCycle(bExportCSV)
                        If nCycle < cboCycle.Items.Count Then
                            mPrintHtml.subAddPageBreak(Status)
                        End If
                    Next
                Else
                    bPrintCycle(bExportCSV)
                End If
                mPrintHtml.subCloseFile(Status)
                If bPrint Then
                    mPrintHtml.subPrintDoc()
                End If
            End If
            subEnableControls(True)
            mbPrintBlocker = False
            Return (True)
        Else
            subEnableControls(True)
            mbPrintBlocker = False
            Return (False)
        End If
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
        ' 12/13/10  MSW     allow import from and export to csv
        '********************************************************************************************
        bPrintdoc(True, True)

    End Sub
    Private Sub subImportCycle(ByVal nCycleNum As Integer, ByRef oDT As DataTable)
        '********************************************************************************************
        'Description:  Import a cycle from a datatable
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 12/22/11  MSW     import UI improvements
        '********************************************************************************************
        Try
            'Select a cycle
            subChangeCycle(nCycleNum)
            Application.DoEvents()
            'Adjust the number of steps
            Dim nImportSteps As Integer = oDT.Columns.Count - 1
            Do While (nImportSteps > mColorChange.Cycle(mnOldCycle).NumberOfSteps)
                mColorChange.Cycle(mnOldCycle).InsertStep(mColorChange.Cycle(mnOldCycle).NumberOfSteps)
                subDrawSteps()
                Application.DoEvents()
            Loop
            Do While (nImportSteps < mColorChange.Cycle(mnOldCycle).NumberOfSteps)
                mColorChange.Cycle(mnOldCycle).RemoveStep(mColorChange.Cycle(mnOldCycle).NumberOfSteps)
                subDrawSteps()
                Application.DoEvents()
            Loop
            'Loop through the rows
            Dim nStep As Integer = 0
            For nDataRow As Integer = 0 To oDT.Rows.Count - 1
                Dim sItem As String = oDT.Rows(nDataRow).Item(0).ToString.Trim.ToLower
                If sItem = gpsRM.GetString("psTIME").Trim.ToLower Then
                    For nStep = 1 To nImportSteps
                        If IsNumeric(oDT.Rows(nDataRow).Item(nStep).ToString) Then
                            If nStep <= mColorChange.Cycle(mnOldCycle).NumberOfSteps Then
                                Debug.Print(oDT.Rows(nDataRow).Item(nStep).ToString)
                                subSetStepTime(nStep, CType(oDT.Rows(nDataRow).Item(nStep).ToString, Single))
                            End If
                        End If
                    Next
                ElseIf sItem = gpsRM.GetString("psPRESET").Trim.ToLower Then
                    For nStep = 1 To nImportSteps
                        If IsNumeric(oDT.Rows(nDataRow).Item(nStep).ToString) Then
                            If nStep <= mColorChange.Cycle(mnOldCycle).NumberOfSteps Then
                                subSetStepPreset(nStep, CType(oDT.Rows(nDataRow).Item(nStep).ToString, Integer))
                            End If
                        End If
                    Next
                ElseIf sItem = gpsRM.GetString("psACTION").Trim.ToLower Then
                    For nStep = 1 To nImportSteps
                        If nStep <= mColorChange.Cycle(mnOldCycle).NumberOfSteps Then
                            For nAction As Integer = 0 To mColorChange.NumActions
                                If oDT.Rows(nDataRow).Item(nStep).ToString.Trim.ToLower = mColorChange.ActionName(nAction).Trim.ToLower Then
                                    If (dgGridView.Rows(mnGRID_ROW_ACTION).Cells(nStep - 1).Value.ToString <> mColorChange.ActionName(nAction)) Then
                                        mColorChange.Cycle(mnOldCycle).Steps(nStep).StepAction.Value = nAction
                                        dgGridView.Rows(mnGRID_ROW_ACTION).Cells(nStep - 1).Value = mColorChange.ActionName(nAction)
                                        Exit For
                                    End If
                                End If
                            Next
                        End If
                    Next
                ElseIf sItem = gpsRM.GetString("psEVENT").Trim.ToLower Then
                    For nStep = 1 To nImportSteps
                        If nStep <= mColorChange.Cycle(mnOldCycle).NumberOfSteps Then
                            For nEvent As Integer = 0 To mColorChange.NumEvents
                                If oDT.Rows(nDataRow).Item(nStep).ToString.Trim.ToLower = mColorChange.EventName(nEvent).Trim.ToLower Then
                                    If (dgGridView.Rows(mnGRID_ROW_EVENT).Cells(nStep - 1).Value.ToString <> mColorChange.EventName(nEvent)) Then
                                        mColorChange.Cycle(mnOldCycle).Steps(nStep).StepEvent.Value = nEvent
                                        dgGridView.Rows(mnGRID_ROW_EVENT).Cells(nStep - 1).Value = mColorChange.EventName(nEvent)
                                        Exit For
                                    End If
                                End If
                            Next
                        End If
                    Next
                Else
                    Dim nValve As Integer = 0
                    Dim nValvesOn As Integer = 0
                    Dim nValvesDC As Integer = 0
                    Dim nValveState As Integer = 0
                    For nRow As Integer = mnGRID_ROW_EVENT + 1 To dgGridView.Rows.Count - 1
                        If sItem = dgGridView.Rows(nRow).HeaderCell.Value.ToString.Trim.ToLower Then
                            For nStep = 1 To nImportSteps
                                If nStep <= mColorChange.Cycle(mnOldCycle).NumberOfSteps Then
                                    If IsNumeric(oDT.Rows(nDataRow).Item(nStep).ToString) Then
                                        Dim nVal As Integer = CType(oDT.Rows(nDataRow).Item(nStep), Integer)
                                        If nRow >= mnGRID_ROW_VALVE1 + mCCToon.NumberOfSharedValves Then
                                            'Group
                                            nValve = nRow - (mnGRID_ROW_VALVE1 + mCCToon.NumberOfSharedValves)
                                            nValveState = mColorChange.Cycle(mnOldCycle).Steps(nStep).GroupValveState(nValve)
                                        Else
                                            'Shared
                                            nValve = nRow - (mnGRID_ROW_VALVE1)
                                            nValveState = mColorChange.Cycle(mnOldCycle).Steps(nStep).SharedValveState(nValve)
                                        End If
                                        If nVal <> nValveState Then
                                            Dim nBitMask As Integer = CType(2 ^ (nValve), Integer)
                                            If nRow >= mnGRID_ROW_VALVE1 + mCCToon.NumberOfSharedValves Then
                                                'Group
                                                nValvesOn = mColorChange.Cycle(mnOldCycle).Steps(nStep).GoutState.Value
                                                nValvesDC = mColorChange.Cycle(mnOldCycle).Steps(nStep).GoutDC.Value
                                            Else
                                                'Shared
                                                nValvesOn = mColorChange.Cycle(mnOldCycle).Steps(nStep).DoutState.Value
                                                nValvesDC = mColorChange.Cycle(mnOldCycle).Steps(nStep).DoutDC.Value
                                            End If
                                            Select Case nVal
                                                Case CheckState.Unchecked 'On, turn off
                                                    nValvesOn = nValvesOn And Not nBitMask
                                                    nValvesDC = nValvesDC And Not (nBitMask)
                                                Case CheckState.Indeterminate 'Off, turn to DC
                                                    nValvesOn = nValvesOn And Not (nBitMask)
                                                    nValvesDC = nValvesDC Or (nBitMask)
                                                Case CheckState.Checked 'DC, turn on
                                                    nValvesOn = nValvesOn Or (nBitMask)
                                                    nValvesDC = nValvesDC And Not (nBitMask)
                                            End Select
                                            If nRow >= mnGRID_ROW_VALVE1 + mCCToon.NumberOfSharedValves Then
                                                'Group
                                                mColorChange.Cycle(mnOldCycle).Steps(nStep).GoutState.Value = nValvesOn
                                                mColorChange.Cycle(mnOldCycle).Steps(nStep).GoutDC.Value = nValvesDC
                                            Else
                                                'Shared
                                                mColorChange.Cycle(mnOldCycle).Steps(nStep).DoutState.Value = nValvesOn
                                                mColorChange.Cycle(mnOldCycle).Steps(nStep).DoutDC.Value = nValvesDC
                                            End If
                                        End If
                                    End If
                                End If
                            Next
                        End If
                    Next
                End If
                subUpdateValveDisplay()
            Next
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subImportCycle", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
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
        ' 12/13/10  MSW     allow import from and export to csv
        '********************************************************************************************
        Dim sFile As String = String.Empty
        subEnableControls(False)
        Try
            Dim sTitleReq As String = msSCREEN_NAME
            'Dim sTableStart(cboCycle.Items.Count - 1) As String
            'For nCycIdx As Integer = 0 To cboCycle.Items.Count - 1
            '    sTableStart(nCycIdx) = cboCycle.Items(nCycIdx).ToString
            'Next
            Dim sTableStart(0) As String
            sTableStart(0) = String.Empty
            Dim sHeaders() As String = Nothing
            Dim oDTs() As DataTable = Nothing
            ImportExport.GetDTFromCSV(sTitleReq, sTableStart, sHeaders, oDTs)
            If (sHeaders IsNot Nothing) AndAlso (oDTs IsNot Nothing) Then
                'Cross reference import data to cycle list
                'Index CC cycle to import table
                Dim nCycleIndex(cboCycle.Items.Count - 1) As Integer
                For nLoop As Integer = 0 To nCycleIndex.Length - 1
                    nCycleIndex(nLoop) = -1
                Next
                'Index import table to CC cycle
                Dim nNumTables As Integer = sHeaders.Length
                If nNumTables > oDTs.Length Then
                    nNumTables = oDTs.Length
                End If
                'If there are more import tables than cycles on this robot, ignore the extras
                If nNumTables > nCycleIndex.Length Then
                    nNumTables = nCycleIndex.Length
                End If
                Dim nTableIndex(nNumTables - 1) As Integer
                For nLoop As Integer = 0 To nNumTables - 1
                    nTableIndex(nLoop) = -1
                Next
                Dim nTablesFound As Integer = 0
                Dim sImportTableNames(nNumTables - 1) As String
                For nDtIdx As Integer = 0 To nNumTables - 1
                    For nCycIdx As Integer = 0 To nCycleIndex.Length - 1
                        If InStr(sHeaders(nDtIdx), cboCycle.Items(nCycIdx).ToString) > 0 Then
                            'Cycle Name found
                            nCycleIndex(nCycIdx) = nDtIdx
                            nTableIndex(nDtIdx) = nCycIdx
                            nTablesFound = nTablesFound + 1
                            Exit For
                        End If
                    Next
                    If nTableIndex(nDtIdx) > -1 Then
                        sImportTableNames(nDtIdx) = cboCycle.Items(nTableIndex(nDtIdx)).ToString
                    Else
                        sImportTableNames(nDtIdx) = String.Format(gpsRM.GetString("psIMPORT_CYCLE"), nDtIdx.ToString)
                    End If
                    oDTs(nDtIdx).Columns(0).ColumnName = gpsRM.GetString("psITEM")
                    Dim nCol As Integer = oDTs(nDtIdx).Columns.Count - 1
                    Dim bDone As Boolean = False
                    Do Until bDone
                        For nRowNum As Integer = 0 To oDTs(nDtIdx).Rows.Count - 1
                            If oDTs(nDtIdx).Rows(nRowNum).Item(nCol).ToString.Trim <> String.Empty Then
                                bDone = True
                                Exit For
                            End If
                            If bDone = False Then
                                oDTs(nDtIdx).Columns.RemoveAt(nCol)
                                nCol = nCol - 1
                                If nCol = 0 Then
                                    bDone = True
                                End If
                            End If
                        Next
                    Loop
                Next
                frmImport.label(1) = String.Format(gpsRM.GetString("psSELECT_IMPORT"), cboRobot.Text, cboColor.Text)
                frmImport.SetFromList(sImportTableNames)
                frmImport.SetToList(cboCycle)
                frmImport.SetDTs(oDTs)
                If frmImport.ShowDialog() = Windows.Forms.DialogResult.OK Then
                    Dim nCycleFrom As Integer = frmImport.FromItem
                    Dim nCycleTo As Integer = frmImport.ToItem
                    If (nCycleFrom < 0) Or (nCycleTo < 0) Then
                        Dim bCyclesFrom() As Boolean = frmImport.FromItems
                        For nCycleNum As Integer = 1 To nNumTables
                            If bCyclesFrom(nCycleNum - 1) Then
                                subImportCycle(nCycleNum, oDTs(nCycleNum - 1))
                            End If
                        Next
                    Else
                        subImportCycle(nCycleTo + 1, oDTs(nCycleFrom))
                    End If
                End If
                'Else
                '    'No import found message
                '    Dim lRet As Response = MessageBox.Show(gpsRM.GetString("psVALID_IMPORT_DATA_NOT_FOUND_TEXT"), gpsRM.GetString("psVALID_IMPORT_DATA_NOT_FOUND_CAP"), _
                '               MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuImport_Click", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

        subDrawSteps()
        subEnableControls(True)

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
    Private Sub btnFunction_DropDownOpening(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFunction.DropDownOpening
        ''********************************************************************************************
        ''Description:  this was needed to enable the menus for some reason
        ''                  now handled in dostatusbar 2
        ''
        ''Parameters: none
        ''Returns:    none
        ''
        ''Modification history:
        ''
        '' Date      By      Reason
        ''********************************************************************************************
        Dim cachePriviledge As ePrivilege
        Dim bRem As Boolean = False
        Dim bAllow As Boolean = False


        If LoggedOnUser <> String.Empty Then

            'If moPrivilege.Privilege = String.Empty Then 'RJO 03/22/12
            '    cachePriviledge = ePrivilege.None
            'Else
            '    If moPrivilege.ActionAllowed Then
            '        cachePriviledge = mPassword.PrivilegeStringToEnum(moPrivilege.Privilege)
            '    Else
            '        cachePriviledge = ePrivilege.None
            '    End If
            'End If

            If moPassword.Privilege = ePrivilege.None Then
                cachePriviledge = ePrivilege.None
            Else
                If moPassword.ActionAllowed Then
                    cachePriviledge = moPassword.Privilege
                Else
                    cachePriviledge = ePrivilege.None
                End If
            End If

            Privilege = ePrivilege.Remote

            If Privilege = ePrivilege.Remote Then
                If colZones.PaintShopComputer = False Then
                    bAllow = True
                End If
            End If


            Privilege = cachePriviledge

        Else
            Privilege = ePrivilege.None
        End If

        If colZones.PaintShopComputer = False Then
            bRem = mbRemoteZone
        End If

        DoStatusBar2(stsStatus, LoggedOnUser, Privilege, bRem, bAllow)


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
            Privilege = ePrivilege.Copy ' extra for buttons etc.
            If Privilege <> ePrivilege.Copy Then
                'didn't have clearance
                Privilege = ePrivilege.Edit
            End If
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/22/12
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
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/22/12
        End If

    End Sub

#End Region


    Private Sub cboCycle_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboCycle.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Cycle Combo Changed 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If cboCycle.Text <> msOldCycle Then
            ' so we don't go off half painted
            Me.Refresh()
            subChangeCycle()
        End If
    End Sub
    Private Sub subChangeCycle(Optional ByVal nNewCycle As Integer = 0)
        '********************************************************************************************
        'Description:  New cycle selected
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*******************************************************************************************

        'just in case
        Try
            'HGB added Or (nNewCycle > cboCycle.Items.Count)
            If (nNewCycle = 0) Or (nNewCycle > cboCycle.Items.Count) Then
                If cboCycle.Text = String.Empty Then
                    Exit Sub
                Else
                    If cboCycle.Text = msOldCycle Then Exit Sub
                End If
                mnOldCycle = cboCycle.SelectedIndex + 1
            Else
                cboCycle.SelectedIndex = nNewCycle - 1
                mnOldCycle = nNewCycle
            End If
            msOldCycle = cboCycle.Text
            mnStep = 1
            subDrawSteps()
            subUpdateTotalTime()
            'Show pushout volume if it's supported
            If mColorChange.ShowPushoutVolume(mnOldCycle) Then
                subSetPushVol(mColorChange.PushOutVolume.Value)
                gpbPushVol.Visible = True
            Else
                gpbPushVol.Visible = False
            End If
            If mColorChange.Cycle(mnOldCycle).NumberOfSteps > 0 Then
                subSelectStep(1, False)
            End If
            If mbPrintBlocker = False Then
                subEnableControls(True)
            End If
            Status(True) = gpsRM.GetString("csREADY")
        Catch ex As Exception

        End Try
    End Sub
    Private Sub subUpdateTotalTime()
        '********************************************************************************************
        'Description:  update the total time display
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim fTime As Single = 0.0
        For nStep As Integer = 1 To mColorChange.Cycle(mnOldCycle).NumberOfSteps
            fTime = fTime + mColorChange.Cycle(mnOldCycle).Steps(nStep).Duration.Value
        Next
        lblTotalTime.Text = fTime.ToString("0.0") & gpsRM.GetString("psSEC")
    End Sub
    Private Sub subUpdateValveDisplay(Optional ByVal nstep As Integer = 0, _
                                      Optional ByVal bGrid As Boolean = True, _
                                      Optional ByVal bList As Boolean = True, _
                                      Optional ByVal bToon As Boolean = True)
        '********************************************************************************************
        'Description:  Display valves        '
        'Parameters: nStep - step number to update
        '       Booleand enable updating the grid display, listbox and cartoon
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbEventBlocker = True

        If nstep = 0 Then
            nstep = mnStep 'No step passed in, use current
        End If
        If bList Then
            lstValves.ClearSelected()
        End If
        If bGrid Then
            If mColorChange.Cycle(mnOldCycle).NumberOfSteps < nstep Then
                dgGridView.Rows(mnGRID_ROW_TIME).Cells(nstep - 1).Value = 0
                dgGridView.Rows(mnGRID_ROW_PRESET).Cells(nstep - 1).Value = 0
                dgGridView.Rows(mnGRID_ROW_ACTION).Cells(nstep - 1).Value = mColorChange.ActionName(0)
                dgGridView.Rows(mnGRID_ROW_ACTION).Cells(nstep - 1).ToolTipText = mColorChange.ActionName(0)
                dgGridView.Rows(mnGRID_ROW_EVENT).Cells(nstep - 1).Value = mColorChange.EventName(0)
                dgGridView.Rows(mnGRID_ROW_EVENT).Cells(nstep - 1).ToolTipText = mColorChange.EventName(0)
                dgGridView.Columns(nstep - 1).Width = 50
                dgGridView.Rows(mnGRID_ROW_TIME).Cells(nstep - 1).Style.ForeColor = Color.Black
                dgGridView.Rows(mnGRID_ROW_TIME).Cells(nstep - 1).Style.SelectionForeColor = Color.White
                dgGridView.Rows(mnGRID_ROW_PRESET).Cells(nstep - 1).Style.ForeColor = Color.Black
                dgGridView.Rows(mnGRID_ROW_PRESET).Cells(nstep - 1).Style.SelectionForeColor = Color.White
                dgGridView.Rows(mnGRID_ROW_ACTION).Cells(nstep - 1).Style.ForeColor = Color.Black
                dgGridView.Rows(mnGRID_ROW_ACTION).Cells(nstep - 1).Style.SelectionForeColor = Color.White
                dgGridView.Rows(mnGRID_ROW_EVENT).Cells(nstep - 1).Style.ForeColor = Color.Black
                dgGridView.Rows(mnGRID_ROW_EVENT).Cells(nstep - 1).Style.SelectionForeColor = Color.White
            Else
                dgGridView.Rows(mnGRID_ROW_TIME).Cells(nstep - 1).Value = mColorChange.Cycle(mnOldCycle).Steps(nstep).Duration.Value
                dgGridView.Rows(mnGRID_ROW_PRESET).Cells(nstep - 1).Value = mColorChange.Cycle(mnOldCycle).Steps(nstep).Preset.Value
                dgGridView.Rows(mnGRID_ROW_ACTION).Cells(nstep - 1).Value = mColorChange.ActionName(mColorChange.Cycle(mnOldCycle).Steps(nstep).StepAction.Value)
                dgGridView.Rows(mnGRID_ROW_ACTION).Cells(nstep - 1).ToolTipText = mColorChange.ActionName(mColorChange.Cycle(mnOldCycle).Steps(nstep).StepAction.Value)
                dgGridView.Rows(mnGRID_ROW_EVENT).Cells(nstep - 1).Value = mColorChange.EventName(mColorChange.Cycle(mnOldCycle).Steps(nstep).StepEvent.Value)
                dgGridView.Rows(mnGRID_ROW_EVENT).Cells(nstep - 1).ToolTipText = mColorChange.EventName(mColorChange.Cycle(mnOldCycle).Steps(nstep).StepEvent.Value)
                If rdoTime.Checked Then
                    dgGridView.Columns(nstep - 1).Width = CType(mColorChange.Cycle(mnOldCycle).Steps(nstep).Duration.Value * mnGridPtsPerSec, Integer)
                End If
                If mColorChange.Cycle(mnOldCycle).Steps(nstep).Duration.Changed Then
                    dgGridView.Rows(mnGRID_ROW_TIME).Cells(nstep - 1).Style.ForeColor = Color.Red
                    dgGridView.Rows(mnGRID_ROW_TIME).Cells(nstep - 1).Style.SelectionForeColor = Color.Red
                Else
                    dgGridView.Rows(mnGRID_ROW_TIME).Cells(nstep - 1).Style.ForeColor = Color.Black
                    dgGridView.Rows(mnGRID_ROW_TIME).Cells(nstep - 1).Style.SelectionForeColor = Color.White
                End If
                If mColorChange.Cycle(mnOldCycle).Steps(nstep).Preset.Changed Then
                    dgGridView.Rows(mnGRID_ROW_PRESET).Cells(nstep - 1).Style.ForeColor = Color.Red
                    dgGridView.Rows(mnGRID_ROW_PRESET).Cells(nstep - 1).Style.SelectionForeColor = Color.Red
                Else
                    dgGridView.Rows(mnGRID_ROW_PRESET).Cells(nstep - 1).Style.ForeColor = Color.Black
                    dgGridView.Rows(mnGRID_ROW_PRESET).Cells(nstep - 1).Style.SelectionForeColor = Color.White
                End If
                If mColorChange.Cycle(mnOldCycle).Steps(nstep).StepAction.Changed Then
                    dgGridView.Rows(mnGRID_ROW_ACTION).Cells(nstep - 1).Style.ForeColor = Color.Red
                    dgGridView.Rows(mnGRID_ROW_ACTION).Cells(nstep - 1).Style.SelectionForeColor = Color.Red
                Else
                    dgGridView.Rows(mnGRID_ROW_ACTION).Cells(nstep - 1).Style.ForeColor = Color.Black
                    dgGridView.Rows(mnGRID_ROW_ACTION).Cells(nstep - 1).Style.SelectionForeColor = Color.White
                End If
                If mColorChange.Cycle(mnOldCycle).Steps(nstep).StepEvent.Changed Then
                    dgGridView.Rows(mnGRID_ROW_EVENT).Cells(nstep - 1).Style.ForeColor = Color.Red
                    dgGridView.Rows(mnGRID_ROW_EVENT).Cells(nstep - 1).Style.SelectionForeColor = Color.Red
                Else
                    dgGridView.Rows(mnGRID_ROW_EVENT).Cells(nstep - 1).Style.ForeColor = Color.Black
                    dgGridView.Rows(mnGRID_ROW_EVENT).Cells(nstep - 1).Style.SelectionForeColor = Color.White
                End If
            End If
        End If
        'Get the shared valve states for this step
        Dim nValveStates As Integer = 0
        Dim nValveDC As Integer = 0
        If mColorChange.Cycle(mnOldCycle).NumberOfSteps >= nstep Then
            nValveStates = mColorChange.Cycle(mnOldCycle).Steps(nstep).DoutState.Value
            nValveDC = mColorChange.Cycle(mnOldCycle).Steps(nstep).DoutDC.Value
        End If
        Dim nBitMask As Integer
        Dim lCheckState As CheckState
        'check each shared valve data for this step
        For nValve As Integer = 0 To mCCToon.NumberOfSharedValves - 1
            nBitMask = CType(2 ^ (nValve), Integer)
            If bList Or bGrid Then
                If ((nValveStates And nBitMask) > 0) Then
                    'select each valve that's on in the step data
                    lCheckState = CheckState.Checked
                ElseIf ((nValveDC And nBitMask) > 0) Then
                    lCheckState = CheckState.Indeterminate
                Else
                    lCheckState = CheckState.Unchecked
                End If
                If bList Then
                    lstValves.SetItemCheckState(nValve, lCheckState)
                End If
                If bGrid Then
                    Select Case lCheckState
                        Case CheckState.Checked
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve).Cells(nstep - 1).Value = 1
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve).Cells(nstep - 1).Style.ForeColor = mcSharedValveColors(nValve)
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve).Cells(nstep - 1).Style.BackColor = mcSharedValveColors(nValve)
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve).Cells(nstep - 1).Style.SelectionBackColor = mcSharedValveColors(nValve)
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve).Cells(nstep - 1).Style.SelectionForeColor = mcSharedValveColors(nValve)
                        Case CheckState.Indeterminate
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve).Cells(nstep - 1).Value = 2
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve).Cells(nstep - 1).Style.ForeColor = Color.Gray
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve).Cells(nstep - 1).Style.BackColor = Color.Gray
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve).Cells(nstep - 1).Style.SelectionForeColor = Color.Gray
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve).Cells(nstep - 1).Style.SelectionBackColor = Color.Gray
                        Case CheckState.Unchecked
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve).Cells(nstep - 1).Value = 0
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve).Cells(nstep - 1).Style.ForeColor = Color.White
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve).Cells(nstep - 1).Style.BackColor = Color.White
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve).Cells(nstep - 1).Style.SelectionForeColor = Color.White
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve).Cells(nstep - 1).Style.SelectionBackColor = Color.White
                    End Select
                End If
            End If
        Next
        If bList Then
            If (mColorChange.Cycle(mnOldCycle).NumberOfSteps >= nstep) AndAlso _
                (mColorChange.Cycle(mnOldCycle).Steps(nstep).DoutDC.Changed Or _
                 mColorChange.Cycle(mnOldCycle).Steps(nstep).DoutState.Changed Or _
                 mColorChange.Cycle(mnOldCycle).Steps(nstep).GoutDC.Changed Or _
                 mColorChange.Cycle(mnOldCycle).Steps(nstep).GoutState.Changed) Then
                lstValves.ForeColor = Color.Red
            Else
                lstValves.ForeColor = Color.Black
            End If
        End If
        If bToon Then
            'Update the cartoon's shared valves
            mCCToon.SharedValveStates = nValveStates Or nValveDC
        End If
        'Get the goup valve data for this step
        nValveStates = 0
        nValveDC = 0
        If mColorChange.Cycle(mnOldCycle).NumberOfSteps >= nstep Then
            nValveStates = mColorChange.Cycle(mnOldCycle).Steps(nstep).GoutState.Value
            nValveDC = mColorChange.Cycle(mnOldCycle).Steps(nstep).GoutDC.Value
        End If

        For nValve As Integer = 0 To mCCToon.NumberOfGroupValves - 1
            nBitMask = CType(2 ^ (nValve), Integer)
            If bList Or bGrid Then
                If ((nValveStates And nBitMask) > 0) Then
                    'select each valve that's on in the step data
                    lCheckState = CheckState.Checked
                ElseIf ((nValveDC And nBitMask) > 0) Then
                    lCheckState = CheckState.Indeterminate
                Else
                    lCheckState = CheckState.Unchecked
                End If
                If bList Then
                    lstValves.SetItemCheckState(nValve + mCCToon.NumberOfSharedValves, lCheckState)
                End If
                If bGrid Then
                    Select Case lCheckState
                        Case CheckState.Checked
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve + mCCToon.NumberOfSharedValves).Cells(nstep - 1).Value = 1
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve + mCCToon.NumberOfSharedValves).Cells(nstep - 1).Style.ForeColor = mcGroupValveColors(nValve)
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve + mCCToon.NumberOfSharedValves).Cells(nstep - 1).Style.BackColor = mcGroupValveColors(nValve)
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve + mCCToon.NumberOfSharedValves).Cells(nstep - 1).Style.SelectionBackColor = mcGroupValveColors(nValve)
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve + mCCToon.NumberOfSharedValves).Cells(nstep - 1).Style.SelectionForeColor = mcGroupValveColors(nValve)
                        Case CheckState.Indeterminate
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve + mCCToon.NumberOfSharedValves).Cells(nstep - 1).Value = 2
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve + mCCToon.NumberOfSharedValves).Cells(nstep - 1).Style.ForeColor = Color.Gray
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve + mCCToon.NumberOfSharedValves).Cells(nstep - 1).Style.BackColor = Color.Gray
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve + mCCToon.NumberOfSharedValves).Cells(nstep - 1).Style.SelectionForeColor = Color.Gray
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve + mCCToon.NumberOfSharedValves).Cells(nstep - 1).Style.SelectionBackColor = Color.Gray
                        Case CheckState.Unchecked
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve + mCCToon.NumberOfSharedValves).Cells(nstep - 1).Value = 0
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve + mCCToon.NumberOfSharedValves).Cells(nstep - 1).Style.ForeColor = Color.White
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve + mCCToon.NumberOfSharedValves).Cells(nstep - 1).Style.BackColor = Color.White
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve + mCCToon.NumberOfSharedValves).Cells(nstep - 1).Style.SelectionForeColor = Color.White
                            dgGridView.Rows(mnGRID_ROW_VALVE1 + nValve + mCCToon.NumberOfSharedValves).Cells(nstep - 1).Style.SelectionBackColor = Color.White
                    End Select
                End If
            End If
        Next
        If bToon Then
            'Update the cartoon's group valves
            mCCToon.GroupValveStates = nValveStates Or nValveDC
            'redraw
            mCCToon.subUpdateValveCartoon()
        End If
        If bGrid Then
            dgGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders)
        End If
        mbEventBlocker = False

    End Sub
    Private Sub subSelectStep(ByVal nStep As Integer, Optional ByVal bIncrement As Boolean = False)
        '********************************************************************************************
        'Description:  New step selected
        '
        'Parameters: nStep - step number if bIncrement = false, 
        '       if bIncrement = true, step +-1 = increment, +-2 = first or last
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim nMax As Integer = mColorChange.Cycle(mnOldCycle).NumberOfSteps
            Dim nMin As Integer = 1
            If nMax = 0 Then
                Exit Sub
            End If
            mbEventBlocker = True
            If bIncrement Then
                Select Case nStep
                    Case 1, -1
                        mnStep = mnStep + nStep
                        If mnStep > nMax Then
                            mnStep = nMax
                            If mnStep < nMin Then
                                mnStep = nMin
                            End If
                        End If
                    Case 2
                        mnStep = nMax
                    Case -2
                        mnStep = nMin

                End Select
            Else
                If nStep >= nMin And nStep <= nMax Then
                    mnStep = nStep
                End If
            End If
            'Update step data
            If mnStep < lstStepActEvt.Items.Count Then
                lstStepActEvt.SelectedIndex = mnStep - 1
            End If
            subSetStepPreset(mnStep, mColorChange.Cycle(mnOldCycle).Steps(mnStep).Preset.Value)
            subSetStepTime(mnStep, mColorChange.Cycle(mnOldCycle).Steps(mnStep).Duration.Value)
            subUpdateValveDisplay(mnStep, True, True, True)
            'Show pushout volume if it's supported
            If mColorChange.ShowPushoutVolume(mnOldCycle) Then
                subSetPushVol(mColorChange.PushOutVolume.Value)
                gpbPushVol.Visible = True
            Else
                gpbPushVol.Visible = False
            End If
            dgGridView.Columns(mnStep - 1).Selected = True
            mbEventBlocker = False
        Catch ex As Exception

        End Try
    End Sub

    Private Sub lstStepActEvt_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lstStepActEvt.MouseDown
        '********************************************************************************************
        'Description:  catch a right click on the step list when the menu opens
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'The bastard won't just give me the index for where the mouse is down at, have to calculate, and account for the scrollbar
        If e.Button <> Windows.Forms.MouseButtons.Right Then Exit Sub
        mnPopupMenuStep = (e.Location.Y \ lstStepActEvt.ItemHeight) + 1
        If mnPopupMenuStep >= 1 And mnPopupMenuStep <= lstStepActEvt.Items.Count Then
            'Account for the scrollbar
            Dim Rect As Rectangle = lstStepActEvt.GetItemRectangle(mnPopupMenuStep - 1)
            If e.Location.Y > Rect.Bottom Then
                mnPopupMenuStep = mnPopupMenuStep + ((e.Location.Y - Rect.Bottom) \ lstStepActEvt.ItemHeight) + 1
            End If
        Else
            'In whitespace underneath? take the last step
            If mnPopupMenuStep > mColorChange.Cycle(mnOldCycle).NumberOfSteps - 1 Then
                mnPopupMenuStep = mColorChange.Cycle(mnOldCycle).NumberOfSteps
            End If
            If mnPopupMenuStep < 0 Then
                mnPopupMenuStep = 1
            End If
        End If
    End Sub

    Private Sub lstStepActEvt_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstStepActEvt.SelectedIndexChanged
        '********************************************************************************************
        'Description:  selected step changed
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If lstStepActEvt.SelectedIndex >= 0 Then
            subSelectStep(lstStepActEvt.SelectedIndex + 1)
        End If
    End Sub
    Private Sub subSetStepPreset(ByVal nStep As Integer, ByVal nPreset As Integer)
        '********************************************************************************************
        'Description:  Preset changed
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mnOldCycle = 0 Then Exit Sub
        'HGB avoid errors when no steps have been defined
        If mColorChange.Cycle(mnOldCycle).NumberOfSteps = 0 Then Exit Sub
        Dim bEnableControls As Boolean = Not (mColorChange.Changed)
        mbEventBlocker = True
        Dim lRet As DialogResult
        If nPreset > nudPreset.Maximum Then
            lRet = MessageBox.Show(gcsRM.GetString("csVAL_ABOVE_MAX"), gcsRM.GetString("csERROR"), _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            nPreset = mColorChange.Cycle(mnOldCycle).Steps(nStep).Preset.Value
        End If
        If nPreset < nudPreset.Minimum Then
            lRet = MessageBox.Show(gcsRM.GetString("csVAL_BELOW_MIN"), gcsRM.GetString("csERROR"), _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            nPreset = mColorChange.Cycle(mnOldCycle).Steps(nStep).Preset.Value
        End If
        mColorChange.Cycle(mnOldCycle).Steps(nStep).Preset.Value = nPreset
        nudPreset.Value = nPreset
        dgGridView.Rows(mnGRID_ROW_PRESET).Cells(nStep - 1).Value = nPreset
        If mColorChange.Cycle(mnOldCycle).Steps(mnStep).Preset.Changed Then
            dgGridView.Rows(mnGRID_ROW_PRESET).Cells(nStep - 1).Style.ForeColor = Color.Red
            dgGridView.Rows(mnGRID_ROW_PRESET).Cells(nStep - 1).Style.SelectionForeColor = Color.Red
            If nStep = mnStep Then
                nudPreset.ForeColor = Color.Red
            End If
        Else
            dgGridView.Rows(mnGRID_ROW_PRESET).Cells(mnStep - 1).Style.ForeColor = Color.Black
            dgGridView.Rows(mnGRID_ROW_PRESET).Cells(mnStep - 1).Style.SelectionForeColor = Color.White
            If nStep = mnStep Then
                nudPreset.ForeColor = Color.Black
            End If
        End If
        'Update preset display
        If nStep = mnStep Then
            If mbPresetsLoaded And mColorChange.Cycle(mnOldCycle).Steps(mnStep).Preset.Value > 0 Then
                For nParm As Integer = 0 To mnNumParms - 1
                    Dim lblTmp As Label = DirectCast(gpbPreset.Controls("lblParm" & (nParm + 1).ToString("00")), Label)
                    lblTmp.Text = mCCPresets.Item(mColorChange.Cycle(mnOldCycle).Steps(mnStep).Preset.Value).Param(nParm).Value.ToString & _
                        "(" & mParmDetail(nParm).sUnits & ")"
                    lblTmp.Visible = True
                Next
            Else
                For nParm As Integer = 1 To mnNumParms
                    Dim lblTmp As Label = DirectCast(gpbPreset.Controls("lblParm" & nParm.ToString("00")), Label)
                    lblTmp.Text = String.Empty
                Next
            End If
        End If
        If bEnableControls Then
            subEnableControls(True)
        End If
        mbEventBlocker = False
    End Sub
    Private Sub subSetPushVol(ByVal nVol As Single)
        '********************************************************************************************
        'Description:  pushout volume changed
        '
        'Parameters: none

        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mnOldCycle = 0 Then Exit Sub
        mbEventBlocker = True
        Dim lRet As DialogResult
        Dim bEnableControls As Boolean = Not (mColorChange.Changed)
        If nVol > nudPushVol.Maximum Then
            lRet = MessageBox.Show(gcsRM.GetString("csVAL_ABOVE_MAX"), gcsRM.GetString("csERROR"), _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            nVol = mColorChange.PushOutVolume.Value
        End If
        If nVol < nudPushVol.Minimum Then
            lRet = MessageBox.Show(gcsRM.GetString("csVAL_BELOW_MIN"), gcsRM.GetString("csERROR"), _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            nVol = mColorChange.PushOutVolume.Value
        End If
        mColorChange.PushOutVolume.Value = nVol
        If mColorChange.PushOutVolume.Changed Then
            nudPushVol.ForeColor = Color.Red
        Else
            nudPushVol.ForeColor = Color.Black
        End If
        nudPushVol.Value = CType(nVol, Decimal)
        If bEnableControls Then
            subEnableControls(True)
        End If
        mbEventBlocker = False
    End Sub
    Private Sub nudPushVol_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudPushVol.ValueChanged
        '********************************************************************************************
        'Description:  pushout volume changed
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'The object limits the range for us, so just set the data and enable the save and undo buttons
        If mnOldCycle = 0 Then Exit Sub
        If mbEventBlocker Then Exit Sub
        subSetPushVol(CType(nudPushVol.Value, Single))
    End Sub

    Private Sub subSetStepTime(ByVal nStep As Integer, ByVal nTime As Single)
        '********************************************************************************************
        'Description:  Step time changed
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mnOldCycle = 0 Then Exit Sub
        'HGB avoid errors when no steps have been defined
        If mColorChange.Cycle(mnOldCycle).NumberOfSteps = 0 Then Exit Sub
        Dim bEnableControls As Boolean = Not (mColorChange.Changed)
        mbEventBlocker = True
        Dim lRet As DialogResult
        If nTime > nudStepTime.Maximum Then
            lRet = MessageBox.Show(gcsRM.GetString("csVAL_ABOVE_MAX"), gcsRM.GetString("csERROR"), _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            nTime = mColorChange.Cycle(mnOldCycle).Steps(nStep).Duration.Value
        End If
        If nTime < nudStepTime.Minimum Then
            lRet = MessageBox.Show(gcsRM.GetString("csVAL_BELOW_MIN"), gcsRM.GetString("csERROR"), _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            nTime = mColorChange.Cycle(mnOldCycle).Steps(nStep).Duration.Value
        End If
        mColorChange.Cycle(mnOldCycle).Steps(nStep).Duration.Value = nTime
        nudStepTime.Value = CType(nTime, Decimal)
        dgGridView.Rows(mnGRID_ROW_TIME).Cells(nStep - 1).Value = nTime
        If mColorChange.Cycle(mnOldCycle).Steps(nStep).Duration.Changed Then
            dgGridView.Rows(mnGRID_ROW_TIME).Cells(nStep - 1).Style.ForeColor = Color.Red
            dgGridView.Rows(mnGRID_ROW_TIME).Cells(nStep - 1).Style.SelectionForeColor = Color.Red
            nudStepTime.ForeColor = Color.Red
        Else
            dgGridView.Rows(mnGRID_ROW_TIME).Cells(nStep - 1).Style.ForeColor = Color.Black
            dgGridView.Rows(mnGRID_ROW_TIME).Cells(nStep - 1).Style.SelectionForeColor = Color.White
            nudStepTime.ForeColor = Color.Black
        End If
        If rdoTime.Checked Then
            dgGridView.Columns(nStep - 1).Width = CType(nTime * mnGridPtsPerSec, Integer)
        End If
        subUpdateTotalTime()
        If bEnableControls Then
            subEnableControls(True)
        End If

        mbEventBlocker = False
    End Sub
    Private Sub nudStepTime_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudStepTime.ValueChanged
        '********************************************************************************************
        'Description:  Step time changed
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/19/11  MSW     This was updating the preset instead
        '********************************************************************************************
        'The object limits the range for us, so just set the data and enable the save and undo buttons
        If mbEventBlocker Then Exit Sub
        If mnOldCycle = 0 Then Exit Sub
        subSetStepTime(mnStep, CType(nudStepTime.Value, Single))
    End Sub
    Private Sub nudPreset_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudPreset.ValueChanged
        '********************************************************************************************
        'Description:  Preset changed
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'The object limits the range for us, so just set the data and enable the save and undo buttons
        If mnOldCycle = 0 Then Exit Sub
        If mbEventBlocker Then Exit Sub
        subSetStepPreset(mnStep, CType(nudPreset.Value, Integer))
    End Sub

    Private Sub mnuInsert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuInsert.Click
        '********************************************************************************************
        'Description:  Insert step selected
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Insert wants the step number to insert after
        mbEventBlocker = True
        subEnableControls(False)
        Dim lRet As DialogResult
        lRet = MessageBox.Show(gpsRM.GetString("psINS_QUES_A") & (mnPopupMenuStep).ToString & _
                               gpsRM.GetString("psSTEP_QUES_B"), gpsRM.GetString("psINSERT"), _
                            MessageBoxButtons.YesNo, _
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

        If lRet = Response.Yes Then
            mColorChange.Cycle(mnOldCycle).InsertStep(mnPopupMenuStep - 1)
            subDrawSteps()
        End If
        subEnableControls(True)
        mbEventBlocker = False
    End Sub
    Private Sub mnuAppend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuAppend.Click
        '********************************************************************************************
        'Description:  Append step selected
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Insert wants the step number to insert after
        mbEventBlocker = True
        subEnableControls(False)
        Dim lRet As DialogResult
        lRet = MessageBox.Show(gpsRM.GetString("psAPP_QUES_A") & (mnPopupMenuStep).ToString & _
                               gpsRM.GetString("psSTEP_QUES_B"), gpsRM.GetString("psAPPEND"), _
                            MessageBoxButtons.YesNo, _
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

        If lRet = Response.Yes Then
            If mColorChange.Cycle(mnOldCycle).NumberOfSteps = 0 Then
                mColorChange.Cycle(mnOldCycle).InsertStep(0)
            Else
                mColorChange.Cycle(mnOldCycle).InsertStep(mnPopupMenuStep)
            End If
            subDrawSteps()
        End If
            subEnableControls(True)
            mbEventBlocker = False
    End Sub
    Private Sub mnuDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuDelete.Click
        '********************************************************************************************
        'Description:  delete a step was selected
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbEventBlocker = True
        subEnableControls(False)
        Dim lRet As DialogResult
        lRet = MessageBox.Show(gpsRM.GetString("psDEL_QUES_A") & (mnPopupMenuStep).ToString & _
                               gpsRM.GetString("psSTEP_QUES_B"), gpsRM.GetString("psDELETE"), _
                            MessageBoxButtons.YesNo, _
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

        If lRet = Response.Yes Then
            mColorChange.Cycle(mnOldCycle).RemoveStep(mnPopupMenuStep)
            subDrawSteps()
        End If
        subEnableControls(True)
        mbEventBlocker = False
    End Sub
    Private Sub mnuAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
            mnuAction0.Click, mnuAction1.Click, mnuAction2.Click, mnuAction3.Click, mnuAction4.Click, _
            mnuAction5.Click, mnuAction6.Click, mnuAction7.Click, mnuAction8.Click, _
            mnuEvent0.Click, mnuEvent1.Click, mnuEvent2.Click, mnuEvent3.Click, mnuEvent4.Click, mnuEvent5.Click, _
            mnuEvent6.Click, mnuEvent7.Click, mnuEvent8.Click
        '********************************************************************************************
        'Description:  Change the action selected for step
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbEventBlocker = True
        subEnableControls(False)



        Dim oMnu As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim nIndex As Integer = CInt(oMnu.Name.Substring(oMnu.Name.Length - 1))
        If oMnu.Name.Substring(0, oMnu.Name.Length - 1) = "mnuAction" Then
            mColorChange.Cycle(mnOldCycle).Steps(mnPopupMenuStep).StepAction.Value = nIndex
        Else
            mColorChange.Cycle(mnOldCycle).Steps(mnPopupMenuStep).StepEvent.Value = nIndex
        End If
        lstStepActEvt.Items.RemoveAt(mnPopupMenuStep - 1)
        lstStepActEvt.Items.Insert(mnPopupMenuStep - 1, _
                ActionEventLine((mnPopupMenuStep), mColorChange.Cycle(mnOldCycle).Steps(mnPopupMenuStep).StepAction.Value, _
                                                    mColorChange.Cycle(mnOldCycle).Steps(mnPopupMenuStep).StepEvent.Value))
        Dim bChange As Boolean = False
        If mColorChange.Cycle(mnOldCycle).Steps(mnPopupMenuStep).StepAction.Changed Then
            dgGridView.Rows(mnGRID_ROW_ACTION).Cells(mnPopupMenuStep - 1).Style.ForeColor = Color.Red
            dgGridView.Rows(mnGRID_ROW_ACTION).Cells(mnPopupMenuStep - 1).Style.SelectionForeColor = Color.Red
            bChange = True
        Else
            dgGridView.Rows(mnGRID_ROW_ACTION).Cells(mnPopupMenuStep - 1).Style.ForeColor = Color.Black
            dgGridView.Rows(mnGRID_ROW_ACTION).Cells(mnPopupMenuStep - 1).Style.SelectionForeColor = Color.White
        End If
        If mColorChange.Cycle(mnOldCycle).Steps(mnPopupMenuStep).StepEvent.Changed Then
            dgGridView.Rows(mnGRID_ROW_EVENT).Cells(mnPopupMenuStep - 1).Style.ForeColor = Color.Red
            dgGridView.Rows(mnGRID_ROW_EVENT).Cells(mnPopupMenuStep - 1).Style.SelectionForeColor = Color.Red
            bChange = True
        Else
            dgGridView.Rows(mnGRID_ROW_EVENT).Cells(mnPopupMenuStep - 1).Style.ForeColor = Color.Black
            dgGridView.Rows(mnGRID_ROW_EVENT).Cells(mnPopupMenuStep - 1).Style.SelectionForeColor = Color.White
        End If
        If Not bChange Then
            For nStep As Integer = 1 To mColorChange.Cycle(mnOldCycle).NumberOfSteps
                If mColorChange.Cycle(mnOldCycle).Steps(nStep).StepAction.Changed Or _
                    mColorChange.Cycle(mnOldCycle).Steps(nStep).StepEvent.Changed Then
                    bChange = True
                    Exit For
                End If
            Next
        End If
        If bChange Then
            lstStepActEvt.ForeColor = Color.Red
        Else
            lstStepActEvt.ForeColor = Color.Black
        End If
        dgGridView.Rows(mnGRID_ROW_ACTION).Cells(mnPopupMenuStep - 1).Value = mColorChange.ActionName(mColorChange.Cycle(mnOldCycle).Steps(mnPopupMenuStep).StepAction.Value)
        dgGridView.Rows(mnGRID_ROW_EVENT).Cells(mnPopupMenuStep - 1).Value = mColorChange.EventName(mColorChange.Cycle(mnOldCycle).Steps(mnPopupMenuStep).StepEvent.Value)
        dgGridView.Rows(mnGRID_ROW_ACTION).Cells(mnPopupMenuStep - 1).ToolTipText = mColorChange.ActionName(mColorChange.Cycle(mnOldCycle).Steps(mnPopupMenuStep).StepAction.Value)
        dgGridView.Rows(mnGRID_ROW_EVENT).Cells(mnPopupMenuStep - 1).ToolTipText = mColorChange.EventName(mColorChange.Cycle(mnOldCycle).Steps(mnPopupMenuStep).StepEvent.Value)

        subEnableControls(True)
        mbEventBlocker = False
    End Sub



    Private Sub lstValves_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstValves.SelectedIndexChanged
        '********************************************************************************************
        'Description:  valve list changed
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEventBlocker Then Exit Sub
        If Privilege = ePrivilege.None Then
            'Reset the display if they click on it without a password
            'I couldn't find a way to let them scroll without allowing edits
            subUpdateValveDisplay()
        Else
            'Update the data
            'Selected item in the list
            Dim nValve As Integer = lstValves.SelectedIndex
            'See if it's a group or shared valve
            Dim bShared As Boolean = (nValve < mCCToon.NumberOfSharedValves)
            If Not (bShared) Then
                nValve = nValve - mCCToon.NumberOfSharedValves
            End If
            subToggleValve(nValve, bShared)
        End If
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
            Dim dNewMessage As New NewMessage_Callback(AddressOf oIPC_NewMessage)
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

    Private Sub subToggleValve(ByVal nValve As Integer, ByVal bShared As Boolean)
        '********************************************************************************************
        'Description:  click on a valve in the list, grid or cartoon
        '               cycle through off, DC, on
        'Parameters: nValve - valve number start with 0 for both shared or group valves
        '               bshared - true for shared valves
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbEventBlocker = True
        Dim nBitMask As Integer
        Dim nValveStates As Integer
        Dim nValveDC As Integer
        Dim lCheckState As CheckState
        Dim nValveOffset As Integer = 0
        Dim ColorOn As Color
        Dim bEnableControls As Boolean = Not (mColorChange.Changed)
        nBitMask = CType(2 ^ (nValve), Integer)
        If bShared Then
            'current shared valve states
            nValveStates = mColorChange.Cycle(mnOldCycle).Steps(mnStep).DoutState.Value
            nValveDC = mColorChange.Cycle(mnOldCycle).Steps(mnStep).DoutDC.Value
            nValveOffset = 0
            ColorOn = mcSharedValveColors(nValve)
        Else
            'current group valve states
            nValveStates = mColorChange.Cycle(mnOldCycle).Steps(mnStep).GoutState.Value
            nValveDC = mColorChange.Cycle(mnOldCycle).Steps(mnStep).GoutDC.Value
            nValveOffset = mCCToon.NumberOfSharedValves
            ColorOn = mcGroupValveColors(nValve)
        End If

        'reading the check state doesn't seem to work, so set it based on the stored valve states
        If (nValveDC And nBitMask) = nBitMask Then
            lCheckState = CheckState.Checked 'DC, change to on
        ElseIf (nValveStates And nBitMask) = nBitMask Then
            lCheckState = CheckState.Unchecked 'On, change to off
        Else
            lCheckState = CheckState.Indeterminate 'off , change to DC
        End If
        If Not (bShared) Then
            nValve = nValve + mCCToon.NumberOfSharedValves
        End If
        'Set the new valve states
        Select Case lCheckState
            Case CheckState.Checked 'On, turn off
                nValveStates = nValveStates And Not nBitMask
                nValveDC = nValveDC And Not (nBitMask)
            Case CheckState.Unchecked 'Off, turn to DC
                nValveStates = nValveStates And Not (nBitMask)
                nValveDC = nValveDC Or (nBitMask)
            Case CheckState.Indeterminate 'DC, turn on
                nValveStates = nValveStates Or (nBitMask)
                nValveDC = nValveDC And Not (nBitMask)
        End Select
        'Combined states for validation
        Dim nsharedStates As Integer = mColorChange.Cycle(mnOldCycle).Steps(mnStep).DoutState.Value Or _
        mColorChange.Cycle(mnOldCycle).Steps(mnStep).DoutDC.Value
        Dim nGroupStates As Integer = mColorChange.Cycle(mnOldCycle).Steps(mnStep).GoutState.Value Or _
        mColorChange.Cycle(mnOldCycle).Steps(mnStep).GoutDC.Value
        If bShared Then
            nsharedStates = nValveDC Or nValveStates
        Else
            nGroupStates = nValveDC Or nValveStates
        End If
        If (mCCToon.ValidateValveSelection(nsharedStates, nGroupStates) = True) Then
            'OK
            If bShared Then
                'new shared valve states
                mColorChange.Cycle(mnOldCycle).Steps(mnStep).DoutState.Value = nValveStates
                mColorChange.Cycle(mnOldCycle).Steps(mnStep).DoutDC.Value = nValveDC
            Else
                'new group valve states
                mColorChange.Cycle(mnOldCycle).Steps(mnStep).GoutState.Value = nValveStates
                mColorChange.Cycle(mnOldCycle).Steps(mnStep).GoutDC.Value = nValveDC
            End If
        Else
            'Invalid valve state
            MessageBox.Show(gcsRM.GetString("csINVALID_VALVESTATE"), gcsRM.GetString("csINVALID_DATA"), _
                MessageBoxButtons.OK, MessageBoxIcon.Warning)

        End If
        'Update the display
        subUpdateValveDisplay(mnStep, True, True, True)
        mbEventBlocker = False
        If bEnableControls Then
            subEnableControls(True)
        End If
    End Sub

    Private Sub SharedValveClicked(ByVal nValve As Integer, ByVal bCurState As Boolean) Handles mCCToon.SharedValveClicked
        '********************************************************************************************
        'Description:  color change cartoon click handlers
        '   Get the click event from each color change type
        '    - each cc type cartoon needs to be added to the "Handles" statement and the case statements
        '
        'Parameters: valve number, current state
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subToggleValve(nValve, True)
    End Sub

    Private Sub GroupValveClicked(ByVal nValve As Integer, ByVal bCurState As Boolean) Handles mCCToon.GroupValveClicked
        '********************************************************************************************
        'Description:  color change cartoon click handlers
        '   Get the click event from each color change type
        '    - each cc type cartoon needs to be added to the "Handles" statement and the case statements
        '
        'Parameters: valve number, current state
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mnStep > 0 And mnOldCycle > 0 Then
            subToggleValve(nValve, False)
        End If
    End Sub

    Private Sub gpbGridView_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles gpbGridView.MouseDoubleClick
        ''********************************************************************************************
        ''Description:  Double-click on gridview frame - resize
        ''
        ''Parameters: mouse event args
        ''Returns:    none
        ''
        ''Modification history:
        ''
        '' Date      By      Reason
        ''********************************************************************************************
        'If tblpData.RowStyles(0).Height < 10 Then
        '    tblpData.RowStyles(0).Height = mnSaveRowHeight
        'Else
        '    mnSaveRowHeight = tblpData.RowStyles(0).Height
        '    tblpData.RowStyles(0).Height = 0
        'End If
        'mnGridDragActive = 0
    End Sub

    Private Sub gpbGridView_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles gpbGridView.MouseDown
        '********************************************************************************************
        'Description:  Drag the grid frame for resize
        '
        'Parameters: mouse event args
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mnGridDragStartGripPos = tblpData.RowStyles(0).Height
        mnGridDragStartMouse = e.Location.Y
        mnGridDragActive = 1
    End Sub

    Private Sub gpbGridView_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles gpbGridView.MouseMove
        '********************************************************************************************
        'Description:  Drag the grid frame for resize
        '
        'Parameters: mouse event args
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nDiff As Single
        If mnGridDragActive = 1 Then
            nDiff = mnGridDragStartMouse - e.Location.Y
            If (nDiff > 10) Or (nDiff < -10) Then
                mnGridDragActive = 2
            End If
        End If
        If mnGridDragActive = 2 Then
            Dim nNewPos As Single = mnGridDragStartGripPos - (mnGridDragStartMouse - e.Location.Y)
            If nNewPos < 0 Then
                nNewPos = 0
            End If
            If (nNewPos > (tblpData.Height * 0.9)) Then
                nNewPos = CType((tblpData.Height * 0.9), Single)
            End If
            nDiff = nNewPos - tblpData.RowStyles(0).Height
            'reposition and offset the starting point
            If (nDiff > 10) Or (nDiff < -10) Then
                tblpData.RowStyles(0).Height = nNewPos
                tblpData.Refresh()
                mnGridDragStartMouse = mnGridDragStartMouse - nDiff
            End If
        End If
    End Sub

    Private Sub gpbGridView_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles gpbGridView.MouseUp
        '********************************************************************************************
        'Description:  Drag the grid frame for resize
        '
        'Parameters: mouse event args
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mnGridDragActive = 2 Then
            Dim nNewPos As Single = mnGridDragStartGripPos - (mnGridDragStartMouse - e.Location.Y)
            If nNewPos < 0 Then
                nNewPos = 0
            End If
            If (nNewPos > (tblpData.Height * 0.9)) Then
                nNewPos = CType((tblpData.Height * 0.9), Single)
            End If
            tblpData.RowStyles(0).Height = nNewPos
            Me.Refresh()
        End If
        mnGridDragActive = 0
    End Sub


    Private Sub dgGridView_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgGridView.CellClick
        '********************************************************************************************
        'Description:  Click in the grid.  If it's on a valve, change it.  Not on a valve, just select the step.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEventBlocker Then Exit Sub
        If mColorChange.Cycle(mnOldCycle).NumberOfSteps = 0 Then Exit Sub
        If e.ColumnIndex > -1 Then
            mnStep = e.ColumnIndex + 1
            If e.RowIndex >= mnGRID_ROW_VALVE1 Then
                If Privilege = ePrivilege.None Then
                    'I couldn't find a way to let them scroll without enabling the controls
                Else
                    Dim nValve As Integer = e.RowIndex - mnGRID_ROW_VALVE1
                    If nValve < mCCToon.NumberOfSharedValves Then
                        subToggleValve(nValve, True)
                    Else
                        nValve = nValve - mCCToon.NumberOfSharedValves
                        subToggleValve(nValve, False)
                    End If
                End If
            Else
                If Privilege > ePrivilege.None Then
                    If e.RowIndex = mnGRID_ROW_TIME Or e.RowIndex = mnGRID_ROW_PRESET Then
                        dgGridView.BeginEdit(True)
                    End If
                End If
            End If
            subSelectStep(mnStep)
        End If
    End Sub

    Private Sub dgGridView_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles dgGridView.MouseDown
        '********************************************************************************************
        'Description:  Track right-clicks before the menu opens
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If mbEventBlocker Then Exit Sub
        If dgGridView.ColumnCount < mnPopupMenuStep Then
            subSelectStep(dgGridView.ColumnCount)
            mnPopupMenuStep = dgGridView.ColumnCount
        End If

    End Sub
    Private Sub dgGridView_ColumnWidthChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewColumnEventArgs) Handles dgGridView.ColumnWidthChanged
        '********************************************************************************************
        'Description:  Column size adjusted by user.  Adjust time setting if in time display
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEventBlocker Then Exit Sub
        If mnOldCycle = 0 Then Exit Sub
        Dim nStep As Integer = e.Column.Index + 1
        If rdoTime.Checked Then
            'use some integer math to force precision to 1/10th
            subSetStepTime(nStep, CType((dgGridView.Columns(nStep - 1).Width * 10 \ mnGridPtsPerSec) / 10, Single))
        End If

    End Sub

    Private Sub rdoStepsTime_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoSteps.CheckedChanged, rdoTime.CheckedChanged
        '********************************************************************************************
        'Description:  Select column size by time or not
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        With dgGridView

            If rdoSteps.Checked Then
                If .Top <> pnlScale.Top Then
                    .Top = pnlScale.Top
                    .Height = .Height + pnlScale.Height
                End If
                dgGridView.AutoResizeColumns()
            Else
                If .Top = pnlScale.Top Then
                    .Top = pnlScale.Top + pnlScale.Height
                    .Height = .Height - pnlScale.Height
                End If
            End If

            If mColorChange Is Nothing Then Exit Sub ' startup
            For nStep As Integer = 1 To mColorChange.Cycle(mnOldCycle).NumberOfSteps
                If rdoSteps.Checked Then
                    dgGridView.AutoResizeColumns()
                    dgGridView.AllowUserToResizeColumns = True
                Else
                    dgGridView.Columns(nStep - 1).Width = CType(mColorChange.Cycle(mnOldCycle).Steps(nStep).Duration.Value * mnGridPtsPerSec, Integer)
                    dgGridView.AllowUserToResizeColumns = (Privilege = ePrivilege.Edit) Or (Privilege = ePrivilege.Copy)
                End If
            Next
        End With
    End Sub

    Private Sub dgGridView_Scroll(ByVal sender As Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles dgGridView.Scroll
        '********************************************************************************************
        'Description:  Adjust time scale for horizontal scrollbar
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        '       Debug.Print(dgGridView.HorizontalScrollingOffset)
        picScale.Left = -1 * dgGridView.HorizontalScrollingOffset
    End Sub
    Private Sub dgGridView_CellMouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles dgGridView.CellMouseDown
        '********************************************************************************************
        'Description:  Used to catch a right click in the grid.  track the column number for the pop-up menu
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEventBlocker Then Exit Sub
        If e.ColumnIndex > -1 Then
            mnPopupMenuStep = e.ColumnIndex + 1
        End If

    End Sub

    Private Sub btnTall_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTall.Click
        '********************************************************************************************
        'Description:  resize the gridview height
        '
        'Parameters: mouse event args
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp As String = DirectCast(btnTall.Tag, String)
        If sTmp = "_Next" Then
            tblpData.RowStyles(0).Height = mnSaveRowHeight
            btnTall.Image = DirectCast(gpsRM.GetObject("Previous"), Image)
            btnTall.Tag = "Previous"
        Else
            mnSaveRowHeight = tblpData.RowStyles(0).Height
            tblpData.RowStyles(0).Height = 0
            btnTall.Image = DirectCast(gpsRM.GetObject("_Next"), Image)
            btnTall.Tag = "_Next"
        End If
        mnGridDragActive = 0

    End Sub
    Private Sub btnWide_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWide.Click
        '********************************************************************************************
        'Description:  resize the gridview width
        '
        'Parameters: mouse event args
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp As String = DirectCast(btnWide.Tag, String)

        If sTmp = "Back" Then
            If uctrlCartoon Is Nothing Then
                tblpData.Width = 694 - tblpData.Left - 5
            Else
                tblpData.Width = uctrlCartoon.Left - tblpData.Left - 5
            End If
            btnWide.Image = DirectCast(gpsRM.GetObject("Forward"), Image)
            btnWide.Tag = "Forward"
        Else
            tblpData.Width = tscMain.ContentPanel.Width - 2 * tblpData.Left
            btnWide.Image = DirectCast(gpsRM.GetObject("Back"), Image)
            btnWide.Tag = "Back"
        End If
        picScale.Left = -1 * dgGridView.HorizontalScrollingOffset
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
                Case Keys.F1
                    'Help Screen request
                    sKeyValue = "btnHelp"
                    subLaunchHelp(gs_HELP_COLORCHANGE, oIPC)
                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty

                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath & msSCREEN_DUMP_NAME & msSCREEN_DUMP_EXT, Imaging.ImageFormat.Jpeg)

                Case Keys.Escape
                    Me.Close()

                Case Else

            End Select
        End If

    End Sub



    Private Sub dgGridView_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgGridView.SelectionChanged
        '********************************************************************************************
        'Description: Support arrow-key navigation on grid view
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Dim tdg As DataGridView = DirectCast(sender, DataGridView)
        If Not (tdg Is Nothing) And Not (mColorChange Is Nothing) And DataLoaded Then
            If tdg.SelectedColumns.Count > 0 Then
                mnStep = tdg.SelectedColumns.Item(0).Index + 1
                'subSelectStep(mnStep) '11/28/09 RJO
                If mnOldCycle <> 0 Then Call subSelectStep(mnStep)
            End If
        End If

    End Sub

    Private Sub dgGridView_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgGridView.CellEndEdit
        '********************************************************************************************
        'Description: Time or preset edited on gridview
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Dim sVal As String = dgGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString
        If e.RowIndex = mnGRID_ROW_PRESET Then
            Dim nPreset As Integer = CInt(sVal)
            If nPreset.ToString = sVal Then
                subSetStepPreset((e.ColumnIndex + 1), nPreset)
            End If
        End If
        If e.RowIndex = mnGRID_ROW_TIME Then
            Dim nTime As Single = CSng(sVal)
            If nTime.ToString = sVal Then
                'It's a number
                subSetStepTime((e.ColumnIndex + 1), nTime)
            End If
        End If

    End Sub

    Private Sub dgGridView_DataError(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles dgGridView.DataError
        '********************************************************************************************
        'Description: edit error 
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Dim sVal As String = dgGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString
        Dim lRet As DialogResult
        lRet = MessageBox.Show(gcsRM.GetString("csINV_ENTRY"), gcsRM.GetString("csERROR"), _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        dgGridView.CancelEdit()
        'If e.RowIndex = mnGRID_ROW_PRESET Then
        '    subSetStepPreset((e.ColumnIndex + 1), mColorChange.Cycle(mnOldCycle).Steps(mnStep).Preset.Value)
        'End If
        'If e.RowIndex = mnGRID_ROW_TIME Then
        '    'It's a number
        '    subSetStepTime((e.ColumnIndex + 1), mColorChange.Cycle(mnOldCycle).Steps(mnStep).Duration.Value)
        'End If

    End Sub

    Private Sub pnlOtherData_Layout(ByVal sender As Object, ByVal e As System.Windows.Forms.LayoutEventArgs) Handles pnlOtherData.Layout
        '********************************************************************************************
        'Description: adjust locations in the panel after a resize
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************

        Const nPUSHVOL_BELOW_PNLHEIGHT As Integer = 51
        Dim nMAX_PUSH_POS As Integer = gpbPreset.Top + gpbPreset.Height + 5
        Dim nMIN_PUSH_POS As Integer = gpbPreset.Top + nudPreset.Top + nudPreset.Height + 3
        If gpbPushVol.Visible Then
            Dim nTmpTop As Integer = pnlOtherData.Height - nPUSHVOL_BELOW_PNLHEIGHT
            If nTmpTop < nMIN_PUSH_POS Then
                nTmpTop = nMIN_PUSH_POS
            ElseIf nTmpTop > nMAX_PUSH_POS Then
                nTmpTop = nMAX_PUSH_POS
            End If
            gpbPushVol.Top = nTmpTop
        Else
            gpbPushVol.Top = nMAX_PUSH_POS
        End If
    End Sub

End Class