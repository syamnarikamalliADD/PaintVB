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
' Form/Module: ColorChangeCartoons.vb
'
' Description: Fluid diagram cartoon specific classes
'		stick all the user control references here so other programs won't need them
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
' 03/17/07      gks     Onceover Cleanup
' 08/15/07      gks     typed collections - imagelist still needs work
' 05/29/09      MSW     switch to VB 2008
'			Revised clsColorChangeCartoon and its children to fit cartoons in user controls.  
'                       Split group and shared valves for 32 valve support
'			Moved cartoon classes out of vb into here (ColorChangeCartoons.vb)
' 11/10/09      MSW     Add versabell2plusWB class
'			change to read valve names from the robot and use them to 
'			generate tags for the resource file as rs{robot valve name}
' 11/30/09      MSW     Add pressure test "subGetPressureTestValveMask" routine
' 12/03/09      MSW     Merge AA and Honda changes
' 04/09/10      RJO     subInitialize(ByVal sDBPath As String) - Added Arm to parameters. 
'                       Otherwise, call to subGetValveLabels in subInitialize barfs because 
'                       mRobot = Nothing.
' 07/07/10      MSW     Add versabell WB class
'   09/27/11    MSW     Standalone changes round 1 - HGB SA paintshop computer changes   4.1.0.1
'    01/24/12   MSW     Applicator Updates                                            4.01.01.02
'    03/15/12   MSW     subInitialize - add NumberOfGroupValves = mApplicator.NumGroupValves 4.01.01.03
'    09/30/13   MSW     add clsVersabell3dualWBCartoon                                       4.01.05.04
'    10/11/13   MSW     add clsVersabell3WBCartoon                                           4.01.06.00
'    02/12/14   MSW     Merge changes from KTP                                               4.01.07.00
'    07/16/14   RJO     Added clsVersabell3P1000Cartoon                                      4.01.07.01
'********************************************************************************************

Option Compare Binary
Option Explicit On
Option Strict On

Imports System.Collections.Generic
Imports System.Collections.ObjectModel

Imports FRRobot

'********************************************************************************************
'Description: Base class for the valve cartoons
'
'
'Modification history:
'
' Date      By      Reason
'********************************************************************************************
Friend MustInherit Class clsColorChangeCartoon

#Region " Declares "

    '9.5.07 remove unnecessary initializations per fxCop

    Friend Event SharedValveClicked(ByVal nValve As Integer, ByVal bCurState As Boolean)
    Friend Event GroupValveClicked(ByVal nValve As Integer, ByVal bCurState As Boolean)
    Private mnNUM_VALVES As Integer = 20
    Private mnNUM_SHARED_VALVES As Integer = 6 'NVSP 11/02/2016 Changed to accomodate piggable
    Private mnNUM_GROUP_VALVES As Integer = 16
    Protected Friend mForm As System.Windows.Forms.Form ' = Nothing
    Protected Friend mUctl As System.Windows.Forms.UserControl ' = Nothing
    Protected Friend msDBPath As String = String.Empty
    Protected Friend mRobot As clsArm
    Protected Friend mApplicator As clsApplicator
    Private meColorChangeType As eColorChangeType = eColorChangeType.NONE
    Protected Friend sSharedValveNames As String()
    Protected Friend sGroupValveNames As String()
    Protected Friend mcSharedValveColors As Color()
    Protected Friend mcGroupValveColors As Color()
    'id's for the params you pass in to additional parameters for the cartoons
    Friend Enum eAddParmID
        CanisterPos = 0
        PaintInCan = 1
    End Enum
    Friend WithEvents muctrlToon As CCToonUctrl

#End Region
#Region " Properties "

    Friend Property ColorChangeType() As eColorChangeType
        '********************************************************************************************
        'Description: who am i - set me first
        '
        'Parameters:  none
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return meColorChangeType
        End Get
        Set(ByVal value As eColorChangeType)
            meColorChangeType = value
        End Set
    End Property
    '********************************************************************************************
    'Description: How many total valves
    '   Override routine only required if it's more than a normal cc setup (20)
    'Parameters:  none
    'Returns:     
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************

    Friend Overridable Property NumberOfColorChangeValves() As Integer
        Get
            Return mnNUM_VALVES
        End Get
        Set(ByVal value As Integer)
            mnNUM_VALVES = value
        End Set
    End Property
    '********************************************************************************************
    'Description: How many shared valves
    '   Override routine only required if it's more than a normal cc setup (4)
    'Parameters:  none
    'Returns:     
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Friend Overridable Property NumberOfSharedValves() As Integer
        Get
            Return mnNUM_SHARED_VALVES
        End Get
        Set(ByVal value As Integer)
            mnNUM_SHARED_VALVES = value
        End Set
    End Property
    '********************************************************************************************
    'Description: How many group valves
    '   Override routine only required if it's more than a normal cc setup (16)
    'Parameters:  none
    'Returns:     
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Friend Overridable Property NumberOfGroupValves() As Integer
        Get
            Return mnNUM_GROUP_VALVES
        End Get
        Set(ByVal value As Integer)
            mnNUM_GROUP_VALVES = value
        End Set
    End Property
    '********************************************************************************************
    'Description: SharedValveNames
    '
    'Parameters:  none
    'Returns:     
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Friend Overridable ReadOnly Property SharedValveNames(ByVal nIndex As Integer) As String
        Get
            Return sSharedValveNames(nIndex)
        End Get
    End Property
    '********************************************************************************************
    'Description: SharedValveNames
    '
    'Parameters:  none
    'Returns:     
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Friend Overridable ReadOnly Property GroupValveNames(ByVal nIndex As Integer) As String
        Get
            Return sGroupValveNames(nIndex)
        End Get
    End Property
    Friend Property SharedValveStates() As Integer
        '********************************************************************************************
        'Description: Shared valve states property.  Just a passthrough between the form and usercontrol
        '       Codes' the same in each cc type, but "muctrlToon" references different objects
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return muctrlToon.SharedValveStates
        End Get
        Set(ByVal value As Integer)
            muctrlToon.SharedValveStates = value
        End Set
    End Property
    Friend Property GroupValveStates() As Integer
        '********************************************************************************************
        'Description: Group valve states property.  Just a passthrough between the form and usercontrol
        '       Codes' the same in each cc type, but "muctrlToon" references different objects
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return muctrlToon.GroupValveStates
        End Get
        Set(ByVal value As Integer)
            muctrlToon.GroupValveStates = value
        End Set
    End Property
    Public Property PaintHeader() As String
        '********************************************************************************************
        'Description: Gives access to the user control's paint header label
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return muctrlToon.PaintColorName
        End Get
        Set(ByVal value As String)
            muctrlToon.PaintColorName = value
        End Set
    End Property
    Public Property ShowFeedbackLabels() As Boolean
        '********************************************************************************************
        'Description: Show or hide feedback labels on usercontrol
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return muctrlToon.ShowFeedBackLabels
        End Get
        Set(ByVal value As Boolean)
            muctrlToon.ShowFeedBackLabels = value
        End Set
    End Property

#End Region
#Region " Routines "
    Public Sub subGetValveColors(ByRef SharedValveColors As Color(), ByRef GroupValveColors As Color())
        '********************************************************************************************
        'Description: return the colors for the valve grid
        'Parameters:  arrays of color values
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            ReDim SharedValveColors(mcSharedValveColors.GetUpperBound(0) + 1)
            Array.Copy(mcSharedValveColors, SharedValveColors, mcSharedValveColors.GetUpperBound(0) + 1)
            ReDim GroupValveColors(mcGroupValveColors.GetUpperBound(0) + 1)
            Array.Copy(mcGroupValveColors, GroupValveColors, mcGroupValveColors.GetUpperBound(0) + 1)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub
    Friend Sub Initialize(ByVal rUserControl As System.Windows.Forms.UserControl, ByVal sDBPath As String, ByVal Arm As clsArm, _
                          ByRef oApplicator As clsApplicator)
        '********************************************************************************************
        'Description: Set type then call me
        '
        'Parameters:  none
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mUctl = rUserControl
        msDBPath = sDBPath
        mRobot = Arm
        If oApplicator IsNot Nothing Then
            mApplicator = oApplicator
        ElseIf mRobot.Applicator IsNot Nothing Then
            mApplicator = mRobot.Applicator
        End If
        'this should set what we need from derived class
        subInitialize()
    End Sub
    Friend Sub Initialize(ByVal rForm As System.Windows.Forms.Form, ByVal sDBPath As String, ByVal Arm As clsArm)
        '********************************************************************************************
        'Description: Set type then call me
        '
        'Parameters:  none
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mForm = rForm
        msDBPath = sDBPath
        mRobot = Arm
        'this should set what we need from derived class
        subInitialize()

    End Sub
    Friend Sub subUpdateAdditionalParams(ByRef sLabels() As String)
        '********************************************************************************************
        'Description: pass extra labels to the control
        '
        'Parameters:  none
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If Not (muctrlToon Is Nothing) Then
            muctrlToon.SetAdditionalData(sLabels)
        End If
    End Sub
    Friend Sub Initialize(ByVal sDBPath As String, ByVal Arm As clsArm)
        'Friend Sub Initialize(ByVal sDBPath As String) 'RJO 04/09/10
        '********************************************************************************************
        'Description: Set type then call me
        '
        'Parameters:  none
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '04/09/10   RJO     Added Arm to parameters. Otherwise, call to subGetValveLabels in 
        '                   subInitialize barfs because mRobot = Nothing.
        '********************************************************************************************
        mForm = Nothing
        mUctl = Nothing
        msDBPath = sDBPath
        mRobot = Arm 'RJO 04/09/10
        'this should set what we need from derived class
        subInitialize()

    End Sub
    '********************************************************************************************
    'Description: Tell the user control to update the display
    '   Just a go-between for frmMain and muctrlToon, saves on some casting and extra variables in frmMain
    '********************************************************************************************
    Friend Sub subUpdateValveCartoon()
        '********************************************************************************************
        'Description: Tell the user control to update the display
        '   Just a go-between for frmMain and muctrlToon, saves on some casting and extra variables in frmMain
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        muctrlToon.UpdateCartoons()
    End Sub
    '********************************************************************************************
    'Description: Initialize a new class object for the cc type
    '********************************************************************************************
    Friend MustOverride Sub subGetPressureTestValveMask(ByVal eStep As ePressureTestStep, ByRef nGroup As Integer, ByRef nShared As Integer)
    Friend MustOverride Sub subInitialize()
    '********************************************************************************************
    'Description: validate the valve states for this applicator
    '
    'Parameters:  valve state selection
    'Returns:     valve state selection with any problems turned off.
    '********************************************************************************************
    Friend MustOverride Function ValidateValveSelection(ByRef nSharedValveStates As Integer, ByRef nGroupValveStates As Integer) As Boolean
#End Region
#Region "Events"

    Friend Sub subSharedValveClickHandler(ByVal nValve As Integer, ByVal bCurState As Boolean) Handles muctrlToon.SharedValveClick
        RaiseEvent SharedValveClicked(nValve, bCurState)
    End Sub
    Friend Sub subGroupValveClickHandler(ByVal nValve As Integer, ByVal bCurState As Boolean) Handles muctrlToon.GroupValveClick
        RaiseEvent GroupValveClicked(nValve, bCurState)
    End Sub
#End Region
#Region " Events"

    Friend Sub New()
        '********************************************************************************************
        'Description: hello world
        '
        'Parameters:  none
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        MyBase.New()

    End Sub


#End Region

End Class  'clsColorChangeCartoon
'********************************************************************************************
'Description: Accustat Cartoons
'       This one is a long way off, don't use it as an example
'
'Modification history:
'
' Date      By      Reason
'********************************************************************************************
Friend Class clsAccustatCartoon

    Inherits clsColorChangeCartoon

#Region " Declares "

    Friend Enum eACCUSTATVALVES
        'Array starts at 0
        Trigger = 0
        Valve2 = 1
        Valve3 = 2
        ColorEnable = 3
        AppClnSolv = 4
        AppClnAir = 5
        PurgeSolvent = 6
        PurgeAir = 7
        Dump = 8
        RegOverride = 9
        PistonPress = 10
        PaintFill = 11
        Valve13 = 12
        Valve14 = 13
        Valve15 = 14
        Valve16 = 15
        Valve17 = 16
        Valve18 = 17
        BlowOff = 18
        DumpRestrictor = 19
        LastValve = 19
    End Enum
    Private msPaintColor As String = "Paint"
#End Region

#Region " Routines "
    '********************************************************************************************
    'Description: provide pressure test valve masks for each step by cc type
    '
    'Parameters:  eStep = pressure test step
    'Returns:  nGroup= Group valves, nShared = shared valves   
    '
    'Modification history:
    '
    ' Date      By      Reason
    ' 11/30/09      MSW     Add pressure test "subGetPressureTestValveMask" routine
    '********************************************************************************************
    Friend Overrides Sub subGetPressureTestValveMask(ByVal eStep As ePressureTestStep, ByRef nGroup As Integer, ByRef nShared As Integer)
        'no pressure test for accustat
        Select Case eStep
            Case ePressureTestStep.FillSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.FillAir
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureAir
                nShared = 0
                nGroup = 0
            Case Else
                nShared = 0
                nGroup = 0
        End Select
    End Sub
    Friend Overrides Sub subInitialize()
        '********************************************************************************************
        'Description: 
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'Dim oL As Label = Nothing
        'Dim sControl As String = String.Empty

        'MyBase.ColorChangeType = eColorChangeType.ACCUSTAT

        'If mForm Is Nothing Then
        '    'must be user control
        '    MyBase.mgpBox = DirectCast(GetControlByName("gpbAccustat", mUctl), GroupBox)
        'Else
        '    'must be form
        '    MyBase.mgpBox = DirectCast(GetControlByName("gpbAccustat", mForm), GroupBox)
        'End If

        'For i As Integer = 0 To NumberOfColorChangeValves
        '    mnValveState(i) = 0
        'Next

        'Try
        '    'get valve names - should come from robot, but they are not to descriptive
        '    For i As Integer = 0 To NumberOfColorChangeValves
        '        sControl = "lblAccustatVLV" & Format(i, "00")
        '        oL = DirectCast(MyBase.mgpBox.Controls(sControl), Label)
        '        If oL Is Nothing = False Then
        '            oL.Text = grsRM.GetString("rsACCUSTAT_VALVE" & Format(i, "00"))
        '            oL.Visible = True
        '        End If
        '    Next

        '    sControl = "lblAccustatSolvHeader01"
        '    oL = DirectCast(MyBase.mgpBox.Controls(sControl), Label)
        '    If oL Is Nothing = False Then
        '        oL.Text = grsRM.GetString("rsACCUSTAT_VALVE07")
        '    End If

        '    sControl = "lblAccustatAirHeader01"
        '    oL = DirectCast(MyBase.mgpBox.Controls(sControl), Label)
        '    If oL Is Nothing = False Then
        '        oL.Text = grsRM.GetString("rsACCUSTAT_VALVE08")
        '    End If

        'Catch ex As Exception
        '    Trace.WriteLine(ex.Message)
        'End Try

    End Sub
    Friend Overrides Function ValidateValveSelection(ByRef nSharedValveStates As Integer, ByRef nGroupValveStates As Integer) As Boolean
        '********************************************************************************************
        'Description: validate the valve states for this applicator
        '
        'Parameters:  valve state selection
        'Returns:     valve state selection with any problems turned off.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Return (True)
    End Function
