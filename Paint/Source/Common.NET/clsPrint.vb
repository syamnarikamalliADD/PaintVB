' This material is the joint property of FANUC Robotics North America and
' FANUC LTD Japan, and must be returned to either FANUC Robotics
' North America or FANUC LTD Japan immediately upon request. This material
' and the information illustrated or contained herein may not be
' reproduced, copied, used, or transmitted in whole or in part in any way
' without the prior written consent of both FANUC Robotics North America and
' FANUC LTD Japan.
'
' All Rights Reserved
' Copyright (C) 2006
' FANUC Robotics North America
' FANUC LTD Japan
'

'Name: clsPrint
'Author: Speedy
'Date: 09/22/99
'
' Dependancies:  Microsoft Word 2003
'
'Description: This module contains the routines to use Microsoft Word to format
'             and print reports. Currently using library 11 (Word 2003)
'
'Note:        This is a temp import for vb.net and requires the interop.word.dll to 
'             be referenced
'
'             File: C:\Program Files\Microsoft Office\OFFICE11\MSWORD.OLB
'
' Modification history:
'
'    Date       By      Reason                                                        Version
'    12/1/11    MSW     Mark a version                                                4.01.01.00
'************************************************************************************


Option Strict Off
Option Explicit On
Option Compare Text

Imports Microsoft.Office.Interop

