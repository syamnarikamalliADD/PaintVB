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
' Form/Module: frmGhostStatus
'
' Description: Ghost Status
' 
'
' Dependancies:  
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
'    Date       By      Reason                                                        Version
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On


Imports System.Windows.Forms

Friend Class frmGhostStatus


#Region " Declares "
    Private mnNumItems As Integer = eBooth.GhostNumBits
#End Region
#Region " Properties "
#End Region
#Region " Routines"
    Friend Sub UpdateHotlinkData(ByVal vData As String())
        '********************************************************************************************
        'Description:  update labels
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim sWord(0) As String
        Dim nBox As Integer = 1

        If vData Is Nothing Then Exit Sub


        Try
            If eBooth.GhostInfoWd <= vData.GetUpperBound(0) Then
                sWord(0) = vData(eBooth.GhostInfoWd)

                If sWord(0) = "-" Then
                    sWord(0) = "0"
                End If
                'Ghost Word Bits
                'GhostBoothQClearBit = 0
                'GhostAutoModeBit = 1
                'GhostRobotsReady = 2
                'GhostJogDisabled = 3
                'GhostZoneOK = 4
                bbGhostStatus.ItemData = sWord

                'If it's ready, enable the button
                btnContinue.Enabled = (CInt(sWord(0)) And eBooth.GhostZoneOK) = eBooth.GhostZoneOK
            Else
                btnContinue.Enabled = False
            End If

        Catch ex As Exception

        End Try



    End Sub

    Friend Sub subInitFormText()
        '********************************************************************************************
        'Description: Init Common text
        '
        'Parameters: robot, presets
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Me.Text = gpsRM.GetString("psGHOST_STATUS")
        btnCancel.Text = gpsRM.GetString("psCANCEL")
        btnContinue.Text = gpsRM.GetString("psCONTINUE")
    End Sub
    Friend Shadows Function Show(ByVal sZoneData() As String, Optional ByVal bZDT As Boolean = False) As Boolean
        '********************************************************************************************
        'Description: start the screen
        '
        'Parameters: robot
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Me.StartPosition = FormStartPosition.CenterScreen
        subInitFormText()
        'set up the bingo board control
        Dim sName As String
        Dim sTagPrefix As String = String.Empty
        If bZDT Then
            mnNumItems = eBooth.ZDTNumBits
            sTagPrefix = "psZDT_DEV"
        Else
            mnNumItems = eBooth.GhostNumBits
            sTagPrefix = "psGHOST_DEV"
        End If
        Dim sData(mnNumItems) As String
        Dim sVal(0) As String
        sVal(0) = "0"
        With bbGhostStatus

            .Columns = 1
            .AutosizeColumns = True
            .ItemOffColor = Color.Tomato
            .ItemCount = mnNumItems
            .TitleFont = New Font("Arial", 10, FontStyle.Bold)
            .TitleText = colZones.CurrentZone 'gpsRM.GetString("psZoneTitle")
            .ItemFont = New Font("Arial", 10, FontStyle.Bold)

            For i As Integer = 0 To mnNumItems
                sData(i) = i.ToString
            Next
            .ItemBitIndex = sData

            .BorderStyle = BorderStyle.Fixed3D

            sData = Nothing
            ReDim sData(mnNumItems)

            For i As Integer = 0 To mnNumItems
                sName = sTagPrefix & "_ON" & Format(i, "00")
                sData(i) = gpsRM.GetString(sName)
            Next
            .ItemOnText = sData

            sData = Nothing
            ReDim sData(mnNumItems)

            For i As Integer = 0 To mnNumItems
                sName = sTagPrefix & "_OFF" & Format(i, "00")
                sData(i) = gpsRM.GetString(sName)
            Next
            .ItemOffText = sData

            UpdateHotlinkData(sZoneData)

        End With
        btnContinue.Enabled = False
        Return MyBase.ShowDialog() = Windows.Forms.DialogResult.OK
    End Function

#End Region
#Region " Events "

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub btnContinue_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnContinue.Click
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        DialogResult = Windows.Forms.DialogResult.OK
    End Sub
    Public Sub New()
        '********************************************************************************************
        'Description:  
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

    End Sub
#End Region
End Class