#End Region

End Class  'clsAccustatCartoon


'********************************************************************************************
'Description: Versabell Cartoons
'       Good starting point here if you need to copy for a new one that only has 1 CC group
'       Declares, properties: Just inherits from clsColorChangeCartooon
'       Routines: ValidateValveSelection and subInitialize
'       To use 2 CC groups, see clsVersabell2plusCartoon
'Modification history:
'
' Date      By      Reason
'********************************************************************************************
Friend Class clsVersabellCartoon
    Inherits clsColorChangeCartoon
#Region " Routines "
    '********************************************************************************************
    'Description: provide pressure test valve masks for each step by cc type
    '
    'Parameters:  eStep = pressure test step
    'Returns:  nGroup= Group valves, nShared = shared valves   
    '
    'Modification history:
    '
    ' Date      By      Reason
    ' 11/30/09      MSW     Add pressure test "subGetPressureTestValveMask" routine
    '********************************************************************************************
    Friend Overrides Sub subGetPressureTestValveMask(ByVal eStep As ePressureTestStep, ByRef nGroup As Integer, ByRef nShared As Integer)
        'Shared
        'pTrig = 1 
        'Group
        'pPE = 1
        'pSOL = 8
        'pAIR = 16
        'pCC = 32
        'pFLUSH = 128
        Const mnTrigOn As Integer = 1
        Const mnAllOff As Integer = 0
        'flush, pcc, paint enable
        Const mnSolvAirPath As Integer = 161 '128 + 32 + 1
        'Solvent
        Const mnSolvMask As Integer = 8
        'Air, valve
        Const mnAirMask As Integer = 16

        Select Case eStep
            Case ePressureTestStep.FillSolv, ePressureTestStep.Start
                nShared = mnTrigOn 'Fill through trigger
                nGroup = mnSolvAirPath + mnSolvMask
            Case ePressureTestStep.MeasureSolv
                nShared = mnAllOff
                nGroup = mnSolvAirPath + mnSolvMask
            Case ePressureTestStep.FillAir
                nShared = mnTrigOn 'Fill through trigger
                nGroup = mnSolvAirPath + mnAirMask
            Case ePressureTestStep.MeasureAir
                nShared = mnAllOff
                nGroup = mnSolvAirPath + mnAirMask
            Case Else
                nShared = mnAllOff
                nGroup = mnAllOff
        End Select
    End Sub
    Friend Overrides Sub subInitialize()
        '********************************************************************************************
        'Description: Initialize clsVersabellCartoon cartoon
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/15/12  MSW     subInitialize - add NumberOfGroupValves = mApplicator.NumGroupValves
        '********************************************************************************************
        Dim oL As Label = Nothing
        Dim sControl As String = String.Empty

        Try

            MyBase.ColorChangeType = eColorChangeType.VERSABELL

            If mForm Is Nothing Then
                'must be user control
                muctrlToon = DirectCast(mUctl, uctrlVersabell)
            Else
                'must be form
                muctrlToon = DirectCast(GetControlByName("uctrlCartoon", mForm), uctrlVersabell)
            End If

            'get valve names, set colors to green as a default
            'Shared Valves
            'ReDim sSharedValveNames(NumberOfSharedValves)
            NumberOfSharedValves = mApplicator.NumSharedValves
            sSharedValveNames = mApplicator.SharedValves
            ReDim mcSharedValveColors(NumberOfSharedValves)
            'Group Valves
            sGroupValveNames = mApplicator.GroupValves
            NumberOfGroupValves = mApplicator.NumGroupValves
            ReDim mcGroupValveColors(NumberOfGroupValves)
            'subGetValveLabels(mRobot, sSharedValveNames, sGroupValveNames, True)
            For i As Integer = sSharedValveNames.GetLowerBound(0) To sSharedValveNames.GetUpperBound(0)
                mcSharedValveColors(i) = Color.Green
            Next
            For i As Integer = sGroupValveNames.GetLowerBound(0) To sGroupValveNames.GetUpperBound(0)
                mcGroupValveColors(i) = Color.Green
            Next

            'Set solvent and air colors
            'Paint
            mcSharedValveColors(3) = Color.Purple
            'Solvent
            mcGroupValveColors(3) = Color.Red
            mcGroupValveColors(15) = Color.Red
            'Air
            mcGroupValveColors(4) = Color.Blue
            mcGroupValveColors(14) = Color.Blue
            Dim sLabels As String()
            ReDim sLabels(10)
            sLabels(0) = "lblOutPressureTag"
            sLabels(1) = grsRM.GetString("rsOUT_PRESSURE")
            sLabels(2) = "lblFlowTag"
            sLabels(3) = grsRM.GetString("rs_CMD_FLOW_CCMIN")
            sLabels(4) = "lblOutPressure"
            sLabels(5) = "0.0"
            sLabels(6) = "lblFlow"
            sLabels(7) = "0"
            sLabels(8) = "lblOutPressure2"
            sLabels(9) = "0.0"
            If Not (muctrlToon Is Nothing) Then
                'Let it get all the valve names without loading a user control
                muctrlToon.SetSharedValveLabels(sSharedValveNames)
                muctrlToon.SetGroupValveLabels(sGroupValveNames)
                muctrlToon.SolvHeaderLabel = grsRM.GetString("rsSOLVENT_SUPPLY")
                muctrlToon.AirHeaderLabel = grsRM.GetString("rsAIR_SUPPLY")
                muctrlToon.PaintColorName = grsRM.GetString("rsPAINT_SUPPLY")
            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Overrides Function ValidateValveSelection(ByRef nSharedValveStates As Integer, ByRef nGroupValveStates As Integer) As Boolean
        '********************************************************************************************
        'Description: validate the valve states for this applicator
        '
        'Parameters:  valve state selection
        'Returns:     valve state selection with any problems turned off.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return (True)
    End Function
#End Region
End Class  'clsVersabellCartoon

'********************************************************************************************
'Description: clsVersabell2plusWBCartoon Cartoons
'       Good starting point here if you need to copy for a new one that has 2 CC groups
'   Inherits from clsColorChangeCartoon and adds:
'       Declares: num valve constants, only required for > 4 shared or 16 group
'       Properties: num valve properties, only required for > 4 shared or 16 group
'       Routines: ValidateValveSelection and subInitialize
'       To use 1 CC group, see clsVersabellCartoon
'Modification history:
'
' Date      By      Reason
'********************************************************************************************
Friend Class clsVersabell2plusWBCartoon
    Inherits clsColorChangeCartoon

#Region " Declares "
    'Expanded valves should be declared here
    Private Const mnNUM_VALVES As Integer = 35
    Private Const mnNUM_SHARED_VALVES As Integer = 4
    Private Const mnNUM_GROUP_VALVES As Integer = 31
#End Region
#Region " Properties for expanded valves "
#End Region
#Region " Routines "
    '********************************************************************************************
    'Description: provide pressure test valve masks for each step by cc type
    '
    'Parameters:  eStep = pressure test step
    'Returns:  nGroup= Group valves, nShared = shared valves   
    '
    'Modification history:
    '
    ' Date      By      Reason
    ' 11/30/09      MSW     Add pressure test "subGetPressureTestValveMask" routine
    '********************************************************************************************
    Friend Overrides Sub subGetPressureTestValveMask(ByVal eStep As ePressureTestStep, ByRef nGroup As Integer, ByRef nShared As Integer)
        'Shared
        'pTrig = 1 
        'Group
        'pPE = 1
        'pSOL = 8
        'pAIR = 16
        'pCC = 32
        'pFLUSH = 128
        Const mnTrigOn As Integer = 1
        Const mnAllOff As Integer = 0
        'flush, pcc, paint enable
        Const mnSolvAirPath As Integer = 161 '128 + 32 + 1
        'Solvent
        Const mnSolvMask As Integer = 8
        'Air, valve
        Const mnAirMask As Integer = 16

        Select Case eStep
            Case ePressureTestStep.FillSolv
                nShared = mnTrigOn 'Fill through trigger
                nGroup = mnSolvAirPath + mnSolvMask
            Case ePressureTestStep.MeasureSolv
                nShared = mnAllOff
                nGroup = mnSolvAirPath + mnSolvMask
            Case ePressureTestStep.FillAir
                nShared = mnTrigOn 'Fill through trigger
                nGroup = mnSolvAirPath + mnAirMask
            Case ePressureTestStep.MeasureAir
                nShared = mnAllOff
                nGroup = mnSolvAirPath + mnAirMask
            Case Else
                nShared = mnAllOff
                nGroup = mnAllOff
        End Select
    End Sub

    Friend Overrides Sub subInitialize()
        '********************************************************************************************
        'Description: Initialize clsVersabell2plusWBCartoon cartoon
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/15/12  MSW     subInitialize - add NumberOfGroupValves = mApplicator.NumGroupValves
        '********************************************************************************************
        Dim oL As Label = Nothing
        Dim sControl As String = String.Empty
        Try

            MyBase.ColorChangeType = eColorChangeType.VERSABELL2_PLUS_WB

            If mForm Is Nothing Then
                'must be user control
                muctrlToon = DirectCast(mUctl, uctrlVersabell2plusWB)
            Else
                'must be form
                muctrlToon = DirectCast(GetControlByName("uctrlCartoon", mForm), uctrlVersabell2plusWB)
            End If

            'get valve names, set colors to green as a default
            'Shared Valves
            'ReDim sSharedValveNames(NumberOfSharedValves)
            NumberOfSharedValves = mApplicator.NumSharedValves
            sSharedValveNames = mApplicator.SharedValves
            ReDim mcSharedValveColors(NumberOfSharedValves)
            'Group Valves
            'ReDim sGroupValveNames(NumberOfGroupValves)
            sGroupValveNames = mApplicator.GroupValves
            NumberOfGroupValves = mApplicator.NumGroupValves
            ReDim mcGroupValveColors(NumberOfGroupValves)
            'subGetValveLabels(mRobot, sSharedValveNames, sGroupValveNames, True)
            For i As Integer = sSharedValveNames.GetLowerBound(0) To sSharedValveNames.GetUpperBound(0)
                mcSharedValveColors(i) = Color.Green
            Next
            For i As Integer = sGroupValveNames.GetLowerBound(0) To sGroupValveNames.GetUpperBound(0)
                mcGroupValveColors(i) = Color.Green
            Next

            'Set solvent and air colors
            'Paint
            mcSharedValveColors(3) = Color.Purple
            'Solvent
            mcGroupValveColors(3) = Color.Red
            mcGroupValveColors(29) = Color.Red
            'Air
            mcGroupValveColors(4) = Color.Blue
            mcGroupValveColors(28) = Color.Blue
            mcGroupValveColors(30) = Color.Blue

            Dim sLabels As String()
            ReDim sLabels(12)
            sLabels(0) = "lblFlow"
            sLabels(1) = "0.0"
            sLabels(2) = "lblCanPos"
            sLabels(3) = "0.0"
            sLabels(4) = "lblCanTorque"
            sLabels(5) = "0.0"
            sLabels(6) = "lblFlowTag"
            sLabels(7) = grsRM.GetString("rs_CMD_FLOW_CCMIN")
            sLabels(8) = "lblCanPosTag"
            sLabels(9) = grsRM.GetString("rsWB_CAN_POS")
            sLabels(10) = "lblCanTorqueTag"
            sLabels(11) = grsRM.GetString("rsWB_CAN_TORQUE")


            If Not (muctrlToon Is Nothing) Then
                'Let it get all the valve names without loading a user control
                muctrlToon.SetSharedValveLabels(sSharedValveNames)
                muctrlToon.SetGroupValveLabels(sGroupValveNames)
                muctrlToon.SolvHeaderLabel = grsRM.GetString("rsSOLVENT_SUPPLY")
                muctrlToon.AirHeaderLabel = grsRM.GetString("rsAIR_SUPPLY")
                muctrlToon.PaintColorName = grsRM.GetString("rsPAINT_SUPPLY")
                muctrlToon.SetAdditionalData(sLabels)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Overrides Function ValidateValveSelection(ByRef nSharedValveStates As Integer, ByRef nGroupValveStates As Integer) As Boolean
        '********************************************************************************************
        'Description: validate the valve states for this applicator
        '
        'Parameters:  valve state selection
        'Returns:     valve state selection with any problems turned off.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return (True)
    End Function
#End Region
End Class  'clsVersabell2plusWBCartoon





'********************************************************************************************
'Description: clsVersabell2WBCartoon Cartoons
'       Good starting point here if you need to copy for a new one that has 2 CC groups
'   Inherits from clsColorChangeCartoon and adds:
'       Declares: num valve constants, only required for > 4 shared or 16 group
'       Properties: num valve properties, only required for > 4 shared or 16 group
'       Routines: ValidateValveSelection and subInitialize
'       To use 1 CC group, see clsVersabellCartoon
'Modification history:
'
' Date      By      Reason
'********************************************************************************************
Friend Class clsVersabell2WBCartoon
    Inherits clsColorChangeCartoon

#Region " Declares "
    'Expanded valves should be declared here
    Private Const mnNUM_VALVES As Integer = 35
    Private Const mnNUM_SHARED_VALVES As Integer = 4
    Private Const mnNUM_GROUP_VALVES As Integer = 31