Friend Class clsPrint


    '************************************************************************************
    ' Declarations
    '************************************************************************************

    'mbDebugMode is true to view output in word, false to send to printer
    Private mbDebugMode As Boolean = False

    ' setup constants
    Private Const sFONT As String = "Times New Roman"
    Private Const sFONT1 As String = "WingDings"

    ' currently word object library 8
    'Private WordApp As New Word.Application
    Private WordApp As Word.Application

    ' working document
    Friend CurDoc As Word.Document
    ' expose the styles to calling application
    Friend DocStyles As Word.Styles

    Friend Enum PrintStyles
        TitleStyle = Word.WdBuiltinStyle.wdStyleHeading1
        SubTitleStyle = Word.WdBuiltinStyle.wdStyleSubtitle
        ColumnHeaderStyle = Word.WdBuiltinStyle.wdStyleBodyText2
        FooterStyle = Word.WdBuiltinStyle.wdStyleFooter
        BodyStyle1 = Word.WdBuiltinStyle.wdStyleBodyText
        BodyStyle2 = Word.WdBuiltinStyle.wdStyleBodyText3 ' not currently used
    End Enum

    'local variable(s) to hold property value(s)
    Private mnOrientation As Short 'local copy
    Private msTitle As String 'local copy
    Private msSubTitles As String 'local copy
    Private msColumnHeadersString As String 'local copy
    Private mvarSpaceBetweenColumns As Single 'local copy
    Private mbAddISOBlurb As Boolean 'local copy
    Private mDataTable As Word.Table
    Private mvarTitleStringForFirstTable As String 'local copy
    Private mvntSeparatorChar As String 'Object

    'collections
    Private colColumnHeaders As New Collection
    Private colBodyTextStrings As New Collection
    Private colBodyText As New Collection ' collection of colBodyTextStrings
    Private colSectionTitleStrings As New Collection

    'table format vars
    Private mFormatName As Word.WdTableFormat
    Private mbApplyboarders As Boolean
    Private mbApplyShading As Boolean
    Private mbApplyFont As Boolean
    Private mbApplyColor As Boolean
    Private mbApplyHeadingRows As Boolean
    Private mbApplyLastRow As Boolean
    Private mbApplyFirstColumn As Boolean
    Private mbApplyLastColumn As Boolean
    Private mbAutoFit As Boolean

    Private mbAbort As Boolean

    ''Private Declare Function GetInputState Lib "user32" () As Integer
    ' ''Private Declare Sub Sleep Lib "kernel32" (ByVal dwMilliseconds As Integer)

    ' the following const is for summary reports to split the top left cell
    ' it must match msCELLSEPARATOR in Localize_Report_Grid.bas in the "Reports" application
    Private Const msCELLSEPARATOR As String = "----"

    Friend Sub Abort()
        '**************************************************************************************************
        ' Description: set the abort flag
        '
        ' Parameters: none
        '
        ' Returns: None
        '**************************************************************************************************
        mbAbort = True
    End Sub

    Friend Sub AddColumnGroupingHeadings(ByVal HeaderTextString As String, ByVal ColumnsPerGroup As Short, Optional ByVal StartColumn As Short = 1, Optional ByVal TableNumber As Short = 2)
        '**************************************************************************************************
        ' Description: Per Dan's request, column grouping headings for comparison screens
        '
        ' Parameters: tab separated string of headers, how many columns per group, column to put the
        '               first header in, and table to use
        '
        ' Returns: None
        ' 01/17/01  gks     Modified to allow more than 1 column group heading.
        '**************************************************************************************************
        Dim CardsOnThe As Word.Table
        Dim sTmp() As String
        Dim inx As Integer
        Dim i As Integer
        Dim nCols As Integer
        Dim nLastColHead As Integer

        If mbAbort Then Exit Sub

        System.Diagnostics.Debug.Assert(Len(HeaderTextString) > 0, "")
        If Len(HeaderTextString) < 1 Then
            Beep()
            Exit Sub
        End If

        'make sure table is there
        If CheckTableForColumns(1, 1, TableNumber) Then
            ' split string
            sTmp = Split(HeaderTextString, vbTab)
            'insert row
            CardsOnThe = CurDoc.Tables.Item(TableNumber)
            With CardsOnThe
                'nCols = .Columns.Count
                .Rows.Add(.Rows.Item(1))
                nCols = .Rows.Item(1).Cells.Count 'gks 01/17/01
                .Rows.Item(1).Borders.Item(Word.WdBorderType.wdBorderBottom).LineStyle = _
                                                Word.WdLineStyle.wdLineStyleNone
                i = 0
                For inx = StartColumn To nCols Step ColumnsPerGroup
                    .Cell(1, inx).Range.Text = sTmp(i)
                    nLastColHead = inx
                    If i = UBound(sTmp) Then Exit For
                    i = i + 1
                Next

                For inx = nLastColHead To StartColumn Step -(ColumnsPerGroup)
                    i = inx + (ColumnsPerGroup - 1)
                    If i > nCols Then i = nCols
                    If i <> inx Then 'gks 01/17/01
                        .Cell(Row:=1, Column:=inx).Merge(MergeTo:=.Cell(Row:=1, Column:=i))
                    End If
                Next
                .Rows.Item(1).Range.Font.Size = .Rows.Item(1).Range.Font.Size + 2
            End With

        End If

    End Sub
    Friend Sub AddColumnGroupingHeadings(ByVal HeaderTextString As String, ByVal ColumnsPerGroup() As Integer, Optional ByVal StartColumn As Short = 1, Optional ByVal TableNumber As Short = 2)
        '**************************************************************************************************
        ' Description: Per Dan's request, column grouping headings for comparison screens
        '
        ' Parameters: tab separated string of headers, how many columns per group, column to put the
        '               first header in, and table to use
        '
        ' Returns: None
        ' 01/17/01  gks     Modified to allow more than 1 column group heading.
        ' 06/12/09  MSW     overload for an array of column group sizes
        '**************************************************************************************************
        Dim CardsOnThe As Word.Table
        Dim sTmp() As String
        Dim inx As Integer
        Dim i As Integer
        Dim nCols As Integer
        Dim nLastColHead As Integer

        If mbAbort Then Exit Sub

        System.Diagnostics.Debug.Assert(Len(HeaderTextString) > 0, "")
        If Len(HeaderTextString) < 1 Then
            Beep()
            Exit Sub
        End If

        'make sure table is there
        If CheckTableForColumns(1, 1, TableNumber) Then
            ' split string
            sTmp = Split(HeaderTextString, vbTab)
            'insert row
            CardsOnThe = CurDoc.Tables.Item(TableNumber)
            With CardsOnThe
                'nCols = .Columns.Count
                .Rows.Add(.Rows.Item(1))
                nCols = .Rows.Item(1).Cells.Count 'gks 01/17/01
                .Rows.Item(1).Borders.Item(Word.WdBorderType.wdBorderBottom).LineStyle = _
                                                Word.WdLineStyle.wdLineStyleNone
                Dim nGrpIdx As Integer = 0
                inx = StartColumn
                Do While (inx <= nCols) And nGrpIdx <= sTmp.GetUpperBound(0)
                    .Cell(1, inx).Range.Text = sTmp(nGrpIdx)
                    nLastColHead = inx
                    inx += ColumnsPerGroup(nGrpIdx)
                    nGrpIdx += 1
                Loop

                nGrpIdx -= 1
                inx = nLastColHead
                Do While (inx >= StartColumn) And nGrpIdx >= 0
                    i = inx + (ColumnsPerGroup(nGrpIdx)) - 1
                    If i > nCols Then i = nCols
                    If i <> inx Then 'gks 01/17/01
                        .Cell(Row:=1, Column:=inx).Merge(MergeTo:=.Cell(Row:=1, Column:=i))
                    End If
                    nGrpIdx -= 1
                    If nGrpIdx >= 0 Then
                        inx -= ColumnsPerGroup(nGrpIdx)
                    End If
                Loop

                .Rows.Item(1).Range.Font.Size = .Rows.Item(1).Range.Font.Size + 2
            End With

        End If

    End Sub
    Friend Sub AddColumnGroupingHeadings(ByVal ColumnsPerGroup() As Integer, Optional ByVal StartColumn As Integer = 1, Optional ByVal TableNumber As Integer = 2, Optional ByVal nRow As Integer = 2)
        '**************************************************************************************************
        ' Description: Per Dan's request, column grouping headings for comparison screens
        '
        ' Parameters: tab separated string of headers, how many columns per group, column to put the
        '               first header in, and table to use
        '
        ' Returns: None
        ' 01/17/01  gks     Modified to allow more than 1 column group heading.
        ' 06/12/09  MSW     overload for an array of column group sizes
        ' 06/12/09  MSW     overload to use the existing first row as the group labels
        '**************************************************************************************************
        Dim CardsOnThe As Word.Table
        Dim inx As Integer
        Dim i As Integer
        Dim nCols As Integer
        Dim nLastColHead As Integer
        If mbAbort Then Exit Sub

        'make sure table is there
        If CheckTableForColumns(1, 1, TableNumber) Then
            ' split string
            'insert row
            CardsOnThe = CurDoc.Tables.Item(TableNumber)
            With CardsOnThe
                'nCols = .Columns.Count
                .Rows.Add(.Rows.Item(1))
                nCols = .Rows.Item(1).Cells.Count 'gks 01/17/01
                .Rows.Item(1).Borders.Item(Word.WdBorderType.wdBorderBottom).LineStyle = _
                                                Word.WdLineStyle.wdLineStyleNone
                Dim nGrpIdx As Integer = 0
                inx = StartColumn
                Do While (inx <= nCols) And nGrpIdx <= ColumnsPerGroup.GetUpperBound(0)
                    .Cell(1, inx).Range.Text = .Cell(nRow + 1, inx).Range.Text
                    nLastColHead = inx
                    inx += ColumnsPerGroup(nGrpIdx)
                    nGrpIdx += 1
                Loop

                nGrpIdx -= 1
                inx = nLastColHead
                nGrpIdx = ColumnsPerGroup.GetUpperBound(0)
                Do While (inx >= StartColumn) And nGrpIdx >= 0
                    i = inx + (ColumnsPerGroup(nGrpIdx)) - 1
                    If i > nCols Then i = nCols
                    If i <> inx Then 'gks 01/17/01
                        .Cell(Row:=1, Column:=inx).Merge(MergeTo:=.Cell(Row:=1, Column:=i))
                    End If
                    nGrpIdx -= 1
                    If nGrpIdx >= 0 Then
                        inx -= ColumnsPerGroup(nGrpIdx)
                    End If
                Loop

                .Rows.Item(1).Range.Font.Size = .Rows.Item(1).Range.Font.Size + 2
                .Rows.Item(2).Range.Font.Size = .Rows.Item(2).Range.Font.Size - 1
                .Rows.Item(nRow + 1).Delete()
            End With

        End If

    End Sub

    Friend Sub ApplyBanding(ByVal Every_nn_Rows As Short)
        '**************************************************************************************************
        ' Description: Applies banding to tables
        '
        ' Parameters: banding frequency
        '
        ' Returns: None
        '**************************************************************************************************
        Dim nTables As Integer
        Dim nTable As Integer
        Dim lRows As Integer
        Dim lRow As Integer

        If mbAbort Then Exit Sub

        With CurDoc.Tables
            nTables = .Count
            If nTables < 2 Then Exit Sub

            For nTable = 2 To nTables
                With .Item(nTable)
                    lRows = .Rows.Count

                    'skip first row
                    For lRow = (Every_nn_Rows + 1) To lRows Step Every_nn_Rows
                        If mbAbort Then Exit Sub
                        If (lRow Mod ((Every_nn_Rows + 1) * 50)) = 0 Then System.Windows.Forms.Application.DoEvents()
                        With .Rows.Item(lRow).Shading
                            .Texture = Word.WdTextureIndex.wdTexture5Percent
                        End With
                    Next
                End With
            Next

        End With

    End Sub

    Friend Sub ConvertToCheckboxes(ByVal StartColumn As Short, ByVal EndColumn As Short, Optional ByVal TableNum As Short = 2)
        '**************************************************************************************************
        ' Description: Applies WingDings font to selected columns starting at row 2
        '
        ' Parameters: the start and end columns to apply formatting to. default to first data table
        '
        ' Returns: None
        '**************************************************************************************************
        Dim MyTable As Word.Table
        Dim nCol As Short
        Dim lRow As Integer
        Dim lLastRow As Integer

        If (CheckTableForColumns(StartColumn, EndColumn, TableNum)) Then

            MyTable = CurDoc.Tables.Item(TableNum)

            With MyTable
                lLastRow = .Rows.Count

                For nCol = StartColumn To EndColumn
                    For lRow = 2 To lLastRow
                        If mbAbort Then Exit Sub
                        If (lRow Mod 50) = 0 Then System.Windows.Forms.Application.DoEvents()
                        .Cell(lRow, nCol).Range.Style = PrintStyles.BodyStyle2
                        .Cell(lRow, nCol).Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight
                    Next
                Next
            End With
        End If

    End Sub
    Friend Sub StandHeaderTextOnEndAll(Optional ByVal StartColumn As Integer = 0, _
                                       Optional ByVal EndColumn As Integer = 0, _
                                       Optional ByVal bTightFit As Boolean = False)
        For nTable As Integer = 2 To CurDoc.Tables.Count
            StandHeaderTextOnEnd(StartColumn, EndColumn, nTable, bTightFit)
        Next
    End Sub

    Friend Sub StandHeaderTextOnEnd(Optional ByVal StartColumn As Integer = 0, _
                Optional ByVal EndColumn As Integer = 0, Optional ByVal TableNum As Integer = 2, _
                Optional ByVal bTightFit As Boolean = False)
        '**************************************************************************************************
        ' Description: Used to print robot names sideways for checkbox columns
        '
        ' Parameters: the start and end columns to apply formatting to. default to first data table
        '
        ' Returns: None
        '09/27/02   gks     Massaged routine
        '**************************************************************************************************
        Dim MyTable As Word.Table
        Dim nCol As Integer
        Dim nStart As Integer
        Dim nEnd As Integer
        Dim fWidth As Single
        Dim oSel As Word.Selection

        If mbAbort Then Exit Sub

        On Error GoTo ErrHere

        fWidth = 0
        ' if not set, set to defaults
        If StartColumn = 1 Then StartColumn = 1
        If EndColumn = 0 Then EndColumn = CurDoc.Tables.Item(TableNum).Columns.Count

        If (CheckTableForColumns(StartColumn, EndColumn, TableNum)) Then

            MyTable = CurDoc.Tables.Item(TableNum)

            nStart = 1
            nEnd = MyTable.Columns.Count

            With MyTable
                For nCol = nStart To nEnd
                    If (nCol < StartColumn) Or (nCol > EndColumn) Then
                        .Cell(1, nCol).VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalBottom
                    Else
                        If .Cell(1, nCol).Width > fWidth Then fWidth = .Cell(1, nCol).Width
                        .Cell(1, nCol).Range.Orientation = Word.WdTextOrientation.wdTextOrientationUpward
                        .Cell(1, nCol).Range.ParagraphFormat.SpaceBefore = 0
                        .Cell(1, nCol).Range.ParagraphFormat.SpaceAfter = 0
                        .Columns.Item(nCol).Select()
                        .Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
                    End If
                Next
                'reapply formatting
                .UpdateAutoFormat()
                If .Rows.Item(1).Height < fWidth Then .Rows.Item(1).Height = fWidth
                If bTightFit Then
                    .Rows.Item(1).Height = .Rows.Item(1).Height * 2
                    .AllowAutoFit = True
                Else
                    ' add a little fudge
                    .Rows.Item(1).Height = .Rows.Item(1).Height + 5
                End If
                .UpdateAutoFormat()
                .Rows.Item(1).Range.Bold = True
            End With
        End If


        Exit Sub
