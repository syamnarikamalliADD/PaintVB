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
' Form/Module: Purge Panel Control
'
' Description: Robot Cavity Purge Control/Monitor Panel Control
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Rick Olejniczak
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    07/09/13   MSW     Update and standardize logos                                  4.01.05.00
'    04/03/14   MSW     Mark a version, split up folders for building block versions  4.01.07.00
'********************************************************************************************

Option Compare Text
Option Explicit On
Option Strict On

Imports System.ComponentModel
Imports System.Reflection
Imports System.Resources

Public Class PurgePanel

    Inherits System.Windows.Forms.UserControl

#Region " Declares "

    '******** Enumerations    **********************************************************************
    Public Enum ePilotLightState
        PL_OFF = 0
        PL_ON = 1
    End Enum

    Public Enum eProgressStyle
        PercentComplete = 0
        TimeElapsed = 1
        TimeRemaining = 2
    End Enum

    Public Enum ePushButtonState
        PB_UP = 0
        PB_DOWN = 1
    End Enum
    '******** End Enumerations    *******************************************************************

    '******** Events    *****************************************************************************
    Public Event StartPurge(ByRef sender As PurgePanel)
    '******** End Events    *************************************************************************

    '******** Module Variables    *******************************************************************
    Private mRM As ResourceManager = New System.Resources.ResourceManager("PurgePanel.Resources", [Assembly].GetExecutingAssembly())
    '******** End Module Variables     **************************************************************

    '******** Property Variables    *****************************************************************
    Private mnEquipmentCount As Integer = 4 'default to larget size
    Private mnPurgeDuration As Integer = 300 'seconds
    Private mEq1FlowState As ePilotLightState = ePilotLightState.PL_OFF
    Private mEq1PressureState As ePilotLightState = ePilotLightState.PL_OFF
    Private mEq2FlowState As ePilotLightState = ePilotLightState.PL_OFF
    Private mEq2PressureState As ePilotLightState = ePilotLightState.PL_OFF
    Private mEq3FlowState As ePilotLightState = ePilotLightState.PL_OFF
    Private mEq3PressureState As ePilotLightState = ePilotLightState.PL_OFF
    Private mEq4FlowState As ePilotLightState = ePilotLightState.PL_OFF
    Private mEq4PressureState As ePilotLightState = ePilotLightState.PL_OFF
    Private mPurgeEnabledState As ePilotLightState = ePilotLightState.PL_OFF
    Private mPurgeFaultState As ePilotLightState = ePilotLightState.PL_OFF
    Private mProgressStyle As eProgressStyle = eProgressStyle.PercentComplete
    Private mRobotPowerState As ePilotLightState = ePilotLightState.PL_OFF
    '******** End Property Variables     ************************************************************

#End Region

#Region " Properties "

    <Category("Design"), Browsable(True), Description("Device Name to appear at top of panel.")> _
    Public Property DeviceNameText() As String
        '********************************************************************************************
        'Description:  Device Name label text.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return lblDeviceName.Text
        End Get
        Set(ByVal value As String)
            lblDeviceName.Text = value
        End Set

    End Property

    <Category("Design"), Browsable(True), Description("Number of Flow/Pressure switch lights to display (1-4).")> _