#End Region
#Region " Properties for expanded valves "
#End Region
#Region " Routines "
    '********************************************************************************************
    'Description: provide pressure test valve masks for each step by cc type
    '
    'Parameters:  eStep = pressure test step
    'Returns:  nGroup= Group valves, nShared = shared valves   
    '
    'Modification history:
    '
    ' Date      By      Reason
    ' 11/30/09      MSW     Add pressure test "subGetPressureTestValveMask" routine
    '********************************************************************************************
    Friend Overrides Sub subGetPressureTestValveMask(ByVal eStep As ePressureTestStep, ByRef nGroup As Integer, ByRef nShared As Integer)
        'Shared
        'pTrig = 1 
        'Group
        'pPE = 1
        'pSOL = 8
        'pAIR = 16
        'pCC = 32
        'pFLUSH = 128
        Const mnTrigOn As Integer = 1
        Const mnAllOff As Integer = 0
        'flush, pcc, paint enable
        Const mnSolvAirPath As Integer = 161 '128 + 32 + 1
        'Solvent
        Const mnSolvMask As Integer = 8
        'Air, valve
        Const mnAirMask As Integer = 16

        Select Case eStep
            Case ePressureTestStep.FillSolv
                nShared = mnTrigOn 'Fill through trigger
                nGroup = mnSolvAirPath + mnSolvMask
            Case ePressureTestStep.MeasureSolv
                nShared = mnAllOff
                nGroup = mnSolvAirPath + mnSolvMask
            Case ePressureTestStep.FillAir
                nShared = mnTrigOn 'Fill through trigger
                nGroup = mnSolvAirPath + mnAirMask
            Case ePressureTestStep.MeasureAir
                nShared = mnAllOff
                nGroup = mnSolvAirPath + mnAirMask
            Case Else
                nShared = mnAllOff
                nGroup = mnAllOff
        End Select
    End Sub

    Friend Overrides Sub subInitialize()
        '********************************************************************************************
        'Description: Initialize clsVersabell2WBCartoon cartoon
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/15/12  MSW     subInitialize - add NumberOfGroupValves = mApplicator.NumGroupValves
        '********************************************************************************************
        Dim oL As Label = Nothing
        Dim sControl As String = String.Empty
        Try

            MyBase.ColorChangeType = eColorChangeType.VERSABELL2_WB

            If mForm Is Nothing Then
                'must be user control
                muctrlToon = DirectCast(mUctl, uctrlVersabell2WB)
            Else
                'must be form
                muctrlToon = DirectCast(GetControlByName("uctrlCartoon", mForm), uctrlVersabell2WB)
            End If

            'get valve names, set colors to green as a default
            'Shared Valves
            'ReDim sSharedValveNames(NumberOfSharedValves)
            NumberOfSharedValves = mApplicator.NumSharedValves
            sSharedValveNames = mApplicator.SharedValves
            ReDim mcSharedValveColors(NumberOfSharedValves)
            'Group Valves
            'ReDim sGroupValveNames(NumberOfGroupValves)
            sGroupValveNames = mApplicator.GroupValves
            NumberOfGroupValves = mApplicator.NumGroupValves
            ReDim mcGroupValveColors(NumberOfGroupValves)
            'subGetValveLabels(mRobot, sSharedValveNames, sGroupValveNames, True)
            For i As Integer = sSharedValveNames.GetLowerBound(0) To sSharedValveNames.GetUpperBound(0)
                mcSharedValveColors(i) = Color.Green
            Next
            For i As Integer = sGroupValveNames.GetLowerBound(0) To sGroupValveNames.GetUpperBound(0)
                mcGroupValveColors(i) = Color.Green
            Next

            'Set solvent and air colors
            'Paint
            mcSharedValveColors(3) = Color.Purple
            'Solvent
            mcGroupValveColors(3) = Color.Red
            mcGroupValveColors(29) = Color.Red
            'Air
            mcGroupValveColors(4) = Color.Blue
            mcGroupValveColors(28) = Color.Blue
            mcGroupValveColors(30) = Color.Blue

            Dim sLabels As String()
            ReDim sLabels(12)
            sLabels(0) = "lblFlow"
            sLabels(1) = "0.0"
            sLabels(2) = "lblCanPos"
            sLabels(3) = "0.0"
            sLabels(4) = "lblCanTorque"
            sLabels(5) = "0.0"
            sLabels(6) = "lblFlowTag"
            sLabels(7) = grsRM.GetString("rs_CMD_FLOW_CCMIN")
            sLabels(8) = "lblCanPosTag"
            sLabels(9) = grsRM.GetString("rsWB_CAN_POS")
            sLabels(10) = "lblCanTorqueTag"
            sLabels(11) = grsRM.GetString("rsWB_CAN_TORQUE")


            If Not (muctrlToon Is Nothing) Then
                'Let it get all the valve names without loading a user control
                muctrlToon.SetSharedValveLabels(sSharedValveNames)
                muctrlToon.SetGroupValveLabels(sGroupValveNames)
                muctrlToon.SolvHeaderLabel = grsRM.GetString("rsSOLVENT_SUPPLY")
                muctrlToon.AirHeaderLabel = grsRM.GetString("rsAIR_SUPPLY")
                muctrlToon.PaintColorName = grsRM.GetString("rsPAINT_SUPPLY")
                muctrlToon.SetAdditionalData(sLabels)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Overrides Function ValidateValveSelection(ByRef nSharedValveStates As Integer, ByRef nGroupValveStates As Integer) As Boolean
        '********************************************************************************************
        'Description: validate the valve states for this applicator
        '
        'Parameters:  valve state selection
        'Returns:     valve state selection with any problems turned off.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return (True)
    End Function
#End Region
End Class  'clsVersabell2WBCartoon







'********************************************************************************************
'Description: Versabell2plus Cartoons
'       Good starting point here if you need to copy for a new one that has 2 CC groups
'   Inherits from clsColorChangeCartoon and adds:
'       Declares: num valve constants, only required for > 4 shared or 16 group
'       Properties: num valve properties, only required for > 4 shared or 16 group
'       Routines: ValidateValveSelection and subInitialize
'       To use 1 CC group, see clsVersabellCartoon
'Modification history:
'
' Date      By      Reason
'********************************************************************************************
Friend Class clsVersabell2plusCartoon
    Inherits clsColorChangeCartoon

#Region " Declares "
    'Expanded valves should be declared here
    Private Const mnNUM_VALVES As Integer = 35
    Private Const mnNUM_SHARED_VALVES As Integer = 4
    Private Const mnNUM_GROUP_VALVES As Integer = 31
#End Region
#Region " Properties for expanded valves "
#End Region
#Region " Routines "
    '********************************************************************************************
    'Description: provide pressure test valve masks for each step by cc type
    '
    'Parameters:  eStep = pressure test step
    'Returns:  nGroup= Group valves, nShared = shared valves   
    '
    'Modification history:
    '
    ' Date      By      Reason
    ' 11/30/09      MSW     Add pressure test "subGetPressureTestValveMask" routine
    '********************************************************************************************
    Friend Overrides Sub subGetPressureTestValveMask(ByVal eStep As ePressureTestStep, ByRef nGroup As Integer, ByRef nShared As Integer)
        'TODO set valve states for pressure test
        Select Case eStep
            Case ePressureTestStep.FillSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.FillAir
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureAir
                nShared = 0
                nGroup = 0
            Case Else
                nShared = 0
                nGroup = 0
        End Select
    End Sub
    Friend Overrides Sub subInitialize()
        '********************************************************************************************
        'Description: Initialize clsVersabell2plusCartoon cartoon
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/15/12  MSW     subInitialize - add NumberOfGroupValves = mApplicator.NumGroupValves
        '********************************************************************************************
        Dim oL As Label = Nothing
        Dim sControl As String = String.Empty
        Try

            MyBase.ColorChangeType = eColorChangeType.VERSABELL2_PLUS

            If mForm Is Nothing Then
                'must be user control
                muctrlToon = DirectCast(mUctl, uctrlVersabell2plus)
            Else
                'must be form
                muctrlToon = DirectCast(GetControlByName("uctrlCartoon", mForm), uctrlVersabell2plus)
            End If

            'get valve names, set colors to green as a default
            'Shared Valves
            'ReDim sSharedValveNames(NumberOfSharedValves)
            NumberOfSharedValves = mApplicator.NumSharedValves
            sSharedValveNames = mApplicator.SharedValves
            ReDim mcSharedValveColors(NumberOfSharedValves)
            'Group Valves
            'ReDim sGroupValveNames(NumberOfGroupValves)
            sGroupValveNames = mApplicator.GroupValves
            NumberOfGroupValves = mApplicator.NumGroupValves
            ReDim mcGroupValveColors(NumberOfGroupValves)
            'subGetValveLabels(mRobot, sSharedValveNames, sGroupValveNames, True)
            For i As Integer = sSharedValveNames.GetLowerBound(0) To sSharedValveNames.GetUpperBound(0)
                mcSharedValveColors(i) = Color.Green
            Next
            For i As Integer = sGroupValveNames.GetLowerBound(0) To sGroupValveNames.GetUpperBound(0)
                mcGroupValveColors(i) = Color.Green
            Next

            'Set solvent and air colors
            'Paint
            mcSharedValveColors(3) = Color.Purple
            'Solvent
            mcGroupValveColors(3) = Color.Red
            mcGroupValveColors(29) = Color.Red
            'Air
            mcGroupValveColors(4) = Color.Blue
            mcGroupValveColors(28) = Color.Blue
            mcGroupValveColors(30) = Color.Blue

            Dim sLabels As String()
            ReDim sLabels(8)
            sLabels(0) = "lblOutPressureTag"
            sLabels(1) = grsRM.GetString("rsOUT_PRESSURE")
            sLabels(2) = "lblFlowTag"
            sLabels(3) = grsRM.GetString("rs_CMD_FLOW_CCMIN")
            sLabels(4) = "lblOutPressure"
            sLabels(5) = "0.0"
            sLabels(6) = "lblFlow"
            sLabels(7) = "0.0"

            If Not (muctrlToon Is Nothing) Then
                'Let it get all the valve names without loading a user control
                muctrlToon.SetSharedValveLabels(sSharedValveNames)
                muctrlToon.SetGroupValveLabels(sGroupValveNames)
                muctrlToon.SolvHeaderLabel = grsRM.GetString("rsSOLVENT_SUPPLY")
                muctrlToon.AirHeaderLabel = grsRM.GetString("rsAIR_SUPPLY")
                muctrlToon.PaintColorName = grsRM.GetString("rsPAINT_SUPPLY")
                muctrlToon.SetAdditionalData(sLabels)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Overrides Function ValidateValveSelection(ByRef nSharedValveStates As Integer, ByRef nGroupValveStates As Integer) As Boolean
        '********************************************************************************************
        'Description: validate the valve states for this applicator
        '
        'Parameters:  valve state selection
        'Returns:     valve state selection with any problems turned off.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return (True)
    End Function
#End Region
End Class  'clsVersabell2plusCartoon
'********************************************************************************************
'Description: Gun Cartoons
'
'
'Modification history:
'
' Date      By      Reason
'********************************************************************************************
Friend Class clsGunCartoon
    Inherits clsColorChangeCartoon
#Region " Routines "
    '********************************************************************************************
    'Description: provide pressure test valve masks for each step by cc type
    '
    'Parameters:  eStep = pressure test step
    'Returns:  nGroup= Group valves, nShared = shared valves   
    '
    'Modification history:
    '
    ' Date      By      Reason
    ' 11/30/09      MSW     Add pressure test "subGetPressureTestValveMask" routine
    '********************************************************************************************
    Friend Overrides Sub subGetPressureTestValveMask(ByVal eStep As ePressureTestStep, ByRef nGroup As Integer, ByRef nShared As Integer)
        'no pressure test for this class
        Select Case eStep
            Case ePressureTestStep.FillSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.FillAir
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureAir
                nShared = 0
                nGroup = 0
            Case Else
                nShared = 0
                nGroup = 0
        End Select
    End Sub
    Friend Overrides Sub subInitialize()
        '********************************************************************************************
        'Description: Initialize gun cartoon
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/15/12  MSW     subInitialize - add NumberOfGroupValves = mApplicator.NumGroupValves
        '********************************************************************************************
        Dim oL As Label = Nothing
        Dim sControl As String = String.Empty


        Try
            MyBase.ColorChangeType = eColorChangeType.SINGLE_PURGE

            'Get the user control
            If mForm Is Nothing Then
                'must be user control
                muctrlToon = DirectCast(mUctl, uctrlGun)
            Else
                'must be form
                muctrlToon = DirectCast(GetControlByName("uctrlCartoon", mForm), uctrlGun)
            End If

            'get valve names, set colors to green as a default
            'Shared Valves
            'ReDim sSharedValveNames(NumberOfSharedValves)
            NumberOfSharedValves = mApplicator.NumSharedValves
            sSharedValveNames = mApplicator.SharedValves
            ReDim mcSharedValveColors(NumberOfSharedValves)
            'Group Valves
            'ReDim sGroupValveNames(NumberOfGroupValves)
            sGroupValveNames = mApplicator.GroupValves
            NumberOfGroupValves = mApplicator.NumGroupValves
            ReDim mcGroupValveColors(NumberOfGroupValves)
            'subGetValveLabels(mRobot, sSharedValveNames, sGroupValveNames, True)
            For i As Integer = sSharedValveNames.GetLowerBound(0) To sSharedValveNames.GetUpperBound(0)
                mcSharedValveColors(i) = Color.Green
            Next
            For i As Integer = sGroupValveNames.GetLowerBound(0) To sGroupValveNames.GetUpperBound(0)
                mcGroupValveColors(i) = Color.Green
            Next

            'Set solvent and air colors
            'Paint
            mcSharedValveColors(3) = Color.Purple
            'Solvent
            mcGroupValveColors(3) = Color.Red
            mcGroupValveColors(15) = Color.Red
            'Air
            mcGroupValveColors(4) = Color.Blue
            mcGroupValveColors(7) = Color.Blue
            mcGroupValveColors(14) = Color.Blue


            Dim sLabels As String()
            ReDim sLabels(8)
            'sLabels(0) = "lblOutPressureTag"
            'sLabels(1) = grsRM.GetString("rsOUT_PRESSURE")
            'sLabels(2) = "lblFlowTag"
            'sLabels(3) = grsRM.GetString("rs_CMD_FLOW_CCMIN")
            'sLabels(4) = "lblOutPressure"
            'sLabels(5) = "0.0"
            'sLabels(6) = "lblFlow"
            'sLabels(7) = "0.0"'
            If Not (muctrlToon Is Nothing) Then
                'Let it get all the valve names without loading a user control
                muctrlToon.SetSharedValveLabels(sSharedValveNames)
                muctrlToon.SetGroupValveLabels(sGroupValveNames)
                muctrlToon.SolvHeaderLabel = grsRM.GetString("rsSOLVENT_SUPPLY")
                muctrlToon.AirHeaderLabel = grsRM.GetString("rsAIR_SUPPLY")
                muctrlToon.PaintColorName = grsRM.GetString("rsPAINT_SUPPLY")
                ' muctrlToon.SetAdditionalData(sLabels)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub

    Friend Overrides Function ValidateValveSelection(ByRef nSharedValveStates As Integer, ByRef nGroupValveStates As Integer) As Boolean
        '********************************************************************************************
        'Description: validate the valve states for this applicator
        '
        'Parameters:  valve state selection
        'Returns:     valve state selection with any problems turned off.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return (True)
    End Function

#End Region
End Class  'clsGunCartoon


