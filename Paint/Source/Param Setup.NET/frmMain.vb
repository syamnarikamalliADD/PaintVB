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
' Description: Parameter setup screen
' 
'
' Dependencies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: White
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                    Version
' 10/20/2009    MSW     Start the param setup screen                                  
' 11/16/2009    msw     Add multiview help files
' 11/18/09      MSW     mdgvData_KeyPress - Offer login if needed
' 11/18/09      MSW     subInitializeForm - set screen name in title bar
' 04/08/2010    RJO     Changes to subSaveData to elimate annoying data save      4.00.00.01
'                       problems. Also changed screenshot image type in 
'                       frmMain_KeyDown to match other apps.
' 04/21/10  	MSW     subGetPLCValue - handle no data situation
' 03/17/10      BTK     Add tags to project strings for Accuflow Parameters.
'                       subSetPLCValue - Added code to process comboboxes.
' 08/23/10      MSW     bLoadData - Fix floating point formatting so 0 doesn't show up blank
' 08/26/11      MSW     make a couple little user interface improvements for multiview
'                       when changing tabs showing all robots or controllers, update to controllers or robots as needed.
'                       shortcut to load all robots or controllers by right-clicking on multiview or add buttons
'    09/14/11   MSW     Assemble a standard version of everything                       4.1.0.0
'    09/28/11   MSW     Copy some resource strings from the standalone versions         4.1.0.1
'    09/29/11   RJO     Added support for RegNumerics and RegStrings.              4.00.00.02 
'    11/08/11   MSW     add some handling for a shutdown in the middle of a load (mbAbortForShutdown)
'    11/08/11   MSW     prevent recursion into subchangetab                        4.01.01.00
'    12/14/11   RJO     Added support for Boolean values with custom combos        4.01.01.01
'    02/15/12   MSW     Import/Export updates, Force 32 bit build for PCDK         4.01.01.02
'    03/21/12   RJO     modified for .NET Password and IPC                         4.01.02.00
'    03/28/12   MSW     Add XML data support                                       4.01.03.00
'    04/27/12   MSW     Fix changelog area for new password                        4.01.03.01
'    06/07/12   MSW     Move setup tables from SQL to XML for quicker startup      4.01.04.00
'    10/01/12   BTK     Changed bGetScreenCfg to use semicolon to parse out        4.01.04.01
'                       customCbo data in the xml file.
'    10/24/12   RJO     Added StartupModule to project to prevent multiple instances 4.01.04.02
'    11/15/12   HGB     Added code to subChangeZone to force a reload of the robot 4.01.04.03
'                       combo. Added bForceReload param to subChangeTab to force a 
'                       reload of the robot combo on a zone change.
'    01/16/13   MSW     Add strings for honda robot setup pages                     4.01.04.04
'    02/17/12   MSW     Add PaintWorksBackups separate from RobotBackups            4.01.04.05
'    04/16/13   MSW     Add Canadian language files                                 4.01.05.00
'                       Standalone Changelog
'    07/09/13   MSW     Update and standardize logos, PLCComm.Dll                   4.01.05.01
'                       Add status screen features to paramsetup,
'                       allow configurable screen names for changelog, password
'    07/19/13   MSW     More work on the status screen features                     4.01.05.02
'    07/25/13   MSW     More work on the status screen features, sealer DBs         4.01.05.03
'    08/20/13   MSW     Allow tabs to be disabled without removing them from the DB 4.01.05.04
'                       PSC remote PC support
'    09/20/13   DE      Modified Function sConvertTags to handle Cultures  that use 4.01.05.05
'                       "," in floating point numbers rather than ".".
'                       Progress, Status - Add error handler so it doesn't get hung up
'    09/30/13   MSW     Save screenshots as jpegs, PLC DLL                          4.01.06.00
'    10/09/13   MSW     subSetRobotValue - Status screen changes broke 2var         4.01.06.01
'                       Write, fix it
'    10/11/13   MSW     Make sure REALs show 0s on the right side of the decimal    4.01.06.02
'                       point if they're set up that way.
'    10/16/13   MSW     More status updates, fixes for other status updates         4.01.06.03
'    10/28/13   BTK     mnuImport_Click changed code so we don't always test for    4.01.06.04
'                       robot variable.
'               RJO     subInitializeForm - Changed to handle Param Setup screen
'                       special case.
'    12/03/13   MSW     Support skipping robot numbers                              4.01.06.05
'                       Also, handle some akward exits
'    02/03/14   RJO     Fix in subSaveData to prevent atempts to save "N/A" data.   4.01.06.06
'                       Fix in mnuImport_Click to prevent errors when attempting to
'                       import "N/A" data.
'    01/14/14   DJM     Changed psUOPSEnabled and "UOPs Enabled" to                 4.01.07.00
'                       psUOPSDisabled and "UOPs Disabled" in string resource file.
'                       Made corresponding changes in 
'                       ParamSetupRobotStatusSealer.XML
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call            4.01.07.00
'    03/22/14   MAD     Added a fix to bLoadData to make sure the context menus     4.01.07.01
'                       show up on every tab.
'    02/26/14   MSW     support robot IO (read only), add cartoons for dispense     4.01.07.01
'********************************************************************************************

Option Compare Text
Option Explicit On
Option Strict On

Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Configuration.ConfigurationSettings
Imports Response = System.Windows.Forms.DialogResult
Imports ConnStat = FRRobotNeighborhood.FRERNConnectionStatusConstants
Imports System.Xml
Imports System.Xml.XPath
Imports clsPLCComm = PLCComm.clsPLCComm