ErrHere:
        Err.Clear()
    End Sub

    Friend Sub StandBodyTextOnEnd(ByVal Row As Integer, Optional ByVal StartColumn As Integer = 0, Optional ByVal EndColumn As Integer = 0, Optional ByVal TableNum As Integer = 2)
        '**************************************************************************************************
        ' Description: Used to print robot names sideways for checkbox columns
        '
        ' Parameters: the start and end columns to apply formatting to. default to first data table
        '
        ' Returns: None
        '09/27/02   gks     Massaged routine
        '**************************************************************************************************
        Dim MyTable As Word.Table
        Dim nCol As Integer
        Dim nStart As Integer
        Dim nEnd As Integer
        Dim fWidth As Single
        Dim oSel As Word.Selection

        If mbAbort Then Exit Sub

        On Error GoTo ErrHere

        fWidth = 0
        ' if not set, set to defaults
        If StartColumn = 1 Then StartColumn = 1
        If EndColumn = 0 Then EndColumn = CurDoc.Tables.Item(TableNum).Columns.Count

        If (CheckTableForColumns(StartColumn, EndColumn, TableNum)) Then

            MyTable = CurDoc.Tables.Item(TableNum)

            nStart = 1
            nEnd = MyTable.Columns.Count

            With MyTable
                For nCol = nStart To nEnd
                    If (nCol < StartColumn) Or (nCol > EndColumn) Then
                        .Cell(Row, nCol).VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalBottom
                    Else
                        If .Cell(Row, nCol).Width > fWidth Then fWidth = .Cell(1, nCol).Width
                        .Cell(Row, nCol).Range.Orientation = Word.WdTextOrientation.wdTextOrientationUpward
                        .Cell(Row, nCol).Range.ParagraphFormat.SpaceBefore = 0
                        .Cell(Row, nCol).Range.ParagraphFormat.SpaceAfter = 0
                    End If
                Next
                'reapply formatting
                .UpdateAutoFormat()
                If .Rows.Item(Row).Height < fWidth Then .Rows.Item(Row).Height = fWidth
                ' add a little fudge
                .Rows.Item(Row).Height = .Rows.Item(Row).Height + 5
            End With
        End If


        Exit Sub
ErrHere:
        Err.Clear()
    End Sub

    Friend Sub JustifyColumn(ByVal ColumnIndex As Short, ByVal JustifyType As Word.WdParagraphAlignment, Optional ByVal TableNumber As Short = 2)
        '**************************************************************************************************
        ' Description: enables user to right, center or left justify a column in a table. the first table in
        '               the document is the subtitles, data tables start at 2 and go up
        '
        ' Parameters: column to use, right left or center, which table - if 2 section report, section 2 table
        '               would be #3
        '
        ' Returns: None
        '**************************************************************************************************

        If mbAbort Then Exit Sub

        If (JustifyType < 0) Or (JustifyType > 3) Then
            Beep()
            System.Diagnostics.Debug.Assert(((JustifyType >= 0) And (JustifyType <= 3)), "")
            Exit Sub
        End If


        If (CheckTableForColumns(ColumnIndex, ColumnIndex, TableNumber)) Then
            Call subJustifyColumns(CurDoc.Tables.Item(TableNumber), ColumnIndex, JustifyType)
        End If

    End Sub

    Friend Sub SetRowHeight(ByVal vPoints As Single, Optional ByVal vAdjustHeaderRow As Boolean = False, Optional ByVal vTableNumber As Short = 2)
        '**************************************************************************************************
        ' Description: enables user to set the row height for a table, data tables start at 2 and go up
        '
        ' Parameters: height in points, should be larger than fontsize. will not adjust header row unless
        '           vAdjustHeaderRow is set to true. table to adjust
        '
        ' Returns: None
        '**************************************************************************************************

        Dim CardsOnThe As Word.Table
        Dim lRows As Integer
        'Dim JennAir As Word.Range
        Dim Corn As Word.Row

        'make sure table is there
        If CheckTableForColumns(1, 1, vTableNumber) Then
            CardsOnThe = CurDoc.Tables.Item(vTableNumber)
            If vAdjustHeaderRow Then
                Call CardsOnThe.Rows.SetHeight(vPoints, Word.WdRowHeightRule.wdRowHeightExactly)
            Else
                For lRows = 2 To CardsOnThe.Rows.Count
                    If mbAbort Then Exit Sub

                    With CardsOnThe.Rows.Item(lRows)
                        Corn = CardsOnThe.Rows.Item(lRows)
                        Corn.Select()
                        Corn.Range.ParagraphFormat.SpaceAfter = 0
                        Corn.Range.ParagraphFormat.SpaceBefore = 0

                        Corn.SetHeight(vPoints, Word.WdRowHeightRule.wdRowHeightExactly)
                    End With

                Next
            End If
        End If

    End Sub


    Friend Sub SetTableFormat(ByRef FormatName As Word.WdTableFormat, Optional ByVal ApplyBorders As Boolean = True, Optional ByVal ApplyShading As Boolean = False, Optional ByVal ApplyFont As Boolean = False, Optional ByVal ApplyColor As Boolean = False, Optional ByVal ApplyHeadingRows As Boolean = True, Optional ByVal ApplyLastRow As Boolean = False, Optional ByVal ApplyFirstColumn As Boolean = False, Optional ByVal ApplyLastColumn As Boolean = False, Optional ByVal AutoFit As Boolean = True)
        '**************************************************************************************************
        ' Description: exposes the format of the table applied to data to the calling program if user
        '               wishes to change the look of the printout
        '
        ' Parameters: corrispond to the word autoformat method
        '
        ' Returns: None
        '**************************************************************************************************

        mFormatName = FormatName
        mbApplyboarders = ApplyBorders
        mbApplyShading = ApplyShading
        mbApplyFont = ApplyFont
        mbApplyColor = ApplyColor
        mbApplyHeadingRows = ApplyHeadingRows
        mbApplyLastRow = ApplyLastRow
        mbApplyFirstColumn = ApplyFirstColumn
        mbApplyLastColumn = ApplyLastColumn
        mbAutoFit = AutoFit

    End Sub
    Friend Sub StartFirstSection(Optional ByVal vTitleString As String = "")
        '**************************************************************************************************
        ' Description: called to add strings to top level collections, called once internally, can be
        '           called from print routine to have more than one section
        '
        ' Parameters: vTitleString is used to give the section a title
        '
        ' Returns: None
        '**************************************************************************************************
        ' this is the title for the new section title so key is nkey+1
        colSectionTitleStrings.Add(vTitleString, "X0")
    End Sub
    Friend Sub StartNewSection(Optional ByVal vTitleString As String = "")
        '**************************************************************************************************
        ' Description: called to add strings to top level collections, called once internally, can be
        '           called from print routine to have more than one section
        '
        ' Parameters: vTitleString is used to give the section a title
        '
        ' Returns: None
        '**************************************************************************************************
        Static nKey As Integer

        If mbAbort Then Exit Sub

        ' this is the title for the new section title so key is nkey+1
        colSectionTitleStrings.Add(vTitleString, "X" & nKey + 1)

        ' this is adding data from the old section so key is nkey
        colColumnHeaders.Add(msColumnHeadersString, "X" & nKey)
        msColumnHeadersString = ""

        colBodyText.Add(colBodyTextStrings, "X" & nKey)
        colBodyTextStrings = New Collection

        nKey = nKey + 1

    End Sub

    Friend Function BuildDocument(Optional ByVal bOneTablePerPage As Boolean = False) As Boolean
        '**************************************************************************************************
        ' Description: After all data is loaded into collections, this sends it to the word document
        '
        ' Parameters: None
        '
        ' Returns: True if built
        ' 10/15/99  gks     Added page break & first table header
        ' 11/16/99  mp      Added code to apply the column header style
        ' 08/10/10  msw     I don't why, but sometimes CC tables hang up without this
        '**************************************************************************************************
        Dim lRow As Integer
        Dim HomeOnThe As Word.Range
        Dim MyTable As Word.Table
        Dim nSectionCount As Integer
        Dim tmpStrings As New Collection
        Dim rTmp As Word.Range
        Dim fForceNewPage As Single

        Const nPAGE_BREAK_FUDGE_FACTOR As Short = 200 ' use to adjust amount of page left before forcing

        If mbAbort Then Exit Function

        BuildDocument = True

        On Error GoTo BuildErr

        If mbDebugMode Then
            WordApp.Visible = True
        End If

        CurDoc.PageSetup.Orientation = mnOrientation
        Call subDoMargins()

        Call subDoHeader()

        HomeOnThe = CurDoc.Sections.Item(1).Range

        ' add the subtitles
        Call subDoSubTitles(HomeOnThe)

        ' add most recent section to top level collections
        ' not to be confused with a document.section
        Call StartNewSection()

        '  body of report
        With HomeOnThe
            ' find the top of footer
            fForceNewPage = (WordApp.ActiveDocument.PageSetup.PageHeight - .PageSetup.FooterDistance)

            'optional 1st table heading
            If Len(mvarTitleStringForFirstTable) > 0 Then
                .Style = CurDoc.Styles.Item(PrintStyles.SubTitleStyle)
                .InsertAfter(mvarTitleStringForFirstTable)
                .InsertParagraphAfter()
                .MoveEnd()
                .Collapse(Direction:=Word.WdCollapseDirection.wdCollapseEnd)
            End If

            For nSectionCount = 0 To (colBodyText.Count() - 1)
                'put space between sections
                If nSectionCount > 0 Then
                    .MoveEnd()
                    .InsertParagraphAfter()
                    .Collapse(Direction:=Word.WdCollapseDirection.wdCollapseEnd)
                    .MoveEnd()
                    .Collapse(Direction:=Word.WdCollapseDirection.wdCollapseEnd)
                    'see if we should force a page break
                    If bOneTablePerPage Then
                        .InsertBreak((Word.WdBreakType.wdPageBreak))
                    Else

                        If (fForceNewPage - .Information(Word.WdInformation.wdVerticalPositionRelativeToPage)) < nPAGE_BREAK_FUDGE_FACTOR Then
                            CurDoc.Repaginate()
                            System.Windows.Forms.Application.DoEvents()
                            'check it again
                            If (fForceNewPage - .Information(Word.WdInformation.wdVerticalPositionRelativeToPage)) < nPAGE_BREAK_FUDGE_FACTOR Then
                                .InsertBreak((Word.WdBreakType.wdPageBreak))
                            End If
                        End If
                    End If
                End If
                'MSW 6/15/09 'moved section title out of "If nSectionCount > 0 Then" so section 1 can have a title, too.
                .Style = CurDoc.Styles.Item(PrintStyles.SubTitleStyle)
                ' add section title if there is one
                If colSectionTitleStrings.Contains("X" & nSectionCount) Then
                    If Len(colSectionTitleStrings.Item("X" & nSectionCount)) > 0 Then
                        .InsertAfter(colSectionTitleStrings.Item("X" & nSectionCount))
                        .InsertParagraphAfter()
                        .MoveEnd()
                        .Collapse(Direction:=Word.WdCollapseDirection.wdCollapseEnd)
                    End If
                End If
                'add the column headers - should be one for each column
                'use empty string "" if necessary for empty cells
                '.Style = CurDoc.Styles(PrintStyles.BodyStyle1) '09/27/02 gks
                .Style = CurDoc.Styles.Item(PrintStyles.ColumnHeaderStyle) '09/27/02 gks
                ' ("X" & nSectionCount) is collection key
                If Len(colColumnHeaders.Item("X" & nSectionCount)) > 0 Then
                    .InsertAfter(colColumnHeaders.Item("X" & nSectionCount))
                    .InsertParagraphAfter()
                End If
                .Style = CurDoc.Styles.Item(PrintStyles.BodyStyle1) '09/27/02 gks
                'get a collection from the collection of collections
                tmpStrings = colBodyText.Item("X" & nSectionCount)
                For lRow = 1 To tmpStrings.Count()
                    If mbAbort Then Exit Function
                    If (lRow Mod 50) = 0 Then System.Windows.Forms.Application.DoEvents()
                    .InsertAfter(tmpStrings.Item(lRow).ToString)
                    .InsertParagraphAfter()
                Next
                'max 63 columns !
                '.ConvertToTable Separator:=wdSeparateByTabs
                If tmpStrings.Count() > 0 Then 'MSW 8/10/10 - I don't why, but sometimes CC tables hang up without this
                    .ConvertToTable(Separator:=ColumnSeparatorCharacter)
                End If

                MyTable = CurDoc.Tables.Item(CurDoc.Tables.Count)

                With MyTable
                    .Borders.Enable = False
                    ' row 1 is columnheaders, they appear at top of each page
                    .Rows.Item(1).HeadingFormat = True
                    .Rows.Item(1).Range.Style = CurDoc.Styles.Item(PrintStyles.ColumnHeaderStyle).NameLocal
                    .Rows.AllowBreakAcrossPages = False
                    .Range.Collapse()
                    .Rows.SpaceBetweenColumns = mvarSpaceBetweenColumns
                    ' line edge of table up with the subtitles
                    .Rows.SetLeftIndent(WordApp.InchesToPoints(0.1), Word.WdRulerStyle.wdAdjustNone)
                    .Rows.Item(1).Range.Bold = True

                    .AutoFormat(Format:=mFormatName, ApplyBorders:=mbApplyboarders, ApplyShading:=mbApplyShading, ApplyFont:=mbApplyFont, ApplyColor:=mbApplyColor, ApplyHeadingRows:=mbApplyHeadingRows, ApplyLastRow:=mbApplyLastRow, ApplyFirstColumn:=mbApplyFirstColumn, ApplyLastColumn:=mbApplyLastColumn, AutoFit:=mbAutoFit)

                    If mbAbort Then Exit Function

                    ' this is a special case for summary reports
                    rTmp = .Cell(1, 1).Range
                    With rTmp.Find
                        .ClearFormatting()
                        .Text = msCELLSEPARATOR
                        With .Replacement
                            .ClearFormatting()
                            .Text = vbCr
                        End With
                        .Execute(Replace:=Word.WdReplace.wdReplaceAll, Format:=True, MatchCase:=True, MatchWholeWord:=True)
                    End With
                End With

                .Collapse(Direction:=Word.WdCollapseDirection.wdCollapseEnd)

            Next  ' nSectionCount
        End With

        Call subDoFooter()

        Exit Function

