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
' Form/Module: uctlDevice.vb
' Description: Displays Position, Estat Status, Bypass Prox Switch Status
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
' 06/12/13      RJO     Added Applicator Status LED and detail context menu
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System.ComponentModel
Imports System.Reflection
Imports System.Resources

Public Class uctlDevice


#Region " Declares "

    Inherits System.Windows.Forms.UserControl

    Private mbIsOpener As Boolean
    Private mnApplStatusItems As Integer
    Private mnRC_Number As Integer
    Private mnEq_Number As Integer
    Private msApplStatusItems() As String

    Public Event Mouse_Down(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

#End Region

#Region " Properties "

    Public WriteOnly Property ApplicatorStatus() As Integer
        '********************************************************************************************
        'Description: Updatess the Applicator Status LED pictures on the control and in the context 
        '             menu.
        '
        'Parameters: Applicator status bits
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As Integer)
            If Not IsOpener Then
                Dim bOK As Boolean = True

                For nItem As Integer = 1 To 4
                    Dim sItem As String = "mnuApplStatusItem" & nItem.ToString
                    Dim oMenuItem As ToolStripItem = mnuApplStatus.Items(sItem)

                    If oMenuItem.Text <> String.Empty Then
                        Dim nBit As Integer = CType(oMenuItem.Tag, Integer)

                        If (value And (CType(2 ^ nBit, Integer))) > 0 Then
                            oMenuItem.Image = imlStatus.Images("green")
                        Else
                            oMenuItem.Image = imlStatus.Images("red")
                            bOK = False
                        End If
                    End If
                Next

                If bOK Then
                    picAppicatorStatus.Image = imlStatus.Images("green")
                Else
                    picAppicatorStatus.Image = imlStatus.Images("red")
                End If
            End If 'Not IsOpener
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Data bit numbers associated with Applicator Status items.")> _
    Public WriteOnly Property ApplStatBits() As Integer()
        '********************************************************************************************
        'Description: Sets the data bit number associated with each Applicator Status context menu 
        '             item.
        '
        'Parameters: Array of Status bit numbers
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As Integer())
            Dim nItems As Integer = value.GetUpperBound(0) + 1

            For nItem As Integer = 1 To nItems
                Dim sItem As String = "mnuApplStatusItem" & nItem.ToString
                Dim oMenuItem As ToolStripItem = mnuApplStatus.Items(sItem)

                If oMenuItem.Text = String.Empty Then
                    oMenuItem.Tag = 0
                Else
                    oMenuItem.Tag = value(nItem - 1)
                End If
            Next
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Applicator Status items to display.")> _
    Public WriteOnly Property ApplStatItems() As String()
        '********************************************************************************************
        'Description: Sets the Applicator Status context menu items' text
        '
        'Parameters: Array of Status Text to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As String())
            Dim nItems As Integer = value.GetUpperBound(0) + 1

            For nItem As Integer = 1 To nItems
                Dim sItem As String = "mnuApplStatusItem" & nItem.ToString
                Dim oMenuItem As ToolStripItem = mnuApplStatus.Items(sItem)

                If value(nItem - 1) = String.Empty Then
                    oMenuItem.Text = String.Empty
                    oMenuItem.Visible = False
                Else
                    oMenuItem.Text = value(nItem - 1)
                    oMenuItem.Visible = True
                End If
            Next
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Applicator Status LED ToolTip text to be displayed.")> _
    Public Property ApplStatLEDTT() As String
        '********************************************************************************************
        'Description: Gets/Sets the Applicator Status LED picture ToolTip
        '
        'Parameters: Text to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return ttDevice.GetToolTip(picAppicatorStatus)
        End Get

        Set(ByVal value As String)
            ttDevice.SetToolTip(picAppicatorStatus, value)
        End Set
    End Property

    Public WriteOnly Property BypassProx() As Image
        '********************************************************************************************
        'Description: Sets the Bypass Prox Switch Status picture
        '
        'Parameters: Image to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As Image)
            picBypassProx.Image = value
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Bypass Prox Switch ToolTip text to be displayed.")> _
    Public Property BypassProxTT() As String
        '********************************************************************************************
        'Description: Gets/Sets the Bypass Prox Switch Status picture ToolTip
        '
        'Parameters: Text to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return ttDevice.GetToolTip(picBypassProx)
        End Get

        Set(ByVal value As String)
            ttDevice.SetToolTip(picBypassProx, value)
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Device Name (Title) text to be displayed.")> _
    Public Property DeviceName() As String
        '********************************************************************************************
        'Description: Gets/Sets the Device Name text
        '
        'Parameters: Text to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return gpbDevice.Text
        End Get

        Set(ByVal value As String)
            gpbDevice.Text = value
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Equipment number of this device.")> _
    Public Property Eq_Number() As Integer
        '********************************************************************************************
        'Description: Gets/Sets the equipment number of the arm that controls this device
        '
        'Parameters: text to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnEq_Number
        End Get

        Set(ByVal value As Integer)
            mnEq_Number = value
        End Set
    End Property

    Public WriteOnly Property Estat() As Image
        '********************************************************************************************
        'Description: Sets the Estat Status picture
        '
        'Parameters: Image to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Set(ByVal value As Image)
            picEstat.Image = value
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Estat Status ToolTip text to be displayed.")> _
    Public Property EstatTT() As String
        '********************************************************************************************
        'Description: Gets/Sets the Estat Status picture ToolTip
        '
        'Parameters: Text to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return ttDevice.GetToolTip(picEstat)
        End Get

        Set(ByVal value As String)
            ttDevice.SetToolTip(picEstat, value)
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("True = This device is an opener.")> _
    Public Property IsOpener() As Boolean
        '********************************************************************************************
        'Description: Gets/Sets the IsOpener property for this device
        '
        'Parameters: Text to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mbIsOpener
        End Get

        Set(ByVal value As Boolean)
            mbIsOpener = value
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Device Position/Status text to be displayed.")> _
    Public Property Position() As String
        '********************************************************************************************
        'Description: Gets/Sets the Position/Status Text
        '
        'Parameters: Text to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return lblPosition.Text
        End Get

        Set(ByVal value As String)
            lblPosition.Text = value
        End Set
    End Property

    Public Property PositionLabel() As Label
        '********************************************************************************************
        'Description: Gets/Sets the Position/Status Label control
        '
        'Parameters: none
        'Returns:    Position/Status Label
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return lblPosition
        End Get

        Set(ByVal value As Label)
            lblPosition = value
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Device Position/Status ToolTip text to be displayed.")> _
    Public Property PositionTT() As String
        '********************************************************************************************
        'Description: Gets/Sets the Position/Status ToolTip Text
        '
        'Parameters: text to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return ttDevice.GetToolTip(lblPosition)
        End Get

        Set(ByVal value As String)
            ttDevice.SetToolTip(lblPosition, value)
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Robot controller number of the RC that controls this device.")> _
    Public Property RC_Number() As Integer
        '********************************************************************************************
        'Description: Gets/Sets the Robot Controller number of the RC that controls this device
        '
        'Parameters: text to display
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return mnRC_Number
        End Get

        Set(ByVal value As Integer)
            mnRC_Number = value
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Gets/Sets Applicator Status LED visibility.")> _
    Public Property ShowApplicatorStatus() As Boolean
        '********************************************************************************************
        'Description: Gets/Sets the Applicator Status LED icon visibilty
        '
        'Parameters: True = visible
        'Returns:    visible
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return picAppicatorStatus.Visible
        End Get

        Set(ByVal value As Boolean)
            picAppicatorStatus.Visible = value
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Gets/Sets Bypass Prox Switch visibility.")> _
    Public Property ShowBypassProx() As Boolean
        '********************************************************************************************
        'Description: Gets/Sets the Bypass Prox Switch icon visibilty
        '
        'Parameters: True = visible
        'Returns:    visible
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return picBypassProx.Visible
        End Get

        Set(ByVal value As Boolean)
            picBypassProx.Visible = value
        End Set
    End Property

    <Category("Custom"), Browsable(True), Description("Gets/Sets Estat Status visibility.")> _
    Public Property ShowEstat() As Boolean
        '********************************************************************************************
        'Description: Gets/Sets the Estat Status icon visibilty
        '
        'Parameters: True = visible
        'Returns:    visible
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return picEstat.Visible
        End Get

        Set(ByVal value As Boolean)
            picEstat.Visible = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub uctlRC_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
                                 Handles Me.MouseDown, lblPosition.MouseDown, picBypassProx.MouseDown, _
                                 picEstat.MouseDown, gpbDevice.MouseDown
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


    Private Sub picAppicatorStatus_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles picAppicatorStatus.Click

    End Sub
End Class
