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
' Form/Module: PAINTworks IV SubMenu Form (frmMenu)
'
' Dependencies: 
'
' Language: Microsoft Visual Basic .NET 2005
'
' Description: This form provides properties and methods for displaying PAINTworks IV
'              type menus.
'
' Author: R.Olejniczak
' FANUC Robotics America
' 3900 W. Hamlin Road
' Rochester Hills, MI.  48309-3253
'
' Modification history:
'
' By          Date              Reason
' MSW         08/18/10          Process #ZONE# tag in program name in XML to manage zone specific .EXEs 
'                               in identical vbapps folders
' RJO         06/15/12          Changed grpbStatus FlatStyle property from "Standard" to "System". This
'                               action is said to prevent an unhandled exception that occurs on some
'                               GUI computers when a Paintworks Main Menu button is clicked while the
'                               Alarm Manager Active Alarms screen is showing.
' MSW         09/13/13          Add usb devices link using DoButtonPressFunction        
'*************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Public Class frmMenu
    'TODO - Need to figure out how to draw a border around the menu window
    'TODO - Adjust menu widths in XML file so the entire status text is visible
#Region " Declares "
    Inherits System.Windows.Forms.Form

    'Valid .LaunchFlags values:
    'Used when the application is managed by Paintworks (.UseFRWM = True)
    'FW_EXCLUSIVE_MODE = 64  'No other App can run while this one is running
    'FW_MULTI_START = 256    'Multiple instances of this App can run simultaneously

    'Valid .LaunchFlags values:
    'Used when the application is not managed by Paintworks (.UseFRWM = False)
    '0 - AppWinStyle.Hide 
    '1 - AppWinStyle.NormalFocus
    '2 - AppWinStyle.MinimizedFocus
    '3 - AppWinStyle.MaximizedFocus
    '4 - AppWinStyle.NormalNoFocus
    '6 - AppWinStyle.MinimizedNoFocus

    'Valid .Action values
    '"ButtonPress"     - Do some action in PW4_Main
    '"LaunchFile"      - Launch an EXE
    '"ShutdownRequest" - Shutdown Paintworks

    Private Const msMODULE As String = "frmMenu"

    Private Structure udsMenuConfig
        Public Columns As Integer
        Public ButtonHeight As Integer
        Public ButtonWidth As Integer
        Public MinWidth As Integer
        Public TopMargin As Integer
        Public BottomMargin As Integer
        Public SideMargin As Integer
        Public HorizontalSpacing As Integer
        Public VerticalSpacing As Integer
        Public TitleTag As String
        Public CaptionTag As String
        Public CancelCaptionTag As String
        Public CancelToolTipTag As String
    End Structure

    Friend Structure udsButtonConfig
        Public CaptionTag As String
        Public DescriptionTag As String
        Public ToolTipTag As String
        Public ImageTag As String
        Public Visible As Boolean
        Public Action As String
        Public LaunchFile As String
        Public IsVB6App As Boolean
        Public UseAppPath As Boolean
        Public UseFRWM As Boolean
        Public UsePassword As Boolean
        Public LaunchFlags As Integer
        Public WindowTitle As String
        Public CommandLine As String
        Public Name As String
    End Structure

    Private mButtonConfigs As New Collection
    Private mCurrentCulture As Globalization.CultureInfo
    Private mMenuConfig As udsMenuConfig
    Private mMenuLocation As System.Drawing.Point
    Private msAppPath As String = String.Empty
    Private msVB6AppPath As String = String.Empty
    Private msConfigFilePath As String = String.Empty
    Private msMenuName As String = String.Empty
    Private msSCREEN_DUMP_NAME As String = "Menus_"
    Private msSCREEN_DUMP_EXT As String = ".jpg"
#End Region

