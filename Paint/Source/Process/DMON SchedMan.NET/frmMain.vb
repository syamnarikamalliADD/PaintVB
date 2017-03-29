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
' Description: DMON Schedule Manager
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Matt White
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    10/14/11   MSW     1st                                                           4.1.1.0
'    01/03/12   MSW     various version compatibility fixes                           4.01.01.01
'    01/18/12   MSW     add page setup menu item                                      4.01.01.02
'    01/24/12   MSW     Applicator Updates                                            4.01.01.03
'    02/16/12   MSW     Print/import/export updates, force 32 bit build               4.01.01.04
'    03/22/12   RJO     Modified for .NET Password and IPC                            4.01.02.00
'    04/11/12   MSW     Changed CommonStrings setup so it ubilds correctly            4.01.03.00
'    04/20/12   MSW     frmMain_KeyDown-Add excape key handler                        4.01.03.01
'    10/04/12   RJO     Bug fix. For 2 equipments, DiagEnable can be 1 or 2 in PLC.   4.01.03.02
'    10/24/12   RJO     Added StartupModule to project to prevent multiple instances  4.01.03.03
'    04/16/13   MSW     Add Canadian language files                                   4.01.05.00
'                       clsDMONCfg - LoadFromRobot -
'                       Adjust the error handling a bit so we don't   
'                       fill the event log with items the robot leaves uninit
'                       Standalone Changelog
'    07/09/13   MSW     Update and standardize logos                                    4.01.05.01
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up  4.01.05.02
'    09/30/13   MSW     Save screenshots as jpegs, PLC DLL                              4.01.06.00
'    12/03/13   MSW     Clean up hidden labels for robots w/o painttool                 4.01.06.01
'    01/14/14   MSW     subShowNewPage - Handle switch from multiarm to singlearm       4.01.06.02
'                       schedule selection
'    02/12/14   MSW     Deal with changes in robot variable structure                   4.01.06.03
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call                4.01.07.00
'    07/30/14   MSW     Update for R30iB Device name variable                           4.01.07.01
'**************************************************************************************************


'******** Unfinished Business   *******************************************************************
'******** End Unfinished Business   ***************************************************************

Imports Response = System.Windows.Forms.DialogResult
Imports ConnStat = FRRobotNeighborhood.FRERNConnectionStatusConstants
Imports clsPLCComm = PLCComm.clsPLCComm

Friend Class frmMain

