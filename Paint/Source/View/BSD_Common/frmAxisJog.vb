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
' Form/Module: frmAxisJog
'
' Description: Axis jog
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
'    02/17/11   MSW     update axis list when robot is changed
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On


Imports System.Windows.Forms
Imports Response = System.Windows.Forms.DialogResult


Friend Class frmAxisJog

#Region " Declares "

    Private mRobot As clsArm = Nothing
    Private mnMaxJoint As Integer = 6
    Private mnMaxWorldCoord As Integer = 3
    Private msPicNamesWorld() As String
    Private msPicNamesJoint() As String
    Private msPicNameAll As String
    Private meJogMode As eJogMode

    Private Enum eJogMode
        Off = 0
        World = CInt(2 ^ 9)
        Joint = CInt(2 ^ 8)
        NoVal = -1
    End Enum

#End Region

#Region " Properties "

#End Region

#Region " Routines"

    Friend Sub subChangeRobot(ByRef sRobot As String)
        '********************************************************************************************
        'Description: select the robot type
        '
        'Parameters: robot
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 02/17/11  MSW     update axis list when robot is changed
        '********************************************************************************************

        Me.Text = gpsRM.GetString("psAXISJOG") & " " & sRobot
        mRobot = frmMain.colArms(sRobot)
        cboRobot.SelectedIndex = mRobot.RobotNumber - 1
        Dim sLeftRight As String
        'BTK 2/03/10 Can't use ArmNumber on a one robot per controller system.  
        Dim nArm As Integer = mRobot.RobotNumber 'mRobot.ArmNumber
        If nArm = ((nArm \ 2) * 2) Then
            sLeftRight = "Right"
        Else
            sLeftRight = "Left"
        End If
        Dim nAxis As Integer
        'Arm type:
        'Are you here looking to add robot models?
        'You'll need a case statement here and add all the pictures to "AxisJogPictures.resx"
        'the imagelists in this version don't like 
        Select Case mRobot.ArmType
            Case eArmType.P200
                mnMaxJoint = 6
                mnMaxWorldCoord = 3
                ReDim msPicNamesWorld(mnMaxWorldCoord)
                ReDim msPicNamesJoint(mnMaxJoint)
                msPicNameAll = sLeftRight & "_P200"
                For nAxis = 1 To mnMaxJoint
                    msPicNamesJoint(nAxis - 1) = sLeftRight & "_P200_A" & nAxis.ToString
                Next
                msPicNamesWorld(0) = sLeftRight & "_P200_WX"
                msPicNamesWorld(1) = sLeftRight & "_P200_WY"
                msPicNamesWorld(2) = sLeftRight & "_P200_WZ"

            Case eArmType.P250
                mnMaxJoint = 6
                mnMaxWorldCoord = 3
                ReDim msPicNamesWorld(mnMaxWorldCoord)
                ReDim msPicNamesJoint(mnMaxJoint)
                msPicNameAll = sLeftRight & "_P250"
                For nAxis = 1 To mnMaxJoint
                    msPicNamesJoint(nAxis - 1) = sLeftRight & "_P250_A" & nAxis.ToString
                Next
                msPicNamesWorld(0) = sLeftRight & "_P250_WX"
                msPicNamesWorld(1) = sLeftRight & "_P250_WY"
                msPicNamesWorld(2) = sLeftRight & "_P250_WZ"

            Case eArmType.P500
                mnMaxJoint = 5
                mnMaxWorldCoord = 3
                ReDim msPicNamesWorld(mnMaxWorldCoord)
                ReDim msPicNamesJoint(mnMaxJoint)
                msPicNameAll = sLeftRight & "_P500"
                For nAxis = 1 To mnMaxJoint
                    msPicNamesJoint(nAxis - 1) = sLeftRight & "_P500_A" & nAxis.ToString
                Next
                msPicNamesWorld(0) = sLeftRight & "_P500_WX"
                msPicNamesWorld(1) = sLeftRight & "_P500_WY"
                msPicNamesWorld(2) = sLeftRight & "_P500_WZ"

            Case eArmType.P700iA
                mnMaxJoint = 7
                mnMaxWorldCoord = 3
                ReDim msPicNamesWorld(mnMaxWorldCoord)
                ReDim msPicNamesJoint(mnMaxJoint)
                msPicNameAll = sLeftRight & "_P700"
                For nAxis = 1 To mnMaxJoint
                    msPicNamesJoint(nAxis - 1) = sLeftRight & "_P700_A" & nAxis.ToString
                Next
                msPicNamesWorld(0) = sLeftRight & "_P700_WX"
                msPicNamesWorld(1) = sLeftRight & "_P700_WY"
                msPicNamesWorld(2) = sLeftRight & "_P700_WZ"

            Case eArmType.P1000
                mnMaxJoint = 7
                mnMaxWorldCoord = 3
                ReDim msPicNamesWorld(mnMaxWorldCoord)
                ReDim msPicNamesJoint(mnMaxJoint)
                msPicNameAll = sLeftRight & "_P1000"
                For nAxis = 1 To mnMaxJoint
                    msPicNamesJoint(nAxis - 1) = sLeftRight & "_P1000_A" & nAxis.ToString
                Next
                msPicNamesWorld(0) = sLeftRight & "_P1000_WX"
                msPicNamesWorld(1) = sLeftRight & "_P1000_WY"
                msPicNamesWorld(2) = sLeftRight & "_P1000_WZ"

            Case eArmType.P20_Opener
                'TODO - look at Oakville for brake release.
                mnMaxJoint = 4
                mnMaxWorldCoord = 3
                ReDim msPicNamesWorld(mnMaxWorldCoord)
                ReDim msPicNamesJoint(mnMaxJoint)
                msPicNameAll = sLeftRight & "_P20"
                For nAxis = 1 To mnMaxJoint
                    msPicNamesJoint(nAxis - 1) = sLeftRight & "_P20_A" & nAxis.ToString
                Next
                msPicNamesWorld(0) = sLeftRight & "_P20_WX"
                msPicNamesWorld(1) = sLeftRight & "_P20_WY"
                msPicNamesWorld(2) = sLeftRight & "_P20_WZ"

            Case eArmType.P25_Opener
                mnMaxJoint = 6
                mnMaxWorldCoord = 3
                ReDim msPicNamesWorld(mnMaxWorldCoord)
                ReDim msPicNamesJoint(mnMaxJoint)
                msPicNameAll = sLeftRight & "_P25"
                For nAxis = 1 To mnMaxJoint
                    msPicNamesJoint(nAxis - 1) = sLeftRight & "_P25_A" & nAxis.ToString
                Next
                msPicNamesWorld(0) = sLeftRight & "_P25_WX"
                msPicNamesWorld(1) = sLeftRight & "_P25_WY"
                msPicNamesWorld(2) = sLeftRight & "_P25_WZ"

            Case eArmType.P35_Opener
                mnMaxJoint = 5
                mnMaxWorldCoord = 3
                ReDim msPicNamesWorld(mnMaxWorldCoord)
                ReDim msPicNamesJoint(mnMaxJoint)
                msPicNameAll = sLeftRight & "_P35"
                For nAxis = 1 To mnMaxJoint
                    msPicNamesJoint(nAxis - 1) = sLeftRight & "_P23_A" & nAxis.ToString
                Next
                msPicNamesWorld(0) = sLeftRight & "_P35_WX"
                msPicNamesWorld(1) = sLeftRight & "_P35_WY"
                msPicNamesWorld(2) = sLeftRight & "_P35_WZ"

            Case Else
                mnMaxJoint = 7
                mnMaxWorldCoord = 3
                ReDim msPicNamesWorld(mnMaxWorldCoord)
                ReDim msPicNamesJoint(mnMaxJoint)
                msPicNameAll = sLeftRight & "_P700"
                For nAxis = 1 To mnMaxJoint
                    msPicNamesJoint(nAxis - 1) = sLeftRight & "_P700_A" & nAxis.ToString
                Next
                msPicNamesWorld(0) = sLeftRight & "_P700_WX"
                msPicNamesWorld(1) = sLeftRight & "_P700_WY"
                msPicNamesWorld(2) = sLeftRight & "_P700_WZ"
        End Select

        'MSW 2/17/11 - update axis list when robot is changed
        If rdoJoint.Checked Then

            gpbAxis.Text = gpsRM.GetString("psJOINT")

            Dim nJoint As Integer

            For Each rdoTmp As RadioButton In gpbAxis.Controls
                If rdoTmp.Name <> "rdoDisable" Then
                    nJoint = CInt(rdoTmp.Name.Substring(7))
                    If nJoint <= mnMaxJoint Then
                        rdoTmp.Text = gpsRM.GetString("psAXIS") & " " & nJoint.ToString
                        rdoTmp.Visible = True
                    Else
                        rdoTmp.Visible = False
                    End If
                    rdoTmp.Checked = False
                End If
            Next

            gpbAxis.Visible = True
            rdoDisable.Checked = True
            Call subWriteAxisToPLC(0, eJogMode.Joint)

        End If


        Call subWriteRobotNumToPLC(mRobot.RobotNumber)
        picJog.Image = CType(Global.BSD.AxisJogPictures.ResourceManager.GetObject(msPicNameAll), Image)
        rdoDisable.Checked = True

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

        gpbCoord.Text = gpsRM.GetString("psCOORD")
        rdoJoint.Text = gpsRM.GetString("psJOINT")
        rdoWorld.Text = gpsRM.GetString("psWORLD")
        lblRobot.Text = gcsRM.GetString("csROBOT")
        rdoDisable.Text = gpsRM.GetString("psDISABLE")

        Call mPWRobotCommon.LoadRobotBoxFromCollection(cboRobot, frmMain.colArms, False)

        gpbCoord.Visible = True

    End Sub

    Private Sub subWriteAxisToPLC(ByVal nAxis As Integer, Optional ByVal eMode As eJogMode = eJogMode.NoVal)
        '********************************************************************************************
        'Description: Write the axis and coordinate system to the PLC
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim nVal As Integer
            Dim s(0) As String

            frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "JogAxisSelect"

            If eMode <> eJogMode.NoVal Then
                meJogMode = eMode
            End If
            If nAxis > 0 Then
                nVal = CInt(2 ^ (nAxis - 1)) Or CInt(meJogMode)
            Else
                nVal = CInt(meJogMode)
            End If
            s(0) = nVal.ToString
            frmMain.mPLC.PLCData = s

        Catch ex As Exception
            Call mDebug.ShowErrorMessagebox("Plc Error ", ex, frmMain.msSCREEN_NAME, frmMain.Status)
        End Try

    End Sub

    Private Sub subWriteJogActiveToPLC(ByVal bJogActive As Boolean)
        '********************************************************************************************
        'Description: Write the selected Axis to the PLC
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "AxisJogActive"
            Dim s(0) As String
            If bJogActive Then
                s(0) = "1"
            Else
                s(0) = "0"
            End If
            frmMain.mPLC.PLCData = s

        Catch ex As Exception
            Call mDebug.ShowErrorMessagebox("Plc Error ", ex, frmMain.msSCREEN_NAME, frmMain.Status)
        End Try
    End Sub

    Private Sub subWriteRobotNumToPLC(ByVal nRobNum As Integer)
        '********************************************************************************************
        'Description: Write the selected robot to the PLC
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try

            frmMain.mPLC.TagName = "Z" & colZones.CurrentZoneNumber & "JogRobotSelect"
            Dim s(0) As String
            s(0) = nRobNum.ToString
            frmMain.mPLC.PLCData = s

        Catch ex As Exception
            Call mDebug.ShowErrorMessagebox("Plc Error ", ex, frmMain.msSCREEN_NAME, frmMain.Status)
        End Try

    End Sub

