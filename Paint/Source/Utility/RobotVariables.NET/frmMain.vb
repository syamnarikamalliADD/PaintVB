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
' Description: RobotVariables
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
' Date          By      Reason                                                        Version
' 05/19/2010    MSW     1st draft
' 10/11/10      MSW     add missing psSCREENCAPTION	to ProjectStrings.resx
' 09/14/11      MSW     Assemble a standard version of everything                       4.1.0.0
'    12/02/11   MSW     Add Dmoncfg reference                                            4.1.1.0
'    01/24/12   MSW     Placeholder include updates                                   4.01.01.02
'    02/15/12   MSW     Force 32 bit build for PCDK compatability                     4.01.01.03
'    03/23/12   RJO     modified for .NET Password and IPC                            4.01.02.00
'    04/11/12   MSW     Change CommonStrings setup so it builds correctly             4.01.03.00
'    04/20/12   MSW     frmMain_KeyDown-Add excape key handler                        4.01.03.01
'    06/07/12   MSW     subWriteToRobots - Add waitcursor during write                4.01.03.02
'    09/19/12   MSW     subWriteToRobots - Reset the cursor before  all the exit subs 4.01.03.03
'    10/24/12   RJO     Added StartupModule to project to prevent multiple instances  4.01.03.04
'    04/16/13   MSW     Add Canadian language files                                   4.01.05.00
'                       Standalone Changelog
'    07/09/13   MSW     Update and standardize logos                                  4.01.05.01
'    07/25/13   MSW     subFormatScreenLayout - fix code that hides extra rc labels   4.01.05.02
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up  4.01.05.03
'    09/30/13   MSW     Save screenshots as jpegs                                     4.01.06.00
'    01/06/14   MSW     Fix write to an uninit var                                    4.01.06.01
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call              4.01.07.00
'**************************************************************************************************


'******** Unfinished Business   *******************************************************************
'******** End Unfinished Business   ***************************************************************

Imports Response = System.Windows.Forms.DialogResult
Imports System.Xml
Imports System.Xml.XPath

Imports ConnStat = FRRobotNeighborhood.FRERNConnectionStatusConstants

Friend Class frmMain