BuildErr:
        ''''Dim lErrNum As Integer
        ''''Dim sErrDesc As String

        BuildDocument = False

        ''''lErrNum = Err.Number
        ''''sErrDesc = Err.Description

        '' '' '' ''UPGRADE_WARNING: App property App.EXEName has a new behavior. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"'
        ' '' '' ''Call LogInternalError(My.Application.Info.AssemblyName, "clsPrint", "BuildDocument", lErrNum, sErrDesc)

        '' ''MessageBox.Show(VB6.GetHInstance.ToInt32, "Error " & lErrNum & vbCr & sErrDesc, "Failed to format document", MsgBoxStyle.Information)

        Err.Clear()

    End Function

    Friend Function SaveDocument(ByRef sFilename As String) As Boolean
        '**************************************************************************************************
        ' Description: saves document
        '
        ' Parameters: None
        '
        ' Returns: false if error
        '**************************************************************************************************

        On Error GoTo SaveErr

        CurDoc.SaveAs(sFilename)

SaveExit:
        On Error Resume Next
        CurDoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges)
        System.Windows.Forms.Application.DoEvents()
        'UPGRADE_NOTE: Object CurDoc may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        CurDoc = Nothing
        On Error GoTo 0

        Exit Function

SaveErr:
        SaveDocument = False
        GoTo SaveExit

    End Function
    Friend Function SendToPrinter(ByVal sPrintername As String) As Boolean
        '**************************************************************************************************
        ' Description: sends document to printer
        '
        ' Parameters: None
        '
        ' Returns: false if error
        '**************************************************************************************************
        Dim i As Integer

        On Error GoTo PrinterErr

        If mbAbort Then GoTo PrinterErr

        SendToPrinter = True

        WordApp.Options.PrintBackground = True
        WordApp.ActivePrinter = sPrintername

        If mbDebugMode Then
            Stop
        Else
            i = 0
            CurDoc.PrintOut()
            Do While (WordApp.BackgroundPrintingStatus > 0)
                System.Threading.Thread.Sleep(100)
                'If GetInputState Or (i = 50) Then
                If (i = 50) Then
                    System.Windows.Forms.Application.DoEvents()
                    i = 0
                End If

                i = i + 1
            Loop
        End If

PrinterExit:
        On Error Resume Next
        CurDoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges)
        System.Windows.Forms.Application.DoEvents()
        'UPGRADE_NOTE: Object CurDoc may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        'CurDoc = Nothing

        On Error GoTo 0

        Exit Function

PrinterErr:
        SendToPrinter = False
        GoTo PrinterExit

    End Function

    Friend Function SendToPrinter() As Boolean
        '**************************************************************************************************
        ' Description: sends document to printer
        '
        ' Parameters: None
        '
        ' Returns: false if error
        '**************************************************************************************************
        Dim i As Integer

        On Error GoTo PrinterErr

        If mbAbort Then GoTo PrinterErr

        SendToPrinter = True

        WordApp.Options.PrintBackground = True

        If mbDebugMode Then
            System.Windows.Forms.Application.DoEvents()
            Stop
        Else
            i = 0
            CurDoc.PrintOut()
            Do While (WordApp.BackgroundPrintingStatus > 0)
                System.Threading.Thread.Sleep(100)
                'If GetInputState Or (i = 50) Then
                If (i = 50) Then
                    System.Windows.Forms.Application.DoEvents()
                    i = 0
                End If

                i = i + 1
            Loop
        End If

