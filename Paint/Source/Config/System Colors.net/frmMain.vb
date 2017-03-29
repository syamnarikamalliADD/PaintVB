' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2008
' FANUC Robotics America
' FANUC LTD Japan
'
' Form/Module: frmMain
'
' Description: System Color setup and edit screen
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
'    Date       By      Reason                                                          Version
'    04/08/08   gks     Changes to use SQL Database
'    07/23/08   gks     Add 2K tab                                                      4.00.02
'    11/18/09   MSW     subTextboxKeypressHandler, subSetTextBoxProperties - Offer a login if they try to enter data without a login
'    11/28/09   RJO     Add RobotsRequiredStatingBit offset Code
'    06/15/10   MSW     handle cross-thread calls
'    06/30/10   MSW     handle page up, page down, home, end
'    09/23/10   MSW     Convert to html print, remove microsoft word references
'    10/20/10   MSW     Add resin and hardener solvent valve text boxes
'    01/11/11   MSW     allow decimal points in ratios
'    09/14/11   MSW     Assemble a standard version of everything                       4.1.0.0
'    09/27/11   MSW     Standalone changes round 1 - HGB SA paintshop computer changes	4.1.0.1
'    12/02/11   MSW     Add DMON placeholder reference                                  4.1.1.0
'    01/24/12   MSW     Placeholder include updates                                     4.01.01.02
'    02/16/12   MSW     Print/Import/Export updates, Force 32 bit build                 4.01.01.03
'    03/21/12   RJO     Modified to use .NET Password and IPC                           4.1.2.0
'    04/05/12   MSW     Tech center mixed 2k and WB in the same zone.                   4.1.3.0
'                       Move tables from SQL to XML
'    04/27/12   MSW     Add some error handle for mixed 2k zones, open utilities menu 
'                       with button click                                               4.1.3.1
'    10/04/12   RJO     In bSaveToPLC, made the change log reporting method for Robots  4.1.3.2
'                       Required match the metod used in Job Setup.
'    10/23/12   RJO     Added StartupModule to project to prevent multiple instances    4.1.3.3
'    12/13/12   MSW     bLoadFromPLC-Don't compare number of valves to number of colors 4.1.3.4
'    04/16/13   MSW     Add Canadian language files                                     4.1.5.0
'                       Standalone Changelog
'    07/09/13   MSW     Update and standardize logos                                    4.01.05.01
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up  4.01.05.02
'    09/30/13   MSW     Save screenshots as jpegs, PLC DLL                              4.01.06.00
'    01/06/14   MSW     Deal with non-consecutive robot numbers.                        4.01.06.01
'    02/03/14   RJO     Change gsChangeLogArea from "SystemColors" to "System Colors"   4.01.06.02
'                       for consistency with Change Log Reports screen. In subShoeChangelog, 
'                       changed sScreenName argument to mPWCommon.subShowChangeLog from 
'                       msSCREEN_NAME to gsChangeLogArea.
'    02/13/14   MSW     Switch cross-thread handling to BeginInvoke call                4.01.07.00
'***************************************************************************************************

Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Configuration.ConfigurationSettings
Imports Response = System.Windows.Forms.DialogResult
Imports Connstat = FRRobotNeighborhood.FRERNConnectionStatusConstants
Imports System.Xml
Imports System.Xml.XPath
Imports clsPLCComm = PLCComm.clsPLCComm


Friend Class frmMain

