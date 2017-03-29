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
' Copyright (C) 2009
' FANUC Robotics America
' FANUC LTD Japan
'
' Form: AboutBox.frm
'
' Description: The purpose of this form is to display information about PAINTworks.
'
'
' Author: R. Olejniczak
' FANUC Robotics America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
' By          Date              Reason
' MSW         04/11/12          Switch to the current MSInfo App.  It doesn't need all the menu mods  4.1.3.0
' RJO         10/24/12          In cboLanguage_SelectedValueChanged, removed code to set 
'                               "CurrentLanguage" value in registry. No longer needed.
'********************************************************************************************

Option Compare Text
Option Explicit On
Option Strict On

Public Class frmAbout

#Region " Declares "

    Private mbLoading As Boolean
    'Private msAppPath As String = String.Empty
    Private msSiteName As String = String.Empty

    Private Const msSCREEN_DUMP_NAME As String = "PaintWorks_About.jpg"

    Private Const MF_BYPOSITION As Int32 = &H400&
    Private Const MF_BYCOMMAND As Int32 = &H0&
    Private Const SC_MINIMIZE As Int32 = &HF020&

    Private Declare Function GetMenu Lib "user32" (ByVal hwnd As Int32) As Int32
    Private Declare Function GetMenuItemCount Lib "user32" (ByVal hMenu As Int32) As Int32
    Private Declare Function GetMenuItemID Lib "user32" (ByVal hMenu As Int32, ByVal nPos As Int32) As Int32
    Private Declare Function GetMenuString Lib "user32" Alias "GetMenuStringA" _
                                              (ByVal hMenu As Int32, ByVal wIDItem As Int32, _
                                               ByVal lpString As String, ByVal ByvalnMaxCount As Int32, _
                                               ByVal wFlag As Int32) As Int32
    Public Declare Function GetSubMenu Lib "user32" (ByVal hMenu As Int32, ByVal nPos As Int32) As Int32
    Public Declare Function GetSystemMenu Lib "user32" (ByVal hwnd As Int32, ByVal bRevert As Int32) As Int32
    Public Declare Function RemoveMenu Lib "user32" (ByVal hMenu As Int32, ByVal nPosition As Int32, _
                                                     ByVal wFlags As Int32) As Int32

#End Region

#Region " Properties "


    Friend WriteOnly Property SiteName() As String
        '********************************************************************************************
        'Description: Allow frmMain to pass in the Site Name
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As String)
            msSiteName = value
        End Set

    End Property

#End Region