PrinterExit:
        On Error Resume Next
        CurDoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges)
        System.Windows.Forms.Application.DoEvents()
        CurDoc = Nothing
        WordApp.Quit()
        WordApp = Nothing
        On Error GoTo 0

        Exit Function

PrinterErr:
        SendToPrinter = False
        GoTo PrinterExit

    End Function

    Friend Sub Initialize()
        '**************************************************************************************************
        ' Description: Calling this fires the class initialize event to get the ball rolling
        '               this routine intentionally blank
        ' Parameters: None
        '
        ' Returns: None
        '**************************************************************************************************

    End Sub

    Private Sub subInitDefaultData()
        'Dim ZoneInfo As Object
        'Dim PWInfo As Object
        'Dim pw3api As Object
        '**************************************************************************************************
        ' Description: Called when class is initialized, sets defaults
        '
        ' Parameters: None
        '
        ' Returns: None
        '
        ' Revisions:
        ' 11/16/99    mp    Initialized 'SpaceBefore' and 'SpaceAfter' properties for
        '                   BodyStyle1 and BodyStyle2.  Also added definition for the
        '                   ColumnHeaderStyle style
        ' 02/09/00    gks   Added options initialize
        ' 01/10/01    gks   Added  pw3api.InIDE
        ' 06/20/02    rjo   Added Sleep (1000) after creating word object to allow Word XP enough time to
        '                   start up.
        '**************************************************************************************************

        ' ''UPGRADE_WARNING: Couldn't resolve default property of object pw3api.InIDE. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'

        'temp move to def at top of class
        ''If pw3api.InIDE Then
        ''mbDebugMode = True
        ''Else
        ''	mbDebugMode = False
        ''End If


        ' start word
        WordApp = CreateObject("Word.application")
        Threading.Thread.Sleep((1000))
        CurDoc = WordApp.Documents.Add
        System.Windows.Forms.Application.DoEvents()
        DocStyles = CurDoc.Styles

        System.Windows.Forms.Application.DoEvents()

        ' initialize word

        On Error Resume Next
        With WordApp
            .DefaultTableSeparator = vbTab
            ColumnSeparatorCharacter = vbTab
            .DisplayAutoCompleteTips = False
            .DisplayRecentFiles = False
            .DisplayStatusBar = False
            .Visible = mbDebugMode
            'UPGRADE_WARNING: Couldn't resolve default property of object PWInfo.SiteName. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ''.UserName = PWInfo.SiteName
            .UserInitials = ""
            'UPGRADE_WARNING: Couldn't resolve default property of object ZoneInfo.ZoneName. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ''.UserAddress = ZoneInfo.ZoneName

            'Debug.Assert Val(.Build) = 8
            ' this was written with word 97. if you are using a different version, you will need to
            'double check everything...

            With .ActiveWindow
                .DisplayHorizontalScrollBar = mbDebugMode
                .DisplayVerticalScrollBar = mbDebugMode
                .DisplayVerticalRuler = False
                .DisplayScreenTips = False
                .DisplayRulers = False

                With .View
                    .ShowAnimation = False
                    .ShowPicturePlaceHolders = False
                    .ShowFieldCodes = False
                    .ShowBookmarks = False
                    .FieldShading = Word.WdFieldShading.wdFieldShadingWhenSelected
                    .ShowTabs = False
                    .ShowSpaces = False
                    .ShowParagraphs = False
                    .ShowHyphens = False
                    .ShowHiddenText = False
                    .ShowAll = False
                    .ShowDrawings = True
                    .ShowObjectAnchors = False
                    .ShowTextBoundaries = False
                    .ShowHighlight = False
                End With
            End With

            With .Options
                .AllowAccentedUppercase = False
                .AllowDragAndDrop = False
                .AllowFastSave = False
                .AnimateScreenMovements = False
                .AutoWordSelection = True
                .AutoFormatApplyBulletedLists = False
                .AutoFormatApplyHeadings = False
                .AutoFormatApplyLists = False
                .AutoFormatApplyOtherParas = False
                .AutoFormatAsYouTypeApplyBorders = False
                .AutoFormatAsYouTypeApplyBulletedLists = False
                .AutoFormatAsYouTypeApplyHeadings = False
                .AutoFormatAsYouTypeApplyNumberedLists = False
                .AutoFormatAsYouTypeApplyTables = False
                .AutoFormatAsYouTypeDefineStyles = False
                .AutoFormatAsYouTypeFormatListItemBeginning = False
                .AutoFormatAsYouTypeReplaceFractions = False
                .AutoFormatAsYouTypeReplaceHyperlinks = False
                .AutoFormatAsYouTypeReplaceOrdinals = False
                .AutoFormatAsYouTypeReplacePlainTextEmphasis = False
                .AutoFormatAsYouTypeReplaceQuotes = False
                .AutoFormatAsYouTypeReplaceSymbols = False
                '.BlueScreen = False
                .BackgroundSave = False
                .CheckSpellingAsYouType = False
                .CheckGrammarAsYouType = False
                .CheckGrammarWithSpelling = False
                .ConfirmConversions = False
                .CreateBackup = False
                .DefaultTray = "Use printer settings"
                .DeletedTextMark = Word.WdDeletedTextMark.wdDeletedTextMarkStrikeThrough
                .DeletedTextColor = Word.WdColorIndex.wdByAuthor
                .EnableSound = False
                .IgnoreUppercase = False
                .IgnoreMixedDigits = False
                .IgnoreInternetAndFileAddresses = False
                .InsertedTextMark = Word.WdInsertedTextMark.wdInsertedTextMarkUnderline
                .InsertedTextColor = Word.WdColorIndex.wdByAuthor
                .INSKeyForPaste = False
                .MapPaperSize = True
                .MeasurementUnit = Word.WdMeasurementUnits.wdInches
                .Overtype = False
                'if you let word repageinate in the background you may end up
                'with blank pages. Pagination is forced in builddocument
                .Pagination = False
                .PictureEditor = "Microsoft Word"
                .PrintBackground = True
                .PrintProperties = False
                .PrintFieldCodes = False
                .PrintComments = False
                .PrintHiddenText = False
                .PrintDrawingObjects = True
                .PrintDraft = False
                .PrintReverse = False
                .ReplaceSelection = True
                .RevisedPropertiesMark = Word.WdRevisedPropertiesMark.wdRevisedPropertiesMarkNone
                .RevisedPropertiesColor = Word.WdColorIndex.wdAuto
                .RevisedLinesMark = Word.WdRevisedLinesMark.wdRevisedLinesMarkOutsideBorder
                .RevisedLinesColor = Word.WdColorIndex.wdAuto
                .RTFInClipboard = True
                .SavePropertiesPrompt = False
                .SaveInterval = 10
                .SaveNormalPrompt = False
                .SendMailAttach = False
                .ShowReadabilityStatistics = False
                .ShortMenuNames = False
                .SmartCutPaste = False
                .SuggestSpellingCorrections = False
                .SuggestFromMainDictionaryOnly = False
                .TabIndentKey = False
                .UpdateFieldsAtPrint = False
                .UpdateLinksAtPrint = False
                .UpdateLinksAtOpen = False
                .VirusProtection = False
                .WPHelp = False
                .WPDocNavKeys = False
            End With
        End With

        With CurDoc
            .Compatibility(Word.WdCompatibility.wdNoTabHangIndent) = False
            .Compatibility(Word.WdCompatibility.wdNoSpaceRaiseLower) = False
            .Compatibility(Word.WdCompatibility.wdPrintColBlack) = True
            .Compatibility(Word.WdCompatibility.wdWrapTrailSpaces) = False
            .Compatibility(Word.WdCompatibility.wdNoColumnBalance) = False
            .Compatibility(Word.WdCompatibility.wdConvMailMergeEsc) = False
            .Compatibility(Word.WdCompatibility.wdSuppressSpBfAfterPgBrk) = False
            .Compatibility(Word.WdCompatibility.wdSuppressTopSpacing) = False
            .Compatibility(Word.WdCompatibility.wdOrigWordTableRules) = False
            .Compatibility(Word.WdCompatibility.wdTransparentMetafiles) = False
            .Compatibility(Word.WdCompatibility.wdShowBreaksInFrames) = False
            .Compatibility(Word.WdCompatibility.wdSwapBordersFacingPages) = False
            .Compatibility(Word.WdCompatibility.wdLeaveBackslashAlone) = False
            .Compatibility(Word.WdCompatibility.wdExpandShiftReturn) = False
            .Compatibility(Word.WdCompatibility.wdDontULTrailSpace) = False
            .Compatibility(Word.WdCompatibility.wdDontBalanceSingleByteDoubleByteWidth) = False
            .Compatibility(Word.WdCompatibility.wdSuppressTopSpacingMac5) = False
            .Compatibility(Word.WdCompatibility.wdSpacingInWholePoints) = False
            .Compatibility(Word.WdCompatibility.wdPrintBodyTextBeforeHeader) = False
            .Compatibility(Word.WdCompatibility.wdNoLeading) = False
            .Compatibility(Word.WdCompatibility.wdNoSpaceForUL) = False
            .Compatibility(Word.WdCompatibility.wdMWSmallCaps) = False
            .Compatibility(Word.WdCompatibility.wdNoExtraLineSpacing) = False
            .Compatibility(Word.WdCompatibility.wdTruncateFontHeight) = False
            .Compatibility(Word.WdCompatibility.wdUsePrinterMetrics) = True
            .Compatibility(Word.WdCompatibility.wdSubFontBySize) = False
            .Compatibility(Word.WdCompatibility.wdWW6BorderRules) = False
            .Compatibility(Word.WdCompatibility.wdExactOnTop) = False
            .Compatibility(Word.WdCompatibility.wdSuppressBottomSpacing) = False
            .Compatibility(Word.WdCompatibility.wdWPSpaceWidth) = False
            .Compatibility(Word.WdCompatibility.wdWPJustification) = False
            .Compatibility(Word.WdCompatibility.wdLineWrapLikeWord6) = False
            .EmbedTrueTypeFonts = False
            .Password = ""
            .PrintFormsData = False
            .PrintPostScriptOverText = False
            .ReadOnlyRecommended = False
            .SaveFormsData = False
            .SaveSubsetFonts = False
            .ShowGrammaticalErrors = False
            .ShowSpellingErrors = False
            .WritePassword = ""
        End With
        'turn off err skipping for init
        On Error GoTo 0

        With CurDoc

            'page setup
            With .PageSetup
                .LineNumbering.Active = False
                .Orientation = Word.WdOrientation.wdOrientPortrait
                .PageWidth = WordApp.InchesToPoints(8.5)
                .PageHeight = WordApp.InchesToPoints(11)
                .FirstPageTray = Word.WdPaperTray.wdPrinterDefaultBin
                .OtherPagesTray = Word.WdPaperTray.wdPrinterDefaultBin
                .SectionStart = Word.WdSectionStart.wdSectionNewPage
                .OddAndEvenPagesHeaderFooter = False
                .DifferentFirstPageHeaderFooter = True
                .VerticalAlignment = Word.WdVerticalAlignment.wdAlignVerticalTop
                .SuppressEndnotes = False
            End With

        End With

        Call subDoMargins()

        With CurDoc
            ' init default styles

            ' set up styles here
            With .Styles.Item(PrintStyles.TitleStyle)
                With .Font
                    .Name = sFONT
                    .Bold = True
                    .Size = 28
                    .AllCaps = True
                    .Italic = True
                    .Underline = Word.WdUnderline.wdUnderlineNone
                    .Shadow = True
                End With
                With .ParagraphFormat
                    .Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft
                End With
            End With

            With .Styles.Item(PrintStyles.SubTitleStyle)
                With .Font
                    .Name = sFONT
                    .Bold = True
                    .Size = 14
                    .AllCaps = False
                    .Italic = False
                    .Underline = Word.WdUnderline.wdUnderlineNone
                End With
                With .ParagraphFormat
                    .Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft
                    'below new - tested with  word 2000
                    .SpaceBeforeAuto = False
                    .SpaceAfterAuto = False
                    .WidowControl = True
                    .KeepWithNext = True
                    .KeepTogether = True
                    .PageBreakBefore = False
                    .NoLineNumber = False
                    .Hyphenation = False
                    .OutlineLevel = Word.WdOutlineLevel.wdOutlineLevelBodyText
                    .CharacterUnitLeftIndent = 0
                    .CharacterUnitRightIndent = 0
                    .CharacterUnitFirstLineIndent = 0
                    .LineUnitBefore = 0
                    .LineUnitAfter = 0
                End With
            End With

            With CurDoc.Styles.Item(PrintStyles.ColumnHeaderStyle)
                With .Font
                    .Name = sFONT
                    .Bold = False
                    .Size = 12
                    .AllCaps = False
                    .Italic = False
                    .Underline = Word.WdUnderline.wdUnderlineNone
                End With
                With .ParagraphFormat
                    .Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft
                    .SpaceBefore = 6
                    .SpaceAfter = 4
                    .LineSpacingRule = Word.WdLineSpacing.wdLineSpaceSingle
                    'below new - tested with  word 2000
                    .SpaceBeforeAuto = False
                    .SpaceAfterAuto = False
                    .WidowControl = True
                    .KeepWithNext = True
                    .KeepTogether = True
                    .PageBreakBefore = False
                    .NoLineNumber = False
                    .Hyphenation = False
                    .OutlineLevel = Word.WdOutlineLevel.wdOutlineLevelBodyText
                    .CharacterUnitLeftIndent = 0
                    .CharacterUnitRightIndent = 0
                    .CharacterUnitFirstLineIndent = 0
                    .LineUnitBefore = 0
                    .LineUnitAfter = 0
                End With

            End With 'CurDoc.Styles(PrintStyles.ColumnHeaderStyle)

            With CurDoc.Styles.Item(PrintStyles.BodyStyle1)
                With .Font
                    .Name = sFONT
                    .Bold = False
                    .Size = 12
                    .AllCaps = False
                    .Italic = False
                    .Underline = Word.WdUnderline.wdUnderlineNone
                End With
                With .ParagraphFormat
                    .Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft
                    .SpaceBefore = 3
                    .SpaceAfter = 3
                    'below new - tested with  word 2000
                    .SpaceBeforeAuto = False
                    .SpaceAfterAuto = False
                    .WidowControl = True
                    .KeepWithNext = True
                    .KeepTogether = True
                    .PageBreakBefore = False
                    .NoLineNumber = False
                    .Hyphenation = False
                    .OutlineLevel = Word.WdOutlineLevel.wdOutlineLevelBodyText
                    .CharacterUnitLeftIndent = 0
                    .CharacterUnitRightIndent = 0
                    .CharacterUnitFirstLineIndent = 0
                    .LineUnitBefore = 0
                    .LineUnitAfter = 0
                End With
            End With

            With CurDoc.Styles.Item(PrintStyles.BodyStyle2) 'wingdings
                With .Font
                    .Name = sFONT1
                    .Bold = False
                    .Size = 12
                    .AllCaps = False
                    .Italic = False
                    .Underline = Word.WdUnderline.wdUnderlineNone
                End With
                With .ParagraphFormat
                    .Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft
                    .SpaceBefore = 3
                    .SpaceAfter = 3
                    'below new - tested with  word 2000
                    .SpaceBeforeAuto = False
                    .SpaceAfterAuto = False
                    .WidowControl = True
                    .KeepWithNext = True
                    .KeepTogether = True
                    .PageBreakBefore = False
                    .NoLineNumber = False
                    .Hyphenation = False
                    .OutlineLevel = Word.WdOutlineLevel.wdOutlineLevelBodyText
                    .CharacterUnitLeftIndent = 0
                    .CharacterUnitRightIndent = 0
                    .CharacterUnitFirstLineIndent = 0
                    .LineUnitBefore = 0
                    .LineUnitAfter = 0
                End With
            End With

            With CurDoc.Styles.Item(PrintStyles.FooterStyle)
                With .Font
                    .Name = sFONT
                    .Bold = False
                    .Size = 10
                    .AllCaps = False
                    .Italic = True
                    .Underline = Word.WdUnderline.wdUnderlineNone
                End With
                With .ParagraphFormat
                    .Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft
                End With
            End With

        End With

        ' var setup
        msSubTitles = "Subtitle"
        msTitle = "Title"
        mvarSpaceBetweenColumns = WordApp.InchesToPoints(0.15)
        mbAddISOBlurb = True
        mvarTitleStringForFirstTable = ""
        mbAbort = False

        ' setup table format (body tables only)
        Call SetTableFormat(Word.WdTableFormat.wdTableFormatSubtle1)

    End Sub

    Private Sub subDoHeader(Optional ByVal SectionNo As Short = 1)
        '**************************************************************************************************
        ' Description: The report title is inserted  as a header, the first page style is different
        '               from the rest of the pages.
        '
        ' Parameters: SectionNo is not used at present, each section in document can have different headers
        '
        ' Returns: None
        '**************************************************************************************************

        If mbAbort Then Exit Sub

        If WordApp.ActiveWindow.View.SplitSpecial <> Word.WdSpecialPane.wdPaneNone Then
            WordApp.ActiveWindow.Panes.Item(2).Close()
        End If

        With CurDoc.Sections.Item(SectionNo)
            .Headers.Item(Word.WdHeaderFooterIndex.wdHeaderFooterPrimary).Range.Style = CurDoc.Styles.Item(PrintStyles.SubTitleStyle).NameLocal
            .Headers.Item(Word.WdHeaderFooterIndex.wdHeaderFooterPrimary).Range.InsertAfter(msTitle)
            .Headers.Item(Word.WdHeaderFooterIndex.wdHeaderFooterFirstPage).Range.Style = CurDoc.Styles.Item(PrintStyles.TitleStyle).NameLocal
            .Headers.Item(Word.WdHeaderFooterIndex.wdHeaderFooterFirstPage).Range.InsertAfter(msTitle)
        End With

    End Sub

    Private Sub subDoFooter(Optional ByVal SectionNo As Short = 1)
        '**************************************************************************************************
        ' Description: The time,page and fanuc are inserted  as a footer, the first page footer could be different
        '               from the rest of the pages.
        '
        ' Parameters: SectionNo is not used at present, each section in document can have different footers
        '
        ' Returns: None
        '**************************************************************************************************
        Dim Myrange As Word.Range
        Dim MyTable As Word.Table
        Dim i As Integer
        Dim sText1 As String
        Dim sText2 As String
        Dim sText3 As String
        Dim sText4 As String

        If mbAbort Then Exit Sub

        If WordApp.ActiveWindow.View.SplitSpecial <> Word.WdSpecialPane.wdPaneNone Then
            WordApp.ActiveWindow.Panes.Item(2).Close()
        End If

        sText4 = gcsRM.GetString("csPAGE_X_OF_Y")

        sText1 = gcsRM.GetString("csFANUC_PRN")
        sText2 = gcsRM.GetString("csPRINT_PRN") & " "
        sText3 = gcsRM.GetString("csDOCUMENT_UNCONTROLLED")

        Const ADJ As Short = 30
        For i = Word.WdHeaderFooterIndex.wdHeaderFooterPrimary To Word.WdHeaderFooterIndex.wdHeaderFooterFirstPage

            Myrange = CurDoc.Sections.Item(SectionNo).Footers.Item(i).Range

            With Myrange

                .Style = CurDoc.Styles.Item(PrintStyles.FooterStyle)
                .Tables.Add(Range:=Myrange, NumRows:=1, NumColumns:=3)
                System.Windows.Forms.Application.DoEvents()
                MyTable = .Tables.Item(.Tables.Count)

                .InsertBefore(sText1)
                .Collapse()
                .Move(Unit:=Word.WdUnits.wdCell)
                'WordApp.NormalTemplate.AutoTextEntries("Last printed").Insert Myrange
                .InsertAfter(sText2 & Now)
                .ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter

                If mbAddISOBlurb Then
                    .MoveEnd(Unit:=Word.WdUnits.wdParagraph)
                    .InsertParagraphAfter()
                    .InsertAfter(sText3)
                    If mnOrientation = 0 Then ' WdOrientation.wdOrientPortrait
                        With MyTable.Columns
                            .Item(1).SetWidth(ColumnWidth:=(.Item(1).Width - ADJ), RulerStyle:=Word.WdRulerStyle.wdAdjustNone)
                            .Item(3).SetWidth(ColumnWidth:=(.Item(3).Width - ADJ), RulerStyle:=Word.WdRulerStyle.wdAdjustNone)
                            .Item(2).SetWidth(ColumnWidth:=(.Item(2).Width + (2 * ADJ)), RulerStyle:=Word.WdRulerStyle.wdAdjustNone)
                        End With
                    End If
                End If

                .Collapse()
                .Move(Unit:=Word.WdUnits.wdCell)
                'MSW 9/28/07 Office 2007 didn't like AutoTextEntries("Page X of Y")
                'WordApp.NormalTemplate.AutoTextEntries(sText4).Insert Myrange
                .Fields.Add(Myrange, Word.WdFieldType.wdFieldNumPages)
                .InsertAfter(" of ")
                .Fields.Add(Myrange, Word.WdFieldType.wdFieldPage, , True)
                .MoveEnd()
                .InsertAfter(" of ")
                .InsertBefore("Page ")
                .ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight

                With MyTable
                    .Borders.Enable = False
                    '.Columns.DistributeWidth
                End With
                'UPGRADE_NOTE: Object MyTable may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
                MyTable = Nothing

            End With

            'UPGRADE_NOTE: Object Myrange may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            Myrange = Nothing

            If i = Word.WdHeaderFooterIndex.wdHeaderFooterFirstPage Then Exit For

            If WordApp.ActiveWindow.ActivePane.View.Type = Word.WdViewType.wdNormalView Or WordApp.ActiveWindow.ActivePane.View.Type = Word.WdViewType.wdOutlineView Or WordApp.ActiveWindow.ActivePane.View.Type = Word.WdViewType.wdMasterView Then
                WordApp.ActiveWindow.ActivePane.View.Type = Word.WdViewTypeOld.wdPageView
            End If

            WordApp.ActiveWindow.ActivePane.View.SeekView = Word.WdSeekView.wdSeekMainDocument

        Next

    End Sub

    Private Sub subDoMargins()
        '**************************************************************************************************
        ' Description: Margins seem to change , call this after changing orientation
        '
        ' Parameters: None
        '
        ' Returns: None
        ' 10/4/00   gks moved footer on landscape
        '**************************************************************************************************
        With CurDoc.PageSetup
            .LeftMargin = WordApp.InchesToPoints(0.5)
            .RightMargin = WordApp.InchesToPoints(0.5)
            .BottomMargin = WordApp.InchesToPoints(1)
            .Gutter = WordApp.InchesToPoints(0)
            .HeaderDistance = WordApp.InchesToPoints(0.25)
            .TopMargin = WordApp.InchesToPoints(0.75)
            If .Orientation = Word.WdOrientation.wdOrientPortrait Then
                .FooterDistance = WordApp.InchesToPoints(0.5)
            Else
                .FooterDistance = WordApp.InchesToPoints(0.2)
            End If
            .MirrorMargins = False
        End With

    End Sub

    Private Sub subDoSubTitles(ByRef HomeOnThe As Word.Range)
        '**************************************************************************************************
        ' Description: Subtitle string is put into a table for locating and aligning
        '
        ' Parameters: range on the currdoc
        '
        ' Returns: None
        '**************************************************************************************************
        Dim MyTable As Word.Table
        Dim nNumCols As Short

        If mbAbort Then Exit Sub

        If CurDoc.PageSetup.Orientation = Word.WdOrientation.wdOrientLandscape Then
            nNumCols = 3
        Else
            nNumCols = 2
        End If

        With HomeOnThe
            .Style = CurDoc.Styles.Item(PrintStyles.SubTitleStyle)
            .InsertAfter(SubTitles)
            ' put subtitles in table
            .ConvertToTable(Separator:=ColumnSeparatorCharacter, NumColumns:=nNumCols)
            MyTable = CurDoc.Tables.Item(CurDoc.Tables.Count)
            With MyTable
                .Borders.Enable = False
                ' give some space
                .Rows.SpaceBetweenColumns = WordApp.InchesToPoints(0.5)
                .Columns.AutoFit()
            End With
            .Collapse(Direction:=Word.WdCollapseDirection.wdCollapseEnd)
            .Style = CurDoc.Styles.Item(PrintStyles.BodyStyle1)
            .InsertParagraph()
            .InsertParagraph()
            .SetRange(.End - 1, .End)

            .Collapse(Direction:=Word.WdCollapseDirection.wdCollapseEnd)
        End With

    End Sub

    Shared Sub subCheckStringLength(ByRef rString As String)
        '**************************************************************************************************
        ' Description: tables are limited to 63 columns by word, check string and truncate if necessary
        '
        ' Parameters: rstring comes in as tab delimited string
        '
        ' Returns: rstring goes out with max 62 tabs
        '**************************************************************************************************
        Dim sTmp() As String

        System.Diagnostics.Debug.Assert(Len(rString) > 0, "")

        sTmp = Split(rString, vbTab)
        If UBound(sTmp) > 62 Then
            ReDim Preserve sTmp(62)
            rString = Join(sTmp, vbTab)
        End If

    End Sub

    Private Sub subJustifyColumns(ByRef rTable As Word.Table, ByRef nCol As Short, ByRef nType As Word.WdParagraphAlignment)
        '**************************************************************************************************
        ' Description: Apply format to desired column
        '
        ' Parameters: table ( body data starts at table 2), column , and type
        '
        ' Returns: None
        '**************************************************************************************************
        'Dim tmpCell As Word.Cell


        If mbAbort Then Exit Sub

        ' check range
        If nCol > rTable.Columns.Count Then Exit Sub

        rTable.Columns.Item(nCol).Select()
        System.Windows.Forms.Application.DoEvents()
        System.Windows.Forms.Application.DoEvents()
        WordApp.Selection.ParagraphFormat.Alignment = nType
        WordApp.Selection.Collapse(Direction:=Word.WdCollapseDirection.wdCollapseEnd)
        rTable.Columns(nCol).AutoFit()
        '  .Rows.Item(1).Cells.AutoFit()

    End Sub

    Private Function CheckTableForColumns(ByVal vStartCol As Integer, ByVal vEndCol As Integer, ByVal vTableNum As Integer) As Boolean
        '**************************************************************************************************
        ' Description: check table to make sure desired columns are there
        '
        ' Parameters: the range of columns to check for and the table to check
        '
        ' Returns: true if successfull
        '**************************************************************************************************

        Dim nColCount As Integer

        CheckTableForColumns = False
        ' does table exist?
        If CurDoc.Tables.Count < vTableNum Then
            Beep()
            System.Diagnostics.Debug.Assert(CurDoc.Tables.Count >= vTableNum, "")
            Exit Function
        End If

        nColCount = CurDoc.Tables.Item(vTableNum).Columns.Count

        If vStartCol > nColCount Then
            Beep()
            System.Diagnostics.Debug.Assert(vStartCol <= nColCount, "")
            Exit Function
        End If

        If vEndCol > nColCount Then
            Beep()
            System.Diagnostics.Debug.Assert(vEndCol <= nColCount, "")
            Exit Function
        End If

        If vStartCol > vEndCol Then
            Beep()
            System.Diagnostics.Debug.Assert(vStartCol <= vEndCol, "")
            Exit Function
        End If
        'limited to 63 columns
        If (vStartCol < 0) Or (vStartCol > 63) Then
            Beep()
            System.Diagnostics.Debug.Assert(((vStartCol >= 0) And (vStartCol <= 63)), "")
            Exit Function
        End If

        If (vEndCol < 0) Or (vEndCol > 63) Then
            Beep()
            System.Diagnostics.Debug.Assert(((vEndCol >= 0) And (vEndCol <= 63)), "")
            Exit Function
        End If

        CheckTableForColumns = True

    End Function


    Friend WriteOnly Property ColumnHeadersString() As String
        Set(ByVal Value As String)
            'used when assigning a value to the property, on the left side of an assignment.
            'Syntax: X.ColumnHeadersString = 5
            msColumnHeadersString = Value
            subCheckStringLength((msColumnHeadersString))

        End Set
    End Property


    Friend Property SubTitles() As String
        Get
            'used when retrieving value of a property, on the right side of an assignment.
            'Syntax: Debug.Print X.SubTitles
            SubTitles = msSubTitles
        End Get
        Set(ByVal Value As String)
            'used when assigning a value to the property, on the left side of an assignment.
            'Syntax: X.SubTitles = 5
            msSubTitles = Value
        End Set
    End Property


    Friend Property Title() As String
        Get
            'used when retrieving value of a property, on the right side of an assignment.
            'Syntax: Debug.Print X.Title
            Title = msTitle
        End Get
        Set(ByVal Value As String)
            'used when assigning a value to the property, on the left side of an assignment.
            'Syntax: X.Title = 5
            msTitle = Value
        End Set
    End Property

    Friend WriteOnly Property Orientation() As Word.WdOrientation
        Set(ByVal Value As Word.WdOrientation)
            'used when assigning a value to the property, on the left side of an assignment.
            'Syntax: X.Orientation = 5
            mnOrientation = Value
        End Set
    End Property

    Friend WriteOnly Property SpaceBetweenColumns() As Single
        Set(ByVal Value As Single)
            'used when assigning a value to the property, on the left side of an assignment.
            'Syntax: X.SpaceBetweenColumns = 5
            If Value < 0.1 Then Value = 0.1
            mvarSpaceBetweenColumns = WordApp.InchesToPoints(Value)
        End Set
    End Property

    Friend WriteOnly Property AddISOBlurb() As Boolean
        Set(ByVal Value As Boolean)
            'used when assigning a value to the property, on the left side of an assignment.
            'Syntax: X.AddISOBlurb = 5
            mbAddISOBlurb = Value
        End Set
    End Property


    Friend Property ColumnSeparatorCharacter() As String 'Object
        Get
            'get the character used to separate the tables into columns
            'UPGRADE_WARNING: Couldn't resolve default property of object mvntSeparatorChar. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            'UPGRADE_WARNING: Couldn't resolve default property of object ColumnSeparatorCharacter. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            ColumnSeparatorCharacter = mvntSeparatorChar

        End Get
        Set(ByVal Value As String) 'Object)
            'set the character used to separate the tables into columns
            'UPGRADE_WARNING: Couldn't resolve default property of object vData. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            'UPGRADE_WARNING: Couldn't resolve default property of object mvntSeparatorChar. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            mvntSeparatorChar = Value

        End Set
    End Property

    Shared ReadOnly Property CheckBoxNotChecked() As String
        Get
            'used when retrieving value of a property, on the right side of an assignment.
            'Syntax: Debug.Print X.CheckBoxNotChecked
            CheckBoxNotChecked = ChrW(168)
        End Get
    End Property

    Shared ReadOnly Property CheckBoxChecked() As String
        Get
            'used when retrieving value of a property, on the right side of an assignment.
            'Syntax: Debug.Print X.CheckBoxChecked
            CheckBoxChecked = ChrW(254)
        End Get
    End Property

    Friend WriteOnly Property BodyTextStrings() As String
        Set(ByVal Value As String)
            '**************************************************************************************************
            ' Description: each one of these is a row in the report
            '
            ' Parameters: vData - tab delimited string
            '
            ' Returns: None
            '**************************************************************************************************
            Dim sTmp As String

            If mbAbort Then Exit Property

            sTmp = Value
            'max 63 cols
            subCheckStringLength((sTmp))
            ' add to collection
            colBodyTextStrings.Add(sTmp)

        End Set
    End Property

    Friend ReadOnly Property Datatable(ByVal vIndex As Short) As Word.Table
        Get
            'This is used to expose the tables to the calling app for those who can't leave well
            ' enough alone. use at your own risk.

            If mbAbort Then
                Return Nothing
            End If


            On Error GoTo DataTableErr

            If (vIndex > 0) And (vIndex <= CurDoc.Tables.Count) Then
                Datatable = CurDoc.Tables.Item(vIndex)
            Else
                'UPGRADE_NOTE: Object Datatable may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
                Datatable = Nothing
            End If

            Exit Property