'********************************************************************************************
'Description: clsHondaWBCartoon Cartoons
'       Good starting point here if you need to copy for a new one that has 2 CC groups
'   Inherits from clsColorChangeCartoon and adds:
'       Declares: num valve constants, only required for > 4 shared or 16 group
'       Properties: num valve properties, only required for > 4 shared or 16 group
'       Routines: ValidateValveSelection and subInitialize
'       To use 1 CC group, see clsVersabellCartoon
'Modification history:
'
' Date      By      Reason
'********************************************************************************************
Friend Class clsHondaWBCartoon
    Inherits clsColorChangeCartoon

#Region " Declares "
    'Expanded valves should be declared here
    Private Const mnNUM_VALVES As Integer = 35
    Private Const mnNUM_SHARED_VALVES As Integer = 4
    Private Const mnNUM_GROUP_VALVES As Integer = 31
#End Region
#Region " Properties for expanded valves "
#End Region
#Region " Routines "
    '********************************************************************************************
    'Description: provide pressure test valve masks for each step by cc type
    '
    'Parameters:  eStep = pressure test step
    'Returns:  nGroup= Group valves, nShared = shared valves   
    '
    'Modification history:
    '
    ' Date      By      Reason
    ' 11/30/09      MSW     Add pressure test "subGetPressureTestValveMask" routine
    '********************************************************************************************
    Friend Overrides Sub subGetPressureTestValveMask(ByVal eStep As ePressureTestStep, ByRef nGroup As Integer, ByRef nShared As Integer)
        'no pressure test for this class
        Select Case eStep
            Case ePressureTestStep.FillSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.FillAir
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureAir
                nShared = 0
                nGroup = 0
            Case Else
                nShared = 0
                nGroup = 0
        End Select
    End Sub
    Friend Overrides Sub subInitialize()
        '********************************************************************************************
        'Description: Initialize clsHondaWBCartoon cartoon
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/15/12  MSW     subInitialize - add NumberOfGroupValves = mApplicator.NumGroupValves
        '********************************************************************************************
        Dim oL As Label = Nothing
        Dim sControl As String = String.Empty
        Try

            MyBase.ColorChangeType = eColorChangeType.HONDA_WB

            If mForm Is Nothing Then
                'must be user control
                muctrlToon = DirectCast(mUctl, uctrlHondaWB)
            Else
                'must be form
                muctrlToon = DirectCast(GetControlByName("uctrlCartoon", mForm), uctrlHondaWB)
            End If

            'get valve names, set colors to green as a default
            'Shared Valves
            'ReDim sSharedValveNames(NumberOfSharedValves)
            NumberOfSharedValves = mApplicator.NumSharedValves
            sSharedValveNames = mApplicator.SharedValves
            ReDim mcSharedValveColors(NumberOfSharedValves)
            'Group Valves
            'ReDim sGroupValveNames(NumberOfGroupValves)
            sGroupValveNames = mApplicator.GroupValves
            NumberOfGroupValves = mApplicator.NumGroupValves
            ReDim mcGroupValveColors(NumberOfGroupValves)
            'subGetValveLabels(mRobot, sSharedValveNames, sGroupValveNames, True)
            For i As Integer = sSharedValveNames.GetLowerBound(0) To sSharedValveNames.GetUpperBound(0)
                mcSharedValveColors(i) = Color.Green
            Next
            For i As Integer = sGroupValveNames.GetLowerBound(0) To sGroupValveNames.GetUpperBound(0)
                mcGroupValveColors(i) = Color.Green
            Next

            'Set solvent and air colors
            'Paint
            mcSharedValveColors(3) = Color.Purple 'pC1_24
            'Solvent
            mcGroupValveColors(3) = Color.Red 'pSOL
            mcGroupValveColors(10) = Color.Red 'pSOL2
            mcGroupValveColors(17) = Color.Red 'pHW
            mcGroupValveColors(18) = Color.Red 'pLW
            mcGroupValveColors(19) = Color.Red 'pWS
            mcGroupValveColors(29) = Color.Red 'pACS
            'Air
            mcGroupValveColors(4) = Color.Blue 'pAIR
            mcGroupValveColors(15) = Color.Blue 'pLA
            mcGroupValveColors(16) = Color.Blue 'pHA
            mcGroupValveColors(28) = Color.Blue 'ACA
            mcGroupValveColors(30) = Color.Blue 'ACVA

            Dim sLabels As String()
            ReDim sLabels(24)
            sLabels(0) = "lblFlow"
            sLabels(1) = "0.0"
            sLabels(2) = "lblCanPos"
            sLabels(3) = "0.0"
            sLabels(4) = "lblCanTorque"
            sLabels(5) = "0.0"
            sLabels(6) = "lblSUnitTorque"
            sLabels(7) = "0.0"
            sLabels(8) = "lblSUnitPosition"
            sLabels(9) = "0"
            sLabels(10) = "lblFlowTag"
            sLabels(11) = grsRM.GetString("rsHWB_CMD_FLOW_CCMIN")
            sLabels(12) = "lblCanPosTag"
            sLabels(13) = grsRM.GetString("rsHWB_CAN_POS")
            sLabels(14) = "lblCanTorqueTag"
            sLabels(15) = grsRM.GetString("rsHWB_CAN_TORQUE")
            sLabels(16) = "lblSUnitTorqueTag"
            sLabels(17) = grsRM.GetString("rsHWB_DOCK_TORQUE")
            sLabels(18) = "lblSUnitPositionTag"
            sLabels(19) = grsRM.GetString("rsHWB_DOCK_POS")
            sLabels(20) = "lblAirHeader02"
            sLabels(21) = grsRM.GetString("rsHWB_LOW_AIR_SUPPLY")
            sLabels(22) = "lblSolventHeader02"
            sLabels(23) = grsRM.GetString("rsHWB_WATERSOLV_SUPPLY")

            If Not (muctrlToon Is Nothing) Then
                'Let it get all the valve names without loading a user control
                muctrlToon.SetSharedValveLabels(sSharedValveNames)
                muctrlToon.SetGroupValveLabels(sGroupValveNames)
                muctrlToon.SolvHeaderLabel = grsRM.GetString("rsHWB_WATER_SUPPLY")
                muctrlToon.AirHeaderLabel = grsRM.GetString("rsHWB_HIGH_AIR_SUPPLY")
                muctrlToon.PaintColorName = grsRM.GetString("rsPAINT_SUPPLY")
                muctrlToon.SetAdditionalData(sLabels)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Overrides Function ValidateValveSelection(ByRef nSharedValveStates As Integer, ByRef nGroupValveStates As Integer) As Boolean
        '********************************************************************************************
        'Description: validate the valve states for this applicator
        '
        'Parameters:  valve state selection
        'Returns:     valve state selection with any problems turned off.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return (True)
    End Function
#End Region
End Class  'clsHondaWBCartoon

'********************************************************************************************
'Description: clsHonda1KCartoon Cartoons
'       Good starting point here if you need to copy for a new one that has 2 CC groups
'   Inherits from clsColorChangeCartoon and adds:
'       Declares: num valve constants, only required for > 4 shared or 16 group
'       Properties: num valve properties, only required for > 4 shared or 16 group
'       Routines: ValidateValveSelection and subInitialize
'       To use 1 CC group, see clsVersabellCartoon
'Modification history:
'
' Date      By      Reason
'********************************************************************************************
Friend Class clsHonda1KCartoon
    Inherits clsColorChangeCartoon

#Region " Declares "
    'Expanded valves should be declared here
    Private Const mnNUM_VALVES As Integer = 35
    Private Const mnNUM_SHARED_VALVES As Integer = 4
    Private Const mnNUM_GROUP_VALVES As Integer = 31
#End Region
#Region " Properties for expanded valves "
#End Region
#Region " Routines "
    '********************************************************************************************
    'Description: provide pressure test valve masks for each step by cc type
    '
    'Parameters:  eStep = pressure test step
    'Returns:  nGroup= Group valves, nShared = shared valves   
    '
    'Modification history:
    '
    ' Date      By      Reason
    ' 11/30/09      MSW     Add pressure test "subGetPressureTestValveMask" routine
    '********************************************************************************************
    Friend Overrides Sub subGetPressureTestValveMask(ByVal eStep As ePressureTestStep, ByRef nGroup As Integer, ByRef nShared As Integer)
        'TODO set valve states for pressure test
        Select Case eStep
            Case ePressureTestStep.FillSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.FillAir
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureAir
                nShared = 0
                nGroup = 0
            Case Else
                nShared = 0
                nGroup = 0
        End Select
    End Sub
    Friend Overrides Sub subInitialize()
        '********************************************************************************************
        'Description: Initialize clsHonda1KCartoon cartoon
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/15/12  MSW     subInitialize - add NumberOfGroupValves = mApplicator.NumGroupValves
        '********************************************************************************************
        Dim oL As Label = Nothing
        Dim sControl As String = String.Empty
        Try

            MyBase.ColorChangeType = eColorChangeType.HONDA_1K

            If mForm Is Nothing Then
                'must be user control
                muctrlToon = DirectCast(mUctl, uctrlhonda1k)
            Else
                'must be form
                muctrlToon = DirectCast(GetControlByName("uctrlCartoon", mForm), uctrlhonda1k)
            End If

            'get valve names, set colors to green as a default
            'Shared Valves
            'ReDim sSharedValveNames(NumberOfSharedValves)
            NumberOfSharedValves = mApplicator.NumSharedValves
            sSharedValveNames = mApplicator.SharedValves
            ReDim mcSharedValveColors(NumberOfSharedValves)
            'Group Valves
            'ReDim sGroupValveNames(NumberOfGroupValves)
            sGroupValveNames = mApplicator.GroupValves
            NumberOfGroupValves = mApplicator.NumGroupValves
            ReDim mcGroupValveColors(NumberOfGroupValves)
            'subGetValveLabels(mRobot, sSharedValveNames, sGroupValveNames, True)
            For i As Integer = sSharedValveNames.GetLowerBound(0) To sSharedValveNames.GetUpperBound(0)
                mcSharedValveColors(i) = Color.Green
            Next
            For i As Integer = sGroupValveNames.GetLowerBound(0) To sGroupValveNames.GetUpperBound(0)
                mcGroupValveColors(i) = Color.Green
            Next

            'Set solvent and air colors
            'Paint
            mcSharedValveColors(3) = Color.Purple
            'Solvent
            mcGroupValveColors(3) = Color.Red
            mcGroupValveColors(7) = Color.Red
            mcGroupValveColors(11) = Color.Red
            mcGroupValveColors(29) = Color.Red
            'Air
            mcGroupValveColors(4) = Color.Blue
            mcGroupValveColors(12) = Color.Blue
            mcGroupValveColors(13) = Color.Blue
            mcGroupValveColors(28) = Color.Blue
            mcGroupValveColors(30) = Color.Blue

            Dim sLabels As String()
            ReDim sLabels(8)
            sLabels(0) = "lblOutPressureTag"
            sLabels(1) = grsRM.GetString("rsOUT_PRESSURE")
            sLabels(2) = "lblFlowTag"
            sLabels(3) = grsRM.GetString("rs_CMD_FLOW_CCMIN")
            sLabels(4) = "lblOutPressure"
            sLabels(5) = "0.0"
            sLabels(6) = "lblFlow"
            sLabels(7) = "0.0"

            If Not (muctrlToon Is Nothing) Then
                'Let it get all the valve names without loading a user control
                muctrlToon.SetSharedValveLabels(sSharedValveNames)
                muctrlToon.SetGroupValveLabels(sGroupValveNames)
                muctrlToon.SolvHeaderLabel = grsRM.GetString("rsSOLVENT_SUPPLY")
                muctrlToon.AirHeaderLabel = grsRM.GetString("rsAIR_SUPPLY")
                muctrlToon.PaintColorName = grsRM.GetString("rsPAINT_SUPPLY")
                muctrlToon.SetAdditionalData(sLabels)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Overrides Function ValidateValveSelection(ByRef nSharedValveStates As Integer, ByRef nGroupValveStates As Integer) As Boolean
        '********************************************************************************************
        'Description: validate the valve states for this applicator
        '
        'Parameters:  valve state selection
        'Returns:     valve state selection with any problems turned off.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return (True)
    End Function
#End Region
End Class  'clsHonda1KCartoon
Friend Class clsVersabell2_32Cartoon
    Inherits clsColorChangeCartoon

#Region " Declares "
    'Expanded valves should be declared here
    Private Const mnNUM_VALVES As Integer = 35
    Private Const mnNUM_SHARED_VALVES As Integer = 4
    Private Const mnNUM_GROUP_VALVES As Integer = 31