Friend Class frmMain
#Region " Declares "

    '******** Form Constants   **********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Friend msSCREEN_NAME As String = "ParamSetup"   ' <-- For password area change log etc.
    Friend msSCREEN_DUMP_NAME As String = "ParamSetup"
    Friend Const msSCREEN_DUMP_EXT As String = ".jpg"
    Friend Const msSCREEN_NAME_C As String = "ParamSetup"
    Private Const mnMAININX As Integer = 100
    Private Const mnSCREENINX As Integer = 101
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME_C & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME_C & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME_C & ".RobotStrings"
    Friend mbUSEROBOTS As Boolean = True
    '******** End Form Constants    *****************************************************************
    Friend Structure tColParm
        Dim sColor As String
        Dim nColor As Integer
        Dim nValve As Integer
        Dim sCustom As String
        Dim sZone As String
        Dim oZone As clsZone
        Dim sDevice As String
        Dim oArm As clsArm
    End Structure

    '******** Form Variables   **********************************************************************
    Friend moColParam As tColParm()
    Private msOldZone As String = String.Empty
    Private msOldRobot As String = String.Empty
    Private msOldController As String = String.Empty
    Private msOldColor As String = String.Empty
    Private mnColorIndex As Integer = 0
    Private msOldParm As String = String.Empty
    Private msParmTag As String = String.Empty
    Private mnOldTab As Integer = 0
    Private colZones As clsZones = Nothing
    Private WithEvents colControllers As clsControllers = Nothing
    Private WithEvents colArms As clsArms = Nothing
    Private mbSAEnableArm() As Boolean
    Private msSAEnableString As String = String.Empty
    Private mbOpenersExist As Boolean = False
    Private mRobot As clsArm = Nothing
    Private mbEventBlocker As Boolean = False
    Private mbRemoteZone As Boolean = False
    Private mbScreenLoaded As Boolean = False
    Private mbAbortForShutdown As Boolean = False
    Private mbDontChangeTabs As Boolean = False
    Private mbRobotNotAvailable() As Boolean = Nothing
    'Applicator config
    Private mApplicator As clsApplicator
    Private mPrintHtml As clsPrintHtml
    Public moGraph As tGraphConfig = Nothing

    Private moToonControl As Control = Nothing
    Private WithEvents moToonInterface As PrmSetToon = Nothing

    Friend Enum eDataSource As Integer
        none
        DB
        Robot
        PLC
        Registry
        XML
    End Enum
    Friend Enum eFormula As Integer
        None = 0   '
        Scale = 0
        'None implemented yet, but I thought it would be good to have the basics in here
    End Enum
    Friend Structure tItemCfg
        Dim Label As String
        Dim Tag As String
        Dim Tab As String
        Dim DataSource As eDataSource
        Dim DataType As Type
        Dim PLCTag As String
        Dim ProgName() As String
        Dim VarName() As String
        Dim DBName As String
        Dim DBTableName As String
        Dim DBRowName As String
        Dim DBColumnName As String
        Dim RegPath As String
        Dim RegKey As String
        Dim bEdit As Boolean
        Dim Min As Double
        Dim Max As Double
        Dim Scale As Double
        Dim Precision As Integer
        Dim Bitmask As Integer
        Dim bCbo As Boolean
        Dim CustomCBO() As String
        Dim CustomColors() As String
        Dim ColorRange() As String
        Dim formula As eFormula
        Dim mnuPopUp As ContextMenuStrip
        Dim bPainterOnly As Boolean
        Dim ArrayLength As Integer
    End Structure
    Friend Structure tGraphConfig
        Dim sGraphName As String
        Dim ItemNames As String()
        Dim ItemUnits As String()
        Dim ItemNumbers As Integer()
        Dim ItemMin As Integer()
        Dim ItemMax As Integer()
        Dim ItemCfg As tItemCfg()
        Dim ItemObj As Object()
    End Structure
    Friend Structure tTabCfg
        Dim TabName As String
        Dim TabTag As String
        Dim bUseZone As Boolean
        Dim bUseController As Boolean
        Dim bUseArm As Boolean
        Dim bShowOpeners As Boolean
        Dim bUseColor As Boolean
        Dim bUseValve As Boolean
        Dim bUseCustomParm As Boolean
        Dim sCustomParm As String
        Dim sHelpFile As String
        Dim nAutoRefresh As Integer
        Dim bNoEdit As Boolean
        Dim bScatteredAccess As Boolean
        Dim ItemCfg As tItemCfg()
        Dim Hide As Boolean
        Dim oGraphs As tGraphConfig()
        Dim sToon As String
    End Structure
    Friend mtTabCfgs() As tTabCfg
    Friend mCombinedTabCfg As tTabCfg
    Friend mnTab As Integer = -1
    Friend WithEvents mdgvData As DataGridView
    Private msOldEditVal As String = String.Empty
    Friend Const mnZONE_ROW As Integer = 0
    Friend Const mnCONTROLLER_ROW As Integer = 1
    Friend Const mnROBOT_ROW As Integer = 2
    Friend Const mnCOLOR_ROW As Integer = 3
    Friend Const mnVALVE_ROW As Integer = 4
    Friend Const mnCUSTOMPARM_ROW As Integer = 5
    Friend Const mnITEM0_ROW As Integer = 6

    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean = False
    Private mbEditsMade As Boolean = False
    Private mnProgress As Integer = 0
    Private mnPopupMenuStep As Integer = 0
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    Private msCulture As String = "en-US" 'Default to english
    Private msScreen As String = String.Empty
    Private msScreenName As String = String.Empty
    Private msTableName As String = String.Empty
    Private msPwdName As String = String.Empty
    Private msFormIcon As String = "FormIcon"
    Private dtScreenTable As DataTable
    Private mColDSData As Collection(Of DataSet)
    Private Const msMNUNAME1 As String = "mnuTab"
    Private Const msMNUNAME2 As String = "Item"
    Private Const msMNUNAME3 As String = "Sel"
    Private mnDataGridSelCol As Integer = 0
    Private mnDataGridSelRow As Integer = 0
    Private mnMouseDownX As Integer = 0
    Private mnMouseDownY As Integer = 0
    Private mnRefreshRotation As Integer = 0
    Private WithEvents mPLC As clsPLCComm = Nothing
    Private mbSimpleViewAll As Boolean = False
    '******** End Property Variables    *************************************************************

    '******** This is the old pw3 password object interop  *****************************************
    'Friend WithEvents moPassword As PWPassword.PasswordObject 'RJO 03/21/12
    'Friend moPrivilege As PWPassword.cPrivilegeObject 'RJO 03/21/12
    Private msColorCache As String = String.Empty
    '******** This is the old pw3 password object interop  *****************************************

    Friend WithEvents moPassword As clsPWUser
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm

    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    Delegate Sub ControllerStatusChange(ByVal Controller As clsController)
    Delegate Sub NewMessage_Callback(ByVal Schema As String, ByVal DS As DataSet)

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


            'If mPassword.CheckPassword(msSCREEN_NAME, Value, moPassword, moPrivilege) Then 'RJO 03/21/12
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
        '********************************************************************************************
        Get
            Return mbDataLoaded
        End Get
        Set(ByVal Value As Boolean)
            mbDataLoaded = Value
            'If mbDataLoaded Then
            '    'just finished loading reset & refresh
            '    subShowNewPage()
            'End If
            'subEnableControls(True)
        End Set
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
            mbEditsMade = Value
            subEnableControls(True)
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

                If mbAbortForShutdown Then
                    Exit Property
                End If
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
        If (mdgvData IsNot Nothing) Then
            mdgvData.CancelEdit()
        End If

        If EditsMade Then
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
        Else
            Return True
        End If
    End Function

    Private Sub subChangeTab(Optional ByVal bForceReload As Boolean = False)
        '********************************************************************************************
        'Description: New Tab Selected - check for save then load new info
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/15/12  HGB     Added bForceReload to force a reload on a zone change
        '********************************************************************************************
        If mbDontChangeTabs Then
            Exit Sub
        End If
        Progress = 10
        ' false means user pressed cancel
        If bAskForSave() = False Then
            cboZone.Text = msOldZone
            Progress = 100
            Exit Sub
        End If
        mbDontChangeTabs = True
        Dim bSetAutoRefresh As Boolean = False
        Try

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            bSetAutoRefresh = mnuAutoRefresh.Checked
            mnuAutoRefresh.Checked = False
            btnRefresh.Image = DirectCast(gpsRM.GetObject("Refresh", mLanguage.FixedCulture), Image)
            tmrRefresh.Enabled = False

            If mnTab >= 0 AndAlso mtTabCfgs(mnTab).bScatteredAccess Then
                subClearSARequests()
            End If
            subEnableControls(False)
            msOldParm = String.Empty '12/30/09  RJO
            Dim nNewTab As Integer
            Dim nOldTab As Integer = mnTab
            'Find the tab in the array.  
            For nIndex As Integer = 0 To mtTabCfgs.GetUpperBound(0)
                If tabMain.SelectedTab.Text = mtTabCfgs(nIndex).TabName Then
                    nNewTab = nIndex
                    Exit For
                End If
            Next
            Dim dgvLast As DataGridView = Nothing
            Dim bReload As Boolean = False
            If DataLoaded Then
                If mdgvData IsNot Nothing Then
                    dgvLast = mdgvData
                    bReload = dgvLast.ColumnCount > 0
                End If
            End If
            'Setup cbos
            mbEventBlocker = True
            'Set current tab here, before these other events can fire
            mnTab = nNewTab
            'do valve/color labels.  subChangeRobot will load the cbo
            If (nOldTab = -1) OrElse _
                    (mtTabCfgs(nNewTab).bUseValve <> mtTabCfgs(nOldTab).bUseValve) OrElse _
                    (mtTabCfgs(nNewTab).bUseCustomParm <> mtTabCfgs(nOldTab).bUseCustomParm) OrElse _
                    (mtTabCfgs(nNewTab).bUseColor <> mtTabCfgs(nOldTab).bUseColor) Then
                If mtTabCfgs(nNewTab).bUseValve Then
                    lblColor.Text = gpsRM.GetString("psVALVE_CAP")
                    lblColor.Visible = True
                    cboColor.Visible = True
                    bReload = False 'Changed this setting, don't reload
                    mnuAddAllColors.Text = gpsRM.GetString("psADD_ALL") & gpsRM.GetString("psVALVES_CAP")
                ElseIf mtTabCfgs(nNewTab).bUseColor Then
                    lblColor.Text = gcsRM.GetString("csCOLOR_CAP")
                    lblColor.Visible = True
                    cboColor.Visible = True
                    bReload = False 'Changed this setting, don't reload
                    mnuAddAllColors.Text = gpsRM.GetString("psADD_ALL") & gpsRM.GetString("psCOLORS_CAP")
                Else
                    lblColor.Visible = False
                    cboColor.Visible = False
                    bReload = False 'Changed this setting, don't reload
                End If
            End If
            If mtTabCfgs(nNewTab).oGraphs Is Nothing Then
                btnGraph.Visible = False
            Else
                btnGraph.DropDownItems.Clear()
                For Each oGraph As tGraphConfig In mtTabCfgs(nNewTab).oGraphs
                    btnGraph.DropDownItems.Add(oGraph.sGraphName)
                Next
                For Each oItem As ToolStripMenuItem In btnGraph.DropDownItems
                    AddHandler oItem.Click, AddressOf mnuGraph_Click
                Next
                btnGraph.Visible = True
            End If
            If mtTabCfgs(nNewTab).sToon = String.Empty Then
                spltMain.Panel2Collapsed = True
                moToonControl = Nothing
                moToonInterface = Nothing
            Else
                moToonControl = Nothing
                moToonInterface = Nothing
                Select Case mtTabCfgs(nNewTab).sToon
                    Case "uctrlISD"
                        moToonControl = New uctrlISD
                    Case "uctrlIPS"
                        moToonControl = New uctrlIPS
                End Select
                If moToonControl IsNot Nothing Then
                    moToonInterface = DirectCast(moToonControl, PrmSetToon)
                    spltMain.Panel2.Controls.Add(moToonControl)
                    moToonControl.Dock = DockStyle.Fill
                    moToonControl.Visible = True
                    moToonInterface.Init()
                End If
                If (btnMultiView.Visible = False) Or (btnMultiView.Checked = False) Then
                    spltMain.Panel2Collapsed = False
                End If
            End If
            'Setup the robot cbo - with mbEventBlocker = True it'll setup labels but not load from the robot.
            If (nOldTab = -1) OrElse _
                     (mtTabCfgs(nNewTab).bUseArm <> mtTabCfgs(nOldTab).bUseArm) OrElse _
                    (mtTabCfgs(nNewTab).bUseController <> mtTabCfgs(nOldTab).bUseController) OrElse _
                    (mtTabCfgs(nNewTab).bShowOpeners <> mtTabCfgs(nOldTab).bShowOpeners) OrElse _
                    bForceReload Then 'HGB 11/15/12
                If mtTabCfgs(nNewTab).bUseArm Then
                    If nOldTab > -1 Then 'still reload if it was using the arm
                        bReload = bReload And (mtTabCfgs(nOldTab).bUseArm Or mtTabCfgs(nOldTab).bUseController)
                    Else
                        bReload = False
                    End If
                    lblRobot.Text = gcsRM.GetString("csROBOT")
                    lblRobot.Visible = True
                    cboRobot.Visible = True
                    mPWRobotCommon.LoadRobotBoxFromCollection(cboRobot, colArms, False, , , , mtTabCfgs(nNewTab).bShowOpeners)
                    cboRobot.SelectedIndex = -1
                    If msOldRobot <> String.Empty Then
                        cboRobot.Text = msOldRobot
                    End If
                    mnuAddAllRobots.Text = gpsRM.GetString("psADD_ALL") & gpsRM.GetString("psROBOTS_CAP")
                ElseIf mtTabCfgs(nNewTab).bUseController Then
                    If nOldTab > -1 Then 'still reload if it was using the arm
                        bReload = bReload And (mtTabCfgs(nOldTab).bUseArm Or mtTabCfgs(nOldTab).bUseController)
                    Else
                        bReload = False
                    End If
                    lblRobot.Text = gpsRM.GetString("psCONTROLLER_CAP")
                    lblRobot.Visible = True
                    cboRobot.Visible = True
                    mPWRobotCommon.LoadRobotBoxFromCollection(cboRobot, colControllers, False)
                    cboRobot.SelectedIndex = -1
                    If msOldController <> String.Empty Then
                        cboRobot.Text = msOldController
                    End If
                    mnuAddAllRobots.Text = gpsRM.GetString("psADD_ALL") & gpsRM.GetString("psCONTROLLERS_CAP")
                Else
                    lblRobot.Visible = False
                    cboRobot.Visible = False
                End If
            End If
            If (nOldTab = -1) OrElse _
                    (mtTabCfgs(nNewTab).bUseCustomParm <> mtTabCfgs(nOldTab).bUseCustomParm) OrElse _
                    (mtTabCfgs(nNewTab).sCustomParm <> mtTabCfgs(nOldTab).sCustomParm) Then
                If mtTabCfgs(nNewTab).bUseCustomParm = False Then
                    lblParm.Visible = False
                    cboParm.Visible = False
                Else
                    bReload = False 'Adding custom cbo to selection
                    'custom parameter - listed as comma separated values in the DB
                    '1st item is the title
                    '2nd item is a display value, 3rd is the tag
                    '4th item is a display value, 5th is the tag ...
                    Dim sTmp() As String = Split(mtTabCfgs(nNewTab).sCustomParm, ",")
                    lblParm.Text = sGetResTxtFrmDBTxt(sTmp(0))
                    Dim nCount As Integer = (sTmp.GetUpperBound(0) - 1) \ 2
                    Dim nIndex As Integer = 0
                    Dim sTag(nCount) As String
                    cboParm.Items.Clear()
                    For nIndex = 0 To nCount
                        cboParm.Items.Add(sGetResTxtFrmDBTxt(sTmp(nIndex * 2 + 1)))
                        sTag(nIndex) = sTmp(nIndex * 2 + 2)
                    Next
                    cboParm.Tag = sTag
                    lblParm.Visible = True
                    cboParm.Visible = True
                    mnuAddAllCustom.Text = gpsRM.GetString("psADD_ALL") & sGetResTxtFrmDBTxt(sTmp(0) & "_S")
                End If
            End If
            mnuAddAllColors.Visible = cboColor.Visible
            mnuAddAllRobots.Visible = cboRobot.Visible
            mnuAddAllPainters.Visible = cboRobot.Visible
            mnuAddAllOpeners.Visible = cboRobot.Visible
            mnuAddAllCustom.Visible = cboParm.Visible
            mnuAddAllColors.Enabled = False
            mnuAddAllRobots.Enabled = False
            mnuAddAllPainters.Enabled = False
            mnuAddAllOpeners.Enabled = False
            mnuAddAllCustom.Enabled = False

            'Find the new tab's data grid control
            mdgvData = DirectCast(tabMain.SelectedTab.Controls("dgv" & mnTab.ToString), DataGridView)
            If mdgvData Is Nothing Then
                'First pass
                mdgvData = New DataGridView
                mdgvData.Name = "dgv" & mnTab.ToString
                mdgvData.Dock = DockStyle.Fill
                mdgvData.EditMode = DataGridViewEditMode.EditProgrammatically
                mdgvData.SelectionMode = DataGridViewSelectionMode.CellSelect
                mdgvData.MultiSelect = False
                mdgvData.AllowUserToAddRows = False
                mdgvData.AllowUserToDeleteRows = False
                mdgvData.AllowUserToOrderColumns = True
            End If
            mdgvData.Enabled = False
            'Format the grid
            mdgvData.ColumnCount = 1
            mdgvData.RowCount = mtTabCfgs(mnTab).ItemCfg.GetUpperBound(0) + mnITEM0_ROW + 1
            mdgvData.Font = lblZone.Font

            mdgvData.Rows(mnZONE_ROW).HeaderCell.Value = gcsRM.GetString("csZONE_CAP")
            mdgvData.Rows(mnCONTROLLER_ROW).HeaderCell.Value = gpsRM.GetString("psCONTROLLER_CAP")
            mdgvData.Rows(mnROBOT_ROW).HeaderCell.Value = gcsRM.GetString("csROBOT")
            mdgvData.Rows(mnCOLOR_ROW).HeaderCell.Value = gcsRM.GetString("csCOLOR_CAP")
            mdgvData.Rows(mnVALVE_ROW).HeaderCell.Value = gpsRM.GetString("psVALVE_CAP")
            mdgvData.Rows(mnCUSTOMPARM_ROW).HeaderCell.Value = lblParm.Text
            For nRow As Integer = 0 To mtTabCfgs(mnTab).ItemCfg.GetUpperBound(0)
                mdgvData.Rows(nRow + mnITEM0_ROW).HeaderCell.Value = mtTabCfgs(mnTab).ItemCfg(nRow).Label
            Next
            mdgvData.ColumnHeadersVisible = False
            mdgvData.BackgroundColor = System.Drawing.SystemColors.Control
            mdgvData.BorderStyle = BorderStyle.None
            'Put it on the tab page
            tabMain.SelectedTab.Controls.Add(mdgvData)
            'Shouldn't really be needed, but it makes me more comfortable
            mdgvData = DirectCast(tabMain.SelectedTab.Controls("dgv" & mnTab.ToString), DataGridView)

            mdgvData.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders)
            If mtTabCfgs(mnTab).bUseZone Then
                mdgvData.Rows(mnZONE_ROW).Visible = True
            Else
                mdgvData.Rows(mnZONE_ROW).Visible = False
            End If
            If mtTabCfgs(mnTab).bUseArm Then
                mdgvData.Rows(mnCONTROLLER_ROW).Visible = True
                mdgvData.Rows(mnROBOT_ROW).Visible = True
            ElseIf mtTabCfgs(mnTab).bUseController Then
                mdgvData.Rows(mnCONTROLLER_ROW).Visible = True
                mdgvData.Rows(mnROBOT_ROW).Visible = False
            Else
                mdgvData.Rows(mnCONTROLLER_ROW).Visible = False
                mdgvData.Rows(mnROBOT_ROW).Visible = False
            End If
            If mtTabCfgs(mnTab).bUseValve Then
                mdgvData.Rows(mnCOLOR_ROW).Visible = True
                mdgvData.Rows(mnVALVE_ROW).Visible = True
            ElseIf mtTabCfgs(mnTab).bUseColor Then
                mdgvData.Rows(mnCOLOR_ROW).Visible = True
                mdgvData.Rows(mnVALVE_ROW).Visible = False
            Else
                mdgvData.Rows(mnCOLOR_ROW).Visible = False
                mdgvData.Rows(mnVALVE_ROW).Visible = False
            End If
            If mtTabCfgs(mnTab).bUseCustomParm Then
                mdgvData.Rows(mnCUSTOMPARM_ROW).Visible = True
            Else
                mdgvData.Rows(mnCUSTOMPARM_ROW).Visible = False
            End If
            If (cboRobot.Text = String.Empty) And (mtTabCfgs(mnTab).bUseArm Or _
                    mtTabCfgs(mnTab).bUseController) Then
                If cboRobot.Items.Count = 1 Then
                    'this selects the robot in cases of just one zone. fires event to call subChangeRobot
                    cboRobot.Text = cboRobot.Items(0).ToString
                Else
                    'statusbar text
                    Status(True) = gcsRM.GetString("csSELECT_ROBOT")
                End If
            End If
            If mtTabCfgs(mnTab).bNoEdit Then
                btnSave.Visible = False
                btnUndo.Visible = False
                mnuImport.Visible = False
            Else
                btnSave.Visible = True
                btnUndo.Visible = True
                mnuImport.Visible = True
            End If
            If mtTabCfgs(mnTab).nAutoRefresh > 0 Then
                tmrRefresh.Interval = mtTabCfgs(mnTab).nAutoRefresh
                btnRefresh.Visible = True
            Else
                btnRefresh.Visible = False
            End If

            If cboRobot.Visible OrElse cboColor.Visible OrElse cboParm.Visible Then
                btnMultiView.Visible = True
            Else
                btnMultiView.Visible = False
                If btnMultiView.Checked Then
                    btnMultiView.Checked = False
                    btnAdd.Visible = False
                    btnClear.Visible = False
                End If
            End If
            mbEventBlocker = False
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subEnableControls(False)
            Status = gcsRM.GetString("csLOADINGDATA")
            If bReload Then
                If btnMultiView.Visible And btnMultiView.Checked And mbSimpleViewAll Then
                    mnuAddAll_Click(mnuAddAllRobots)
                Else
                    mdgvData.ColumnCount = dgvLast.ColumnCount
                    For nCol As Integer = 0 To mdgvData.ColumnCount - 1
                        For nRow As Integer = 0 To mnCUSTOMPARM_ROW
                            mdgvData.Rows(nRow).Cells(nCol).Value = dgvLast.Rows(nRow).Cells(nCol).Value
                        Next
                    Next
                    'Need all the column headers filled before calling the load routine
                    For nCol As Integer = 0 To mdgvData.ColumnCount - 1
                        bLoadData(True, nCol)
                    Next
                End If
                If bSetAutoRefresh Then
                    Application.DoEvents()
                    mnuAutoRefresh.Checked = True
                    mnRefreshRotation = -1
                    Application.DoEvents()
                    subRefreshData()
                End If
            Else
                subClearScreen()
                If (Not (btnMultiView.Checked)) Then
                    bLoadData()
                End If
            End If
            If mbAbortForShutdown Then
                Exit Sub
            End If
            mdgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
            mdgvData.AutoResizeColumns()
            mdgvData.Enabled = True
            'subFormatScreenLayout()
            EditsMade = False
            Me.Cursor = System.Windows.Forms.Cursors.Default
            Status(True) = gpsRM.GetString("csREADY")
            msSCREEN_DUMP_NAME = msTableName & "_" & mtTabCfgs(mnTab).TabTag
            subEnableControls(True)

            Me.stsStatus.Refresh()
            mbDontChangeTabs = False
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try
        mbDontChangeTabs = False

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
        ' 11/15/12  HGB     Force a reload of the robot combo
        '********************************************************************************************
        Progress = 10
        ' false means user pressed cancel
        If bAskForSave() = False Then
            cboZone.Text = msOldZone
            Progress = 100
            Exit Sub
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

        'HGB Clear out the old controller on a zone change
        msOldController = String.Empty
        msOldRobot = String.Empty
        cboRobot.SelectedIndex = -1

        Try

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subClearScreen()

            subEnableControls(False)
            If mbUSEROBOTS Then
                colControllers = New clsControllers(colZones, False)
                SetUpStatusStrip(Me, colControllers)
                colArms = LoadArmCollection(colControllers)
                ReDim mbSAEnableArm(colArms.Count - 1)
                mbOpenersExist = False
                For Each oArm As clsArm In colArms
                    If oArm.IsOpener Then
                        mbOpenersExist = True
                        Exit For
                    End If
                Next
                'make sure the robot cartoons get the right color
                colControllers.RefreshConnectionStatus()
                cboColor.Items.Clear()
                msOldColor = String.Empty
                msOldRobot = String.Empty

                ReDim mbRobotNotAvailable(colControllers.Count - 1)
                For Each bTmp As Boolean In mbRobotNotAvailable
                    bTmp = False
                Next
            Else
                colControllers = Nothing
                SetUpStatusStrip(Me, colControllers)
                colArms = Nothing
                ReDim mbSAEnableArm(0)
                mbOpenersExist = False
                cboColor.Items.Clear()
                msOldColor = String.Empty
                msOldRobot = String.Empty

                ReDim mbRobotNotAvailable(0)
                For Each bTmp As Boolean In mbRobotNotAvailable
                    bTmp = False
                Next
            End If
            'Select tab 1
            mbDontChangeTabs = True
            tabMain.SelectedIndex = 0

            mbDontChangeTabs = False
            'HGB parameter added to force a reload of the robot combo
            subChangeTab(True)
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
        mbEventBlocker = bTmp
        If (mdgvData IsNot Nothing) Then
            mdgvData.CancelEdit()
            mdgvData.ColumnCount = 1
            For nItem As Integer = 0 To mdgvData.RowCount - 1
                mdgvData.Rows(nItem).Cells(0).Value = Nothing
            Next
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
        Dim bEditsMade As Boolean = EditsMade
        btnClose.Enabled = True

        If bEnable = False Then
            btnSave.Enabled = False
            btnUndo.Enabled = False
            btnRefresh.Enabled = False
            btnCopy.Enabled = False
            btnPrint.Enabled = False
            btnChangeLog.Enabled = False
            btnStatus.Enabled = True
            btnMultiView.Enabled = True
            bRestOfControls = False
            btnAdd.Enabled = False
            btnClear.Enabled = False
            mnuPrintFile.Enabled = False
            If (mdgvData IsNot Nothing) Then
                mdgvData.EndEdit()
            End If
            btnUtilities.Enabled = False
            tabMain.Enabled = False
            btnGraph.Enabled = False
        Else
            tabMain.Enabled = True
            Select Case Privilege
                Case ePrivilege.None
                    btnSave.Enabled = False
                    btnUndo.Enabled = False
                    btnCopy.Enabled = False
                    btnPrint.Enabled = DataLoaded
                    btnRefresh.Enabled = DataLoaded
                    btnChangeLog.Enabled = True
                    btnStatus.Enabled = True
                    bRestOfControls = False
                    btnMultiView.Enabled = True
                    If (mdgvData IsNot Nothing) Then
                        mdgvData.EndEdit()
                    End If
                    mnuPrintFile.Enabled = False
                    btnClear.Enabled = DataLoaded
                    btnUtilities.Enabled = False
                    btnGraph.Enabled = True
                Case ePrivilege.Edit
                    btnSave.Enabled = bEditsMade
                    btnUndo.Enabled = bEditsMade
                    btnPrint.Enabled = DataLoaded
                    btnRefresh.Enabled = DataLoaded
                    btnMultiView.Enabled = True
                    btnChangeLog.Enabled = True
                    btnStatus.Enabled = True
                    bRestOfControls = True
                    btnCopy.Enabled = False
                    btnClear.Enabled = DataLoaded
                    mnuPrintFile.Enabled = DataLoaded
                    btnUtilities.Enabled = (True And DataLoaded)
                    btnGraph.Enabled = True
                Case ePrivilege.Copy
                    btnCopy.Enabled = DataLoaded
                    btnSave.Enabled = bEditsMade
                    btnUndo.Enabled = bEditsMade
                    btnPrint.Enabled = DataLoaded
                    btnRefresh.Enabled = DataLoaded
                    btnMultiView.Enabled = True
                    btnChangeLog.Enabled = True
                    btnStatus.Enabled = True
                    bRestOfControls = True
                    btnClear.Enabled = DataLoaded
                    mnuPrintFile.Enabled = DataLoaded
                    btnUtilities.Enabled = (True And DataLoaded)
                    btnGraph.Enabled = True
            End Select
            If (mtTabCfgs IsNot Nothing) And (mnTab >= 0) Then
                'OK to load
                '0 = not used, 1= cbo selected, 2 = cbo empty
                Dim nRobot As Integer = 0
                Dim nColor As Integer = 0
                Dim nCustom As Integer = 0
                If mtTabCfgs(mnTab).bUseArm Or mtTabCfgs(mnTab).bUseController Then
                    If cboRobot.Text = String.Empty Then
                        nRobot = 2
                    Else
                        nRobot = 1
                    End If
                End If
                If mtTabCfgs(mnTab).bUseValve Or mtTabCfgs(mnTab).bUseColor Then
                    If cboColor.Text = String.Empty Then
                        nColor = 2
                    Else
                        nColor = 1
                    End If
                End If
                If mtTabCfgs(mnTab).bUseCustomParm Then
                    If cboParm.Text = String.Empty Then
                        nCustom = 2
                    Else
                        nCustom = 1
                    End If
                End If
                If btnMultiView.Checked Then
                    'Enable add all drop-downs
                    mnuAddAllRobots.Enabled = (nRobot > 0) And (nColor < 2) And (nCustom < 2)
                    mnuAddAllPainters.Enabled = mnuAddAllRobots.Enabled And mbOpenersExist And mtTabCfgs(mnTab).bUseArm And mtTabCfgs(mnTab).bShowOpeners
                    mnuAddAllPainters.Visible = mnuAddAllPainters.Enabled And mtTabCfgs(mnTab).bShowOpeners
                    mnuAddAllOpeners.Enabled = mnuAddAllRobots.Enabled And mbOpenersExist And mtTabCfgs(mnTab).bUseArm And mtTabCfgs(mnTab).bShowOpeners
                    mnuAddAllOpeners.Visible = mnuAddAllOpeners.Enabled
                    mnuAddAllColors.Enabled = (nRobot < 2) And (nColor > 0) And (nCustom < 2)
                    mnuAddAllCustom.Enabled = (nRobot < 2) And (nColor < 2) And (nCustom > 0)
                    btnAdd.Enabled = ((nRobot < 2) And (nColor < 2) And (nCustom < 2)) Or _
                        mnuAddAllRobots.Enabled Or mnuAddAllColors.Enabled Or mnuAddAllCustom.Enabled
                Else
                    btnAdd.Enabled = (nRobot < 2) And (nColor < 2) And (nCustom < 2)
                End If
            Else
                btnAdd.Enabled = False
                mnuAddAllRobots.Enabled = False
                mnuAddAllPainters.Enabled = False
                mnuAddAllOpeners.Enabled = False
                mnuAddAllColors.Enabled = False
                mnuAddAllCustom.Enabled = False
            End If
        End If


    End Sub
    'Private Function subFormatScreenLayout() As Integer
    '    '********************************************************************************************
    '    'Description:  new data selected - set up the screen layout
    '    '
    '    'Parameters: none
    '    'Returns:    none
    '    '
    '    'Modification history:
    '    '
    '    ' Date      By      Reason
    '    '********************************************************************************************
    '    With gpsRM
    '    End With

    'End Function
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
            mnuYes.Text = gpsRM.GetString("psYES")
            mnuNo.Text = gpsRM.GetString("psNO")
            btnAdd.Text = gpsRM.GetString("psADD")
            btnClear.Text = gpsRM.GetString("psCLEAR")
            mnuAddAllPainters.Text = gpsRM.GetString("psADD_ALL") & gpsRM.GetString("psPainters")
            mnuAddAllOpeners.Text = gpsRM.GetString("psADD_ALL") & gpsRM.GetString("psOpeners")
            mnuAutoRefresh.Text = gpsRM.GetString("psAUTOREFRESH")
            mnuRefreshRate.Text = gpsRM.GetString("psREFRESHRATE")
            btnRefresh.Text = gpsRM.GetString("psREFRESH")
            Do While (tabMain.TabPages.Count > (mtTabCfgs.GetUpperBound(0) + 1))
                tabMain.TabPages.RemoveAt(tabMain.TabPages.Count - 1)
            Loop
            For nTab As Integer = 0 To mtTabCfgs.GetUpperBound(0)
                'only add a tab if it has items
                If (mtTabCfgs(nTab).ItemCfg IsNot Nothing) Then
                    If mtTabCfgs(nTab).ItemCfg.GetUpperBound(0) > 0 Then
                        mCombinedTabCfg.bUseZone = mCombinedTabCfg.bUseZone Or mtTabCfgs(nTab).bUseZone
                        mCombinedTabCfg.bUseArm = mCombinedTabCfg.bUseArm Or mtTabCfgs(nTab).bUseArm
                        mCombinedTabCfg.bUseController = mCombinedTabCfg.bUseController Or mtTabCfgs(nTab).bUseController
                        mCombinedTabCfg.bUseColor = mCombinedTabCfg.bUseColor Or mtTabCfgs(nTab).bUseColor
                        mCombinedTabCfg.bUseValve = mCombinedTabCfg.bUseValve Or mtTabCfgs(nTab).bUseValve
                        If tabMain.TabPages.Count <= nTab Then
                            tabMain.TabPages.Add(mtTabCfgs(nTab).TabName)
                        Else
                            tabMain.TabPages.Item(nTab).Text = mtTabCfgs(nTab).TabName
                        End If
                    End If
                End If
            Next
            If mCombinedTabCfg.bUseZone Then
                lblZone.Text = gcsRM.GetString("csZONE_CAP")
                cboZone.Visible = True
                lblZone.Visible = True
            Else
                cboZone.Visible = False
                lblZone.Visible = False
            End If
            lblColor.Visible = False
            cboColor.Visible = False
            lblParm.Visible = False
            cboParm.Visible = False
            lblRobot.Visible = False
            cboRobot.Visible = False
        End With
    End Sub
    'Private Function subLoadItemTable(ByRef sScreen As String) As DataTable
    '    '********************************************************************************************
    '    'Description: Load the item table
    '    '
    '    'Parameters: none
    '    'Returns:    none
    '    '
    '    'Modification history:
    '    '
    '    ' Date      By      Reason
    '    '********************************************************************************************
    '    Dim sTable As String = "[" & sScreen & gsPRMSET_ITEM_SFX & "]"
    '    Dim DS As New DataSet

    '    Try

    '        Dim DB As New clsSQLAccess

    '        With DB
    '            .Zone = colZones.ActiveZone
    '            .DBFileName = gsPRMSET_DB_NAME
    '            .DBTableName = sTable
    '            .SQLString = "SELECT * FROM " & sTable & " ORDER BY " & _
    '                            sTable & ".[" & gsPRMSET_COL_INDEX & "]"
    '            DS = .GetDataSet
    '            .Close()
    '        End With

    '        If DS.Tables.Contains(sTable) Then
    '            Return (DS.Tables(sTable))
    '        Else
    '            Return (Nothing)
    '        End If

    '    Catch ex As Exception
    '        Trace.WriteLine(ex.Message)
    '        Dim x As New CheckedListBox
    '        Return (Nothing)
    '    End Try
    'End Function
    Private Function subLoadItemTable(ByRef sScreen As String) As DataTable
        '********************************************************************************************
        'Description: Load the tab table
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/04/12  MSW     Move to XML for speed
        '********************************************************************************************

        Dim sXMLFILE As String = "ParamSetup" & sScreen
        Const sXMLMAINNODE As String = "ParamSetupScreen"
        Const sXMLTABLE As String = "Items"
        Const sXMLNODE As String = "Item"


        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Dim oTable As DataTable = New DataTable



        oTable.Columns.Add(gsPRMSET_COL_LABEL)  '0
        oTable.Columns.Add(gsPRMSET_COL_TAB)  '1
        oTable.Columns.Add(gsPRMSET_COL_DATASOURCE)  '2
        oTable.Columns.Add(gsPRMSET_COL_DATATYPE)  '3
        oTable.Columns.Add(gsPRMSET_COL_PLCTAG)  '4
        oTable.Columns.Add(gsPRMSET_COL_PROGNAME1)  '5
        oTable.Columns.Add(gsPRMSET_COL_VARNAME1)  '6
        oTable.Columns.Add(gsPRMSET_COL_PROGNAME2)  '7
        oTable.Columns.Add(gsPRMSET_COL_VARNAME2)  '8
        oTable.Columns.Add(gsPRMSET_COL_DBNAME)  '9
        oTable.Columns.Add(gsPRMSET_COL_DBTABLENAME)  '10
        oTable.Columns.Add(gsPRMSET_COL_DBROWNAME)  '11
        oTable.Columns.Add(gsPRMSET_COL_DBCOLUMNNAME)  '12
        oTable.Columns.Add(gsPRMSET_COL_REGPATH)  '13
        oTable.Columns.Add(gsPRMSET_COL_REGKEY)  '14
        oTable.Columns.Add(gsPRMSET_COL_EDIT)  '15
        oTable.Columns.Add(gsPRMSET_COL_MIN)  '16
        oTable.Columns.Add(gsPRMSET_COL_MAX)  '17
        oTable.Columns.Add(gsPRMSET_COL_SCALE)  '18
        oTable.Columns.Add(gsPRMSET_COL_PRECISION)  '19
        oTable.Columns.Add(gsPRMSET_COL_USECBO)  '20
        oTable.Columns.Add(gsPRMSET_COL_CUSTOMCBO)  '21
        oTable.Columns.Add(gsPRMSET_COL_FORMULA)  '22
        oTable.Columns.Add(gsPRMSET_COL_PAINTER_ONLY)  '23
        oTable.Columns.Add(gsPRMSET_COL_DISPLAY)  '24
        oTable.Columns.Add(gsPRMSET_COL_CUSTOMCOLORS)  '25
        oTable.Columns.Add(gsPRMSET_COL_COLORRANGE) '26
        oTable.Columns.Add(gsPRMSET_COL_BITMASK) '27
        Try
            '    08/15/13   MSW     PSC remote PC support
            If GetDefaultFilePath(sXMLFilePath, eDir.XML, colZones.ActiveZone.RemotePath, sXMLFILE & ".XML") Then


                oXMLDoc.Load(sXMLFilePath)

                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLMAINNODE)
                oNodeList = oMainNode.SelectNodes("//" & sXMLTABLE & "//" & sXMLNODE)
                Try
                    If oNodeList.Count = 0 Then
                        mDebug.WriteEventToLog("frmMain:subLoadItemTable", sXMLFilePath & " not found.")
                    Else
                        Dim nMax As Integer = oNodeList.Count - 1
                        Dim sText(27) As String
                        For nNode As Integer = 0 To nMax
                            Try

                                sText(0) = oNodeList(nNode).Item(gsPRMSET_COL_LABEL).InnerText
                                sText(1) = oNodeList(nNode).Item(gsPRMSET_COL_TAB).InnerText
                                sText(2) = oNodeList(nNode).Item(gsPRMSET_COL_DATASOURCE).InnerText
                                sText(3) = oNodeList(nNode).Item(gsPRMSET_COL_DATATYPE).InnerText
                                sText(4) = oNodeList(nNode).Item(gsPRMSET_COL_PLCTAG).InnerText
                                sText(5) = oNodeList(nNode).Item(gsPRMSET_COL_PROGNAME1).InnerText
                                sText(6) = oNodeList(nNode).Item(gsPRMSET_COL_VARNAME1).InnerText
                                sText(7) = oNodeList(nNode).Item(gsPRMSET_COL_PROGNAME2).InnerText
                                sText(8) = oNodeList(nNode).Item(gsPRMSET_COL_VARNAME2).InnerText
                                sText(9) = oNodeList(nNode).Item(gsPRMSET_COL_DBNAME).InnerText
                                sText(10) = oNodeList(nNode).Item(gsPRMSET_COL_DBTABLENAME).InnerText
                                sText(11) = oNodeList(nNode).Item(gsPRMSET_COL_DBROWNAME).InnerText
                                sText(12) = oNodeList(nNode).Item(gsPRMSET_COL_DBCOLUMNNAME).InnerText
                                sText(13) = oNodeList(nNode).Item(gsPRMSET_COL_REGPATH).InnerText
                                sText(14) = oNodeList(nNode).Item(gsPRMSET_COL_REGKEY).InnerText
                                sText(15) = oNodeList(nNode).Item(gsPRMSET_COL_EDIT).InnerText
                                sText(16) = oNodeList(nNode).Item(gsPRMSET_COL_MIN).InnerText
                                sText(17) = oNodeList(nNode).Item(gsPRMSET_COL_MAX).InnerText
                                sText(18) = oNodeList(nNode).Item(gsPRMSET_COL_SCALE).InnerText
                                sText(19) = oNodeList(nNode).Item(gsPRMSET_COL_PRECISION).InnerText
                                sText(20) = oNodeList(nNode).Item(gsPRMSET_COL_USECBO).InnerText
                                sText(21) = oNodeList(nNode).Item(gsPRMSET_COL_CUSTOMCBO).InnerText
                                sText(22) = oNodeList(nNode).Item(gsPRMSET_COL_FORMULA).InnerText
                                sText(23) = oNodeList(nNode).Item(gsPRMSET_COL_PAINTER_ONLY).InnerText
                                sText(24) = oNodeList(nNode).Item(gsPRMSET_COL_DISPLAY).InnerText
                                If oNodeList(nNode).InnerXml.Contains(gsPRMSET_COL_CUSTOMCOLORS) Then
                                    sText(25) = oNodeList(nNode).Item(gsPRMSET_COL_CUSTOMCOLORS).InnerText
                                Else
                                    sText(25) = ""
                                End If
                                If oNodeList(nNode).InnerXml.Contains(gsPRMSET_COL_COLORRANGE) Then
                                    sText(26) = oNodeList(nNode).Item(gsPRMSET_COL_COLORRANGE).InnerText
                                Else
                                    sText(26) = ""
                                End If
                                If oNodeList(nNode).InnerXml.Contains(gsPRMSET_COL_BITMASK) Then
                                    sText(27) = oNodeList(nNode).Item(gsPRMSET_COL_BITMASK).InnerText
                                Else
                                    sText(27) = "0"
                                End If
                                oTable.Rows.Add(sText)

                            Catch ex As Exception
                                ShowErrorMessagebox(sXMLFilePath & "  : Invalid Data", ex, "frmMain:subLoadItemTable", String.Empty)
                            End Try
                        Next
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog("frmMain:subLoadItemTable", "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                End Try

            End If
            Return (oTable)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return Nothing
        End Try
    End Function
    Private Function subLoadTabTable(ByRef sScreen As String) As DataTable
        '********************************************************************************************
        'Description: Load the tab table
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/04/12  MSW     Move to XML for speed
        ' 08/08/13  MSW     Allow tabs to be disabled without removing them from the DB
        ' 10/10/13  MSW     Add Graph function for sealer
        '********************************************************************************************

        Dim sXMLFILE As String = "ParamSetup" & sScreen
        Const sXMLMAINNODE As String = "ParamSetupScreen"
        Const sXMLTABLE As String = "Tabs"
        Const sXMLNODE As String = "Tab"

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Dim oTable As DataTable = New DataTable
        oTable.Columns.Add(gsPRMSET_COL_TABLABEL)
        oTable.Columns.Add(gsPRMSET_COL_TABTAG)
        oTable.Columns.Add(gsPRMSET_COL_USEZONE)
        oTable.Columns.Add(gsPRMSET_COL_USECNTR)
        oTable.Columns.Add(gsPRMSET_COL_USEARM)
        oTable.Columns.Add(gsPRMSET_COL_USECOLOR)
        oTable.Columns.Add(gsPRMSET_COL_USEVALVE)
        oTable.Columns.Add(gsPRMSET_COL_USECUSTOMPARM)
        oTable.Columns.Add(gsPRMSET_COL_SHOWOPENERS)
        oTable.Columns.Add(gsPRMSET_COL_CUSTOMPARM)
        oTable.Columns.Add(gsPRMSET_COL_HELPFILE)
        oTable.Columns.Add(gsPRMSET_COL_NOEDIT)
        oTable.Columns.Add(gsPRMSET_COL_AUTOREFRESH)
        oTable.Columns.Add(gsPRMSET_COL_SA)
        oTable.Columns.Add(gsPRMSET_COL_HIDE)
        oTable.Columns.Add(gsPRMSET_COL_GRAPHS)
        oTable.Columns.Add(gsPRMSET_COL_TOON)

        Try
            '    08/15/13   MSW     PSC remote PC support
            If GetDefaultFilePath(sXMLFilePath, eDir.XML, colZones.ActiveZone.RemotePath, sXMLFILE & ".XML") Then


                oXMLDoc.Load(sXMLFilePath)

                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLMAINNODE)

                oNodeList = oMainNode.SelectNodes("//" & sXMLTABLE & "//" & sXMLNODE)

                Try
                    If oNodeList.Count = 0 Then
                        mDebug.WriteEventToLog("frmMain:subLoadTabTable", sXMLFilePath & " not found.")
                    Else
                        Dim nMax As Integer = oNodeList.Count - 1
                        Dim sText(16) As String
                        For nNode As Integer = 0 To nMax
                            Try
                                sText(0) = oNodeList(nNode).Item(gsPRMSET_COL_TABLABEL).InnerText
                                sText(1) = oNodeList(nNode).Item(gsPRMSET_COL_TABTAG).InnerText
                                sText(2) = oNodeList(nNode).Item(gsPRMSET_COL_USEZONE).InnerText
                                sText(3) = oNodeList(nNode).Item(gsPRMSET_COL_USECNTR).InnerText
                                sText(4) = oNodeList(nNode).Item(gsPRMSET_COL_USEARM).InnerText
                                sText(5) = oNodeList(nNode).Item(gsPRMSET_COL_USECOLOR).InnerText
                                sText(6) = oNodeList(nNode).Item(gsPRMSET_COL_USEVALVE).InnerText
                                sText(7) = oNodeList(nNode).Item(gsPRMSET_COL_USECUSTOMPARM).InnerText
                                Try
                                    sText(8) = ""
                                    If oNodeList(nNode).InnerXml.Contains("<" & gsPRMSET_COL_SHOWOPENERS & ">") Then
                                        sText(8) = oNodeList(nNode).Item(gsPRMSET_COL_SHOWOPENERS).InnerText
                                    End If
                                Catch ex As Exception
                                    sText(8) = ""
                                End Try
                                sText(9) = oNodeList(nNode).Item(gsPRMSET_COL_CUSTOMPARM).InnerText
                                sText(10) = oNodeList(nNode).Item(gsPRMSET_COL_HELPFILE).InnerText
                                Try
                                    sText(11) = oNodeList(nNode).Item(gsPRMSET_COL_NOEDIT).InnerText
                                Catch ex As Exception
                                    sText(11) = "False"
                                End Try
                                Try
                                    sText(12) = oNodeList(nNode).Item(gsPRMSET_COL_AUTOREFRESH).InnerText
                                Catch ex As Exception
                                    sText(12) = "0"
                                End Try
                                Try
                                    sText(13) = oNodeList(nNode).Item(gsPRMSET_COL_SA).InnerText
                                Catch ex As Exception
                                    sText(13) = "False"
                                End Try
                                Try
                                    sText(14) = oNodeList(nNode).Item(gsPRMSET_COL_HIDE).InnerText
                                Catch ex As Exception
                                    sText(14) = "False"
                                End Try
                                Try
                                    sText(15) = ""
                                    If oNodeList(nNode).InnerXml.Contains("<" & gsPRMSET_COL_GRAPHS & ">") Then
                                        Dim oGraphList As XmlNodeList = oNodeList(nNode).Item(gsPRMSET_COL_GRAPHS).ChildNodes
                                        If oGraphList.Count > 0 Then
                                            For nGraph As Integer = 0 To oGraphList.Count - 1
                                                sText(15) = sText(15) & oGraphList(nGraph).InnerText & ":"
                                            Next
                                        End If
                                    End If

                                Catch ex As Exception
                                    sText(15) = ""
                                End Try
                                Try
                                    sText(16) = ""
                                    If oNodeList(nNode).InnerXml.Contains("<" & gsPRMSET_COL_TOON & ">") Then
                                        sText(16) = oNodeList(nNode).Item(gsPRMSET_COL_TOON).InnerText
                                    End If
                                Catch ex As Exception
                                    sText(16) = ""
                                End Try
                                oTable.Rows.Add(sText)

                            Catch ex As Exception
                                ShowErrorMessagebox(sXMLFilePath & "  : Invalid Data", ex, "frmMain:subLoadTabTable", String.Empty)
                            End Try
                        Next
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog("frmMain:subLoadTabTable", "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                End Try

            End If
            Return (oTable)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return Nothing
        End Try
    End Function

    Private Sub subLoadTableList(ByRef table As DataTable)
        '********************************************************************************************
        'Description: Load the list of screens
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/04/12  MSW     Move to XML for speed
        '********************************************************************************************

        Const sXMLFILE As String = "ParamSetupScreens"
        Const sXMLTABLE As String = "ParamSetupScreens"
        Const sXMLNODE As String = "ParamSetupScreen"


        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        table = New DataTable
        table.Columns.Add(gsPRMSET_COL_TBLNAME)
        table.Columns.Add(gsPRMSET_COL_SCRNNAME)
        table.Columns.Add(gsPRMSET_COL_PWDNAME)
        table.Columns.Add(gsPRMSET_COL_ICON)
        'Use remote
        Try
            '    08/15/13   MSW     PSC remote PC support
            If GetDefaultFilePath(sXMLFilePath, eDir.XML, colZones.ActiveZone.RemotePath, sXMLFILE & ".XML") Then


                oXMLDoc.Load(sXMLFilePath)

                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                oNodeList = oMainNode.SelectNodes("//" & sXMLNODE)
                Try
                    If oNodeList.Count = 0 Then
                        mDebug.WriteEventToLog("frmMain:subLoadTableList", sXMLFilePath & " not found.")
                    Else
                        Dim nMax As Integer = oNodeList.Count - 1
                        Dim sText(3) As String
                        For nNode As Integer = 0 To nMax
                            Try
                                sText(0) = oNodeList(nNode).Item(gsPRMSET_COL_TBLNAME).InnerText
                                sText(1) = oNodeList(nNode).Item(gsPRMSET_COL_SCRNNAME).InnerText
                                sText(2) = oNodeList(nNode).Item(gsPRMSET_COL_PWDNAME).InnerText
                                Try
                                    sText(3) = oNodeList(nNode).Item(gsPRMSET_COL_ICON).InnerText
                                Catch ex As Exception
                                    sText(3) = "FormIcon"
                                End Try
                                table.Rows.Add(sText)

                            Catch ex As Exception
                                ShowErrorMessagebox(sXMLFilePath & "  : Invalid Data", ex, "frmMain:subLoadTableList", String.Empty)
                            End Try
                        Next
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog("frmMain:subLoadTableList", "Invalid XML Data: " & sXMLFilePath & " - " & ex.Message)
                End Try

            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub
    Friend Function sReadTextItem(ByVal dr As DataRow, ByVal sTag As String) As String
        '********************************************************************************************
        'Description: Put a bunch of error handling for a db read in one spot for each type
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If dr.Item(sTag) Is Nothing Then
                Return (String.Empty)
            Else
                If dr.Item(sTag).GetType Is GetType(DBNull) Then
                    Return (String.Empty)
                Else
                    Return Trim(dr.Item(sTag).ToString)
                End If
            End If
        Catch ex As Exception
            Return String.Empty
        End Try
    End Function
    Friend Function bReadBooleanItem(ByVal dr As DataRow, ByVal sTag As String) As Boolean
        '********************************************************************************************
        'Description: Put a bunch of error handling for a db read in one spot for each type
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If dr.Item(sTag) Is Nothing Then
                Return False
            Else
                If dr.Item(sTag).GetType Is GetType(DBNull) Then
                    Return False
                Else
                    If dr.Item(sTag).ToString = "" Then
                        Return False
                    Else
                        Return CType(dr.Item(sTag), Boolean)
                    End If
                End If
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function
    Friend Function bReadIntegerItem(ByVal dr As DataRow, ByVal sTag As String) As Integer
        '********************************************************************************************
        'Description: Put a bunch of error handling for a db read in one spot for each type
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If dr.Item(sTag) Is Nothing Then
                Return 0
            Else
                If dr.Item(sTag).GetType Is GetType(DBNull) Then
                    Return 0
                Else
                    Return CType(dr.Item(sTag), Integer)
                End If
            End If
        Catch ex As Exception
            Return 0
        End Try
    End Function
    Friend Function bReadFloatItem(ByVal dr As DataRow, ByVal sTag As String) As Double
        '********************************************************************************************
        'Description: Put a bunch of error handling for a db read in one spot for each type
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/12/10  MSW     Gettiing wierd results(DB 0.01 = 0.009999999999...), convert to string on 
        '                   the way to double smooths it out
        '********************************************************************************************
        Try
            If dr.Item(sTag) Is Nothing Then
                Return 0
            Else
                If dr.Item(sTag).GetType Is GetType(DBNull) Then
                    Return 0
                Else
                    Dim sTmp As String = dr.Item(sTag).ToString 'MSW
                    Return CType(sTmp, Double)
                End If
            End If
        Catch ex As Exception
            Return 0
        End Try
    End Function
    Friend Function sGetResTxtFrmDBTxt(ByVal sTag As String) As String
        '********************************************************************************************
        'Description: get text from resource file using tag from DB
        '               if there's nothing in the resource file, return the DB tag
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp As String = gpsRM.GetString(Trim(sTag))
        If sTmp = String.Empty Then
            Return sTag
        Else
            Return sTmp
        End If
    End Function
    Private Sub subCheckCbos(ByRef sTag As String, ByRef oTabCfg As tTabCfg)
        '********************************************************************************************
        'Description:  replace tokens in variable, PLC, or
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If InStr(sTag, "#Arm#") > 0 Then
            oTabCfg.bUseArm = True
            oTabCfg.bUseController = True
        ElseIf InStr(sTag, "#TsParm") > 0 Then
            oTabCfg.bUseArm = True
            oTabCfg.bUseController = True
        ElseIf InStr(sTag, "#Parm") > 0 Then
            oTabCfg.bUseArm = True
            oTabCfg.bUseController = True
        ElseIf InStr(sTag, "#Equip") > 0 Then
            oTabCfg.bUseArm = True
            oTabCfg.bUseController = True
        ElseIf InStr(sTag, "#Controller") > 0 Then
            oTabCfg.bUseController = True
        End If
        If InStr(sTag, "#Valve#") > 0 Then
            oTabCfg.bUseValve = True
        End If
        If InStr(sTag, "#Color#") > 0 Then
            oTabCfg.bUseColor = True
        End If
        If InStr(sTag, "#Zone#") > 0 Then
            oTabCfg.bUseZone = True
        End If
    End Sub

    Friend Function bGetScreenCfg(ByRef sScreenName As String, ByRef sTableName As String, _
                               ByRef TabCfgs As tTabCfg()) As Boolean
        '********************************************************************************************
        'Description: Load screen config from DB
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/1/12   BTK     Changed bGetScreenCfg to use semicolon to parse out customCbo data in the xml file.
        ' 08/08/13  MSW     Allow tabs to be disabled without removing them from the DB
        ' 10/10/13  MSW     Add Graph function for sealer
        '********************************************************************************************
        'Set globals for change log, password
        msSCREEN_NAME = msPwdName
        msSCREEN_DUMP_NAME = sTableName
        'gsChangeLogArea = sTableName ' New password
        'Read tables from DB
        Dim dtTabTable As DataTable
        Dim dtItemTable As DataTable
        dtTabTable = subLoadTabTable(sTableName)
        If dtTabTable Is Nothing Then
            Return False
        End If
        dtItemTable = subLoadItemTable(sTableName)
        If dtItemTable Is Nothing Then
            Return False
        End If
        ReDim TabCfgs(dtTabTable.Rows.Count - 1)
        'Configure tabs
        Dim nIndex As Integer = 0
        mbUSEROBOTS = False
        Dim sTmp As String = String.Empty
        For Each dr As DataRow In dtTabTable.Rows
            With TabCfgs(nIndex)
                If dr.Item(gsPRMSET_COL_TABLABEL) Is Nothing Then
                    Return False 'Can't have this item empty
                Else
                    If dr.Item(gsPRMSET_COL_TABLABEL).GetType Is GetType(DBNull) Then
                        Return False
                    Else
                        .TabName = sGetResTxtFrmDBTxt(dr.Item(gsPRMSET_COL_TABLABEL).ToString)
                    End If
                End If
                If dr.Item(gsPRMSET_COL_TABTAG) Is Nothing Then
                    Return False 'Can't have this item empty
                Else
                    If dr.Item(gsPRMSET_COL_TABTAG).GetType Is GetType(DBNull) Then
                        Return False
                    Else
                        .TabTag = Trim(dr.Item(gsPRMSET_COL_TABTAG).ToString)
                    End If
                End If
                .bUseZone = bReadBooleanItem(dr, gsPRMSET_COL_USEZONE)
                .bUseController = bReadBooleanItem(dr, gsPRMSET_COL_USECNTR)
                .bUseArm = bReadBooleanItem(dr, gsPRMSET_COL_USEARM)
                .bShowOpeners = bReadBooleanItem(dr, gsPRMSET_COL_SHOWOPENERS)
                If .bUseController Or .bUseArm Then
                    mbUSEROBOTS = True
                End If
                .sToon = sReadTextItem(dr, gsPRMSET_COL_TOON)
                .bUseColor = bReadBooleanItem(dr, gsPRMSET_COL_USECOLOR)
                .bUseValve = bReadBooleanItem(dr, gsPRMSET_COL_USEVALVE)
                .bUseCustomParm = bReadBooleanItem(dr, gsPRMSET_COL_USECUSTOMPARM)
                .sCustomParm = sReadTextItem(dr, gsPRMSET_COL_CUSTOMPARM)
                .sHelpFile = sReadTextItem(dr, gsPRMSET_COL_HELPFILE)
                Try
                    .bNoEdit = bReadBooleanItem(dr, gsPRMSET_COL_NOEDIT)
                Catch ex As Exception
                    .bNoEdit = False
                End Try
                Try
                    .nAutoRefresh = bReadIntegerItem(dr, gsPRMSET_COL_AUTOREFRESH)
                Catch ex As Exception
                    .nAutoRefresh = 0
                End Try
                Try
                    .bScatteredAccess = bReadBooleanItem(dr, gsPRMSET_COL_SA)
                Catch ex As Exception
                    .bScatteredAccess = False
                End Try
                'Graphs
                'It gets stuffed into a big string because it gets passed through a dataset
                Try
                    Dim sGraphs As String = sReadTextItem(dr, gsPRMSET_COL_GRAPHS)
                    If sGraphs <> String.Empty Then
                        Dim sGraph As String() = Split(sGraphs, ":")
                        ReDim .oGraphs(sGraph.GetUpperBound(0))
                        Dim nGraph As Integer = 0
                        Dim nString As Integer = 0
                        Const nPARMS_PER_LINE As Integer = 4
                        Do While nString < sGraph.GetUpperBound(0)
                            Try
                                Dim sGraphData As String() = Split(sGraph(nString), ";")
                                Dim nItems As Integer = sGraphData.GetUpperBound(0) \ nPARMS_PER_LINE
                                With .oGraphs(nGraph)
                                    .sGraphName = sGetResTxtFrmDBTxt(sGraphData(0))
                                    ReDim .ItemNames(nItems - 1)
                                    ReDim .ItemNumbers(nItems - 1)
                                    ReDim .ItemUnits(nItems - 1)
                                    ReDim .ItemMax(nItems - 1)
                                    ReDim .ItemMin(nItems - 1)
                                    ReDim .ItemCfg(nItems - 1)
                                    ReDim .ItemObj(nItems - 1)
                                    For nItem As Integer = 0 To nItems - 1
                                        .ItemNames(nItem) = sGraphData((nItem * nPARMS_PER_LINE) + 1)
                                        .ItemUnits(nItem) = sGetResTxtFrmDBTxt(sGraphData((nItem * nPARMS_PER_LINE) + 2))
                                        .ItemMin(nItem) = CInt(sGraphData((nItem * nPARMS_PER_LINE) + 3))
                                        .ItemMax(nItem) = CInt(sGraphData((nItem * nPARMS_PER_LINE) + 4))
                                        .ItemNumbers(nItem) = -1
                                    Next
                                End With
                                nGraph += 1
                            Catch ex As Exception
                                '
                            End Try

                            nString += 1
                        Loop
                        ReDim Preserve .oGraphs(nGraph - 1)
                    Else
                        .oGraphs = Nothing
                    End If
                    Debug.Print(" ")
                Catch ex As Exception
                    .oGraphs = Nothing
                End Try
                'Allow tabs to be disabled without removing them from the DB
                Try
                    .Hide = bReadBooleanItem(dr, gsPRMSET_COL_HIDE)
                Catch ex As Exception
                    .Hide = True
                End Try
                If .Hide Then
                    ReDim Preserve TabCfgs(TabCfgs.GetUpperBound(0) - 1)
                Else
                    nIndex += 1
                End If
            End With
        Next 'dr As DataRow In dtTabTable.Rows
        'Configure Items
        Dim sTab As String = String.Empty
        For Each dr As DataRow In dtItemTable.Rows
            If dr.Item(gsPRMSET_COL_LABEL) Is Nothing Then
                Return False 'Can't have this item empty
            Else
                'check if it's enabled
                'default to enable if they dont't fill in the DB item
                Dim bEnable As Boolean = True
                Try
                    If dr.Item(gsPRMSET_COL_DISPLAY) Is Nothing Then
                        bEnable = True
                    Else
                        If dr.Item(gsPRMSET_COL_DISPLAY).GetType Is GetType(DBNull) Then
                            bEnable = True
                        Else
                            If dr.Item(gsPRMSET_COL_DISPLAY).ToString = "" Then
                                bEnable = True
                            Else
                                bEnable = CType(dr.Item(gsPRMSET_COL_DISPLAY), Boolean)
                            End If
                        End If
                    End If
                Catch ex As Exception
                    bEnable = True
                End Try
                If bEnable Then
                    sTab = Trim(dr.Item(gsPRMSET_COL_TAB).ToString)
                    'For Each TabCfg As tTabCfg In TabCfgs
                    For nTab As Integer = 0 To TabCfgs.GetUpperBound(0)
                        'Find which tab it belongs in
                        If (sTab = TabCfgs(nTab).TabTag) Then
                            'Found it, add the item
                            If TabCfgs(nTab).ItemCfg Is Nothing Then
                                nIndex = 0
                            Else
                                nIndex = TabCfgs(nTab).ItemCfg.GetUpperBound(0) + 1
                            End If
                            ReDim Preserve TabCfgs(nTab).ItemCfg(nIndex)
                            With TabCfgs(nTab).ItemCfg(nIndex)
                                .Tag = sReadTextItem(dr, gsPRMSET_COL_LABEL)
                                .Label = sGetResTxtFrmDBTxt(.Tag)
                                .Tab = sReadTextItem(dr, gsPRMSET_COL_TAB)
                                sTmp = sReadTextItem(dr, gsPRMSET_COL_DATASOURCE)
                                'Debug.Print(.Label)
                                Select Case sTmp.ToLower.Trim
                                    Case "db", "database"
                                        .DataSource = eDataSource.DB
                                        .DBName = sReadTextItem(dr, gsPRMSET_COL_DBNAME)
                                        .DBTableName = sReadTextItem(dr, gsPRMSET_COL_DBTABLENAME)
                                        .DBRowName = sReadTextItem(dr, gsPRMSET_COL_DBROWNAME)
                                        .DBColumnName = sReadTextItem(dr, gsPRMSET_COL_DBCOLUMNNAME)
                                        subCheckCbos(TabCfgs(nTab).ItemCfg(nIndex).DBColumnName, TabCfgs(nTab))
                                    Case "xml"
                                        .DataSource = eDataSource.XML
                                        .DBName = sReadTextItem(dr, gsPRMSET_COL_DBNAME)
                                        .DBTableName = sReadTextItem(dr, gsPRMSET_COL_DBTABLENAME)
                                        .DBRowName = sReadTextItem(dr, gsPRMSET_COL_DBROWNAME)
                                        .DBColumnName = sReadTextItem(dr, gsPRMSET_COL_DBCOLUMNNAME)
                                        subCheckCbos(TabCfgs(nTab).ItemCfg(nIndex).DBColumnName, TabCfgs(nTab))
                                    Case "robot", "program", "system"
                                        .DataSource = eDataSource.Robot
                                        'Use the full name for the redim
                                        ReDim TabCfgs(nTab).ItemCfg(nIndex).ProgName(1)
                                        ReDim TabCfgs(nTab).ItemCfg(nIndex).VarName(1)
                                        TabCfgs(nTab).ItemCfg(nIndex).ProgName(0) = sReadTextItem(dr, gsPRMSET_COL_PROGNAME1)
                                        TabCfgs(nTab).ItemCfg(nIndex).ProgName(1) = sReadTextItem(dr, gsPRMSET_COL_PROGNAME2)
                                        TabCfgs(nTab).ItemCfg(nIndex).VarName(0) = sReadTextItem(dr, gsPRMSET_COL_VARNAME1)
                                        TabCfgs(nTab).ItemCfg(nIndex).VarName(1) = sReadTextItem(dr, gsPRMSET_COL_VARNAME2)
                                        subCheckCbos(TabCfgs(nTab).ItemCfg(nIndex).ProgName(0), TabCfgs(nTab))
                                        subCheckCbos(TabCfgs(nTab).ItemCfg(nIndex).ProgName(1), TabCfgs(nTab))
                                        subCheckCbos(TabCfgs(nTab).ItemCfg(nIndex).VarName(0), TabCfgs(nTab))
                                        subCheckCbos(TabCfgs(nTab).ItemCfg(nIndex).VarName(1), TabCfgs(nTab))
                                    Case "plc"
                                        .DataSource = eDataSource.PLC
                                        .PLCTag = sReadTextItem(dr, gsPRMSET_COL_PLCTAG)
                                        subCheckCbos(TabCfgs(nTab).ItemCfg(nIndex).PLCTag, TabCfgs(nTab))
                                    Case "reg", "registry"
                                        .DataSource = eDataSource.Registry
                                        .RegPath = sReadTextItem(dr, gsPRMSET_COL_REGPATH)
                                        .RegKey = sReadTextItem(dr, gsPRMSET_COL_REGKEY)
                                    Case Else
                                        .DataSource = eDataSource.none
                                End Select
                                sTmp = sReadTextItem(dr, gsPRMSET_COL_DATATYPE)
                                Select Case Trim(sTmp.ToLower.Trim)
                                    Case "int", "integer"
                                        .DataType = GetType(Integer)
                                    Case "real", "float", "decimal"
                                        .DataType = GetType(Double)
                                    Case "single" 'RJO 09/30/11
                                        .DataType = GetType(Single)
                                    Case "string", "text"
                                        .DataType = GetType(String)
                                    Case "boolean", "bit"
                                        .DataType = GetType(Boolean)
                                    Case Else
                                        .DataType = Nothing
                                End Select
                                .bEdit = bReadBooleanItem(dr, gsPRMSET_COL_EDIT)
                                .Min = bReadFloatItem(dr, gsPRMSET_COL_MIN)
                                .Max = bReadFloatItem(dr, gsPRMSET_COL_MAX)
                                .Scale = bReadFloatItem(dr, gsPRMSET_COL_SCALE)
                                If .Scale = 0.0 Then
                                    .Scale = 1.0
                                End If
                                .Precision = bReadIntegerItem(dr, gsPRMSET_COL_PRECISION)
                                .bCbo = bReadBooleanItem(dr, gsPRMSET_COL_USECBO)
                                '10/1/12  BTK Changed to use semicolon to match the xml file.
                                .CustomCBO = Split(sReadTextItem(dr, gsPRMSET_COL_CUSTOMCBO), ";")
                                .bPainterOnly = bReadBooleanItem(dr, gsPRMSET_COL_PAINTER_ONLY)
                                .CustomColors = Split(sReadTextItem(dr, gsPRMSET_COL_CUSTOMCOLORS), ";")
                                .ColorRange = Split(sReadTextItem(dr, gsPRMSET_COL_COLORRANGE), ";")
                                .Bitmask = bReadIntegerItem(dr, gsPRMSET_COL_BITMASK)
                                If .bCbo Then
                                    'Make a pop-up menu
                                    .mnuPopUp = New ContextMenuStrip
                                    .mnuPopUp.Name = msMNUNAME1 & nTab.ToString & msMNUNAME2 & nIndex.ToString
                                    AddHandler .mnuPopUp.Opening, AddressOf mnu_Opening
                                    Dim nMnuItem As Integer = 0
                                    Do While nMnuItem <= (.CustomCBO.GetUpperBound(0) - 1)
                                        Dim mnuItemTmp As New ToolStripMenuItem
                                        mnuItemTmp.Name = msMNUNAME1 & nTab.ToString & msMNUNAME2 & nIndex.ToString & msMNUNAME3 & nMnuItem.ToString
                                        .CustomCBO(nMnuItem) = sGetResTxtFrmDBTxt(.CustomCBO(nMnuItem))
                                        mnuItemTmp.Text = .CustomCBO(nMnuItem)
                                        mnuItemTmp.Tag = .CustomCBO(nMnuItem)
                                        AddHandler mnuItemTmp.Click, AddressOf mnu_Click
                                        .mnuPopUp.Items.Add(mnuItemTmp)
                                        nMnuItem += 2
                                    Loop
                                End If
                                If .DataType Is GetType(Boolean) Then
                                    .bCbo = True
                                End If
                                Try
                                    .formula = CType(bReadIntegerItem(dr, gsPRMSET_COL_FORMULA), eFormula)
                                Catch ex As Exception
                                    .formula = eFormula.None
                                End Try

                                If TabCfgs(nTab).oGraphs IsNot Nothing Then
                                    For nGraph As Integer = TabCfgs(nTab).oGraphs.GetLowerBound(0) To TabCfgs(nTab).oGraphs.GetUpperBound(0)
                                        For nItem As Integer = TabCfgs(nTab).oGraphs(nGraph).ItemNames.GetLowerBound(0) To TabCfgs(nTab).oGraphs(nGraph).ItemNames.GetUpperBound(0)
                                            If .Tag = TabCfgs(nTab).oGraphs(nGraph).ItemNames(nItem) Then
                                                TabCfgs(nTab).oGraphs(nGraph).ItemNames(nItem) = .Label
                                                TabCfgs(nTab).oGraphs(nGraph).ItemNumbers(nItem) = nIndex
                                                TabCfgs(nTab).oGraphs(nGraph).ItemCfg(nItem) = TabCfgs(nTab).ItemCfg(nIndex)
                                            End If
                                        Next
                                    Next
                                End If
                            End With
                            Exit For
                        End If
                    Next 'nTab
                End If 'enabled
            End If
        Next 'drI As DataRow In dtItemTable.Rows
        'Development debug
        If InIDE Then
            For Each TabCfg As tTabCfg In TabCfgs
                With TabCfg
                    Dim sDbg As String = TabCfg.ToString & ": " & .TabName & ", " & .TabTag
                    If .bUseZone Then
                        sDbg = sDbg & ", UseZone"
                    End If
                    If .bUseArm Then
                        sDbg = sDbg & ", UseArm"
                    Else
                        If .bUseController Then
                            sDbg = sDbg & ", UseCntr"
                        End If
                    End If
                    If .bUseValve Then
                        sDbg = sDbg & ", bUseValve"
                    Else
                        If .bUseColor Then
                            sDbg = sDbg & ", bUseColor"
                        End If
                    End If
                    If .oGraphs IsNot Nothing Then
                        Debug.Print(sDbg)
                        For Each oGraph As tGraphConfig In .oGraphs
                            Debug.Print("  " & oGraph.sGraphName & " - ")
                            For nItem As Integer = oGraph.ItemNumbers.GetLowerBound(0) To oGraph.ItemNumbers.GetUpperBound(0)
                                Debug.Print("    -" & oGraph.ItemNumbers(nItem).ToString & ": " & oGraph.ItemNames(nItem) & " - " & oGraph.ItemUnits(nItem))
                            Next
                        Next
                    End If
                End With
                If Not (TabCfg.ItemCfg Is Nothing) Then
                    For Each ItemCfg As tItemCfg In TabCfg.ItemCfg
                        With ItemCfg
                            Dim sDbg As String = "  " & .Label & ": "
                            Select Case .DataSource
                                Case eDataSource.DB, eDataSource.XML
                                    sDbg = sDbg & "DB=" & .DBName & ", Table=" & .DBTableName & _
                                    ", Row=" & .DBRowName & ", Col=" & .DBColumnName
                                Case eDataSource.Robot
                                    sDbg = sDbg & "Var=[" & .ProgName(0) & "]" & .VarName(0)
                                    If .ProgName(1) <> String.Empty Then
                                        sDbg = sDbg & ", Var2=[" & .ProgName(1) & "]" & .VarName(1)
                                    End If
                                Case eDataSource.PLC
                                    sDbg = sDbg & "PLCTag=" & .PLCTag
                                Case eDataSource.Registry
                                    sDbg = sDbg & "Regpath=" & .RegPath & ", RegKey=" & .RegKey
                                Case Else
                                    sDbg = sDbg & " - Invalid Datasource"
                            End Select
                            Debug.Print(sDbg)
                            If .DataType Is GetType(Integer) Then
                                sDbg = "   - Integer"
                            ElseIf .DataType Is GetType(Double) Then
                                sDbg = "   - Decimal"
                            ElseIf .DataType Is GetType(Single) Then
                                sDbg = "   - Decimal"
                            ElseIf .DataType Is GetType(String) Then
                                sDbg = "   - Text"
                            ElseIf .DataType Is GetType(Boolean) Then
                                sDbg = "   - Boolean"
                            Else
                                sDbg = "   - Invalid type"
                            End If
                            If .bEdit Then
                                sDbg = sDbg & ", Editable"
                            End If
                            sDbg = sDbg & ", Min=" & .Min & ", Max=" & .Max & ", Scale=" & .Scale & ", Prec=" & .Precision
                            Debug.Print(sDbg)
                            If .bCbo Then
                                'Debug.Print("   - " & .CustomCBO(0))
                            End If
                        End With
                    Next
                End If
            Next
        End If

        Return True
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
        ' 11/18/09  MSW     subInitializeForm - set screen name in title bar
        ' 10/28/13  RJO     subInitializeForm - Changed to handle Param Setup screen special case.
        '********************************************************************************************
        Dim bAbort As Boolean = False
        Try

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

            DataLoaded = False
            mbScreenLoaded = False

            Status = gcsRM.GetString("csINITALIZING")
            Progress = 1

            gpsRM.GetString("psSCREENCAPTION")

            colZones = New clsZones(String.Empty)
            mScreenSetup.LoadZoneBox(cboZone, colZones, False)

            subProcessCommandLine()
            Me.Show()

            subLoadTableList(dtScreenTable)

            'For Each dr As DataRow In dtScreenTable.Rows
            '    Debug.Print("Table: ")
            '    For Each di As DataColumn In dtScreenTable.Columns
            '        Debug.Print("  " & di.ColumnName.ToString & ":  " & dr.Item(di.ColumnName).ToString)
            '    Next
            'Next

            Progress = 5
            'Figure out which screen DB to load
            msScreenName = String.Empty
            msTableName = String.Empty
            Do
                If msScreen = String.Empty Then
                    frmSelectScreen.dgvSelScreen.DataSource = dtScreenTable
                    Dim lRet As DialogResult = frmSelectScreen.ShowDialog()
                    If lRet = Windows.Forms.DialogResult.OK Then
                        msScreen = frmSelectScreen.Screen
                    Else
                        Me.Close()
                        bAbort = True
                        Exit Sub
                    End If
                End If

                For Each dr As DataRow In dtScreenTable.Rows
                    If (msScreen = dr.Item(gsPRMSET_COL_TBLNAME).ToString) Then
                        msScreenName = sGetResTxtFrmDBTxt(dr.Item(gsPRMSET_COL_SCRNNAME).ToString)
                        msTableName = sGetResTxtFrmDBTxt(dr.Item(gsPRMSET_COL_TBLNAME).ToString)
                        msPwdName = dr.Item(gsPRMSET_COL_PWDNAME).ToString
                        msFormIcon = dr.Item(gsPRMSET_COL_ICON).ToString
                    End If
                Next
            Loop Until (msScreenName <> String.Empty) And (msTableName <> String.Empty)
            Me.Text = msScreenName

            'gsChangeLogArea = sTableName ' New password
            'Load the screen config
            If Not (bGetScreenCfg(msScreenName, msTableName, mtTabCfgs)) Then
                MessageBox.Show(gpsRM.GetString("psDBERROR"), gpsRM.GetString("psERROR"), MessageBoxButtons.OK)
                Me.Close()
                bAbort = True
                Exit Sub
            End If

            'init new IPC and new Password 'RJO 03/15/12
            '10/28/13 RJO Changed to handle Param Setup screen special case. 
            oIPC = New Paintworks_IPC.clsInterProcessComm("?")
            moPassword = New clsPWUser(msSCREEN_NAME)
            Progress = 50

            mPrintHtml = New clsPrintHtml(msSCREEN_NAME, True, 30)

            mScreenSetup.InitializeForm(Me)
            ' what ever icon you want to use should be included in project resource file
            Me.Icon = CType(gpsRM.GetObject(msFormIcon), Icon)

            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor


            'init the old password for now 'RJO 03/21/12
            'moPassword = New PWPassword.PasswordObject
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

            subInitFormText()
            'Call once to clean up the table
            'subFormatScreenLayout()
            Progress = 70

            Progress = 98
            subEnableControls(True)

            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound
            If cboZone.Items.Count = 1 Then
                'this selects the zone in cases of just one zone. fires event to call subchangezone
                cboZone.Text = cboZone.Items(0).ToString
            Else
                'statusbar text
                Status(True) = gcsRM.GetString("csSELECT_ZONE")
            End If
            ' refresh lock pic
            DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
            mbScreenLoaded = True


        Catch ex As Exception

        Finally
            If Not bAbort Then
                Progress = 100
            End If
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try


    End Sub
    Private Sub subGetXMLValue(ByRef oItemCfg As tItemCfg, ByRef oCell As DataGridViewCell)
        '********************************************************************************************
        'Description:  Read XML value
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Add XML data support
        '********************************************************************************************
        Dim sXMLFile As String = oItemCfg.DBName
        Dim sXMLTABLE As String = oItemCfg.DBTableName
        Dim sXMLRow As String = oItemCfg.DBRowName
        Dim sXMLCol As String = oItemCfg.DBColumnName

        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oRows As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Dim sVal As String = String.Empty
        '    08/15/13   MSW     PSC remote PC support
        If GetDefaultFilePath(sXMLFilePath, eDir.XML, colZones.ActiveZone.RemotePath, sXMLFile & ".XML") Then
            Try
                oXMLDoc.Load(sXMLFilePath)
                oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                If sXMLRow <> String.Empty Then
                    oRows = oMainNode.SelectNodes("//" & sXMLRow)
                    oNodeList = oRows(0).SelectNodes("//" & sXMLCol)
                Else
                    oNodeList = oMainNode.SelectNodes("//" & sXMLCol)
                End If
                If oNodeList.Count > 0 Then
                    sVal = oNodeList(0).InnerXml
                End If
            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subGetXMLValue", _
                                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If
        Try
            If (oCell.ValueType Is GetType(String)) Then
                oCell.Value = sVal
            ElseIf (oCell.ValueType Is GetType(Integer)) Then
                If IsNumeric(sVal) Then
                    If oItemCfg.Bitmask = 0 Then
                        oCell.Value = CType(sVal, Integer)
                    Else
                        oCell.Value = CType(sVal, Integer) And oItemCfg.Bitmask
                    End If
                Else
                    oCell.Value = 0
                End If
            ElseIf (oCell.ValueType Is GetType(Single)) Then 'RJO 10/12/11 - Added
                If IsNumeric(sVal) Then
                    Select Case oItemCfg.formula
                        Case Else
                            oCell.Value = CType(sVal, Single) * oItemCfg.Scale
                    End Select
                Else
                    oCell.Value = 0
                End If
            ElseIf (oCell.ValueType Is GetType(Double)) Then
                If IsNumeric(sVal) Then
                    Select Case oItemCfg.formula
                        Case Else
                            oCell.Value = CType(sVal, Double) * oItemCfg.Scale
                    End Select
                Else
                    oCell.Value = 0
                End If
            Else
                oCell.Value = 0
            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Dim x As New CheckedListBox
        End Try
    End Sub
    Private Sub subGetDBValue(ByRef oItemCfg As tItemCfg, ByRef oCell As DataGridViewCell)
        'SQL Server settings aren't used any more.  If you need them, uncomment
        'This routine, subSetDBValue, and the links below will be needed in the project file
        '<Reference Include="Interop.JRO, Version=2.6.0.0, Culture=neutral, processorArchitecture=MSIL">
        '  <SpecificVersion>False</SpecificVersion>
        '  <HintPath>..\..\Vbapps\Interop.JRO.dll</HintPath>
        '</Reference>
        '<Reference Include="Microsoft.SqlServer.ConnectionInfo, Version=9.0.242.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91, processorArchitecture=MSIL" />
        '<Reference Include="Microsoft.SqlServer.Smo, Version=9.0.242.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91, processorArchitecture=MSIL" />
        '<Reference Include="Microsoft.SqlServer.WmiEnum, Version=9.0.242.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91, processorArchitecture=MSIL" />
        ' <Compile Include="..\Common.NET\clsDBAccess.vb">
        '  <Link>clsDBAccess.vb</Link>
        '</Compile>
        '<Compile Include="..\Common.NET\clsSQLAccess.vb">
        '  <Link>clsSQLAccess.vb</Link>
        '</Compile>

        ''********************************************************************************************
        ''Description:  Read DB value
        ''
        ''Parameters: none
        ''Returns:    none
        ''
        ''Modification history:
        ''
        '' Date      By      Reason
        ''********************************************************************************************

        'Dim sTable As String = "[" & oItemCfg.DBTableName & "]"
        'Try

        '    'See if the table is already loaded
        '    If mColDSData Is Nothing Then
        '        mColDSData = New Collection(Of DataSet)
        '    End If
        '    Dim oDT As DataTable = Nothing
        '    For Each oDStmp As DataSet In mColDSData
        '        If oDStmp.Tables.Contains(sTable) Then
        '            oDT = oDStmp.Tables(sTable)
        '        End If
        '    Next

        '    If oDT Is Nothing Then
        '        'Not loaded yet
        '        Dim oDB As clsSQLAccess = New clsSQLAccess
        '        With oDB
        '            Dim ds As New DataSet
        '            .Zone = colZones.ActiveZone
        '            .DBFileName = oItemCfg.DBName
        '            .DBTableName = sTable
        '            .SQLString = "SELECT * FROM " & sTable
        '            ds = .GetDataSet
        '            If ds.Tables.Contains(sTable) Then
        '                oDT = ds.Tables(sTable)
        '                mColDSData.Add(ds) 'save for later
        '            End If
        '        End With
        '        oDB.Close()
        '    End If
        '    Dim sVal As String = String.Empty
        '    If Not (oDT Is Nothing) Then
        '        Dim dr As DataRow = Nothing
        '        If oItemCfg.DBRowName = String.Empty Then
        '            dr = oDT.Rows(0)
        '        ElseIf IsNumeric(oItemCfg.DBRowName) Then
        '            Dim nRow As Integer = CInt(oItemCfg.DBRowName)
        '            dr = oDT.Rows(nRow)
        '        Else
        '            Dim dra() As DataRow = oDT.Select(oItemCfg.DBRowName)
        '            dr = dra(0)
        '        End If
        '        If Not (dr Is Nothing) Then
        '            sVal = dr.Item(oItemCfg.DBColumnName).ToString
        '        End If
        '    End If

        '    If (oCell.ValueType Is GetType(String)) Then
        '        oCell.Value = sVal
        '    ElseIf (oCell.ValueType Is GetType(Integer)) Then
        '        If IsNumeric(sVal) Then
        '            oCell.Value = CType(sVal, Integer)
        '        Else
        '            oCell.Value = 0
        '        End If
        '    ElseIf (oCell.ValueType Is GetType(Single)) Then 'RJO 10/12/11 - Added
        '        If IsNumeric(sVal) Then
        '            Select Case oItemCfg.formula
        '                Case Else
        '                    oCell.Value = CType(sVal, Single) * oItemCfg.Scale
        '            End Select
        '        Else
        '            oCell.Value = 0
        '        End If
        '    ElseIf (oCell.ValueType Is GetType(Double)) Then
        '        If IsNumeric(sVal) Then
        '            Select Case oItemCfg.formula
        '                Case Else
        '                    oCell.Value = CType(sVal, Double) * oItemCfg.Scale
        '            End Select
        '        Else
        '            oCell.Value = 0
        '        End If
        '    Else
        '        oCell.Value = 0
        '    End If
        'Catch ex As Exception
        '    Trace.WriteLine(ex.Message)
        '    Dim x As New CheckedListBox

        'End Try

    End Sub
    Private Sub subGetPLCValue(ByRef oItemCfg As tItemCfg, ByRef oCell As DataGridViewCell, ByRef Arm As clsArm, _
                                    Optional ByRef nColor As Integer = 0, Optional ByRef nValve As Integer = 0, _
                                    Optional ByRef sCustom As String = "", Optional ByRef Zone As clsZone = Nothing)
        '********************************************************************************************
        'Description:  Read PLC value
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/18/10  MSW	    subGetPLCValue - handle no data situation
        ' 11/19/10  MSW     Add string array support
        '********************************************************************************************
        Dim sTag As String = sConvertTags(oItemCfg.PLCTag, Arm, nColor, nValve, sCustom, Zone)
        Dim sData() As String
        If mPLC Is Nothing Then
            mPLC = New clsPLCComm
        End If

        If (Not (Arm Is Nothing)) Then
            If (sTag Is Nothing) Or (Arm.IsOpener And oItemCfg.bPainterOnly) Then
                oCell.Value = gpsRM.GetString("psNA")
                oCell.Tag = Nothing 'Mark it unavailable 
                oCell.ReadOnly = True
                oCell.ContextMenuStrip = Nothing
                Exit Sub
            End If
        End If

        If (sTag Is Nothing) Then
            oCell.Value = gpsRM.GetString("psNA")
            oCell.Tag = Nothing 'Mark it unavailable 
            oCell.ReadOnly = True
            oCell.ContextMenuStrip = Nothing
        Else
            With mPLC
                .ZoneName = colZones.ActiveZone.Name
                .TagName = sTag
                sData = .PLCData
            End With
            'MSW 2/18/10 - handle no data situation
            If sData Is Nothing Then
                ReDim sData(0)
                sData(0) = ""
            End If
            If (oCell.ValueType Is GetType(String)) Then
                If sData.GetUpperBound(0) > 0 Then
                    oItemCfg.ArrayLength = sData.GetUpperBound(0)
                    Dim cData(sData.GetUpperBound(0)) As Char
                    Dim sTmp As String = String.Empty
                    For nIdx As Integer = 0 To sData.GetUpperBound(0)
                        cData(nIdx) = Chr(CType(sData(nIdx), Integer))
                        sTmp = sTmp & cData(nIdx).ToString
                    Next
                    oCell.Value = sTmp
                Else
                    oItemCfg.ArrayLength = 0
                    oCell.Value = sData(0)
                End If
            ElseIf (oCell.ValueType Is GetType(Integer)) Then
                If IsNumeric(sData(0)) Then
                    If oItemCfg.Bitmask = 0 Then
                        oCell.Value = CType(sData(0), Integer)
                    Else
                        oCell.Value = CType(sData(0), Integer) And oItemCfg.Bitmask
                    End If
                Else
                    oCell.Value = 0
                End If
            ElseIf (oCell.ValueType Is GetType(Double)) Then
                If IsNumeric(sData(0)) Then
                    Select Case oItemCfg.formula
                        Case Else
                            oCell.Value = CType(sData(0), Double) * oItemCfg.Scale
                    End Select
                Else
                    oCell.Value = 0
                End If
            Else
                oCell.Value = 0
            End If
        End If
    End Sub
    Private Sub subGetRegistryValue(ByRef oItemCfg As tItemCfg, ByRef oCell As DataGridViewCell)
        '********************************************************************************************
        'Description:  Read Registry value
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

    End Sub
    Private Function sConvertTags(ByRef sTag As String, Optional ByRef Arm As clsArm = Nothing, _
                                  Optional ByVal nColor As Integer = 0, Optional ByVal nValve As Integer = 0, _
                                    Optional ByVal sCustom As String = "", Optional ByRef Zone As clsZone = Nothing) As String
        '********************************************************************************************
        'Description:  replace tokens in variable, PLC, or
        '
        'Parameters: none
        'Returns:    converted tag or variable name, null if it should get disabled
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/20/13  DE      Modified to handle Cultures that use "," in floating point numbers rather 
        '                   than ".".
        '********************************************************************************************
        Dim sTmp As String = sTag

        If Not (Arm Is Nothing) Then
            'Multiversion decoding
            '{#VERx.xx#}var_name;#VERx.xx#var_name ... #VERx.xx##NOT_USED#
            'First version optional, if no version is listed it's a default
            'If the first version is listed, lower versions get this item disabled
            'Assumes versions are in order
            'For a variable that isn't used above a final version, add the version and #NOT_USED#
            If InStr(sTmp, "#VER") > 0 Then
                Dim nCntrVersion As Single = Arm.Controller.Version
                Dim sVersions() As String = Strings.Split(sTmp, ";")
                sTmp = Nothing
                For nVersionIdx As Integer = 0 To sVersions.GetUpperBound(0)
                    If InStr(sVersions(nVersionIdx), "#VER") = 1 Then
                        Dim sVersion As String = sVersions(nVersionIdx).Substring(4, 4)
                        'Dim nVersion As Single = CType(sVersion, Single) 'DE 09/20/13
                        Dim nVersion As Single = Single.Parse(Trim(sVersion), mLanguage.FixedCulture)
                        If nCntrVersion >= nVersion Then
                            sTmp = sVersions(nVersionIdx).Substring(9)
                        End If
                    Else
                        sTmp = sVersions(nVersionIdx) 'Default
                    End If
                Next
                If (InStr(sTmp, "#NOT_USED#") > 0) Or (sTmp Is Nothing) Or sTmp = String.Empty Then
                    Return Nothing
                End If
            End If
            'Option Based
            If (InStr(sTmp, "#2K#") > 0) Then
                Select Case Arm.ColorChangeType
                    Case eColorChangeType.BELL_2K, eColorChangeType.GUN_2K, _
                         eColorChangeType.VERSABELL_2K, eColorChangeType.VERSABELL2_2K, _
                          eColorChangeType.VERSABELL2_2K_1PLUS1, eColorChangeType.VERSABELL2_2K_MULTIRESIN
                        sTmp = sTmp.Replace("#2K#", String.Empty)
                    Case Else
                        Return Nothing
                End Select
            End If

            'Done with the multiversion, on to the combobox tags
            sTmp = sTmp.Replace("#Equip#", Arm.ArmNumber.ToString)
            sTmp = sTmp.Replace("#Arm#", Arm.RobotNumber.ToString)
            sTmp = sTmp.Replace("#Controller#", Arm.Controller.ControllerNumber.ToString)
            Dim bDone As Boolean = False
            Do Until bDone = True
                Dim nStart As Integer = InStr(sTmp, "#Parm")
                If nStart > 0 Then
                    Dim nParm As Integer = CInt(sTmp.Substring(nStart + 4, 1))
                    nParm = nParm + 6 * (Arm.ArmNumber - 1)
                    sTmp = sTmp.Substring(0, nStart - 1) & nParm.ToString & sTmp.Substring(nStart + 6, sTmp.Length - (nStart + 6))
                Else
                    bDone = True
                End If
            Loop
            bDone = False
            Do Until bDone = True
                Dim nStart As Integer = InStr(sTmp, "#TsParm")
                If nStart > 0 Then
                    Dim nParm As Integer = CInt(sTmp.Substring(nStart + 6, 1))
                    nParm = nParm + 4 * (Arm.ArmNumber - 1)
                    sTmp = sTmp.Substring(0, nStart - 1) & nParm.ToString & sTmp.Substring(nStart + 8, sTmp.Length - (nStart + 8))
                Else
                    bDone = True
                End If
            Loop
        End If
        sTmp = sTmp.Replace("#Cust#", sCustom)
        If nValve > 0 Then
            sTmp = sTmp.Replace("#Valve#", nValve.ToString)
        End If
        If nColor > 0 Then
            sTmp = sTmp.Replace("#Color#", nColor.ToString)
        End If
        If Zone IsNot Nothing Then
            sTmp = sTmp.Replace("#Zone#", Zone.ZoneNumber.ToString)
        End If
        Return sTmp
    End Function
    Private Function PrintVal(ByVal bTmp As Boolean) As String
        '********************************************************************************************
        'Description:  Get text for a boolean value
        '
        'Parameters: boolean 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If bTmp Then
            Return gpsRM.GetString("psON")
        Else
            Return gpsRM.GetString("psOFF")
        End If
    End Function
    Private Function PrintVal(ByVal nTmp As Integer) As String
        '********************************************************************************************
        'Description:  Get text for an integer value
        '
        'Parameters: boolean 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return nTmp.ToString
    End Function

    Private Sub subGetIOSignalDetails(ByRef oPoint As Object, ByRef sValue As String)
        '********************************************************************************************
        'Description:  Get details for an I/O point
        '
        'Parameters: boolean 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/30/12  MSW     btnMonitor_Click, subGetSignalDetails - 
        '                   Add error checking for robot IO unassigned or offline
        '********************************************************************************************
        If TypeOf (oPoint) Is FRRobot.FRCAnalogIOSignal Then
            Dim oIOPoint As FRRobot.FRCAnalogIOSignal = DirectCast(oPoint, FRRobot.FRCAnalogIOSignal)
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
                sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
        ElseIf TypeOf (oPoint) Is FRRobot.FRCDigitalIOSignal Then
            Dim oIOPoint As FRRobot.FRCDigitalIOSignal = DirectCast(oPoint, FRRobot.FRCDigitalIOSignal)
            Debug.Print(oIOPoint.IsOffline.ToString)
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
                sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
        ElseIf TypeOf (oPoint) Is FRRobot.FRCFlagSignal Then
            Dim oIOPoint As FRRobot.FRCFlagSignal = DirectCast(oPoint, FRRobot.FRCFlagSignal)
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
                sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
        ElseIf TypeOf (oPoint) Is FRRobot.FRCGroupIOSignal Then
            Dim oIOPoint As FRRobot.FRCGroupIOSignal = DirectCast(oPoint, FRRobot.FRCGroupIOSignal)
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
                sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
        ElseIf TypeOf (oPoint) Is FRRobot.FRCMarkerSignal Then
            Dim oIOPoint As FRRobot.FRCMarkerSignal = DirectCast(oPoint, FRRobot.FRCMarkerSignal)
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
                sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
        ElseIf TypeOf (oPoint) Is FRRobot.FRCPLCIOSignal Then
            Dim oIOPoint As FRRobot.FRCPLCIOSignal = DirectCast(oPoint, FRRobot.FRCPLCIOSignal)
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
                sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
        ElseIf TypeOf (oPoint) Is FRRobot.FRCRobotIOSignal Then
            Dim oIOPoint As FRRobot.FRCRobotIOSignal = DirectCast(oPoint, FRRobot.FRCRobotIOSignal)
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
                sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
        ElseIf TypeOf (oPoint) Is FRRobot.FRCSOPIOSignal Then
            Dim oIOPoint As FRRobot.FRCSOPIOSignal = DirectCast(oPoint, FRRobot.FRCSOPIOSignal)
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
                sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
        ElseIf TypeOf (oPoint) Is FRRobot.FRCTPIOSignal Then
            Dim oIOPoint As FRRobot.FRCTPIOSignal = DirectCast(oPoint, FRRobot.FRCTPIOSignal)
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
                sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
        ElseIf TypeOf (oPoint) Is FRRobot.FRCUOPIOSignal Then
            Dim oIOPoint As FRRobot.FRCUOPIOSignal = DirectCast(oPoint, FRRobot.FRCUOPIOSignal)
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
                sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
        ElseIf TypeOf (oPoint) Is FRRobot.FRCWeldDigitalIOSignal Then
            Dim oIOPoint As FRRobot.FRCWeldDigitalIOSignal = DirectCast(oPoint, FRRobot.FRCWeldDigitalIOSignal)
            If oIOPoint.IsAssigned And Not oIOPoint.IsOffline Then
                sValue = PrintVal(oIOPoint.Value)
            Else
                sValue = "*"
            End If
        End If
        'Case FRRobot.FREIOTypeConstants.frLAInType
        'Case FRRobot.FREIOTypeConstants.frLAOutType
        'Case FRRobot.FREIOTypeConstants.frLDInType
        'Case FRRobot.FREIOTypeConstants.frLDOutType

    End Sub

    Private Sub subGetRobotValue(ByRef oItemCfg As tItemCfg, ByRef oCell As DataGridViewCell, ByRef Arm As clsArm, _
                                  Optional ByVal nColor As Integer = 0, Optional ByVal nValve As Integer = 0, _
                                  Optional ByVal sCustom As String = "", Optional ByVal bNoShortcuts As Boolean = True)
        '********************************************************************************************
        'Description:  Read robot variable value
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/29/11  RJO     Added support for RegNumerics
        '********************************************************************************************
        Dim sVal As String = String.Empty
        Dim sProgram As String = sConvertTags(oItemCfg.ProgName(0), Arm, nColor, nValve, sCustom)
        Dim sVar As String = sConvertTags(oItemCfg.VarName(0), Arm, nColor, nValve, sCustom)
        If (sProgram Is Nothing) Or (sVar Is Nothing) Or (Arm.IsOpener And oItemCfg.bPainterOnly) Then
            oCell.Value = gpsRM.GetString("psNA")
            oCell.Tag = Nothing 'Mark it unavailable 
            oCell.ReadOnly = True
            oCell.ContextMenuStrip = Nothing
        Else
            Select Case oItemCfg.ProgName(0).ToLower
                Case "*system*"
                    If ((oCell.Tag IsNot Nothing) And (bNoShortcuts = False)) Then
                        Dim oVar As FRRobot.FRCVar = DirectCast(oCell.Tag, FRRobot.FRCVar)
                        'oVar.Refresh()
                        If oVar.IsInitialized Then
                            sVal = oVar.Value.ToString
                        Else
                            If (oItemCfg.DataType Is GetType(Boolean)) Then
                                sVal = "False"
                            ElseIf ((oItemCfg.DataType Is GetType(Integer)) Or (oItemCfg.DataType Is GetType(Double))) Then
                                sVal = "-1"
                            Else
                                sVal = "*"
                            End If
                        End If
                    Else
                        Dim oFRCRobot As FRRobot.FRCRobot = Arm.Controller.Robot
                        Dim oVars As FRRobot.FRCVars = oFRCRobot.SysVariables
                        Dim oVar As FRRobot.FRCVar = DirectCast(oVars(sVar), FRRobot.FRCVar)
                        If oVar.IsInitialized Then
                            sVal = oVar.Value.ToString
                        Else
                            If (oItemCfg.DataType Is GetType(Boolean)) Then
                                sVal = "False"
                            ElseIf ((oItemCfg.DataType Is GetType(Integer)) Or (oItemCfg.DataType Is GetType(Double))) Then
                                sVal = "-1"
                            Else
                                sVal = "*"
                            End If
                        End If
                        oCell.Tag = oVar
                    End If
                Case "*numreg*"
                    If ((oCell.Tag IsNot Nothing) And (bNoShortcuts = False)) Then
                        Dim oVar As FRRobot.FRCRegNumeric = DirectCast(oCell.Tag, FRRobot.FRCRegNumeric)
                        sVal = mRegVal.ReadNumReg(oVar)
                    Else
                        Dim oFRCRobot As FRRobot.FRCRobot = Arm.Controller.Robot
                        Dim oVars As FRRobot.FRCVars = oFRCRobot.RegNumerics
                        Dim oVarA As FRRobot.FRCVar = DirectCast(oVars(CType(sVar, Integer)), FRRobot.FRCVar)
                        Dim oVar As FRRobot.FRCRegNumeric = DirectCast(oVarA.Value, FRRobot.FRCRegNumeric)
                        sVal = mRegVal.ReadNumReg(oVar)
                        oCell.Tag = oVar
                    End If
                Case "*strreg*"
                    If ((oCell.Tag IsNot Nothing) And (bNoShortcuts = False)) Then
                        Dim oVar As FRRobot.FRCRegString = DirectCast(oCell.Tag, FRRobot.FRCRegString)
                        sVal = mRegVal.ReadStringReg(oVar)
                    Else
                        Dim oFRCRobot As FRRobot.FRCRobot = Arm.Controller.Robot
                        Dim oVars As FRRobot.FRCVars = oFRCRobot.RegStrings
                        Dim oVarA As FRRobot.FRCVar = DirectCast(oVars(CType(sVar, Integer)), FRRobot.FRCVar)
                        Dim oVar As FRRobot.FRCRegString = DirectCast(oVarA.Value, FRRobot.FRCRegString)
                        sVal = mRegVal.ReadStringReg(oVar)
                        oCell.Tag = oVar
                    End If
                Case "din", "dout", "ain", "aout", "gin", "gout", "rin", "rout"
                    Dim oPoint As Object
                    If ((oCell.Tag IsNot Nothing) And (bNoShortcuts = False)) Then
                        oPoint = oCell.Tag
                    Else
                        Dim oFRCRobot As FRRobot.FRCRobot = Arm.Controller.Robot
                        Dim oAllIO As FRRobot.FRCIOTypes = Arm.Controller.Robot.IOTypes
                        'Dim oIOType As FRRobot.FREIOTypeConstants = FRRobot.FREIOTypeConstants.frDOutType
                        Dim oIOSignals As FRRobot.FRCIOSignals = Nothing
                        Select Case oItemCfg.ProgName(0).ToLower
                            Case "din"
                                Dim oIOType As FRRobot.FRCDigitalIOType = DirectCast(oAllIO(FRRobot.FREIOTypeConstants.frDInType), FRRobot.FRCDigitalIOType)
                                oIOSignals = oIOType.Signals
                            Case "dout"
                                Dim oIOType As FRRobot.FRCDigitalIOType = DirectCast(oAllIO(FRRobot.FREIOTypeConstants.frDOutType), FRRobot.FRCDigitalIOType)
                                oIOSignals = oIOType.Signals
                            Case "ain"
                                Dim oIOType As FRRobot.FRCAnalogIOType = DirectCast(oAllIO(FRRobot.FREIOTypeConstants.frAInType), FRRobot.FRCAnalogIOType)
                                oIOSignals = oIOType.Signals
                            Case "aout"
                                Dim oIOType As FRRobot.FRCAnalogIOType = DirectCast(oAllIO(FRRobot.FREIOTypeConstants.frAOutType), FRRobot.FRCAnalogIOType)
                                oIOSignals = oIOType.Signals
                            Case "gin"
                                Dim oIOType As FRRobot.FRCGroupIOType = DirectCast(oAllIO(FRRobot.FREIOTypeConstants.frGPInType), FRRobot.FRCGroupIOType)
                                oIOSignals = oIOType.Signals
                            Case "gout"
                                Dim oIOType As FRRobot.FRCGroupIOType = DirectCast(oAllIO(FRRobot.FREIOTypeConstants.frGPOutType), FRRobot.FRCGroupIOType)
                                oIOSignals = oIOType.Signals
                            Case "rin"
                                Dim oIOType As FRRobot.FRCRobotIOType = DirectCast(oAllIO(FRRobot.FREIOTypeConstants.frRDInType), FRRobot.FRCRobotIOType)
                                oIOSignals = oIOType.Signals
                            Case "rout"
                                Dim oIOType As FRRobot.FRCRobotIOType = DirectCast(oAllIO(FRRobot.FREIOTypeConstants.frDOutType), FRRobot.FRCRobotIOType)
                                oIOSignals = oIOType.Signals
                        End Select
                        Dim nPoint As Integer = CType(sVar, Integer)
                        oPoint = oIOSignals(nPoint)
                        oCell.Tag = oPoint
                    End If
                    subGetIOSignalDetails(oPoint, sVal)
                Case Else
                    If ((oCell.Tag IsNot Nothing) And (bNoShortcuts = False)) Then
                        Dim oVar As FRRobot.FRCVar = DirectCast(oCell.Tag, FRRobot.FRCVar)
                        'oVar.Refresh()
                        If oVar.IsInitialized Then
                            sVal = oVar.Value.ToString
                        Else
                            If (oItemCfg.DataType Is GetType(Boolean)) Then
                                sVal = "False"
                            ElseIf ((oItemCfg.DataType Is GetType(Integer)) Or (oItemCfg.DataType Is GetType(Double))) Then
                                sVal = "-1"
                            Else
                                sVal = "*"
                            End If
                        End If
                        'Debug.Print(sVal)
                        'Debug.Print(oVar.NoRefresh.ToString)
                        'Debug.Print(oVar.NoUpdate.ToString)
                    Else
                        Dim oList As FRRobot.FRCPrograms = Arm.Controller.Robot.Programs
                        Dim oProgram As FRRobot.FRCProgram = CType(oList.Item(sProgram), FRRobot.FRCProgram)
                        Dim oVar As FRRobot.FRCVar = DirectCast(oProgram.Variables(sVar), FRRobot.FRCVar)
                        If oVar.IsInitialized Then
                            sVal = oVar.Value.ToString
                        Else
                            If (oItemCfg.DataType Is GetType(Boolean)) Then
                                sVal = "False"
                            ElseIf ((oItemCfg.DataType Is GetType(Integer)) Or (oItemCfg.DataType Is GetType(Double))) Then
                                sVal = "-1"
                            Else
                                sVal = "*"
                            End If
                        End If
                        oCell.Tag = oVar
                    End If
                    'Arm.ProgramName = sProgram
                    'Arm.VariableName = sVar
                    'sVal = Arm.VarValue

            End Select

            If sVal = String.Empty Then
                If (oItemCfg.DataType Is GetType(String)) Then
                    oCell.Value = String.Empty
                Else
                    oCell.Value = 0
                End If
            Else
                If (oItemCfg.DataType Is GetType(String)) Then
                    oCell.Value = sVal
                ElseIf (oItemCfg.DataType Is GetType(Integer)) Then
                    If IsNumeric(sVal) Then
                        If oItemCfg.Bitmask = 0 Then
                            oCell.Value = CType(sVal, Integer)
                        Else
                            oCell.Value = CType(sVal, Integer) And oItemCfg.Bitmask
                        End If
                    Else
                        oCell.Value = 0
                    End If
                ElseIf (oItemCfg.DataType Is GetType(Single)) Then
                    If IsNumeric(sVal) Then
                        Select Case oItemCfg.formula
                            Case Else
                                oCell.Value = CType(CType(sVal, Single) * oItemCfg.Scale, Single)
                        End Select
                    Else
                        oCell.Value = 0
                    End If
                ElseIf (oItemCfg.DataType Is GetType(Double)) Then
                    If IsNumeric(sVal) Then

                        Select Case oItemCfg.formula
                            Case Else
                                oCell.Value = CType(sVal, Double) * oItemCfg.Scale
                        End Select
                    Else
                        oCell.Value = 0
                    End If
                ElseIf (oItemCfg.DataType Is GetType(Boolean)) Then
                    Debug.Print(sVal)
                    Select Case sVal.ToLower
                        Case "yes", "true", gpsRM.GetString("psYES").ToLower, gpsRM.GetString("psTRUE").ToLower, "on", gpsRM.GetString("psON").ToLower
                            oCell.Value = True.ToString
                        Case "no", "false", gpsRM.GetString("psNO").ToLower, gpsRM.GetString("psFALSE").ToLower, "of", gpsRM.GetString("psOFF").ToLower
                            oCell.Value = False.ToString
                    End Select
                Else
                    oCell.Value = 0
                End If
            End If
        End If
    End Sub
    Private Sub subGetRobotValue(ByRef oItemCfg As tItemCfg, ByRef sData As String, ByVal oObj As Object, ByRef Arm As clsArm, _
                                    Optional ByVal nColor As Integer = 0, Optional ByVal nValve As Integer = 0, _
                                    Optional ByVal sCustom As String = "")
        '********************************************************************************************
        'Description:  Read robot variable value
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/29/11  RJO     Added support for RegNumerics
        '********************************************************************************************
        Dim sProgram As String = sConvertTags(oItemCfg.ProgName(0), Arm, nColor, nValve, sCustom)
        Dim sVar As String = sConvertTags(oItemCfg.VarName(0), Arm, nColor, nValve, sCustom)
        If (sProgram Is Nothing) Or (Arm.IsOpener And oItemCfg.bPainterOnly) Then
            sData = gpsRM.GetString("psNA")
        Else
            Select Case oItemCfg.ProgName(0).ToLower
                Case "*system*"
                    If oObj IsNot Nothing Then
                        Dim oVar As FRRobot.FRCVar = DirectCast(oObj, FRRobot.FRCVar)
                        'oVar.Refresh()
                        If oVar.IsInitialized Then
                            sData = oVar.Value.ToString
                        Else
                            If (oItemCfg.DataType Is GetType(Boolean)) Then
                                sData = "False"
                            ElseIf ((oItemCfg.DataType Is GetType(Integer)) Or (oItemCfg.DataType Is GetType(Double))) Then
                                sData = "-1"
                            Else
                                sData = "*"
                            End If
                        End If
                    Else
                        Dim oFRCRobot As FRRobot.FRCRobot = Arm.Controller.Robot
                        Dim oVars As FRRobot.FRCVars = oFRCRobot.SysVariables
                        Dim oVar As FRRobot.FRCVar = DirectCast(oVars(sVar), FRRobot.FRCVar)
                        If oVar.IsInitialized Then
                            sData = oVar.Value.ToString
                        Else
                            If (oItemCfg.DataType Is GetType(Boolean)) Then
                                sData = "False"
                            ElseIf ((oItemCfg.DataType Is GetType(Integer)) Or (oItemCfg.DataType Is GetType(Double))) Then
                                sData = "-1"
                            Else
                                sData = "*"
                            End If
                        End If
                        oObj = oVar
                    End If
                Case "*numreg*"
                    If oObj IsNot Nothing Then
                        Dim oReg As FRRobot.FRCRegNumeric = DirectCast(oObj, FRRobot.FRCRegNumeric)
                        sData = mRegVal.ReadNumReg(oReg)
                    Else
                        Dim oFRCRobot As FRRobot.FRCRobot = Arm.Controller.Robot
                        Dim oVars As FRRobot.FRCVars = oFRCRobot.RegNumerics
                        Dim oVarA As FRRobot.FRCVar = DirectCast(oVars(CType(sVar, Integer)), FRRobot.FRCVar)
                        Dim oVar As FRRobot.FRCRegNumeric = DirectCast(oVarA.Value, FRRobot.FRCRegNumeric)
                        sData = mRegVal.ReadNumReg(oVar)
                        oObj = oVar
                    End If
                Case "*strreg*"
                    If oObj IsNot Nothing Then
                        Dim oReg As FRRobot.FRCRegString = DirectCast(oObj, FRRobot.FRCRegString)
                        sData = mRegVal.ReadStringReg(oReg)
                    Else
                        Dim oFRCRobot As FRRobot.FRCRobot = Arm.Controller.Robot
                        Dim oVars As FRRobot.FRCVars = oFRCRobot.RegStrings
                        Dim oVarA As FRRobot.FRCVar = DirectCast(oVars(CType(sVar, Integer)), FRRobot.FRCVar)
                        Dim oVar As FRRobot.FRCRegString = DirectCast(oVarA.Value, FRRobot.FRCRegString)
                        sData = mRegVal.ReadStringReg(oVar)
                        oObj = oVar
                    End If
                Case "din", "dout", "ain", "aout", "gin", "gout", "rin", "rout"
                    Dim oPoint As Object
                    If ((oObj IsNot Nothing)) Then
                        oPoint = oObj
                    Else
                        Dim oFRCRobot As FRRobot.FRCRobot = Arm.Controller.Robot
                        Dim oAllIO As FRRobot.FRCIOTypes = Arm.Controller.Robot.IOTypes
                        'Dim oIOType As FRRobot.FREIOTypeConstants = FRRobot.FREIOTypeConstants.frDOutType
                        Dim oIOSignals As FRRobot.FRCIOSignals = Nothing
                        Select Case oItemCfg.ProgName(0).ToLower
                            Case "din"
                                Dim oIOType As FRRobot.FRCDigitalIOType = DirectCast(oAllIO(FRRobot.FREIOTypeConstants.frDInType), FRRobot.FRCDigitalIOType)
                                oIOSignals = oIOType.Signals
                            Case "dout"
                                Dim oIOType As FRRobot.FRCDigitalIOType = DirectCast(oAllIO(FRRobot.FREIOTypeConstants.frDOutType), FRRobot.FRCDigitalIOType)
                                oIOSignals = oIOType.Signals
                            Case "ain"
                                Dim oIOType As FRRobot.FRCAnalogIOType = DirectCast(oAllIO(FRRobot.FREIOTypeConstants.frAInType), FRRobot.FRCAnalogIOType)
                                oIOSignals = oIOType.Signals
                            Case "aout"
                                Dim oIOType As FRRobot.FRCAnalogIOType = DirectCast(oAllIO(FRRobot.FREIOTypeConstants.frAOutType), FRRobot.FRCAnalogIOType)
                                oIOSignals = oIOType.Signals
                            Case "gin"
                                Dim oIOType As FRRobot.FRCGroupIOType = DirectCast(oAllIO(FRRobot.FREIOTypeConstants.frGPInType), FRRobot.FRCGroupIOType)
                                oIOSignals = oIOType.Signals
                            Case "gout"
                                Dim oIOType As FRRobot.FRCGroupIOType = DirectCast(oAllIO(FRRobot.FREIOTypeConstants.frGPOutType), FRRobot.FRCGroupIOType)
                                oIOSignals = oIOType.Signals
                            Case "rin"
                                Dim oIOType As FRRobot.FRCRobotIOType = DirectCast(oAllIO(FRRobot.FREIOTypeConstants.frRDInType), FRRobot.FRCRobotIOType)
                                oIOSignals = oIOType.Signals
                            Case "rout"
                                Dim oIOType As FRRobot.FRCRobotIOType = DirectCast(oAllIO(FRRobot.FREIOTypeConstants.frDOutType), FRRobot.FRCRobotIOType)
                                oIOSignals = oIOType.Signals
                        End Select
                        Dim nPoint As Integer = CType(sVar, Integer)
                        oPoint = oIOSignals(nPoint)
                        oObj = oPoint
                    End If
                    subGetIOSignalDetails(oPoint, sData)

                Case Else
                    If oObj IsNot Nothing Then
                        Dim oVar As FRRobot.FRCVar = DirectCast(oObj, FRRobot.FRCVar)
                        If oVar.IsInitialized Then
                            sData = oVar.Value.ToString
                        Else
                            If (oItemCfg.DataType Is GetType(Boolean)) Then
                                sData = "False"
                            ElseIf ((oItemCfg.DataType Is GetType(Integer)) Or (oItemCfg.DataType Is GetType(Double))) Then
                                sData = "-1"
                            Else
                                sData = "*"
                            End If
                        End If
                    Else
                        Dim oList As FRRobot.FRCPrograms = Arm.Controller.Robot.Programs
                        Dim oProgram As FRRobot.FRCProgram = CType(oList.Item(sProgram), FRRobot.FRCProgram)
                        Dim oVar As FRRobot.FRCVar = DirectCast(oProgram.Variables(sVar), FRRobot.FRCVar)
                        If oVar.IsInitialized Then
                            sData = oVar.Value.ToString
                        Else
                            If (oItemCfg.DataType Is GetType(Boolean)) Then
                                sData = "False"
                            ElseIf ((oItemCfg.DataType Is GetType(Integer)) Or (oItemCfg.DataType Is GetType(Double))) Then
                                sData = "-1"
                            Else
                                sData = "*"
                            End If
                        End If
                        oObj = oVar

                    End If
            End Select

            If IsNumeric(sData) Then
                If oItemCfg.Bitmask <> 0 Then
                    sData = (CType(sData, Integer) And oItemCfg.Bitmask).ToString
                End If
                Select Case oItemCfg.formula
                    Case Else
                        sData = CType(CType(sData, Single) * oItemCfg.Scale, Single).ToString
                End Select
            End If

        End If
    End Sub
    Private Sub subClearMultiView()
        '********************************************************************************************
        'Description:  clear multi-view screen
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mnuAutoRefresh.Checked = False
        btnRefresh.Image = DirectCast(gpsRM.GetObject("Refresh", mLanguage.FixedCulture), Image)
        tmrRefresh.Enabled = False
        If bAskForSave() Then
            mdgvData.ColumnCount = 1
            For nRow As Integer = 0 To mdgvData.RowCount - 1
                mdgvData.Rows(nRow).Cells(0).Value = String.Empty
            Next
            EditsMade = False
            DataLoaded = False
            If mtTabCfgs(mnTab).bScatteredAccess Then
                subClearSARequests()
            End If
        End If
    End Sub

    Friend Property ScatteredAccessEnableArm(ByVal Index As Integer) As Boolean
        '********************************************************************************************
        'Description:  set bits in an array to determine which robots to read scattered access data from
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/21/13  MSW     Support skipping robot numbers
        '********************************************************************************************
        Get

            For nArm As Integer = 0 To colArms.Count - 1
                If colArms(nArm).RobotNumber = Index Then
                    Return mbSAEnableArm(nArm)
                End If
            Next
            Return False
        End Get
        Set(ByVal value As Boolean)
            For nArm As Integer = 0 To colArms.Count - 1
                If colArms(nArm).RobotNumber = Index Then
                    mbSAEnableArm(nArm) = value
                End If
            Next
        End Set
    End Property
    Friend Property ScatteredAccessEnableController(ByVal Index As Integer) As Boolean
        '********************************************************************************************
        'Description:  get or set scattered access enable for all arms on a control
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/21/13  MSW     Support skipping robot numbers
        '********************************************************************************************
        Get
            'Return true if any arm is enabled
            For nArm As Integer = 0 To colArms.Count - 1
                If (colArms(nArm).Controller.ControllerNumber = Index) Then
                    If mbSAEnableArm(nArm) Then
                        Return True
                    End If
                End If
            Next
            Return False
        End Get
        Set(ByVal value As Boolean)
            'set all the arms on the controller
            For nArm As Integer = 0 To colArms.Count - 1
                If colArms(nArm).Controller.ControllerNumber = Index Then
                    mbSAEnableArm(nArm) = value
                End If
            Next
        End Set
    End Property
    Friend ReadOnly Property ScatteredAccessEnableString() As String
        '********************************************************************************************
        'Description:  get or set scattered access enable for all arms on a control
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/21/13  MSW     Support skipping robot numbers
        '********************************************************************************************
        Get
            Dim sTmp As String = String.Empty
            For nArm As Integer = 0 To colArms.Count - 1
                If mbSAEnableArm(nArm) Then
                    sTmp = sTmp & colArms(nArm).Name & ";"
                End If
            Next
            Return sTmp
        End Get
    End Property
    Private Sub subClearSARequests()
        '********************************************************************************************
        'Description:  update request to scattered access manager
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            For nItem As Integer = mbSAEnableArm.GetLowerBound(0) To mbSAEnableArm.GetUpperBound(0)
                mbSAEnableArm(nItem) = False
            Next
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subUpdateSARequests", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
        subUpdateSARequests()
    End Sub

    Private Sub subUpdateSARequests()
        '********************************************************************************************
        'Description:  update request to scattered access manager
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If mbUSEROBOTS Then
                Dim sTmpEnable As String = ScatteredAccessEnableString
                If msSAEnableString <> sTmpEnable Then
                    'set enables
                    msSAEnableString = sTmpEnable
                    Dim sMessage(1) As String
                    If sTmpEnable = String.Empty Then
                        sMessage(0) = msSCREEN_NAME & ";OFF"
                        sMessage(1) = "ALL"
                    Else
                        sMessage(0) = msSCREEN_NAME & ";SET"
                        sMessage(1) = msSAEnableString
                    End If
                    oIPC.WriteControlMsg(gs_COM_ID_SA, sMessage)
                End If
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subUpdateSARequests", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Private Function bLoadData(Optional ByVal bReload As Boolean = False, Optional ByVal nColumn As Integer = -1, _
                               Optional ByVal bOverWrite As Boolean = True) As Boolean
        '********************************************************************************************
        'Description:  Data Load Routine
        '
        'Parameters: reload - use parameters in table instead of screen cbos
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/23/10  MSW     Fix floating point formatting so 0 doesn't show up blank
        ' 11/08/11  MSW     add some handling for a shutdown in the middle of a load (mbAbortForShutdown)
        ' 12/14/11  RJO     Added support for Boolean values with custom combos
        ' 03/28/12  MSW     Add XML data support
        ' 10/10/13  MSW     Make sure REALs show 0s on the right side of the decimal 
        '                   point if they're set up that way.
        ' 03/22/14  MAD     Added a fix to make sure the context menus show up on every tab.
        '********************************************************************************************

        Dim bSomethingSkipped As Boolean = False
        'give whatever called us time to repaint
        'System.Windows.Forms.Application.DoEvents()
        'If mbAbortForShutdown Then
        '    Exit Function
        'End If
        Dim bReturnVal As Boolean = True
        Try
            mColDSData = Nothing
            '            Progress = 0
            'Flexible requirements on this screen, first see if we have all the choices we need to load
            Dim bLoad As Boolean = True
            If Not (bReload) Then
                If mtTabCfgs(mnTab).bUseArm Or mtTabCfgs(mnTab).bUseController Then
                    If cboRobot.Text = String.Empty Then
                        bLoad = False
                    End If
                End If
                If mtTabCfgs(mnTab).bUseValve Or mtTabCfgs(mnTab).bUseColor Then
                    If cboColor.Text = String.Empty Then
                        bLoad = False
                    End If
                End If
                If mtTabCfgs(mnTab).bUseCustomParm Then
                    If cboParm.Text = String.Empty Then
                        bLoad = False
                    End If
                End If
            End If

            If bLoad Or bReload Then

                'Status = gcsRM.GetString("csLOADINGDATA")

                'Progress = 20


                Dim bArmOK As Boolean = True
                If Not (bReload) Then
                    If nColumn = -1 Then 'no value passed in, calculate it
                        If btnMultiView.Checked Then
                            'We don't actually delete the 1st column, so use dataloaded property to see if it's empty
                            If DataLoaded Then
                                mdgvData.ColumnCount += 1
                                nColumn = mdgvData.ColumnCount - 1
                            Else
                                nColumn = 0
                            End If
                        Else
                            nColumn = 0
                        End If
                    End If                    'Header cells
                    For nRow As Integer = 0 To mnCUSTOMPARM_ROW
                        Dim oCell As DataGridViewCell = DirectCast(mdgvData.Rows(nRow).Cells.Item(nColumn), DataGridViewCell)
                        oCell.ValueType = GetType(String)
                        oCell.ReadOnly = True
                    Next
                    If mtTabCfgs(mnTab).bUseZone Then
                        mdgvData.Rows(mnZONE_ROW).Cells.Item(nColumn).Value = msOldZone
                    Else
                        mdgvData.Rows(mnZONE_ROW).Cells.Item(nColumn).Value = String.Empty
                    End If
                    If mtTabCfgs(mnTab).bUseArm Then
                        mdgvData.Rows(mnCONTROLLER_ROW).Cells.Item(nColumn).Value = msOldController
                        mdgvData.Rows(mnROBOT_ROW).Cells.Item(nColumn).Value = msOldRobot
                    ElseIf mtTabCfgs(mnTab).bUseController Then
                        mdgvData.Rows(mnCONTROLLER_ROW).Cells.Item(nColumn).Value = msOldController
                        mdgvData.Rows(mnROBOT_ROW).Cells.Item(nColumn).Value = mRobot.Name
                    Else
                        mdgvData.Rows(mnCONTROLLER_ROW).Cells.Item(nColumn).Value = String.Empty
                        mdgvData.Rows(mnROBOT_ROW).Cells.Item(nColumn).Value = String.Empty
                    End If
                    If mtTabCfgs(mnTab).bUseValve Then
                        mdgvData.Rows(mnCOLOR_ROW).Cells.Item(nColumn).Value = msOldColor
                        mdgvData.Rows(mnVALVE_ROW).Cells.Item(nColumn).Value = mnColorIndex.ToString
                        mdgvData.Rows(mnCOLOR_ROW).Cells.Item(nColumn).Tag = 0
                    ElseIf mtTabCfgs(mnTab).bUseColor Then
                        mdgvData.Rows(mnCOLOR_ROW).Cells.Item(nColumn).Value = msOldColor
                        mdgvData.Rows(mnVALVE_ROW).Cells.Item(nColumn).Value = String.Empty
                        mdgvData.Rows(mnCOLOR_ROW).Cells.Item(nColumn).Tag = mnColorIndex
                    Else
                        mdgvData.Rows(mnCOLOR_ROW).Cells.Item(nColumn).Value = String.Empty
                        mdgvData.Rows(mnVALVE_ROW).Cells.Item(nColumn).Value = String.Empty
                        mdgvData.Rows(mnCOLOR_ROW).Cells.Item(nColumn).Tag = 0
                    End If
                    If mtTabCfgs(mnTab).bUseCustomParm Then
                        mdgvData.Rows(mnCUSTOMPARM_ROW).Cells.Item(nColumn).Value = msParmTag
                    Else
                        mdgvData.Rows(mnCUSTOMPARM_ROW).Cells.Item(nColumn).Value = String.Empty
                    End If
                    If moColParam Is Nothing Then
                        ReDim Preserve moColParam(mdgvData.Columns.Count - 1)
                    Else
                        If moColParam.GetUpperBound(0) < (mdgvData.Columns.Count - 1) Then
                            ReDim Preserve moColParam(mdgvData.Columns.Count - 1)
                        End If
                    End If
                    Dim sTmpStr As String = String.Empty
                    If mdgvData.Rows(mnCOLOR_ROW).Cells.Item(nColumn).Tag IsNot Nothing Then
                        sTmpStr = mdgvData.Rows(mnCOLOR_ROW).Cells.Item(nColumn).Tag.ToString
                    End If
                    If IsNumeric(sTmpStr) Then
                        moColParam(nColumn).nColor = CInt(sTmpStr)
                    Else
                        moColParam(nColumn).nColor = 0
                    End If
                    If mdgvData.Rows(mnVALVE_ROW).Cells.Item(nColumn).Value IsNot Nothing Then
                        sTmpStr = mdgvData.Rows(mnVALVE_ROW).Cells.Item(nColumn).Value.ToString
                    End If
                    If IsNumeric(sTmpStr) Then
                        moColParam(nColumn).nValve = CInt(sTmpStr)
                    Else
                        moColParam(nColumn).nValve = 0
                    End If
                    moColParam(nColumn).sColor = String.Empty
                    If mdgvData.Rows(mnCOLOR_ROW).Cells.Item(nColumn).Value IsNot Nothing Then
                        moColParam(nColumn).sColor = mdgvData.Rows(mnCOLOR_ROW).Cells.Item(nColumn).Value.ToString
                    End If
                    moColParam(nColumn).sCustom = String.Empty
                    If mdgvData.Rows(mnCUSTOMPARM_ROW).Cells.Item(nColumn).Value IsNot Nothing Then
                        moColParam(nColumn).sCustom = mdgvData.Rows(mnCUSTOMPARM_ROW).Cells.Item(nColumn).Value.ToString
                    End If
                    moColParam(nColumn).sZone = String.Empty

                    If mdgvData.Rows(mnZONE_ROW).Cells.Item(nColumn).Value IsNot Nothing Then
                        moColParam(nColumn).sZone = mdgvData.Rows(mnZONE_ROW).Cells.Item(nColumn).Value.ToString
                        moColParam(nColumn).oZone = colZones(moColParam(nColumn).sZone)
                    Else
                        moColParam(nColumn).sZone = colZones.CurrentZone
                        moColParam(nColumn).oZone = colZones.ActiveZone
                    End If
                    moColParam(nColumn).sDevice = moColParam(nColumn).sZone
                    If mtTabCfgs(mnTab).bUseArm Then
                        If mdgvData.Rows(mnROBOT_ROW).Cells.Item(nColumn).Value IsNot Nothing Then
                            moColParam(nColumn).sDevice = mdgvData.Rows(mnROBOT_ROW).Cells.Item(nColumn).Value.ToString
                            moColParam(nColumn).oArm = colArms.Item(moColParam(nColumn).sDevice)
                            bArmOK = moColParam(nColumn).oArm.IsOnLine
                            If mtTabCfgs(mnTab).bScatteredAccess Then
                                ScatteredAccessEnableArm(moColParam(nColumn).oArm.RobotNumber) = True
                            End If
                        End If
                    ElseIf mtTabCfgs(mnTab).bUseController Then
                        If mdgvData.Rows(mnCONTROLLER_ROW).Cells.Item(nColumn).Value IsNot Nothing Then
                            moColParam(nColumn).sDevice = mdgvData.Rows(mnCONTROLLER_ROW).Cells.Item(nColumn).Value.ToString
                            moColParam(nColumn).oArm = colArms.Item(colControllers.Item(moColParam(nColumn).sDevice).Arms(0).Name)
                            bArmOK = moColParam(nColumn).oArm.IsOnLine
                            If mtTabCfgs(mnTab).bScatteredAccess Then
                                ScatteredAccessEnableController(colControllers.Item(moColParam(nColumn).sDevice).ControllerNumber) = True
                            End If
                        End If
                    End If
                    If mtTabCfgs(mnTab).bScatteredAccess Then
                        subUpdateSARequests()
                    End If
                Else
                    If mtTabCfgs(mnTab).bUseArm OrElse mtTabCfgs(mnTab).bUseController Then
                        If moColParam(nColumn).oArm IsNot Nothing Then
                            bArmOK = moColParam(nColumn).oArm.IsOnLine
                        End If
                    End If
                End If

                'Get column parameters

                'If mbAbortForShutdown Then
                '    Exit Function
                'End If
                'Make sure the column isn't already there.
                Dim bAlreadyInTable As Boolean = False
                If mdgvData.ColumnCount > 1 And Not (bReload) Then
                    For nCol As Integer = 0 To mdgvData.ColumnCount - 2
                        Dim bDupCol As Boolean = True
                        If mtTabCfgs(mnTab).bUseZone Then
                            bDupCol = (mdgvData.Rows(mnZONE_ROW).Cells.Item(nCol).Value.ToString = _
                                       mdgvData.Rows(mnZONE_ROW).Cells.Item(nColumn).Value.ToString)
                        End If
                        If mtTabCfgs(mnTab).bUseArm And bDupCol Then
                            bDupCol = (mdgvData.Rows(mnROBOT_ROW).Cells.Item(nCol).Value.ToString = _
                                      mdgvData.Rows(mnROBOT_ROW).Cells.Item(nColumn).Value.ToString)
                        End If
                        If mtTabCfgs(mnTab).bUseController And bDupCol Then
                            bDupCol = (mdgvData.Rows(mnCONTROLLER_ROW).Cells.Item(nCol).Value.ToString = _
                                      mdgvData.Rows(mnCONTROLLER_ROW).Cells.Item(nColumn).Value.ToString)
                        End If
                        If mtTabCfgs(mnTab).bUseValve And bDupCol Then
                            bDupCol = (mdgvData.Rows(mnVALVE_ROW).Cells.Item(nCol).Value.ToString = _
                                      mdgvData.Rows(mnVALVE_ROW).Cells.Item(nColumn).Value.ToString)
                        End If
                        If mtTabCfgs(mnTab).bUseColor And bDupCol Then
                            bDupCol = (mdgvData.Rows(mnCOLOR_ROW).Cells.Item(nCol).Value.ToString = _
                                      mdgvData.Rows(mnCOLOR_ROW).Cells.Item(nColumn).Value.ToString)
                        End If
                        If mtTabCfgs(mnTab).bUseCustomParm And bDupCol Then
                            bDupCol = (mdgvData.Rows(mnCUSTOMPARM_ROW).Cells.Item(nCol).Value.ToString = _
                                      mdgvData.Rows(mnCUSTOMPARM_ROW).Cells.Item(nColumn).Value.ToString)
                        End If
                        If bDupCol Then
                            'Duplicate column exists, remove it
                            bAlreadyInTable = True
                            mdgvData.Columns.RemoveAt(nColumn)
                            Exit For
                        End If
                    Next
                End If
                'selection OK
                If Not (bAlreadyInTable) And bArmOK And _
                        ((moColParam(nColumn).nValve > 0) Or (mtTabCfgs(mnTab).bUseValve = False)) And _
                        ((moColParam(nColumn).nColor > 0) Or (mtTabCfgs(mnTab).bUseColor = False)) And _
                        ((moColParam(nColumn).sCustom <> String.Empty) Or (mtTabCfgs(mnTab).bUseCustomParm = False)) Then
                    'Data cells
                    For nRow As Integer = mnITEM0_ROW To mtTabCfgs(mnTab).ItemCfg.GetUpperBound(0) + mnITEM0_ROW
                        'If mbAbortForShutdown Then
                        '    Exit Function
                        'End If
                        Dim oCell As DataGridViewCell = DirectCast(mdgvData.Rows(nRow).Cells.Item(nColumn), DataGridViewCell)
                        Dim bDoLoad As Boolean = True
                        If (oCell.Style.ForeColor = Color.Red) AndAlso (bOverWrite = False) AndAlso _
                            (oCell.Tag IsNot Nothing) Then
                            bDoLoad = False
                        End If

                        If bDoLoad Then
                            With mtTabCfgs(mnTab).ItemCfg(nRow - mnITEM0_ROW)
                                If Not (bReload) Or oCell.ContextMenuStrip Is Nothing Then  'MAD 3/22/14
                                    If (.bCbo) Then
                                        If .bEdit Then
                                            If (.DataType Is GetType(Boolean)) Then
                                                If .CustomCBO.GetUpperBound(0) > 0 Then 'RJO 12/14/11
                                                    oCell.ContextMenuStrip = .mnuPopUp
                                                Else
                                                    oCell.ContextMenuStrip = mnuYesNo
                                                End If
                                            Else
                                                oCell.ContextMenuStrip = .mnuPopUp
                                            End If
                                        Else
                                            oCell.ContextMenuStrip = Nothing
                                        End If
                                        oCell.ValueType = GetType(String)
                                        oCell.ReadOnly = True
                                    Else
                                        If (.DataType Is GetType(String)) Then
                                            oCell.ValueType = GetType(String)
                                        ElseIf (.DataType Is GetType(Integer)) Then
                                            If .Scale = 1.0 Then
                                                oCell.ValueType = GetType(Integer)
                                            Else
                                                oCell.ValueType = GetType(Double)
                                            End If
                                        ElseIf (.DataType Is GetType(Single)) Then 'RJO 09/30/11
                                            oCell.ValueType = GetType(Single)
                                        ElseIf (.DataType Is GetType(Double)) Then
                                            oCell.ValueType = GetType(Double)
                                        End If
                                        oCell.ContextMenuStrip = Nothing
                                        oCell.ReadOnly = Not (.bEdit)
                                        If (oCell.ValueType Is GetType(Double)) Then
                                            Dim sFormat As String = "0" 'MSW this was "#", but 0 would show up blank
                                            If .Precision > 0 Then
                                                sFormat = sFormat & "."
                                                For nTmp As Integer = 1 To .Precision
                                                    sFormat = sFormat & "0"
                                                Next
                                            End If
                                            oCell.Style.Format = sFormat
                                        End If
                                    End If
                                End If
                                Select Case .DataSource
                                    Case eDataSource.DB
                                        subGetDBValue(mtTabCfgs(mnTab).ItemCfg(nRow - mnITEM0_ROW), oCell)
                                    Case eDataSource.XML
                                        subGetXMLValue(mtTabCfgs(mnTab).ItemCfg(nRow - mnITEM0_ROW), oCell)
                                    Case eDataSource.PLC
                                        subGetPLCValue(mtTabCfgs(mnTab).ItemCfg(nRow - mnITEM0_ROW), oCell, moColParam(nColumn).oArm, _
                                            moColParam(nColumn).nColor, moColParam(nColumn).nValve, moColParam(nColumn).sCustom, moColParam(nColumn).oZone)
                                    Case eDataSource.Registry
                                        subGetRegistryValue(mtTabCfgs(mnTab).ItemCfg(nRow - mnITEM0_ROW), oCell)
                                    Case eDataSource.Robot
                                        subGetRobotValue(mtTabCfgs(mnTab).ItemCfg(nRow - mnITEM0_ROW), oCell, moColParam(nColumn).oArm, _
                                            moColParam(nColumn).nColor, moColParam(nColumn).nValve, moColParam(nColumn).sCustom, bOverWrite)
                                    Case Else
                                        If (oCell.ValueType Is GetType(String)) Then
                                            oCell.Value = String.Empty
                                        Else
                                            oCell.Value = 0
                                        End If

                                End Select
                                If moToonInterface IsNot Nothing Then
                                    moToonInterface.SetItem(mtTabCfgs(mnTab).ItemCfg(nRow - mnITEM0_ROW), oCell.Value)
                                End If
                                'Process cbo and boolean data
                                If .bCbo Then
                                    Dim sTmp As String = oCell.Value.ToString
                                    If (.DataType Is GetType(Boolean)) And .CustomCBO.GetUpperBound(0) = 0 Then 'RJO 12/14/11
                                        If IsNumeric(sTmp) Then '0,1 bit values
                                            If (CInt(sTmp) > 0) Then
                                                oCell.Value = gpsRM.GetString("psYES")
                                            Else
                                                oCell.Value = gpsRM.GetString("psNO")
                                            End If
                                        Else 'text version
                                            'Check for yes,no,true,false
                                            'Hopefully this will handle english and the local version
                                            Select Case sTmp.ToLower
                                                Case "yes", "true", gpsRM.GetString("psYES").ToLower, gpsRM.GetString("psTRUE").ToLower
                                                    oCell.Value = gpsRM.GetString("psYES")
                                                Case "no", "false", gpsRM.GetString("psNO").ToLower, gpsRM.GetString("psFALSE").ToLower
                                                    oCell.Value = gpsRM.GetString("psNO")
                                            End Select
                                        End If
                                    Else
                                        'Search custom CBO data 
                                        Dim nItem As Integer = 1
                                        Dim bFound As Boolean = False
                                        Do While (nItem <= .CustomCBO.GetUpperBound(0)) And (bFound = False)
                                            If (.CustomCBO(nItem) = sTmp) Or (.CustomCBO(nItem) = "*") Then
                                                oCell.Value = .CustomCBO(nItem - 1)
                                                bFound = True
                                            Else
                                                nItem += 2
                                            End If
                                        Loop
                                    End If
                                    If .CustomColors.GetUpperBound(0) > 0 Then
                                        'Search custom color data 
                                        Dim nItem As Integer = 1
                                        Dim bFound As Boolean = False
                                        Do While (nItem <= .CustomColors.GetUpperBound(0)) And (bFound = False)
                                            If (.CustomColors(nItem) = sTmp) Or (.CustomColors(nItem) = "*") Then
                                                oCell.Style.BackColor = Color.FromName(.CustomColors(nItem - 1))
                                                bFound = True
                                            Else
                                                nItem += 2
                                            End If
                                        Loop
                                    End If

                                Else
                                    'No extra processing required
                                End If
                                'save the old value in the cell tag
                                If .DataSource <> eDataSource.Robot Then
                                    oCell.Tag = oCell.Value
                                End If
                                oCell.Style.ForeColor = Color.Black
                                oCell.Style.SelectionForeColor = Color.White
                            End With
                        End If
                    Next
                    DataLoaded = True
                Else
                    If Not bArmOK Then
                        If Not mbRobotNotAvailable(moColParam(nColumn).oArm.Controller.ControllerNumber - 1) Then
                            mbRobotNotAvailable(moColParam(nColumn).oArm.Controller.ControllerNumber - 1) = True
                            MessageBox.Show(moColParam(nColumn).oArm.Controller.Name & " " & gcsRM.GetString("csISUNAVAILABLE"), _
                                gcsRM.GetString("csPAINTWORKS"), MessageBoxButtons.OK, _
                                MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, _
                                MessageBoxOptions.DefaultDesktopOnly)
                        End If
                    End If
                    If Not (bAlreadyInTable) Then
                        'Something failed for the whole column, give up
                        For nRow As Integer = mnITEM0_ROW To mtTabCfgs(mnTab).ItemCfg.GetUpperBound(0) + mnITEM0_ROW
                            Dim oCell As DataGridViewCell = DirectCast(mdgvData.Rows(nRow).Cells.Item(nColumn), DataGridViewCell)
                            With mtTabCfgs(mnTab).ItemCfg(nRow - mnITEM0_ROW)
                                oCell.Value = gpsRM.GetString("psNA")
                                oCell.Tag = Nothing 'Mark it unavailable 
                                oCell.ReadOnly = True
                                oCell.ContextMenuStrip = Nothing
                            End With
                            bReturnVal = False
                        Next

                    End If
                End If
                '                Progress = 80
                'Status = gcsRM.GetString("csLOADDONE")

            End If

        Catch ex As Exception
            Status = gcsRM.GetString("csLOADFAILED")
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            bReturnVal = False
        Finally
            'If mbAbortForShutdown = False Then
            '    Progress = 100
            'End If
        End Try

        Return bReturnVal
    End Function
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

        ' false means user pressed cancel
        If bAskForSave() = False Then
            cboColor.Text = msOldColor
            Exit Sub
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
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subEnableControls(False)
            Status = gcsRM.GetString("csLOADINGDATA")

            bStatus = lstStatus.Visible
            lstStatus.Visible = True
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

            Progress = 5
            Dim nTag() As Integer = DirectCast(cboParm.Tag, Integer())
            mnColorIndex = nTag(cboParm.SelectedIndex)
            If (Not (mbEventBlocker)) And (Not (btnMultiView.Checked)) Then
                bLoadData()
                mdgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
                mdgvData.AutoResizeColumns()
                mdgvData.Enabled = True
            End If

            'subFormatScreenLayout()
            EditsMade = False
            Me.Cursor = System.Windows.Forms.Cursors.Default
            Status(True) = gpsRM.GetString("csREADY")
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
        Dim sCultureArg As String = "/culture="

        'If a culture string has been passed in, set the current culture (display language)
        For Each s As String In My.Application.CommandLineArgs
            If s.ToLower.StartsWith(sCultureArg) Then
                Culture = s.Remove(0, sCultureArg.Length)
            End If
            'Robot and zone select
            Dim sArg As String = "/screen="
            If s.ToLower.StartsWith(sArg) Then
                msScreen = s.Remove(0, sArg.Length)
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
        'Select tab 1   RV 161005
        'tabMain.SelectedIndex = 0
        'HGB parameter added to force a reload of the robot combo
        'subChangeTab(True)
        ' copy button

        ' false means user pressed cancel
        If bAskForSave() = False Then
            cboRobot.Text = msOldRobot
            Exit Sub
        End If

        'just in case
        If cboRobot.Text = String.Empty Then
            Exit Sub
        Else
            If cboRobot.Text = msOldRobot Then Exit Sub
        End If
        If msOldRobot <> String.Empty Then _
                                colArms.Item(msOldRobot).Selected = False
        If msOldController <> String.Empty Then _
                                colControllers.Item(msOldController).Selected = False
        'Testing
        'If cboZone.Text = "Base1" Then msOldRobot = "B1"
        'If cboZone.Text = "Base1" Then msOldController = "BC1"
        'If cboZone.Text = "Base2" Then msOldRobot = "B2"
        'If cboZone.Text = "Base2" Then msOldController = "BC2"

        If mtTabCfgs(mnTab).bScatteredAccess And (Not (btnMultiView.Checked)) Then
            subClearSARequests()
        End If
        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            Progress = 5

            Status = gpsRM.GetString("psSELECTING") & cboRobot.Text
            If mtTabCfgs(mnTab).bUseArm Then
                msOldRobot = cboRobot.Text
                mRobot = colArms.Item(cboRobot.Text)
                colArms.Item(cboRobot.Text).Selected = True
                msOldController = mRobot.Controller.Name
            Else 'go ahead and assume it's by controller
                msOldController = cboRobot.Text
                If (msOldRobot <> String.Empty) AndAlso (colArms.Item(msOldRobot).Controller.Name = msOldController) Then
                    'current robot selection is on this controller, set the vars to be sure
                    mRobot = colArms.Item(msOldRobot)
                    colArms.Item(msOldRobot).Selected = True
                Else
                    'Take arm one from the selected controller
                    msOldRobot = colControllers.Item(msOldController).Arms(0).Name
                    'If cboZone.Text = "Base1" Then msOldRobot = "B1"
                    'If cboZone.Text = "Base1" Then msOldController = "BC1"
                    'If cboZone.Text = "Base2" Then msOldRobot = "B2"
                    'If cboZone.Text = "Base2" Then msOldController = "BC2"
                    mRobot = colArms.Item(msOldRobot)
                    colArms.Item(msOldRobot).Selected = True
                End If
            End If

            Progress = 15

            Dim bFind As Boolean = False
            If mtTabCfgs(mnTab).bUseValve Then
                mRobot.SystemColors.Load(mRobot)
                mSysColorCommon.LoadValveBoxFromCollection(mRobot.SystemColors, cboColor, True)
                bFind = True
            ElseIf mtTabCfgs(mnTab).bUseColor Then
                mRobot.SystemColors.Load(mRobot)
                mSysColorCommon.LoadColorBoxFromCollection(mRobot.SystemColors, cboColor)
                bFind = True
            End If
            If bFind Then
                cboColor.SelectedIndex = -1
                For nIdx As Integer = 0 To cboColor.Items.Count
                    If cboColor.Items(nIdx).ToString = msOldColor Then
                        cboColor.Text = msOldColor
                        bFind = False
                        Exit For
                    End If
                Next
                If bFind Then
                    msOldColor = String.Empty
                End If
            End If
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Progress = 70
            If (Not (mbEventBlocker)) And (Not (btnMultiView.Checked)) Then
                subEnableControls(False)
                Status = gcsRM.GetString("csLOADINGDATA")
                bLoadData()
                mdgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
                mdgvData.AutoResizeColumns()
                mdgvData.Enabled = True
                'subFormatScreenLayout()
                EditsMade = False
                Status(True) = gpsRM.GetString("csREADY")
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
    Private Sub subChangeParm()
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

        ' false means user pressed cancel
        If bAskForSave() = False Then
            cboParm.Text = msOldParm
            Exit Sub
        End If

        'just in case
        If cboParm.Text = String.Empty Then
            Exit Sub
        Else
            If cboParm.Text = msOldParm Then Exit Sub
        End If

        msOldParm = cboParm.Text

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            Progress = 5

            Status = gpsRM.GetString("psSELECTING") & msOldParm
            Dim sTag() As String = DirectCast(cboParm.Tag, String())

            'The tag is the robot's parm number starting with 1, all the data starts at 0
            msParmTag = sTag(cboParm.SelectedIndex)

            If (Not (mbEventBlocker)) And (Not (btnMultiView.Checked)) Then
                subEnableControls(False)
                Status = gcsRM.GetString("csLOADINGDATA")
                bLoadData()
                mdgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
                mdgvData.AutoResizeColumns()
                mdgvData.Enabled = True
                'subFormatScreenLayout()
                EditsMade = False
                Status(True) = gpsRM.GetString("csREADY")
            End If
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''


            Progress = 96


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

    Private Sub subSetXMLValue(ByRef oItemCfg As tItemCfg, ByRef oCell As DataGridViewCell)
        '********************************************************************************************
        'Description:  set XML value
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/28/12  MSW     Add XML data support
        '********************************************************************************************
        Dim sXMLFile As String = oItemCfg.DBName
        Dim sXMLTABLE As String = oItemCfg.DBTableName
        Dim sXMLRow As String = oItemCfg.DBRowName
        Dim sXMLCol As String = oItemCfg.DBColumnName
        Dim sTopic As String = String.Empty
        Dim oMainNode As XmlNode = Nothing
        Dim oNodeList As XmlNodeList = Nothing
        Dim oRows As XmlNodeList = Nothing
        Dim oXMLDoc As New XmlDocument
        Dim sXMLFilePath As String = String.Empty
        Dim sVal As String = String.Empty
        Try
            If (oItemCfg.bCbo) Then
                If (oItemCfg.DataType Is GetType(Boolean)) And oItemCfg.CustomCBO.GetUpperBound(0) = 0 Then 'RJO 12/14/11
                    Select Case oCell.Value.ToString.ToLower
                        Case "yes", "true", gpsRM.GetString("psYES").ToLower, gpsRM.GetString("psTRUE").ToLower
                            sVal = True.ToString
                        Case "no", "false", gpsRM.GetString("psNO").ToLower, gpsRM.GetString("psFALSE").ToLower
                            sVal = False.ToString
                    End Select
                Else
                    sVal = oCell.Value.ToString
                End If
            Else
                If oCell.ValueType Is GetType(Double) Then
                    Select Case oItemCfg.formula
                        Case Else
                            If oItemCfg.DataType Is GetType(Integer) Then
                                sVal = CType((CType(oCell.Value, Double) / oItemCfg.Scale), Integer).ToString
                            Else
                                sVal = (CType(oCell.Value, Double) / oItemCfg.Scale).ToString
                            End If
                    End Select
                Else
                    sVal = oCell.Value.ToString
                End If
            End If
            If oItemCfg.Bitmask <> 0 Then
                Dim nTmp1 As Integer = DirectCast(oCell.Tag, Integer)
                nTmp1 = nTmp1 And Not (oItemCfg.Bitmask)
                Dim nTmp2 As Integer = CType(sVal, Integer)
                sVal = (nTmp1 Or nTmp2).ToString
                oCell.Tag = oCell.Value
            Else
                oCell.Tag = oCell.Value
            End If
            '    08/15/13   MSW     PSC remote PC support
            If GetDefaultFilePath(sXMLFilePath, eDir.XML, colZones.ActiveZone.RemotePath, sXMLFile & ".XML") Then
                Try
                    oXMLDoc.Load(sXMLFilePath)
                    oMainNode = oXMLDoc.SelectSingleNode("//" & sXMLTABLE)
                    If sXMLRow <> String.Empty Then
                        oRows = oMainNode.SelectNodes("//" & sXMLRow)
                        oNodeList = oRows(0).SelectNodes("//" & sXMLCol)
                    Else
                        oNodeList = oMainNode.SelectNodes("//" & sXMLCol)
                    End If
                    If oNodeList.Count > 0 Then
                        oNodeList(0).InnerXml = sVal
                    End If
                Catch ex As Exception
                    mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: subSetXMLValue", _
                                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
                End Try
            End If
            oXMLDoc.Save(sXMLFilePath)
        Catch ex As Exception
        End Try

    End Sub
    Private Sub subSetDBValue(ByRef oItemCfg As tItemCfg, ByRef oCell As DataGridViewCell)
        ''********************************************************************************************
        ''Description:  set DB value
        ''
        ''Parameters: none
        ''Returns:    none
        ''
        ''Modification history:
        ''
        '' Date      By      Reason
        '' 12/14/11  RJO     Added support for Boolean values with custom combos
        ''********************************************************************************************
        'Dim sTable As String = "[" & oItemCfg.DBTableName & "]"
        'Try
        '    'Not loaded yet
        '    Dim oDB As clsSQLAccess = New clsSQLAccess
        '    Dim oDT As DataTable = Nothing

        '    Dim ds As DataSet = Nothing
        '    With oDB
        '        .Zone = colZones.ActiveZone
        '        .DBFileName = oItemCfg.DBName
        '        .DBTableName = sTable
        '        .SQLString = "SELECT * FROM " & sTable '& " ORDER BY PRIMARY KEY" '" & _
        '        'sTable & ".[" & gsPRMSET_COL_INDEX & "]"
        '        ds = .GetDataSet
        '        If ds.Tables.Contains(sTable) Then
        '            oDT = ds.Tables(sTable)
        '        End If
        '    End With

        '    '''''''''''''''''''''''''''''''''
        '    If Not (oDT Is Nothing) Then
        '        'Get the datatable row
        '        Dim dr As DataRow = Nothing
        '        If oItemCfg.DBRowName = String.Empty Then
        '            dr = oDT.Rows(0)
        '        ElseIf IsNumeric(oItemCfg.DBRowName) Then
        '            Dim nRow As Integer = CInt(oItemCfg.DBRowName)
        '            dr = oDT.Rows(nRow)
        '        Else
        '            Dim dra() As DataRow = oDT.Select(oItemCfg.DBRowName)
        '            dr = dra(0)
        '        End If
        '        'Set the value
        '        If Not (dr Is Nothing) Then
        '            dr.BeginEdit()
        '            If (oItemCfg.bCbo) Then
        '                If (oItemCfg.DataType Is GetType(Boolean)) And oItemCfg.CustomCBO.GetUpperBound(0) = 0 Then 'RJO 12/14/11
        '                    Select Case oCell.Value.ToString.ToLower
        '                        Case "yes", "true", gpsRM.GetString("psYES").ToLower, gpsRM.GetString("psTRUE").ToLower
        '                            dr.Item(oItemCfg.DBColumnName) = True
        '                        Case "no", "false", gpsRM.GetString("psNO").ToLower, gpsRM.GetString("psFALSE").ToLower
        '                            dr.Item(oItemCfg.DBColumnName) = False
        '                    End Select
        '                Else
        '                    'Search custom CBO data 
        '                    Dim sTmp As String = oCell.Value.ToString
        '                    Dim nItem As Integer = 0
        '                    Dim bFound As Boolean = False
        '                    Do While (nItem <= oItemCfg.CustomCBO.GetUpperBound(0)) And (bFound = False)
        '                        If oItemCfg.CustomCBO(nItem) = sTmp Then
        '                            If (oItemCfg.DataType Is GetType(Boolean)) Then
        '                                dr.Item(oItemCfg.DBColumnName) = CType(oItemCfg.CustomCBO(nItem + 1), Boolean)
        '                            ElseIf (oItemCfg.DataType Is GetType(Integer)) Then
        '                                dr.Item(oItemCfg.DBColumnName) = CType(oItemCfg.CustomCBO(nItem + 1), Integer)
        '                            ElseIf (oItemCfg.DataType Is GetType(Double)) Then
        '                                dr.Item(oItemCfg.DBColumnName) = CType(oItemCfg.CustomCBO(nItem + 1), Double)
        '                            Else
        '                                dr.Item(oItemCfg.DBColumnName) = oItemCfg.CustomCBO(nItem + 1)
        '                            End If
        '                            bFound = True
        '                        Else
        '                            nItem += 2
        '                        End If
        '                    Loop

        '                End If
        '            Else
        '                If oCell.ValueType Is GetType(Double) Then
        '                    Select Case oItemCfg.formula
        '                        Case Else
        '                            If oItemCfg.DataType Is GetType(Integer) Then
        '                                dr.Item(oItemCfg.DBColumnName) = CType((CType(oCell.Value, Double) / oItemCfg.Scale), Integer)
        '                            Else
        '                                dr.Item(oItemCfg.DBColumnName) = CType(oCell.Value, Double) / oItemCfg.Scale
        '                            End If
        '                    End Select
        '                Else
        '                    dr.Item(oItemCfg.DBColumnName) = oCell.Value
        '                End If
        '            End If

        '            dr.EndEdit()
        '        End If

        '        oDB.UpdateDataSet(ds, sTable)
        '        oDB.Close()
        '        oCell.Tag = oCell.Value
        '    End If
        'Catch ex As Exception
        '    Trace.WriteLine(ex.Message)
        '    Dim x As New CheckedListBox

        'End Try

    End Sub
    Private Sub subSetPLCValue(ByRef oItemCfg As tItemCfg, ByRef oCell As DataGridViewCell, ByRef Arm As clsArm, _
                                    Optional ByVal nColor As Integer = 0, Optional ByVal nValve As Integer = 0, _
                                    Optional ByRef sCustom As String = "", Optional ByRef Zone As clsZone = Nothing)
        '********************************************************************************************
        'Description:  set PLC value
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/17/10  BTK     Added code to check if we are using a combobox for data.  If so we need to
        '                   determine what value needs to be written to the PLC based on the text selection.
        ' 12/14/11  RJO     Added support for Boolean values with custom combos
        '********************************************************************************************
        Dim sTag As String = sConvertTags(oItemCfg.PLCTag, Arm, nColor, nValve, sCustom, Zone)
        Dim sData(0) As String
        If mPLC Is Nothing Then
            mPLC = New clsPLCComm
        End If

        If (oItemCfg.bCbo) Then
            If (oItemCfg.DataType Is GetType(Boolean)) And oItemCfg.CustomCBO.GetUpperBound(0) = 0 Then 'RJO 12/14/11
                Select Case oCell.Value.ToString.ToLower
                    Case "yes", "true", gpsRM.GetString("psYES").ToLower, gpsRM.GetString("psTRUE").ToLower
                        sData(0) = CType(1, String)
                    Case "no", "false", gpsRM.GetString("psNO").ToLower, gpsRM.GetString("psFALSE").ToLower
                        sData(0) = CType(0, String)
                End Select
            Else
                'Search custom CBO data 
                Dim sTmp As String = oCell.Value.ToString
                Dim nItem As Integer = 0
                Dim bFound As Boolean = False
                Do While (nItem <= oItemCfg.CustomCBO.GetUpperBound(0)) And (bFound = False)
                    If oItemCfg.CustomCBO(nItem) = sTmp Then
                        If (oItemCfg.DataType Is GetType(Boolean)) Then
                            sData(0) = CType(oItemCfg.CustomCBO(nItem + 1), Boolean).ToString
                        ElseIf (oItemCfg.DataType Is GetType(Integer)) Then
                            sData(0) = CType(oItemCfg.CustomCBO(nItem + 1), Integer).ToString
                        ElseIf (oItemCfg.DataType Is GetType(Double)) Then
                            sData(0) = CType(oItemCfg.CustomCBO(nItem + 1), Double).ToString
                        Else
                            sData(0) = oItemCfg.CustomCBO(nItem + 1).ToString
                        End If
                        bFound = True
                    Else
                        nItem += 2
                    End If
                Loop

            End If
        Else
            If oCell.ValueType Is GetType(Double) Then
                Select Case oItemCfg.formula
                    Case Else
                        If oItemCfg.DataType Is GetType(Integer) Then
                            'converts cell to double, divides, then converts to intetger, then to string
                            sData(0) = CType((CType(oCell.Value, Double) / oItemCfg.Scale), Integer).ToString
                        Else
                            sData(0) = (CType(oCell.Value, Double) / oItemCfg.Scale).ToString
                        End If
                End Select
            ElseIf (oCell.ValueType Is GetType(String)) Then
                Dim sTmp As String = oCell.Value.ToString
                ReDim sData(oItemCfg.ArrayLength)
                If oItemCfg.ArrayLength > 0 Then
                    For nIdx As Integer = 0 To oItemCfg.ArrayLength
                        If nIdx < sTmp.Length Then
                            sData(nIdx) = Asc(sTmp.Substring(nIdx, 1)).ToString
                        Else
                            sData(nIdx) = "0"
                        End If
                    Next
                Else
                    sData(0) = oCell.Value.ToString
                End If
            Else
                sData(0) = oCell.Value.ToString
            End If
        End If
        If oItemCfg.Bitmask <> 0 Then
            Dim nTmp1 As Integer = DirectCast(oCell.Tag, Integer)
            nTmp1 = nTmp1 And Not (oItemCfg.Bitmask)
            Dim nTmp2 As Integer = CType(sData(0), Integer)
            sData(0) = (nTmp1 Or nTmp2).ToString
        End If

        With mPLC
            .ZoneName = colZones.ActiveZone.Name
            .TagName = sTag
            .PLCData = sData
        End With
        oCell.Tag = oCell.Value

    End Sub
    Private Sub subSetRegistryValue(ByRef oItemCfg As tItemCfg, ByRef oCell As DataGridViewCell)
        '********************************************************************************************
        'Description:  set Registry value
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        oCell.Tag = oCell.Value

    End Sub
    Private Function sGetCellVal(ByRef oItemCfg As tItemCfg, ByRef oCell As DataGridViewCell) As String
        '********************************************************************************************
        'Description:  set robot variable value
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sTmp As String = oCell.Value.ToString
        If (oItemCfg.bCbo) Then
            'Search custom CBO data 
            Dim nItem As Integer = 0
            Dim bFound As Boolean = False
            Do While (nItem <= oItemCfg.CustomCBO.GetUpperBound(0)) And (bFound = False)
                If oItemCfg.CustomCBO(nItem) = sTmp Then
                    sTmp = (oItemCfg.CustomCBO(nItem + 1).ToString)
                    bFound = True
                Else
                    nItem += 2
                End If
            Loop
        Else
            If oCell.ValueType Is GetType(Single) Then
                Dim nTmp As Single = CType(oCell.Value, Single)
                Select Case oItemCfg.formula
                    Case Else
                        sTmp = ((nTmp / oItemCfg.Scale).ToString)
                End Select
            End If
        End If
        Return (sTmp)
    End Function
    Private Sub subSetRobotValue(ByRef oItemCfg As tItemCfg, ByRef oCell As DataGridViewCell, ByRef Arm As clsArm, _
                                    Optional ByVal nColor As Integer = 0, Optional ByVal nValve As Integer = 0, _
                                    Optional ByVal sCustom As String = "")
        '********************************************************************************************
        'Description:  set robot variable value
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/29/11  RJO     Added support for RegNumerics
        ' 12/14/11  RJO     Added support for Boolean values with custom combos
        ' 10/09/13  MSW     subSetRobotValue - Status screen changes broke 2var
        '                   Write, fix it
        '********************************************************************************************
        Dim sVal As String = String.Empty
        For nIdx As Integer = 0 To oItemCfg.ProgName.GetUpperBound(0)
            If oItemCfg.ProgName(nIdx) <> String.Empty Then
                Dim sProg As String = sConvertTags(oItemCfg.ProgName(nIdx), Arm, nColor, nValve, sCustom)
                Dim sVar As String = sConvertTags(oItemCfg.VarName(nIdx), Arm, nColor, nValve, sCustom)
                'Dim oFRCRobot As FRRobot.FRCRobot = Arm.Controller.Robot
                'Dim oVars As FRRobot.FRCVars= nothing
                Select Case oItemCfg.ProgName(nIdx).ToLower
                    'Case "*system*"
                    '    'oVars = oFRCRobot.SysVariables
                    Case "*numreg*"
                        Dim oVar As FRRobot.FRCRegNumeric
                        If oCell.Tag IsNot Nothing AndAlso (nIdx = 0) Then
                            oVar = DirectCast(oCell.Tag, FRRobot.FRCRegNumeric)
                        Else
                            Dim oFRCRobot As FRRobot.FRCRobot = Arm.Controller.Robot
                            Dim oVars As FRRobot.FRCVars = oFRCRobot.RegNumerics
                            Dim oVarA As FRRobot.FRCVar = DirectCast(oVars(CType(sVar, Integer)), FRRobot.FRCVar)
                            oVar = DirectCast(oVarA.Value, FRRobot.FRCRegNumeric)
                            oCell.Tag = oVar
                        End If
                        mRegVal.WriteNumReg(oVar, sGetCellVal(oItemCfg, oCell))
                    Case "*strreg*"
                        Dim oVar As FRRobot.FRCRegString
                        If oCell.Tag IsNot Nothing AndAlso (nIdx = 0) Then
                            oVar = DirectCast(oCell.Tag, FRRobot.FRCRegString)
                        Else
                            Dim oFRCRobot As FRRobot.FRCRobot = Arm.Controller.Robot
                            Dim oVars As FRRobot.FRCVars = oFRCRobot.RegStrings
                            Dim oVarA As FRRobot.FRCVar = DirectCast(oVars(CType(sVar, Integer)), FRRobot.FRCVar)
                            oVar = DirectCast(oVarA.Value, FRRobot.FRCRegString)
                            oCell.Tag = oVar
                        End If
                        mRegVal.WriteStringReg(oVar, sGetCellVal(oItemCfg, oCell))
                    Case Else
                        'Arm.ProgramName = sProg
                        'oVars = Arm.ProgramVars
                        Dim oVar As FRRobot.FRCVar = DirectCast(oCell.Tag, FRRobot.FRCVar)
                        If oCell.Tag IsNot Nothing AndAlso (nIdx = 0) Then
                            oVar = DirectCast(oCell.Tag, FRRobot.FRCVar)
                        Else
                            Dim oFRCRobot As FRRobot.FRCRobot = Arm.Controller.Robot
                            If oItemCfg.ProgName(nIdx).ToLower = "*system*" Then
                                Dim oVars As FRRobot.FRCVars = oFRCRobot.SysVariables
                                oVar = DirectCast(oVars(sVar), FRRobot.FRCVar)
                            Else
                                Dim oList As FRRobot.FRCPrograms = oFRCRobot.Programs
                                Dim oProgram As FRRobot.FRCProgram = CType(oList.Item(oItemCfg.ProgName(nIdx).ToLower), FRRobot.FRCProgram)
                                oVar = DirectCast(oProgram.Variables(sVar), FRRobot.FRCVar)
                            End If

                        End If
                        If (oItemCfg.bCbo) Then
                            If (oItemCfg.DataType Is GetType(Boolean)) And oItemCfg.CustomCBO.GetUpperBound(0) = 0 Then 'RJO 12/14/11
                                Select Case oCell.Value.ToString.ToLower
                                    Case "yes", "true", gpsRM.GetString("psYES").ToLower, gpsRM.GetString("psTRUE").ToLower
                                        oVar.Value = True
                                    Case "no", "false", gpsRM.GetString("psNO").ToLower, gpsRM.GetString("psFALSE").ToLower
                                        oVar.Value = False
                                End Select
                            Else
                                'Search custom CBO data 
                                Dim sTmp As String = oCell.Value.ToString
                                Dim nItem As Integer = 0
                                Dim bFound As Boolean = False
                                Do While (nItem <= oItemCfg.CustomCBO.GetUpperBound(0)) And (bFound = False)
                                    If oItemCfg.CustomCBO(nItem) = sTmp Then
                                        If (oItemCfg.DataType Is GetType(Boolean)) Then
                                            oVar.Value = CType(oItemCfg.CustomCBO(nItem + 1), Boolean)
                                        ElseIf (oItemCfg.DataType Is GetType(Integer)) Then
                                            oVar.Value = CType(oItemCfg.CustomCBO(nItem + 1), Integer)
                                        ElseIf (oItemCfg.DataType Is GetType(Double)) Then
                                            oVar.Value = CType(oItemCfg.CustomCBO(nItem + 1), Double)
                                        Else
                                            oVar.Value = oItemCfg.CustomCBO(nItem + 1)
                                        End If
                                        bFound = True
                                    Else
                                        nItem += 2
                                    End If
                                Loop
                            End If
                        Else
                            If oCell.ValueType Is GetType(Double) Then
                                Select Case oItemCfg.formula
                                    Case Else
                                        If oItemCfg.DataType Is GetType(Integer) Then
                                            'converts cell to double, divides, then converts to intetger, then to string
                                            oVar.Value = CType((CType(oCell.Value, Double) / oItemCfg.Scale), Integer)
                                        Else
                                            oVar.Value = (CType(oCell.Value, Double) / oItemCfg.Scale)
                                        End If
                                End Select
                            Else
                                oVar.Value = oCell.Value
                            End If
                        End If
                End Select
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
        ' 11/23/09  MSW     subSaveData - Add check for tag = nothing
        ' 04/08/10  RJO     Added code to highligt the top cell before saving in case a cell with
        '                   changed data is highlighted. When this was the case, the data would not 
        '                   be saved. Also added code to update the Tag of the cell with changed
        '                   data. Otherwise you couldn't change it back to the original value in the
        '                   same edit session.
        ' 07/27/10  msw     send the right custom cbo value (tracking schedule)
        ' 03/28/12  MSW     Add XML data support
        ' 02/03/14  RJO     Fix to prevent atempts to save "N/A" data.
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

        mdgvData.EndEdit()
        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        mScreenSetup.MoveCursorOffButton(DirectCast(btnSave, ToolStripItem))

        Try

            ' do save

            Status = gcsRM.GetString("csSAVINGDATA")
            Dim nColumn As Integer = 0

            'Select the top cell so we save data in the highlighted cell if it was changed
            mdgvData.Rows(0).Cells(0).Selected = True 'RJO 04/08/10

            For nColumn = 0 To mdgvData.ColumnCount - 1

                Progress = 10 * nColumn \ mdgvData.ColumnCount
                Dim sChange As String = String.Empty
                'Get column parameters
                Dim nColor As Integer = 0

                Dim sTmp As String = Nothing
                If mdgvData.Rows(mnCOLOR_ROW).Cells.Item(nColumn).Tag IsNot Nothing Then
                    sTmp = mdgvData.Rows(mnCOLOR_ROW).Cells.Item(nColumn).Tag.ToString()
                Else
                    sTmp = String.Empty
                End If
                If IsNumeric(sTmp) Then
                    nColor = CInt(sTmp)
                Else
                    nColor = 0
                End If
                Dim nValve As Integer = 0
                sTmp = mdgvData.Rows(mnVALVE_ROW).Cells.Item(nColumn).Value.ToString
                If IsNumeric(sTmp) Then
                    nValve = CInt(sTmp)
                Else
                    nValve = 0
                End If
                Dim sColor As String = mdgvData.Rows(mnCOLOR_ROW).Cells.Item(nColumn).Value.ToString
                Dim sCustom As String = mdgvData.Rows(mnCUSTOMPARM_ROW).Cells.Item(nColumn).Value.ToString
                Dim sZone As String = mdgvData.Rows(mnZONE_ROW).Cells.Item(nColumn).Value.ToString
                Dim Zone As clsZone = colZones(sZone)
                Dim sDevice As String = sZone
                Dim Arm As clsArm = Nothing
                If mtTabCfgs(mnTab).bUseArm Then
                    sDevice = mdgvData.Rows(mnROBOT_ROW).Cells.Item(nColumn).Value.ToString
                    Arm = colArms.Item(sDevice)
                ElseIf mtTabCfgs(mnTab).bUseController Then
                    sDevice = mdgvData.Rows(mnCONTROLLER_ROW).Cells.Item(nColumn).Value.ToString
                    Arm = colArms.Item(colControllers.Item(sDevice).Arms(0).Name)
                End If
                mdgvData.Enabled = True
                Progress = 30 * nColumn \ mdgvData.ColumnCount
                For nRow As Integer = mnITEM0_ROW To mtTabCfgs(mnTab).ItemCfg.GetUpperBound(0) + mnITEM0_ROW
                    Dim oCell As DataGridViewCell = DirectCast(mdgvData.Rows(nRow).Cells.Item(nColumn), DataGridViewCell)
                    Dim bDiff As Boolean = False
                    Dim sOld As String = String.Empty

                    If Not oCell.Value.ToString = gpsRM.GetString("psNA") Then 'RJO 02/03/14 Do not try to save "N/A" cells.

                        With mtTabCfgs(mnTab).ItemCfg(nRow - mnITEM0_ROW)
                            Select Case .DataSource
                                Case eDataSource.Robot
                                    Select Case .ProgName(0).ToLower
                                        Case "*numreg*"
                                            Dim oVar As FRRobot.FRCRegNumeric = DirectCast(oCell.Tag, FRRobot.FRCRegNumeric)
                                            sOld = mRegVal.ReadNumReg(oVar)
                                            bDiff = (sOld <> oCell.Value.ToString)
                                        Case "*strreg*"
                                            Dim oVar As FRRobot.FRCRegString = DirectCast(oCell.Tag, FRRobot.FRCRegString)
                                            sOld = mRegVal.ReadStringReg(oVar)
                                            bDiff = (sOld <> oCell.Value.ToString)
                                        Case Else 'program or system variable
                                            Dim oVar As FRRobot.FRCVar = DirectCast(oCell.Tag, FRRobot.FRCVar)
                                            If oVar.IsInitialized Then
                                                sOld = oVar.Value.ToString
                                            Else
                                                sOld = gpsRM.GetString("psUNINIT")
                                            End If
                                    End Select
                                    If (.bCbo) Then
                                        If (.DataType Is GetType(Boolean)) And .CustomCBO.GetUpperBound(0) = 0 Then 'RJO 12/14/11
                                            Dim bNew As Boolean = False
                                            Dim bOld As Boolean = False
                                            Select Case oCell.Value.ToString.ToLower
                                                Case "yes", "true", gpsRM.GetString("psYES").ToLower, gpsRM.GetString("psTRUE").ToLower
                                                    bNew = True
                                                Case "no", "false", gpsRM.GetString("psNO").ToLower, gpsRM.GetString("psFALSE").ToLower
                                                    bNew = False
                                            End Select
                                            Select Case sOld.ToLower
                                                Case "yes", "true", gpsRM.GetString("psYES").ToLower, gpsRM.GetString("psTRUE").ToLower
                                                    bOld = True
                                                Case "no", "false", gpsRM.GetString("psNO").ToLower, gpsRM.GetString("psFALSE").ToLower
                                                    bOld = False
                                            End Select
                                            bDiff = (bOld <> bNew)
                                        Else
                                            'Search custom CBO data 
                                            Dim sNewVal As String = oCell.Value.ToString
                                            subGetRobotValue(mtTabCfgs(mnTab).ItemCfg(nRow - mnITEM0_ROW), sOld, oCell.Tag, Arm, _
                                                       nColor, nValve, sCustom)
                                            Dim nItem As Integer = 1
                                            Dim bFound As Boolean = False
                                            Do While (nItem <= .CustomCBO.GetUpperBound(0)) And (bFound = False)
                                                If .CustomCBO(nItem) = sOld Then
                                                    sOld = .CustomCBO(nItem - 1)
                                                    bFound = True
                                                Else
                                                    nItem += 2
                                                End If
                                            Loop
                                            bDiff = (sOld <> oCell.Value.ToString)
                                        End If
                                    Else
                                        bDiff = (sOld <> oCell.Value.ToString)
                                    End If

                                Case Else 'Anything but robot data
                                    sOld = oCell.Tag.ToString
                                    bDiff = (sOld <> oCell.Value.ToString)
                            End Select
                        End With
                        If bDiff Then
                            With mtTabCfgs(mnTab).ItemCfg(nRow - mnITEM0_ROW)
                                Select Case .DataSource
                                    Case eDataSource.DB
                                        subSetDBValue(mtTabCfgs(mnTab).ItemCfg(nRow - mnITEM0_ROW), oCell)
                                    Case eDataSource.XML
                                        subSetXMLValue(mtTabCfgs(mnTab).ItemCfg(nRow - mnITEM0_ROW), oCell)
                                    Case eDataSource.PLC
                                        subSetPLCValue(mtTabCfgs(mnTab).ItemCfg(nRow - mnITEM0_ROW), oCell, Arm, _
                                                        nColor, nValve, sCustom, Zone) 'MSW 7/27/10 send the right custom cbo value
                                    Case eDataSource.Registry
                                        subSetRegistryValue(mtTabCfgs(mnTab).ItemCfg(nRow - mnITEM0_ROW), oCell)
                                    Case eDataSource.Robot
                                        subSetRobotValue(mtTabCfgs(mnTab).ItemCfg(nRow - mnITEM0_ROW), oCell, Arm, _
                                                        nColor, nValve, sCustom) 'MSW 7/27/10 send the right custom cbo value
                                    Case Else
                                        'Problem
                                End Select
                                '           Changed item  from old to new 
                                sChange = gpsRM.GetString("psCHANGED") & .Label & gpsRM.GetString("psFROM") & _
                                        sOld & gpsRM.GetString("psTO") & oCell.Value.ToString
                                AddChangeRecordToCollection(msSCREEN_NAME, LoggedOnUser, _
                                        colZones.CurrentZoneNumber, sDevice, sColor, sChange, sZone)
                            End With

                        End If
                        oCell.Style.ForeColor = Color.Black
                        oCell.Style.SelectionForeColor = Color.White
                    End If

                Next


                EditsMade = False

                Progress = 90 * nColumn \ mdgvData.ColumnCount
            Next
            'mPWCommon.SaveToChangeLog(colZones.DatabasePath)
            'For SQL database - remove above eventually
            mPWCommon.SaveToChangeLog(colZones.ActiveZone)
            ' save done
            Status = gcsRM.GetString("csSAVE_DONE")
            'subShowNewPage()
            subEnableControls(True)
            Progress = 0


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
        ' 11/01/10  MSW     Don't try to pick a device for the change log.  
        ' 04/16/13  MSW     Standalone ChangeLog                                            4.1.5.0
        '********************************************************************************************

        Try
            mPWCommon.subShowChangeLog(colZones.CurrentZone, msSCREEN_NAME, msSCREEN_NAME, gcsRM.GetString("csALL"), _
                                          cboColor.Text, mDeclares.eParamType.None, nIndex, mbUSEROBOTS, True, oIPC)

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
        '********************************************************************************************
        Try
            mbSimpleViewAll = False
            If bAskForSave() Then
                If btnMultiView.Checked Then
                    btnMultiView.Checked = False
                    btnAdd.Visible = False
                    btnClear.Visible = False
                    If DataLoaded Then
                        mdgvData.ColumnCount = 1
                        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
                        subEnableControls(False)
                        Status = gcsRM.GetString("csLOADINGDATA")
                        bLoadData()
                        mdgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
                        mdgvData.AutoResizeColumns()
                        mdgvData.Enabled = True
                        'subFormatScreenLayout()
                        EditsMade = False
                        Me.Cursor = System.Windows.Forms.Cursors.Default
                        Status(True) = gpsRM.GetString("csREADY")
                    End If
                    If mtTabCfgs(mnTab).sToon <> String.Empty Then
                        If (btnMultiView.Visible = False) Or (btnMultiView.Checked = False) Then
                            spltMain.Panel2Collapsed = False
                        End If
                    End If
                Else
                    btnMultiView.Checked = True
                    btnAdd.Visible = True
                    btnClear.Visible = True
                    spltMain.Panel2Collapsed = True
                End If
                subEnableControls(True)
            End If
        Catch ex As Exception

        End Try

    End Sub
    'Private Sub subShowNewPage()
    '    '********************************************************************************************
    '    'Description:  Data is all loaded or changed and screen needs to be refreshed
    '    '
    '    'Parameters: none
    '    'Returns:    none
    '    '
    '    'Modification history:
    '    '
    '    ' Date      By      Reason
    '    '********************************************************************************************
    '    'Not needed for this screen

    'End Sub

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

        Dim dtStart As DateTime = Now
        If MessageBox.Show(gcsRM.GetString("csUNDOMSG"), msSCREEN_NAME, MessageBoxButtons.OKCancel, _
                                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) _
                                                                    = Response.OK Then
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subEnableControls(False)
            Status = gcsRM.GetString("csLOADINGDATA")
            If DataLoaded Then
                For nCol As Integer = 0 To mdgvData.ColumnCount - 1
                    bLoadData(True, nCol, True)
                Next
            End If
            mdgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
            mdgvData.AutoResizeColumns()
            mdgvData.Enabled = True
            'subFormatScreenLayout()
            EditsMade = False
            Me.Cursor = System.Windows.Forms.Cursors.Default
            Status(True) = gpsRM.GetString("csREADY")
        End If
        Dim dtEnd As DateTime = Now
        Dim nSec As Integer = dtEnd.Second - dtStart.Second
        If nSec < 0 Then
            nSec = nSec + 60
        End If
        Dim nMS As Integer = dtEnd.Millisecond - dtStart.Millisecond
        If nMS < 0 Then
            nMS = nMS + 1000
            nSec = nSec - 1
        End If
        Debug.Print(nSec.ToString & "." & nMS.ToString("000"))

    End Sub
    Private Sub subRefreshData()
        '********************************************************************************************
        'Description:  Refresh Button Pressed
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        tmrRefresh.Enabled = False
        Dim dtStart As DateTime = Now

        If mbAbortForShutdown Then
            Exit Sub
        End If
        'Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        'subEnableControls(False)
        Status = gcsRM.GetString("csLOADINGDATA")
        For nCol As Integer = 0 To mdgvData.ColumnCount - 1
            bLoadData(True, nCol, False)
        Next
        mdgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        mdgvData.AutoResizeColumns()
        mdgvData.Enabled = True
        mnRefreshRotation = mnRefreshRotation + 1
        If (mnRefreshRotation < 0) Or (mnRefreshRotation > 3) Then
            mnRefreshRotation = 0
        Else
        End If
        btnRefresh.Image = DirectCast(gpsRM.GetObject("RefreshBlue" & mnRefreshRotation.ToString, mLanguage.FixedCulture), Image)
        'subFormatScreenLayout()
        'Me.Cursor = System.Windows.Forms.Cursors.Default
        'Status(True) = gpsRM.GetString("csREADY")
        'subEnableControls(True)
        Dim dtEnd As DateTime = Now
        Dim nSec As Integer = dtEnd.Second - dtStart.Second
        If nSec < 0 Then
            nSec = nSec + 60
        End If
        Dim nMS As Integer = dtEnd.Millisecond - dtStart.Millisecond
        If nMS < 0 Then
            nMS = nMS + 1000
            nSec = nSec - 1
        End If
        Debug.Print(nSec.ToString & "." & nMS.ToString("000"))
        Application.DoEvents()
        tmrRefresh.Enabled = mnuAutoRefresh.Checked
    End Sub