#Region " Routines "

    Private Function sParseMenuItem(ByVal MenuItem As String) As String
        '********************************************************************************************
        'Description:   Removes unwanted characters from System Info Menu Item strings
        '
        'Parameters:    Menu Item string(raw from API call)
        'Returns:       Menu Item string
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Try
            Dim nPos As Integer

            sParseMenuItem = Strings.Trim(MenuItem)

            'get rid of hot key code string (if any)
            nPos = Strings.InStr(sParseMenuItem, Strings.Chr(9))
            If nPos > 0 Then sParseMenuItem = Strings.Left(sParseMenuItem, nPos - 1)

            'get rid of nulls (if any)
            nPos = InStr(sParseMenuItem, Chr(0))
            If nPos > 0 Then sParseMenuItem = Strings.Left(sParseMenuItem, nPos - 1)

            'get rid of hotkey delimiter (if any)
            nPos = InStr(sParseMenuItem, "&")
            If nPos = 1 Then
                sParseMenuItem = Strings.Right(sParseMenuItem, Strings.Len(sParseMenuItem) - 1)
            Else
                If nPos > 0 Then
                    sParseMenuItem = Strings.Left(sParseMenuItem, nPos - 1) & _
                                     Strings.Right(sParseMenuItem, Len(sParseMenuItem) - nPos)
                End If
            End If

        Catch ex As Exception
            mDebug.WriteEventToLog("PW4_Main Module: frmAbout Routine: sParseMenuItem", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            sParseMenuItem = String.Empty
        End Try

    End Function

    Private Sub subLoadControlCaptions()
        '********************************************************************************************
        'Description:   Loads the captions of all screen controls with current language strings.
        '
        'Parameters:    None
        'Returns:       none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Try
            Dim oCulture As CultureInfo = frmMain.DisplayCulture

            Me.Text = gpsRM.GetString("psABOUT_TXT", oCulture) & " PAINTworks"
            lblCopyrightInfo.Text = gpsRM.GetString("psCOPYRIGHT_TXT", oCulture)
            lblPWVersion.Text = "PAINTworks " & gpsRM.GetString("psPW_VERSION_TXT", oCulture) & Application.ProductVersion
            If frmMain.gbREGISTRATION_ENABLED Then
                lblRegistered.Text = gpsRM.GetString("psREGISTERED_TO_TXT", oCulture)
                'TODO - Check if Paintworks is registered
                'TODO - Can't make an interop for PWRegistration.dll - need to do this in .NET
                'If <PAINTworks is registered> then
                '     lblSiteName.Text = msSiteName
                'Else
                lblSiteName.Text = gpsRM.GetString("psUNREGISTERED_TXT", oCulture)
                'End if
            Else
                lblRegistered.Text = gpsRM.GetString("psSITE_NAME_TXT", oCulture)
                lblSiteName.Text = msSiteName
            End If
            lblWarning.Text = gpsRM.GetString("psCR_WARNING_TXT", oCulture)
            btnOK.Text = gpsRM.GetString("psOK_BTN_TXT", oCulture)
            btnSysInfo.Text = gpsRM.GetString("psSYS_INFO_BTN_TXT", oCulture)
            btnVersions.Text = gpsRM.GetString("psVERSIONS_BTN_TXT", oCulture)
            btnRegister.Text = gpsRM.GetString("psREGISTER_BTN_TXT", oCulture)
            lblAmerica.Text = gpsRM.GetString("psAMERICA", oCulture)

        Catch ex As Exception
            mDebug.WriteEventToLog("PW4_Main Module: frmAbout Routine: subLoadControlCaptions", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subLoadLanguageBox()
        '********************************************************************************************
        'Description:   Loads cboLanguage with a list of all supported languages.
        '
        'Parameters:    none
        'Returns:       none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Try
            Dim oCulture As CultureInfo = frmMain.DisplayCulture
            Dim nIndex As Integer

            cboLanguage.Items.Clear()

            For Each oCultureInfo As frmMain.udsCultureInfo In frmMain.gcolCultureInfo
                cboLanguage.Items.Add(gpsRM.GetString(oCultureInfo.CultureText, oCulture))
                If oCultureInfo.CultureName = oCulture.Name Then
                    nIndex = cboLanguage.Items.Count - 1
                    pnlFlag.BackgroundImage = CType(gpsRM.GetObject(oCultureInfo.FlagImage, oCulture), Image)
                End If
            Next

            cboLanguage.Text = cboLanguage.Items(nIndex).ToString

        Catch ex As Exception
            mDebug.WriteEventToLog("PW4_Main Module: frmAbout Routine: subLoadLanguageBox", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subRemoveMenuItem(ByVal hWnd As Int32, ByVal Item As String)
        '********************************************************************************************
        'Description:   Removes a menu item from the menu with the supplied handle
        '
        'Parameters:    Menu Handle, Item to be removed
        'Returns:       none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Try
            Dim nItemCount As Int32 = GetMenuItemCount(hWnd)

            For nItem As Integer = 0 To nItemCount - 1
                Dim sBuffer As String = Space(80)
                Dim nItemID As Int32 = GetMenuItemID(hWnd, nItem)
                Dim nReturnCode As Int32 = GetMenuString(hWnd, nItemID, sBuffer, Len(sBuffer), MF_BYCOMMAND)

                If sParseMenuItem(sBuffer).ToLower = Item.ToLower Then

                    nReturnCode = RemoveMenu(hWnd, nItemID, MF_BYCOMMAND)
                    If nReturnCode = 0 Then 'command failed
                        Call mDebug.WriteEventToLog("PW4_Main Module: frmAbout Routine: subRemoveMenuItem", _
                                                    "RemoveMenu Error: " & nReturnCode.ToString & _
                                                    " - Could not remove menu item [" & Item & "].")
                    End If 'lResponse = 0

                    Exit For

                End If 'sParseMenuItem(sBuffer).ToLower = Item.ToLower

            Next 'nItem

        Catch ex As Exception
            mDebug.WriteEventToLog("PW4_Main Module: frmAbout Routine: subRemoveMenuItem", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

#End Region

#Region " Event Handlers "

    Private Sub btnOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOK.Click
        '********************************************************************************************
        'Description: Closes this form
        '
        'Parameters:    none
        'Returns:       none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************

        Me.Close()

    End Sub

    Private Sub btnRegister_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRegister.Click
        '********************************************************************************************
        'Description:   Shows the PAINTworks registration form.
        '
        'Parameters:    none
        'Returns:       none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        'TODO - Add frmRegistration
        'frmRegistration.Show vbModeless

        Me.Close()

    End Sub

    Private Sub btnSysInfo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSysInfo.Click
        '********************************************************************************************
        'Description:   Shows the Microsoft System Information screen without some menu items that
        '               would allow any user to bypass our security system and run applications.
        '
        'Parameters:    none
        'Returns:       none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/11/12  MSW     Switch to the current MSInfo App.  It doesn't need all the menu mods
        '********************************************************************************************

        Try
            Dim hAppHandle As Int32 = mPWDesktop.FindWindow(BLANK, "Microsoft System Information")

            'Shell msinfo32.exe if it isn't already running
            If hAppHandle = 0 Then
                'Dim sAppPath As String = String.Empty

                'Call mPWCommon.GetDefaultFilePath(sAppPath, eDir.VB6Apps, _
                '                                  String.Empty, String.Empty)
                'Shell(sAppPath & "msinfo\msinfo32.exe", AppWinStyle.NormalFocus)

                Shell(Environment.ExpandEnvironmentVariables("%SystemRoot%\system32\msinfo32.exe"), AppWinStyle.NormalFocus)
                Application.DoEvents()

                ''get it's handle
                'Do While hAppHandle = 0
                '    hAppHandle = mPWDesktop.FindWindow(BLANK, "Microsoft System Information")
                '    If hAppHandle = 0 Then
                '        Application.DoEvents()
                '    End If
                'Loop

            Else
                Exit Sub
            End If

            ''Hide the toolbar and set the always on top menu selection
            'AppActivate("Microsoft System Information")
            'SendKeys.Send("%VT%VA")
            'Application.DoEvents()

            ''Remove Run item from the File menu. Also remove Toolbar and Always On Top
            ''items from the View menu.
            'Dim hScreenMenuHandle As Int32 = GetMenu(hAppHandle)
            'Dim nItemCount As Int32 = GetMenuItemCount(hScreenMenuHandle)

            'For nLoop As Int32 = 0 To nItemCount - 1
            '    Dim sBuffer As String = Space(80)
            '    Dim nResponse As Int32 = GetMenuString(hScreenMenuHandle, nLoop, sBuffer, _
            '                                           Strings.Len(sBuffer), MF_BYPOSITION)
            '    If nResponse = 0 Then 'command failed
            '        Call mDebug.WriteEventToLog("PW4_Main Module: frmAbout Routine: btnSysInfo_Click", _
            '                                    "GetMenuString Error: " & nResponse.ToString & _
            '                                    " - Could not retrieve menu string.")
            '    End If

            '    Dim sItem As String = sParseMenuItem(sBuffer)
            '    Dim hSubMenuHandle As Int32 = GetSubMenu(hScreenMenuHandle, nLoop)

            '    Select Case sItem.ToLower

            '        Case "file"
            '            Call subRemoveMenuItem(hSubMenuHandle, "Run...")

            '        Case "view"
            '            Call subRemoveMenuItem(hSubMenuHandle, "Toolbar")
            '            Call subRemoveMenuItem(hSubMenuHandle, "Always on Top")

            '    End Select

            'Next 'nLoop

            ''Remove minimize from the system menu
            'Dim hSysMenuHandle As Int32 = GetSystemMenu(hAppHandle, 0)
            'Dim nReturnCode As Int32 = RemoveMenu(hSysMenuHandle, SC_MINIMIZE, MF_BYCOMMAND)

            'If nReturnCode = 0 Then 'command failed
            '    Call mDebug.WriteEventToLog("PW4_Main Module: frmAbout Routine: btnSysInfo_Click", _
            '                    "RemoveMenu Error: " & nReturnCode.ToString & _
            '                    " - Could not remove menu item [Minimize].")
            'End If

        Catch ex As Exception
            mDebug.WriteEventToLog("PW4_Main Module: frmAbout Routine: btnSysInfo_Click", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub btnVersions_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVersions.Click
        '********************************************************************************************
        'Description:   Shows the PAINTworks source version utility screen.
        '
        'Parameters:    sender, eventargs
        'Returns:       none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim sAppFilePath As String = String.Empty

        Call mPWCommon.GetDefaultFilePath(sAppFilePath, eDir.VBApps, String.Empty, String.Empty)
        'Versions is currently a VB6 App so the ".NET" must be removed from "vbapps.NET" in sAppFilePath
        sAppFilePath = Strings.Left(sAppFilePath, Len(sAppFilePath)) & "\"
        Call mPWDesktop.StartPWApp(sAppFilePath & "versions.exe PW3_VERSIONS", 0&)

    End Sub

    Private Sub cboLanguage_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboLanguage.SelectedValueChanged
        '********************************************************************************************
        'Description:   A new display language has been selected
        '
        'Parameters:    sender, eventargs
        'Returns:       none
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' RJO       10/24/12        Removed code to set "CurrentLanguage" value in registry. No longer needed.
        '********************************************************************************************

        If Not mbLoading Then
            Dim sCurrentLanguage As String = "1"

            For Each oCultureInfo As frmMain.udsCultureInfo In frmMain.gcolCultureInfo

                If gpsRM.GetString(oCultureInfo.CultureText, frmMain.DisplayCulture) = cboLanguage.Text Then
                    frmMain.Culture = oCultureInfo.CultureName
                    sCurrentLanguage = oCultureInfo.CurrentLanguage
                    Application.DoEvents()
                    Exit For
                End If

            Next 'oCultureInfo

            'Legacy for PW3 (VB6) Apps 'RJO Removed 10/24/12
            'My.Computer.Registry.SetValue("HKEY_LOCAL_MACHINE\Software\FANUC\PAINTworks", "CurrentLanguage", _
            '                              sCurrentLanguage)

            'Redo the screen in the new language
            mbLoading = True
            Call subLoadLanguageBox()
            Call subLoadControlCaptions()
            mbLoading = False

        End If 'Not mbLoading

    End Sub

    Private Sub frmAbout_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '********************************************************************************************
        'Description:   Shuts down Microsoft System Information (if it is running) when form unloads
        '
        'Parameters:    sender, eventargs
        'Returns:       none
        '
        'Modification history:
        '
        ' By          Date          Reason
        '********************************************************************************************
        Dim hWnd As Int32 = mPWDesktop.FindWindow(BLANK, "Microsoft System Information")

        If hWnd <> 0 Then
            Call mPWDesktop.PostMessage(hWnd, WM_QUIT, 0, BLANK)
            Call mPWDesktop.DestroyWindow(hWnd)
        End If

    End Sub

    Private Sub frmAbout_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        '********************************************************************************************
        'Description: Trap Function key presses
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
		'    09/30/13   MSW     Save screenshots as jpegs
        '********************************************************************************************

        'Trap Function Key presses
        If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
            If e.KeyCode = Keys.F2 Then
                Dim oSC As New ScreenShot.ScreenCapture
                Dim sDumpPath As String = String.Empty

                mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                oSC.CaptureWindowToFile(Me.Handle, sDumpPath & msSCREEN_DUMP_NAME, Imaging.ImageFormat.Jpeg)
            End If
        End If

    End Sub

    Private Sub frmAbout_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description: Positions and shows this form
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Dim objLocation As Drawing.Point

        objLocation.X = (frmMain.grecWorkingArea.Width - Me.Width) \ 2
        objLocation.Y = ((frmMain.grecWorkingArea.Height - Me.Height) \ 2) + frmMain.grecWorkingArea.Top

        Me.Location = objLocation

        btnRegister.Enabled = frmMain.gbREGISTRATION_ENABLED

        mbLoading = True
        Call subLoadLanguageBox()
        Call subLoadControlCaptions()
        mbLoading = False

    End Sub

#End Region


End Class