#Region " Declares "
    '******** Form Constants   *********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Public Const msSCREEN_NAME As String = "DMONSchedMan"   ' <-- For password area change log etc.
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    Friend Const mbUSEROBOTS As Boolean = True
    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    Friend gsChangeLogArea As String = msSCREEN_NAME
    Private msCulture As String = "en-US" 'Default to English
    Private msOldZone As String = String.Empty
    Private mcolZones As clsZones = Nothing
    Private msTabName As String = String.Empty
    Private oCurTab As TabPage = Nothing
    Private mbLocal As Boolean
    Private mbScreenLoaded As Boolean
    Private mbEventBlocker As Boolean
    Private mbRemoteZone As Boolean
    Private mbControllerEventBlocker As Boolean = False
    Private mPrintHtml As clsPrintHtml


    Private msSCREEN_DUMP_NAME As String = "Process_DmonMan" 'changes with selected tab
    Private moMaint As Maintenance = Nothing
    Private mnRobot As Integer = -1
    Private mnParm As Integer = -1
    Private msControllerName As String = ""
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private mbEditsMade As Boolean
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean
    Private mnProgress As Integer
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    '******** End Property Variables    *************************************************************

    '******** Structures    *************************************************************************
    '******** End Structures    *********************************************************************

    Private WithEvents colControllers As clsControllers = Nothing
    Private WithEvents mPLC As clsPLCComm
    Private msPLCData() As String
    Private mDMONCfg As clsDMONCfg
    Private mbInAskForSave As Boolean = False
    '******** This is the old pw3 password object interop  ******************************************
    'Friend WithEvents moPassword As PWPassword.PasswordObject 'RJO 03/22/12
    'Friend moPrivilege As PWPassword.cPrivilegeObject

    Friend WithEvents moPassword As clsPWUser 'RJO 03/22/12
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm 'RJO 03/22/12

    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    Delegate Sub ControllerStatusChange(ByVal Controller As clsController)
    Delegate Sub NewMessage_Callback(ByVal Schema As String, ByVal DS As DataSet) 'RJO 03/22/12
    '******** This is the old pw3 password object interop  ******************************************


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
            End If
            'if not loaded disable all controls
            subEnableControls(True)
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

            mPrivilegeRequested = Value

            'handle logout
            If mPrivilegeRequested = ePrivilege.None Then
                mPrivilegeGranted = ePrivilege.None
                subEnableControls(True)  ' This is confusing True but really false sub will look at privilege
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
                Exit Property
            End If

            'prevent recursion
            If mPrivilegeGranted = mPrivilegeRequested Then
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
                'hack me
                If moPassword.UserName <> String.Empty Then _
                        mPrivilegeRequested = mPrivilegeGranted
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
            End If

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

        Dim oCulture As System.Globalization.CultureInfo = DisplayCulture

        Dim lRet As Response = MessageBox.Show(gcsRM.GetString("csSAVEMSG1", oCulture) & vbCrLf & _
                                               gcsRM.GetString("csSAVEMSG", oCulture), _
                                               gcsRM.GetString("csSAVE_CHANGES", oCulture), _
                                               MessageBoxButtons.YesNoCancel, _
                                               MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

        Select Case lRet
            Case Response.Yes 'Response.Yes
                Select Case Privilege
                    Case ePrivilege.None
                        'false aborts closing form , changing zone etc
                        Status = gcsRM.GetString("csSAVE_CANCEL", oCulture)
                        Privilege = ePrivilege.Edit 'popup a login
                        Return False
                    Case Else
                        mbInAskForSave = True
                        Call subSaveData()
                        mbInAskForSave = False
                        Return True
                End Select
            Case Response.Cancel
                'false aborts closing form , changing zone etc
                Status = gcsRM.GetString("csSAVE_CANCEL", oCulture)
                Return False
            Case Else
                Return True
        End Select

    End Function

    Private Function bSaveToGUI() As Boolean
        '********************************************************************************************
        'Description:  Save data stored on GUI in database, xml file etc.
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            If moMaint.oDiagnosticDaysToKeep.Changed Then
                subUpdateChangeLog(moMaint.oDiagnosticDaysToKeep.Value.ToString(), moMaint.oDiagnosticDaysToKeep.OldValue.ToString, mcolZones.ActiveZone, mcolZones.CurrentZone, _
                                                gpsRM.GetString("psDMONFiles") & " " & gpsRM.GetString("psDAYS_TO_KEEP"))
            End If
            If moMaint.oDiagnosticArchiveDaysToKeep.Changed Then
                subUpdateChangeLog(moMaint.oDiagnosticArchiveDaysToKeep.Value.ToString(), moMaint.oDiagnosticArchiveDaysToKeep.OldValue.ToString, mcolZones.ActiveZone, mcolZones.CurrentZone, _
                                                gpsRM.GetString("psARCHIVE") & " " & gpsRM.GetString("psDAYS_TO_KEEP"))
            End If
            moMaint.SaveToDatabase()
            Return (True)
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bSaveToGUI", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Return (False)
        End Try

    End Function
    Private Function bSaveToPLC(ByRef oController As clsController) As Boolean
        '********************************************************************************************
        'Description:  Save data to PLC
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/04/12  RJO     Bug fix. For 2 equipments, DiagEnable can be 1 or 2 in PLC.
        '********************************************************************************************
        Dim sTag As String
        Dim sData(0) As String
        If mcolZones.StandAlone Then
            Return True
        End If
        Try
            If mPLC Is Nothing Then
                mPLC = New clsPLCComm
            End If

            If oController.DMONCfg.PLCEnable.Value Then
                If oController.DMONCfg.Schedule.Value < 6 Then 'RJO 10/04/12
                    sData(0) = "1"
                Else
                    sData(0) = "2"
                End If
            Else
                sData(0) = "0"
            End If

            With mPLC
                .ZoneName = mcolZones.ActiveZone.Name
                sTag = oController.FanucName & "DiagEnable"
                .TagName = sTag
                .PLCData = sData
                subUpdateChangeLog(oController.DMONCfg.PLCEnable.Value.ToString(), oController.DMONCfg.PLCEnable.OldValue.ToString, mcolZones.ActiveZone, mcolZones.CurrentZone, _
                                                gpsRM.GetString("psDMON_PLC_ENABLE"))
                oController.DMONCfg.PLCEnable.Update()
            End With
            Return (True)
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bSaveToPLC", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Function
    Private Sub MakeCheckBox(ByRef rControl As CheckBox, ByVal sName As String, _
                      ByVal rLoc As Point, ByVal rModel As CheckBox)
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
        rControl = (DirectCast(tabSystem.Controls(sName), CheckBox))
        If rControl Is Nothing Then
            rControl = New CheckBox
            rControl.Name = sName
            rControl.Text = String.Empty
            rControl.Location = rLoc
            rControl.BackColor = rModel.BackColor
            rControl.CreateControl()
            rControl.Show()
            rControl.Parent = Me.tabSystem
            Me.tabSystem.Controls.Add(rControl)
        End If
        rControl.TextAlign = rModel.TextAlign
        rControl.Size = rModel.Size
        rControl.Font = rModel.Font
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
        rControl = (DirectCast(tabSystem.Controls(sName), Label))
        If rControl Is Nothing Then
            rControl = New Label
            rControl.Name = sName
            rControl.Text = String.Empty
            rControl.Location = rLoc
            rControl.BackColor = rModel.BackColor
            rControl.BorderStyle = rModel.BorderStyle
            rControl.CreateControl()
            rControl.Show()
            rControl.Parent = Me.tabSystem
            Me.tabSystem.Controls.Add(rControl)
        End If
        rControl.TextAlign = rModel.TextAlign
        rControl.Size = rModel.Size
        rControl.Font = rModel.Font
        rControl.Visible = rModel.Visible
    End Sub
    Private Sub MakeComboBox(ByRef rControl As ComboBox, ByVal sName As String, _
                  ByVal rLoc As Point, ByVal rModel As ComboBox)
        '********************************************************************************************
        'Description:  Make a new ComboBox and add it to pnlMain
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        rControl = (DirectCast(tabSystem.Controls(sName), ComboBox))
        If rControl Is Nothing Then
            rControl = New ComboBox
            rControl.Name = sName
            rControl.Text = String.Empty
            rControl.Location = rLoc
            rControl.ItemHeight = rModel.ItemHeight
            rControl.Size = rModel.Size
            rControl.Font = rModel.Font
            rControl.Visible = rModel.Visible
            rControl.DropDownStyle = rModel.DropDownStyle
            rControl.FlatStyle = rModel.FlatStyle
            rControl.Font = rModel.Font
            rControl.BackColor = rModel.BackColor
            rControl.ForeColor = rModel.ForeColor
            rControl.CreateControl()
            rControl.Show()
            rControl.Parent = Me.tabSystem
            Me.tabSystem.Controls.Add(rControl)
        End If
        rControl.Name = sName
        rControl.Text = String.Empty
        rControl.Location = rLoc
        rControl.ItemHeight = rModel.ItemHeight
        rControl.Size = rModel.Size
        rControl.Font = rModel.Font
        rControl.Visible = rModel.Visible
        rControl.DropDownStyle = rModel.DropDownStyle
        rControl.FlatStyle = rModel.FlatStyle
        rControl.Font = rModel.Font
        rControl.BackColor = rModel.BackColor
        rControl.ForeColor = rModel.ForeColor


    End Sub
    Private Sub subFormatScreenLayout()
        '********************************************************************************************
        'Description: Update the robot list for the screen
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nRobot As Integer = 0
        For nRobot = 1 To colControllers.Count
            Dim chkRC As CheckBox = DirectCast(tabSystem.Controls("chkRC_" & nRobot.ToString), CheckBox)
            Dim cboEnable As ComboBox = DirectCast(tabSystem.Controls("cboEnable" & nRobot.ToString), ComboBox)
            Dim cboSchedule As ComboBox = DirectCast(tabSystem.Controls("cboSchedule" & nRobot.ToString), ComboBox)
            If chkRC Is Nothing Then
                'Make the labels
                Dim pt As Point = chkRC_1.Location
                Dim nDiff As Integer = (chkRC_2.Location.Y - chkRC_1.Location.Y)
                nDiff = nDiff * (nRobot - 1)
                pt.Y = pt.Y + nDiff
                MakeCheckBox(chkRC, ("chkRC_" & nRobot.ToString), pt, chkRC_1)
                pt.X = cboEnable1.Location.X
                MakeComboBox(cboEnable, ("cboEnable" & nRobot.ToString), pt, cboEnable1)
                pt.X = cboSchedule1.Location.X
                MakeComboBox(cboSchedule, ("cboSchedule" & nRobot.ToString), pt, cboSchedule1)
            End If
            chkRC.Text = colControllers(nRobot - 1).Name
        Next
        nRobot = colControllers.Count + 1
        Dim bDone As Boolean = False
        Do Until bDone
            Dim chkRC As CheckBox = DirectCast(tabSystem.Controls("chkRC_" & nRobot.ToString), CheckBox)
            If chkRC IsNot Nothing Then
                chkRC.Visible = False
                Dim cboEnable As ComboBox = DirectCast(tabSystem.Controls("cboEnable" & nRobot.ToString), ComboBox)
                Dim cboSchedule As ComboBox = DirectCast(tabSystem.Controls("cboSchedule" & nRobot.ToString), ComboBox)
                cboEnable.Visible = False
                cboSchedule.Visible = False
                nRobot = nRobot + 1
            Else
                bDone = True
            End If
        Loop
        Application.DoEvents()
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
        msOldZone = cboZone.Text
        mcolZones.CurrentZone = cboZone.Text

        tabMain.SelectedIndex = 0 'NRU 161006 When changing zone in multizone start over on first tab or be punished by failures to update old zone data.

        Try
            mbControllerEventBlocker = True

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subEnableControls(False)
            EditsMade = False
            DataLoaded = False

            colControllers = New clsControllers(mcolZones, False)
            SetUpStatusStrip(Me, colControllers)
            colControllers.RefreshConnectionStatus()
            LoadRobotBoxFromCollection(cboRobot, colControllers, False)
            subFormatScreenLayout()
            oCurTab = tabMain.SelectedTab
            Application.DoEvents()
            mbControllerEventBlocker = False
            If mbScreenLoaded = True Then
                msTabName = tabMain.SelectedTab.Name
                Call subLoadData()
                subShowNewPage(True)
            Else
                subEnableControls(True)

            End If

            'NRU 161006 Autoselect if there is a single robot
            If cboRobot.Items.Count = 1 Then cboRobot.SelectedIndex = 0

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try


    End Sub

    Private Sub subEnableControls(ByVal bEnable As Boolean)
        '********************************************************************************************
        'Description: Disable or enable controls. Check privileges and edits etc. 
        '
        'Parameters: bEnable - false can be used to disable during load etc 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/02/10  RJO     If user logged in and had Copy privilege, controls would not be enabled.
        '                   Added ePrivilege.Copy to Case statement
        '********************************************************************************************
        Dim bRestOfControls As Boolean = False
        btnClose.Enabled = True

        If bEnable = False Then
            btnSave.Enabled = False
            btnUndo.Enabled = False
            btnChangeLog.Enabled = False
            bRestOfControls = False
            pnlMain.Enabled = False
            btnPrint.Enabled = False
            mnuPrintFile.Enabled = False
            btnUtilities.Enabled = False
        Else
            Select Case Privilege
                Case ePrivilege.None
                    btnSave.Enabled = False
                    btnUndo.Enabled = False
                    btnChangeLog.Enabled = (True And DataLoaded)
                    bRestOfControls = False
                    pnlMain.Enabled = True
                    btnPrint.Enabled = DataLoaded
                    mnuPrintFile.Enabled = False
                    btnUtilities.Enabled = False
                Case ePrivilege.Edit, ePrivilege.Copy
                    btnSave.Enabled = EditsMade
                    btnUndo.Enabled = EditsMade
                    btnChangeLog.Enabled = DataLoaded
                    bRestOfControls = DataLoaded
                    pnlMain.Enabled = True
                    btnPrint.Enabled = bRestOfControls
                    mnuPrintFile.Enabled = bRestOfControls
            End Select
        End If
        'only update what's visible
        Select Case msTabName
            Case tabSystem.Name
                ftbArchive.Enabled = bRestOfControls
                ftbDMONFiles.Enabled = bRestOfControls
                If colControllers Is Nothing Then
                    Dim bDone As Boolean = False
                    Dim nRobot As Integer = 1
                    Do Until bDone = True
                        Dim chkRC As CheckBox = DirectCast(tabSystem.Controls("chkRC_" & nRobot.ToString), CheckBox)
                        Dim cboEnable As ComboBox = DirectCast(tabSystem.Controls("cboEnable" & nRobot.ToString), ComboBox)
                        Dim cboSchedule As ComboBox = DirectCast(tabSystem.Controls("cboSchedule" & nRobot.ToString), ComboBox)
                        If cboEnable Is Nothing Then
                            bDone = True
                        Else
                            cboEnable.Enabled = False
                            cboSchedule.Enabled = False
                            chkRC.Enabled = False
                            nRobot = nRobot + 1
                        End If
                    Loop
                Else
                    For nRobot As Integer = 1 To colControllers.Count
                        Dim chkRC As CheckBox = DirectCast(tabSystem.Controls("chkRC_" & nRobot.ToString), CheckBox)
                        Dim cboEnable As ComboBox = DirectCast(tabSystem.Controls("cboEnable" & nRobot.ToString), ComboBox)
                        Dim cboSchedule As ComboBox = DirectCast(tabSystem.Controls("cboSchedule" & nRobot.ToString), ComboBox)
                        chkRC.Enabled = bRestOfControls
                        cboEnable.Enabled = bRestOfControls
                        cboSchedule.Enabled = bRestOfControls
                    Next
                End If
                btnUtilities.Enabled = False
            Case tabSchedules.Name
                btnUtilities.Enabled = bRestOfControls
                If bRestOfControls Then
                    If (mcolZones.StandAlone = False) AndAlso (mDMONCfg.PLCEnable.OldValue) Then
                        bRestOfControls = False
                    Else
                        If mDMONCfg.UsePT Then
                            If mDMONCfg.AutoSample.OldValue Then
                                bRestOfControls = False
                            ElseIf mDMONCfg.UseCC AndAlso mDMONCfg.EnableCCDiagOldValue Then
                                bRestOfControls = False
                            End If
                        Else
                            'SEALER - are there more conditions to check than just the PLC enable?
                        End If
                    End If
                End If
                lblSaveWarning.Visible = (bRestOfControls = False)
                ftbComment.Enabled = bRestOfControls
                cboFileDevice.Enabled = bRestOfControls
                ftbFileName.Enabled = bRestOfControls
                ftbFileIndex.Enabled = bRestOfControls
                ftbFileSize.Enabled = bRestOfControls
                ftbNumItems.Enabled = bRestOfControls

                ftbSampleReq.Enabled = bRestOfControls
                ftbSampleAct.Enabled = bRestOfControls
                ftbMonitorReq.Enabled = bRestOfControls
                ftbMonitorAct.Enabled = bRestOfControls
                ftbRecordReq.Enabled = bRestOfControls
                ftbRecordAct.Enabled = bRestOfControls

                cboRecordMode.Enabled = bRestOfControls

                chkStartItem.Enabled = bRestOfControls
                chkStopItem.Enabled = bRestOfControls
                cboStartCond.Enabled = bRestOfControls
                cboStopCond.Enabled = bRestOfControls
                cboStartItem.Enabled = bRestOfControls
                cboStopItem.Enabled = bRestOfControls
                ftbStartValue.Enabled = bRestOfControls
                ftbStopValue.Enabled = bRestOfControls
                For nItem As Integer = 1 To 10
                    Dim cboItem As ComboBox = DirectCast(tabSchedules.Controls("cboItem" & nItem.ToString), ComboBox)
                    cboItem.Enabled = bRestOfControls
                Next
            Case tabItems.Name
                btnUtilities.Enabled = bRestOfControls
                If bRestOfControls Then
                    If (mcolZones.StandAlone = False) AndAlso (mDMONCfg.PLCEnable.OldValue) Then
                        bRestOfControls = False
                    Else
                        If mDMONCfg.UsePT Then
                            If mDMONCfg.AutoSample.OldValue Then
                                bRestOfControls = False
                            ElseIf mDMONCfg.UseCC AndAlso mDMONCfg.EnableCCDiagOldValue Then
                                bRestOfControls = False
                            End If
                        Else
                            'SEALER - are there more conditions to check than just the PLC enable?
                        End If
                    End If
                End If
                lblSaveWarning1.Visible = (bRestOfControls = False)
                cboType.Enabled = bRestOfControls
                cboIoType.Enabled = False
                ftbPortNum.Enabled = False
                ftbGroup.Enabled = False
                ftbAxis.Enabled = False
                ftbPrgName.Enabled = False
                ftbVarName.Enabled = False
                If bRestOfControls Then
                    If cboType.SelectedIndex > -1 Then
                        Dim nTag() As clsDMONCfg.eTYPE = CType(cboType.Tag, clsDMONCfg.eTYPE())
                        Select Case nTag(cboType.SelectedIndex)
                            Case clsDMONCfg.eTYPE.Int, clsDMONCfg.eTYPE.Real
                                ftbPrgName.Enabled = True
                                ftbVarName.Enabled = True
                            Case clsDMONCfg.eTYPE.IO
                                cboIoType.Enabled = True
                                ftbPortNum.Enabled = True
                            Case clsDMONCfg.eTYPE.Register
                                ftbPortNum.Enabled = True
                            Case clsDMONCfg.eTYPE.Axis
                                ftbGroup.Enabled = True
                                ftbAxis.Enabled = True
                            Case Else
                                '?
                        End Select
                    End If
                End If
                ftbDesc.Enabled = bRestOfControls
                ftbUnits.Enabled = bRestOfControls
                ftbSlope.Enabled = bRestOfControls
                ftbIntercept.Enabled = bRestOfControls
                btnValidate.Enabled = bRestOfControls
                btnBrowse.Enabled = bRestOfControls
        End Select

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
        Dim oCulture As CultureInfo = DisplayCulture

        Try

            With gpsRM
                'main menu
                mnuPrintAll.Text = .GetString("psPRINTALL", oCulture)
                mnuSaveDefault.Text = .GetString("psSAVE_DEFAULT_MNU", oCulture)
                mnuRestoreDefault.Text = .GetString("psRESTORE_DEFAULT_MNU", oCulture)
                ' set labels 
                tabSystem.Text = .GetString("psSYSTEM", oCulture)
                tabSchedules.Text = .GetString("psSCHEDULES", oCulture)
                tabItems.Text = .GetString("psITEMS", oCulture)
                ' tabSystem
                lblJobOrCC.Text = .GetString("psJOBORCC", oCulture)
                lblScheduleCap.Text = .GetString("psSCHEDULE", oCulture)
                lblEnableCap.Text = .GetString("psENABLE", oCulture)
                lblDaysToKeep.Text = .GetString("psDAYS_TO_KEEP", oCulture)
                lblDMONFiles.Text = .GetString("psDMONFiles", oCulture)
                lblArchive.Text = .GetString("psARCHIVE", oCulture)
                ' tabSchedule
                lblComment.Text = .GetString("psCOMMENT", oCulture)
                lblFileDevice.Text = .GetString("psFILEDEVICE", oCulture)

                lblFileName.Text = .GetString("psFILENAME", oCulture)
                lblFileIndex.Text = .GetString("psFILEINDEX", oCulture)
                lblFileSize.Text = .GetString("psFILESIZE", oCulture)
                lblKB.Text = .GetString("psKB", oCulture)
                lblNumItems.Text = .GetString("psNUMITEMS", oCulture)
                lblFrequency.Text = .GetString("psFREQUENCY", oCulture)
                lblReq.Text = .GetString("psREQ", oCulture)
                lblAct.Text = .GetString("psACT", oCulture)
                lblSample.Text = .GetString("psSAMPLE", oCulture)
                lblMonitor.Text = .GetString("psMONITOR", oCulture)
                lblRecord.Text = .GetString("psRECORD", oCulture)


                lblRecordMode.Text = .GetString("psRECORDMODE", oCulture)
                cboRecordMode.Items.Add(.GetString("psRECORDMODE1", oCulture))
                cboRecordMode.Items.Add(.GetString("psRECORDMODE2", oCulture))
                Dim nTag(1) As Integer
                nTag(0) = 1
                nTag(1) = 2
                cboRecordMode.Tag = nTag

                chkStartItem.Text = .GetString("psSTARTITEM", oCulture)
                chkStopItem.Text = .GetString("psSTOPITEM", oCulture)

                lblSaveWarning.Text = .GetString("psDSBLAUTOWARN", oCulture)

                'TabItems
                lblType.Text = .GetString("psTYPE", oCulture)
                lblIoType.Text = .GetString("psIOTYPE", oCulture)
                lblPortNum.Text = .GetString("psPORTNUM", oCulture)
                lblAxis.Text = .GetString("psAXIS", oCulture)
                lblG.Text = .GetString("psG", oCulture)
                lblA.Text = .GetString("psA", oCulture)
                lblPrgName.Text = .GetString("psPRGNAME", oCulture)
                lblVarName.Text = .GetString("psVARNAME", oCulture)
                lblDesc.Text = .GetString("psDESC", oCulture)
                lblUnits.Text = .GetString("psUNITS", oCulture)
                lblSlope.Text = .GetString("psSLOPE", oCulture)
                lblIntercept.Text = .GetString("psINTERCEPT", oCulture)
                btnValidate.Text = .GetString("psVALIDATE", oCulture)
                btnBrowse.Text = .GetString("psBROWSE", oCulture)

                lblPortNumComment.Text = String.Empty

                For nItem As Integer = 1 To 10
                    Dim lblItem As Label = DirectCast(tabSchedules.Controls("lblItem" & nItem.ToString), Label)
                    lblItem.Text = Format(.GetString("psITEM_X", oCulture), nItem.ToString)
                Next
                lblSaveWarning1.Text = .GetString("psDSBLAUTOWARN", oCulture)

            End With 'gpsRM

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitFormText", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

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
        '04/25/11  HGB      Hide PLC menu item if SA
        '********************************************************************************************
        Dim lReply As Response = Response.OK
        'Dim cachePrivilege As mPassword.ePrivilege 'RJO 03/22/12

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

        Status = gcsRM.GetString("csINITALIZING", DisplayCulture)
        Progress = 1

        Try
            DataLoaded = False
            EditsMade = False
            mbScreenLoaded = False

            subProcessCommandLine()

            mcolZones = New clsZones(String.Empty)
            'Show the Zone combobox only if this computer connects to remote zones
            mbLocal = False 'NRU 161006 Changed to False. (Does not affect local/remote functionality, only hides zone box)
            For Each oZone As clsZone In mcolZones
                If oZone.IsRemoteZone Then
                    mbLocal = False
                    Exit For
                End If
            Next
            lblZone.Visible = Not mbLocal
            cboZone.Visible = Not mbLocal

            'init the old password for now
            'moPassword = New PWPassword.PasswordObject 'RJO 03/22/12
            'mPassword.InitPassword(moPassword, moPrivilege, LoggedOnUser, msSCREEN_NAME)
            'If LoggedOnUser <> String.Empty Then

            '    If moPrivilege.ActionAllowed Then
            '        cachePrivilege = mPassword.PrivilegeStringToEnum(moPrivilege.Privilege)
            '    Else
            '        cachePrivilege = ePrivilege.None
            '    End If

            '    Privilege = cachePrivilege

            'Else
            '    Privilege = ePrivilege.None
            'End If

            Progress = 5

            Me.Text = gpsRM.GetString("psSCREENCAPTION", DisplayCulture)

            mScreenSetup.LoadZoneBox(cboZone, mcolZones, False)

            Progress = 30

            'init new IPC and new Password 'RJO 03/15/12
            oIPC = New Paintworks_IPC.clsInterProcessComm
            moPassword = New clsPWUser
            Progress = 50

            mPrintHtml = New clsPrintHtml

            mScreenSetup.InitializeForm(Me)
            Call subInitFormText()
            Me.Show()

            'Handle user selection change events with common event handler routines

            Progress = 70

            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound

            'statusbar text
            If (Not mbLocal) And (cboZone.Text = String.Empty) Then
                Status(True) = gcsRM.GetString("csSELECT_ZONE", DisplayCulture)
            End If

            Progress = 98

            mbScreenLoaded = True
            If mbLocal Then
                cboZone.Text = cboZone.Items(0).ToString
                Call subChangeZone()
            End If

            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, Privilege, mcolZones.ActiveZone.IsRemoteZone)

        Catch ex As Exception

            lReply = ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), ex, msSCREEN_NAME, _
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
    Private Function bSaveToRobot(ByRef oController As clsController, ByVal bDetails As Boolean) As Boolean
        '********************************************************************************************
        'Description:  Update the robot controller
        '
        'Parameters: oController - controller to load from, bDetails - load details for items and schedule tab
        'Returns:    True = OK
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim bReturn As Boolean = True
            Try
                oController.DMONCfg.SaveToRobot(oController, bDetails, mcolZones.ActiveZone)
            Catch ex As Exception
                Trace.WriteLine(ex.Message)
                bReturn = False
            End Try
            Return bReturn
        Catch ex As Exception
            Return False
        End Try

    End Function
    Private Function bLoadFromRobot(ByRef oController As clsController, ByVal bDetails As Boolean) As Boolean
        '********************************************************************************************
        'Description:  Update the robot controller data
        '
        'Parameters: oController - controller to load from, bDetails - load details for items and schedule tab
        'Returns:    True = OK
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Try
                oController.DMONCfg.LoadFromRobot(oController, bDetails)
            Catch ex As Exception
                'Get what we can
            End Try
            Return True
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bLoadFromRobot", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Return False
        End Try

    End Function
    Private Function bLoadFromPlc(ByRef oController As clsController) As Boolean
        '********************************************************************************************
        'Description:  Setup the PLC hotlink
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/04/12  RJO     Bug fix. For 2 equipments, DiagEnable can be 1 or 2 in PLC.
        '********************************************************************************************
        Dim sTag As String
        Dim sData() As String
        If mcolZones.StandAlone Then
            Return True
        End If
        Try
            If mPLC Is Nothing Then
                mPLC = New clsPLCComm
            End If

            With mPLC
                .ZoneName = mcolZones.ActiveZone.Name
                sTag = oController.FanucName & "DiagEnable"
                .TagName = sTag
                sData = .PLCData
            End With
            Try
                If sData IsNot Nothing AndAlso _
                    (sData.GetUpperBound(0) > -1) AndAlso _
                    (sData(0) <> String.Empty) AndAlso _
                    (IsNumeric(sData(0))) Then
                    If CType(sData(0), Integer) > 0 Then 'RJO 10/04/12
                        oController.DMONCfg.PLCEnable.Value = True
                    Else
                        oController.DMONCfg.PLCEnable.Value = False
                    End If
                    oController.DMONCfg.PLCEnable.Update()
                Else
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bLoadFromPlc", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                oController.DMONCfg.PLCEnable.Value = False
                Return (False)
            End Try

            Return (True)
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bLoadFromPlc", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try


    End Function
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
            Dim sTmp As String = gcsRM.GetString("csLOADINGDATA", DisplayCulture)

            Status = sTmp
            Dim bLoaded As Boolean = True
            Progress = 5
            Select Case msTabName
                Case tabSystem.Name
                    'Load system setup, robot schedules for the system tab
                    Dim nProgressPerRobot As Integer = CType((80 / colControllers.Count), Integer)
                    Dim nProgress As Integer = 20
                    If moMaint Is Nothing Then
                        moMaint = New Maintenance(mcolZones.ActiveZone)
                    Else
                        moMaint.RefreshInfo()
                    End If
                    Progress = nProgress
                    Try
                        For Each oController As clsController In colControllers
                            If oController.DMONCfg Is Nothing Then
                                oController.DMONCfg = New clsDMONCfg
                            End If
                            If Not (mcolZones.StandAlone) Then
                                If (bLoadFromPlc(oController) = False) Then
                                    bLoaded = False
                                End If

                            End If
                            If oController.Robot.IsConnected Then
                                bLoaded = bLoaded And bLoadFromRobot(oController, False)
                                'Still set data loaded if some robots are offline, bring them up when they're available
                            End If
                            nProgress = nProgress + nProgressPerRobot
                            Progress = nProgress
                        Next
                        EditsMade = False
                    Catch ex As Exception
                        DataLoaded = False
                    End Try
                Case Else
                    'Load single robot details
                    bLoaded = False
                    mDMONCfg = Nothing
                    If cboRobot.SelectedIndex > -1 Then
                        Dim oController As clsController = colControllers(cboRobot.SelectedIndex)
                        If oController.Robot.IsConnected Then
                            bLoaded = bLoadFromRobot(oController, True)
                            mDMONCfg = oController.DMONCfg
                            If mDMONCfg.RobotVersion >= 8.0 Then
                                cboFileDevice.DropDownStyle = ComboBoxStyle.DropDown
                            Else
                                cboFileDevice.DropDownStyle = ComboBoxStyle.DropDownList
                            End If

                        End If

                    End If
            End Select

            DataLoaded = bLoaded


            Progress = 98

            If DataLoaded Then

                Status = gcsRM.GetString("csLOADDONE", DisplayCulture)

                subShowNewPage(True)

                'this resets edit flag
                DataLoaded = True

                Status = gcsRM.GetString("csLOADDONE", DisplayCulture)

            Else
                Status = gcsRM.GetString("csLOADFAILED", DisplayCulture)
                'load failed
                MessageBox.Show(gcsRM.GetString("csLOADFAILED"), msSCREEN_NAME, MessageBoxButtons.OK, _
                                MessageBoxIcon.Error)

            End If

        Catch ex As Exception

            mDebug.ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), ex, msSCREEN_NAME, _
                                       Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default

        End Try

    End Sub


    Private Sub subLogChanges()
        '********************************************************************************************
        'Description:  Log changes to the database after a save
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/03/12  MSW     various version compatibility fixes
        '********************************************************************************************
        'If mcolZones.SaveAccessData Then
        '    mPWCommon.SaveToChangeLog(mcolZones.ActiveZone.DatabasePath) 'Access Database
        'End If
        mPWCommon.SaveToChangeLog(mcolZones.ActiveZone) 'SQL database

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
        Dim sCultureArg As String = "/culture="

        'If a culture string has been passed in, set the current culture (display language)
        For Each s As String In My.Application.CommandLineArgs
            If s.ToLower.StartsWith(sCultureArg) Then
                Culture = s.Remove(0, sCultureArg.Length)
            End If
        Next

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
        ' 08/24/11  MSW     Improve error handling
        '********************************************************************************************

        Select Case Privilege
            Case ePrivilege.None
                ' shouldnt be here
                Exit Sub

            Case Else
                'ok
        End Select
        tabMain.Focus()
        Application.DoEvents()
        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        Dim bOK As Boolean = True
        Try
            Select Case msTabName
                Case tabSystem.Name
                    'Load system setup, robot schedules for the system tab
                    Dim nProgressPerRobot As Integer = CType((80 / colControllers.Count), Integer)
                    Dim nProgress As Integer = 20
                    bOK = bSaveToGUI()
                    Progress = nProgress
                    Try
                        For Each oController As clsController In colControllers
                            If Not (mcolZones.StandAlone) Then
                                bOK = bOK And bSaveToPLC(oController)
                            End If
                            If oController.Robot.IsConnected Then
                                bOK = bOK And bSaveToRobot(oController, False)
                            Else
                                MessageBox.Show(Format(gcsRM.GetString("csSAVEROBOTOFFLINE"), oController.Name), msSCREEN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End If
                            nProgress = nProgress + nProgressPerRobot
                            Progress = nProgress
                        Next
                    Catch ex As Exception
                    End Try
                Case Else
                    'Load single robot details
                    If cboRobot.SelectedIndex > -1 Then
                        For Each oController As clsController In colControllers
                            If oController.Name = msControllerName Then
                                bOK = bOK And bSaveToRobot(oController, True)
                            ElseIf (oController.Name = "RC_M1") Then
                                bOK = bOK And bSaveToRobot(oController, True)
                            Else

                            End If
                        Next
                    End If
            End Select
            ' do save
            If bOK Then
                Call subLogChanges()
                ' save done
                EditsMade = False
                If Not mbInAskForSave Then
                    Call subLoadData()
                    subShowNewPage(True)
                End If
                
                Status = gcsRM.GetString("csSAVE_DONE", DisplayCulture)
            Else
                Status = gcsRM.GetString("csSAVEFAILED", DisplayCulture)
                'save to DB failed
                MessageBox.Show(gcsRM.GetString("csSAVEFAILED"), msSCREEN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Call subLogChanges()
                Call subLoadData()
            End If 'bSaveToGUI()

        Catch ex As Exception

            mDebug.ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED", DisplayCulture), ex, msSCREEN_NAME, _
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
            mPWCommon.subShowChangeLog(mcolZones.CurrentZone, gsChangeLogArea, msSCREEN_NAME, gcsRM.GetString("csALL"), _
                                          gcsRM.GetString("csALL"), mDeclares.eParamType.None, nIndex, True, False, oIPC)

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        End Try

    End Sub
    Private Sub subLoadEnableCbo(ByRef rCbo As ComboBox)
        '********************************************************************************************
        'Description:  Load an enable cbo
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        rCbo.Items.Clear()
        rCbo.Items.Add(gcsRM.GetString("csJOB"))
        rCbo.Items.Add(gcsRM.GetString("csCCCYCLE"))

    End Sub
    Public Sub subSetCbo(ByRef oCbo As ComboBox, ByRef oIntVal As clsIntValue)
        '********************************************************************************************
        'Description:  shortcut for cbo handling with clsIntValue 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nTag() As Integer = CType(oCbo.Tag, Integer())
        If nTag Is Nothing Then
            oCbo.SelectedIndex = oIntVal.Value - 1
        Else
            For nIndex As Integer = 0 To nTag.GetUpperBound(0)
                If nTag(nIndex) = oIntVal.Value Then
                    oCbo.SelectedIndex = nIndex
                End If
            Next
        End If
        If oIntVal.Changed Then
            oCbo.ForeColor = Color.Red
        Else
            oCbo.ForeColor = Color.Black
        End If
        oCbo.Refresh()
    End Sub
    Private Sub subSetFtb(ByRef oFtb As FocusedTextBox.FocusedTextBox, ByRef oTxtVal As clsTextValue)
        '********************************************************************************************
        'Description:  shortcut for control handling
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        oFtb.Text = oTxtVal.Text
        oFtb.MaxLength = oTxtVal.MaxLength
        If oTxtVal.Changed Then
            oFtb.ForeColor = Color.Red
            oFtb.Modified = True
        Else
            oFtb.ForeColor = Color.Black
            oFtb.Modified = False
        End If
    End Sub
    Private Sub subSetFtb(ByRef oFtb As FocusedTextBox.FocusedTextBox, ByRef oIntVal As clsIntValue)
        '********************************************************************************************
        'Description:  shortcut for control handling
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        oFtb.Text = oIntVal.Value.ToString
        If oIntVal.Changed Then
            oFtb.ForeColor = Color.Red
            oFtb.Modified = True
        Else
            oFtb.ForeColor = Color.Black
            oFtb.Modified = False
        End If
    End Sub
    Private Sub subSetFtb(ByRef oFtb As FocusedTextBox.FocusedTextBox, ByRef oSngVal As clsSngValue)
        '********************************************************************************************
        'Description:  shortcut for control handling
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        oFtb.Text = oSngVal.Value.ToString
        If oSngVal.Changed Then
            oFtb.ForeColor = Color.Red
            oFtb.Modified = True
        Else
            oFtb.ForeColor = Color.Black
            oFtb.Modified = False
        End If
    End Sub
    Private Sub subSetFtbChange(ByRef oFtb As FocusedTextBox.FocusedTextBox, ByVal bChanged As Boolean)
        '********************************************************************************************
        'Description:  shortcut for control handling
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If bChanged Then
            oFtb.ForeColor = Color.Red
            oFtb.Modified = True
        Else
            oFtb.ForeColor = Color.Black
            oFtb.Modified = False
        End If
    End Sub
    Private Sub subSetChk(ByRef oChk As CheckBox, ByRef oBoolVal As clsBoolValue)
        '********************************************************************************************
        'Description:  shortcut for control handling
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason

        '********************************************************************************************
        oChk.Checked = oBoolVal.Value
        If oBoolVal.Changed Then
            oChk.ForeColor = Color.Red
        Else
            oChk.ForeColor = Color.Black
        End If
    End Sub
    Private Sub subShowDetailPage()
        '********************************************************************************************
        'Description:  show detail after item or schedule is selected 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 07/28/14  MSW     Update for R30iB Device name variable
        '********************************************************************************************
        Dim bOldEventBlocker As Boolean = mbEventBlocker
        mbEventBlocker = True
        Dim bTmp As Boolean = False

        Select Case tabMain.SelectedTab.Name
            Case tabSystem.Name
                'Not expecting this.
                Debug.Assert(False)
            Case tabSchedules.Name
                Dim nIndex As Integer = cboParm.SelectedIndex
                If (mDMONCfg IsNot Nothing) AndAlso (nIndex >= -1) AndAlso _
                 (nIndex < mDMONCfg.NumSchedules) Then
                    With mDMONCfg.Schedules(nIndex)
                        subSetFtb(ftbComment, .COMMENT)
                        If .DEVICE.Value = -1 Then
                            Dim sDevice As String = .DEVICE_NAME.Text
                            If cboFileDevice.Items.Contains(sDevice) = False Then
                                cboFileDevice.Items.Add(sDevice)
                            End If
                            cboFileDevice.Text = sDevice
                            ' 07/30/14  MSW     Update for R30iB Device name variable
                            If .DEVICE_NAME.Changed Then
                                cboFileDevice.ForeColor = Color.Red
                            Else
                                cboFileDevice.ForeColor = Color.Black
                            End If
                        Else
                        subSetCbo(cboFileDevice, .DEVICE)
                        End If
                        subSetFtb(ftbFileName, .FILE_NAME)
                        subSetFtb(ftbFileIndex, .FILE_INDEX)
                        subSetFtb(ftbFileSize, .FILE_SIZE)
                        subSetFtb(ftbNumItems, .NUM_ITEMS)

                        ftbSampleAct.Text = .SAMP_RATE.ToString
                        subSetFtbChange(ftbSampleAct, False)
                        subSetFtb(ftbSampleReq, .SAMP_REQ)

                        ftbMonitorAct.Text = .MNTR_RATE.ToString
                        subSetFtbChange(ftbMonitorAct, False)
                        subSetFtb(ftbMonitorReq, .MNTR_REQ)

                        ftbRecordAct.Text = .REC_PER_SEC.ToString
                        subSetFtbChange(ftbRecordAct, False)
                        subSetFtb(ftbRecordReq, .REC_REQ)

                        subSetCbo(cboRecordMode, .REC_MODE)

                        subSetChk(chkStartItem, .START_ENBL)
                        subSetChk(chkStopItem, .STOP_ENBL)
                        subSetCbo(cboStartItem, .START_ITEM)
                        subSetCbo(cboStopItem, .STOP_ITEM)
                        subSetCbo(cboStartCond, .START_COND)
                        subSetCbo(cboStopCond, .STOP_COND)
                        subSetFtb(ftbStartValue, .START_VALUE)
                        subSetFtb(ftbStopValue, .STOP_VALUE)

                        For nItem As Integer = 1 To mDMONCfg.MaxItemsPerSchedule
                            Dim oCbo As ComboBox = DirectCast(tabSchedules.Controls("cboItem" & nItem.ToString), ComboBox)
                            subSetCbo(oCbo, .ITEM(nItem - 1))
                        Next

                    End With


                Else
                    'Not expecting this.
                    Debug.Assert(False)
                End If
            Case tabItems.Name
                Dim nIndex As Integer = cboParm.SelectedIndex
                If (mDMONCfg IsNot Nothing) AndAlso (nIndex >= -1) AndAlso _
                 (nIndex < mDMONCfg.NumItems) Then
                    With mDMONCfg.Items(nIndex)
                        subSetCbo(cboType, .TYPE)
                        subSetCbo(cboIoType, .IO_TYPE)
                        subSetFtb(ftbPortNum, .PORT_NUM)
                        subSetFtb(ftbGroup, .GROUP_NUM)
                        subSetFtb(ftbAxis, .AXIS_NUM)

                        subSetFtb(ftbPrgName, .PRG_NAME)
                        subSetFtb(ftbVarName, .VAR_NAME)
                        subSetFtb(ftbDesc, .DESC)
                        subSetFtb(ftbUnits, .UNITS)
                        subSetFtb(ftbSlope, .SLOPE)
                        subSetFtb(ftbIntercept, .INTERCEPT)
                        Select Case .TYPE.Value
                            Case clsDMONCfg.eTYPE.Register
                                'Robot Register lookup
                                Try
                                    If cboRobot.SelectedIndex > -1 AndAlso cboParm.SelectedIndex > -1 Then
                                        For Each oController As clsController In colControllers
                                            If oController.Name = msControllerName Then
                                                Dim nReg As Integer = .PORT_NUM.Value
                                                Dim oReg As FRRobot.FRCRegNumeric = oGetRegister(oController.Robot, nReg)
                                                If oReg IsNot Nothing Then
                                                    lblPortNumComment.Text = oReg.Comment
                                                End If
                                            End If
                                        Next
                                    End If
                                Catch ex As Exception
                                End Try
                            Case Else
                                lblPortNumComment.Text = String.Empty
                        End Select
                    End With

                Else
                    'Not expecting this.
                    Debug.Assert(False)
                End If
        End Select

        subEnableControls(True)
        mbEventBlocker = bOldEventBlocker
    End Sub
    Private Sub subShowNewPage(ByVal BlockEvents As Boolean)
        '********************************************************************************************
        'Description:  Data is all loaded or changed and screen needs to be refreshed
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/03/12  MSW     various version compatibility fixes
        '********************************************************************************************
        mbEventBlocker = True
        Dim bTmp As Boolean = False

        Select Case tabMain.SelectedTab.Name
            Case tabSystem.Name
                cboRobot.Visible = False
                lblRobot.Visible = False
                cboParm.Visible = False
                lblParm.Visible = False
                mnuPrintAll.Visible = False
                If DataLoaded Then
                    If moMaint IsNot Nothing Then
                        ftbArchive.NumericOnly = True
                        ftbDMONFiles.NumericOnly = True
                        ftbDMONFiles.Text = moMaint.DiagnosticDaysToKeep.ToString
                        ftbArchive.Text = moMaint.DiagnosticArchiveDaysToKeep.ToString
                        ftbDMONFiles.Modified = moMaint.oDiagnosticDaysToKeep.Changed
                        ftbArchive.Modified = moMaint.oDiagnosticArchiveDaysToKeep.Changed
                        subSetFtbChange(ftbDMONFiles, moMaint.oDiagnosticDaysToKeep.Changed)
                        subSetFtbChange(ftbArchive, moMaint.oDiagnosticArchiveDaysToKeep.Changed)
                    Else
                        ftbDMONFiles.Text = String.Empty
                        ftbArchive.Text = String.Empty
                    End If
                    Try
                        lblJobOrCC.Visible = False
                        lblScheduleCap.Visible = False
                        For nRobot As Integer = 1 To colControllers.Count
                            Try
                                Dim chkRC As CheckBox = DirectCast(tabSystem.Controls("chkRC_" & nRobot.ToString), CheckBox)
                                Dim cboEnable As ComboBox = DirectCast(tabSystem.Controls("cboEnable" & nRobot.ToString), ComboBox)
                                Dim cboSchedule As ComboBox = DirectCast(tabSystem.Controls("cboSchedule" & nRobot.ToString), ComboBox)
                                If colControllers(nRobot - 1).Robot.IsConnected Then
                                    chkRC.Visible = True
                                    Dim oDMON As clsDMONCfg = colControllers(nRobot - 1).DMONCfg
                                    If oDMON IsNot Nothing AndAlso oDMON.Loaded Then
                                        If oDMON.UsePT Then
                                            lblScheduleCap.Visible = True
                                            subLoadEnableCbo(cboEnable)
                                            If oDMON.UseCC Then
                                                cboEnable.Visible = True
                                                lblJobOrCC.Visible = True
                                            Else
                                                cboEnable.Visible = False
                                            End If
                                            If oDMON.AutoSample.Value Then
                                                cboEnable.SelectedIndex = 0
                                                chkRC.Checked = True
                                                oDMON.AsyncEnab.Value = False
                                                oDMON.EnableCCDiag = False
                                                oDMON.PLCEnable.Value = True
                                            ElseIf oDMON.UseCC AndAlso oDMON.EnableCCDiag Then
                                                cboEnable.SelectedIndex = 1
                                                chkRC.Checked = True
                                                oDMON.AutoSample.Value = False
                                                oDMON.AsyncEnab.Value = True
                                                oDMON.PLCEnable.Value = True
                                            Else
                                                cboEnable.SelectedIndex = 0
                                                chkRC.Checked = False
                                                oDMON.AsyncEnab.Value = False
                                                oDMON.EnableCCDiag = False
                                                oDMON.PLCEnable.Value = False
                                            End If
                                            If oDMON.AutoSample.Changed Or oDMON.AsyncEnab.Changed Or oDMON.EnableCCDiagChanged Or _
                                                    (oDMON.PLCEnable.Changed And (mcolZones.StandAlone = False)) Then
                                                chkRC.ForeColor = Color.Red
                                                cboEnable.ForeColor = Color.Red
                                                cboEnable.Refresh()
                                            Else
                                                chkRC.ForeColor = Color.Black
                                                cboEnable.ForeColor = Color.Black
                                            End If
                                            oDMON.LoadScheduleBox(cboSchedule)
                                            subSetCbo(cboSchedule, oDMON.Schedule)
                                            chkRC.Visible = True
                                            cboEnable.Visible = True
                                            cboSchedule.Visible = True
                                        Else
                                            lblScheduleCap.Visible = False
                                            cboSchedule.Visible = False
                                            cboEnable.Visible = False
                                            'Just enable/Disable recording
                                            If (mcolZones.StandAlone) Then
                                                chkRC.Visible = False
                                            Else
                                                cboEnable.Visible = False
                                                chkRC.Visible = True
                                                chkRC.Checked = oDMON.PLCEnable.Value
                                                If oDMON.PLCEnable.Changed Then
                                                    chkRC.ForeColor = Color.Red
                                                Else
                                                    chkRC.ForeColor = Color.Black
                                                End If
                                            End If
                                        End If
                                        chkRC.Enabled = chkRC.Visible
                                    End If
                                Else
                                                chkRC.Visible = True
                                    cboEnable.Visible = False
                                    cboSchedule.Visible = False
                                    chkRC.Enabled = False
                                End If

                            Catch ex As Exception
                                mDebug.WriteEventToLog(msSCREEN_NAME & " Module:frmMain Routine: subShowNewPage", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                            End Try
                        Next
                    Catch ex As Exception
                    End Try
                End If

            Case tabSchedules.Name
                If DataLoaded Then
                    Dim nIndex As Integer = cboParm.SelectedIndex
                    If (lblParm.Text <> gpsRM.GetString("psSCHEDULE")) Then
                        lblParm.Text = gpsRM.GetString("psSCHEDULE")
                        nIndex = 0
                        cboParm.SelectedIndex = -1
                    End If
                    mDMONCfg.LoadScheduleBox(cboParm)
                    mDMONCfg.LoadItemBox(cboStartItem)
                    mDMONCfg.LoadItemBox(cboStopItem)
                    mDMONCfg.LoadItemBox(cboItem1)
                    mDMONCfg.LoadItemBox(cboItem2)
                    mDMONCfg.LoadItemBox(cboItem3)
                    mDMONCfg.LoadItemBox(cboItem4)
                    mDMONCfg.LoadItemBox(cboItem5)
                    mDMONCfg.LoadItemBox(cboItem6)
                    mDMONCfg.LoadItemBox(cboItem7)
                    mDMONCfg.LoadItemBox(cboItem8)
                    mDMONCfg.LoadItemBox(cboItem9)
                    mDMONCfg.LoadItemBox(cboItem10)
                    mDMONCfg.LoadFileDeviceBox(cboFileDevice)
                    mDMONCfg.LoadStartStopConditionBox(cboStartCond)
                    mDMONCfg.LoadStartStopConditionBox(cboStopCond)

                    If nIndex >= 0 Then
                        cboParm.SelectedIndex = nIndex
                    Else
                        cboParm.SelectedIndex = 0
                    End If
                    subShowDetailPage()
                Else
                    lblParm.Text = gpsRM.GetString("psSCHEDULE")
                End If
                cboRobot.Visible = True
                lblRobot.Visible = True
                cboParm.Visible = True
                lblParm.Visible = True
                mnuPrintAll.Visible = True
            Case tabItems.Name
                If DataLoaded Then
                    Dim nIndex As Integer = cboParm.SelectedIndex
                    If (lblParm.Text <> gpsRM.GetString("psITEM")) Then
                        lblParm.Text = gpsRM.GetString("psITEM")
                        nIndex = 0
                        cboParm.SelectedIndex = -1
                    End If

                    mDMONCfg.LoadItemBox(cboParm, False)
                    mDMONCfg.LoadTYPEBox(cboType)
                    mDMONCfg.LoadIO_TYPEBox(cboIoType)
                    If mDMONCfg.DisplayItemAxis Then
                        lblAxis.Visible = True
                        lblG.Visible = True
                        lblA.Visible = True
                        ftbGroup.Visible = True
                        ftbAxis.Visible = True
                    Else
                        lblAxis.Visible = True
                        lblG.Visible = True
                        lblA.Visible = True
                        ftbGroup.Visible = True
                        ftbAxis.Visible = True
                    End If
                    If nIndex >= 0 Then
                        cboParm.SelectedIndex = nIndex
                    Else
                        cboParm.SelectedIndex = 0
                    End If
                    subShowDetailPage()
                Else
                    lblParm.Text = gpsRM.GetString("psITEM")
                End If
                cboRobot.Visible = True
                lblRobot.Visible = True
                cboParm.Visible = True
                lblParm.Visible = True
                mnuPrintAll.Visible = True
        End Select

        subEnableControls(True)
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

        If MessageBox.Show(gcsRM.GetString("csUNDOMSG", DisplayCulture), _
                                        msSCREEN_NAME, MessageBoxButtons.OKCancel, _
                                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) _
                                                                    = Response.OK Then
            subLoadData()
        End If

    End Sub


    Friend Overloads Sub subUpdateChangeLog(ByRef NewValue As String, ByRef OldValue As String, _
                                        ByRef oZone As clsZone, ByRef DeviceName As String, _
                                        ByRef sDesc As String)
        '********************************************************************************************
        'Description:  Build up the change text for a value change
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp As String = sDesc & " " & gcsRM.GetString("csCHANGED_FROM", DisplayCulture) & " " & OldValue & " " & _
                        gcsRM.GetString("csTO", DisplayCulture) & " " & NewValue

        Call mPWCommon.AddChangeRecordToCollection(gsChangeLogArea, LoggedOnUser, _
                                        oZone.ZoneNumber, DeviceName, String.Empty, sTmp, oZone.Name)

    End Sub

#End Region

#Region " Form Events "

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

        'Check for call from the robot object thread
        If Me.stsStatus.InvokeRequired Then
            Dim dCntrStat As New ControllerStatusChange(AddressOf colControllers_ConnectionStatusChange)
            Me.BeginInvoke(dCntrStat, New Object() {Controller})
        Else
            subDoStatusBar(Controller)
            Application.DoEvents()
            'Update system tab with multiple robots automatically
            If (mbControllerEventBlocker = False) And (tabMain.SelectedTab.Name = tabSystem.Name) Then
                If Controller.RCMConnectStatus = FRRobotNeighborhood.FRERNConnectionStatusConstants.frRNConnected Then
                    If Controller.DMONCfg Is Nothing Then
                        Controller.DMONCfg = New clsDMONCfg
                    End If
                    If Not (mcolZones.StandAlone) Then
                        bLoadFromPlc(Controller)
                    End If

                    If Controller.Robot.IsConnected Then
                        Application.DoEvents()
                        Thread.Sleep(250)
                        Application.DoEvents()
                        bLoadFromRobot(Controller, False)
                        'Still set data loaded if some robots are offline, bring them up when they're available
                    End If
                End If
                subShowNewPage(True)
            End If
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
        '    04/20/12   MSW     frmMain_KeyDown-Add excape key handler
        '********************************************************************************************
        Dim sKeyValue As String = String.Empty
        Dim sTab As String = tabMain.SelectedTab.Name
        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode
                Case Keys.F1
                    'Help Screen request
                    sKeyValue = "btnHelp"
                    Select Case tabMain.SelectedTab.Name
                        Case tabSystem.Name
                            subLaunchHelp(gs_HELP_DMON_SCHEDMAN_SYSTEM, oIPC)
                        Case tabSchedules.Name
                            subLaunchHelp(gs_HELP_DMON_SCHEDMAN_SCHEDULES, oIPC)
                        Case tabItems.Name
                            subLaunchHelp(gs_HELP_DMON_SCHEDMAN_ITEMS, oIPC)
                        Case Else
                            subLaunchHelp(gs_HELP_DMON_SCHEDMAN, oIPC)
                    End Select

                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty
                    Dim sDumpName As String = msSCREEN_DUMP_NAME

                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, sDumpName)

                    Select Case tabMain.SelectedTab.Name
                        Case tabSystem.Name
                            sDumpPath = sDumpPath & "_SYSTEM.jpg"
                        Case tabSchedules.Name
                            sDumpPath = sDumpPath & "_SCHEDULES.jpg"
                        Case tabItems.Name
                            sDumpPath = sDumpPath & "_ITEMS.jpg"
                        Case Else
                            sDumpPath = sDumpPath & ".jpg"
                    End Select

                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath, Imaging.ImageFormat.Jpeg)

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
        'Content panel 1016, 626
        'pnlmain 1001, 557
        'TODO - find out what this -70 is from.
        Dim nHeight As Integer = tscMain.ContentPanel.Height - 70
        Dim nWidth As Integer = tscMain.ContentPanel.Width - (pnlMain.Left * 2)
        If tscMain.ContentPanel.Height > 800 Then
            Debug.Print("why")
        End If
        If nHeight < 100 Then nHeight = 100
        If nWidth < 100 Then nWidth = 100

        pnlMain.Height = nHeight
        pnlMain.Width = nWidth

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
        End Try

    End Sub

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

#End Region

#Region " Control Events "



    Private Sub cboZone_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) _
                                        Handles cboZone.SelectionChangeCommitted
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
            Application.DoEvents()
            subChangeZone()
        End If

    End Sub


