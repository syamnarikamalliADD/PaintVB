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
' Form/Module: mScreenSetup
'
' Description: Routines for setting up screens, loading dropdowns etc.
' 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: FANUC Programmer
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                      Version
'    02/14/07   gks     Add ContainerName parameter  to loadtextboxcollection                                                    
'    03/17/07   gks     Onceover Cleanup
'    08/15/07   gks     Typed collections of focusedtextboxes
'    10/29/09  	MSW	    update print dropdown menu labeling
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'    10/16/12   MSW     subDoStatusStrip - be more flexible about cross-thread calls  4.01.01.01
'    11/15/12   HGB     Added Sub LoadComboBoxCollection                              4.01.01.02
'    04/02/13   DE      Modified subSetUpToolStrip to localize Multi-View buttons     4.01.01.03
'    08/21/13   MSW     SetUpStatusStrip - hide the sizing grip for the status bar    4.01.05.00
'                       so the spacing works right
'    08/30/13   MSW     Add CloneRadioButton for manual cycle screen                  4.01.05.01
'    09/13/13   MSW     SetUpStatusStrip, SetResizeStatusStrip - get the startusbar   4.01.05.02
'                       to resize properly
'    04/03/14   MSW     Consolidate some more button setup                            4.01.07.00
'********************************************************************************************
Option Compare Text
Option Explicit On
Option Strict On

Imports System.Collections.Generic
Imports System.Collections.ObjectModel

Friend Module mScreenSetup

#Region " Declares "

    'height of the pwmain buttonbar
    Friend Const gnPW_MAINBANNERHEIGHT As Integer = 68
    Friend Const gnPW_STATUSBARHEIGHT As Integer = 0 ' this is for future 68
    'Render Me
    Friend Const gnRENDERMODE As ToolStripRenderMode = ToolStripRenderMode.ManagerRenderMode
    Private mrecB As New _
            System.Drawing.Rectangle(0, gnPW_MAINBANNERHEIGHT, 800, (600 - gnPW_MAINBANNERHEIGHT))

    'this is some hokeyness for statusstrip sizing and hopefully will be replaced
    Friend Structure tContSize
        Dim ProgBarVisibleSize As Integer
        Dim ProgBarInvisSize As Integer
        Dim SpaceLabelVisbleSize As Integer
        Dim SpaceLabelInvisSize As Integer
    End Structure
    Friend gtSSSize As tContSize

    ' enums for dropdown menus
    Friend Enum ePrintMnu
        Print = 0
        SelPrinter = 1
    End Enum
    Friend Enum eChangeMnu
        Hours24 = 0
        Days7 = 1
        All = 2
    End Enum
    Friend Enum eRestoreMnu
        FromDB = 0
        FromPLC = 1
    End Enum
    Friend Enum eUtilitiesMnu
        Export_CSV = 0
        Import_CSV = 1
    End Enum

#End Region
#Region " Properties "

    Friend Property MaxBound() As Drawing.Rectangle
        '********************************************************************************************
        'Description:  This property sets the size of the form when maximize button is clicked
        '
        'Parameters: Rect
        'Returns:    Rect
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            MaxBound = mrecB
        End Get
        Set(ByVal Value As Drawing.Rectangle)
            mrecB = Value
        End Set
    End Property

