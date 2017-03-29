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
' Form/Module: frmOverrides
'
' Description: set/save override values
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
'    Date       By      Reason                                                        Version
' 11/10/09      MSW     Modifications for PaintTool 7.50 presets
'                       Reads preset config from robot.
'           			enables or disables each override slider per config
' 11/24/09      MSW     update display as it's changing
' 04/02/13      DE      Added subInitFormText to localize screen text
' 09/30/13      MSW     Save screenshots as jpegs
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On


Imports System.Windows.Forms
Imports Response = System.Windows.Forms.DialogResult


Friend Class frmOverrides

#Region " Declares "

    Private mRobot As clsArm = Nothing
    Private oPresets As clsPresets = Nothing
    Private msFORM_NAME As String = "OVERRIDE"
    Private mbEditsMade As Boolean
    Private msIgnoreEvent As Boolean = False
    Friend Event UpdateInfo(ByVal bClosing As Boolean)

#End Region
#Region " Properties "
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
            btnSave.Enabled = Value
        End Set
    End Property

#End Region
#Region " Routines"
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
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button3)

        Select Case lRet
            Case Response.Yes 'Response.Yes
                subSaveData()
                Return True
            Case Response.Cancel
                Return False
            Case Else
                If mPresetCommon.LoadOverRidesFromRobot(mRobot, oPresets) Then
                    RaiseEvent UpdateInfo(False)
                End If
                Return True
        End Select

    End Function

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
        ' 04/02/13  DE      Added to localize screen text
        '********************************************************************************************

        btnClose.Text = gcsRM.GetString("csCLOSE")
        btnSave.Text = gcsRM.GetString("csSAVE")
        btnDefault.Text = gpsRM.GetString("psDEFAULT")
        chkEnable.Text = gpsRM.GetString("psENABLE_OVERRIDES")

    End Sub

    Private Sub subSaveData()
        '********************************************************************************************
        'Description: Save me!
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            For nParm As Integer = 1 To oPresets.NumParms
                Dim eParam As ePresetParam = CType(nParm - 1, ePresetParam)

                Dim tkbTemp As TrackBar = DirectCast(tscMain.ContentPanel.Controls("tkbParm" & nParm.ToString("00")), TrackBar)
                oPresets.SetOverRidePercent(eParam, tkbTemp.Value)
            Next

            'set the value in each member
            oPresets.SetEnableOverRide(chkEnable.Checked)

            If mPresetCommon.SaveOverRidesToRobot(mRobot, oPresets) Then
                EditsMade = False
                RaiseEvent UpdateInfo(False)
            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)

        End Try

    End Sub
    Friend Shadows Sub Show(ByRef rRobot As clsArm, ByRef rPresets As clsPresets)
        '********************************************************************************************
        'Description: Here I am!
        '
        'Parameters: robot, presets
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/25/09  MSW     reset editsMade
        ' 04/02/13  DE      Added call to subInitFormText
        '********************************************************************************************

        mRobot = rRobot
        oPresets = rPresets
        msIgnoreEvent = True
        chkEnable.Checked = oPresets.OverRideEnabled.Value
        tkbParm01.Visible = False
        tkbParm02.Visible = False
        tkbParm03.Visible = False
        tkbParm04.Visible = False
        lblParm01.Visible = False
        lblParm02.Visible = False
        lblParm03.Visible = False
        lblParm04.Visible = False

        Call subInitFormText() 'DE 04/02/13

        For nParm As Integer = 1 To oPresets.NumParms
            Dim eParam As ePresetParam = CType(nParm - 1, ePresetParam)
            Dim tkbTemp As TrackBar = DirectCast(tscMain.ContentPanel.Controls("tkbParm" & nParm.ToString("00")), TrackBar)
            tkbTemp.Value = oPresets.OverRidePercent(eParam).Value
            'Debug.Print(oPresets.OverRidePercent(eParam).Value.ToString)
            tkbTemp.Visible = True
            Dim lblTemp As Label = DirectCast(tscMain.ContentPanel.Controls("lblParm" & nParm.ToString("00")), Label)
            lblTemp.Text = frmMain.mParmDetail(nParm - 1).sLblName & " - " & tkbTemp.Value.ToString & "%"
            lblTemp.Visible = True
        Next

        'update screen 
        Dim o As Object = Nothing
        Dim e As EventArgs = Nothing
        subScrollHandler(o, e)

        AddHandler tkbParm02.ValueChanged, AddressOf subScrollHandler
        AddHandler tkbParm01.ValueChanged, AddressOf subScrollHandler
        AddHandler tkbParm03.ValueChanged, AddressOf subScrollHandler
        AddHandler tkbParm04.ValueChanged, AddressOf subScrollHandler
        msIgnoreEvent = False
        Me.StartPosition = FormStartPosition.CenterScreen
        EditsMade = False 'MSW 11/24/09
        MyBase.Show()

    End Sub

    Private Sub subScrollHandler(ByVal sender As Object, ByVal e As EventArgs)
        '********************************************************************************************
        'Description: do the labels
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/24/09  MSW     update display as it's changing
        '********************************************************************************************

        For nParm As Integer = 1 To oPresets.NumParms
            Dim eParam As ePresetParam = CType(nParm - 1, ePresetParam)
            Dim tkbTemp As TrackBar = DirectCast(tscMain.ContentPanel.Controls("tkbParm" & nParm.ToString("00")), TrackBar)
            Dim lblTemp As Label = DirectCast(tscMain.ContentPanel.Controls("lblParm" & nParm.ToString("00")), Label)
            lblTemp.Text = frmMain.mParmDetail(nParm - 1).sLblName & " - " & tkbTemp.Value.ToString & "%"
            If oPresets.OverRidePercent(eParam).Value <> tkbTemp.Value Then EditsMade = True
            oPresets.SetOverRidePercent(eParam, tkbTemp.Value)
        Next
        '********************************************************************************************
        EditsMade = True
        RaiseEvent UpdateInfo(False)

    End Sub

