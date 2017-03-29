'
' This material is the joint property of FANUC Robotics North America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics North America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2009
' FANUC Robotics America, Inc.
' FANUC LTD Japan
'
' Form: frmstart.frm
'
' NOTE: If the filename is the same as the module name, only the file's extension is shown.  RJK
' Dependencies:
'
' Description:   PAINTworks Scheduled Maintenance Startup Form
'
' The purpose of this form is to warn the user that Scheduled Maintenance
' will start soon and then count down until the startup delay expires. The
' Scheduled Maintenance main form is then shown.
'
' Author: R. Olejniczak
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
' Date      By      Reason                      Version

Public Class frmStart

#Region " Declares "

    '******** Form Constants   **********************************************************************
    '  msSCREEN_NAME = root namespace or you won't find the resources
    Public Const msSCREEN_NAME As String = "PW_Maint"   ' <-- For password area change log etc.
    Private Const msMODULE As String = "frmStart"
    Friend Const msBASE_ASSEMBLY_COMMON As String = msSCREEN_NAME & ".CommonStrings"
    Friend Const msBASE_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".ProjectStrings"
    Friend Const msROBOT_ASSEMBLY_LOCAL As String = msSCREEN_NAME & ".RobotStrings"
    '******** End Form Constants    *****************************************************************

    '******** Form Variables   **********************************************************************
    Private mbInIDE As Boolean
    Private mnFlags As Integer
    Private mnTimeLeft As Integer
    Private msCulture As String = "en-US" 'Default to English
    Private mbManual As Boolean = False
    '******** End Form Variables    *****************************************************************
#End Region

#Region " Properties "

    Friend WriteOnly Property Culture() As String
        '********************************************************************************************
        'Description:  Write to this property to change the screen language.
        '
        'Parameters: Culture String (ex. "en-US")
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As String)

            msCulture = value
            mLanguage.DisplayCultureString = value

            'Use current language text for screen labels
            Dim Void As Boolean = mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, _
                                                                msBASE_ASSEMBLY_LOCAL, _
                                                                String.Empty)

        End Set

    End Property

    Friend ReadOnly Property DisplayCulture() As CultureInfo
        '********************************************************************************************
        'Description:  The Culture Club
        '
        'Parameters: None
        'Returns:    CultureInfo for current culture.
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return New CultureInfo(msCulture)
        End Get

    End Property

    Friend Property Flags() As Integer
        '********************************************************************************************
        'Description:  Requested Maintenance Functions Flags
        '
        'Parameters: None
        'Returns:    Maintenance request flags
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Get
            Return mnFlags
        End Get

        Set(ByVal value As Integer)
            mnFlags = value
        End Set

    End Property

    Friend ReadOnly Property InIDE() As Boolean
        '********************************************************************************************
        'Description: Test to determine if running from the Visual Studio IDE
        '
        'Parameters: None
        'Returns:    True if running in Visual Studio IDE
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            mbInIDE = False
            Debug.Assert(IsInIDE)
            InIDE = mbInIDE
        End Get

    End Property
    Friend ReadOnly Property Manual() As Boolean
        '********************************************************************************************
        'Description: Running maintenace manually
        '
        'Parameters: None
        'Returns:    True if running in Visual Studio IDE
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            Manual = mbManual
        End Get
    End Property

    Private ReadOnly Property IsInIDE() As Boolean
        '********************************************************************************************
        'Description: The actual test to detrmine if running from the Visual Studio IDE. This called 
        '             from the InIDE Property via a Debug.Assert so it'll never get here if the
        '             compiled EXE is running.
        '
        'Parameters: None
        'Returns:    True if running in Visual Studio IDE
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Get
            mbInIDE = True
            IsInIDE = True
        End Get

    End Property

#End Region

#Region " Routines "

    Private Function sFormatTimeLeft(ByVal TimeLeft As Integer) As String
        '********************************************************************************************
        'Description:   returns a formatted string mm:ss
        '
        'Parameters:  number of seconds
        '
        'Returns:    formatted string mm:ss
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nMinutes As Integer = TimeLeft \ 60
        Dim nSeconds As Integer = TimeLeft Mod 60

        Return nMinutes & ":" & Strings.Format(nSeconds, "00")

    End Function

    Private Sub subInitializeForm()
        '********************************************************************************************
        'Description: Put all startup code here, not in the form load event. this is called
        '             right after form is shown.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        mnTimeLeft = 120 'seconds

        'put text in all of the captions
        Me.Text = gpsRM.GetString("psSTARTUP_TITLE", DisplayCulture)
        lblTimeRemaining.Text = gpsRM.GetString("psTIME_LEFT_LBL", DisplayCulture)
        btnOK.Text = gcsRM.GetString("csOK", DisplayCulture)
        btnCancel.Text = gcsRM.GetString("csCANCEL", DisplayCulture)

        'initialize the time remaining display
        lblCountDown.Text = sFormatTimeLeft(mnTimeLeft)
        If mbManual Then
            lblTimeRemaining.Visible = False
            lblMsg.Width = pnlCheckBoxes.Left - lblMsg.Left
            lblMsg.Text = gpsRM.GetString("psSTARTUP_MANUAL_MSG", DisplayCulture)
            chkCleanup.Text = gpsRM.GetString("psCLEANUP_OP", DisplayCulture)
            chkRobotBackup.Text = gpsRM.GetString("psROB_BACKUP_OP", DisplayCulture)
            chkFiles.Text = gpsRM.GetString("psFILE_BACKUP_OP", DisplayCulture)
            pnlCheckBoxes.Visible = True
            lblCountDown.Visible = False
        Else
            pnlCheckBoxes.Visible = False
            lblMsg.Text = gpsRM.GetString("psSTARTUP_MSG", DisplayCulture)
            lblCountDown.Visible = True
            With tmrCountDown
                If InIDE Then
                    .Interval = 1
                Else
                    .Interval = 1000
                End If
                .Enabled = True
            End With
        End If
        'start the countdown

    End Sub