#Region " Properties "

    Friend WriteOnly Property AppFilePath() As String
        '********************************************************************************************
        'Description: Allows frmMain to specify the Path to the VB.NET EXE files. 
        '
        'Parameters: AppFilePath
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As String)
            msAppPath = value
        End Set

    End Property

    Friend WriteOnly Property MenuLocation() As System.Drawing.Point
        '********************************************************************************************
        'Description: Allows frmMain to set the Display Location for the SubMenu.
        '
        'Parameters: MenuLocation -  The X,Y location of the upper left corner of the SubMenu.
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '******************************************************************************************** 

        Set(ByVal value As System.Drawing.Point)
            mMenuLocation = value
        End Set

    End Property

    Friend WriteOnly Property MenuConfigFilePath() As String
        '********************************************************************************************
        'Description: Allows frmMain to specify the Path the SubMenu configuration file. 
        '
        'Parameters: MenuConfigFilePath -  The path to XML configuration file for this SubMenu.
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As String)
            msConfigFilePath = value
        End Set

    End Property

    Friend WriteOnly Property MenuName() As String
        '********************************************************************************************
        'Description: Allows frmMain to specify the Name of the SubMenu to be shown.
        '
        'Parameters: MenuName -  The name of the button that launched this SubMenu.
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As String)
            msMenuName = value
        End Set

    End Property

    Friend WriteOnly Property VB6AppFilePath() As String
        '********************************************************************************************
        'Description: Allows frmMain to specify the Path to the VB6 EXE files. 
        '
        'Parameters: VB6AppFilePath
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Set(ByVal value As String)
            msVB6AppPath = value
        End Set

    End Property

