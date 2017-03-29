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
' Form/Module: frmSteps
'
' Description: set/save Step values - just view for now
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
'    09/30/13   MSW     Save screenshots as jpegs
'********************************************************************************************
Option Compare Binary
Option Explicit On
Option Strict On


Imports System.Windows.Forms
Imports Response = System.Windows.Forms.DialogResult


Friend Class frmSteps

#Region " Declares "

    Private mRobot As clsArm = Nothing
    Private mValues As String()

    Private mbEditsMade As Boolean

    Private msFORM_NAME As String = "STEPS"

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
                Return True
        End Select

    End Function
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

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)

        End Try

    End Sub
    Friend Shadows Sub Show(ByRef rRobot As clsArm, ByRef Values As String())
        '********************************************************************************************
        'Description: Here I am!
        '
        'Parameters: robot, presets
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        mRobot = rRobot
        mvalues = Values
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Text = mRobot.Name
        subLoadData()
        MyBase.Show()

    End Sub
    Private Sub subLoadData()
        '********************************************************************************************
        'Description: 
        '
        'Parameters: 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        txtStep01.Text = mValues(1)
        txtStep02.Text = mValues(2)
        txtStep03.Text = mValues(3)
        txtStep04.Text = mValues(4)
        txtStep05.Text = mValues(5)
        txtStep06.Text = mValues(6)
        txtStep07.Text = mValues(7)

        txtStep01.NumericOnly = True
        txtStep02.NumericOnly = True
        txtStep03.NumericOnly = True
        txtStep04.NumericOnly = True
        txtStep05.NumericOnly = True
        txtStep06.NumericOnly = True
        txtStep07.NumericOnly = True

        ' for now this is just a viewer
        txtStep01.ReadOnly = True
        txtStep02.ReadOnly = True
        txtStep03.ReadOnly = True
        txtStep04.ReadOnly = True
        txtStep05.ReadOnly = True
        txtStep06.ReadOnly = True
        txtStep07.ReadOnly = True


        subShowNewPage()


        EditsMade = False

    End Sub
    Private Sub subShowNewPage()
        '********************************************************************************************
        'Description: 
        '
        'Parameters: 
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'With mPresets
        '    If .EstatStepValue(1).Changed Then
        '        txtStep01.ForeColor = Color.Red
        '    Else
        txtStep01.ForeColor = Color.Black
        '    End If
        '    If .EstatStepValue(2).Changed Then
        '        txtStep02.ForeColor = Color.Red
        '    Else
        txtStep02.ForeColor = Color.Black
        '    End If
        '    If .EstatStepValue(3).Changed Then
        '        txtStep03.ForeColor = Color.Red
        '    Else
        txtStep03.ForeColor = Color.Black
        '    End If
        '    If .EstatStepValue(4).Changed Then
        '        txtStep04.ForeColor = Color.Red
        '    Else
        txtStep04.ForeColor = Color.Black
        '    End If
        '    If .EstatStepValue(5).Changed Then
        '        txtStep05.ForeColor = Color.Red
        '    Else
        txtStep05.ForeColor = Color.Black
        '    End If
        '    If .EstatStepValue(6).Changed Then
        '        txtStep06.ForeColor = Color.Red
        '    Else
        txtStep06.ForeColor = Color.Black
        '    End If
        '    If .EstatStepValue(7).Changed Then
        '        txtStep07.ForeColor = Color.Red
        '    Else
        txtStep07.ForeColor = Color.Black
        '    End If

        'End With
    End Sub
#End Region
#Region " Events "


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

            RaiseEvent UpdateInfo(True)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)

        End Try

        MyBase.Finalize()
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
    Private Sub txtStep0_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
                                                        txtStep01.TextChanged, _
                                                        txtStep02.TextChanged, _
                                                        txtStep03.TextChanged, _
                                                        txtStep04.TextChanged, _
                                                        txtStep05.TextChanged, _
                                                        txtStep06.TextChanged, _
                                                        txtStep07.TextChanged
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
        EditsMade = True

    End Sub
#End Region



  

    Private Sub frmSteps_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
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
                            subLaunchHelp(gs_HELP_ESTATPRESETS_STEPS, frmMain.oIPC)
                        Case frmMain.eScreenSelect.FluidPresets
                            subLaunchHelp(gs_HELP_PRESETS, frmMain.oIPC)
                        Case Else
                            subLaunchHelp(gs_HELP_ESTATPRESETS_STEPS, frmMain.oIPC)
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
End Class