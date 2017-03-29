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
' Form/Module: frmCopy
'
' Description: display and select copy params - send elswhere to do copy
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: FANUC Programmer
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    06/20/07   gks     Modifications for paint shop computer
'    11/05/09   MSW     CCType, opener properties
'    11/10/09   MSW     Modifications for PaintTool 7.50 presets
'                       Reads preset config from robot.
'                       parm name on the robot is used for resource lookup:
'                       for APP_CON_NAM= "FF", rsFF = "Paint", rsFF_CAP = "Paint cc/min", rsFF_UNITS ="
'                       supports 2nd shaping air, volume on CC screen
'    11/19/09   RJO     Modified calls in subChangeZone to LoadArmCollection for colArmsFrom and colArmsTo to
'                       use new version of LoadArmCollection with IncludeOpeners parameter.
'    07/04/10   RJO     Modified tlsMain_ItemClicked to present a less confusing message to the user if 
'                       the Copy Values button is clicked but no parameters have been selected from the 
'                       drop menu. CommonStrings.resx was also modified for this purpose.
'    12/1/11    MSW     Mark a version                                                 4.01.01.00
'    01/09/12   MSW     Manage enable buttons during copy                              4.01.01.01
'    04/30/12   RJO     Added copy confimation messages for all copy buttons. Added    4.01.01.02
'                       property ParamDetails to support this feature.
'    01/01/13   RJO     Added Function nGetStyleSelection. Modified all calls to       4.01.01.03
'                       mPresetCommon.LoadCopyScreenSubParamBox to pass in Style.
'                       Added code to update preset descriptions based on Style
'                       selections.
'    04/02/13   DE      Added localization to subInitFormText for lblZone and lblRobot 4.01.01.04
'    08/20/13   MSW     Progress, Status - Add error handler so it doesn't get hung up  4.01.05.00
'    09/30/13   MSW     Use the control's min and max instead of literal constants    4.01.05.01
'    01/06/14   MSW     Disable the form controlbox                                   4.01.06.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On


Imports Response = System.Windows.Forms.DialogResult
Imports System.Configuration.ConfigurationSettings
Imports System.Collections.Generic
Imports System.Collections.ObjectModel

Friend Enum eCopyType
    CopyNone = 0
    CopyAll = 1
    CopyValues = 2
    CopyText = 0
    CopyParm01 = CInt(2 ^ 0)
    CopyParm02 = CInt(2 ^ 1)
    CopyParm03 = CInt(2 ^ 2)
    CopyParm04 = CInt(2 ^ 3)
    CopyParm05 = CInt(2 ^ 4)
    CopyDesc = CInt(2 ^ 5)
    CopyValuesMask = CInt(2 ^ 5) - 1
End Enum

Friend Class frmCopy

#Region " Declares "
    '******** Form Constants   **********************************************************************
    ' if msSCREEN_NAME has a space in it you won't find the resources
    Friend Const msSCREEN_NAME As String = "Copy"   ' <-- For password area change log etc.
    Private Const mnMAININX As Integer = 100
    Private Const mnSCREENINX As Integer = 101
    Private Const msMODULE As String = "frmCopy"
    Private msALL As String = gcsRM.GetString("csALL")
    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    Private colZonesFrom As clsZones ' = Nothing
    Private colZonesTo As clsZones ' = Nothing
    Private colControllersFrom As clsControllers ' = Nothing
    Private colControllersTo As clsControllers ' = Nothing
    Private colArmsFrom As clsArms ' = Nothing
    Private colArmsTo As clsArms ' = Nothing
    Private mArmFrom As clsArm ' = Nothing
    Private mArmTo As clsArm ' = Nothing
    Private colParamFrom As Collection(Of String) ' = Nothing
    Private colParamTo As Collection(Of String) ' = Nothing
    Private colSubParamFrom As Collection(Of String) ' = Nothing
    Private colSubParamTo As Collection(Of String) ' = Nothing
    Private colStyleFrom As Collection(Of String) ' = Nothing
    Private colStyleTo As Collection(Of String) ' = Nothing
    Private mbNotWarnedYet As Boolean = True
    Private mbEnableAccessibilityFeatures As Boolean = True
    Private moParamDetails() As mPWRobotCommon.tParmDetail
    Private msFormCaption As String = String.Empty
    '******** End Form Variables    *****************************************************************

    '******** Property Variables    *****************************************************************
    Private mbDataLoaded As Boolean = False
    Private mnProgress As Integer = 0
    Private msScreen As String = msALL
    Private msUser As String = msALL
    Private msDevice As String = msALL
    Private msParameter As String = String.Empty
    Private mnPeriod As Integer = 0
    Private msScreenName As String
    Private mbUseRobots As Boolean
    Private msDatabasepath As String = String.Empty
    Private mbUseParam As Boolean = False
    Private mbUseStyle As Boolean = False
    Private mbUseRobot As Boolean = True
    Private mbUseSubParam As Boolean = False
    Private mbByArm As Boolean = True
    Private mnParamType As eParamType = eParamType.Colors
    Private msSubParamName As String = ""
    Private msParamName As String = ""
    Private mbSubParamToFromMustMatch As Boolean = False
    Private mbFancyCopyButtons As Boolean = True
    Private mbLoadSubParamByRobot As Boolean = False
    Private mCCType1 As eColorChangeType = eColorChangeType.NOT_SELECTED
    Private mCCType2 As eColorChangeType = eColorChangeType.NOT_SELECTED
    Private mbIncludeOpeners As Boolean = False
    '******** End Property Variable *************************************************************

    Public Enum eParamType
        None
        Colors
        Valves
        Degrade
    End Enum


#End Region
#Region " Properties "
    Friend WriteOnly Property ParamDetails() As mPWRobotCommon.tParmDetail()
        '********************************************************************************************
        'Description:  Added to support conformation messageboxes for Preset Copy operations
        '
        'Parameters: Parameternames
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/30/12  RJO     Initial code
        '********************************************************************************************
        Set(ByVal value As mPWRobotCommon.tParmDetail())
            moParamDetails = value
        End Set
    End Property
    Friend WriteOnly Property ParameterType() As eParamType
        '********************************************************************************************
        'Description:  sys color or valve
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
                    mbUseParam = False
                Case eParamType.Colors
                    mbUseParam = True
                Case eParamType.Valves
                    mbUseParam = True
            End Select
        End Set
    End Property
    Friend Property IncludeOpeners() As Boolean
        '********************************************************************************************
        'Description:  Include openers in robot list
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbIncludeOpeners
        End Get
        Set(ByVal Value As Boolean)
            mbIncludeOpeners = Value
        End Set

    End Property
    Friend Property CCType1() As eColorChangeType
        '********************************************************************************************
        'Description:  provide the ability to filter robots by CC type
        '
        'Parameters: 
        'Returns: 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mCCType1
        End Get
        Set(ByVal value As eColorChangeType)
            mCCType1 = value
        End Set
    End Property
    Friend Property CCType2() As eColorChangeType
        '********************************************************************************************
        'Description:  provide the ability to filter robots by CC type
        '
        'Parameters: 
        'Returns: 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mCCType2
        End Get
        Set(ByVal value As eColorChangeType)
            mCCType2 = value
        End Set
    End Property
    Friend Property FormCaption() As String
        '********************************************************************************************
        'Description:  let the main form set the caption text
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msFormCaption
        End Get
        Set(ByVal Value As String)
            msFormCaption = Value
            Me.Text = msFormCaption
        End Set
    End Property

    Friend Property ParamName() As String
        '********************************************************************************************
        'Description:  Use the first param (usually color)
        '
        'Parameters: Set true to use param (color) select
        'Returns:    True if using param (color) select
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msParamName
        End Get
        Set(ByVal Value As String)
            msParamName = Value
            lblParam.Text = msParamName
        End Set
    End Property
    Friend Property LoadSubParamByRobot() As Boolean
        '********************************************************************************************
        'Description:  load subparam boxes by robot, not color (or valve)
        '
        'Parameters: none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbLoadSubParamByRobot
        End Get
        Set(ByVal value As Boolean)
            mbLoadSubParamByRobot = value
        End Set
    End Property

    Friend Property FancyCopyButtons() As Boolean
        '********************************************************************************************
        'Description:  Enable copy all, values, descriptions
        '
        'Parameters: none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbFancyCopyButtons
        End Get
        Set(ByVal value As Boolean)
            mbFancyCopyButtons = value
        End Set
    End Property
    Friend Property SubParamToFromMustMatch() As Boolean
        '********************************************************************************************
        'Description:  Select arm names instead of controllers
        '
        'Parameters: none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbSubParamToFromMustMatch
        End Get
        Set(ByVal value As Boolean)
            mbSubParamToFromMustMatch = value
        End Set
    End Property
    Friend Property ByArm() As Boolean
        '********************************************************************************************
        'Description:  Select arm names instead of controllers
        '
        'Parameters: none
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbByArm
        End Get
        Set(ByVal value As Boolean)
            mbByArm = value
        End Set
    End Property

    Friend Property SubParamName() As String
        '********************************************************************************************
        'Description:  Use the first param (usually color)
        '
        'Parameters: Set true to use param (color) select
        'Returns:    True if using param (color) select
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return msSubParamName
        End Get
        Set(ByVal Value As String)
            msSubParamName = Value
            lblSubParam.Text = msSubParamName
            lblSubParamTo.Text = msSubParamName
        End Set
    End Property
    Friend Property UseParam() As Boolean
        '********************************************************************************************
        'Description:  Use the first param (usually color)
        '
        'Parameters: Set true to use param (color) select
        'Returns:    True if using param (color) select
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbUseParam
        End Get
        Set(ByVal Value As Boolean)
            mbUseParam = Value
            clbParamFrom.Visible = Value
            clbParamTo.Visible = Value
            lblParam.Visible = Value
            lblParamTo.Visible = Value
        End Set
    End Property

    Friend Property UseRobot() As Boolean
        '********************************************************************************************
        'Description:  Use the style cbo
        '
        'Parameters: Set true to use style select
        'Returns:    True if using style select
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbUseRobot
        End Get
        Set(ByVal value As Boolean)
            mbUseRobot = value
            clbRobotFrom.Visible = value
            clbRobotTo.Visible = value
            lblRobot.Visible = value
            LblRobotTo.Visible = value
            lblViewDevFrom.Visible = value
            lblViewDevTo.Visible = value
        End Set
    End Property
    Friend Property UseStyle() As Boolean
        '********************************************************************************************
        'Description:  Use the style cbo
        '
        'Parameters: Set true to use style select
        'Returns:    True if using style select
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbUseStyle
        End Get
        Set(ByVal value As Boolean)
            mbUseStyle = value
            clbStyleFrom.Visible = value
            clbStyleTo.Visible = value
            lblStyle.Visible = value
            lblstyleTo.Visible = value
            lblViewStyleFrom.Visible = value
            lblViewStyleTo.Visible = value
        End Set
    End Property
    Friend Property UseSubParam() As Boolean
        '********************************************************************************************
        'Description:  Use the 2nd param (preset#, ...)
        '
        'Parameters: Set true to use param (color) select
        'Returns:    True if using param (color) select
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbUseSubParam
        End Get
        Set(ByVal Value As Boolean)
            mbUseSubParam = Value
            clbSubParamFrom.Visible = Value
            clbSubParamTo.Visible = Value
            lblSubParam.Visible = Value
            lblSubParamTo.Visible = Value
        End Set
    End Property
    Friend Property DatabasePath() As String
        '********************************************************************************************
        'Description: Path to database
        '
        'Parameters: None 
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            If msDatabasepath = String.Empty Then
                'local machine
                GetDefaultFilePath(msDatabasepath, eDir.Database, String.Empty, String.Empty)
            End If
            Return msDatabasepath
        End Get
        Set(ByVal value As String)
            If value <> String.Empty Then
                If Strings.Right(value, 1) <> "\" Then
                    value = value & "\"
                End If
            End If
            msDatabasepath = value
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