DataTableErr:
            'UPGRADE_NOTE: Object Datatable may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            Datatable = Nothing

        End Get
    End Property

    Friend WriteOnly Property TitleStringForFirstTable() As String
        Set(ByVal Value As String)
            'used when assigning a value to the property, on the left side of an assignment.
            'Syntax: X.TitleStringForFirstTable = 5
            mvarTitleStringForFirstTable = Value
        End Set
    End Property

    'UPGRADE_NOTE: Class_Initialize was upgraded to Class_Initialize_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Private Sub Class_Initialize_Renamed()
        Call subInitDefaultData()
    End Sub
    Friend Sub New()
        MyBase.New()
        mbDebugMode = mPWCommon.InIDE
        Class_Initialize_Renamed()
    End Sub

    'UPGRADE_NOTE: Class_Terminate was upgraded to Class_Terminate_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Private Sub Class_Terminate_Renamed()
        Try

            'UPGRADE_NOTE: Object colBodyTextStrings may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            colBodyTextStrings = Nothing
            'UPGRADE_NOTE: Object colColumnHeaders may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            colColumnHeaders = Nothing
            'UPGRADE_NOTE: Object colBodyText may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            colBodyText = Nothing
            'UPGRADE_NOTE: Object colSectionTitleStrings may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            colSectionTitleStrings = Nothing

            'todo this barfs
            ''WordApp.Application.Quit()
            ''''WordApp.Quit()
            ''''UPGRADE_NOTE: Object CurDoc may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            ''CurDoc = Nothing
            ''''UPGRADE_NOTE: Object WordApp may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            ''WordApp = Nothing

            System.Windows.Forms.Application.DoEvents()
        Catch ex As Exception
            Trace.WriteLine("Module: clsPrint, Routine: Class_Terminate_Renamed, Error: " & ex.Message)
            Trace.WriteLine("Module: clsPrint, Routine: Class_Terminate_Renamed, StackTrace: " & ex.StackTrace)
        End Try

    End Sub
    Protected Overrides Sub Finalize()
        Class_Terminate_Renamed()
        MyBase.Finalize()
    End Sub
End Class