#Region " Declares "
    '******** Form Constants   **********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Public Const msSCREEN_NAME As String = "SystemColors"   ' <-- For password area change log etc.
    Public Const msSCREEN_DUMP_NAME As String = "Config_SystemColors"
    Private Const mnMAININX As Integer = 100
    Private Const mnSCREENINX As Integer = 101
    Private Const msMODULE As String = "frmMain"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    Friend Const mbUSEROBOTS As Boolean = True
    Private Const mnROWSPACE As Integer = 40 ' interval for rows of textboxes etc
    Private Const mnMAX_TABS As Integer = 5 'NRU 161213 Piggable 4 to 5
    'NRU 161213 Piggable
    Private Const mnMAX_MAINLINE_VALVES As Integer = 14
    Private colTabComboBoxes(mnMAX_TABS) As Collection(Of ComboBox)
    Private strPigSystems() As String 'The currently selected values to write to the PLC
    Private strPrevPigSystems() As String 'For changelog comparison

    Private msCulture As String = "en-US" 'Default to English
    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    '9.5.07 remove unnecessary initializations per fxCop
    Friend Shared gsChangeLogArea As String = "System Colors" 'msSCREEN_NAME 'RJO 02/03/14
    Private msOldZone As String = String.Empty
    Private msOldRobot As String = String.Empty
    Private msOldParam As String = String.Empty
    Private colZones As clsZones ' = Nothing
    Private WithEvents colControllers As clsControllers ' = Nothing
    Private WithEvents colArms As clsArms ' = Nothing
    Private mRobot As clsArm ' = Nothing
    Private colTabTextboxes(mnMAX_TABS) As Collection(Of FocusedTextBox.FocusedTextBox)
    Private mcolCheckBoxes As Collection(Of CheckBox) = New Collection(Of CheckBox)
    Private mbRemoteZone As Boolean ' = False
    Private mbEventBlocker As Boolean ' = False
    Private mbScreenLoaded As Boolean ' = False
    Private WithEvents moPLC As New clsPLCComm
    Private mbPLCFail As Boolean ' = False
    Private mnScrollPtr As Integer ' = 0
    Private tbcDummy As New TabControl
    Private mnControllerMaxColors As Integer = 1
    Private mPrintHtml As clsPrintHtml
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private mbEditsMade As Boolean ' = False
    Private msUserName As String = String.Empty
    Private mbDataLoaded As Boolean ' = False
    Private mnProgress As Integer ' = 0
    Private mPrivilegeGranted As ePrivilege = ePrivilege.None
    Private mPrivilegeRequested As ePrivilege = ePrivilege.None
    '******** End Property Variables    *************************************************************

    '******** This is the old pw3 password object interop  *****************************************
    'Friend WithEvents moPassword As PWPassword.PasswordObject 'RJO 03/21/12
    'Friend moPrivilege As PWPassword.cPrivilegeObject 'RJO 03/21/12
    '******** This is the old pw3 password object interop  *****************************************

    Friend WithEvents moPassword As clsPWUser
    Friend WithEvents oIPC As Paintworks_IPC.clsInterProcessComm

    ' The delegates (function pointers) enable asynchronous calls from the password object.
    Delegate Sub LoginCallBack()
    Delegate Sub LogOutCallBack()
    Delegate Sub ControllerStatusChange(ByVal Controller As clsController)
    Delegate Sub NewMessage_Callback(ByVal Schema As String, ByVal DS As DataSet) 'RJO 03/21/12

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
            End If
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
            Dim bOld As Boolean = mbEditsMade
            mbEditsMade = Value
            If mbEditsMade <> bOld Then
                subEnableControls(True)
            End If
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
        Dim lRet As DialogResult

        lRet = MessageBox.Show(gcsRM.GetString("csSAVEMSG1") & vbCrLf & _
                            gcsRM.GetString("csSAVEMSG"), gcsRM.GetString("csSAVE_CHANGES"), _
                            MessageBoxButtons.YesNoCancel, _
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button3, _
                            MessageBoxOptions.DefaultDesktopOnly)

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
    Private Function bLoadFromGUI(ByRef rRobot As clsArm) As Boolean
        '********************************************************************************************
        'Description:  This verifies the number of rows in database. the actual load is done in
        '               clssystemcolors called from bottom of this routine.
        '
        'Parameters: clsArm to load
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bTmp As Boolean = False
        Dim oZone As clsZone = colZones.ActiveZone

        Try

            Status = gcsRM.GetString("csLOAD_GUI")


            'Load from Database we just checked
            rRobot.SystemColors.Load(rRobot)

            Return True

        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            mDebug.WriteEventToLog(msSCREEN_NAME & ":bLoadFromGUI", ex.Message)
            Return False

        End Try

    End Function
    Private Function bLoadFromPLC(ByRef oColors As clsSysColors) As Boolean
        '********************************************************************************************
        'Description:  Load data stored on PLC. Exceptions not localized, should be a setup only thing
        '
        'Parameters: none
        'Returns:     True if load success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/17/10  MSW     Don't bomb out on tricoat data unless we need it.
        ' 02/22/11  JBW     Added in Two coat by colors data
        ' 12/13/12  MSW     bLoadFromPLC - Don't compare number of valves to number of colors
        '********************************************************************************************
        Dim sPLCData() As String
        Dim sPLCData2() As String 'NRU 161209 300 colors
        Dim sPLCData3() As String 'NRU 161209 300 colors

        Dim sTag As String = String.Empty
        Dim i As Integer
        Dim nTmp As Integer
        Dim sPrefix As String = colZones.ActiveZone.PLCTagPrefix

        Try

            If colZones.ActiveZone.PLCType = ePLCType.None Then Return True

            Status = gcsRM.GetString("csLOAD_PLC")

            moPLC = New clsPLCComm
            moPLC.ZoneName = colZones.ActiveZone.Name

            'NRU 161209 300 Colors

            ' Valve Number
            sTag = sPrefix & "ColorValves1"
            moPLC.TagName = sTag
            sPLCData = moPLC.PLCData

            sTag = sPrefix & "ColorValves2"
            moPLC.TagName = sTag
            sPLCData2 = moPLC.PLCData

            sTag = sPrefix & "ColorValves3"
            moPLC.TagName = sTag
            sPLCData3 = moPLC.PLCData

            sPLCData = CType(MergeArrays(sPLCData, sPLCData2), String())
            sPLCData = CType(MergeArrays(sPLCData, sPLCData3), String())

            If sPLCData Is Nothing Then Return False

            ' 12/13/12  MSW     bLoadFromPLC - Don't compare number of valves to number of colors
            'NO NO NO NO NO Valves are not the same as colors
            'sanity check
            'If oColors.EffectiveColors <> UBound(sPLCData) + 1 Then
            '    Throw New Data.ConstraintException( _
            '        "Number of Colors (valves) from PLC does not match Number of colors from Database")

            'End If

            For i = 0 To oColors.EffectiveColors - 1
                'For i = 0 To UBound(sPLCData) - 1   'JZ
                nTmp = CType(sPLCData(i), Integer)
                If nTmp < 1 Then nTmp = 1
                If nTmp > oColors.MaximumValveNumber Then nTmp = oColors.MaximumValveNumber
                oColors.Item(i).Valve.Number.Value = nTmp
            Next

            'NRU 161209 300 Colors
            ' Plant Number
            sTag = sPrefix & "PlantColors1"
            moPLC.TagName = sTag
            sPLCData = moPLC.PLCData

            ' Plant Number
            sTag = sPrefix & "PlantColors2"
            moPLC.TagName = sTag
            sPLCData2 = moPLC.PLCData

            ' Plant Number
            sTag = sPrefix & "PlantColors3"
            moPLC.TagName = sTag
            sPLCData3 = moPLC.PLCData

            sPLCData = CType(MergeArrays(sPLCData, sPLCData2), String())
            sPLCData = CType(MergeArrays(sPLCData, sPLCData3), String())

            If sPLCData Is Nothing Then Return False

            'sanity check
            If oColors.EffectiveColors <> UBound(sPLCData) + 1 Then
                Throw New Data.ConstraintException( _
                    "Number of Plant Colors  from PLC does not match Number of colors from Database")

            End If

            For i = 0 To oColors.EffectiveColors - 1
                If oColors.IsAscii Then
                    'Try
                    '    Dim nVal As Integer = CInt(sPLCData(i))
                    '    Dim sTmp As String = String.Empty
                    '    Do While nVal > 0
                    '        sTmp = Chr(nVal And 255) & sTmp
                    '        nVal = nVal \ 256
                    '    Loop
                    '    If sTmp = String.Empty Then
                    '        sTmp = i.ToString
                    '    End If
                    '    oColors.Item(i).PlantNumber.Text = sTmp

                    'Catch ex As Exception
                    '    Trace.WriteLine(ex.Message)
                    '    ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED"), ex, msSCREEN_NAME, _
                    '                        Status, MessageBoxButtons.OK)
                    '    oColors.Item(i).PlantNumber.Text = i.ToString
                    'End Try
                    oColors.Item(i).PlantNumber.Text = _
                        mMathFunctions.CvIntegerToASCII(CInt(sPLCData(i)), oColors.PlantAsciiMaxLength)
                Else
                    nTmp = CType(sPLCData(i), Integer)
                    If nTmp < 1 Then nTmp = 1
                    If nTmp > oColors.MaximumPlantColorNumber Then nTmp = oColors.MaximumPlantColorNumber
                    oColors.Item(i).PlantNumber.Text = nTmp.ToString(mLanguage.FixedCulture)
                End If
            Next

            '
            'FANUC color not loaded - should be 1 to number of colors no matter what
            '

            'NRU 161209 300 Colors
            ' Robots Required
            sTag = sPrefix & "RobotsRequiredForColor1"
            moPLC.TagName = sTag
            sPLCData = moPLC.PLCData

            sTag = sPrefix & "RobotsRequiredForColor2"
            moPLC.TagName = sTag
            sPLCData2 = moPLC.PLCData

            sTag = sPrefix & "RobotsRequiredForColor3"
            moPLC.TagName = sTag
            sPLCData3 = moPLC.PLCData

            sPLCData = CType(MergeArrays(sPLCData, sPLCData2), String())
            sPLCData = CType(MergeArrays(sPLCData, sPLCData3), String())

            If sPLCData Is Nothing Then Return False

            'sanity check
            If oColors.EffectiveColors <> UBound(sPLCData) + 1 Then
                Throw New Data.ConstraintException( _
                    "Number of Colors (Robots Required) from PLC does not match Number of colors from Database")
            End If

            For i = 0 To oColors.EffectiveColors - 1
                oColors.Item(i).RobotsRequired.Value = CType(sPLCData(i), Integer)
            Next
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            If oColors.UseTwoCoat Then
                'jbw added two coat colors 2/22/11
                sTag = sPrefix & "TwoCoatColors"
                moPLC.TagName = sTag
                sPLCData = moPLC.PLCData
                If sPLCData Is Nothing Then Return False
                If oColors.EffectiveColors <> UBound(sPLCData) + 1 Then
                    Throw New Data.ConstraintException( _
                        "Number of colors (Twocoats by color) from PLC does nto match Number of colors from Database")
                End If
                For i = 0 To oColors.EffectiveColors - 1
                    nTmp = CType(sPLCData(i), Integer)
                    If nTmp = 0 Then
                        oColors.Item(i).TwoCoats.Value = False
                    Else
                        oColors.Item(i).TwoCoats.Value = True
                    End If

                Next
            End If
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            If oColors.UseTricoat Then 'MSW don't bomb out on tricoat data unless we need it.
                ' Tricoat Number
                sTag = sPrefix & "TricoatColors"
                moPLC.TagName = sTag
                sPLCData = moPLC.PLCData

                If sPLCData Is Nothing Then Return False

                'sanity check
                If oColors.EffectiveColors <> UBound(sPLCData) + 1 Then
                    Throw New Data.ConstraintException( _
                        "Number of Tricoat Colors  from PLC does not match Number of colors from Database")

                End If

                For i = 0 To oColors.EffectiveColors - 1
                    If oColors.IsAscii Then
                        oColors.Item(i).Tricoat.Text = _
                            mMathFunctions.CvIntegerToASCII(CInt(sPLCData(i)), oColors.PlantAsciiMaxLength)
                    Else
                        nTmp = CType(sPLCData(i), Integer)
                        If nTmp < 1 Then nTmp = 1
                        If nTmp > oColors.MaximumPlantColorNumber Then nTmp = oColors.MaximumPlantColorNumber
                        oColors.Item(i).Tricoat.Text = nTmp.ToString(mLanguage.FixedCulture)
                    End If
                Next

            End If

            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            Return False

        End Try

    End Function
    Private Function bLoadFromRobot(ByRef rRobot As clsArm) As Boolean
        ''********************************************************************************************
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
            Status = gcsRM.GetString("csLOAD_ROBOT") & " " & rRobot.Controller.Name

            If rRobot.IsOnLine Then
                'drill down to color struct
                rRobot.ProgramName = "pavrsysc"
                Dim o As FRRobot.FRCVars = rRobot.ProgramVars
                Dim oo As FRRobot.FRCVars = DirectCast(o.Item("SYS_COLORS"), FRRobot.FRCVars)
                Dim oCVars As FRRobot.FRCVars = DirectCast(oo.Item("NODEDATA"), FRRobot.FRCVars)
                Dim oVar As FRRobot.FRCVar

                '2K
                Dim o2K As FRRobot.FRCVars = Nothing
                Dim o2KVars As FRRobot.FRCVars = Nothing
                Dim bOpenerOnly As Boolean = rRobot.IsOpener
                If bOpenerOnly Then
                    For Each oArm As clsArm In rRobot.Controller.Arms
                        If oArm.IsOpener = False Then
                            bOpenerOnly = False
                            Exit For
                        End If
                    Next
                End If
                If rRobot.SystemColors.Use2K And (bOpenerOnly = False) Then
                    o2K = DirectCast(o.Item("SYS_2K_COLRS"), FRRobot.FRCVars)
                    o2KVars = DirectCast(o2K.Item("NODEDATA"), FRRobot.FRCVars)
                End If

                Dim bUse2K As Boolean = rRobot.SystemColors.Use2K And (bOpenerOnly = False)
                ' pick a color, any color
                For nColIndex As Integer = 0 To oCVars.Count - 1 ' rRobot.SystemColors.Count - 1

                    If nColIndex = rRobot.SystemColors.Count Then Exit For

                    Dim oCVar As FRRobot.FRCVars = DirectCast(oCVars(, nColIndex), FRRobot.FRCVars)
                    Dim oColor As clsSysColor = rRobot.SystemColors(nColIndex)

                    With oCVar


                        oVar = CType(.Item("COLOR_DESC"), FRRobot.FRCVar)
                        oColor.Description.Text = oVar.Value.ToString

                        oVar = CType(.Item("COLOR_VALVE"), FRRobot.FRCVar)
                        oColor.Valve.Number.Value = CInt(oVar.Value)

                    End With

                    Try

                        If bUse2K Then
                            oCVar = DirectCast(o2KVars(, nColIndex), FRRobot.FRCVars)

                            With oCVar

                                oVar = CType(.Item("HDR_VALVE"), FRRobot.FRCVar)
                                oColor.HardenerValve.Value = CInt(oVar.Value)

                                oVar = CType(.Item("HDR_RATIO"), FRRobot.FRCVar)
                                oColor.HardenerRatio.Value = CSng(oVar.Value)

                                oVar = CType(.Item("RES_RATIO"), FRRobot.FRCVar)
                                oColor.ResinRatio.Value = CSng(oVar.Value)

                                oVar = CType(.Item("RES_SOLV_NO"), FRRobot.FRCVar)
                                oColor.ResinSolventValve.Value = CInt(oVar.Value)

                                oVar = CType(.Item("HDR_SOLV_NO"), FRRobot.FRCVar)
                                oColor.HardenerSolventValve.Value = CInt(oVar.Value)

                            End With


                        End If
                    Catch ex As Exception
                        Debug.Assert(False)
                        'MSW 04/05/12 - Tech center mixed 2k and WB in the same zone.
                        'Deal with mixed 2K and WB robots.  Just skip saving the 2K if they aren't there.
                        'Assert statement is added above to check for this condition in case it causes trouble for someone else
                        'If you're here you probably have the GUI setup for 2K and a robot setup without it.
                        bUse2K = False

                    End Try


                Next

                'plc and database could differ
                rRobot.SystemColors.RefreshValveDescriptions()

                'sys colors normally loaded from database in load from gui routine
                Return True

            Else
                ' cant talk to robot
                Dim sTmp As String = gcsRM.GetString("csLOADFAILED") & vbCrLf & _
                                            gcsRM.GetString("csCOULD_NOT_CONNECT")
                Status = gcsRM.GetString("csLOADFAILED")
                MessageBox.Show(sTmp, rRobot.Controller.Name, _
                                MessageBoxButtons.OK, MessageBoxIcon.Error, _
                                MessageBoxDefaultButton.Button1, _
                                MessageBoxOptions.DefaultDesktopOnly)

                Return False
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bSaveToGUI(ByRef oColors As clsSysColors) As Boolean
        '********************************************************************************************
        'Description:  Save data stored on GUI in xml file .
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '4/8/08     gks     Change to SQL Database
        '02/22/11   JBW     Added two coats by color
        '********************************************************************************************
        Dim nPtr As Integer
        Dim colValve As New Collection(Of clsValve)
        Dim bFound As Boolean = False

        Dim oZone As clsZone = colZones.ActiveZone
        Try

            Status = gcsRM.GetString("csSAVE_GUI")

            
            For Each oColor As clsSysColor In oColors
                nPtr = oColor.FanucNumber - 1

                If oColor.PlantNumber.Changed Then
                    subUpdateChangeLog(oColor.PlantNumber.Text, oColor.PlantNumber.OldText, _
                                        lblPlantColorCap.Text, String.Empty, _
                                        oColor.FanucNumber.ToString(mLanguage.FixedCulture))
                End If

                If oColor.Description.Changed Then
                    subUpdateChangeLog(oColor.Description.Text, oColor.Description.OldText, _
                                        lblColorDescCap.Text, String.Empty, oColor.PlantNumber.Text)
                End If

                If oColor.Tricoat.Changed Then
                    subUpdateChangeLog(oColor.Tricoat.Text, oColor.Tricoat.OldText, _
                                        lblTricoat4Cap.Text, String.Empty, oColor.PlantNumber.Text)
                End If
                If oColor.TwoCoats.Changed Then
                    subUpdateChangeLog(oColor.TwoCoats.Value.ToString, oColor.TwoCoats.OldValue.ToString, _
                                       lblTwoCoats.Text, String.Empty, oColor.PlantNumber.Text)
                End If
            Next    'oColor

            
            bSaveColorsToXML(oZone, oColors)

            

            'Valve  table  ----------------------------------------------------------------
            Dim sColorValveNames() As String = mSysColorCommon.GetValveTableDataset(oZone)

            For i As Integer = 0 To oColors.Count - 1
                ' see if valve description has changed
                If oColors.Item(i).Valve.Description.Changed Then
                    With oColors.Item(i).Valve
                        sColorValveNames(.Number.Value - 1) = oColors.Item(i).Valve.Description.Text
                    End With
                    bFound = False
                    For Each v As clsValve In colValve
                        If v.Number.Value = nPtr Then
                            'already in collection for change log
                            bFound = True
                            Exit For
                        End If
                    Next
                    If Not bFound Then
                        'add to collection for change log
                        Dim newvalve As clsValve = oColors.Item(i).Valve
                        colValve.Add(newvalve)
                    End If
                End If 'oColors.Item(i).Valve.Description.Changed
            Next    'i

            'change log for valve desc changes
            For Each v As clsValve In colValve
                subUpdateChangeLog(v.Description.Text, v.Description.OldText, _
                    lblValveDescCap.Text, String.Empty, v.Number.Value.ToString(mLanguage.FixedCulture))
            Next
            bSaveValvesToXML(oZone, sColorValveNames)
            mRobot.SystemColors.Update()

            Return True


        Catch ex As Exception

            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            mDebug.WriteEventToLog(msSCREEN_NAME & ":bSaveToGUI", ex.Message)

            Return False

        End Try

    End Function
    Private Function bSaveToPLC(ByRef oColors As clsSysColors) As Boolean
        '********************************************************************************************
        'Description:  Save data stored on PLC.
        '
        'Parameters: none
        'Returns:     True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/30/10  MSW     add save to ascii logic
        ' 10/05/10  MSW     Clear out extra bits in the robots required word
        ' 02/22/11  JBW     Added two coats by color
        ' 10/04/12  RJO     Made change log reporting for Robots Required the same as Job Setup.
        '********************************************************************************************
        Dim sPLCData() As String
        Dim sTag As String = String.Empty
        Dim i As Integer
        Dim x As Integer 'NRU 161209 300 Colors
        Dim y As Integer 'NRU 161209 300 Colors
        Dim nUB As Integer = oColors.EffectiveColors - 1
        Dim sPrefix As String = colZones.ActiveZone.PLCTagPrefix

        Try

            If colZones.ActiveZone.PLCType = ePLCType.None Then Return True

            Status = gcsRM.GetString("csSAVE_PLC")
            Progress = 80

            moPLC.ZoneName = colZones.ActiveZone.Name
			
			mbPLCFail = False

            'color valves

            'NRU 161209 300 colors
            For x = 1 To 3
                ReDim sPLCData(99)
                For i = 0 To 99
                    y = (x - 1) * 100 + i
                    sPLCData(i) = oColors.Item(y).Valve.Number.Value.ToString(mLanguage.CurrentCulture)
                Next
                sTag = sPrefix & "ColorValves" & x.ToString
                moPLC.TagName = sTag
                moPLC.PLCData = sPLCData
                Application.DoEvents()
            Next x

            'NRU 161213 Piggable
            'Pigsystems are by robot in PLC, but by zone in GUI.  Write to all robots in this zone.
            For i = 1 To colControllers.Count
                sTag = sPrefix & "PigSystemsRC" & CStr(i)
                moPLC.TagName = sTag
                moPLC.PLCData = strPigSystems
                Application.DoEvents()
            Next i

            'change log
            For i = 0 To nUB
                If oColors.Item(i).Valve.Number.Changed Then
                    subUpdateChangeLog(oColors.Item(i).Valve.Number.Value.ToString(mLanguage.CurrentCulture), _
                                        oColors.Item(i).Valve.Number.OldValue.ToString( _
                                        mLanguage.CurrentCulture), lblValveNumberCap.Text, "PLC", _
                                        oColors.Item(i).PlantNumber.Text)
                End If
            Next

            If oColors.Use2K Then

                'Hardener valves

                'NRU 161209 300 colors
                For x = 1 To 3
                    ReDim sPLCData(99)
                    For i = 0 To 99
                        y = (x - 1) * 100 + i
                        sPLCData(i) = oColors.Item(y).HardenerValve.Value.ToString(mLanguage.CurrentCulture)
                    Next

                    sTag = sPrefix & "HardenerValves" & x.ToString
                    moPLC.TagName = sTag
                    moPLC.PLCData = sPLCData
                    Application.DoEvents()
                Next x

                'change log
                For i = 0 To nUB
                    If oColors.Item(i).HardenerValve.Changed Then
                        subUpdateChangeLog(oColors.Item(i).HardenerValve.Value.ToString(mLanguage.CurrentCulture), _
                                            oColors.Item(i).HardenerValve.OldValue.ToString( _
                                            mLanguage.CurrentCulture), lblHardenerCap.Text, "PLC", _
                                            oColors.Item(i).PlantNumber.Text)
                    End If
                Next
                Application.DoEvents()
            End If

            ' Plant Color

            'NRU 161209 300 colors
            For x = 1 To 3
                ReDim sPLCData(99)
                For i = 0 To 99
                    y = (x - 1) * 100 + i
                    sPLCData(i) = oColors.Item(y).PlantNumber.Text
                Next
                If mRobot.SystemColors.IsAscii Then
                    For nCounter As Integer = 0 To sPLCData.GetUpperBound(0)
                        'If sPLCData(nCounter).Length < 4 Then
                        '    sPLCData(nCounter) = "    " & sPLCData(nCounter)
                        'End If
                        'If sPLCData(nCounter).Length > 4 Then
                        '    sPLCData(nCounter) = sPLCData(nCounter).Substring(sPLCData(nCounter).Length - 4)
                        'End If
                        'Dim nDigit As Integer = sPLCData(nCounter).Length - 1
                        ''Assume DINT type, max 4 char
                        'Dim nMult As Integer = 1
                        'Dim nTmp As Integer = 0
                        'Do While nDigit >= 0
                        '    nTmp = nTmp + Asc(sPLCData(nCounter).Substring(nDigit, 1)) * nMult
                        '    If nDigit > 0 Then
                        '        nMult = nMult * 256
                        '    End If
                        '    nDigit = nDigit - 1
                        'Loop
                        'sPLCData(nCounter) = nTmp.ToString
                        sPLCData(nCounter) = mMathFunctions.CvASCIIToInteger(sPLCData(nCounter), oColors.PlantAsciiMaxLength).ToString
                    Next
                End If
                sTag = sPrefix & "PlantColors" & x.ToString
                moPLC.TagName = sTag
                moPLC.PLCData = sPLCData
                Application.DoEvents()
            Next x

            If mRobot.SystemColors.UseTwoCoat Then
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'jbw 2/21/11 twocoats by color
                Dim tempint As Integer
                ReDim sPLCData(nUB)
                For i = 0 To nUB
                    If (oColors.Item(i).TwoCoats.Value) = False Then
                        tempint = 0
                    Else
                        tempint = 1
                    End If
                    sPLCData(i) = tempint.ToString(mLanguage.CurrentCulture)
                Next

                sTag = sPrefix & "TwoCoatColors"
                moPLC.TagName = sTag
                moPLC.PLCData = sPLCData
                Application.DoEvents()
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            End If
            'change log for plant color done elsewhere

            ' FANUC Color

            'NRU 161209 300 colors
            For x = 1 To 3
                ReDim sPLCData(99)
                For i = 0 To 99
                    y = (x - 1) * 100 + i
                    sPLCData(i) = oColors.Item(y).FanucNumber.ToString
                Next
                sTag = sPrefix & "FanucColors" & x.ToString
                moPLC.TagName = sTag
                moPLC.PLCData = sPLCData
                Application.DoEvents()
            Next x

            ' Tricoat Color
            If oColors.UseTricoat Then
                ReDim sPLCData(nUB)
                For i = 0 To nUB
                    If mRobot.SystemColors.IsAscii Then
                        sPLCData(i) = mMathFunctions.CvASCIIToInteger(oColors.Item(i).Tricoat.Text, oColors.PlantAsciiMaxLength).ToString
                    Else
                        sPLCData(i) = oColors.Item(i).Tricoat.Text
                    End If

                Next
                sTag = sPrefix & "TricoatColors"
                moPLC.TagName = sTag
                moPLC.PLCData = sPLCData
                Application.DoEvents()
            End If

            ' Robots Required

            'NRU 161209 300 colors
            For x = 1 To 3
                ReDim sPLCData(99)
                Dim nMax As Integer = colArms(colArms.Count - 1).RobotNumber
                Dim nMask As Integer = CType(((2 ^ nMax) - 1) * (2 ^ colZones.ActiveZone.RobotsRequiredStartingBit), Integer)
                For i = 0 To 99
                    y = (x - 1) * 100 + i
                    oColors.Item(y).RobotsRequired.Value = oColors.Item(y).RobotsRequired.Value And nMask
                    sPLCData(i) = oColors.Item(y).RobotsRequired.Value.ToString
                Next

                sTag = sPrefix & "RobotsRequiredForColor" & x.ToString
                moPLC.TagName = sTag
                moPLC.PLCData = sPLCData
                Application.DoEvents()
            Next x

            'change log
            Dim nRobots As Integer = colArms.Count
            For i = 0 To nUB
                If oColors.Item(i).RobotsRequired.Changed Then

                    Dim sNewValue As String = StrReverse(mMathFunctions.CvBin(oColors.Item(i).RobotsRequired.Value, _
                            nRobots + 1))
                    Dim sOldValue As String = StrReverse(mMathFunctions.CvBin(oColors.Item(i).RobotsRequired.OldValue, _
                            nRobots + 1))
                    'Make sure we get the proper bits 'RJO 10/04/12
                    If colZones.ActiveZone.RobotsRequiredStartingBit = 1 Then
                        sNewValue = Strings.Right(sNewValue, nRobots)
                        sOldValue = Strings.Right(sOldValue, nRobots)
                    Else
                        sNewValue = Strings.Left(sNewValue, nRobots)
                        sOldValue = Strings.Left(sOldValue, nRobots)
                    End If

                    subUpdateChangeLog(sNewValue, sOldValue, lblRobotReqCap.Text, String.Empty, _
                                        oColors.Item(i).PlantNumber.Text)
                End If
            Next

            If mbPLCFail Then Return False

            Return True

        Catch ex As Exception

            Trace.WriteLine(ex.Message)
            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bSaveToRobot(ByRef oColors As clsSysColors, ByRef vRobot As clsArm, Optional ByVal bOpenerOnly As Boolean = False) As Boolean
        '********************************************************************************************
        'Description:  Save data stored on Robot
        '
        'Parameters: none
        'Returns:    True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/14/10  MSW     Add openeronly parameter to decide if 2k details get saved
        ' 04/05/12  MSW     Tech center mixed 2k and WB in the same zone.
        '********************************************************************************************


        Try

            Status = gcsRM.GetString("csSAVE_ROBOT") & " " & vRobot.Controller.Name

            If vRobot.IsOnLine Then
                'save that data
                Status = gcsRM.GetString("csSTARTING_SAVE_TO") & " " & vRobot.Controller.Name

                'drill down to color struct
                vRobot.ProgramName = "pavrsysc"
                Dim o As FRRobot.FRCVars = vRobot.ProgramVars
                Dim oo As FRRobot.FRCVars = DirectCast(o.Item("SYS_COLORS"), FRRobot.FRCVars)
                Dim oCVars As FRRobot.FRCVars = DirectCast(oo.Item("NODEDATA"), FRRobot.FRCVars)
                Dim oVar As FRRobot.FRCVar
                Dim sOldVal As String = String.Empty
                Dim sNewVal As String = String.Empty

                '2K
                Dim o2K As FRRobot.FRCVars = Nothing
                Dim o2KVars As FRRobot.FRCVars = Nothing
                Dim bUse2K As Boolean = vRobot.SystemColors.Use2K And (bOpenerOnly = False)
                Try
                    If bUse2K Then
                        o2K = DirectCast(o.Item("SYS_2K_COLRS"), FRRobot.FRCVars)
                        o2KVars = DirectCast(o2K.Item("NODEDATA"), FRRobot.FRCVars)
                        If o2KVars.Count = 0 Then
                            Debug.Assert(False)
                            'MSW 04/05/12 - Tech center mixed 2k and WB in the same zone.
                            'Deal with mixed 2K and WB robots.  Just skip saving the 2K if they aren't there.
                            'Assert statement is added above to check for this condition in case it causes trouble for someone else
                            'If you're here you probably have the GUI setup for 2K and a robot setup without it.
                            bUse2K = False
                        End If
                    End If
                Catch ex As Exception
                    bUse2K = False
                End Try
                
                ' pick a color, any color
                For nColIndex As Integer = 0 To oColors.Count - 1
                    Dim oCVar As FRRobot.FRCVars = DirectCast(oCVars(, nColIndex), FRRobot.FRCVars)

                    With oCVar

                        oVar = CType(.Item("valid"), FRRobot.FRCVar)
                        oVar.Value = "True"

                        oVar = CType(.Item("COLOR_DESC"), FRRobot.FRCVar)
                        sOldVal = oVar.Value.ToString
                        sNewVal = oColors.Item(nColIndex).Description.Text
                        If Len(sNewVal) > 24 Then sNewVal = Strings.Left(sNewVal, 24)

                        If sOldVal <> sNewVal Then
                            'do change log in gui routine to get full string
                            oVar.Value = sNewVal
                        End If

                        oVar = CType(.Item("COLOR_VALVE"), FRRobot.FRCVar)
                        sOldVal = oVar.Value.ToString

                        If sOldVal <> oColors.Item(nColIndex).Valve.Number.Value.ToString( _
                                                                                mLanguage.CurrentCulture) Then
                            oVar.Value = oColors.Item(nColIndex).Valve.Number.Value
                            subUpdateChangeLog(oColors.Item(nColIndex).Valve.Number.Value.ToString( _
                                                                mLanguage.CurrentCulture), sOldVal, _
                                                                lblValveNumberCap.Text, vRobot.Name, _
                                                                oColors(nColIndex).PlantNumber.Text)

                        End If


                    End With

                    'MSW 04/05/12 - Tech center mixed 2k and WB in the same zone.
                    'Deal with mixed 2K and WB robots.  Just skip saving the 2K if they aren't there.
                    'Assert statement is added above to check for this condition in case it causes trouble for someone else
                    If vRobot.SystemColors.Use2K And (bOpenerOnly = False) And bUse2K Then
                        oCVar = DirectCast(o2KVars(, nColIndex), FRRobot.FRCVars)

                        With oCVar

                            oVar = CType(.Item("HDR_VALVE"), FRRobot.FRCVar)
                            sOldVal = oVar.Value.ToString

                            If sOldVal <> oColors.Item(nColIndex).HardenerValve.Value.ToString( _
                                                                                    mLanguage.CurrentCulture) Then
                                oVar.Value = oColors.Item(nColIndex).HardenerValve.Value
                                subUpdateChangeLog(oColors.Item(nColIndex).HardenerValve.Value.ToString( _
                                                                    mLanguage.CurrentCulture), sOldVal, _
                                                                    lblHardenerCap.Text, vRobot.Name, _
                                                                    oColors(nColIndex).PlantNumber.Text)

                            End If


                            oVar = CType(.Item("HDR_RATIO"), FRRobot.FRCVar)
                            sOldVal = oVar.Value.ToString

                            If sOldVal <> oColors.Item(nColIndex).HardenerRatio.Value.ToString( _
                                                                                    mLanguage.CurrentCulture) Then
                                oVar.Value = oColors.Item(nColIndex).HardenerRatio.Value
                                subUpdateChangeLog(oColors.Item(nColIndex).HardenerRatio.Value.ToString( _
                                                                    mLanguage.CurrentCulture), sOldVal, _
                                                                    lblResinRatioCap.Text, vRobot.Name, _
                                                                    oColors(nColIndex).PlantNumber.Text)

                            End If


                            oVar = CType(.Item("RES_RATIO"), FRRobot.FRCVar)
                            sOldVal = oVar.Value.ToString

                            If sOldVal <> oColors.Item(nColIndex).ResinRatio.Value.ToString( _
                                                                                    mLanguage.CurrentCulture) Then
                                oVar.Value = oColors.Item(nColIndex).ResinRatio.Value
                                subUpdateChangeLog(oColors.Item(nColIndex).ResinRatio.Value.ToString( _
                                                                    mLanguage.CurrentCulture), sOldVal, _
                                                                    lblResinRatioCap.Text, vRobot.Name, _
                                                                    oColors(nColIndex).PlantNumber.Text)

                            End If


                            ' not used at present
                            oVar = CType(.Item("RES_SOLV_NO"), FRRobot.FRCVar)
                            sOldVal = oVar.Value.ToString

                            If sOldVal <> oColors.Item(nColIndex).ResinSolventValve.Value.ToString( _
                                                                                    mLanguage.CurrentCulture) Then
                                oVar.Value = oColors.Item(nColIndex).ResinSolventValve.Value
                                subUpdateChangeLog(oColors.Item(nColIndex).ResinSolventValve.Value.ToString( _
                                                                    mLanguage.CurrentCulture), sOldVal, _
                                                                    lblResinSolvCap.Text, vRobot.Name, _
                                                                    oColors(nColIndex).PlantNumber.Text)

                            End If


                            ' not used at present
                            oVar = CType(.Item("HDR_SOLV_NO"), FRRobot.FRCVar)
                            sOldVal = oVar.Value.ToString

                            If sOldVal <> oColors.Item(nColIndex).HardenerSolventValve.Value.ToString( _
                                                                                    mLanguage.CurrentCulture) Then
                                oVar.Value = oColors.Item(nColIndex).HardenerSolventValve.Value
                                subUpdateChangeLog(oColors.Item(nColIndex).HardenerSolventValve.Value.ToString( _
                                                                    mLanguage.CurrentCulture), sOldVal, _
                                                                    lblHardSolvCap.Text, vRobot.Name, _
                                                                    oColors(nColIndex).PlantNumber.Text)

                            End If



                        End With
                    End If
                Next

                Return True
            Else
                'warn user robot is offline
                Status = vRobot.Controller.Name & " " & gcsRM.GetString("csISUNAVAILABLE")

                Dim res As Response
                res = MessageBox.Show(vRobot.Controller.Name & vbCrLf & gpsRM.GetString("psROBOT_OFFLINE"), _
                                        gpsRM.GetString("psSAVE_WARNING"), MessageBoxButtons.YesNo, _
                                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, _
                                        MessageBoxOptions.DefaultDesktopOnly)

                'repaintme
                Application.DoEvents()

                If res = Response.Yes Then
                    Return True
                Else
                    Return False
                End If

            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            ShowErrorMessagebox(gcsRM.GetString("csSAVEFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Return False

        End Try

    End Function
    Private Function bSaveToRobots(ByRef oColors As clsSysColors) As Boolean
        '********************************************************************************************
        'Description:  Save data stored on Robots
        '
        'Parameters: none
        'Returns:    True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/23/10  MSW     handle controllers with all opener arms
        ' 10/14/10  MSW     Tuens out openers need to save the standard color dsata, just not the 2k. 
        '                   use a parameter in the routine call to decide if 2k details get saved
        '********************************************************************************************
        Dim nProg As Integer = 75
        If colControllers.Count > 0 Then
            nProg = CInt(75 / colControllers.Count)
        End If

        For i As Integer = 0 To colControllers.Count - 1
            Progress = Progress + nProg
            'Skip all-opener controllers
            Dim bOpenerOnly As Boolean = True
            For Each oArm As clsArm In colControllers.Item(i).Arms
                If oArm.IsOpener = False Then
                    bOpenerOnly = False
                    Exit For
                End If
            Next
            bSaveToRobot(oColors, colControllers.Item(i).Arms.Item(0), bOpenerOnly)
            'oBot = colControllers.Item(i).Arms(0)
            'colArms.Item(oBot.Name).Selected = True
        Next

        Return True

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
    Private Function bValidateColorTable(ByVal ZoneName As String, ByVal MaxColors As Integer, _
                            ByRef SCDataset As DataSet, ByRef UpdateFile As Boolean) As Boolean
        '*********************************************************************************************
        'Description:  Check the System Colors table to see that there are 1-Max_Colors (fanuc)
        '               (not effective colors ) and no fields are null - no duplicate checking here
        '
        'Parameters: Name of zone, number of colors, dataset from xml file and whether to rewrite file
        'Returns:    True if Save success
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*********************************************************************************************

        Try
            Dim dtColor As DataTable = SCDataset.Tables("[" & gsSYSC_DS_TABLENAME & "]")
            Dim oRows As DataRowCollection = dtColor.Rows()
            Dim nUB As Integer = dtColor.Rows.Count - 1

            'when we recurse, the rows are only marked for deletion, need to account for this
            For i As Integer = 0 To dtColor.Rows.Count - 1
                Dim r As DataRow = dtColor.Rows(i)
                If r.RowState = DataRowState.Deleted Then nUB -= 1
            Next

            Select Case nUB
                Case Is > (MaxColors - 1)
                    'whack the extras
                    For i As Integer = nUB To (MaxColors) Step -1
                        Dim r As DataRow = dtColor.Rows(i)
                        r.Delete()
                        Status = "Delete Color"
                    Next

                    UpdateFile = True
                    'recursion - danger will robinson!
                    If bValidateColorTable(ZoneName, MaxColors, SCDataset, _
                                                                UpdateFile) = False Then
                        Return False
                    End If

                Case Is < (MaxColors - 1)
                    'Add the extras
                    For i As Integer = (nUB + 1) To (MaxColors - 1)
                        Dim r As DataRow = dtColor.NewRow
                        bValidateColorDataRow(ZoneName, (i + 1), r)
                        dtColor.Rows.Add(r)
                        Status = "Add Color"
                    Next

                    UpdateFile = True
                    'recursion - danger will robinson!
                    If bValidateColorTable(ZoneName, MaxColors, SCDataset, _
                                                                UpdateFile) = False Then
                        Return False
                    End If

                Case Is = (MaxColors - 1)
                    'this is the right number - should be 1 thru max_colors
                    'verify the data
                    For i As Integer = 0 To nUB
                        Dim r As DataRow = oRows(i)
                        If bValidateColorDataRow(ZoneName, (i + 1), r) Then
                            'changed
                            UpdateFile = True
                        End If

                    Next
            End Select


            Return True

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Return False
        End Try

    End Function
    Private Shared Function bValidateColorDataRow(ByVal ZoneName As String, _
                                            ByVal FanucNumber As Integer, _
                                            ByRef dRow As DataRow) As Boolean
        '*********************************************************************************************
        'Description:  Check datarow for integrity
        '
        'Parameters:
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*********************************************************************************************
        Dim bChanged As Boolean = False

        If dRow.IsNull(gsSYSC_COL_FANUCNUM) Then
            bChanged = True
            dRow(gsSYSC_COL_FANUCNUM) = FanucNumber
        End If

        If CType(dRow(gsSYSC_COL_FANUCNUM), Integer) <> FanucNumber Then
            bChanged = True
            dRow(gsSYSC_COL_FANUCNUM) = FanucNumber
            'change desc as well
            dRow(gsSYSC_COL_DESC) = gcsRM.GetString("csCOLOR") & " " & _
                        FanucNumber.ToString(mLanguage.FixedCulture)
            dRow(gsSYSC_COL_PLANTNUM) = FanucNumber.ToString(mLanguage.FixedCulture)
        End If
        'check for nulls
        If dRow.IsNull(gsZONE_NAME) Then
            bChanged = True
            dRow(gsZONE_NAME) = ZoneName
        End If
        If CType(dRow(gsZONE_NAME), String) <> ZoneName Then
            bChanged = True
            dRow(gsZONE_NAME) = ZoneName
        End If

        If dRow.IsNull(gsSYSC_COL_DESC) Then
            bChanged = True
            dRow(gsSYSC_COL_DESC) = gcsRM.GetString("csNO_DESCRIPTION")
        End If

        If dRow.IsNull(gsSYSC_COL_VALVENUM) Then
            bChanged = True
            dRow(gsSYSC_COL_VALVENUM) = 1
        End If

        If dRow.IsNull(gsSYSC_COL_PLANTNUM) Then
            bChanged = True
            dRow(gsSYSC_COL_PLANTNUM) = FanucNumber
        End If

        If dRow.IsNull(gsSYSC_COL_ENABLE) Then
            bChanged = True
            dRow(gsSYSC_COL_ENABLE) = 0
        End If

        If dRow.IsNull(gsSYSC_COL_ARGB) Then
            bChanged = True
            dRow(gsSYSC_COL_ARGB) = 0
        End If

        If dRow.IsNull(gsCOL_SORTORDER) Then
            bChanged = True
            dRow(gsCOL_SORTORDER) = FanucNumber
        Else
            If CType(dRow(gsCOL_SORTORDER), Integer) <> FanucNumber Then
                dRow(gsCOL_SORTORDER) = FanucNumber
                bChanged = True
            End If
        End If

        Return bChanged

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
        ' 04/21/11 HGB      Remove PLC selection and hide Plant Color label for SA
        '********************************************************************************************
        Dim sText As String
        Dim i As Integer

        Progress = 10

        If EditsMade Then
            ' false means user pressed cancel
            If bAskForSave() = False Then
                cboZone.Text = msOldZone
                Progress = 100
                Exit Sub
            End If
        End If  'EditsMade

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

        If colZones.StandAlone Then
            lblPlantColorCap.Visible = False
        End If

        subInitFormText()

        Status = cboZone.Text & "  " & gcsRM.GetString("csSELECTED")

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

            subClearScreen()
            subEnableControls(False)

            Progress = 5

            colControllers = New clsControllers(colZones, False)
            SetUpStatusStrip(Me, colControllers)

            Status = gcsRM.GetString("csCONNECTING")

            colArms = LoadArmCollection(colControllers)

            Progress = 10

            If colZones.ActiveZone.UseSplitShell Then  '4/8/08
                mPWRobotCommon.LoadRobotBoxFromCollection(clbRobotsReq001, colArms, False)
            Else
                mPWRobotCommon.LoadRobotBoxFromCollection(clbRobotsReq001, colControllers, False)
            End If

            'at some point in time, may want to massage listbox to make it more visually appealing
            'here before it gets cloned

            Dim mnuSave As ToolStripDropDownMenu = CType(btnSave.DropDown, ToolStripDropDownMenu)
            Dim void As System.Drawing.Image = Nothing
            With mnuSave
                .Name = "mnuSave"
                .Items.Clear()

                For i = 0 To colControllers.Count - 1
                    sText = gcsRM.GetString("csSAVE_TO") & " " & colControllers.Item(i).Name
                    .Items.Add(sText, void, AddressOf subSaveMenuHandler)
                    .Items(i).Name = colControllers.Item(i).Name
                Next
                If Not colZones.StandAlone Then
                    sText = gcsRM.GetString("csSAVE_TO") & " " & gcsRM.GetString("csPLC")
                    .Items.Add(sText, void, AddressOf subSaveMenuHandler)
                    .Items(i).Name = gcsRM.GetString("csPLC")
                    i += 1
                End If
                sText = gcsRM.GetString("csSAVE_TO") & " " & gcsRM.GetString("csDATABASE")
                .Items.Add(sText, void, AddressOf subSaveMenuHandler)
                .Items(i).Name = gcsRM.GetString("csDATABASE")

            End With
            btnSave.DropDown = mnuSave

            Dim mnuLoad As ToolStripDropDownMenu = CType(btnUpload.DropDown, ToolStripDropDownMenu)
            With mnuLoad
                .Name = "mnuLoad"
                .Items.Clear()
                For i = 0 To colControllers.Count - 1
                    sText = gcsRM.GetString("csLOAD_FROM") & " " & colControllers.Item(i).Name
                    .Items.Add(sText, void, AddressOf subLoadMenuHandler)
                    .Items(i).Name = colControllers.Item(i).Name
                Next
                If Not colZones.StandAlone Then
                    sText = gcsRM.GetString("csLOAD_FROM") & " " & gcsRM.GetString("csPLC")
                    .Items.Add(sText, void, AddressOf subLoadMenuHandler)
                    .Items(i).Name = gcsRM.GetString("csPLC")
                    i += 1
                End If
                sText = gcsRM.GetString("csLOAD_FROM") & " " & gcsRM.GetString("csDATABASE")
                .Items.Add(sText, void, AddressOf subLoadMenuHandler)
                .Items(i).Name = gcsRM.GetString("csDATABASE")

            End With
            btnUpload.DropDown = mnuLoad

            colControllers.RefreshConnectionStatus()
            System.Windows.Forms.Application.DoEvents()

            'without this pause the robot gets checked for being online before it is connected
            Thread.Sleep(2500)
            System.Windows.Forms.Application.DoEvents()

            Progress = 15

            mRobot = Nothing

            For Each o As clsArm In colArms
                If o.IsOnLine Then
                    'should all be the same for the zone - use the first one we come to
                    mRobot = o
                    mRobot.ProgramName = "PAVRSYSC"
                    Dim oV As FRRobot.FRCVar = DirectCast(mRobot.ProgramVars("QTY_SYS_CLR"), FRRobot.FRCVar)
                    mnControllerMaxColors = CInt(oV.Value)
                    Status = gpsRM.GetString("psUSING") & " " & mRobot.Controller.Name
                    If colZones.ActiveZone.MaxColors <> mnControllerMaxColors Then
                        ' for developer
                        Dim sDb As String = "Number of colors in database = " & colZones.ActiveZone.MaxColors
                        Dim sRb As String = "Number of colors in controller = " & mnControllerMaxColors
                        MessageBox.Show(sDb & vbCrLf & sRb, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, _
                        MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
                    End If

                    Exit For

                End If
            Next

            Progress = 20


            If mRobot Is Nothing Then
                'no one on line
                MessageBox.Show(gcsRM.GetString("csUNABLE_CONNECT_CONTROLLERS"), colZones.ActiveZone.Name, _
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning, _
                                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)


                '3/19/08 if there are no arms in collection this crashes
                If colArms.Count = 0 Then
                    cboZone.SelectedIndex = -1
                    msOldZone = String.Empty
                    Exit Sub
                End If

                Dim reply As DialogResult = MessageBox.Show(gpsRM.GetString("psLOAD_FROM_DATABASE"), _
                                    colZones.ActiveZone.Name, _
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)

                If reply = DialogResult.Yes Then
                    mRobot = colArms(0)
                    If bLoadFromGUI(mRobot) Then
                        'dont know if 2k from database - hide tab
                        subShowHideTabs(False, False, colControllers(0).Arms(0).PigSystems > 0) 'NRU 161213 Piggable
                        mRobot.SystemColors.Update()
                        'this resets edit flag
                        DataLoaded = True
                        subFormatScreenLayout()
                        subShowNewPage()
                    End If
                Else
                    cboZone.SelectedIndex = -1
                    msOldZone = String.Empty
                End If

            Else
                'normal route

                'load me up buttercup
                subLoadData()
                If DataLoaded Then
                    subShowHideTabs(mRobot.SystemColors.Use2K, mRobot.SystemColors.UseTricoat, colControllers(0).Arms(0).PigSystems > 0) 'NRU 161213 Piggable
                    subFormatScreenLayout()
                    subShowNewPage()
                    subEnableControls(True)
                End If
            End If


            'statusbar text
            Status((Not gbUseWatch)) = gcsRM.GetString("csREADY")

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            mDebug.WriteEventToLog(msSCREEN_NAME & ":subChangeZone", ex.Message)
        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try


    End Sub
    Private Sub subClearScreen()
        '********************************************************************************************
        'Description:  This unloads all controls but the first row
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
        mnScrollPtr = 0
        subEnableControls(False)
        pnlTab1.Visible = False
        subUnloadPanel(pnlTab1, colTabTextboxes(1))
        Dim void As Collection(Of FocusedTextBox.FocusedTextBox) = Nothing
        pnlTab2.Visible = False
        subUnloadPanel(pnlTab2, void)
        mbEventBlocker = bTmp

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
            Case Connstat.frRNConnecting
                sImgKey = "imgSBRYellow"
                sTipText = gcsRM.GetString("csCONNECTING")
            Case Connstat.frRNDisconnecting
                sImgKey = "imgSBRYellow"
                sTipText = gcsRM.GetString("csDISCONNECTING")
            Case Connstat.frRNAvailable
                sImgKey = "imgSBRBlue"
                sTipText = gcsRM.GetString("csAVAILABLE")
            Case Connstat.frRNConnected
                sImgKey = "imgSBRGreen"
                sTipText = gcsRM.GetString("csCONNECTED")
            Case Connstat.frRNUnavailable
                sImgKey = "imgSBRRed"
                sTipText = gcsRM.GetString("csUNAVAILABLE")
            Case Connstat.frRNUnknown
                sImgKey = "imgSBRGrey"
                sTipText = gcsRM.GetString("csUNKNOWN")
            Case Connstat.frRNHeartbeatLost
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
        ' 03/03/11  JBW     Two coat checkboxes needed edit priviledges taken care of
        '********************************************************************************************
        Dim bRestOfControls As Boolean = False
        Dim sName As String = String.Empty

        btnClose.Enabled = True

        If bEnable = False Then
            btnSave.Enabled = False
            btnUndo.Enabled = False
            btnCopy.Enabled = False
            btnPrint.Enabled = False
            btnMultiView.Enabled = False
            btnChangeLog.Enabled = False
            btnStatus.Enabled = True
            bRestOfControls = False
            pnlMain.Enabled = False
            mnuPrintFile.Enabled = False
            btnUtilities.Enabled = False
        Else
            Select Case Privilege
                Case ePrivilege.None
                    btnSave.Enabled = False
                    btnUndo.Enabled = False
                    btnCopy.Enabled = False
                    btnPrint.Enabled = (True And DataLoaded)
                    btnChangeLog.Enabled = (True And DataLoaded)
                    btnStatus.Enabled = True
                    bRestOfControls = False
                    btnMultiView.Enabled = True
                    pnlMain.Enabled = True
                    btnUpload.Enabled = (True And DataLoaded)
                    mnuPrintFile.Enabled = False
                    btnUtilities.Enabled = False
                Case ePrivilege.Edit
                    btnSave.Enabled = (True And EditsMade)
                    btnUndo.Enabled = (True And EditsMade)
                    btnPrint.Enabled = (True And DataLoaded)
                    btnChangeLog.Enabled = (True And DataLoaded)
                    btnStatus.Enabled = True
                    bRestOfControls = (True And DataLoaded)
                    btnMultiView.Enabled = True
                    btnCopy.Enabled = False
                    pnlMain.Enabled = True
                    btnUpload.Enabled = (True And DataLoaded)
                    mnuPrintFile.Enabled = (True And DataLoaded)
                    btnUtilities.Enabled = (True And DataLoaded)
                Case ePrivilege.Copy
                    btnSave.Enabled = (True And EditsMade)
                    btnUndo.Enabled = (True And EditsMade)
                    btnPrint.Enabled = (True And DataLoaded)
                    btnChangeLog.Enabled = (True And DataLoaded)
                    btnStatus.Enabled = True
                    bRestOfControls = (True And DataLoaded)
                    btnMultiView.Enabled = True
                    btnCopy.Enabled = True
                    pnlMain.Enabled = True
                    btnUpload.Enabled = (True And DataLoaded)
                    mnuPrintFile.Enabled = (True And DataLoaded)
                    btnUtilities.Enabled = (True And DataLoaded)
            End Select
        End If

        'restof controls here
        LockAllTextBoxes(colTabTextboxes(1), (Not bRestOfControls))
        LockAllTextBoxes(colTabTextboxes(3), (Not bRestOfControls))
        LockAllTextBoxes(colTabTextboxes(4), (Not bRestOfControls))
        'jbw 03/03/11
        LockAllCheckBoxes(mcolCheckBoxes, bRestOfControls)
        'NRU 161213 Piggable
        LockAllComboBoxes(colTabComboBoxes(5), bRestOfControls)

        If IsNothing(mRobot) Then Exit Sub
        Dim c As CheckedListBox
        Dim l As Label
        For i As Integer = 1 To mRobot.SystemColors.EffectiveColors
            sName = "clbRobotsReq" & Format(i, "000")
            c = TryCast(pnlTab2.Controls(sName), CheckedListBox)
            If c Is Nothing = False Then
                c.Enabled = bRestOfControls
            End If

            sName = "lblColor" & Format(i, "000")
            l = TryCast(pnlTab1.Controls(sName), Label)
            If l Is Nothing = False Then
                l.Enabled = bRestOfControls
            End If
        Next

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
        subFormatTab1Layout()
        Progress = Progress + 8
        subFormatTab2Layout()
        Progress = Progress + 8
        subFormatTab3Layout()
        Progress = Progress + 8
        subFormatTab4Layout()
        Progress = Progress + 8
        'NRU 161213 Piggable
        subFormatTab5Layout()
        Progress = Progress + 8

    End Sub
    Private Sub subFormatTab1Layout()
        '********************************************************************************************
        'Description:  set up the screen layout
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/30/10  MSW     Change to look at ascii enable in option/style/color config
        ' 02/18/11  JBW     Add checkbox for "two coat color"
        ' 04/21/11  HGB     Hide plant colors for SA
        '********************************************************************************************
        Dim i As Integer = 0
        Dim sName As String
        Dim nRowPos As Integer = lblFanucColor001.Top + mnROWSPACE
        Dim nFColorLeft As Integer = lblFanucColor001.Left
        Dim nPColorLeft As Integer = ftbPlantColor001.Left
        Dim nDescLeft As Integer = ftbColorDesc001.Left
        Dim nValveLeft As Integer = ftbValveNumber001.Left
        Dim nValveDescLeft As Integer = ftbValveDesc001.Left
        Dim nTwoCoatLeft As Integer = ftbTwoCoat001.Left
        Dim nColorLeft As Integer = lblColor001.Left
        Dim cc As Control.ControlCollection = pnlTab1.Controls

        Try

            With pnlTab1
                .Visible = False
                .SuspendLayout()
                .AutoScroll = True
                .AutoScrollMargin = New Size(10, mnROWSPACE)
                .VerticalScroll.SmallChange = mnROWSPACE
            End With

            If colZones.StandAlone Then
                ftbPlantColor001.Visible = False
            End If

            ftbPlantColor001.NumericOnly = (mRobot.SystemColors.IsAscii = False)
            ftbValveNumber001.NumericOnly = True
            AddHandler ftbTwoCoat001.CheckedChanged, AddressOf cbTwoCoat_CheckedChanged

            AddHandler lblColor001.MouseClick, AddressOf subLabelClickHandler

            'populate grid based on existing 1st row
            For i = 2 To mRobot.SystemColors.EffectiveColors

                'fanuc color col
                sName = "lblfanucColor" & Format(i, "000")
                Dim l As Label = mScreenSetup.CloneLabel(lblFanucColor001, sName)
                l.Text = i.ToString(mLanguage.CurrentCulture)
                l.Location = New Point(nFColorLeft, nRowPos)
                cc.Add(l)

                'plant color col
                sName = "ftbPlantColor" & Format(i, "000")
                Dim ftbPC As FocusedTextBox.FocusedTextBox = mScreenSetup.CloneFocusedTextBox( _
                                                                ftbPlantColor001, sName)
                ftbPC.Location = New Point(nPColorLeft, nRowPos)
                If colZones.StandAlone Then
                    ftbPC.Visible = False
                End If
                cc.Add(ftbPC)


                'plant color Desc
                sName = "ftbColorDesc" & Format(i, "000")
                Dim ftbCD As FocusedTextBox.FocusedTextBox = mScreenSetup.CloneFocusedTextBox( _
                                                            ftbColorDesc001, sName)
                ftbCD.Location = New Point(nDescLeft, nRowPos)
                cc.Add(ftbCD)

                'color label col
                sName = "lblColor" & Format(i, "000")
                Dim lc As Label = mScreenSetup.CloneLabel(lblColor001, sName)
                lc.Text = String.Empty
                AddHandler lc.MouseClick, AddressOf subLabelClickHandler
                lc.Location = New Point(nColorLeft, nRowPos)
                cc.Add(lc)

                'Valve Number
                sName = "ftbValveNumber" & Format(i, "000")
                Dim ftbVN As FocusedTextBox.FocusedTextBox = mScreenSetup.CloneFocusedTextBox( _
                                                                ftbValveNumber001, sName)
                ftbVN.Location = New Point(nValveLeft, nRowPos)
                cc.Add(ftbVN)

                'Valve Desc
                sName = "ftbValveDesc" & Format(i, "000")
                Dim ftbVD As FocusedTextBox.FocusedTextBox = mScreenSetup.CloneFocusedTextBox( _
                                                                ftbValveDesc001, sName)
                ftbVD.Location = New Point(nValveDescLeft, nRowPos)
                cc.Add(ftbVD)
                If mRobot.SystemColors.UseTwoCoat Then
                    'jbw
                    'Two Coat Checkbox
                    sName = "ftbTwoCoat" & Format(i, "000")
                    Dim cbTC As CheckBox = mScreenSetup.CloneCheckBox(ftbTwoCoat001, sName)
                    cbTC.Location = New Point(nTwoCoatLeft, nRowPos)
                    cc.Add(cbTC)
                    AddHandler cbTC.CheckedChanged, AddressOf cbTwoCoat_CheckedChanged
                End If
                nRowPos += mnROWSPACE
            Next
            lblTwoCoats.Visible = mRobot.SystemColors.UseTwoCoat
            mScreenSetup.LoadTextBoxCollection(Me, "pnlTab1", colTabTextboxes(1))
            subSetTextBoxProperties(colTabTextboxes(1))
            LockAllTextBoxes(colTabTextboxes(1), True)


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        Finally
            pnlTab1.ResumeLayout()
            pnlTab1.Visible = True
        End Try

    End Sub
    Private Sub subFormatTab2Layout()
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
        Dim i As Integer = 0
        Dim sName As String
        Dim mnuSelect As New ContextMenuStrip
        Dim void As Image = Nothing
        Dim nRowPos As Integer = lblPlantColor001.Top + mnROWSPACE
        Dim nPColorLeft As Integer = lblPlantColor001.Left
        Dim nDescLeft As Integer = lblColorDesc001.Left
        Dim nRobotsLeft As Integer = clbRobotsReq001.Left
        Dim cc As Control.ControlCollection = pnlTab2.Controls

        Try

            With pnlTab2
                .Visible = False
                .SuspendLayout()
                .AutoScroll = True
                .AutoScrollMargin = New Size(10, mnROWSPACE)
                .VerticalScroll.SmallChange = mnROWSPACE
                If .HorizontalScroll.Visible Then
                    .HorizontalScroll.SmallChange = clbRobotsReq001.ColumnWidth
                End If
            End With

            'set up the checked list box before cloning
            AddHandler clbRobotsReq001.Validated, AddressOf subCheckListBoxValidatedHandler
            AddHandler clbRobotsReq001.ItemCheck, AddressOf subCheckListBoxItemCheckHandler
            AddHandler clbRobotsReq001.Leave, AddressOf subRobotsReqLeaveHandler
            clbRobotsReq001.Enabled = False
            clbRobotsReq001.HorizontalScrollbar = False

            'this is set to false to avoid undesirable behaviour if the box is too long and requires
            ' any kind of horizontal scroll the wrong box ends up getting checked
            clbRobotsReq001.CheckOnClick = False

            mnuSelect = New ContextMenuStrip
            With mnuSelect
                .Name = "clbRobotsReq001"
                .Items.Add(gcsRM.GetString("csSELECT_ALL"), void, AddressOf subMenuSelectHandler)
                .Items.Add(gcsRM.GetString("csSELECT_NONE"), void, AddressOf subMenuSelectHandler)
            End With
            clbRobotsReq001.ContextMenuStrip = mnuSelect

            lblRobotReqCap.Left = CInt(clbRobotsReq001.Left + _
                        (clbRobotsReq001.Size.Width / 2)) - CInt(lblRobotReqCap.Size.Width / 2)

            'populate tab page based on existing 1st row
            For i = 2 To mRobot.SystemColors.EffectiveColors

                'Plant color col
                sName = "lblPlantColor" & Format(i, "000")
                Dim l As Label = mScreenSetup.CloneLabel(lblPlantColor001, sName)
                l.Location = New Point(nPColorLeft, nRowPos)
                cc.Add(l)

                'plant color Desc
                sName = "lblColorDesc" & Format(i, "000")
                Dim l1 As Label = mScreenSetup.CloneLabel(lblColorDesc001, sName)
                l1.Location = New Point(nDescLeft, nRowPos)
                cc.Add(l1)

                'Robots Required
                sName = "clbRobotsReq" & Format(i, "000")
                Dim c As CheckedListBox = mScreenSetup.CloneCheckedListBox( _
                                                                clbRobotsReq001, sName)
                AddHandler c.Validated, AddressOf subCheckListBoxValidatedHandler
                AddHandler c.ItemCheck, AddressOf subCheckListBoxItemCheckHandler
                AddHandler c.Leave, AddressOf subRobotsReqLeaveHandler

                'could not successfully clone context menu, set up here
                mnuSelect = New ContextMenuStrip
                With mnuSelect
                    .Name = sName
                    .Items.Add(gcsRM.GetString("csSELECT_ALL"), void, AddressOf subMenuSelectHandler)
                    .Items.Add(gcsRM.GetString("csSELECT_NONE"), void, AddressOf subMenuSelectHandler)
                End With
                c.ContextMenuStrip = mnuSelect

                c.Location = New Point(nRobotsLeft, nRowPos)
                cc.Add(c)

                nRowPos += mnROWSPACE
            Next


        Catch ex As Exception

            Trace.WriteLine(ex.Message)

        Finally
            subEnableControls(True)
            pnlTab2.ResumeLayout()
            pnlTab2.Visible = True
        End Try

    End Sub
    Private Sub subFormatTab3Layout()
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
        Dim i As Integer = 0
        Dim sName As String
        Dim nRowPos As Integer = lblPlantColorT3001.Top + mnROWSPACE
        Dim nPColorLeft As Integer = lblPlantColorT3001.Left
        Dim nDescLeft As Integer = lblColorDescT3001.Left
        Dim nValveLeft As Integer = ftbHardValve001.Left
        Dim nResinLeft As Integer = ftbResinRatio001.Left
        Dim nColonLeft As Integer = lblColon001.Left
        Dim nHardLeft As Integer = ftbHardRatio001.Left
        Dim nResinSolvLeft As Integer = ftbResinSolv001.Left
        Dim nHardSolvLeft As Integer = ftbHardSolv001.Left
        Dim cc As Control.ControlCollection = pnlTab3.Controls

        Try


            With pnlTab3
                .Visible = False
                .SuspendLayout()
                .AutoScroll = True
                .AutoScrollMargin = New Size(10, mnROWSPACE)
                .VerticalScroll.SmallChange = mnROWSPACE
            End With


            ftbHardValve001.NumericOnly = True
            ftbHardRatio001.NumericOnly = True
            ftbResinRatio001.NumericOnly = True
            ftbHardSolv001.NumericOnly = True
            ftbResinSolv001.NumericOnly = True

            cc.Add(lblPlantColorT3001)
            cc.Add(lblColorDescT3001)
            cc.Add(ftbHardValve001)
            cc.Add(ftbResinRatio001)
            cc.Add(lblColon001)
            cc.Add(ftbHardRatio001)
            cc.Add(ftbResinSolv001)
            cc.Add(ftbHardSolv001)
            'populate grid based on existing 1st row
            For i = 2 To mRobot.SystemColors.EffectiveColors

                'plant color col
                sName = "lblPlantColorT3" & Format(i, "000")
                Dim lblPC As Label = mScreenSetup.CloneLabel(lblPlantColorT3001, sName)
                lblPC.Location = New Point(nPColorLeft, nRowPos)
                cc.Add(lblPC)

                'Desc col
                sName = "lblColorDescT3" & Format(i, "000")
                Dim lblDesc As Label = mScreenSetup.CloneLabel(lblColorDescT3001, sName)
                lblDesc.Location = New Point(nDescLeft, nRowPos)
                cc.Add(lblDesc)


                'Hardener valve col
                sName = "ftbHardValve" & Format(i, "000")
                Dim ftbHV As FocusedTextBox.FocusedTextBox = mScreenSetup.CloneFocusedTextBox( _
                                                            ftbHardValve001, sName)
                ftbHV.Location = New Point(nValveLeft, nRowPos)
                cc.Add(ftbHV)

                'resin ratio col
                sName = "ftbResinRatio" & Format(i, "000")
                Dim ftbRR As FocusedTextBox.FocusedTextBox = mScreenSetup.CloneFocusedTextBox( _
                                                            ftbResinRatio001, sName)
                ftbRR.Location = New Point(nResinLeft, nRowPos)
                cc.Add(ftbRR)

                'Colon col
                sName = "lblColon" & Format(i, "000")
                Dim lblC As Label = mScreenSetup.CloneLabel(lblColon001, sName)
                lblC.Location = New Point(nColonLeft, nRowPos)
                lblC.Text = ":"
                cc.Add(lblC)

                'resin ratio col
                sName = "ftbHardRatio" & Format(i, "000")
                Dim ftbHR As FocusedTextBox.FocusedTextBox = mScreenSetup.CloneFocusedTextBox( _
                                                            ftbHardRatio001, sName)
                ftbHR.Location = New Point(nHardLeft, nRowPos)
                cc.Add(ftbHR)

                'resin solvent col
                sName = "ftbResinSolv" & Format(i, "000")
                Dim ftbRS As FocusedTextBox.FocusedTextBox = mScreenSetup.CloneFocusedTextBox( _
                                                            ftbResinSolv001, sName)
                ftbRS.Location = New Point(nResinSolvLeft, nRowPos)
                cc.Add(ftbRS)

                'resin solvent col
                sName = "ftbHardSolv" & Format(i, "000")
                Dim ftbHS As FocusedTextBox.FocusedTextBox = mScreenSetup.CloneFocusedTextBox( _
                                                            ftbHardSolv001, sName)
                ftbHS.Location = New Point(nHardSolvLeft, nRowPos)
                cc.Add(ftbHS)

                nRowPos += mnROWSPACE

            Next

            mScreenSetup.LoadTextBoxCollection(Me, "pnlTab3", colTabTextboxes(3))
            subSetTextBoxProperties(colTabTextboxes(3))
            LockAllTextBoxes(colTabTextboxes(3), True)


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        Finally
            pnlTab3.ResumeLayout()
            pnlTab3.Visible = True
        End Try


    End Sub
    Private Sub subFormatTab4Layout()
        '********************************************************************************************
        'Description:  set up the screen layout
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/23/10  BTK     Added Tricoat Tab
        '********************************************************************************************
        Dim i As Integer = 0
        Dim sName As String
        Dim mnuSelect As New ContextMenuStrip
        Dim void As Image = Nothing
        Dim nRowPos As Integer = lblTab4PlantColor001.Top + mnROWSPACE
        Dim nPColorLeft As Integer = lblTab4PlantColor001.Left
        Dim nDescLeft As Integer = lblTab4ColorDesc001.Left
        Dim nTricoatLeft As Integer = ftbTricoatColor001.Left
        Dim cc As Control.ControlCollection = pnlTab4.Controls

        Try

            With pnlTab4
                .Visible = False
                .SuspendLayout()
                .AutoScroll = True
                .AutoScrollMargin = New Size(10, mnROWSPACE)
                .VerticalScroll.SmallChange = mnROWSPACE
                If .HorizontalScroll.Visible Then
                    .HorizontalScroll.SmallChange = ftbTricoatColor001.Width
                End If
            End With

            ftbTricoatColor001.NumericOnly = (mRobot.SystemColors.IsAscii = False)

            'populate tab page based on existing 1st row
            For i = 2 To mRobot.SystemColors.EffectiveColors

                'Plant color col
                sName = "lblTab4PlantColor" & Format(i, "000")
                Dim l As Label = mScreenSetup.CloneLabel(lblPlantColor001, sName)
                l.Location = New Point(nPColorLeft, nRowPos)
                cc.Add(l)

                'plant color Desc
                sName = "lblTab4ColorDesc" & Format(i, "000")
                Dim l1 As Label = mScreenSetup.CloneLabel(lblColorDesc001, sName)
                l1.Location = New Point(nDescLeft, nRowPos)
                cc.Add(l1)

                'tricoat color col
                sName = "ftbTricoatColor" & Format(i, "000")
                Dim ftbPC As FocusedTextBox.FocusedTextBox = mScreenSetup.CloneFocusedTextBox( _
                                                                ftbTricoatColor001, sName)
                ftbPC.Location = New Point(nTricoatLeft, nRowPos)
                cc.Add(ftbPC)

                nRowPos += mnROWSPACE
            Next

            mScreenSetup.LoadTextBoxCollection(Me, "pnlTab4", colTabTextboxes(4))
            subSetTextBoxProperties(colTabTextboxes(4))
            LockAllTextBoxes(colTabTextboxes(4), True)

        Catch ex As Exception

            Trace.WriteLine(ex.Message)

        Finally
            subEnableControls(True)
            pnlTab4.ResumeLayout()
            pnlTab4.Visible = True
        End Try
    End Sub
    Private Sub subFormatTab5Layout()
        '********************************************************************************************
        'Description:  set up the screen layout
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/20/13  NRU     Added Piggable tab
        '********************************************************************************************
        'NRU 161213 Piggable
        Dim i As Long = 0
        Dim sName As String
        Dim mnuSelect As New ContextMenuStrip
        Dim void As Image = Nothing
        Dim nRowPos As Integer = lblSysNum001crap.Top + mnROWSPACE
        Dim nPColorLeft As Integer = lblSysNum001crap.Left
        Dim nDescLeft As Integer = cboValveDesc001crap.Left
        Dim cc As Control.ControlCollection = pnlTab5.Controls

        Try

            With pnlTab5
                .Visible = False
                .SuspendLayout()
                .AutoScroll = True
                .AutoScrollMargin = New Size(10, mnROWSPACE)
                .VerticalScroll.SmallChange = mnROWSPACE
                If .HorizontalScroll.Visible Then
                    .HorizontalScroll.SmallChange = cboValveDesc001crap.Width
                End If
            End With

            'populate tab page based on existing 1st row
            For i = 2 To colControllers(0).Arms(0).PigSystems


                '01/03/13 referenece pig systems 
                'mRobot.SystemColors.EffectiveColors

                'Piggable System Number
                sName = "lblSysNum" & Format(i, "000")
                Dim l As Label = mScreenSetup.CloneLabel(lblSysNum001crap, sName)
                l.Text = CStr(i)
                l.Location = New Point(nPColorLeft, nRowPos)
                cc.Add(l)

                'Valve Description (Piggable Color Desc)
                sName = "cboValveDesc" & Format(i, "000")
                Dim cbo As ComboBox = mScreenSetup.CloneComboBox(cboValveDesc001crap, sName)
                cbo.Location = New Point(nDescLeft, nRowPos)
                cc.Add(cbo)

                nRowPos += mnROWSPACE
            Next

            mScreenSetup.LoadComboBoxCollection(Me, "pnlTab5", colTabComboBoxes(5))
            subSetComboBoxProperties(colTabComboBoxes(5))
            LockAllComboBoxes(colTabComboBoxes(5), True)
        Catch ex As Exception

            Trace.WriteLine(ex.Message)

        Finally
            subEnableControls(True)
            pnlTab5.ResumeLayout()
            pnlTab5.Visible = True
        End Try
    End Sub
    Private Shared Sub subInitFormText()
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
            'tab 1
            frmMain.TabPage1.Text = .GetString("psTAB1CAPTION")
            frmMain.lblFanucColorCap.Text = .GetString("psFANUCCOLOR")
            frmMain.lblPlantColorCap.Text = .GetString("psPLANTCOLOR")
            frmMain.lblColorDescCap.Text = .GetString("psCOLOR_DESC")
            frmMain.lblValveNumberCap.Text = .GetString("psVALVE_NUMBER")
            frmMain.lblValveDescCap.Text = .GetString("psVALVE_DESC")

            'tab 2
            frmMain.TabPage2.Text = .GetString("psTAB2CAPTION")
            frmMain.lblPlantColor2Cap.Text = .GetString("psPLANTCOLOR")
            frmMain.lblColorDesc2Cap.Text = .GetString("psCOLOR_DESC")
            frmMain.lblRobotReqCap.Text = .GetString("psROBOTS_REQ")

            'tab 3
            frmMain.TabPage3.Text = .GetString("psTAB3CAPTION")
            frmMain.lblPlantColor3Cap.Text = .GetString("psPLANTCOLOR")
            frmMain.lblColorDesc3Cap.Text = .GetString("psCOLOR_DESC")
            frmMain.lblHardenerCap.Text = .GetString("psHARDENER_VALVE_CAP")
            frmMain.lblHardenerRatioCap.Text = .GetString("psHARDENER_RATIO_CAP")
            frmMain.lblResinRatioCap.Text = .GetString("psRESIN_RATIO_CAP")
            frmMain.lblResinSolvCap.Text = .GetString("psRESIN_SOLV_CAP")
            frmMain.lblHardSolvCap.Text = .GetString("psHARD_SOLV_CAP")


            'tab 4
            frmMain.TabPage4.Text = .GetString("psTAB4CAPTION")
            frmMain.lblPlantColor4Cap.Text = .GetString("psPLANTCOLOR")
            frmMain.lblColorDesc4Cap.Text = .GetString("psCOLOR_DESC")
            frmMain.lblTricoat4Cap.Text = .GetString("psTRICOATCOLOR")

            'NRU 161213 Piggable
            frmMain.TabPage5.Text = .GetString("psTAB5CAPTION")

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

        'for debug
        ''mPWCommon.oWatch = New Stopwatch
        ''mPWCommon.oWatch.Start()
        ''gbUseWatch = True

        Status = gcsRM.GetString("csINITALIZING")
        Progress = 1

        Try
            DataLoaded = False
            EditsMade = False
            mbScreenLoaded = False

            subProcessCommandLine()

            Me.Show()
            Me.Refresh()

            mPrintHtml = New clsPrintHtml(msSCREEN_NAME, True, 30)

            'init the old password for now 'RJO 03/21/12
            'moPassword = New PWPassword.PasswordObject
            'mPassword.InitPassword(moPassword, moPrivilege, LoggedOnUser, msSCREEN_NAME)
            'If LoggedOnUser <> String.Empty Then
            '    If moPrivilege.ActionAllowed Then
            '        Privilege = mPassword.PrivilegeStringToEnum(moPrivilege.Privilege)
            '    Else
            '        Privilege = ePrivilege.None
            '    End If
            'Else
            '    Privilege = ePrivilege.None
            'End If

            Progress = 5

            Me.Text = gpsRM.GetString("psSCREENCAPTION")

            Progress = 30

            'init new IPC and new Password 'RJO 03/21/12
            oIPC = New Paintworks_IPC.clsInterProcessComm
            moPassword = New clsPWUser
            Progress = 50

            mScreenSetup.InitializeForm(Me)
            colZones = New clsZones(String.Empty)
            subShowHideTabs(False, False, False) 'NRU 161213 Piggable
            subInitFormText()
            Application.DoEvents()

            '4/21/08
            If colZones.PaintShopComputer Then
                mScreenSetup.LoadZoneBox(cboZone, colZones, False)
            Else
                mScreenSetup.LoadZoneBox(cboZone, colZones, False, colZones.ActiveZone.ServerName)
            End If
            Application.DoEvents()


            Progress = 70

            Application.DoEvents()
            Progress = 98
            subEnableControls(True)

            Application.DoEvents()
            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound


            'statusbar text
            If cboZone.Text = String.Empty Then
                Status(True) = gcsRM.GetString("csSELECT_ZONE")
            End If


            mbScreenLoaded = True
            If cboZone.Items.Count = 1 Then
                'this selects the zone in cases of just one zone. fires event to call subchangezone
                cboZone.Text = cboZone.Items(0).ToString
            End If



        Catch ex As Exception

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

            If colZones.PaintShopComputer Then  '6/1/07
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, _
                        colZones.ActiveZone.IsRemoteZone, False)
            Else
                DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, _
                        mbRemoteZone, True)
            End If

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
        '********************************************************************************************
        Dim cTmp As Cursor = Me.Cursor

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        DataLoaded = False

        Try
            Status = gcsRM.GetString("csLOADINGDATA")

            Progress = 25

            If bLoadFromGUI(mRobot) Then
                Progress = 30
                If bLoadFromPLC(mRobot.SystemColors) Then
                    Progress = 50

                    'plc and database could differ
                    mRobot.SystemColors.RefreshValveDescriptions()

                    If LoggedOnUser <> String.Empty Then
                        'check for copy button
                        Privilege = ePrivilege.Copy
                        'back to edit
                        Privilege = ePrivilege.Edit
                    End If

                    mRobot.SystemColors.Update()
                    'this resets edit flag
                    DataLoaded = True

                    Status = gcsRM.GetString("csLOADDONE")

                Else
                    'Load Failed


                    Dim reply As DialogResult = MessageBox.Show(gpsRM.GetString("psLOAD_FROM_DATABASE"), _
                                        colZones.ActiveZone.Name, _
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                                        MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)

                    If reply = DialogResult.Yes Then
                        'this resets edit flag
                        DataLoaded = True
                    Else
                        cboZone.SelectedIndex = -1
                        msOldZone = String.Empty
                    End If


                End If  'bLoadFromPLC()
            Else
                'Load Failed
            End If  ' bLoadFromGUI()


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            DataLoaded = False
        Finally

            Me.Cursor = cTmp

        End Try
    End Sub
    Private Sub subLoadTab1Data(ByRef oColor As clsSysColor)
        '********************************************************************************************
        'Description:  Load Tab 1 data
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sName As String = String.Empty
        Dim sInx As String = Format(oColor.FanucNumber, "000")
        Dim cc As Control.ControlCollection = pnlTab1.Controls

        sName = "ftbPlantColor" & sInx
        cc.Item(sName).Text = oColor.PlantNumber.Text
        If oColor.PlantNumber.Changed Then
            cc.Item(sName).ForeColor = Color.Red
        Else
            cc.Item(sName).ForeColor = Color.Black
        End If

        sName = "ftbColorDesc" & sInx
        cc.Item(sName).Text = oColor.Description.Text
        If oColor.Description.Changed Then
            cc.Item(sName).ForeColor = Color.Red
        Else
            cc.Item(sName).ForeColor = Color.Black
        End If

        sName = "ftbValveNumber" & sInx
        Debug.Print(oColor.Valve.Number.Value.ToString(mLanguage.FixedCulture))
        cc.Item(sName).Text = oColor.Valve.Number.Value.ToString(mLanguage.FixedCulture)
        If oColor.Valve.Number.Changed Then
            cc.Item(sName).ForeColor = Color.Red
        Else
            cc.Item(sName).ForeColor = Color.Black
        End If

        sName = "ftbValveDesc" & sInx
        cc.Item(sName).Text = oColor.Valve.Description.Text
        If oColor.Valve.Description.Changed Then
            cc.Item(sName).ForeColor = Color.Red
        Else
            cc.Item(sName).ForeColor = Color.Black
        End If

        If mRobot.SystemColors.UseTwoCoat Then
            'jbw added for two coats by color
            sName = "ftbtwocoat" & sInx
            Dim tempcheckbox As CheckBox = DirectCast(cc.Item(sName), CheckBox)
            tempcheckbox.Checked = oColor.TwoCoats.Value
        End If

        sName = "ftbValveDesc" & sInx
        If oColor.Valve.Description.Changed Then
            cc.Item(sName).ForeColor = Color.Red
        Else
            cc.Item(sName).ForeColor = Color.Black
        End If

        sName = "lblColor" & sInx
        Dim l As Label = TryCast(cc.Item(sName), Label)
        If l Is Nothing Then Exit Sub
        l.BackColor = oColor.DisplayColor

    End Sub
    Private Sub subLoadTab2Data(ByRef oColor As clsSysColor)
        '********************************************************************************************
        'Description:  Load Tab 2 data
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/28/09  RJO     Add RobotsRequiredStatingBit offset Code
        '********************************************************************************************
        Dim sName As String = String.Empty
        Dim sInx As String = Format(oColor.FanucNumber, "000")
        Dim cc As Control.ControlCollection = pnlTab2.Controls
        Dim nBitOffset As Integer = colZones.ActiveZone.RobotsRequiredStartingBit ' 11/28/09  RJO


        sName = "lblPlantColor" & sInx
        cc.Item(sName).Text = oColor.PlantNumber.Text
        cc.Item(sName).ForeColor = Color.Black

        sName = "lblColorDesc" & sInx
        cc.Item(sName).Text = oColor.Description.Text
        cc.Item(sName).ForeColor = Color.Black

        Dim nValue As Integer = oColor.RobotsRequired.Value

        sName = "clbRobotsReq" & sInx
        Dim clb As CheckedListBox = DirectCast(cc.Item(sName), CheckedListBox)
        If oColor.RobotsRequired.OldValue <> nValue Then
            clb.ForeColor = Color.Red
            EditsMade = True
        Else
            clb.ForeColor = Color.Black
        End If
        'lets hope there are no more than 15 robots in a zone....
        'For i As Integer = 0 To clb.Items.Count - 1
        '    If i > 15 Then Exit Sub
        '    If (nValue And gnBitVal(i + nBitOffset)) = gnBitVal(i + nBitOffset) Then ' 11/28/09  RJO
        '        clb.SetItemChecked(i, True)
        '    Else
        '        clb.SetItemChecked(i, False)
        '    End If
        'Next
        For nArm As Integer = 0 To colArms.Count - 1
            'lets hope there are no more than 15 robots in a zone....
            If (nValue And gnBitVal(colArms(nArm).RobotNumber + nBitOffset - 1)) = gnBitVal(colArms(nArm).RobotNumber + nBitOffset - 1) Then
                clb.SetItemChecked(nArm, True)
            Else
                clb.SetItemChecked(nArm, False)
            End If
        Next
    End Sub
    Private Sub subLoadTab3Data(ByRef oColor As clsSysColor)
        '********************************************************************************************
        'Description:  Load Tab 3 data
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sName As String = String.Empty
        Dim sInx As String = Format(oColor.FanucNumber, "000")
        Dim cc As Control.ControlCollection = pnlTab3.Controls

        sName = "lblPlantColorT3" & sInx
        cc.Item(sName).Text = oColor.PlantNumber.Text

        sName = "lblColorDescT3" & sInx
        cc.Item(sName).Text = oColor.Description.Text

        sName = "ftbHardValve" & sInx
        cc.Item(sName).Text = oColor.HardenerValve.Value.ToString(mLanguage.FixedCulture)
        If oColor.HardenerValve.Changed Then
            cc.Item(sName).ForeColor = Color.Red
        Else
            cc.Item(sName).ForeColor = Color.Black
        End If

        sName = "ftbResinRatio" & sInx
        cc.Item(sName).Text = oColor.ResinRatio.Value.ToString(mLanguage.FixedCulture)
        If oColor.ResinRatio.Changed Then
            cc.Item(sName).ForeColor = Color.Red
        Else
            cc.Item(sName).ForeColor = Color.Black
        End If

        sName = "ftbHardRatio" & sInx
        cc.Item(sName).Text = oColor.HardenerRatio.Value.ToString(mLanguage.FixedCulture)
        If oColor.HardenerRatio.Changed Then
            cc.Item(sName).ForeColor = Color.Red
        Else
            cc.Item(sName).ForeColor = Color.Black
        End If

        sName = "ftbResinSolv" & sInx
        cc.Item(sName).Text = oColor.ResinSolventValve.Value.ToString(mLanguage.FixedCulture)
        If oColor.ResinSolventValve.Changed Then
            cc.Item(sName).ForeColor = Color.Red
        Else
            cc.Item(sName).ForeColor = Color.Black
        End If

        sName = "ftbHardSolv" & sInx
        cc.Item(sName).Text = oColor.HardenerSolventValve.Value.ToString(mLanguage.FixedCulture)
        If oColor.HardenerSolventValve.Changed Then
            cc.Item(sName).ForeColor = Color.Red
        Else
            cc.Item(sName).ForeColor = Color.Black
        End If

    End Sub
    Private Sub subLoadTab4Data(ByRef oColor As clsSysColor)
        '********************************************************************************************
        'Description:  Load Tab 4 data
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/23/10  BTK     Added Tricoat Tab
        '********************************************************************************************
        Dim sName As String = String.Empty
        Dim sInx As String = Format(oColor.FanucNumber, "000")
        Dim cc As Control.ControlCollection = pnlTab4.Controls


        sName = "lblTab4PlantColor" & sInx
        cc.Item(sName).Text = oColor.PlantNumber.Text
        cc.Item(sName).ForeColor = Color.Black

        sName = "lblTab4ColorDesc" & sInx
        cc.Item(sName).Text = oColor.Description.Text
        cc.Item(sName).ForeColor = Color.Black

        sName = "ftbTricoatColor" & sInx
        cc.Item(sName).Text = oColor.Tricoat.Text
        cc.Item(sName).ForeColor = Color.Black

    End Sub
    Private Sub subLoadTab5Data()
        '********************************************************************************************
        'Description:  Load Tab 5 data
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/20/13  NRU     Piggable Tab
        '********************************************************************************************
        'NRU 161213 Piggable
        Dim sName As String = String.Empty
        Dim cc As Control.ControlCollection = pnlTab5.Controls
        Dim lngCBOBoxes As Long
        Dim sPrefix As String = colZones.ActiveZone.PLCTagPrefix
        Dim sTag As String = String.Empty
        Dim sColorValveNames As String() = GetValveTableDataset(colZones.ActiveZone)
        Dim intValves As Integer
        Dim intTargetIndex As Integer

        'Piggable systems are by robot in the PLC. This Piggable tab is by zone.
        'This means we group together by reading from the first robot in the zone
        'and writing to all robots in the zone.

        Try
            'Read the currently loaded piggable colors from the PLC
            'first robot's data applies to all.
            If colZones.ActiveZone.PLCType = ePLCType.None Then Exit Sub
            Status = gcsRM.GetString("csLOAD_PLC")
            moPLC = New clsPLCComm
            moPLC.ZoneName = colZones.ActiveZone.Name
            sTag = sPrefix & "PigSystemsRC1"
            moPLC.TagName = sTag
            strPigSystems = moPLC.PLCData
            strPrevPigSystems = moPLC.PLCData

            'Go through each combobox. The first controller's number of systems applies to all.
            For lngCBOBoxes = 1 To colControllers(0).Arms(0).PigSystems
                'Get ahold of this particular cbo
                sName = "cboValveDesc" & Format(lngCBOBoxes, "000")
                Dim cb As ComboBox = CType(cc.Item(sName), ComboBox)
                cb.Items.Clear()
                cb.Items.Add("")
                'Put all valve descriptions in this cbo.  All that are piggable (valve>mainline) that is.
                For intValves = 0 To UBound(sColorValveNames)
                    If intValves > mnMAX_MAINLINE_VALVES - 1 Then
                        cb.Items.Add(sColorValveNames(intValves))
                    End If
                Next
                'Set the index of this cbo to the currently selected color retrieved from PLC.
                'Trap any BS you would get from gibberish in the PLC before the system is actually
                'setup and used.
                intTargetIndex = CInt(strPigSystems(CInt(lngCBOBoxes - 1))) - mnMAX_MAINLINE_VALVES '- 1
                If intTargetIndex > -2 And intTargetIndex < cb.Items.Count Then
                    cb.SelectedIndex = intTargetIndex
                Else
                    If cb.Items.Count > 0 Then
                        cb.SelectedIndex = 0
                    Else
                        cb.SelectedIndex = -1
                    End If
                End If
            Next

        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csLOADFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
            mDebug.WriteEventToLog(msSCREEN_NAME & ":bLoadFromGUI", ex.Message)
        End Try

    End Sub
    Private Sub subMenuSelectHandler(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  context select all/none handler
        '
        'Parameters: sender is menuitem
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim m As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim c As CheckedListBox = DirectCast(pnlTab2.Controls.Item(m.Owner.Name), CheckedListBox)
        Dim bTmp As Boolean

        Select Case m.Text
            Case gcsRM.GetString("csSELECT_ALL")
                bTmp = True
            Case Else
                bTmp = False
        End Select

        For i As Integer = 0 To c.Items.Count - 1
            c.SetItemChecked(i, bTmp)
        Next

        subCheckListBoxValidatedHandler(c, e)

    End Sub
    Private Function bPrintdocSA(ByVal bPrint As Boolean, Optional ByVal bExportCSV As Boolean = False) As Boolean
        '********************************************************************************************
        'Description:  build the print document
        '
        'Parameters:  bPrint - send to printer
        'Returns:   true if successful, OK to continue with preview or page setup
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 4/21/11  HGB      Copied from bPrintDoc and modified for SA (remove Plant Color and Robots Req'd)
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
            sTitle(1) = String.Empty
            Dim sSubTitle(0) As String
            sSubTitle(0) = String.Empty

            '******************* Standard system color table
            'column headers
            Dim sText(0) As String
            sText(0) = lblFanucColorCap.Text & vbTab & _
                    lblColorDescCap.Text & vbTab & _
                    lblValveNumberCap.Text & vbTab & _
                    lblValveDescCap.Text

            ReDim Preserve sText(mRobot.SystemColors.EffectiveColors)
            For i As Integer = 0 To mRobot.SystemColors.EffectiveColors - 1
                With mRobot.SystemColors.Item(i)
                    sText(i + 1) = (i + 1).ToString(mLanguage.FixedCulture) & _
                            vbTab & .Description.Text & vbTab & _
                            .Valve.Number.Value.ToString(mLanguage.CurrentCulture) & vbTab & _
                            .Valve.Description.Text
                    sText(i + 1) = sText(i + 1).Replace("<", "")
                    sText(i + 1) = sText(i + 1).Replace(">", "")
                    Dim nValue As Integer = .RobotsRequired.Value
                End With
            Next
            dim bCancel as boolean  = false
            mPrintHtml.subStartDoc(Status, sPageTitle, bExportCSV, bCancel)
            if bCancel then
              return(false)
            end if
            mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle, sPageTitle)

            '******************* 2K
            If mRobot.SystemColors.Use2K Then
                sTitle(0) = colZones.SiteName & " - " & colZones.CurrentZone
                sTitle(1) = String.Empty
                sSubTitle(0) = TabPage3.Text

                'column headers
                'column headers
                sText(0) = lblFanucColorCap.Text & vbTab & _
                        lblColorDesc3Cap.Text & vbTab & lblHardenerCap.Text & vbTab & _
                        lblResinRatioCap.Text & vbTab & lblHardenerRatioCap.Text & vbTab & _
                        lblResinSolvCap.Text & vbTab & lblHardSolvCap.Text

                For i As Integer = 0 To mRobot.SystemColors.EffectiveColors - 1
                    With mRobot.SystemColors.Item(i)
                        sText(i + 1) = (i + 1).ToString(mLanguage.FixedCulture) & vbTab & _
                                .Description.Text & vbTab & _
                                .HardenerValve.Value.ToString(mLanguage.CurrentCulture) & vbTab & _
                                .ResinRatio.Value.ToString(mLanguage.CurrentCulture) & vbTab & _
                                .HardenerRatio.Value.ToString(mLanguage.CurrentCulture) & vbTab & _
                                .ResinSolventValve.Value.ToString(mLanguage.CurrentCulture) & vbTab & _
                                .HardenerSolventValve.Value.ToString(mLanguage.CurrentCulture)
                    End With
                    sText(i + 1) = sText(i + 1).Replace("<", "")
                    sText(i + 1) = sText(i + 1).Replace(">", "")
                Next
                mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle, sPageTitle)
            End If ' mRobot.SystemColors.Use2K 

            '******************* Finish up
            mPrintHtml.subCloseFile(Status)
            If bPrint Then
                mPrintHtml.subPrintDoc()
            End If
            Return (True)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)

            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Status = gcsRM.GetString("csPRINTFAILED")
            Return (False)

        End Try

    End Function

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
        ' 10/04/10  MSW     Fix ratio printout
        ' 04/14/11  MSW     CSV export
        ' 4/21/11   HGB     Added PaintWorks SA print Function
        '********************************************************************************************
        Try
            If colZones.StandAlone Then
                Return (bPrintdocSA(bPrint, bExportCSV))
            End If
            Dim nBitOffset As Integer = colZones.ActiveZone.RobotsRequiredStartingBit ' 11/28/09  RJO

            mPrintHtml.subSetPageFormat()
            mPrintHtml.subClearTableFormat()
            mPrintHtml.subSetColumnCfg("align=right", 0)
            mPrintHtml.subSetRowcfg("Bold=on", 0, 0)

            mPrintHtml.HeaderRowsPerTable = 1

            Dim sPageTitle As String = gpsRM.GetString("psSCREENCAPTION")
            Dim sTitle(1) As String
            sTitle(0) = colZones.SiteName & " - " & colZones.CurrentZone
            sTitle(1) = String.Empty
            Dim sSubTitle(0) As String
            sSubTitle(0) = String.Empty

            '******************* Standard system color table
            'column headers
            Dim sText(0) As String
            sText(0) = lblFanucColorCap.Text & vbTab & _
                    lblPlantColorCap.Text & vbTab & lblColorDescCap.Text & vbTab & _
                    lblValveNumberCap.Text & vbTab & lblValveDescCap.Text

            For i As Integer = 0 To colArms.Count - 1
                sText(0) = sText(0) & vbTab & colArms.Item(i).Name
            Next

            ReDim Preserve sText(mRobot.SystemColors.EffectiveColors)
            For i As Integer = 0 To mRobot.SystemColors.EffectiveColors - 1
                With mRobot.SystemColors.Item(i)
                    sText(i + 1) = (i + 1).ToString(mLanguage.FixedCulture) & vbTab & .PlantNumber.Text & _
                            vbTab & .Description.Text & vbTab & _
                            .Valve.Number.Value.ToString(mLanguage.CurrentCulture) & vbTab & _
                            .Valve.Description.Text
                    sText(i + 1) = sText(i + 1).Replace("<", "")
                    sText(i + 1) = sText(i + 1).Replace(">", "")
                    Dim nValue As Integer = .RobotsRequired.Value
                    For j As Integer = 0 To colArms.Count - 1
                        Dim nBot As Integer = CType(2 ^ (colArms.Item(j).RobotNumber - 1 + nBitOffset), Integer)
                        If (nValue And nBot) = nBot Then
                            'sTmp = sTmp & vbTab & "X"
                            sText(i + 1) = sText(i + 1) & vbTab & "X"
                        Else
                            'sTmp = sTmp & vbTab & "."
                            sText(i + 1) = sText(i + 1) & vbTab & "."
                        End If
                    Next
                End With
            Next
            dim bCancel as boolean  = false
            mPrintHtml.subStartDoc(Status, sPageTitle, bExportCSV, bCancel)
            if bCancel then
              return(false)
            end if

            mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle, sPageTitle)

            '******************* Tricoat
            If mRobot.SystemColors.UseTricoat Then
                sTitle(0) = colZones.SiteName & " - " & colZones.CurrentZone
                sTitle(1) = String.Empty
                sSubTitle(0) = TabPage4.Text

                '******************* Standard system color table
                'column headers
                sText(0) = lblPlantColor4Cap.Text & vbTab & _
                        lblColorDesc4Cap.Text & vbTab & lblTricoat4Cap.Text

                For i As Integer = 0 To mRobot.SystemColors.EffectiveColors - 1
                    With mRobot.SystemColors.Item(i)
                        sText(i + 1) = .PlantNumber.Text & vbTab & _
                                .Description.Text & vbTab & .Tricoat.Text
                    End With
                    sText(i + 1) = sText(i + 1).Replace("<", "")
                    sText(i + 1) = sText(i + 1).Replace(">", "")
                Next
                mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle)
            End If ' mRobot.SystemColors.UseTricoat

            '******************* 2K
            If mRobot.SystemColors.Use2K Then
                sTitle(0) = colZones.SiteName & " - " & colZones.CurrentZone
                sTitle(1) = String.Empty
                sSubTitle(0) = TabPage3.Text

                'column headers
                'column headers
                sText(0) = lblFanucColorCap.Text & vbTab & _
                        lblPlantColor3Cap.Text & vbTab & _
                        lblColorDesc3Cap.Text & vbTab & lblHardenerCap.Text & vbTab & _
                        lblResinRatioCap.Text & vbTab & lblHardenerRatioCap.Text & vbTab & _
                        lblResinSolvCap.Text & vbTab & lblHardSolvCap.Text

                For i As Integer = 0 To mRobot.SystemColors.EffectiveColors - 1
                    With mRobot.SystemColors.Item(i)
                        sText(i + 1) = (i + 1).ToString(mLanguage.FixedCulture) & vbTab & _
                                .PlantNumber.Text & vbTab & _
                                .Description.Text & vbTab & _
                                .HardenerValve.Value.ToString(mLanguage.CurrentCulture) & vbTab & _
                                .ResinRatio.Value.ToString(mLanguage.CurrentCulture) & vbTab & _
                                .HardenerRatio.Value.ToString(mLanguage.CurrentCulture) & vbTab & _
                                .ResinSolventValve.Value.ToString(mLanguage.CurrentCulture) & vbTab & _
                                .HardenerSolventValve.Value.ToString(mLanguage.CurrentCulture)
                    End With
                    sText(i + 1) = sText(i + 1).Replace("<", "")
                    sText(i + 1) = sText(i + 1).Replace(">", "")
                Next
                mPrintHtml.subAddObject(sText, Status, sTitle, sSubTitle)
            End If ' mRobot.SystemColors.Use2K 

            '******************* Finish up
            mPrintHtml.subCloseFile(Status)
            If bPrint Then
                mPrintHtml.subPrintDoc()
            End If
            Return (True)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)

            ShowErrorMessagebox(gcsRM.GetString("csPRINTFAILED"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

            Status = gcsRM.GetString("csPRINTFAILED")
            Return (False)

        End Try

    End Function

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
    Private Sub subSaveMenuHandler(ByVal sender As Object, _
                                            ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  One of save sub menus clicked. mRobot should exist and have the working data
        '               because it should be loaded to be here
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim oColors As clsSysColors = mRobot.SystemColors

        'make sure its good data
        If Not bValidateData() Then Exit Sub

        Cursor = Cursors.WaitCursor

        Select Case o.Name
            Case gcsRM.GetString("csPLC")
                If bSaveToPLC(oColors) = False Then

                    MessageBox.Show(gcsRM.GetString("csSAVEFAILED"), o.Name, _
                                MessageBoxButtons.OK, MessageBoxIcon.Information, _
                                MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                End If
            Case gcsRM.GetString("csDATABASE")
                If bSaveToGUI(oColors) = False Then

                    MessageBox.Show(gcsRM.GetString("csSAVEFAILED"), o.Name, _
                                MessageBoxButtons.OK, MessageBoxIcon.Information, _
                                MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)

                End If
            Case Else
                ' its a controller name
                Dim oArm As clsArm = colControllers.Item(o.Name).Arms(0)

                If bSaveToRobot(oColors, oArm) = False Then

                    MessageBox.Show(gcsRM.GetString("csSAVEFAILED"), o.Name, _
                                MessageBoxButtons.OK, MessageBoxIcon.Information, _
                                MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)

                End If
        End Select

        Progress = 95
        'mPWCommon.SaveToChangeLog(colZones.ActiveZone.DatabasePath)
        mPWCommon.SaveToChangeLog(colZones.ActiveZone)

        Progress = 100
        Cursor = Cursors.Default

    End Sub
    Private Sub subLoadMenuHandler(ByVal sender As Object, _
                                        ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  One of Load sub menus clicked. mRobot should exist and have the working data
        '               because it should be loaded to be here
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim oColors As clsSysColors = mRobot.SystemColors

        Try
            Select Case o.Name
                Case gcsRM.GetString("csPLC")
                    If bLoadFromPLC(oColors) = False Then

                        MessageBox.Show(gcsRM.GetString("csLOADFAILED"), o.Name, _
                                MessageBoxButtons.OK, MessageBoxIcon.Information, _
                                MessageBoxDefaultButton.Button1, _
                                MessageBoxOptions.DefaultDesktopOnly)
                    End If
                Case gcsRM.GetString("csDATABASE")
                    If bLoadFromGUI(mRobot) = False Then

                        MessageBox.Show(gcsRM.GetString("csLOADFAILED"), o.Name, _
                                MessageBoxButtons.OK, MessageBoxIcon.Information, _
                                MessageBoxDefaultButton.Button1, _
                                MessageBoxOptions.DefaultDesktopOnly)

                    End If
                Case Else
                    'this will be a controller name
                    Dim oArm As clsArm = colControllers.Item(o.Name).Arms(0)

                    If bLoadFromRobot(oArm) = False Then

                        MessageBox.Show(gcsRM.GetString("csLOADFAILED"), o.Name, _
                                MessageBoxButtons.OK, MessageBoxIcon.Information, _
                                MessageBoxDefaultButton.Button1, _
                                MessageBoxOptions.DefaultDesktopOnly)
                    Else
                        mRobot = oArm
                    End If
            End Select
            Status = gcsRM.GetString("csREADY")
        Catch ex As Exception

        Finally
            subShowNewPage()
            EditsMade = True
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

            Progress = 5
            'make sure its good data
            If Not bValidateData() Then Exit Sub


            ' do save
            If bSaveToRobots(mRobot.SystemColors) Then
                If bSaveToPLC(mRobot.SystemColors) Then
                    'NRU 161213 Save to PLC Succeded, might as well do my change log here.
                    Dim intCtr As Integer
                    If Not strPigSystems Is Nothing Then
                        For intCtr = 0 To UBound(strPigSystems) - 1
                            If Strings.StrComp(strPigSystems(intCtr), strPrevPigSystems(intCtr), CompareMethod.Text) <> 0 Then
                                subUpdateChangeLog(strPigSystems(intCtr), strPrevPigSystems(intCtr), "Piggable System", "PLC", CStr(intCtr + 1))
                            End If
                        Next
                    End If

                    If bSaveToGUI(mRobot.SystemColors) Then
                        mRobot.SystemColors.Update()
                        'mPWCommon.SaveToChangeLog(colZones.ActiveZone.DatabasePath)
                        'For SQL database - remove above eventually
                        mPWCommon.SaveToChangeLog(colZones.ActiveZone)
                        ' save done
                        EditsMade = False
                        subShowNewPage()

                    Else
                        'save failed
                        Status = gcsRM.GetString("csSAVEFAILED")
                    End If  'SaveToRobot()
                Else
                    'save failed
                    Status = gcsRM.GetString("csSAVEFAILED")
                End If    ' bSaveToPLC()
            Else
                'save failed
                Status = gcsRM.GetString("csSAVEFAILED")
            End If      'bSaveToGUI()

            If mbPLCFail Then
                Status = gcsRM.GetString("csSAVEFAILED")
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)

        Finally

            Me.Cursor = System.Windows.Forms.Cursors.Default
            Progress = 100
            Status = gcsRM.GetString("csSAVE_DONE")

        End Try


    End Sub
    Private Sub subSetTextBoxProperties(ByRef rCol As Collection(Of FocusedTextBox.FocusedTextBox))
        '********************************************************************************************
        'Description:  this is based on the naming convention of 
        '                   textboxname + 3 digit number  e.g. txtMyBox01
        '
        'Parameters:
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/18/09  MSW     subTextboxKeypressHandler, subSetTextBoxProperties - Offer a login if they try to enter data without a login
        ' 06/30/10  MSW     handle page up, page down, home, end
        '********************************************************************************************
        Dim o As FocusedTextBox.FocusedTextBox

        For Each o In rCol

            AddHandler o.UpArrow, AddressOf subTextBoxUpArrowHandler
            AddHandler o.DownArrow, AddressOf subTextBoxDownArrowHandler
            AddHandler o.LeftArrow, AddressOf subTextBoxLeftArrowHandler
            AddHandler o.RightArrow, AddressOf subTextBoxRightArrowHandler
            AddHandler o.Validating, AddressOf subTextboxValidatingHandler
            AddHandler o.Validated, AddressOf subTextboxValidatedHandler
            AddHandler o.TextChanged, AddressOf subTextboxChangeHandler
            AddHandler o.KeyPress, AddressOf subTextboxKeypressHandler
            AddHandler o.KeyUp, AddressOf subTextboxKeyUpHandler ' 06/30/10  MSW     handle page up, page down, home, end
        Next

    End Sub
    Private Sub subSetComboBoxProperties(ByRef rCol As Collection(Of ComboBox))
        '********************************************************************************************
        'Description:  this is based on the naming convention of 
        '                   textboxname + 3 digit number  e.g. txtMyBox01
        '
        'Parameters:
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '02/20/13   NRU     Piggable Tab
        '********************************************************************************************
        'NRU 161213 Piggable
        Dim o As ComboBox

        For Each o In rCol
            AddHandler o.TextChanged, AddressOf subComboboxTextChangedHandler
            AddHandler o.SelectedIndexChanged, AddressOf subComboboxSelectedIndexChangedHandler
            AddHandler o.LostFocus, AddressOf subComboboxLostFocusHandler
        Next

    End Sub
    Private Sub subUnloadPanel(ByRef rPanel As Panel, _
                                         ByRef rCol As Collection(Of FocusedTextBox.FocusedTextBox))
        '********************************************************************************************
        'Description:  this is based on the naming convention of 
        '                   textboxname + 3 digit number  e.g. txtMyBox01
        '
        'Parameters:
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As Control
        Dim f As FocusedTextBox.FocusedTextBox
        Dim c As CheckedListBox
        Dim l As Label

        Dim cc As Control.ControlCollection = rPanel.Controls

        For i As Integer = cc.Count - 1 To 0 Step -1
            o = cc(i)

            If TypeOf (o) Is FocusedTextBox.FocusedTextBox Then
                If rCol Is Nothing = False Then
                    If rCol.Count <> 0 Then
                        f = DirectCast(o, FocusedTextBox.FocusedTextBox)
                        RemoveHandler f.UpArrow, AddressOf subTextBoxUpArrowHandler
                        RemoveHandler f.DownArrow, AddressOf subTextBoxDownArrowHandler
                        RemoveHandler f.LeftArrow, AddressOf subTextBoxLeftArrowHandler
                        RemoveHandler f.RightArrow, AddressOf subTextBoxRightArrowHandler
                        RemoveHandler f.Validating, AddressOf subTextboxValidatingHandler
                        RemoveHandler f.Validated, AddressOf subTextboxValidatedHandler
                        RemoveHandler f.TextChanged, AddressOf subTextboxChangeHandler

                        If Strings.Right(o.Name, 3) <> "001" Then
                            cc.Remove(o)
                        Else
                            f.Text = String.Empty
                        End If
                    End If
                End If
            Else

                If TypeOf o Is CheckedListBox Then
                    c = DirectCast(o, CheckedListBox)
                    RemoveHandler c.Validated, AddressOf subCheckListBoxValidatedHandler
                    RemoveHandler c.ItemCheck, AddressOf subCheckListBoxItemCheckHandler
                    RemoveHandler c.Leave, AddressOf subRobotsReqLeaveHandler
                    c.Items.Clear()
                End If

                If TypeOf o Is Label Then
                    l = DirectCast(o, Label)
                    If Strings.Right(l.Name, 8) = "lblColor" Then
                        RemoveHandler l.MouseClick, AddressOf subLabelClickHandler
                    End If
                End If

                If (Strings.Right(o.Name, 3) <> "001") And (IsNumeric(Strings.Right(o.Name, 3))) Then
                    cc.Remove(o)
                Else

                End If

            End If


        Next


        rCol = New Collection(Of FocusedTextBox.FocusedTextBox)


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
        ' 04/16/13  MSW     Standalone ChangeLog                                            4.1.5.0
        ' 02/03/11  RJO     Changed sScreenName argument to mPWCommon.subShowChangeLog from 
        '                   msSCREEN_NAME to gsChangeLogArea for consistency between this screen
        '                   and Change Log Reports screen.
        '********************************************************************************************

        Try
            mPWCommon.subShowChangeLog(colZones.CurrentZone, gsChangeLogArea, gsChangeLogArea, gcsRM.GetString("csALL"), _
                                          gcsRM.GetString("csALL"), mDeclares.eParamType.None, nIndex, False, False, oIPC)  'JZ 12072016
        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
        End Try

    End Sub
    Private Sub subShowHideTabs(ByVal Is2K As Boolean, ByVal IsTricoat As Boolean, ByVal IsPiggable As Boolean) 'NRU 161213 Piggable
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '04/05/11   HGB     Added StandAlone flag to hide Robots Required tab for SA
        '********************************************************************************************

        tabMain.SuspendLayout()

        '       TabPage 1 = sys colors
        '       TabPage 2 = Robots required
        '       TabPage 3 = 2k
        '       TabPage 4 = Tricoat color

        Dim bTmp(mnMAX_TABS) As Boolean
        Dim sKey As String

        For i As Integer = 0 To mnMAX_TABS
            bTmp(i) = True
        Next

        bTmp(3) = Is2K
        bTmp(4) = IsTricoat
        bTmp(5) = IsPiggable 'NRU 161213 Piggable

        If colZones.ActiveZone.IsRemoteZone Or colZones.StandAlone Then

            bTmp(2) = False

        End If

        For i As Integer = 1 To mnMAX_TABS

            sKey = "TabPage" & i.ToString

            If bTmp(i) Then
                ' want it visible
                If tabMain.TabPages.ContainsKey(sKey) = False Then
                    'better be in dummy!
                    Dim x As TabPage = tbcDummy.TabPages(sKey)
                    tbcDummy.TabPages.Remove(x)
                    tabMain.TabPages.Add(x)
                End If
            Else
                ' want it hid
                If tabMain.TabPages.ContainsKey(sKey) Then
                    Dim x As TabPage = tabMain.TabPages(sKey)
                    tabMain.TabPages.Remove(x)
                    tbcDummy.TabPages.Add(x)
                End If
            End If

            'clear out old collections
            colTabTextboxes(i) = New Collection(Of FocusedTextBox.FocusedTextBox)

        Next


        tabMain.ResumeLayout()

    End Sub
    Private Sub subShowNewPage(Optional ByVal bRefreshPig As Boolean = True) 'NRU 161213 Piggable
        '********************************************************************************************
        'Description:  Data is all loaded or changed and screen needs to be refreshed
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/03/11  JBW     Two coat checkboxes needed edit priviledges taken care of
        '********************************************************************************************
        mbEventBlocker = True
        'JBW added for two coat checkboxes 03/03/11
        mScreenSetup.LoadCheckBoxCollection(Me, "pnltab1", mcolCheckBoxes)
        For i As Integer = 0 To mRobot.SystemColors.EffectiveColors - 1
            Dim oCol As SystemColors.clsSysColor = mRobot.SystemColors.Item(i)
            subLoadTab1Data(oCol)
            Progress = Progress + 5
            subLoadTab2Data(oCol)
            Progress = Progress + 5
            subLoadTab3Data(oCol)
            Progress = Progress + 5
            subLoadTab4Data(oCol)
        Next

        'NRU 161213 Piggable Tab not based on the number of system colors, called only once.
        Progress = Progress + 5
        If bRefreshPig Then subLoadTab5Data()

        mbEventBlocker = False

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
        ' 04/05/10  MSW     ascii edit handling
        ' 05/13/10  BTK     Moved the check for the spacebar to only check when we are editing plant color or tricoat color.
        ' 01/11/11  MSW     allow decimal points in ratios
        '********************************************************************************************
        Dim oFTB As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sName As String = Strings.Left(oFTB.Name, (Len(oFTB.Name) - 3))
        If Char.IsNumber(e.KeyChar) OrElse Char.IsLetter(e.KeyChar) OrElse Char.IsWhiteSpace(e.KeyChar) Then
            If oFTB.ReadOnly Then
                If Privilege = ePrivilege.None Then
                    Privilege = ePrivilege.Edit
                End If
            Else
                'if using ascii plant numbers cap the length
                If ((sName = "ftbPlantColor") Or (sName = "ftbTricoatColor")) And mRobot.SystemColors.IsAscii Then
                    If (oFTB.Text.Length - oFTB.SelectionLength) >= mRobot.SystemColors.PlantAsciiMaxLength OrElse Char.IsWhiteSpace(e.KeyChar) Then
                        e.Handled = True
                    End If
                End If
            End If
        Else
            If Char.IsPunctuation(e.KeyChar) OrElse Char.IsSymbol(e.KeyChar) Then
                If (sName = "ftbHardRatio") Or (sName = "ftbResinRatio") Then
                    If (e.KeyChar <> ".") Then
                        e.Handled = True
                    End If
                Else
                    e.Handled = True
                End If
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
        ' 06/30/10  MSW     handle page up, page down, home, end
        '********************************************************************************************
        Dim oFTB As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sNumber As String = Strings.Right(oFTB.Name, 3)
        Dim sName As String = Strings.Left(oFTB.Name, (Len(oFTB.Name) - 3))
        Dim sNewName As String = String.Empty
        Dim nTmp As Integer = 0
        Dim o As FocusedTextBox.FocusedTextBox
        Dim oContainer As Control = Nothing
        Dim nMax As Integer =  mRobot.SystemColors.EffectiveColors
        Select Case tabMain.SelectedTab.Name
            Case "TabPage1"
                oContainer = pnlTab1
            Case "TabPage2"
                oContainer = pnlTab2
            Case "TabPage3"
                oContainer = pnlTab3
            Case "TabPage4"
                oContainer = pnlTab4
        End Select
        nTmp = CType(sNumber, Integer)
        Select Case e.KeyCode
            Case Keys.Home
                'validate the current box before shifting it around
                cboZone.Focus()
                Application.DoEvents()
                Select Case tabMain.SelectedTab.Name
                    Case "TabPage1"
                        sNewName = "ftbPlantColor" & Strings.Format(nTmp, "000")
                    Case "TabPage2"
                        sNewName = sName & Strings.Format(nTmp, "000")
                    Case "TabPage3"
                        sNewName = "ftbHardValve" & Strings.Format(nTmp, "000")
                    Case "TabPage4"
                        sNewName = "ftbTricoatColor" & Strings.Format(nTmp, "000")
                End Select
            Case Keys.End
                'validate the current box before shifting it around
                cboZone.Focus()
                Application.DoEvents()
                Select Case tabMain.SelectedTab.Name
                    Case "TabPage1"
                        sNewName = "ftbValveDesc" & Strings.Format(nTmp, "000")
                    Case "TabPage2"
                        sNewName = sName & Strings.Format(nTmp, "000")
                    Case "TabPage3"
                        sNewName = "ftbHardRatio" & Strings.Format(nTmp, "000")
                    Case "TabPage4"
                        sNewName = "ftbTricoatColor" & Strings.Format(nTmp, "000")
                End Select
            Case Keys.PageUp
                'validate the current box before shifting it around
                cboZone.Focus()
                Application.DoEvents()
                nTmp = nTmp - 10
                If nTmp < 1 Then
                    If nTmp <= -9 Then
                        nTmp = nMax
                    Else
                        nTmp = 1
                    End If
                End If

                sNewName = sName & Strings.Format(nTmp, "000")

            Case Keys.PageDown
                'validate the current box before shifting it around
                cboZone.Focus()
                Application.DoEvents()
                nTmp = nTmp + 10
                If nTmp > nMax Then
                    If nTmp >= nMax + 10 Then
                        nTmp = 1
                    Else
                        nTmp = nMax
                    End If
                End If

                sNewName = sName & Strings.Format(nTmp, "000")

            Case Else
        End Select
        o = DirectCast(oContainer.Controls(sNewName), FocusedTextBox.FocusedTextBox)
        If IsNothing(o) Then Exit Sub
        o.Focus()
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
        '********************************************************************************************
        Dim sNumber As String = Strings.Right(sender.Name, 3)
        Dim sName As String = Strings.Left(sender.Name, (Len(sender.Name) - 3))
        Dim sNewName As String = String.Empty
        Dim nTmp As Integer = 0
        Dim o As FocusedTextBox.FocusedTextBox
        Dim oContainer As Control = Nothing

        nTmp = CType(sNumber, Integer) - 1
        If nTmp < 1 Then
            'first one - back to bottom
            nTmp = mRobot.SystemColors.EffectiveColors
        End If ' nTmp = 

        sNewName = sName & Strings.Format(nTmp, "000")

        Select Case tabMain.SelectedTab.Name
            Case "TabPage1"
                oContainer = pnlTab1
            Case "TabPage2"
                oContainer = pnlTab2
            Case "TabPage3"
                oContainer = pnlTab3
            Case "TabPage4"
                oContainer = pnlTab4

        End Select

        o = DirectCast(oContainer.Controls(sNewName), FocusedTextBox.FocusedTextBox)
        If IsNothing(o) Then Exit Sub

        o.Focus()


    End Sub
    Private Sub subCheckListBoxValidatedHandler(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validated Routine
        '
        'Parameters: event parameters
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/28/09  RJO     Add RobotsRequiredStatingBit offset Code
        '********************************************************************************************
        Dim oC As CheckedListBox = DirectCast(sender, CheckedListBox)
        Dim nBoxInx As Integer = CType(Strings.Right(oC.Name, 3), Integer)
        Dim nValue As Integer = 0
        Dim nBitOffset As Integer = colZones.ActiveZone.RobotsRequiredStartingBit ' 11/28/09  RJO

        If DataLoaded = False Then Exit Sub

        Try
            'lets hope there are no more than 15 robots in a zone....
            For i As Integer = 0 To oC.Items.Count - 1
                If i > 15 Then Exit Sub
                If oC.GetItemChecked(i) Then
                    nValue = nValue Or gnBitVal(i + nBitOffset) ' 11/28/09  RJO
                End If
            Next

            mRobot.SystemColors.Item(nBoxInx - 1).RobotsRequired.Value = nValue

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Private Sub subRobotsReqLeaveHandler(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  generic leave Routine
        '
        'Parameters: event parameters
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oC As CheckedListBox = DirectCast(sender, CheckedListBox)
        oC.SelectedItems.Clear()

    End Sub
    Private Sub subCheckListBoxItemCheckHandler(ByVal sender As Object, _
                                            ByVal e As System.Windows.Forms.ItemCheckEventArgs)
        '********************************************************************************************
        'Description:  generic ItemCheck Routine
        '
        'Parameters: event parameters
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/28/09  RJO     Add RobotsRequiredStatingBit offset Code
        '********************************************************************************************
        Dim oC As CheckedListBox = DirectCast(sender, CheckedListBox)
        Dim nBoxInx As Integer = CType(Strings.Right(oC.Name, 3), Integer)
        Dim nValue As Integer = 0
        Dim nBitOffset As Integer = colZones.ActiveZone.RobotsRequiredStartingBit ' 11/28/09  RJO

        If DataLoaded = False Then Exit Sub
        If mbEventBlocker Then Exit Sub

        Try

            'lets hope there are no more than 15 robots in a zone....
            For i As Integer = 0 To oC.Items.Count - 1
                If i > 15 Then Exit Sub
                If i = e.Index Then
                    If e.NewValue = CheckState.Checked Then
                        nValue = nValue Or gnBitVal(i + nBitOffset) ' 11/28/09  RJO
                    End If
                Else
                    If oC.GetItemChecked(i) Then
                        nValue = nValue Or gnBitVal(i + nBitOffset) ' 11/28/09  RJO
                    End If
                End If
            Next
            For nArm As Integer = 0 To colArms.Count - 1
                If nArm = e.Index Then
                    If e.NewValue = CheckState.Checked Then
                        nValue = nValue Or gnBitVal(colArms(nArm).RobotNumber + nBitOffset - 1) ' 11/28/09  RJO
                    End If
                Else
                    If oC.GetItemChecked(nArm) Then
                        nValue = nValue Or gnBitVal(colArms(nArm).RobotNumber + nBitOffset - 1) ' 11/28/09  RJO
                    End If
                End If
            Next
            If mRobot.SystemColors.Item(nBoxInx - 1).RobotsRequired.OldValue <> nValue Then
                oC.ForeColor = Color.Red
                EditsMade = True
            Else
                oC.ForeColor = Color.Black
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

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
        '********************************************************************************************
        Dim oT As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sName As String = Strings.Left(oT.Name, (Len(oT.Name) - 3))
        Dim sText As String = oT.Text
        Dim nBoxInx As Integer = CType(Strings.Right(oT.Name, 3), Integer)


        If DataLoaded = False Then Exit Sub

        Try
            With mRobot.SystemColors.Item(nBoxInx - 1)
                Select Case sName
                    Case "ftbPlantColor"
                        .PlantNumber.Text = sText
                    Case "ftbColorDesc"
                        .Description.Text = sText
                    Case "ftbValveNumber"
                        .Valve.Number.Value = CType(sText, Integer)
                        If .Valve.Number.Changed Then
                            'update the desc - array pos 0 is place holder
                            mRobot.SystemColors.RefreshValveDescriptions()
                            'NRU 161213 Added optional param to refrain from reading Piggable
                            'stuff in whenever a valve is changed.
                            subShowNewPage(False)
                        End If
                    Case "ftbValveDesc"
                        .Valve.Description.Text = sText
                        'update array - array pos 0 is place holder
                        mRobot.SystemColors.msColorValveName(.Valve.Number.Value) = sText
                        mRobot.SystemColors.RefreshValveDescriptions()
                        subShowNewPage()
                    Case "ftbTricoatColor"
                        .Tricoat.Text = sText
                    Case "ftbHardValve"
                        .HardenerValve.Value = CType(sText, Integer)
                    Case "ftbResinRatio"
                        .ResinRatio.Value = CType(sText, Single)
                    Case "ftbHardRatio"
                        .HardenerRatio.Value = CType(sText, Single)
                    Case "ftbResinSolv"
                        .ResinSolventValve.Value = CType(sText, Integer)
                    Case "ftbHardSolv"
                        .HardenerSolventValve.Value = CType(sText, Integer)

                End Select
            End With
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Private Sub subComboboxTextChangedHandler(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  generic ComboBox change Routine
        '
        'Parameters: event parameters
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '02/20/13   NRU     Piggable Tab
        '********************************************************************************************
        'NRU 161213 Piggable
        Dim intPos As Integer
        Dim intTypedChars As Integer
        If mbEventBlocker Then Exit Sub

        'Get the combo box
        Dim cb As ComboBox = DirectCast(sender, ComboBox)

        'Autofill as the user types with the first found item containing their typed chars
        'select non typed characters to be overwritten by further typing. (Proper cbobox)
        intPos = cb.FindString(cb.Text)
        If intPos >= 0 And cb.SelectedIndex <> intPos Then
            intTypedChars = cb.Text.Length
            cb.SelectedIndex = intPos
            cb.SelectionStart = intTypedChars
            cb.SelectionLength = cb.Text.Length - intTypedChars
        End If

        If DataLoaded And EditsMade = False Then EditsMade = True
    End Sub
    Private Sub subComboboxSelectedIndexChangedHandler(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  generic ComboBox change Routine
        '
        'Parameters: event parameters
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '02/20/13   NRU     Piggable Tab
        '********************************************************************************************
        'NRU 161213 Piggable
        Dim intPos As Integer
        If mbEventBlocker Then Exit Sub

        'Get the combo box and which system (pos)
        Dim cb As ComboBox = DirectCast(sender, ComboBox)
        intPos = CInt(Strings.Right(cb.Name, 3)) - 1

        'If the blank item is selected send a 0, else send the appropriate valve
        If cb.SelectedIndex < 1 Then
            strPigSystems(intPos) = "0"
        Else
            strPigSystems(intPos) = CStr(cb.SelectedIndex + mnMAX_MAINLINE_VALVES)
        End If

        If DataLoaded And EditsMade = False Then EditsMade = True

    End Sub
    Private Sub subComboboxLostFocusHandler(ByVal sender As Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  generic ComboBox change Routine
        '
        'Parameters: event parameters
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '02/20/13   NRU     Piggable Tab
        '********************************************************************************************
        'NRU 161213 Piggable
        Dim intPos As Integer
        If mbEventBlocker Then Exit Sub

        'Get the combo box
        Dim cb As ComboBox = DirectCast(sender, ComboBox)

        'If the user leaves the cbo with invalid crap in it, clear it
        '(limit to list)
        intPos = cb.FindStringExact(cb.Text)
        If intPos = -1 Then
            If cb.Items.Count > 0 Then
                cb.SelectedIndex = 0
            Else
                cb.SelectedIndex = -1
            End If
        End If

        If DataLoaded And EditsMade = False Then EditsMade = True

    End Sub
    Private Sub subTextboxValidatingHandler(ByVal sender As Object, _
                                ByVal e As System.ComponentModel.CancelEventArgs)
        '********************************************************************************************
        'Description:  generic textbox Validating Routine
        '
        'Parameters: event parameters
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/05/10  MSW     ascii edit handling
        ' 07/01/10  MSW     make sure it still does numeric checks on valves
        '********************************************************************************************
        Dim oT As FocusedTextBox.FocusedTextBox = DirectCast(sender, FocusedTextBox.FocusedTextBox)
        Dim sName As String = Strings.Left(oT.Name, (Len(oT.Name) - 3))
        Dim sText As String = oT.Text
        Dim bBadData As Boolean = False
        Dim nMin As Integer = 1
        Dim nMax As Integer = 100
        Dim bAscii As Boolean = False

        ''if no edit priv and bad data this locks us up
        If Privilege = ePrivilege.None Then
            e.Cancel = True
            Exit Sub
        End If

        ''no value?
        If Strings.Len(sText) = 0 Then
            bBadData = True
            MessageBox.Show(gcsRM.GetString("csNO_NULL"), gcsRM.GetString("csINVALID_DATA"), _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning, _
                            MessageBoxDefaultButton.Button1, _
                            MessageBoxOptions.DefaultDesktopOnly)
            oT.Undo()
            e.Cancel = True
            Exit Sub
        End If


        ''description boxes
        Select Case sName
            Case "ftbColorDesc", "ftbValveDesc"
                'only check for blank
                If bBadData Then
                    oT.Undo()
                    e.Cancel = True
                End If
                Exit Sub
            Case Else
        End Select

        'if using ascii plant numbers only check for blank
        If mRobot.SystemColors.IsAscii Then
            Select Case sName
                Case "ftbPlantColor"
                    If Strings.Len(sText) > mRobot.SystemColors.PlantAsciiMaxLength Then
                        bBadData = True
                    End If
                    If bBadData Then
                        oT.Undo()
                        e.Cancel = True
                    End If
                    Exit Sub
                Case "ftbTricoatColor"
                    If IsNumeric(sText.Trim) Then
                        If CInt(sText.Trim) = 0 Then
                            bBadData = False
                        Else
                            bBadData = True
                        End If
                    Else
                        bBadData = True
                    End If
                    For Each oColor As clsSysColor In mRobot.SystemColors
                        If oColor.PlantNumber.Text = sText Then
                            bBadData = False
                            Exit For
                        End If
                    Next
                    If bBadData Then
                        MessageBox.Show(gpsRM.GetString("psCOLOR_DOESNT_EXISTS"), _
                            gcsRM.GetString("csINVALID_DATA"), _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning, _
                            MessageBoxDefaultButton.Button1, _
                            MessageBoxOptions.DefaultDesktopOnly)
                    End If
                    If Strings.Len(sText) > mRobot.SystemColors.PlantAsciiMaxLength Then
                        bBadData = True
                    End If
                    If bBadData Then
                        oT.Undo()
                        e.Cancel = True
                    End If
                    Exit Sub
            End Select
        End If

        Select Case sName
            Case "ftbPlantColor"
                nMin = mRobot.SystemColors.MinimumPlantColorNumber
                nMax = mRobot.SystemColors.MaximumPlantColorNumber
                bAscii = mRobot.SystemColors.IsAscii
            Case "ftbTricoatColor"
                nMin = mRobot.SystemColors.MinimumPlantColorNumber
                nMax = mRobot.SystemColors.MaximumPlantColorNumber
                bAscii = mRobot.SystemColors.IsAscii
                bBadData = True
                For Each oColor As clsSysColor In mRobot.SystemColors
                    If oColor.PlantNumber.Text = sText.Trim Then
                        bBadData = False
                        Exit For
                    End If
                Next
                If bBadData Then
                    MessageBox.Show(gpsRM.GetString("psCOLOR_DOESNT_EXISTS"), _
                        gcsRM.GetString("csINVALID_DATA"), _
                        MessageBoxButtons.OK, MessageBoxIcon.Warning, _
                        MessageBoxDefaultButton.Button1, _
                        MessageBoxOptions.DefaultDesktopOnly)
                End If
            Case "ftbValveNumber"
                nMin = mRobot.SystemColors.MinimumValveNumber
                nMax = mRobot.SystemColors.MaximumValveNumber
                bAscii = False
            Case "ftbHardValve"
                nMin = 0
                nMax = 3
                bAscii = False
            Case "ftbResinRatio"
                nMin = 0
                nMax = 100
                bAscii = False
            Case "ftbHardRatio"
                nMin = 0
                nMax = 100
                bAscii = False
            Case "ftbResinSolv"
                nMin = 0
                nMax = 2
                bAscii = False
            Case "ftbHardSolv"
                nMin = 0
                nMax = 2
                bAscii = False
            Case Else
                Exit Sub
        End Select


        If (bAscii = False) Then
            'numeric
            If bBadData = False Then
                If (Not (IsNumeric(sText))) Then
                    bBadData = True
                    MessageBox.Show(gcsRM.GetString("csBAD_ENTRY"), gcsRM.GetString("csINVALID_DATA"), _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning, _
                                   MessageBoxDefaultButton.Button1, _
                                   MessageBoxOptions.DefaultDesktopOnly)
                End If

                ' limit check
                If bBadData = False Then

                    'low limit
                    If CType(sText, Integer) < nMin Then
                        bBadData = True
                        MessageBox.Show(gcsRM.GetString("csVAL_BELOW_MIN") & vbCrLf & _
                                       gcsRM.GetString("csMINIMUM_EQ") & nMin, _
                                       gcsRM.GetString("csINVALID_DATA"), _
                                       MessageBoxButtons.OK, MessageBoxIcon.Warning, _
                                       MessageBoxDefaultButton.Button1, _
                                       MessageBoxOptions.DefaultDesktopOnly)
                    End If

                    'hi limit
                    If CSng(sText) > nMax Then
                        bBadData = True
                        MessageBox.Show(gcsRM.GetString("csVAL_ABOVE_MAX") & vbCrLf & _
                                       gcsRM.GetString("csMAXIMUM_EQ") & nMax, _
                                       gcsRM.GetString("csINVALID_DATA"), _
                                       MessageBoxButtons.OK, MessageBoxIcon.Warning, _
                                       MessageBoxDefaultButton.Button1, _
                                       MessageBoxOptions.DefaultDesktopOnly)
                    End If

                End If '  bBadData = False 
            End If ' bBadData = False
        End If
        'check for duplicates
        Select Case sName
            Case "ftbPlantColor"
                Dim inx As Integer = CType(Strings.Right(oT.Name, 3), Integer)
                For i As Integer = 1 To mRobot.SystemColors.EffectiveColors
                    If i <> inx Then
                        If sText = mRobot.SystemColors.Item(i - 1).PlantNumber.Text Then
                            bBadData = True
                            MessageBox.Show(gpsRM.GetString("psCOLOR_ALREADY_EXISTS"), _
                                           gcsRM.GetString("csINVALID_DATA"), _
                                           MessageBoxButtons.OK, MessageBoxIcon.Warning, _
                                           MessageBoxDefaultButton.Button1, _
                                           MessageBoxOptions.DefaultDesktopOnly)
                        End If
                    End If
                Next
        End Select
        If bBadData Then
            oT.Undo()
            e.Cancel = True
        End If


    End Sub
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
        '********************************************************************************************
        Dim sNumber As String = Strings.Right(sender.Name, 3)
        Dim sName As String = Strings.Left(sender.Name, (Len(sender.Name) - 3))
        Dim sNewName As String = String.Empty
        Dim nTmp As Integer = 0
        Dim o As FocusedTextBox.FocusedTextBox
        Dim oContainer As Control = Nothing

        nTmp = CType(sNumber, Integer) + 1
        If nTmp > mRobot.SystemColors.EffectiveColors Then
            'last one - back to top
            nTmp = 1
        End If ' nTmp = 11

        sNewName = sName & Strings.Format(nTmp, "000")

        Select Case tabMain.SelectedTab.Name
            Case "TabPage1"
                oContainer = pnlTab1
            Case "TabPage2"
                oContainer = pnlTab2
            Case "TabPage3"
                oContainer = pnlTab3
            Case "TabPage4"
                oContainer = pnlTab4

        End Select

        o = DirectCast(oContainer.Controls(sNewName), FocusedTextBox.FocusedTextBox)
        If IsNothing(o) Then Exit Sub

        o.Focus()


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
        Dim sNumber As String = Strings.Right(sender.Name, 3)
        Dim sName As String = Strings.Left(sender.Name, (Len(sender.Name) - 3))
        Dim sNewName As String = String.Empty
        Dim o As FocusedTextBox.FocusedTextBox
        Dim oContainer As Control = Nothing

        Select Case sName
            ' set name of control to right
            Case "ftbPlantColor"
                sNewName = "ftbColorDesc" & sNumber
            Case "ftbColorDesc"
                sNewName = "ftbValveNumber" & sNumber
            Case "ftbValveNumber"
                sNewName = "ftbValveDesc" & sNumber
            Case "ftbValveDesc"
                sNewName = "ftbPlantColor" & sNumber

            Case "ftbHardRatio"
                sNewName = "ftbHardValve" & sNumber
            Case "ftbHardValve"
                sNewName = "ftbResinRatio" & sNumber
            Case "ftbResinRatio"
                sNewName = "ftbHardRatio" & sNumber
            Case "ftbResinSolv"
                sNewName = "ftbResinSolv" & sNumber
            Case "ftbHardSolv"
                sNewName = "ftbHardSolv" & sNumber
            Case "ftbTricoatColor"
                sNewName = "ftbTricoatColor" & sNumber

        End Select

        Select Case tabMain.SelectedTab.Name
            Case "TabPage1"
                oContainer = pnlTab1
            Case "TabPage2"
                oContainer = pnlTab2
            Case "TabPage3"
                oContainer = pnlTab3
            Case "TabPage4"
                oContainer = pnlTab4

        End Select

        o = DirectCast(oContainer.Controls(sNewName), FocusedTextBox.FocusedTextBox)
        If IsNothing(o) Then Exit Sub

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
        Dim sNumber As String = Strings.Right(sender.Name, 3)
        Dim sName As String = Strings.Left(sender.Name, (Len(sender.Name) - 3))
        Dim sNewName As String = String.Empty
        Dim o As FocusedTextBox.FocusedTextBox
        Dim oContainer As Control = Nothing

        Select Case sName
            ' set name of control to right
            Case "ftbPlantColor"
                sNewName = "ftbValveDesc" & sNumber
            Case "ftbColorDesc"
                sNewName = "ftbPlantColor" & sNumber
            Case "ftbValveNumber"
                sNewName = "ftbColorDesc" & sNumber
            Case "ftbValveDesc"
                sNewName = "ftbValveNumber" & sNumber

            Case "ftbHardValve"
                sNewName = "ftbHardSolv" & sNumber
            Case "ftbResinRatio"
                sNewName = "ftbHardValve" & sNumber
            Case "ftbHardRatio"
                sNewName = "ftbResinRatio" & sNumber
            Case "ftbResinSolv"
                sNewName = "ftbHardRatio" & sNumber
            Case "ftbHardSolv"
                sNewName = "ftbResinSolv" & sNumber

            Case "ftbTricoatColor"
                sNewName = "ftbTricoatColor" & sNumber

        End Select

        Select Case tabMain.SelectedTab.Name
            Case "TabPage1"
                oContainer = pnlTab1
            Case "TabPage2"
                oContainer = pnlTab2
            Case "TabPage3"
                oContainer = pnlTab3
            Case "TabPage4"
                oContainer = pnlTab4

        End Select

        o = DirectCast(oContainer.Controls(sNewName), FocusedTextBox.FocusedTextBox)
        If IsNothing(o) Then Exit Sub

        o.Focus()


    End Sub
    Private Sub subUpdateChangeLog(ByRef NewValue As String, ByRef OldValue As String, _
                                                ByRef ParamName As String, ByRef Device As String, _
                                                ByRef Color As String)
        '********************************************************************************************
        'Description:  build up the change text
        '
        'Parameters: 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Dim sTmp As String = ParamName & " " & gcsRM.GetString("csCHANGED_FROM") & " " & OldValue & _
                            " " & gcsRM.GetString("csTO") & " " & NewValue

        AddChangeRecordToCollection(gsChangeLogArea, LoggedOnUser, _
                    colZones.CurrentZoneNumber, Device, Color, sTmp, colZones.CurrentZone)

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
                                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, _
                                        MessageBoxOptions.DefaultDesktopOnly) _
                                                                    = Response.OK Then

            subLoadData()
            subShowNewPage()
            Progress = 100
        End If

    End Sub

#End Region
#Region " Events "

    Private Sub cbTwoCoat_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description:  Handler for checkbox change event for two coat being checked/unchecked
        '
        'Parameters: checkbox
        'Returns:
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/22/11  JBW     Created for 2 coats by color for DHAM
        '********************************************************************************************
        If mbEventBlocker Then Exit Sub 'RJO 05/12/11

        Dim tempcheckbox As CheckBox = DirectCast(sender, CheckBox)
        'jbw
        'parse sender.name and get last 3 characters, convert to integer
        Dim tempnum As Integer = CType(tempcheckbox.Name.Substring(10, 3), Integer)
        Dim ocolor As clsSysColor = mRobot.SystemColors.Item(tempnum - 1)
        ocolor.TwoCoats.Value = tempcheckbox.Checked
        EditsMade = True
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
                    Select Case tabMain.SelectedTab.Name
                        Case "TabPage1"
                            subLaunchHelp(gs_HELP_SYSCOLORS_VALVES, oIPC)
                        Case "TabPage2"
                            subLaunchHelp(gs_HELP_SYSCOLORS_ROBREQ, oIPC)
                        Case "TabPage3"
                            subLaunchHelp(gs_HELP_SYSCOLORS_2K, oIPC)
                        Case "TabPage4"
                            subLaunchHelp(gs_HELP_SYSCOLORS_TRICOAT, oIPC)
                    End Select
                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty
                    Dim sName As String = msSCREEN_DUMP_NAME
                    Select Case tabMain.SelectedTab.Name
                        Case "TabPage1"
                            sName = sName & "_Valve"
                        Case "TabPage2"
                            sName = sName & "_RobotsRequired"
                        Case "TabPage3"
                            sName = sName & "_2K"
                        Case "TabPage4"
                            sName = sName & "_Tricoat"
                    End Select
                    sName = sName & ".jpg"
                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath & sName, Imaging.ImageFormat.Jpeg)

                Case Keys.Escape
                    Me.Close()

                Case Else

            End Select
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

        subInitializeForm()

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

            If (colArms Is Nothing = False) Then
                If (colArms.Count > 0) Then  '3/19/08
                    For i As Integer = colArms.Count - 1 To 0
                        colArms.Remove(colArms(i))
                    Next
                End If
            End If
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
    Private Sub frmMain_Layout(Optional ByVal sender As Object = Nothing, _
                    Optional ByVal e As System.Windows.Forms.LayoutEventArgs = Nothing) Handles MyBase.Layout
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

        'todo find out what this -70 is from.
        Dim nHeight As Integer = tscMain.ContentPanel.Height - 70
        If nHeight < 100 Then nHeight = 100
        Dim nWidth As Integer = tscMain.ContentPanel.Width - (pnlMain.Left * 2)
        If nWidth < 100 Then nWidth = 100
        pnlMain.Height = nHeight
        pnlMain.Width = nWidth
        nWidth = nWidth - 2 * tabMain.Left
        nHeight = nHeight - 2 * tabMain.Top
        tabMain.Width = nWidth
        tabMain.Height = nHeight
        Select Case tabMain.SelectedTab.Name
            Case "TabPage1"
                pnlTab1.Width = TabPage1.Width - (pnlTab1.Left + 10)
                pnlTab1.Height = TabPage1.Height - pnlTab1.Top - 5
            Case "TabPage2"
                pnlTab2.Width = TabPage2.Width - (pnlTab2.Left + 10)
                pnlTab2.Height = TabPage2.Height - pnlTab2.Top - 5
            Case "TabPage3"
                pnlTab3.Width = TabPage3.Width - (pnlTab3.Left + 10)
                pnlTab3.Height = TabPage3.Height - pnlTab3.Top - 5
        End Select

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
        If (Progress > 0) And (Progress < 95) Then Progress += 5

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
        ' 06/15/10  MSW     handle cross-thread calls
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
                'privilege check done in subroutine
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
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

            Case btnUtilities.Name
                btnUtilities.ShowDropDown()

        End Select
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
        '********************************************************************************************
        If DataLoaded Then
            If bPrintdoc(False) Then
                mPrintHtml.subShowPageSetup()
            End If
        End If
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
        ' 08/30/11  MSW     1st version - only standard 1k settings, no tricoat or 2k import yet.
        '********************************************************************************************
        Const nFANUC_COLOR As Integer = 1
        Const nPLANT_COLOR As Integer = 2
        Const nCOLOR_DESC As Integer = 3
        Const nVALVE_NUMBER As Integer = 4
        Const nVALVE_DESC As Integer = 5
        Const nARM_1 As Integer = 6
        Const nARM_MAX As Integer = 7

        ' Open the file to read from.
        Dim sTitleReq As String = gpsRM.GetString("psSCREENCAPTION")
        Dim sTableStart(0) As String
        sTableStart(0) = ""
        Dim sHeader As String = Nothing
        Dim oDT As DataTable = Nothing
        ImportExport.GetDTFromCSV(sTitleReq, sTableStart, sHeader, oDT)
        If sHeader IsNot Nothing AndAlso oDT IsNot Nothing Then
            Dim nStep As Integer = 1
            Dim nColumns() As Integer = Nothing
            Dim nMaxcol As Integer = -1
            For Each oRow As DataRow In oDT.Rows
                Dim sTmp As String = String.Empty
                For Each oItem As Object In oRow.ItemArray
                    sTmp = sTmp & vbTab & oItem.ToString
                Next
                sTmp = sTmp.Substring(1)
                Dim sText() As String = Split(sTmp, vbTab)
                nMaxcol = sText.GetUpperBound(0)
                Select Case nStep
                    Case 1
                        'column headers
                        If InStr(sText(0), lblFanucColorCap.Text) > 0 Then
                            ReDim nColumns(nMaxcol)
                            For Each nColNum As Integer In nColumns
                                nColNum = -1
                            Next
                            For nCol As Integer = 0 To nMaxcol
                                Select Case sText(nCol)
                                    Case lblFanucColorCap.Text
                                        nColumns(nCol) = nFANUC_COLOR
                                    Case lblPlantColorCap.Text
                                        nColumns(nCol) = nPLANT_COLOR
                                    Case lblColorDescCap.Text
                                        nColumns(nCol) = nCOLOR_DESC
                                    Case lblValveNumberCap.Text
                                        nColumns(nCol) = nVALVE_NUMBER
                                    Case lblValveDescCap.Text
                                        nColumns(nCol) = nVALVE_DESC
                                    Case colArms.Item(0).Name()
                                        nColumns(nCol) = nARM_1
                                        Dim nCol2 As Integer = nCol
                                        Dim nColMax As Integer = nCol
                                        Dim bDone As Boolean = False
                                        Do While nCol2 <= nMaxcol And bDone = False
                                            bDone = True
                                            For Each oArm As clsArm In colArms
                                                If oArm.Name() = sText(nCol2) Then
                                                    bDone = False
                                                    nColMax = nCol2
                                                End If
                                            Next
                                            nCol2 = nCol2 + 1
                                        Loop
                                        nColumns(nColMax) = nARM_MAX
                                    Case Else
                                End Select
                            Next
                            nStep = 2
                        End If
                    Case 2
                        Dim nFanucColor As Integer = 0
                        Dim sPlantColor As String = String.Empty
                        Dim sColorDesc As String = String.Empty
                        Dim nValve As Integer = 0
                        Dim sValveDesc As String = String.Empty
                        Dim nArmEnable() As Integer = Nothing
                        For nCol As Integer = 0 To nMaxcol
                            Try
                                Select Case nColumns(nCol)
                                    Case nFANUC_COLOR
                                        If sText(nCol).Length > 0 And IsNumeric(sText(nCol)) Then
                                            nFanucColor = CInt(sText(nCol))
                                        End If
                                    Case nPLANT_COLOR
                                        If mRobot.SystemColors.IsAscii Then
                                            If sText(nCol).Length > 0 Then
                                                sPlantColor = sText(nCol)
                                            End If
                                        Else
                                            If sText(nCol).Length > 0 And IsNumeric(sText(nCol)) Then
                                                sPlantColor = sText(nCol)
                                            End If
                                        End If
                                    Case nCOLOR_DESC
                                        If sText(nCol).Length > 0 Then
                                            sColorDesc = sText(nCol)
                                        End If
                                    Case nVALVE_NUMBER
                                        If sText(nCol).Length > 0 And IsNumeric(sText(nCol)) Then
                                            nValve = CInt(sText(nCol))
                                        End If
                                    Case nVALVE_DESC
                                        If sText(nCol).Length > 0 Then
                                            sValveDesc = sText(nCol)
                                        End If
                                    Case nARM_1
                                        ReDim nArmEnable(colArms.Count - 1)
                                        For Each nItem As Integer In nArmEnable
                                            nItem = -1
                                        Next
                                        Dim bDone As Boolean = False
                                        Dim nArm As Integer = 0
                                        Do While (nArm < colArms.Count) And ((nCol + nArm) <= nMaxcol) And bDone = False
                                            If sText(nCol + nArm) = "X" Then
                                                nArmEnable(nArm) = 1
                                            ElseIf sText(nCol + nArm) = "." Then
                                                nArmEnable(nArm) = 0
                                            End If
                                            If nColumns(nCol) = nARM_MAX Then
                                                bDone = True
                                            End If
                                            nArm = nArm + 1
                                        Loop
                                    Case Else
                                        'ignore unknown columns
                                End Select
                            Catch ex As Exception
                            End Try
                        Next
                        Try
                            If (nFanucColor > 0) And (nFanucColor <= mRobot.SystemColors.EffectiveColors) Then
                                Dim oCol As SystemColors.clsSysColor = mRobot.SystemColors.Item(nFanucColor - 1)
                                If sPlantColor <> String.Empty Then
                                    oCol.PlantNumber.Text = sPlantColor
                                End If
                                If sColorDesc <> String.Empty Then
                                    oCol.Description.Text = sColorDesc
                                End If
                                If sValveDesc <> String.Empty Then
                                    oCol.Valve.Description.Text = sValveDesc
                                End If
                                If nValve <> 0 Then
                                    oCol.Valve.Number.Value = nValve
                                End If
                                If nArmEnable IsNot Nothing Then
                                    Dim nValue As Integer = oCol.RobotsRequired.Value
                                    Dim nBitOffset As Integer = colZones.ActiveZone.RobotsRequiredStartingBit
                                    Dim nArmMax As Integer = colArms.Count - 1
                                    If nArmMax > nArmEnable.GetUpperBound(0) Then
                                        nArmMax = nArmEnable.GetUpperBound(0)
                                    End If
                                    For nArmIdx As Integer = 0 To nArmMax
                                        Dim nBot As Integer = CType(2 ^ (colArms.Item(nArmIdx).RobotNumber - 1 + nBitOffset), Integer)
                                        If (nValue And nBot) = nBot Then 'Currently Enbaled
                                            If nArmEnable(nArmIdx) = 0 Then ' import disable
                                                nValue = nValue - nBot
                                            End If
                                        Else 'Currently Disabled
                                            If nArmEnable(nArmIdx) = 1 Then ' import enable
                                                nValue = nValue + nBot
                                            End If
                                        End If
                                    Next
                                    oCol.RobotsRequired.Value = nValue
                                End If
                                EditsMade = True
                            End If

                        Catch ex As Exception

                        End Try
                End Select
            Next
        End If
        subShowNewPage()


        'Dim sFile As String = String.Empty
        'Try
        '    Const sCSV_EXT As String = "csv"
        '    Dim o As New OpenFileDialog
        '    With o
        '        .Filter = gcsRM.GetString("csSAVE_CSV_CDFILTER")
        '        .Title = gcsRM.GetString("csOPEN_FILE_DLG_CAP")
        '        .AddExtension = True
        '        .CheckPathExists = True
        '        .DefaultExt = "." & sCSV_EXT
        '        If .ShowDialog() = System.Windows.Forms.DialogResult.OK Then
        '            sFile = .FileName
        '        End If
        '    End With
        'Catch ex As Exception

        'End Try
        '' Open the file to read from.
        'Dim sr As System.IO.StreamReader = Nothing
        'Try
        '    'process the file
        '    If sFile <> String.Empty Then
        '        'Open the stream and read it back.
        '        sr = System.IO.File.OpenText(sFile)
        '        Dim nStep As Integer = 0
        '        Dim nColumns() As Integer = Nothing
        '        Dim nMaxCol As Integer = 0
        '        Do While sr.Peek() >= 0
        '            Dim sText() As String = Split(sr.ReadLine(), ",")
        '            nMaxCol = sText.GetUpperBound(0)
        '            Select Case nStep
        '                Case 0
        '                    If InStr(sText(0), gpsRM.GetString("psSCREENCAPTION")) > 0 Then
        '                        nStep = 1
        '                    End If
        '                Case 1
        '                    'column headers
        '                    If InStr(sText(0), lblFanucColorCap.Text) > 0 Then
        '                        ReDim nColumns(nMaxCol)
        '                        For Each nColNum As Integer In nColumns
        '                            nColNum = -1
        '                        Next
        '                        For nCol As Integer = 0 To nMaxCol
        '                            Select Case sText(nCol)
        '                                Case lblFanucColorCap.Text
        '                                    nColumns(nCol) = nFANUC_COLOR
        '                                Case lblPlantColorCap.Text
        '                                    nColumns(nCol) = nPLANT_COLOR
        '                                Case lblColorDescCap.Text
        '                                    nColumns(nCol) = nCOLOR_DESC
        '                                Case lblValveNumberCap.Text
        '                                    nColumns(nCol) = nVALVE_NUMBER
        '                                Case lblValveDescCap.Text
        '                                    nColumns(nCol) = nVALVE_DESC
        '                                Case colArms.Item(0).Name()
        '                                    nColumns(nCol) = nARM_1
        '                                    Dim nCol2 As Integer = nCol
        '                                    Dim nColMax As Integer = nCol
        '                                    Dim bDone As Boolean = False
        '                                    Do While nCol2 <= nMaxCol And bDone = False
        '                                        bDone = True
        '                                        For Each oArm As clsArm In colArms
        '                                            If oArm.Name() = sText(nCol2) Then
        '                                                bDone = False
        '                                                nColMax = nCol2
        '                                            End If
        '                                        Next
        '                                        nCol2 = nCol2 + 1
        '                                    Loop
        '                                    nColumns(nColMax) = nARM_MAX
        '                                Case Else
        '                            End Select
        '                        Next
        '                        nStep = 2
        '                    End If
        '                Case 2
        '                    Dim nFanucColor As Integer = 0
        '                    Dim sPlantColor As String = String.Empty
        '                    Dim sColorDesc As String = String.Empty
        '                    Dim nValve As Integer = 0
        '                    Dim sValveDesc As String = String.Empty
        '                    Dim nArmEnable() As Integer = Nothing
        '                    For nCol As Integer = 0 To nMaxCol
        '                        Try
        '                            Select Case nColumns(nCol)
        '                                Case nFANUC_COLOR
        '                                    If sText(nCol).Length > 0 And IsNumeric(sText(nCol)) Then
        '                                        nFanucColor = CInt(sText(nCol))
        '                                    End If
        '                                Case nPLANT_COLOR
        '                                    If mRobot.SystemColors.IsAscii Then
        '                                        If sText(nCol).Length > 0 Then
        '                                            sPlantColor = sText(nCol)
        '                                        End If
        '                                    Else
        '                                        If sText(nCol).Length > 0 And IsNumeric(sText(nCol)) Then
        '                                            sPlantColor = sText(nCol)
        '                                        End If
        '                                    End If
        '                                Case nCOLOR_DESC
        '                                    If sText(nCol).Length > 0 Then
        '                                        sColorDesc = sText(nCol)
        '                                    End If
        '                                Case nVALVE_NUMBER
        '                                    If sText(nCol).Length > 0 And IsNumeric(sText(nCol)) Then
        '                                        nValve = CInt(sText(nCol))
        '                                    End If
        '                                Case nVALVE_DESC
        '                                    If sText(nCol).Length > 0 Then
        '                                        sValveDesc = sText(nCol)
        '                                    End If
        '                                Case nARM_1
        '                                    ReDim nArmEnable(colArms.Count - 1)
        '                                    For Each nItem As Integer In nArmEnable
        '                                        nItem = -1
        '                                    Next
        '                                    Dim bDone As Boolean = False
        '                                    Dim nArm As Integer = 0
        '                                    Do While (nArm < colArms.Count) And ((nCol + nArm) <= nMaxCol) And bDone = False
        '                                        If sText(nCol + nArm) = "X" Then
        '                                            nArmEnable(nArm) = 1
        '                                        ElseIf sText(nCol + nArm) = "." Then
        '                                            nArmEnable(nArm) = 0
        '                                        End If
        '                                        If nColumns(nCol) = nARM_MAX Then
        '                                            bDone = True
        '                                        End If
        '                                        nArm = nArm + 1
        '                                    Loop
        '                                Case Else
        '                                    'ignore unknown columns
        '                            End Select
        '                        Catch ex As Exception
        '                        End Try
        '                    Next
        '                    Try
        '                        If (nFanucColor > 0) And (nFanucColor <= mRobot.SystemColors.EffectiveColors) Then
        '                            Dim oCol As SystemColors.clsSysColor = mRobot.SystemColors.Item(nFanucColor - 1)
        '                            If sPlantColor <> String.Empty Then
        '                                oCol.PlantNumber.Text = sPlantColor
        '                            End If
        '                            If sColorDesc <> String.Empty Then
        '                                oCol.Description.Text = sColorDesc
        '                            End If
        '                            If sValveDesc <> String.Empty Then
        '                                oCol.Valve.Description.Text = sValveDesc
        '                            End If
        '                            If nValve <> 0 Then
        '                                oCol.Valve.Number.Value = nValve
        '                            End If
        '                            If nArmEnable IsNot Nothing Then
        '                                Dim nValue As Integer = oCol.RobotsRequired.Value
        '                                Dim nBitOffset As Integer = colZones.ActiveZone.RobotsRequiredStartingBit
        '                                Dim nArmMax As Integer = colArms.Count - 1
        '                                If nArmMax > nArmEnable.GetUpperBound(0) Then
        '                                    nArmMax = nArmEnable.GetUpperBound(0)
        '                                End If
        '                                For nArmIdx As Integer = 0 To nArmMax
        '                                    Dim nBot As Integer = CType(2 ^ (colArms.Item(nArmIdx).RobotNumber - 1 + nBitOffset), Integer)
        '                                    If (nValue And nBot) = nBot Then 'Currently Enbaled
        '                                        If nArmEnable(nArmIdx) = 0 Then ' import disable
        '                                            nValue = nValue - nBot
        '                                        End If
        '                                    Else 'Currently Disabled
        '                                        If nArmEnable(nArmIdx) = 1 Then ' import enable
        '                                            nValue = nValue + nBot
        '                                        End If
        '                                    End If
        '                                Next
        '                                oCol.RobotsRequired.Value = nValue
        '                            End If
        '                            EditsMade = True
        '                        End If

        '                    Catch ex As Exception

        '                    End Try
        '            End Select
        '        Loop
        '        sr.Close()

        '    End If
        '    subShowNewPage()
        'Catch ex As Exception

        'Finally
        '    If (sr IsNot Nothing) Then
        '        sr.Close()
        '    End If

        'End Try
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

            'If moPrivilege.Privilege = String.Empty Then 'RJO 03/21/12
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

    Private Sub tabMain_Deselecting(ByVal sender As Object, ByVal e As  _
                        System.Windows.Forms.TabControlCancelEventArgs) Handles tabMain.Deselecting
        '********************************************************************************************
        'Description:  Determine which tab we just left, and record scroll position for use in the
        '               paint event of the new tab selected
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select Case e.TabPageIndex
            Case 0
                mnScrollPtr = pnlTab1.VerticalScroll.Value
            Case 1
                mnScrollPtr = pnlTab2.VerticalScroll.Value
            Case 2
                'mnScrollPtr = pnlTab3.VerticalScroll.Value
            Case 3
                'mnScrollPtr = pnlTab4.VerticalScroll.Value

        End Select
        ' Trace.WriteLine("deselect " & e.TabPage.Name & "  " & mnScrollPtr.ToString)

    End Sub

    Private Sub tabMain_Selected(ByVal sender As Object, ByVal e As System.Windows.Forms.TabControlEventArgs) Handles tabMain.Selected
        '********************************************************************************************
        'Description:  resize all the panels
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        frmMain_Layout()
    End Sub
    Private Sub tabMain_Selecting(ByVal sender As Object, _
                ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles tabMain.Selecting
        '********************************************************************************************
        'Description:  Determine which tab we just left, and record scroll position for use in the
        '               paint event of the new tab selected
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim p As Panel

        Select Case e.TabPageIndex
            Case 0
                p = pnlTab1
            Case 1
                p = pnlTab2
                'Case 2
                '     p = pnlTab3
                'Case 3
                '     p = pnlTab4
            Case Else
                p = pnlTab1
        End Select
        If mnScrollPtr > p.VerticalScroll.Maximum Then
            p.VerticalScroll.Value = p.VerticalScroll.Maximum
        Else
            p.VerticalScroll.Value = mnScrollPtr
        End If

    End Sub
    Private Sub moPLC_ModuleError(ByVal nErrNum As Integer, ByVal sErrDesc As String, _
                     ByVal sModule As String, ByVal AdditionalInfo As String) Handles moPLC.ModuleError
        '********************************************************************************************
        'Description: PLC Read/Write error
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mbPLCFail = True
        MessageBox.Show(sErrDesc & vbCrLf & AdditionalInfo, gcsRM.GetString("csPLC_COMM_ERROR") _
                                & ":" & Err.Number.ToString(mLanguage.CurrentCulture), _
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation, _
                                MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)

    End Sub
    Private Sub subLabelClickHandler(ByVal sender As Object, _
                                                    ByVal e As System.Windows.Forms.MouseEventArgs)
        '********************************************************************************************
        'Description:  this is based on the naming convention of 
        '                   label + 3 digit number  
        '
        'Parameters:
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim l As Label = DirectCast(sender, Label)
        Dim nIndex As Integer = CType(Strings.Right(l.Name, 3), Integer) - 1

        Dim cd As New ColorDialog
        Try
            cd.AllowFullOpen = True
            cd.AnyColor = True
            cd.FullOpen = True
            cd.Color = l.BackColor
            If cd.ShowDialog() = DialogResult.OK Then
                l.BackColor = cd.Color
                mRobot.SystemColors.Item(nIndex).DisplayColor = cd.Color
                EditsMade = True
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

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
        ' 06/15/10  MSW     handle cross-thread calls
        ' 08/19/10  MSW     Fix the last fix so it''s not asking for execute authority 
        '********************************************************************************************
        'This can get called by the password  thread
        If Me.stsStatus.InvokeRequired Then
            Dim dLogin As New LoginCallBack(AddressOf moPassword_LogIn)
            Me.BeginInvoke(dLogin)
        Else
            Me.LoggedOnUser = moPassword.UserName
            Status = Me.LoggedOnUser & " " & gcsRM.GetString("csLOGGED_IN")
            'Privilege = ePrivilege.Execute 'don't allow Override Access without this priviledge 01/05/10 RJO
            'If Privilege <> ePrivilege.Execute Then
            Privilege = ePrivilege.Copy ' extra for buttons etc.
            If Privilege <> ePrivilege.Copy Then
                'didn't have clearance
                Privilege = ePrivilege.Edit
            End If
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/21/12
            'End If
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
        ' 06/15/10  MSW     handle cross-thread calls
        '********************************************************************************************
        'This can get called by the password thread
        If Me.stsStatus.InvokeRequired Then
            Dim dLogin As New LogOutCallBack(AddressOf moPassword_LogOut)
            Me.BeginInvoke(dLogin)
        Else
            Status = Me.LoggedOnUser & " " & gcsRM.GetString("csLOGGED_OUT")
            Me.LoggedOnUser = String.Empty
            Me.Privilege = ePrivilege.None
            Call mScreenSetup.DoStatusBar2(stsStatus, LoggedOnUser, mPrivilegeGranted, mbRemoteZone) 'RJO 03/21/12
        End If

    End Sub
#End Region

End Class