#End Region

#Region " Events "

    Private Sub chkEnable_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                                                         Handles chkEnable.CheckStateChanged
        '********************************************************************************************
        'Description: 
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 11/24/09  MSW     update display as it's changing
        '********************************************************************************************
        Dim bChecked As Boolean = chkEnable.Checked

        lblParm02.Enabled = bChecked
        lblParm03.Enabled = bChecked
        lblParm01.Enabled = bChecked
        lblParm04.Enabled = bChecked
        tkbParm01.Enabled = bChecked
        tkbParm03.Enabled = bChecked
        tkbParm02.Enabled = bChecked
        tkbParm04.Enabled = bChecked

        If oPresets.OverRideEnabled.Value <> chkEnable.Checked Then EditsMade = True
        'set the value in each member
        oPresets.SetEnableOverRide(chkEnable.Checked)
        EditsMade = True
        RaiseEvent UpdateInfo(False)

    End Sub

    Friend Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Protected Overrides Sub Finalize()
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

        Try
            RemoveHandler tkbParm02.ValueChanged, AddressOf subScrollHandler
            RemoveHandler tkbParm01.ValueChanged, AddressOf subScrollHandler
            RemoveHandler tkbParm03.ValueChanged, AddressOf subScrollHandler
            RemoveHandler tkbParm04.ValueChanged, AddressOf subScrollHandler

            RaiseEvent UpdateInfo(True)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)

        End Try

        MyBase.Finalize()

    End Sub

    Private Sub btnDefault_Click(ByVal sender As System.Object, _
                            ByVal e As System.EventArgs) Handles btnDefault.Click
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

        tkbParm01.Value = 100
        tkbParm02.Value = 100
        tkbParm03.Value = 100
        tkbParm04.Value = 100

        chkEnable.Checked = False

    End Sub

    Private Sub frmOverrides_FormClosing(ByVal sender As Object, _
            ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
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

    Private Sub frmOverrides_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
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
                    sKeyValue = "btnHelp"
                    Select Case frmMain.ScreenSelect
                        Case frmMain.eScreenSelect.CCPresets
                            subLaunchHelp(gs_HELP_CCPRESETS, frmMain.oIPC)
                        Case frmMain.eScreenSelect.EstatPresets
                            subLaunchHelp(gs_HELP_ESTATPRESETS_OVERRIDE, frmMain.oIPC)
                        Case frmMain.eScreenSelect.FluidPresets
                            subLaunchHelp(gs_HELP_PRESETS_OVERRIDE, frmMain.oIPC)
                        Case Else
                            subLaunchHelp(gs_HELP_PRESETS_OVERRIDE, frmMain.oIPC)
                    End Select
                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty

                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    Select Case frmMain.ScreenSelect
                        Case frmMain.eScreenSelect.CCPresets
                            oSC.CaptureWindowToFile(Me.Handle, sDumpPath & frmMain.msSCREEN_DUMP_NAME_CC & _
                                                    "_" & msFORM_NAME & frmMain.msSCREEN_DUMP_EXT, Imaging.ImageFormat.Jpeg)
                        Case frmMain.eScreenSelect.EstatPresets
                            oSC.CaptureWindowToFile(Me.Handle, sDumpPath & frmMain.msSCREEN_DUMP_NAME_ESTAT & _
                                                    "_" & msFORM_NAME & frmMain.msSCREEN_DUMP_EXT, Imaging.ImageFormat.Jpeg)
                        Case frmMain.eScreenSelect.FluidPresets
                            oSC.CaptureWindowToFile(Me.Handle, sDumpPath & frmMain.msSCREEN_DUMP_NAME & _
                                                    "_" & msFORM_NAME & frmMain.msSCREEN_DUMP_EXT, Imaging.ImageFormat.Jpeg)
                        Case Else
                            oSC.CaptureWindowToFile(Me.Handle, sDumpPath & frmMain.msSCREEN_DUMP_NAME & _
                                                    "_" & msFORM_NAME & frmMain.msSCREEN_DUMP_EXT, Imaging.ImageFormat.Jpeg)
                    End Select

                Case Keys.Escape
                    Me.Close()
                Case Else
            End Select
        End If

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
                Me.Close()
            Case "btnSave"
                subSaveData()
        End Select

    End Sub

#End Region

End Class