#End Region
#Region " Routines "

    Private Function bAllApplicatorsTheSame(Optional ByVal bAbortCheck As Boolean = False) As Boolean
        '********************************************************************************************
        'Description: are All Applicator types the same?
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 07/06/10  MSW     Use robot names for indexing instead of numbers
        '********************************************************************************************
        Dim tTmp As eColorChangeType = Nothing
        Dim tTmp2 As eColorChangeType = Nothing
        Dim i%
        Dim sMsg As String = String.Empty

        For i% = 0 To clbRobotFrom.Items.Count - 1
            If clbRobotFrom.GetItemChecked(i%) Then
                If tTmp = Nothing Then
                    If mbByArm Then
                        tTmp = colArmsFrom.Item(clbRobotFrom.Items(i%).ToString).ColorChangeType
                    Else
                        tTmp = colControllersFrom.Item(clbRobotFrom.Items(i%).ToString).Arms(0).ColorChangeType
                    End If
                Else
                    If mbByArm Then
                        tTmp2 = colArmsFrom.Item(clbRobotFrom.Items(i%).ToString).ColorChangeType
                    Else
                        tTmp2 = colControllersFrom.Item(clbRobotFrom.Items(i%).ToString).Arms(0).ColorChangeType
                    End If
                    If tTmp <> tTmp2 Then
                        sMsg = gcsRM.GetString("csAPPLICATOR_TYPE_DIFFERENT")
                        Exit For
                    End If
                End If ' tTmp = Nothing
                End If ' clbRobotFrom.GetItemChecked(i%)
        Next

        For i% = 0 To clbRobotTo.Items.Count - 1
            If clbRobotTo.GetItemChecked(i%) Then
                If tTmp = Nothing Then
                    If mbByArm Then
                        tTmp = colArmsTo.Item(clbRobotTo.Items(i%).ToString).ColorChangeType
                    Else
                        tTmp = colControllersTo.Item(clbRobotTo.Items(i%).ToString).Arms(0).ColorChangeType
                    End If
                Else
                    If mbByArm Then
                        tTmp2 = colArmsTo.Item(clbRobotTo.Items(i%).ToString).ColorChangeType
                    Else
                        tTmp2 = colControllersTo.Item(clbRobotTo.Items(i%).ToString).Arms(0).ColorChangeType
                    End If
                    If tTmp <> tTmp2 Then
                        sMsg = gcsRM.GetString("csAPPLICATOR_TYPE_DIFFERENT")
                        Exit For
                    End If
                End If ' tTmp = Nothing
            End If ' clbRobotTo.GetItemChecked(i%)
        Next

        If sMsg = String.Empty Then Return True
        If bAbortCheck Then
            Dim lReply As Windows.Forms.DialogResult = MessageBox.Show(sMsg, gcsRM.GetString("csWARNING"), _
                                            MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)
            If lReply = Windows.Forms.DialogResult.OK Then
                Return True
            Else
                Return False
                'Pass back abort status if it comes in = true
            End If
        Else
            MessageBox.Show(sMsg, gcsRM.GetString("csWARNING"), _
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

        Return False

    End Function
    Private Sub subChangeZone(ByVal bFromComboBox As Boolean)
        '********************************************************************************************
        'Description: New Zone Selected 
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/19/09  RJO     Modified call to LoadArmCollection for colArmsFrom and colArmsTo to
        '                   use new version of LoadArmCollection with IncludeOpeners parameter.
        ' 09/14/10  MSW     skip openers for controllers, too.
        ' 01/01/13  RJO     Added Style Parameter to mPresetCommon.LoadCopyScreenSubParamBox calls.
        '********************************************************************************************

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        Try
            Progress = 50
            mbNotWarnedYet = True
            If bFromComboBox Then
                'from box
                If colZonesFrom.SetCurrentZone(cboZoneFrom.Text) = False Then
                    cboZoneFrom.SelectedIndex = -1
                    Exit Try
                End If

                clbSubParamFrom.Items.Clear()
                clbParamFrom.Items.Clear()
                clbRobotFrom.Items.Clear()
                If mbUseRobot = False Then
                    If (mbLoadSubParamByRobot = False) Then
                        '>>>>>> This is a generic call - routine needs to be provided in screen specific module
                        LoadCopyScreenSubParamBox(mArmFrom, clbSubParamFrom, lblViewSubParamFrom.Text, 0) 'RJO 01/01/13
                        '>>>>>> This is a generic call - routine needs to be provided in screen specific module
                        LoadCopyScreenSubParamBox(mArmTo, clbSubParamTo, lblViewSubParamTo.Text, 0) 'RJO 01/01/13
                    End If
                End If
                If mbUseStyle Then
                    clbStyleFrom.Items.Clear()
                    Dim oStyles As New clsSysStyles(colZonesFrom.ActiveZone)
                    oStyles.LoadStyleBoxFromCollection(clbStyleFrom, False, False, True)
                End If
                lblViewDevFrom.Text = cboZoneFrom.Text
                lblViewParamFrom.Text = String.Empty
                lblViewSubParamFrom.Text = String.Empty
                If mbUseRobot Then
                    colControllersFrom = New clsControllers(colZonesFrom, False)
                    colArmsFrom = LoadArmCollection(colControllersFrom, mbIncludeOpeners)
                    If ByArm Then
                        LoadRobotBoxFromCollection(clbRobotFrom, colArmsFrom, False, CCType1, CCType2, mbIncludeOpeners)
                    Else
                        LoadRobotBoxFromCollection(clbRobotFrom, colControllersFrom, False, mbIncludeOpeners)
                    End If
                End If
            Else
                'to box

                clbSubParamTo.Items.Clear()
                clbParamTo.Items.Clear()
                clbRobotTo.Items.Clear()
                If mbUseRobot = False Then
                    If (mbLoadSubParamByRobot = False) Then
                        '>>>>>> This is a generic call - routine needs to be provided in screen specific module
                        LoadCopyScreenSubParamBox(mArmFrom, clbSubParamFrom, lblViewSubParamFrom.Text, 0) 'RJO 01/01/13
                        '>>>>>> This is a generic call - routine needs to be provided in screen specific module
                        LoadCopyScreenSubParamBox(mArmTo, clbSubParamTo, lblViewSubParamTo.Text, 0) 'RJO 01/01/13
                    End If
                End If
                If mbUseStyle Then
                    clbStyleTo.Items.Clear()
                    Dim oStyles As New clsSysStyles(colZonesFrom.ActiveZone)
                    oStyles.LoadStyleBoxFromCollection(clbStyleTo, False, False, True)
                End If

                If clbZoneTo.CheckedItems.Count > 0 Then
                    Dim sZone As String = String.Empty
                    For Each o As Object In clbZoneTo.CheckedItems
                        sZone = sZone & o.ToString & ", "
                    Next
                    lblViewDevTo.Text = sZone.Substring(0, (sZone.Length - 2))
                Else
                    lblViewDevTo.Text = String.Empty
                End If
                lblViewParamTo.Text = String.Empty
                lblViewSubParamTo.Text = String.Empty
                If mbUseRobot Then
                    colControllersTo = New clsControllers(colZonesTo, False)
                    colArmsTo = LoadArmCollection(colControllersTo, mbIncludeOpeners)
                    If ByArm Then
                        LoadRobotBoxFromCollection(clbRobotTo, colArmsTo, False, CCType1, CCType2, mbIncludeOpeners) 'NVSP 12/08/2016 colArmsFrom to colArmsTo
                    Else
                        LoadRobotBoxFromCollection(clbRobotTo, colControllersTo, False, mbIncludeOpeners)
                    End If
                    'want this true to not overrite any selections made in from box
                    subRobotChangeHandler(True)
                End If
            End If


        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csCOULD_NOT_LOAD_ROB"), ex, Me.Text, _
                                 Status, MessageBoxButtons.OK)
        Finally
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try
    End Sub
    Private Function bCheckRobotSelection() As Boolean
        '********************************************************************************************
        'Description: Check if the robot(s) selected make sense
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If clbRobotFrom.CheckedItems.Count = 0 Then Return False
        If clbRobotTo.Items.Count = 0 Then Return False

        If clbRobotFrom.CheckedItems.Count > 1 Then
            'if more than 1 robot selected, same target robots should be selected
            ' dunno what to do for different zones
            For i As Integer = 0 To clbRobotFrom.Items.Count - 1
                If i > (clbRobotTo.Items.Count - 1) Then Exit For
                If clbRobotFrom.GetItemChecked(i) <> clbRobotTo.GetItemChecked(i) Then
                    MessageBox.Show(gcsRM.GetString("csCOPY_ROBOTS_NOT_SAME"), _
                                                        gcsRM.GetString("csWARNING"), _
                                                        MessageBoxButtons.OK, _
                                                        MessageBoxIcon.Information)
                    Return False
                End If ' clbRobotFrom.GetItemChecked(i%)
            Next
            Return True
        Else
            'one source selected
            If clbRobotTo.CheckedItems.Count = 0 Then
                Return False
            Else
                Return True
            End If
        End If

    End Function
    Private Function bCheckParameterSelection() As Boolean
        '********************************************************************************************
        'Description: Check if the Param(s) selected make sense
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If clbParamFrom.CheckedItems.Count = 0 Then Return False
        If clbParamTo.Items.Count = 0 Then Return False

        If clbParamFrom.CheckedItems.Count > 1 Then
            'if more than 1 Param selected, same target Params should be selected
            ' dunno what to do for different zones
            For i As Integer = 0 To clbParamFrom.Items.Count - 1
                If i > (clbParamTo.Items.Count - 1) Then Exit For
                If clbParamFrom.GetItemChecked(i) <> clbParamTo.GetItemChecked(i) Then
                    MessageBox.Show(gcsRM.GetString("csCOPY_PARAM_NOT_SAME"), _
                                                        gcsRM.GetString("csWARNING"), _
                                                        MessageBoxButtons.OK, _
                                                        MessageBoxIcon.Information, _
                                                        MessageBoxDefaultButton.Button1, _
                                                        MessageBoxOptions.DefaultDesktopOnly)
                    Return False
                End If ' clbParamFrom.GetItemChecked(i%)
            Next
            Return True
        Else
            'one source selected
            If clbParamTo.CheckedItems.Count = 0 Then
                Return False
            Else
                Return True
            End If
        End If

    End Function
    Private Function bCheckStyleSelection() As Boolean
        '********************************************************************************************
        'Description: Check if the Styles selected make sense
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If clbStyleFrom.CheckedItems.Count = 0 Then Return False
        If clbStyleTo.Items.Count = 0 Then Return False

        If clbStyleFrom.CheckedItems.Count > 1 Then
            'if more than 1 Param selected, same target Params should be selected
            ' dunno what to do for different zones
            For i As Integer = 0 To clbStyleFrom.Items.Count - 1
                If i > (clbStyleTo.Items.Count - 1) Then Exit For
                If clbStyleFrom.GetItemChecked(i) <> clbStyleTo.GetItemChecked(i) Then
                    MessageBox.Show(gpsRM.GetString("psCOPY_STYLE_NOT_SAME"), _
                                                        gcsRM.GetString("csWARNING"), _
                                                        MessageBoxButtons.OK, _
                                                        MessageBoxIcon.Information, _
                                                        MessageBoxDefaultButton.Button1, _
                                                        MessageBoxOptions.DefaultDesktopOnly)
                    Return False
                End If ' clbStyleFrom.GetItemChecked(i%)
            Next
            Return True
        Else
            'one source selected
            If clbStyleTo.CheckedItems.Count = 0 Then
                Return False
            Else
                Return True
            End If
        End If

    End Function
    Private Function bCheckSubParameterSelection() As Boolean
        '********************************************************************************************
        'Description: Check if the Param(s) selected make sense
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If clbSubParamFrom.Visible = False Then Return True

        If clbSubParamFrom.CheckedItems.Count = 0 Then Return False
        If clbSubParamTo.Items.Count = 0 Then Return False

        If clbSubParamFrom.CheckedItems.Count > 1 Then
            'if more than 1 Param selected, same target Params should be selected
            ' dunno what to do for different zones
            For i As Integer = 0 To clbSubParamFrom.Items.Count - 1
                If i > (clbSubParamTo.Items.Count - 1) Then Exit For
                If clbSubParamFrom.GetItemChecked(i) <> clbSubParamTo.GetItemChecked(i) Then
                    MessageBox.Show(gcsRM.GetString("csCOPY_SUBPARAM_NOT_SAME"), _
                                                        gcsRM.GetString("csWARNING"), _
                                                        MessageBoxButtons.OK, _
                                                        MessageBoxIcon.Information)
                    Return False
                End If ' clbSubParamFrom.GetItemChecked(i%)
            Next
            Return True
        Else
            'one source selected
            If clbSubParamTo.CheckedItems.Count = 0 Then
                Return False
            Else
                Return True
            End If
        End If

    End Function
    Private Function bFromSelectionComplete() As Boolean
        '********************************************************************************************
        'Description: are there enough selections to enable the to frame
        '
        'Parameters:  
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bTmp As Boolean

        If (cboZoneFrom.Text <> String.Empty) And  (clbParamFrom.CheckedItems.Count > 0) Then

            If clbSubParamFrom.Visible Then
                bTmp = (clbSubParamFrom.CheckedItems.Count > 0)
            Else
                bTmp = True
            End If
            If mbUseRobot Then
                bTmp = bTmp And (clbRobotFrom.CheckedItems.Count > 0)
            End If
            If mbUseStyle Then
                bTmp = bTmp And (clbStyleFrom.CheckedItems.Count > 0)
            End If

            Status = gcsRM.GetString("csSELECT_TARGET")
        Else
            bTmp = False
        End If

        Return bTmp

    End Function
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

        clbRobotFrom.Items.Clear()
        clbParamFrom.Items.Clear()
        clbSubParamFrom.Items.Clear()

        clbRobotTo.Items.Clear()
        clbParamTo.Items.Clear()
        clbSubParamTo.Items.Clear()
        clbStyleTo.Items.Clear()
        clbStyleFrom.Items.Clear()
    End Sub
    Private Sub subDoCopy(ByVal vType As eCopyType)
        '********************************************************************************************
        'Description: Call the copy routine in the applications common module
        '
        'Parameters: what kind of copy
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/21/07  gks     Switch to fanuc number for parameter arrays - parameter text may be 
        '                   different across zones
        ' 01/09/12  MSW     Manage enable buttons during copy
        ' 01/01/13  RJO     Added Style Parameter to mPresetCommon.LoadCopyScreenSubParamBox calls.
        '********************************************************************************************
        Dim oZ As clsZone
        Dim i%
        Dim sTag() As String

        Me.Cursor = Cursors.WaitCursor

        lstStatus.Items.Clear()
        subEnableControls(False)
        lstStatus.Visible = True
        Try
            For Each oZ In colZonesFrom
                If oZ.Name = cboZoneFrom.Text Then
                    oZ.Selected = True
                Else
                    oZ.Selected = False
                End If
            Next

            For i% = 0 To clbZoneTo.Items.Count - 1
                colZonesTo.Item(clbZoneTo.Items(i%).ToString).Selected = _
                                                clbZoneTo.GetItemChecked(i%)
            Next
            If mbUseRobot Then
                If mbByArm Then
                    For i% = 0 To clbRobotFrom.Items.Count - 1
                        colArmsFrom.Item(clbRobotFrom.Items(i%).ToString).Selected = _
                                                                            clbRobotFrom.GetItemChecked(i%)
                    Next

                    For i% = 0 To clbRobotTo.Items.Count - 1
                        colArmsTo.Item(clbRobotTo.Items(i%).ToString).Selected = _
                                                                            clbRobotTo.GetItemChecked(i%)
                    Next
                Else
                    For Each oArm As clsArm In colArmsFrom
                        oArm.Selected = False
                    Next
                    For Each oArm As clsArm In colArmsTo
                        oArm.Selected = False
                    Next
                    For i% = 0 To clbRobotFrom.Items.Count - 1
                        If clbRobotFrom.GetItemChecked(i%) Then
                            For Each oArm As clsArm In colArmsFrom
                                If oArm.Controller.Name = clbRobotFrom.Items(i%).ToString Then
                                    oArm.Selected = True
                                    Exit For
                                End If
                            Next
                        End If
                    Next

                    For i% = 0 To clbRobotTo.Items.Count - 1
                        If clbRobotTo.GetItemChecked(i%) Then
                            For Each oArm As clsArm In colArmsTo
                                If oArm.Controller.Name = clbRobotTo.Items(i%).ToString Then
                                    oArm.Selected = True
                                    Exit For
                                End If
                            Next
                        End If
                    Next
                End If
            End If

            colParamFrom = New Collection(Of String)
            If mbUseParam Then
                sTag = DirectCast(clbParamFrom.Tag, String())
                For i% = 0 To clbParamFrom.Items.Count - 1
                    ''If clbParamFrom.GetItemChecked(i%) Then _
                    ''                                colParamFrom.Add(clbParamFrom.Items(i%).ToString)
                    If clbParamFrom.GetItemChecked(i%) Then  'using fanuc number
                        colParamFrom.Add(sTag(i%))
                    End If
                Next
            Else
                colParamFrom.Add("1")
            End If


            colParamTo = New Collection(Of String)
            If mbUseParam Then
                sTag = DirectCast(clbParamTo.Tag, String())
                For i% = 0 To clbParamTo.Items.Count - 1
                    ''If clbParamTo.GetItemChecked(i%) Then _
                    ''                                colParamTo.Add(clbParamTo.Items(i%).ToString)
                    If clbParamTo.GetItemChecked(i%) Then 'using fanuc number
                        colParamTo.Add(sTag(i%))
                    End If
                Next
            Else
                colParamTo.Add("1")
            End If

            colSubParamFrom = New Collection(Of String)
            For i% = 0 To clbSubParamFrom.Items.Count - 1
                If clbSubParamFrom.GetItemChecked(i%) Then _
                                                colSubParamFrom.Add(clbSubParamFrom.Items(i%).ToString)
            Next

            colSubParamTo = New Collection(Of String)
            For i% = 0 To clbSubParamTo.Items.Count - 1
                If clbSubParamTo.GetItemChecked(i%) Then _
                                                colSubParamTo.Add(clbSubParamTo.Items(i%).ToString)
            Next


            colStyleFrom = New Collection(Of String)
            If mbUseStyle Then
                sTag = DirectCast(clbStyleFrom.Tag, String())
                For i% = 0 To clbStyleFrom.Items.Count - 1
                    ''If clbParamFrom.GetItemChecked(i%) Then _
                    ''                                colParamFrom.Add(clbParamFrom.Items(i%).ToString)
                    If clbStyleFrom.GetItemChecked(i%) Then  'using fanuc number
                        colStyleFrom.Add(sTag(i%))
                    End If
                Next
            Else
                colStyleFrom.Add("1")
            End If

            colStyleTo = New Collection(Of String)
            If mbUseStyle Then
                sTag = DirectCast(clbStyleTo.Tag, String())
                For i% = 0 To clbStyleTo.Items.Count - 1
                    ''If clbParamFrom.GetItemChecked(i%) Then _
                    ''                                colParamFrom.Add(clbParamFrom.Items(i%).ToString)
                    If clbStyleTo.GetItemChecked(i%) Then  'using fanuc number
                        colStyleTo.Add(sTag(i%))
                    End If
                Next
            Else
                colStyleTo.Add("1")
            End If

            If lstStatus.Visible = False Then btnStatus.PerformClick()
            '>>>>>> This is a generic call - routine needs to be provided in screen specific module
            '' do the copy here
            Dim bTmp As Boolean = False
            If mbUseStyle Then
                bTmp = DoCopy(colZonesFrom, colZonesTo, colArmsFrom, colArmsTo, colParamFrom, _
                                colParamTo, colSubParamFrom, colSubParamTo, vType, Me, colStyleFrom, colStyleTo)
            Else
                bTmp = DoCopy(colZonesFrom, colZonesTo, colArmsFrom, colArmsTo, colParamFrom, _
                colParamTo, colSubParamFrom, colSubParamTo, vType, Me)
            End If
            If bTmp Then
                If clbSubParamFrom.Visible = False Then
                    For i% = 0 To clbParamTo.Items.Count - 1
                        clbParamTo.SetItemChecked(i%, False)
                    Next
                    For i% = 0 To clbParamFrom.Items.Count - 1
                        clbParamFrom.SetItemChecked(i%, False)
                    Next
                Else
                    For i% = 0 To clbSubParamTo.Items.Count - 1
                        clbSubParamTo.SetItemChecked(i%, False)
                    Next
                    For i% = 0 To clbSubParamFrom.Items.Count - 1
                        clbSubParamFrom.SetItemChecked(i%, False)
                    Next
                End If

                '****-----------------debug
                '''Shell("notepad.exe c:\temp\Presetcopy.log", AppWinStyle.NormalFocus)

                mPWCommon.SaveToChangeLog(colZonesTo.ActiveZone)

                Status = gcsRM.GetString("csCOPY_COMPLETE")
            Else
                ' copy failed
                Status = gcsRM.GetString("csCOPY_FAILED")
            End If

            'Update subparam boxes after copy
            '>>>>>> This is a generic call - routine needs to be provided in screen specific module
            Dim nStyle As Integer = nGetStyleSelection(clbStyleFrom) 'RJO 01/01/13
            LoadCopyScreenSubParamBox(mArmFrom, clbSubParamFrom, lblViewSubParamFrom.Text, nStyle) 'RJO 01/01/13
            '>>>>>> This is a generic call - routine needs to be provided in screen specific module
            nStyle = nGetStyleSelection(clbStyleTo) 'RJO 01/01/13
            LoadCopyScreenSubParamBox(mArmTo, clbSubParamTo, lblViewSubParamTo.Text, nStyle) 'RJO 01/01/13


        Catch ex As Exception
            ShowErrorMessagebox(gcsRM.GetString("csCOPY_FAILED"), ex, msSCREEN_NAME, _
                Status, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Trace.WriteLine(ex.Message)
        Finally
            subEnableControls(True)
            Me.Cursor = Cursors.Default
        End Try

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
        ' 01/09/12  MSW     Manage enable buttons during copy
        '********************************************************************************************
        Dim bRestOfControls As Boolean = False

        btnClose.Enabled = bEnable

        If bEnable = False Then
            btnStatus.Enabled = True
            bRestOfControls = False
        Else
            Select Case frmMain.Privilege
                Case ePrivilege.None
                    btnStatus.Enabled = True
                    bRestOfControls = False
                Case Else
                    btnStatus.Enabled = True
                    bRestOfControls = bEnable
            End Select
        End If


        tlsMain.Refresh()

        'restof controls here
        tlsMain.Enabled = bRestOfControls
        pnlMain.Enabled = bRestOfControls
        gpbTo.Enabled = bRestOfControls
        gpbFrom.Enabled = bRestOfControls

    End Sub
    Shared Sub subFormatScreenLayout()
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
    Private Sub subRobotChangeHandler(ByVal bFromBox As Boolean)
        '********************************************************************************************
        'Description:  ?
        '                
        '
        'SubParameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/15/07  gks     redo - all this forcing stuff was messing up PSC
        '********************************************************************************************
        Dim i%

        If clbRobotFrom.Items.Count = 0 Then Exit Sub
        If clbRobotTo.Items.Count = 0 Then Exit Sub

        If bFromBox Then

            If clbRobotFrom.CheckedItems.Count > 1 Then
                If mbUseSubParam Or mbUseParam Then
                    'Allow multiple select
                    'need to make to robots match, indexwise anyway
                    If (clbRobotFrom.Items.Count > clbRobotTo.Items.Count) Then
                        'more from than to
                        For i% = 0 To (clbRobotFrom.Items.Count - 1)
                            If i% > clbRobotTo.Items.Count - 1 Then
                                clbRobotFrom.SetItemChecked(i%, False)
                            Else
                                clbRobotTo.SetItemChecked(i%, clbRobotFrom.GetItemChecked(i%))
                            End If
                        Next

                    ElseIf (clbRobotFrom.Items.Count < clbRobotTo.Items.Count) Then
                        'more to than from
                        For i% = 0 To (clbRobotTo.Items.Count - 1)
                            If i% > clbRobotFrom.Items.Count - 1 Then
                                clbRobotTo.SetItemChecked(i%, False)
                            Else
                                clbRobotTo.SetItemChecked(i%, clbRobotFrom.GetItemChecked(i%))
                            End If
                        Next

                    Else
                        'same number
                        For i% = 0 To (clbRobotTo.Items.Count - 1)
                            clbRobotTo.SetItemChecked(i%, clbRobotFrom.GetItemChecked(i%))
                        Next
                    End If '(clbRobotFrom.Items.Count > clbRobotTo.Items.Count)
                Else
                    'same number
                    For i% = 0 To (clbRobotFrom.Items.Count - 1)
                        If i% <> clbRobotFrom.SelectedIndex Then
                            clbRobotFrom.SetItemChecked(i%, False)
                        End If
                    Next
                End If
            End If 'clbRobotFrom.CheckedItems.Count > 1

        Else
            'to box

            If clbRobotFrom.CheckedItems.Count > 1 Then
                'need to make to robots match, indexwise anyway match frombox 
                'with selections just made in to box
                If (clbRobotFrom.Items.Count > clbRobotTo.Items.Count) Then
                    'more from than to
                    For i% = 0 To (clbRobotFrom.Items.Count - 1)
                        If i% > clbRobotTo.Items.Count - 1 Then
                            clbRobotFrom.SetItemChecked(i%, False)
                        Else
                            clbRobotFrom.SetItemChecked(i%, clbRobotTo.GetItemChecked(i%))
                        End If
                    Next

                ElseIf (clbRobotFrom.Items.Count < clbRobotTo.Items.Count) Then
                    'more to than from
                    For i% = 0 To (clbRobotTo.Items.Count - 1)
                        If i% > clbRobotFrom.Items.Count - 1 Then
                            clbRobotTo.SetItemChecked(i%, False)
                        Else
                            clbRobotFrom.SetItemChecked(i%, clbRobotTo.GetItemChecked(i%))
                        End If
                    Next

                Else
                    'same number
                    For i% = 0 To (clbRobotTo.Items.Count - 1)
                        clbRobotFrom.SetItemChecked(i%, clbRobotTo.GetItemChecked(i%))
                    Next
                End If '(clbRobotFrom.Items.Count > clbRobotTo.Items.Count)
            End If 'clbRobotFrom.CheckedItems.Count > 1

        End If 'bFromBox

        If bFromSelectionComplete() Then
            bCopyParameterSelectionComplete()
        End If

    End Sub
    Private Sub subParamChangeHandler(ByVal bFromBox As Boolean)
        '********************************************************************************************
        'Description:  enable to frame?
        '                
        '
        'SubParameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/15/07  gks     redo - all this forcing stuff was messing up PSC
        '********************************************************************************************
        Dim i%

        If clbParamFrom.Items.Count = 0 Then Exit Sub
        If clbParamTo.Items.Count = 0 Then Exit Sub

        If bFromBox Then

            If clbParamFrom.CheckedItems.Count > 1 Then

                'need to make to robots match, indexwise anyway
                If (clbParamFrom.Items.Count > clbParamTo.Items.Count) Then
                    'more from than to
                    For i% = 0 To (clbParamFrom.Items.Count - 1)
                        If i% > clbParamTo.Items.Count - 1 Then
                            clbParamFrom.SetItemChecked(i%, False)
                        Else
                            clbParamTo.SetItemChecked(i%, clbParamFrom.GetItemChecked(i%))
                        End If
                    Next

                ElseIf (clbParamFrom.Items.Count < clbParamTo.Items.Count) Then
                    'more to than from
                    For i% = 0 To (clbParamTo.Items.Count - 1)
                        If i% > clbParamFrom.Items.Count - 1 Then
                            clbParamTo.SetItemChecked(i%, False)
                        Else
                            clbParamTo.SetItemChecked(i%, clbParamFrom.GetItemChecked(i%))
                        End If
                    Next

                Else
                    'same number
                    For i% = 0 To (clbParamTo.Items.Count - 1)
                        clbParamTo.SetItemChecked(i%, clbParamFrom.GetItemChecked(i%))
                    Next
                End If '(clbParamFrom.Items.Count > clbParamTo.Items.Count)
            End If 'clbParamFrom.CheckedItems.Count > 1

        Else
            'to box

            If clbParamFrom.CheckedItems.Count > 1 Then
                'need to make to robots match, indexwise anyway match frombox 
                'with selections just made in to box
                If (clbParamFrom.Items.Count > clbParamTo.Items.Count) Then
                    'more from than to
                    For i% = 0 To (clbParamFrom.Items.Count - 1)
                        If i% > clbParamTo.Items.Count - 1 Then
                            clbParamFrom.SetItemChecked(i%, False)
                        Else
                            clbParamFrom.SetItemChecked(i%, clbParamTo.GetItemChecked(i%))
                        End If
                    Next

                ElseIf (clbParamFrom.Items.Count < clbParamTo.Items.Count) Then
                    'more to than from
                    For i% = 0 To (clbParamTo.Items.Count - 1)
                        If i% > clbParamFrom.Items.Count - 1 Then
                            clbParamTo.SetItemChecked(i%, False)
                        Else
                            clbParamFrom.SetItemChecked(i%, clbParamTo.GetItemChecked(i%))
                        End If
                    Next

                Else
                    'same number
                    For i% = 0 To (clbParamTo.Items.Count - 1)
                        clbParamFrom.SetItemChecked(i%, clbParamTo.GetItemChecked(i%))
                    Next
                End If '(clbParamFrom.Items.Count > clbParamTo.Items.Count)
            End If 'clbParamFrom.CheckedItems.Count > 1

        End If 'bFromBox


        If bFromSelectionComplete() Then
            bCopyParameterSelectionComplete()
        End If

    End Sub
    Private Sub subStyleChangeHandler(ByVal bFromBox As Boolean)
        '********************************************************************************************
        'Description:  enable to frame?
        '                
        '
        'SubStyleeters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/15/07  gks     redo - all this forcing stuff was messing up PSC
        '********************************************************************************************
        Dim i%

        If clbStyleFrom.Items.Count = 0 Then Exit Sub
        If clbStyleTo.Items.Count = 0 Then Exit Sub

        If bFromBox Then

            If clbStyleFrom.CheckedItems.Count > 1 Then

                'need to make to robots match, indexwise anyway
                If (clbStyleFrom.Items.Count > clbStyleTo.Items.Count) Then
                    'more from than to
                    For i% = 0 To (clbStyleFrom.Items.Count - 1)
                        If i% > clbStyleTo.Items.Count - 1 Then
                            clbStyleFrom.SetItemChecked(i%, False)
                        Else
                            clbStyleTo.SetItemChecked(i%, clbStyleFrom.GetItemChecked(i%))
                        End If
                    Next

                ElseIf (clbStyleFrom.Items.Count < clbStyleTo.Items.Count) Then
                    'more to than from
                    For i% = 0 To (clbStyleTo.Items.Count - 1)
                        If i% > clbStyleFrom.Items.Count - 1 Then
                            clbStyleTo.SetItemChecked(i%, False)
                        Else
                            clbStyleTo.SetItemChecked(i%, clbStyleFrom.GetItemChecked(i%))
                        End If
                    Next

                Else
                    'same number
                    For i% = 0 To (clbStyleTo.Items.Count - 1)
                        clbStyleTo.SetItemChecked(i%, clbStyleFrom.GetItemChecked(i%))
                    Next
                End If '(clbStyleFrom.Items.Count > clbStyleTo.Items.Count)
            End If 'clbStyleFrom.CheckedItems.Count > 1

        Else
            'to box

            If clbStyleFrom.CheckedItems.Count > 1 Then
                'need to make to robots match, indexwise anyway match frombox 
                'with selections just made in to box
                If (clbStyleFrom.Items.Count > clbStyleTo.Items.Count) Then
                    'more from than to
                    For i% = 0 To (clbStyleFrom.Items.Count - 1)
                        If i% > clbStyleTo.Items.Count - 1 Then
                            clbStyleFrom.SetItemChecked(i%, False)
                        Else
                            clbStyleFrom.SetItemChecked(i%, clbStyleTo.GetItemChecked(i%))
                        End If
                    Next

                ElseIf (clbStyleFrom.Items.Count < clbStyleTo.Items.Count) Then
                    'more to than from
                    For i% = 0 To (clbStyleTo.Items.Count - 1)
                        If i% > clbStyleFrom.Items.Count - 1 Then
                            clbStyleTo.SetItemChecked(i%, False)
                        Else
                            clbStyleFrom.SetItemChecked(i%, clbStyleTo.GetItemChecked(i%))
                        End If
                    Next

                Else
                    'same number
                    For i% = 0 To (clbStyleTo.Items.Count - 1)
                        clbStyleFrom.SetItemChecked(i%, clbStyleTo.GetItemChecked(i%))
                    Next
                End If '(clbStyleFrom.Items.Count > clbStyleTo.Items.Count)
            End If 'clbStyleFrom.CheckedItems.Count > 1

        End If 'bFromBox


        If bFromSelectionComplete() Then
            bCopyParameterSelectionComplete()
        End If

    End Sub
    Private Sub subSubParamChangeHandler(ByVal bFromBox As Boolean)
        '********************************************************************************************
        'Description:  enable to frame?
        '                
        '
        'SubParameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/15/07  gks     redo - all this forcing stuff was messing up PSC
        '********************************************************************************************
        Dim i%

        If clbSubParamFrom.Items.Count = 0 Then Exit Sub
        If clbSubParamTo.Items.Count = 0 Then Exit Sub

        If bFromBox Then
            If mbSubParamToFromMustMatch Then
                'Keep them in sync
                For i% = 0 To (clbSubParamTo.Items.Count - 1)
                    clbSubParamTo.SetItemChecked(i%, clbSubParamFrom.GetItemChecked(i%))
                Next
            Else    'mbSubParamToFromMustMatch
                If clbSubParamFrom.CheckedItems.Count > 1 Then

                    'need to make to robots match, indexwise anyway
                    If (clbSubParamFrom.Items.Count > clbSubParamTo.Items.Count) Then
                        'more from than to
                        For i% = 0 To (clbSubParamFrom.Items.Count - 1)
                            If i% > clbSubParamTo.Items.Count - 1 Then
                                clbSubParamFrom.SetItemChecked(i%, False)
                            Else
                                clbSubParamTo.SetItemChecked(i%, clbSubParamFrom.GetItemChecked(i%))
                            End If
                        Next

                    ElseIf (clbSubParamFrom.Items.Count < clbSubParamTo.Items.Count) Then
                        'more to than from
                        For i% = 0 To (clbSubParamTo.Items.Count - 1)
                            If i% > clbSubParamFrom.Items.Count - 1 Then
                                clbSubParamTo.SetItemChecked(i%, False)
                            Else
                                clbSubParamTo.SetItemChecked(i%, clbSubParamFrom.GetItemChecked(i%))
                            End If
                        Next

                    Else
                        'same number
                        For i% = 0 To (clbSubParamTo.Items.Count - 1)
                            clbSubParamTo.SetItemChecked(i%, clbSubParamFrom.GetItemChecked(i%))
                        Next
                    End If '(clbSubParamFrom.Items.Count > clbSubParamTo.Items.Count)
                End If 'clbSubParamFrom.CheckedItems.Count > 1
            End If    'mbSubParamToFromMustMatch
        Else
            'to box
            If mbSubParamToFromMustMatch Then
                'Keep them in sync
                For i% = 0 To (clbSubParamTo.Items.Count - 1)
                    clbSubParamFrom.SetItemChecked(i%, clbSubParamTo.GetItemChecked(i%))
                Next
            Else    'mbSubParamToFromMustMatch

                If clbSubParamFrom.CheckedItems.Count > 1 Then
                    'need to make to robots match, indexwise anyway match frombox 
                    'with selections just made in to box
                    If (clbSubParamFrom.Items.Count > clbSubParamTo.Items.Count) Then
                        'more from than to
                        For i% = 0 To (clbSubParamFrom.Items.Count - 1)
                            If i% > clbSubParamTo.Items.Count - 1 Then
                                clbSubParamFrom.SetItemChecked(i%, False)
                            Else
                                clbSubParamFrom.SetItemChecked(i%, clbSubParamTo.GetItemChecked(i%))
                            End If
                        Next

                    ElseIf (clbSubParamFrom.Items.Count < clbSubParamTo.Items.Count) Then
                        'more to than from
                        For i% = 0 To (clbSubParamTo.Items.Count - 1)
                            If i% > clbSubParamFrom.Items.Count - 1 Then
                                clbSubParamTo.SetItemChecked(i%, False)
                            Else
                                clbSubParamFrom.SetItemChecked(i%, clbSubParamTo.GetItemChecked(i%))
                            End If
                        Next

                    Else
                        'same number
                        For i% = 0 To (clbSubParamTo.Items.Count - 1)
                            clbSubParamFrom.SetItemChecked(i%, clbSubParamTo.GetItemChecked(i%))
                        Next
                    End If '(clbSubParamFrom.Items.Count > clbSubParamTo.Items.Count)
                End If 'clbSubParamFrom.CheckedItems.Count > 1
            End If   'mbSubParamToFromMustMatch

        End If 'bFromBox

        If bFromSelectionComplete() Then
            bCopyParameterSelectionComplete()
        End If

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
        Dim lReply As System.Windows.Forms.DialogResult = Response.OK

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

        Status = gcsRM.GetString("csINITALIZING")
        Progress = 1

        Try
            DataLoaded = False

            Progress = 5

            colZonesFrom = New clsZones(DatabasePath)
            colZonesTo = New clsZones(DatabasePath)
            mScreenSetup.LoadZoneBox(cboZoneFrom, colZonesFrom, False)
            mScreenSetup.LoadZoneBox(clbZoneTo, colZonesTo, False)

            mScreenSetup.InitializeForm(Me)

            subInitFormText()

            Progress = 98


            ' this is to make form appear under the pw3 banner 
            'if the maximize button is clicked
            ' can't go in screensetup because its protected
            Me.MaximizedBounds = mScreenSetup.MaxBound

            ' build the context menus
            Dim void As System.Drawing.Image = Nothing

            Dim mnuRobotFrom As New ContextMenuStrip
            With mnuRobotFrom
                .Name = "clbRobotFrom"
                .Items.Add(gcsRM.GetString("csSELECT_ALL"), void, AddressOf mnuAll_Click)
                .Items.Add(gcsRM.GetString("csSELECT_NONE"), void, AddressOf mnuAll_Click)
                .Items(0).Name = "mnuAll"
            End With
            clbRobotFrom.ContextMenuStrip = mnuRobotFrom

            Dim mnuRobotTo As New ContextMenuStrip
            With mnuRobotTo
                .Name = "clbRobotTo"
                .Items.Add(gcsRM.GetString("csSELECT_ALL"), void, AddressOf mnuAll_Click)
                .Items.Add(gcsRM.GetString("csSELECT_NONE"), void, AddressOf mnuAll_Click)
                .Items(0).Name = "mnuAll"
            End With
            clbRobotTo.ContextMenuStrip = mnuRobotTo

            Dim mnuParamFrom As New ContextMenuStrip
            With mnuParamFrom
                .Name = "clbParamFrom"
                .Items.Add(gcsRM.GetString("csSELECT_ALL"), void, AddressOf mnuAll_Click)
                .Items.Add(gcsRM.GetString("csSELECT_NONE"), void, AddressOf mnuAll_Click)
                .Items(0).Name = "mnuAll"
            End With
            clbParamFrom.ContextMenuStrip = mnuParamFrom

            Dim mnuParamTo As New ContextMenuStrip
            With mnuParamTo
                .Name = "clbParamTo"
                .Items.Add(gcsRM.GetString("csSELECT_ALL"), void, AddressOf mnuAll_Click)
                .Items.Add(gcsRM.GetString("csSELECT_NONE"), void, AddressOf mnuAll_Click)
                .Items(0).Name = "mnuAll"
            End With
            clbParamTo.ContextMenuStrip = mnuParamTo

            Dim mnuStyleFrom As New ContextMenuStrip
            With mnuStyleFrom
                .Name = "clbStyleFrom"
                .Items.Add(gcsRM.GetString("csSELECT_ALL"), void, AddressOf mnuAll_Click)
                .Items.Add(gcsRM.GetString("csSELECT_NONE"), void, AddressOf mnuAll_Click)
                .Items(0).Name = "mnuAll"
            End With
            clbStyleFrom.ContextMenuStrip = mnuStyleFrom

            Dim mnuStyleTo As New ContextMenuStrip
            With mnuStyleTo
                .Name = "clbStyleTo"
                .Items.Add(gcsRM.GetString("csSELECT_ALL"), void, AddressOf mnuAll_Click)
                .Items.Add(gcsRM.GetString("csSELECT_NONE"), void, AddressOf mnuAll_Click)
                .Items(0).Name = "mnuAll"
            End With
            clbStyleTo.ContextMenuStrip = mnuStyleTo

            Dim mnuSubParamFrom As New ContextMenuStrip
            With mnuSubParamFrom
                .Name = "clbSubParamFrom"
                .Items.Add(gcsRM.GetString("csSELECT_ALL"), void, AddressOf mnuAll_Click)
                .Items.Add(gcsRM.GetString("csSELECT_NONE"), void, AddressOf mnuAll_Click)
                .Items(0).Name = "mnuAll"
            End With
            clbSubParamFrom.ContextMenuStrip = mnuSubParamFrom

            Dim mnuSubParamTo As New ContextMenuStrip
            With mnuSubParamTo
                .Name = "clbSubParamTo"
                .Items.Add(gcsRM.GetString("csSELECT_ALL"), void, AddressOf mnuAll_Click)
                .Items.Add(gcsRM.GetString("csSELECT_NONE"), void, AddressOf mnuAll_Click)
                .Items(0).Name = "mnuAll"
            End With
            clbSubParamTo.ContextMenuStrip = mnuSubParamTo

            Dim mnuZoneTo As New ContextMenuStrip
            With mnuZoneTo
                .Name = "clbZoneTo"
                .Items.Add(gcsRM.GetString("csSELECT_ALL"), void, AddressOf mnuAll_Click)
                .Items.Add(gcsRM.GetString("csSELECT_NONE"), void, AddressOf mnuAll_Click)
                .Items(0).Name = "mnuAll"
            End With
            clbZoneTo.ContextMenuStrip = mnuZoneTo

            Status = gcsRM.GetString("csSELECT_SOURCE")

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
            Progress = 100
            Me.Cursor = System.Windows.Forms.Cursors.Default
            subEnableControls(True)
        End Try
        If cboZoneFrom.Items.Count = 1 Then
            cboZoneFrom.Text = cboZoneFrom.Items(0).ToString
        End If
        If clbZoneTo.Items.Count = 1 Then
            clbZoneTo.SetItemChecked(0, True)
        End If
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
        ' 01/01/13  RJO     Added Style Parameter to mPresetCommon.LoadCopyScreenSubParamBox calls.
        ' 04/02/13  DE      Added localization for lblZone and lblRobot text
        '********************************************************************************************

        lblZone.Text = gcsRM.GetString("csZONE_CAP") 'DE 04/02/13
        lblZoneTo.Text = lblZone.Text
        lblRobot.Text = gcsRM.GetString("csROBOT") 'DE 04/02/13
        LblRobotTo.Text = lblRobot.Text
        lblViewFrom.Text = gcsRM.GetString("csVIEWING")
        lblViewTo.Text = gcsRM.GetString("csVIEWING")
        lblViewDevFrom.Text = String.Empty
        lblViewDevTo.Text = String.Empty
        lblViewParamFrom.Text = String.Empty
        lblViewParamTo.Text = String.Empty
        lblViewSubParamFrom.Text = String.Empty
        lblViewSubParamTo.Text = String.Empty
        lblViewStyleFrom.Text = String.Empty
        lblViewStyleTo.Text = String.Empty
        With gpsRM
            ' this is after initializeform to overwrite version number
            If msFormCaption <> String.Empty Then
                Me.Text = msFormCaption
            Else
                Me.Text = .GetString("psPRESET_COPY")
            End If
            If msSubParamName = "" Then
                lblSubParam.Text = .GetString("psPRESET")
                lblSubParamTo.Text = lblSubParam.Text
            Else
                lblSubParam.Text = msSubParamName
                lblSubParamTo.Text = msSubParamName
            End If
            If mbUseRobot = False Then
                If (mbLoadSubParamByRobot = False) Then
                    '>>>>>> This is a generic call - routine needs to be provided in screen specific module
                    LoadCopyScreenSubParamBox(mArmFrom, clbSubParamFrom, lblViewSubParamFrom.Text, 0) 'RJO 01/01/13
                    '>>>>>> This is a generic call - routine needs to be provided in screen specific module
                    LoadCopyScreenSubParamBox(mArmTo, clbSubParamTo, lblViewSubParamTo.Text, 0) 'RJO 01/01/13
                End If
            End If
            If mbUseStyle Then
                lblStyle.Text = .GetString("psSTYLE_CAP")
                lblstyleTo.Text = .GetString("psSTYLE_CAP")
                clbStyleFrom.Items.Clear()
                clbStyleTo.Items.Clear()
                Dim oStyles As New clsSysStyles(colZonesFrom.ActiveZone)
                oStyles.LoadStyleBoxFromCollection(clbStyleFrom, False, False, True)
                oStyles.LoadStyleBoxFromCollection(clbStyleTo, False, False, True)
            End If
            'other screen specific
            If msSubParamName <> String.Empty Then
                lblParam.Text = msParamName
                lblParamTo.Text = msParamName
            Else
                lblParam.Text = gcsRM.GetString("csCOLOR_CAP")
                lblParamTo.Text = lblParam.Text
            End If
            gpbFrom.Text = .GetString("psFROM")
            gpbTo.Text = .GetString("psTO")

            btnCopyV.Text = .GetString("psCOPY_VAL")
            btnCopyD.Text = .GetString("psCOPY_DESC")
            If mbFancyCopyButtons Then
                btnCopy.Text = .GetString("psCOPY_ALL")
                btnCopyV.Text = .GetString("psCOPY_VAL")
                btnCopyD.Text = .GetString("psCOPY_DESC")
                btnCopyV.Visible = True
                btnCopyD.Visible = True
            Else
                btnCopy.Text = .GetString("psCOPY")
                btnCopyV.Visible = False
                btnCopyD.Visible = False
            End If


        End With
    End Sub
    Private Function bAllBoxesHaveSelections() As Boolean
        '********************************************************************************************
        'Description: do we have something checked in every box??
        '
        'Parameters: none
        'Returns:    True if Selections 
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If cboZoneFrom.Text = String.Empty Then Return False
        If clbZoneTo.CheckedItems.Count = 0 Then Return False
        If mbUseRobot Then
            If clbRobotFrom.CheckedItems.Count = 0 Then Return False
            If clbRobotTo.CheckedItems.Count = 0 Then Return False
        End If
        If mbUseParam Then
            If clbParamFrom.CheckedItems.Count = 0 Then Return False
            If clbParamTo.CheckedItems.Count = 0 Then Return False
        End If
        If mbUseSubParam Then
            If clbSubParamFrom.CheckedItems.Count = 0 Then Return False
            If clbSubParamTo.CheckedItems.Count = 0 Then Return False
        End If
        If mbUseStyle Then
            If clbStyleFrom.CheckedItems.Count = 0 Then Return False
            If clbStyleTo.CheckedItems.Count = 0 Then Return False
        End If
        Return True

    End Function
    Private Function bCopyParameterSelectionComplete() As Boolean
        '********************************************************************************************
        'Description: do we have enough selections to enable the copy buttons??
        '
        'Parameters: none
        'Returns:    True if Selections OK
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim bButtons As Boolean = True



        If cboZoneFrom.Text = String.Empty Then bButtons = False
        If clbZoneTo.CheckedItems.Count = 0 Then bButtons = False
        If mbUseRobot Then
            If clbRobotFrom.CheckedItems.Count = 0 Then bButtons = False
            If clbRobotTo.CheckedItems.Count = 0 Then bButtons = False
        End If
        If mbNotWarnedYet Then
            If bAllApplicatorsTheSame() = False Then
                'if we want to make this more than a warning we could do it here
                mbNotWarnedYet = False
            End If
        End If
        If mbUseParam Then
            If clbParamFrom.CheckedItems.Count = 0 Then bButtons = False
            If clbParamTo.CheckedItems.Count = 0 Then bButtons = False
        End If

        If mbUseSubParam Then
            If clbSubParamFrom.CheckedItems.Count = 0 Then bButtons = False
            If clbSubParamTo.CheckedItems.Count = 0 Then bButtons = False
        End If
        If mbUseStyle Then
            If clbStyleFrom.CheckedItems.Count = 0 Then bButtons = False
            If clbStyleTo.CheckedItems.Count = 0 Then bButtons = False
        End If
        If bAllBoxesHaveSelections() Then
            ' do final checks
            If mbUseRobot Then
                If bCheckRobotSelection() = False Then bButtons = False
            End If
            If mbUseParam Then
                If bCheckParameterSelection() = False Then bButtons = False
            End If
            If mbUseStyle Then
                If bCheckStyleSelection() = False Then bButtons = False
            End If
            If mbUseSubParam Then
                If bCheckSubParameterSelection() = False Then bButtons = False
            End If
        Else
            bButtons = False
        End If

        btnCopy.Enabled = bButtons
        btnCopyD.Enabled = bButtons
        btnCopyV.Enabled = bButtons

        Return bButtons

    End Function
    Private Sub subChangeParameter(ByVal bFromBox As Boolean, ByVal ColorName As String)
        '********************************************************************************************
        'Description:  New Parameter selected
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/01/13  RJO     Added Style Parameter to mPresetCommon.LoadCopyScreenSubParamBox calls.
        '********************************************************************************************


        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            Dim nStyle As Integer 'RJO 01/01/13

            Progress = 50

            If bFromBox Then
                If (mbLoadSubParamByRobot = False) Then
                    '>>>>>> This is a generic call - routine needs to be provided in screen specific module
                    Dim nCheckedItems As Integer() = nGetCheckedItems(clbSubParamFrom) 'RJO 01/08/13

                    nStyle = nGetStyleSelection(clbStyleFrom) 'RJO 01/01/13
                    LoadCopyScreenSubParamBox(mArmFrom, clbSubParamFrom, ColorName, nStyle) 'RJO 01/01/13
                    If nCheckedItems(0) > -1 Then Call subCheckItems(clbSubParamFrom, nCheckedItems) 'RJO 01/08/13
                    lblViewSubParamFrom.Text = ColorName
                End If

            Else
                '>>>>>> This is a generic call - routine needs to be provided in screen specific module
                If (mbLoadSubParamByRobot = False) Then
                    Dim nCheckedItems As Integer() = nGetCheckedItems(clbSubParamTo) 'RJO 01/08/13

                    nStyle = nGetStyleSelection(clbStyleTo) 'RJO 01/01/13
                    LoadCopyScreenSubParamBox(mArmTo, clbSubParamTo, ColorName, nStyle) 'RJO 01/01/13
                    If nCheckedItems(0) > -1 Then Call subCheckItems(clbSubParamTo, nCheckedItems) 'RJO 01/08/13
                    lblViewSubParamTo.Text = ColorName
                End If

                If clbSubParamFrom.CheckedItems.Count > 1 Or mbSubParamToFromMustMatch Then
                    'select the same in to box
                    Dim i%
                    For i% = 0 To clbSubParamFrom.Items.Count - 1
                        If i% = clbSubParamTo.Items.Count Then Exit For
                        clbSubParamTo.SetItemChecked(i%, clbSubParamFrom.GetItemChecked(i%))
                    Next

                End If ' clbsubParamFrom.CheckedItems.Count > 1 

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
    Private Sub subChangeStyle(ByVal bFromBox As Boolean, ByVal ColorName As String)
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


        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

            Progress = 50

            If clbSubParamFrom.CheckedItems.Count > 1 Or mbSubParamToFromMustMatch Then
                'select the same in to box
                Dim i%
                For i% = 0 To clbStyleFrom.Items.Count - 1
                    If i% = clbStyleTo.Items.Count Then Exit For
                    clbStyleTo.SetItemChecked(i%, clbStyleFrom.GetItemChecked(i%))
                Next

            End If ' clbstyleFrom.CheckedItems.Count > 1 
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
    Private Sub subChangeRobot(ByVal bFromBox As Boolean, ByVal RobotName As String)
        '********************************************************************************************
        'Description:  New robot selected
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/01/13  RJO     Added Style Parameter to mPresetCommon.LoadCopyScreenSubParamBox calls.
        '********************************************************************************************
        Dim sStyleInfo() As String = Nothing

        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            Progress = 50


            If bFromBox Then
                If ByArm Then
                    mArmFrom = colArmsFrom(RobotName)
                Else
                    mArmFrom = colControllersFrom(RobotName).Arms(0)
                End If
                lblViewParamFrom.Text = String.Empty
                lblViewSubParamFrom.Text = String.Empty

                If mbUseParam Then
                    mArmFrom.SystemColors.Load(mArmFrom)
                    Select Case mnParamType
                        Case eParamType.Colors
                            mSysColorCommon.LoadColorBoxFromCollection(mArmFrom.SystemColors, clbParamFrom)
                        Case eParamType.Valves
                            mSysColorCommon.LoadValveBoxFromCollection(mArmFrom.SystemColors, clbParamFrom)
                    End Select
                    lblViewParamFrom.Text = mArmFrom.Name
                End If

                If mbUseSubParam And ((mbUseParam = False) Or (mbLoadSubParamByRobot)) Then
                    '>>>>>> This is a generic call - routine needs to be provided in screen specific module
                    Dim sColorName As String = String.Empty
                    Dim nStyle As Integer = nGetStyleSelection(clbStyleFrom) 'RJO 01/01/13
                    Dim nCheckedItems As Integer() = nGetCheckedItems(clbSubParamFrom) 'RJO 01/08/13

                    LoadCopyScreenSubParamBox(mArmFrom, clbSubParamFrom, sColorName, nStyle) 'RJO 01/01/13
                    If nCheckedItems(0) > -1 Then Call subCheckItems(clbSubParamFrom, nCheckedItems) 'RJO 01/08/13
                    lblViewSubParamFrom.Text = mArmFrom.Name
                End If

            Else
                If ByArm Then
                    mArmTo = colArmsTo(RobotName)
                Else
                    mArmTo = colControllersTo(RobotName).Arms(0)
                End If
                lblViewParamTo.Text = String.Empty
                lblViewSubParamTo.Text = String.Empty

                If mbUseParam Then
                    mArmTo.SystemColors.Load(mArmTo)
                    Select Case mnParamType
                        Case eParamType.Colors
                            mSysColorCommon.LoadColorBoxFromCollection(mArmTo.SystemColors, clbParamTo)
                        Case eParamType.Valves
                            mSysColorCommon.LoadValveBoxFromCollection(mArmTo.SystemColors, clbParamTo)
                    End Select
                    lblViewParamTo.Text = mArmTo.Name

                    If clbParamFrom.CheckedItems.Count > 1 Then
                        'select the same in to box
                        Dim i%
                        For i% = 0 To clbParamFrom.Items.Count - 1
                            If i% = clbParamTo.Items.Count Then Exit For
                            clbParamTo.SetItemChecked(i%, clbParamFrom.GetItemChecked(i%))
                        Next

                    End If ' clbParamFrom.CheckedItems.Count > 1 
                End If

                If mbUseSubParam And ((mbUseParam = False) Or (mbLoadSubParamByRobot)) Then
                    '>>>>>> This is a generic call - routine needs to be provided in screen specific module
                    Dim sColorName As String = String.Empty
                    Dim nStyle As Integer = nGetStyleSelection(clbStyleTo) 'RJO 01/01/13
                    Dim nCheckedItems As Integer() = nGetCheckedItems(clbSubParamTo) 'RJO 01/08/13

                    LoadCopyScreenSubParamBox(mArmTo, clbSubParamTo, sColorName, nStyle) 'RJO 01/01/13
                    If nCheckedItems(0) > -1 Then Call subCheckItems(clbSubParamTo, nCheckedItems) 'RJO 01/08/13
                    lblViewSubParamTo.Text = mArmTo.Name


                    If clbSubParamFrom.CheckedItems.Count > 1 Then
                        'select the same in to box
                        Dim i%
                        For i% = 0 To clbSubParamFrom.Items.Count - 1
                            If i% = clbSubParamTo.Items.Count Then Exit For
                            clbSubParamTo.SetItemChecked(i%, clbSubParamFrom.GetItemChecked(i%))
                        Next

                    End If ' clbsubParamFrom.CheckedItems.Count > 1 
                End If
            End If



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

    Private Function nGetCheckedItems(ByRef clbList As CheckedListBox) As Integer()
        '********************************************************************************************
        'Description:  
        '
        'Parameters: CheckedListBox
        'Returns:    Array of Integer values
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/08/13  RJO     Added 
        '********************************************************************************************
        Dim nCheckedItems(0) As Integer
        nCheckedItems(0) = -1

        If clbList.Items.Count > 0 Then
            Dim nUbound As Integer = clbList.Items.Count - 1
            Dim nItem As Integer = -1

            For nIndex As Integer = 0 To nUbound
                If clbList.GetItemChecked(nIndex) Then
                    nItem += 1
                    ReDim Preserve nCheckedItems(nItem)

                    nCheckedItems(nItem) = nIndex
                End If
            Next
        End If

        Return nCheckedItems

    End Function

    Private Sub subCheckItems(ByRef clbList As CheckedListBox, ByVal Items() As Integer)
        '********************************************************************************************
        'Description:  
        '
        'Parameters: CheckedListBox, Array of Integer values
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/08/13  RJO     Added 
        '********************************************************************************************

        For Each nIndex As Integer In Items
            clbList.SetItemChecked(nIndex, True)
        Next

    End Sub

    Private Function nGetStyleSelection(ByRef clbStyle As CheckedListBox) As Integer
        '********************************************************************************************
        'Description:  If UseStyle = True, when a single style is selected returns the Style number.
        '              If UseStyle = False, return 1.
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/01/13  RJO     Added to determine style for mPresetCommon.LoadCopyScreenSubParamBox
        '********************************************************************************************

        If UseStyle Then
            Dim sTag() As String = DirectCast(clbStyle.Tag, String())
            Dim nStyle As Integer

            If clbStyle.CheckedIndices.Count = 1 Then
                For nItem As Integer = 0 To clbStyle.Items.Count - 1
                    If clbStyle.GetItemChecked(nItem) Then
                        nStyle = CType(sTag(nItem), Integer)
                        Exit For
                    End If
                Next
            Else
                nStyle = 0
            End If

            Return nStyle
        Else
            Return 1
        End If

    End Function
#End Region
#Region " Events "

    Private Sub cboZoneFrom_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                Handles cboZoneFrom.SelectedIndexChanged
        '********************************************************************************************
        'Description:  Zone Combo Changed 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '06/19/07   gks     zoneto is now a checkedlistbox
        '********************************************************************************************
        Dim o As ComboBox = DirectCast(sender, ComboBox)

        If o.Text = String.Empty Then Exit Sub

        If (o.Text <> colZonesFrom.CurrentZone) Or (clbRobotFrom.Items.Count = 0) Then
            colZonesFrom.CurrentZone = o.Text
            subChangeZone(True)
            bFromSelectionComplete()
        End If

    End Sub
    Private Sub clbZoneTo_ItemCheck(ByVal sender As Object, _
                    ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles clbZoneTo.ItemCheck
        '********************************************************************************************
        'Description:  Zone Combo Changed 
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '06/19/07   gks     to is now a checkedlistbox
        '********************************************************************************************
        Dim o As CheckedListBox = DirectCast(sender, CheckedListBox)
        Dim nChecked As Integer = o.CheckedItems.Count

        Select Case e.NewValue
            Case CheckState.Checked
                nChecked += 1
            Case CheckState.Indeterminate
                Exit Sub
            Case CheckState.Unchecked
                nChecked -= 1
        End Select


        Select Case nChecked
            Case 0
                clbRobotTo.Items.Clear()
                clbParamTo.Items.Clear()
                clbSubParamTo.Items.Clear()
                lblViewDevFrom.Text = cboZoneFrom.Text
                lblViewParamFrom.Text = String.Empty
                lblViewSubParamFrom.Text = String.Empty
                lblViewStyleFrom.Text = String.Empty

                Exit Sub
            Case 1
                clbRobotTo.Items.Clear()
                clbParamTo.Items.Clear()
                clbSubParamTo.Items.Clear()

                If clbZoneTo.CheckedItems.Count = 0 Then
                    'first one checked
                    If colZonesTo.SetCurrentZone(o.Items(e.Index).ToString) = False Then
                        e.NewValue = CheckState.Unchecked
                        Exit Select
                    End If

                Else
                    'down to just 1 - Which one?
                    Dim sTmp As String = clbZoneTo.GetItemText(clbZoneTo.Items(e.Index))
                    Dim sZone As String
                    ' get other one
                    If sTmp = clbZoneTo.CheckedItems(0).ToString Then
                        sZone = clbZoneTo.CheckedItems(1).ToString
                    Else
                        sZone = clbZoneTo.CheckedItems(0).ToString
                    End If

                    If colZonesTo.SetCurrentZone(sZone) = False Then
                        e.NewValue = CheckState.Unchecked
                        'the last one checked is not available
                        For i As Integer = 0 To clbZoneTo.Items.Count - 1
                            clbZoneTo.SetItemChecked(i, False)
                        Next
                        Exit Select
                    End If

                End If

                subChangeZone(False)

            Case Else
                'just display what's there and hope they're the same
                Dim sTmp As String = clbZoneTo.GetItemText(clbZoneTo.Items(e.Index))
                If colZonesTo.Item(sTmp).IsAvailable = False Then
                    e.NewValue = CheckState.Unchecked
                    MessageBox.Show(gcsRM.GetString("csSELECTED_ZONE_UNAVAILABLE"))
                End If

        End Select

    End Sub
    Private Sub clbRobot_ItemCheck(ByVal sender As Object, _
                                            ByVal e As System.Windows.Forms.ItemCheckEventArgs) _
                                            Handles clbRobotFrom.ItemCheck, clbRobotTo.ItemCheck
        '********************************************************************************************
        'Description:  robot got clicked - coming in here, checked count has not been incremented
        '               
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '5/21/07    gks     fixed selection bug
        '********************************************************************************************
        Dim o As CheckedListBox = DirectCast(sender, CheckedListBox)
        Dim nChecked As Integer = o.CheckedItems.Count

        Select Case e.NewValue
            Case CheckState.Checked
                nChecked += 1
            Case CheckState.Indeterminate
                Exit Sub
            Case CheckState.Unchecked
                nChecked -= 1
        End Select

        If o.Name = "clbRobotFrom" Then

            Select Case nChecked
                Case 0
                    clbParamFrom.Items.Clear()
                    clbSubParamFrom.Items.Clear()
                    lblViewDevFrom.Text = String.Empty
                    Exit Sub
                Case 1
                    clbParamFrom.Items.Clear()
                    clbSubParamFrom.Items.Clear()

                    If clbRobotFrom.CheckedItems.Count = 0 Then
                        'first one checked
                        subChangeRobot(True, o.Items(e.Index).ToString)
                    Else
                        'down to just 1 - Which one?
                        Dim sTmp As String = clbRobotFrom.GetItemText(clbRobotFrom.Items(e.Index))
                        ' get other one
                        If sTmp = clbRobotFrom.CheckedItems(0).ToString Then
                            subChangeRobot(True, clbRobotFrom.CheckedItems(1).ToString)
                        Else
                            subChangeRobot(True, clbRobotFrom.CheckedItems(0).ToString)
                        End If

                    End If

                Case Else
                    'just display what's there and hope the'yre the same

            End Select

        Else
            'to box
            Select Case nChecked
                Case 0
                    clbParamTo.Items.Clear()
                    clbSubParamTo.Items.Clear()
                    lblViewDevTo.Text = String.Empty
                    Exit Sub
                Case 1
                    clbParamTo.Items.Clear()
                    clbSubParamTo.Items.Clear()

                    If clbRobotTo.CheckedItems.Count = 0 Then
                        'first one checked
                        subChangeRobot(False, o.Items(e.Index).ToString)
                    Else
                        'down to just 1 - Which one?
                        Dim sTmp As String = clbRobotTo.GetItemText(clbRobotTo.Items(e.Index))
                        ' get other one
                        If sTmp = clbRobotTo.CheckedItems(0).ToString Then
                            subChangeRobot(False, clbRobotTo.CheckedItems(1).ToString)
                        Else
                            subChangeRobot(False, clbRobotTo.CheckedItems(0).ToString)
                        End If

                    End If

                Case Else
                    'just display what's there and hope they're the same

            End Select

        End If

    End Sub
    Private Sub clbParam_ItemCheck(ByVal sender As Object, _
                                    ByVal e As System.Windows.Forms.ItemCheckEventArgs) _
                                    Handles clbParamFrom.ItemCheck, clbParamTo.ItemCheck
        '********************************************************************************************
        'Description:  param got clicked - coming in here, checked count has not been incremented
        '                
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As CheckedListBox = DirectCast(sender, CheckedListBox)
        Dim nChecked As Integer = o.CheckedItems.Count

        Select Case e.NewValue
            Case CheckState.Checked
                nChecked += 1
            Case CheckState.Indeterminate
                Exit Sub
            Case CheckState.Unchecked
                nChecked -= 1
        End Select

        If o.Name = "clbParamFrom" Then

            Select Case nChecked
                Case 0
                    If Not (mbLoadSubParamByRobot) Then
                        clbSubParamFrom.Items.Clear()
                    End If

                    Exit Sub
                Case 1
                    If Not (mbLoadSubParamByRobot) Then
                        clbSubParamFrom.Items.Clear()
                    End If


                    If clbParamFrom.CheckedItems.Count = 0 Then
                        'first one checked
                        subChangeParameter(True, o.Items(e.Index).ToString)
                    Else
                        'down to just 1 - Which one?
                        Dim sTmp As String = clbParamFrom.GetItemText(clbParamFrom.Items(e.Index))
                        ' get other one
                        If sTmp = clbParamFrom.CheckedItems(0).ToString Then
                            subChangeParameter(True, o.CheckedItems(1).ToString)
                        Else
                            subChangeParameter(True, o.CheckedItems(0).ToString)
                        End If
                    End If

                Case Else
                    'just display what's there and hope the'yre the same


            End Select

        Else
            'to box
            Select Case nChecked
                Case 0
                    If Not (mbLoadSubParamByRobot) Then
                        clbSubParamTo.Items.Clear()
                    End If
                    Exit Sub
                Case 1
                    If Not (mbLoadSubParamByRobot) Then
                        clbSubParamTo.Items.Clear()
                    End If

                    If clbParamTo.CheckedItems.Count = 0 Then
                        'first one checked
                        subChangeParameter(False, o.Items(e.Index).ToString)
                    Else
                        'down to just 1 - Which one?
                        Dim sTmp As String = clbParamTo.GetItemText(clbParamTo.Items(e.Index))
                        ' get other one
                        If sTmp = clbParamTo.CheckedItems(0).ToString Then
                            subChangeParameter(False, o.CheckedItems(1).ToString)
                        Else
                            subChangeParameter(False, o.CheckedItems(0).ToString)
                        End If
                    End If

                Case Else
                    'just display what's there and hope they're the same


            End Select


        End If


    End Sub

    Private Sub clbStyle_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                                         Handles clbStyleFrom.SelectedIndexChanged, clbStyleTo.SelectedIndexChanged
        '********************************************************************************************
        'Description:  style got clicked - coming in here, checked count has not been incremented
        '                
        '
        'Styleeters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 01/01/13  RJO     Update Preset description clbs based on style selection.
        '********************************************************************************************
        Dim oClb As CheckedListBox = DirectCast(sender, CheckedListBox)
        Dim nChecked As Integer = oClb.CheckedItems.Count

        If oClb.Name = "clbStyleFrom" Then

            Select Case nChecked
                Case 0
                    'Clear preset listbox
                    clbSubParamFrom.Items.Clear()

                Case 1
                    'If single Robot and color selected, load preset listbox for selected style
                    Dim bRobotAndColorSelected As Boolean = clbRobotFrom.CheckedItems.Count > 0
                    Dim bSingleRobotAndColorSelected As Boolean = (clbRobotFrom.CheckedItems.Count = 1)

                    If bRobotAndColorSelected Then
                        bRobotAndColorSelected = clbParamFrom.CheckedItems.Count > 0
                    End If

                    If bSingleRobotAndColorSelected Then
                        bSingleRobotAndColorSelected = (clbParamFrom.CheckedItems.Count = 1)
                    End If

                    If bRobotAndColorSelected Then
                        Dim nCheckedItems As Integer() = nGetCheckedItems(clbSubParamFrom) 'RJO 01/08/13

                        If bSingleRobotAndColorSelected Then
                            Dim nStyle As Integer = nGetStyleSelection(clbStyleFrom)

                            LoadCopyScreenSubParamBox(mArmFrom, clbSubParamFrom, lblViewSubParamFrom.Text, nStyle)
                        Else
                            LoadCopyScreenSubParamBox(mArmFrom, clbSubParamFrom, lblViewSubParamFrom.Text, 0)
                        End If

                        If nCheckedItems(0) > -1 Then Call subCheckItems(clbSubParamFrom, nCheckedItems) 'RJO 01/08/13
                    End If


                Case Else
                    'If Robot and color selected, load preset listbox with generic list
                    Dim bRobotAndColorSelected As Boolean = clbRobotFrom.CheckedItems.Count > 0

                    If bRobotAndColorSelected Then
                        bRobotAndColorSelected = clbParamFrom.CheckedItems.Count > 0
                    End If

                    If bRobotAndColorSelected Then
                        Dim nCheckedItems As Integer() = nGetCheckedItems(clbSubParamFrom) 'RJO 01/08/13

                        LoadCopyScreenSubParamBox(mArmFrom, clbSubParamFrom, lblViewSubParamFrom.Text, 0)
                        If nCheckedItems(0) > -1 Then Call subCheckItems(clbSubParamFrom, nCheckedItems) 'RJO 01/08/13
                    End If


            End Select

        Else
            'to box
            Select Case nChecked
                Case 0
                    'Clear preset listbox
                    clbSubParamTo.Items.Clear()

                Case 1
                    'If single Robot and color selected, load preset listbox for selected style
                    Dim bRobotAndColorSelected As Boolean = clbRobotTo.CheckedItems.Count > 0
                    Dim bSingleRobotAndColorSelected As Boolean = (clbRobotTo.CheckedItems.Count = 1)

                    If bRobotAndColorSelected Then
                        bRobotAndColorSelected = clbParamTo.CheckedItems.Count > 0
                    End If

                    If bSingleRobotAndColorSelected Then
                        bSingleRobotAndColorSelected = (clbParamTo.CheckedItems.Count = 1)
                    End If

                    If bRobotAndColorSelected Then
                        Dim nCheckedItems As Integer() = nGetCheckedItems(clbSubParamTo) 'RJO 01/08/13

                        If bSingleRobotAndColorSelected Then
                            Dim nStyle As Integer = nGetStyleSelection(clbStyleTo)

                            LoadCopyScreenSubParamBox(mArmTo, clbSubParamTo, lblViewSubParamTo.Text, nStyle)
                        Else
                            LoadCopyScreenSubParamBox(mArmTo, clbSubParamTo, lblViewSubParamTo.Text, 0)
                        End If

                        If nCheckedItems(0) > -1 Then Call subCheckItems(clbSubParamTo, nCheckedItems) 'RJO 01/08/13
                    End If


                Case Else
                    'If Robot and color selected, load preset listbox with generic list
                    Dim bRobotAndColorSelected As Boolean = clbRobotTo.CheckedItems.Count > 0

                    If bRobotAndColorSelected Then
                        bRobotAndColorSelected = clbParamTo.CheckedItems.Count > 0
                    End If

                    If bRobotAndColorSelected Then
                        Dim nCheckedItems As Integer() = nGetCheckedItems(clbSubParamTo) 'RJO 01/08/13

                        LoadCopyScreenSubParamBox(mArmTo, clbSubParamTo, lblViewSubParamTo.Text, 0)
                        If nCheckedItems(0) > -1 Then Call subCheckItems(clbSubParamTo, nCheckedItems) 'RJO 01/08/13
                    End If

            End Select
        End If

    End Sub
    Private Sub clb_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) _
        Handles clbSubParamTo.KeyUp, clbParamTo.KeyUp, clbRobotTo.KeyUp, clbZoneTo.KeyUp, _
                clbSubParamFrom.KeyUp, clbParamFrom.KeyUp, clbRobotFrom.KeyUp, clbStyleFrom.KeyUp, clbStyleTo.KeyUp
        '********************************************************************************************
        'Description:  adjust selections as necessary in other boxes
        '                
        '
        'SubParameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As CheckedListBox = DirectCast(sender, CheckedListBox)


        Select Case o.Name
            Case "clbZoneTo"

            Case "clbRobotTo"
                subRobotChangeHandler(False)
            Case "clbParamTo"
                subParamChangeHandler(False)
            Case "clbSubParamTo"
                subSubParamChangeHandler(False)

            Case "clbRobotFrom"
                subRobotChangeHandler(True)
            Case "clbParamFrom"
                subParamChangeHandler(True)
            Case "clbSubParamFrom"
                subSubParamChangeHandler(True)

            Case "clbStyleTo"
                subStyleChangeHandler(False)
            Case "clbStyleFrom"
                subStyleChangeHandler(True)

        End Select

        bCopyParameterSelectionComplete()

    End Sub
    Private Sub clb_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
                Handles clbSubParamTo.MouseUp, clbParamTo.MouseUp, clbRobotTo.MouseUp, clbZoneTo.MouseUp, _
                        clbSubParamFrom.MouseUp, clbParamFrom.MouseUp, clbRobotFrom.MouseUp, clbStyleFrom.MouseUp, clbStyleTo.MouseUp
        '********************************************************************************************
        'Description:  adjust selections as necessary in other boxes
        '                
        '
        'SubParameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As CheckedListBox = DirectCast(sender, CheckedListBox)


        Select Case o.Name
            Case "clbZoneTo"

            Case "clbRobotTo"
                subRobotChangeHandler(False)
            Case "clbParamTo"
                subParamChangeHandler(False)
            Case "clbSubParamTo"
                subSubParamChangeHandler(False)

            Case "clbRobotFrom"
                subRobotChangeHandler(True)
            Case "clbParamFrom"
                subParamChangeHandler(True)
            Case "clbSubParamFrom"
                subSubParamChangeHandler(True)

            Case "clbStyleTo"
                subStyleChangeHandler(False)
            Case "clbStyleFrom"
                subStyleChangeHandler(True)

        End Select

        bCopyParameterSelectionComplete()


    End Sub
    Private Sub mnuAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                        Handles mnuAll.Click, mnuNotAll.Click
        '********************************************************************************************
        'Description:  Un/Select all context menu
        '                
        '
        'Parameters: none
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim o As System.Windows.Forms.ToolStripMenuItem = _
                            DirectCast(sender, System.Windows.Forms.ToolStripMenuItem)

        Dim oClb As CheckedListBox = DirectCast(GetControlByName(o.Owner.Name, Me), CheckedListBox)

        Dim bValue As Boolean = (o.Name = "mnuAll")

        If oClb.Items.Count = 0 Then Exit Sub

        Dim i%
        For i% = 0 To oClb.Items.Count - 1
            oClb.SetItemChecked(i%, bValue)
        Next

        Select Case oClb.Name
            Case "clbZoneTo"

            Case "clbRobotFrom"
                Call subRobotChangeHandler(True)
            Case "clbParamFrom"
                Call subParamChangeHandler(True)
            Case "clbSubParamFrom"
                Call subSubParamChangeHandler(True)
            Case "clbRobotTo"
                Call subRobotChangeHandler(False)
            Case "clbParamTo"
                Call subParamChangeHandler(False)
            Case "clbSubParamTo"
                Call subSubParamChangeHandler(False)

            Case "clbStyleTo"
                subStyleChangeHandler(False)
            Case "clbStyleFrom"
                subStyleChangeHandler(True)

        End Select

        bCopyParameterSelectionComplete()

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
        '07/04/10   RJO     Modified to present a less confusing message to the user if the Copy Values
        '                   button is clicked but no parameters have been selected from the drop menu.
        '                   CommonStrings.resx was also modified for this purpose.
        ' 01/09/12  MSW     Manage enable buttons during copy
        ' 04/30/12  RJO     Added confimation messages to Copy All and Copy Descriptions buttons.
        '********************************************************************************************

        Try
            Select Case e.ClickedItem.Name
                Case "btnClose"
                    Me.Close()
                Case "btnStatus"
                    lstStatus.Visible = Not lstStatus.Visible
                Case "btnCopy"
                    If bAllApplicatorsTheSame(True) = False Then
                        Exit Sub
                    End If
                    'btnClose.Enabled = False
                    If mbEnableAccessibilityFeatures Then
                        Dim sMsg As String = String.Empty

                        If (ScreenName = "Presets") Then
                            For nItem As Integer = 0 To moParamDetails.GetUpperBound(0)
                                sMsg = sMsg & moParamDetails(nItem).sLblName & vbCrLf
                            Next
                            sMsg = sMsg & gcsRM.GetString("csDESCRIPTION_CAP") & vbCrLf
                            sMsg = gcsRM.GetString("csU_SELECTED_FOR_COPY") & vbCrLf & vbCrLf & _
                                   sMsg & vbCrLf & gcsRM.GetString("csCLICK_OK_PROCEED")

                            Dim lReply As Windows.Forms.DialogResult = MessageBox.Show(sMsg, gcsRM.GetString("csCONFIRM_COPY"), _
                                                                                       MessageBoxButtons.OKCancel, MessageBoxIcon.Information, _
                                                                                       MessageBoxDefaultButton.Button2, _
                                                                                       MessageBoxOptions.DefaultDesktopOnly)
                            If lReply <> Windows.Forms.DialogResult.OK Then
                                subEnableControls(True)
                                Exit Select
                            End If
                        Else
                            sMsg = gcsRM.GetString("csCLICK_OK_PROCEED")

                            Dim lReply As Windows.Forms.DialogResult = MessageBox.Show(sMsg, gcsRM.GetString("csCONFIRM_COPY"), _
                                                           MessageBoxButtons.OKCancel, MessageBoxIcon.Information, _
                                                           MessageBoxDefaultButton.Button2, _
                                                           MessageBoxOptions.DefaultDesktopOnly)
                            If lReply <> Windows.Forms.DialogResult.OK Then
                                subEnableControls(True)
                                Exit Select
                            End If
                        End If
                    End If
                    If e.ClickedItem.Tag Is Nothing Then
                        subDoCopy(eCopyType.CopyAll)
                    Else
                        subDoCopy(CType(e.ClickedItem.Tag, eCopyType))
                    End If

                Case "btnCopyV"
                    Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                    If o.DropDownButtonPressed Then Exit Sub

                    Dim bNothingSelected As Boolean 'RJO 07/04/2010
                    Dim nTmp As Integer = 0
                    Dim nTag As Integer = 0
                    Dim sMsg As String = String.Empty
                    'first see if the dropdowns are a sub menu
                    Dim oMenu As ToolStripMenuItem = TryCast(o.DropDownItems(0), ToolStripMenuItem)

                    If oMenu Is Nothing Then
                        For Each oD As ToolStripButton In o.DropDownItems
                            If oD.Tag Is Nothing = False Then
                                If oD.Checked Then
                                    nTag = CInt(oD.Tag)
                                    If nTag > 0 Then
                                        Status = oD.Text & " " & gcsRM.GetString("csSELECTED_FOR_COPY")
                                        sMsg = sMsg & oD.Text & vbCrLf
                                    End If
                                    nTmp = nTmp + nTag
                                End If
                            End If
                        Next
                        'If no items were checked, then default to All items 'RJO 07/04/2010
                        If nTmp = 0 Then
                            bNothingSelected = True
                            For Each oD As ToolStripButton In o.DropDownItems
                                If oD.Tag Is Nothing = False Then
                                    nTag = CInt(oD.Tag)
                                    If nTag > 0 Then
                                        Status = oD.Text & " " & gcsRM.GetString("csSELECTED_FOR_COPY")
                                        sMsg = sMsg & oD.Text & vbCrLf
                                    End If
                                    nTmp = nTmp + nTag
                                End If
                            Next
                        End If
                    Else
                        For Each oD As ToolStripButton In oMenu.DropDownItems
                            If oD.Checked Then
                                If oD.Tag Is Nothing = False Then
                                    nTag = CInt(oD.Tag)
                                    If nTag > 0 Then
                                        Status = oD.Text & " " & gcsRM.GetString("csSELECTED_FOR_COPY")
                                        sMsg = sMsg & oD.Text & vbCrLf
                                    End If
                                    nTmp = nTmp + nTag
                                End If
                            End If
                        Next
                        'If no items were checked, then default to All items 'RJO 07/04/2010
                        If nTmp = 0 Then
                            bNothingSelected = True
                            For Each oD As ToolStripButton In oMenu.DropDownItems
                                If oD.Tag Is Nothing = False Then
                                    nTag = CInt(oD.Tag)
                                    If nTag > 0 Then
                                        Status = oD.Text & " " & gcsRM.GetString("csSELECTED_FOR_COPY")
                                        sMsg = sMsg & oD.Text & vbCrLf
                                    End If
                                    nTmp = nTmp + nTag
                                End If
                            Next
                        End If
                    End If

                    If bNothingSelected Then 'nTmp = 0 Then 'RJO 07/04/2010
                        sMsg = gcsRM.GetString("csNOTHING_SELECTED_COPY") & vbCrLf & vbCrLf & sMsg & vbCrLf & _
                               gcsRM.GetString("csSELECTED_FOR_COPY_DEF") & " " & gcsRM.GetString("csCLICK_OK_PROCEED") 'RJO 07/04/2010
                        Dim lReply As Windows.Forms.DialogResult = MessageBox.Show(sMsg, gcsRM.GetString("csCONFIRM_COPY"), _
                                                                                   MessageBoxButtons.OKCancel, MessageBoxIcon.Information, _
                                                                                   MessageBoxDefaultButton.Button2, _
                                                                                   MessageBoxOptions.DefaultDesktopOnly)
                        If lReply <> Windows.Forms.DialogResult.OK Then
                            subEnableControls(True)
                            Exit Select
                        End If
                        nTmp = eCopyType.CopyValuesMask
                    Else
                        sMsg = gcsRM.GetString("csU_SELECTED_FOR_COPY") & vbCrLf & vbCrLf & sMsg & vbCrLf & _
                                    gcsRM.GetString("csCLICK_OK_PROCEED")

                        Dim lReply As Windows.Forms.DialogResult = MessageBox.Show(sMsg, gcsRM.GetString("csCONFIRM_COPY"), _
                                                                                   MessageBoxButtons.OKCancel, MessageBoxIcon.Information, _
                                                                                   MessageBoxDefaultButton.Button2, _
                                                                                   MessageBoxOptions.DefaultDesktopOnly)
                        If lReply <> Windows.Forms.DialogResult.OK Then
                            subEnableControls(True)
                            Exit Select
                        End If

                    End If

                    If bAllApplicatorsTheSame(True) = False Then
                        Exit Sub
                    End If

                    subEnableControls(False)
                    btnClose.Enabled = False

                    subDoCopy(CType(nTmp, eCopyType))

                Case "btnCopyD"
                    If bAllApplicatorsTheSame(True) = False Then
                        Exit Sub
                    End If

                    If mbEnableAccessibilityFeatures Then
                        Dim sMsg As String = String.Empty

                        If (ScreenName = "Presets") Then
                            sMsg = gcsRM.GetString("csDESCRIPTION_CAP") & vbCrLf
                            sMsg = gcsRM.GetString("csU_SELECTED_FOR_COPY") & vbCrLf & vbCrLf & _
                                   sMsg & vbCrLf & gcsRM.GetString("csCLICK_OK_PROCEED")

                            Dim lReply As Windows.Forms.DialogResult = MessageBox.Show(sMsg, gcsRM.GetString("csCONFIRM_COPY"), _
                                                                                       MessageBoxButtons.OKCancel, MessageBoxIcon.Information, _
                                                                                       MessageBoxDefaultButton.Button2, _
                                                                                       MessageBoxOptions.DefaultDesktopOnly)
                            If lReply <> Windows.Forms.DialogResult.OK Then
                                subEnableControls(True)
                                Exit Select
                            End If
                        Else
                            sMsg = gcsRM.GetString("csCLICK_OK_PROCEED")

                            Dim lReply As Windows.Forms.DialogResult = MessageBox.Show(sMsg, gcsRM.GetString("csCONFIRM_COPY"), _
                                                           MessageBoxButtons.OKCancel, MessageBoxIcon.Information, _
                                                           MessageBoxDefaultButton.Button2, _
                                                           MessageBoxOptions.DefaultDesktopOnly)
                            If lReply <> Windows.Forms.DialogResult.OK Then
                                subEnableControls(True)
                                Exit Select
                            End If
                        End If
                    End If

                    subEnableControls(False)
                    btnClose.Enabled = False
                    If e.ClickedItem.Tag Is Nothing Then
                        subDoCopy(eCopyType.CopyText)
                    Else
                        subDoCopy(CType(e.ClickedItem.Tag, eCopyType))
                    End If

            End Select

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub frmCopy_Layout(ByVal sender As Object, _
                                ByVal e As System.Windows.Forms.LayoutEventArgs) Handles Me.Layout
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


    End Sub
    Private Sub frmCopy_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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
    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

#End Region

End Class  'frmCopy
