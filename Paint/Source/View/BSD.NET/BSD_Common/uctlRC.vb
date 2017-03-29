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
' Form/Module: uctlRC.vb
' Description: Displays Servo Disconnect, SOP estop, TP estop, Purge Status
'
' Dependencies:
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
' 10/21/09      RJO     first draft based on MSW uctlRobotPanel
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System.ComponentModel
Imports System.Reflection
Imports System.Resources

Public Class uctlRC


#Region " Declares "

    Inherits System.Windows.Forms.UserControl

    Public Event Mouse_Down(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

#End Region

#Region " Properties "

    Public WriteOnly Property Estop() As Image
        '********************************************************************************************
        'Description: Sets the SOP Estop picture
        '
        'Parameters: Image to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As Image)
            picSOPEstop.Image = value
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("SOP Estop text to be displayed.")> _
    Public Property EstopText() As String
        '********************************************************************************************
        'Description: Gets/Sets the SOP Estop label text
        '
        'Parameters: Text to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return lblEstop.Text
        End Get

        Set(ByVal value As String)
            lblEstop.Text = value
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Purge Status ToolTip text to be displayed.")> _
    Public Property EstopTT() As String
        '********************************************************************************************
        'Description: Gets/Sets the SOP Estop icon ToolTip text
        '
        'Parameters: Text to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return ttRC.GetToolTip(picSOPEstop)
        End Get

        Set(ByVal value As String)
            ttRC.SetToolTip(picSOPEstop, value)
        End Set
    End Property

    Public WriteOnly Property Purge() As Image
        '********************************************************************************************
        'Description: Sets the Purge Status Indicator picture
        '
        'Parameters: Image to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As Image)
            picPurge.Image = value
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Purge Status Indicator text to be displayed.")> _
    Public Property PurgeText() As String
        '********************************************************************************************
        'Description: Gets/Sets the Purge Status label text
        '
        'Parameters: Text to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return lblPurge.Text
        End Get

        Set(ByVal value As String)
            lblPurge.Text = value
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Purge Status ToolTip text to be displayed.")> _
    Public Property PurgeTT() As String
        '********************************************************************************************
        'Description: Gets/Sets the Purge Status icon ToolTip text
        '
        'Parameters: Text to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return ttRC.GetToolTip(picPurge)
        End Get

        Set(ByVal value As String)
            ttRC.SetToolTip(picPurge, value)
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Robot Controller Name to be displayed.")> _
    Public Property RCName() As String
        '********************************************************************************************
        'Description: Gets/Sets the groupbox title text
        '
        'Parameters: RC Name
        'Returns:    RC Name
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return gpbRC.Text
        End Get

        Set(ByVal value As String)
            gpbRC.Text = value
        End Set
    End Property

    Public WriteOnly Property ServoDisc() As Image
        '********************************************************************************************
        'Description: Sets the Servo Disconnect picture
        '
        'Parameters: Image to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As Image)
            picServoDisc.Image = value
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Servo Disconnect text to be displayed.")> _
    Public Property ServoDiscText() As String
        '********************************************************************************************
        'Description: Gets/Sets the Servo Disconnect label text
        '
        'Parameters: Text to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return lblDisconnect.Text
        End Get

        Set(ByVal value As String)
            lblDisconnect.Text = value
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Servo Disconnect ToolTip text to be displayed.")> _
    Public Property ServoDiscTT() As String
        '********************************************************************************************
        'Description: Gets/Sets the Servo Disconnect icon ToolTip text
        '
        'Parameters: Text to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return ttRC.GetToolTip(picServoDisc)
        End Get

        Set(ByVal value As String)
            ttRC.SetToolTip(picServoDisc, value)
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Gets/Sets SOP Estop visibility.")> _
    Public Property ShowEstop() As Boolean
        '********************************************************************************************
        'Description: Gets/Sets the SOP Estop icon/text visibilty
        '
        'Parameters: True = visible
        'Returns:    visible
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return picSOPEstop.Visible
        End Get

        Set(ByVal value As Boolean)
            picSOPEstop.Visible = value
            lblEstop.Visible = value
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Gets/Sets Purge Status Indicator visibility.")> _
    Public Property ShowPurge() As Boolean
        '********************************************************************************************
        'Description: Gets/Sets the Purge Status icon/text visibilty
        '
        'Parameters: True = visible
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return picPurge.Visible
        End Get

        Set(ByVal value As Boolean)
            picPurge.Visible = value
            lblPurge.Visible = value
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Gets/Sets Servo Disconnect visibility.")> _
    Public Property ShowServoDisc() As Boolean
        '********************************************************************************************
        'Description: Sets the Servo Disconnect icon/text visibilty
        '
        'Parameters: True = visible
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return picServoDisc.Visible
        End Get

        Set(ByVal value As Boolean)
            picServoDisc.Visible = value
            lblDisconnect.Visible = value
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Gets/Sets Teach Pendant visibility.")> _
    Public Property ShowTP() As Boolean
        '********************************************************************************************
        'Description: Sets the Teach Pendant icon/text visibilty
        '
        'Parameters: True = visible
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return picTP.Visible
        End Get

        Set(ByVal value As Boolean)
            picTP.Visible = value
            lblTP.Visible = value
        End Set
    End Property

    Public WriteOnly Property TP() As Image
        '********************************************************************************************
        'Description: Sets the Teach Pendant picture
        '
        'Parameters: Image to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As Image)
            picTP.Image = value
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Teach Pendant text to be displayed.")> _
    Public Property TPText() As String
        '********************************************************************************************
        'Description: Gets/Sets the Teach Pendant label text
        '
        'Parameters: Text to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return lblTP.Text
        End Get
        Set(ByVal value As String)
            lblTP.Text = value
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Teach Pendant ToolTip text to be displayed.")> _
    Public Property TPTT() As String
        '********************************************************************************************
        'Description: Gets/Sets the Teach Pendant icon ToolTip text
        '
        'Parameters: Text to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return ttRC.GetToolTip(picTP)
        End Get

        Set(ByVal value As String)
            ttRC.SetToolTip(picTP, value)
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub uctlRC_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
                                 Handles Me.MouseDown, lblDisconnect.MouseDown, lblEstop.MouseDown, _
        lblPurge.MouseDown, lblTP.MouseDown, picPurge.MouseDown, _
        picServoDisc.MouseDown, picSOPEstop.MouseDown, picTP.MouseDown, _
        gpbRC.MouseDown
        '********************************************************************************************
        'Description: Raise an event if the user clicks on this control
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            RaiseEvent Mouse_Down(Me, e)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub

#End Region

End Class
