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
' Form/Module: frmMain
'
' Description: File view/Compare MDI parent form
' ` 
'
' Dependancies:  
'
' Language: Microsoft Visual Basic .Net 2008
'
' Author: Speedy
' FANUC Robotics North America
' 3900 W. Hamlin Road
' Rochester Hills, MI.
'
' Modification history:
'
'    Date       By      Reason                                                        Version
' 05/13/2010    MSW     first draft
'********************************************************************************************

Imports System.Windows.Forms

Public Class frmFileViewCompare
    Friend Const msSCREEN_NAME As String = "FileViewCompare"   ' <-- For password area change log etc.
    Friend Const msSCREEN_DUMP_NAME As String = "Utility_FileCopy_FileViewCompare.jpg"
    Private mPrintHtml As clsPrintHtml
    Friend moBaseForm As frmFileViewCompareChild = Nothing
    Friend moBaseTextBox As RichTextBox = Nothing
    Friend moCompForm As frmFileViewCompareChild = Nothing
    Friend moCompTextBox As RichTextBox = Nothing
    Private m_ChildFormNumber As Integer
    Friend mbIgnoreEvents As Boolean = False
    Friend Enum eDiffType
        Different
        ExtraBase
        ExtraCompare
        None
    End Enum

    Friend Property Status(Optional ByVal StatusStripOnly As Boolean = True) As String
        '********************************************************************************************
        'Description:  write status messages to listbox and statusbar
        '
        'Parameters: If StatusbarOnly is true, doesn't write to listbox
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************
        Get
            Return lblStatus.Text
        End Get
        Set(ByVal Value As String)
            lblStatus.Text = Value
        End Set
    End Property

    Private Sub mnuOpenFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuOpenFolder.Click
        '********************************************************************************************
        'Description:  open a folder
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Dim oFB As New FolderBrowserDialog
        Dim sPath As String = String.Empty
        Try
            With oFB
                'No poimnt in making a new folder here
                .ShowNewFolderButton = False
                Dim sPathTmp As String = String.Empty
                If mPWCommon.GetDefaultFilePath(sPathTmp, eDir.PAINTworks, String.Empty, String.Empty) Then
                    .SelectedPath = sPathTmp
                End If
                .Description = gpsRM.GetString("psSELECT_FOLDER")
                .ShowDialog()
                sPath = .SelectedPath
            End With
            Dim oForm As New frmFileViewCompareChild
            oForm.FolderView = True
            oForm.subLoadFile(sPath)
            oForm.FileName = sPath
            oForm.MdiParent = Me
            oForm.Title = sPath
            oForm.Show()
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
        End Try

    End Sub
    Private Sub OpenFile(Optional ByVal sender As Object = Nothing, Optional ByVal e As EventArgs = Nothing) Handles mnuOpen.Click
        '********************************************************************************************
        'Description:  open a file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  

        mbIgnoreEvents = True
        Dim OpenFileDialog As New OpenFileDialog
        OpenFileDialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        OpenFileDialog.Filter = gpsRM.GetString("psOPENSAVEFILEMASK")
        If (OpenFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
            Dim FileName As String = OpenFileDialog.FileName
            Dim oForm As New frmFileViewCompareChild
            oForm.FolderView = False
            oForm.subLoadFile(FileName)
            oForm.FileName = OpenFileDialog.SafeFileName
            oForm.MdiParent = Me
            oForm.Title = FileName
            oForm.Show()
        End If
        mbIgnoreEvents = False
    End Sub
    Private Sub ToolStrip_ItemClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles ToolStrip.ItemClicked
        '********************************************************************************************
        'Description: Tool Bar button clicked - double check privilege here incase it expired
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************

        Select e.ClickedItem.Name
            Case "tlsbOpen"
                Dim o As ToolStripSplitButton = DirectCast(e.ClickedItem, ToolStripSplitButton)
                If o.DropDownButtonPressed Then Exit Sub
                OpenFile()
            Case Else
                '?
        End Select

    End Sub
    Private Sub SaveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSave.Click, tlsbSave.Click
        '********************************************************************************************
        'Description:  save a copy
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        mbIgnoreEvents = True
        Dim SaveFileDialog As New SaveFileDialog
        SaveFileDialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        SaveFileDialog.Filter = gpsRM.GetString("psOPENSAVEFILEMASK")
        Dim oForm As frmFileViewCompareChild = DirectCast(Me.ActiveMdiChild, frmFileViewCompareChild)
        If oForm IsNot Nothing Then
            SaveFileDialog.FileName = oForm.FileName
            If (SaveFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                Dim FileName As String = SaveFileDialog.FileName
                oForm.subSaveFile(FileName)
            End If
        End If
        mbIgnoreEvents = False
    End Sub

    Private Sub ExitToolsStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles mnuExit.Click
        '********************************************************************************************
        'Description:  close the window
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Me.Close()
    End Sub

    Private Sub CopyToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles mnuCopy.Click
        '********************************************************************************************
        'Description:  copy selected text
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Dim oForm As frmFileViewCompareChild = DirectCast(Me.ActiveMdiChild, frmFileViewCompareChild)
        My.Computer.Clipboard.SetText(oForm.TextBox.SelectedText)
    End Sub

    Private Sub ToolBarToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles mnuToolbar.Click
        '********************************************************************************************
        'Description:  menu management
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Me.ToolStrip.Visible = Me.mnuToolbar.Checked
    End Sub

    Private Sub StatusBarToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles mnuStatusBar.Click
        '********************************************************************************************
        'Description:  menu management
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Me.StatusStrip.Visible = Me.mnuStatusBar.Checked
        For Each oForm As frmFileViewCompareChild In Me.MdiChildren
            oForm.stsStatus.Visible = Me.mnuStatusBar.Checked
            oForm.subResizertb()
        Next
    End Sub

    Private Sub CascadeToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles mnuCascade.Click
        '********************************************************************************************
        'Description:  menu management
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Me.LayoutMdi(MdiLayout.Cascade)
    End Sub

    Private Sub TileVerticalToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles mnuTileVert.Click
        '********************************************************************************************
        'Description:  menu management
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Me.LayoutMdi(MdiLayout.TileVertical)
    End Sub

    Private Sub TileHorizontalToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles mnuTileHorz.Click
        '********************************************************************************************
        'Description:  menu management
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Me.LayoutMdi(MdiLayout.TileHorizontal)
    End Sub

    Private Sub ArrangeIconsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles mnuArrangeAll.Click
        '********************************************************************************************
        'Description:  menu management
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Me.LayoutMdi(MdiLayout.ArrangeIcons)
    End Sub

    Private Sub CloseAllToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles mnuCloseAll.Click
        '********************************************************************************************
        'Description:  close all child windows
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        ' Close all child forms of the parent.
        For Each ChildForm As Form In Me.MdiChildren
            ChildForm.Close()
        Next
    End Sub


    Private Sub PrintToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tlsbPrint.Click, mnuPrint.Click
        '********************************************************************************************
        'Description:  print the child window
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        mbIgnoreEvents = True
        Dim oForm As frmFileViewCompareChild = DirectCast(Me.ActiveMdiChild, frmFileViewCompareChild)
        mPrintHtml.subCreateSimpleDoc(oForm.TextBox.Text, Status, oForm.Text)
        mPrintHtml.subPrintDoc()
        mbIgnoreEvents = False
    End Sub

    Private Sub PrintPreviewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintPreview.Click, tlsbPrintPreview.Click
        '********************************************************************************************
        'Description:  print preview of child window
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        mbIgnoreEvents = True
        Dim oForm As frmFileViewCompareChild = DirectCast(Me.ActiveMdiChild, frmFileViewCompareChild)
        mPrintHtml.subCreateSimpleDoc(oForm.TextBox.Text, Status, oForm.Text)
        mPrintHtml.subShowPrintPreview()
        mbIgnoreEvents = False
    End Sub

    Private Sub PrintSetupToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPrintSetup.Click
        '********************************************************************************************
        'Description:  open print setup dialog
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        mPrintHtml.subShowPageSetup()
    End Sub

    Private Sub SelectAllToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSelectAll.Click
        '********************************************************************************************
        'Description:  select all text in the current child window
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        Dim oForm As frmFileViewCompareChild = DirectCast(Me.ActiveMdiChild, frmFileViewCompareChild)
        oForm.TextBox.SelectAll()
    End Sub

    Private Sub subInitFormText()
        '********************************************************************************************
        'Description:  Initialize menu text
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        With gpsRM
            mnuSelectAll.Text = .GetString("psSELECTALL")
            mnuCopy.Text = .GetString("psCOPY")
            mnuToolbar.Text = .GetString("psTOOLBAR")
            mnuStatusBar.Text = .GetString("psSTATUSBAR")
            mnuOpen.Text = .GetString("psOPEN")
            mnuSave.Text = .GetString("psSAVE")
            mnuPrint.Text = .GetString("psPRINTMNU")
            mnuPrintPreview.Text = .GetString("psPRINTPREVIEW")
            mnuPrintSetup.Text = .GetString("psPRINTSETUP")
            mnuExit.Text = .GetString("psEXIT")
            mnuWindows.Text = .GetString("psWINDOWS")
            mnuHelp.Text = .GetString("psHELP")
            mnuView.Text = .GetString("psVIEW")
            mnuCopy.Text = .GetString("psCOPY")
            mnuEdit.Text = .GetString("psEDIT")
            mnuFile.Text = .GetString("psFILE")
            mnuOpen.Text = .GetString("psOPEN")
            mnuHelpScreen.Text = .GetString("psHELPSCREEN")
            mnuCascade.Text = .GetString("psCASCADE")
            mnuTileVert.Text = .GetString("psTILEVERT")
            mnuTileHorz.Text = .GetString("psTILEHORZ")
            mnuCloseAll.Text = .GetString("psCLOSEALL")
            mnuArrangeAll.Text = .GetString("psARRANGEALL")
            mnuCompRunComp.Text = .GetString("psCOMPARE")
            mnuCompSelBase.Text = .GetString("psCOMPSELBASE")
            mnuCompSelComp.Text = .GetString("psCOMPSELCOMP")
            mnuOpenFolder.Text = .GetString("psOPEN_FOLDER")
            Status = String.Empty
            tlsbOpen.ToolTipText = .GetString("psOPEN_TT")
            tlsbSave.ToolTipText = .GetString("psSAVE_TT")
            tlsbPrint.ToolTipText = .GetString("psPRINT_TT")
            tlsbPrintPreview.ToolTipText = .GetString("psPRINTPREVIEW_TT")
            tlsbPrev.ToolTipText = .GetString("psPREV")
            tlsbNext.ToolTipText = .GetString("psNEXT")
            tlsbCompare.ToolTipText = .GetString("psCOMPARE_TT")
            tlsbCompare.Text = .GetString("psCOMPARE_TT")
            tlsbSyncScroll.Text = .GetString("psSYNCSCROLL")
            tlsbSyncScroll.ToolTipText = .GetString("psSYNCSCROLL_TT")
            Me.Icon = CType(gpsRM.GetObject("FormIcon"), Icon)
        End With
        
            mPrintHtml = New clsPrintHtml(msSCREEN_NAME)
    End Sub
    Private Sub frmFileViewCompare_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '********************************************************************************************
        'Description:  Initialize menu text
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        subInitFormText()
    End Sub

    Private Sub mnuHelpScreen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuHelpScreen.Click
        '********************************************************************************************
        'Description:  open help
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        ' Date      By      Reason
        '********************************************************************************************  
        subLaunchHelp(gs_HELP_UTILITIES_FILEVIEWCOMPARE, frmMain.oIPC)
    End Sub
    Private Sub frm_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
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
                    subLaunchHelp(gs_HELP_UTILITIES_FILEVIEWCOMPARE, frmMain.oIPC)

                Case Keys.F2
                    'Screen Dump request
                    Dim oSC As New ScreenShot.ScreenCapture
                    Dim sDumpPath As String = String.Empty

                    Dim sName As String = msSCREEN_DUMP_NAME


                    mPWCommon.GetDefaultFilePath(sDumpPath, eDir.ScreenDumps, String.Empty, String.Empty)
                    oSC.CaptureWindowToFile(Me.Handle, sDumpPath & sName, Imaging.ImageFormat.Jpeg)
                Case Keys.Escape
                    Me.Close()
                Case Else

            End Select
        End If

    End Sub

    Private Function nCompareFolders(ByRef oBaseForm As frmFileViewCompareChild, _
                                       ByRef oCompForm As frmFileViewCompareChild) As Integer
        '********************************************************************************************
        'Description: compare folders
        '
        'Parameters: oBaseForm, oCompForm folder windows
        'Returns:    number of differences
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Try
            'Reset the text color
            Dim oBaseFileList As ListView = oBaseForm.Filelist
            Dim oCompFileList As ListView = oCompForm.Filelist
            oBaseFileList.Sorting = SortOrder.Ascending
            oCompFileList.Sorting = SortOrder.Ascending
            oBaseFileList.Sort()
            oCompFileList.Sort()
            Dim nNumBaseLines As Integer = oBaseFileList.Items.Count
            Dim nNumCompLines As Integer = oCompFileList.Items.Count
            Dim nDiff As Integer = 0
            Dim nBaseline As Integer = 0
            Dim nCompLine As Integer = 0
            Do Until (nBaseline >= nNumBaseLines) Or (nCompLine >= nNumCompLines)
                Application.DoEvents()
                'progress = (nBaseline * 100) \ nNumBaseLines
                Dim sBaseFile As String = oBaseFileList.Items(nBaseline).Text
                Dim sCompFile As String = oCompFileList.Items(nCompLine).Text
                If sBaseFile <> sCompFile Then
                    'This line is different.  
                    nDiff += 1
                    'It's sorted alphabetically, so handle the lower one
                    If sBaseFile < sCompFile Then
                        'sBasefile is first 
                        oBaseFileList.Items(nBaseline).ForeColor = Color.Blue
                        nBaseline += 1
                    Else
                        'sCompfile is first 
                        oCompFileList.Items(nCompLine).ForeColor = Color.Green
                        nCompLine += 1
                    End If
                Else
                    'Same file.  Compare the files 
                    Dim bDiff As Boolean = False
                    Dim oBaseFile As New IO.FileInfo(oBaseForm.FileName & "\" & sBaseFile)
                    Dim oCompFile As New IO.FileInfo(oCompForm.FileName & "\" & sCompFile)
                    If oBaseFile.Length <> oCompFile.Length Then
                        bDiff = True
                    Else
                        'Skip the text compare and go right to binary.  It's faster
                        'Dim sTmp() As String = Split(sBaseFile, ".")
                        'Dim sExt As String = sTmp(sTmp.GetUpperBound(0))
                        'Select Case sExt.ToLower
                        'Case "txt", "tx", "dg", "ls", "va", "dat", "dt"
                        '    Try
                        '        moBaseTextBox = oBaseForm.TextBox
                        '        moCompTextBox = oCompForm.TextBox
                        '        moBaseTextBox.LoadFile(oBaseForm.FileName & "\" & sBaseFile, RichTextBoxStreamType.PlainText)
                        '        moCompTextBox.LoadFile(oCompForm.FileName & "\" & sCompFile, RichTextBoxStreamType.PlainText)
                        '        bDiff = (nCompareTextFiles(moBaseTextBox, moCompTextBox, False) > 0)
                        '    Catch ex As Exception
                        '        'treat like binary if there's an error
                        '        Dim byBase As Byte() = My.Computer.FileSystem.ReadAllBytes(oBaseForm.FileName & "\" & sBaseFile)
                        '        Dim byComp As Byte() = My.Computer.FileSystem.ReadAllBytes(oCompForm.FileName & "\" & sCompFile)
                        '        Dim nMax As Integer = byBase.GetUpperBound(0)
                        '        If nMax <> byComp.GetUpperBound(0) Then
                        '            bDiff = True
                        '        Else
                        '            Dim nIndex As Integer = 0
                        '            Do While nIndex <= nMax And bDiff = False
                        '                bDiff = (byBase(nIndex) <> byComp(nIndex))
                        '                nIndex += 1
                        '            Loop
                        '        End If
                        '    End Try
                        'Case Else
                        Dim byBase As Byte() = My.Computer.FileSystem.ReadAllBytes(oBaseForm.FileName & "\" & sBaseFile)
                        Dim byComp As Byte() = My.Computer.FileSystem.ReadAllBytes(oCompForm.FileName & "\" & sCompFile)
                        Dim nMax As Integer = byBase.GetUpperBound(0)
                        If nMax <> byComp.GetUpperBound(0) Then
                            bDiff = True
                        Else
                            Dim nIndex As Integer = 0
                            Do While nIndex <= nMax And bDiff = False
                                bDiff = (byBase(nIndex) <> byComp(nIndex))
                                nIndex += 1
                            Loop
                        End If
                        'End Select
                    End If
                    If bDiff Then
                        nDiff += 1
                        oBaseFileList.Items(nBaseline).ForeColor = Color.Red
                        oCompFileList.Items(nCompLine).ForeColor = Color.Red
                    Else
                        oBaseFileList.Items(nBaseline).ForeColor = Color.Black
                        oCompFileList.Items(nCompLine).ForeColor = Color.Black
                    End If

                    nBaseline += 1
                    nCompLine += 1
                End If
            Loop
            'Color in the rest if one list is shorter
            Do While (nBaseline < nNumBaseLines)
                nDiff += 1
                oBaseFileList.Items(nBaseline).ForeColor = Color.Blue
                nBaseline += 1
            Loop
            Do While (nCompLine < nNumCompLines)
                nDiff += 1
                oCompFileList.Items(nCompLine).ForeColor = Color.Green
                nCompLine += 1
            Loop
            Return (nDiff)

        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
        End Try
        Return (-1)
    End Function

    Private Function nCompareTextFiles(ByRef oBaseTextBox As RichTextBox, _
                                       ByRef oCompTextBox As RichTextBox, _
                                       ByVal bDetails As Boolean) As Integer
        '********************************************************************************************
        'Description: compare files in richtextboxes
        '
        'Parameters: oBaseTextBox, oCompTextBox text to compare, bDetails - full compare for display
        'Returns:    number of differences
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Try


            'Reset the text color
            If bDetails Then
                oBaseTextBox.SelectAll()
                oBaseTextBox.SelectionColor = Color.Black
                oCompTextBox.SelectAll()
                oCompTextBox.SelectionColor = Color.Black
            End If
            Dim nNumBaseLines As Integer = oBaseTextBox.Lines.GetUpperBound(0)
            Dim nNumCompLines As Integer = oCompTextBox.Lines.GetUpperBound(0)
            Dim nDiff As Integer = 0
            Dim oDiffType As eDiffType = eDiffType.None
            Dim nBaseline As Integer = 0
            Dim nCompLine As Integer = 0
            Do Until (nBaseline >= nNumBaseLines) Or (nCompLine >= nNumCompLines)
                oDiffType = eDiffType.None
                If oBaseTextBox.Lines(nBaseline) <> oCompTextBox.Lines(nCompLine) Then
                    If (bDetails = False) Then
                        Return (1)
                    End If
                    'This line is different.  Determine if it's an extra in 1 file or if they match back up later
                    Dim nDiffSize As Integer = 0
                    Do Until (oDiffType <> eDiffType.None) Or ((nBaseline + nDiffSize) >= nNumBaseLines) Or ((nCompLine + nDiffSize) >= nNumCompLines)
                        nDiffSize += 1
                        If oBaseTextBox.Lines(nBaseline + nDiffSize) = oCompTextBox.Lines(nCompLine + nDiffSize) Then
                            'It matches up again here, get out of the loop and mark this difference
                            oDiffType = eDiffType.Different
                        End If
                        If oBaseTextBox.Lines(nBaseline) = oCompTextBox.Lines(nCompLine + nDiffSize) Then
                            'It matches up again here, get out of the loop and mark this difference
                            oDiffType = eDiffType.ExtraCompare
                        End If
                        If oBaseTextBox.Lines(nBaseline + nDiffSize) = oCompTextBox.Lines(nCompLine) Then
                            'It matches up again here, get out of the loop and mark this difference
                            oDiffType = eDiffType.ExtraBase
                        End If
                    Loop
                    Select Case oDiffType
                        Case eDiffType.Different
                            'Simple difference before it matches back up. mark in red
                            For nOffset As Integer = 0 To nDiffSize - 1
                                oBaseTextBox.Select(oBaseTextBox.GetFirstCharIndexFromLine(nBaseline + nOffset), oBaseTextBox.GetFirstCharIndexFromLine(nBaseline + nOffset + 1) - oBaseTextBox.GetFirstCharIndexFromLine(nBaseline + nOffset))
                                oBaseTextBox.SelectionColor = Color.Red
                                oCompTextBox.Select(oCompTextBox.GetFirstCharIndexFromLine(nCompLine + nOffset), oCompTextBox.GetFirstCharIndexFromLine(nCompLine + nOffset + 1) - oCompTextBox.GetFirstCharIndexFromLine(nCompLine + nOffset))
                                oCompTextBox.SelectionColor = Color.Red
                            Next
                            nDiff += 1
                            nBaseline += nDiffSize
                            nCompLine += nDiffSize
                        Case eDiffType.ExtraBase
                            'Extras in base file. mark in blue
                            For nOffset As Integer = 0 To nDiffSize - 1
                                oBaseTextBox.Select(oBaseTextBox.GetFirstCharIndexFromLine(nBaseline + nOffset), oBaseTextBox.GetFirstCharIndexFromLine(nBaseline + nOffset + 1) - oBaseTextBox.GetFirstCharIndexFromLine(nBaseline + nOffset))
                                oBaseTextBox.SelectionColor = Color.Blue
                            Next
                            nDiff += 1
                            nBaseline += nDiffSize
                        Case eDiffType.ExtraCompare
                            'Extras in compare file. mark in green
                            For nOffset As Integer = 0 To nDiffSize - 1
                                oCompTextBox.Select(oCompTextBox.GetFirstCharIndexFromLine(nCompLine + nOffset), oCompTextBox.GetFirstCharIndexFromLine(nCompLine + nOffset + 1) - oCompTextBox.GetFirstCharIndexFromLine(nCompLine + nOffset))
                                oCompTextBox.SelectionColor = Color.Green
                            Next
                            nDiff += 1
                            nCompLine += nDiffSize
                        Case eDiffType.None
                            'No matches at all, finish it off
                            Do While nBaseline <= nNumBaseLines
                                oBaseTextBox.Select(oBaseTextBox.GetFirstCharIndexFromLine(nBaseline), oBaseTextBox.GetFirstCharIndexFromLine(nBaseline + 1) - oBaseTextBox.GetFirstCharIndexFromLine(nBaseline))
                                oBaseTextBox.SelectionColor = Color.Blue
                                nBaseline += 1
                            Loop
                            Do While nCompLine <= nNumCompLines
                                oCompTextBox.Select(oCompTextBox.GetFirstCharIndexFromLine(nCompLine), oCompTextBox.GetFirstCharIndexFromLine(nCompLine + 1) - oCompTextBox.GetFirstCharIndexFromLine(nCompLine))
                                oCompTextBox.SelectionColor = Color.Green
                                nCompLine += 1
                            Loop
                            nDiff += 1
                    End Select
                Else
                    nBaseline += 1
                    nCompLine += 1
                End If

            Loop
            nBaseline += 1
            nCompLine += 1
            If nBaseline <= nNumBaseLines Then
                oBaseTextBox.Select(oBaseTextBox.GetFirstCharIndexFromLine(nBaseline), oBaseTextBox.TextLength - oBaseTextBox.GetFirstCharIndexFromLine(nBaseline))
                oBaseTextBox.SelectionColor = Color.Blue
                nDiff += 1
            ElseIf nCompLine <= nNumCompLines Then
                oCompTextBox.Select(oCompTextBox.GetFirstCharIndexFromLine(nCompLine), oCompTextBox.TextLength - oCompTextBox.GetFirstCharIndexFromLine(nCompLine))
                oCompTextBox.SelectionColor = Color.Green
                nDiff += 1
            End If
            Return (nDiff)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
        End Try
        Return (-1)
    End Function
    Private Sub mnuCompare_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCompRunComp.Click, tlsbCompare.Click
        '********************************************************************************************
        'Description: Run the compare
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Try
            'First, minimize any other forms and show the compare windows tiled
            If Me.MdiChildren.GetUpperBound(0) <= 0 Then
                'Need at least 2 files for a compare
                Status = gpsRM.GetString("psFVC_NEED_MORE_FILES")
                Exit Sub
            End If
            mbIgnoreEvents = True
            Status = gpsRM.GetString("psFVC_COMPARING_FILES")
            If moBaseForm IsNot Nothing Then
                'If both selected, make sure they're the same type
                If moCompForm IsNot Nothing Then
                    If moCompForm.FolderView <> moBaseForm.FolderView Then
                        moCompForm = Nothing
                    End If
                End If
            Else
                'if compare file is selected w/o base, swqitch it to base
                If moCompForm IsNot Nothing Then
                    moBaseForm = moCompForm
                    moCompForm = Nothing
                End If
            End If
            For Each oForm As frmFileViewCompareChild In Me.MdiChildren
                Select Case oForm.BaseCompareSelect
                    Case frmFileViewCompareChild.eCompareSelect.Compare, frmFileViewCompareChild.eCompareSelect.Base
                        'Show the window
                        oForm.WindowState = FormWindowState.Normal
                    Case frmFileViewCompareChild.eCompareSelect.None
                        'if there's no compare file selection, take the first files available
                        If moBaseForm Is Nothing Then
                            'This one isn't selected and no base file is selected.  Make this the base file.
                            moBaseForm = oForm
                            moBaseForm.BaseCompareSelect = frmFileViewCompareChild.eCompareSelect.Base
                            oForm.WindowState = FormWindowState.Normal
                        ElseIf moCompForm Is Nothing Then
                            'This one isn't selected and no compare file is selected.  Make this the compare file.
                            moCompForm = oForm
                            moCompForm.BaseCompareSelect = frmFileViewCompareChild.eCompareSelect.Compare
                            oForm.WindowState = FormWindowState.Normal
                        Else
                            'This window isn't selected, but base and compare already are.  Minimize it.
                            oForm.WindowState = FormWindowState.Minimized
                        End If
                End Select
            Next

            If Me.MdiChildren.GetUpperBound(0) <= 0 Then
                'Need at least 2 files for a compare
                Status = gpsRM.GetString("psFVC_NEED_MORE_FILES")
                Exit Sub
            End If
            If moBaseForm Is Nothing Or moCompForm Is Nothing Then
                'If both selected, make sure they're the same type
                If moBaseForm IsNot Nothing Then
                    If moBaseForm.FolderView Then
                        Status = gpsRM.GetString("psFVC_NEED_MORE_FOLDERS")
                    Else
                        Status = gpsRM.GetString("psFVC_NEED_MORE_FILES")
                    End If
                ElseIf moCompForm IsNot Nothing Then
                    If moCompForm.FolderView Then
                        Status = gpsRM.GetString("psFVC_NEED_MORE_FOLDERS")
                    Else
                        Status = gpsRM.GetString("psFVC_NEED_MORE_FILES")
                    End If
                Else
                    Status = gpsRM.GetString("psFVC_NEED_MORE_FILES")
                End If

            End If
            'Tile the base and compare files
            Me.LayoutMdi(MdiLayout.TileVertical)
            'Compare
            If moBaseForm.FolderView Then
                'Compare folders
                Dim nDiff As Integer = nCompareFolders(moBaseForm, moCompForm)
            Else
                '
                moBaseTextBox = moBaseForm.TextBox
                moCompTextBox = moCompForm.TextBox
                Dim nDiff As Integer = nCompareTextFiles(moBaseTextBox, moCompTextBox, True)
                If nDiff > 0 Then
                    Status = gpsRM.GetString("psCOMPDIFFSTAT1") & nDiff.ToString & gpsRM.GetString("psCOMPDIFFSTAT2")
                Else
                    Status = gpsRM.GetString("psCOMPDIFFSTAT3")
                End If
            End If
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
            Trace.WriteLine(ex.StackTrace)
            ShowErrorMessagebox(gcsRM.GetString("csERROR"), ex, msSCREEN_NAME, _
                                Status, MessageBoxButtons.OK)
        End Try
        mbIgnoreEvents = False
    End Sub

    Private Sub mnuCompSelBase_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCompSelBase.Click
        '********************************************************************************************
        'Description: select the base compare file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Dim oForm As frmFileViewCompareChild = DirectCast(Me.ActiveMdiChild, frmFileViewCompareChild)
        Select Case oForm.BaseCompareSelect
            Case frmFileViewCompareChild.eCompareSelect.Compare
                'Change base to compare, swap both ways if possible
                If moBaseForm Is Nothing Then
                    moCompForm = Nothing
                Else
                    moCompForm = moBaseForm
                    moCompForm.BaseCompareSelect = frmFileViewCompareChild.eCompareSelect.Compare
                End If
            Case frmFileViewCompareChild.eCompareSelect.Base
                'All set, do nothing
            Case frmFileViewCompareChild.eCompareSelect.None
                'Deselect the old one
                If moBaseForm IsNot Nothing Then
                    moBaseForm.BaseCompareSelect = frmFileViewCompareChild.eCompareSelect.None
                End If
        End Select
        moBaseForm = oForm
        moBaseForm.BaseCompareSelect = frmFileViewCompareChild.eCompareSelect.Base
        If moCompForm IsNot Nothing Then
            If moCompForm.FolderView <> moBaseForm.FolderView Then
                moCompForm = Nothing
            End If
        End If
    End Sub

    Private Sub mnuCompSelComp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCompSelComp.Click
        '********************************************************************************************
        'Description: select the compare file
        '
        'Parameters: none
        'Returns:    none
        '
        'Modification history:
        '
        '    Date       By      Reason
        '********************************************************************************************
        Dim oForm As frmFileViewCompareChild = DirectCast(Me.ActiveMdiChild, frmFileViewCompareChild)
        Select Case oForm.BaseCompareSelect
            Case frmFileViewCompareChild.eCompareSelect.Base
                'Change base to compare, swap both ways if possible
                If moCompForm Is Nothing Then
                    moBaseForm = Nothing
                Else
                    moBaseForm = moCompForm
                    moBaseForm.BaseCompareSelect = frmFileViewCompareChild.eCompareSelect.Base
                End If
            Case frmFileViewCompareChild.eCompareSelect.Compare
                'All set, do nothing
            Case frmFileViewCompareChild.eCompareSelect.None
                'Deselect the old one
                If moCompForm IsNot Nothing Then
                    moCompForm.BaseCompareSelect = frmFileViewCompareChild.eCompareSelect.None
                End If
        End Select
        moCompForm = oForm
        moCompForm.BaseCompareSelect = frmFileViewCompareChild.eCompareSelect.Compare
        If moBaseForm IsNot Nothing Then
            If moBaseForm.FolderView <> moCompForm.FolderView Then
                moBaseForm = Nothing
            End If
        End If

    End Sub

End Class