Public Property EquipmentCount() As Integer
        '********************************************************************************************
        'Description:  Device Name label text.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnEquipmentCount
        End Get
        Set(ByVal value As Integer)
            'Make sure the supplied value is within range
            If value < 1 Then value = 1
            If value > 4 Then value = 4
            mnEquipmentCount = value

            Select Case mnEquipmentCount
                Case 1
                    Me.Width = 194
                    Call subEq2Visible(False)
                    Call subEq3Visible(False)
                    Call subEq4Visible(False)

                Case 2
                    Me.Width = 194
                    Call subEq2Visible(True)
                    Call subEq3Visible(False)
                    Call subEq4Visible(False)

                Case 3
                    Me.Width = 276
                    Call subEq2Visible(True)
                    Call subEq3Visible(True)
                    Call subEq4Visible(False)

                Case 4
                    Me.Width = 360
                    Call subEq2Visible(True)
                    Call subEq3Visible(True)
                    Call subEq4Visible(True)
            End Select

        End Set

    End Property

    <Category("Status"), Browsable(True), Description("Eq1 Flow Switch Pilot Light state (OFF/ON).")> _
    Public Property Eq1FlowState() As ePilotLightState
        '********************************************************************************************
        'Description:  Eq1 Flow Switch PL state.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mEq1FlowState
        End Get

        Set(ByVal value As ePilotLightState)
            Select Case value
                Case ePilotLightState.PL_OFF
                    picEq1FlowPL.BackgroundImage = DirectCast(mRM.GetObject("AB_Green_PL_OFF"), Image)
                Case ePilotLightState.PL_ON
                    picEq1FlowPL.BackgroundImage = DirectCast(mRM.GetObject("AB_Green_PL_ON"), Image)
            End Select
            mEq1FlowState = value
        End Set

    End Property

    <Category("Design"), Browsable(True), Description("Eq1 Flow Switch Pilot Light description label text.")> _
    Public Property Eq1FlowText() As String
        '********************************************************************************************
        'Description:  Eq1 Flow Switch PL label text.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return lblEq1Flow.Text
        End Get

        Set(ByVal value As String)
            lblEq1Flow.Text = value
        End Set

    End Property

    <Category("Status"), Browsable(True), Description("Eq1 Pressure Switch Pilot Light state (OFF/ON).")> _
    Public Property Eq1PressureState() As ePilotLightState
        '********************************************************************************************
        'Description:  Eq1 Pressure Switch PL state.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mEq1PressureState
        End Get

        Set(ByVal value As ePilotLightState)
            Select Case value
                Case ePilotLightState.PL_OFF
                    picEq1PressurePL.BackgroundImage = DirectCast(mRM.GetObject("AB_Green_PL_OFF"), Image)
                Case ePilotLightState.PL_ON
                    picEq1PressurePL.BackgroundImage = DirectCast(mRM.GetObject("AB_Green_PL_ON"), Image)
            End Select
            mEq1PressureState = value
        End Set

    End Property

    <Category("Design"), Browsable(True), Description("Eq1 Pressure Switch Pilot Light description label text.")> _
    Public Property Eq1PressureText() As String
        '********************************************************************************************
        'Description:  Eq1 Pressure Switch PL label text.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return lblEq1Pressure.Text
        End Get

        Set(ByVal value As String)
            lblEq1Pressure.Text = value
        End Set

    End Property

    <Category("Status"), Browsable(True), Description("Eq2 Flow Switch Pilot Light state (OFF/ON).")> _
    Public Property Eq2FlowState() As ePilotLightState
        '********************************************************************************************
        'Description:  Eq2 Flow Switch PL state.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mEq2FlowState
        End Get

        Set(ByVal value As ePilotLightState)
            Select Case value
                Case ePilotLightState.PL_OFF
                    picEq2FlowPL.BackgroundImage = DirectCast(mRM.GetObject("AB_Green_PL_OFF"), Image)
                Case ePilotLightState.PL_ON
                    picEq2FlowPL.BackgroundImage = DirectCast(mRM.GetObject("AB_Green_PL_ON"), Image)
            End Select
            mEq2FlowState = value
        End Set

    End Property

    <Category("Design"), Browsable(True), Description("Eq2 Flow Switch Pilot Light description label text.")> _
    Public Property Eq2FlowText() As String
        '********************************************************************************************
        'Description:  Eq2 Flow Switch PL label text.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return lblEq2Flow.Text
        End Get

        Set(ByVal value As String)
            lblEq2Flow.Text = value
        End Set

    End Property

    <Category("Status"), Browsable(True), Description("Eq2 Pressure Switch Pilot Light state (OFF/ON).")> _
    Public Property Eq2PressureState() As ePilotLightState
        '********************************************************************************************
        'Description:  Eq2 Pressure Switch PL state.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mEq2PressureState
        End Get

        Set(ByVal value As ePilotLightState)
            Select Case value
                Case ePilotLightState.PL_OFF
                    picEq2PressurePL.BackgroundImage = DirectCast(mRM.GetObject("AB_Green_PL_OFF"), Image)
                Case ePilotLightState.PL_ON
                    picEq2PressurePL.BackgroundImage = DirectCast(mRM.GetObject("AB_Green_PL_ON"), Image)
            End Select
            mEq2PressureState = value
        End Set

    End Property

    <Category("Design"), Browsable(True), Description("Eq2 Pressure Switch Pilot Light description label text.")> _
    Public Property Eq2PressureText() As String
        '********************************************************************************************
        'Description:  Eq2 Pressure Switch PL label text.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return lblEq2Pressure.Text
        End Get

        Set(ByVal value As String)
            lblEq2Pressure.Text = value
        End Set

    End Property
    '>>>
    <Category("Status"), Browsable(True), Description("Eq3 Flow Switch Pilot Light state (OFF/ON).")> _
    Public Property Eq3FlowState() As ePilotLightState
        '********************************************************************************************
        'Description:  Eq3 Flow Switch PL state.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mEq3FlowState
        End Get

        Set(ByVal value As ePilotLightState)
            Select Case value
                Case ePilotLightState.PL_OFF
                    picEq3FlowPL.BackgroundImage = DirectCast(mRM.GetObject("AB_Green_PL_OFF"), Image)
                Case ePilotLightState.PL_ON
                    picEq3FlowPL.BackgroundImage = DirectCast(mRM.GetObject("AB_Green_PL_ON"), Image)
            End Select
            mEq3FlowState = value
        End Set

    End Property

    <Category("Design"), Browsable(True), Description("Eq3 Flow Switch Pilot Light description label text.")> _
    Public Property Eq3FlowText() As String
        '********************************************************************************************
        'Description:  Eq3 Flow Switch PL label text.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return lblEq3Flow.Text
        End Get

        Set(ByVal value As String)
            lblEq3Flow.Text = value
        End Set

    End Property

    <Category("Status"), Browsable(True), Description("Eq3 Pressure Switch Pilot Light state (OFF/ON).")> _
    Public Property Eq3PressureState() As ePilotLightState
        '********************************************************************************************
        'Description:  Eq3 Pressure Switch PL state.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mEq3PressureState
        End Get

        Set(ByVal value As ePilotLightState)
            Select Case value
                Case ePilotLightState.PL_OFF
                    picEq3PressurePL.BackgroundImage = DirectCast(mRM.GetObject("AB_Green_PL_OFF"), Image)
                Case ePilotLightState.PL_ON
                    picEq3PressurePL.BackgroundImage = DirectCast(mRM.GetObject("AB_Green_PL_ON"), Image)
            End Select
            mEq3PressureState = value
        End Set

    End Property

    <Category("Design"), Browsable(True), Description("Eq1 Pressure Switch Pilot Light description label text.")> _
    Public Property Eq3PressureText() As String
        '********************************************************************************************
        'Description:  Eq3 Pressure Switch PL label text.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return lblEq3Pressure.Text
        End Get

        Set(ByVal value As String)
            lblEq3Pressure.Text = value
        End Set

    End Property

    <Category("Status"), Browsable(True), Description("Eq4 Flow Switch Pilot Light state (OFF/ON).")> _
    Public Property Eq4FlowState() As ePilotLightState
        '********************************************************************************************
        'Description:  Eq4 Flow Switch PL state.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mEq4FlowState
        End Get

        Set(ByVal value As ePilotLightState)
            Select Case value
                Case ePilotLightState.PL_OFF
                    picEq4FlowPL.BackgroundImage = DirectCast(mRM.GetObject("AB_Green_PL_OFF"), Image)
                Case ePilotLightState.PL_ON
                    picEq4FlowPL.BackgroundImage = DirectCast(mRM.GetObject("AB_Green_PL_ON"), Image)
            End Select
            mEq4FlowState = value
        End Set

    End Property

    <Category("Design"), Browsable(True), Description("Eq4 Flow Switch Pilot Light description label text.")> _
    Public Property Eq4FlowText() As String
        '********************************************************************************************
        'Description:  Eq4 Flow Switch PL label text.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return lblEq4Flow.Text
        End Get

        Set(ByVal value As String)
            lblEq4Flow.Text = value
        End Set

    End Property

    <Category("Status"), Browsable(True), Description("Eq4 Pressure Switch Pilot Light state (OFF/ON).")> _
    Public Property Eq4PressureState() As ePilotLightState
        '********************************************************************************************
        'Description:  Eq4 Pressure Switch PL state.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mEq4PressureState
        End Get

        Set(ByVal value As ePilotLightState)
            Select Case value
                Case ePilotLightState.PL_OFF
                    picEq4PressurePL.BackgroundImage = DirectCast(mRM.GetObject("AB_Green_PL_OFF"), Image)
                Case ePilotLightState.PL_ON
                    picEq4PressurePL.BackgroundImage = DirectCast(mRM.GetObject("AB_Green_PL_ON"), Image)
            End Select
            mEq4PressureState = value
        End Set

    End Property

    <Category("Design"), Browsable(True), Description("Eq4 Pressure Switch Pilot Light description label text.")> _
    Public Property Eq4PressureText() As String
        '********************************************************************************************
        'Description:  Eq2 Pressure Switch PL label text.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return lblEq4Pressure.Text
        End Get

        Set(ByVal value As String)
            lblEq4Pressure.Text = value
        End Set

    End Property

    <Category("Design"), Browsable(True), Description("Defines the type of progress data that will be " & _
                                                      "supplied (PercentComplete/TimeElapsed/TimeRemaining).")> _
    Public Property ProgressStyle() As eProgressStyle
        '********************************************************************************************
        'Description:  Defines how PurgeProgress is intepreted.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mProgressStyle
        End Get

        Set(ByVal value As eProgressStyle)
            mProgressStyle = value
        End Set

    End Property

    <Category("Design"), Browsable(True), Description("Gets/Sets the duration of the purge cycle in seconds.")> _
    Public Property PurgeDuration() As Integer
        '********************************************************************************************
        'Description:  Purge Duration in seconds. Used by progress bar.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnPurgeDuration
        End Get

        Set(ByVal value As Integer)
            mnPurgeDuration = value
        End Set

    End Property

    <Category("Status"), Browsable(True), Description("Purge Enabled Pilot Light state (OFF/ON).")> _
    Public Property PurgeEnabledState() As ePilotLightState
        '********************************************************************************************
        'Description:  Purge Enabled PL state.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mPurgeEnabledState
        End Get

        Set(ByVal value As ePilotLightState)
            Select Case value
                Case ePilotLightState.PL_OFF
                    picPurgeEnabledPL.BackgroundImage = DirectCast(mRM.GetObject("AB_Amber_PL_OFF"), Image)
                Case ePilotLightState.PL_ON
                    picPurgeEnabledPL.BackgroundImage = DirectCast(mRM.GetObject("AB_Amber_PL_ON"), Image)
            End Select
            mPurgeEnabledState = value
        End Set

    End Property

    <Category("Design"), Browsable(True), Description("Purge Enabled Pilot Light description label text.")> _
    Public Property PurgeEnabledText() As String
        '********************************************************************************************
        'Description:  Purge Enabled PL label text.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return lblPurgeEnabled.Text
        End Get

        Set(ByVal value As String)
            lblPurgeEnabled.Text = value
        End Set

    End Property

    <Category("Status"), Browsable(True), Description("Purge Fault Pilot Light state (OFF/ON).")> _
    Public Property PurgeFaultState() As ePilotLightState
        '********************************************************************************************
        'Description:  Purge Fault PL state.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mPurgeFaultState
        End Get

        Set(ByVal value As ePilotLightState)
            Select Case value
                Case ePilotLightState.PL_OFF
                    picPurgeFaultPL.BackgroundImage = DirectCast(mRM.GetObject("AB_Red_PL_OFF"), Image)
                Case ePilotLightState.PL_ON
                    picPurgeFaultPL.BackgroundImage = DirectCast(mRM.GetObject("AB_Red_PL_ON"), Image)
            End Select
            mPurgeFaultState = value
        End Set

    End Property

    <Category("Design"), Browsable(True), Description("Purge Fault Pilot Light description label text.")> _
    Public Property PurgeFaultText() As String
        '********************************************************************************************
        'Description:  Purge Fault PL label text.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return lblPurgeFault.Text
        End Get

        Set(ByVal value As String)
            lblPurgeFault.Text = value
        End Set

    End Property

    <Category("Status"), Browsable(True), Description("Purge progress value.")> _
    Public Property PurgeProgress() As Integer
        '********************************************************************************************
        'Description:  Purge Progress value. Determine percent complete based on ProgressStyle setting.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return ProgressBar1.Value
        End Get

        Set(ByVal value As Integer)
            Dim nPercent As Integer = 0

            Select Case mProgressStyle
                Case eProgressStyle.PercentComplete
                    If value > 0 Then nPercent = value
                    If nPercent > 100 Then nPercent = 100

                Case eProgressStyle.TimeElapsed
                    If (value > 0) And (value <= mnPurgeDuration) Then
                        nPercent = CType((value / mnPurgeDuration) * 100, Integer)
                    End If
                    If value > mnPurgeDuration Then nPercent = 100

                Case eProgressStyle.TimeRemaining
                    If (value > 0) And (value <= mnPurgeDuration) Then
                        nPercent = 100 - CType((value / mnPurgeDuration) * 100, Integer)
                    End If
                    If value = 0 Then nPercent = 100

            End Select

            ProgressBar1.Value = nPercent

        End Set

    End Property

    <Category("Design"), Browsable(True), Description("Purge Progress Bar description label text.")> _
    Public Property PurgeProgressText() As String
        '********************************************************************************************
        'Description:  Purge Progress Control label text.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return lblPurgeProgress.Text
        End Get

        Set(ByVal value As String)
            lblPurgeProgress.Text = value
        End Set

    End Property

    <Category("Status"), Browsable(True), Description("Robot (Controller) Power Pilot Light state (OFF/ON).")> _
    Public Property RobotPowerState() As ePilotLightState
        '********************************************************************************************
        'Description:  Robot (Controller) Power PL state.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mRobotPowerState
        End Get

        Set(ByVal value As ePilotLightState)
            Select Case value
                Case ePilotLightState.PL_OFF
                    picRobotPowerPL.BackgroundImage = DirectCast(mRM.GetObject("AB_Green_PL_OFF"), Image)
                Case ePilotLightState.PL_ON
                    picRobotPowerPL.BackgroundImage = DirectCast(mRM.GetObject("AB_Green_PL_ON"), Image)
            End Select
            mRobotPowerState = value
        End Set

    End Property

    <Category("Design"), Browsable(True), Description("Robot (Controller) Power Pilot Light description label text.")> _
    Public Property RobotPowerText() As String
        '********************************************************************************************
        'Description:  Robot Power PL label text.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return lblRobotPower.Text
        End Get

        Set(ByVal value As String)
            lblRobotPower.Text = value
        End Set

    End Property

    <Category("Design"), Browsable(True), Description("Robot (Controller) Power Pilot Light visible state (True/False).")> _
    Public Property RobotPowerVisible() As Boolean
        '********************************************************************************************
        'Description:  Robot Power PL visibility.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return picRobotPowerPL.Visible
        End Get

        Set(ByVal value As Boolean)
            lblRobotPower.Visible = value
            picRobotPowerPL.Visible = value
        End Set

    End Property

    <Category("Design"), Browsable(True), Description("Start Purge PushButton description label text.")> _
    Public Property StartPurgeText() As String
        '********************************************************************************************
        'Description:  Purge Start PB label text.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return lblPurgeStart.Text
        End Get

        Set(ByVal value As String)
            lblPurgeStart.Text = value
        End Set

    End Property