#End Region
#Region " Properties for expanded valves "
#End Region
#Region " Routines "
    '********************************************************************************************
    'Description: provide pressure test valve masks for each step by cc type
    '
    'Parameters:  eStep = pressure test step
    'Returns:  nGroup= Group valves, nShared = shared valves   
    '
    'Modification history:
    '
    ' Date      By      Reason
    ' 11/30/09      MSW     Add pressure test "subGetPressureTestValveMask" routine
    '********************************************************************************************
    Friend Overrides Sub subGetPressureTestValveMask(ByVal eStep As ePressureTestStep, ByRef nGroup As Integer, ByRef nShared As Integer)
        'TODO set valve states for pressure test
        Select Case eStep
            Case ePressureTestStep.FillSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.FillAir
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureAir
                nShared = 0
                nGroup = 0
            Case Else
                nShared = 0
                nGroup = 0
        End Select
    End Sub
    Friend Overrides Sub subInitialize()
        '********************************************************************************************
        'Description: Initialize clsVersabell2_32Cartoon cartoon
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/15/10  BTK     Added color change type for Versabell2 32 valves
        ' 03/15/12  MSW     subInitialize - add NumberOfGroupValves = mApplicator.NumGroupValves
        '********************************************************************************************
        Dim oL As Label = Nothing
        Dim sControl As String = String.Empty
        Try

            MyBase.ColorChangeType = eColorChangeType.VERSABELL2_32

            If mForm Is Nothing Then
                'must be user control
                muctrlToon = DirectCast(mUctl, uctrlVersabell2_32)
            Else
                'must be form
                muctrlToon = DirectCast(GetControlByName("uctrlCartoon", mForm), uctrlVersabell2_32)
            End If

            'get valve names, set colors to green as a default
            'Shared Valves
            'ReDim sSharedValveNames(NumberOfSharedValves)
            NumberOfSharedValves = mApplicator.NumSharedValves
            sSharedValveNames = mApplicator.SharedValves
            ReDim mcSharedValveColors(NumberOfSharedValves)
            'Group Valves
            'ReDim sGroupValveNames(NumberOfGroupValves)
            sGroupValveNames = mApplicator.GroupValves
            NumberOfGroupValves = mApplicator.NumGroupValves
            ReDim mcGroupValveColors(NumberOfGroupValves)
            'subGetValveLabels(mRobot, sSharedValveNames, sGroupValveNames, True)
            For i As Integer = sSharedValveNames.GetLowerBound(0) To sSharedValveNames.GetUpperBound(0)
                mcSharedValveColors(i) = Color.Green
            Next
            For i As Integer = sGroupValveNames.GetLowerBound(0) To sGroupValveNames.GetUpperBound(0)
                mcGroupValveColors(i) = Color.Green
            Next

            'Set solvent and air colors
            'Paint
            mcSharedValveColors(3) = Color.Purple
            'Solvent
            mcGroupValveColors(3) = Color.Red
            mcGroupValveColors(29) = Color.Red
            'Air
            mcGroupValveColors(4) = Color.Blue
            mcGroupValveColors(28) = Color.Blue
            mcGroupValveColors(30) = Color.Blue

            Dim sLabels As String()
            ReDim sLabels(8)
            sLabels(0) = "lblOutPressureTag"
            sLabels(1) = grsRM.GetString("rsOUT_PRESSURE")
            sLabels(2) = "lblFlowTag"
            sLabels(3) = grsRM.GetString("rs_CMD_FLOW_CCMIN")
            sLabels(4) = "lblOutPressure"
            sLabels(5) = "0.0"
            sLabels(6) = "lblFlow"
            sLabels(7) = "0.0"

            If Not (muctrlToon Is Nothing) Then
                'Let it get all the valve names without loading a user control
                muctrlToon.SetSharedValveLabels(sSharedValveNames)
                muctrlToon.SetGroupValveLabels(sGroupValveNames)
                muctrlToon.SolvHeaderLabel = grsRM.GetString("rsSOLVENT_SUPPLY")
                muctrlToon.AirHeaderLabel = grsRM.GetString("rsAIR_SUPPLY")
                muctrlToon.PaintColorName = grsRM.GetString("rsPAINT_SUPPLY")
                muctrlToon.SetAdditionalData(sLabels)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Overrides Function ValidateValveSelection(ByRef nSharedValveStates As Integer, ByRef nGroupValveStates As Integer) As Boolean
        '********************************************************************************************
        'Description: validate the valve states for this applicator
        '
        'Parameters:  valve state selection
        'Returns:     valve state selection with any problems turned off.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return (True)
    End Function
#End Region

    Public Sub New()

    End Sub
End Class  'clsVersabell2_32
'********************************************************************************************
'Description: Versabell2plus Cartoons
'       Good starting point here if you need to copy for a new one that has 2 CC groups
'   Inherits from clsColorChangeCartoon and adds:
'       Declares: num valve constants, only required for > 4 shared or 16 group
'       Properties: num valve properties, only required for > 4 shared or 16 group
'       Routines: ValidateValveSelection and subInitialize
'       To use 1 CC group, see clsVersabellCartoon
'Modification history:
'
' Date      By      Reason
'********************************************************************************************
Friend Class clsVersabell2_2kCartoon
    Inherits clsColorChangeCartoon

#Region " Declares "
    'Expanded valves should be declared here
    Private Const mnNUM_VALVES As Integer = 35
    Private Const mnNUM_SHARED_VALVES As Integer = 4
    Private Const mnNUM_GROUP_VALVES As Integer = 31
#End Region
#Region " Properties for expanded valves "
#End Region
#Region " Routines "
    '********************************************************************************************
    'Description: provide pressure test valve masks for each step by cc type
    '
    'Parameters:  eStep = pressure test step
    'Returns:  nGroup= Group valves, nShared = shared valves   
    '
    'Modification history:
    '
    ' Date      By      Reason
    ' 11/30/09      MSW     Add pressure test "subGetPressureTestValveMask" routine
    '********************************************************************************************
    Friend Overrides Sub subGetPressureTestValveMask(ByVal eStep As ePressureTestStep, ByRef nGroup As Integer, ByRef nShared As Integer)
        'TODO set valve states for pressure test
        Select Case eStep
            Case ePressureTestStep.FillSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.FillAir
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureAir
                nShared = 0
                nGroup = 0
            Case Else
                nShared = 0
                nGroup = 0
        End Select
    End Sub
    Friend Overrides Sub subInitialize()
        '********************************************************************************************
        'Description: Initialize clsVersabell2plus2kCartoon cartoon
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/15/12  MSW     subInitialize - add NumberOfGroupValves = mApplicator.NumGroupValves
        '********************************************************************************************
        Dim oL As Label = Nothing
        Dim sControl As String = String.Empty
        Try

            MyBase.ColorChangeType = eColorChangeType.VERSABELL2_2K

            If mForm Is Nothing Then
                'must be user control
                muctrlToon = DirectCast(mUctl, uctrlVersabell2_2k)
            Else
                'must be form
                muctrlToon = DirectCast(GetControlByName("uctrlCartoon", mForm), uctrlVersabell2_2k)
            End If

            'get valve names, set colors to green as a default
            'Shared Valves
            'ReDim sSharedValveNames(NumberOfSharedValves)
            NumberOfSharedValves = mApplicator.NumSharedValves
            sSharedValveNames = mApplicator.SharedValves
            ReDim mcSharedValveColors(NumberOfSharedValves)
            'Group Valves
            'ReDim sGroupValveNames(NumberOfGroupValves)
            sGroupValveNames = mApplicator.GroupValves
            NumberOfGroupValves = mApplicator.NumGroupValves
            ReDim mcGroupValveColors(NumberOfGroupValves)
            'subGetValveLabels(mRobot, sSharedValveNames, sGroupValveNames, True)
            For i As Integer = sSharedValveNames.GetLowerBound(0) To sSharedValveNames.GetUpperBound(0)
                mcSharedValveColors(i) = Color.Green
            Next
            For i As Integer = sGroupValveNames.GetLowerBound(0) To sGroupValveNames.GetUpperBound(0)
                mcGroupValveColors(i) = Color.Green
            Next

            'Set solvent and air colors
            'Paint
            mcSharedValveColors(3) = Color.Purple
            'Solvent
            mcGroupValveColors(3) = Color.Red
            mcGroupValveColors(29) = Color.Red
            'Air
            mcGroupValveColors(4) = Color.Blue
            mcGroupValveColors(28) = Color.Blue
            mcGroupValveColors(30) = Color.Blue

            Dim sLabels As String()
            ReDim sLabels(16)
            sLabels(0) = "lblOutPressureTag"
            sLabels(1) = grsRM.GetString("rsOUT_PRESSURE")
            sLabels(2) = "lblFlowTag"
            sLabels(3) = grsRM.GetString("rs_CMD_FLOW_CCMIN")
            sLabels(4) = "lblOutPressure"
            sLabels(5) = "0.0"
            sLabels(6) = "lblOutPressure2"
            sLabels(7) = "0.0"
            sLabels(8) = "lblFlow"
            sLabels(9) = "0.0"
            sLabels(10) = "lblInPressureTag"
            sLabels(11) = grsRM.GetString("rsIN_PRESSURE")
            sLabels(12) = "lblInPressure"
            sLabels(13) = "0.0"
            sLabels(14) = "lblInPressure2"
            sLabels(15) = "0.0"

            If Not (muctrlToon Is Nothing) Then
                'Let it get all the valve names without loading a user control
                muctrlToon.SetSharedValveLabels(sSharedValveNames)
                muctrlToon.SetGroupValveLabels(sGroupValveNames)
                muctrlToon.SolvHeaderLabel = grsRM.GetString("rsSOLVENT_SUPPLY")
                muctrlToon.AirHeaderLabel = grsRM.GetString("rsAIR_SUPPLY")
                muctrlToon.PaintColorName = grsRM.GetString("rsPAINT_SUPPLY")
                muctrlToon.SetAdditionalData(sLabels)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Overrides Function ValidateValveSelection(ByRef nSharedValveStates As Integer, ByRef nGroupValveStates As Integer) As Boolean
        '********************************************************************************************
        'Description: validate the valve states for this applicator
        '
        'Parameters:  valve state selection
        'Returns:     valve state selection with any problems turned off.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return (True)
    End Function
#End Region
End Class  'clsVersabell2_2kCartoon

'********************************************************************************************
'Description: clsVersabell3dualWBCartoon Cartoons
'   Inherits from clsColorChangeCartoon and adds:
'       Declares: num valve constants, only required for > 4 shared or 16 group
'       Properties: num valve properties, only required for > 4 shared or 16 group
'       Routines: ValidateValveSelection and subInitialize
'       To use 1 CC group, see clsVersabellCartoon
'Modification history:
'
' Date      By      Reason
' 09/16/13  MSW     Add dual can cc type - eColorChangeType.VERSABELL3_DUAL_WB
'********************************************************************************************
Friend Class clsVersabell3dualWBCartoon
    Inherits clsColorChangeCartoon

#Region " Declares "
    'Expanded valves should be declared here
    Private Const mnNUM_VALVES As Integer = 35
    Private Const mnNUM_SHARED_VALVES As Integer = 4
    Private Const mnNUM_GROUP_VALVES As Integer = 31
#End Region
#Region " Properties for expanded valves "
#End Region
#Region " Routines "
    '********************************************************************************************
    'Description: provide pressure test valve masks for each step by cc type
    '
    'Parameters:  eStep = pressure test step
    'Returns:  nGroup= Group valves, nShared = shared valves   
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Friend Overrides Sub subGetPressureTestValveMask(ByVal eStep As ePressureTestStep, ByRef nGroup As Integer, ByRef nShared As Integer)
        'Shared
        'pTrig = 1 
        'Group
        'pPE = 1
        'pSOL = 8
        'pAIR = 16
        'pCC = 32
        'pFLUSH = 128
        Const mnTrigOn As Integer = 1
        Const mnAllOff As Integer = 0
        'flush, pcc, paint enable
        Const mnSolvAirPath As Integer = 161 '128 + 32 + 1
        'Solvent
        Const mnSolvMask As Integer = 8
        'Air, valve
        Const mnAirMask As Integer = 16

        Select Case eStep
            Case ePressureTestStep.FillSolv
                nShared = mnTrigOn 'Fill through trigger
                nGroup = mnSolvAirPath + mnSolvMask
            Case ePressureTestStep.MeasureSolv
                nShared = mnAllOff
                nGroup = mnSolvAirPath + mnSolvMask
            Case ePressureTestStep.FillAir
                nShared = mnTrigOn 'Fill through trigger
                nGroup = mnSolvAirPath + mnAirMask
            Case ePressureTestStep.MeasureAir
                nShared = mnAllOff
                nGroup = mnSolvAirPath + mnAirMask
            Case Else
                nShared = mnAllOff
                nGroup = mnAllOff
        End Select
    End Sub

    Friend Overrides Sub subInitialize()
        '********************************************************************************************
        'Description: Initialize clsVersabell3dualWBCartoon cartoon
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oL As Label = Nothing
        Dim sControl As String = String.Empty
        Try

            MyBase.ColorChangeType = eColorChangeType.VERSABELL3_DUAL_WB

            If mForm Is Nothing Then
                'must be user control
                muctrlToon = DirectCast(mUctl, uctrlVersabell3dualwb)
            Else
                'must be form
                muctrlToon = DirectCast(GetControlByName("uctrlCartoon", mForm), uctrlVersabell3dualwb)
            End If

            'get valve names, set colors to green as a default
            'Shared Valves
            NumberOfSharedValves = mApplicator.NumSharedValves
            sSharedValveNames = mApplicator.SharedValves
            ReDim mcSharedValveColors(NumberOfSharedValves)
            'Group Valves
            sGroupValveNames = mApplicator.GroupValves
            NumberOfGroupValves = mApplicator.NumGroupValves
            ReDim mcGroupValveColors(NumberOfGroupValves)
            For i As Integer = sSharedValveNames.GetLowerBound(0) To sSharedValveNames.GetUpperBound(0)
                mcSharedValveColors(i) = Color.Green
            Next
            For i As Integer = sGroupValveNames.GetLowerBound(0) To sGroupValveNames.GetUpperBound(0)
                mcGroupValveColors(i) = Color.Green
            Next

            'Set solvent and air colors
            'Paint
            mcSharedValveColors(3) = Color.Purple
            'Solvent
            mcGroupValveColors(3) = Color.Red
            mcGroupValveColors(29) = Color.Red
            'Air
            mcGroupValveColors(4) = Color.Blue
            mcGroupValveColors(28) = Color.Blue
            mcGroupValveColors(30) = Color.Blue

            Dim sLabels As String()
            ReDim sLabels(12)
            sLabels(0) = "lblFlow"
            sLabels(1) = "0.0"
            sLabels(2) = "lblCanPos"
            sLabels(3) = "0.0"
            sLabels(4) = "lblCanTorque"
            sLabels(5) = "0.0"
            sLabels(6) = "lblFlowTag"
            sLabels(7) = grsRM.GetString("rs_CMD_FLOW_CCMIN")
            sLabels(8) = "lblCanPosTag"
            sLabels(9) = grsRM.GetString("rsWB_CAN_POS")
            sLabels(10) = "lblCanTorqueTag"
            sLabels(11) = grsRM.GetString("rsWB_CAN_TORQUE")


            If Not (muctrlToon Is Nothing) Then
                'Let it get all the valve names without loading a user control
                muctrlToon.SetSharedValveLabels(sSharedValveNames)
                muctrlToon.SetGroupValveLabels(sGroupValveNames)
                muctrlToon.SolvHeaderLabel = grsRM.GetString("rsSOLVENT_SUPPLY")
                muctrlToon.AirHeaderLabel = grsRM.GetString("rsAIR_SUPPLY")
                muctrlToon.PaintColorName = grsRM.GetString("rsPAINT_SUPPLY")
                muctrlToon.SetAdditionalData(sLabels)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Overrides Function ValidateValveSelection(ByRef nSharedValveStates As Integer, ByRef nGroupValveStates As Integer) As Boolean
        '********************************************************************************************
        'Description: validate the valve states for this applicator
        '
        'Parameters:  valve state selection
        'Returns:     valve state selection with any problems turned off.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return (True)
    End Function
#End Region

    Public Sub New()

    End Sub
End Class  'clsVersabell3dualWBCartoon
'********************************************************************************************
'Description: clsVersabell3SingleWBCartoon Cartoons
'   Inherits from clsColorChangeCartoon and adds:
'       Declares: num valve constants, only required for > 4 shared or 16 group
'       Properties: num valve properties, only required for > 4 shared or 16 group
'       Routines: ValidateValveSelection and subInitialize
'       To use 1 CC group, see clsVersabellCartoon
'Modification history:
'
' Date      By      Reason
' 11/16/13  BTK     Add single can cc type - eColorChangeType.VERSABELL3_WB
'********************************************************************************************
Friend Class clsVersabell3SingleWBCartoon
    Inherits clsColorChangeCartoon

