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
' Form/Module: frmHelp
'
' Description: Display Alarm Help text.
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2005
'
' Author: Speedy and Associates
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    01/13/12   MSW     Support printing of help page, extract the specific alarm       4.01.01.01
'                       from the help file to keep it from getting too big.
'    01/14/14   MSW     Add some error handling when reading wbHelp.DocumentText      4.01.06.03
'********************************************************************************************

Option Compare Text
Option Explicit On
Option Strict On

Friend Class frmHelp

#Region " Declares "
    Private msAddress As String = Nothing
    Private msSaveAddress As String = Nothing
    Private msHEADER As String = "<HTML><HEAD><TITLE>Basic Diagnostic Resources</TITLE></HEAD><BODY topmargin=0 leftmargin=2 bgcolor=white>"
    Private msFOOTER As String = "</BODY></HTML>"
    Private msFullText As String = String.Empty
    Private msSingleAlarm As String = String.Empty
    Private msAlarm As String = String.Empty
    Private msFacilityCode As String = String.Empty
    Private msTitle As String
    Private msMODULE As String = "frmHelp.vb"
#End Region

#Region " Properties "

    Friend Property Target() As String
        '********************************************************************************************
        'Description: Navigate to the Target URL.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************


        Get
            Return msSaveAddress
        End Get
        Set(ByVal value As String)
            Try
                wbHelp.Navigate(value)
                msAddress = value
                msSaveAddress = value
                Dim sAddress() As String = Split(msAddress, "#")
                msAlarm = sAddress(sAddress.GetUpperBound(0))
                Dim sCode() As String = Split(msAlarm, "-")
                msFacilityCode = sCode(0)
            Catch ex As Exception
                mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Property: Target", _
                           "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            End Try
        End Set

    End Property

#End Region

#Region " Routines "

#End Region

#Region " Events "

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        '********************************************************************************************
        'Description: This button is hidden behind the WebBrowser control, but is tied to the 
        '             CancelButton property of frmHelp. This routine hides this form if the user 
        '             presses the Escape key, and returns the form to it's normal size and location.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim recScreen As Rectangle = Screen.GetBounds(frmMain)

        Me.Visible = False
        Me.Size = Me.MinimumSize
        Me.Top = (recScreen.Height - Me.Height) \ 2
        Me.Left = (recScreen.Width - Me.Width) \ 2

    End Sub

    Private Sub frmHelp_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description: Initialize button and menu text
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim Culture As System.Globalization.CultureInfo = frmMain.DisplayCulture
        btnFullFile.Text = gpsRM.GetString("psFULL_FILE", Culture)
        btnSingleAlarm.Text = gpsRM.GetString("psSINGLE_ALARM", Culture)
        With mnuPrintHelp
            .Text = gcsRM.GetString("csPRINTMNU", Culture)
            .ToolTipText = gpsRM.GetString("psPRINT_TT", Culture)
            .Enabled = True
        End With
        With mnuPreviewHelp
            .Text = gcsRM.GetString("csPRINT_PREVIEW", Culture)
            .ToolTipText = gpsRM.GetString("psPRINT_PREVIEW_MNU_TT", Culture)
            .Enabled = True
        End With
        With mnuPageSetupHelp
            .Text = gcsRM.GetString("csPAGE_SETUP", Culture)
            .ToolTipText = gpsRM.GetString("psPAGE_SETUP_MNU_TT", Culture)
            .Enabled = True
        End With
        With mnuPrintFileHelp
            .Text = gcsRM.GetString("csPRINT_FILE", Culture)
            .ToolTipText = gpsRM.GetString("psPRINT_FILE_MNU_TT", Culture)
            .Enabled = True
        End With
    End Sub

    Private Sub frmHelp_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        '********************************************************************************************
        'Description: Locate this form in the center of the screen when first shown.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim recScreen As Rectangle = Screen.GetBounds(frmMain)

        Me.Top = (recScreen.Height - Me.Height) \ 2
        Me.Left = (recScreen.Width - Me.Width) \ 2

    End Sub