#End Region

#Region " Temp Stuff for Old Password Object "

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
        ' 11/12/09  MSW     add delegate declarations tohandle cross-thread calls
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
        ' 11/12/09  MSW     add delegate declarations tohandle cross-thread calls
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

#Region " Main Menu Button Events "

    Private Sub btnFunction_DropDownOpening(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFunction.DropDownOpening
        '********************************************************************************************
        'Description:  this was needed to enable the menus for some reason
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim cachePrivilege As ePrivilege

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
                If mbRemoteZone = False Then
                    mnuRemote.Enabled = True
                End If
            Else
                mnuRemote.Enabled = False
            End If
            Privilege = cachePrivilege

        Else
            Privilege = ePrivilege.None
        End If

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

    Private Sub mnuLast24_Click(ByVal sender As Object, _
                                            ByVal e As System.EventArgs) Handles mnuLast24.Click
        '********************************************************************************************
        'Description:  show changes for last 24 hours
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

        subInitializeForm()

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

            subInitializeForm()
        Else
            mbRemoteZone = bTmp
        End If 'SetRemoteServer()

        mbEventBlocker = False

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

            Case "btnStatus"
                lstStatus.Visible = Not lstStatus.Visible

            Case "btnSave"
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
                        subEnableControls(True)
                    Case Else
                        subUndoData()
                End Select

            Case "btnChangeLog"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                subShowChangeLog(CType(eChangeMnu.Hours24, Integer))
            Case btnUtilities.Name
                btnUtilities.ShowDropDown()
        End Select

    End Sub

#End Region

#Region " Log In, Log Out Events "

    Private Sub mnuLogIn_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                  Handles mnuLogin.Click
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

    Private Sub mnuLogOut_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                                    Handles mnuLogOut.Click
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

#End Region

    Private Sub ftb_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles _
        ftbArchive.KeyPress, ftbDMONFiles.KeyPress, _
        ftbComment.KeyPress, ftbFileName.KeyPress, ftbFileIndex.KeyPress, ftbFileSize.KeyPress, _
        ftbNumItems.KeyPress, ftbSampleReq.KeyPress, ftbMonitorReq.KeyPress, ftbRecordReq.KeyPress, _
        ftbStartValue.KeyPress, ftbStopValue.KeyPress, _
        ftbPortNum.KeyPress, ftbGroup.KeyPress, ftbAxis.KeyPress, ftbPrgName.KeyPress, _
        ftbVarName.KeyPress, ftbDesc.KeyPress, ftbUnits.KeyPress, ftbSlope.KeyPress, ftbIntercept.KeyPress
        '********************************************************************************************
        'Description: Check for keypress that requires login
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim oFTB As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
            If oFTB.NumericOnly Then
                'All numeric FTBs
                Select Case oFTB.Name
                    Case ftbArchive.Name, ftbDMONFiles.Name, ftbFileIndex.Name, ftbFileSize.Name, ftbNumItems.Name, _
                        ftbPortNum.Name, ftbGroup.Name, ftbAxis.Name
                        'positive integers - only let numbers through
                        If Char.IsPunctuation(e.KeyChar) OrElse Char.IsSymbol(e.KeyChar) OrElse Char.IsLetter(e.KeyChar) OrElse Char.IsWhiteSpace(e.KeyChar) Then
                            e.Handled = True
                        End If
                    Case ftbSampleReq.Name, ftbMonitorReq.Name, ftbRecordReq.Name, ftbStartValue.Name, ftbStopValue.Name, _
                        ftbSlope.Name, ftbIntercept.Name
                        'Positive floating point - also allow "."
                        If Char.IsPunctuation(e.KeyChar) OrElse Char.IsSymbol(e.KeyChar) OrElse Char.IsLetter(e.KeyChar) OrElse Char.IsWhiteSpace(e.KeyChar) Then
                            If e.KeyChar.ToString <> "." Then
                                e.Handled = True
                            End If
                        End If
                        'case
                        '    'Floating point - also allow "-"
                        '    If Char.IsPunctuation(e.KeyChar) OrElse Char.IsSymbol(e.KeyChar) OrElse Char.IsLetter(e.KeyChar) OrElse Char.IsWhiteSpace(e.KeyChar) Then
                        '        If (e.KeyChar.ToString <> ".") orelse (e.KeyChar.ToString <> "-")Then
                        '            e.Handled = True
                        '        End If
                        '    End If
                    Case Else
                        'Forgot something
                        Debug.Assert(False)
                End Select
            Else
                'All text FTBs
                Select Case oFTB.Name
                    Case ftbComment.Name, ftbDesc.Name, ftbUnits.Name
                        'No restriction
                    Case ftbFileName.Name
                        'File names.  No punctuation, spaces
                        If Char.IsPunctuation(e.KeyChar) OrElse Char.IsSymbol(e.KeyChar) OrElse Char.IsWhiteSpace(e.KeyChar) Then
                            e.Handled = True
                        End If
                        'First character must be a letter
                        If Char.IsNumber(e.KeyChar) Then
                            If oFTB.SelectionStart = 0 Then
                                e.Handled = True
                            End If
                        End If
                    Case ftbPrgName.Name
                        'Karel program names.  limited punctuation
                        If Char.IsPunctuation(e.KeyChar) OrElse Char.IsSymbol(e.KeyChar) OrElse Char.IsWhiteSpace(e.KeyChar) Then
                            If (e.KeyChar.ToString <> "*") Then
                                e.Handled = True
                            End If
                        End If
                        'First character must be a letter
                        If Char.IsNumber(e.KeyChar) Then
                            If oFTB.SelectionStart = 0 Then
                                e.Handled = True
                            End If
                        End If
                    Case ftbVarName.Name
                        'Karel variable names.  limited punctuation
                        If Char.IsPunctuation(e.KeyChar) OrElse Char.IsSymbol(e.KeyChar) OrElse Char.IsWhiteSpace(e.KeyChar) Then
                            If (e.KeyChar.ToString <> ".") And (e.KeyChar.ToString <> "$") And _
                               (e.KeyChar.ToString <> "[") And (e.KeyChar.ToString <> "]") Then
                                e.Handled = True
                            End If
                        End If
                        'First character must be a letter
                        If Char.IsNumber(e.KeyChar) Then
                            If oFTB.SelectionStart = 0 Then
                                e.Handled = True
                            End If
                        End If
                    Case Else
                        'Forgot something
                        Debug.Assert(False)
                End Select
            End If
            If oFTB.ReadOnly Then
                If Char.IsNumber(e.KeyChar) OrElse Char.IsLetter(e.KeyChar) Then
                    If Privilege = ePrivilege.None Then
                        Privilege = ePrivilege.Edit
                    End If
                End If
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module:frmMain Routine: ftbArchive_KeyPress", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub ftb_UpArrow(ByRef sender As FocusedTextBox.FocusedTextBox, ByVal bAlt As Boolean, ByVal bShift As Boolean, ByVal bControl As Boolean) Handles _
        ftbArchive.UpArrow, ftbDMONFiles.UpArrow, _
        ftbComment.UpArrow, ftbFileName.UpArrow, ftbFileIndex.UpArrow, ftbFileSize.UpArrow, _
        ftbNumItems.UpArrow, ftbSampleReq.UpArrow, ftbMonitorReq.UpArrow, ftbRecordReq.UpArrow, _
        ftbStartValue.UpArrow, ftbStopValue.UpArrow, _
        ftbPortNum.UpArrow, ftbGroup.UpArrow, ftbAxis.UpArrow, ftbPrgName.UpArrow, _
        ftbVarName.UpArrow, ftbDesc.UpArrow, ftbUnits.UpArrow, ftbSlope.UpArrow, ftbIntercept.UpArrow


        '********************************************************************************************
        'Description:  Arrow key handling for text boxes
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim nRow As Integer = 1
            Select Case sender.Name
                'System tab
                Case ftbDMONFiles.Name
                    ftbArchive.Focus()
                Case ftbArchive.Name
                    ftbDMONFiles.Focus()
                    'Schedule tab
                Case ftbComment.Name
                    chkStopItem.Focus()
                Case ftbFileName.Name
                    ftbComment.Focus()
                Case ftbFileIndex.Name
                    ftbFileName.Focus()
                Case ftbFileSize.Name
                    ftbFileIndex.Focus()
                Case ftbNumItems.Name
                    ftbFileSize.Focus()
                Case ftbSampleReq.Name
                    ftbNumItems.Focus()
                Case ftbMonitorReq.Name
                    ftbSampleReq.Focus()
                Case ftbRecordReq.Name
                    ftbMonitorReq.Focus()
                Case ftbStartValue.Name
                    ftbRecordReq.Focus()
                Case ftbStopValue.Name
                    ftbStartValue.Focus()
                    'Item tab
                Case ftbPortNum.Name
                    cboType.Focus()
                Case ftbGroup.Name, ftbAxis.Name
                    ftbPortNum.Focus()
                Case ftbPrgName.Name
                    ftbGroup.Focus()
                Case ftbVarName.Name
                    ftbPrgName.Focus()
                Case ftbDesc.Name
                    ftbVarName.Focus()
                Case ftbUnits.Name
                    ftbDesc.Focus()
                Case ftbSlope.Name
                    ftbUnits.Focus()
                Case ftbIntercept.Name
                    ftbSlope.Focus()
                Case Else
                    'Forgot something
                    Debug.Assert(False)
            End Select
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module:frmMain Routine: ftb_UpArrow", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Private Sub ftb_DownArrow(ByRef sender As FocusedTextBox.FocusedTextBox, ByVal bAlt As Boolean, ByVal bShift As Boolean, ByVal bControl As Boolean) Handles _
        ftbArchive.DownArrow, ftbDMONFiles.DownArrow, _
        ftbComment.DownArrow, ftbFileName.DownArrow, ftbFileIndex.DownArrow, ftbFileSize.DownArrow, _
        ftbNumItems.DownArrow, ftbSampleReq.DownArrow, ftbMonitorReq.DownArrow, ftbRecordReq.DownArrow, _
        ftbStartValue.DownArrow, ftbStopValue.DownArrow, _
        ftbPortNum.DownArrow, ftbGroup.DownArrow, ftbAxis.DownArrow, ftbPrgName.DownArrow, _
        ftbVarName.DownArrow, ftbDesc.DownArrow, ftbUnits.DownArrow, ftbSlope.DownArrow, ftbIntercept.DownArrow
        '********************************************************************************************
        'Description:  Arrow key handling for text boxes
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim nRow As Integer = 1
            Select Case sender.Name
                'System tab
                Case ftbDMONFiles.Name
                    ftbArchive.Focus()
                Case ftbArchive.Name
                    ftbDMONFiles.Focus()
                    'Schedule tab
                Case ftbComment.Name
                    ftbFileName.Focus()
                Case ftbFileName.Name
                    ftbFileIndex.Focus()
                Case ftbFileIndex.Name
                    ftbFileSize.Focus()
                Case ftbFileSize.Name
                    ftbNumItems.Focus()
                Case ftbNumItems.Name
                    ftbSampleReq.Focus()
                Case ftbSampleReq.Name
                    ftbMonitorReq.Focus()
                Case ftbMonitorReq.Name
                    ftbRecordReq.Focus()
                Case ftbRecordReq.Name
                    ftbStartValue.Focus()
                Case ftbStartValue.Name
                    ftbStopValue.Focus()
                Case ftbStopValue.Name
                    ftbComment.Focus()
                    'Item tab
                Case ftbPortNum.Name
                    ftbGroup.Focus()
                Case ftbGroup.Name, ftbAxis.Name
                    ftbPrgName.Focus()
                Case ftbPrgName.Name
                    ftbVarName.Focus()
                Case ftbVarName.Name
                    ftbDesc.Focus()
                Case ftbDesc.Name
                    ftbUnits.Focus()
                Case ftbUnits.Name
                    ftbSlope.Focus()
                Case ftbSlope.Name
                    ftbIntercept.Focus()
                Case ftbIntercept.Name
                    cboType.Focus()
                Case Else
                    'Forgot something
                    Debug.Assert(False)
            End Select
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module:frmMain Routine: ftb_DownArrow", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Private Sub ftb_LeftArrow(ByRef sender As FocusedTextBox.FocusedTextBox, ByVal bAlt As Boolean, ByVal bShift As Boolean, ByVal bControl As Boolean) Handles ftbDMONFiles.LeftArrow, _
                            ftbArchive.LeftArrow, _
                            ftbGroup.LeftArrow, ftbAxis.LeftArrow
        '********************************************************************************************
        'Description:  Arrow key handling for text boxes
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim nRow As Integer = 1
            Select Case sender.Name
                Case ftbDMONFiles.Name
                    nRow = 1
                Case ftbArchive.Name
                    If colControllers.Count > 1 Then
                        nRow = 2
                    Else
                        nRow = 1
                    End If
                Case ftbGroup.Name
                    ftbAxis.Focus()
                Case ftbAxis.Name
                    ftbGroup.Focus()
                Case Else

            End Select
            Dim sNewName As String = "cboSchedule" & nRow.ToString
            Dim o As ComboBox = DirectCast(tabSystem.Controls(sNewName), ComboBox)
            o.Focus()
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module:frmMain Routine: ftb_LeftArrow", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub ftb_RightArrow(ByRef sender As FocusedTextBox.FocusedTextBox, ByVal bAlt As Boolean, ByVal bShift As Boolean, ByVal bControl As Boolean) Handles ftbDMONFiles.RightArrow, _
                            ftbArchive.RightArrow, _
                            ftbGroup.RightArrow, ftbAxis.RightArrow
        '********************************************************************************************
        'Description:  Arrow key handling for text boxes
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            Dim nRow As Integer = 1
            Select Case sender.Name
                Case ftbDMONFiles.Name
                    nRow = 1
                Case ftbArchive.Name
                    If colControllers.Count > 1 Then
                        nRow = 2
                    Else
                        nRow = 1
                    End If
                Case ftbGroup.Name
                    ftbAxis.Focus()
                Case ftbAxis.Name
                    ftbGroup.Focus()
                Case Else
                    'Only supporting this on the system tab
            End Select
            Dim sNewName As String = "lblRC_" & nRow.ToString
            Dim o As Label = DirectCast(tabSystem.Controls(sNewName), Label)
            o.Focus()
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module:frmMain Routine: ftb_RightArrow", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub


    Private Sub ftb_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        ftbArchive.TextChanged, ftbDMONFiles.TextChanged, _
        ftbComment.TextChanged, ftbFileName.TextChanged, ftbFileIndex.TextChanged, ftbFileSize.TextChanged, _
        ftbNumItems.TextChanged, ftbSampleReq.TextChanged, ftbMonitorReq.TextChanged, ftbRecordReq.TextChanged, _
        ftbStartValue.TextChanged, ftbStopValue.TextChanged, _
        ftbPortNum.TextChanged, ftbGroup.TextChanged, ftbAxis.TextChanged, ftbPrgName.TextChanged, _
        ftbVarName.TextChanged, ftbDesc.TextChanged, ftbUnits.TextChanged, ftbSlope.TextChanged, ftbIntercept.TextChanged
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

        Try
            If mbEventBlocker Then Exit Sub
            If DataLoaded And EditsMade = False Then EditsMade = True
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module:frmMain Routine: ftb_TextChanged", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub ftb_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        ftbArchive.Validated, ftbDMONFiles.Validated, _
        ftbComment.Validated, ftbFileName.Validated, ftbFileIndex.Validated, ftbFileSize.Validated, _
        ftbNumItems.Validated, ftbSampleReq.Validated, ftbMonitorReq.Validated, ftbRecordReq.Validated, _
        ftbStartValue.Validated, ftbStopValue.Validated, _
        ftbPortNum.Validated, ftbGroup.Validated, ftbAxis.Validated, ftbPrgName.Validated, _
        ftbVarName.Validated, ftbDesc.Validated, ftbUnits.Validated, ftbSlope.Validated, ftbIntercept.Validated
        '********************************************************************************************
        'Description:  generic textbox Validated Routine
        '
        'Parameters: event parameters
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim oT As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
            Dim sText As String = oT.Text
            Dim sName As String = oT.Name

            If DataLoaded = False Then Exit Sub
            Dim nVal As Integer = 0
            If IsNumeric(sText) Then
                nVal = CType(sText, Integer)
            End If
            Dim oItem As clsDMONCfg.tDmonItem = Nothing
            Dim oSchedule As clsDMONCfg.tDmonSchedule = Nothing
            Dim nParm As Integer = cboParm.SelectedIndex
            Select Case tabMain.SelectedTab.Name
                Case tabSystem.Name
                Case tabSchedules.Name
                    If mDMONCfg Is Nothing OrElse nParm < 0 Then
                        Exit Sub
                    End If
                    oSchedule = mDMONCfg.Schedules(nParm)
                Case tabItems.Name
                    If mDMONCfg Is Nothing OrElse nParm < 0 Then
                        Exit Sub
                    End If
                    oItem = mDMONCfg.Items(nParm)
            End Select
            Select Case sName
                'System Tab
                Case ftbDMONFiles.Name
                    moMaint.DiagnosticDaysToKeep = nVal
                    If moMaint.oDiagnosticDaysToKeep.Changed Then EditsMade = True
                Case ftbArchive.Name
                    moMaint.DiagnosticArchiveDaysToKeep = nVal
                    If moMaint.oDiagnosticArchiveDaysToKeep.Changed Then EditsMade = True
                    'Schedule Tab
                Case ftbComment.Name
                    oSchedule.COMMENT.Text = oT.Text
                    If oSchedule.COMMENT.Changed Then EditsMade = True
                Case ftbFileName.Name
                    oSchedule.FILE_NAME.Text = oT.Text
                    If oSchedule.FILE_NAME.Changed Then EditsMade = True
                Case ftbFileIndex.Name
                    oSchedule.FILE_INDEX.Value = CType(oT.Text, Integer)
                    If oSchedule.FILE_INDEX.Changed Then EditsMade = True
                Case ftbFileSize.Name
                    oSchedule.FILE_SIZE.Value = CType(oT.Text, Integer)
                    If oSchedule.FILE_SIZE.Changed Then EditsMade = True
                Case ftbNumItems.Name
                    oSchedule.NUM_ITEMS.Value = CType(oT.Text, Integer)
                    If oSchedule.NUM_ITEMS.Changed Then EditsMade = True
                Case ftbSampleReq.Name
                    oSchedule.SAMP_REQ.Value = CType(oT.Text, Single)
                    If oSchedule.SAMP_REQ.Changed Then EditsMade = True
                Case ftbMonitorReq.Name
                    oSchedule.MNTR_REQ.Value = CType(oT.Text, Single)
                    If oSchedule.MNTR_REQ.Changed Then EditsMade = True
                Case ftbRecordReq.Name
                    oSchedule.REC_REQ.Value = CType(oT.Text, Single)
                    If oSchedule.REC_REQ.Changed Then EditsMade = True
                Case ftbStartValue.Name
                    oSchedule.START_VALUE.Value = CType(oT.Text, Single)
                    If oSchedule.START_VALUE.Changed Then EditsMade = True
                Case ftbStopValue.Name
                    oSchedule.STOP_VALUE.Value = CType(oT.Text, Single)
                    If oSchedule.STOP_VALUE.Changed Then EditsMade = True
                    'Item tab
                Case ftbPortNum.Name
                    oItem.PORT_NUM.Value = CType(oT.Text, Integer)
                    If oItem.PORT_NUM.Changed Then EditsMade = True
                    Select Case oItem.TYPE.Value
                        Case clsDMONCfg.eTYPE.Register
                            'Robot Register lookup
                            Try
                                If cboRobot.SelectedIndex > -1 AndAlso cboParm.SelectedIndex > -1 Then
                                    For Each oController As clsController In colControllers
                                        If oController.Name = msControllerName Then
                                            Dim nReg As Integer = oItem.PORT_NUM.Value
                                            Dim oReg As FRRobot.FRCRegNumeric = oGetRegister(oController.Robot, nReg)
                                            If oReg IsNot Nothing Then
                                                lblPortNumComment.Text = oReg.Comment
                                            End If
                                        End If
                                    Next
                                End If
                            Catch ex As Exception
                            End Try
                        Case Else
                            lblPortNumComment.Text = String.Empty
                    End Select
                Case ftbGroup.Name
                    oItem.GROUP_NUM.Value = CType(oT.Text, Integer)
                    If oItem.GROUP_NUM.Changed Then EditsMade = True
                Case ftbAxis.Name
                    oItem.AXIS_NUM.Value = CType(oT.Text, Integer)
                    If oItem.AXIS_NUM.Changed Then EditsMade = True
                Case ftbPrgName.Name
                    oItem.PRG_NAME.Text = oT.Text
                    If oItem.PRG_NAME.Changed Then EditsMade = True
                Case ftbVarName.Name
                    oItem.VAR_NAME.Text = oT.Text
                    If oItem.VAR_NAME.Changed Then EditsMade = True
                Case ftbDesc.Name
                    oItem.DESC.Text = oT.Text
                    If oItem.DESC.Changed Then EditsMade = True
                Case ftbUnits.Name
                    oItem.UNITS.Text = oT.Text
                    If oItem.UNITS.Changed Then EditsMade = True
                Case ftbSlope.Name
                    oItem.SLOPE.Value = CType(oT.Text, Single)
                    If oItem.SLOPE.Changed Then EditsMade = True
                Case ftbIntercept.Name
                    oItem.INTERCEPT.Value = CType(oT.Text, Single)
                    If oItem.INTERCEPT.Changed Then EditsMade = True
                Case Else
                    'Forgot something
                    Debug.Assert(False)
            End Select
            EditsMade = True
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module:frmMain Routine: ftb_Validated", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub


    Private Sub ftb_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles _
        ftbArchive.Validating, ftbDMONFiles.Validating, _
        ftbComment.Validating, ftbFileName.Validating, ftbFileIndex.Validating, ftbFileSize.Validating, _
        ftbNumItems.Validating, ftbSampleReq.Validating, ftbMonitorReq.Validating, ftbRecordReq.Validating, _
        ftbStartValue.Validating, ftbStopValue.Validating, _
        ftbPortNum.Validating, ftbGroup.Validating, ftbAxis.Validating, ftbPrgName.Validating, _
        ftbVarName.Validating, ftbDesc.Validating, ftbUnits.Validating, ftbSlope.Validating, ftbIntercept.Validating
        '********************************************************************************************
        'Description:  generic textbox Validating Routine
        '
        'Parameters: event parameters
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim oT As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
            Dim sText As String = oT.Text
            Dim bBadData As Boolean = False
            Dim sName As String = oT.Name
            If Privilege = ePrivilege.None Then
                e.Cancel = True
                Exit Sub
            End If
            Dim bAllowNothing As Boolean = False
            Dim oIntVal As clsIntValue = Nothing
            Dim oSngVal As clsSngValue = Nothing
            Dim oTextVal As clsTextValue = Nothing
            Select Case oT.Name
                'System Tab
                Case ftbDMONFiles.Name
                    oIntVal = moMaint.oDiagnosticDaysToKeep
                Case ftbArchive.Name
                    oIntVal = moMaint.oDiagnosticArchiveDaysToKeep
                    'Schedule Tab
                Case ftbComment.Name
                    'Text, no number rules
                    oTextVal = mDMONCfg.Schedules(cboParm.SelectedIndex).COMMENT
                    bAllowNothing = True
                Case ftbFileName.Name
                    oTextVal = mDMONCfg.Schedules(cboParm.SelectedIndex).FILE_NAME
                Case ftbFileIndex.Name
                    oIntVal = mDMONCfg.Schedules(cboParm.SelectedIndex).FILE_INDEX
                Case ftbFileSize.Name
                    oIntVal = mDMONCfg.Schedules(cboParm.SelectedIndex).FILE_SIZE
                Case ftbNumItems.Name
                    oIntVal = mDMONCfg.Schedules(cboParm.SelectedIndex).NUM_ITEMS
                Case ftbSampleReq.Name
                    oSngVal = mDMONCfg.Schedules(cboParm.SelectedIndex).SAMP_REQ
                Case ftbMonitorReq.Name
                    oSngVal = mDMONCfg.Schedules(cboParm.SelectedIndex).MNTR_REQ
                Case ftbRecordReq.Name
                    oSngVal = mDMONCfg.Schedules(cboParm.SelectedIndex).REC_REQ
                Case ftbStartValue.Name
                    oSngVal = mDMONCfg.Schedules(cboParm.SelectedIndex).START_VALUE
                Case ftbStopValue.Name
                    oSngVal = mDMONCfg.Schedules(cboParm.SelectedIndex).STOP_VALUE
                    'Item tab
                Case ftbPortNum.Name
                    oIntVal = mDMONCfg.Items(cboParm.SelectedIndex).PORT_NUM
                Case ftbGroup.Name
                    oIntVal = mDMONCfg.Items(cboParm.SelectedIndex).GROUP_NUM
                Case ftbAxis.Name
                    oIntVal = mDMONCfg.Items(cboParm.SelectedIndex).AXIS_NUM
                Case ftbPrgName.Name
                    oTextVal = mDMONCfg.Items(cboParm.SelectedIndex).PRG_NAME
                Case ftbVarName.Name
                    oTextVal = mDMONCfg.Items(cboParm.SelectedIndex).VAR_NAME
                Case ftbDesc.Name
                    oTextVal = mDMONCfg.Items(cboParm.SelectedIndex).DESC
                    bAllowNothing = True
                Case ftbUnits.Name
                    oTextVal = mDMONCfg.Items(cboParm.SelectedIndex).UNITS
                    bAllowNothing = True
                Case ftbSlope.Name
                    oSngVal = mDMONCfg.Items(cboParm.SelectedIndex).SLOPE
                Case ftbIntercept.Name
                    oSngVal = mDMONCfg.Items(cboParm.SelectedIndex).INTERCEPT
                Case Else
                    'Forgot something
                    Debug.Assert(False)
            End Select
            'no value?
            If (bAllowNothing = False) And (Strings.Len(sText) = 0) Then
                bBadData = True
                MessageBox.Show(gcsRM.GetString("csNO_NULL"), gcsRM.GetString("csINVALID_DATA"), _
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
            If oT.NumericOnly Then
                If Not (IsNumeric(sText)) Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csBAD_ENTRY"), gcsRM.GetString("csINVALID_DATA"), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                ' limit check
                If bBadData = False Then
                    Try
                        If oIntVal IsNot Nothing Then
                            'Switch to decimal on text conversion so we don't bomb out on big numbers
                            'low limit
                            If CType(sText, Decimal) < oIntVal.MinValue Then
                                bBadData = True
                                MessageBox.Show(gcsRM.GetString("csVAL_BELOW_MIN") & vbCrLf & gcsRM.GetString("csMINIMUM_EQ") & oIntVal.MinValue, _
                                                gcsRM.GetString("csINVALID_DATA"), MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            End If
                            'high limit
                            If CType(sText, Decimal) > oIntVal.MaxValue Then
                                bBadData = True
                                MessageBox.Show(gcsRM.GetString("csVAL_ABOVE_MAX") & vbCrLf & gcsRM.GetString("csMAXIMUM_EQ") & oIntVal.MaxValue, _
                                                gcsRM.GetString("csINVALID_DATA"), MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            End If
                        ElseIf oSngVal IsNot Nothing Then
                            'low limit
                            If CType(sText, Decimal) < oSngVal.MinValue Then
                                bBadData = True
                                MessageBox.Show(gcsRM.GetString("csVAL_BELOW_MIN") & vbCrLf & gcsRM.GetString("csMINIMUM_EQ") & oSngVal.MinValue, _
                                                gcsRM.GetString("csINVALID_DATA"), MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            End If
                            'high limit
                            If CType(sText, Decimal) > oSngVal.MaxValue Then
                                bBadData = True
                                MessageBox.Show(gcsRM.GetString("csVAL_ABOVE_MAX") & vbCrLf & gcsRM.GetString("csMAXIMUM_EQ") & oSngVal.MaxValue, _
                                                gcsRM.GetString("csINVALID_DATA"), MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            End If
                        End If
                    Catch ex As Exception
                        'can't convert value to decimal.  
                        bBadData = True
                        MessageBox.Show(gcsRM.GetString("csBAD_ENTRY"), gcsRM.GetString("csINVALID_DATA"), _
                                       MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End Try
                End If '  bBadData = False 

            Else
                'No checks for text here, I guess
            End If
            If bBadData Then
                If oIntVal IsNot Nothing Then
                    subSetFtb(oT, oIntVal)
                ElseIf oSngVal IsNot Nothing Then
                    subSetFtb(oT, oSngVal)
                ElseIf oTextVal IsNot Nothing Then
                    subSetFtb(oT, oTextVal)
                Else
                    'Shouldn't get here
                    Debug.Assert(False)
                End If
                e.Cancel = True
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module:frmMain Routine: ftb_Validating", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
    Private Sub chkRC_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
        chkRC_1.CheckedChanged, chkRC_2.CheckedChanged, chkRC_3.CheckedChanged, chkRC_4.CheckedChanged, _
        chkRC_5.CheckedChanged, chkRC_6.CheckedChanged, chkRC_7.CheckedChanged, chkRC_8.CheckedChanged, _
        chkRC_9.CheckedChanged, chkRC_10.CheckedChanged, chkRC_11.CheckedChanged, chkRC_12.CheckedChanged
        '********************************************************************************************
        'Description:  system tab checkbox change routine
        '
        'Parameters: event parameters
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEventBlocker Then
            Exit Sub
        End If
        Dim chkRC As CheckBox = DirectCast(sender, CheckBox)
        Dim sName As String = chkRC.Name
        Dim nRobot As Integer = CType(sName.Substring(6), Integer)
        Dim cboEnable As ComboBox = DirectCast(tabSystem.Controls("cboEnable" & nRobot.ToString), ComboBox)
        Dim cboSchedule As ComboBox = DirectCast(tabSystem.Controls("cboSchedule" & nRobot.ToString), ComboBox)
        Dim oDMON As clsDMONCfg = colControllers(nRobot - 1).DMONCfg
        If oDMON.UsePT Then
            If oDMON.AutoSample.Value Or oDMON.EnableCCDiag Then
                oDMON.AutoSample.Value = False
                oDMON.EnableCCDiag = False
                oDMON.AsyncEnab.Value = False
                oDMON.PLCEnable.Value = False
                mbEventBlocker = True
                cboEnable.SelectedIndex = 0
                mbEventBlocker = False
            Else
                If oDMON.UseCC Then
                    Dim nSchedOffset As Integer = cboSchedule.SelectedIndex Mod 5
                    If nSchedOffset = 4 Then
                        oDMON.AutoSample.Value = False
                        oDMON.EnableCCDiag = True
                        oDMON.AsyncEnab.Value = True
                        mbEventBlocker = True
                        cboEnable.SelectedIndex = 1
                        mbEventBlocker = False
                    Else
                        oDMON.AutoSample.Value = True
                        oDMON.EnableCCDiag = False
                        oDMON.AsyncEnab.Value = False
                        mbEventBlocker = True
                        cboEnable.SelectedIndex = 0
                        mbEventBlocker = False
                    End If
                Else
                    oDMON.AutoSample.Value = True
                    oDMON.EnableCCDiag = False
                    oDMON.AsyncEnab.Value = False
                    mbEventBlocker = True
                    cboEnable.SelectedIndex = 0
                    mbEventBlocker = False
                End If
                oDMON.PLCEnable.Value = True
            End If
        Else
            'Just toggle the PLC value
            oDMON.PLCEnable.Value = Not (oDMON.PLCEnable.Value)
        End If

        If (oDMON.UsePT AndAlso (oDMON.AutoSample.Changed Or oDMON.AsyncEnab.Changed Or oDMON.EnableCCDiagChanged)) Or _
         ((mcolZones.StandAlone = False) AndAlso oDMON.PLCEnable.Changed) Then
            chkRC.ForeColor = Color.Red
            cboEnable.ForeColor = Color.Red
            cboEnable.Refresh()
            EditsMade = True
        Else
            chkRC.ForeColor = Color.Black
            cboEnable.ForeColor = Color.Black
        End If

    End Sub
    Private Sub cboTabSystem_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        cboEnable1.SelectionChangeCommitted, cboSchedule1.SelectionChangeCommitted, _
        cboEnable2.SelectionChangeCommitted, cboSchedule2.SelectionChangeCommitted, _
        cboEnable3.SelectionChangeCommitted, cboSchedule3.SelectionChangeCommitted, _
        cboEnable4.SelectionChangeCommitted, cboSchedule4.SelectionChangeCommitted, _
        cboEnable5.SelectionChangeCommitted, cboSchedule5.SelectionChangeCommitted, _
        cboEnable6.SelectionChangeCommitted, cboSchedule6.SelectionChangeCommitted, _
        cboEnable7.SelectionChangeCommitted, cboSchedule7.SelectionChangeCommitted, _
        cboEnable8.SelectionChangeCommitted, cboSchedule8.SelectionChangeCommitted, _
        cboEnable9.SelectionChangeCommitted, cboSchedule9.SelectionChangeCommitted, _
        cboEnable10.SelectionChangeCommitted, cboSchedule10.SelectionChangeCommitted, _
        cboEnable11.SelectionChangeCommitted, cboSchedule11.SelectionChangeCommitted, _
        cboEnable12.SelectionChangeCommitted, cboSchedule12.SelectionChangeCommitted
        '********************************************************************************************
        'Description:  system tab cbo validation routine
        '
        'Parameters: event parameters
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If mbEventBlocker Then Exit Sub
            Dim oCbo As ComboBox = DirectCast(sender, ComboBox)
            Dim sName As String = oCbo.Name
            Dim nRobot As Integer


            Dim sCol As String
            If InStr(sName, "cboEnable") > 0 Then
                nRobot = CType(sName.Substring(9), Integer)
                sCol = "cboEnable"
            Else
                nRobot = CType(sName.Substring(11), Integer)
                sCol = "cboSchedule"
            End If
            Dim chkRC As CheckBox = DirectCast(tabSystem.Controls("chkRC_" & nRobot.ToString), CheckBox)
            Dim oDMON As clsDMONCfg = colControllers(nRobot - 1).DMONCfg
            Select Case sCol
                Case "cboEnable"
                    If chkRC.Checked = False Then
                        'These are linked.  The cbo doesn't really mean anything if the chk is off
                        mbEventBlocker = True
                        chkRC.Checked = True
                        mbEventBlocker = False
                    End If
                    Select Case oCbo.SelectedIndex
                        Case 0 'Autosample job
                            oDMON.AutoSample.Value = True
                            oDMON.AsyncEnab.Value = False
                            oDMON.EnableCCDiag = False
                            oDMON.PLCEnable.Value = True
                        Case 1 'autosample cc
                            oDMON.AutoSample.Value = False
                            oDMON.AsyncEnab.Value = True
                            oDMON.EnableCCDiag = True
                            oDMON.PLCEnable.Value = True
                        Case Else
                            If oDMON.AutoSample.Value Then
                                oCbo.SelectedIndex = 0
                                oDMON.AsyncEnab.Value = False
                                oDMON.EnableCCDiag = False
                                oDMON.PLCEnable.Value = True
                            ElseIf oDMON.UseCC AndAlso oDMON.EnableCCDiag Then
                                oCbo.SelectedIndex = 1
                                oDMON.AutoSample.Value = False
                                oDMON.AsyncEnab.Value = True
                                oDMON.PLCEnable.Value = True
                            Else
                                oCbo.SelectedIndex = 0
                                oDMON.AsyncEnab.Value = False
                                oDMON.EnableCCDiag = False
                                oDMON.PLCEnable.Value = False
                            End If
                    End Select
                    If (oDMON.UsePT AndAlso (oDMON.AutoSample.Changed Or oDMON.AsyncEnab.Changed Or oDMON.EnableCCDiagChanged)) Or _
                        ((mcolZones.StandAlone = False) AndAlso oDMON.PLCEnable.Changed) Then
                        chkRC.ForeColor = Color.Red
                        oCbo.ForeColor = Color.Red
                        oCbo.Refresh()
                        EditsMade = True
                    Else
                        chkRC.ForeColor = Color.Black
                        oCbo.ForeColor = Color.Black
                    End If
                Case "cboSchedule"
                    If oCbo.SelectedIndex >= 0 Then
                        oDMON.Schedule.Value = oCbo.SelectedIndex + 1
                    Else
                        'The only invalid value is nothing.  Set it to the existing value
                        oCbo.SelectedIndex = oDMON.Schedule.Value - 1
                    End If
                    If oDMON.Schedule.Changed Then
                        oCbo.ForeColor = Color.Red
                        oCbo.Refresh()
                        EditsMade = True
                    Else
                        oCbo.ForeColor = Color.Black
                    End If
            End Select
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module:frmMain Routine: cboTabSystem_SelectionChangeCommitted", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub cboTabItemTabSchedule_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        cboFileDevice.SelectionChangeCommitted, cboRecordMode.SelectionChangeCommitted, _
        cboStartItem.SelectionChangeCommitted, cboStartCond.SelectionChangeCommitted, _
        cboStopItem.SelectionChangeCommitted, cboStopCond.SelectionChangeCommitted, _
        cboItem1.SelectionChangeCommitted, cboItem2.SelectionChangeCommitted, _
        cboItem3.SelectionChangeCommitted, cboItem4.SelectionChangeCommitted, _
        cboItem5.SelectionChangeCommitted, cboItem6.SelectionChangeCommitted, _
        cboItem7.SelectionChangeCommitted, cboItem8.SelectionChangeCommitted, _
        cboItem9.SelectionChangeCommitted, cboItem10.SelectionChangeCommitted, _
        cboType.SelectionChangeCommitted, cboIoType.SelectionChangeCommitted, cboFileDevice.TextChanged
        '********************************************************************************************
        'Description:  schedule and item tab cbo validation routine
        '
        'Parameters: event parameters
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If mbEventBlocker Then Exit Sub
            Dim oCbo As ComboBox = DirectCast(sender, ComboBox)
            Dim sName As String = oCbo.Name
            Dim oIntVal As clsIntValue = Nothing
            Dim nMinVal As Integer = 0
            Dim bString As Boolean = False
            If sName.Substring(0, 7).ToLower = "cboitem" Then
                'sort out the item list index
                Dim nItemNumber As Integer = CType(sName.Substring(7), Integer)
                oIntVal = mDMONCfg.Schedules(cboParm.SelectedIndex).ITEM(nItemNumber - 1)
            Else
                'every thing else uses the case statement
                Select Case sName
                    Case cboFileDevice.Name
                        oIntVal = mDMONCfg.Schedules(cboParm.SelectedIndex).DEVICE
                        If oIntVal.Value = -1 Then
                            mDMONCfg.Schedules(cboParm.SelectedIndex).DEVICE_NAME.Text = oCbo.Text
                            bString = True
                        End If
                    Case cboRecordMode.Name
                        oIntVal = mDMONCfg.Schedules(cboParm.SelectedIndex).REC_MODE
                    Case cboStartItem.Name
                        oIntVal = mDMONCfg.Schedules(cboParm.SelectedIndex).START_ITEM
                    Case cboStartCond.Name
                        oIntVal = mDMONCfg.Schedules(cboParm.SelectedIndex).START_COND
                    Case cboStopItem.Name
                        oIntVal = mDMONCfg.Schedules(cboParm.SelectedIndex).STOP_ITEM
                    Case cboStopCond.Name
                        oIntVal = mDMONCfg.Schedules(cboParm.SelectedIndex).STOP_COND
                    Case cboType.Name
                        oIntVal = mDMONCfg.Items(cboParm.SelectedIndex).TYPE
                    Case cboIoType.Name
                        oIntVal = mDMONCfg.Items(cboParm.SelectedIndex).IO_TYPE
                        lblPortNumComment.Text = String.Empty
                End Select
            End If
            If bString Then
                If mDMONCfg.Schedules(cboParm.SelectedIndex).DEVICE_NAME.Changed Then
                    cboFileDevice.ForeColor = Color.Red
                Else
                    cboFileDevice.ForeColor = Color.Black
                End If
            Else
                Dim nTag() As Integer = CType(oCbo.Tag, Integer())
                If nTag Is Nothing Then
                    If oCbo.SelectedIndex >= 0 Then
                        oIntVal.Value = oCbo.SelectedIndex + oIntVal.MinValue
                    Else
                        'The only invalid value is nothing.  Set it to the existing value
                        oCbo.SelectedIndex = oIntVal.Value - oIntVal.MinValue
                    End If
                Else

                    If oCbo.SelectedIndex > -1 Then
                        oIntVal.Value = nTag(oCbo.SelectedIndex)
                    End If
                End If
                subSetCbo(oCbo, oIntVal)
            End If
            EditsMade = True
            subEnableControls(True)
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module:frmMain Routine: cboTabItemTabSchedule_SelectionChangeCommitted", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub cboRobot_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboRobot.SelectionChangeCommitted
        '********************************************************************************************
        'Description:  New robot selected
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mnRobot <> cboRobot.SelectedIndex Then
            If EditsMade Then
                If Not bAskForSave() Then
                    cboRobot.SelectedIndex = mnRobot
                    Exit Sub
                End If
            End If
        End If
        msControllerName = cboRobot.Text
        mnRobot = cboRobot.SelectedIndex
        subLoadData()
    End Sub

    Private Sub cboParm_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboParm.SelectionChangeCommitted
        '********************************************************************************************
        'Description:  New item or schedule selected 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mnParm <> cboParm.SelectedIndex Then
            'If EditsMade Then
            '    If Not bAskForSave() Then
            '        cboParm.SelectedIndex = mnParm
            '        Exit Sub
            '    End If
            'End If
        End If
        If cboParm.SelectedIndex > -1 Then
            subShowDetailPage()
            mnParm = cboParm.SelectedIndex
        End If
    End Sub
    Private Sub tabMain_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabMain.SelectedIndexChanged
        '********************************************************************************************
        'Description:  New tab selected
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEventBlocker Then Exit Sub
        msTabName = tabMain.SelectedTab.Name
        oCurTab = tabMain.SelectedTab
        Select Case tabMain.SelectedTab.Name
            Case tabSystem.Name
                subLoadData()
            Case Else
                If cboRobot.Visible = False Then
                    DataLoaded = False
                    If cboRobot.SelectedIndex > -1 Then
                        subLoadData()
                    Else
                        subShowNewPage(True)
                    End If
                Else
                    subShowNewPage(True)
                End If
        End Select
    End Sub

    Private Sub tabMain_Selecting(ByVal sender As Object, ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles tabMain.Selecting
        '********************************************************************************************
        'Description:  New tab selected. check for save here while we can still cancel the selection
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If (tabMain.SelectedTab.Name = tabSystem.Name) Or _
            (msTabName = tabSystem.Name) Then
            If EditsMade AndAlso bAskForSave() = False Then
                e.Cancel = True
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

    Private Function sFillIn(ByVal sText As String, ByVal bExportCSV As Boolean) As String
        If (sText.Trim = String.Empty) And (bExportCSV = False) Then
            Return "&nbsp;"
        Else
            Return sText
        End If
    End Function
    Private Function bPrintdoc(ByVal bPrint As Boolean, Optional ByVal bExportCSV As Boolean = False, Optional ByRef sFile As String = "") As Boolean
        '********************************************************************************************
        'Description:  build the print document
        '
        'Parameters:  bPrint - send to printer, bExportCSV - export to CSV, sFile - force export path
        'Returns:   true if successful, OK to continue with preview or page setup
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/03/12  MSW     various version compatibility fixes
        '********************************************************************************************
        Try

            mPrintHtml.subSetPageFormat()
            mPrintHtml.subClearTableFormat()
            mPrintHtml.subSetColumnCfg("align=right", 0)

            mPrintHtml.HeaderRowsPerTable = 1
            Dim bAll As Boolean
            If bExportCSV Then
                bAll = True
            Else
                bAll = mnuPrintAll.Checked
            End If

            Dim sPageTitle As String = gpsRM.GetString("psSCREENCAPTION") & " - " & tabMain.SelectedTab.Text
            Dim sTitle(1) As String
            sTitle(0) = mcolZones.SiteName & " - " & mcolZones.CurrentZone
            sTitle(1) = String.Empty

            Dim sSubTitle(0) As String
            sSubTitle(0) = String.Empty
            Dim sText() As String = Nothing
            'Select CC or color presets
            Dim oPresets As clsPresets = Nothing

            Dim bCancel As Boolean = False

            Select Case tabMain.SelectedTab.Name
                Case tabSystem.Name
                    sSubTitle(0) = String.Empty
                    ReDim sText(2)
                    sText(0) = " " & vbTab & lblDaysToKeep.Text
                    sText(1) = lblDMONFiles.Text & vbTab & ftbDMONFiles.Text
                    sText(2) = lblArchive.Text & vbTab & ftbArchive.Text
                    mPrintHtml.subStartDoc(Status, sPageTitle, bExportCSV, bCancel, sFile)
                    If bCancel = False Then
                        mPrintHtml.subSetRowcfg("Bold=on", 0, 0)
                        mPrintHtml.HeaderRowsPerTable = 1
                        mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle)
                        ReDim sText(colControllers.Count)
                        sText(0) = lblRobot.Text & vbTab & lblEnableCap.Text & vbTab & lblJobOrCC.Text & vbTab & lblScheduleCap.Text
                        For nRobot As Integer = 1 To colControllers.Count
                            Dim chkRC As CheckBox = DirectCast(tabSystem.Controls("chkRC_" & nRobot.ToString), CheckBox)
                            Dim cboEnable As ComboBox = DirectCast(tabSystem.Controls("cboEnable" & nRobot.ToString), ComboBox)
                            Dim cboSchedule As ComboBox = DirectCast(tabSystem.Controls("cboSchedule" & nRobot.ToString), ComboBox)

                            sText(nRobot) = chkRC.Text & vbTab & chkRC.Checked & vbTab & cboEnable.Text & vbTab & cboSchedule.Text
                        Next
                        If bCancel = False Then
                            mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle)
                            mPrintHtml.subCloseFile(Status)
                            If bPrint Then
                                mPrintHtml.subPrintDoc()
                            End If
                        End If
                    End If
                    Return (bCancel = False)
                Case tabSchedules.Name
                    ReDim sSubTitle(1)
                    sSubTitle(0) = lblRobot.Text & ": " & msControllerName
                    If bAll Then
                        sSubTitle(1) = gpsRM.GetString("psDMON") & gpsRM.GetString("psSCHEDULES")
                        ReDim sText(mDMONCfg.NumSchedules)
                        sText(0) = lblParm.Text & vbTab & lblComment.Text & vbTab & lblFileDevice.Text & vbTab & lblFileName.Text & vbTab & _
                            lblFileIndex.Text & vbTab & lblFileSize.Text & vbTab & lblNumItems.Text & vbTab & _
                            lblSample.Text & " " & lblReq.Text & "-" & lblAct.Text & vbTab & _
                            lblMonitor.Text & " " & lblReq.Text & "-" & lblAct.Text & vbTab & _
                            lblRecord.Text & " " & lblReq.Text & "-" & lblAct.Text & vbTab & _
                            lblRecordMode.Text & vbTab & chkStartItem.Text & vbTab & chkStopItem.Text & vbTab & _
                            gpsRM.GetString("psITEMS")
                        For nSchedule As Integer = 1 To mDMONCfg.NumSchedules
                            With mDMONCfg.Schedules(nSchedule - 1)
                                'The robot may leave invalid data in here
                                Dim nDevice As Integer = .DEVICE.Value
                                Dim sDevice As String = String.Empty
                                If .DEVICE.Value = -1 Then
                                    sDevice = .DEVICE_NAME.Text
                                Else
                                If nDevice < 1 Then
                                    nDevice = 1
                                End If
                                    sDevice = cboFileDevice.Items(nDevice - 1).ToString
                                End If
                                sText(nSchedule) = nSchedule.ToString & vbTab & .COMMENT.Text & vbTab & _
                                sDevice & vbTab & _
                                .FILE_NAME.Text & vbTab & .FILE_INDEX.Value.ToString & vbTab & _
                                .FILE_SIZE.Value.ToString & lblKB.Text & vbTab & _
                                .NUM_ITEMS.Value.ToString & vbTab & _
                                .SAMP_REQ.Value.ToString & ":" & .SAMP_RATE.ToString & vbTab & _
                                .MNTR_REQ.Value.ToString & ":" & .MNTR_RATE.ToString & vbTab & _
                                .REC_REQ.Value.ToString & ":" & .REC_PER_SEC.ToString & vbTab & _
                                cboRecordMode.Items(.REC_MODE.Value - 1).ToString & vbTab
                                If .START_ENBL.Value Then
                                    sText(nSchedule) = sText(nSchedule) & .START_ITEM.Value.ToString & ":" & _
                                            mDMONCfg.Items(.START_ITEM.Value - 1).DESC.Text & _
                                            " " & cboStartCond.Items(.START_COND.Value - 1).ToString & " " & _
                                            .START_VALUE.Value.ToString & vbTab
                                Else
                                    sText(nSchedule) = sText(nSchedule) & gpsRM.GetString("psDISABLED") & vbTab
                                End If
                                If .STOP_ENBL.Value Then
                                    sText(nSchedule) = sText(nSchedule) & .STOP_ITEM.Value.ToString & ":" & _
                                            mDMONCfg.Items(.STOP_ITEM.Value - 1).DESC.Text & _
                                            " " & cboStartCond.Items(.STOP_COND.Value - 1).ToString & " " & _
                                            .STOP_VALUE.Value.ToString & vbTab
                                Else
                                    sText(nSchedule) = sText(nSchedule) & gpsRM.GetString("psDISABLED") & vbTab
                                End If
                                Dim sTmp As String = String.Empty
                                Dim sSep As String = "; "
                                For nItem As Integer = 1 To .NUM_ITEMS.Value
                                    sTmp = sTmp & .ITEM(nItem - 1).Value.ToString
                                    If nItem < .NUM_ITEMS.Value Then
                                        sTmp = sTmp & sSep
                                    End If
                                Next
                                sText(nSchedule) = sText(nSchedule) & sTmp
                            End With
                        Next
                        mPrintHtml.subStartDoc(Status, sPageTitle, bExportCSV, bCancel, sFile)
                    Else
                        sSubTitle(1) = gpsRM.GetString("psDMON") & gpsRM.GetString("psSCHEDULE") & ": " & cboParm.Text
                        sPageTitle = gpsRM.GetString("psSCREENCAPTION") & " - " & msControllerName & " - " & sSubTitle(1)
                        ReDim sText(12)
                        With mDMONCfg.Schedules(cboParm.SelectedIndex)
                            sText(0) = lblComment.Text & vbTab & .COMMENT.Text
                            Dim nDevice As Integer = .DEVICE.Value
                            Dim sDevice As String = String.Empty
                            If .DEVICE.Value = -1 Then
                                sDevice = .DEVICE_NAME.Text
                            Else
                                If nDevice < 1 Then
                                    nDevice = 1
                                End If
                                sDevice = cboFileDevice.Items(nDevice - 1).ToString
                            End If
                            sText(1) = lblFileDevice.Text & vbTab & sDevice
                            sText(2) = lblFileName.Text & vbTab & .FILE_NAME.Text
                            sText(3) = lblFileIndex.Text & vbTab & .FILE_INDEX.Value.ToString
                            sText(4) = lblFileSize.Text & vbTab & .FILE_SIZE.Value.ToString
                            sText(5) = lblNumItems.Text & vbTab & .NUM_ITEMS.Value.ToString
                            sText(6) = lblSample.Text & " " & lblReq.Text & "-" & lblAct.Text & vbTab & _
                                .SAMP_REQ.Value.ToString & ":" & .SAMP_RATE.ToString
                            sText(7) = lblMonitor.Text & " " & lblReq.Text & "-" & lblAct.Text & vbTab & _
                                .MNTR_REQ.Value.ToString & ":" & .MNTR_RATE.ToString
                            sText(8) = lblRecord.Text & " " & lblReq.Text & "-" & lblAct.Text & vbTab & _
                                    .REC_REQ.Value.ToString & ":" & .REC_PER_SEC.ToString
                            sText(9) = lblRecordMode.Text & vbTab & cboRecordMode.Items(.REC_MODE.Value - 1).ToString
                            sText(10) = chkStartItem.Text & vbTab
                            If .START_ENBL.Value Then
                                sText(10) = sText(10) & .START_ITEM.Value.ToString & ":" & _
                                mDMONCfg.Items(.START_ITEM.Value - 1).DESC.Text & _
                                " " & cboStartCond.Items(.START_COND.Value - 1).ToString & " " & _
                                .START_VALUE.Value.ToString()
                            Else
                                sText(10) = sText(10) & gpsRM.GetString("psDISABLED")
                            End If
                            sText(11) = chkStopItem.Text & vbTab
                            If .STOP_ENBL.Value Then
                                sText(11) = sText(11) & .STOP_ITEM.Value.ToString & ":" & _
                                mDMONCfg.Items(.STOP_ITEM.Value - 1).DESC.Text & _
                                " " & cboStartCond.Items(.STOP_COND.Value - 1).ToString & " " & _
                                .STOP_VALUE.Value.ToString
                            Else
                                sText(11) = sText(11) & gpsRM.GetString("psDISABLED")
                            End If
                            sText(12) = gpsRM.GetString("psITEMS") & vbTab
                            Dim sTmp As String = String.Empty
                            Dim sSep As String = "; "
                            For nItem As Integer = 1 To .NUM_ITEMS.Value
                                sTmp = sTmp & .ITEM(nItem - 1).Value.ToString
                                If nItem < .NUM_ITEMS.Value Then
                                    sTmp = sTmp & sSep
                                End If
                            Next
                            sText(12) = sText(12) & sTmp
                        End With
                        mPrintHtml.subStartDoc(Status, sPageTitle, bExportCSV, bCancel, sFile)
                        mPrintHtml.subSetRowcfg("Bold=off", 0, 0)
                        mPrintHtml.HeaderRowsPerTable = 0
                    End If
                    If bCancel = False Then
                        mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle)
                        mPrintHtml.subCloseFile(Status)
                        If bPrint Then
                            mPrintHtml.subPrintDoc()
                        End If
                    End If
                    Return (bCancel = False)
                Case tabItems.Name
                    ReDim sSubTitle(1)
                    sSubTitle(0) = lblRobot.Text & ": " & msControllerName
                    If bAll Then
                        sSubTitle(1) = gpsRM.GetString("psDMON") & gpsRM.GetString("psITEMS")
                        ReDim sText(mDMONCfg.NumItems)
                        sText(0) = lblParm.Text & vbTab & _
                                lblDesc.Text & vbTab & _
                                lblType.Text & vbTab & _
                                lblIoType.Text & vbTab & _
                                lblPortNum.Text & vbTab
                        If mDMONCfg.DisplayItemAxis Then
                            sText(0) = sText(0) & lblAxis.Text & vbTab
                        End If
                        sText(0) = sText(0) & lblPrgName.Text & vbTab & _
                                lblVarName.Text & vbTab & _
                                lblUnits.Text & vbTab & _
                                lblSlope.Text & vbTab & _
                                lblIntercept.Text
                        For nItem As Integer = 1 To mDMONCfg.NumItems
                            With mDMONCfg.Items(nItem - 1)
                                Dim sType As String = mDMONCfg.sGetTYPEString(DirectCast(.TYPE.Value, clsDMONCfg.eTYPE))
                                Dim sIoType As String = mDMONCfg.sGetIOTYPEString(DirectCast(.IO_TYPE.Value, clsDMONCfg.eIO_TYPE))
                                sText(nItem) = nItem.ToString & vbTab & sFillIn(.DESC.Text, bExportCSV) & vbTab & _
                                    sFillIn(sType, bExportCSV) & vbTab & _
                                    sFillIn(sIoType, bExportCSV) & vbTab & _
                                    .PORT_NUM.Value.ToString & vbTab
                                If mDMONCfg.DisplayItemAxis Then
                                    sText(nItem) = sText(nItem) & lblG.Text & .GROUP_NUM.Value.ToString & " " & lblA.Text & .AXIS_NUM.Value.ToString & vbTab
                                End If
                                sText(nItem) = sText(nItem) & sFillIn(.PRG_NAME.Text, bExportCSV) & vbTab & _
                                    sFillIn(.VAR_NAME.Text, bExportCSV) & vbTab & _
                                    sFillIn(.UNITS.Text, bExportCSV) & vbTab & _
                                    .SLOPE.Value.ToString & vbTab & _
                                    .INTERCEPT.Value.ToString
                            End With
                        Next
                        mPrintHtml.subStartDoc(Status, sPageTitle, bExportCSV, bCancel, sFile)
                    Else
                        sSubTitle(1) = gpsRM.GetString("psDMON") & gpsRM.GetString("psITEM") & ": " & cboParm.Text
                        sPageTitle = gpsRM.GetString("psSCREENCAPTION") & " - " & msControllerName & " - " & sSubTitle(1)
                        ReDim sText(9)
                        Dim nItem As Integer = cboParm.SelectedIndex + 1
                        With mDMONCfg.Items(nItem - 1)
                            sText(0) = lblParm.Text & vbTab & nItem.ToString
                            sText(1) = lblDesc.Text & vbTab & sFillIn(.DESC.Text, bExportCSV)
                            Dim sType As String = mDMONCfg.sGetTYPEString(DirectCast(.TYPE.Value, clsDMONCfg.eTYPE))
                            Dim sIoType As String = mDMONCfg.sGetIOTYPEString(DirectCast(.IO_TYPE.Value, clsDMONCfg.eIO_TYPE))
                            sText(2) = lblType.Text & vbTab & sFillIn(sType, bExportCSV)
                            sText(3) = lblIoType.Text & vbTab & sFillIn(sIoType, bExportCSV)
                            sText(4) = lblPortNum.Text & vbTab & sFillIn(.PORT_NUM.Value.ToString, bExportCSV)
                            Dim nOffset As Integer = 0
                            If mDMONCfg.DisplayItemAxis Then
                                sText(5) = lblAxis.Text & vbTab & lblG.Text & .GROUP_NUM.Value.ToString & " " & lblA.Text & .AXIS_NUM.Value.ToString
                                ReDim Preserve sText(10)
                                nOffset = 1
                            End If
                            sText(4 + nOffset) = lblPrgName.Text & vbTab & sFillIn(.PRG_NAME.Text, bExportCSV)
                            sText(5 + nOffset) = lblVarName.Text & vbTab & sFillIn(.VAR_NAME.Text, bExportCSV)
                            sText(7 + nOffset) = lblUnits.Text & vbTab & sFillIn(.UNITS.Text, bExportCSV)
                            sText(8 + nOffset) = lblSlope.Text & vbTab & .SLOPE.Value.ToString
                            sText(9 + nOffset) = lblIntercept.Text & vbTab & .INTERCEPT.Value.ToString
                        End With
                        mPrintHtml.subStartDoc(Status, sPageTitle, bExportCSV, bCancel, sFile)
                    End If
                    If bCancel = False Then
                        mPrintHtml.subSetRowcfg("Bold=on", 0, 0)
                        mPrintHtml.HeaderRowsPerTable = 1
                        mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle)
                        mPrintHtml.subCloseFile(Status)
                        If bPrint Then
                            mPrintHtml.subPrintDoc()
                        End If
                    End If
                    Return (bCancel = False)
            End Select

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            Status = gcsRM.GetString("csPRINTFAILED")
            Return (False)

        End Try

    End Function
    Private Sub mnuImport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuImport.Click, mnuRestoreDefault.Click
        '********************************************************************************************
        'Description:  Import settings from a csv file
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/07/11  MSW     allow import from and export to csv
        ' 01/03/12  MSW     various version compatibility fixes
 
        '********************************************************************************************
        Const nIMPORT_NONE As Integer = -1
        Const nIMPORT_SCHEDULES As Integer = 0
        Const nIMPORT_ITEMS As Integer = 1
        Dim sFilePath As String = String.Empty
        Try
            Dim oMnu As System.Windows.Forms.ToolStripMenuItem = DirectCast(sender, System.Windows.Forms.ToolStripMenuItem)
            If oMnu.Name = mnuRestoreDefault.Name Then
                Dim sFile As String = String.Format(gpsRM.GetString("psDMON_DEFAULTS_FILE"), tabMain.SelectedTab.Text, cboRobot.Text)
                If GetDefaultFilePath(sFilePath, eDir.XML, String.Empty, sFile) Then
                    'Use sFilePath
                Else
                    Dim sMsg As String = String.Format(gpsRM.GetString("psDEFAULT_NOT_FOUND"), tabMain.SelectedTab.Text, cboRobot.Text)
                    Dim lRet As Response = MessageBox.Show(sMsg, gpsRM.GetString("psDEFAULT_NOT_FOUND_CAP"), MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    If lRet <> Windows.Forms.DialogResult.Yes Then
                        Exit Sub
                    Else
                        sFilePath = String.Empty
                    End If
                End If
            End If
        Catch ex As Exception
            sFilePath = String.Empty
        End Try

        Try
            Dim sTitleReq As String = gpsRM.GetString("psSCREENCAPTION")
            Dim sTableStart(1) As String
            sTableStart(nIMPORT_SCHEDULES) = gpsRM.GetString("psSCHEDULE")
            sTableStart(nIMPORT_ITEMS) = gpsRM.GetString("psITEM")
            Dim sHeaderTmp As String = String.Empty
            Dim oDT As DataTable = Nothing
            ImportExport.GetDTFromCSV(sTitleReq, sTableStart, sHeaderTmp, oDT, sFilePath)
            Dim sHeader() As String = Split(sHeaderTmp, vbTab)
            'Is there anything there?
            If (sHeader IsNot Nothing) AndAlso (oDT IsNot Nothing) AndAlso _
                (sHeader.Length > 0) AndAlso (oDT.Rows.Count > 0) Then
                'Get some details for the prompt
                Dim sZone As String = gpsRM.GetString("psUNKNOWN")
                Dim sDevice As String = gpsRM.GetString("psUNKNOWN")
                'If the header format is exactly how we write it, name the source
                If sHeader.Length = 5 AndAlso sHeader(3).StartsWith(lblRobot.Text) Then
                    'Format fits nicely, identify the source data
                    sZone = sHeader(1)
                    sDevice = sHeader(3).Replace(lblRobot.Text, String.Empty)
                    sDevice = sDevice.Replace(" - ", String.Empty)
                End If
                frmImport.Zone = sZone
                frmImport.Device = sDevice
                frmImport.dgTmp.DataSource = oDT
                'Check for items or schedules, get column number for the index and name
                Dim nImportType As Integer = nIMPORT_NONE
                'Common columns
                Dim nIndexColumn As Integer = -1
                Dim nNameColumn As Integer = -1
                'schedule items
                Dim nFileDevColumn As Integer = -1
                Dim nFileNameColumn As Integer = -1
                Dim nFileIndexColumn As Integer = -1
                Dim nFileSizeColumn As Integer = -1
                Dim nNumItemsColumn As Integer = -1
                Dim nSampleColumn As Integer = -1
                Dim nMonitorColumn As Integer = -1
                Dim nRecordColumn As Integer = -1
                Dim nRecModeColumn As Integer = -1
                Dim nStartItemColumn As Integer = -1
                Dim nStopItemColumn As Integer = -1
                Dim nItemsColumn As Integer = -1
                'Item columns
                Dim nTypeColumn As Integer = -1
                Dim nIOTypeColumn As Integer = -1
                Dim nPortColumn As Integer = -1
                Dim nAxisColumn As Integer = -1
                Dim nProgColumn As Integer = -1
                Dim nVarColumn As Integer = -1
                Dim nUnitsColumn As Integer = -1
                Dim nSlopeColumn As Integer = -1
                Dim nIntColumn As Integer = -1
                For nCol As Integer = 0 To oDT.Columns.Count - 1
                    Select Case nImportType
                        Case nIMPORT_NONE
                            If sTableStart(nIMPORT_SCHEDULES) = oDT.Columns.Item(nCol).ColumnName Then
                                nImportType = nIMPORT_SCHEDULES
                                nIndexColumn = nCol
                                frmImport.MaxToItems = mDMONCfg.NumSchedules
                            End If
                            If sTableStart(nIMPORT_ITEMS) = oDT.Columns.Item(nCol).ColumnName Then
                                nImportType = nIMPORT_ITEMS
                                nIndexColumn = nCol
                                frmImport.MaxToItems = mDMONCfg.NumItems
                            End If
                        Case nIMPORT_SCHEDULES
                            Dim sTmp As String = oDT.Columns.Item(nCol).ColumnName
                            Debug.Print(sTmp)
                            Select Case sTmp
                                Case gpsRM.GetString("psCOMMENT")
                                    nNameColumn = nCol
                                Case gpsRM.GetString("psFILEDEVICE")
                                    nFileDevColumn = nCol
                                Case gpsRM.GetString("psFILENAME")
                                    nFileNameColumn = nCol
                                Case gpsRM.GetString("psFILEINDEX")
                                    nFileIndexColumn = nCol
                                Case gpsRM.GetString("psFILESIZE")
                                    nFileSizeColumn = nCol
                                Case gpsRM.GetString("psNUMITEMS")
                                    nNumItemsColumn = nCol
                                Case lblSample.Text & " " & lblReq.Text & "-" & lblAct.Text
                                    nSampleColumn = nCol
                                Case lblMonitor.Text & " " & lblReq.Text & "-" & lblAct.Text
                                    nMonitorColumn = nCol
                                Case lblRecord.Text & " " & lblReq.Text & "-" & lblAct.Text
                                    nRecordColumn = nCol
                                Case gpsRM.GetString("psRECORDMODE")
                                    nRecModeColumn = nCol
                                Case gpsRM.GetString("psSTARTITEM")
                                    nStartItemColumn = nCol
                                Case gpsRM.GetString("psSTOPITEM")
                                    nStopItemColumn = nCol
                                Case gpsRM.GetString("psITEMS")
                                    nItemsColumn = nCol
                                Case Else
                            End Select
                        Case nIMPORT_ITEMS
                            Select Case oDT.Columns.Item(nCol).ColumnName
                                Case gpsRM.GetString("psDESC")
                                    nNameColumn = nCol
                                Case gpsRM.GetString("psTYPE")
                                    nTypeColumn = nCol
                                Case gpsRM.GetString("psIOTYPE")
                                    nIOTypeColumn = nCol
                                Case gpsRM.GetString("psPORTNUM")
                                    nPortColumn = nCol
                                Case gpsRM.GetString("psAXIS")
                                    nAxisColumn = nCol
                                Case gpsRM.GetString("psPRGNAME")
                                    nProgColumn = nCol
                                Case gpsRM.GetString("psVARNAME")
                                    nVarColumn = nCol
                                Case gpsRM.GetString("psUNITS")
                                    nUnitsColumn = nCol
                                Case gpsRM.GetString("psSLOPE")
                                    nSlopeColumn = nCol
                                Case gpsRM.GetString("psINTERCEPT")
                                    nIntColumn = nCol
                                Case Else
                            End Select
                    End Select

                Next

                'load a cbo on frmImport to select an individual import or single item.
                If nImportType <> nIMPORT_NONE Then
                    Dim nItemCount As Integer = 0
                    frmImport.ClearCboItems()
                    frmImport.AddCboItem(gcsRM.GetString("csALL"), 0)
                    For nItem As Integer = 0 To oDT.Rows.Count - 1
                        Dim sIndex As String = oDT.Rows.Item(nItem).Item(nIndexColumn).ToString
                        If (sIndex <> String.Empty) AndAlso (IsNumeric(sIndex)) Then
                            Dim nIndex As Integer = CType(sIndex, Integer)
                            Dim sText As String = oDT.Rows.Item(nItem).Item(nNameColumn).ToString
                            frmImport.AddCboItem((nIndex.ToString & " - " & sText), nIndex)
                            nItemCount = nItemCount + 1
                        End If
                    Next
                    frmImport.FromItem = 0

                    'Fill out the labels
                    frmImport.label(1) = String.Format(gpsRM.GetString("psIMPORT_PROMPT1"), nItemCount.ToString, sTableStart(nImportType))
                    frmImport.label(2) = String.Format(gpsRM.GetString("psIMPORT_PROMPT2"), sTableStart(nImportType))
                    frmImport.label(3) = String.Format(gpsRM.GetString("psIMPORT_PROMPT3"), sTableStart(nImportType))

                    'Get the selection
                    If frmImport.ShowDialog = DialogResult.OK Then
                        Dim nEnd As Integer
                        Dim nStart As Integer
                        Dim nFrom As Integer = frmImport.FromItem
                        Dim nTo As Integer
                        If nFrom = 0 Then
                            'Copy all
                            nEnd = frmImport.MaxToItems - 1
                            nStart = 0
                        Else
                            nEnd = nFrom - 1
                            nStart = nFrom - 1
                            nTo = nFrom - 1
                        End If
                        For nIndex As Integer = nStart To nEnd
                            If nFrom = 0 Then
                                'Copy all
                                nTo = nIndex
                            End If
                            Dim oDR As DataRow = oDT.Rows.Item(nIndex)
                            Select Case nImportType
                                Case nIMPORT_SCHEDULES
                                    Try
                                        If nNameColumn >= 0 Then
                                            mDMONCfg.Schedules(nTo).COMMENT.Text = oDR.Item(nNameColumn).ToString
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nNameColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                        MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nFileDevColumn >= 0 Then
                                            Dim sTmp As String = oDR.Item(nFileDevColumn).ToString
                                            If mDMONCfg.RobotVersion >= 8.0 Then
                                                mDMONCfg.Schedules(nTo).DEVICE_NAME.Text = sTmp
                                            Else
                                                For nIdx As Integer = 0 To cboFileDevice.Items.Count - 1
                                                    If sTmp = cboFileDevice.Items(nIdx).ToString Then
                                                        mDMONCfg.Schedules(nTo).DEVICE.Value = nIdx + 1
                                                        Exit For
                                                    End If
                                                Next
                                            End If
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nFileDevColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                        MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nFileNameColumn >= 0 Then
                                            mDMONCfg.Schedules(nTo).FILE_NAME.Text = oDR.Item(nFileNameColumn).ToString
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nFileNameColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                                                                MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nFileIndexColumn >= 0 Then
                                            Dim sTmp As String = oDR.Item(nFileIndexColumn).ToString
                                            If IsNumeric(sTmp) Then
                                                mDMONCfg.Schedules(nTo).FILE_INDEX.Value = CType(sTmp, Integer)
                                            End If
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nFileIndexColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                        MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nFileSizeColumn >= 0 Then
                                            Dim sTmp As String = oDR.Item(nFileSizeColumn).ToString
                                            Dim nTmp As Integer = -1
                                            For nLen As Integer = 1 To sTmp.Length
                                                If IsNumeric(sTmp.Substring(0, nLen)) Then
                                                    nTmp = CType(sTmp.Substring(0, nLen), Integer)
                                                Else
                                                    Exit For
                                                End If
                                            Next
                                            If nTmp > -1 Then
                                                mDMONCfg.Schedules(nTo).FILE_SIZE.Value = nTmp
                                            End If
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nFileSizeColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                        MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nNumItemsColumn >= 0 Then
                                            Dim sTmp As String = oDR.Item(nNumItemsColumn).ToString
                                            If IsNumeric(sTmp) Then
                                                mDMONCfg.Schedules(nTo).NUM_ITEMS.Value = CType(sTmp, Integer)
                                            End If
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nNumItemsColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                        MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nSampleColumn >= 0 Then
                                            Dim sTmp As String = oDR.Item(nSampleColumn).ToString
                                            Dim sTmp1() As String = Split(sTmp, ":")
                                            If IsNumeric(sTmp1(0)) Then
                                                mDMONCfg.Schedules(nTo).SAMP_REQ.Value = CType(sTmp1(0), Integer)
                                            End If
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nSampleColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                        MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nMonitorColumn >= 0 Then
                                            Dim sTmp As String = oDR.Item(nMonitorColumn).ToString
                                            Dim sTmp1() As String = Split(sTmp, ":")
                                            If IsNumeric(sTmp1(0)) Then
                                                mDMONCfg.Schedules(nTo).MNTR_REQ.Value = CType(sTmp1(0), Integer)
                                            End If
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nMonitorColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                        MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nRecordColumn >= 0 Then
                                            Dim sTmp As String = oDR.Item(nRecordColumn).ToString
                                            Dim sTmp1() As String = Split(sTmp, ":")
                                            If IsNumeric(sTmp1(0)) Then
                                                mDMONCfg.Schedules(nTo).REC_REQ.Value = CType(sTmp1(0), Integer)
                                            End If
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nRecordColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                        MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nRecModeColumn >= 0 Then
                                            Dim sTmp As String = oDR.Item(nRecModeColumn).ToString
                                            For nIdx As Integer = 0 To cboRecordMode.Items.Count - 1
                                                If sTmp = cboRecordMode.Items(nIdx).ToString Then
                                                    mDMONCfg.Schedules(nTo).REC_MODE.Value = nIdx + 1
                                                    Exit For
                                                End If
                                            Next
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nRecModeColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                        MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try


                                    Try
                                        If nStartItemColumn >= 0 Then
                                            Dim sTmp As String = oDR.Item(nStartItemColumn).ToString.Trim
                                            If sTmp.ToLower = gpsRM.GetString("psDISABLED").ToLower Then
                                                mDMONCfg.Schedules(nTo).START_ENBL.Value = False
                                            Else
                                                mDMONCfg.Schedules(nTo).START_ENBL.Value = True
                                                Dim sTmp1() As String = Split(sTmp)
                                                Dim sTmp3() As String = Split(sTmp1(0), ":")
                                                If IsNumeric(sTmp3(0)) Then
                                                    mDMONCfg.Schedules(nTo).START_ITEM.Value = CType(sTmp3(0), Integer)
                                                End If
                                                If IsNumeric(sTmp1(sTmp1.Length - 1)) Then
                                                    mDMONCfg.Schedules(nTo).START_VALUE.Value = CType(sTmp1(sTmp1.Length - 1), Single)
                                                End If
                                                Dim sTmp2 As String = sTmp1(sTmp1.Length - 2)
                                                For nIdx As Integer = 0 To cboStartCond.Items.Count - 1
                                                    If sTmp2 = cboStartCond.Items(nIdx).ToString Then
                                                        mDMONCfg.Schedules(nTo).START_COND.Value = nIdx + 1
                                                        Exit For
                                                    End If
                                                Next
                                            End If
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nStartItemColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                        MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nStopItemColumn >= 0 Then
                                            Dim sTmp As String = oDR.Item(nStopItemColumn).ToString.Trim
                                            If sTmp.ToLower = gpsRM.GetString("psDISABLED").ToLower Then
                                                mDMONCfg.Schedules(nTo).STOP_ENBL.Value = False
                                            Else
                                                mDMONCfg.Schedules(nTo).STOP_ENBL.Value = True
                                                Dim sTmp1() As String = Split(sTmp)
                                                Dim sTmp3() As String = Split(sTmp1(0), ":")
                                                If IsNumeric(sTmp3(0)) Then
                                                    mDMONCfg.Schedules(nTo).STOP_ITEM.Value = CType(sTmp3(0), Integer)
                                                End If
                                                If IsNumeric(sTmp1(sTmp1.Length - 1)) Then
                                                    mDMONCfg.Schedules(nTo).STOP_VALUE.Value = CType(sTmp1(sTmp1.Length - 1), Single)
                                                End If
                                                Dim sTmp2 As String = sTmp1(sTmp1.Length - 2)
                                                For nIdx As Integer = 0 To cboStartCond.Items.Count - 1
                                                    If sTmp2 = cboStartCond.Items(nIdx).ToString Then
                                                        mDMONCfg.Schedules(nTo).STOP_COND.Value = nIdx + 1
                                                        Exit For
                                                    End If
                                                Next
                                            End If
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nStopItemColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                        MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nItemsColumn >= 0 Then
                                            Dim sTmp() As String = Split(oDR.Item(nItemsColumn).ToString, ";")
                                            For nIdx As Integer = 0 To sTmp.Length - 1
                                                If IsNumeric(sTmp(nIdx)) Then
                                                    mDMONCfg.Schedules(nTo).ITEM(nIdx).Value = CType(sTmp(nIdx), Integer)
                                                End If
                                            Next
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nItemsColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                        MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                Case nIMPORT_ITEMS

                                    Try
                                        If nNameColumn >= 0 Then
                                            mDMONCfg.Items(nTo).DESC.Text = oDR.Item(nNameColumn).ToString
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nNameColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                        MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nTypeColumn >= 0 Then
                                            Dim nTag() As Integer = CType(cboType.Tag, Integer())
                                            Dim sTmp As String = oDR.Item(nTypeColumn).ToString
                                            For nIdx As Integer = 0 To cboType.Items.Count - 1
                                                If sTmp = cboType.Items(nIdx).ToString Then
                                                    mDMONCfg.Items(nTo).TYPE.Value = nTag(nIdx)
                                                    Exit For
                                                End If
                                            Next
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nTypeColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                        MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nIOTypeColumn >= 0 Then
                                            Dim nTag() As Integer = CType(cboIoType.Tag, Integer())
                                            Dim sTmp As String = oDR.Item(nIOTypeColumn).ToString
                                            For nIdx As Integer = 0 To cboIoType.Items.Count - 1
                                                If sTmp = cboIoType.Items(nIdx).ToString Then
                                                    mDMONCfg.Items(nTo).IO_TYPE.Value = nTag(nIdx)
                                                    Exit For
                                                End If
                                            Next
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nIOTypeColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                                MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nPortColumn >= 0 Then
                                            Dim sTmp As String = oDR.Item(nPortColumn).ToString
                                            If IsNumeric(sTmp) Then
                                                mDMONCfg.Items(nTo).PORT_NUM.Value = CType(sTmp, Integer)
                                            End If
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nPortColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                        MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nAxisColumn >= 0 Then
                                            Dim sTmp As String = oDR.Item(nAxisColumn).ToString
                                            Dim sTmp1() As String = Split(sTmp, " ")
                                            Dim sTmpG As String = gpsRM.GetString("psG")
                                            Dim sTmpA As String = gpsRM.GetString("psA")
                                            If sTmp1(0).StartsWith(sTmpG) Then
                                                Dim sTmp2 As String = sTmp1(0).Substring(sTmpG.Length)
                                                If IsNumeric(sTmp2) Then
                                                    mDMONCfg.Items(nTo).GROUP_NUM.Value = CType(sTmp2, Integer)
                                                End If
                                            End If
                                            If sTmp1(1).StartsWith(sTmpA) Then
                                                Dim sTmp3 As String = sTmp1(0).Substring(sTmpA.Length)
                                                If IsNumeric(sTmp3) Then
                                                    mDMONCfg.Items(nTo).AXIS_NUM.Value = CType(sTmp3, Integer)
                                                End If
                                            End If
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nAxisColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                        MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nProgColumn >= 0 Then
                                            mDMONCfg.Items(nTo).PRG_NAME.Text = oDR.Item(nProgColumn).ToString
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nProgColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                           MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nVarColumn >= 0 Then
                                            mDMONCfg.Items(nTo).VAR_NAME.Text = oDR.Item(nVarColumn).ToString
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nVarColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                        MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nUnitsColumn >= 0 Then
                                            mDMONCfg.Items(nTo).UNITS.Text = oDR.Item(nUnitsColumn).ToString
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nUnitsColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                        MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nSlopeColumn >= 0 Then
                                            Dim sTmp As String = oDR.Item(nSlopeColumn).ToString
                                            If IsNumeric(sTmp) Then
                                                mDMONCfg.Items(nTo).SLOPE.Value = CType(sTmp, Single)
                                            End If
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nSlopeColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                                MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                    Try
                                        If nIntColumn >= 0 Then
                                            Dim sTmp As String = oDR.Item(nIntColumn).ToString
                                            If IsNumeric(sTmp) Then
                                                mDMONCfg.Items(nTo).INTERCEPT.Value = CType(sTmp, Single)
                                            End If
                                        End If
                                    Catch ex As Exception
                                        Dim nCol As Integer = nIntColumn
                                        Dim sText As String = String.Format(gpsRM.GetString("psIMPORT_FAIL_TEXT"), oDR.Item(nNameColumn).ToString, oDT.Columns.Item(nCol).ColumnName, oDR.Item(nCol).ToString)
                                        MessageBox.Show(sText, gpsRM.GetString("psIMPORT_FAIL_CAP"), MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try
                                Case Else
                                    '?
                            End Select
                        Next
                        EditsMade = True
                    End If
                End If
            Else
                Dim lRet As Response = MessageBox.Show(gpsRM.GetString("psVALID_IMPORT_DATA_NOT_FOUND_TEXT"), gpsRM.GetString("psVALID_IMPORT_DATA_NOT_FOUND_CAP"), _
                                                       MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
            subShowNewPage(True)
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuImport_Click", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub chkStartStopItem_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkStartItem.CheckedChanged, chkStopItem.CheckedChanged
        '********************************************************************************************
        'Description:  schedule and item tab cbo validation routine
        '
        'Parameters: event parameters
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If mbEventBlocker Then Exit Sub
            Dim oChk As CheckBox = DirectCast(sender, CheckBox)
            Dim sName As String = oChk.Name
            'every thing else uses the case statement
            Select Case sName
                Case chkStartItem.Name
                    mDMONCfg.Schedules(cboParm.SelectedIndex).START_ENBL.Value = oChk.Checked
                Case chkStopItem.Name
                    mDMONCfg.Schedules(cboParm.SelectedIndex).STOP_ENBL.Value = oChk.Checked
                Case Else
            End Select
            EditsMade = True
            subEnableControls(True)
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module:frmMain Routine: cboTabItemTabSchedule_SelectionChangeCommitted", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Public Function oGetIOSignals(ByRef oRobot As FRRobot.FRCRobot, ByVal oIO_Type As clsDMONCfg.eIO_TYPE) As FRRobot.FRCIOSignals
        '********************************************************************************************
        'Robot Register lookup
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/16/11  MSW     Get IO collection
        '********************************************************************************************
        Try
            Dim oFRCIOTypes As FRRobot.FRCIOTypes = DirectCast(oRobot.IOTypes, FRRobot.FRCIOTypes)
            Dim oFRCIOSignals As FRRobot.FRCIOSignals = Nothing
            Select Case oIO_Type
                Case clsDMONCfg.eIO_TYPE.IO_AI
                    Dim oFRCIOType As FRRobot.FRCAnalogIOType = DirectCast(oFRCIOTypes(FRRobot.FREIOTypeConstants.frAInType), FRRobot.FRCAnalogIOType)
                    oFRCIOSignals = DirectCast(oFRCIOType.Signals, FRRobot.FRCIOSignals)
                Case clsDMONCfg.eIO_TYPE.IO_AO
                    Dim oFRCIOType As FRRobot.FRCAnalogIOType = DirectCast(oFRCIOTypes(FRRobot.FREIOTypeConstants.frAOutType), FRRobot.FRCAnalogIOType)
                    oFRCIOSignals = DirectCast(oFRCIOType.Signals, FRRobot.FRCIOSignals)
                Case clsDMONCfg.eIO_TYPE.IO_DI
                    Dim oFRCIOType As FRRobot.FRCDigitalIOType = DirectCast(oFRCIOTypes(FRRobot.FREIOTypeConstants.frDInType), FRRobot.FRCDigitalIOType)
                    oFRCIOSignals = DirectCast(oFRCIOType.Signals, FRRobot.FRCIOSignals)
                Case clsDMONCfg.eIO_TYPE.IO_DO
                    Dim oFRCIOType As FRRobot.FRCDigitalIOType = DirectCast(oFRCIOTypes(FRRobot.FREIOTypeConstants.frDOutType), FRRobot.FRCDigitalIOType)
                    oFRCIOSignals = DirectCast(oFRCIOType.Signals, FRRobot.FRCIOSignals)
                Case clsDMONCfg.eIO_TYPE.IO_GI
                    Dim oFRCIOType As FRRobot.FRCGroupIOType = DirectCast(oFRCIOTypes(FRRobot.FREIOTypeConstants.frGPInType), FRRobot.FRCGroupIOType)
                    oFRCIOSignals = DirectCast(oFRCIOType.Signals, FRRobot.FRCIOSignals)
                Case clsDMONCfg.eIO_TYPE.IO_GO
                    Dim oFRCIOType As FRRobot.FRCGroupIOType = DirectCast(oFRCIOTypes(FRRobot.FREIOTypeConstants.frGPOutType), FRRobot.FRCGroupIOType)
                    oFRCIOSignals = DirectCast(oFRCIOType.Signals, FRRobot.FRCIOSignals)
                Case clsDMONCfg.eIO_TYPE.IO_RI
                    Dim oFRCIOType As FRRobot.FRCRobotIOType = DirectCast(oFRCIOTypes(FRRobot.FREIOTypeConstants.frRDInType), FRRobot.FRCRobotIOType)
                    oFRCIOSignals = DirectCast(oFRCIOType.Signals, FRRobot.FRCIOSignals)
                Case clsDMONCfg.eIO_TYPE.IO_RO
                    Dim oFRCIOType As FRRobot.FRCRobotIOType = DirectCast(oFRCIOTypes(FRRobot.FREIOTypeConstants.frRDOutType), FRRobot.FRCRobotIOType)
                    oFRCIOSignals = DirectCast(oFRCIOType.Signals, FRRobot.FRCIOSignals)
                Case clsDMONCfg.eIO_TYPE.IO_WI
                    Dim oFRCIOType As FRRobot.FRCWeldDigitalIOType = DirectCast(oFRCIOTypes(FRRobot.FREIOTypeConstants.frWDInType), FRRobot.FRCWeldDigitalIOType)
                    oFRCIOSignals = DirectCast(oFRCIOType.Signals, FRRobot.FRCIOSignals)
                Case clsDMONCfg.eIO_TYPE.IO_WO
                    Dim oFRCIOType As FRRobot.FRCWeldDigitalIOType = DirectCast(oFRCIOTypes(FRRobot.FREIOTypeConstants.frWDOutType), FRRobot.FRCWeldDigitalIOType)
                    oFRCIOSignals = DirectCast(oFRCIOType.Signals, FRRobot.FRCIOSignals)
                Case Else
            End Select

            Return oFRCIOSignals
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Private Function oGetRegister(ByRef oRobot As FRRobot.FRCRobot, ByVal nReg As Integer) As FRRobot.FRCRegNumeric
        '********************************************************************************************
        'Robot Register lookup
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/16/11  MSW     Get Register
        '********************************************************************************************
        If nReg <= 0 Then
            Return Nothing
        End If
        Dim oRegs As FRRobot.FRCVars = oRobot.RegNumerics
        Try
            Dim oVar As FRRobot.FRCVar = DirectCast(oRegs(nReg), FRRobot.FRCVar)
            Dim oReg As FRRobot.FRCRegNumeric = DirectCast(oVar.Value, FRRobot.FRCRegNumeric)
            Return oReg
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Private Sub btnValidate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnValidate.Click
        '********************************************************************************************
        'Validate item configuration 
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/16/11  MSW     Check item configuration for valid variable, IO, or register.
        '********************************************************************************************
        Dim bMessagePosted As Boolean = False
        Try
            If cboRobot.SelectedIndex > -1 AndAlso cboParm.SelectedIndex > -1 Then
                For Each oController As clsController In colControllers
                    If oController.Name = msControllerName Then
                        If oController.DMONCfg IsNot Nothing Then
                            Dim oRobot As FRRobot.FRCRobot = oController.Robot
                            With oController.DMONCfg.Items(cboParm.SelectedIndex)
                                Select Case .TYPE.Value
                                    Case clsDMONCfg.eTYPE.Int, clsDMONCfg.eTYPE.Real
                                        'Robot Variable lookup
                                        Try
                                            Dim sProg As String = .PRG_NAME.Text.ToUpper
                                            Dim sVar As String = .VAR_NAME.Text.ToUpper
                                            Dim oFRCVar As FRRobot.FRCVar = Nothing
                                            Dim oFRCVars As FRRobot.FRCVars = Nothing
                                            Dim bVarFound As Boolean = False
                                            If sProg = "*SYSTEM*" Then
                                                Try
                                                    oFRCVar = DirectCast(oRobot.SysVariables.Item(sVar), FRRobot.FRCVar)
                                                    If oFRCVar IsNot Nothing Then
                                                        bVarFound = True
                                                        'Found
                                                        Dim bTypeOK As Boolean = False
                                                        Select Case oFRCVar.TypeCode
                                                            Case FRRobot.FRETypeCodeConstants.frBooleanType, FRRobot.FRETypeCodeConstants.frByteType, _
                                                                  FRRobot.FRETypeCodeConstants.frIntegerType, FRRobot.FRETypeCodeConstants.frShortType
                                                                bTypeOK = (CType(.TYPE.Value, clsDMONCfg.eTYPE) = clsDMONCfg.eTYPE.Int)
                                                            Case FRRobot.FRETypeCodeConstants.frRealType
                                                                bTypeOK = (CType(.TYPE.Value, clsDMONCfg.eTYPE) = clsDMONCfg.eTYPE.Real)
                                                            Case Else
                                                                bTypeOK = False
                                                        End Select
                                                        If bTypeOK Then
                                                            MessageBox.Show(gpsRM.GetString("psVAL_OK"), gpsRM.GetString("psVAL_CAP"), _
                                                                            MessageBoxButtons.OK, MessageBoxIcon.Information)
                                                        Else
                                                            Dim sTmp As String = String.Format(gpsRM.GetString("psVAL_VAR_TYPE_MISMATCH"), oFRCVar.TypeName)
                                                            MessageBox.Show(sTmp, gpsRM.GetString("psVAL_CAP"), _
                                                                            MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                        End If
                                                        bMessagePosted = True
                                                    End If
                                                Catch ex As Exception
                                                    'Nothing Here
                                                End Try
                                                If bVarFound = False Then
                                                    Try
                                                        oFRCVars = DirectCast(oRobot.SysVariables.Item(sVar), FRRobot.FRCVars)
                                                        If oFRCVars IsNot Nothing Then
                                                            Dim sTmp As String = String.Format(gpsRM.GetString("psVAL_PRG_VAR_STRUCTURE"), sProg, sVar)
                                                            MessageBox.Show(sTmp, gpsRM.GetString("psVAL_CAP"), _
                                                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                            bMessagePosted = True
                                                        End If
                                                    Catch ex As Exception
                                                        'NotFound
                                                    End Try
                                                End If
                                                If bMessagePosted = False Then
                                                    'Not found
                                                    Dim sTmp As String = String.Format(gpsRM.GetString("psVAL_SYS_VAR_NOT_FOUND"), sVar)
                                                    MessageBox.Show(sTmp, gpsRM.GetString("psVAL_CAP"), _
                                                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                End If
                                            Else
                                                'Program Variable
                                                Try
                                                    Dim oFRCPrograms As FRRobot.FRCPrograms = oRobot.Programs
                                                    Dim oFRCProg As FRRobot.FRCProgram = DirectCast(oFRCPrograms(sProg), FRRobot.FRCProgram)
                                                    If oFRCProg IsNot Nothing Then
                                                        Try
                                                            oFRCVar = DirectCast(oFRCProg.Variables(sVar), FRRobot.FRCVar)
                                                            If oFRCVar IsNot Nothing Then
                                                                bVarFound = True
                                                                'Found
                                                                Dim bTypeOK As Boolean = False
                                                                Select Case oFRCVar.TypeCode
                                                                    Case FRRobot.FRETypeCodeConstants.frBooleanType, FRRobot.FRETypeCodeConstants.frByteType, _
                                                                          FRRobot.FRETypeCodeConstants.frIntegerType, FRRobot.FRETypeCodeConstants.frShortType
                                                                        bTypeOK = (CType(.TYPE.Value, clsDMONCfg.eTYPE) = clsDMONCfg.eTYPE.Int)
                                                                    Case FRRobot.FRETypeCodeConstants.frRealType
                                                                        bTypeOK = (CType(.TYPE.Value, clsDMONCfg.eTYPE) = clsDMONCfg.eTYPE.Real)
                                                                    Case Else
                                                                        bTypeOK = False
                                                                End Select
                                                                If bTypeOK Then

                                                                    MessageBox.Show(gpsRM.GetString("psVAL_OK"), gpsRM.GetString("psVAL_CAP"), _
                                                                                    MessageBoxButtons.OK, MessageBoxIcon.Information)
                                                                Else
                                                                    Dim sTmp As String = String.Format(gpsRM.GetString("psVAL_VAR_TYPE_MISMATCH"), oFRCVar.TypeName)
                                                                    MessageBox.Show(sTmp, gpsRM.GetString("psVAL_CAP"), _
                                                                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                                End If
                                                                bMessagePosted = True
                                                            End If
                                                        Catch ex As Exception
                                                            'Nothing Here
                                                        End Try
                                                        If bVarFound = False Then
                                                            oFRCVars = DirectCast(oFRCProg.Variables(sVar), FRRobot.FRCVars)
                                                            If oFRCVars IsNot Nothing Then
                                                                Dim sTmp As String = String.Format(gpsRM.GetString("psVAL_SYS_VAR_STRUCTURE"), sVar)
                                                                MessageBox.Show(sTmp, gpsRM.GetString("psVAL_CAP"), _
                                                                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                                bMessagePosted = True
                                                            End If
                                                        End If
                                                    Else
                                                        'Prog Not found
                                                        Dim sTmp As String = String.Format(gpsRM.GetString("psVAL_PRG_NOT_FOUND"), sProg)
                                                        MessageBox.Show(sTmp, gpsRM.GetString("psVAL_CAP"), _
                                                                        MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                    End If

                                                Catch ex As Exception
                                                    'Prog Not found
                                                    Dim sTmp As String = String.Format(gpsRM.GetString("psVAL_PRG_NOT_FOUND"), sProg)
                                                    MessageBox.Show(sTmp, gpsRM.GetString("psVAL_CAP"), _
                                                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                    bMessagePosted = True
                                                End Try
                                                If bMessagePosted = False Then
                                                    'Not found
                                                    Dim sTmp As String = String.Format(gpsRM.GetString("psVAL_PRG_VAR_NOT_FOUND"), sProg)
                                                    MessageBox.Show(sTmp, gpsRM.GetString("psVAL_CAP"), _
                                                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                End If
                                            End If
                                        Catch ex As Exception
                                            MessageBox.Show(gpsRM.GetString("psVAL_ERR2") & ex.Message, _
                                                    gpsRM.GetString("psVAL_CAP"), _
                                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                                            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: btnValidate_Click - Var", _
                                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                                        End Try
                                    Case clsDMONCfg.eTYPE.IO
                                        'Robot IO lookup
                                        Try
                                            Dim nPort As Integer = .PORT_NUM.Value
                                            Dim oFRCIOSignals As FRRobot.FRCIOSignals = oGetIOSignals(oRobot, CType(.IO_TYPE.Value, clsDMONCfg.eIO_TYPE))
                                            If oFRCIOSignals IsNot Nothing Then
                                                Try
                                                    Dim oItem As Object = oFRCIOSignals(nPort)
                                                    If oItem IsNot Nothing Then
                                                        MessageBox.Show(gpsRM.GetString("psVAL_OK"), gpsRM.GetString("psVAL_CAP"), _
                                                                        MessageBoxButtons.OK, MessageBoxIcon.Information)
                                                    Else
                                                        Dim sTmp As String = String.Format(gpsRM.GetString("psVAL_IO_POINT_NOT_FOUND"), cboIoType.SelectedText, nPort.ToString)
                                                        MessageBox.Show(sTmp, gpsRM.GetString("psVAL_CAP"), _
                                                                        MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                    End If
                                                Catch ex As Exception
                                                    Dim sTmp As String = String.Format(gpsRM.GetString("psVAL_IO_POINT_NOT_FOUND"), _
                                                                                       mDMONCfg.sGetIOTYPEString(CType(.IO_TYPE.Value, clsDMONCfg.eIO_TYPE)), nPort.ToString)
                                                    MessageBox.Show(sTmp, gpsRM.GetString("psVAL_CAP"), _
                                                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                End Try
                                            Else
                                                MessageBox.Show(gpsRM.GetString("psVAL_IO_TYPE_NOT_FOUND"), gpsRM.GetString("psVAL_CAP"), _
                                                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                                            End If
                                        Catch ex As Exception
                                            MessageBox.Show(gpsRM.GetString("psVAL_IO_TYPE_VAL_ERR") & ex.Message, gpsRM.GetString("psVAL_CAP"), _
                                                            MessageBoxButtons.OK, MessageBoxIcon.Error)
                                        End Try
                                    Case clsDMONCfg.eTYPE.Register
                                        'Robot Register lookup
                                        Try
                                            Dim nReg As Integer = .PORT_NUM.Value
                                            Dim oReg As FRRobot.FRCRegNumeric = oGetRegister(oRobot, nReg)
                                            If oReg IsNot Nothing Then
                                                lblPortNumComment.Text = oReg.Comment
                                                MessageBox.Show(gpsRM.GetString("psVAL_OK"), gpsRM.GetString("psVAL_CAP"), _
                                                                MessageBoxButtons.OK, MessageBoxIcon.Information)
                                            Else
                                                Dim sTmp As String = String.Format(gpsRM.GetString("psVAL_REG_NOT_FOUND"), nReg.ToString)
                                                MessageBox.Show(sTmp, gpsRM.GetString("psVAL_CAP"), _
                                                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                                            End If
                                        Catch ex As Exception
                                            MessageBox.Show(gpsRM.GetString("psVAL_REG_ERR") & ex.Message, gpsRM.GetString("psVAL_CAP"), _
                                                            MessageBoxButtons.OK, MessageBoxIcon.Error)
                                        End Try


                                    Case clsDMONCfg.eTYPE.Axis
                                        'Robot Axis Range Check
                                        Try
                                            Dim nGroup As Integer = .GROUP_NUM.Value
                                            Dim nAxis As Integer = .AXIS_NUM.Value
                                            Dim oStruct As FRRobot.FRCVars = DirectCast(oRobot.SysVariables.Item("$SCR"), FRRobot.FRCVars)
                                            Dim oFRCVar As FRRobot.FRCVar = DirectCast(oStruct.Item("$NUM_GROUP"), FRRobot.FRCVar)
                                            Dim nMaxGroup As Integer = CType(oFRCVar.Value, Integer)
                                            If nGroup <= nMaxGroup Then
                                                oStruct = DirectCast(oRobot.SysVariables.Item("$SCR_GRP"), FRRobot.FRCVars)
                                                Dim oStruct2 As FRRobot.FRCVars = DirectCast(oStruct.Item(nGroup.ToString), FRRobot.FRCVars)
                                                oFRCVar = DirectCast(oStruct2.Item("$NUM_AXES"), FRRobot.FRCVar)
                                                Dim nMaxAxes As Integer = CType(oFRCVar.Value, Integer)
                                                If nAxis <= nMaxAxes Then
                                                    MessageBox.Show(gpsRM.GetString("psVAL_OK"), gpsRM.GetString("psVAL_CAP"), _
                                                            MessageBoxButtons.OK, MessageBoxIcon.Information)
                                                Else
                                                    Dim sTmp As String = String.Empty
                                                    If nMaxAxes = 1 Then
                                                        sTmp = String.Format(gpsRM.GetString("psVAL_AXIS_AXIS_ERR1"), nAxis.ToString, nGroup.ToString)
                                                    Else
                                                        sTmp = String.Format(gpsRM.GetString("psVAL_AXIS_AXIS_ERR"), nAxis.ToString, nGroup.ToString, nMaxAxes.ToString)
                                                    End If
                                                    MessageBox.Show(sTmp, gpsRM.GetString("psVAL_CAP"), _
                                                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                End If
                                            Else
                                                Dim sTmp As String = String.Empty
                                                If nMaxGroup = 1 Then
                                                    sTmp = String.Format(gpsRM.GetString("psVAL_AXIS_GROUP_ERR1"), nGroup.ToString)
                                                Else
                                                    sTmp = String.Format(gpsRM.GetString("psVAL_AXIS_GROUP_ERR"), nGroup.ToString, nMaxGroup.ToString)
                                                End If
                                                MessageBox.Show(sTmp, gpsRM.GetString("psVAL_CAP"), _
                                                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                                            End If
                                        Catch ex As Exception
                                            MessageBox.Show(gpsRM.GetString("psVAL_AXIS_VAL_ERR") & ex.Message, gpsRM.GetString("psVAL_CAP"), _
                                                            MessageBoxButtons.OK, MessageBoxIcon.Error)
                                        End Try
                                    Case Else
                                        MessageBox.Show(gpsRM.GetString("psVAL_IMCOMP"), gpsRM.GetString("psVAL_CAP"), _
                                                        MessageBoxButtons.OK, MessageBoxIcon.Error)
                                End Select

                            End With
                        End If
                    End If
                Next
            End If

        Catch ex As Exception
            MessageBox.Show(gpsRM.GetString("psVAL_ERR1") & ex.Message, _
                            gpsRM.GetString("psVAL_CAP"), _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: btnValidate_Click", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

        End Try
    End Sub
    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
        '********************************************************************************************
        'Browse for item configuration details
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/16/11  MSW     Check item configuration for valid variable, IO, or register.
        '********************************************************************************************
        Dim bMessagePosted As Boolean = False
        Try
            If cboRobot.SelectedIndex > -1 AndAlso cboParm.SelectedIndex > -1 Then
                For Each oController As clsController In colControllers
                    If oController.Name = msControllerName Then
                        If oController.DMONCfg IsNot Nothing Then
                            frmBrowse.InitBrowseForm(oController.DMONCfg, oController.DMONCfg.Items(cboParm.SelectedIndex), oController)
                            If frmBrowse.ShowDialog() = Windows.Forms.DialogResult.OK Then
                                subShowDetailPage()
                            End If
                        End If
                    End If
                Next
            End If

        Catch ex As Exception
            MessageBox.Show(gpsRM.GetString("psVAL_ERR1") & ex.Message, _
                            gpsRM.GetString("psVAL_CAP"), _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: btnBrowse_Click", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

        End Try
    End Sub
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
        '********************************************************************************************
        bPrintdoc(True, True)

    End Sub

    Private Sub mnuSaveDefault_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSaveDefault.Click
        '********************************************************************************************
        'Description:  save current settings as the default in XML folder
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sFile As String = String.Format(gpsRM.GetString("psDMON_DEFAULTS_FILE"), tabMain.SelectedTab.Text, cboRobot.Text)
        Dim sFilePath As String = String.Empty
        Dim bFileExists As Boolean = GetDefaultFilePath(sFilePath, eDir.XML, String.Empty, sFile)
        Dim lRet As Response = Windows.Forms.DialogResult.No
        Dim sCap As String = String.Format(gpsRM.GetString("psSAVE_DEFAULT_CAP"), tabMain.SelectedTab.Text, cboRobot.Text)
        If bFileExists Then
            Dim sMsg As String = String.Format(gpsRM.GetString("psSAVE_DEFAULT_OW"), sFilePath, tabMain.SelectedTab.Text, cboRobot.Text)
            lRet = MessageBox.Show(sMsg, sCap, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If lRet = Windows.Forms.DialogResult.Yes Then
                IO.File.Delete(sFilePath)
                bPrintdoc(True, True, sFilePath)
            End If

        Else
            Dim sMsg As String = String.Format(gpsRM.GetString("psSAVE_DEFAULT"), tabMain.SelectedTab.Text, cboRobot.Text, sFilePath)
            lRet = MessageBox.Show(sMsg, sCap, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If lRet = Windows.Forms.DialogResult.Yes Then
                bPrintdoc(True, True, sFilePath)
            End If
        End If
    End Sub

    Private Sub mnuPageSetup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPageSetup.Click
        '********************************************************************************************
        'Description:  Web browser's page setup form
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
                mPrintHtml.subShowPageSetup()
            End If
        End If
    End Sub

    Private Sub mnuPrintOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintOptions.Click
        '********************************************************************************************
        'Description:  Print option form
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

End Class
