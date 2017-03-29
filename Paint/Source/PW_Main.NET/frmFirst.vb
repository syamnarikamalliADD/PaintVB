'
' This material is the joint property of FANUC Robotics America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 1999-2009
' FANUC Robotics America
' FANUC LTD Japan
'
' Form: frmFirst.frm
'
' Description:
'
' Author: Rick O.
' FANUC Robotics America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
' By          Date              Reason
' MSW         04/19/12          Add error traps throughout the program
'*************************************************************************

Option Compare Text
Option Explicit On
Option Strict On

'TODO - Should add function to print or save the startup log

Friend Class frmFirst

#Region " Declares "

#End Region

#Region " Properties "

    Friend WriteOnly Property LoadError() As Boolean
        '********************************************************************************************
        'Description: Allow frmMain to make the Load Error panel visible if an error occurred.
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************

        Set(ByVal value As Boolean)
            pnlLoadError.Visible = value
        End Set

    End Property

#End Region

#Region " Routines "

    Private Sub subInitFormTexts()
        '********************************************************************************************
        'Description: Show the form text in the current language.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Try

            Dim CurrentCulture As System.Globalization.CultureInfo = frmMain.DisplayCulture

            With mLanguage.gpsRM
                btnAbort.Text = .GetString("psABORT_BTN_TXT", CurrentCulture)
                btnOK.Text = .GetString("psOK_BTN_TXT", CurrentCulture)
                lblLoadError.Text = .GetString("psERRORS_OCCURRED_TXT", CurrentCulture)
            End With
        Catch ex As Exception
            mDebug.WriteEventToLog("PW_MAIN" & " Module: " & "frmFirst" & " Routine: subInitFormTexts", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

#End Region

#Region " Events "

    Private Sub btnAbort_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAbort.Click
        '********************************************************************************************
        'Description: Errors occurred during Startup. Operator chooses not to continue.
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************

        frmMain.Abort = True

    End Sub

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        '********************************************************************************************
        'Description: Errors occurred during Startup. Operator says "Let's run it anyway!".
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************

        Me.Close()

    End Sub

    Private Sub frmFirst_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '********************************************************************************************
        'Description: let the Desktop Manager know this form is closing.
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************

        mPWDesktop.FormFirstVisible = False

    End Sub

    Private Sub frmFirst_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description: Initialize this form and let the Desktop Manager know this form is open.
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************

        mPWDesktop.FormFirstVisible = True
        Call subInitFormTexts()

    End Sub

    Private Sub frmFirst_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        '********************************************************************************************
        'Description: Size and Position this form.
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************

        If frmMain.InIDE Then
            'Center this form in the Working area
            Me.WindowState = FormWindowState.Normal
            Me.TopMost = False

            With frmMain.grecWorkingArea
                Me.Top = ((.Height - Me.Height) \ 2) + .Top
                Me.Left = (.Width - Me.Width) \ 2
            End With

        Else
            'This window covers the entire screen
            Me.WindowState = FormWindowState.Maximized
            Me.TopMost = True
            Application.DoEvents()



        End If
        'Center lstDebugOutput in working area, then move pnlLoadError directly below.
        Dim nOffset As Integer = pnlLoadError.Top - lstDebugOutput.Top

        lstDebugOutput.Left = (Me.Width - lstDebugOutput.Width) \ 2
        lstDebugOutput.Top = CInt(Me.Height * 0.4)
        pnlLoadError.Left = lstDebugOutput.Left
        pnlLoadError.Top = lstDebugOutput.Top + nOffset

        'Title and robot pic.  This works in development and with a 4:3 screen.  It may
        'Need adjustement for widescreen
        picFANUC.Width = CInt(Me.Width / 2.75)
        picFANUC.Height = CInt(picFANUC.Width * 0.16)
        picFANUC.Left = (Me.Width - picFANUC.Width) \ 2
        picFANUC.Top = CInt(Me.Height * 0.025)

        lblPAINTworks.AutoSize = True
        lblPAINTworks.Font = New System.Drawing.Font("Tahoma", CSng(Me.Height * 0.068), System.Drawing.FontStyle.Bold)
        lblPAINTworks.Text = gpsRM.GetString("psSHORT_CAP")

        lblPAINTworks.Top = picFANUC.Height + picFANUC.Top + 10
        lblPAINTworks.Left = (Me.Width - lblPAINTworks.Width) \ 2

        picRobot.Width = CInt(Me.Width * 0.436)
        picRobot.Height = CInt(Me.Height * 0.829)
        picRobot.Left = CInt(Me.Width * 0.635)
        picRobot.Top = CInt(Me.Height * 0.075)

    End Sub

#End Region






    Private Sub lblPAINTworks_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblPAINTworks.Click

    End Sub
End Class