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
' FANUC LTD JapanflblPLCDate
'
' Form/Module: frmMain
'
' Description: Clock
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
'    11/12/09   MSW     add delegate declarations tohandle cross-thread calls
'    10/11/10   MSW     add missing psSCREENCAPTION	to ProjectStrings.resx
'    09/14/11   MSW     Assemble a standard version of everything                       4.1.0.0
'    09/27/11   MSW     Standalone changes round 1 - HGB SA paintshop computer changes	4.1.0.1
'    12/02/11   MSW     Add DMONCfg reference                                           4.1.1.0
 
'    01/24/12   MSW     Placeholder include updates                                   4.01.01.02
'    02/15/12   MSW     Force 32 bit build for PCDK compatability                     4.01.01.03
'    03/23/12   RJO     modified for .NET Password and IPC                            4.01.02.00
'    04/11/12   MSW     Change CommonStrings setup so it builds correctly             4.01.03.00
'    04/20/12   MSW     frmMain_KeyDown-Add excape key handler                        4.01.03.01
'    09/24/12   AM      Added MIS Computer support NO Robot Controllers on XML        4.01.03.02
'                       Only reading GUI and PLC.
'    10/24/12   RJO     Added StartupModule to project to prevent multiple instances  4.01.03.03
'    04/16/13   MSW     Add Canadian language files                                   4.01.05.00
'                       Standalone Changelog
'    06/11/13   MSW     Fix from Dave for single robot configs                        4.01.05.01
'    07/09/13   MSW     Update and standardize logos                                  4.01.05.02
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up  4.01.05.03
'    09/30/13   MSW     Save screenshots as jpegs, PLC DLL                            4.01.06.00
'    01/07/14   MSW     Remove ControlBox from main form                              4.01.06.01
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call              4.01.07.00
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
    Public Const msSCREEN_NAME As String = "Clock"   ' <-- For password area change log etc.
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

    Private mbLocal As Boolean
    Private mbScreenLoaded As Boolean
    Private mbEventBlocker As Boolean
    Private mbRemoteZone As Boolean

    Private msSCREEN_DUMP_NAME As String = "Utility_Clock.jpg" 'changes with selected tab
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

    Private mGUIDate As Date
    Private mPLCDate As Date
    Private mRobotDate() As Date = Nothing
    Dim msOldGUITime As String = String.Empty
    Dim msOldPLCTime As String = String.Empty
    Dim msOldRobotTime() As String = Nothing
    Private WithEvents colControllers As clsControllers = Nothing
    Private WithEvents mPLC As clsPLCComm
    Private msPLCData() As String
    '******** This is the old pw3 password object interop  ******************************************
    'Friend WithEvents moPassword As PWPassword.PasswordObject 'RJO 03/23/12
    'Friend moPrivilege As PWPassword.cPrivilegeObject 'RJO 03/23/12

    Friend WithEvents moPassword As clsPWUser 'RJO 03/23/12
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm  'RJO 03/23/12

    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    Delegate Sub ControllerStatusChange(ByVal Controller As clsController)
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet)  'RJO 03/23/12
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
            subEnableControls(Value)
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
                        Call subSaveData()
                        Return True
                End Select
            Case Response.Cancel
                'false aborts closing form , changing zone etc
                Status = gcsRM.GetString("csSAVE_CANCEL", oCulture)
                Return False
            Case Else
                Call subLoadData()
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
            Today = mGUIDate
            TimeOfDay = mGUIDate
            subUpdateChangeLog(mGUIDate.ToString(), msOldGUITime, mcolZones.ActiveZone, mcolZones.CurrentZone, _
                               (gpsRM.GetString("psGUI_DEVICE_NAME") & " " & gpsRM.GetString("psDATETIME")))
            Return (True)
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bSaveToGUI", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Function
    Private Function bSaveToPLC() As Boolean
        '********************************************************************************************
        'Description:  Save data to PLC
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '04/25/11   HGB     Just return TRUE if SA
        '********************************************************************************************
        Try
            If mcolZones.StandAlone Then Return (True)
            Dim sData(5) As String
            Dim sTag As String = "PLCSetDateTime"

            sData(0) = mPLCDate.Year.ToString
            sData(1) = mPLCDate.Month.ToString
            sData(2) = mPLCDate.Day.ToString
            sData(3) = mPLCDate.Hour.ToString
            sData(4) = mPLCDate.Minute.ToString
            sData(5) = mPLCDate.Second.ToString

            With mPLC
                '.TagName = "Z" & mcolZones.CurrentZoneNumber.ToString & sTag 'NVSP 12/08/2016 Only one PLC no need to be zone specific
                .TagName = sTag
                .PLCData = sData
            End With
            subUpdateChangeLog(mPLCDate.ToString(), msOldPLCTime, mcolZones.ActiveZone, mcolZones.CurrentZone, _
                   gpsRM.GetString("psPLC_DEVICE_NAME") & " " & gpsRM.GetString("psDATETIME"))

            Return True
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: bSaveToPLC", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

        End Try
    End Function
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
        ' 06/11/13  MSW     Fix from Dave
        '********************************************************************************************
        Dim nRobot As Integer = 0
        '09/24/2012 AM Added MIS Computer Support
        Dim bNotMISComputer As Boolean = Not (mcolZones.MISComputer)
        If bNotMISComputer Then
            ReDim mRobotDate(colControllers.Count - 1)
            For nRobot = 1 To colControllers.Count
                Dim lblRCName As Label = DirectCast(pnlMain.Controls("lblRC_" & nRobot.ToString), Label)
                Dim lblRCDate As Label = DirectCast(pnlMain.Controls("lblRCDate" & nRobot.ToString), Label)
                Dim lblRCTime As Label = DirectCast(pnlMain.Controls("lblRCTime" & nRobot.ToString), Label)
                If lblRCName Is Nothing Then
                    'Make the labels
                    Dim pt As Point = lblRC_1.Location
                    Dim nDiff As Integer = (lblRC_2.Location.Y - lblRC_1.Location.Y)
                    nDiff = nDiff * (nRobot - 1)
                    pt.Y = pt.Y + nDiff
                    MakeLabel(lblRCName, ("lblRC_" & nRobot.ToString), pt, lblRC_1)
                    pt.X = lblRCDate1.Location.X
                    MakeLabel(lblRCDate, ("lblRCDate" & nRobot.ToString), pt, lblRCDate1)
                    pt.X = lblRCTime1.Location.X
                    MakeLabel(lblRCTime, ("lblRCTime" & nRobot.ToString), pt, lblRCTime1)
                End If
                lblRCName.Text = colControllers(nRobot - 1).Name
                lblRCDate.Text = String.Empty
                lblRCTime.Text = String.Empty
                Dim mnuRCName As ToolStripMenuItem = DirectCast(mnuSaveRobotToAll.DropDownItems("mnuSaveTimeR" & nRobot.ToString), ToolStripMenuItem)
                If mnuRCName Is Nothing Then
                    mnuRCName = New ToolStripMenuItem
                    mnuRCName.Name = "mnuSaveTimeR" & nRobot.ToString
                    AddHandler mnuRCName.Click, AddressOf mnuSaveToAll_Click
                    mnuSaveRobotToAll.DropDownItems.Add(mnuRCName)
                End If
                mnuRCName.Text = colControllers(nRobot - 1).Name
            Next
            nRobot = colControllers.Count + 1
            Dim bDone As Boolean = False
            Do Until bDone
                Dim lblRCName As Label = DirectCast(pnlMain.Controls("lblRC_" & nRobot.ToString), Label)
                If lblRCName IsNot Nothing Then
                    lblRCName.Visible = False
                    Dim lblRCDate As Label = DirectCast(pnlMain.Controls("lblRCDate" & nRobot.ToString), Label)
                    lblRCDate.Visible = False
                    Dim lblRCTime As Label = DirectCast(pnlMain.Controls("lblRCTime" & nRobot.ToString), Label)
                    lblRCTime.Visible = False
                Else
                    bDone = True
                End If
                Dim mnuRCName As ToolStripMenuItem = DirectCast(mnuSaveRobotToAll.DropDownItems("mnuSaveTimeR" & nRobot.ToString), ToolStripMenuItem)
                If mnuRCName IsNot Nothing Then
                    mnuRCName.Visible = False
                End If
                nRobot += 1
            Loop
        Else
            lblRobotControllers.Visible = bNotMISComputer
            lblRobotNameCap.Visible = bNotMISComputer
            lblRobotDateCap.Visible = bNotMISComputer
            lblRobotTimeCap.Visible = bNotMISComputer
            lblRC_1.Visible = bNotMISComputer
            lblRCDate1.Visible = bNotMISComputer
            lblRCTime1.Visible = bNotMISComputer
            lblRC_2.Visible = bNotMISComputer
            lblRCDate2.Visible = bNotMISComputer
            lblRCTime2.Visible = bNotMISComputer
            mnuSaveRobotToAll.Visible = bNotMISComputer
        End If
        Dim bNotStandalone As Boolean = Not (mcolZones.StandAlone)
        lblPLCDate.Visible = bNotStandalone
        lblPLCDateCap.Visible = bNotStandalone
        lblPLCTime.Visible = bNotStandalone
        lblPLCTimeCap.Visible = bNotStandalone
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

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subEnableControls(False)
            EditsMade = False
            DataLoaded = False

            colControllers = New clsControllers(mcolZones, False)
            SetUpStatusStrip(Me, colControllers)
            colControllers.RefreshConnectionStatus()

            subFormatScreenLayout()
            If mbScreenLoaded = True Then
                Call subLoadData()
            End If

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

        Else
            Select Case Privilege
                Case ePrivilege.None
                    btnSave.Enabled = False
                    btnUndo.Enabled = False
                    btnChangeLog.Enabled = (True And DataLoaded)
                    bRestOfControls = False
                    pnlMain.Enabled = True

                Case ePrivilege.Edit, ePrivilege.Copy '02/02/10 RJO
                    'Let them save if the times are different, even if they didn't edit
                    Dim bSaveEnable As Boolean = EditsMade
                    If Not (bSaveEnable) And DataLoaded Then
                        If Math.Abs(DateDiff(DateInterval.Second, mGUIDate, mPLCDate)) > 5 Then
                            bSaveEnable = True
                        End If
                        If Not (bSaveEnable) And (mRobotDate IsNot Nothing) Then
                            For Each oDate As Date In mRobotDate
                                If Math.Abs(DateDiff(DateInterval.Second, mGUIDate, oDate)) > 5 Then
                                    bSaveEnable = True
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                    btnSave.Enabled = bSaveEnable
                    btnUndo.Enabled = EditsMade
                    btnChangeLog.Enabled = DataLoaded
                    bRestOfControls = DataLoaded
                    pnlMain.Enabled = DataLoaded

            End Select
        End If
        dtpDate.Enabled = bRestOfControls
        dtpTime.Enabled = bRestOfControls
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
                ' set labels 
                lblEditDateCap.Text = .GetString("psEDIT_DATE", oCulture)
                lblEditTimeCap.Text = .GetString("psEDIT_TIME", oCulture)
                lblGUIDate.Text = String.Empty
                lblGUITime.Text = String.Empty
                lblGUIDateCap.Text = .GetString("psGUI_DATE", oCulture)
                lblGUITimeCap.Text = .GetString("psGUI_TIME", oCulture)
                lblPLCDate.Text = String.Empty
                lblPLCTime.Text = String.Empty
                lblPLCDateCap.Text = .GetString("psPLC_DATE", oCulture)
                lblPLCTimeCap.Text = .GetString("psPLC_TIME", oCulture)
                lblRobotControllers.Text = .GetString("psROBOT_CONTROLLERS", oCulture)
                lblRobotNameCap.Text = .GetString("psROBOT_NAME", oCulture)
                lblRobotDateCap.Text = .GetString("psROBOT_DATE", oCulture)
                lblRobotTimeCap.Text = .GetString("psROBOT_TIME", oCulture)
                mnuSaveGUIToAll.Text = .GetString("psCOPY_GUI_TIME_TO_ALL", oCulture)
                mnuSavePLCToAll.Text = .GetString("psCOPY_PLC_TIME_TO_ALL", oCulture)
                mnuSaveRobotToAll.Text = .GetString("psCOPY_ROBOT_TIME_TO_ALL", oCulture)
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
        'Dim cachePrivilege As mPassword.ePrivilege 'RJO 03/23/12

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

        Status = gcsRM.GetString("csINITALIZING", DisplayCulture)
        Progress = 1

        Try
            DataLoaded = False
            EditsMade = False
            mbScreenLoaded = False

            subProcessCommandLine()

            mcolZones = New clsZones(String.Empty)
            'NVSP 03182017 doesn't make any sense to display combo box only for remote zones in a multi zone environment hence commented out
            'Show the Zone combobox only if this computer connects to remote zones
            ''''''mbLocal = True
            ''''''For Each oZone As clsZone In mcolZones
            ''''''    If oZone.IsRemoteZone Then
            ''''''        mbLocal = False
            ''''''        Exit For
            ''''''    End If
            ''''''Next
            mbLocal = False
            lblZone.Visible = Not mbLocal
            cboZone.Visible = Not mbLocal

            'init the old password for now
            'moPassword = New PWPassword.PasswordObject 'RJO 03/23/12
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

            'init new IPC and new Password 'RJO 03/23/12
            oIPC = New Paintworks_IPC.clsInterProcessComm
            moPassword = New clsPWUser
            Progress = 50

            mScreenSetup.LoadZoneBox(cboZone, mcolZones, False)

            Progress = 30

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

            mnuSavePLCToAll.Visible = Not mcolZones.StandAlone

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
    Private Function bSaveToRobots() As Boolean
        '********************************************************************************************
        'Description:  Update all the robot controllers
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/24/11  MSW     Improve error handling
        '********************************************************************************************
        Try
            Dim bReturn As Boolean = True
            For nRobot As Integer = 1 To colControllers.Count
                Try
                    colControllers(nRobot - 1).Robot.SysInfo.Clock = mRobotDate(nRobot - 1)
                    subUpdateChangeLog(mRobotDate(nRobot - 1).ToString(), msOldRobotTime(nRobot - 1), mcolZones.ActiveZone, _
                        colControllers(nRobot - 1).Name, _
                        (gpsRM.GetString("psROBOT") & " " & gpsRM.GetString("psDATETIME")))
                Catch ex As Exception
                    Trace.WriteLine(ex.Message)
                    bReturn = False
                End Try
            Next
            Return bReturn
        Catch ex As Exception
            Return False
        End Try

    End Function
    Private Function bLoadFromRobots() As Boolean
        '********************************************************************************************
        'Description:  Update all the robot controllers
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            For nRobot As Integer = 1 To colControllers.Count
                Try
                    Dim lblRCDate As Label = DirectCast(pnlMain.Controls("lblRCDate" & nRobot.ToString), Label)
                    Dim lblRCTime As Label = DirectCast(pnlMain.Controls("lblRCTime" & nRobot.ToString), Label)
                    If colControllers(nRobot - 1).Robot.IsConnected Then
                        mRobotDate(nRobot - 1) = colControllers(nRobot - 1).Robot.SysInfo.Clock
                        lblRCDate.Text = mRobotDate(nRobot - 1).ToString("D")
                        lblRCTime.Text = mRobotDate(nRobot - 1).ToString("T")
                    Else
                        lblRCDate.Text = String.Empty
                        lblRCTime.Text = String.Empty
                    End If
                Catch ex As Exception
                    'Get what we can
                End Try
            Next
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function
    Private Function bInitPlc() As Boolean
        '********************************************************************************************
        'Description:  Setup the PLC hotlink
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTag As String
        Dim sData(4) As String


        Try
            If mPLC Is Nothing Then
                mPLC = New clsPLCComm
            End If

            With mPLC
                .ZoneName = mcolZones.ActiveZone.Name
                'sTag = "Z" & mcolZones.CurrentZoneNumber.ToString & "PLCDateTime"
                sTag = "PLCDateTime" 'NVSP 12/08/2016 No need of zone specific only one plc
                .TagName = sTag
                msPLCData = .PLCData
            End With
            mPLC_NewData(mcolZones.ActiveZone.Name, sTag, msPLCData)
            'let hotlink fire
            System.Windows.Forms.Application.DoEvents()
            Return (True)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
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
            Progress = 50


            Try
                'GUI Time
                mGUIDate = Date.Now
                mbEventBlocker = True
                dtpDate.Value = mGUIDate
                dtpTime.Value = mGUIDate
                mbEventBlocker = False
                lblGUIDate.Text = mGUIDate.ToString("D")
                lblGUITime.Text = mGUIDate.ToString("T")
                DataLoaded = bLoadFromRobots()
                If Not (mcolZones.StandAlone) Then
                    DataLoaded = DataLoaded And bInitPlc()
                End If
                EditsMade = False
            Catch ex As Exception
                DataLoaded = False
            End Try

            Progress = 98

            If DataLoaded Then
                tmrRefresh.Enabled = True
                Status = gcsRM.GetString("csLOADDONE", DisplayCulture)

                subShowNewPage(True)

                'this resets edit flag
                DataLoaded = True

                Status = gcsRM.GetString("csLOADDONE", DisplayCulture)

            Else
                Status = gcsRM.GetString("csLOADFAILED", DisplayCulture)
                'load from DB failed
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
    Private Sub mPLC_NewData(ByVal ZoneName As String, ByVal TagName As String, _
                                                ByVal Data() As String) Handles mPLC.NewData
        '********************************************************************************************
        'Description:  Incoming Hotlink Data
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            msPLCData = Data
            Dim nPLCData(5) As Integer
            Dim bExit As Boolean = False
            'If we have some PLC data, convert it
            If msPLCData IsNot Nothing AndAlso msPLCData.GetUpperBound(0) >= 5 Then
                For nItem As Integer = 0 To 5
                    If IsNumeric(msPLCData(nItem)) Then
                        nPLCData(nItem) = CInt(msPLCData(nItem))
                    Else
                        bExit = True
                        Exit For
                    End If
                Next
                If Not (bExit) Then
                    mPLCDate = New Date((nPLCData(0)), (nPLCData(1)), (nPLCData(2)), (nPLCData(3)), (nPLCData(4)), (nPLCData(5)))
                    lblPLCDate.Text = mPLCDate.ToString("D")
                    lblPLCTime.Text = mPLCDate.ToString("T")
                End If
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: NewData", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Private Sub tmrRefresh_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrRefresh.Tick
        '********************************************************************************************
        'Description:  Update GUI and robot times
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            tmrRefresh.Enabled = False
            mGUIDate = Date.Now
            lblGUIDate.Text = mGUIDate.ToString("D")
            lblGUITime.Text = mGUIDate.ToString("T")
            bLoadFromRobots()
            'Keep the edit boxes updated if they haven't changed yet.
            If EditsMade = False Then
                mbEventBlocker = True
                dtpDate.Value = mGUIDate
                dtpTime.Value = mGUIDate
                mbEventBlocker = False
            End If
            tmrRefresh.Enabled = True

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: Tick", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
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
        '********************************************************************************************

        'mPWCommon.SaveToChangeLog(mcolZones.ActiveZone.DatabasePath) 'Access Database
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
                subEnableControls(False)
                Exit Sub

            Case Else
                'ok
        End Select

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

        Try
            tmrRefresh.Enabled = False

            msOldGUITime = mGUIDate.ToString()
            msOldPLCTime = mGUIDate.ToString()
            ReDim msOldRobotTime(colControllers.Count - 1)

            Status = gcsRM.GetString("csSAVINGDATA", DisplayCulture)
            mGUIDate = New DateTime(dtpDate.Value.Year, dtpDate.Value.Month, dtpDate.Value.Day, dtpTime.Value.Hour, dtpTime.Value.Minute, dtpTime.Value.Second)
            mPLCDate = mGUIDate
            If mcolZones.MISComputer = False Then
            For nRobot As Integer = 0 To colControllers.Count - 1
                msOldRobotTime(nRobot) = mRobotDate(nRobot).ToString
                mRobotDate(nRobot) = mGUIDate
            Next
            End If
            ' do save
            Dim bResult As Boolean = bSaveToGUI()
            bResult = bResult And bSaveToPLC()
            If mcolZones.MISComputer = False Then
                bResult = bResult And bSaveToRobots()
            End If
            If bResult Then
                Call subLogChanges()
                Call subLoadData()
                ' save done
                EditsMade = False

                subShowNewPage(True)
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
        ' 04/16/13  MSW     Standalone ChangeLog                                          4.01.05.00
        '********************************************************************************************

        Try
            mPWCommon.subShowChangeLog(mcolZones.CurrentZone, gsChangeLogArea, msSCREEN_NAME, gcsRM.GetString("csALL"), _
                                        gcsRM.GetString("csALL"), mDeclares.eParamType.None, nIndex, True, False, oIPC)
        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        End Try

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
        '********************************************************************************************
        mbEventBlocker = BlockEvents
        Dim bTmp As Boolean = False


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


    Private Overloads Sub subUpdateChangeLog(ByRef NewValue As String, ByRef OldValue As String, _
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
		'    09/30/13   MSW     Save screenshots as jpegs
        '********************************************************************************************
        Dim sKeyValue As String = String.Empty

        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode
                Case Keys.F1
                    'Help Screen request
                    sKeyValue = "btnHelp"
                    subLaunchHelp(gs_HELP_UTILITIES_CLOCK, oIPC)

                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty

                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath & msSCREEN_DUMP_NAME, Imaging.ImageFormat.Jpeg)

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
        'TODO - find out what this -70 is from.
        Dim nHeight As Integer = tscMain.ContentPanel.Height - 70
        Dim nWidth As Integer = tscMain.ContentPanel.Width - (pnlMain.Left * 2)

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



    Private Sub cboZone_SelectionChangeCommitted1(ByVal sender As Object, ByVal e As System.EventArgs) _
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

    Private Sub dtp_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpDate.ValueChanged, dtpTime.ValueChanged
        '********************************************************************************************
        'Description:  Date or time edited, stop auto-updating
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If (mbEventBlocker = False) Then
            EditsMade = True
        End If
    End Sub
    Private Sub mnuSaveToAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSaveGUIToAll.Click, mnuSavePLCToAll.Click
        '********************************************************************************************
        'Description:  save to all menu click, each robot submenu gets an addhandler when it's created
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oMnu As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        ' set the time setting box to the selected time source
        Try
            Select Case oMnu.Name
                Case mnuSaveGUIToAll.Name
                    dtpDate.Value = mGUIDate
                    dtpTime.Value = mGUIDate
                Case mnuSavePLCToAll.Name
                    dtpDate.Value = mPLCDate
                    dtpTime.Value = mPLCDate
                Case Else
                    'Robot submenu "mnuSaveTimeR" & nRobot.ToString
                    Dim sRobot As String = oMnu.Name.Substring(12)
                    mbEventBlocker = True
                    dtpDate.Value = mRobotDate(CInt(sRobot))
                    dtpTime.Value = mRobotDate(CInt(sRobot))
                    mbEventBlocker = False
            End Select
            subSaveData()
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuSaveToAll_Click", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

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
                Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/23/12
            End If
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
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/23/12
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

            'If moPrivilege.Privilege = String.Empty Then 'RJO 03/23/12
            '    cachePrivilege = ePrivilege.None
            'Else
            '    If moPrivilege.ActionAllowed Then
            '        cachePrivilege = mPassword.PrivilegeStringToEnum(moPrivilege.Privilege)
            '    Else
            '        cachePrivilege = ePrivilege.None
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
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub

                subSaveData()

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

End Class