#End Region

    Private Function ButtonInfoValid(ByVal ButtonConfig As udsButtonConfig) As Boolean
        '********************************************************************************************
        'Description:   This looks at he menu button properties and checks (if the button launches an
        '               application) to make sure the EXE (Launchfile) exists in the specified folder.
        '
        'Parameters:    none
        'Returns:       True if Launchfile is OK
        '
        'Modification history:
        '
        ' By          Date          Reason
        ' RJO       09/16/2011      Where the launchfile is can depend on .UseAppPath and .IsVB6App
        '********************************************************************************************
        Try
            ButtonInfoValid = True

            With ButtonConfig
                If .Action.ToLower = "launchfile" Then
                    Dim sLaunchString As String
                    If .UseAppPath Then
                        sLaunchString = GetExeName(.LaunchFile)
                        If .IsVB6App Then
                            sLaunchString = msVB6AppPath & sLaunchString
                        Else
                            sLaunchString = msAppPath & sLaunchString
                        End If
                    Else
                        sLaunchString = .LaunchFile
                    End If
                    ButtonInfoValid = System.IO.File.Exists(sLaunchString)
                End If
            End With
            'TODO - might need to check other conditions
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.gsSCREEN_NAME & " Module: " & msMODULE & " Routine: ButtonInfoValid", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Function



    Private Function GetExeName(ByVal LaunchString As String) As String
        '********************************************************************************************
        'Description: Returns the exe filename in the LaunchString.
        '
        'Parameters: none
        'Returns:    exe filename - ex. "calc.exe"
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            If Strings.Len(LaunchString) > 6 Then
                Dim sTemp As String = String.Empty

                For nChars As Integer = 6 To Strings.Len(LaunchString)
                    sTemp = Strings.Right(LaunchString, nChars)

                    If Strings.Left(sTemp, 1) = "\" Then
                        sTemp = Strings.Right(sTemp, nChars - 1)
                        Exit For
                    End If
                Next
                Return sTemp
            Else
                Return String.Empty
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.gsSCREEN_NAME & " Module: " & msMODULE & " Routine: GetExeName", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
            Return String.Empty
        End Try
    End Function

    Private Function GetMenuConfig() As Boolean
        '********************************************************************************************
        'Description: Read menu and button configuration data from MenuData.XML.
        '
        'Parameters: none
        'Returns:    True if Success
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/18/10  MSW     Process #ZONE# tag in program name in XML to manage zone specific .EXEs 
        '                   in identical vbapps folders
        '********************************************************************************************
        Dim dsMenus As New DataSet
        Dim drMenu As DataRow = Nothing
        Dim bExists As Boolean
        Dim bSuccess As Boolean
        Dim sTmp As String = String.Empty

        Try
            dsMenus.ReadXmlSchema(msConfigFilePath & "MenuData.xsd")
            dsMenus.ReadXml(msConfigFilePath & "MenuData.xml")

            'Find the Menu configuration for msMenuName
            For Each drMenu In dsMenus.Tables("MenuData").Rows
                If drMenu.Item("MainMenuButtonName").ToString = msMenuName Then

                    With mMenuConfig
                        .Columns = CType(drMenu.Item("Columns"), Integer)
                        .ButtonHeight = CType(drMenu.Item("ButtonHeight"), Integer)
                        .ButtonWidth = CType(drMenu.Item("ButtonWidth"), Integer)
                        .MinWidth = CType(drMenu.Item("MinWidth"), Integer)
                        .TopMargin = CType(drMenu.Item("TopMargin"), Integer)
                        .BottomMargin = CType(drMenu.Item("BottomMargin"), Integer)
                        .SideMargin = CType(drMenu.Item("SideMargin"), Integer)
                        .HorizontalSpacing = CType(drMenu.Item("HorizontalSpacing"), Integer)
                        .VerticalSpacing = CType(drMenu.Item("VerticalSpacing"), Integer)
                        .TitleTag = drMenu.Item("TitleTag").ToString
                        .CaptionTag = drMenu.Item("CaptionTag").ToString
                        .CancelCaptionTag = drMenu.Item("CancelCaptionTag").ToString
                        .CancelToolTipTag = drMenu.Item("CancelToolTipTag").ToString
                    End With 'mMenuConfig

                    bExists = True
                    Exit For

                End If
            Next 'drMenu

            If bExists Then
                Dim relButtons As DataRelation = dsMenus.Tables("MenuData").ChildRelations(0)
                Dim drButtons() As DataRow = drMenu.GetChildRows(relButtons)

                If drButtons.GetUpperBound(0) > 0 Then
                    'Get the configuration for each button
                    Dim nIndex As Integer

                    For Each drButton As DataRow In drButtons
                        Dim ButtonConfig As New udsButtonConfig

                        nIndex += 1
                        With ButtonConfig
                            .CaptionTag = drButton.Item("CaptionTag").ToString
                            .DescriptionTag = drButton.Item("DescriptionTag").ToString
                            .ToolTipTag = drButton.Item("ToolTipTag").ToString
                            .ImageTag = drButton.Item("ImageTag").ToString
                            sTmp = drButton.Item("Visible").ToString
                            If sTmp.ToLower = "true" Then .Visible = True
                            .Action = drButton.Item("Action").ToString
                            .LaunchFile = Environment.ExpandEnvironmentVariables(drButton.Item("LaunchFile").ToString).Replace("#ZONE#", frmMain.mcolZones.CurrentZone)
                            sTmp = drButton.Item("IsVB6App").ToString
                            If sTmp.ToLower = "true" Then .IsVB6App = True
                            sTmp = drButton.Item("UseAppPath").ToString
                            If sTmp.ToLower = "true" Then .UseAppPath = True
                            sTmp = drButton.Item("UseFRWM").ToString
                            If sTmp.ToLower = "true" Then .UseFRWM = True
                            sTmp = drButton.Item("UsePassword").ToString
                            If sTmp.ToLower = "true" Then .UsePassword = True
                            .LaunchFlags = CType(drButton.Item("LaunchFlags"), Integer)
                            .WindowTitle = drButton.Item("WindowTitle").ToString
                            .CommandLine = drButton.Item("CommandLine").ToString
                            .Name = "Button" & Strings.Format(nIndex, "00")
                        End With 'ButtonConfig


                        If ButtonConfig.Visible Then
                            If ButtonInfoValid(ButtonConfig) Then
                                mButtonConfigs.Add(ButtonConfig, ButtonConfig.Name)
                            Else
                                Dim sMsg As String = msMenuName & " "

                                sMsg = sMsg & ButtonConfig.Name & " configuration invalid."
                                mDebug.WriteEventToLog("PW4_Main Module: frmMenu Routine: GetMenuConfig", _
                                                       "Error: " & sMsg)
                                If nIndex > 0 Then nIndex -= 1
                            End If
                        End If 'ButtonConfig.Visible

                    Next 'drButton

                Else
                    lblEmptyMenu.Visible = True
                End If 'drButtons.GetUpperBound(0) > 0

            bSuccess = True

            End If 'bExists

        Catch ex As Exception
            mDebug.WriteEventToLog("PW4_Main Module: frmMenu Routine: GetMenuConfig", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

        Return bSuccess

    End Function

    Private Sub subBuildMenu()
        '********************************************************************************************
        'Description: Add and position the buttons on this SubMenu.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            For Each ButtonConfig As udsButtonConfig In mButtonConfigs
                'Create the button
                Dim objButton As New Button

                With objButton
                    .Name = ButtonConfig.Name
                    '.Text = String.Empty
                    .Height = mMenuConfig.ButtonHeight
                    .Width = mMenuConfig.ButtonWidth
                    .Image = DirectCast(gasRM.GetObject(ButtonConfig.ImageTag, mCurrentCulture), Image)
                End With
                Me.Controls.Add(objButton)
                AddHandler objButton.Click, AddressOf subButton_Click
                AddHandler objButton.MouseHover, AddressOf subButton_MouseHover
                AddHandler objButton.MouseLeave, AddressOf subButton_MouseLeave
                ToolTipMenu.SetToolTip(objButton, gasRM.GetString(ButtonConfig.ToolTipTag, mCurrentCulture))

                'Create the Description Label
                Dim objLabel As New Label

                With objLabel
                    .Name = "lbl" & ButtonConfig.Name
                    .Font = lblCaption.Font
                    .Text = gasRM.GetString(ButtonConfig.DescriptionTag, mCurrentCulture)
                    .TextAlign = ContentAlignment.MiddleCenter
                End With
                Me.Controls.Add(objLabel)

                'Position the button on the form
                Dim nIndex As Integer = CType(Strings.Right(ButtonConfig.Name, 2), Integer)
                Dim nRow As Integer = (nIndex - 1) \ mMenuConfig.Columns
                Dim nCol As Integer = (nIndex - 1) Mod mMenuConfig.Columns

                'Calculate X
                objButton.Left = mMenuConfig.SideMargin + (nCol * (mMenuConfig.ButtonWidth + _
                                 mMenuConfig.HorizontalSpacing))
                'Calculate Y
                objButton.Top = mMenuConfig.TopMargin + (nRow * (mMenuConfig.ButtonHeight + _
                                mMenuConfig.VerticalSpacing))

                objLabel.Left = (objButton.Width - objLabel.Width) \ 2 + objButton.Left
                objLabel.Top = objButton.Height + 4 + objButton.Top

            Next 'ButtonConfig"
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subBuildMenu", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

        End Try
    End Sub

    Private Sub subExecuteSubMenuCmd(ByVal ButtonConfig As udsButtonConfig)
        '********************************************************************************************
        'Description:  A menu button has been clicked on one of the sub menus. Execute the command 
        '              associated with this button.
        '
        'Parameters: None
        'Returns:    None
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/13/13  MSW     Add usb devices link using DoButtonPressFunction 
        '********************************************************************************************

        Try
            'TODO - see if this makes things work better
            'Me.Hide()

            With ButtonConfig

                Select Case .Action.ToLower

                    Case "buttonpress"
                        'Request to launch a PW4_Main embedded function
                        Call frmMain.DoButtonPressFunction(.CommandLine, gasRM.GetString(ButtonConfig.DescriptionTag, mCurrentCulture))

                    Case "launchfile"
                        'Launch an EXE file
                        Dim sLaunchString As String = String.Empty

                        If .UseAppPath Then
                            sLaunchString = GetExeName(.LaunchFile)
                            If .IsVB6App Then
                                sLaunchString = msVB6AppPath & sLaunchString
                            Else
                                sLaunchString = msAppPath & sLaunchString
                            End If
                        Else
                            sLaunchString = .LaunchFile
                        End If

                        Call frmMain.LaunchPWApp(sLaunchString, .UseFRWM, .LaunchFlags, .CommandLine, .IsVB6App, .UsePassword)

                    Case "shutdownrequest"
                        'Request to shutdown Paintworks
                        Call frmMain.ShutDownPaintworks()

                    Case Else
                        mDebug.WriteEventToLog("PW4_Main Module: frmMenu Routine: subExecuteSubMenuCmd", _
                                               "Unrecognized Action attached to " _
                                               & .Name & " button. Action = [" & .Action & "]")

                End Select

            End With 'ButtonConfig

        Catch ex As Exception
            mDebug.WriteEventToLog("PW4_Main Module: frmMenu Routine: subExecuteSubMenuCmd", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        Finally
            Me.Close()
        End Try

    End Sub

    Private Sub subFormatMenuPanel()
        '********************************************************************************************
        'Description: Put the text for the current culture in the SubMenu form items, then size the 
        '             form for the number of menu buttons.
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim nButtons As Integer
            Dim nColumns As Integer
            Dim nRows As Integer

            'Only display this if it turns out that there are no buttons available to this
            'user for this menu.
            lblEmptyMenu.Visible = False

            'Set the Menu Title and Caption
            lblTitle.Text = gasRM.GetString(mMenuConfig.TitleTag, mCurrentCulture)
            lblCaption.Text = gasRM.GetString(mMenuConfig.CaptionTag, mCurrentCulture)

            'Set the Cancel Button Text and ToolTip
            btnCancel.Text = gasRM.GetString(mMenuConfig.CancelCaptionTag, mCurrentCulture)
            ToolTipMenu.SetToolTip(btnCancel, gasRM.GetString(mMenuConfig.CancelToolTipTag, mCurrentCulture))

            'Resize the panel
            nColumns = mMenuConfig.Columns
            nButtons = mButtonConfigs.Count

            'Set panel width
            If nButtons < nColumns Then
                nColumns = nButtons
                mMenuConfig.Columns = nColumns
            End If

            Me.Width = (mMenuConfig.SideMargin * 2) + (mMenuConfig.ButtonWidth * nColumns) + _
                       (mMenuConfig.HorizontalSpacing * (nColumns - 1))

            If Me.Width < mMenuConfig.MinWidth Then
                Me.Width = mMenuConfig.MinWidth
            End If

            'Set panel height
            If nColumns = 0 Then
                nRows = 1
            Else
                nRows = nButtons \ nColumns
                If nButtons Mod nColumns > 0 Then
                    nRows += 1
                End If
            End If

            Me.Height = mMenuConfig.TopMargin + mMenuConfig.BottomMargin + _
                        (mMenuConfig.ButtonHeight * nRows) + _
                        (mMenuConfig.VerticalSpacing * (nRows - 1))
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subFormatMenuPanel", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

        End Try
    End Sub

    Private Sub frmMenu_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
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
        Try

            'Trap Function Key presses
            If (Not e.Alt) And (Not e.Control) And (Not e.Shift) Then
                Select Case e.KeyCode
                    Case Keys.F1
                        'Help Screen request
                        Select Case msMenuName
                            Case "btnConfig"
                                subLaunchHelp(gs_HELP_MENU_CONFIG, frmMain.oIPC)
                            Case "btnProcess"
                                subLaunchHelp(gs_HELP_MENU_PROCESS, frmMain.oIPC)
                            Case "btnView"
                                subLaunchHelp(gs_HELP_MENU_VIEW, frmMain.oIPC)
                            Case "btnOperate"
                                subLaunchHelp(gs_HELP_MENU_OPERATE, frmMain.oIPC)
                            Case "btnReports"
                                subLaunchHelp(gs_HELP_MENU_REPORTS, frmMain.oIPC)
                            Case "btnUtilities"
                                subLaunchHelp(gs_HELP_MENU_UTILITIES, frmMain.oIPC)
                            Case "btnMaint"
                                subLaunchHelp(gs_HELP_MENU_MAINT, frmMain.oIPC)
                            Case "btnHelp"
                                subLaunchHelp(gs_HELP_MENU_HELP, frmMain.oIPC)
                            Case Else
                                subLaunchHelp(gs_HELP_MAIN, frmMain.oIPC)
                        End Select
                        'Friend Const gs_HELP_MAIN As String = ""
                        'Friend Const gs_HELP_MENU_CONFIG As String = "[1.0]PAINTWorksMainMenu\ConfigurationMenu.htm"
                        'Friend Const gs_HELP_MENU_PROCESS As String = "[1.0]PAINTWorksMainMenu\ProcessMenu.htm"
                        'Friend Const gs_HELP_MENU_VIEW As String = "[1.0]PAINTWorksMainMenu\ViewMenu.htm"
                        'Friend Const gs_HELP_MENU_OPERATE As String = "[1.0]PAINTWorksMainMenu\OperateMenu.htm"
                        'Friend Const gs_HELP_MENU_REPORTS As String = "[1.0]PAINTWorksMainMenu\ReportsMenu.htm"
                        'Friend Const gs_HELP_MENU_UTILITIES As String = "[1.0]PAINTWorksMainMenu\UtilitiesMenu.htm"
                        'Friend Const gs_HELP_MENU_MAINT As String = "[1.0]PAINTWorksMainMenu\MaintenanceMenuMenu.htm"
                        'Friend Const gs_HELP_MENU_HELP As String = "[1.0]PAINTWorksMainMenu\HelpMenu.htm"

                    Case Keys.F2
                        'Screen Dump request
                        Dim oSC As New ScreenShot.ScreenCapture
                        Dim sDumpPath As String = String.Empty

                        mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                        oSC.CaptureWindowToFile(Me.Handle, sDumpPath & msSCREEN_DUMP_NAME & _
                                                gasRM.GetString(mMenuConfig.TitleTag, mCurrentCulture) & _
                                                msSCREEN_DUMP_EXT, Imaging.ImageFormat.Jpeg)
                    Case Else

                End Select
            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.gsSCREEN_NAME & " Module: " & msMODULE & " Routine: frmMenu_KeyDown", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

        End Try
    End Sub

    Private Sub frmMenu_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description: The SubMenu form has just been loaded. Configure the SubMenu and Buttons.
        '             
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            'TODO - Make sure we have everthing we need to configure the menu. Error out if not.
            mCurrentCulture = frmMain.DisplayCulture

            If GetMenuConfig() Then
                Call subFormatMenuPanel()
                Call subBuildMenu()
            Else

            End If
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.gsSCREEN_NAME & " Module: " & msMODULE & " Routine: frmMenu_Load", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)

        End Try
    End Sub

    Private Sub frmMenu_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        '********************************************************************************************
        'Description: The SubMenu form has just been shown. Move it to the proper location.
        '             
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Me.Location = mMenuLocation
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.gsSCREEN_NAME & " Module: " & msMODULE & " Routine: frmMenu_Shown", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub subButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        '********************************************************************************************
        'Description: Handles all Button Click events on this form.
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim objButton As Button = DirectCast(sender, Button)

            Select Case objButton.Name

                Case "btnCancel"
                    Me.Close()

                Case Else
                    'A menu button has been clicked
                    Dim ButtonConfig As udsButtonConfig = DirectCast(mButtonConfigs.Item(objButton.Name), udsButtonConfig)
                    Me.Hide()
                    Call subExecuteSubMenuCmd(ButtonConfig)
                    Me.Close()

            End Select
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subButton_Click", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try

    End Sub

    Private Sub subButton_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.MouseHover
        '********************************************************************************************
        'Description: The MousePointer is hovering above one of the SubMenu Buttons. Show the
        '             CaptionTag text in the SubMenu Status Bar.
        '             
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            Dim objButton As Button = DirectCast(sender, Button)

            Select Case objButton.Name

                Case "btnCancel"
                    lblCaption.Text = gasRM.GetString(mMenuConfig.CaptionTag, mCurrentCulture)

                Case Else
                    'The MousePointer is hovering above a menu button
                    Dim ButtonConfig As udsButtonConfig = DirectCast(mButtonConfigs.Item(objButton.Name), udsButtonConfig)

                    lblCaption.Text = gasRM.GetString(ButtonConfig.CaptionTag, mCurrentCulture)

            End Select
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subButton_MouseHover", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub subButton_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.MouseLeave
        '********************************************************************************************
        'Description: The MousePointer has just left one of the SubMenu Buttons. Show the Menu
        '             CaptionTag text in the SubMenu Status Bar.
        '             
        '
        'Parameters: sender, eventargs
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Try
            lblCaption.Text = gasRM.GetString(mMenuConfig.CaptionTag, mCurrentCulture)
        Catch ex As Exception
            mDebug.WriteEventToLog(frmMain.gsSCREEN_NAME & " Module: " & msMODULE & " Routine: subButton_MouseLeave", _
                                   "Error: " & ex.Message & vbCrLf & "StackTrace: " & ex.StackTrace)
        End Try
    End Sub

End Class