#End Region
#Region " Events "

    Private Sub frmMain_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed

    End Sub

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '********************************************************************************************
        'Description: Shutdown.  See if it needs a save
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/04/10  MSW     Let cancel button work in bAskForSave popup
        ' 11/08/11  MSW     add some handling for a shutdown in the middle of a load (mbAbortForShutdown)
        '********************************************************************************************
        If Not (bAskForSave()) Then
            e.Cancel = True
        Else
            mbAbortForShutdown = True
            subClearSARequests()
        End If
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
                tmrRefresh.Enabled = False
                Me.Close()
                'End
            Case "btnStatus"
                lstStatus.Visible = Not lstStatus.Visible

            Case "btnSave"
                tmrRefresh.Enabled = False
                'give validate a chance to fire
                Application.DoEvents()
                'privilege check done in subroutine
                subSaveData()
                Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
                subEnableControls(False)
                Status = gcsRM.GetString("csLOADINGDATA")
                For nCol As Integer = 0 To mdgvData.ColumnCount - 1
                    bLoadData(True, nCol, True)
                Next
                mdgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
                mdgvData.AutoResizeColumns()
                mdgvData.Enabled = True
                'subFormatScreenLayout()
                EditsMade = False
                Me.Cursor = System.Windows.Forms.Cursors.Default
                Status(True) = gpsRM.GetString("csREADY")

                Application.DoEvents()
                tmrRefresh.Enabled = mnuAutoRefresh.Checked
            Case "btnPrint"
                tmrRefresh.Enabled = False
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                bPrintdoc(True)
                Application.DoEvents()
                tmrRefresh.Enabled = mnuAutoRefresh.Checked
            Case "btnUndo"
                tmrRefresh.Enabled = False
                Select Case Privilege
                    Case ePrivilege.None
                        'logout occured while screen open
                        subEnableControls(False)
                    Case Else
                        subUndoData()
                End Select
                Application.DoEvents()
                tmrRefresh.Enabled = mnuAutoRefresh.Checked
            Case "btnRefresh"
                If mnuAutoRefresh.Checked Then
                    mnuAutoRefresh.Checked = False
                    btnRefresh.Image = DirectCast(gpsRM.GetObject("Refresh", mLanguage.FixedCulture), Image)
                    tmrRefresh.Enabled = False
                Else
                    Application.DoEvents()
                    mnuAutoRefresh.Checked = True
                    mnRefreshRotation = -1
                    Application.DoEvents()
                    subRefreshData()
                End If
            Case "btnChangeLog"
                tmrRefresh.Enabled = False
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                subShowChangeLog(CType(eChangeMnu.Hours24, Integer))

            Case "btnCopy"
                'Select Case Privilege
                '    Case ePrivilege.None
                '        'logout occured while screen open
                '        subEnableControls(False)
                '    Case Else
                '        subCopyButtonPressed()
                'End Select
            Case "btnMultiView"
                tmrRefresh.Enabled = False
                subShowMultiView()
            Case "btnAdd"
                tmrRefresh.Enabled = False
                'Check for dropdown
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                'Drop-down not selected, but open it if not all cbos are selected
                '0 = not used, 1= cbo selected, 2 = cbo empty
                Dim nRobot As Integer = 0
                Dim nColor As Integer = 0
                Dim nCustom As Integer = 0
                If mtTabCfgs(mnTab).bUseArm Or mtTabCfgs(mnTab).bUseController Then
                    If cboRobot.Text = String.Empty Then
                        nRobot = 2
                    Else
                        nRobot = 1
                    End If
                End If
                If mtTabCfgs(mnTab).bUseValve Or mtTabCfgs(mnTab).bUseColor Then
                    If cboColor.Text = String.Empty Then
                        nRobot = 2
                    Else
                        nRobot = 1
                    End If
                End If
                If mtTabCfgs(mnTab).bUseCustomParm Then
                    If cboParm.Text = String.Empty Then
                        nRobot = 2
                    Else
                        nRobot = 1
                    End If
                End If
                If ((nRobot < 2) And (nColor < 2) And (nCustom < 2)) Then
                    'enabled for single load
                    Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
                    subEnableControls(False)
                    Status = gcsRM.GetString("csLOADINGDATA")
                    bLoadData()
                    mdgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
                    mdgvData.AutoResizeColumns()
                    mdgvData.Enabled = True
                    'subFormatScreenLayout()
                    EditsMade = False
                    Me.Cursor = System.Windows.Forms.Cursors.Default
                    Status(True) = gpsRM.GetString("csREADY")
                Else
                    'Open the drop-down-menu
                    o.ShowDropDown()
                End If
            Case "btnClear"
                tmrRefresh.Enabled = False
                subClearMultiView()
            Case btnGraph.Name
                btnGraph.ShowDropDown()
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
        mbEventBlocker = True
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
    Private Sub cboZone_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboZone.SelectedIndexChanged

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
    Private Sub cboParm_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboParm.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Applicator Parameter Combo Changed 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If cboParm.Text <> msOldParm Then
            ' so we don't go off half painted
            Me.Refresh()
            subChangeParm()
        End If

    End Sub
    Private Sub cboColor_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)

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
        Dim nHeight As Integer = tscMain.ContentPanel.Height
        If nHeight < 100 Then nHeight = 100
        Dim nWidth As Integer = tscMain.ContentPanel.Width
        If nWidth < 100 Then nWidth = 100
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

            'If moPrivilege.Privilege = String.Empty Then 'RJO 03/15/12
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
                Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
            End If
        End If
    End Sub
    Private Sub moPassword_LogOut() Handles moPassword.Logout
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
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone)
        End If

    End Sub