#Region " Declares "
    'Expanded valves should be declared here
    Private Const mnNUM_VALVES As Integer = 35
    Private Const mnNUM_SHARED_VALVES As Integer = 4
    Private Const mnNUM_GROUP_VALVES As Integer = 31
#End Region
#Region " Properties for expanded valves "
#End Region
#Region " Routines "
    '********************************************************************************************
    'Description: provide pressure test valve masks for each step by cc type
    '
    'Parameters:  eStep = pressure test step
    'Returns:  nGroup= Group valves, nShared = shared valves   
    '
    'Modification history:
    '
    ' Date      By      Reason
    '********************************************************************************************
    Friend Overrides Sub subGetPressureTestValveMask(ByVal eStep As ePressureTestStep, ByRef nGroup As Integer, ByRef nShared As Integer)
        'Shared
        'pTrig = 1 
        'Group
        'pPE = 1
        'pSOL = 8
        'pAIR = 16
        'pCC = 32
        'pFLUSH = 128
        Const mnTrigOn As Integer = 1
        Const mnAllOff As Integer = 0
        'flush, pcc, paint enable
        Const mnSolvAirPath As Integer = 161 '128 + 32 + 1
        'Solvent
        Const mnSolvMask As Integer = 8
        'Air, valve
        Const mnAirMask As Integer = 16

        Select Case eStep
            Case ePressureTestStep.FillSolv
                nShared = mnTrigOn 'Fill through trigger
                nGroup = mnSolvAirPath + mnSolvMask
            Case ePressureTestStep.MeasureSolv
                nShared = mnAllOff
                nGroup = mnSolvAirPath + mnSolvMask
            Case ePressureTestStep.FillAir
                nShared = mnTrigOn 'Fill through trigger
                nGroup = mnSolvAirPath + mnAirMask
            Case ePressureTestStep.MeasureAir
                nShared = mnAllOff
                nGroup = mnSolvAirPath + mnAirMask
            Case Else
                nShared = mnAllOff
                nGroup = mnAllOff
        End Select
    End Sub

    Friend Overrides Sub subInitialize()
        '********************************************************************************************
        'Description: Initialize clsVersabell3dualWBCartoon cartoon
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim oL As Label = Nothing
        Dim sControl As String = String.Empty
        Try

            MyBase.ColorChangeType = eColorChangeType.VERSABELL3_WB

            If mForm Is Nothing Then
                'must be user control
                muctrlToon = DirectCast(mUctl, uctrlVersabell3SingleWB)
            Else
                'must be form
                muctrlToon = DirectCast(GetControlByName("uctrlCartoon", mForm), uctrlVersabell3SingleWB)
            End If

            'get valve names, set colors to green as a default
            'Shared Valves
            NumberOfSharedValves = mApplicator.NumSharedValves
            sSharedValveNames = mApplicator.SharedValves
            ReDim mcSharedValveColors(NumberOfSharedValves)
            'Group Valves
            sGroupValveNames = mApplicator.GroupValves
            NumberOfGroupValves = mApplicator.NumGroupValves
            ReDim mcGroupValveColors(NumberOfGroupValves)
            For i As Integer = sSharedValveNames.GetLowerBound(0) To sSharedValveNames.GetUpperBound(0)
                mcSharedValveColors(i) = Color.Green
            Next
            For i As Integer = sGroupValveNames.GetLowerBound(0) To sGroupValveNames.GetUpperBound(0)
                mcGroupValveColors(i) = Color.Green
            Next

            'Set solvent and air colors
            'Paint
            mcSharedValveColors(3) = Color.Purple
            'Solvent
            mcGroupValveColors(3) = Color.Red
            mcGroupValveColors(29) = Color.Red
            'Air
            mcGroupValveColors(4) = Color.Blue
            mcGroupValveColors(28) = Color.Blue
            mcGroupValveColors(30) = Color.Blue

            Dim sLabels As String()
            ReDim sLabels(12)
            sLabels(0) = "lblFlow"
            sLabels(1) = "0.0"
            sLabels(2) = "lblCanPos"
            sLabels(3) = "0.0"
            sLabels(4) = "lblCanTorque"
            sLabels(5) = "0.0"
            sLabels(6) = "lblFlowTag"
            sLabels(7) = grsRM.GetString("rs_CMD_FLOW_CCMIN")
            sLabels(8) = "lblCanPosTag"
            sLabels(9) = grsRM.GetString("rsWB_CAN_POS")
            sLabels(10) = "lblCanTorqueTag"
            sLabels(11) = grsRM.GetString("rsWB_CAN_TORQUE")


            If Not (muctrlToon Is Nothing) Then
                'Let it get all the valve names without loading a user control
                muctrlToon.SetSharedValveLabels(sSharedValveNames)
                muctrlToon.SetGroupValveLabels(sGroupValveNames)
                muctrlToon.SolvHeaderLabel = grsRM.GetString("rsSOLVENT_SUPPLY")
                muctrlToon.AirHeaderLabel = grsRM.GetString("rsAIR_SUPPLY")
                muctrlToon.PaintColorName = grsRM.GetString("rsPAINT_SUPPLY")
                muctrlToon.SetAdditionalData(sLabels)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Overrides Function ValidateValveSelection(ByRef nSharedValveStates As Integer, ByRef nGroupValveStates As Integer) As Boolean
        '********************************************************************************************
        'Description: validate the valve states for this applicator
        '
        'Parameters:  valve state selection
        'Returns:     valve state selection with any problems turned off.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return (True)
    End Function
#End Region

    Public Sub New()

    End Sub
End Class  'clsVersabell3SingleWBCartoon
'********************************************************************************************
'Description: Versabell3 Cartoons
'       Good starting point here if you need to copy for a new one that has 2 CC groups
'   Inherits from clsColorChangeCartoon and adds:
'       Declares: num valve constants, only required for > 4 shared or 16 group
'       Properties: num valve properties, only required for > 4 shared or 16 group
'       Routines: ValidateValveSelection and subInitialize
'       To use 1 CC group, see clsVersabellCartoon
'Modification history:
'
' Date      By      Reason
'********************************************************************************************
Friend Class clsVersabell3Cartoon
    Inherits clsColorChangeCartoon

#Region " Declares "
    'Expanded valves should be declared here
    Private Const mnNUM_VALVES As Integer = 35
    Private Const mnNUM_SHARED_VALVES As Integer = 4
    Private Const mnNUM_GROUP_VALVES As Integer = 31
#End Region
#Region " Properties for expanded valves "
#End Region
#Region " Routines "
    '********************************************************************************************
    'Description: provide pressure test valve masks for each step by cc type
    '
    'Parameters:  eStep = pressure test step
    'Returns:  nGroup= Group valves, nShared = shared valves   
    '
    'Modification history:
    '
    ' Date      By      Reason
    ' 11/30/09      MSW     Add pressure test "subGetPressureTestValveMask" routine
    '********************************************************************************************
    Friend Overrides Sub subGetPressureTestValveMask(ByVal eStep As ePressureTestStep, ByRef nGroup As Integer, ByRef nShared As Integer)
        'TODO set valve states for pressure test
        Select Case eStep
            Case ePressureTestStep.FillSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.FillAir
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureAir
                nShared = 0
                nGroup = 0
            Case Else
                nShared = 0
                nGroup = 0
        End Select
    End Sub
    Friend Overrides Sub subInitialize()
        '********************************************************************************************
        'Description: Initialize clsVersabell2plusCartoon cartoon
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/15/12  MSW     subInitialize - add NumberOfGroupValves = mApplicator.NumGroupValves
        '********************************************************************************************
        Dim oL As Label = Nothing
        Dim sControl As String = String.Empty
        Try

            MyBase.ColorChangeType = eColorChangeType.VERSABELL3

            If mForm Is Nothing Then
                'must be user control
                muctrlToon = DirectCast(mUctl, uctrlVersabell3)
            Else
                'must be form
                muctrlToon = DirectCast(GetControlByName("uctrlCartoon", mForm), uctrlVersabell3)
            End If

            'get valve names, set colors to green as a default
            'Shared Valves
            'ReDim sSharedValveNames(NumberOfSharedValves)
            NumberOfSharedValves = mApplicator.NumSharedValves
            sSharedValveNames = mApplicator.SharedValves
            ReDim mcSharedValveColors(NumberOfSharedValves)
            'Group Valves
            'ReDim sGroupValveNames(NumberOfGroupValves)
            sGroupValveNames = mApplicator.GroupValves
            NumberOfGroupValves = mApplicator.NumGroupValves
            ReDim mcGroupValveColors(NumberOfGroupValves)
            'subGetValveLabels(mRobot, sSharedValveNames, sGroupValveNames, True)
            For i As Integer = sSharedValveNames.GetLowerBound(0) To sSharedValveNames.GetUpperBound(0)
                mcSharedValveColors(i) = Color.Green
            Next
            For i As Integer = sGroupValveNames.GetLowerBound(0) To sGroupValveNames.GetUpperBound(0)
                mcGroupValveColors(i) = Color.Green
            Next

            'Set solvent and air colors
            'Paint
            mcSharedValveColors(3) = Color.Purple
            'Solvent
            mcGroupValveColors(3) = Color.Red
            mcGroupValveColors(29) = Color.Red
            'Air
            mcGroupValveColors(4) = Color.Blue
            mcGroupValveColors(28) = Color.Blue
            mcGroupValveColors(30) = Color.Blue

            Dim sLabels As String()
            ReDim sLabels(8)
            sLabels(0) = "lblOutPressureTag"
            sLabels(1) = grsRM.GetString("rsOUT_PRESSURE")
            sLabels(2) = "lblFlowTag"
            sLabels(3) = grsRM.GetString("rs_CMD_FLOW_CCMIN")
            sLabels(4) = "lblOutPressure"
            sLabels(5) = "0.0"
            sLabels(6) = "lblFlow"
            sLabels(7) = "0.0"

            If Not (muctrlToon Is Nothing) Then
                'Let it get all the valve names without loading a user control
                muctrlToon.SetSharedValveLabels(sSharedValveNames)
                muctrlToon.SetGroupValveLabels(sGroupValveNames)
                muctrlToon.SolvHeaderLabel = grsRM.GetString("rsSOLVENT_SUPPLY")
                muctrlToon.AirHeaderLabel = grsRM.GetString("rsAIR_SUPPLY")
                muctrlToon.PaintColorName = grsRM.GetString("rsPAINT_SUPPLY")
                muctrlToon.SetAdditionalData(sLabels)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Overrides Function ValidateValveSelection(ByRef nSharedValveStates As Integer, ByRef nGroupValveStates As Integer) As Boolean
        '********************************************************************************************
        'Description: validate the valve states for this applicator
        '
        'Parameters:  valve state selection
        'Returns:     valve state selection with any problems turned off.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return (True)
    End Function
#End Region
End Class  'clsVersabell3Cartoon

'********************************************************************************************
'Description: Versabell3 Cartoons
'       Good starting point here if you need to copy for a new one that has 2 CC groups
'   Inherits from clsColorChangeCartoon and adds:
'       Declares: num valve constants, only required for > 4 shared or 16 group
'       Properties: num valve properties, only required for > 4 shared or 16 group
'       Routines: ValidateValveSelection and subInitialize
'       To use 1 CC group, see clsVersabellCartoon
'Modification history:
'
' Date      By      Reason
'********************************************************************************************
Friend Class clsVersabell3P1000Cartoon
    Inherits clsColorChangeCartoon

#Region " Declares "
    'Expanded valves should be declared here
    Private Const mnNUM_VALVES As Integer = 35
    Private Const mnNUM_SHARED_VALVES As Integer = 4
    Private Const mnNUM_GROUP_VALVES As Integer = 31
#End Region
#Region " Properties for expanded valves "
#End Region
#Region " Routines "
    '********************************************************************************************
    'Description: provide pressure test valve masks for each step by cc type
    '
    'Parameters:  eStep = pressure test step
    'Returns:  nGroup= Group valves, nShared = shared valves   
    '
    'Modification history:
    '
    ' Date      By      Reason
    ' 11/30/09      MSW     Add pressure test "subGetPressureTestValveMask" routine
    '********************************************************************************************
    Friend Overrides Sub subGetPressureTestValveMask(ByVal eStep As ePressureTestStep, ByRef nGroup As Integer, ByRef nShared As Integer)
        'TODO set valve states for pressure test
        Select Case eStep
            Case ePressureTestStep.FillSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.FillAir
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureAir
                nShared = 0
                nGroup = 0
            Case Else
                nShared = 0
                nGroup = 0
        End Select
    End Sub
    Friend Overrides Sub subInitialize()
        '********************************************************************************************
        'Description: Initialize clsVersabell2plusCartoon cartoon
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/15/12  MSW     subInitialize - add NumberOfGroupValves = mApplicator.NumGroupValves
        '********************************************************************************************
        Dim oL As Label = Nothing
        Dim sControl As String = String.Empty
        Try

            MyBase.ColorChangeType = eColorChangeType.VERSABELL3P1000

            If mForm Is Nothing Then
                'must be user control
                muctrlToon = DirectCast(mUctl, uctrlVersabell3P1000)
            Else
                'must be form
                muctrlToon = DirectCast(GetControlByName("uctrlCartoon", mForm), uctrlVersabell3P1000)
            End If

            'get valve names, set colors to green as a default
            'Shared Valves
            'ReDim sSharedValveNames(NumberOfSharedValves)
            NumberOfSharedValves = mApplicator.NumSharedValves
            sSharedValveNames = mApplicator.SharedValves
            ReDim mcSharedValveColors(NumberOfSharedValves)
            'Group Valves
            'ReDim sGroupValveNames(NumberOfGroupValves)
            sGroupValveNames = mApplicator.GroupValves
            NumberOfGroupValves = mApplicator.NumGroupValves
            ReDim mcGroupValveColors(NumberOfGroupValves)
            'subGetValveLabels(mRobot, sSharedValveNames, sGroupValveNames, True)
            For i As Integer = sSharedValveNames.GetLowerBound(0) To sSharedValveNames.GetUpperBound(0)
                mcSharedValveColors(i) = Color.Green
            Next
            For i As Integer = sGroupValveNames.GetLowerBound(0) To sGroupValveNames.GetUpperBound(0)
                mcGroupValveColors(i) = Color.Green
            Next

            'Set solvent and air colors
            'Paint
            mcSharedValveColors(3) = Color.Purple
            'Solvent
            mcGroupValveColors(3) = Color.Red
            mcGroupValveColors(29) = Color.Red
            'Air
            mcGroupValveColors(4) = Color.Blue
            mcGroupValveColors(28) = Color.Blue
            mcGroupValveColors(30) = Color.Blue

            Dim sLabels As String()
            ReDim sLabels(8)
            sLabels(0) = "lblOutPressureTag"
            sLabels(1) = grsRM.GetString("rsOUT_PRESSURE")
            sLabels(2) = "lblFlowTag"
            sLabels(3) = grsRM.GetString("rs_CMD_FLOW_CCMIN")
            sLabels(4) = "lblOutPressure"
            sLabels(5) = "0.0"
            sLabels(6) = "lblFlow"
            sLabels(7) = "0.0"

            If Not (muctrlToon Is Nothing) Then
                'Let it get all the valve names without loading a user control
                muctrlToon.SetSharedValveLabels(sSharedValveNames)
                muctrlToon.SetGroupValveLabels(sGroupValveNames)
                muctrlToon.SolvHeaderLabel = grsRM.GetString("rsSOLVENT_SUPPLY")
                muctrlToon.AirHeaderLabel = grsRM.GetString("rsAIR_SUPPLY")
                muctrlToon.PaintColorName = grsRM.GetString("rsPAINT_SUPPLY")
                muctrlToon.SetAdditionalData(sLabels)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Overrides Function ValidateValveSelection(ByRef nSharedValveStates As Integer, ByRef nGroupValveStates As Integer) As Boolean
        '********************************************************************************************
        'Description: validate the valve states for this applicator
        '
        'Parameters:  valve state selection
        'Returns:     valve state selection with any problems turned off.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return (True)
    End Function