#End Region

#Region " Routines "

    Private Sub subEq2Visible(ByVal State As Boolean)
        '********************************************************************************************
        'Description:  Set Eq2 Pilot light visbility based on state.
        '
        'Parameters: State (True = visible, False = invisible)
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        lblEq2Flow.Visible = State
        picEq2FlowPL.Visible = State
        lblEq2Pressure.Visible = State
        picEq2PressurePL.Visible = State

    End Sub

    Private Sub subEq3Visible(ByVal State As Boolean)
        '********************************************************************************************
        'Description:  Set Eq3 Pilot light visbility based on state.
        '
        'Parameters: State (True = visible, False = invisible)
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        lblEq3Flow.Visible = State
        picEq3FlowPL.Visible = State
        lblEq3Pressure.Visible = State
        picEq3PressurePL.Visible = State

    End Sub

    Private Sub subEq4Visible(ByVal State As Boolean)
        '********************************************************************************************
        'Description:  Set Eq4 Pilot light visbility based on state.
        '
        'Parameters: State (True = visible, False = invisible)
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        lblEq4Flow.Visible = State
        picEq4FlowPL.Visible = State
        lblEq4Pressure.Visible = State
        picEq4PressurePL.Visible = State

    End Sub

#End Region

#Region " Events "

    Private Sub picPurgeStartPB_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles picPurgeStartPB.Click
        '********************************************************************************************
        'Description:  The user clicked the Start Purge pushbutton. Tell the parent application.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        RaiseEvent StartPurge(Me)

    End Sub

    Private Sub picPurgeStartPB_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles picPurgeStartPB.MouseDown
        '********************************************************************************************
        'Description:  Show the "Down" pushbutton picture.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        picPurgeStartPB.BackgroundImage = DirectCast(mRM.GetObject("AB_Black_PB_DOWN"), Image)

    End Sub

    Private Sub picPurgeStartPB_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles picPurgeStartPB.MouseUp
        '********************************************************************************************
        'Description:  Show the "Up" pushbutton picture.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        picPurgeStartPB.BackgroundImage = DirectCast(mRM.GetObject("AB_Black_PB_UP"), Image)

    End Sub

#End Region

End Class