#End Region


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
        '    11/16/09   msw     Add multiview help files
        '    04/08/10   RJO     Changed screenshot image type to .bmp to be consistant with other PW
        '                       applications.
        '    09/30/13   MSW     Save screenshots as jpegs
        '********************************************************************************************
        Dim sKeyValue As String = String.Empty

        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode
                Case Keys.F1
                    'Help Screen request
                    sKeyValue = "btnHelp"
                    If btnMultiView.Checked Then
                        Dim nTmp As Integer = InStr(mtTabCfgs(mnTab).sHelpFile, ".htm")
                        Dim sTmp As String = mtTabCfgs(mnTab).sHelpFile
                        If nTmp > 0 Then
                            sTmp = sTmp.Substring(0, nTmp - 1) & "_MV" & sTmp.Substring(nTmp - 1)
                        End If
                        subLaunchHelp(sTmp, oIPC)
                    Else
                        subLaunchHelp(mtTabCfgs(mnTab).sHelpFile, oIPC)
                    End If
                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty

                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    If btnMultiView.Checked Then
                        sDumpPath = sDumpPath & msSCREEN_DUMP_NAME & "_MV" & msSCREEN_DUMP_EXT
                    Else
                        sDumpPath = sDumpPath & msSCREEN_DUMP_NAME & msSCREEN_DUMP_EXT
                    End If
                    'oSC.CaptureWindowToFile(Me.Handle, sDumpPath, Imaging.ImageFormat.Jpeg) 'RJO 04/08/10
                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath, Imaging.ImageFormat.Jpeg)

                Case Keys.Escape
                    Me.Close()

                Case Else

            End Select
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
        subChangeTab()  'subChangeTab() RV 10/6/16
    End Sub

    Private Sub mnu_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles mnuYesNo.Opening
        '********************************************************************************************
        'Description:  Menu handlers - boolean pop-up is attached at design time, custom menus get added when they're made
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If Privilege = ePrivilege.None Then
            e.Cancel = True
        End If
    End Sub

    Private Sub mnu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuYes.Click, mnuNo.Click
        '********************************************************************************************
        'Description:  Menu handlers - boolean pop-up is attached at design time, custom menus get added when they're made
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oCell As DataGridViewCell = DirectCast(mdgvData.Rows(mnDataGridSelRow).Cells.Item(mnDataGridSelCol), DataGridViewCell)
        Dim oMnu As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim sTag As String = DirectCast(oMnu.Tag, String)
        Dim sText As String = oMnu.Text
        If (sText <> oCell.Value.ToString) Then
            If (oCell.ValueType Is GetType(String)) Then
                oCell.Value = sText
            ElseIf (oCell.ValueType Is GetType(Integer)) Then
                If IsNumeric(sTag) Then
                    oCell.Value = CType(sText, Integer)
                Else
                    oCell.Value = 0
                End If
            ElseIf (oCell.ValueType Is GetType(Double)) Then
                If IsNumeric(sTag) Then
                    oCell.Value = CType(sText, Double)
                Else
                    oCell.Value = 0
                End If
            Else
                Debug.Assert(False)
            End If
            Dim oVar As FRRobot.FRCVar = DirectCast(oCell.Tag, FRRobot.FRCVar)
            Dim sVal As String = String.Empty
            If oVar IsNot Nothing Then
                sVal = oVar.Value.ToString
            Else
                sVal = oCell.Tag.ToString
            End If
            If (sVal <> oCell.Value.ToString) Then
                oCell.Style.ForeColor = Color.Red
                oCell.Style.SelectionForeColor = Color.Red
                EditsMade = True
            End If
        End If
    End Sub

    Private Sub mdgvData_CellMouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles mdgvData.CellMouseDown
        '********************************************************************************************
        'Description: mouse-down - record the column and index for the pop-up menus
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mnDataGridSelCol = e.ColumnIndex
        mnDataGridSelRow = e.RowIndex
        mnMouseDownX = e.X
        mnMouseDownY = e.Y
    End Sub


    Private Sub mdgvData_CellEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles mdgvData.CellEnter
        '********************************************************************************************
        'Description: mouse-down - record the column and index for the pop-up menus
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************        
        If mbEventBlocker Then Exit Sub
        mnDataGridSelCol = e.ColumnIndex
        mnDataGridSelRow = e.RowIndex
        If e.ColumnIndex >= 0 Then
            If e.RowIndex >= mnITEM0_ROW Then
                If Privilege > ePrivilege.None Then
                    Dim oCell As DataGridViewCell = DirectCast(mdgvData.Rows(mnDataGridSelRow).Cells.Item(mnDataGridSelCol), DataGridViewCell)
                    If Not (oCell.Tag Is Nothing) Then 'cell not exluded because of opener
                        If (mtTabCfgs(mnTab).ItemCfg(e.RowIndex - mnITEM0_ROW).bEdit) Then
                            If (mtTabCfgs(mnTab).ItemCfg(e.RowIndex - mnITEM0_ROW).bCbo) Then
                                'wait for a keypress
                            Else
                                'Not a menu cell, so start edit
                                If (oCell.IsInEditMode = False) And (oCell.ReadOnly = False) Then
                                    mdgvData.BeginEdit(True)
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If
    End Sub
    Private Sub mdgvData_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles mdgvData.CellClick
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
        mnDataGridSelCol = e.ColumnIndex
        mnDataGridSelRow = e.RowIndex

        If e.ColumnIndex >= 0 Then
            If e.RowIndex >= mnITEM0_ROW Then
                If Privilege > ePrivilege.None Then
                    Dim oCell As DataGridViewCell = DirectCast(mdgvData.Rows(mnDataGridSelRow).Cells.Item(mnDataGridSelCol), DataGridViewCell)
                    If Not (oCell.Tag Is Nothing) Then 'cell not exluded because of opener
                        If (mtTabCfgs(mnTab).ItemCfg(e.RowIndex - mnITEM0_ROW).bEdit) Then
                            If (mtTabCfgs(mnTab).ItemCfg(e.RowIndex - mnITEM0_ROW).bCbo) And (oCell.ContextMenuStrip IsNot Nothing) Then
                                Dim oPoint As Point = New Point((mnMouseDownX + mdgvData.RowHeadersWidth), _
                                                                    (mnMouseDownY))
                                Dim nIdx As Integer = 0
                                Do While nIdx < mnDataGridSelCol
                                    oPoint.X += mdgvData.Columns(nIdx).Width
                                    nIdx += 1
                                Loop
                                nIdx = 0
                                Do While nIdx < mnDataGridSelRow
                                    If mdgvData.Rows(nIdx).Visible Then
                                        oPoint.Y += mdgvData.Rows(nIdx).Height
                                    End If
                                    nIdx += 1
                                Loop
                                oCell.ContextMenuStrip.Show(mdgvData.PointToScreen(oPoint))
                            Else
                                'Not a menu cell, so start edit
                                If (oCell.IsInEditMode = False) And (oCell.ReadOnly = False) Then
                                    mdgvData.BeginEdit(True)
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub mdgvData_DataError(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles mdgvData.DataError
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
        Dim sVal As String = mdgvData.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString
        Dim lRet As DialogResult
        lRet = MessageBox.Show(gcsRM.GetString("csINV_ENTRY"), gcsRM.GetString("csERROR"), _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        mdgvData.CancelEdit()
    End Sub
    Private Sub mdgvData_CellBeginEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles mdgvData.CellBeginEdit
        '********************************************************************************************
        'Description:  Start edit - save the old value
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        msOldEditVal = mdgvData.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString
    End Sub
    Private Sub mdgvData_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles mdgvData.CellEndEdit
        '********************************************************************************************
        'Description: end edit
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        'Check for invalid data
        Dim oCell As DataGridViewCell = DirectCast(mdgvData.Rows(e.RowIndex).Cells.Item(e.ColumnIndex), DataGridViewCell)
        Dim sVal As String = oCell.Value.ToString
        Dim bRestore As Boolean = (Privilege = ePrivilege.None)
        With mtTabCfgs(mnTab).ItemCfg(e.RowIndex - mnITEM0_ROW)
            If (.DataType Is GetType(Integer)) OrElse (.DataType Is GetType(Double)) Then
                If Not (bRestore) Then
                    'Numeric, check min/max
                    If IsNumeric(sVal) Then
                        Dim nTmp As Double = CType(sVal, Double)
                        Dim lRet As DialogResult
                        If nTmp > .Max Then
                            lRet = MessageBox.Show(gcsRM.GetString("csVAL_ABOVE_MAX"), gcsRM.GetString("csERROR"), _
                                                MessageBoxButtons.OK, _
                                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                            bRestore = True
                        End If
                        If nTmp < .Min Then
                            lRet = MessageBox.Show(gcsRM.GetString("csVAL_BELOW_MIN"), gcsRM.GetString("csERROR"), _
                                                MessageBoxButtons.OK, _
                                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                            bRestore = True
                        End If
                    End If
                End If
                If bRestore Then
                    If (.DataType Is GetType(Integer)) Then
                        mdgvData.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = CType(msOldEditVal, Integer)
                    End If
                    If (.DataType Is GetType(Double)) Then
                        mdgvData.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = CType(msOldEditVal, Double)
                    End If
                End If
            Else
                If bRestore Then
                    mdgvData.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = msOldEditVal
                End If
            End If
        End With
        Dim sOldVal As String = String.Empty

        If (TypeOf oCell.Tag Is String) OrElse (TypeOf oCell.Tag Is Integer) OrElse _
            (TypeOf oCell.Tag Is Boolean) OrElse (TypeOf oCell.Tag Is Double) OrElse _
            (TypeOf oCell.Tag Is Single) OrElse (TypeOf oCell.Tag Is Short) OrElse _
            (TypeOf oCell.Tag Is Byte) OrElse (TypeOf oCell.Tag Is Long) Then
            sOldVal = oCell.Tag.ToString
        Else
            Try
                Dim oVar As FRRobot.FRCVar = DirectCast(oCell.Tag, FRRobot.FRCVar)
                If oVar IsNot Nothing Then
                    If oVar IsNot Nothing Then
                        If oVar.IsInitialized Then
                            sOldVal = oVar.Value.ToString
                        Else
                            sOldVal = "****"
                        End If
                    End If
                Else
                    Dim oReg As FRRobot.FRCRegNumeric = DirectCast(oCell.Tag, FRRobot.FRCRegNumeric)
                    If oReg IsNot Nothing Then
                        sOldVal = mRegVal.ReadNumReg(oReg)
                    Else
                        Dim oRegStr As FRRobot.FRCRegString = DirectCast(oCell.Tag, FRRobot.FRCRegString)
                        If oRegStr IsNot Nothing Then
                            sOldVal = mRegVal.ReadStringReg(oRegStr)
                        End If
                    End If
                End If

            Catch ex As Exception
                mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: mdgvData_CellEndEdit", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End If

        'Mark changes in red
        If (sOldVal <> oCell.Value.ToString) Then

            mdgvData.Rows(e.RowIndex).Cells(e.ColumnIndex).Style.ForeColor = Color.Red
            mdgvData.Rows(e.RowIndex).Cells(e.ColumnIndex).Style.SelectionForeColor = Color.Red
            EditsMade = True
        End If

    End Sub


    Private Sub mdgvData_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles mdgvData.KeyDown
        '********************************************************************************************
        'Description:  keydown on grid control
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbAbortForShutdown Then
            Exit Sub
        End If
        'Track the selected cell
        mnDataGridSelRow = mdgvData.SelectedCells(0).RowIndex
        mnDataGridSelCol = mdgvData.SelectedCells(0).ColumnIndex

        'If it's already edited then the control is handling the keys
        If Not (mdgvData.IsCurrentCellInEditMode) Then
            'which key
            If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
                Select Case e.KeyCode
                    Case Keys.Enter, Keys.Space, Keys.Menu, Keys.RMenu
                        'get the current cell, see should get a pop-up menu
                        Dim oCell As DataGridViewCell = DirectCast(mdgvData.Rows(mnDataGridSelRow).Cells.Item(mnDataGridSelCol), DataGridViewCell)
                        If Not (oCell.IsInEditMode) Then
                            With mtTabCfgs(mnTab).ItemCfg(mnDataGridSelRow - mnITEM0_ROW)
                                If (oCell.Tag IsNot Nothing) Then 'cell not exluded because of opener
                                    If .bCbo And .bEdit And (oCell.ContextMenuStrip IsNot Nothing) Then
                                        Dim oPoint As Point = New Point( _
                                            ((mdgvData.Columns(mnDataGridSelCol).Width \ 2) + mdgvData.RowHeadersWidth), _
                                            (mdgvData.Rows(mnDataGridSelRow).Height \ 2))
                                        Dim nIdx As Integer = 0
                                        Do While nIdx < mnDataGridSelCol
                                            oPoint.X += mdgvData.Columns(nIdx).Width
                                            nIdx += 1
                                        Loop
                                        nIdx = 0
                                        Do While nIdx < mnDataGridSelRow
                                            If mdgvData.Rows(nIdx).Visible Then
                                                oPoint.Y += mdgvData.Rows(nIdx).Height
                                            End If
                                            nIdx += 1
                                        Loop
                                        oCell.ContextMenuStrip.Show(mdgvData.PointToScreen(oPoint))
                                    End If
                                End If
                            End With
                        End If
                    Case Else

                End Select
            End If

        End If
    End Sub

    Private Sub mnuAddAll_Click(ByVal sender As System.Object, Optional ByVal e As System.EventArgs = Nothing) Handles _
                mnuAddAllRobots.Click, mnuAddAllColors.Click, mnuAddAllCustom.Click, _
                mnuAddAllPainters.Click, mnuAddAllOpeners.Click
        '********************************************************************************************
        'Description:  add all selected
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim dtStart As DateTime = Now
        Dim oMnu As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim oCbo As ComboBox = Nothing
        Dim nMaskRobots As Integer = 0
        Dim bClear As Boolean = False ' For simple tabs with only one cbo, clear first
        Dim bContinue As Boolean = True
        Dim bAllOneRobot As Boolean = True 'if there are repeated loads on one robot, make sure
        mbSimpleViewAll = False
        '   the "robot not connectected" messagebox only pops up once
        Select Case oMnu.Name
            Case "mnuAddAllRobots"
                oCbo = DirectCast(pnlTop.Controls("cboRobot"), ComboBox)
                bClear = mtTabCfgs(mnTab).bUseColor Or mtTabCfgs(mnTab).bUseValve Or _
                            mtTabCfgs(mnTab).bUseCustomParm
                bAllOneRobot = False
                nMaskRobots = 0
                mbSimpleViewAll = Not (bClear)
            Case "mnuAddAllPainters"
                oCbo = DirectCast(pnlTop.Controls("cboRobot"), ComboBox)
                bClear = mtTabCfgs(mnTab).bUseColor Or mtTabCfgs(mnTab).bUseValve Or _
                            mtTabCfgs(mnTab).bUseCustomParm
                bAllOneRobot = False
                nMaskRobots = 1
            Case "mnuAddAllOpeners"
                oCbo = DirectCast(pnlTop.Controls("cboRobot"), ComboBox)
                bClear = mtTabCfgs(mnTab).bUseColor Or mtTabCfgs(mnTab).bUseValve Or _
                            mtTabCfgs(mnTab).bUseCustomParm
                bAllOneRobot = False
                nMaskRobots = 2
            Case "mnuAddAllColors"
                oCbo = DirectCast(pnlTop.Controls("cboColor"), ComboBox)
                bClear = mtTabCfgs(mnTab).bUseArm Or mtTabCfgs(mnTab).bUseController Or _
                            mtTabCfgs(mnTab).bUseCustomParm
                bAllOneRobot = mtTabCfgs(mnTab).bUseArm Or mtTabCfgs(mnTab).bUseController
            Case "mnuAddAllCustom"
                oCbo = DirectCast(pnlTop.Controls("cboParm"), ComboBox)
                bClear = mtTabCfgs(mnTab).bUseArm Or mtTabCfgs(mnTab).bUseController Or _
                            mtTabCfgs(mnTab).bUseColor Or mtTabCfgs(mnTab).bUseValve
                bAllOneRobot = mtTabCfgs(mnTab).bUseArm Or mtTabCfgs(mnTab).bUseController
            Case Else
                bContinue = False
        End Select

        If Not (bClear) Then
            bContinue = bAskForSave()
            If bContinue Then
                'This is a simple tab, so clear all the existing items when "show all" is selected
                mdgvData.ColumnCount = 1
                For nRow As Integer = 0 To mdgvData.RowCount - 1
                    mdgvData.Rows(nRow).Cells(0).Value = String.Empty
                Next
                EditsMade = False
                DataLoaded = False
            End If
        End If
        If bContinue Then
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            subEnableControls(False)
            Status = gcsRM.GetString("csLOADINGDATA")
            For nItem As Integer = 0 To oCbo.Items.Count - 1
                Select Case nMaskRobots
                    Case 0
                        oCbo.SelectedIndex = nItem
                        Application.DoEvents()
                        If Not (bLoadData()) Then
                            If bAllOneRobot Then
                                Exit For
                            End If
                        End If
                    Case 1
                        If colArms.Item(oCbo.Items(nItem).ToString).IsOpener = False Then
                            oCbo.SelectedIndex = nItem
                            Application.DoEvents()
                            If Not (bLoadData()) Then
                                If bAllOneRobot Then
                                    Exit For
                                End If
                            End If
                        End If
                    Case 2
                        If colArms.Item(oCbo.Items(nItem).ToString).IsOpener = True Then
                            oCbo.SelectedIndex = nItem
                            Application.DoEvents()
                            If Not (bLoadData()) Then
                                If bAllOneRobot Then
                                    Exit For
                                End If
                            End If
                        End If
                End Select
            Next
            If mbAbortForShutdown Then
                Exit Sub
            End If
            'subFormatScreenLayout()
            mdgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
            mdgvData.AutoResizeColumns()
            mdgvData.Enabled = True
            EditsMade = False
            Me.Cursor = System.Windows.Forms.Cursors.Default
            Status(True) = gpsRM.GetString("csREADY")
            subEnableControls(True)
        End If
        Dim dtEnd As DateTime = Now
        Dim nSec As Integer = dtEnd.Second - dtStart.Second
        If nSec < 0 Then
            nSec = nSec + 60
        End If
        Dim nMS As Integer = dtEnd.Millisecond - dtStart.Millisecond
        If nMS < 0 Then
            nMS = nMS + 1000
            nSec = nSec - 1
        End If
        Debug.Print(nSec.ToString & "." & nMS.ToString("000"))

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
        If (mdgvData IsNot Nothing) AndAlso (mnTab >= 0) AndAlso DataLoaded Then
            bPrintdoc(True)
            mPrintHtml.subShowPageSetup()
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
        bPrintdoc(True)

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
    Private Sub mnuGraph_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  Launch the live graph view
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/10/13  MSW     Add Graph function for sealer
        '********************************************************************************************
        'First sort out names
        Dim oMenuItem As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim sItem As String = oMenuItem.Text
        If DataLoaded = False Then
            Exit Sub
        End If
        If moColParam Is Nothing Then
            Exit Sub
        End If

        For Each oGraph As tGraphConfig In mtTabCfgs(mnTab).oGraphs
            If sItem = oGraph.sGraphName Then
                'Found the right graph, setup the graph form:
                'Clear optional axis labels
                frmGraph.AxisUnitLabel(0) = String.Empty
                frmGraph.AxisUnitLabel(1) = String.Empty
                'set lines
                frmGraph.NumLines = oGraph.ItemNames.GetUpperBound(0) + 1
                frmGraph.Title = moColParam(0).oZone.Name & " - " & moColParam(0).oArm.Name & " - " & sItem
                For nItem As Integer = oGraph.ItemNames.GetLowerBound(0) To oGraph.ItemNames.GetUpperBound(0)
                    'rotate up to 4 colors
                    Select Case nItem Mod 4
                        Case 0
                            frmGraph.LineColor(nItem) = Color.Red
                        Case 1
                            frmGraph.LineColor(nItem) = Color.Blue
                        Case 2
                            frmGraph.LineColor(nItem) = Color.Green
                        Case 3
                            frmGraph.LineColor(nItem) = Color.White
                    End Select
                    'Set the label
                    frmGraph.LineLabel(nItem) = oGraph.ItemNames(nItem)
                    'setup vertical axis - two available
                    If nItem = 0 Then
                        frmGraph.AxisUnitLabel(0) = oGraph.ItemUnits(0)
                        frmGraph.YAxis(nItem) = 0
                    Else
                        If oGraph.ItemUnits(nItem) = frmGraph.AxisUnitLabel(0) Then
                            frmGraph.YAxis(nItem) = 0
                        ElseIf oGraph.ItemUnits(nItem) = frmGraph.AxisUnitLabel(1) Then
                            frmGraph.YAxis(nItem) = 1
                        ElseIf frmGraph.AxisUnitLabel(1) = String.Empty Then
                            frmGraph.AxisUnitLabel(1) = oGraph.ItemUnits(nItem)
                            frmGraph.YAxis(nItem) = 1
                        Else
                            frmGraph.YAxis(nItem) = 0
                        End If
                    End If
                    frmGraph.MaxScale(nItem) = oGraph.ItemMax(nItem)
                    frmGraph.MinScale(nItem) = oGraph.ItemMin(nItem)
                Next

                moGraph = oGraph
                'Show the graph
                frmGraph.Show()
                tmrGraph.Enabled = True
            End If
        Next
    End Sub

    Private Sub tmrGraph_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrGraph.Tick
        '********************************************************************************************
        'Description:  Launch the live graph view
        '
        'Parameters:  
        'Returns:   
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/10/13  MSW     Add Graph function for sealer
        '********************************************************************************************

        tmrGraph.Enabled = False
        Try

            If frmGraph.Visible Then
                Dim nMax As Integer = moGraph.ItemNames.GetUpperBound(0)
                Dim sGraphData(nMax) As String
                For nItem As Integer = 0 To nMax
                    sGraphData(nItem) = ""
                    Select Case moGraph.ItemCfg(nItem).DataSource
                        'Case eDataSource.DB
                        '    subGetDBValue(moGraph.ItemCfg(nItem), oCell)
                        'Case eDataSource.XML
                        '    subGetXMLValue(moGraph.ItemCfg(nItem), oCell)
                        'Case eDataSource.Registry
                        '    subGetRegistryValue(moGraph.ItemCfg(nItem), oCell)
                        'Case eDataSource.PLC
                        '    subGetPLCValue(moGraph.ItemCfg(nItem), sGraphData(nItem), moGraph.ItemObj(nItem), moColParam(0).oArm, _
                        '        moColParam(0).nColor, moColParam(0).nValve, msParmTag, moColParam(0).oZone)
                        Case eDataSource.Robot
                            subGetRobotValue(moGraph.ItemCfg(nItem), sGraphData(nItem), moGraph.ItemObj(nItem), moColParam(0).oArm, _
                                moColParam(0).nColor, moColParam(0).nValve, msParmTag)
                        Case Else
                            sGraphData(nItem) = "0"
                    End Select

                Next
                If Not (sGraphData Is Nothing) Then
                    frmGraph.AddNodes(sGraphData)
                End If
                tmrGraph.Enabled = True
            End If
        Catch ex As Exception

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
        ' 10/28/13  BTK     Changed so import doesn't always test for robot variable.
        ' 02/03/14  RJO     Fix to prevent errors when trying to import "N/A" values
        '********************************************************************************************
        Dim sFile As String = String.Empty

        ' Open the file to read from.
        Dim sTitleReq As String = msScreenName
        Dim sTableStart(0) As String
        sTableStart(0) = msScreenName
        Dim sHeader As String = Nothing
        Dim oDT As DataTable = Nothing
        ImportExport.GetDTFromCSV(sTitleReq, sTableStart, sHeader, oDT)

        If sHeader IsNot Nothing AndAlso oDT IsNot Nothing Then

            Try
                'process the file
                Dim nStep As Integer = 1
                Dim nColumns() As Integer = Nothing
                Dim sZone() As String = Nothing
                Dim sArm() As String = Nothing
                Dim sController() As String = Nothing
                Dim sColor() As String = Nothing
                Dim sValve() As String = Nothing
                Dim sParm() As String = Nothing
                '10/28/13 BTK Changed so import doesn't always test for robot variable.
                Dim oVar As FRRobot.FRCVar = Nothing
                For Each oRow As DataRow In oDT.Rows
                    Dim sTmp As String = String.Empty
                    For Each oItem As Object In oRow.ItemArray
                        sTmp = sTmp & vbTab & oItem.ToString
                    Next
                    sTmp = sTmp.Substring(1)
                    Dim sText() As String = Split(sTmp, vbTab)
                    Select Case nStep
                        Case 1
                            If InStr(sText(0), mtTabCfgs(mnTab).TabName) > 0 Then
                                If (mtTabCfgs(mnTab).bUseController = False) AndAlso _
                                   (mtTabCfgs(mnTab).bUseArm = False) AndAlso _
                                   (mtTabCfgs(mnTab).bUseColor = False) AndAlso _
                                   (mtTabCfgs(mnTab).bUseValve = False) AndAlso _
                                    (mtTabCfgs(mnTab).bUseCustomParm = False) Then
                                    nStep = 3
                                    ReDim nColumns(1)
                                    nColumns(1) = 0
                                Else
                                    nStep = 2
                                End If
                            End If
                        Case 2
                            Select Case sText(0).Trim.ToLower
                                Case gcsRM.GetString("csZONE_CAP").Trim.ToLower
                                    ReDim sZone(sText.GetUpperBound(0))
                                    Array.Copy(sText, sZone, sText.GetUpperBound(0) + 1)
                                Case gpsRM.GetString("psCONTROLLER_CAP").Trim.ToLower
                                    ReDim sController(sText.GetUpperBound(0))
                                    Array.Copy(sText, sController, sText.GetUpperBound(0) + 1)
                                Case gcsRM.GetString("csROBOT").Trim.ToLower
                                    ReDim sArm(sText.GetUpperBound(0))
                                    Array.Copy(sText, sArm, sText.GetUpperBound(0) + 1)
                                Case gcsRM.GetString("csCOLOR_CAP").Trim.ToLower
                                    ReDim sColor(sText.GetUpperBound(0))
                                    Array.Copy(sText, sColor, sText.GetUpperBound(0) + 1)
                                Case gpsRM.GetString("psVALVE_CAP").Trim.ToLower
                                    ReDim sValve(sText.GetUpperBound(0))
                                    Array.Copy(sText, sValve, sText.GetUpperBound(0) + 1)
                                Case lblParm.Text.Trim.ToLower
                                    ReDim sParm(sText.GetUpperBound(0))
                                    Array.Copy(sText, sParm, sText.GetUpperBound(0) + 1)
                                Case Else
                            End Select
                            If ((mtTabCfgs(mnTab).bUseController = False) Or (sController IsNot Nothing)) AndAlso _
                               ((mtTabCfgs(mnTab).bUseArm = False) Or (sArm IsNot Nothing)) AndAlso _
                               ((mtTabCfgs(mnTab).bUseColor = False) Or (sColor IsNot Nothing)) AndAlso _
                               ((mtTabCfgs(mnTab).bUseValve = False) Or (sValve IsNot Nothing)) AndAlso _
                               ((mtTabCfgs(mnTab).bUseCustomParm = False) Or (sParm IsNot Nothing)) Then
                                Dim nColCount As Integer = 0
                                If mtTabCfgs(mnTab).bUseController Then
                                    nColCount = sController.GetUpperBound(0)
                                End If
                                If mtTabCfgs(mnTab).bUseArm AndAlso (sArm.GetUpperBound(0) < nColCount) Then
                                    nColCount = sArm.GetUpperBound(0)
                                End If
                                If mtTabCfgs(mnTab).bUseColor AndAlso (sColor.GetUpperBound(0) < nColCount) Then
                                    nColCount = sColor.GetUpperBound(0)
                                End If
                                If mtTabCfgs(mnTab).bUseValve AndAlso (sValve.GetUpperBound(0) < nColCount) Then
                                    nColCount = sValve.GetUpperBound(0)
                                End If
                                ReDim nColumns(nColCount)
                                For nColIdx As Integer = 1 To nColCount
                                    Dim nDGColIdx As Integer = 0
                                    nColumns(nColIdx) = -1
                                    Do While (nDGColIdx < mdgvData.ColumnCount) And (nColumns(nColIdx) = -1)
                                        'Check for a matching column
                                        If (mtTabCfgs(mnTab).bUseController = False) OrElse _
                                            (mdgvData.Rows(mnCONTROLLER_ROW).Cells(nDGColIdx).Value.ToString.Trim = sController(nColIdx).Trim) Then
                                            If (mtTabCfgs(mnTab).bUseArm = False) OrElse _
                                                (mdgvData.Rows(mnROBOT_ROW).Cells(nDGColIdx).Value.ToString.Trim = sArm(nColIdx).Trim) Then
                                                If (mtTabCfgs(mnTab).bUseColor = False) OrElse _
                                                    (mdgvData.Rows(mnCOLOR_ROW).Cells(nDGColIdx).Value.ToString.Trim = sColor(nColIdx).Trim) Then
                                                    If (mtTabCfgs(mnTab).bUseValve = False) OrElse _
                                                        (mdgvData.Rows(mnVALVE_ROW).Cells(nDGColIdx).Value.ToString.Trim = sValve(nColIdx).Trim) Then
                                                        If (mtTabCfgs(mnTab).bUseCustomParm = False) OrElse _
                                                            (mdgvData.Rows(mnCUSTOMPARM_ROW).Cells(nDGColIdx).Value.ToString.Trim = sParm(nColIdx).Trim) Then
                                                            nColumns(nColIdx) = nDGColIdx
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        End If
                                        nDGColIdx += 1
                                    Loop
                                Next
                                nStep = 3
                            End If
                        Case 3
                            Dim nRow As Integer = mnITEM0_ROW
                            Dim oCell As DataGridViewCell
                            Do While nRow < mdgvData.RowCount
                                If (sText(0).Trim = mdgvData.Rows(nRow).HeaderCell.Value.ToString.Trim) Then
                                    For nCol As Integer = 1 To nColumns.GetUpperBound(0)
                                        If nColumns(nCol) >= 0 Then
                                            oCell = mdgvData.Rows(nRow).Cells(nColumns(nCol))
                                            'Debug.Print(oCell.Value.ToString)
                                            'Debug.Print(sText(nCol))
                                            If Not (oCell.FormattedValue.ToString = gpsRM.GetString("psNA")) Then 'RJO 02/03/14
                                            If (oCell.ValueType Is GetType(String)) Then
                                                oCell.Value = sText(nCol)
                                            ElseIf (oCell.ValueType Is GetType(Integer)) Then
                                                If IsNumeric(sText(nCol)) Then
                                                    oCell.Value = CType(sText(nCol), Integer)
                                                Else
                                                    oCell.Value = 0
                                                End If
                                            ElseIf (oCell.ValueType Is GetType(Double)) Then
                                                If IsNumeric(sText(nCol)) Then
                                                    oCell.Value = CType(sText(nCol), Double)
                                                Else
                                                    oCell.Value = 0
                                                End If
                                            Else
                                                '10/28/13 BTK Changed so import doesn't always test for robot variable.
                                                oVar = DirectCast(oCell.Tag, FRRobot.FRCVar)
                                            End If

                                            Dim sOldVal As String = String.Empty
                                            If oVar IsNot Nothing Then
                                                sOldVal = oVar.Value.ToString
                                            Else
                                                sOldVal = oCell.Tag.ToString
                                            End If
                                            If (sOldVal <> oCell.Value.ToString) Then
                                                oCell.Style.ForeColor = Color.Red
                                                oCell.Style.SelectionForeColor = Color.Red
                                                EditsMade = True
                                            End If
                                        End If
                                        End If
                                    Next
                                End If
                                nRow += 1
                            Loop
                    End Select

                Next

            Catch ex As Exception

                Trace.WriteLine(ex.Message)
                Trace.WriteLine(ex.StackTrace)
                ShowErrorMessagebox(gcsRM.GetString("csIMPORTFAILED"), ex, msSCREEN_NAME, _
                                     Status, MessageBoxButtons.OK)
            End Try

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

    Private Sub oIPC_NewMessage(ByVal Schema As String, ByVal DS As System.Data.DataSet) Handles oIPC.NewMessage
        '********************************************************************************************
        'Description:  A new message has been received from another PAINTworks Application
        '
        'Parameters: DS - PWUser Dataset, ProcessName PAINTworks screen process name
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
        '********************************************************************************************
        If (mdgvData IsNot Nothing) AndAlso (mnTab >= 0) AndAlso DataLoaded Then
            'If mdgvData.RowCount < mdgvData.ColumnCount * 5 Then
            mPrintHtml.subSetPageFormat()
            'End If
            mPrintHtml.subClearTableFormat()
            mPrintHtml.subSetColumnCfg("Bold=on,align=right", 0)
            Dim nBoldRows As Integer = 0
            For nRow As Integer = 0 To mnITEM0_ROW - 1
                If mdgvData.Rows(nRow).Visible Then
                    nBoldRows += 1
                End If
            Next
            If nBoldRows > 0 Then
                mPrintHtml.subSetRowcfg("Bold=on", 0, nBoldRows - 1)
            End If
            Dim sTitle(0) As String
            sTitle(0) = msScreenName
            Dim sSubTitle(0) As String
            sSubTitle(0) = mtTabCfgs(mnTab).TabName
            Dim bCancel As Boolean = False
            mPrintHtml.subCreateSimpleDoc(mdgvData, Status, msScreenName & " - " & mtTabCfgs(mnTab).TabName, sTitle, sSubTitle, bExportCSV, bCancel)
            If bPrint And (bCancel = False) Then
                mPrintHtml.subPrintDoc()
            End If
            Return (True)
        Else
            Return (False)
        End If
    End Function


    Private Sub mdgvData_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles mdgvData.KeyPress
        '********************************************************************************************
        'Description:  offer login
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/18/09  MSW     mdgvData_KeyPress - Offer login if needed
        '********************************************************************************************
        If Not (mdgvData.IsCurrentCellInEditMode) Then
            If Char.IsNumber(e.KeyChar) OrElse Char.IsLetter(e.KeyChar) Then
                If Privilege = ePrivilege.None Then
                    Privilege = ePrivilege.Edit
                End If
            End If
        End If
    End Sub

    'Private Sub btnMultiView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.DoubleClick, btnMultiView.DoubleClick
    '    '********************************************************************************************
    '    'Description:  shortcut to add all arms or robots with a doubleclick on the multiview or add button
    '    '
    '    'Parameters: none
    '    'Returns:    none
    '    '
    '    'Modification history:
    '    '
    '    ' Date      By      Reason
    '    ' 08/26/11  MSW     failed to add feature - doubleclick seems more intuitive than right-click, but the event doesn't trigger
    '    '********************************************************************************************
    '    Application.DoEvents()
    '    If mtTabCfgs(mnTab).bUseColor = False And mtTabCfgs(mnTab).bUseValve = False And _
    '       mtTabCfgs(mnTab).bUseCustomParm = False And (mtTabCfgs(mnTab).bUseArm Or mtTabCfgs(mnTab).bUseController) Then
    '        If btnMultiView.Checked = False Then
    '            subShowMultiView()
    '            Application.DoEvents()
    '        End If
    '        mnuAddAll_Click(mnuAddAllRobots)
    '    End If
    'End Sub

    Private Sub btnMultiView_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles btnAdd.MouseDown, btnMultiView.MouseDown
        '********************************************************************************************
        'Description:  shortcut to add all arms or robots with a right click on the multiview or add button
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/26/11  MSW     added feature
        '********************************************************************************************
        If e.Button = Windows.Forms.MouseButtons.Right Then
            Application.DoEvents()
            If mtTabCfgs(mnTab).bUseColor = False And mtTabCfgs(mnTab).bUseValve = False And _
               mtTabCfgs(mnTab).bUseCustomParm = False And (mtTabCfgs(mnTab).bUseArm Or mtTabCfgs(mnTab).bUseController) Then
                If btnMultiView.Checked = False Then
                    subShowMultiView()
                    Application.DoEvents()
                End If
                mnuAddAll_Click(mnuAddAllRobots)
            End If
        End If
    End Sub

    Private Sub mnuAutoRefresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuAutoRefresh.Click
        '********************************************************************************************
        'Description:  Enable autorefresh
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mnuAutoRefresh.Checked Then
            mnuAutoRefresh.Checked = False
            btnRefresh.Image = DirectCast(gpsRM.GetObject("Refresh", mLanguage.FixedCulture), Image)
            tmrRefresh.Enabled = False
        Else
            Application.DoEvents()
            mnuAutoRefresh.Checked = True
            mnRefreshRotation = -1
            Application.DoEvents()
            subRefreshData()
        End If
    End Sub

    Private Sub tmrRefresh_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrRefresh.Tick
        '********************************************************************************************
        'Description:  autorefresh
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        subRefreshData()
    End Sub

End Class