#End Region

#Region " Events "

    Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        '********************************************************************************************
        'Description:   The user has chosen to abort scheduled maintenance. Write abort message to
        '               the alarm log and the scheduled maintenance log, then exit.
        '
        'Parameters:  none
        '
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'TODO - There's no PW4 utility to write an anonymous alarm to the alarm log. Should a bit be set in the PLC to log this alarm??
        'Call pw3api.LogPWMsg("GUI", msSCREEN_CAPTION & Localize.GetLocalString(PString.psMAINT_ABORT), MAINT_ABORT_MSG)
        'Call frmMain.WriteMaintLogEntry(msSCREEN_NAME & gpsRM.GetString("psMAINT_ABORT", DisplayCulture))
        Me.Close()

    End Sub

    Private Sub btnOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOK.Click
        '********************************************************************************************
        'Description:   hides this screen while the countdown continues
        '
        'Parameters:  none
        '
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        If mbManual Then
            mnFlags = 0
            If chkCleanup.Checked Then
                mnFlags = mnFlags + frmMain.eFlags.Cleanup
            End If
            If chkRobotBackup.Checked Then
                mnFlags = mnFlags + frmMain.eFlags.RobotBackup
            End If
            If chkFiles.Checked Then
                mnFlags = mnFlags + frmMain.eFlags.FileBackup
            End If
            'Call subShowMainForm()
            frmMain.Show()
        End If
        Me.Hide()

    End Sub

    Private Sub frmStart_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
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
        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            Select Case e.KeyCode

                Case Keys.N
                    'count down expired. Show frmMain
                    tmrCountDown.Enabled = False
                    'Call subShowMainForm()
                    frmMain.Show()
                    Me.Hide()
                Case Keys.Escape
                    Me.Close()
                Case Else

            End Select
        End If


    End Sub


    Private Sub frmStart_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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

        Try
            Dim sCultureArg As String = "/culture="
            Dim bFlagsSet As Boolean = False
            For Each s As String In My.Application.CommandLineArgs
                If s.ToLower.StartsWith(sCultureArg) Then
                    'If a culture string has been passed in, set the current culture (display language)
                    Culture = s.Remove(0, sCultureArg.Length)
                ElseIf Microsoft.VisualBasic.Information.IsNumeric(s) Then
                    'Maintenance request flags
                    Flags = Integer.Parse(s)
                    bFlagsSet = True
                ElseIf s.ToLower = "manual" Then
                    mbManual = True
                End If
            Next 's
            If bFlagsSet = False Then
                mbManual = True
            End If
            'center the form on the screen
            Me.Left = (Screen.GetBounds(Screen.PrimaryScreen.Bounds).Width - Me.Width) \ 2
            Me.Top = (Screen.GetBounds(Screen.PrimaryScreen.Bounds).Height - Me.Height) \ 2

            Call subInitializeForm()

        Catch ex As Exception
            mDebug.WriteEventToLog(msSCREEN_NAME & " Module: " & msMODULE & " Routine: frmStart_Load", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Public Sub New()
        '********************************************************************************************
        'Description:  Class Constructor
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        'for language support
        mLanguage.GetResourceManagers(msBASE_ASSEMBLY_COMMON, msBASE_ASSEMBLY_LOCAL, _
                                            msROBOT_ASSEMBLY_LOCAL)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub tmrCountDown_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrCountDown.Tick
        '********************************************************************************************
        'Description:   decrements the modular variable that holds the time (in seconds) remaining
        '               before frmMain is shown and then updates the remaining time display.
        '
        'Parameters:  none
        '
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        mnTimeLeft = mnTimeLeft - 1

        If mnTimeLeft < 0 Then

            'count down expired. Show frmMain
            tmrCountDown.Enabled = False
            'Call subShowMainForm()
            frmMain.Show()
            Me.Hide()
        End If

        'update the time remaining display
        lblCountDown.Text = sFormatTimeLeft(mnTimeLeft)

    End Sub

#End Region

End Class