#End Region

#Region " Events "

    Private Sub cboRobot_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboRobot.SelectedIndexChanged
        '********************************************************************************************
        'Description: Robot Selected
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If cboRobot.Text = String.Empty Then
            Exit Sub
        End If

        If Not (mRobot Is Nothing) Then
            If cboRobot.Text = mRobot.Name Then
                Exit Sub
            End If
        End If

        Call subChangeRobot(cboRobot.Text)

    End Sub
    Private Sub frmAxisJog_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '********************************************************************************************
        'Description: Make sure it shuts down properly when they click the red X.
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/12/11  RJO     Closing the form with the red "x" in the upper right caused the plc bit to stay latched
        '                   saying axis jog is enabled
        '********************************************************************************************

        Call subWriteRobotNumToPLC(0)
        Call subWriteAxisToPLC(0, eJogMode.Off)
        Call subWriteJogActiveToPLC(False)

    End Sub
    Friend Sub New()
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

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub rdoAxis_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '********************************************************************************************
        'Description: Axis Selected
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim rdoTmp As RadioButton = DirectCast(sender, RadioButton)

        If rdoTmp.Checked Then
            If rdoTmp.Name = "rdoDisable" Then
                picJog.Image = CType(Global.BSD.AxisJogPictures.ResourceManager.GetObject(msPicNameAll), Image)
                Call subWriteAxisToPLC(0)
            Else
                Dim nAxis As Integer = CInt(rdoTmp.Name.Substring(7)) - 1
                Dim sImageName As String = msPicNamesJoint(nAxis)

                If rdoWorld.Checked Then sImageName = msPicNamesWorld(nAxis)
                picJog.Image = CType(Global.BSD.AxisJogPictures.ResourceManager.GetObject(sImageName), Image)
                Call subWriteAxisToPLC(nAxis + 1)
            End If
        End If

    End Sub

    Private Sub rdoJoint_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoJoint.CheckedChanged
        '********************************************************************************************
        'Description: Joint jog selected
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        If rdoJoint.Checked = False Then Exit Sub

        gpbAxis.Text = gpsRM.GetString("psJOINT")

        Dim nJoint As Integer

        For Each rdoTmp As RadioButton In gpbAxis.Controls
            If rdoTmp.Name <> "rdoDisable" Then
                nJoint = CInt(rdoTmp.Name.Substring(7))
                If nJoint <= mnMaxJoint Then
                    rdoTmp.Text = gpsRM.GetString("psAXIS") & " " & nJoint.ToString
                    rdoTmp.Visible = True
                Else
                    rdoTmp.Visible = False
                End If
                rdoTmp.Checked = False
            End If
        Next

        gpbAxis.Visible = True
        rdoDisable.Checked = True
        Call subWriteAxisToPLC(0, eJogMode.Joint)

    End Sub

    Private Sub rdoWorld_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoWorld.CheckedChanged
        '********************************************************************************************
        'Description: World jog selected
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If rdoWorld.Checked = False Then Exit Sub
        gpbAxis.Text = gpsRM.GetString("psAXIS")
        Dim nAxis As Integer
        For Each rdoTmp As RadioButton In gpbAxis.Controls
            If rdoTmp.Name <> "rdoDisable" Then
                nAxis = CInt(rdoTmp.Name.Substring(7))
                If nAxis <= mnMaxWorldCoord Then
                    Select Case nAxis
                        Case 1
                            rdoTmp.Text = gpsRM.GetString("psCOORD_WX")
                            rdoTmp.Visible = True
                        Case 2
                            rdoTmp.Text = gpsRM.GetString("psCOORD_WY")
                            rdoTmp.Visible = True
                        Case 3
                            rdoTmp.Text = gpsRM.GetString("psCOORD_WZ")
                            rdoTmp.Visible = True
                        Case 7
                            rdoTmp.Text = gpsRM.GetString("psCOORD_WRAIL")
                            rdoTmp.Visible = True
                        Case Else
                            rdoTmp.Visible = False
                    End Select
                Else
                    rdoTmp.Visible = False
                End If
                rdoTmp.Checked = False
            End If
        Next
        ' picJog.Image = CType(Global.BSD.AxisJogPictures.ResourceManager.GetObject(msPicNameAll), Image)
        rdoDisable.Checked = True
        subWriteAxisToPLC(0, eJogMode.World)
    End Sub

    Friend Shadows Sub Show(ByRef sRobot As String)
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
        Call subWriteJogActiveToPLC(True)
        Call subInitFormText()
        Call subChangeRobot(sRobot)

        For Each rdoTmp As RadioButton In gpbAxis.Controls
            AddHandler rdoTmp.CheckedChanged, AddressOf rdoAxis_CheckedChanged
        Next

        MyBase.ShowDialog()

    End Sub

    Private Sub tlsMain_ItemClicked(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tlsMain.ItemClicked
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

        Select Case e.ClickedItem.Name
            Case "btnClose"
                Call subWriteRobotNumToPLC(0)
                Call subWriteAxisToPLC(0, eJogMode.Off)
                Call subWriteJogActiveToPLC(False)
                Me.Close()
        End Select

    End Sub

#End Region

End Class