#Region " Declares "
    '******** Form Constants   *********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Public Const msSCREEN_NAME As String = "RobotVariables"   ' <-- For password area change log etc.
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
    Private msOldRobot As String = String.Empty
    Private mcolZones As clsZones = Nothing
    Private mController As clsController = Nothing
    Private mbLocal As Boolean
    Private mbScreenLoaded As Boolean
    Private mbEventBlocker As Boolean
    Private mbRemoteZone As Boolean
    Private mnPopUpMenuRobotNumber As Integer = 0
    Private mnodeSystem As TreeNode = Nothing
    Private mnodePrograms As TreeNode = Nothing
    Private moSysVars As FRRobot.FRCVars = Nothing
    Private moPrograms As FRRobot.FRCPrograms = Nothing
    Delegate Sub ControllerStatusChange(ByVal Controller As clsController)
    Private msSCREEN_DUMP_NAME As String = "Utility_RobotVariables.jpg" 'changes with selected tab
    Private msSCREEN_DUMP_NAME_BROWSE As String = "Utility_RobotVariables_Browse.jpg" 'changes with selected tab
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private mbEditsMade As Boolean
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean
    Private mnProgress As Integer
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    Private Const msXMLFILENAME As String = "RobotVariables.xml"
    Private Const msPROGRAMS As String = "PROGRAMS"
    Private Const msPROGRAMSTAG As String = "*PROGRAMS*"
    Private Const msSYSTEM As String = "*SYSTEM*"
    Private msXMLFilePath As String = String.Empty
    Private mbAutoSetRobotCheckBoxes As Boolean = True
    Private Structure tVarSetupType
        Dim sName As String
        Dim sDesc As String
        Dim sVar As String
        Dim sFile As String
    End Structure
    Private tVarSetup() As tVarSetupType = Nothing
    Private mnVarIndex As Integer = -1

    'jbw testing
    Private mnVARINDEXSELECTED As Integer = 0
    '******** End Property Variables    *************************************************************

    '******** Structures    *************************************************************************
    '******** End Structures    *********************************************************************

    Private WithEvents colControllers As clsControllers = Nothing
    '******** This is the old pw3 password object interop  ******************************************
    'Friend WithEvents moPassword As PWPassword.PasswordObject 'RJO 03/23/12
    'Friend moPrivilege As PWPassword.cPrivilegeObject 'RJO 03/23/12

    Friend WithEvents moPassword As clsPWUser 'RJO 03/23/12
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm 'RJO 03/23/12

    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    Delegate Sub NewMessage_CallBack(ByVal Schema As String, ByVal DS As DataSet)  'RJO 03/23/12
    'Delegate Sub ControllerStatusChange(ByVal Controller As clsController)
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

            'this is needed with old password object raising events
            'and can hopefully be removed when password is redone
           'Control.CheckForIllegalCrossThreadCalls = False

            mPrivilegeRequested = Value

            'handle logout
            If mPrivilegeRequested = ePrivilege.None Then
                mPrivilegeGranted = ePrivilege.None
                subEnableControls(True)  ' This is confusing True but really false sub will look at privilege
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
    Private Sub MakeButton(ByRef rControl As Button, ByVal sName As String, _
                  ByVal rLoc As Point, ByVal rModel As Button)
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
        rControl = (DirectCast(pnlMain.Controls(sName), Button))
        If rControl Is Nothing Then
            rControl = New Button
            rControl.Name = sName
            rControl.Text = String.Empty
            rControl.Location = rLoc
            rControl.BackColor = rModel.BackColor
            rControl.ContextMenuStrip = rModel.ContextMenuStrip
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
    Private Sub MakeTextBox(ByRef rControl As TextBox, ByVal sName As String, _
                      ByVal rLoc As Point, ByVal rModel As TextBox)
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
        rControl = (DirectCast(pnlMain.Controls(sName), TextBox))
        If rControl Is Nothing Then
            rControl = New TextBox
            rControl.Name = sName
            rControl.Text = String.Empty
            rControl.Location = rLoc
            rControl.ContextMenuStrip = rModel.ContextMenuStrip
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
            rControl.ContextMenuStrip = rModel.ContextMenuStrip
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

    Private Sub MakeCheckBox(ByRef rControl As CheckBox, ByVal sName As String, _
                      ByVal rLoc As Point, ByVal rModel As CheckBox)
        '********************************************************************************************
        'Description:  Make a new checkbox and add it to pnlMain
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        rControl = (DirectCast(pnlMain.Controls(sName), CheckBox))
        If rControl Is Nothing Then
            rControl = New CheckBox
            rControl.Name = sName
            rControl.Text = String.Empty
            rControl.Location = rLoc
            rControl.BackColor = rModel.BackColor
            rControl.ContextMenuStrip = rModel.ContextMenuStrip
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
        ' 07/25/13  MSW     subFormatScreenLayout - fix code that hides extra rc labels   4.01.05.02
        '********************************************************************************************
        Dim nRobot As Integer = 0
        Dim pt2 As Point = Nothing
        Dim nDiff As Integer = 0
        For nRobot = 1 To colControllers.Count
            Dim chkRCName As CheckBox = DirectCast(pnlMain.Controls("chkRC_" & nRobot.ToString), CheckBox)
            Dim lblRCVar As Label = DirectCast(pnlMain.Controls("lblRCVar" & nRobot.ToString), Label)
            If chkRCName Is Nothing Then
                'Make the labels 
                Dim pt As Point = chkRC_1.Location
                nDiff = (chkRC_2.Location.Y - chkRC_1.Location.Y)
                nDiff = nDiff * (nRobot - 1)
                pt.Y = pt.Y + nDiff
                MakeCheckBox(chkRCName, ("chkRC_" & nRobot.ToString), pt, chkRC_1)
                pt.X = lblRCVar1.Location.X
                MakeLabel(lblRCVar, ("lblRCVar" & nRobot.ToString), pt, lblRCVar1)

            End If
            chkRCName.Text = colControllers(nRobot - 1).Name
            lblRCVar.Text = String.Empty
            pt2 = chkRCName.Location 'Save  location to offset the write boxes
            'Set the event handlers
            RemoveHandler chkRCName.CheckedChanged, AddressOf chkRC_CheckedChanged
            RemoveHandler chkRCName.MouseDown, AddressOf chkRC_MouseDown
            RemoveHandler lblRCVar.MouseDown, AddressOf lblRCVar_MouseDown
            AddHandler chkRCName.CheckedChanged, AddressOf chkRC_CheckedChanged
            AddHandler chkRCName.MouseDown, AddressOf chkRC_MouseDown
            AddHandler lblRCVar.MouseDown, AddressOf lblRCVar_MouseDown
        Next
        'offset the write boxes
        nDiff = (chkRC_2.Location.Y - chkRC_1.Location.Y)
        pt2.Y = pt2.Y + nDiff
        btnWriteRC.Location = pt2
        pt2.Y = pt2.Y + nDiff
        txtVarValue.Location = pt2

        nRobot = colControllers.Count + 1
        Dim bDone As Boolean = False
        Do Until bDone
            Dim chkRCName As CheckBox = DirectCast(pnlMain.Controls("chkRC_" & nRobot.ToString), CheckBox)
            If chkRCName IsNot Nothing Then
                chkRCName.Visible = False
                Dim lblRCVar As Label = DirectCast(pnlMain.Controls("lblRCVar" & nRobot.ToString), Label)
                lblRCVar.Visible = False
            Else
                bDone = True
            End If
            nRobot = nRobot + 1
        Loop
        trvBrowse.Location = lstVars.Location
        trvBrowse.Size = lstVars.Size
        subEnableControls(True)
    End Sub
    Private Sub cboRobot_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboRobot.SelectedIndexChanged

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
            subChangeRobot()
        End If

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
        'just in case
        If cboRobot.Text = String.Empty Then
            Exit Sub
        Else
            If cboRobot.Text = msOldRobot Then Exit Sub
        End If
        If msOldRobot <> String.Empty Then _
                                colControllers.Item(msOldRobot).Selected = False

        msOldRobot = cboRobot.Text

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            Progress = 30

            Status = gpsRM.GetString("psSELECTING") & msOldRobot
            mController = colControllers.Item(cboRobot.Text)
            colControllers.Item(cboRobot.Text).Selected = True

            'set the robot checkbox
            mbEventBlocker = True
            For nRobot As Integer = 1 To colControllers.Count
                Dim chkRCName As CheckBox = DirectCast(pnlMain.Controls("chkRC_" & nRobot.ToString), CheckBox)
                If cboRobot.Text = colControllers.Item(nRobot - 1).Name Then
                    chkRCName.Checked = True
                Else
                    If mbAutoSetRobotCheckBoxes Then  'Clear the other robot checkboxes
                        chkRCName.Checked = False
                    End If
                End If
            Next
            subShowNewPage(True, True, True)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Progress = 70
            'setup limits:
            'statusbar text

            DataLoaded = True
            subEnableControls(True)
            mbEventBlocker = False

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

            subFormatScreenLayout()

            SetUpStatusStrip(Me, colControllers)
            mPWRobotCommon.LoadRobotBoxFromCollection(cboRobot, colControllers, False)
            'set the robot checkbox
            mbEventBlocker = True
            mbAutoSetRobotCheckBoxes = True
            For nRobot As Integer = 1 To colControllers.Count
                Dim chkRCName As CheckBox = DirectCast(pnlMain.Controls("chkRC_" & nRobot.ToString), CheckBox)
                If chkRCName IsNot Nothing Then
                    chkRCName.Checked = False
                End If
            Next

            If GetDefaultFilePath(msXMLFilePath, mPWCommon.eDir.XML, String.Empty, msXMLFILENAME) Then
                subReadXMLSetup()
                subShowNewPage(True, True, True)
            Else
                mDebug.WriteEventToLog(msSCREEN_NAME & ":frmMain:subChangeZone", "Could not find configuration file: " & _
                                       msXMLFILENAME)
            End If
            mbEventBlocker = False

            'make sure the robot cartoons get the right color
            colControllers.RefreshConnectionStatus()
            msOldRobot = String.Empty


            If cboRobot.Items.Count = 1 Then
                'this selects the robot in cases of just one zone. fires event to call subchangerobot
                cboRobot.Text = cboRobot.Items(0).ToString
            Else
                'statusbar text
                Status(True) = gcsRM.GetString("csSELECT_ROBOT")
            End If

            Me.stsStatus.Refresh()

            subFormatScreenLayout()

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
        Dim bEditControls As Boolean = False
        btnClose.Enabled = True

        If bEnable = False Then
            btnChangeLog.Enabled = False
            bRestOfControls = False
            pnlMain.Enabled = False
            btnSave.Enabled = False
            btnAdd.Enabled = False
            btnRemove.Enabled = False
            bEditControls = False
        Else
            Select Case Privilege
                Case ePrivilege.None
                    btnChangeLog.Enabled = DataLoaded
                    bRestOfControls = DataLoaded
                    pnlMain.Enabled = DataLoaded
                    btnSave.Enabled = False
                    btnAdd.Enabled = False
                    btnRemove.Enabled = False
                    mnuCopyToAll.Visible = False
                    bEditControls = False
                Case ePrivilege.Edit, ePrivilege.Copy '02/02/10 RJO
                    btnChangeLog.Enabled = DataLoaded
                    bRestOfControls = DataLoaded
                    pnlMain.Enabled = DataLoaded
                    btnSave.Enabled = EditsMade
                    btnAdd.Enabled = DataLoaded
                    btnRemove.Enabled = DataLoaded And rdoList.Checked
                    mnuCopy.Visible = True
                    bEditControls = True
            End Select
        End If
        btnWriteRC.Enabled = bEditControls And (mnVarIndex >= 0)
        txtVarValue.Enabled = bEditControls
        btnDescEdit.Enabled = bEditControls And (mnVarIndex >= 0) And rdoList.Checked
        If colControllers IsNot Nothing Then
            For nRobot As Integer = 1 To colControllers.Count
                Dim chkRCName As CheckBox = DirectCast(pnlMain.Controls("chkRC_" & nRobot.ToString), CheckBox)
                If chkRCName IsNot Nothing Then
                    chkRCName.Enabled = bRestOfControls
                End If
                Dim lblRCVar As Label = DirectCast(pnlMain.Controls("lblRCVar" & nRobot.ToString), Label)
                If lblRCVar IsNot Nothing Then
                    lblRCVar.Enabled = bRestOfControls
                End If
            Next
        End If
        trvBrowse.Enabled = bRestOfControls
        lstVars.Enabled = bRestOfControls
        txtVarName.Enabled = bRestOfControls
        rdoBrowse.Enabled = bRestOfControls
        rdoList.Enabled = bRestOfControls
    End Sub
    Private Sub subClearVarDetails()
        '********************************************************************************************
        'Description: clear var detail labels
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        lblProgName.Text = String.Empty
        lblVarName.Text = String.Empty
        lblVarType.Text = String.Empty
        lblStorage.Text = String.Empty
        lblAccess.Text = String.Empty
        lblMin.Text = String.Empty
        lblMax.Text = String.Empty
        lblStrLen.Text = String.Empty
        txtDescription.Text = String.Empty
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
                lblSelectVariableCap.Text = .GetString("psSELECT_VAR", oCulture)
                lblVariableValueCap.Text = .GetString("psCURRENT_VALUE", oCulture)
                rdoList.Text = .GetString("psLIST", oCulture)
                rdoBrowse.Text = .GetString("psBROWSE", oCulture)
                btnAdd.Text = .GetString("psADD", oCulture)
                btnRemove.Text = .GetString("psREMOVE", oCulture)
                mnuBrowse.Text = .GetString("psBROWSE", oCulture)
                mnuCopy.Text = .GetString("psCOPY")
                mnuCopyToAll.Text = .GetString("psCOPY_TO_ALL")
                txtVarValue.Text = String.Empty
                btnWriteRC.Text = .GetString("psWRITE_TO_RC")
                lblProgNameCap.Text = .GetString("psPROG_NAME_CAP")
                lblVarNameCap.Text = .GetString("psVAR_NAME_CAP")
                lblVarTypeCap.Text = .GetString("psVAR_TYPE_CAP")
                lblStorageCap.Text = .GetString("psSTORAGE_CAP")
                lblAccessCap.Text = gpsRM.GetString("psACCESS_CAP")
                lblMinCap.Text = .GetString("psMIN_CAP")
                lblMaxCap.Text = .GetString("psMAX_CAP")
                lblStrLenCap.Text = .GetString("psSTR_LEN_CAP")
                lblDescriptionCap.Text = .GetString("psDESCRIPTION_CAP")
                btnDescEdit.Text = .GetString("psEDIT")
            End With 'gpsRM
            mnuSelectAll.Text = gcsRM.GetString("csSELECT_ALL")
            mnuUnselectAll.Text = gcsRM.GetString("csUNSELECT_ALL")
            subClearVarDetails()
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subInitFormText", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Friend ReadOnly Property XMLPath() As String
        '********************************************************************************************
        'Description:  return where we are looking for print options
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return msXMLFilePath
        End Get

    End Property

    Private Sub subReadXMLSetup()
        '********************************************************************************************
        'Description:  Read settings from XML file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sPath As String = "//RobotVariable"
        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument

        Try
            oXMLDoc.Load(XMLPath)

            oMainNode = oXMLDoc.SelectSingleNode("//RobotVariables")
            oNodeList = oMainNode.SelectNodes(sPath)
            Try
                If oNodeList.Count = 0 Then
                    tVarSetup = Nothing
                    mDebug.WriteEventToLog(msSCREEN_NAME & ":frmMain:subReadXMLSetup", "RobotVariables not found.")
                Else
                    ReDim tVarSetup(oNodeList.Count - 1)
                    Dim nNodeOffset As Integer = 0
                    Dim bError As Boolean = False
                    For nNode As Integer = 0 To oNodeList.Count - 1
                        Try
                            tVarSetup(nNodeOffset).sName = oNodeList(nNode).Item("Name").InnerXml
                            tVarSetup(nNodeOffset).sDesc = oNodeList(nNode).Item("Description").InnerXml
                            nNodeOffset += 1
                        Catch ex As Exception
                            bError = True
                        End Try
                    Next
                    'Clear out the errors
                    ReDim Preserve tVarSetup(nNodeOffset - 1)
                    If bError Then
                        'Offer to cleanup the XML file
                    End If
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & ":frmMain:subReadXMLSetup", "Invalid XML Data: [" & sPath & "] - " & ex.Message)
                tVarSetup = Nothing
            End Try
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & ":frmMain:subReadXMLSetup", "Invalid XPath syntax: [" & sPath & "] - " & ex.Message)
            tVarSetup = Nothing
        End Try
    End Sub
    Private Function bSaveXMLSetup() As Boolean
        '********************************************************************************************
        'Description:  Save settings to XML file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sPath As String = String.Empty
        Dim sTopic As String = String.Empty
        Dim oNode As XmlNode = Nothing
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument

        Try
            If (IO.File.Exists(msXMLFilePath) = False) Then
                IO.File.Create(msXMLFilePath)
                oXMLDoc = New XmlDocument
                oXMLDoc.CreateElement("RobotVariables")
                oMainNode = oXMLDoc.SelectSingleNode("//RobotVariables")
            Else
                Try
                    oXMLDoc.Load(msXMLFilePath)
                    oMainNode = oXMLDoc.SelectSingleNode("//RobotVariables")
                Catch ex As Exception
                    oXMLDoc = New XmlDocument
                    oMainNode = oXMLDoc.CreateElement("RobotVariables")
                    oXMLDoc.AppendChild(oMainNode)
                    oMainNode = oXMLDoc.SelectSingleNode("//RobotVariables")
                End Try
            End If
            'JBW TESTING
            oXMLDoc.RemoveAll()
            oMainNode = oXMLDoc.CreateElement("RobotVariables")
            oXMLDoc.AppendChild(oMainNode)
            oMainNode = oXMLDoc.SelectSingleNode("//RobotVariables")
            'END JBW TESTING
            For Each oVarItem As tVarSetupType In tVarSetup
                sPath = "//RobotVariable[Name='" & oVarItem.sName & "']"
                oNodeList = oMainNode.SelectNodes(sPath)
                If oNodeList.Count = 0 Then
                    oNode = oXMLDoc.CreateElement("RobotVariable")
                    Dim oNameNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Name", Nothing)
                    Dim oDescNode As XmlNode = oXMLDoc.CreateNode(XmlNodeType.Element, "Description", Nothing)
                    If oVarItem.sName <> Nothing Then
                        oNameNode.InnerXml = oVarItem.sName
                        oDescNode.InnerXml = oVarItem.sDesc
                        oNode.AppendChild(oNameNode)
                        oNode.AppendChild(oDescNode)
                        oMainNode.AppendChild(oNode)
                    End If

                Else
                    If oNode.Name <> Nothing Then
                        oNode = oNodeList(0) 'Should only be one match!!!
                        oNode.Item("Name").InnerXml = oVarItem.sName
                        oNode.Item("Description").InnerXml = oVarItem.sDesc
                    End If

                End If
            Next
            Dim oIOStream As System.IO.StreamWriter = New System.IO.StreamWriter(msXMLFilePath)
            Dim oWriter As XmlTextWriter = New XmlTextWriter(oIOStream)
            oWriter.Formatting = Formatting.Indented
            oXMLDoc.WriteTo(oWriter)
            oWriter.Close()
            oIOStream.Close()
            Return True
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & ":frmMain:subSaveXMLSetup", "Invalid XPath syntax: [" & sPath & "] - " & ex.Message)
            Return False
        End Try
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
            'Show the Zone combobox only if this computer connects to remote zones
            mbLocal = True
            For Each oZone As clsZone In mcolZones
                If oZone.IsRemoteZone Then
                    mbLocal = False
                    Exit For
                End If
            Next
            'lblZone.Visible = Not mbLocal
            'cboZone.Visible = Not mbLocal

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

            mScreenSetup.LoadZoneBox(cboZone, mcolZones, False)

            Progress = 30

            'init new IPC and new Password 'RJO 03/23/12
            oIPC = New Paintworks_IPC.clsInterProcessComm
            moPassword = New clsPWUser
            Progress = 50

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
            'If mbLocal Then
            '    cboZone.Text = cboZone.Items(0).ToString
            '    Call subChangeZone()
            'End If
            If cboZone.Items.Count = 1 Then
                'this selects the zone in cases of just one zone. fires event to call subchangezone
                cboZone.Text = cboZone.Items(0).ToString
                subChangeZone()
            Else
                'statusbar text
                Status(True) = gcsRM.GetString("csSELECT_ZONE")
                subEnableControls(True)
            End If
            Me.stsStatus.Refresh()

            ' refresh lock pic
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
        '********************************************************************************************
        Try
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function
    Private Function oReadFromRobot(ByRef oVarSetup As tVarSetupType, ByVal nRobot As Integer) As FRRobot.FRCVar
        '********************************************************************************************
        'Description:  Read from a controller
        '
        'Parameters: oVarSetup - variable details
        'Returns:    an FRCVar object
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim chkRCName As CheckBox = DirectCast(pnlMain.Controls("chkRC_" & nRobot.ToString), CheckBox)
            Dim lblRCVar As Label = DirectCast(pnlMain.Controls("lblRCVar" & nRobot.ToString), Label)
            If chkRCName.Checked Then
                If colControllers(nRobot - 1).Robot.IsConnected Then
                    Dim bSaveable As Boolean = False
                    Dim oTmp As Object = oValidVariable(colControllers(nRobot - 1), oVarSetup.sName, oVarSetup.sFile, oVarSetup.sVar, bSaveable, False, True)
                    Dim oVar As FRRobot.FRCVar = Nothing
                    If oTmp IsNot Nothing Then
                        If TypeOf (oTmp) Is FRRobot.FRCVar Then
                            oVar = DirectCast(oTmp, FRRobot.FRCVar)
                            oVar.Refresh()
                            If oVar.IsInitialized Then
                                lblRCVar.Text = oVar.Value.ToString
                                lblRCVar.ForeColor = Color.Black
                                Return oVar
                            Else
                                lblRCVar.Text = gpsRM.GetString("psUNINITIALIZED", DisplayCulture)
                                lblRCVar.ForeColor = Color.Red
                                Return oVar
                            End If
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCVars Then
                            'Do nothing
                        ElseIf (TypeOf (oTmp) Is Double) Or (TypeOf (oTmp) Is Integer) Or _
                             (TypeOf (oTmp) Is Short) Or (TypeOf (oTmp) Is Byte) Or _
                             (TypeOf (oTmp) Is Long) Or (TypeOf (oTmp) Is Single) Or _
                             (TypeOf (oTmp) Is Boolean) Or (TypeOf (oTmp) Is Decimal) Or _
                             (TypeOf (oTmp) Is String) Then
                            lblRCVar.Text = oTmp.ToString
                            lblRCVar.ForeColor = Color.Black
                            Return oVar
                        ElseIf oTmp IsNot Nothing Then
                            lblRCVar.Text = gpsRM.GetString("psVARIABLE", DisplayCulture) & _
                                gpsRM.GetString("psIS_NOT_AVAILABLE", DisplayCulture)
                            lblRCVar.ForeColor = Color.Red
                        Else
                            lblRCVar.Text = gpsRM.GetString("psVARIABLE", DisplayCulture) & _
                                gpsRM.GetString("psIS_NOT_AVAILABLE", DisplayCulture)
                            lblRCVar.ForeColor = Color.Red
                        End If
                    End If
                Else
                    lblRCVar.Text = gpsRM.GetString("psCONTROLLER_NOT_AVAILABLE", DisplayCulture)
                    lblRCVar.ForeColor = Color.Red
                End If
            Else
                lblRCVar.Text = String.Empty
                lblRCVar.ForeColor = Color.Black
            End If

        Catch ex As Exception
            'Get what we can
        End Try
        Return Nothing
    End Function

    Private Function oReadFromRobots(ByRef oVarSetup As tVarSetupType) As FRRobot.FRCVar
        '********************************************************************************************
        'Description:  Read from all selected controllers
        '
        'Parameters: none
        'Returns:    an FRCVar object - from any robot.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oVar As FRRobot.FRCVar = Nothing
        Dim oVarReturn As FRRobot.FRCVar = Nothing
        For nRobot As Integer = 1 To colControllers.Count
            Try
                oVar = oReadFromRobot(oVarSetup, nRobot)
                If oVar IsNot Nothing Then
                    'If it's defined on some robots, but not all then return one of the good ones.
                    oVarReturn = oVar
                End If
            Catch ex As Exception
                'Get what we can
            End Try
        Next
        Return oVarReturn
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

            Progress = 98

            If DataLoaded Then
                tmrRefresh.Enabled = True
                Status = gcsRM.GetString("csLOADDONE", DisplayCulture)

                subShowNewPage(True)

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
            If bSaveXMLSetup() Then
                EditsMade = False
            End If
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

            subLogChanges()
            
            mPWCommon.subShowChangeLog(mcolZones.CurrentZone, gsChangeLogArea, msSCREEN_NAME, gcsRM.GetString("csALL"), _
                                        gcsRM.GetString("csALL"), mDeclares.eParamType.None, nIndex, True, False, oIPC)
        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR", DisplayCulture), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
                                
        End Try

    End Sub

    Private Sub subShowNewPage(ByVal BlockEvents As Boolean, _
                               Optional ByVal bRefreshList As Boolean = False, _
                               Optional ByVal bRefreshBrowse As Boolean = False)
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
        If rdoBrowse.Checked Then
            trvBrowse.Visible = True
            lstVars.Visible = False
        Else
            trvBrowse.Visible = False
            lstVars.Visible = True
        End If

        If bRefreshList Then
            lstVars.Items.Clear()
            txtVarName.Text = String.Empty
            If tVarSetup IsNot Nothing Then
                For Each oVarSetup As tVarSetupType In tVarSetup
                    If oVarSetup.sName <> Nothing Then
                        lstVars.Items.Add(oVarSetup.sName)
                    End If
                Next
            End If
        End If
        If bRefreshBrowse Then
            trvBrowse.Nodes.Clear()
            If cboRobot.SelectedIndex >= 0 Then
                mnodeSystem = New TreeNode(msSYSTEM)
                mnodeSystem.Tag = msSYSTEM
                trvBrowse.Nodes.Add(mnodeSystem)
                moSysVars = mController.Robot.SysVariables
                For nSysVar As Integer = 0 To moSysVars.Count - 1
                    If TypeOf (moSysVars.Item(, nSysVar)) Is FRRobot.FRCVars Then
                        Try
                            Dim oVars As FRRobot.FRCVars = DirectCast(moSysVars.Item(, nSysVar), FRRobot.FRCVars)
                            If oVars IsNot Nothing Then
                                Dim newNode As New TreeNode(oVars.FieldName)
                                newNode.Tag = oVars.VarName
                                mnodeSystem.Nodes.Add(newNode)
                            End If
                        Catch ex As Exception
                        End Try
                    ElseIf TypeOf (moSysVars.Item(, nSysVar)) Is FRRobot.FRCVar Then
                        Try
                            Dim oVar As FRRobot.FRCVar = DirectCast(moSysVars.Item(, nSysVar), FRRobot.FRCVar)
                            If oVar IsNot Nothing Then
                                Dim newNode As New TreeNode(oVar.FieldName)
                                newNode.Tag = oVar.VarName
                                mnodeSystem.Nodes.Add(newNode)
                            End If
                        Catch ex As Exception
                        End Try
                    Else
                        Debug.Print("???")
                    End If
                Next
                mnodePrograms = New TreeNode(msPROGRAMS)
                mnodePrograms.Tag = msPROGRAMSTAG
                trvBrowse.Nodes.Add(mnodePrograms)
                moPrograms = mController.Robot.Programs
                For Each oProg As FRRobot.FRCProgram In moPrograms
                    If oProg IsNot Nothing Then
                        Dim oVars As FRRobot.FRCVars = oProg.Variables
                        If oVars IsNot Nothing AndAlso oVars.Count > 0 Then
                            Dim newNode As New TreeNode(oProg.Name)
                            newNode.Tag = "[" & oProg.Name & "]"
                            mnodePrograms.Nodes.Add(newNode)
                        End If
                    End If
                Next

            End If
        End If
        subEnableControls(True)
        mbEventBlocker = False

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

    Private Sub rdo_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoBrowse.CheckedChanged, rdoList.CheckedChanged
        '********************************************************************************************
        'Description:  update display for rdo selection
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim oRdo As RadioButton = DirectCast(sender, RadioButton)
            Debug.Print(oRdo.Name)
            Debug.Print(oRdo.Checked.ToString)
            btnRemove.Enabled = rdoList.Checked
            subShowNewPage(True)
        Catch ex As Exception

        End Try
    End Sub
    Private Function oParseConfig(ByRef oConfig As FRRobot.FRCConfig, ByRef sVarArray() As String, _
                            ByVal nIndex As Integer) As Object

        Select Case sVarArray(nIndex)
            Case "FLIP"
                Return oConfig.Flip
            Case "FRONT"
                Return oConfig.Front
            Case "LEFT"
                Return oConfig.Left
            Case "UP"
                Return oConfig.Up
            Case "TEXT"
                Return oConfig.Text
            Case Else
                Return oConfig
        End Select
    End Function
    Private Function oValidVariable(ByRef oCntr As clsController, ByRef sName As String, _
                                    ByRef sFile As String, ByRef sVar As String, _
                                    ByRef bSaveable As Boolean, _
                                    Optional ByVal bNoMessage As Boolean = False, _
                                    Optional ByVal bReturnVal As Boolean = False) As Object
        '********************************************************************************************
        'Description:  check for a valid variable
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Parse the name
        Try
            bSaveable = False

            If oCntr.Robot.IsConnected Then
                Dim oVar As FRRobot.FRCVar = Nothing
                Dim oVars As FRRobot.FRCVars = Nothing
                Dim oProg As FRRobot.FRCProgram = Nothing
                Dim sVarArray() As String = Nothing
                'Check for sys var first
                Dim nDollarSign As Integer = InStr(sName, "$")
                If (nDollarSign = 1) Then
                    'prep the sys vars a bit.
                    sFile = msSYSTEM
                    sVar = sName.Substring(nDollarSign - 1)
                    sVarArray = Split(sVar, ".")
                    oVars = oCntr.Robot.SysVariables
                Else
                    Dim nRightBracket As Integer = InStr(sName, "]")
                    If (nRightBracket > 1) AndAlso (sName.Substring(0, 1) = "[") Then
                        sFile = sName.Substring(1, (nRightBracket - 2))
                        sVar = sName.Substring(nRightBracket)
                        sVarArray = Split(sVar, ".")
                    Else
                        sFile = sName
                        sVar = String.Empty
                        'If (bNoMessage = False) Then
                        '    Dim oCulture As System.Globalization.CultureInfo = DisplayCulture
                        '    Dim lRet As Response = MessageBox.Show(gpsRM.GetString("psVARIABLE_NAME", oCulture) & sName & _
                        '                                           gpsRM.GetString("psIS_NOT_VALID", oCulture), _
                        '                    gpsRM.GetString("psINVALID_VAR_NAME", oCulture), _
                        '                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        'End If
                        'Return Nothing
                    End If
                    Dim oList As FRRobot.FRCPrograms
                    oList = oCntr.Robot.Programs
                    oProg = DirectCast(oList.Item(sFile), FRRobot.FRCProgram)
                    If oProg Is Nothing Then
                        If (bNoMessage = False) Then
                            Dim oCulture As System.Globalization.CultureInfo = DisplayCulture
                            Dim lRet As Response = MessageBox.Show(gpsRM.GetString("psPROGRAM", oCulture) & sName & _
                                                                   gpsRM.GetString("psIS_NOT_AVAILABLE", oCulture), _
                                            gpsRM.GetString("psPROGRAM_NOT_AVAILABLE", oCulture), _
                                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        End If
                        Return Nothing
                    Else
                        If sVar = String.Empty Then
                            bSaveable = False
                            Return oProg
                        Else
                            oVars = oProg.Variables
                        End If
                    End If
                End If
                Dim sTmp As String = String.Empty
                Dim sTmpIdx As String = String.Empty
                Dim nLeftBracket As Integer = 0
                For nVar As Integer = 0 To sVarArray.GetUpperBound(0)
                    sTmpIdx = String.Empty
                    nLeftBracket = InStr(sVarArray(nVar), "[")
                    If nLeftBracket > 0 Then
                        'An Array or other complicated type
                        sTmp = sVarArray(nVar).Substring(0, nLeftBracket - 1)

                        'get the index
                        sTmpIdx = sVarArray(nVar).Substring(nLeftBracket, ((sVarArray(nVar).Length - nLeftBracket) - 1))
                    Else
                        sTmp = sVarArray(nVar)
                    End If
                    'Get the var or array first
                    If sTmp <> String.Empty Then
                        If TypeOf (oVars(sTmp)) Is FRRobot.FRCVar Then
                            oVar = DirectCast(oVars(sTmp), FRRobot.FRCVar)
                        Else
                            oVars = DirectCast(oVars(sTmp), FRRobot.FRCVars)
                        End If
                    End If
                    If oVar Is Nothing And oVars IsNot Nothing Then
                        'Array
                        If sTmpIdx <> String.Empty Then
                            Dim sTmp2() As String = Split(sTmpIdx, ",")
                            For nVar2 As Integer = 0 To sTmp2.GetUpperBound(0)
                                Dim oTmpVar As Object = oVars(sTmp2(nVar2))
                                If oTmpVar IsNot Nothing Then
                                    If TypeOf (oTmpVar) Is FRRobot.FRCVars Then
                                        oVars = DirectCast(oTmpVar, FRRobot.FRCVars)
                                    ElseIf TypeOf (oTmpVar) Is FRRobot.FRCVar Then
                                        oVar = DirectCast(oTmpVar, FRRobot.FRCVar)
                                    End If
                                End If
                            Next
                        End If
                    End If
                    If oVar IsNot Nothing Then
                        oVar.Refresh()
                        'Check for a complicated variable
                        Select Case oVar.TypeCode
                            Case FRRobot.FRETypeCodeConstants.frJoint
                                Dim oPos As FRRobot.FRCVarPosition = DirectCast(oVar.Value, FRRobot.FRCVarPosition)
                                If oPos IsNot Nothing AndAlso oPos.IsInitialized Then
                                    Dim oJnt As FRRobot.FRCJoint = DirectCast(oPos.Formats(FRRobot.FRETypeCodeConstants.frJoint), FRRobot.FRCJoint)
                                    Dim oTmp As Object = oJnt
                                    bSaveable = False
                                    If (sTmpIdx <> String.Empty) Then
                                        bSaveable = True
                                        Dim nTmp As Short = CShort(sTmpIdx)
                                        oTmp = oJnt.Item(nTmp)
                                    ElseIf (nVar < sVarArray.GetUpperBound(0)) Then
                                        bSaveable = True
                                        sTmp = sVarArray(nVar + 1)
                                        Dim nTmp As Short = CShort(sTmp)
                                        oTmp = oJnt.Item(nTmp)
                                    End If
                                    If bReturnVal Then
                                        Return oTmp
                                    Else
                                        Return oJnt
                                    End If
                                End If
                            Case FRRobot.FRETypeCodeConstants.frExtTransform, FRRobot.FRETypeCodeConstants.frTransform
                                Dim oPos As FRRobot.FRCVarPosition = DirectCast(oVar.Value, FRRobot.FRCVarPosition)
                                If oPos IsNot Nothing AndAlso oPos.IsInitialized Then
                                    Dim oTrn As FRRobot.FRCTransform = DirectCast(oPos.Formats(FRRobot.FRETypeCodeConstants.frTransform), FRRobot.FRCTransform)
                                    Dim oTmp As Object = oTrn
                                    bSaveable = False

                                    If bReturnVal Then
                                        Return oTmp
                                    Else
                                        Return oTrn
                                    End If
                                End If
                            Case FRRobot.FRETypeCodeConstants.frExtXyz456, FRRobot.FRETypeCodeConstants.frExtXyzAes, _
                                 FRRobot.FRETypeCodeConstants.frExtXyzWpr, FRRobot.FRETypeCodeConstants.frXyz456, _
                                 FRRobot.FRETypeCodeConstants.frXyzAes, FRRobot.FRETypeCodeConstants.frXyzWpr
                                Dim oPos As FRRobot.FRCVarPosition = DirectCast(oVar.Value, FRRobot.FRCVarPosition)
                                If oPos IsNot Nothing AndAlso oPos.IsInitialized Then
                                    Dim oXyz As FRRobot.FRCXyzWpr = DirectCast(oPos.Formats(FRRobot.FRETypeCodeConstants.frXyzWpr), FRRobot.FRCXyzWpr)
                                    Dim oTmpVal As Object = oXyz
                                    Dim oTmpObj As Object = oXyz
                                    bSaveable = False
                                    If (nVar < sVarArray.GetUpperBound(0)) Then
                                        sTmp = sVarArray(nVar + 1)
                                        Select Case sTmp
                                            Case "X"
                                                bSaveable = True
                                                oTmpVal = oXyz.X
                                            Case "Y"
                                                bSaveable = True
                                                oTmpVal = oXyz.Y
                                            Case "Z"
                                                bSaveable = True
                                                oTmpVal = oXyz.Z
                                            Case "W"
                                                bSaveable = True
                                                oTmpVal = oXyz.W
                                            Case "P"
                                                bSaveable = True
                                                oTmpVal = oXyz.P
                                            Case "R"
                                                bSaveable = True
                                                oTmpVal = oXyz.R
                                            Case "CONFIG"
                                                oTmpObj = oXyz.Config
                                                oTmpVal = oXyz.Config
                                                If (nVar + 2 <= sVarArray.GetUpperBound(0)) Then
                                                    bSaveable = True
                                                    oTmpVal = oParseConfig(oXyz.Config, sVarArray, nVar + 2)
                                                End If
                                            Case "EXT"
                                                oTmpObj = oXyz.Ext
                                                oTmpVal = oXyz.Ext
                                                If (sTmpIdx <> String.Empty) Then
                                                    bSaveable = True
                                                    Dim nTmp As Short = CShort(sTmpIdx)
                                                    oTmpVal = oXyz.Ext.Item(nTmp)
                                                ElseIf (nVar + 1 < sVarArray.GetUpperBound(0)) Then
                                                    sTmp = sVarArray(nVar + 2)
                                                    Dim nTmp As Short = CShort(sTmp)
                                                    oTmpVal = oXyz.Ext.Item(nTmp)
                                                End If
                                            Case Else
                                                Return oXyz
                                        End Select
                                    End If
                                    If bReturnVal Then
                                        Return oTmpVal
                                    Else
                                        Return oTmpObj
                                    End If
                                End If

                            Case FRRobot.FRETypeCodeConstants.frConfigType
                                Dim oConfig As FRRobot.FRCConfig = DirectCast(oVar.Value, FRRobot.FRCConfig)
                                Dim oTmp As Object = oConfig
                                bSaveable = False
                                If (nVar < sVarArray.GetUpperBound(0)) Then
                                    bSaveable = True
                                    oTmp = oParseConfig(oConfig, sVarArray, nVar + 1)
                                End If
                                If bReturnVal Then
                                    Return oTmp
                                Else
                                    Return oConfig
                                End If
                            Case FRRobot.FRETypeCodeConstants.frVectorType
                                Dim oVector As FRRobot.FRCVector = DirectCast(oVar.Value, FRRobot.FRCVector)
                                Dim oTmp As Object = oVector
                                bSaveable = False
                                If (sTmp <> String.Empty) Then
                                    Dim nTmp As Short = CShort(sTmp)
                                    bSaveable = True
                                    oTmp = oVector.Item(nTmp)
                                ElseIf (nVar < sVarArray.GetUpperBound(0)) Then
                                    sTmp = sVarArray(nVar + 1)
                                    Select Case sTmp
                                        Case "X"
                                            bSaveable = True
                                            oTmp = oVector.X
                                        Case "Y"
                                            bSaveable = True
                                            oTmp = oVector.Y
                                        Case "Z"
                                            bSaveable = True
                                            oTmp = oVector.Z
                                        Case Else
                                            bSaveable = False
                                    End Select
                                End If
                                If bReturnVal Then
                                    Return oTmp
                                Else
                                    Return oVector
                                End If
                            Case Else
                                'simple variable, leave it alone
                                bSaveable = True
                        End Select
                    End If
                Next
                If oVar IsNot Nothing Then
                    Return oVar
                ElseIf oVars IsNot Nothing Then
                    Return oVars
                ElseIf oProg IsNot Nothing Then
                    Return oProg
                Else
                    If (bNoMessage = False) Then
                        Dim oCulture As System.Globalization.CultureInfo = DisplayCulture
                        Dim lRet As Response = MessageBox.Show(gpsRM.GetString("psVARIABLE", oCulture) & sName & _
                                                               gpsRM.GetString("psIS_NOT_AVAILABLE", oCulture), _
                                        gpsRM.GetString("psVARIABLE_NOT_AVAILABLE", oCulture), _
                                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    End If
                    Return Nothing
                End If
            Else
                If bNoMessage = False Then
                    Dim oCulture As System.Globalization.CultureInfo = DisplayCulture
                    Dim lRet As Response = MessageBox.Show(gpsRM.GetString("psCONTROLLER", oCulture) & oCntr.Name & _
                                                           gpsRM.GetString("psIS_NOT_AVAILABLE", oCulture), _
                                    gpsRM.GetString("psCONTROLLER_NOT_AVAILABLE", oCulture), _
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                End If
                Return Nothing
            End If
        Catch ex As Exception
            If (bNoMessage = False) Then
                Dim oCulture As System.Globalization.CultureInfo = DisplayCulture
                Dim lRet As Response = MessageBox.Show(gpsRM.GetString("psVARIABLE", oCulture) & sName & _
                                                       gpsRM.GetString("psIS_NOT_AVAILABLE", oCulture), _
                                gpsRM.GetString("psVARIABLE_NOT_AVAILABLE", oCulture), _
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
            Return Nothing
        End Try
        Return Nothing
    End Function

    Private Sub trvBrowse_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles trvBrowse.AfterSelect
        '********************************************************************************************
        'Description:  item selected in browse window
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        For nRobot As Integer = 1 To colControllers.Count
            Dim lblRCVar As Label = DirectCast(pnlMain.Controls("lblRCVar" & nRobot.ToString), Label)
            lblRCVar.Text = String.Empty
        Next
        If trvBrowse.SelectedNode.Nodes.Count > 0 Then
            'already has children expanded, don't need to do anything here
            Exit Sub
        End If
        Dim sTag As String = DirectCast(trvBrowse.SelectedNode.Tag, String)
        Dim bSaveable As Boolean = False
        If sTag IsNot Nothing Then
            Select Case sTag 'Select one of the root nodes or any normal node
                Case msSYSTEM
                    'Nothing to do here
                Case msPROGRAMSTAG
                    'Nothing to do here
                Case Else
                    txtVarName.Text = sTag
                    Dim sFile As String = String.Empty
                    Dim sVar As String = String.Empty
                    Dim oTmp As Object = oValidVariable(mController, sTag, sFile, sVar, bSaveable, False, True)
                    Dim oVar As FRRobot.FRCVar = Nothing
                    Dim oVars As FRRobot.FRCVars = Nothing
                    Dim oProg As FRRobot.FRCProgram = Nothing
                    If oTmp IsNot Nothing Then
                        If TypeOf (oTmp) Is FRRobot.FRCVar Then
                            oVar = DirectCast(oTmp, FRRobot.FRCVar)
                            subDisplayVarDetails(oVar, False)
                            Dim oTmpVarSetup As tVarSetupType
                            oTmpVarSetup.sName = sTag
                            oTmpVarSetup.sDesc = String.Empty
                            oTmpVarSetup.sFile = String.Empty
                            oTmpVarSetup.sVar = String.Empty
                            'A variable is selected.  Display it
                            oReadFromRobots(oTmpVarSetup)
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCJoint Then
                            Dim oJnt As FRRobot.FRCJoint = DirectCast(oTmp, FRRobot.FRCJoint)
                            For nVar As Integer = 1 To oJnt.Count
                                Dim newNode As New TreeNode((nVar).ToString)
                                newNode.Tag = sTag & "[" & (nVar).ToString & "]"
                                trvBrowse.SelectedNode.Nodes.Add(newNode)
                            Next
                            subDisplayVarDetails(oJnt)
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCTransform Then
                            Dim oPos As FRRobot.FRCTransform = DirectCast(oTmp, FRRobot.FRCTransform)
                            subDisplayVarDetails(oPos)
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCXyzWpr Then
                            Dim oPos As FRRobot.FRCXyzWpr = DirectCast(oTmp, FRRobot.FRCXyzWpr)
                            subDisplayVarDetails(oPos)
                            Dim newNode As New TreeNode("X")
                            newNode.Tag = sTag & ".X"
                            trvBrowse.SelectedNode.Nodes.Add(newNode)
                            newNode = New TreeNode("Y")
                            newNode.Tag = sTag & ".Y"
                            trvBrowse.SelectedNode.Nodes.Add(newNode)
                            newNode = New TreeNode("Z")
                            newNode.Tag = sTag & ".Z"
                            trvBrowse.SelectedNode.Nodes.Add(newNode)
                            newNode = New TreeNode("W")
                            newNode.Tag = sTag & ".W"
                            trvBrowse.SelectedNode.Nodes.Add(newNode)
                            newNode = New TreeNode("P")
                            newNode.Tag = sTag & ".P"
                            trvBrowse.SelectedNode.Nodes.Add(newNode)
                            newNode = New TreeNode("R")
                            newNode.Tag = sTag & ".R"
                            trvBrowse.SelectedNode.Nodes.Add(newNode)
                            newNode = New TreeNode("EXT")
                            newNode.Tag = sTag & ".EXT"
                            trvBrowse.SelectedNode.Nodes.Add(newNode)
                            newNode = New TreeNode("CONFIG")
                            newNode.Tag = sTag & ".CONFIG"
                            trvBrowse.SelectedNode.Nodes.Add(newNode)
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCConfig Then
                            Dim newNode As New TreeNode("FLIP")
                            newNode.Tag = sTag & ".FLIP"
                            trvBrowse.SelectedNode.Nodes.Add(newNode)
                            newNode = New TreeNode("FRONT")
                            newNode.Tag = sTag & ".FRONT"
                            trvBrowse.SelectedNode.Nodes.Add(newNode)
                            newNode = New TreeNode("LEFT")
                            newNode.Tag = sTag & ".LEFT"
                            trvBrowse.SelectedNode.Nodes.Add(newNode)
                            newNode = New TreeNode("UP")
                            newNode.Tag = sTag & ".UP"
                            trvBrowse.SelectedNode.Nodes.Add(newNode)
                            newNode = New TreeNode("TEXT")
                            newNode.Tag = sTag & ".TEXT"
                            trvBrowse.SelectedNode.Nodes.Add(newNode)
                            Dim oCfg As FRRobot.FRCConfig = DirectCast(oTmp, FRRobot.FRCConfig)
                            subDisplayVarDetails(oCfg)
                            Dim oTmpVarSetup As tVarSetupType
                            oTmpVarSetup.sName = sTag
                            oTmpVarSetup.sDesc = String.Empty
                            oTmpVarSetup.sFile = String.Empty
                            oTmpVarSetup.sVar = String.Empty
                            'It can display a text version
                            oReadFromRobots(oTmpVarSetup)
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCVector Then
                            Dim oVector As FRRobot.FRCVector = DirectCast(oTmp, FRRobot.FRCVector)
                            subDisplayVarDetails(oVector)
                            Dim newNode As New TreeNode("X")
                            newNode.Tag = sTag & ".X"
                            trvBrowse.SelectedNode.Nodes.Add(newNode)
                            newNode = New TreeNode("Y")
                            newNode.Tag = sTag & ".Y"
                            trvBrowse.SelectedNode.Nodes.Add(newNode)
                            newNode = New TreeNode("Z")
                            newNode.Tag = sTag & ".Z"
                            trvBrowse.SelectedNode.Nodes.Add(newNode)
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCAxesCollection Then
                            Dim oAxesCol As FRRobot.FRCAxesCollection = DirectCast(oTmp, FRRobot.FRCAxesCollection)
                            For nVar As Integer = 1 To oAxesCol.Count
                                Dim newNode As New TreeNode((nVar).ToString)
                                newNode.Tag = sTag & "[" & (nVar).ToString & "]"
                                trvBrowse.SelectedNode.Nodes.Add(newNode)
                            Next
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCVars Then
                            oVars = DirectCast(oTmp, FRRobot.FRCVars)
                            subDisplayVarDetails(oVars)
                            'A structure is selected, expand the treeview
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCProgram Then
                            oProg = DirectCast(oTmp, FRRobot.FRCProgram)
                            'A program is selected, expand the treeview
                            oVars = oProg.Variables
                            subDisplayVarDetails(oProg)
                        ElseIf (TypeOf (oTmp) Is Double) Or (TypeOf (oTmp) Is Single) Or _
                         (TypeOf (oTmp) Is Integer) Or (TypeOf (oTmp) Is Short) Or _
                          (TypeOf (oTmp) Is Byte) Or (TypeOf (oTmp) Is Long) Or _
                          (TypeOf (oTmp) Is Decimal) Or (TypeOf (oTmp) Is Boolean) Or _
                          (TypeOf (oTmp) Is String) Then
                            'Readable detail inside a var structure
                            Dim oTmpVarSetup As tVarSetupType
                            oTmpVarSetup.sName = sTag
                            oTmpVarSetup.sDesc = String.Empty
                            oTmpVarSetup.sFile = String.Empty
                            oTmpVarSetup.sVar = String.Empty
                            'It can display a text version
                            oReadFromRobots(oTmpVarSetup)
                        End If
                        If oVars IsNot Nothing Then
                            If InStr("$", sTag) > 0 Then
                                sTag = String.Empty
                            Else
                                sTag = "[" & sFile & "]"
                            End If
                            For nVar As Integer = 0 To oVars.Count - 1
                                If TypeOf (oVars.Item(, nVar)) Is FRRobot.FRCVars Then
                                    Try
                                        Dim oVars2 As FRRobot.FRCVars = DirectCast(oVars.Item(, nVar), FRRobot.FRCVars)
                                        If oVars IsNot Nothing Then
                                            Dim newNode As New TreeNode(oVars2.FieldName)
                                            newNode.Tag = sTag & oVars2.VarName
                                            trvBrowse.SelectedNode.Nodes.Add(newNode)
                                        End If
                                    Catch ex As Exception
                                    End Try
                                ElseIf TypeOf (oVars.Item(, nVar)) Is FRRobot.FRCVar Then
                                    Try
                                        Dim oVar2 As FRRobot.FRCVar = DirectCast(oVars.Item(, nVar), FRRobot.FRCVar)
                                        If oVar2 IsNot Nothing Then
                                            Dim newNode As New TreeNode(oVar2.FieldName)
                                            newNode.Tag = sTag & oVar2.VarName
                                            trvBrowse.SelectedNode.Nodes.Add(newNode)
                                        End If
                                    Catch ex As Exception
                                    End Try
                                Else
                                    Debug.Print("???")
                                End If
                            Next
                        End If
                    End If
                    'oVars = oValidVars(sTag)
            End Select
        Else
            ''sTag not set.  Find child nodes.

        End If
    End Sub
    Private Function bVarAdd(ByVal sName As String) As Boolean
        '********************************************************************************************
        'Description:  Add selected item to list
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim bExit As Boolean = False

            For nItem As Integer = 0 To lstVars.Items.Count - 1
                If sName.ToLower = lstVars.Items(nItem).ToString.ToLower Then
                    bExit = True
                    lstVars.SelectedItems.Clear()
                    lstVars.SelectedItems.Add(nItem)
                    rdoList.Checked = True
                    Exit For
                End If
            Next
            If (bExit) Then
                Dim oCulture As System.Globalization.CultureInfo = DisplayCulture
                Dim lRet As Response = MessageBox.Show(gpsRM.GetString("psVARIABLE", oCulture) & txtVarName.Text & _
                                                       gpsRM.GetString("psALREADY_EXISTS", oCulture), _
                                gpsRM.GetString("psALREADY_IN_LIST", oCulture), _
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Return False
                EditsMade = True
                rdoList.Checked = True
            Else
                Dim sFile As String = String.Empty
                Dim sVar As String = String.Empty
                Dim bSaveable As Boolean = False
                Dim oTmp As Object = oValidVariable(mController, sName, sFile, sVar, bSaveable, True, False)
                If bSaveable Then
                    If oTmp IsNot Nothing Then
                        If TypeOf (oTmp) Is FRRobot.FRCVar Then
                            Dim oVar As FRRobot.FRCVar = DirectCast(oTmp, FRRobot.FRCVar)
                            subDisplayVarDetails(oVar, False)
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCVars Then
                            Dim oVars As FRRobot.FRCVars = DirectCast(oTmp, FRRobot.FRCVars)
                            subDisplayVarDetails(oVars)
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCJoint Then
                            Dim oJnt As FRRobot.FRCJoint = DirectCast(oTmp, FRRobot.FRCJoint)
                            subDisplayVarDetails(oJnt)
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCTransform Then
                            Dim oPos As FRRobot.FRCTransform = DirectCast(oTmp, FRRobot.FRCTransform)
                            subDisplayVarDetails(oPos)
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCXyzWpr Then
                            Dim oPos As FRRobot.FRCXyzWpr = DirectCast(oTmp, FRRobot.FRCXyzWpr)
                            subDisplayVarDetails(oPos)
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCConfig Then
                            Dim oConfig As FRRobot.FRCConfig = DirectCast(oTmp, FRRobot.FRCConfig)
                            subDisplayVarDetails(oConfig)
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCVector Then
                            Dim oVector As FRRobot.FRCVector = DirectCast(oTmp, FRRobot.FRCVector)
                            subDisplayVarDetails(oVector)
                        End If
                    End If
                    If tVarSetup Is Nothing Then
                        ReDim tVarSetup(0)
                    Else
                        ReDim Preserve tVarSetup(tVarSetup.GetUpperBound(0) + 1)
                    End If
                    tVarSetup(tVarSetup.GetUpperBound(0)).sName = sName
                    tVarSetup(tVarSetup.GetUpperBound(0)).sDesc = String.Empty
                    tVarSetup(tVarSetup.GetUpperBound(0)).sFile = sFile
                    tVarSetup(tVarSetup.GetUpperBound(0)).sVar = sVar
                    subShowNewPage(True, True, False)
                    oReadFromRobots(tVarSetup(tVarSetup.GetUpperBound(0)))
                    lstVars.SelectedItems.Clear()
                    lstVars.SelectedItems.Add(sName)
                    EditsMade = True
                    rdoList.Checked = True
                    Return True
                Else
                    Dim bAdded As Boolean = False
                    If (rdoBrowse.Checked) AndAlso (oTmp IsNot Nothing) AndAlso _
                       (TypeOf (oTmp) Is FRRobot.FRCProgram) Then
                        Dim oProg As FRRobot.FRCProgram = DirectCast(oTmp, FRRobot.FRCProgram)
                        If oProg IsNot Nothing Then
                            Dim oVars As FRRobot.FRCVars = oProg.Variables
                            If oVars IsNot Nothing AndAlso oVars.Count > 0 Then
                                Dim newNode As New TreeNode(oProg.Name)
                                newNode.Tag = "[" & oProg.Name & "]"
                                For Each oNode As TreeNode In trvBrowse.Nodes
                                    If oNode.Tag.ToString = msPROGRAMSTAG Then
                                        oNode.Nodes.Add(newNode)
                                        bAdded = True
                                        Exit For
                                    End If
                                Next

                            End If
                        End If
                    End If
                    If bAdded = False Then
                        Dim oCulture As System.Globalization.CultureInfo = DisplayCulture
                        Dim lRet As Response = MessageBox.Show(gpsRM.GetString("psVARIABLE", oCulture) & txtVarName.Text & _
                                                               gpsRM.GetString("psIS_NOT_VALID", oCulture), _
                                        gpsRM.GetString("psNOT_VALID", oCulture), _
                                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        Return False
                    End If
                End If
            End If
        Catch ex As Exception

        End Try

    End Function
    Private Sub subDisplayVarDetails(ByRef oVar As FRRobot.FRCVar, Optional ByVal bList As Boolean = False)
        '********************************************************************************************
        'Description:  show details
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            subClearVarDetails()
            If oVar IsNot Nothing Then
                lblProgName.Text = oVar.Program.Name.ToString
                lblVarName.Text = oVar.VarName.ToString
                lblVarType.Text = oVar.TypeName.ToString
                Select Case oVar.StorageClass
                    Case FRRobot.FREVarStorageClassConstants.frVarCMOS
                        lblStorage.Text = gpsRM.GetString("psCMOS")
                    Case FRRobot.FREVarStorageClassConstants.frVarDRAM
                        lblStorage.Text = gpsRM.GetString("psDRAM")
                    Case FRRobot.FREVarStorageClassConstants.frVarFastCMOS
                        lblStorage.Text = gpsRM.GetString("psFAST_CMOS")
                    Case Else
                        lblStorage.Text = oVar.StorageClass.ToString
                End Select
                Select Case oVar.AccessCode
                    Case FRRobot.FREVarAccessCodeConstants.frVarApplicationControlAccess
                        lblAccess.Text = gpsRM.GetString("psACCESS_APP")
                    Case FRRobot.FREVarAccessCodeConstants.frVarFieldProtectionAccess
                        lblAccess.Text = gpsRM.GetString("psACCESS_FP")
                    Case FRRobot.FREVarAccessCodeConstants.frVarMotionControlAccess
                        lblAccess.Text = gpsRM.GetString("psACCESS_MOT")
                    Case FRRobot.FREVarAccessCodeConstants.frVarNoAccess
                        lblAccess.Text = gpsRM.GetString("psACCESS_NO")
                    Case FRRobot.FREVarAccessCodeConstants.frVarReadAccess
                        lblAccess.Text = gpsRM.GetString("psACCESS_RD")
                    Case FRRobot.FREVarAccessCodeConstants.frVarReadWriteAccess
                        lblAccess.Text = gpsRM.GetString("psACCESS_RW")
                    Case Else
                        lblAccess.Text = oVar.AccessCode.ToString
                End Select
                If bList Then
                    txtDescription.Text = tVarSetup(mnVarIndex).sDesc
                Else
                    txtDescription.Text = String.Empty
                    txtDescription.Enabled = False
                    btnDescEdit.Enabled = False
                End If
                lblStrLenCap.Text = gpsRM.GetString("psSTR_LEN_CAP")
                Select Case oVar.TypeCode
                    Case FRRobot.FRETypeCodeConstants.frStringType
                        lblStrLen.Text = oVar.MaxStringLen.ToString
                        lblMin.Text = gpsRM.GetString("psNA")
                        lblMax.Text = gpsRM.GetString("psNA")
                    Case FRRobot.FRETypeCodeConstants.frByteType, FRRobot.FRETypeCodeConstants.frIntegerType, _
                         FRRobot.FRETypeCodeConstants.frRealType, FRRobot.FRETypeCodeConstants.frShortType
                        lblMin.Text = oVar.MinValue.ToString
                        lblMax.Text = oVar.MaxValue.ToString
                        lblStrLen.Text = gpsRM.GetString("psNA")
                    Case FRRobot.FRETypeCodeConstants.frBooleanType
                        lblMin.Text = gpsRM.GetString("psNA")
                        lblMax.Text = gpsRM.GetString("psNA")
                        lblStrLen.Text = gpsRM.GetString("psNA")
                    Case FRRobot.FRETypeCodeConstants.frConfigType, FRRobot.FRETypeCodeConstants.frExtTransform, _
                         FRRobot.FRETypeCodeConstants.frExtXyz456, FRRobot.FRETypeCodeConstants.frExtXyz456, _
                         FRRobot.FRETypeCodeConstants.frExtXyzAes, FRRobot.FRETypeCodeConstants.frExtXyzWpr, _
                         FRRobot.FRETypeCodeConstants.frJoint, FRRobot.FRETypeCodeConstants.frTransform, _
                         FRRobot.FRETypeCodeConstants.frVectorType, FRRobot.FRETypeCodeConstants.frXyz456, _
                         FRRobot.FRETypeCodeConstants.frXyzAes, FRRobot.FRETypeCodeConstants.frXyzWpr
                        'Positions
                        lblMin.Text = gpsRM.GetString("psNA")
                        lblMax.Text = gpsRM.GetString("psNA")
                        Dim oPos As FRRobot.FRCVarPosition = DirectCast(oVar.Value, FRRobot.FRCVarPosition)
                        lblStrLenCap.Text = gpsRM.GetString("psGROUP_CAP")
                        If oPos IsNot Nothing AndAlso oPos.IsInitialized Then
                            lblStrLen.Text = oPos.GroupNum.ToString
                        Else
                            lblStrLen.Text = gpsRM.GetString("psNA")
                        End If

                    Case FRRobot.FRETypeCodeConstants.frArrayType
                        lblMin.Text = gpsRM.GetString("psNA")
                        lblMax.Text = gpsRM.GetString("psNA")
                        lblStrLen.Text = gpsRM.GetString("psNA")
                    Case Else
                        Try
                            lblMin.Text = oVar.MinValue.ToString
                        Catch ex As Exception
                            lblMin.Text = gpsRM.GetString("psNA")
                        End Try
                        Try
                            lblMax.Text = oVar.MaxValue.ToString
                        Catch ex As Exception
                            lblMax.Text = gpsRM.GetString("psNA")
                        End Try
                        Try
                            lblStrLen.Text = oVar.MaxStringLen.ToString
                        Catch ex As Exception
                            lblStrLen.Text = gpsRM.GetString("psNA")
                        End Try
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Sub subDisplayVarDetails(ByRef oVars As FRRobot.FRCVars)
        '********************************************************************************************
        'Description:  show details
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            subClearVarDetails()
            If oVars IsNot Nothing Then
                lblProgName.Text = oVars.Program.Name.ToString
                lblVarName.Text = oVars.VarName.ToString
                lblVarType.Text = gpsRM.GetString("psNA")
                lblStorage.Text = gpsRM.GetString("psNA")
                lblAccess.Text = gpsRM.GetString("psNA")
                txtDescription.Text = String.Empty
                lblStrLenCap.Text = gpsRM.GetString("psCOUNT_CAP")
                lblStrLen.Text = oVars.Count.ToString()
                lblMin.Text = gpsRM.GetString("psNA")
                lblMax.Text = gpsRM.GetString("psNA")
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Sub subDisplayVarDetails(ByRef oProg As FRRobot.FRCProgram)
        '********************************************************************************************
        'Description:  show details
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            subClearVarDetails()
            If oProg IsNot Nothing Then
                lblProgName.Text = oProg.Name.ToString
                lblVarName.Text = gpsRM.GetString("psNA")
                lblVarType.Text = gpsRM.GetString("psNA")
                lblStorage.Text = gpsRM.GetString("psNA")
                lblAccess.Text = gpsRM.GetString("psNA")
                txtDescription.Text = String.Empty
                lblStrLenCap.Text = gpsRM.GetString("psCOUNT_CAP")
                lblStrLen.Text = oProg.Variables.Count.ToString()
                lblMin.Text = gpsRM.GetString("psNA")
                lblMax.Text = gpsRM.GetString("psNA")
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Sub subDisplayVarDetails(ByRef oVector As FRRobot.FRCVector)
        '********************************************************************************************
        'Description:  show details
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            subClearVarDetails()
            If oVector IsNot Nothing Then
                Dim oParent As Object = oVector.Parent
                Dim oVar As FRRobot.FRCVar = Nothing
                If TypeOf (oParent) Is FRRobot.FRCVar Then
                    oVar = DirectCast(oParent, FRRobot.FRCVar)
                ElseIf TypeOf (oParent) Is FRRobot.FRCTransform Then
                    Dim oTransform As FRRobot.FRCTransform = DirectCast(oParent, FRRobot.FRCTransform)
                    Dim oPos As FRRobot.FRCVarPosition = DirectCast(oTransform.Parent, FRRobot.FRCVarPosition)
                    oVar = DirectCast(oPos.Parent, FRRobot.FRCVar)
                End If
                subDisplayVarDetails(oVar)
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Sub subDisplayVarDetails(ByRef oJnt As FRRobot.FRCJoint)
        '********************************************************************************************
        'Description:  show details
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            subClearVarDetails()
            If oJnt IsNot Nothing Then
                Dim oVar As FRRobot.FRCVar = Nothing
                Dim oPos As FRRobot.FRCVarPosition = DirectCast(oJnt.Parent, FRRobot.FRCVarPosition)
                oVar = DirectCast(oPos.Parent, FRRobot.FRCVar)
                subDisplayVarDetails(oVar)
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Sub subDisplayVarDetails(ByRef oXyzPos As FRRobot.FRCXyzWpr)
        '********************************************************************************************
        'Description:  show details
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            subClearVarDetails()
            If oXyzPos IsNot Nothing Then
                Dim oVar As FRRobot.FRCVar = Nothing
                Dim oPos As FRRobot.FRCVarPosition = DirectCast(oXyzPos.Parent, FRRobot.FRCVarPosition)
                oVar = DirectCast(oPos.Parent, FRRobot.FRCVar)
                subDisplayVarDetails(oVar)
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Sub subDisplayVarDetails(ByRef oTrn As FRRobot.FRCTransform)
        '********************************************************************************************
        'Description:  show details
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            subClearVarDetails()
            If oTrn IsNot Nothing Then
                Dim oVar As FRRobot.FRCVar = Nothing
                Dim oPos As FRRobot.FRCVarPosition = DirectCast(oTrn.Parent, FRRobot.FRCVarPosition)
                oVar = DirectCast(oPos.Parent, FRRobot.FRCVar)
                subDisplayVarDetails(oVar)
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Sub subDisplayVarDetails(ByRef oConfig As FRRobot.FRCConfig)
        '********************************************************************************************
        'Description:  show details
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            subClearVarDetails()
            If oConfig IsNot Nothing Then
                Dim oParent As Object = oConfig.Parent
                Dim oVar As FRRobot.FRCVar = Nothing
                If TypeOf (oParent) Is FRRobot.FRCVar Then
                    oVar = DirectCast(oParent, FRRobot.FRCVar)
                ElseIf TypeOf (oParent) Is FRRobot.FRCTransform Then
                    Dim oTransform As FRRobot.FRCTransform = DirectCast(oParent, FRRobot.FRCTransform)
                    Dim oPos As FRRobot.FRCVarPosition = DirectCast(oTransform.Parent, FRRobot.FRCVarPosition)
                    oVar = DirectCast(oPos.Parent, FRRobot.FRCVar)
                ElseIf TypeOf (oParent) Is FRRobot.FRCXyzWpr Then
                    Dim oXyzPos As FRRobot.FRCXyzWpr = DirectCast(oParent, FRRobot.FRCXyzWpr)
                    Dim oPos As FRRobot.FRCVarPosition = DirectCast(oXyzPos.Parent, FRRobot.FRCVarPosition)
                    oVar = DirectCast(oPos.Parent, FRRobot.FRCVar)
                ElseIf TypeOf (oParent) Is FRRobot.FRCJoint Then
                    Dim oJnt As FRRobot.FRCXyzWpr = DirectCast(oParent, FRRobot.FRCXyzWpr)
                    Dim oPos As FRRobot.FRCVarPosition = DirectCast(oJnt.Parent, FRRobot.FRCVarPosition)
                    oVar = DirectCast(oPos.Parent, FRRobot.FRCVar)
                End If
                subDisplayVarDetails(oVar)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub lstVars_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstVars.Click
        mnVARINDEXSELECTED = lstVars.SelectedIndex
    End Sub
    '                    ElseIf TypeOf (oTmp) Is FRRobot.FRCConfig Then
    'Dim oConfig As FRRobot.FRCConfig = DirectCast(oTmp, FRRobot.FRCConfig)
    '                        subDisplayVarDetails(oConfig)
    Private Sub lstVars_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstVars.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Variable selected in list
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If lstVars.SelectedItem IsNot Nothing Then
                Dim sName As String = lstVars.SelectedItem.ToString
                For nItem As Integer = 0 To tVarSetup.GetUpperBound(0)
                    If tVarSetup(nItem).sName = sName Then
                        mnVarIndex = nItem
                        Exit For
                    End If
                Next
                If mnVarIndex >= 0 Then
                    Dim bSaveable As Boolean = False
                    Dim oTmp As Object = oValidVariable(mController, tVarSetup(mnVarIndex).sName, tVarSetup(mnVarIndex).sFile, tVarSetup(mnVarIndex).sVar, bSaveable, False, False)
                    Dim oVar As FRRobot.FRCVar = Nothing
                    If oTmp IsNot Nothing Then
                        If TypeOf (oTmp) Is FRRobot.FRCVar Then
                            oVar = DirectCast(oTmp, FRRobot.FRCVar)
                            'subDisplayVarDetails(oVar, False)
                            subDisplayVarDetails(oVar, True)    'JZ 12072016
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCJoint Then
                            Dim oJnt As FRRobot.FRCJoint = DirectCast(oTmp, FRRobot.FRCJoint)
                            subDisplayVarDetails(oJnt)
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCTransform Then
                            Dim oPos As FRRobot.FRCTransform = DirectCast(oTmp, FRRobot.FRCTransform)
                            subDisplayVarDetails(oPos)
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCXyzWpr Then
                            Dim oPos As FRRobot.FRCXyzWpr = DirectCast(oTmp, FRRobot.FRCXyzWpr)
                            subDisplayVarDetails(oPos)
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCConfig Then
                            Dim newNode As New TreeNode("FLIP")
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCVector Then
                            Dim oVector As FRRobot.FRCVector = DirectCast(oTmp, FRRobot.FRCVector)
                            subDisplayVarDetails(oVector)
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCAxesCollection Then
                            Dim oAxesCol As FRRobot.FRCAxesCollection = DirectCast(oTmp, FRRobot.FRCAxesCollection)
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCVars Then
                            Dim oVars As FRRobot.FRCVars = DirectCast(oTmp, FRRobot.FRCVars)
                            subDisplayVarDetails(oVars)
                            'A structure is selected, expand the treeview
                        ElseIf TypeOf (oTmp) Is FRRobot.FRCProgram Then
                            Dim oProg As FRRobot.FRCProgram = DirectCast(oTmp, FRRobot.FRCProgram)
                            'A program is selected, expand the treeview
                            subDisplayVarDetails(oProg)
                        End If
                    End If
                    oVar = oReadFromRobots(tVarSetup(mnVarIndex))
                End If
            End If
        Catch ex As Exception

        End Try

        subEnableControls(True)

    End Sub
    Private Sub lblRCVar_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        '********************************************************************************************
        'Description:  Mousedown before menu popup
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oLbl As Label = DirectCast(sender, Label)
        If oLbl IsNot Nothing Then
            Dim sTmp As String = oLbl.Name.Substring(8)
            mnPopUpMenuRobotNumber = CInt(sTmp)
        End If
    End Sub

    Private Sub chkRC_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        '********************************************************************************************
        'Description:  Mousedown before menu popup
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oChk As CheckBox = DirectCast(sender, CheckBox)
        If oChk IsNot Nothing Then
            Dim sTmp As String = oChk.Name.Substring(6)
            mnPopUpMenuRobotNumber = CInt(sTmp)
        End If

    End Sub

    Private Sub btnWriteRC_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWriteRC.Click
        '********************************************************************************************
        'Description:  Write variable to selected robots
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subWriteToRobots()
    End Sub
    Private Sub subWriteToRobots()
        '********************************************************************************************
        'Description:  Write variable to selected robots
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason

        ' 06/07/12  MSW     subWriteToRobots - Add waitcursor during write 
        ' 09/19/12  MSW     subWriteToRobots - Reset the cursor before  all the exit subs
        '********************************************************************************************
        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            Dim oCulture As System.Globalization.CultureInfo = DisplayCulture
            Dim sValue As String = txtVarValue.Text
            Dim nValue As Integer = 0
            Dim rValue As Double = 0 'This is the max range for "isnumeric"
            Dim bConfirmEmpty As Boolean = False
            If (mnVarIndex >= 0) Then 'a variable is selected
                For nRobot As Integer = 1 To colControllers.Count
                    Try
                        Dim chkRCName As CheckBox = DirectCast(pnlMain.Controls("chkRC_" & nRobot.ToString), CheckBox)
                        If chkRCName.Checked Then
                            'Get the variable
                            Dim bSaveable As Boolean = False
                            Dim oTmp As Object = oValidVariable(colControllers(nRobot - 1), tVarSetup(mnVarIndex).sName, tVarSetup(mnVarIndex).sFile, tVarSetup(mnVarIndex).sVar, bSaveable, False, True)
                            If TypeOf (oTmp) Is FRRobot.FRCVar Then
                                Dim oVar As FRRobot.FRCVar = DirectCast(oTmp, FRRobot.FRCVar)
                                If oVar IsNot Nothing Then
                                    Dim sOldVal As String = String.Empty
                                    'MSW 12/06/13 - fix write for uninit vars
                                    If oVar.IsInitialized Then
                                        sOldVal = oVar.Value.ToString
                                    Else
                                        sOldVal = gpsRM.GetString("psUNINITIALIZED", DisplayCulture)
                                    End If
                                    Dim sDesc As String = String.Empty
                                    If oVar.Program.Name <> String.Empty Then
                                        sDesc = gpsRM.GetString("psVARIABLE") & " [" & oVar.Program.Name & "]" & oVar.VarName
                                    Else
                                        sDesc = gpsRM.GetString("psVARIABLE") & " " & oVar.VarName
                                    End If
                                    'Do some basic validation of the variable first.
                                    Select Case oVar.AccessCode
                                        Case FRRobot.FREVarAccessCodeConstants.frVarReadWriteAccess
                                            Select Case oVar.TypeCode
                                                Case FRRobot.FRETypeCodeConstants.frBooleanType
                                                    Try

                                                        If IsNumeric(sValue) Then
                                                            nValue = CInt(sValue)
                                                            'It passed the data check, so the robot should except the range
                                                            If nValue > 0 Then
                                                                oVar.Value = True
                                                            Else
                                                                oVar.Value = False
                                                            End If
                                                        Else
                                                            oVar.Value = CBool(sValue)
                                                        End If
                                                    Catch ex As Exception
                                                        MessageBox.Show(gpsRM.GetString("psERROR", oCulture), _
                                                                gpsRM.GetString("psERR_WRT_VAR_TO", oCulture) & colControllers(nRobot - 1).Name & ".", _
                                                                MessageBoxButtons.OK, _
                                                                MessageBoxIcon.Error)
                                                    End Try

                                                Case FRRobot.FRETypeCodeConstants.frStringType
                                                    Try
                                                        If (sValue = String.Empty) And (bConfirmEmpty = False) Then
                                                            'Message box, ask if they want to make an empty string
                                                            Dim lRet As Response = MessageBox.Show(colControllers(nRobot - 1).Name & " - " & _
                                                                gpsRM.GetString("psSET_TO_EMPTY", oCulture), _
                                                                gpsRM.GetString("psSETEMPTY", oCulture), _
                                                                MessageBoxButtons.YesNo, _
                                                                MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                                                            Select Case lRet
                                                                Case Windows.Forms.DialogResult.Yes
                                                                    bConfirmEmpty = True
                                                                Case Else
                                                                    Me.Cursor = System.Windows.Forms.Cursors.Default
                                                                    Exit Sub 'get out
                                                            End Select
                                                        End If
                                                        If sValue.Length > oVar.MaxStringLen Then
                                                            Dim lRet As Response = MessageBox.Show(colControllers(nRobot - 1).Name & " - " & _
                                                                gpsRM.GetString("psSTR_GRT_MAX", oCulture) & _
                                                                vbCrLf & gpsRM.GetString("psCONT_NEXT_RC"), _
                                                                gpsRM.GetString("psCONTINUE", oCulture), _
                                                                MessageBoxButtons.YesNo, _
                                                                MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                                                            Select Case lRet
                                                                Case Windows.Forms.DialogResult.Yes
                                                                    'Do nothing, come back for the next robot
                                                                Case Else
                                                                    Me.Cursor = System.Windows.Forms.Cursors.Default
                                                                    Exit Sub 'get out
                                                            End Select
                                                        Else
                                                            oVar.Value = sValue
                                                        End If
                                                    Catch ex As Exception
                                                        MessageBox.Show(gpsRM.GetString("psERROR", oCulture), _
                                                                gpsRM.GetString("psERR_WRT_VAR_TO", oCulture) & colControllers(nRobot - 1).Name & ".", _
                                                                MessageBoxButtons.OK, _
                                                                MessageBoxIcon.Error)
                                                        Me.Cursor = System.Windows.Forms.Cursors.Default
                                                        Exit Sub
                                                    End Try
                                                Case FRRobot.FRETypeCodeConstants.frByteType, FRRobot.FRETypeCodeConstants.frIntegerType, _
                                                     FRRobot.FRETypeCodeConstants.frShortType, FRRobot.FRETypeCodeConstants.frRealType
                                                    Try
                                                        If IsNumeric(sValue) Then
                                                            rValue = CDbl(sValue)
                                                            If (rValue > CDbl(oVar.MaxValue)) Or (rValue < CDbl(oVar.MinValue)) Then
                                                                Dim lRet As Response = MessageBox.Show(colControllers(nRobot - 1).Name & " - " & _
                                                                        gpsRM.GetString("psVAL_OUT_RANGE", oCulture) & _
                                                                        vbCrLf & gpsRM.GetString("psCONT_NEXT_RC"), _
                                                                        gpsRM.GetString("psCONTINUE", oCulture), _
                                                                        MessageBoxButtons.YesNo, _
                                                                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                                                                Select Case lRet
                                                                    Case Windows.Forms.DialogResult.Yes
                                                                        'Do nothing, come back for the next robot
                                                                    Case Else
                                                                        Me.Cursor = System.Windows.Forms.Cursors.Default
                                                                        Exit Sub 'get out
                                                                End Select
                                                            Else
                                                                'It passed the data check, so the robot should except the range
                                                                If oVar.TypeCode = FRRobot.FRETypeCodeConstants.frRealType Then
                                                                    oVar.Value = rValue
                                                                Else
                                                                    Debug.Print(oVar.Program.Name)
                                                                    Debug.Print(oVar.VarName)
                                                                    nValue = CInt(rValue)
                                                                    oVar.Value = nValue
                                                                End If

                                                            End If
                                                        Else
                                                            MessageBox.Show(gpsRM.GetString("psNUMERIC_REQ", oCulture), _
                                                                    gpsRM.GetString("psINV_DATA", oCulture), _
                                                                    MessageBoxButtons.OK, _
                                                                    MessageBoxIcon.Error)
                                                            Me.Cursor = System.Windows.Forms.Cursors.Default
                                                            Exit Sub
                                                        End If

                                                    Catch ex As Exception
                                                        MessageBox.Show(gpsRM.GetString("psERROR", oCulture), _
                                                                gpsRM.GetString("psERR_WRT_VAR_TO", oCulture) & colControllers(nRobot - 1).Name & ".", _
                                                                MessageBoxButtons.OK, _
                                                                MessageBoxIcon.Error)
                                                    End Try
                                                Case Else
                                            End Select
                                            subUpdateChangeLog(oVar.Value.ToString, sOldVal, mcolZones.ActiveZone, _
                                                                colControllers(nRobot - 1).Name, sDesc)

                                        Case Else
                                            'No write access
                                            MessageBox.Show(gpsRM.GetString("psERROR", oCulture), _
                                                gpsRM.GetString("psNO_WRT_ACC", oCulture) & colControllers(nRobot - 1).Name & ".", _
                                                MessageBoxButtons.OK, _
                                                MessageBoxIcon.Error)
                                    End Select

                                Else
                                    MessageBox.Show(gpsRM.GetString("psERR_WRT_VAR_TO", oCulture) & colControllers(nRobot - 1).Name & ".", _
                                                    gpsRM.GetString("psERROR", oCulture), _
                                                    MessageBoxButtons.OK, _
                                            MessageBoxIcon.Error)
                                End If
                                'Read again after the write for confirmation.
                                oReadFromRobot(tVarSetup(mnVarIndex), nRobot)
                            ElseIf (TypeOf (oTmp) Is FRRobot.FRCPosition) OrElse _
                                (TypeOf (oTmp) Is FRRobot.FRCJoint) OrElse _
                                (TypeOf (oTmp) Is FRRobot.FRCTransform) OrElse _
                                (TypeOf (oTmp) Is FRRobot.FRCVector) OrElse _
                                (TypeOf (oTmp) Is FRRobot.FRCConfig) Then
                                MessageBox.Show(gpsRM.GetString("psCANNOT_WRITE_POS_DAT", oCulture), _
                                        gpsRM.GetString("psERROR", oCulture), _
                                        MessageBoxButtons.OK, _
                                        MessageBoxIcon.Error)
                            ElseIf oTmp IsNot Nothing Then
                                MessageBox.Show(gpsRM.GetString("psCANNOT_WRITE_VAR", oCulture) & tVarSetup(mnVarIndex).sName & ".", _
                                        gpsRM.GetString("psERROR", oCulture), _
                                        MessageBoxButtons.OK, _
                                        MessageBoxIcon.Error)

                            End If
                        End If
                    Catch ex As Exception
                        MessageBox.Show(gpsRM.GetString("psERR_WRT_VAR", oCulture) & colControllers(nRobot - 1).Name & ".", _
                                         gpsRM.GetString("psERROR", oCulture), _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Error)
                    End Try
                Next

            End If
            subLogChanges()
        Catch ex As Exception

        End Try
        Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub
    Private Sub btnDescEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDescEdit.Click
        '********************************************************************************************
        'Description:  edit description
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Select Case txtDescription.ReadOnly
            Case True
                'Save current settings
                txtDescription.ReadOnly = False
                txtDescription.Enabled = True   'JZ 12072016 - Enable edit.
                btnDescEdit.Text = gpsRM.GetString("psACCEPT")
            Case False
                'allow edit
                txtDescription.ReadOnly = True
                txtDescription.Enabled = False   'JZ 12072016 - Disable edit.
                btnDescEdit.Text = gpsRM.GetString("psEDIT")
                If mnVarIndex >= 0 Then
                    tVarSetup(mnVarIndex).sDesc = txtDescription.Text
                    EditsMade = True
                End If
        End Select
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        '********************************************************************************************
        'Description:  Add selected item to list
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        bVarAdd(txtVarName.Text)
    End Sub
    Private Sub chkRC_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  robot checkbox changed.  Stop autoupdate
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbEventBlocker = False Then
            mbAutoSetRobotCheckBoxes = False
            Dim chkRCName As CheckBox = DirectCast(sender, CheckBox)
            Dim nRobot As Integer = CInt(chkRCName.Name.Substring(6))
            If mnVarIndex >= 0 Then
                oReadFromRobot(tVarSetup(mnVarIndex), nRobot)
            Else
                Dim lblRCVar As Label = DirectCast(pnlMain.Controls("lblRCVar" & nRobot.ToString), Label)
                lblRCVar.Text = String.Empty
            End If
        End If
    End Sub
    Private Sub mnuSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSelectAll.Click
        '********************************************************************************************
        'Description:  Select All robot checkboxes pop-up menu.
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'set the robot checkbox
        mbEventBlocker = True
        For nRobot As Integer = 1 To colControllers.Count
            Dim chkRCName As CheckBox = DirectCast(pnlMain.Controls("chkRC_" & nRobot.ToString), CheckBox)
            chkRCName.Checked = True
        Next
        Application.DoEvents()
        If mnVarIndex >= 0 Then
            Dim oVar As FRRobot.FRCVar = oReadFromRobots(tVarSetup(mnVarIndex))
            subDisplayVarDetails(oVar, True)
        End If

        mbEventBlocker = False
    End Sub

    Private Sub mnuUnselectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuUnselectAll.Click
        '********************************************************************************************
        'Description:  Unselect All robot checkboxes pop-up menu.
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'set the robot checkbox
        mbEventBlocker = True
        For nRobot As Integer = 1 To colControllers.Count
            Dim chkRCName As CheckBox = DirectCast(pnlMain.Controls("chkRC_" & nRobot.ToString), CheckBox)
            chkRCName.Checked = False
            Dim lblRCVar As Label = DirectCast(pnlMain.Controls("lblRCVar" & nRobot.ToString), Label)
            lblRCVar.Text = String.Empty
        Next
        mbEventBlocker = False
    End Sub

    Private Sub mnuBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuBrowse.Click
        '********************************************************************************************
        'Description:  Select the robot assigned to this checkbox and switch to browse mode
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If cboRobot.SelectedIndex <> (mnPopUpMenuRobotNumber - 1) Then
                cboRobot.SelectedIndex = mnPopUpMenuRobotNumber - 1
            End If
            rdoBrowse.Checked = True
        Catch ex As Exception

        End Try
    End Sub

    Private Sub mnuCopy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCopy.Click
        '********************************************************************************************
        'Description:  Copy the value of this variable to the clipboard
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim lblRCVar As Label = DirectCast(pnlMain.Controls("lblRCVar" & mnPopUpMenuRobotNumber.ToString), Label)
        Clipboard.SetDataObject(lblRCVar.Text)
        txtVarValue.Text = lblRCVar.Text
    End Sub
    Private Sub mnuCopyToAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCopyToAll.Click
        '********************************************************************************************
        'Description:  Copy the value of this variable to all robots
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim lblRCVar As Label = DirectCast(pnlMain.Controls("lblRCVar" & mnPopUpMenuRobotNumber.ToString), Label)
        txtVarValue.Text = lblRCVar.Text
        subWriteToRobots()
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
                    subLaunchHelp(gs_HELP_UTILITIES_ROBOTVARIABLES, oIPC)

                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty

                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    If rdoBrowse.Checked Then
                        oSC.CaptureWindowToFile(Me.Handle, sDumpPath & msSCREEN_DUMP_NAME_BROWSE, Imaging.ImageFormat.Jpeg)
                    Else
                        oSC.CaptureWindowToFile(Me.Handle, sDumpPath & msSCREEN_DUMP_NAME, Imaging.ImageFormat.Jpeg)
                    End If

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

    Private Sub btnRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemove.Click
        '********************************************************************************************
        'Description:  Someone clicked the Remove button 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************        lstVars.SelectedItems.Remove(lstVars.Items(mnVARINDEXSELECTED))
        Dim i As Integer
        Dim temptvar(tVarSetup.GetUpperBound(0)) As tVarSetupType
        Dim bremoved As Boolean = False
        'store all the variables we are keeping in a temporary array
        ReDim temptvar(tVarSetup.GetUpperBound(0))

        For i = 0 To tVarSetup.GetUpperBound(0)
            'keep all the variables' data except the one we're deleting
            If tVarSetup(i).sName <> lstVars.Items(mnVARINDEXSELECTED).ToString Then
                temptvar(i) = tVarSetup(i)
            Else
                bremoved = True
            End If
        Next

        If bremoved = True Then
            'if there was something or some things deleted, resize tvarsetup and copy temptvar back into it
            'If temptvar.GetUpperBound(0) <> tVarSetup.GetUpperBound(0) Then
            'ReDim tVarSetup(tVarSetup.GetUpperBound(0) - 1)
            For i = 0 To temptvar.GetUpperBound(0)
                tVarSetup(i) = temptvar(i)
            Next
            'End If
            subShowNewPage(True, True, False)
            EditsMade = True
        End If

    End Sub

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
                subSaveData()

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