#End Region

    Public Sub New()

    End Sub
End Class  'clsVersabell3P1000Cartoon

'********************************************************************************************
'Description: Gun 2K Cartoons
'       
'Modification history:
'
' Date 20160909     By MAS     Reason Initial Gun 2k
'********************************************************************************************
Friend Class clsGun_2kCartoon
    Inherits clsColorChangeCartoon

#Region " Declares "
    'Expanded valves should be declared here
    Private Const mnNUM_VALVES As Integer = 35
    Private Const mnNUM_SHARED_VALVES As Integer = 4
    Private Const mnNUM_GROUP_VALVES As Integer = 31
#End Region
#Region " Properties for expanded valves "
#End Region
#Region " Routines "
    '********************************************************************************************
    'Description: provide pressure test valve masks for each step by cc type
    '
    'Parameters:  eStep = pressure test step
    'Returns:  nGroup= Group valves, nShared = shared valves   
    '
    'Modification history:
    '
    ' Date      By      Reason
    ' 11/30/09      MSW     Add pressure test "subGetPressureTestValveMask" routine
    '********************************************************************************************
    Friend Overrides Sub subGetPressureTestValveMask(ByVal eStep As ePressureTestStep, ByRef nGroup As Integer, ByRef nShared As Integer)
        'TODO set valve states for pressure test
        Select Case eStep
            Case ePressureTestStep.FillSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.FillAir
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureAir
                nShared = 0
                nGroup = 0
            Case Else
                nShared = 0
                nGroup = 0
        End Select
    End Sub
    Friend Overrides Sub subInitialize()
        '********************************************************************************************
        'Description: Initialize clsGun_2kCartoon cartoon
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/15/12  MSW     subInitialize - add NumberOfGroupValves = mApplicator.NumGroupValves
        '********************************************************************************************
        Dim oL As Label = Nothing
        Dim sControl As String = String.Empty
        Try

            MyBase.ColorChangeType = eColorChangeType.GUN_2K

            If mForm Is Nothing Then
                'must be user control
                muctrlToon = DirectCast(mUctl, uctrlGun_2k)
            Else
                'must be form
                muctrlToon = DirectCast(GetControlByName("uctrlCartoon", mForm), uctrlGun_2k)
            End If

            'get valve names, set colors to green as a default
            'Shared Valves
            'ReDim sSharedValveNames(NumberOfSharedValves)
            NumberOfSharedValves = mApplicator.NumSharedValves
            sSharedValveNames = mApplicator.SharedValves
            ReDim mcSharedValveColors(NumberOfSharedValves)
            'Group Valves
            'ReDim sGroupValveNames(NumberOfGroupValves)
            sGroupValveNames = mApplicator.GroupValves
            NumberOfGroupValves = mApplicator.NumGroupValves
            ReDim mcGroupValveColors(NumberOfGroupValves)
            'subGetValveLabels(mRobot, sSharedValveNames, sGroupValveNames, True)
            For i As Integer = sSharedValveNames.GetLowerBound(0) To sSharedValveNames.GetUpperBound(0)
                mcSharedValveColors(i) = Color.Green
            Next
            For i As Integer = sGroupValveNames.GetLowerBound(0) To sGroupValveNames.GetUpperBound(0)
                mcGroupValveColors(i) = Color.Green
            Next

            'Set solvent and air colors
            'Paint
            mcSharedValveColors(3) = Color.Purple
            'Solvent
            mcGroupValveColors(3) = Color.Red
            mcGroupValveColors(29) = Color.Red
            'Air
            mcGroupValveColors(4) = Color.Blue
            mcGroupValveColors(28) = Color.Blue
            mcGroupValveColors(30) = Color.Blue

            Dim sLabels As String()
            ReDim sLabels(16)
            sLabels(0) = "lblOutPressureTag"
            sLabels(1) = grsRM.GetString("rsOUT_PRESSURE")
            sLabels(2) = "lblFlowTag"
            sLabels(3) = grsRM.GetString("rs_CMD_FLOW_CCMIN")
            sLabels(4) = "lblOutPressure"
            sLabels(5) = "0.0"
            sLabels(6) = "lblOutPressure2"
            sLabels(7) = "0.0"
            sLabels(8) = "lblFlow"
            sLabels(9) = "0.0"
            sLabels(10) = "lblInPressureTag"
            sLabels(11) = grsRM.GetString("rsIN_PRESSURE")
            sLabels(12) = "lblInPressure"
            sLabels(13) = "0.0"
            sLabels(14) = "lblInPressure2"
            sLabels(15) = "0.0"

            If Not (muctrlToon Is Nothing) Then
                'Let it get all the valve names without loading a user control
                muctrlToon.SetSharedValveLabels(sSharedValveNames)
                muctrlToon.SetGroupValveLabels(sGroupValveNames)
                muctrlToon.SolvHeaderLabel = grsRM.GetString("rsSOLVENT_SUPPLY")
                muctrlToon.AirHeaderLabel = grsRM.GetString("rsAIR_SUPPLY")
                muctrlToon.PaintColorName = grsRM.GetString("rsPAINT_SUPPLY")
                muctrlToon.SetAdditionalData(sLabels)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Overrides Function ValidateValveSelection(ByRef nSharedValveStates As Integer, ByRef nGroupValveStates As Integer) As Boolean
        '********************************************************************************************
        'Description: validate the valve states for this applicator
        '
        'Parameters:  valve state selection
        'Returns:     valve state selection with any problems turned off.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return (True)
    End Function
#End Region

    Private Sub clsGun_2kCartoon_GroupValveClicked(ByVal nValve As Integer, ByVal bCurState As Boolean) Handles Me.GroupValveClicked

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class  'clsGun_2kCartoon

'NVSP 12/05/2016 Added a new class to accomodate piggable stack
Friend Class clsGun_2kCartoon_Pig
    Inherits clsColorChangeCartoon

#Region " Declares "
    'Expanded valves should be declared here
    Private Const mnNUM_VALVES As Integer = 37
    Private Const mnNUM_SHARED_VALVES As Integer = 6 'NVSP 12/05/2016 Changed to accomodate piggable stack
    Private Const mnNUM_GROUP_VALVES As Integer = 31
#End Region
#Region " Properties for expanded valves "
#End Region
#Region " Routines "
    '********************************************************************************************
    'Description: provide pressure test valve masks for each step by cc type
    '
    'Parameters:  eStep = pressure test step
    'Returns:  nGroup= Group valves, nShared = shared valves   
    '
    'Modification history:
    '
    ' Date      By      Reason
    ' 11/30/09      MSW     Add pressure test "subGetPressureTestValveMask" routine
    '********************************************************************************************
    Friend Overrides Sub subGetPressureTestValveMask(ByVal eStep As ePressureTestStep, ByRef nGroup As Integer, ByRef nShared As Integer)
        'TODO set valve states for pressure test
        Select Case eStep
            Case ePressureTestStep.FillSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.FillAir
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureAir
                nShared = 0
                nGroup = 0
            Case Else
                nShared = 0
                nGroup = 0
        End Select
    End Sub
    Friend Overrides Sub subInitialize()
        '********************************************************************************************
        'Description: Initialize clsGun_2kCartoon cartoon
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/15/12  MSW     subInitialize - add NumberOfGroupValves = mApplicator.NumGroupValves
        '********************************************************************************************
        Dim oL As Label = Nothing
        Dim sControl As String = String.Empty
        Try

            MyBase.ColorChangeType = eColorChangeType.GUN_2K_PIG

            If mForm Is Nothing Then
                'must be user control
                muctrlToon = DirectCast(mUctl, uctrlGun_2k_Pig)
            Else
                'must be form
                muctrlToon = DirectCast(GetControlByName("uctrlCartoon", mForm), uctrlGun_2k_Pig)
            End If

            'get valve names, set colors to green as a default
            'Shared Valves
            'ReDim sSharedValveNames(NumberOfSharedValves)
            NumberOfSharedValves = mApplicator.NumSharedValves
            sSharedValveNames = mApplicator.SharedValves
            ReDim mcSharedValveColors(NumberOfSharedValves)
            'Group Valves
            'ReDim sGroupValveNames(NumberOfGroupValves)
            sGroupValveNames = mApplicator.GroupValves
            NumberOfGroupValves = mApplicator.NumGroupValves
            ReDim mcGroupValveColors(NumberOfGroupValves)
            'subGetValveLabels(mRobot, sSharedValveNames, sGroupValveNames, True)
            For i As Integer = sSharedValveNames.GetLowerBound(0) To sSharedValveNames.GetUpperBound(0)
                mcSharedValveColors(i) = Color.Green
            Next
            For i As Integer = sGroupValveNames.GetLowerBound(0) To sGroupValveNames.GetUpperBound(0)
                mcGroupValveColors(i) = Color.Green
            Next

            'Set solvent and air colors
            'Paint
            mcSharedValveColors(3) = Color.Purple
            'Solvent
            mcGroupValveColors(3) = Color.Red
            mcGroupValveColors(29) = Color.Red
            'Air
            mcGroupValveColors(4) = Color.Blue
            mcGroupValveColors(28) = Color.Blue
            mcGroupValveColors(30) = Color.Blue

            Dim sLabels As String()
            ReDim sLabels(16)
            sLabels(0) = "lblOutPressureTag"
            sLabels(1) = grsRM.GetString("rsOUT_PRESSURE")
            sLabels(2) = "lblFlowTag"
            sLabels(3) = grsRM.GetString("rs_CMD_FLOW_CCMIN")
            sLabels(4) = "lblOutPressure"
            sLabels(5) = "0.0"
            sLabels(6) = "lblOutPressure2"
            sLabels(7) = "0.0"
            sLabels(8) = "lblFlow"
            sLabels(9) = "0.0"
            sLabels(10) = "lblInPressureTag"
            sLabels(11) = grsRM.GetString("rsIN_PRESSURE")
            sLabels(12) = "lblInPressure"
            sLabels(13) = "0.0"
            sLabels(14) = "lblInPressure2"
            sLabels(15) = "0.0"

            If Not (muctrlToon Is Nothing) Then
                'Let it get all the valve names without loading a user control
                muctrlToon.SetSharedValveLabels(sSharedValveNames)
                muctrlToon.SetGroupValveLabels(sGroupValveNames)
                muctrlToon.SolvHeaderLabel = grsRM.GetString("rsSOLVENT_SUPPLY")
                muctrlToon.AirHeaderLabel = grsRM.GetString("rsAIR_SUPPLY")
                muctrlToon.PaintColorName = grsRM.GetString("rsPAINT_SUPPLY")
                muctrlToon.SetAdditionalData(sLabels)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Overrides Function ValidateValveSelection(ByRef nSharedValveStates As Integer, ByRef nGroupValveStates As Integer) As Boolean
        '********************************************************************************************
        'Description: validate the valve states for this applicator
        '
        'Parameters:  valve state selection
        'Returns:     valve state selection with any problems turned off.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return (True)
    End Function
#End Region

    Private Sub clsGun_2kCartoon_PIG_GroupValveClicked(ByVal nValve As Integer, ByVal bCurState As Boolean) Handles Me.GroupValveClicked

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
'********************************************************************************************
'Description: Gun 2K Mica
'       
'Modification history:
'
' Date 20160909     By MAS     Reason Initial Gun 2k
'12/06/16           JZ         Initial Gun 2k Mica
'********************************************************************************************
Friend Class clsGun_2k_MicaCartoon
    Inherits clsColorChangeCartoon

#Region " Declares "
    'Expanded valves should be declared here
    Private Const mnNUM_VALVES As Integer = 37
    Private Const mnNUM_SHARED_VALVES As Integer = 6 'NVSP 12/05/2016 Changed to accomodate piggable stack
    Private Const mnNUM_GROUP_VALVES As Integer = 31