#End Region

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub wbHelp_DocumentCompleted(ByVal sender As Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles wbHelp.DocumentCompleted
        '********************************************************************************************
        'Description: Pull the single alarm out of the file and build a web page for the alarm so it's small enough to print
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If msAddress <> String.Empty Then
                msAddress = String.Empty
                If msAlarm <> String.Empty Then
                    Dim sOut As String = msHEADER
                    Try
                    msFullText = wbHelp.DocumentText
                    Catch ex As Exception
                        'Might get a wierd error here.  Not sure what to do so I'll pretend it didn't happen
                    End Try

                    Dim oPage As IO.StreamReader = New IO.StreamReader(wbHelp.DocumentStream)
                    Dim bFound As Boolean = False
                    Dim bDone As Boolean = False
                    Do While oPage.Peek() >= 0 And bDone = False
                        Dim sLine As String = oPage.ReadLine
                        If bFound Then
                            If InStr(sLine, "<a", CompareMethod.Text) > 0 Then
                                bDone = True
                            Else
                                sOut = sOut & sLine
                            End If
                        Else
                            If InStr(sLine, msAlarm, CompareMethod.Text) > 0 Then
                                bFound = True
                                sOut = sOut & sLine
                            End If
                        End If
                        Debug.Print(sLine)
                    Loop
                    sOut = sOut & msFOOTER
                    msSingleAlarm = sOut
                    wbHelp.DocumentText = sOut
                    btnSingleAlarm.Checked = True
                    btnFullFile.Checked = False
                    msTitle = Me.Text & " - " & msAlarm
                End If
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: wbHelp_DocumentCompleted", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
   
    Private Sub btnSingleAlarm_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSingleAlarm.Click
        '********************************************************************************************
        'Description: Show single alarm
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            wbHelp.DocumentText = msSingleAlarm
            btnSingleAlarm.Checked = True
            btnFullFile.Checked = False
            msTitle = Me.Text & " - " & msAlarm
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: btnSingleAlarm_Click", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub btnFullFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFullFile.Click
        '********************************************************************************************
        'Description: Show full alarm help file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            wbHelp.Navigate(msSaveAddress)
            'wbHelp.DocumentText = msFullText
            btnSingleAlarm.Checked = False
            btnFullFile.Checked = True
            msTitle = Me.Text & " - " & msFacilityCode
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: btnFullFile_Click", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
    Private Sub tlspPrintHelp_ButtonClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tlspPrintHelp.ButtonClick
        '********************************************************************************************
        'Description: print function
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Try
            If btnSingleAlarm.Checked Then
                frmMain.mPrintHtml.subCreateFromHtml(msSingleAlarm)
            Else
                frmMain.mPrintHtml.subCreateFromHtml(msFullText)
            End If
            frmMain.mPrintHtml.subPrintDoc(True)
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: tlspPrintHelp_ButtonClick", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub mnuPreviewHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPreviewHelp.Click
        '********************************************************************************************
        'Description: print preview
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If btnSingleAlarm.Checked Then
                frmMain.mPrintHtml.subCreateFromHtml(msSingleAlarm)
            Else
                frmMain.mPrintHtml.subCreateFromHtml(msFullText)
            End If
            frmMain.mPrintHtml.subShowPrintPreview()
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuPreviewHelp_Click", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub mnuPrintHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintHelp.Click
        '********************************************************************************************
        'Description: print function
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If btnSingleAlarm.Checked Then
                frmMain.mPrintHtml.subCreateFromHtml(msSingleAlarm)
            Else
                frmMain.mPrintHtml.subCreateFromHtml(msFullText)
            End If
            frmMain.mPrintHtml.subPrintDoc(True)
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuPrintHelp_Click", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub mnuPageSetupHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPageSetupHelp.Click
        '********************************************************************************************
        'Description: print setup
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If btnSingleAlarm.Checked Then
                frmMain.mPrintHtml.subCreateFromHtml(msSingleAlarm)
            Else
                frmMain.mPrintHtml.subCreateFromHtml(msFullText)
            End If
            frmMain.mPrintHtml.subShowPageSetup()
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuPageSetupHelp_Click", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub mnuPrintFileHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintFileHelp.Click
        '********************************************************************************************
        'Description: print setup
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If btnSingleAlarm.Checked Then
                frmMain.mPrintHtml.subCreateFromHtml(msSingleAlarm)
            Else
                frmMain.mPrintHtml.subCreateFromHtml(msFullText)
            End If
            frmMain.mPrintHtml.subSaveAs()
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.msSCREEN_NAME & " Module: " & msMODULE & " Routine: mnuPrintFileHelp_Click", _
                       "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub
End Class