#End Region
#Region " Routines "

    Friend Sub FinalizeToolStrip(ByRef oTS As ToolStrip)
        '********************************************************************************************
        'Description:  Call this after loading pics and text to make buttons more uniform etc
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        Const MINBUTTONWIDTH As Integer = 75

        For Each o As ToolStripItem In oTS.Items
            If TypeOf o Is ToolStripButton Then
                Dim b As ToolStripButton = DirectCast(o, ToolStripButton)

                'maintain minimum width
                Dim sz As Size = b.Size
                If sz.Width < MINBUTTONWIDTH Then
                    b.AutoSize = False
                    sz.Width = MINBUTTONWIDTH
                    b.Size = sz
                End If
                'font me
                b.Font = New System.Drawing.Font("Arial", 10, FontStyle.Regular)
                b.Alignment = ToolStripItemAlignment.Left
                b.DoubleClickEnabled = False
                b.ImageAlign = ContentAlignment.TopCenter
                b.TextAlign = ContentAlignment.BottomCenter
            End If

            If TypeOf o Is ToolStripSplitButton Then
                Dim b As ToolStripSplitButton = DirectCast(o, ToolStripSplitButton)
                'maintain minimum width
                Dim sz As Size = b.Size
                If sz.Width < MINBUTTONWIDTH Then
                    b.AutoSize = False
                    sz.Width = MINBUTTONWIDTH
                    b.Size = sz
                End If
                'font me
                b.Font = New System.Drawing.Font("Arial", 10, FontStyle.Regular)
                b.Alignment = ToolStripItemAlignment.Left
                b.DoubleClickEnabled = False
                b.ImageAlign = ContentAlignment.TopCenter
                b.TextAlign = ContentAlignment.BottomCenter
            End If

            If TypeOf o Is ToolStripDropDownButton Then
                Dim b As ToolStripDropDownButton = DirectCast(o, ToolStripDropDownButton)
                'maintain minimum width
                Dim sz As Size = b.Size
                If sz.Width < MINBUTTONWIDTH Then
                    b.AutoSize = False
                    sz.Width = MINBUTTONWIDTH
                    b.Size = sz
                End If
                'font me
                b.Font = New System.Drawing.Font("Arial", 10, FontStyle.Regular)
                b.Alignment = ToolStripItemAlignment.Left
                b.DoubleClickEnabled = False
                b.ImageAlign = ContentAlignment.TopCenter
                b.TextAlign = ContentAlignment.BottomCenter
            End If
        Next
    End Sub
    Friend Function CloneLabel(ByVal ExistingLabel As Label, ByVal NewLabelName As String) As Label
        '********************************************************************************************
        'Description:  Make a label just like the other one
        '
        'Parameters: label to clone, and name for the new one
        'Returns:    label - you do the text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        Dim l As New Label

        With l
            .Name = NewLabelName
            .Font = ExistingLabel.Font
            .AutoSize = ExistingLabel.AutoSize
            .BackColor = ExistingLabel.BackColor
            .BorderStyle = ExistingLabel.BorderStyle
            .FlatStyle = ExistingLabel.FlatStyle
            .ForeColor = ExistingLabel.ForeColor
            .Height = ExistingLabel.Height
            .Size = ExistingLabel.Size
            .TextAlign = ExistingLabel.TextAlign
            .Width = ExistingLabel.Width
            .Text = String.Empty
            'anchor property not set here - it messes with the autoscroll in the page layout
            'existing is probably disabled at this point, but if you are cloning it, you probably
            'want it enabled
            .Enabled = True 'ExistingLabel.Enabled
            'existing is probably invisible at this point, but if you are cloning it, you probably
            'want to see it
            .Visible = True 'ExistingLabel.Visible
        End With

        Return l

    End Function

    Friend Function CloneRadioButton(ByVal ExistingRadioButton As RadioButton, ByVal NewRadioButtonName As String) As RadioButton
        '********************************************************************************************
        'Description:  Make a label just like the other one
        '
        'Parameters: label to clone, and name for the new one
        'Returns:    label - you do the text
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/30/13  MSW     Add CloneRadioButton for manual cycle screen
        '********************************************************************************************    
        Dim l As New RadioButton

        With l
            .Name = NewRadioButtonName
            .Font = ExistingRadioButton.Font
            .AutoSize = ExistingRadioButton.AutoSize
            .BackColor = ExistingRadioButton.BackColor
            .FlatStyle = ExistingRadioButton.FlatStyle
            .ForeColor = ExistingRadioButton.ForeColor
            .Height = ExistingRadioButton.Height
            .Size = ExistingRadioButton.Size
            .TextAlign = ExistingRadioButton.TextAlign
            .Width = ExistingRadioButton.Width
            .Text = String.Empty
            'anchor property not set here - it messes with the autoscroll in the page layout
            'existing is probably disabled at this point, but if you are cloning it, you probably
            'want it enabled
            .Enabled = True 'ExistingLabel.Enabled
            'existing is probably invisible at this point, but if you are cloning it, you probably
            'want to see it
            .Visible = True 'ExistingLabel.Visible
        End With

        Return l

    End Function
    Friend Function CloneFocusedTextBox(ByVal ExistingFTextBox As FocusedTextBox.FocusedTextBox, _
                                        ByVal NewFTextBoxName As String) As FocusedTextBox.FocusedTextBox
        '********************************************************************************************
        'Description:  Make a FocusedTextBox just like the other one
        '
        'Parameters: FocusedTextBox to clone, and name for the new one
        'Returns:    FocusedTextBox - you do the text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        Dim f As New FocusedTextBox.FocusedTextBox

        With f
            .Name = NewFTextBoxName
            .Font = ExistingFTextBox.Font
            .BackColor = ExistingFTextBox.BackColor
            .BorderStyle = ExistingFTextBox.BorderStyle
            .ForeColor = ExistingFTextBox.ForeColor
            .Height = ExistingFTextBox.Height
            .MaxLength = ExistingFTextBox.MaxLength
            .Multiline = ExistingFTextBox.Multiline
            .NumericOnly = ExistingFTextBox.NumericOnly
            .ReadOnly = ExistingFTextBox.ReadOnly
            .ScrollBars = ExistingFTextBox.ScrollBars
            .Size = ExistingFTextBox.Size
            .TabStop = ExistingFTextBox.TabStop
            .TextAlign = ExistingFTextBox.TextAlign
            .Width = ExistingFTextBox.Width
            .WordWrap = ExistingFTextBox.WordWrap
            .Text = String.Empty
            'anchor property not set here - it messes with the autoscroll in the page layout
            '.Anchor = ExistingFTextBox.Anchor
            'existing is probably invisible at this point, but if you are cloning it, you probably
            'want to see it
            .Visible = True 'ExistingFTextBox.Visible
            'existing is probably disabled at this point, but enable here anyway
            .Enabled = True 'ExistingFTextBox.Enabled
        End With

        Return f

    End Function
    Friend Function CloneCheckBox(ByVal ExistingCheckBox As CheckBox, _
                                    ByVal NewCheckBoxName As String) As CheckBox
        '********************************************************************************************
        'Description:  Make a FocusedTextBox just like the other one
        '
        'Parameters: FocusedTextBox to clone, and name for the new one
        'Returns:    FocusedTextBox - you do the text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        Dim f As New CheckBox

        With f
            .Name = NewCheckBoxName
            .Font = ExistingCheckBox.Font
            .BackColor = ExistingCheckBox.BackColor
            .ForeColor = ExistingCheckBox.ForeColor
            .Height = ExistingCheckBox.Height
            .Size = ExistingCheckBox.Size
            .TabStop = ExistingCheckBox.TabStop
            .Width = ExistingCheckBox.Width
            .Text = String.Empty
            .Checked = ExistingCheckBox.Checked
            'anchor property not set here - it messes with the autoscroll in the page layout
            '.Anchor = ExistingFTextBox.Anchor
            'existing is probably invisible at this point, but if you are cloning it, you probably
            'want to see it
            .Visible = True 'ExistingFTextBox.Visible
            'existing is probably disabled at this point, but enable here anyway
            .Enabled = True 'ExistingFTextBox.Enabled
        End With

        Return f

    End Function

    Friend Function ClonePictureBox(ByVal ExistingPictureBox As PictureBox, _
                                    ByVal NewPictureBoxName As String) As PictureBox
        '********************************************************************************************
        'Description:  Make a PictureBox just like the other one
        '
        'Parameters: PictureBox to clone, and name for the new one
        'Returns:    PictureBox - you do the image
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        Dim picTemp As New PictureBox

        With picTemp
            .Name = NewPictureBoxName
            .BackColor = ExistingPictureBox.BackColor
            .BorderStyle = ExistingPictureBox.BorderStyle
            .ForeColor = ExistingPictureBox.ForeColor
            .Size = ExistingPictureBox.Size
            .Image = ExistingPictureBox.Image
            .SizeMode = ExistingPictureBox.SizeMode
            'existing is probably invisible at this point, but if you are cloning it, you probably
            'want to see it
            .Visible = True 'ExistingFTextBox.Visible
            'existing is probably disabled at this point, but enable here anyway
            .Enabled = True 'ExistingFTextBox.Enabled
        End With

        Return picTemp

    End Function
    Friend Function CloneCheckedListBox(ByVal ExistingCheckedListBox As CheckedListBox, _
                                    ByVal NewCheckedListBoxName As String) As CheckedListBox
        '********************************************************************************************
        'Description:  Make a CheckedListBox just like the other one
        '
        'Parameters: CheckedListBox to clone, and name for the new one
        'Returns:    CheckedListBox - you do the text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        Dim c As New CheckedListBox

        With c
            .Name = NewCheckedListBoxName
            .MultiColumn = ExistingCheckedListBox.MultiColumn
            .Width = ExistingCheckedListBox.Width
            .CheckOnClick = ExistingCheckedListBox.CheckOnClick
            .ColumnWidth = ExistingCheckedListBox.ColumnWidth
            .Items.AddRange(ExistingCheckedListBox.Items)
            .Font = ExistingCheckedListBox.Font
            .BackColor = ExistingCheckedListBox.BackColor
            .BorderStyle = ExistingCheckedListBox.BorderStyle
            .ForeColor = ExistingCheckedListBox.ForeColor
            .Height = ExistingCheckedListBox.Height
            .IntegralHeight = ExistingCheckedListBox.IntegralHeight
            .Size = ExistingCheckedListBox.Size
            .TabStop = ExistingCheckedListBox.TabStop
            .ThreeDCheckBoxes = ExistingCheckedListBox.ThreeDCheckBoxes
            .HorizontalScrollbar = ExistingCheckedListBox.HorizontalScrollbar
            .ScrollAlwaysVisible = ExistingCheckedListBox.ScrollAlwaysVisible
            .Margin = ExistingCheckedListBox.Margin
            .CausesValidation = ExistingCheckedListBox.CausesValidation
            'anchor property not set here - it messes with the autoscroll in the page layout
            '.Anchor = ExistingCheckedListBox.Anchor
            'existing is probably invisible at this point, but if you are cloning it, you probably
            'want to see it
            .Visible = True 'ExistingCheckedListBox.Visible
            'existing is probably disabled at this point, but enable elsewhere
            .Enabled = ExistingCheckedListBox.Enabled

        End With

        Return c

    End Function
    Friend Function CloneComboBox(ByVal ExistingComboBox As ComboBox, _
                               ByVal NewComboBoxName As String) As ComboBox
        '********************************************************************************************
        'Description:  Make a ComboBox just like the other one
        '
        'Parameters: ComboBox to clone, and name for the new one
        'Returns:    ComboBox - you do the text
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'NRU 161213 Piggable
        Dim c As New ComboBox

        With c
            .Name = NewComboBoxName
            .Width = ExistingComboBox.Width
            .Font = ExistingComboBox.Font
            .BackColor = ExistingComboBox.BackColor
            .ForeColor = ExistingComboBox.ForeColor
            .Height = ExistingComboBox.Height
            .IntegralHeight = ExistingComboBox.IntegralHeight
            .Size = ExistingComboBox.Size
            .TabStop = ExistingComboBox.TabStop
            .Margin = ExistingComboBox.Margin
            .CausesValidation = ExistingComboBox.CausesValidation
            .DropDownHeight = ExistingComboBox.DropDownHeight
            .DropDownStyle = ExistingComboBox.DropDownStyle
            .DropDownWidth = ExistingComboBox.DropDownWidth
            'anchor property not set here - it messes with the autoscroll in the page layout
            '.Anchor = ExistingCheckedListBox.Anchor
            'existing is probably invisible at this point, but if you are cloning it, you probably
            'want to see it
            .Visible = True 'ExistingCheckedListBox.Visible
            'existing is probably disabled at this point, but enable elsewhere
            .Enabled = ExistingComboBox.Enabled

        End With

        Return c

    End Function
    Friend Sub DoProgressBar(ByVal sbdevent As System.Windows.Forms.StatusBarDrawItemEventArgs, _
                                                                        ByVal nProgress As Integer)
        '********************************************************************************************
        'Description: do the progressbar
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ''********************************************************************************************
        Dim rBlue As Drawing.Rectangle = Nothing
        Dim rGray As Drawing.Rectangle = Nothing
        Dim nTmp As Integer = 0
        Dim fTmp As Single = 0
        Dim oBrush As New SolidBrush(Drawing.SystemColors.Control)

        Select Case sbdevent.Index
            Case 1 ' progress bar

                If nProgress = 0 Then
                    sbdevent.Graphics.FillRectangle(oBrush, sbdevent.Bounds)
                    Exit Sub
                End If

                With sbdevent.Bounds
                    nTmp = .Width
                    fTmp = CType((.Width / 100), Single)
                    nTmp = CType((fTmp * nProgress), Integer)

                    rBlue.Size = .Size
                    rBlue.Width = nTmp
                    rBlue.X = .X
                    rBlue.Y = .Y

                    rGray.Size = .Size
                    rGray.Width = .Width - nTmp
                    rGray.X = .X + nTmp
                    rGray.Y = .Y
                End With

                sbdevent.Graphics.FillRectangle(Brushes.Blue, rBlue)
                sbdevent.Graphics.FillRectangle(oBrush, rGray)

        End Select

    End Sub
    Friend Sub InitializeForm(ByVal rForm As System.Windows.Forms.Form)
        '********************************************************************************************
        'Description:  Called when form starts up
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Application.EnableVisualStyles()


        'size form
        Call subSizeMainForm(rForm)
        'set up Std ToolBar
        Call subSetUpToolBar(rForm)
        Call subSetUpToolStrip(rForm)
        'set up statusBar
        Call subSetUpStatusBar(rForm)

        Dim void As clsControllers = Nothing
        Call SetUpStatusStrip(rForm, void)

        'set up text
        Call subSetStandardText(rForm)
        ' bit values for math
        Call FillBitValArray()

        Try
            ' what ever icon you want to use should be included in project resource file
            rForm.Icon = CType(gpsRM.GetObject("FormIcon"), Icon)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)

        End Try

    End Sub
    Friend Function GetTextboxByName(ByVal FTBCollection As Collection(Of FocusedTextBox.FocusedTextBox), _
                                ByVal sName As String) As FocusedTextBox.FocusedTextBox
        '****************************************************************************************
        'Description: 
        '
        'Parameters: 
        'Returns:  
        '
        'Modification history:
        '
        ' Date      By      Reason
        '*****************************************************************************************
        For Each ftb As FocusedTextBox.FocusedTextBox In FTBCollection
            If ftb.Name = sName Then Return ftb
        Next
        Return Nothing
    End Function
    Friend Sub LoadZoneBox(ByRef oCbo As ComboBox, ByRef oZones As clsZones, _
            Optional ByVal AddAll As Boolean = False, Optional ByVal Servername As String = Nothing)
        '********************************************************************************************
        'Description:  Load zone combo with zone names
        '
        'Parameters: Box to load, zone collection, do we add "all"? and a server name if we only
        '               want zones local to a computer (for change log)
        'Returns:    loaded box
        '
        'Modification history:
        '
        ' Date      By      Reason
        '4/21/08    gks     Add Servername to add zones local to a particular server
        '********************************************************************************************    

        If oCbo Is Nothing Then Exit Sub

        oCbo.Items.Clear()

        If AddAll Then
            oCbo.Items.Add(gcsRM.GetString("csALL"))
        End If

        For Each o As clsZone In oZones
            If IsNothing(Servername) Then
                oCbo.Items.Add(o.Name)
            Else
                If o.ServerName = Servername Then
                    oCbo.Items.Add(o.Name)
                End If
            End If
        Next

    End Sub
    Friend Sub LoadZoneBox(ByRef oClb As CheckedListBox, ByRef oZones As clsZones, _
            Optional ByVal AddAll As Boolean = False, Optional ByVal Servername As String = Nothing)
        '********************************************************************************************
        'Description:  Load zone combo with zone names
        '
        'Parameters: Box to load, zone collection, do we add "all"? and a server name if we only
        '               want zones local to a computer (for change log)
        'Returns:    loaded box
        '
        'Modification history:
        '
        ' Date      By      Reason
        '4/21/08    gks     Add Servername to add zones local to a particular server
        '********************************************************************************************    

        If oClb Is Nothing Then Exit Sub

        oClb.Items.Clear()

        If AddAll Then
            oClb.Items.Add(gcsRM.GetString("csALL"))
        End If

        For Each o As clsZone In oZones
            If IsNothing(Servername) Then
                oClb.Items.Add(o.Name)
            Else
                If o.ServerName = Servername Then
                    oClb.Items.Add(o.Name)
                End If
            End If
        Next

    End Sub
    Friend Sub LoadZoneBox(ByRef oCbo As ToolStripComboBox, ByRef oZones As clsZones, _
            Optional ByVal AddAll As Boolean = False, Optional ByVal Servername As String = Nothing)
        '********************************************************************************************
        'Description:  Load zone combo with zone names
        '
        'Parameters: Box to load, zone collection, do we add "all"? and a server name if we only
        '               want zones local to a computer (for change log)
        'Returns:    loaded box
        '
        'Modification history:
        '
        ' Date      By      Reason
        '4/21/08    gks     Add Servername to add zones local to a particular server
        '********************************************************************************************    

        If oCbo Is Nothing Then Exit Sub

        oCbo.Items.Clear()

        If AddAll Then
            oCbo.Items.Add(gcsRM.GetString("csALL"))
        End If

        For Each o As clsZone In oZones
            If IsNothing(Servername) Then
                oCbo.Items.Add(o.Name)
            Else
                If o.ServerName = Servername Then
                    oCbo.Items.Add(o.Name)
                End If
            End If
        Next

    End Sub
    Private Sub subSetStandardText(ByRef rForm As Form)
        '********************************************************************************************
        'Description:  set the text of items that should be on all screens
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        Dim sVer As String = String.Empty
        Dim oTmp As Control = Nothing
        Dim sCap As String = rForm.Text

        Try
            sVer = CType(System.Diagnostics.FileVersionInfo.GetVersionInfo( _
                        System.Reflection.Assembly.GetExecutingAssembly.Location).FileMajorPart, String)
            sVer = sVer & "." & Format(System.Diagnostics.FileVersionInfo.GetVersionInfo( _
                        System.Reflection.Assembly.GetExecutingAssembly.Location).FileMinorPart, "00")
            sVer = sVer & "." & Format(System.Diagnostics.FileVersionInfo.GetVersionInfo( _
                        System.Reflection.Assembly.GetExecutingAssembly.Location).FileBuildPart, "00")
            sVer = sVer & "." & Format(System.Diagnostics.FileVersionInfo.GetVersionInfo( _
                        System.Reflection.Assembly.GetExecutingAssembly.Location).FilePrivatePart, "00")
            'add version number
            rForm.Text = sCap & "  " & gcsRM.GetString("csVERSION") & "  " & sVer


            'find controls
            For Each oTmp In rForm.Controls
                Select Case oTmp.Name
                    Case "lblZone"
                        oTmp.Text = gcsRM.GetString("csZONE_CAP")
                    Case "lblRobot"
                        oTmp.Text = gcsRM.GetString("csROBOT_CAP")
                    Case "lblPARAM", "lblColor"
                        oTmp.Text = gcsRM.GetString("csCOLOR_CAP")
                    Case "lblCycle"
                        oTmp.Text = gcsRM.GetString("csCYCLE_CAP")
                End Select
            Next
            Dim otscMain As ToolStripContainer = DirectCast(rForm.Controls("tscMain"), ToolStripContainer)
            If otscMain IsNot Nothing Then
                For Each oTmp In otscMain.ContentPanel.Controls
                    Select Case oTmp.Name
                        Case "lblZone"
                            oTmp.Text = gcsRM.GetString("csZONE_CAP")
                        Case "lblRobot"
                            oTmp.Text = gcsRM.GetString("csROBOT_CAP")
                        Case "lblPARAM", "lblColor"
                            oTmp.Text = gcsRM.GetString("csCOLOR_CAP")
                        Case "lblCycle"
                            oTmp.Text = gcsRM.GetString("csCYCLE_CAP")
                    End Select
                Next
            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            WriteEventToLog(sCap, ex.StackTrace & vbCrLf & ex.Message)
        End Try

    End Sub
    Private Sub subSetUpToolBar(ByRef rForm As Form)
        '********************************************************************************************
        'Description:  Size and locate the ToolBar
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 04/03/14  MSW     Consolidate some more button setup
        '********************************************************************************************    
        Dim oButton As ToolBarButton = Nothing
        Dim oTool As ToolBar = Nothing
        Dim oMenu As MenuItem = Nothing

        oTool = DirectCast(GetControlByName("tlbmain", rForm), ToolBar)

        If oTool Is Nothing Then Exit Sub

        With oTool
            .Top = 0
            .Left = 0
            .Width = rForm.Width - 10
            .Wrappable = False
            .Appearance = ToolBarAppearance.Flat
            .BorderStyle = BorderStyle.FixedSingle
            .AutoSize = False
            .TextAlign = System.Windows.Forms.ToolBarTextAlign.Underneath
            .TabIndex = 0
            .ShowToolTips = True
            .Divider = True
            .DropDownArrows = True

        End With

        'note to self. If Adding a button with text (&) for an alt key, and it shows
        'up in the text instead of underscore, make sure the design version of the text has
        ' an & in it. this seems to make it work for no good reason
        For Each oButton In oTool.Buttons
            With oButton
                Select Case .Name
                    Case "btnClose"
                        .Text = gcsRM.GetString("csCLOSE")
                        .Enabled = True
                    Case "btnNew"
                        .Text = gcsRM.GetString("csNEW")
                        .Enabled = False
                    Case "btnOpen"
                        .Text = gcsRM.GetString("csOPEN")
                        .Enabled = False
                    Case "btnSave"
                        .Text = gcsRM.GetString("csSAVE")
                        .Enabled = False
                    Case "btnPrint"
                        .Text = gcsRM.GetString("csPRINT")
                        .Enabled = False

                        'dropdown stuff
                        oMenu = .DropDownMenu.MenuItems(ePrintMnu.Print)
                        oMenu.Enabled = True
                        oMenu.DefaultItem = True
                        oMenu.Text = gcsRM.GetString("csPRINTMNU")

                        oMenu = .DropDownMenu.MenuItems(ePrintMnu.SelPrinter)
                        oMenu.Enabled = True
                        oMenu.Text = gcsRM.GetString("csSELPRNTMNU")

                    Case "btnUndo"
                        .Text = gcsRM.GetString("csUNDO")
                        .Enabled = False
                    Case "btnChangeLog"
                        .Text = gcsRM.GetString("csCHANGELOG")
                        .Enabled = False

                        'dropdown stuff
                        oMenu = .DropDownMenu.MenuItems(eChangeMnu.Hours24)
                        oMenu.Enabled = True
                        oMenu.DefaultItem = True
                        oMenu.Text = gcsRM.GetString("cs24HRS")

                        oMenu = .DropDownMenu.MenuItems(eChangeMnu.Days7)
                        oMenu.Enabled = True
                        oMenu.Text = gcsRM.GetString("cs7DAYS")

                        oMenu = .DropDownMenu.MenuItems(eChangeMnu.All)
                        oMenu.Enabled = True
                        oMenu.Text = gcsRM.GetString("csALLCHG")

                    Case "btnStatus"
                        .Text = gcsRM.GetString("csSTATUS")
                        .Enabled = True

                    Case "btnCopy"
                        .Text = gcsRM.GetString("csCOPY")
                        .Enabled = False

                    Case "btnMultiView"
                        .Text = gcsRM.GetString("csMULTIVIEW")
                        .Enabled = True
                    Case "Prev", "Previous"
                        .Text = gcsRM.GetString("csPREV")
                    Case "Next"
                        .Text = gcsRM.GetString("csNEXT")
                    Case "Forward"
                        .Text = gcsRM.GetString("csFORWARD")
                    Case "Back"
                        .Text = gcsRM.GetString("csBACK")
                    Case "btnRestore"
                        .Text = gcsRM.GetString("csRESTORE")
                        '.Enabled = False

                        'dropdown stuff
                        oMenu = .DropDownMenu.MenuItems(eRestoreMnu.FromDB)
                        oMenu.Enabled = True
                        oMenu.DefaultItem = True
                        oMenu.Text = gcsRM.GetString("csFROM_DB")

                        oMenu = .DropDownMenu.MenuItems(eRestoreMnu.FromPLC)
                        oMenu.Enabled = True
                        oMenu.Text = gcsRM.GetString("csFROM_PLC")
                    Case "btnUtilities"
                        .Text = gcsRM.GetString("csUTILITIES")
                        .Enabled = False

                        'dropdown stuff
                        oMenu = .DropDownMenu.MenuItems("mnuExport")
                        If oMenu IsNot Nothing Then
                            oMenu.Enabled = True
                            oMenu.Text = gcsRM.GetString("csEXPORT_CSV")
                        End If
                        oMenu = .DropDownMenu.MenuItems("mnuImport")
                        If oMenu IsNot Nothing Then
                            oMenu.Enabled = True
                            oMenu.Text = gcsRM.GetString("csIMPORT_CSV")
                        End If
                End Select
            End With
        Next
    End Sub

    Private Sub subSetUpToolStrip(ByRef rForm As Form)
        '********************************************************************************************
        'Description:  Size and locate the Toolstrip
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/29/09  MSW	    update print dropdown menu labeling
        ' 04/02/13  DE      Added setup and localization for multi-view btnAdd and btnClear
        ' 04/03/14  MSW     Consolidate some more button setup
        '********************************************************************************************    
        Dim oTS As ToolStrip = DirectCast(GetControlByName("tlsMain", rForm), ToolStrip)

        If oTS IsNot Nothing Then
            subSetUpToolStrip(oTS)
        End If

    End Sub
    Friend Sub subSetUpToolStrip(ByRef oTS As ToolStrip)
        '********************************************************************************************
        'Description:  Size and locate the Toolstrip
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 10/29/09  MSW	    update print dropdown menu labeling
        ' 04/02/13  DE      Added setup and localization for multi-view btnAdd and btnClear
        ' 04/03/14  MSW     Consolidate some more button setup
        '********************************************************************************************    

        If oTS Is Nothing Then Exit Sub
        'case sensitive!!!
        With oTS
            .SuspendLayout()
            .Top = 0
            .Left = 0
            .AllowDrop = False
            .AllowItemReorder = False
            .AllowMerge = False
            .AutoSize = False
            .Stretch = True
            .GripStyle = ToolStripGripStyle.Hidden
            .ShowItemToolTips = True
            .Font = New System.Drawing.Font("Arial", 10, FontStyle.Regular)

            For Each o As ToolStripItem In .Items
                Select Case o.Name.Substring(3)
                    Case "Add" 'DE 04/02/13
                        o.Text = gcsRM.GetString("csADD")
                        o.Image = DirectCast(gcsRM.GetObject("imgAdd"), Image)
                    Case "Clear"  'DE 04/02/13
                        o.Text = gcsRM.GetString("csCLEARALL")
                        o.Image = DirectCast(gcsRM.GetObject("imgDelete"), Image)
                    Case "Close"
                        o.Text = gcsRM.GetString("csCLOSE")
                        o.Enabled = True
                        o.Image = DirectCast(gcsRM.GetObject("imgClose"), Image)
                    Case "Save"
                        o.Text = gcsRM.GetString("csSAVE")
                        o.Enabled = False
                        o.Image = DirectCast(gcsRM.GetObject("imgSave"), Image)
                    Case "New"
                        o.Text = gcsRM.GetString("csNEW")
                        o.Image = DirectCast(gcsRM.GetObject("imgNew"), Image)
                    Case "Open"
                        o.Text = gcsRM.GetString("csOPEN")
                        o.Image = DirectCast(gcsRM.GetObject("imgOpen"), Image)
                    Case "Prev", "Previous"
                        o.Text = gcsRM.GetString("csPREV")
                        o.Image = DirectCast(gcsRM.GetObject("imgPrevious"), Image)
                    Case "Next"
                        o.Text = gcsRM.GetString("csNEXT")
                        o.Image = DirectCast(gcsRM.GetObject("imgNext"), Image)
                    Case "Forward"
                        o.Text = gcsRM.GetString("csFORWARD")
                        o.Image = DirectCast(gcsRM.GetObject("imgForward"), Image)
                    Case "Back"
                        o.Text = gcsRM.GetString("csBACK")
                        o.Image = DirectCast(gcsRM.GetObject("imgBack"), Image)
                    Case "Print"
                        Dim oo As ToolStripSplitButton = _
                            DirectCast(o, ToolStripSplitButton)
                        oo.Text = gcsRM.GetString("csPRINT")
                        oo.Enabled = False
                        oo.Image = DirectCast(gcsRM.GetObject("imgPrint"), Image)
                        Dim ooo As ToolStripMenuItem
                        ooo = DirectCast(oo.DropDownItems("mnuPrint"), ToolStripMenuItem)
                        If ooo IsNot Nothing Then
                            ooo.Text = gcsRM.GetString("csPRINTMNU")
                            ooo.Enabled = True
                        End If
                        ooo = DirectCast(oo.DropDownItems("mnuSelPrinter"), ToolStripMenuItem)
                        If ooo IsNot Nothing Then
                            ooo.Text = gcsRM.GetString("csSELPRNTMNU")
                            ooo.Enabled = True
                        End If
                        ooo = DirectCast(oo.DropDownItems("mnuPrintPreview"), ToolStripMenuItem)
                        If ooo IsNot Nothing Then
                            ooo.Text = gcsRM.GetString("csPRINT_PREVIEW")
                            ooo.Enabled = True
                        End If
                        ooo = DirectCast(oo.DropDownItems("mnuPageSetup"), ToolStripMenuItem)
                        If ooo IsNot Nothing Then
                            ooo.Text = gcsRM.GetString("csPAGE_SETUP")
                            ooo.Enabled = True
                        End If
                        ooo = DirectCast(oo.DropDownItems("mnuPrintFile"), ToolStripMenuItem)
                        If ooo IsNot Nothing Then
                            ooo.Text = gcsRM.GetString("csPRINT_FILE")
                            ooo.Enabled = True
                        End If
                        ooo = DirectCast(oo.DropDownItems("mnuPrintOptions"), ToolStripMenuItem)
                        If ooo IsNot Nothing Then
                            ooo.Text = gcsRM.GetString("csPRINT_OPTIONS")
                            ooo.Enabled = True
                        End If
                    Case "Undo"
                        o.Text = gcsRM.GetString("csUNDO")
                        o.Enabled = False
                        o.Image = DirectCast(gcsRM.GetObject("imgUndo"), Image)
                    Case "ChangeLog"
                        Dim oo As ToolStripSplitButton = _
                            DirectCast(o, ToolStripSplitButton)
                        oo.Text = gcsRM.GetString("csCHANGELOG")
                        oo.Enabled = True
                        oo.Image = DirectCast(gcsRM.GetObject("imgChange"), Image)
                        With oo.DropDownItems("mnuLast24")
                            .Text = gcsRM.GetString("cs24HRS")
                            .Enabled = True
                        End With
                        With oo.DropDownItems("mnuLast7")
                            .Text = gcsRM.GetString("cs7DAYS")
                            .Enabled = True
                        End With
                        With oo.DropDownItems("mnuAllChanges")
                            .Text = gcsRM.GetString("csALLCHG")
                            .Enabled = True
                        End With
                    Case "Status"
                        o.Text = gcsRM.GetString("csSTATUS")
                        o.Enabled = True
                        o.Image = DirectCast(gcsRM.GetObject("imgStatus"), Image)
                    Case "Copy"
                        o.Text = gcsRM.GetString("csCOPY")
                        o.Enabled = False
                        o.Image = DirectCast(gcsRM.GetObject("imgCopy"), Image)
                    Case "MultiView"
                        o.Text = gcsRM.GetString("csMULTIVIEW")
                        o.Enabled = True
                        o.Image = DirectCast(gcsRM.GetObject("imgMultiView"), Image)
                    Case "Restore"
                        o.Text = gcsRM.GetString("csRESTORE")
                        o.Enabled = False
                        o.Visible = False
                        o.Image = DirectCast(gcsRM.GetObject("imgRestore"), Image)
                    Case "Device"
                        o.Text = gcsRM.GetString("csDEVICES")
                        o.Enabled = True
                        o.Image = DirectCast(gcsRM.GetObject("imgDevices"), Image)
                    Case "Utilities"
                        Dim oo As ToolStripSplitButton = _
                            DirectCast(o, ToolStripSplitButton)
                        oo.Text = gcsRM.GetString("csUTILITIES")
                        oo.Enabled = False
                        oo.Image = DirectCast(gcsRM.GetObject("imgUtil"), Image)
                        Dim ooo As ToolStripMenuItem

                        'dropdown stuff
                        ooo = DirectCast(oo.DropDownItems("mnuExport"), ToolStripMenuItem)
                        If ooo IsNot Nothing Then
                            ooo.Enabled = True
                            ooo.Text = gcsRM.GetString("csEXPORT_CSV")
                        End If
                        ooo = DirectCast(oo.DropDownItems("mnuImport"), ToolStripMenuItem)
                        If ooo IsNot Nothing Then
                            ooo.Enabled = True
                            ooo.Text = gcsRM.GetString("csIMPORT_CSV")
                        End If
                End Select
            Next
            .RenderMode = ToolStripRenderMode.ManagerRenderMode
            .ResumeLayout()
        End With

    End Sub
    Private Sub subSetUpStatusBar(ByRef rForm As Form)
        '********************************************************************************************
        'Description:  Size and locate the StatusBar
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        Dim oStat As StatusBar = Nothing
        Const nROBOTBOXES As Integer = 10
        Dim nWidth As Integer = 0
        Dim i% = 0

        oStat = DirectCast(GetControlByName("stbStatus", rForm), StatusBar)

        If oStat Is Nothing Then Exit Sub

        nWidth = rForm.Width - 10

        With oStat
            '.Anchor = AnchorStyles.Bottom
            .Dock = DockStyle.Bottom
            .SizingGrip = False
            .Width = nWidth
            'text
            .Panels(0).Width = CType((nWidth * 0.475), Integer)
            'progress
            .Panels(1).Width = CType((nWidth * 0.175), Integer)
            .Panels(1).Style = StatusBarPanelStyle.OwnerDraw


            For i% = 1 To nROBOTBOXES + 1
                Dim panel As New StatusBarPanel
                panel.Width = CType(((nWidth * 0.35) / (nROBOTBOXES + 1)), Integer)
                panel.Style = StatusBarPanelStyle.OwnerDraw
                If i% > 1 Then
                    panel.Name = "R" & (i% - 1).ToString(mLanguage.FixedCulture)
                Else
                    panel.Name = "LoginStatus"

                End If
                .Panels.Add(panel)

            Next


        End With
    End Sub
    Friend Sub SetUpStatusStrip(ByRef rForm As Form, ByVal Controllers As clsControllers)
        '********************************************************************************************
        'Description:  Size and locate the StatusBar - this routine needs work - have to locate
        ' the controls properly - now set up with designer - they dont cooperate
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 08/21/13  MSW     SetUpStatusStrip - hide the sizing grip for the status bar so the spacing works right
        ' 09/13/13  MSW     SetUpStatusStrip, SetResizeStatusStrip - get the startusbar   4.01.05.02
        '                       to resize properly
        '********************************************************************************************    
        Dim oStat As StatusStrip = Nothing
        Dim nWidth As Integer = 0
        Dim i% = 0
        Dim nHeight As Integer = 25
        Dim oPad As New Padding(2, 1, 2, 1)
        Dim nRunWidth As Integer = 0

        oStat = DirectCast(GetControlByName("stsStatus", rForm), StatusStrip)

        If oStat Is Nothing Then Exit Sub

        nWidth = rForm.Size.Width - 10

        oStat.Width = nWidth

        With oStat
            .SuspendLayout()

            .AllowDrop = False
            .AllowItemReorder = False
            .AllowMerge = False
            .AutoSize = False
            .Height = nHeight
            .ShowItemToolTips = True
            .SizingGrip = False 'This set to true is what messed up the status bar spacing when the progress bar was visible
            .Stretch = True
            .CanOverflow = True
            .RenderMode = gnRENDERMODE

            'status label
            Dim l As ToolStripStatusLabel = DirectCast(oStat.Items("lblStatus"), ToolStripStatusLabel)
            If Not (l Is Nothing) Then
                With l
                    .AutoSize = False
                    .Width = CType((nWidth * 0.475), Integer)
                    .Text = String.Empty
                    .Height = nHeight - 2
                    .Padding = oPad
                    nRunWidth = .Width
                End With
            End If

            'progress bar
            Dim p As ToolStripProgressBar = DirectCast(oStat.Items("tspProgress"), ToolStripProgressBar)
            If Not (p Is Nothing) Then
                With p
                    .AutoSize = False
                    .Width = CType((nWidth * 0.175), Integer)
                    .Height = nHeight - 1
                    .Padding = oPad
                    nRunWidth += .Width
                    gtSSSize.ProgBarVisibleSize = .Width
                    gtSSSize.ProgBarInvisSize = 0
                    .Visible = False
                End With
            End If

            Dim l1 As ToolStripStatusLabel = DirectCast(oStat.Items("lblSpacer"), ToolStripStatusLabel)
            If Not (l Is Nothing) Then
                With l1
                    .AutoSize = False
                    gtSSSize.SpaceLabelInvisSize = 10
                    gtSSSize.SpaceLabelVisbleSize = gtSSSize.SpaceLabelInvisSize + gtSSSize.ProgBarVisibleSize
                    .Width = gtSSSize.SpaceLabelVisbleSize
                    .Text = String.Empty
                    .Height = nHeight - 2
                    .Padding = oPad
                    nRunWidth += gtSSSize.SpaceLabelInvisSize
                End With
            End If

            'function button
            Dim b As ToolStripDropDownButton = _
                        DirectCast(oStat.Items("btnFunction"), ToolStripDropDownButton)
            If Not (b Is Nothing) Then
                nRunWidth += b.Width
                b.AutoSize = True
                b.Padding = oPad
                b.Image = DirectCast(gcsRM.GetObject("imgLock"), Image)
                For Each o As ToolStripItem In b.DropDownItems
                    Select Case o.Name
                        Case "mnuLogin"
                            o.Text = gcsRM.GetString("csLOGIN")
                        Case "mnuLogOut"
                            o.Text = gcsRM.GetString("csLOGOUT")
                            o.Enabled = False
                        Case "mnuRemote"
                            o.Text = gcsRM.GetString("csREMOTE_ZONE")
                            o.Enabled = False
                        Case "mnuLocal"
                            o.Text = gcsRM.GetString("csLOCAL_ZONE")
                            o.Enabled = False

                    End Select
                Next
            End If

            'get rid of old robots
            For i% = oStat.Items.Count - 1 To 0 Step -1
                Dim o As ToolStripItem = oStat.Items(i%)
                Select Case o.Name
                    Case "btnFunction", "tspProgress", "lblStatus", "lblSpacer"

                    Case Else
                        oStat.Items.Remove(o)

                End Select
            Next

            Dim lx As New ToolStripStatusLabel
            With lx
                .AutoSize = False
                .Size = New Size(24, 24)
                .Name = "lblPic"
                .DisplayStyle = ToolStripItemDisplayStyle.Image
                .ImageScaling = ToolStripItemImageScaling.None
                .Visible = True
            End With
            oStat.Items.Add(lx)

            If Controllers Is Nothing Then
                .ResumeLayout()
                Exit Sub
            End If

            If Controllers.Count = 0 Then
                .ResumeLayout()
                Exit Sub
            End If


            nRunWidth += 20 ' fudge space if you get 2 rows of robots bump this up

            Dim nRestofbar As Integer = nWidth - nRunWidth
            'start assuming square pics
            Dim nRobotSpace As Integer = (Controllers.Count + 1) * nHeight '2/11/09 +1 added to get last one out of corner
            Dim nBotWidth As Integer
            If nRobotSpace >= nRestofbar Then
                nBotWidth = CInt(nRestofbar / Controllers.Count)
            Else
                ' add a pad label
                Dim s As New ToolStripStatusLabel
                With s
                    .AutoSize = False
                    .Width = nRestofbar - nRobotSpace
                    .Name = "Spacer"
                    .Visible = True
                    .DisplayStyle = ToolStripItemDisplayStyle.None
                    .Height = nHeight
                End With
                oStat.Items.Add(s)
                nBotWidth = nHeight
            End If

            'add new robots
            Dim oBot As Bitmap = CType(gcsRM.GetObject("imgSBRGrey"), Bitmap)
            Dim oCol As Color = oBot.GetPixel(0, 0)

            For i% = 1 To Controllers.Count
                Dim s As New ToolStripStatusLabel
                With s
                    .AutoSize = False
                    .Width = nBotWidth
                    .Name = "lbl" & Controllers(i% - 1).Name
                    .Visible = True
                    .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
                    .ImageAlign = ContentAlignment.MiddleCenter
                    .Image = oBot
                    .Height = nHeight
                    .ImageTransparentColor = oCol
                    .AutoToolTip = True

                End With
                oStat.Items.Add(s)
            Next

            .ResumeLayout()

            .Update()

        End With
        oStat.Tag = Controllers.Count
        AddHandler oStat.Resize, AddressOf SetResizeStatusStrip
    End Sub
    Friend Sub SetResizeStatusStrip(ByVal sender As Object, Optional ByVal e As System.EventArgs = Nothing)
        '********************************************************************************************
        'Description:  Size and locate the StatusBar - this routine needs work - have to locate
        ' the controls properly - now set up with designer - they dont cooperate
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 09/13/13  MSW     SetUpStatusStrip, SetResizeStatusStrip - get the startusbar   4.01.05.02
        '                       to resize properly
        '********************************************************************************************    
        Dim oStat As StatusStrip = Nothing
        Dim nWidth As Integer = 0
        Dim i% = 0
        Dim nHeight As Integer = 25
        Dim oPad As New Padding(2, 1, 2, 1)
        Dim nRunWidth As Integer = 0

        oStat = DirectCast(sender, StatusStrip)
        If oStat Is Nothing Then Exit Sub

        nWidth = oStat.Parent.Width - 10

        oStat.Width = nWidth

        Dim nControllerCount As Integer = DirectCast(oStat.Tag, Integer)

        With oStat
            .SuspendLayout()
            'status label
            Dim l As ToolStripStatusLabel = DirectCast(oStat.Items("lblStatus"), ToolStripStatusLabel)
            If Not (l Is Nothing) Then
                With l
                    .AutoSize = False
                    .Width = CType((nWidth * 0.475), Integer)
                    .Text = String.Empty
                    .Height = nHeight - 2
                    .Padding = oPad
                    nRunWidth = .Width
                End With
            End If

            'progress bar
            Dim bProgressBarVisible As Boolean = False
            Dim p As ToolStripProgressBar = DirectCast(oStat.Items("tspProgress"), ToolStripProgressBar)
            If Not (p Is Nothing) Then
                With p
                    .AutoSize = False
                    .Width = CType((nWidth * 0.175), Integer)
                    .Height = nHeight - 1
                    .Padding = oPad
                    nRunWidth += .Width
                    gtSSSize.ProgBarVisibleSize = .Width
                    gtSSSize.ProgBarInvisSize = 0
                    bProgressBarVisible = .Visible
                End With
            End If

            Dim l1 As ToolStripStatusLabel = DirectCast(oStat.Items("lblSpacer"), ToolStripStatusLabel)
            If Not (l Is Nothing) Then
                With l1
                    .AutoSize = False
                    gtSSSize.SpaceLabelInvisSize = 10
                    gtSSSize.SpaceLabelVisbleSize = gtSSSize.SpaceLabelInvisSize + gtSSSize.ProgBarVisibleSize
                    If bProgressBarVisible Then
                        .Width = gtSSSize.SpaceLabelInvisSize
                    Else
                        .Width = gtSSSize.SpaceLabelVisbleSize
                    End If
                    nRunWidth += gtSSSize.SpaceLabelInvisSize
                    .Text = String.Empty
                    .Height = nHeight - 2
                    .Padding = oPad
                End With
            End If

            'function button
            Dim b As ToolStripDropDownButton = _
                        DirectCast(oStat.Items("btnFunction"), ToolStripDropDownButton)
            If Not (b Is Nothing) Then
                b.AutoSize = True
                nRunWidth += b.Width
                b.Padding = oPad
            End If

            If nControllerCount = 0 Then
                .ResumeLayout()
                Exit Sub
            End If


            nRunWidth += 20 ' fudge space if you get 2 rows of robots bump this up

            Dim nRestofbar As Integer = nWidth - nRunWidth
            'start assuming square pics
            Dim nRobotSpace As Integer = (nControllerCount + 1) * nHeight '2/11/09 +1 added to get last one out of corner
            Dim nBotWidth As Integer
            If nRobotSpace >= nRestofbar Then
                nBotWidth = CInt(nRestofbar / nControllerCount)
            Else
                ' add a pad label
                Dim s As ToolStripStatusLabel = DirectCast(oStat.Items("Spacer"), ToolStripStatusLabel)
                If Not (s Is Nothing) Then
                With s
                    .Width = nRestofbar - nRobotSpace
                    .Height = nHeight
                End With

                End If
                nBotWidth = nHeight
            End If


            .ResumeLayout()

            .Update()

        End With
    End Sub

    Private Sub subSizeMainForm(ByRef rForm As Form)
        '********************************************************************************************
        'Description:  Size and locate the form
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim i%
        Dim nUbound As Integer = 0
        Dim nWidth As Integer = 800
        Dim nHeight As Integer = 600
        Dim tSize As Size

        ' Gets an array of all the screens connected to the system.
        Dim Screens() As System.Windows.Forms.Screen = System.Windows.Forms.Screen.AllScreens
        nUbound = Screens.GetUpperBound(0)

        For i% = 0 To nUbound
            If Screens(i%).Primary Then
                nWidth = Screens(i%).WorkingArea.Width
                nHeight = Screens(i%).WorkingArea.Height - gnPW_MAINBANNERHEIGHT
                nHeight = nHeight - gnPW_STATUSBARHEIGHT
            End If
        Next


        With rForm
            'to start sub screens a little lower
            Dim nOffset As Integer = 0
            If .Name <> "frmMain" Then
                nOffset = 25
            End If
            ' initial size of screen
            .Height = nHeight - nOffset
            .Top = gnPW_MAINBANNERHEIGHT + nOffset
            .Width = nWidth
            .Left = 0

            'size limits for screen resizing
            tSize.Height = .Height
            tSize.Width = .Width
            .MaximumSize = tSize

            tSize.Height = 100
            tSize.Width = 200
            .MinimumSize = tSize

            'set some screen properties
            .AutoScaleMode = AutoScaleMode.Font
            .AutoScroll = True
            .KeyPreview = True

            'set bounds property
            MaxBound = New Drawing.Rectangle(.Left, .Top, .Width, .Height)


            Dim P As Panel = DirectCast(GetControlByName("pnlMain", rForm), Panel)
            If P Is Nothing Then Exit Sub

            P.BorderStyle = BorderStyle.None
            'top  should have been set at design time!
            P.Left = 9
            P.Width = .Width - (P.Left * 2)
            P.Height = .Height - 100
            P.AutoScroll = True
            P.Enabled = False
        End With

    End Sub
    Friend Sub ClearAllTextBoxes(ByRef rCollection As Collection(Of FocusedTextBox.FocusedTextBox))
        '********************************************************************************************
        'Description:  clear the text
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        Dim o As FocusedTextBox.FocusedTextBox

        If rCollection Is Nothing Then Exit Sub

        For Each o In rCollection
            o.Text = String.Empty
        Next
    End Sub
    Friend Sub LockAllTextBoxes(ByRef rCollection As Collection(Of FocusedTextBox.FocusedTextBox), _
                                                                     ByVal bLock As Boolean)
        '********************************************************************************************
        'Description:  Make it read only
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        If rCollection Is Nothing Then Exit Sub
        For Each o As FocusedTextBox.FocusedTextBox In rCollection
            o.ReadOnly = bLock
            o.CausesValidation = Not bLock
            'Trace.WriteLine(o.Name)
        Next
    End Sub
    Friend Sub LockAllCheckBoxes(ByRef rCollection As Collection(Of CheckBox), _
                                                                      ByVal bLock As Boolean)
        '********************************************************************************************
        'Description:  Make it read only
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        If rCollection Is Nothing Then Exit Sub
        For Each o As CheckBox In rCollection
            o.Enabled = bLock
            'o.CausesValidation = Not bLock
            'Trace.WriteLine(o.Name)
        Next
    End Sub
    Friend Sub LockAllComboBoxes(ByRef rCollection As Collection(Of ComboBox), _
                                                                  ByVal bLock As Boolean)
        '********************************************************************************************
        'Description:  Make it read only
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        'NRU 161213 Piggable
        If rCollection Is Nothing Then Exit Sub
        For Each o As ComboBox In rCollection
            o.Enabled = bLock
            'o.CausesValidation = Not bLock
            'Trace.WriteLine(o.Name)
        Next
    End Sub
    Friend Sub LockAllCheckedListBoxes(ByRef rCollection As Collection(Of CheckedListBox), _
                                                                 ByVal bLock As Boolean)
        '********************************************************************************************
        'Description:  Make it read only
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        If rCollection Is Nothing Then Exit Sub

        For Each o As CheckedListBox In rCollection
            If bLock Then
                o.ClearSelected()
                o.BackColor = Color.White
            Else

            End If
            o.CausesValidation = Not bLock
            o.Enabled = Not bLock

        Next
    End Sub
    Friend Sub LockAllButtons(ByRef rCollection As Collection(Of Button), _
                                                             ByVal bLock As Boolean)
        '********************************************************************************************
        'Description:  Make it read only
        '
        'Parameters: main form
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        If rCollection Is Nothing Then Exit Sub

        For Each o As Button In rCollection
            o.CausesValidation = Not bLock
            o.Enabled = Not bLock
        Next
    End Sub
    Friend Sub LoadTextBoxCollection(ByRef rForm As Form, ByVal ContainerName As String, _
                                ByRef rCollection As Collection(Of FocusedTextBox.FocusedTextBox))
        '********************************************************************************************
        'Description:  load a collection with all the textboxes on the form use name as key
        '
        'Parameters: collection
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '02/14/07   gks     Add ContainerName
        '03/07/07   gks     Just look for type in name
        '********************************************************************************************    
        Dim oT As Control = Nothing
        Dim sType As String = Strings.Left(ContainerName, 3).ToLower

        rCollection = New Collection(Of FocusedTextBox.FocusedTextBox)

        Select Case sType
            Case "pnl" '"pnlMain"
                Dim o As Panel = DirectCast(GetControlByName(ContainerName, rForm), Panel)

                If o Is Nothing Then Exit Sub

                For Each oT In o.Controls
                    If TypeOf (oT) Is FocusedTextBox.FocusedTextBox Then
                        rCollection.Add(DirectCast(oT, FocusedTextBox.FocusedTextBox)) ', oT.Name)
                    End If
                Next

            Case "tab" '"tabPage1", "tabPage2", "tabPage3"
                Dim o As TabPage = DirectCast(GetControlByName(ContainerName, rForm), TabPage)

                If o Is Nothing Then Exit Sub

                For Each oT In o.Controls
                    If TypeOf (oT) Is FocusedTextBox.FocusedTextBox Then
                        rCollection.Add(DirectCast(oT, FocusedTextBox.FocusedTextBox)) ', oT.Name)
                    End If
                Next

            Case "gpb"
                Dim o As GroupBox = DirectCast(GetControlByName(ContainerName, rForm), GroupBox)

                If o Is Nothing Then Exit Sub

                For Each oT In o.Controls
                    If TypeOf (oT) Is FocusedTextBox.FocusedTextBox Then
                        rCollection.Add(DirectCast(oT, FocusedTextBox.FocusedTextBox)) ', oT.Name)
                    End If
                Next

            Case "tlp"
                Dim o As TableLayoutPanel = _
                            DirectCast(GetControlByName(ContainerName, rForm), TableLayoutPanel)

                If o Is Nothing Then Exit Sub

                For Each oT In o.Controls
                    If TypeOf (oT) Is FocusedTextBox.FocusedTextBox Then
                        rCollection.Add(DirectCast(oT, FocusedTextBox.FocusedTextBox)) ', oT.Name)
                    End If
                Next

        End Select

    End Sub
    '############################################################################################################




    Friend Sub LoadCheckBoxCollection(ByRef rForm As Form, ByVal ContainerName As String, _
                            ByRef rCollection As Collection(Of CheckBox))
        '********************************************************************************************
        'Description:  load a collection with all the textboxes on the form use name as key
        '
        'Parameters: collection
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '03/03/11   JBW     Hacked together from loadtexboxcollection
        '********************************************************************************************    
        Dim oT As Control = Nothing
        Dim sType As String = Strings.Left(ContainerName, 3).ToLower

        rCollection = New Collection(Of CheckBox)

        Select Case sType
            Case "pnl" '"pnlMain"
                Dim o As Panel = DirectCast(GetControlByName(ContainerName, rForm), Panel)

                If o Is Nothing Then Exit Sub

                For Each oT In o.Controls
                    If TypeOf (oT) Is CheckBox Then
                        rCollection.Add(DirectCast(oT, CheckBox)) ', oT.Name)
                    End If
                Next

            Case "tab" '"tabPage1", "tabPage2", "tabPage3"
                Dim o As TabPage = DirectCast(GetControlByName(ContainerName, rForm), TabPage)

                If o Is Nothing Then Exit Sub

                For Each oT In o.Controls
                    If TypeOf (oT) Is CheckBox Then
                        rCollection.Add(DirectCast(oT, CheckBox)) ', oT.Name)
                    End If
                Next

            Case "gpb"
                Dim o As GroupBox = DirectCast(GetControlByName(ContainerName, rForm), GroupBox)

                If o Is Nothing Then Exit Sub

                For Each oT In o.Controls
                    If TypeOf (oT) Is CheckBox Then
                        rCollection.Add(DirectCast(oT, CheckBox)) ', oT.Name)
                    End If
                Next

            Case "tlp"
                Dim o As TableLayoutPanel = _
                            DirectCast(GetControlByName(ContainerName, rForm), TableLayoutPanel)

                If o Is Nothing Then Exit Sub

                For Each oT In o.Controls
                    If TypeOf (oT) Is CheckBox Then
                        rCollection.Add(DirectCast(oT, CheckBox)) ', oT.Name)
                    End If
                Next

        End Select

    End Sub




    '############################################################################################################
    Friend Sub LoadCheckedListBoxCollection(ByRef rForm As Form, ByVal ContainerName As String, _
                            ByRef rCollection As Collection(Of CheckedListBox))
        '********************************************************************************************
        'Description:  load a collection with all the textboxes on the form use name as key
        '
        'Parameters: collection
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************    
        Dim oT As Control = Nothing
        Dim sType As String = Strings.Left(ContainerName, 3)

        rCollection = New Collection(Of CheckedListBox)

        Select Case sType
            Case "pnl" '"pnlMain"
                Dim o As Panel = DirectCast(GetControlByName(ContainerName, rForm), Panel)

                If o Is Nothing Then Exit Sub

                For Each oT In o.Controls
                    If TypeOf (oT) Is CheckedListBox Then
                        rCollection.Add(DirectCast(oT, CheckedListBox)) ', oT.Name)
                    End If
                Next

            Case "tab" '"tabPage1", "tabPage2", "tabPage3"
                Dim o As TabPage = DirectCast(GetControlByName(ContainerName, rForm), TabPage)

                If o Is Nothing Then Exit Sub

                For Each oT In o.Controls
                    If TypeOf (oT) Is CheckedListBox Then
                        rCollection.Add(DirectCast(oT, CheckedListBox)) ', oT.Name)
                    End If
                Next

            Case "gpb"
                Dim o As GroupBox = DirectCast(GetControlByName(ContainerName, rForm), GroupBox)

                If o Is Nothing Then Exit Sub

                For Each oT In o.Controls
                    If TypeOf (oT) Is CheckedListBox Then
                        rCollection.Add(DirectCast(oT, CheckedListBox)) ', oT.Name)
                    End If
                Next

            Case "tlp"
                Dim o As TableLayoutPanel = _
                            DirectCast(GetControlByName(ContainerName, rForm), TableLayoutPanel)

                If o Is Nothing Then Exit Sub

                For Each oT In o.Controls
                    If TypeOf (oT) Is CheckedListBox Then
                        rCollection.Add(DirectCast(oT, CheckedListBox)) ', oT.Name)
                    End If
                Next

        End Select

    End Sub
    Friend Sub LoadComboBoxCollection(ByRef rForm As Form, ByVal ContainerName As String, _
                  ByRef rCollection As Collection(Of ComboBox))
        '********************************************************************************************
        'Description:  load a collection with all the textboxes on the form use name as key
        '
        'Parameters: collection
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '11/15/12   HGB     Hacked together from loadcheckboxcollection
        '********************************************************************************************    
        Dim oT As Control = Nothing
        Dim sType As String = Strings.Left(ContainerName, 3).ToLower

        rCollection = New Collection(Of ComboBox)

        Select Case sType
            Case "pnl" '"pnlMain"
                Dim o As Panel = DirectCast(GetControlByName(ContainerName, rForm), Panel)

                If o Is Nothing Then Exit Sub

                For Each oT In o.Controls
                    If TypeOf (oT) Is ComboBox Then
                        rCollection.Add(DirectCast(oT, ComboBox)) ', oT.Name)
                    End If
                Next

            Case "tab" '"tabPage1", "tabPage2", "tabPage3"
                Dim o As TabPage = DirectCast(GetControlByName(ContainerName, rForm), TabPage)

                If o Is Nothing Then Exit Sub

                For Each oT In o.Controls
                    If TypeOf (oT) Is ComboBox Then
                        rCollection.Add(DirectCast(oT, ComboBox)) ', oT.Name)
                    End If
                Next

            Case "gpb"
                Dim o As GroupBox = DirectCast(GetControlByName(ContainerName, rForm), GroupBox)

                If o Is Nothing Then Exit Sub

                For Each oT In o.Controls
                    If TypeOf (oT) Is ComboBox Then
                        rCollection.Add(DirectCast(oT, ComboBox)) ', oT.Name)
                    End If
                Next

            Case "tlp"
                Dim o As TableLayoutPanel = _
                            DirectCast(GetControlByName(ContainerName, rForm), TableLayoutPanel)

                If o Is Nothing Then Exit Sub

                For Each oT In o.Controls
                    If TypeOf (oT) Is ComboBox Then
                        rCollection.Add(DirectCast(oT, ComboBox)) ', oT.Name)
                    End If
                Next

        End Select

    End Sub

    Friend Sub DoStatusBar2(ByRef stsStatus As StatusStrip, ByVal LoggedOnUser As String, _
                            ByVal Privilege As ePrivilege, ByVal ZoneIsRemote As Boolean)
        '********************************************************************************************
        'Description: do icon enable menus etc
        '
        'Parameters: bEnable - hide, which param
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '06/01/07   gks     moved to subDoStatusStrip for overloading
        '********************************************************************************************

        Try

            Call subDoStatusStrip(stsStatus, LoggedOnUser, Privilege, ZoneIsRemote, True)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Friend Sub DoStatusBar2(ByRef stsStatus As StatusStrip, ByVal LoggedOnUser As String, _
                            ByVal Privilege As ePrivilege, ByVal ZoneIsRemote As Boolean, _
                            ByVal bAllowRemote As Boolean)
        '********************************************************************************************
        'Description: do icon enable menus etc
        '
        'Parameters: bEnable - hide, which param
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '06/01/07   gks     moved to subDoStatusStrip for overloading this is the preferred one
        '********************************************************************************************
        ' function button
        Try

            Call subDoStatusStrip(stsStatus, LoggedOnUser, Privilege, ZoneIsRemote, bAllowRemote)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

    End Sub
    Private Sub subDoStatusStrip(ByVal stsStatus As StatusStrip, ByVal LoggedOnUser As String, _
                        ByVal Privilege As ePrivilege, ByVal ZoneIsRemote As Boolean, _
                        ByVal bAllowRemote As Boolean)
        '********************************************************************************************
        'Description: do icon enable menus etc
        '
        'Parameters: bEnable - hide, which param
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        ' 06/01/09  MSW     Catch the calls from a different thread - point to the workaround for 
        '                   whoever has to deal with this in other screens 
        ' 10/16/12  MSW     Don't catch them anymore, sometimes that works best
        '********************************************************************************************
        ' function button
        Dim b As ToolStripDropDownButton = _
                            DirectCast(stsStatus.Items("btnFunction"), ToolStripDropDownButton)
        Try

            With b
                'This can get called by the password or robot object from a different thread
                'If stsStatus.InvokeRequired Then
                '    'It was getting an attitude about being called from the wrong thread
                '    'later in this routine.
                '    'There's an example of the code to get around it in FluidMaint, ColorChange and presets frmMain
                '    'In the declares, add:
                '    ' The delegates (function pointers) enable asynchronous calls from the password object.
                '    'Delegate Sub LoginCallBack()
                '    'Delegate Sub LogOutCallBack()
                '    'Delegate Sub ControllerStatusChange(ByVal Controller As clsController)
                '    'Code changes are in moPassword_LogIn(), moPassword_LogOut() and colControllers_ConnectionStatusChange() 
                '    Debug.Print("Different Thread")
                '    Debug.Assert(False)
                'End If
                If LoggedOnUser = String.Empty Then
                    .Image = DirectCast(gcsRM.GetObject("imgLock"), Image)
                    .DropDownItems("mnuLogin").Enabled = True
                    .DropDownItems("mnuLogOut").Enabled = False
                    .DropDownItems("mnuRemote").Enabled = False
                    .DropDownItems("mnuLocal").Enabled = False
                Else
                    If Privilege >= ePrivilege.Edit Then
                        .Image = DirectCast(gcsRM.GetObject("imgUnlock"), Image)
                    End If
                    .DropDownItems("mnuLogin").Enabled = False
                    .DropDownItems("mnuLogOut").Enabled = True
                    If ZoneIsRemote Then
                        .DropDownItems("mnuRemote").Enabled = False
                        .DropDownItems("mnuLocal").Enabled = True
                    Else
                        If Privilege >= ePrivilege.Remote Then
                            .DropDownItems("mnuRemote").Enabled = True
                        End If
                        .DropDownItems("mnuLocal").Enabled = False
                    End If
                End If
                .DropDownItems("mnuRemote").Visible = bAllowRemote
                .DropDownItems("mnuLocal").Visible = bAllowRemote
                .DropDownItems("mnuLogin").Invalidate()
                .DropDownItems("mnuLogOut").Invalidate()
                .DropDownItems("mnuRemote").Invalidate()
                .DropDownItems("mnuLocal").Invalidate()
                .Invalidate()

            End With

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try

        Try
            Dim l As ToolStripItem = TryCast(stsStatus.Items("lblPic"), ToolStripItem)
            If l Is Nothing = False Then
                'l.Image = DirectCast(gcsRM.GetObject("z_Beaker"), Image)
                'l.Invalidate()
            End If
        Catch ex As Exception

        End Try
        stsStatus.Refresh()


    End Sub
    Friend Sub MoveCursorOffButton(ByRef rButton As ToolStripItem)
        '********************************************************************************************
        'Description: This piece of manure moves the cursor off a button so that when you disable
        '               it it doesnt stay selected
        '
        'Parameters: 
        'Returns:    
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Dim nHeightTS As Integer = rButton.Owner.Top + rButton.Owner.Height
        Dim p As Point = Cursor.Position
        Dim oTS As Control = rButton.Owner
        Dim p1 As Point = oTS.PointToClient(p)
        Dim p2 As Point

        ' this should be how far the cursor is from the bottom of the button
        Dim nOver As Integer = nHeightTS - p1.Y

        p2.Y = p.Y + nOver
        p2.X = p.X

        Cursor.Position = p2
        oTS.Invalidate()
        Application.DoEvents()
        rButton.Enabled = False
        Cursor.Position = p


    End Sub
#End Region

End Module