#End Region
#Region " Properties for expanded valves "
#End Region
#Region " Routines "
    '********************************************************************************************
    'Description: provide pressure test valve masks for each step by cc type
    '
    'Parameters:  eStep = pressure test step
    'Returns:  nGroup= Group valves, nShared = shared valves   
    '
    'Modification history:
    '
    ' Date      By      Reason
    ' 11/30/09      MSW     Add pressure test "subGetPressureTestValveMask" routine
    '********************************************************************************************
    Friend Overrides Sub subGetPressureTestValveMask(ByVal eStep As ePressureTestStep, ByRef nGroup As Integer, ByRef nShared As Integer)
        'TODO set valve states for pressure test
        Select Case eStep
            Case ePressureTestStep.FillSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.FillAir
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureAir
                nShared = 0
                nGroup = 0
            Case Else
                nShared = 0
                nGroup = 0
        End Select
    End Sub
    Friend Overrides Sub subInitialize()
        '********************************************************************************************
        'Description: Initialize clsGun_2kCartoon cartoon
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 03/15/12  MSW     subInitialize - add NumberOfGroupValves = mApplicator.NumGroupValves
        '********************************************************************************************
        Dim oL As Label = Nothing
        Dim sControl As String = String.Empty
        Try

            MyBase.ColorChangeType = eColorChangeType.GUN_2K_Mica

            If mForm Is Nothing Then
                'must be user control
                muctrlToon = DirectCast(mUctl, uctrlGun_2k_Mica)
            Else
                'must be form
                muctrlToon = DirectCast(GetControlByName("uctrlCartoon", mForm), uctrlGun_2k_Mica)
            End If

            'get valve names, set colors to green as a default
            'Shared Valves
            'ReDim sSharedValveNames(NumberOfSharedValves)
            NumberOfSharedValves = mApplicator.NumSharedValves
            sSharedValveNames = mApplicator.SharedValves
            ReDim mcSharedValveColors(NumberOfSharedValves)
            'Group Valves
            'ReDim sGroupValveNames(NumberOfGroupValves)
            sGroupValveNames = mApplicator.GroupValves
            NumberOfGroupValves = mApplicator.NumGroupValves
            ReDim mcGroupValveColors(NumberOfGroupValves)
            'subGetValveLabels(mRobot, sSharedValveNames, sGroupValveNames, True)
            For i As Integer = sSharedValveNames.GetLowerBound(0) To sSharedValveNames.GetUpperBound(0)
                mcSharedValveColors(i) = Color.Green
            Next
            For i As Integer = sGroupValveNames.GetLowerBound(0) To sGroupValveNames.GetUpperBound(0)
                mcGroupValveColors(i) = Color.Green
            Next

            'Set solvent and air colors
            'Paint
            mcSharedValveColors(3) = Color.Purple
            'Solvent
            mcGroupValveColors(3) = Color.Red
            mcGroupValveColors(29) = Color.Red
            'Air
            mcGroupValveColors(4) = Color.Blue
            mcGroupValveColors(28) = Color.Blue
            mcGroupValveColors(30) = Color.Blue

            Dim sLabels As String()
            ReDim sLabels(16)
            sLabels(0) = "lblOutPressureTag"
            sLabels(1) = grsRM.GetString("rsOUT_PRESSURE")
            sLabels(2) = "lblFlowTag"
            sLabels(3) = grsRM.GetString("rs_CMD_FLOW_CCMIN")
            sLabels(4) = "lblOutPressure"
            sLabels(5) = "0.0"
            sLabels(6) = "lblOutPressure2"
            sLabels(7) = "0.0"
            sLabels(8) = "lblFlow"
            sLabels(9) = "0.0"
            sLabels(10) = "lblInPressureTag"
            sLabels(11) = grsRM.GetString("rsIN_PRESSURE")
            sLabels(12) = "lblInPressure"
            sLabels(13) = "0.0"
            sLabels(14) = "lblInPressure2"
            sLabels(15) = "0.0"

            If Not (muctrlToon Is Nothing) Then
                'Let it get all the valve names without loading a user control
                muctrlToon.SetSharedValveLabels(sSharedValveNames)
                muctrlToon.SetGroupValveLabels(sGroupValveNames)
                muctrlToon.SolvHeaderLabel = grsRM.GetString("rsSOLVENT_SUPPLY")
                muctrlToon.AirHeaderLabel = grsRM.GetString("rsAIR_SUPPLY")
                muctrlToon.PaintColorName = grsRM.GetString("rsPAINT_SUPPLY")
                muctrlToon.SetAdditionalData(sLabels)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Overrides Function ValidateValveSelection(ByRef nSharedValveStates As Integer, ByRef nGroupValveStates As Integer) As Boolean
        '********************************************************************************************
        'Description: validate the valve states for this applicator
        '
        'Parameters:  valve state selection
        'Returns:     valve state selection with any problems turned off.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return (True)
    End Function
#End Region

    Private Sub clsGun_2kCartoon_Mica_GroupValveClicked(ByVal nValve As Integer, ByVal bCurState As Boolean) Handles Me.GroupValveClicked

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
Friend Class clsGunPumpCartoon
    Inherits clsColorChangeCartoon

#Region " Declares "
    'Expanded valves should be declared here
    Private Const mnNUM_VALVES As Integer = 35
    Private Const mnNUM_SHARED_VALVES As Integer = 4
    Private Const mnNUM_GROUP_VALVES As Integer = 31
#End Region
#Region " Properties for expanded valves "
#End Region
#Region " Routines "
    '********************************************************************************************
    'Description: provide pressure test valve masks for each step by cc type
    '
    'Parameters:  eStep = pressure test step
    'Returns:  nGroup= Group valves, nShared = shared valves   
    '
    'Modification history:
    '
    ' Date      By      Reason
    ' 11/30/09      MSW     Add pressure test "subGetPressureTestValveMask" routine
    '********************************************************************************************
    Friend Overrides Sub subGetPressureTestValveMask(ByVal eStep As ePressureTestStep, ByRef nGroup As Integer, ByRef nShared As Integer)
        'TODO set valve states for pressure test
        Select Case eStep
            Case ePressureTestStep.FillSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureSolv
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.FillAir
                nShared = 0
                nGroup = 0
            Case ePressureTestStep.MeasureAir
                nShared = 0
                nGroup = 0
            Case Else
                nShared = 0
                nGroup = 0
        End Select
    End Sub
    Friend Overrides Sub subInitialize()
        '********************************************************************************************
        'Description: Initialize clsVersabell2_32Cartoon cartoon
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/15/10  BTK     Added color change type for Versabell2 32 valves
        ' 03/15/12  MSW     subInitialize - add NumberOfGroupValves = mApplicator.NumGroupValves
        '********************************************************************************************
        Dim oL As Label = Nothing
        Dim sControl As String = String.Empty
        Try

            MyBase.ColorChangeType = eColorChangeType.GUN_1K


            If mForm Is Nothing Then
                'must be user control
                muctrlToon = DirectCast(mUctl, uctrlGunPump)
            Else
                'must be form
                muctrlToon = DirectCast(GetControlByName("uctrlCartoon", mForm), uctrlGunPump)
            End If

            'get valve names, set colors to green as a default
            'Shared Valves
            'ReDim sSharedValveNames(NumberOfSharedValves)
            NumberOfSharedValves = mApplicator.NumSharedValves
            sSharedValveNames = mApplicator.SharedValves
            ReDim mcSharedValveColors(NumberOfSharedValves)
            'Group Valves
            'ReDim sGroupValveNames(NumberOfGroupValves)
            sGroupValveNames = mApplicator.GroupValves
            NumberOfGroupValves = mApplicator.NumGroupValves
            ReDim mcGroupValveColors(NumberOfGroupValves)
            'subGetValveLabels(mRobot, sSharedValveNames, sGroupValveNames, True)
            For i As Integer = sSharedValveNames.GetLowerBound(0) To sSharedValveNames.GetUpperBound(0)
                mcSharedValveColors(i) = Color.Green
            Next
            For i As Integer = sGroupValveNames.GetLowerBound(0) To sGroupValveNames.GetUpperBound(0)
                mcGroupValveColors(i) = Color.Green
            Next

            'Set solvent and air colors
            'Paint
            mcSharedValveColors(3) = Color.Purple
            'Solvent
            mcGroupValveColors(3) = Color.Red
            mcGroupValveColors(29) = Color.Red
            'Air
            mcGroupValveColors(4) = Color.Blue
            mcGroupValveColors(28) = Color.Blue
            mcGroupValveColors(30) = Color.Blue

            Dim sLabels As String()
            ReDim sLabels(8)
            sLabels(0) = "lblOutPressureTag"
            sLabels(1) = grsRM.GetString("rsOUT_PRESSURE")
            sLabels(2) = "lblFlowTag"
            sLabels(3) = grsRM.GetString("rs_CMD_FLOW_CCMIN")
            sLabels(4) = "lblOutPressure"
            sLabels(5) = "0.0"
            sLabels(6) = "lblFlow"
            sLabels(7) = "0.0"

            If Not (muctrlToon Is Nothing) Then
                'Let it get all the valve names without loading a user control
                muctrlToon.SetSharedValveLabels(sSharedValveNames)
                muctrlToon.SetGroupValveLabels(sGroupValveNames)
                muctrlToon.SolvHeaderLabel = grsRM.GetString("rsSOLVENT_SUPPLY")
                muctrlToon.AirHeaderLabel = grsRM.GetString("rsAIR_SUPPLY")
                muctrlToon.PaintColorName = grsRM.GetString("rsPAINT_SUPPLY")
                muctrlToon.SetAdditionalData(sLabels)
            End If

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Overrides Function ValidateValveSelection(ByRef nSharedValveStates As Integer, ByRef nGroupValveStates As Integer) As Boolean
        '********************************************************************************************
        'Description: validate the valve states for this applicator
        '
        'Parameters:  valve state selection
        'Returns:     valve state selection with any problems turned off.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Return (True)
    End Function
#End Region

    Public Sub New()

    End Sub
End Class  'clsVersabell2_32

Friend Module mCCToonRoutines
    Friend Enum ePressureTestStep
        No_Parameter
        Start
        Abort
        FillSolv
        MeasureSolv
        FillAir
        MeasureAir
        Finish
    End Enum
    Friend Sub SetCCType(ByRef rZones As clsZones, ByRef rControls As Control.ControlCollection, _
                            ByRef rUctrlCartoon As UserControl, ByRef rCCToon As clsColorChangeCartoon, _
                            ByRef rRobot As clsArm, Optional ByRef oApplicator As clsApplicator = Nothing, _
                            Optional ByVal bCCDataScreen As Boolean = False )
        '********************************************************************************************
        'Description: Select a color change type, setup thee toon type and user control
        '
        'Parameters:  
        'Returns:     
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/30/13  MSW     Add dual can cc type - eColorChangeType.VERSABELL3_DUAL_WB
        '********************************************************************************************

        'Set up the fancy graphics
        'First remove the old one
        Try
            If ((rUctrlCartoon Is Nothing) = False) Then
                rControls.Remove(rUctrlCartoon)
                rUctrlCartoon.Dispose()
            End If
        Catch ex As Exception
            WriteEventToLog("mCCToonRoutines:SetCCType", ex.StackTrace & vbCrLf & ex.Message)
        End Try
        Select Case rRobot.ColorChangeType
            Case eColorChangeType.ACCUSTAT

            Case eColorChangeType.SINGLE_PURGE
                'App specific color change cartoon usercontrol
                rUctrlCartoon = New uctrlGun
                'And the app specific class from vb
                rCCToon = New clsGunCartoon
            Case eColorChangeType.VERSABELL
                'App specific color change cartoon usercontrol
                rUctrlCartoon = New uctrlVersabell
                'And the app specific class from vb
                rCCToon = New clsVersabellCartoon
            Case eColorChangeType.VERSABELL2
                'App specific color change cartoon usercontrol
                rUctrlCartoon = New uctrlVersabell
                'And the app specific class from vb
                rCCToon = New clsVersabellCartoon

            Case eColorChangeType.VERSABELL2_2K
                'App specific color change cartoon usercontrol
                rUctrlCartoon = New uctrlVersabell2_2k
                'And the app specific class from vb
                rCCToon = New clsVersabell2_2kCartoon

            Case eColorChangeType.VERSABELL2_PLUS
                'App specific color change cartoon usercontrol
                rUctrlCartoon = New uctrlVersabell2plus
                'And the app specific class from vb
                rCCToon = New clsVersabell2plusCartoon

            Case eColorChangeType.VERSABELL2_WB
                'App specific color change cartoon usercontrol
                rUctrlCartoon = New uctrlVersabell2WB
                'And the app specific class from vb
                rCCToon = New clsVersabell2WBCartoon

            Case eColorChangeType.VERSABELL2_PLUS_WB
                'App specific color change cartoon usercontrol
                rUctrlCartoon = New uctrlVersabell2plusWB
                'And the app specific class from vb
                rCCToon = New clsVersabell2plusWBCartoon

            Case eColorChangeType.HONDA_1K
                'App specific color change cartoon usercontrol
                rUctrlCartoon = New uctrlHonda1K
                'And the app specific class from vb
                rCCToon = New clsHonda1KCartoon

            Case eColorChangeType.HONDA_WB
                'App specific color change cartoon usercontrol
                rUctrlCartoon = New uctrlHondaWB
                'And the app specific class from vb
                rCCToon = New clsHondaWBCartoon
            Case eColorChangeType.VERSABELL2_32  'BTK 02/15/10 Added color change type for Versabell2 32 valves
                'App specific color change cartoon usercontrol
                rUctrlCartoon = New uctrlVersabell2_32
                'And the app specific class from vb
                rCCToon = New clsVersabell2_32Cartoon
            Case eColorChangeType.VERSABELL3_DUAL_WB ' 09/30/13  MSW     Add dual can cc type - eColorChangeType.VERSABELL3_DUAL_WB
                'App specific color change cartoon usercontrol
                rUctrlCartoon = New uctrlVersabell3dualwb
                'And the app specific class from vb
                rCCToon = New clsVersabell3dualWBCartoon
            Case eColorChangeType.VERSABELL3_WB ' 11/11/13  BTK     Add dual can cc type - eColorChangeType.VERSABELL3_WB
                'App specific color change cartoon usercontrol
                rUctrlCartoon = New uctrlVersabell3SingleWB
                'And the app specific class from vb
                rCCToon = New clsVersabell3SingleWBCartoon
            Case eColorChangeType.VERSABELL3 ' 11/11/13  BTK     Add dual can cc type - eColorChangeType.VERSABELL3_WB
                'App specific color change cartoon usercontrol
                rUctrlCartoon = New uctrlVersabell3
                'And the app specific class from vb
                rCCToon = New clsVersabell3Cartoon
            Case eColorChangeType.VERSABELL3P1000 'RJO 07/16/14
                'App specific color change cartoon usercontrol
                rUctrlCartoon = New uctrlVersabell3P1000
                'And the app specific class from vb
                rCCToon = New clsVersabell3P1000Cartoon
            Case eColorChangeType.GUN_2K ' MAS 20160909 Add in guns
                rUctrlCartoon = New uctrlGun_2k
                rCCToon = New clsGun_2kCartoon
            Case eColorChangeType.GUN_2K_PIG ' MAS 20160909 Add in guns
                rUctrlCartoon = New uctrlGun_2k_Pig
                rCCToon = New clsGun_2kCartoon_Pig
            Case eColorChangeType.GUN_2K_Mica   'JZ 12062016 - Piggable stack only.
                rUctrlCartoon = New uctrlGun_2k_Mica
                rCCToon = New clsGun_2k_MicaCartoon
            Case eColorChangeType.GUN_1K
                rUctrlCartoon = New uctrlGunPump
                rCCToon = New clsGunPumpCartoon
        End Select
        If ((rUctrlCartoon Is Nothing) = False) And ((rCCToon Is Nothing) = False) Then
            'Set up labels,  the class also gets it's hooks into the usercontrol
            rCCToon.Initialize(rUctrlCartoon, rZones.DatabasePath, rRobot, oApplicator)
            'Put the cartoon on the screen
            rControls.Add(rUctrlCartoon)
            rCCToon.ShowFeedbackLabels = Not (bCCDataScreen)
        End If

    End Sub
End Module

